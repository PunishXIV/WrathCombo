#region

using Dalamud.Networking.Http;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using WrathCombo.API.Enum;
using WrathCombo.API.Extension;
using WrathCombo.Attributes;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Services.IPC;

public partial class Helper(ref Leasing leasing)
{
    private readonly Leasing _leasing = leasing;

    /// <summary>
    ///     Checks for typical bail conditions at the time of a set.
    /// </summary>
    /// <param name="result">
    ///     The result to set if the method should bail.
    /// </param>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <returns>If the method should bail.</returns>
    internal bool CheckForBailConditionsAtSetTime
        (out SetResult result, Guid? lease = null)
    {
        // Bail if IPC is disabled
        if (!IPCEnabled)
        {
            Logging.Warn(BailMessage.LiveDisabled.Description);
            result = SetResult.IPCDisabled;
            return true;
        }

        // Bail if the lease is not valid
        if (lease is not null &&
            !_leasing.CheckLeaseExists(lease.Value))
        {
            Logging.Warn(BailMessage.InvalidLease.Description);
            result = SetResult.InvalidLease;
            return true;
        }

        // Bail if the lease is blacklisted
        if (lease is not null &&
            _leasing.CheckBlacklist(lease.Value))
        {
            Logging.Warn(BailMessage.BlacklistedLease.Description);
            result = SetResult.BlacklistedLease;
            return true;
        }

        result = SetResult.IGNORED;
        return false;
    }

    /// <summary>
    ///     Gets the " " preset, as in Advanced if given Simple, and vice
    ///     versa.
    /// </summary>
    /// <param name="preset">The preset to search for the opposite of.</param>
    /// <returns>The Opposite-mode preset.</returns>
    internal static Preset? GetOppositeModeCombo(Preset preset)
    {
        var presetData = preset.Attributes();

        // Bail if it is not one of the main combos
        if (presetData.ComboType is not (ComboType.AdvancedDPS or ComboType.SimpleDPS or ComboType.SimpleHealing or ComboType.AdvancedHealing))
            return null;

        // Detect the target type
        var targetType = presetData.TargetType;

        // Bail if it is not a Single-Target or Multi-Target primary preset
        if (targetType == ComboTargetTypeKeys.Other)
            return null;

        // Detect the simplicity level
        var simplicityLevel =
            presetData.ComboType is ComboType.SimpleDPS or ComboType.SimpleHealing
                ? ComboSimplicityLevelKeys.Simple
                : ComboSimplicityLevelKeys.Advanced;
        // Flip the simplicity level
        var simplicityLevelToSearchFor =
            simplicityLevel == ComboSimplicityLevelKeys.Simple
                ? ComboSimplicityLevelKeys.Advanced
                : ComboSimplicityLevelKeys.Simple;

        try
        {
            // Get the opposite mode
            var categorizedPreset =
                P.IPCSearch.CurrentJobComboStatesCategorized
                        [presetData.JobInfo.Job]
                    [targetType][simplicityLevelToSearchFor];

            // Bail if there's no opposite-mode preset for this target type
            // (e.g. no Advanced healer combos for the same target type)
            if (categorizedPreset.Count == 0)
                return null;

            // Return the opposite mode, as a proper preset
            return categorizedPreset.Keys.First();
        }
        catch (Exception ex)
        {
            ex.LogWarning(
                "No opposite combo found, this is probably correct if this is a healer.");
            return null;
        }
    }

    #region Auto-Rotation Ready

