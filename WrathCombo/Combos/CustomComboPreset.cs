﻿#region

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
public enum CustomComboPreset
{
    #region PvE Combos

    #region PHANTOM ACTIONS
    [OccultCrescent(OccultCrescent.JobIDs.Freelancer)]
    [CustomComboInfo("Freelancer Job Actions", "Enable this to add Freelancer-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Freelancer = 110000,

    [OccultCrescent]
    [ParentCombo(Phantom_Freelancer)]
    [CustomComboInfo("Occult Resuscitation", "Adds Occult Resuscitation into the rotation.", OccultCrescent.JobID)]
    Phantom_Freelancer_OccultResuscitation = 110001,

    [OccultCrescent]
    [ParentCombo(Phantom_Freelancer)]
    [CustomComboInfo("Occult Treasuresight", "Adds Occult Treasuresight into the rotation.", OccultCrescent.JobID)]
    Phantom_Freelancer_OccultTreasuresight = 110002,

    [OccultCrescent(OccultCrescent.JobIDs.Knight)]
    [CustomComboInfo("Knight Job Actions", "Enable this to add Knight-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Knight = 110003,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo("Phantom Guard", "Adds Phantom Guard into the rotation.", OccultCrescent.JobID)]
    Phantom_Knight_PhantomGuard = 110004,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo("Pray", "Adds Pray into the rotation.", OccultCrescent.JobID)]
    Phantom_Knight_Pray = 110005,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo("Occult Heal", "Adds Occult Heal into the rotation.", OccultCrescent.JobID)]
    Phantom_Knight_OccultHeal = 110006,

    [OccultCrescent]
    [ParentCombo(Phantom_Knight)]
    [CustomComboInfo("Pledge", "Adds Pledge into the rotation.", OccultCrescent.JobID)]
    Phantom_Knight_Pledge = 110007,

    [OccultCrescent(OccultCrescent.JobIDs.Monk)]
    [CustomComboInfo("Monk Job Actions", "Enable this to add Monk-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Monk = 110008,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo("Phantom Kick", "Adds Phantom Kick into the rotation.", OccultCrescent.JobID)]
    Phantom_Monk_PhantomKick = 110009,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo("Occult Counter", "Adds Occult Counter into the rotation.", OccultCrescent.JobID)]
    Phantom_Monk_OccultCounter = 110010,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo("Counterstance", "Adds Counterstance into the rotation.", OccultCrescent.JobID)]
    Phantom_Monk_Counterstance = 110011,

    [OccultCrescent]
    [ParentCombo(Phantom_Monk)]
    [CustomComboInfo("Occult Chakra", "Adds Occult Chakra into the rotation.", OccultCrescent.JobID)]
    Phantom_Monk_OccultChakra = 110012,

    [OccultCrescent(OccultCrescent.JobIDs.Thief)]
    [CustomComboInfo("Thief Job Actions", "Enable this to add Thief-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Thief = 110013,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo("Occult Sprint", "Adds Occult Sprint into the rotation.", OccultCrescent.JobID)]
    Phantom_Thief_OccultSprint = 110014,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo("Steal", "Adds Steal into the rotation.", OccultCrescent.JobID)]
    Phantom_Thief_Steal = 110015,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo("Vigilance", "Adds Vigilance into the rotation.", OccultCrescent.JobID)]
    Phantom_Thief_Vigilance = 110016,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo("Trap Detection", "Adds Trap Detection into the rotation.", OccultCrescent.JobID)]
    Phantom_Thief_TrapDetection = 110017,

    [OccultCrescent]
    [ParentCombo(Phantom_Thief)]
    [CustomComboInfo("Pilfer Weapon", "Adds Pilfer Weapon into the rotation.", OccultCrescent.JobID)]
    Phantom_Thief_PilferWeapon = 110018,

    [OccultCrescent(OccultCrescent.JobIDs.Samurai)]
    [CustomComboInfo("Samurai Job Actions", "Enable this to add Samurai-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Samurai = 110053,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo("Phantom Mineuchi", "Adds Phantom Mineuchi into the rotation.", OccultCrescent.JobID)]
    Phantom_Samurai_Mineuchi = 110054,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo("Phantom Shirahadori", "Adds Phantom Shirahadori into the rotation.", OccultCrescent.JobID)]
    Phantom_Samurai_Shirahadori = 110055,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo("Phantom Iainuki", "Adds Phantom Iainuki into the rotation.", OccultCrescent.JobID)]
    Phantom_Samurai_Iainuki = 110056,

    [OccultCrescent]
    [ParentCombo(Phantom_Samurai)]
    [CustomComboInfo("Phantom Zeninage", "Adds Phantom Zeninage into the rotation.", OccultCrescent.JobID)]
    Phantom_Samurai_Zeninage = 110057,

    [OccultCrescent(OccultCrescent.JobIDs.Berserker)]
    [CustomComboInfo("Berserker Job Actions", "Enable this to add Berserker-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Berserker = 110019,

    [OccultCrescent]
    [ParentCombo(Phantom_Berserker)]
    [CustomComboInfo("Rage", "Adds Rage into the rotation.", OccultCrescent.JobID)]
    Phantom_Berserker_Rage = 110020,

    [OccultCrescent]
    [ParentCombo(Phantom_Berserker)]
    [CustomComboInfo("Deadly Blow", "Adds Deadly Blow into the rotation.", OccultCrescent.JobID)]
    Phantom_Berserker_DeadlyBlow = 110021,

    [OccultCrescent(OccultCrescent.JobIDs.Ranger)]
    [CustomComboInfo("Ranger Job Actions", "Enable this to add Ranger-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Ranger = 110022,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo("Phantom Aim", "Adds Phantom Aim into the rotation.", OccultCrescent.JobID)]
    Phantom_Ranger_PhantomAim = 110023,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo("Occult Featherfoot", "Adds Occult Featherfoot into the rotation.", OccultCrescent.JobID)]
    Phantom_Ranger_OccultFeatherfoot = 110024,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo("Occult Falcon", "Adds Occult Falcon into the rotation.", OccultCrescent.JobID)]
    Phantom_Ranger_OccultFalcon = 110025,

    [OccultCrescent]
    [ParentCombo(Phantom_Ranger)]
    [CustomComboInfo("Occult Unicorn", "Adds Occult Unicorn into the rotation.", OccultCrescent.JobID)]
    Phantom_Ranger_OccultUnicorn = 110026,

    [OccultCrescent(OccultCrescent.JobIDs.TimeMage)]
    [CustomComboInfo("Time Mage Job Actions", "Enable this to add Time Mage-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_TimeMage = 110027,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo("Occult Slowga", "Adds Occult Slowga into the rotation.", OccultCrescent.JobID)]
    Phantom_TimeMage_OccultSlowga = 110028,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage_OccultSlowga)]
    [CustomComboInfo("Wait to get Full(-ish) Effect", "Will not use Slowga if the diminishing return from repeated use is substantial, or there would be no diminishing return at all.", OccultCrescent.JobID)]
    Phantom_TimeMage_OccultSlowga_Wait = 110075,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo("Occult Comet", "Adds Occult Comet into the rotation.", OccultCrescent.JobID)]
    Phantom_TimeMage_OccultComet = 110029,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo("Occult Mage Masher", "Adds Occult Mage Masher into the rotation.", OccultCrescent.JobID)]
    Phantom_TimeMage_OccultMageMasher = 110030,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo("Occult Dispel", "Adds Occult Dispel into the rotation if the target has a known-dispellable status.", OccultCrescent.JobID)]
    Phantom_TimeMage_OccultDispel = 110031,

    [OccultCrescent]
    [ParentCombo(Phantom_TimeMage)]
    [CustomComboInfo("Occult Quick", "Adds Occult Quick into the rotation.", OccultCrescent.JobID)]
    Phantom_TimeMage_OccultQuick = 110032,

    [OccultCrescent(OccultCrescent.JobIDs.Chemist)]
    [CustomComboInfo("Chemist Job Actions", "Enable this to add Chemist-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Chemist = 110033,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo("Occult Potion", "Adds Occult Potion into the rotation.\nRequires Occult Potions in inventory.", OccultCrescent.JobID)]
    Phantom_Chemist_OccultPotion = 110034,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo("Occult Ether", "Adds Occult Ether into the rotation.\nRequires Occult Potions in inventory.", OccultCrescent.JobID)]
    Phantom_Chemist_OccultEther = 110035,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo("Revive", "Adds Revive into the rotation.", OccultCrescent.JobID)]
    Phantom_Chemist_Revive = 110036,

    [OccultCrescent]
    [ParentCombo(Phantom_Chemist)]
    [CustomComboInfo("Occult Elixir", "Adds Occult Elixir into the rotation.\nRequires Occult Elixir in inventory.", OccultCrescent.JobID)]
    Phantom_Chemist_OccultElixir = 110037,

    [OccultCrescent(OccultCrescent.JobIDs.Bard)]
    [CustomComboInfo("Bard Job Actions", "Enable this to add Bard-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Bard = 110038,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo("Mighty March", "Adds Mighty March into the rotation.", OccultCrescent.JobID)]
    Phantom_Bard_MightyMarch = 110039,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo("Offensive Aria", "Adds Offensive Aria into the rotation.", OccultCrescent.JobID)]
    Phantom_Bard_OffensiveAria = 110040,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo("Romeo’s Ballad", "Adds Romeo’s Ballad into the rotation.", OccultCrescent.JobID)]
    Phantom_Bard_RomeosBallad = 110041,

    [OccultCrescent]
    [ParentCombo(Phantom_Bard)]
    [CustomComboInfo("Hero’s Rime", "Adds Hero’s Rime into the rotation.", OccultCrescent.JobID)]
    Phantom_Bard_HerosRime = 110042,

    [OccultCrescent(OccultCrescent.JobIDs.Oracle)]
    [CustomComboInfo("Oracle Job Actions", "Enable this to add Oracle-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle = 110043,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Predict", "Adds Predict into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_Predict = 110044,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Phantom Judgment", "Adds Phantom Judgment into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_PhantomJudgment = 110045,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Cleansing", "Adds Cleansing into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_Cleansing = 110046,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Blessing", "Adds Blessing into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_Blessing = 110047,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Starfall", "Adds Starfall into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_Starfall = 110048,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Recuperation", "Adds Recuperation into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_Recuperation = 110049,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Phantom Doom", "Adds Phantom Doom into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_PhantomDoom = 110050,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Phantom Rejuvenation", "Adds Phantom Rejuvenation into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_PhantomRejuvenation = 110051,

    [OccultCrescent]
    [ParentCombo(Phantom_Oracle)]
    [CustomComboInfo("Invulnerability", "Adds Invulnerability into the rotation.", OccultCrescent.JobID)]
    Phantom_Oracle_Invulnerability = 110052,

    [OccultCrescent(OccultCrescent.JobIDs.Cannoneer)]
    [CustomComboInfo("Cannoneer Job Actions", "Enable this to add Cannoneer-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Cannoneer = 110058,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo("Phantom Fire", "Adds Phantom Fire into the rotation.", OccultCrescent.JobID)]
    Phantom_Cannoneer_PhantomFire = 110059,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo("Holy Cannon", "Adds Holy Cannon into the rotation.", OccultCrescent.JobID)]
    Phantom_Cannoneer_HolyCannon = 110060,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo("Dark Cannon", "Adds Dark Cannon into the rotation.", OccultCrescent.JobID)]
    Phantom_Cannoneer_DarkCannon = 110061,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo("Shock Cannon", "Adds Shock Cannon into the rotation.", OccultCrescent.JobID)]
    Phantom_Cannoneer_ShockCannon = 110062,

    [OccultCrescent]
    [ParentCombo(Phantom_Cannoneer)]
    [CustomComboInfo("Silver Cannon", "Adds Silver Cannon into the rotation.", OccultCrescent.JobID)]
    Phantom_Cannoneer_SilverCannon = 110063,

    [OccultCrescent(OccultCrescent.JobIDs.Geomancer)]
    [CustomComboInfo("Geomancer Job Actions", "Enable this to add Geomancer-specific actions into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer = 110064,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo("Battle Bell", "Adds Battle Bell into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_BattleBell = 110065,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo("Ringing Respite", "Adds Ringing Respite into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_RingingRespite = 110073,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo("Suspend", "Adds Suspend into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_Suspend = 110074,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer)]
    [CustomComboInfo("Weather", "Adds Weather into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_Weather = 110066,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo("Sunbath", "Adds Sunbath into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_Sunbath = 110067,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo("Cloudy Caress", "Adds Cloudy Caress into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_CloudyCaress = 110068,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo("Blessed Rain", "Adds Blessed Rain into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_BlessedRain = 110069,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo("Misty Mirage", "Adds Misty Mirage into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_MistyMirage = 110070,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo("Hasty Mirage", "Adds Hasty Mirage into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_HastyMirage = 110071,

    [OccultCrescent]
    [ParentCombo(Phantom_Geomancer_Weather)]
    [CustomComboInfo("Aetherial Gain", "Adds Aetherial Gain into the rotation.", OccultCrescent.JobID)]
    Phantom_Geomancer_AetherialGain = 110072,

    //Last Value = 110075
    #endregion

    #region GLOBAL FEATURES

    [Role(JobRole.All)]
    [ReplaceSkill(All.Sprint)]
    [CustomComboInfo("Island Sanctuary Sprint Feature",
        "Replaces Sprint with Isle Sprint.\nOnly works at the Island Sanctuary. Icon does not change.\nDo not use with SimpleTweaks' Island Sanctuary Sprint fix.",
        ADV.JobID)]
    ALL_IslandSanctuary_Sprint = 100093,

    #region Global Tank Features

    [Role(JobRole.Tank)]
    [CustomComboInfo("Global Tank Features",
        "Features and options involving shared role actions for Tanks.",
        ADV.JobID)]
    ALL_Tank_Menu = 100099,

    [Role(JobRole.Tank)]
    [ReplaceSkill(RoleActions.Tank.LowBlow, PLD.ShieldBash)]
    [ConflictingCombos(PLD_RetargetShieldBash)]
    [ParentCombo(ALL_Tank_Menu)]
    [CustomComboInfo("Tank: Interrupt Feature",
        "Replaces Low Blow (Stun) with Interject (Interrupt) when the target can be interrupted.\nPLDs can slot Shield Bash to have the feature to work with Shield Bash.",
        ADV.JobID)]
    ALL_Tank_Interrupt = 100000,

    [ParentCombo(ALL_Tank_Interrupt)]
    [Retargeted(RoleActions.Tank.Interject, RoleActions.Tank.LowBlow)]
    [CustomComboInfo("Retarget Interrupts", "Will retarget the interrupt if the caster is not your current target.", ADV.JobID)]
    ALL_Tank_Interrupt_Retarget = 100005,

    [Role(JobRole.Tank)]
    [ReplaceSkill(RoleActions.Tank.Reprisal)]
    [ParentCombo(ALL_Tank_Menu)]
    [CustomComboInfo("Tank: Double Reprisal Protection",
        "Prevents the use of Reprisal when target already has the effect by replacing it with Savage Blade.", ADV.JobID)]
    ALL_Tank_Reprisal = 100001,

    [Role(JobRole.Tank)]
    [ReplaceSkill(RoleActions.Tank.Shirk)]
    [ParentCombo(ALL_Tank_Menu)]
    [CustomComboInfo("Tank: Shirk Retargeting",
        "Retargets Shirk to the other tank if there is one.", ADV.JobID)]
    [Retargeted(RoleActions.Tank.Shirk)]
    ALL_Tank_ShirkRetargeting = 100002,

    [Role(JobRole.Tank)]
    [ParentCombo(ALL_Tank_ShirkRetargeting)]
    [CustomComboInfo("Use Healers instead",
        "Will Retarget Shirk to healers, instead of the other tank.\nOnly recommended during some specific enrages.", ADV.JobID)]
    [Retargeted]
    ALL_Tank_ShirkRetargeting_Healer = 100003,

    [Role(JobRole.Tank)]
    [ParentCombo(ALL_Tank_ShirkRetargeting)]
    [CustomComboInfo("Fallback to Any Support",
        "Will Retarget Shirk to tanks or healers, per your setting above, but will include a fallback to any tank OR healer if none of your setting is found.\nUseful to help your Shirk always go to *someone* even if your chosen players are not alive.", ADV.JobID)]
    [Retargeted]
    ALL_Tank_ShirkRetargeting_Fallback = 100004,

    #endregion

    #region Global Healer Features

    [Role(JobRole.Healer)]
    [CustomComboInfo("Global Healer Features",
        "Features and options involving shared role actions for Healers.",
        ADV.JobID)]
    ALL_Healer_Menu = 100098,

    [Role(JobRole.Healer)]
    [ReplaceSkill(AST.Ascend, WHM.Raise, SCH.Resurrection, SGE.Egeiro)]
    [ConflictingCombos(AST_Raise_Alternative, SCH_Raise, SGE_Raise, WHM_Raise)]
    [ParentCombo(ALL_Healer_Menu)]
    [CustomComboInfo("Healer: Raise Feature", "Changes the class' Raise Ability into Swiftcast.", ADV.JobID)]
    ALL_Healer_Raise = 100010,

    [ParentCombo(ALL_Healer_Raise)]
    [CustomComboInfo("Retarget Raises", "Will Retarget the Raises affected here to your Heal Stack.", ADV.JobID)]
    [Retargeted(WHM.Raise, AST.Ascend, SGE.Egeiro, SCH.Resurrection)]
    ALL_Healer_Raise_Retarget = 100011,

    [Role(JobRole.Healer)]
    [ReplaceSkill(RoleActions.Healer.Esuna)]
    [ParentCombo(ALL_Healer_Menu)]
    [CustomComboInfo("Healer: Esuna Retargeting",
        "Retargets Esuna (outside of combo usage) to your Heal Stack, checking if each potential target in the stack has a cleansable debuff.", ADV.JobID)]
    [Retargeted(RoleActions.Healer.Esuna)]
    ALL_Healer_EsunaRetargeting = 100012,
    
    [Role(JobRole.Healer)]
    [ReplaceSkill(RoleActions.Healer.Rescue)]
    [ParentCombo(ALL_Healer_Menu)]
    [CustomComboInfo("Healer: Rescue Retargeting", "Retargets Rescue (outside of combo usage) to UI Mouseover and additional options.", ADV.JobID)]
    [Retargeted(RoleActions.Healer.Rescue)]
    ALL_Healer_RescueRetargeting = 100013,
    #endregion

    #region Global Magical Ranged Features

    [Role(JobRole.MagicalDPS)]
    [CustomComboInfo("Global Magical Ranged Features",
        "Features and options involving shared role actions for Magical Ranged DPS.",
        ADV.JobID)]
    ALL_Caster_Menu = 100097,

    [Role(JobRole.MagicalDPS)]
    [ReplaceSkill(RoleActions.Caster.Addle)]
    [ParentCombo(ALL_Caster_Menu)]
    [CustomComboInfo("Magical Ranged DPS: Double Addle Protection",
        "Prevents the use of Addle when target already has the effect by replacing it with Savage Blade.", ADV.JobID)]
    ALL_Caster_Addle = 100020,

    [Role(JobRole.MagicalDPS)]
    [ReplaceSkill(RDM.Verraise, SMN.Resurrection, BLU.AngelWhisper)]
    [ConflictingCombos(SMN_Raise, RDM_Raise)]
    [ParentCombo(ALL_Caster_Menu)]
    [CustomComboInfo("Magical Ranged DPS: Raise Feature",
        "Changes the class' Raise Ability into Swiftcast. Red Mage will also show VerCure if Swiftcast is on cooldown.",
        ADV.JobID)]
    ALL_Caster_Raise = 100021,

    [ParentCombo(ALL_Caster_Raise)]
    [CustomComboInfo("Retarget Raises", "Will Retarget the Raises affected here to your Heal Stack.", ADV.JobID)]
    [Retargeted(BLU.AngelWhisper, RDM.Verraise, SMN.Resurrection)]
    ALL_Caster_Raise_Retarget = 100022,

    #endregion

    #region Global Melee Features

    [Role(JobRole.MeleeDPS)]
    [CustomComboInfo("Global Melee DPS Features",
        "Features and options involving shared role actions for Melee DPS.",
        ADV.JobID)]
    ALL_Melee_Menu = 100096,

    [Role(JobRole.MeleeDPS)]
    [ReplaceSkill(RoleActions.Melee.Feint)]
    [ParentCombo(ALL_Melee_Menu)]
    [CustomComboInfo("Melee DPS: Double Feint Protection",
        "Prevents the use of Feint when target already has the effect by replacing it with Savage Blade.", ADV.JobID)]
    ALL_Melee_Feint = 100030,

    [Role(JobRole.MeleeDPS)]
    [ReplaceSkill(RoleActions.Melee.TrueNorth)]
    [ParentCombo(ALL_Melee_Menu)]
    [CustomComboInfo("Melee DPS: True North Protection",
        "Prevents the use of True North when its buff is already active by replacing it with Savage Blade.", ADV.JobID)]
    ALL_Melee_TrueNorth = 100031,

    #endregion

    #region Global Ranged Physical Features

    [Role(JobRole.RangedDPS)]
    [CustomComboInfo("Global Physical Ranged Features",
        "Features and options involving shared role actions for Physical Ranged DPS.",
        ADV.JobID)]
    ALL_Ranged_Menu = 100095,

    [Role(JobRole.RangedDPS)]
    [ReplaceSkill(MCH.Tactician, BRD.Troubadour, DNC.ShieldSamba)]
    [ParentCombo(ALL_Ranged_Menu)]
    [CustomComboInfo("Physical Ranged DPS: Double Mitigation Protection",
        "Prevents the use of Tactician/Troubadour/Shield Samba when target already has one of those three effects by replacing them with Savage Blade.",
        ADV.JobID)]
    ALL_Ranged_Mitigation = 100040,

    [Role(JobRole.RangedDPS)]
    [ReplaceSkill(RoleActions.PhysRanged.FootGraze)]
    [ParentCombo(ALL_Ranged_Menu)]
    [CustomComboInfo("Physical Ranged DPS: Ranged Interrupt Feature",
        "Replaces Foot Graze with Head Graze when target can be interrupted.", ADV.JobID)]
    ALL_Ranged_Interrupt = 100041,

    #endregion

    //Non-gameplay Features
    //[CustomComboInfo("Output Combat Log", "Outputs your performed actions to the chat.", ADV.JobID)]
    //AllOutputCombatLog = 100094,

    // Last value = 100094

    #endregion

    // Jobs

    #region ASTROLOGIAN

    #region Simple Modes
    [AutoAction(false, false)]
    [ReplaceSkill(AST.Malefic, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic)]
    [ConflictingCombos(AST_ST_DPS)]
    [CustomComboInfo("Simple DPS Mode - Single Target", "Replaces Malefic with a full one-button single target rotation, including automatic dps card assignment.\nThis is the ideal option for newcomers to the job.", AST.JobID)]
    [SimpleCombo]
    AST_ST_Simple_DPS = 1179,
    
    [AutoAction(true, false)]
    [ReplaceSkill(AST.Gravity, AST.Gravity2)]
    [ConflictingCombos(AST_AOE_DPS)]
    [CustomComboInfo("Simple DPS Mode - AoE", "Replaces Gravity with a full one-button AoE rotation, including automatic dps card assignment.\nThis is the ideal option for newcomers to the job.", AST.JobID)]
    [SimpleCombo]
    AST_AOE_Simple_DPS = 1180,
        
    #endregion
    
    #region ST DPS

    [AutoAction(false, false)]
    [ReplaceSkill(AST.Malefic, AST.Malefic2, AST.Malefic3, AST.Malefic4, AST.FallMalefic, AST.Combust, AST.Combust2,
        AST.Combust3)]
    [ConflictingCombos(AST_ST_Simple_DPS)]
    [CustomComboInfo("Advanced DPS Mode - Single Target", "Replaces Malefic or Combust with options below", AST.JobID)]
    [AdvancedCombo]
    AST_ST_DPS = 1004,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Balance Opener (Level 92)", "Adds the Balance opener from level 92 onwards.", AST.JobID)]
    AST_ST_DPS_Opener = 1040,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Combust Uptime Option",
        "Adds Combust to the DPS feature if it's not present on current target, or is about to expire.", AST.JobID)]
    AST_ST_DPS_CombustUptime = 1018,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Lightspeed Weave Option", "Adds Lightspeed when moving", AST.JobID)]
    AST_DPS_LightSpeed = 1020,

    [ParentCombo(AST_DPS_LightSpeed)]
    [CustomComboInfo("Lightspeed Hold Option", "Retains 1 Lightspeed charge for manual use", AST.JobID)]
    AST_DPS_LightSpeedHold = 1061,   

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Divination Weave Option", "Adds Divination", AST.JobID)]
    AST_DPS_Divination = 1016,

    [ParentCombo(AST_DPS_Divination)]
    [CustomComboInfo("Lightspeed Burst Option", "Add Lightspeed before Divination \nCombine with Lightspeed Hold Options to ensure you have a charge to use", AST.JobID)]
    AST_DPS_LightspeedBurst = 1064,   

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Card Draw Weave Option", "Draws your cards", AST.JobID)]
    AST_DPS_AutoDraw = 1011,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Card Play Weave Option", "Weaves your Balance or Spear card (best used with Quick Target Cards)",
        AST.JobID)]
    [PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled, AST.Play1, AST.Balance, AST.Spear)]
    AST_DPS_AutoPlay = 1037,

    [ParentCombo(AST_DPS_AutoPlay)]
    [CustomComboInfo("Card Play Pooling Option", "Pools your dps cards for the Divination window",
        AST.JobID)]
    AST_DPS_CardPool = 1055,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Lord of Crowns Weave Option", "Adds Lord Of Crowns", AST.JobID)]
    AST_DPS_LazyLord = 1014,

    [ParentCombo(AST_DPS_LazyLord)]
    [CustomComboInfo("Lord of Crowns Pooling Option", "Holds Lord of Crowns for the Divination window", AST.JobID)]
    AST_DPS_LordPool = 1056,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Oracle Option", "Adds Oracle after Divination", AST.JobID)]
    AST_DPS_Oracle = 1015,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Earthly Star Option", "Adds Earthly Star placement, but not detonation, to the rotation.\nWill be targeted to any enemy, then your focus target, then soft and hard targets, before falling back to placing it at your feet.", AST.JobID)]
    [Retargeted(AST.EarthlyStar)]
    AST_ST_DPS_EarthlyStar = 1051,
    
    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Stellar Detonation Option", "Adds early Stellar Detonation with Giant Dominance based on targets HP percentage and type of encounter", AST.JobID)]
    AST_ST_DPS_StellarDetonation = 1081,

    [ParentCombo(AST_ST_DPS)]
    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value", AST.JobID)]
    AST_DPS_Lucid = 1008,

    #endregion

    #region AOE DPS

    [AutoAction(true, false)]
    [ReplaceSkill(AST.Gravity, AST.Gravity2)]
    [ConflictingCombos(AST_AOE_Simple_DPS)]
    [CustomComboInfo("Advanced DPS Mode - AoE", "Replaces Gravity with options below", AST.JobID)]
    [AdvancedCombo]
    AST_AOE_DPS = 1041,
    
    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Multitarget Dot Option", "Maintains dots on multiple targets.", AST.JobID)]
    AST_AOE_DPS_DoT = 1083,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Lightspeed Weave Option", "Adds Lightspeed when moving", AST.JobID)]
    AST_AOE_LightSpeed = 1048,

    [ParentCombo(AST_AOE_LightSpeed)]
    [CustomComboInfo("Lightspeed Hold Option", "Retains 1 Lightspeed charge for manual use", AST.JobID)]
    AST_AOE_LightSpeedHold = 1062,    

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Divination Weave Option", "Adds Divination", AST.JobID)]
    AST_AOE_Divination = 1043,

    [ParentCombo(AST_AOE_Divination)]
    [CustomComboInfo("Lightspeed Burst Option", "Add Lightspeed before Divination. \nCombine with Lightspeed Hold Options to ensure you have a charge to use", AST.JobID)]
    AST_AOE_LightspeedBurst = 1063,    

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Card Draw Weave Option", "Draws your cards", AST.JobID)]
    AST_AOE_AutoDraw = 1044,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Card Play Weave Option", "Weaves your Balance or Spear card (best used with Quick Target Cards)",
        AST.JobID)]
    [PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled, AST.Play1, AST.Balance, AST.Spear)]
    AST_AOE_AutoPlay = 1045,

    [ParentCombo(AST_AOE_AutoPlay)]
    [CustomComboInfo("Card Play Pooling Option", "Pools your dps cards for the Divination window",
        AST.JobID)]
    AST_AOE_CardPool = 1057,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Lord of Crowns Weave Option", "Adds Lord Of Crowns", AST.JobID)]
    AST_AOE_LazyLord = 1046,

    [ParentCombo(AST_AOE_LazyLord)]
    [CustomComboInfo("Lord of Crowns Pooling Option", "Holds Lord of Crowns for the Divination window", AST.JobID)]
    AST_AOE_LordPool = 1058,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Oracle Option", "Adds Oracle after Divination", AST.JobID)]
    AST_AOE_Oracle = 1047,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Earthly Star Option", "Adds Earthly Star placement, but not detonation, to the rotation.\nWill be targeted to your focus target, then soft and hard targets, before falling back to placing it at your feet.", AST.JobID)]
    [Retargeted(AST.EarthlyStar)]
    AST_AOE_DPS_EarthlyStar = 1052,
    
    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Stellar Detonation Option", "Adds early Stellar Detonation with Giant Dominance based on targets HP percentage and type of encounter", AST.JobID)]
    AST_AOE_DPS_StellarDetonation = 1082,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("MacroCosmos Option", "Adds Macrocosmos to the Aoe rotation after 3 GCDs", AST.JobID)]    
    AST_AOE_DPS_MacroCosmos = 1066,

    [ParentCombo(AST_AOE_DPS)]
    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value", AST.JobID)]
    AST_AOE_Lucid = 1042,

    #endregion

    #region Healing

    [AutoAction(false, true)]
    [ReplaceSkill(AST.Benefic2)]
    [CustomComboInfo("Advanced Healing Mode - Single Target", "Replaces Benefic II with a one button healing replacement.",
        AST.JobID)]
    [PossiblyRetargeted(AST.Benefic2)]
    [HealingCombo]
    AST_ST_Heals = 1023,
    
    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Esuna Option", "Applies Esuna to your target if there is a cleansable debuff.", AST.JobID)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    AST_ST_Heals_Esuna = 1039,
    
    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Aspected Benefic Option", "Adds Aspected Benefic & refreshes it if needed.", AST.JobID)]
    [PossiblyRetargeted(AST.AspectedBenefic)]
    AST_ST_Heals_AspectedBenefic = 1027,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Essential Dignity Option",
        "Essential Dignity will be added when the target is at or below the value set", AST.JobID)]
    [PossiblyRetargeted(AST.EssentialDignity)]
    AST_ST_Heals_EssentialDignity = 1024,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Celestial Intersection Option", "Adds Celestial Intersection.", AST.JobID)]
    [PossiblyRetargeted(AST.CelestialIntersection)]
    AST_ST_Heals_CelestialIntersection = 1025,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Exaltation Option", "Adds Exaltation.", AST.JobID)]
    [PossiblyRetargeted(AST.Exaltation)]
    AST_ST_Heals_Exaltation = 1028,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("The Spire Option", "Adds The Spire (Shield) when the card has been drawn", AST.JobID)]
    [PossiblyRetargeted(AST.Spire)]
    AST_ST_Heals_Spire = 1030,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("The Ewer Option", "Adds The Ewer (Heal over time) when the card has been drawn", AST.JobID)]
    [PossiblyRetargeted(AST.Ewer)]
    AST_ST_Heals_Ewer = 1032,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("The Arrow Option", "Adds The Arrow (increased healing)  when the card has been drawn", AST.JobID)]
    [PossiblyRetargeted(AST.Arrow)]
    AST_ST_Heals_Arrow = 1049,

    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("The Bole Option", "Adds The Bole (Reduced Damage) when the card has been drawn", AST.JobID)]
    [PossiblyRetargeted(AST.Bole)]
    AST_ST_Heals_Bole = 1050,
    
    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Celestial Opposition Option", "Adds Celestial Opposition", AST.JobID)]
    AST_ST_Heals_CelestialOpposition = 1068,
    
    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Collective Unconscious Option", "Adds Collective Unconscious (for the regen so it will not channel)", AST.JobID)]
    AST_ST_Heals_CollectiveUnconscious = 1069,
    
    [ParentCombo(AST_ST_Heals)]
    [CustomComboInfo("Lady Option", "Adds Lady of Crowns, if the card is drawn.", AST.JobID)]
    AST_ST_Heals_SoloLady = 1070,

    [AutoAction(true, true)]
    [ReplaceSkill(AST.Helios, AST.AspectedHelios, AST.HeliosConjuction)]
    [CustomComboInfo("Advanced Healing Mode - AoE",
        "Replaces Aspected Helios/Helios Conjunction or Helios with a one button healing replacement."
        + "This Spell will be consider the bottom priority with no checks regardless of below settings.", AST.JobID)]
    AST_AoE_Heals = 1010,
    
    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Aspected Helios Option", "Adds Aspected Helios/Helios Conjunction", AST.JobID)]
    AST_AoE_Heals_Aspected = 1053,
    
    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Helios Option", "Adds Helios", AST.JobID)]
    AST_AoE_Heals_Helios = 1073,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Celestial Opposition Option", "Adds Celestial Opposition", AST.JobID)]
    AST_AoE_Heals_CelestialOpposition = 1021,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Lazy Lady Option", "Adds Lady of Crowns, if the card is drawn", AST.JobID)]
    AST_AoE_Heals_LazyLady = 1022,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Horoscope Option", "Adds Horoscope followed by Aspected Helios or Helios.", AST.JobID)]
    AST_AoE_Heals_Horoscope = 1026,
    
    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Horoscope Helios Option", "Adds Horoscope Helios.", AST.JobID)]
    AST_AoE_Heals_HoroscopeHeal = 1071,

    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Neutral Sect Option", "Adds Neutral Sect and its followup Sun Sign.", AST.JobID)]
    AST_AoE_Heals_NeutralSect = 1067,
    
    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Stellar Detonation Option", "Adds Stellar Detonation when under the effect of Giant Dominance", AST.JobID)]
    AST_AoE_Heals_StellarDetonation = 1072,
    
    [ParentCombo(AST_AoE_Heals)]
    [CustomComboInfo("Collective Unconscious Option", "Adds Collective Unconscious (for the regen so it will not channel)", AST.JobID)]
    AST_AoE_Heals_CollectiveUnconscious = 1074,

    [ReplaceSkill(AST.Benefic2)]
    [CustomComboInfo("Benefic 2 Downgrade", "Changes Benefic 2 to Benefic when Benefic 2 is not unlocked or available.",
        AST.JobID)]
    AST_Benefic = 1002,

    #endregion

    #region Utility

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo("Alternative Raise Feature", "Changes Swiftcast to Ascend", AST.JobID)]
    AST_Raise_Alternative = 1003,

    [ParentCombo(AST_Raise_Alternative)]
    [CustomComboInfo("Retarget Raise", "Will Retarget the Raise affected here to your Heal Stack.", AST.JobID)]
    [Retargeted(AST.Ascend)]
    AST_Raise_Alternative_Retarget = 1060,

    [ReplaceSkill(AST.Lightspeed)]
    [CustomComboInfo("Lightspeed Overwrite Protection Feature", "Blocks the Lightspeed Button while buff is active.", AST.JobID)]
    AST_Lightspeed_Protection = 1065,    

    [ReplaceSkill(AST.EssentialDignity)]
    [CustomComboInfo("Retarget Essential Dignity Feature", "Will Retarget Essential Dignity outside of Healing combos to your Heal Stack.", AST.JobID)]
    [Retargeted(AST.EssentialDignity)]
    AST_RetargetEssentialDignity = 1059,

    [Variant]
    [VariantParent(AST_ST_DPS_CombustUptime)]
    [CustomComboInfo("Spirit Dart Option",
        "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", AST.JobID)]
    AST_Variant_SpiritDart = 1035,

    [Variant]
    [VariantParent(AST_ST_DPS)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", AST.JobID)]
    AST_Variant_Rampart = 1036,

    #endregion

    #region Cards

    [CustomComboInfo("Quick Target Damage Cards",
        "When you play the Balance or Spear in a combo, this will automatically target the buff on to a party member.\nIt will look at DPS that suit the card first, following The Balance's priorities; if none found or they have buffs already, will look at the other DPS instead.\nWill try to skip party members that have damage downs or rez sickness.\nWill default to you if no suitable party members were found.",
        AST.JobID)]
    [Retargeted(AST.Play1, AST.Balance, AST.Spear)]
    AST_Cards_QuickTargetCards = 1029,

    #endregion
    
    #region Raidwide Features
    [CustomComboInfo("Boss Raidwide Options",
        "Collection of tools designed to try and cast during a raidwide attack when detected." +
        "\nThis will work for most, but not all raidwide attacks and is no substitute for learning the fight", AST.JobID)]
    AST_Raidwide = 1075,
    
    [ParentCombo(AST_Raidwide)]
    [CustomComboInfo("RaidWide Collective Unconscious Option", "Additionally, Will try to Weave Collective Unconscious when a raidwide casting. \nWill be used in all 4 main combos.", AST.JobID)]
    AST_Raidwide_CollectiveUnconscious = 1076,
    
    [ParentCombo(AST_Raidwide)]
    [CustomComboInfo("RaidWide Neutral Sect Combo Option", "Additionally, Will try to Weave Neutral Sect and Sun sign when a raidwide casting. " +
                                                               "\nWill be used in all 4 main combos.", AST.JobID)]
    AST_Raidwide_NeutralSect = 1077,
    
    [ParentCombo(AST_Raidwide)]
    [CustomComboInfo("RaidWide Aspected Helios Option", "Additionally, Will try to cast Aspected Helios for with Neutral Sect Buff for shields when a raidwide casting. " +
                                                           "\nWill be used in all 4 main combos.", AST.JobID)]
    AST_Raidwide_AspectedHelios = 1078,
    
    #endregion

    // Last value = 1083

    #endregion

    #region BLACK MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_ST_AdvancedMode, BLM_Fire1to3, BLM_Fire1Despair)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Fire with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", BLM.JobID)]
    [SimpleCombo]
    BLM_ST_SimpleMode = 2001,

    [AutoAction(true, false)]
    [ReplaceSkill(BLM.Blizzard2, BLM.HighBlizzard2)]
    [ConflictingCombos(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Blizzard II with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", BLM.JobID)]
    [SimpleCombo]
    BLM_AoE_SimpleMode = 2002,

    #endregion

    #region Single Target - Advanced

    [AutoAction(false, false)]
    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_ST_SimpleMode, BLM_Fire1to3, BLM_Fire1Despair)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Fire with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", BLM.JobID)]
    [AdvancedCombo]
    BLM_ST_AdvancedMode = 2100,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", BLM.JobID)]
    BLM_ST_Opener = 2101,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Transpose Option", "Add Transpose to the rotation.", BLM.JobID)]
    BLM_ST_Transpose = 2114,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Leylines Option", "Add Leylines to the rotation.", BLM.JobID)]
    BLM_ST_LeyLines = 2103,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Amplifier Option", "Add Amplifier to the rotation.", BLM.JobID)]
    BLM_ST_Amplifier = 2102,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Manafont Option", "Add Manafont to the rotation.", BLM.JobID)]
    BLM_ST_Manafont = 2108,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("(High) Thunder Option", "Add (High) Thunder to the rotation.", BLM.JobID)]
    BLM_ST_Thunder = 2110,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Despair Option", "Add Despair to the rotation.", BLM.JobID)]
    BLM_ST_Despair = 2111,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Flare Star Option", "Add Flare Star to the rotation.", BLM.JobID)]
    BLM_ST_FlareStar = 2112,
    
    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Swiftcast Option", "Add Swiftcast to the rotation.", BLM.JobID)]
    BLM_ST_Swiftcast = 2106,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Triplecast Option", "Add Triplecast to the rotation.\nWill only be used if Swiftcast is on cooldown.", BLM.JobID)]
    BLM_ST_Triplecast = 2115,
    
    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Foul/Xenoglossy Option", "Add Foul/Xenoglossy to the rotation.", BLM.JobID)]
    BLM_ST_UsePolyglot = 2104,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Movement Option", "Add chosen options for movement.", BLM.JobID)]
    BLM_ST_Movement = 2113,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Scathe Option", "Add Scathe while moving and below lvl 66.", BLM.JobID)]
    BLM_ST_UseScathe = 2116,

    [ParentCombo(BLM_ST_AdvancedMode)]
    [CustomComboInfo("Manaward Option", "Adds Manaward to the rotation.", BLM.JobID)]
    BLM_ST_Manaward = 2199,

    #endregion

    #region AoE - Advanced

    [AutoAction(true, false)]
    [ReplaceSkill(BLM.Blizzard2, BLM.HighBlizzard2)]
    [ConflictingCombos(BLM_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Blizzard II with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", BLM.JobID)]
    [AdvancedCombo]
    BLM_AoE_AdvancedMode = 2200,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Transpose Option", "Add Transpose to the rotation.", BLM.JobID)]
    BLM_AoE_Transpose = 2212,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Leylines Option", "Add Leylines to the rotation.", BLM.JobID)]
    BLM_AoE_LeyLines = 2202,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Amplifier Option", "Add Amplifier to the rotation.", BLM.JobID)]
    BLM_AoE_Amplifier = 2201,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Manafont Option", "Add Manafont to the rotation.", BLM.JobID)]
    BLM_AoE_Manafont = 2207,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Triplecast Option", "Adds Triplecast to the rotation.", BLM.JobID)]
    BLM_AoE_Triplecast = 2208,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Paradox Filler Option", "Adds Paradox as a filler at max level to the rotation.", BLM.JobID)]
    BLM_AoE_ParadoxFiller = 2210,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("(High) Thunder II Option", "Add (High) Thunder II to the rotation.", BLM.JobID)]
    BLM_AoE_Thunder = 2209,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Foul Option", "Add Foul to the rotation.", BLM.JobID)]
    BLM_AoE_UsePolyglot = 2203,

    [ParentCombo(BLM_AoE_AdvancedMode)]
    [CustomComboInfo("Blizzard IV on 2 Targets Option", "Substitutes Freeze for Blizzard 4 whenever it would hit 2 targets.", BLM.JobID)]
    BLM_AoE_Blizzard4Sub = 2211,

    #endregion

    #region Variant

    [Variant]
    [VariantParent(BLM_ST_SimpleMode, BLM_AoE_SimpleMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", BLM.JobID)]
    BLM_Variant_Rampart = 2032,

    [Variant]
    [CustomComboInfo("Raise Option", "Turn Swiftcast into Variant Raise whenever you have the Swiftcast buff.", BLM.JobID)]
    BLM_Variant_Raise = 2033,

    [Variant]
    [VariantParent(BLM_ST_SimpleMode, BLM_AoE_SimpleMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", BLM.JobID)]
    BLM_Variant_Cure = 2034,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(BLM.Triplecast)]
    [CustomComboInfo("Triplecast Protection", "Replaces Triplecast with Savage Blade when you already have triplecast active.", BLM.JobID)]
    BLM_TriplecastProtection = 2056,

    [ReplaceSkill(BLM.Fire, BLM.Fire3)]
    [ConflictingCombos(BLM_ST_AdvancedMode, BLM_ST_SimpleMode, BLM_Fire1Despair)]
    [CustomComboInfo("Fire I/III Feature", "Replaces Fire I or Fire III.", BLM.JobID)]
    BLM_Fire1to3 = 2054,

    [ReplaceSkill(BLM.Blizzard, BLM.Blizzard3)]
    [ConflictingCombos(BLM_FreezeBlizzard2)]
    [CustomComboInfo("Blizzard I/III Feature", "Replaces Blizzard I or Blizzard III.\nReplaces Freeze with Blizzard II when synced below Lv.40.", BLM.JobID)]
    BLM_Blizzard1to3 = 2052,

    [ReplaceSkill(BLM.Fire4)]
    [ConflictingCombos(BLM_FireandIce)]
    [CustomComboInfo("Fire 4 to 3", "Replaces Fire 4 with Fire 3 when not in Astral Fire III or not in combat.", BLM.JobID)]
    BLM_Fire4to3 = 2059,

    [ReplaceSkill(BLM.Fire)]
    [ConflictingCombos(BLM_FireandIce, BLM_ST_AdvancedMode, BLM_ST_SimpleMode, BLM_Fire1to3)]
    [CustomComboInfo("Fire to Despair", "Replaces Fire  with Despair when in Astral Fire and below 2400 MP.", BLM.JobID)]
    BLM_Fire1Despair = 2065,

    [ReplaceSkill(BLM.Blizzard4)]
    [CustomComboInfo("Blizzard 4 to Despair", "Replaces Blizzard 4 with Despair when in Astral Fire.", BLM.JobID)]
    BLM_Blizzard4toDespair = 2060,
    
    [ReplaceSkill(BLM.Fire4, BLM.Flare)]
    [ConflictingCombos(BLM_Fire4to3)]
    [CustomComboInfo("Fire & Ice", "Replaces Fire4 with Blizzard4 when in Umbral Ice.\nReplaces Flare with Freeze when in Umbral Ice.", BLM.JobID)]
    BLM_FireandIce = 2057,

    [ReplaceSkill(BLM.Blizzard, BLM.Blizzard3)]
    [ConflictingCombos(BLM_FreezeParadox, BLM_Blizzard1to3)]
    [CustomComboInfo("Freeze to Blizzard II", "nReplaces Freeze with Blizzard II when synced below Lv.40.", BLM.JobID)]
    BLM_FreezeBlizzard2 = 2064,

    [ReplaceSkill(BLM.Fire4, BLM.Flare)]
    [ConflictingCombos(BLM_Fire4to3, BLM_FireandIce)]
    [CustomComboInfo(" Fire and Flare to Star", "Replaces Fire4 and Flare to Flarestar when on max stacks.", BLM.JobID)]
    BLM_FireFlarestar = 2058,

    [ReplaceSkill(BLM.Freeze)]
    [ConflictingCombos(BLM_FreezeBlizzard2)]
    [CustomComboInfo("Freeze to Paradox", "Replaces Freeze with Ice Paradox when you have 3 Umbral Heart stacks.", BLM.JobID)]
    BLM_FreezeParadox = 2062,

    [ReplaceSkill(BLM.FlareStar)]
    [CustomComboInfo("Flarestar to Paradox", "Replaces Flarestar with Fire Paradox when not at max Flarestar stacks.", BLM.JobID)]
    BLM_FlareParadox = 2063,

    [ReplaceSkill(BLM.Amplifier)]
    [CustomComboInfo("Amplifier to Xenoglossy", "Replaces Amplifier with Xenoglossy when at max Polyglot stacks.", BLM.JobID)]
    BLM_AmplifierXeno = 2061,
    
    [ReplaceSkill(BLM.Transpose)]
    [CustomComboInfo("Umbral Soul/Transpose Feature", "Replaces Transpose with Umbral Soul when Umbral Soul is available.", BLM.JobID)]
    BLM_UmbralSoul = 2050,

    [ReplaceSkill(BLM.Scathe)]
    [CustomComboInfo("Xenoglossy Feature", "Replaces Scathe with Xenoglossy when available.", BLM.JobID)]
    BLM_ScatheXeno = 2053,

    [ReplaceSkill(BLM.LeyLines)]
    [CustomComboInfo("Between the Ley Lines Feature", "Replaces Ley Lines with Between the Lines when Ley Lines is active.", BLM.JobID)]
    BLM_Between_The_LeyLines = 2051,

    [ReplaceSkill(BLM.AetherialManipulation)]
    [CustomComboInfo("Aetherial Manipulation Feature", "Replaces Aetherial Manipulation with Between the Lines when you are out of active Ley Lines and standing still.", BLM.JobID)]
    BLM_Aetherial_Manipulation = 2055,
    
    #endregion

    // Last value ST = 2117
    //Last Value AoE = 2212
    //Last Value misc = 2065

    #endregion

    #region BLUE MAGE

    [ReplaceSkill(BLU.MoonFlute)]
    [BlueInactive(BLU.Whistle, BLU.Tingle, BLU.RoseOfDestruction, BLU.MoonFlute, BLU.JKick, BLU.TripleTrident,
        BLU.Nightbloom, BLU.WingedReprobation, BLU.SeaShanty, BLU.BeingMortal, BLU.ShockStrike, BLU.Surpanakha,
        BLU.MatraMagic, BLU.PhantomFlurry, BLU.Bristle)]
    [ConflictingCombos(BLU_Opener)]
    [CustomComboInfo("BLU Moon Flute Opener (Level 80)",
        "Turns Moon Flute into a full opener.\nUse the remaining 2 charges of Winged Reprobation before starting the opener again!\nCan be done with 2.50 spell speed",
        BLU.JobID)]
    BLU_NewMoonFluteOpener = 70021,

    [BlueInactive(BLU.BreathOfMagic, BLU.MortalFlame)]
    [ParentCombo(BLU_NewMoonFluteOpener)]
    [CustomComboInfo("DoT Opener",
        "Changes the opener to apply either Mortal Flame or Breath of Magic instead of using Winged Reprobation.\nRequires 2.20 or faster spell speed",
        BLU.JobID)]
    BLU_NewMoonFluteOpener_DoTOpener = 70022,

    [BlueInactive(BLU.Whistle, BLU.Tingle, BLU.MoonFlute, BLU.JKick, BLU.TripleTrident, BLU.Nightbloom,
        BLU.RoseOfDestruction, BLU.FeatherRain, BLU.Bristle, BLU.GlassDance, BLU.Surpanakha, BLU.MatraMagic,
        BLU.ShockStrike, BLU.PhantomFlurry)]
    [ReplaceSkill(BLU.MoonFlute)]
    [ConflictingCombos(BLU_NewMoonFluteOpener)]
    [CustomComboInfo("BLU Moon Flute Opener (Level 70)",
        "Turns Moon Flute into a full opener. Here for historical value; level 80 opener is more potent.", BLU.JobID)]
    BLU_Opener = 70001,

    [BlueInactive(BLU.MoonFlute, BLU.Tingle, BLU.ShockStrike, BLU.Whistle, BLU.FinalSting)]
    [ReplaceSkill(BLU.FinalSting)]
    [CustomComboInfo("Final Sting Combo",
        "Turns Final Sting into the buff combo of: Moon Flute > Tingle > Whistle > Final Sting.", BLU.JobID)]
    BLU_FinalSting = 70002,

    [BlueInactive(BLU.RoseOfDestruction, BLU.FeatherRain, BLU.GlassDance, BLU.JKick)]
    [ParentCombo(BLU_FinalSting)]
    [CustomComboInfo("Off-cooldown Primal Additions",
        "Adds Rose of Destruction, Feather Rain, Glass Dance, and J Kick to the combo.", BLU.JobID)]
    BLU_Primals = 70003,

    [BlueInactive(BLU.BasicInstinct)]
    [ParentCombo(BLU_FinalSting)]
    [CustomComboInfo("Solo Mode", "Uses Basic Instinct if you're in an instance and on your own.", BLU.JobID)]
    BLU_SoloMode = 70011,

    [BlueInactive(BLU.RamsVoice, BLU.Ultravibration)]
    [ReplaceSkill(BLU.Ultravibration)]
    [CustomComboInfo("Vibe Combo",
        "Turns Ultravibration into Ram's Voice if Deep Freeze isn't on the target. Will swiftcast Ultravibration if available.",
        BLU.JobID)]
    BLU_Ultravibrate = 70005,

    [BlueInactive(BLU.HydroPull)]
    [ParentCombo(BLU_Ultravibrate)]
    [CustomComboInfo("Hydro Pull Setup", "Uses Hydro Pull before using Ram's Voice.", BLU.JobID)]
    BLU_HydroPull = 70012,

    [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance)]
    [ReplaceSkill(BLU.FeatherRain)]
    [CustomComboInfo("Primal Feature",
        "Turns Feather Rain into Shock Strike, Rose of Destruction, and Glass Dance.\nWill cause primals to desync from Moon Flute burst phases if used on cooldown.",
        BLU.JobID)]
    BLU_PrimalCombo = 70008,

    [BlueInactive(BLU.FeatherRain, BLU.ShockStrike, BLU.RoseOfDestruction, BLU.GlassDance)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Moon Flute Burst Pooling Option",
        "Holds spells if Moon Flute burst is about to occur and spells are off cooldown.", BLU.JobID)]
    BLU_PrimalCombo_Pool = 70015,

    [BlueInactive(BLU.JKick)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("J Kick Option", "Adds J Kick to the combo.", BLU.JobID)]
    BLU_PrimalCombo_JKick = 70013,

    [BlueInactive(BLU.SeaShanty)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Sea Shanty Option", "Adds Sea Shanty to the combo.", BLU.JobID)]
    BLU_PrimalCombo_SeaShanty = 70024,

    [BlueInactive(BLU.WingedReprobation)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Winged Reprobration Option", "Adds Winged Reprobation to the combo.", BLU.JobID)]
    BLU_PrimalCombo_WingedReprobation = 70025,

    [BlueInactive(BLU.MatraMagic)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Matra Magic Option", "Adds Matra Magic to the combo.", BLU.JobID)]
    BLU_PrimalCombo_Matra = 70017,

    [BlueInactive(BLU.Surpanakha)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Surpanakha Option", "Adds Surpanakha to the combo.", BLU.JobID)]
    BLU_PrimalCombo_Suparnakha = 70018,

    [BlueInactive(BLU.PhantomFlurry)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Phantom Flurry Option", "Adds Phantom Flurry to the combo.", BLU.JobID)]
    BLU_PrimalCombo_PhantomFlurry = 70019,

    [BlueInactive(BLU.Nightbloom, BLU.Bristle)]
    [ParentCombo(BLU_PrimalCombo)]
    [CustomComboInfo("Nightbloom Option", "Adds Nightbloom to the combo.", BLU.JobID)]
    BLU_PrimalCombo_Nightbloom = 70020,

    [BlueInactive(BLU.SongOfTorment, BLU.Bristle)]
    [ReplaceSkill(BLU.SongOfTorment)]
    [CustomComboInfo("Buffed Song of Torment", "Turns Song of Torment into Bristle so Song of Torment is buffed.",
        BLU.JobID)]
    BLU_BuffedSoT = 70000,

    [BlueInactive(BLU.PeripheralSynthesis, BLU.MustardBomb)]
    [ReplaceSkill(BLU.PeripheralSynthesis)]
    [CustomComboInfo("Peripheral Synthesis into Mustard Bomb",
        "Turns Peripheral Synthesis into Mustard Bomb when target is under the effect of Lightheaded.", BLU.JobID)]
    BLU_LightHeadedCombo = 70010,

    [BlueInactive(BLU.PerpetualRay, BLU.SharpenedKnife)]
    [CustomComboInfo("Perpetual Ray into Sharpened Knife",
        "Turns Perpetual Ray into Sharpened Knife when target is stunned and in melee range.", BLU.JobID)]
    BLU_PerpetualRayStunCombo = 70014,

    [BlueInactive(BLU.SonicBoom, BLU.SharpenedKnife)]
    [CustomComboInfo("Sonic Boom Melee", "Turns Sonic Boom into Sharpened Knife when in melee range.", BLU.JobID)]
    BLU_MeleeCombo = 70016,

    [BlueInactive(BLU.MagicHammer)]
    [ReplaceSkill(BLU.MagicHammer)]
    [CustomComboInfo("Addle & Magic Hammer Debuff", "Turns Magic Hammer into Addle when off cooldown.", BLU.JobID)]
    BLU_Addle = 70007,

    [BlueInactive(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
    [ReplaceSkill(BLU.BlackKnightsTour, BLU.WhiteKnightsTour)]
    [CustomComboInfo("Knight's Tour",
        "Turns Black Knight's Tour or White Knight's Tour into its counterpart when the enemy is under the effect of the spell's debuff.",
        BLU.JobID)]
    BLU_KnightCombo = 70009,

    [BlueInactive(BLU.Offguard, BLU.BadBreath, BLU.Devour)]
    [ReplaceSkill(BLU.Devour, BLU.Offguard, BLU.BadBreath)]
    [CustomComboInfo("Tank Debuff",
        "Puts Devour, Off-Guard, Lucid Dreaming, and Bad Breath into one button when under Tank Mimicry.", BLU.JobID)]
    BLU_DebuffCombo = 70006,

    [ReplaceSkill(BLU.DeepClean)]
    [BlueInactive(BLU.PeatPelt, BLU.DeepClean)]
    [CustomComboInfo("Peat Clean", "Changes Deep Clean to Peat Pelt if current target is not inflicted with Begrimed.",
        BLU.JobID)]
    BLU_PeatClean = 70023,

    // Last value = 70023

    #endregion

    #region BARD

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_AdvMode, BRD_StraightShotUpgrade)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Heavy Shot with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.",
        BRD.JobID)]
    [SimpleCombo]
    BRD_ST_SimpleMode = 3036,

    [AutoAction(true, false)]
    [ConflictingCombos(BRD_AoE_Combo, BRD_AoE_AdvMode)]
    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Quick Nock with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.",
        BRD.JobID)]
    [SimpleCombo]
    BRD_AoE_SimpleMode = 3035,

    #endregion

    #region Advanced Mode

    [AutoAction(false, false)]
    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_SimpleMode, BRD_StraightShotUpgrade)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Heavy Shot with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        BRD.JobID)]
    [AdvancedCombo]
    BRD_ST_AdvMode = 3009,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", BRD.JobID)]
    BRD_ST_Adv_Balance_Standard = 3048,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Bard Songs Option", "This option adds the Bard's Songs to the Advanced Bard Feature.", BRD.JobID)]
    BRD_Adv_Song = 3011,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Bard DoTs Option", "Enables the use of dot sub options below",
       BRD.JobID)]
    BRD_Adv_DoT = 3010,

    [ParentCombo(BRD_Adv_DoT)]
    [CustomComboInfo("Iron Jaws Option", "Enable the refreshing of dots with Ironjaws", BRD.JobID)]
    BRD_Adv_IronJaws = 3060,

    [ParentCombo(BRD_Adv_DoT)]
    [CustomComboInfo("Dot Application Option",
        "Enable the application of dots outside of the opener", BRD.JobID)]
    BRD_Adv_ApplyDots = 3059,

    [ParentCombo(BRD_Adv_DoT)]
    [CustomComboInfo("Raging Jaws Option",
        "Enable the snapshotting of DoTs, within the remaining time of Raging Strikes below:", BRD.JobID)]
    BRD_Adv_RagingJaws = 3025,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Buffs Option", "Adds buffs onto the Advanced Bard feature. \nEnable all to follow balance buff windows \nDisabling any buff will follow simple priority", BRD.JobID)]
    BRD_Adv_Buffs = 3017,

    [ParentCombo(BRD_Adv_Buffs)]
    [CustomComboInfo("Raging Strikes Option", "Adds Raging Strikes", BRD.JobID)]
    BRD_Adv_Buffs_Raging = 3049,

    [ParentCombo(BRD_Adv_Buffs)]
    [CustomComboInfo("Battlevoice Option", "Adds Battle Voice", BRD.JobID)]
    BRD_Adv_Buffs_Battlevoice = 3050,

    [ParentCombo(BRD_Adv_Buffs)]
    [CustomComboInfo("Radiant Finale Option", "Adds Radiant Finale", BRD.JobID)]
    BRD_Adv_Buffs_RadiantFinale = 3051,

    [ParentCombo(BRD_Adv_Buffs)]
    [CustomComboInfo("Barrage Option", "Adds Barrage", BRD.JobID)]
    BRD_Adv_Buffs_Barrage = 3052,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Resonant Option", "Adds Resonant Arrow to the Rotation after Barrage.", BRD.JobID)]
    BRD_Adv_BuffsResonant = 3041,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Encore Option", "Adds Radiant Encore to the Rotation after Finale.", BRD.JobID)]
    BRD_Adv_BuffsEncore = 3042,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Apex Arrow Option", "Adds Apex Arrow and Blast shot", BRD.JobID)]
    BRD_ST_ApexArrow = 3021,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("oGcd Option",
        "Weave Sidewinder, Empyreal arrow, Rain of death, and Pitch perfect when available.", BRD.JobID)]
    BRD_ST_Adv_oGCD = 3038,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Pooling Option", "Pools Bloodletter charges and Sidewinder to allow for optimum burst phases ", BRD.JobID)]
    BRD_Adv_Pooling = 3023,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Apex Pooling Option", "Pools Apex arrow into buff window and when buffs are between 50-60 seconds on cd.", BRD.JobID)]
    BRD_Adv_ApexPooling = 3057,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Interrupt Option", "Uses interrupt during the rotation if applicable.", BRD.JobID)]
    BRD_Adv_Interrupt = 3020,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Second Wind Option", "Uses Second Wind when below set HP percentage.", BRD.JobID)]
    BRD_ST_SecondWind = 3028,

    [ParentCombo(BRD_ST_AdvMode)]
    [CustomComboInfo("Self Cleanse Option", "Uses Wardens Paeon when you have a cleansable debuff.", BRD.JobID)]
    BRD_ST_Wardens = 3047,

    [ParentCombo(BRD_ST_Wardens)]
    [CustomComboInfo("Party Cleanse Option", "Uses Wardens Paeon when someone in the party has a cleansable debuff using the Retargeting Function following party list", BRD.JobID)]
    [Retargeted(BRD.TheWardensPaeon)]
    BRD_ST_WardensAuto = 3064,

    [AutoAction(true, false)]
    [ConflictingCombos(BRD_AoE_Combo, BRD_AoE_SimpleMode)]
    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Quick Nock with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        BRD.JobID)]
    [AdvancedCombo]
    BRD_AoE_AdvMode = 3015,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Bard Song Option", "Weave Songs on the Advanced AoE.", BRD.JobID)]
    BRD_AoE_Adv_Songs = 3016,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("AoE Buffs Option", "Adds buffs onto the Advance AoE Bard feature.\nEnable all to follow balance buff windows \nDisabling any buff will follow simple priority", BRD.JobID)]
    BRD_AoE_Adv_Buffs = 3032,

    [ParentCombo(BRD_AoE_Adv_Buffs)]
    [CustomComboInfo("Raging Strikes Option", "Adds Raging Strikes", BRD.JobID)]
    BRD_AoE_Adv_Buffs_Raging = 3053,

    [ParentCombo(BRD_AoE_Adv_Buffs)]
    [CustomComboInfo("Battlevoice Option", "Adds Battle Voice", BRD.JobID)]
    BRD_AoE_Adv_Buffs_Battlevoice = 3054,

    [ParentCombo(BRD_AoE_Adv_Buffs)]
    [CustomComboInfo("Radiant Finale Option", "Adds Radiant Finale", BRD.JobID)]
    BRD_AoE_Adv_Buffs_RadiantFinale = 3055,

    [ParentCombo(BRD_AoE_Adv_Buffs)]
    [CustomComboInfo("Barrage Option", "Adds Barrage", BRD.JobID)]
    BRD_AoE_Adv_Buffs_Barrage = 3056,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("oGcd Option",
       "Weave Sidewinder, Empyreal arrow, Rain of death, and Pitch perfect when available.", BRD.JobID)]
    BRD_AoE_Adv_oGCD = 3037,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Pooling Option", "Pools Rain of death charges to allow for optimum burst phases.", BRD.JobID)]
    BRD_AoE_Pooling = 3040,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Apex Pooling Option", "Pools Apex arrow into buff window and when buffs are between 50-60 seconds on cd.", BRD.JobID)]
    BRD_AoE_ApexPooling = 3058,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Interrupt Option", "Uses interrupt during the rotation if applicable.", BRD.JobID)]
    BRD_AoE_Adv_Interrupt = 3043,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Encore Option", "Adds Radiant Encore to the Rotation after Finale.", BRD.JobID)]
    BRD_AoE_BuffsEncore = 3062,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Resonant Option", "Adds Resonant Arrow to the Rotation after Barrage.", BRD.JobID)]
    BRD_AoE_BuffsResonant = 3061,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Apex Arrow Option", "Adds Apex Arrow and Blast shot", BRD.JobID)]
    BRD_AoE_ApexArrow = 3039,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Second Wind Option", "Uses Second Wind when below set HP percentage.", BRD.JobID)]
    BRD_AoE_SecondWind = 3029,

    [ParentCombo(BRD_AoE_AdvMode)]
    [CustomComboInfo("Self Cleanse Option", "Uses Wardens Paeon when you have a cleansable debuff.", BRD.JobID)]
    BRD_AoE_Wardens = 3046,

    [ParentCombo(BRD_AoE_Wardens)]
    [CustomComboInfo("Party Cleanse Option", "Uses Wardens Paeon when someone in the party has a cleansable debuff using the Retargeting Function following party list.", BRD.JobID)]
    [Retargeted(BRD.TheWardensPaeon)]
    BRD_AoE_WardensAuto = 3063,

    #endregion

    #region Smaller Features

    [ReplaceSkill(BRD.HeavyShot, BRD.BurstShot)]
    [ConflictingCombos(BRD_ST_AdvMode, BRD_ST_SimpleMode)]
    [CustomComboInfo("Heavy Shot into Straight Shot Feature",
        "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced.", BRD.JobID)]
    BRD_StraightShotUpgrade = 3001,

    [ParentCombo(BRD_StraightShotUpgrade)]
    [CustomComboInfo("DoT Maintenance Option",
        "Enabling this option will make Heavy Shot into Straight Shot refresh your DoTs on your current.", BRD.JobID
    )]
    BRD_DoTMaintainance = 3002,

    [ParentCombo(BRD_StraightShotUpgrade)]
    [CustomComboInfo("Apex Arrow Option",
        "Replaces Burst Shot with Apex Arrow when gauge is full and Blast Arrow when you are Blast Arrow ready.",
        BRD.JobID)]
    BRD_ApexST = 3034,

    [ReplaceSkill(BRD.IronJaws)]
    [ConflictingCombos(BRD_IronJaws_Alternate)]
    [CustomComboInfo("Iron Jaws Feature",
        "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nAlternates between the two if Iron Jaws isn't available.",
        BRD.JobID)]
    BRD_IronJaws = 3003,

    [ParentCombo(BRD_IronJaws)]
    [CustomComboInfo("Iron Jaws Apex Option", "Adds Apex and Blast Arrow to Iron Jaws when available.", BRD.JobID)]
    BRD_IronJawsApex = 3024,

    [ReplaceSkill(BRD.IronJaws)]
    [ConflictingCombos(BRD_IronJaws)]
    [CustomComboInfo("Iron Jaws Alternate Feature",
        "Iron Jaws is replaced with Caustic Bite/Stormbite if one or both are not up.\nIron Jaws will only show up when debuffs are about to expire.",
        BRD.JobID)]
    BRD_IronJaws_Alternate = 3004,

    [ReplaceSkill(BRD.QuickNock, BRD.Ladonsbite)]
    [ConflictingCombos(BRD_AoE_AdvMode, BRD_AoE_SimpleMode)]
    [CustomComboInfo("Quick Nock Feature", "Replaces Quick Nock/Ladonsbite with Shadowbite when ready.", BRD.JobID)]
    BRD_AoE_Combo = 3008,

    [ParentCombo(BRD_AoE_Combo)]
    [CustomComboInfo("Apex Arrow Option",
        "Replaces Ladonsbite and Quick Nock with Apex Arrow when gauge is full and Blast Arrow when you are Blast Arrow ready.",
        BRD.JobID)]
    BRD_Apex = 3005,

    [ReplaceSkill(BRD.Bloodletter)]
    [CustomComboInfo("Single Target oGCD Feature", "All oGCD's on Bloodletter/Heartbreakshot", BRD.JobID)]
    BRD_ST_oGCD = 3006,

    [ParentCombo(BRD_ST_oGCD)]
    [CustomComboInfo("Quick song option", "Adds the songs to the oGCD feature. Wanderers > Mages > Armys", BRD.JobID
    )]
    BRD_ST_oGCD_Songs = 3044,

    [ReplaceSkill(BRD.RainOfDeath)]
    [CustomComboInfo("AoE oGCD Feature", "All AoE oGCD's on Rain of Death depending on their CD.", BRD.JobID)]
    BRD_AoE_oGCD = 3007,

    [ParentCombo(BRD_AoE_oGCD)]
    [CustomComboInfo("Quick song option", "Adds the songs to the AoE oGCD feature. Wanderers > Mages > Armys",
        BRD.JobID)]
    BRD_AoE_oGCD_Songs = 3045,

    [ReplaceSkill(BRD.Barrage)]
    [CustomComboInfo("Bard Buffs Feature", "Adds Raging Strikes and Battle Voice onto Barrage.", BRD.JobID)]
    BRD_Buffs = 3013,

    [ReplaceSkill(BRD.WanderersMinuet)]
    [CustomComboInfo("One Button Songs Feature",
        "Add Mage's Ballad and Army's Paeon to Wanderer's Minuet depending on cooldowns.", BRD.JobID)]
    BRD_OneButtonSongs = 3014,

    #endregion

    #region Variants
    [Variant]
    [VariantParent(BRD_ST_AdvMode, BRD_AoE_AdvMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", BRD.JobID)]
    BRD_Variant_Rampart = 3030,

    [Variant]
    [VariantParent(BRD_ST_AdvMode, BRD_AoE_AdvMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", BRD.JobID)]
    BRD_Variant_Cure = 3031,

    #endregion

    // Last value = 3063

    #endregion

    #region DANCER

    [ReplaceSkill(DNC.StandardFinish2, DNC.TechnicalFinish4)]
    [CustomComboInfo("Require Nearby Enemy for Finishes Feature",
        "Will hold Standard Finish and Technical Finish until an enemy is within range of the abilities in all (non-Simple) Modes and Features below by replacing whatever button with Savage Blade." +
        "\nWill show either Finish when the dance is about to expire." +
        "\nThis behavior is recommended by The Balance but can introduce drift, so it may not be what is best for your group.", DNC.JobID)]
    DNC_ST_BlockFinishes = 4000,

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_MultiButton, DNC_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Cascade with a full one-button single target rotation." +
        "\nEmploys the Forced Triple Weave Anti-Drift solution.", DNC.JobID)]
    [SimpleCombo]
    DNC_ST_SimpleMode = 4001,

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Windmill with a full one-button AoE rotation.", DNC.JobID)]
    [SimpleCombo]
    DNC_AoE_SimpleMode = 4002,

    #endregion
    // Last value = 4002
    
    #region Advanced Dancer (Single Target)

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_MultiButton, DNC_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Cascade with a full one-button single target rotation." +
        "\nThis mode is ideal if you want to customize the rotation.", DNC.JobID)]
    [AdvancedCombo]
    DNC_ST_AdvancedMode = 4010,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)",
        "Adds the Balance opener at level 100." +
        "\nRequirements:" +
        "\n- Standard Step ready" +
        "\n- Technical Step ready" +
        "\n- Devilment ready" +
        "\n(Will change to Savage Blade to wait for the countdown)" +
        "\n(REQUIRES a countdown)", DNC.JobID)]
    DNC_ST_BalanceOpener = 4011,

    [ParentCombo(DNC_ST_BalanceOpener)]
    [CustomComboInfo("Wait for Countdown Options", "When not in combat will change to Savage Blade to wait for a countdown to appear.\nMostly here to allow for targeting the boss before the exact countdown window your opener selection is waiting for.\nONLY designed to be something you turn on while doing back-to-back pulls of content, NOT suitable to leave on all the time.", DNC.JobID)]
    DNC_ST_Opener_BlockEarly = 4031,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Dance Partner Reminder Option", "Includes Closed Position when out of combat and you do not have a dance partner.", DNC.JobID)]
    DNC_ST_Adv_Partner = 4012,

    [ParentCombo(DNC_ST_Adv_Partner)]
    [CustomComboInfo("Retarget to Optimal Partner", "Will Retarget the Closed Position shown out of combat to your optimal partner.", DNC.JobID)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_ST_Adv_PartnerAuto = 4033,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Optimal Dance Partner Option", "Includes Ending and then Closed Position in the rotation when your current partner is not the optimal one, such as when they get a damage down.", DNC.JobID)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_ST_Adv_AutoPartner = 4032,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Peloton Pre-Pull Option",
        "Uses Peloton when you are out of combat, do not already have the Peloton buff and are performing Standard Step with greater than 5s remaining of your dance.", DNC.JobID)]
    DNC_ST_Adv_Peloton = 4013,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Interrupt Option", "Includes an interrupt in the rotation (if applicable to your current target).", DNC.JobID)]
    DNC_ST_Adv_Interrupt = 4014,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Standard Dance Option", "Include all dance steps, and Finish, and optionally Standard Step, in the rotation.", DNC.JobID)]
    DNC_ST_Adv_SS = 4015,

    [ParentCombo(DNC_ST_Adv_SS)]
    [CustomComboInfo("Finishing Move Option", "Includes Finishing Move in the rotation.", DNC.JobID)]
    DNC_ST_Adv_FM = 4016,

    [ParentCombo(DNC_ST_Adv_SS)]
    [CustomComboInfo("Standard Dance Pre-Pull Option",
        "Starts Standard Step (and steps) before combat." +
        "\n(Already included in The Balance Opener).", DNC.JobID)]
    DNC_ST_Adv_SS_Prepull = 4017,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Technical Dance Option", "Include all dance steps, and Finish, and optionally Technical Step, in the rotation.", DNC.JobID)]
    DNC_ST_Adv_TS = 4018,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Devilment Option",
        "Includes Devilment in the rotation." +
        "\nWill activate only during Technical Finish if you're Lv70 or above." +
        "\nWill be used on cooldown below Lv70.", DNC.JobID)]
    DNC_ST_Adv_Devilment = 4019,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Flourish Option", "Includes Flourish in the rotation.", DNC.JobID)]
    DNC_ST_Adv_Flourish = 4020,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Fan Dance Procc Options",
        "Options for including Fan Dance 3 and 4 into the rotation." +
        "\nNote: If using the Forced Triple Weave option, FD3&4 will be used regardless of these options.", DNC.JobID)]
    DNC_ST_Adv_FanProccs = 4028,

    [ParentCombo(DNC_ST_Adv_FanProccs)]
    [CustomComboInfo("Fan Dance 3", "Includes Fan Dance 3 when under Threefold Fan Dance.", DNC.JobID)]
    DNC_ST_Adv_FanProcc3 = 4029,

    [ParentCombo(DNC_ST_Adv_FanProccs)]
    [CustomComboInfo("Fan Dance 4", "Includes Fan Dance 4 when under Fourfold Fan Dance.", DNC.JobID)]
    DNC_ST_Adv_FanProcc4 = 4030,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Feathers Option",
        "Expends a feather in the next available weave window when capped and under the effect of Flourishing Symmetry or Flourishing Flow." +
        "\nWeaves feathers where possible during Technical Finish." +
        "\nWeaves feathers outside of burst when target is below set HP percentage (Set to 0 to disable)." +
        "\nWeaves feathers whenever available when under Lv.70.", DNC.JobID)]
    DNC_ST_Adv_Feathers = 4021,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Improvisation Option",
        "Includes Improvisation in the rotation when available." +
        "\nWill not use while under Technical Finish", DNC.JobID)]
    DNC_ST_Adv_Improvisation = 4022,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Tillana Option", "Includes Tillana in the rotation.", DNC.JobID)]
    DNC_ST_Adv_Tillana = 4023,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Saber Dance Option",
        "Includes Saber Dance in the rotation when at or over the Esprit threshold." +
        "\n(And to prevent overcapping while under Technical Finish)", DNC.JobID)]
    DNC_ST_Adv_SaberDance = 4024,

    [ParentCombo(DNC_ST_Adv_SaberDance)]
    [CustomComboInfo("Dance of the Dawn Option",
        "Includes Dance of the Dawn in the rotation after Saber Dance and when over the threshold, or in the final seconds of Dance of the Dawn ready.",
        DNC.JobID)]
    DNC_ST_Adv_DawnDance = 4025,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Last Dance Option", "Includes Last Dance in the rotation.", DNC.JobID)]
    DNC_ST_Adv_LD = 4026,

    [ParentCombo(DNC_ST_AdvancedMode)]
    [CustomComboInfo("Panic Heals Option",
        "Includes Curing Waltz and Second Wind in the rotation when available and your HP is below the set percentages.",
        DNC.JobID)]
    DNC_ST_Adv_PanicHeals = 4027,

    #endregion
    // Last value = 4033

    #region Advanced Dancer (AoE)

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_MultiButton, DNC_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Windmill with a full one-button AoE rotation." +
        "\nThis mode is ideal if you want to customize the rotation.", DNC.JobID)]
    [AdvancedCombo]
    DNC_AoE_AdvancedMode = 4040,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Dance Partner Reminder Option",
        "Includes Closed Position when out of combat and no dance partner is found.", DNC.JobID)]
    DNC_AoE_Adv_Partner = 4041,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Interrupt Option",
        "Includes an interrupt in the AoE rotation (if your current target can be interrupted).", DNC.JobID)]
    DNC_AoE_Adv_Interrupt = 4042,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Standard Dance Option", "Include all dance steps, and Finish, and optionally Standard Step, in the AoE rotation.", DNC.JobID)]
    DNC_AoE_Adv_SS = 4043,

    [ParentCombo(DNC_AoE_Adv_SS)]
    [CustomComboInfo("Finishing Move Option", "Includes Finishing Move in the AoE rotation.", DNC.JobID)]
    DNC_AoE_Adv_FM = 4044,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Technical Dance Option", "Include all dance steps, and Finish, and optionally Technical Step, in the AoE rotation.", DNC.JobID)]
    DNC_AoE_Adv_TS = 4045,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Tech Devilment Option",
        "Includes Devilment in the AoE rotation." +
        "\nWill activate only during Technical Finish if you're Lv70 or above." +
        "\nWill be used on cooldown below Lv70.", DNC.JobID)]
    DNC_AoE_Adv_Devilment = 4046,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Flourish Option", "Includes Flourish in the AoE rotation.", DNC.JobID)]
    DNC_AoE_Adv_Flourish = 4047,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Fan Dance Procc Options", "Options for including Fan Dance 3 and 4 into the rotation.", DNC.JobID)]
    DNC_AoE_Adv_FanProccs = 4055,

    [ParentCombo(DNC_AoE_Adv_FanProccs)]
    [CustomComboInfo("Fan Dance 3", "Includes Fan Dance 3 when under Threefold Fan Dance.", DNC.JobID)]
    DNC_AoE_Adv_FanProcc3 = 4056,

    [ParentCombo(DNC_AoE_Adv_FanProccs)]
    [CustomComboInfo("Fan Dance 4", "Includes Fan Dance 4 when under Fourfold Fan Dance.", DNC.JobID)]
    DNC_AoE_Adv_FanProcc4 = 4057,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Feathers Option",
        "Expends a feather in the next available weave window when capped and under the effect of Flourishing Symmetry or Flourishing Flow." +
        "\nWeaves feathers where possible during Technical Finish." +
        "\nWeaves feathers whenever available when under Lv.70.", DNC.JobID)]
    DNC_AoE_Adv_Feathers = 4048,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Improvisation Option",
        "Includes Improvisation in the AoE rotation when available." +
        "\nWill not use while under Technical Finish", DNC.JobID)]
    DNC_AoE_Adv_Improvisation = 4049,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Tillana Option", "Includes Tillana in the rotation.", DNC.JobID)]
    DNC_AoE_Adv_Tillana = 4050,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Saber Dance Option",
        "Includes Saber Dance in the AoE rotation when at or over the Esprit threshold." +
        "\n(And to prevent overcapping while under Technical Finish)", DNC.JobID)]
    DNC_AoE_Adv_SaberDance = 4051,

    [ParentCombo(DNC_AoE_Adv_SaberDance)]
    [CustomComboInfo("Dance of the Dawn Option",
        "Includes Dance of the Dawn in the AoE rotation after Saber Dance and when over the threshold, or in the final seconds of Dance of the Dawn ready.",
        DNC.JobID)]
    DNC_AoE_Adv_DawnDance = 4052,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Last Dance Option", "Includes Last Dance in the rotation.", DNC.JobID)]
    DNC_AoE_Adv_LD = 4053,

    [ParentCombo(DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Panic Heals Option",
        "Includes Curing Waltz and Second Wind in the AoE rotation when available and your HP is below the set percentages.",
        DNC.JobID)]
    DNC_AoE_Adv_PanicHeals = 4054,

    #endregion
    // Last value = 4057

    #region Basic combo

    [ReplaceSkill(DNC.Fountain)]
    [CustomComboInfo("Basic Combo", "Replace Fountain with its combo chain.", DNC.JobID)]
    [BasicCombo]
    DNC_ST_BasicCombo = 4003,

    #endregion
    // Last value = 4003

    #region Multibutton Features

    #region Single Target Multibutton

    [AutoAction(false, false)]
    [ReplaceSkill(DNC.Cascade)]
    [ConflictingCombos(DNC_ST_AdvancedMode, DNC_ST_SimpleMode)]
    [CustomComboInfo("Single Target Multibutton Feature", "Single target combo with Fan Dances and Esprit use.",
        DNC.JobID)]
    DNC_ST_MultiButton = 4070,

    [ParentCombo(DNC_ST_MultiButton)]
    [CustomComboInfo("Esprit Overcap Option", "Adds Saber Dance above the set Esprit threshold.", DNC.JobID)]
    DNC_ST_EspritOvercap = 4071,

    [ParentCombo(DNC_ST_MultiButton)]
    [CustomComboInfo("Fan Dance Overcap Protection Option", "Adds Fan Dance 1 when Fourfold Feathers are full.",
        DNC.JobID)]
    DNC_ST_FanDanceOvercap = 4072,

    [ParentCombo(DNC_ST_MultiButton)]
    [CustomComboInfo("Fan Dance Option", "Adds Fan Dance 3/4 when available.", DNC.JobID)]
    DNC_ST_FanDance34 = 4073,

    #endregion
    // Last value = 4073

    #region AoE Multibutton

    [AutoAction(true, false)]
    [ReplaceSkill(DNC.Windmill)]
    [ConflictingCombos(DNC_AoE_AdvancedMode, DNC_AoE_SimpleMode)]
    [CustomComboInfo("AoE Multibutton Feature", "AoE combo with Fan Dances and Esprit use.", DNC.JobID)]
    DNC_AoE_MultiButton = 4090,

    [ParentCombo(DNC_AoE_MultiButton)]
    [CustomComboInfo("Esprit Overcap Option", "Adds Saber Dance above the set Esprit threshold.", DNC.JobID)]
    DNC_AoE_EspritOvercap = 4091,

    [ParentCombo(DNC_AoE_MultiButton)]
    [CustomComboInfo("AoE Fan Dance Overcap Protection Option", "Adds Fan Dance 2 when Fourfold Feathers are full.",
        DNC.JobID)]
    DNC_AoE_FanDanceOvercap = 4092,

    [ParentCombo(DNC_AoE_MultiButton)]
    [CustomComboInfo("AoE Fan Dance Option", "Adds Fan Dance 3/4 when available.", DNC.JobID)]
    DNC_AoE_FanDance34 = 4093,

    #endregion
    // Last value = 4093

    #region Smaller Features

    #region Dance Partner Features

    [ReplaceSkill(DNC.ClosedPosition, DNC.Ending)]
    [CustomComboInfo("Dance Partner on Desirable Partner Feature",
        "Replaces Closed Position with itself, but targeted to a party member, so you can cast it without having to pick anyone and without having to actually target anyone.\nWill show Ending when your current Partner is no longer the best choice.", DNC.JobID)]
    [Retargeted(DNC.ClosedPosition)]
    DNC_DesirablePartner = 4175,

    #endregion
    // Last value = 4176

    #region Dance Features

    [CustomComboInfo("Custom Dance Step Feature",
        "Change custom actions into dance steps while dancing." +
        "\nLets you still dance with combos on, without using Step Combo Features above.", DNC.JobID)]
    DNC_CustomDanceSteps = 4115,

    [ParentCombo(DNC_CustomDanceSteps)]
    [CustomComboInfo("Override Smaller Features", "If enabled, will let you choose actions that are replaced by the smaller features listed below here, and they will return the Step that you have set them to when dancing.", DNC.JobID)]
    DNC_CustomDanceSteps_Conflicts = 4116,

    [CustomComboInfo("Dance Features", "Small Features for Standard and Technical Step", DNC.JobID)]
    DNC_DanceFeatures = 4111,

    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.StandardStep)]
    [CustomComboInfo("Standard Step Combo Feature",
        "Change Standard Step into each dance step, while dancing.", DNC.JobID)]
    DNC_StandardStepCombo = 4110,

    // StandardStep(or Finishing Move) --> Last Dance
    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.StandardStep, DNC.FinishingMove)]
    [CustomComboInfo("Standard Step to Last Dance Feature",
        "Change Standard Step or Finishing Move to Last Dance when available.", DNC.JobID)]
    DNC_StandardStep_LastDance = 4155,

    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.TechnicalStep)]
    [CustomComboInfo("Technical Step Combo Feature",
        "Change Technical Step into each dance step, while dancing.", DNC.JobID)]
    DNC_TechnicalStepCombo = 4112,

    // Technical Step --> Devilment
    [ParentCombo(DNC_DanceFeatures)]
    [ReplaceSkill(DNC.TechnicalStep)]
    [CustomComboInfo("Technical Step to Devilment Feature", "Change Technical Step to Devilment as soon as possible.",
        DNC.JobID)]
    DNC_TechnicalStep_Devilment = 4160,

    #endregion
    // Last value = 4116

    #region Fan Features

    [ReplaceSkill(DNC.Flourish)]
    [CustomComboInfo("Flourishing Fan Dance Feature",
        "Replace Flourish with Fan Dance 4 during weave-windows, when Flourish is on cooldown.", DNC.JobID)]
    DNC_FlourishingFanDances = 4130,

    [ParentCombo(DNC_FlourishingFanDances)]
    [CustomComboInfo("Fan Dance 3 Option",
        "Include Fan Dance 3 before 4.", DNC.JobID)]
    DNC_Flourishing_FD3 = 4131,

    [CustomComboInfo("Fan Dance Combo Feature",
        "Options for Fan Dance combos." +
        "\nFan Dance 3 takes priority over Fan Dance 4.", DNC.JobID)]
    DNC_FanDanceCombos = 4135,

    [ReplaceSkill(DNC.FanDance1)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo("Fan Dance 1 -> 3 Option", "Changes Fan Dance 1 to Fan Dance 3 when available.", DNC.JobID)]
    DNC_FanDance_1to3_Combo = 4136,

    [ReplaceSkill(DNC.FanDance1)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo("Fan Dance 1 -> 4 Option", "Changes Fan Dance 1 to Fan Dance 4 when available.", DNC.JobID)]
    DNC_FanDance_1to4_Combo = 4137,

    [ReplaceSkill(DNC.FanDance2)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo("Fan Dance 2 -> 3 Option", "Changes Fan Dance 2 to Fan Dance 3 when available.", DNC.JobID)]
    DNC_FanDance_2to3_Combo = 4138,

    [ReplaceSkill(DNC.FanDance2)]
    [ParentCombo(DNC_FanDanceCombos)]
    [CustomComboInfo("Fan Dance 2 -> 4 Option", "Changes Fan Dance 2 to Fan Dance 4 when available.", DNC.JobID)]
    DNC_FanDance_2to4_Combo = 4139,

    #endregion
    // Last value = 4139

    // Bladeshower --> Bloodshower
    [ReplaceSkill(DNC.Bladeshower)]
    [CustomComboInfo("Bladeshower to Bloodshower Feature", "Change Bladeshower to Bloodshower when available.", DNC.JobID)]
    DNC_Procc_Bladeshower = 4165,

    // Windmill --> Rising Windmill
    [ReplaceSkill(DNC.Windmill)]
    [CustomComboInfo("Windmill to Rising Windmill Feature", "Change Windmill to Rising Windmill when available.", DNC.JobID)]
    DNC_Procc_Windmill = 4170,

    #endregion
    // Last value = 4176

    #endregion
    // Last value = 4176

    #region Variant

    [Variant]
    [VariantParent(DNC_ST_AdvancedMode, DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", DNC.JobID)]
    DNC_Variant_Rampart = 4190,

    [Variant]
    [VariantParent(DNC_ST_AdvancedMode, DNC_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", DNC.JobID)]
    DNC_Variant_Cure = 4195,

    #endregion
    // Last value = 4195

    #endregion

    #region DARK KNIGHT

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DRK.HardSlash)]
    [ConflictingCombos(DRK_ST_Adv)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Hard Slash with a full one-button single target rotation.", DRK.JobID)]
    [SimpleCombo]
    DRK_ST_Simple = 5001,

    [AutoAction(true, false)]
    [ReplaceSkill(DRK.Unleash)]
    [ConflictingCombos(DRK_AoE_Adv)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Unleash with a full one-button AoE rotation.", DRK.JobID)]
    [SimpleCombo]
    DRK_AoE_Simple = 5002,

    #endregion
    // Last value = 5003

    #region Advanced Single Target Combo

    [AutoAction(false, false)]
    [ReplaceSkill(DRK.HardSlash)]
    [ConflictingCombos(DRK_ST_Simple)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Hard Slash with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        DRK.JobID)]
    [AdvancedCombo]
    DRK_ST_Adv = 5010,

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo("Balance Opener (Level 100)",
        "Adds the Balance opener at level 100." +
        "\nRequirements:" +
        "\n- Over 7,000 mana" +
        "\n- 2 Shadowbringer charges ready" +
        "\n- Living Shadow off cooldown" +
        "\n- Delirium off cooldown" +
        "\n- Carve and Spit off cooldown" +
        "\n- Salted Earth off cooldown" +
        "\n(supports TBN'ing during use or pre-pull)",
        DRK.JobID)]
    DRK_ST_BalanceOpener = 5011,

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo("Unmend Uptime Option", "Adds Unmend to the rotation when you are out of range.", DRK.JobID)]
    DRK_ST_RangedUptime = 5012,

    #region Cooldowns

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo("Cooldown Options", "Collection of cooldowns to add to the rotation.", DRK.JobID)]
    DRK_ST_CDs = 5013,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", DRK.JobID)]
    DRK_ST_CD_Interrupt = 5014,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Interrupt with Stun Option", "Adds Low Blow to the rotation when your target is casting.\nNot advised outside of overworld content, as it can waste a lot of Low Blows on un-stun-able enemies, etc. Will try to not use it in boss fights.", DRK.JobID)]
    DRK_ST_CD_Stun = 5040,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Delirium on Cooldown",
        "Adds Delirium (or Blood Weapon at lower levels) to the rotation on cooldown and when Darkside is up.\n Will also spend 50 blood gauge if Delirium is nearly ready to protect from overcap.",
        DRK.JobID)]
    DRK_ST_CD_Delirium = 5015,

    #region Living Shadow Options

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Living Shadow Option", "Adds Living Shadow to the rotation when Darkside is up.", DRK.JobID)]
    DRK_ST_CD_Shadow = 5016,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Disesteem Option", "Adds Disesteem to the rotation when available.", DRK.JobID)]
    DRK_ST_CD_Disesteem = 5017,

    #endregion

    #region Shadowbringer Options

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Shadowbringer Option",
        "Adds Shadowbringer to the rotation while Darkside is up. Will use all stacks on cooldown.", DRK.JobID)]
    DRK_ST_CD_Bringer = 5018,

    [ParentCombo(DRK_ST_CD_Bringer)]
    [CustomComboInfo("Shadowbringer Burst Option", "Pools Shadowbringer to use during even minute window bursts (after Disesteem).",
        DRK.JobID)]
    DRK_ST_CD_BringerBurst = 5019,

    #endregion

    #region Salt Options

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Salted Earth Option",
        "Adds Salted Earth to the rotation while Darkside is up, will use Salt and Darkness if unlocked.", DRK.JobID)]
    DRK_ST_CD_Salt = 5020,

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Salt and Darkness Option", "Adds Salt and Darkness to the rotation in the latter half of its duration.", DRK.JobID)]
    DRK_ST_CD_Darkness = 5021,

    #endregion

    [ParentCombo(DRK_ST_CDs)]
    [CustomComboInfo("Carve and Spit Option", "Adds Carve and Spit to the rotation while Darkside is up.", DRK.JobID)]
    DRK_ST_CD_Spit = 5022,

    #endregion

    #region Spenders

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo("Spender Options", "Collection of spenders (mana and blood) to add to the rotation.", DRK.JobID)]
    DRK_ST_Spenders = 5023,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Scarlet Delirium Combo Option", "Adds the Scarlet Delirium combo chain to the rotation when Delirium is activated.", DRK.JobID)]
    DRK_ST_Sp_ScarletChain = 5024,

    #region Blood

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Bloodspiller Option", "Adds Bloodspiller to the rotation when under Delirium, or just after Burst.", DRK.JobID)]
    DRK_ST_Sp_Bloodspiller = 5025,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Blood Gauge Overcap Option",
        "Adds Bloodspiller to the rotation before Souleater when the blood gauge is above the chosen threshold.", DRK.JobID)]
    DRK_ST_Sp_BloodOvercap = 5026,

    #endregion

    #region Mana

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Edge of Darkness Option", "Uses Edge of Darkness in burst windows.", DRK.JobID)]
    DRK_ST_Sp_Edge = 5027,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Dark Arts Drop Prevention", "Will spend Dark Arts if your own The Blackest Night shield is active on you, except when saving for burst.", DRK.JobID)]
    DRK_ST_Sp_DarkArts = 5028,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Darkside Maintenance Option", "Uses Edge of Darkness if Darkside is about to expire (<10s).", DRK.JobID)]
    DRK_ST_Sp_EdgeDarkside = 5029,

    [ParentCombo(DRK_ST_Spenders)]
    [CustomComboInfo("Mana Overcap Option", "Uses Edge of Darkness if you are at or above 9,400 mana, except when saving for burst.", DRK.JobID)]
    DRK_ST_Sp_ManaOvercap = 5030,

    #endregion

    #endregion

    #region Mitigation Options

    [ParentCombo(DRK_ST_Adv)]
    [CustomComboInfo("Mitigation Options", "Collection of Mitigations to add to the rotation.", DRK.JobID)]
    DRK_ST_Mitigation = 5031,

    [ParentCombo(DRK_ST_Mitigation)]
    [CustomComboInfo("The Blackest Night Option",
        "Uses The Blackest Night based on Health Remaining.\n" +
        "(Note: makes no attempt to ensure shield will break)", DRK.JobID)]
    DRK_ST_Mit_TBN = 5032,

    [ParentCombo(DRK_ST_Mitigation)]
    [CustomComboInfo("Oblation Option", "Uses Oblation based on Health Remaining.", DRK.JobID)]
    DRK_ST_Mit_Oblation = 5033,

    [ParentCombo(DRK_ST_Mitigation)]
    [CustomComboInfo("Reprisal Option", "Uses Reprisal when a raidwide is in the process of casting.", DRK.JobID)]
    DRK_ST_Mit_Reprisal = 5034,

    [ParentCombo(DRK_ST_Mitigation)]
    [CustomComboInfo("Dark Missionary Option", "Uses Dark Missionary when a raidwide is in the process of casting.", DRK.JobID)]
    DRK_ST_Mit_Missionary = 5035,

    [ParentCombo(DRK_ST_Mit_Missionary)]
    [CustomComboInfo("Avoid Doubling up on Group Mit", "Won't use Dark Missionary if your own Reprisal is on the target.", DRK.JobID)]
    DRK_ST_Mit_MissionaryAvoid = 5039,

    [ParentCombo(DRK_ST_Mitigation)]
    [CustomComboInfo("Shadowed Vigil Option", "Uses Shadowed Vigil based on Health Remaining.", DRK.JobID)]
    DRK_ST_Mit_Vigil = 5036,

    [ParentCombo(DRK_ST_Mitigation)]
    [CustomComboInfo("Living Dead Option", "Uses Living Dead based on Health Remaining.", DRK.JobID)]
    DRK_ST_Mit_LivingDead = 5037,

    #endregion

    #endregion
    // Last value = 5040

    #region Advanced Multi Target Combo

    [AutoAction(true, false)]
    [ReplaceSkill(DRK.Unleash)]
    [ConflictingCombos(DRK_AoE_Simple)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Unleash with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        DRK.JobID)]
    [AdvancedCombo]
    DRK_AoE_Adv = 5050,

    #region Cooldowns

    [ParentCombo(DRK_AoE_Adv)]
    [CustomComboInfo("Cooldowns Options", "Collection of cooldowns to add to the rotation.", DRK.JobID)]
    DRK_AoE_CDs = 5051,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", DRK.JobID)]
    DRK_AoE_Interrupt = 5052,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Interrupt with Stun Option", "Adds Low Blow to the rotation when your target is casting.", DRK.JobID)]
    DRK_AoE_Stun = 5053,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Delirium Option",
        "Adds Delirium (or Blood Weapon at lower levels) to the rotation on cooldown and when Darkside is up.",
        DRK.JobID)]
    DRK_AoE_CD_Delirium = 5054,

    #region Living Shadow Options

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Living Shadow Option", "Adds Living Shadow to the rotation on cooldown and when Darkside is up.",
        DRK.JobID)]
    DRK_AoE_CD_Shadow = 5055,

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Disesteem Option", "Adds Disesteem to the rotation when available.", DRK.JobID)]
    DRK_AoE_CD_Disesteem = 5056,

    #endregion

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("AoE Shadowbringer Option", "Adds Shadowbringer to the rotation.", DRK.JobID)]
    DRK_AoE_CD_Bringer = 5057,

    #region Salt Options

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Salted Earth Option",
        "Adds Salted Earth and Salt and Darkness to the rotation on cooldown and when Darkside is up.", DRK.JobID)]
    DRK_AoE_CD_Salt = 5058,

    [ParentCombo(DRK_AoE_CD_Salt)]
    [CustomComboInfo("Salt After Pull Option",
        "Requires to be at a stand-still and for combat to have been going on for >7 seconds to use Salted Earth, to try to make it be placed after you finish your pull.", DRK.JobID)]
    DRK_AoE_CD_SaltStill = 5059,

    #endregion

    [ParentCombo(DRK_AoE_CDs)]
    [CustomComboInfo("Abyssal Drain Option", "Adds Abyssal Drain to the rotation when you fall below the chosen HP.",
        DRK.JobID)]
    DRK_AoE_CD_Drain = 5060,

    #endregion

    #region Spenders

    [ParentCombo(DRK_AoE_Adv)]
    [CustomComboInfo("Spender Options", "Collection of spenders (mana and blood) to add to the rotation.", DRK.JobID)]
    DRK_AoE_Spenders = 5061,

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo("Impalement Option", "Adds Impalement to the rotation when Delirium is active.", DRK.JobID)]
    DRK_AoE_Sp_ImpalementChain = 5062,

    #region Blood

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo("Quietus Option", "Adds Quietus to the rotation when under Delirium, or just after Burst..", DRK.JobID)]
    DRK_AoE_Sp_Quietus = 5063,

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo("Blood Gauge Overcap Option", "Adds Quietus to the rotation when the blood gauge is above the chosen threshold.", DRK.JobID)]
    DRK_AoE_Sp_BloodOvercap = 5064,

    #endregion

    #region Mana

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo("Flood of Shadow Option", "Uses Flood of Shadow in burst, if Darkside is about to expire (<10s), if you have Dark Arts and use The Blackest Night, and outside of burst will spend to chosen MP limit.", DRK.JobID)]
    DRK_AoE_Sp_Flood = 5065,

    [ParentCombo(DRK_AoE_Spenders)]
    [CustomComboInfo("Mana Overcap Option", "Uses Flood of Shadow if you are above 9,400 mana.", DRK.JobID)]
    DRK_AoE_Sp_ManaOvercap = 5066,

    #endregion

    #endregion

    #region Mitigation Options

    [ParentCombo(DRK_AoE_Adv)]
    [CustomComboInfo("Mitigation Options", "Collection of Mitigations to add to the rotation.", DRK.JobID)]
    DRK_AoE_Mitigation = 5067,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("The Blackest Night Option", "Adds The Blackest Night to the rotation.", DRK.JobID)]
    DRK_AoE_Mit_TBN = 5068,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Oblation Option", "Uses Oblation based on Health Remaining.", DRK.JobID)]
    DRK_AoE_Mit_Oblation = 5069,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal to the rotation.", DRK.JobID)]
    DRK_AoE_Mit_Reprisal = 5070,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Dark Mind Option", "Adds Dark Mind to the rotation.", DRK.JobID)]
    DRK_AoE_Mit_DarkMind = 5075,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart to the rotation.", DRK.JobID)]
    DRK_AoE_Mit_Rampart = 5071,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length to the rotation, when the set number of enemies are within range of you.", DRK.JobID)]
    DRK_AoE_Mit_ArmsLength = 5072,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Shadowed Vigil Option", "Uses Shadowed Vigil based on Health Remaining.", DRK.JobID)]
    DRK_AoE_Mit_Vigil = 5073,

    [ParentCombo(DRK_AoE_Mitigation)]
    [CustomComboInfo("Living Dead Option", "Uses Living Dead based on your and your enemy's Remaining Health.", DRK.JobID)]
    DRK_AoE_Mit_LivingDead = 5074,

    #endregion

    #endregion
    // Last value = 5075

    #region Basic combo

    [ReplaceSkill(DRK.Souleater)]
    [CustomComboInfo("Basic Combo", "Replace Souleater with its combo chain.", DRK.JobID)]
    [BasicCombo]
    DRK_ST_BasicCombo = 5003,

    #endregion

    #region Multibutton Features

    #region One-Button Mitigation

    [ReplaceSkill(DRK.DarkMind)]
    [CustomComboInfo("One-Button Mitigation Feature", "Replaces Dark Mind with an all-in-one mitigation button.", DRK.JobID)]
    [MitigationCombo]
    DRK_Mit_OneButton = 5090,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Living Dead Emergency Option", "Gives max priority to Living Dead when the Health percentage threshold is met.", DRK.JobID)]
    DRK_Mit_LivingDead_Max = 5091,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("The Blackest Night Option", "Adds The Blackest Night to the one-button mitigation.", DRK.JobID)]
    DRK_Mit_TheBlackestNight = 5092,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Oblation Option", "Adds Oblation to the one-button mitigation.", DRK.JobID)]
    DRK_Mit_Oblation = 5093,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Reprisal Option",
        "Adds Reprisal to the one-button mitigation." +
        "\nNOTE: Will not use unless there is a target within range to prevent waste.", DRK.JobID)]
    DRK_Mit_Reprisal = 5094,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Dark Missionary Option", "Adds Dark Missionary to the one-button mitigation.", DRK.JobID)]
    DRK_Mit_DarkMissionary = 5095,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Rampart Option", "Adds Rampart to the one-button mitigation.", DRK.JobID)]
    DRK_Mit_Rampart = 5096,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Dark Mind Option",
        "Adds Dark Mind to the one-button mitigation." +
        "\nNOTE: even if disabled, will still try to use Dark Mind as the lowest priority.", DRK.JobID)]
    DRK_Mit_DarkMind = 5097,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length to the one-button mitigation.", DRK.JobID)]
    DRK_Mit_ArmsLength = 5098,

    [ParentCombo(DRK_Mit_OneButton)]
    [CustomComboInfo("Shadow Wall / Vigil Option", "Adds Shadow Wall / Vigil to the one-button mitigation.", DRK.JobID)]
    DRK_Mit_ShadowWall = 5099,

    [ReplaceSkill(DRK.DarkMissionary)]
    [CustomComboInfo("One-Button Party Mitigation Feature", "Replaces Dark Missionary with Reprisal when ready.", DRK.JobID)]
    [MitigationCombo]
    DRK_Mit_Party = 5100,

    #endregion
    // Last value = 5100

    #region oGCD Feature

    [ReplaceSkill(DRK.CarveAndSpit, DRK.AbyssalDrain)]
    [CustomComboInfo("oGCD Feature", "Adds selected oGCD abilities to Carve And Spit and Abyssal Drain.", DRK.JobID)]
    DRK_oGCD = 5120,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the feature when your target's cast is interruptible.", DRK.JobID)]
    DRK_oGCD_Interrupt = 5121,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Delirium Option", "Adds Delirium (or Blood Weapon) to the Feature.", DRK.JobID)]
    DRK_oGCD_Delirium = 5122,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Living Shadow Option", "Adds Living Shadow to the Feature.", DRK.JobID)]
    DRK_oGCD_Shadow = 5124,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Disesteem Option", "Adds Disesteem to the Feature.", DRK.JobID)]
    DRK_oGCD_Disesteem = 5125,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Salted Earth Option", "Adds Salted Earth to the Feature.", DRK.JobID)]
    DRK_oGCD_SaltedEarth = 5126,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Salt and Darkness Option", "Adds Saltand Darkness to the Feature when under the effect of Salted Earth.", DRK.JobID)]
    DRK_oGCD_SaltAndDarkness = 5127,

    [ParentCombo(DRK_oGCD)]
    [CustomComboInfo("Shadowbringer Option", "Adds Shadowbringer to the Feature.", DRK.JobID)]
    DRK_oGCD_Shadowbringer = 5123,

    #endregion
    // Last value = 5123

    #region Standalones

    [ReplaceSkill(DRK.BlackestNight)]
    [CustomComboInfo("Retarget The Blackest Night Feature", "Will Retarget The Blackest Night outside of other combos to your MouseOver Target (or a friendly Hard Target).", DRK.JobID)]
    [Retargeted(DRK.BlackestNight)]
    DRK_Retarget_TBN = 5130,

    [ParentCombo(DRK_Retarget_TBN)]
    [CustomComboInfo("Include Target's Target", "If your target's target is not you, will Retarget The Blackest Night onto them.\n(if you're not top aggro, and not mousing over or hard targeting an ally)", DRK.JobID)]
    [Retargeted]
    DRK_Retarget_TBN_TT = 5131,

    [ReplaceSkill(DRK.Oblation)]
    [CustomComboInfo("Retarget Oblation Feature", "Will Retarget Oblation outside of other combos to your MouseOver Target (or a friendly Hard Target).", DRK.JobID)]
    [Retargeted(DRK.Oblation)]
    DRK_Retarget_Oblation = 5132,

    [ParentCombo(DRK_Retarget_Oblation)]
    [CustomComboInfo("Include Target's Target", "If your target's target is not you, will Retarget Oblation onto them.\n(if you're not top aggro, and not mousing over or hard targeting an ally)", DRK.JobID)]
    [Retargeted]
    DRK_Retarget_Oblation_TT = 5133,

    [ParentCombo(DRK_Retarget_Oblation)]
    [CustomComboInfo("Prevent Double Oblations", "Will change Oblation to Savage Blade when your target already has Oblation on them.", DRK.JobID)]
    DRK_Retarget_Oblation_DoubleProtection = 5134,

    #endregion
    // Last value = 5134

    #endregion
    // Last value = 5134

    #region Variant

    [Variant]
    [VariantParent(DRK_ST_Adv, DRK_AoE_Adv, DRK_ST_Simple, DRK_AoE_Simple)]
    [CustomComboInfo("Spirit Dart Option",
        "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", DRK.JobID)]
    DRK_Var_Dart = 5140,

    [Variant]
    [VariantParent(DRK_ST_Adv, DRK_AoE_Adv, DRK_ST_Simple, DRK_AoE_Simple)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", DRK.JobID)]
    DRK_Var_Cure = 5141,

    [Variant]
    [VariantParent(DRK_ST_Adv, DRK_AoE_Adv, DRK_ST_Simple, DRK_AoE_Simple)]
    [CustomComboInfo("Ultimatum Option", "Use Variant Ultimatum on cooldown.", DRK.JobID)]
    DRK_Var_Ulti = 5142,

    #endregion
    // Last value = 5142

    #region Hidden Features

    [CustomComboInfo("Hidden Options", "Collection of cheeky or encounter-specific extra options only available to those in the know.\nDo not expect these options to be maintained, or even kept, after they are no longer Current.", DRK.JobID)]
    [Hidden]
    DRK_Hidden = 5200,

    [ParentCombo(DRK_Hidden)]
    [CustomComboInfo("R6S: Hold Burst on Squirrels", "When you're targeting Squirrels in R6S add phase, hold burst.\n(until about the time the first manta is dying)", DRK.JobID)]
    [Hidden]
    DRK_Hid_R6SHoldSquirrelBurst = 5201,

    [ParentCombo(DRK_Hidden)]
    [CustomComboInfo("R6S: Only Stun Jabberwock", "When in R6S, stun will only ever be used on the Jabberwock.", DRK.JobID)]
    [Hidden]
    DRK_Hid_R6SStunJabberOnly = 5202,

    [ParentCombo(DRK_Hidden)]
    [CustomComboInfo("R6S: Save Reprisal and Dark Missionary", "When in R6S, never try use Reprisal or Dark Missionary automatically.", DRK.JobID)]
    [Hidden]
    DRK_Hid_R6SNoAutoGroupMits = 5203,

    [ParentCombo(DRK_Hidden)]
    [CustomComboInfo("R7S: Only Interrupt the adds casting Circle AoEs", "When you're in R7S, Interrupting will only work when you're targeting an add casting the circle AoE.", DRK.JobID)]
    [Hidden]
    DRK_Hid_R7SCircleCastOnly = 5204,

    #endregion
    // Last value = 5204

    #endregion

    #region DRAGOON
    
    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(DRG.TrueThrust)]
    [ConflictingCombos(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces True Thrust with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", DRG.JobID)]
    [SimpleCombo]
    DRG_ST_SimpleMode = 6001,

    [AutoAction(true, false)]
    [ReplaceSkill(DRG.DoomSpike)]
    [ConflictingCombos(DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Doom Spike with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", DRG.JobID)]
    [SimpleCombo]
    DRG_AOE_SimpleMode = 6200,

    #endregion

    #region Advanced ST Dragoon

    [AutoAction(false, false)]
    [ReplaceSkill(DRG.TrueThrust)]
    [ConflictingCombos(DRG_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces True Thrust with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", DRG.JobID)]
    [AdvancedCombo]
    DRG_ST_AdvancedMode = 6100,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", DRG.JobID)]
    DRG_ST_Opener = 6101,

    #region Buffs ST

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Buffs Option", "Adds various buffs to the rotation.", DRG.JobID)]
    DRG_ST_Buffs = 6102,

    [ParentCombo(DRG_ST_Buffs)]
    [CustomComboInfo("Battle Litany Option", "Adds Battle Litany to the rotation.", DRG.JobID)]
    DRG_ST_Litany = 6103,

    [ParentCombo(DRG_ST_Buffs)]
    [CustomComboInfo("Lance Charge Option", "Adds Lance Charge to the rotation.", DRG.JobID)]
    DRG_ST_Lance = 6104,

    #endregion

    #region Cooldowns ST

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Cooldowns Option", "Adds various cooldowns to the rotation.", DRG.JobID)]
    DRG_ST_CDs = 6105,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Life Surge Option", "Adds Life Surge, on the proper GCD, to the rotation.", DRG.JobID)]
    DRG_ST_LifeSurge = 6106,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("High Jump Option", "Adds (High) Jump to the rotation.", DRG.JobID)]
    DRG_ST_HighJump = 6113,
    
    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Mirage Dive Option", "Adds Mirage Dive to the rotation.", DRG.JobID)]
    DRG_ST_Mirage = 6115,
    
    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Dragonfire Dive Option", "Adds Dragonfire Dive to the rotation.", DRG.JobID)]
    DRG_ST_DragonfireDive = 6107,
    
    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Geirskogul Option", "Adds Geirskogul to the rotation.", DRG.JobID)]
    DRG_ST_Geirskogul = 6116,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Nastrond Option", "Adds Nastrond to the rotation.", DRG.JobID)]
    DRG_ST_Nastrond = 6117,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Stardiver Option", "Adds Stardiver to the rotation.", DRG.JobID)]
    DRG_ST_Stardiver = 6110,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Wyrmwind Thrust Option", "Adds Wyrmwind Thrust to the rotation.", DRG.JobID)]
    DRG_ST_Wyrmwind = 6118,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Rise of the Dragon Option", "Adds Rise of the Dragon to the rotation.", DRG.JobID)]
    DRG_ST_Dives_RiseOfTheDragon = 6109,

    [ParentCombo(DRG_ST_CDs)]
    [CustomComboInfo("Starcross Option", "Adds Starcross to the rotation.", DRG.JobID)]
    DRG_ST_Starcross = 6112,

    #endregion

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Dynamic True North Option", "Adds True North before Chaos Thrust/Chaotic Spring, Fang And Claw and Wheeling Thrust when you are not in the correct position for the enhanced potency bonus.", DRG.JobID)]
    DRG_TrueNorthDynamic = 6199,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Ranged Uptime Option", "Adds Piercing Talon to the rotation when you are out of melee range.", DRG.JobID)]
    DRG_ST_RangedUptime = 6197,

    [ParentCombo(DRG_ST_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the rotation.", DRG.JobID)]
    DRG_ST_ComboHeals = 6198,

    #endregion

    #region Advanced AoE Dragoon

    [AutoAction(true, false)]
    [ReplaceSkill(DRG.DoomSpike)]
    [ConflictingCombos(DRG_AOE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Doomspike with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", DRG.JobID)]
    [AdvancedCombo]
    DRG_AOE_AdvancedMode = 6201,

    #region Buffs AoE

    [ParentCombo(DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Buffs AoE Option", "Adds Lance Charge and Battle Litany to the rotation.", DRG.JobID)]
    DRG_AoE_Buffs = 6202,

    [ParentCombo(DRG_AoE_Buffs)]
    [CustomComboInfo("Battle Litany AoE Option", "Adds Battle Litany to the rotation.", DRG.JobID)]
    DRG_AoE_Litany = 6203,

    [ParentCombo(DRG_AoE_Buffs)]
    [CustomComboInfo("Lance Charge AoE Option", "Adds Lance Charge to the rotation.", DRG.JobID)]
    DRG_AoE_Lance = 6204,

    #endregion

    #region cooldowns AoE

    [ParentCombo(DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Cooldowns Option", "Adds various cooldowns to the rotation.", DRG.JobID)]
    DRG_AoE_CDs = 6205,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Life Surge Option", "Adds Life Surge, onto proper GCDs, to the rotation.", DRG.JobID)]
    DRG_AoE_LifeSurge = 6206,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("High Jump Option", "Adds (High) Jump to the rotation.", DRG.JobID)]
    DRG_AoE_HighJump = 6213,
    
    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Mirage Dive Option", "Adds Mirage Dive to the rotation.", DRG.JobID)]
    DRG_AoE_Mirage = 6215,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Dragonfire Dive Option", "Adds Dragonfire Dive to the rotation.", DRG.JobID)]
    DRG_AoE_DragonfireDive = 6207,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Geirskogul Option", "Adds Geirskogul to the rotation.", DRG.JobID)]
    DRG_AoE_Geirskogul = 6216,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Nastrond Option", "Adds Nastrond to the rotation.", DRG.JobID)]
    DRG_AoE_Nastrond = 6217,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Stardiver Option", "Adds Stardiver to the rotation.", DRG.JobID)]
    DRG_AoE_Stardiver = 6210,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Wyrmwind Option", "Adds Wyrmwind Thrust to the rotation.", DRG.JobID)]
    DRG_AoE_Wyrmwind = 6218,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Rise of the Dragon Option", "Adds Rise of the Dragon to the rotation.", DRG.JobID)]
    DRG_AoE_RiseOfTheDragon = 6209,

    [ParentCombo(DRG_AoE_CDs)]
    [CustomComboInfo("Starcross Option", "Adds Starcross to the rotation.", DRG.JobID)]
    DRG_AoE_Starcross = 6212,

    #endregion

    [ParentCombo(DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Low Level Disembowel", "Adds Disembowel combo to the rotation when you are or synced below level 62.", DRG.JobID)]
    DRG_AoE_Disembowel = 6297,

    [ParentCombo(DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Ranged Uptime Option", "Adds Piercing Talon to the rotation when you are out of melee range.",
        DRG.JobID)]
    DRG_AoE_RangedUptime = 6298,

    [ParentCombo(DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the rotation.", DRG.JobID)]
    DRG_AoE_ComboHeals = 6299,

    #endregion
    
    #region Basic Combo

    [ReplaceSkill(DRG.FullThrust, DRG.HeavensThrust)]
    [CustomComboInfo("Basic Combo", "Replace Full Thrust/Heavens' Thrust with the basic combo chain.", DRG.JobID)]
    [BasicCombo]
    DRG_BasicCombo = 6304,
    
    #endregion

    #region Variant

    [Variant]
    [VariantParent(DRG_ST_AdvancedMode, DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", DRG.JobID)]
    DRG_Variant_Cure = 6302,

    [Variant]
    [VariantParent(DRG_ST_AdvancedMode, DRG_AOE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", DRG.JobID)]
    DRG_Variant_Rampart = 6303,

    #endregion
    
    [ReplaceSkill(DRG.LanceCharge)]
    [CustomComboInfo("Lance Charge to Battle Litany Feature", "Turns Lance Charge into Battle Litany when the former is on cooldown.", DRG.JobID)]
    DRG_BurstCDFeature = 6301,

    // Last value ST = 6119
    // Last value AoE = 6216

    #endregion

    #region GUNBREAKER
    
    #region Simple Mode
    [AutoAction(false, false)]
    [ConflictingCombos(GNB_ST_Advanced)]
    [ReplaceSkill(GNB.KeenEdge)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Keen Edge with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", GNB.JobID)]
    [SimpleCombo]
    GNB_ST_Simple = 7001,

    [AutoAction(true, false)]
    [ConflictingCombos(GNB_AoE_Advanced)]
    [ReplaceSkill(GNB.DemonSlice)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Demon Slice with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", GNB.JobID)]
    [SimpleCombo]
    GNB_AoE_Simple = 7002,
    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ConflictingCombos(GNB_ST_Simple)]
    [ReplaceSkill(GNB.KeenEdge)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Keen Edge with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", GNB.JobID)]
    [AdvancedCombo]
    GNB_ST_Advanced = 7003,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Balance Openers", "Add openers into the rotation based on Skill Speed and current Level, starting at Lv90.", GNB.JobID)]
    GNB_ST_Opener = 7006,

    [ConflictingCombos(GNB_NM_Features)]
    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("No Mercy Option", "Adds No Mercy into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_NoMercy = 7008,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Zone Option", "Adds Danger / Blasting Zone into the rotation when available.", GNB.JobID)]
    GNB_ST_Zone = 7009,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Burst Strike Option", "Adds Burst Strike into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_BurstStrike = 7015,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Sonic Break Option", "Adds Sonic Break into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_SonicBreak = 7012,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Gnashing Fang Option", "Adds Gnashing Fang combo into the rotation.", GNB.JobID)]
    GNB_ST_GnashingFang = 7016,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Bow Shock Option", "Adds Bow Shock into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_BowShock = 7010,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Continuation Option", "Adds Continuation & Hypervelocity into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_Continuation = 7005,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Bloodfest Option", "Adds Bloodfest into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_Bloodfest = 7011,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Double Down Option", "Adds Double Down into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_DoubleDown = 7017,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Reign Combo Option", "Adds Reign/Noble/Lionheart into the rotation when appropriate.", GNB.JobID)]
    GNB_ST_Reign = 7014,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Scuffed Option", "Adds Solid Barrel into the rotation under scuffed conditions:\n- Level 90 or above\n- No Mercy is active\n- Only 1 cartridge available\n- Last combo action was Brutal Shell\n- Gnashing Fang combo is still not active", GNB.JobID)]
    GNB_ST_Scuffed = 7372,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Lightning Shot Uptime Option", "Adds Lightning Shot to the main combo when you are out of range.", GNB.JobID)]
    GNB_ST_RangedUptime = 7004,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", GNB.JobID)]
    GNB_ST_Interrupt = 7084,

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Interrupt with Stun Option", "Adds Low Blow to the rotation when your target is casting.\nNot advised outside of overworld content, as it can waste a lot of Low Blows on un-stun-able enemies, etc. Will try to not use it in boss fights.", GNB.JobID)]
    GNB_ST_Stun = 7086,

    #region Mitigations

    [ParentCombo(GNB_ST_Advanced)]
    [CustomComboInfo("Mitigation Options", "Collection of Mitigation features.", GNB.JobID)]
    GNB_ST_Mitigation = 7019,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Heart of Corundum Option", "Adds Heart of Stone / Corundum into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Corundum = 7020,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Aurora Option", "Adds Aurora into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Aurora = 7024,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Rampart = 7025,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Camouflage Option", "Adds Camouflage into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Camouflage = 7026,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Nebula Option", "Adds Nebula into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Nebula = 7021,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Superbolide Option", "Adds Superbolide into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Superbolide = 7022,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_Reprisal = 7027,

    [ParentCombo(GNB_ST_Mitigation)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_ST_ArmsLength = 7028,

    #endregion

    #endregion

    #region Advanced AoE
    [AutoAction(true, false)]
    [ConflictingCombos(GNB_AoE_Simple)]
    [ReplaceSkill(GNB.DemonSlice)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Demon Slice with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", GNB.JobID)]
    [AdvancedCombo]
    GNB_AoE_Advanced = 7200,

    [ConflictingCombos(GNB_NM_Features)]
    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("No Mercy Option", "Adds No Mercy into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_NoMercy = 7201,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Danger/Blasting Zone Option", "Adds Danger/Blasting Zone into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_Zone = 7202,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Bow Shock Option", "Adds Bow Shock oninto the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_BowShock = 7203,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Bloodfest Option", "Adds Bloodfest into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_Bloodfest = 7204,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Sonic Break Option", "Adds Sonic Break into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_SonicBreak = 7205,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Double Down Option", "Adds Double Down into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_DoubleDown = 7206,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Reign Combo Option", "Adds Reign/Noble/LionHeart into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_Reign = 7207,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Fated Circle Option", "Adds Fated Circle into the AoE rotation when appropriate.", GNB.JobID)]
    GNB_AoE_FatedCircle = 7208,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", GNB.JobID)]
    GNB_AoE_Interrupt = 7222,

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Interrupt with Stun Option", "Adds Low Blow to the rotation when your target is casting, interruptible or not.", GNB.JobID)]
    GNB_AoE_Stun = 7223,

    #region Mitigations

    [ParentCombo(GNB_AoE_Advanced)]
    [CustomComboInfo("Mitigation Options", "Collection of Mitigation features.", GNB.JobID)]
    GNB_AoE_Mitigation = 7216,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Heart of Corundum Option", "Adds Heart of Stone / Corundum into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Corundum = 7213,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Aurora Option", "Adds Aurora into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Aurora = 7217,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Rampart = 7218,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Camouflage Option", "Adds Camouflage into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Camouflage = 7219,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Nebula Option", "Adds Nebula into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Nebula = 7214,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Superbolide Option", "Adds Superbolide into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Superbolide = 7215,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_Reprisal = 7220,

    [ParentCombo(GNB_AoE_Mitigation)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length into the rotation based on Health percentage remaining.", GNB.JobID)]
    GNB_AoE_ArmsLength = 7221,

    #endregion

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(GNB.Camouflage)]
    [CustomComboInfo("One-Button Mitigation Feature", "Replaces Camouflage with an all-in-one mitigation button.", GNB.JobID)]
    [MitigationCombo]
    GNB_Mit_OneButton = 7074,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Superbolide Emergency Option", "Gives max priority to Superbolide when the Health percentage threshold is met.", GNB.JobID)]
    GNB_Mit_Superbolide_Max = 7075,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Heart of Corundum Option", "Adds Heart of Stone / Corundum to the one-button mitigation.", GNB.JobID)]
    GNB_Mit_Corundum = 7076,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Aurora Option", "Adds Aurora to the one-button mitigation.", GNB.JobID)]
    GNB_Mit_Aurora = 7077,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Camouflage First Option",
        "Adds Camouflage to the one-button mitigation." +
        "\nNOTE: even if disabled, will still try to use Camouflage as the lowest priority.", GNB.JobID)]
    GNB_Mit_Camouflage = 7078,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal to the one-button mitigation.\nNOTE: Will not use unless there is a target within range to prevent waste", GNB.JobID)]
    GNB_Mit_Reprisal = 7079,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Heart Of Light Option", "Adds Heart Of Light to the one-button mitigation.", GNB.JobID)]
    GNB_Mit_HeartOfLight = 7080,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Rampart Option", "Adds Rampart to the one-button mitigation.", GNB.JobID)]
    GNB_Mit_Rampart = 7081,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Arms Length Option", "Adds Arms Length to the one-button mitigation.", GNB.JobID)]
    GNB_Mit_ArmsLength = 7082,

    [ParentCombo(GNB_Mit_OneButton)]
    [CustomComboInfo("Nebula Option", "Adds Nebula to the one-button mitigation.", GNB.JobID)]
    GNB_Mit_Nebula = 7083,

    [ReplaceSkill(GNB.HeartOfLight)]
    [CustomComboInfo("One-Button Party Mitigation Feature", "Replaces Heart of Light with Reprisal when ready.", GNB.JobID)]
    [MitigationCombo]
    GNB_Mit_Party = 7085,
    #endregion

    #region Misc

    #region Basic combo

    [ReplaceSkill(GNB.SolidBarrel)]
    [CustomComboInfo("Basic Combo", "Replace Solid Barrel with its combo chain.", GNB.JobID)]
    [BasicCombo]
    GNB_ST_BasicCombo = 7100,

    #endregion

    #region Gnashing Fang
    [ReplaceSkill(GNB.GnashingFang)]
    [CustomComboInfo("Gnashing Fang Features", "Collection of Gnashing Fang related features.\n Enable all for this to be an all-in-one Single Target Burst button.", GNB.JobID)]
    GNB_GF_Features = 7300,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("No Mercy Option", "Adds No Mercy to Gnashing Fang when available.", GNB.JobID)]
    GNB_GF_NoMercy = 7302,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Zone Option", "Adds Danger / Blasting Zone to Gnashing Fang when available.", GNB.JobID)]
    GNB_GF_Zone = 7303,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Burst Strike Option", "Adds Burst Strike on Gnashing Fang under No Mercy when appropriate.", GNB.JobID)]
    GNB_GF_BurstStrike = 7309,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Sonic Break Option", "Adds Sonic Break on Gnashing Fang under No Mercy when appropriate.", GNB.JobID)]
    GNB_GF_SonicBreak = 7306,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Bow Shock Option", "Adds Bow Shock to Gnashing Fang when available.", GNB.JobID)]
    GNB_GF_BowShock = 7304,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Continuation Option", "Adds Continuation to Gnashing Fang when available.", GNB.JobID)]
    GNB_GF_Continuation = 7301,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Bloodfest Option", "Adds Bloodfest to Gnashing Fang when available.", GNB.JobID)]
    GNB_GF_Bloodfest = 7305,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Double Down Option", "Adds Double Down to Gnashing Fang under No Mercy when appropriate.", GNB.JobID)]
    GNB_GF_DoubleDown = 7307,

    [ParentCombo(GNB_GF_Features)]
    [CustomComboInfo("Reign Combo Option", "Adds Reign combo on Gnashing Fang under No Mercy when appropriate.", GNB.JobID)]
    GNB_GF_Reign = 7308,
    #endregion

    #region No Mercy
    [ReplaceSkill(GNB.NoMercy)]
    [CustomComboInfo("No Mercy Features", "Collection of No Mercy related features.", GNB.JobID)]
    GNB_NM_Features = 7500,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo("Bloodfest Option", "Adds Bloodfest to No Mercy when appropriate.", GNB.JobID)]
    GNB_NM_Bloodfest = 7501,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo("Zone Option", "Adds Danger / Blasting Zone to No Mercy.", GNB.JobID)]
    GNB_NM_Zone = 7502,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo("Bow Shock Option", "Adds Bow Shock to No Mercy appropriately after No Mercy is used.", GNB.JobID)]
    GNB_NM_BowShock = 7503,

    [ParentCombo(GNB_NM_Features)]
    [CustomComboInfo("Continuation Option", "Adds all Continuation procs to No Mercy appropriately.", GNB.JobID)]
    GNB_NM_Continuation = 7504,
    #endregion

    #region Burst Strike

    [ReplaceSkill(GNB.BurstStrike)]
    [CustomComboInfo("Burst Strike Features", "Collection of Burst Strike related features.", GNB.JobID)]
    GNB_BS_Features = 7400,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo("Continuation Option", "Adds all Single-Target Continuation procs to Burst Strike when available.",
        GNB.JobID)]
    GNB_BS_Continuation = 7401,

    [ParentCombo(GNB_BS_Continuation)]
    [CustomComboInfo("Only Hypervelocity Option", "Adds only Hypervelocity to Burst Strike when available.", GNB.JobID)]
    GNB_BS_Hypervelocity = 7406,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo("Bloodfest Option", "Adds Bloodfest to Burst Strike when approrpiate.", GNB.JobID)]
    GNB_BS_Bloodfest = 7402,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo("Gnashing Fang Option", "Adds Gnashing Fang & its combo to Burst Strike when available.",
        GNB.JobID)]
    GNB_BS_GnashingFang = 7405,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo("Double Down Option", "Adds Double Down to Burst Strike when available.", GNB.JobID)]
    GNB_BS_DoubleDown = 7403,

    [ParentCombo(GNB_BS_Features)]
    [CustomComboInfo("Reign Combo Option", "Adds Reign/Noble/Lionheart to Burst Strike when available.", GNB.JobID)]
    GNB_BS_Reign = 7404,

    #endregion

    #region Fated Circle

    [ReplaceSkill(GNB.FatedCircle)]
    [CustomComboInfo("Fated Circle Features", "Collection of Fated Circle related features.", GNB.JobID)]
    GNB_FC_Features = 7600,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo("Fated Brand Option", "Adds Fated Brand to Fated Circle.", GNB.JobID)]
    GNB_FC_Continuation = 7601,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo("Bloodfest Option", "Adds Bloodfest to Fated Circle when appropriate.", GNB.JobID)]
    GNB_FC_Bloodfest = 7602,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo("Bow Shock Option", "Adds Bow Shock to Fated Circle when appropriate.", GNB.JobID)]
    GNB_FC_BowShock = 7605,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo("Double Down Option", "Adds Double Down to Fated Circle when appropriate.", GNB.JobID)]
    GNB_FC_DoubleDown = 7603,

    [ParentCombo(GNB_FC_DoubleDown)]
    [CustomComboInfo("Under No Mercy Option", "Adds Double Down to Fated Circle only when No Mercy is active.",
        GNB.JobID)]
    GNB_FC_DoubleDown_NM = 7606,

    [ParentCombo(GNB_FC_Features)]
    [CustomComboInfo("Reign Option", "Adds Reign/Noble/LionHeart to Fated Circle when appropriate.", GNB.JobID)]
    GNB_FC_Reign = 7604,

    #endregion

    #region Aurora Protection
    [ReplaceSkill(GNB.Aurora)]
    [CustomComboInfo("Aurora Protection Feature", "Locks out Aurora if Aurora's effect is on the target by replacing it with Savage Blade.", GNB.JobID)]
    GNB_AuroraProtection = 7023,
    
    [ParentCombo(GNB_AuroraProtection)]
    [CustomComboInfo("Aurora Mouseover Option", "Retargets Aurora to your mouseover target if they do not have the HoT", GNB.JobID)]
    [Retargeted(GNB.Aurora)]
    GNB_RetargetAurora_MO = 7087,
    
    [ParentCombo(GNB_AuroraProtection)]
    [CustomComboInfo("Aurora Target's Target Option", "Retargets Aurora to the Target's Target if they do not have the HoT and you do not have Aggro", GNB.JobID)]
    [Retargeted(GNB.Aurora)]
    GNB_RetargetAurora_TT = 7088,
    
    #endregion
    
    #region Heart Of Stone Retarget
    [ReplaceSkill(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    [CustomComboInfo("Heart Of Stone Feature", "Will retarget Heart of Stone/Corundum to your mouseover target or hard target outside of other combos", GNB.JobID)]
    [Retargeted(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    GNB_RetargetHeartofStone = 7089,
    
    [ParentCombo(GNB_RetargetHeartofStone)]
    [CustomComboInfo("Heart of Stone Target's Target Option", "Retargets Heart of Stone/Corundum to the Target's Target you do not have Aggro, can still be overridden with mouseover.", GNB.JobID)]
    [Retargeted(GNB.HeartOfCorundum, GNB.HeartOfStone)]
    GNB_RetargetHeartofStone_TT = 7090,
    #endregion

    #region Variant Skills

    [Variant]
    [VariantParent(GNB_ST_Advanced, GNB_AoE_Advanced)]
    [CustomComboInfo("Spirit Dart Option",
        "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", GNB.JobID)]
    GNB_Variant_SpiritDart = 7033,

    [Variant]
    [VariantParent(GNB_ST_Advanced, GNB_AoE_Advanced)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", GNB.JobID)]
    GNB_Variant_Cure = 7034,

    [Variant]
    [VariantParent(GNB_ST_Advanced, GNB_AoE_Advanced)]
    [CustomComboInfo("Ultimatum Option", "Use Variant Ultimatum on cooldown.", GNB.JobID)]
    GNB_Variant_Ultimatum = 7035,

    #endregion

    #region Bozja

    [Bozja]
    [CustomComboInfo("Lost Focus Option", "Use Lost Focus when available.", GNB.JobID)]
    GNB_Bozja_LostFocus = 7070,

    [Bozja]
    [CustomComboInfo("Lost Font Of Power Option", "Use Lost Font Of Power when available.", GNB.JobID)]
    GNB_Bozja_LostFontOfPower = 7036,

    [Bozja]
    [CustomComboInfo("Lost Slash Option", "Use Lost Slash when available.", GNB.JobID)]
    GNB_Bozja_LostSlash = 7037,

    [Bozja]
    [CustomComboInfo("Lost Death Option", "Use Lost Death when available.", GNB.JobID)]
    GNB_Bozja_LostDeath = 7038,

    [Bozja]
    [CustomComboInfo("Banner Of Noble Ends Option", "Use Banner Of Noble Ends when available.", GNB.JobID)]
    GNB_Bozja_BannerOfNobleEnds = 7039,

    [Bozja]
    [ParentCombo(GNB_Bozja_BannerOfNobleEnds)]
    [CustomComboInfo("Only with `Lost Font Of Power` Option",
        "Use Banner Of Noble Ends only when under Lost Font of Power.", GNB.JobID)]
    GNB_Bozja_PowerEnds = 7040,

    [Bozja]
    [CustomComboInfo("Banner Of Honored Sacrifice Option", "Use Banner Of Honored Sacrifice when available.",
        GNB.JobID)]
    GNB_Bozja_BannerOfHonoredSacrifice = 7041,

    [Bozja]
    [ParentCombo(GNB_Bozja_BannerOfHonoredSacrifice)]
    [CustomComboInfo("Only with `Lost Font Of Power` Option",
        "Use Banner Of Honored Sacrifice only when under Lost Font of Power.", GNB.JobID)]
    GNB_Bozja_PowerSacrifice = 7042,

    [Bozja]
    [CustomComboInfo("Banner Of Honed Acuity Option", "Use Banner Of Honed Acuity when available.", GNB.JobID)]
    GNB_Bozja_BannerOfHonedAcuity = 7043,

    [Bozja]
    [CustomComboInfo("Lost Fair Trade Option", "Use Lost Fair Trade when available.", GNB.JobID)]
    GNB_Bozja_LostFairTrade = 7044,

    [Bozja]
    [CustomComboInfo("Lost Assassination Option", "Use Lost Assassination when available.", GNB.JobID)]
    GNB_Bozja_LostAssassination = 7045,

    [Bozja]
    [CustomComboInfo("Lost Manawall Option", "Use Lost Manawall when available.", GNB.JobID)]
    GNB_Bozja_LostManawall = 7046,

    [Bozja]
    [CustomComboInfo("Banner Of Tireless Conviction Option", "Use Banner Of Tireless Conviction when available.",
        GNB.JobID)]
    GNB_Bozja_BannerOfTirelessConviction = 7047,

    [Bozja]
    [CustomComboInfo("Lost Blood Rage Option", "Use Lost Blood Rage when available.", GNB.JobID)]
    GNB_Bozja_LostBloodRage = 7048,

    [Bozja]
    [CustomComboInfo("Banner Of Solemn Clarity Option", "Use Banner Of Solemn Clarity when available.", GNB.JobID)]
    GNB_Bozja_BannerOfSolemnClarity = 7049,

    [Bozja]
    [CustomComboInfo("Lost Cure Option", "Use Lost Cure when available.", GNB.JobID)]
    GNB_Bozja_LostCure = 7050,

    [Bozja]
    [CustomComboInfo("Lost Cure II Option", "Use Lost Cure II when available.", GNB.JobID)]
    GNB_Bozja_LostCure2 = 7051,

    [Bozja]
    [CustomComboInfo("Lost Cure III Option", "Use Lost Cure III when available.", GNB.JobID)]
    GNB_Bozja_LostCure3 = 7052,

    [Bozja]
    [CustomComboInfo("Lost Cure IV Option", "Use Lost Cure IV when available.", GNB.JobID)]
    GNB_Bozja_LostCure4 = 7053,

    [Bozja]
    [CustomComboInfo("Lost Arise Option", "Use Lost Arise when available.", GNB.JobID)]
    GNB_Bozja_LostArise = 7054,

    [Bozja]
    [CustomComboInfo("Lost Sacrifice Option", "Use Lost Sacrifice when available.", GNB.JobID)]
    GNB_Bozja_LostSacrifice = 7055,

    [Bozja]
    [CustomComboInfo("Lost Reraise Option", "Use Lost Reraise when available.", GNB.JobID)]
    GNB_Bozja_LostReraise = 7056,

    [Bozja]
    [CustomComboInfo("Lost Spellforge Option", "Use Lost Spellforge when available.", GNB.JobID)]
    GNB_Bozja_LostSpellforge = 7057,

    [Bozja]
    [CustomComboInfo("Lost Steel Sting Option", "Use Lost Steel Sting when available.", GNB.JobID)]
    GNB_Bozja_LostSteelsting = 7058,

    [Bozja]
    [CustomComboInfo("Lost Protect Option", "Use Lost Protect when available.", GNB.JobID)]
    GNB_Bozja_LostProtect = 7059,

    [Bozja]
    [CustomComboInfo("Lost Shell Option", "Use Lost Shell when available.", GNB.JobID)]
    GNB_Bozja_LostShell = 7060,

    [Bozja]
    [CustomComboInfo("Lost Reflect Option", "Use Lost Reflect when available.", GNB.JobID)]
    GNB_Bozja_LostReflect = 7061,

    [Bozja]
    [CustomComboInfo("Lost Bravery Option", "Use Lost Bravery when available.", GNB.JobID)]
    GNB_Bozja_LostBravery = 7062,

    [Bozja]
    [CustomComboInfo("Lost Aethershield Option", "Use Lost Aether Shield when available.", GNB.JobID)]
    GNB_Bozja_LostAethershield = 7063,

    [Bozja]
    [CustomComboInfo("Lost Protect II Option", "Use Lost Protect II when available.", GNB.JobID)]
    GNB_Bozja_LostProtect2 = 7064,

    [Bozja]
    [CustomComboInfo("Lost Shell II Option", "Use Lost Shell II when available.", GNB.JobID)]
    GNB_Bozja_LostShell2 = 7065,

    [Bozja]
    [CustomComboInfo("Lost Bubble Option", "Use Lost Bubble when available.", GNB.JobID)]
    GNB_Bozja_LostBubble = 7066,

    [Bozja]
    [CustomComboInfo("Lost Stealth Option", "Use Lost Stealth when available.", GNB.JobID)]
    GNB_Bozja_LostStealth = 7067,

    [Bozja]
    [CustomComboInfo("Lost Swift Option", "Use Lost Swift when available.", GNB.JobID)]
    GNB_Bozja_LostSwift = 7068,

    [Bozja]
    [CustomComboInfo("Lost Font Of Skill Option", "Use Lost Font Of Skill when available.", GNB.JobID)]
    GNB_Bozja_LostFontOfSkill = 7069,

    [Bozja]
    [CustomComboInfo("Lost Impetus Option", "Use Lost Impetus when available.", GNB.JobID)]
    GNB_Bozja_LostImpetus = 7071,

    [Bozja]
    [CustomComboInfo("Lost Paralyze III Option", "Use Lost Paralyze III when available.", GNB.JobID)]
    GNB_Bozja_LostParalyze3 = 7072,

    [Bozja]
    [CustomComboInfo("Lost Rampage Option", "Use Lost Rampage when available.", GNB.JobID)]
    GNB_Bozja_LostRampage = 7073,

    #endregion

    #endregion

    // Last Value = 7090
    #endregion

    #region MACHINIST

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(MCH.SplitShot, MCH.HeatedSplitShot)]
    [ConflictingCombos(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Split Shot with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", MCH.JobID)]
    [SimpleCombo]
    MCH_ST_SimpleMode = 8001,

    [AutoAction(true, false)]
    [ReplaceSkill(MCH.SpreadShot, MCH.Scattergun)]
    [ConflictingCombos(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Spreadshot with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.\nWill lock out input to keep Flamethrower going by replacing Spreadshot with Savage Blade.", MCH.JobID)]
    [SimpleCombo]
    MCH_AoE_SimpleMode = 8200,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(MCH.SplitShot, MCH.HeatedSplitShot)]
    [ConflictingCombos(MCH_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Split Shot with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", MCH.JobID)]
    [AdvancedCombo]
    MCH_ST_AdvancedMode = 8100,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener", "Adds the Balance opener at lvl 90+.", MCH.JobID)]
    MCH_ST_Adv_Opener = 8101,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Barrel Stabilizer Option", "Adds Barrel Stabilizer to the rotation.", MCH.JobID)]
    MCH_ST_Adv_Stabilizer = 8110,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Full Metal Field Option", "Adds Full Metal Field to the rotation.", MCH.JobID)]
    MCH_ST_Adv_Stabilizer_FullMetalField = 8111,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Gauss Round / Ricochet \nDouble Check / Checkmate option", "Adds Gauss Round and Ricochet or Double Check and Checkmate to the rotation. Will prevent overcapping.", MCH.JobID)]
    MCH_ST_Adv_GaussRicochet = 8104,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Wildfire Option", "Adds Wildfire to the rotation.", MCH.JobID)]
    MCH_ST_Adv_WildFire = 8108,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Hypercharge Option", "Adds Hypercharge to the rotation.", MCH.JobID)]
    MCH_ST_Adv_Hypercharge = 8105,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Heat Blast / Blazing Shot Option", "Adds Heat Blast or Blazing Shot to the rotation", MCH.JobID)]
    MCH_ST_Adv_Heatblast = 8106,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Rook Autoturret/Automaton Queen Option", "Adds Rook Autoturret or Automaton Queen to the rotation.", MCH.JobID)]
    MCH_ST_Adv_TurretQueen = 8107,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Rook / Queen Overdrive Option", "Adds Rook or Queen Overdrive to the rotation.", MCH.JobID)]
    MCH_ST_Adv_QueenOverdrive = 8115,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Reassemble Option", "Adds Reassemble to the rotation.\nWill be used priority based.\nOrder from highest to lowest priority :\nExcavator - Chainsaw - Air Anchor - Drill - Clean Shot", MCH.JobID)]
    MCH_ST_Adv_Reassemble = 8103,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Drill Option", "Adds Drill to the rotation.", MCH.JobID)]
    MCH_ST_Adv_Drill = 8109,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Hot Shot / Air Anchor Option", "Adds Hot Shot/Air Anchor to the rotation.", MCH.JobID)]
    MCH_ST_Adv_AirAnchor = 8102,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Chain Saw Option", "Adds Chain Saw to the rotation.", MCH.JobID)]
    MCH_ST_Adv_Chainsaw = 8112,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Excavator Option", "Adds Excavator to the rotation.", MCH.JobID)]
    MCH_ST_Adv_Excavator = 8116,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Head Graze Option", "Uses Head Graze to interrupt during the rotation, where applicable.", MCH.JobID)]
    MCH_ST_Adv_Interrupt = 8113,

    [ParentCombo(MCH_ST_AdvancedMode)]
    [CustomComboInfo("Second Wind Option", "Use Second Wind when below the set HP percentage.", MCH.JobID)]
    MCH_ST_Adv_SecondWind = 8114,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(MCH.SpreadShot, MCH.Scattergun)]
    [ConflictingCombos(MCH_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Spreadshot with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.\nWill lock out input to keep Flamethrower going by replacing Spreadshot with Savage Blade.", MCH.JobID)]
    [AdvancedCombo]
    MCH_AoE_AdvancedMode = 8300,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Flamethrower Option", "Adds Flamethrower to the rotation.\n Changes to Savage blade when in use to prevent cancelling.", MCH.JobID)]
    MCH_AoE_Adv_FlameThrower = 8305,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Barrel Stabilizer Option", "Adds Barrel Stabilizer to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Stabilizer = 8307,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Full Metal Field Option", "Adds Full Metal Field to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Stabilizer_FullMetalField = 8308,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Gauss Round / Ricochet \nDouble Check / Checkmate option", "Adds Gauss Round and Ricochet or Double Check and Checkmate to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_GaussRicochet = 8302,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Hypercharge Option", "Adds Hypercharge to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Hypercharge = 8303,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Rook Autoturret/Automaton Queen Option", "Adds Rook Autoturret or Automaton Queen to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Queen = 8304,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Rook / Queen Overdrive Option", "Adds Rook or Queen Overdrive to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_QueenOverdrive = 8314,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Reassemble Option", "Adds Reassemble to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Reassemble = 8301,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Bioblaster Option", "Adds Bioblaster to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Bioblaster = 8306,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Air Anchor Option", "Adds Air Anchor to the the rotation.", MCH.JobID)]
    MCH_AoE_Adv_AirAnchor = 8313,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Chain Saw Option", "Adds Chain Saw to the the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Chainsaw = 8309,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Excavator Option", "Adds Excavator to the rotation.", MCH.JobID)]
    MCH_AoE_Adv_Excavator = 8310,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Head Graze Option", "Uses Head Graze to interrupt during the rotation, where applicable.", MCH.JobID)]
    MCH_AoE_Adv_Interrupt = 8311,

    [ParentCombo(MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Second Wind Option", "Use Second Wind when below the set HP percentage.", MCH.JobID)]
    MCH_AoE_Adv_SecondWind = 8399,

    #endregion

    #region Basic combo

    [ReplaceSkill(MCH.CleanShot, MCH.HeatedCleanShot)]
    [CustomComboInfo("Basic Combo", "Replace Clean Shot with its combo chain.", MCH.JobID)]
    [BasicCombo]
    MCH_ST_BasicCombo = 8117,

    #endregion

    #region Variant

    [Variant]
    [VariantParent(MCH_ST_AdvancedMode, MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", MCH.JobID)]
    MCH_Variant_Rampart = 8039,

    [Variant]
    [VariantParent(MCH_ST_AdvancedMode, MCH_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", MCH.JobID)]
    MCH_Variant_Cure = 8040,

    #endregion

    [ReplaceSkill(MCH.Dismantle)]
    [CustomComboInfo("Double Dismantle Protection", "Prevents the use of Dismantle when target already has the effect by replacing it with Savage Blade.", MCH.JobID)]
    MCH_DismantleProtection = 8042,

    [ReplaceSkill(MCH.Dismantle)]
    [CustomComboInfo("Dismantle - Tactician", "Swap dismantle with tactician when dismantle is on cooldown.", MCH.JobID)]
    MCH_DismantleTactician = 8058,

    #region Heatblast

    [ReplaceSkill(MCH.Heatblast, MCH.BlazingShot)]
    [CustomComboInfo("Single Button Heat Blast Feature", "Turns Heat Blast or Blazing Shot into Hypercharge \nwhen u have 50 or more heat or when u got Hypercharged buff.", MCH.JobID)]
    MCH_Heatblast = 8006,

    [ParentCombo(MCH_Heatblast)]
    [CustomComboInfo("Barrel Option", "Adds Barrel Stabilizer to the feature when off cooldown.", MCH.JobID)]
    MCH_Heatblast_AutoBarrel = 8052,

    [ParentCombo(MCH_Heatblast)]
    [CustomComboInfo("Wildfire Option", "Adds Wildfire to the feature when off cooldown and overheated.", MCH.JobID)]
    MCH_Heatblast_Wildfire = 8015,

    [ParentCombo(MCH_Heatblast)]
    [CustomComboInfo("Gauss Round / Ricochet \nDouble Check / Checkmate Option", "Switches between Heat Blast and either Gauss Round and Ricochet or Double Check and Checkmate, depending on cooldown timers.", MCH.JobID)]
    MCH_Heatblast_GaussRound = 8016,

    #endregion

    #region AutoCrossbow

    [ReplaceSkill(MCH.AutoCrossbow)]
    [CustomComboInfo("Single Button Auto Crossbow Feature", "Turns Auto Crossbow into Hypercharge when at or above 50 heat.", MCH.JobID)]
    MCH_AutoCrossbow = 8018,

    [ParentCombo(MCH_AutoCrossbow)]
    [CustomComboInfo("Barrel Option", "Adds Barrel Stabilizer to the feature when below 50 Heat Gauge.", MCH.JobID)]
    MCH_AutoCrossbow_AutoBarrel = 8019,

    [ParentCombo(MCH_AutoCrossbow)]
    [CustomComboInfo("Gauss Round / Ricochet\n Double Check / Checkmate Option", "Switches between Auto Crossbow and either Gauss Round and Ricochet or Double Check and Checkmate, depending on cooldown timers.", MCH.JobID)]
    MCH_AutoCrossbow_GaussRound = 8020,

    #endregion

    [ReplaceSkill(MCH.RookAutoturret, MCH.AutomatonQueen)]
    [CustomComboInfo("Overdrive Feature", "Replace Rook Autoturret and Automaton Queen with Overdrive while active.", MCH.JobID)]
    MCH_Overdrive = 8002,

    [ReplaceSkill(MCH.Drill, MCH.AirAnchor, MCH.HotShot, MCH.Chainsaw)]
    [CustomComboInfo("Big Hitter Feature", "Replace Hot Shot, Drill, Air Anchor, Chainsaw and Excavator depending on which is on cooldown.", MCH.JobID)]
    MCH_HotShotDrillChainsawExcavator = 8004,

    [ReplaceSkill(MCH.GaussRound, MCH.Ricochet, MCH.CheckMate, MCH.DoubleCheck)]
    [CustomComboInfo("Gauss Round / Ricochet \nDouble Check / Checkmate Feature", "Replace Gauss Round and Ricochet or Double Check and Checkmate with one or the other depending on which has more charges.", MCH.JobID)]
    MCH_GaussRoundRicochet = 8003,

    // Last value ST = 8117
    // Last value AoE = 8314
    // Last value Misc = 8058

    #endregion

    #region MONK
    
    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(MNK.Bootshine, MNK.LeapingOpo)]
    [ConflictingCombos(MNK_ST_BeastChakras, MNK_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Bootshine with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", MNK.JobID)]
    [SimpleCombo]
    MNK_ST_SimpleMode = 9004,

    [AutoAction(true, false)]
    [ReplaceSkill(MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
    [ConflictingCombos(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Arms of the Destroyer with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", MNK.JobID)]
    [SimpleCombo]
    MNK_AOE_SimpleMode = 9003,

    #endregion

    #region Monk Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(MNK.Bootshine, MNK.LeapingOpo)]
    [ConflictingCombos(MNK_ST_BeastChakras, MNK_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Bootshine with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", MNK.JobID)]
    [AdvancedCombo]
    MNK_ST_AdvancedMode = 9005,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("Meditation Option", "Adds Meditation to the rotation", MNK.JobID)]
    MNK_STUseMeditation = 9007,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("The Forbidden Chakra Option", "Adds The Forbidden Chakra to the rotation", MNK.JobID)]
    MNK_STUseTheForbiddenChakra = 9012,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("Form Shift Option", "Adds Form Shift to the rotation", MNK.JobID)]
    MNK_STUseFormShift = 9017,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("Opener Option", "Uses selected opener", MNK.JobID)]
    MNK_STUseOpener = 9006,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("Buffs Option", "Adds selected buffs to the rotation", MNK.JobID)]
    MNK_STUseBuffs = 9008,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo("Brotherhood Option", "Adds Brotherhood to the rotation", MNK.JobID)]
    MNK_STUseBrotherhood = 9009,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo("Riddle of Fire Option", "Adds Riddle of Fire to the rotation", MNK.JobID)]
    MNK_STUseROF = 9011,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo("Fire's Reply Option", "Adds Fire's Reply to the rotation", MNK.JobID)]
    MNK_STUseFiresReply = 9016,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo("Riddle of Wind Option", "Adds Riddle of Wind to the rotation", MNK.JobID)]
    MNK_STUseROW = 9010,

    [ParentCombo(MNK_STUseBuffs)]
    [CustomComboInfo("Wind's Reply Option", "Adds Wind's Reply to the rotation", MNK.JobID)]
    MNK_STUseWindsReply = 9015,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("Perfect Balance Option", "Adds Perfect Balance to the rotation", MNK.JobID)]
    MNK_STUsePerfectBalance = 9013,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo(" Masterful Blitz Option", "Adds Masterful Blitz to the rotation", MNK.JobID)]
    MNK_STUseMasterfulBlitz = 9039,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("True North Option", "Adds True North dynamically, when not in positional, to the rotation", MNK.JobID)]
    MNK_STUseTrueNorth = 9014,

    [ParentCombo(MNK_ST_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the rotation.", MNK.JobID)]
    MNK_ST_ComboHeals = 9018,

    #endregion

    #region Monk Advanced AOE

    [AutoAction(true, false)]
    [ReplaceSkill(MNK.ArmOfTheDestroyer, MNK.ShadowOfTheDestroyer)]
    [ConflictingCombos(MNK_AOE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Arms of the Destroyer with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", MNK.JobID)]
    [AdvancedCombo]
    MNK_AOE_AdvancedMode = 9027,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Meditation Option", "Adds Meditation to the rotation", MNK.JobID)]
    MNK_AoEUseMeditation = 9028,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Howling Fist Option", "Adds Howling Fist to the rotation", MNK.JobID)]
    MNK_AoEUseHowlingFist = 9033,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Form Shift Option", "Adds Form Shift to the rotation", MNK.JobID)]
    MNK_AoEUseFormShift = 9038,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Buffs Option", "Adds selected buffs to the rotation", MNK.JobID)]
    MNK_AoEUseBuffs = 9029,

    [ParentCombo(MNK_AoEUseBuffs)]
    [CustomComboInfo("Brotherhood Option", "Adds Brotherhood to the rotation", MNK.JobID)]
    MNK_AoEUseBrotherhood = 9030,

    [ParentCombo(MNK_AoEUseBuffs)]
    [CustomComboInfo("Riddle of Fire Option", "Adds Riddle of Fire to the rotation", MNK.JobID)]
    MNK_AoEUseROF = 9032,

    [ParentCombo(MNK_AoEUseROF)]
    [CustomComboInfo("Fire's Reply Option", "Adds Fire's Reply to the rotation", MNK.JobID)]
    MNK_AoEUseFiresReply = 9036,

    [ParentCombo(MNK_AoEUseBuffs)]
    [CustomComboInfo("Riddle of Wind Option", "Adds Riddle of Wind to the rotation", MNK.JobID)]
    MNK_AoEUseROW = 9031,

    [ParentCombo(MNK_AoEUseROW)]
    [CustomComboInfo("Wind's Reply Option", "Adds Wind's Reply to the rotation", MNK.JobID)]
    MNK_AoEUseWindsReply = 9035,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Perfect Balance Option", "Adds Perfect Balance to the rotation", MNK.JobID)]
    MNK_AoEUsePerfectBalance = 9034,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo(" Masterful Blitz Option", "Adds Masterful Blitz to the rotation", MNK.JobID)]
    MNK_AoEUseMasterfulBlitz = 9040,

    [ParentCombo(MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the rotation.", MNK.JobID)]
    MNK_AoE_ComboHeals = 9037,

    #endregion

    #region Basic Combo

    [ConflictingCombos(MNK_ST_AdvancedMode, MNK_ST_SimpleMode)]
    [CustomComboInfo("Beast Chakra Handlers", "Merge single target GCDs which share the same beast chakra", MNK.JobID)]
    MNK_ST_BeastChakras = 9019,

    [ReplaceSkill(MNK.Bootshine)]
    [CustomComboInfo("Opo-opo Option", "Replace Bootshine/Leaping Opo with Dragon Kick.", MNK.JobID)]
    [ParentCombo(MNK_ST_BeastChakras)]
    MNK_BC_OPOOPO = 9020,

    [ReplaceSkill(MNK.TrueStrike)]
    [CustomComboInfo("Raptor Option", "Replace True Strike/Rising Raptor with Twin Snakes.", MNK.JobID)]
    [ParentCombo(MNK_ST_BeastChakras)]
    MNK_BC_RAPTOR = 9021,

    [ReplaceSkill(MNK.SnapPunch)]
    [CustomComboInfo("Coeurl Option", "Replace Snap Punch/Pouncing Coeurl with Demolish.", MNK.JobID)]
    [ParentCombo(MNK_ST_BeastChakras)]
    MNK_BC_COEURL = 9022,

    #endregion

    #region Variant

    [Variant]
    [VariantParent(MNK_ST_SimpleMode, MNK_ST_AdvancedMode, MNK_AOE_SimpleMode, MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", MNK.JobID)]
    MNK_Variant_Rampart = 9025,

    [Variant]
    [VariantParent(MNK_ST_SimpleMode, MNK_ST_AdvancedMode, MNK_AOE_SimpleMode, MNK_AOE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", MNK.JobID)]
    MNK_Variant_Cure = 9026,

    #endregion
    
    #region Misc

    [ReplaceSkill(MNK.PerfectBalance)]
    [ConflictingCombos(MNK_PerfectBalanceProtection)]
    [CustomComboInfo("Perfect Balance Feature", "Perfect Balance becomes Masterful Blitz while you have 3 Beast Chakra.", MNK.JobID)]
    MNK_PerfectBalance = 9023,

    [ReplaceSkill(MNK.RiddleOfFire)]
    [CustomComboInfo("Riddle of Fire/Brotherhood Feature", "Replaces Riddle of Fire with Brotherhood when Riddle of Fire is on cooldown.", MNK.JobID)]
    MNK_Riddle_Brotherhood = 9024,

    [ReplaceSkill(MNK.RiddleOfFire)]
    [CustomComboInfo("Riddle of Fire/Brotherhood Feature", "Replaces Brotherhood with Riddle of Fire when Brotherhood is on cooldown.", MNK.JobID)]
    MNK_Brotherhood_Riddle = 9041,

    [ReplaceSkill(MNK.PerfectBalance)]
    [ConflictingCombos(MNK_PerfectBalance)]
    [CustomComboInfo("Perfect Balance Protection", "Replaces Perfect Balance with Savage Blade when you already have Perfect Balance active.", MNK.JobID)]
    MNK_PerfectBalanceProtection = 9042,
    
    #endregion

    #region Hidden Features

    [Hidden]
    [CustomComboInfo("Hidden Options", "Collection of cheeky or encounter-specific extra options only available to those in the know.\nDo not expect these options to be maintained, or even kept, after they are no longer Current.", MNK.JobID)]
    MNK_Hidden = 9300,

    [ParentCombo(MNK_Hidden)]
    [Hidden]
    [CustomComboInfo("M6S: Hold Burst on Squirrels", "When you're targeting Squirrels in M6S add phase, hold burst.\n(until about the time the first manta is dying)", MNK.JobID)]
    MNK_Hid_M6SHoldSquirrelBurst = 9301,

    #endregion

    // Last value = 9042

    #endregion

    #region NINJA
    
    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(NIN.SpinningEdge)]
    [ConflictingCombos(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Spinning Edge with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.",
        NIN.JobID)]
    [SimpleCombo]
    NIN_ST_SimpleMode = 10000,

    [AutoAction(true, false)]
    [ReplaceSkill(NIN.DeathBlossom)]
    [ConflictingCombos(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Death Blossom with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.",
        NIN.JobID)]
    [SimpleCombo]
    NIN_AoE_SimpleMode = 10002,

    #endregion

    [AutoAction(false, false)]
    [ReplaceSkill(NIN.SpinningEdge)]
    [ConflictingCombos(NIN_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Spinning Edge with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        NIN.JobID)]
    [AdvancedCombo]
    NIN_ST_AdvancedMode = 10003,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)",
        "Adds the Balance opener at level 100.\nRequirements:\n- 2 mudra charges ready\n- Dokumori off cooldown.\n- Kunai's Bane off cooldown.\n- TenChiJin off cooldown.\n- Phantom Kamaitachi off cooldown.\n- Bunshin off cooldown.\n- Dream Within a Dream off cooldown.\n- Kassatsu off cooldown.",
        NIN.JobID)]
    NIN_ST_AdvancedMode_BalanceOpener = 10029,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Throwing Dagger Uptime Option", "Adds Throwing Dagger to Advanced Mode if out of melee range.",
        NIN.JobID)]
    NIN_ST_AdvancedMode_RangedUptime = 10004,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Mug/Dokumori Option", "Adds Mug/Dokumori to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Mug = 10005,

    [ConflictingCombos(NIN_ST_AdvancedMode_Mug_AlignBefore)]
    [ParentCombo(NIN_ST_AdvancedMode_Mug)]
    [CustomComboInfo("Align Mug with Trick Attack/Kunai's Bane Option",
        "Only uses Mug whilst the target has Trick Attack/Kunai's Bane, otherwise will use on cooldown.", NIN.JobID)]
    NIN_ST_AdvancedMode_Mug_AlignAfter = 10006,

    [ConflictingCombos(NIN_ST_AdvancedMode_Mug_AlignAfter)]
    [ParentCombo(NIN_ST_AdvancedMode_Mug)]
    [CustomComboInfo("Use Mug before Trick Attack/Kunai's Bane Option",
        "Aligns Mug with Trick Attack/Kunai's Bane but weaves it at least 1 GCD before Trick Attack/Kunai's Bane.",
        NIN.JobID)]
    NIN_ST_AdvancedMode_Mug_AlignBefore = 10007,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Trick Attack/Kunai's Bane Option", "Adds Trick Attack/Kunai's Bane to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_TrickAttack = 10008,

    [ParentCombo(NIN_ST_AdvancedMode_TrickAttack)]
    [CustomComboInfo("Save Cooldowns Before Trick Attack/Kunai's Bane Option",
        "Stops using abilities with longer cooldowns up to 15 seconds before Trick Attack/Kunai's Bane comes off cooldown.",
        NIN.JobID)]
    NIN_ST_AdvancedMode_TrickAttack_Cooldowns = 10009,

    [ParentCombo(NIN_ST_AdvancedMode_TrickAttack)]
    [CustomComboInfo("Delayed Trick Attack/Kunai's Bane Option",
        "Waits at least 8 seconds into combat before using Trick Attack/Kunai's Bane.", NIN.JobID)]
    NIN_ST_AdvancedMode_TrickAttack_Delayed = 10010,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Ninjitsu Option", "Adds Ninjitsu to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Ninjitsus = 10011,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Hold 1 Charge", "Prevent using both charges of Mudra.", NIN.JobID)]
    NIN_ST_AdvancedMode_Ninjitsus_ChargeHold = 10012,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Use Fuma Shuriken", "Spends Mudra charges on Fuma Shuriken (only before Raiton is available).",
        NIN.JobID)]
    NIN_ST_AdvancedMode_Ninjitsus_FumaShuriken = 10013,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Use Raiton", "Spends Mudra charges on Raiton.", NIN.JobID)]
    NIN_ST_AdvancedMode_Ninjitsus_Raiton = 10014,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Use Suiton", "Spends Mudra charges on Suiton.", NIN.JobID)]
    NIN_ST_AdvancedMode_Ninjitsus_Suiton = 10015,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Assassinate/Dream Within a Dream Option",
        "Adds Assassinate and Dream Within a Dream to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_AssassinateDWAD = 10017,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Kassatsu Option", "Adds Kassatsu to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Kassatsu = 10018,

    [ParentCombo(NIN_ST_AdvancedMode_Kassatsu)]
    [CustomComboInfo("Use Hyosho Ranryu Option", "Spends Kassatsu on Hyosho Ranryu.", NIN.JobID)]
    NIN_ST_AdvancedMode_Kassatsu_HyoshoRaynryu = 10019,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Bhavacakra Option", "Adds Bhavacakra to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Bhavacakra = 10022,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Ten Chi Jin Option", "Adds Ten Chi Jin (the cooldown) to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_TCJ = 10023,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Tenri Jindo Option", "Adds Tenri Jindo to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_TenriJindo = 10071,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Meisui Option", "Adds Meisui to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Meisui = 10024,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Bunshin Option", "Adds Bunshin to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Bunshin = 10025,

    [ParentCombo(NIN_ST_AdvancedMode_Bunshin)]
    [CustomComboInfo("Phantom Kamaitachi Option", "Adds Phantom Kamaitachi to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Bunshin_Phantom = 10026,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Raiju Option", "Adds Fleeting/Forked Raiju to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Raiju = 10027,

    [ParentCombo(NIN_ST_AdvancedMode_Raiju)]
    [CustomComboInfo("Forked Raiju Gap-Closer Option", "Uses Forked Raiju when out of range.", NIN.JobID)]
    NIN_ST_AdvancedMode_Raiju_Forked = 10028,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("True North Option", "Adds True North to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_TrueNorth = 10030,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Second Wind Option", "Adds Second Wind to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_SecondWind = 10032,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Shade Shift Option", "Adds Shade Shift to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_ShadeShift = 10033,

    [ParentCombo(NIN_ST_AdvancedMode)]
    [CustomComboInfo("Bloodbath Option", "Adds Bloodbath to Advanced Mode.", NIN.JobID)]
    NIN_ST_AdvancedMode_Bloodbath = 10034,

    [AutoAction(true, false)]
    [ReplaceSkill(NIN.DeathBlossom)]
    [ConflictingCombos(NIN_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Death Blossom with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        NIN.JobID)]
    [AdvancedCombo]
    NIN_AoE_AdvancedMode = 10035,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Kunai's Bane Option", "Adds Kunai's Bane to Advanced Mode. (Does not add Trick Attack)",
        NIN.JobID)]
    NIN_AoE_AdvancedMode_KunaisBane = 10073,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Assassinate/Dream Within a Dream Option",
        "Adds Assassinate/Dream Within a Dream to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_AssassinateDWAD = 10036,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Ninjitsu Option", "Adds Ninjitsu to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Ninjitsus = 10037,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Hold 1 Charge", "Prevent using both charges of Mudra.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Ninjitsus_ChargeHold = 10038,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Use Katon", "Spends Mudra charges on Katon.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Ninjitsus_Katon = 10039,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Use Doton", "Spends Mudra charges on Doton.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Ninjitsus_Doton = 10040,

    [ParentCombo(NIN_AoE_AdvancedMode_Ninjitsus)]
    [CustomComboInfo("Use Huton", "Spends Mudra charges on Huton.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Ninjitsus_Huton = 10041,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Kassatsu Option", "Adds Kassatsu to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Kassatsu = 10042,

    [ParentCombo(NIN_AoE_AdvancedMode_Kassatsu)]
    [CustomComboInfo("Goka Mekkyaku Option", "Adds Goka Mekkyaku to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_GokaMekkyaku = 10043,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Hellfrog Medium Option", "Adds Hellfrog Medium to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_HellfrogMedium = 10045,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Ten Chi Jin Option", "Adds Ten Chi Jin (the cooldown) to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_TCJ = 10046,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Tenri Jindo Option", "Adds Tenri Jindo to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_TenriJindo = 10072,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Meisui Option", "Adds Meisui to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Meisui = 10047,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Bunshin Option", "Adds Bunshin to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Bunshin = 10048,

    [ParentCombo(NIN_AoE_AdvancedMode_Bunshin)]
    [CustomComboInfo("Phantom Kamaitachi Option", "Adds Phantom Kamaitachi to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Bunshin_Phantom = 10049,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Second Wind Option", "Adds Second Wind to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_SecondWind = 10050,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Shade Shift Option", "Adds Shade Shift to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_ShadeShift = 10051,

    [ParentCombo(NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Bloodbath Option", "Adds Bloodbath to Advanced Mode.", NIN.JobID)]
    NIN_AoE_AdvancedMode_Bloodbath = 10052,
        
    #region Basic Combo

    [ReplaceSkill(NIN.AeolianEdge)]
    [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain.", NIN.JobID)]
    [BasicCombo]
    NIN_ST_AeolianEdgeCombo = 10074,

    [ReplaceSkill(NIN.ArmorCrush)]
    [CustomComboInfo("Armor Crush Combo Feature", "Replace Armor Crush with its combo chain.", NIN.JobID)]
    NIN_ArmorCrushCombo = 10053,

    #endregion
    
    [ReplaceSkill(NIN.Kassatsu)]
    [CustomComboInfo("Kassatsu to Trick Feature",
        "Replaces Kassatsu with Trick Attack/Kunai's Bane while Suiton or Hidden is up.\nCooldown tracking plugin recommended.",
        NIN.JobID)]
    NIN_KassatsuTrick = 10054,

    [ReplaceSkill(NIN.TenChiJin)]
    [CustomComboInfo("Ten Chi Jin to Meisui Feature",
        "Replaces Ten Chi Jin (the move) with Meisui while Suiton is up.\nCooldown tracking plugin recommended.",
        NIN.JobID)]
    NIN_TCJMeisui = 10055,

    [ReplaceSkill(NIN.Chi)]
    [CustomComboInfo("Kassatsu Chi/Jin Feature",
        "Replaces Chi with Jin while Kassatsu is up if you have Enhanced Kassatsu.", NIN.JobID)]
    NIN_KassatsuChiJin = 10056,

    [ReplaceSkill(NIN.Hide)]
    [CustomComboInfo("Hide to Mug/Trick Attack/Kunai's Bane Feature",
        "Replaces Hide with Mug while in combat and Trick Attack/Kunai's Bane whilst Hidden.", NIN.JobID)]
    NIN_HideMug = 10057,

    [ReplaceSkill(NIN.Ten, NIN.Chi, NIN.Jin)]
    [CustomComboInfo("Simple Mudras Feature", "Simplify the mudra casting to avoid failing.", NIN.JobID)]
    NIN_Simple_Mudras = 10062,

    [ReplaceSkill(NIN.TenChiJin)]
    [ParentCombo(NIN_TCJMeisui)]
    [CustomComboInfo("Ten Chi Jin Feature", "Turns Ten Chi Jin (the move) into Ten, Chi, and Jin.", NIN.JobID)]
    NIN_TCJ = 10063,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus_Raiton)]
    [CustomComboInfo("Raiton Uptime Option", "Adds Raiton as an uptime feature.", NIN.JobID)]
    NIN_ST_AdvancedMode_Raiton_Uptime = 10065,

    [ParentCombo(NIN_ST_AdvancedMode_Bunshin_Phantom)]
    [CustomComboInfo("Phantom Kamaitachi Uptime Option", "Adds Phantom Kamaitachi as an uptime feature.", NIN.JobID)]
    NIN_ST_AdvancedMode_Phantom_Uptime = 10066,

    [ParentCombo(NIN_ST_AdvancedMode_Ninjitsus_Suiton)]
    [CustomComboInfo("Suiton Uptime Option", "Adds Suiton as an uptime feature.", NIN.JobID)]
    NIN_ST_AdvancedMode_Suiton_Uptime = 10067,

    [Variant]
    [VariantParent(NIN_ST_SimpleMode, NIN_ST_AdvancedMode, NIN_AoE_SimpleMode, NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", NIN.JobID)]
    NIN_Variant_Cure = 10069,

    [Variant]
    [VariantParent(NIN_ST_SimpleMode, NIN_ST_AdvancedMode, NIN_AoE_SimpleMode, NIN_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", NIN.JobID)]
    NIN_Variant_Rampart = 10070,

    // Last value = 10075

    #endregion

    #region PICTOMANCER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(PCT.FireInRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Fire in Red with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.",
        PCT.JobID)]
    [SimpleCombo]
    PCT_ST_SimpleMode = 20000,

    [AutoAction(true, false)]
    [ReplaceSkill(PCT.FireIIinRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Fire in Red II with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.",
        PCT.JobID)]
    [SimpleCombo]
    PCT_AoE_SimpleMode = 20001,

    #endregion

    #region ST

    [AutoAction(false, false)]
    [ReplaceSkill(PCT.FireInRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Fire in Red with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        PCT.JobID)]
    [AdvancedCombo]
    PCT_ST_AdvancedMode = 20005,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)",
        "Adds the Balance opener at level 100.",
        PCT.JobID)]
    PCT_ST_Advanced_Openers = 20006,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Optimal Burst Window Feature", "Uses an optimized rotation for burst window for Level 100", PCT.JobID)]
    PCT_ST_AdvancedMode_Burst_Phase = 20010,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Subtractive Palette Feature", "Adds Subtractive Palette to the combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_SubtractivePalette = 20025,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Blizzard in Cyan Option", "Adds Blizzard in Cyan to the combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_BlizzardInCyan = 20033,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Comet in Black Option", "Adds Comet in Black to the combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_CometinBlack = 20026,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Living Muse Option", "Adds Living Muse.", PCT.JobID)]
    PCT_ST_AdvancedMode_LivingMuse = 20022,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Mog/Madeen Feature", "Adds Mog/Madeen to the combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_MogOfTheAges = 20024,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Steel Muse Option", "Adds Steel Muse.", PCT.JobID)]
    PCT_ST_AdvancedMode_SteelMuse = 20023,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Hammer Stamp Combo Option", "Adds Hammer Stamp combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_HammerStampCombo = 20027,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Scenic Muse Option", "Adds Scenic Muse.", PCT.JobID)]
    PCT_ST_AdvancedMode_ScenicMuse = 20021,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Star Prism Option", "Adds Star Prism to the Rotation.", PCT.JobID)]
    PCT_ST_AdvancedMode_StarPrism = 20012,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Rainbow Drip Option", "Adds Rainbow Drip to the rotation with Rainbow Birght Buff.", PCT.JobID)]
    PCT_ST_AdvancedMode_RainbowDrip = 20013,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming to the combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_LucidDreaming = 20034,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Motif Selection Feature", "Add Selected Motifs to the combo.", PCT.JobID)]
    PCT_ST_AdvancedMode_MotifFeature = 20016,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Landscape Motif Option", "Adds Landscape Motif.", PCT.JobID)]
    PCT_ST_AdvancedMode_LandscapeMotif = 20017,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Creature Motif Option", "Adds Landscape Motif.", PCT.JobID)]
    PCT_ST_AdvancedMode_CreatureMotif = 20018,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Weapon Motif Option", "Adds Weapon Motif.", PCT.JobID)]
    PCT_ST_AdvancedMode_WeaponMotif = 20019,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Prepull Motifs Feature", "Adds missing Motifs to the combo while out of combat.", PCT.JobID)]
    PCT_ST_AdvancedMode_PrePullMotifs = 20008,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Downtime Motifs Option", "Adds missing Motifs to the combo while no target is present in combat.",
        PCT.JobID)]
    PCT_ST_AdvancedMode_NoTargetMotifs = 20009,

    [ParentCombo(PCT_ST_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Swiftcast Motifs Option ", "Use swiftcast for motifs.", PCT.JobID)]
    PCT_ST_AdvancedMode_SwiftMotifs = 20035,

    [ParentCombo(PCT_ST_AdvancedMode)]
    [CustomComboInfo("Movement Features", "Adds selected features to the combo while moving.", PCT.JobID)]
    PCT_ST_AdvancedMode_MovementFeature = 20028,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Hammer Stamp Combo Option", "Adds Hammer Stamp Combo to the combo while moving.", PCT.JobID)]
    PCT_ST_AdvancedMode_MovementOption_HammerStampCombo = 20029,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Holy in White Option", "Adds Holy in White to the combo while moving. \n Prioritizes if Inspiration and Hyperphantasia are active.", PCT.JobID)]
    PCT_ST_AdvancedMode_MovementOption_HolyInWhite = 20030,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Comet in Black Option", "Adds Comet in Black to the combo while moving. \n Prioritizes if Inspiration and Hyperphantasia are active.", PCT.JobID)]
    PCT_ST_AdvancedMode_MovementOption_CometinBlack = 20031,

    [ParentCombo(PCT_ST_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Swiftcast Option ", "Adds Swiftcast to the combo while moving.", PCT.JobID)]
    PCT_ST_AdvancedMode_SwitfcastOption = 20032,

    #endregion

    #region AoE

    [AutoAction(true, false)]
    [ReplaceSkill(PCT.FireIIinRed)]
    [ConflictingCombos(CombinedAetherhues, PCT_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Fire in Red II with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        PCT.JobID)]
    [AdvancedCombo]
    PCT_AoE_AdvancedMode = 20040,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Subtractive Palette Feature", "Adds Subtractive Palette to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_SubtractivePalette = 20058,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Blizzard II in Cyan Option", "Adds Blizzard II in Cyan to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_BlizzardInCyan = 20066,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Holy in White Option", "Adds Holy in White to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_HolyinWhite = 20068,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Comet in Black Option", "Adds Comet in Black to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_CometinBlack = 20059,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Living Muse Option", "Adds Living Muse.", PCT.JobID)]
    PCT_AoE_AdvancedMode_LivingMuse = 20055,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Mog/Madeen Feature", "Adds Mog/Madeen to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_MogOfTheAges = 20057,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Steel Muse Option", "Adds Steel Muse.", PCT.JobID)]
    PCT_AoE_AdvancedMode_SteelMuse = 20056,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Hammer Stamp Combo Option", "Adds Hammer Stamp combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_HammerStampCombo = 20060,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Scenic Muse Option", "Adds Scenic Muse.", PCT.JobID)]
    PCT_AoE_AdvancedMode_ScenicMuse = 20054,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Star Prism Option", "Adds Star Prism to the burst phase.", PCT.JobID)]
    PCT_AoE_AdvancedMode_StarPrism = 20045,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Rainbow Drip Option", "Adds Rainbow Drip to the burst phase.", PCT.JobID)]
    PCT_AoE_AdvancedMode_RainbowDrip = 20046,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_LucidDreaming = 20067,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Motif Selection Feature", "Add Selected Motifs to the combo.", PCT.JobID)]
    PCT_AoE_AdvancedMode_MotifFeature = 20049,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Landscape Motif Option", "Adds Landscape Motif.", PCT.JobID)]
    PCT_AoE_AdvancedMode_LandscapeMotif = 20050,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Creature Motif Option", "Adds Landscape Motif.", PCT.JobID)]
    PCT_AoE_AdvancedMode_CreatureMotif = 20051,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Weapon Motif Option", "Adds Weapon Motif.", PCT.JobID)]
    PCT_AoE_AdvancedMode_WeaponMotif = 20052,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Prepull Motifs Feature", "Adds missing Motifs to the combo while out of combat.", PCT.JobID)]
    PCT_AoE_AdvancedMode_PrePullMotifs = 20041,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Downtime Motifs Option", "Adds missing Motifs to the combo while no target is present in combat.",
        PCT.JobID)]
    PCT_AoE_AdvancedMode_NoTargetMotifs = 20042,

    [ParentCombo(PCT_AoE_AdvancedMode_MotifFeature)]
    [CustomComboInfo("Swiftcast Motifs Option ", "Use swiftcast for motifs.", PCT.JobID)]
    PCT_AoE_AdvancedMode_SwiftMotifs = 20069,

    [ParentCombo(PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Movement Features", "Adds selected features to the combo while moving.", PCT.JobID)]
    PCT_AoE_AdvancedMode_MovementFeature = 20061,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Hammer Stamp Combo Option", "Adds Hammer Stamp Combo to the combo while moving.", PCT.JobID)]
    PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo = 20062,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Holy in White Option", "Adds Holy in White to the combo while moving.  \n Prioritizes if Inspiration and Hyperphantasia are active.", PCT.JobID)]
    PCT_AoE_AdvancedMode_MovementOption_HolyInWhite = 20063,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Comet in Black Option", "Adds Comet in Black to the combo while moving. \n Prioritizes if Inspiration and Hyperphantasia are active.", PCT.JobID)]
    PCT_AoE_AdvancedMode_MovementOption_CometinBlack = 20064,

    [ParentCombo(PCT_AoE_AdvancedMode_MovementFeature)]
    [CustomComboInfo("Swiftcast Option ", "Adds Swiftcast to the combo while moving.", PCT.JobID)]
    PCT_AoE_AdvancedMode_SwitfcastOption = 20065,

    #endregion

    #region Standalone Features
    
    [ReplaceSkill(PCT.FireInRed, PCT.FireIIinRed)]
    [ConflictingCombos(PCT_ST_SimpleMode, PCT_AoE_SimpleMode)]
    [CustomComboInfo("Combined Aetherhues Feature",
        "Merges aetherhue actions for specific target types into a single button.", PCT.JobID)]
    CombinedAetherhues = 20002,

    [ReplaceSkill(PCT.CreatureMotif, PCT.WeaponMotif, PCT.LandscapeMotif)]
    [CustomComboInfo("One Button Motifs", "Merges Motifs and Muses into a single button.", PCT.JobID)]
    CombinedMotifs = 20003,

    [ReplaceSkill(PCT.HolyInWhite)]
    [CustomComboInfo("One Button Paint", "Consolidates paint-consuming actions into one button.", PCT.JobID)]
    CombinedPaint = 20004,

    #endregion

    #region Variant

    [Variant]
    [VariantParent(PCT_ST_SimpleMode, PCT_AoE_SimpleMode, PCT_ST_AdvancedMode, PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", PCT.JobID)]
    PCT_Variant_Cure = 20100,

    [Variant]
    [VariantParent(PCT_ST_SimpleMode, PCT_AoE_SimpleMode, PCT_ST_AdvancedMode, PCT_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", PCT.JobID)]
    PCT_Variant_Rampart = 20101,

    #endregion

    #endregion

    #region PALADIN
    
    #region Simple Mode

    // Simple Modes
    [AutoAction(false, false)]
    [ConflictingCombos(PLD_ST_AdvancedMode)]
    [ReplaceSkill(PLD.FastBlade)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Fast Blade with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.",
        PLD.JobID)]
    [SimpleCombo]
    PLD_ST_SimpleMode = 11000,

    [AutoAction(true, false)]
    [ConflictingCombos(PLD_AoE_AdvancedMode)]
    [ReplaceSkill(PLD.TotalEclipse)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Total Eclipse with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.",
        PLD.JobID)]
    [SimpleCombo]
    PLD_AoE_SimpleMode = 11001,

    #endregion

    #region ST Advanced Mode

    [AutoAction(false, false)]
    [ConflictingCombos(PLD_ST_SimpleMode)]
    [ReplaceSkill(PLD.FastBlade)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Fast Blade with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        PLD.JobID)]
    [AdvancedCombo]
    PLD_ST_AdvancedMode = 11002,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", PLD.JobID)]
    PLD_ST_AdvancedMode_BalanceOpener = 11046,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", PLD.JobID)]
    PLD_ST_Interrupt = 11058,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Interrupt with Low Blow Option", "Adds Low Blow to the rotation when your target is casting.\nNot advised outside of overworld content, as it can waste a lot of Low Blows on un-stun-able enemies, etc. Will try to not use it in boss fights.", PLD.JobID)]
    PLD_ST_LowBlow = 11062,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Interrupt with Shield Bash Option", "Adds Shield Bash to the rotation when your target is casting.\nNot advised outside of overworld content, as it can waste a lot of GCDs on un-stun-able enemies, etc. Will try to not use it in boss fights.", PLD.JobID)]
    PLD_ST_ShieldBash = 11066,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Fight or Flight Option",
        "Adds Fight or Flight to Advanced Mode.\n- Uses after Royal Authority during opener.\n- Afterward, on cooldown alongside Requiescat.\n- Target HP must be at or above:",
        PLD.JobID)]
    PLD_ST_AdvancedMode_FoF = 11003,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Shield Lob Option",
        "Adds Shield Lob to Advanced Mode.\n- Uses only while out of melee range.\n- Will not override better actions.",
        PLD.JobID)]
    PLD_ST_AdvancedMode_ShieldLob = 11004,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Circle of Scorn Option",
        "Adds Circle of Scorn to Advanced Mode.\n- Uses only when in range of the target.\n- Prefers to use during Fight or Flight.",
        PLD.JobID)]
    PLD_ST_AdvancedMode_CircleOfScorn = 11005,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Spirits Within Option",
        "Adds Spirits Within to Advanced Mode.\n- Prefers to use during Fight or Flight.", PLD.JobID)]
    PLD_ST_AdvancedMode_SpiritsWithin = 11006,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Goring Blade Option", "Adds Goring Blade to Advanced Mode.\n- Prefers to use after Requiescat.",
        PLD.JobID)]
    PLD_ST_AdvancedMode_GoringBlade = 11008,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Holy Spirit Option",
        "Adds Holy Spirit to Advanced Mode.\n- Uses only when under Divine Might.\n- Will be prioritized if buff is expiring.",
        PLD.JobID)]
    PLD_ST_AdvancedMode_HolySpirit = 11009,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Requiescat Option", "Adds Requiescat to Advanced Mode.\n- Uses after Fight or Flight.", PLD.JobID
    )]
    PLD_ST_AdvancedMode_Requiescat = 11010,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Intervene Option",
        "Adds Intervene to Advanced Mode.\n- Prefers to use during Fight or Flight.\n- Will not use during movement.\n- Amount of charges to keep:",
        PLD.JobID)]
    PLD_ST_AdvancedMode_Intervene = 11011,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Atonement Option",
        "Adds the Atonement chain to Advanced Mode.\n- Will be prioritized if buff is expiring.", PLD.JobID)]
    PLD_ST_AdvancedMode_Atonement = 11012,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Confiteor Option",
        "Adds Confiteor to Advanced Mode.\n- At lower levels, uses Holy Spirit instead.", PLD.JobID)]
    PLD_ST_AdvancedMode_Confiteor = 11013,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Blade Chain Option",
        "Adds Blade of Faith/Truth/Valor to Advanced Mode.\n- At lower levels, uses Holy Spirit instead.", PLD.JobID
    )]
    PLD_ST_AdvancedMode_Blades = 11014,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Blade of Honor Option", "Adds Blade of Honor to Advanced Mode.\n- Uses after Blade of Valor.",
        PLD.JobID)]
    PLD_ST_AdvancedMode_BladeOfHonor = 11033,

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("MP Reservation Option",
        "Adds a minimum MP limit to Advanced Mode.\n- This is not recommended in most cases.\n- Player MP must remain at or above:",
        PLD.JobID)]
    PLD_ST_AdvancedMode_MP_Reserve = 11035,

    // ST Mitigation Options

    [ParentCombo(PLD_ST_AdvancedMode)]
    [CustomComboInfo("Mitigation Options",
        "Adds defensive actions to Advanced Mode.\n- Will not override offensive actions.\n- Uses only when being targeted.",
        PLD.JobID)]
    PLD_ST_AdvancedMode_Mitigation = 11038,

    [ParentCombo(PLD_ST_AdvancedMode_Mitigation)]
    [CustomComboInfo("Sheltron Option", "Adds Sheltron.\n- Required gauge threshold:", PLD.JobID)]
    PLD_ST_AdvancedMode_Sheltron = 11007,

    [ParentCombo(PLD_ST_AdvancedMode_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart.\n- Player HP must be under:", PLD.JobID)]
    PLD_ST_AdvancedMode_Rampart = 11039,

    [ParentCombo(PLD_ST_AdvancedMode_Mitigation)]
    [CustomComboInfo("Sentinel Option", "Adds Sentinel.\n- Player HP must be under:", PLD.JobID)]
    PLD_ST_AdvancedMode_Sentinel = 11040,

    [ParentCombo(PLD_ST_AdvancedMode_Mitigation)]
    [CustomComboInfo("Hallowed Ground Option", "Adds Hallowed Ground.\n- Player HP must be under:", PLD.JobID)]
    PLD_ST_AdvancedMode_HallowedGround = 11041,

    #endregion

    #region AoE Advanced Mode

    [AutoAction(true, false)]
    [ConflictingCombos(PLD_AoE_SimpleMode)]
    [ReplaceSkill(PLD.TotalEclipse)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Total Eclipse with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        PLD.JobID)]
    [AdvancedCombo]
    PLD_AoE_AdvancedMode = 11015,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", PLD.JobID)]
    PLD_AoE_Interrupt = 11059,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Interrupt with Low Blow Option", "Adds Low Blow to the rotation when your target is casting, interruptible or not.", PLD.JobID)]
    PLD_AoE_LowBlow = 11060,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Interrupt with Shield Bash Option", "Adds Shield Bash to the rotation when your target is casting, interruptible or not.", PLD.JobID)]
    PLD_AoE_ShieldBash = 11065,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Fight or Flight Option",
        "Adds Fight or Flight to Advanced Mode.\n- Uses on cooldown alongside Requiescat.\n- Target HP must be at or above:",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_FoF = 11016,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Spirits Within Option",
        "Adds Spirits Within to Advanced Mode.\n- Prefers to use during Fight or Flight.", PLD.JobID)]
    PLD_AoE_AdvancedMode_SpiritsWithin = 11017,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Circle of Scorn Option",
        "Adds Circle of Scorn to Advanced Mode.\n- Uses only when in range of the target.\n- Prefers to use during Fight or Flight.",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_CircleOfScorn = 11018,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Requiescat Option", "Adds Requiescat to Advanced Mode.\n- Uses after Fight or Flight.", PLD.JobID
    )]
    PLD_AoE_AdvancedMode_Requiescat = 11019,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Intervene Option",
        "Adds Intervene to Advanced Mode.\n- Prefers to use during Fight or Flight.\n- Will not use during movement.\n- Amount of charges to keep:",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_Intervene = 11037,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Holy Circle Option", "Adds Holy Circle to Advanced Mode.\n- Uses only when under Divine Might.",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_HolyCircle = 11020,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Confiteor Option",
        "Adds Confiteor to Advanced Mode.\n- At lower levels, uses Holy Circle instead.", PLD.JobID)]
    PLD_AoE_AdvancedMode_Confiteor = 11021,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Blade Chain Option",
        "Adds Blade of Faith/Truth/Valor to Advanced Mode.\n- At lower levels, uses Holy Circle instead.", PLD.JobID
    )]
    PLD_AoE_AdvancedMode_Blades = 11022,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Blade of Honor Option", "Adds Blade of Honor to Advanced Mode.\n- Uses after Blade of Valor.",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_BladeOfHonor = 11034,

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("MP Reservation Option",
        "Adds a minimum MP limit to Advanced Mode.\n- This is not recommended in most cases.\n- Player MP must remain at or above:",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_MP_Reserve = 11036,

    // AoE Mitigation Options

    [ParentCombo(PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Mitigation Options",
        "Adds defensive actions to Advanced Mode.\n- Will not override offensive actions.\n- Uses only when being targeted.",
        PLD.JobID)]
    PLD_AoE_AdvancedMode_Mitigation = 11042,

    [ParentCombo(PLD_AoE_AdvancedMode_Mitigation)]
    [CustomComboInfo("Sheltron Option", "Adds Sheltron.\n- Required gauge threshold:", PLD.JobID)]
    PLD_AoE_AdvancedMode_Sheltron = 11023,

    [ParentCombo(PLD_AoE_AdvancedMode_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart.\n- Player HP must be under:", PLD.JobID)]
    PLD_AoE_AdvancedMode_Rampart = 11043,

    [ParentCombo(PLD_AoE_AdvancedMode_Mitigation)]
    [CustomComboInfo("Sentinel Option", "Adds Sentinel.\n- Player HP must be under:", PLD.JobID)]
    PLD_AoE_AdvancedMode_Sentinel = 11044,

    [ParentCombo(PLD_AoE_AdvancedMode_Mitigation)]
    [CustomComboInfo("Hallowed Ground Option", "Adds Hallowed Ground.\n- Player HP must be under:", PLD.JobID)]
    PLD_AoE_AdvancedMode_HallowedGround = 11045,

    #endregion
    
    #region Basic combo

    [ReplaceSkill(PLD.RageOfHalone)]
    [CustomComboInfo("Basic Combo", "Replace Rage Of Halone with its combo chain.", PLD.JobID)]
    [BasicCombo]
    PLD_ST_BasicCombo = 11061,

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(PLD.Bulwark)]
    [CustomComboInfo("One-Button Mitigation Feature", "Replaces Bulwark with an all-in-one mitigation button.", PLD.JobID)]
    [MitigationCombo]
    PLD_Mit_OneButton = 11047,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Hallowed Ground Emergency Option", "Gives max priority to Hallowed Ground when the Health percentage threshold is met.", PLD.JobID)]
    PLD_Mit_HallowedGround_Max = 11048,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Sheltron Option", "Adds Sheltron to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_Sheltron = 11049,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal to the one-button mitigation.\nNOTE: Will not use unless there is a target within range to prevent waste", PLD.JobID)]
    PLD_Mit_Reprisal = 11050,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Divine Veil Option", "Adds Divine Veil to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_DivineVeil = 11051,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Rampart Option", "Adds Rampart to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_Rampart = 11052,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Sentinel Option", "Adds Sentinel / Guardian to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_Sentinel = 11053,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_ArmsLength = 11054,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Bulwark Option",
        "Adds Bulwark to the one-button mitigation." +
        "\nNOTE: even if disabled, will still try to use Bulwark as the lowest priority.", PLD.JobID)]
    PLD_Mit_Bulwark = 11055,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Hallowed Ground Option", "Adds Hallowed Ground to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_HallowedGround = 11056,

    [ParentCombo(PLD_Mit_OneButton)]
    [CustomComboInfo("Clemency Option", "Adds Clemency to the one-button mitigation.", PLD.JobID)]
    PLD_Mit_Clemency = 11057,

    [ReplaceSkill(PLD.DivineVeil)]
    [CustomComboInfo("One-Button Party Mitigation Feature", "Replaces Divine Veil with Reprisal when ready.", PLD.JobID)]
    [MitigationCombo]
    PLD_Mit_Party = 11063,

    [ParentCombo(PLD_Mit_Party)]
    [CustomComboInfo("Include Passage of Arms", "Includes Passage of Arms when both Divine Veil and Reprisal are on cooldown.",
        PLD.JobID)]
    PLD_Mit_Party_Wings = 11064,
    #endregion

    #region Extra Features

    [ReplaceSkill(PLD.Requiescat, PLD.Imperator)]
    [CustomComboInfo("Requiescat Spender Feature",
        "Replaces Requiescat with all Requiescat-related actions.\n- Prioritizes Confiteor and Blade actions when available.\n- Uses Holy Spirit or Holy Circle when appropriate.",
        PLD.JobID)]
    PLD_Requiescat_Options = 11024,

    [ReplaceSkill(PLD.SpiritsWithin, PLD.Expiacion)]
    [CustomComboInfo("Spirits Within / Circle of Scorn Feature",
        "Replaces Spirits Within with Circle of Scorn when available.", PLD.JobID)]
    PLD_SpiritsWithin = 11025,

    [ReplaceSkill(PLD.ShieldLob)]
    [CustomComboInfo("Shield Lob / Holy Spirit Feature",
        "Replaces Shield Lob with Holy Spirit when available.\n- Must be under the effect of Divine Might or not moving.",
        PLD.JobID)]
    PLD_ShieldLob_Feature = 11027,
    
    [ReplaceSkill(PLD.Clemency)]
    [CustomComboInfo("Retarget Clemency Feature", "Will retarget Clemency according to following Suboptions", PLD.JobID)]
    [Retargeted(PLD.Clemency)]
    PLD_RetargetClemency = 11067,
    
    [ParentCombo(PLD_RetargetClemency)]
    [CustomComboInfo("Mouseover Clemency Option", "Adds UI mouseover to the priority. Above LowHP option.", PLD.JobID)]
    [Retargeted]
    PLD_RetargetClemency_MO = 11071,
    
    [ParentCombo(PLD_RetargetClemency)]
    [CustomComboInfo("Low hp Clemency Option", "Will Heal Lowest Health Party member until you fall below set threshold", PLD.JobID)]
    [Retargeted]
    PLD_RetargetClemency_LowHP = 11072,
    
    [ReplaceSkill(PLD.Sheltron)]
    [CustomComboInfo("Sheltron to Intervention Feature", "Will use intervention on your Hard Target if target is a friendly party member, if not then Sheltron." +
                                                         "\n- UI Mousover > Hard target > Target's target > Self Sheltron", PLD.JobID)]
    [Retargeted(PLD.Sheltron)]
    PLD_RetargetSheltron = 11068,
    
    [ParentCombo(PLD_RetargetSheltron)]
    [CustomComboInfo("Mouseover Intervention Option", "Adds UI mouseover to the priority.", PLD.JobID)]
    [Retargeted]
    PLD_RetargetSheltron_MO = 11069,
    
    [ParentCombo(PLD_RetargetSheltron)]
    [CustomComboInfo("Target's Target Intervention Option", "Adds Target's Target to the priority when you do not have agro.", PLD.JobID)]
    [Retargeted]
    PLD_RetargetSheltron_TT = 11070,

    [Retargeted(PLD.ShieldBash)]
    [ConflictingCombos(ALL_Tank_Interrupt)]
    [CustomComboInfo("Retarget Shield Bash", "Redirects your Shield Bash to a stunnable enemy if your current target cannot be stunned.", PLD.JobID)]
    PLD_RetargetShieldBash = 11073,

    // Variant Features

    [Variant]
    [VariantParent(PLD_ST_SimpleMode, PLD_ST_AdvancedMode, PLD_AoE_SimpleMode, PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Spirit Dart Feature",
        "Uses Variant Spirit Dart whenever the debuff is not present on the target or about to expire.", PLD.JobID)]
    PLD_Variant_SpiritDart = 11030,

    [Variant]
    [VariantParent(PLD_ST_SimpleMode, PLD_ST_AdvancedMode, PLD_AoE_SimpleMode, PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Feature", "Uses Variant Cure when the player's HP falls below the set threshold.",
        PLD.JobID)]
    PLD_Variant_Cure = 11031,

    [Variant]
    [VariantParent(PLD_ST_SimpleMode, PLD_ST_AdvancedMode, PLD_AoE_SimpleMode, PLD_AoE_AdvancedMode)]
    [CustomComboInfo("Ultimatum Feature", "Uses Variant Ultimatum on cooldown as long as the target is within range.",
        PLD.JobID)]
    PLD_Variant_Ultimatum = 11032,

    #endregion

    //// Last value = 11072

    #endregion

    #region REAPER
    
    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(RPR.Slice)]
    [ConflictingCombos(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Slice with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", RPR.JobID)]
    [SimpleCombo]
    RPR_ST_SimpleMode = 12000,

    [AutoAction(true, false)]
    [ReplaceSkill(RPR.SpinningScythe)]
    [ConflictingCombos(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Spinning Scythe with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", RPR.JobID)]
    [SimpleCombo]
    RPR_AoE_SimpleMode = 12100,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(RPR.Slice)]
    [ConflictingCombos(RPR_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Slice with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", RPR.JobID)]
    [AdvancedCombo]
    RPR_ST_AdvancedMode = 12001,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", RPR.JobID)]
    RPR_ST_Opener = 12002,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Arcane Circle Option", "Adds Arcane Circle to the rotation.", RPR.JobID)]
    RPR_ST_ArcaneCircle = 12006,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Plentiful Harvest Option", "Adds Plentiful Harvest to the rotation.", RPR.JobID)]
    RPR_ST_PlentifulHarvest = 12007,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Shadow Of Death Option", "Adds Shadow of Death to the rotation.", RPR.JobID)]
    RPR_ST_SoD = 12003,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Soulsow", "Adds Soulsow to the rotation when out of combat and you dont have the buff.", RPR.JobID)]
    RPR_ST_SoulSow = 12020,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Soul Slice Option", "Adds Soul Slice to the rotation.", RPR.JobID)]
    RPR_ST_SoulSlice = 12004,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Bloodstalk Option", "Adds Bloodstalk to the rotation.", RPR.JobID)]
    RPR_ST_Bloodstalk = 12008,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Gluttony Option", "Adds Gluttony to the rotation.", RPR.JobID)]
    RPR_ST_Gluttony = 12009,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Gibbet and Gallows Option", "Adds Gibbet and Gallows to the rotation.", RPR.JobID)]
    RPR_ST_GibbetGallows = 12016,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Enshroud Option", "Adds Enshroud to the rotation.", RPR.JobID)]
    RPR_ST_Enshroud = 12010,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Void/Cross Reaping Option", "Adds Void Reaping and Cross Reaping to the rotation.\n(Disabling this may stop the one-button combo working during enshroud)", RPR.JobID)]
    RPR_ST_Reaping = 12011,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Lemure's Slice Option", "Adds Lemure's Slice to the rotation.", RPR.JobID)]
    RPR_ST_Lemure = 12012,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Communio Finisher Option", "Adds Communio to the rotation.", RPR.JobID)]
    RPR_ST_Communio = 12014,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Sacrificium Option", "Adds Sacrificium to the rotation.", RPR.JobID)]
    RPR_ST_Sacrificium = 12013,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Perfectio Option", "Adds Perfectio to the rotation.", RPR.JobID)]
    RPR_ST_Perfectio = 12015,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Dynamic True North Feature", "Adds True North before Gibbet/Gallows when you are not in the correct position.", RPR.JobID)]
    RPR_ST_TrueNorthDynamic = 12098,
    
    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Ranged Filler Option", "Replaces the combo chain with Harpe when outside of melee range. Will not override Communio.", RPR.JobID)]
    RPR_ST_RangedFiller = 12017,

    [ParentCombo(RPR_ST_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the combo, using them when below the HP Percentage threshold.", RPR.JobID)]
    RPR_ST_ComboHeals = 12097,

    //last value = 12021

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(RPR.SpinningScythe)]
    [ConflictingCombos(RPR_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Spinning Scythe with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", RPR.JobID)]
    [AdvancedCombo]
    RPR_AoE_AdvancedMode = 12101,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Arcane Circle Option", "Adds Arcane Circle to the rotation.", RPR.JobID)]
    RPR_AoE_ArcaneCircle = 12105,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Plentiful Harvest Option", "Adds Plentiful Harvest to the rotation.", RPR.JobID)]
    RPR_AoE_PlentifulHarvest = 12106,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Whorl Of Death Option", "Adds Whorl of Death to the rotation.", RPR.JobID)]
    RPR_AoE_WoD = 12102,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Soulsow", "Adds Soulsow to the rotation when out of combat and u dont have the buff.", RPR.JobID)]
    RPR_AoE_SoulSow = 12117,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Soul Scythe Option", "Adds Soul Scythe to the rotation.", RPR.JobID)]
    RPR_AoE_SoulScythe = 12103,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Grim Swathe Option", "Adds Grim Swathe to the rotation.", RPR.JobID)]
    RPR_AoE_GrimSwathe = 12107,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Gluttony Option", "Adds Gluttony to the rotation.", RPR.JobID)]
    RPR_AoE_Gluttony = 12108,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Guillotine Option", "Adds Guillotine to the rotation.", RPR.JobID)]
    RPR_AoE_Guillotine = 12115,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Enshroud Option", "Adds Enshroud to the rotation.", RPR.JobID)]
    RPR_AoE_Enshroud = 12109,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Grim Reaping Option", "Adds Grim Reaping to the rotation.\n(Disabling this may stop the one-button combo working during enshroud)", RPR.JobID)]
    RPR_AoE_Reaping = 12110,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Lemure's Scythe Option", "Adds Lemure's Scythe to the rotation.", RPR.JobID)]
    RPR_AoE_Lemure = 12111,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Communio Finisher Option", "Adds Communio to the rotation.", RPR.JobID)]
    RPR_AoE_Communio = 12113,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Sacrificium Option", "Adds Sacrificium to the rotation.", RPR.JobID)]
    RPR_AoE_Sacrificium = 12112,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Perfectio Option", "Adds Perfectio to the rotation.", RPR.JobID)]
    RPR_AoE_Perfectio = 12114,

    [ParentCombo(RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the combo, using them when below the HP Percentage threshold.", RPR.JobID)]
    RPR_AoE_ComboHeals = 12116,

    // Last value = 12117

    #endregion
    
    #region Basic combo

    [ReplaceSkill(RPR.InfernalSlice)]
    [CustomComboInfo("Basic Combo", "Replace Infernal Slice with its combo chain.", RPR.JobID)]
    [BasicCombo]
    RPR_ST_BasicCombo = 12021,

    #endregion

    #region Blood Stalk/Grim Swathe Combo Section

    [ReplaceSkill(RPR.BloodStalk, RPR.GrimSwathe)]
    [CustomComboInfo("Gluttony on Blood Stalk/Grim Swathe Feature", "Blood Stalk and Grim Swathe will turn into Gluttony when it is available.", RPR.JobID)]
    RPR_GluttonyBloodSwathe = 12200,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo("Gibbet and Gallows/Guillotine on Blood Stalk/Grim Swathe Feature", "Adds (Executioner's) Gibbet and Gallows on Blood Stalk.\nAdds (Executioner's) Guillotine on Grim Swathe.", RPR.JobID)]
    RPR_GluttonyBloodSwathe_BloodSwatheCombo = 12201,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo("Enshroud Combo Option", "Adds Enshroud combo (Void/Cross Reaping, Communio, Lemure's Slice, Sacrificium and Perfectio) on Blood Stalk and Grim Swathe.", RPR.JobID)]
    RPR_GluttonyBloodSwathe_Enshroud = 12202,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo("OGCD's Option", "Adds Enshroud, Lemure's Slice and Sacrificium.", RPR.JobID)]
    RPR_GluttonyBloodSwathe_OGCD = 12204,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo("Sacrificium only Option", "Adds only Sacrificium on Blood Stalk and Grim Swathe while enshrouded.", RPR.JobID)]
    RPR_GluttonyBloodSwathe_Sacrificium = 12203,

    [ParentCombo(RPR_GluttonyBloodSwathe)]
    [CustomComboInfo("True North Feature", "Adds True North when under Gluttony and if Gibbet/Gallows options are selected to replace those skills.", RPR.JobID)]
    RPR_TrueNorthGluttony = 12310,

    // Last value = 12204

    #endregion

    #region Variant

    [Variant]
    [VariantParent(RPR_ST_AdvancedMode, RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", RPR.JobID)]
    RPR_Variant_Cure = 12311,

    [Variant]
    [VariantParent(RPR_ST_AdvancedMode, RPR_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", RPR.JobID)]
    RPR_Variant_Rampart = 12312,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(RPR.Slice, RPR.SpinningScythe, RPR.ShadowOfDeath, RPR.Harpe, RPR.BloodStalk)]
    [CustomComboInfo("Soulsow Reminder Feature", "Adds Soulsow to the skills selected below when out of combat. \nWill also add Soulsow to Harpe when in combat and no target is selected.", RPR.JobID)]
    RPR_Soulsow = 12302,

    [ParentCombo(RPR_Soulsow)]
    [CustomComboInfo("Soulsow Reminder during Combat", "Adds Soulsow to Harpe during combat when no target is selected.", RPR.JobID)]
    RPR_Soulsow_Combat = 12309,

    [ReplaceSkill(RPR.ArcaneCircle)]
    [CustomComboInfo("Arcane Circle Harvest Feature", "Replaces Arcane Circle with Plentiful Harvest when you have stacks of Immortal Sacrifice.", RPR.JobID)]
    RPR_ArcaneCirclePlentifulHarvest = 12300,

    [ReplaceSkill(RPR.HellsEgress, RPR.HellsIngress)]
    [CustomComboInfo("Regress Feature", "Changes both Hell's Ingress and Hell's Egress turn into Regress when Threshold is active.", RPR.JobID)]
    RPR_Regress = 12301,

    [ReplaceSkill(RPR.Enshroud)]
    [ConflictingCombos(RPR_EnshroudCommunio)]
    [CustomComboInfo("Enshroud Protection Feature", "Turns Enshroud into Gibbet/Gallows to protect Soul Reaver waste.", RPR.JobID)]
    RPR_EnshroudProtection = 12304,

    [ParentCombo(RPR_EnshroudProtection)]
    [CustomComboInfo("True North Feature", "Adds True North when under Gluttony.", RPR.JobID)]
    RPR_TrueNorthEnshroud = 12308,

    [ReplaceSkill(RPR.Enshroud)]
    [ConflictingCombos(RPR_EnshroudProtection)]
    [CustomComboInfo("Enshroud to Communio to Perfectio Feature", "Turns Enshroud to Communio and Perfectio when available to use.", RPR.JobID)]
    RPR_EnshroudCommunio = 12307,

    [ReplaceSkill(RPR.Gibbet, RPR.Gallows, RPR.Guillotine)]
    [CustomComboInfo("Communio on Gibbet/Gallows and Guillotine Feature", "Adds Communio to Gibbet/Gallows and Guillotine.", RPR.JobID)]
    RPR_CommunioOnGGG = 12305,

    [ParentCombo(RPR_CommunioOnGGG)]
    [CustomComboInfo("Lemure's Slice/Scythe Option", "Adds Lemure's Slice to Gibbet/Gallows and Lemure's Scythe to Guillotine.", RPR.JobID)]
    RPR_LemureOnGGG = 12306,

    // Last value = 12312

    #endregion

    #endregion

    #region RED MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ConflictingCombos(RDM_ST_DPS)]
    [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Jolt3)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Jolt with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.\nTo start the melee combo, you must be within melee range.",
        RDM.JobID)]
    [SimpleCombo]
    RDM_ST_SimpleMode = 13000,

    [AutoAction(true, false)]
    [ReplaceSkill(RDM.Scatter, RDM.Impact)]
    [ConflictingCombos(RDM_AoE_DPS)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Scatter with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.\nTo start the melee combo, you must be within melee range.",
        RDM.JobID)]
    [SimpleCombo]
    RDM_AoE_SimpleMode = 13200,

    #endregion

    #region Single Target DPS

    [AutoAction(false, false)]
    [ReplaceSkill(RDM.Jolt, RDM.Jolt2, RDM.Jolt3)]
    [ConflictingCombos(RDM_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Jolt with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        RDM.JobID)]
    [AdvancedCombo]
    RDM_ST_DPS = 13001,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Opener (Level 100)",
        "Adds the opener at level 100.\n**Must move into melee range before melee combo for standard**", RDM.JobID)]
    RDM_Balance_Opener = 13002,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Verthunder / Veraero Option", "Adds Verthunder & Veraero.", RDM.JobID)]
    RDM_ST_ThunderAero = 13003,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Verfire / Verstone Option", "Adds Verfire & Verstone.", RDM.JobID)]
    RDM_ST_FireStone = 13004,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Verflare / Verholy Option", "Adds Verflare & Verholy when available.", RDM.JobID)]
    RDM_ST_HolyFlare = 13005,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Melee Combo Option",
        "Adds Zwerchhau & Redoublement. \n**Must be in melee range or have Gap close with Corps-a-corps enabled**", RDM.JobID)]
    RDM_ST_MeleeCombo = 13006,

    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo("Include Riposte",
        "Adds Riposte to start the combo. Recommended for Auto Rotation. \n**Must be in melee range or have gap-close with Corps-a-corps enabled**", RDM.JobID)]
    RDM_ST_MeleeCombo_IncludeRiposte = 13007,
    
    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo("Gap-Close with Corps-a-corps Option",
        "Use Corp-a-corps when out of melee range and you have enough mana to start the melee combo or starting Manafication Burst.", RDM.JobID)]
    RDM_ST_MeleeCombo_GapCloser = 13008,
    
    [ParentCombo(RDM_ST_MeleeCombo)]
    [CustomComboInfo("Enforced Melee Check", "Once the melee combo has started, don't switch away even if target is out of range.",
        RDM.JobID)]
    RDM_ST_MeleeCombo_MeleeCheck = 13009,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Embolden Option", "Add Embolden when Available.", RDM.JobID)]
    RDM_ST_Embolden = 13010,
    
    [ParentCombo(RDM_ST_Embolden)]
    [CustomComboInfo("Use Manafication", "Add Manafication before embolden for burst.", RDM.JobID)]
    RDM_ST_Manafication = 13011,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Vice Of Thorns Option", "Add Vice of Thorns when available.", RDM.JobID)]
    RDM_ST_ViceOfThorns = 13012,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Prefulgence Option", "Add Prefulgence when available.", RDM.JobID)]
    RDM_ST_Prefulgence = 13013,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Fleche Option", "Add Fleche when available.", RDM.JobID)]
    RDM_ST_Fleche = 13014,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Contre Sixte Option", "Add Contre Sixte when available.", RDM.JobID)]
    RDM_ST_ContreSixte = 13015,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Engagement Option", "Add Engagement when available.", RDM.JobID)]
    RDM_ST_Engagement = 13016,
    
    [ParentCombo(RDM_ST_Engagement)]
    [CustomComboInfo("Engagement Pooling Option", "Prevents overcap but pools at least one charge for burst window.", RDM.JobID)]
    RDM_ST_Engagement_Pooling = 13018,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Corps-a-corps Option", "Add Corps-a-corps when available for DPS. \nWill retain a charge for gap-closing.", RDM.JobID)]
    RDM_ST_Corpsacorps = 13017,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Acceleration Option", "Add Acceleration when available.", RDM.JobID)]
    RDM_ST_Acceleration = 13019,

    [ParentCombo(RDM_ST_Acceleration)]
    [CustomComboInfo("Acceleration Movement Option", "Uses Acceleration for movement.", RDM.JobID)]
    RDM_ST_Acceleration_Movement = 13020,

    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Swiftcast Option", "Add Swiftcast when available.", RDM.JobID)]
    RDM_ST_Swiftcast = 13021,
    
    [ParentCombo(RDM_ST_Swiftcast)]
    [CustomComboInfo("Swiftcast Option", "Use Swiftcast for movement only.", RDM.JobID)]
    RDM_ST_SwiftcastMovement = 13022,
    
    [ParentCombo(RDM_ST_DPS)]
    [CustomComboInfo("Lucid Dreaming Option", "Weaves Lucid Dreaming when your MP drops below the specified value.",
        RDM.JobID)]
    RDM_ST_Lucid = 13023,

    //Last Used 13024
    #endregion

    #region AoE DPS

    [AutoAction(true, false)]
    [ReplaceSkill(RDM.Scatter, RDM.Impact)]
    [ConflictingCombos(RDM_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Scatter with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        RDM.JobID)]
    [AdvancedCombo]
    RDM_AoE_DPS = 13201,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Thunder/Aero Option", "Adds Verthunder 2 and Veraero 2 into the Aoe Rotation.", RDM.JobID)]
    RDM_AoE_ThunderAero = 13202,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Verflare/Verholy Option", "Adds Verflare and Verholy into the Aoe Rotation.", RDM.JobID)]
    RDM_AoE_HolyFlare = 13203,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Moulinet Melee Combo Option", "Addes Moulinet into the Aoe Rotation. \nWill use ST Melee combo below 52.", RDM.JobID)]
    RDM_AoE_MeleeCombo = 13204,
    
    [ParentCombo(RDM_AoE_MeleeCombo)]
    [CustomComboInfo("Require Target Option", "Requires a target within 8 Yalms. \nSkill itself requires none usually, this is for the range check.", RDM.JobID)]
    RDM_AoE_MeleeCombo_Target = 13205,
    
    [ParentCombo(RDM_AoE_MeleeCombo)]
    [CustomComboInfo("Gap-close with Corps-a-corps Option",
        "Use Corp-a-corps when out of melee range and you have enough mana to start the melee combo.", RDM.JobID)]
    RDM_AoE_MeleeCombo_GapCloser = 13206,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Embolden Option", "Add Embolden when Available.", RDM.JobID)]
    RDM_AoE_Embolden = 13207,
    
    [ParentCombo(RDM_AoE_Embolden)]
    [CustomComboInfo("Use Manafication", "Add Manafication before embolden for burst.", RDM.JobID)]
    RDM_AoE_Manafication = 13208,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Vice Of Thorns Option", "Add Vice of Thorns when available.", RDM.JobID)]
    RDM_AoE_ViceOfThorns = 13209,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Prefulgence Option", "Add Prefulgence when available.", RDM.JobID)]
    RDM_AoE_Prefulgence = 13210,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Fleche Option", "Add Fleche when available.", RDM.JobID)]
    RDM_AoE_Fleche = 13211,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Contre Sixte Option", "Add Contre Sixte when available.", RDM.JobID)]
    RDM_AoE_ContreSixte = 13212,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Engagement Option", "Add Engagement when available.", RDM.JobID)]
    RDM_AoE_Engagement = 13213,
    
    [ParentCombo(RDM_AoE_Engagement)]
    [CustomComboInfo("Engagement Pooling Option", "Prevents overcap but pools at least one charge for burst window.", RDM.JobID)]
    RDM_AoE_Engagement_Pooling = 13215,
    
    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Corps-a-corps Option", "Add Corps-a-corps when available for DPS. \nWill retain a charge for gap-closing.", RDM.JobID)]
    RDM_AoE_Corpsacorps = 13214,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Acceleration Option", "Add Acceleration when available.", RDM.JobID)]
    RDM_AoE_Acceleration = 13216,

    [ParentCombo(RDM_AoE_Acceleration)]
    [CustomComboInfo("Acceleration Movement Option", "Uses Acceleration for movement.", RDM.JobID)]
    RDM_AoE_Acceleration_Movement = 13217,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Swiftcast Option", "Add Swiftcast when available.", RDM.JobID)]
    RDM_AoE_Swiftcast = 13218,
    
    [ParentCombo(RDM_AoE_Swiftcast)]
    [CustomComboInfo("Swiftcast Option", "Use Swiftcast for movement only.", RDM.JobID)]
    RDM_AoE_SwiftcastMovement = 13219,

    [ParentCombo(RDM_AoE_DPS)]
    [CustomComboInfo("Lucid Dreaming Option", "Weaves Lucid Dreaming when your MP drops below the specified value.",
        RDM.JobID)]
    RDM_AoE_Lucid = 13220,
    
    //Last Used 13221
    #endregion

    #region Stand Alone Features

    [ReplaceSkill(RDM.Veraero, RDM.Veraero3)]
    [CustomComboInfo("Spell Combo on Veraero", "Replaces Veraero with Jolt.", RDM.JobID)]
    RDM_VerAero = 13400,

    [ParentCombo(RDM_VerAero)]
    [CustomComboInfo("Add Verstone", "Replaces Veraero with Verstone.", RDM.JobID)]
    RDM_VerAero_Stone = 13401,
    
    [ReplaceSkill(RDM.Verthunder, RDM.Verthunder3)]
    [CustomComboInfo("Spell Combo on Verthunder", "Replaces Verthunder with Jolt.", RDM.JobID)]
    RDM_VerThunder = 13418,

    [ParentCombo(RDM_VerThunder)]
    [CustomComboInfo("Add Verfire", "Replaces Verthunder With Verfire.", RDM.JobID)]
    RDM_VerThunder_Fire = 13419,

    [ReplaceSkill(RDM.Riposte)]
    [CustomComboInfo("Riposte Melee Combo", "Replaces Riposte with the basic melee combo.", RDM.JobID)]
    RDM_Riposte = 13403,
    
    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo("Gap-Close with Corps-a-corps Option",
        "Use Corp-a-corps when out of melee range and you have enough mana or Magicked Swordplay to start the melee combo", RDM.JobID)]
    RDM_Riposte_GapCloser = 13424,
    
    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo("Riposte Finisher Option", "Adds Verholy/Verflare, Scorch, and Resolution", RDM.JobID)]
    RDM_Riposte_Finisher = 13423,
    
    [ParentCombo(RDM_Riposte)]
    [CustomComboInfo("Riposte Waste Prevention Option", "Replaces Riposte with Savage Blade when resources are too low to complete combo", RDM.JobID)]
    RDM_Riposte_NoWaste = 13429,
    
    [ReplaceSkill(RDM.Moulinet)]
    [CustomComboInfo("Moulinet Melee Combo", "Replaces Moulinet with the basic melee aoe combo.", RDM.JobID)]
    RDM_Moulinet= 13425,
    
    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo("Gap-Close with Corps-a-corps Option",
        "Use Corp-a-corps when out of melee range and you have enough mana or Magicked Swordplay to start the melee combo", RDM.JobID)]
    RDM_Moulinet_GapCloser = 13426,
    
    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo("Moulinet Finisher Option", "Adds Verholy/Verflare, Scorch, and Resolution", RDM.JobID)]
    RDM_Moulinet_Finisher = 13427,
    
    [ParentCombo(RDM_Moulinet)]
    [CustomComboInfo("Moulinet Waste Prevention Option", "Replaces Moulinet with Savage Blade when resources are too low to complete combo", RDM.JobID)]
    RDM_Moulinet_NoWaste = 13428,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Caster_Raise)]
    [CustomComboInfo("Verraise Feature",
        "Changes Swiftcast to Verraise when under the effect of Swiftcast or Dualcast.", RDM.JobID)]
    RDM_Raise = 13406,

    [ParentCombo(RDM_Raise)]
    [CustomComboInfo("Vercure Option", "If Swiftcast is on cooldown, change to Vercure to proc Dualcast.", RDM.JobID)]
    RDM_Raise_Vercure = 13407,

    [ParentCombo(RDM_Raise)]
    [CustomComboInfo("Retarget Raise and Vercure", "Will Retarget the Raise and Vercure affected here to your Heal Stack.", RDM.JobID)]
    [Retargeted(RDM.Verraise, RDM.Vercure)]
    RDM_Raise_Retarget = 13408,

    [ReplaceSkill(RDM.Displacement)]
    [CustomComboInfo("Displacement <> Corps-a-corps Feature",
        "Replace Displacement with Corps-a-corps when out of range.", RDM.JobID)]
    RDM_CorpsDisplacement = 13409,
    
    [ReplaceSkill(RDM.Embolden)]
    [CustomComboInfo("Embolden Overlap Protection", "Disables Embolden when buffed by another Red Mage's Embolden by replacing it with Savage Blade.",
        RDM.JobID)]
    RDM_EmboldenProtection = 13412,

    [ParentCombo(RDM_EmboldenProtection)]
    [CustomComboInfo("Embolden to Manafication Option", "Changes Embolden to Manafication when on cooldown or under the effect of anyones Embolden.",
        RDM.JobID)]
    RDM_EmboldenManafication = 13410,

    [ParentCombo(RDM_MagickProtection)]
    [CustomComboInfo("Magick Barrier to Addle Option", "Changes Magick Barrier to Addle when on cooldown. \nWill Display the lowest cooldown skill if both are on cooldown.", RDM.JobID)]
    RDM_MagickBarrierAddle = 13411,

    [ReplaceSkill(RDM.MagickBarrier)]
    [CustomComboInfo("Magick Barrier Overlap Protection",
        "Disables Magick Barrier when buffed by another Red Mage's Magick Barrier by replacing it with Savage Blade. \nApplies to suboption for Addle as well.", RDM.JobID)]
    RDM_MagickProtection = 13413,
    
    [ReplaceSkill(RDM.Fleche)]
    [CustomComboInfo("OGCDs One Button Feature",
        "Replaces Fleche with Contre Sixte, Vice of Thorns, Prefulgence, Engagement, and 1 charge of Corps-a-corps.", RDM.JobID)]
    RDM_OGCDs = 13420,
    
    [ParentCombo(RDM_OGCDs)]
    [CustomComboInfo("Engagement Pooling Option",
        "Will not spend both Engagement Charges unless you have Embolden.", RDM.JobID)]
    RDM_OGCDs_EngagementPool = 13421,
    
    [ParentCombo(RDM_OGCDs)]
    [CustomComboInfo("Corps-a-corps Melee only Option",
        "Needs to be in melee range to use Corps-a-corps.", RDM.JobID)]
    RDM_OGCDs_CorpsMelee = 13422,

    [Variant]
    [VariantParent(RDM_ST_DPS, RDM_ST_SimpleMode, RDM_AoE_DPS, RDM_AoE_SimpleMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown. Replaces Jolts.", RDM.JobID)]
    RDM_Variant_Rampart = 13414,

    [Variant]
    [VariantParent(RDM_Raise)]
    [CustomComboInfo("Raise Option",
        "Turn Swiftcast into Variant Raise whenever you have the Swiftcast or Dualcast buffs.", RDM.JobID)]
    RDM_Variant_Raise = 13415,

    [Variant]
    [VariantParent(RDM_ST_DPS, RDM_ST_SimpleMode, RDM_AoE_DPS, RDM_AoE_SimpleMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold. Replaces Jolts.", RDM.JobID)]
    RDM_Variant_Cure = 13416,

    [Variant]
    [CustomComboInfo("Cure on Vercure Option", "Replaces Vercure with Variant Cure.", RDM.JobID)]
    RDM_Variant_Cure2 = 13417,
    
    //Last Used 13423
    #endregion

    #endregion

    #region SAGE

    #region Single Target DPS Feature

    [AutoAction(false, false)]
    [ReplaceSkill(SGE.Dosis, SGE.Dosis2, SGE.Dosis3)]
    [CustomComboInfo("Advanced DPS Mode - Single Target", "Adds various options to Dosis I/II/III.", SGE.JobID)]
    [AdvancedCombo]
    SGE_ST_DPS = 14001,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Balance Opener (Level 92)", "Use the Balance opener from level 92 onwards.", SGE.JobID)]
    SGE_ST_DPS_Opener = 14055,
    
    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Eukrasian Dosis Option", "Automatic DoT Uptime.", SGE.JobID)]
    SGE_ST_DPS_EDosis = 14003,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Addersgall Overflow Protection", "Weaves Druochole when Addersgall gauge is greater than or equal to the specified value.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Druochole)]
    SGE_ST_DPS_AddersgallProtect = 14054,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Rhizomata Option", "Weaves Rhizomata when Addersgall gauge falls below the specified value.", SGE.JobID)]
    SGE_ST_DPS_Rhizo = 14007,
    
    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Phlegma Option", "Use Phlegma if available and within range.", SGE.JobID)]
    SGE_ST_DPS_Phlegma = 14005,
    
    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Psyche Option", "Weaves Psyche when available.", SGE.JobID)]
    SGE_ST_DPS_Psyche = 14008,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Movement Options", "Use selected instant cast actions while moving.", SGE.JobID)]
    SGE_ST_DPS_Movement = 14004,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Kardia Reminder Option", "Adds Kardia when not under the effect.", SGE.JobID)]
    [Retargeted(SGE.Kardia)]
    SGE_ST_DPS_Kardia = 14006,

    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Soteria Option", "Weaves Soteria if you have the Kardia buff.", SGE.JobID)]
    SGE_ST_DPS_Soteria = 14056,
    
    [ParentCombo(SGE_ST_DPS)]
    [CustomComboInfo("Lucid Dreaming Option", "Weaves Lucid Dreaming when your MP drops below the specified value.", SGE.JobID)]
    SGE_ST_DPS_Lucid = 14002,

    #endregion

    #region AoE DPS Feature

    [AutoAction(true, false)]
    [ReplaceSkill(SGE.Dyskrasia, SGE.Dyskrasia2)]
    [CustomComboInfo("Advanced DPS Mode - AoE", "Adds various options to Dyskrasia I & II. Requires a target.", SGE.JobID)]
    [AdvancedCombo]
    SGE_AoE_DPS = 14009,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Eukrasia Option", "Uses Eukrasia for Eukrasia Dyskrasia.", SGE.JobID)]
    SGE_AoE_DPS_EDyskrasia = 14052,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Addersgall Overflow Protection", "Weaves Druochole when Addersgall gauge is greater than or equal to the specified value.", SGE.JobID)]
    [PossiblyRetargeted]
    SGE_AoE_DPS_AddersgallProtect = 14053,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Rhizomata Option", "Weaves Rhizomata when Addersgall gauge falls below the specified value.", SGE.JobID)]
    SGE_AoE_DPS_Rhizo = 14013,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Phlegma Option", "Uses Phlegma if available.", SGE.JobID)]
    SGE_AoE_DPS_Phlegma = 14010,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Psyche Option", "Weaves Psyche if available.", SGE.JobID)]
    SGE_AoE_DPS_Psyche = 14051,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Toxikon Option", "Use Toxikon if available.", SGE.JobID)]
    SGE_AoE_DPS_Toxikon = 14011,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Pneuma Option", "Adds Pneuma if available.", SGE.JobID)]
    SGE_AoE_DPS_Pneuma = 14059,

    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Soteria Option", "Weaves Soteria if you have the Kardia buff.", SGE.JobID)]
    SGE_AoE_DPS_Soteria = 14057,
    
    [ParentCombo(SGE_AoE_DPS)]
    [CustomComboInfo("Lucid Dreaming Option", "Weaves Lucid Dreaming when your MP falls below the specified value.", SGE.JobID)]
    SGE_AoE_DPS_Lucid = 14012,
    
    #endregion

    #region Single Target Heal

    [AutoAction(false, true)]
    [ReplaceSkill(SGE.Diagnosis)]
    [CustomComboInfo("Advanced Healing Mode - Single Target", "Change Diagnosis into various options.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Diagnosis)]
    [HealingCombo]
    SGE_ST_Heal = 14014,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value:", SGE.JobID)]
    SGE_ST_Heal_Lucid = 14063,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Rhizomata Option", "Adds Rhizomata when Addersgall is 0.", SGE.JobID)]
    SGE_ST_Heal_Rhizomata = 14023,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Apply Kardia Option", "Applies Kardia if it's not applied to anyone.", SGE.JobID)]
    [Retargeted(SGE.Kardia)]
    SGE_ST_Heal_Kardia = 14016,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Esuna Option", "Applies Esuna if there is a cleansable debuff.", SGE.JobID)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    SGE_ST_Heal_Esuna = 14015,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Eukrasian Diagnosis Option", "Diagnosis becomes Eukrasian Diagnosis if the shield is not applied to the target.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Eukrasia)]
    SGE_ST_Heal_EDiagnosis = 14017,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Druochole Option", "Applies Druochole.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Druochole)]
    SGE_ST_Heal_Druochole = 14025,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Taurochole Option", "Adds Taurochole.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Taurochole)]
    SGE_ST_Heal_Taurochole = 14021,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Haima Option", "Applies Haima.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Haima)]
    SGE_ST_Heal_Haima = 14022,
    
    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Soteria Option", "Applies Soteria.", SGE.JobID)]
    SGE_ST_Heal_Soteria = 14018,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Zoe Option", "Applies Zoe.", SGE.JobID)]
    SGE_ST_Heal_Zoe = 14019,
    
    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Krasis Option", "Applies Krasis.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Krasis)]
    SGE_ST_Heal_Krasis = 14024,

    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Pepsis Option", "Triggers Pepsis if a shield is present.", SGE.JobID)]
    SGE_ST_Heal_Pepsis = 14020,
    
    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Physis Option", "Adds Physis.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Physis)]
    SGE_ST_Heal_Physis = 14065,
    
    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Kerachole Option", "Adds Kerachole.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Kerachole)]
    SGE_ST_Heal_Kerachole = 14066,
    
    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Holos Option", "Adds Holos.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Holos)]
    SGE_ST_Heal_Holos = 14067,
    
    [ParentCombo(SGE_ST_Heal)]
    [CustomComboInfo("Panhaima Option", "Adds Panhaima.", SGE.JobID)]
    [PossiblyRetargeted(SGE.Panhaima)]
    SGE_ST_Heal_Panhaima = 14068,

    #endregion

    #region AoE Heal

    [AutoAction(true, true)]
    [ReplaceSkill(SGE.Prognosis)]
    [CustomComboInfo("Advanced Healing Mode - AoE", "Change Prognosis into various options.", SGE.JobID)]
    [HealingCombo]
    SGE_AoE_Heal = 14026,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming when MP drops below slider value:", SGE.JobID)]
    SGE_AoE_Heal_Lucid = 14064,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Rhizomata Option", "Adds Rhizomata when Addersgall is 0.", SGE.JobID)]
    SGE_AoE_Heal_Rhizomata = 14036,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Eukrasian Prognosis Option", "Adds Eukrasian Prognosis.", SGE.JobID)]
    SGE_AoE_Heal_EPrognosis = 14028,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Ixochole Option", "Adds Ixochole.", SGE.JobID)]
    SGE_AoE_Heal_Ixochole = 14033,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Kerachole Option", "Adds Kerachole.", SGE.JobID)]
    SGE_AoE_Heal_Kerachole = 14035,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Holos Option", "Adds Holos.", SGE.JobID)]
    SGE_AoE_Heal_Holos = 14030,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Physis Option", "Adds Physis.", SGE.JobID)]
    SGE_AoE_Heal_Physis = 14027,
    
    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Panhaima Option", "Adds Panhaima.", SGE.JobID)]
    SGE_AoE_Heal_Panhaima = 14031,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Zoe Option", "Adds Zoe.", SGE.JobID)]
    SGE_AoE_Heal_Zoe = 14058,

    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Philosophia Option", "Adds Philosophia.", SGE.JobID)]
    SGE_AoE_Heal_Philosophia = 14050,
    
    [ParentCombo(SGE_AoE_Heal)]
    [CustomComboInfo("Pepsis Option", "Triggers Pepsis if a shield is present.", SGE.JobID)]
    SGE_AoE_Heal_Pepsis = 14032,
    
    #endregion
    
    #region Overprotect

    [ReplaceSkill(SGE.Kerachole)]
    [CustomComboInfo("Spell Overlap Protection", "Prevents you from wasting actions if under the effect of someone else's actions", SGE.JobID)]
    SGE_OverProtect = 14043,

    [ParentCombo(SGE_OverProtect)]
    [CustomComboInfo("Under Kerachole", "Don't use Kerachole when under the effect of someone's Kerachole", SGE.JobID)]
    SGE_OverProtect_Kerachole = 14044,

    [ParentCombo(SGE_OverProtect_Kerachole)]
    [CustomComboInfo("Under Sacred Soil", "Don't use Kerachole when under the effect of someone's Sacred Soil", SGE.JobID)]
    SGE_OverProtect_SacredSoil = 14045,

    [ParentCombo(SGE_OverProtect)]
    [CustomComboInfo("Under Panhaima", "Don't use Panhaima when under the effect of someone's Panhaima", SGE.JobID)]
    SGE_OverProtect_Panhaima = 14046,

    [ParentCombo(SGE_OverProtect)]
    [CustomComboInfo("Under Philosophia", "Don't use Philosophia when under the effect of someone's Philosophia", SGE.JobID)]
    SGE_OverProtect_Philosophia = 14047,
    
    #endregion

    #region Misc Healing

    [ReplaceSkill(SGE.Taurochole, SGE.Druochole, SGE.Ixochole, SGE.Kerachole)]
    [CustomComboInfo("Rhizomata Feature", "Replaces Addersgall skills with Rhizomata when empty.", SGE.JobID)]
    SGE_Rhizo = 14037,

    [ConflictingCombos(SGE_Retarget_Taurochole)]
    [ReplaceSkill(SGE.Taurochole)]
    [CustomComboInfo("Taurochole to Druochole Feature", "Turns Taurochole to Druochole when Taurochole is on cooldown.", SGE.JobID)]
    [PossiblyRetargeted]
    SGE_TauroDruo = 14038,

    [ReplaceSkill(SGE.Pneuma)]
    [CustomComboInfo("Zoe Pneuma Feature", "Places Zoe on top of Pneuma when both actions are on cooldown.", SGE.JobID)]
    SGE_ZoePneuma = 14039,

    #endregion

    #region Utility

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo("Swiftcast Raise Feature", "Changes Swiftcast to Egeiro while Swiftcast is on cooldown.", SGE.JobID)]
    SGE_Raise = 14040,

    [ParentCombo(SGE_Raise)]
    [CustomComboInfo("Retarget Raise", "Will Retarget the Raise affected here to your Heal Stack.", SGE.JobID)]
    [Retargeted(SGE.Egeiro)]
    SGE_Raise_Retarget = 14061,

    [ReplaceSkill(SGE.Soteria)]
    [CustomComboInfo("Soteria to Kardia Feature", "Soteria turns into Kardia when not active or Soteria is on-cooldown.", SGE.JobID)]
    [Retargeted]
    SGE_Kardia = 14041,

    [ReplaceSkill(SGE.Eukrasia)]
    [CustomComboInfo("Eukrasia Feature", "Eukrasia turns into the selected Eukrasian-type action when active.", SGE.JobID)]
    [PossiblyRetargeted]
    SGE_Eukrasia = 14042,
    
    [Variant]
    [VariantParent(SGE_ST_DPS_EDosis, SGE_AoE_DPS)]
    [CustomComboInfo("Spirit Dart Option", "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", SGE.JobID)]
    SGE_DPS_Variant_SpiritDart = 14048,

    [Variant]
    [VariantParent(SGE_ST_DPS, SGE_AoE_DPS)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", SGE.JobID)]
    SGE_DPS_Variant_Rampart = 14049,

    #endregion
    
    #region Standalone Healing option

    [CustomComboInfo("Retarget Options", "Retargets Single Target Healing options.", SGE.JobID)]
    [Retargeted]
    SGE_Retarget = 14073,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo("Haima Options", "Retargets Haima according to your Healing stack.", SGE.JobID)]
    [Retargeted(SGE.Haima)]
    SGE_Retarget_Haima = 14074,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo("Druochole Options", "Retargets Druochole according to your Healing stack.", SGE.JobID)]
    [Retargeted(SGE.Druochole)]
    SGE_Retarget_Druochole = 14075,
    
    [ConflictingCombos(SGE_TauroDruo)]
    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo("Taurochole Options", "Retargets Taurochole according to your Healing stack.", SGE.JobID)]
    [Retargeted(SGE.Taurochole)]
    SGE_Retarget_Taurochole = 14076,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo("Krasis Options", "Retargets Krasis according to your Healing stack.", SGE.JobID)]
    [Retargeted(SGE.Krasis)]
    SGE_Retarget_Krasis = 14077,

    [ParentCombo(SGE_Retarget)]
    [CustomComboInfo("Kardia Options", "Retargets Kardia according to your Healing stack.", SGE.JobID)]
    [Retargeted(SGE.Kardia)]
    SGE_Retarget_Kardia = 14078,
    
    #endregion
    
    #region Hidden Features
    [CustomComboInfo("Hidden Options", "Collection of cheeky or encounter-specific extra options only available to those in the know.\nDo not expect these options to be maintained, or even kept, after they are no longer Current.", SGE.JobID)]
    [Hidden]
    SGE_Hidden = 14069,
    
    [ParentCombo(SGE_Hidden)]
    [CustomComboInfo("Eukrasian Prognosis Option", "Will try to cast Shields when a raidwide casting is detected if shieldcheck from Eukrasian Prognosis setting passes. \nWill be used in all 4 main combos.", SGE.JobID)]
    [Hidden]
    SGE_Hidden_EPrognosis = 14070,
    
    [ParentCombo(SGE_Hidden)]
    [CustomComboInfo("Kerachole Option", "Will try to cast Kerachole when a raidwide casting is detected. \nWill be used in all 4 main combos.", SGE.JobID)]
    [Hidden]
    SGE_Hidden_Kerachole = 14071,
    
    [ParentCombo(SGE_Hidden)]
    [CustomComboInfo("Holos Option", "Will try to cast Holos when a raidwide casting is detected. \nWill be used in all 4 main combos.", SGE.JobID)]
    [Hidden]
    SGE_Hidden_Holos = 14072,
    
    #endregion

    // Last used number = 14078

    #endregion

    #region SAMURAI
    
    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(SAM.Hakaze, SAM.Gyofu)]
    [ConflictingCombos(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Hakaze with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", SAM.JobID)]
    [SimpleCombo]
    SAM_ST_SimpleMode = 15002,

    [AutoAction(true, false)]
    [ReplaceSkill(SAM.Fuga, SAM.Fuko)]
    [ConflictingCombos(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Fuga with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", SAM.JobID)]
    [SimpleCombo]
    SAM_AoE_SimpleMode = 15102,

    #endregion

    #region Advanced ST

    [AutoAction(false, false)]
    [ReplaceSkill(SAM.Hakaze, SAM.Gyofu)]
    [ConflictingCombos(SAM_ST_SimpleMode)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Hakaze with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", SAM.JobID)]
    [AdvancedCombo]
    SAM_ST_AdvancedMode = 15003,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", SAM.JobID)]
    SAM_ST_Opener = 15006,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Yukikaze Combo", "Adds Yukikaze combo to the rotation.", SAM.JobID)]
    SAM_ST_Yukikaze = 15004,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Kasha Combo", "Adds Kasha combo to the rotation.", SAM.JobID)]
    SAM_ST_Kasha = 15005,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Gekko Combo", "Adds Gekko combo to the rotation.", SAM.JobID)]
    SAM_ST_Gekko = 15022,
    
    #region cooldowns on Main Combo

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Cooldowns on Main Combo", "Collection of Cooldown features on main combo.", SAM.JobID)]
    SAM_ST_CDs = 15011,

    [ParentCombo(SAM_ST_CDs)]
    [CustomComboInfo("Meikyo Shisui Option", "Adds Meikyo Shisui to the rotation.\n Dynamically changes usage for 2.14 or 2.08 GCD", SAM.JobID)]
    SAM_ST_CDs_MeikyoShisui = 15018,

    [ParentCombo(SAM_ST_CDs)]
    [CustomComboInfo("Ikishoten Option", "Adds Ikishoten when at or below 50 Kenki.\nWill dump Kenki at 10 seconds left to allow Ikishoten to be used.", SAM.JobID)]
    SAM_ST_CDs_Ikishoten = 15012,

    #endregion
    
    #region Damage skills

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Damage skills on Main Combo", "Collection of Damage skills on main combo.", SAM.JobID)]
    SAM_ST_Damage = 15023,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo("Iaijutsu Option", "Adds Midare: Setsugekka, Higanbana, and Kaeshi: Setsugekka to the rotation.", SAM.JobID)]
    SAM_ST_CDs_Iaijutsu = 15013,

    [ParentCombo(SAM_ST_CDs_Iaijutsu)]
    [CustomComboInfo("Iajutsu movement Option", "Adds Midare: Setsugekka and Higanbana when you're not moving.", SAM.JobID)]
    SAM_ST_CDs_Iaijutsu_Movement = 15014,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo("Senei Option", "Adds Senei to the rotation.", SAM.JobID)]
    SAM_ST_CDs_Senei = 15020,
    
    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo("Ogi Namikiri Option", "Adds Ogi Namikiri and Kaeshi: Namikiri to the rotation.", SAM.JobID)]
    SAM_ST_CDs_OgiNamikiri = 15015,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo("Zanshin Option", "Adds Zanshin when ready to the rotation.", SAM.JobID)]
    SAM_ST_CDs_Zanshin = 15017,

    [ParentCombo(SAM_ST_Damage)]
    [CustomComboInfo("Shoha Option", "Adds Shoha when you have three meditation stacks.", SAM.JobID)]
    SAM_ST_CDs_Shoha = 15019,
    
    #endregion
    
    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Shinten Option", "Adds Shinten to the rotation", SAM.JobID)]
    SAM_ST_Shinten = 15008,
    
    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("True North Feature", "Adds True North when you are not in the correct position for the enhanced potency bonus.", SAM.JobID)]
    SAM_ST_TrueNorth = 15099,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Ranged Uptime Feature", "Adds Enpi to the rotation when you are out of range.", SAM.JobID)]
    SAM_ST_RangedUptime = 15097,

    [ParentCombo(SAM_ST_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the combo, using them when below the HP Percentage threshold.", SAM.JobID)]
    SAM_ST_ComboHeals = 15098,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(SAM.Fuga, SAM.Fuko)]
    [ConflictingCombos(SAM_AoE_SimpleMode)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Fuga with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", SAM.JobID)]
    [AdvancedCombo]
    SAM_AoE_AdvancedMode = 15103,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Oka Combo", "Adds Oka combo to the rotation.", SAM.JobID)]
    SAM_AoE_Oka = 15104,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Hagakure Option", "Adds Hagakure to the rotation when there are three Sen.", SAM.JobID)]
    SAM_AoE_Hagakure = 15113,

    #region Cooldowns on Main Combo
    
    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Cooldowns on Main Combo", "Collection of Cooldown features on main combo.", SAM.JobID)]
    SAM_AoE_CDs = 15115,

    [ParentCombo(SAM_AoE_CDs)]
    [CustomComboInfo("Meikyo Shisui Option", "Adds Meikyo Shisui to the rotation.", SAM.JobID)]
    SAM_AoE_MeikyoShisui = 15114,

    [ParentCombo(SAM_AoE_CDs)]
    [CustomComboInfo("Ikishoten Option", "Adds Ikishoten when at or below 50 Kenki.\nWill dump Kenki at 10 seconds left to allow Ikishoten to be used.", SAM.JobID)]
    SAM_AOE_CDs_Ikishoten = 15108,
    
    #endregion

    #region Damage Skills

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Damage skills on Main Combo", "Collection of Damage skills on main combo.", SAM.JobID)]
    SAM_AoE_Damage = 15116,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo("Iaijutsu Option", "Adds Tenka Goken, Midare: Setsugekka, and Kaeshi: Goken when ready and when you're not moving to the rotation.", SAM.JobID)]
    SAM_AoE_TenkaGoken = 15107,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo("Guren Option", "Adds Guren to the rotation.", SAM.JobID)]
    SAM_AoE_Guren = 15112,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo("Ogi Namikiri Option", "Adds Ogi Namikiri and Kaeshi: Namikiri when ready and when you're not moving to the rotation.", SAM.JobID)]
    SAM_AoE_OgiNamikiri = 15109,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo("Zanshin Option", "Adds Zanshin to the rotation.", SAM.JobID)]
    SAM_AoE_Zanshin = 15110,

    [ParentCombo(SAM_AoE_Damage)]
    [CustomComboInfo("Shoha Option", "Adds Shoha when you have 3 meditation stacks.", SAM.JobID)]
    SAM_AoE_Shoha = 15111,
    
    #endregion
    
    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Kyuten Option", "Adds Kyuten to the rotation.", SAM.JobID)]
    SAM_AoE_Kyuten = 15105,

    [ParentCombo(SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the combo, using them when below the HP Percentage threshold.", SAM.JobID)]
    SAM_AoE_ComboHeals = 15199,

    #endregion
    
    #region Basic Combo

    [ReplaceSkill(SAM.Yukikaze)]
    [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain.", SAM.JobID)]
    SAM_ST_YukikazeCombo = 15000,

    [ReplaceSkill(SAM.Kasha)]
    [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain.", SAM.JobID)]
    SAM_ST_KashaCombo = 15001,

    [ReplaceSkill(SAM.Gekko)]
    [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain.", SAM.JobID)]
    SAM_ST_GekkoCombo = 15010,

    [ReplaceSkill(SAM.Oka)]
    [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain.", SAM.JobID)]
    SAM_AoE_OkaCombo = 15100,

    [ReplaceSkill(SAM.Mangetsu)]
    [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain.", SAM.JobID)]
    SAM_AoE_MangetsuCombo = 15101,

    #endregion
    
    #region Meikyo Features

    [ReplaceSkill(SAM.MeikyoShisui)]
    [ConflictingCombos(SAM_MeikyoShisuiProtection)]
    [CustomComboInfo("Sens Feature", "Replace Meikyo Shisui with Gekko, Kasha, and Yukikaze depending on what is needed.", SAM.JobID)]
    SAM_MeikyoSens = 15200,

    [ReplaceSkill(SAM.MeikyoShisui)]
    [ConflictingCombos(SAM_MeikyoSens)]
    [CustomComboInfo("Meikyo Shisui Protection", "Replaces Meikyo Shisui with Savage Blade when you already have Meikyo Shisui active.", SAM.JobID)]
    SAM_MeikyoShisuiProtection = 15214,

    #endregion

    #region Iaijutsu Features

    [ReplaceSkill(SAM.Iaijutsu)]
    [CustomComboInfo("Iaijutsu Features", "Collection of Iaijutsu Features.", SAM.JobID)]
    SAM_Iaijutsu = 15201,

    [ParentCombo(SAM_Iaijutsu)]
    [CustomComboInfo("Iaijutsu to Tsubame-Gaeshi", "Replace Iaijutsu with Tsubame-gaeshi when appropriate.", SAM.JobID)]
    SAM_Iaijutsu_TsubameGaeshi = 15202,

    [ParentCombo(SAM_Iaijutsu)]
    [CustomComboInfo("Iaijutsu to Shoha", "Replace Iaijutsu with Shoha when meditation is 3.", SAM.JobID)]
    SAM_Iaijutsu_Shoha = 15203,

    [ParentCombo(SAM_Iaijutsu)]
    [CustomComboInfo("Iaijutsu to Ogi Namikiri", "Replace Iaijutsu with Ogi Namikiri and Kaeshi: Namikiri when buffed with Ogi Namikiri Ready.", SAM.JobID)]
    SAM_Iaijutsu_OgiNamikiri = 15204,

    #endregion

    #region Shinten Features

    [ReplaceSkill(SAM.Shinten)]
    [CustomComboInfo("Shinten Features", "Collection of Hissatsu: Shinten Features.", SAM.JobID)]
    SAM_Shinten = 15251,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo("Shinten to Shoha", "Replace Hissatsu: Shinten with Shoha when Meditation is full.", SAM.JobID)]
    SAM_Shinten_Shoha = 15205,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo("Shinten to Senei", "Replace Hissatsu: Shinten with Senei when its cooldown is up.", SAM.JobID)]
    SAM_Shinten_Senei = 15206,

    [ParentCombo(SAM_Shinten)]
    [CustomComboInfo("Shinten to Zanshin", "Replace Hissatsu: Shinten with Zanshin when usable.", SAM.JobID)]
    SAM_Shinten_Zanshin = 15207,

    #endregion

    #region Kyuten Features

    [ReplaceSkill(SAM.Kyuten)]
    [CustomComboInfo("Kyuten Features", "Collection of Hissatsu: Kyuten Features.", SAM.JobID)]
    SAM_Kyuten = 15252,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo("Kyuten to Shoha", "Replace Hissatsu: Kyuten with Shoha when Meditation is full.", SAM.JobID)]
    SAM_Kyuten_Shoha = 15208,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo("Kyuten to Guren", "Replace Hissatsu: Kyuten with Guren when its cooldown is up.", SAM.JobID)]
    SAM_Kyuten_Guren = 15209,

    [ParentCombo(SAM_Kyuten)]
    [CustomComboInfo("Kyuten to Zanshin", "Replace Hissatsu: Kyuten with Zanshin when usable.", SAM.JobID)]
    SAM_Kyuten_Zanshin = 15210,

    #endregion

    #region Ikishoten Features

    [ReplaceSkill(SAM.Ikishoten)]
    [CustomComboInfo("Ikishoten Features", "Collection of Ikishoten Features.", SAM.JobID)]
    SAM_Ikishoten = 15253,

    [ParentCombo(SAM_Ikishoten)]
    [CustomComboInfo("Ikishoten to Namikiri", "Replace Ikishoten with Ogi Namikiri & Kaeshi Namikiri when available.", SAM.JobID)]
    SAM_Ikishoten_Namikiri = 15212,

    [ParentCombo(SAM_Ikishoten)]
    [CustomComboInfo("Ikishoten to Shoha", "Replace Ikishoten with Shoha when Meditation is full before Ogi Namikiri.", SAM.JobID)]
    SAM_Ikishoten_Shoha = 15213,

    #endregion

    #region variant

    [Variant]
    [VariantParent(SAM_ST_AdvancedMode, SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", SAM.JobID)]
    SAM_Variant_Cure = 15254,

    [Variant]
    [VariantParent(SAM_ST_AdvancedMode, SAM_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", SAM.JobID)]
    SAM_Variant_Rampart = 15255,

    #endregion

    #region Other

    [ReplaceSkill(SAM.Gyoten)]
    [CustomComboInfo("Gyoten Feature", "Hissatsu: Gyoten becomes Yaten/Gyoten depending on the distance from your target.", SAM.JobID)]
    SAM_GyotenYaten = 15211,

    [ReplaceSkill(SAM.Senei)]
    [CustomComboInfo("Senei - Guren Feature", "Hissatsu: Senei becomes Hissatsu : Guren when synched below lvl 72.", SAM.JobID)]
    SAM_SeneiGuren = 15215,

    #endregion

    #region Hidden Features

    [CustomComboInfo("Hidden Options", "Collection of cheeky or encounter-specific extra options only available to those in the know.\nDo not expect these options to be maintained, or even kept, after they are no longer Current.", SAM.JobID)]
    [Hidden]
    SAM_Hidden = 15300,

    [ParentCombo(SAM_Hidden)]
    [CustomComboInfo("M6S: Hold Burst on Squirrels", "When you're targeting Squirrels in M6S add phase, hold burst.\n(until about the time the first manta is dying)", SAM.JobID)]
    [Hidden]
    SAM_Hid_M6SHoldSquirrelBurst = 15301,

    #endregion

    // Last Value ST = 15023
    // Last Value AoE = 15113
    // Last Value Misc = 15255
    // Last Value Hidden = 15301

    #endregion

    #region SCHOLAR

    #region Simples
    
    [AutoAction(false, false)]
    [ReplaceSkill(SCH.Ruin, SCH.Broil, SCH.Broil2, SCH.Broil3, SCH.Broil4)]
    [SimpleCombo]
    [ConflictingCombos(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Simple DPS Mode - Single Target", "Replaces Ruin I / Broils with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", SCH.JobID)]
    SCH_ST_Simple_DPS = 16070,
    
    
    [AutoAction(true, false)]
    [ReplaceSkill(SCH.ArtOfWar, SCH.ArtOfWarII)]
    [SimpleCombo]
    [ConflictingCombos(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Simple DPS Mode - AoE", "Replaces Art of War with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", SCH.JobID)]
    SCH_AoE_Simple_DPS = 16071,
    
    #endregion
    
    #region ST DPS
    [AutoAction(false, false)]
    [ReplaceSkill(SCH.Ruin, SCH.Broil, SCH.Broil2, SCH.Broil3, SCH.Broil4, SCH.Bio, SCH.Bio2, SCH.Biolysis)]
    [CustomComboInfo("Advanced DPS Mode - Single Target", "Replaces Ruin I / Broils with options below.", SCH.JobID)]
    [AdvancedCombo]
    [ConflictingCombos(SCH_ST_Simple_DPS)]  
    SCH_ST_ADV_DPS = 16001,


    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", SCH.JobID)]
    SCH_ST_ADV_DPS_Balance_Opener = 16009,
    
    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Bio / Biolysis Option", "Automatic DoT uptime.", SCH.JobID)]
    SCH_ST_ADV_DPS_Bio = 16008,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Aetherflow Weave Option", "Use Aetherflow when out of Aetherflow stacks.", SCH.JobID)]
    SCH_ST_ADV_DPS_Aetherflow = 16004,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Energy Drain Weave Option",
        "Use Energy Drain to consume remaining Aetherflow stacks when Aetherflow is about to come off cooldown.",
        SCH.JobID)]
    SCH_ST_ADV_DPS_EnergyDrain = 16005,
    
    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Chain Stratagem",
        "Adds Chain Stratagem on cooldown with overlap protection", SCH.JobID)]
    SCH_ST_ADV_DPS_ChainStrat = 16003,
    
    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Baneful Impact",
        "Adds Baneful Impact when available.", SCH.JobID)]
    SCH_ST_ADV_DPS_BanefulImpact = 16052,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Ruin II Moving Option", "Use Ruin II when you have to move.", SCH.JobID)]
    SCH_ST_ADV_DPS_Ruin2Movement = 16007,
    
    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Fairy Reminder", "Adds Summon Eos whenever you've not summoned your fairy.", SCH.JobID)]
    SCH_ST_ADV_DPS_FairyReminder = 16048,

    [ParentCombo(SCH_ST_ADV_DPS)]
    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value:", SCH.JobID)]
    SCH_ST_ADV_DPS_Lucid = 16002,
    
    #endregion
    
    #region AoE DPS
    [AutoAction(true, false)]
    [ReplaceSkill(SCH.ArtOfWar, SCH.ArtOfWarII)]
    [ConflictingCombos(SCH_AoE_Simple_DPS)]
    [CustomComboInfo("Advanced DPS Mode - AoE", "Replaces Art of War with options below.", SCH.JobID)]
    [AdvancedCombo]
    SCH_AoE_ADV_DPS = 16010,
    
    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Multitarget Dot Option", "Maintains dots on multiple targets.", SCH.JobID)]
    SCH_AoE_ADV_DPS_DoT = 16072,
    
    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Energy Drain Weave Option",
        "Use Energy Drain to consume remaining Aetherflow stacks when Aetherflow is about to come off cooldown.",
        SCH.JobID)]
    SCH_AoE_ADV_DPS_EnergyDrain = 16056,
    
    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Chain Stratagem",
        "Adds Chain Stratagem on cooldown with overlap protection", SCH.JobID)]
    SCH_AoE_ADV_DPS_ChainStrat = 16054,
    
    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Baneful Impact",
        "Adds Baneful Impact when available.", SCH.JobID)]
    SCH_AoE_ADV_DPS_BanefulImpact = 16053,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Fairy Reminder", "Adds Summon Eos whenever you've not summoned your fairy.", SCH.JobID)]
    SCH_AoE_ADV_DPS_FairyReminder = 16049,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value:", SCH.JobID)]
    SCH_AoE_ADV_DPS_Lucid = 16011,

    [ParentCombo(SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Aetherflow Weave Option", "Use Aetherflow when out of Aetherflow stacks.", SCH.JobID)]
    SCH_AoE_ADV_DPS_Aetherflow = 16012,

    #endregion

    #region  ST Healing
    [AutoAction(false, true)]
    [ReplaceSkill(SCH.Physick)]
    [CustomComboInfo("Advanced Healing Mode - Single Target",
        "Change Physick based on the below options::", SCH.JobID)]
    [PossiblyRetargeted(SCH.Physick)]
    [HealingCombo]
    SCH_ST_Heal = 16023,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Lucid Dreaming Weave Option", "Adds Lucid Dreaming when MP drops below slider value:", SCH.JobID)]
    SCH_ST_Heal_Lucid = 16024,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Aetherflow Weave Option", "Use Aetherflow when out of Aetherflow stacks.", SCH.JobID)]
    SCH_ST_Heal_Aetherflow = 16025,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Disspation Option", "Use Dissipation when out of Aetherflow stacks.", SCH.JobID)]
    SCH_ST_Heal_Dissipation = 16040,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Esuna Option", "Applies Esuna to your target if there is a cleansable debuff.", SCH.JobID)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    SCH_ST_Heal_Esuna = 16026,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Adloquium Option", "Use Adloquium", SCH.JobID)]
    [PossiblyRetargeted(SCH.Adloquium)]
    SCH_ST_Heal_Adloquium = 16027,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Lustrate Option", "Use Lustrate", SCH.JobID)]
    [PossiblyRetargeted(SCH.Lustrate)]
    SCH_ST_Heal_Lustrate = 16028,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Excogitation Option", "Use Excogitation", SCH.JobID)]
    [PossiblyRetargeted(SCH.Excogitation)]
    SCH_ST_Heal_Excogitation = 16038,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Protraction Option", "Use Protraction", SCH.JobID)]
    [PossiblyRetargeted(SCH.Protraction)]
    SCH_ST_Heal_Protraction = 16039,

    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Aetherpact Option", "Use Aetherpact", SCH.JobID)]
    [PossiblyRetargeted(SCH.Aetherpact)]
    SCH_ST_Heal_Aetherpact = 16047,
    
    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Whispering Dawn Option", "Use Whispering Dawn", SCH.JobID)]
    SCH_ST_Heal_WhisperingDawn = 16067,
    
    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Fey Illumination Option", "Use Fey Illumination", SCH.JobID)]
    SCH_ST_Heal_FeyIllumination = 16068,
    
    [ParentCombo(SCH_ST_Heal)]
    [CustomComboInfo("Fey Blessing Option", "Use Fey Blessing", SCH.JobID)]
    SCH_ST_Heal_FeyBlessing = 16069,

    [AutoAction(true, true)]
    [ReplaceSkill(SCH.Succor)]
    [CustomComboInfo("Advanced Healing Mode - AoE", "Replaces Succor with options below:", SCH.JobID)]
    [HealingCombo]
    SCH_AoE_Heal = 16018,
    
    #endregion
    
    #region AoE Healing
    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Indomitability Option", "Use Indomitabilty", SCH.JobID)]
    SCH_AoE_Heal_Indomitability = 16022,
    
    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Whispering Dawn Option", "Use Whispering Dawn", SCH.JobID)]
    SCH_AoE_Heal_WhisperingDawn = 16043,
    
    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Fey Illumination Option", "Use Fey Illumination", SCH.JobID)]
    SCH_AoE_Heal_FeyIllumination = 16042,
    
    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Fey Blessing Option", "Use Fey Blessing", SCH.JobID)]
    SCH_AoE_Heal_FeyBlessing = 16045,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Seraphism Option", "Use Seraphism", SCH.JobID)]
    SCH_AoE_Heal_Seraphism = 16044,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Summon Seraph Option", "Use Summon Seraph", SCH.JobID)]
    SCH_AoE_Heal_SummonSeraph = 16063,
    
    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Consolation Option", "Use Consolation", SCH.JobID)]
    SCH_AoE_Heal_Consolation = 16046,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Aetherflow Option", "Use Aetherflow when out of Aetherflow stacks.", SCH.JobID)]
    SCH_AoE_Heal_Aetherflow = 16020,

    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Disspation Option", "Use Dissipation when out of Aetherflow stacks.", SCH.JobID)]
    SCH_AoE_Heal_Dissipation = 16041,
    
    [ParentCombo(SCH_AoE_Heal)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming when MP isn't high enough to cast Succor.",
        SCH.JobID)]
    SCH_AoE_Heal_Lucid = 16019,
    
    #endregion

    #region Utilities
    
    [ReplaceSkill(SCH.EnergyDrain, SCH.Lustrate, SCH.SacredSoil, SCH.Indomitability, SCH.Excogitation)]
    [CustomComboInfo("Aetherflow Helper Feature",
        "Change Aetherflow-using skills to Aetherflow, Recitation, or Dissipation as selected.", SCH.JobID)]
    SCH_Aetherflow = 16029,
    
    [ParentCombo(SCH_Aetherflow)]
    [CustomComboInfo("Dissipation Option", "If Aetherflow is on cooldown, show Dissipation instead.", SCH.JobID)]
    SCH_Aetherflow_Dissipation = 16031,
    
    [ParentCombo(SCH_Aetherflow)]
    [CustomComboInfo("Recitation Option", "Prioritizes Recitation usage on Excogitation or Indomitability.", SCH.JobID)]
    SCH_Aetherflow_Recite = 16030,
    
    [ReplaceSkill(SCH.Lustrate)]
    [CustomComboInfo("Lustrate to Excogitation Feature",
        "Change Lustrate into Excogitation when Excogitation is ready.", SCH.JobID)]
    SCH_Lustrate = 16014,
    
    [ReplaceSkill(SCH.SacredSoil)]
    [CustomComboInfo("Sacred Soil Retargetting", "Adds Self retargetting to Sacred Soil", SCH.JobID)]
    [Retargeted(SCH.SacredSoil)]
    SCH_SacredSoil = 16066,
    
    [ParentCombo(SCH_SacredSoil)]
    [CustomComboInfo("Ally Placement Option", "Will add any ally UI MouseOver target, focus target, soft target, or hard target as the priority Retarget for Sacred Soil.\nBeneath the Enemy placement option, but above yourself.", SCH.JobID)]
    [Retargeted]
    SCH_SacredSoil_Allies = 16061,
    
    [ParentCombo(SCH_SacredSoil)]
    [CustomComboInfo("Enemy Placement Option", "Will add an enemy hard target as the top priority Retarget for Sacred Soil", SCH.JobID)]
    [Retargeted]
    SCH_SacredSoil_Enemy = 16060,
    
    [ReplaceSkill(SCH.Recitation)]
    [CustomComboInfo("Recitation Combo Feature",
        "Change Recitation into either Adloquium, Succor, Indomitability, or Excogitation when used.", SCH.JobID)]
    SCH_Recitation = 16015,
    
    [ReplaceSkill(SCH.DeploymentTactics)]
    [CustomComboInfo("Deployment Tactics Feature",
        "Changes Deployment Tactics to Adloquium until a party member has the Galvanize buff.", SCH.JobID)]
    SCH_DeploymentTactics = 16034,

    [ParentCombo(SCH_DeploymentTactics)]
    [CustomComboInfo("Recitation Option",
        "Adds Recitation when off cooldown to force a critical Galvanize buff on a party member.", SCH.JobID)]
    SCH_DeploymentTactics_Recitation = 16035,
    
    [ReplaceSkill(SCH.WhisperingDawn, SCH.FeyIllumination, SCH.FeyBlessing, SCH.Aetherpact, SCH.Dissipation,
        SCH.SummonSeraph)]
    [CustomComboInfo("Fairy Feature", "Change all fairy actions into Summon Eos when the Fairy is not summoned.",
        SCH.JobID)]
    SCH_FairyReminder = 16033,
    
    [ReplaceSkill(SCH.FeyBlessing)]
    [CustomComboInfo("Fey Blessing to Seraph's Consolation Feature",
        "Change Fey Blessing into Consolation when Seraph is out.", SCH.JobID)]
    SCH_Consolation = 16013,

    [ReplaceSkill(SCH.WhisperingDawn)]
    [CustomComboInfo("Fairy Healing Combo Feature",
        "Change Whispering Dawn into Fey Illumination, Fey Blessing, then Whispering Dawn when used.", SCH.JobID)]
    SCH_Fairy_Combo = 16016,

    [ParentCombo(SCH_Fairy_Combo)]
    [CustomComboInfo("Consolation During Seraph Option", "Adds Consolation during Seraph.", SCH.JobID)]
    SCH_Fairy_Combo_Consolation = 16017,

    [ReplaceSkill(RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo("Swiftcast Raise Combo Feature",
        "Changes Swiftcast to Resurrection while Swiftcast is on cooldown.", SCH.JobID)]
    SCH_Raise = 16032,

    [ParentCombo(SCH_Raise)]
    [CustomComboInfo("Retarget Raise", "Will Retarget the Raise affected here to your Heal Stack.", SCH.JobID)]
    [Retargeted(SCH.Resurrection)]
    SCH_Raise_Retarget = 16050,
    
    [Variant]
    [VariantParent(SCH_ST_ADV_DPS, SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Spirit Dart Option",
        "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", SCH.JobID)]
    SCH_DPS_Variant_SpiritDart = 16036,

    [Variant]
    [VariantParent(SCH_ST_ADV_DPS, SCH_AoE_ADV_DPS)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", SCH.JobID)]
    SCH_DPS_Variant_Rampart = 16037,

    #endregion
    
    #region Raidwide Features
    [CustomComboInfo("Raidwide Options", "Collection of tools designed to try and cast during a raidwide attack when detected." +
                                         "\nThis will work for most, but not all raidwide attacks and is no substitute for learning the fight", SCH.JobID)]
    SCH_Hidden = 16065,
    
    [ParentCombo(SCH_Hidden)]
    [CustomComboInfo("RaidWide Succor Option", "Will try to cast Succor when a raidwide casting is detected if shieldcheck from succor setting passes. \nWill be used in all 4 Advanced combos.", SCH.JobID)]
    SCH_Raidwide_Succor = 16062,
    
    [ParentCombo(SCH_Hidden)]
    [CustomComboInfo("Sacred Soil Option", "Will try to use Sacred Soil on self when a raidwide casting is detected.\nWill be used in all 4 Advanced combos", SCH.JobID)]
    [Retargeted(SCH.SacredSoil)]
    SCH_Raidwide_SacredSoil = 16059,
    
    [ParentCombo(SCH_Hidden)]
    [CustomComboInfo("Expedient Raidwide Option", "Will try to use Expedient when a raidwide casting is detected. \nWill be used in all 4 Advanced combos.", SCH.JobID)]
    SCH_Raidwide_Expedient = 16064,
    #endregion

    // Last value = 16072

    #endregion

    #region SUMMONER

    #region Simple Modes

    [AutoAction(false, false)]
    [ConflictingCombos(SMN_ST_Advanced_Combo)]
    [ReplaceSkill(SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Ruin with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.",
        SMN.JobID)]
    [SimpleCombo]
    SMN_ST_Simple_Combo = 17041,

    [AutoAction(true, false)]
    [ConflictingCombos(SMN_AoE_Advanced_Combo)]
    [ReplaceSkill(SMN.Outburst)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Outburst with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.",
        SMN.JobID)]
    [SimpleCombo]
    SMN_AoE_Simple_Combo = 17066,

    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ReplaceSkill(SMN.Ruin, SMN.Ruin2, SMN.Ruin3)]
    [ConflictingCombos(SMN_ST_Simple_Combo)]
    [CustomComboInfo("Advanced Mode - Single Target",
        "Replaces Ruin with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.",
        SMN.JobID)]
    [AdvancedCombo]
    SMN_ST_Advanced_Combo = 17000,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", SMN.JobID)]
    SMN_ST_Advanced_Combo_Balance_Opener = 170001,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Demi Summons Combo Option", "Adds Demi summons to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_DemiSummons = 17020,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Demi Attacks Combo Option", "Adds Demi Summon oGCDs to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_DemiSummons_Attacks = 17002,

    [ParentCombo(SMN_ST_Advanced_Combo_DemiSummons_Attacks)]
    [CustomComboInfo("Rekindle Combo Option", "Adds Rekindle to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_DemiSummons_Rekindle = 17028,

    [ParentCombo(SMN_ST_Advanced_Combo_DemiSummons_Rekindle)]
    [CustomComboInfo("Retarget Rekindle Combo Option", "Will Retarget Rekindle to a tank that needs it, then a party member that need healing, before Self.", SMN.JobID)]
    [Retargeted(SMN.Rekindle)]
    SMN_ST_Advanced_Combo_DemiSummons_Rekindle_Retarget = 17080,    

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Summon Titan", "Adds Titan to the Single Target Rotation", SMN.JobID)]
    SMN_ST_Advanced_Combo_Titan = 17073,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Summon Garuda", "Adds Garuda to the Single Target Rotation", SMN.JobID)]
    SMN_ST_Advanced_Combo_Garuda = 17074,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Summon Ifrit", "Adds Ifrit to the Single Target Rotation", SMN.JobID)]
    SMN_ST_Advanced_Combo_Ifrit = 17075,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Egi Attacks Combo Option", "Adds Gemshine to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_EgiSummons_Attacks = 17004,

    [ParentCombo(SMN_ST_Advanced_Combo_EgiSummons_Attacks)]
    [CustomComboInfo("Use Ruin III instead of Emerald Ruin III when standing still between the levels of 54 and 72",
        "Replaces Emerald Ruin III with Ruin III in the rotation when standing still and Ruin Mastery III is not active.",
        SMN.JobID)]
    SMN_ST_Ruin3_Emerald_Ruin3 = 17067,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Add Egi Astralflow", "Choose which Egi Astralflows to add to the rotation.", SMN.JobID)]
    SMN_ST_Advanced_Combo_Egi_AstralFlow = 17048,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Swiftcast Egi Ability Option", "Uses Swiftcast during the selected Egi summon.", SMN.JobID)]
    SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi = 17023,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Energy Attacks Combo Option",
        "Adds Energy Drain and Fester to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_EDFester = 17014,

    [ParentCombo(SMN_ST_Advanced_Combo_EDFester)]
    [CustomComboInfo("Pooled oGCDs Option",
        "Pools damage oGCDs for use while under the Searing Light buff.",
        SMN.JobID)]
    SMN_ST_Advanced_Combo_oGCDPooling = 17025,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Searing Light Combo Option",
       "Adds Searing Light to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_SearingLight = 17017,

    [ParentCombo(SMN_ST_Advanced_Combo_SearingLight)]
    [CustomComboInfo("Searing Light Burst Option",
        "Casts Searing Light only during Demi phases.\nSpellspeed builds would turn this off.",
        SMN.JobID)]
    SMN_ST_Advanced_Combo_SearingLight_Burst = 17018,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Searing Flash Combo Option", "Adds Searing Flash to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_SearingFlash = 17019,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Ruin IV Combo Option",
        "Adds Ruin IV to the single target combo.\nUses when moving during Garuda Phase and you have no attunement, when moving during Ifrit phase, or when you have no active Egi or Demi summon.",
        SMN.JobID)]
    SMN_ST_Advanced_Combo_Ruin4 = 17011,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Lux Solaris Combo Option", "Adds Lux Solaris to the single target combo.", SMN.JobID)]
    SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris = 17029,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Radiant Aegis Option", "Will use Radiant Aegis, 30 second self shield, when at 2 charges to prevent waste ", SMN.JobID)]
    SMN_ST_Advanced_Combo_Radiant = 17071,

    [ParentCombo(SMN_ST_Advanced_Combo)]
    [CustomComboInfo("Lucid Dreaming Option",
       "Adds Lucid Dreaming to the single target combo when MP falls below the set value.", SMN.JobID)]
    SMN_ST_Advanced_Combo_Lucid = 17031,

    #endregion

    #region Advanced AoE

    [AutoAction(true, false)]
    [ReplaceSkill(SMN.Outburst, SMN.Tridisaster)]
    [ConflictingCombos(SMN_AoE_Simple_Combo)]
    [CustomComboInfo("Advanced Mode - AoE",
        "Replaces Outburst with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.",
        SMN.JobID)]
    [AdvancedCombo]
    SMN_AoE_Advanced_Combo = 17049,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Demi Summons Combo Option", "Adds Demi summons to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_DemiSummons = 17061,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Demi Attacks Combo Option", "Adds Demi Summon oGCDs to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_DemiSummons_Attacks = 17055,

    [ParentCombo(SMN_AoE_Advanced_Combo_DemiSummons_Attacks)]
    [CustomComboInfo("Rekindle Combo Option", "Adds Rekindle to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_DemiSummons_Rekindle = 17056,

    [ParentCombo(SMN_AoE_Advanced_Combo_DemiSummons_Rekindle)]
    [CustomComboInfo("Retarget Rekindle Combo Option", "Will Retarget Rekindle to a tank that needs it, then a party member that need healing, before Self.", SMN.JobID)]
    [Retargeted(SMN.Rekindle)]
    SMN_AoE_Advanced_Combo_DemiSummons_Rekindle_Retarget = 17081,    

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Summon Titan", "Adds Titan to the AoE Rotation", SMN.JobID)]
    SMN_AoE_Advanced_Combo_Titan = 17076,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Summon Garuda", "Adds Garuda to the AoE Rotation", SMN.JobID)]
    SMN_AoE_Advanced_Combo_Garuda = 17077,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Summon Ifrit", "Adds Ifrit to the AoE Rotation", SMN.JobID)]
    SMN_AoE_Advanced_Combo_Ifrit = 17078,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Egi Attacks Combo Option", "Adds Precious Brilliance to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_EgiSummons_Attacks = 17064,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Add Egi Astralflow", "Choose which Egi Astralflows to add to the rotation.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_Egi_AstralFlow = 17068,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Swiftcast Egi Ability Option", "Uses Swiftcast during the selected Egi summon.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi = 17063,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Energy Attacks Combo Option",
        "Adds Energy Siphon and Painflare to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_ESPainflare = 17051,

    [ParentCombo(SMN_AoE_Advanced_Combo_ESPainflare)]
    [CustomComboInfo("Pooled oGCDs Option",
        "Pools damage oGCDs for use inside the Searing Light buff",
        SMN.JobID)]
    SMN_AoE_Advanced_Combo_oGCDPooling = 17050,    

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Searing Light Combo Option", "Adds Searing Light to the AoE combo.",
        SMN.JobID)]
    SMN_AoE_Advanced_Combo_SearingLight = 17053,

    [ParentCombo(SMN_AoE_Advanced_Combo_SearingLight)]
    [CustomComboInfo("Searing Light Burst Option",
        "Casts Searing Light only during Demi phases.\nSpellspeed Builds would turn this off.",
        SMN.JobID)]
    SMN_AoE_Advanced_Combo_SearingLight_Burst = 17054,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Searing Flash Combo Option", "Adds Searing Flash to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_SearingFlash = 17058,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Ruin IV Combo Option",
       "Adds Ruin IV to the AoE combo.\nUses when moving during Garuda Phase and you have no attunement, when moving during Ifrit phase, or when you have no active Egi or Demi summon.",
       SMN.JobID)]
    SMN_AoE_Advanced_Combo_Ruin4 = 17062,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Lux Solaris Combo Option", "Adds Lux Solaris to the AoE combo.", SMN.JobID)]
    SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris = 17059,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Radiant Aegis Option", "Will use Radiant Aegis, 30 second self shield, when at 2 charges to prevent waste ", SMN.JobID)]
    SMN_AoE_Advanced_Combo_Radiant = 17070,

    [ParentCombo(SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming to the AoE combo when MP falls below the set value.",
        SMN.JobID)]
    SMN_AoE_Advanced_Combo_Lucid = 17060,    
    #endregion

    #region Standalone Features
    [ReplaceSkill(SMN.Fester)]
    [CustomComboInfo("Fester to Energy Drain Feature", "Change Fester into Energy Drain when out of Aetherflow stacks.",
        SMN.JobID)]
    SMN_EDFester = 17008,

    [ParentCombo(SMN_EDFester)]
    [CustomComboInfo("Ruin IV Fester Option",
        "Changes Fester to Ruin IV when out of Aetherflow stacks, Energy Drain is on cooldown, and Ruin IV is available.",
        SMN.JobID)]
    SMN_EDFester_Ruin4 = 17013,

    [ReplaceSkill(SMN.Painflare)]
    [CustomComboInfo("Painflare to Energy Siphon Feature",
        "Change Painflare into Energy Siphon when out of Aetherflow stacks.", SMN.JobID)]
    SMN_ESPainflare = 17009,

    [CustomComboInfo("Carbuncle Reminder Feature",
        "Replaces most offensive actions with Summon Carbuncle when it is not summoned.", SMN.JobID)]
    SMN_CarbuncleReminder = 17010,

    [CustomComboInfo("Astral Flow/Enkindle on Demis Feature",
        "Adds Enkindle Bahamut, Enkindle Phoenix and Astral Flow to their relevant summons.", SMN.JobID)]
    SMN_DemiAbilities = 17024,

    [ConflictingCombos(ALL_Caster_Raise)]
    [CustomComboInfo("Alternative Raise Feature", "Changes Swiftcast to Raise when on cooldown.", SMN.JobID)]
    SMN_Raise = 17027,

    [ParentCombo(SMN_Raise)]
    [CustomComboInfo("Retarget Raises", "Will Retarget the Raises affected here to your Heal Stack.", SMN.JobID)]
    [Retargeted(SMN.Resurrection)]
    SMN_Raise_Retarget = 17079,

    [ReplaceSkill(SMN.Ruin4)]
    [CustomComboInfo("Ruin III Mobility Feature", "Puts Ruin III on Ruin IV when you don't have Further Ruin.",
        SMN.JobID)]
    SMN_RuinMobility = 17030,

    [CustomComboInfo("Egi Abilities on Summons Feature",
        "Adds Egi Abilities (Astral Flow) to Egi summons when ready.\nEgi abilities will appear on their respective Egi summon ability, as well as Titan.",
        SMN.JobID)]
    SMN_Egi_AstralFlow = 17034,

    [ParentCombo(SMN_ESPainflare)]
    [CustomComboInfo("Ruin IV Painflare Option",
        "Changes Painflare to Ruin IV when out of Aetherflow stacks, Energy Siphon is on cooldown, and Ruin IV is up.",
        SMN.JobID)]
    SMN_ESPainflare_Ruin4 = 17039,

    [CustomComboInfo("Searing Light Waste Protection Feature",
           "Changes Searing light to SavageBlade while under the affect of another summoners buff to prevent waste.", SMN.JobID)]
    SMN_Searing = 17072,
    #endregion

    #region Variant
    [Variant]
    [VariantParent(SMN_ST_Simple_Combo, SMN_ST_Advanced_Combo, SMN_AoE_Simple_Combo, SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", SMN.JobID)]
    SMN_Variant_Rampart = 17045,

    [Variant]
    [VariantParent(SMN_Raise)]
    [CustomComboInfo("Raise Option", "Turn Swiftcast into Variant Raise whenever you have the Swiftcast buff.",
        SMN.JobID)]
    SMN_Variant_Raise = 17046,

    [Variant]
    [VariantParent(SMN_ST_Simple_Combo, SMN_ST_Advanced_Combo, SMN_AoE_Simple_Combo, SMN_AoE_Advanced_Combo)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", SMN.JobID)]
    SMN_Variant_Cure = 17047,

    #endregion

    // Last Used 17078

    #endregion

    #region VIPER

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(VPR.SteelFangs)]
    [ConflictingCombos(VPR_ST_AdvancedMode, VPR_SerpentsTail, VPR_Legacies)]
    [CustomComboInfo("Simple Mode - Single Target", "Replaces Steel Fangs with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.", VPR.JobID)]
    [SimpleCombo]
    VPR_ST_SimpleMode = 30000,

    [AutoAction(true, false)]
    [ReplaceSkill(VPR.SteelMaw)]
    [ConflictingCombos(VPR_AoE_AdvancedMode, VPR_SerpentsTail)]
    [CustomComboInfo("Simple Mode - AoE", "Replaces Steel Maw with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.", VPR.JobID)]
    [SimpleCombo]
    VPR_AoE_SimpleMode = 30100,

    #endregion

    #region Advanced ST Viper

    [AutoAction(false, false)]
    [ReplaceSkill(VPR.SteelFangs)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_SerpentsTail, VPR_Legacies)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Steel Fangs with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", VPR.JobID)]
    [AdvancedCombo]
    VPR_ST_AdvancedMode = 30001,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.\n Does not check positional choice.\n Always does Hunter's Coil first (FLANK)", VPR.JobID)]
    VPR_ST_Opener = 30002,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Serpents Ire", "Adds Serpents Ire to the rotation.", VPR.JobID)]
    VPR_ST_SerpentsIre = 30005,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Vicewinder", "Adds Vicewinder to the rotation.", VPR.JobID)]
    VPR_ST_Vicewinder = 30006,
    
    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Vicewinder Combo", "Adds Swiftskin's Coil and Hunter's Coil to the rotation.\nWill automatically swap depending on your position.", VPR.JobID)]
    VPR_ST_VicewinderCombo = 30007,
    
    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Vicewinder Weaves", "Adds Twinfang Bite and Twinblood Bite to the rotation.", VPR.JobID)]
    VPR_ST_VicewinderWeaves = 30013,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Serpents Tail", "Adds Serpents Tail to the rotation.", VPR.JobID)]
    VPR_ST_SerpentsTail = 30008,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Uncoiled Fury", "Adds Uncoiled Fury to the rotation.", VPR.JobID)]
    VPR_ST_UncoiledFury = 30009,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Uncoiled Fury Combo", "Adds Uncoiled Twinfang and Uncoiled Twinblood to the rotation.", VPR.JobID)]
    VPR_ST_UncoiledFuryCombo = 30010,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Reawaken", "Adds Reawaken to the rotation.", VPR.JobID)]
    VPR_ST_Reawaken = 30011,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Generations Combo", "Adds Generations to the rotation.", VPR.JobID)]
    VPR_ST_GenerationCombo = 30012,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Legacy weaves", "Adds Legacy weaves to the rotation.", VPR.JobID)]
    VPR_ST_LegacyWeaves = 30014,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Dynamic True North Option", "Adds True North when you are not in the correct position for the enhanced potency bonus.", VPR.JobID)]
    VPR_TrueNorthDynamic = 30098,

    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Ranged Uptime Option", "Adds Writhing Snap to the rotation when you are out of melee range.", VPR.JobID)]
    VPR_ST_RangedUptime = 30095,
    
    [ParentCombo(VPR_ST_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the rotation.", VPR.JobID)]
    VPR_ST_ComboHeals = 30097,

    #endregion

    #region Advanced AoE Viper

    [AutoAction(true, false)]
    [ReplaceSkill(VPR.SteelMaw)]
    [ConflictingCombos(VPR_AoE_SimpleMode, VPR_SerpentsTail)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Steel Maw with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", VPR.JobID)]
    [AdvancedCombo]
    VPR_AoE_AdvancedMode = 30101,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Serpents Ire", "Adds Serpents Ire to the rotation.", VPR.JobID)]
    VPR_AoE_SerpentsIre = 30104,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Vicepit", "Adds Vicepit to the rotation.", VPR.JobID)]
    VPR_AoE_Vicepit = 30105,
    
    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Vicepit Combo", "Adds Swiftskin's Den and Hunter's Den to the rotation.", VPR.JobID)]
    VPR_AoE_VicepitCombo = 30106,
    
    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Vicepit Weaves", "Adds Twinfang Thresh and Twinblood Thresh to the rotation.", VPR.JobID)]
    VPR_AoE_VicepitWeaves = 30115,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Serpents Tail", "Adds Serpents Tail to the rotation.", VPR.JobID)]
    VPR_AoE_SerpentsTail = 30107,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Uncoiled Fury", "Adds Uncoiled Fury to the rotation.", VPR.JobID)]
    VPR_AoE_UncoiledFury = 30108,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Uncoiled Fury Combo", "Adds Uncoiled Twinfang and Uncoiled Twinblood to the rotation.", VPR.JobID)]
    VPR_AoE_UncoiledFuryCombo = 30109,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Reawaken", "Adds Reawaken to the rotation.", VPR.JobID)]
    VPR_AoE_Reawaken = 30110,
    
    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Reawaken Combo", "Adds Generations and Legacy weaves to the rotation.", VPR.JobID)]
    VPR_AoE_ReawakenCombo = 30112,

    [ParentCombo(VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Combo Heals Option", "Adds Bloodbath and Second Wind to the rotation.", VPR.JobID)]
    VPR_AoE_ComboHeals = 30199,

    #endregion
    
    #region Basic combo

    [ReplaceSkill(VPR.ReavingFangs)]
    [ConflictingCombos(VPR_ReawakenLegacy, VPR_Legacies, VPR_SerpentsTail)]
    [CustomComboInfo("Basic Combo", "Replace Reaving Fangs with its combo chain.", VPR.JobID)]
    [BasicCombo]
    VPR_ST_BasicCombo = 30015,

    #endregion

    #region Variant

    [Variant]
    [VariantParent(VPR_ST_SimpleMode, VPR_AoE_SimpleMode, VPR_ST_AdvancedMode, VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", VPR.JobID)]
    VPR_Variant_Cure = 30300,

    [Variant]
    [VariantParent(VPR_ST_SimpleMode, VPR_AoE_SimpleMode, VPR_ST_AdvancedMode, VPR_AoE_AdvancedMode)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", VPR.JobID)]
    VPR_Variant_Rampart = 30301,

    #endregion

    #region Miscellaneous

    [ReplaceSkill(VPR.Vicewinder)]
    [CustomComboInfo("Vicewinder - Coils", "Replaces Vicewinder with Hunter's/Swiftskin's Coils.\nWill automatically swap depending on your position.", VPR.JobID)]
    VPR_VicewinderCoils = 30200,

    [ReplaceSkill(VPR.Vicepit)]
    [CustomComboInfo("Vicepit - Dens", "Replaces Vicepit with Hunter's/Swiftskin's Dens.", VPR.JobID)]
    VPR_VicepitDens = 30201,

    [ReplaceSkill(VPR.UncoiledFury)]
    [CustomComboInfo("Uncoiled - Twins", "Replaces Uncoiled Fury with Uncoiled Twinfang and Uncoiled Twinblood.", VPR.JobID)]
    VPR_UncoiledTwins = 30202,

    [ReplaceSkill(VPR.Reawaken, VPR.ReavingFangs)]
    [ConflictingCombos(VPR_Legacies, VPR_ST_BasicCombo)]
    [CustomComboInfo("Reawaken - Generation", "Replaces Option with the Generations.", VPR.JobID)]
    VPR_ReawakenLegacy = 30203,

    [ParentCombo(VPR_ReawakenLegacy)]
    [CustomComboInfo("Reawaken - Legacy", "Replaces Option with the Legacys.", VPR.JobID)]
    VPR_ReawakenLegacyWeaves = 30204,

    [ReplaceSkill(VPR.SerpentsTail)]
    [CustomComboInfo("Combined Combo Ability Feature", "Combines Serpent's Tail, Twinfang, and Twinblood to one button.", VPR.JobID)]
    VPR_TwinTails = 30205,

    [ParentCombo(VPR_VicewinderCoils)]
    [CustomComboInfo("Include Twin Combo Actions", "Adds Twinfang and Twinblood to the button.", VPR.JobID)]
    VPR_VicewinderCoils_oGCDs = 30206,

    [ParentCombo(VPR_VicepitDens)]
    [CustomComboInfo("Include Twin Combo Actions", "Adds Twinfang and Twinblood to the button.", VPR.JobID)]
    VPR_VicepitDens_oGCDs = 30207,

    [ReplaceSkill(VPR.SteelFangs, VPR.ReavingFangs, VPR.HuntersCoil, VPR.SwiftskinsCoil)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_ST_AdvancedMode, VPR_SerpentsTail, VPR_ReawakenLegacy, VPR_ST_BasicCombo)]
    [CustomComboInfo("Legacy Buttons", "Replaces Generations with the Legacys.", VPR.JobID)]
    VPR_Legacies = 30209,

    [ReplaceSkill(VPR.SteelFangs, VPR.ReavingFangs, VPR.SteelMaw, VPR.ReavingMaw)]
    [ConflictingCombos(VPR_ST_SimpleMode, VPR_AoE_SimpleMode, VPR_ST_AdvancedMode, VPR_AoE_AdvancedMode, VPR_Legacies, VPR_ST_BasicCombo)]
    [CustomComboInfo("Serpents Tail", "Replaces basic combo with Death Rattle or Last Lash when applicable.", VPR.JobID)]
    VPR_SerpentsTail = 30210,

    #endregion
    //ST 30016
    //AoE 30115
    //Misc 30210

    #endregion

    #region WARRIOR

    #region Simple Mode
    [AutoAction(false, false)]
    [ConflictingCombos(WAR_ST_Advanced)]
    [ReplaceSkill(WAR.HeavySwing)]
    [CustomComboInfo("Simple Mode - Single Target",
        "Replaces Heavy Swing with a full one-button single target rotation.\nThis is the ideal option for newcomers to the job.",
        WAR.JobID)]
    [SimpleCombo]
    WAR_ST_Simple = 18000,

    [AutoAction(true, false)]
    [ConflictingCombos(WAR_AoE_Advanced)]
    [ReplaceSkill(WAR.Overpower)]
    [CustomComboInfo("Simple Mode - AoE",
        "Replaces Overpower with a full one-button AoE rotation.\nThis is the ideal option for newcomers to the job.",
        WAR.JobID)]
    [SimpleCombo]
    WAR_AoE_Simple = 18001,
    #endregion

    #region Advanced ST
    [AutoAction(false, false)]
    [ConflictingCombos(WAR_ST_Simple)]
    [ReplaceSkill(WAR.HeavySwing)]
    [CustomComboInfo("Advanced Mode - Single Target", "Replaces Heavy Swing with a full one-button single target rotation.\nThese features are ideal if you want to customize the rotation.", WAR.JobID)]
    [AdvancedCombo]
    WAR_ST_Advanced = 18002,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Balance Opener (Level 100)", "Adds the Balance opener at level 100.", WAR.JobID)]
    WAR_ST_BalanceOpener = 18058,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Storm's Eye Option", "Adds Storm's Eye into the rotation.", WAR.JobID)]
    WAR_ST_StormsEye = 18005,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Inner Release Option", "Adds Berserk / Inner Release into the rotation.", WAR.JobID)]
    WAR_ST_InnerRelease = 18003,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Fell Cleave Option", "Adds Inner Beast / Fell Cleave into the rotation.\n- Will use at set minimum Beast Gauge value or to consume Inner Release stacks\n- Also includes Inner Chaos when available", WAR.JobID)]
    WAR_ST_FellCleave = 18006,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Infuriate Option", "Adds Infuriate into the rotation.", WAR.JobID)]
    WAR_ST_Infuriate = 18007,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Onslaught Option", "Adds Onslaught into the rotation.", WAR.JobID)]
    WAR_ST_Onslaught = 18008,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Upheaval Option", "Adds Upheaval into the rotation.", WAR.JobID)]
    WAR_ST_Upheaval = 18009,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Primal Rend Option", "Adds Primal Rend into the rotation.", WAR.JobID)]
    WAR_ST_PrimalRend = 18013,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Primal Wrath Option", "Adds Primal Wrath into the rotation.", WAR.JobID)]
    WAR_ST_PrimalWrath = 18010,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Primal Ruination Option", "Adds Primal Ruination into the rotation.", WAR.JobID)]
    WAR_ST_PrimalRuination = 18011,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Tomahawk Uptime Option", "Adds Tomahawk into the rotation when you are out of range.", WAR.JobID)]
    WAR_ST_RangedUptime = 18004,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the rotation when your target's cast is interruptible.", WAR.JobID)]
    WAR_ST_Interrupt = 18066,

    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Interrupt with Stun Option", "Adds Low Blow to the rotation when your target is casting.\nNot advised outside of overworld content, as it can waste a lot of Low Blows on un-stun-able enemies, etc. Will try to not use it in boss fights.", WAR.JobID)]
    WAR_ST_Stun = 18112,

    #region Mitigations
    [ParentCombo(WAR_ST_Advanced)]
    [CustomComboInfo("Mitigation Options", "Adds defensive actions into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Mitigation = 18040,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Bloodwhetting Option", "Adds Raw Intuition / Bloodwhetting into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Bloodwhetting = 18031,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Equilibrium Option", "Adds Equilibrium into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Equilibrium = 18043,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Rampart = 18032,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Thrill of Battle Option", "Adds Thrill of Battle into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Thrill = 18042,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Vengeance Option", "Adds Vengeance / Damnation into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Vengeance = 18033,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Holmgang Option", "Adds Holmgang into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Holmgang = 18034,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_Reprisal = 18061,

    [ParentCombo(WAR_ST_Mitigation)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_ST_ArmsLength = 18062,
    #endregion

    #endregion

    #region Advanced AoE
    [AutoAction(true, false)]
    [ConflictingCombos(WAR_AoE_Simple)]
    [ReplaceSkill(WAR.Overpower)]
    [CustomComboInfo("Advanced Mode - AoE", "Replaces Overpower with a full one-button AoE rotation.\nThese features are ideal if you want to customize the rotation.", WAR.JobID)]
    [AdvancedCombo]
    WAR_AoE_Advanced = 18016,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Berserk / Inner Release Option", "Adds Berserk / Inner Release into the AoE rotation.", WAR.JobID)]
    WAR_AoE_InnerRelease = 18019,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Steel Cyclone / Decimate Option", "Adds Steel Cyclone / Decimate into the AoE rotation.", WAR.JobID)]
    WAR_AoE_Decimate = 18023,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Infuriate Option", "Adds Infuriate into the AoE rotation.", WAR.JobID)]
    WAR_AoE_Infuriate = 18018,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Onslaught Option", "Adds Onslaught into the AoE rotation.", WAR.JobID)]
    WAR_AoE_Onslaught = 18071,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Orogeny Option", "Adds Orogeny into the AoE rotation.", WAR.JobID)]
    WAR_AoE_Orogeny = 18012,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Primal Rend Option", "Adds Primal Rend into the AoE rotation.", WAR.JobID)]
    WAR_AoE_PrimalRend = 18021,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Primal Wrath Option", "Adds Primal Wrath into the AoE rotation.", WAR.JobID)]
    WAR_AoE_PrimalWrath = 18020,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Primal Ruination Option", "Adds Primal Ruination into the AoE rotation.", WAR.JobID)]
    WAR_AoE_PrimalRuination = 18022,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Tomahawk Uptime Option", "Adds Tomahawk into the rotation when you are out of range.", WAR.JobID)]
    WAR_AoE_RangedUptime = 18110,

    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Interrupt Option", "Adds Interject to the AoE rotation when your target's cast is interruptible.", WAR.JobID)]
    WAR_AoE_Interrupt = 18067,

    [ParentCombo(WAR_AoE_Interrupt)]
    [CustomComboInfo("Interrupt with Stun Option", "Adds Low Blow to the AoE rotation when your target is casting, interruptible or not.", WAR.JobID)]
    WAR_AoE_Stun = 18068,

    #region Mitigations
    [ParentCombo(WAR_AoE_Advanced)]
    [CustomComboInfo("Mitigation Options", "Adds defensive actions into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Mitigation = 18035,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Bloodwhetting Option", "Adds Raw Intuition / Bloodwhetting into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Bloodwhetting = 18036,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Equilibrium Option", "Adds Equilibrium into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Equilibrium = 18044,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Rampart Option", "Adds Rampart into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Rampart = 18037,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Thrill of Battle Option", "Adds Thrill of Battle into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Thrill = 18041,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Vengeance Option", "Adds Vengeance / Damnation into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Vengeance = 18038,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Holmgang Option", "Adds Holmgang into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Holmgang = 18039,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_Reprisal = 18063,

    [ParentCombo(WAR_AoE_Mitigation)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length into the rotation based on Health percentage remaining.", WAR.JobID)]
    WAR_AoE_ArmsLength = 18064,
    #endregion

    #endregion

    #region One-Button Mitigation
    [ReplaceSkill(WAR.ThrillOfBattle)]
    [CustomComboInfo("One-Button Mitigation Feature", "Replaces Thrill Of Battle with an all-in-one mitigation button.", WAR.JobID)]
    [MitigationCombo]
    WAR_Mit_OneButton = 18045,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Holmgang Emergency Option", "Gives max priority to Holmgang when the Health percentage threshold is met.", WAR.JobID)]
    WAR_Mit_Holmgang_Max = 18046,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Bloodwhetting Option", "Adds Raw Intuition / Bloodwhetting to the one-button mitigation.", WAR.JobID)]
    WAR_Mit_Bloodwhetting = 18047,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Equilibrium Option", "Adds Equilibrium to the one-button mitigation.", WAR.JobID)]
    WAR_Mit_Equilibrium = 18048,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Reprisal Option", "Adds Reprisal to the one-button mitigation.\nNOTE: Will not use unless there is a target within range to prevent waste", WAR.JobID)]
    WAR_Mit_Reprisal = 18049,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Thrill Of Battle First Option", "Adds Thrill Of Battle to the one-button mitigation.\nNOTE: even if disabled, will still try to use Thrill Of Battle as the lowest priority.", WAR.JobID)]
    WAR_Mit_ThrillOfBattle = 18050,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Rampart Option", "Adds Rampart to the one-button mitigation.", WAR.JobID)]
    WAR_Mit_Rampart = 18051,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Shake It Off Option", "Adds Shake It Off to the one-button mitigation.", WAR.JobID)]
    WAR_Mit_ShakeItOff = 18052,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Arm's Length Option", "Adds Arm's Length to the one-button mitigation.", WAR.JobID)]
    WAR_Mit_ArmsLength = 18053,

    [ParentCombo(WAR_Mit_OneButton)]
    [CustomComboInfo("Vengeance Option", "Adds Vengeance to the one-button mitigation.", WAR.JobID)]
    WAR_Mit_Vengeance = 18054,

    [ReplaceSkill(WAR.ShakeItOff)]
    [CustomComboInfo("One-Button Party Mitigation Feature", "Replaces Shake It Off with Reprisal when ready.", WAR.JobID)]
    [MitigationCombo]
    WAR_Mit_Party = 18111,
    #endregion

    #region Misc

    #region Fell Cleave Features
    [ReplaceSkill(WAR.FellCleave)]
    [ConflictingCombos(WAR_InfuriateFellCleave)]
    [CustomComboInfo("Fell Cleave Features", "Collection of Fell Cleave related features.\n Enable all for this to be an all-in-one Single Target Burst button.", WAR.JobID)]
    WAR_FC_Features = 18122,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Inner Release Option", "Adds Berserk / Inner Release to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_InnerRelease = 18123,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Infuriate Option", "Adds Infuriate to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_Infuriate = 18124,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Onslaught Option", "Adds Onslaught to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_Onslaught = 18125,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Upheaval Option", "Adds Upheaval to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_Upheaval = 18126,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Primal Rend Option", "Adds Primal Rend to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_PrimalRend = 18127,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Primal Wrath Option", "Adds Primal Wrath to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_PrimalWrath = 18128,

    [ParentCombo(WAR_FC_Features)]
    [CustomComboInfo("Primal Ruination Option", "Adds Primal Ruination to Fell Cleave when available.", WAR.JobID)]
    WAR_FC_PrimalRuination = 18129,
    #endregion

    #region Basic Combo
    [ReplaceSkill(WAR.StormsPath)]
    [CustomComboInfo("Storm's Path Combo", "Replace Storm's Path with its combo chain.", WAR.JobID)]
    WAR_ST_StormsPathCombo = 18069,

    [ReplaceSkill(WAR.StormsEye)]
    [CustomComboInfo("Storm's Eye Combo", "Replace Storm's Eye with its combo chain.", WAR.JobID)]
    WAR_ST_StormsEyeCombo = 18070,
    #endregion

    [ReplaceSkill(WAR.FellCleave, WAR.Decimate)]
    [ConflictingCombos(WAR_FC_Features)]
    [CustomComboInfo("Infuriate on Fell Cleave / Decimate Feature", "Turns Fell Cleave and Decimate into Infuriate if at or under set gauge value.", WAR.JobID)]
    WAR_InfuriateFellCleave = 18024,

    [ParentCombo(WAR_InfuriateFellCleave)]
    [CustomComboInfo("Inner Release Priority Option", "Prevents the use of Infuriate while you have Inner Release stacks available.", WAR.JobID)]
    WAR_InfuriateFellCleave_IRFirst = 18027,

    [ReplaceSkill(WAR.StormsPath)]
    [CustomComboInfo("Storm's Eye Feature", "Replaces Storm's Path with Storm's Eye when Surging Tempest buff needs refreshing.", WAR.JobID)]
    WAR_EyePath = 18057,

    [ReplaceSkill(WAR.Berserk, WAR.InnerRelease)]
    [CustomComboInfo("Primal Combo Feature", "Turns Berserk / Inner Release into the Primal combo (Primal Rend -> Primal Ruination) on use.", WAR.JobID)]
    WAR_PrimalCombo_InnerRelease = 18026,

    [Variant]
    [VariantParent(WAR_ST_Advanced, WAR_AoE_Advanced)]
    [CustomComboInfo("Spirit Dart Option", "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", WAR.JobID)]
    WAR_Variant_SpiritDart = 18028,

    [Variant]
    [VariantParent(WAR_ST_Advanced, WAR_AoE_Advanced)]
    [CustomComboInfo("Cure Option", "Use Variant Cure when HP is below set threshold.", WAR.JobID)]
    WAR_Variant_Cure = 18029,

    [Variant]
    [VariantParent(WAR_ST_Advanced, WAR_AoE_Advanced)]
    [CustomComboInfo("Ultimatum Option", "Use Variant Ultimatum on cooldown.", WAR.JobID)]
    WAR_Variant_Ultimatum = 18030,

    [ReplaceSkill(WAR.Equilibrium)]
    [CustomComboInfo("Equilibirum to Thrill of Battle Feature", "Replaces Equilibirum with Thrill of Battle when ready.", WAR.JobID)]
    WAR_ThrillEquilibrium = 18055,

    [ReplaceSkill(WAR.NascentFlash)]
    [CustomComboInfo("Nascent Flash Feature", "Replace Nascent Flash with Raw intuition when level synced below 76.", WAR.JobID)]
    WAR_NascentFlash = 18017,
    
    [ReplaceSkill(WAR.RawIntuition, WAR.Bloodwhetting)]
    [CustomComboInfo("Raw Intuition to Nascent Flash Retargeting Feature", "If available, will replace the Raw Intuition/Bloodwhetting with Nascent Flash if you are hard targeting an ally.", WAR.JobID)]
    [Retargeted(WAR.NascentFlash)]
    WAR_RawIntuition_Targeting = 18119,

    [ParentCombo(WAR_RawIntuition_Targeting)]
    [CustomComboInfo("Include MouseOver Target", "If mousing over an ally in the UI, will Retarget Nascent Flash onto them.", WAR.JobID)]
    [Retargeted]
    WAR_RawIntuition_Targeting_MO = 18120,

    [ParentCombo(WAR_RawIntuition_Targeting)]
    [CustomComboInfo("Include Target's Target", "If your target's target is not you, will Retarget Nascent Flash onto them.\n(if you're not top aggro, and not mousing over or hard targeting an ally)", WAR.JobID)]
    [Retargeted]
    WAR_RawIntuition_Targeting_TT = 18121,

    [ReplaceSkill(WAR.Holmgang)]
    [CustomComboInfo("Retarget Holmgang Feature", "Will Retarget Holmgang to yourself, instead of letting it go on enemies.", WAR.JobID)]
    [Retargeted(WAR.Holmgang)]
    WAR_RetargetHolmgang = 18130,

    #region Bozja
    [Bozja]
    [CustomComboInfo("Lost Focus Option", "Use Lost Focus when available.", WAR.JobID)]
    WAR_Bozja_LostFocus = 18072,

    [Bozja]
    [CustomComboInfo("Lost Font Of Power Option", "Use Lost Font Of Power when available.", WAR.JobID)]
    WAR_Bozja_LostFontOfPower = 18073,

    [Bozja]
    [CustomComboInfo("Lost Slash Option", "Use Lost Slash when available.", WAR.JobID)]
    WAR_Bozja_LostSlash = 18074,

    [Bozja]
    [CustomComboInfo("Lost Death Option", "Use Lost Death when available.", WAR.JobID)]
    WAR_Bozja_LostDeath = 18075,

    [Bozja]
    [CustomComboInfo("Banner Of Noble Ends Option", "Use Banner Of Noble Ends when available.", WAR.JobID)]
    WAR_Bozja_BannerOfNobleEnds = 18076,

    [Bozja]
    [ParentCombo(WAR_Bozja_BannerOfNobleEnds)]
    [CustomComboInfo("Only with `Lost Font Of Power` Option", "Use only under Lost Font of Power.", WAR.JobID)]
    WAR_Bozja_PowerEnds = 18077,

    [Bozja]
    [CustomComboInfo("Banner Of Honored Sacrifice Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_BannerOfHonoredSacrifice = 18078,

    [Bozja]
    [ParentCombo(WAR_Bozja_BannerOfHonoredSacrifice)]
    [CustomComboInfo("Only with `Lost Font Of Power` Option", "Use only under Lost Font of Power.", WAR.JobID)]
    WAR_Bozja_PowerSacrifice = 18079,

    [Bozja]
    [CustomComboInfo("Banner Of Honed Acuity Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_BannerOfHonedAcuity = 18080,

    [Bozja]
    [CustomComboInfo("Lost Fair Trade Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostFairTrade = 18081,

    [Bozja]
    [CustomComboInfo("Lost Assassination Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostAssassination = 18082,

    [Bozja]
    [CustomComboInfo("Lost Manawall Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostManawall = 18083,

    [Bozja]
    [CustomComboInfo("Banner Of Tireless Conviction Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_BannerOfTirelessConviction = 18084,

    [Bozja]
    [CustomComboInfo("Lost Blood Rage Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostBloodRage = 18085,

    [Bozja]
    [CustomComboInfo("Banner Of Solemn Clarity Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_BannerOfSolemnClarity = 18086,

    [Bozja]
    [CustomComboInfo("Lost Cure Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostCure = 18087,

    [Bozja]
    [CustomComboInfo("Lost Cure II Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostCure2 = 18088,

    [Bozja]
    [CustomComboInfo("Lost Cure III Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostCure3 = 18089,

    [Bozja]
    [CustomComboInfo("Lost Cure IV Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostCure4 = 18090,

    [Bozja]
    [CustomComboInfo("Lost Arise Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostArise = 18091,

    [Bozja]
    [CustomComboInfo("Lost Sacrifice Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostSacrifice = 18092,

    [Bozja]
    [CustomComboInfo("Lost Reraise Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostReraise = 18093,

    [Bozja]
    [CustomComboInfo("Lost Spellforge Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostSpellforge = 18094,

    [Bozja]
    [CustomComboInfo("Lost Steel Sting Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostSteelsting = 18095,

    [Bozja]
    [CustomComboInfo("Lost Protect Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostProtect = 18096,

    [Bozja]
    [CustomComboInfo("Lost Shell Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostShell = 18097,

    [Bozja]
    [CustomComboInfo("Lost Reflect Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostReflect = 18098,

    [Bozja]
    [CustomComboInfo("Lost Bravery Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostBravery = 18099,

    [Bozja]
    [CustomComboInfo("Lost Aethershield Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostAethershield = 18100,

    [Bozja]
    [CustomComboInfo("Lost Protect II Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostProtect2 = 18101,

    [Bozja]
    [CustomComboInfo("Lost Shell II Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostShell2 = 18102,

    [Bozja]
    [CustomComboInfo("Lost Bubble Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostBubble = 18103,

    [Bozja]
    [CustomComboInfo("Lost Stealth Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostStealth = 18104,

    [Bozja]
    [CustomComboInfo("Lost Swift Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostSwift = 18105,

    [Bozja]
    [CustomComboInfo("Lost Font Of Skill Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostFontOfSkill = 18106,

    [Bozja]
    [CustomComboInfo("Lost Impetus Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostImpetus = 18107,

    [Bozja]
    [CustomComboInfo("Lost Paralyze III Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostParalyze3 = 18108,

    [Bozja]
    [CustomComboInfo("Lost Rampage Option", "Use when available.", WAR.JobID)]
    WAR_Bozja_LostRampage = 18109,

    #endregion

    #region Hidden Features

    [CustomComboInfo("Hidden Options", "Collection of cheeky or encounter-specific extra options only available to those in the know.\nDo not expect these options to be maintained, or even kept, after they are no longer Current.", WAR.JobID)]
    [Hidden]
    WAR_Hidden = 18113,

    [ParentCombo(WAR_Hidden)]
    [CustomComboInfo("R6S: Hold Burst on Squirrels", "When you're targeting Squirrels in R6S add phase, hold burst.\n(until about the time the first manta is dying)", WAR.JobID)]
    [Hidden]
    WAR_Hid_R6SHoldSquirrelBurst = 18114,

    [ParentCombo(WAR_Hidden)]
    [CustomComboInfo("R6S: Only Stun Jabberwock", "When in R6S, stun will only ever be used on the Jabberwock.", WAR.JobID)]
    [Hidden]
    WAR_Hid_R6SStunJabberOnly = 18115,

    [ParentCombo(WAR_Hidden)]
    [CustomComboInfo("R6S: Save Reprisal and Dark Missionary", "When in R6S, never try use Reprisal or Shake it Off automatically.", WAR.JobID)]
    [Hidden]
    WAR_Hid_R6SNoAutoGroupMits = 18116,

    [ParentCombo(WAR_Hidden)]
    [CustomComboInfo("R7S: Only Interrupt the adds casting Circle AoEs", "When you're in R7S, Interrupting will only work when you're targeting an add casting the circle AoE.", WAR.JobID)]
    [Hidden]
    WAR_Hid_R7SCircleCastOnly = 18117,

    #endregion

    // Last value = 18130

    #endregion

    #endregion

    #region WHITE MAGE

    #region Simple Mode

    [AutoAction(false, false)]
    [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
    [ConflictingCombos(WHM_ST_MainCombo)]
    [CustomComboInfo("Simple DPS Mode - Single Target", "Replaces Stone with a full one-button single target rotation. \nThis is the ideal option for newcomers to the job.",
        WHM.JobID)]
    [SimpleCombo]
    WHM_ST_Simple_DPS = 19050,
    
    [AutoAction(true, false)]
    [ReplaceSkill(WHM.Holy, WHM.Holy3)]
    [ConflictingCombos(WHM_AoE_DPS)]
    [CustomComboInfo("Simple DPS Mode - AoE", "Replaces Holy with a full one-button AoE rotation. \nThis is the ideal option for newcomers to the job.",
        WHM.JobID)]
    [SimpleCombo]
    WHM_AoE_Simple_DPS = 19051,

    #endregion

    #region Advanced Single Target DPS Combo

    [AutoAction(false, false)]
    [ReplaceSkill(WHM.Stone1, WHM.Stone2, WHM.Stone3, WHM.Stone4, WHM.Glare1, WHM.Glare3)]
    [ConflictingCombos(WHM_ST_Simple_DPS)]
    [CustomComboInfo("Advanced DPS Mode - Single Target", "Collection of cooldowns and spell features on Glares/Stones/Aeros/Dia.",
        WHM.JobID)]
    [AdvancedCombo]
    WHM_ST_MainCombo = 19099,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Balance Opener (Level 92)", "Adds the Balance opener from level 92 onwards.", WHM.JobID)]
    WHM_ST_MainCombo_Opener = 19023,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Movement DoT Option", "Will reapply DoT early to all enemies within range while moving.", WHM.JobID)]
    [Retargeted(WHM.Aero, WHM.Aero2, WHM.Dia)]
    WHM_ST_MainCombo_Move_DoT = 19053,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Aero/Dia Uptime Option",
        "Adds Aero/Dia to the single target combo if the debuff is not present on current target, or is about to expire.",
        WHM.JobID)]
    WHM_ST_MainCombo_DoT = 19013,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Assize Option", "Adds Assize to the single target combo.", WHM.JobID)]
    WHM_ST_MainCombo_Assize = 19009,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Glare IV Option", "Adds Glare IV to the single target combo when under Sacred Sight", WHM.JobID)]
    WHM_ST_MainCombo_GlareIV = 19015,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Afflatus Misery Option",
        "Adds Afflatus Misery to the single target combo when it is ready to be used.", WHM.JobID)]
    WHM_ST_MainCombo_Misery_oGCD = 19017,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Lily Overcap Protection Option",
        "Adds Afflatus Rapture to the single target combo when at three Lilies.", WHM.JobID)]
    WHM_ST_MainCombo_LilyOvercap = 19016,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Presence of Mind Option", "Adds Presence of Mind to the single target combo.", WHM.JobID)]
    WHM_ST_MainCombo_PresenceOfMind = 19008,

    [ParentCombo(WHM_ST_MainCombo)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming to the single target combo when below set MP value.",
        WHM.JobID)]
    WHM_ST_MainCombo_Lucid = 19006,

    #endregion

    #region Advanced AoE DPS Combo

    [AutoAction(true, false)]
    [ReplaceSkill(WHM.Holy, WHM.Holy3)]
    [ConflictingCombos(WHM_AoE_Simple_DPS)]
    [CustomComboInfo("Advanced DPS Mode - AoE", "Collection of cooldowns and spell features on Holy/Holy III.", WHM.JobID)]
    [AdvancedCombo]
    WHM_AoE_DPS = 19190,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Swift Holy Pull 'Opener' Option",
        "Adds a Swiftcast->Holy at the beginning of your AoE rotation." +
        "\nRequires you to already be in combat, to have stopped moving, and to not have used Assize yet.", WHM.JobID)]
    WHM_AoE_DPS_SwiftHoly = 19197,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Assize Option", "Adds Assize to the AoE combo.", WHM.JobID)]
    WHM_AoE_DPS_Assize = 19192,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Glare IV Option", "Adds Glare IV to the AoE combo when under Sacred Sight", WHM.JobID)]
    WHM_AoE_DPS_GlareIV = 19196,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Afflatus Misery Option", "Adds Afflatus Misery to the AoE combo when it is ready to be used.",
        WHM.JobID)]
    WHM_AoE_DPS_Misery = 19194,
    
    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Multitarget Dot Option", "Maintains dots on multiple targets.",
        WHM.JobID)]
    [Retargeted(WHM.Aero, WHM.Aero2, WHM.Dia)]
    WHM_AoE_MainCombo_DoT = 19198,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Lily Overcap Protection Option", "Adds Afflatus Rapture to the AoE combo when at three Lilies.",
        WHM.JobID)]
    WHM_AoE_DPS_LilyOvercap = 19193,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Presence of Mind Option",
        "Adds Presence of Mind to the AoE combo", WHM.JobID)]
    WHM_AoE_DPS_PresenceOfMind = 19195,

    [ParentCombo(WHM_AoE_DPS)]
    [CustomComboInfo("Lucid Dreaming Option",
        "Adds Lucid Dreaming to the AoE combo when below the set MP value if you are moving or it can be weaved without GCD delay.",
        WHM.JobID)]
    WHM_AoE_DPS_Lucid = 19191,

    #endregion

    #region Advanced Single Target Heals Combo

    [AutoAction(false, true)]
    [ReplaceSkill(WHM.Cure)]
    [CustomComboInfo("Advanced Healing Mode - Single Target", "Replaces Cure with a one button single target healing setup.",
        WHM.JobID)]
    [PossiblyRetargeted(WHM.Cure)]
    [HealingCombo]
    WHM_STHeals = 19300,
    
    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Esuna Option", "Applies Esuna to your target if there is a cleansable debuff.", WHM.JobID)]
    [PossiblyRetargeted(RoleActions.Healer.Esuna)]
    WHM_STHeals_Esuna = 19309,
    
    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Lucid Dreaming Option", "Adds Lucid Dreaming.", WHM.JobID)]
    WHM_STHeals_Lucid = 19308,
    
    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Thin Air Option", "Adds Thin Air.", WHM.JobID)]
    WHM_STHeals_ThinAir = 19304,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Regen Option", "Adds Regen.", WHM.JobID)]
    [PossiblyRetargeted(WHM.Regen)]
    WHM_STHeals_Regen = 19301,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Afflatus Solace Option", "Adds Afflatus Solace.", WHM.JobID)]
    [PossiblyRetargeted]
    WHM_STHeals_Solace = 19303,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Tetragrammaton Option", "Adds Tetragrammaton.", WHM.JobID)]
    [PossiblyRetargeted(WHM.Tetragrammaton)]
    WHM_STHeals_Tetragrammaton = 19305,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Divine Benison Option", "Adds Divine Benison.", WHM.JobID)]
    [PossiblyRetargeted(WHM.DivineBenison)]
    WHM_STHeals_Benison = 19306,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Aquaveil Option", "Adds Aquaveil.", WHM.JobID)]
    [PossiblyRetargeted(WHM.Aquaveil)]
    WHM_STHeals_Aquaveil = 19307,
    
    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Benediction Option", "Adds Benediciton.", WHM.JobID)]
    [PossiblyRetargeted]
    WHM_STHeals_Benediction = 19302,

    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Temperance Option", "Adds Temperance and it's followup Divine Caress.", WHM.JobID)]
    [PossiblyRetargeted]
    WHM_STHeals_Temperance = 19310,
    
    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("Asylum Option", "Adds Asylum.", WHM.JobID)]
    [PossiblyRetargeted]
    WHM_STHeals_Asylum = 19311,
    
    [ParentCombo(WHM_STHeals)]
    [CustomComboInfo("LiturgyOfTheBell Option", "Adds LiturgyOfTheBell.", WHM.JobID)]
    [PossiblyRetargeted]
    WHM_STHeals_LiturgyOfTheBell = 19312,

    #endregion

    #region Advanced AoE Heals Combo

    [AutoAction(true, true)]
    [ReplaceSkill(WHM.Medica1)]
    [CustomComboInfo("Advanced Healing Mode - AoE", "Replaces Medica with a one button AoE healing setup.", WHM.JobID)]
    [HealingCombo]
    WHM_AoEHeals = 19007,
    
    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Lucid Dreaming Option", "Uses Lucid Dreaming when available.", WHM.JobID)]
    WHM_AoEHeals_Lucid = 19204,
    
    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Thin Air Option", "Uses Thin Air when available.", WHM.JobID)]
    WHM_AoEHeals_ThinAir = 19200,
    
    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Afflatus Misery Option", "Uses Afflatus Misery when available.", WHM.JobID)]
    WHM_AoEHeals_Misery = 19010,
    
    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Medica II Option",
        "Uses Medica II when heal target doesn't have Medica II buff." +
        "\nUpgrades to Medica III when level allows.", WHM.JobID)]
    WHM_AoEHeals_Medica2 = 19205,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Afflatus Rapture Option", "Uses Afflatus Rapture when available.", WHM.JobID)]
    WHM_AoEHeals_Rapture = 19011,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Cure III Option", "Replaces Medica with Cure III when available.", WHM.JobID)]
    WHM_AoEHeals_Cure3 = 19201,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Assize Option", "Uses Assize when available.", WHM.JobID)]
    WHM_AoEHeals_Assize = 19202,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Plenary Indulgence Option", "Uses Plenary Indulgence when available.", WHM.JobID)]
    WHM_AoEHeals_Plenary = 19203,
    
    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Asylum Option", "Adds Asylum placement, when standing still, to the rotation.\nWill Retarget it onto yourself.", WHM.JobID)]
    [Retargeted]
    WHM_AoEHeals_Asylum = 19028,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Temperance Option", "Uses Temperance when available for a healing boost whenever the party average falls below the set threshold.", WHM.JobID)]
    WHM_AoEHeals_Temperance = 19210,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Divine Caress", "Uses Divine Caress when Divine Grace from Temperance is active.", WHM.JobID)]
    WHM_AoEHeals_DivineCaress = 19207,

    [ParentCombo(WHM_AoEHeals)]
    [CustomComboInfo("Liturgy of the Bell Option", "Adds Liturgy of the Bell (Lilybell) placement to the rotation.", WHM.JobID)]
    [Retargeted(WHM.LiturgyOfTheBell)]
    WHM_AoEHeals_LiturgyOfTheBell = 19206,
    
    #endregion
    
    #region Mitigation Features

    [ReplaceSkill(WHM.Aquaveil)]
    [CustomComboInfo("Mitigation Feature - Single Target", "Changes Aquaveil into Tetragrammaton and/or Divine Benison after use.\nEach action can be Retargeted with the Retargeting Features below.", WHM.JobID)]
    [PossiblyRetargeted("Retargeting Features below, Enable Aquaveil (and optionally Tetra and Benison)", Condition.WHMRetargetingFeaturesEnabledForSTMit)]
    WHM_Mit_ST = 19041,
    
    [ReplaceSkill(WHM.Asylum)]
    [CustomComboInfo("Mitigation Feature - AoE", "Changes Asylum into Temperance and then Divine Caress after use.\nCan be Retargeted with the Retargeting Features below.", WHM.JobID)]
    [PossiblyRetargeted("Retargeting Features below, Enable Asylum", Condition.WHMRetargetingFeaturesEnabledForAoEMit)]
    WHM_Mit_AoE = 19040,

    #endregion
    
    #region Raidwide Heals
    
    [CustomComboInfo("Boss Raidwide Options",
        "Collection of tools designed to try and cast during a raidwide attack when detected." +
        "\nThis will work for most, but not all raidwide attacks and is no substitute for learning the fight", WHM.JobID)]
    WHM_Raidwide = 19220,
    
    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo("RaidWide Asylum Option", "Will try to Weave Asylum when a raidwide casting. \nWill be used in all 4 main combos.", WHM.JobID)]
    WHM_Raidwide_Asylum = 19221,
    
    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo("RaidWide Temperance Combo Option",
        "Will try to Weave Temperance and Divine Caress when a raidwide casting. " +
        "\nWill be used in all 4 main combos.", WHM.JobID)]
    WHM_Raidwide_Temperance = 19222,
    
    [ParentCombo(WHM_Raidwide)]
    [CustomComboInfo("RaidWide LiturgyOfTheBell Option",
        "Will try to weave LiturgyOfTheBell when a raidwide casting. " +
        "\nWill be used in all 4 main combos.", WHM.JobID)]
    WHM_Raidwide_LiturgyOfTheBell = 19223,
    
    #endregion
    
    #region Small Features
    
    [ReplaceSkill(WHM.AfflatusSolace)]
    [CustomComboInfo("Solace into Misery Feature",
        "Replaces Afflatus Solace with Afflatus Misery when it is ready to be used.\nSolace can be Retargeted with the Retargeting Features below.", WHM.JobID)]
    [PossiblyRetargeted("Retargeting Features below, Enable Afflatus Solace", 
        Condition.WHMRetargetingFeaturesEnabledForSolace)]
    WHM_SolaceMisery = 19000,

    [ReplaceSkill(WHM.AfflatusRapture)]
    [CustomComboInfo("Rapture into Misery Feature",
        "Replaces Afflatus Rapture with Afflatus Misery when it is ready to be used.", WHM.JobID)]
    WHM_RaptureMisery = 19001,

    [ReplaceSkill(WHM.Cure2)]
    [CustomComboInfo("Cure II Sync Feature", "Changes Cure II to Cure when synced below Lv.30.\nCan be Retargeted with the Retargeting Features below.", WHM.JobID)]
    [PossiblyRetargeted("Retargeting Features below, Enable Cure", Condition.WHMRetargetingFeaturesEnabledForCure)]
    WHM_CureSync = 19002,

    [ReplaceSkill( RoleActions.Magic.Swiftcast)]
    [ConflictingCombos(ALL_Healer_Raise)]
    [CustomComboInfo("Alternative Raise Feature", "Changes Swiftcast to Raise.", WHM.JobID)]
    WHM_Raise = 19004,

    [ParentCombo(WHM_Raise)]
    [CustomComboInfo("Retarget Raise", "Will Retarget the Raise affected here to your Raise Stack.", WHM.JobID)]
    [Retargeted(WHM.Raise)]
    WHM_Raise_Retarget = 19029,

    [ReplaceSkill(WHM.Raise)]
    [CustomComboInfo("Thin Air Raise Feature", "Adds Thin Air to the Global Raise Feature/Alternative Raise Feature.",
        WHM.JobID)]
    WHM_ThinAirRaise = 19014,

    #endregion

    #region Retargeting

    [CustomComboInfo("Retargeting Features", "Collection of Options to Retarget Manually-Used Single Target Heals.", WHM.JobID)]
    WHM_Retargets = 19037,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Aquaveil)]
    [CustomComboInfo("Cure Option", "Retargets Cure and Cure II to the heal stack (even from the Cure II Sync Feature above).", WHM.JobID)]
    [Retargeted(WHM.Cure, WHM.Cure2)]
    WHM_Re_Cure = 19038,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Aquaveil)]
    [CustomComboInfo("Afflatus Solace Option", "Retargets Afflatus Solace to the heal stack (even from the Solace into Misery Feature above).", WHM.JobID)]
    [Retargeted(WHM.AfflatusSolace)]
    WHM_Re_Solace = 19039,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Aquaveil)]
    [CustomComboInfo("Aquaveil Option", "Retargets Aquaveil to the heal stack (even from the Mitigation Feature above).", WHM.JobID)]
    [Retargeted(WHM.Aquaveil)]
    WHM_Re_Aquaveil = 19036,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Asylum)]
    [CustomComboInfo("Asylum Option", "Retargets Asylum to yourself (even from the Mitigation Feature above).", WHM.JobID)]
    [Retargeted(WHM.Asylum)]
    WHM_Re_Asylum = 19027,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.LiturgyOfTheBell)]
    [CustomComboInfo("Liturgy Of The Bell Option", "Retargets Liturgy Of The Bell to yourself.", WHM.JobID)]
    [Retargeted(WHM.LiturgyOfTheBell)]
    WHM_Re_LiturgyOfTheBell = 19030,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Cure3)]
    [CustomComboInfo("Cure 3 Retarget Option", "Retargets Cure 3 to the heal stack.", WHM.JobID)]
    [Retargeted(WHM.Cure3)]
    WHM_Re_Cure3 = 19031,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Benediction)]
    [CustomComboInfo("Benediction Option", "Retargets Benediction to the heal stack.", WHM.JobID)]
    [Retargeted(WHM.Benediction)]
    WHM_Re_Benediction = 19032,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Tetragrammaton)]
    [CustomComboInfo("Tetragrammaton Option", "Retargets Tetragrammaton to the heal stack (even from the Mitigation Feature above).", WHM.JobID)]
    [Retargeted(WHM.Tetragrammaton)]
    WHM_Re_Tetragrammaton = 19033,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.Regen)]
    [CustomComboInfo("Regen Option", "Retargets Regen to the heal stack.", WHM.JobID)]
    [Retargeted(WHM.Regen)]
    WHM_Re_Regen = 19034,
    
    [ParentCombo(WHM_Retargets)]
    [ReplaceSkill(WHM.DivineBenison)]
    [CustomComboInfo("Divine Benison Option", "Retargets Divine Benison to the heal stack (even from the Mitigation Feature above).", WHM.JobID)]
    [Retargeted(WHM.DivineBenison)]
    WHM_Re_DivineBenison = 19035,

    #endregion

    #region Variants

    [Variant]
    [VariantParent(WHM_ST_MainCombo_DoT, WHM_AoE_DPS)]
    [CustomComboInfo("Spirit Dart Option",
        "Use Variant Spirit Dart whenever the debuff is not present or less than 3s.", WHM.JobID)]
    WHM_DPS_Variant_SpiritDart = 19025,

    [Variant]
    [VariantParent(WHM_ST_MainCombo, WHM_AoE_DPS)]
    [CustomComboInfo("Rampart Option", "Use Variant Rampart on cooldown.", WHM.JobID)]
    WHM_DPS_Variant_Rampart = 19026,

    #endregion

    // Last value = 19051 (then skips to next last used: 19210)

    #endregion

    // Non-combat

    #region DOH

    // [CustomComboInfo("Placeholder", "Placeholder.", DOH.JobID)]
    // DohPlaceholder = 50001,

    #endregion

    #region DOL

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.AgelessWords, DOL.SolidReason)]
    [CustomComboInfo("[BTN/MIN] Eureka Feature",
        "Replaces Ageless Words and Solid Reason with Wise to the World when available", DOL.JobID)]
    DOL_Eureka = 51001,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.ArborCall, DOL.ArborCall2, DOL.LayOfTheLand, DOL.LayOfTheLand2)]
    [CustomComboInfo("[BTN/MIN] Locate & Truth Feature",
        "Replaces Lay of the Lands or Arbor Calls with Prospect/Triangulate and Truth of Mountains/Forests if not active.",
        DOL.JobID)]
    DOL_NodeSearchingBuffs = 51012,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Cast)]
    [CustomComboInfo("[FSH] Cast to Hook Feature", "Replaces Cast with Hook when fishing", DOL.JobID)]
    FSH_CastHook = 51002,

    [Role(JobRole.DoL)]
    [CustomComboInfo("[FSH] Diving Feature", "Replace fishing abilities with diving abilities when underwater",
        DOL.JobID)]
    FSH_Swim = 51008,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Cast)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("[FSH] Cast to Gig Option", "Replaces Cast with Gig when diving.", DOL.JobID)]
    FSH_CastGig = 51003,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.SurfaceSlap)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Surface Slap to Veteran Trade Option", "Replaces Surface Slap with Veteran Trade when diving.",
        DOL.JobID)]
    FSH_SurfaceTrade = 51004,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.PrizeCatch)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Prize Catch to Nature's Bounty Option", "Replaces Prize Catch with Nature's Bounty when diving.",
        DOL.JobID)]
    FSH_PrizeBounty = 51005,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Snagging)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Snagging to Salvage Option", "Replaces Snagging with Salvage when diving.", DOL.JobID)]
    FSH_SnaggingSalvage = 51006,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.CastLight)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Cast Light to Electric Current Option", "Replaces Cast Light with Electric Current when diving.",
        DOL.JobID)]
    FSH_CastLight_ElectricCurrent = 51007,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Mooch, DOL.MoochII)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Mooch to Shark Eye Option", "Replaces Mooch with Shark Eye when diving.", DOL.JobID)]
    FSH_Mooch_SharkEye = 51009,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.FishEyes)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Fish Eyes to Vital Sight Option", "Replaces Fish Eyes with Vital Sight when diving.", DOL.JobID)]
    FSH_FishEyes_VitalSight = 51010,

    [Role(JobRole.DoL)]
    [ReplaceSkill(DOL.Chum)]
    [ParentCombo(FSH_Swim)]
    [CustomComboInfo("Chum to Baited Breath Option", "Replaces Chum with Baited Breath when diving.", DOL.JobID)]
    FSH_Chum_BaitedBreath = 51011,

    // Last value = 51011

    #endregion

    #endregion

    #region PvP Combos

    #region PvP GLOBAL FEATURES

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo("Emergency Heals Feature",
        "Uses Recuperate when your HP is under the set threshold and you have sufficient MP.", ADV.JobID)]
    PvP_EmergencyHeals = 1100000,

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo("Emergency Guard Feature", "Uses Guard when your HP is under the set threshold.", ADV.JobID)]
    PvP_EmergencyGuard = 1100010,

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo("Quick Purify Feature", "Uses Purify when afflicted with any selected debuff.", ADV.JobID)]
    PvP_QuickPurify = 1100020,

    [Role(JobRole.All)]
    [PvPCustomCombo]
    [CustomComboInfo("Prevent Mash Cancelling Feature",
        "Stops you cancelling your guard if you're pressing buttons quickly by replacing your buttons with Savage Blade.", ADV.JobID)]
    PvP_MashCancel = 1100030,

    [Role(JobRole.All)]
    [ParentCombo(PvP_MashCancel)]
    [CustomComboInfo("Recuperate Option",
        "Allows you to cancel your guard with Recuperate on the Guard button if health is low enough to not waste it.",
        ADV.JobID)]
    PvP_MashCancelRecup = 1100031,

    // Last value = 1100030
    // Extra 0 on the end keeps things working the way they should be. Nothing to see here.

    #endregion

    #region ASTROLOGIAN

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.Malefic)]
    [CustomComboInfo("Burst Mode", "Turns Fall Malefic into an all-in-one damage button.", ASTPvP.JobID)]
    ASTPvP_Burst = 111000,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo("Card Draw Option", "Adds Drawing Cards to Burst Mode.", ASTPvP.JobID)]
    ASTPvP_Burst_DrawCard = 111002,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo("Card Play Option", "Adds Playing Cards to Burst Mode.", ASTPvP.JobID)]
    ASTPvP_Burst_PlayCard = 111003,

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.AspectedBenefic)]
    [CustomComboInfo("Double Cast Heal Feature", "Adds Double Cast to Aspected Benefic.", ASTPvP.JobID)]
    ASTPvP_Heal = 111004,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo("Double Malefic Cast Option", "Adds Double Malefic Cast to Burst Mode.", ASTPvP.JobID)]
    ASTPvP_Burst_DoubleMalefic = 111005,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst_Gravity)]
    [CustomComboInfo("Double Gravity Cast Option", "Adds Double Gravity Cast to Burst Mode.", ASTPvP.JobID)]
    ASTPvP_Burst_DoubleGravity = 111009,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo("Gravity Burst Option", "Adds Gravity Cast to Burst Mode.", ASTPvP.JobID)]
    ASTPvP_Burst_Gravity = 111006,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo("Macrocosmos Option",
        "Adds Macrocosmos to Burst Mode. \n If Double Gravity is enabled, it will hold Macrocosmos for the double gravity burst.",
        ASTPvP.JobID)]
    ASTPvP_Burst_Macrocosmos = 111007,

    [PvPCustomCombo]
    [ParentCombo(ASTPvP_Burst)]
    [CustomComboInfo("Role Action Diabrosis Option",
        "Adds Role Action Diabrosis to Burst Mode below selected health",
        ASTPvP.JobID)]
    ASTPvP_Diabrosis = 111010,

    [PvPCustomCombo]
    [ReplaceSkill(ASTPvP.Epicycle)]
    [CustomComboInfo("Epicycle Burst Feature", "Turns Epicycle into burst combo.", ASTPvP.JobID)]
    ASTPvP_Epicycle = 111008,

    // Last value = 111010

    #endregion

    #region BLACK MAGE

    [PvPCustomCombo]
    [ReplaceSkill(BLMPvP.Fire, BLMPvP.Blizzard)]
    [CustomComboInfo("Burst Mode", "Turns Fire into an all-in-one button.\n- Uses Blizzard spells while moving (One Button Mode only).\n- Will use Paradox when appropriate.", BLMPvP.JobID)]
    BLMPvP_BurstMode = 112000,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Burst Option", "Uses Burst when available.\n- Requires target to be within range.", BLMPvP.JobID)]
    BLMPvP_Burst = 112001,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Xenoglossy Option", "Uses Xenoglossy when available.\n- Requires target's HP to be under:", BLMPvP.JobID)]
    BLMPvP_Xenoglossy = 112002,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Lethargy Option", "Uses Lethargy when available.\n- Will not use against non-players.\n- Requires target's HP to be under:", BLMPvP.JobID)]
    BLMPvP_Lethargy = 112003,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Elemental Weave Option", "Uses Wreath of Fire when available.\n- Requires player's HP to be at or above:", BLMPvP.JobID)]
    BLMPvP_ElementalWeave = 112004,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Elemental Star Option", "Uses Elemental Star when available.\n- Prioritizes Flare Star unless expiring.", BLMPvP.JobID)]
    BLMPvP_ElementalStar = 112005,

    [ParentCombo(BLMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Phantom Dart Option", "Uses Phantom Dart (if selected) when available at or below set health threshold.", BLMPvP.JobID)]
    BLMPvP_PhantomDart = 112007,

    [PvPCustomCombo]
    [ReplaceSkill(BLMPvP.AetherialManipulation)]
    [CustomComboInfo("Aetherial Manipulation Feature", "Adds Purify when affected by crowd control.\n- Requires Purify to be available.", BLMPvP.JobID)]
    BLMPvP_Manipulation_Feature = 112006,


    // Last value = 112007

    #endregion

    #region BARD

    [PvPCustomCombo]
    [ReplaceSkill(BRDPvP.PowerfulShot)]
    [CustomComboInfo("Burst Mode", "Turns Powerful Shot into an all-in-one damage button.", BRDPvP.JobID)]
    BRDPvP_BurstMode = 113000,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Silent Nocturne Option", "Adds Silent Nocturne to Burst Mode.", BRDPvP.JobID)]
    BRDPvP_SilentNocturne = 113001,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Apex Arrow Option", "Adds Apex Arrow to Burst Mode.", BRDPvP.JobID)]
    BRDPvP_ApexArrow = 113002,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Blast Arrow Option", "Adds Blast Arrow to Burst Mode.", BRDPvP.JobID)]
    BRDPvP_BlastArrow = 113003,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Harmonic Arrow Option",
        "Adds Harmonic Arrow to Burst Mode. Will use it at set number of charges AND when target is below the health threshold per charge for execute. ",
        BRDPvP.JobID)]
    BRDPvP_HarmonicArrow = 113004,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Encore of Light Option", "Adds Encore of Light to Burst Mode.", BRDPvP.JobID)]
    BRDPvP_EncoreOfLight = 113005,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Wardens Paeon Option", "Auto Self cleanse of soft cc. \n Half Asleep, Heavy, and Bind",
        BRDPvP.JobID)]
    BRDPvP_Wardens = 113006,

    [PvPCustomCombo]
    [ParentCombo(BRDPvP_BurstMode)]
    [CustomComboInfo("Role Action Eagle Eye Shot Option", "Automatically Adds Eagle Eye Shot to Burst Mode when target is guarded or under selected health percentage \n WILL ONLY WORK IN LARGE SCALE PVP",
        BRDPvP.JobID)]
    BRDPvP_Eagle = 113007,

    // Last value = 113007

    #endregion

    #region DANCER

    [PvPCustomCombo]
    [ReplaceSkill(DNCPvP.Fountain)]
    [CustomComboInfo("Burst Mode", "Turns Fountain Combo into an all-in-one damage button.", DNCPvP.JobID)]
    DNCPvP_BurstMode = 114000,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo("Honing Dance Option",
        "Adds Honing Dance to the main combo when in melee range (respects global offset).\nThis option prevents early use of Honing Ovation!\nKeep Honing Dance bound to another key if you want to end early.",
        DNCPvP.JobID)]
    DNCPvP_BurstMode_HoningDance = 114001,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo("Curing Waltz Option",
        "Adds Curing Waltz to the combo when available, and your HP is at or below the set percentage.", DNCPvP.JobID)]
    DNCPvP_BurstMode_CuringWaltz = 114002,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo("Dance Partner Reminder Option", "Adds Closed Position reminder when you have none", DNCPvP.JobID)]
    DNCPvP_BurstMode_Partner = 114003,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo("En Avant Option", "Uses En Avant if available and buff is missing to boost 1 2 combo damage.",
        DNCPvP.JobID)]
    DNCPvP_BurstMode_Dash = 114004,

    [PvPCustomCombo]
    [ParentCombo(DNCPvP_BurstMode)]
    [CustomComboInfo("Role Action Eagle Eye Shot Option", "Automatically Adds Eagle Eye Shot to Burst Mode when target is guarded or under selected health percentage \n WILL ONLY WORK IN LARGE SCALE PVP",
       DNCPvP.JobID)]
    DNCPvP_Eagle = 114005,

    // Last value = 114005

    #endregion

    #region DARK KNIGHT

    [PvPCustomCombo]
    [ReplaceSkill(DRKPvP.Souleater)]
    [CustomComboInfo("Burst Mode", "Turns Souleater Combo into an all-in-one damage button.", DRKPvP.JobID)]
    DRKPvP_Burst = 115000,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Plunge Option", "Adds Plunge to Burst Mode.", DRKPvP.JobID)]
    DRKPvP_Plunge = 115001,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Plunge)]
    [CustomComboInfo("Melee Plunge Option", "Uses Plunge whilst in melee range, and not just as a gap-closer.",
        DRKPvP.JobID)]
    DRKPvP_PlungeMelee = 115002,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Salted Earth Option", "Adds Salted Earth to Burst mode.", DRKPvP.JobID)]
    DRKPvP_SaltedEarth = 115003,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Salt and Darkness Option", "Adds Salt and Darkness to Burst mode.", DRKPvP.JobID)]
    DRKPvP_SaltAndDarkness = 115004,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Impalement Option", "Adds Impalement to Burst mode.", DRKPvP.JobID)]
    DRKPvP_Impalement = 115005,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Shadowbringer Option", "Adds Shadowbringer to Burst mode.", DRKPvP.JobID)]
    DRKPvP_Shadowbringer = 115006,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Blackest Night Option", "Adds Blackest Night to Burst mode.", DRKPvP.JobID)]
    DRKPvP_BlackestNight = 115007,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Scorn Option", "Adds Scorn to Burst mode.", DRKPvP.JobID)]
    DRKPvP_Scorn = 115008,

    [PvPCustomCombo]
    [ParentCombo(DRKPvP_Burst)]
    [CustomComboInfo("Role Action Rampart Option",
        "Adds Defensive Role Action Rampart to Burst Mode below selected health", DRK.JobID)]
    DRKPvP_Rampart = 115009,

    // Last value = 115009

    #endregion

    #region DRAGOON

    [PvPCustomCombo]
    [ReplaceSkill(DRGPvP.Drakesbane)]
    [CustomComboInfo("Burst Mode", "Turns Drakesbane Combo into an all-in-one damage button.", DRGPvP.JobID)]
    DRGPvP_Burst = 116000,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Geirskogul Option", "Adds Geirskogul to Burst Mode.", DRGPvP.JobID)]
    DRGPvP_Geirskogul = 116001,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Geirskogul)]
    [CustomComboInfo("Nastrond Option", "Adds Nastrond to Burst Mode.", DRGPvP.JobID)]
    DRGPvP_Nastrond = 116002,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Horrid Roar Option", "Adds Horrid Roar to Burst Mode.", DRGPvP.JobID)]
    DRGPvP_HorridRoar = 116003,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Sustain Chaos Spring Option", "Adds Chaos Spring to Burst Mode when below the set HP percentage.",
        DRGPvP.JobID)]
    DRGPvP_ChaoticSpringSustain = 116004,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Execute Chaos Spring Option",
        "Adds Chaos Spring to Burst Mode when target is below 8k health because it goes through guard.", DRGPvP.JobID)]
    DRGPvP_ChaoticSpringExecute = 116009,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Wyrmwind Thrust Option", "Adds Wyrmwind Thrust to Burst Mode.", DRGPvP.JobID)]
    DRGPvP_WyrmwindThrust = 116006,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("High Jump Weave Option", "Adds High Jump to Burst Mode.", DRGPvP.JobID)]
    DRGPvP_HighJump = 116007,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Elusive Jump Burst Option",
        "Using Elusive Jump turns Drakesbane Combo into all-in-one burst damage button once all cooldowns are ready. \n Disables Elusive Jump if Burst is not ready.",
        DRGPvP.JobID)]
    DRGPvP_BurstProtection = 116008,

    [PvPCustomCombo]
    [ParentCombo(DRGPvP_Burst)]
    [CustomComboInfo("Role Action Smite Option",
        "Adds Role Action Smite to Burst Mode below selected health", DRGPvP.JobID)]
    DRGPvP_Smite = 116010,

    // Last value = 116010

    #endregion

    #region GUNBREAKER

    #region Burst Mode

    [PvPCustomCombo]
    [ReplaceSkill(GNBPvP.SolidBarrel)]
    [CustomComboInfo("Burst Mode", "Turns Solid Barrel Combo into an all-in-one damage button.", GNBPvP.JobID)]
    GNBPvP_Burst = 117000,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Fated Circle Option", "Adds Fated Circle to rotation under No Mercy status.", GNBPvP.JobID)]
    GNBPvP_FatedCircle = 117001,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Gnashing Fang Option", "Adds Gnashing Fang to Burst Mode.", GNBPvP.JobID)]
    GNBPvP_ST_GnashingFang = 117004,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Continuation Option", "Adds Continuation to Burst Mode.", GNBPvP.JobID)]
    GNBPvP_ST_Continuation = 117005,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Rough Divide Option", "Adds Rough Divide to rotation when appropriate.", GNBPvP.JobID)]
    GNBPvP_RoughDivide = 117006,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Blasting Zone Option", "Adds Blasting Zone to Burst Mode when under Threshold.", GNBPvP.JobID)]
    GNBPvP_BlastingZone = 117007,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Heart of Corundum Option", "Adds Heart of Corundum to Burst Mode under set health %.", GNBPvP.JobID)]
    GNBPvP_Corundum = 117011,

    [PvPCustomCombo]
    [ParentCombo(GNBPvP_Burst)]
    [CustomComboInfo("Role Action Rampart Option",
        "Adds Defensive Role Action Rampart to Burst Mode below selected health", GNBPvP.JobID)]
    GNBPvP_Rampart = 117012,

    #endregion

    #region Option Select

    [PvPCustomCombo]
    [ReplaceSkill(GNBPvP.GnashingFang)]
    [CustomComboInfo("Continuation Feature", "Adds Continuation to Gnashing Fang.", GNBPvP.JobID)]
    GNBPvP_GnashingFang = 117010,

    // Last value = 117012

    #endregion

    #endregion

    #region MACHINIST

    [PvPCustomCombo]
    [ReplaceSkill(MCHPvP.BlastCharge)]
    [CustomComboInfo("Burst Mode", "Turns Blast Charge into an all-in-one damage button.", MCHPvP.JobID)]
    MCHPvP_BurstMode = 118000,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Air Anchor Option", "Adds Air Anchor to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_AirAnchor = 118001,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Analysis Option", "Adds Analysis to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_Analysis = 118002,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode_Analysis)]
    [CustomComboInfo("Alternate Analysis Option", "Uses Analysis with Air Anchor instead of Chain Saw.", MCHPvP.JobID)]
    MCHPvP_BurstMode_AltAnalysis = 118003,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Drill Option", "Adds Drill to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_Drill = 118004,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode_Drill)]
    [CustomComboInfo("Alternate Drill Option", "Saves Drill for use after Wildfire.", MCHPvP.JobID)]
    MCHPvP_BurstMode_AltDrill = 118005,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Bio Blaster Option", "Adds Bio Blaster to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_BioBlaster = 118006,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Chain Saw Option", "Adds Chain Saw to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_ChainSaw = 118007,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Full Metal Field Option", "Adds Full Metal Field to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_FullMetalField = 118008,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Wildfire Option", "Adds Wildfire to Burst Mode.", MCHPvP.JobID)]
    MCHPvP_BurstMode_Wildfire = 118009,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Marksmans Spite Option",
        "Adds Marksmans Spite to Burst Mode when the target is below specified HP.", MCHPvP.JobID)]
    MCHPvP_BurstMode_MarksmanSpite = 118011,

    [PvPCustomCombo]
    [ParentCombo(MCHPvP_BurstMode)]
    [CustomComboInfo("Role Action Eagle Eye Shot Option", "Automatically Adds Eagle Eye Shot to Burst Mode when target is guarded or under selected health percentage \n WILL ONLY WORK IN LARGE SCALE PVP",
       MCHPvP.JobID)]
    MCHPvP_Eagle = 118012,

    // Last value = 118012

    #endregion

    #region MONK

    [PvPCustomCombo]
    [ReplaceSkill(MNKPvP.PhantomRush)]
    [CustomComboInfo("Burst Mode", "Turns Phantom Rush Combo into an all-in-one damage button.", MNKPvP.JobID)]
    MNKPvP_Burst = 119000,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Meteodrive Option",
        "Adds Meteodrive Limit break to Burst Mode when target is below 20k and guarded", MNKPvP.JobID)]
    MNKPvP_Burst_Meteodrive = 119006,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Thunderclap Option", "Adds Thunderclap to Burst Mode when not buffed with Wind Resonance.",
        MNKPvP.JobID)]
    MNKPvP_Burst_Thunderclap = 119001,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Riddle of Earth Option", "Adds Riddle of Earth and Earth's Reply to Burst Mode when in combat.",
        MNKPvP.JobID)]
    MNKPvP_Burst_RiddleOfEarth = 119002,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Flints Reply Option", "Adds Flints Reply to Burst Mode.", MNKPvP.JobID)]
    MNKPvP_Burst_FlintsReply = 119003,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Rising Phoenix Option", "Adds Rising Phoenix to Burst Mode.", MNKPvP.JobID)]
    MNKPvP_Burst_RisingPhoenix = 119004,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Wind's Reply Option", "Adds Wind's Reply to Burst Mode.", MNKPvP.JobID)]
    MNKPvP_Burst_WindsReply = 119005,

    [ParentCombo(MNKPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Role Action Smite Option",
        "Adds Role Action Smite to Burst Mode below selected health", MNKPvP.JobID)]
    MNKPvP_Smite = 119007,

    // Last value = 119007

    #endregion

    #region NINJA

    [PvPCustomCombo]
    [ReplaceSkill(NINPvP.AeolianEdge)]
    [CustomComboInfo("Burst Mode", "Turns Aeolian Edge Combo into an all-in-one damage button.", NINPvP.JobID)]
    NINPvP_ST_BurstMode = 120000,

    [PvPCustomCombo]
    [ReplaceSkill(NINPvP.FumaShuriken)]
    [CustomComboInfo("AoE Burst Mode", "Turns Fuma Shuriken into an all-in-one AoE damage button.", NINPvP.JobID)]
    NINPvP_AoE_BurstMode = 120001,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Meisui Option", "Uses Three Mudra on Meisui when HP is under the set threshold.", NINPvP.JobID)]
    NINPvP_ST_Meisui = 120002,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Automatic Mudra Mode", "Uses the mudra from three mudra, automatically on ST burst mode. " +
                                             "\n Will use Hyosho Ranryu > Forked Raiju IF YOU HAVE BUNSHIN STACKS > Huton",
        NINPvP.JobID)]
    NINPvP_ST_MudraMode = 120013,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Fuma Shuriken Option", "Adds Fuma Shuriken to Burst Mode.", NINPvP.JobID)]
    NINPvP_ST_FumaShuriken = 120003,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Three Mudra Option", "Adds Three Mudra to Burst Mode.", NINPvP.JobID)]
    NINPvP_ST_ThreeMudra = 120004,

    [ParentCombo(NINPvP_ST_ThreeMudra)]
    [PvPCustomCombo]
    [CustomComboInfo("Three Mudra Pooling Option", "Saves Both charges for when Bunshin is up for burst", NINPvP.JobID)]
    NINPvP_ST_ThreeMudraPool = 120014,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Dokumori Option", "Adds Dokumori to Burst Mode.", NINPvP.JobID)]
    NINPvP_ST_Dokumori = 120005,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Bunshin Option", "Adds Bunshin to Burst Mode.", NINPvP.JobID)]
    NINPvP_ST_Bunshin = 120006,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Seiton Tenchu Option", "Adds SeitonTenchu to Burst Mode when the target is below threshold HP%.",
        NINPvP.JobID)]
    NINPvP_ST_SeitonTenchu = 120007,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Meisui Option", "Uses Three Mudra on Meisui when HP is under the set threshold.", NINPvP.JobID)]
    NINPvP_AoE_Meisui = 120008,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Automatic Mudra Mode", "Uses the mudra from three mudra, automatically on AoE burst mode. " +
                                             "\n Will use Doton > GokaMekkyaku", NINPvP.JobID)]
    NINPvP_AoE_MudraMode = 120016,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Fuma Shuriken Option", "Adds Fuma Shuriken to Burst Mode.", NINPvP.JobID)]
    NINPvP_AoE_FumaShuriken = 120009,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Three Mudra Option", "Adds Three Mudra to Burst Mode.", NINPvP.JobID)]
    NINPvP_AoE_ThreeMudra = 120010,

    [ParentCombo(NINPvP_AoE_ThreeMudra)]
    [PvPCustomCombo]
    [CustomComboInfo("Three Mudra Pooling Option", "Saves Both charges for when Bunshin is up for burst", NINPvP.JobID)]
    NINPvP_AoE_ThreeMudraPool = 120015,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Dokumori Option", "Adds Dokumori to Burst Mode.", NINPvP.JobID)]
    NINPvP_AoE_Dokumori = 120011,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Bunshin Option", "Adds Bunshin to Burst Mode.", NINPvP.JobID)]
    NINPvP_AoE_Bunshin = 120012,

    [ParentCombo(NINPvP_AoE_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Seiton Tenchu Option", "Adds SeitonTenchu to Burst Mode when the target is below threshold HP%.",
        NINPvP.JobID)]
    NINPvP_AoE_SeitonTenchu = 120017,

    [ParentCombo(NINPvP_ST_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Role Action Smite Option",
      "Adds Role Action Smite to Burst Mode below selected health",
      NINPvP.JobID)]
    NINPvP_Smite = 120018,

    // Last value = 120018

    #endregion

    #region PALADIN

    [PvPCustomCombo]
    [ReplaceSkill(PLDPvP.RoyalAuthority)]
    [CustomComboInfo("Burst Mode", "Turns Royal Authority Combo into an all-in-one damage button.", PLDPvP.JobID)]
    PLDPvP_Burst = 121000,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Shield Smite Option", "Adds Shield Smite to Burst Mode.", PLDPvP.JobID)]
    PLDPvP_ShieldSmite = 121001,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Imperator Option", "Adds Imperator to Burst Mode.", PLDPvP.JobID)]
    PLDPvP_Imperator = 121002,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Confiteor Option", "Adds Confiteor to Burst Mode.", PLDPvP.JobID)]
    PLDPvP_Confiteor = 121003,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Holy Spirit Option", "Adds Holy Spirit to Burst Mode.", PLDPvP.JobID)]
    PLDPvP_HolySpirit = 121004,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Intervene Option", "Adds Intervene to Burst Mode.", PLDPvP.JobID)]
    PLDPvP_Intervene = 121005,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Melee Intervene Option", "Adds Intervene to Burst Mode when in melee range.", PLDPvP.JobID)]
    PLDPvP_Intervene_Melee = 121006,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Phalanx Combo Option", "Adds Phalanx Combo to Burst Mode.", PLDPvP.JobID)]
    PLDPvP_PhalanxCombo = 121007,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Holy Sheltron Option", "Adds Holy Sheltron to Burst Mode in melee range.", PLDPvP.JobID)]
    PLDPvP_Sheltron = 121008,

    [PvPCustomCombo]
    [ParentCombo(PLDPvP_Burst)]
    [CustomComboInfo("Role Action Rampart Option",
        "Adds Defensive Role Action Rampart to Burst Mode below selected health", PLDPvP.JobID)]
    PLDPvP_Rampart = 121009,

    // Last value = 121009

    #endregion

    #region PICTOMANCER

    [PvPCustomCombo]
    [ReplaceSkill(PCTPvP.FireInRed)]
    [CustomComboInfo("Burst Mode", "Turns Fire in Red into an all-in-one damage button.", PCTPvP.JobID)]
    PCTPvP_Burst = 140000,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Burst Control Option",
        "Saves high-damaging actions until the target's HP falls below the threshold.", PCTPvP.JobID)]
    PCTPvP_BurstControl = 140001,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Tempera Coat Option", "Uses Tempera Coat when HP falls below the threshold during combat.",
        PCTPvP.JobID)]
    PCTPvP_TemperaCoat = 140002,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Smart Palette Option",
        "Uses Subtractive Palette when standing still and releases it when moving.", PCTPvP.JobID)]
    PCTPvP_SubtractivePalette = 140003,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Creature Motif Option", "Adds Creature Motif to Burst Mode.", PCTPvP.JobID)]
    PCTPvP_CreatureMotif = 140004,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Living Muse Option", "Adds Living Muse to Burst Mode.", PCTPvP.JobID)]
    PCTPvP_LivingMuse = 140005,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Mog Of The Ages Option", "Adds Mog Of The Ages to Burst Mode.", PCTPvP.JobID)]
    PCTPvP_MogOfTheAges = 140006,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Holy In White Option", "Adds Holy In White to Burst Mode.", PCTPvP.JobID)]
    PCTPvP_HolyInWhite = 140007,

    [PvPCustomCombo]
    [ParentCombo(PCTPvP_Burst)]
    [CustomComboInfo("Star Prism Option", "Adds Star Prism to Burst Mode.", PCTPvP.JobID)]
    PCTPvP_StarPrism = 140008,

    [ParentCombo(PCTPvP_Burst)]
    [PvPCustomCombo]
    [CustomComboInfo("Phantom Dart Option", "Uses Phantom Dart (if selected) when available at or below set health threshold.", PCTPvP.JobID)]
    PCTPvP_PhantomDart = 140009,

    // Last value = 140009

    #endregion

    #region REAPER

    [PvPCustomCombo]
    [ReplaceSkill(RPRPvP.Slice)]
    [CustomComboInfo("Burst Mode",
        "Turns Slice Combo into an all-in-one damage button.\nAdds Soul Slice to the main combo.", RPRPvP.JobID)]
    RPRPvP_Burst = 122000,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Grim Swathe Option", "Add's Grim Swathe onto the main combo on cd", RPRPvP.JobID)]
    RPRPvP_Burst_GrimSwathe = 122009,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Death Warrant Option",
        "Adds Death Warrant onto the main combo when Plentiful Harvest is ready to use, or when Plentiful Harvest's cooldown is longer than Death Warrant's.\nRespects Immortal Sacrifice Pooling Option.",
        RPRPvP.JobID)]
    RPRPvP_Burst_DeathWarrant = 122001,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Plentiful Harvest + Immortal Sacrifice Pooling Option",
        "Pools stacks of Immortal Sacrifice before using Plentiful Harvest.\nAlso holds Plentiful Harvest if Death Warrant is on cooldown.\nSet the value to 3 or below to use Plentiful Harvest as soon as it's available.",
        RPRPvP.JobID)]
    RPRPvP_Burst_ImmortalPooling = 122003,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Enshrouded Burst Option",
        "Adds Lemure's Slice to the main combo during the Enshroud burst phase.\nContains burst options.", RPRPvP.JobID)]
    RPRPvP_Burst_Enshrouded = 122004,

    #region RPR Enshrouded Option

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst_Enshrouded)]
    [CustomComboInfo("Enshrouded Death Warrant Option",
        "Adds Death Warrant onto the main combo during the Enshroud burst when available.", RPR.JobID)]
    RPRPvP_Burst_Enshrouded_DeathWarrant = 122005,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst_Enshrouded)]
    [CustomComboInfo("Communio Finisher Option",
        "Adds Communio onto the main combo when you have 1 stack of Enshroud remaining.\nWill not trigger if you are moving.",
        RPR.JobID)]
    RPRPvP_Burst_Enshrouded_Communio = 122006,

    #endregion

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Ranged Harvest Moon Option",
        "Adds Harvest Moon onto the main combo when you're out of melee range, the GCD is not rolling and it's available for use. Will also throw it when the enemy is under 12k health for execute",
        RPRPvP.JobID)]
    RPRPvP_Burst_RangedHarvest = 122007,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Arcane Crest Option", "Adds Arcane Crest to the main combo when under the set HP perecentage.",
        RPRPvP.JobID)]
    RPRPvP_Burst_ArcaneCircle = 122008,

    [PvPCustomCombo]
    [ParentCombo(RPRPvP_Burst)]
    [CustomComboInfo("Role Action Smite Option",
        "Adds Role Action Smite to Burst Mode below selected health",
        RPRPvP.JobID)]
    RPRPvP_Smite = 122010,

    // Last value = 122010

    #endregion

    #region RED MAGE

    [PvPCustomCombo]
    [ReplaceSkill(RDMPvP.Jolt3)]
    [CustomComboInfo("Burst Mode", "Turns Jolt III into an all-in-one button.\n- Will not attempt to cast Jolt III while moving by replacing it with Savage Blade.", RDMPvP.JobID)]
    RDMPvP_BurstMode = 123000,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo("Riposte Combo Option", "Uses Riposte and Scorch when available.\n- Requires melee range for Riposte Combo.", RDMPvP.JobID)]
    RDMPvP_Riposte = 123001,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo("Resolution Option", "Uses Resolution when available.\n- Will not use against non-players.\n- Requires target's HP to be under:", RDMPvP.JobID)]
    RDMPvP_Resolution = 123002,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo("Embolden Option", "Uses Embolden when available.\n- Requires Enchanted Riposte to be available.\n- Will use alongside Corps-a-corps if enabled.", RDMPvP.JobID)]
    RDMPvP_Embolden = 123003,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo("Corps-a-corps Option", "Uses Corps-a-corps when available.\n- Must remain within maximum range.\n- Requires Enchanted Riposte to be available.", RDMPvP.JobID)]
    RDMPvP_Corps = 123004,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo("Displacement Option", "Uses Displacement when available.\n- Will use when Scorch becomes available.\n- Requires target to be within melee range.", RDMPvP.JobID)]
    RDMPvP_Displacement = 123005,

    [PvPCustomCombo]
    [ParentCombo(RDMPvP_BurstMode)]
    [CustomComboInfo("Forte Option", "Uses Forte when available.\n- Will not use outside combat.\n- Requires player's HP to be under:", RDMPvP.JobID)]
    RDMPvP_Forte = 123006,

    [PvPCustomCombo]
    [ReplaceSkill(RDMPvP.CorpsACorps, RDMPvP.Displacement)]
    [CustomComboInfo("Corps-a-corps / Displacement Feature", "Adds Purify when affected by crowd control.\n- Requires Purify to be available.", RDMPvP.JobID)]
    RDMPvP_Dash_Feature = 123007,

    [ParentCombo(RDMPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Phantom Dart Option", "Uses Phantom Dart (if selected) when available at or below set health threshold.", RDMPvP.JobID)]
    RDMPvP_PhantomDart = 123008,

    // Last value = 123008

    #endregion

    #region SAGE

    [PvPCustomCombo]
    [ReplaceSkill(SGEPvP.Dosis)]
    [CustomComboInfo("Burst Mode", "Turns Dosis III into an all-in-one damage button.", SGEPvP.JobID)]
    SGEPvP_BurstMode = 124000,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Pneuma Option", "Adds Pneuma to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_Pneuma = 124001,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Eukrasia Option", "Adds Eukrasia to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_Eukrasia = 124002,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Phlegma Option", "Adds Phlegma to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_Phlegma = 124003,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Psyche Option", "Adds Psyche to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_Psyche = 124004,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Toxikon Option", "Adds Toxikon to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_Toxikon = 124005,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Toxikon II Option", "Adds Toxikon II to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_Toxikon2 = 124006,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Kardia Reminder Option", "Adds Kardia to Burst Mode.", SGEPvP.JobID)]
    SGEPvP_BurstMode_KardiaReminder = 124007,

    [PvPCustomCombo]
    [ParentCombo(SGEPvP_BurstMode)]
    [CustomComboInfo("Role Action Diabrosis Option",
       "Adds Role Action Diabrosis to Burst Mode below selected health",
       SGEPvP.JobID)]
    SGEPvP_Diabrosis = 124008,

    // Last value = 124008

    #endregion

    #region SAMURAI

    [PvPCustomCombo]
    [ReplaceSkill(SAMPvP.Yukikaze)]
    [CustomComboInfo("Burst Mode",
        "Turns Kasha Combo into an all-in-one button.\n- Will not use actions with cast time while moving.",
        SAMPvP.JobID)]
    SAMPvP_Burst = 125000,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo("Meikyo Shisui Option",
        "Uses Meikyo Shisui when available.\n- Requires target to be in melee range.", SAMPvP.JobID)]
    SAMPvP_Meikyo = 125001,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo("Chiten Option",
        "Uses Chiten when available.\n- Will not use outside combat.\n- Requires player's HP to be under:",
        SAMPvP.JobID)]
    SAMPvP_Chiten = 125002,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo("Mineuchi Option",
        "Uses Mineuchi when available.\n- Will not use against non-players.\n- Requires target's HP to be under:",
        SAMPvP.JobID)]
    SAMPvP_Mineuchi = 125003,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo("Soten Option",
        "Uses Soten when available.\n- Must remain within maximum range.\n- Will not use if already under Kaiten's effect.",
        SAMPvP.JobID)]
    SAMPvP_Soten = 125004,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo("Zantetsuken Option",
        "Uses Zantetsuken when available.\n- Will not use if target is invulnerable.\n- Requires target to have player's Kuzushi.",
        SAMPvP.JobID)]
    SAMPvP_Zantetsuken = 125005,

    [PvPCustomCombo]
    [ParentCombo(SAMPvP_Burst)]
    [CustomComboInfo("Role Action Smite Option",
        "Adds Role Action Smite to Burst Mode below selected health",
        SAMPvP.JobID)]
    SAMPvP_Smite = 125006,

    // Last value = 125006

    #endregion

    #region SCHOLAR

    [PvPCustomCombo]
    [ReplaceSkill(SCHPvP.Broil)]
    [CustomComboInfo("Burst Mode", "Turns Broil IV into all-in-one damage button.", SCHPvP.JobID)]
    SCHPvP_Burst = 126000,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo("Expedient Option", "Adds Expedient to Burst Mode to empower Biolysis.", SCHPvP.JobID)]
    SCHPvP_Expedient = 126001,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo("Biolysis Option", "Adds Biolysis use on cooldown to Burst Mode.", SCHPvP.JobID)]
    SCHPvP_Biolysis = 126002,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo("Deployment Tactics Option", "Adds Deployment Tactics to Burst Mode when available.", SCHPvP.JobID)]
    SCHPvP_DeploymentTactics = 126003,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo("Chain Stratagem Option", "Adds Chain Stratagem to Burst Mode when available.", SCHPvP.JobID)]
    SCHPvP_ChainStratagem = 126004,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo("Role Action Diabrosis Option",
       "Adds Role Action Diabrosis to Burst Mode below selected health",
       SCHPvP.JobID)]
    SCHPvP_Diabrosis = 126005,

    [PvPCustomCombo]
    [ParentCombo(SCHPvP_Burst)]
    [CustomComboInfo("Self Adlo Option",
       "Adds Adloquium to self when at or beneath selected heath threshold. \n Will Not Overwrite Catalyze",
       SCHPvP.JobID)]
    SCHPvP_Selfcare = 126006,

    // Last value = 126006

    #endregion

    #region SUMMONER

    [PvPCustomCombo]
    [ReplaceSkill(SMNPvP.Ruin3)]
    [CustomComboInfo("Burst Mode",
        "Turns Ruin III into an all-in-one damage button.\nOnly uses Crimson Cyclone when in melee range.",
        SMNPvP.JobID)]
    SMNPvP_BurstMode = 127000,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Radiant Aegis Option",
        "Adds Radiant Aegis to Burst Mode when available, and your HP is at or below the set percentage.",
        SMNPvP.JobID)]
    SMNPvP_BurstMode_RadiantAegis = 127001,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Crimson Cyclone Option", "Adds Crimson Cyclone to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_CrimsonCyclone = 127002,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Crimson Strike Option", "Adds Crimson Strike to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_CrimsonStrike = 127003,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Mountain Buster Option", "Adds Mountain Buster to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_MountainBuster = 127004,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Slipstream Option", "Adds Slipstream to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_Slipstream = 127005,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Necrotize Option", "Adds Necrotize to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_Necrotize = 127006,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("DeathFlare Option", "Adds DeathFlare to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_DeathFlare = 127007,

    [PvPCustomCombo]
    [ParentCombo(SMNPvP_BurstMode)]
    [CustomComboInfo("Brand of Purgatory Option", "Adds Brand of Purgatory to Burst Mode.", SMNPvP.JobID)]
    SMNPvP_BurstMode_BrandofPurgatory = 127008,

    [ParentCombo(SMNPvP_BurstMode)]
    [PvPCustomCombo]
    [CustomComboInfo("Phantom Dart Option", "Uses Phantom Dart (if selected) when available at or below set health threshold.", SMNPvP.JobID)]
    SMNPvP_PhantomDart = 127009,

    // Last value = 127008

    #endregion

    #region VIPER

    [PvPCustomCombo]
    [ReplaceSkill(VPRPvP.SteelFangs)]
    [CustomComboInfo("Burst Mode", "Turns Dual Fang Combo into an all-in-one button.", VPRPvP.JobID)]
    VPRPvP_Burst = 130000,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo("Bloodcoil Option",
        "Uses Bloodcoil when available.\n- Requires target's or player's HP to be under:", VPRPvP.JobID)]
    VPRPvP_Bloodcoil = 130001,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo("Uncoiled Fury Option", "Uses Uncoiled Fury when available.\n- Requires target's HP to be under:",
        VPRPvP.JobID)]
    VPRPvP_UncoiledFury = 130002,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo("Backlash Option", "Uses Backlash when available.", VPRPvP.JobID)]
    VPRPvP_Backlash = 130003,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo("Rattling Coil Option", "Uses Rattling Coil when any condition is met.", VPRPvP.JobID)]
    VPRPvP_RattlingCoil = 130004,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo("Slither Option",
        "Uses Slither when outside melee.\n- Must remain within maximum range.\n- Will not use if already under Slither's effect.",
        VPRPvP.JobID)]
    VPRPvP_Slither = 130005,

    [PvPCustomCombo]
    [ReplaceSkill(VPRPvP.SnakeScales)]
    [CustomComboInfo("Snake Scales Reset Feature",
        "Adds Rattling Coil to Snake Scales when available.\n- Requires Snake Scales to be on cooldown.", VPRPvP.JobID)]
    VPRPvP_SnakeScales_Feature = 130006,

    [PvPCustomCombo]
    [ParentCombo(VPRPvP_Burst)]
    [CustomComboInfo("Role Action Smite Option",
        "Adds Role Action Smite to Burst Mode below selected health",
        VPRPvP.JobID)]
    VPRPvP_Smite = 130007,

    // Last value = 130007

    #endregion

    #region WARRIOR

    [PvPCustomCombo]
    [ReplaceSkill(WARPvP.HeavySwing)]
    [CustomComboInfo("Burst Mode", "Turns Heavy Swing into an all-in-one damage button.", WARPvP.JobID)]
    WARPvP_BurstMode = 128000,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Bloodwhetting Option", "Allows use of Bloodwhetting any time, not just between GCDs.",
        WARPvP.JobID)]
    WARPvP_BurstMode_Bloodwhetting = 128001,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Blota Option", "Adds Blota to Burst Mode when not in melee range.", WARPvP.JobID)]
    WARPvP_BurstMode_Blota = 128003,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Primal Rend Option", "Adds Primal Rend to Burst Mode.", WARPvP.JobID)]
    WARPvP_BurstMode_PrimalRend = 128004,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Inner Chaos Option", "Adds Inner Chaos to Burst Mode.", WARPvP.JobID)]
    WARPvP_BurstMode_InnerChaos = 128005,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Orogeny Option", "Adds Orogeny to Burst Mode.", WARPvP.JobID)]
    WARPvP_BurstMode_Orogeny = 128006,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Onslaught Option", "Adds Onslaught to Burst Mode.", WARPvP.JobID)]
    WARPvP_BurstMode_Onslaught = 128007,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("PrimalScream Option", "Adds Primal Scream to Burst Mode.", WARPvP.JobID)]
    WARPvP_BurstMode_PrimalScream = 128008,

    [PvPCustomCombo]
    [ParentCombo(WARPvP_BurstMode)]
    [CustomComboInfo("Role Action Rampart Option",
        "Adds Defensive Role Action Rampart to Burst Mode below selected health", WARPvP.JobID)]
    WARPvP_Rampart = 128009,

    // Last value = 128009

    #endregion

    #region WHITE MAGE

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.Glare)]
    [CustomComboInfo("Burst Mode", "Turns Glare into an all-in-one damage button.", WHMPvP.JobID)]
    WHMPvP_Burst = 129000,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo("Misery Option", "Adds Afflatus Misery to Burst Mode.", WHMPvP.JobID)]
    WHMPvP_Afflatus_Misery = 129001,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo("Miracle of Nature Option", "Adds Miracle of Nature to Burst Mode.", WHMPvP.JobID)]
    WHMPvP_Mirace_of_Nature = 129002,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo("Seraph Strike Option", "Adds Seraph Strike to Burst Mode.", WHMPvP.JobID)]
    WHMPvP_Seraph_Strike = 129003,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo("Afflatus Purgation Option", "Adds Afflatus Purgation (Limit Break) to Burst Mode.", WHMPvP.JobID)]
    WHMPvP_AfflatusPurgation = 129008,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo("Role Action Diabrosis Option",
       "Adds Role Action Diabrosis to Burst Mode below selected health",
       WHMPvP.JobID)]
    WHMPvP_Diabrosis = 129009,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Burst)]
    [CustomComboInfo("Cure 3 Waste Prevention", "Adds Cure 3 to Burst combo when the Cure 3 Ready buff is under 6 seconds", WHMPvP.JobID)]
    WHMPvP_NoWasteCure = 129010,

    [PvPCustomCombo]
    [ReplaceSkill(WHMPvP.Cure2)]
    [CustomComboInfo("Heal Feature", "Adds the below options onto Cure II.", WHMPvP.JobID)]
    WHMPvP_Heals = 129004,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Aquaveil)]
    [CustomComboInfo("Cure III Priority Option", "Makes Cure 3 prioritized before Aquaveil. \n Cure 3 replaces Cure 2 regardless of this setting as that is how SE made it", WHMPvP.JobID)]
    WHMPvP_Cure3 = 129005,

    [PvPCustomCombo]
    [ParentCombo(WHMPvP_Heals)]
    [CustomComboInfo("Aquaveil Option", "Adds Aquaviel to Cure II when available.", WHMPvP.JobID)]
    WHMPvP_Aquaveil = 129007,

    // Last value = 129010

    #endregion

    #endregion
}
