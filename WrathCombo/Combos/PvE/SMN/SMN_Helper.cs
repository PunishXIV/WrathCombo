using ECommons.DalamudServices;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    internal static SMNOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked) return Opener1;

        return WrathOpener.Dummy;
    }


    internal class SMNOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Ruin3,
            SummonSolarBahamut,
            UmbralImpulse,
            SearingLight,
            UmbralImpulse,
            UmbralImpulse,
            EnergyDrain,
            UmbralImpulse,
            EnkindleSolarBahamut,
            Necrotize,
            UmbralImpulse,
            SearingFlash,
            SummonTitan2,
            TopazRite,
            MountainBuster,
            TopazRite,
            MountainBuster,
            TopazRite,
            MountainBuster,
            TopazRite,
            MountainBuster,
            SummonGaruda2,
            All.Swiftcast,
            Slipstream,

        ];

        public override List<int> DelayedWeaveSteps { get; set; } = 
        [
            4,
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData? ContentCheckConfig => Config.SMN_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!HasPetPresent())
                return false;

            if (!IsOffCooldown(SummonSolarBahamut) ||
                !IsOffCooldown(SearingFlash) ||
                !IsOffCooldown(SearingLight) ||
                !IsOffCooldown(All.Swiftcast) ||
                !IsOffCooldown(EnergyDrain))
                return false;

            return true;
        }
    }

}
