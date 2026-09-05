using ECommons;
using ECommons.ExcelServices;
using ECommons.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WrathCombo.API.Enum;
using WrathCombo.Attributes;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using static WrathCombo.Core.Configuration;
using static WrathCombo.Core.PresetStorage;
using static WrathCombo.CustomComboNS.Functions.Jobs;
using static WrathCombo.Window.Text;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

namespace WrathCombo.Core;

internal static class PresetStorage
{
    private static PresetDictionaries? _instance;

    /// <summary>
    /// Singleton instance holding all preset data, initialized during plugin load.
    /// INTERNAL: Only PresetStorage accesses this. All external access goes through typed properties below.
    /// </summary>
    private static PresetDictionaries Dictionaries
    {
        get => _instance ?? throw new InvalidOperationException("PresetStorage Dictionaries not initialized");
        set => _instance = value;
    }

    /// <summary>
    /// Initialize the preset dictionaries from an async load.
    /// Call this once during plugin initialization.
    /// </summary>
    internal static void InitializeDictionaries() => Dictionaries = new PresetDictionaries();

    /// <summary>
    /// A frozen dictionary containing the Preset as the key, and a PresetData object containing all of its relevant attributes as the value.
    /// </summary>
    internal static FrozenDictionary<Preset, PresetData> AllPresets =>
        Dictionaries.AllPresetsData;

    /// <summary>
    /// A frozen set of presets that have at least one conflict with another preset
    /// </summary>
    internal static FrozenSet<Preset> ConflictingCombos =>
        Dictionaries.ConflictingCombosData;

    /// <summary>
    ///     A frozen lookup from a preset's internal name (case-insensitive) to
    ///     the <see cref="Preset" /> itself, used by <see cref="GetPresetByName" />
    ///     so that resolving a name doesn't require scanning every preset.
    /// </summary>
    private static FrozenDictionary<string, Preset> PresetsByName =>
        Dictionaries.PresetsByNameData;

    /// <summary>
    ///     A frozen lookup from a preset's underlying integer ID to the
    ///     <see cref="Preset" /> itself, used by <see cref="GetPresetByInt" />
    ///     so that resolving an ID doesn't require scanning every preset.
    /// </summary>
    private static FrozenDictionary<int, Preset> PresetsById =>
        Dictionaries.PresetsByIdData;

    /// <summary>
    /// Dictionary of top level presets grouped by job, ordered by role and then job order, with their preset data pre-cached for quick access.
    /// </summary>
    internal static Dictionary<Job, List<PresetData>> groupedPresets =>
        Dictionaries.GroupedPresets;

    /// <summary>
    ///  Dictionary of a preset and an array of it's children, with their preset data pre-cached for quick access.
    /// </summary>
    internal static Dictionary<Preset, (Preset Preset, PresetData Attr)[]> presetChildren =>
        Dictionaries.PresetChildren;

    internal class PresetData
    {
        public Preset Preset { get; }
        //UI Facing Name and Description
        public string Name => PresetLocalization.GetName(Preset);
        public string Description => PresetLocalization.GetDescription(Preset);
        //The Enum Name
        public string InternalName { get; }
        public bool IsPvP { get; }
        public bool IsAoE { get; }
        public Preset[] Conflicts;
        public Preset? Parent;
        public Preset? GrandParent;
        public Preset? GreatGrandParent;
        public Preset RootParent;
        public BlueInactiveAttribute? BlueInactive;
        public bool IsVariant { get; }
        public PossiblyRetargetedAttribute? PossiblyRetargeted;
        public RetargetedAttribute? RetargetedAttribute;
        public uint[] RetargetedActions =>
            GetRetargetedActions(Preset, RetargetedAttribute, PossiblyRetargeted, Parent);
        public bool IsBozja { get; }
        public bool IsBlueDPS { get; }
        public bool IsBlueTank { get; }
        public bool IsBlueHealer { get; }
        public bool IsBlueRole => IsBlueDPS || IsBlueTank || IsBlueHealer;
        public bool IsOccultCrescent => OccultCrescentJob != null;
        public OccultCrescentAttribute? OccultCrescentJob;
        public bool IsDeepDungeon { get; }
        public string? HoverText { get; }
        public ReplaceSkillAttribute? ReplaceSkill;
        public JobInfoAttribute? JobInfo;
        public AutoActionAttribute? AutoAction;
        public bool IsHidden { get; }
        public bool ShouldBeHidden => (IsHidden && !Service.Configuration.ShowHiddenFeatures);
        public ComboType ComboType;
        public ComboTargetTypeKeys TargetType;

