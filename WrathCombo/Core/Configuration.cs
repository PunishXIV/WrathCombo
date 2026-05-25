#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Dalamud.Configuration;
using Newtonsoft.Json;
using WrathCombo.AutoRotation;
using WrathCombo.Window;
using WrathCombo.Attributes;
using WrathCombo.Window.Functions;
using WrathCombo.Window.Tabs;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Attributes.SettingCategory.Category;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Setting = WrathCombo.Attributes.Setting;
using Space = WrathCombo.Attributes.SettingUI_Space;
using Or = WrathCombo.Attributes.SettingUI_Or;
using Retarget = WrathCombo.Attributes.SettingUI_RetargetIcon;

#endregion

// ReSharper disable RedundantDefaultMemberInitializer

namespace WrathCombo.Core;

/// <summary> Plugin configuration. </summary>
[Serializable]
public partial class Configuration : IPluginConfiguration
{
    /// <summary> Gets or sets the configuration version. </summary>
    public int Version { get; set; } = 6;

    #region Settings

    #region UI Settings

    /// <summary>
    /// For the Oops all MNK gag
    /// </summary>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool AprilFools2026 = true;

    /// Whether to hide the children of a feature if it is disabled. Default: false.
    /// <seealso cref="Presets.DrawPreset"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool HideChildren = false;

    /// Whether to hide combos which conflict with enabled presets. Default: false.
    /// <seealso cref="Presets.DrawPreset"/>
    /// <seealso cref="PvEFeatures.DrawHeadingContents"/>
    /// <seealso cref="PvPFeatures.DrawHeadingContents"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool HideConflictedCombos = false;

    /// If the DTR Bar text should be shortened. Default: false.
    /// <seealso cref="WrathCombo.OnFrameworkUpdate"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool ShortDTRText = false;

    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool ShowOpenerDtr = false;

    /// Hides the message of the day. Default: false.
    /// <seealso cref="WrathCombo.PrintLoginMessage"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool SuppressSetCommands = false;

    /// Hides the Autorot set message. Default: false.
    /// <seealso cref="WrathCombo.PrintLoginMessage"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool SuppressAutorotCommand = false;

    /// Hides the message of the day. Default: false.
    /// <seealso cref="WrathCombo.PrintLoginMessage"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool HideMessageOfTheDay = false;

    /// Whether to draw a box around targeted party members. Default: false.
    /// <seealso cref="TargetHelper"/>
    /// <seealso cref="TargetHighlightColor"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool ShowTargetHighlight = false;

    /// The color of box to draw around targeted party members. Default: 808080FF.
    /// <seealso cref="ShowTargetHighlight"/>
    /// <seealso cref="TargetHelper"/>
    [SettingParent(nameof(ShowTargetHighlight))]
    [SettingCategory(Main_UI_Options)]
    [Setting(type: Setting.Type.Color)]
    public Vector4 TargetHighlightColor =
        new() { W = 1, X = 0.5f, Y = 0.5f, Z = 0.5f };

    /// Whether to draw a box around Presets with children. Default: true.
    /// <seealso cref="Presets.DrawPreset"/>
    /// <seealso cref="InfoBox"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool ShowBorderAroundOptionsWithChildren = true;

    /// Whether to label Presets with their ID. Default: true.
    /// <seealso cref="Presets.DrawPreset"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool UIShowPresetIDs = true;

    /// Whether to show search bars. Default: true.
    /// <seealso cref="FeaturesWindow.DrawSearchBar"/>
    /// <seealso cref="ConfigWindow.Search"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool UIShowSearchBar = true;

    /// <summary>
    /// Whether to play TTS when Tankbusters are detected. Default: false.
    /// </summary>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool TankbusterTTS = false;

    /// <summary>
    /// Whether to play TTS when Raidwides/Group Damages are detected. Default: false.
    /// </summary>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool AoEDamageTTS = false;

    /// <summary>
    /// Whether to show a toast whenever Tankbusters are detected. Default: false.
    /// </summary>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool TankbusterToast = false;

    /// <summary>
    /// Whether to show a toast when Raidwides/Group Damages are detected. Default: false.
    /// </summary>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool AoEDamageToast = false;

    #region Future Search Settings

    /// The preferred search behavior. Default: Filter.
    /// <seealso cref="FeaturesWindow.PresetMatchesSearch"/>
    /// <seealso cref="ConfigWindow.Search"/>
    /// <seealso cref="SearchMode"/>
    public SearchMode SearchBehavior = SearchMode.Filter;

    /// The search mode. Default: Filter.
    /// <seealso cref="Configuration.SearchBehavior"/>
    public enum SearchMode
    {
        /// Only shows matching Presets.
        Filter,
        /// Shows all Presets, but highlights matching ones.
        Highlight,
    }

    /// Whether to preserve hierarchy in Filter mode. Default: false.
    /// <seealso cref="Configuration.SearchBehavior"/>
    public bool SearchPreserveHierarchy = false;

    #endregion

