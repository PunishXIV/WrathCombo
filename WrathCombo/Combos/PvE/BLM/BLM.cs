using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.BLM.Config;
namespace WrathCombo.Combos.PvE;

internal partial class BLM : Caster
{
    #region Simple Mode

    internal class BLM_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Blizzard))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (UseManaward())
                return Manaward;

            if (CanWeave())
            {
                if (UseAmplifier())
                    return Amplifier;

                if (UseLeyLines(allowMoving: false, timeStillSeconds: 2.5))
                    return LeyLines;

                if (UseEndOfFireWeave(ref actionID, fallbackWhenNoTranspose: Blizzard))
                    return actionID;

                if (UseIceWeave(ref actionID))
                    return actionID;

                if (UseAddle())
                    return Role.Addle;
            }

            if (UseScathe())
                return Scathe;

            if (UsePolyglotOvercap())
                return PolyglotSpell;

            if (UseThunder())
                return OriginalHook(Thunder);

            if (UseAmplifierXeno())
                return Xenoglossy;

            if (UseMovementGcd(ref actionID))
                return actionID;

            if (IsInFirePhase && UseFirePhaseGcd(ref actionID))
                return actionID;

            if (IsInIcePhase && UseIcePhaseGcd(ref actionID))
                return actionID;

            if (UseOutOfPhaseGcd(ref actionID))
                return actionID;

            return OriginalHook(Blizzard);
        }
    }

    internal class BLM_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Blizzard2, HighBlizzard2))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseAoETriplecastMovement())
                    return Triplecast;

                if (UseAoEManafont())
                    return Manafont;

                if (UseAoETranspose())
                    return Transpose;

                if (UseAmplifier(onAoE: true))
                    return Amplifier;

                if (UseLeyLines(
                    0,
                    false,
                    2.5,
                    40))
                    return LeyLines;
            }

            if (UseAoEPolyglotOvercap())
                return Foul;

            if (UseAoEPolyglot())
                return Foul;

            if (UseAoEThunder())
                return OriginalHook(Thunder2);

            if (UseAoEParadoxFiller())
                return OriginalHook(Blizzard);

            if (IsInFirePhase && UseAoEFirePhaseGcd(ref actionID))
                return actionID;

            if (IsInIcePhase && UseAoEIcePhaseGcd(ref actionID))
                return actionID;

            return OriginalHook(Blizzard2);
        }
    }

    #endregion

    #region Advanced Mode

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Blizzard))
                return actionID;

            if (IsEnabled(Preset.BLM_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (IsEnabled(Preset.BLM_ST_Manaward) &&
                UseManaward(
                    BLM_ST_ManawardHPThreshold,
                    false,
                    BLM_ST_ManawardTrigger,
                    BLM_ST_ManawardSolo))
                return Manaward;

            if (CanWeave())
            {
                if (IsEnabled(Preset.BLM_ST_Amplifier) && UseAmplifier())
                    return Amplifier;

                if (IsEnabled(Preset.BLM_ST_LeyLines) &&
                    UseLeyLines(
                        BLM_ST_LeyLinesCharges,
                        BLM_ST_LeyLinesMovement == 1,
                        BLM_ST_LeyLinesTimeStill,
                        LeyLinesHPThreshold))
                    return LeyLines;

                if (UseEndOfFireWeave(
                    ref actionID,
                    IsEnabled(Preset.BLM_ST_Manafont),
                    IsEnabled(Preset.BLM_ST_Swiftcast),
                    IsEnabled(Preset.BLM_ST_Triplecast),
                    BLM_ST_Triplecast_WhenToUse == 0,
                    true,
                    IsEnabled(Preset.BLM_ST_Transpose),
                    true,
                    Blizzard))
                    return actionID;

                if (UseIceWeave(
                    ref actionID,
                    IsEnabled(Preset.BLM_ST_Transpose),
                    IsEnabled(Preset.BLM_ST_Swiftcast),
                    IsEnabled(Preset.BLM_ST_Triplecast),
                    BLM_ST_Triplecast_WhenToUse == 0,
                    true))
                    return actionID;

                if (IsEnabled(Preset.BLM_ST_Addle) && UseAddle())
                    return Role.Addle;
            }

            if (IsEnabled(Preset.BLM_ST_UsePolyglot) && UsePolyglotOvercap())
                return PolyglotSpell;

            if (IsEnabled(Preset.BLM_ST_Thunder) &&
                UseThunder(ThunderHPThreshold(), BLM_ST_ThunderRefresh))
                return OriginalHook(Thunder);

            if (IsEnabled(Preset.BLM_ST_Amplifier) &&
                IsEnabled(Preset.BLM_ST_UsePolyglot) &&
                UseAmplifierXeno())
                return Xenoglossy;

            if (IsEnabled(Preset.BLM_ST_Movement) &&
                UseMovementGcd(ref actionID, true))
                return actionID;

            if (IsInFirePhase &&
                UseFirePhaseGcd(
                    ref actionID,
                    IsEnabled(Preset.BLM_ST_FlareStar),
                    IsEnabled(Preset.BLM_ST_Despair),
                    IsEnabled(Preset.BLM_ST_Transpose),
                    IsEnabled(Preset.BLM_ST_UsePolyglot),
                    false,
                    BLM_ST_PolyglotMovement,
                    BLM_ST_PolyglotSaveUsage))
                return actionID;

            if (IsInIcePhase &&
                UseIcePhaseGcd(ref actionID, IsEnabled(Preset.BLM_ST_Transpose)))
                return actionID;

            if (UseOutOfPhaseGcd(ref actionID))
                return actionID;

            return OriginalHook(Blizzard);
        }
    }

    internal class BLM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Blizzard2, HighBlizzard2))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.BLM_AoE_Movement) && UseAoETriplecastMovement())
                    return Triplecast;

                if (IsEnabled(Preset.BLM_AoE_Manafont) && UseAoEManafont())
                    return Manafont;

                if (IsEnabled(Preset.BLM_AoE_Transpose) && UseAoETranspose())
                    return Transpose;

                if (IsEnabled(Preset.BLM_AoE_Amplifier) && UseAmplifier(onAoE: true))
                    return Amplifier;

                if (IsEnabled(Preset.BLM_AoE_LeyLines) &&
                    UseLeyLines(
                        BLM_AoE_LeyLinesCharges,
                        BLM_AoE_LeyLinesMovement == 1,
                        BLM_AoE_LeyLinesTimeStill,
                        BLM_AoE_LeyLinesOption))
                    return LeyLines;
            }

            if (IsEnabled(Preset.BLM_AoE_UsePolyglot) && UseAoEPolyglotOvercap())
                return Foul;

            if (IsEnabled(Preset.BLM_AoE_UsePolyglot) && UseAoEPolyglot())
                return Foul;

            if (IsEnabled(Preset.BLM_AoE_Thunder) && UseAoEThunder(BLM_AoE_ThunderHP))
                return OriginalHook(Thunder2);

            if (IsEnabled(Preset.BLM_AoE_ParadoxFiller) && UseAoEParadoxFiller())
                return OriginalHook(Blizzard);

            bool useTranspose = IsEnabled(Preset.BLM_AoE_Transpose);

            if (IsInFirePhase &&
                UseAoEFirePhaseGcd(
                    ref actionID,
                    IsEnabled(Preset.BLM_AoE_Triplecast),
                    BLM_AoE_TriplecastHoldCharges,
                    useTranspose,
                    !useTranspose))
                return actionID;

            if (IsInIcePhase &&
                UseAoEIcePhaseGcd(
                    ref actionID,
                    useTranspose,
                    !useTranspose,
                    IsEnabled(Preset.BLM_AoE_Blizzard4Sub),
                    true))
                return actionID;

            return OriginalHook(Blizzard2);
        }
    }

    #endregion

    #region Features

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

            return HasStatusEffect(Buffs.Triplecast) && ActionLearned(Triplecast)
                ? All.Cease
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
                Fire when BLM_F1to3 == 0 && BLM_Fire1_Despair && IsInFirePhase && MP.Cur < 2400 && ActionLearned(Despair) => Despair,

                Fire when BLM_F1to3 == 0 && ActionLearned(Fire3) &&
                          (AstralFireStacks is 1 or 2 && HasStatusEffect(Buffs.Firestarter) ||
                           ActionLearned(Paradox) && !IsParadoxActive ||
                           !InCombat() && ActionLearned(Fire4) ||
                           IsInIcePhase && !IsParadoxActive ||
                           !ActionLearned(Fire4) &&
                           HasStatusEffect(Buffs.Firestarter)) && !JustUsed(Fire3) => Fire3,

                Fire3 when BLM_F1to3 == 1 && ActionLearned(Fire3) && IsInFirePhase &&
                           (ActionLearned(Paradox) && IsParadoxActive && AstralFireStacks is 3 ||
                            !ActionLearned(Fire4) && !HasStatusEffect(Buffs.Firestarter)) &&
                           !JustUsed(OriginalHook(Fire)) => OriginalHook(Fire),

                _ => actionID
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

            return IsParadoxActive && IsInIcePhase
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
                    ? ActionLearned(Fire3)
                        ? Fire3
                        : Fire
                    : actionID;
            }

            return IsInIcePhase switch
            {
                false when BLM_Fire4_FlareStar && CanFlareStar() && ActionLearned(FlareStar) => FlareStar,
                false when BLM_Fire4_Fire3 && AstralFireStacks < 3 => ActionLearned(Fire3) ? Fire3 : Fire,
                false => actionID,

                true when BLM_Fire4_FireAndIce == 0 && UmbralIceStacks < 3 => ActionLearned(Blizzard3) ? Blizzard3 : Blizzard,
                true when BLM_Fire4_FireAndIce == 0 && UmbralIceStacks == 3 && ActionLearned(Blizzard4) => Blizzard4,
                true when BLM_Fire4_FireAndIce == 1 => BLM_Fire4_Fire3 && ActionLearned(Fire3) ? Fire3 : Fire,
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
                Flare when BLM_Flare_FlareStar && IsInFirePhase && CanFlareStar() => FlareStar,
                Flare when IsInFirePhase && ActionLearned(Flare) => Flare,
                Flare when IsInIcePhase && ActionReady(Freeze) => Freeze,
                _ => actionID
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
                Blizzard when BLM_B1to3 == 0 && ActionLearned(Blizzard3) &&
                              (IsInFirePhase ||
                               UmbralIceStacks is 1 ||
                               UmbralIceStacks is 2) => Blizzard3,

                Blizzard3 when BLM_B1to3 == 1 && ActionLearned(Blizzard3) && IsInIcePhase && UmbralIceStacks is 3 => OriginalHook(Blizzard),
                Blizzard3 when BLM_Blizzard3_Despair && IsInFirePhase && ActionLearned(Despair) && MP.Cur >= 800 => Despair,

                _ => actionID
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

            return IsParadoxActive && IsInFirePhase
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

            return IsInFirePhase && ActionLearned(Despair) && MP.Cur >= 800
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
                Freeze when IsUmbralHeartCapped && ActionLearned(Paradox) && IsParadoxActive && IsInIcePhase => OriginalHook(Blizzard),
                Freeze when !ActionLearned(Freeze) => Blizzard2,
                _ => actionID
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

            return IsInFirePhase && ActionLearned(FlareStar) && IsParadoxActive && AstralSoulStacks < 6
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

            return BLM_AmplifierXenoCD && IsOnCooldown(Amplifier) && HasPolyglot || IsPolyglotCapped
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

            return IsInIcePhase && ActionLearned(UmbralSoul)
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

            return ActionLearned(Xenoglossy) && HasPolyglot
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

            return HasStatusEffect(Buffs.LeyLines) && ActionLearned(BetweenTheLines)
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

    #endregion
}