        public PresetData(Preset preset)
        {
            Preset = preset;
            InternalName = preset.ToString();
            IsPvP = preset.GetAttribute<PvPCustomComboAttribute>() != null;
            Conflicts = preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? [];
            Parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
            BlueInactive = preset.GetAttribute<BlueInactiveAttribute>();
            IsDeepDungeon = preset.GetAttribute<DeepDungeonAttribute>() != null;
            IsVariant = preset.GetAttribute<VariantAttribute>() != null;
            PossiblyRetargeted = preset.GetAttribute<PossiblyRetargetedAttribute>();
            RetargetedAttribute = preset.GetAttribute<RetargetedAttribute>();
            IsBozja = preset.GetAttribute<BozjaAttribute>() != null;
            IsBlueDPS = preset.GetAttribute<BlueDPSAttribute>() != null;
            IsBlueTank = preset.GetAttribute<BlueTankAttribute>() != null;
            IsBlueHealer = preset.GetAttribute<BlueHealerAttribute>() != null;
            OccultCrescentJob = preset.GetAttribute<OccultCrescentAttribute>();
            HoverText = preset.GetAttribute<HoverInfoAttribute>()?.HoverText;
            ReplaceSkill = preset.GetAttribute<ReplaceSkillAttribute>();
            JobInfo = preset.GetAttribute<JobInfoAttribute>();
            AutoAction = preset.GetAttribute<AutoActionAttribute>();
            IsAoE = AutoAction?.IsAoE
                ?? InternalName.Contains("_AoE_", StringComparison.OrdinalIgnoreCase);
            IsHidden = preset.GetAttribute<HiddenAttribute>() != null;
            ComboType = GetComboType(preset);
            if (AutoAction != null)
            {
                if (AutoAction.IsHeal)
                {
                    if (AutoAction.IsAoE)
                        TargetType = ComboTargetTypeKeys.AoEHeals;
                    else
                        TargetType = ComboTargetTypeKeys.SingleTargetHeals;
                }
                else
                {
                    if (AutoAction.IsAoE)
                        TargetType = ComboTargetTypeKeys.AoEDPS;
                    else
                        TargetType = ComboTargetTypeKeys.SingleTargetDPS;
                }
            }
            else
                TargetType = ComboTargetTypeKeys.Other;
        }
    }

    // Override Dalamud's GetCustomAttribute
    // Dalamud's creates a new instance of EVERY attribute when needing to just read one, which is bad for performance
    // This version returns strictly the one and only attribute we want
    public static TAttribute? GetAttribute<TAttribute>(this Enum value)
    where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (string.IsNullOrEmpty(name))
            return null;

        var field = type.GetField(name);
        if (field == null)
            return null;

