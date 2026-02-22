using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.SMN.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    #region ID's

    public const float CooldownThreshold = 0.5f;

    public const uint
        // Summons
        SummonRuby = 25802,
        SummonTopaz = 25803,
        SummonEmerald = 25804,

        SummonIfrit = 25805,
        SummonTitan = 25806,
        SummonGaruda = 25807,

        SummonIfrit2 = 25838,
        SummonTitan2 = 25839,
        SummonGaruda2 = 25840,

        SummonCarbuncle = 25798,

        // Summon abilities
        Gemshine = 25883,
        PreciousBrilliance = 25884,
        DreadwyrmTrance = 3581,

        // Summon Ruins
        RubyRuin1 = 25808,
        RubyRuin2 = 25811,
        RubyRuin3 = 25817,
        TopazRuin1 = 25809,
        TopazRuin2 = 25812,
        TopazRuin3 = 25818,
        EmeralRuin1 = 25810,
        EmeralRuin2 = 25813,
        EmeralRuin3 = 25819,

        // Summon Outbursts
        Outburst = 16511,
        RubyOutburst = 25814,
        TopazOutburst = 25815,
        EmeraldOutburst = 25816,

        // Summon single targets
        RubyRite = 25823,
        TopazRite = 25824,
        EmeraldRite = 25825,

        // Summon AoEs
        RubyCata = 25832,
        TopazCata = 25833,
        EmeraldCata = 25834,

        // Summon Astral Flows
        CrimsonCyclone = 25835,     // Dash
        CrimsonStrike = 25885,      // Melee
        MountainBuster = 25836,
        Slipstream = 25837,

        // Demi summons
        SummonBahamut = 7427,
        SummonPhoenix = 25831,
        SummonSolarBahamut = 36992,

        // Demi summon abilities
        AstralImpulse = 25820,      // Single target Bahamut GCD
        AstralFlare = 25821,        // AoE Bahamut GCD
        Deathflare = 3582,          // Damage oGCD Bahamut
        EnkindleBahamut = 7429,

        FountainOfFire = 16514,     // Single target Phoenix GCD
        BrandOfPurgatory = 16515,   // AoE Phoenix GCD
        Rekindle = 25830,           // Healing oGCD Phoenix
        EnkindlePhoenix = 16516,

        UmbralImpulse = 36994,          //Single target Solar Bahamut GCD
        UmbralFlare = 36995,            //AoE Solar Bahamut GCD
        Sunflare = 36996,               //Damage oGCD Solar Bahamut
        EnkindleSolarBahamut = 36998,
        LuxSolaris = 36997,             //Healing oGCD Solar Bahamut

        // Shared summon abilities
        AstralFlow = 25822,

        // Summoner GCDs
        Ruin = 163,
        Ruin2 = 172,
        Ruin3 = 3579,
        Ruin4 = 7426,
        Tridisaster = 25826,

        // Summoner AoE
        RubyDisaster = 25827,
        TopazDisaster = 25828,
        EmeraldDisaster = 25829,

        // Summoner oGCDs
        EnergyDrain = 16508,
        Fester = 181,
        EnergySiphon = 16510,
        Painflare = 3578,
        Necrotize = 36990,
        SearingFlash = 36991,
        Exodus = 36999,

        // Revive
        Resurrection = 173,

        // Buff 
        RadiantAegis = 25799,
        Aethercharge = 25800,
        SearingLight = 25801;

    public static class Buffs
    {
        public const ushort
            FurtherRuin = 2701,
            GarudasFavor = 2725,
            TitansFavor = 2853,
            IfritsFavor = 2724,
            EverlastingFlight = 16517,
            SearingLight = 2703,
            RubysGlimmer = 3873,
            RefulgentLux = 3874,
            CrimsonStrike = 4403;
    }

    public static class Traits
    {
        public const ushort
            EnhancedDreadwyrmTrance = 178,
            RuinMastery3 = 476,
            EnhancedBahamut = 619;
    }

    #endregion

    #region Variables
    internal static readonly List<uint>
        AllRuinsList = [Ruin, Ruin2, Ruin3],
        NotRuin3List = [Ruin, Ruin2];

    internal static SMNGauge Gauge => GetJobGauge<SMNGauge>();

    internal static bool IsIfritAttuned => Gauge.AttunementType is SummonAttunement.Ifrit;
    internal static bool IsTitanAttuned => Gauge.AttunementType is SummonAttunement.Titan;
    internal static bool IsGarudaAttuned => Gauge.AttunementType is SummonAttunement.Garuda;
    internal static bool CanSummonEgi => Gauge.IsTitanReady || Gauge.IsGarudaReady || Gauge.IsIfritReady;
    internal static bool GemshineReady => Gauge.AttunementCount > 0;
    internal static bool IsAttunedAny => IsIfritAttuned || IsTitanAttuned || IsGarudaAttuned;
    internal static bool IsDreadwyrmTranceReady => !LevelChecked(SummonBahamut) && IsBahamutReady;
    internal static bool IsBahamutReady => !IsPhoenixReady && !IsSolarBahamutReady;
    internal static bool IsPhoenixReady => Gauge.AetherFlags.HasFlag((AetherFlags)4) && !Gauge.AetherFlags.HasFlag((AetherFlags)8);
    internal static bool IsSolarBahamutReady => Gauge.AetherFlags.HasFlag((AetherFlags)8) || Gauge.AetherFlags.HasFlag((AetherFlags)12);
    internal static bool DemiExists => CurrentDemiSummon is not DemiSummon.None;
    internal static bool DemiNone => CurrentDemiSummon is DemiSummon.None;
    internal static bool DemiNotPheonix => CurrentDemiSummon is not DemiSummon.Phoenix;
    internal static bool DemiPheonix => CurrentDemiSummon is DemiSummon.Phoenix;
    internal static bool SearingBurstDriftCheck => SearingCD >=3 && SearingCD <=8;
    internal static bool SummonerWeave => CanWeave();
    internal static float SearingCD => GetCooldownRemainingTime(SearingLight);
   
    #endregion

    #region Carbuncle Summoner
    private static DateTime SummonTime
    {
        get
        {
            if (HasPetPresent())
                return field = DateTime.Now.AddSeconds(1);

            return field;
        }
    }
    public static bool NeedToSummon => DateTime.Now > SummonTime && !HasPetPresent() && ActionReady(SummonCarbuncle);
    #endregion

    #region Demi Summon Detector
    internal static DemiSummon CurrentDemiSummon
    {
        get
        {
            if (Gauge.SummonTimerRemaining > 0 && Gauge.AttunementTimerRemaining == 0)
            {
                if (IsDreadwyrmTranceReady) return DemiSummon.Dreadwyrm;
                if (IsBahamutReady) return DemiSummon.Bahamut;
                if (IsPhoenixReady) return DemiSummon.Phoenix;
                if (IsSolarBahamutReady) return DemiSummon.SolarBahamut;
            }
            return DemiSummon.None;
        }
    }

    internal enum DemiSummon
    {
        None,
        Dreadwyrm,
        Bahamut,
        Phoenix,
        SolarBahamut
    }
    #endregion

    #region Egi Priority
    public static int GetMatchingConfigST(
        int i,
        out uint action,
        out bool enabled)
    {      
        switch (i)
        {
            case 0:
                action = OriginalHook(SummonTopaz);

                enabled = IsEnabled(Preset.SMN_ST_Advanced_Combo_Titan) && Gauge.IsTitanReady;
                return 0;

            case 1:
                action = OriginalHook(SummonEmerald);

                enabled = IsEnabled(Preset.SMN_ST_Advanced_Combo_Garuda) && Gauge.IsGarudaReady;
                return 0;

            case 2:
                action = OriginalHook(SummonRuby);

                enabled = IsEnabled(Preset.SMN_ST_Advanced_Combo_Ifrit) && Gauge.IsIfritReady;
                return 0;
        }

        enabled = false;
        action = 0;

        return 0;
    }

    public static int GetMatchingConfigAoE(
        int i,
        out uint action,
        out bool enabled)
    {       
        switch (i)
        {
            case 0:
                action = OriginalHook(SummonTopaz);

                enabled = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Titan) && Gauge.IsTitanReady;
                return 0;
            case 1:
                action = OriginalHook(SummonEmerald);

                enabled = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Garuda) && Gauge.IsGarudaReady;
                return 0;
            case 2:
                action = OriginalHook(SummonRuby);

                enabled = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Ifrit) && Gauge.IsIfritReady;
                return 0;
        }

        enabled = false;
        action = 0;

        return 0;
    }
    #endregion   
    
    #region Rotation
    
    #region Flag Stuff
    [Flags]
    private enum Combo
    {
        // Target-type for combo
        ST = 1 << 0, // 1
        AoE = 1 << 1, // 2

        // Complexity of combo
        Adv = 1 << 2, // 4
        Simple = 1 << 3, // 8
        Basic = 1 << 4, // 16
    }
    
    /// <summary>
    ///     Checks whether a given preset is enabled, and the flags match it.
    /// </summary>
    private static bool IsSTEnabled(Combo flags, Preset preset) =>
        flags.HasFlag(Combo.ST) && IsEnabled(preset);

    /// <summary>
    ///     Checks whether a given preset is enabled, and the flags match it.
    /// </summary>
    private static bool IsAoEEnabled(Combo flags, Preset preset) =>
        flags.HasFlag(Combo.AoE) && IsEnabled(preset);
    #endregion
    
    #region OGCD Spells

    private static bool TryOGCDSpells(Combo flags, ref uint actionID)
    {
        #region Enables
        bool demiSummonsAttacksEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks);

        bool searingLightEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_SearingLight) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_SearingLight);

        bool SearingLightBurstEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_AoE_Advanced_Combo_SearingLight_Burst) ||
            IsAoEEnabled(flags, Preset.SMN_ST_Advanced_Combo_SearingLight_Burst);

        bool energyDrainEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_EDFester) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_ESPainflare);

        bool ogcdPoolingEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_oGCDPooling) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_oGCDPooling);
        
        bool rekindleEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle);
        
        bool searingFlashEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_SearingFlash) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_SearingFlash);
        
        bool LucidDreamingEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_Lucid) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_Lucid);
        #endregion
        
        #region Configs
        int lucidThreshold =
            flags.HasFlag(Combo.Simple) ? 6500 : 
            flags.HasFlag(Combo.ST) ? SMN_ST_Lucid : SMN_AoE_Lucid;
        #endregion

        #region Emergency Demi Attacks
        //Checks if you ran out of time waiting for demi attack ogcds to weave and pushes them out asap. Frankly, this should never happen.
        if (demiSummonsAttacksEnabled && DemiExists && Gauge.SummonTimerRemaining <= 2500) 
        {
            if (ActionReady(OriginalHook(EnkindleBahamut)))
            {
                actionID = OriginalHook(EnkindleBahamut);
                return true;
            }

            if (ActionReady(AstralFlow) && DemiNotPheonix)
            {
                actionID = OriginalHook(AstralFlow);
                return true;
            }
        }
        #endregion
        
        if (SummonerWeave)
        {
            #region Searing Light
            if (searingLightEnabled && ActionReady(SearingLight) && !HasStatusEffect(Buffs.SearingLight, anyOwner: true))
            {
                if (SearingLightBurstEnabled && TraitLevelChecked(Traits.EnhancedDreadwyrmTrance)) //Burst window is enabled so you wait for the demi
                {
                    if (DemiExists)
                    {
                        actionID = SearingLight;
                        return true;
                    }
                }
                else if (!ActionReady(OriginalHook(Aethercharge))) //Burst window is not enabled, won't wait for demi unless demi is already available
                {
                    actionID = SearingLight;
                    return true;
                }
            }
            #endregion
            
            #region Energy Drain / Energy Siphon
            if (energyDrainEnabled && !Gauge.HasAetherflowStacks && ActionReady(EnergyDrain))
            {
                //Fire asap to get charges if pooling is not enabled
                //Too low level for searing
                //Searing cd is over 30 seconds (used for the 1 min)
                //Searing light is active
                if (!ogcdPoolingEnabled || !LevelChecked(SearingLight) || SearingCD > 30 || HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                {
                    if (flags.HasFlag(Combo.ST))
                    {
                        actionID = OriginalHook(EnergyDrain);
                        return true;
                    }
                    if (flags.HasFlag(Combo.AoE))
                    {
                        actionID = OriginalHook(EnergySiphon);
                        return true;
                    }
                }
            }
            #endregion
            
            #region Demi Summon Attacks
            
            if (demiSummonsAttacksEnabled && DemiExists && !JustUsed(SearingLight, 1.5f) &&
                (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || //Searing is active
                 SearingCD > Gauge.SummonTimerRemaining / 1000f + GCDTotal))  //There is not enough time left in demi phase for searing to happen
            {
                if (ActionReady(OriginalHook(EnkindleBahamut)))
                {
                    actionID = OriginalHook(EnkindleBahamut);
                    return true;
                }

                if (ActionReady(AstralFlow) && DemiNotPheonix)
                {
                    actionID = OriginalHook(AstralFlow);
                    return true;
                }
                
                if (rekindleEnabled && ActionReady(AstralFlow) && DemiPheonix)
                {
                    actionID = Rekindle;
                    return true;
                }
            }
            #endregion
            
            #region Fester and Painflare
            if (energyDrainEnabled && ActionReady(Fester) && !HasStatusEffect(Buffs.TitansFavor))
            {
                //Fire asap without pooling
                //Too low level for Searing Light
                //You have Searing Light
                if (!ogcdPoolingEnabled || !LevelChecked(SearingLight) || HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                {
                    if (flags.HasFlag(Combo.ST))
                    {
                        actionID = OriginalHook(Fester);
                        return true;
                    }
                    if (flags.HasFlag(Combo.AoE))
                    {
                        actionID = OriginalHook(Painflare);
                        return true;
                    }
                }
            }
            #endregion
            
            #region Searing Flash
            if (searingFlashEnabled && HasStatusEffect(Buffs.RubysGlimmer))
            {
                actionID = SearingFlash;
                return true;
            }
            #endregion
            
            #region Lucid Dreaming
            if (LucidDreamingEnabled && Role.CanLucidDream(lucidThreshold))
            {
                actionID = Role.LucidDreaming;
                return true;
            }
            #endregion
        }
        return false;
    }
    #endregion
    
    #region Mitigation
    private static bool TryMitigation(Combo flags, ref uint actionID)
    {
        #region Enables
        bool luxSolarisEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris);
        
        bool radiantAegisEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_Radiant) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_Radiant);
        
        bool addleEnabled =
            flags.HasFlag(Combo.Simple) || IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_Addle);
        
        #endregion

        if (SummonerWeave)
        {
            #region Lux Solaris
            
            if (luxSolarisEnabled && ActionReady(LuxSolaris) &&
                (PlayerHealthPercentageHp() < 100 || //uses early if you drop below max health indicating raid damage
                 GetStatusEffectRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)) // use before it runs out, hopefully tanks could get some at least
            {
                actionID = OriginalHook(LuxSolaris);
                return true;
            }
            #endregion
            
            #region Radiant Aegis Overcap
            if (radiantAegisEnabled && 
                !HasStatusEffect(Buffs.SearingLight) && !HasStatusEffect(Buffs.TitansFavor) && // Dont use in window or when titan needs to do the mountainbuster
                GetRemainingCharges(RadiantAegis) == 2 && ActionReady(RadiantAegis)) // The shield is super long so no waiting on raidwide
            {
                actionID = RadiantAegis;
                return true;
            }
            #endregion
            
            #region Addle
            if (addleEnabled && Role.CanAddle() && GroupDamageIncoming())
            {
                actionID = Role.Addle;
                return true;
            }
            #endregion
        }
        return false;
    }
    #endregion
    
    #region GCD Spells
    private static bool TrySummonSpells(Combo flags, ref uint actionID)
    {
        #region Enables
        bool demiSummonEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_DemiSummons) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_DemiSummons);
        
        bool searingLightBurstEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_SearingLight_Burst) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_SearingLight_Burst);
        
        bool egiAstralFlowEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_Egi_AstralFlow) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_Egi_AstralFlow);
        
        bool egiSummonAttacksEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks);
        
        bool swiftcastEgiEnabled = 
            !flags.HasFlag(Combo.Simple) &&
            (IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi));
        
        bool emeraldToRuinBeforeMasteryEnabled = 
            !flags.HasFlag(Combo.Simple) && IsSTEnabled(flags, Preset.SMN_ST_Ruin3_Emerald_Ruin3);
        
        bool ruin4Enabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.SMN_ST_Advanced_Combo_Ruin4) ||
            IsAoEEnabled(flags, Preset.SMN_AoE_Advanced_Combo_Ruin4);
        #endregion
        
        #region Configs
        bool mountainBusterEnabled = 
            flags.HasFlag(Combo.Simple) || (flags.HasFlag(Combo.ST) ? SMN_ST_Egi_AstralFlow[0] : SMN_AoE_Egi_AstralFlow[0]);
        
        bool slipstreamEnabled = 
            flags.HasFlag(Combo.Simple) || (flags.HasFlag(Combo.ST) ? SMN_ST_Egi_AstralFlow[2] : SMN_AoE_Egi_AstralFlow[2]);
        
        bool IfritAstralFlowCyclone = 
            flags.HasFlag(Combo.Simple) || (flags.HasFlag(Combo.ST) ? SMN_ST_Egi_AstralFlow[1] : SMN_AoE_Egi_AstralFlow[1]);
        
        bool IfritAstralFlowStrike = 
            flags.HasFlag(Combo.Simple) || (flags.HasFlag(Combo.ST) ? SMN_ST_Egi_AstralFlow[3] : SMN_AoE_Egi_AstralFlow[3]);

        float simpleSTCrimsonCycloneMeleeDistance = 
            SMN_ST_Simple_Combo_Gapclose == 0 ? 3 : 25; //simple st safe mode 3y otherwise 25y
        float simpleAoECrimsonCycloneMeleeDistance = 
            SMN_AoE_Simple_Combo_Gapclose == 0 ? 3 : 25; //simple aoe safe mode 3y otherwise 25y
        
        float crimsonCycloneMeleeDistance= 
            flags.HasFlag(Combo.Simple) && flags.HasFlag(Combo.ST) ? simpleSTCrimsonCycloneMeleeDistance: 
            flags.HasFlag(Combo.Simple) && flags.HasFlag(Combo.AoE) ? simpleAoECrimsonCycloneMeleeDistance: 
            flags.HasFlag(Combo.ST) ? SMN_ST_CrimsonCycloneMeleeDistance : SMN_AoE_CrimsonCycloneMeleeDistance;
        
        int swiftcastPhase = 
            flags.HasFlag(Combo.ST) ? SMN_ST_SwiftcastPhase : SMN_AoE_SwiftcastPhase;
            
        #endregion
        
        #region Call Demi Summon
        if (demiSummonEnabled && PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
        {
            // If burst window is enabled, checks the cooldown of searing light and throws in an extra ruin if needed
            // Drift mitigation for people with slightly higher spellspeed. (gcd < 2.48)
            if (searingLightBurstEnabled && SearingBurstDriftCheck) 
            {
                actionID = OriginalHook(Ruin);
            }
            else 
            {
                actionID = OriginalHook(Aethercharge);
            }
            return true;
        }
        #endregion
        
        #region Titan Phase
        if (IsTitanAttuned || OriginalHook(AstralFlow) is MountainBuster) //Titan attunement ends before last mountain buster
        {
            if (egiAstralFlowEnabled && mountainBusterEnabled && ActionReady(AstralFlow) && SummonerWeave)
            {
                actionID = OriginalHook(AstralFlow);
                return true;
            }
            if (egiSummonAttacksEnabled && GemshineReady)
            {
                if (flags.HasFlag(Combo.ST))
                {
                    actionID = OriginalHook(Gemshine);
                    return true;
                }
                if (flags.HasFlag(Combo.AoE))
                {
                    actionID = OriginalHook(PreciousBrilliance);
                    return true;
                }
            }
        }
        #endregion
        
        #region Garuda Phase
        if (IsGarudaAttuned || OriginalHook(AstralFlow) is Slipstream)
        {
            if (egiAstralFlowEnabled && slipstreamEnabled && HasStatusEffect(Buffs.GarudasFavor))
            {
                if (swiftcastEgiEnabled && swiftcastPhase is 1 or 3 && Role.CanSwiftcast()) // Forced Swiftcast option
                {
                    actionID = Role.Swiftcast;
                    return true;
                }
                if (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast))
                {
                    actionID = OriginalHook(AstralFlow);
                    return true;
                }
            }

            #region Special Ruin 3 rule lvl 54 - 72 (ST ADV Only)
            // Use Ruin III instead of Emerald Ruin III if enabled and Ruin Mastery III is not active
            if (emeraldToRuinBeforeMasteryEnabled && !TraitLevelChecked(Traits.RuinMastery3) && LevelChecked(Ruin3) && !IsMoving())
            {
                actionID = OriginalHook(Ruin);
                return true;
            }
            #endregion

            if (egiSummonAttacksEnabled && GemshineReady)
            {
                if (flags.HasFlag(Combo.ST))
                {
                    actionID = OriginalHook(Gemshine);
                    return true;
                }
                if (flags.HasFlag(Combo.AoE))
                {
                    actionID = OriginalHook(PreciousBrilliance);
                    return true;
                }
            }
            if (ruin4Enabled && HasStatusEffect(Buffs.FurtherRuin) && IsMoving())
            {
                actionID = Ruin4;
                return true;
            }
        }
        #endregion
        
        #region Ifrit Phase
        if (IsIfritAttuned || OriginalHook(AstralFlow) is CrimsonCyclone or CrimsonStrike)
        {
            if (swiftcastEgiEnabled && swiftcastPhase is 2 or 3 && Role.CanSwiftcast())
            {
                actionID = Role.Swiftcast;
                return true;
            }

            if (egiSummonAttacksEnabled && GemshineReady && (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
            {
                if (flags.HasFlag(Combo.ST))
                {
                    actionID = OriginalHook(Gemshine);
                    return true;
                }
                if (flags.HasFlag(Combo.AoE))
                {
                    actionID = OriginalHook(PreciousBrilliance);
                    return true;
                }
            }

            if (IfritAstralFlowCyclone && HasStatusEffect(Buffs.IfritsFavor) &&
                GetTargetDistance() <= crimsonCycloneMeleeDistance  //Melee Check
                || IfritAstralFlowStrike && HasStatusEffect(Buffs.CrimsonStrike) && InMeleeRange()) //After Strike
            {
                actionID = OriginalHook(AstralFlow);
                return true;
            }

            if (ruin4Enabled && HasStatusEffect(Buffs.FurtherRuin) && !HasStatusEffect(Role.Buffs.Swiftcast))
            {
                actionID = Ruin4;
                return true;
            }
        }
        #endregion
        
        #region Ruin 4 Dump
        //Dump for ruin 4 if all your summons are done and you arent ready to demi yet. 
        if (ruin4Enabled && !IsAttunedAny && DemiNone && HasStatusEffect(Buffs.FurtherRuin) && !CanSummonEgi)
        {
            actionID = Ruin4;
            return true;
        }
        #endregion
        
        return false;
    }
    #endregion
    
    #endregion

    #region Opener
    internal static SMNOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        return Opener1.LevelChecked ? Opener1 : WrathOpener.Dummy;
    }

    internal class SMNOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Ruin3,
            SummonSolarBahamut,
            UmbralImpulse,
            SearingLight,
            UmbralImpulse,
            UmbralImpulse,
            EnergyDrain,
            UmbralImpulse,
            EnkindleSolarBahamut,
            Necrotize,
            UmbralImpulse,
            Sunflare,
            Necrotize,
            UmbralImpulse,
            SearingFlash,
            SummonTitan2,
            TopazRite,
            MountainBuster,
            TopazRite,
            MountainBuster,
            TopazRite,
            MountainBuster,
            TopazRite,
            MountainBuster,
            SummonGaruda2,
            Role.Swiftcast,
            Slipstream,

        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            4,
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([26], () => SMN_Opener_SkipSwiftcast == 2)];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData? ContentCheckConfig => SMN_Balance_Content;
        public override Preset Preset => Preset.SMN_ST_Advanced_Combo_Balance_Opener;
        public override bool HasCooldowns()
        {
            if (!HasPetPresent())
                return false;

            if (!ActionReady(SummonSolarBahamut) ||
                !IsOffCooldown(SearingFlash) ||
                !IsOffCooldown(SearingLight) ||
                !IsOffCooldown(Role.Swiftcast) ||
                !IsOffCooldown(EnergyDrain))
                return false;

            return true;
        }
    }
    #endregion
}

