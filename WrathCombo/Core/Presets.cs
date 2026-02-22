using Dalamud.Utility;
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
using WrathCombo.Window.Functions;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using static WrathCombo.Core.Configuration;
using static WrathCombo.Window.Functions.Presets;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

namespace WrathCombo.Core;

internal static class PresetStorage
{
    private static FrozenSet<Preset>? PvPCombos;
    private static FrozenSet<Preset>? VariantCombos;
    private static FrozenSet<Preset>? BozjaCombos;
    private static FrozenSet<Preset>? OccultCrescentCombos;
    private static FrozenSet<Preset>? EurekaCombos;
    private static FrozenDictionary<Preset, Preset[]>? ConflictingCombos;
    private static FrozenDictionary<Preset, Preset?>? ParentCombos; // child: Parent

    public static FrozenDictionary<Preset, PresetAttributes>? AllPresets;

    internal class PresetAttributes
    {
        public Preset Preset { get; }
        public bool IsPvP;
        public Preset[] Conflicts;
        public Preset? Parent;
        public Preset? GrandParent;
        public Preset? GreatGrandParent;
        public Preset RootParent;
        public BlueInactiveAttribute? BlueInactive;
        public VariantAttribute? Variant;
        public PossiblyRetargetedAttribute? PossiblyRetargeted;
        public RetargetedAttribute? RetargetedAttribute;
        public uint[] RetargetedActions =>
            GetRetargetedActions(Preset, RetargetedAttribute, PossiblyRetargeted, Parent);
        public BozjaParentAttribute? BozjaParent;
        public EurekaParentAttribute? EurekaParent;
        public OccultCrescentAttribute? OccultCrescentJob;
        public HoverInfoAttribute? HoverInfo;
        public ReplaceSkillAttribute? ReplaceSkill;
        public CustomComboInfoAttribute? CustomComboInfo;
        public AutoActionAttribute? AutoAction;
        public RoleAttribute? RoleAttribute;
        public HiddenAttribute? Hidden;
        public ComboType ComboType;

        public PresetAttributes(Preset preset)
        {
            Preset = preset;
            IsPvP = preset.GetAttribute<PvPCustomComboAttribute>() != null;
            Conflicts = preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? [];
            Parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
            BlueInactive = preset.GetAttribute<BlueInactiveAttribute>();
            Variant = preset.GetAttribute<VariantAttribute>();
            PossiblyRetargeted = preset.GetAttribute<PossiblyRetargetedAttribute>();
            RetargetedAttribute = preset.GetAttribute<RetargetedAttribute>();
            BozjaParent = preset.GetAttribute<BozjaParentAttribute>();
            EurekaParent = preset.GetAttribute<EurekaParentAttribute>();
            OccultCrescentJob = preset.GetAttribute<OccultCrescentAttribute>();
            HoverInfo = preset.GetAttribute<HoverInfoAttribute>();
            ReplaceSkill = preset.GetAttribute<ReplaceSkillAttribute>();
            CustomComboInfo = preset.GetAttribute<CustomComboInfoAttribute>();
            AutoAction = preset.GetAttribute<AutoActionAttribute>();
            RoleAttribute = preset.GetAttribute<RoleAttribute>();
            Hidden = preset.GetAttribute<HiddenAttribute>();
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
            if (parent?.Attributes()?.Parent is { } grandParent &&
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
            var result = Enum.GetValues<Preset>()
                .SelectMany(preset => preset.Attributes()?.RetargetedActions ?? [])
                .ToHashSet();
            PluginLog.Verbose($"Retrieved {result.Count} retargeted actions");
            field = result;
            return result;
        }
    } = null!;

