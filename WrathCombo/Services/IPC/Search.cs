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
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Tabs;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Services.IPC;

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
    ///     When <see cref="PresetStates" /> was last built.
    /// </summary>
    private DateTime _lastCacheUpdateForPresetStates = DateTime.MinValue;

    /// <summary>
    ///     Recursively finds the root parent of a given Preset.
    /// </summary>
    /// <param name="preset">The Preset to find the root parent for.</param>
    /// <returns>The root parent Preset.</returns>
    public Preset GetRootParent(Preset preset)
    {
        if (!Attribute.IsDefined(
                typeof(Preset).GetField(preset.ToString())!,
                typeof(ParentComboAttribute)))
        {
            return preset;
        }

        var parentAttribute = (ParentComboAttribute)Attribute.GetCustomAttribute(
            typeof(Preset).GetField(preset.ToString())!,
            typeof(ParentComboAttribute)
        )!;

        return GetRootParent(parentAttribute.ParentPreset);
    }

    /// <summary>
    ///     Cached list of <see cref="Preset">Presets</see>, and most of
    ///     their attribute-based information.
    /// </summary>
    [field: AllowNull, MaybeNull]
    // ReSharper disable once MemberCanBePrivate.Global
    internal Dictionary<string, (Job Job, Preset ID,
        CustomComboInfoAttribute Info, bool HasParentCombo, bool IsVariant, string
        ParentComboName, ComboType ComboType)> Presets
    {
        get
        {
            return field ??= PresetStorage.AllPresets!
                .Select(preset => new
                {
                    ID = preset,
                    JobId = preset.Attributes().CustomComboInfo.Job,
                    InternalName = preset.ToString(),
                    Info = preset.Attributes().CustomComboInfo!,
                    HasParentCombo = preset.Attributes().Parent != null,
                    IsVariant = preset.Attributes().Variant != null,
                    ParentComboName = preset.Attributes().Parent != null
                        ? GetRootParent(preset).ToString()
                        : string.Empty,
                    preset.Attributes().ComboType,
                })
                .Where(combo =>
                    !combo.InternalName.EndsWith("any", ToLower))
                .ToDictionary(
                    combo => combo.InternalName,
                    combo => (combo.JobId, combo.ID, combo.Info,
                        combo.HasParentCombo, combo.IsVariant,
                        combo.ParentComboName, combo.ComboType)
                );
        }
    }

    /// <summary>
    ///     Cached list of <see cref="Preset">Presets</see>, and the
    ///     state and Auto-Mode state of each.
    /// </summary>
    /// <remarks>
    ///     Rebuilt if the <see cref="ConfigFilePath">Config File</see> has been
    ///     updated since
    ///     <see cref="_lastCacheUpdateForPresetStates">last cached</see>.
    /// </remarks>
    [field: AllowNull, MaybeNull]
    // ReSharper disable once MemberCanBePrivate.Global
    internal Dictionary<string, Dictionary<ComboStateKeys, bool>> PresetStates
    {
        get
        {
            var presetsUpdated = (DateTime)
                (_leasing.CombosUpdated > _leasing
                    .OptionsUpdated
                    ? _leasing.CombosUpdated
                    : _leasing.OptionsUpdated ?? DateTime.MinValue);

            if (!Debug.DebugConfig)
            {
                if (field != null &&
                    File.GetLastWriteTime(ConfigFilePath) <=
                    _lastCacheUpdateForPresetStates &&
                    presetsUpdated <= _lastCacheUpdateForPresetStates)
                    return field;
            }
            else
            {
                if (field != null &&
                    !EZ.Throttle("ipcPresetStateCheck", TS.FromSeconds(1)) &&
                    presetsUpdated <= _lastCacheUpdateForPresetStates)
                    return field;
            }

            field = Presets
                .ToDictionary(
                    preset => preset.Key,
                    preset =>
                    {
                        var isEnabled =
                            CustomComboFunctions.IsEnabled(preset.Value.ID);
                        var ipcAutoMode = _leasing.CheckComboControlled(
                            preset.Value.ID.ToString())?.autoMode ?? false;
                        var isAutoMode =
                            Service.Configuration.AutoActions.TryGetValue(
                                preset.Value.ID, out var autoMode) &&
                            autoMode && preset.Value.ID.Attributes().AutoAction !=
                            null;
                        return new Dictionary<ComboStateKeys, bool>
                        {
                            { ComboStateKeys.Enabled, isEnabled },
                            { ComboStateKeys.AutoMode, isAutoMode || ipcAutoMode },
                        };
                    }
                );
            _lastCacheUpdateForPresetStates = DateTime.Now;
            UpdateActiveJobPresets();
            return field;
        }
    }

    internal void UpdateActiveJobPresets()
    {
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
        Presets
            .Where(preset =>
                preset.Value is { IsVariant: false, HasParentCombo: false } &&
                !preset.Key.Contains("pvp", ToLower))
            .GroupBy(preset => preset.Value.Job)
            .ToDictionary(
                g => g.Key,
                g => g.Select(preset => preset.Key).ToList()
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
    ///     Internal Name ->
    ///     <see cref="ComboStateKeys">State Key</see> -><br />
    ///     <c>bool</c> - Whether the state is enabled or not.
    /// </value>
    [field: AllowNull, MaybeNull]
    internal Dictionary<Job,
            Dictionary<ComboTargetTypeKeys,
                Dictionary<ComboSimplicityLevelKeys,
                    Dictionary<string, Dictionary<ComboStateKeys, bool>>>>>
        CurrentJobComboStatesCategorized
    {
        get
        {
            Job job = (WrathCombo.JobID!.Value).GetUpgradedJob();

            if (field != null && field.ContainsKey(job))
                return field;

            field = Presets
                .Where(preset =>
                    preset.Value is
                        { IsVariant: false, HasParentCombo: false } &&
                    preset.Value.Job == job &&
                    !preset.Key.Contains("pvp", ToLower))
                .SelectMany(preset => new[]
                {
                    new
                    {
                        Job = preset.Value.Info.Job,
                        Combo = preset.Key,
                        preset.Value.Info,
                        preset.Value.ComboType,
                    },
                })
                .GroupBy(x => x.Job)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x =>
                            x.ComboType switch
                            {
                                ComboType.Healing =>
                                    x.Info.Name.Contains("single target", ToLower)
                                        ? ComboTargetTypeKeys.HealST
                                        : ComboTargetTypeKeys.HealMT,
                                ComboType.Advanced or ComboType.Simple =>
                                    x.Info.Name.Contains("single target", ToLower)
                                        ? ComboTargetTypeKeys.SingleTarget
                                        : ComboTargetTypeKeys.MultiTarget,
                                _ => ComboTargetTypeKeys.Other,
                            }
                        )
                        .ToDictionary(
                            g2 => g2.Key,
                            g2 => g2.GroupBy(x =>
                                    x.ComboType switch
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
                                        x => x.Combo,
                                        x => ComboStatesByJob[x.Job][x.Combo]
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
        Presets
            .Where(preset =>
                preset.Value is { IsVariant: false, HasParentCombo: true } &&
                !preset.Key.Contains("pvp", ToLower))
            .GroupBy(preset => preset.Value.Job)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(preset => preset.Value.ParentComboName)
                    .ToDictionary(
                        g2 => g2.Key,
                        g2 => g2.Select(preset => preset.Key).ToList()
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

    /// <summary>
    ///     A wrapper for <see cref="Configuration.AutoActions" /> with
    ///     IPC settings on top.
    /// </summary>
    internal Dictionary<Preset, bool> AutoActions =>
        PresetStates
            .Where(x =>
                Enum.Parse<Preset>(x.Key).Attributes()
                    .AutoAction is not null)
            .ToDictionary(
                preset => Enum.Parse<Preset>(preset.Key),
                preset => preset.Value[ComboStateKeys.AutoMode]
            );

    /// <summary>
    ///     A wrapper for <see cref="Configuration.EnabledActions" /> with
    ///     IPC settings on top.
    /// </summary>
    internal HashSet<Preset> EnabledActions =>
        PresetStates
            .Where(preset => preset.Value[ComboStateKeys.Enabled])
            .Select(preset => Enum.Parse<Preset>(preset.Key))
            .ToHashSet();

    #endregion
}