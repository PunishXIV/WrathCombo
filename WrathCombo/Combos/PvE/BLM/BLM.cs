using System.Linq;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.BLM.Config;
namespace WrathCombo.Combos.PvE;

internal partial class BLM : Caster
{
    internal class BLM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            if (CanWeave())
            {
                if (ActionReady(Amplifier) && !HasMaxPolyglotStacks)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines))
                    return LeyLines;

                if (EndOfFirePhase)
                {
                    if (ActionReady(Manafont) && EndOfFirePhase)
                        return Manafont;

                    if (ActionReady(Role.Swiftcast) && JustUsed(Despair) &&
                        !ActionReady(Manafont) && !HasStatusEffect(Buffs.Triplecast))
                        return Role.Swiftcast;

                    if (ActionReady(Transpose) &&
                        LevelChecked(Fire3) &&
                        (HasStatusEffect(Role.Buffs.Swiftcast) ||
                         HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (IcePhase)
                {
                    if (CurMp is MP.MaxMP &&
                        JustUsed(Paradox) &&
                        ActionReady(Transpose))
                        return Transpose;

                    if (LevelChecked(Blizzard3) && UmbralIceStacks < 3 &&
                        ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                        return Role.Swiftcast;
                }

                if (ActionReady(Manaward) && PlayerHealthPercentageHp() < 40)
                    return Manaward;
            }

            if (IsMoving() && !LevelChecked(Triplecast))
                return Scathe;

            if (HasMaxPolyglotStacks && PolyglotTimer <= 5000)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (LevelChecked(Thunder) && HasStatusEffect(Buffs.Thunderhead) &&
                CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder)]) &&
                ((ThunderDebuffST is null && ThunderDebuffAoE is null) ||
                 ThunderDebuffST?.RemainingTime <= 3 ||
                 ThunderDebuffAoE?.RemainingTime <= 3) &&
                GetTargetHPPercent() > 0)
                return OriginalHook(Thunder);

            if (LevelChecked(Amplifier) &&
                GetCooldownRemainingTime(Amplifier) < 5 &&
                HasMaxPolyglotStacks)
                return Xenoglossy;

            if (IsMoving() && InCombat())
            {
                if (ActionReady(Triplecast) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    !HasStatusEffect(Buffs.LeyLines))
                    return Triplecast;

                if (LevelChecked(Paradox) &&
                    FirePhase && ActiveParadox &&
                    !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Paradox;

                if (ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Role.Swiftcast;

                if (HasPolyglotStacks() &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (FirePhase)
            {
                // TODO: Revisit when Raid Buff checks are in place
                if ((PolyglotStacks > 1))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && AstralFireStacks < 3 ||
                    !LevelChecked(Fire4) && TimeSinceFirestarterBuff >= 2 && LevelChecked(Fire3))
                    return Fire3;

                if (ActiveParadox &&
                    CurMp > 1600 &&
                    (AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return Paradox;

                if (FlarestarReady)
                    return FlareStar;

                if (ActionReady(FireSpam) && (LevelChecked(Despair) && CurMp - MP.FireI >= 800 || !LevelChecked(Despair)))
                    return FireSpam;

                if (ActionReady(Despair))
                    return Despair;

                if (LevelChecked(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (ActionReady(Transpose) &&
                    !LevelChecked(Fire3) &&
                    CurMp < MP.FireI)
                    return Transpose;
            }

            if (IcePhase)
            {
                if (UmbralHearts is 3 &&
                    UmbralIceStacks is 3 &&
                    ActiveParadox)
                    return Paradox;

                if (CurMp is MP.MaxMP)
                {
                    if (LevelChecked(Fire3))
                        return Fire3;

                    if (ActionReady(Transpose) && !LevelChecked(Blizzard3))
                        return Transpose;
                }

                if (LevelChecked(Blizzard3) && UmbralIceStacks < 3 &&
                    (HasStatusEffect(Role.Buffs.Swiftcast) ||
                     HasStatusEffect(Buffs.Triplecast) ||
                     JustUsed(Freeze, 10f)))
                    return Blizzard3;

                if (ActionReady(BlizzardSpam))
                    return BlizzardSpam;
            }

            if (LevelChecked(Fire3))
                return CurMp >= 7500
                    ? Fire3
                    : Blizzard3;

            return actionID;
        }
    }

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            // Opener
            if (IsEnabled(CustomComboPreset.BLM_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && !HasMaxPolyglotStacks)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_LeyLines) &&
                    ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > BLM_ST_LeyLinesCharges)
                    return LeyLines;

                if (EndOfFirePhase)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Manafont) &&
                        ActionReady(Manafont) && EndOfFirePhase)
                        return Manafont;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                        ActionReady(Role.Swiftcast) && JustUsed(Despair) &&
                        !ActionReady(Manafont) && !HasStatusEffect(Buffs.Triplecast))
                        return Role.Swiftcast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                        !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                        (BLM_ST_Triplecast_SubOption == 0 || !HasStatusEffect(Buffs.LeyLines)) &&
                        ((BLM_ST_MovementOption[0] && GetRemainingCharges(Triplecast) > BLM_ST_Triplecast_Movement) ||
                         !BLM_ST_MovementOption[0]) && JustUsed(Despair) && !ActionReady(Manafont))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) &&
                        (HasStatusEffect(Role.Buffs.Swiftcast) ||
                         HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (IcePhase)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        CurMp is MP.MaxMP && JustUsed(Paradox) &&
                        ActionReady(Transpose))
                        return Transpose;

                    if (LevelChecked(Blizzard3) && UmbralIceStacks < 3)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                            ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                            return Role.Swiftcast;

                        if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                            !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                            (BLM_ST_Triplecast_SubOption == 0 || !HasStatusEffect(Buffs.LeyLines)) &&
                            ((BLM_ST_MovementOption[0] && GetRemainingCharges(Triplecast) > BLM_ST_Triplecast_Movement) ||
                             !BLM_ST_MovementOption[0]) && JustUsed(Despair) && !ActionReady(Manafont))
                            return Triplecast;
                    }
                }

                if (IsEnabled(CustomComboPreset.BLM_ST_Manaward) &&
                    ActionReady(Manaward) && PlayerHealthPercentageHp() < BLM_ST_Manaward_Threshold)
                    return Manaward;
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_UseScathe) &&
                IsMoving() && !LevelChecked(Triplecast))
                return Scathe;

            //Overcap protection
            if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                HasMaxPolyglotStacks && PolyglotTimer <= 5000)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                LevelChecked(Thunder) && HasStatusEffect(Buffs.Thunderhead))
            {
                float refreshTimer = BLM_ST_ThunderUptime_Threshold;
                int hpThreshold = BLM_ST_Thunder_SubOption == 1 || !InBossEncounter() ? BLM_ST_ThunderOption : 0;

                if (CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder)]) &&
                    ((ThunderDebuffST is null && ThunderDebuffAoE is null) ||
                     ThunderDebuffST?.RemainingTime <= refreshTimer ||
                     ThunderDebuffAoE?.RemainingTime <= refreshTimer) &&
                    GetTargetHPPercent() > hpThreshold)
                    return OriginalHook(Thunder);
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                LevelChecked(Amplifier) &&
                GetCooldownRemainingTime(Amplifier) < 5 &&
                HasMaxPolyglotStacks)
                return Xenoglossy;

            if (IsMoving() && InCombat())
            {
                foreach(int priority in BLM_ST_Movement_Priority.Items.OrderBy(x => x))
                {
                    int index = BLM_ST_Movement_Priority.IndexOf(priority);
                    if (CheckMovementConfigMeetsRequirements(index, out uint action))
                        return action;
                }
            }

            if (FirePhase)
            {
                // TODO: Revisit when Raid Buff checks are in place
                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                    ((BLM_ST_MovementOption[3] &&
                      PolyglotStacks > BLM_ST_Polyglot_Movement &&
                      PolyglotStacks > BLM_ST_Polyglot_Save) ||
                     (!BLM_ST_MovementOption[3] &&
                      PolyglotStacks > BLM_ST_Polyglot_Save)))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && AstralFireStacks < 3 ||
                    !LevelChecked(Fire4) && TimeSinceFirestarterBuff >= 2 && LevelChecked(Fire3))
                    return Fire3;

                if (ActiveParadox &&
                    CurMp > 1600 &&
                    (AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return Paradox;

                if (IsEnabled(CustomComboPreset.BLM_ST_FlareStar) &&
                    FlarestarReady)
                    return FlareStar;

                if (ActionReady(FireSpam) && (LevelChecked(Despair) && CurMp - MP.FireI >= 800 || !LevelChecked(Despair)))
                    return FireSpam;

                if (IsEnabled(CustomComboPreset.BLM_ST_Despair) &&
                    ActionReady(Despair))
                    return Despair;

                if (LevelChecked(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                    ActionReady(Transpose) &&
                    !LevelChecked(Fire3) &&
                    CurMp < MP.FireI)
                    return Transpose;
            }

            if (IcePhase)
            {
                if (UmbralHearts is 3 &&
                    UmbralIceStacks is 3 &&
                    ActiveParadox)
                    return Paradox;

                if (CurMp is MP.MaxMP)
                {
                    if (LevelChecked(Fire3))
                        return Fire3;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) &&
                        !LevelChecked(Blizzard3))
                        return Transpose;
                }

                if (LevelChecked(Blizzard3) && UmbralIceStacks < 3 &&
                    (HasStatusEffect(Role.Buffs.Swiftcast) ||
                     HasStatusEffect(Buffs.Triplecast) ||
                     JustUsed(Freeze, 10f)))
                    return Blizzard3;

                if (ActionReady(BlizzardSpam))
                    return BlizzardSpam;
            }

            if (LevelChecked(Fire3))
                return CurMp >= 7500
                    ? Fire3
                    : Blizzard3;

            return actionID;
        }
    }

    internal class BLM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            if (CanWeave())
            {
                if (ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (ActionReady(Transpose) &&
                    (EndOfFirePhase ||
                     EndOfIcePhaseAoEMaxLevel))
                    return Transpose;

                if (ActionReady(Amplifier) && PolyglotTimer >= 20000)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > 1)
                    return LeyLines;
            }

            if ((EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel) &&
                HasPolyglotStacks())
                return Foul;

            if (HasStatusEffect(Buffs.Thunderhead) && LevelChecked(Thunder2) &&
                GetTargetHPPercent() > 1 &&
                CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder2)]) &&
                ((ThunderDebuffAoE is null && ThunderDebuffST is null) ||
                 ThunderDebuffAoE?.RemainingTime <= 3 ||
                 ThunderDebuffST?.RemainingTime <= 3) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel))
                return OriginalHook(Thunder2);

            if (ActiveParadox && EndOfIcePhaseAoEMaxLevel)
                return Paradox;

            if (FirePhase)
            {
                if (FlarestarReady)
                    return FlareStar;

                if (LevelChecked(Fire2) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Fire2);

                if (!HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    GetRemainingCharges(Triplecast) > 1 && HasMaxUmbralHeartStacks &&
                    !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (ActionReady(Transpose) &&
                    !LevelChecked(Fire3) &&
                    CurMp < MP.FireAoE)
                    return Transpose;
            }

            if (IcePhase)
            {
                if ((CurMp is MP.MaxMP || HasMaxUmbralHeartStacks) &&
                    ActionReady(Transpose))
                    return Transpose;

                if (LevelChecked(Freeze))
                    return LevelChecked(Blizzard4) && HasBattleTarget() &&
                           NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                        ? Blizzard4
                        : Freeze;

                if (!LevelChecked(Freeze) && LevelChecked(Blizzard2))
                    return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }

    internal class BLM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();


            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_Manafont) &&
                    ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) &&
                    (EndOfFirePhase || EndOfIcePhaseAoEMaxLevel))
                    return Transpose;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Amplifier) &&
                    ActionReady(Amplifier) && PolyglotTimer >= 20000)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_AoE_LeyLines) &&
                    ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > BLM_AoE_LeyLinesCharges)
                    return LeyLines;
            }

            if (IsEnabled(CustomComboPreset.BLM_AoE_UsePolyglot) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel) &&
                HasPolyglotStacks())
                return Foul;

            if (IsEnabled(CustomComboPreset.BLM_AoE_Thunder) &&
                HasStatusEffect(Buffs.Thunderhead) && LevelChecked(Thunder2) &&
                CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder2)]) &&
                (GetTargetHPPercent() > BLM_AoE_ThunderHP) &&
                ((ThunderDebuffAoE is null && ThunderDebuffST is null) ||
                 ThunderDebuffAoE?.RemainingTime <= 3 ||
                 ThunderDebuffST?.RemainingTime <= 3) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel))
                return OriginalHook(Thunder2);

            if (IsEnabled(CustomComboPreset.BLM_AoE_ParadoxFiller) &&
                ActiveParadox && EndOfIcePhaseAoEMaxLevel)
                return Paradox;

            if (FirePhase)
            {
                if (FlarestarReady)
                    return FlareStar;

                if (ActionReady(Fire2) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Fire2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Triplecast) &&
                    !HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    GetRemainingCharges(Triplecast) > BLM_AoE_Triplecast_HoldCharges && HasMaxUmbralHeartStacks &&
                    !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (IsNotEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    LevelChecked(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Blizzard2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && CurMp < MP.FireAoE)
                    return Transpose;
            }

            if (IcePhase)
            {
                if (HasMaxUmbralHeartStacks || CurMp is MP.MaxMP)
                {
                    if (IsNotEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        LevelChecked(Fire2) && TraitLevelChecked(Traits.AspectMasteryIII))
                        return OriginalHook(Fire2);

                    if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        ActionReady(Transpose))
                        return Transpose;
                }
                if (LevelChecked(Freeze))
                    return IsEnabled(CustomComboPreset.BLM_AoE_Blizzard4Sub) &&
                           LevelChecked(Blizzard4) && HasBattleTarget() &&
                           NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                        ? Blizzard4
                        : Freeze;

                if (!LevelChecked(Freeze) && LevelChecked(Blizzard2))
                    return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }

    internal class BLM_Variant_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Variant_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && Variant.CanRaise(CustomComboPreset.BLM_Variant_Raise)
                ? Variant.Raise
                : actionID;
    }

    internal class BLM_ScatheXeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_ScatheXeno;

        protected override uint Invoke(uint actionID) =>
            actionID is Scathe && LevelChecked(Xenoglossy) && HasPolyglotStacks()
                ? Xenoglossy
                : actionID;
    }

    internal class BLM_Blizzard1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Blizzard1to3;

        protected override uint Invoke(uint actionID) =>
            actionID switch
            {
                Blizzard when BLM_B1to3 == 0 && LevelChecked(Blizzard3) && (FirePhase || UmbralIceStacks is 1 || UmbralIceStacks is 2) => Blizzard3,
                Blizzard3 when BLM_B1to3 == 1 && LevelChecked(Blizzard3) && IcePhase && UmbralIceStacks is 3 => OriginalHook(Blizzard),
                var _ => actionID
            };
    }

    internal class BLM_Fire1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Fire1to3;

        protected override uint Invoke(uint actionID) =>
            actionID switch
            {
                Fire when BLM_F1to3 == 0 && LevelChecked(Fire3) && (IcePhase || ((AstralFireStacks is 1 or 2) && HasStatusEffect(Buffs.Firestarter)) || !InCombat()) && !JustUsed(Fire3) => Fire3,
                Fire3 when BLM_F1to3 == 1 && LevelChecked(Fire3) && FirePhase && (AstralFireStacks is 3 || ((AstralFireStacks is 1 or 2) && !HasStatusEffect(Buffs.Firestarter))) && !JustUsed(OriginalHook(Fire)) => OriginalHook(Fire),
                var _ => actionID
            };
    }

    internal class BLM_Fire4to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Fire4to3;
        protected override uint Invoke(uint actionID) =>
            actionID is Fire4 && LevelChecked(Fire4) && (IcePhase || AstralFireStacks is 1 || AstralFireStacks is 2 || !InCombat())
                ? Fire3
                : actionID;
    }

    internal class BLM_FireandIce : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_FireandIce;

        protected override uint Invoke(uint actionID) =>
            actionID switch
            {
                Fire4 when FirePhase && LevelChecked(Fire4) => Fire4,
                Fire4 when IcePhase && LevelChecked(Blizzard4) => Blizzard4,
                Flare when FirePhase && LevelChecked(Flare) => Flare,
                Flare when IcePhase && LevelChecked(Freeze) => Freeze,
                var _ => actionID
            };
    }

    internal class BLM_FireFlarestar : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_FireFlarestar;

        protected override uint Invoke(uint actionID) =>
            actionID is Fire4 && FirePhase && FlarestarReady && LevelChecked(FlareStar) ||
            actionID is Flare && FirePhase && FlarestarReady && LevelChecked(FlareStar)
                ? FlareStar
                : actionID;
    }

    internal class BLM_Blizzard4toDespair : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Blizzard4toDespair;
        protected override uint Invoke(uint actionID) =>
            actionID is Blizzard4 && FirePhase && LevelChecked(Despair)
                ? Despair
                : actionID;
    }

    internal class BLM_Fire1Despair : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Fire1Despair;
        protected override uint Invoke(uint actionID) =>
            actionID is Fire && FirePhase && CurMp < 2400
                ? Despair
                : OriginalHook(Fire);
    }

    internal class BLM_FreezeParadox : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_FreezeParadox;
        protected override uint Invoke(uint actionID) =>
            actionID is Freeze && HasMaxUmbralHeartStacks && LevelChecked(Paradox) && ActiveParadox && IcePhase
                ? OriginalHook(Blizzard)
                : actionID;
    }

    internal class BLM_FlareParadox : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_FlareParadox;
        protected override uint Invoke(uint actionID) =>
            actionID is FlareStar && FirePhase && LevelChecked(FlareStar) && ActiveParadox && AstralSoulStacks < 6
                ? OriginalHook(Fire)
                : actionID;
    }

    internal class BLM_FreezeBlizzard2 : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_FreezeBlizzard2;
        protected override uint Invoke(uint actionID) =>
            actionID is Freeze && !LevelChecked(Freeze)
                ? Blizzard2
                : actionID;
    }

    internal class BLM_AmplifierXeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_AmplifierXeno;
        protected override uint Invoke(uint actionID) =>
            actionID is Amplifier && HasMaxPolyglotStacks
                ? Xenoglossy
                : actionID;
    }

    internal class BLM_Between_The_LeyLines : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Between_The_LeyLines;

        protected override uint Invoke(uint actionID) =>
            actionID is LeyLines && HasStatusEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_Aetherial_Manipulation : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID) =>
            actionID is AetherialManipulation && ActionReady(BetweenTheLines) &&
            HasStatusEffect(Buffs.LeyLines) && !HasStatusEffect(Buffs.CircleOfPower) && !IsMoving()
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_UmbralSoul : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_UmbralSoul;

        protected override uint Invoke(uint actionID) =>
            actionID is Transpose && IcePhase && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
    }

    internal class BLM_TriplecastProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.BLM_TriplecastProtection;

        protected override uint Invoke(uint actionID) =>
            actionID is Triplecast && HasStatusEffect(Buffs.Triplecast) && LevelChecked(Triplecast)
                ? All.SavageBlade
                : actionID;
    }
}
