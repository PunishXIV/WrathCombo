using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
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

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, Config.BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (CanWeave())
            {
                if (ActionReady(Amplifier) && Gauge.EnochianTimer >= 20000 &&
                    !HasMaxPolyglotStacks)
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

                if (Gauge.InUmbralIce)
                {
                    if (JustUsed(Paradox) && CurMp is MP.MaxMP)
                        return Transpose;

                    if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3)
                    {
                        if (ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                            return Role.Swiftcast;

                        if (ActionReady(Triplecast) &&
                            !HasStatusEffect(Buffs.Triplecast) &&
                            !HasStatusEffect(Role.Buffs.Swiftcast))
                            return Triplecast;
                    }
                }
            }

            if (HasMaxPolyglotStacks && Gauge.EnochianTimer <= 5000)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (HasStatusEffect(Buffs.Thunderhead) && LevelChecked(Thunder) &&
                (ThunderDebuffST is null || ThunderDebuffST.RemainingTime <= 3))
                return OriginalHook(Thunder);

            if (IsMoving() && InCombat())
            {
                if (ActionReady(Triplecast) && !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.LeyLines))
                    return Triplecast;

                if (ActionReady(Paradox) &&
                    Gauge.InAstralFire && Gauge.IsParadoxActive &&
                    !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Paradox;

                if (ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Role.Swiftcast;

                if (HasPolyglotStacks() && !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Role.Buffs.Swiftcast))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (Gauge.InAstralFire)
            {
                // Revisit when Raid Buff checks are in place
                //if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                //    LevelChecked(Amplifier) && HasPolyglotStacks() &&
                //    (GetCooldownRemainingTime(Amplifier) < 3 || GetCooldownRemainingTime(Amplifier) > 100) ||
                //    !LevelChecked(Amplifier) && HasPolyglotStacks())
                //    return LevelChecked(Xenoglossy)
                //        ? Xenoglossy
                //        : Foul;

                if (Gauge.IsParadoxActive && !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    (Gauge.AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return Paradox;

                if (FlarestarReady)
                    return FlareStar;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && Gauge.AstralFireStacks < 3 ||
                    !Fire4.LevelChecked() && TimeSinceFirestarterBuff >= 2 && ActionReady(Fire3) &&
                    !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Fire3;

                if (ActionReady(FireSpam) && (LevelChecked(Despair) && CurMp - MP.FireI >= 800 || !LevelChecked(Despair)))
                    return FireSpam;

                if (ActionReady(Despair))
                    return Despair;

                if (ActionReady(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (ActionReady(Transpose))
                    return Transpose; //Level 4-34
            }

            if (Gauge.InUmbralIce)
            {
                if (Gauge.UmbralHearts is 3 &&
                    Gauge.UmbralIceStacks is 3 &&
                    Gauge.IsParadoxActive)
                    return Paradox;

                if (CurMp == MP.MaxMP)
                {
                    if (ActionReady(Fire3))
                        return Fire3; //35-100, pre-Paradox/scuffed starting combat

                    if (ActionReady(Transpose))
                        return Transpose; //Levels 4-34
                }

                if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3)
                {
                    if (HasStatusEffect(Buffs.Triplecast) || HasStatusEffect(Role.Buffs.Swiftcast))
                        return Blizzard3;

                    if (HasPolyglotStacks())
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;

                    if (Gauge.IsParadoxActive)
                        return Paradox;
                }

                if (ActionReady(BlizzardSpam))
                    return BlizzardSpam;
            }

            if (LevelChecked(Fire3))
            {
                return CurMp >= 7500
                    ? Fire3
                    : Blizzard3;
            }

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

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, Config.BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            // Opener
            if (IsEnabled(CustomComboPreset.BLM_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && Gauge.EnochianTimer >= 20000 &&
                    !HasMaxPolyglotStacks)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_LeyLines) &&
                    ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > Config.BLM_ST_LeyLinesCharges)
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
                        (GetRemainingCharges(Triplecast) is 2 && Config.BLM_ST_Triplecast_UseCharges == 1 ||
                         HasCharges(Triplecast) && Config.BLM_ST_Triplecast_UseCharges == 2) &&
                        JustUsed(Despair) && !ActionReady(Manafont) &&
                        !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Buffs.LeyLines))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) && (HasStatusEffect(Role.Buffs.Swiftcast) || HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (Gauge.InUmbralIce)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        JustUsed(Paradox) && CurMp is MP.MaxMP)
                        return Transpose;

                    if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                            ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                            return Role.Swiftcast;

                        if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            ActionReady(Triplecast) &&
                            !HasStatusEffect(Buffs.Triplecast) &&
                            !HasStatusEffect(Role.Buffs.Swiftcast))
                            return Triplecast;
                    }
                }
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                HasMaxPolyglotStacks && Gauge.EnochianTimer <= 5000)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                HasStatusEffect(Buffs.Thunderhead) && LevelChecked(Thunder) &&
                (Config.BLM_ST_Thunder_SubOption == 0 ||
                 Config.BLM_ST_Thunder_SubOption == 1 && InBossEncounter()) &&
                GetTargetHPPercent() > Config.BLM_ST_Thunder_Threshold &&
                (ThunderDebuffST is null || ThunderDebuffST.RemainingTime <= 3))
                return OriginalHook(Thunder);

            if (IsMoving() && InCombat())
            {
                if (Config.BLM_ST_MovementOption[0] &&
                    ActionReady(Triplecast) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    !HasStatusEffect(Buffs.LeyLines))
                    return Triplecast;

                if (Config.BLM_ST_MovementOption[1] &&
                    ActionReady(Paradox) &&
                    Gauge.InAstralFire && Gauge.IsParadoxActive &&
                    !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Paradox;

                if (Config.BLM_ST_MovementOption[2] &&
                    ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Role.Swiftcast;

                if (Config.BLM_ST_MovementOption[3] &&
                    HasPolyglotStacks() &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (Gauge.InAstralFire)
            {
                // Revisit when Raid Buff checks are in place
                //if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                //    LevelChecked(Amplifier) && HasPolyglotStacks() &&
                //    (GetCooldownRemainingTime(Amplifier) < 3 || GetCooldownRemainingTime(Amplifier) > 100) ||
                //    !LevelChecked(Amplifier) && HasPolyglotStacks())
                //    return LevelChecked(Xenoglossy)
                //        ? Xenoglossy
                //        : Foul;

                if (Gauge.IsParadoxActive && !HasStatusEffect(Buffs.Firestarter) &&
                    !HasStatusEffect(Buffs.Triplecast) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    (Gauge.AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return Paradox;

                if (IsEnabled(CustomComboPreset.BLM_ST_FlareStar) &&
                    FlarestarReady)
                    return FlareStar;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && Gauge.AstralFireStacks < 3 ||
                    !Fire4.LevelChecked() && TimeSinceFirestarterBuff >= 2 && ActionReady(Fire3) &&
                    !HasStatusEffect(Buffs.Triplecast) && !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Fire3;

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

            if (Gauge.InUmbralIce)
            {
                if (Gauge.UmbralHearts is 3 &&
                    Gauge.UmbralIceStacks is 3 &&
                    Gauge.IsParadoxActive)
                    return Paradox;

                if (CurMp == MP.MaxMP)
                {
                    if (ActionReady(Fire3))
                        return Fire3; //35-100, pre-Paradox/scuffed starting combat

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose))
                        return Transpose; //Levels 4-34
                }

                if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3)
                {
                    if (HasStatusEffect(Buffs.Triplecast) || HasStatusEffect(Role.Buffs.Swiftcast))
                        return Blizzard3;

                    if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                        HasPolyglotStacks())
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;

                    if (Gauge.IsParadoxActive)
                        return Paradox;
                }

                if (ActionReady(BlizzardSpam))
                    return BlizzardSpam;
            }

            if (LevelChecked(Fire3))
            {
                return CurMp >= 7500
                    ? Fire3
                    : Blizzard3;
            }

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

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, Config.BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (CanWeave())
            {
                if (ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (ActionReady(Transpose) && (EndOfFirePhase || EndOfIcePhaseAoEMaxLevel))
                    return Transpose;

                if (ActionReady(Amplifier) && RemainingPolyglotCD >= 20000)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > Config.BLM_AoE_LeyLinesCharges)
                    return LeyLines;
            }

            if ((EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel) &&
                HasPolyglotStacks())
                return Foul;

            if (HasStatusEffect(Buffs.Thunderhead) && LevelChecked(Thunder2) &&
                (ThunderDebuffAoE is null || ThunderDebuffAoE.RemainingTime <= 3) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel))
                return OriginalHook(Thunder2);

            if (Gauge.IsParadoxActive &&
                EndOfIcePhaseAoEMaxLevel)
                return OriginalHook(Paradox);

            if (Gauge.InAstralFire)
            {
                if (FlarestarReady)
                    return FlareStar;

                if (ActionReady(Fire2) &&
                    !TraitLevelChecked(Traits.EnhancedAstralFire) &&
                    (TraitLevelChecked(Traits.UmbralHeart) && Gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (!HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    HasMaxUmbralHeartStacks &&
                    !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Blizzard2);

                if (ActionReady(Transpose))
                    return Transpose;
            }

            if (Gauge.InUmbralIce)
            {
                if ((CurMp == MP.MaxMP || TraitLevelChecked(Traits.EnhancedAstralFire)) && HasMaxUmbralHeartStacks)
                {
                    if (ActionReady(Fire2) && TraitLevelChecked(Traits.AspectMasteryIII))
                        return OriginalHook(Fire2);

                    if (ActionReady(Transpose))
                        return Transpose;
                }

                if (ActionReady(Freeze) && (Gauge.UmbralIceStacks == 3 || TraitLevelChecked(Traits.EnhancedAstralFire)))
                {
                    if (HasBattleTarget() && NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2)
                        return Blizzard4;

                    return Freeze;
                }

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

            if (Variant.CanCure(CustomComboPreset.BLM_Variant_Cure, Config.BLM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BLM_Variant_Rampart))
                return Variant.Rampart;

            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_Manafont) &&
                    ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && (EndOfFirePhase || EndOfIcePhaseAoEMaxLevel))
                    return Transpose;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Amplifier) &&
                    ActionReady(Amplifier) && RemainingPolyglotCD >= 20000)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_AoE_LeyLines) &&
                    ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > Config.BLM_AoE_LeyLinesCharges)
                    return LeyLines;
            }

            if (IsEnabled(CustomComboPreset.BLM_AoE_UsePolyglot) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel) &&
                HasPolyglotStacks())
                return Foul;

            if (IsEnabled(CustomComboPreset.BLM_AoE_Thunder) &&
                HasStatusEffect(Buffs.Thunderhead) && LevelChecked(Thunder2) &&
                GetTargetHPPercent() > Config.BLM_AoE_ThunderHP &&
                (ThunderDebuffAoE is null || ThunderDebuffAoE.RemainingTime <= 3) &&
                (EndOfFirePhase || EndOfIcePhase || EndOfIcePhaseAoEMaxLevel))
                return OriginalHook(Thunder2);

            if (IsEnabled(CustomComboPreset.BLM_AoE_ParadoxFiller) &&
                Gauge.IsParadoxActive &&
                EndOfIcePhaseAoEMaxLevel)
                return OriginalHook(Paradox);

            if (Gauge.InAstralFire)
            {
                if (FlarestarReady)
                    return FlareStar;

                if (ActionReady(Fire2) &&
                    !TraitLevelChecked(Traits.EnhancedAstralFire) &&
                    (TraitLevelChecked(Traits.UmbralHeart) && Gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Triplecast) &&
                    !HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    GetRemainingCharges(Triplecast) > Config.BLM_AoE_Triplecast_HoldCharges && HasMaxUmbralHeartStacks &&
                    !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (IsNotEnabled(CustomComboPreset.BLM_AoE_Transpose) && ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Blizzard2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose))
                    return Transpose;
            }

            if (Gauge.InUmbralIce)
            {
                if ((CurMp == MP.MaxMP || TraitLevelChecked(Traits.EnhancedAstralFire)) && HasMaxUmbralHeartStacks)
                {
                    if (IsNotEnabled(CustomComboPreset.BLM_AoE_Transpose) && ActionReady(Fire2) && TraitLevelChecked(Traits.AspectMasteryIII))
                        return OriginalHook(Fire2);

                    if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        ActionReady(Transpose))
                        return Transpose;
                }

                if (ActionReady(Freeze) && (Gauge.UmbralIceStacks == 3 || TraitLevelChecked(Traits.EnhancedAstralFire)))
                {
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Blizzard4Sub) &&
                        HasBattleTarget() && NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2)
                        return Blizzard4;

                    return Freeze;
                }

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

    internal class BLM_Scathe_Xeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Scathe_Xeno;

        protected override uint Invoke(uint actionID) =>
            actionID is Scathe && LevelChecked(Xenoglossy) && HasPolyglotStacks()
                ? Xenoglossy
                : actionID;
    }

    internal class BLM_Blizzard_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_1to3;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Blizzard when LevelChecked(Blizzard3) && !Gauge.InUmbralIce:
                    return Blizzard3;

                case Freeze when !LevelChecked(Freeze):
                    return Blizzard2;

                default:
                    return actionID;
            }
        }
    }

    internal class BLM_Fire_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

        protected override uint Invoke(uint actionID) =>
            actionID is Fire &&
            (LevelChecked(Fire3) && !Gauge.InAstralFire ||
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
            actionID is Transpose && Gauge.InUmbralIce && LevelChecked(UmbralSoul)
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
                case Fire4 when Gauge.InAstralFire && LevelChecked(Fire4):
                    return Fire4;

                case Fire4 when Gauge.InUmbralIce && LevelChecked(Blizzard4):
                    return Blizzard4;

                case Flare when Gauge.InAstralFire && LevelChecked(Flare):
                    return Flare;

                case Flare when Gauge.InUmbralIce && LevelChecked(Freeze):
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
            actionID == Fire4 && Gauge.InAstralFire && FlarestarReady && LevelChecked(FlareStar) ||
            actionID == Flare && Gauge.InAstralFire && FlarestarReady && LevelChecked(FlareStar)
                ? FlareStar
                : actionID;
    }

    internal class BLM_Fire_4to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_4to3;
        protected override uint Invoke(uint actionID) =>
            actionID is Fire4 && !(Gauge.InAstralFire && LevelChecked(Fire4))
                ? Fire3
                : actionID;
    }

    internal class BLM_Blizzard_4toDespair : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_4toDespair;
        protected override uint Invoke(uint actionID) =>
            actionID is Blizzard4 && Gauge.InAstralFire && LevelChecked(Despair)
                ? Despair
                : actionID;
    }
}
