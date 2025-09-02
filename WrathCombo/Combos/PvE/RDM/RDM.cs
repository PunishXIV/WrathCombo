using System;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.RDM.Config;
namespace WrathCombo.Combos.PvE;

internal partial class RDM : Caster
{
    #region Simple Modes
    internal class RDM_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3))
                return actionID;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCDs
            if (CanWeave())
            {
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
        protected internal override Preset Preset => Preset.RDM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Scatter or Impact))
                return actionID;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCDs
            if (CanWeave())
            {
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

            if (IsEnabled(Preset.RDM_AoE_MeleeCombo))
            {
                if (ActionReady(Moulinet) && HasBattleTarget() && GetTargetDistance() < 8 &&
                    (CanMagickedSwordplay || HasEnoughManaToStart || ComboAction is EnchantedMoulinet or Moulinet or EnchantedMoulinetDeux && HasEnoughManaForCombo))
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
        protected internal override Preset Preset => Preset.RDM_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Jolt or Jolt2 or Jolt3))
                return actionID;

            #region Opener
            if (IsEnabled(Preset.RDM_Balance_Opener) && HasBattleTarget() &&
                Opener().FullOpener(ref actionID))
                return actionID;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCDs
            if (CanWeave())
            {
                if (IsEnabled(Preset.RDM_ST_MeleeCombo_GapCloser) && !InMeleeRange() &&
                    ActionReady(Corpsacorps) && TimeStoodStill >= TimeSpan.FromSeconds(RDM_ST_GapCloseCorpsacorps_Time) &&
                    (HasEnoughManaToStart || CanMagickedSwordplay)) 
                    return Corpsacorps;

                if (IsEnabled(Preset.RDM_ST_Manafication) && ActionReady(Manafication) && (EmboldenCD <= 5 || HasEmbolden) && !CanPrefulgence)
                    return Manafication;

                if (IsEnabled(Preset.RDM_ST_Embolden) && ActionReady(Embolden) && !HasEmbolden)
                    return Embolden;

                if (IsEnabled(Preset.RDM_ST_ContreSixte) && ActionReady(ContreSixte))
                    return ContreSixte;

                if (IsEnabled(Preset.RDM_ST_Fleche) && ActionReady(Fleche))
                    return Fleche;

                if (IsEnabled(Preset.RDM_ST_Engagement) && CanEngagement && (IsNotEnabled(Preset.RDM_ST_Engagement_Pooling) || PoolEngagement))
                    return Engagement;

                if (IsEnabled(Preset.RDM_ST_Corpsacorps) && CanCorps &&
                    GetTargetDistance() <= RDM_ST_Corpsacorps_Distance &&
                    TimeStoodStill >= TimeSpan.FromSeconds(RDM_ST_Corpsacorps_Time))
                    return Corpsacorps;

                if (IsEnabled(Preset.RDM_ST_Prefulgence) && CanPrefulgence)
                    return Prefulgence;

                if (IsEnabled(Preset.RDM_ST_ViceOfThorns) && CanViceOfThorns)
                    return ViceOfThorns;

                if (IsEnabled(Preset.RDM_ST_Lucid) && Role.CanLucidDream(RDM_ST_Lucid_Threshold))
                    return Role.LucidDreaming;

                if (IsEnabled(Preset.RDM_ST_Acceleration) &&
                    (CanAcceleration && GetRemainingCharges(Acceleration) > RDM_ST_Acceleration_Charges ||
                    CanAccelerationMovement && IsEnabled(Preset.RDM_ST_Acceleration_Movement)))
                    return Acceleration;

                if (IsEnabled(Preset.RDM_ST_Swiftcast) &&
                    (!IsEnabled(Preset.RDM_ST_SwiftcastMovement) && CanSwiftcast || CanSwiftcastMovement))
                    return Role.Swiftcast;
                
                if (IsEnabled(Preset.RDM_ST_Addle) && 
                    Role.CanAddle() &&
                    RaidWideCasting())
                    return Role.Addle;
                
                if (IsEnabled(Preset.RDM_ST_MagickBarrier) && 
                    NumberOfAlliesInRange(MagickBarrier) >= GetPartyMembers().Count * .75 &&
                    !HasStatusEffect(Buffs.MagickBarrier, anyOwner:true) &&
                    !JustUsed(Role.Addle, 6) &&
                    ActionReady(MagickBarrier) && RaidWideCasting())
                    return MagickBarrier;
            }
            #endregion

            #region Melee Combo and Finishers 
            if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch))
                return actionID;

            if (IsEnabled(Preset.RDM_ST_HolyFlare) && HasManaStacks)
                return UseHolyFlare(actionID);

            if (IsEnabled(Preset.RDM_ST_MeleeCombo))
            {
                if ((InMeleeRange() || IsEnabled(Preset.RDM_ST_MeleeCombo_MeleeCheck)) && (HasEnoughManaForCombo || CanMagickedSwordplay))
                {
                    if (ComboAction is Zwerchhau or EnchantedZwerchhau && LevelChecked(Redoublement))
                        return EnchantedRedoublement;
                    if (ComboAction is Riposte or EnchantedRiposte && LevelChecked(Zwerchhau))
                        return EnchantedZwerchhau;
                }

                if (IsEnabled(Preset.RDM_ST_MeleeCombo_IncludeRiposte) && ActionReady(EnchantedRiposte) &&
                    InMeleeRange() && !HasDualcast && !HasAccelerate && !HasSwiftcast &&
                    (HasEnoughManaToStart || CanMagickedSwordplay))
                    return EnchantedRiposte;
            }
            #endregion

            #region GCD Casts
            if (IsEnabled(Preset.RDM_ST_ThunderAero) && CanInstantCast)
                return UseInstantCastST(actionID);

            if (CanGrandImpact)
                return GrandImpact;
            
            if (IsEnabled(Preset.RDM_ST_VerCure) && ActionReady(Vercure) && 
                PlayerHealthPercentageHp() <= RDM_ST_VerCureThreshold &&
                (!InDuty() || InFATE() || !IsInParty()))
                return Vercure;

            if (IsEnabled(Preset.RDM_ST_FireStone))
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
        protected internal override Preset Preset => Preset.RDM_AoE_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Scatter or Impact))
                return actionID;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCDs
            if (CanWeave())
            {
                if (IsEnabled(Preset.RDM_AoE_MeleeCombo_GapCloser) && !InMeleeRange() &&
                    ActionReady(Corpsacorps) && TimeStoodStill >= TimeSpan.FromSeconds(RDM_AoE_GapCloseCorpsacorps_Time) &&
                    (HasEnoughManaToStart || CanMagickedSwordplay)) 
                    return Corpsacorps;

                if (IsEnabled(Preset.RDM_AoE_Manafication) && ActionReady(Manafication) && (EmboldenCD <= 5 || HasEmbolden) && !CanPrefulgence)
                    return Manafication;

                if (IsEnabled(Preset.RDM_AoE_Embolden) && ActionReady(Embolden) && !HasEmbolden)
                    return Embolden;

                if (IsEnabled(Preset.RDM_AoE_ContreSixte) && ActionReady(ContreSixte))
                    return ContreSixte;

                if (IsEnabled(Preset.RDM_AoE_Fleche) && ActionReady(Fleche))
                    return Fleche;

                if (IsEnabled(Preset.RDM_AoE_Engagement) && CanEngagement && (IsNotEnabled(Preset.RDM_AoE_Engagement_Pooling) || PoolEngagement))
                    return Engagement;

                if (IsEnabled(Preset.RDM_AoE_Corpsacorps) && CanCorps &&
                    GetTargetDistance() <= RDM_AoE_Corpsacorps_Distance &&
                    TimeStoodStill >= TimeSpan.FromSeconds(RDM_AoE_Corpsacorps_Time))
                    return Corpsacorps;

                if (IsEnabled(Preset.RDM_AoE_Prefulgence) && CanPrefulgence)
                    return Prefulgence;

                if (IsEnabled(Preset.RDM_AoE_ViceOfThorns) && CanViceOfThorns)
                    return ViceOfThorns;

                if (IsEnabled(Preset.RDM_AoE_Lucid) && Role.CanLucidDream(RDM_AoE_Lucid_Threshold))
                    return Role.LucidDreaming;

                if (IsEnabled(Preset.RDM_AoE_Acceleration) &&
                    (CanAcceleration && GetRemainingCharges(Acceleration) > RDM_AoE_Acceleration_Charges ||
                    CanAccelerationMovement && IsEnabled(Preset.RDM_AoE_Acceleration_Movement)))
                    return Acceleration;

                if (IsEnabled(Preset.RDM_AoE_Swiftcast) &&
                    (!IsEnabled(Preset.RDM_AoE_SwiftcastMovement) && CanSwiftcast || CanSwiftcastMovement))
                    return Role.Swiftcast;
            }
            #endregion

            #region Melee Combo and Finishers 
            if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch))
                return actionID;

            if (IsEnabled(Preset.RDM_AoE_HolyFlare) && HasManaStacks)
                return UseHolyFlare(actionID);

            if (IsEnabled(Preset.RDM_AoE_MeleeCombo))
            {
                if (ActionReady(Moulinet) &&
                    (IsNotEnabled(Preset.RDM_AoE_MeleeCombo_Target) && !HasBattleTarget() || HasBattleTarget() && GetTargetDistance() < 8) &&
                    (CanMagickedSwordplay || HasEnoughManaToStart || ComboAction is EnchantedMoulinet or Moulinet or EnchantedMoulinetDeux && HasEnoughManaForCombo))
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
            
            if (IsEnabled(Preset.RDM_AoE_VerCure) && ActionReady(Vercure) && 
                PlayerHealthPercentageHp() <= RDM_AoE_VerCureThreshold && !CanInstantCast &&
                (!InDuty() || InFATE() || !IsInParty()))
                return Vercure;            

            if (IsEnabled(Preset.RDM_AoE_ThunderAero) && !CanInstantCast)
                return UseThunderAeroAoE(actionID);

            return !LevelChecked(Scatter) ? UseInstantCastST(actionID) : actionID;
            #endregion
        }
    }
    #endregion

    #region Standalone Features

    internal class RDM_Verraise : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_Raise;

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

            if (LevelChecked(Verraise))
            {
                bool schwifty = HasStatusEffect(Role.Buffs.Swiftcast);
                if (schwifty || HasStatusEffect(Buffs.Dualcast))
                    return IsEnabled(Preset.RDM_Raise_Retarget)
                        ? Verraise.Retarget(Role.Swiftcast,
                            SimpleTarget.Stack.AllyToRaise)
                        : Verraise;
                if (IsEnabled(Preset.RDM_Raise_Vercure) &&
                    !schwifty &&
                    ActionReady(Vercure) &&
                    IsOnCooldown(Role.Swiftcast))
                    return IsEnabled(Preset.RDM_Raise_Retarget)
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
        protected internal override Preset Preset => Preset.RDM_VerAero;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Veraero or Veraero3))
                return actionID;

            if (RDM_VerAero_Options[2] && ComboAction is Scorch or Verholy or Verflare)
                return OriginalHook(Jolt);

            if (RDM_VerAero_Options[0] && HasManaStacks)
                return UseHolyFlare(actionID);

            if (RDM_VerAero_Options[1] && CanVerStone && !HasDualcast && !HasSwiftcast)
                return Verstone;

            if (RDM_VerAero_Options[3] && !HasDualcast && !HasSwiftcast && !HasManaStacks &&
                ComboAction is not (Scorch or Verholy or Verflare))
                return OriginalHook(Jolt);

            return actionID;
        }
    }

    internal class RDM_VerThunder : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_VerThunder;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Verthunder or Verthunder3))
                return actionID;

            if (RDM_VerThunder_Options[2] && ComboAction is Scorch or Verholy or Verflare)
                return OriginalHook(Jolt);

            if (RDM_VerThunder_Options[0] && HasManaStacks)
                return UseHolyFlare(actionID);

            if (RDM_VerThunder_Options[1] && CanVerFire && !HasDualcast && !HasSwiftcast)
                return Verfire;

            if (RDM_VerThunder_Options[3] && !HasDualcast && !HasSwiftcast && !HasManaStacks &&
                ComboAction is not (Scorch or Verholy or Verflare))
                return OriginalHook(Jolt);

            return actionID;
        }
    }

    internal class RDM_VerAeroAoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_VerAero2;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Veraero2)
                return actionID;

            if (RDM_VerAero2_Options[1] && ComboAction is Scorch or Verholy or Verflare)
                return OriginalHook(Impact);

            if (RDM_VerAero2_Options[0] && HasManaStacks)
                return UseHolyFlare(actionID);

            if (RDM_VerAero2_Options[2] && (HasDualcast || HasSwiftcast) && !HasManaStacks &&
                ComboAction is not (Scorch or Verholy or Verflare))
                return OriginalHook(Impact);

            return actionID;
        }
    }

    internal class RDM_VerThunderAoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_VerThunder2;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Verthunder2)
                return actionID;

            if (RDM_VerThunder2_Options[1] && ComboAction is Scorch or Verholy or Verflare)
                return OriginalHook(Impact);

            if (RDM_VerThunder2_Options[0] && HasManaStacks)
                return UseHolyFlare(actionID);

            if (RDM_VerThunder2_Options[2] && (HasDualcast || HasSwiftcast) && !HasManaStacks &&
                ComboAction is not (Scorch or Verholy or Verflare))
                return OriginalHook(Impact);

            return actionID;
        }
    }

    internal class RDM_ST_Melee_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_Riposte;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Riposte)
                return actionID;

            if (IsEnabled(Preset.RDM_Riposte_GapCloser) && ActionReady(Corpsacorps) &&
                (HasEnoughManaToStartStandalone || CanMagickedSwordplay) && !InMeleeRange())
                return Corpsacorps;

            if (IsEnabled(Preset.RDM_Riposte_Weaves) && CanWeave())
            {
                if (RDM_Riposte_Weaves_Options[0] && ActionReady(Fleche))
                    return Fleche;
                if (RDM_Riposte_Weaves_Options[1] && ActionReady(ContreSixte))
                    return ContreSixte;
                if (RDM_Riposte_Weaves_Options[2] && HasStatusEffect(Buffs.ThornedFlourish))
                    return ViceOfThorns;
                if (RDM_Riposte_Weaves_Options[3] && CanPrefulgence)
                    return Prefulgence;
                if (RDM_Riposte_Weaves_Options[4] && InMeleeRange() &&
                    GetRemainingCharges(Engagement) > RDM_Riposte_Weaves_Options_EngagementCharges)
                    return Engagement;
                if (RDM_Riposte_Weaves_Options[5] &&
                    GetRemainingCharges(Corpsacorps) > RDM_Riposte_Weaves_Options_CorpsCharges &&
                    (InMeleeRange() || GetTargetDistance() <= RDM_Riposte_Weaves_Options_Corpsacorps_Distance))
                    return Corpsacorps;
            }

            if (IsEnabled(Preset.RDM_Riposte_Finisher))
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

            if (IsEnabled(Preset.RDM_Riposte_NoWaste) && !HasEnoughManaToStartStandalone && !CanMagickedSwordplay)
                return All.SavageBlade;

            return actionID;
        }
    }

    internal class RDM_AOE_Melee_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_Moulinet;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Moulinet)
                return actionID;

            if (IsEnabled(Preset.RDM_Moulinet_GapCloser) && ActionReady(Corpsacorps) &&
                (HasEnoughManaToStartStandalone || CanMagickedSwordplay) && !InMeleeRange())
                return Corpsacorps;

            if (IsEnabled(Preset.RDM_Moulinet_Weaves) && CanWeave())
            {
                if (RDM_Moulinet_Weaves_Options[0] && ActionReady(Fleche))
                    return Fleche;
                if (RDM_Moulinet_Weaves_Options[1] && ActionReady(ContreSixte))
                    return ContreSixte;
                if (RDM_Moulinet_Weaves_Options[2] && HasStatusEffect(Buffs.ThornedFlourish))
                    return ViceOfThorns;
                if (RDM_Moulinet_Weaves_Options[3] && CanPrefulgence)
                    return Prefulgence;
                if (RDM_Moulinet_Weaves_Options[4] && InMeleeRange() &&
                    GetRemainingCharges(Engagement) > RDM_Moulinet_Weaves_Options_EngagementCharges)
                    return Engagement;
                if (RDM_Moulinet_Weaves_Options[5] &&
                    GetRemainingCharges(Corpsacorps) > RDM_Moulinet_Weaves_Options_CorpsCharges &&
                    (InMeleeRange() || GetTargetDistance() <= RDM_Moulinet_Weaves_Options_Corpsacorps_Distance))
                    return Corpsacorps;
            }

            if (IsEnabled(Preset.RDM_Moulinet_Finisher))
            {
                if (ComboAction is Scorch && LevelChecked(Resolution) || ComboAction is Verholy or Verflare && LevelChecked(Scorch))
                    return OriginalHook(Jolt);

                if (HasManaStacks)
                    return UseHolyFlare(actionID);
            }

            if (IsEnabled(Preset.RDM_Moulinet_NoWaste) &&
                ComboAction is not (Moulinet or EnchantedMoulinet or EnchantedMoulinetDeux) && !HasEnoughManaToStartStandalone && !CanMagickedSwordplay)
                return All.SavageBlade;

            return actionID;
        }
    }

    internal class RDM_CorpsDisplacement : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_CorpsDisplacement;

        protected override uint Invoke(uint actionID) =>
            actionID is Displacement
            && LevelChecked(Displacement)
            && HasTarget()
            && GetTargetDistance() >= 5 ? Corpsacorps : actionID;
    }

    internal class RDM_EmboldenProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_EmboldenProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Embolden)
                return actionID;

            if (CanViceOfThorns)
                return ViceOfThorns;

            if (IsEnabled(Preset.RDM_EmboldenManafication) && ActionReady(Manafication) &&
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
        protected internal override Preset Preset => Preset.RDM_MagickProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MagickBarrier)
                return actionID;

            if (IsEnabled(Preset.RDM_MagickBarrierAddle))
            {
                if (Role.CanAddle() && CanNotMagickBarrier ||
                    GetCooldownRemainingTime(Role.Addle) < GetCooldownRemainingTime(MagickBarrier))
                    return HasStatusEffect(Debuffs.Addle, CurrentTarget, anyOwner: true) ? All.SavageBlade : Role.Addle;
            }

            if (ActionReady(MagickBarrier) && HasStatusEffect(Buffs.MagickBarrier, anyOwner: true))
                return All.SavageBlade;

            if (IsEnabled(Preset.RDM_MagickBarrierAddle) && GetCooldownRemainingTime(Role.Addle) < GetCooldownRemainingTime(MagickBarrier))
                return Role.Addle;

            return actionID;
        }
    }

    internal class RDM_OGCDs : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDM_OGCDs;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fleche)
                return actionID;

            if (ActionReady(Fleche))
                return Fleche;

            if (RDM_OGCDs_Options[0] &&
                ActionReady(ContreSixte))
                return ContreSixte;

            if (RDM_OGCDs_Options[1] &&
                HasStatusEffect(Buffs.ThornedFlourish))
                return ViceOfThorns;

            if (RDM_OGCDs_Options[2] &&
                CanPrefulgence)
                return Prefulgence;

            if (RDM_OGCDs_Options[3] &&
                InMeleeRange() &&
                GetRemainingCharges(Engagement) > RDM_OGCDs_Options_EngagementCharges)
                return Engagement;

            if (RDM_OGCDs_Options[4] &&
                GetRemainingCharges(Corpsacorps) > RDM_OGCDs_Options_CorpsCharges &&
                (InMeleeRange() || GetTargetDistance() <= RDM_OGCDs_Options_Corpsacorps_Distance))
                return Corpsacorps;

            return RDM_OGCDs_Options[0] && GetCooldownRemainingTime(ContreSixte) < GetCooldownRemainingTime(Fleche) ? ContreSixte : actionID;
        }
    }
    #endregion 
}
