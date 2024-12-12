using System;
using System.Collections.Generic;
using System.Linq;
using ECommons.ExcelServices;
using WrathCombo.Attributes;
using WrathCombo.Combos;

namespace WrathCombo.Services.IPC;

public class Search(ref Leasing leasing)
{
    private readonly Leasing _leasing = leasing;

    /// <summary>
    ///     Searches for all combos that are enabled in Auto-Mode.
    /// </summary>
    /// <returns>
    ///     A dictionary of jobs enabled in Auto-Mode somehow.<br />
    ///     <b>Key:</b> The job abbreviation.<br />
    ///     <b>Value:</b> The internal name of the combo that is enabled in
    ///         Auto-Mode.
    /// </returns>
    internal static Dictionary<string, string> SearchForCombosInAutoMode() =>
        Service.Configuration.AutoActions
            // Only get combos with their auto-modes turned on
            .Where(action => action.Value)
            // Get the enum value of those combos
            .Select(action =>
                Enum.Parse(typeof(CustomComboPreset), action.Key.ToString()))
            // Get the actual enum value
            .Cast<CustomComboPreset>()
            // Select the internal name and job ID of the combo
            .Select(preset => new
            {
                InternalName = preset.ToString(),
                ((CustomComboInfoAttribute)Attribute.GetCustomAttribute(
                    typeof(CustomComboPreset).GetField(preset.ToString())!,
                    typeof(CustomComboInfoAttribute)
                )!).JobID
            })
            // Output a dictionary of `job abbr` -> `internal name`
            .ToDictionary(
                combo => CustomComboInfoAttribute.JobIDToShorthand(combo.JobID),
                combo => combo.InternalName
            );

    #region Aggregations of Leasing Configurations

    internal Dictionary<AutoRotationConfigOption, Dictionary<string, int>>
        AllAutoRotationConfigsControlled =>
        _leasing.Registrations.Values
            .SelectMany(registration => registration.AutoRotationConfigsControlled
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

    internal Dictionary<Job, Dictionary<string, bool>> AllJobsControlled =>
        _leasing.Registrations.Values
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

    internal Dictionary<CustomComboPreset, Dictionary<string, bool>>
        AllPresetsControlled =>
        _leasing.Registrations.Values
            .SelectMany(registration => registration.CombosControlled
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
                            .ToDictionary(x => x.PluginName, x => x.Value)
                    )
            )
            .ToDictionary(pair => pair.Key, pair => pair.Value);

    #endregion
}
