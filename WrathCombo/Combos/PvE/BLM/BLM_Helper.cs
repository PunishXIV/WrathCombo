using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.GameHelpers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
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
        (AstralFireStacks < 3 || !LevelChecked(Fire4) && TimeSinceFirestarterBuff >= GCDTotal * 3);

    private static bool CanFireParadox =>
        ActiveParadox && MP.Cur >= MP.FireParadox &&
        (!HasStatusEffect(Buffs.Firestarter) && AstralFireStacks < 3 ||
         JustUsed(FlareStar, GCDTotal * 4) ||
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
        JustUsed(Freeze, GCDTotal) || JustUsed(Blizzard4, GCDTotal);

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

    #region ST Weaves

    private static bool CanStAmplifierWeave(bool useAmplifier = true) =>
        useAmplifier && ActionReady(Amplifier) && !HasMaxPolyglotStacks;

    private static bool CanStLeyLinesWeave(
        bool useLeyLines = true,
        int minCharges = 1,
        bool allowMoving = true,
        double timeStillSeconds = 2.5,
        int hpThreshold = 0) =>
        useLeyLines &&
        ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
        !JustUsed(LeyLines) &&
        GetRemainingCharges(LeyLines) > minCharges &&
        (allowMoving || !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(timeStillSeconds)) &&
        GetTargetHPPercent() > hpThreshold;

    private static uint TryEndOfFireWeave(
        bool useManafont = true,
        bool useSwiftcast = true,
        bool useTriplecast = true,
        bool triplecastIgnoreLeyLines = true,
        bool triplecastRequireChargeReserve = false,
        bool useTranspose = true,
        bool transposeIncludeLowMp = false,
        uint fallbackWhenNoTranspose = 0)
    {
        if (!EndOfFirePhase)
            return 0;

        if (useManafont && ActionReady(Manafont))
            return Manafont;

        if (useSwiftcast &&
            ActionReady(Role.Swiftcast) && JustUsed(Despair) &&
            GetCooldownRemainingTime(Manafont) > GCDTotal &&
            !HasStatusEffect(Buffs.Triplecast) &&
            InActionRange(Fire) && HasBattleTarget())
            return Role.Swiftcast;

        if (useTriplecast &&
            ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
            !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
            InActionRange(Fire) && HasBattleTarget() &&
            (triplecastIgnoreLeyLines || !HasStatusEffect(Buffs.LeyLines)) &&
            (!triplecastRequireChargeReserve || HasTriplecastChargesForMovement()) &&
            JustUsed(Despair) && !JustUsed(Triplecast) && !JustUsed(Manafont))
            return Triplecast;

        if (useTranspose &&
            ActionReady(Transpose) &&
            (HasStatusEffect(Role.Buffs.Swiftcast) ||
             HasStatusEffect(Buffs.Triplecast) ||
             transposeIncludeLowMp && !LevelChecked(Fire3) && MP.Cur < MP.FireI))
            return Transpose;

        return fallbackWhenNoTranspose != 0 && !ActionReady(Transpose)
            ? fallbackWhenNoTranspose
            : 0;
    }

    private static uint TryIceWeave(
        bool useTranspose = true,
        bool useSwiftcast = true,
        bool useTriplecast = false,
        bool triplecastIgnoreLeyLines = true,
        bool triplecastRequireChargeReserve = false)
    {
        if (!IcePhase)
            return 0;

        if (useTranspose && MP.Full && JustUsed(Paradox) && ActionReady(Transpose))
            return Transpose;

        if (ActionReady(Blizzard3) && UmbralIceStacks < 3)
        {
            if (useSwiftcast &&
                ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                HasBattleTarget() && InActionRange(Blizzard))
                return Role.Swiftcast;

            if (useTriplecast &&
                ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                HasBattleTarget() && InActionRange(Blizzard) && !JustUsed(Triplecast) &&
                !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                (triplecastIgnoreLeyLines || !HasStatusEffect(Buffs.LeyLines)) &&
                (!triplecastRequireChargeReserve || HasTriplecastChargesForMovement()) &&
                JustUsed(Despair) && !JustUsed(Manafont))
                return Triplecast;
        }

        return 0;
    }

    private static bool CanStManaward(
        bool useManaward = false,
        int hpThreshold = 60,
        bool simpleLogic = true,
        int triggerMode = 0,
        bool soloOption = false) =>
        useManaward && ActionReady(Manaward) && !LocalPlayer!.HasShield() &&
        (simpleLogic
            ? PlayerHealthPercentageHp() < hpThreshold && !IsInParty() || GroupDamageIncoming()
            : (triggerMode == 0 && PlayerHealthPercentageHp() <= hpThreshold && GroupDamageIncoming()) ||
              ((triggerMode == 1 || triggerMode == 0 && soloOption && !IsInParty()) &&
               PlayerHealthPercentageHp() <= hpThreshold) ||
              triggerMode == 2 && GroupDamageIncoming());

    private static bool CanStAddleWeave(bool useAddle = true) =>
        useAddle && Role.CanAddle() && GroupDamageIncoming();

    #endregion

    #region ST GCD Utilities

    private static bool CanStScatheFiller(bool useScathe = true) =>
        useScathe && IsMoving() && !LevelChecked(Triplecast) && ActionReady(Scathe);

    private static uint TryStPolyglotOvercap(bool usePolyglot = true) =>
        usePolyglot && PolyglotOvercapProtection()
            ? PolyglotSpell
            : 0;

    private static uint TryStThunder(bool useThunder = true, int hpThreshold = 0, float dotRefresh = 5f) =>
        useThunder && CanThunder(hpThreshold, dotRefresh)
            ? OriginalHook(Thunder)
            : 0;

    private static uint TryStAmplifierXeno(bool useAmplifier = true, bool usePolyglot = true) =>
        useAmplifier && usePolyglot &&
        LevelChecked(Amplifier) &&
        GetCooldownRemainingTime(Amplifier) < 5 &&
        HasMaxPolyglotStacks
            ? Xenoglossy
            : 0;

    private static uint TryStMovementGcd(bool useConfiguredPriority = false)
    {
        if (!IsMoving() || !InCombat() || !HasBattleTarget() || !InActionRange(Fire))
            return 0;

        if (useConfiguredPriority)
        {
            foreach(int priority in BLM_ST_MovementPriority.OrderBy(x => x))
            {
                int index = BLM_ST_MovementPriority.IndexOf(priority);
                if (TryMovementAction(index, out uint action))
                    return action;
            }

            return 0;
        }

        if (ActionReady(Triplecast) &&
            !HasStatusEffect(Buffs.Triplecast) &&
            !HasStatusEffect(Role.Buffs.Swiftcast) &&
            !HasStatusEffect(Buffs.LeyLines) &&
            !JustUsed(Triplecast))
            return Triplecast;

        if (LevelChecked(Paradox) &&
            FirePhase && ActiveParadox &&
            MP.Cur >= MP.FireParadox &&
            !HasStatusEffect(Buffs.Firestarter) &&
            !HasStatusEffect(Buffs.Triplecast) &&
            !HasStatusEffect(Role.Buffs.Swiftcast))
            return OriginalHook(Fire);

        if (ActionReady(Role.Swiftcast) &&
            !HasStatusEffect(Buffs.Triplecast))
            return Role.Swiftcast;

        if (HasPolyglot &&
            !HasStatusEffect(Buffs.Triplecast) &&
            !HasStatusEffect(Role.Buffs.Swiftcast))
            return PolyglotSpell;

        return 0;
    }

    #endregion

    #region AoE Weaves

    private static uint TryAoEMovementTriplecast(bool useTriplecast = true) =>
        useTriplecast &&
        IsMoving() && InCombat() &&
        InActionRange(Fire2) && HasBattleTarget() &&
        ActionReady(Triplecast) &&
        !HasStatusEffect(Buffs.Triplecast) &&
        !JustUsed(Triplecast)
            ? Triplecast
            : 0;

    private static bool CanAoEManafontWeave(bool useManafont = true) =>
        useManafont && ActionReady(Manafont) && EndOfFirePhase;

    private static bool CanAoETransposeWeave(bool useTranspose = true) =>
        useTranspose && ActionReady(Transpose) && (EndOfFirePhase || EndOfIcePhaseAoE);

    private static bool CanAoEAmplifierWeave(bool useAmplifier = true, int polyglotTimerThreshold = 20) =>
        useAmplifier && ActionReady(Amplifier) && PolyglotTimer >= polyglotTimerThreshold;

    private static bool CanAoELeyLinesWeave(
        bool useLeyLines = true,
        int minCharges = 0,
        bool allowMoving = true,
        double timeStillSeconds = 2.5,
        int hpThreshold = 0) =>
        useLeyLines &&
        ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
        !JustUsed(LeyLines) &&
        GetRemainingCharges(LeyLines) > minCharges &&
        (allowMoving || !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(timeStillSeconds)) &&
        GetTargetHPPercent() > hpThreshold;

    #endregion

    #region AoE GCDs

    private static uint TryAoEPolyglotOvercap(bool usePolyglot = true) =>
        usePolyglot && PolyglotOvercapProtection() && ActionReady(Foul)
            ? Foul
            : 0;

    private static uint TryAoEPolyglot(bool usePolyglot = true) =>
        usePolyglot &&
        (EndOfFirePhase || EndOfIcePhaseAoE || IcePhase && JustUsedFreezeOrBlizzard) &&
        HasPolyglot && ActionReady(Foul)
            ? Foul
            : 0;

    private static uint TryAoEThunder(bool useThunder = true, int hpThreshold = 0, float dotRefresh = 3f) =>
        useThunder && CanAoEThunder(hpThreshold, dotRefresh)
            ? OriginalHook(Thunder2)
            : 0;

    private static uint TryAoEParadoxFiller(bool useParadox = true) =>
        useParadox &&
        ActiveParadox && (EndOfIcePhaseAoE || IcePhase && JustUsedFreezeOrBlizzard)
            ? OriginalHook(Blizzard)
            : 0;

    private static uint UseAoEFirePhaseGcd(
        bool useTriplecast = true,
        int triplecastHoldCharges = 0,
        bool useTranspose = true,
        bool useBlizzard2Fallback = false)
    {
        if (CanFlareStar())
            return FlareStar;

        if (ActionReady(Fire2) && !TraitLevelChecked(Traits.UmbralHeart))
            return OriginalHook(Fire2);

        if (useTriplecast &&
            !HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
            HasBattleTarget() && InActionRange(Fire2) && !JustUsed(Triplecast) &&
            GetRemainingCharges(Triplecast) > triplecastHoldCharges &&
            HasMaxUmbralHeartStacks && GetCooldownRemainingTime(Manafont) > GCDTotal * 3)
            return Triplecast;

        if (ActionReady(Flare))
            return Flare;

        if (useBlizzard2Fallback &&
            LevelChecked(Blizzard2) &&
            TraitLevelChecked(Traits.AspectMasteryIII) &&
            !TraitLevelChecked(Traits.UmbralHeart))
            return OriginalHook(Blizzard2);

        if (useTranspose && ActionReady(Transpose) && MP.Cur < MP.FireAoE)
            return Transpose;

        return 0;
    }

    private static uint UseAoEIcePhaseGcd(
        bool useTranspose = true,
        bool useFire2WithoutTranspose = false,
        bool useBlizzard4Sub = true,
        bool flareTransposeRequiresNoUmbralHeart = false)
    {
        if (HasMaxUmbralHeartStacks ||
            MP.Cur >= 5000 && LevelChecked(Flare) &&
            (!flareTransposeRequiresNoUmbralHeart || !TraitLevelChecked(Traits.UmbralHeart)) ||
            MP.Full && !LevelChecked(Flare))
        {
            if (useTranspose && ActionReady(Transpose))
                return Transpose;

            if (useFire2WithoutTranspose &&
                LevelChecked(Fire2) &&
                TraitLevelChecked(Traits.AspectMasteryIII))
                return OriginalHook(Fire2);
        }

        if (ActionReady(Freeze))
            return useBlizzard4Sub &&
                   ActionReady(Blizzard4) && HasBattleTarget() &&
                   NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                ? Blizzard4
                : Freeze;

        if (!ActionReady(Freeze) && LevelChecked(Blizzard2))
            return OriginalHook(Blizzard2);

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
               PrioritizedMovement[index].Logic();
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
