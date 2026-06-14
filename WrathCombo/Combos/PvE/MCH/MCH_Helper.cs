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
    #region Queen

    private static bool CanQueen()
    {
        if (!HasStatusEffect(Buffs.Wildfire) &&
            ActionReady(OriginalHook(RookAutoturret)) &&
            !RobotActive &&
            GetTargetHPPercent() > HPThresholdQueen)
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

    #region Hypercharge

    private static bool CanHypercharge(bool onAoE = false)
    {
        switch (onAoE)
        {
            case true when GetTargetHPPercent() <= HyperchargeHPThresholdAoE:

            case false when GetTargetHPPercent() <= HyperchargeHPThresholdST:
                return false;

            default:
                switch (onAoE)
                {
                    case false when
                        (ActionReady(Hypercharge) || HasStatusEffect(Buffs.Hypercharged)) &&
                        (!IsComboExpiring(6) || ShouldSkipHyperchargeHold()) && !IsOverheated &&
                        IsDrillCD(CustomCooldownForHyperHold) && IsAirAnchorCD(CustomCooldownForHyperHold) &&
                        (IsChainSawCD(CustomCooldownForHyperHold) || ShouldSkipHyperchargeHold()) &&
                        (!HasStatusEffect(Buffs.ExcavatorReady) || ShouldSkipExcavatorHold()) &&
                        !HasStatusEffect(Buffs.FullMetalMachinist) &&
                        (ActionReady(Wildfire) ||
                         JustUsed(FullMetalField, GCD / 2) ||
                         (MCH_ST_WildfireBossOption == 1 && !TargetIsBoss()) ||
                         GetCooldownRemainingTime(Wildfire) > GCD * 15 ||
                         (Heat is 100 && GetCooldownRemainingTime(Wildfire) > 10) ||
                         !LevelChecked(Wildfire)):

                    case true when
                        (ActionReady(Hypercharge) || HasStatusEffect(Buffs.Hypercharged)) &&
                        !IsOverheated:
                        return true;
                }

                return false;
        }
    }

    public static bool IsWildfireAboutToBeUsed() =>
        IsEnabled(Preset.MCH_ST_Adv_WildFire) &&
        ((MCH_ST_WildfireBossOption == 0 && GetTargetHPPercent() > HPThresholdWildFire) || TargetIsBoss()) &&
        CanApplyStatus(CurrentTarget, Debuffs.Wildfire) &&
        ActionReady(Wildfire);

    public static bool ShouldSkipExcavatorHold()
    {
        if (!IsEnabled(Preset.MCH_ST_Adv_Tools_AllowExcavatorPostWildfire))
            return false;

        if (IsWildfireAboutToBeUsed())
            return true;

        return false;
    }

    public static bool ShouldSkipHyperchargeHold()
    {
        if (!IsEnabled(Preset.MCH_ST_Adv_Tools_AllowClainsawPostWildfire))
            return false;

        if (IsWildfireAboutToBeUsed())
            return true;

        return false;
    }

    #endregion

    #region Misc

    private static bool CanUseFullMetalField =>
        HasStatusEffect(Buffs.FullMetalMachinist) &&
        !IsOverheated &&
        (ActionReady(Wildfire) ||
         GetCooldownRemainingTime(Wildfire) > 90 ||
         GetCooldownRemainingTime(Wildfire) <= GCD ||
         GetStatusEffectRemainingTime(Buffs.FullMetalMachinist) <= 6);

    private static float CustomCooldownForHyperHold =>
        IsWildfireAboutToBeUsed() ? MCH_ST_WildfireHyperchargeCutoffThreshold : 9f;

    #endregion

    #region Reassembled

    private static uint CurrentReassembleCharges = uint.MaxValue;
    private static bool UseBothCharges;

    private static bool TwoChargesUnlocked => GetMaxCharges(Reassemble) >= 2;

    private static bool IsWildfireActive => HasStatusEffect(Buffs.Wildfire);

    private static void UpdateReassembleChargeTracking()
    {
        uint charges = GetRemainingCharges(Reassemble);
        if (charges == CurrentReassembleCharges)
            return;

        if (TwoChargesUnlocked)
        {
            switch (charges)
            {
                case 2 when CurrentReassembleCharges != 2:
                    UseBothCharges = true;
                    break;

                case 0:

                case 1 when CurrentReassembleCharges == 0:
                    UseBothCharges = false;
                    break;
            }
        }
        else
            UseBothCharges = false;

        CurrentReassembleCharges = charges;
    }

    private static bool ShouldReassemble() =>
        !TwoChargesUnlocked || UseBothCharges;

    private static int ReadyTools()
    {
        int numberOfReadyTools = 0;

        if (ActionReady(Drill))
            numberOfReadyTools += (int)GetRemainingCharges(Drill);

        if (ActionReady(Chainsaw))
        {
            numberOfReadyTools++;
            if (LevelChecked(Excavator))
                numberOfReadyTools++;
        }
        else if (HasStatusEffect(Buffs.ExcavatorReady))
        {
            numberOfReadyTools++;
        }

        if (ActionReady(OriginalHook(AirAnchor)))
            numberOfReadyTools++;

        if (!LevelChecked(Drill) && ComboTimer > 0 && ComboAction is SlugShot &&
            LevelChecked(CleanShot))
            numberOfReadyTools++;

        return numberOfReadyTools;
    }

    private static bool HasAoEReassembleTarget() =>
        LevelChecked(Drill) && GetCooldownRemainingTime(Drill) < GCD ||
        LevelChecked(AirAnchor) && GetCooldownRemainingTime(AirAnchor) < GCD ||
        LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) < GCD ||
        LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady) ||
        LevelChecked(Scattergun) && ActionReady(Scattergun) ||
        ActionReady(OriginalHook(SpreadShot));

    private static bool CanReassembleAoE()
    {
        uint remainingCharges = GetRemainingCharges(Reassemble);

        if (!ActionReady(Reassemble) || HasStatusEffect(Buffs.Reassembled) ||
            !HasBattleTarget() || GetTargetHPPercent() <= AoEReassembleHPThreshold ||
            !InReassembleRange() || JustUsed(Reassemble, 2f))
            return false;

        if (remainingCharges == 0 || remainingCharges <= AoEReassembleChargePool)
            return false;

        return HasAoEReassembleTarget();
    }

    private static bool InReassembleRange() =>
        LevelChecked(Drill) && InActionRange(Drill) ||
        LevelChecked(AirAnchor) && InActionRange(AirAnchor) ||
        LevelChecked(Chainsaw) && InActionRange(Chainsaw) ||
        LevelChecked(Scattergun) && InActionRange(OriginalHook(SpreadShot)) ||
        !LevelChecked(Drill) && InActionRange(OriginalHook(SpreadShot));

    private static bool CanReassembleST()
    {
        UpdateReassembleChargeTracking();

        uint remainingCharges = GetRemainingCharges(Reassemble);

        if (!ActionReady(Reassemble) || HasStatusEffect(Buffs.Reassembled) || IsWildfireActive || !HasBattleTarget() ||
            GetTargetHPPercent() <= ReassembleHPThreshold ||
            !InReassembleRange() || JustUsed(Reassemble, 2f))
            return false;

        if (remainingCharges == 0 || remainingCharges <= ReassembleChargePool || !ShouldReassemble())
            return false;

        if (!IsEnabled(Preset.MCH_ST_SimpleMode) && MCH_ST_Adv_ReassembleChoice == 0)
        {
            int numberOfReadyTools = ReadyTools();
            return numberOfReadyTools >= remainingCharges;
        }

        if (IsEnabled(Preset.MCH_ST_SimpleMode) || MCH_ST_Adv_ReassembleChoice == 1)
        {
            if (ActionReady(Excavator) && HasStatusEffect(Buffs.ExcavatorReady))
                return true;

            if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
                return true;

            if (ActionReady(AirAnchor) && (!LevelChecked(Chainsaw) || GetCooldownRemainingTime(Chainsaw) > GCD * 2))
                return true;

            if (ActionReady(Drill) && (!LevelChecked(AirAnchor) || GetCooldownRemainingTime(AirAnchor) > GCD * 2))
                return true;

            if (!LevelChecked(Drill) && ComboTimer > 0 && ComboAction is SlugShot && LevelChecked(CleanShot))
                return true;
        }

        return false;
    }

    #endregion

    #region Gauss and Rico

    private static bool OvercapGaussRound =>
        ActionReady(OriginalHook(GaussRound)) && ((!LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(GaussRound)) is 1 ||
                                                   LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(GaussRound)) is 2) &&
                                                  GetCooldownChargeRemainingTime(OriginalHook(GaussRound)) < 25 ||
                                                  !LevelChecked(Hypercharge) && GetRemainingCharges(OriginalHook(GaussRound)) is 2);

    private static bool OvercapRicochet =>
        ActionReady(OriginalHook(Ricochet)) && (!LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(Ricochet)) is 1 ||
                                                LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(OriginalHook(Ricochet)) is 2) &&
        GetCooldownChargeRemainingTime(OriginalHook(Ricochet)) < 25;

    private static bool CanGaussRound =>
        ActionReady(OriginalHook(GaussRound)) &&
        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet));

    private static bool CanRicochet =>
        ActionReady(OriginalHook(Ricochet)) &&
        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound));

    #endregion

    #region HP Treshold

    private static int ReassembleHPThreshold =>
        IsEnabled(Preset.MCH_ST_SimpleMode) ? 25 : HPThresholdReassemble;

    private static int ReassembleChargePool =>
        IsEnabled(Preset.MCH_ST_SimpleMode) ? 0 : MCH_ST_ReassemblePool;

    private static int AoEReassembleHPThreshold =>
        IsEnabled(Preset.MCH_AoE_SimpleMode) ? 25 : MCH_AoE_ReassembleHPThreshold;

    private static int AoEReassembleChargePool =>
        IsEnabled(Preset.MCH_AoE_SimpleMode) ? 0 : MCH_AoE_ReassemblePool;

    private static int HyperchargeHPThresholdST =>
        IsEnabled(Preset.MCH_ST_SimpleMode) ? 25 : HPThresholdHypercharge;

    private static int HyperchargeHPThresholdAoE =>
        IsEnabled(Preset.MCH_AoE_SimpleMode) ? 25 : MCH_AoE_HyperchargeHPThreshold;

    private static int HPThresholdQueen =>
        MCH_ST_QueenBossOption == 1 ||
        !InBossEncounter() ? MCH_ST_QueenHPOption : 0;

    private static int HPThresholdHypercharge =>
        MCH_ST_HyperchargeBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_HyperchargeHPOption : 0;

    private static int HPThresholdReassemble =>
        MCH_ST_ReassembleBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_ReassembleHPOption : 0;

    private static int HPThresholdTools =>
        MCH_ST_ToolsBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_ToolsHPOption : 0;

    private static int HPThresholdBarrelStabilizer =>
        MCH_ST_BarrelStabilizerHPBossOption == 1 ||
        !TargetIsBoss() ? MCH_ST_BarrelStabilizerHPOption : 0;

    private static int HPThresholdWildFire =>
        MCH_ST_WildfireBossHPOption == 1 ||
        !TargetIsBoss() ? MCH_ST_WildfireHPOption : 0;

    #endregion

    #region Tools

    private static bool IsDrillCD(float time = 9f) =>
        !ActionReady(Drill) ||
        !TraitLevelChecked(Traits.EnhancedMultiWeapon) && GetCooldownRemainingTime(Drill) >= time ||
        TraitLevelChecked(Traits.EnhancedMultiWeapon) && GetRemainingCharges(Drill) < GetMaxCharges(Drill) && GetCooldownChargeRemainingTime(Drill) >= time;

    private static bool IsAirAnchorCD(float time = 9f) =>
        !LevelChecked(OriginalHook(HotShot)) ||
        LevelChecked(OriginalHook(HotShot)) && GetCooldownRemainingTime(OriginalHook(HotShot)) >= time;

    private static bool IsChainSawCD(float time = 9f) =>
        !LevelChecked(Chainsaw) ||
        LevelChecked(Chainsaw) && GetCooldownRemainingTime(Chainsaw) >= time;

    private static bool CanUseTools(ref uint actionID)
    {
        if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
        {
            actionID = Chainsaw;
            return true;
        }

        if (ActionReady(Excavator) &&
            (!IsEnabled(Preset.MCH_ST_Adv_Tools_AllowClainsawPostWildfire) || !ShouldSkipHyperchargeHold()))
        {
            actionID = Excavator;
            return true;
        }

        if (ActionReady(AirAnchor))
        {
            actionID = AirAnchor;
            return true;
        }

        if (ActionReady(Drill))
        {
            actionID = Drill;
            return true;
        }

        if (ActionReady(HotShot) && !LevelChecked(AirAnchor) && !HasStatusEffect(Buffs.Reassembled))
        {
            actionID = HotShot;
            return true;
        }

        return false;
    }

    private static bool CanUseAoETools(ref uint actionID, bool useAirAnchor = true)
    {
        if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
        {
            actionID = Chainsaw;
            return true;
        }

        if (ActionReady(Excavator))
        {
            actionID = Excavator;
            return true;
        }

        if (useAirAnchor && ActionReady(OriginalHook(AirAnchor)))
        {
            actionID = OriginalHook(AirAnchor);
            return true;
        }

        if (ActionReady(BioBlaster) &&
            !HasStatusEffect(Debuffs.Bioblaster, CurrentTarget) &&
            CanApplyStatus(CurrentTarget, Debuffs.Bioblaster))
        {
            actionID = BioBlaster;
            return true;
        }

        if (ActionReady(Drill))
        {
            actionID = Drill;
            return true;
        }

        if (HasStatusEffect(Buffs.Reassembled) && ActionReady(OriginalHook(SpreadShot)))
        {
            actionID = OriginalHook(SpreadShot);
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

        public override Preset Preset => Preset.MCH_ST_Adv_Opener;

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
        public override Preset Preset => Preset.MCH_ST_Adv_Opener;
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
        public override Preset Preset => Preset.MCH_ST_Adv_Opener;
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
