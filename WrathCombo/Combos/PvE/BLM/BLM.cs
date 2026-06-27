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
            if (actionID is not Blizzard)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (CanStAmplifierWeave())
                    return Amplifier;

                if (CanStLeyLinesWeave(allowMoving: false, timeStillSeconds: 2.5))
                    return LeyLines;

                if (TryEndOfFireWeave(fallbackWhenNoTranspose: Blizzard) is var endOfFireWeave and not 0)
                    return endOfFireWeave;

                if (TryIceWeave() is var iceWeave and not 0)
                    return iceWeave;

                if (CanStManaward(true, simpleLogic: true))
                    return Manaward;

                if (CanStAddleWeave())
                    return Role.Addle;
            }

            if (CanStScatheFiller())
                return Scathe;

            if (TryStPolyglotOvercap() is var polyglotOvercap and not 0)
                return polyglotOvercap;

            if (TryStThunder() is var thunder and not 0)
                return thunder;

            if (TryStAmplifierXeno() is var amplifierXeno and not 0)
                return amplifierXeno;

            if (TryStMovementGcd() is var movementGcd and not 0)
                return movementGcd;

            if (FirePhase)
            {
                uint gcd = UseFirePhaseGcd();
                if (gcd != 0)
                    return gcd;
            }

            if (IcePhase)
            {
                uint gcd = UseIcePhaseGcd();
                if (gcd != 0)
                    return gcd;
            }

            if (UseOutOfPhaseGcd() is var outOfPhase and not 0)
                return outOfPhase;

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
                if (TryAoEMovementTriplecast() is var movementTriplecast and not 0)
                    return movementTriplecast;

                if (CanAoEManafontWeave())
                    return Manafont;

                if (CanAoETransposeWeave())
                    return Transpose;

                if (CanAoEAmplifierWeave())
                    return Amplifier;

                if (CanAoELeyLinesWeave(
                    allowMoving: false,
                    timeStillSeconds: 2.5,
                    hpThreshold: 40))
                    return LeyLines;
            }

            if (TryAoEPolyglotOvercap() is var polyglotOvercap and not 0)
                return polyglotOvercap;

            if (TryAoEPolyglot() is var polyglot and not 0)
                return polyglot;

            if (TryAoEThunder() is var thunder and not 0)
                return thunder;

            if (TryAoEParadoxFiller() is var paradox and not 0)
                return paradox;

            if (FirePhase && UseAoEFirePhaseGcd() is var fireGcd and not 0)
                return fireGcd;

            if (IcePhase && UseAoEIcePhaseGcd() is var iceGcd and not 0)
                return iceGcd;

            return actionID;
        }
    }

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Blizzard)
                return actionID;

            // Opener
            if (IsEnabled(Preset.BLM_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanStManaward(
                IsEnabled(Preset.BLM_ST_Manaward),
                BLM_ST_ManawardHPThreshold,
                false,
                BLM_ST_ManawardTrigger,
                BLM_ST_ManawardSolo))
                return Manaward;

            if (CanWeave())
            {
                if (CanStAmplifierWeave(IsEnabled(Preset.BLM_ST_Amplifier)))
                    return Amplifier;

                if (CanStLeyLinesWeave(
                    IsEnabled(Preset.BLM_ST_LeyLines),
                    BLM_ST_LeyLinesCharges,
                    BLM_ST_LeyLinesMovement == 1,
                    BLM_ST_LeyLinesTimeStill,
                    LeyLinesHPThreshold))
                    return LeyLines;

                if (TryEndOfFireWeave(
                    IsEnabled(Preset.BLM_ST_Manafont),
                    IsEnabled(Preset.BLM_ST_Swiftcast),
                    IsEnabled(Preset.BLM_ST_Triplecast),
                    BLM_ST_Triplecast_WhenToUse == 0,
                    true,
                    IsEnabled(Preset.BLM_ST_Transpose),
                    true,
                    Blizzard) is var endOfFireWeave and not 0)
                    return endOfFireWeave;

                if (TryIceWeave(
                    IsEnabled(Preset.BLM_ST_Transpose),
                    IsEnabled(Preset.BLM_ST_Swiftcast),
                    IsEnabled(Preset.BLM_ST_Triplecast),
                    BLM_ST_Triplecast_WhenToUse == 0,
                    true) is var iceWeave and not 0)
                    return iceWeave;

                if (CanStAddleWeave(IsEnabled(Preset.BLM_ST_Addle)))
                    return Role.Addle;
            }

            if (TryStPolyglotOvercap(IsEnabled(Preset.BLM_ST_UsePolyglot)) is var polyglotOvercap and not 0)
                return polyglotOvercap;

            if (TryStThunder(
                IsEnabled(Preset.BLM_ST_Thunder),
                ThunderHPThreshold(),
                BLM_ST_ThunderRefresh) is var thunder and not 0)
                return thunder;

            if (TryStAmplifierXeno(
                IsEnabled(Preset.BLM_ST_Amplifier),
                IsEnabled(Preset.BLM_ST_UsePolyglot)) is var amplifierXeno and not 0)
                return amplifierXeno;

            if (IsEnabled(Preset.BLM_ST_Movement) &&
                TryStMovementGcd(useConfiguredPriority: true) is var movementGcd and not 0)
                return movementGcd;

            if (FirePhase)
            {
                uint gcd = UseFirePhaseGcd(
                    IsEnabled(Preset.BLM_ST_FlareStar),
                    IsEnabled(Preset.BLM_ST_Despair),
                    IsEnabled(Preset.BLM_ST_Transpose),
                    IsEnabled(Preset.BLM_ST_UsePolyglot),
                    false,
                    BLM_ST_MovementOption[MovementSwiftcast],
                    BLM_ST_PolyglotMovement,
                    BLM_ST_PolyglotSaveUsage);
                if (gcd != 0)
                    return gcd;
            }

            if (IcePhase)
            {
                uint gcd = UseIcePhaseGcd(useTranspose: IsEnabled(Preset.BLM_ST_Transpose));
                if (gcd != 0)
                    return gcd;
            }

            if (UseOutOfPhaseGcd() is var outOfPhase and not 0)
                return outOfPhase;

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
                if (TryAoEMovementTriplecast(IsEnabled(Preset.BLM_AoE_Movement)) is var movementTriplecast and not 0)
                    return movementTriplecast;

                if (CanAoEManafontWeave(IsEnabled(Preset.BLM_AoE_Manafont)))
                    return Manafont;

                if (CanAoETransposeWeave(IsEnabled(Preset.BLM_AoE_Transpose)))
                    return Transpose;

                if (CanAoEAmplifierWeave(IsEnabled(Preset.BLM_AoE_Amplifier)))
                    return Amplifier;

                if (CanAoELeyLinesWeave(
                    IsEnabled(Preset.BLM_AoE_LeyLines),
                    BLM_AoE_LeyLinesCharges,
                    BLM_AoE_LeyLinesMovement == 1,
                    BLM_AoE_LeyLinesTimeStill,
                    BLM_AoE_LeyLinesOption))
                    return LeyLines;
            }

            if (TryAoEPolyglotOvercap(IsEnabled(Preset.BLM_AoE_UsePolyglot)) is var polyglotOvercap and not 0)
                return polyglotOvercap;

            if (TryAoEPolyglot(IsEnabled(Preset.BLM_AoE_UsePolyglot)) is var polyglot and not 0)
                return polyglot;

            if (TryAoEThunder(IsEnabled(Preset.BLM_AoE_Thunder), BLM_AoE_ThunderHP) is var thunder and not 0)
                return thunder;

            if (TryAoEParadoxFiller(IsEnabled(Preset.BLM_AoE_ParadoxFiller)) is var paradox and not 0)
                return paradox;

            if (FirePhase &&
                UseAoEFirePhaseGcd(
                    IsEnabled(Preset.BLM_AoE_Triplecast),
                    BLM_AoE_TriplecastHoldCharges,
                    IsEnabled(Preset.BLM_AoE_Transpose),
                    IsNotEnabled(Preset.BLM_AoE_Transpose)) is var fireGcd and not 0)
                return fireGcd;

            if (IcePhase &&
                UseAoEIcePhaseGcd(
                    IsEnabled(Preset.BLM_AoE_Transpose),
                    IsNotEnabled(Preset.BLM_AoE_Transpose),
                    IsEnabled(Preset.BLM_AoE_Blizzard4Sub),
                    true) is var iceGcd and not 0)
                return iceGcd;

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

    internal class BLM_F1toF4 : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_F1toF4;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            return ActiveParadox && IcePhase
                ? OriginalHook(Blizzard)
                : ActionReady(Fire4)
                    ? Fire4
                    : actionID;
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
                false when BLM_Fire4_FlareStar && CanFlareStar() && LevelChecked(FlareStar) => FlareStar,
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
                Flare when BLM_Flare_FlareStar && FirePhase && CanFlareStar() => FlareStar,
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

    internal class BLM_B1toB4 : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_B1toB4;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Blizzard)
                return actionID;

            return ActiveParadox && FirePhase
                ? OriginalHook(Fire)
                : ActionReady(Blizzard4)
                    ? Blizzard4
                    : actionID;
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

            return BLM_AmplifierXenoCD && IsOnCooldown(Amplifier) && HasPolyglot || HasMaxPolyglotStacks
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

            return LevelChecked(Xenoglossy) && HasPolyglot
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
