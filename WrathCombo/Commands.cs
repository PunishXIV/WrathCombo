#region

using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ExcelServices;
using ECommons.Logging;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.AutoRotation;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window;
using WrathCombo.Window.Tabs;
using static ECommons.ExcelServices.ExcelJobHelper;

#endregion

namespace WrathCombo;

public partial class WrathCombo
{
    private const string Command = "/wrath";
    private const string OldCommand = "/scombo";

    /// <summary>
    ///     Registers the base commands for the plugin.<br />
    ///     Also displays the biggest commands in Dalamud.
    /// </summary>
    private void RegisterCommands()
    {
        EzCmd.Add(Command, OnCommand,
            "Open a window to edit custom combo settings.\n" +
            $"{Command} auto → Toggle Auto-rotation on/off.\n" +
            $"{Command} debug → Dumps a debug log onto your desktop for developers.\n" +
            $"{OldCommand} → Old alias from XIVSlothCombo, still works!");
        EzCmd.Add(OldCommand, OnCommand);
    }

    /// <summary>
    ///     Handles the command input, and calls the appropriate method.
    /// </summary>
    /// <param name="command">
    ///     Irrelevant, as we handle all commands the same.<br />
    ///     Required for the command handler.
    /// </param>
    /// <param name="arguments">
    ///     The arguments provided with the command.<br />
    ///     Generally treated as:<br />
    ///     The first argument is the command to execute, and the second is the
    ///     argument for the command.<br />
    ///     If the command is not recognized, the
    ///     <see cref="HandleOpenCommand">Open Command</see> is assumed, to handle
    ///     opening to a specific job.
    /// </param>
    private void OnCommand(string command, string arguments)
    {
        var argumentParts = arguments.ToLowerInvariant().Split();
        switch (argumentParts[0])
        {
            case "unsetall":
            case "set":
            case "toggle":
            case "unset":
                HandleSetCommands(argumentParts); break;

            case "list":
            case "enabled":
            case "disabled": // unlisted
                HandleListCommands(argumentParts); break;

            case "combo":
                HandleComboCommands(argumentParts); break;

            case "auto":
                HandleAutoCommands(argumentParts); break;

            case "ignore":
                HandleIgnoreCommand(); break;

            case "debug":
                HandleDebugCommands(argumentParts); break;

            case "settings":
            case "config": // unlisted
                HandleOpenCommand(tab: OpenWindow.Settings, forceOpen: true); break;

            case "autosettings":
            case "autorotationsettings": // unlisted
            case "autoconfig": // unlisted
            case "autorotationconfig": // unlisted
                HandleOpenCommand(tab: OpenWindow.AutoRotation, forceOpen: true);
                break;

            case "pve":
                HandleOpenCommand(tab: OpenWindow.PvE, forceOpen: true); break;

            case "pvp":
                HandleOpenCommand(tab: OpenWindow.PvP, forceOpen: true); break;

            case "dbg": // unlisted
            case "debugtab": // unlisted
                HandleOpenCommand(tab: OpenWindow.Debug, forceOpen: true); break;
            
            // "IPromiseIWillDoMyJobQuestsLater" will be accepted
            // ReSharper disable once StringLiteralTypo
            case "ipromiseiwilldomyjobquestslater": // unlisted
                HandleJobStoneCheckCommand(); break;

            default:
                HandleOpenCommand(argumentParts); break;
        }

        Service.Configuration.Save();
    }

