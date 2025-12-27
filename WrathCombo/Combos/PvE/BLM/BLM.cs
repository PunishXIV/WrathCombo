using System;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.BLM.Config;
namespace WrathCombo.Combos.PvE;

internal partial class BLM : Caster
{
    internal class BLM_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (ActionReady(Amplifier) && !HasMaxPolyglotStacks)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > 1 &&
                    !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(2.5f))
                    return LeyLines;

                if (EndOfFirePhase)
                {
                    if (ActionReady(Manafont))
                        return Manafont;

                    if (ActionReady(Role.Swiftcast) && JustUsed(Despair) &&
                        !ActionReady(Manafont) && !HasStatusEffect(Buffs.Triplecast) &&
                        InActionRange(Fire) && HasBattleTarget())
                        return Role.Swiftcast;

                    if (ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                        !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                        InActionRange(Fire) && HasBattleTarget() && !HasStatusEffect(Buffs.LeyLines) &&
                        JustUsed(Despair) && !ActionReady(Manafont))
                        return Triplecast;

                    if (ActionReady(Transpose) &&
                        (HasStatusEffect(Role.Buffs.Swiftcast) ||
                         HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (IcePhase)
                {
                    if (MP.Full && JustUsed(Paradox) &&
                        ActionReady(Transpose))
                        return Transpose;

                    if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
                        ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                        HasBattleTarget() && InActionRange(Fire))
                        return Role.Swiftcast;
                }

                if (ActionReady(Manaward) &&
                    PlayerHealthPercentageHp() < 40 && RaidWideCasting())
                    return Manaward;
            }

            if (IsMoving() && !LevelChecked(Triplecast) &&
                ActionReady(Scathe))
                return Scathe;

            //Overcap protection
            if (HasMaxPolyglotStacks && PolyglotTimer <= 5)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (CanUseThunder())
                return OriginalHook(Thunder);

            if (LevelChecked(Amplifier) &&
                GetCooldownRemainingTime(Amplifier) < 5 &&
                HasMaxPolyglotStacks)
                return Xenoglossy;

            if (IsMoving() && InCombat() &&
                HasBattleTarget() && InActionRange(Fire))
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
                if (HasPolyglotStacks())
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && AstralFireStacks < 3 ||
                    !ActionReady(Fire4) && TimeSinceFirestarterBuff >= 2 && LevelChecked(Fire3))
                    return Fire3;

                if (ActiveParadox &&
                    MP.Cur > 1600 &&
                    (AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return Paradox;

                if (CanFlarestar)
                    return FlareStar;

                if (ActionReady(FireSpam) &&
                    (LevelChecked(Despair) && MP.Cur - MP.FireI >= 800 ||
                     !LevelChecked(Despair)))
                    return FireSpam;

                if (ActionReady(Despair))
                    return Despair;

                if (ActionReady(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) &&
                    !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (ActionReady(Transpose) &&
                    !LevelChecked(Fire3) &&
                    MP.Cur < MP.FireI)
                    return Transpose;
            }

            if (IcePhase)
            {
                if (UmbralHearts is 3 &&
                    UmbralIceStacks is 3 &&
                    ActiveParadox)
                    return Paradox;

                if (MP.Full || JustUsed(Blizzard4))
                {
                    if (LevelChecked(Fire3))
                        return Fire3;

                    if (ActionReady(Transpose) &&
                        !LevelChecked(Fire3))
                        return Transpose;
                }

                if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
                    (HasStatusEffect(Role.Buffs.Swiftcast) ||
                     HasStatusEffect(Buffs.Triplecast) ||
                     JustUsed(Freeze, 10f)))
                    return Blizzard3;

                if (ActionReady(BlizzardSpam))
                    return BlizzardSpam;
            }

            if (ActionReady(Blizzard3))
                return MP.Cur < 7500
                    ? Blizzard3
                    : Fire3;

            return actionID;
        }
    }

    internal class BLM_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsMoving() && InCombat() && InActionRange(Fire2) && HasBattleTarget() &&
                    ActionReady(Triplecast) && !HasStatusEffect(Buffs.Triplecast))
                    return Triplecast;

                if (ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (ActionReady(Transpose) &&
                    (EndOfFirePhase || EndOfIcePhaseAoE))
                    return Transpose;

                if (ActionReady(Amplifier) && PolyglotTimer >= 20)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(BLM_AoE_LeyLinesTimeStill) &&
                    GetTargetHPPercent() > 40)
                    return LeyLines;
            }

            if ((EndOfFirePhase || EndOfIcePhaseAoE) &&
                HasPolyglotStacks() && ActionReady(Foul))
                return Foul;

            if (LevelChecked(OriginalHook(Thunder2)) && HasStatusEffect(Buffs.Thunderhead) &&
                CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder2)]) &&
                (ThunderDebuffAoE is null && ThunderDebuffST is null ||
                 ThunderDebuffAoE?.RemainingTime <= 3 ||
                 ThunderDebuffST?.RemainingTime <= 3))
                return OriginalHook(Thunder2);

            if (ActiveParadox && EndOfIcePhaseAoE)
                return Paradox;

            if (FirePhase)
            {
                if (CanFlarestar)
                    return FlareStar;

                if (ActionReady(Fire2) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Fire2);

                if (!HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    HasBattleTarget() && InActionRange(Fire2) &&
                    HasMaxUmbralHeartStacks && !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (ActionReady(Transpose) && MP.Cur < MP.FireAoE)
                    return Transpose;
            }

            if (IcePhase)
            {
                if ((HasMaxUmbralHeartStacks ||
                     MP.Full && !ActionReady(Flare) ||
                     MP.Cur >= 5000 && ActionReady(Flare)) &&
                    ActionReady(Transpose))
                    return Transpose;

                if (ActionReady(Freeze))
                    return ActionReady(Blizzard4) && HasBattleTarget() &&
                           NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                        ? Blizzard4
                        : Freeze;

                if (!ActionReady(Freeze) && LevelChecked(Blizzard2))
                    return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            // Opener
            if (IsEnabled(Preset.BLM_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && !HasMaxPolyglotStacks)
                    return Amplifier;

                if (IsEnabled(Preset.BLM_ST_LeyLines) &&
                    ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > BLM_ST_LeyLinesCharges &&
                    (BLM_ST_LeyLinesMovement == 1 ||
                     BLM_ST_LeyLinesMovement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(BLM_ST_LeyLinesTimeStill)) &&
                    GetTargetHPPercent() > HPThresholdLeylines)
                    return LeyLines;

                if (EndOfFirePhase)
                {
                    if (IsEnabled(Preset.BLM_ST_Manafont) &&
                        ActionReady(Manafont))
                        return Manafont;

                    if (IsEnabled(Preset.BLM_ST_Swiftcast) &&
                        ActionReady(Role.Swiftcast) && JustUsed(Despair) &&
                        HasBattleTarget() && InActionRange(Fire) &&
                        !ActionReady(Manafont) &&
                        !HasStatusEffect(Buffs.Triplecast))
                        return Role.Swiftcast;

                    if (IsEnabled(Preset.BLM_ST_Triplecast) &&
                        ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                        HasBattleTarget() && InActionRange(Fire) &&
                        !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                        (BLM_ST_Triplecast_WhenToUse == 0 || !HasStatusEffect(Buffs.LeyLines)) &&
                        (BLM_ST_MovementOption[0] && GetRemainingCharges(Triplecast) > BLM_ST_Triplecast_MovementCharges ||
                         !BLM_ST_MovementOption[0]) && JustUsed(Despair) && !ActionReady(Manafont))
                        return Triplecast;

                    if (IsEnabled(Preset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) &&
                        (HasStatusEffect(Role.Buffs.Swiftcast) ||
                         HasStatusEffect(Buffs.Triplecast)))
                        return Transpose;
                }

                if (IcePhase)
                {
                    if (IsEnabled(Preset.BLM_ST_Transpose) &&
                        MP.Full && JustUsed(Paradox) &&
                        ActionReady(Transpose))
                        return Transpose;

                    if (ActionReady(Blizzard3) && UmbralIceStacks < 3)
                    {
                        if (IsEnabled(Preset.BLM_ST_Swiftcast) &&
                            ActionReady(Role.Swiftcast) &&
                            !HasStatusEffect(Buffs.Triplecast) &&
                            HasBattleTarget() && InActionRange(Fire))
                            return Role.Swiftcast;

                        if (IsEnabled(Preset.BLM_ST_Triplecast) &&
                            ActionReady(Triplecast) && IsOnCooldown(Role.Swiftcast) &&
                            HasBattleTarget() && InActionRange(Fire) &&
                            !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast) &&
                            (BLM_ST_Triplecast_WhenToUse == 0 || !HasStatusEffect(Buffs.LeyLines)) &&
                            (BLM_ST_MovementOption[0] && GetRemainingCharges(Triplecast) > BLM_ST_Triplecast_MovementCharges ||
                             !BLM_ST_MovementOption[0]) && JustUsed(Despair) && !ActionReady(Manafont))
                            return Triplecast;
                    }
                }

                if (IsEnabled(Preset.BLM_ST_Manaward) &&
                    ActionReady(Manaward) &&
                    PlayerHealthPercentageHp() < BLM_ST_Manaward_Threshold && RaidWideCasting())
                    return Manaward;

                if (IsEnabled(Preset.BLM_ST_Addle) &&
                    Role.CanAddle() && RaidWideCasting())
                    return Role.Addle;
            }

            //Overcap protection
            if (IsEnabled(Preset.BLM_ST_UsePolyglot) &&
                HasMaxPolyglotStacks && PolyglotTimer <= 5)
                return LevelChecked(Xenoglossy)
                    ? Xenoglossy
                    : Foul;

            if (IsEnabled(Preset.BLM_ST_Thunder) &&
                CanUseThunder())
                return OriginalHook(Thunder);

            if (IsEnabled(Preset.BLM_ST_Amplifier) &&
                IsEnabled(Preset.BLM_ST_UsePolyglot) &&
                LevelChecked(Amplifier) &&
                GetCooldownRemainingTime(Amplifier) < 5 &&
                HasMaxPolyglotStacks)
                return Xenoglossy;

            if (IsMoving() && InCombat() &&
                HasBattleTarget() && InActionRange(Fire))
            {
                foreach(int priority in BLM_ST_Movement_Priority.OrderBy(x => x))
                {
                    int index = BLM_ST_Movement_Priority.IndexOf(priority);
                    if (CheckMovementConfigMeetsRequirements(index, out uint action))
                        return action;
                }
            }

            if (FirePhase)
            {
                // TODO: Revisit when Raid Buff checks are in place
                if (IsEnabled(Preset.BLM_ST_UsePolyglot) &&
                    (BLM_ST_MovementOption[3] &&
                     PolyglotStacks > BLM_ST_Polyglot_Movement &&
                     PolyglotStacks > BLM_ST_Polyglot_Save ||
                     !BLM_ST_MovementOption[3] &&
                     PolyglotStacks > BLM_ST_Polyglot_Save))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;

                if ((LevelChecked(Paradox) && HasStatusEffect(Buffs.Firestarter) ||
                     TimeSinceFirestarterBuff >= 2) && AstralFireStacks < 3 ||
                    !ActionReady(Fire4) && TimeSinceFirestarterBuff >= 2 && LevelChecked(Fire3))
                    return Fire3;

                if (ActiveParadox &&
                    MP.Cur > 1600 &&
                    (AstralFireStacks < 3 ||
                     JustUsed(FlareStar, 5) ||
                     !LevelChecked(FlareStar) && ActionReady(Despair)))
                    return Paradox;

                if (IsEnabled(Preset.BLM_ST_FlareStar) &&
                    CanFlarestar)
                    return FlareStar;

                if (ActionReady(FireSpam) &&
                    (LevelChecked(Despair) && MP.Cur - MP.FireI >= 800 ||
                     !LevelChecked(Despair)))
                    return FireSpam;

                if (IsEnabled(Preset.BLM_ST_Despair) &&
                    ActionReady(Despair))
                    return Despair;

                if (ActionReady(Blizzard3) &&
                    !HasStatusEffect(Role.Buffs.Swiftcast) && !HasStatusEffect(Buffs.Triplecast))
                    return Blizzard3;

                if (IsEnabled(Preset.BLM_ST_Transpose) &&
                    ActionReady(Transpose) &&
                    !LevelChecked(Fire3) &&
                    MP.Cur < MP.FireI)
                    return Transpose;
            }

            if (IcePhase)
            {
                if (UmbralHearts is 3 &&
                    UmbralIceStacks is 3 &&
                    ActiveParadox)
                    return Paradox;

                if (MP.Full || JustUsed(Blizzard4))
                {
                    if (LevelChecked(Fire3))
                        return Fire3;

                    if (IsEnabled(Preset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) &&
                        !ActionReady(Blizzard3))
                        return Transpose;
                }

                if (ActionReady(Blizzard3) && UmbralIceStacks < 3 &&
                    (HasStatusEffect(Role.Buffs.Swiftcast) ||
                     HasStatusEffect(Buffs.Triplecast) ||
                     JustUsed(Freeze, 10f)))
                    return Blizzard3;

                if (ActionReady(BlizzardSpam))
                    return BlizzardSpam;
            }

            if (ActionReady(Blizzard3))
                return MP.Cur < 7500
                    ? Blizzard3
                    : Fire3;

            return actionID;
        }
    }

    internal class BLM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.BLM_AoE_Movement) &&
                    IsMoving() && InCombat() &&
                    HasBattleTarget() && InActionRange(Fire2) &&
                    ActionReady(Triplecast) && !HasStatusEffect(Buffs.Triplecast))
                    return Triplecast;

                if (IsEnabled(Preset.BLM_AoE_Manafont) &&
                    ActionReady(Manafont) &&
                    EndOfFirePhase)
                    return Manafont;

                if (IsEnabled(Preset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) &&
                    (EndOfFirePhase || EndOfIcePhaseAoE))
                    return Transpose;

                if (IsEnabled(Preset.BLM_AoE_Amplifier) &&
                    ActionReady(Amplifier) && PolyglotTimer >= 20)
                    return Amplifier;

                if (IsEnabled(Preset.BLM_AoE_LeyLines) &&
                    ActionReady(LeyLines) && !HasStatusEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > BLM_AoE_LeyLinesCharges &&
                    (BLM_AoE_LeyLinesMovement == 1 ||
                     BLM_AoE_LeyLinesMovement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(BLM_AoE_LeyLinesTimeStill)) &&
                    GetTargetHPPercent() > BLM_AoE_LeyLinesOption)
                    return LeyLines;
            }

            if (IsEnabled(Preset.BLM_AoE_UsePolyglot) &&
                (EndOfFirePhase || EndOfIcePhaseAoE) &&
                HasPolyglotStacks() && ActionReady(Foul))
                return Foul;

            if (IsEnabled(Preset.BLM_AoE_Thunder) &&
                ActionReady(OriginalHook(Thunder2)) && HasStatusEffect(Buffs.Thunderhead) &&
                CanApplyStatus(CurrentTarget, ThunderList[OriginalHook(Thunder2)]) &&
                GetTargetHPPercent() > BLM_AoE_ThunderHP &&
                (ThunderDebuffAoE is null && ThunderDebuffST is null ||
                 ThunderDebuffAoE?.RemainingTime <= 3 ||
                 ThunderDebuffST?.RemainingTime <= 3))
                return OriginalHook(Thunder2);

            if (IsEnabled(Preset.BLM_AoE_ParadoxFiller) &&
                ActiveParadox && EndOfIcePhaseAoE)
                return Paradox;

            if (FirePhase)
            {
                if (CanFlarestar)
                    return FlareStar;

                if (ActionReady(Fire2) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Fire2);

                if (IsEnabled(Preset.BLM_AoE_Triplecast) &&
                    !HasStatusEffect(Buffs.Triplecast) && ActionReady(Triplecast) &&
                    HasBattleTarget() && InActionRange(Fire2) &&
                    GetRemainingCharges(Triplecast) > BLM_AoE_Triplecast_HoldCharges && HasMaxUmbralHeartStacks &&
                    !ActionReady(Manafont))
                    return Triplecast;

                if (ActionReady(Flare))
                    return Flare;

                if (IsNotEnabled(Preset.BLM_AoE_Transpose) &&
                    LevelChecked(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII) && !TraitLevelChecked(Traits.UmbralHeart))
                    return OriginalHook(Blizzard2);

                if (IsEnabled(Preset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && MP.Cur < MP.FireAoE)
                    return Transpose;
            }

            if (IcePhase)
            {
                if (HasMaxUmbralHeartStacks ||
                    MP.Full && !ActionReady(Flare) ||
                    MP.Cur >= 5000 && ActionReady(Flare))
                {
                    if (IsNotEnabled(Preset.BLM_AoE_Transpose) &&
                        LevelChecked(Fire2) && TraitLevelChecked(Traits.AspectMasteryIII))
                        return OriginalHook(Fire2);

                    if (IsEnabled(Preset.BLM_AoE_Transpose) &&
                        ActionReady(Transpose))
                        return Transpose;
                }

                if (ActionReady(Freeze))
                    return IsEnabled(Preset.BLM_AoE_Blizzard4Sub) &&
                           ActionReady(Blizzard4) && HasBattleTarget() &&
                           NumberOfEnemiesInRange(Freeze, CurrentTarget) == 2
                        ? Blizzard4
                        : Freeze;

                if (!ActionReady(Freeze) && LevelChecked(Blizzard2))
                    return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }

    internal class BLM_Retargetting_Aetherial_Manipulation : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Retargetting_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AetherialManipulation)
                return actionID;

            return BLM_AM_FieldMouseover
                ? AetherialManipulation.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.ModelMouseOverTarget ?? SimpleTarget.HardTarget)
                : AetherialManipulation.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.HardTarget);
        }
    }

    internal class BLM_TriplecastProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_TriplecastProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Triplecast)
                return actionID;

            return HasStatusEffect(Buffs.Triplecast) && LevelChecked(Triplecast)
                ? All.SavageBlade
                : actionID;
        }
    }

    internal class BLM_Fire1and3 : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Fire1and3;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Fire or Fire3))
                return actionID;

            return actionID switch
            {
                Fire when BLM_F1to3 == 0 && BLM_Fire1_Despair && FirePhase && MP.Cur < 2400 && LevelChecked(Despair) => Despair,

                Fire when BLM_F1to3 == 0 && LevelChecked(Fire3) &&
                          (AstralFireStacks is 1 or 2 && HasStatusEffect(Buffs.Firestarter) ||
                           LevelChecked(Paradox) && !ActiveParadox ||
                           !InCombat() && LevelChecked(Fire4) ||
                           IcePhase && !ActiveParadox ||
                           !LevelChecked(Fire4) &&
                           HasStatusEffect(Buffs.Firestarter)) && !JustUsed(Fire3) => Fire3,

                Fire3 when BLM_F1to3 == 1 && LevelChecked(Fire3) && FirePhase &&
                           (LevelChecked(Paradox) && ActiveParadox && AstralFireStacks is 3 ||
                            !LevelChecked(Fire4) && !HasStatusEffect(Buffs.Firestarter)) &&
                           !JustUsed(OriginalHook(Fire)) => OriginalHook(Fire),

                var _ => actionID
            };
        }
    }

    internal class BLM_Fire4 : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Fire4;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire4)
                return actionID;

            if (!InCombat())
            {
                return BLM_Fire4_Fire3
                    ? LevelChecked(Fire3)
                        ? Fire3
                        : Fire
                    : actionID;
            }

            return IcePhase switch
            {
                false when BLM_Fire4_FlareStar && CanFlarestar && LevelChecked(FlareStar) => FlareStar,
                false when BLM_Fire4_Fire3 && AstralFireStacks < 3 => LevelChecked(Fire3) ? Fire3 : Fire,
                false => actionID,

                //Ice Phase
                true when BLM_Fire4_FireAndIce == 0 && UmbralIceStacks < 3 => LevelChecked(Blizzard3) ? Blizzard3 : Blizzard,
                true when BLM_Fire4_FireAndIce == 0 && UmbralIceStacks == 3 && LevelChecked(Blizzard4) => Blizzard4,
                true when BLM_Fire4_FireAndIce == 1 => BLM_Fire4_Fire3 && LevelChecked(Fire3) ? Fire3 : Fire,
                true => actionID
            };
        }
    }

    internal class BLM_Flare : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Flare;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Flare)
                return actionID;

            return actionID switch
            {
                Flare when BLM_Flare_FlareStar && FirePhase && CanFlarestar => FlareStar,
                Flare when FirePhase && LevelChecked(Flare) => Flare,
                Flare when IcePhase && ActionReady(Freeze) => Freeze,
                var _ => actionID
            };
        }
    }

    internal class BLM_Blizzard1and3 : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Blizzard1and3;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard or Blizzard3))
                return actionID;

            return actionID switch
            {
                Blizzard when BLM_B1to3 == 0 && LevelChecked(Blizzard3) &&
                              (FirePhase ||
                               UmbralIceStacks is 1 ||
                               UmbralIceStacks is 2) => Blizzard3,

                Blizzard3 when BLM_B1to3 == 1 && LevelChecked(Blizzard3) && IcePhase && UmbralIceStacks is 3 => OriginalHook(Blizzard),
                Blizzard3 when BLM_Blizzard3_Despair && FirePhase && LevelChecked(Despair) && MP.Cur >= 800 => Despair,

                var _ => actionID
            };
        }
    }

    internal class BLM_Blizzard4toDespair : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Blizzard4toDespair;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Blizzard4)
                return actionID;

            return FirePhase && LevelChecked(Despair) && MP.Cur >= 800
                ? Despair
                : actionID;
        }
    }

    internal class BLM_Freeze : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Freeze;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Freeze)
                return actionID;

            return actionID switch
            {
                Freeze when HasMaxUmbralHeartStacks && LevelChecked(Paradox) && ActiveParadox && IcePhase => OriginalHook(Blizzard),
                Freeze when !LevelChecked(Freeze) => Blizzard2,
                var _ => actionID
            };
        }
    }

    internal class BLM_FlareParadox : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_FlareParadox;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FlareStar)
                return actionID;

            return FirePhase && LevelChecked(FlareStar) && ActiveParadox && AstralSoulStacks < 6
                ? OriginalHook(Fire)
                : actionID;
        }
    }

    internal class BLM_AmplifierXeno : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_AmplifierXeno;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Amplifier)
                return actionID;

            return BLM_AmplifierXenoCD && IsOnCooldown(Amplifier) && HasPolyglotStacks() || HasMaxPolyglotStacks
                ? Xenoglossy
                : actionID;
        }
    }

    internal class BLM_XenoThunder : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_XenoThunder;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Xenoglossy)
                return actionID;

            return ThunderDebuffST is null && ThunderDebuffAoE is null ||
                   ThunderDebuffST?.RemainingTime < 3
                ? OriginalHook(Thunder)
                : actionID;
        }
    }

    internal class BLM_FoulThunder : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_FoulThunder;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Foul)
                return actionID;

            return ThunderDebuffST is null && ThunderDebuffAoE is null ||
                   ThunderDebuffAoE?.RemainingTime < 3
                ? OriginalHook(Thunder2)
                : actionID;
        }
    }

    internal class BLM_UmbralSoul : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_UmbralSoul;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Transpose)
                return actionID;

            return IcePhase && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
        }
    }

    internal class BLM_ScatheXeno : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_ScatheXeno;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Scathe)
                return actionID;

            return LevelChecked(Xenoglossy) && HasPolyglotStacks()
                ? Xenoglossy
                : actionID;
        }
    }

    internal class BLM_Between_The_LeyLines : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Between_The_LeyLines;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not LeyLines)
                return actionID;

            return HasStatusEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
        }
    }

    internal class BLM_Aetherial_Manipulation : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AetherialManipulation)
                return actionID;

            return ActionReady(BetweenTheLines) &&
                   HasStatusEffect(Buffs.LeyLines) && !HasStatusEffect(Buffs.CircleOfPower) && !IsMoving()
                ? BetweenTheLines
                : actionID;
        }
    }
}
