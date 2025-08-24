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

    /// <summary> Iterates up a preset's parent tree, enabling each of them. </summary>
    /// <param name="preset"> Combo preset to enabled. </param>
    public static void EnableParentPresets(Preset preset)
    {
        var parentMaybe = GetParent(preset);

        while (parentMaybe != null)
        {
            EnablePreset(parentMaybe.Value);
            parentMaybe = GetParent(parentMaybe.Value);
        }
    }

    public static void DisableAllConflicts(Preset preset)
    {
        var conflicts = GetConflicts(preset);
        foreach (var conflict in conflicts)
        {
            Service.Configuration.EnabledActions.Remove(conflict);
        }
    }

    public static bool EnablePreset(string preset, bool outputLog = false)
    {
        var pre = GetPresetByString(preset);
        if (pre != null)
        {
            return EnablePreset(pre.Value, outputLog);
        }
        return false;
    }

    public static bool EnablePreset(int preset, bool outputLog = false)
    {
        var pre = GetPresetByInt(preset);
        if (pre != null)
        {
            return EnablePreset(pre.Value, outputLog);
        }
        return false;
    }

    public static bool EnablePreset(Preset preset, bool outputLog = false)
    {
        var ctrlText = GetControlledText(preset);
        EnableParentPresets(preset);
        var ret = Service.Configuration.EnabledActions.Add(preset);
        DisableAllConflicts(preset);

        if (outputLog)
            DuoLog.Information($"{(int)preset} - {preset} SET{ctrlText}");

        return ret;
    }

    public static bool DisablePreset(string preset, bool outputLog = false)
    {
        var pre = GetPresetByString(preset);
        if (pre != null)
        {
            return DisablePreset(pre.Value, outputLog);
        }
        return false;
    }

    public static bool DisablePreset(int preset, bool outputLog = false)
    {
        var pre = GetPresetByInt(preset);
        if (pre != null)
        {
            return DisablePreset(pre.Value, outputLog);
        }
        return false;
    }

    public static bool DisablePreset(Preset preset, bool outputLog = false)
    {
        if (Service.Configuration.EnabledActions.Remove(preset))
        {
            var ctrlText = GetControlledText(preset);

            if (outputLog)
                DuoLog.Information($"{(int)preset} - {preset} UNSET{ctrlText}");

            return true;
        }

        return false;
    }

    private static object GetControlledText(Preset preset)
    {
        var controlled = P.UIHelper.PresetControlled(preset) is not null;
        var ctrlText = controlled ? " " + OptionControlledByIPC : "";

        return ctrlText;
    }

    public static bool TogglePreset(Preset preset, bool outputLog = false)
    {
        var ctrlText = GetControlledText(preset);
        if (!Service.Configuration.EnabledActions.Remove(preset))
        {
            var ret = EnablePreset(preset);
            if (outputLog)
                DuoLog.Information($"{(int)preset} - {preset} SET{ctrlText}");
        }
        else if (outputLog)
        {
            DuoLog.Information($"{(int)preset} - {preset} UNSET{ctrlText}");
        }
        return false;
    }

    public static bool TogglePreset(string preset, bool outputLog = false)
    {
        var pre = GetPresetByString(preset);
        if (pre != null)
        {
            return TogglePreset(pre.Value, outputLog);
        }
        return false;
    }

    public static bool TogglePreset(int preset, bool outputLog = false)
    {
        var pre = GetPresetByInt(preset);
        if (pre != null)
        {
            return TogglePreset(pre.Value, outputLog);
        }
        return false;
    }

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