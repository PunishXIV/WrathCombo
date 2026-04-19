#region

using ECommons.ExcelServices;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using JobRole = WrathCombo.CustomComboNS.Functions.Jobs.JobRole;

// ReSharper disable EmptyRegion
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

#endregion

namespace WrathCombo.Combos;

/// <summary> Combo presets. </summary>
public enum Preset
{
    #region PvE Combos

    #region GLOBAL FEATURES

    #region Global Tank Features

    [JobInfo(Job.ADV, JobRole.Tank)]
    ALL_Tank_Menu = 100099,

    [ReplaceSkill(RoleActions.Tank.LowBlow, PLD.ShieldBash)]
    [ConflictingCombos(PLD_RetargetShieldBash)]
    [ParentCombo(ALL_Tank_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Tank_Interrupt = 100000,

    [ParentCombo(ALL_Tank_Interrupt)]
    [Retargeted(RoleActions.Tank.Interject, RoleActions.Tank.LowBlow, PLD.ShieldBash)]
    [JobInfo(Job.ADV)]
    ALL_Tank_Interrupt_Retarget = 100005,

    [ReplaceSkill(RoleActions.Tank.Reprisal)]
    [ParentCombo(ALL_Tank_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Tank_Reprisal = 100001,

    [ReplaceSkill(RoleActions.Tank.Shirk)]
    [ParentCombo(ALL_Tank_Menu)]
    [JobInfo(Job.ADV)]
    [Retargeted(RoleActions.Tank.Shirk)]
    ALL_Tank_ShirkRetargeting = 100002,

    [ParentCombo(ALL_Tank_ShirkRetargeting)]
    [JobInfo(Job.ADV)]
    [Retargeted]
    ALL_Tank_ShirkRetargeting_Healer = 100003,

    [ParentCombo(ALL_Tank_ShirkRetargeting)]
    [JobInfo(Job.ADV)]
    [Retargeted]
    ALL_Tank_ShirkRetargeting_Fallback = 100004,

    #endregion

    #region Global Healer Features

    [JobInfo(Job.ADV, JobRole.Healer)]
    ALL_Healer_Menu = 100098,

    [ReplaceSkill(AST.Ascend, WHM.Raise, SCH.Resurrection, SGE.Egeiro)]
    [ParentCombo(ALL_Healer_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Healer_Raise = 100010,

    [ParentCombo(ALL_Healer_Raise)]
    [JobInfo(Job.ADV)]
    [Retargeted(WHM.Raise, AST.Ascend, SGE.Egeiro, SCH.Resurrection)]
    ALL_Healer_Raise_Retarget = 100011,

    [ReplaceSkill(RoleActions.Healer.Esuna)]
    [ParentCombo(ALL_Healer_Menu)]
    [JobInfo(Job.ADV)]
    [Retargeted(RoleActions.Healer.Esuna)]
    ALL_Healer_EsunaRetargeting = 100012,

    [ReplaceSkill(RoleActions.Healer.Rescue)]
    [ParentCombo(ALL_Healer_Menu)]
    [JobInfo(Job.ADV)]
    [Retargeted(RoleActions.Healer.Rescue)]
    ALL_Healer_RescueRetargeting = 100013,
    #endregion

    #region Global Magical Ranged Features

    [JobInfo(Job.ADV, JobRole.MagicalDPS)]
    ALL_Caster_Menu = 100097,

    [ReplaceSkill(RoleActions.Caster.Addle)]
    [ParentCombo(ALL_Caster_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Caster_Addle = 100020,

    [ConflictingCombos(SMN_Raise, RDM_Raise)]
    [ParentCombo(ALL_Caster_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Caster_Raise = 100021,

    [ParentCombo(ALL_Caster_Raise)]
    [JobInfo(Job.ADV)]
    [Retargeted(BLU.AngelWhisper, RDM.Verraise, SMN.Resurrection)]
    ALL_Caster_Raise_Retarget = 100022,

    #endregion

    #region Global Melee Features

    [JobInfo(Job.ADV, JobRole.MeleeDPS)]
    ALL_Melee_Menu = 100096,

    [ReplaceSkill(RoleActions.Melee.Feint)]
    [ParentCombo(ALL_Melee_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Melee_Feint = 100030,

    [ReplaceSkill(RoleActions.Melee.TrueNorth)]
    [ParentCombo(ALL_Melee_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Melee_TrueNorth = 100031,

    #endregion

    #region Global Ranged Physical Features

    [JobInfo(Job.ADV, JobRole.RangedDPS)]
    ALL_Ranged_Menu = 100095,

    [ReplaceSkill(MCH.Tactician, BRD.Troubadour, DNC.ShieldSamba)]
    [ParentCombo(ALL_Ranged_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Ranged_Mitigation = 100040,

    [ReplaceSkill(RoleActions.PhysRanged.FootGraze)]
    [ParentCombo(ALL_Ranged_Menu)]
    [JobInfo(Job.ADV)]
    ALL_Ranged_Interrupt = 100041,

    #endregion

    //Non-gameplay Features
    //[CustomComboInfo("Output Combat Log", "Outputs your performed actions to the chat.", Job.ADV)]
    //AllOutputCombatLog = 100094,

    // Last value = 100094

    #endregion

    #region BOZJA ACTIONS

    [Bozja]
    [JobInfo(Job.ADV, JobRole.Tank)]
    Bozja_Tank = 210000,

    #region Tank
    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostFocus = 210001,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostFontOfPower = 210002,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostSlash = 210003,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostDeath = 210004,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_BannerOfNobleEnds = 210005,

    [Bozja]
    [ParentCombo(Bozja_Tank_BannerOfNobleEnds)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_PowerEnds = 210006,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_BannerOfHonoredSacrifice = 210007,

    [Bozja]
    [ParentCombo(Bozja_Tank_BannerOfHonoredSacrifice)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_PowerSacrifice = 210008,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_BannerOfHonedAcuity = 210009,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostFairTrade = 210010,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostAssassination = 210011,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostManawall = 210012,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_BannerOfTirelessConviction = 210013,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostBloodRage = 210014,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_BannerOfSolemnClarity = 210015,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostCure = 210016,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostCure2 = 210017,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostCure3 = 210018,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostCure4 = 210019,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostArise = 210020,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostSacrifice = 210021,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostReraise = 210022,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostSpellforge = 210023,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostSteelsting = 210024,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostProtect = 210025,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostShell = 210026,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostReflect = 210027,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostBravery = 210028,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostAethershield = 210029,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostProtect2 = 210030,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostShell2 = 210031,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostBubble = 210032,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostStealth = 210033,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostSwift = 210034,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostFontOfSkill = 210035,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostImpetus = 210036,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostParalyze3 = 210037,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [JobInfo(Job.ADV)]
    Bozja_Tank_LostRampage = 210038,
    #endregion

    #region Healer

    // TODO: maybe someday

    #endregion

    #region Melee

    // TODO: maybe someday

    #endregion

    #region Ranged

    // TODO: maybe someday

    #endregion

    #region Caster

    // TODO: maybe someday

    #endregion

    #endregion

    #region VARIANT ACTIONS
    [Variant]
    [JobInfo(Job.ADV, JobRole.Tank)]
    Variant_Tank = 200000,

    [Variant]
    [ParentCombo(Variant_Tank)]
    [JobInfo(Job.ADV)]
    Variant_Tank_Cure = 200001,

    [Variant]
    [ParentCombo(Variant_Tank)]
    [JobInfo(Job.ADV)]
    Variant_Tank_Ultimatum = 200002,

    [Variant]
    [ParentCombo(Variant_Tank)]
    [JobInfo(Job.ADV)]
    Variant_Tank_Raise = 200003,

    [Variant]
    [ParentCombo(Variant_Tank)]
    [JobInfo(Job.ADV)]
    Variant_Tank_SpiritDart = 200004,

    [Variant]
    [ParentCombo(Variant_Tank)]
    [JobInfo(Job.ADV)]
    Variant_Tank_EagleEyeShot = 200024,


    [Variant]
    [JobInfo(Job.ADV, JobRole.Healer)]
    Variant_Healer = 200005,

    [Variant]
    [ParentCombo(Variant_Healer)]
    [JobInfo(Job.ADV)]
    Variant_Healer_Ultimatum = 200006,

    [Variant]
    [ParentCombo(Variant_Healer)]
    [JobInfo(Job.ADV)]
    Variant_Healer_SpiritDart = 200007,

    [Variant]
    [ParentCombo(Variant_Healer)]
    [JobInfo(Job.ADV)]
    Variant_Healer_Rampart = 200008,

    [Variant]
    [ParentCombo(Variant_Healer)]
    [JobInfo(Job.ADV)]
    Variant_Healer_EagleEyeShot = 200025,


    [Variant]
    [JobInfo(Job.ADV, JobRole.MeleeDPS)]
    Variant_Melee = 200009,

    [Variant]
    [ParentCombo(Variant_Melee)]
    [JobInfo(Job.ADV)]
    Variant_Melee_Cure = 200010,

    [Variant]
    [ParentCombo(Variant_Melee)]
    [JobInfo(Job.ADV)]
    Variant_Melee_Ultimatum = 200011,

    [Variant]
    [ParentCombo(Variant_Melee)]
    [JobInfo(Job.ADV)]
    Variant_Melee_Raise = 200012,

    [Variant]
    [ParentCombo(Variant_Melee)]
    [JobInfo(Job.ADV)]
    Variant_Melee_Rampart = 200013,

    [Variant]
    [ParentCombo(Variant_Melee)]
    [JobInfo(Job.ADV)]
    Variant_Melee_EagleEyeShot = 200026,


    [Variant]
    [JobInfo(Job.ADV, JobRole.RangedDPS)]
    Variant_PhysRanged = 200014,

    [Variant]
    [ParentCombo(Variant_PhysRanged)]
    [JobInfo(Job.ADV)]
    Variant_PhysRanged_Cure = 200015,

    [Variant]
    [ParentCombo(Variant_PhysRanged)]
    [JobInfo(Job.ADV)]
    Variant_PhysRanged_Ultimatum = 200016,

    [Variant]
    [ParentCombo(Variant_PhysRanged)]
    [JobInfo(Job.ADV)]
    Variant_PhysRanged_Raise = 200017,

    [Variant]
    [ParentCombo(Variant_PhysRanged)]
    [JobInfo(Job.ADV)]
    Variant_PhysRanged_Rampart = 200018,

    [Variant]
    [ParentCombo(Variant_PhysRanged)]
    [JobInfo(Job.ADV)]
    Variant_PhysRanged_EagleEyeShot = 200027,


    [Variant]
    [JobInfo(Job.ADV, JobRole.MagicalDPS)]
    Variant_Magic = 200019,

    [Variant]
    [ParentCombo(Variant_Magic)]
    [JobInfo(Job.ADV)]
    Variant_Magic_Cure = 200020,

    [Variant]
    [ParentCombo(Variant_Magic)]
    [JobInfo(Job.ADV)]
    Variant_Magic_Ultimatum = 200021,

    [Variant]
    [ParentCombo(Variant_Magic)]
    [JobInfo(Job.ADV)]
    Variant_Magic_Raise = 200022,

    [Variant]
    [ParentCombo(Variant_Magic)]
    [JobInfo(Job.ADV)]
    Variant_Magic_Rampart = 200023,

    [Variant]
    [ParentCombo(Variant_Magic)]
    [JobInfo(Job.ADV)]
    Variant_Magic_EagleEyeShot = 200028,

    // last value = 200028

    #endregion

    #region PHANTOM ACTIONS
    [OccultCrescent(OccultCrescent.JobIDs.Freelancer)]
    [JobInfo(Job.ADV)]
    Phantom_Freelancer = 110000,

    [OccultCrescent]
    [ParentCombo(Phantom_Freelancer)]
    [JobInfo(Job.ADV)]
    Phantom_Freelancer_OccultResuscitation = 110001,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Freelancer)]
    //[CustomComboInfo("Occult Treasuresight", "Adds Occult Treasuresight into the rotation.", Job.ADV)]
    //Phantom_Freelancer_OccultTreasuresight = 110002,

    [OccultCrescent(OccultCrescent.JobIDs.Knight)]
    [JobInfo(Job.ADV)]
    Phantom_Knight = 110003,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [JobInfo(Job.ADV)]
    Phantom_Knight_PhantomGuard = 110004,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [JobInfo(Job.ADV)]
    Phantom_Knight_Pray = 110005,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [JobInfo(Job.ADV)]
    Phantom_Knight_OccultHeal = 110006,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [JobInfo(Job.ADV)]
    Phantom_Knight_Pledge = 110007,

    [OccultCrescent(OccultCrescent.JobIDs.Monk)]
    [JobInfo(Job.ADV)]
    Phantom_Monk = 110008,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [JobInfo(Job.ADV)]
    Phantom_Monk_PhantomKick = 110009,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [JobInfo(Job.ADV)]
    Phantom_Monk_OccultCounter = 110010,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [JobInfo(Job.ADV)]
    Phantom_Monk_Counterstance = 110011,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [JobInfo(Job.ADV)]
    Phantom_Monk_OccultChakra = 110012,

    [OccultCrescent(OccultCrescent.JobIDs.Thief)]
    [JobInfo(Job.ADV)]
    Phantom_Thief = 110013,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [JobInfo(Job.ADV)]
    Phantom_Thief_OccultSprint = 110014,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [JobInfo(Job.ADV)]
    Phantom_Thief_Steal = 110015,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [JobInfo(Job.ADV)]
    Phantom_Thief_Vigilance = 110016,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Thief)]
    //[CustomComboInfo("Trap Detection", "Adds Trap Detection into the rotation.", Job.ADV)]
    //Phantom_Thief_TrapDetection = 110017,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [JobInfo(Job.ADV)]
    Phantom_Thief_PilferWeapon = 110018,

    [OccultCrescent(OccultCrescent.JobIDs.Samurai)]
    [JobInfo(Job.ADV)]
    Phantom_Samurai = 110053,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [JobInfo(Job.ADV)]
    Phantom_Samurai_Mineuchi = 110054,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [JobInfo(Job.ADV)]
    Phantom_Samurai_Shirahadori = 110055,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [JobInfo(Job.ADV)]
    Phantom_Samurai_Iainuki = 110056,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [JobInfo(Job.ADV)]
    Phantom_Samurai_Zeninage = 110057,

    [OccultCrescent(OccultCrescent.JobIDs.Berserker)]
    [JobInfo(Job.ADV)]
    Phantom_Berserker = 110019,

    [OccultCrescent]
    [ParentCombo(Phantom_Berserker)]
    [JobInfo(Job.ADV)]
    Phantom_Berserker_Rage = 110020,

    [OccultCrescent]
    [ParentCombo(Phantom_Berserker)]
    [JobInfo(Job.ADV)]
    Phantom_Berserker_DeadlyBlow = 110021,

    [OccultCrescent(OccultCrescent.JobIDs.Ranger)]
    [JobInfo(Job.ADV)]
    Phantom_Ranger = 110022,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [JobInfo(Job.ADV)]
    Phantom_Ranger_PhantomAim = 110023,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Ranger)]
    //[CustomComboInfo("Occult Featherfoot", "Adds Occult Featherfoot into the rotation.", Job.ADV)]
    //Phantom_Ranger_OccultFeatherfoot = 110024,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Ranger)]
    //[CustomComboInfo("Occult Falcon", "Adds Occult Falcon into the rotation.", Job.ADV)]
    //Phantom_Ranger_OccultFalcon = 110025,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [JobInfo(Job.ADV)]
    Phantom_Ranger_OccultUnicorn = 110026,

    [OccultCrescent(OccultCrescent.JobIDs.TimeMage)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage = 110027,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage_OccultSlowga = 110028,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage_OccultSlowga)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage_OccultSlowga_Wait = 110075,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage_OccultComet = 110029,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage_OccultMageMasher = 110030,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage_OccultDispel = 110031,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [JobInfo(Job.ADV)]
    Phantom_TimeMage_OccultQuick = 110032,

    [OccultCrescent(OccultCrescent.JobIDs.Chemist)]
    [JobInfo(Job.ADV)]
    Phantom_Chemist = 110033,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [JobInfo(Job.ADV)]
    Phantom_Chemist_OccultPotion = 110034,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [JobInfo(Job.ADV)]
    Phantom_Chemist_OccultEther = 110035,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [JobInfo(Job.ADV)]
    Phantom_Chemist_Revive = 110036,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [JobInfo(Job.ADV)]
    Phantom_Chemist_OccultElixir = 110037,

    [OccultCrescent(OccultCrescent.JobIDs.Bard)]
    [JobInfo(Job.ADV)]
    Phantom_Bard = 110038,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [JobInfo(Job.ADV)]
    Phantom_Bard_MightyMarch = 110039,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [JobInfo(Job.ADV)]
    Phantom_Bard_OffensiveAria = 110040,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [JobInfo(Job.ADV)]
    Phantom_Bard_RomeosBallad = 110041,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [JobInfo(Job.ADV)]
    Phantom_Bard_HerosRime = 110042,

    [OccultCrescent(OccultCrescent.JobIDs.Oracle)]
    [JobInfo(Job.ADV)]
    Phantom_Oracle = 110043,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [JobInfo(Job.ADV)]
    Phantom_Oracle_Predict = 110044,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [JobInfo(Job.ADV)]
    Phantom_Oracle_PhantomJudgment = 110045,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [JobInfo(Job.ADV)]
    Phantom_Oracle_Cleansing = 110046,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [JobInfo(Job.ADV)]
    Phantom_Oracle_Blessing = 110047,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [JobInfo(Job.ADV)]
    Phantom_Oracle_Starfall = 110048,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Oracle)]
    //[CustomComboInfo("Recuperation", "Adds Recuperation into the rotation.", Job.ADV)]
    //Phantom_Oracle_Recuperation = 110049,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Oracle)]
    //[CustomComboInfo("Phantom Doom", "Adds Phantom Doom into the rotation.", Job.ADV)]
    //Phantom_Oracle_PhantomDoom = 110050,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Oracle)]
    //[CustomComboInfo("Phantom Rejuvenation", "Adds Phantom Rejuvenation into the rotation.", Job.ADV)]
    //Phantom_Oracle_PhantomRejuvenation = 110051,

    //[OccultCrescent]
    //[ParentCombo(Phantom_Oracle)]
    //[CustomComboInfo("Invulnerability", "Adds Invulnerability into the rotation.", Job.ADV)]
    //Phantom_Oracle_Invulnerability = 110052,

    [OccultCrescent(OccultCrescent.JobIDs.Cannoneer)]
    [JobInfo(Job.ADV)]
    Phantom_Cannoneer = 110058,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [JobInfo(Job.ADV)]
    Phantom_Cannoneer_PhantomFire = 110059,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [JobInfo(Job.ADV)]
    Phantom_Cannoneer_HolyCannon = 110060,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [JobInfo(Job.ADV)]
    Phantom_Cannoneer_DarkCannon = 110061,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [JobInfo(Job.ADV)]
    Phantom_Cannoneer_ShockCannon = 110062,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [JobInfo(Job.ADV)]
    Phantom_Cannoneer_SilverCannon = 110063,

    [OccultCrescent(OccultCrescent.JobIDs.Geomancer)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer = 110064,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_BattleBell = 110065,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_RingingRespite = 110073,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_Suspend = 110074,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_Weather = 110066,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_Sunbath = 110067,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_CloudyCaress = 110068,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_BlessedRain = 110069,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_MistyMirage = 110070,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_HastyMirage = 110071,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [JobInfo(Job.ADV)]
    Phantom_Geomancer_AetherialGain = 110072,

    [OccultCrescent(OccultCrescent.JobIDs.Dancer)]
    [JobInfo(Job.ADV)]
    Phantom_Dancer = 110076,

    [OccultCrescent]
    [ParentCombo(Phantom_Dancer)]
    [JobInfo(Job.ADV)]
    Phantom_Dancer_Dance = 110077,

    [OccultCrescent]
    [ParentCombo(Phantom_Dancer)]
    [JobInfo(Job.ADV)]
    Phantom_Dancer_QuickStep = 110078,

    [OccultCrescent]
    [ParentCombo(Phantom_Dancer)]
    [JobInfo(Job.ADV)]
    Phantom_Dancer_Mesmerize = 110079,

    [OccultCrescent(OccultCrescent.JobIDs.MysticKnight)]
    [JobInfo(Job.ADV)]
    Phantom_MysticKnight = 110080,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [JobInfo(Job.ADV)]
    Phantom_MysticKnight_SunderingSpellblade = 110081,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [JobInfo(Job.ADV)]
    Phantom_MysticKnight_HolySpellblade = 110082,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [JobInfo(Job.ADV)]
    Phantom_MysticKnight_BlazingSpellblade = 110083,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [JobInfo(Job.ADV)]
    Phantom_MysticKnight_MagicShell = 110084,

    [OccultCrescent(OccultCrescent.JobIDs.Gladiator)]
    [JobInfo(Job.ADV)]
    Phantom_Gladiator = 110085,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [JobInfo(Job.ADV)]
    Phantom_Gladiator_Finisher = 110086,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [JobInfo(Job.ADV)]
    Phantom_Gladiator_Defend = 110087,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [JobInfo(Job.ADV)]
    Phantom_Gladiator_LongReach = 110088,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [JobInfo(Job.ADV)]
    Phantom_Gladiator_BladeBlitz = 110089,

    //Last Value = 110089
    #endregion

    // Jobs

    #region ASTROLOGIAN

    #region Simple Modes
    [AutoAction(false, false)]
    [ReplaceSkill(AST.Malefic, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic)]
    [ConflictingCombos(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    [SimpleCombo]
    AST_ST_Simple_DPS = 1179,

    [AutoAction(true, false)]
    [ReplaceSkill(AST.Gravity, AST.Gravity2)]
    [ConflictingCombos(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    [SimpleCombo]
    AST_AOE_Simple_DPS = 1180,

    [AutoAction(false, true)]
    [ReplaceSkill(AST.Benefic)]
    [ConflictingCombos(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [SimpleCombo]
    [PossiblyRetargeted]
    AST_Simple_ST_Heals = 1196,


    [AutoAction(true, true)]
    [ReplaceSkill(AST.Helios)]
    [ConflictingCombos(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    [SimpleCombo]
    [PossiblyRetargeted]
    AST_Simple_AoE_Heals = 1197,

    #endregion

    #region ST DPS

    [AutoAction(false, false)]
    [ReplaceSkill(AST.Malefic, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic, AST.Combust, AST.Combust2,
        AST.Combust3)]
    [ConflictingCombos(AST_ST_Simple_DPS)]
    [JobInfo(Job.AST)]
    [AdvancedCombo]
    AST_ST_DPS = 1004,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_ST_DPS_Opener = 1040,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_ST_DPS_CombustUptime = 1018,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    [Retargeted]
    AST_ST_DPS_Move_DoT = 1084,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_DPS_LightSpeed = 1020,

    [ParentCombo(AST_DPS_LightSpeed)]
    [JobInfo(Job.AST)]
    AST_DPS_LightSpeedHold = 1061,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_DPS_Divination = 1016,

    [ParentCombo(AST_DPS_Divination)]
    [JobInfo(Job.AST)]
    AST_DPS_LightspeedBurst = 1064,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_DPS_AutoDraw = 1011,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled, AST.Play1, AST.Balance, AST.Spear)]
    AST_DPS_AutoPlay = 1037,

    [ParentCombo(AST_DPS_AutoPlay)]
    [JobInfo(Job.AST)]
    AST_DPS_CardPool = 1055,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_DPS_LazyLord = 1014,

    [ParentCombo(AST_DPS_LazyLord)]
    [JobInfo(Job.AST)]
    AST_DPS_LordPool = 1056,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_DPS_Oracle = 1015,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.EarthlyStar)]
    AST_ST_DPS_EarthlyStar = 1051,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_ST_DPS_StellarDetonation = 1081,

    [ParentCombo(AST_ST_DPS)]
    [JobInfo(Job.AST)]
    AST_DPS_Lucid = 1008,

    #endregion

    #region AOE DPS

    [AutoAction(true, false)]
    [ReplaceSkill(AST.Gravity, AST.Gravity2)]
    [ConflictingCombos(AST_AOE_Simple_DPS)]
    [JobInfo(Job.AST)]
    [AdvancedCombo]
    AST_AOE_DPS = 1041,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.Combust, AST.Combust2, AST.Combust3)]
    AST_AOE_DPS_DoT = 1083,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_LightSpeed = 1048,

    [ParentCombo(AST_AOE_LightSpeed)]
    [JobInfo(Job.AST)]
    AST_AOE_LightSpeedHold = 1062,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_Divination = 1043,

    [ParentCombo(AST_AOE_Divination)]
    [JobInfo(Job.AST)]
    AST_AOE_LightspeedBurst = 1063,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_AutoDraw = 1044,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled, AST.Play1, AST.Balance, AST.Spear)]
    AST_AOE_AutoPlay = 1045,

    [ParentCombo(AST_AOE_AutoPlay)]
    [JobInfo(Job.AST)]
    AST_AOE_CardPool = 1057,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_LazyLord = 1046,

    [ParentCombo(AST_AOE_LazyLord)]
    [JobInfo(Job.AST)]
    AST_AOE_LordPool = 1058,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_Oracle = 1047,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.EarthlyStar)]
    AST_AOE_DPS_EarthlyStar = 1052,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_DPS_StellarDetonation = 1082,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_DPS_MacroCosmos = 1066,

    [ParentCombo(AST_AOE_DPS)]
    [JobInfo(Job.AST)]
    AST_AOE_Lucid = 1042,

    #endregion

    #region Healing

    [AutoAction(false, true)]
    [ReplaceSkill(AST.Benefic)]
    [ConflictingCombos(AST_Simple_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.Benefic2)]
    [HealingCombo]
    AST_ST_Heals = 1023,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    AST_ST_Heals_Esuna = 1039,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.AspectedBenefic)]
    AST_ST_Heals_AspectedBenefic = 1027,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.EssentialDignity)]
    AST_ST_Heals_EssentialDignity = 1024,

    [ParentCombo(AST_ST_Heals_EssentialDignity)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.EssentialDignity)]
    AST_ST_Heals_EssentialDignity_Emergency = 1096,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.CelestialIntersection)]
    AST_ST_Heals_CelestialIntersection = 1025,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.Exaltation)]
    AST_ST_Heals_Exaltation = 1028,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.Spire)]
    AST_ST_Heals_Spire = 1030,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.Ewer)]
    AST_ST_Heals_Ewer = 1032,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.Arrow)]
    AST_ST_Heals_Arrow = 1049,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted(AST.Bole)]
    AST_ST_Heals_Bole = 1050,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    AST_ST_Heals_CelestialOpposition = 1068,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    AST_ST_Heals_CollectiveUnconscious = 1069,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    AST_ST_Heals_SoloLady = 1070,

    [ParentCombo(AST_ST_Heals)]
    [JobInfo(Job.AST)]
    AST_ST_Heals_NeutralSect = 1097,

    [AutoAction(true, true)]
    [ReplaceSkill(AST.Helios, AST.AspectedHelios, AST.HeliosConjuction)]
    [ConflictingCombos(AST_Simple_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals = 1010,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_Aspected = 1053,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_Helios = 1073,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_CelestialOpposition = 1021,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_LazyLady = 1022,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_Horoscope = 1026,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_HoroscopeHeal = 1071,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_NeutralSect = 1067,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_StellarDetonation = 1072,

    [ParentCombo(AST_AoE_Heals)]
    [JobInfo(Job.AST)]
    AST_AoE_Heals_CollectiveUnconscious = 1074,
    #endregion

    #region Cards
    [JobInfo(Job.AST)]
    [Retargeted(AST.Play1, AST.Balance, AST.Spear)]
    AST_Cards_QuickTargetCards = 1029,
    #endregion

    #region Standalones
    [ReplaceSkill(AST.Benefic2)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted("Retargeting Features below, Enable Cure", Condition.ASTRetargetingFeaturesEnabledForBenefic)]
    AST_Benefic = 1002,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [JobInfo(Job.AST)]
    AST_Raise_Alternative = 1003,

    [ParentCombo(AST_Raise_Alternative)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.Ascend)]
    AST_Raise_Alternative_Retarget = 1060,

    [ReplaceSkill(AST.Lightspeed)]
    [JobInfo(Job.AST)]
    AST_Lightspeed_Protection = 1065,

    [ReplaceSkill(AST.Exaltation)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted("Retargeting Features below, Enable Exaltation (and optionally Essential Dignity and Celestial Intersection", Condition.ASTRetargetingFeaturesEnabledForSTMit)]
    AST_Mit_ST = 1094,

    [ReplaceSkill(AST.CollectiveUnconscious)]
    [JobInfo(Job.AST)]
    AST_Mit_AoE = 1095,
    #endregion

    #region Raidwide Features
    [JobInfo(Job.AST)]
    AST_Raidwide = 1075,

    [ParentCombo(AST_Raidwide)]
    [JobInfo(Job.AST)]
    AST_Raidwide_CollectiveUnconscious = 1076,

    [ParentCombo(AST_Raidwide)]
    [JobInfo(Job.AST)]
    AST_Raidwide_NeutralSect = 1077,

    [ParentCombo(AST_Raidwide)]
    [JobInfo(Job.AST)]
    AST_Raidwide_AspectedHelios = 1078,

    #endregion

    #region Retargeting
    [JobInfo(Job.AST)]
    AST_Retargets = 1085,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Benefic, AST.Benefic2)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.Benefic, AST.Benefic2)]
    AST_Retargets_Benefic = 1086,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.AspectedBenefic)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.AspectedBenefic)]
    AST_Retargets_AspectedBenefic = 1087,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.EssentialDignity)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.EssentialDignity)]
    AST_Retargets_EssentialDignity = 1059,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Exaltation)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.Exaltation)]
    AST_Retargets_Exaltation = 1089,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Synastry)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.Synastry)]
    AST_Retargets_Synastry = 1090,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.CelestialIntersection)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.CelestialIntersection)]
    AST_Retargets_CelestialIntersection = 1091,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Play2, AST.Play3)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.Play2, AST.Play3)]
    AST_Retargets_HealCards = 1092,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.EarthlyStar)]
    [JobInfo(Job.AST)]
    [Retargeted(AST.EarthlyStar)]
    AST_Retargets_EarthlyStar = 1093,
    #endregion

    // Last value = 1097

    #endregion

    #region BLACK MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BLM.Blizzard)]
    [ConflictingCombos(BLM_ST_AdvancedMode, BLM_Blizzard1and3, BLM_B1toB4)]
    [JobInfo(Job.BLM)]
    [SimpleCombo]
    BLM_ST_SimpleMode = 2001,

    [AutoAction(true, false)]
    [ReplaceSkill(BLM.Blizzard2, BLM.HighBlizzard2)]
    [ConflictingCombos(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    [SimpleCombo]
    BLM_AoE_SimpleMode = 2002,

    #endregion

    #region Single Target - Advanced

    [AutoAction(false, false)]
    [ReplaceSkill(BLM.Blizzard)]
    [ConflictingCombos(BLM_ST_SimpleMode, BLM_Blizzard1and3, BLM_B1toB4)]
    [JobInfo(Job.BLM)]
    [AdvancedCombo]
    BLM_ST_AdvancedMode = 2100,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Opener = 2101,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Transpose = 2114,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_LeyLines = 2103,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Amplifier = 2102,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Manafont = 2108,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Thunder = 2110,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Despair = 2111,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_FlareStar = 2112,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Swiftcast = 2106,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Triplecast = 2115,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_UsePolyglot = 2104,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Movement = 2113,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Manaward = 2199,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_ST_Addle = 2195,

    #endregion

    #region AoE - Advanced

    [AutoAction(true, false)]
    [ReplaceSkill(BLM.Blizzard2, BLM.HighBlizzard2)]
    [ConflictingCombos(BLM_AoE_SimpleMode)]
    [JobInfo(Job.BLM)]
    [AdvancedCombo]
    BLM_AoE_AdvancedMode = 2200,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Transpose = 2212,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_LeyLines = 2202,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Amplifier = 2201,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Manafont = 2207,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Triplecast = 2208,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_ParadoxFiller = 2210,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Thunder = 2209,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Movement = 2213,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_UsePolyglot = 2203,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [JobInfo(Job.BLM)]
    BLM_AoE_Blizzard4Sub = 2211,

    #endregion

    #region Movement

    [ConflictingCombos(BLM_Aetherial_Manipulation)]
    [ReplaceSkill(BLM.AetherialManipulation)]
    [JobInfo(Job.BLM)]
    [Retargeted(BLM.AetherialManipulation)]
    BLM_Retargetting_Aetherial_Manipulation = 2066,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(BLM.Triplecast)]
    [JobInfo(Job.BLM)]
    BLM_TriplecastProtection = 2056,

    [ReplaceSkill(BLM.Fire, BLM.Fire3)]
    [ConflictingCombos(BLM_F1toF4)]
    [JobInfo(Job.BLM)]
    BLM_Fire1and3 = 2054,

    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_Fire1and3)]
    [JobInfo(Job.BLM)]
    BLM_F1toF4 = 2070,

    [ReplaceSkill(BLM.Fire4)]
    [JobInfo(Job.BLM)]
    BLM_Fire4 = 2059,

    [ReplaceSkill(BLM.Flare)]
    [JobInfo(Job.BLM)]
    BLM_Flare = 2069,

    [ReplaceSkill(BLM.Blizzard, BLM.Blizzard3)]
    [ConflictingCombos(BLM_ST_SimpleMode, BLM_ST_AdvancedMode, BLM_B1toB4)]
    [JobInfo(Job.BLM)]
    BLM_Blizzard1and3 = 2052,

    [ReplaceSkill(BLM.Blizzard)]
    [ConflictingCombos(BLM_ST_SimpleMode, BLM_ST_AdvancedMode, BLM_Blizzard1and3)]
    [JobInfo(Job.BLM)]
    BLM_B1toB4 = 2071,

    [ReplaceSkill(BLM.Blizzard4)]
    [JobInfo(Job.BLM)]
    BLM_Blizzard4toDespair = 2060,

    [ReplaceSkill(BLM.Freeze)]
    [JobInfo(Job.BLM)]
    BLM_Freeze = 2062,

    [ReplaceSkill(BLM.FlareStar)]
    [JobInfo(Job.BLM)]
    BLM_FlareParadox = 2063,

    [ReplaceSkill(BLM.Amplifier)]
    [JobInfo(Job.BLM)]
    BLM_AmplifierXeno = 2061,

    [ReplaceSkill(BLM.Xenoglossy)]
    [JobInfo(Job.BLM)]
    BLM_XenoThunder = 2067,

    [ReplaceSkill(BLM.Foul)]
    [JobInfo(Job.BLM)]
    BLM_FoulThunder = 2068,

    [ReplaceSkill(BLM.Transpose)]
    [JobInfo(Job.BLM)]
    BLM_UmbralSoul = 2050,

    [ReplaceSkill(BLM.Scathe)]
    [JobInfo(Job.BLM)]
    BLM_ScatheXeno = 2053,

    [ReplaceSkill(BLM.LeyLines)]
    [JobInfo(Job.BLM)]
    BLM_Between_The_LeyLines = 2051,

    [ReplaceSkill(BLM.AetherialManipulation)]
    [ConflictingCombos(BLM_Retargetting_Aetherial_Manipulation)]
    [JobInfo(Job.BLM)]
    BLM_Aetherial_Manipulation = 2055,

    #endregion

    // Last value ST = 2117
    //Last Value AoE = 2213
    //Last Value misc = 2071

    #endregion

    #region BLUE MAGE

    [ReplaceSkill(BLU.MoonFlute)]
    [BlueInactive(BLU.Whistle, BLU.Tingle, BLU.RoseOfDestruction, BLU.MoonFlute, BLU.JKick, BLU.TripleTrident,
        BLU.Nightbloom, BLU.WingedReprobation, BLU.SeaShanty, BLU.BeingMortal, BLU.ShockStrike, BLU.Surpanakha,
        BLU.MatraMagic, BLU.PhantomFlurry, BLU.Bristle)]
    [ConflictingCombos(BLU_Opener)]
    [JobInfo(Job.BLU)]
    [Retargeted(BLU.FeatherRain)]
    BLU_NewMoonFluteOpener = 70021,

    [BlueInactive(BLU.BreathOfMagic, BLU.MortalFlame)]
    [ParentCombo(BLU_NewMoonFluteOpener)]
    [JobInfo(Job.BLU)]
    BLU_NewMoonFluteOpener_DoTOpener = 70022,

    [BlueInactive(BLU.Whistle, BLU.Tingle, BLU.MoonFlute, BLU.JKick, BLU.TripleTrident, BLU.Nightbloom,
        BLU.RoseOfDestruction, BLU.FeatherRain, BLU.Bristle, BLU.GlassDance, BLU.Surpanakha, BLU.MatraMagic,
        BLU.ShockStrike, BLU.PhantomFlurry)]
    [ReplaceSkill(BLU.MoonFlute)]
    [ConflictingCombos(BLU_NewMoonFluteOpener)]
    [JobInfo(Job.BLU)]
    [Retargeted(BLU.FeatherRain)]
    BLU_Opener = 70001,

    [BlueInactive(BLU.MoonFlute, BLU.Tingle, BLU.ShockStrike, BLU.Whistle, BLU.FinalSting)]
    [ReplaceSkill(BLU.FinalSting)]
    [JobInfo(Job.BLU)]
    [Retargeted(BLU.FeatherRain)]
    BLU_FinalSting = 70002,

    [BlueInactive(BLU.RoseOfDestruction, BLU.FeatherRain, BLU.GlassDance, BLU.JKick)]
    [ParentCombo(BLU_FinalSting)]
    [JobInfo(Job.BLU)]
    [Retargeted(BLU.FeatherRain)]
    BLU_Primals = 70003,

    [BlueInactive(BLU.BasicInstinct)]
    [ParentCombo(BLU_FinalSting)]
    [JobInfo(Job.BLU)]
    BLU_SoloMode = 70011,

    [BlueInactive(BLU.RamsVoice, BLU.Ultravibration)]
    [ReplaceSkill(BLU.Ultravibration)]
    [JobInfo(Job.BLU)]
    BLU_Ultravibrate = 70005,

    [BlueInactive(BLU.HydroPull)]
    [ParentCombo(BLU_Ultravibrate)]
    [JobInfo(Job.BLU)]
    BLU_HydroPull = 70012,

    [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance)]
    [ReplaceSkill(BLU.FeatherRain)]
    [JobInfo(Job.BLU)]
    [Retargeted(BLU.FeatherRain)]
    BLU_PrimalCombo = 70008,

    [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_Pool = 70015,

    [BlueInactive(BLU.JKick)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_JKick = 70013,

    [BlueInactive(BLU.SeaShanty)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_SeaShanty = 70024,

    [BlueInactive(BLU.WingedReprobation)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_WingedReprobation = 70025,

    [BlueInactive(BLU.MatraMagic)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_Matra = 70017,

    [BlueInactive(BLU.Surpanakha)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_Suparnakha = 70018,

    [BlueInactive(BLU.PhantomFlurry)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_PhantomFlurry = 70019,

    [BlueInactive(BLU.Nightbloom, BLU.Bristle)]
    [ParentCombo(BLU_PrimalCombo)]
    [JobInfo(Job.BLU)]
    BLU_PrimalCombo_Nightbloom = 70020,

    [BlueInactive(BLU.SongOfTorment, BLU.Bristle)]
    [ReplaceSkill(BLU.SongOfTorment)]
    [JobInfo(Job.BLU)]
    BLU_BuffedSoT = 70000,

    [BlueInactive(BLU.PeripheralSynthesis, BLU.MustardBomb)]
    [ReplaceSkill(BLU.PeripheralSynthesis)]
    [JobInfo(Job.BLU)]
    BLU_LightHeadedCombo = 70010,

    [BlueInactive(BLU.PerpetualRay, BLU.SharpenedKnife)]
    [JobInfo(Job.BLU)]
    BLU_PerpetualRayStunCombo = 70014,

    [BlueInactive(BLU.SonicBoom, BLU.SharpenedKnife)]
    [JobInfo(Job.BLU)]
    BLU_MeleeCombo = 70016,

    [BlueInactive(BLU.MagicHammer)]
    [ReplaceSkill(BLU.MagicHammer)]
    [JobInfo(Job.BLU)]
    BLU_Addle = 70007,

    [BlueInactive(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
    [ReplaceSkill(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
    [JobInfo(Job.BLU)]
    BLU_KnightCombo = 70009,

    [BlueInactive(BLU.Offguard, BLU.BadBreath, BLU.Devour)]
    [ReplaceSkill(BLU.Devour, BLU.Offguard, BLU.BadBreath)]
    [JobInfo(Job.BLU)]
    BLU_DebuffCombo = 70006,

    [ReplaceSkill(BLU.DeepClean)]
    [BlueInactive(BLU.PeatPelt, BLU.DeepClean)]
    [JobInfo(Job.BLU)]
    BLU_PeatClean = 70023,

    // Last value = 70023

    #endregion

    #region BARD

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    [SimpleCombo]
    BRD_ST_SimpleMode = 3036,

    [AutoAction(true, false)]
    [ConflictingCombos(BRD_AoE_AdvMode)]
    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [JobInfo(Job.BRD)]
    [SimpleCombo]
    BRD_AoE_SimpleMode = 3035,

    #endregion

    #region Advanced Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_SimpleMode)]
    [JobInfo(Job.BRD)]
    [AdvancedCombo]
    BRD_ST_AdvMode = 3009,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_ST_Adv_Balance_Standard = 3048,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_Song = 3011,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_DoT = 3010,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_Buffs = 3017,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_BuffsResonant = 3041,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_BuffsEncore = 3042,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_ST_ApexArrow = 3021,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_ST_Adv_oGCD = 3038,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_Pooling = 3023,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_ApexPooling = 3057,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_Interrupt = 3020,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_ST_SecondWind = 3028,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_ST_Wardens = 3047,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_Troubadour = 3069,

    [ParentCombo(BRD_ST_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_Adv_NaturesMinne = 3070,

    [AutoAction(true, false)]
    [ConflictingCombos(BRD_AoE_SimpleMode)]
    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [JobInfo(Job.BRD)]
    [AdvancedCombo]
    BRD_AoE_AdvMode = 3015,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    [Retargeted(BRD.Windbite, BRD.VenomousBite, BRD.IronJaws, BRD.CausticBite, BRD.Stormbite)]
    BRD_AoE_Adv_Multidot = 3065,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_Adv_Songs = 3016,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_Adv_Buffs = 3032,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_Adv_oGCD = 3037,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_Pooling = 3040,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_ApexPooling = 3058,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_Adv_Interrupt = 3043,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_BuffsEncore = 3062,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_BuffsResonant = 3061,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_ApexArrow = 3039,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_SecondWind = 3029,

    [ParentCombo(BRD_AoE_AdvMode)]
    [JobInfo(Job.BRD)]
    BRD_AoE_Wardens = 3046,

    #endregion

    #region Smaller Features

    [ReplaceSkill(BRD.StraightShot, BRD.RefulgentArrow)]
    [JobInfo(Job.BRD)]
    BRD_StraightShotUpgrade = 3001,

    [ParentCombo(BRD_StraightShotUpgrade)]
    [JobInfo(Job.BRD)]
    BRD_StraightShotUpgrade_OGCDs = 3002,

    [ParentCombo(BRD_StraightShotUpgrade)]
    [JobInfo(Job.BRD)]
    BRD_DoTMaintainance = 3067,

    [ReplaceSkill(BRD.IronJaws)]
    [JobInfo(Job.BRD)]
    BRD_IronJaws = 3003,

    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [JobInfo(Job.BRD)]
    BRD_WideVolleyUpgrade = 3008,

    [ParentCombo(BRD_WideVolleyUpgrade)]
    [JobInfo(Job.BRD)]
    BRD_WideVolleyUpgrade_OGCDs = 3068,

    [ParentCombo(BRD_WideVolleyUpgrade)]
    [JobInfo(Job.BRD)]
    BRD_WideVolleyUpgrade_Apex = 3005,

    [ReplaceSkill(BRD.Bloodletter)]
    [JobInfo(Job.BRD)]
    BRD_ST_oGCD = 3006,

    [ParentCombo(BRD_ST_oGCD)]
    [JobInfo(Job.BRD)]
    BRD_ST_oGCD_Songs = 3044,

    [ReplaceSkill(BRD.RainOfDeath)]
    [JobInfo(Job.BRD)]
    BRD_AoE_oGCD = 3007,

    [ParentCombo(BRD_AoE_oGCD)]
    [JobInfo(Job.BRD)]
    BRD_AoE_oGCD_Songs = 3045,

    [ReplaceSkill(BRD.Barrage)]
    [JobInfo(Job.BRD)]
    BRD_Buffs = 3013,

    [ReplaceSkill(BRD.WanderersMinuet)]
    [JobInfo(Job.BRD)]
    BRD_OneButtonSongs = 3014,

    [ReplaceSkill(BRD.VenomousBite, BRD.CausticBite)]
    [JobInfo(Job.BRD)]
    BRD_OneButtonDots = 3004,

    [ParentCombo(BRD_OneButtonDots)]
    [JobInfo(Job.BRD)]
    BRD_OneButtonDots_IronJaws = 3071,

    [Retargeted]
    [ParentCombo(BRD_OneButtonDots)]
    [JobInfo(Job.BRD)]
    BRD_OneButtonDots_Retargeted = 3072,

    [ParentCombo(BRD_OneButtonDots_Retargeted)]
    [JobInfo(Job.BRD)]
    BRD_OneButtonDots_SavageBlade = 3073,

    #endregion
    #region Hidden
    [JobInfo(Job.BRD)]
    [Hidden]
    BRD_Hidden_Song_Extension = 3074,
    #endregion

    // Last value = 3074

    #endregion

    #region DANCER

    [ReplaceSkill(DNC.StandardFinish2, DNC.TechnicalFinish4)]
    [JobInfo(Job.DNC)]
    DNC_ST_BlockFinishes = 4000,

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_MultiButton, DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    [SimpleCombo]
    DNC_ST_SimpleMode = 4001,

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_AdvancedMode, DNC_Procc_Windmill)]
    [JobInfo(Job.DNC)]
    [SimpleCombo]
    DNC_AoE_SimpleMode = 4002,

    #endregion
    // Last value = 4002

    #region Advanced Dancer (Single Target)

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_MultiButton, DNC_ST_SimpleMode)]
    [JobInfo(Job.DNC)]
    [AdvancedCombo]
    DNC_ST_AdvancedMode = 4010,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_BalanceOpener = 4011,

    [ParentCombo(DNC_ST_BalanceOpener)]
    [JobInfo(Job.DNC)]
    DNC_ST_Opener_BlockEarly = 4031,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Partner = 4012,

    [ParentCombo(DNC_ST_Adv_Partner)]
    [JobInfo(Job.DNC)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_ST_Adv_PartnerAuto = 4033,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_ST_Adv_AutoPartner = 4032,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Peloton = 4013,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Interrupt = 4014,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_SS = 4015,

    [ParentCombo(DNC_ST_Adv_SS)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_FM = 4016,

    [ParentCombo(DNC_ST_Adv_SS)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_SS_Prepull = 4017,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_TS = 4018,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Devilment = 4019,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Flourish = 4020,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_FanProccs = 4028,

    [ParentCombo(DNC_ST_Adv_FanProccs)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_FanProcc3 = 4029,

    [ParentCombo(DNC_ST_Adv_FanProccs)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_FanProcc4 = 4030,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Feathers = 4021,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Improvisation = 4022,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_Tillana = 4023,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_SaberDance = 4024,

    [ParentCombo(DNC_ST_Adv_SaberDance)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_DawnDance = 4025,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_LD = 4026,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_Adv_PanicHeals = 4027,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    DNC_ST_Adv_ShieldSamba = 4034,

    #endregion
    // Last value = 4034

    #region Advanced Dancer (AoE)

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_SimpleMode, DNC_Procc_Windmill)]
    [JobInfo(Job.DNC)]
    [AdvancedCombo]
    DNC_AoE_AdvancedMode = 4040,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Partner = 4041,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Interrupt = 4042,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_SS = 4043,

    [ParentCombo(DNC_AoE_Adv_SS)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_FM = 4044,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_TS = 4045,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Devilment = 4046,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Flourish = 4047,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_FanProccs = 4055,

    [ParentCombo(DNC_AoE_Adv_FanProccs)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_FanProcc3 = 4056,

    [ParentCombo(DNC_AoE_Adv_FanProccs)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_FanProcc4 = 4057,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Feathers = 4048,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Improvisation = 4049,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_Tillana = 4050,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_SaberDance = 4051,

    [ParentCombo(DNC_AoE_Adv_SaberDance)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_DawnDance = 4052,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_LD = 4053,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [JobInfo(Job.DNC)]
    DNC_AoE_Adv_PanicHeals = 4054,

    #endregion
    // Last value = 4057

    #region Basic combo

    [ReplaceSkill(DNC.Fountain)]
    [JobInfo(Job.DNC)]
    [BasicCombo]
    DNC_ST_BasicCombo = 4003,

    #endregion
    // Last value = 4003

    #region Multibutton Features

    #region Single Target Multibutton

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_AdvancedMode, DNC_ST_SimpleMode)]
    [JobInfo(Job.DNC)]
    DNC_ST_MultiButton = 4070,

    [ParentCombo(DNC_ST_MultiButton)]
    [JobInfo(Job.DNC)]
    DNC_ST_EspritOvercap = 4071,

    [ParentCombo(DNC_ST_MultiButton)]
    [JobInfo(Job.DNC)]
    DNC_ST_FanDanceOvercap = 4072,

    [ParentCombo(DNC_ST_MultiButton)]
    [JobInfo(Job.DNC)]
    DNC_ST_FanDance34 = 4073,

    #endregion
    // Last value = 4073

    #region AoE Multibutton

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_AdvancedMode, DNC_AoE_SimpleMode, DNC_Procc_Windmill)]
    [JobInfo(Job.DNC)]
    DNC_AoE_MultiButton = 4090,

    [ParentCombo(DNC_AoE_MultiButton)]
    [JobInfo(Job.DNC)]
    DNC_AoE_EspritOvercap = 4091,

    [ParentCombo(DNC_AoE_MultiButton)]
    [JobInfo(Job.DNC)]
    DNC_AoE_FanDanceOvercap = 4092,

    [ParentCombo(DNC_AoE_MultiButton)]
    [JobInfo(Job.DNC)]
    DNC_AoE_FanDance34 = 4093,

    #endregion
    // Last value = 4093

    #region Smaller Features

    #region Dance Partner Features

    [ReplaceSkill(DNC.ClosedPosition, DNC.Ending)]
    [JobInfo(Job.DNC)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_DesirablePartner = 4175,

    #endregion
    // Last value = 4176

    #region Dance Features

    [JobInfo(Job.DNC)]
    DNC_CustomDanceSteps = 4115,

    [ParentCombo(DNC_CustomDanceSteps)]
    [JobInfo(Job.DNC)]
    DNC_CustomDanceSteps_Conflicts = 4116,

    [JobInfo(Job.DNC)]
    DNC_DanceFeatures = 4111,

    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.StandardStep)]
    [JobInfo(Job.DNC)]
    DNC_StandardStepCombo = 4110,

    // StandardStep(or Finishing Move) --> Last Dance
    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.StandardStep, DNC.FinishingMove)]
    [JobInfo(Job.DNC)]
    DNC_StandardStep_LastDance = 4155,

    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.TechnicalStep)]
    [JobInfo(Job.DNC)]
    DNC_TechnicalStepCombo = 4112,

    // Technical Step --> Devilment
    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.TechnicalStep)]
    [JobInfo(Job.DNC)]
    DNC_TechnicalStep_Devilment = 4160,

    #endregion
    // Last value = 4116

    #region Fan Features

    [ReplaceSkill(DNC.Flourish)]
    [JobInfo(Job.DNC)]
    DNC_FlourishingFanDances = 4130,

    [ParentCombo(DNC_FlourishingFanDances)]
    [JobInfo(Job.DNC)]
    DNC_Flourishing_FD3 = 4131,

    [JobInfo(Job.DNC)]
    DNC_FanDanceCombos = 4135,

    [ReplaceSkill(DNC.FanDance1)]
    [ParentCombo(DNC_FanDanceCombos)]
    [JobInfo(Job.DNC)]
    DNC_FanDance_1to3_Combo = 4136,

    [ReplaceSkill(DNC.FanDance1)]
    [ParentCombo(DNC_FanDanceCombos)]
    [JobInfo(Job.DNC)]
    DNC_FanDance_1to4_Combo = 4137,

    [ReplaceSkill(DNC.FanDance2)]
    [ParentCombo(DNC_FanDanceCombos)]
    [JobInfo(Job.DNC)]
    DNC_FanDance_2to3_Combo = 4138,

    [ReplaceSkill(DNC.FanDance2)]
    [ParentCombo(DNC_FanDanceCombos)]
    [JobInfo(Job.DNC)]
    DNC_FanDance_2to4_Combo = 4139,

    #endregion
    // Last value = 4139

    // Bladeshower --> Bloodshower
    [ReplaceSkill(DNC.Bladeshower)]
    [JobInfo(Job.DNC)]
    DNC_Procc_Bladeshower = 4165,

    // Windmill --> Rising Windmill
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_AdvancedMode, DNC_AoE_SimpleMode)]
    [JobInfo(Job.DNC)]
    DNC_Procc_Windmill = 4170,

    #endregion
    // Last value = 4176

    #endregion
    // Last value = 4195

    #endregion

    #region DARK KNIGHT

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DRK.HardSlash)]
    [ConflictingCombos(DRK_ST_Adv)]
    [JobInfo(Job.DRK)]
    [SimpleCombo]
    DRK_ST_Simple = 5001,

    [AutoAction(true, false)]
    [ReplaceSkill(DRK.Unleash)]
    [ConflictingCombos(DRK_AoE_Adv)]
    [JobInfo(Job.DRK)]
    [SimpleCombo]
    DRK_AoE_Simple = 5002,

    #endregion
    // Last value = 5003

    #region Advanced Single Target Combo

    [AutoAction(false, false)]
    [ReplaceSkill(DRK.HardSlash)]
    [ConflictingCombos(DRK_ST_Simple)]
    [JobInfo(Job.DRK)]
    [AdvancedCombo]
    DRK_ST_Adv = 5010,

    [ParentCombo(DRK_ST_Adv)]
    [JobInfo(Job.DRK)]
    DRK_ST_BalanceOpener = 5011,

    [ParentCombo(DRK_ST_Adv)]
    [JobInfo(Job.DRK)]
    DRK_ST_RangedUptime = 5012,

    #region Cooldowns

    [ParentCombo(DRK_ST_Adv)]
    [JobInfo(Job.DRK)]
    DRK_ST_CDs = 5013,

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Interrupt = 5014,

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Stun = 5040,

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Delirium = 5015,

    #region Living Shadow Options

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Shadow = 5016,

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Disesteem = 5017,

    #endregion

    #region Shadowbringer Options

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Bringer = 5018,

    [ParentCombo(DRK_ST_CD_Bringer)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_BringerBurst = 5019,

    #endregion

    #region Salt Options

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Salt = 5020,

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Darkness = 5021,

    #endregion

    [ParentCombo(DRK_ST_CDs)]
    [JobInfo(Job.DRK)]
    DRK_ST_CD_Spit = 5022,

    #endregion

    #region Spenders

    [ParentCombo(DRK_ST_Adv)]
    [JobInfo(Job.DRK)]
    DRK_ST_Spenders = 5023,

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_ScarletChain = 5024,

    #region Blood

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_Bloodspiller = 5025,

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_BloodOvercap = 5026,

    #endregion

    #region Mana

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_Edge = 5027,

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_DarkArts = 5028,

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_EdgeDarkside = 5029,

    [ParentCombo(DRK_ST_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_ST_Sp_ManaOvercap = 5030,

    #endregion

    #endregion

    #endregion
    // Last value = 5040

    #region Advanced Multi Target Combo

    [AutoAction(true, false)]
    [ReplaceSkill(DRK.Unleash)]
    [ConflictingCombos(DRK_AoE_Simple)]
    [JobInfo(Job.DRK)]
    [AdvancedCombo]
    DRK_AoE_Adv = 5050,

    #region Cooldowns

    [ParentCombo(DRK_AoE_Adv)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CDs = 5051,

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Interrupt = 5052,

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Stun = 5053,

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Delirium = 5054,

    #region Living Shadow Options

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Shadow = 5055,

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Disesteem = 5056,

    #endregion

    #region Shadowbringer Options

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Bringer = 5057,
    
    [ParentCombo(DRK_AoE_CD_Bringer)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_BringerBurst = 5076,

    #endregion

    #region Salt Options

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Salt = 5058,

    [ParentCombo(DRK_AoE_CD_Salt)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_SaltStill = 5059,

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Darkness = 5077,

    #endregion

    [ParentCombo(DRK_AoE_CDs)]
    [JobInfo(Job.DRK)]
    DRK_AoE_CD_Drain = 5060,

    #endregion

    #region Spenders

    [ParentCombo(DRK_AoE_Adv)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Spenders = 5061,

    [ParentCombo(DRK_AoE_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Sp_ImpalementChain = 5062,

    #region Blood

    [ParentCombo(DRK_AoE_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Sp_Quietus = 5063,

    [ParentCombo(DRK_AoE_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Sp_BloodOvercap = 5064,

    #endregion

    #region Mana

    [ParentCombo(DRK_AoE_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Sp_Flood = 5065,

    [ParentCombo(DRK_AoE_Spenders)]
    [JobInfo(Job.DRK)]
    DRK_AoE_Sp_ManaOvercap = 5066,

    #endregion

    #endregion

    #endregion
    // Last value = 5077

    #region Advanced Mitigation
    [JobInfo(Job.DRK)]
    DRK_Mitigation = 5300,

    [ParentCombo(DRK_Mitigation)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss = 5301,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_BlackestNight = 5307,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_LivingDead = 5305,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_Rampart = 5302,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_DarkMind = 5304,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_ShadowWall = 5303,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_ArmsLength = 5306,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_Reprisal = 5313,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_DarkMissionary = 5308,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_NonBoss_Oblation = 5318,

    [ParentCombo(DRK_Mitigation)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss = 5309,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_BlackestNight_OnCD = 5310,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_BlackestNight_TB = 5314,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_Oblation = 5319,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_Rampart = 5316,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_ShadowWall = 5315,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_DarkMind = 5317,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_Reprisal = 5311,

    [ParentCombo(DRK_Mitigation_Boss)]
    [JobInfo(Job.DRK)]
    DRK_Mitigation_Boss_DarkMissionary = 5312,

    #endregion
    // Lastvalue = 5319

    #region Basic combo

    [ReplaceSkill(DRK.Souleater)]
    [JobInfo(Job.DRK)]
    [BasicCombo]
    DRK_ST_BasicCombo = 5003,

    [ReplaceSkill(DRK.StalwartSoul)]
    [JobInfo(Job.DRK)]
    [BasicCombo]
    DRK_AoE_BasicCombo = 5004,

    #endregion

    #region Multibutton Features

    #region One-Button Mitigation

    [ReplaceSkill(DRK.DarkMind)]
    [JobInfo(Job.DRK)]
    [MitigationCombo]
    DRK_Mit_OneButton = 5090,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_LivingDead_Max = 5091,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_TheBlackestNight = 5092,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_Oblation = 5093,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_Reprisal = 5094,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_DarkMissionary = 5095,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_Rampart = 5096,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_DarkMind = 5097,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_ArmsLength = 5098,

    [ParentCombo(DRK_Mit_OneButton)]
    [JobInfo(Job.DRK)]
    DRK_Mit_ShadowWall = 5099,

    [ReplaceSkill(DRK.DarkMissionary)]
    [JobInfo(Job.DRK)]
    [MitigationCombo]
    DRK_Mit_Party = 5100,

    #endregion
    // Last value = 5100

    #region oGCD Feature

    [ReplaceSkill(DRK.CarveAndSpit, DRK.AbyssalDrain)]
    [JobInfo(Job.DRK)]
    DRK_oGCD = 5120,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_Interrupt = 5121,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_Delirium = 5122,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_Shadow = 5124,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_Disesteem = 5125,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_SaltedEarth = 5126,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_SaltAndDarkness = 5127,

    [ParentCombo(DRK_oGCD)]
    [JobInfo(Job.DRK)]
    DRK_oGCD_Shadowbringer = 5123,

    #endregion
    // Last value = 5123

    #region Standalones

    [ReplaceSkill(DRK.BlackestNight)]
    [JobInfo(Job.DRK)]
    [Retargeted(DRK.BlackestNight)]
    DRK_Retarget_TBN = 5130,

    [ParentCombo(DRK_Retarget_TBN)]
    [JobInfo(Job.DRK)]
    [Retargeted]
    DRK_Retarget_TBN_TT = 5131,

    [ReplaceSkill(DRK.Oblation)]
    [JobInfo(Job.DRK)]
    [Retargeted(DRK.Oblation)]
    DRK_Retarget_Oblation = 5132,

    [ParentCombo(DRK_Retarget_Oblation)]
    [JobInfo(Job.DRK)]
    [Retargeted]
    DRK_Retarget_Oblation_TT = 5133,

    [ParentCombo(DRK_Retarget_Oblation)]
    [JobInfo(Job.DRK)]
    DRK_Retarget_Oblation_DoubleProtection = 5134,

    [ReplaceSkill(DRK.Shadowstride)]
    [JobInfo(Job.DRK)]
    [Retargeted(DRK.Shadowstride)]
    DRK_RetargetShadowstride = 5135,

    #endregion
    // Last value = 5135

    #endregion
    // Last value = 5135

    #region Hidden Features

    [JobInfo(Job.DRK)]
    [Hidden]
    DRK_Hidden = 5200,

    #endregion
    // Last value = 5200

    #endregion

    #region DRAGOON

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DRG.TrueThrust)]
    [ConflictingCombos(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    [SimpleCombo]
    DRG_ST_SimpleMode = 6001,

    [AutoAction(true, false)]
    [ReplaceSkill(DRG.DoomSpike)]
    [ConflictingCombos(DRG_AoE_AdvancedMode)]
    [JobInfo(Job.DRG)]
    [SimpleCombo]
    DRG_AoE_SimpleMode = 6200,

    #endregion

    #region Advanced ST Dragoon

    [AutoAction(false, false)]
    [ReplaceSkill(DRG.TrueThrust)]
    [ConflictingCombos(DRG_ST_SimpleMode)]
    [JobInfo(Job.DRG)]
    [AdvancedCombo]
    DRG_ST_AdvancedMode = 6100,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_ST_Opener = 6101,

    #region Buffs ST

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_ST_Buffs = 6102,

    [ParentCombo(DRG_ST_Buffs)]
    [JobInfo(Job.DRG)]
    DRG_ST_BattleLitany = 6103,

    [ParentCombo(DRG_ST_Buffs)]
    [JobInfo(Job.DRG)]
    DRG_ST_LanceCharge = 6104,

    [ParentCombo(DRG_ST_Buffs)]
    [JobInfo(Job.DRG)]
    DRG_ST_LifeSurge = 6106,

    #endregion

    #region Damage SKills

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_ST_Damage = 6105,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_HighJump = 6113,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_Mirage = 6115,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_Geirskogul = 6116,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_Nastrond = 6117,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_Wyrmwind = 6118,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_Stardiver = 6110,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_Starcross = 6112,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_DragonfireDive = 6107,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_RiseOfTheDragon = 6109,

    [ParentCombo(DRG_ST_Damage)]
    [JobInfo(Job.DRG)]
    DRG_ST_RangedUptime = 6197,

    #endregion

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_TrueNorthDynamic = 6199,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_ST_StunInterupt = 6196,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_ST_ComboHeals = 6198,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_ST_Feint = 6195,

    #endregion

    #region Advanced AoE Dragoon

    [AutoAction(true, false)]
    [ReplaceSkill(DRG.DoomSpike)]
    [ConflictingCombos(DRG_AoE_SimpleMode)]
    [JobInfo(Job.DRG)]
    [AdvancedCombo]
    DRG_AoE_AdvancedMode = 6201,

    #region Buffs AoE

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Buffs = 6202,

    [ParentCombo(DRG_AoE_Buffs)]
    [JobInfo(Job.DRG)]
    DRG_AoE_BattleLitany = 6203,

    [ParentCombo(DRG_AoE_Buffs)]
    [JobInfo(Job.DRG)]
    DRG_AoE_LanceCharge = 6204,

    [ParentCombo(DRG_AoE_Buffs)]
    [JobInfo(Job.DRG)]
    DRG_AoE_LifeSurge = 6206,

    #endregion

    #region cooldowns AoE

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Damage = 6205,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_HighJump = 6213,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Mirage = 6215,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Geirskogul = 6216,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Nastrond = 6217,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Wyrmwind = 6218,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Stardiver = 6210,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Starcross = 6212,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_DragonfireDive = 6207,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_RiseOfTheDragon = 6209,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_RangedUptime = 6298,

    [ParentCombo(DRG_AoE_Damage)]
    [JobInfo(Job.DRG)]
    DRG_AoE_Disembowel = 6297,

    #endregion

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_AoE_StunInterupt = 6296,

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [JobInfo(Job.DRG)]
    DRG_AoE_ComboHeals = 6299,

    #endregion

    #region Basic Combo

    [ReplaceSkill(DRG.FullThrust, DRG.HeavensThrust)]
    [JobInfo(Job.DRG)]
    [BasicCombo]
    DRG_HeavensThrust = 6304,

    [ReplaceSkill(DRG.ChaosThrust, DRG.ChaoticSpring)]
    [JobInfo(Job.DRG)]
    [BasicCombo]
    DRG_ChaoticSpring = 6305,

    #endregion

    [ReplaceSkill(DRG.LanceCharge)]
    [JobInfo(Job.DRG)]
    DRG_BurstCDFeature = 6301,

    // Last value ST = 6120
    // Last value AoE = 6216

    #endregion

    #region GUNBREAKER

    #region Simple Mode
    [AutoAction(false, false)]
    [ConflictingCombos(GNB_ST_Advanced)]
    [ReplaceSkill(GNB.KeenEdge)]
    [JobInfo(Job.GNB)]
    [SimpleCombo]
    GNB_ST_Simple = 7001,

    [AutoAction(true, false)]
    [ConflictingCombos(GNB_AoE_Advanced)]
    [ReplaceSkill(GNB.DemonSlice)]
    [JobInfo(Job.GNB)]
    [SimpleCombo]
    GNB_AoE_Simple = 7002,
    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ConflictingCombos(GNB_ST_Simple)]
    [ReplaceSkill(GNB.KeenEdge)]
    [JobInfo(Job.GNB)]
    [AdvancedCombo]
    GNB_ST_Advanced = 7003,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Opener = 7006,

    [ConflictingCombos(GNB_NM_Features)]
    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_NoMercy = 7008,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Zone = 7009,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_BurstStrike = 7015,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_SonicBreak = 7012,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_GnashingFang = 7016,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_BowShock = 7010,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Continuation = 7005,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Bloodfest = 7011,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_DoubleDown = 7017,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Reign = 7014,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_RangedUptime = 7004,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Interrupt = 7084,

    [ParentCombo(GNB_ST_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_ST_Stun = 7086,

    #endregion

    #region Advanced AoE
    [AutoAction(true, false)]
    [ConflictingCombos(GNB_AoE_Simple)]
    [ReplaceSkill(GNB.DemonSlice)]
    [JobInfo(Job.GNB)]
    [AdvancedCombo]
    GNB_AoE_Advanced = 7200,

    [ConflictingCombos(GNB_NM_Features)]
    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_NoMercy = 7201,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_Zone = 7202,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_BowShock = 7203,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_Bloodfest = 7204,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_SonicBreak = 7205,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_DoubleDown = 7206,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_Reign = 7207,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_FatedCircle = 7208,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_Interrupt = 7222,

    [ParentCombo(GNB_AoE_Advanced)]
    [JobInfo(Job.GNB)]
    GNB_AoE_Stun = 7223,

    #endregion

    #region Advanced Mitigation
    //7700s LastUsed 7713
    [JobInfo(Job.GNB)]
    GNB_Mitigation = 7700,

    [ParentCombo(GNB_Mitigation)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss = 7701,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_Rampart = 7702,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_Nebula = 7703,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_Camouflage = 7704,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_Aurora = 7705,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_Superbolide = 7706,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_SuperBolideEmergency = 7721,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_ArmsLength = 7707,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_Reprisal = 7708,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_HeartOfLight = 7709,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_NonBoss_HeartOfStone = 7710,

    [ParentCombo(GNB_Mitigation)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss = 7711,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_Aurora = 7712,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_HeartOfStone_OnCD = 7716,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_HeartOfStone_TankBuster = 7717,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_Rampart = 7719,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_Nebula = 7718,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_Camouflage = 7720,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_Reprisal = 7713,

    [ParentCombo(GNB_Mitigation_Boss)]
    [JobInfo(Job.GNB)]
    GNB_Mitigation_Boss_HeartOfLight = 7714,
    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(GNB.Camouflage)]
    [JobInfo(Job.GNB)]
    [MitigationCombo]
    GNB_Mit_OneButton = 7074,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Superbolide_Max = 7075,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Corundum = 7076,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Aurora = 7077,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Camouflage = 7078,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Reprisal = 7079,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_HeartOfLight = 7080,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Rampart = 7081,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_ArmsLength = 7082,

    [ParentCombo(GNB_Mit_OneButton)]
    [JobInfo(Job.GNB)]
    GNB_Mit_Nebula = 7083,

    [ReplaceSkill(GNB.HeartOfLight)]
    [JobInfo(Job.GNB)]
    [MitigationCombo]
    GNB_Mit_Party = 7085,
    #endregion

    #region Misc

    #region Basic combo

    [ReplaceSkill(GNB.SolidBarrel)]
    [JobInfo(Job.GNB)]
    [BasicCombo]
    GNB_ST_BasicCombo = 7100,

    [ReplaceSkill(GNB.DemonSlaughter)]
    [JobInfo(Job.GNB)]
    [BasicCombo]
    GNB_AoE_BasicCombo = 7101,

    #endregion

    #region Gnashing Fang
    [ReplaceSkill(GNB.GnashingFang)]
    [JobInfo(Job.GNB)]
    GNB_GF_Features = 7300,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_NoMercy = 7302,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_Zone = 7303,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_BurstStrike = 7309,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_SonicBreak = 7306,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_BowShock = 7304,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_Continuation = 7301,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_Bloodfest = 7305,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_DoubleDown = 7307,

    [ParentCombo(GNB_GF_Features)]
    [JobInfo(Job.GNB)]
    GNB_GF_Reign = 7308,
    #endregion

    #region No Mercy
    [ReplaceSkill(GNB.NoMercy)]
    [JobInfo(Job.GNB)]
    GNB_NM_Features = 7500,

    [ParentCombo(GNB_NM_Features)]
    [JobInfo(Job.GNB)]
    GNB_NM_Bloodfest = 7501,

    [ParentCombo(GNB_NM_Features)]
    [JobInfo(Job.GNB)]
    GNB_NM_Zone = 7502,

    [ParentCombo(GNB_NM_Features)]
    [JobInfo(Job.GNB)]
    GNB_NM_BowShock = 7503,

    [ParentCombo(GNB_NM_Features)]
    [JobInfo(Job.GNB)]
    GNB_NM_Continuation = 7504,
    #endregion

    #region Burst Strike

    [ReplaceSkill(GNB.BurstStrike)]
    [JobInfo(Job.GNB)]
    GNB_BS_Features = 7400,

    [ParentCombo(GNB_BS_Features)]
    [JobInfo(Job.GNB)]
    GNB_BS_Continuation = 7401,

    [ParentCombo(GNB_BS_Continuation)]
    [JobInfo(Job.GNB)]
    GNB_BS_Hypervelocity = 7406,

    [ParentCombo(GNB_BS_Features)]
    [JobInfo(Job.GNB)]
    GNB_BS_Bloodfest = 7402,

    [ParentCombo(GNB_BS_Features)]
    [JobInfo(Job.GNB)]
    GNB_BS_GnashingFang = 7405,

    [ParentCombo(GNB_BS_Features)]
    [JobInfo(Job.GNB)]
    GNB_BS_DoubleDown = 7403,

    [ParentCombo(GNB_BS_Features)]
    [JobInfo(Job.GNB)]
    GNB_BS_Reign = 7404,

    #endregion

    #region Fated Circle

    [ReplaceSkill(GNB.FatedCircle)]
    [JobInfo(Job.GNB)]
    GNB_FC_Features = 7600,

    [ParentCombo(GNB_FC_Features)]
    [JobInfo(Job.GNB)]
    GNB_FC_Continuation = 7601,

    [ParentCombo(GNB_FC_Features)]
    [JobInfo(Job.GNB)]
    GNB_FC_Bloodfest = 7602,

    [ParentCombo(GNB_FC_Features)]
    [JobInfo(Job.GNB)]
    GNB_FC_BowShock = 7605,

    [ParentCombo(GNB_FC_Features)]
    [JobInfo(Job.GNB)]
    GNB_FC_DoubleDown = 7603,

    [ParentCombo(GNB_FC_DoubleDown)]
    [JobInfo(Job.GNB)]
    GNB_FC_DoubleDown_NM = 7606,

    [ParentCombo(GNB_FC_Features)]
    [JobInfo(Job.GNB)]
    GNB_FC_Reign = 7604,

    #endregion

    #region Aurora Protection
    [ReplaceSkill(GNB.Aurora)]
    [JobInfo(Job.GNB)]
    GNB_AuroraProtection = 7023,

    [ParentCombo(GNB_AuroraProtection)]
    [JobInfo(Job.GNB)]
    [Retargeted(GNB.Aurora)]
    GNB_RetargetAurora_MO = 7087,

    [ParentCombo(GNB_AuroraProtection)]
    [JobInfo(Job.GNB)]
    [Retargeted(GNB.Aurora)]
    GNB_RetargetAurora_TT = 7088,

    #endregion

    #region Heart Of Stone Retarget
    [ReplaceSkill(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    [JobInfo(Job.GNB)]
    [Retargeted(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    GNB_RetargetHeartofStone = 7089,

    [ParentCombo(GNB_RetargetHeartofStone)]
    [JobInfo(Job.GNB)]
    [Retargeted(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    GNB_RetargetHeartofStone_TT = 7090,
    #endregion

    [ReplaceSkill(GNB.Trajectory)]
    [JobInfo(Job.GNB)]
    [Retargeted(GNB.Trajectory)]
    GNB_RetargetTrajectory = 7091,

    #endregion

    // Last Value = 7091
    #endregion

    #region MACHINIST

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(MCH.SplitShot, MCH.HeatedSplitShot)]
    [ConflictingCombos(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    [SimpleCombo]
    MCH_ST_SimpleMode = 8001,

    [AutoAction(true, false)]
    [ReplaceSkill(MCH.SpreadShot, MCH.Scattergun)]
    [ConflictingCombos(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    [SimpleCombo]
    MCH_AoE_SimpleMode = 8200,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(MCH.SplitShot, MCH.HeatedSplitShot)]
    [ConflictingCombos(MCH_ST_SimpleMode)]
    [JobInfo(Job.MCH)]
    [AdvancedCombo]
    MCH_ST_AdvancedMode = 8100,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Opener = 8101,

    #region BS

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Stabilizer = 8110,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Stabilizer_FullMetalField = 8111,

    #endregion

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_WildFire = 8108,

    #region Hypercharge

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Hypercharge = 8105,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Heatblast = 8106,

    #endregion

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_GaussRicochet = 8104,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Reassemble = 8103,

    #region Tools

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Tools = 8119,

    #endregion

    #region Queen

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_TurretQueen = 8107,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_QueenOverdrive = 8115,

    #endregion

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Interrupt = 8113,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_SecondWind = 8114,

    #region Raidwides

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Dismantle = 8195,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_ST_Adv_Tactician = 8118,

    #endregion

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(MCH.SpreadShot, MCH.Scattergun)]
    [ConflictingCombos(MCH_AoE_SimpleMode)]
    [JobInfo(Job.MCH)]
    [AdvancedCombo]
    MCH_AoE_AdvancedMode = 8300,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_FlameThrower = 8305,

    #region BS

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Stabilizer = 8307,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Stabilizer_FullMetalField = 8308,

    #endregion

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_GaussRicochet = 8302,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Hypercharge = 8303,

    #region Queen

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Queen = 8304,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_QueenOverdrive = 8314,

    #endregion

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Reassemble = 8301,

    #region Tools

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Tools = 8315,

    #endregion

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_Interrupt = 8311,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [JobInfo(Job.MCH)]
    MCH_AoE_Adv_SecondWind = 8399,

    #endregion

    #region Basic combo

    [ReplaceSkill(MCH.CleanShot, MCH.HeatedCleanShot)]
    [JobInfo(Job.MCH)]
    [BasicCombo]
    MCH_ST_BasicCombo = 8117,

    #endregion

    [ReplaceSkill(MCH.Dismantle)]
    [ConflictingCombos(MCH_DismantleTactician)]
    [JobInfo(Job.MCH)]
    MCH_DismantleProtection = 8042,

    [ReplaceSkill(MCH.Dismantle)]
    [ConflictingCombos(MCH_DismantleProtection)]
    [JobInfo(Job.MCH)]
    MCH_DismantleTactician = 8058,

    #region Heatblast

    [ReplaceSkill(MCH.Heatblast, MCH.BlazingShot)]
    [JobInfo(Job.MCH)]
    MCH_Heatblast = 8006,

    [ParentCombo(MCH_Heatblast)]
    [JobInfo(Job.MCH)]
    MCH_Heatblast_AutoBarrel = 8052,

    [ParentCombo(MCH_Heatblast)]
    [JobInfo(Job.MCH)]
    MCH_Heatblast_Wildfire = 8015,

    [ParentCombo(MCH_Heatblast)]
    [JobInfo(Job.MCH)]
    MCH_Heatblast_GaussRound = 8016,

    #endregion

    #region AutoCrossbow

    [ReplaceSkill(MCH.AutoCrossbow)]
    [JobInfo(Job.MCH)]
    MCH_AutoCrossbow = 8018,

    [ParentCombo(MCH_AutoCrossbow)]
    [JobInfo(Job.MCH)]
    MCH_AutoCrossbow_AutoBarrel = 8019,

    [ParentCombo(MCH_AutoCrossbow)]
    [JobInfo(Job.MCH)]
    MCH_AutoCrossbow_GaussRound = 8020,

    #endregion

    [ReplaceSkill(MCH.RookAutoturret, MCH.AutomatonQueen)]
    [JobInfo(Job.MCH)]
    MCH_Overdrive = 8002,

    [ReplaceSkill(MCH.HotShot)]
    [JobInfo(Job.MCH)]
    MCH_BigHitter = 8004,

    [ReplaceSkill(MCH.GaussRound, MCH.Ricochet, MCH.CheckMate, MCH.DoubleCheck)]
    [JobInfo(Job.MCH)]
    MCH_GaussRoundRicochet = 8003,

    // Last value ST = 8119
    // Last value AoE = 8315
    // Last value Misc = 8058

    #endregion

    #region MONK

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(MNK.Bootshine, MNK.LeapingOpo)]
    [ConflictingCombos(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    [SimpleCombo]
    MNK_ST_SimpleMode = 9004,

    [AutoAction(true, false)]
    [ReplaceSkill(MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
    [ConflictingCombos(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    [SimpleCombo]
    MNK_AOE_SimpleMode = 9003,

    #endregion

    #region Monk Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(MNK.Bootshine, MNK.LeapingOpo)]
    [ConflictingCombos(MNK_ST_SimpleMode)]
    [JobInfo(Job.MNK)]
    [AdvancedCombo]
    MNK_ST_AdvancedMode = 9005,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseMeditation = 9007,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseTheForbiddenChakra = 9012,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseFormShift = 9017,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseOpener = 9006,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseBuffs = 9008,

    [ParentCombo(MNK_STUseBuffs)]
    [JobInfo(Job.MNK)]
    MNK_STUseBrotherhood = 9009,

    [ParentCombo(MNK_STUseBuffs)]
    [JobInfo(Job.MNK)]
    MNK_STUseROF = 9011,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseFiresReply = 9016,

    [ParentCombo(MNK_STUseBuffs)]
    [JobInfo(Job.MNK)]
    MNK_STUseROW = 9010,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseWindsReply = 9015,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUsePerfectBalance = 9013,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseMasterfulBlitz = 9039,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_STUseTrueNorth = 9014,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_ST_StunInterupt = 9044,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_ST_ComboHeals = 9018,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_ST_Feint = 9095,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_ST_UseRoE = 9096,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_ST_UseMantra = 9097,

    #endregion

    #region Monk Advanced AOE

    [AutoAction(true, false)]
    [ReplaceSkill(MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
    [ConflictingCombos(MNK_AOE_SimpleMode)]
    [JobInfo(Job.MNK)]
    [AdvancedCombo]
    MNK_AOE_AdvancedMode = 9027,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseMeditation = 9028,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseHowlingFist = 9033,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseFormShift = 9038,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseBuffs = 9029,

    [ParentCombo(MNK_AoEUseBuffs)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseBrotherhood = 9030,

    [ParentCombo(MNK_AoEUseBuffs)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseROF = 9032,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseFiresReply = 9036,

    [ParentCombo(MNK_AoEUseBuffs)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseROW = 9031,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseWindsReply = 9035,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUsePerfectBalance = 9034,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoEUseMasterfulBlitz = 9040,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoE_StunInterupt = 9045,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [JobInfo(Job.MNK)]
    MNK_AoE_ComboHeals = 9037,

    #endregion

    #region Basic Combo
    [ReplaceSkill(MNK.SnapPunch, MNK.PouncingCoeurl)]
    [BasicCombo]
    [JobInfo(Job.MNK)]
    MNK_ST_BasicCombo = 9046,

    [JobInfo(Job.MNK)]
    MNK_Basic_BeastChakras = 9019,

    #endregion

    #region Movement

    [JobInfo(Job.MNK)]
    [Retargeted(MNK.Thunderclap)]
    MNK_Retarget_Thunderclap = 9043,

    #endregion

    #region Misc

    [ReplaceSkill(MNK.PerfectBalance)]
    [ConflictingCombos(MNK_PerfectBalanceProtection)]
    [JobInfo(Job.MNK)]
    MNK_PerfectBalance = 9023,

    [ReplaceSkill(MNK.RiddleOfFire, MNK.Brotherhood)]
    [JobInfo(Job.MNK)]
    MNK_Brotherhood_Riddle = 9024,

    [ReplaceSkill(MNK.PerfectBalance)]
    [ConflictingCombos(MNK_PerfectBalance)]
    [JobInfo(Job.MNK)]
    MNK_PerfectBalanceProtection = 9042,

    #endregion

    #region Hidden Features

    [Hidden]
    [JobInfo(Job.MNK)]
    MNK_Hidden = 9300,

    #endregion

    // Last value = 9046
    // Hidden = 9300

    #endregion

    #region NINJA

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(NIN.SpinningEdge)]
    [ConflictingCombos(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    [SimpleCombo]
    NIN_ST_SimpleMode = 10000,

    [AutoAction(true, false)]
    [ReplaceSkill(NIN.DeathBlossom)]
    [ConflictingCombos(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    [SimpleCombo]
    NIN_AoE_SimpleMode = 10001,

    #endregion

    #region ST Advanced
    [AutoAction(false, false)]
    [ReplaceSkill(NIN.SpinningEdge)]
    [ConflictingCombos(NIN_ST_SimpleMode)]
    [JobInfo(Job.NIN)]
    [AdvancedCombo]
    NIN_ST_AdvancedMode = 10002,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_BalanceOpener = 10004,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Ninjitsus = 10005,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Ninjitsus_Raiton = 10051,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Ninjitsus_Suiton = 10052,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Ninjitsus_Hyosho = 10053,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_TrickAttack = 10006,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Mug = 10007,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Bunshin = 10008,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Bhavacakra = 10009,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Kassatsu = 10010,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_TenChiJin = 10011,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_TenriJindo = 10054,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Assassinate = 10012,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Meisui = 10013,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_StunInterupt = 10045,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_PhantomKamaitachi = 10014,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Raiju = 10015,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_ThrowingDaggers = 10016,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_SecondWind = 10017,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_ShadeShift = 10018,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Bloodbath = 10019,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_ST_AdvancedMode_Feint = 10020,

    #endregion

    #region AoE Advanced
    [AutoAction(true, false)]
    [ReplaceSkill(NIN.DeathBlossom)]
    [ConflictingCombos(NIN_AoE_SimpleMode)]
    [JobInfo(Job.NIN)]
    [AdvancedCombo]
    NIN_AoE_AdvancedMode = 10003,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Ninjitsus = 10021,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Ninjitsus_Katon = 10047,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Ninjitsus_Huton = 10048,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Ninjitsus_Doton = 10049,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Ninjitsus_Goka = 10050,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_TrickAttack = 10022,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Mug = 10023,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Bunshin = 10024,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_HellfrogMedium = 10025,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Kassatsu = 10026,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_TenChiJin = 10027,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_TenriJindo = 10055,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Assassinate = 10028,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Meisui = 10029,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_StunInterupt = 10044,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_PhantomKamaitachi = 10030,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_ThrowingDaggers = 10031,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_SecondWind = 10032,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_ShadeShift = 10033,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [JobInfo(Job.NIN)]
    NIN_AoE_AdvancedMode_Bloodbath = 10034,

    #endregion

    #region Standalones

    #region Basic Combo

    [ReplaceSkill(NIN.AeolianEdge)]
    [JobInfo(Job.NIN)]
    [BasicCombo]
    NIN_ST_AeolianEdgeCombo = 10042,

    [ReplaceSkill(NIN.ArmorCrush)]
    [JobInfo(Job.NIN)]
    NIN_ArmorCrushCombo = 10041,

    #endregion
    [ReplaceSkill(NIN.ShadeShift, NIN.Shukuchi, RoleActions.Melee.Feint, RoleActions.Melee.Bloodbath, RoleActions.Physical.SecondWind)]
    [JobInfo(Job.NIN)]
    NIN_MudraProtection = 10046,


    [ReplaceSkill(NIN.Kassatsu)]
    [JobInfo(Job.NIN)]
    NIN_KassatsuTrick = 10035,

    [ReplaceSkill(NIN.TenChiJin)]
    [JobInfo(Job.NIN)]
    NIN_TCJMeisui = 10036,

    [ReplaceSkill(NIN.Chi)]
    [JobInfo(Job.NIN)]
    NIN_KassatsuChiJin = 10037,

    [ReplaceSkill(NIN.Hide)]
    [JobInfo(Job.NIN)]
    NIN_HideMug = 10038,

    [ReplaceSkill(NIN.Ten, NIN.Chi, NIN.Jin)]
    [ConflictingCombos(Preset.NIN_Simple_Mudras_Alt)]
    [JobInfo(Job.NIN)]
    NIN_Simple_Mudras = 10039,

    [ReplaceSkill(NIN.Ten, NIN.Chi, NIN.Jin)]
    [ConflictingCombos(Preset.NIN_Simple_Mudras)]
    [JobInfo(Job.NIN)]
    NIN_Simple_Mudras_Alt = 10043,

    [ReplaceSkill(NIN.TenChiJin)]
    [ParentCombo(NIN_TCJMeisui)]
    [JobInfo(Job.NIN)]
    NIN_TCJ = 10040,

    #endregion

    // Last value = 10053

    #endregion

    #region PICTOMANCER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(PCT.FireInRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    [SimpleCombo]
    PCT_ST_SimpleMode = 20000,

    [AutoAction(true, false)]
    [ReplaceSkill(PCT.FireIIinRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    [SimpleCombo]
    PCT_AoE_SimpleMode = 20001,

    #endregion

    #region ST

    [AutoAction(false, false)]
    [ReplaceSkill(PCT.FireInRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_ST_SimpleMode)]
    [JobInfo(Job.PCT)]
    [AdvancedCombo]
    PCT_ST_AdvancedMode = 20005,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_Advanced_Openers = 20006,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_SubtractivePalette = 20025,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_BlizzardInCyan = 20033,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_HolyinWhite = 20072,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_CometinBlack = 20026,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_LivingMuse = 20022,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_MogOfTheAges = 20024,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_SteelMuse = 20023,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_HammerStampCombo = 20027,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_ScenicMuse = 20021,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_StarPrism = 20012,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_RainbowDrip = 20013,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_LucidDreaming = 20034,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_MotifFeature = 20016,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_LandscapeMotif = 20017,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_CreatureMotif = 20018,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_WeaponMotif = 20019,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_PrePullMotifs = 20008,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_NoTargetMotifs = 20009,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_SwiftMotifs = 20035,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_MovementFeature = 20028,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_MovementOption_HammerStampCombo = 20029,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_MovementOption_HolyInWhite = 20030,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_MovementOption_CometinBlack = 20031,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_SwiftcastOption = 20032,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_Addle = 20070,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_ST_AdvancedMode_Tempera = 20071,

    #endregion

    #region AoE

    [AutoAction(true, false)]
    [ReplaceSkill(PCT.FireIIinRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_AoE_SimpleMode)]
    [JobInfo(Job.PCT)]
    [AdvancedCombo]
    PCT_AoE_AdvancedMode = 20040,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_SubtractivePalette = 20058,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_BlizzardInCyan = 20066,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_HolyinWhite = 20068,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_CometinBlack = 20059,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_LivingMuse = 20055,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_MogOfTheAges = 20057,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_SteelMuse = 20056,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_HammerStampCombo = 20060,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_ScenicMuse = 20054,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_StarPrism = 20045,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_RainbowDrip = 20046,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_LucidDreaming = 20067,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_MotifFeature = 20049,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_LandscapeMotif = 20050,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_CreatureMotif = 20051,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_WeaponMotif = 20052,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_PrePullMotifs = 20041,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_NoTargetMotifs = 20042,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_SwiftMotifs = 20069,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_MovementFeature = 20061,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo = 20062,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_MovementOption_HolyInWhite = 20063,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_MovementOption_CometinBlack = 20064,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [JobInfo(Job.PCT)]
    PCT_AoE_AdvancedMode_SwiftcastOption = 20065,

    #endregion

    #region Standalone Features

    [ReplaceSkill(PCT.FireInRed, PCT.FireIIinRed)]
    [ConflictingCombos(PCT_ST_SimpleMode, PCT_AoE_SimpleMode)]
    [JobInfo(Job.PCT)]
    CombinedAetherhues = 20002,

    [ReplaceSkill(PCT.CreatureMotif, PCT.WeaponMotif, PCT.LandscapeMotif)]
    [JobInfo(Job.PCT)]
    CombinedMotifs = 20003,

    [ReplaceSkill(PCT.HolyInWhite)]
    [JobInfo(Job.PCT)]
    CombinedPaint = 20004,

    #endregion
    // Last used: 20072

    #endregion

    #region PALADIN

    #region Simple Mode

    // Simple Modes
    [AutoAction(false, false)]
    [ConflictingCombos(PLD_ST_AdvancedMode)]
    [ReplaceSkill(PLD.FastBlade)]
    [JobInfo(Job.PLD)]
    [SimpleCombo]
    PLD_ST_SimpleMode = 11000,

    [AutoAction(true, false)]
    [ConflictingCombos(PLD_AoE_AdvancedMode)]
    [ReplaceSkill(PLD.TotalEclipse)]
    [JobInfo(Job.PLD)]
    [SimpleCombo]
    PLD_AoE_SimpleMode = 11001,

    #endregion

    #region ST Advanced Mode

    [AutoAction(false, false)]
    [ConflictingCombos(PLD_ST_SimpleMode)]
    [ReplaceSkill(PLD.FastBlade)]
    [JobInfo(Job.PLD)]
    [AdvancedCombo]
    PLD_ST_AdvancedMode = 11002,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_BalanceOpener = 11046,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_Interrupt = 11058,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_LowBlow = 11062,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_ShieldBash = 11066,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_FoF = 11003,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_ShieldLob = 11004,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_CircleOfScorn = 11005,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_SpiritsWithin = 11006,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_GoringBlade = 11008,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_HolySpirit = 11009,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_Requiescat = 11010,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_Intervene = 11011,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_Atonement = 11012,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_Confiteor = 11013,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_Blades = 11014,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_BladeOfHonor = 11033,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_ST_AdvancedMode_MP_Reserve = 11035,

    #endregion

    #region AoE Advanced Mode

    [AutoAction(true, false)]
    [ConflictingCombos(PLD_AoE_SimpleMode)]
    [ReplaceSkill(PLD.TotalEclipse)]
    [JobInfo(Job.PLD)]
    [AdvancedCombo]
    PLD_AoE_AdvancedMode = 11015,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_Interrupt = 11059,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_LowBlow = 11060,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_ShieldBash = 11065,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_FoF = 11016,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_SpiritsWithin = 11017,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_CircleOfScorn = 11018,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_Requiescat = 11019,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_Intervene = 11037,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_HolyCircle = 11020,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_Confiteor = 11021,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_Blades = 11022,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_BladeOfHonor = 11034,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [JobInfo(Job.PLD)]
    PLD_AoE_AdvancedMode_MP_Reserve = 11036,

    // AoE Mitigation Options
    #endregion

    #region Advanced Mitigation

    [JobInfo(Job.PLD)]
    PLD_Mitigation = 11086,

    [ParentCombo(PLD_Mitigation)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss = 11087,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_Rampart = 11088,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_Sentinel = 11089,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_Bulwark = 11090,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_HallowedGround = 11091,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_HallowedGroundEmergency = 11104,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_ArmsLength = 11092,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_Reprisal = 11099,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_Sheltron = 11093,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_NonBoss_DivineVeil = 11094,

    [ParentCombo(PLD_Mitigation)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss = 11095,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_SheltronOvercap = 11096,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_SheltronTankbuster = 11100,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_Rampart = 11102,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_Sentinel = 11101,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_Bulwark = 11103,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_Reprisal = 11097,

    [ParentCombo(PLD_Mitigation_Boss)]
    [JobInfo(Job.PLD)]
    PLD_Mitigation_Boss_DivineVeil = 11098,

    [ParentCombo(PLD_Mitigation)]
    [JobInfo(Job.PLD)]
    PLD_BlockForWings = 11074,
    #endregion

    #region Basic combo

    [ReplaceSkill(PLD.RageOfHalone, PLD.RoyalAuthority)]
    [JobInfo(Job.PLD)]
    [BasicCombo]
    PLD_ST_BasicCombo = 11061,

    [ReplaceSkill(PLD.RageOfHalone, PLD.Prominence)]
    [JobInfo(Job.PLD)]
    [BasicCombo]
    PLD_AoE_BasicCombo = 11078,

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(PLD.Bulwark)]
    [JobInfo(Job.PLD)]
    [MitigationCombo]
    PLD_Mit_OneButton = 11047,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_HallowedGround_Max = 11048,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Sheltron = 11049,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Reprisal = 11050,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_DivineVeil = 11051,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Rampart = 11052,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Bulwark = 11055,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_ArmsLength = 11054,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Sentinel = 11053,

    [ParentCombo(PLD_Mit_OneButton)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Clemency = 11057,

    [ReplaceSkill(PLD.DivineVeil)]
    [JobInfo(Job.PLD)]
    [MitigationCombo]
    PLD_Mit_Party = 11063,

    [ParentCombo(PLD_Mit_Party)]
    [JobInfo(Job.PLD)]
    PLD_Mit_Party_Wings = 11064,
    #endregion

    #region Extra Features

    [ReplaceSkill(PLD.Requiescat, PLD.Imperator)]
    [JobInfo(Job.PLD)]
    PLD_Requiescat_Options = 11024,

    [ReplaceSkill(PLD.SpiritsWithin, PLD.Expiacion)]
    [JobInfo(Job.PLD)]
    PLD_SpiritsWithin = 11025,

    [ReplaceSkill(PLD.ShieldLob)]
    [JobInfo(Job.PLD)]
    PLD_ShieldLob_Feature = 11027,

    [ReplaceSkill(PLD.Clemency)]
    [JobInfo(Job.PLD)]
    [Retargeted]
    PLD_RetargetClemency = 11067,

    [ParentCombo(PLD_RetargetClemency)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Clemency)]
    PLD_RetargetClemency_MO = 11071,

    [ParentCombo(PLD_RetargetClemency)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Clemency)]
    PLD_RetargetClemency_LowHP = 11072,

    [ReplaceSkill(PLD.Sheltron)]
    [JobInfo(Job.PLD)]
    [Retargeted]
    PLD_RetargetSheltron = 11068,

    [ParentCombo(PLD_RetargetSheltron)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Sheltron)]
    PLD_RetargetSheltron_MO = 11069,

    [ParentCombo(PLD_RetargetSheltron)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Sheltron)]
    PLD_RetargetSheltron_TT = 11070,

    [ConflictingCombos(ALL_Tank_Interrupt)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.ShieldBash)]
    PLD_RetargetShieldBash = 11073,

    [ReplaceSkill(PLD.Cover)]
    [JobInfo(Job.PLD)]
    [Retargeted]
    PLD_RetargetCover = 11075,

    [ParentCombo(PLD_RetargetCover)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Cover)]
    PLD_RetargetCover_MO = 11076,

    [ParentCombo(PLD_RetargetCover)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Cover)]
    PLD_RetargetCover_LowHP = 11077,

    [ReplaceSkill(PLD.Intervene)]
    [JobInfo(Job.PLD)]
    [Retargeted(PLD.Intervene)]
    PLD_RetargetIntervene = 11105,

    #endregion

    //// Last value = 11105

    #endregion

    #region REAPER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(RPR.Slice)]
    [ConflictingCombos(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    [SimpleCombo]
    RPR_ST_SimpleMode = 12000,

    [AutoAction(true, false)]
    [ReplaceSkill(RPR.SpinningScythe)]
    [ConflictingCombos(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    [SimpleCombo]
    RPR_AoE_SimpleMode = 12100,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(RPR.Slice)]
    [ConflictingCombos(RPR_ST_SimpleMode)]
    [JobInfo(Job.RPR)]
    [AdvancedCombo]
    RPR_ST_AdvancedMode = 12001,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Opener = 12002,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_ArcaneCircle = 12006,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_PlentifulHarvest = 12007,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_SoD = 12003,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_SoulSow = 12020,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_SoulSlice = 12004,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Bloodstalk = 12008,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Gluttony = 12009,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_GibbetGallows = 12016,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Enshroud = 12010,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Reaping = 12011,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Lemure = 12012,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Communio = 12014,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Sacrificium = 12013,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Perfectio = 12015,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_TrueNorthDynamic = 12098,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_StunInterupt = 12096,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_RangedFiller = 12017,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_RangedFillerHarvestMoon = 12024,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_ComboHeals = 12097,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_Feint = 12095,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_ST_ArcaneCrest = 12022,

    //last value = 12024

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(RPR.SpinningScythe)]
    [ConflictingCombos(RPR_AoE_SimpleMode)]
    [JobInfo(Job.RPR)]
    [AdvancedCombo]
    RPR_AoE_AdvancedMode = 12101,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_ArcaneCircle = 12105,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_PlentifulHarvest = 12106,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_WoD = 12102,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_SoulSow = 12117,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_SoulScythe = 12103,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_GrimSwathe = 12107,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Gluttony = 12108,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Guillotine = 12115,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Enshroud = 12109,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Reaping = 12110,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Lemure = 12111,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Communio = 12113,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Sacrificium = 12112,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_Perfectio = 12114,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_StunInterupt = 12118,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [JobInfo(Job.RPR)]
    RPR_AoE_ComboHeals = 12116,

    // Last value = 12118

    #endregion

    #region Basic combo

    [ReplaceSkill(RPR.InfernalSlice)]
    [JobInfo(Job.RPR)]
    [BasicCombo]
    RPR_ST_BasicCombo = 12021,

    [ParentCombo(RPR_ST_BasicCombo)]
    [JobInfo(Job.RPR)]
    RPR_ST_BasicCombo_SoD = 12023,

    #endregion

    #region Blood Stalk/Grim Swathe Combo Section

    [ReplaceSkill(RPR.BloodStalk, RPR.GrimSwathe)]
    [JobInfo(Job.RPR)]
    RPR_GluttonyBloodSwathe = 12200,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [JobInfo(Job.RPR)]
    RPR_GluttonyBloodSwathe_BloodSwatheCombo = 12201,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [JobInfo(Job.RPR)]
    RPR_GluttonyBloodSwathe_Enshroud = 12202,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [JobInfo(Job.RPR)]
    RPR_GluttonyBloodSwathe_OGCD = 12204,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [JobInfo(Job.RPR)]
    RPR_GluttonyBloodSwathe_Sacrificium = 12203,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [JobInfo(Job.RPR)]
    RPR_TrueNorthGluttony = 12310,

    // Last value = 12204

    #endregion

    #region Miscellaneous

    [ReplaceSkill(RPR.Slice, RPR.SpinningScythe, RPR.ShadowOfDeath, RPR.Harpe, RPR.BloodStalk)]
    [JobInfo(Job.RPR)]
    RPR_Soulsow = 12302,

    [ParentCombo(RPR_Soulsow)]
    [JobInfo(Job.RPR)]
    RPR_Soulsow_Combat = 12309,

    [ReplaceSkill(RPR.ArcaneCircle)]
    [JobInfo(Job.RPR)]
    RPR_ArcaneCirclePlentifulHarvest = 12300,

    [ReplaceSkill(RPR.HellsEgress, RPR.HellsIngress)]
    [JobInfo(Job.RPR)]
    RPR_Regress = 12301,

    [ReplaceSkill(RPR.Enshroud)]
    [ConflictingCombos(RPR_EnshroudCommunio)]
    [JobInfo(Job.RPR)]
    RPR_EnshroudProtection = 12304,

    [ParentCombo(RPR_EnshroudProtection)]
    [JobInfo(Job.RPR)]
    RPR_TrueNorthEnshroud = 12308,

    [ReplaceSkill(RPR.Enshroud)]
    [ConflictingCombos(RPR_EnshroudProtection)]
    [JobInfo(Job.RPR)]
    RPR_EnshroudCommunio = 12307,

    [ReplaceSkill(RPR.Gibbet, RPR.Gallows, RPR.Guillotine)]
    [JobInfo(Job.RPR)]
    RPR_CommunioOnGGG = 12305,

    [ParentCombo(RPR_CommunioOnGGG)]
    [JobInfo(Job.RPR)]
    RPR_LemureOnGGG = 12306,

    // Last value = 12312

    #endregion

    #endregion

    #region RED MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ConflictingCombos(RDM_ST_DPS)]
    [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Jolt3)]
    [JobInfo(Job.RDM)]
    [SimpleCombo]
    RDM_ST_SimpleMode = 13000,

    [AutoAction(true, false)]
    [ReplaceSkill(RDM.Scatter, RDM.Impact)]
    [ConflictingCombos(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    [SimpleCombo]
    RDM_AoE_SimpleMode = 13200,

    #endregion

    #region Single Target DPS

    [AutoAction(false, false)]
    [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Jolt3)]
    [ConflictingCombos(RDM_ST_SimpleMode)]
    [JobInfo(Job.RDM)]
    [AdvancedCombo]
    RDM_ST_DPS = 13001,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_Balance_Opener = 13002,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_ThunderAero = 13003,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_FireStone = 13004,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_HolyFlare = 13005,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_MeleeCombo = 13006,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [JobInfo(Job.RDM)]
    RDM_ST_MeleeCombo_IncludeRiposte = 13007,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [JobInfo(Job.RDM)]
    RDM_ST_MeleeCombo_IncludeReprise = 13027,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [JobInfo(Job.RDM)]
    RDM_ST_MeleeCombo_GapCloser = 13008,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [JobInfo(Job.RDM)]
    RDM_ST_MeleeCombo_MeleeCheck = 13009,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Embolden = 13010,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Manafication = 13011,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_ViceOfThorns = 13012,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Prefulgence = 13013,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Fleche = 13014,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_ContreSixte = 13015,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Engagement = 13016,

    [ParentCombo(RDM_ST_Engagement)]
    [JobInfo(Job.RDM)]
    RDM_ST_Engagement_Pooling = 13018,

    [ParentCombo(RDM_ST_Engagement)]
    [JobInfo(Job.RDM)]
    RDM_ST_Engagement_Saving = 13028,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Corpsacorps = 13017,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Acceleration = 13019,

    [ParentCombo(RDM_ST_Acceleration)]
    [JobInfo(Job.RDM)]
    RDM_ST_Acceleration_Movement = 13020,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Swiftcast = 13021,

    [ParentCombo(RDM_ST_Swiftcast)]
    [JobInfo(Job.RDM)]
    RDM_ST_SwiftcastMovement = 13022,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Lucid = 13023,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_Addle = 13024,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_MagickBarrier = 13025,

    [ParentCombo(RDM_ST_DPS)]
    [JobInfo(Job.RDM)]
    RDM_ST_VerCure = 13026,

    //Last Used 13028
    #endregion

    #region AoE DPS

    [AutoAction(true, false)]
    [ReplaceSkill(RDM.Scatter, RDM.Impact)]
    [ConflictingCombos(RDM_AoE_SimpleMode)]
    [JobInfo(Job.RDM)]
    [AdvancedCombo]
    RDM_AoE_DPS = 13201,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_ThunderAero = 13202,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_HolyFlare = 13203,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_MeleeCombo = 13204,

    [ParentCombo(RDM_AoE_MeleeCombo)]
    [JobInfo(Job.RDM)]
    RDM_AoE_MeleeCombo_Target = 13205,

    [ParentCombo(RDM_AoE_MeleeCombo)]
    [JobInfo(Job.RDM)]
    RDM_AoE_MeleeCombo_GapCloser = 13206,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Embolden = 13207,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Manafication = 13208,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_ViceOfThorns = 13209,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Prefulgence = 13210,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Fleche = 13211,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_ContreSixte = 13212,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Engagement = 13213,

    [ParentCombo(RDM_AoE_Engagement)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Engagement_Pooling = 13215,

    [ParentCombo(RDM_AoE_Engagement)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Engagement_Saving = 13223,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Corpsacorps = 13214,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Acceleration = 13216,

    [ParentCombo(RDM_AoE_Acceleration)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Acceleration_Movement = 13217,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Swiftcast = 13218,

    [ParentCombo(RDM_AoE_Swiftcast)]
    [JobInfo(Job.RDM)]
    RDM_AoE_SwiftcastMovement = 13219,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_Lucid = 13220,

    [ParentCombo(RDM_AoE_DPS)]
    [JobInfo(Job.RDM)]
    RDM_AoE_VerCure = 13222,

    //Last Used 13223

    #endregion

    #region Stand Alone Features

    [ReplaceSkill(RDM.Veraero, RDM.Veraero3)]
    [JobInfo(Job.RDM)]
    RDM_VerAero = 13400,

    [ReplaceSkill(RDM.Verthunder, RDM.Verthunder3)]
    [JobInfo(Job.RDM)]
    RDM_VerThunder = 13418,

    [ReplaceSkill(RDM.Veraero2)]
    [JobInfo(Job.RDM)]
    RDM_VerAero2 = 13432,

    [ReplaceSkill(RDM.Verthunder2)]
    [JobInfo(Job.RDM)]
    RDM_VerThunder2 = 13433,

    [ReplaceSkill(RDM.Riposte)]
    [JobInfo(Job.RDM)]
    RDM_Riposte = 13403,

    [ParentCombo(RDM_Riposte)]
    [JobInfo(Job.RDM)]
    RDM_Riposte_Weaves = 13434,

    [ParentCombo(RDM_Riposte)]
    [JobInfo(Job.RDM)]
    RDM_Riposte_GapCloser = 13424,

    [ParentCombo(RDM_Riposte)]
    [JobInfo(Job.RDM)]
    RDM_Riposte_Finisher = 13423,

    [ParentCombo(RDM_Riposte)]
    [JobInfo(Job.RDM)]
    RDM_Riposte_NoWaste = 13429,

    [ReplaceSkill(RDM.Moulinet)]
    [JobInfo(Job.RDM)]
    RDM_Moulinet = 13425,

    [ParentCombo(RDM_Moulinet)]
    [JobInfo(Job.RDM)]
    RDM_Moulinet_Weaves = 13431,

    [ParentCombo(RDM_Moulinet)]
    [JobInfo(Job.RDM)]
    RDM_Moulinet_GapCloser = 13426,

    [ParentCombo(RDM_Moulinet)]
    [JobInfo(Job.RDM)]
    RDM_Moulinet_Finisher = 13427,

    [ParentCombo(RDM_Moulinet)]
    [JobInfo(Job.RDM)]
    RDM_Moulinet_NoWaste = 13428,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [JobInfo(Job.RDM)]
    RDM_Raise = 13406,

    [ParentCombo(RDM_Raise)]
    [JobInfo(Job.RDM)]
    RDM_Raise_Vercure = 13407,

    [ParentCombo(RDM_Raise)]
    [JobInfo(Job.RDM)]
    [Retargeted(RDM.Verraise, RDM.Vercure)]
    RDM_Raise_Retarget = 13408,

    [ReplaceSkill(RDM.Displacement)]
    [JobInfo(Job.RDM)]
    RDM_CorpsDisplacement = 13409,

    [ReplaceSkill(RDM.Embolden)]
    [JobInfo(Job.RDM)]
    RDM_EmboldenProtection = 13412,

    [ParentCombo(RDM_EmboldenProtection)]
    [JobInfo(Job.RDM)]
    RDM_EmboldenManafication = 13410,

    [ParentCombo(RDM_MagickProtection)]
    [JobInfo(Job.RDM)]
    RDM_MagickBarrierAddle = 13411,

    [ReplaceSkill(RDM.MagickBarrier)]
    [JobInfo(Job.RDM)]
    RDM_MagickProtection = 13413,

    [ReplaceSkill(RDM.Fleche)]
    [JobInfo(Job.RDM)]
    RDM_OGCDs = 13420,

    //Last Used 13434
    #endregion

    #endregion

    #region SAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(SGE.Dosis, SGE.Dosis2, SGE.Dosis3)]
    [ConflictingCombos(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    [SimpleCombo]
    SGE_ST_Simple_DPS = 14084,

    [AutoAction(true, false)]
    [ReplaceSkill(SGE.Dyskrasia, SGE.Dyskrasia2)]
    [ConflictingCombos(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    [SimpleCombo]
    SGE_AoE_Simple_DPS = 14085,

    [AutoAction(false, true)]
    [ReplaceSkill(SGE.Diagnosis)]
    [ConflictingCombos(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SGE_Simple_ST_Heal = 14087,


    [AutoAction(true, true)]
    [ReplaceSkill(SGE.Prognosis)]
    [ConflictingCombos(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SGE_Simple_AoE_Heal = 14086,

    #endregion

    #region Single Target DPS Feature

    [AutoAction(false, false)]
    [ReplaceSkill(SGE.Dosis, SGE.Dosis2, SGE.Dosis3)]
    [ConflictingCombos(SGE_ST_Simple_DPS)]
    [JobInfo(Job.SGE)]
    [AdvancedCombo]
    SGE_ST_DPS = 14001,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Opener = 14055,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_EDosis = 14003,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Druochole)]
    SGE_ST_DPS_AddersgallProtect = 14054,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Rhizo = 14007,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Phlegma = 14005,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Psyche = 14008,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Movement = 14004,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Kardia)]
    SGE_ST_DPS_Kardia = 14006,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Soteria = 14056,

    [ParentCombo(SGE_ST_DPS)]
    [JobInfo(Job.SGE)]
    SGE_ST_DPS_Lucid = 14002,

    #endregion

    #region AoE DPS Feature

    [AutoAction(true, false)]
    [ReplaceSkill(SGE.Dyskrasia, SGE.Dyskrasia2)]
    [ConflictingCombos(SGE_AoE_Simple_DPS)]
    [JobInfo(Job.SGE)]
    [AdvancedCombo]
    SGE_AoE_DPS = 14009,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_EDyskrasia = 14052,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted]
    SGE_AoE_DPS_AddersgallProtect = 14053,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Rhizo = 14013,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Phlegma = 14010,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Psyche = 14051,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Toxikon = 14011,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Pneuma = 14059,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Soteria = 14057,

    [ParentCombo(SGE_AoE_DPS)]
    [JobInfo(Job.SGE)]
    SGE_AoE_DPS_Lucid = 14012,

    #endregion

    #region Single Target Heal

    [AutoAction(false, true)]
    [ReplaceSkill(SGE.Diagnosis)]
    [ConflictingCombos(SGE_Simple_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Diagnosis)]
    [HealingCombo]
    SGE_ST_Heal = 14014,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    SGE_ST_Heal_Lucid = 14063,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    SGE_ST_Heal_Rhizomata = 14023,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Kardia)]
    SGE_ST_Heal_Kardia = 14016,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    SGE_ST_Heal_Esuna = 14015,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Eukrasia)]
    SGE_ST_Heal_EDiagnosis = 14017,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Druochole)]
    SGE_ST_Heal_Druochole = 14025,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Taurochole)]
    SGE_ST_Heal_Taurochole = 14021,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Haima)]
    SGE_ST_Heal_Haima = 14022,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    SGE_ST_Heal_Soteria = 14018,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    SGE_ST_Heal_Zoe = 14019,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Krasis)]
    SGE_ST_Heal_Krasis = 14024,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    SGE_ST_Heal_Pepsis = 14020,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Physis)]
    SGE_ST_Heal_Physis = 14065,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Kerachole)]
    SGE_ST_Heal_Kerachole = 14066,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Holos)]
    SGE_ST_Heal_Holos = 14067,

    [ParentCombo(SGE_ST_Heal)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted(SGE.Panhaima)]
    SGE_ST_Heal_Panhaima = 14068,

    #endregion

    #region AoE Heal

    [AutoAction(true, true)]
    [ReplaceSkill(SGE.Prognosis)]
    [ConflictingCombos(SGE_Simple_AoE_Heal)]
    [JobInfo(Job.SGE)]
    [HealingCombo]
    SGE_AoE_Heal = 14026,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Lucid = 14064,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Rhizomata = 14036,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_EPrognosis = 14028,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Ixochole = 14033,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Kerachole = 14035,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Holos = 14030,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Physis = 14027,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Panhaima = 14031,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Zoe = 14058,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Philosophia = 14050,

    [ParentCombo(SGE_AoE_Heal)]
    [JobInfo(Job.SGE)]
    SGE_AoE_Heal_Pepsis = 14032,

    #endregion

    #region Overprotect

    [ReplaceSkill(SGE.Kerachole)]
    [JobInfo(Job.SGE)]
    SGE_OverProtect = 14043,

    [ParentCombo(SGE_OverProtect)]
    [JobInfo(Job.SGE)]
    SGE_OverProtect_Kerachole = 14044,

    [ParentCombo(SGE_OverProtect_Kerachole)]
    [JobInfo(Job.SGE)]
    SGE_OverProtect_SacredSoil = 14045,

    [ParentCombo(SGE_OverProtect)]
    [JobInfo(Job.SGE)]
    SGE_OverProtect_Panhaima = 14046,

    [ParentCombo(SGE_OverProtect)]
    [JobInfo(Job.SGE)]
    SGE_OverProtect_Philosophia = 14047,

    #endregion

    #region Misc Healing

    [ReplaceSkill(SGE.Taurochole, SGE.Druochole, SGE.Ixochole, SGE.Kerachole)]
    [JobInfo(Job.SGE)]
    SGE_Rhizo = 14037,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [JobInfo(Job.SGE)]
    SGE_Raise = 14040,

    [ParentCombo(SGE_Raise)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Egeiro)]
    SGE_Raise_Retarget = 14061,

    [ReplaceSkill(SGE.Pneuma)]
    [JobInfo(Job.SGE)]
    SGE_ZoePneuma = 14039,

    [ReplaceSkill(SGE.Soteria)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted("Retargeting Features below, Enable Kardia", Condition.SGERetargetingFeaturesEnabledForKardia)]
    SGE_Kardia = 14041,

    [ReplaceSkill(SGE.Eukrasia)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted("Retargeting Features below, Enable Eukrasion Diagnosis", Condition.SGERetargetingFeaturesEnabledForEDiagnosis)]
    SGE_Eukrasia = 14042,

    [ReplaceSkill(SGE.Taurochole)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted("Retargeting Features below, Enable Druochole and Taurochole", Condition.SGERetargetingFeaturesEnabledForTauroDruo)]
    SGE_TauroDruo = 14038,

    [ReplaceSkill(SGE.Krasis)]
    [JobInfo(Job.SGE)]
    [PossiblyRetargeted("Retargeting Features below, Enable Krasis, Haima, Eukrasian Diagnosis, and Taurochole", Condition.SGERetargetingFeaturesEnabledForSTMit)]
    SGE_Mit_ST = 14081,

    [ReplaceSkill(SGE.Holos)]
    [JobInfo(Job.SGE)]
    SGE_Mit_AoE = 14082,

    #region Standalone Healing option

    [JobInfo(Job.SGE)]
    [Retargeted]
    SGE_Retarget = 14073,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Diagnosis)]
    SGE_Retarget_Diagnosis = 14079,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.EukrasianDiagnosis)]
    SGE_Retarget_EukrasianDiagnosis = 14080,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Haima)]
    SGE_Retarget_Haima = 14074,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Druochole)]
    SGE_Retarget_Druochole = 14075,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Taurochole)]
    SGE_Retarget_Taurochole = 14076,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Krasis)]
    SGE_Retarget_Krasis = 14077,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Kardia)]
    SGE_Retarget_Kardia = 14078,

    [ParentCombo(SGE_Retarget)]
    [JobInfo(Job.SGE)]
    [Retargeted(SGE.Icarus)]
    SGE_Retarget_Icarus = 14083,

    #endregion

    #region Raidwide Features
    [JobInfo(Job.SGE)]
    SGE_Raidwide = 14069,

    [ParentCombo(SGE_Raidwide)]
    [JobInfo(Job.SGE)]
    SGE_Raidwide_EPrognosis = 14070,

    [ParentCombo(SGE_Raidwide)]
    [JobInfo(Job.SGE)]
    SGE_Raidwide_Kerachole = 14071,

    [ParentCombo(SGE_Raidwide)]
    [JobInfo(Job.SGE)]
    SGE_Raidwide_Holos = 14072,
    #endregion

    #endregion

    // Last used number = 14087

    #endregion

    #region SAMURAI

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(SAM.Hakaze, SAM.Gyofu)]
    [ConflictingCombos(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    [SimpleCombo]
    SAM_ST_SimpleMode = 15002,

    [AutoAction(true, false)]
    [ReplaceSkill(SAM.Fuga, SAM.Fuko)]
    [ConflictingCombos(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    [SimpleCombo]
    SAM_AoE_SimpleMode = 15102,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(SAM.Hakaze, SAM.Gyofu)]
    [ConflictingCombos(SAM_ST_SimpleMode)]
    [JobInfo(Job.SAM)]
    [AdvancedCombo]
    SAM_ST_AdvancedMode = 15003,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Opener = 15006,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Yukikaze = 15004,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Kasha = 15005,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Gekko = 15022,

    #region cooldowns on Main Combo

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs = 15011,

    [ParentCombo(SAM_ST_CDs)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_MeikyoShisui = 15018,

    [ParentCombo(SAM_ST_CDs)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_Ikishoten = 15012,

    #endregion

    #region Damage skills

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Damage = 15023,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_Iaijutsu = 15013,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_UseHiganbana = 15024,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_UseTenkaGoken = 15025,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_UseMidare = 15026,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_UseTsubame = 15027,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_Iaijutsu_Movement = 15014,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_Senei = 15020,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_OgiNamikiri = 15015,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_Zanshin = 15017,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_CDs_Shoha = 15019,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_Shinten = 15008,

    [ParentCombo(SAM_ST_Damage)]
    [JobInfo(Job.SAM)]
    SAM_ST_RangedUptime = 15097,

    #endregion

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_TrueNorth = 15099,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_StunInterupt = 15096,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_ComboHeals = 15098,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Feint = 15095,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_ThirdEye = 15094,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_ST_Meditate = 15093,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(SAM.Fuga, SAM.Fuko)]
    [ConflictingCombos(SAM_AoE_SimpleMode)]
    [JobInfo(Job.SAM)]
    [AdvancedCombo]
    SAM_AoE_AdvancedMode = 15103,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Oka = 15104,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Hagakure = 15113,

    #region Cooldowns on Main Combo

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_CDs = 15115,

    [ParentCombo(SAM_AoE_CDs)]
    [JobInfo(Job.SAM)]
    SAM_AoE_MeikyoShisui = 15114,

    [ParentCombo(SAM_AoE_CDs)]
    [JobInfo(Job.SAM)]
    SAM_AOE_CDs_Ikishoten = 15108,

    #endregion

    #region Damage Skills

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Damage = 15116,

    [ParentCombo(SAM_AoE_Damage)]
    [JobInfo(Job.SAM)]
    SAM_AoE_TenkaGoken = 15107,

    [ParentCombo(SAM_AoE_Damage)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Guren = 15112,

    [ParentCombo(SAM_AoE_Damage)]
    [JobInfo(Job.SAM)]
    SAM_AoE_OgiNamikiri = 15109,

    [ParentCombo(SAM_AoE_Damage)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Zanshin = 15110,

    [ParentCombo(SAM_AoE_Damage)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Shoha = 15111,

    #endregion

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_Kyuten = 15105,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_StunInterupt = 15196,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [JobInfo(Job.SAM)]
    SAM_AoE_ComboHeals = 15199,

    #endregion

    #region Basic Combo

    [ReplaceSkill(SAM.Yukikaze)]
    [JobInfo(Job.SAM)]
    SAM_ST_YukikazeCombo = 15000,

    [ReplaceSkill(SAM.Kasha)]
    [JobInfo(Job.SAM)]
    SAM_ST_KashaCombo = 15001,

    [ReplaceSkill(SAM.Gekko)]
    [JobInfo(Job.SAM)]
    SAM_ST_GekkoCombo = 15010,

    [ReplaceSkill(SAM.Oka)]
    [JobInfo(Job.SAM)]
    SAM_AoE_OkaCombo = 15100,

    [ReplaceSkill(SAM.Mangetsu)]
    [JobInfo(Job.SAM)]
    SAM_AoE_MangetsuCombo = 15101,

    #endregion

    #region Meikyo Features

    [ReplaceSkill(SAM.MeikyoShisui)]
    [ConflictingCombos(SAM_MeikyoShisuiProtection)]
    [JobInfo(Job.SAM)]
    SAM_MeikyoSens = 15200,

    [ReplaceSkill(SAM.MeikyoShisui)]
    [ConflictingCombos(SAM_MeikyoSens)]
    [JobInfo(Job.SAM)]
    SAM_MeikyoShisuiProtection = 15214,

    #endregion

    #region Iaijutsu Features

    [ReplaceSkill(SAM.Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_Iaijutsu = 15201,

    [ParentCombo(SAM_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_Iaijutsu_TsubameGaeshi = 15202,

    [ParentCombo(SAM_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_Iaijutsu_Shoha = 15203,

    [ParentCombo(SAM_Iaijutsu)]
    [JobInfo(Job.SAM)]
    SAM_Iaijutsu_OgiNamikiri = 15204,

    #endregion

    #region Shinten Features

    [ReplaceSkill(SAM.Shinten)]
    [JobInfo(Job.SAM)]
    SAM_Shinten = 15251,

    [ParentCombo(SAM_Shinten)]
    [JobInfo(Job.SAM)]
    SAM_Shinten_Shoha = 15205,

    [ParentCombo(SAM_Shinten)]
    [JobInfo(Job.SAM)]
    SAM_Shinten_Senei = 15206,

    [ParentCombo(SAM_Shinten)]
    [JobInfo(Job.SAM)]
    SAM_Shinten_Zanshin = 15207,

    [ParentCombo(SAM_Shinten)]
    [JobInfo(Job.SAM)]
    SAM_Shinten_Ikishoten = 15256,

    #endregion

    #region Kyuten Features

    [ReplaceSkill(SAM.Kyuten)]
    [JobInfo(Job.SAM)]
    SAM_Kyuten = 15252,

    [ParentCombo(SAM_Kyuten)]
    [JobInfo(Job.SAM)]
    SAM_Kyuten_Shoha = 15208,

    [ParentCombo(SAM_Kyuten)]
    [JobInfo(Job.SAM)]
    SAM_Kyuten_Guren = 15209,

    [ParentCombo(SAM_Kyuten)]
    [JobInfo(Job.SAM)]
    SAM_Kyuten_Zanshin = 15210,

    [ParentCombo(SAM_Kyuten)]
    [JobInfo(Job.SAM)]
    SAM_Kyuten_Ikishoten = 15257,

    #endregion

    #region Ikishoten Features

    [ReplaceSkill(SAM.Ikishoten)]
    [JobInfo(Job.SAM)]
    SAM_Ikishoten = 15253,

    [ParentCombo(SAM_Ikishoten)]
    [JobInfo(Job.SAM)]
    SAM_Ikishoten_Namikiri = 15212,

    [ParentCombo(SAM_Ikishoten)]
    [JobInfo(Job.SAM)]
    SAM_Ikishoten_Shoha = 15213,

    #endregion

    #region Other

    [ReplaceSkill(SAM.Gyoten)]
    [JobInfo(Job.SAM)]
    SAM_GyotenYaten = 15211,

    [ReplaceSkill(SAM.Senei)]
    [JobInfo(Job.SAM)]
    SAM_SeneiGuren = 15215,

    [ReplaceSkill(SAM.OgiNamikiri)]
    [JobInfo(Job.SAM)]
    SAM_OgiShoha = 15258,

    #endregion

    #region Hidden Features

    [JobInfo(Job.SAM)]
    [Hidden]
    SAM_Hidden = 15300,


    #endregion

    // Last Value ST = 15027
    // Last Value AoE = 15113
    // Last Value Misc = 15258
    // Last Value Hidden = 153010
    #endregion

    #region SCHOLAR

    #region Simples

    [AutoAction(false, false)]
    [ReplaceSkill(SCH.Ruin, SCH.Broil, SCH.Broil2, SCH.Broil3, SCH.Broil4)]
    [SimpleCombo]
    [ConflictingCombos(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_Simple_DPS = 16070,


    [AutoAction(true, false)]
    [ReplaceSkill(SCH.ArtOfWar, SCH.ArtOfWarII)]
    [SimpleCombo]
    [ConflictingCombos(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Simple_DPS = 16071,

    [AutoAction(false, true)]
    [ReplaceSkill(SCH.Physick)]
    [ConflictingCombos(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SCH_Simple_ST_Heal = 16085,


    [AutoAction(true, true)]
    [ReplaceSkill(SCH.Succor)]
    [ConflictingCombos(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SCH_Simple_AoE_Heal = 16084,

    #endregion

    #region ST DPS
    [AutoAction(false, false)]
    [ReplaceSkill(SCH.Ruin, SCH.Broil, SCH.Broil2, SCH.Broil3, SCH.Broil4, SCH.Bio, SCH.Bio2, SCH.Biolysis)]
    [JobInfo(Job.SCH)]
    [AdvancedCombo]
    [ConflictingCombos(SCH_ST_Simple_DPS)]
    SCH_ST_ADV_DPS = 16001,


    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_Balance_Opener = 16009,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_Bio = 16008,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_Aetherflow = 16004,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_EnergyDrain = 16005,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_ChainStrat = 16003,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_BanefulImpact = 16052,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_Ruin2Movement = 16007,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_FairyReminder = 16048,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_ST_ADV_DPS_Lucid = 16002,

    #endregion

    #region AoE DPS
    [AutoAction(true, false)]
    [ReplaceSkill(SCH.ArtOfWar, SCH.ArtOfWarII)]
    [ConflictingCombos(SCH_AoE_Simple_DPS)]
    [JobInfo(Job.SCH)]
    [AdvancedCombo]
    SCH_AoE_ADV_DPS = 16010,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Bio, SCH.Bio2, SCH.Biolysis)]
    SCH_AoE_ADV_DPS_DoT = 16072,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_ADV_DPS_EnergyDrain = 16056,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_ADV_DPS_ChainStrat = 16054,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_ADV_DPS_BanefulImpact = 16053,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_ADV_DPS_FairyReminder = 16049,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_ADV_DPS_Lucid = 16011,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [JobInfo(Job.SCH)]
    SCH_AoE_ADV_DPS_Aetherflow = 16012,

    #endregion

    #region  ST Healing
    [AutoAction(false, true)]
    [ReplaceSkill(SCH.Physick)]
    [ConflictingCombos(SCH_Simple_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(SCH.Physick)]
    [HealingCombo]
    SCH_ST_Heal = 16023,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_Lucid = 16024,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_Aetherflow = 16025,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_Dissipation = 16040,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    SCH_ST_Heal_Esuna = 16026,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(SCH.Adloquium)]
    SCH_ST_Heal_Adloquium = 16027,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(SCH.Lustrate)]
    SCH_ST_Heal_Lustrate = 16028,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(SCH.Excogitation)]
    SCH_ST_Heal_Excogitation = 16038,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(SCH.Protraction)]
    SCH_ST_Heal_Protraction = 16039,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted(SCH.Aetherpact)]
    SCH_ST_Heal_Aetherpact = 16047,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_WhisperingDawn = 16067,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_FeyIllumination = 16068,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_FeyBlessing = 16069,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_Seraphism = 16086,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_Expedient = 16087,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_SummonSeraph = 16088,

    [ParentCombo(SCH_ST_Heal)]
    [JobInfo(Job.SCH)]
    SCH_ST_Heal_Consolation = 16089,
    #endregion

    #region AoE Healing
    [AutoAction(true, true)]
    [ReplaceSkill(SCH.Succor)]
    [ConflictingCombos(SCH_Simple_AoE_Heal)]
    [JobInfo(Job.SCH)]
    [HealingCombo]
    SCH_AoE_Heal = 16018,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_Indomitability = 16022,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_WhisperingDawn = 16043,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_FeyIllumination = 16042,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_FeyBlessing = 16045,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_Seraphism = 16044,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_SummonSeraph = 16063,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_Consolation = 16046,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_Aetherflow = 16020,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_Dissipation = 16041,

    [ParentCombo(SCH_AoE_Heal)]
    [JobInfo(Job.SCH)]
    SCH_AoE_Heal_Lucid = 16019,

    #endregion

    #region Utilities

    [ReplaceSkill(SCH.EnergyDrain, SCH.Lustrate, SCH.SacredSoil, SCH.Indomitability, SCH.Excogitation)]
    [JobInfo(Job.SCH)]
    SCH_Aetherflow = 16029,

    [ParentCombo(SCH_Aetherflow)]
    [JobInfo(Job.SCH)]
    SCH_Aetherflow_Dissipation = 16031,

    [ParentCombo(SCH_Aetherflow)]
    [JobInfo(Job.SCH)]
    SCH_Aetherflow_Recite = 16030,

    [ReplaceSkill(SCH.Lustrate)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted("Retargeting Features below, Enable Lustrate and Excogitation", Condition.SCHRetargetingFeaturesEnabledForLustcog)]
    SCH_Lustrate = 16014,

    [ReplaceSkill(SCH.Recitation)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted("Retargeting Features below, Enable Adloquium and Excogitation", Condition.SCHRetargetingFeaturesEnabledForAdlocog)]
    SCH_Recitation = 16015,

    [ReplaceSkill(SCH.DeploymentTactics)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted("Retargeting Features below, Enable Adloquium and Deployment Tactics", Condition.SCHRetargetingFeaturesEnabledForAdloDeployment)]
    SCH_DeploymentTactics = 16034,

    [ParentCombo(SCH_DeploymentTactics)]
    [JobInfo(Job.SCH)]
    SCH_DeploymentTactics_Recitation = 16035,

    [ReplaceSkill(SCH.WhisperingDawn, SCH.FeyIllumination, SCH.FeyBlessing, SCH.Aetherpact, SCH.Dissipation,
        SCH.SummonSeraph)]
    [JobInfo(Job.SCH)]
    SCH_FairyReminder = 16033,

    [ReplaceSkill(SCH.FeyBlessing)]
    [JobInfo(Job.SCH)]
    SCH_Consolation = 16013,

    [ReplaceSkill(SCH.WhisperingDawn)]
    [JobInfo(Job.SCH)]
    SCH_Fairy_Combo = 16016,

    [ParentCombo(SCH_Fairy_Combo)]
    [JobInfo(Job.SCH)]
    SCH_Fairy_Combo_Consolation = 16017,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [JobInfo(Job.SCH)]
    SCH_Raise = 16032,

    [ParentCombo(SCH_Raise)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Resurrection)]
    SCH_Raise_Retarget = 16050,

    #endregion

    #region Mitigation Features

    [ReplaceSkill(SCH.Protraction)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted("Retargeting Features below, Enable Protraction and Adloquium (and optionally Deployment Tactics and Excogitation)", Condition.SCHRetargetingFeaturesEnabledForSTMit)]
    SCH_Mit_ST = 16083,

    [ReplaceSkill(SCH.SacredSoil)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted("Retargeting Features below, Enable Sacred Soil", Condition.SCHRetargetingFeaturesEnabledForAoEMit)]
    SCH_Mit_AoE = 16082,

    #endregion

    #region Standalone Healing option

    [JobInfo(Job.SCH)]
    [Retargeted]
    SCH_Retarget = 16073,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Physick)]
    SCH_Retarget_Physick = 16074,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Adloquium)]
    SCH_Retarget_Adloquium = 16081,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Lustrate)]
    SCH_Retarget_Lustrate = 16075,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Protraction)]
    SCH_Retarget_Protraction = 16076,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.DeploymentTactics)]
    SCH_Retarget_DeploymentTactics = 16077,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Excogitation)]
    SCH_Retarget_Excogitation = 16078,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.Aetherpact)]
    SCH_Retarget_Aetherpact = 16079,

    [ParentCombo(SCH_Retarget)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.SacredSoil)]
    SCH_Retarget_SacredSoil = 16080,

    #endregion

    #region Raidwide Features
    [JobInfo(Job.SCH)]
    SCH_Raidwide = 16065,

    [ParentCombo(SCH_Raidwide)]
    [JobInfo(Job.SCH)]
    SCH_Raidwide_Succor = 16062,

    [ParentCombo(SCH_Raidwide)]
    [JobInfo(Job.SCH)]
    [Retargeted(SCH.SacredSoil)]
    SCH_Raidwide_SacredSoil = 16059,

    [ParentCombo(SCH_Raidwide)]
    [JobInfo(Job.SCH)]
    SCH_Raidwide_Expedient = 16064,
    #endregion

    // Last value = 16089

    #endregion

    #region SUMMONER

    #region Simple Modes

    [AutoAction(false, false)]
    [ConflictingCombos(SMN_ST_Advanced_Combo)]
    [ReplaceSkill(SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
    [JobInfo(Job.SMN)]
    [SimpleCombo]
    SMN_ST_Simple_Combo = 17041,

    [AutoAction(true, false)]
    [ConflictingCombos(SMN_AoE_Advanced_Combo)]
    [ReplaceSkill(SMN.Outburst)]
    [JobInfo(Job.SMN)]
    [SimpleCombo]
    SMN_AoE_Simple_Combo = 17066,

    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ReplaceSkill(SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
    [ConflictingCombos(SMN_ST_Simple_Combo)]
    [JobInfo(Job.SMN)]
    [AdvancedCombo]
    SMN_ST_Advanced_Combo = 17000,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Balance_Opener = 170001,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_DemiSummons = 17020,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_DemiSummons_Attacks = 17002,

    [ParentCombo(SMN_ST_Advanced_Combo_DemiSummons_Attacks)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_DemiSummons_Rekindle = 17028,

    [ParentCombo(SMN_ST_Advanced_Combo_DemiSummons_Rekindle)]
    [JobInfo(Job.SMN)]
    [Retargeted(SMN.Rekindle)]
    SMN_ST_Advanced_Combo_DemiSummons_Rekindle_Retarget = 17080,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Titan = 17073,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Garuda = 17074,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Ifrit = 17075,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_EgiSummons_Attacks = 17004,

    [ParentCombo(SMN_ST_Advanced_Combo_EgiSummons_Attacks)]
    [JobInfo(Job.SMN)]
    SMN_ST_Ruin3_Emerald_Ruin3 = 17067,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Egi_AstralFlow = 17048,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi = 17023,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_EDFester = 17014,

    [ParentCombo(SMN_ST_Advanced_Combo_EDFester)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_oGCDPooling = 17025,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_SearingLight = 17017,

    [ParentCombo(SMN_ST_Advanced_Combo_SearingLight)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_SearingLight_Burst = 17018,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_SearingFlash = 17019,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Ruin4 = 17011,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris = 17029,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Radiant = 17071,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Lucid = 17031,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_ST_Advanced_Combo_Addle = 17082,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(SMN.Outburst, SMN.Tridisaster)]
    [ConflictingCombos(SMN_AoE_Simple_Combo)]
    [JobInfo(Job.SMN)]
    [AdvancedCombo]
    SMN_AoE_Advanced_Combo = 17049,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_DemiSummons = 17061,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_DemiSummons_Attacks = 17055,

    [ParentCombo(SMN_AoE_Advanced_Combo_DemiSummons_Attacks)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_DemiSummons_Rekindle = 17056,

    [ParentCombo(SMN_AoE_Advanced_Combo_DemiSummons_Rekindle)]
    [JobInfo(Job.SMN)]
    [Retargeted(SMN.Rekindle)]
    SMN_AoE_Advanced_Combo_DemiSummons_Rekindle_Retarget = 17081,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Titan = 17076,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Garuda = 17077,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Ifrit = 17078,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_EgiSummons_Attacks = 17064,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Egi_AstralFlow = 17068,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi = 17063,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_ESPainflare = 17051,

    [ParentCombo(SMN_AoE_Advanced_Combo_ESPainflare)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_oGCDPooling = 17050,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_SearingLight = 17053,

    [ParentCombo(SMN_AoE_Advanced_Combo_SearingLight)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_SearingLight_Burst = 17054,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_SearingFlash = 17058,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Ruin4 = 17062,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris = 17059,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Radiant = 17070,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [JobInfo(Job.SMN)]
    SMN_AoE_Advanced_Combo_Lucid = 17060,
    #endregion

    #region Standalone Features
    [ReplaceSkill(SMN.Fester)]
    [JobInfo(Job.SMN)]
    SMN_EDFester = 17008,

    [ParentCombo(SMN_EDFester)]
    [JobInfo(Job.SMN)]
    SMN_EDFester_Ruin4 = 17013,

    [ReplaceSkill(SMN.Painflare)]
    [JobInfo(Job.SMN)]
    SMN_ESPainflare = 17009,

    [JobInfo(Job.SMN)]
    SMN_CarbuncleReminder = 17010,

    [JobInfo(Job.SMN)]
    SMN_DemiAbilities = 17024,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [JobInfo(Job.SMN)]
    SMN_Raise = 17027,

    [ParentCombo(SMN_Raise)]
    [JobInfo(Job.SMN)]
    [Retargeted(SMN.Resurrection)]
    SMN_Raise_Retarget = 17079,

    [ReplaceSkill(SMN.Ruin4)]
    [JobInfo(Job.SMN)]
    SMN_RuinMobility = 17030,

    [JobInfo(Job.SMN)]
    SMN_Egi_AstralFlow = 17034,

    [ParentCombo(SMN_ESPainflare)]
    [JobInfo(Job.SMN)]
    SMN_ESPainflare_Ruin4 = 17039,

    [JobInfo(Job.SMN)]
    SMN_Searing = 17072,
    #endregion

    // Last Used 17080

    #endregion

    #region VIPER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(VPR.SteelFangs)]
    [ConflictingCombos(VPR_ST_AdvancedMode, VPR_SerpentsTail, VPR_Legacies)]
    [JobInfo(Job.VPR)]
    [SimpleCombo]
    VPR_ST_SimpleMode = 30000,

    [AutoAction(true, false)]
    [ReplaceSkill(VPR.SteelMaw)]
    [ConflictingCombos(VPR_AoE_AdvancedMode, VPR_SerpentsTail)]
    [JobInfo(Job.VPR)]
    [SimpleCombo]
    VPR_AoE_SimpleMode = 30100,

    #endregion

    #region Advanced ST Viper

    [AutoAction(false, false)]
    [ReplaceSkill(VPR.SteelFangs)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_SerpentsTail, VPR_Legacies)]
    [JobInfo(Job.VPR)]
    [AdvancedCombo]
    VPR_ST_AdvancedMode = 30001,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_Opener = 30002,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_SerpentsIre = 30005,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_Vicewinder = 30006,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_VicewinderCombo = 30007,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_VicewinderWeaves = 30013,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_SerpentsTail = 30008,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_UncoiledFury = 30009,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_UncoiledFuryCombo = 30010,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_Reawaken = 30011,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_GenerationCombo = 30012,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_LegacyWeaves = 30014,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_TrueNorthDynamic = 30098,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_StunInterupt = 30096,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_RangedUptime = 30095,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_ComboHeals = 30097,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_ST_Feint = 30094,

    #endregion

    #region Advanced AoE Viper

    [AutoAction(true, false)]
    [ReplaceSkill(VPR.SteelMaw)]
    [ConflictingCombos(VPR_AoE_SimpleMode, VPR_SerpentsTail)]
    [JobInfo(Job.VPR)]
    [AdvancedCombo]
    VPR_AoE_AdvancedMode = 30101,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_SerpentsIre = 30104,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_Vicepit = 30105,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_VicepitCombo = 30106,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_VicepitWeaves = 30115,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_SerpentsTail = 30107,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_UncoiledFury = 30108,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_UncoiledFuryCombo = 30109,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_Reawaken = 30110,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_ReawakenCombo = 30112,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_StunInterupt = 30196,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [JobInfo(Job.VPR)]
    VPR_AoE_ComboHeals = 30199,

    #endregion

    #region Basic combo

    [ReplaceSkill(VPR.ReavingFangs)]
    [ConflictingCombos(VPR_ReawakenLegacy, VPR_Legacies, VPR_SerpentsTail)]
    [JobInfo(Job.VPR)]
    [BasicCombo]
    VPR_ST_BasicCombo = 30015,

    #endregion

    #region Movement

    [JobInfo(Job.VPR)]
    [Retargeted(VPR.Slither)]
    VPR_Retarget_Slither = 30211,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(VPR.Vicewinder)]
    [ConflictingCombos(VPR_VicewinderProtection)]
    [JobInfo(Job.VPR)]
    VPR_VicewinderCoils = 30200,

    [ParentCombo(VPR_VicewinderCoils)]
    [JobInfo(Job.VPR)]
    VPR_VicewinderCoils_oGCDs = 30206,

    [ReplaceSkill(VPR.Vicepit)]
    [ConflictingCombos(VPR_VicewinderProtection)]
    [JobInfo(Job.VPR)]
    VPR_VicepitDens = 30201,

    [ParentCombo(VPR_VicepitDens)]
    [JobInfo(Job.VPR)]
    VPR_VicepitDens_oGCDs = 30207,

    [ReplaceSkill(VPR.UncoiledFury)]
    [JobInfo(Job.VPR)]
    VPR_UncoiledTwins = 30202,

    [ReplaceSkill(VPR.Reawaken, VPR.ReavingFangs)]
    [ConflictingCombos(VPR_Legacies, VPR_ST_BasicCombo)]
    [JobInfo(Job.VPR)]
    VPR_ReawakenLegacy = 30203,

    [ParentCombo(VPR_ReawakenLegacy)]
    [JobInfo(Job.VPR)]
    VPR_ReawakenLegacyWeaves = 30204,

    [ReplaceSkill(VPR.SerpentsTail)]
    [JobInfo(Job.VPR)]
    VPR_TwinTails = 30205,

    [ReplaceSkill(VPR.SteelFangs, VPR.ReavingFangs, VPR.HuntersCoil, VPR.SwiftskinsCoil)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_ST_AdvancedMode, VPR_SerpentsTail, VPR_ReawakenLegacy, VPR_ST_BasicCombo)]
    [JobInfo(Job.VPR)]
    VPR_Legacies = 30209,

    [ReplaceSkill(VPR.SteelFangs, VPR.ReavingFangs, VPR.SteelMaw, VPR.ReavingMaw)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_AoE_SimpleMode, VPR_ST_AdvancedMode, VPR_AoE_AdvancedMode, VPR_Legacies, VPR_ST_BasicCombo)]
    [JobInfo(Job.VPR)]
    VPR_SerpentsTail = 30210,

    [ReplaceSkill(VPR.Vicewinder, VPR.Vicepit)]
    [ConflictingCombos(VPR_VicepitDens, VPR_VicewinderCoils)]
    [JobInfo(Job.VPR)]
    VPR_VicewinderProtection = 30212,

    #endregion

    //ST 30016
    //AoE 30115
    //Misc 30212

    #endregion

    #region WARRIOR

    #region Simple Mode
    [AutoAction(false, false)]
    [ConflictingCombos(WAR_ST_Advanced)]
    [ReplaceSkill(WAR.HeavySwing)]
    [JobInfo(Job.WAR)]
    [SimpleCombo]
    WAR_ST_Simple = 18000,

    [AutoAction(true, false)]
    [ConflictingCombos(WAR_AoE_Advanced)]
    [ReplaceSkill(WAR.Overpower)]
    [JobInfo(Job.WAR)]
    [SimpleCombo]
    WAR_AoE_Simple = 18001,
    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ConflictingCombos(WAR_ST_Simple)]
    [ReplaceSkill(WAR.HeavySwing)]
    [JobInfo(Job.WAR)]
    [AdvancedCombo]
    WAR_ST_Advanced = 18002,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_BalanceOpener = 18058,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_StormsEye = 18005,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_InnerRelease = 18003,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_FellCleave = 18006,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_Infuriate = 18007,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_Onslaught = 18008,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_Upheaval = 18009,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_PrimalRend = 18013,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_PrimalWrath = 18010,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_PrimalRuination = 18011,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_RangedUptime = 18004,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_Interrupt = 18066,

    [ParentCombo(WAR_ST_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_ST_Stun = 18112,

    #endregion

    #region Advanced AoE
    [AutoAction(true, false)]
    [ConflictingCombos(WAR_AoE_Simple)]
    [ReplaceSkill(WAR.Overpower)]
    [JobInfo(Job.WAR)]
    [AdvancedCombo]
    WAR_AoE_Advanced = 18016,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_InnerRelease = 18019,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_Decimate = 18023,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_Infuriate = 18018,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_Onslaught = 18071,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_Orogeny = 18012,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_PrimalRend = 18021,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_PrimalWrath = 18020,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_PrimalRuination = 18022,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_RangedUptime = 18110,

    [ParentCombo(WAR_AoE_Advanced)]
    [JobInfo(Job.WAR)]
    WAR_AoE_Interrupt = 18067,

    [ParentCombo(WAR_AoE_Interrupt)]
    [JobInfo(Job.WAR)]
    WAR_AoE_Stun = 18068,

    #endregion

    #region Advanced Mitigation
    [JobInfo(Job.WAR)]
    WAR_Mitigation = 18131,

    [ParentCombo(WAR_Mitigation)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss = 18132,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_Rampart = 18133,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_ThrillOfBattle = 18134,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_Vengeance = 18135,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_ArmsLength = 18136,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_Reprisal = 18137,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_RawIntuition = 18138,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_ShakeItOff = 18139,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_Equilibrium = 18140,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_NonBoss_Holmgang = 18141,

    [ParentCombo(WAR_Mitigation)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss = 18142,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_Equilibrium = 18146,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_RawIntuition_OnCD = 18143,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_RawIntuition_TankBuster = 18147,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_Rampart = 18149,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_Vengeance = 18148,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_ThrillOfBattle = 18150,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_Reprisal = 18144,

    [ParentCombo(WAR_Mitigation_Boss)]
    [JobInfo(Job.WAR)]
    WAR_Mitigation_Boss_ShakeItOff = 18145,

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(WAR.ThrillOfBattle)]
    [JobInfo(Job.WAR)]
    [MitigationCombo]
    WAR_Mit_OneButton = 18045,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_Holmgang_Max = 18046,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_Bloodwhetting = 18047,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_Equilibrium = 18048,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_Reprisal = 18049,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_ThrillOfBattle = 18050,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_Rampart = 18051,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_ShakeItOff = 18052,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_ArmsLength = 18053,

    [ParentCombo(WAR_Mit_OneButton)]
    [JobInfo(Job.WAR)]
    WAR_Mit_Vengeance = 18054,

    [ReplaceSkill(WAR.ShakeItOff)]
    [JobInfo(Job.WAR)]
    [MitigationCombo]
    WAR_Mit_Party = 18111,
    #endregion

    #region Misc

    #region Fell Cleave Features
    [ReplaceSkill(WAR.FellCleave)]
    [ConflictingCombos(WAR_InfuriateFellCleave)]
    [JobInfo(Job.WAR)]
    WAR_FC_Features = 18122,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_InnerRelease = 18123,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_Infuriate = 18124,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_Onslaught = 18125,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_Upheaval = 18126,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_PrimalRend = 18127,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_PrimalWrath = 18128,

    [ParentCombo(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_FC_PrimalRuination = 18129,
    #endregion

    #region Basic Combo
    [ReplaceSkill(WAR.StormsPath)]
    [JobInfo(Job.WAR)]
    WAR_ST_StormsPathCombo = 18069,

    [ReplaceSkill(WAR.StormsEye)]
    [JobInfo(Job.WAR)]
    WAR_ST_StormsEyeCombo = 18070,

    [ReplaceSkill(WAR.MythrilTempest)]
    [JobInfo(Job.WAR)]
    WAR_AoE_BasicCombo = 18151,
    #endregion

    [ReplaceSkill(WAR.FellCleave, WAR.Decimate)]
    [ConflictingCombos(WAR_FC_Features)]
    [JobInfo(Job.WAR)]
    WAR_InfuriateFellCleave = 18024,

    [ParentCombo(WAR_InfuriateFellCleave)]
    [JobInfo(Job.WAR)]
    WAR_InfuriateFellCleave_IRFirst = 18027,

    [ReplaceSkill(WAR.StormsPath)]
    [JobInfo(Job.WAR)]
    WAR_EyePath = 18057,

    [ReplaceSkill(WAR.Berserk, WAR.InnerRelease)]
    [JobInfo(Job.WAR)]
    WAR_PrimalCombo_InnerRelease = 18026,

    [ReplaceSkill(WAR.Equilibrium)]
    [JobInfo(Job.WAR)]
    WAR_ThrillEquilibrium = 18055,

    [ReplaceSkill(WAR.NascentFlash)]
    [JobInfo(Job.WAR)]
    WAR_NascentFlash = 18017,

    [ParentCombo(WAR_NascentFlash)]
    [JobInfo(Job.WAR)]
    [Retargeted]
    WAR_NascentFlash_MO = 18154,

    [ParentCombo(WAR_NascentFlash)]
    [JobInfo(Job.WAR)]
    [Retargeted]
    WAR_NascentFlash_TT = 18155,

    [ReplaceSkill(WAR.RawIntuition, WAR.Bloodwhetting)]
    [JobInfo(Job.WAR)]
    [Retargeted(WAR.NascentFlash)]
    WAR_RawIntuition_Targeting = 18119,

    [ParentCombo(WAR_RawIntuition_Targeting)]
    [JobInfo(Job.WAR)]
    [Retargeted]
    WAR_RawIntuition_Targeting_MO = 18120,

    [ParentCombo(WAR_RawIntuition_Targeting)]
    [JobInfo(Job.WAR)]
    [Retargeted]
    WAR_RawIntuition_Targeting_TT = 18121,

    [ReplaceSkill(WAR.Onslaught)]
    [JobInfo(Job.WAR)]
    [Retargeted(WAR.Onslaught)]
    WAR_RetargetOnslaught = 18152,

    [ReplaceSkill(RoleActions.Physical.ArmsLength)]
    [JobInfo(Job.WAR)]
    WAR_ArmsLengthLockout = 18153,

    [ReplaceSkill(WAR.Holmgang)]
    [JobInfo(Job.WAR)]
    [Retargeted(WAR.Holmgang)]
    WAR_RetargetHolmgang = 18130,

    #endregion
    // Last value = 18155
    #endregion

    #region WHITE MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
    [ConflictingCombos(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    [SimpleCombo]
    WHM_ST_Simple_DPS = 19050,

    [AutoAction(true, false)]
    [ReplaceSkill(WHM.Holy, WHM.Holy3)]
    [ConflictingCombos(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    [SimpleCombo]
    WHM_AoE_Simple_DPS = 19051,

    [AutoAction(false, true)]
    [ReplaceSkill(WHM.Cure)]
    [ConflictingCombos(WHM_STHeals, WHM_Re_Cure)]
    [JobInfo(Job.WHM)]
    [SimpleCombo]
    [PossiblyRetargeted]
    WHM_SimpleSTHeals = 19052,

    [AutoAction(true, true)]
    [ReplaceSkill(WHM.Medica1)]
    [ConflictingCombos(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    [SimpleCombo]
    [PossiblyRetargeted]
    WHM_Simple_AoEHeals = 19054,

    #endregion

    #region Advanced Single Target DPS Combo

    [AutoAction(false, false)]
    [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
    [ConflictingCombos(WHM_ST_Simple_DPS)]
    [JobInfo(Job.WHM)]
    [AdvancedCombo]
    WHM_ST_MainCombo = 19099,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_Opener = 19023,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Aero, WHM.Aero2, WHM.Dia)]
    WHM_ST_MainCombo_Move_DoT = 19053,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_DoT = 19013,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_Assize = 19009,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_GlareIV = 19015,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_Misery = 19017,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_LilyOvercap = 19016,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_PresenceOfMind = 19008,

    [ParentCombo(WHM_ST_MainCombo)]
    [JobInfo(Job.WHM)]
    WHM_ST_MainCombo_Lucid = 19006,

    #endregion

    #region Advanced AoE DPS Combo

    [AutoAction(true, false)]
    [ReplaceSkill(WHM.Holy, WHM.Holy3)]
    [ConflictingCombos(WHM_AoE_Simple_DPS)]
    [JobInfo(Job.WHM)]
    [AdvancedCombo]
    WHM_AoE_DPS = 19190,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_SwiftHoly = 19197,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_Assize = 19192,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_GlareIV = 19196,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_Misery = 19194,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Aero, WHM.Aero2, WHM.Dia)]
    WHM_AoE_MainCombo_DoT = 19198,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_LilyOvercap = 19193,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_PresenceOfMind = 19195,

    [ParentCombo(WHM_AoE_DPS)]
    [JobInfo(Job.WHM)]
    WHM_AoE_DPS_Lucid = 19191,

    #endregion

    #region Advanced Single Target Heals Combo

    [AutoAction(false, true)]
    [ReplaceSkill(WHM.Cure)]
    [ConflictingCombos(WHM_SimpleSTHeals, WHM_Re_Cure)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.Cure)]
    [HealingCombo]
    WHM_STHeals = 19300,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    WHM_STHeals_Esuna = 19309,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    WHM_STHeals_Lucid = 19308,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    WHM_STHeals_ThinAir = 19304,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.Regen)]
    WHM_STHeals_Regen = 19301,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.AfflatusSolace)]
    WHM_STHeals_Solace = 19303,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.Tetragrammaton)]
    WHM_STHeals_Tetragrammaton = 19305,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.DivineBenison)]
    WHM_STHeals_Benison = 19306,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.Aquaveil)]
    WHM_STHeals_Aquaveil = 19307,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.Benediction)]
    WHM_STHeals_Benediction = 19302,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    WHM_STHeals_Temperance = 19310,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Asylum)]
    WHM_STHeals_Asylum = 19311,

    [ParentCombo(WHM_STHeals)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted(WHM.LiturgyOfTheBell, WHM.LiturgyOfTheBellRecast)]
    WHM_STHeals_LiturgyOfTheBell = 19312,

    #endregion

    #region Advanced AoE Heals Combo

    [AutoAction(true, true)]
    [ReplaceSkill(WHM.Medica1)]
    [ConflictingCombos(WHM_Simple_AoEHeals)]
    [JobInfo(Job.WHM)]
    [HealingCombo]
    WHM_AoEHeals = 19007,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Lucid = 19204,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_ThinAir = 19200,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Medica2 = 19205,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Rapture = 19011,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Cure3 = 19201,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Assize = 19202,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Plenary = 19203,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Asylum)]
    WHM_AoEHeals_Asylum = 19028,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_Temperance = 19210,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    WHM_AoEHeals_DivineCaress = 19207,

    [ParentCombo(WHM_AoEHeals)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.LiturgyOfTheBell)]
    WHM_AoEHeals_LiturgyOfTheBell = 19206,

    #endregion

    #region Small Features

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [JobInfo(Job.WHM)]
    WHM_Raise = 19004,

    [ParentCombo(WHM_Raise)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Raise)]
    WHM_Raise_Retarget = 19029,

    [ReplaceSkill(WHM.Raise)]
    [JobInfo(Job.WHM)]
    WHM_ThinAirRaise = 19014,

    [ReplaceSkill(WHM.AfflatusRapture)]
    [JobInfo(Job.WHM)]
    WHM_RaptureMisery = 19001,

    [ReplaceSkill(WHM.AfflatusSolace)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted("Retargeting Features below, Enable Afflatus Solace",
        Condition.WHMRetargetingFeaturesEnabledForSolace)]
    WHM_SolaceMisery = 19000,

    [ReplaceSkill(WHM.Cure2)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted("Retargeting Features below, Enable Cure", Condition.WHMRetargetingFeaturesEnabledForCure)]
    WHM_CureSync = 19002,
    #endregion

    #region Mitigation Features

    [ReplaceSkill(WHM.Aquaveil)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted("Retargeting Features below, Enable Aquaveil (and optionally Tetra and Benison)", Condition.WHMRetargetingFeaturesEnabledForSTMit)]
    WHM_Mit_ST = 19041,

    [ReplaceSkill(WHM.Asylum)]
    [JobInfo(Job.WHM)]
    [PossiblyRetargeted("Retargeting Features below, Enable Asylum", Condition.WHMRetargetingFeaturesEnabledForAoEMit)]
    WHM_Mit_AoE = 19040,

    #endregion

    #region Retargeting

    [JobInfo(Job.WHM)]
    WHM_Retargets = 19037,

    [ConflictingCombos(WHM_SimpleSTHeals, WHM_STHeals)]
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Cure)]
    WHM_Re_Cure = 19038,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure2)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Cure2)]
    WHM_Re_Cure2 = 19055,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.AfflatusSolace)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.AfflatusSolace)]
    WHM_Re_Solace = 19039,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Aquaveil)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Aquaveil)]
    WHM_Re_Aquaveil = 19036,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Asylum)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Asylum)]
    WHM_Re_Asylum = 19027,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.LiturgyOfTheBell)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.LiturgyOfTheBell)]
    WHM_Re_LiturgyOfTheBell = 19030,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure3)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Cure3)]
    WHM_Re_Cure3 = 19031,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Benediction)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Benediction)]
    WHM_Re_Benediction = 19032,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Tetragrammaton)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Tetragrammaton)]
    WHM_Re_Tetragrammaton = 19033,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Regen)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.Regen)]
    WHM_Re_Regen = 19034,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.DivineBenison)]
    [JobInfo(Job.WHM)]
    [Retargeted(WHM.DivineBenison)]
    WHM_Re_DivineBenison = 19035,

    #endregion

    #region Raidwide Heals

    [JobInfo(Job.WHM)]
    WHM_Raidwide = 19220,

    [ParentCombo(WHM_Raidwide)]
    [JobInfo(Job.WHM)]
    WHM_Raidwide_Asylum = 19221,

    [ParentCombo(WHM_Raidwide)]
    [JobInfo(Job.WHM)]
    WHM_Raidwide_Temperance = 19222,

    [ParentCombo(WHM_Raidwide)]
    [JobInfo(Job.WHM)]
    WHM_Raidwide_LiturgyOfTheBell = 19223,

    [ParentCombo(WHM_Raidwide)]
    [JobInfo(Job.WHM)]
    WHM_Raidwide_PlenaryIndulgence = 19224,

    #endregion

    // Last value = 19051 (then skips to next last used: 19210)

    #endregion

    // Non-combat

    #region DOH

    // [CustomComboInfo("Placeholder", "Placeholder.", DOH.JobID)]
    // DohPlaceholder = 50001,

    #endregion

    #region DOL

    [ReplaceSkill(DOL.AgelessWords, DOL.SolidReason)]
    [JobInfo(Job.MIN)]
    DOL_Eureka = 51001,

    [ReplaceSkill(DOL.ArborCall, DOL.ArborCall2, DOL.LayOfTheLand, DOL.LayOfTheLand2)]
    [JobInfo(Job.MIN)]
    DOL_NodeSearchingBuffs = 51012,

    [ReplaceSkill(DOL.Cast)]
    [JobInfo(Job.FSH)]
    FSH_CastHook = 51002,

    [JobInfo(Job.FSH)]
    FSH_Swim = 51008,

    [ReplaceSkill(DOL.Cast)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_CastGig = 51003,

    [ReplaceSkill(DOL.SurfaceSlap)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_SurfaceTrade = 51004,

    [ReplaceSkill(DOL.PrizeCatch)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_PrizeBounty = 51005,

    [ReplaceSkill(DOL.Snagging)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_SnaggingSalvage = 51006,

    [ReplaceSkill(DOL.CastLight)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_CastLight_ElectricCurrent = 51007,

    [ReplaceSkill(DOL.Mooch, DOL.MoochII)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_Mooch_SharkEye = 51009,

    [ReplaceSkill(DOL.FishEyes)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_FishEyes_VitalSight = 51010,

    [ReplaceSkill(DOL.Chum)]
    [ParentCombo(FSH_Swim)]
    [JobInfo(Job.FSH)]
    FSH_Chum_BaitedBreath = 51011,

    // Last value = 51011

    #endregion

    #endregion

    #region PvP Combos

    #region PvP GLOBAL FEATURES

    [PvPCustomCombo]
    [JobInfo(Job.ADV)]
    PvP_EmergencyHeals = 1100000,

    [PvPCustomCombo]
    [JobInfo(Job.ADV)]
    PvP_EmergencyGuard = 1100010,

    [PvPCustomCombo]
    [JobInfo(Job.ADV)]
    PvP_QuickPurify = 1100020,

    [PvPCustomCombo]
    [JobInfo(Job.ADV)]
    PvP_MashCancel = 1100030,

    [ParentCombo(PvP_MashCancel)]
    [JobInfo(Job.ADV)]
    PvP_MashCancelRecup = 1100031,

    // Last value = 1100030
    // Extra 0 on the end keeps things working the way they should be. Nothing to see here.

    #endregion

    #region ASTROLOGIAN

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.Malefic)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst = 111000,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst_DrawCard = 111002,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst_PlayCard = 111003,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst_DoubleMalefic = 111005,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst_Gravity)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst_DoubleGravity = 111009,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst_Gravity = 111006,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    ASTPvP_Burst_Macrocosmos = 111007,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    ASTPvP_Diabrosis = 111010,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted]
    ASTPvP_Burst_Heal = 111011,

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.Epicycle)]
    [JobInfo(Job.AST)]
    ASTPvP_Epicycle = 111008,

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.AspectedBenefic)]
    [JobInfo(Job.AST)]
    [PossiblyRetargeted]
    ASTPvP_Heal = 111004,

    // Last value = 111010

    #endregion

    #region BLACK MAGE

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(BLMPvP.Fire, BLMPvP.Blizzard)]
    [JobInfo(Job.BLM)]
    BLMPvP_BurstMode = 112000,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.BLM)]
    BLMPvP_Burst = 112001,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.BLM)]
    BLMPvP_Xenoglossy = 112002,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.BLM)]
    BLMPvP_Lethargy = 112003,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.BLM)]
    BLMPvP_ElementalWeave = 112004,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.BLM)]
    BLMPvP_ElementalStar = 112005,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.BLM)]
    BLMPvP_PhantomDart = 112007,

    [PvPCustomCombo]
    [ReplaceSkill(BLMPvP.AetherialManipulation)]
    [JobInfo(Job.BLM)]
    BLMPvP_Manipulation_Feature = 112006,


    // Last value = 112007

    #endregion

    #region BARD

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(BRDPvP.PowerfulShot)]
    [JobInfo(Job.BRD)]
    BRDPvP_BurstMode = 113000,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_SilentNocturne = 113001,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_ApexArrow = 113002,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_BlastArrow = 113003,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_HarmonicArrow = 113004,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_EncoreOfLight = 113005,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_Wardens = 113006,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [JobInfo(Job.BRD)]
    BRDPvP_Eagle = 113007,

    // Last value = 113007

    #endregion

    #region DANCER

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(DNCPvP.Fountain)]
    [JobInfo(Job.DNC)]
    DNCPvP_BurstMode = 114000,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [JobInfo(Job.DNC)]
    DNCPvP_BurstMode_HoningDance = 114001,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [JobInfo(Job.DNC)]
    DNCPvP_BurstMode_CuringWaltz = 114002,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [JobInfo(Job.DNC)]
    DNCPvP_BurstMode_Partner = 114003,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [JobInfo(Job.DNC)]
    DNCPvP_BurstMode_Dash = 114004,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [JobInfo(Job.DNC)]
    DNCPvP_Eagle = 114005,

    // Last value = 114005

    #endregion

    #region DARK KNIGHT

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(DRKPvP.Souleater)]
    [JobInfo(Job.DRK)]
    DRKPvP_Burst = 115000,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_Plunge = 115001,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Plunge)]
    [JobInfo(Job.DRK)]
    DRKPvP_PlungeMelee = 115002,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_SaltedEarth = 115003,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_SaltAndDarkness = 115004,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_Impalement = 115005,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_Shadowbringer = 115006,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_BlackestNight = 115007,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_Scorn = 115008,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [JobInfo(Job.DRK)]
    DRKPvP_Rampart = 115009,

    // Last value = 115009

    #endregion

    #region DRAGOON

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(DRGPvP.Drakesbane)]
    [JobInfo(Job.DRG)]
    DRGPvP_Burst = 116000,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_Geirskogul = 116001,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Geirskogul)]
    [JobInfo(Job.DRG)]
    DRGPvP_Nastrond = 116002,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_HorridRoar = 116003,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_ChaoticSpringSustain = 116004,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_ChaoticSpringExecute = 116009,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_WyrmwindThrust = 116006,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_HighJump = 116007,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_BurstProtection = 116008,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [JobInfo(Job.DRG)]
    DRGPvP_Smite = 116010,

    // Last value = 116010

    #endregion

    #region GUNBREAKER

    #region Burst Mode

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(GNBPvP.SolidBarrel)]
    [JobInfo(Job.GNB)]
    GNBPvP_Burst = 117000,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_FatedCircle = 117001,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_ST_GnashingFang = 117004,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_ST_Continuation = 117005,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_RoughDivide = 117006,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_BlastingZone = 117007,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_Corundum = 117011,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [JobInfo(Job.GNB)]
    GNBPvP_Rampart = 117012,

    #endregion

    #region Option Select

    [PvPCustomCombo]
    [ReplaceSkill(GNBPvP.GnashingFang)]
    [JobInfo(Job.GNB)]
    GNBPvP_GnashingFang = 117010,

    // Last value = 117012

    #endregion

    #endregion

    #region MACHINIST

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(MCHPvP.BlastCharge)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode = 118000,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_AirAnchor = 118001,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_Analysis = 118002,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode_Analysis)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_AltAnalysis = 118003,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_Drill = 118004,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode_Drill)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_AltDrill = 118005,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_BioBlaster = 118006,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_ChainSaw = 118007,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_FullMetalField = 118008,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_Wildfire = 118009,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_BurstMode_MarksmanSpite = 118011,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [JobInfo(Job.MCH)]
    MCHPvP_Eagle = 118012,

    // Last value = 118012

    #endregion

    #region MONK

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(MNKPvP.PhantomRush)]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst = 119000,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst_Meteodrive = 119006,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst_Thunderclap = 119001,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst_RiddleOfEarth = 119002,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst_FiresReply = 119003,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst_RisingPhoenix = 119004,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Burst_WindsReply = 119005,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.MNK)]
    MNKPvP_Smite = 119007,

    // Last value = 119007

    #endregion

    #region NINJA

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(NINPvP.AeolianEdge)]
    [JobInfo(Job.NIN)]
    NINPvP_ST_BurstMode = 120000,

    [PvPCustomCombo]
    [ReplaceSkill(NINPvP.FumaShuriken)]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_BurstMode = 120001,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_Meisui = 120002,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_MudraMode = 120013,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_FumaShuriken = 120003,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_ThreeMudra = 120004,

    [ParentCombo(NINPvP_ST_ThreeMudra)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_ThreeMudraPool = 120014,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_Dokumori = 120005,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_Bunshin = 120006,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_ST_SeitonTenchu = 120007,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_Meisui = 120008,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_MudraMode = 120016,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_FumaShuriken = 120009,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_ThreeMudra = 120010,

    [ParentCombo(NINPvP_AoE_ThreeMudra)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_ThreeMudraPool = 120015,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_Dokumori = 120011,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_Bunshin = 120012,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_AoE_SeitonTenchu = 120017,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.NIN)]
    NINPvP_Smite = 120018,

    // Last value = 120018

    #endregion

    #region PALADIN

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(PLDPvP.RoyalAuthority)]
    [JobInfo(Job.PLD)]
    PLDPvP_Burst = 121000,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_ShieldSmite = 121001,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_Imperator = 121002,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_Confiteor = 121003,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_HolySpirit = 121004,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_Intervene = 121005,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_Intervene_Melee = 121006,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_PhalanxCombo = 121007,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_Sheltron = 121008,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [JobInfo(Job.PLD)]
    PLDPvP_Rampart = 121009,

    // Last value = 121009

    #endregion

    #region PICTOMANCER

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(PCTPvP.FireInRed)]
    [JobInfo(Job.PCT)]
    PCTPvP_Burst = 140000,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_BurstControl = 140001,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_TemperaCoat = 140002,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_SubtractivePalette = 140003,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_CreatureMotif = 140004,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_LivingMuse = 140005,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_MogOfTheAges = 140006,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_HolyInWhite = 140007,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [JobInfo(Job.PCT)]
    PCTPvP_StarPrism = 140008,

    [ParentCombo(PCTPvP_Burst)]
    [PvPCustomCombo]
    [JobInfo(Job.PCT)]
    PCTPvP_PhantomDart = 140009,

    [PvPCustomCombo]
    [ReplaceSkill(PCTPvP.LivingMuse)]
    [JobInfo(Job.PCT)]
    PCTPvP_OneButtonMotifs = 140010,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_OneButtonMotifs)]
    [JobInfo(Job.PCT)]
    PCTPvP_StarPrismOneButtonMotifs = 140011,

    // Last value = 140011

    #endregion

    #region REAPER

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(RPRPvP.Slice)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst = 122000,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_GrimSwathe = 122009,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_DeathWarrant = 122001,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_ImmortalPooling = 122003,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_Enshrouded = 122004,

    #region RPR Enshrouded Option

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst_Enshrouded)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_Enshrouded_DeathWarrant = 122005,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst_Enshrouded)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_Enshrouded_Communio = 122006,

    #endregion

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_RangedHarvest = 122007,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Burst_ArcaneCircle = 122008,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [JobInfo(Job.RPR)]
    RPRPvP_Smite = 122010,

    // Last value = 122010

    #endregion

    #region RED MAGE

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(RDMPvP.Jolt3)]
    [JobInfo(Job.RDM)]
    RDMPvP_BurstMode = 123000,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [JobInfo(Job.RDM)]
    RDMPvP_Riposte = 123001,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [JobInfo(Job.RDM)]
    RDMPvP_Resolution = 123002,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [JobInfo(Job.RDM)]
    RDMPvP_Embolden = 123003,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [JobInfo(Job.RDM)]
    RDMPvP_Corps = 123004,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [JobInfo(Job.RDM)]
    RDMPvP_Displacement = 123005,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [JobInfo(Job.RDM)]
    RDMPvP_Forte = 123006,

    [PvPCustomCombo]
    [ReplaceSkill(RDMPvP.CorpsACorps, RDMPvP.Displacement)]
    [JobInfo(Job.RDM)]
    RDMPvP_Dash_Feature = 123007,

    [ParentCombo(RDMPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.RDM)]
    RDMPvP_PhantomDart = 123008,

    // Last value = 123008

    #endregion

    #region SAGE

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(SGEPvP.Dosis)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode = 124000,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_Pneuma = 124001,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_Eukrasia = 124002,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_Phlegma = 124003,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_Psyche = 124004,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_Toxikon = 124005,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_Toxikon2 = 124006,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_BurstMode_KardiaReminder = 124007,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [JobInfo(Job.SGE)]
    SGEPvP_Diabrosis = 124008,

    [PvPCustomCombo]
    [ReplaceSkill(SGEPvP.Kardia)]
    [JobInfo(Job.SGE)]
    [Retargeted]
    SGEPvP_RetargetKardia = 124009,

    // Last value = 124009

    #endregion

    #region SAMURAI

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(SAMPvP.Yukikaze)]
    [JobInfo(Job.SAM)]
    SAMPvP_Burst = 125000,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [JobInfo(Job.SAM)]
    SAMPvP_Meikyo = 125001,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [JobInfo(Job.SAM)]
    SAMPvP_Chiten = 125002,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [JobInfo(Job.SAM)]
    SAMPvP_Mineuchi = 125003,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [JobInfo(Job.SAM)]
    SAMPvP_Soten = 125004,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [JobInfo(Job.SAM)]
    SAMPvP_Zantetsuken = 125005,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [JobInfo(Job.SAM)]
    SAMPvP_Smite = 125006,

    // Last value = 125006

    #endregion

    #region SCHOLAR

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(SCHPvP.Broil)]
    [JobInfo(Job.SCH)]
    SCHPvP_Burst = 126000,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [JobInfo(Job.SCH)]
    SCHPvP_Expedient = 126001,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [JobInfo(Job.SCH)]
    SCHPvP_Biolysis = 126002,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [JobInfo(Job.SCH)]
    SCHPvP_DeploymentTactics = 126003,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [JobInfo(Job.SCH)]
    SCHPvP_ChainStratagem = 126004,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [JobInfo(Job.SCH)]
    SCHPvP_Diabrosis = 126005,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [JobInfo(Job.SCH)]
    [PossiblyRetargeted]
    SCHPvP_Adlo = 126006,

    [PvPCustomCombo]
    [ReplaceSkill(SCHPvP.Adloquilum)]
    [JobInfo(Job.SCH)]
    [Retargeted]
    SCHPvP_RetargetAdlo = 126007,

    // Last value = 126006

    #endregion

    #region SUMMONER

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(SMNPvP.Ruin3)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode = 127000,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_RadiantAegis = 127001,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_CrimsonCyclone = 127002,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_CrimsonStrike = 127003,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_MountainBuster = 127004,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_Slipstream = 127005,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_Necrotize = 127006,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_DeathFlare = 127007,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [JobInfo(Job.SMN)]
    SMNPvP_BurstMode_BrandofPurgatory = 127008,

    [ParentCombo(SMNPvP_BurstMode)]
    [PvPCustomCombo]
    [JobInfo(Job.SMN)]
    SMNPvP_PhantomDart = 127009,

    // Last value = 127008

    #endregion

    #region VIPER

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(VPRPvP.SteelFangs)]
    [JobInfo(Job.VPR)]
    VPRPvP_Burst = 130000,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [JobInfo(Job.VPR)]
    VPRPvP_Bloodcoil = 130001,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [JobInfo(Job.VPR)]
    VPRPvP_UncoiledFury = 130002,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [JobInfo(Job.VPR)]
    VPRPvP_Backlash = 130003,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [JobInfo(Job.VPR)]
    VPRPvP_RattlingCoil = 130004,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [JobInfo(Job.VPR)]
    VPRPvP_Slither = 130005,

    [PvPCustomCombo]
    [ReplaceSkill(VPRPvP.SnakeScales)]
    [JobInfo(Job.VPR)]
    VPRPvP_SnakeScales_Feature = 130006,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [JobInfo(Job.VPR)]
    VPRPvP_Smite = 130007,

    // Last value = 130007

    #endregion

    #region WARRIOR

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(WARPvP.HeavySwing)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode = 128000,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_Bloodwhetting = 128001,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_Blota = 128003,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_PrimalRend = 128004,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_InnerChaos = 128005,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_Orogeny = 128006,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_Onslaught = 128007,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_BurstMode_PrimalScream = 128008,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [JobInfo(Job.WAR)]
    WARPvP_Rampart = 128009,

    // Last value = 128009

    #endregion

    #region WHITE MAGE

    [AutoAction(false, false)]
    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.Glare)]
    [JobInfo(Job.WHM)]
    WHMPvP_Burst = 129000,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [JobInfo(Job.WHM)]
    WHMPvP_Afflatus_Misery = 129001,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [JobInfo(Job.WHM)]
    WHMPvP_Mirace_of_Nature = 129002,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [JobInfo(Job.WHM)]
    WHMPvP_Seraph_Strike = 129003,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [JobInfo(Job.WHM)]
    WHMPvP_AfflatusPurgation = 129008,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [JobInfo(Job.WHM)]
    WHMPvP_Diabrosis = 129009,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [JobInfo(Job.WHM)]
    [Retargeted]
    WHMPvP_Burst_Heals = 129010,

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.Cure2)]
    [JobInfo(Job.WHM)]
    [Retargeted]
    WHMPvP_Heals = 129004,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Aquaveil)]
    [JobInfo(Job.WHM)]
    WHMPvP_Cure3 = 129005,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Heals)]
    [JobInfo(Job.WHM)]
    WHMPvP_Aquaveil = 129007,

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.SeraphStrike)]
    [JobInfo(Job.WHM)]
    WHMPvP_Seraphstrike = 129011,

    // Last value = 129011

    #endregion

    #endregion
}
