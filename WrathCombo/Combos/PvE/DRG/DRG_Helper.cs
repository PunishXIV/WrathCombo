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
    #region Misc

    private static IStatus? ChaosDebuff =>
        GetStatusEffect(ChaoticList[OriginalHook(ChaosThrust)], CurrentTarget);

    #endregion
    #region Basic Combo

    private static uint DoBasicCombo(
        bool useTrueNorth = false,
        bool onAoE = false,
        bool includeDisembowel = false,
        int trueNorthCharges = 0)
    {
        if (onAoE)
        {
            if (ComboTimer > 0)
            {
                if (includeDisembowel && !LevelChecked(SonicThrust))
                {
                    if (ComboAction == TrueThrust && LevelChecked(Disembowel))
                        return Disembowel;

                    if (ComboAction == Disembowel && LevelChecked(ChaosThrust))
                        return OriginalHook(ChaosThrust);
                }
                else
                {
                    if (ComboAction is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                        return SonicThrust;

                    if (ComboAction == SonicThrust && LevelChecked(CoerthanTorment))
                        return CoerthanTorment;
                }
            }

            if (includeDisembowel && !HasStatusEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust))
                return OriginalHook(TrueThrust);

            return OriginalHook(DoomSpike);
        }

        if (ComboTimer > 0)
        {
            if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                return LevelChecked(Disembowel) &&
                       (LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                        CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)]) ||
                        GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                    ? OriginalHook(Disembowel)
                    : OriginalHook(VorpalThrust);

            if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                return useTrueNorth &&
                       GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
                       Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                    ? Role.TrueNorth
                    : OriginalHook(ChaosThrust);

            if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                return useTrueNorth &&
                       GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
                       Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                    ? Role.TrueNorth
                    : WheelingThrust;

            if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                return OriginalHook(FullThrust);

            if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                return useTrueNorth &&
                       GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
                       Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsFlank()
                    ? Role.TrueNorth
                    : FangAndClaw;

            if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                return Drakesbane;
        }

        return OriginalHook(TrueThrust);
    }

    #endregion

    #region Lifesurge

    private static bool CanLifeSurge(bool onAoE = false)
    {
        if (!ActionReady(LifeSurge) || HasStatusEffect(Buffs.LifeSurge))
            return false;

        if (onAoE)
        {
            if (!InActionRange(DoomSpike))
                return false;

            if (LevelChecked(CoerthanTorment))
            {
                if (!JustUsed(SonicThrust))
                    return false;

                return HasStatusEffect(Buffs.LanceCharge) ||
                       HasStatusEffect(Buffs.BattleLitany) ||
                       LoTDActive;
            }

            if (LevelChecked(SonicThrust) && JustUsed(DoomSpike))
                return true;

            return JustUsed(DoomSpike);
        }


        if (!InActionRange(TrueThrust))
            return false;

        if (LevelChecked(Drakesbane) && LoTDActive &&
            (HasStatusEffect(Buffs.LanceCharge) || HasStatusEffect(Buffs.BattleLitany)) &&
            (JustUsed(WheelingThrust) ||
             JustUsed(FangAndClaw) ||
             LevelChecked(LanceBarrage) && JustUsed(LanceBarrage) ||
             LevelChecked(HeavensThrust) && JustUsed(OriginalHook(FullThrust)) ||
             !LevelChecked(LanceBarrage) && JustUsed(OriginalHook(VorpalThrust)) && LevelChecked(HeavensThrust)))
            return true;

        if (!LevelChecked(Drakesbane) && JustUsed(VorpalThrust))
            return true;

        if (!LevelChecked(FullThrust) && JustUsed(TrueThrust))
            return true;

        return false;
    }

    #endregion

    #region Animation Locks

    private static bool CanDRGWeave(float weaveTime = BaseAnimationLock, bool forceFirst = false) =>
        !HasWeavedAction(Stardiver) && (!forceFirst || !HasWeaved()) && CanWeave(weaveTime);

    private static bool CanWeaveOgcds() =>
        HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel);

    private const int HoldOnlyWhenStationary = 0;
    private const int HoldOnlyInMeleeRange = 1;

    private static bool CanUseWithHoldOptions(UserBoolArray? movingOrInRangedOptions)
    {
        if (movingOrInRangedOptions is null || movingOrInRangedOptions.Count == 0)
            return true;

        if (movingOrInRangedOptions[HoldOnlyWhenStationary] && IsMoving())
            return false;

        if (movingOrInRangedOptions.Count > HoldOnlyInMeleeRange &&
            movingOrInRangedOptions[HoldOnlyInMeleeRange] && !InMeleeRange())
            return false;

        return true;
    }

    #endregion

    #region Burst skills

    private static bool CanBattleLitany(int hpThreshold = 0) =>
        ActionReady(BattleLitany) && GetTargetHPPercent() > hpThreshold;

    private static bool CanLanceCharge(int hpThreshold = 0) =>
        ActionReady(LanceCharge) && HasBattleTarget() && GetTargetHPPercent() > hpThreshold &&
        (IsOnCooldown(BattleLitany) || !LevelChecked(BattleLitany));

    private static bool CanUseWyrmwind() =>
        ActionReady(WyrmwindThrust) &&
        FirstmindsFocus is 2 &&
        InActionRange(WyrmwindThrust) &&
        (LoTDActive ||
         HasStatusEffect(Buffs.DraconianFire) ||
         HasStatusEffect(Buffs.RaidenThrustReady) ||
         NumberOfEnemiesInRange(WyrmwindThrust, CurrentTarget) >= 2);

    private static bool CanMirageDive(bool onAoE = false, bool ignoreDoubleMirageHold = false)
    {
        if (!ActionReady(MirageDive) || !HasStatusEffect(Buffs.DiveReady) ||
            OriginalHook(Jump) is not MirageDive || !InActionRange(MirageDive))
            return false;

        if (onAoE || ignoreDoubleMirageHold || LoTDActive)
            return true;

        bool diveExpiring = GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                            GetCooldownRemainingTime(Geirskogul) > 3;

        return diveExpiring || !DRG_ST_DoubleMirage;
    }

    private static bool CanUseGeirskogul(int hpThreshold = 0) =>
        ActionReady(Geirskogul) &&
        InActionRange(Geirskogul) &&
        HasBattleTarget() &&
        !LoTDTimerActive &&
        GetTargetHPPercent() > hpThreshold;

    private static int GeirskogulHPThreshold() =>
        InBossEncounter() ? TargetIsBoss()
                ? DRG_ST_GeirskogulBossHPOption
                : DRG_ST_GeirskogulBossAddsHPOption
            : DRG_ST_GeirskogulTrashHPOption;

    private static bool CanStarcross() =>
        ActionReady(Starcross) && HasStatusEffect(Buffs.StarcrossReady) && InActionRange(Starcross);

    private static bool CanRiseOfTheDragon() =>
        ActionReady(RiseOfTheDragon) && HasStatusEffect(Buffs.DragonsFlight) && InActionRange(RiseOfTheDragon);

    private static bool CanNastrond() =>
        ActionReady(Nastrond) && HasStatusEffect(Buffs.NastrondReady) && LoTDActive && InActionRange(Nastrond);

    private static bool CanHighJump(
        bool onAoE = false,
        UserBoolArray? holdOptions = null,
        bool allowDoubleMirageHold = true) =>
        ActionReady(OriginalHook(Jump)) && CanUseWithHoldOptions(holdOptions) &&
        (onAoE
            ? IsOriginal(Jump) || IsOriginal(HighJump)
            : !LevelChecked(HighJump) && IsOriginal(Jump) ||
              LevelChecked(HighJump) && IsOriginal(HighJump) &&
              (allowDoubleMirageHold || !DRG_ST_DoubleMirage ||
               DRG_ST_DoubleMirage && (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDTimerActive)));

    private static bool CanDragonfireDive(
        UserBoolArray? holdOptions = null,
        int hpThreshold = 0) =>
        ActionReady(DragonfireDive) && !HasStatusEffect(Buffs.DragonsFlight) &&
        GetTargetHPPercent() > hpThreshold &&
        CanUseWithHoldOptions(holdOptions) &&
        (LoTDTimerActive || !LevelChecked(Geirskogul));

    private static bool CanStardiver(UserBoolArray? holdOptions = null) =>
        ActionReady(Stardiver) && LoTDActive && !HasStatusEffect(Buffs.StarcrossReady) &&
        CanUseWithHoldOptions(holdOptions);

    private readonly struct OutsideOfMeleeOptions
    {
        public bool OnAoE { get; init; }
        public int GeirskogulHpThreshold { get; init; }
        public bool UseDamage { get; init; }
        public bool UseMirage { get; init; }
        public bool UseWyrmwind { get; init; }
        public bool UseStarcross { get; init; }
        public bool UseRiseOfTheDragon { get; init; }
        public bool UseGeirskogul { get; init; }
        public bool UseNastrond { get; init; }
        public bool UseRangedUptime { get; init; }
        public bool UseTrueNorth { get; init; }
        public int TrueNorthCharges { get; init; }
        public bool IncludeDisembowel { get; init; }
        public bool IgnoreDoubleMirageHold { get; init; }

        public static OutsideOfMeleeOptions SimpleSt => new()
        {
            UseDamage = true,
            UseMirage = true,
            UseWyrmwind = true,
            UseStarcross = true,
            UseRiseOfTheDragon = true,
            UseGeirskogul = true,
            UseNastrond = true,
            UseRangedUptime = true,
            UseTrueNorth = true,
            IgnoreDoubleMirageHold = true
        };

        public static OutsideOfMeleeOptions SimpleAoE => new()
        {
            OnAoE = true,
            UseDamage = true,
            UseMirage = true,
            UseWyrmwind = true,
            UseStarcross = true,
            UseRiseOfTheDragon = true,
            UseGeirskogul = true,
            UseNastrond = true,
            UseRangedUptime = true,
            IncludeDisembowel = true,
            IgnoreDoubleMirageHold = true
        };
    }

    private static uint OutsideOfMelee(uint actionId, in OutsideOfMeleeOptions options)
    {
        if (!options.UseDamage)
            return actionId;

        if (options.OnAoE)
        {
            if (options.UseMirage &&
                CanMirageDive(true, options.IgnoreDoubleMirageHold) && InCombat())
                return MirageDive;

            if (options.UseWyrmwind &&
                CanUseWyrmwind() && InCombat())
                return WyrmwindThrust;

            if (options.UseStarcross &&
                CanStarcross() && InCombat())
                return Starcross;

            if (options.UseRiseOfTheDragon &&
                CanRiseOfTheDragon() && InCombat())
                return RiseOfTheDragon;

            if (options.UseGeirskogul &&
                CanUseGeirskogul(options.GeirskogulHpThreshold) && InCombat())
                return Geirskogul;

            if (options.UseNastrond &&
                CanNastrond() && InCombat())
                return Nastrond;

            if (options.UseRangedUptime &&
                ActionReady(PiercingTalon) && !CanDRGWeave())
                return PiercingTalon;

            return DoBasicCombo(onAoE: true, includeDisembowel: options.IncludeDisembowel);
        }

        if (options.UseMirage &&
            CanMirageDive(ignoreDoubleMirageHold: options.IgnoreDoubleMirageHold) && InCombat())
            return MirageDive;

        if (options.UseWyrmwind &&
            CanUseWyrmwind() && InCombat())
            return WyrmwindThrust;

        if (options.UseStarcross &&
            CanStarcross() && InCombat())
            return Starcross;

        if (options.UseRiseOfTheDragon &&
            CanRiseOfTheDragon() && InCombat())
            return RiseOfTheDragon;

        if (options.UseGeirskogul &&
            CanUseGeirskogul(hpThreshold: options.GeirskogulHpThreshold) && InCombat())
            return Geirskogul;

        if (options.UseNastrond &&
            CanNastrond() && InCombat())
            return Nastrond;

        if (options.UseRangedUptime &&
            ActionReady(PiercingTalon))
            return PiercingTalon;

        return DoBasicCombo(options.UseTrueNorth, trueNorthCharges: options.TrueNorthCharges);
    }

    #endregion

    #region HP Thresholds

    private static int BossHpThreshold(int hpBossOption, int hpOption, bool isBoss) =>
        hpBossOption == 1 || !isBoss ? hpOption : 0;

    private static int BattleLitanyHPThreshold =>
        BossHpThreshold(DRG_ST_BattleLitanyHPBossOption, DRG_ST_BattleLitanyHPOption, InBossEncounter());

    private static int LanceChargeHPThreshold =>
        BossHpThreshold(DRG_ST_LanceChargeHPBossOption, DRG_ST_LanceChargeHPOption, InBossEncounter());

    private static int DragonfireDiveHPThreshold =>
        BossHpThreshold(DRG_ST_DragonfireDiveHPBossOption, DRG_ST_DragonfireDiveHPOption, InBossEncounter());

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

        public override Preset Preset => Preset.DRG_ST_Opener;

        internal override UserData ContentCheckConfig => DRG_BalanceContent;

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

        public override Preset Preset => Preset.DRG_ST_Opener;
        internal override UserData ContentCheckConfig => DRG_BalanceContent;

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

    private static short LoTDTimer => Gauge.LOTDTimer;

    private static byte FirstmindsFocus => Gauge.FirstmindsFocusCount;

    private static bool LoTDTimerActive => LoTDTimer > 0;

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
            LifeOfTheDragon = 3177, // Do not use, for translation only
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
