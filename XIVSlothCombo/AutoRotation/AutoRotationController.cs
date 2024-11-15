using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVSlothCombo.Attributes;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using Action = Lumina.Excel.Sheets.Action;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.AutoRotation;

internal static unsafe class AutoRotationController
{
    private static long LastHealAt;

    private static Func<IBattleChara?, bool> RezQuery => x =>
        x.IsDead && CustomComboFunctions.FindEffectOnMember(2648, x) == null &&
        CustomComboFunctions.FindEffectOnMember(148, x) == null && x.IsTargetable;

    internal static void Run()
    {
        AutoRotationConfig cfg = Service.Configuration.RotationConfig;

        if (!cfg.Enabled || !Player.Available || Svc.Condition[ConditionFlag.Mounted])
            return;

        if (Player.Object.CurrentCastTime > 0) return;

        if (!EzThrottler.Throttle("AutoRotController", 150))
            return;

        if (cfg.HealerSettings.PreEmptiveHoT && Player.Job is Job.CNJ or Job.WHM or Job.AST)
            PreEmptiveHot();

        bool combatBypass =
            (cfg.BypassQuest && DPSTargeting.BaseSelection.Any(x => CustomComboFunctions.IsQuestMob(x))) ||
            (cfg.BypassFATE && CustomComboFunctions.InFATE());

        if (cfg.InCombatOnly &&
            (!CustomComboFunctions.GetPartyMembers().Any(x => x.Struct()->InCombat) ||
             CustomComboFunctions.PartyEngageDuration().TotalSeconds < cfg.CombatDelay) && !combatBypass)
            return;

        if (Player.Job is Job.SGE && cfg.HealerSettings.ManageKardia)
            UpdateKardiaTarget();

        IGameObject? healTarget = Player.Object.GetRole() is CombatRole.Healer
            ? AutoRotationHelper.GetSingleTarget(cfg.HealerRotationMode)
            : null;
        bool aoeheal = Player.Object.GetRole() is CombatRole.Healer && HealerTargeting.CanAoEHeal();

        if (Player.Object.GetRole() is CombatRole.Healer)
        {
            bool needsHeal = healTarget != null || aoeheal;

            if (cfg.HealerSettings.AutoCleanse && !needsHeal)
            {
                CleanseParty();

                if (CustomComboFunctions.GetPartyMembers().Any(x => CustomComboFunctions.HasCleansableDebuff(x)))
                    return;
            }

            if (cfg.HealerSettings.AutoRez)
            {
                RezParty();

                if (CustomComboFunctions.GetPartyMembers().Any(RezQuery))
                    return;
            }
        }

        foreach (KeyValuePair<CustomComboPreset, bool> preset in Service.Configuration.AutoActions
                     .OrderByDescending(x => Presets.Attributes[x.Key].AutoAction.IsHeal)
                     .ThenByDescending(x => Presets.Attributes[x.Key].AutoAction.IsAoE))
        {
            if (!CustomComboFunctions.IsEnabled(preset.Key) || !preset.Value) continue;

            Presets.PresetAttributes attributes = Presets.Attributes[preset.Key];
            AutoActionAttribute? action = attributes.AutoAction;
            uint gameAct = attributes.ReplaceSkill.ActionIDs.First();
            Action sheetAct = Svc.Data.GetExcelSheet<Action>().GetRow(gameAct);
            byte classToJob = CustomComboFunctions.JobIDs.ClassToJob((byte)Player.Job);

            if ((byte)Player.Job != attributes.CustomComboInfo.JobID && classToJob != attributes.CustomComboInfo.JobID)
                continue;

            uint outAct = AutoRotationHelper.InvokeCombo(preset.Key, attributes);

            if (!CustomComboFunctions.ActionReady(gameAct))
                continue;

            if (action.IsHeal)
            {
                if (!AutomateHealing(preset.Key, attributes, gameAct) && Svc.Targets.Target != null &&
                    !Svc.Targets.Target.IsHostile() && Environment.TickCount64 > LastHealAt + 1000)
                    Svc.Targets.Target = null;

                if ((healTarget != null && !action.IsAoE) || (aoeheal && action.IsAoE))
                    return;

                continue;
            }

            if (Player.Object.GetRole() is CombatRole.Tank)
            {
                AutomateTanking(preset.Key, attributes, gameAct);

                continue;
            }

            AutomateDPS(preset.Key, attributes, gameAct);
        }
    }