        return field.GetCustomAttribute<TAttribute>(false);
    }

    private static uint[] GetRetargetedActions
    (Preset preset,
        RetargetedAttribute? retargetedAttribute,
        PossiblyRetargetedAttribute? possiblyRetargeted,
        Preset? parent)
    {
        // Pick whichever Retargeted attribute is available
        RetargetedAttributeBase? retargetAttribute = null;
        if (retargetedAttribute != null)
            retargetAttribute = retargetedAttribute;
        else if (possiblyRetargeted != null)
            retargetAttribute = possiblyRetargeted;

        // Bail if the Preset is not Retargeted
        if (retargetAttribute == null)
            return [];

        try
        {
            // Bail if not actually enabled
            if (!Service.Configuration.EnabledActions.Contains(preset))
                return [];
            // ReSharper disable once DuplicatedSequentialIfBodies
            if (parent != null &&
                !Service.Configuration.EnabledActions
                    .Contains((Preset)parent))
                return [];
            if (parent?.Attributes().Parent is { } grandParent &&
                !Service.Configuration.EnabledActions
                    .Contains(grandParent))
                return [];

            // Bail if the Condition for PossiblyRetargeted is not satisfied
            if (retargetAttribute is PossiblyRetargetedAttribute attribute
                && IsConditionSatisfied(attribute.PossibleCondition) != true)
                return [];
        }
        catch (Exception e)
        {
            PluginLog.Error($"Failed to check if Preset {preset} is enabled: {e.ToStringFull()}");
            return [];
        }

        // Set the Retargeted Actions if all bails are passed
        return retargetAttribute.RetargetedActions;
    }

    public static HashSet<uint> AllRetargetedActions
    {
        get
        {
            if (!EZ.Throttle("allRetargetedActions", TS.FromSeconds(3)))
                return Dictionaries.AllRetargetedActionsCache;
            var result = Dictionaries.AllPresetsData.Values
                .SelectMany(attr => attr.RetargetedActions ?? [])
                .ToHashSet();
            PluginLog.Verbose($"Retrieved {result.Count} retargeted actions");
            Dictionaries.AllRetargetedActionsCache = result;
            return result;
        }
    }

    // Build methods moved to PresetStorageData class below

    /// <summary> Gets a value indicating whether a preset is enabled. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsEnabled(Preset preset) => Service.Configuration.EnabledActions.Contains(preset) && !ShouldBeHidden(preset);

    /// <summary>
    /// Gets a value indicating whether a preset is marked as hidden.
    /// </summary>
    /// <param name="preset"></param>
    /// <returns></returns>
    private static bool ShouldBeHidden(Preset preset) =>
        AllPresets[preset].IsHidden &&
        !Service.Configuration.ShowHiddenFeatures;

    /// <summary>
    ///     Gets a value indicating whether a preset can be retargeted under some
    ///     settings, with <see cref="ActionRetargeting" />.
    /// </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsPossiblyRetargeted(Preset preset) =>
        AllPresets[preset].PossiblyRetargeted != null;

    /// <summary>
    ///     Gets a value indicating whether a preset is possibly retargeted with
    ///     <see cref="ActionRetargeting" />.
    /// </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsRetargeted(Preset preset) =>
        AllPresets[preset].RetargetedAttribute != null;

    /// <summary> Gets the parent combo preset if it exists, or null. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The parent preset. </returns>
    public static Preset? GetParent(Preset preset) => AllPresets[preset].Parent;

    /// <summary> Gets an array of conflicting combo presets. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The conflicting presets. </returns>
    public static Preset[] GetConflicts(Preset preset) => AllPresets[preset].Conflicts;

    public static Preset? GetPresetByName(string value) =>
        !string.IsNullOrEmpty(value) &&
        PresetsByName.TryGetValue(value, out var preset)
            ? preset
            : null;

    public static Preset? GetPresetByInt(int value) =>
        PresetsById.TryGetValue(value, out var preset) ? preset : null;

    /// <summary>
    ///     Gets a preset by either its internal name or numeric ID.
    /// </summary>
    /// <param name="value">
    ///     The preset identifier - either the enum member name (e.g., "DRK_Delirium")
    ///     or its numeric ID (e.g., "123").
    /// </param>
    /// <returns>
    ///     The Preset if found, or null if neither the name nor ID match any preset.
    /// </returns>
    public static Preset? GetPresetByIdentifier(string value)
    {
        return int.TryParse(value, out var numericId)
                ? PresetStorage.GetPresetByInt(numericId)
                : PresetStorage.GetPresetByName(value);
    }

    private static object GetControlledText(Preset preset)
    {
        var controlled = P.UIHelper.PresetControlled(preset) is not null;
        var ctrlText = controlled ? " " + OptionControlledByIPC : "";

        return ctrlText;
    }

    public static void RemoveRedundantPresets()
    {
        var redundantIDs = Service.Configuration.EnabledActions.Where(x => int.TryParse(x.ToString(), out _)).OrderBy(x => x).Cast<int>().ToList();
        foreach (var id in redundantIDs)
            Service.Configuration.EnabledActions.RemoveWhere(x => (int)x == id);

        Service.Configuration.Save();
    }

    public static void HandleCurrentConflicts()
    {
        if (!EZ.Throttle("PeriodicPresetDeconflicting", TS.FromSeconds(7)))
            return;

        var enabledPresets = Service.Configuration.EnabledActions.ToArray();
        List<Preset> removedPresets = [];

        foreach (var preset in enabledPresets)
        {
            if (removedPresets.Contains(preset))
                continue;

            foreach (var conflict in preset.Attributes().Conflicts)
            {
                if (!IsEnabled(conflict))
                    continue;

                if (DisablePreset(conflict, ConfigChangeSource.Task))
                {
                    removedPresets.Add(conflict);
                    DuoLog.Warning($"Disabled `{conflict.NameWithFullLineage()}`, " +
                                   $"because it conflicts with " +
                                   $"`{preset.NameWithFullLineage()}`.");
                }
            }
        }
    }

    public static void DisableAllConflicts(Preset preset)
    {
        var conflicts = GetConflicts(preset);
        foreach (var conflict in conflicts)
            DisablePreset(conflict, ConfigChangeSource.AutomaticReaction);
    }

    #region Toggling Presets

    /// <summary> Iterates up a preset's parent tree, enabling each of them. </summary>
    /// <param name="preset"> Combo preset to enable. </param>
    public static void EnableParentPresets(Preset preset)
    {
        var parentMaybe = GetParent(preset);

        while (parentMaybe != null)
        {
            if (!IsEnabled(parentMaybe.Value))
                EnablePreset(parentMaybe.Value);
            parentMaybe = GetParent(parentMaybe.Value);
        }
    }

    public static bool EnablePreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // Bail if already satisfied
        if (!Service.Configuration.EnabledActions.Add(preset))
            return false;

        // Handle Parents and Conflicts
        if (GetParent(preset) is not null)
            EnableParentPresets(preset);
        DisableAllConflicts(preset);

        // Notify of change and save
        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.Preset, source ?? ConfigChangeSource.UI,
            preset.ToString(), true);
        P.IPCSearch.UpdateActiveJobPresets();
        Service.Configuration.Save();

        return true;
    }

    public static bool EnablePreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByName(preset) is { } pre &&
        EnablePreset(pre, source);

    public static bool EnablePreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        EnablePreset(pre, source);

    public static bool DisablePreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // Bail if already satisfied
        if (!Service.Configuration.EnabledActions.Remove(preset))
            return false;

        // Notify of change and save
        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.Preset, source ?? ConfigChangeSource.UI,
            preset.ToString(), false);
        P.IPCSearch.UpdateActiveJobPresets();
        Service.Configuration.Save();

        return true;
    }

    public static bool DisablePreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByName(preset) is { } pre &&
        DisablePreset(pre, source);

    public static bool DisablePreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        DisablePreset(pre, source);

    public static bool TogglePreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // If not already listed, enable it
        if (!Service.Configuration.EnabledActions.Any(x => x == preset))
        {
            return EnablePreset(preset);
        }
        else
        {
            return DisablePreset(preset);
        }
    }

    public static bool TogglePreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByName(preset) is { } pre &&
        TogglePreset(pre, source);

    public static bool TogglePreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        TogglePreset(pre, source);

    #region Auto-Mode

    public static bool EnableAutoModeForPreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // Ensure the preset exists in the dictionary
        Service.Configuration.AutoActions.TryAdd(preset, false);

        Service.Configuration.AutoActions[preset] = true;

        // Notify of change and save
        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.PresetAutoMode, source ?? ConfigChangeSource.UI,
            preset.ToString(), true);
        P.IPCSearch.UpdateActiveJobPresets();
        Service.Configuration.Save();

        return true;
    }

    public static bool EnableAutoModeForPreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByName(preset) is { } pre &&
        EnableAutoModeForPreset(pre, source);

    public static bool EnableAutoModeForPreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        EnableAutoModeForPreset(pre, source);

    public static bool DisableAutoModeForPreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // Ensure the preset exists in the dictionary
        Service.Configuration.AutoActions.TryAdd(preset, false);

        Service.Configuration.AutoActions[preset] = false;

        // Notify of change and save
        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.PresetAutoMode, source ?? ConfigChangeSource.UI,
            preset.ToString(), false);
        P.IPCSearch.UpdateActiveJobPresets();
        Service.Configuration.Save();

        return true;
    }

    public static bool DisableAutoModeForPreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByName(preset) is { } pre &&
        DisableAutoModeForPreset(pre, source);

    public static bool DisableAutoModeForPreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        DisableAutoModeForPreset(pre, source);

    public static bool ToggleAutoModeForPreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // Ensure the preset exists in the dictionary
        Service.Configuration.AutoActions.TryAdd(preset, false);

        var newValue = Service.Configuration.AutoActions[preset] =
            !Service.Configuration.AutoActions[preset];

        // Notify of change and save
        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.PresetAutoMode, source ?? ConfigChangeSource.UI,
            preset.ToString(), newValue);
        P.IPCSearch.UpdateActiveJobPresets();
        Service.Configuration.Save();
        return true;
    }

    public static bool ToggleAutoModeForPreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByName(preset) is { } pre &&
        ToggleAutoModeForPreset(pre, source);

    public static bool ToggleAutoModeForPreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        ToggleAutoModeForPreset(pre, source);

    #endregion

    #endregion

    internal static ComboType GetComboType(Preset preset)
    {
        var simpleDps = preset.GetAttribute<SimpleDPSCombo>();
        var advancedDps = preset.GetAttribute<AdvancedDPSCombo>();
        var basic = preset.GetAttribute<BasicCombo>();
        var simplehealing = preset.GetAttribute<SimpleHealingCombo>();
        var advancedhealing = preset.GetAttribute<AdvancedHealingCombo>();
        var mitigation = preset.GetAttribute<MitigationCombo>();
        var parent = (object?)preset.GetAttribute<ParentComboAttribute>();

        if (simpleDps != null)
            return ComboType.SimpleDPS;
        if (advancedDps != null)
            return ComboType.AdvancedDPS;
        if (basic != null)
            return ComboType.Basic;

        if (simplehealing != null)
            return ComboType.SimpleHealing;
        if (advancedhealing != null)
            return ComboType.AdvancedHealing;
        if (mitigation != null)
            return ComboType.Mitigation;

        if (parent == null)
            return ComboType.Feature;

        return ComboType.Option;
    }
}

