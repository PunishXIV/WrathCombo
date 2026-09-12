using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Buddy = FFXIVClientStructs.FFXIV.Client.Game.UI.Buddy;

namespace WrathCombo.Combos.PvE;

internal partial class BST
{
    public static bool FinisherReady = false;
    static BST()
    {
        Svc.Framework.Update += CheckForFinisher;
    }

    static List<uint> FinisherActions = new List<uint>();
    static DateTime? TimeFinisherStarted;

    private static void CheckForFinisher(IFramework framework)
    {
        if (!FinisherReady)
        {
            if (JobGauge.MasterInstinct >= 2 && JobGauge.PetInstinct >= 1 && ActionReady(Rally) && ActionReady(RallyingCheer) && JobGauge.PlayerTP >= 100 && JobGauge.BeastTP >= 100)
            {
                FinisherReady = true;
                FinisherActions.AddRange([Trick, CounterClockwiseInstinctualAction, Rally, RallyingCheer, Trick]);
            }
        }
        else
        {
            if (FinisherActions.Count == 0 || (TimeFinisherStarted.HasValue && (DateTime.Now - TimeFinisherStarted.Value).TotalSeconds > 10))
            {
                TimeFinisherStarted = null;
                FinisherReady = false;
                FinisherActions.Clear();
            }

            if (FinisherActions.Count > 0 && ActionWatching.LastAction == FinisherActions[0])
            {
                if (FinisherActions.Count == 5)
                    TimeFinisherStarted = DateTime.Now;

                Svc.Log.Debug($"Removing {FinisherActions[0].ActionName()} from FinisherActions");
                FinisherActions.RemoveAt(0);
            }
        }
    }

    public const uint
        SmashAxe = 44879,
        Capture = 44880,
        FirstBattlehorn = 44881,
        Gauge = 44882,
        AxebladeBite = 44883,
        AvalancheAxe = 44884,
        Shieldsplitter = 44885,
        BeastMode = 44886,
        MistralAxe = 44887,
        SpinningAxe = 44888,
        GaleAxe = 44889,
        TemperedRelease = 44890,
        PartingBlow = 44891,
        SecondBattlehorn = 44892,
        ShieldCharge = 44893,
        ThirdBattlehorn = 44894,
        Borrow = 44895,
        Beastskin = 44896,
        Vileskin = 44897,
        CloudSkim = 44898,
        Seedsower = 44899,
        QuellingWave = 44900,
        Scaleskin = 44901,
        SoulCrush = 44902,
        ScouringAsh = 44903,
        RallyingCheer = 44904,
        Rally = 44905,
        BrutalRage = 44930,
        HawkishTalons = 44931,
        RisenFall = 44932,
        Calamity = 44933,
        Trick = 47093,
        BorrowBeast = 47238,
        BorrowVile = 47239,
        BorrowCloud = 47240,
        BorrowSeed = 47241,
        BorrowWave = 47242,
        BorrowScale = 47243,
        BorrowSoul = 47244,
        BorrowAsh = 47245;

    public static class TrickActions
    {
        public const uint
            Cusith_Rake = 44935,
            Squirrel_SomersaultSlash = 44937,
            Lamb_FleeceButt = 44939,
            Pugil_Screwdriver = 44941,
            Opoopo_StoneThrow = 44943,
            Dodo_FowlStench = 44945,
            Coblyn_BestialThunder = 44947,
            Diremite_DeadlyThrust = 44949,
            Megalocrab_DrenchingBlow = 44951,
            Wespe_SharpSting = 44953,
            Vulture_WingCutter = 44955,
            Mandragora_Budbutt = 44957,
            Geshunpest_DarkThunder = 44959,
            Puk_Fireball = 44961,
            Crab_BubbleShower = 44963,
            Mantis_StandingChine = 44965,
            Slime_Digest = 44967,
            Dullahan_IronJustice = 44969,
            Bat_BloodDrain = 44971,
            Flyingtrap_SourSough = 44973,
            Ziz_IceBreath = 44975,
            Sabotender_NaturalNeedles = 44977,
            Golem_BoulderClap = 44979,
            Apkallu_FlyingSardine = 44981,
            Adamantoise_BestialThunderII = 44983,
            Buffalo_Heave = 44985,
            Uragnite_FrostBreath = 44987,
            Worm_SandBreath = 44989,
            Spriggan_Romp = 44991,
            Goobbue_Beatdown = 44993,
            Gigantoad_BestialBlizzardII = 44995,
            Colibri_Loop = 44997,
            Coeurl_Blaster = 44999,
            Raptor_FrostBreath = 45001,
            Drake_BurningCyclone = 45003,
            Treant_AcornBomb = 45005,
            Antling_MandibleBite = 45007,
            Chimera_theLionsBreath = 45009,
            Morbol_VineProbe = 45011,
            Ghost_FellGale = 45013,
            Salamander_BrackishRain = 45015,
            Cobra_DrippingFang = 45017,
            Hydra_MainTrap = 45019,
            Damselfly_CursedSphere = 45021,
            Rottinggoobbue_DirtySneeze = 45023,
            Zu_FlyingFrenzy = 45025,
            Icegolem_IceGuillotine = 45027,
            Karlabos_Impale = 45029,
            Rafflesia_BloodyCaress = 45031,
            Behemoth_Thunderbolt = 45033;
    }

