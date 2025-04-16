using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    internal static VPRGauge Gauge = GetJobGauge<VPRGauge>();
    internal static VPROpenerMaxLevel1 Opener1 = new();

    internal static float GCD => GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;

    internal static float IreCD => GetCooldownRemainingTime(SerpentsIre);

    internal static bool In5Y => HasBattleTarget() && GetTargetDistance() <= 5;

    internal static bool CappedOnCoils =>
        TraitLevelChecked(Traits.EnhancedVipersRattle) && Gauge.RattlingCoilStacks > 2 ||
        !TraitLevelChecked(Traits.EnhancedVipersRattle) && Gauge.RattlingCoilStacks > 1;

    internal static bool VicewinderReady => Gauge.DreadCombo == DreadCombo.Dreadwinder;

    internal static bool HuntersCoilReady => Gauge.DreadCombo == DreadCombo.HuntersCoil;

    internal static bool SwiftskinsCoilReady => Gauge.DreadCombo == DreadCombo.SwiftskinsCoil;

    internal static bool VicepitReady => Gauge.DreadCombo == DreadCombo.PitOfDread;

    internal static bool SwiftskinsDenReady => Gauge.DreadCombo == DreadCombo.SwiftskinsDen;

    internal static bool HuntersDenReady => Gauge.DreadCombo == DreadCombo.HuntersDen;

    internal static bool HasRattlingCoilStack(VPRGauge gauge) => Gauge.RattlingCoilStacks > 0;

    #region Combos

    internal static bool IsHoningExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return HasStatusEffect(Buffs.HonedSteel) && GetStatusEffectRemainingTime(Buffs.HonedSteel) < gcd ||
               HasStatusEffect(Buffs.HonedReavers) && GetStatusEffectRemainingTime(Buffs.HonedReavers) < gcd;
    }

    internal static bool IsVenomExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return HasStatusEffect(Buffs.FlankstungVenom) && GetStatusEffectRemainingTime(Buffs.FlankstungVenom) < gcd ||
               HasStatusEffect(Buffs.FlanksbaneVenom) && GetStatusEffectRemainingTime(Buffs.FlanksbaneVenom) < gcd ||
               HasStatusEffect(Buffs.HindstungVenom) && GetStatusEffectRemainingTime(Buffs.HindstungVenom) < gcd ||
               HasStatusEffect(Buffs.HindsbaneVenom) && GetStatusEffectRemainingTime(Buffs.HindsbaneVenom) < gcd;
    }

    internal static bool IsEmpowermentExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return GetStatusEffectRemainingTime(Buffs.Swiftscaled) < gcd || GetStatusEffectRemainingTime(Buffs.HuntersInstinct) < gcd;
    }

    internal static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < gcd;
    }

    #endregion

    #region Awaken

    internal static bool UseReawaken(VPRGauge gauge)
    {
        if (LevelChecked(Reawaken) && !HasStatusEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
            !HasStatusEffect(Buffs.HuntersVenom) && !HasStatusEffect(Buffs.SwiftskinsVenom) &&
            !HasStatusEffect(Buffs.PoisedForTwinblood) && !HasStatusEffect(Buffs.PoisedForTwinfang) &&
            !IsEmpowermentExpiring(6))
        {
            //2min burst
            if (!JustUsed(SerpentsIre, 2.2f) && HasStatusEffect(Buffs.ReadyToReawaken) ||
                WasLastWeaponskill(Ouroboros) && Gauge.SerpentOffering >= 50 && IreCD >= 50)
                return true;

            //1min
            if (Gauge.SerpentOffering is >= 50 and <= 80 && IreCD is >= 50 and <= 62)
                return true;

            //overcap protection
            if (Gauge.SerpentOffering >= 100)
                return true;

            //non boss encounters
            if ((IsEnabled(CustomComboPreset.VPR_ST_SimpleMode) && !InBossEncounter() ||
                 IsEnabled(CustomComboPreset.VPR_ST_AdvancedMode) && Config.VPR_ST_SerpentsIre_SubOption == 1 && !InBossEncounter()) &&
                gauge.SerpentOffering >= 50)
                return true;

            //Lower lvl
            if (Gauge.SerpentOffering >= 50 &&
                WasLastWeaponskill(FourthGeneration) && !LevelChecked(Ouroboros))
                return true;
        }

        return false;
    }

    internal static bool ReawakenComboST(ref uint actionID)
    {
        if (HasStatusEffect(Buffs.Reawakened))
        {
                #region Pre Ouroboros

            if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                switch (Gauge.AnguineTribute)
                {
                    case 4:
                        actionID = OriginalHook(SteelFangs);
                        return true;

                    case 3:
                        actionID = OriginalHook(ReavingFangs);
                        return true;

                    case 2:
                        actionID = OriginalHook(HuntersCoil);
                        return true;

                    case 1:
                        actionID = OriginalHook(SwiftskinsCoil);
                        return true;
    }

                #endregion

                #region With Ouroboros

            if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                switch (Gauge.AnguineTribute)
    {
                    case 5:
                        actionID = OriginalHook(SteelFangs);
                        return true;

                    case 4:
                        actionID = OriginalHook(ReavingFangs);
                        return true;

                    case 3:
                        actionID = OriginalHook(HuntersCoil);
                        return true;

                    case 2:
                        actionID = OriginalHook(SwiftskinsCoil);
                        return true;

                    case 1:
                        actionID = OriginalHook(Reawaken);
                        return true;
    }

                #endregion
        }

        return false;
    }

    internal static bool ReawakenComboAoE(ref uint actionID)
    {
        if (HasStatusEffect(Buffs.Reawakened))
        {
                #region Pre Ouroboros

            if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                switch (Gauge.AnguineTribute)
                {
                    case 4:
                        actionID = OriginalHook(SteelMaw);
                        return true;

                    case 3:
                        actionID = OriginalHook(ReavingMaw);
                        return true;

                    case 2:
                        actionID = OriginalHook(HuntersDen);
                        return true;

                    case 1:
                        actionID = OriginalHook(SwiftskinsDen);
                        return true;
    }

                #endregion

                #region With Ouroboros

            if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                switch (Gauge.AnguineTribute)
    {
                    case 5:
                        actionID = OriginalHook(SteelMaw);
                        return true;

                    case 4:
                        actionID = OriginalHook(ReavingMaw);
                        return true;

                    case 3:
                        actionID = OriginalHook(HuntersDen);
                        return true;

                    case 2:
                        actionID = OriginalHook(SwiftskinsDen);
                        return true;

                    case 1:
                        actionID = OriginalHook(Reawaken);
                        return true;
    }

                #endregion
        }
        return false;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class VPROpenerMaxLevel1 : WrathOpener
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
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            HindstingStrike,
            DeathRattle,
            Vicewinder,
            UncoiledFury,
            UncoiledTwinfang,
            UncoiledTwinblood,
            HuntersCoil, //33
            TwinfangBite, //34
            TwinbloodBite, //35
            SwiftskinsCoil, //36
            TwinbloodBite, //37
            TwinfangBite //38
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([33], SwiftskinsCoil, () => OnTargetsRear()),
            ([34], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom)),
            ([35], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([36], HuntersCoil, () => SwiftskinsCoilReady),
            ([37], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([38], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom))
        ];

        internal override UserData ContentCheckConfig => Config.VPR_Balance_Content;

        public override bool HasCooldowns() =>
            IsOriginal(ReavingFangs) &&
            GetRemainingCharges(Vicewinder) is 2 &&
            IsOffCooldown(SerpentsIre);
    }

    #endregion

    #region ID's

    public const byte JobID = 41;

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
        LastLash = 34635;

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