/// <summary>
/// Instance-backed data store for PresetStorage. Holds all preset collections and initialization logic.
/// This class is instantiated as a singleton during async load and passed to PresetStorage.Initialize().
/// NOTE: Instantiable (internal) but the instance is hidden via PresetStorage.Dictionaries (private property).
/// External code only accesses typed data through PresetStorage's public properties.
/// </summary>
internal class PresetDictionaries
{
    // Public properties that mirror the static interface
    internal FrozenDictionary<Preset, PresetData> AllPresetsData { get; private set; } = null!;
    internal FrozenSet<Preset> ConflictingCombosData { get; private set; } = null!;
    internal FrozenDictionary<string, Preset> PresetsByNameData { get; private set; } = null!;
    internal FrozenDictionary<int, Preset> PresetsByIdData { get; private set; } = null!;
    internal HashSet<uint> AllRetargetedActionsCache { get; set; } = null!;
    internal Dictionary<Job, List<PresetData>> GroupedPresets { get; set; } = null!;
    internal Dictionary<Preset, (Preset Preset, PresetData Attr)[]> PresetChildren { get; private set; } = null!;

    /// <summary>
    /// Initializes the PresetDictionaries with all preset data.
    /// </summary>
    internal PresetDictionaries()
    {
        // Build master dictionary
        AllPresetsData = BuildPresets();

        // Then parallelize the three derived dictionaries
        Parallel.Invoke(
            () => ConflictingCombosData = BuildConflictingCombos(),
            () => PresetsByNameData = AllPresetsData.Values.ToFrozenDictionary(
                data => data.InternalName,
                data => data.Preset,
                StringComparer.OrdinalIgnoreCase),
            () => PresetsByIdData = AllPresetsData.Keys.ToFrozenDictionary(
                preset => (int)preset,
                preset => preset),
            () => GroupedPresets = GetGroupedPresets(),
            () => PresetChildren = GetPresetChildren()
        );
    }

