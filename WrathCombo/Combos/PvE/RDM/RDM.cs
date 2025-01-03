using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE;

internal partial class RDM
{
    internal class RDM_VariantVerCure : CustomCombo
    {  
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Variant_Cure2;
        
        protected override uint Invoke(uint actionID) =>            
            actionID is Vercure && IsEnabled(Variant.VariantCure)
            ? Variant.VariantCure
            : actionID;
    }

    internal class RDM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3)) return actionID;

            //VARIANTS
            if (IsEnabled(CustomComboPreset.RDM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.RDM_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.RDM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            // Opener for RDM
            if (Opener().FullOpener(ref actionID))
                return actionID;

            //oGCDs
            if (TryOGCDs(actionID, true, out uint oGCDAction)) return oGCDAction;

            //Lucid Dreaming
            if (TryLucidDreaming(actionID, 6500, ComboAction)) return All.LucidDreaming;

            //Melee Finisher
            if (MeleeCombo.TryMeleeFinisher(out uint finisherAction)) return finisherAction;

            //Melee Combo
            //  Manafication/Embolden Code
            if (MeleeCombo.TrySTManaEmbolden(actionID, out uint ManaEmbolden)) return ManaEmbolden;
            if (MeleeCombo.TrySTMeleeCombo(actionID, out uint MeleeID)) return MeleeID;

            //Normal Spell Rotation
            if (SpellCombo.TryAcceleration(actionID, out uint Accel)) return Accel;
            if (SpellCombo.TrySTSpellRotation(actionID, out uint SpellID)) return SpellID;

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
                actionID is not (Fleche or Riposte or Reprise)) return actionID;

            if (actionID is Jolt or Jolt2 or Jolt3)
            {
                //VARIANTS
                if (IsEnabled(CustomComboPreset.RDM_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= GetOptionValue(Config.RDM_VariantCure))
                    return Variant.VariantCure;

                if (IsEnabled(CustomComboPreset.RDM_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanSpellWeave())
                    return Variant.VariantRampart;

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
                    if (TryOGCDs(actionID, true, out uint oGCDAction, true)) return oGCDAction;
                }
            }
            //END_RDM_OGCD

            //Lucid Dreaming
            if (IsEnabled(CustomComboPreset.RDM_ST_Lucid)
                && actionID is Jolt or Jolt2 or Jolt3
                && TryLucidDreaming(actionID, Config.RDM_ST_Lucid_Threshold, ComboAction)) //Don't interupt certain combos
                return All.LucidDreaming;

            //RDM_MELEEFINISHER
            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeFinisher))
            {
                bool ActionFound =
                    (!Config.RDM_ST_MeleeFinisher_Adv && actionID is Jolt or Jolt2 or Jolt3) ||
                    (Config.RDM_ST_MeleeFinisher_Adv &&
                        ((Config.RDM_ST_MeleeFinisher_OnAction[0] && actionID is Jolt or Jolt2 or Jolt3) ||
                         (Config.RDM_ST_MeleeFinisher_OnAction[1] && actionID is Riposte or EnchantedRiposte) ||
                         (Config.RDM_ST_MeleeFinisher_OnAction[2] && actionID is Veraero or Veraero3 or Verthunder or Verthunder3)));

                if (ActionFound && MeleeCombo.TryMeleeFinisher(out uint finisherAction))
                    return finisherAction;
            }
            //END_RDM_MELEEFINISHER

            //RDM_ST_MELEECOMBO
            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo)
                && LocalPlayer.IsCasting == false)
            {
                bool ActionFound =
                    (!Config.RDM_ST_MeleeCombo_Adv && (actionID is Jolt or Jolt2 or Jolt3)) ||
                    (Config.RDM_ST_MeleeCombo_Adv &&
                        ((Config.RDM_ST_MeleeCombo_OnAction[0] && actionID is Jolt or Jolt2 or Jolt3) ||
                         (Config.RDM_ST_MeleeCombo_OnAction[1] && actionID is Riposte or EnchantedRiposte)));

                if (ActionFound)
                {
                    if (MeleeCombo.TrySTManaEmbolden(
                        actionID, out uint ManaEmboldenID, IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden), IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser),
                        IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo),
                        IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)))
                        return ManaEmboldenID;

                    if (MeleeCombo.TrySTMeleeCombo(actionID, out uint MeleeID, Config.RDM_ST_MeleeEnforced, IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser),
                        IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)))
                        return MeleeID;
                }
            }

            //RDM_ST_ACCELERATION
            if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) && IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel)
                && actionID is Jolt or Jolt2 or Jolt3)
            {
                if (SpellCombo.TryAcceleration(actionID, out uint AccID, IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel_Swiftcast))) 
                    return AccID;
            }

            if (actionID is Jolt or Jolt2 or Jolt3)
            {

                if (SpellCombo.TrySTSpellRotation(actionID, out uint SpellID,
                    IsEnabled(CustomComboPreset.RDM_ST_FireStone),
                    IsEnabled(CustomComboPreset.RDM_ST_ThunderAero)))
                   return SpellID;
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
            if (actionID is not (Scatter or Impact)) return actionID;

            //VARIANTS
            if (IsEnabled(CustomComboPreset.RDM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.RDM_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.RDM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            //RDM_OGCD
            if (TryOGCDs(actionID, true, out uint oGCDAction, true)) return oGCDAction;

            // LUCID
            if (TryLucidDreaming(actionID, 6500, ComboAction))
                return All.LucidDreaming;

            //RDM_MELEEFINISHER
            if (MeleeCombo.TryMeleeFinisher(out uint finisherAction))
                return finisherAction;

            if (MeleeCombo.TryAoEManaEmbolden(actionID, out uint ManaEmbolden))
                return ManaEmbolden;

            if (MeleeCombo.TryAoEMeleeCombo(actionID, out uint AoEMeleeID))
                return AoEMeleeID;

            if (SpellCombo.TryAcceleration(actionID, out uint AccelID))
                return AccelID;

            if (SpellCombo.TryAoESpellRotation(actionID, out uint SpellID))
                return SpellID;
            return actionID;
        }
    }

    internal class RDM_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is Scatter or Impact)
            {
                //VARIANTS
                if (IsEnabled(CustomComboPreset.RDM_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= GetOptionValue(Config.RDM_VariantCure))
                    return Variant.VariantCure;

                if (IsEnabled(CustomComboPreset.RDM_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanSpellWeave())
                    return Variant.VariantRampart;

                //RDM_OGCD
                if (IsEnabled(CustomComboPreset.RDM_AoE_oGCD)
                    && LevelChecked(Corpsacorps)
                    && TryOGCDs(actionID, true, out uint oGCDAction, true)) return oGCDAction;

                // LUCID
                if (IsEnabled(CustomComboPreset.RDM_AoE_Lucid)
                    && TryLucidDreaming(actionID, Config.RDM_AoE_Lucid_Threshold, ComboAction))
                    return All.LucidDreaming;
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


                if (ActionFound && MeleeCombo.TryMeleeFinisher(out uint finisherAction))
                    return finisherAction;
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
                        && MeleeCombo.TryAoEManaEmbolden(actionID, out uint ManaEmbolen, Config.RDM_AoE_MoulinetRange))
                        return ManaEmbolen;

                    if (MeleeCombo.TryAoEMeleeCombo(actionID, out uint AoEMelee, Config.RDM_AoE_MoulinetRange, IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_CorpsGapCloser),
                        false)) //Melee range enforced
                        return AoEMelee;
                }
            }

            if (actionID is Scatter or Impact)
            {
                if (IsEnabled(CustomComboPreset.RDM_AoE_Accel) 
                    && SpellCombo.TryAcceleration(actionID, out uint AccelID, IsEnabled(CustomComboPreset.RDM_AoE_Accel_Swiftcast),
                    IsEnabled(CustomComboPreset.RDM_AoE_Accel_Weave)))
                    return AccelID;
                
                if (SpellCombo.TryAoESpellRotation(actionID, out uint SpellID)) 
                    return SpellID;
            
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
            if (actionID is not All.Swiftcast) return actionID;

            if (HasEffect(All.Buffs.Swiftcast) && IsEnabled(CustomComboPreset.SMN_Variant_Raise) && IsEnabled(Variant.VariantRaise))
                return Variant.VariantRaise;

            if (LevelChecked(Verraise))
            {
                bool schwifty = HasEffect(All.Buffs.Swiftcast);
                if (schwifty || HasEffect(Buffs.Dualcast)) return Verraise;
                if (IsEnabled(CustomComboPreset.RDM_Raise_Vercure) &&
                    !schwifty &&
                    ActionReady(Vercure) &&
                    IsOnCooldown(All.Swiftcast))
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
            && ActionReady(All.Addle)
            && !TargetHasEffectAny(All.Debuffs.Addle) ? All.Addle : actionID;
    }

    internal class RDM_EmboldenProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenProtection;
        protected override uint Invoke(uint actionID) =>
            actionID is Embolden &&
            ActionReady(Embolden) &&
            HasEffectAny(Buffs.EmboldenOthers) ? OriginalHook(11) : actionID;
    }

    internal class RDM_MagickProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickProtection;
        protected override uint Invoke(uint actionID) =>
            actionID is MagickBarrier &&
            ActionReady(MagickBarrier) &&
            HasEffectAny(Buffs.MagickBarrier) ? OriginalHook(11) : actionID;
    }
}