    /// <summary>
    ///     Checks the current job to see whatever specified mode is enabled
    ///     (enabled and enabled in Auto-Mode).
    /// </summary>
    /// <param name="mode">
    ///     The <see cref="ComboTargetTypeKeys">Target Type</see> to check.
    /// </param>
    /// <param name="enabledStateToCheck">
    ///     The <see cref="ComboStateKeys">State</see> to check.
    /// </param>
    /// <param name="previousMatch">
    ///     The <see cref="ComboSimplicityLevelKeys">Simplicity Level</see> that
    ///     was used in the last set of calls of this method, to make sure that it
    ///     uses the same level for both checking if enabled and enabled in
    ///     Auto-Mode.<br />
    ///     Or <see langword="null" /> if it is the first call, so the level can be
    ///     set.
    /// </param>
    /// <returns>
    ///     Whether the current job has simple or advanced combo enabled
    ///     (however specified) for the target type specified.
    /// </returns>
    /// <seealso cref="Provider.IsCurrentJobConfiguredOn" />
    /// <seealso cref="Provider.IsCurrentJobAutoModeOn" />
    internal ComboSimplicityLevelKeys? CheckCurrentJobModeIsEnabled
    (ComboTargetTypeKeys mode,
        ComboStateKeys enabledStateToCheck,
        ComboSimplicityLevelKeys? previousMatch = null)
    {
        if (CustomComboFunctions.LocalPlayer is null)
            return null;

        // Convert current job/class to a job, if it is a class
        var job = Player.Job.GetUpgradedJob();

        // Get the user's settings for this job
        P.IPCSearch.CurrentJobComboStatesCategorized.TryGetValue(job,
            out var comboStates);

        // Bail if there are no combos found for this job
        if (comboStates is null || comboStates.Count == 0)
            return null;

        // Try to get the Simple Mode settings
        comboStates[mode]
            .TryGetValue(ComboSimplicityLevelKeys.Simple, out var simpleResults);
        var simpleHigher = simpleResults?.FirstOrDefault();
        var simple = simpleHigher?.Value;

        #region Override the Values with any IPC-control

        var simpleComboPreset = simpleHigher?.Key;
        if (simpleComboPreset is not null)
        {
            simple[ComboStateKeys.AutoMode] =
                P.IPCSearch.AutoActions[simpleComboPreset.Value];
            simple[ComboStateKeys.Enabled] =
                P.IPCSearch.EnabledActions.Contains(simpleComboPreset.Value);
        }

        #endregion

        // Get the Advanced Mode settings
        var (advancedComboPreset, advancedValue) =
            comboStates[mode][ComboSimplicityLevelKeys.Advanced].First();

        #region Override the Values with any IPC-control

        advancedValue[ComboStateKeys.AutoMode] =
            P.IPCSearch.AutoActions[advancedComboPreset];
        advancedValue[ComboStateKeys.Enabled] =
            P.IPCSearch.EnabledActions.Contains(advancedComboPreset);

        #endregion

        // If the simplicity level is set, check that specifically instead of either
        if (previousMatch is not null)
        {
            if (previousMatch == ComboSimplicityLevelKeys.Simple &&
                simple is not null && simple[enabledStateToCheck])
                return ComboSimplicityLevelKeys.Simple;
            return advancedValue[enabledStateToCheck]
                ? ComboSimplicityLevelKeys.Advanced
                : null;
        }

        // Check for either Simple or Advanced being ready
        return simple is not null && simple[enabledStateToCheck] ?
            ComboSimplicityLevelKeys.Simple :
            advancedValue[enabledStateToCheck] ? ComboSimplicityLevelKeys.Advanced :
                null;
    }

    /// <summary>
    ///     Cache of the combos to set the current job to be Auto-Rotation ready.
    /// </summary>
    private static readonly Dictionary<Job, List<string>>
        CombosForARCache = new();

