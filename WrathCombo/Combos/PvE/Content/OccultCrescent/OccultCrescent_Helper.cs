#region Dependencies

using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using System;
using System.Reflection;
using static WrathCombo.Combos.PvE.JobIDExtensions;
using static WrathCombo.Combos.PvE.OccultCrescent.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    public static uint
        //Freelancer
        OccultResuscitation = 41650,
        OccultTreasuresight = 41651,

        //Knight
        PhantomGuard = 41588,
        Pray = 41589,
        OccultHeal = 41590,
        Pledge = 41591,

        //Monk
        PhantomKick = 41595,
        OccultCounter = 41596,
        Counterstance = 41597,
        OccultChakra = 41598,

        //Thief
        OccultSprint = 41646,
        Steal = 41645,
        Vigilance = 41647,
        TrapDetection = 41648,
        PilferWeapon = 41649,

        //Berserker
        Rage = 41592,
        DeadlyBlow = 41594,

        //Ranger
        PhantomAim = 41599,
        OccultFeatherfoot = 41600,
        OccultFalcon = 41601,
        OccultUnicorn = 41602,

        //Time Mage
        OccultSlowga = 41621,
        OccultComet = 41623,
        OccultMageMasher = 41624,
        OccultDispel = 41622,
        OccultQuick = 41625,

        //Chemist
        OccultPotion = 41631,
        OccultEther = 41633,
        Revive = 41634,
        OccultElixir = 41635,

        //Bard
        MightyMarch = 41607,
        OffensiveAria = 41608,
        RomeosBallad = 41609,
        HerosRime = 41610,

        //Oracle
        Predict = 41636,
        PhantomJudgment = 41637,
        Cleansing = 41638,
        Blessing = 41639,
        Starfall = 41640,
        Recuperation = 41641,
        PhantomDoom = 41642,
        PhantomRejuvenation = 41643,
        Invulnerability = 41644,

        //Cannoneer
        PhantomFire = 41626,
        HolyCannon = 41627,
        DarkCannon = 41628,
        ShockCannon = 41629,
        SilverCannon = 41630,

        //Samurai
        Mineuchi = 41603,
        Shirahadori = 41604,
        Iainuki = 41605,
        Zeninage = 41606,

        //Geomancer
        BattleBell = 41611,
        Weather = 41612,
        Sunbath = 41613,
        CloudyCaress = 41614,
        BlessedRain = 41615,
        MistyMirage = 41616,
        HastyMirage = 41617,
        AetherialGain = 41618,
        RingingRespite = 41619,
        Suspend = 41620,

        //Mystic Knight
        MagicShell = 46590,
        SunderingSpellblade = 46591,
        HolySpellblade = 46592,
        BlazingSpellblade = 46593,

        //Dancer
        Dance = 46598,
        PoisedToSwordDance = 46599,
        TemptedToTango = 46600,
        Jitterbug = 46601,
        WillingToWaltz = 46602,
        Quickstep = 46603,
        SteadfastStance = 46604,
        Mesmerize = 46605,

        //Gladiator
        Finisher = 46594,
        Defend = 46595,
        LongReach = 46596,
        BladeBlitz = 46597,

        //Ninja
        FumaShuriken = 49062,
        Smoke = 49063,
        LightningScroll = 49064,
        FlameScroll = 49065,
        Image = 49066,

        //White Mage
        OccultCureII = 49067,
        OccultCureIII = 49068,
        OccultBlink = 49069,
        OccultRaise = 49070,
        OccultHoly = 49071,

        //Black Mage
        OccultFireIII = 49072,
        OccultBlizzardIII = 49073,
        OccultThunderIII = 49074,
        OccultToad = 49075,
        OccultFlare = 49076,

        //Dragoon
        OccultJump = 49077,
        StepForth = 49078,
        Lance = 49079,

        //Summoner
        Hellfire = 49080,
        JudgmentBolt = 49081,
        EarthenWall = 49082,
        Thunderstorm = 49083,
        Megaflare = 49084,

        //Blue Mage
        OccultAero = 49085,
        OccultMissile = 49086,
        OccultAquaBreath = 49087,
        OccultMightyGuard = 49088,
        OccultAeroII = 49089,
        OccultWhiteWind = 49090,
        OccultAeroIII = 49091,

        //Red Mage
        OccultFireII = 49092,
        OccultCureII_RDM = 49093,
        OccultLibra = 49094,
        OccultBlizzardII = 49095,
        OccultThunderII = 49096,

        //Necromancer
        DrainTouch = 49097,
        DeepFreeze = 49098,
        HellWind = 49099,
        ChaosDrive = 49100,
        Doomsday = 49101;

    internal static unsafe int CurrentJobLevel => (nint)PublicContentOccultCrescent.GetInstance() == nint.Zero ? 0 : PublicContentOccultCrescent.GetInstance()->State.SupportJobLevels[PublicContentOccultCrescent.GetInstance()->State.CurrentSupportJob];


    internal static bool IsEnabledAndUsable(Preset preset, uint action) => IsEnabled(preset) && ActionReady(action);

    private const int HoldOnlyWhenStationary = 0;
    private const int HoldOnlyInMeleeRange = 1;

    private static bool CanUseOccultJumpHoldOptions()
    {
        if (Phantom_Dragoon_OccultJumpMovingOrInRanged.Count == 0)
            return true;

        if (Phantom_Dragoon_OccultJumpMovingOrInRanged[HoldOnlyWhenStationary] && IsMoving())
            return false;

        if (Phantom_Dragoon_OccultJumpMovingOrInRanged.Count > HoldOnlyInMeleeRange &&
            Phantom_Dragoon_OccultJumpMovingOrInRanged[HoldOnlyInMeleeRange] && !InMeleeRange())
            return false;

        return true;
    }

    /// <summary>
    ///     Job identifiers and which Icon is their own. <br />
    ///     Matched to their Icon via FFV Job Sprites: <br />
    ///     https://finalfantasy.fandom.com/wiki/Final_Fantasy_V_jobs
    /// </summary>
    /// <seealso cref="Window.Icons.Occult.JobSprites" />
    /// <seealso cref="Window.Functions.Presets.DrawOccultJobIcon(int)">
    ///     Window.Functions.Presets.DrawOccultJobIcon()
    /// </seealso>
    /// <seealso cref="JobIDExtensions" />
    internal enum JobIDs
    {
        // 7.25
        Freelancer = 0,
        Knight = 1,
        Berserker = 2,
        Monk = 3,
        Ranger = 4,
        Samurai = 5,
        Bard = 6,
        Geomancer = 7,
        TimeMage = 8,
        Cannoneer = 9,
        Chemist = 10,
        Oracle = 11,
        Thief = 12,

        // 7.4
        MysticKnight = 13,
        Gladiator = 14,
        Dancer = 15,

        // 7.55
        Ninja = 16,
        WhiteMage = 17,
        BlackMage = 18,
        Dragoon = 19,
        Summoner = 20,
        BlueMage = 21,
        RedMage = 22,
        Necromancer = 23,

        // N/A
        N_A = -1
    }

    public static class Buffs
    {
        public const uint
            PhantomGuard = 4231,
            Pray = 4232,
            EnduringFortitude = 4233,
            Pledge = 4234,
            Rage = 4235,
            PentupRage = 4236,
            PhantomKick = 4237,
            Counterstance = 4238,
            Fleetfooted = 4239,
            PhantomAim = 4240,
            OccultUnicorn = 4243,
            RomeosBallad = 4244,
            MightyMarch = 4246,
            OffensiveAria = 4247,
            HerosRime = 4249,
            BattleBell = 4251,
            BattlesClangor = 4252,
            BlessedRain = 4253,
            MistyMirage = 4254,
            HastyMirage = 4255,
            AetherialGain = 4256,
            RingingRespite = 4257,
            Suspend = 4258,
            OccultQuick = 4260,
            OccultSprint = 4261,
            OccultSwift = 4262,
            PredictionOfJudgment = 4265,
            PredictionOfCleansing = 4266,
            PredictionOfBlessing = 4267,
            PredictionOfStarfall = 4268,
            Recuperation = 4271,
            FortifiedRecuperation = 4272,
            PhantomDoom = 4273,
            PhantomRejuvenation = 4274,
            Invulnerability = 4275,
            Shirahadori = 4245,
            Vigilance = 4277,
            CloudyCaress = 4280,
            BlazingSpellblade = 4790,
            PoisedToSwordDance = 4794,
            TemptedToTango = 4795,
            Jitterbugged = 4796,
            WillingToWaltz = 4797,
            Quickstep = 4798,
            SteadfastStance = 4800,
            FinishingFervor = 4793,
            OccultBlink = 5316,
            JumpVulnerabilityDown = 5318,
            Lance = 5319,
            EarthenWall = 5320,
            OccultMightyGuard = 5321,
            DrainTouch = 5326,
            Smoke = 5327,
            Dualcast = 5438;
    }

    public static class Debuffs
    {
        public const uint
            Slow = 3493,
            Blind = 15,
            Paralysis = 17,
            OccultMageMasher = 4259,
            SilverSickness = 4264,
            FalsePrediction = 4269,
            WeaponPlifered = 4279,
            OccultToad = 5317,
            FireWeakness = 5322,
            IceWeakness = 5323,
            LightningWeakness = 5324,
            WindWeakness = 5325;
    }

    public static class Traits
    {
        public const ushort
            EnhancedPhantomGuard = 0,
            EnhancedPray = 1,
            EnhancedPhantomKick = 2,
            EnhancedPhantomKickII = 3,
            Lockpicker = 4,
            EnhancedRage = 5,
            EnhancedPhantomAim = 6,
            EnhancedPhantomAimII = 7,
            EnhancedVocals = 8,
            EnhancedPhantomFire = 9,
            EnhancedIainuki = 10,
            EnhancedBell = 11;
    }

    public static class Items
    {
        public const ushort
            OccultPotion = 47741,
            OccultElixir = 47743;
    }
}

