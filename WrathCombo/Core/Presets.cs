using ECommons;
using ECommons.Logging;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WrathCombo.Attributes;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using static WrathCombo.Core.Configuration;
using static WrathCombo.Window.Text;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

namespace WrathCombo.Core;

internal static class PresetStorage
{
    /// <summary>
    /// A frozen dictionary containing the Preset as the key, and a PresetData object containing all of its relevant attributes as the value.
    /// </summary>
    internal static readonly FrozenDictionary<Preset, PresetData> AllPresets = BuildPresets();

    /// <summary>
    /// A frozen set of presets that have at least one conflict with another preset
    /// </summary>
    internal static readonly FrozenSet<Preset> ConflictingCombos = BuildConflictingCombos();

    internal class PresetData
    {
        public Preset Preset { get; }
        public string Name => PresetLocalization.GetName(Preset);
        public string Description => PresetLocalization.GetDescription(Preset);
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
        public bool IsOccultCrescent => OccultCrescentJob != null;
        public OccultCrescentAttribute? OccultCrescentJob;
        public string? HoverText { get; }
        public ReplaceSkillAttribute? ReplaceSkill;
        public JobInfoAttribute? JobInfo;
        public AutoActionAttribute? AutoAction;
        public bool IsHidden { get; }
        public bool ShouldBeHidden => (IsHidden && !Service.Configuration.ShowHiddenFeatures);
        public ComboType ComboType;

        public PresetData(Preset preset)
        {
            Preset = preset;
            IsPvP = preset.GetAttribute<PvPCustomComboAttribute>() != null;
            Conflicts = preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? [];
            Parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
            BlueInactive = preset.GetAttribute<BlueInactiveAttribute>();
            IsVariant = preset.GetAttribute<VariantAttribute>() != null;
            PossiblyRetargeted = preset.GetAttribute<PossiblyRetargetedAttribute>();
            RetargetedAttribute = preset.GetAttribute<RetargetedAttribute>();
            IsBozja = preset.GetAttribute<BozjaAttribute>() != null;
            OccultCrescentJob = preset.GetAttribute<OccultCrescentAttribute>();
            HoverText = preset.GetAttribute<HoverInfoAttribute>()?.HoverText;
            ReplaceSkill = preset.GetAttribute<ReplaceSkillAttribute>();
            JobInfo = preset.GetAttribute<JobInfoAttribute>();
            AutoAction = preset.GetAttribute<AutoActionAttribute>();
            IsAoE = AutoAction?.IsAoE
                ?? preset.ToString().Contains("_AoE_", StringComparison.OrdinalIgnoreCase);
            IsHidden = preset.GetAttribute<HiddenAttribute>() != null;
            ComboType = GetComboType(preset);
        }
    }

    // Override Dalamud's GetCustomAttribute
    // It creates a new instance of every attribute every time, which is bad for performance when we want to cache them
    // This returns strictly the one and only attribute we want, and we can cache it in our own structures
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
                return field;
            var result = AllPresets.Values
                .SelectMany(attr => attr.RetargetedActions ?? [])
                .ToHashSet();
            PluginLog.Verbose($"Retrieved {result.Count} retargeted actions");
            field = result;
            return result;
        }
    } = null!;

    private static FrozenDictionary<Preset, PresetData> BuildPresets()
    {
        // Master dictionary of presets and attributes
        var dict = new Dictionary<Preset, PresetData>();

        foreach (var preset in Enum.GetValues<Preset>())
        {
            dict[preset] = new PresetData(preset);
        }

        var frozen = dict.ToFrozenDictionary();

        // Precompute parent hierarchy
        foreach (var (preset, attrs) in frozen)
        {
            if (attrs.Parent.HasValue)
            {
                var root = attrs.Parent.Value;

                while (frozen[root].Parent.HasValue)
                {
                    root = frozen[root].Parent!.Value;
                }

                attrs.RootParent = root;

                attrs.GrandParent = frozen[attrs.Parent.Value].Parent;
                attrs.GreatGrandParent = attrs.GrandParent.HasValue
                    ? frozen[attrs.GrandParent.Value].Parent
                    : null;
            }
            else
            {
                attrs.RootParent = preset;
                attrs.GrandParent = null;
                attrs.GreatGrandParent = null;
            }
        }

        PluginLog.Information($"Cached {frozen.Count} presets & attributes.");

        return frozen;
    }

    internal static FrozenSet<Preset> BuildConflictingCombos()
    {
        return AllPresets
            .Where(kvp => kvp.Value.Conflicts is { Length: > 0 })
            .Select(kvp => kvp.Key)
            .ToFrozenSet();
    }

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

    public static Preset? GetPresetByString(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        foreach (var preset in AllPresets.Keys)
        {
            if (string.Equals(preset.ToString(), value, StringComparison.OrdinalIgnoreCase))
                return preset;
        }

        return null;
    }

    public static Preset? GetPresetByInt(int value)
    {
        foreach (var preset in AllPresets.Keys)
        {
            if ((int)preset == value)
                return preset;
        }
        return null;
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
        GetPresetByString(preset) is { } pre &&
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
        GetPresetByString(preset) is { } pre &&
        DisablePreset(pre, source);

    public static bool DisablePreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        DisablePreset(pre, source);

    public static bool TogglePreset
        (Preset preset, ConfigChangeSource? source = null)
    {
        // If not already listed, enable it
        if (!Service.Configuration.EnabledActions.Remove(preset))
        {
            return EnablePreset(preset);
        }

        // Notify of change and save (only if disabling, manually)
        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.Preset, source ?? ConfigChangeSource.UI,
            preset.ToString(), false);
        P.IPCSearch.UpdateActiveJobPresets();
        Service.Configuration.Save();
        return true;
    }

    public static bool TogglePreset
        (string preset, ConfigChangeSource? source = null) =>
        GetPresetByString(preset) is { } pre &&
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
        GetPresetByString(preset) is { } pre &&
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
        GetPresetByString(preset) is { } pre &&
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
        GetPresetByString(preset) is { } pre &&
        ToggleAutoModeForPreset(pre, source);

    public static bool ToggleAutoModeForPreset
        (int preset, ConfigChangeSource? source = null) =>
        GetPresetByInt(preset) is { } pre &&
        ToggleAutoModeForPreset(pre, source);

    #endregion

    #endregion

    internal static ComboType GetComboType(Preset preset)
    {
        var simple = preset.GetAttribute<SimpleCombo>();
        var advanced = preset.GetAttribute<AdvancedCombo>();
        var basic = preset.GetAttribute<BasicCombo>();
        var healing = preset.GetAttribute<HealingCombo>();
        var mitigation = preset.GetAttribute<MitigationCombo>();
        var parent = (object?)preset.GetAttribute<ParentComboAttribute>();

        if (simple != null)
            return ComboType.Simple;
        if (advanced != null)
            return ComboType.Advanced;
        if (basic != null)
            return ComboType.Basic;

        if (healing != null)
            return ComboType.Healing;
        if (mitigation != null)
            return ComboType.Mitigation;

        if (parent == null)
            return ComboType.Feature;

        return ComboType.Option;
    }
}