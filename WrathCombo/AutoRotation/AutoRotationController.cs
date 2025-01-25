using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using Action = Lumina.Excel.Sheets.Action;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using WrathCombo.CustomComboNS.Functions;

#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace WrathCombo.AutoRotation
{
    internal unsafe static class AutoRotationController
    {
        public static AutoRotationConfigIPCWrapper? cfg;

        static long LastHealAt = 0;
        static long LastRezAt = 0;

        static bool _lockedST = false;
        static bool _lockedAoE = false;

        static DateTime? TimeToHeal;

        static Func<WrathPartyMember, bool> RezQuery => x => x.BattleChara.IsDead && FindEffectOnMember(2648, x.BattleChara) == null && FindEffectOnMember(148, x.BattleChara) == null && x.BattleChara.IsTargetable && TimeSpentDead(x.BattleChara.GameObjectId).TotalSeconds > 2;

        public static bool LockedST
        {
            get => _lockedST; 
            set
            {
                if (_lockedST != value)
                    Svc.Log.Debug($"Locked ST updated to {value}");

                _lockedST = value;
            }
        }
        public static bool LockedAoE
        {
            get => _lockedAoE; 
            set
            {
                if (_lockedAoE != value)
                    Svc.Log.Debug($"Locked AoE updated to {value}");

                _lockedAoE = value;
            }
        }

        internal static void Run()
        {
            cfg ??= new AutoRotationConfigIPCWrapper(Service.Configuration.RotationConfig);

            if (!cfg.Enabled || !Player.Available || Player.Object.IsDead || Svc.Condition[ConditionFlag.Mounted] || !EzThrottler.Throttle("Autorot", cfg.Throttler))
                return;

            if (cfg.HealerSettings.PreEmptiveHoT && Player.Job is Job.CNJ or Job.WHM or Job.AST)
                PreEmptiveHot();

            bool combatBypass = (cfg.BypassQuest && DPSTargeting.BaseSelection.Any(x => IsQuestMob(x))) || (cfg.BypassFATE && InFATE());

            if (cfg.InCombatOnly && (!GetPartyMembers().Any(x => x.BattleChara.Struct()->InCombat) || PartyEngageDuration().TotalSeconds < cfg.CombatDelay) && !combatBypass)
                return;

            if (Player.Job is Job.SGE && cfg.HealerSettings.ManageKardia)
                UpdateKardiaTarget();

            var autoActions = Presets.GetJobAutorots;
            var healTarget = Player.Object.GetRole() is CombatRole.Healer ? AutoRotationHelper.GetSingleTarget(cfg.HealerRotationMode) : null;
            var aoeheal = Player.Object.GetRole() is CombatRole.Healer && HealerTargeting.CanAoEHeal() && autoActions.Any(x => x.Key.Attributes().AutoAction.IsHeal && x.Key.Attributes().AutoAction.IsAoE);
            bool needsHeal = ((healTarget != null && autoActions.Any(x => x.Key.Attributes().AutoAction.IsHeal && !x.Key.Attributes().AutoAction.IsAoE)) || aoeheal) && Player.Object.GetRole() is CombatRole.Healer;

            if (needsHeal && TimeToHeal is null)
                TimeToHeal = DateTime.Now;

            if (!needsHeal)
                TimeToHeal = null;

            uint _ = 0;
            bool actCheck = autoActions.Any(x => x.Key.Attributes().AutoAction.IsHeal && ActionReady(AutoRotationHelper.InvokeCombo(x.Key, x.Key.Attributes()!, ref _)));
            bool canHeal = TimeToHeal is null ? false : (DateTime.Now - TimeToHeal.Value).TotalSeconds >= cfg.HealerSettings.HealDelay && actCheck;

            if (Player.Object.CurrentCastTime > 0) return;
            if (Player.Object.GetRole() is CombatRole.Healer || (Player.Job is Job.SMN or Job.RDM && cfg.HealerSettings.AutoRezDPSJobs))
            {
                if (!needsHeal)
                {
                    if (cfg.HealerSettings.AutoCleanse && Player.Object.GetRole() is CombatRole.Healer)
                    {
                        CleanseParty();
                        //if (GetPartyMembers().Any((x => HasCleansableDebuff(x))))
                        //    return;
                    }

                    if (cfg.HealerSettings.AutoRez)
                    {
                        RezParty();
                        bool rdmCheck = Player.Job is Job.RDM && ActionReady(RDM.Verraise) && ActionManager.GetAdjustedCastTime(ActionType.Action, RDM.Verraise) > 0;

                        //if (GetPartyMembers().Any(RezQuery) && !rdmCheck)
                        //    return;
                    }
                }
            }


            if (ActionManager.Instance()->AnimationLock > 0) return;

            foreach (var preset in autoActions.Where(x => x.Key.Attributes().AutoAction.IsHeal == canHeal).OrderByDescending(x => x.Key.Attributes().AutoAction.IsAoE))
            {
                var attributes = preset.Key.Attributes();
                var action = attributes.AutoAction;
                if ((action.IsAoE && LockedST) || (!action.IsAoE && LockedAoE)) continue;
                var gameAct = attributes.ReplaceSkill.ActionIDs.First();
                if (ActionManager.Instance()->GetActionStatus(ActionType.Action, gameAct) == 639) continue;
                var sheetAct = Svc.Data.GetExcelSheet<Action>().GetRow(gameAct);

                var outAct = OriginalHook(AutoRotationHelper.InvokeCombo(preset.Key, attributes, ref _));
                if (!CanQueue(outAct)) continue;
                if (action.IsHeal)
                {
                    AutomateHealing(preset.Key, attributes, gameAct);
                    continue;
                }

                if (!action.IsHeal && HasEffect(418)) //Rez Invuln
                    continue;

                if (Player.Object.GetRole() is CombatRole.Tank)
                {
                    AutomateTanking(preset.Key, attributes, gameAct);
                    continue;
                }

                if (!action.IsHeal)
                    AutomateDPS(preset.Key, attributes, gameAct);
            }

        }

        private static void PreEmptiveHot()
        {
            if (PartyInCombat())
                return;

            if (Svc.Targets.FocusTarget is null)
                return;

            if (InDuty() && !Svc.DutyState.IsDutyStarted)
                return;

            ushort regenBuff = Player.Job switch
            {
                Job.AST => AST.Buffs.AspectedBenefic,
                Job.CNJ or Job.WHM => WHM.Buffs.Regen,
                _ => 0
            };

            uint regenSpell = Player.Job switch
            {
                Job.AST => AST.AspectedBenefic,
                Job.CNJ or Job.WHM => WHM.Regen,
                _ => 0
            };

            if (regenSpell != 0 && Svc.Targets.FocusTarget != null && (!MemberHasEffect(regenBuff, Svc.Targets.FocusTarget, true, out var regen) || regen?.RemainingTime <= 5f))
            {
                var query = Svc.Objects.Where(x => !x.IsDead && x.IsHostile() && x.IsTargetable);
                if (!query.Any())
                    return;

                if (query.Min(x => GetTargetDistance(x, Svc.Targets.FocusTarget)) <= 30)
                {
                    var spell = ActionManager.Instance()->GetAdjustedActionId(regenSpell);

                    if (Svc.Targets.FocusTarget.IsDead)
                        return;

                    if (!ActionReady(spell))
                        return;

                    if (ActionManager.CanUseActionOnTarget(spell, Svc.Targets.FocusTarget.Struct()) && !ActionWatching.OutOfRange(spell, Player.Object, Svc.Targets.FocusTarget) && ActionManager.Instance()->GetActionStatus(ActionType.Action, spell) == 0)
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, regenSpell, Svc.Targets.FocusTarget.GameObjectId);
                        return;
                    }
                }
            }
        }

        private static void RezParty()
        {
            uint resSpell = Player.Job switch
            {
                Job.CNJ or Job.WHM => WHM.Raise,
                Job.SCH or Job.SMN => SCH.Resurrection,
                Job.AST => AST.Ascend,
                Job.SGE => SGE.Egeiro,
                Job.RDM => RDM.Verraise,
                _ => throw new NotImplementedException(),
            };

            if (ActionManager.Instance()->QueuedActionId == resSpell)
                ActionManager.Instance()->QueuedActionId = 0;

            if (Player.Object.CurrentMp >= GetResourceCost(resSpell) && ActionReady(resSpell))
            {
                var timeSinceLastRez = TimeSpan.FromMilliseconds(ActionWatching.TimeSinceLastSuccessfulCast(resSpell));
                if ((ActionWatching.TimeSinceLastSuccessfulCast(resSpell) != -1f && timeSinceLastRez.TotalSeconds < 4) || Player.Object.IsCasting())
                    return;

                if (GetPartyMembers().Where(RezQuery).FindFirst(x => x is not null, out var member))
                {
                    if (Player.Job is Job.RDM)
                    {
                        if (ActionReady(All.Swiftcast) && !HasEffect(RDM.Buffs.Dualcast))
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, All.Swiftcast);
                            return;
                        }

                        if (ActionManager.GetAdjustedCastTime(ActionType.Action, resSpell) == 0)
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.BattleChara.GameObjectId);
                        }

                    }
                    else
                    {
                        if (ActionReady(All.Swiftcast))
                        {
                            if (ActionManager.Instance()->GetActionStatus(ActionType.Action, All.Swiftcast) == 0)
                            {
                                ActionManager.Instance()->UseAction(ActionType.Action, All.Swiftcast);
                                return;
                            }
                        }

                        if (!IsMoving() || HasEffect(All.Buffs.Swiftcast))
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.BattleChara.GameObjectId);
                        }
                    }
                }
            }
        }

        private static void CleanseParty()
        {
            if (ActionManager.Instance()->QueuedActionId == All.Esuna)
                ActionManager.Instance()->QueuedActionId = 0;

            if (GetPartyMembers().FindFirst(x => HasCleansableDebuff(x.BattleChara), out var member))
            {
                if (InActionRange(All.Esuna, member.BattleChara) && IsInLineOfSight(member.BattleChara))
                ActionManager.Instance()->UseAction(ActionType.Action, All.Esuna, member.BattleChara.GameObjectId);
            }
        }

        private static void UpdateKardiaTarget()
        {
            if (!LevelChecked(SGE.Kardia)) return;
            if (CombatEngageDuration().TotalSeconds < 3) return;

            foreach (var member in GetPartyMembers().OrderByDescending(x => x.BattleChara.GetRole() is CombatRole.Tank))
            {
                if (cfg.HealerSettings.KardiaTanksOnly && member.BattleChara.GetRole() is not CombatRole.Tank &&
                    FindEffectOnMember(3615, member.BattleChara) is null) continue;

                var enemiesTargeting = Svc.Objects.Where(x => x.IsTargetable && x.IsHostile() && x.TargetObjectId == member.BattleChara.GameObjectId).Count();
                if (enemiesTargeting > 0 && FindEffectOnMember(SGE.Buffs.Kardion, member.BattleChara, true) is null)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, SGE.Kardia, member.BattleChara.GameObjectId);
                    return;
                }
            }

        }

        private unsafe static bool AutomateDPS(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
        {
            var mode = cfg.DPSRotationMode;
            if (attributes.AutoAction.IsAoE)
            {
                return AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
            }
            else
            {
                return AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
            }
        }

        private static bool AutomateTanking(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
        {
            var mode = cfg.DPSRotationMode;
            if (attributes.AutoAction.IsAoE)
            {
                return AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
            }
            else
            {
                return AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
            }
        }

        private static bool AutomateHealing(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
        {
            var mode = cfg.HealerRotationMode;
            if (Player.Object.IsCasting()) return false;
            if (Environment.TickCount64 < LastHealAt + 1200) return false;

            if (attributes.AutoAction.IsAoE)
            {
                var ret = AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
                return ret;
            }
            else
            {
                var ret = AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
                return ret;
            }
        }

        public static class AutoRotationHelper
        {
            public static IGameObject? GetSingleTarget(Enum rotationMode)
            {
                if (rotationMode is DPSRotationMode dpsmode)
                {
                    if (Player.Object.GetRole() is CombatRole.Tank)
                    {
                        IGameObject? target = dpsmode switch
                        {
                            DPSRotationMode.Manual => Svc.Targets.Target,
                            DPSRotationMode.Highest_Max => TankTargeting.GetHighestMaxTarget(),
                            DPSRotationMode.Lowest_Max => TankTargeting.GetLowestMaxTarget(),
                            DPSRotationMode.Highest_Current => TankTargeting.GetHighestCurrentTarget(),
                            DPSRotationMode.Lowest_Current => TankTargeting.GetLowestCurrentTarget(),
                            DPSRotationMode.Tank_Target => Svc.Targets.Target,
                            DPSRotationMode.Nearest => DPSTargeting.GetNearestTarget(),
                            DPSRotationMode.Furthest => DPSTargeting.GetFurthestTarget(),
                            _ => Svc.Targets.Target,
                        };
                        return target;
                    }
                    else
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
                            _ => Svc.Targets.Target,
                        };
                        return target;
                    }
                }
                if (rotationMode is HealerRotationMode healermode)
                {
                    if (Player.Object.GetRole() != CombatRole.Healer) return null;
                    IGameObject? target = healermode switch
                    {
                        HealerRotationMode.Manual => HealerTargeting.ManualTarget(),
                        HealerRotationMode.Highest_Current => HealerTargeting.GetHighestCurrent(),
                        HealerRotationMode.Lowest_Current => HealerTargeting.GetLowestCurrent(),
                        _ => HealerTargeting.ManualTarget(),
                    };
                    return target;
                }

                return null;
            }

            public static bool ExecuteAoE(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
            {
                if (attributes.AutoAction.IsHeal)
                {
                    uint outAct = OriginalHook(InvokeCombo(preset, attributes, ref gameAct, Player.Object));
                    if (ActionManager.Instance()->GetActionStatus(ActionType.Action, outAct) != 0) return false;
                    if (!ActionReady(outAct))
                        return false;

                    if (HealerTargeting.CanAoEHeal(outAct))
                    {
                        var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                        if (IsMoving() && castTime > 0)
                            return false;

                        var ret = ActionManager.Instance()->UseAction(ActionType.Action, Service.IconReplacer.getIconHook.IsEnabled ? gameAct : outAct);

                        if (ret)
                            LastHealAt = Environment.TickCount64 + castTime;

                        return ret;
                    }
                }
                else
                {
                    var target = DPSTargeting.BaseSelection.MaxBy(x => NumberOfEnemiesInRange(OriginalHook(gameAct), x, true));
                    uint outAct = OriginalHook(InvokeCombo(preset, attributes, ref gameAct));
                    if (!CanQueue(outAct)) return false;
                    if (!ActionReady(outAct))
                        return false;

                    var sheet = Svc.Data.GetExcelSheet<Action>().GetRow(outAct);
                    var mustTarget = sheet.CanTargetHostile;
                    var numEnemies = NumberOfEnemiesInRange(gameAct, target, true);
                    if (numEnemies >= cfg.DPSSettings.DPSAoETargets)
                    {
                        bool switched = SwitchOnDChole(attributes, outAct, ref target);
                        var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                        if (IsMoving() && castTime > 0)
                            return false;

                        if (mustTarget || cfg.DPSSettings.AlwaysSelectTarget)
                            Svc.Targets.Target = target;

                        var ret = ActionManager.Instance()->UseAction(ActionType.Action, Service.IconReplacer.getIconHook.IsEnabled ? gameAct : outAct, (mustTarget && target != null) || switched ? target.GameObjectId : Player.Object.GameObjectId);

                        if (outAct is NIN.Ten or NIN.Chi or NIN.Jin or NIN.TenCombo or NIN.ChiCombo or NIN.JinCombo && ret)
                            LockedAoE = true;
                        else
                            LockedAoE = false;

                        return ret;
                    }
                }
                return false;
            }

            public static bool ExecuteST(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
            {
                var target = GetSingleTarget(mode);
                if (target is null)
                    return false;

                var outAct = OriginalHook(InvokeCombo(preset, attributes, ref gameAct, target));
                if (!CanQueue(outAct))
                {
                    return false;
                }

                bool switched = SwitchOnDChole(attributes, outAct, ref target);

                if (target is null)
                    return false;

                var areaTargeted = Svc.Data.GetExcelSheet<Action>().GetRow(outAct).TargetArea;
                var canUseTarget = ActionManager.CanUseActionOnTarget(outAct, target.Struct());
                var canUseSelf = ActionManager.CanUseActionOnTarget(outAct, Player.GameObject);
                var inRange = IsInLineOfSight(target) && InActionRange(outAct, target);

                var canUse = canUseSelf || canUseTarget || areaTargeted;

                if (canUse || cfg.DPSSettings.AlwaysSelectTarget)
                    Svc.Targets.Target = target;

                var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                if (IsMoving() && castTime > 0)
                    return false;

                if (canUse && (inRange || areaTargeted))
                {
                    var ret = ActionManager.Instance()->UseAction(ActionType.Action, Service.IconReplacer.getIconHook.IsEnabled ? gameAct : outAct, canUseTarget ? target.GameObjectId : Player.Object.GameObjectId);
                    if (mode is HealerRotationMode && ret)
                        LastHealAt = Environment.TickCount64 + castTime;

                    if (outAct is NIN.Ten or NIN.Chi or NIN.Jin or NIN.TenCombo or NIN.ChiCombo or NIN.JinCombo && ret)
                        LockedST = true;
                    else
                        LockedST = false;

                    return ret;
                }

                return false;
            }

            private static bool SwitchOnDChole(Presets.PresetAttributes attributes, uint outAct, ref IGameObject? newtarget)
            {
                if (outAct is SGE.Druochole && !attributes.AutoAction.IsHeal)
                {
                    if (GetPartyMembers().Where(x => !x.BattleChara.IsDead && x.BattleChara.IsTargetable && IsInLineOfSight(x.BattleChara) && GetTargetDistance(x.BattleChara) < 30).OrderBy(x => GetTargetHPPercent(x.BattleChara)).Select(x => x.BattleChara).TryGetFirst(out newtarget))
                        return true;
                }

                return false;
            }

            public static uint InvokeCombo(CustomComboPreset preset, Presets.PresetAttributes attributes, ref uint originalAct, IGameObject? optionalTarget = null)
            {
                var outAct = attributes.ReplaceSkill.ActionIDs.FirstOrDefault();
                foreach (var actToCheck in attributes.ReplaceSkill.ActionIDs)
                {
                    var customCombo = Service.IconReplacer.CustomCombos.FirstOrDefault(x => x.Preset == preset);
                    if (customCombo != null)
                    {
                        if (customCombo.TryInvoke(actToCheck, out var changedAct, optionalTarget))
                        {
                            originalAct = actToCheck;
                            outAct = changedAct;
                            break;
                        }
                    }
                }
                
                return outAct;
            }
        }

        public class DPSTargeting
        {
            private static bool Query(IGameObject x) => x is IBattleChara chara && chara.IsHostile() && IsInRange(chara, cfg.DPSSettings.MaxDistance) && !chara.IsDead && chara.IsTargetable && IsInLineOfSight(chara) && !TargetIsInvincible(chara) && !Service.Configuration.IgnoredNPCs.Any(x => x.Key == chara.DataId) && 
                ((cfg.DPSSettings.OnlyAttackInCombat && chara.Struct()->InCombat) || !cfg.DPSSettings.OnlyAttackInCombat);
            public static IEnumerable<IGameObject> BaseSelection => Svc.Objects.Any(x => Query(x) && IsPriority(x)) ?
                                                                    Svc.Objects.Where(x => Query(x) && IsPriority(x)) :
                                                                    Svc.Objects.Where(x => Query(x));

            private static bool IsPriority(IGameObject x)
            {
                if (x is IBattleChara chara)
                {
                    bool isFate = cfg.DPSSettings.FATEPriority && x.Struct()->FateId != 0 && InFATE();
                    bool isQuest = cfg.DPSSettings.QuestPriority && IsQuestMob(x);

                    return isFate || isQuest;
                }
                return false;
            }

            public static bool IsCombatPriority(IGameObject x)
            {
                if (x is IBattleChara chara)
                {
                    if (!cfg.DPSSettings.PreferNonCombat) return true;
                    bool inCombat = cfg.DPSSettings.PreferNonCombat && !chara.Struct()->InCombat;
                    return inCombat;
                }
                return false;
            }

            public static IGameObject? GetTankTarget()
            {
                var tank = GetPartyMembers().Where(x => x.BattleChara.GetRole() == CombatRole.Tank || FindEffectOnMember(3615, x.BattleChara) is not null).FirstOrDefault();
                if (tank == null)
                    return null;

                return tank.BattleChara.TargetObject;
            }

            public static IGameObject? GetNearestTarget()
            {
                return BaseSelection.OrderByDescending(x => IsCombatPriority(x)).ThenBy(x => GetTargetDistance(x)).FirstOrDefault();
            }

            public static IGameObject? GetFurthestTarget()
            {
                return BaseSelection.OrderByDescending(x => IsCombatPriority(x)).ThenByDescending(x => GetTargetDistance(x)).FirstOrDefault();
            }

            public static IGameObject? GetLowestCurrentTarget()
            {
                return BaseSelection.OrderByDescending(x => IsCombatPriority(x)).ThenBy(x => (x as IBattleChara).CurrentHp).FirstOrDefault();
            }

            public static IGameObject? GetHighestCurrentTarget()
            {
                return BaseSelection.OrderByDescending(x => IsCombatPriority(x)).ThenByDescending(x => (x as IBattleChara).CurrentHp).FirstOrDefault();
            }

            public static IGameObject? GetLowestMaxTarget()
            {

                return BaseSelection.OrderByDescending(x => IsCombatPriority(x)).ThenBy(x => (x as IBattleChara).MaxHp).ThenBy(x => GetTargetHPPercent(x)).ThenBy(x => GetTargetDistance(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestMaxTarget()
            {
                return BaseSelection.OrderByDescending(x => IsCombatPriority(x)).ThenByDescending(x => (x as IBattleChara).MaxHp).ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
            }
        }

        public static class HealerTargeting
        {
            internal static IGameObject? ManualTarget()
            {
                if (Svc.Targets.Target == null) return null;
                var t = Svc.Targets.Target;
                bool goodToHeal = GetTargetHPPercent(t) <= (TargetHasRegen(t) ? cfg.HealerSettings.SingleTargetRegenHPP : cfg.HealerSettings.SingleTargetHPP);
                if (goodToHeal && !t.IsHostile())
                {
                    return t;
                }
                return null;
            }
            internal static IGameObject? GetHighestCurrent()
            {
                if (GetPartyMembers().Count == 0) return Player.Object;
                var target = GetPartyMembers()
                    .Where(x => IsInLineOfSight(x.BattleChara) && GetTargetDistance(x.BattleChara) <= 30 && !x.BattleChara.IsDead && x.BattleChara.IsTargetable && GetTargetHPPercent(x.BattleChara) <= (TargetHasRegen(x.BattleChara) ? cfg.HealerSettings.SingleTargetRegenHPP : cfg.HealerSettings.SingleTargetHPP))
                    .OrderByDescending(x => GetTargetHPPercent(x.BattleChara)).FirstOrDefault();
                return target?.BattleChara;
            }

            internal static IGameObject? GetLowestCurrent()
            {
                if (GetPartyMembers().Count == 0) return Player.Object;
                var target = GetPartyMembers()
                    .Where(x => IsInLineOfSight(x.BattleChara) && GetTargetDistance(x.BattleChara) <= 30 && !x.BattleChara.IsDead && x.BattleChara.IsTargetable && ((float)x.CurrentHP / x.BattleChara.MaxHp * 100) <= (TargetHasRegen(x.BattleChara) ? cfg.HealerSettings.SingleTargetRegenHPP : cfg.HealerSettings.SingleTargetHPP))
                    .OrderBy(x => GetTargetHPPercent(x.BattleChara)).FirstOrDefault();
                return target?.BattleChara;
            }

            internal static bool CanAoEHeal(uint outAct = 0)
            {
                var members = GetPartyMembers().Where(x => !x.BattleChara.IsDead && x.BattleChara.IsTargetable && (outAct == 0 ? GetTargetDistance(x.BattleChara) <= 15 : InActionRange(outAct, x.BattleChara)) && ((float)x.CurrentHP / x.BattleChara.MaxHp * 100) <= cfg.HealerSettings.AoETargetHPP);
                if (members.Count() < cfg.HealerSettings.AoEHealTargetCount)
                    return false;

                return true;
            }

            private static bool TargetHasRegen(IGameObject target)
            {
                ushort regenBuff = JobID switch
                {
                    AST.JobID => AST.Buffs.AspectedBenefic,
                    WHM.JobID => WHM.Buffs.Regen,
                    _ => 0
                };

                return FindEffectOnMember(regenBuff, target) != null;
            }
        }

        public static class TankTargeting
        {
            public static IGameObject? GetLowestCurrentTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                    .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenBy(x => (x as IBattleChara).CurrentHp)
                    .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestCurrentTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                    .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenByDescending(x => (x as IBattleChara).CurrentHp)
                    .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetLowestMaxTarget()
            {
                var t = DPSTargeting.BaseSelection
                    .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                    .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenBy(x => (x as IBattleChara).MaxHp)
                    .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();

                return t;
            }

            public static IGameObject? GetHighestMaxTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                    .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenByDescending(x => (x as IBattleChara).MaxHp)
                    .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
            }
        }
    }
}