    /// <summary>
    ///     Handles the set command, which toggles, sets, or unsets presets.
    /// </summary>
    /// <value>
    ///     <c>&lt;blank&gt;</c> - list valid arguments<br />
    ///     <c>toggle</c> - toggle preset, requires another argument<br />
    ///     <c>set</c> - enable preset, requires another argument<br />
    ///     <c>unset</c> - disable preset, requires another argument<br />
    ///     <c>unsetall</c> - disable all presets
    /// </value>
    /// <param name="argument">
    ///     The action to take on the preset, then (if not "unset"), the preset to
    ///     act on. The preset can be provided as the internal name or the ID.
    /// </param>
    /// <remarks>
    ///     Will not allow the command to be used in combat.
    /// </remarks>
    private void HandleSetCommands(string[] argument)
    {
        #region Variable Setup

        const string toggle = "toggle";
        const string set = "set";
        const string unset = "unset";

        const string all = "all";

        string? action;
        string? target = null;

        Preset? preset = null;

        #endregion

        /*
        #if !DEBUG
        if (Player.Available && CustomComboFunctions.InCombat())
        {
            DuoLog.Error("Cannot use this command in combat");
            return;
        }
        #endif
        */

        // Parse the action
        switch (argument[0])
        {
            case "unsetall":
                action = unset;
                target = all;
                break;

            case "set":
                action = set;
                break;

            case "toggle":
                action = toggle;
                break;

            case "unset":
                action = unset;
                break;

            default:
                DuoLog.Error("Available set actions: toggle, set, unset, unsetall");
                return;
        }

        if (target is null && argument.Length < 2)
        {
            DuoLog.Error($"Please specify a feature to {action}");
            return;
        }

        // Parse the target feature
        target ??= argument[1];
        if (target != all)
        {
            var presetCanNumber = int.TryParse(target, out var targetNumber);
            try
            {
                preset = presetCanNumber
                    ? (Preset)targetNumber
                    : Enum.Parse<Preset>(target, true);
            }
            catch
            {
                DuoLog.Error($"Could not find preset '{target}'");
                return;
            }
        }

        // Give the correct method for the action
        Func<Preset, bool, bool> method = action switch
        {
            toggle => PresetStorage.TogglePreset,
            set => PresetStorage.EnablePreset,
            unset => PresetStorage.DisablePreset,
            _ => throw new ArgumentOutOfRangeException(nameof(argument), action,
                null),
        };

        // Execute the method
        if (target == all)
        {
            Service.Configuration.EnabledActions.Clear();
            DuoLog.Information("All unset");
        }
        else
        {
            var usablePreset = (Preset)preset!;
            method(usablePreset, false);

            if (action == toggle)
                action =
                    Service.Configuration.EnabledActions
                        .TryGetValue(usablePreset, out _)
                        ? set
                        : unset;

            var ctrlText = P.UIHelper.PresetControlled(usablePreset) is not null
                ? " " + OptionControlledByIPC
                : "";

            DuoLog.Information(
                $"{usablePreset.Attributes().CustomComboInfo.Name} {action} {ctrlText}");
        }
    }

    /// <summary>
    ///     Handles the list command, which lists all available presets.
    /// </summary>
    /// <value>
    ///     <c>&lt;blank&gt;</c> - list valid arguments<br />
    ///     <c>set</c> - enabled presets<br />
    ///     <c>enabled</c> - enabled presets<br />
    ///     <c>unset</c> - disabled presets<br />
    ///     <c>disabled</c> - disabled presets (unlisted command)<br />
    ///     <c>all</c> - all presets
    /// </value>
    /// <param name="argument">
    ///     The filter to apply to the list.<br />
    ///     If no argument is provided, all presets are listed.
    /// </param>
    private void HandleListCommands(string[] argument)
    {
        IEnumerable<Preset> presets = Enum.GetValues<Preset>()
            .Where(x => x.Attributes().Hidden is null);
        const StringComparison lower = StringComparison.InvariantCultureIgnoreCase;
        var filter =
            argument.Length > 1 && argument[0].Trim().Equals("list", lower)
                ? argument[1]
                : argument[0];
        var job = argument.Length > 2
            ? argument[2]
            : argument.Length > 1 && !argument[0].Trim().Equals("list", lower)
                ? argument[1]
                : null;
        PluginLog.Debug($"Filter: {filter}, " +
                        $"Job: {job}");

        switch (filter)
        {
            case "enabled":
            case "set":
                presets = presets.Where(preset =>
                    IPC.GetComboState(preset.ToString())?.First().Value ?? false
                );

                presets = FilterPresetsToJob(presets, job);
                if (!presets.Any())
                    return;

                foreach (var preset in presets)
                {
                    var controlled =
                        P.UIHelper.PresetControlled(preset) is not null;
                    var ctrlText = controlled ? " " + OptionControlledByIPC : "";
                    DuoLog.Information($"{(int)preset} - {preset}{ctrlText}");
                }

                break;

            case "disabled":
            case "unset":
                presets = presets.Where(preset =>
                    !IPC.GetComboState(preset.ToString())!.First().Value);

                presets = FilterPresetsToJob(presets, job);
                if (!presets.Any())
                    return;

                foreach (var preset in presets)
                {
                    var controlled =
                        P.UIHelper.PresetControlled(preset) is not null;
                    var ctrlText = controlled ? " " + OptionControlledByIPC : "";
                    DuoLog.Information($"{(int)preset} - {preset}{ctrlText}");
                }

                break;

            case "all":
                presets = FilterPresetsToJob(presets, job);
                if (!presets.Any())
                    return;

                foreach (var preset in presets)
                {
                    var controlled =
                        P.UIHelper.PresetControlled(preset) is not null;
                    var ctrlText = controlled ? " " + OptionControlledByIPC : "";
                    DuoLog.Information($"{(int)preset} - {preset}{ctrlText}");
                }

                break;

            default:
                DuoLog.Error("Available list filters: set, unset, all");
                break;
        }

        return;

        Preset[] FilterPresetsToJob
            (IEnumerable<Preset> presetsList, string? jobShort)
        {
            if (jobShort is not null)
            {
                presetsList = presetsList.Where(preset =>
                    preset.Attributes().CustomComboInfo.JobShorthand
                        .Equals(jobShort, lower));
            }

            presetsList = presetsList.ToArray();
            if (presetsList.Any()) return presetsList.ToArray();

            if (jobShort is not null)
                DuoLog.Error($"{jobShort} is not a correct job abbreviation," +
                             $" or has nothing to list.");
            else
                DuoLog.Error($"Nothing is disabled.");
            return [];

        }
    }

