using ContentHelper = ECommons.GameHelpers;
using IntendedUse = ECommons.ExcelServices.TerritoryIntendedUseEnum;

namespace WrathCombo.Combos.PvE.Content;

internal class Bozja
{
    public const uint
    #region Offensive
        LostFocus = 20714,
        LostFontOfMagic = 20715,
        LostFontOfPower = 20717,
        LostSlash = 20718,
        LostDeath = 20719,
        BannerOfNobleEnds = 20720,
        BannerOfHonoredSacrifice = 20721,
        BannerOfHonedAcuity = 20725,
        LostFairTrade = 20732,
        LostFlareStar = 22352,
        LostChainspell = 23913,
        LostAssassination = 23914,
    #endregion

    #region Defensive
        LostManawall = 20703,
        BannerOfTirelessConviction = 20722,
        BannerOfFirmResolve = 20723,
        LostIncense = 20731,
        LostExcellence = 23919,
        LostBloodRage = 23921,
    #endregion

    #region Restorative
        BannerOfSolemnClarity = 20724,
        LostCure = 20726,
        LostCure2 = 20727,
        LostCure3 = 20728,
        LostCure4 = 20729,
        LostArise = 20730,
        LostSacrifice = 22345,
        LostReraise = 23912,
        LostFullCure = 23920,
    #endregion

    #region Beneficial
        LostSpellforge = 20706,
        LostSteelsting = 20707,
        LostProtect = 20709,
        LostShell = 20710,
        LostReflect = 20711,
        LostStoneskin = 20712,
        LostBravery = 20713,
        LostAethershield = 22355,
        LostDervish = 22356,
        LostStoneskin2 = 23908,
        LostProtect2 = 23915,
        LostShell2 = 23916,
        LostBubble = 23917,
    #endregion

    #region Tactical
        LostStealth = 20705,
        LostSwift = 20708,
        LostFontOfSkill = 20716,
        Mimic = 20733,
        LostPerception = 22344,
        LostImpetus = 23918,
    #endregion

    #region Detrimental
        LostParalyze3 = 20701,
        LostBanish3 = 20702,
        LostDispel = 20704,
        LostRendArmor = 22353,
        LostSeraphStrike = 22354,
        LostBurst = 23909,
        LostRampage = 23910,
    #endregion

    #region Item-related
        DynamisDice = 20734,
        ResistancePhoenix = 20735,
        ResistanceReraiser = 20736,
        ResistancePotionKit = 20737,
        ResistanceEtherKit = 20738,
        ResistanceMedikit = 20739,
        ResistancePotion = 20740,
        EssenceOfTheAetherweaver = 20741,
        EssenceOfTheMartialist = 20742,
        EssenceOfTheSavior = 20743,
        EssenceOfTheVeteran = 20744,
        EssenceOfThePlatebearer = 20745,
        EssenceOfTheGuardian = 20746,
        EssenceOfTheOrdained = 20747,
        EssenceOfTheSkirmisher = 20748,
        EssenceOfTheWatcher = 20749,
        EssenceOfTheProfane = 20750,
        EssenceOfTheIrregular = 20751,
        EssenceOfTheBreathtaker = 20752,
        EssenceOfTheBloodsucker = 20753,
        EssenceOfTheBeast = 20754,
        EssenceOfTheTemplar = 20755,
        DeepEssenceOfTheAetherweaver = 20756,
        DeepEssenceOfTheMartialist = 20757,
        DeepEssenceOfTheSavior = 20758,
        DeepEssenceOfTheVeteran = 20759,
        DeepEssenceOfThePlatebearer = 20760,
        DeepEssenceOfTheGuardian = 20761,
        DeepEssenceOfTheOrdained = 20762,
        DeepEssenceOfTheSkirmisher = 20763,
        DeepEssenceOfTheWatcher = 20764,
        DeepEssenceOfTheProfane = 20765,
        DeepEssenceOfTheIrregular = 20766,
        DeepEssenceOfTheBreathtaker = 20767,
        DeepEssenceOfTheBloodsucker = 20768,
        DeepEssenceOfTheBeast = 20769,
        DeepEssenceOfTheTemplar = 20770,
        PureEssenceOfTheGambler = 22346,
        PureEssenceOfTheElder = 22347,
        PureEssenceOfTheDuelist = 22348,
        PureEssenceOfTheFieldhunter = 22349,
        PureEssenceOfTheIndomitable = 22350,
        PureEssenceOfTheDivine = 22351,
        Lodestone = 23907,
        LightCurtain = 23911,
        ResistanceElixir = 23922;
    #endregion

    public static class Buffs
    {
        public const ushort

