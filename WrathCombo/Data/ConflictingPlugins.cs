#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;

#endregion

namespace WrathCombo.Data;

public static class ConflictingPlugins
{
    /// <summary>
    ///     Gets all current conflicts.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicts.
    /// </param>
    /// <returns>
    ///     Whether there are any conflicts at all.
    /// </returns>
    public static bool TryGetConflicts(out Conflicts conflicts)
    {
        conflicts = new Conflicts();

        var hasSimpleConflicts = TryGetSimpleComboConflicts(out var simpleCombos);
        var hasComplexConflicts = TryGetComplexComboConflicts(out var complexCombos);
        if (hasSimpleConflicts || hasComplexConflicts)
            conflicts[ConflictType.Combo] =
                simpleCombos.Concat(complexCombos).ToArray();

        if (TryGetTargetingConflicts(out var targetingConflicts))
            conflicts[ConflictType.Targeting] = targetingConflicts;

        if (TryGetSettingConflicts(out var settingConflicts))
            conflicts[ConflictType.Settings] = settingConflicts;

        return conflicts.ToArray().Length > 0;
    }

    /// <summary>
    ///     Draws all conflicts for the user to see.<br />
    ///     For <see cref="Window.ConfigWindow" />.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If a color was not added for a new <see cref="ConflictType" /> value.
    /// </exception>
    public static void Draw()
    {
        if (!TryGetConflicts(out var conflicts))
            return;

        Conflict[] currentConflicts = [];
        var hasComboConflicts = conflicts[ConflictType.Combo].Length > 0;
        var hasTargetingConflicts = conflicts[ConflictType.Targeting].Length > 0;
        var hasSettingsConflicts = conflicts[ConflictType.Settings].Length > 0;

        ImGui.Spacing();
        ImGui.Spacing();

        if (hasComboConflicts)
        {
            currentConflicts = conflicts[ConflictType.Combo];
            var conflictingPluginsText = "- " + string.Join("\n- ",
                conflicts[ConflictType.Combo]
                    .Select(x => $"{x.Name} v{x.Version}" +
                                 (string.IsNullOrEmpty(x.Reason)
                                     ? ""
                                     : $" ({x.Reason})")));
            var tooltipText =
                "The following plugins are known to conflict " +
                $"with {Svc.PluginInterface.InternalName}:\n" +
                conflictingPluginsText +
                "\n\nIt is recommended you disable these plugins to prevent\n" +
                "unexpected behavior and bugs.";

            ShowWarning(ConflictType.Combo, tooltipText, false);
        }

        if (hasTargetingConflicts)
        {
            currentConflicts = conflicts[ConflictType.Targeting];
            var tooltipText =
                "The following plugins are known to conflict with\n" +
                $"{Svc.PluginInterface.InternalName}'s Action Retargeting, which you have enabled:";

            foreach (var conflict in conflicts[ConflictType.Targeting])
                tooltipText +=
                    $"\n- {conflict.Name} v{conflict.Version}" +
                    $"\n    Actions:\n        - " +
                    string.Join("\n        - ", conflict.Reason.Split(','));

            tooltipText +=
                "\n\nIt is recommended you disable these plugins, or\n" +
                "remove the conflicting actions from their settings, or\n" +
                $"disable Retargeting for the action in {Svc.PluginInterface.InternalName},\n" +
                "to prevent unexpected behavior and bugs.";

            ShowWarning(ConflictType.Targeting, tooltipText, hasComboConflicts);
        }

        if (hasSettingsConflicts)
        {
            currentConflicts = conflicts[ConflictType.Settings];
            var conflictingSettingsText = "- " + string.Join("\n- ",
                conflicts[ConflictType.Combo]
                    .Select(x => $"{x.Name} v{x.Version} (setting: {x.Reason})"));

            var tooltipText =
                "The following plugins are known to conflict with\n" +
                $"{Svc.PluginInterface.InternalName}'s Settings, which you have enabled:" +
                conflictingSettingsText +
                "\n\nIt is recommended you disable these plugins, or\n" +
                "remove the conflicting setting in the plugins\n" +
                "to prevent unexpected behavior and bugs.";

            ShowWarning(ConflictType.Settings, tooltipText,
                hasComboConflicts || hasTargetingConflicts);
        }

        return;

        void ShowWarning(ConflictType type, string tooltipText, bool hasWarningAbove)
        {
            var color = type switch
            {
                ConflictType.Combo => ImGuiColors.DalamudRed,
                ConflictType.Targeting => ImGuiColors.DalamudYellow,
                ConflictType.Settings => ImGuiColors.DalamudOrange,
                _ => throw new ArgumentOutOfRangeException(nameof(type),
                    $"Unknown conflict type: {type}"),
            };

            if (hasWarningAbove)
                ImGui.Spacing();

            var conflictMessage = currentConflicts.ToArray()[0].ConflictMessageParts;
            var twoLines = ImGui.GetColumnWidth() <=
                           ImGui.CalcTextSize(conflictMessage[0] + " " +
                                              conflictMessage[1]).X.Scale();
            
            ImGui.BeginGroup();
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 2));
            if (twoLines)
            {
                ImGuiEx.LineCentered($"###Conflicting{type}Plugins0", () =>
                    ImGui.TextColored(color, conflictMessage[0])
                );
                ImGuiEx.LineCentered($"###Conflicting{type}Plugins1", () =>
                    ImGui.TextColored(color, conflictMessage[1])
                );
            }
            else
                ImGuiEx.LineCentered($"###Conflicting{type}Plugins0", () =>
                    ImGui.TextColored(color, conflictMessage[0] + " " +
                                             conflictMessage[1])
                );
            ImGui.PopStyleVar();
            ImGui.EndGroup();

