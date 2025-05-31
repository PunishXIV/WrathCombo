using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.BLM.Config;
using static WrathCombo.Data.ActionWatching;
namespace WrathCombo.Combos.PvE;

internal partial class BLM : Caster
{
    internal class BLM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (CanSpellWeave() && !HasDoubleWeaved())
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

                    if (ActionReady(Transpose) && (HasStatusEffect(Role.Buffs.Swiftcast) || HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (IcePhase)
                {
                    if (JustUsed(Paradox) && CurMp is MP.MaxMP)
                        return Transpose;

                    if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
                        ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                        return Role.Swiftcast;
                }

                if (ActionReady(Manaward) && PlayerHealthPercentageHp() < 25)
                    return Manaward;
            }

            if (IsMoving() && !LevelChecked(Triplecast))
                return Scathe;

            if (HasMaxPolyglotStacks && PolyglotTimer <= 5000)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (LevelChecked(Thunder) && HasStatusEffect(Buffs.Thunderhead) &&
                (ThunderDebuffST is null && ThunderDebuffAoE is null ||
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

                if (ActionReady(Paradox) &&
                    FirePhase && ActiveParadox &&
                    !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return OriginalHook(Paradox);

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
                    !LevelChecked(Fire4) && TimeSinceFirestarterBuff >= 2 && ActionReady(Fire3))
                    return Fire3;

                if (ActiveParadox &&
                    CurMp > 1600 &&
                    (AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return OriginalHook(Paradox);

                if (FlarestarReady)
                    return FlareStar;

                if (ActionReady(FireSpam) && (LevelChecked(Despair) && CurMp - MP.FireI >= 800 || !LevelChecked(Despair)))
                    return FireSpam;

                if (ActionReady(Despair))
                    return Despair;

                if (ActionReady(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (ActionReady(Transpose))
                    return Transpose; //Level 4-34
            }

            if (IcePhase)
            {
                if (UmbralHearts is 3 &&
                    UmbralIceStacks is 3 &&
                    ActiveParadox)
                    return OriginalHook(Paradox);

                if (CurMp == MP.MaxMP)
                {
                    if (ActionReady(Fire3))
                        return Fire3; //35-100, pre-Paradox/scuffed starting combat

                    if (ActionReady(Transpose))
                        return Transpose; //Levels 4-34
                }

                if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
                    (JustUsed(Transpose, 5f) || JustUsed(Freeze, 10f)))
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            // Opener
            if (IsEnabled(CustomComboPreset.BLM_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (CanSpellWeave() && !HasDoubleWeaved())
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
                        !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Buffs.LeyLines) &&
                        ((BLM_ST_MovementOption[0] && GetRemainingCharges(Triplecast) > BLM_ST_Triplecast_Movement) ||
                         !BLM_ST_MovementOption[0]) && JustUsed(Despair) && !ActionReady(Manafont))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) && (HasStatusEffect(Role.Buffs.Swiftcast) || HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (IcePhase)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        JustUsed(Paradox) && CurMp is MP.MaxMP)
                        return Transpose;

                    if (ActionReady(Blizzard3) && UmbralIceStacks < 3)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                            ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                            return Role.Swiftcast;

                        if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                            !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Buffs.LeyLines) &&
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

            if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot))
            {
                //Overcap protection
                if (HasMaxPolyglotStacks && PolyglotTimer <= 5000)
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglotAsap) &&
                    HasPolyglotStacks())
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                LevelChecked(Thunder) && HasStatusEffect(Buffs.Thunderhead))
            {
                float refreshTimer = BLM_ST_ThunderUptime_Threshold;
                int hpThreshold = BLM_ST_Thunder_SubOption == 1 || !InBossEncounter() ? BLM_ST_ThunderOption : 0;

                if ((ThunderDebuffST is null && ThunderDebuffAoE is null ||
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
                if (BLM_ST_MovementOption[0] &&
                    ActionReady(Triplecast) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    !HasStatusEffect(Buffs.LeyLines))
                    return Triplecast;

                if (BLM_ST_MovementOption[1] &&
                    ActionReady(Paradox) &&
                    FirePhase && ActiveParadox &&
                    !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return OriginalHook(Paradox);

                if (BLM_ST_MovementOption[2] &&
                    ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Role.Swiftcast;

                if (BLM_ST_MovementOption[3] &&
                    HasPolyglotStacks() &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (FirePhase)
            {
                // TODO: Revisit when Raid Buff checks are in place
                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                    ((BLM_ST_MovementOption[3] && PolyglotStacks > BLM_ST_Polyglot_Movement) ||
                     (!BLM_ST_MovementOption[3] && HasPolyglotStacks())))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && AstralFireStacks < 3 ||
                    !LevelChecked(Fire4) && TimeSinceFirestarterBuff >= 2 && ActionReady(Fire3))
                    return Fire3;

                if (ActiveParadox &&
                    CurMp > 1600 &&
                    (AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return OriginalHook(Paradox);

                if (IsEnabled(CustomComboPreset.BLM_ST_FlareStar) &&
                    FlarestarReady)
                    return FlareStar;

                if (ActionReady(FireSpam) && (LevelChecked(Despair) && CurMp - MP.FireI >= 800 || !LevelChecked(Despair)))
                    return FireSpam;

                if (IsEnabled(CustomComboPreset.BLM_ST_Despair) &&
                    ActionReady(Despair))
                    return Despair;

                if (ActionReady(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                    ActionReady(Transpose))
                    return Transpose; //Level 4-34
            }

            if (IcePhase)
            {
                if (UmbralHearts is 3 &&
                    UmbralIceStacks is 3 &&
                    ActiveParadox)
                    return OriginalHook(Paradox);

                if (CurMp == MP.MaxMP)
                {
                    //35-100, pre-Paradox/scuffed starting combat
                    if (ActionReady(Fire3))
                        return Fire3;

                    //Levels 4-34
                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose))
                        return Transpose;
                }

                if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
                    (JustUsed(Transpose, 5f) || JustUsed(Freeze, 10f)))
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (CanSpellWeave() && !HasDoubleWeaved())
            {
                if (ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (ActionReady(Transpose) && (EndOfFirePhase || EndOfIcePhaseAoEMaxLevel))
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
                (GetTargetHPPercent() > 1) &&
                (ThunderDebuffAoE is null && ThunderDebuffST is null ||
                 ThunderDebuffAoE?.RemainingTime <= 3 ||
                 ThunderDebuffST?.RemainingTime <= 3) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel))
                return OriginalHook(Thunder2);

            if (ActiveParadox && EndOfIcePhaseAoEMaxLevel)
                return OriginalHook(Paradox);

            if (FirePhase)
            {
                if (FlarestarReady)
                    return FlareStar;

                if (ActionReady(Fire2) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Fire2);

                if (!HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    GetRemainingCharges(Triplecast) > 1 && HasMaxUmbralHeartStacks &&
                    !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (ActionReady(Transpose))
                    return Transpose;
            }

            if (IcePhase)
            {
                if ((CurMp == MP.MaxMP || HasMaxUmbralHeartStacks) &&
                    ActionReady(Transpose))
                    return Transpose;

                if (ActionReady(Freeze))
                    return LevelChecked(Blizzard4) && HasBattleTarget() && NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                        ? Blizzard4
                        : Freeze;

                if (!LevelChecked(Freeze) && ActionReady(Blizzard2))
                    return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }

    internal class BLM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (CanSpellWeave() && !HasDoubleWeaved())
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_Manafont) &&
                    ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && (EndOfFirePhase || EndOfIcePhaseAoEMaxLevel))
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
                (GetTargetHPPercent() > BLM_AoE_ThunderHP) &&
                (ThunderDebuffAoE is null && ThunderDebuffST is null ||
                 ThunderDebuffAoE?.RemainingTime <= 3 ||
                 ThunderDebuffST?.RemainingTime <= 3) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel))
                return OriginalHook(Thunder2);

            if (IsEnabled(CustomComboPreset.BLM_AoE_ParadoxFiller) &&
                ActiveParadox && EndOfIcePhaseAoEMaxLevel)
                return OriginalHook(Paradox);

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
                    ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Blizzard2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose))
                    return Transpose;
            }

            if (IcePhase)
            {
                if (CurMp == MP.MaxMP || HasMaxUmbralHeartStacks)
                {
                    if (IsNotEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        ActionReady(Fire2) && (TraitLevelChecked(Traits.AspectMasteryIII) || !TraitLevelChecked(Traits.UmbralHeart)))
                        return OriginalHook(Fire2);

                    if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        ActionReady(Transpose))
                        return Transpose;
                }

                if (ActionReady(Freeze))
                    return IsEnabled(CustomComboPreset.BLM_AoE_Blizzard4Sub) &&
                           LevelChecked(Blizzard4) && HasBattleTarget() && NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                        ? Blizzard4
                        : Freeze;

                if (!LevelChecked(Freeze) && ActionReady(Blizzard2))
                    return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }

    internal class BLM_Variant_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Variant_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && Variant.CanRaise(CustomComboPreset.BLM_Variant_Raise)
                ? Variant.Raise
                : actionID;
    }

    internal class BLM_ScatheXeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ScatheXeno;

        protected override uint Invoke(uint actionID) =>
            actionID is Scathe && LevelChecked(Xenoglossy) && HasPolyglotStacks()
                ? Xenoglossy
                : actionID;
    }

    internal class BLM_Blizzard1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard1to3;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Blizzard when LevelChecked(Blizzard3) && !IcePhase:
                    return Blizzard3;

                case Freeze when !LevelChecked(Freeze):
                    return Blizzard2;

                default:
                    return actionID;
            }
        }
    }

    internal class BLM_Fire1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire1to3;

        protected override uint Invoke(uint actionID) =>
            actionID is Fire &&
            (LevelChecked(Fire3) && !FirePhase ||
             HasStatusEffect(Buffs.Firestarter))
                ? Fire3
                : actionID;
    }

    internal class BLM_Between_The_LeyLines : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Between_The_LeyLines;

        protected override uint Invoke(uint actionID) =>
            actionID is LeyLines && HasStatusEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_Aetherial_Manipulation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID) =>
            actionID is AetherialManipulation && ActionReady(BetweenTheLines) &&
            HasStatusEffect(Buffs.LeyLines) && !HasStatusEffect(Buffs.CircleOfPower) && !IsMoving()
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_UmbralSoul : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_UmbralSoul;

        protected override uint Invoke(uint actionID) =>
            actionID is Transpose && IcePhase && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
    }

    internal class BLM_TriplecastProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_TriplecastProtection;

        protected override uint Invoke(uint actionID) =>
            actionID is Triplecast && HasStatusEffect(Buffs.Triplecast) && LevelChecked(Triplecast)
                ? All.SavageBlade
                : actionID;
    }

    internal class BLM_FireandIce : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_FireandIce;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Fire4 when FirePhase && LevelChecked(Fire4):
                    return Fire4;

                case Fire4 when IcePhase && LevelChecked(Blizzard4):
                    return Blizzard4;

                case Flare when FirePhase && LevelChecked(Flare):
                    return Flare;

                case Flare when IcePhase && LevelChecked(Freeze):
                    return Freeze;

                default:
                    return actionID;
            }
        }
    }

    internal class BLM_FireFlarestar : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_FireFlarestar;

        protected override uint Invoke(uint actionID) =>
            actionID is Fire4 && FirePhase && FlarestarReady && LevelChecked(FlareStar) ||
            actionID is Flare && FirePhase && FlarestarReady && LevelChecked(FlareStar)
                ? FlareStar
                : actionID;
    }

    internal class BLM_Fire4to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire4to3;
        protected override uint Invoke(uint actionID) =>
            actionID is Fire4 && !(FirePhase && LevelChecked(Fire4))
                ? Fire3
                : actionID;
    }

    internal class BLM_Blizzard4toDespair : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard4toDespair;
        protected override uint Invoke(uint actionID) =>
            actionID is Blizzard4 && FirePhase && LevelChecked(Despair)
                ? Despair
                : actionID;
    }

    internal class BLM_AmplifierXeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AmplifierXeno;
        protected override uint Invoke(uint actionID) =>
            actionID is Amplifier && HasMaxPolyglotStacks
                ? Xenoglossy
                : actionID;
    }
}