    /// <summary>
    ///     Gets the combos to set the current job to be Auto-Rotation ready.
    /// </summary>
    /// <param name="job">The job to get the combos for.</param>
    /// <param name="includeOptions">
    ///     Whether to include the options for the combos.
    /// </param>
    /// <returns>
    ///     A list of combo names to set the current job to be Auto-Rotation ready.
    /// </returns>
    /// <seealso cref="Provider.SetCurrentJobAutoRotationReady" />
    internal static List<string>? GetCombosToSetJobAutoRotationReady
        (Job job, bool includeOptions = true)
    {
        if (CombosForARCache.TryGetValue(job, out var value))
            return value;

        if (!P.IPCSearch.CurrentJobComboStatesCategorized.TryGetValue(job, out var comboStates))
            return null;

        List<string> combos = [];

        // Add combos for each target type category
        AddComboForTargetType(combos, comboStates, job, ComboTargetTypeKeys.SingleTargetDPS, includeOptions);
        AddComboForTargetType(combos, comboStates, job, ComboTargetTypeKeys.AoEDPS, includeOptions);
        if (job.IsHealer() || job is Job.BLU)
        {
            AddComboForTargetType(combos, comboStates, job, ComboTargetTypeKeys.SingleTargetHeals, includeOptions);
            AddComboForTargetType(combos, comboStates, job, ComboTargetTypeKeys.AoEHeals, includeOptions);
        }

        if (includeOptions)
            CombosForARCache[job] = combos;
        return combos;
    }

    /// <summary>
    ///     Adds the appropriate combo for a specific target type to the list.
    ///     Prioritizes simple combos, falls back to advanced if no simple combo exists.
    /// </summary>
    private static void AddComboForTargetType(
        List<string> combos,
        Dictionary<ComboTargetTypeKeys, Dictionary<ComboSimplicityLevelKeys, Dictionary<Preset, Dictionary<ComboStateKeys, bool>>>> comboStates,
        Job job,
        ComboTargetTypeKeys targetType,
        bool includeOptions)
    {
        if (!comboStates.TryGetValue(targetType, out var bySimplicity))
            return;

        // Get simple combo if available
        if (bySimplicity.TryGetValue(ComboSimplicityLevelKeys.Simple, out var simpleCombo) && simpleCombo.Count > 0)
        {
            if (job is Job.BLU)
                combos.AddRange(simpleCombo.Keys.Select(k => k.ToString()));
            else
                combos.Add(simpleCombo.First().Key.ToString());
            return;
        }

        // Fall back to advanced combo
        if (!bySimplicity.TryGetValue(ComboSimplicityLevelKeys.Advanced, out var advancedCombo) ||
            advancedCombo.Count == 0)
            return;

        if (job is Job.BLU)
        {
            foreach (var preset in advancedCombo.Keys)
            {
                var bluComboName = preset.ToString();
                combos.Add(bluComboName);
                if (includeOptions &&
                    P.IPCSearch.OptionNamesByJob.TryGetValue(job, out var bluOptions) &&
                    bluOptions.TryGetValue(bluComboName, out var bluComboOptions))
                    combos.AddRange(bluComboOptions);
            }
            return;
        }

        var comboName = advancedCombo.First().Key.ToString();
        combos.Add(comboName);

        // Add related options if requested
        if (includeOptions && P.IPCSearch.OptionNamesByJob.TryGetValue(job, out var jobOptions) &&
            jobOptions.TryGetValue(comboName, out var options))
        {
            combos.AddRange(options);
        }
    }

    #endregion

    #region IPC Callback

    /// <summary>
    ///     Calls the leasee's <c>{prefix}.WrathComboCallback</c> IPC method.
    /// </summary>
    /// <param name="prefix">The leasee's IPC prefix for the callback.</param>
    /// <param name="reason">The cancellation reason, passed as an int.</param>
    /// <param name="additionalInfo">Any additional info about the cancellation.</param>
    /// <remarks>
    ///     Subscribes per call, so the callback always reaches the gate of the
    ///     leasee actually being cancelled — independent of any other leasees.
    /// </remarks>
    internal static void CallIPCCallback(string prefix, CancellationReason reason,
        string additionalInfo = "")
    {
        try
        {
            Svc.PluginInterface
                .GetIpcSubscriber<int, string, object>(
                    $"{prefix}.WrathComboCallback")
                .InvokeAction((int)reason, additionalInfo);
        }
        catch
        {
            Logging.Error("Failed to call IPC callback with IPC prefix: " + prefix);
        }
    }

