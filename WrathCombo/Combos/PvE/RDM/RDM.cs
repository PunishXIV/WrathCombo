using System;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class RDM : Caster
{
    #region Simple Modes
    internal class RDM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3))
                return actionID;
            
            #region Special Content
            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
                
            if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                return Variant.Rampart;

            #endregion
            
            #region OGCDs
            if (CanWeave())
            {
                //Gap Closer
                if (ActionReady(Corpsacorps) && (HasEnoughManaToStart || CanMagickedSwordplay) && !InMeleeRange()) 
                    return Corpsacorps;
                 
                if (ActionReady(Manafication) && (EmboldenCD <= 5 || HasEmbolden) && !CanPrefulgence) 
                    return Manafication;
                
                if (ActionReady(Embolden) && !HasEmbolden) 
                    return Embolden;
                
                if (ActionReady(ContreSixte)) 
                    return ContreSixte;
                
                if (ActionReady(Fleche)) 
                    return Fleche;
                
                if (CanEngagement && PoolEngagement)  
                    return Engagement;
                
                if (CanCorps && InMeleeRange())
                    return Corpsacorps;
                
                if (CanPrefulgence)
                    return Prefulgence;
               
                if (CanViceOfThorns)
                    return ViceOfThorns;
               
                if (Role.CanLucidDream(8000))
                    return Role.LucidDreaming;
               
                if (CanAcceleration || CanAccelerationMovement) 
                    return Acceleration;
               
                if (CanSwiftcast || CanSwiftcastMovement)
                    return Role.Swiftcast;
            }
            #endregion
            
            #region Melee Combo and Finishers 
            if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch)) 
                return actionID;
            
            if (HasManaStacks) 
                return UseHolyFlare(actionID);
            
            if (InMeleeRange() && (HasEnoughManaForCombo || CanMagickedSwordplay))
            {
                if (ComboAction is Zwerchhau or EnchantedZwerchhau && LevelChecked(Redoublement)) 
                    return EnchantedRedoublement;
                
                if (ComboAction is Riposte or EnchantedRiposte && LevelChecked(Zwerchhau))
                    return EnchantedZwerchhau;
                
                if (ActionReady(EnchantedRiposte) && InMeleeRange() && !HasDualcast && !HasAccelerate && !HasSwiftcast &&
                    (HasEnoughManaToStart || CanMagickedSwordplay)) 
                    return EnchantedRiposte;
            }
            #endregion
            
            #region GCD Casts
            if (CanInstantCast)
                return UseInstantCastST(actionID);
            
            if (CanGrandImpact)  
                return GrandImpact;
            
            if (UseVerStone())
                return Verstone;
            
            if (UseVerFire())
                return Verfire;

            return actionID;
            #endregion
        }
    }
    
    internal class RDM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Scatter or Impact))
                return actionID;
            
            #region Special Content
            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
                
            if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                return Variant.Rampart;
            #endregion
            
            #region OGCDs
            if (CanWeave())
            {
                //Gap Closer Option
                if (ActionReady(Corpsacorps) && (HasEnoughManaToStart || CanMagickedSwordplay) && !InMeleeRange()) 
                    return Corpsacorps;
                 
                if (ActionReady(Manafication) && (EmboldenCD <= 5 || HasEmbolden) && !CanPrefulgence) 
                    return Manafication;
                
                if (ActionReady(Embolden) && !HasEmbolden) 
                    return Embolden;
                
                if (ActionReady(ContreSixte)) 
                    return ContreSixte;
                
                if (ActionReady(Fleche)) 
                    return Fleche;
                
                if (CanEngagement && PoolEngagement)  
                    return Engagement;
                
                if (CanCorps && InMeleeRange())
                    return Corpsacorps;
                
                if (CanPrefulgence)
                    return Prefulgence;
                
                if (CanViceOfThorns)
                    return ViceOfThorns;
                
                if (Role.CanLucidDream(8000))
                    return Role.LucidDreaming;
                
                if (CanAcceleration && GetRemainingCharges(Acceleration) > 1 || CanAccelerationMovement) 
                    return Acceleration;
                
                if (CanSwiftcast || CanSwiftcastMovement)
                    return Role.Swiftcast;
            }
            #endregion
            
            #region Melee Combo and Finishers 
            if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch)) 
                return actionID;
            
            if (HasManaStacks) 
                return UseHolyFlare(actionID);
            
            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo))
            {
                if (ActionReady(Moulinet) && HasBattleTarget() && GetTargetDistance() < 8 && 
                    (CanMagickedSwordplay ||HasEnoughManaToStart || ComboAction is EnchantedMoulinet or Moulinet or EnchantedMoulinetDeux && HasEnoughManaForCombo))
                    return OriginalHook(Moulinet);
                
                if (!ActionReady(Moulinet) && InMeleeRange() && HasEnoughManaForCombo)
                {
                    if (ComboAction is Zwerchhau or EnchantedZwerchhau && LevelChecked(Redoublement))  
                        return EnchantedRedoublement;
                    if (ComboAction is Riposte or EnchantedRiposte && LevelChecked(Zwerchhau))
                        return EnchantedZwerchhau;
                    if (ActionReady(EnchantedRiposte) && !HasDualcast && !HasAccelerate && !HasSwiftcast && HasEnoughManaToStart) 
                        return EnchantedRiposte; 
                }
            }
            #endregion
            
            #region GCD Casts
            if (CanGrandImpact)  
                return GrandImpact;
            
            if (!CanInstantCast)
                return UseThunderAeroAoE(actionID);
            
            return !LevelChecked(Scatter) ? UseInstantCastST(actionID) : actionID;
            #endregion
        }
    }
    #endregion
    
    #region Advanced Modes
    internal class RDM_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_ST_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3))
                return actionID;
            
            #region Opener
            if (IsEnabled(CustomComboPreset.RDM_Balance_Opener) && HasBattleTarget() &&
                Opener().FullOpener(ref actionID)) 
                return actionID;
            #endregion
            
            #region Special Content
            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
                
            if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                return Variant.Rampart;
            #endregion

            #region OGCDs
            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_GapCloser) && 
                    ActionReady(Corpsacorps) && (HasEnoughManaToStart || CanMagickedSwordplay) && !InMeleeRange()) 
                    return Corpsacorps;
                 
                if (IsEnabled(CustomComboPreset.RDM_ST_Manafication) && ActionReady(Manafication) && (EmboldenCD <= 5 || HasEmbolden) && !CanPrefulgence) 
                    return Manafication;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Embolden) && ActionReady(Embolden) && !HasEmbolden) 
                    return Embolden;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_ContreSixte) && ActionReady(ContreSixte)) 
                    return ContreSixte;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Fleche) && ActionReady(Fleche)) 
                    return Fleche;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Engagement) && CanEngagement && (IsNotEnabled(CustomComboPreset.RDM_ST_Engagement_Pooling) || PoolEngagement))  
                    return Engagement;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Corpsacorps) && CanCorps && 
                    GetTargetDistance()<= Config.RDM_ST_Corpsacorps_Distance &&
                    TimeStoodStill >= TimeSpan.FromSeconds(Config.RDM_ST_Corpsacorps_Time))
                    return Corpsacorps;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Prefulgence) && CanPrefulgence)
                    return Prefulgence;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_ViceOfThorns) && CanViceOfThorns)
                    return ViceOfThorns;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Lucid) && Role.CanLucidDream(Config.RDM_ST_Lucid_Threshold))
                    return Role.LucidDreaming;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Acceleration) && 
                    (CanAcceleration && GetRemainingCharges(Acceleration) > Config.RDM_ST_Acceleration_Charges || 
                    CanAccelerationMovement && IsEnabled(CustomComboPreset.RDM_ST_Acceleration_Movement))) 
                    return Acceleration;
                
                if (IsEnabled(CustomComboPreset.RDM_ST_Swiftcast) && 
                    (!IsEnabled(CustomComboPreset.RDM_ST_SwiftcastMovement) && CanSwiftcast || CanSwiftcastMovement))
                    return Role.Swiftcast;
            }
            #endregion
            
            #region Melee Combo and Finishers 
            if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch)) 
                return actionID;

            if (IsEnabled(CustomComboPreset.RDM_ST_HolyFlare) && HasManaStacks) 
                return UseHolyFlare(actionID);
            
            if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo) )
            {
                if ((InMeleeRange() || IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_MeleeCheck)) && (HasEnoughManaForCombo || CanMagickedSwordplay))
                {
                    if (ComboAction is Zwerchhau or EnchantedZwerchhau && LevelChecked(Redoublement)) 
                        return EnchantedRedoublement;
                    if (ComboAction is Riposte or EnchantedRiposte && LevelChecked(Zwerchhau))
                        return EnchantedZwerchhau;
                }
                
                if (IsEnabled(CustomComboPreset.RDM_ST_MeleeCombo_IncludeRiposte) && ActionReady(EnchantedRiposte) && 
                    InMeleeRange() && !HasDualcast && !HasAccelerate && !HasSwiftcast &&
                    (HasEnoughManaToStart || CanMagickedSwordplay)) 
                    return EnchantedRiposte;
            }
            #endregion
            
            #region GCD Casts
            if (IsEnabled(CustomComboPreset.RDM_ST_ThunderAero) && CanInstantCast)
                return UseInstantCastST(actionID);
            
            if (CanGrandImpact) 
                return GrandImpact;

            if (IsEnabled(CustomComboPreset.RDM_ST_FireStone))
            {
                if (UseVerStone())
                    return Verstone;
                if (UseVerFire())
                    return Verfire;
            }

            return actionID;
            #endregion
        }
    }
   
    internal class RDM_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_AoE_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Scatter or Impact))
                return actionID;

            #region Special Content
            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
                
            if (Variant.CanCure(CustomComboPreset.RDM_Variant_Cure, Config.RDM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RDM_Variant_Rampart))
                return Variant.Rampart;
            #endregion

            #region OGCDs
            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_GapCloser) && 
                    ActionReady(Corpsacorps) && (HasEnoughManaToStart || CanMagickedSwordplay) && !InMeleeRange()) 
                    return Corpsacorps;
                 
                if (IsEnabled(CustomComboPreset.RDM_AoE_Manafication) && ActionReady(Manafication) && (EmboldenCD <= 5 || HasEmbolden) && !CanPrefulgence) 
                    return Manafication;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_Embolden) && ActionReady(Embolden) && !HasEmbolden) 
                    return Embolden;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_ContreSixte) && ActionReady(ContreSixte)) 
                    return ContreSixte;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_Fleche) && ActionReady(Fleche)) 
                    return Fleche;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_Engagement) && CanEngagement && (IsNotEnabled(CustomComboPreset.RDM_AoE_Engagement_Pooling) || PoolEngagement))  
                    return Engagement;

                if (IsEnabled(CustomComboPreset.RDM_AoE_Corpsacorps) && CanCorps &&
                    GetTargetDistance() <= Config.RDM_AoE_Corpsacorps_Distance &&
                    TimeStoodStill >= TimeSpan.FromSeconds(Config.RDM_AoE_Corpsacorps_Time))
                    return Corpsacorps;
                    
                if (IsEnabled(CustomComboPreset.RDM_AoE_Prefulgence) && CanPrefulgence)
                    return Prefulgence;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_ViceOfThorns) && CanViceOfThorns)
                    return ViceOfThorns;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_Lucid) && Role.CanLucidDream(Config.RDM_AoE_Lucid_Threshold))
                    return Role.LucidDreaming;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_Acceleration) && 
                    (CanAcceleration && GetRemainingCharges(Acceleration) > Config.RDM_AoE_Acceleration_Charges || 
                    CanAccelerationMovement && IsEnabled(CustomComboPreset.RDM_AoE_Acceleration_Movement))) 
                    return Acceleration;
                
                if (IsEnabled(CustomComboPreset.RDM_AoE_Swiftcast) && 
                    (!IsEnabled(CustomComboPreset.RDM_AoE_SwiftcastMovement) && CanSwiftcast || CanSwiftcastMovement))
                    return Role.Swiftcast;
            }
            #endregion
            
            #region Melee Combo and Finishers 
            if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch)) 
                return actionID;
            
            if (IsEnabled(CustomComboPreset.RDM_AoE_HolyFlare) && HasManaStacks) 
                return UseHolyFlare(actionID);   

            if (IsEnabled(CustomComboPreset.RDM_AoE_MeleeCombo))
            {
                if (ActionReady(Moulinet) && 
                    (IsNotEnabled(CustomComboPreset.RDM_AoE_MeleeCombo_Target) && !HasBattleTarget() || HasBattleTarget() && GetTargetDistance() < 8) && 
                    (CanMagickedSwordplay ||HasEnoughManaToStart || ComboAction is EnchantedMoulinet or Moulinet or EnchantedMoulinetDeux && HasEnoughManaForCombo))
                    return OriginalHook(Moulinet);
                
                if (!ActionReady(Moulinet) && InMeleeRange() && HasEnoughManaForCombo)
                {
                    if (ComboAction is Zwerchhau or EnchantedZwerchhau && LevelChecked(Redoublement)) 
                        return EnchantedRedoublement;
                    if (ComboAction is Riposte or EnchantedRiposte && LevelChecked(Zwerchhau))
                        return EnchantedZwerchhau;
                    if (ActionReady(EnchantedRiposte) && !HasDualcast && !HasAccelerate && !HasSwiftcast && HasEnoughManaToStart) 
                        return EnchantedRiposte; 
                }
            }
            #endregion
            
            #region GCD Casts
            if (CanGrandImpact) 
                return GrandImpact;
            
            if (IsEnabled(CustomComboPreset.RDM_AoE_ThunderAero) && !CanInstantCast)
                return UseThunderAeroAoE(actionID);

            return !LevelChecked(Scatter) ? UseInstantCastST(actionID) : actionID;
            #endregion
        }
    }
    #endregion
    
    #region Standalone Features
    internal class RDM_VariantVerCure : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Variant_Cure2;

        protected override uint Invoke(uint actionID) =>
            actionID is Vercure && Variant.CanCure(Preset, 100)
                ? Variant.Cure
                : actionID;
    }
    
    internal class RDM_Verraise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Raise;
        protected override uint Invoke(uint actionID)
        {
            /*
            RDM_Verraise
            Swiftcast combos to Verraise when:
            - Swiftcast is on cooldown.
            - Swiftcast is available, but we have Dualcast (Dualcasting Verraise)
            Using this variation other than the alternate feature style, as Verraise is level 63
            and swiftcast is unlocked way earlier and in theory, on a hotbar somewhere
            */
            
            if (actionID != Role.Swiftcast)
                return actionID;

            if (Variant.CanRaise(CustomComboPreset.RDM_Variant_Raise))
                return IsEnabled(CustomComboPreset.RDM_Raise_Retarget)
                    ? Variant.Raise.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Variant.Raise;

            if (LevelChecked(Verraise))
            {
                bool schwifty = HasStatusEffect(Role.Buffs.Swiftcast);
                if (schwifty || HasStatusEffect(Buffs.Dualcast))
                    return IsEnabled(CustomComboPreset.RDM_Raise_Retarget)
                        ? Verraise
                            .Retarget(Role.Swiftcast,
                                SimpleTarget.Stack.AllyToRaise)
                            .AndRunMacro()
                        : Verraise.AndRunMacro();
                if (IsEnabled(CustomComboPreset.RDM_Raise_Vercure) &&
                    !schwifty &&
                    ActionReady(Vercure) &&
                    IsOnCooldown(Role.Swiftcast))
                    return IsEnabled(CustomComboPreset.RDM_Raise_Retarget)
                        ? Vercure.Retarget(Role.Swiftcast,
                            SimpleTarget.Stack.AllyToHeal)
                        : Vercure;
            }

            // Else we just exit normally and return Swiftcast
            return actionID;
        }
    }
    
    internal class RDM_VerAero : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_VerAero;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Veraero or Veraero3))
                return actionID;

            if (ComboAction is Scorch or Verholy or Verflare)
                return OriginalHook(Jolt);

            if (HasManaStacks)
                return UseHolyFlare(actionID);

            if (IsEnabled(CustomComboPreset.RDM_VerAero_Stone) && CanVerStone)
                return Verstone;

            if (!HasDualcast && !HasSwiftcast)
                return OriginalHook(Jolt);

            return actionID;
        }
    }

    internal class RDM_VerThunder : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_VerThunder;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Verthunder or Verthunder3))
                return actionID;

            if (ComboAction is Scorch or Verholy or Verflare) 
                return OriginalHook(Jolt);

            if (HasManaStacks) 
                return UseHolyFlare(actionID);

            if (IsEnabled(CustomComboPreset.RDM_VerThunder_Fire) && CanVerFire) 
                return Verfire;
        
            if (!HasDualcast && !HasSwiftcast)
                return OriginalHook(Jolt);
        
            return actionID;
        }
    }
    
    internal class RDM_ST_Melee_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Riposte;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Riposte)
                return actionID;
            
            if (IsEnabled(CustomComboPreset.RDM_Riposte_GapCloser) && ActionReady(Corpsacorps) && 
                (HasEnoughManaToStartStandalone || CanMagickedSwordplay) && !InMeleeRange()) 
                return Corpsacorps;
            
            if (IsEnabled(CustomComboPreset.RDM_Riposte_Finisher))
            {
                if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch)) 
                    return OriginalHook(Jolt);
                            
                if (HasManaStacks) 
                    return UseHolyFlare(actionID);
            }

            if (HasEnoughManaForCombo || CanMagickedSwordplay)
            {
                if (ComboAction is Zwerchhau or EnchantedZwerchhau && LevelChecked(Redoublement))  
                    return EnchantedRedoublement;
                                               
                if (ComboAction is Riposte or EnchantedRiposte && LevelChecked(Zwerchhau))
                    return EnchantedZwerchhau;
            }
            
            if (IsEnabled(CustomComboPreset.RDM_Riposte_NoWaste) && !HasEnoughManaToStartStandalone && !CanMagickedSwordplay)
                return All.SavageBlade;

            return actionID;
        }
    }
    
    internal class RDM_AOE_Melee_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_Moulinet;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Moulinet)
                return actionID;
            
            if (IsEnabled(CustomComboPreset.RDM_Moulinet_GapCloser) && ActionReady(Corpsacorps) && 
                (HasEnoughManaToStartStandalone || CanMagickedSwordplay) && !InMeleeRange()) 
                return Corpsacorps;
            
            if (IsEnabled(CustomComboPreset.RDM_Moulinet_Finisher))
            {
                if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch)) 
                    return OriginalHook(Jolt);
                            
                if (HasManaStacks) 
                    return UseHolyFlare(actionID);
            }
            
            if (IsEnabled(CustomComboPreset.RDM_Moulinet_NoWaste) && 
                ComboAction is not (Moulinet or EnchantedMoulinet or EnchantedMoulinetDeux) && !HasEnoughManaToStartStandalone && !CanMagickedSwordplay)
                return All.SavageBlade;
            
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

    internal class RDM_EmboldenProtection: CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_EmboldenProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Embolden)
                return actionID;

            if (CanViceOfThorns)
                return ViceOfThorns;
            
            if (IsEnabled(CustomComboPreset.RDM_EmboldenManafication) && ActionReady(Manafication) &&
                (IsOnCooldown(Embolden) || HasStatusEffect(Buffs.Embolden, SimpleTarget.Self, true)))
                return Manafication;

            return ActionReady(Embolden) &&
                   HasStatusEffect(Buffs.EmboldenOthers, anyOwner: true)
                ? All.SavageBlade
                : actionID;
        }
    }

    internal class RDM_MagickProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_MagickProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MagickBarrier)
                return actionID;
            
            if (IsEnabled(CustomComboPreset.RDM_MagickBarrierAddle))
            {
                if (Role.CanAddle() && CanNotMagickBarrier || 
                    GetCooldownRemainingTime(Role.Addle) < GetCooldownRemainingTime(MagickBarrier))
                    return HasStatusEffect(Debuffs.Addle, CurrentTarget, anyOwner: true) ? All.SavageBlade : Role.Addle;
            }
            
            if (ActionReady(MagickBarrier) && HasStatusEffect(Buffs.MagickBarrier, anyOwner: true))
                return All.SavageBlade;
            
            if (IsEnabled(CustomComboPreset.RDM_MagickBarrierAddle) && GetCooldownRemainingTime(Role.Addle) < GetCooldownRemainingTime(MagickBarrier))
                return Role.Addle;
            
            return actionID;
        }
    }
    
    internal class RDM_OGCDs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDM_OGCDs;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fleche)
                return actionID;

            if (ActionReady(Fleche))
                return Fleche;
            
            if (ActionReady(ContreSixte)) 
                return ContreSixte;
            
            if (HasStatusEffect(Buffs.ThornedFlourish))
                return ViceOfThorns;
            
            if (CanPrefulgence)
                return Prefulgence;
                
            if (GetRemainingCharges(Engagement) == 2 || 
                (HasEmbolden || !LevelChecked(Embolden) || IsNotEnabled(CustomComboPreset.RDM_OGCDs_EngagementPool)) & HasCharges(Engagement) ||
                GetRemainingCharges(Engagement) == 1 && GetCooldownChargeRemainingTime(Engagement) < EmboldenCD)
                return Engagement;
                
            if (GetRemainingCharges(Corpsacorps) == 2 && (InMeleeRange() || IsNotEnabled(CustomComboPreset.RDM_OGCDs_CorpsMelee)))
                return Corpsacorps;

            return GetCooldownRemainingTime(ContreSixte) < GetCooldownRemainingTime(Fleche) ? ContreSixte : actionID;
        }
    }
    #endregion 
}
