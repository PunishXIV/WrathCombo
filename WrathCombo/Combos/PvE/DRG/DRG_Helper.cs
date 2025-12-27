using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Frozen;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.DRG.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    #region Lifesurge

    private static bool CanLifeSurge()
    {
        if (ActionReady(LifeSurge) && !HasStatusEffect(Buffs.LifeSurge))
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

    #endregion

    #region Animation Locks

    private static bool CanDRGWeave(float weaveTime = BaseAnimationLock, bool forceFirst = false) =>
        !HasWeavedAction(Stardiver) && (!forceFirst || !HasWeaved()) && CanWeave(weaveTime);

    #endregion

    #region Misc

    private static IStatus? ChaosDebuff =>
        GetStatusEffect(ChaoticList[OriginalHook(ChaosThrust)], CurrentTarget);

    private static int HPThresholdBuffs =>
        DRG_ST_BuffsBossOption == 1 ||
        !InBossEncounter() ? DRG_ST_BuffsHPOption : 0;

  #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked &&
            DRG_SelectedOpener == 0)
            return StandardOpener;

        if (PiercingTalonOpener.LevelChecked &&
            DRG_SelectedOpener == 1)
            return PiercingTalonOpener;

        return WrathOpener.Dummy;
    }

    internal static DRGStandardOpener StandardOpener = new();
    internal static DRGPiercingTalonOpener PiercingTalonOpener = new();

    internal class DRGStandardOpener : WrathOpener
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

    internal class DRGPiercingTalonOpener : WrathOpener
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

    private static DRGGauge Gauge => GetJobGauge<DRGGauge>();

    private static bool LoTDActive => Gauge.IsLOTDActive;

    private static byte FirstmindsFocus => Gauge.FirstmindsFocusCount;

    private static readonly FrozenDictionary<uint, ushort> ChaoticList = new Dictionary<uint, ushort>
    {
        { ChaosThrust, Debuffs.ChaosThrust },
        { ChaoticSpring, Debuffs.ChaoticSpring }
    }.ToFrozenDictionary();

    #endregion

    #region ID's

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

    public static class Traits
    {
        public const ushort
            LifeOfTheDragon = 163;
    }

    #endregion
}