    public static class TemperedReleaseActions
    {
        public const uint
            Cusith_RelentlessRake = 44936,
            Squirrel_Scamper = 44938,
            Lamb_Lullaby = 44940,
            Pugil_WaterWall = 44942,
            Opoopo_PinsandNails = 44944,
            Dodo_Strut = 44946,
            Coblyn_Vulcanize = 44948,
            Diremite_Silkscreen = 44950,
            Megalocrab_BubbleShower = 44952,
            Wespe_FinalSting = 44954,
            Vulture_BloodcurdlingCaw = 44956,
            Mandragora_HeirloomScream = 44958,
            Geshunpest_Odium = 44960,
            Puk_TailChase = 44962,
            Crab_HundredFists = 44964,
            Mantis_EerieSoundwave = 44966,
            Slime_Syrup = 44968,
            Dullahan_KingsWill = 44970,
            Bat_Ultrasonics = 44972,
            Flyingtrap_NecroticNectar = 44974,
            Ziz_Petribreath = 44976,
            Sabotender_HypodermicHustle = 44978,
            Golem_Rockslide = 44980,
            Apkallu_Regurgitate = 44982,
            Adamantoise_HardenShell = 44984,
            Buffalo_WarCry = 44986,
            Uragnite_GasShell = 44988,
            Worm_BottomlessDesert = 44990,
            Spriggan_FreneticFlurry = 44992,
            Goobbue_MoldySneeze = 44994,
            Gigantoad_StickyTongue = 44996,
            Colibri_PeckingFlurry = 44998,
            Coeurl_ChargedWhisker = 45000,
            Raptor_FoulBreath = 45002,
            Drake_SmolderingScales = 45004,
            Treant_ArborealStorm = 45006,
            Antling_FormicPheromones = 45008,
            Chimera_theRamsVoice = 45010,
            Morbol_BadBreath = 45012,
            Ghost_Curse = 45014,
            Salamander_PeculiarLight = 45016,
            Cobra_StoneGaze = 45018,
            Hydra_WhiteBreath = 45020,
            Damselfly_Venom = 45022,
            Rottinggoobbue_Inhale = 45024,
            Zu_BreathWing = 45026,
            Icegolem_FrozenHeart = 45028,
            Karlabos_TailScrew = 45030,
            Rafflesia_BlightedBouquet = 45032,
            Behemoth_Meteor = 45034;

    }

    public static class Buffs
    {
        public const uint

            VolantHeart = 4595,
            RampantHeart = 4596,
            DurantHeart = 4597,
            EldritchHeart = 4598;
    }

    public static class Debuffs
    {
        public const uint
            InterestCaptured = 4626;
    }

    public static class Traits
    {
        public const uint
            WildHeart = 690,
            WildHeartII = 691,
            BattlehornMastery = 692,
            WildHeartIV = 693,
            WildHeartIII = 694,
            TemperedReleaseMastery = 749,
            EnhancedBorrow = 750,
            EnhancedShieldCharge = 751,
            Beastmastery = 752,
            BattlehornMasteryII = 754,
            BattlehornMasteryIII = 755,
            EnhancedRally = 756,
            EnhancedRallyingCheer = 757,
            InstinctualMastery = 758;

    }

    private static List<uint> RampantTricks =
    [
         TrickActions.Cusith_Rake,
         TrickActions.Squirrel_SomersaultSlash,
         TrickActions.Lamb_FleeceButt,
         TrickActions.Opoopo_StoneThrow,
         TrickActions.Diremite_DeadlyThrust,
         TrickActions.Mandragora_Budbutt,
         TrickActions.Puk_Fireball,
         TrickActions.Sabotender_NaturalNeedles,
         TrickActions.Buffalo_Heave,
         TrickActions.Spriggan_Romp,
         TrickActions.Goobbue_Beatdown,
         TrickActions.Drake_BurningCyclone,
         TrickActions.Antling_MandibleBite,
         TrickActions.Chimera_theLionsBreath,
         TrickActions.Morbol_VineProbe,
    ];

