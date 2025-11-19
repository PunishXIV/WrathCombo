#region

using Dalamud.Plugin;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace WrathCombo.Data.Conflicts;

/// The different types of conflicts that are checked for.
public enum ConflictType
{
    Combo,
    Targeting,
    Settings,
    GameSetting,
}

/// <summary>
///     A list of conflicts, sorted by their <see cref="ConflictType" /> internally,
///     and accessible like a dictionary.
/// </summary>
/// <remarks>
///     Access via <see cref="ToArray" /> to get all conflicts in a flat array,<br />
///     or via the <see cref="this[ConflictType]" /> indexer to get conflicts of a
///     specific type.
/// </remarks>
public class Conflicts
{
    private readonly Dictionary<ConflictType, Conflict[]> _conflicts = [];

    public Conflict[] this[ConflictType type]
    {
        get => _conflicts.TryGetValue(type, out var conflicts) ? conflicts : [];
        set => _conflicts[type] = value;
    }

    public Conflict[] ToArray() => _conflicts.Values.SelectMany(x => x).ToArray();
}

/// <summary>
///     A plugin conflict object, containing information about the offending plugin,
///     and the offence.
/// </summary>
public class Conflict
{
    /// The internal plugin's data that is offending.
    private readonly IExposedPlugin _plugin;

    /// <summary>
    ///     Create a new conflict object.
    /// </summary>
    /// <param name="internalName">The internal name of the plugin.</param>
    /// <param name="conflictType">
    ///     What <see cref="ConflictType">type</see> this conflict is.
    /// </param>
    /// <param name="reason">
    ///     The reason for the conflict, if applicable.<br />
    ///     Combo conflicts: nothing, or the part of the plugin that offends.<br />
    ///     Targeting conflicts: the actions that conflict, separated by commas.<br />
    ///     Settings conflicts: directions to the setting that conflicts.
    /// </param>
    /// <exception cref="KeyNotFoundException">
    ///     If the plugin with the given internal name was not found.
    /// </exception>
    public Conflict(
        string internalName, ConflictType conflictType, string? reason = null)
    {
        var search = Svc.PluginInterface.InstalledPlugins
            .FirstOrDefault(x => x.InternalName == internalName);

        if (conflictType is ConflictType.GameSetting)
            _xiv = true;

        _plugin = search ??
                  (_xiv
                      ? null!
                      : throw new KeyNotFoundException(
                          $"Plugin with internal name '{internalName}' not found."));
        ConflictType = conflictType;
        Reason = reason;
    }

    private readonly bool _xiv;

    /// The display name of the plugin.
    public string Name => _xiv ? "XIV" : _plugin.Name;

    /// <summary>
    ///     The internal name of the plugin, which can be used for getting a
    ///     <see cref="IExposedPlugin" /> instance from
    ///     <see cref="Svc.PluginInterface">Svc.PluginInterface.InstalledPlugins</see>.
    /// </summary>
    internal string InternalName =>_xiv ? "XIV" :  _plugin.InternalName;

    /// The version of the plugin, as a string.
    public string Version => _xiv ? "" :  "v" + _plugin.Version;

    /// What
    /// <see cref="ConflictType">type</see>
    /// this conflict is.
    internal ConflictType ConflictType { get; }

    /// <summary>
    ///     The reason for the conflict, if applicable.<br />
    ///     Combo conflicts: nothing, or the part of the plugin that offends.<br />
    ///     Targeting conflicts: the actions that conflict, separated by commas.<br />
    ///     Settings conflicts: directions to the setting that conflicts.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    ///     The parts of the conflict message that should be displayed to the user
    ///     in the UI.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If there is are not constants set for the given
    ///     <see cref="ConflictType" />.
    /// </exception>
    public string[] ConflictMessageParts =>
        ConflictType switch
        {
            ConflictType.Combo => [ComboConflictStart, ComboConflictEnd],
            ConflictType.Targeting => [TargetingConflictStart, TargetingConflictEnd],
            ConflictType.Settings => [SettingsConflictStart, SettingsConflictEnd],
            ConflictType.GameSetting => [GameConflictStart, GameConflictEnd],
            _ => throw new ArgumentOutOfRangeException(nameof(ConflictType),
                $"Unknown conflict type: {ConflictType}"),
        };

    #region UI Display Strings

    private const string ComboConflictStart = "Conflicting Combo";
    private const string ComboConflictEnd = "Plugin(s) Detected!";

    private const string TargetingConflictStart = "Conflicting Action";
    private const string TargetingConflictEnd = "Retargeting Detected!";

    private const string SettingsConflictStart = "Conflicting Plugin";
    private const string SettingsConflictEnd = "Setting(s) Detected!";

    private const string GameConflictStart = "Conflicting Game";
    private const string GameConflictEnd = "Setting(s) Detected!";

    #endregion
}