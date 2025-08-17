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
        IGameObject? optionalTarget,
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
        IGameObject? optionalTarget,
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

