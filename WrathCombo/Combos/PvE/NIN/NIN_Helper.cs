using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.NIN.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    static NINGauge gauge = GetJobGauge<NINGauge>();
    public static FrozenSet<uint> MudraSigns = [Ten, Chi, Jin, TenCombo, ChiCombo, JinCombo];
    internal static bool STSimpleMode => IsEnabled(Preset.NIN_ST_SimpleMode);
    internal static bool AoESimpleMode => IsEnabled(Preset.NIN_AoE_SimpleMode);
    internal static bool NinjaWeave => CanWeave(.6f, 10);
    
    #region Mudra Logic
    public static uint CurrentNinjutsu => OriginalHook(Ninjutsu);
    internal static bool InMudra = false;
    internal static bool MudraPhase => WasLastAction(Ten) || WasLastAction(Chi) || WasLastAction(Jin) || WasLastAction(TenCombo) || WasLastAction(ChiCombo) || WasLastAction(JinCombo);
    internal static bool MudraReady => MudraCasting.CanCast();
    internal static uint MudraCharges => GetRemainingCharges(Ten);
    internal static bool MudraAlmostReady => MudraCharges == 1 && GetCooldownChargeRemainingTime(Ten) < 3;
    #endregion
    
    #region Ninjutsu Logic
    internal static bool HasDoton => HasStatusEffect(Buffs.Doton);
    internal static float DotonRemaining => GetStatusEffectRemainingTime(Buffs.Doton);
    internal static bool DotonStoppedMoving => TimeStoodStill >= TimeSpan.FromSeconds(DotonTimeStill);
    internal static float DotonTimeStill => AoESimpleMode ? 1.5f : NIN_AoE_AdvancedMode_Doton_TimeStill;
    
    internal static bool CanUseFumaShuriken => LevelChecked(FumaShuriken) && MudraReady;
     
    internal static bool CanUseRaiton =>  LevelChecked(Raiton) && MudraReady && 
                                          (!HasKassatsu || !NIN_ST_AdvancedMode_Ninjitsus_Options[2] && !STSimpleMode || !LevelChecked(HyoshoRanryu)) && //Use kassatsu on it if Hyosho isn't selected. 
                                           (TrickDebuff || // Buff Window
                                           !LevelChecked(Suiton) || //Dont Pool because of Suiton not learned yet
                                           GetCooldownChargeRemainingTime(Ten) < 1 || // Spend to avoid cap
                                           !NIN_ST_AdvancedMode_Raiton_Options[0] && !STSimpleMode || //Dont Pool because of Raiton Option
                                           NIN_ST_AdvancedMode_Raiton_Options[1] && !InMeleeRange() && GetCooldownChargeRemainingTime(Ten) <= TrickCD - 10); //Uptime option
    
    internal static bool CanUseKaton =>  LevelChecked(Katon) && MudraReady &&
                                         (!HasKassatsu || !NIN_AoE_AdvancedMode_Ninjitsus_Options[2] && !STSimpleMode) &&
                                         (TrickDebuff || //Buff Window
                                          !LevelChecked(Huton) || //Dont Pool because of Huton not learned yet
                                          GetCooldownChargeRemainingTime(Ten) < 1 || // Spend to avoid cap
                                          !NIN_AoE_AdvancedMode_Katon_Options[0] && !AoESimpleMode || //Dont Pool because of Raiton Option
                                          NIN_AoE_AdvancedMode_Katon_Options[1] && !InMeleeRange() && 
                                          GetCooldownChargeRemainingTime(Ten) <= TrickCD - 10); //Uptime option
    
    internal static bool CanUseDoton => LevelChecked(Doton) && MudraReady && DotonStoppedMoving &&
                                        (!HasDoton || DotonRemaining <= 2) &&
                                        (TrickDebuff || //Buff Window
                                         GetCooldownChargeRemainingTime(Ten) < 3); // Use if you have Kassatsu before you get Hosho Ranryu
    
    internal static bool CanUseSuiton => LevelChecked(Suiton) && MudraReady && !HasStatusEffect(Buffs.ShadowWalker);
    
    internal static bool CanUseHuton => LevelChecked(Huton) && MudraReady && !HasStatusEffect(Buffs.ShadowWalker);
    
    internal static bool CanUseHyoshoRanryu => LevelChecked(HyoshoRanryu) && MudraReady && HasKassatsu && 
                                               (BuffWindow || IsNotEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && !STSimpleMode || KassatsuRemaining < 3);

    internal static bool CanUseGokaMekkyaku => LevelChecked(GokaMekkyaku) && MudraReady && HasKassatsu && 
                                               (BuffWindow || IsNotEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && !STSimpleMode || KassatsuRemaining < 3);
    #endregion
    
    #region GCD Logic
    internal static bool TNArmorCrush => !MudraPhase && !OnTargetsFlank() && TargetNeedsPositionals() && Role.CanTrueNorth();
    internal static bool TNAeolianEdge => !MudraPhase && !OnTargetsRear() && TargetNeedsPositionals() && Role.CanTrueNorth();
    internal static bool CanPhantomKamaitachi => !MudraPhase && HasStatusEffect(Buffs.PhantomReady) &&
                                                 (TrickDebuff && ComboAction != GustSlash ||
                                                  !TrickDebuff);
    internal static bool CanThrowingDaggers => !MudraPhase && ActionReady(ThrowingDaggers) && HasTarget() && !InMeleeRange() &&
                                               !HasStatusEffect(Buffs.RaijuReady);
    internal static bool CanThrowingDaggersAoE => !MudraPhase && ActionReady(ThrowingDaggers) && HasTarget() && GetTargetDistance() >= 4.5 &&
                                                  !HasStatusEffect(Buffs.RaijuReady);
    internal static bool CanRaiju => !MudraPhase && HasStatusEffect(Buffs.RaijuReady);
    #endregion
    
    #region Buff Window Logic
    internal static bool TrickDisabledST => IsNotEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && !STSimpleMode;
    internal static bool TrickDisabledAoE => IsNotEnabled(Preset.NIN_AoE_AdvancedMode_TrickAttack) && !AoESimpleMode;
    internal static bool MugDisabledST => IsNotEnabled(Preset.NIN_ST_AdvancedMode_Mug) && !STSimpleMode;
    internal static bool MugDisabledAoE => IsNotEnabled(Preset.NIN_AoE_AdvancedMode_Mug) && !AoESimpleMode;
    internal static int STMugThreshold => NIN_ST_AdvancedMode_Mug_SubOption == 1 || !InBossEncounter() ? NIN_ST_AdvancedMode_Mug_Threshold : 0;
    internal static int AoEMugThreshold => NIN_AoE_AdvancedMode_Mug_SubOption == 1 || !InBossEncounter() ? NIN_AoE_AdvancedMode_Mug_Threshold : 0;
    internal static int STTrickThreshold => NIN_ST_AdvancedMode_TrickAttack_SubOption == 1 || !InBossEncounter() ? NIN_ST_AdvancedMode_TrickAttack_Threshold : 0;
    internal static int AoETrickThreshold => NIN_AoE_AdvancedMode_TrickAttack_SubOption == 1 || !InBossEncounter() ? NIN_AoE_AdvancedMode_TrickAttack_Threshold : 0;
    internal static bool BuffWindow => TrickDebuff || MugDebuff && TrickCD >= 30;
    internal static float TrickCD => GetCooldownRemainingTime(OriginalHook(TrickAttack));
    internal static float MugCD => GetCooldownRemainingTime(OriginalHook(Mug));
    
    internal static bool CanTrickST => ActionReady(OriginalHook(TrickAttack)) && NinjaWeave && CanApplyStatus(CurrentTarget, [Debuffs.TrickAttack, Debuffs.KunaisBane]) && HasStatusEffect(Buffs.ShadowWalker) && !MudraPhase &&
                                     (MugDebuff || MugCD >= 45 || MugDisabledST);
    internal static bool CanTrickAoE => ActionReady(OriginalHook(TrickAttack)) && NinjaWeave && CanApplyStatus(CurrentTarget, [Debuffs.TrickAttack, Debuffs.KunaisBane])&& HasStatusEffect(Buffs.ShadowWalker) && !MudraPhase &&
                                     (MugDebuff || MugCD >= 45 || MugDisabledAoE);
    
    internal static bool CanMugST => ActionReady(OriginalHook(Mug)) && CanApplyStatus(CurrentTarget, [Debuffs.Mug, Debuffs.Dokumori])&& CanDelayedWeave(1.25f, .6f, 10) && !MudraPhase &&
                                   (TrickCD <= 6 || TrickDisabledST) && 
                                   (LevelChecked(Dokumori) && GetTargetDistance() <= 8 ||InMeleeRange());
    internal static bool CanMugAoE => ActionReady(OriginalHook(Mug)) && CanApplyStatus(CurrentTarget, [Debuffs.Mug, Debuffs.Dokumori])&& CanDelayedWeave(1.25f, .6f, 10) && !MudraPhase &&
                                   (TrickCD <= 6 || TrickDisabledAoE) && 
                                   (LevelChecked(Dokumori) && GetTargetDistance() <= 8 ||InMeleeRange());
    
    internal static bool TrickDebuff => HasStatusEffect(Debuffs.TrickAttack, CurrentTarget) || HasStatusEffect(Debuffs.KunaisBane, CurrentTarget) || JustUsed(OriginalHook(TrickAttack));
    internal static bool MugDebuff => HasStatusEffect(Debuffs.Mug, CurrentTarget) || HasStatusEffect(Debuffs.Dokumori, CurrentTarget) || JustUsed(OriginalHook(Mug));
    #endregion
    
    #region Ninki Use Logic
    internal static bool NinkiWillOvercap => gauge.Ninki > 50;
    internal static bool CanBunshin => NinjaWeave && !MudraPhase && LevelChecked(Bunshin) && IsOffCooldown(Bunshin) && gauge.Ninki >= 50 ;
    internal static bool CanBhavacakra => NinjaWeave && gauge.Ninki >= 50 && !MudraPhase && 
                                          (!HasStatusEffect(Buffs.Higi) || BuffWindow || TrickDisabledST);
    internal static bool CanHellfrogMedium => NinjaWeave && gauge.Ninki >= 50 && LevelChecked(HellfrogMedium) && !MudraPhase &&
                                              (!HasStatusEffect(Buffs.Higi) || BuffWindow || TrickDisabledAoE);
    
    internal static bool NinkiPooling => gauge.Ninki >= NinkiPool();
    internal static int NinkiPool()
    {
        if (MugCD < 5) 
            return 60;
        if (GetCooldownRemainingTime(Bunshin) < 15)
            return 85;
        if  (TrickDebuff)
            return 50;
        if (HasStatusEffect(Buffs.Bunshin))
            return ComboAction == GustSlash ? 65: 85;
        return ComboAction == GustSlash ? 80 : 90;
    }
    #endregion
    
    #region Kassatsu, Meisui, Assassinate, TenChiJin Logic
    internal static bool HasKassatsu => HasStatusEffect(Buffs.Kassatsu);
    internal static float KassatsuRemaining => GetStatusEffectRemainingTime(Buffs.Kassatsu);
    internal static bool CanKassatsu => !MudraPhase && ActionReady(Kassatsu) && NinjaWeave &&  
                                        (TrickCD < 10 && HasStatusEffect(Buffs.ShadowWalker) ||
                                         BuffWindow || 
                                         TrickDisabledST);
    
    internal static bool CanKassatsuAoE => !MudraPhase && ActionReady(Kassatsu) && NinjaWeave &&  
                                        (TrickCD < 10 && HasStatusEffect(Buffs.ShadowWalker) ||
                                         BuffWindow || 
                                         TrickDisabledAoE);
    
    internal static bool CanMeisui => !MudraPhase && ActionReady(Meisui) && NinjaWeave && HasStatusEffect(Buffs.ShadowWalker) && 
                                      (BuffWindow || TrickDisabledST);
    internal static bool CanMeisuiAoE => !MudraPhase && ActionReady(Meisui) && NinjaWeave && HasStatusEffect(Buffs.ShadowWalker) && 
                                      (BuffWindow || TrickDisabledAoE);

    internal static bool CanAssassinate => !MudraPhase && ActionReady(OriginalHook(Assassinate)) && NinjaWeave && 
                                           (BuffWindow || TrickDisabledST);
    internal static bool CanAssassinateAoE => !MudraPhase && ActionReady(OriginalHook(Assassinate)) && NinjaWeave && 
                                           (BuffWindow || TrickDisabledAoE);

    internal static bool CanTenChiJin => !MudraPhase && !MudraAlmostReady && IsOffCooldown(TenChiJin) && LevelChecked(TenChiJin) && NinjaWeave &&
                                         (BuffWindow || TrickDisabledST);
    internal static bool CanTenChiJinAoE => !MudraPhase && !MudraAlmostReady && IsOffCooldown(TenChiJin) && LevelChecked(TenChiJin) && NinjaWeave &&
                                            (BuffWindow || TrickDisabledAoE);

    internal static bool CanTenriJindo => NinjaWeave && HasStatusEffect(Buffs.TenriJendoReady);
    #endregion
    
    #region TCJ Methods
    internal static uint STTenChiJin(uint actionID)
    {
        if (OriginalHook(Ten) == TCJFumaShurikenTen)
            return OriginalHook(Ten);
        if (OriginalHook(Chi) == TCJRaiton)
            return OriginalHook(Chi);
        return OriginalHook(Jin) == TCJSuiton ? OriginalHook(Jin) : actionID;
    }
    internal static uint AoETenChiJinDoton(uint actionID)
    {
        if (OriginalHook(Jin) == TCJFumaShurikenJin)
            return OriginalHook(Jin);
        if (OriginalHook(Ten) == TCJKaton)
            return OriginalHook(Ten);
        return OriginalHook(Chi) == TCJDoton ? OriginalHook(Chi) : actionID;
    }
    internal static uint AoETenChiJinSuiton(uint actionID)
    {
        if (OriginalHook(Chi) == TCJFumaShurikenChi)
            return OriginalHook(Chi);
        if (OriginalHook(Ten) == TCJKaton)
            return OriginalHook(Ten);
        return OriginalHook(Jin) == TCJSuiton ? OriginalHook(Jin) : actionID;
    }
    #endregion
    
    #region Mudra
    internal class MudraCasting
    {
        #region Mudra State Stuff
        
        public MudraState CurrentMudra = MudraState.None;
        
        public enum MudraState
        {
            None,
            CastingFumaShuriken,
            CastingKaton,
            CastingRaiton,
            CastingHuton,
            CastingDoton,
            CastingSuiton,
            CastingGokaMekkyaku,
            CastingHyoshoRanryu
        }
        
        public bool ContinueCurrentMudra(ref uint actionID)
        {
            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 2 && CurrentNinjutsu == Ninjutsu)
            {
                InMudra = false;
                ActionWatching.LastAction = 0;
                CurrentMudra = MudraState.None;
                return false;
            }

            if (ActionWatching.LastAction == FumaShuriken ||
                ActionWatching.LastAction == Katon ||
                ActionWatching.LastAction == Raiton ||
                ActionWatching.LastAction == Hyoton ||
                ActionWatching.LastAction == Huton ||
                ActionWatching.LastAction == Doton ||
                ActionWatching.LastAction == Suiton ||
                ActionWatching.LastAction == GokaMekkyaku ||
                ActionWatching.LastAction == HyoshoRanryu)
            {
                CurrentMudra = MudraState.None;
                InMudra = false;
            }

            return CurrentMudra switch
            {
                MudraState.None => false,
                MudraState.CastingFumaShuriken => CastFumaShuriken(ref actionID),
                MudraState.CastingKaton => CastKaton(ref actionID),
                MudraState.CastingRaiton => CastRaiton(ref actionID),
                MudraState.CastingHuton => CastHuton(ref actionID),
                MudraState.CastingDoton => CastDoton(ref actionID),
                MudraState.CastingSuiton => CastSuiton(ref actionID),
                MudraState.CastingGokaMekkyaku => CastGokaMekkyaku(ref actionID),
                MudraState.CastingHyoshoRanryu => CastHyoshoRanryu(ref actionID),
                _ => false
            };
        }
        #endregion
        
        #region Mudra Cast Check
        public static bool CanCast()
        {
            if (InMudra) return true;
            
            if (GetCooldown(GustSlash).CooldownTotal == 0.5) return true;

            if (GetRemainingCharges(Ten) == 0 &&
                !HasStatusEffect(Buffs.Mudra) &&
                !HasStatusEffect(Buffs.Kassatsu))
                return false;
            return true;
        }
        #endregion
        
        #region Fuma Shuriken
        public bool CastFumaShuriken(ref uint actionID) // Ten
        {
            if (CurrentMudra is MudraState.None or MudraState.CastingFumaShuriken)
            {
                // Reset State
                if (!CanCast() || ActionWatching.LastAction == FumaShuriken) 
                {
                    CurrentMudra = MudraState.None;
                    return false;
                }
                // Finish the Mudra
                if (ActionWatching.LastAction is Ten or TenCombo) 
                {
                    actionID = OriginalHook(Ninjutsu);
                    return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingFumaShuriken;
                actionID = OriginalHook(Ten);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
        
        #region Raiton
        public bool CastRaiton(ref uint actionID)  // Ten Chi
        {
            if (Raiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingRaiton)
            {
                // Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Ten or TenCombo or Jin or JinCombo:
                        actionID = OriginalHook(Chi);
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingRaiton;
                actionID = OriginalHook(Ten);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
        
        #region Suiton
        public bool CastSuiton(ref uint actionID)  //Ten Chi Jin
        {
            if (Suiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingSuiton)
            {
                //Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Ten or TenCombo:
                        actionID = OriginalHook(Chi);
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout Hyoten
                        actionID = OriginalHook(Jin);
                        return true;
                    case Jin or JinCombo: //Jin == Bailout Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingSuiton;
                actionID = OriginalHook(Ten);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Hyosho Ranryu 
        public bool CastHyoshoRanryu(ref uint actionID) // Ten Jin
        {
            if (HyoshoRanryu.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoshoRanryu)
            {
                //Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Ten or TenCombo or Chi or ChiCombo:
                        actionID = JinCombo;
                        return true;
                    case Jin or JinCombo: //Jin == Bailout to Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingHyoshoRanryu;
                actionID = OriginalHook(Ten);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
        
        #region Katon
        public bool CastKaton(ref uint actionID) // Jin Ten
        {
            if (Katon.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingKaton)
            {
                //Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Jin or JinCombo or Chi or ChiCombo:
                        actionID = OriginalHook(Ten);
                        return true;
                    case Ten or TenCombo: //Ten == Bailout to Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingKaton;
                actionID = OriginalHook(Jin);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
        
        #region Doton
        public bool CastDoton(ref uint actionID) // Jin Ten Chi
        {
            if (Doton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingDoton)
            {
                //Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Jin or JinCombo: 
                        actionID = OriginalHook(Ten);
                        return true;
                    case Ten or TenCombo: // Ten == Bailout to Raiton
                        actionID = OriginalHook(Chi);
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingDoton;
                actionID = OriginalHook(Jin);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
        
        #region Huton
        public bool CastHuton(ref uint actionID) // Jin Chi Ten
        {
            if (Huton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHuton)
            {
                //Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Jin or JinCombo:
                        actionID = OriginalHook(Chi);
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout katon
                        actionID = OriginalHook(Ten);
                        return true;
                    case Ten or TenCombo: // Ten == Bailout to Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingHuton;
                actionID = OriginalHook(Jin);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
        
        #region Goka Mekkyaku
        public bool CastGokaMekkyaku(ref uint actionID) // Jin Ten
        {
            if (GokaMekkyaku.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingGokaMekkyaku)
            {
                //Finish the Mudra
                switch (ActionWatching.LastAction)
                {
                    case Jin or JinCombo or Chi or ChiCombo:
                        actionID = OriginalHook(Ten);
                        return true;
                    case Ten or TenCombo: // Ten == Bailout to Fuma
                        actionID = OriginalHook(Ninjutsu);
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingGokaMekkyaku;
                actionID = OriginalHook(Jin);
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
    }
    #endregion
    
    #region Mudra Standalone Logic
     // Single Target
    internal static uint UseFumaShuriken(uint actionId)
    {
        return OriginalHook(Ninjutsu) == FumaShuriken ? OriginalHook(Ninjutsu) : Ten;
    }
    internal static uint UseRaiton(uint actionId) // Ten Chi
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu) 
            return HasKassatsu ? TenCombo : Ten;
        return OriginalHook(Ninjutsu) == FumaShuriken &&
               ActionWatching.LastAction is TenCombo or Ten or JinCombo or Jin 
            ? ChiCombo 
            : OriginalHook(Ninjutsu);
    }
    internal static uint UseHyoshoRanryu(uint actionId) // Ten Jin
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu)
            return TenCombo;
        return OriginalHook(Ninjutsu) == FumaShuriken &&
               ActionWatching.LastAction is TenCombo 
            ? JinCombo 
            : OriginalHook(Ninjutsu);
    }
    internal static uint UseSuiton(uint actionId) // Ten Chi Jin
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu)
            return Ten;
        if (ActionWatching.LastAction is Ten)
            return ChiCombo;
        return ActionWatching.LastAction is ChiCombo ? JinCombo : OriginalHook(Ninjutsu);
    }
    //Multi Target
    internal static uint UseGokaMekkyaku(uint actionId) // Chi Ten
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu)
            return HasKassatsu ? ChiCombo : Chi;
        return OriginalHook(Ninjutsu) == FumaShuriken && 
               ActionWatching.LastAction is Chi or ChiCombo 
            ? TenCombo 
            : OriginalHook(Ninjutsu);
    }
    internal static uint UseKaton(uint actionId) // Chi Ten
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu)
            return HasKassatsu ? ChiCombo : Chi;
        return OriginalHook(Ninjutsu) == FumaShuriken && 
               ActionWatching.LastAction is Jin or JinCombo or ChiCombo or Chi
            ? TenCombo 
            : OriginalHook(Ninjutsu);
    }
    internal static uint UseDoton(uint actionId)  //Jin Ten Chi
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu)
            return HasKassatsu ? JinCombo : Jin;
        if (ActionWatching.LastAction is Jin or JinCombo)
            return TenCombo;
        return ActionWatching.LastAction is TenCombo ? ChiCombo : OriginalHook(Ninjutsu);
    }

    internal static uint UseHuton(uint actionId) // Jin Chi Ten
    {
        if (OriginalHook(Ninjutsu) == Ninjutsu)
            return Chi;
        if (ActionWatching.LastAction is Chi or ChiCombo)
            return JinCombo;
        return ActionWatching.LastAction is JinCombo ? TenCombo : Huton;
    }
    #endregion

    #region Opener
    internal static NINOpenerMaxLevel4thGCDKunai Opener1 = new();
    internal static NINOpenerMaxLevel3rdGCDDokumori Opener2 = new();
    internal static NINOpenerMaxLevel3rdGCDKunai Opener3 = new();
    
    internal static WrathOpener Opener()
    {
        if (IsEnabled(Preset.NIN_ST_AdvancedMode))
        {
            if (NIN_Adv_Opener_Selection == 0 && Opener1.LevelChecked)
                return Opener1;
            if (NIN_Adv_Opener_Selection == 1 && Opener2.LevelChecked)
                return Opener2;
            if (NIN_Adv_Opener_Selection == 2 && Opener3.LevelChecked)
                return Opener3;
        }

        return Opener1.LevelChecked ? Opener1 : WrathOpener.Dummy;
    }
     internal class NINOpenerMaxLevel4thGCDKunai : WrathOpener
    {
        //4th GCD Kunai
        public override List<uint> OpenerActions { get; set; } =
        [
            Chi, //1
            TenCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            GustSlash, //7
            Dokumori, //8
            Bunshin, //9
            PhantomKamaitachi, //10
            ArmorCrush, //11
            KunaisBane, //12
            ChiCombo, //13
            JinCombo, //14
            HyoshoRanryu, //15
            DreamWithinADream, //16
            Ten, //17
            ChiCombo, //18
            Raiton, //19
            TenChiJin, //20
            TCJFumaShurikenTen, //21
            TCJRaiton, //22
            TCJSuiton, //23
            Meisui, //24
            FleetingRaiju, //25
            ZeshoMeppo, //26
            TenriJendo, //27
            FleetingRaiju, //28
            Bhavacakra, //29
            Ten, //30
            ChiCombo, //31
            Raiton, //32
            FleetingRaiju, //33
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            12
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData? ContentCheckConfig => NIN_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }

    internal class NINOpenerMaxLevel3rdGCDDokumori : WrathOpener
    {
        //3rd GCD Dokumori
        public override List<uint> OpenerActions { get; set; } =
        [
            Chi, //1
            TenCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            GustSlash, //7
            ArmorCrush, //8
            Dokumori, //9
            Bunshin, //10
            PhantomKamaitachi, //11
            KunaisBane, //12
            ChiCombo, //13
            JinCombo, //14
            HyoshoRanryu, //15
            DreamWithinADream, //16
            Ten, //17
            ChiCombo, //18
            Raiton, //19
            TenChiJin, //20
            TCJFumaShurikenTen, //21
            TCJRaiton, //22
            TCJSuiton, //23
            Meisui, //24
            FleetingRaiju, //25
            ZeshoMeppo, //26
            TenriJendo, //27
            FleetingRaiju, //28
            Ten, //29
            ChiCombo, //30
            Raiton, //31
            FleetingRaiju, //32
            Bhavacakra, //33
            SpinningEdge //34
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            12
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData? ContentCheckConfig => NIN_Balance_Content;
        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }

    internal class NINOpenerMaxLevel3rdGCDKunai : WrathOpener
    {
        //3rd GCD Kunai
        public override List<uint> OpenerActions { get; set; } =
        [
            Chi, //1
            TenCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            GustSlash, //7
            Dokumori, //8
            Bunshin, //9
            PhantomKamaitachi, //10
            KunaisBane, //11
            ChiCombo, //12
            JinCombo, //13
            HyoshoRanryu, //14
            DreamWithinADream, //15
            Ten, //16
            ChiCombo, //17
            Raiton, //18
            TenChiJin, //19
            TCJFumaShurikenTen, //20
            TCJRaiton, //21
            TCJSuiton, //22
            Meisui, //23
            FleetingRaiju, //24
            ZeshoMeppo, //25
            TenriJendo, //26
            FleetingRaiju, //27
            ArmorCrush, //28
            Bhavacakra, //29
            Ten, //30
            ChiCombo, //31
            Raiton, //32
            FleetingRaiju, //33
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            11
        ];
        internal override UserData? ContentCheckConfig => NIN_Balance_Content;
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }
    #endregion
    
    #region ID's

    public const uint
        SpinningEdge = 2240,
        ShadeShift = 2241,
        GustSlash = 2242,
        Hide = 2245,
        Assassinate = 2246,
        ThrowingDaggers = 2247,
        Mug = 2248,
        DeathBlossom = 2254,
        AeolianEdge = 2255,
        TrickAttack = 2258,
        Shukuchi = 2262,
        Kassatsu = 2264,
        ArmorCrush = 3563,
        DreamWithinADream = 3566,
        TenChiJin = 7403,
        Bhavacakra = 7402,
        HakkeMujinsatsu = 16488,
        Meisui = 16489,
        Bunshin = 16493,
        PhantomKamaitachi = 25774,
        ForkedRaiju = 25777,
        FleetingRaiju = 25778,
        HellfrogMedium = 7401,
        HollowNozuchi = 25776,
        TenriJendo = 36961,
        KunaisBane = 36958,
        ZeshoMeppo = 36960,
        Dokumori = 36957,

        //Mudras
        Ninjutsu = 2260,
        Rabbit = 2272,

        //-- initial state mudras (the ones with charges)
        Ten = 2259,
        Chi = 2261,
        Jin = 2263,

        //-- mudras used for combos (the ones used while you have the mudra buff)
        TenCombo = 18805,
        ChiCombo = 18806,
        JinCombo = 18807,

        //Ninjutsu
        FumaShuriken = 2265,
        Hyoton = 2268,
        Doton = 2270,
        Katon = 2266,
        Suiton = 2271,
        Raiton = 2267,
        Huton = 2269,
        GokaMekkyaku = 16491,
        HyoshoRanryu = 16492,

        //TCJ Jutsus
        TCJFumaShurikenTen = 18873,
        TCJFumaShurikenChi = 18874,
        TCJFumaShurikenJin = 18875,
        TCJKaton = 18876,
        TCJRaiton = 18877,
        TCJHyoton = 18878,
        TCJHuton = 18879,
        TCJDoton = 18880,
        TCJSuiton = 18881;

    public static class Buffs
    {
        public const ushort
            Mudra = 496,
            Kassatsu = 497,
            Higi = 3850,
            TenriJendoReady = 3851,
            ShadowWalker = 3848,
            Hidden = 614,
            TenChiJin = 1186,
            AssassinateReady = 1955,
            RaijuReady = 2690,
            PhantomReady = 2723,
            Meisui = 2689,
            Doton = 501,
            Bunshin = 1954;
    }

    public static class Debuffs
    {
        public const ushort
            Dokumori = 3849,
            TrickAttack = 3254,
            KunaisBane = 3906,
            Mug = 638;
    }

    public static class Traits
    {
        public const uint
            EnhancedKasatsu = 250,
            MugMastery = 585;
    }

    #endregion
}