    /// <summary>
    ///     Handles the combo command, the replacing of actions.
    /// </summary>
    /// <value>
    ///     <c>&lt;blank&gt;</c> - toggle<br />
    ///     <c>on</c> - enable<br />
    ///     <c>off</c> - disable<br />
    ///     <c>toggle</c> - toggle
    /// </value>
    /// <param name="argument">
    ///     The way to change the combo setting.<br />
    ///     If no argument is provided, the setting is toggled.
    /// </param>
    private void HandleComboCommands(string[] argument)
    {
        if (argument.Length < 2)
        {
            Service.Configuration.SetActionChanging(
                !Service.Configuration.ActionChanging);
            DuoLog.Information(
                "Action Replacing set to "
                + (Service.Configuration.ActionChanging ? "ON" : "OFF"));
            return;
        }

        switch (argument[1])
        {
            case "on":
                Service.Configuration.SetActionChanging(true);
                break;

            case "off":
                Service.Configuration.SetActionChanging(false);
                break;

            case "toggle":
                Service.Configuration.SetActionChanging(
                    !Service.Configuration.ActionChanging);
                break;

            default:
                DuoLog.Error("Available combo options: on, off, toggle");
                return;
        }

        DuoLog.Information(
            "Action Replacing set to "
            + (Service.Configuration.ActionChanging ? "ON" : "OFF"));
    }

    /// <summary>
    ///     Handles the auto command, which calls <see cref="ToggleAutoRotation" />.
    /// </summary>
    /// <value>
    ///     <c>target</c> - modify targeting mode<br />
    ///     <c>&lt;blank&gt;</c> - toggle<br />
    ///     <c>on</c> - enable<br />
    ///     <c>off</c> - disable<br />
    ///     <c>toggle</c> - toggle
    /// </value>
    /// <param name="argument">
    ///     The way to change the auto-rotation setting.<br />
    ///     If no argument is provided, the setting is toggled.
    /// </param>
    private void HandleAutoCommands(string[] argument)
    {
        // ADD: Handle targeting mode changes
        if (argument.Length >= 4 && argument[1] == "target")
        {
            var role = argument[2].ToLowerInvariant();
            var mode = argument[3];
            
            switch (role)
            {
                case "damage" when Enum.TryParse<DPSRotationMode>(mode, true, out var dpsMode):
                {
                    Service.Configuration.RotationConfig.DPSRotationMode = dpsMode;
                    Service.Configuration.Save();
                
                    var dpsControlled = P.UIHelper.AutoRotationConfigControlled("DPSRotationMode") is not null;
                    var ctrlText = dpsControlled ? " " + OptionControlledByIPC : "";
                
                    DuoLog.Information($"Damage targeting mode set to: {dpsMode.ToString().Replace('_', ' ')}{ctrlText}");
                    return;
                }
                case "healer" when Enum.TryParse<HealerRotationMode>(mode, true, out var healerMode):
                {
                    Service.Configuration.RotationConfig.HealerRotationMode = healerMode;
                    Service.Configuration.Save();
                
                    var healerControlled = P.UIHelper.AutoRotationConfigControlled("HealerRotationMode") is not null;
                    var ctrlText = healerControlled ? " " + OptionControlledByIPC : "";
                
                    DuoLog.Information($"Healer targeting mode set to: {healerMode.ToString().Replace('_', ' ')}{ctrlText}");
                    return;
                }
                default:
                    DuoLog.Error("Usage: /wrath auto target <damage|healer> <mode>");
                    return;
            }
        }
        
        // Handle Normal Toggling of Auto-Rotation
        var toggledVal = !Service.Configuration.RotationConfig.Enabled;
        var newVal = argument.Length > 1
            ? argument[1] == "toggle"
                ? toggledVal
                : argument[1] == "on"
            : toggledVal;

        if (newVal != Service.Configuration.RotationConfig.Enabled)
            ToggleAutoRotation(newVal);
    }

