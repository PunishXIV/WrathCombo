#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using WrathCombo.Attributes;
using WrathCombo.Combos;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;

#endregion

namespace WrathCombo.Services.IPC;

public class Search(ref Leasing leasing)
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
                        pair.Key, registration.PluginName, pair.Value,
                        registration.LastUpdated
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
                        pair.Key, registration.PluginName, pair.Value,
                        registration.LastUpdated
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
    internal Dictionary<CustomComboPreset,
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
                        pair.Key, registration.PluginName, pair.Value.enabled,
                        pair.Value.autoMode,
                        registration.LastUpdated
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
                                pair.Key, registration.PluginName, pair.Value,
                                registration.LastUpdated
                            }))
                        .GroupBy(x => x.Key)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderByDescending(x => x.LastUpdated)
                                .ToDictionary(x => x.PluginName,
                                    x => (x.Value, false))
                        )
                )
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
    private string ConfigFilePath
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
    ///     Recursively finds the root parent of a given CustomComboPreset.
    /// </summary>
    /// <param name="preset">The CustomComboPreset to find the root parent for.</param>
    /// <returns>The root parent CustomComboPreset.</returns>
    public CustomComboPreset GetRootParent(CustomComboPreset preset)
    {
        if (!Attribute.IsDefined(
                typeof(CustomComboPreset).GetField(preset.ToString())!,
                typeof(ParentComboAttribute)))
        {
            return preset;
        }

        var parentAttribute = (ParentComboAttribute)Attribute.GetCustomAttribute(
            typeof(CustomComboPreset).GetField(preset.ToString())!,
            typeof(ParentComboAttribute)
        )!;

        return GetRootParent(parentAttribute.ParentPreset);
    }

    /// <summary>
    ///     Cached list of <see cref="CustomComboPreset">Presets</see>, and most of
    ///     their attribute-based information.
    /// </summary>
    [field: AllowNull, MaybeNull]
    // ReSharper disable once MemberCanBePrivate.Global
    internal Dictionary<string, (CustomComboPreset ID,
        CustomComboInfoAttribute Info, bool HasParentCombo, bool IsVariant, string
        ParentComboName)> Presets
    {
        get
        {
            return field ??= Enum.GetValues(typeof(CustomComboPreset))
                .Cast<CustomComboPreset>()
                .Select(preset => new
                {
                    ID = preset,
                    InternalName = preset.ToString(),
                    Info = preset.Attributes().CustomComboInfo!,
                    HasParentCombo = preset.Attributes().Parent != null,
                    IsVariant = preset.Attributes().Variant != null,
                    ParentComboName = preset.Attributes().Parent != null
                        ? GetRootParent(preset).ToString()
                        : string.Empty
                })
                .Where(combo =>
                    !combo.InternalName.EndsWith("any", ToLower))
                .ToDictionary(
                    combo => combo.InternalName,
                    combo => (combo.ID, combo.Info, combo.HasParentCombo,
                        combo.IsVariant, combo.ParentComboName)
                );
        }
    }

    /// <summary>
    ///     Cached list of <see cref="CustomComboPreset">Presets</see>, and the
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

            if (field != null &&
                File.GetLastWriteTime(ConfigFilePath) <=
                _lastCacheUpdateForPresetStates &&
                presetsUpdated <= _lastCacheUpdateForPresetStates)
                return field;

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
                                preset.Value.ID, out bool autoMode) &&
                            autoMode;
                        return new Dictionary<ComboStateKeys, bool>
                        {
                            { ComboStateKeys.Enabled, isEnabled },
                            { ComboStateKeys.AutoMode, isAutoMode || ipcAutoMode }
                        };
                    }
                );
            _lastCacheUpdateForPresetStates = DateTime.Now;

            return field;
        }
    }

    #endregion

    #region Combo Information

    /// <summary>
    ///     The names of each combo.
    /// </summary>
    /// <value>
    ///     Job -> <c>list</c> of combo internal names.
    /// </value>
    internal Dictionary<string, List<string>> ComboNamesByJob =>
        Presets
            .Where(preset =>
                preset.Value is { IsVariant: false, HasParentCombo: false } &&
                !preset.Key.Contains("pvp", ToLower))
            .GroupBy(preset =>
                CustomComboFunctions.JobIDs.JobIDToShorthand(preset.Value.Info
                    .JobID))
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
    internal Dictionary<string,
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
    ///     When <see cref="ComboStatesByJobCategorized" /> was last built.
    /// </summary>
    private DateTime _lastCacheUpdateForComboStatesByJobCategorized =
        DateTime.MinValue;

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
    internal Dictionary<string,
            Dictionary<ComboTargetTypeKeys,
                Dictionary<ComboSimplicityLevelKeys,
                    Dictionary<string, Dictionary<ComboStateKeys, bool>>>>>
        ComboStatesByJobCategorized
    {
        get
        {
            if (field != null &&
                File.GetLastWriteTime(ConfigFilePath) <=
                _lastCacheUpdateForComboStatesByJobCategorized)
                return field;

            field = Presets
                .Where(preset =>
                    preset.Value is { IsVariant: false, HasParentCombo: false } &&
                    !preset.Key.Contains("pvp", ToLower))
                .SelectMany(preset => new[]
                {
                    new
                    {
                        Job = CustomComboFunctions.JobIDs.JobIDToShorthand(preset
                            .Value
                            .Info
                            .JobID),
                        Combo = preset.Key,
                        preset.Value.Info
                    }
                })
                .GroupBy(x => x.Job)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x =>
                            x.Info.Name.Contains("heals - single", ToLower)
                                ? ComboTargetTypeKeys.HealST
                                : x.Info.Name.Contains("heals - aoe", ToLower)
                                    ? ComboTargetTypeKeys.HealMT
                                    : x.Info.Name.Contains("- aoe", ToLower) ||
                                      x.Info.Name.Contains("aoe dps feature",
                                          ToLower)
                                        ? ComboTargetTypeKeys.MultiTarget
                                        : x.Info.Name.Contains("- single target",
                                              ToLower) ||
                                          x.Info.Name.Contains(
                                              "single target dps feature",
                                              ToLower)
                                            ? ComboTargetTypeKeys.SingleTarget
                                            : ComboTargetTypeKeys.Other
                        )
                        .ToDictionary(
                            g2 => g2.Key,
                            g2 => g2.GroupBy(x =>
                                    x.Info.Name.Contains("advanced mode -",
                                        ToLower) ||
                                    x.Info.Name.Contains("dps feature", ToLower)
                                        ? ComboSimplicityLevelKeys.Advanced
                                        : x.Info.Name.Contains("simple mode -",
                                            ToLower)
                                            ? ComboSimplicityLevelKeys.Simple
                                            : ComboSimplicityLevelKeys.Other
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
            _lastCacheUpdateForComboStatesByJobCategorized = DateTime.Now;

            return field;
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
    internal Dictionary<string,
            Dictionary<string,
                List<string>>>
        OptionNamesByJob =>
        Presets
            .Where(preset =>
                preset.Value is { IsVariant: false, HasParentCombo: true } &&
                !preset.Key.Contains("pvp", ToLower))
            .GroupBy(preset =>
                CustomComboFunctions.JobIDs.JobIDToShorthand(preset.Value.Info
                    .JobID))
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
    internal Dictionary<string,
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
                                    }
                                }
                            )
                    )
            );

    #endregion

    /// <summary>
    ///     A wrapper for <see cref="Core.PluginConfiguration.AutoActions" /> with
    ///     IPC settings on top.
    /// </summary>
    internal Dictionary<CustomComboPreset, bool> AutoActions =>
        PresetStates
            .ToDictionary(
                preset => Enum.Parse<CustomComboPreset>(preset.Key),
                preset => preset.Value[ComboStateKeys.AutoMode]
            );

    #endregion
}