    private static List<uint> EldritchTricks =
    [
         TrickActions.Dodo_FowlStench,
         TrickActions.Coblyn_BestialThunder,
         TrickActions.Geshunpest_DarkThunder,
         TrickActions.Slime_Digest,
         TrickActions.Golem_BoulderClap,
         TrickActions.Adamantoise_BestialThunderII,
         TrickActions.Worm_SandBreath,
         TrickActions.Gigantoad_BestialBlizzardII,
         TrickActions.Coeurl_Blaster,
         TrickActions.Treant_AcornBomb,
         TrickActions.Rottinggoobbue_DirtySneeze,
         TrickActions.Rafflesia_BloodyCaress,
         TrickActions.Behemoth_Thunderbolt,
    ];

    private static List<uint> DurantTricks =
    [
         TrickActions.Pugil_Screwdriver,
         TrickActions.Megalocrab_DrenchingBlow,
         TrickActions.Crab_BubbleShower,
         TrickActions.Mantis_StandingChine,
         TrickActions.Dullahan_IronJustice,
         TrickActions.Ziz_IceBreath,
         TrickActions.Apkallu_FlyingSardine,
         TrickActions.Uragnite_FrostBreath,
         TrickActions.Raptor_FrostBreath,
         TrickActions.Salamander_BrackishRain,
         TrickActions.Cobra_DrippingFang,
         TrickActions.Hydra_MainTrap,
         TrickActions.Icegolem_IceGuillotine,
         TrickActions.Karlabos_Impale,

    ];

    private static List<uint> VolantTricks =
    [
         TrickActions.Wespe_SharpSting,
         TrickActions.Vulture_WingCutter,
         TrickActions.Bat_BloodDrain,
         TrickActions.Flyingtrap_SourSough,
         TrickActions.Colibri_Loop,
         TrickActions.Ghost_FellGale,
         TrickActions.Damselfly_CursedSphere,
         TrickActions.Zu_FlyingFrenzy,
    ];

    public unsafe static TmpBSTGauge* _jobGauge => (TmpBSTGauge*)((nint)JobGaugeManager.StaticAddressPointers.pInstance + 0x08);

    public unsafe static TmpBSTGauge JobGauge => *_jobGauge;

    public unsafe static Buddy.BuddyMember? CurrentPet => *UIState.Instance()->Buddy.PetInfo.Pet;
    public static unsafe bool CurrentPetIsBMPet => CurrentPet?.DataId > 0 && Svc.Data.GetExcelSheet<XBMPet>().Any(x => x.Pet.RowId == CurrentPet?.DataId);

    public static Pet? CurrentPetSheet => CurrentPetIsBMPet ? Svc.Data.GetExcelSheet<Pet>().GetRow(CurrentPet?.DataId ?? 0) : null;

    public static uint? CurrentPetTrickAction => CurrentPetSheet?.Abilities[0].RowId ?? 0;
    public static uint? CurrentPetReleaseAction => CurrentPetSheet?.Abilities[1].RowId ?? 0;
    private static bool TrickIsDurant => DurantTricks.Any(x => x == CurrentPetTrickAction);
    private static bool TrickIsEldritch => EldritchTricks.Any(x => x == CurrentPetTrickAction);
    private static bool TrickIsVolant => VolantTricks.Any(x => x == CurrentPetTrickAction);
    private static bool TrickIsRampant => RampantTricks.Any(x => x == CurrentPetTrickAction);

    public enum TrickTypes
    {
        Durant,
        Eldritch,
        Volant,
        Rampant,
        None
    }

    public static TrickTypes TrickType => TrickIsDurant ? TrickTypes.Durant : TrickIsEldritch ? TrickTypes.Eldritch : TrickIsVolant ? TrickTypes.Volant : TrickIsRampant ? TrickTypes.Rampant : TrickTypes.None;

    public static uint InstinctualComboAxe
    {
        get
        {
            if (TrickIsRampant)
                return AvalancheAxe;
            if (TrickIsDurant)
                return MistralAxe;
            if (TrickIsEldritch)
                return SpinningAxe;
            if (TrickIsVolant)
                return GaleAxe;

            return 0;
        }
    }

    /// <summary>
    /// Used if Trick is the first action used
    /// </summary>
    public static uint ClockwiseInstinctualAction
    {
        get
        {
            if (TrickIsRampant)
                return MistralAxe;
            if (TrickIsDurant)
                return SpinningAxe;
            if (TrickIsEldritch)
                return GaleAxe;
            if (TrickIsVolant)
                return AvalancheAxe;

            return 0;
        }
    }