    #endregion

    #region Checking the repo for live IPC status

    /// Dalamud's happy eyeballs handler, which handles IPv6, among other things.
    // ReSharper disable once InconsistentNaming
    private static readonly SocketsHttpHandler _httpHandler = new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        ConnectCallback = new HappyEyeballsCallback().ConnectCallback,
    };

    /// The HTTP client, setup with a short timeout and Dalamud's happy handler.
    private readonly HttpClient _httpClient = new(_httpHandler)
        { Timeout = TS.FromSeconds(5) };

    /// <summary>
    ///     The endpoint for checking the IPC status straight from the repo,
    ///     so it can be disabled without a plugin update if for some reason
    ///     necessary.
    /// </summary>
    private const string IPCStatusEndpoint =
        "https://raw.githubusercontent.com/PunishXIV/WrathCombo/main/res/ipc_status.txt";

    /// <summary>
    ///     The cached backing field for the IPC status.
    /// </summary>
    /// <seealso cref="IPCEnabled" />
    private bool? _ipcEnabled;

    /// <summary>
    ///     The lightly-cached live IPC status.<br />
    ///     Backed by <see cref="_ipcEnabled" />.
    /// </summary>
    /// <seealso cref="IPCStatusEndpoint" />
    /// <seealso cref="_ipcEnabled" />
    public bool IPCEnabled
    {
        get
        {
            // If the IPC status was checked within the last 45 minutes:
            // return the cached value
            if (_ipcEnabled is not null &&
                !EZ.Throttle("ipcLastStatusChecked", TS.FromMinutes(45)))
                return _ipcEnabled!.Value;

            // Otherwise, check the status and cache the result
            string data;
            // Check the status
            try
            {
                using var ipcStatusQuery =
                    _httpClient.GetAsync(IPCStatusEndpoint).Result;
                ipcStatusQuery.EnsureSuccessStatusCode();
                data = ipcStatusQuery.Content.ReadAsStringAsync()
                    .Result.Trim().ToLower();
            }
            catch (Exception e)
            {
                data = "enabled";
                Logging.Error(
                    "Failed to check IPC status. Assuming it is enabled.\n" +
                    e.Message
                );
            }

            // Read the status
            var ipcStatus = data.StartsWith("enabled");
            // Cache the status
            _ipcEnabled = ipcStatus;

            // Handle suspended status
            if (!ipcStatus)
                _leasing.SuspendLeases();

            return ipcStatus;
        }
    }

    #endregion
}

/// <summary>
///     Simple Wrapper for logging IPC events, to help keep things consistent.
/// </summary>
internal static class Logging
{
    private const string Prefix = "[Wrath IPC] ";

    private static StackTrace StackTrace => new();

    private static string PrefixMethod
    {
        get
        {
            for (var i = 3; i >= 0; i--)
            {
                try
                {
                    var frame = StackTrace.GetFrame(i);
                    var method = frame.GetMethod();
                    var className = method.DeclaringType.Name;
                    var methodName = method.Name;
                    return $"[{className}.{methodName}] ";
                }
                catch
                {
                    // Continue to the next index
                }
            }

            return "[Unknown Method] ";
        }
    }

    public static void Verbose(string message) =>
        PluginLog.Verbose(Prefix + PrefixMethod + message);

    public static void Log(string message) =>
        PluginLog.Debug(Prefix + PrefixMethod + message);

    public static void Warn(string message) =>
        PluginLog.Warning(Prefix + PrefixMethod + message
#if DEBUG
                          + "\n" + (StackTrace)
#endif
        );

    public static void Error(string message) =>
        PluginLog.Error(Prefix + PrefixMethod + message + "\n" + (StackTrace));
}

