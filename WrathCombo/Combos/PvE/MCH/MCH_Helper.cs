using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.MCH.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    #region Hypercharge

    private static bool CanHypercharge(bool onAoE = false)
    {
        switch (onAoE)
        {
            case false when
                (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) &&
                !IsComboExpiring(6) && ActionReady(Hypercharge) &&
                !JustUsed(BarrelStabilizer) &&
                DrillCD && AirAnchorCD && ChainSawCD &&
                !HasStatusEffect(Buffs.ExcavatorReady) &&
                (ActionReady(Wildfire) ||
                 MCH_ST_WildfireBossOption == 1 && !TargetIsBoss() ||
                 GetCooldownRemainingTime(Wildfire) > GCD * 15 ||
                 Heat is 100 && GetCooldownRemainingTime(Wildfire) > 10 ||
                 !LevelChecked(Wildfire)):

            case true when
                (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                LevelChecked(AutoCrossbow) &&
                (LevelChecked(BioBlaster) && GetCooldownRemainingTime(BioBlaster) > 10 ||
                 !LevelChecked(BioBlaster)) &&
                (LevelChecked(Flamethrower) && GetCooldownRemainingTime(Flamethrower) > 10 ||
                 !LevelChecked(Flamethrower) || IsNotEnabled(Preset.MCH_AoE_Adv_FlameThrower)):
                return true;
        }

        return false;
    }

        #endregion

    #region Queen

    private static bool CanQueen()
    {
        if (!HasStatusEffect(Buffs.Wildfire) &&
            ActionReady(RookAutoturret) &&
            !RobotActive && Battery >= 50)
        {
            if (LevelChecked(Wildfire))
            {
                if (MCH_ST_WildfireBossOption == 0 || TargetIsBoss())
                {
                    switch (Battery)
                    {
                        //Always use on 100
                        case 100:

                        //Failsafe
                        case > 80 when
                            HasStatusEffect(Buffs.ExcavatorReady) ||
                            ActionReady(Chainsaw) ||
                            ActionReady(OriginalHook(AirAnchor)):

                        case > 90 when ComboAction == OriginalHook(SlugShot):
                            return true;
                    }
                }

                if (MCH_ST_WildfireBossOption == 1 && !TargetIsBoss() && Battery >= MCH_ST_TurretUsage)
                    return true;
            }

            if (!LevelChecked(Wildfire) && Battery >= MCH_ST_TurretUsage)
                return true;
        }

        return false;
    }

    #endregion

    #region Reassembled

    private static bool CanReassemble()
    {
        if (HasStatusEffect(Buffs.Reassembled) || !HasBattleTarget())
            return false;

        ushort maxCharges = GetMaxCharges(Reassemble);
        uint remainingCharges = GetRemainingCharges(Reassemble);

        switch (maxCharges)
        {
            case 1 when remainingCharges == 1 && InActionRange(Drill) &&
                (ActionReady(Drill) || LevelChecked(AirAnchor) && ActionReady(AirAnchor)):
                return true;

            case 2:
            {
                int numberOfReadyTools = 0;

                if (ActionReady(Drill))
                    numberOfReadyTools += (int)GetRemainingCharges(Drill);

                if (ActionReady(Chainsaw))
                    numberOfReadyTools++;

                if (ActionReady(AirAnchor))
                    numberOfReadyTools++;

                if (ActionReady(Excavator))
                    numberOfReadyTools++;

                switch (remainingCharges)
                {
                    case 2 when numberOfReadyTools >= 2 && InActionRange(Drill):

                    //Kage tweak this last number if needed, it could maybe go to 50
                    case 1 when numberOfReadyTools >= 1 && GetCooldownChargeRemainingTime(Reassemble) >= 45 && InActionRange(Drill):
                        return true;
                }

                break;
            }
        }

        return false;
    }

    #endregion

    #region Gauss and Rico

    private static bool OvercapGaussRound =>
        ActionReady(GaussRound) && ((!LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(GaussRound)) is 1 ||
                                     LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(GaussRound)) is 2) &&
                                    GetCooldownChargeRemainingTime(OriginalHook(GaussRound)) < 25 ||
                                    !LevelChecked(Hypercharge) && GetRemainingCharges(OriginalHook(GaussRound)) is 2);

    private static bool OvercapRicochet =>
        ActionReady(Ricochet) && (!LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(Ricochet)) is 1 ||
                                  LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(Ricochet)) is 2) &&
        GetCooldownChargeRemainingTime(OriginalHook(Ricochet)) < 25;

    private static bool CanGaussRound =>
        ActionReady(GaussRound) &&
        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet));

    private static bool CanRicochet =>
        ActionReady(Ricochet) &&
        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound));

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

    private static int HPThresholdBarrelStabilizer =>
        MCH_ST_BarrelStabilizerHPBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_BarrelStabilizerHPBossOption : 0;

    private static int HPThresholdWildFire =>
        MCH_ST_WildfireBossHPOption == 1 ||
        !TargetIsBoss() ? MCH_ST_WildfireBossHPOption : 0;

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

    private static bool CanUseTools(ref uint actionID)
    {
        if (LevelChecked(AirAnchor) && ActionReady(AirAnchor))
        {
            actionID = AirAnchor;
            return true;
        }

        if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
        {
            actionID = Chainsaw;
            return true;
        }

        if (ActionReady(Excavator) && HasStatusEffect(Buffs.ExcavatorReady))
        {
            actionID = Excavator;
            return true;
        }

        if (ActionReady(Drill))
        {
            actionID = Drill;
            return true;
        }

        if (ActionReady(HotShot))
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
        if (Lvl100StandardOpener.LevelChecked &&
            MCH_SelectedOpener == 0)
            return Lvl100StandardOpener;

        if (Lvl100EarlyWFOpener.LevelChecked &&
            MCH_SelectedOpener == 1)
            return Lvl100EarlyWFOpener;

        if (Lvl90EarlyTools.LevelChecked)
            return Lvl90EarlyTools;

        return WrathOpener.Dummy;
    }

    internal static MCHLvl90EarlyToolsOpener Lvl90EarlyTools = new();
    internal static MCHLvl100EarlyWFOpener Lvl100EarlyWFOpener = new();
    internal static MCHLvl100StandardOpener Lvl100StandardOpener = new();

    internal class MCHLvl100StandardOpener : WrathOpener
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

    internal class MCHLvl100EarlyWFOpener : WrathOpener
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
            Reassemble,
            Chainsaw,
            DoubleCheck,
            Wildfire,
            Excavator,
            Hypercharge,
            AutomatonQueen,
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
            FullMetalField,
            DoubleCheck,
            CheckMate,
            Drill,
            HeatedSplitShot,
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

    private static MCHGauge Gauge => GetJobGauge<MCHGauge>();

    private static bool IsOverheated => Gauge.IsOverheated;

    private static bool RobotActive => Gauge.IsRobotActive;

    private static byte Heat => Gauge.Heat;

    private static byte Battery => Gauge.Battery;

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
            EnhancedMultiWeapon = 605,
            ChargedActionMastery = 292;
    }

    #endregion
}