    /// <summary>
    /// Used if Trick is the second action used
    /// </summary>
    public static uint CounterClockwiseInstinctualAction
    {
        get
        {
            if (TrickIsRampant)
                return GaleAxe;
            if (TrickIsDurant)
                return AvalancheAxe;
            if (TrickIsEldritch)
                return MistralAxe;
            if (TrickIsVolant)
                return SpinningAxe;
            return 0;
        }
    }

    public static bool InInstinctualCombo
    {
        get
        {
            if (LocalPlayer is not { } p)
                return false;

            if (JobGauge.BeastTP < 100 && JobGauge.PlayerTP < 100)
                return false;

            if (p.HasStatus(Buffs.RampantHeart) || p.HasStatus(Buffs.DurantHeart) || p.HasStatus(Buffs.EldritchHeart) || p.HasStatus(Buffs.VolantHeart))
                return true;

            return false;
        }
    }

    public static bool CanStartInstinctualCombo => JobGauge.PlayerTP >= 100 && JobGauge.BeastTP >= 100;
    public static bool AbleToIntentional => ActionLearned(ClockwiseInstinctualAction) || ActionLearned(CounterClockwiseInstinctualAction);
    public static bool RallyLearnt => ActionLearned(Rally);
    public static bool RallyingCheerLearnt => ActionLearned(RallyingCheer);
    public static bool FinisherLearnt => Player.Available && TraitLevelChecked(Traits.InstinctualMastery);
    public static bool OnLastHorn
    {
        get
        {
            if (!ActionReady(FirstBattlehorn) && !ActionReady(SecondBattlehorn) && !ActionReady(ThirdBattlehorn))
                return true;
            return false;
        }
    }

    public static int TPRestoredByStacks(int stacks) => 40 + (stacks * 70);

    public enum RallyingType
    {
        None,
        Rally,
        RallyingCheer
    }

    public static RallyingType RallyStackFocus
    {
        get
        {
            if (RallyLearnt && !RallyingCheerLearnt && JobGauge.MasterInstinct < 3)
                return RallyingType.Rally;

            if (RallyLearnt && RallyingCheerLearnt)
            {
                if (JobGauge.MasterInstinct == 3 && JobGauge.PetInstinct == 3) //Both are maxed
                    return RallyingType.None;

                if (JobGauge.MasterInstinct < 2 || JobGauge.PetInstinct == 3)
                    return RallyingType.Rally;

                return RallyingType.RallyingCheer;
            }

            return RallyingType.None;
        }
    }

    public static unsafe bool PetUnlocked(uint petId) => XBMManager.Instance()->IsPetUnlocked(petId);

    public static bool TargetIsBstPet(IBattleChara? tar) => tar is not null && GetPetIdFromModel(tar) is not 0;

    public static uint PetIdToModel(uint petId)
    {
        var xbmPet = Svc.Data.GetExcelSheet<XBMPet>().GetRow(petId);
        var modelRow = xbmPet.Pet.Value.AllowedPetMirage[0].Value.ModelChara.Value.Model;

        return modelRow;
    }

    public static uint ModelToPetId(uint modelId, byte baseval)
    {
        var mirageRow = Svc.Data.GetExcelSheet<PetMirage>().FirstOrDefault(x => x.ModelChara.Value.Model == modelId && x.ModelChara.Value.Base == baseval).RowId;
        var petRow = Svc.Data.GetExcelSheet<Pet>().FirstOrDefault(x => x.AllowedPetMirage[0].RowId == mirageRow);

        return petRow.Unknown18;
    }

    public static uint GetPetIdFromModel(IBattleChara? tar)
    {
        if (tar is null)
            return 0;
        var baseNpc = Svc.Data.GetExcelSheet<BNpcBase>().GetRow(tar.BaseId);
        if (baseNpc.Unknown10 != 5)
            return 0;
        var model = baseNpc.ModelChara;
        var modelId = model.Value.Model;
        var baseval = model.Value.Base;
        var petId = ModelToPetId(modelId, baseval);

        return petId;
    }

    public static bool BasicCombo(ref uint actionId)
    {
        if (ComboAction is SmashAxe && ActionReady(AxebladeBite))
        {
            actionId = AxebladeBite;
            return true;
        }

        if (ComboAction is AxebladeBite && ActionReady(Shieldsplitter))
        {
            actionId = Shieldsplitter;
            return true;
        }

        actionId = SmashAxe;
        return true;
    }

}