    /// <summary>
    ///     Toggles the auto-rotation setting.
    /// </summary>
    /// <param name="value">
    ///     Whether to enable or disable auto-rotation.
    /// </param>
    private static void ToggleAutoRotation(bool value)
    {
        Service.Configuration.RotationConfig.Enabled = value;
        Service.Configuration.Save();

        var stateControlled =
            P.UIHelper.AutoRotationStateControlled() is not null;

        DuoLog.Information(
            "Auto-Rotation set to "
            + (Service.Configuration.RotationConfig.Enabled ? "ON" : "OFF")
            + (stateControlled ? " " + OptionControlledByIPC : "")
        );
    }

    /// <summary>
    ///     Handles the ignore command.
    /// </summary>
    /// <value>
    ///     <c>&lt;blank&gt;</c> - add target<br />
    /// </value>
    /// <remarks>
    ///     Requires a target to be selected, and the target to be hostile.
    /// </remarks>
    private void HandleIgnoreCommand()
    {
        var target = Svc.Targets.Target;

        if (target == null)
        {
            DuoLog.Error("No target selected");
            return;
        }

        if (!target.IsHostile())
        {
            DuoLog.Error("No valid target selected");
            return;
        }

        if (Service.Configuration.IgnoredNPCs.Any(x => x.Key == target.DataId))
        {
            DuoLog.Error(
                $"{target.Name} (ID: {target.DataId}) is already on the ignored list");
            return;
        }

        if (Service.Configuration.IgnoredNPCs.All(x => x.Key != target.DataId))
        {
            Service.Configuration.IgnoredNPCs.Add(target.DataId, target.GetNameId());

            DuoLog.Information(
                $"Successfully added {target.Name} (ID: {target.DataId}) to ignored list");
        }
    }

    /// <summary>
    ///     Handles the debug command, which calls
    ///     <see cref="DebugFile.MakeDebugFile" />.
    /// </summary>
    /// <value>
    ///     <c>&lt;blank&gt;</c> - current job<br />
    ///     <c>&lt;job abbr&gt;</c> - that job<br />
    ///     <c>all</c> - all jobs<br />
    /// </value>
    /// <param name="argument">
    ///     The job abbreviation to provide the debug file for (or "all").<br />
    ///     If no argument is provided, the current job is used.
    /// </param>
    private void HandleDebugCommands(string[] argument)
    {
        try
        {
            ClassJob? job = null;

            // Handle an entered job abbreviation
            if (argument.Length > 1)
            {
                if (argument[1] == "path")
                {
                    DuoLog.Information(
                        $"WrathDebug.txt should have been created at:\n" +
                        $"{DebugFile.GetDebugFilePath()}");
                    return;
                }

                if (argument[1] == "string")
                {
                    GenericHelpers.Copy(
                        "```\n" + DebugFile.GetDebugCode() + "\n```");
                    DuoLog.Information(
                        "Debug string copied to clipboard. Paste this where requested.");
                    return;
                }

                if (argument[1].Length != 3)
                {
                    DuoLog.Error("Invalid job abbreviation");
                    throw new ArgumentException("Invalid job abbreviation");
                }

                if (argument[1] == "all")
                {
                    DebugFile.MakeDebugFile(allJobs: true);
                    return;
                }

                var jobAbbr = argument[1].ToUpperInvariant();
                try
                {
                    // Look up the entered job
                    if (TryGetJobByAbbreviation(jobAbbr, out ClassJob jobSearch))
                    {
                        //ClassJob -> enum,
                        //Check if Class and change to Job
                        //Retrieve final ClassJob
                        job = jobSearch.GetJob().GetUpgradedJob().GetData();

                        if (job.Value.RowId != Player.JobId)
                            DuoLog.Warning($"You are not on {job.Value.Name()}");
                    }
                }
                // the .first() failed
                catch (InvalidOperationException)
                {
                    DuoLog.Error($"Invalid job abbreviation, '{jobAbbr}'");
                    throw;
                }
                // unknown
                catch (Exception ex)
                {
                    DuoLog.Error($"Error looking up job abbreviation, '{jobAbbr}'");
                    Svc.Log.Error(ex, "Debug Log");
                    throw;
                }
            }

            // Request a debug file, with null, or the entered Job
            // (if converted successfully)
            DebugFile.MakeDebugFile(job);
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Debug Log");
            DuoLog.Error("Unable to write Debug log");
        }
    }