    private static FrozenDictionary<Preset, PresetData> BuildPresets()
    {
        var dict = new ConcurrentDictionary<Preset, PresetData>();

        // Create all data objects
        Parallel.ForEach(Enum.GetValues<Preset>(), preset =>
        {
            // This operation is independent for each preset
            dict[preset] = new PresetStorage.PresetData(preset);
        });

        var frozen = dict.ToFrozenDictionary();

        // Do not Parallel, seems slower
        foreach (var (preset, attrs) in frozen)
        {
            if (attrs.Parent.HasValue)
            {
                // Walk to root once
                var current = attrs.Parent.Value;
                var ancestors = new List<Preset> { current };

                while (frozen[current].Parent.HasValue)
                {
                    current = frozen[current].Parent!.Value;
                    ancestors.Add(current);
                }

                attrs.RootParent = current;
                attrs.GrandParent = ancestors.Count > 1 ? ancestors[1] : null;
                attrs.GreatGrandParent = ancestors.Count > 2 ? ancestors[2] : null;
            }
            else
            {
                attrs.RootParent = preset;
            }
        }

        return frozen;
    }

    private FrozenSet<Preset> BuildConflictingCombos()
    {
        return AllPresetsData
            .Where(kvp => kvp.Value.Conflicts is { Length: > 0 })
            .Select(kvp => kvp.Key)
            .ToFrozenSet();
    }

