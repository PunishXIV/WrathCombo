using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    #region ID's

    public const byte ClassID = 26;
    public const byte JobID = 27;

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
            RefulgentLux = 3874;
    }

    public static class Traits
    {
        public const ushort
            RuinMastery3 = 476;
    }

    #endregion

    internal static SMNOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
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
            All.Swiftcast,
            Slipstream,

        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            4,
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData? ContentCheckConfig => Config.SMN_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!ActionsReady([SummonSolarBahamut, SearingLight, EnergyDrain, SearingFlash, All.Swiftcast]))
                return false;

            return true;
        }
    }
}
