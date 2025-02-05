#region

#region

using System;
using System.Linq;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Options = WrathCombo.Combos.CustomComboPreset;

#endregion

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace WrathCombo.Combos.PvE;

#endregion

internal partial class BLU
{
    internal class DoTs(
        UserInt minHp,
        UserInt minTime,
        DoT[] dotsToUse)
    {
        public bool AnyDotsWanted() => dotsToUse.Any(CheckDotWanted);

        public bool CheckDotWanted(DoT dot) =>
            // Check config preset is enabled
            IsEnabled(dot.Preset()) &&
            // Check spell is ready
            IsSpellActive(dot.Action()) &&
            ActionReady(dot.Action()) &&
            !WasLastAction(dot.Action()) &&
            // Check debuff is not applied or remaining time is less than requirement
            (!TargetHasEffect(dot.Debuff()) ||
             GetDebuffRemainingTime(dot.Debuff()) <= minTime) &&
            // Check target HP is above requirement
            GetTargetHPPercent() > minHp;
    }

    #region ID's

    public const byte JobID = 36;

    public const byte BypassAction = 2;

    #region Abilities

    public const uint
        WaterCannon_Spell1 = 11385,
        FlameThrower_Spell2 = 11402,
        AquaBreath_Spell3 = 11390,
        FlyingFrenzy_Spell4 = 11389,
        DrillCannons_Spell5 = 11398,
        HighVoltage_Spell6 = 11387,
        Loom_Spell7 = 11401,
        FinalSting_Spell8 = 11407,
        SongofTorment_Spell9 = 11386,
        Glower_Spell10 = 11404,
        Plaincracker_Spell11 = 11391,
        Bristle_Spell12 = 11393,
        WhiteWind_Spell13 = 11406,
        LevelfivePetrify_Spell14 = 11414,
        SharpenedKnife_Spell15 = 11400,
        IceSpikes_Spell16 = 11418,
        BloodDrain_Spell17 = 11395,
        AcornBomb_Spell18 = 11392,
        BombToss_Spell19 = 11396,
        Offguard_Spell20 = 11411,
        Selfdestruct_Spell21 = 11408,
        Transfusion_Spell22 = 11409,
        Faze_Spell23 = 11403,
        FlyingSardine_Spell24 = 11423,
        Snort_Spell25 = 11383,
        FourTonzeWeight_Spell26 = 11384,
        TheLook_Spell27 = 11399,
        BadBreath_Spell28 = 11388,
        Diamondback_Spell29 = 11424,
        MightyGuard_Spell30 = 11417,
        StickyTongue_Spell31 = 11412,
        ToadOil_Spell32 = 11410,
        RamsVoice_Spell33 = 11419,
        TheDragonsVoice_Spell34 = 11420,
        Missile_Spell35 = 11405,
        ThousandNeedles_Spell36 = 11397,
        InkJet_Spell37 = 11422,
        FireAngon_Spell38 = 11425,
        MoonFlute_Spell39 = 11415,
        TailScrew_Spell40 = 11413,
        MindBlast_Spell41 = 11394,
        Doom_Spell42 = 11416,
        PeculiarLight_Spell43 = 11421,
        FeatherRain_Spell44 = 11426,
        Eruption_Spell45 = 11427,
        MountainBuster_Spell46 = 11428,
        ShockStrike_Spell47 = 11429,
        GlassDance_Spell48 = 11430,
        VeiloftheWhorl_Spell49 = 11431,
        AlpineDraft_Spell50 = 18295,
        ProteanWave_Spell51 = 18296,
        Northerlies_Spell52 = 18297,
        Electrogenesis_Spell53 = 18298,
        Kaltstrahl_Spell54 = 18299,
        AbyssalTransfixion_Spell55 = 18300,
        Chirp_Spell56 = 18301,
        EerieSoundwave_Spell57 = 18302,
        PomCure_Spell58 = 18303,
        Gobskin_Spell59 = 18304,
        MagicHammer_Spell60 = 18305,
        Avail_Spell61 = 18306,
        FrogLegs_Spell62 = 18307,
        SonicBoom_Spell63 = 18308,
        Whistle_Spell64 = 18309,
        WhiteKnightsTour_Spell65 = 18310,
        BlackKnightsTour_Spell66 = 18311,
        LevelfiveDeath_Spell67 = 18312,
        Launcher_Spell68 = 18313,
        PerpetualRay_Spell69 = 18314,
        Cactguard_Spell70 = 18315,
        RevengeBlast_Spell71 = 18316,
        AngelWhisper_Spell72 = 18317,
        Exuviation_Spell73 = 18318,
        Reflux_Spell74 = 18319,
        Devour_Spell75 = 18320,
        CondensedLibra_Spell76 = 18321,
        AethericMimicry_Spell77 = 18322,
        Surpanakha_Spell78 = 18323,
        Quasar_Spell79 = 18324,
        JKick_Spell80 = 18325,
        TripleTrident_Spell81 = 23264,
        Tingle_Spell82 = 23265,
        Tatamigaeshi_Spell83 = 23266,
        ColdFog_Spell84 = 23267,
        Stotram_Spell85 = 23269,
        SaintlyBeam_Spell86 = 23270,
        FeculentFlood_Spell87 = 23271,
        AngelsSnack_Spell88 = 23272,
        ChelonianGate_Spell89 = 23273,
        RoseofDestruction_Spell90 = 23275,
        BasicInstinct_Spell91 = 23276,
        Ultravibration_Spell92 = 23277,
        Blaze_Spell93 = 23278,
        MustardBomb_Spell94 = 23279,
        DragonForce_Spell95 = 23280,
        AetherialSpark_Spell96 = 23281,
        HydroPull_Spell97 = 23282,
        MaledictionofWater_Spell98 = 23283,
        ChocoMeteor_Spell99 = 23284,
        MatraMagic_Spell100 = 23285,
        PeripheralSynthesis_Spell101 = 23286,
        BothEnds_Spell102 = 23287,
        PhantomFlurry_Spell103 = 23288,
        Nightbloom_Spell104 = 23290,
        GoblinPunch_Spell105 = 34563,
        RightRound_Spell106 = 34564,
        Schiltron_Spell107 = 34565,
        Rehydration_Spell108 = 34566,
        BreathofMagic_Spell109 = 34567,
        WildRage_Spell110 = 34568,
        PeatPelt_Spell111 = 34569,
        DeepClean_Spell112 = 34570,
        RubyDynamics_Spell113 = 34571,
        DivinationRune_Spell114 = 34572,
        DimensionalShift_Spell115 = 34573,
        ConvictionMarcato_Spell116 = 34574,
        ForceField_Spell117 = 34575,
        WingedReprobation_Spell118 = 34576,
        LaserEye_Spell119 = 34577,
        CandyCane_Spell120 = 34578,
        MortalFlame_Spell121 = 34579,
        SeaShanty_Spell122 = 34580,
        Apokalypsis_Spell123 = 34581,
        BeingMortal_Spell124 = 34582;

