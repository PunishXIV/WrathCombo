using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.DRG.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Data.ActionWatching;
namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal static StandardOpenerLogic StandardOpener = new();
    internal static PiercingTalonOpenerLogic PiercingTalonOpener = new();

    internal static bool UseLifeSurge()
    {
        if (ActionReady(LifeSurge) && CanDRGWeave(LifeSurge) && !HasStatusEffect(Buffs.LifeSurge))
        {
            if (LevelChecked(Drakesbane) && LoTDActive &&
                (HasStatusEffect(Buffs.LanceCharge) || HasStatusEffect(Buffs.BattleLitany)) &&
                (JustUsed(WheelingThrust) ||
                 JustUsed(FangAndClaw) ||
                 JustUsed(OriginalHook(VorpalThrust)) && LevelChecked(HeavensThrust)))
                return true;

            if (!LevelChecked(Drakesbane) && JustUsed(VorpalThrust))
                return true;

            if (!LevelChecked(FullThrust) && JustUsed(TrueThrust))
                return true;
        }

        return false;
    }

    #region Animation Locks

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
        Variant.Rampart,
        Role.TrueNorth
    ];

    internal static readonly List<uint> MidLocks =
    [
        Jump,
        HighJump,
        DragonfireDive
    ];

    internal static Status? ChaosDoTDebuff => LevelChecked(ChaoticSpring)
        ? GetStatusEffect(Debuffs.ChaoticSpring, CurrentTarget)
        : GetStatusEffect(Debuffs.ChaosThrust, CurrentTarget);

    internal static uint SlowLock => Stardiver;

    internal static bool CanDRGWeave(uint oGCD)
    {
        float gcdTimer = GetCooldownRemainingTime(TrueThrust);

        //GCD Ready - No Weave
        if (IsOffCooldown(TrueThrust))
            return false;

        if (FastLocks.Any(x => x == oGCD) && gcdTimer >= 0.6f && !HasDoubleWeaved())
            return true;

        if (MidLocks.Any(x => x == oGCD) && gcdTimer >= 0.8f && !HasDoubleWeaved())
            return true;

        if (SlowLock == oGCD && gcdTimer >= 1.5f && !HasDoubleWeaved())
            return true;

        return false;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked && DRG_SelectedOpener == 0)
            return StandardOpener;

        if (PiercingTalonOpener.LevelChecked && DRG_SelectedOpener == 1)
            return PiercingTalonOpener;

        return WrathOpener.Dummy;
    }

    internal class StandardOpenerLogic : WrathOpener
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

        internal override UserData ContentCheckConfig => DRG_Balance_Content;

        public override bool HasCooldowns() =>
            GetRemainingCharges(LifeSurge) is 2 &&
            IsOffCooldown(BattleLitany) &&
            IsOffCooldown(DragonfireDive) &&
            IsOffCooldown(LanceCharge);
    }

    internal class PiercingTalonOpenerLogic : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            PiercingTalon,
            TrueThrust,
            SpiralBlow,
            LanceCharge,
            BattleLitany,
            ChaoticSpring,
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

        internal override UserData ContentCheckConfig => DRG_Balance_Content;

        public override bool HasCooldowns() =>
            GetRemainingCharges(LifeSurge) is 2 &&
            IsOffCooldown(BattleLitany) &&
            IsOffCooldown(DragonfireDive) &&
            IsOffCooldown(LanceCharge);
    }

    #endregion

    #region Gauge

    internal static DRGGauge Gauge = GetJobGauge<DRGGauge>();

    internal static bool LoTDActive => Gauge.IsLOTDActive;

    internal static byte FirstmindsFocus => Gauge.FirstmindsFocusCount;

    #endregion

    #region ID's

    public const byte ClassID = 4;
    public const byte JobID = 22;

    public const uint
        PiercingTalon = 90,
        ElusiveJump = 94,
        LanceCharge = 85,
        BattleLitany = 3557,
        Jump = 92,
        LifeSurge = 83,
        HighJump = 16478,
        MirageDive = 7399,
        BloodOfTheDragon = 3553,
        Stardiver = 16480,
        CoerthanTorment = 16477,
        DoomSpike = 86,
        SonicThrust = 7397,
        ChaosThrust = 88,
        RaidenThrust = 16479,
        TrueThrust = 75,
        Disembowel = 87,
        FangAndClaw = 3554,
        WheelingThrust = 3556,
        FullThrust = 84,
        VorpalThrust = 78,
        WyrmwindThrust = 25773,
        DraconianFury = 25770,
        ChaoticSpring = 25772,
        DragonfireDive = 96,
        Geirskogul = 3555,
        Nastrond = 7400,
        HeavensThrust = 25771,
        Drakesbane = 36952,
        RiseOfTheDragon = 36953,
        LanceBarrage = 36954,
        SpiralBlow = 36955,
        Starcross = 36956;

    public static class Buffs
    {
        public const ushort
            LanceCharge = 1864,
            BattleLitany = 786,
            DiveReady = 1243,
            RaidenThrustReady = 1863,
            PowerSurge = 2720,
            LifeSurge = 116,
            DraconianFire = 1863,
            NastrondReady = 3844,
            StarcrossReady = 3846,
            DragonsFlight = 3845;
    }

    public static class Debuffs
    {
        public const ushort
            ChaosThrust = 118,
            ChaoticSpring = 2719;
    }

    #endregion
}
