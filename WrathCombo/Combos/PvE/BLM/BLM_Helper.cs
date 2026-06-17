using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.GameHelpers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.BLM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    #region Misc

    private static int MaxPolyglot =>
        TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
        TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;

    private static bool HasMaxPolyglotStacks =>
        PolyglotStacks == MaxPolyglot;

    private static int BossHpThreshold(int hpBossOption, int hpOption, bool isBoss) =>
        hpBossOption == 1 || !isBoss ? hpOption : 0;

    private static int LeyLinesHPThreshold =>
        BossHpThreshold(BLM_ST_LeyLinesHPBossOption, BLM_ST_LeyLinesHPOption, InBossEncounter());

    private static bool HasPolyglot =>
        PolyglotStacks > 0;

    #endregion

    #region Polyglot

    private static uint PolyglotSpell =>
        LevelChecked(Xenoglossy) ? Xenoglossy : Foul;

    private static bool PolyglotOvercapProtection() =>
        HasMaxPolyglotStacks && PolyglotTimer <= 5;

    private static bool ShouldSpendPolyglotInFire(
        bool alwaysSpend = true,
        bool useMovementSwiftcastPolyglot = false,
        int polyglotMovementThreshold = 0,
        int polyglotSaveUsage = 0)
    {
        if (!HasPolyglot)
            return false;

        if (alwaysSpend)
            return true;

        if (useMovementSwiftcastPolyglot &&
            PolyglotStacks > polyglotMovementThreshold &&
            PolyglotStacks > polyglotSaveUsage)
            return true;

        return !useMovementSwiftcastPolyglot &&
               PolyglotStacks > polyglotSaveUsage;
    }

    #endregion

    #region Fire Phase

    private static bool CanFlareStar() =>
        LevelChecked(FlareStar) && AstralSoulStacks is 6;

    private static float TimeSinceFirestarterBuff =>
        HasStatusEffect(Buffs.Firestarter) ? GetPartyMembers().First().TimeSinceBuffApplied(Buffs.Firestarter) : 0;

    private static uint FireSpam =>
        ActionReady(Fire4)
            ? Fire4
            : Fire;

    private static bool CanFire3 =>
        LevelChecked(Fire3) && HasStatusEffect(Buffs.Firestarter) &&
        (AstralFireStacks < 3 || !LevelChecked(Fire4) && TimeSinceFirestarterBuff >= GCD * 3);

    private static bool CanFireParadox =>
        ActiveParadox && MP.Cur >= MP.FireParadox &&
        (!HasStatusEffect(Buffs.Firestarter) && AstralFireStacks < 3 ||
         JustUsed(FlareStar, GCD * 4) ||
         !LevelChecked(FlareStar) && ActionReady(Despair));

    private static bool EndOfFirePhase =>
        FirePhase && !ActionReady(Despair) && !ActionReady(FireSpam) && !ActionReady(FlareStar);

    #endregion

    #region Ice Phase

    private static uint BlizzardSpam =>
        ActionReady(Blizzard4)
            ? Blizzard4
            : Blizzard;

    private static bool HasMaxUmbralHeartStacks =>
        UmbralHearts is 3;

    private static bool EndOfIcePhaseAoE =>
        IcePhase && HasMaxUmbralHeartStacks && TraitLevelChecked(Traits.EnhancedAstralFire);

    private static bool JustUsedFreezeOrBlizzard =>
        JustUsed(Freeze, GCD) || JustUsed(Blizzard4, GCD);

    #endregion

    #region Thunder

    private static IStatus? ThunderDebuffST =>
        GetStatusEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget);

    private static IStatus? ThunderDebuffAoE =>
        GetStatusEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget);

    internal static bool CanThunder(int hpThreshold = 0, float dotRefresh = 5f)
    {
        uint dotAction = OriginalHook(Thunder);
        ThunderList.TryGetValue(dotAction, out ushort dotDebuffID);
        float dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(dotAction) &&
               CanApplyStatus(CurrentTarget, dotDebuffID) &&
               !JustUsedOn(dotAction, CurrentTarget, 5f) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh;
    }

    internal static int ComputeHpThreshold()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? BLM_ST_ThunderBossHPOption : BLM_ST_ThunderBossAddsHPOption;

        return BLM_ST_ThunderTrashHPOption;
    }

    private static bool CanAoEThunder(int hpThreshold = 0, float dotRefresh = 3f) =>
        LevelChecked(OriginalHook(Thunder2)) && HasStatusEffect(Buffs.Thunderhead) &&
        CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder2)]) &&
        GetTargetHPPercent() > hpThreshold &&
        (!IcePhase || JustUsedFreezeOrBlizzard || EndOfIcePhaseAoE || !ActionReady(Freeze)) &&
        (ThunderDebuffAoE is null && ThunderDebuffST is null ||
         ThunderDebuffAoE?.RemainingTime <= dotRefresh ||
         ThunderDebuffST?.RemainingTime <= dotRefresh);

    #endregion

    #region Phase GCDs

    private static uint UseFirePhaseGcd(
        bool useFlareStar = true,
        bool useDespair = true,
        bool useTranspose = true,
        bool usePolyglot = true,
        bool alwaysSpendPolyglot = true,
        bool useMovementSwiftcastPolyglot = false,
        int polyglotMovementThreshold = 0,
        int polyglotSaveUsage = 0)
    {
        if (usePolyglot &&
            ShouldSpendPolyglotInFire(alwaysSpendPolyglot, useMovementSwiftcastPolyglot, polyglotMovementThreshold,
                polyglotSaveUsage))
            return PolyglotSpell;

        if (CanFireParadox)
            return OriginalHook(Fire);

        if (CanFire3)
            return Fire3;

        if (useFlareStar && CanFlareStar())
            return FlareStar;

        if (ActionReady(FireSpam) &&
            (LevelChecked(Despair) && MP.Cur - MP.FireI >= 800 || !LevelChecked(Despair)))
            return FireSpam;

        if (ActionReady(Flare) && !LevelChecked(Fire4) && MP.Cur <= 800)
            return Flare;

        if (useDespair && ActionReady(Despair))
            return Despair;

        if (ActionReady(Blizzard3) && EndOfFirePhase)
            return Blizzard3;

        if (useTranspose && ActionReady(Transpose) &&
            !LevelChecked(Fire3) && MP.Cur < MP.FireI)
            return Transpose;

        return 0;
    }

    private static uint UseIcePhaseGcd(bool useTranspose = true)
    {
        if (UmbralHearts is 3 && UmbralIceStacks is 3 && ActiveParadox)
            return OriginalHook(Blizzard);

        if (MP.Full || JustUsed(Blizzard4))
        {
            if (LevelChecked(Fire3))
                return Fire3;

            if (useTranspose && ActionReady(Transpose) && !ActionReady(Blizzard3))
                return Transpose;

            if (!ActionReady(Transpose) && LevelChecked(Fire))
                return Fire;
        }

        if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
            (HasStatusEffect(Role.Buffs.Swiftcast) ||
             HasStatusEffect(Buffs.Triplecast) ||
             JustUsed(Freeze, 10f)))
            return Blizzard3;

        if (ActionReady(BlizzardSpam))
            return BlizzardSpam;

        return 0;
    }

    private static uint UseOutOfPhaseGcd()
    {
        if (LevelChecked(Blizzard3))
            return MP.Cur < 7500 ? Blizzard3 : Fire3;

        if (LevelChecked(Fire) && !ActionReady(Transpose) && MP.Cur > MP.FireI)
            return Fire;

        return 0;
    }

    #endregion

    #region Movement Prio

    private const int MovementDespair = 0;
    private const int MovementTriplecast = 1;
    private const int MovementParadox = 2;
    private const int MovementSwiftcast = 3;
    private const int MovementXenoglossy = 4;
    private const int MovementFire3 = 5;
    private const int MovementScathe = 6;

    private static bool HasTriplecastChargesForMovement() =>
        !BLM_ST_MovementOption[MovementDespair] ||
        GetRemainingCharges(Triplecast) > BLM_ST_TriplecastMovementCharges;

    private static (uint Action, Preset Preset, System.Func<bool> Logic)[]
        PrioritizedMovement =>
    [
        //Despair at lvl 100
        (Despair, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementDespair] &&
                  ActionReady(Despair) &&
                  TraitLevelChecked(Traits.EnhancedAstralFire) &&
                  FirePhase && MP.Cur is >= 800 and < 1500 &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast)),

        //Triplecast
        (Triplecast, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementTriplecast] &&
                  ActionReady(Triplecast) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast) &&
                  !HasStatusEffect(Buffs.LeyLines) &&
                  !JustUsed(Triplecast)),

        // Paradox
        (OriginalHook(Fire), Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementParadox] &&
                  ActionReady(OriginalHook(Paradox)) &&
                  FirePhase && ActiveParadox &&
                  MP.Cur >= MP.FireParadox &&
                  !HasStatusEffect(Buffs.Firestarter) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast)),

        //Swiftcast
        (Role.Swiftcast, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementSwiftcast] &&
                  ActionReady(Role.Swiftcast) &&
                  !HasStatusEffect(Buffs.Triplecast)),

        //Xeno
        (Xenoglossy, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementXenoglossy] &&
                  ActionReady(Xenoglossy) &&
                  HasPolyglot &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast)),

        // Firestarter
        (Fire3, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementFire3] &&
                  ActionReady(Fire3) &&
                  FirePhase &&
                  HasStatusEffect(Buffs.Firestarter) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast)),

        //Scathe
        (Scathe, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[MovementScathe] &&
                  ActionReady(Scathe) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast))
    ];

    private static bool TryMovementAction(int index, out uint action)
    {
        action = PrioritizedMovement[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMovement[index].Logic() &&
               IsEnabled(PrioritizedMovement[index].Preset);
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked &&
            BLM_SelectedOpener == 0)
            return StandardOpener;

        if (FlareOpener.LevelChecked &&
            BLM_SelectedOpener == 1)
            return FlareOpener;

        return WrathOpener.Dummy;
    }

    internal static BLMStandardOpener StandardOpener = new();
    internal static BLMFlareOpener FlareOpener = new();

    internal class BLMStandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Fire3,
            HighThunder,
            Role.Swiftcast,
            Amplifier,
            Fire4,
            LeyLines,
            Fire4,
            Fire4,
            Fire4,
            Fire4,
            Xenoglossy,
            Manafont,
            Fire4,
            FlareStar,
            Fire4,
            Fire4,
            HighThunder,
            Fire4,
            Fire4,
            Fire4,
            Fire4,
            FlareStar,
            Despair,
            Transpose,
            Triplecast,
            Blizzard3,
            Blizzard4,
            Paradox,
            Transpose,
            Paradox,
            Fire3
        ];

        internal override UserData ContentCheckConfig => BLM_Balance_Content;

        public override List<int> DelayedWeaveSteps { get; set; } = [6];

        public override Preset Preset => Preset.BLM_ST_Opener;

        public override bool HasCooldowns() =>
            MP.Full &&
            IsOffCooldown(Manafont) &&
            GetRemainingCharges(Triplecast) >= 1 &&
            GetRemainingCharges(LeyLines) >= 1 &&
            IsOffCooldown(Role.Swiftcast) &&
            IsOffCooldown(Amplifier);
    }

    internal class BLMFlareOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Fire3,
            HighThunder,
            Role.Swiftcast,
            Amplifier,
            Fire4,
            LeyLines,
            Fire4,
            Xenoglossy,
            Fire4,
            Fire4,
            Despair,
            Manafont,
            Fire4,
            Fire4,
            FlareStar,
            Fire4,
            HighThunder,
            Fire4,
            Fire4,
            Fire4,
            Paradox,
            Triplecast,
            Flare,
            FlareStar,
            Transpose,
            Blizzard3,
            Blizzard4,
            Paradox,
            Transpose,
            Fire3
        ];

        internal override UserData ContentCheckConfig => BLM_Balance_Content;

        public override List<int> DelayedWeaveSteps { get; set; } = [6];

        public override Preset Preset => Preset.BLM_ST_Opener;

        public override bool HasCooldowns() =>
            MP.Full &&
            IsOffCooldown(Manafont) &&
            GetRemainingCharges(Triplecast) >= 1 &&
            GetRemainingCharges(LeyLines) >= 1 &&
            IsOffCooldown(Role.Swiftcast) &&
            IsOffCooldown(Amplifier);
    }

  #endregion

    #region Gauge

    private static BLMGauge Gauge => GetJobGauge<BLMGauge>();

    private static bool FirePhase => Gauge.InAstralFire;

    private static byte AstralFireStacks => Gauge.AstralFireStacks;

    private static bool IcePhase => Gauge.InUmbralIce;

    private static byte UmbralIceStacks => Gauge.UmbralIceStacks;

    private static byte UmbralHearts => Gauge.UmbralHearts;

    private static bool ActiveParadox => Gauge.IsParadoxActive;

    private static int AstralSoulStacks => Gauge.AstralSoulStacks;

    private static byte PolyglotStacks => Gauge.PolyglotStacks;

    private static int PolyglotTimer => Gauge.EnochianTimer / 1000;

    private static class MP
    {
        private static unsafe uint Max => Player.Character->MaxMana;

        internal static bool Full => Max == Cur;

        internal static unsafe uint Cur => Player.Character->Mana;

        internal static int FireI => GetResourceCost(OriginalHook(Fire));

        internal static int FireAoE => GetResourceCost(OriginalHook(Fire2));

        internal static int FireParadox => GetResourceCost(Paradox);
    }

    private static readonly FrozenDictionary<uint, ushort> ThunderList = new Dictionary<uint, ushort>
    {
        { Thunder, Debuffs.Thunder },
        { Thunder2, Debuffs.Thunder2 },
        { Thunder3, Debuffs.Thunder3 },
        { Thunder4, Debuffs.Thunder4 },
        { HighThunder, Debuffs.HighThunder },
        { HighThunder2, Debuffs.HighThunder2 }
    }.ToFrozenDictionary();

    private static float GCD = GetCooldown(OriginalHook(Fire)).CooldownTotal;

    #endregion

    #region ID's

    public const uint
        Fire = 141,
        Blizzard = 142,
        Thunder = 144,
        Fire2 = 147,
        Transpose = 149,
        Fire3 = 152,
        Thunder3 = 153,
        Blizzard3 = 154,
        AetherialManipulation = 155,
        Scathe = 156,
        Manaward = 157,
        Manafont = 158,
        Freeze = 159,
        Flare = 162,
        LeyLines = 3573,
        Blizzard4 = 3576,
        Fire4 = 3577,
        BetweenTheLines = 7419,
        Thunder4 = 7420,
        Triplecast = 7421,
        Foul = 7422,
        Thunder2 = 7447,
        Despair = 16505,
        UmbralSoul = 16506,
        Xenoglossy = 16507,
        Blizzard2 = 25793,
        HighFire2 = 25794,
        HighBlizzard2 = 25795,
        Amplifier = 25796,
        Paradox = 25797,
        HighThunder = 36986,
        HighThunder2 = 36987,
        FlareStar = 36989;

    // Debuff Pairs of Actions and Debuff
    public static class Buffs
    {
        public const ushort
            Firestarter = 165,
            LeyLines = 737,
            CircleOfPower = 738,
            Triplecast = 1211,
            AstralFire = 173, // Do not use, for translation only
            AstralFire2 = 174, // Do not use, for translation only
            AstralFire3 = 175, // Do not use, for translation only
            UmbralIce = 176, // Do not use, for translation only
            UmbralIce2 = 177, // Do not use, for translation only
            UmbralIce3 = 178, // Do not use, for translation only
            Thunderhead = 3870;
    }

    public static class Debuffs
    {
        public const ushort
            Thunder = 161,
            Thunder2 = 162,
            Thunder3 = 163,
            Thunder4 = 1210,
            HighThunder = 3871,
            HighThunder2 = 3872;
    }

    public static class Traits
    {
        public const uint
            UmbralHeart = 295,
            EnhancedPolyglot = 297,
            AspectMasteryIII = 459,
            EnhancedFoul = 461,
            EnhancedManafont = 463,
            Enochian = 460,
            EnhancedPolyglotII = 615,
            EnhancedAstralFire = 616;
    }

    #endregion
}
