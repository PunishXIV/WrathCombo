using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using XIVSlothCombo.Combos;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Services;
using XIVSlothCombo.Window.Functions;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVSlothCombo.AutoRotation
{
    internal unsafe static class AutoRotationController
    {
        static long LastHealAt = 0;
        internal static void Run()
        {
            if (!Service.Configuration.RotationConfig.Enabled || !Player.Available || Svc.Condition[ConditionFlag.Mounted] || (Service.Configuration.RotationConfig.InCombatOnly && !CustomComboFunctions.InCombat()))
                return;

            if (!EzThrottler.Throttle("AutoRotController", 150))
                return;

            foreach (var preset in Service.Configuration.AutoActions.OrderByDescending(x => Presets.Attributes[x.Key].AutoAction.IsHeal)
                                                                    .ThenByDescending(x => Presets.Attributes[x.Key].AutoAction.IsAoE))
            {
                if (!CustomComboFunctions.IsEnabled(preset.Key) || !preset.Value) continue;

                var attributes = Presets.Attributes[preset.Key];
                var action = attributes.AutoAction;
                var gameAct = attributes.ReplaceSkill.ActionIDs.First();
                var sheetAct = Svc.Data.GetExcelSheet<Action>().GetRow(gameAct);
                var classToJob = CustomComboFunctions.JobIDs.ClassToJob((byte)Player.Job);
                if ((byte)Player.Job != attributes.CustomComboInfo.JobID && classToJob != attributes.CustomComboInfo.JobID)
                    continue;

                var outAct = AutoRotationHelper.InvokeCombo(preset.Key, attributes);
                var healTarget = AutoRotationHelper.GetSingleTarget(Service.Configuration.RotationConfig.HealerRotationMode);
                var aoeheal = HealerTargeting.CanAoEHeal(outAct);

                if (action.IsHeal)
                {
                    if (!AutomateHealing(preset.Key, attributes, gameAct) && Svc.Targets.Target != null && !Svc.Targets.Target.IsHostile())
                        Svc.Targets.Target = null;

                    continue;
                }

                if (Player.Object.GetRole() is CombatRole.Tank)
                {
                    AutomateTanking(preset.Key, attributes, gameAct);
                    continue;
                }

                if (healTarget == null && !aoeheal)
                AutomateDPS(preset.Key, attributes, gameAct);
            }


        }

        private static bool AutomateDPS(CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
        {
            if (Svc.Targets.Target != null && !Svc.Targets.Target.IsHostile()) return false;

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
                var ret = AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
                if (ret)
                    LastHealAt = Environment.TickCount64;

                return ret;
            }
            else
            {
                var ret = AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
                if (ret)
                    LastHealAt = Environment.TickCount64;

                return ret;
            }
        }

        public static class AutoRotationHelper
        {
            public static IGameObject? GetSingleTarget(System.Enum rotationMode)
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
                    };

                    return target;
                }

                return null;
            }

            public static bool ExecuteAoE(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
            {
                if (attributes.AutoAction.IsHeal)
                {
                    uint outAct = InvokeCombo(preset, attributes, Player.Object);
                    if (!CustomComboFunctions.ActionReady(outAct))
                        return false;

                    if (HealerTargeting.CanAoEHeal(outAct))
                    {
                        var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                        if (CustomComboFunctions.IsMoving && castTime > 0)
                            return false;

                        return ActionManager.Instance()->UseAction(ActionType.Action, outAct);
                    }
                }
                else
                {
                    uint outAct = InvokeCombo(preset, attributes, Player.Object);
                    if (!CustomComboFunctions.ActionReady(outAct))
                        return false;

                    var target = GetSingleTarget(mode);
                    var sheet = Svc.Data.GetExcelSheet<Action>().GetRow(outAct);
                    var mustTarget = sheet.CanTargetHostile;
                    var numEnemies = CustomComboFunctions.NumberOfEnemiesInRange(outAct, target);
                    if (numEnemies >= Service.Configuration.RotationConfig.DPSSettings.DPSAoETargets ||
                        (sheet.EffectRange == 0 && sheet.CanTargetSelf && !mustTarget))
                    {
                        var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                        if (CustomComboFunctions.IsMoving && castTime > 0)
                            return false;

                        if (mustTarget)
                            Svc.Targets.Target = target;

                        return ActionManager.Instance()->UseAction(ActionType.Action, outAct, mustTarget && target != null ? target.GameObjectId : Player.Object.GameObjectId);
                    }
                }
                return false;
            }

            public static bool ExecuteST(Enum mode, CustomComboPreset preset, Presets.PresetAttributes attributes, uint gameAct)
            {
                var target = GetSingleTarget(mode);
                if (target is null)
                    return false;

                var outAct = InvokeCombo(preset, attributes, target);

                var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                if (CustomComboFunctions.IsMoving && castTime > 0)
                    return false;

                var areaTargeted = Svc.Data.GetExcelSheet<Action>().GetRow(outAct).TargetArea;
                var inRange = ActionManager.GetActionInRangeOrLoS(outAct, Player.GameObject, target.Struct()) != 562;
                var canUseTarget = ActionManager.CanUseActionOnTarget(outAct, target.Struct());
                var canUseSelf = ActionManager.CanUseActionOnTarget(outAct, Player.GameObject);

                var canUse = canUseSelf || canUseTarget || areaTargeted;
                if (canUse && inRange)
                {
                    if (target.IsHostile())
                        Svc.Targets.Target = target;

                    return ActionManager.Instance()->UseAction(ActionType.Action, outAct, canUseTarget ? target.GameObjectId : Player.Object.GameObjectId);
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
            public static System.Collections.Generic.IEnumerable<IGameObject> BaseSelection => Svc.Objects.Where(x => x is IBattleChara chara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead && x.IsTargetable).OrderByDescending(x => IsPriority(x));

            private static bool IsPriority(IGameObject x)
            {
                bool isFate = Service.Configuration.RotationConfig.DPSSettings.FATEPriority && x.Struct()->FateId != 0;
                var namePlateIcon = x.Struct()->NamePlateIconId;
                bool isQuest = Service.Configuration.RotationConfig.DPSSettings.QuestPriority && namePlateIcon is 71204 or 71144 or 71224 or 71344;
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

            internal static bool CanAoEHeal(uint outAct)
            {
                var members = CustomComboFunctions.GetPartyMembers().Where(x => CustomComboFunctions.InActionRange(outAct, x) && CustomComboFunctions.GetTargetHPPercent(x) <= Service.Configuration.RotationConfig.HealerSettings.AoETargetHPP);
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

                return CustomComboFunctions.FindEffect(regenBuff) != null;
            }
        }

        public static class TankTargeting
        {
            public static IGameObject? GetLowestCurrentTarget()
            {
                return Svc.Objects.Where(x => x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead && x.IsTargetable)
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenBy(x => (x as IBattleChara).CurrentHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestCurrentTarget()
            {
                return Svc.Objects.Where(x => x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead && x.IsTargetable)
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenByDescending(x => (x as IBattleChara).CurrentHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetLowestMaxTarget()
            {
                return Svc.Objects.Where(x => x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead && x.IsTargetable)
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .OrderBy(x => (x as IBattleChara).MaxHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }

            public static IGameObject? GetHighestMaxTarget()
            {
                return Svc.Objects.Where(x => x is IBattleChara && x.IsHostile() && CustomComboFunctions.IsInRange(x) && !x.IsDead && x.IsTargetable)
                    .OrderByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                    .ThenByDescending(x => (x as IBattleChara).MaxHp)
                    .ThenBy(x => CustomComboFunctions.GetTargetHPPercent(x)).FirstOrDefault();
            }
        }
    }
}
