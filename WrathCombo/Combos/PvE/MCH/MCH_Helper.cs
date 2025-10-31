﻿using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.MCH.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Data.ActionWatching;
namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    private static int BSUsed =>
        CombatActions.Count(x => x == BarrelStabilizer);

    private static bool CanGaussRound =>
        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet));

    private static bool CanRicochet =>
        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound));

    #region Hypercharge

    private static bool CanHypercharge(bool onAoE = false)
    {
        switch (onAoE)
        {
            case false when
                (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) &&
                !IsComboExpiring(6) && ActionReady(Hypercharge) &&
                DrillCD && AirAnchorCD && ChainSawCD &&
                (LevelChecked(FullMetalField) && JustUsed(FullMetalField) ||
                 !LevelChecked(FullMetalField) && ActionReady(Wildfire) ||
                 GetCooldownRemainingTime(Wildfire) > 40 ||
                 !LevelChecked(Wildfire)):

            case true when
                (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                LevelChecked(AutoCrossbow) &&
                (LevelChecked(BioBlaster) && GetCooldownRemainingTime(BioBlaster) > 10 ||
                 !LevelChecked(BioBlaster) || IsNotEnabled(Preset.MCH_AoE_Adv_Bioblaster)) &&
                (LevelChecked(Flamethrower) && GetCooldownRemainingTime(Flamethrower) > 10 ||
                 !LevelChecked(Flamethrower) || IsNotEnabled(Preset.MCH_AoE_Adv_FlameThrower)):
                return true;

        }

        return false;
    }

        #endregion

    #region Queen

    private static bool CanQueen(bool simpleMode = false)
    {
        if (!HasStatusEffect(Buffs.Wildfire) &&
            !JustUsed(OriginalHook(Heatblast)) && ActionReady(RookAutoturret) &&
            !RobotActive && Battery >= 50)
        {
            if ((MCH_ST_QueenBossOption == 0 || InBossEncounter() ||
                 simpleMode && InBossEncounter()) &&
                (GetCooldownRemainingTime(Wildfire) > GCD || !LevelChecked(Wildfire)))
            {
                if (LevelChecked(BarrelStabilizer))
                {
                    switch (BSUsed)
                    {
                        //1min
                        case 1 when Battery >= 90:
 
                        //even mins
                        case >= 2 when Battery == 100:
     
                        //odd mins 1st queen
                        case >= 2 when Battery is 50 && LastSummonBattery is 100:
                            return true;
                    }

                    //odd mins 2nd queen
                    if ((BSUsed % 3 is 2 && Battery >= 60 ||
                         BSUsed % 3 is 0 && Battery >= 70 ||
                         BSUsed % 3 is 1 && Battery >= 80) && LastSummonBattery is 50)
                        return true;
                }

                if (!LevelChecked(BarrelStabilizer))
                    return true;
            }

            if (simpleMode && !InBossEncounter() && Battery is 100 ||
                MCH_ST_QueenBossOption == 1 && !InBossEncounter() && Battery >= MCH_ST_TurretUsage)
                return true;
        }

        return false;
    }

    #endregion

    #region HP Treshold

    private static int HPThresholdHypercharge =>
        MCH_ST_HyperchargeBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_HyperchargeHPOption : 0;

    private static int HPThresholdReassemble =>
        MCH_ST_ReassembleBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_ReassembleHPOption : 0;

    private static int HPThresholdTools =>
        MCH_ST_ToolsBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_ToolsBossOption : 0;

    #endregion

    #region Reassembled

    #region Variables

    private static bool ReassembledExcavatorST =>
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && MCH_ST_Reassembled[0] && (HasStatusEffect(Buffs.Reassembled) || !HasStatusEffect(Buffs.Reassembled)) ||
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && !MCH_ST_Reassembled[0] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_ST_ReassemblePool ||
        !IsEnabled(Preset.MCH_ST_Adv_Reassemble);

    private static bool ReassembledChainsawST =>
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && MCH_ST_Reassembled[1] && (HasStatusEffect(Buffs.Reassembled) || !HasStatusEffect(Buffs.Reassembled)) ||
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && !MCH_ST_Reassembled[1] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_ST_ReassemblePool ||
        !IsEnabled(Preset.MCH_ST_Adv_Reassemble);

    private static bool ReassembledAnchorST =>
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && MCH_ST_Reassembled[2] && (HasStatusEffect(Buffs.Reassembled) || !HasStatusEffect(Buffs.Reassembled)) ||
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && !MCH_ST_Reassembled[2] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_ST_ReassemblePool ||
        !IsEnabled(Preset.MCH_ST_Adv_Reassemble);

    private static bool ReassembledDrillST =>
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && MCH_ST_Reassembled[3] && (HasStatusEffect(Buffs.Reassembled) || !HasStatusEffect(Buffs.Reassembled)) ||
        IsEnabled(Preset.MCH_ST_Adv_Reassemble) && !MCH_ST_Reassembled[3] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_ST_ReassemblePool ||
        !IsEnabled(Preset.MCH_ST_Adv_Reassemble);

    private static bool ReassembledExcavatorAoE =>
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[3] && HasStatusEffect(Buffs.Reassembled) ||
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && !MCH_AoE_Reassembled[3] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_AoE_ReassemblePool ||
        !IsEnabled(Preset.MCH_AoE_Adv_Reassemble);

    private static bool ReassembledChainsawAoE =>
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[2] && HasStatusEffect(Buffs.Reassembled) ||
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && !MCH_AoE_Reassembled[2] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_AoE_ReassemblePool ||
        !IsEnabled(Preset.MCH_AoE_Adv_Reassemble);

    private static bool ReassembledAirAnchorAoE =>
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[1] && HasStatusEffect(Buffs.Reassembled) ||
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && !MCH_AoE_Reassembled[1] && !HasStatusEffect(Buffs.Reassembled) ||
        !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_AoE_ReassemblePool ||
        !IsEnabled(Preset.MCH_AoE_Adv_Reassemble);

    private static bool ReassembledScattergunAoE =>
        IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[0] && HasStatusEffect(Buffs.Reassembled);

    #endregion

    private static bool CanReassemble(bool onExcavator, bool onChainsaw, bool onAirAnchor, bool onDrill)
    {
        if (!JustUsed(OriginalHook(Heatblast)) &&
            !HasStatusEffect(Buffs.Reassembled) && ActionReady(Reassemble))
        {
            switch (onExcavator)
            {
                case true when
                    IsNotEnabled(Preset.MCH_ST_Adv_TurretQueen) || MCH_ST_QueenBossOption == 1 && !InBossEncounter() &&
                    LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady):

                case true when
                    IsEnabled(Preset.MCH_ST_Adv_TurretQueen) && (MCH_ST_QueenBossOption == 0 || InBossEncounter()) &&
                    LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady) &&
                    (BSUsed is 1 ||
                     BSUsed % 3 is 2 && Battery <= 40 ||
                     BSUsed % 3 is 0 && Battery <= 50 ||
                     BSUsed % 3 is 1 && Battery <= 60 ||
                     GetStatusEffectRemainingTime(Buffs.ExcavatorReady) <= 6):
                    return true;
            }

            if (onChainsaw &&
                LevelChecked(Chainsaw) && !MaxBattery &&
                GetCooldownRemainingTime(Chainsaw) < GCD &&
                (!LevelChecked(Excavator) || !MCH_ST_Reassembled[0]))
                return true;

            if (onAirAnchor &&
                LevelChecked(AirAnchor) && !MaxBattery &&
                GetCooldownRemainingTime(AirAnchor) < GCD &&
                (!LevelChecked(Excavator) || MCH_ST_Reassembled[0] && GetCooldownRemainingTime(Chainsaw) > 40 || !MCH_ST_Reassembled[0]))
                return true;

            if (onDrill &&
                LevelChecked(Drill) &&
                (TraitLevelChecked(Traits.EnhancedMultiWeapon) && GetRemainingCharges(Drill) is 1 or 2 ||
                 GetCooldownRemainingTime(Drill) < GCD) &&
                GetCooldownRemainingTime(Wildfire) is >= 20 or <= 10 &&
                (!LevelChecked(AirAnchor) || MCH_ST_Reassembled[2] && GetCooldownRemainingTime(AirAnchor) > 20 || !MCH_ST_Reassembled[2]) &&
                (!LevelChecked(Chainsaw) || MCH_ST_Reassembled[1] && GetCooldownRemainingTime(Chainsaw) > 40 || !MCH_ST_Reassembled[1]) &&
                (!LevelChecked(Excavator) || MCH_ST_Reassembled[0] && GetCooldownRemainingTime(Chainsaw) > 40 || !MCH_ST_Reassembled[0]))
                return true;
        }

        return false;
    }

    #endregion

    #region Cooldowns

    private static bool DrillCD =>
        !LevelChecked(Drill) ||
        !TraitLevelChecked(Traits.EnhancedMultiWeapon) && GetCooldownRemainingTime(Drill) >= 9 ||
        TraitLevelChecked(Traits.EnhancedMultiWeapon) && GetRemainingCharges(Drill) < GetMaxCharges(Drill) && GetCooldownRemainingTime(Drill) >= 9;

    private static bool AirAnchorCD =>
        !LevelChecked(OriginalHook(AirAnchor)) ||
        LevelChecked(OriginalHook(AirAnchor)) && GetCooldownRemainingTime(OriginalHook(AirAnchor)) >= 9;

    private static bool ChainSawCD =>
        !LevelChecked(Chainsaw) ||
        LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) >= 9;

    private static bool CanUseTools(ref uint actionID, bool useExcavator, bool useChainsaw, bool useAirAnchor, bool useDrill, bool simpleMode = false)
    {
        switch (useExcavator)
        {
            case true when
                ReassembledExcavatorST &&
                (IsNotEnabled(Preset.MCH_ST_Adv_TurretQueen) || (MCH_ST_QueenBossOption == 1 || simpleMode) && !InBossEncounter()) &&
                LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady):

            case true when
                ReassembledExcavatorST &&
                (IsEnabled(Preset.MCH_ST_Adv_TurretQueen) && (MCH_ST_QueenBossOption == 0 || InBossEncounter()) ||
                 simpleMode && InBossEncounter()) &&
                LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady) &&
                (BSUsed is 1 ||
                 BSUsed % 3 is 2 && Battery <= 40 ||
                 BSUsed % 3 is 0 && Battery <= 50 ||
                 BSUsed % 3 is 1 && Battery <= 60 ||
                 GetStatusEffectRemainingTime(Buffs.ExcavatorReady) <= 6):
                actionID = Excavator;
                return true;
        }

        if (useChainsaw &&
            ReassembledChainsawST &&
            !MaxBattery && !HasStatusEffect(Buffs.ExcavatorReady) && LevelChecked(Chainsaw) &&
            GetCooldownRemainingTime(Chainsaw) <= GCD / 2)
        {
            actionID = Chainsaw;
            return true;
        }

        if (useAirAnchor &&
            ReassembledAnchorST &&
            !MaxBattery && LevelChecked(AirAnchor) &&
            GetCooldownRemainingTime(AirAnchor) <= GCD / 2)
        {
            actionID = AirAnchor;
            return true;
        }

        if (useDrill &&
            ReassembledDrillST &&
            LevelChecked(Drill) &&
            (TraitLevelChecked(Traits.EnhancedMultiWeapon) && GetRemainingCharges(Drill) is 1 or 2 ||
             GetCooldownRemainingTime(Drill) < GCD / 2) &&
            GetCooldownRemainingTime(Wildfire) is >= 20 or <= 10)
        {
            actionID = Drill;
            return true;
        }

        if (useAirAnchor &&
            LevelChecked(HotShot) && !LevelChecked(AirAnchor) && !MaxBattery &&
            GetCooldownRemainingTime(HotShot) <= GCD / 2)
        {
            actionID = HotShot;
            return true;
        }
        return false;
    }

    #endregion

    #region Combos

    private static float GCD => GetCooldown(OriginalHook(SplitShot)).CooldownTotal;

    private static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GCD * times;

        return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < gcd;
    }

  #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Lvl90EarlyTools.LevelChecked)
            return Lvl90EarlyTools;

        if (StandardOpener.LevelChecked)
            return StandardOpener;

        return WrathOpener.Dummy;
    }

    internal static MCHStandardOpener StandardOpener = new();
    internal static MCHLvl90EarlyToolsOpener Lvl90EarlyTools = new();

    internal class MCHStandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
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
            Hypercharge,
            DoubleCheck,
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
        ];

        internal override UserData ContentCheckConfig => MCH_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => 4)
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(Reassemble) is 2 &&
            GetRemainingCharges(OriginalHook(GaussRound)) is 3 &&
            GetRemainingCharges(OriginalHook(Ricochet)) is 3 &&
            IsOffCooldown(Chainsaw) &&
            IsOffCooldown(Wildfire) &&
            IsOffCooldown(BarrelStabilizer) &&
            IsOffCooldown(Excavator) &&
            IsOffCooldown(FullMetalField);
    }

    internal class MCHLvl90EarlyToolsOpener : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            Reassemble,
            AirAnchor,
            GaussRound,
            Ricochet,
            Drill,
            BarrelStabilizer,
            Chainsaw,
            GaussRound,
            Ricochet,
            HeatedSplitShot,
            GaussRound,
            Ricochet,
            HeatedSlugShot,
            Wildfire,
            HeatedCleanShot,
            AutomatonQueen,
            Hypercharge,
            BlazingShot,
            Ricochet,
            BlazingShot,
            GaussRound,
            BlazingShot,
            Ricochet,
            BlazingShot,
            GaussRound,
            BlazingShot,
            Reassemble,
            Drill
        ];

        internal override UserData ContentCheckConfig => MCH_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => 4)
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            14
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(Reassemble) is 2 &&
            GetRemainingCharges(OriginalHook(GaussRound)) is 3 &&
            GetRemainingCharges(OriginalHook(Ricochet)) is 3 &&
            IsOffCooldown(Chainsaw) &&
            IsOffCooldown(Wildfire) &&
            IsOffCooldown(BarrelStabilizer);
    }

    #endregion

    #region Gauge

    private static MCHGauge Gauge = GetJobGauge<MCHGauge>();

    private static bool IsOverheated => Gauge.IsOverheated;

    private static bool RobotActive => Gauge.IsRobotActive;

    private static byte LastSummonBattery => Gauge.LastSummonBatteryPower;

    private static byte Heat => Gauge.Heat;

    private static byte Battery => Gauge.Battery;

    private static bool MaxBattery => Battery >= 90;

    #endregion

    #region ID's

    public const uint
        CleanShot = 2873,
        HeatedCleanShot = 7413,
        SplitShot = 2866,
        HeatedSplitShot = 7411,
        SlugShot = 2868,
        HeatedSlugShot = 7412,
        GaussRound = 2874,
        Ricochet = 2890,
        Reassemble = 2876,
        Drill = 16498,
        HotShot = 2872,
        AirAnchor = 16500,
        Hypercharge = 17209,
        Heatblast = 7410,
        SpreadShot = 2870,
        Scattergun = 25786,
        AutoCrossbow = 16497,
        RookAutoturret = 2864,
        RookOverdrive = 7415,
        AutomatonQueen = 16501,
        QueenOverdrive = 16502,
        Tactician = 16889,
        Chainsaw = 25788,
        BioBlaster = 16499,
        BarrelStabilizer = 7414,
        Wildfire = 2878,
        Dismantle = 2887,
        Flamethrower = 7418,
        BlazingShot = 36978,
        DoubleCheck = 36979,
        CheckMate = 36980,
        Excavator = 36981,
        FullMetalField = 36982;

    public static class Buffs
    {
        public const ushort
            Reassembled = 851,
            Tactician = 1951,
            Wildfire = 1946,
            Overheated = 2688,
            Flamethrower = 1205,
            Hypercharged = 3864,
            ExcavatorReady = 3865,
            FullMetalMachinist = 3866;
    }

    public static class Debuffs
    {
        public const ushort
            Dismantled = 860,
            Wildfire = 861,
            Bioblaster = 1866;
    }

    public static class Traits
    {
        public const ushort
            EnhancedMultiWeapon = 605;
    }

    #endregion

}