    private static void PreEmptiveHot()
    {
        if (CustomComboFunctions.InCombat())
            return;

        if (Svc.Targets.FocusTarget is null)
            return;

        ushort regenBuff = Player.Job switch
        {
            Job.AST => AST.Buffs.AspectedBenefic,
            Job.CNJ or Job.WHM => WHM.Buffs.Regen,
            var _ => 0
        };

        uint regenSpell = Player.Job switch
        {
            Job.AST => AST.AspectedBenefic,
            Job.CNJ or Job.WHM => WHM.Regen,
            var _ => 0
        };

        if (regenSpell != 0 && Svc.Targets.FocusTarget != null &&
            (!CustomComboFunctions.MemberHasEffect(regenBuff, Svc.Targets.FocusTarget, true, out Status? regen) ||
             regen?.RemainingTime <= 5f))
        {
            IEnumerable<IBattleNpc> query = Svc.Objects.Where(x => !x.IsDead && x.ObjectKind == ObjectKind.BattleNpc)
                .Cast<IBattleNpc>().Where(x => x.BattleNpcKind == BattleNpcSubKind.Enemy && x.IsTargetable);

            if (!query.Any())
                return;

            if (query.Min(x => CustomComboFunctions.GetTargetDistance(x, Svc.Targets.FocusTarget)) <= 30)
            {
                uint spell = ActionManager.Instance()->GetAdjustedActionId(regenSpell);

                if (Svc.Targets.FocusTarget.IsDead)
                    return;

                if (!CustomComboFunctions.ActionReady(spell))
                    return;

                if (ActionManager.CanUseActionOnTarget(spell, Svc.Targets.FocusTarget.Struct()) &&
                    !ActionWatching.OutOfRange(spell, Player.Object, Svc.Targets.FocusTarget))
                    ActionManager.Instance()->UseAction(ActionType.Action, regenSpell,
                        Svc.Targets.FocusTarget.GameObjectId);
            }
        }
    }

    private static void RezParty()
    {
        uint resSpell = Player.Job switch
        {
            Job.CNJ or Job.WHM => WHM.Raise,
            Job.SCH => SCH.Resurrection,
            Job.AST => AST.Ascend,
            Job.SGE => SGE.Egeiro,
            var _ => throw new NotImplementedException()
        };

        if (Player.Object.CurrentMp >= CustomComboFunctions.GetResourceCost(resSpell))
            if (CustomComboFunctions.GetPartyMembers().Where(RezQuery)
                .FindFirst(x => x is not null, out IBattleChara? member))
            {
                if (CustomComboFunctions.ActionReady(All.Swiftcast))
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, All.Swiftcast);

                    return;
                }

