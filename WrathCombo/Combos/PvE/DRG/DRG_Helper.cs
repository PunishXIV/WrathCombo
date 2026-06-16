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
    #region Basic Combo

    private static uint DoBasicCombo(uint actionId, bool useTrueNorth = false, bool onAoE = false, bool simpleAoE = false)
    {
        int tnCharges = IsNotEnabled(Preset.DRG_ST_SimpleMode) ? DRG_ManualTN : 0;

        if (onAoE)
        {
            if (ComboTimer > 0)
            {
                if ((simpleAoE || IsEnabled(Preset.DRG_AoE_Disembowel)) &&
                    !LevelChecked(SonicThrust))
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

            if ((simpleAoE || IsEnabled(Preset.DRG_AoE_Disembowel)) &&
                !HasStatusEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust))
                return OriginalHook(TrueThrust);

            return actionId;
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
                       GetRemainingCharges(Role.TrueNorth) > tnCharges &&
                       Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                    ? Role.TrueNorth
                    : OriginalHook(ChaosThrust);

            if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                return useTrueNorth &&
                       GetRemainingCharges(Role.TrueNorth) > tnCharges &&
                       Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                    ? Role.TrueNorth
                    : WheelingThrust;

            if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                return OriginalHook(FullThrust);

            if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                return useTrueNorth &&
                       GetRemainingCharges(Role.TrueNorth) > tnCharges &&
                       Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsFlank()
                    ? Role.TrueNorth
                    : FangAndClaw;

            if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                return Drakesbane;
        }

        return actionId;
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

    private static bool CanMirageDive(bool onAoE = false)
    {
        if (!ActionReady(MirageDive) || !HasStatusEffect(Buffs.DiveReady) ||
            OriginalHook(Jump) is not MirageDive || !InActionRange(MirageDive))
            return false;

        if (onAoE || IsEnabled(Preset.DRG_ST_SimpleMode) || LoTDActive)
            return true;

        bool diveExpiring = GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                            GetCooldownRemainingTime(Geirskogul) > 3;

        return diveExpiring || !DRG_ST_DoubleMirage;
    }

    private static int GeirskogulHpThreshold(bool onAoE) =>
        onAoE
            ? IsNotEnabled(Preset.DRG_AoE_SimpleMode) ? DRG_AoE_GeirskogulHPThreshold : 0
            : IsNotEnabled(Preset.DRG_ST_SimpleMode) ? ComputeHpThresholdGeirskogul() : 0;

    private static bool CanUseGeirskogul(bool onAoE = false, int hpThreshold = int.MinValue)
    {
        int threshold = hpThreshold == int.MinValue ? GeirskogulHpThreshold(onAoE) : hpThreshold;

        return ActionReady(Geirskogul) &&
               InActionRange(Geirskogul) &&
               HasBattleTarget() &&
               !LoTDTimerActive &&
               GetTargetHPPercent() > threshold;
    }

    private static int ComputeHpThresholdGeirskogul()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? DRG_ST_GeirskogulBossOption : DRG_ST_GeirskogulBossAddsOption;

        return DRG_ST_GeirskogulTrashOption;
    }

    private static bool CanStarcross() =>
        ActionReady(Starcross) && HasStatusEffect(Buffs.StarcrossReady) && InActionRange(Starcross);

    private static bool CanRiseOfTheDragon() =>
        ActionReady(RiseOfTheDragon) && HasStatusEffect(Buffs.DragonsFlight) && InActionRange(RiseOfTheDragon);

    private static bool CanNastrond() =>
        ActionReady(Nastrond) && HasStatusEffect(Buffs.NastrondReady) && LoTDActive && InActionRange(Nastrond);

    private static bool CanHighJump(bool onAoE = false, bool simpleMode = true) =>
        ActionReady(OriginalHook(Jump)) && CanUseWithHoldOptions(onAoE || simpleMode ? null : DRG_ST_JumpMovingOrInRanged) &&
        (onAoE
            ? IsOriginal(Jump) || IsOriginal(HighJump)
            : !LevelChecked(HighJump) && IsOriginal(Jump) ||
              LevelChecked(HighJump) && IsOriginal(HighJump) &&
              (simpleMode || !DRG_ST_DoubleMirage ||
               DRG_ST_DoubleMirage && (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDTimerActive)));

    private static bool CanDragonfireDive(bool onAoE = false, bool simpleMode = true, int hpThreshold = 0) =>
        ActionReady(DragonfireDive) && !HasStatusEffect(Buffs.DragonsFlight) &&
        GetTargetHPPercent() > hpThreshold &&
        CanUseWithHoldOptions(simpleMode ? null : onAoE ? DRG_AoE_DragonfireDiveMovingOrInRanged : DRG_ST_DragonfireDiveMovingOrInRanged) &&
        (LoTDTimerActive || !LevelChecked(Geirskogul));

    private static bool CanStardiver(bool onAoE = false, bool simpleMode = true) =>
        ActionReady(Stardiver) && LoTDActive && !HasStatusEffect(Buffs.StarcrossReady) &&
        CanUseWithHoldOptions(simpleMode ? null : onAoE ? DRG_AoE_StardiverMovingOrInRanged : DRG_ST_StardiverMovingOrInRanged);

    private static uint OutsideOfMelee(uint actionId, bool simpleMode = false, bool onAoE = false)
    {
        if (onAoE)
        {
            if (simpleMode || IsEnabled(Preset.DRG_AoE_Damage))
            {
                if ((simpleMode || IsEnabled(Preset.DRG_AoE_Mirage)) &&
                    CanMirageDive(true) && InCombat())
                    return MirageDive;

                if ((simpleMode || IsEnabled(Preset.DRG_AoE_Wyrmwind)) &&
                    CanUseWyrmwind() && InCombat())
                    return WyrmwindThrust;

                if ((simpleMode || IsEnabled(Preset.DRG_AoE_Starcross)) &&
                    CanStarcross() && InCombat())
                    return Starcross;

                if ((simpleMode || IsEnabled(Preset.DRG_AoE_RiseOfTheDragon)) &&
                    CanRiseOfTheDragon() && InCombat())
                    return RiseOfTheDragon;

                if ((simpleMode || IsEnabled(Preset.DRG_AoE_Geirskogul)) &&
                    CanUseGeirskogul(true) && InCombat())
                    return Geirskogul;

                if ((simpleMode || IsEnabled(Preset.DRG_AoE_Nastrond)) &&
                    CanNastrond() && InCombat())
                    return Nastrond;

                if ((simpleMode || IsEnabled(Preset.DRG_AoE_RangedUptime)) &&
                    ActionReady(PiercingTalon) && !CanDRGWeave())
                    return PiercingTalon;

                return simpleMode
                    ? DoBasicCombo(actionId, onAoE: true, simpleAoE: true)
                    : DoBasicCombo(actionId, onAoE: true);
            }

            return actionId;
        }

        if (simpleMode || IsEnabled(Preset.DRG_ST_Damage))
        {
            if ((simpleMode || IsEnabled(Preset.DRG_ST_Mirage)) &&
                CanMirageDive() && InCombat())
                return MirageDive;

            if ((simpleMode || IsEnabled(Preset.DRG_ST_Wyrmwind)) &&
                CanUseWyrmwind() && InCombat())
                return WyrmwindThrust;

            if ((simpleMode || IsEnabled(Preset.DRG_ST_Starcross)) &&
                CanStarcross() && InCombat())
                return Starcross;

            if ((simpleMode || IsEnabled(Preset.DRG_ST_RiseOfTheDragon)) &&
                CanRiseOfTheDragon() && InCombat())
                return RiseOfTheDragon;

            if ((simpleMode || IsEnabled(Preset.DRG_ST_Geirskogul)) &&
                CanUseGeirskogul() && InCombat())
                return Geirskogul;

            if ((simpleMode || IsEnabled(Preset.DRG_ST_Nastrond)) &&
                CanNastrond() && InCombat())
                return Nastrond;

            if ((simpleMode || IsEnabled(Preset.DRG_ST_RangedUptime)) &&
                ActionReady(PiercingTalon))
                return PiercingTalon;

            return simpleMode
                ? DoBasicCombo(actionId, true)
                : DoBasicCombo(actionId, IsEnabled(Preset.DRG_TrueNorthDynamic));
        }

        return actionId;
    }

    #endregion

    #region Misc

    private static IStatus? ChaosDebuff =>
        GetStatusEffect(ChaoticList[OriginalHook(ChaosThrust)], CurrentTarget);

    #endregion

    #region HP Thresholds

    private static int HPThresholdSTBattleLitany =>
        DRG_ST_BattleLitanyBossOption == 1 ||
        !InBossEncounter() ? DRG_ST_BattleLitanyHPOption : 0;

    private static int HPThresholdSTLanceCharge =>
        DRG_ST_LanceChargeBossOption == 1 ||
        !InBossEncounter() ? DRG_ST_LanceChargeHPOption : 0;

    private static int HPThresholdSTDragonfireDive =>
        DRG_ST_DragonfireDiveBossOption == 1 ||
        !InBossEncounter() ? DRG_ST_DragonfireDiveHPOption : 0;

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