    #endregion

    public static class Buffs
    {
        public const ushort
            MoonFlute = 1718,
            Bristle = 1716,
            WaningNocturne = 1727,
            PhantomFlurry = 2502,
            Tingle = 2492,
            Whistle = 2118,
            TankMimicry = 2124,
            DPSMimicry = 2125,
            BasicInstinct = 2498,
            WingedReprobation = 3640;
    }

    public static class Debuffs
    {
        public const ushort
            Slow = 9,
            Bind = 13,
            BadBreath = 18,
            Stun = 142,
            SongOfTorment = 1714,
            NightBloom = 1714,
            FeatherRain = 1723,
            DeepFreeze = 1731,
            Offguard = 1717,
            Malodorous = 1715,
            Conked = 2115,
            Lightheaded = 2501,
            MortalFlame = 3643,
            BreathOfMagic = 3712,
            Begrimed = 3636;
    }

    internal enum DoT
    {
        [DoTInfo(
            Debuffs.SongOfTorment,
            SongofTorment_Spell9,
            Options.BLU_DPS_DoT)]
        DPS_SongOfTorment,

        [DoTInfo(
            Debuffs.BreathOfMagic,
            BreathofMagic_Spell109,
            Options.BLU_DPS_DoT_Breath)]
        DPS_BreathOfMagic,

        [DoTInfo(
            Debuffs.MortalFlame,
            MortalFlame_Spell121,
            Options.BLU_DPS_DoT_Flame)]
        DPS_MortalFlame,

        [DoTInfo(
            Debuffs.FeatherRain,
            FeatherRain_Spell44,
            Options.BLU_Tank_DoT)]
        Tank_FeatherRain,

        [DoTInfo(
            Debuffs.SongOfTorment,
            SongofTorment_Spell9,
            Options.BLU_Tank_DoT_Torment)]
        Tank_SongOfTorment,

        [DoTInfo(
            Debuffs.BadBreath,
            BadBreath_Spell28,
            Options.BLU_Tank_DoT_Bad)]
        Tank_BadBreath,

        [DoTInfo(
            Debuffs.BreathOfMagic,
            BreathofMagic_Spell109,
            Options.BLU_Tank_DoT_Breath)]
        Tank_BreathOfMagic,

        [DoTInfo(
            Debuffs.MortalFlame,
            MortalFlame_Spell121,
            Options.BLU_Tank_DoT_Flame)]
        Tank_MortalFlame
    }

    #endregion
}

#region DoT Attributes

[AttributeUsage(AttributeTargets.Field)]
internal class DoTInfoAttribute(
    ushort debuffID,
    uint spellID,
    Options configPreset) : Attribute
{
    public ushort DebuffID { get; } = debuffID;
    public uint SpellID { get; } = spellID;
    public Options Config { get; } = configPreset;
}

internal static class DoTExtensions
{
    public static ushort Debuff(this BLU.DoT dot)
    {
        var type = typeof(BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes =
            memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).DebuffID;
    }

    public static uint Action(this BLU.DoT dot)
    {
        var type = typeof(BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes =
            memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).SpellID;
    }

    public static Options Preset(this BLU.DoT dot)
    {
        var type = typeof(BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes =
            memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).Config;
    }
}

#endregion