    /// <summary>
    ///     Handles the opening of the window, as well as the opening command.
    /// </summary>
    /// <value>
    ///     <c>&lt;blank&gt;</c> - toggle window<br />
    ///     <c>&lt;job abbr&gt;</c> - open window, to that job
    /// </value>
    /// <param name="argument">
    ///     Only should be provided if coming from
    ///     <see cref="OnCommand">OnCommand</see>.<br />
    ///     Job Abbreviation to open to (the PvE tab for).
    /// </param>
    /// <param name="tab">
    ///     Only should be provided if coming from <see cref="OnOpenMainUi" /> or
    ///     <see cref="OnOpenConfigUi" />, or the tab commands in
    ///     <see cref="OnCommand">OnCommand</see>.<br />
    ///     The tab of the UI window to open to.
    /// </param>
    /// <param name="forceOpen">
    ///     Only should be provided if coming from <see cref="OnOpenMainUi" /> or
    ///     <see cref="OnOpenConfigUi" />, or the tab commands in
    ///     <see cref="OnCommand">OnCommand</see>.<br />
    ///     If provided: the state the window should be forced to.
    /// </param>
    /// <remarks>
    ///     The order of operations is as follows:
    ///     <list type="number">
    ///         <item>Toggle the window state</item>
    ///         <item>
    ///             Force window state (UI buttons)
    ///             (if <paramref name="forceOpen" />)
    ///         </item>
    ///         <item>
    ///             Open to specific tab
    ///             (if <paramref name="tab" />)
    ///             (returns early)
    ///         </item>
    ///         <item>
    ///             Open to current job setting
    ///             (if <see cref="PluginConfiguration.OpenToCurrentJob" />)
    ///         </item>
    ///         <item>
    ///             Open to specified job
    ///             (if specified in <paramref name="argument" />, from
    ///             <see cref="OnCommand">OnCommand</see>)
    ///         </item>
    ///     </list>
    /// </remarks>
    private void HandleOpenCommand
        (string[]? argument = null, OpenWindow? tab = null, bool? forceOpen = null)
    {
        argument ??= [""];

        // Toggle the window state
        ConfigWindow.IsOpen = !ConfigWindow.IsOpen;

        // Force open (UI buttons)
        if (forceOpen is not null)
            ConfigWindow.IsOpen = forceOpen.Value;

        // Handle option to always open to the PvE tab
        var openingToPvP =
            ContentCheck.IsInPVPContent && Service.Configuration.OpenToPvP;
        if (ConfigWindow.IsOpen)
            if (openingToPvP)
                ConfigWindow.OpenWindow = OpenWindow.PvP;
            else if (Service.Configuration.OpenToPvE)
                ConfigWindow.OpenWindow = OpenWindow.PvE;

        // Open to specific tab
        if (tab is not null)
        {
            ConfigWindow.OpenWindow = tab.Value;
            return;
        }

        // If no arguments provided
        if (argument[0].Length <= 0)
        {
            // Handle the "Open to current job" setting
            if (ConfigWindow.IsOpen && !openingToPvP)
                PvEFeatures.OpenToCurrentJob(false);

            // Skip trying to process arguments
            return;
        }
        
        // Open to specified job
        var jobAbbrev = argument[0];

        if (TryGetJobByAbbreviation(jobAbbrev, out var job))
        {
            ConfigWindow.IsOpen = true;
            ConfigWindow.OpenWindow = OpenWindow.PvE;
            PvEFeatures.OpenJob = job.GetJob();
        } 
        else
        {
            DuoLog.Error($"{argument[0]} is not a correct job abbreviation.");
            return;
        }

        
    }

    /// <summary>
    ///     Disables job stone checking for the session.<br />
    ///     This is used to allow the user to play classes without being
    ///     prompted to use a job stone.<br />
    ///     Disables <see cref="ActionReplacer.ClassLocked" />.
    /// </summary>
    private void HandleJobStoneCheckCommand()
    {
        if (ActionReplacer.DisableJobCheck)
        {
            DuoLog.Information("Job Stone Checking is already disabled.");
            return;
        }
        
        ActionReplacer.DisableJobCheck = true;
        DuoLog.Information("Job Stone Checking has been disabled for this session.");
        DuoLog.Warning("Please do not play Classes with other people, " +
                       "it is objectively worse in every way, and you will lack " +
                       "a significant amount of functionality anyway.");
    }
}
