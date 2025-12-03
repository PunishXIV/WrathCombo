using Dalamud.Utility;
using ECommons;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Attributes;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using static WrathCombo.Core.Configuration;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

namespace WrathCombo.Core;

internal static class PresetStorage
{
    private static HashSet<Preset>? PvPCombos;
    private static HashSet<Preset>? VariantCombos;
    private static HashSet<Preset>? BozjaCombos;
    private static HashSet<Preset>? OccultCrescentCombos;
    private static HashSet<Preset>? EurekaCombos;
    private static Dictionary<Preset, Preset[]>? ConflictingCombos;
    private static Dictionary<Preset, Preset?>? ParentCombos;  // child: parent

    public static HashSet<Preset>? AllPresets;

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
        PvPCombos = Enum.GetValues<Preset>()
            .Where(preset => preset.GetAttribute<PvPCustomComboAttribute>() != null)
            .ToHashSet();

        VariantCombos = Enum.GetValues<Preset>()
            .Where(preset => preset.GetAttribute<VariantAttribute>() != null)
            .ToHashSet();

        BozjaCombos = Enum.GetValues<Preset>()
            .Where(preset => preset.GetAttribute<BozjaAttribute>() != null)
            .ToHashSet();

        OccultCrescentCombos = Enum.GetValues<Preset>()
            .Where(preset => preset.GetAttribute<OccultCrescentAttribute>() != null)
            .ToHashSet();

        EurekaCombos = Enum.GetValues<Preset>()
            .Where(preset => preset.GetAttribute<EurekaAttribute>() != null)
            .ToHashSet();

        ConflictingCombos = Enum.GetValues<Preset>()
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? []);

        ParentCombos = Enum.GetValues<Preset>()
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset);

        AllPresets = Enum.GetValues<Preset>().ToHashSet();

        foreach (var preset in Enum.GetValues<Preset>())
        {
            Presets.Attributes.Add(preset, new Presets.PresetAttributes(preset));
        }
        PluginLog.Information($"Cached {Presets.Attributes.Count} preset attributes.");
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
        preset.GetAttribute<RetargetedAttribute>() != null;

    /// <summary>
    ///     Gets a value indicating whether a preset is possibly retargeted with
    ///     <see cref="ActionRetargeting" />.
    /// </summary>
    /// <param name="preset"> Preset to check. </param>
    /// <returns> The boolean representation. </returns>
    public static bool IsRetargeted(Preset preset) =>
        preset.GetAttribute<RetargetedAttribute>() != null;

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
                    removedPresets.Add(conflict);
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