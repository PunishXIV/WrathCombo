using WrathCombo.CustomComboNS;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE;

internal partial class RDM : Caster
{
    internal class RDM_VariantVerCure : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Variant_Cure2;

        protected override uint Invoke(uint actionID) =>
            actionID is Vercure && Variant.CanCure(Preset,100)
            ? Variant.Cure
            : actionID;
    }

    internal class RDM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3))
                return actionID;

            //VARIANTS
            if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                return Variant.Rampart;

            uint NewActionID = 0;

            //oGCDs
            if (TryOGCDs(actionID, true, ref NewActionID))
                return NewActionID;

            //Lucid Dreaming
            if (TryLucidDreaming(6500, ComboAction))
                return Role.LucidDreaming;

            //Melee Finisher
            if (MeleeCombo.TryMeleeFinisher(ref NewActionID))
                return NewActionID;

            //Melee Combo
            //  Manafication/Embolden Code
            if (MeleeCombo.TrySTManaEmbolden(ref NewActionID))
                return NewActionID;
            if (MeleeCombo.TrySTMeleeCombo(ref NewActionID))
                return NewActionID;
            if (MeleeCombo.TrySTMeleeStart(ref NewActionID))
                return NewActionID;

            //Normal Spell Rotation
            if (SpellCombo.TryAcceleration(ref NewActionID))
                return NewActionID;
            if (SpellCombo.TrySTSpellRotation(ref NewActionID))
                return NewActionID;

            //NO_CONDITIONS_MET
            return actionID;
        }
    }

    internal class RDM_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3) &&
                actionID is not (Fleche or Riposte or Reprise))
                return actionID;

            uint NewActionID = 0;

            if (actionID is Jolt or Jolt2 or Jolt3)
            {
                //VARIANTS
                if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                    return Variant.Cure;

                if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                    return Variant.Rampart;

                // Opener for RDM
                if (IsEnabled(CustomComboPreset.RDM_Balance_Opener) && ContentCheck.IsInConfiguredContent(Config.RDM_BalanceOpener_Content, ContentCheck.ListSet.BossOnly))
                {
                    if (Opener().FullOpener(ref actionID))
                        return actionID;
                }
            }

            //RDM_OGCD
            if (IsEnabled(CustomComboPreset.RDM_ST_oGCD))
            {
                bool ActionFound =
                    (!Config.RDM_ST_oGCD_OnAction_Adv && actionID is Jolt or Jolt2 or Jolt3) ||
                      (Config.RDM_ST_oGCD_OnAction_Adv &&
                        ((Config.RDM_ST_oGCD_OnAction[0] && actionID is Jolt or Jolt2 or Jolt3) ||
                         (Config.RDM_ST_oGCD_OnAction[1] && actionID is Fleche) ||
                         (Config.RDM_ST_oGCD_OnAction[2] && actionID is Riposte) ||
                         (Config.RDM_ST_oGCD_OnAction[3] && actionID is Reprise)
                        )
                      );

                if (ActionFound && LevelChecked(Corpsacorps))
                {
                    if (TryOGCDs(actionID, true, ref NewActionID, true))
                        return NewActionID;
                }
            }
            //END_RDM_OGCD

            //Lucid Dreaming
            if (IsEnabled(CustomComboPreset.RDM_ST_Lucid)
                && actionID is Jolt or Jolt2 or Jolt3
                && TryLucidDreaming(Config.RDM_ST_Lucid_Threshold, ComboAction)) //Don't interupt certain combos
                return Role.LucidDreaming;

            //RDM_MELEEFINISHER
            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeFinisher))
            {
                bool isJoltAction = actionID is Jolt or Jolt2 or Jolt3;
                bool useJolts = !Config.RDM_ST_MeleeFinisher_Adv && isJoltAction;
                bool useJoltsAdv = Config.RDM_ST_MeleeFinisher_OnAction[0] && isJoltAction;
                bool useRiposteAdv = Config.RDM_ST_MeleeFinisher_OnAction[1] && actionID is Riposte or EnchantedRiposte;
                bool useVerMagicAdv = Config.RDM_ST_MeleeFinisher_OnAction[2] && actionID is Veraero or Veraero3 or Verthunder or Verthunder3;

                bool ActionFound = useJolts ||
                    (Config.RDM_ST_MeleeFinisher_Adv &&
                        (useJoltsAdv || useRiposteAdv || useVerMagicAdv));

                if (ActionFound && MeleeCombo.TryMeleeFinisher(ref NewActionID))
                    return NewActionID;
            }
            //END_RDM_MELEEFINISHER

            //RDM_ST_MELEECOMBO
            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo)
                && LocalPlayer.IsCasting == false)
            {
                bool isJoltAction = actionID is Jolt or Jolt2 or Jolt3;
                bool isAutoRotOn = P.IPC.GetComboOptionState(Preset.ToString()) && P.IPC.GetAutoRotationState();

                bool useJolts = !Config.RDM_ST_MeleeCombo_Adv && isJoltAction;
                bool useJoltsAdv = Config.RDM_ST_MeleeCombo_OnAction[0] && isJoltAction;
                bool useRiposte = Config.RDM_ST_MeleeCombo_OnAction[1] && (actionID is Riposte or EnchantedRiposte);

                bool ActionFound = useJolts || (Config.RDM_ST_MeleeCombo_Adv && (useJoltsAdv || useRiposte));

                //Burst
                if (ActionFound)
                {
                    if (MeleeCombo.TrySTManaEmbolden(
                        ref NewActionID, IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden), IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser),
                        IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo),
                        IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)))
                        return NewActionID;
                }

                //Zwerchhau & Redoublement. Force to Jolts if Auto Rotation
                if (ActionFound || (isAutoRotOn && isJoltAction))
                {
                    if (MeleeCombo.TrySTMeleeCombo(ref NewActionID, Config.RDM_ST_MeleeEnforced))
                        return NewActionID;
                }

                //Start the Combo
                if (ActionFound)
                {
                    if (MeleeCombo.TrySTMeleeStart(ref NewActionID, IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser),
                        IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)))
                        return NewActionID;
                }
            }

            //RDM_ST_ACCELERATION
            if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) && IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel)
                && actionID is Jolt or Jolt2 or Jolt3)
            {
                if (SpellCombo.TryAcceleration(ref NewActionID, IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel_Swiftcast)))
                    return NewActionID;
            }

            if (actionID is Jolt or Jolt2 or Jolt3)
            {

                if (SpellCombo.TrySTSpellRotation(ref NewActionID,
                    IsEnabled(CustomComboPreset.RDM_ST_FireStone),
                    IsEnabled(CustomComboPreset.RDM_ST_ThunderAero)))
                    return NewActionID;
            }

            //NO_CONDITIONS_MET
            return actionID;
        }
    }

    internal class RDM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Scatter or Impact))
                return actionID;

            //VARIANTS
            if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                return Variant.Rampart;

            uint NewActionID = 0;

            //RDM_OGCD
            if (TryOGCDs(actionID, false, ref NewActionID))
                return NewActionID;

            // LUCID
            if (TryLucidDreaming(6500, ComboAction))
                return Role.LucidDreaming;

            //RDM_MELEEFINISHER
            if (MeleeCombo.TryMeleeFinisher(ref NewActionID))
                return NewActionID;

            if (MeleeCombo.TryAoEManaEmbolden(ref NewActionID))
                return NewActionID;

            if (MeleeCombo.TryAoEMeleeCombo(ref NewActionID))
                return NewActionID;

            if (SpellCombo.TryAcceleration(ref NewActionID))
                return NewActionID;

            if (SpellCombo.TryAoESpellRotation(ref NewActionID))
                return NewActionID;
            return actionID;
        }
    }

    internal class RDM_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Scatter or Impact) &&
                actionID is not (Moulinet or Veraero2 or Verthunder2))
                return actionID;

            uint NewActionID = 0;

            if (actionID is Scatter or Impact)
            {
                //VARIANTS
                if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                    return Variant.Cure;

                if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                    return Variant.Rampart;

                //RDM_OGCD
                if (IsEnabled(CustomComboPreset.RDM_AoE_oGCD)
                    && LevelChecked(Corpsacorps)
                    && TryOGCDs(actionID, false, ref NewActionID, true))
                    return NewActionID;

                // LUCID
                if (IsEnabled(CustomComboPreset.RDM_AoE_Lucid)
                    && TryLucidDreaming(Config.RDM_AoE_Lucid_Threshold, ComboAction))
                    return Role.LucidDreaming;
            }

            //RDM_MELEEFINISHER
            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeFinisher))
            {
                bool ActionFound =
                    (!Config.RDM_AoE_MeleeFinisher_Adv && actionID is Scatter or Impact) ||
                    (Config.RDM_AoE_MeleeFinisher_Adv &&
                        ((Config.RDM_AoE_MeleeFinisher_OnAction[0] && actionID is Scatter or Impact) ||
                         (Config.RDM_AoE_MeleeFinisher_OnAction[1] && actionID is Moulinet) ||
                         (Config.RDM_AoE_MeleeFinisher_OnAction[2] && actionID is Veraero2 or Verthunder2)));

                if (ActionFound && MeleeCombo.TryMeleeFinisher(ref NewActionID))
                    return NewActionID;
            }
            //END_RDM_MELEEFINISHER

            //RDM_AOE_MELEECOMBO
            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo))
            {
                bool ActionFound =
                    (!Config.RDM_AoE_MeleeCombo_Adv && actionID is Scatter or Impact) ||
                    (Config.RDM_AoE_MeleeCombo_Adv &&
                        ((Config.RDM_AoE_MeleeCombo_OnAction[0] && actionID is Scatter or Impact) ||
                            (Config.RDM_AoE_MeleeCombo_OnAction[1] && actionID is Moulinet)));

                if (ActionFound)
                {
                    if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_ManaEmbolden)
                        && MeleeCombo.TryAoEManaEmbolden(ref NewActionID, Config.RDM_AoE_MoulinetRange))
                        return NewActionID;

                    if (MeleeCombo.TryAoEMeleeCombo(ref NewActionID, Config.RDM_AoE_MoulinetRange, IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_CorpsGapCloser),
                        false)) //Melee range enforced
                        return NewActionID;
                }
            }

            if (actionID is Scatter or Impact)
            {
                if (IsEnabled(CustomComboPreset.RDM_AoE_Accel)
                    && SpellCombo.TryAcceleration(ref NewActionID, IsEnabled(CustomComboPreset.RDM_AoE_Accel_Swiftcast),
                    IsEnabled(CustomComboPreset.RDM_AoE_Accel_Weave)))
                    return NewActionID;

                if (SpellCombo.TryAoESpellRotation(ref NewActionID))
                    return NewActionID;

            }

            return actionID;
        }
    }

    /*
    RDM_Verraise
    Swiftcast combos to Verraise when:
    -Swiftcast is on cooldown.
    -Swiftcast is available, but we we have Dualcast (Dualcasting Verraise)
    Using this variation other than the alternate feature style, as Verraise is level 63
    and swiftcast is unlocked way earlier and in theory, on a hotbar somewhere
    */
    internal class RDM_Verraise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Raise;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            if (Variant.CanRaise(CustomComboPreset.RDM_Variant_Raise))
                return Variant.Raise;

            if (LevelChecked(Verraise))
            {
                bool schwifty = HasStatusEffect(Role.Buffs.Swiftcast);
                if (schwifty || HasStatusEffect(Buffs.Dualcast))
                    return Verraise;
                if (IsEnabled(CustomComboPreset.RDM_Raise_Vercure) &&
                    !schwifty &&
                    ActionReady(Vercure) &&
                    IsOnCooldown(Role.Swiftcast))
                    return Vercure;
            }

            // Else we just exit normally and return Swiftcast
            return actionID;
        }
    }

    internal class RDM_CorpsDisplacement : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_CorpsDisplacement;
        protected override uint Invoke(uint actionID) =>
            actionID is Displacement
            && LevelChecked(Displacement)
            && HasTarget()
            && GetTargetDistance() >= 5 ? Corpsacorps : actionID;
    }

    internal class RDM_EmboldenManafication : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenManafication;
        protected override uint Invoke(uint actionID) =>
            actionID is Embolden
            && IsOnCooldown(Embolden)
            && ActionReady(Manafication) ? Manafication : actionID;
    }

    internal class RDM_MagickBarrierAddle : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickBarrierAddle;
        protected override uint Invoke(uint actionID) =>
            actionID is MagickBarrier
            && (IsOnCooldown(MagickBarrier) || !LevelChecked(MagickBarrier))
            && Role.CanAddle() ? Role.Addle : actionID;
    }

    internal class RDM_EmboldenProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenProtection;
        protected override uint Invoke(uint actionID) =>
            actionID is Embolden &&
            ActionReady(Embolden) &&
            HasStatusEffect(Buffs.EmboldenOthers, anyOwner: true) ? All.SavageBlade : actionID;
    }

    internal class RDM_MagickProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickProtection;
        protected override uint Invoke(uint actionID) =>
            actionID is MagickBarrier &&
            ActionReady(MagickBarrier) &&
            HasStatusEffect(Buffs.MagickBarrier, anyOwner: true) ? All.SavageBlade : actionID;
    }
}
