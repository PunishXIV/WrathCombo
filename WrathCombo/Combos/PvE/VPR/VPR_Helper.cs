using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager;
using static WrathCombo.Combos.PvE.VPR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    #region Basic Combo

    private static uint DoBasicCombo(uint actionId, bool useTrueNorth = true, bool isAoE = false)
    {
        switch (isAoE)
        {
            case false:
            {
                //1-2-3 (4-5-6) Combo
                if (ComboTimer > 0)
                {
                    if (ComboAction is ReavingFangs or SteelFangs)
                    {
                        if (LevelChecked(SwiftskinsSting) &&
                            (HasHindVenom || NoSwiftscaled || NoBasicComboVenom))
                            return OriginalHook(ReavingFangs);

                        if (LevelChecked(HuntersSting) &&
                            (HasFlankVenom || NoHuntersInstinct))
                            return OriginalHook(SteelFangs);
                    }

                    if (ComboAction is HuntersSting or SwiftskinsSting)
                    {
                        if ((HasStatusEffect(Buffs.FlanksbaneVenom) || HasStatusEffect(Buffs.HindsbaneVenom)) &&
                            LevelChecked(HindstingStrike))
                            return useTrueNorth &&
                                   Role.CanTrueNorth() &&
                                   (!OnTargetsRear() && HasStatusEffect(Buffs.HindsbaneVenom) ||
                                    !OnTargetsFlank() && HasStatusEffect(Buffs.FlanksbaneVenom))
                                ? Role.TrueNorth
                                : OriginalHook(ReavingFangs);

                        if ((HasStatusEffect(Buffs.FlankstungVenom) || HasStatusEffect(Buffs.HindstungVenom)) &&
                            LevelChecked(FlanksbaneFang))
                            return useTrueNorth &&
                                   Role.CanTrueNorth() &&
                                   (!OnTargetsRear() && HasStatusEffect(Buffs.HindstungVenom) ||
                                    !OnTargetsFlank() && HasStatusEffect(Buffs.FlankstungVenom))
                                ? Role.TrueNorth
                                : OriginalHook(SteelFangs);
                    }

                    if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                        return LevelChecked(ReavingFangs) && HasStatusEffect(Buffs.HonedReavers)
                            ? OriginalHook(ReavingFangs)
                            : OriginalHook(SteelFangs);
                }

                //LowLevels
                if (LevelChecked(ReavingFangs) &&
                    (HasStatusEffect(Buffs.HonedReavers) ||
                     !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                    return OriginalHook(ReavingFangs);

                return actionId;
            }

            case true:
            {
                //1-2-3 (4-5-6) Combo
                if (ComboTimer > 0)
                {
                    if (ComboAction is ReavingMaw or SteelMaw)
                    {
                        if (LevelChecked(HuntersBite) &&
                            HasStatusEffect(Buffs.GrimhuntersVenom))
                            return OriginalHook(SteelMaw);

                        if (LevelChecked(SwiftskinsBite) &&
                            (HasStatusEffect(Buffs.GrimskinsVenom) ||
                             !HasStatusEffect(Buffs.Swiftscaled) && !HasStatusEffect(Buffs.HuntersInstinct)))
                            return OriginalHook(ReavingMaw);
                    }

                    if (ComboAction is HuntersBite or SwiftskinsBite)
                    {
                        if (HasStatusEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                            return OriginalHook(SteelMaw);

                        if (HasStatusEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                            return OriginalHook(ReavingMaw);
                    }

                    if (ComboAction is BloodiedMaw or JaggedMaw)
                        return LevelChecked(ReavingMaw) && HasStatusEffect(Buffs.HonedReavers)
                            ? OriginalHook(ReavingMaw)
                            : OriginalHook(SteelMaw);
                }

                //for lower lvls
                if (LevelChecked(ReavingMaw) &&
                    (HasStatusEffect(Buffs.HonedReavers) ||
                     !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                    return OriginalHook(ReavingMaw);

                return actionId;
            }
        }
    }

    #endregion

    #region Misc

    private static float IreCD =>
        GetCooldownRemainingTime(SerpentsIre);

    private static bool MaxCoils =>
        TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoilStacks > 2 ||
        !TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoilStacks > 1;

    private static bool HasRattlingCoilStacks =>
        RattlingCoilStacks > 0;

    private static bool HasHindVenom =>
        HasStatusEffect(Buffs.HindstungVenom) ||
        HasStatusEffect(Buffs.HindsbaneVenom);

    private static bool HasFlankVenom =>
        HasStatusEffect(Buffs.FlankstungVenom) ||
        HasStatusEffect(Buffs.FlanksbaneVenom);

    private static bool NoSwiftscaled =>
        !HasStatusEffect(Buffs.Swiftscaled);

    private static bool NoHuntersInstinct =>
        !HasStatusEffect(Buffs.HuntersInstinct);

    private static bool NoBasicComboVenom =>
        !HasStatusEffect(Buffs.FlanksbaneVenom) &&
        !HasStatusEffect(Buffs.FlankstungVenom) &&
        !HasStatusEffect(Buffs.HindsbaneVenom) &&
        !HasStatusEffect(Buffs.HindstungVenom);

    private static bool NoSTComboWeaves =>
        !HasStatusEffect(Buffs.HuntersVenom) &&
        !HasStatusEffect(Buffs.SwiftskinsVenom) &&
        !HasStatusEffect(Buffs.PoisedForTwinblood) &&
        !HasStatusEffect(Buffs.PoisedForTwinfang);

    private static bool NoAoEComboWeaves =>
        !HasStatusEffect(Buffs.FellhuntersVenom) &&
        !HasStatusEffect(Buffs.FellskinsVenom) &&
        !HasStatusEffect(Buffs.PoisedForTwinblood) &&
        !HasStatusEffect(Buffs.PoisedForTwinfang);

    // private static bool RefreshHuntersInstinct =>
    //   GetStatusEffectRemainingTime(Buffs.HuntersInstinct) <= GCD * 6;

    // private static bool RefreshSwiftscaled =>
    //   GetStatusEffectRemainingTime(Buffs.Swiftscaled) <= GCD * 6;

    #endregion

    #region Reawaken

    private static bool CanReawaken(bool isAoE = false)
    {
        int hpThresholdUsageST = IsNotEnabled(Preset.VPR_ST_SimpleMode) ? ComputeHpThresholdReawaken() : 0;
        int hpThresholdDontSaveST = IsNotEnabled(Preset.VPR_ST_SimpleMode) ? VPR_ST_ReAwaken_Threshold : 5;
        int hpThresholdUsageAoE = IsNotEnabled(Preset.VPR_AoE_SimpleMode) ? VPR_AoE_Reawaken_Usage : 40;

        switch (isAoE)
        {
            case false:
            {
                if (ActionReady(Reawaken) && !HasStatusEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                    NoSTComboWeaves && HasBattleTarget() &&
                    !IsEmpowermentExpiring(6) && !IsComboExpiring(6) &&
                    GetTargetHPPercent() > hpThresholdUsageST)
                {
                    //Use whenever
                    if (SerpentOffering >= 50 && TargetIsBoss() &&
                        GetTargetHPPercent() < hpThresholdDontSaveST)
                        return true;

                    //2min burst
                    if (!JustUsed(SerpentsIre, 2.2f) && HasStatusEffect(Buffs.ReadyToReawaken) ||
                        WasLastWeaponskill(Ouroboros) && SerpentOffering >= 50 && IreCD >= 50)
                        return true;

                    //1min
                    if (SerpentOffering is >= 50 and <= 80 &&
                        IreCD is >= 50 and <= 62)
                        return true;

                    //overcap protection
                    if (SerpentOffering >= 100)
                        return true;

                    //non-boss encounters
                    if (!InBossEncounter() && SerpentOffering >= 50)
                        return true;

                    //Lower lvl
                    if (SerpentOffering >= 50 &&
                        WasLastWeaponskill(FourthGeneration) && !LevelChecked(Ouroboros))
                        return true;
                }
                break;
            }

            case true when ActionReady(Reawaken) && GetTargetHPPercent() > hpThresholdUsageAoE &&
                           (HasStatusEffect(Buffs.ReadyToReawaken) || SerpentOffering >= 50) &&
                           HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                           !HasStatusEffect(Buffs.Reawakened) && NoAoEComboWeaves:
                return true;
        }

        return false;
    }

    private static uint ReawakenCombo(uint actionId)
    {
        #region Pre Ouroboros

        if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
        {
            return AnguineTribute switch
            {
                4 => OriginalHook(SteelFangs),
                3 => OriginalHook(ReavingFangs),
                2 => OriginalHook(HuntersCoil),
                1 => OriginalHook(SwiftskinsCoil),
                var _ => actionId
            };
        }

        #endregion

        #region With Ouroboros

        if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
        {
            return AnguineTribute switch
            {
                5 => OriginalHook(SteelFangs),
                4 => OriginalHook(ReavingFangs),
                3 => OriginalHook(HuntersCoil),
                2 => OriginalHook(SwiftskinsCoil),
                1 => OriginalHook(Reawaken),
                var _ => actionId
            };
        }

        #endregion

        return actionId;
    }

    private static int ComputeHpThresholdReawaken()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? VPR_ST_ReawakenBossOption : VPR_ST_ReawakenBossAddsOption;

        return VPR_ST_ReawakenTrashOption;
    }

   #endregion

    #region Combos

    private static float GCD => GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;

    private static bool IsHoningExpiring(float times)
    {
        float gcd = GCD * times;

        return HasStatusEffect(Buffs.HonedSteel) && GetStatusEffectRemainingTime(Buffs.HonedSteel) < gcd ||
               HasStatusEffect(Buffs.HonedReavers) && GetStatusEffectRemainingTime(Buffs.HonedReavers) < gcd;
    }

    private static bool IsVenomExpiring(float times)
    {
        float gcd = GCD * times;

        return HasStatusEffect(Buffs.FlankstungVenom) && GetStatusEffectRemainingTime(Buffs.FlankstungVenom) < gcd ||
               HasStatusEffect(Buffs.FlanksbaneVenom) && GetStatusEffectRemainingTime(Buffs.FlanksbaneVenom) < gcd ||
               HasStatusEffect(Buffs.HindstungVenom) && GetStatusEffectRemainingTime(Buffs.HindstungVenom) < gcd ||
               HasStatusEffect(Buffs.HindsbaneVenom) && GetStatusEffectRemainingTime(Buffs.HindsbaneVenom) < gcd;
    }

    private static bool IsEmpowermentExpiring(float times)
    {
        float gcd = GCD * times;

        return GetStatusEffectRemainingTime(Buffs.Swiftscaled) < gcd || GetStatusEffectRemainingTime(Buffs.HuntersInstinct) < gcd;
    }

    private static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GCD * times;

        return Instance()->Combo.Timer != 0 && Instance()->Combo.Timer < gcd;
    }

    #endregion

    #region Vicewinder & Uncoied Fury Combo

    private static bool CanUseVicewinder =>
        ActionReady(Vicewinder) && InActionRange(Vicewinder) && InCombat() &&
        !IsComboExpiring(4) && !IsVenomExpiring(4) && !IsHoningExpiring(4) &&
        !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
        (IreCD >= GCD * 3 && InBossEncounter() || !InBossEncounter() || !LevelChecked(SerpentsIre));

    private static bool CanUseUncoiledFury(bool isAoE = false)
    {
        int ufHoldChargesST = IsNotEnabled(Preset.VPR_ST_SimpleMode) ? VPR_ST_UncoiledFury_HoldCharges : 1;
        int ufHPThresholdST = IsNotEnabled(Preset.VPR_ST_SimpleMode) ? VPR_ST_UncoiledFury_Threshold : 1;
        int ufHoldChargesAoE = IsNotEnabled(Preset.VPR_AoE_SimpleMode) ? VPR_AoE_UncoiledFury_HoldCharges : 1;
        int ufHPThresholdAoE = IsNotEnabled(Preset.VPR_AoE_SimpleMode) ? VPR_AoE_UncoiledFury_Threshold : 1;


        switch (isAoE)
        {
            case false when !IsComboExpiring(2) && !IsVenomExpiring(2) && !IsHoningExpiring(2) &&
                            ActionReady(UncoiledFury) && HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                            (RattlingCoilStacks > ufHoldChargesST || GetTargetHPPercent() < ufHPThresholdST && HasRattlingCoilStacks) &&
                            !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady && NoSTComboWeaves &&
                            !HasStatusEffect(Buffs.Reawakened) && !HasStatusEffect(Buffs.ReadyToReawaken) &&
                            !WasLastWeaponskill(Ouroboros) && !IsEmpowermentExpiring(3):

            case true when ActionReady(UncoiledFury) &&
                           (RattlingCoilStacks > ufHoldChargesAoE ||
                            GetTargetHPPercent() < ufHPThresholdAoE && HasRattlingCoilStacks) &&
                           HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                           !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                           !HasStatusEffect(Buffs.Reawakened) && NoAoEComboWeaves &&
                           !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) && !WasLastAbility(SerpentsIre):
                return true;

            default:
                return false;
        }
    }

    private static bool CanVicewinderCombo(ref uint actionId)
    {
        if ((VicewinderReady || SwiftskinsCoilReady || HuntersCoilReady) &&
            LevelChecked(Vicewinder) && InActionRange(Vicewinder) &&
            !HasStatusEffect(Buffs.Reawakened))
        {
            // Swiftskin's Coil
            if (VicewinderReady &&
                (!OnTargetsFlank() ||
                 !TargetNeedsPositionals() ||
                 !HasStatusEffect(Buffs.Swiftscaled) ||
                 IsEmpowermentExpiring(6)) ||
                HuntersCoilReady)
            {
                actionId = SwiftskinsCoil;
                return true;
            }

            // Hunter's Coil
            if (VicewinderReady &&
                (!OnTargetsRear() ||
                 !TargetNeedsPositionals() ||
                 !HasStatusEffect(Buffs.HuntersInstinct) ||
                 IsEmpowermentExpiring(6)) ||
                SwiftskinsCoilReady)
            {
                actionId = HuntersCoil;
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked)
            return StandardOpener;

        return WrathOpener.Dummy;
    }

    internal static VPRStandardOpener StandardOpener = new();

    internal class VPRStandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            ReavingFangs,
            SerpentsIre,
            SwiftskinsSting,
            Vicewinder,
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite,
            Reawaken,
            FirstGeneration,
            FirstLegacy,
            SecondGeneration,
            SecondLegacy,
            ThirdGeneration,
            ThirdLegacy,
            FourthGeneration,
            FourthLegacy,
            Ouroboros,
            UncoiledFury, //21
            UncoiledTwinfang, //22
            UncoiledTwinblood, //23
            UncoiledFury, //24
            UncoiledTwinfang, //25
            UncoiledTwinblood, //26
            HindstingStrike,
            DeathRattle,
            Vicewinder,
            UncoiledFury, //30
            UncoiledTwinfang, //31
            UncoiledTwinblood, //32
            HuntersCoil, //33
            TwinfangBite, //34
            TwinbloodBite, //35
            SwiftskinsCoil, //36
            TwinbloodBite, //37
            TwinfangBite //38
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([33], SwiftskinsCoil, OnTargetsRear),
            ([34], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom)),
            ([35], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([36], HuntersCoil, () => SwiftskinsCoilReady),
            ([37], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([38], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([21, 22, 23, 24, 25, 26, 30, 31, 32], () => VPR_Opener_ExcludeUF)
        ];

        internal override UserData ContentCheckConfig => VPR_Balance_Content;
        public override Preset Preset => Preset.VPR_ST_Opener;
        public override bool HasCooldowns() =>
            IsOriginal(ReavingFangs) &&
            GetRemainingCharges(Vicewinder) is 2 &&
            IsOffCooldown(SerpentsIre);
    }

    #endregion

    #region Gauge

    private static VPRGauge Gauge => GetJobGauge<VPRGauge>();

    private static byte RattlingCoilStacks => Gauge.RattlingCoilStacks;

    private static byte SerpentOffering => Gauge.SerpentOffering;

    private static byte AnguineTribute => Gauge.AnguineTribute;

    private static DreadCombo DreadCombo => Gauge.DreadCombo;

    private static bool VicewinderReady => DreadCombo is DreadCombo.Dreadwinder;

    private static bool HuntersCoilReady => DreadCombo is DreadCombo.HuntersCoil;

    private static bool SwiftskinsCoilReady => DreadCombo is DreadCombo.SwiftskinsCoil;

    private static bool VicepitReady => DreadCombo is DreadCombo.PitOfDread;

    private static bool SwiftskinsDenReady => DreadCombo is DreadCombo.SwiftskinsDen;

    private static bool HuntersDenReady => DreadCombo is DreadCombo.HuntersDen;

    private static SerpentCombo SerpentCombo => Gauge.SerpentCombo;

    private static bool Legacyweaves =>
        HasStatusEffect(Buffs.Reawakened) &&
        (SerpentCombo.HasFlag(SerpentCombo.FirstLegacy) ||
         SerpentCombo.HasFlag(SerpentCombo.SecondLegacy) ||
         SerpentCombo.HasFlag(SerpentCombo.ThirdLegacy) ||
         SerpentCombo.HasFlag(SerpentCombo.FourthLegacy));

    private static bool DeathRattleWeave => Gauge.SerpentCombo is SerpentCombo.DeathRattle;

    private static bool LastLashWeave => Gauge.SerpentCombo is SerpentCombo.LastLash;

    #endregion

    #region ID's

    public const uint
        ReavingFangs = 34607,
        ReavingMaw = 34615,
        Vicewinder = 34620,
        HuntersCoil = 34621,
        HuntersDen = 34624,
        HuntersSnap = 39166,
        Vicepit = 34623,
        RattlingCoil = 39189,
        Reawaken = 34626,
        SerpentsIre = 34647,
        SerpentsTail = 35920,
        Slither = 34646,
        SteelFangs = 34606,
        SteelMaw = 34614,
        SwiftskinsCoil = 34622,
        SwiftskinsDen = 34625,
        Twinblood = 35922,
        Twinfang = 35921,
        UncoiledFury = 34633,
        WrithingSnap = 34632,
        SwiftskinsSting = 34609,
        TwinfangBite = 34636,
        TwinbloodBite = 34637,
        UncoiledTwinfang = 34644,
        UncoiledTwinblood = 34645,
        HindstingStrike = 34612,
        DeathRattle = 34634,
        HuntersSting = 34608,
        HindsbaneFang = 34613,
        FlankstingStrike = 34610,
        FlanksbaneFang = 34611,
        HuntersBite = 34616,
        JaggedMaw = 34618,
        SwiftskinsBite = 34617,
        BloodiedMaw = 34619,
        FirstGeneration = 34627,
        FirstLegacy = 34640,
        SecondGeneration = 34628,
        SecondLegacy = 34641,
        ThirdGeneration = 34629,
        ThirdLegacy = 34642,
        FourthGeneration = 34630,
        FourthLegacy = 34643,
        Ouroboros = 34631,
        LastLash = 34635,
        TwinfangThresh = 34638,
        TwinbloodThresh = 34639;

    public static class Buffs
    {
        public const ushort
            FellhuntersVenom = 3659,
            FellskinsVenom = 3660,
            FlanksbaneVenom = 3646,
            FlankstungVenom = 3645,
            HindstungVenom = 3647,
            HindsbaneVenom = 3648,
            GrimhuntersVenom = 3649,
            GrimskinsVenom = 3650,
            HuntersVenom = 3657,
            SwiftskinsVenom = 3658,
            HuntersInstinct = 3668,
            Swiftscaled = 3669,
            Reawakened = 3670,
            ReadyToReawaken = 3671,
            PoisedForTwinfang = 3665,
            PoisedForTwinblood = 3666,
            HonedReavers = 3772,
            HonedSteel = 3672;
    }

    public static class Debuffs
    {
    }

    public static class Traits
    {
        public const uint
            EnhancedVipersRattle = 530,
            EnhancedSerpentsLineage = 533,
            SerpentsLegacy = 534;
    }

    #endregion
}
