#region

using Dalamud.Networking.Http;
using ECommons;
using ECommons.ExcelServices;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using ECommons.Logging;
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
    ///     Gets the "opposite" preset, as in Advanced if given Simple, and vice
    ///     versa.
    /// </summary>
    /// <param name="preset">The preset to search for the opposite of.</param>
    /// <returns>The Opposite-mode preset.</returns>
    internal static Preset? GetOppositeModeCombo(Preset preset)
    {
        const StringComparison lower = StringComparison.CurrentCultureIgnoreCase;
        var attr = preset.Attributes();

        // Bail if it is not one of the main combos
        if (attr.ComboType is not (ComboType.Advanced or ComboType.Simple))
            return null;

        // Detect the target type
        var targetType =
            attr.CustomComboInfo.Name.Contains("single target", lower)
                ? ComboTargetTypeKeys.SingleTarget
                : (attr.CustomComboInfo.Name.Contains("- aoe", lower))
                    ? ComboTargetTypeKeys.MultiTarget
                    : ComboTargetTypeKeys.Other;

        // Bail if it is not a Single-Target or Multi-Target primary preset
        if (targetType == ComboTargetTypeKeys.Other)
            return null;

        // Detect the simplicity level
        var simplicityLevel =
            attr.ComboType is ComboType.Simple
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
                        [attr.CustomComboInfo.Job]
                    [targetType][simplicityLevelToSearchFor];

            // Return the opposite mode, as a proper preset
            var oppositeMode = categorizedPreset.FirstOrDefault().Key;
            var oppositeModePreset = (Preset)
                Enum.Parse(typeof(Preset), oppositeMode, true);
            return oppositeModePreset;
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

        Preset? simpleComboPreset = simpleHigher is null
            ? null
            : (Preset)
            Enum.Parse(typeof(Preset), simpleHigher.Value.Key, true);
        if (simpleComboPreset is not null)
        {
            simple[ComboStateKeys.AutoMode] =
                P.IPCSearch.AutoActions[(Preset)simpleComboPreset];
            simple[ComboStateKeys.Enabled] =
                P.IPCSearch.EnabledActions.Contains(
                    (Preset)simpleComboPreset);
        }

        #endregion

        // Get the Advanced Mode settings
        var (advancedKey, advancedValue) =
            comboStates[mode][ComboSimplicityLevelKeys.Advanced].First();

        #region Override the Values with any IPC-control

        var advancedComboPreset = (Preset)
            Enum.Parse(typeof(Preset), advancedKey, true);
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
        #region Getting Combo data

        if (CombosForARCache.TryGetValue(job, out var value))
            return value;

        P.IPCSearch.CurrentJobComboStatesCategorized.TryGetValue(job,
            out var comboStates);

        if (comboStates is null)
            return null;

        #endregion

        List<string> combos = [];

        #region Single Target

        comboStates[ComboTargetTypeKeys.SingleTarget]
            .TryGetValue(ComboSimplicityLevelKeys.Simple, out var stSimpleResults);
        var stSimple =
            stSimpleResults?.FirstOrDefault();

        if (stSimple is not null)
            combos.Add(comboStates[ComboTargetTypeKeys.SingleTarget]
                [ComboSimplicityLevelKeys.Simple].First().Key);
        else
        {
            var stAdvanced = comboStates[ComboTargetTypeKeys.SingleTarget]
                [ComboSimplicityLevelKeys.Advanced].First().Key;
            combos.Add(stAdvanced);
            if (includeOptions)
                combos.AddRange(P.IPCSearch.OptionNamesByJob[job][stAdvanced]);
        }

        #endregion

        #region Multi Target

        comboStates[ComboTargetTypeKeys.MultiTarget]
            .TryGetValue(ComboSimplicityLevelKeys.Simple, out var mtSimpleResults);
        var mtSimple =
            mtSimpleResults?.FirstOrDefault();

        if (mtSimple is not null)
            combos.Add(comboStates[ComboTargetTypeKeys.MultiTarget]
                [ComboSimplicityLevelKeys.Simple].First().Key);
        else
        {
            var mtAdvanced = comboStates[ComboTargetTypeKeys.MultiTarget]
                [ComboSimplicityLevelKeys.Advanced].First().Key;
            combos.Add(mtAdvanced);
            if (includeOptions)
                combos.AddRange(P.IPCSearch.OptionNamesByJob[job][mtAdvanced]);
        }

        #endregion

        #region Heals

        if (comboStates.TryGetValue(ComboTargetTypeKeys.HealST, out var healResults))
            combos.Add(healResults
                [ComboSimplicityLevelKeys.Other].First().Key);
        var healST = healResults?.FirstOrDefault().Key;
        if (healST is not null)
        {
            var healSTPreset = comboStates[ComboTargetTypeKeys.HealST]
                [ComboSimplicityLevelKeys.Other].First().Key;
            if (includeOptions)
                combos.AddRange(P.IPCSearch.OptionNamesByJob[job][healSTPreset]);
        }

        if (comboStates.TryGetValue(ComboTargetTypeKeys.HealMT, out healResults))
            combos.Add(healResults
                [ComboSimplicityLevelKeys.Other].First().Key);
        var healMT = healResults?.FirstOrDefault().Key;
        if (healMT is not null)
        {
            var healMTPreset = comboStates[ComboTargetTypeKeys.HealMT]
                [ComboSimplicityLevelKeys.Other].First().Key;
            if (includeOptions)
                combos.AddRange(P.IPCSearch.OptionNamesByJob[job][healMTPreset]);
        }

        #endregion

        if (includeOptions)
            CombosForARCache[job] = combos;
        return combos;
    }

    #endregion

    #region IPC Callback

    public static string? PrefixForIPC;

    /// <summary>
    ///     Method to set up an IPC, call the Wrath Combo callback, and dispose
    ///     of the IPC.
    /// </summary>
    /// <param name="prefix">The leasee's </param>
    /// <param name="reason"></param>
    /// <param name="additionalInfo"></param>
    internal static void CallIPCCallback(string prefix, CancellationReason reason,
        string additionalInfo = "")
    {
        try
        {
            PrefixForIPC = prefix;
            LeaseeIPC.WrathComboCallback((int)reason, additionalInfo);
            LeaseeIPC.Dispose();
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

internal static class LeaseeIPC
{
    private static EzIPCDisposalToken[]? _disposalTokens =
        EzIPC.Init(typeof(LeaseeIPC), Helper.PrefixForIPC, SafeWrapper.IPCException);

#pragma warning disable CS0649, CS8618 // Complaints of the method
    [EzIPC] internal static readonly Action<int, string> WrathComboCallback;
#pragma warning restore CS8618, CS0649

    public static void Dispose()
    {
        if (_disposalTokens is null)
            return;
        foreach (var token in _disposalTokens)
            token.Dispose();
        _disposalTokens = null;
    }
}
