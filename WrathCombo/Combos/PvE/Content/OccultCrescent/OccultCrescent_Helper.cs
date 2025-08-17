#region Dependencies

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
        Suspend = 41620;
    internal static bool IsEnabledAndUsable(Preset preset, uint action) => IsEnabled(preset) && HasActionEquipped(action) && ActionReady(action);

    public static class JobIDs
    {
        public const int
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
            #region Not Yet Added

            Dragoon = 13,
            WhiteMage = 14,
            BlackMage = 15,
            Ninja = 16,
            Summoner = 17,
            Dancer = 18,
            RedMage = 19,
            BlueMage = 20,
            Gladiator = 21,
            MysticKnight = 22,
            BeastMaster = 23,
            Necromancer = 24,
            Mime = 25;

            #endregion
    }

    public static class Buffs
    {
        public static ushort
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
            CloudyCaress = 4280;
    }

    public static class Debuffs
    {
        public static ushort
            Slow = 3493,
            OccultMageMasher = 4259,
            SilverSickness = 4264,
            FalsePrediction = 4269,
            WeaponPlifered = 4279;
    }

    public static class Traits
    {
        public static ushort
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
        public static ushort
            OccultPotion = 47741,
            OccultElixir = 47743;
    }
}
