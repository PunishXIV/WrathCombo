using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE
{
    internal partial class RDM
    {
        //7.0 Note
        //Gauge information is available via RDMMana
        public const byte JobID = 35;

        public const uint
            Verthunder = 7505,
            Veraero = 7507,
            Veraero2 = 16525,
            Veraero3 = 25856,
            Verthunder2 = 16524,
            Verthunder3 = 25855,
            Impact = 16526,
            Redoublement = 7516,
            EnchantedRedoublement = 7529,
            Zwerchhau = 7512,
            EnchantedZwerchhau = 7528,
            Riposte = 7504,
            EnchantedRiposte = 7527,
            Scatter = 7509,
            Verstone = 7511,
            Verfire = 7510,
            Vercure = 7514,
            Jolt = 7503,
            Jolt2 = 7524,
            Jolt3 = 37004,
            Verholy = 7526,
            Verflare = 7525,
            Fleche = 7517,
            ContreSixte = 7519,
            Engagement = 16527,
            Verraise = 7523,
            Scorch = 16530,
            Resolution = 25858,
            Moulinet = 7513,
            EnchantedMoulinet = 7530,
            EnchantedMoulinetDeux = 37002,
            EnchantedMoulinetTrois = 37003,
            Corpsacorps = 7506,
            Displacement = 7515,
            Reprise = 16529,
            ViceOfThorns = 37005,
            GrandImpact = 37006,
            Prefulgence = 37007,

            //Buffs
            Acceleration = 7518,
            Manafication = 7521,
            Embolden = 7520,
            MagickBarrier = 25857;

        public static class Buffs
        {
            public const ushort
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249,
                Chainspell = 2560,
                Acceleration = 1238,
                Embolden = 1239,
                EmboldenOthers = 1297,
                Manafication = 1971,
                MagickBarrier = 2707,
                MagickedSwordPlay = 3875,
                ThornedFlourish = 3876,
                GrandImpactReady = 3877,
                PrefulugenceReady = 3878;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Traits
        {
            public const uint
                EnhancedEmbolden = 620,
                EnhancedManaficationII = 622,
                EnhancedManaficationIII = 622,
                EnhancedAccelerationII = 624;
        }

        internal class RDM_VariantVerCure : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Vercure && IsEnabled(CustomComboPreset.RDM_Variant_Cure2) && IsEnabled(Variant.VariantCure)
                    ? Variant.VariantCure : actionID;
        }

        internal class RDM_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
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
                    if (Opener().FullOpener(ref actionID))
                        return actionID;

                    //oGCDs
                    if (TryOGCDs(actionID, true, out uint oGCDAction)) return oGCDAction;

                    //Lucid Dreaming
                    if (TryLucidDreaming(actionID, 6500, lastComboMove)) return All.LucidDreaming;

                    //Melee Finisher
                    if (MeleeCombo.TryMeleeFinisher(lastComboMove, out uint finisherAction)) return finisherAction;

                    //Melee Combo
                    //  Manafication/Embolden Code
                    if (MeleeCombo.TrySTManaEmbolden(actionID, lastComboMove, level, out uint ManaEmbolden)) return ManaEmbolden;
                    if (MeleeCombo.TrySTMeleeCombo(actionID, lastComboMove, comboTime, out uint MeleeID)) return MeleeID;

                    //Normal Spell Rotation
                    if (SpellCombo.TryAcceleration(actionID, lastComboMove, out uint Accel)) return Accel;
                    if (SpellCombo.TrySTSpellRotation(actionID, out uint SpellID)) return SpellID;
                                        
                }

                //NO_CONDITIONS_MET
                return actionID;
            }
        }

        internal class RDM_ST_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_DPS;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
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
                    && TryLucidDreaming(actionID, Config.RDM_ST_Lucid_Threshold, lastComboMove)) //Don't interupt certain combos
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

                    if (ActionFound && MeleeCombo.TryMeleeFinisher(lastComboMove, out uint finisherAction))
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
                            actionID, lastComboMove, level, out uint ManaEmboldenID,
                            IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden),
                            IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser),
                            IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_ManaEmbolden_DoubleCombo),
                            IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)))
                            return ManaEmboldenID;

                        if (MeleeCombo.TrySTMeleeCombo(actionID, lastComboMove, comboTime, out uint MeleeID,
                            Config.RDM_ST_MeleeEnforced,
                            IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_CorpsGapCloser),
                            IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_UnbalanceMana)))
                            return MeleeID;
                    }
                }

                //RDM_ST_ACCELERATION
                if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) && IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel)
                    && actionID is Jolt or Jolt2 or Jolt3)
                {
                    if (SpellCombo.TryAcceleration(actionID, lastComboMove, out uint AccID,
                        IsEnabled(CustomComboPreset.RDM_ST_ThunderAero_Accel_Swiftcast))) 
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
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
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
                    if (TryOGCDs(actionID, true, out uint oGCDAction, true)) return oGCDAction;

                    // LUCID
                    if (TryLucidDreaming(actionID, 6500, lastComboMove))
                        return All.LucidDreaming;

                    //RDM_MELEEFINISHER
                    if (MeleeCombo.TryMeleeFinisher(lastComboMove, out uint finisherAction))
                        return finisherAction;
                
                    if (MeleeCombo.TryAoEManaEmbolden(actionID, lastComboMove, out uint ManaEmbolden))
                        return ManaEmbolden;

                    if (MeleeCombo.TryAoEMeleeCombo(actionID, lastComboMove, comboTime, out uint AoEMeleeID))
                        return AoEMeleeID;
                    
                    if (SpellCombo.TryAcceleration(actionID, lastComboMove, out uint AccelID))
                        return AccelID;

                    if (SpellCombo.TryAoESpellRotation(actionID, out uint SpellID))
                        return SpellID;
                }
                return actionID;
            }
        }

        internal class RDM_AoE_DPS : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_DPS;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
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
                        && TryLucidDreaming(actionID, Config.RDM_AoE_Lucid_Threshold, lastComboMove))
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


                    if (ActionFound && MeleeCombo.TryMeleeFinisher(lastComboMove, out uint finisherAction))
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
                            && MeleeCombo.TryAoEManaEmbolden(actionID, lastComboMove, out uint ManaEmbolen, Config.RDM_AoE_MoulinetRange))
                            return ManaEmbolen;

                        if (MeleeCombo.TryAoEMeleeCombo(actionID, lastComboMove, comboTime, out uint AoEMelee, 
                            Config.RDM_AoE_MoulinetRange, //Range
                            IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_CorpsGapCloser), //Gap Closer
                            false)) //Melee range enforced
                            return AoEMelee;
                    }
                }

                if (actionID is Scatter or Impact)
                {
                    if (IsEnabled(CustomComboPreset.RDM_AoE_Accel) 
                        && SpellCombo.TryAcceleration(actionID, lastComboMove, out uint AccelID, 
                        IsEnabled(CustomComboPreset.RDM_AoE_Accel_Swiftcast), IsEnabled(CustomComboPreset.RDM_AoE_Accel_Weave)))
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
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is All.Swiftcast)
                {
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

                }

                // Else we just exit normally and return Swiftcast
                return actionID;
            }
        }

        internal class RDM_CorpsDisplacement : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_CorpsDisplacement;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Displacement
                && LevelChecked(Displacement)
                && HasTarget()
                && GetTargetDistance() >= 5 ? Corpsacorps : actionID;
        }

        internal class RDM_EmboldenManafication : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenManafication;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Embolden
                && IsOnCooldown(Embolden)
                && ActionReady(Manafication) ? Manafication : actionID;
        }

        internal class RDM_MagickBarrierAddle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickBarrierAddle;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is MagickBarrier
                && (IsOnCooldown(MagickBarrier) || !LevelChecked(MagickBarrier))
                && ActionReady(All.Addle)
                && !TargetHasEffectAny(All.Debuffs.Addle) ? All.Addle : actionID;
        }

        internal class RDM_EmboldenProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Embolden &&
                ActionReady(Embolden) &&
                HasEffectAny(Buffs.EmboldenOthers) ? OriginalHook(11) : actionID;
        }

        internal class RDM_MagickProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickProtection;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is MagickBarrier &&
                ActionReady(MagickBarrier) &&
                HasEffectAny(Buffs.MagickBarrier) ? OriginalHook(11) : actionID;
        }
    }
}
