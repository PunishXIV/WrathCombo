#region

using ECommons.ExcelServices;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;

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

    [Role(JobRole.Tank)]
    [CustomComboInfo(Job.ADV, ALL_Tank_Menu)]
    ALL_Tank_Menu = 100099,

    [Role(JobRole.Tank)]
    [ReplaceSkill(RoleActions.Tank.LowBlow, PLD.ShieldBash)]
    [ConflictingCombos(PLD_RetargetShieldBash)]
    [ParentCombo(ALL_Tank_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Tank_Interrupt)]
    ALL_Tank_Interrupt = 100000,

    [ParentCombo(ALL_Tank_Interrupt)]
    [Retargeted(RoleActions.Tank.Interject, RoleActions.Tank.LowBlow, PLD.ShieldBash)]
    [CustomComboInfo(Job.ADV, ALL_Tank_Interrupt_Retarget)]
    ALL_Tank_Interrupt_Retarget = 100005,

    [Role(JobRole.Tank)]
    [ReplaceSkill(RoleActions.Tank.Reprisal)]
    [ParentCombo(ALL_Tank_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Tank_Reprisal)]
    ALL_Tank_Reprisal = 100001,

    [Role(JobRole.Tank)]
    [ReplaceSkill(RoleActions.Tank.Shirk)]
    [ParentCombo(ALL_Tank_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Tank_ShirkRetargeting)]
    [Retargeted(RoleActions.Tank.Shirk)]
    ALL_Tank_ShirkRetargeting = 100002,

    [Role(JobRole.Tank)]
    [ParentCombo(ALL_Tank_ShirkRetargeting)]
    [CustomComboInfo(Job.ADV, ALL_Tank_ShirkRetargeting_Healer)]
    [Retargeted]
    ALL_Tank_ShirkRetargeting_Healer = 100003,

    [Role(JobRole.Tank)]
    [ParentCombo(ALL_Tank_ShirkRetargeting)]
    [CustomComboInfo(Job.ADV, ALL_Tank_ShirkRetargeting_Fallback)]
    [Retargeted]
    ALL_Tank_ShirkRetargeting_Fallback = 100004,

    #endregion

    #region Global Healer Features

    [Role(JobRole.Healer)]
    [CustomComboInfo(Job.ADV, ALL_Healer_Menu)]
    ALL_Healer_Menu = 100098,

    [Role(JobRole.Healer)]
    [ReplaceSkill(AST.Ascend, WHM.Raise, SCH.Resurrection, SGE.Egeiro)]
    [ConflictingCombos(AST_Raise_Alternative, SCH_Raise, SGE_Raise, WHM_Raise)]
    [ParentCombo(ALL_Healer_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Healer_Raise)]
    ALL_Healer_Raise = 100010,

    [ParentCombo(ALL_Healer_Raise)]
    [CustomComboInfo(Job.ADV, ALL_Healer_Raise_Retarget)]
    [Retargeted(WHM.Raise, AST.Ascend, SGE.Egeiro, SCH.Resurrection)]
    ALL_Healer_Raise_Retarget = 100011,

    [Role(JobRole.Healer)]
    [ReplaceSkill(RoleActions.Healer.Esuna)]
    [ParentCombo(ALL_Healer_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Healer_EsunaRetargeting)]
    [Retargeted(RoleActions.Healer.Esuna)]
    ALL_Healer_EsunaRetargeting = 100012,

    [Role(JobRole.Healer)]
    [ReplaceSkill(RoleActions.Healer.Rescue)]
    [ParentCombo(ALL_Healer_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Healer_RescueRetargeting)]
    [Retargeted(RoleActions.Healer.Rescue)]
    ALL_Healer_RescueRetargeting = 100013,
    #endregion

    #region Global Magical Ranged Features

    [Role(JobRole.MagicalDPS)]
    [CustomComboInfo(Job.ADV, ALL_Caster_Menu)]
    ALL_Caster_Menu = 100097,

    [Role(JobRole.MagicalDPS)]
    [ReplaceSkill(RoleActions.Caster.Addle)]
    [ParentCombo(ALL_Caster_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Caster_Addle)]
    ALL_Caster_Addle = 100020,

    [Role(JobRole.MagicalDPS)]
    [ReplaceSkill(RDM.Verraise, SMN.Resurrection, BLU.AngelWhisper)]
    [ConflictingCombos(SMN_Raise, RDM_Raise)]
    [ParentCombo(ALL_Caster_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Caster_Raise)]
    ALL_Caster_Raise = 100021,

    [ParentCombo(ALL_Caster_Raise)]
    [CustomComboInfo(Job.ADV, ALL_Caster_Raise_Retarget)]
    [Retargeted(BLU.AngelWhisper, RDM.Verraise, SMN.Resurrection)]
    ALL_Caster_Raise_Retarget = 100022,

    #endregion

    #region Global Melee Features

    [Role(JobRole.MeleeDPS)]
    [CustomComboInfo(Job.ADV, ALL_Melee_Menu)]
    ALL_Melee_Menu = 100096,

    [Role(JobRole.MeleeDPS)]
    [ReplaceSkill(RoleActions.Melee.Feint)]
    [ParentCombo(ALL_Melee_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Melee_Feint)]
    ALL_Melee_Feint = 100030,

    [Role(JobRole.MeleeDPS)]
    [ReplaceSkill(RoleActions.Melee.TrueNorth)]
    [ParentCombo(ALL_Melee_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Melee_TrueNorth)]
    ALL_Melee_TrueNorth = 100031,

    #endregion

    #region Global Ranged Physical Features

    [Role(JobRole.RangedDPS)]
    [CustomComboInfo(Job.ADV, ALL_Ranged_Menu)]
    ALL_Ranged_Menu = 100095,

    [Role(JobRole.RangedDPS)]
    [ReplaceSkill(MCH.Tactician, BRD.Troubadour, DNC.ShieldSamba)]
    [ParentCombo(ALL_Ranged_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Ranged_Mitigation)]
    ALL_Ranged_Mitigation = 100040,

    [Role(JobRole.RangedDPS)]
    [ReplaceSkill(RoleActions.PhysRanged.FootGraze)]
    [ParentCombo(ALL_Ranged_Menu)]
    [CustomComboInfo(Job.ADV, ALL_Ranged_Interrupt)]
    ALL_Ranged_Interrupt = 100041,

    #endregion

    //Non-gameplay Features
    //[CustomComboInfo(Job.ADV, Bozja_Tank)]
    //AllOutputCombatLog = 100094,

    // Last value = 100094

    #endregion

    #region BOZJA ACTIONS

    [Bozja]
    [Role(JobRole.Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank)]
    Bozja_Tank = 210000,

    #region Tank
    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostFocus)]
    Bozja_Tank_LostFocus = 210001,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostFontOfPower)]
    Bozja_Tank_LostFontOfPower = 210002,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostSlash)]
    Bozja_Tank_LostSlash = 210003,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostDeath)]
    Bozja_Tank_LostDeath = 210004,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_BannerOfNobleEnds)]
    Bozja_Tank_BannerOfNobleEnds = 210005,

    [Bozja]
    [ParentCombo(Bozja_Tank_BannerOfNobleEnds)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_PowerEnds)]
    Bozja_Tank_PowerEnds = 210006,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_BannerOfHonoredSacrifice)]
    Bozja_Tank_BannerOfHonoredSacrifice = 210007,

    [Bozja]
    [ParentCombo(Bozja_Tank_BannerOfHonoredSacrifice)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_PowerSacrifice)]
    Bozja_Tank_PowerSacrifice = 210008,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_BannerOfHonedAcuity)]
    Bozja_Tank_BannerOfHonedAcuity = 210009,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostFairTrade)]
    Bozja_Tank_LostFairTrade = 210010,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostAssassination)]
    Bozja_Tank_LostAssassination = 210011,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostManawall)]
    Bozja_Tank_LostManawall = 210012,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_BannerOfTirelessConviction)]
    Bozja_Tank_BannerOfTirelessConviction = 210013,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostBloodRage)]
    Bozja_Tank_LostBloodRage = 210014,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_BannerOfSolemnClarity)]
    Bozja_Tank_BannerOfSolemnClarity = 210015,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostCure)]
    Bozja_Tank_LostCure = 210016,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostCure2)]
    Bozja_Tank_LostCure2 = 210017,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostCure3)]
    Bozja_Tank_LostCure3 = 210018,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostCure4)]
    Bozja_Tank_LostCure4 = 210019,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostArise)]
    Bozja_Tank_LostArise = 210020,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostSacrifice)]
    Bozja_Tank_LostSacrifice = 210021,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostReraise)]
    Bozja_Tank_LostReraise = 210022,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostSpellforge)]
    Bozja_Tank_LostSpellforge = 210023,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostSteelsting)]
    Bozja_Tank_LostSteelsting = 210024,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostProtect)]
    Bozja_Tank_LostProtect = 210025,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostShell)]
    Bozja_Tank_LostShell = 210026,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostReflect)]
    Bozja_Tank_LostReflect = 210027,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostBravery)]
    Bozja_Tank_LostBravery = 210028,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostAethershield)]
    Bozja_Tank_LostAethershield = 210029,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostProtect2)]
    Bozja_Tank_LostProtect2 = 210030,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostShell2)]
    Bozja_Tank_LostShell2 = 210031,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostBubble)]
    Bozja_Tank_LostBubble = 210032,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostStealth)]
    Bozja_Tank_LostStealth = 210033,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostSwift)]
    Bozja_Tank_LostSwift = 210034,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostFontOfSkill)]
    Bozja_Tank_LostFontOfSkill = 210035,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostImpetus)]
    Bozja_Tank_LostImpetus = 210036,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostParalyze3)]
    Bozja_Tank_LostParalyze3 = 210037,

    [Bozja]
    [ParentCombo(Bozja_Tank)]
    [CustomComboInfo(Job.ADV, Bozja_Tank_LostRampage)]
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
    [Role(JobRole.Tank)]
    [CustomComboInfo(Job.ADV, Variant_Tank)]
    Variant_Tank = 200000,

    [Variant]
    [Role(JobRole.Tank)]
    [ParentCombo(Variant_Tank)]
    [CustomComboInfo(Job.ADV, Variant_Tank_Cure)]
    Variant_Tank_Cure = 200001,

    [Variant]
    [Role(JobRole.Tank)]
    [ParentCombo(Variant_Tank)]
    [CustomComboInfo(Job.ADV, Variant_Tank_Ultimatum)]
    Variant_Tank_Ultimatum = 200002,

    [Variant]
    [Role(JobRole.Tank)]
    [ParentCombo(Variant_Tank)]
    [CustomComboInfo(Job.ADV, Variant_Tank_Raise)]
    Variant_Tank_Raise = 200003,

    [Variant]
    [Role(JobRole.Tank)]
    [ParentCombo(Variant_Tank)]
    [CustomComboInfo(Job.ADV, Variant_Tank_SpiritDart)]
    Variant_Tank_SpiritDart = 200004,

    [Variant]
    [Role(JobRole.Tank)]
    [ParentCombo(Variant_Tank)]
    [CustomComboInfo(Job.ADV, Variant_Tank_EagleEyeShot)]
    Variant_Tank_EagleEyeShot = 200024,


    [Variant]
    [Role(JobRole.Healer)]
    [CustomComboInfo(Job.ADV, Variant_Healer)]
    Variant_Healer = 200005,

    [Variant]
    [Role(JobRole.Healer)]
    [ParentCombo(Variant_Healer)]
    [CustomComboInfo(Job.ADV, Variant_Healer_Ultimatum)]
    Variant_Healer_Ultimatum = 200006,

    [Variant]
    [Role(JobRole.Healer)]
    [ParentCombo(Variant_Healer)]
    [CustomComboInfo(Job.ADV, Variant_Healer_SpiritDart)]
    Variant_Healer_SpiritDart = 200007,

    [Variant]
    [Role(JobRole.Healer)]
    [ParentCombo(Variant_Healer)]
    [CustomComboInfo(Job.ADV, Variant_Healer_Rampart)]
    Variant_Healer_Rampart = 200008,

    [Variant]
    [Role(JobRole.Healer)]
    [ParentCombo(Variant_Healer)]
    [CustomComboInfo(Job.ADV, Variant_Healer_EagleEyeShot)]
    Variant_Healer_EagleEyeShot = 200025,


    [Variant]
    [Role(JobRole.MeleeDPS)]
    [CustomComboInfo(Job.ADV, Variant_Melee)]
    Variant_Melee = 200009,

    [Variant]
    [Role(JobRole.MeleeDPS)]
    [ParentCombo(Variant_Melee)]
    [CustomComboInfo(Job.ADV, Variant_Melee_Cure)]
    Variant_Melee_Cure = 200010,

    [Variant]
    [Role(JobRole.MeleeDPS)]
    [ParentCombo(Variant_Melee)]
    [CustomComboInfo(Job.ADV, Variant_Melee_Ultimatum)]
    Variant_Melee_Ultimatum = 200011,

    [Variant]
    [Role(JobRole.MeleeDPS)]
    [ParentCombo(Variant_Melee)]
    [CustomComboInfo(Job.ADV, Variant_Melee_Raise)]
    Variant_Melee_Raise = 200012,

    [Variant]
    [Role(JobRole.MeleeDPS)]
    [ParentCombo(Variant_Melee)]
    [CustomComboInfo(Job.ADV, Variant_Melee_Rampart)]
    Variant_Melee_Rampart = 200013,

    [Variant]
    [Role(JobRole.MeleeDPS)]
    [ParentCombo(Variant_Melee)]
    [CustomComboInfo(Job.ADV, Variant_Melee_EagleEyeShot)]
    Variant_Melee_EagleEyeShot = 200026,


    [Variant]
    [Role(JobRole.RangedDPS)]
    [CustomComboInfo(Job.ADV, Variant_PhysRanged)]
    Variant_PhysRanged = 200014,

    [Variant]
    [Role(JobRole.RangedDPS)]
    [ParentCombo(Variant_PhysRanged)]
    [CustomComboInfo(Job.ADV, Variant_PhysRanged_Cure)]
    Variant_PhysRanged_Cure = 200015,

    [Variant]
    [Role(JobRole.RangedDPS)]
    [ParentCombo(Variant_PhysRanged)]
    [CustomComboInfo(Job.ADV, Variant_PhysRanged_Ultimatum)]
    Variant_PhysRanged_Ultimatum = 200016,

    [Variant]
    [Role(JobRole.RangedDPS)]
    [ParentCombo(Variant_PhysRanged)]
    [CustomComboInfo(Job.ADV, Variant_PhysRanged_Raise)]
    Variant_PhysRanged_Raise = 200017,

    [Variant]
    [Role(JobRole.RangedDPS)]
    [ParentCombo(Variant_PhysRanged)]
    [CustomComboInfo(Job.ADV, Variant_PhysRanged_Rampart)]
    Variant_PhysRanged_Rampart = 200018,

    [Variant]
    [Role(JobRole.RangedDPS)]
    [ParentCombo(Variant_PhysRanged)]
    [CustomComboInfo(Job.ADV, Variant_PhysRanged_EagleEyeShot)]
    Variant_PhysRanged_EagleEyeShot = 200027,


    [Variant]
    [Role(JobRole.MagicalDPS)]
    [CustomComboInfo(Job.ADV, Variant_Magic)]
    Variant_Magic = 200019,

    [Variant]
    [Role(JobRole.MagicalDPS)]
    [ParentCombo(Variant_Magic)]
    [CustomComboInfo(Job.ADV, Variant_Magic_Cure)]
    Variant_Magic_Cure = 200020,

    [Variant]
    [Role(JobRole.MagicalDPS)]
    [ParentCombo(Variant_Magic)]
    [CustomComboInfo(Job.ADV, Variant_Magic_Ultimatum)]
    Variant_Magic_Ultimatum = 200021,

    [Variant]
    [Role(JobRole.MagicalDPS)]
    [ParentCombo(Variant_Magic)]
    [CustomComboInfo(Job.ADV, Variant_Magic_Raise)]
    Variant_Magic_Raise = 200022,

    [Variant]
    [Role(JobRole.MagicalDPS)]
    [ParentCombo(Variant_Magic)]
    [CustomComboInfo(Job.ADV, Variant_Magic_Rampart)]
    Variant_Magic_Rampart = 200023,

    [Variant]
    [Role(JobRole.MagicalDPS)]
    [ParentCombo(Variant_Magic)]
    [CustomComboInfo(Job.ADV, Variant_Magic_EagleEyeShot)]
    Variant_Magic_EagleEyeShot = 200028,

    // last value = 200028

    #endregion

    #region PHANTOM ACTIONS
    [OccultCrescent(OccultCrescent.JobIDs.Freelancer)]
    [CustomComboInfo(Job.ADV, Phantom_Freelancer)]
    Phantom_Freelancer = 110000,

    [OccultCrescent]
    [ParentCombo(Phantom_Freelancer)]
    [CustomComboInfo(Job.ADV, Phantom_Freelancer_OccultResuscitation)]
    Phantom_Freelancer_OccultResuscitation = 110001,

    [OccultCrescent]
    [ParentCombo(Phantom_Freelancer)]
    [CustomComboInfo(Job.ADV, Phantom_Freelancer_OccultTreasuresight)]
    Phantom_Freelancer_OccultTreasuresight = 110002,

    [OccultCrescent(OccultCrescent.JobIDs.Knight)]
    [CustomComboInfo(Job.ADV, Phantom_Knight)]
    Phantom_Knight = 110003,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo(Job.ADV, Phantom_Knight_PhantomGuard)]
    Phantom_Knight_PhantomGuard = 110004,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo(Job.ADV, Phantom_Knight_Pray)]
    Phantom_Knight_Pray = 110005,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo(Job.ADV, Phantom_Knight_OccultHeal)]
    Phantom_Knight_OccultHeal = 110006,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo(Job.ADV, Phantom_Knight_Pledge)]
    Phantom_Knight_Pledge = 110007,

    [OccultCrescent(OccultCrescent.JobIDs.Monk)]
    [CustomComboInfo(Job.ADV, Phantom_Monk)]
    Phantom_Monk = 110008,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo(Job.ADV, Phantom_Monk_PhantomKick)]
    Phantom_Monk_PhantomKick = 110009,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo(Job.ADV, Phantom_Monk_OccultCounter)]
    Phantom_Monk_OccultCounter = 110010,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo(Job.ADV, Phantom_Monk_Counterstance)]
    Phantom_Monk_Counterstance = 110011,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo(Job.ADV, Phantom_Monk_OccultChakra)]
    Phantom_Monk_OccultChakra = 110012,

    [OccultCrescent(OccultCrescent.JobIDs.Thief)]
    [CustomComboInfo(Job.ADV, Phantom_Thief)]
    Phantom_Thief = 110013,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo(Job.ADV, Phantom_Thief_OccultSprint)]
    Phantom_Thief_OccultSprint = 110014,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo(Job.ADV, Phantom_Thief_Steal)]
    Phantom_Thief_Steal = 110015,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo(Job.ADV, Phantom_Thief_Vigilance)]
    Phantom_Thief_Vigilance = 110016,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo(Job.ADV, Phantom_Thief_TrapDetection)]
    Phantom_Thief_TrapDetection = 110017,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo(Job.ADV, Phantom_Thief_PilferWeapon)]
    Phantom_Thief_PilferWeapon = 110018,

    [OccultCrescent(OccultCrescent.JobIDs.Samurai)]
    [CustomComboInfo(Job.ADV, Phantom_Samurai)]
    Phantom_Samurai = 110053,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo(Job.ADV, Phantom_Samurai_Mineuchi)]
    Phantom_Samurai_Mineuchi = 110054,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo(Job.ADV, Phantom_Samurai_Shirahadori)]
    Phantom_Samurai_Shirahadori = 110055,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo(Job.ADV, Phantom_Samurai_Iainuki)]
    Phantom_Samurai_Iainuki = 110056,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo(Job.ADV, Phantom_Samurai_Zeninage)]
    Phantom_Samurai_Zeninage = 110057,

    [OccultCrescent(OccultCrescent.JobIDs.Berserker)]
    [CustomComboInfo(Job.ADV, Phantom_Berserker)]
    Phantom_Berserker = 110019,

    [OccultCrescent]
    [ParentCombo(Phantom_Berserker)]
    [CustomComboInfo(Job.ADV, Phantom_Berserker_Rage)]
    Phantom_Berserker_Rage = 110020,

    [OccultCrescent]
    [ParentCombo(Phantom_Berserker)]
    [CustomComboInfo(Job.ADV, Phantom_Berserker_DeadlyBlow)]
    Phantom_Berserker_DeadlyBlow = 110021,

    [OccultCrescent(OccultCrescent.JobIDs.Ranger)]
    [CustomComboInfo(Job.ADV, Phantom_Ranger)]
    Phantom_Ranger = 110022,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo(Job.ADV, Phantom_Ranger_PhantomAim)]
    Phantom_Ranger_PhantomAim = 110023,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo(Job.ADV, Phantom_Ranger_OccultFeatherfoot)]
    Phantom_Ranger_OccultFeatherfoot = 110024,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo(Job.ADV, Phantom_Ranger_OccultFalcon)]
    Phantom_Ranger_OccultFalcon = 110025,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo(Job.ADV, Phantom_Ranger_OccultUnicorn)]
    Phantom_Ranger_OccultUnicorn = 110026,

    [OccultCrescent(OccultCrescent.JobIDs.TimeMage)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage)]
    Phantom_TimeMage = 110027,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage_OccultSlowga)]
    Phantom_TimeMage_OccultSlowga = 110028,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage_OccultSlowga)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage_OccultSlowga_Wait)]
    Phantom_TimeMage_OccultSlowga_Wait = 110075,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage_OccultComet)]
    Phantom_TimeMage_OccultComet = 110029,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage_OccultMageMasher)]
    Phantom_TimeMage_OccultMageMasher = 110030,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage_OccultDispel)]
    Phantom_TimeMage_OccultDispel = 110031,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo(Job.ADV, Phantom_TimeMage_OccultQuick)]
    Phantom_TimeMage_OccultQuick = 110032,

    [OccultCrescent(OccultCrescent.JobIDs.Chemist)]
    [CustomComboInfo(Job.ADV, Phantom_Chemist)]
    Phantom_Chemist = 110033,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo(Job.ADV, Phantom_Chemist_OccultPotion)]
    Phantom_Chemist_OccultPotion = 110034,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo(Job.ADV, Phantom_Chemist_OccultEther)]
    Phantom_Chemist_OccultEther = 110035,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo(Job.ADV, Phantom_Chemist_Revive)]
    Phantom_Chemist_Revive = 110036,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo(Job.ADV, Phantom_Chemist_OccultElixir)]
    Phantom_Chemist_OccultElixir = 110037,

    [OccultCrescent(OccultCrescent.JobIDs.Bard)]
    [CustomComboInfo(Job.ADV, Phantom_Bard)]
    Phantom_Bard = 110038,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo(Job.ADV, Phantom_Bard_MightyMarch)]
    Phantom_Bard_MightyMarch = 110039,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo(Job.ADV, Phantom_Bard_OffensiveAria)]
    Phantom_Bard_OffensiveAria = 110040,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo(Job.ADV, Phantom_Bard_RomeosBallad)]
    Phantom_Bard_RomeosBallad = 110041,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo(Job.ADV, Phantom_Bard_HerosRime)]
    Phantom_Bard_HerosRime = 110042,

    [OccultCrescent(OccultCrescent.JobIDs.Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle)]
    Phantom_Oracle = 110043,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_Predict)]
    Phantom_Oracle_Predict = 110044,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_PhantomJudgment)]
    Phantom_Oracle_PhantomJudgment = 110045,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_Cleansing)]
    Phantom_Oracle_Cleansing = 110046,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_Blessing)]
    Phantom_Oracle_Blessing = 110047,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_Starfall)]
    Phantom_Oracle_Starfall = 110048,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_Recuperation)]
    Phantom_Oracle_Recuperation = 110049,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_PhantomDoom)]
    Phantom_Oracle_PhantomDoom = 110050,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_PhantomRejuvenation)]
    Phantom_Oracle_PhantomRejuvenation = 110051,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo(Job.ADV, Phantom_Oracle_Invulnerability)]
    Phantom_Oracle_Invulnerability = 110052,

    [OccultCrescent(OccultCrescent.JobIDs.Cannoneer)]
    [CustomComboInfo(Job.ADV, Phantom_Cannoneer)]
    Phantom_Cannoneer = 110058,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo(Job.ADV, Phantom_Cannoneer_PhantomFire)]
    Phantom_Cannoneer_PhantomFire = 110059,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo(Job.ADV, Phantom_Cannoneer_HolyCannon)]
    Phantom_Cannoneer_HolyCannon = 110060,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo(Job.ADV, Phantom_Cannoneer_DarkCannon)]
    Phantom_Cannoneer_DarkCannon = 110061,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo(Job.ADV, Phantom_Cannoneer_ShockCannon)]
    Phantom_Cannoneer_ShockCannon = 110062,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo(Job.ADV, Phantom_Cannoneer_SilverCannon)]
    Phantom_Cannoneer_SilverCannon = 110063,

    [OccultCrescent(OccultCrescent.JobIDs.Geomancer)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer)]
    Phantom_Geomancer = 110064,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_BattleBell)]
    Phantom_Geomancer_BattleBell = 110065,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_RingingRespite)]
    Phantom_Geomancer_RingingRespite = 110073,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_Suspend)]
    Phantom_Geomancer_Suspend = 110074,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_Weather)]
    Phantom_Geomancer_Weather = 110066,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_Sunbath)]
    Phantom_Geomancer_Sunbath = 110067,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_CloudyCaress)]
    Phantom_Geomancer_CloudyCaress = 110068,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_BlessedRain)]
    Phantom_Geomancer_BlessedRain = 110069,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_MistyMirage)]
    Phantom_Geomancer_MistyMirage = 110070,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_HastyMirage)]
    Phantom_Geomancer_HastyMirage = 110071,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo(Job.ADV, Phantom_Geomancer_AetherialGain)]
    Phantom_Geomancer_AetherialGain = 110072,

    [OccultCrescent(OccultCrescent.JobIDs.Dancer)]
    [CustomComboInfo(Job.ADV, Phantom_Dancer)]
    Phantom_Dancer = 110076,

    [OccultCrescent]
    [ParentCombo(Phantom_Dancer)]
    [CustomComboInfo(Job.ADV, Phantom_Dancer_Dance)]
    Phantom_Dancer_Dance = 110077,

    [OccultCrescent]
    [ParentCombo(Phantom_Dancer)]
    [CustomComboInfo(Job.ADV, Phantom_Dancer_QuickStep)]
    Phantom_Dancer_QuickStep = 110078,

    [OccultCrescent]
    [ParentCombo(Phantom_Dancer)]
    [CustomComboInfo(Job.ADV, Phantom_Dancer_Mesmerize)]
    Phantom_Dancer_Mesmerize = 110079,

    [OccultCrescent(OccultCrescent.JobIDs.MysticKnight)]
    [CustomComboInfo(Job.ADV, Phantom_MysticKnight)]
    Phantom_MysticKnight = 110080,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [CustomComboInfo(Job.ADV, Phantom_MysticKnight_SunderingSpellblade)]
    Phantom_MysticKnight_SunderingSpellblade = 110081,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [CustomComboInfo(Job.ADV, Phantom_MysticKnight_HolySpellblade)]
    Phantom_MysticKnight_HolySpellblade = 110082,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [CustomComboInfo(Job.ADV, Phantom_MysticKnight_BlazingSpellblade)]
    Phantom_MysticKnight_BlazingSpellblade = 110083,

    [OccultCrescent]
    [ParentCombo(Phantom_MysticKnight)]
    [CustomComboInfo(Job.ADV, Phantom_MysticKnight_MagicShell)]
    Phantom_MysticKnight_MagicShell = 110084,

    [OccultCrescent(OccultCrescent.JobIDs.Gladiator)]
    [CustomComboInfo(Job.ADV, Phantom_Gladiator)]
    Phantom_Gladiator = 110085,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [CustomComboInfo(Job.ADV, Phantom_Gladiator_Finisher)]
    Phantom_Gladiator_Finisher = 110086,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [CustomComboInfo(Job.ADV, Phantom_Gladiator_Defend)]
    Phantom_Gladiator_Defend = 110087,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [CustomComboInfo(Job.ADV, Phantom_Gladiator_LongReach)]
    Phantom_Gladiator_LongReach = 110088,

    [OccultCrescent]
    [ParentCombo(Phantom_Gladiator)]
    [CustomComboInfo(Job.ADV, Phantom_Gladiator_BladeBlitz)]
    Phantom_Gladiator_BladeBlitz = 110089,

    //Last Value = 110089
    #endregion

    // Jobs

    #region ASTROLOGIAN

    #region Simple Modes
    [AutoAction(false, false)]
    [ReplaceSkill(AST.Malefic, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic)]
    [ConflictingCombos(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_Simple_DPS)]
    [SimpleCombo]
    AST_ST_Simple_DPS = 1179,

    [AutoAction(true, false)]
    [ReplaceSkill(AST.Gravity, AST.Gravity2)]
    [ConflictingCombos(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_Simple_DPS)]
    [SimpleCombo]
    AST_AOE_Simple_DPS = 1180,

    [AutoAction(false, true)]
    [ReplaceSkill(AST.Benefic)]
    [ConflictingCombos(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_Simple_ST_Heals)]
    [SimpleCombo]
    [PossiblyRetargeted]
    AST_Simple_ST_Heals = 1196,


    [AutoAction(true, true)]
    [ReplaceSkill(AST.Helios)]
    [ConflictingCombos(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_Simple_AoE_Heals)]
    [SimpleCombo]
    [PossiblyRetargeted]
    AST_Simple_AoE_Heals = 1197,

    #endregion

    #region ST DPS

    [AutoAction(false, false)]
    [ReplaceSkill(AST.Malefic, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic, AST.Combust, AST.Combust2,
        AST.Combust3)]
    [ConflictingCombos(AST_ST_Simple_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_DPS)]
    [AdvancedCombo]
    AST_ST_DPS = 1004,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_DPS_Opener)]
    AST_ST_DPS_Opener = 1040,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_DPS_CombustUptime)]
    AST_ST_DPS_CombustUptime = 1018,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_DPS_Move_DoT)]
    [Retargeted]
    AST_ST_DPS_Move_DoT = 1084,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_LightSpeed)]
    AST_DPS_LightSpeed = 1020,

    [ParentCombo(AST_DPS_LightSpeed)]
    [CustomComboInfo(Job.AST, AST_DPS_LightSpeedHold)]
    AST_DPS_LightSpeedHold = 1061,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_Divination)]
    AST_DPS_Divination = 1016,

    [ParentCombo(AST_DPS_Divination)]
    [CustomComboInfo(Job.AST, AST_DPS_LightspeedBurst)]
    AST_DPS_LightspeedBurst = 1064,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_AutoDraw)]
    AST_DPS_AutoDraw = 1011,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_AutoPlay)]
    [PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled, AST.Play1, AST.Balance, AST.Spear)]
    AST_DPS_AutoPlay = 1037,

    [ParentCombo(AST_DPS_AutoPlay)]
    [CustomComboInfo(Job.AST, AST_DPS_CardPool)]
    AST_DPS_CardPool = 1055,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_LazyLord)]
    AST_DPS_LazyLord = 1014,

    [ParentCombo(AST_DPS_LazyLord)]
    [CustomComboInfo(Job.AST, AST_DPS_LordPool)]
    AST_DPS_LordPool = 1056,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_Oracle)]
    AST_DPS_Oracle = 1015,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_DPS_EarthlyStar)]
    [Retargeted(AST.EarthlyStar)]
    AST_ST_DPS_EarthlyStar = 1051,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_ST_DPS_StellarDetonation)]
    AST_ST_DPS_StellarDetonation = 1081,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo(Job.AST, AST_DPS_Lucid)]
    AST_DPS_Lucid = 1008,

    #endregion

    #region AOE DPS

    [AutoAction(true, false)]
    [ReplaceSkill(AST.Gravity, AST.Gravity2)]
    [ConflictingCombos(AST_AOE_Simple_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_DPS)]
    [AdvancedCombo]
    AST_AOE_DPS = 1041,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_DPS_DoT)]
    [Retargeted(AST.Combust, AST.Combust2, AST.Combust3)]
    AST_AOE_DPS_DoT = 1083,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_LightSpeed)]
    AST_AOE_LightSpeed = 1048,

    [ParentCombo(AST_AOE_LightSpeed)]
    [CustomComboInfo(Job.AST, AST_AOE_LightSpeedHold)]
    AST_AOE_LightSpeedHold = 1062,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_Divination)]
    AST_AOE_Divination = 1043,

    [ParentCombo(AST_AOE_Divination)]
    [CustomComboInfo(Job.AST, AST_AOE_LightspeedBurst)]
    AST_AOE_LightspeedBurst = 1063,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_AutoDraw)]
    AST_AOE_AutoDraw = 1044,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_AutoPlay)]
    [PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled, AST.Play1, AST.Balance, AST.Spear)]
    AST_AOE_AutoPlay = 1045,

    [ParentCombo(AST_AOE_AutoPlay)]
    [CustomComboInfo(Job.AST, AST_AOE_CardPool)]
    AST_AOE_CardPool = 1057,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_LazyLord)]
    AST_AOE_LazyLord = 1046,

    [ParentCombo(AST_AOE_LazyLord)]
    [CustomComboInfo(Job.AST, AST_AOE_LordPool)]
    AST_AOE_LordPool = 1058,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_Oracle)]
    AST_AOE_Oracle = 1047,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_DPS_EarthlyStar)]
    [Retargeted(AST.EarthlyStar)]
    AST_AOE_DPS_EarthlyStar = 1052,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_DPS_StellarDetonation)]
    AST_AOE_DPS_StellarDetonation = 1082,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_DPS_MacroCosmos)]
    AST_AOE_DPS_MacroCosmos = 1066,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo(Job.AST, AST_AOE_Lucid)]
    AST_AOE_Lucid = 1042,

    #endregion

    #region Healing

    [AutoAction(false, true)]
    [ReplaceSkill(AST.Benefic)]
    [ConflictingCombos(AST_Simple_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals)]
    [PossiblyRetargeted(AST.Benefic2)]
    [HealingCombo]
    AST_ST_Heals = 1023,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_Esuna)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    AST_ST_Heals_Esuna = 1039,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_AspectedBenefic)]
    [PossiblyRetargeted(AST.AspectedBenefic)]
    AST_ST_Heals_AspectedBenefic = 1027,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_EssentialDignity)]
    [PossiblyRetargeted(AST.EssentialDignity)]
    AST_ST_Heals_EssentialDignity = 1024,

    [ParentCombo(AST_ST_Heals_EssentialDignity)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_EssentialDignity_Emergency)]
    [PossiblyRetargeted(AST.EssentialDignity)]
    AST_ST_Heals_EssentialDignity_Emergency = 1096,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_CelestialIntersection)]
    [PossiblyRetargeted(AST.CelestialIntersection)]
    AST_ST_Heals_CelestialIntersection = 1025,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_Exaltation)]
    [PossiblyRetargeted(AST.Exaltation)]
    AST_ST_Heals_Exaltation = 1028,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_Spire)]
    [PossiblyRetargeted(AST.Spire)]
    AST_ST_Heals_Spire = 1030,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_Ewer)]
    [PossiblyRetargeted(AST.Ewer)]
    AST_ST_Heals_Ewer = 1032,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_Arrow)]
    [PossiblyRetargeted(AST.Arrow)]
    AST_ST_Heals_Arrow = 1049,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_Bole)]
    [PossiblyRetargeted(AST.Bole)]
    AST_ST_Heals_Bole = 1050,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_CelestialOpposition)]
    AST_ST_Heals_CelestialOpposition = 1068,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_CollectiveUnconscious)]
    AST_ST_Heals_CollectiveUnconscious = 1069,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_SoloLady)]
    AST_ST_Heals_SoloLady = 1070,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo(Job.AST, AST_ST_Heals_NeutralSect)]
    AST_ST_Heals_NeutralSect = 1097,

    [AutoAction(true, true)]
    [ReplaceSkill(AST.Helios, AST.AspectedHelios, AST.HeliosConjuction)]
    [ConflictingCombos(AST_Simple_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals)]
    AST_AoE_Heals = 1010,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_Aspected)]
    AST_AoE_Heals_Aspected = 1053,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_Helios)]
    AST_AoE_Heals_Helios = 1073,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_CelestialOpposition)]
    AST_AoE_Heals_CelestialOpposition = 1021,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_LazyLady)]
    AST_AoE_Heals_LazyLady = 1022,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_Horoscope)]
    AST_AoE_Heals_Horoscope = 1026,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_HoroscopeHeal)]
    AST_AoE_Heals_HoroscopeHeal = 1071,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_NeutralSect)]
    AST_AoE_Heals_NeutralSect = 1067,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_StellarDetonation)]
    AST_AoE_Heals_StellarDetonation = 1072,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo(Job.AST, AST_AoE_Heals_CollectiveUnconscious)]
    AST_AoE_Heals_CollectiveUnconscious = 1074,
    #endregion

    #region Cards
    [CustomComboInfo(Job.AST, AST_Cards_QuickTargetCards)]
    [Retargeted(AST.Play1, AST.Balance, AST.Spear)]
    AST_Cards_QuickTargetCards = 1029,
    #endregion

    #region Standalones
    [ReplaceSkill(AST.Benefic2)]
    [CustomComboInfo(Job.AST, AST_Benefic)]
    [PossiblyRetargeted("Retargeting Features below, Enable Cure", Condition.ASTRetargetingFeaturesEnabledForBenefic)]
    AST_Benefic = 1002,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo(Job.AST, AST_Raise_Alternative)]
    AST_Raise_Alternative = 1003,

    [ParentCombo(AST_Raise_Alternative)]
    [CustomComboInfo(Job.AST, AST_Raise_Alternative_Retarget)]
    [Retargeted(AST.Ascend)]
    AST_Raise_Alternative_Retarget = 1060,

    [ReplaceSkill(AST.Lightspeed)]
    [CustomComboInfo(Job.AST, AST_Lightspeed_Protection)]
    AST_Lightspeed_Protection = 1065,

    [ReplaceSkill(AST.Exaltation)]
    [CustomComboInfo(Job.AST, AST_Mit_ST)]
    [PossiblyRetargeted("Retargeting Features below, Enable Exaltation (and optionally Essential Dignity and Celestial Intersection", Condition.ASTRetargetingFeaturesEnabledForSTMit)]
    AST_Mit_ST = 1094,

    [ReplaceSkill(AST.CollectiveUnconscious)]
    [CustomComboInfo(Job.AST, AST_Mit_AoE)]
    AST_Mit_AoE = 1095,
    #endregion

    #region Raidwide Features
    [CustomComboInfo(Job.AST, AST_Raidwide)]
    AST_Raidwide = 1075,

    [ParentCombo(AST_Raidwide)]
    [CustomComboInfo(Job.AST, AST_Raidwide_CollectiveUnconscious)]
    AST_Raidwide_CollectiveUnconscious = 1076,

    [ParentCombo(AST_Raidwide)]
    [CustomComboInfo(Job.AST, AST_Raidwide_NeutralSect)]
    AST_Raidwide_NeutralSect = 1077,

    [ParentCombo(AST_Raidwide)]
    [CustomComboInfo(Job.AST, AST_Raidwide_AspectedHelios)]
    AST_Raidwide_AspectedHelios = 1078,

    #endregion

    #region Retargeting
    [CustomComboInfo(Job.AST, AST_Retargets)]
    AST_Retargets = 1085,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Benefic, AST.Benefic2)]
    [CustomComboInfo(Job.AST, AST_Retargets_Benefic)]
    [Retargeted(AST.Benefic, AST.Benefic2)]
    AST_Retargets_Benefic = 1086,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.AspectedBenefic)]
    [CustomComboInfo(Job.AST, AST_Retargets_AspectedBenefic)]
    [Retargeted(AST.AspectedBenefic)]
    AST_Retargets_AspectedBenefic = 1087,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.EssentialDignity)]
    [CustomComboInfo(Job.AST, AST_Retargets_EssentialDignity)]
    [Retargeted(AST.EssentialDignity)]
    AST_Retargets_EssentialDignity = 1059,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Exaltation)]
    [CustomComboInfo(Job.AST, AST_Retargets_Exaltation)]
    [Retargeted(AST.Exaltation)]
    AST_Retargets_Exaltation = 1089,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Synastry)]
    [CustomComboInfo(Job.AST, AST_Retargets_Synastry)]
    [Retargeted(AST.Synastry)]
    AST_Retargets_Synastry = 1090,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.CelestialIntersection)]
    [CustomComboInfo(Job.AST, AST_Retargets_CelestialIntersection)]
    [Retargeted(AST.CelestialIntersection)]
    AST_Retargets_CelestialIntersection = 1091,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.Play2, AST.Play3)]
    [CustomComboInfo(Job.AST, AST_Retargets_HealCards)]
    [Retargeted(AST.Play2, AST.Play3)]
    AST_Retargets_HealCards = 1092,

    [ParentCombo(AST_Retargets)]
    [ReplaceSkill(AST.EarthlyStar)]
    [CustomComboInfo(Job.AST, AST_Retargets_EarthlyStar)]
    [Retargeted(AST.EarthlyStar)]
    AST_Retargets_EarthlyStar = 1093,
    #endregion

    // Last value = 1097

    #endregion

    #region BLACK MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_ST_AdvancedMode, BLM_Fire1and3, BLM_F1toF4)]
    [CustomComboInfo(Job.BLM, BLM_ST_SimpleMode)]
    [SimpleCombo]
    BLM_ST_SimpleMode = 2001,

    [AutoAction(true, false)]
    [ReplaceSkill(BLM.Blizzard2, BLM.HighBlizzard2)]
    [ConflictingCombos(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_SimpleMode)]
    [SimpleCombo]
    BLM_AoE_SimpleMode = 2002,

    #endregion

    #region Single Target - Advanced

    [AutoAction(false, false)]
    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_ST_SimpleMode, BLM_Fire1and3, BLM_F1toF4)]
    [CustomComboInfo(Job.BLM, BLM_ST_AdvancedMode)]
    [AdvancedCombo]
    BLM_ST_AdvancedMode = 2100,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Opener)]
    BLM_ST_Opener = 2101,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Transpose)]
    BLM_ST_Transpose = 2114,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_LeyLines)]
    BLM_ST_LeyLines = 2103,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Amplifier)]
    BLM_ST_Amplifier = 2102,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Manafont)]
    BLM_ST_Manafont = 2108,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Thunder)]
    BLM_ST_Thunder = 2110,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Despair)]
    BLM_ST_Despair = 2111,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_FlareStar)]
    BLM_ST_FlareStar = 2112,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Swiftcast)]
    BLM_ST_Swiftcast = 2106,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Triplecast)]
    BLM_ST_Triplecast = 2115,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_UsePolyglot)]
    BLM_ST_UsePolyglot = 2104,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Movement)]
    BLM_ST_Movement = 2113,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Manaward)]
    BLM_ST_Manaward = 2199,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_ST_Addle)]
    BLM_ST_Addle = 2195,

    #endregion

    #region AoE - Advanced

    [AutoAction(true, false)]
    [ReplaceSkill(BLM.Blizzard2, BLM.HighBlizzard2)]
    [ConflictingCombos(BLM_AoE_SimpleMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_AdvancedMode)]
    [AdvancedCombo]
    BLM_AoE_AdvancedMode = 2200,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Transpose)]
    BLM_AoE_Transpose = 2212,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_LeyLines)]
    BLM_AoE_LeyLines = 2202,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Amplifier)]
    BLM_AoE_Amplifier = 2201,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Manafont)]
    BLM_AoE_Manafont = 2207,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Triplecast)]
    BLM_AoE_Triplecast = 2208,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_ParadoxFiller)]
    BLM_AoE_ParadoxFiller = 2210,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Thunder)]
    BLM_AoE_Thunder = 2209,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Movement)]
    BLM_AoE_Movement = 2213,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_UsePolyglot)]
    BLM_AoE_UsePolyglot = 2203,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.BLM, BLM_AoE_Blizzard4Sub)]
    BLM_AoE_Blizzard4Sub = 2211,

    #endregion

    #region Movement

    [ConflictingCombos(BLM_Aetherial_Manipulation)]
    [CustomComboInfo(Job.BLM, BLM_Retargetting_Aetherial_Manipulation)]
    [Retargeted(BLM.AetherialManipulation)]
    BLM_Retargetting_Aetherial_Manipulation = 2066,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(BLM.Triplecast)]
    [CustomComboInfo(Job.BLM, BLM_TriplecastProtection)]
    BLM_TriplecastProtection = 2056,

    [ReplaceSkill(BLM.Fire, BLM.Fire3)]
    [ConflictingCombos(BLM_ST_AdvancedMode, BLM_ST_SimpleMode, BLM_F1toF4)]
    [CustomComboInfo(Job.BLM, BLM_Fire1and3)]
    BLM_Fire1and3 = 2054,

    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_ST_AdvancedMode, BLM_ST_SimpleMode, BLM_Fire1and3)]
    [CustomComboInfo(Job.BLM, BLM_F1toF4)]
    BLM_F1toF4 = 2070,

    [ReplaceSkill(BLM.Fire4)]
    [CustomComboInfo(Job.BLM, BLM_Fire4)]
    BLM_Fire4 = 2059,

    [ReplaceSkill(BLM.Flare)]
    [CustomComboInfo(Job.BLM, BLM_Flare)]
    BLM_Flare = 2069,

    [ReplaceSkill(BLM.Blizzard, BLM.Blizzard3)]
    [ConflictingCombos(BLM_B1toB4)]
    [CustomComboInfo(Job.BLM, BLM_Blizzard1and3)]
    BLM_Blizzard1and3 = 2052,

    [ReplaceSkill(BLM.Blizzard)]
    [ConflictingCombos(BLM_Blizzard1and3)]
    [CustomComboInfo(Job.BLM, BLM_B1toB4)]
    BLM_B1toB4 = 2071,

    [ReplaceSkill(BLM.Blizzard4)]
    [CustomComboInfo(Job.BLM, BLM_Blizzard4toDespair)]
    BLM_Blizzard4toDespair = 2060,

    [ReplaceSkill(BLM.Freeze)]
    [CustomComboInfo(Job.BLM, BLM_Freeze)]
    BLM_Freeze = 2062,

    [ReplaceSkill(BLM.FlareStar)]
    [CustomComboInfo(Job.BLM, BLM_FlareParadox)]
    BLM_FlareParadox = 2063,

    [ReplaceSkill(BLM.Amplifier)]
    [CustomComboInfo(Job.BLM, BLM_AmplifierXeno)]
    BLM_AmplifierXeno = 2061,

    [ReplaceSkill(BLM.Xenoglossy)]
    [CustomComboInfo(Job.BLM, BLM_XenoThunder)]
    BLM_XenoThunder = 2067,

    [ReplaceSkill(BLM.Foul)]
    [CustomComboInfo(Job.BLM, BLM_FoulThunder)]
    BLM_FoulThunder = 2068,

    [ReplaceSkill(BLM.Transpose)]
    [CustomComboInfo(Job.BLM, BLM_UmbralSoul)]
    BLM_UmbralSoul = 2050,

    [ReplaceSkill(BLM.Scathe)]
    [CustomComboInfo(Job.BLM, BLM_ScatheXeno)]
    BLM_ScatheXeno = 2053,

    [ReplaceSkill(BLM.LeyLines)]
    [CustomComboInfo(Job.BLM, BLM_Between_The_LeyLines)]
    BLM_Between_The_LeyLines = 2051,

    [ReplaceSkill(BLM.AetherialManipulation)]
    [ConflictingCombos(BLM_Retargetting_Aetherial_Manipulation)]
    [CustomComboInfo(Job.BLM, BLM_Aetherial_Manipulation)]
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
    [CustomComboInfo(Job.BLU, BLU_NewMoonFluteOpener)]
    [Retargeted(BLU.FeatherRain)]
    BLU_NewMoonFluteOpener = 70021,

    [BlueInactive(BLU.BreathOfMagic, BLU.MortalFlame)]
    [ParentCombo(BLU_NewMoonFluteOpener)]
    [CustomComboInfo(Job.BLU, BLU_NewMoonFluteOpener_DoTOpener)]
    BLU_NewMoonFluteOpener_DoTOpener = 70022,

    [BlueInactive(BLU.Whistle, BLU.Tingle, BLU.MoonFlute, BLU.JKick, BLU.TripleTrident, BLU.Nightbloom,
        BLU.RoseOfDestruction, BLU.FeatherRain, BLU.Bristle, BLU.GlassDance, BLU.Surpanakha, BLU.MatraMagic,
        BLU.ShockStrike, BLU.PhantomFlurry)]
    [ReplaceSkill(BLU.MoonFlute)]
    [ConflictingCombos(BLU_NewMoonFluteOpener)]
    [CustomComboInfo(Job.BLU, BLU_Opener)]
    [Retargeted(BLU.FeatherRain)]
    BLU_Opener = 70001,

    [BlueInactive(BLU.MoonFlute, BLU.Tingle, BLU.ShockStrike, BLU.Whistle, BLU.FinalSting)]
    [ReplaceSkill(BLU.FinalSting)]
    [CustomComboInfo(Job.BLU, BLU_FinalSting)]
    [Retargeted(BLU.FeatherRain)]
    BLU_FinalSting = 70002,

    [BlueInactive(BLU.RoseOfDestruction, BLU.FeatherRain, BLU.GlassDance, BLU.JKick)]
    [ParentCombo(BLU_FinalSting)]
    [CustomComboInfo(Job.BLU, BLU_Primals)]
    [Retargeted(BLU.FeatherRain)]
    BLU_Primals = 70003,

    [BlueInactive(BLU.BasicInstinct)]
    [ParentCombo(BLU_FinalSting)]
    [CustomComboInfo(Job.BLU, BLU_SoloMode)]
    BLU_SoloMode = 70011,

    [BlueInactive(BLU.RamsVoice, BLU.Ultravibration)]
    [ReplaceSkill(BLU.Ultravibration)]
    [CustomComboInfo(Job.BLU, BLU_Ultravibrate)]
    BLU_Ultravibrate = 70005,

    [BlueInactive(BLU.HydroPull)]
    [ParentCombo(BLU_Ultravibrate)]
    [CustomComboInfo(Job.BLU, BLU_HydroPull)]
    BLU_HydroPull = 70012,

    [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance)]
    [ReplaceSkill(BLU.FeatherRain)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo)]
    [Retargeted(BLU.FeatherRain)]
    BLU_PrimalCombo = 70008,

    [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_Pool)]
    BLU_PrimalCombo_Pool = 70015,

    [BlueInactive(BLU.JKick)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_JKick)]
    BLU_PrimalCombo_JKick = 70013,

    [BlueInactive(BLU.SeaShanty)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_SeaShanty)]
    BLU_PrimalCombo_SeaShanty = 70024,

    [BlueInactive(BLU.WingedReprobation)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_WingedReprobation)]
    BLU_PrimalCombo_WingedReprobation = 70025,

    [BlueInactive(BLU.MatraMagic)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_Matra)]
    BLU_PrimalCombo_Matra = 70017,

    [BlueInactive(BLU.Surpanakha)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_Suparnakha)]
    BLU_PrimalCombo_Suparnakha = 70018,

    [BlueInactive(BLU.PhantomFlurry)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_PhantomFlurry)]
    BLU_PrimalCombo_PhantomFlurry = 70019,

    [BlueInactive(BLU.Nightbloom, BLU.Bristle)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo(Job.BLU, BLU_PrimalCombo_Nightbloom)]
    BLU_PrimalCombo_Nightbloom = 70020,

    [BlueInactive(BLU.SongOfTorment, BLU.Bristle)]
    [ReplaceSkill(BLU.SongOfTorment)]
    [CustomComboInfo(Job.BLU, BLU_BuffedSoT)]
    BLU_BuffedSoT = 70000,

    [BlueInactive(BLU.PeripheralSynthesis, BLU.MustardBomb)]
    [ReplaceSkill(BLU.PeripheralSynthesis)]
    [CustomComboInfo(Job.BLU, BLU_LightHeadedCombo)]
    BLU_LightHeadedCombo = 70010,

    [BlueInactive(BLU.PerpetualRay, BLU.SharpenedKnife)]
    [CustomComboInfo(Job.BLU, BLU_PerpetualRayStunCombo)]
    BLU_PerpetualRayStunCombo = 70014,

    [BlueInactive(BLU.SonicBoom, BLU.SharpenedKnife)]
    [CustomComboInfo(Job.BLU, BLU_MeleeCombo)]
    BLU_MeleeCombo = 70016,

    [BlueInactive(BLU.MagicHammer)]
    [ReplaceSkill(BLU.MagicHammer)]
    [CustomComboInfo(Job.BLU, BLU_Addle)]
    BLU_Addle = 70007,

    [BlueInactive(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
    [ReplaceSkill(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
    [CustomComboInfo(Job.BLU, BLU_KnightCombo)]
    BLU_KnightCombo = 70009,

    [BlueInactive(BLU.Offguard, BLU.BadBreath, BLU.Devour)]
    [ReplaceSkill(BLU.Devour, BLU.Offguard, BLU.BadBreath)]
    [CustomComboInfo(Job.BLU, BLU_DebuffCombo)]
    BLU_DebuffCombo = 70006,

    [ReplaceSkill(BLU.DeepClean)]
    [BlueInactive(BLU.PeatPelt, BLU.DeepClean)]
    [CustomComboInfo(Job.BLU, BLU_PeatClean)]
    BLU_PeatClean = 70023,

    // Last value = 70023

    #endregion

    #region BARD

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_SimpleMode)]
    [SimpleCombo]
    BRD_ST_SimpleMode = 3036,

    [AutoAction(true, false)]
    [ConflictingCombos(BRD_AoE_AdvMode)]
    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [CustomComboInfo(Job.BRD, BRD_AoE_SimpleMode)]
    [SimpleCombo]
    BRD_AoE_SimpleMode = 3035,

    #endregion

    #region Advanced Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_SimpleMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_AdvMode)]
    [AdvancedCombo]
    BRD_ST_AdvMode = 3009,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_Adv_Balance_Standard)]
    BRD_ST_Adv_Balance_Standard = 3048,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_Song)]
    BRD_Adv_Song = 3011,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_DoT)]
    BRD_Adv_DoT = 3010,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_Buffs)]
    BRD_Adv_Buffs = 3017,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_BuffsResonant)]
    BRD_Adv_BuffsResonant = 3041,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_BuffsEncore)]
    BRD_Adv_BuffsEncore = 3042,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_ApexArrow)]
    BRD_ST_ApexArrow = 3021,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_Adv_oGCD)]
    BRD_ST_Adv_oGCD = 3038,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_Pooling)]
    BRD_Adv_Pooling = 3023,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_ApexPooling)]
    BRD_Adv_ApexPooling = 3057,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_Interrupt)]
    BRD_Adv_Interrupt = 3020,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_SecondWind)]
    BRD_ST_SecondWind = 3028,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_ST_Wardens)]
    BRD_ST_Wardens = 3047,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_Troubadour)]
    BRD_Adv_Troubadour = 3069,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_Adv_NaturesMinne)]
    BRD_Adv_NaturesMinne = 3070,

    [AutoAction(true, false)]
    [ConflictingCombos(BRD_AoE_SimpleMode)]
    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [CustomComboInfo(Job.BRD, BRD_AoE_AdvMode)]
    [AdvancedCombo]
    BRD_AoE_AdvMode = 3015,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Adv_Multidot)]
    [Retargeted(BRD.Windbite, BRD.VenomousBite, BRD.IronJaws, BRD.CausticBite, BRD.Stormbite)]
    BRD_AoE_Adv_Multidot = 3065,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Adv_Songs)]
    BRD_AoE_Adv_Songs = 3016,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Adv_Buffs)]
    BRD_AoE_Adv_Buffs = 3032,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Adv_oGCD)]
    BRD_AoE_Adv_oGCD = 3037,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Pooling)]
    BRD_AoE_Pooling = 3040,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_ApexPooling)]
    BRD_AoE_ApexPooling = 3058,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Adv_Interrupt)]
    BRD_AoE_Adv_Interrupt = 3043,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_BuffsEncore)]
    BRD_AoE_BuffsEncore = 3062,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_BuffsResonant)]
    BRD_AoE_BuffsResonant = 3061,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_ApexArrow)]
    BRD_AoE_ApexArrow = 3039,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_SecondWind)]
    BRD_AoE_SecondWind = 3029,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo(Job.BRD, BRD_AoE_Wardens)]
    BRD_AoE_Wardens = 3046,

    #endregion

    #region Smaller Features

    [ReplaceSkill(BRD.StraightShot, BRD.RefulgentArrow)]
    [CustomComboInfo(Job.BRD, BRD_StraightShotUpgrade)]
    BRD_StraightShotUpgrade = 3001,

    [ParentCombo(BRD_StraightShotUpgrade)]
    [CustomComboInfo(Job.BRD, BRD_StraightShotUpgrade_OGCDs)]
    BRD_StraightShotUpgrade_OGCDs = 3002,

    [ParentCombo(BRD_StraightShotUpgrade)]
    [CustomComboInfo(Job.BRD, BRD_DoTMaintainance)]
    BRD_DoTMaintainance = 3067,

    [ReplaceSkill(BRD.IronJaws)]
    [CustomComboInfo(Job.BRD, BRD_IronJaws)]
    BRD_IronJaws = 3003,

    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [CustomComboInfo(Job.BRD, BRD_WideVolleyUpgrade)]
    BRD_WideVolleyUpgrade = 3008,

    [ParentCombo(BRD_WideVolleyUpgrade)]
    [CustomComboInfo(Job.BRD, BRD_WideVolleyUpgrade_OGCDs)]
    BRD_WideVolleyUpgrade_OGCDs = 3068,

    [ParentCombo(BRD_WideVolleyUpgrade)]
    [CustomComboInfo(Job.BRD, BRD_WideVolleyUpgrade_Apex)]
    BRD_WideVolleyUpgrade_Apex = 3005,

    [ReplaceSkill(BRD.Bloodletter)]
    [CustomComboInfo(Job.BRD, BRD_ST_oGCD)]
    BRD_ST_oGCD = 3006,

    [ParentCombo(BRD_ST_oGCD)]
    [CustomComboInfo(Job.BRD, BRD_ST_oGCD_Songs)]
    BRD_ST_oGCD_Songs = 3044,

    [ReplaceSkill(BRD.RainOfDeath)]
    [CustomComboInfo(Job.BRD, BRD_AoE_oGCD)]
    BRD_AoE_oGCD = 3007,

    [ParentCombo(BRD_AoE_oGCD)]
    [CustomComboInfo(Job.BRD, BRD_AoE_oGCD_Songs)]
    BRD_AoE_oGCD_Songs = 3045,

    [ReplaceSkill(BRD.Barrage)]
    [CustomComboInfo(Job.BRD, BRD_Buffs)]
    BRD_Buffs = 3013,

    [ReplaceSkill(BRD.WanderersMinuet)]
    [CustomComboInfo(Job.BRD, BRD_OneButtonSongs)]
    BRD_OneButtonSongs = 3014,

    [ReplaceSkill(BRD.VenomousBite, BRD.CausticBite)]
    [CustomComboInfo(Job.BRD, BRD_OneButtonDots)]
    BRD_OneButtonDots = 3004,

    [ParentCombo(BRD_OneButtonDots)]
    [CustomComboInfo(Job.BRD, BRD_OneButtonDots_IronJaws)]
    BRD_OneButtonDots_IronJaws = 3071,

    [Retargeted]
    [ParentCombo(BRD_OneButtonDots)]
    [CustomComboInfo(Job.BRD, BRD_OneButtonDots_Retargeted)]
    BRD_OneButtonDots_Retargeted = 3072,

    [ParentCombo(BRD_OneButtonDots_Retargeted)]
    [CustomComboInfo(Job.BRD, BRD_OneButtonDots_SavageBlade)]
    BRD_OneButtonDots_SavageBlade = 3073,

    #endregion
    #region Hidden
    [CustomComboInfo(Job.BRD, BRD_Hidden_Song_Extension)]
    [Hidden]
    BRD_Hidden_Song_Extension = 3074,
    #endregion

    // Last value = 3074

    #endregion

    #region DANCER

    [ReplaceSkill(DNC.StandardFinish2, DNC.TechnicalFinish4)]
    [CustomComboInfo(Job.DNC, DNC_ST_BlockFinishes)]
    DNC_ST_BlockFinishes = 4000,

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_MultiButton, DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_SimpleMode)]
    [SimpleCombo]
    DNC_ST_SimpleMode = 4001,

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_SimpleMode)]
    [SimpleCombo]
    DNC_AoE_SimpleMode = 4002,

    #endregion
    // Last value = 4002

    #region Advanced Dancer (Single Target)

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_MultiButton, DNC_ST_SimpleMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_AdvancedMode)]
    [AdvancedCombo]
    DNC_ST_AdvancedMode = 4010,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_BalanceOpener)]
    DNC_ST_BalanceOpener = 4011,

    [ParentCombo(DNC_ST_BalanceOpener)]
    [CustomComboInfo(Job.DNC, DNC_ST_Opener_BlockEarly)]
    DNC_ST_Opener_BlockEarly = 4031,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Partner)]
    DNC_ST_Adv_Partner = 4012,

    [ParentCombo(DNC_ST_Adv_Partner)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_PartnerAuto)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_ST_Adv_PartnerAuto = 4033,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_AutoPartner)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_ST_Adv_AutoPartner = 4032,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Peloton)]
    DNC_ST_Adv_Peloton = 4013,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Interrupt)]
    DNC_ST_Adv_Interrupt = 4014,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_SS)]
    DNC_ST_Adv_SS = 4015,

    [ParentCombo(DNC_ST_Adv_SS)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_FM)]
    DNC_ST_Adv_FM = 4016,

    [ParentCombo(DNC_ST_Adv_SS)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_SS_Prepull)]
    DNC_ST_Adv_SS_Prepull = 4017,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_TS)]
    DNC_ST_Adv_TS = 4018,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Devilment)]
    DNC_ST_Adv_Devilment = 4019,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Flourish)]
    DNC_ST_Adv_Flourish = 4020,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_FanProccs)]
    DNC_ST_Adv_FanProccs = 4028,

    [ParentCombo(DNC_ST_Adv_FanProccs)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_FanProcc3)]
    DNC_ST_Adv_FanProcc3 = 4029,

    [ParentCombo(DNC_ST_Adv_FanProccs)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_FanProcc4)]
    DNC_ST_Adv_FanProcc4 = 4030,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Feathers)]
    DNC_ST_Adv_Feathers = 4021,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Improvisation)]
    DNC_ST_Adv_Improvisation = 4022,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_Tillana)]
    DNC_ST_Adv_Tillana = 4023,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_SaberDance)]
    DNC_ST_Adv_SaberDance = 4024,

    [ParentCombo(DNC_ST_Adv_SaberDance)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_DawnDance)]
    DNC_ST_Adv_DawnDance = 4025,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_LD)]
    DNC_ST_Adv_LD = 4026,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_PanicHeals)]
    DNC_ST_Adv_PanicHeals = 4027,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_Adv_ShieldSamba)]
    DNC_ST_Adv_ShieldSamba = 4034,

    #endregion
    // Last value = 4034

    #region Advanced Dancer (AoE)

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_SimpleMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_AdvancedMode)]
    [AdvancedCombo]
    DNC_AoE_AdvancedMode = 4040,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Partner)]
    DNC_AoE_Adv_Partner = 4041,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Interrupt)]
    DNC_AoE_Adv_Interrupt = 4042,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_SS)]
    DNC_AoE_Adv_SS = 4043,

    [ParentCombo(DNC_AoE_Adv_SS)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_FM)]
    DNC_AoE_Adv_FM = 4044,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_TS)]
    DNC_AoE_Adv_TS = 4045,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Devilment)]
    DNC_AoE_Adv_Devilment = 4046,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Flourish)]
    DNC_AoE_Adv_Flourish = 4047,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_FanProccs)]
    DNC_AoE_Adv_FanProccs = 4055,

    [ParentCombo(DNC_AoE_Adv_FanProccs)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_FanProcc3)]
    DNC_AoE_Adv_FanProcc3 = 4056,

    [ParentCombo(DNC_AoE_Adv_FanProccs)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_FanProcc4)]
    DNC_AoE_Adv_FanProcc4 = 4057,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Feathers)]
    DNC_AoE_Adv_Feathers = 4048,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Improvisation)]
    DNC_AoE_Adv_Improvisation = 4049,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_Tillana)]
    DNC_AoE_Adv_Tillana = 4050,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_SaberDance)]
    DNC_AoE_Adv_SaberDance = 4051,

    [ParentCombo(DNC_AoE_Adv_SaberDance)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_DawnDance)]
    DNC_AoE_Adv_DawnDance = 4052,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_LD)]
    DNC_AoE_Adv_LD = 4053,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_Adv_PanicHeals)]
    DNC_AoE_Adv_PanicHeals = 4054,

    #endregion
    // Last value = 4057

    #region Basic combo

    [ReplaceSkill(DNC.Fountain)]
    [CustomComboInfo(Job.DNC, DNC_ST_BasicCombo)]
    [BasicCombo]
    DNC_ST_BasicCombo = 4003,

    #endregion
    // Last value = 4003

    #region Multibutton Features

    #region Single Target Multibutton

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_AdvancedMode, DNC_ST_SimpleMode)]
    [CustomComboInfo(Job.DNC, DNC_ST_MultiButton)]
    DNC_ST_MultiButton = 4070,

    [ParentCombo(DNC_ST_MultiButton)]
    [CustomComboInfo(Job.DNC, DNC_ST_EspritOvercap)]
    DNC_ST_EspritOvercap = 4071,

    [ParentCombo(DNC_ST_MultiButton)]
    [CustomComboInfo(Job.DNC, DNC_ST_FanDanceOvercap)]
    DNC_ST_FanDanceOvercap = 4072,

    [ParentCombo(DNC_ST_MultiButton)]
    [CustomComboInfo(Job.DNC, DNC_ST_FanDance34)]
    DNC_ST_FanDance34 = 4073,

    #endregion
    // Last value = 4073

    #region AoE Multibutton

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_AdvancedMode, DNC_AoE_SimpleMode)]
    [CustomComboInfo(Job.DNC, DNC_AoE_MultiButton)]
    DNC_AoE_MultiButton = 4090,

    [ParentCombo(DNC_AoE_MultiButton)]
    [CustomComboInfo(Job.DNC, DNC_AoE_EspritOvercap)]
    DNC_AoE_EspritOvercap = 4091,

    [ParentCombo(DNC_AoE_MultiButton)]
    [CustomComboInfo(Job.DNC, DNC_AoE_FanDanceOvercap)]
    DNC_AoE_FanDanceOvercap = 4092,

    [ParentCombo(DNC_AoE_MultiButton)]
    [CustomComboInfo(Job.DNC, DNC_AoE_FanDance34)]
    DNC_AoE_FanDance34 = 4093,

    #endregion
    // Last value = 4093

    #region Smaller Features

    #region Dance Partner Features

    [ReplaceSkill(DNC.ClosedPosition, DNC.Ending)]
    [CustomComboInfo(Job.DNC, DNC_DesirablePartner)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_DesirablePartner = 4175,

    #endregion
    // Last value = 4176

    #region Dance Features

    [CustomComboInfo(Job.DNC, DNC_CustomDanceSteps)]
    DNC_CustomDanceSteps = 4115,

    [ParentCombo(DNC_CustomDanceSteps)]
    [CustomComboInfo(Job.DNC, DNC_CustomDanceSteps_Conflicts)]
    DNC_CustomDanceSteps_Conflicts = 4116,

    [CustomComboInfo(Job.DNC, DNC_DanceFeatures)]
    DNC_DanceFeatures = 4111,

    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.StandardStep)]
    [CustomComboInfo(Job.DNC, DNC_StandardStepCombo)]
    DNC_StandardStepCombo = 4110,

    // StandardStep(or Finishing Move) --> Last Dance
    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.StandardStep, DNC.FinishingMove)]
    [CustomComboInfo(Job.DNC, DNC_StandardStep_LastDance)]
    DNC_StandardStep_LastDance = 4155,

    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.TechnicalStep)]
    [CustomComboInfo(Job.DNC, DNC_TechnicalStepCombo)]
    DNC_TechnicalStepCombo = 4112,

    // Technical Step --> Devilment
    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.TechnicalStep)]
    [CustomComboInfo(Job.DNC, DNC_TechnicalStep_Devilment)]
    DNC_TechnicalStep_Devilment = 4160,

    #endregion
    // Last value = 4116

    #region Fan Features

    [ReplaceSkill(DNC.Flourish)]
    [CustomComboInfo(Job.DNC, DNC_FlourishingFanDances)]
    DNC_FlourishingFanDances = 4130,

    [ParentCombo(DNC_FlourishingFanDances)]
    [CustomComboInfo(Job.DNC, DNC_Flourishing_FD3)]
    DNC_Flourishing_FD3 = 4131,

    [CustomComboInfo(Job.DNC, DNC_FanDanceCombos)]
    DNC_FanDanceCombos = 4135,

    [ReplaceSkill(DNC.FanDance1)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo(Job.DNC, DNC_FanDance_1to3_Combo)]
    DNC_FanDance_1to3_Combo = 4136,

    [ReplaceSkill(DNC.FanDance1)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo(Job.DNC, DNC_FanDance_1to4_Combo)]
    DNC_FanDance_1to4_Combo = 4137,

    [ReplaceSkill(DNC.FanDance2)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo(Job.DNC, DNC_FanDance_2to3_Combo)]
    DNC_FanDance_2to3_Combo = 4138,

    [ReplaceSkill(DNC.FanDance2)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo(Job.DNC, DNC_FanDance_2to4_Combo)]
    DNC_FanDance_2to4_Combo = 4139,

    #endregion
    // Last value = 4139

    // Bladeshower --> Bloodshower
    [ReplaceSkill(DNC.Bladeshower)]
    [CustomComboInfo(Job.DNC, DNC_Procc_Bladeshower)]
    DNC_Procc_Bladeshower = 4165,

    // Windmill --> Rising Windmill
    [ReplaceSkill(DNC.Windmill)]
    [CustomComboInfo(Job.DNC, DNC_Procc_Windmill)]
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
    [CustomComboInfo(Job.DRK, DRK_ST_Simple)]
    [SimpleCombo]
    DRK_ST_Simple = 5001,

    [AutoAction(true, false)]
    [ReplaceSkill(DRK.Unleash)]
    [ConflictingCombos(DRK_AoE_Adv)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Simple)]
    [SimpleCombo]
    DRK_AoE_Simple = 5002,

    #endregion
    // Last value = 5003

    #region Advanced Single Target Combo

    [AutoAction(false, false)]
    [ReplaceSkill(DRK.HardSlash)]
    [ConflictingCombos(DRK_ST_Simple)]
    [CustomComboInfo(Job.DRK, DRK_ST_Adv)]
    [AdvancedCombo]
    DRK_ST_Adv = 5010,

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo(Job.DRK, DRK_ST_BalanceOpener)]
    DRK_ST_BalanceOpener = 5011,

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo(Job.DRK, DRK_ST_RangedUptime)]
    DRK_ST_RangedUptime = 5012,

    #region Cooldowns

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo(Job.DRK, DRK_ST_CDs)]
    DRK_ST_CDs = 5013,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Interrupt)]
    DRK_ST_CD_Interrupt = 5014,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Stun)]
    DRK_ST_CD_Stun = 5040,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Delirium)]
    DRK_ST_CD_Delirium = 5015,

    #region Living Shadow Options

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Shadow)]
    DRK_ST_CD_Shadow = 5016,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Disesteem)]
    DRK_ST_CD_Disesteem = 5017,

    #endregion

    #region Shadowbringer Options

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Bringer)]
    DRK_ST_CD_Bringer = 5018,

    [ParentCombo(DRK_ST_CD_Bringer)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_BringerBurst)]
    DRK_ST_CD_BringerBurst = 5019,

    #endregion

    #region Salt Options

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Salt)]
    DRK_ST_CD_Salt = 5020,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Darkness)]
    DRK_ST_CD_Darkness = 5021,

    #endregion

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo(Job.DRK, DRK_ST_CD_Spit)]
    DRK_ST_CD_Spit = 5022,

    #endregion

    #region Spenders

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo(Job.DRK, DRK_ST_Spenders)]
    DRK_ST_Spenders = 5023,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_ScarletChain)]
    DRK_ST_Sp_ScarletChain = 5024,

    #region Blood

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_Bloodspiller)]
    DRK_ST_Sp_Bloodspiller = 5025,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_BloodOvercap)]
    DRK_ST_Sp_BloodOvercap = 5026,

    #endregion

    #region Mana

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_Edge)]
    DRK_ST_Sp_Edge = 5027,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_DarkArts)]
    DRK_ST_Sp_DarkArts = 5028,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_EdgeDarkside)]
    DRK_ST_Sp_EdgeDarkside = 5029,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_ST_Sp_ManaOvercap)]
    DRK_ST_Sp_ManaOvercap = 5030,

    #endregion

    #endregion

    #endregion
    // Last value = 5040

    #region Advanced Multi Target Combo

    [AutoAction(true, false)]
    [ReplaceSkill(DRK.Unleash)]
    [ConflictingCombos(DRK_AoE_Simple)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Adv)]
    [AdvancedCombo]
    DRK_AoE_Adv = 5050,

    #region Cooldowns

    [ParentCombo(DRK_AoE_Adv)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CDs)]
    DRK_AoE_CDs = 5051,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Interrupt)]
    DRK_AoE_Interrupt = 5052,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Stun)]
    DRK_AoE_Stun = 5053,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_Delirium)]
    DRK_AoE_CD_Delirium = 5054,

    #region Living Shadow Options

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_Shadow)]
    DRK_AoE_CD_Shadow = 5055,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_Disesteem)]
    DRK_AoE_CD_Disesteem = 5056,

    #endregion

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_Bringer)]
    DRK_AoE_CD_Bringer = 5057,

    #region Salt Options

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_Salt)]
    DRK_AoE_CD_Salt = 5058,

    [ParentCombo(DRK_AoE_CD_Salt)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_SaltStill)]
    DRK_AoE_CD_SaltStill = 5059,

    #endregion

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo(Job.DRK, DRK_AoE_CD_Drain)]
    DRK_AoE_CD_Drain = 5060,

    #endregion

    #region Spenders

    [ParentCombo(DRK_AoE_Adv)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Spenders)]
    DRK_AoE_Spenders = 5061,

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Sp_ImpalementChain)]
    DRK_AoE_Sp_ImpalementChain = 5062,

    #region Blood

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Sp_Quietus)]
    DRK_AoE_Sp_Quietus = 5063,

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Sp_BloodOvercap)]
    DRK_AoE_Sp_BloodOvercap = 5064,

    #endregion

    #region Mana

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Sp_Flood)]
    DRK_AoE_Sp_Flood = 5065,

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo(Job.DRK, DRK_AoE_Sp_ManaOvercap)]
    DRK_AoE_Sp_ManaOvercap = 5066,

    #endregion

    #endregion

    #endregion
    // Last value = 5075

    #region Advanced Mitigation
    [CustomComboInfo(Job.DRK, DRK_Mitigation)]
    DRK_Mitigation = 5300,

    [ParentCombo(DRK_Mitigation)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss)]
    DRK_Mitigation_NonBoss = 5301,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_BlackestNight)]
    DRK_Mitigation_NonBoss_BlackestNight = 5307,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_LivingDead)]
    DRK_Mitigation_NonBoss_LivingDead = 5305,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_Rampart)]
    DRK_Mitigation_NonBoss_Rampart = 5302,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_DarkMind)]
    DRK_Mitigation_NonBoss_DarkMind = 5304,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_ShadowWall)]
    DRK_Mitigation_NonBoss_ShadowWall = 5303,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_ArmsLength)]
    DRK_Mitigation_NonBoss_ArmsLength = 5306,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_Reprisal)]
    DRK_Mitigation_NonBoss_Reprisal = 5313,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_DarkMissionary)]
    DRK_Mitigation_NonBoss_DarkMissionary = 5308,

    [ParentCombo(DRK_Mitigation_NonBoss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_NonBoss_Oblation)]
    DRK_Mitigation_NonBoss_Oblation = 5318,

    [ParentCombo(DRK_Mitigation)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss)]
    DRK_Mitigation_Boss = 5309,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_BlackestNight_OnCD)]
    DRK_Mitigation_Boss_BlackestNight_OnCD = 5310,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_BlackestNight_TB)]
    DRK_Mitigation_Boss_BlackestNight_TB = 5314,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_Oblation)]
    DRK_Mitigation_Boss_Oblation = 5319,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_Rampart)]
    DRK_Mitigation_Boss_Rampart = 5316,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_ShadowWall)]
    DRK_Mitigation_Boss_ShadowWall = 5315,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_DarkMind)]
    DRK_Mitigation_Boss_DarkMind = 5317,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_Reprisal)]
    DRK_Mitigation_Boss_Reprisal = 5311,

    [ParentCombo(DRK_Mitigation_Boss)]
    [CustomComboInfo(Job.DRK, DRK_Mitigation_Boss_DarkMissionary)]
    DRK_Mitigation_Boss_DarkMissionary = 5312,

    #endregion
    // Lastvalue = 5319

    #region Basic combo

    [ReplaceSkill(DRK.Souleater)]
    [CustomComboInfo(Job.DRK, DRK_ST_BasicCombo)]
    [BasicCombo]
    DRK_ST_BasicCombo = 5003,

    [ReplaceSkill(DRK.StalwartSoul)]
    [CustomComboInfo(Job.DRK, DRK_AoE_BasicCombo)]
    [BasicCombo]
    DRK_AoE_BasicCombo = 5004,

    #endregion

    #region Multibutton Features

    #region One-Button Mitigation

    [ReplaceSkill(DRK.DarkMind)]
    [CustomComboInfo(Job.DRK, DRK_Mit_OneButton)]
    [MitigationCombo]
    DRK_Mit_OneButton = 5090,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_LivingDead_Max)]
    DRK_Mit_LivingDead_Max = 5091,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_TheBlackestNight)]
    DRK_Mit_TheBlackestNight = 5092,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_Oblation)]
    DRK_Mit_Oblation = 5093,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_Reprisal)]
    DRK_Mit_Reprisal = 5094,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_DarkMissionary)]
    DRK_Mit_DarkMissionary = 5095,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_Rampart)]
    DRK_Mit_Rampart = 5096,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_DarkMind)]
    DRK_Mit_DarkMind = 5097,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_ArmsLength)]
    DRK_Mit_ArmsLength = 5098,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo(Job.DRK, DRK_Mit_ShadowWall)]
    DRK_Mit_ShadowWall = 5099,

    [ReplaceSkill(DRK.DarkMissionary)]
    [CustomComboInfo(Job.DRK, DRK_Mit_Party)]
    [MitigationCombo]
    DRK_Mit_Party = 5100,

    #endregion
    // Last value = 5100

    #region oGCD Feature

    [ReplaceSkill(DRK.CarveAndSpit, DRK.AbyssalDrain)]
    [CustomComboInfo(Job.DRK, DRK_oGCD)]
    DRK_oGCD = 5120,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_Interrupt)]
    DRK_oGCD_Interrupt = 5121,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_Delirium)]
    DRK_oGCD_Delirium = 5122,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_Shadow)]
    DRK_oGCD_Shadow = 5124,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_Disesteem)]
    DRK_oGCD_Disesteem = 5125,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_SaltedEarth)]
    DRK_oGCD_SaltedEarth = 5126,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_SaltAndDarkness)]
    DRK_oGCD_SaltAndDarkness = 5127,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo(Job.DRK, DRK_oGCD_Shadowbringer)]
    DRK_oGCD_Shadowbringer = 5123,

    #endregion
    // Last value = 5123

    #region Standalones

    [ReplaceSkill(DRK.BlackestNight)]
    [CustomComboInfo(Job.DRK, DRK_Retarget_TBN)]
    [Retargeted(DRK.BlackestNight)]
    DRK_Retarget_TBN = 5130,

    [ParentCombo(DRK_Retarget_TBN)]
    [CustomComboInfo(Job.DRK, DRK_Retarget_TBN_TT)]
    [Retargeted]
    DRK_Retarget_TBN_TT = 5131,

    [ReplaceSkill(DRK.Oblation)]
    [CustomComboInfo(Job.DRK, DRK_Retarget_Oblation)]
    [Retargeted(DRK.Oblation)]
    DRK_Retarget_Oblation = 5132,

    [ParentCombo(DRK_Retarget_Oblation)]
    [CustomComboInfo(Job.DRK, DRK_Retarget_Oblation_TT)]
    [Retargeted]
    DRK_Retarget_Oblation_TT = 5133,

    [ParentCombo(DRK_Retarget_Oblation)]
    [CustomComboInfo(Job.DRK, DRK_Retarget_Oblation_DoubleProtection)]
    DRK_Retarget_Oblation_DoubleProtection = 5134,

    [ReplaceSkill(DRK.Shadowstride)]
    [CustomComboInfo(Job.DRK, DRK_RetargetShadowstride)]
    [Retargeted(DRK.Shadowstride)]
    DRK_RetargetShadowstride = 5135,

    #endregion
    // Last value = 5135

    #endregion
    // Last value = 5135

    #region Hidden Features

    [CustomComboInfo(Job.DRK, DRK_Hidden)]
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
    [CustomComboInfo(Job.DRG, DRG_ST_SimpleMode)]
    [SimpleCombo]
    DRG_ST_SimpleMode = 6001,

    [AutoAction(true, false)]
    [ReplaceSkill(DRG.DoomSpike)]
    [ConflictingCombos(DRG_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_AoE_SimpleMode)]
    [SimpleCombo]
    DRG_AoE_SimpleMode = 6200,

    #endregion

    #region Advanced ST Dragoon

    [AutoAction(false, false)]
    [ReplaceSkill(DRG.TrueThrust)]
    [ConflictingCombos(DRG_ST_SimpleMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_AdvancedMode)]
    [AdvancedCombo]
    DRG_ST_AdvancedMode = 6100,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_Opener)]
    DRG_ST_Opener = 6101,

    #region Buffs ST

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_Buffs)]
    DRG_ST_Buffs = 6102,

    [ParentCombo(DRG_ST_Buffs)]
    [CustomComboInfo(Job.DRG, DRG_ST_BattleLitany)]
    DRG_ST_BattleLitany = 6103,

    [ParentCombo(DRG_ST_Buffs)]
    [CustomComboInfo(Job.DRG, DRG_ST_LanceCharge)]
    DRG_ST_LanceCharge = 6104,

    [ParentCombo(DRG_ST_Buffs)]
    [CustomComboInfo(Job.DRG, DRG_ST_LifeSurge)]
    DRG_ST_LifeSurge = 6106,

    #endregion

    #region Damage SKills

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_Damage)]
    DRG_ST_Damage = 6105,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_HighJump)]
    DRG_ST_HighJump = 6113,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_Mirage)]
    DRG_ST_Mirage = 6115,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_Geirskogul)]
    DRG_ST_Geirskogul = 6116,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_Nastrond)]
    DRG_ST_Nastrond = 6117,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_Wyrmwind)]
    DRG_ST_Wyrmwind = 6118,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_Stardiver)]
    DRG_ST_Stardiver = 6110,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_Starcross)]
    DRG_ST_Starcross = 6112,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_DragonfireDive)]
    DRG_ST_DragonfireDive = 6107,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_RiseOfTheDragon)]
    DRG_ST_RiseOfTheDragon = 6109,

    [ParentCombo(DRG_ST_Damage)]
    [CustomComboInfo(Job.DRG, DRG_ST_RangedUptime)]
    DRG_ST_RangedUptime = 6197,

    #endregion

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_TrueNorthDynamic)]
    DRG_TrueNorthDynamic = 6199,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_StunInterupt)]
    DRG_ST_StunInterupt = 6196,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_ComboHeals)]
    DRG_ST_ComboHeals = 6198,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_ST_Feint)]
    DRG_ST_Feint = 6195,

    #endregion

    #region Advanced AoE Dragoon

    [AutoAction(true, false)]
    [ReplaceSkill(DRG.DoomSpike)]
    [ConflictingCombos(DRG_AoE_SimpleMode)]
    [CustomComboInfo(Job.DRG, DRG_AoE_AdvancedMode)]
    [AdvancedCombo]
    DRG_AoE_AdvancedMode = 6201,

    #region Buffs AoE

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Buffs)]
    DRG_AoE_Buffs = 6202,

    [ParentCombo(DRG_AoE_Buffs)]
    [CustomComboInfo(Job.DRG, DRG_AoE_BattleLitany)]
    DRG_AoE_BattleLitany = 6203,

    [ParentCombo(DRG_AoE_Buffs)]
    [CustomComboInfo(Job.DRG, DRG_AoE_LanceCharge)]
    DRG_AoE_LanceCharge = 6204,

    [ParentCombo(DRG_AoE_Buffs)]
    [CustomComboInfo(Job.DRG, DRG_AoE_LifeSurge)]
    DRG_AoE_LifeSurge = 6206,

    #endregion

    #region cooldowns AoE

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Damage)]
    DRG_AoE_Damage = 6205,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_HighJump)]
    DRG_AoE_HighJump = 6213,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Mirage)]
    DRG_AoE_Mirage = 6215,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Geirskogul)]
    DRG_AoE_Geirskogul = 6216,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Nastrond)]
    DRG_AoE_Nastrond = 6217,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Wyrmwind)]
    DRG_AoE_Wyrmwind = 6218,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Stardiver)]
    DRG_AoE_Stardiver = 6210,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Starcross)]
    DRG_AoE_Starcross = 6212,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_DragonfireDive)]
    DRG_AoE_DragonfireDive = 6207,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_RiseOfTheDragon)]
    DRG_AoE_RiseOfTheDragon = 6209,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_RangedUptime)]
    DRG_AoE_RangedUptime = 6298,

    [ParentCombo(DRG_AoE_Damage)]
    [CustomComboInfo(Job.DRG, DRG_AoE_Disembowel)]
    DRG_AoE_Disembowel = 6297,

    #endregion

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_AoE_StunInterupt)]
    DRG_AoE_StunInterupt = 6296,

    [ParentCombo(DRG_AoE_AdvancedMode)]
    [CustomComboInfo(Job.DRG, DRG_AoE_ComboHeals)]
    DRG_AoE_ComboHeals = 6299,

    #endregion

    #region Basic Combo

    [ReplaceSkill(DRG.FullThrust, DRG.HeavensThrust)]
    [CustomComboInfo(Job.DRG, DRG_HeavensThrust)]
    [BasicCombo]
    DRG_HeavensThrust = 6304,

    [ReplaceSkill(DRG.ChaosThrust, DRG.ChaoticSpring)]
    [CustomComboInfo(Job.DRG, DRG_ChaoticSpring)]
    [BasicCombo]
    DRG_ChaoticSpring = 6305,

    #endregion

    [ReplaceSkill(DRG.LanceCharge)]
    [CustomComboInfo(Job.DRG, DRG_BurstCDFeature)]
    DRG_BurstCDFeature = 6301,

    // Last value ST = 6120
    // Last value AoE = 6216

    #endregion

    #region GUNBREAKER

    #region Simple Mode
    [AutoAction(false, false)]
    [ConflictingCombos(GNB_ST_Advanced)]
    [ReplaceSkill(GNB.KeenEdge)]
    [CustomComboInfo(Job.GNB, GNB_ST_Simple)]
    [SimpleCombo]
    GNB_ST_Simple = 7001,

    [AutoAction(true, false)]
    [ConflictingCombos(GNB_AoE_Advanced)]
    [ReplaceSkill(GNB.DemonSlice)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Simple)]
    [SimpleCombo]
    GNB_AoE_Simple = 7002,
    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ConflictingCombos(GNB_ST_Simple)]
    [ReplaceSkill(GNB.KeenEdge)]
    [CustomComboInfo(Job.GNB, GNB_ST_Advanced)]
    [AdvancedCombo]
    GNB_ST_Advanced = 7003,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Opener)]
    GNB_ST_Opener = 7006,

    [ConflictingCombos(GNB_NM_Features)]
    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_NoMercy)]
    GNB_ST_NoMercy = 7008,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Zone)]
    GNB_ST_Zone = 7009,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_BurstStrike)]
    GNB_ST_BurstStrike = 7015,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_SonicBreak)]
    GNB_ST_SonicBreak = 7012,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_GnashingFang)]
    GNB_ST_GnashingFang = 7016,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_BowShock)]
    GNB_ST_BowShock = 7010,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Continuation)]
    GNB_ST_Continuation = 7005,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Bloodfest)]
    GNB_ST_Bloodfest = 7011,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_DoubleDown)]
    GNB_ST_DoubleDown = 7017,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Reign)]
    GNB_ST_Reign = 7014,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_RangedUptime)]
    GNB_ST_RangedUptime = 7004,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Interrupt)]
    GNB_ST_Interrupt = 7084,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_ST_Stun)]
    GNB_ST_Stun = 7086,

    #endregion

    #region Advanced AoE
    [AutoAction(true, false)]
    [ConflictingCombos(GNB_AoE_Simple)]
    [ReplaceSkill(GNB.DemonSlice)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Advanced)]
    [AdvancedCombo]
    GNB_AoE_Advanced = 7200,

    [ConflictingCombos(GNB_NM_Features)]
    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_NoMercy)]
    GNB_AoE_NoMercy = 7201,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Zone)]
    GNB_AoE_Zone = 7202,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_BowShock)]
    GNB_AoE_BowShock = 7203,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Bloodfest)]
    GNB_AoE_Bloodfest = 7204,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_SonicBreak)]
    GNB_AoE_SonicBreak = 7205,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_DoubleDown)]
    GNB_AoE_DoubleDown = 7206,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Reign)]
    GNB_AoE_Reign = 7207,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_FatedCircle)]
    GNB_AoE_FatedCircle = 7208,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Interrupt)]
    GNB_AoE_Interrupt = 7222,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo(Job.GNB, GNB_AoE_Stun)]
    GNB_AoE_Stun = 7223,

    #endregion

    #region Advanced Mitigation
    //7700s LastUsed 7713
    [CustomComboInfo(Job.GNB, GNB_Mitigation)]
    GNB_Mitigation = 7700,

    [ParentCombo(GNB_Mitigation)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss)]
    GNB_Mitigation_NonBoss = 7701,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_Rampart)]
    GNB_Mitigation_NonBoss_Rampart = 7702,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_Nebula)]
    GNB_Mitigation_NonBoss_Nebula = 7703,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_Camouflage)]
    GNB_Mitigation_NonBoss_Camouflage = 7704,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_Aurora)]
    GNB_Mitigation_NonBoss_Aurora = 7705,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_Superbolide)]
    GNB_Mitigation_NonBoss_Superbolide = 7706,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_SuperBolideEmergency)]
    GNB_Mitigation_NonBoss_SuperBolideEmergency = 7721,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_ArmsLength)]
    GNB_Mitigation_NonBoss_ArmsLength = 7707,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_Reprisal)]
    GNB_Mitigation_NonBoss_Reprisal = 7708,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_HeartOfLight)]
    GNB_Mitigation_NonBoss_HeartOfLight = 7709,

    [ParentCombo(GNB_Mitigation_NonBoss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_NonBoss_HeartOfStone)]
    GNB_Mitigation_NonBoss_HeartOfStone = 7710,

    [ParentCombo(GNB_Mitigation)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss)]
    GNB_Mitigation_Boss = 7711,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_Aurora)]
    GNB_Mitigation_Boss_Aurora = 7712,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_HeartOfStone_OnCD)]
    GNB_Mitigation_Boss_HeartOfStone_OnCD = 7716,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_HeartOfStone_TankBuster)]
    GNB_Mitigation_Boss_HeartOfStone_TankBuster = 7717,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_Rampart)]
    GNB_Mitigation_Boss_Rampart = 7719,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_Nebula)]
    GNB_Mitigation_Boss_Nebula = 7718,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_Camouflage)]
    GNB_Mitigation_Boss_Camouflage = 7720,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_Reprisal)]
    GNB_Mitigation_Boss_Reprisal = 7713,

    [ParentCombo(GNB_Mitigation_Boss)]
    [CustomComboInfo(Job.GNB, GNB_Mitigation_Boss_HeartOfLight)]
    GNB_Mitigation_Boss_HeartOfLight = 7714,
    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(GNB.Camouflage)]
    [CustomComboInfo(Job.GNB, GNB_Mit_OneButton)]
    [MitigationCombo]
    GNB_Mit_OneButton = 7074,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Superbolide_Max)]
    GNB_Mit_Superbolide_Max = 7075,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Corundum)]
    GNB_Mit_Corundum = 7076,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Aurora)]
    GNB_Mit_Aurora = 7077,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Camouflage)]
    GNB_Mit_Camouflage = 7078,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Reprisal)]
    GNB_Mit_Reprisal = 7079,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_HeartOfLight)]
    GNB_Mit_HeartOfLight = 7080,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Rampart)]
    GNB_Mit_Rampart = 7081,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_ArmsLength)]
    GNB_Mit_ArmsLength = 7082,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Nebula)]
    GNB_Mit_Nebula = 7083,

    [ReplaceSkill(GNB.HeartOfLight)]
    [CustomComboInfo(Job.GNB, GNB_Mit_Party)]
    [MitigationCombo]
    GNB_Mit_Party = 7085,
    #endregion

    #region Misc

    #region Basic combo

    [ReplaceSkill(GNB.SolidBarrel)]
    [CustomComboInfo(Job.GNB, GNB_ST_BasicCombo)]
    [BasicCombo]
    GNB_ST_BasicCombo = 7100,

    [ReplaceSkill(GNB.DemonSlaughter)]
    [CustomComboInfo(Job.GNB, GNB_AoE_BasicCombo)]
    [BasicCombo]
    GNB_AoE_BasicCombo = 7101,

    #endregion

    #region Gnashing Fang
    [ReplaceSkill(GNB.GnashingFang)]
    [CustomComboInfo(Job.GNB, GNB_GF_Features)]
    GNB_GF_Features = 7300,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_NoMercy)]
    GNB_GF_NoMercy = 7302,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_Zone)]
    GNB_GF_Zone = 7303,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_BurstStrike)]
    GNB_GF_BurstStrike = 7309,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_SonicBreak)]
    GNB_GF_SonicBreak = 7306,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_BowShock)]
    GNB_GF_BowShock = 7304,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_Continuation)]
    GNB_GF_Continuation = 7301,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_Bloodfest)]
    GNB_GF_Bloodfest = 7305,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_DoubleDown)]
    GNB_GF_DoubleDown = 7307,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo(Job.GNB, GNB_GF_Reign)]
    GNB_GF_Reign = 7308,
    #endregion

    #region No Mercy
    [ReplaceSkill(GNB.NoMercy)]
    [CustomComboInfo(Job.GNB, GNB_NM_Features)]
    GNB_NM_Features = 7500,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo(Job.GNB, GNB_NM_Bloodfest)]
    GNB_NM_Bloodfest = 7501,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo(Job.GNB, GNB_NM_Zone)]
    GNB_NM_Zone = 7502,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo(Job.GNB, GNB_NM_BowShock)]
    GNB_NM_BowShock = 7503,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo(Job.GNB, GNB_NM_Continuation)]
    GNB_NM_Continuation = 7504,
    #endregion

    #region Burst Strike

    [ReplaceSkill(GNB.BurstStrike)]
    [CustomComboInfo(Job.GNB, GNB_BS_Features)]
    GNB_BS_Features = 7400,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo(Job.GNB, GNB_BS_Continuation)]
    GNB_BS_Continuation = 7401,

    [ParentCombo(GNB_BS_Continuation)]
    [CustomComboInfo(Job.GNB, GNB_BS_Hypervelocity)]
    GNB_BS_Hypervelocity = 7406,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo(Job.GNB, GNB_BS_Bloodfest)]
    GNB_BS_Bloodfest = 7402,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo(Job.GNB, GNB_BS_GnashingFang)]
    GNB_BS_GnashingFang = 7405,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo(Job.GNB, GNB_BS_DoubleDown)]
    GNB_BS_DoubleDown = 7403,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo(Job.GNB, GNB_BS_Reign)]
    GNB_BS_Reign = 7404,

    #endregion

    #region Fated Circle

    [ReplaceSkill(GNB.FatedCircle)]
    [CustomComboInfo(Job.GNB, GNB_FC_Features)]
    GNB_FC_Features = 7600,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo(Job.GNB, GNB_FC_Continuation)]
    GNB_FC_Continuation = 7601,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo(Job.GNB, GNB_FC_Bloodfest)]
    GNB_FC_Bloodfest = 7602,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo(Job.GNB, GNB_FC_BowShock)]
    GNB_FC_BowShock = 7605,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo(Job.GNB, GNB_FC_DoubleDown)]
    GNB_FC_DoubleDown = 7603,

    [ParentCombo(GNB_FC_DoubleDown)]
    [CustomComboInfo(Job.GNB, GNB_FC_DoubleDown_NM)]
    GNB_FC_DoubleDown_NM = 7606,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo(Job.GNB, GNB_FC_Reign)]
    GNB_FC_Reign = 7604,

    #endregion

    #region Aurora Protection
    [ReplaceSkill(GNB.Aurora)]
    [CustomComboInfo(Job.GNB, GNB_AuroraProtection)]
    GNB_AuroraProtection = 7023,

    [ParentCombo(GNB_AuroraProtection)]
    [CustomComboInfo(Job.GNB, GNB_RetargetAurora_MO)]
    [Retargeted(GNB.Aurora)]
    GNB_RetargetAurora_MO = 7087,

    [ParentCombo(GNB_AuroraProtection)]
    [CustomComboInfo(Job.GNB, GNB_RetargetAurora_TT)]
    [Retargeted(GNB.Aurora)]
    GNB_RetargetAurora_TT = 7088,

    #endregion

    #region Heart Of Stone Retarget
    [ReplaceSkill(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    [CustomComboInfo(Job.GNB, GNB_RetargetHeartofStone)]
    [Retargeted(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    GNB_RetargetHeartofStone = 7089,

    [ParentCombo(GNB_RetargetHeartofStone)]
    [CustomComboInfo(Job.GNB, GNB_RetargetHeartofStone_TT)]
    [Retargeted(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    GNB_RetargetHeartofStone_TT = 7090,
    #endregion

    [ReplaceSkill(GNB.Trajectory)]
    [CustomComboInfo(Job.GNB, GNB_RetargetTrajectory)]
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
    [CustomComboInfo(Job.MCH, MCH_ST_SimpleMode)]
    [SimpleCombo]
    MCH_ST_SimpleMode = 8001,

    [AutoAction(true, false)]
    [ReplaceSkill(MCH.SpreadShot, MCH.Scattergun)]
    [ConflictingCombos(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_SimpleMode)]
    [SimpleCombo]
    MCH_AoE_SimpleMode = 8200,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(MCH.SplitShot, MCH.HeatedSplitShot)]
    [ConflictingCombos(MCH_ST_SimpleMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_AdvancedMode)]
    [AdvancedCombo]
    MCH_ST_AdvancedMode = 8100,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Opener)]
    MCH_ST_Adv_Opener = 8101,

    #region BS

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Stabilizer)]
    MCH_ST_Adv_Stabilizer = 8110,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Stabilizer_FullMetalField)]
    MCH_ST_Adv_Stabilizer_FullMetalField = 8111,

    #endregion

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_WildFire)]
    MCH_ST_Adv_WildFire = 8108,

    #region Hypercharge

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Hypercharge)]
    MCH_ST_Adv_Hypercharge = 8105,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Heatblast)]
    MCH_ST_Adv_Heatblast = 8106,

    #endregion

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_GaussRicochet)]
    MCH_ST_Adv_GaussRicochet = 8104,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Reassemble)]
    MCH_ST_Adv_Reassemble = 8103,

    #region Tools

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Tools)]
    MCH_ST_Adv_Tools = 8119,

    #endregion

    #region Queen

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_TurretQueen)]
    MCH_ST_Adv_TurretQueen = 8107,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_QueenOverdrive)]
    MCH_ST_Adv_QueenOverdrive = 8115,

    #endregion

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Interrupt)]
    MCH_ST_Adv_Interrupt = 8113,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_SecondWind)]
    MCH_ST_Adv_SecondWind = 8114,

    #region Raidwides

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Dismantle)]
    MCH_ST_Dismantle = 8195,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_ST_Adv_Tactician)]
    MCH_ST_Adv_Tactician = 8118,

    #endregion

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(MCH.SpreadShot, MCH.Scattergun)]
    [ConflictingCombos(MCH_AoE_SimpleMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_AdvancedMode)]
    [AdvancedCombo]
    MCH_AoE_AdvancedMode = 8300,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_FlameThrower)]
    MCH_AoE_Adv_FlameThrower = 8305,

    #region BS

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Stabilizer)]
    MCH_AoE_Adv_Stabilizer = 8307,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Stabilizer_FullMetalField)]
    MCH_AoE_Adv_Stabilizer_FullMetalField = 8308,

    #endregion

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_GaussRicochet)]
    MCH_AoE_Adv_GaussRicochet = 8302,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Hypercharge)]
    MCH_AoE_Adv_Hypercharge = 8303,

    #region Queen

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Queen)]
    MCH_AoE_Adv_Queen = 8304,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_QueenOverdrive)]
    MCH_AoE_Adv_QueenOverdrive = 8314,

    #endregion

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Reassemble)]
    MCH_AoE_Adv_Reassemble = 8301,

    #region Tools

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Tools)]
    MCH_AoE_Adv_Tools = 8315,

    #endregion

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_Interrupt)]
    MCH_AoE_Adv_Interrupt = 8311,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo(Job.MCH, MCH_AoE_Adv_SecondWind)]
    MCH_AoE_Adv_SecondWind = 8399,

    #endregion

    #region Basic combo

    [ReplaceSkill(MCH.CleanShot, MCH.HeatedCleanShot)]
    [CustomComboInfo(Job.MCH, MCH_ST_BasicCombo)]
    [BasicCombo]
    MCH_ST_BasicCombo = 8117,

    #endregion

    [ReplaceSkill(MCH.Dismantle)]
    [ConflictingCombos(MCH_DismantleTactician)]
    [CustomComboInfo(Job.MCH, MCH_DismantleProtection)]
    MCH_DismantleProtection = 8042,

    [ReplaceSkill(MCH.Dismantle)]
    [ConflictingCombos(MCH_DismantleProtection)]
    [CustomComboInfo(Job.MCH, MCH_DismantleTactician)]
    MCH_DismantleTactician = 8058,

    #region Heatblast

    [ReplaceSkill(MCH.Heatblast, MCH.BlazingShot)]
    [CustomComboInfo(Job.MCH, MCH_Heatblast)]
    MCH_Heatblast = 8006,

    [ParentCombo(MCH_Heatblast)]
    [CustomComboInfo(Job.MCH, MCH_Heatblast_AutoBarrel)]
    MCH_Heatblast_AutoBarrel = 8052,

    [ParentCombo(MCH_Heatblast)]
    [CustomComboInfo(Job.MCH, MCH_Heatblast_Wildfire)]
    MCH_Heatblast_Wildfire = 8015,

    [ParentCombo(MCH_Heatblast)]
    [CustomComboInfo(Job.MCH, MCH_Heatblast_GaussRound)]
    MCH_Heatblast_GaussRound = 8016,

    #endregion

    #region AutoCrossbow

    [ReplaceSkill(MCH.AutoCrossbow)]
    [CustomComboInfo(Job.MCH, MCH_AutoCrossbow)]
    MCH_AutoCrossbow = 8018,

    [ParentCombo(MCH_AutoCrossbow)]
    [CustomComboInfo(Job.MCH, MCH_AutoCrossbow_AutoBarrel)]
    MCH_AutoCrossbow_AutoBarrel = 8019,

    [ParentCombo(MCH_AutoCrossbow)]
    [CustomComboInfo(Job.MCH, MCH_AutoCrossbow_GaussRound)]
    MCH_AutoCrossbow_GaussRound = 8020,

    #endregion

    [ReplaceSkill(MCH.RookAutoturret, MCH.AutomatonQueen)]
    [CustomComboInfo(Job.MCH, MCH_Overdrive)]
    MCH_Overdrive = 8002,

    [ReplaceSkill(MCH.HotShot)]
    [CustomComboInfo(Job.MCH, MCH_BigHitter)]
    MCH_BigHitter = 8004,

    [ReplaceSkill(MCH.GaussRound, MCH.Ricochet, MCH.CheckMate, MCH.DoubleCheck)]
    [CustomComboInfo(Job.MCH, MCH_GaussRoundRicochet)]
    MCH_GaussRoundRicochet = 8003,

    // Last value ST = 8119
    // Last value AoE = 8315
    // Last value Misc = 8058

    #endregion

    #region MONK

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(MNK.Bootshine, MNK.LeapingOpo)]
    [ConflictingCombos(MNK_Basic_BeastChakras, MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_SimpleMode)]
    [SimpleCombo]
    MNK_ST_SimpleMode = 9004,

    [AutoAction(true, false)]
    [ReplaceSkill(MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
    [ConflictingCombos(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AOE_SimpleMode)]
    [SimpleCombo]
    MNK_AOE_SimpleMode = 9003,

    #endregion

    #region Monk Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(MNK.Bootshine, MNK.LeapingOpo)]
    [ConflictingCombos(MNK_Basic_BeastChakras, MNK_ST_SimpleMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_AdvancedMode)]
    [AdvancedCombo]
    MNK_ST_AdvancedMode = 9005,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseMeditation)]
    MNK_STUseMeditation = 9007,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseTheForbiddenChakra)]
    MNK_STUseTheForbiddenChakra = 9012,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseFormShift)]
    MNK_STUseFormShift = 9017,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseOpener)]
    MNK_STUseOpener = 9006,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseBuffs)]
    MNK_STUseBuffs = 9008,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo(Job.MNK, MNK_STUseBrotherhood)]
    MNK_STUseBrotherhood = 9009,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo(Job.MNK, MNK_STUseROF)]
    MNK_STUseROF = 9011,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseFiresReply)]
    MNK_STUseFiresReply = 9016,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo(Job.MNK, MNK_STUseROW)]
    MNK_STUseROW = 9010,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseWindsReply)]
    MNK_STUseWindsReply = 9015,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUsePerfectBalance)]
    MNK_STUsePerfectBalance = 9013,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseMasterfulBlitz)]
    MNK_STUseMasterfulBlitz = 9039,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_STUseTrueNorth)]
    MNK_STUseTrueNorth = 9014,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_StunInterupt)]
    MNK_ST_StunInterupt = 9044,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_ComboHeals)]
    MNK_ST_ComboHeals = 9018,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_Feint)]
    MNK_ST_Feint = 9095,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_UseRoE)]
    MNK_ST_UseRoE = 9096,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_ST_UseMantra)]
    MNK_ST_UseMantra = 9097,

    #endregion

    #region Monk Advanced AOE

    [AutoAction(true, false)]
    [ReplaceSkill(MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
    [ConflictingCombos(MNK_AOE_SimpleMode)]
    [CustomComboInfo(Job.MNK, MNK_AOE_AdvancedMode)]
    [AdvancedCombo]
    MNK_AOE_AdvancedMode = 9027,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseMeditation)]
    MNK_AoEUseMeditation = 9028,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseHowlingFist)]
    MNK_AoEUseHowlingFist = 9033,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseFormShift)]
    MNK_AoEUseFormShift = 9038,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseBuffs)]
    MNK_AoEUseBuffs = 9029,

    [ParentCombo(MNK_AoEUseBuffs)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseBrotherhood)]
    MNK_AoEUseBrotherhood = 9030,

    [ParentCombo(MNK_AoEUseBuffs)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseROF)]
    MNK_AoEUseROF = 9032,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseFiresReply)]
    MNK_AoEUseFiresReply = 9036,

    [ParentCombo(MNK_AoEUseBuffs)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseROW)]
    MNK_AoEUseROW = 9031,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseWindsReply)]
    MNK_AoEUseWindsReply = 9035,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUsePerfectBalance)]
    MNK_AoEUsePerfectBalance = 9034,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoEUseMasterfulBlitz)]
    MNK_AoEUseMasterfulBlitz = 9040,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoE_StunInterupt)]
    MNK_AoE_StunInterupt = 9045,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(Job.MNK, MNK_AoE_ComboHeals)]
    MNK_AoE_ComboHeals = 9037,

    #endregion

    #region Basic Combo

    [ConflictingCombos(MNK_ST_AdvancedMode, MNK_ST_SimpleMode)]
    [CustomComboInfo(Job.MNK, MNK_Basic_BeastChakras)]
    MNK_Basic_BeastChakras = 9019,

    #endregion

    #region Movement

    [CustomComboInfo(Job.MNK, MNK_Retarget_Thunderclap)]
    [Retargeted(MNK.Thunderclap)]
    MNK_Retarget_Thunderclap = 9043,

    #endregion

    #region Misc

    [ReplaceSkill(MNK.PerfectBalance)]
    [ConflictingCombos(MNK_PerfectBalanceProtection)]
    [CustomComboInfo(Job.MNK, MNK_PerfectBalance)]
    MNK_PerfectBalance = 9023,

    [ReplaceSkill(MNK.RiddleOfFire, MNK.Brotherhood)]
    [CustomComboInfo(Job.MNK, MNK_Brotherhood_Riddle)]
    MNK_Brotherhood_Riddle = 9024,

    [ReplaceSkill(MNK.PerfectBalance)]
    [ConflictingCombos(MNK_PerfectBalance)]
    [CustomComboInfo(Job.MNK, MNK_PerfectBalanceProtection)]
    MNK_PerfectBalanceProtection = 9042,

    #endregion

    #region Hidden Features

    [Hidden]
    [CustomComboInfo(Job.MNK, MNK_Hidden)]
    MNK_Hidden = 9300,

    #endregion

    // Last value = 9045
    // Hidden = 9300

    #endregion

    #region NINJA

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(NIN.SpinningEdge)]
    [ConflictingCombos(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_SimpleMode)]
    [SimpleCombo]
    NIN_ST_SimpleMode = 10000,

    [AutoAction(true, false)]
    [ReplaceSkill(NIN.DeathBlossom)]
    [ConflictingCombos(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_SimpleMode)]
    [SimpleCombo]
    NIN_AoE_SimpleMode = 10001,

    #endregion

    #region ST Advanced
    [AutoAction(false, false)]
    [ReplaceSkill(NIN.SpinningEdge)]
    [ConflictingCombos(NIN_ST_SimpleMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode)]
    [AdvancedCombo]
    NIN_ST_AdvancedMode = 10002,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_BalanceOpener)]
    NIN_ST_AdvancedMode_BalanceOpener = 10004,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Ninjitsus)]
    NIN_ST_AdvancedMode_Ninjitsus = 10005,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Ninjitsus_Raiton)]
    NIN_ST_AdvancedMode_Ninjitsus_Raiton = 10051,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Ninjitsus_Suiton)]
    NIN_ST_AdvancedMode_Ninjitsus_Suiton = 10052,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Ninjitsus_Hyosho)]
    NIN_ST_AdvancedMode_Ninjitsus_Hyosho = 10053,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_TrickAttack)]
    NIN_ST_AdvancedMode_TrickAttack = 10006,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Mug)]
    NIN_ST_AdvancedMode_Mug = 10007,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Bunshin)]
    NIN_ST_AdvancedMode_Bunshin = 10008,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Bhavacakra)]
    NIN_ST_AdvancedMode_Bhavacakra = 10009,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Kassatsu)]
    NIN_ST_AdvancedMode_Kassatsu = 10010,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_TenChiJin)]
    NIN_ST_AdvancedMode_TenChiJin = 10011,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_TenriJindo)]
    NIN_ST_AdvancedMode_TenriJindo = 10054,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Assassinate)]
    NIN_ST_AdvancedMode_Assassinate = 10012,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Meisui)]
    NIN_ST_AdvancedMode_Meisui = 10013,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_StunInterupt)]
    NIN_ST_AdvancedMode_StunInterupt = 10045,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_PhantomKamaitachi)]
    NIN_ST_AdvancedMode_PhantomKamaitachi = 10014,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Raiju)]
    NIN_ST_AdvancedMode_Raiju = 10015,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_ThrowingDaggers)]
    NIN_ST_AdvancedMode_ThrowingDaggers = 10016,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_SecondWind)]
    NIN_ST_AdvancedMode_SecondWind = 10017,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_ShadeShift)]
    NIN_ST_AdvancedMode_ShadeShift = 10018,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Bloodbath)]
    NIN_ST_AdvancedMode_Bloodbath = 10019,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_ST_AdvancedMode_Feint)]
    NIN_ST_AdvancedMode_Feint = 10020,

    #endregion

    #region AoE Advanced
    [AutoAction(true, false)]
    [ReplaceSkill(NIN.DeathBlossom)]
    [ConflictingCombos(NIN_AoE_SimpleMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode)]
    [AdvancedCombo]
    NIN_AoE_AdvancedMode = 10003,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Ninjitsus)]
    NIN_AoE_AdvancedMode_Ninjitsus = 10021,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Ninjitsus_Katon)]
    NIN_AoE_AdvancedMode_Ninjitsus_Katon = 10047,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Ninjitsus_Huton)]
    NIN_AoE_AdvancedMode_Ninjitsus_Huton = 10048,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Ninjitsus_Doton)]
    NIN_AoE_AdvancedMode_Ninjitsus_Doton = 10049,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Ninjitsus_Goka)]
    NIN_AoE_AdvancedMode_Ninjitsus_Goka = 10050,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_TrickAttack)]
    NIN_AoE_AdvancedMode_TrickAttack = 10022,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Mug)]
    NIN_AoE_AdvancedMode_Mug = 10023,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Bunshin)]
    NIN_AoE_AdvancedMode_Bunshin = 10024,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_HellfrogMedium)]
    NIN_AoE_AdvancedMode_HellfrogMedium = 10025,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Kassatsu)]
    NIN_AoE_AdvancedMode_Kassatsu = 10026,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_TenChiJin)]
    NIN_AoE_AdvancedMode_TenChiJin = 10027,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_TenriJindo)]
    NIN_AoE_AdvancedMode_TenriJindo = 10055,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Assassinate)]
    NIN_AoE_AdvancedMode_Assassinate = 10028,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Meisui)]
    NIN_AoE_AdvancedMode_Meisui = 10029,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_StunInterupt)]
    NIN_AoE_AdvancedMode_StunInterupt = 10044,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_PhantomKamaitachi)]
    NIN_AoE_AdvancedMode_PhantomKamaitachi = 10030,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_ThrowingDaggers)]
    NIN_AoE_AdvancedMode_ThrowingDaggers = 10031,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_SecondWind)]
    NIN_AoE_AdvancedMode_SecondWind = 10032,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_ShadeShift)]
    NIN_AoE_AdvancedMode_ShadeShift = 10033,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo(Job.NIN, NIN_AoE_AdvancedMode_Bloodbath)]
    NIN_AoE_AdvancedMode_Bloodbath = 10034,

    #endregion

    #region Standalones

    #region Basic Combo

    [ReplaceSkill(NIN.AeolianEdge)]
    [CustomComboInfo(Job.NIN, NIN_ST_AeolianEdgeCombo)]
    [BasicCombo]
    NIN_ST_AeolianEdgeCombo = 10042,

    [ReplaceSkill(NIN.ArmorCrush)]
    [CustomComboInfo(Job.NIN, NIN_ArmorCrushCombo)]
    NIN_ArmorCrushCombo = 10041,

    #endregion
    [ReplaceSkill(NIN.ShadeShift, NIN.Shukuchi, RoleActions.Melee.Feint, RoleActions.Melee.Bloodbath, RoleActions.Physical.SecondWind)]
    [CustomComboInfo(Job.NIN, NIN_MudraProtection)]
    NIN_MudraProtection = 10046,


    [ReplaceSkill(NIN.Kassatsu)]
    [CustomComboInfo(Job.NIN, NIN_KassatsuTrick)]
    NIN_KassatsuTrick = 10035,

    [ReplaceSkill(NIN.TenChiJin)]
    [CustomComboInfo(Job.NIN, NIN_TCJMeisui)]
    NIN_TCJMeisui = 10036,

    [ReplaceSkill(NIN.Chi)]
    [CustomComboInfo(Job.NIN, NIN_KassatsuChiJin)]
    NIN_KassatsuChiJin = 10037,

    [ReplaceSkill(NIN.Hide)]
    [CustomComboInfo(Job.NIN, NIN_HideMug)]
    NIN_HideMug = 10038,

    [ReplaceSkill(NIN.Ten, NIN.Chi, NIN.Jin)]
    [ConflictingCombos(Preset.NIN_Simple_Mudras_Alt)]
    [CustomComboInfo(Job.NIN, NIN_Simple_Mudras)]
    NIN_Simple_Mudras = 10039,

    [ReplaceSkill(NIN.Ten, NIN.Chi, NIN.Jin)]
    [ConflictingCombos(Preset.NIN_Simple_Mudras)]
    [CustomComboInfo(Job.NIN, NIN_Simple_Mudras_Alt)]
    NIN_Simple_Mudras_Alt = 10043,

    [ReplaceSkill(NIN.TenChiJin)]
    [ParentCombo(NIN_TCJMeisui)]
    [CustomComboInfo(Job.NIN, NIN_TCJ)]
    NIN_TCJ = 10040,

    #endregion

    // Last value = 10053

    #endregion

    #region PICTOMANCER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(PCT.FireInRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_SimpleMode)]
    [SimpleCombo]
    PCT_ST_SimpleMode = 20000,

    [AutoAction(true, false)]
    [ReplaceSkill(PCT.FireIIinRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_SimpleMode)]
    [SimpleCombo]
    PCT_AoE_SimpleMode = 20001,

    #endregion

    #region ST

    [AutoAction(false, false)]
    [ReplaceSkill(PCT.FireInRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_ST_SimpleMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode)]
    [AdvancedCombo]
    PCT_ST_AdvancedMode = 20005,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_Advanced_Openers)]
    PCT_ST_Advanced_Openers = 20006,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_SubtractivePalette)]
    PCT_ST_AdvancedMode_SubtractivePalette = 20025,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_BlizzardInCyan)]
    PCT_ST_AdvancedMode_BlizzardInCyan = 20033,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_HolyinWhite)]
    PCT_ST_AdvancedMode_HolyinWhite = 20072,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_CometinBlack)]
    PCT_ST_AdvancedMode_CometinBlack = 20026,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_LivingMuse)]
    PCT_ST_AdvancedMode_LivingMuse = 20022,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_MogOfTheAges)]
    PCT_ST_AdvancedMode_MogOfTheAges = 20024,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_SteelMuse)]
    PCT_ST_AdvancedMode_SteelMuse = 20023,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_HammerStampCombo)]
    PCT_ST_AdvancedMode_HammerStampCombo = 20027,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_ScenicMuse)]
    PCT_ST_AdvancedMode_ScenicMuse = 20021,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_StarPrism)]
    PCT_ST_AdvancedMode_StarPrism = 20012,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_RainbowDrip)]
    PCT_ST_AdvancedMode_RainbowDrip = 20013,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_LucidDreaming)]
    PCT_ST_AdvancedMode_LucidDreaming = 20034,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_MotifFeature)]
    PCT_ST_AdvancedMode_MotifFeature = 20016,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_LandscapeMotif)]
    PCT_ST_AdvancedMode_LandscapeMotif = 20017,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_CreatureMotif)]
    PCT_ST_AdvancedMode_CreatureMotif = 20018,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_WeaponMotif)]
    PCT_ST_AdvancedMode_WeaponMotif = 20019,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_PrePullMotifs)]
    PCT_ST_AdvancedMode_PrePullMotifs = 20008,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_NoTargetMotifs)]
    PCT_ST_AdvancedMode_NoTargetMotifs = 20009,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_SwiftMotifs)]
    PCT_ST_AdvancedMode_SwiftMotifs = 20035,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_MovementFeature)]
    PCT_ST_AdvancedMode_MovementFeature = 20028,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_MovementOption_HammerStampCombo)]
    PCT_ST_AdvancedMode_MovementOption_HammerStampCombo = 20029,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_MovementOption_HolyInWhite)]
    PCT_ST_AdvancedMode_MovementOption_HolyInWhite = 20030,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_MovementOption_CometinBlack)]
    PCT_ST_AdvancedMode_MovementOption_CometinBlack = 20031,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_SwiftcastOption)]
    PCT_ST_AdvancedMode_SwiftcastOption = 20032,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_Addle)]
    PCT_ST_AdvancedMode_Addle = 20070,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_ST_AdvancedMode_Tempura)]
    PCT_ST_AdvancedMode_Tempura = 20071,

    #endregion

    #region AoE

    [AutoAction(true, false)]
    [ReplaceSkill(PCT.FireIIinRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_AoE_SimpleMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode)]
    [AdvancedCombo]
    PCT_AoE_AdvancedMode = 20040,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_SubtractivePalette)]
    PCT_AoE_AdvancedMode_SubtractivePalette = 20058,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_BlizzardInCyan)]
    PCT_AoE_AdvancedMode_BlizzardInCyan = 20066,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_HolyinWhite)]
    PCT_AoE_AdvancedMode_HolyinWhite = 20068,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_CometinBlack)]
    PCT_AoE_AdvancedMode_CometinBlack = 20059,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_LivingMuse)]
    PCT_AoE_AdvancedMode_LivingMuse = 20055,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_MogOfTheAges)]
    PCT_AoE_AdvancedMode_MogOfTheAges = 20057,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_SteelMuse)]
    PCT_AoE_AdvancedMode_SteelMuse = 20056,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_HammerStampCombo)]
    PCT_AoE_AdvancedMode_HammerStampCombo = 20060,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_ScenicMuse)]
    PCT_AoE_AdvancedMode_ScenicMuse = 20054,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_StarPrism)]
    PCT_AoE_AdvancedMode_StarPrism = 20045,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_RainbowDrip)]
    PCT_AoE_AdvancedMode_RainbowDrip = 20046,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_LucidDreaming)]
    PCT_AoE_AdvancedMode_LucidDreaming = 20067,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_MotifFeature)]
    PCT_AoE_AdvancedMode_MotifFeature = 20049,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_LandscapeMotif)]
    PCT_AoE_AdvancedMode_LandscapeMotif = 20050,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_CreatureMotif)]
    PCT_AoE_AdvancedMode_CreatureMotif = 20051,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_WeaponMotif)]
    PCT_AoE_AdvancedMode_WeaponMotif = 20052,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_PrePullMotifs)]
    PCT_AoE_AdvancedMode_PrePullMotifs = 20041,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_NoTargetMotifs)]
    PCT_AoE_AdvancedMode_NoTargetMotifs = 20042,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_SwiftMotifs)]
    PCT_AoE_AdvancedMode_SwiftMotifs = 20069,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_MovementFeature)]
    PCT_AoE_AdvancedMode_MovementFeature = 20061,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo)]
    PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo = 20062,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_MovementOption_HolyInWhite)]
    PCT_AoE_AdvancedMode_MovementOption_HolyInWhite = 20063,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_MovementOption_CometinBlack)]
    PCT_AoE_AdvancedMode_MovementOption_CometinBlack = 20064,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo(Job.PCT, PCT_AoE_AdvancedMode_SwiftcastOption)]
    PCT_AoE_AdvancedMode_SwiftcastOption = 20065,

    #endregion

    #region Standalone Features

    [ReplaceSkill(PCT.FireInRed, PCT.FireIIinRed)]
    [ConflictingCombos(PCT_ST_SimpleMode, PCT_AoE_SimpleMode)]
    [CustomComboInfo(Job.PCT, CombinedAetherhues)]
    CombinedAetherhues = 20002,

    [ReplaceSkill(PCT.CreatureMotif, PCT.WeaponMotif, PCT.LandscapeMotif)]
    [CustomComboInfo(Job.PCT, CombinedMotifs)]
    CombinedMotifs = 20003,

    [ReplaceSkill(PCT.HolyInWhite)]
    [CustomComboInfo(Job.PCT, CombinedPaint)]
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
    [CustomComboInfo(Job.PLD, PLD_ST_SimpleMode)]
    [SimpleCombo]
    PLD_ST_SimpleMode = 11000,

    [AutoAction(true, false)]
    [ConflictingCombos(PLD_AoE_AdvancedMode)]
    [ReplaceSkill(PLD.TotalEclipse)]
    [CustomComboInfo(Job.PLD, PLD_AoE_SimpleMode)]
    [SimpleCombo]
    PLD_AoE_SimpleMode = 11001,

    #endregion

    #region ST Advanced Mode

    [AutoAction(false, false)]
    [ConflictingCombos(PLD_ST_SimpleMode)]
    [ReplaceSkill(PLD.FastBlade)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode)]
    [AdvancedCombo]
    PLD_ST_AdvancedMode = 11002,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_BalanceOpener)]
    PLD_ST_AdvancedMode_BalanceOpener = 11046,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_Interrupt)]
    PLD_ST_Interrupt = 11058,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_LowBlow)]
    PLD_ST_LowBlow = 11062,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_ShieldBash)]
    PLD_ST_ShieldBash = 11066,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_FoF)]
    PLD_ST_AdvancedMode_FoF = 11003,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_ShieldLob)]
    PLD_ST_AdvancedMode_ShieldLob = 11004,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_CircleOfScorn)]
    PLD_ST_AdvancedMode_CircleOfScorn = 11005,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_SpiritsWithin)]
    PLD_ST_AdvancedMode_SpiritsWithin = 11006,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_GoringBlade)]
    PLD_ST_AdvancedMode_GoringBlade = 11008,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_HolySpirit)]
    PLD_ST_AdvancedMode_HolySpirit = 11009,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_Requiescat)]
    PLD_ST_AdvancedMode_Requiescat = 11010,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_Intervene)]
    PLD_ST_AdvancedMode_Intervene = 11011,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_Atonement)]
    PLD_ST_AdvancedMode_Atonement = 11012,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_Confiteor)]
    PLD_ST_AdvancedMode_Confiteor = 11013,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_Blades)]
    PLD_ST_AdvancedMode_Blades = 11014,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_BladeOfHonor)]
    PLD_ST_AdvancedMode_BladeOfHonor = 11033,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_ST_AdvancedMode_MP_Reserve)]
    PLD_ST_AdvancedMode_MP_Reserve = 11035,

    #endregion

    #region AoE Advanced Mode

    [AutoAction(true, false)]
    [ConflictingCombos(PLD_AoE_SimpleMode)]
    [ReplaceSkill(PLD.TotalEclipse)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode)]
    [AdvancedCombo]
    PLD_AoE_AdvancedMode = 11015,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_Interrupt)]
    PLD_AoE_Interrupt = 11059,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_LowBlow)]
    PLD_AoE_LowBlow = 11060,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_ShieldBash)]
    PLD_AoE_ShieldBash = 11065,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_FoF)]
    PLD_AoE_AdvancedMode_FoF = 11016,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_SpiritsWithin)]
    PLD_AoE_AdvancedMode_SpiritsWithin = 11017,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_CircleOfScorn)]
    PLD_AoE_AdvancedMode_CircleOfScorn = 11018,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_Requiescat)]
    PLD_AoE_AdvancedMode_Requiescat = 11019,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_Intervene)]
    PLD_AoE_AdvancedMode_Intervene = 11037,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_HolyCircle)]
    PLD_AoE_AdvancedMode_HolyCircle = 11020,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_Confiteor)]
    PLD_AoE_AdvancedMode_Confiteor = 11021,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_Blades)]
    PLD_AoE_AdvancedMode_Blades = 11022,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_BladeOfHonor)]
    PLD_AoE_AdvancedMode_BladeOfHonor = 11034,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo(Job.PLD, PLD_AoE_AdvancedMode_MP_Reserve)]
    PLD_AoE_AdvancedMode_MP_Reserve = 11036,

    // AoE Mitigation Options
    #endregion

    #region Advanced Mitigation

    [CustomComboInfo(Job.PLD, PLD_Mitigation)]
    PLD_Mitigation = 11086,

    [ParentCombo(PLD_Mitigation)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss)]
    PLD_Mitigation_NonBoss = 11087,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_Rampart)]
    PLD_Mitigation_NonBoss_Rampart = 11088,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_Sentinel)]
    PLD_Mitigation_NonBoss_Sentinel = 11089,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_Bulwark)]
    PLD_Mitigation_NonBoss_Bulwark = 11090,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_HallowedGround)]
    PLD_Mitigation_NonBoss_HallowedGround = 11091,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_HallowedGroundEmergency)]
    PLD_Mitigation_NonBoss_HallowedGroundEmergency = 11104,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_ArmsLength)]
    PLD_Mitigation_NonBoss_ArmsLength = 11092,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_Reprisal)]
    PLD_Mitigation_NonBoss_Reprisal = 11099,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_Sheltron)]
    PLD_Mitigation_NonBoss_Sheltron = 11093,

    [ParentCombo(PLD_Mitigation_NonBoss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_NonBoss_DivineVeil)]
    PLD_Mitigation_NonBoss_DivineVeil = 11094,

    [ParentCombo(PLD_Mitigation)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss)]
    PLD_Mitigation_Boss = 11095,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_SheltronOvercap)]
    PLD_Mitigation_Boss_SheltronOvercap = 11096,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_SheltronTankbuster)]
    PLD_Mitigation_Boss_SheltronTankbuster = 11100,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_Rampart)]
    PLD_Mitigation_Boss_Rampart = 11102,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_Sentinel)]
    PLD_Mitigation_Boss_Sentinel = 11101,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_Bulwark)]
    PLD_Mitigation_Boss_Bulwark = 11103,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_Reprisal)]
    PLD_Mitigation_Boss_Reprisal = 11097,

    [ParentCombo(PLD_Mitigation_Boss)]
    [CustomComboInfo(Job.PLD, PLD_Mitigation_Boss_DivineVeil)]
    PLD_Mitigation_Boss_DivineVeil = 11098,

    [ParentCombo(PLD_Mitigation)]
    [CustomComboInfo(Job.PLD, PLD_BlockForWings)]
    PLD_BlockForWings = 11074,
    #endregion

    #region Basic combo

    [ReplaceSkill(PLD.RageOfHalone, PLD.RoyalAuthority)]
    [CustomComboInfo(Job.PLD, PLD_ST_BasicCombo)]
    [BasicCombo]
    PLD_ST_BasicCombo = 11061,

    [ReplaceSkill(PLD.RageOfHalone, PLD.Prominence)]
    [CustomComboInfo(Job.PLD, PLD_AoE_BasicCombo)]
    [BasicCombo]
    PLD_AoE_BasicCombo = 11078,

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(PLD.Bulwark)]
    [CustomComboInfo(Job.PLD, PLD_Mit_OneButton)]
    [MitigationCombo]
    PLD_Mit_OneButton = 11047,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_HallowedGround_Max)]
    PLD_Mit_HallowedGround_Max = 11048,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Sheltron)]
    PLD_Mit_Sheltron = 11049,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Reprisal)]
    PLD_Mit_Reprisal = 11050,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_DivineVeil)]
    PLD_Mit_DivineVeil = 11051,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Rampart)]
    PLD_Mit_Rampart = 11052,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Bulwark)]
    PLD_Mit_Bulwark = 11055,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_ArmsLength)]
    PLD_Mit_ArmsLength = 11054,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Sentinel)]
    PLD_Mit_Sentinel = 11053,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Clemency)]
    PLD_Mit_Clemency = 11057,

    [ReplaceSkill(PLD.DivineVeil)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Party)]
    [MitigationCombo]
    PLD_Mit_Party = 11063,

    [ParentCombo(PLD_Mit_Party)]
    [CustomComboInfo(Job.PLD, PLD_Mit_Party_Wings)]
    PLD_Mit_Party_Wings = 11064,
    #endregion

    #region Extra Features

    [ReplaceSkill(PLD.Requiescat, PLD.Imperator)]
    [CustomComboInfo(Job.PLD, PLD_Requiescat_Options)]
    PLD_Requiescat_Options = 11024,

    [ReplaceSkill(PLD.SpiritsWithin, PLD.Expiacion)]
    [CustomComboInfo(Job.PLD, PLD_SpiritsWithin)]
    PLD_SpiritsWithin = 11025,

    [ReplaceSkill(PLD.ShieldLob)]
    [CustomComboInfo(Job.PLD, PLD_ShieldLob_Feature)]
    PLD_ShieldLob_Feature = 11027,

    [ReplaceSkill(PLD.Clemency)]
    [CustomComboInfo(Job.PLD, PLD_RetargetClemency)]
    [Retargeted]
    PLD_RetargetClemency = 11067,

    [ParentCombo(PLD_RetargetClemency)]
    [CustomComboInfo(Job.PLD, PLD_RetargetClemency_MO)]
    [Retargeted(PLD.Clemency)]
    PLD_RetargetClemency_MO = 11071,

    [ParentCombo(PLD_RetargetClemency)]
    [CustomComboInfo(Job.PLD, PLD_RetargetClemency_LowHP)]
    [Retargeted(PLD.Clemency)]
    PLD_RetargetClemency_LowHP = 11072,

    [ReplaceSkill(PLD.Sheltron)]
    [CustomComboInfo(Job.PLD, PLD_RetargetSheltron)]
    [Retargeted]
    PLD_RetargetSheltron = 11068,

    [ParentCombo(PLD_RetargetSheltron)]
    [CustomComboInfo(Job.PLD, PLD_RetargetSheltron_MO)]
    [Retargeted(PLD.Sheltron)]
    PLD_RetargetSheltron_MO = 11069,

    [ParentCombo(PLD_RetargetSheltron)]
    [CustomComboInfo(Job.PLD, PLD_RetargetSheltron_TT)]
    [Retargeted(PLD.Sheltron)]
    PLD_RetargetSheltron_TT = 11070,

    [ConflictingCombos(ALL_Tank_Interrupt)]
    [CustomComboInfo(Job.PLD, PLD_RetargetShieldBash)]
    [Retargeted(PLD.ShieldBash)]
    PLD_RetargetShieldBash = 11073,

    [ReplaceSkill(PLD.Cover)]
    [CustomComboInfo(Job.PLD, PLD_RetargetCover)]
    [Retargeted]
    PLD_RetargetCover = 11075,

    [ParentCombo(PLD_RetargetCover)]
    [CustomComboInfo(Job.PLD, PLD_RetargetCover_MO)]
    [Retargeted(PLD.Cover)]
    PLD_RetargetCover_MO = 11076,

    [ParentCombo(PLD_RetargetCover)]
    [CustomComboInfo(Job.PLD, PLD_RetargetCover_LowHP)]
    [Retargeted(PLD.Cover)]
    PLD_RetargetCover_LowHP = 11077,

    [ReplaceSkill(PLD.Intervene)]
    [CustomComboInfo(Job.PLD, PLD_RetargetIntervene)]
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
    [CustomComboInfo(Job.RPR, RPR_ST_SimpleMode)]
    [SimpleCombo]
    RPR_ST_SimpleMode = 12000,

    [AutoAction(true, false)]
    [ReplaceSkill(RPR.SpinningScythe)]
    [ConflictingCombos(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_SimpleMode)]
    [SimpleCombo]
    RPR_AoE_SimpleMode = 12100,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(RPR.Slice)]
    [ConflictingCombos(RPR_ST_SimpleMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_AdvancedMode)]
    [AdvancedCombo]
    RPR_ST_AdvancedMode = 12001,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Opener)]
    RPR_ST_Opener = 12002,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_ArcaneCircle)]
    RPR_ST_ArcaneCircle = 12006,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_PlentifulHarvest)]
    RPR_ST_PlentifulHarvest = 12007,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_SoD)]
    RPR_ST_SoD = 12003,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_SoulSow)]
    RPR_ST_SoulSow = 12020,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_SoulSlice)]
    RPR_ST_SoulSlice = 12004,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Bloodstalk)]
    RPR_ST_Bloodstalk = 12008,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Gluttony)]
    RPR_ST_Gluttony = 12009,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_GibbetGallows)]
    RPR_ST_GibbetGallows = 12016,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Enshroud)]
    RPR_ST_Enshroud = 12010,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Reaping)]
    RPR_ST_Reaping = 12011,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Lemure)]
    RPR_ST_Lemure = 12012,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Communio)]
    RPR_ST_Communio = 12014,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Sacrificium)]
    RPR_ST_Sacrificium = 12013,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Perfectio)]
    RPR_ST_Perfectio = 12015,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_TrueNorthDynamic)]
    RPR_ST_TrueNorthDynamic = 12098,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_StunInterupt)]
    RPR_ST_StunInterupt = 12096,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_RangedFiller)]
    RPR_ST_RangedFiller = 12017,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_RangedFillerHarvestMoon)]
    RPR_ST_RangedFillerHarvestMoon = 12024,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_ComboHeals)]
    RPR_ST_ComboHeals = 12097,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_Feint)]
    RPR_ST_Feint = 12095,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_ST_ArcaneCrest)]
    RPR_ST_ArcaneCrest = 12022,

    //last value = 12024

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(RPR.SpinningScythe)]
    [ConflictingCombos(RPR_AoE_SimpleMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_AdvancedMode)]
    [AdvancedCombo]
    RPR_AoE_AdvancedMode = 12101,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_ArcaneCircle)]
    RPR_AoE_ArcaneCircle = 12105,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_PlentifulHarvest)]
    RPR_AoE_PlentifulHarvest = 12106,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_WoD)]
    RPR_AoE_WoD = 12102,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_SoulSow)]
    RPR_AoE_SoulSow = 12117,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_SoulScythe)]
    RPR_AoE_SoulScythe = 12103,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_GrimSwathe)]
    RPR_AoE_GrimSwathe = 12107,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Gluttony)]
    RPR_AoE_Gluttony = 12108,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Guillotine)]
    RPR_AoE_Guillotine = 12115,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Enshroud)]
    RPR_AoE_Enshroud = 12109,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Reaping)]
    RPR_AoE_Reaping = 12110,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Lemure)]
    RPR_AoE_Lemure = 12111,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Communio)]
    RPR_AoE_Communio = 12113,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Sacrificium)]
    RPR_AoE_Sacrificium = 12112,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_Perfectio)]
    RPR_AoE_Perfectio = 12114,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_StunInterupt)]
    RPR_AoE_StunInterupt = 12118,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.RPR, RPR_AoE_ComboHeals)]
    RPR_AoE_ComboHeals = 12116,

    // Last value = 12118

    #endregion

    #region Basic combo

    [ReplaceSkill(RPR.InfernalSlice)]
    [CustomComboInfo(Job.RPR, RPR_ST_BasicCombo)]
    [BasicCombo]
    RPR_ST_BasicCombo = 12021,

    [ParentCombo(RPR_ST_BasicCombo)]
    [CustomComboInfo(Job.RPR, RPR_ST_BasicCombo_SoD)]
    RPR_ST_BasicCombo_SoD = 12023,

    #endregion

    #region Blood Stalk/Grim Swathe Combo Section

    [ReplaceSkill(RPR.BloodStalk, RPR.GrimSwathe)]
    [CustomComboInfo(Job.RPR, RPR_GluttonyBloodSwathe)]
    RPR_GluttonyBloodSwathe = 12200,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo(Job.RPR, RPR_GluttonyBloodSwathe_BloodSwatheCombo)]
    RPR_GluttonyBloodSwathe_BloodSwatheCombo = 12201,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo(Job.RPR, RPR_GluttonyBloodSwathe_Enshroud)]
    RPR_GluttonyBloodSwathe_Enshroud = 12202,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo(Job.RPR, RPR_GluttonyBloodSwathe_OGCD)]
    RPR_GluttonyBloodSwathe_OGCD = 12204,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo(Job.RPR, RPR_GluttonyBloodSwathe_Sacrificium)]
    RPR_GluttonyBloodSwathe_Sacrificium = 12203,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo(Job.RPR, RPR_TrueNorthGluttony)]
    RPR_TrueNorthGluttony = 12310,

    // Last value = 12204

    #endregion

    #region Miscellaneous

    [ReplaceSkill(RPR.Slice, RPR.SpinningScythe, RPR.ShadowOfDeath, RPR.Harpe, RPR.BloodStalk)]
    [CustomComboInfo(Job.RPR, RPR_Soulsow)]
    RPR_Soulsow = 12302,

    [ParentCombo(RPR_Soulsow)]
    [CustomComboInfo(Job.RPR, RPR_Soulsow_Combat)]
    RPR_Soulsow_Combat = 12309,

    [ReplaceSkill(RPR.ArcaneCircle)]
    [CustomComboInfo(Job.RPR, RPR_ArcaneCirclePlentifulHarvest)]
    RPR_ArcaneCirclePlentifulHarvest = 12300,

    [ReplaceSkill(RPR.HellsEgress, RPR.HellsIngress)]
    [CustomComboInfo(Job.RPR, RPR_Regress)]
    RPR_Regress = 12301,

    [ReplaceSkill(RPR.Enshroud)]
    [ConflictingCombos(RPR_EnshroudCommunio)]
    [CustomComboInfo(Job.RPR, RPR_EnshroudProtection)]
    RPR_EnshroudProtection = 12304,

    [ParentCombo(RPR_EnshroudProtection)]
    [CustomComboInfo(Job.RPR, RPR_TrueNorthEnshroud)]
    RPR_TrueNorthEnshroud = 12308,

    [ReplaceSkill(RPR.Enshroud)]
    [ConflictingCombos(RPR_EnshroudProtection)]
    [CustomComboInfo(Job.RPR, RPR_EnshroudCommunio)]
    RPR_EnshroudCommunio = 12307,

    [ReplaceSkill(RPR.Gibbet, RPR.Gallows, RPR.Guillotine)]
    [CustomComboInfo(Job.RPR, RPR_CommunioOnGGG)]
    RPR_CommunioOnGGG = 12305,

    [ParentCombo(RPR_CommunioOnGGG)]
    [CustomComboInfo(Job.RPR, RPR_LemureOnGGG)]
    RPR_LemureOnGGG = 12306,

    // Last value = 12312

    #endregion

    #endregion

    #region RED MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ConflictingCombos(RDM_ST_DPS)]
    [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Jolt3)]
    [CustomComboInfo(Job.RDM, RDM_ST_SimpleMode)]
    [SimpleCombo]
    RDM_ST_SimpleMode = 13000,

    [AutoAction(true, false)]
    [ReplaceSkill(RDM.Scatter, RDM.Impact)]
    [ConflictingCombos(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_SimpleMode)]
    [SimpleCombo]
    RDM_AoE_SimpleMode = 13200,

    #endregion

    #region Single Target DPS

    [AutoAction(false, false)]
    [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Jolt3)]
    [ConflictingCombos(RDM_ST_SimpleMode)]
    [CustomComboInfo(Job.RDM, RDM_ST_DPS)]
    [AdvancedCombo]
    RDM_ST_DPS = 13001,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_Balance_Opener)]
    RDM_Balance_Opener = 13002,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_ThunderAero)]
    RDM_ST_ThunderAero = 13003,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_FireStone)]
    RDM_ST_FireStone = 13004,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_HolyFlare)]
    RDM_ST_HolyFlare = 13005,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_MeleeCombo)]
    RDM_ST_MeleeCombo = 13006,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo(Job.RDM, RDM_ST_MeleeCombo_IncludeRiposte)]
    RDM_ST_MeleeCombo_IncludeRiposte = 13007,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo(Job.RDM, RDM_ST_MeleeCombo_IncludeReprise)]
    RDM_ST_MeleeCombo_IncludeReprise = 13027,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo(Job.RDM, RDM_ST_MeleeCombo_GapCloser)]
    RDM_ST_MeleeCombo_GapCloser = 13008,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo(Job.RDM, RDM_ST_MeleeCombo_MeleeCheck)]
    RDM_ST_MeleeCombo_MeleeCheck = 13009,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Embolden)]
    RDM_ST_Embolden = 13010,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Manafication)]
    RDM_ST_Manafication = 13011,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_ViceOfThorns)]
    RDM_ST_ViceOfThorns = 13012,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Prefulgence)]
    RDM_ST_Prefulgence = 13013,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Fleche)]
    RDM_ST_Fleche = 13014,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_ContreSixte)]
    RDM_ST_ContreSixte = 13015,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Engagement)]
    RDM_ST_Engagement = 13016,

    [ParentCombo(RDM_ST_Engagement)]
    [CustomComboInfo(Job.RDM, RDM_ST_Engagement_Pooling)]
    RDM_ST_Engagement_Pooling = 13018,

    [ParentCombo(RDM_ST_Engagement)]
    [CustomComboInfo(Job.RDM, RDM_ST_Engagement_Saving)]
    RDM_ST_Engagement_Saving = 13028,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Corpsacorps)]
    RDM_ST_Corpsacorps = 13017,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Acceleration)]
    RDM_ST_Acceleration = 13019,

    [ParentCombo(RDM_ST_Acceleration)]
    [CustomComboInfo(Job.RDM, RDM_ST_Acceleration_Movement)]
    RDM_ST_Acceleration_Movement = 13020,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Swiftcast)]
    RDM_ST_Swiftcast = 13021,

    [ParentCombo(RDM_ST_Swiftcast)]
    [CustomComboInfo(Job.RDM, RDM_ST_SwiftcastMovement)]
    RDM_ST_SwiftcastMovement = 13022,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Lucid)]
    RDM_ST_Lucid = 13023,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_Addle)]
    RDM_ST_Addle = 13024,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_MagickBarrier)]
    RDM_ST_MagickBarrier = 13025,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo(Job.RDM, RDM_ST_VerCure)]
    RDM_ST_VerCure = 13026,

    //Last Used 13028
    #endregion

    #region AoE DPS

    [AutoAction(true, false)]
    [ReplaceSkill(RDM.Scatter, RDM.Impact)]
    [ConflictingCombos(RDM_AoE_SimpleMode)]
    [CustomComboInfo(Job.RDM, RDM_AoE_DPS)]
    [AdvancedCombo]
    RDM_AoE_DPS = 13201,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_ThunderAero)]
    RDM_AoE_ThunderAero = 13202,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_HolyFlare)]
    RDM_AoE_HolyFlare = 13203,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_MeleeCombo)]
    RDM_AoE_MeleeCombo = 13204,

    [ParentCombo(RDM_AoE_MeleeCombo)]
    [CustomComboInfo(Job.RDM, RDM_AoE_MeleeCombo_Target)]
    RDM_AoE_MeleeCombo_Target = 13205,

    [ParentCombo(RDM_AoE_MeleeCombo)]
    [CustomComboInfo(Job.RDM, RDM_AoE_MeleeCombo_GapCloser)]
    RDM_AoE_MeleeCombo_GapCloser = 13206,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Embolden)]
    RDM_AoE_Embolden = 13207,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Manafication)]
    RDM_AoE_Manafication = 13208,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_ViceOfThorns)]
    RDM_AoE_ViceOfThorns = 13209,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Prefulgence)]
    RDM_AoE_Prefulgence = 13210,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Fleche)]
    RDM_AoE_Fleche = 13211,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_ContreSixte)]
    RDM_AoE_ContreSixte = 13212,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Engagement)]
    RDM_AoE_Engagement = 13213,

    [ParentCombo(RDM_AoE_Engagement)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Engagement_Pooling)]
    RDM_AoE_Engagement_Pooling = 13215,

    [ParentCombo(RDM_AoE_Engagement)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Engagement_Saving)]
    RDM_AoE_Engagement_Saving = 13223,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Corpsacorps)]
    RDM_AoE_Corpsacorps = 13214,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Acceleration)]
    RDM_AoE_Acceleration = 13216,

    [ParentCombo(RDM_AoE_Acceleration)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Acceleration_Movement)]
    RDM_AoE_Acceleration_Movement = 13217,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Swiftcast)]
    RDM_AoE_Swiftcast = 13218,

    [ParentCombo(RDM_AoE_Swiftcast)]
    [CustomComboInfo(Job.RDM, RDM_AoE_SwiftcastMovement)]
    RDM_AoE_SwiftcastMovement = 13219,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_Lucid)]
    RDM_AoE_Lucid = 13220,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo(Job.RDM, RDM_AoE_VerCure)]
    RDM_AoE_VerCure = 13222,

    //Last Used 13223

    #endregion

    #region Stand Alone Features

    [ReplaceSkill(RDM.Veraero, RDM.Veraero3)]
    [CustomComboInfo(Job.RDM, RDM_VerAero)]
    RDM_VerAero = 13400,

    [ReplaceSkill(RDM.Verthunder, RDM.Verthunder3)]
    [CustomComboInfo(Job.RDM, RDM_VerThunder)]
    RDM_VerThunder = 13418,

    [ReplaceSkill(RDM.Veraero2)]
    [CustomComboInfo(Job.RDM, RDM_VerAero2)]
    RDM_VerAero2 = 13432,

    [ReplaceSkill(RDM.Verthunder2)]
    [CustomComboInfo(Job.RDM, RDM_VerThunder2)]
    RDM_VerThunder2 = 13433,

    [ReplaceSkill(RDM.Riposte)]
    [CustomComboInfo(Job.RDM, RDM_Riposte)]
    RDM_Riposte = 13403,

    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo(Job.RDM, RDM_Riposte_Weaves)]
    RDM_Riposte_Weaves = 13434,

    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo(Job.RDM, RDM_Riposte_GapCloser)]
    RDM_Riposte_GapCloser = 13424,

    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo(Job.RDM, RDM_Riposte_Finisher)]
    RDM_Riposte_Finisher = 13423,

    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo(Job.RDM, RDM_Riposte_NoWaste)]
    RDM_Riposte_NoWaste = 13429,

    [ReplaceSkill(RDM.Moulinet)]
    [CustomComboInfo(Job.RDM, RDM_Moulinet)]
    RDM_Moulinet = 13425,

    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo(Job.RDM, RDM_Moulinet_Weaves)]
    RDM_Moulinet_Weaves = 13431,

    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo(Job.RDM, RDM_Moulinet_GapCloser)]
    RDM_Moulinet_GapCloser = 13426,

    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo(Job.RDM, RDM_Moulinet_Finisher)]
    RDM_Moulinet_Finisher = 13427,

    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo(Job.RDM, RDM_Moulinet_NoWaste)]
    RDM_Moulinet_NoWaste = 13428,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Caster_Raise)]
    [CustomComboInfo(Job.RDM, RDM_Raise)]
    RDM_Raise = 13406,

    [ParentCombo(RDM_Raise)]
    [CustomComboInfo(Job.RDM, RDM_Raise_Vercure)]
    RDM_Raise_Vercure = 13407,

    [ParentCombo(RDM_Raise)]
    [CustomComboInfo(Job.RDM, RDM_Raise_Retarget)]
    [Retargeted(RDM.Verraise, RDM.Vercure)]
    RDM_Raise_Retarget = 13408,

    [ReplaceSkill(RDM.Displacement)]
    [CustomComboInfo(Job.RDM, RDM_CorpsDisplacement)]
    RDM_CorpsDisplacement = 13409,

    [ReplaceSkill(RDM.Embolden)]
    [CustomComboInfo(Job.RDM, RDM_EmboldenProtection)]
    RDM_EmboldenProtection = 13412,

    [ParentCombo(RDM_EmboldenProtection)]
    [CustomComboInfo(Job.RDM, RDM_EmboldenManafication)]
    RDM_EmboldenManafication = 13410,

    [ParentCombo(RDM_MagickProtection)]
    [CustomComboInfo(Job.RDM, RDM_MagickBarrierAddle)]
    RDM_MagickBarrierAddle = 13411,

    [ReplaceSkill(RDM.MagickBarrier)]
    [CustomComboInfo(Job.RDM, RDM_MagickProtection)]
    RDM_MagickProtection = 13413,

    [ReplaceSkill(RDM.Fleche)]
    [CustomComboInfo(Job.RDM, RDM_OGCDs)]
    RDM_OGCDs = 13420,

    //Last Used 13434
    #endregion

    #endregion

    #region SAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(SGE.Dosis, SGE.Dosis2, SGE.Dosis3)]
    [ConflictingCombos(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_Simple_DPS)]
    [SimpleCombo]
    SGE_ST_Simple_DPS = 14084,

    [AutoAction(true, false)]
    [ReplaceSkill(SGE.Dyskrasia, SGE.Dyskrasia2)]
    [ConflictingCombos(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Simple_DPS)]
    [SimpleCombo]
    SGE_AoE_Simple_DPS = 14085,

    [AutoAction(false, true)]
    [ReplaceSkill(SGE.Diagnosis)]
    [ConflictingCombos(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_Simple_ST_Heal)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SGE_Simple_ST_Heal = 14087,


    [AutoAction(true, true)]
    [ReplaceSkill(SGE.Prognosis)]
    [ConflictingCombos(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_Simple_AoE_Heal)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SGE_Simple_AoE_Heal = 14086,

    #endregion

    #region Single Target DPS Feature

    [AutoAction(false, false)]
    [ReplaceSkill(SGE.Dosis, SGE.Dosis2, SGE.Dosis3)]
    [ConflictingCombos(SGE_ST_Simple_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS)]
    [AdvancedCombo]
    SGE_ST_DPS = 14001,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Opener)]
    SGE_ST_DPS_Opener = 14055,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_EDosis)]
    SGE_ST_DPS_EDosis = 14003,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_AddersgallProtect)]
    [PossiblyRetargeted(SGE.Druochole)]
    SGE_ST_DPS_AddersgallProtect = 14054,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Rhizo)]
    SGE_ST_DPS_Rhizo = 14007,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Phlegma)]
    SGE_ST_DPS_Phlegma = 14005,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Psyche)]
    SGE_ST_DPS_Psyche = 14008,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Movement)]
    SGE_ST_DPS_Movement = 14004,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Kardia)]
    [Retargeted(SGE.Kardia)]
    SGE_ST_DPS_Kardia = 14006,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Soteria)]
    SGE_ST_DPS_Soteria = 14056,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo(Job.SGE, SGE_ST_DPS_Lucid)]
    SGE_ST_DPS_Lucid = 14002,

    #endregion

    #region AoE DPS Feature

    [AutoAction(true, false)]
    [ReplaceSkill(SGE.Dyskrasia, SGE.Dyskrasia2)]
    [ConflictingCombos(SGE_AoE_Simple_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS)]
    [AdvancedCombo]
    SGE_AoE_DPS = 14009,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_EDyskrasia)]
    SGE_AoE_DPS_EDyskrasia = 14052,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_AddersgallProtect)]
    [PossiblyRetargeted]
    SGE_AoE_DPS_AddersgallProtect = 14053,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Rhizo)]
    SGE_AoE_DPS_Rhizo = 14013,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Phlegma)]
    SGE_AoE_DPS_Phlegma = 14010,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Psyche)]
    SGE_AoE_DPS_Psyche = 14051,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Toxikon)]
    SGE_AoE_DPS_Toxikon = 14011,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Pneuma)]
    SGE_AoE_DPS_Pneuma = 14059,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Soteria)]
    SGE_AoE_DPS_Soteria = 14057,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo(Job.SGE, SGE_AoE_DPS_Lucid)]
    SGE_AoE_DPS_Lucid = 14012,

    #endregion

    #region Single Target Heal

    [AutoAction(false, true)]
    [ReplaceSkill(SGE.Diagnosis)]
    [ConflictingCombos(SGE_Simple_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal)]
    [PossiblyRetargeted(SGE.Diagnosis)]
    [HealingCombo]
    SGE_ST_Heal = 14014,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Lucid)]
    SGE_ST_Heal_Lucid = 14063,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Rhizomata)]
    SGE_ST_Heal_Rhizomata = 14023,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Kardia)]
    [Retargeted(SGE.Kardia)]
    SGE_ST_Heal_Kardia = 14016,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Esuna)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    SGE_ST_Heal_Esuna = 14015,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_EDiagnosis)]
    [PossiblyRetargeted(SGE.Eukrasia)]
    SGE_ST_Heal_EDiagnosis = 14017,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Druochole)]
    [PossiblyRetargeted(SGE.Druochole)]
    SGE_ST_Heal_Druochole = 14025,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Taurochole)]
    [PossiblyRetargeted(SGE.Taurochole)]
    SGE_ST_Heal_Taurochole = 14021,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Haima)]
    [PossiblyRetargeted(SGE.Haima)]
    SGE_ST_Heal_Haima = 14022,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Soteria)]
    SGE_ST_Heal_Soteria = 14018,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Zoe)]
    SGE_ST_Heal_Zoe = 14019,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Krasis)]
    [PossiblyRetargeted(SGE.Krasis)]
    SGE_ST_Heal_Krasis = 14024,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Pepsis)]
    SGE_ST_Heal_Pepsis = 14020,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Physis)]
    [PossiblyRetargeted(SGE.Physis)]
    SGE_ST_Heal_Physis = 14065,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Kerachole)]
    [PossiblyRetargeted(SGE.Kerachole)]
    SGE_ST_Heal_Kerachole = 14066,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Holos)]
    [PossiblyRetargeted(SGE.Holos)]
    SGE_ST_Heal_Holos = 14067,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo(Job.SGE, SGE_ST_Heal_Panhaima)]
    [PossiblyRetargeted(SGE.Panhaima)]
    SGE_ST_Heal_Panhaima = 14068,

    #endregion

    #region AoE Heal

    [AutoAction(true, true)]
    [ReplaceSkill(SGE.Prognosis)]
    [ConflictingCombos(SGE_Simple_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal)]
    [HealingCombo]
    SGE_AoE_Heal = 14026,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Lucid)]
    SGE_AoE_Heal_Lucid = 14064,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Rhizomata)]
    SGE_AoE_Heal_Rhizomata = 14036,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_EPrognosis)]
    SGE_AoE_Heal_EPrognosis = 14028,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Ixochole)]
    SGE_AoE_Heal_Ixochole = 14033,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Kerachole)]
    SGE_AoE_Heal_Kerachole = 14035,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Holos)]
    SGE_AoE_Heal_Holos = 14030,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Physis)]
    SGE_AoE_Heal_Physis = 14027,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Panhaima)]
    SGE_AoE_Heal_Panhaima = 14031,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Zoe)]
    SGE_AoE_Heal_Zoe = 14058,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Philosophia)]
    SGE_AoE_Heal_Philosophia = 14050,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo(Job.SGE, SGE_AoE_Heal_Pepsis)]
    SGE_AoE_Heal_Pepsis = 14032,

    #endregion

    #region Overprotect

    [ReplaceSkill(SGE.Kerachole)]
    [CustomComboInfo(Job.SGE, SGE_OverProtect)]
    SGE_OverProtect = 14043,

    [ParentCombo(SGE_OverProtect)]
    [CustomComboInfo(Job.SGE, SGE_OverProtect_Kerachole)]
    SGE_OverProtect_Kerachole = 14044,

    [ParentCombo(SGE_OverProtect_Kerachole)]
    [CustomComboInfo(Job.SGE, SGE_OverProtect_SacredSoil)]
    SGE_OverProtect_SacredSoil = 14045,

    [ParentCombo(SGE_OverProtect)]
    [CustomComboInfo(Job.SGE, SGE_OverProtect_Panhaima)]
    SGE_OverProtect_Panhaima = 14046,

    [ParentCombo(SGE_OverProtect)]
    [CustomComboInfo(Job.SGE, SGE_OverProtect_Philosophia)]
    SGE_OverProtect_Philosophia = 14047,

    #endregion

    #region Misc Healing

    [ReplaceSkill(SGE.Taurochole, SGE.Druochole, SGE.Ixochole, SGE.Kerachole)]
    [CustomComboInfo(Job.SGE, SGE_Rhizo)]
    SGE_Rhizo = 14037,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo(Job.SGE, SGE_Raise)]
    SGE_Raise = 14040,

    [ParentCombo(SGE_Raise)]
    [CustomComboInfo(Job.SGE, SGE_Raise_Retarget)]
    [Retargeted(SGE.Egeiro)]
    SGE_Raise_Retarget = 14061,

    [ReplaceSkill(SGE.Pneuma)]
    [CustomComboInfo(Job.SGE, SGE_ZoePneuma)]
    SGE_ZoePneuma = 14039,

    [ReplaceSkill(SGE.Soteria)]
    [CustomComboInfo(Job.SGE, SGE_Kardia)]
    [PossiblyRetargeted("Retargeting Features below, Enable Kardia", Condition.SGERetargetingFeaturesEnabledForKardia)]
    SGE_Kardia = 14041,

    [ReplaceSkill(SGE.Eukrasia)]
    [CustomComboInfo(Job.SGE, SGE_Eukrasia)]
    [PossiblyRetargeted("Retargeting Features below, Enable Eukrasion Diagnosis", Condition.SGERetargetingFeaturesEnabledForEDiagnosis)]
    SGE_Eukrasia = 14042,

    [ReplaceSkill(SGE.Taurochole)]
    [CustomComboInfo(Job.SGE, SGE_TauroDruo)]
    [PossiblyRetargeted("Retargeting Features below, Enable Druochole and Taurochole", Condition.SGERetargetingFeaturesEnabledForTauroDruo)]
    SGE_TauroDruo = 14038,

    [ReplaceSkill(SGE.Krasis)]
    [CustomComboInfo(Job.SGE, SGE_Mit_ST)]
    [PossiblyRetargeted("Retargeting Features below, Enable Krasis, Haima, Eukrasian Diagnosis, and Taurochole", Condition.SGERetargetingFeaturesEnabledForSTMit)]
    SGE_Mit_ST = 14081,

    [ReplaceSkill(SGE.Holos)]
    [CustomComboInfo(Job.SGE, SGE_Mit_AoE)]
    SGE_Mit_AoE = 14082,

    #region Standalone Healing option

    [CustomComboInfo(Job.SGE, SGE_Retarget)]
    [Retargeted]
    SGE_Retarget = 14073,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Diagnosis)]
    [Retargeted(SGE.Diagnosis)]
    SGE_Retarget_Diagnosis = 14079,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_EukrasianDiagnosis)]
    [Retargeted(SGE.EukrasianDiagnosis)]
    SGE_Retarget_EukrasianDiagnosis = 14080,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Haima)]
    [Retargeted(SGE.Haima)]
    SGE_Retarget_Haima = 14074,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Druochole)]
    [Retargeted(SGE.Druochole)]
    SGE_Retarget_Druochole = 14075,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Taurochole)]
    [Retargeted(SGE.Taurochole)]
    SGE_Retarget_Taurochole = 14076,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Krasis)]
    [Retargeted(SGE.Krasis)]
    SGE_Retarget_Krasis = 14077,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Kardia)]
    [Retargeted(SGE.Kardia)]
    SGE_Retarget_Kardia = 14078,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo(Job.SGE, SGE_Retarget_Icarus)]
    [Retargeted(SGE.Icarus)]
    SGE_Retarget_Icarus = 14083,

    #endregion

    #region Raidwide Features
    [CustomComboInfo(Job.SGE, SGE_Raidwide)]
    SGE_Raidwide = 14069,

    [ParentCombo(SGE_Raidwide)]
    [CustomComboInfo(Job.SGE, SGE_Raidwide_EPrognosis)]
    SGE_Raidwide_EPrognosis = 14070,

    [ParentCombo(SGE_Raidwide)]
    [CustomComboInfo(Job.SGE, SGE_Raidwide_Kerachole)]
    SGE_Raidwide_Kerachole = 14071,

    [ParentCombo(SGE_Raidwide)]
    [CustomComboInfo(Job.SGE, SGE_Raidwide_Holos)]
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
    [CustomComboInfo(Job.SAM, SAM_ST_SimpleMode)]
    [SimpleCombo]
    SAM_ST_SimpleMode = 15002,

    [AutoAction(true, false)]
    [ReplaceSkill(SAM.Fuga, SAM.Fuko)]
    [ConflictingCombos(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_SimpleMode)]
    [SimpleCombo]
    SAM_AoE_SimpleMode = 15102,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(SAM.Hakaze, SAM.Gyofu)]
    [ConflictingCombos(SAM_ST_SimpleMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_AdvancedMode)]
    [AdvancedCombo]
    SAM_ST_AdvancedMode = 15003,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Opener)]
    SAM_ST_Opener = 15006,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Yukikaze)]
    SAM_ST_Yukikaze = 15004,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Kasha)]
    SAM_ST_Kasha = 15005,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Gekko)]
    SAM_ST_Gekko = 15022,

    #region cooldowns on Main Combo

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs)]
    SAM_ST_CDs = 15011,

    [ParentCombo(SAM_ST_CDs)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_MeikyoShisui)]
    SAM_ST_CDs_MeikyoShisui = 15018,

    [ParentCombo(SAM_ST_CDs)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_Ikishoten)]
    SAM_ST_CDs_Ikishoten = 15012,

    #endregion

    #region Damage skills

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Damage)]
    SAM_ST_Damage = 15023,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_Iaijutsu)]
    SAM_ST_CDs_Iaijutsu = 15013,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_UseHiganbana)]
    SAM_ST_CDs_UseHiganbana = 15024,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_UseTenkaGoken)]
    SAM_ST_CDs_UseTenkaGoken = 15025,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_UseMidare)]
    SAM_ST_CDs_UseMidare = 15026,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_UseTsubame)]
    SAM_ST_CDs_UseTsubame = 15027,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_Iaijutsu_Movement)]
    SAM_ST_CDs_Iaijutsu_Movement = 15014,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_Senei)]
    SAM_ST_CDs_Senei = 15020,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_OgiNamikiri)]
    SAM_ST_CDs_OgiNamikiri = 15015,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_Zanshin)]
    SAM_ST_CDs_Zanshin = 15017,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_CDs_Shoha)]
    SAM_ST_CDs_Shoha = 15019,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_Shinten)]
    SAM_ST_Shinten = 15008,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo(Job.SAM, SAM_ST_RangedUptime)]
    SAM_ST_RangedUptime = 15097,

    #endregion

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_TrueNorth)]
    SAM_ST_TrueNorth = 15099,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_StunInterupt)]
    SAM_ST_StunInterupt = 15096,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_ComboHeals)]
    SAM_ST_ComboHeals = 15098,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Feint)]
    SAM_ST_Feint = 15095,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_ThirdEye)]
    SAM_ST_ThirdEye = 15094,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_ST_Meditate)]
    SAM_ST_Meditate = 15093,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(SAM.Fuga, SAM.Fuko)]
    [ConflictingCombos(SAM_AoE_SimpleMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_AdvancedMode)]
    [AdvancedCombo]
    SAM_AoE_AdvancedMode = 15103,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Oka)]
    SAM_AoE_Oka = 15104,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Hagakure)]
    SAM_AoE_Hagakure = 15113,

    #region Cooldowns on Main Combo

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_CDs)]
    SAM_AoE_CDs = 15115,

    [ParentCombo(SAM_AoE_CDs)]
    [CustomComboInfo(Job.SAM, SAM_AoE_MeikyoShisui)]
    SAM_AoE_MeikyoShisui = 15114,

    [ParentCombo(SAM_AoE_CDs)]
    [CustomComboInfo(Job.SAM, SAM_AOE_CDs_Ikishoten)]
    SAM_AOE_CDs_Ikishoten = 15108,

    #endregion

    #region Damage Skills

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Damage)]
    SAM_AoE_Damage = 15116,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo(Job.SAM, SAM_AoE_TenkaGoken)]
    SAM_AoE_TenkaGoken = 15107,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Guren)]
    SAM_AoE_Guren = 15112,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo(Job.SAM, SAM_AoE_OgiNamikiri)]
    SAM_AoE_OgiNamikiri = 15109,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Zanshin)]
    SAM_AoE_Zanshin = 15110,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Shoha)]
    SAM_AoE_Shoha = 15111,

    #endregion

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_Kyuten)]
    SAM_AoE_Kyuten = 15105,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_StunInterupt)]
    SAM_AoE_StunInterupt = 15196,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo(Job.SAM, SAM_AoE_ComboHeals)]
    SAM_AoE_ComboHeals = 15199,

    #endregion

    #region Basic Combo

    [ReplaceSkill(SAM.Yukikaze)]
    [CustomComboInfo(Job.SAM, SAM_ST_YukikazeCombo)]
    SAM_ST_YukikazeCombo = 15000,

    [ReplaceSkill(SAM.Kasha)]
    [CustomComboInfo(Job.SAM, SAM_ST_KashaCombo)]
    SAM_ST_KashaCombo = 15001,

    [ReplaceSkill(SAM.Gekko)]
    [CustomComboInfo(Job.SAM, SAM_ST_GekkoCombo)]
    SAM_ST_GekkoCombo = 15010,

    [ReplaceSkill(SAM.Oka)]
    [CustomComboInfo(Job.SAM, SAM_AoE_OkaCombo)]
    SAM_AoE_OkaCombo = 15100,

    [ReplaceSkill(SAM.Mangetsu)]
    [CustomComboInfo(Job.SAM, SAM_AoE_MangetsuCombo)]
    SAM_AoE_MangetsuCombo = 15101,

    #endregion

    #region Meikyo Features

    [ReplaceSkill(SAM.MeikyoShisui)]
    [ConflictingCombos(SAM_MeikyoShisuiProtection)]
    [CustomComboInfo(Job.SAM, SAM_MeikyoSens)]
    SAM_MeikyoSens = 15200,

    [ReplaceSkill(SAM.MeikyoShisui)]
    [ConflictingCombos(SAM_MeikyoSens)]
    [CustomComboInfo(Job.SAM, SAM_MeikyoShisuiProtection)]
    SAM_MeikyoShisuiProtection = 15214,

    #endregion

    #region Iaijutsu Features

    [ReplaceSkill(SAM.Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_Iaijutsu)]
    SAM_Iaijutsu = 15201,

    [ParentCombo(SAM_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_Iaijutsu_TsubameGaeshi)]
    SAM_Iaijutsu_TsubameGaeshi = 15202,

    [ParentCombo(SAM_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_Iaijutsu_Shoha)]
    SAM_Iaijutsu_Shoha = 15203,

    [ParentCombo(SAM_Iaijutsu)]
    [CustomComboInfo(Job.SAM, SAM_Iaijutsu_OgiNamikiri)]
    SAM_Iaijutsu_OgiNamikiri = 15204,

    #endregion

    #region Shinten Features

    [ReplaceSkill(SAM.Shinten)]
    [CustomComboInfo(Job.SAM, SAM_Shinten)]
    SAM_Shinten = 15251,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo(Job.SAM, SAM_Shinten_Shoha)]
    SAM_Shinten_Shoha = 15205,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo(Job.SAM, SAM_Shinten_Senei)]
    SAM_Shinten_Senei = 15206,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo(Job.SAM, SAM_Shinten_Zanshin)]
    SAM_Shinten_Zanshin = 15207,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo(Job.SAM, SAM_Shinten_Ikishoten)]
    SAM_Shinten_Ikishoten = 15256,

    #endregion

    #region Kyuten Features

    [ReplaceSkill(SAM.Kyuten)]
    [CustomComboInfo(Job.SAM, SAM_Kyuten)]
    SAM_Kyuten = 15252,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo(Job.SAM, SAM_Kyuten_Shoha)]
    SAM_Kyuten_Shoha = 15208,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo(Job.SAM, SAM_Kyuten_Guren)]
    SAM_Kyuten_Guren = 15209,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo(Job.SAM, SAM_Kyuten_Zanshin)]
    SAM_Kyuten_Zanshin = 15210,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo(Job.SAM, SAM_Kyuten_Ikishoten)]
    SAM_Kyuten_Ikishoten = 15257,

    #endregion

    #region Ikishoten Features

    [ReplaceSkill(SAM.Ikishoten)]
    [CustomComboInfo(Job.SAM, SAM_Ikishoten)]
    SAM_Ikishoten = 15253,

    [ParentCombo(SAM_Ikishoten)]
    [CustomComboInfo(Job.SAM, SAM_Ikishoten_Namikiri)]
    SAM_Ikishoten_Namikiri = 15212,

    [ParentCombo(SAM_Ikishoten)]
    [CustomComboInfo(Job.SAM, SAM_Ikishoten_Shoha)]
    SAM_Ikishoten_Shoha = 15213,

    #endregion

    #region Other

    [ReplaceSkill(SAM.Gyoten)]
    [CustomComboInfo(Job.SAM, SAM_GyotenYaten)]
    SAM_GyotenYaten = 15211,

    [ReplaceSkill(SAM.Senei)]
    [CustomComboInfo(Job.SAM, SAM_SeneiGuren)]
    SAM_SeneiGuren = 15215,

    [ReplaceSkill(SAM.OgiNamikiri)]
    [CustomComboInfo(Job.SAM, SAM_OgiShoha)]
    SAM_OgiShoha = 15258,

    #endregion

    #region Hidden Features

    [CustomComboInfo(Job.SAM, SAM_Hidden)]
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
    [CustomComboInfo(Job.SCH, SCH_ST_Simple_DPS)]
    SCH_ST_Simple_DPS = 16070,


    [AutoAction(true, false)]
    [ReplaceSkill(SCH.ArtOfWar, SCH.ArtOfWarII)]
    [SimpleCombo]
    [ConflictingCombos(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Simple_DPS)]
    SCH_AoE_Simple_DPS = 16071,

    [AutoAction(false, true)]
    [ReplaceSkill(SCH.Physick)]
    [ConflictingCombos(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_Simple_ST_Heal)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SCH_Simple_ST_Heal = 16085,


    [AutoAction(true, true)]
    [ReplaceSkill(SCH.Succor)]
    [ConflictingCombos(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_Simple_AoE_Heal)]
    [SimpleCombo]
    [PossiblyRetargeted]
    SCH_Simple_AoE_Heal = 16084,

    #endregion

    #region ST DPS
    [AutoAction(false, false)]
    [ReplaceSkill(SCH.Ruin, SCH.Broil, SCH.Broil2, SCH.Broil3, SCH.Broil4, SCH.Bio, SCH.Bio2, SCH.Biolysis)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS)]
    [AdvancedCombo]
    [ConflictingCombos(SCH_ST_Simple_DPS)]
    SCH_ST_ADV_DPS = 16001,


    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_Balance_Opener)]
    SCH_ST_ADV_DPS_Balance_Opener = 16009,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_Bio)]
    SCH_ST_ADV_DPS_Bio = 16008,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_Aetherflow)]
    SCH_ST_ADV_DPS_Aetherflow = 16004,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_EnergyDrain)]
    SCH_ST_ADV_DPS_EnergyDrain = 16005,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_ChainStrat)]
    SCH_ST_ADV_DPS_ChainStrat = 16003,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_BanefulImpact)]
    SCH_ST_ADV_DPS_BanefulImpact = 16052,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_Ruin2Movement)]
    SCH_ST_ADV_DPS_Ruin2Movement = 16007,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_FairyReminder)]
    SCH_ST_ADV_DPS_FairyReminder = 16048,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_ST_ADV_DPS_Lucid)]
    SCH_ST_ADV_DPS_Lucid = 16002,

    #endregion

    #region AoE DPS
    [AutoAction(true, false)]
    [ReplaceSkill(SCH.ArtOfWar, SCH.ArtOfWarII)]
    [ConflictingCombos(SCH_AoE_Simple_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS)]
    [AdvancedCombo]
    SCH_AoE_ADV_DPS = 16010,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_DoT)]
    [Retargeted(SCH.Bio, SCH.Bio2, SCH.Biolysis)]
    SCH_AoE_ADV_DPS_DoT = 16072,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_EnergyDrain)]
    SCH_AoE_ADV_DPS_EnergyDrain = 16056,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_ChainStrat)]
    SCH_AoE_ADV_DPS_ChainStrat = 16054,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_BanefulImpact)]
    SCH_AoE_ADV_DPS_BanefulImpact = 16053,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_FairyReminder)]
    SCH_AoE_ADV_DPS_FairyReminder = 16049,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_Lucid)]
    SCH_AoE_ADV_DPS_Lucid = 16011,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo(Job.SCH, SCH_AoE_ADV_DPS_Aetherflow)]
    SCH_AoE_ADV_DPS_Aetherflow = 16012,

    #endregion

    #region  ST Healing
    [AutoAction(false, true)]
    [ReplaceSkill(SCH.Physick)]
    [ConflictingCombos(SCH_Simple_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal)]
    [PossiblyRetargeted(SCH.Physick)]
    [HealingCombo]
    SCH_ST_Heal = 16023,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Lucid)]
    SCH_ST_Heal_Lucid = 16024,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Aetherflow)]
    SCH_ST_Heal_Aetherflow = 16025,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Dissipation)]
    SCH_ST_Heal_Dissipation = 16040,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Esuna)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    SCH_ST_Heal_Esuna = 16026,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Adloquium)]
    [PossiblyRetargeted(SCH.Adloquium)]
    SCH_ST_Heal_Adloquium = 16027,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Lustrate)]
    [PossiblyRetargeted(SCH.Lustrate)]
    SCH_ST_Heal_Lustrate = 16028,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Excogitation)]
    [PossiblyRetargeted(SCH.Excogitation)]
    SCH_ST_Heal_Excogitation = 16038,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Protraction)]
    [PossiblyRetargeted(SCH.Protraction)]
    SCH_ST_Heal_Protraction = 16039,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Aetherpact)]
    [PossiblyRetargeted(SCH.Aetherpact)]
    SCH_ST_Heal_Aetherpact = 16047,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_WhisperingDawn)]
    SCH_ST_Heal_WhisperingDawn = 16067,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_FeyIllumination)]
    SCH_ST_Heal_FeyIllumination = 16068,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_FeyBlessing)]
    SCH_ST_Heal_FeyBlessing = 16069,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Seraphism)]
    SCH_ST_Heal_Seraphism = 16086,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Expedient)]
    SCH_ST_Heal_Expedient = 16087,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_SummonSeraph)]
    SCH_ST_Heal_SummonSeraph = 16088,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo(Job.SCH, SCH_ST_Heal_Consolation)]
    SCH_ST_Heal_Consolation = 16089,
    #endregion

    #region AoE Healing
    [AutoAction(true, true)]
    [ReplaceSkill(SCH.Succor)]
    [ConflictingCombos(SCH_Simple_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal)]
    [HealingCombo]
    SCH_AoE_Heal = 16018,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_Indomitability)]
    SCH_AoE_Heal_Indomitability = 16022,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_WhisperingDawn)]
    SCH_AoE_Heal_WhisperingDawn = 16043,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_FeyIllumination)]
    SCH_AoE_Heal_FeyIllumination = 16042,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_FeyBlessing)]
    SCH_AoE_Heal_FeyBlessing = 16045,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_Seraphism)]
    SCH_AoE_Heal_Seraphism = 16044,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_SummonSeraph)]
    SCH_AoE_Heal_SummonSeraph = 16063,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_Consolation)]
    SCH_AoE_Heal_Consolation = 16046,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_Aetherflow)]
    SCH_AoE_Heal_Aetherflow = 16020,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_Dissipation)]
    SCH_AoE_Heal_Dissipation = 16041,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo(Job.SCH, SCH_AoE_Heal_Lucid)]
    SCH_AoE_Heal_Lucid = 16019,

    #endregion

    #region Utilities

    [ReplaceSkill(SCH.EnergyDrain, SCH.Lustrate, SCH.SacredSoil, SCH.Indomitability, SCH.Excogitation)]
    [CustomComboInfo(Job.SCH, SCH_Aetherflow)]
    SCH_Aetherflow = 16029,

    [ParentCombo(SCH_Aetherflow)]
    [CustomComboInfo(Job.SCH, SCH_Aetherflow_Dissipation)]
    SCH_Aetherflow_Dissipation = 16031,

    [ParentCombo(SCH_Aetherflow)]
    [CustomComboInfo(Job.SCH, SCH_Aetherflow_Recite)]
    SCH_Aetherflow_Recite = 16030,

    [ReplaceSkill(SCH.Lustrate)]
    [CustomComboInfo(Job.SCH, SCH_Lustrate)]
    [PossiblyRetargeted("Retargeting Features below, Enable Lustrate and Excogitation", Condition.SCHRetargetingFeaturesEnabledForLustcog)]
    SCH_Lustrate = 16014,

    [ReplaceSkill(SCH.Recitation)]
    [CustomComboInfo(Job.SCH, SCH_Recitation)]
    [PossiblyRetargeted("Retargeting Features below, Enable Adloquium and Excogitation", Condition.SCHRetargetingFeaturesEnabledForAdlocog)]
    SCH_Recitation = 16015,

    [ReplaceSkill(SCH.DeploymentTactics)]
    [CustomComboInfo(Job.SCH, SCH_DeploymentTactics)]
    [PossiblyRetargeted("Retargeting Features below, Enable Adloquium and Deployment Tactics", Condition.SCHRetargetingFeaturesEnabledForAdloDeployment)]
    SCH_DeploymentTactics = 16034,

    [ParentCombo(SCH_DeploymentTactics)]
    [CustomComboInfo(Job.SCH, SCH_DeploymentTactics_Recitation)]
    SCH_DeploymentTactics_Recitation = 16035,

    [ReplaceSkill(SCH.WhisperingDawn, SCH.FeyIllumination, SCH.FeyBlessing, SCH.Aetherpact, SCH.Dissipation,
        SCH.SummonSeraph)]
    [CustomComboInfo(Job.SCH, SCH_FairyReminder)]
    SCH_FairyReminder = 16033,

    [ReplaceSkill(SCH.FeyBlessing)]
    [CustomComboInfo(Job.SCH, SCH_Consolation)]
    SCH_Consolation = 16013,

    [ReplaceSkill(SCH.WhisperingDawn)]
    [CustomComboInfo(Job.SCH, SCH_Fairy_Combo)]
    SCH_Fairy_Combo = 16016,

    [ParentCombo(SCH_Fairy_Combo)]
    [CustomComboInfo(Job.SCH, SCH_Fairy_Combo_Consolation)]
    SCH_Fairy_Combo_Consolation = 16017,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo(Job.SCH, SCH_Raise)]
    SCH_Raise = 16032,

    [ParentCombo(SCH_Raise)]
    [CustomComboInfo(Job.SCH, SCH_Raise_Retarget)]
    [Retargeted(SCH.Resurrection)]
    SCH_Raise_Retarget = 16050,

    #endregion

    #region Mitigation Features

    [ReplaceSkill(SCH.Protraction)]
    [CustomComboInfo(Job.SCH, SCH_Mit_ST)]
    [PossiblyRetargeted("Retargeting Features below, Enable Protraction and Adloquium (and optionally Deployment Tactics and Excogitation)", Condition.SCHRetargetingFeaturesEnabledForSTMit)]
    SCH_Mit_ST = 16083,

    [ReplaceSkill(SCH.SacredSoil)]
    [CustomComboInfo(Job.SCH, SCH_Mit_AoE)]
    [PossiblyRetargeted("Retargeting Features below, Enable Sacred Soil", Condition.SCHRetargetingFeaturesEnabledForAoEMit)]
    SCH_Mit_AoE = 16082,

    #endregion

    #region Standalone Healing option

    [CustomComboInfo(Job.SCH, SCH_Retarget)]
    [Retargeted]
    SCH_Retarget = 16073,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_Physick)]
    [Retargeted(SCH.Physick)]
    SCH_Retarget_Physick = 16074,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_Adloquium)]
    [Retargeted(SCH.Adloquium)]
    SCH_Retarget_Adloquium = 16081,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_Lustrate)]
    [Retargeted(SCH.Lustrate)]
    SCH_Retarget_Lustrate = 16075,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_Protraction)]
    [Retargeted(SCH.Protraction)]
    SCH_Retarget_Protraction = 16076,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_DeploymentTactics)]
    [Retargeted(SCH.DeploymentTactics)]
    SCH_Retarget_DeploymentTactics = 16077,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_Excogitation)]
    [Retargeted(SCH.Excogitation)]
    SCH_Retarget_Excogitation = 16078,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_Aetherpact)]
    [Retargeted(SCH.Aetherpact)]
    SCH_Retarget_Aetherpact = 16079,

    [ParentCombo(SCH_Retarget)]
    [CustomComboInfo(Job.SCH, SCH_Retarget_SacredSoil)]
    [Retargeted(SCH.SacredSoil)]
    SCH_Retarget_SacredSoil = 16080,

    #endregion

    #region Raidwide Features
    [CustomComboInfo(Job.SCH, SCH_Raidwide)]
    SCH_Raidwide = 16065,

    [ParentCombo(SCH_Raidwide)]
    [CustomComboInfo(Job.SCH, SCH_Raidwide_Succor)]
    SCH_Raidwide_Succor = 16062,

    [ParentCombo(SCH_Raidwide)]
    [CustomComboInfo(Job.SCH, SCH_Raidwide_SacredSoil)]
    [Retargeted(SCH.SacredSoil)]
    SCH_Raidwide_SacredSoil = 16059,

    [ParentCombo(SCH_Raidwide)]
    [CustomComboInfo(Job.SCH, SCH_Raidwide_Expedient)]
    SCH_Raidwide_Expedient = 16064,
    #endregion

    // Last value = 16089

    #endregion

    #region SUMMONER

    #region Simple Modes

    [AutoAction(false, false)]
    [ConflictingCombos(SMN_ST_Advanced_Combo)]
    [ReplaceSkill(SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
    [CustomComboInfo(Job.SMN, SMN_ST_Simple_Combo)]
    [SimpleCombo]
    SMN_ST_Simple_Combo = 17041,

    [AutoAction(true, false)]
    [ConflictingCombos(SMN_AoE_Advanced_Combo)]
    [ReplaceSkill(SMN.Outburst)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Simple_Combo)]
    [SimpleCombo]
    SMN_AoE_Simple_Combo = 17066,

    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ReplaceSkill(SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
    [ConflictingCombos(SMN_ST_Simple_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo)]
    [AdvancedCombo]
    SMN_ST_Advanced_Combo = 17000,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Balance_Opener)]
    SMN_ST_Advanced_Combo_Balance_Opener = 170001,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_DemiSummons)]
    SMN_ST_Advanced_Combo_DemiSummons = 17020,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_DemiSummons_Attacks)]
    SMN_ST_Advanced_Combo_DemiSummons_Attacks = 17002,

    [ParentCombo(SMN_ST_Advanced_Combo_DemiSummons_Attacks)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_DemiSummons_Rekindle)]
    SMN_ST_Advanced_Combo_DemiSummons_Rekindle = 17028,

    [ParentCombo(SMN_ST_Advanced_Combo_DemiSummons_Rekindle)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_DemiSummons_Rekindle_Retarget)]
    [Retargeted(SMN.Rekindle)]
    SMN_ST_Advanced_Combo_DemiSummons_Rekindle_Retarget = 17080,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Titan)]
    SMN_ST_Advanced_Combo_Titan = 17073,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Garuda)]
    SMN_ST_Advanced_Combo_Garuda = 17074,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Ifrit)]
    SMN_ST_Advanced_Combo_Ifrit = 17075,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_EgiSummons_Attacks)]
    SMN_ST_Advanced_Combo_EgiSummons_Attacks = 17004,

    [ParentCombo(SMN_ST_Advanced_Combo_EgiSummons_Attacks)]
    [CustomComboInfo(Job.SMN, SMN_ST_Ruin3_Emerald_Ruin3)]
    SMN_ST_Ruin3_Emerald_Ruin3 = 17067,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Egi_AstralFlow)]
    SMN_ST_Advanced_Combo_Egi_AstralFlow = 17048,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi)]
    SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi = 17023,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_EDFester)]
    SMN_ST_Advanced_Combo_EDFester = 17014,

    [ParentCombo(SMN_ST_Advanced_Combo_EDFester)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_oGCDPooling)]
    SMN_ST_Advanced_Combo_oGCDPooling = 17025,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_SearingLight)]
    SMN_ST_Advanced_Combo_SearingLight = 17017,

    [ParentCombo(SMN_ST_Advanced_Combo_SearingLight)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_SearingLight_Burst)]
    SMN_ST_Advanced_Combo_SearingLight_Burst = 17018,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_SearingFlash)]
    SMN_ST_Advanced_Combo_SearingFlash = 17019,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Ruin4)]
    SMN_ST_Advanced_Combo_Ruin4 = 17011,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris)]
    SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris = 17029,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Radiant)]
    SMN_ST_Advanced_Combo_Radiant = 17071,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Lucid)]
    SMN_ST_Advanced_Combo_Lucid = 17031,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_ST_Advanced_Combo_Addle)]
    SMN_ST_Advanced_Combo_Addle = 17082,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(SMN.Outburst, SMN.Tridisaster)]
    [ConflictingCombos(SMN_AoE_Simple_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo)]
    [AdvancedCombo]
    SMN_AoE_Advanced_Combo = 17049,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_DemiSummons)]
    SMN_AoE_Advanced_Combo_DemiSummons = 17061,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_DemiSummons_Attacks)]
    SMN_AoE_Advanced_Combo_DemiSummons_Attacks = 17055,

    [ParentCombo(SMN_AoE_Advanced_Combo_DemiSummons_Attacks)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_DemiSummons_Rekindle)]
    SMN_AoE_Advanced_Combo_DemiSummons_Rekindle = 17056,

    [ParentCombo(SMN_AoE_Advanced_Combo_DemiSummons_Rekindle)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_DemiSummons_Rekindle_Retarget)]
    [Retargeted(SMN.Rekindle)]
    SMN_AoE_Advanced_Combo_DemiSummons_Rekindle_Retarget = 17081,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Titan)]
    SMN_AoE_Advanced_Combo_Titan = 17076,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Garuda)]
    SMN_AoE_Advanced_Combo_Garuda = 17077,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Ifrit)]
    SMN_AoE_Advanced_Combo_Ifrit = 17078,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_EgiSummons_Attacks)]
    SMN_AoE_Advanced_Combo_EgiSummons_Attacks = 17064,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Egi_AstralFlow)]
    SMN_AoE_Advanced_Combo_Egi_AstralFlow = 17068,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi)]
    SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi = 17063,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_ESPainflare)]
    SMN_AoE_Advanced_Combo_ESPainflare = 17051,

    [ParentCombo(SMN_AoE_Advanced_Combo_ESPainflare)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_oGCDPooling)]
    SMN_AoE_Advanced_Combo_oGCDPooling = 17050,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_SearingLight)]
    SMN_AoE_Advanced_Combo_SearingLight = 17053,

    [ParentCombo(SMN_AoE_Advanced_Combo_SearingLight)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_SearingLight_Burst)]
    SMN_AoE_Advanced_Combo_SearingLight_Burst = 17054,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_SearingFlash)]
    SMN_AoE_Advanced_Combo_SearingFlash = 17058,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Ruin4)]
    SMN_AoE_Advanced_Combo_Ruin4 = 17062,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris)]
    SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris = 17059,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Radiant)]
    SMN_AoE_Advanced_Combo_Radiant = 17070,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo(Job.SMN, SMN_AoE_Advanced_Combo_Lucid)]
    SMN_AoE_Advanced_Combo_Lucid = 17060,
    #endregion

    #region Standalone Features
    [ReplaceSkill(SMN.Fester)]
    [CustomComboInfo(Job.SMN, SMN_EDFester)]
    SMN_EDFester = 17008,

    [ParentCombo(SMN_EDFester)]
    [CustomComboInfo(Job.SMN, SMN_EDFester_Ruin4)]
    SMN_EDFester_Ruin4 = 17013,

    [ReplaceSkill(SMN.Painflare)]
    [CustomComboInfo(Job.SMN, SMN_ESPainflare)]
    SMN_ESPainflare = 17009,

    [CustomComboInfo(Job.SMN, SMN_CarbuncleReminder)]
    SMN_CarbuncleReminder = 17010,

    [CustomComboInfo(Job.SMN, SMN_DemiAbilities)]
    SMN_DemiAbilities = 17024,

    [ConflictingCombos(ALL_Caster_Raise)]
    [CustomComboInfo(Job.SMN, SMN_Raise)]
    SMN_Raise = 17027,

    [ParentCombo(SMN_Raise)]
    [CustomComboInfo(Job.SMN, SMN_Raise_Retarget)]
    [Retargeted(SMN.Resurrection)]
    SMN_Raise_Retarget = 17079,

    [ReplaceSkill(SMN.Ruin4)]
    [CustomComboInfo(Job.SMN, SMN_RuinMobility)]
    SMN_RuinMobility = 17030,

    [CustomComboInfo(Job.SMN, SMN_Egi_AstralFlow)]
    SMN_Egi_AstralFlow = 17034,

    [ParentCombo(SMN_ESPainflare)]
    [CustomComboInfo(Job.SMN, SMN_ESPainflare_Ruin4)]
    SMN_ESPainflare_Ruin4 = 17039,

    [CustomComboInfo(Job.SMN, SMN_Searing)]
    SMN_Searing = 17072,
    #endregion

    // Last Used 17080

    #endregion

    #region VIPER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(VPR.SteelFangs)]
    [ConflictingCombos(VPR_ST_AdvancedMode, VPR_SerpentsTail, VPR_Legacies)]
    [CustomComboInfo(Job.VPR, VPR_ST_SimpleMode)]
    [SimpleCombo]
    VPR_ST_SimpleMode = 30000,

    [AutoAction(true, false)]
    [ReplaceSkill(VPR.SteelMaw)]
    [ConflictingCombos(VPR_AoE_AdvancedMode, VPR_SerpentsTail)]
    [CustomComboInfo(Job.VPR, VPR_AoE_SimpleMode)]
    [SimpleCombo]
    VPR_AoE_SimpleMode = 30100,

    #endregion

    #region Advanced ST Viper

    [AutoAction(false, false)]
    [ReplaceSkill(VPR.SteelFangs)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_SerpentsTail, VPR_Legacies)]
    [CustomComboInfo(Job.VPR, VPR_ST_AdvancedMode)]
    [AdvancedCombo]
    VPR_ST_AdvancedMode = 30001,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_Opener)]
    VPR_ST_Opener = 30002,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_SerpentsIre)]
    VPR_ST_SerpentsIre = 30005,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_Vicewinder)]
    VPR_ST_Vicewinder = 30006,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_VicewinderCombo)]
    VPR_ST_VicewinderCombo = 30007,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_VicewinderWeaves)]
    VPR_ST_VicewinderWeaves = 30013,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_SerpentsTail)]
    VPR_ST_SerpentsTail = 30008,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_UncoiledFury)]
    VPR_ST_UncoiledFury = 30009,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_UncoiledFuryCombo)]
    VPR_ST_UncoiledFuryCombo = 30010,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_Reawaken)]
    VPR_ST_Reawaken = 30011,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_GenerationCombo)]
    VPR_ST_GenerationCombo = 30012,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_LegacyWeaves)]
    VPR_ST_LegacyWeaves = 30014,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_TrueNorthDynamic)]
    VPR_TrueNorthDynamic = 30098,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_StunInterupt)]
    VPR_ST_StunInterupt = 30096,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_RangedUptime)]
    VPR_ST_RangedUptime = 30095,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_ComboHeals)]
    VPR_ST_ComboHeals = 30097,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_ST_Feint)]
    VPR_ST_Feint = 30094,

    #endregion

    #region Advanced AoE Viper

    [AutoAction(true, false)]
    [ReplaceSkill(VPR.SteelMaw)]
    [ConflictingCombos(VPR_AoE_SimpleMode, VPR_SerpentsTail)]
    [CustomComboInfo(Job.VPR, VPR_AoE_AdvancedMode)]
    [AdvancedCombo]
    VPR_AoE_AdvancedMode = 30101,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_SerpentsIre)]
    VPR_AoE_SerpentsIre = 30104,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_Vicepit)]
    VPR_AoE_Vicepit = 30105,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_VicepitCombo)]
    VPR_AoE_VicepitCombo = 30106,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_VicepitWeaves)]
    VPR_AoE_VicepitWeaves = 30115,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_SerpentsTail)]
    VPR_AoE_SerpentsTail = 30107,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_UncoiledFury)]
    VPR_AoE_UncoiledFury = 30108,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_UncoiledFuryCombo)]
    VPR_AoE_UncoiledFuryCombo = 30109,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_Reawaken)]
    VPR_AoE_Reawaken = 30110,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_ReawakenCombo)]
    VPR_AoE_ReawakenCombo = 30112,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_StunInterupt)]
    VPR_AoE_StunInterupt = 30196,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo(Job.VPR, VPR_AoE_ComboHeals)]
    VPR_AoE_ComboHeals = 30199,

    #endregion

    #region Basic combo

    [ReplaceSkill(VPR.ReavingFangs)]
    [ConflictingCombos(VPR_ReawakenLegacy, VPR_Legacies, VPR_SerpentsTail)]
    [CustomComboInfo(Job.VPR, VPR_ST_BasicCombo)]
    [BasicCombo]
    VPR_ST_BasicCombo = 30015,

    #endregion

    #region Movement

    [CustomComboInfo(Job.VPR, VPR_Retarget_Slither)]
    [Retargeted(VPR.Slither)]
    VPR_Retarget_Slither = 30211,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(VPR.Vicewinder)]
    [ConflictingCombos(VPR_VicewinderProtection)]
    [CustomComboInfo(Job.VPR, VPR_VicewinderCoils)]
    VPR_VicewinderCoils = 30200,

    [ParentCombo(VPR_VicewinderCoils)]
    [CustomComboInfo(Job.VPR, VPR_VicewinderCoils_oGCDs)]
    VPR_VicewinderCoils_oGCDs = 30206,

    [ReplaceSkill(VPR.Vicepit)]
    [ConflictingCombos(VPR_VicewinderProtection)]
    [CustomComboInfo(Job.VPR, VPR_VicepitDens)]
    VPR_VicepitDens = 30201,

    [ParentCombo(VPR_VicepitDens)]
    [CustomComboInfo(Job.VPR, VPR_VicepitDens_oGCDs)]
    VPR_VicepitDens_oGCDs = 30207,

    [ReplaceSkill(VPR.UncoiledFury)]
    [CustomComboInfo(Job.VPR, VPR_UncoiledTwins)]
    VPR_UncoiledTwins = 30202,

    [ReplaceSkill(VPR.Reawaken, VPR.ReavingFangs)]
    [ConflictingCombos(VPR_Legacies, VPR_ST_BasicCombo)]
    [CustomComboInfo(Job.VPR, VPR_ReawakenLegacy)]
    VPR_ReawakenLegacy = 30203,

    [ParentCombo(VPR_ReawakenLegacy)]
    [CustomComboInfo(Job.VPR, VPR_ReawakenLegacyWeaves)]
    VPR_ReawakenLegacyWeaves = 30204,

    [ReplaceSkill(VPR.SerpentsTail)]
    [CustomComboInfo(Job.VPR, VPR_TwinTails)]
    VPR_TwinTails = 30205,

    [ReplaceSkill(VPR.SteelFangs, VPR.ReavingFangs, VPR.HuntersCoil, VPR.SwiftskinsCoil)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_ST_AdvancedMode, VPR_SerpentsTail, VPR_ReawakenLegacy, VPR_ST_BasicCombo)]
    [CustomComboInfo(Job.VPR, VPR_Legacies)]
    VPR_Legacies = 30209,

    [ReplaceSkill(VPR.SteelFangs, VPR.ReavingFangs, VPR.SteelMaw, VPR.ReavingMaw)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_AoE_SimpleMode, VPR_ST_AdvancedMode, VPR_AoE_AdvancedMode, VPR_Legacies, VPR_ST_BasicCombo)]
    [CustomComboInfo(Job.VPR, VPR_SerpentsTail)]
    VPR_SerpentsTail = 30210,

    [ReplaceSkill(VPR.Vicewinder, VPR.Vicepit)]
    [ConflictingCombos(VPR_VicepitDens, VPR_VicewinderCoils)]
    [CustomComboInfo(Job.VPR, VPR_VicewinderProtection)]
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
    [CustomComboInfo(Job.WAR, WAR_ST_Simple)]
    [SimpleCombo]
    WAR_ST_Simple = 18000,

    [AutoAction(true, false)]
    [ConflictingCombos(WAR_AoE_Advanced)]
    [ReplaceSkill(WAR.Overpower)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Simple)]
    [SimpleCombo]
    WAR_AoE_Simple = 18001,
    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ConflictingCombos(WAR_ST_Simple)]
    [ReplaceSkill(WAR.HeavySwing)]
    [CustomComboInfo(Job.WAR, WAR_ST_Advanced)]
    [AdvancedCombo]
    WAR_ST_Advanced = 18002,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_BalanceOpener)]
    WAR_ST_BalanceOpener = 18058,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_StormsEye)]
    WAR_ST_StormsEye = 18005,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_InnerRelease)]
    WAR_ST_InnerRelease = 18003,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_FellCleave)]
    WAR_ST_FellCleave = 18006,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_Infuriate)]
    WAR_ST_Infuriate = 18007,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_Onslaught)]
    WAR_ST_Onslaught = 18008,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_Upheaval)]
    WAR_ST_Upheaval = 18009,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_PrimalRend)]
    WAR_ST_PrimalRend = 18013,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_PrimalWrath)]
    WAR_ST_PrimalWrath = 18010,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_PrimalRuination)]
    WAR_ST_PrimalRuination = 18011,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_RangedUptime)]
    WAR_ST_RangedUptime = 18004,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_Interrupt)]
    WAR_ST_Interrupt = 18066,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_ST_Stun)]
    WAR_ST_Stun = 18112,

    #endregion

    #region Advanced AoE
    [AutoAction(true, false)]
    [ConflictingCombos(WAR_AoE_Simple)]
    [ReplaceSkill(WAR.Overpower)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Advanced)]
    [AdvancedCombo]
    WAR_AoE_Advanced = 18016,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_InnerRelease)]
    WAR_AoE_InnerRelease = 18019,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Decimate)]
    WAR_AoE_Decimate = 18023,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Infuriate)]
    WAR_AoE_Infuriate = 18018,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Onslaught)]
    WAR_AoE_Onslaught = 18071,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Orogeny)]
    WAR_AoE_Orogeny = 18012,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_PrimalRend)]
    WAR_AoE_PrimalRend = 18021,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_PrimalWrath)]
    WAR_AoE_PrimalWrath = 18020,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_PrimalRuination)]
    WAR_AoE_PrimalRuination = 18022,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_RangedUptime)]
    WAR_AoE_RangedUptime = 18110,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Interrupt)]
    WAR_AoE_Interrupt = 18067,

    [ParentCombo(WAR_AoE_Interrupt)]
    [CustomComboInfo(Job.WAR, WAR_AoE_Stun)]
    WAR_AoE_Stun = 18068,

    #endregion

    #region Advanced Mitigation
    [CustomComboInfo(Job.WAR, WAR_Mitigation)]
    WAR_Mitigation = 18131,

    [ParentCombo(WAR_Mitigation)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss)]
    WAR_Mitigation_NonBoss = 18132,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_Rampart)]
    WAR_Mitigation_NonBoss_Rampart = 18133,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_ThrillOfBattle)]
    WAR_Mitigation_NonBoss_ThrillOfBattle = 18134,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_Vengeance)]
    WAR_Mitigation_NonBoss_Vengeance = 18135,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_ArmsLength)]
    WAR_Mitigation_NonBoss_ArmsLength = 18136,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_Reprisal)]
    WAR_Mitigation_NonBoss_Reprisal = 18137,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_RawIntuition)]
    WAR_Mitigation_NonBoss_RawIntuition = 18138,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_ShakeItOff)]
    WAR_Mitigation_NonBoss_ShakeItOff = 18139,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_Equilibrium)]
    WAR_Mitigation_NonBoss_Equilibrium = 18140,

    [ParentCombo(WAR_Mitigation_NonBoss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_NonBoss_Holmgang)]
    WAR_Mitigation_NonBoss_Holmgang = 18141,

    [ParentCombo(WAR_Mitigation)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss)]
    WAR_Mitigation_Boss = 18142,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_Equilibrium)]
    WAR_Mitigation_Boss_Equilibrium = 18146,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_RawIntuition_OnCD)]
    WAR_Mitigation_Boss_RawIntuition_OnCD = 18143,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_RawIntuition_TankBuster)]
    WAR_Mitigation_Boss_RawIntuition_TankBuster = 18147,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_Rampart)]
    WAR_Mitigation_Boss_Rampart = 18149,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_Vengeance)]
    WAR_Mitigation_Boss_Vengeance = 18148,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_ThrillOfBattle)]
    WAR_Mitigation_Boss_ThrillOfBattle = 18150,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_Reprisal)]
    WAR_Mitigation_Boss_Reprisal = 18144,

    [ParentCombo(WAR_Mitigation_Boss)]
    [CustomComboInfo(Job.WAR, WAR_Mitigation_Boss_ShakeItOff)]
    WAR_Mitigation_Boss_ShakeItOff = 18145,

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(WAR.ThrillOfBattle)]
    [CustomComboInfo(Job.WAR, WAR_Mit_OneButton)]
    [MitigationCombo]
    WAR_Mit_OneButton = 18045,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Holmgang_Max)]
    WAR_Mit_Holmgang_Max = 18046,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Bloodwhetting)]
    WAR_Mit_Bloodwhetting = 18047,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Equilibrium)]
    WAR_Mit_Equilibrium = 18048,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Reprisal)]
    WAR_Mit_Reprisal = 18049,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_ThrillOfBattle)]
    WAR_Mit_ThrillOfBattle = 18050,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Rampart)]
    WAR_Mit_Rampart = 18051,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_ShakeItOff)]
    WAR_Mit_ShakeItOff = 18052,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_ArmsLength)]
    WAR_Mit_ArmsLength = 18053,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Vengeance)]
    WAR_Mit_Vengeance = 18054,

    [ReplaceSkill(WAR.ShakeItOff)]
    [CustomComboInfo(Job.WAR, WAR_Mit_Party)]
    [MitigationCombo]
    WAR_Mit_Party = 18111,
    #endregion

    #region Misc

    #region Fell Cleave Features
    [ReplaceSkill(WAR.FellCleave)]
    [ConflictingCombos(WAR_InfuriateFellCleave)]
    [CustomComboInfo(Job.WAR, WAR_FC_Features)]
    WAR_FC_Features = 18122,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_InnerRelease)]
    WAR_FC_InnerRelease = 18123,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_Infuriate)]
    WAR_FC_Infuriate = 18124,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_Onslaught)]
    WAR_FC_Onslaught = 18125,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_Upheaval)]
    WAR_FC_Upheaval = 18126,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_PrimalRend)]
    WAR_FC_PrimalRend = 18127,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_PrimalWrath)]
    WAR_FC_PrimalWrath = 18128,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_FC_PrimalRuination)]
    WAR_FC_PrimalRuination = 18129,
    #endregion

    #region Basic Combo
    [ReplaceSkill(WAR.StormsPath)]
    [CustomComboInfo(Job.WAR, WAR_ST_StormsPathCombo)]
    WAR_ST_StormsPathCombo = 18069,

    [ReplaceSkill(WAR.StormsEye)]
    [CustomComboInfo(Job.WAR, WAR_ST_StormsEyeCombo)]
    WAR_ST_StormsEyeCombo = 18070,

    [ReplaceSkill(WAR.MythrilTempest)]
    [CustomComboInfo(Job.WAR, WAR_AoE_BasicCombo)]
    WAR_AoE_BasicCombo = 18151,
    #endregion

    [ReplaceSkill(WAR.FellCleave, WAR.Decimate)]
    [ConflictingCombos(WAR_FC_Features)]
    [CustomComboInfo(Job.WAR, WAR_InfuriateFellCleave)]
    WAR_InfuriateFellCleave = 18024,

    [ParentCombo(WAR_InfuriateFellCleave)]
    [CustomComboInfo(Job.WAR, WAR_InfuriateFellCleave_IRFirst)]
    WAR_InfuriateFellCleave_IRFirst = 18027,

    [ReplaceSkill(WAR.StormsPath)]
    [CustomComboInfo(Job.WAR, WAR_EyePath)]
    WAR_EyePath = 18057,

    [ReplaceSkill(WAR.Berserk, WAR.InnerRelease)]
    [CustomComboInfo(Job.WAR, WAR_PrimalCombo_InnerRelease)]
    WAR_PrimalCombo_InnerRelease = 18026,

    [ReplaceSkill(WAR.Equilibrium)]
    [CustomComboInfo(Job.WAR, WAR_ThrillEquilibrium)]
    WAR_ThrillEquilibrium = 18055,

    [ReplaceSkill(WAR.NascentFlash)]
    [CustomComboInfo(Job.WAR, WAR_NascentFlash)]
    WAR_NascentFlash = 18017,

    [ReplaceSkill(WAR.RawIntuition, WAR.Bloodwhetting)]
    [CustomComboInfo(Job.WAR, WAR_RawIntuition_Targeting)]
    [Retargeted(WAR.NascentFlash)]
    WAR_RawIntuition_Targeting = 18119,

    [ParentCombo(WAR_RawIntuition_Targeting)]
    [CustomComboInfo(Job.WAR, WAR_RawIntuition_Targeting_MO)]
    [Retargeted]
    WAR_RawIntuition_Targeting_MO = 18120,

    [ParentCombo(WAR_RawIntuition_Targeting)]
    [CustomComboInfo(Job.WAR, WAR_RawIntuition_Targeting_TT)]
    [Retargeted]
    WAR_RawIntuition_Targeting_TT = 18121,

    [ReplaceSkill(WAR.Onslaught)]
    [CustomComboInfo(Job.WAR, WAR_RetargetOnslaught)]
    [Retargeted(WAR.Onslaught)]
    WAR_RetargetOnslaught = 18152,

    [ReplaceSkill(RoleActions.Physical.ArmsLength)]
    [CustomComboInfo(Job.WAR, WAR_ArmsLengthLockout)]
    WAR_ArmsLengthLockout = 18153,

    [ReplaceSkill(WAR.Holmgang)]
    [CustomComboInfo(Job.WAR, WAR_RetargetHolmgang)]
    [Retargeted(WAR.Holmgang)]
    WAR_RetargetHolmgang = 18130,

    #endregion
    // Last value = 18153
    #endregion

    #region WHITE MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
    [ConflictingCombos(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_Simple_DPS)]
    [SimpleCombo]
    WHM_ST_Simple_DPS = 19050,

    [AutoAction(true, false)]
    [ReplaceSkill(WHM.Holy, WHM.Holy3)]
    [ConflictingCombos(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_Simple_DPS)]
    [SimpleCombo]
    WHM_AoE_Simple_DPS = 19051,

    [AutoAction(false, true)]
    [ReplaceSkill(WHM.Cure)]
    [ConflictingCombos(WHM_STHeals, WHM_Re_Cure)]
    [CustomComboInfo(Job.WHM, WHM_SimpleSTHeals)]
    [SimpleCombo]
    [PossiblyRetargeted]
    WHM_SimpleSTHeals = 19052,

    [AutoAction(true, true)]
    [ReplaceSkill(WHM.Medica1)]
    [ConflictingCombos(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_Simple_AoEHeals)]
    [SimpleCombo]
    [PossiblyRetargeted]
    WHM_Simple_AoEHeals = 19054,

    #endregion

    #region Advanced Single Target DPS Combo

    [AutoAction(false, false)]
    [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
    [ConflictingCombos(WHM_ST_Simple_DPS)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo)]
    [AdvancedCombo]
    WHM_ST_MainCombo = 19099,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_Opener)]
    WHM_ST_MainCombo_Opener = 19023,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_Move_DoT)]
    [Retargeted(WHM.Aero, WHM.Aero2, WHM.Dia)]
    WHM_ST_MainCombo_Move_DoT = 19053,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_DoT)]
    WHM_ST_MainCombo_DoT = 19013,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_Assize)]
    WHM_ST_MainCombo_Assize = 19009,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_GlareIV)]
    WHM_ST_MainCombo_GlareIV = 19015,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_Misery)]
    WHM_ST_MainCombo_Misery = 19017,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_LilyOvercap)]
    WHM_ST_MainCombo_LilyOvercap = 19016,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_PresenceOfMind)]
    WHM_ST_MainCombo_PresenceOfMind = 19008,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo(Job.WHM, WHM_ST_MainCombo_Lucid)]
    WHM_ST_MainCombo_Lucid = 19006,

    #endregion

    #region Advanced AoE DPS Combo

    [AutoAction(true, false)]
    [ReplaceSkill(WHM.Holy, WHM.Holy3)]
    [ConflictingCombos(WHM_AoE_Simple_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS)]
    [AdvancedCombo]
    WHM_AoE_DPS = 19190,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_SwiftHoly)]
    WHM_AoE_DPS_SwiftHoly = 19197,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_Assize)]
    WHM_AoE_DPS_Assize = 19192,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_GlareIV)]
    WHM_AoE_DPS_GlareIV = 19196,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_Misery)]
    WHM_AoE_DPS_Misery = 19194,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_MainCombo_DoT)]
    [Retargeted(WHM.Aero, WHM.Aero2, WHM.Dia)]
    WHM_AoE_MainCombo_DoT = 19198,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_LilyOvercap)]
    WHM_AoE_DPS_LilyOvercap = 19193,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_PresenceOfMind)]
    WHM_AoE_DPS_PresenceOfMind = 19195,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo(Job.WHM, WHM_AoE_DPS_Lucid)]
    WHM_AoE_DPS_Lucid = 19191,

    #endregion

    #region Advanced Single Target Heals Combo

    [AutoAction(false, true)]
    [ReplaceSkill(WHM.Cure)]
    [ConflictingCombos(WHM_SimpleSTHeals, WHM_Re_Cure)]
    [CustomComboInfo(Job.WHM, WHM_STHeals)]
    [PossiblyRetargeted(WHM.Cure)]
    [HealingCombo]
    WHM_STHeals = 19300,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Esuna)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    WHM_STHeals_Esuna = 19309,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Lucid)]
    WHM_STHeals_Lucid = 19308,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_ThinAir)]
    WHM_STHeals_ThinAir = 19304,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Regen)]
    [PossiblyRetargeted(WHM.Regen)]
    WHM_STHeals_Regen = 19301,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Solace)]
    [PossiblyRetargeted(WHM.AfflatusSolace)]
    WHM_STHeals_Solace = 19303,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Tetragrammaton)]
    [PossiblyRetargeted(WHM.Tetragrammaton)]
    WHM_STHeals_Tetragrammaton = 19305,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Benison)]
    [PossiblyRetargeted(WHM.DivineBenison)]
    WHM_STHeals_Benison = 19306,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Aquaveil)]
    [PossiblyRetargeted(WHM.Aquaveil)]
    WHM_STHeals_Aquaveil = 19307,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Benediction)]
    [PossiblyRetargeted(WHM.Benediction)]
    WHM_STHeals_Benediction = 19302,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Temperance)]
    WHM_STHeals_Temperance = 19310,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_Asylum)]
    [Retargeted(WHM.Asylum)]
    WHM_STHeals_Asylum = 19311,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo(Job.WHM, WHM_STHeals_LiturgyOfTheBell)]
    [PossiblyRetargeted(WHM.LiturgyOfTheBell, WHM.LiturgyOfTheBellRecast)]
    WHM_STHeals_LiturgyOfTheBell = 19312,

    #endregion

    #region Advanced AoE Heals Combo

    [AutoAction(true, true)]
    [ReplaceSkill(WHM.Medica1)]
    [ConflictingCombos(WHM_Simple_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals)]
    [HealingCombo]
    WHM_AoEHeals = 19007,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Lucid)]
    WHM_AoEHeals_Lucid = 19204,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_ThinAir)]
    WHM_AoEHeals_ThinAir = 19200,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Medica2)]
    WHM_AoEHeals_Medica2 = 19205,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Rapture)]
    WHM_AoEHeals_Rapture = 19011,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Cure3)]
    WHM_AoEHeals_Cure3 = 19201,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Assize)]
    WHM_AoEHeals_Assize = 19202,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Plenary)]
    WHM_AoEHeals_Plenary = 19203,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Asylum)]
    [Retargeted(WHM.Asylum)]
    WHM_AoEHeals_Asylum = 19028,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_Temperance)]
    WHM_AoEHeals_Temperance = 19210,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_DivineCaress)]
    WHM_AoEHeals_DivineCaress = 19207,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo(Job.WHM, WHM_AoEHeals_LiturgyOfTheBell)]
    [Retargeted(WHM.LiturgyOfTheBell)]
    WHM_AoEHeals_LiturgyOfTheBell = 19206,

    #endregion

    #region Small Features

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo(Job.WHM, WHM_Raise)]
    WHM_Raise = 19004,

    [ParentCombo(WHM_Raise)]
    [CustomComboInfo(Job.WHM, WHM_Raise_Retarget)]
    [Retargeted(WHM.Raise)]
    WHM_Raise_Retarget = 19029,

    [ReplaceSkill(WHM.Raise)]
    [CustomComboInfo(Job.WHM, WHM_ThinAirRaise)]
    WHM_ThinAirRaise = 19014,

    [ReplaceSkill(WHM.AfflatusRapture)]
    [CustomComboInfo(Job.WHM, WHM_RaptureMisery)]
    WHM_RaptureMisery = 19001,

    [ReplaceSkill(WHM.AfflatusSolace)]
    [CustomComboInfo(Job.WHM, WHM_SolaceMisery)]
    [PossiblyRetargeted("Retargeting Features below, Enable Afflatus Solace",
        Condition.WHMRetargetingFeaturesEnabledForSolace)]
    WHM_SolaceMisery = 19000,

    [ReplaceSkill(WHM.Cure2)]
    [CustomComboInfo(Job.WHM, WHM_CureSync)]
    [PossiblyRetargeted("Retargeting Features below, Enable Cure", Condition.WHMRetargetingFeaturesEnabledForCure)]
    WHM_CureSync = 19002,
    #endregion

    #region Mitigation Features

    [ReplaceSkill(WHM.Aquaveil)]
    [CustomComboInfo(Job.WHM, WHM_Mit_ST)]
    [PossiblyRetargeted("Retargeting Features below, Enable Aquaveil (and optionally Tetra and Benison)", Condition.WHMRetargetingFeaturesEnabledForSTMit)]
    WHM_Mit_ST = 19041,

    [ReplaceSkill(WHM.Asylum)]
    [CustomComboInfo(Job.WHM, WHM_Mit_AoE)]
    [PossiblyRetargeted("Retargeting Features below, Enable Asylum", Condition.WHMRetargetingFeaturesEnabledForAoEMit)]
    WHM_Mit_AoE = 19040,

    #endregion

    #region Retargeting

    [CustomComboInfo(Job.WHM, WHM_Retargets)]
    WHM_Retargets = 19037,

    [ConflictingCombos(WHM_SimpleSTHeals, WHM_STHeals)]
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure)]
    [CustomComboInfo(Job.WHM, WHM_Re_Cure)]
    [Retargeted(WHM.Cure)]
    WHM_Re_Cure = 19038,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure2)]
    [CustomComboInfo(Job.WHM, WHM_Re_Cure2)]
    [Retargeted(WHM.Cure2)]
    WHM_Re_Cure2 = 19055,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.AfflatusSolace)]
    [CustomComboInfo(Job.WHM, WHM_Re_Solace)]
    [Retargeted(WHM.AfflatusSolace)]
    WHM_Re_Solace = 19039,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Aquaveil)]
    [CustomComboInfo(Job.WHM, WHM_Re_Aquaveil)]
    [Retargeted(WHM.Aquaveil)]
    WHM_Re_Aquaveil = 19036,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Asylum)]
    [CustomComboInfo(Job.WHM, WHM_Re_Asylum)]
    [Retargeted(WHM.Asylum)]
    WHM_Re_Asylum = 19027,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.LiturgyOfTheBell)]
    [CustomComboInfo(Job.WHM, WHM_Re_LiturgyOfTheBell)]
    [Retargeted(WHM.LiturgyOfTheBell)]
    WHM_Re_LiturgyOfTheBell = 19030,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure3)]
    [CustomComboInfo(Job.WHM, WHM_Re_Cure3)]
    [Retargeted(WHM.Cure3)]
    WHM_Re_Cure3 = 19031,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Benediction)]
    [CustomComboInfo(Job.WHM, WHM_Re_Benediction)]
    [Retargeted(WHM.Benediction)]
    WHM_Re_Benediction = 19032,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Tetragrammaton)]
    [CustomComboInfo(Job.WHM, WHM_Re_Tetragrammaton)]
    [Retargeted(WHM.Tetragrammaton)]
    WHM_Re_Tetragrammaton = 19033,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Regen)]
    [CustomComboInfo(Job.WHM, WHM_Re_Regen)]
    [Retargeted(WHM.Regen)]
    WHM_Re_Regen = 19034,

    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.DivineBenison)]
    [CustomComboInfo(Job.WHM, WHM_Re_DivineBenison)]
    [Retargeted(WHM.DivineBenison)]
    WHM_Re_DivineBenison = 19035,

    #endregion

    #region Raidwide Heals

    [CustomComboInfo(Job.WHM, WHM_Raidwide)]
    WHM_Raidwide = 19220,

    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo(Job.WHM, WHM_Raidwide_Asylum)]
    WHM_Raidwide_Asylum = 19221,

    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo(Job.WHM, WHM_Raidwide_Temperance)]
    WHM_Raidwide_Temperance = 19222,

    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo(Job.WHM, WHM_Raidwide_LiturgyOfTheBell)]
    WHM_Raidwide_LiturgyOfTheBell = 19223,

    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo(Job.WHM, WHM_Raidwide_PlenaryIndulgence)]
    WHM_Raidwide_PlenaryIndulgence = 19224,

    #endregion

    // Last value = 19051 (then skips to next last used: 19210)

    #endregion

    // Non-combat

    #region DOH

    // [CustomComboInfo(DOH.JobID, DOL_Eureka)]
    // DohPlaceholder = 50001,

    #endregion

    #region DOL

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.AgelessWords, DOL.SolidReason)]
    [CustomComboInfo(Job.MIN, DOL_Eureka)]
    DOL_Eureka = 51001,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.ArborCall, DOL.ArborCall2, DOL.LayOfTheLand, DOL.LayOfTheLand2)]
    [CustomComboInfo(Job.MIN, DOL_NodeSearchingBuffs)]
    DOL_NodeSearchingBuffs = 51012,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Cast)]
    [CustomComboInfo(Job.FSH, FSH_CastHook)]
    FSH_CastHook = 51002,

    [Role(JobRole.DoL)]
    [CustomComboInfo(Job.FSH, FSH_Swim)]
    FSH_Swim = 51008,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Cast)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_CastGig)]
    FSH_CastGig = 51003,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.SurfaceSlap)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_SurfaceTrade)]
    FSH_SurfaceTrade = 51004,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.PrizeCatch)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_PrizeBounty)]
    FSH_PrizeBounty = 51005,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Snagging)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_SnaggingSalvage)]
    FSH_SnaggingSalvage = 51006,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.CastLight)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_CastLight_ElectricCurrent)]
    FSH_CastLight_ElectricCurrent = 51007,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Mooch, DOL.MoochII)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_Mooch_SharkEye)]
    FSH_Mooch_SharkEye = 51009,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.FishEyes)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_FishEyes_VitalSight)]
    FSH_FishEyes_VitalSight = 51010,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Chum)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo(Job.FSH, FSH_Chum_BaitedBreath)]
    FSH_Chum_BaitedBreath = 51011,

    // Last value = 51011

    #endregion

    #endregion

    #region PvP Combos

    #region PvP GLOBAL FEATURES

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.ADV, PvP_EmergencyHeals)]
    PvP_EmergencyHeals = 1100000,

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.ADV, PvP_EmergencyGuard)]
    PvP_EmergencyGuard = 1100010,

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.ADV, PvP_QuickPurify)]
    PvP_QuickPurify = 1100020,

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.ADV, PvP_MashCancel)]
    PvP_MashCancel = 1100030,

    [Role(JobRole.All)]
    [ParentCombo(PvP_MashCancel)]
    [CustomComboInfo(Job.ADV, PvP_MashCancelRecup)]
    PvP_MashCancelRecup = 1100031,

    // Last value = 1100030
    // Extra 0 on the end keeps things working the way they should be. Nothing to see here.

    #endregion

    #region ASTROLOGIAN

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.Malefic)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst)]
    ASTPvP_Burst = 111000,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_DrawCard)]
    ASTPvP_Burst_DrawCard = 111002,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_PlayCard)]
    ASTPvP_Burst_PlayCard = 111003,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_DoubleMalefic)]
    ASTPvP_Burst_DoubleMalefic = 111005,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst_Gravity)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_DoubleGravity)]
    ASTPvP_Burst_DoubleGravity = 111009,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_Gravity)]
    ASTPvP_Burst_Gravity = 111006,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_Macrocosmos)]
    ASTPvP_Burst_Macrocosmos = 111007,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Diabrosis)]
    ASTPvP_Diabrosis = 111010,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo(Job.AST, ASTPvP_Burst_Heal)]
    [PossiblyRetargeted]
    ASTPvP_Burst_Heal = 111011,

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.Epicycle)]
    [CustomComboInfo(Job.AST, ASTPvP_Epicycle)]
    ASTPvP_Epicycle = 111008,

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.AspectedBenefic)]
    [CustomComboInfo(Job.AST, ASTPvP_Heal)]
    [PossiblyRetargeted]
    ASTPvP_Heal = 111004,

    // Last value = 111010

    #endregion

    #region BLACK MAGE

    [PvPCustomCombo]
    [ReplaceSkill(BLMPvP.Fire, BLMPvP.Blizzard)]
    [CustomComboInfo(Job.BLM, BLMPvP_BurstMode)]
    BLMPvP_BurstMode = 112000,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.BLM, BLMPvP_Burst)]
    BLMPvP_Burst = 112001,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.BLM, BLMPvP_Xenoglossy)]
    BLMPvP_Xenoglossy = 112002,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.BLM, BLMPvP_Lethargy)]
    BLMPvP_Lethargy = 112003,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.BLM, BLMPvP_ElementalWeave)]
    BLMPvP_ElementalWeave = 112004,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.BLM, BLMPvP_ElementalStar)]
    BLMPvP_ElementalStar = 112005,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.BLM, BLMPvP_PhantomDart)]
    BLMPvP_PhantomDart = 112007,

    [PvPCustomCombo]
    [ReplaceSkill(BLMPvP.AetherialManipulation)]
    [CustomComboInfo(Job.BLM, BLMPvP_Manipulation_Feature)]
    BLMPvP_Manipulation_Feature = 112006,


    // Last value = 112007

    #endregion

    #region BARD

    [PvPCustomCombo]
    [ReplaceSkill(BRDPvP.PowerfulShot)]
    [CustomComboInfo(Job.BRD, BRDPvP_BurstMode)]
    BRDPvP_BurstMode = 113000,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_SilentNocturne)]
    BRDPvP_SilentNocturne = 113001,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_ApexArrow)]
    BRDPvP_ApexArrow = 113002,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_BlastArrow)]
    BRDPvP_BlastArrow = 113003,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_HarmonicArrow)]
    BRDPvP_HarmonicArrow = 113004,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_EncoreOfLight)]
    BRDPvP_EncoreOfLight = 113005,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_Wardens)]
    BRDPvP_Wardens = 113006,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo(Job.BRD, BRDPvP_Eagle)]
    BRDPvP_Eagle = 113007,

    // Last value = 113007

    #endregion

    #region DANCER

    [PvPCustomCombo]
    [ReplaceSkill(DNCPvP.Fountain)]
    [CustomComboInfo(Job.DNC, DNCPvP_BurstMode)]
    DNCPvP_BurstMode = 114000,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo(Job.DNC, DNCPvP_BurstMode_HoningDance)]
    DNCPvP_BurstMode_HoningDance = 114001,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo(Job.DNC, DNCPvP_BurstMode_CuringWaltz)]
    DNCPvP_BurstMode_CuringWaltz = 114002,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo(Job.DNC, DNCPvP_BurstMode_Partner)]
    DNCPvP_BurstMode_Partner = 114003,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo(Job.DNC, DNCPvP_BurstMode_Dash)]
    DNCPvP_BurstMode_Dash = 114004,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo(Job.DNC, DNCPvP_Eagle)]
    DNCPvP_Eagle = 114005,

    // Last value = 114005

    #endregion

    #region DARK KNIGHT

    [PvPCustomCombo]
    [ReplaceSkill(DRKPvP.Souleater)]
    [CustomComboInfo(Job.DRK, DRKPvP_Burst)]
    DRKPvP_Burst = 115000,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_Plunge)]
    DRKPvP_Plunge = 115001,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Plunge)]
    [CustomComboInfo(Job.DRK, DRKPvP_PlungeMelee)]
    DRKPvP_PlungeMelee = 115002,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_SaltedEarth)]
    DRKPvP_SaltedEarth = 115003,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_SaltAndDarkness)]
    DRKPvP_SaltAndDarkness = 115004,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_Impalement)]
    DRKPvP_Impalement = 115005,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_Shadowbringer)]
    DRKPvP_Shadowbringer = 115006,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_BlackestNight)]
    DRKPvP_BlackestNight = 115007,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_Scorn)]
    DRKPvP_Scorn = 115008,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo(Job.DRK, DRKPvP_Rampart)]
    DRKPvP_Rampart = 115009,

    // Last value = 115009

    #endregion

    #region DRAGOON

    [PvPCustomCombo]
    [ReplaceSkill(DRGPvP.Drakesbane)]
    [CustomComboInfo(Job.DRG, DRGPvP_Burst)]
    DRGPvP_Burst = 116000,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_Geirskogul)]
    DRGPvP_Geirskogul = 116001,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Geirskogul)]
    [CustomComboInfo(Job.DRG, DRGPvP_Nastrond)]
    DRGPvP_Nastrond = 116002,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_HorridRoar)]
    DRGPvP_HorridRoar = 116003,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_ChaoticSpringSustain)]
    DRGPvP_ChaoticSpringSustain = 116004,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_ChaoticSpringExecute)]
    DRGPvP_ChaoticSpringExecute = 116009,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_WyrmwindThrust)]
    DRGPvP_WyrmwindThrust = 116006,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_HighJump)]
    DRGPvP_HighJump = 116007,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_BurstProtection)]
    DRGPvP_BurstProtection = 116008,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo(Job.DRG, DRGPvP_Smite)]
    DRGPvP_Smite = 116010,

    // Last value = 116010

    #endregion

    #region GUNBREAKER

    #region Burst Mode

    [PvPCustomCombo]
    [ReplaceSkill(GNBPvP.SolidBarrel)]
    [CustomComboInfo(Job.GNB, GNBPvP_Burst)]
    GNBPvP_Burst = 117000,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_FatedCircle)]
    GNBPvP_FatedCircle = 117001,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_ST_GnashingFang)]
    GNBPvP_ST_GnashingFang = 117004,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_ST_Continuation)]
    GNBPvP_ST_Continuation = 117005,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_RoughDivide)]
    GNBPvP_RoughDivide = 117006,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_BlastingZone)]
    GNBPvP_BlastingZone = 117007,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_Corundum)]
    GNBPvP_Corundum = 117011,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo(Job.GNB, GNBPvP_Rampart)]
    GNBPvP_Rampart = 117012,

    #endregion

    #region Option Select

    [PvPCustomCombo]
    [ReplaceSkill(GNBPvP.GnashingFang)]
    [CustomComboInfo(Job.GNB, GNBPvP_GnashingFang)]
    GNBPvP_GnashingFang = 117010,

    // Last value = 117012

    #endregion

    #endregion

    #region MACHINIST

    [PvPCustomCombo]
    [ReplaceSkill(MCHPvP.BlastCharge)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode)]
    MCHPvP_BurstMode = 118000,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_AirAnchor)]
    MCHPvP_BurstMode_AirAnchor = 118001,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_Analysis)]
    MCHPvP_BurstMode_Analysis = 118002,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode_Analysis)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_AltAnalysis)]
    MCHPvP_BurstMode_AltAnalysis = 118003,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_Drill)]
    MCHPvP_BurstMode_Drill = 118004,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode_Drill)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_AltDrill)]
    MCHPvP_BurstMode_AltDrill = 118005,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_BioBlaster)]
    MCHPvP_BurstMode_BioBlaster = 118006,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_ChainSaw)]
    MCHPvP_BurstMode_ChainSaw = 118007,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_FullMetalField)]
    MCHPvP_BurstMode_FullMetalField = 118008,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_Wildfire)]
    MCHPvP_BurstMode_Wildfire = 118009,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_BurstMode_MarksmanSpite)]
    MCHPvP_BurstMode_MarksmanSpite = 118011,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo(Job.MCH, MCHPvP_Eagle)]
    MCHPvP_Eagle = 118012,

    // Last value = 118012

    #endregion

    #region MONK

    [PvPCustomCombo]
    [ReplaceSkill(MNKPvP.PhantomRush)]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst)]
    MNKPvP_Burst = 119000,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst_Meteodrive)]
    MNKPvP_Burst_Meteodrive = 119006,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst_Thunderclap)]
    MNKPvP_Burst_Thunderclap = 119001,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst_RiddleOfEarth)]
    MNKPvP_Burst_RiddleOfEarth = 119002,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst_FlintsReply)]
    MNKPvP_Burst_FlintsReply = 119003,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst_RisingPhoenix)]
    MNKPvP_Burst_RisingPhoenix = 119004,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Burst_WindsReply)]
    MNKPvP_Burst_WindsReply = 119005,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.MNK, MNKPvP_Smite)]
    MNKPvP_Smite = 119007,

    // Last value = 119007

    #endregion

    #region NINJA

    [PvPCustomCombo]
    [ReplaceSkill(NINPvP.AeolianEdge)]
    [CustomComboInfo(Job.NIN, NINPvP_ST_BurstMode)]
    NINPvP_ST_BurstMode = 120000,

    [PvPCustomCombo]
    [ReplaceSkill(NINPvP.FumaShuriken)]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_BurstMode)]
    NINPvP_AoE_BurstMode = 120001,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_Meisui)]
    NINPvP_ST_Meisui = 120002,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_MudraMode)]
    NINPvP_ST_MudraMode = 120013,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_FumaShuriken)]
    NINPvP_ST_FumaShuriken = 120003,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_ThreeMudra)]
    NINPvP_ST_ThreeMudra = 120004,

    [ParentCombo(NINPvP_ST_ThreeMudra)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_ThreeMudraPool)]
    NINPvP_ST_ThreeMudraPool = 120014,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_Dokumori)]
    NINPvP_ST_Dokumori = 120005,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_Bunshin)]
    NINPvP_ST_Bunshin = 120006,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_ST_SeitonTenchu)]
    NINPvP_ST_SeitonTenchu = 120007,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_Meisui)]
    NINPvP_AoE_Meisui = 120008,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_MudraMode)]
    NINPvP_AoE_MudraMode = 120016,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_FumaShuriken)]
    NINPvP_AoE_FumaShuriken = 120009,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_ThreeMudra)]
    NINPvP_AoE_ThreeMudra = 120010,

    [ParentCombo(NINPvP_AoE_ThreeMudra)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_ThreeMudraPool)]
    NINPvP_AoE_ThreeMudraPool = 120015,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_Dokumori)]
    NINPvP_AoE_Dokumori = 120011,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_Bunshin)]
    NINPvP_AoE_Bunshin = 120012,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_AoE_SeitonTenchu)]
    NINPvP_AoE_SeitonTenchu = 120017,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.NIN, NINPvP_Smite)]
    NINPvP_Smite = 120018,

    // Last value = 120018

    #endregion

    #region PALADIN

    [PvPCustomCombo]
    [ReplaceSkill(PLDPvP.RoyalAuthority)]
    [CustomComboInfo(Job.PLD, PLDPvP_Burst)]
    PLDPvP_Burst = 121000,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_ShieldSmite)]
    PLDPvP_ShieldSmite = 121001,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_Imperator)]
    PLDPvP_Imperator = 121002,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_Confiteor)]
    PLDPvP_Confiteor = 121003,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_HolySpirit)]
    PLDPvP_HolySpirit = 121004,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_Intervene)]
    PLDPvP_Intervene = 121005,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_Intervene_Melee)]
    PLDPvP_Intervene_Melee = 121006,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_PhalanxCombo)]
    PLDPvP_PhalanxCombo = 121007,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_Sheltron)]
    PLDPvP_Sheltron = 121008,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo(Job.PLD, PLDPvP_Rampart)]
    PLDPvP_Rampart = 121009,

    // Last value = 121009

    #endregion

    #region PICTOMANCER

    [PvPCustomCombo]
    [ReplaceSkill(PCTPvP.FireInRed)]
    [CustomComboInfo(Job.PCT, PCTPvP_Burst)]
    PCTPvP_Burst = 140000,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_BurstControl)]
    PCTPvP_BurstControl = 140001,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_TemperaCoat)]
    PCTPvP_TemperaCoat = 140002,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_SubtractivePalette)]
    PCTPvP_SubtractivePalette = 140003,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_CreatureMotif)]
    PCTPvP_CreatureMotif = 140004,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_LivingMuse)]
    PCTPvP_LivingMuse = 140005,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_MogOfTheAges)]
    PCTPvP_MogOfTheAges = 140006,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_HolyInWhite)]
    PCTPvP_HolyInWhite = 140007,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo(Job.PCT, PCTPvP_StarPrism)]
    PCTPvP_StarPrism = 140008,

    [ParentCombo(PCTPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.PCT, PCTPvP_PhantomDart)]
    PCTPvP_PhantomDart = 140009,

    [PvPCustomCombo]
    [ReplaceSkill(PCTPvP.LivingMuse)]
    [CustomComboInfo(Job.PCT, PCTPvP_OneButtonMotifs)]
    PCTPvP_OneButtonMotifs = 140010,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_OneButtonMotifs)]
    [CustomComboInfo(Job.PCT, PCTPvP_StarPrismOneButtonMotifs)]
    PCTPvP_StarPrismOneButtonMotifs = 140011,

    // Last value = 140011

    #endregion

    #region REAPER

    [PvPCustomCombo]
    [ReplaceSkill(RPRPvP.Slice)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst)]
    RPRPvP_Burst = 122000,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_GrimSwathe)]
    RPRPvP_Burst_GrimSwathe = 122009,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_DeathWarrant)]
    RPRPvP_Burst_DeathWarrant = 122001,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_ImmortalPooling)]
    RPRPvP_Burst_ImmortalPooling = 122003,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_Enshrouded)]
    RPRPvP_Burst_Enshrouded = 122004,

    #region RPR Enshrouded Option

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst_Enshrouded)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_Enshrouded_DeathWarrant)]
    RPRPvP_Burst_Enshrouded_DeathWarrant = 122005,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst_Enshrouded)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_Enshrouded_Communio)]
    RPRPvP_Burst_Enshrouded_Communio = 122006,

    #endregion

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_RangedHarvest)]
    RPRPvP_Burst_RangedHarvest = 122007,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Burst_ArcaneCircle)]
    RPRPvP_Burst_ArcaneCircle = 122008,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo(Job.RPR, RPRPvP_Smite)]
    RPRPvP_Smite = 122010,

    // Last value = 122010

    #endregion

    #region RED MAGE

    [PvPCustomCombo]
    [ReplaceSkill(RDMPvP.Jolt3)]
    [CustomComboInfo(Job.RDM, RDMPvP_BurstMode)]
    RDMPvP_BurstMode = 123000,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo(Job.RDM, RDMPvP_Riposte)]
    RDMPvP_Riposte = 123001,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo(Job.RDM, RDMPvP_Resolution)]
    RDMPvP_Resolution = 123002,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo(Job.RDM, RDMPvP_Embolden)]
    RDMPvP_Embolden = 123003,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo(Job.RDM, RDMPvP_Corps)]
    RDMPvP_Corps = 123004,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo(Job.RDM, RDMPvP_Displacement)]
    RDMPvP_Displacement = 123005,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo(Job.RDM, RDMPvP_Forte)]
    RDMPvP_Forte = 123006,

    [PvPCustomCombo]
    [ReplaceSkill(RDMPvP.CorpsACorps, RDMPvP.Displacement)]
    [CustomComboInfo(Job.RDM, RDMPvP_Dash_Feature)]
    RDMPvP_Dash_Feature = 123007,

    [ParentCombo(RDMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.RDM, RDMPvP_PhantomDart)]
    RDMPvP_PhantomDart = 123008,

    // Last value = 123008

    #endregion

    #region SAGE

    [PvPCustomCombo]
    [ReplaceSkill(SGEPvP.Dosis)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode)]
    SGEPvP_BurstMode = 124000,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_Pneuma)]
    SGEPvP_BurstMode_Pneuma = 124001,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_Eukrasia)]
    SGEPvP_BurstMode_Eukrasia = 124002,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_Phlegma)]
    SGEPvP_BurstMode_Phlegma = 124003,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_Psyche)]
    SGEPvP_BurstMode_Psyche = 124004,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_Toxikon)]
    SGEPvP_BurstMode_Toxikon = 124005,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_Toxikon2)]
    SGEPvP_BurstMode_Toxikon2 = 124006,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_BurstMode_KardiaReminder)]
    SGEPvP_BurstMode_KardiaReminder = 124007,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo(Job.SGE, SGEPvP_Diabrosis)]
    SGEPvP_Diabrosis = 124008,

    [PvPCustomCombo]
    [ReplaceSkill(SGEPvP.Kardia)]
    [CustomComboInfo(Job.SGE, SGEPvP_RetargetKardia)]
    [Retargeted]
    SGEPvP_RetargetKardia = 124009,

    // Last value = 124009

    #endregion

    #region SAMURAI

    [PvPCustomCombo]
    [ReplaceSkill(SAMPvP.Yukikaze)]
    [CustomComboInfo(Job.SAM, SAMPvP_Burst)]
    SAMPvP_Burst = 125000,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo(Job.SAM, SAMPvP_Meikyo)]
    SAMPvP_Meikyo = 125001,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo(Job.SAM, SAMPvP_Chiten)]
    SAMPvP_Chiten = 125002,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo(Job.SAM, SAMPvP_Mineuchi)]
    SAMPvP_Mineuchi = 125003,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo(Job.SAM, SAMPvP_Soten)]
    SAMPvP_Soten = 125004,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo(Job.SAM, SAMPvP_Zantetsuken)]
    SAMPvP_Zantetsuken = 125005,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo(Job.SAM, SAMPvP_Smite)]
    SAMPvP_Smite = 125006,

    // Last value = 125006

    #endregion

    #region SCHOLAR

    [PvPCustomCombo]
    [ReplaceSkill(SCHPvP.Broil)]
    [CustomComboInfo(Job.SCH, SCHPvP_Burst)]
    SCHPvP_Burst = 126000,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo(Job.SCH, SCHPvP_Expedient)]
    SCHPvP_Expedient = 126001,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo(Job.SCH, SCHPvP_Biolysis)]
    SCHPvP_Biolysis = 126002,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo(Job.SCH, SCHPvP_DeploymentTactics)]
    SCHPvP_DeploymentTactics = 126003,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo(Job.SCH, SCHPvP_ChainStratagem)]
    SCHPvP_ChainStratagem = 126004,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo(Job.SCH, SCHPvP_Diabrosis)]
    SCHPvP_Diabrosis = 126005,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo(Job.SCH, SCHPvP_Adlo)]
    [PossiblyRetargeted]
    SCHPvP_Adlo = 126006,

    [PvPCustomCombo]
    [ReplaceSkill(SCHPvP.Adloquilum)]
    [CustomComboInfo(Job.SCH, SCHPvP_RetargetAdlo)]
    [Retargeted]
    SCHPvP_RetargetAdlo = 126007,

    // Last value = 126006

    #endregion

    #region SUMMONER

    [PvPCustomCombo]
    [ReplaceSkill(SMNPvP.Ruin3)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode)]
    SMNPvP_BurstMode = 127000,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_RadiantAegis)]
    SMNPvP_BurstMode_RadiantAegis = 127001,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_CrimsonCyclone)]
    SMNPvP_BurstMode_CrimsonCyclone = 127002,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_CrimsonStrike)]
    SMNPvP_BurstMode_CrimsonStrike = 127003,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_MountainBuster)]
    SMNPvP_BurstMode_MountainBuster = 127004,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_Slipstream)]
    SMNPvP_BurstMode_Slipstream = 127005,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_Necrotize)]
    SMNPvP_BurstMode_Necrotize = 127006,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_DeathFlare)]
    SMNPvP_BurstMode_DeathFlare = 127007,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo(Job.SMN, SMNPvP_BurstMode_BrandofPurgatory)]
    SMNPvP_BurstMode_BrandofPurgatory = 127008,

    [ParentCombo(SMNPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo(Job.SMN, SMNPvP_PhantomDart)]
    SMNPvP_PhantomDart = 127009,

    // Last value = 127008

    #endregion

    #region VIPER

    [PvPCustomCombo]
    [ReplaceSkill(VPRPvP.SteelFangs)]
    [CustomComboInfo(Job.VPR, VPRPvP_Burst)]
    VPRPvP_Burst = 130000,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo(Job.VPR, VPRPvP_Bloodcoil)]
    VPRPvP_Bloodcoil = 130001,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo(Job.VPR, VPRPvP_UncoiledFury)]
    VPRPvP_UncoiledFury = 130002,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo(Job.VPR, VPRPvP_Backlash)]
    VPRPvP_Backlash = 130003,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo(Job.VPR, VPRPvP_RattlingCoil)]
    VPRPvP_RattlingCoil = 130004,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo(Job.VPR, VPRPvP_Slither)]
    VPRPvP_Slither = 130005,

    [PvPCustomCombo]
    [ReplaceSkill(VPRPvP.SnakeScales)]
    [CustomComboInfo(Job.VPR, VPRPvP_SnakeScales_Feature)]
    VPRPvP_SnakeScales_Feature = 130006,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo(Job.VPR, VPRPvP_Smite)]
    VPRPvP_Smite = 130007,

    // Last value = 130007

    #endregion

    #region WARRIOR

    [PvPCustomCombo]
    [ReplaceSkill(WARPvP.HeavySwing)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode)]
    WARPvP_BurstMode = 128000,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_Bloodwhetting)]
    WARPvP_BurstMode_Bloodwhetting = 128001,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_Blota)]
    WARPvP_BurstMode_Blota = 128003,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_PrimalRend)]
    WARPvP_BurstMode_PrimalRend = 128004,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_InnerChaos)]
    WARPvP_BurstMode_InnerChaos = 128005,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_Orogeny)]
    WARPvP_BurstMode_Orogeny = 128006,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_Onslaught)]
    WARPvP_BurstMode_Onslaught = 128007,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_BurstMode_PrimalScream)]
    WARPvP_BurstMode_PrimalScream = 128008,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo(Job.WAR, WARPvP_Rampart)]
    WARPvP_Rampart = 128009,

    // Last value = 128009

    #endregion

    #region WHITE MAGE

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.Glare)]
    [CustomComboInfo(Job.WHM, WHMPvP_Burst)]
    WHMPvP_Burst = 129000,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo(Job.WHM, WHMPvP_Afflatus_Misery)]
    WHMPvP_Afflatus_Misery = 129001,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo(Job.WHM, WHMPvP_Mirace_of_Nature)]
    WHMPvP_Mirace_of_Nature = 129002,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo(Job.WHM, WHMPvP_Seraph_Strike)]
    WHMPvP_Seraph_Strike = 129003,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo(Job.WHM, WHMPvP_AfflatusPurgation)]
    WHMPvP_AfflatusPurgation = 129008,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo(Job.WHM, WHMPvP_Diabrosis)]
    WHMPvP_Diabrosis = 129009,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo(Job.WHM, WHMPvP_Burst_Heals)]
    [Retargeted]
    WHMPvP_Burst_Heals = 129010,

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.Cure2)]
    [CustomComboInfo(Job.WHM, WHMPvP_Heals)]
    [Retargeted]
    WHMPvP_Heals = 129004,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Aquaveil)]
    [CustomComboInfo(Job.WHM, WHMPvP_Cure3)]
    WHMPvP_Cure3 = 129005,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Heals)]
    [CustomComboInfo(Job.WHM, WHMPvP_Aquaveil)]
    WHMPvP_Aquaveil = 129007,

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.SeraphStrike)]
    [CustomComboInfo(Job.WHM, WHMPvP_Seraphstrike)]
    WHMPvP_Seraphstrike = 129011,

    // Last value = 129011

    #endregion

    #endregion
}