    private Dictionary<Job, List<PresetData>> GetGroupedPresets()
    {
        static int GetRoleOrder(JobRole role) => role switch
        {
            JobRole.Tank => 0,
            JobRole.Healer => 1,
            JobRole.MeleeDPS => 2,
            JobRole.RangedDPS => 3,
            JobRole.MagicalDPS => 4,
            _ => 5
        };

        return AllPresetsData
            .Where(kvp => (int)kvp.Key > 100)
            .Where(kvp => kvp.Value.Parent == null)
            .Where(kvp => kvp.Value.JobInfo != null)
            .OrderBy(kvp => GetRoleOrder(kvp.Value.JobInfo.Role))
            .ThenByDescending(kvp => kvp.Value.JobInfo.Job is Job.ADV)
            .ThenByDescending(kvp => kvp.Value.JobInfo.Job is Job.MIN)
            .ThenBy(kvp => kvp.Value.JobInfo.Job)
            .ThenBy(kvp => kvp.Value.JobInfo.Order)
            .GroupBy(kvp => kvp.Value.JobInfo.Job)
            .ToDictionary(
                g => g.Key,
                g => g.Select(kvp => kvp.Value).ToList()
            );
    }

    private Dictionary<Preset, (Preset Preset, PresetData Info)[]> GetPresetChildren()
    {
        // Initialize dictionary with all presets as keys
        var childCombos = AllPresetsData.Keys
            .ToDictionary(p => p, _ => new List<Preset>());

        // Build parent → children map using cached Parent
        foreach (var (preset, attrs) in AllPresetsData)
        {
            if (attrs.Parent is { } parent)
            {
                childCombos[parent].Add(preset);
            }
        }

        // Project to final structure using cached CustomComboInfo
        return childCombos.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
                .Select(child =>
                {
                    var info = AllPresetsData[child]!;
                    return (Preset: child, Info: info);
                })
                .OrderBy(tpl => tpl.Info.JobInfo.Order)
                .ToArray()
        );
    }

}