#region

using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.CustomComboNS;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class VPR
{
    // VPR Gauge & Extensions
    internal static VPROpenerMaxLevel1 Opener1 = new();

    internal static VPRGauge gauge = GetJobGauge<VPRGauge>();

    internal static float GCD => GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;

    internal static float ireCD => GetCooldownRemainingTime(SerpentsIre);

    internal static bool trueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static bool VicewinderReady => gauge.DreadCombo == DreadCombo.Dreadwinder;

    internal static bool HuntersCoilReady => gauge.DreadCombo == DreadCombo.HuntersCoil;

    internal static bool SwiftskinsCoilReady => gauge.DreadCombo == DreadCombo.SwiftskinsCoil;

    internal static bool VicepitReady => gauge.DreadCombo == DreadCombo.PitOfDread;

    internal static bool SwiftskinsDenReady => gauge.DreadCombo == DreadCombo.SwiftskinsDen;

    internal static bool HuntersDenReady => gauge.DreadCombo == DreadCombo.HuntersDen;

    internal static bool CappedOnCoils =>
        (TraitLevelChecked(Traits.EnhancedVipersRattle) && gauge.RattlingCoilStacks > 2) ||
        (!TraitLevelChecked(Traits.EnhancedVipersRattle) && gauge.RattlingCoilStacks > 1);

    internal static bool HasRattlingCoilStack(VPRGauge Gauge) => gauge.RattlingCoilStacks > 0;

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
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite
        ];

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Vicewinder) < 2)
                return false;

            if (!ActionReady(SerpentsIre))
                return false;

            return true;
        }
    }

    internal static class VPRHelper
    {
        internal static bool UseReawaken(VPRGauge gauge)
        {
            float ireCD = GetCooldownRemainingTime(SerpentsIre);

            if (LevelChecked(Reawaken) && !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang) &&
                !IsEmpowermentExpiring(6))
                if ((!JustUsed(SerpentsIre, 2.2f) && HasEffect(Buffs.ReadyToReawaken)) || //2min burst
                    (WasLastWeaponskill(Ouroboros) && gauge.SerpentOffering >= 50 && ireCD >= 50) || //2nd RA
                    (gauge.SerpentOffering is >= 50 and <= 80 && ireCD is >= 50 and <= 62) || //1min
                    gauge.SerpentOffering >= 100 || //overcap
                    (gauge.SerpentOffering >= 50 && WasLastWeaponskill(FourthGeneration) &&
                     !LevelChecked(Ouroboros))) //<100
                    return true;

            return false;
        }

        internal static bool IsHoningExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return (HasEffect(Buffs.HonedSteel) && GetBuffRemainingTime(Buffs.HonedSteel) < GCD) ||
                   (HasEffect(Buffs.HonedReavers) && GetBuffRemainingTime(Buffs.HonedReavers) < GCD);
        }

        internal static bool IsVenomExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return (HasEffect(Buffs.FlankstungVenom) && GetBuffRemainingTime(Buffs.FlankstungVenom) < GCD) ||
                   (HasEffect(Buffs.FlanksbaneVenom) && GetBuffRemainingTime(Buffs.FlanksbaneVenom) < GCD) ||
                   (HasEffect(Buffs.HindstungVenom) && GetBuffRemainingTime(Buffs.HindstungVenom) < GCD) ||
                   (HasEffect(Buffs.HindsbaneVenom) && GetBuffRemainingTime(Buffs.HindsbaneVenom) < GCD);
        }

        internal static bool IsEmpowermentExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return GetBuffRemainingTime(Buffs.Swiftscaled) < GCD || GetBuffRemainingTime(Buffs.HuntersInstinct) < GCD;
        }

        internal static unsafe bool IsComboExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < GCD;
        }
    }
}