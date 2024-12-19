#region

using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class DRG
{
    // DRG Gauge & Extensions
    internal static DRGOpenerLogic Opener1 = new();

    internal static DRGGauge Gauge = GetJobGauge<DRGGauge>();

    internal static Status? ChaosDoTDebuff =>
        FindTargetEffect(LevelChecked(ChaoticSpring)
            ? Debuffs.ChaoticSpring
            : Debuffs.ChaosThrust);

    internal static bool trueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class DRGOpenerLogic : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            TrueThrust,
            SpiralBlow,
            LanceCharge,
            ChaoticSpring,
            BattleLitany,
            Geirskogul,
            WheelingThrust,
            HighJump,
            LifeSurge,
            Drakesbane,
            DragonfireDive,
            Nastrond,
            RaidenThrust,
            Stardiver,
            LanceBarrage,
            Starcross,
            LifeSurge,
            HeavensThrust,
            RiseOfTheDragon,
            MirageDive,
            FangAndClaw,
            Drakesbane,
            RaidenThrust,
            WyrmwindThrust
        ];

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(LifeSurge) < 2)
                return false;

            if (!ActionReady(BattleLitany))
                return false;

            if (!ActionReady(DragonfireDive))
                return false;

            if (!ActionReady(LanceCharge))
                return false;

            return true;
        }
    }

    internal static class DRGHelper
    {
        internal static readonly List<uint> FastLocks =
        [
            BattleLitany,
            LanceCharge,
            LifeSurge,
            Geirskogul,
            Nastrond,
            MirageDive,
            WyrmwindThrust,
            RiseOfTheDragon,
            Starcross,
            Variant.VariantRampart,
            All.TrueNorth
        ];

        internal static readonly List<uint> MidLocks =
        [
            Jump,
            HighJump,
            DragonfireDive
        ];

        internal static uint SlowLock => Stardiver;

        internal static bool CanDRGWeave(uint oGCD)
        {
            float gcdTimer = GetCooldownRemainingTime(TrueThrust);

            //GCD Ready - No Weave
            if (IsOffCooldown(TrueThrust))
                return false;

            if (FastLocks.Any(x => x == oGCD) && gcdTimer >= 0.6f)
                return true;

            if (MidLocks.Any(x => x == oGCD) && gcdTimer >= 0.8f)
                return true;

            if (SlowLock == oGCD && gcdTimer >= 1.5f)
                return true;

            return false;
        }
    }
}