/// <summary>
///     Provides helper methods for working with <see cref="OccultCrescent.JobIDs" />.
/// </summary>
internal static class JobIDExtensions
{
    /// <summary>
    ///     Determines whether the specified job identifier represents an active job.
    /// </summary>
    public static bool IsActive(this OccultCrescent.JobIDs jobID)
    {
        FieldInfo? member = typeof(OccultCrescent.JobIDs).GetField(jobID.ToString()!);
        return member is not null && !Attribute.IsDefined(member, typeof(NotYetImplemented));
    }

    /// <summary>
    ///     Gets the name of the job associated with the provided numeric value.
    /// </summary>
    public static string? GetNameFromValue(int value) =>
        Enum.IsDefined(typeof(OccultCrescent.JobIDs), value)
            ? Enum.GetName(typeof(OccultCrescent.JobIDs), value)
            : null;

    /// <summary>
    ///     Determines whether the specified numeric value corresponds to an active <see cref="OccultCrescent.JobIDs" /> entry.
    /// </summary>
    public static bool GetActiveFromValue(int value)
    {
        if (!Enum.IsDefined(typeof(OccultCrescent.JobIDs), value))
        {
            return false;
        }

        OccultCrescent.JobIDs jobID = (OccultCrescent.JobIDs)value;
        return jobID.IsActive();
    }

    /// <summary>
    ///     Gets the highest numeric value among active job identifiers.
    /// </summary>
    public static int GetHighestActiveOccultID()
    {
        // Iterate through defined jobs, tracking the highest active numeric value.
        int highest = -1;
        foreach(OccultCrescent.JobIDs job in
            Enum.GetValues(typeof(OccultCrescent.JobIDs)))
        {
            if (!job.IsActive())
            {
                continue;
            }

            int value = (int)job;
            if (value > highest)
            {
                highest = value;
            }
        }

        return highest;
    }

    /// <summary>
    ///     Marks job identifiers that are not yet implemented.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class NotYetImplemented : Attribute
    {
    }
}