        #region Lost Actions
            LostFontOfPower = 2346, //granted by Lost Font of Power action or Lost Assasination
            LostSpellforge = 2338, //granted by Lost Spellforge
            LostSteelsting = 2339, //granted by Lost Steelsting
            LostProtect = 2333, //granted by Lost Protect
            LostProtect2 = 2561, //granted by Lost Protect II
            LostShell = 2334, //granted by Lost Shell
            LostShell2 = 2562, //granted by Lost Shell II
            LostReflect = 2337, //granted by Lost Reflect
            LostBravery = 2341, //granted by Lost Bravery
            LostAethershield = 2443, //granted by Lost Aethershield
            LostDervish = 2444, //granted by Lost Dervish
            LostBubble = 2563, //granted by Lost Bubble
            LostFontOfMagic = 2332, //granted by Lost Font of Magic
            LostChainspell = 2560, //granted by Lost Chainspell
            LostExcellence = 2564, //granted by Lost Excellence
            LostBloodRage = 2566, //granted by Lost Blood Rage
            LostManawall = 2345, //granted by Lost Manawall
            LostStealth = 2336, //granted by Lost Stealth
            LostSwift = 2335, //granted by Lost Swift
        #endregion

        #region Banners
            BannerOfNobleEnds = 2326, //granted by Banner of Noble Ends
            BannerOfHonoredSacrifice = 2327, //granted by Banner of Honored Sacrifice
            BannerOfHonedAcuity = 2331, //granted by Banner of Honed Acuity
            BannerOfTirelessConviction = 2328, //granted by Banner of Tireless Conviction
            BannerOfFirmResolve = 2329, //granted by Banner of Firm Resolve
            BannerOfSolemnClarity = 2330, //granted by Banner of Solemn Clarity
            BannerOfTranscendentFinesse = 2354, //granted by 3 stacks of Banner of Honed Acuity
            BannerOfUnyieldingDefense = 2352, //granted by 5 stacks of either Banner of Firm Resolve or Tireless Conviction
            BannerOfLimitlessGrace = 2353, //granted by 4 stacks of Banner of Solemn Clarity
        #endregion

        #region Misc
            MPRefresh = 909,
            MPRefresh2 = 1198,
            Boost = 1656, //granted by Lost Focus; max 16 stacks
            SpellShield = 1648, //granted by Lost Font of Magic, requires Spirit of the Veteran
            SolidShield = 1647, //granted by Lost Font of Power, requires Spirit of the Platebearer
            MagicBurst = 1652, //granted by Lost Chainspell
            Memorable = 2565, //granted by Lost Excellence
            BloodRush = 2567, //granted by Lost Blood Rage
            Sacrifice = 1743, //granted by Lost Sacrifice
            Reraise = 2355, //granted by Lost Reraise
            Autopotion = 2342, //Granted by Lost Full Cure
            Autoether = 2343, //Granted by Lost Full Cure
            RapidRecast = 1645, //granted by Lost Swift, requires Spirit of the Watcher
            ClericStance = 2484, //granted by Lost Seraph Strike 
        #endregion

        #region Essences
            SpiritOfTheAetherweaver = 2311, //granted by any Essence of the Aetherweaver
            SpiritOfTheMartialist = 2312, //granted by any Essence of the Martialist
            SpiritOfTheSavior = 2313, //granted by any Essence of the Savior
            SpiritOfTheVeteran = 2314, //granted by any Essence of the Veteran
            SpiritOfThePlatebearer = 2315, //granted by any Essence of the Platebearer
            SpiritOfTheGuardian = 2316, //granted by any Essence of the Guardian
            SpiritOfTheOrdained = 2317, //granted by any Essence of the Ordained
            SpiritOfTheSkirmisher = 2318, //granted by any Essence of the Skirmisher
            SpiritOfTheWatcher = 2319, //granted by any Essence of the Watcher
            SpiritOfTheProfane = 2320, //granted by any Essence of the Profane
            SpiritOfTheIrregular = 2321, //granted by any Essence of the Irregular
            SpiritOfTheBreathtaker = 2322, //granted by any Essence of the Breathtaker 
            SpiritOfTheBloodsucker = 2323, //granted by any Essence of the Bloodsucker 
            SpiritOfTheBeast = 2324, //granted by any Essence of the Beast
            SpiritOfTheTemplar = 2325, //granted by any Essence of the Templar
            SpiritOfTheGambler = 2434, //granted by Pure Essence of the Gambler
            SpiritOfTheElder = 2435, //granted by PureEssence of the Elder 
            SpiritOfTheDuelist = 2436, //granted by Pure Essence of the Duelist
            SpiritOfTheFieldhunter = 2437, //granted by Pure Essence of the Fieldhunter
            SpiritOfTheIndomitable = 2438, //granted by Pure Essence of the Indomitable
            SpiritOfTheDivine = 2439; //granted by Pure Essence of the Divine
        #endregion

    }

    public static class Debuffs
    {
        public const ushort
            LostFlareStar = 2440,
            LostRendArmor = 2441,
            MagicalAversion = 2370,
            PhysicalAversion = 2369;
    }

    public static bool InFieldOperations => ContentHelper.Content.ContentType == ContentHelper.ContentType.FieldOperations; //Southern Front, Zadnor
    public static bool InFieldRaids => ContentHelper.Content.ContentType == ContentHelper.ContentType.FieldRaid; //Delubrum Reginae, etc.
    public static bool IsInBozja => ContentHelper.Content.TerritoryIntendedUse == IntendedUse.Bozja && (InFieldOperations || InFieldRaids);
}
