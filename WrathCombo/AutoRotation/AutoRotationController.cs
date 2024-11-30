using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System;
using System.Linq;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using WrathCombo.Extensions;
using Action = Lumina.Excel.Sheets.Action;

namespace WrathCombo.AutoRotation
{
    internal unsafe static class AutoRotationController
    {
        static long LastHealAt = 0;
        static long LastRezAt = 0;

        static bool LockedST = false;
        static bool LockedAoE = false;

        static Func<IBattleChara, bool> RezQuery => x => x.IsDead && CustomComboFunctions.FindEffectOnMember(2648, x) == null && CustomComboFunctions.FindEffectOnMember(148, x) == null && x.IsTargetable() && CustomComboFunctions.TimeSpentDead(x.GameObjectId).TotalSeconds > 2;

        internal static void Run()
        {
            var cfg = Service.Configuration.RotationConfig;

            if (!cfg.Enabled || !Player.Available || Svc.Condition[ConditionFlag.Mounted])
                return;

            if (Player.Object.CurrentCastTime > 0) return;

            if (!EzThrottler.Throttle("AutoRotController", 50))
                return;

            if (cfg.HealerSettings.PreEmptiveHoT && Player.Job is Job.CNJ or Job.WHM or Job.AST)
                PreEmptiveHot();

            bool combatBypass = (cfg.BypassQuest && DPSTargeting.BaseSelection.Any(x => CustomComboFunctions.IsQuestMob(x))) || (cfg.BypassFATE && CustomComboFunctions.InFATE());

            if (cfg.InCombatOnly && (!CustomComboFunctions.GetPartyMembers().Any(x => x.Struct()->InCombat) || CustomComboFunctions.PartyEngageDuration().TotalSeconds < cfg.CombatDelay) && !combatBypass)
                return;

            if (Player.Job is Job.SGE && cfg.HealerSettings.ManageKardia)
                UpdateKardiaTarget();

            var healTarget = Player.Object.GetRole() is CombatRole.Healer ? AutoRotationHelper.GetSingleTarget(cfg.HealerRotationMode) : null;
            var aoeheal = Player.Object.GetRole() is CombatRole.Healer && HealerTargeting.CanAoEHeal();

            if (Player.Object.GetRole() is CombatRole.Healer || (Player.Job is Job.SMN or Job.RDM && cfg.HealerSettings.AutoRezDPSJobs))
            {
                bool needsHeal = (healTarget != null || aoeheal) && Player.Object.GetRole() is CombatRole.Healer;

                if (!needsHeal)
                {
                    if (cfg.HealerSettings.AutoCleanse && Player.Object.GetRole() is CombatRole.Healer)
                    {
                        CleanseParty();
                        if (CustomComboFunctions.GetPartyMembers().Any((x => CustomComboFunctions.HasCleansableDebuff(x))))
                            return;
                    }

                    if (cfg.HealerSettings.AutoRez)
                    {
                        RezParty();
                        bool rdmCheck = Player.Job is Job.RDM && CustomComboFunctions.ActionReady(RDM.Verraise) && ActionManager.GetAdjustedCastTime(ActionType.Action, RDM.Verraise) > 0;

                        if (CustomComboFunctions.GetPartyMembers().Any(RezQuery) && !rdmCheck)
                            return;
                    }
                }
            }

            foreach (var preset in Service.Configuration.AutoActions.OrderByDescending(x => Presets.Attributes[x.Key].AutoAction.IsHeal)
                                                                    .ThenByDescending(x => Presets.Attributes[x.Key].AutoAction.IsAoE))
            {
                if (!CustomComboFunctions.IsEnabled(preset.Key) || !preset.Value) continue;

                var attributes = Presets.Attributes[preset.Key];
                var action = attributes.AutoAction;
                if ((action.IsAoE && LockedST) || (!action.IsAoE && LockedAoE)) continue;
                var gameAct = attributes.ReplaceSkill.ActionIDs.First();
                var sheetAct = Svc.Data.GetExcelSheet<Action>().GetRow(gameAct);
                var classToJob = CustomComboFunctions.JobIDs.ClassToJob((byte)Player.Job);
                if ((byte)Player.Job != attributes.CustomComboInfo.JobID && classToJob != attributes.CustomComboInfo.JobID)
                    continue;

                var outAct = AutoRotationHelper.InvokeCombo(preset.Key, attributes);
                if (!CustomComboFunctions.ActionReady(gameAct))
                    continue;

                if (action.IsHeal)
                {
                    if (!AutomateHealing(preset.Key, attributes, gameAct) && Svc.Targets.Target != null && !Svc.Targets.Target.IsHostile() && Environment.TickCount64 > LastHealAt + 1000)
                        Svc.Targets.Target = null;

                    if ((healTarget != null && !action.IsAoE) || (aoeheal && action.IsAoE))
                        return;
                    else
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
                _ => 0
            };

            uint regenSpell = Player.Job switch
            {
                Job.AST => AST.AspectedBenefic,
                Job.CNJ or Job.WHM => WHM.Regen,
                _ => 0
            };

            if (regenSpell != 0 && Svc.Targets.FocusTarget != null && (!CustomComboFunctions.MemberHasEffect(regenBuff, Svc.Targets.FocusTarget, true, out var regen) || regen?.RemainingTime <= 5f))
            {
                var query = Svc.Objects.Where(x => !x.IsDead && x.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc).Cast<IBattleNpc>().Where(x => x.BattleNpcKind == Dalamud.Game.ClientState.Objects.Enums.BattleNpcSubKind.Enemy && x.IsTargetable());
                if (!query.Any())
                    return;

                if (query.Min(x => CustomComboFunctions.GetTargetDistance(x, Svc.Targets.FocusTarget)) <= 30)
                {
                    var spell = ActionManager.Instance()->GetAdjustedActionId(regenSpell);

                    if (Svc.Targets.FocusTarget.IsDead)
                        return;

                    if (!CustomComboFunctions.ActionReady(spell))
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

            if (Player.Object.CurrentMp >= CustomComboFunctions.GetResourceCost(resSpell) && CustomComboFunctions.ActionReady(resSpell) && ActionManager.Instance()->GetActionStatus(ActionType.Action, resSpell) == 0)
            {
                var timeSinceLastRez = TimeSpan.FromMilliseconds(ActionWatching.TimeSinceLastSuccessfulCast(resSpell));
                if ((ActionWatching.TimeSinceLastSuccessfulCast(resSpell) != -1f && timeSinceLastRez.TotalSeconds < 4) || Player.Object.IsCasting())
                    return;

                if (CustomComboFunctions.GetPartyMembers().Where(RezQuery).FindFirst(x => x is not null, out var member))
                {
                    if (Player.Job is Job.RDM)
                    {
                        if (CustomComboFunctions.ActionReady(All.Swiftcast) && !CustomComboFunctions.HasEffect(RDM.Buffs.Dualcast))
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, All.Swiftcast);
                            return;
                        }

                        if (ActionManager.GetAdjustedCastTime(ActionType.Action, resSpell) == 0)
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.GameObjectId);
                        }

                    }
                    else
                    {
                        if (CustomComboFunctions.ActionReady(All.Swiftcast))
                        {
                            if (ActionManager.Instance()->GetActionStatus(ActionType.Action, All.Swiftcast) == 0)
                            {
                                ActionManager.Instance()->UseAction(ActionType.Action, All.Swiftcast);
                                return;
                            }
                        }

                        if (!CustomComboFunctions.IsMoving || CustomComboFunctions.HasEffect(All.Buffs.Swiftcast))
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.GameObjectId);
                        }
                    }
                }
            }
        }

        private static void CleanseParty()
        {
            if (ActionManager.Instance()->QueuedActionId == All.Esuna)
                ActionManager.Instance()->QueuedActionId = 0;

            if (CustomComboFunctions.GetPartyMembers().FindFirst(x => CustomComboFunctions.HasCleansableDebuff(x), out var member) && !CustomComboFunctions.IsMoving)
                ActionManager.Instance()->UseAction(ActionType.Action, All.Esuna, member.GameObjectId);
        }

        private static void UpdateKardiaTarget()
        {
            if (!CustomComboFunctions.LevelChecked(SGE.Kardia)) return;
            if (CustomComboFunctions.CombatEngageDuration().TotalSeconds < 3) return;

            foreach (var member in CustomComboFunctions.GetPartyMembers().OrderByDescending(x => x.GetRole() is CombatRole.Tank))
            {
                if (Service.Configuration.RotationConfig.HealerSettings.KardiaTanksOnly && member.GetRole() is not CombatRole.Tank &&
                    CustomComboFunctions.FindEffectOnMember(3615, member) is null) continue;

                var enemiesTargeting = Svc.Objects.Where(x => x.IsTargetable && x.IsHostile() && x.TargetObjectId == member.GameObjectId).Count();
                if (enemiesTargeting > 0 && CustomComboFunctions.FindEffectOnMember(SGE.Buffs.Kardion, member, true) is null)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, SGE.Kardia, member.GameObjectId);
                    return;
                }
            }

        }

        private unsafe static bool AutomateDPS(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
        {
            var mode = Service.Configuration.RotationConfig.DPSRotationMode;
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
            var mode = Service.Configuration.RotationConfig.DPSRotationMode;
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
            var mode = Service.Configuration.RotationConfig.HealerRotationMode;
            if (Player.Object.IsCasting()) return false;

            if (attributes.AutoAction.IsAoE)
            {
                if (Environment.TickCount64 < LastHealAt + 1500) return false;
                var ret = AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
                return ret;
            }
            else
            {
                if (Environment.TickCount64 < LastHealAt + 1500) return false;
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
                    uint outAct = CustomComboFunctions.OriginalHook(InvokeCombo(preset, attributes, Player.Object));
                    if (ActionManager.Instance()->GetActionStatus(ActionType.Action, outAct) != 0) return false;
                    if (!CustomComboFunctions.ActionReady(outAct))
                        return false;

                    if (HealerTargeting.CanAoEHeal(outAct))
                    {
                        var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                        if (CustomComboFunctions.IsMoving && castTime > 0)
                            return false;

                        var ret = ActionManager.Instance()->UseAction(ActionType.Action, outAct);

                        if (ret)
                            LastHealAt = Environment.TickCount64 + castTime;

                        if (outAct is NIN.Ten or NIN.Chi or NIN.Jin or NIN.TenCombo or NIN.ChiCombo or NIN.JinCombo && ret)
                            LockedAoE = true;
                        else
                            LockedAoE = false;

                        return ret;
                    }
                }
                else
                {
                    uint outAct = CustomComboFunctions.OriginalHook(InvokeCombo(preset, attributes, Player.Object));
                    if (ActionManager.Instance()->GetActionStatus(ActionType.Action, outAct) != 0) return false;
                    if (!CustomComboFunctions.ActionReady(outAct))
                        return false;

                    var target = GetSingleTarget(mode);
                    var sheet = Svc.Data.GetExcelSheet<Action>().GetRow(outAct);
                    var mustTarget = sheet.CanTargetHostile;
                    var numEnemies = CustomComboFunctions.NumberOfEnemiesInRange(gameAct, target);
                    if (numEnemies >= Service.Configuration.RotationConfig.DPSSettings.DPSAoETargets)
                    {
                        bool switched = SwitchOnDChole(attributes, outAct, ref target);
                        var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                        if (CustomComboFunctions.IsMoving && castTime > 0)
                            return false;

                        if (mustTarget)
                            Svc.Targets.Target = target;

                        return ActionManager.Instance()->UseAction(ActionType.Action, outAct, (mustTarget && target != null) || switched ? target.GameObjectId : Player.Object.GameObjectId);
                    }
                }
                return false;
            }

            public static bool ExecuteST(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
            {
                var target = GetSingleTarget(mode);
                if (target is null)
                    return false;

                var outAct = CustomComboFunctions.OriginalHook(InvokeCombo(preset, attributes, target));
                if (ActionManager.Instance()->GetActionStatus(ActionType.Action, outAct) != 0) return false;
                var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                if (CustomComboFunctions.IsMoving && castTime > 0)
                    return false;

                bool switched = SwitchOnDChole(attributes, outAct, ref target);

                if (target is null)
                    return false;

                var areaTargeted = Svc.Data.GetExcelSheet<Action>().GetRow(outAct).TargetArea;
                var canUseTarget = ActionManager.CanUseActionOnTarget(outAct, target.Struct());
                var canUseSelf = ActionManager.CanUseActionOnTarget(outAct, Player.GameObject);
                var inRange = CustomComboFunctions.IsInLineOfSight(target) && CustomComboFunctions.InActionRange(outAct, target);

                var canUse = canUseSelf || canUseTarget || areaTargeted;
                if (canUse && (inRange || areaTargeted))
                {
                    Svc.Targets.Target = target;

                    var ret = ActionManager.Instance()->UseAction(ActionType.Action, outAct, canUseTarget ? target.GameObjectId : Player.Object.GameObjectId);
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

            private static bool SwitchOnDChole(Presets.PresetAttributes attributes, uint outAct, ref IGameObject newtarget)
            {
                if (outAct is SGE.Druochole && !attributes.AutoAction.IsHeal)
                {
                    if (CustomComboFunctions.GetPartyMembers().Where(x => !x.IsDead && x.IsTargetable() && CustomComboFunctions.IsInLineOfSight(x) && CustomComboFunctions.GetTargetDistance(x) < 30).OrderBy(x => CustomComboFunctions.GetTargetHPPercent(x)).TryGetFirst(out newtarget))
                        return true;
                }

                return false;
            }

            public static uint InvokeCombo(CustomComboPreset preset, Presets.PresetAttributes attributes, IGameObject? optionalTarget = null)
            {
                var outAct = attributes.ReplaceSkill.ActionIDs.FirstOrDefault();
                foreach (var actToCheck in attributes.ReplaceSkill.ActionIDs)
                {
                    var customCombo = Service.IconReplacer.CustomCombos.FirstOrDefault(x => x.Preset == preset);
                    if (customCombo != null)
                    {
                        if (customCombo.TryInvoke(actToCheck, (byte)Player.Level, ActionManager.Instance()->Combo.Action, ActionManager.Instance()->Combo.Timer, out var changedAct, optionalTarget))
                        {
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
            public static System.Collections.Generic.IEnumerable<IGameObject> BaseSelection =>  Svc.Objects.Any(x => x is IBattleChara chara && chara.IsHostile() && CustomComboFunctions.IsInRange(chara) && !chara.IsDead && chara.IsTargetable && CustomComboFunctions.IsInLineOfSight(chara) && IsPriority(chara)) ? 
                                                                                                Svc.Objects.Where(x => x is IBattleChara chara && chara.IsHostile() && CustomComboFunctions.IsInRange(chara) && !chara.IsDead && chara.IsTargetable && CustomComboFunctions.IsInLineOfSight(chara) && IsPriority(chara)) :
                                                                                                Svc.Objects.Where(x => x is IBattleChara chara && chara.IsHostile() && CustomComboFunctions.IsInRange(chara) && !chara.IsDead && chara.IsTargetable && CustomComboFunctions.IsInLineOfSight(chara));

            private static bool IsPriority(IGameObject x)
            {
                bool isFate = Service.Configuration.RotationConfig.DPSSettings.FATEPriority && x.Struct()->FateId != 0 && CustomComboFunctions.InFATE();
                bool isQuest = Service.Configuration.RotationConfig.DPSSettings.QuestPriority && CustomComboFunctions.IsQuestMob(x);
                if (Player.Object.GetRole() is CombatRole.Tank && x.TargetObjectId != Player.Object.GameObjectId)
                    return true;

                return isFate || isQuest;
            }

            public static IGameObject? GetTankTarget()
            {
                var tank = CustomComboFunctions.GetPartyMembers().Where(x => x.GetRole() == CombatRole.Tank).FirstOrDefault();
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

                return BaseSelection.OrderBy(x => (x as IBattleChara).MaxHp).ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).ThenBy(x => CustomComboFunctions.GetTargetDistance(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestMaxTarget()
            {
                return BaseSelection.OrderByDescending(x => (x as IBattleChara).MaxHp).ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }
        }

        public static class HealerTargeting
        {
            internal static IGameObject? ManualTarget()
            {
                if (Svc.Targets.Target == null) return null;
                var t = Svc.Targets.Target;
                bool goodToHeal = CustomComboFunctions.GetTargetHPPercent(t) <= (TargetHasRegen(t) ? Service.Configuration.RotationConfig.HealerSettings.SingleTargetRegenHPP : Service.Configuration.RotationConfig.HealerSettings.SingleTargetHPP);
                if (goodToHeal)
                {
                    return t;
                }
                return null;
            }
            internal static IGameObject? GetHighestCurrent()
            {
                if (CustomComboFunctions.GetPartyMembers().Count == 0) return Player.Object;
                var target = CustomComboFunctions.GetPartyMembers()
                    .Where(x => !x.IsDead && x.IsTargetable && CustomComboFunctions.GetTargetHPPercent(x) <= (TargetHasRegen(x) ? Service.Configuration.RotationConfig.HealerSettings.SingleTargetRegenHPP : Service.Configuration.RotationConfig.HealerSettings.SingleTargetHPP))
                    .OrderByDescending(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
                return target;
            }

            internal static IGameObject? GetLowestCurrent()
            {
                if (CustomComboFunctions.GetPartyMembers().Count == 0) return Player.Object;
                var target = CustomComboFunctions.GetPartyMembers()
                    .Where(x => !x.IsDead && x.IsTargetable && CustomComboFunctions.GetTargetHPPercent(x) <= (TargetHasRegen(x) ? Service.Configuration.RotationConfig.HealerSettings.SingleTargetRegenHPP : Service.Configuration.RotationConfig.HealerSettings.SingleTargetHPP))
                    .OrderBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
                return target;
            }

            internal static bool CanAoEHeal(uint outAct = 0)
            {
                var members = CustomComboFunctions.GetPartyMembers().Where(x => (outAct == 0 ? CustomComboFunctions.GetTargetDistance(x) <= 15 : CustomComboFunctions.InActionRange(outAct, x)) && CustomComboFunctions.GetTargetHPPercent(x) <= Service.Configuration.RotationConfig.HealerSettings.AoETargetHPP);
                if (members.Count() < Service.Configuration.RotationConfig.HealerSettings.AoEHealTargetCount)
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

                return CustomComboFunctions.FindEffectOnMember(regenBuff, target) != null;
            }
        }

        public static class TankTargeting
        {
            public static IGameObject? GetLowestCurrentTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenBy(x => (x as IBattleChara).CurrentHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestCurrentTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenByDescending(x => (x as IBattleChara).CurrentHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetLowestMaxTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .OrderBy(x => (x as IBattleChara).MaxHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestMaxTarget()
            {
                return DPSTargeting.BaseSelection
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenByDescending(x => (x as IBattleChara).MaxHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }
        }
    }
}