            // Tooltip with explanation
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(tooltipText);
        }
    }

    #region Targeting Conflicts

    /// <summary>
    ///     Checks for targeting conflicts, which are also more complicated and
    ///     requires checking the settings of the plugins.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicting plugins.
    /// </param>
    /// <returns>
    ///     Whether there are targeting conflicts.
    /// </returns>
    private static bool TryGetTargetingConflicts(out Conflict[] conflicts)
    {
        conflicts = [];

        // Redirect
        // Reaction
        if (ConflictingPluginsChecks.MOAction.Conflicted)
            conflicts = conflicts.Append(new Conflict(
                    "MOAction", ConflictType.Targeting,
                    "blackest night"))
                .ToArray();

        return conflicts.Length > 0;
    }

    #endregion

    #region Setting Conflicts

    /// <summary>
    ///     Checks for conflicts from specific settings in other plugins,
    ///     like those that modify action queueing.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicting plugins.
    /// </param>
    /// <returns>
    ///     Whether there are settings conflicts.
    /// </returns>
    private static bool TryGetSettingConflicts(out Conflict[] conflicts)
    {
        conflicts = [];

        // BossMod

        return conflicts.Length > 0;
    }

    #endregion

    #region Combo Conflicts

    /// <summary>
    ///     List of the most popular conflicting plugins.
    /// </summary>
    /// <remarks>
    ///     The list is case-sensitive, and needs to be lowercase.
    /// </remarks>
    private static string[] conflictingPluginsNames = new[]
    {
        "xivcombo",
        "xivcomboexpanded",
        "xivcomboexpandedest",
        "xivcombovx",
        "xivslothcombo",
        "rotationsolver",
    };

    /// <summary>
    ///     Searches for any enabled conflicting plugins.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicting plugins.
    /// </param>
    /// <returns>
    ///     Whether there are any simple combo conflicts.
    /// </returns>
    private static bool TryGetSimpleComboConflicts(out Conflict[] conflicts)
    {
        conflicts = Svc.PluginInterface.InstalledPlugins
            .Where(x =>
                Enumerable.Contains(conflictingPluginsNames,
                    x.InternalName.ToLower()) &&
                x.IsLoaded)
            .Select(x => new Conflict(x.InternalName, ConflictType.Combo))
            .ToArray();

        return conflicts.Length > 0;
    }

    /// <summary>
    ///     Checks for nuanced conflicts, which are only conflicts under
    ///     certain conditions, and as such we actually need to check the settings
    ///     of such plugins.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicting plugins.
    /// </param>
    /// <returns>
    ///     Whether there are any complex combo conflicts.
    /// </returns>
    private static bool TryGetComplexComboConflicts(out Conflict[] conflicts)
    {
        conflicts = [];

        if (ConflictingPluginsChecks.BossMod.Conflicted)
            conflicts = conflicts.Append(new Conflict(
                "BossMod", ConflictType.Combo,
                "is queueing actions!"))
                .ToArray();
        if (ConflictingPluginsChecks.BossModReborn.Conflicted)
            conflicts = conflicts.Append(new Conflict(
                "BossModReborn", ConflictType.Combo,
                "is queueing actions!"))
                .ToArray();

        return conflicts.Length > 0;
    }

    #endregion
}

/// The different types of conflicts that are checked for.
public enum ConflictType
{
    Combo,
    Targeting,
    Settings,
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

        _plugin = search ?? throw new KeyNotFoundException(
            $"Plugin with internal name '{internalName}' not found.");
        ConflictType = conflictType;
        Reason = reason;
    }

    /// The display name of the plugin.
    public string Name => _plugin.Name;

    /// <summary>
    ///     The internal name of the plugin, which can be used for getting a
    ///     <see cref="IExposedPlugin" /> instance from
    ///     <see cref="Svc.PluginInterface">Svc.PluginInterface.InstalledPlugins</see>.
    /// </summary>
    internal string InternalName => _plugin.InternalName;

    /// The version of the plugin, as a string.
    public string Version => _plugin.Version.ToString();

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

    #endregion
}
