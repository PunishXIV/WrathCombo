#region

using Dalamud.Interface.Colors;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using System;
using System.Linq;
using System.Numerics;
using WrathCombo.Extensions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Data.Conflicts;

public static class ConflictingPlugins
{
    /// <summary>
    ///     Cache for <see cref="TryGetConflicts" /> results.
    /// </summary>
    private static Conflicts? _cachedConflicts;

    /// <summary>
    ///     Gets all current conflicts.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicts.
    /// </param>
    /// <param name="forceRefresh">
    ///     Whether to force a refresh of the conflicts, ignoring the cache.
    /// </param>
    /// <returns>
    ///     Whether there are any conflicts at all.
    /// </returns>
    public static bool TryGetConflicts
        (out Conflicts conflicts, bool forceRefresh = false)
    {
        // Only check for new conflicts periodically, not refreshed on demand anyway
        if (!EZ.Throttle("conflictCheck", TS.FromSeconds(1.5)) &&
            _cachedConflicts is not null &&
            !forceRefresh)
        {
            conflicts = _cachedConflicts;
            return _cachedConflicts.ToArray().Length > 0;
        }

        conflicts = new Conflicts();

        // try/catch blocks are here for issues with the Conflict instantiations

        try
        {
            var hasSimpleConflicts =
                TryGetSimpleComboConflicts(out var simpleCombos);
            var hasComplexConflicts =
                TryGetComplexComboConflicts(out var complexCombos);
            if (hasSimpleConflicts || hasComplexConflicts)
                conflicts[ConflictType.Combo] =
                    simpleCombos.Concat(complexCombos).ToArray();
        }
        catch (Exception e)
        {
            PluginLog.Error(
                "[ConflictingPlugins] Failed to check for combo conflicts: " +
                e.ToStringFull());
        }

        try
        {
            if (TryGetTargetingConflicts(out var targetingConflicts))
                conflicts[ConflictType.Targeting] = targetingConflicts;
        }
        catch (Exception e)
        {
            PluginLog.Error(
                "[ConflictingPlugins] Failed to check for targeting conflicts: " +
                e.ToStringFull());
        }

        try
        {
            if (TryGetSettingConflicts(out var settingConflicts))
                conflicts[ConflictType.Settings] = settingConflicts;
        }
        catch (Exception e)
        {
            PluginLog.Error(
                "[ConflictingPlugins] Failed to check for setting conflicts: " +
                e.ToStringFull());
        }

        try
        {
            if (TryGetGameSettingConflicts(out var gameConflicts))
                conflicts[ConflictType.GameSetting] = gameConflicts;
        }
        catch (Exception e)
        {
            PluginLog.Error(
                "[ConflictingPlugins] Failed to check for game setting conflicts: " +
                e.ToStringFull());
        }

        _cachedConflicts = conflicts;
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

        Conflict[] currentConflicts;
        var hasComboConflicts = conflicts[ConflictType.Combo].Length > 0;
        var hasSettingsConflicts = conflicts[ConflictType.Settings].Length > 0;
        var hasGameConflicts = conflicts[ConflictType.GameSetting].Length > 0;
        var hasTargetingConflicts = conflicts[ConflictType.Targeting].Length > 0;

        ImGui.Spacing();
        ImGui.Spacing();

        if (hasComboConflicts)
        {
            currentConflicts = conflicts[ConflictType.Combo];
            var conflictingPluginsText = "- " + string.Join("\n- ",
                conflicts[ConflictType.Combo]
                    .Select(x => $"{x.Name} {x.Version}" +
                                 (string.IsNullOrEmpty(x.Reason)
                                     ? ""
                                     : $" ({x.Reason})")));
            var tooltipText =
                "The following plugins are known to conflict " +
                $"with {P.Name}:\n" +
                conflictingPluginsText +
                "\n\nIt is recommended you disable these plugins, or their " +
                "rotation\ncomponents, to prevent unexpected behavior and bugs.";

            ShowWarning(ConflictType.Combo, tooltipText, false);
        }

        if (hasSettingsConflicts)
        {
            currentConflicts = conflicts[ConflictType.Settings];
            var conflictingSettingsText = "- " + string.Join("\n- ",
                conflicts[ConflictType.Settings]
                    .Select(x => $"{x.Name} {x.Version} (setting: {x.Reason})"));

            var tooltipText =
                "The following plugins are known to conflict with\n" +
                $"{P.Name}'s Settings, which you have enabled:\n" +
                conflictingSettingsText +
                "\n\nIt is recommended you disable these plugins, or\n" +
                "remove the conflicting setting in the plugins\n" +
                "to prevent unexpected behavior and bugs.";

            ShowWarning(ConflictType.Settings, tooltipText,
                hasComboConflicts || hasTargetingConflicts);
        }

        if (hasGameConflicts)
        {
            currentConflicts = conflicts[ConflictType.GameSetting];

            var tooltipText =
                "The following game settings are known to conflict with " +
                $"{P.Name}'s Settings:";

            foreach (var conflict in conflicts[ConflictType.GameSetting])
            {
                var reasonSplit = conflict.Reason.Split("    ");
                tooltipText +=
                    $"\n- Setting: {reasonSplit[0]}" +
                    $"\n    Problem: {reasonSplit[1]}";
            }

            tooltipText +=
                "\n\nIt is recommended you change these settings, " +
                "to prevent unexpected behavior and bugs.";

            ShowWarning(ConflictType.GameSetting, tooltipText,
                hasComboConflicts || hasTargetingConflicts || hasSettingsConflicts);
        }

        if (hasTargetingConflicts)
        {
            currentConflicts = conflicts[ConflictType.Targeting];
            var tooltipText =
                "Your configuration in the following plugins will conflict\n" +
                $"with {P.Name}'s enabled Action Retargeting:";

            foreach (var conflict in conflicts[ConflictType.Targeting])
                tooltipText +=
                    $"\n- {conflict.Name} {conflict.Version}" +
                    $"\n    Actions Retargeted there and in {P.Name}:\n        - " +
                    string.Join("\n        - ", conflict.Reason.Split(','));

            tooltipText +=
                "\n\nIt is recommended you disable these plugins, or\n" +
                "remove the conflicting actions from their settings, or\n" +
                $"disable Retargeting for the action in {P.Name},\n" +
                "to prevent unexpected behavior and bugs.";

            ShowWarning(ConflictType.Targeting, tooltipText,
                hasComboConflicts || hasTargetingConflicts ||
                hasSettingsConflicts || hasGameConflicts);
        }

        return;

        void ShowWarning(ConflictType type, string tooltipText, bool hasWarningAbove)
        {
            var color = type switch
            {
                ConflictType.Combo => ImGuiColors.DalamudRed,
                ConflictType.Targeting => ImGuiColors.DalamudYellow,
                ConflictType.Settings => ImGuiColors.DalamudOrange,
                ConflictType.GameSetting => ImGuiColors.DalamudOrange,
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

        #region Redirect

        if (ConflictingPluginsChecks.Redirect.Conflicted)
        {
            var actions = ConflictingPluginsChecks.Redirect.ConflictingActions;
            var conflictMessage = actions
                .Where((action, i) => action is not (0 or 1) || i >= 2)
                .Aggregate("",
                    (current, action) => current + action.ActionName() + ",");
            conflictMessage = conflictMessage[..^1]; // remove last comma

            conflicts = conflicts.Append(new Conflict(
                    "Redirect", ConflictType.Targeting,
                    conflictMessage))
                .ToArray();
        }

        #endregion

        #region ReAction

        if (ConflictingPluginsChecks.ReAction.Conflicted &&
            ConflictingPluginsChecks.ReAction.ConflictingActions.Length > 4)
        {
            var actions = ConflictingPluginsChecks.ReAction.ConflictingActions;
            var conflictMessage = actions
                .Where((x, i) => x.Action is not (0 or 1 or 2) || i > 4)
                .Aggregate("",
                    (current, x) => current + $"{x.Action.ActionName()} " +
                                    $"    (stack: {x.stackName}),");
            conflictMessage = conflictMessage[..^1]; // remove last comma

            conflicts = conflicts.Append(new Conflict(
                    "ReAction", ConflictType.Targeting,
                    conflictMessage))
                .ToArray();
        }

        #endregion

        #region ReActionEx

        if (ConflictingPluginsChecks.ReActionEx.Conflicted &&
            ConflictingPluginsChecks.ReActionEx.ConflictingActions.Length > 4)
        {
            var actions = ConflictingPluginsChecks.ReActionEx.ConflictingActions;
            var conflictMessage = actions
                .Where((x, i) => x.Action is not (0 or 1 or 2) || i > 4)
                .Aggregate("",
                    (current, x) => current + $"{x.Action.ActionName()} " +
                                    $"    (stack: {x.stackName}),");
            conflictMessage = conflictMessage[..^1]; // remove last comma

            conflicts = conflicts.Append(new Conflict(
                    "ReActionEx", ConflictType.Targeting,
                    conflictMessage))
                .ToArray();
        }

        #endregion

        #region MOAction

        if (ConflictingPluginsChecks.MOAction.Conflicted)
            conflicts = conflicts.Append(new Conflict(
                    "MOAction", ConflictType.Targeting,
                    string.Join(",",
                        ConflictingPluginsChecks.MOAction.ConflictingActions
                            .Select(x => x.ActionName()))))
                .ToArray();

        #endregion

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

        #region BossMod

        if (ConflictingPluginsChecks.BossMod.TargetingSettingConflicted)
        {
            conflicts = conflicts.Append(new Conflict(
                    "BossMod", ConflictType.Settings,
                    "AI is enabled WITH targeting [check 'Disable auto-targeting']"))
                .ToArray();
        }

        if (ConflictingPluginsChecks.BossMod.QueueSettingConflicted)
        {
            conflicts = conflicts.Append(new Conflict(
                    "BossMod", ConflictType.Settings,
                    "Manual Queueing is Enabled [uncheck 'Use custom queueing']"))
                .ToArray();
        }

        #endregion

        #region BossModReborn

        if (ConflictingPluginsChecks.BossModReborn.TargetingSettingConflicted)
        {
            conflicts = conflicts.Append(new Conflict(
                    "BossModReborn", ConflictType.Settings,
                    "AI is enabled WITH targeting [check 'Manual targeting']"))
                .ToArray();
        }

        if (ConflictingPluginsChecks.BossModReborn.QueueSettingConflicted)
        {
            conflicts = conflicts.Append(new Conflict(
                    "BossModReborn", ConflictType.Settings,
                    "Manual Queueing is Enabled [uncheck 'Use custom queueing']"))
                .ToArray();
        }

        #endregion

        #region Redirect

        if (ConflictingPluginsChecks.Redirect.Conflicted)
        {
            if (ConflictingPluginsChecks.Redirect.ConflictingActions[0] is 1)
                conflicts = conflicts.Append(new Conflict(
                        "Redirect", ConflictType.Settings,
                        "Options > Treat all ground-targeted actions as mouseovers"))
                    .ToArray();
            if (ConflictingPluginsChecks.Redirect.ConflictingActions[1] is 1)
                conflicts = conflicts.Append(new Conflict(
                        "Redirect", ConflictType.Settings,
                        "Options > Treat all friendly actions as mouseovers"))
                    .ToArray();
        }
        if (ConflictingPluginsChecks.Redirect.BunnyConflict)
            conflicts = conflicts.Append(new Conflict(
                    "Redirect", ConflictType.Settings,
                    "Whole Plugin - Could be causing Bunnies [Reload or Disable Redirect]"))
                .ToArray();

        #endregion

        #region ReAction

        if (ConflictingPluginsChecks.ReAction.Conflicted)
        {
            var reFeedback = ConflictingPluginsChecks.ReAction.ConflictingActions;
            if (reFeedback[0].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReAction", ConflictType.Settings,
                        "Other Settings > Enable Auto Target"))
                    .ToArray();
            if (reFeedback[1].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReAction", ConflictType.Settings,
                        "Stacks > " + reFeedback[1].stackName + " > " +
                        "'All Actions' is retargeted"))
                    .ToArray();
            if (reFeedback[2].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReAction", ConflictType.Settings,
                        "Stacks > " + reFeedback[2].stackName + " > " +
                        "'All Harmful Actions' is retargeted"))
                    .ToArray();
            if (reFeedback[3].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReAction", ConflictType.Settings,
                        "Stacks > " + reFeedback[3].stackName + " > " +
                        "'All Beneficial Actions' is retargeted"))
                    .ToArray();
        }

        #endregion

        #region ReActionEx

        if (ConflictingPluginsChecks.ReActionEx.Conflicted)
        {
            var reFeedback = ConflictingPluginsChecks.ReActionEx.ConflictingActions;
            if (reFeedback[0].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReActionEx", ConflictType.Settings,
                        "Other Settings > Enable Auto Target"))
                    .ToArray();
            if (reFeedback[1].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReActionEx", ConflictType.Settings,
                        "Stacks > " + reFeedback[1].stackName + " > " +
                        "'All Actions' is retargeted"))
                    .ToArray();
            if (reFeedback[2].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReActionEx", ConflictType.Settings,
                        "Stacks > " + reFeedback[2].stackName + " > " +
                        "'All Harmful Actions' is retargeted"))
                    .ToArray();
            if (reFeedback[3].Action is 1)
                conflicts = conflicts.Append(new Conflict(
                        "ReActionEx", ConflictType.Settings,
                        "Stacks > " + reFeedback[3].stackName + " > " +
                        "'All Beneficial Actions' is retargeted"))
                    .ToArray();
        }

        #endregion

        return conflicts.Length > 0;
    }

    #endregion

    #region Game Setting Conflicts

    /// <summary>
    ///     Checks for conflicts from specific settings in the game,
    ///     like those that modify ground target placement.
    /// </summary>
    /// <param name="conflicts">
    ///     The output list of conflicting game settings.
    /// </param>
    /// <returns>
    ///     Whether there are game settings conflicts.
    /// </returns>
    private static bool TryGetGameSettingConflicts(out Conflict[] conflicts)
    {
        conflicts = [];

        if (ConflictingPluginsChecks.XIV.Conflicted)
        {

            if (ConflictingPluginsChecks.XIV.AutoFaceTargetConflicted)
                conflicts = conflicts.Append(new Conflict(
                        "XIV", ConflictType.GameSetting,
                        "Character Configuration > Control Settings > Target > " +
                        "Target Settings > Automatically face target when using action." +
                        "    " +
                        "You will have to manually face the target, " +
                        "outside of Auto Rotation, for actions to execute."))
                    .ToArray();

            if (ConflictingPluginsChecks.XIV.GroundTargetingPlacementConflicted)
                conflicts = conflicts.Append(new Conflict(
                        "XIV", ConflictType.GameSetting,
                        "Character Configuration > Control Settings > Target > " +
                        "Ground Targeting Settings > Press action twice to execute." +
                        "    " +
                        "Ground Actions cannot be Retargeted without additional click."))
                    .ToArray();
        }

        return conflicts.Length > 0;
    }

    #endregion

    #region Combo Conflicts

    /// <summary>
    ///     List of the most popular conflicting plugins.
    /// </summary>
    /// <remarks>
    ///     The list is not case-sensitive.
    /// </remarks>
    private static readonly string[] ConflictingPluginsNames =
    [
        "XIVCombo",
        "XIVComboExpanded",
        "XIVComboExpandedest",
        "XIVComboVX",
        "XIVSlothCombo",
        "RotationSolver",
    ];

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
                x.IsLoaded && // lighter check first
                ConflictingPluginsNames.Any(y => y.Equals(x.InternalName,
                    StringComparison.InvariantCultureIgnoreCase)))
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
                    "Autorotation module is queueing actions"))
                .ToArray();
        if (ConflictingPluginsChecks.BossModReborn.Conflicted)
        {
            conflicts = conflicts.Append(new Conflict(
                    "BossModReborn", ConflictType.Combo,
                    "Autorotation module is queueing actions"))
                .ToArray();
        }

        return conflicts.Length > 0;
    }

    #endregion
}