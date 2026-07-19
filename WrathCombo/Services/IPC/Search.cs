#region

using ECommons.DalamudServices;
using ECommons.ExcelServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using WrathCombo.API.Enum;
using WrathCombo.Attributes;
using WrathCombo.Core;
using static WrathCombo.CustomComboNS.Functions.Jobs;
using WrathCombo.CustomComboNS.Functions;

#endregion

namespace WrathCombo.Services.IPC;

/// <summary>
///     Wraps a <see cref="PresetStorage.PresetData" /> reference together with
///     its current, mutable runtime state (enabled / auto-mode) as seen
///     through the IPC layer (i.e. accounting for lease overrides).
/// </summary>
/// <remarks>
///     Callers that need attribute data (<see cref="PresetStorage.PresetData.AutoAction" />,
///     <see cref="PresetStorage.PresetData.Parent" />, etc.) alongside runtime
///     state can read <see cref="Data" /> directly, instead of re-deriving a
///     <see cref="Preset" /> from a string and re-querying its attributes.
/// </remarks>
internal sealed class PresetRuntimeState(PresetStorage.PresetData data)
{
    /// <summary>
    ///     The static, attribute-derived data for this preset.
    /// </summary>
    public PresetStorage.PresetData Data { get; } = data;

    /// <summary>
    ///     Whether the preset is currently enabled, including any lease
    ///     override.
    /// </summary>
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Whether the preset is currently in Auto-Mode, including any lease
    ///     override.
    /// </summary>
    public required bool AutoMode { get; init; }
}

public class Search(Leasing leasing)
{
    /// <summary>
    ///     A shortcut for <see cref="StringComparison.CurrentCultureIgnoreCase" />.
    /// </summary>
    private const StringComparison ToLower =
        StringComparison.CurrentCultureIgnoreCase;

    private readonly Leasing _leasing = leasing;

    #region Aggregations of Leasing Configurations

    /// <summary>
    ///     When <see cref="AllAutoRotationConfigsControlled" /> was last cached.
    /// </summary>
    /// <seealso cref="Leasing.AutoRotationConfigsUpdated" />
    internal DateTime? LastCacheUpdateForAutoRotationConfigs;