                if (!CustomComboFunctions.IsMoving || CustomComboFunctions.HasEffect(All.Buffs.Swiftcast))
                    ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.GameObjectId);
            }
    }

    private static void CleanseParty()
    {
        if (ActionManager.Instance()->QueuedActionId == All.Esuna)
            ActionManager.Instance()->QueuedActionId = 0;

        if (CustomComboFunctions.GetPartyMembers()
                .FindFirst(x => CustomComboFunctions.HasCleansableDebuff(x), out IBattleChara? member) &&
            !CustomComboFunctions.IsMoving)
            ActionManager.Instance()->UseAction(ActionType.Action, All.Esuna, member.GameObjectId);
    }

    private static void UpdateKardiaTarget()
    {
        if (!CustomComboFunctions.LevelChecked(SGE.Kardia)) return;
        if (CustomComboFunctions.CombatEngageDuration().TotalSeconds < 3) return;

        foreach (IBattleChara? member in CustomComboFunctions.GetPartyMembers()
                     .OrderByDescending(x => x.GetRole() is CombatRole.Tank))
        {
            if (Service.Configuration.RotationConfig.HealerSettings.KardiaTanksOnly &&
                member.GetRole() is not CombatRole.Tank &&
                CustomComboFunctions.FindEffectOnMember(3615, member) is null) continue;

            int enemiesTargeting = Svc.Objects
                .Where(x => x.IsTargetable && x.IsHostile() && x.TargetObjectId == member.GameObjectId).Count();

            if (enemiesTargeting > 0 && CustomComboFunctions.FindEffectOnMember(SGE.Buffs.Kardion, member) is null)
            {
                ActionManager.Instance()->UseAction(ActionType.Action, SGE.Kardia, member.GameObjectId);

                return;
            }
        }
    }

    private static bool AutomateDPS(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
    {
        if (Svc.Targets.Target != null && !Svc.Targets.Target.IsHostile()) return false;

        DPSRotationMode mode = Service.Configuration.RotationConfig.DPSRotationMode;

        if (attributes.AutoAction.IsAoE) return AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);

        return AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
    }

    private static bool AutomateTanking(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
    {
        DPSRotationMode mode = Service.Configuration.RotationConfig.DPSRotationMode;

        if (attributes.AutoAction.IsAoE) return AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);

        return AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
    }

    private static bool AutomateHealing(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
    {
        HealerRotationMode mode = Service.Configuration.RotationConfig.HealerRotationMode;

        if (Player.Object.IsCasting()) return false;

        if (attributes.AutoAction.IsAoE)
        {
            if (Environment.TickCount64 < LastHealAt + 1500) return false;

            bool ret = AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);

            return ret;
        }
        else
        {
            if (Environment.TickCount64 < LastHealAt + 1500) return false;

            bool ret = AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);

            return ret;
        }
    }

    public static class AutoRotationHelper
    {
        public static IGameObject? GetSingleTarget(Enum rotationMode)
        {
            if (rotationMode is DPSRotationMode dpsmode)
            {
                IGameObject? target = dpsmode switch
                {
                    DPSRotationMode.Manual => Svc.Targets.Target,
                    DPSRotationMode.Highest_Max => DPSTargeting.GetHighestMaxTarget(),
                    DPSRotationMode.Lowest_Max => DPSTargeting.GetLowestMaxTarget(),
                    DPSRotationMode.Highest_Current => DPSTargeting.GetHighestCurrentTarget(),
                    DPSRotationMode.Lowest_Current => DPSTargeting.GetLowestCurrentTarget(),
                    DPSRotationMode.Tank_Target => DPSTargeting.GetTankTarget(),
                    DPSRotationMode.Nearest => DPSTargeting.GetNearestTarget(),
                    DPSRotationMode.Furthest => DPSTargeting.GetFurthestTarget(),
                    var _ => throw new ArgumentOutOfRangeException()
                };

                return target;
            }

            if (rotationMode is HealerRotationMode healermode)
            {
                if (Player.Object.GetRole() != CombatRole.Healer) return null;

                IGameObject? target = healermode switch
                {
                    HealerRotationMode.Manual => HealerTargeting.ManualTarget(),
                    HealerRotationMode.Highest_Current => HealerTargeting.GetHighestCurrent(),
                    HealerRotationMode.Lowest_Current => HealerTargeting.GetLowestCurrent(),
                    var _ => throw new ArgumentOutOfRangeException()
                };

                return target;
            }

            return null;
        }

        public static bool ExecuteAoE(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes,
            uint gameAct)
        {
            if (attributes.AutoAction.IsHeal)
            {
                uint outAct = InvokeCombo(preset, attributes, Player.Object);

                if (!CustomComboFunctions.ActionReady(outAct))
                    return false;

                if (HealerTargeting.CanAoEHeal(outAct))
                {
                    int castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);

                    if (CustomComboFunctions.IsMoving && castTime > 0)
                        return false;

                    bool ret = ActionManager.Instance()->UseAction(ActionType.Action, outAct);

                    if (ret)
                        LastHealAt = Environment.TickCount64 + castTime;

                    return ret;
                }
            }
            else
            {
                uint outAct = InvokeCombo(preset, attributes, Player.Object);

                if (!CustomComboFunctions.ActionReady(outAct))
                    return false;

                IGameObject? target = GetSingleTarget(mode);
                Action sheet = Svc.Data.GetExcelSheet<Action>().GetRow(outAct);
                bool mustTarget = sheet.CanTargetHostile;
                int numEnemies = CustomComboFunctions.NumberOfEnemiesInRange(gameAct, target);

                if (numEnemies >= Service.Configuration.RotationConfig.DPSSettings.DPSAoETargets)
                {
                    bool switched = SwitchOnDChole(attributes, outAct, ref target);
                    int castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);

                    if (CustomComboFunctions.IsMoving && castTime > 0)
                        return false;

                    if (mustTarget)
                        Svc.Targets.Target = target;

                    return ActionManager.Instance()->UseAction(ActionType.Action, gameAct,
                        (mustTarget && target != null) || switched ? target.GameObjectId : Player.Object.GameObjectId);
                }
            }

            return false;
        }

        public static bool ExecuteST(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes,
            uint gameAct)
        {
            IGameObject? target = GetSingleTarget(mode);

            if (target is null)
                return false;

            uint outAct = InvokeCombo(preset, attributes, target);
            int castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);

            if (CustomComboFunctions.IsMoving && castTime > 0)
                return false;

            bool switched = SwitchOnDChole(attributes, outAct, ref target);

            bool areaTargeted = Svc.Data.GetExcelSheet<Action>().GetRow(outAct).TargetArea;
            bool inRange = ActionManager.GetActionInRangeOrLoS(outAct, Player.GameObject, target.Struct()) != 562;
            bool canUseTarget = ActionManager.CanUseActionOnTarget(outAct, target.Struct());
            bool canUseSelf = ActionManager.CanUseActionOnTarget(outAct, Player.GameObject);

            bool canUse = canUseSelf || canUseTarget || areaTargeted;

            if (canUse && inRange)
            {
                Svc.Targets.Target = target;

                bool ret = ActionManager.Instance()->UseAction(ActionType.Action, outAct,
                    canUseTarget ? target.GameObjectId : Player.Object.GameObjectId);

                if (mode is HealerRotationMode && ret)
                    LastHealAt = Environment.TickCount64 + castTime;

                return ret;
            }

            return false;
        }

        private static bool SwitchOnDChole(Presets.PresetAttributes attributes, uint outAct, ref IGameObject? newtarget)
        {
            if (outAct is SGE.Druochole && !attributes.AutoAction.IsHeal)
                if (CustomComboFunctions.GetPartyMembers()
                    .Where(x => CustomComboFunctions.FindEffectOnMember(SGE.Buffs.Kardion, x) is not null)
                    .TryGetFirst(out newtarget))
                    return true;

            return false;
        }

        public static uint InvokeCombo(CustomComboPreset preset, Presets.PresetAttributes attributes,
            IGameObject? optionalTarget = null)
        {
            uint outAct = attributes.ReplaceSkill.ActionIDs.FirstOrDefault();

            foreach (uint actToCheck in attributes.ReplaceSkill.ActionIDs)
            {
                CustomCombo? customCombo = Service.IconReplacer.CustomCombos.FirstOrDefault(x => x.Preset == preset);

                if (customCombo != null)
                    if (customCombo.TryInvoke(actToCheck, (byte)Player.Level, ActionManager.Instance()->Combo.Action,
                            ActionManager.Instance()->Combo.Timer, out uint changedAct, optionalTarget))
                    {
                        outAct = changedAct;

                        break;
                    }
            }

            return outAct;
        }
    }

    public class DPSTargeting
    {
        public static IEnumerable<IGameObject> BaseSelection =>
            Svc.Objects.Any(x =>
                x is IBattleChara chara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                CustomComboFunctions.IsInLineOfSight(x) && IsPriority(x))
                ? Svc.Objects.Where(x =>
                    x is IBattleChara chara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                    x.IsTargetable && CustomComboFunctions.IsInLineOfSight(x) && IsPriority(x))
                : Svc.Objects.Where(x =>
                    x is IBattleChara chara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                    x.IsTargetable && CustomComboFunctions.IsInLineOfSight(x));

        private static bool IsPriority(IGameObject x)
        {
            bool isFate = Service.Configuration.RotationConfig.DPSSettings.FATEPriority && x.Struct()->FateId != 0 &&
                          CustomComboFunctions.InFATE();

            bool isQuest = Service.Configuration.RotationConfig.DPSSettings.QuestPriority &&
                           CustomComboFunctions.IsQuestMob(x);

            if (Player.Object.GetRole() is CombatRole.Tank && x.TargetObjectId != Player.Object.GameObjectId)
                return true;

            return isFate || isQuest;
        }

        public static IGameObject? GetTankTarget()
        {
            IBattleChara? tank = CustomComboFunctions.GetPartyMembers().Where(x => x.GetRole() == CombatRole.Tank)
                .FirstOrDefault();

            if (tank == null)
                return null;

            return tank.TargetObject;
        }

        public static IGameObject? GetNearestTarget()
        {
            return BaseSelection.OrderBy(x => CustomComboFunctions.GetTargetDistance(x)).FirstOrDefault();
        }

        public static IGameObject? GetFurthestTarget()
        {
            return BaseSelection.OrderByDescending(x => CustomComboFunctions.GetTargetDistance(x)).FirstOrDefault();
        }

        public static IGameObject? GetLowestCurrentTarget()
        {
            return BaseSelection.OrderBy(x => (x as IBattleChara).CurrentHp).FirstOrDefault();
        }

        public static IGameObject? GetHighestCurrentTarget()
        {
            return BaseSelection.OrderByDescending(x => (x as IBattleChara).CurrentHp).FirstOrDefault();
        }

        public static IGameObject? GetLowestMaxTarget()
        {
            return BaseSelection.OrderBy(x => (x as IBattleChara).MaxHp)
                .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x))
                .ThenBy(x => CustomComboFunctions.GetTargetDistance(x)).FirstOrDefault();
        }

        public static IGameObject? GetHighestMaxTarget()
        {
            return BaseSelection.OrderByDescending(x => (x as IBattleChara).MaxHp)
                .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
        }
    }

    public static class HealerTargeting
    {
        internal static IGameObject? ManualTarget()
        {
            if (Svc.Targets.Target == null) return null;

            IGameObject? t = Svc.Targets.Target;

            bool goodToHeal = CustomComboFunctions.GetTargetHPPercent(t) <= (TargetHasRegen(t)
                ? Service.Configuration.RotationConfig.HealerSettings.SingleTargetRegenHPP
                : Service.Configuration.RotationConfig.HealerSettings.SingleTargetHPP);

            if (goodToHeal) return t;

            return null;
        }

        internal static IGameObject? GetHighestCurrent()
        {
            if (CustomComboFunctions.GetPartyMembers().Count == 0) return Player.Object;

            IBattleChara? target = CustomComboFunctions.GetPartyMembers()
                .Where(x => !x.IsDead && x.IsTargetable && CustomComboFunctions.GetTargetHPPercent(x) <=
                    (TargetHasRegen(x)
                        ? Service.Configuration.RotationConfig.HealerSettings.SingleTargetRegenHPP
                        : Service.Configuration.RotationConfig.HealerSettings.SingleTargetHPP))
                .OrderByDescending(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();

            return target;
        }

        internal static IGameObject? GetLowestCurrent()
        {
            if (CustomComboFunctions.GetPartyMembers().Count == 0) return Player.Object;

            IBattleChara? target = CustomComboFunctions.GetPartyMembers()
                .Where(x => !x.IsDead && x.IsTargetable && CustomComboFunctions.GetTargetHPPercent(x) <=
                    (TargetHasRegen(x)
                        ? Service.Configuration.RotationConfig.HealerSettings.SingleTargetRegenHPP
                        : Service.Configuration.RotationConfig.HealerSettings.SingleTargetHPP))
                .OrderBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();

            return target;
        }

        internal static bool CanAoEHeal(uint outAct = 0)
        {
            IEnumerable<IBattleChara?> members = CustomComboFunctions.GetPartyMembers().Where(x =>
                (outAct == 0
                    ? CustomComboFunctions.GetTargetDistance(x) <= 15
                    : CustomComboFunctions.InActionRange(outAct, x)) && CustomComboFunctions.GetTargetHPPercent(x) <=
                Service.Configuration.RotationConfig.HealerSettings.AoETargetHPP);

            if (members.Count() < Service.Configuration.RotationConfig.HealerSettings.AoEHealTargetCount)
                return false;

            return true;
        }

        private static bool TargetHasRegen(IGameObject? target)
        {
            ushort regenBuff = JobID switch
            {
                AST.JobID => AST.Buffs.AspectedBenefic,
                WHM.JobID => WHM.Buffs.Regen,
                var _ => 0
            };

            return CustomComboFunctions.FindEffectOnMember(regenBuff, target) != null;
        }
    }

    public static class TankTargeting
    {
        public static IGameObject? GetLowestCurrentTarget()
        {
            return Svc.Objects.Where(x =>
                    x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                    x.IsTargetable)
                .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenBy(x => (x as IBattleChara).CurrentHp)
                .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
        }

        public static IGameObject? GetHighestCurrentTarget()
        {
            return Svc.Objects.Where(x =>
                    x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                    x.IsTargetable)
                .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenByDescending(x => (x as IBattleChara).CurrentHp)
                .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
        }

        public static IGameObject? GetLowestMaxTarget()
        {
            return Svc.Objects.Where(x =>
                    x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                    x.IsTargetable)
                .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .OrderBy(x => (x as IBattleChara).MaxHp)
                .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
        }

        public static IGameObject? GetHighestMaxTarget()
        {
            return Svc.Objects.Where(x =>
                    x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead &&
                    x.IsTargetable)
                .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenByDescending(x => (x as IBattleChara).MaxHp)
                .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
        }
    }
}