    /// Whether, upon opening, it should always go to the PvE tab. Default: false.
    /// <seealso cref="WrathCombo.HandleOpenCommand"/>
    [Space]
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool OpenToPvE = false;

    /// Whether, upon opening, it should go to the PvP tab in PvP zones. Default: false.
    /// <seealso cref="WrathCombo.HandleOpenCommand"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool OpenToPvP = false;

    /// Whether the PvE Features tab should open to your current Job. Default: false.
    /// <seealso cref="PvEFeatures.OpenToCurrentJob"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool OpenToCurrentJob = false;

    /// Whether the PvE Features tab, upon switching jobs, should open to your new Job. Default: false.
    /// <seealso cref="PvEFeatures.OpenToCurrentJob"/>
    [SettingCategory(Main_UI_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool OpenToCurrentJobOnSwitch = false;

    #endregion

    #region Rotation Behavior Settings

    /// Whether all Combos should be <see cref="All.SavageBlade"/> when moving. Default: false.
    /// <seealso cref="ActionReplacer.GetAdjustedAction"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool BlockSpellOnMove = false;

    /// Whether Hotbars will be walked, and matching actions updated. Default: true.
    /// <seealso cref="SetActionChanging" />
    /// <seealso cref="WrathCombo.HandleComboCommands" />
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool ActionChanging = true;

    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool QueueAdjust = false;

    [SettingParent(nameof(QueueAdjust))]
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Slider_Float,
        minFloat: 0f,
        maxFloat: 2.5f)]
    public float QueueAdjustThreshold = 1.5f;

    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool OverwriteQueue = false;