    public static void Init()
    {
        // Master Dictionary of Presets and Attributes
        var dict = new Dictionary<Preset, PresetAttributes>();
        foreach (var preset in Enum.GetValues<Preset>())
        {
            dict[preset] = new PresetAttributes(preset);
        }

        AllPresets = dict.ToFrozenDictionary();
        // Update Parents

        // Precompute RootParent for all Presets
        foreach (var (preset, attrs) in AllPresets)
        {
            if (attrs.Parent.HasValue)
            {
                // Start from the direct parent
                var root = attrs.Parent!.Value;

                // Walk up the parent chain until we reach a top-level preset (Parent is null)
                while (AllPresets[root].Parent.HasValue)
                {
                    root = AllPresets[root].Parent!.Value;
                }

                attrs.RootParent = root;

                // Also compute GrandParent and GreatGrandParent safely
                attrs.GrandParent = attrs.Parent.HasValue ? AllPresets[attrs.Parent!.Value].Parent : null;
                attrs.GreatGrandParent = attrs.GrandParent.HasValue ? AllPresets[attrs.GrandParent!.Value].Parent : null;
            }
            else
            {
                // Top-level preset → root is self
                attrs.RootParent = preset;
                attrs.GrandParent = null;
                attrs.GreatGrandParent = null;
            }
        }

        PluginLog.Information($"Cached {AllPresets.Count} presets & attributes.");


        // Build frozen sets
        PvPCombos = AllPresets
            .Where(kvp => kvp.Value.IsPvP)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

        VariantCombos = AllPresets
            .Where(kvp => kvp.Value.Variant != null)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

        BozjaCombos = AllPresets
            .Where(kvp => kvp.Value.BozjaParent != null)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

        OccultCrescentCombos = AllPresets
            .Where(kvp => kvp.Value.OccultCrescentJob != null)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

        EurekaCombos = AllPresets
            .Where(kvp => kvp.Value.EurekaParent != null)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

        ConflictingCombos = AllPresets
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Conflicts ?? [])
            .ToFrozenDictionary();

        ParentCombos = AllPresets
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Parent)
            .ToFrozenDictionary();
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
    public static bool ShouldBeHidden(Preset preset) =>
        preset.Attributes().Hidden != null &&
        !Service.Configuration.ShowHiddenFeatures;

    /// <summary> Gets a value indicating whether a preset is secret. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsPvP(Preset preset) => PvPCombos.Contains(preset);

    /// <summary> Gets a value indicating whether a preset is secret. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsVariant(Preset preset) => VariantCombos.Contains(preset);

    /// <summary>
    ///     Gets a value indicating whether a preset can be retargeted under some
    ///     settings, with <see cref="ActionRetargeting" />.
    /// </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsPossiblyRetargeted(Preset preset) =>
        AllPresets[preset].RetargetedAttribute != null;

    /// <summary>
    ///     Gets a value indicating whether a preset is possibly retargeted with
    ///     <see cref="ActionRetargeting" />.
    /// </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsRetargeted(Preset preset) =>
        AllPresets[preset].RetargetedAttribute != null;

    /// <summary> Gets a value indicating whether a preset is secret. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsBozja(Preset preset) => BozjaCombos.Contains(preset);

    /// <summary> Gets a value indicating whether a preset is secret. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsOccultCrescent(Preset preset) => OccultCrescentCombos.Contains(preset);

    /// <summary> Gets a value indicating whether a preset is secret. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsEureka(Preset preset) => EurekaCombos.Contains(preset);

    /// <summary> Gets the parent combo preset if it exists, or null. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The parent preset. </returns>
    public static Preset? GetParent(Preset preset) => ParentCombos[preset];

    /// <summary> Gets an array of conflicting combo presets. </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The conflicting presets. </returns>
    public static Preset[] GetConflicts(Preset preset) => ConflictingCombos[preset];

    /// <summary> Gets the full list of conflicted combos. </summary>
    public static List<Preset> GetAllConflicts() => ConflictingCombos.Keys.ToList();

    /// <summary> Get all the info from conflicted combos. </summary>
    public static List<Preset[]> GetAllConflictOriginals() => ConflictingCombos.Values.ToList();

    public static Preset? GetPresetByString(string value)
    {
        if (Enum.GetValues<Preset>().TryGetFirst(x => x.ToString().ToLower() == value.ToLower(), out var pre))
        {
            return pre;
        }
        return null;
    }

    public static Preset? GetPresetByInt(int value)
    {
        if (Enum.GetValues<Preset>().TryGetFirst(x => (int)x == value, out var pre))
        {
            return pre;
        }
        return null;
    }

    private static object GetControlledText(Preset preset)
    {
        var controlled = P.UIHelper.PresetControlled(preset) is not null;
        var ctrlText = controlled ? " " + OptionControlledByIPC : "";

        return ctrlText;
    }

    public static void HandleDuplicatePresets()
    {
        if (!EZ.Throttle("PeriodicPresetDeDuplicating", TS.FromSeconds(15)))
            return;

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
        var parent = (object?)preset.GetAttribute<ParentComboAttribute>() ??
                     (object?)preset.GetAttribute<BozjaParentAttribute>() ??
                     (object?)preset.GetAttribute<EurekaParentAttribute>();

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