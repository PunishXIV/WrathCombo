#region

using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class MCH
{
    // MCH Gauge & Extensions
    internal static MCHOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked) return Opener1;

        return WrathOpener.Dummy;
    }


    internal static MCHGauge Gauge = GetJobGauge<MCHGauge>();

    internal static bool reassembledExcavatorST =>
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[0] &&
         (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[0] &&
         !HasEffect(Buffs.Reassembled)) ||
        (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
        !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

    internal static bool reassembledChainsawST =>
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[1] &&
         (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[1] &&
         !HasEffect(Buffs.Reassembled)) ||
        (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
        !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

    internal static bool reassembledAnchorST =>
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[2] &&
         (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[2] &&
         !HasEffect(Buffs.Reassembled)) ||
        (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
        !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

    internal static bool reassembledDrillST =>
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[3] &&
         (HasEffect(Buffs.Reassembled) || !HasEffect(Buffs.Reassembled))) ||
        (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && !Config.MCH_ST_Reassembled[3] &&
         !HasEffect(Buffs.Reassembled)) ||
        (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_ST_ReassemblePool) ||
        !IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble);

    internal static float GCD => GetCooldown(OriginalHook(SplitShot)).CooldownTotal;

    internal static float heatblastRC => GetCooldown(Heatblast).CooldownTotal;

    internal static bool drillCD =>
        !LevelChecked(Drill) || (!TraitLevelChecked(Traits.EnhancedMultiWeapon) &&
                                 GetCooldownRemainingTime(Drill) > heatblastRC * 6) ||
        (TraitLevelChecked(Traits.EnhancedMultiWeapon) &&
         GetRemainingCharges(Drill) < GetMaxCharges(Drill) &&
         GetCooldownRemainingTime(Drill) > heatblastRC * 6);

    internal static bool anchorCD =>
        !LevelChecked(AirAnchor) ||
        (LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) > heatblastRC * 6);

    internal static bool sawCD =>
        !LevelChecked(Chainsaw) ||
        (LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) > heatblastRC * 6);

    internal static bool interruptReady =>
        ActionReady(All.HeadGraze) && CanInterruptEnemy() &&
        CanDelayedWeave();

    internal static bool battery => Gauge.Battery >= 100;

    internal static bool HasNotWeaved =>
        ActionWatching.GetAttackType(ActionWatching.LastAction) !=
        ActionWatching.ActionAttackType.Ability;

    public static int BSUsed => ActionWatching.CombatActions.Count(x => x == BarrelStabilizer);

    internal class MCHOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;
        public override List<uint> OpenerActions { get; set; } = new()
        {
            Reassemble,
            AirAnchor,
            CheckMate,
            DoubleCheck,
            Drill,
            BarrelStabilizer,
            Chainsaw,
            Excavator,
            AutomatonQueen,
            Reassemble,
            Drill,
            CheckMate,
            Wildfire,
            FullMetalField,
            DoubleCheck,
            Hypercharge,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            Drill,
            DoubleCheck,
            CheckMate,
            HeatedSplitShot,
            DoubleCheck,
            HeatedSlugShot,
            HeatedCleanShot
        };

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Reassemble) < 2)
                return false;

            if (GetRemainingCharges(CheckMate) < 3)
                return false;

            if (GetRemainingCharges(DoubleCheck) < 3)
                return false;

            if (!ActionReady(Chainsaw))
                return false;

            if (!ActionReady(Wildfire))
                return false;

            if (!ActionReady(BarrelStabilizer))
                return false;

            if (!ActionReady(Excavator))
                return false;

            if (!ActionReady(FullMetalField))
                return false;

            return true;
        }
    }

    internal static class MCHHelper
    {
        internal static unsafe bool IsComboExpiring(float Times)
        {
            float GCD = GetCooldown(OriginalHook(SplitShot)).CooldownTotal * Times;

            return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < GCD;
        }

        internal static bool UseQueen(MCHGauge gauge)
        {
            if (!ActionWatching.HasDoubleWeaved() && !HasEffect(Buffs.Wildfire) &&
                !JustUsed(OriginalHook(Heatblast)) && LevelChecked(OriginalHook(RookAutoturret)) &&
                gauge is { IsRobotActive: false, Battery: >= 50 })
            {
                if (LevelChecked(FullMetalField))
                {
                    //1min
                    if ((BSUsed == 1) & (gauge.Battery >= 90))
                        return true;

                    //even mins
                    if (BSUsed >= 2 && gauge.Battery == 100)
                        return true;

                    //odd mins 1st queen
                    if (BSUsed >= 2 && gauge is { Battery: 50, LastSummonBatteryPower: 100 })
                        return true;

                    //odd mins 2nd queen
                    if (BSUsed % 3 is 2 && gauge is { Battery: >= 60, LastSummonBatteryPower: 50 })
                        return true;

                    //odd mins 2nd queen
                    if (BSUsed % 3 is 0 && gauge is { Battery: >= 70, LastSummonBatteryPower: 50 })
                        return true;

                    //odd mins 2nd queen
                    if (BSUsed % 3 is 1 && gauge is { Battery: >= 80, LastSummonBatteryPower: 50 })
                        return true;
                }

                if (!LevelChecked(FullMetalField))
                    if (gauge.Battery == 100)
                        return true;

                if (!LevelChecked(BarrelStabilizer))
                    return true;
            }

            return false;
        }

        internal static bool Reassembled(MCHGauge gauge)
        {
            if (!JustUsed(OriginalHook(Heatblast)) &&
                !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) && !JustUsed(OriginalHook(Heatblast)))
            {
                if (IsEnabled(CustomComboPreset.MCH_ST_AdvancedMode) && Config.MCH_ST_Reassembled[0] &&
                    IsNotEnabled(CustomComboPreset.MCH_Adv_TurretQueen) &&
                    LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                    return true;

                if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                     (IsEnabled(CustomComboPreset.MCH_ST_AdvancedMode) && Config.MCH_ST_Reassembled[0])) &&
                    IsEnabled(CustomComboPreset.MCH_Adv_TurretQueen) &&
                    LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady) &&
                    (BSUsed is 1 ||
                     (BSUsed % 3 is 2 && Gauge.Battery <= 40) ||
                     (BSUsed % 3 is 0 && Gauge.Battery <= 50) ||
                     (BSUsed % 3 is 1 && Gauge.Battery <= 60) ||
                     GetBuffRemainingTime(Buffs.ExcavatorReady) < 6))
                    return true;

                if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                     (IsEnabled(CustomComboPreset.MCH_ST_AdvancedMode) && Config.MCH_ST_Reassembled[1])) &&
                    LevelChecked(Chainsaw) && !LevelChecked(Excavator) &&
                    (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                     ActionReady(Chainsaw)) && !battery)
                    return true;

                if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                     (IsEnabled(CustomComboPreset.MCH_ST_AdvancedMode) && Config.MCH_ST_Reassembled[2])) &&
                    LevelChecked(AirAnchor) &&
                    (GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                     ActionReady(AirAnchor)) && !battery)
                    return true;

                if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                     (IsEnabled(CustomComboPreset.MCH_ST_AdvancedMode) && Config.MCH_ST_Reassembled[3])) &&
                    LevelChecked(Drill) &&
                    ((!LevelChecked(AirAnchor) && Config.MCH_ST_Reassembled[2]) || !Config.MCH_ST_Reassembled[2]) &&
                    (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                     ActionReady(Drill)))
                    return true;
            }

            return false;
        }

        internal static bool Tools(ref uint actionID)
        {
            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Excavator) && reassembledExcavatorST &&
                IsNotEnabled(CustomComboPreset.MCH_Adv_TurretQueen) &&
                LevelChecked(Excavator) &&
                HasEffect(Buffs.ExcavatorReady))
            {
                actionID = Excavator;

                return true;
            }

            if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                 (IsEnabled(CustomComboPreset.MCH_ST_Adv_Excavator) && reassembledExcavatorST)) &&
                IsEnabled(CustomComboPreset.MCH_Adv_TurretQueen) &&
                LevelChecked(Excavator) &&
                HasEffect(Buffs.ExcavatorReady) &&
                (BSUsed is 1 ||
                 (BSUsed % 3 is 2 && Gauge.Battery <= 40) ||
                 (BSUsed % 3 is 0 && Gauge.Battery <= 50) ||
                 (BSUsed % 3 is 1 && Gauge.Battery <= 60) ||
                 GetBuffRemainingTime(Buffs.ExcavatorReady) < 6))
            {
                actionID = Excavator;

                return true;
            }

            if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                 (IsEnabled(CustomComboPreset.MCH_ST_Adv_Chainsaw) && reassembledChainsawST)) &&
                LevelChecked(Chainsaw) && !battery &&
                (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                 ActionReady(Chainsaw)))
            {
                actionID = Chainsaw;

                return true;
            }

            if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                 (IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor) && reassembledAnchorST)) &&
                LevelChecked(AirAnchor) && !battery &&
                (GetCooldownRemainingTime(AirAnchor) <=
                    GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(AirAnchor)))
            {
                actionID = AirAnchor;

                return true;
            }

            if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                 (IsEnabled(CustomComboPreset.MCH_ST_Adv_Drill) && reassembledDrillST)) &&
                LevelChecked(Drill) &&
                !JustUsed(Drill) &&
                (GetCooldownRemainingTime(Drill) <= GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 ||
                 ActionReady(Drill)) && GetCooldownRemainingTime(Wildfire) is >= 20 or <= 10)
            {
                actionID = Drill;

                return true;
            }

            if ((IsEnabled(CustomComboPreset.MCH_ST_SimpleMode) ||
                 IsEnabled(CustomComboPreset.MCH_ST_Adv_AirAnchor)) &&
                LevelChecked(HotShot) && !LevelChecked(AirAnchor) && !battery &&
                (GetCooldownRemainingTime(HotShot) <=
                    GetCooldownRemainingTime(OriginalHook(SplitShot)) + 0.25 || ActionReady(HotShot)))
            {
                actionID = HotShot;

                return true;
            }

            return false;
        }
    }
}