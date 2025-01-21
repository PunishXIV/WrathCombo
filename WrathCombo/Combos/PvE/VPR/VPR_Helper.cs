using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal static partial class VPR
{
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

    internal static VPROpenerMaxLevel1 Opener1 = new();
    internal static VPRGauge gauge = GetJobGauge<VPRGauge>();

    internal static float GCD => GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;

    internal static float IreCD => GetCooldownRemainingTime(SerpentsIre);

    internal static bool In5y => HasBattleTarget() && GetTargetDistance() <= 5;

    internal static bool TrueNorthReady => TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);

    internal static bool CappedOnCoils =>
        (TraitLevelChecked(Traits.EnhancedVipersRattle) && gauge.RattlingCoilStacks > 2) ||
        (!TraitLevelChecked(Traits.EnhancedVipersRattle) && gauge.RattlingCoilStacks > 1);

    internal static bool HasRattlingCoilStack(VPRGauge Gauge) => gauge.RattlingCoilStacks > 0;

    internal static bool VicewinderReady => gauge.DreadCombo == DreadCombo.Dreadwinder;

    internal static bool HuntersCoilReady => gauge.DreadCombo == DreadCombo.HuntersCoil;

    internal static bool SwiftskinsCoilReady => gauge.DreadCombo == DreadCombo.SwiftskinsCoil;

    internal static bool VicepitReady => gauge.DreadCombo == DreadCombo.PitOfDread;

    internal static bool SwiftskinsDenReady => gauge.DreadCombo == DreadCombo.SwiftskinsDen;

    internal static bool HuntersDenReady => gauge.DreadCombo == DreadCombo.HuntersDen;

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal static bool UseReawaken(VPRGauge gauge)
    {
        if (LevelChecked(Reawaken) && !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
            !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
            !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang) &&
            !IsEmpowermentExpiring(6))
        {
            //2min burst
            if ((!JustUsed(SerpentsIre, 2.2f) && HasEffect(Buffs.ReadyToReawaken)) ||
               (WasLastWeaponskill(Ouroboros) && gauge.SerpentOffering >= 50 && IreCD >= 50))
                return true;

            //1min
            if (gauge.SerpentOffering is >= 50 and <= 80 && IreCD is >= 50 and <= 62)
                return true;

            //overcap protection
            if (gauge.SerpentOffering >= 100)
                return true;

            //Lower lvl
            if (gauge.SerpentOffering >= 50 &&
                WasLastWeaponskill(FourthGeneration) && !LevelChecked(Ouroboros))
                return true;
        }

        return false;
    }

    internal static bool IsHoningExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return (HasEffect(Buffs.HonedSteel) && GetBuffRemainingTime(Buffs.HonedSteel) < gcd) ||
               (HasEffect(Buffs.HonedReavers) && GetBuffRemainingTime(Buffs.HonedReavers) < gcd);
    }

    internal static bool IsVenomExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return (HasEffect(Buffs.FlankstungVenom) && GetBuffRemainingTime(Buffs.FlankstungVenom) < gcd) ||
               (HasEffect(Buffs.FlanksbaneVenom) && GetBuffRemainingTime(Buffs.FlanksbaneVenom) < gcd) ||
               (HasEffect(Buffs.HindstungVenom) && GetBuffRemainingTime(Buffs.HindstungVenom) < gcd) ||
               (HasEffect(Buffs.HindsbaneVenom) && GetBuffRemainingTime(Buffs.HindsbaneVenom) < gcd);
    }

    internal static bool IsEmpowermentExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return GetBuffRemainingTime(Buffs.Swiftscaled) < gcd || GetBuffRemainingTime(Buffs.HuntersInstinct) < gcd;
    }

    internal static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GetCooldown(SteelFangs).CooldownTotal * times;

        return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < gcd;
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
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite
        ];
        internal override UserData? ContentCheckConfig => Config.VPR_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!IsOriginal(ReavingFangs))
                return false;

            if (GetRemainingCharges(Vicewinder) < 2)
                return false;

            if (!IsOffCooldown(SerpentsIre))
                return false;

            return true;
        }
    }
}