    /// <summary>
    ///     Lists all auto-rotation configurations controlled under leases.
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal Dictionary<AutoRotationConfigOption, Dictionary<string, int>>
        AllAutoRotationConfigsControlled
    {
        get
        {
            if (field is not null &&
                LastCacheUpdateForAutoRotationConfigs is not null &&
                _leasing.AutoRotationConfigsUpdated ==
                LastCacheUpdateForAutoRotationConfigs)
                return field;

            field = _leasing.Registrations.Values
                .SelectMany(registration => registration
                    .AutoRotationConfigsControlled
                    .Select(pair => new
                    {
                        pair.Key,
                        registration.PluginName,
                        pair.Value,
                        registration.LastUpdated,
                    }))
                .GroupBy(x => x.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.LastUpdated)
                        .ToDictionary(x => x.PluginName, x => x.Value)
                );

            LastCacheUpdateForAutoRotationConfigs =
                _leasing.AutoRotationConfigsUpdated;
            return field;
        }
    }

    /// <summary>
    ///     When <see cref="AllJobsControlled" /> was last cached.
    /// </summary>
    /// <seealso cref="Leasing.JobsUpdated" />
    internal DateTime? LastCacheUpdateForAllJobsControlled;

    /// <summary>
    ///     Lists all jobs controlled under leases.
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal Dictionary<Job, Dictionary<string, bool>> AllJobsControlled
    {
        get
        {
            if (field is not null &&
                LastCacheUpdateForAllJobsControlled is not null &&
                _leasing.JobsUpdated == LastCacheUpdateForAllJobsControlled)
                return field;

            field = _leasing.Registrations.Values
                .SelectMany(registration => registration.JobsControlled
                    .Select(pair => new
                    {
                        pair.Key,
                        registration.PluginName,
                        pair.Value,
                        registration.LastUpdated,
                    }))
                .GroupBy(x => x.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.LastUpdated)
                        .ToDictionary(x => x.PluginName, x => x.Value)
                );

            LastCacheUpdateForAllJobsControlled = _leasing.JobsUpdated;
            return field;
        }
    }

    /// <summary>
    ///     When <see cref="AllPresetsControlled" /> was last cached.
    /// </summary>
    /// <seealso cref="Leasing.CombosUpdated" />
    /// <seealso cref="Leasing.OptionsUpdated" />
    internal DateTime? LastCacheUpdateForAllPresetsControlled;

    /// <summary>
    ///     Lists all presets controlled under leases.<br />
    ///     Include both combos and options, but also jobs' options.
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal Dictionary<Preset,
            Dictionary<string, (bool enabled, bool autoMode)>>
        AllPresetsControlled
    {
        get
        {
            var presetsUpdated = (DateTime)
                (_leasing.CombosUpdated > _leasing
                    .OptionsUpdated
                    ? _leasing.CombosUpdated
                    : _leasing.OptionsUpdated ?? DateTime.MinValue);

            if (field is not null &&
                LastCacheUpdateForAllPresetsControlled is not null &&
                presetsUpdated == LastCacheUpdateForAllPresetsControlled)
                return field;

            field = _leasing.Registrations.Values
                .SelectMany(registration => registration.CombosControlled
                    .Select(pair => new
                    {
                        pair.Key,
                        registration.PluginName,
                        pair.Value.enabled,
                        pair.Value.autoMode,
                        registration.LastUpdated,
                    }))
                .GroupBy(x => x.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.LastUpdated)
                        .ToDictionary(x => x.PluginName,
                            x => (x.enabled, x.autoMode))
                )
                .Concat(
                    _leasing.Registrations.Values
                        .SelectMany(registration => registration.OptionsControlled
                            .Select(pair => new
                            {
                                pair.Key,
                                registration.PluginName,
                                pair.Value,
                                registration.LastUpdated,
                            }))
                        .GroupBy(x => x.Key)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderByDescending(x => x.LastUpdated)
                                .ToDictionary(x => x.PluginName,
                                    x => (x.Value, false))
                        )
                )
                .DistinctBy(x => x.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            LastCacheUpdateForAllPresetsControlled = presetsUpdated;
            return field;
        }
    }

    #endregion

    #region Presets Information

    #region Cached Preset Info

    /// <summary>
    ///     The path to the configuration file for Wrath Combo.
    /// </summary>
    internal string ConfigFilePath
    {
        get
        {
            var pluginConfig = Svc.PluginInterface.GetPluginConfigDirectory();
            if (Path.EndsInDirectorySeparator(pluginConfig))
                pluginConfig = Path.TrimEndingDirectorySeparator(pluginConfig);
            pluginConfig =
                pluginConfig
                    [..pluginConfig.LastIndexOf(Path.DirectorySeparatorChar)];
            pluginConfig = Path.Combine(pluginConfig, "WrathCombo.json");
            return pluginConfig;
        }
    }

    /// <summary>
    ///     Set to <c>true</c> to force <see cref="PresetRuntimeStates" />/
    ///     <see cref="PresetStates" /> to rebuild on next access. Cleared
    ///     automatically once the rebuild completes.
    /// </summary>
    internal bool UpdateDue = true;

    private Dictionary<Preset, PresetRuntimeState>? _presetRuntimeStates;

    private Dictionary<string, Dictionary<ComboStateKeys, bool>>?
        _presetStatesByName;

    /// <summary>
    ///     Rebuilds the preset-state caches if <see cref="UpdateDue" /> is
    ///     set. Otherwise a no-op.
    /// </summary>
    /// <remarks>
    ///     This is the single source of truth for preset runtime state:
    ///     <see cref="PresetRuntimeStates" /> (keyed by <see cref="Preset" />,
    ///     for internal use, carrying a direct <see cref="PresetStorage.PresetData" />
    ///     reference) and <see cref="PresetStates" /> (keyed by internal name
    ///     string, for the IPC boundary) are both projected from the same
    ///     build, so nothing downstream needs to re-derive a <see cref="Preset" />
    ///     from a string, or re-run this work per access.
    /// </remarks>
    private void EnsurePresetStatesCurrent()
    {
        if (_presetRuntimeStates != null && !UpdateDue)
            return;

        // Walk every lease once, keeping only the most-recently-updated
        // combo override per Preset — avoids an O(leases) scan being
        // repeated for every preset below.
        var latestComboOverrides = _leasing.Registrations.Values
            .SelectMany(registration => registration.CombosControlled
                .Select(pair => new
                {
                    pair.Key,
                    pair.Value.autoMode,
                    registration.LastUpdated,
                }))
            .GroupBy(x => x.Key)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(x => x.LastUpdated).First().autoMode
            );

        _presetRuntimeStates = PresetStorage.AllPresets
            .ToDictionary(
                preset => preset.Key,
                preset =>
                {
                    var isEnabled =
                        CustomComboFunctions.IsEnabled(preset.Key);
                    var ipcAutoMode =
                        latestComboOverrides.GetValueOrDefault(preset.Key);
                    var isAutoMode =
                        Service.Configuration.AutoActions.TryGetValue(
                            preset.Key, out var autoMode) &&
                        autoMode && preset.Value.AutoAction != null;

                    return new PresetRuntimeState(preset.Value)
                    {
                        Enabled = isEnabled,
                        AutoMode = isAutoMode || ipcAutoMode,
                    };
                }
            );

        _presetStatesByName = _presetRuntimeStates.ToDictionary(
            preset => preset.Value.Data.InternalName,
            preset => new Dictionary<ComboStateKeys, bool>
            {
                { ComboStateKeys.Enabled, preset.Value.Enabled },
                { ComboStateKeys.AutoMode, preset.Value.AutoMode },
            }
        );

        UpdateDue = false;
    }

    /// <summary>
    ///     Cached list of <see cref="Preset">Presets</see> and their current
    ///     runtime state, keyed by <see cref="Preset" /> for internal
    ///     (non-IPC) use. Each entry carries a direct reference to its
    ///     <see cref="PresetStorage.PresetData" />, so callers never need to
    ///     re-derive attribute data via string parsing.
    /// </summary>
    internal Dictionary<Preset, PresetRuntimeState> PresetRuntimeStates
    {
        get
        {
            EnsurePresetStatesCurrent();
            return _presetRuntimeStates!;
        }
    }

    /// <summary>
    ///     Cached list of <see cref="Preset">Presets</see>, and the
    ///     state and Auto-Mode state of each, keyed by internal name string.
    /// </summary>
    /// <remarks>
    ///     This string-keyed shape exists for the IPC boundary
    ///     (<see cref="Provider.GetComboState" />,
    ///     <see cref="Provider.GetComboOptionState" />), since external
    ///     plugins can only address presets by name/ID across the IPC
    ///     boundary, not by the internal <see cref="Preset" /> enum type.
    ///     Internal (non-IPC) callers should prefer
    ///     <see cref="PresetRuntimeStates" /> instead.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    internal Dictionary<string, Dictionary<ComboStateKeys, bool>> PresetStates
    {
        get
        {
            EnsurePresetStatesCurrent();
            return _presetStatesByName!;
        }
    }

    internal void UpdateActiveJobPresets()
    {
        Window.Functions.Presets.UpdateDue = true;
        ActiveJobPresets = Window.Functions.Presets.GetJobAutorots.Count;
    }

    internal int ActiveJobPresets;

    #endregion

    #region Combo Information

    /// <summary>
    ///     The names of each combo.
    /// </summary>
    /// <value>
    ///     Job -> <c>list</c> of combo internal names.
    /// </value>
    internal Dictionary<Job, List<string>> ComboNamesByJob =>
        PresetRuntimeStates
            .Where(preset =>
                preset.Value.Data is { IsVariant: false, Parent: null, IsPvP: false })
            .GroupBy(preset => preset.Value.Data.JobInfo!.Job)
            .ToDictionary(
                g => g.Key,
                g => g.Select(preset => preset.Value.Data.InternalName).ToList()
            );

    /// <summary>
    ///     The states of each combo.
    /// </summary>
    /// <value>
    ///     Job -> Internal Name ->
    ///     <see cref="ComboStateKeys">State Key</see> -><br />
    ///     <c>bool</c> - Whether the state is enabled or not.
    /// </value>
    internal Dictionary<Job,
            Dictionary<string, Dictionary<ComboStateKeys, bool>>>
        ComboStatesByJob =>
        ComboNamesByJob
            .ToDictionary(
                job => job.Key,
                job => job.Value
                    .ToDictionary(
                        combo => combo,
                        combo => PresetStates[combo]
                    )
            );

    /// <summary>
    ///     The states of each combo, but heavily categorized.
    /// </summary>
    /// <value>
    ///     Job -> <see cref="ComboTargetTypeKeys">Target Key</see> ->
    ///     <see cref="ComboSimplicityLevelKeys">Simplicity Key</see> ->
    ///     <see cref="Preset" /> ->
    ///     <see cref="ComboStateKeys">State Key</see> -><br />
    ///     <c>bool</c> - Whether the state is enabled or not.
    /// </value>
    /// <remarks>
    ///     Keyed by <see cref="Preset" /> (rather than internal name string)
    ///     since this is internal-only — see <see cref="ComboNamesByJob" />/
    ///     <see cref="OptionNamesByJob" /> for the string-keyed, IPC-facing
    ///     equivalents.
    /// </remarks>
    [field: AllowNull, MaybeNull]
    internal Dictionary<Job,
            Dictionary<ComboTargetTypeKeys,
                Dictionary<ComboSimplicityLevelKeys,
                    Dictionary<Preset, Dictionary<ComboStateKeys, bool>>>>>
        CurrentJobComboStatesCategorized
    {
        get
        {
            Job job = (WrathCombo.JobID!.Value).GetUpgradedJob();

            if (field != null && field.ContainsKey(job))
                return field;

            field = PresetRuntimeStates
                .Where(preset =>
                    preset.Value.Data is
                    { IsVariant: false, Parent: null, IsPvP: false } &&
                    preset.Value.Data.JobInfo!.Job == job)
                .GroupBy(preset => preset.Value.Data.JobInfo!.Job)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.Value.Data.TargetType)
                        .ToDictionary(
                            g2 => g2.Key,
                            g2 => g2.GroupBy(x =>
                                    x.Value.Data.ComboType switch
                                    {
                                        ComboType.Advanced =>
                                            ComboSimplicityLevelKeys.Advanced,
                                        ComboType.Simple =>
                                            ComboSimplicityLevelKeys.Simple,
                                        _ => ComboSimplicityLevelKeys.Other,
                                    }
                                )
                                .ToDictionary(
                                    g3 => g3.Key,
                                    g3 => g3.ToDictionary(
                                        x => x.Key,
                                        x => new Dictionary<ComboStateKeys, bool>
                                        {
                                            {
                                                ComboStateKeys.Enabled,
                                                x.Value.Enabled
                                            },
                                            {
                                                ComboStateKeys.AutoMode,
                                                x.Value.AutoMode
                                            },
                                        }
                                    )
                                )
                        )
                );

            Svc.Log.Verbose($"IPC Combo Built for {job}");

            return field ?? [];
        }
    }

    #endregion

    #region Options Information

    /// <summary>
    ///     The names of each option.
    /// </summary>
    /// <value>
    ///     Job -> Parent Combo Internal Name ->
    ///     <c>list</c> of option internal names.
    /// </value>
    internal Dictionary<Job,
            Dictionary<string,
                List<string>>>
        OptionNamesByJob =>
        PresetRuntimeStates
            .Where(preset =>
                preset.Value.Data is
                { IsVariant: false, Parent: not null, IsPvP: false })
            .GroupBy(preset => preset.Value.Data.JobInfo!.Job)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(preset =>
                        PresetStorage.AllPresets[preset.Value.Data.RootParent]
                            .InternalName)
                    .ToDictionary(
                        g2 => g2.Key,
                        g2 => g2.Select(preset => preset.Value.Data.InternalName)
                            .ToList()
                    )
            );

    /// <summary>
    ///     The states of each option.
    /// </summary>
    /// <value>
    ///     Job -> Parent Combo Internal Name -> Option Internal Name ->
    ///     State Key (really just <see cref="ComboStateKeys.Enabled" />) ->
    ///     <c>bool</c> - Whether the option is enabled or not.
    /// </value>
    internal Dictionary<Job,
            Dictionary<string,
                Dictionary<string,
                    Dictionary<ComboStateKeys, bool>>>>
        OptionStatesByJob =>
        OptionNamesByJob
            .ToDictionary(
                job => job.Key,
                job => job.Value
                    .ToDictionary(
                        parentCombo => parentCombo.Key,
                        parentCombo => parentCombo.Value
                            .ToDictionary(
                                option => option,
                                option => new Dictionary<ComboStateKeys, bool>
                                {
                                    {
                                        ComboStateKeys.Enabled,
                                        PresetStates[option][ComboStateKeys.Enabled]
                                    },
                                }
                            )
                    )
            );

    #endregion

    #region Variant Dungeons

    private static readonly Dictionary<JobRole, string> VariantParentNames = new()
    {
        { JobRole.Tank, nameof(Preset.Variant_Tank) },
        { JobRole.Healer, nameof(Preset.Variant_Healer) },
        { JobRole.MeleeDPS, nameof(Preset.Variant_Melee) },
        { JobRole.RangedDPS, nameof(Preset.Variant_PhysRanged) },
        { JobRole.MagicalDPS, nameof(Preset.Variant_Magic) },
    };

    internal bool TryGetVariantJobRole(uint jobID, out JobRole jobRole)
    {
        jobRole = GetRoleFromJob(jobID);

        return jobRole is JobRole.Tank or JobRole.Healer or JobRole.MeleeDPS
            or JobRole.RangedDPS or JobRole.MagicalDPS;
    }

    internal string? GetVariantParentComboName(JobRole role) =>
        VariantParentNames.GetValueOrDefault(role);

    internal List<string> GetVariantOptionNames(JobRole role)
    {
        if (!VariantParentNames.TryGetValue(role, out var parent))
            return [];

        return PresetRuntimeStates
            .Where(preset =>
                preset.Value.Data is { IsVariant: true, Parent: not null } &&
                PresetStorage.AllPresets[preset.Value.Data.RootParent]
                    .InternalName == parent)
            .Select(preset => preset.Value.Data.InternalName)
            .ToList();
    }

    #endregion

    /// <summary>
    ///     A wrapper for <see cref="Configuration.AutoActions" /> with
    ///     IPC settings on top.
    /// </summary>
    internal Dictionary<Preset, bool> AutoActions =>
        PresetRuntimeStates
            .Where(preset => preset.Value.Data.AutoAction is not null)
            .ToDictionary(
                preset => preset.Key,
                preset => preset.Value.AutoMode
            );

    /// <summary>
    ///     A wrapper for <see cref="Configuration.EnabledActions" /> with
    ///     IPC settings on top.
    /// </summary>
    internal HashSet<Preset> EnabledActions =>
        PresetRuntimeStates
            .Where(preset => preset.Value.Enabled)
            .Select(preset => preset.Key)
            .ToHashSet();

    #endregion
}