    /// The throttle for how often the hotbar gets walked. Default: 50.
    /// <seealso cref="ActionChanging"/>
    /// <seealso cref="ActionReplacer.GetAdjustedActionDetour"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Number_Int,
        minInt: 0,
        maxInt: 500)]
    public int Throttle = 50;

    /// Delay before recognizing movement. Default: 0.
    /// <seealso cref="CustomComboFunctions.IsMoving"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Number_Float,
        minFloat: 0,
        maxFloat: 10)]
    public float MovementLeeway = 0f;

    /// The timeout for opener failure. Default: 4.
    /// <seealso cref="CustomComboNS.WrathOpener.FullOpener"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Number_Float,
        minFloat: 4f,
        maxFloat: 20)]
    public float OpenerTimeout = 4f;

    /// The offset of the melee range check. Default: 0.
    /// <seealso cref="InMeleeRange"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Number_Float,
        minFloat: -3,
        maxFloat: 30)]
    public float MeleeOffset = 0;

    /// The % through a cast before interrupting. Default: 0.
    /// <seealso cref="CanInterruptEnemy"/>
    /// <seealso cref="CanStunToInterruptEnemy"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Slider_Float,
        minFloat: 0,
        maxFloat: 100)]
    public float InterruptDelay = 0;

    /// The maximum allowable weaves between GCDs. Default: 2.
    /// <seealso cref="CanWeave"/>
    /// <seealso cref="CanDelayedWeave"/>
    [SettingCategory(Rotation_Behavior_Options)]
    [Setting(type: Setting.Type.Slider_Int,
        minInt: 1,
        maxInt: 3)]
    public int MaximumWeavesPerWindow = 2;

    #endregion

    #region Target Settings

    /// Whether to retarget heals to the Heal Stack. Default: false.
    /// <seealso cref="HealRetargeting"/>
    [SettingCategory(Targeting_Options)]
    [Setting(Setting.Type.Toggle)]
    [Retarget]
    public bool RetargetHealingActionsToStack = false;

    /// Whether to include out-of-party NPCs to retargeting. Default: false.
    /// <seealso cref="GetPartyMembers"/>
    [SettingCategory(Targeting_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool AddOutOfPartyNPCsToRetargeting = false;

    #region Default+ Heal Stack

    /// Whether to include UI Mouseover in 'default' Heal Stack. Default: false.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    [SettingCategory(Targeting_Options)]
    // The spaces make it align better with the raise stack collapsible group
    [SettingCollapsibleGroup("Heal Stack Customization Options  ")]
    [SettingGroup("defaultPlus", "healStackPlus")]
    [Setting(Setting.Type.Toggle)]
    public bool UseUIMouseoverOverridesInDefaultHealStack = false;
    
    /// Whether to include UI Mouseover in 'default' Heal Stack. Default: false.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    [SettingCategory(Targeting_Options)]
    [SettingCollapsibleGroup("Heal Stack Customization Options  ")]
    [SettingGroup("defaultPlus", "healStackPlus")]
    [Setting(Setting.Type.Toggle)]
    public bool UseFieldMouseoverOverridesInDefaultHealStack = false;
    
    /// Whether to include Focus Target in 'default' Heal Stack. Default: false.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    [SettingCategory(Targeting_Options)]
    [SettingCollapsibleGroup("Heal Stack Customization Options  ")]
    [SettingGroup("defaultPlus", "healStackPlus")]
    [Setting(Setting.Type.Toggle)]
    public bool UseFocusTargetOverrideInDefaultHealStack = false;
    
    /// Whether to include Lowest HP% in 'default' Heal Stack. Default: false.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    [SettingCategory(Targeting_Options)]
    [SettingCollapsibleGroup("Heal Stack Customization Options  ")]
    [SettingGroup("defaultPlus", "healStackPlus")]
    [Setting(Setting.Type.Toggle)]
    public bool UseLowestHPOverrideInDefaultHealStack = false;

    #endregion

    #region Custom Heal Stack

    /// Whether to use a Custom Heal Stack. Default: false.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    /// <seealso cref="HealRetargeting.RetargetSettingOn"/>
    [Or]
    [SettingCollapsibleGroup("Heal Stack Customization Options  ")]
    [SettingGroup("custom", "healStackPlus", false)]
    [SettingCategory(Targeting_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool UseCustomHealStack = false;

    /// The Custom Heal Stack.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    /// <seealso cref="HealRetargeting.HealStack"/>
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.AllyToHeal"/>
    [SettingCollapsibleGroup("Heal Stack Customization Options  ")]
    [SettingParent(nameof(UseCustomHealStack))]
    [SettingCategory(Targeting_Options)]
    [Setting(type: Setting.Type.Stack,
        stackStringsToExclude:
        ["Enemy", "Attack", "Dead", "Living"])]
    public string[] CustomHealStack =
    [
        "FocusTarget",
        "HardTarget",
        "Self",
    ];

    #endregion

    /// The Custom Raise Stack.
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.GetStack"/>
    /// <seealso cref="CustomComboNS.SimpleTarget.Stack.AllyToRaise"/>
    [SettingCollapsibleGroup("Raise Stack Customization Options")]
    [SettingCategory(Targeting_Options)]
    [Setting(type: Setting.Type.Stack,
        stackStringsToExclude:
        ["Enemy", "Attack", "MissingHP", "Lowest", "Chocobo", "Living"])]
    public string[] RaiseStack =
    [
        "AnyHealer",
        "AnyTank",
        "AnyRaiser",
        "AnyDeadPartyMember",
    ];

    #endregion

    #region Troubleshooting

    /// Whether to output Combo actions to the chatbox.
    /// <seealso cref="Data.ActionWatching.UpdateLastUsedAction"/>
    [SettingCategory(Troubleshooting_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool EnabledOutputLog = false;

    /// Whether to output Opener state to the chatbox.
    /// <seealso cref="CustomComboNS.WrathOpener.CurrentState"/>
    [SettingCategory(Troubleshooting_Options)]
    [Setting(Setting.Type.Toggle)]
    public bool OutputOpenerLogs;

    #endregion

    #endregion

    #region Non-Settings Configurations

    public bool UILeftColumnCollapsed = false;

    public bool ShowHiddenFeatures = false;

    #region EnabledActions

    /// <summary> Gets or sets the collection of enabled combos. </summary>
    [JsonProperty("EnabledActionsV6")]
    public HashSet<Preset> EnabledActions { get; set; } = [];

    #endregion

    #region AutoAction Settings

    public Dictionary<Preset, bool> AutoActions { get; set; } = [];

    public AutoRotationConfig RotationConfig { get; set; } = new();

    public Dictionary<uint, uint> IgnoredNPCs { get; set; } = new();

    #endregion

    #region Job-specific

    /// <summary> Gets active Blue Mage (BLU) spells. </summary>
    public List<uint> ActiveBLUSpells { get; set; } = [];

    /// <summary>
    ///     Gets or sets an array of 4 ability IDs to interact with the
    ///     <see cref="Preset.DNC_CustomDanceSteps" /> combo.
    /// </summary>
    public uint[] DancerDanceCompatActionIDs { get; set; } = [0, 0, 0, 0,];

    #endregion

    #region Popups

    /// <summary>
    ///     Whether the Major Changes window was hidden for a
    ///     specific version.
    /// </summary>
    /// <seealso cref="MajorChangesWindow" />
    public Version HideMajorChangesForVersion =
        System.Version.Parse("0.0.0");

    #endregion

    #region UserConfig Values

    [JsonProperty("CustomFloatValuesV6")]
    internal static Dictionary<string, float>
        CustomFloatValues { get; set; } = [];

    [JsonProperty("CustomIntValuesV6")]
    internal static Dictionary<string, int>
        CustomIntValues { get; set; } = [];

    [JsonProperty("CustomIntArrayValuesV6")]
    internal static Dictionary<string, int[]>
        CustomIntArrayValues { get; set; } = [];

    [JsonProperty("CustomBoolValuesV6")]
    internal static Dictionary<string, bool>
        CustomBoolValues { get; set; } = [];

    [JsonProperty("CustomBoolArrayValuesV6")]
    internal static Dictionary<string, bool[]>
        CustomBoolArrayValues { get; set; } = [];

    #endregion

    public HashSet<(ushort Status, uint BaseId)> StatusBlacklist = [];

    #endregion
}