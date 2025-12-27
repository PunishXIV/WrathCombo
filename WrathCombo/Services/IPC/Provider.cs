#region

using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WrathCombo.API.Enum;
using WrathCombo.API.Extension;
using WrathCombo.Combos;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

#endregion

namespace WrathCombo.Services.IPC;

/// <summary>
///     IPC service for other plugins to have user-overridable control of Wrath.<br />
///     See <see cref="RegisterForLease(string,string)" /> for details on use.
///     <br />
///     See the "Normal IPC Flow" region for the main IPC methods.
/// </summary>
public partial class Provider : IDisposable
{
    /// <summary>
    ///     Method to test IPC.
    /// </summary>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void Test() => Logging.Log("IPC connection successful.");

    #region Helpers

    /// <summary>
    ///     Leasing services for the IPC, essentially a backer for <c>Set</c>
    ///     methods.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    internal readonly Leasing Leasing;

    /// <summary>
    ///     The helper services for the IPC provider.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    internal readonly Helper Helper;

    /// <summary>
    ///     Whether the IPC (when initialized by <see cref="Init"/>) is ready.
    /// </summary>
    private bool _ipcReady;

    /// <summary>
    ///     Initializes the class, and sets up the other parts of the IPC provider.
    /// </summary>
    private Provider()
    {
        Leasing = new Leasing();
        Helper = new Helper(ref Leasing);
    }

    /// <summary>
    ///     Initializes the IPC provider, setting up the IPC and the helper services.
    /// </summary>
    /// <returns><see cref="Provider" /></returns>
    /// <seealso cref="Provider()"/>
    public static Provider Init()
    {
        Provider output = new();

        // Initiate the IPC and helper services
        EzIPC.Init(output, prefix: "WrathCombo");
        P.IPCSearch = new Search(output.Leasing);
        P.UIHelper = new UIHelper(output.Leasing);

        // Build Caches of presets
        Svc.Framework.RunOnTick(BuildCachesAction(output));

        return output;
    }

    /// <summary>
    ///     A token to cancel <see cref="BuildCaches" /> if the IPC is disabled.
    /// </summary>
    private static readonly CancellationTokenSource ActionToken = new();

    /// <summary>
    ///     Just provides a signature-compatible way to call <see cref="BuildCaches" />
    ///     with <see cref="Svc.Framework" />.
    /// </summary>
    /// <param name="output">The IPC provider instance to set ready.</param>
    /// <returns>An Action of <see cref="BuildCaches" /></returns>
    internal static Action BuildCachesAction(Provider? output = null)
    {
        return () => BuildCaches(output ?? P.IPC);
    }

    /// <summary>
    /// Builds necessary caches for IPC functionality.
    /// Called after initialization to ensure all data is ready for IPC interactions.
    /// </summary>
    /// <param name="output">The IPC provider instance to set ready.</param>
    private static void BuildCaches(Provider output)
    {
        // Respect the token
        if (ActionToken.IsCancellationRequested)
        {
            Logging.Verbose("IPC caches cancelled, IPC disabled");
            return;
        }

        // Wait until player is ready
        if (!Svc.ClientState.IsLoggedIn || !Player.Available)
        {
            Svc.Framework.RunOnTick(BuildCachesAction(output),
                TimeSpan.FromSeconds(3));
            Logging.Verbose("IPC caches delayed, waiting for player-ready");
            return;
        }

        // Getting the IPC status early
        Task.Run(() => P.IPC.Helper.IPCEnabled);

        // Build job-specific combo state caches
        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        P.IPCSearch.ComboStatesByJob.TryGetValue(Player.Job, out _);

        // Build UI-state caches
        P.UIHelper.PresetControlled(Preset.AST_ST_DPS);

        // Mark IPC as ready after caches are built
        output._ipcReady = true;

        Logging.Log("IPC caches built successfully");
    }

    /// <summary>
    ///     Disposes of the IPC provider, cancelling all leases.
    /// </summary>
    public void Dispose()
    {
        ActionToken.Cancel();
        Leasing.SuspendLeases(CancellationReason.WrathPluginDisabled);
    }

    #endregion

    #region Normal IPC Flow

    /// <summary>
    ///     Checks the state of the Wrath IPC.<br />
    ///     Subscribers should check this before using IPC methods, especially
    ///     if working on a local build of Wrath Combo.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> once IPC has been fully initialised, including
    ///     caches.
    /// </returns>
    [EzIPC]
    public bool IPCReady()
    {
        return _ipcReady;
    }

    /// <summary>
    ///     Register your plugin for control of Wrath Combo.<br />
    ///     Use
    ///     <see cref="RegisterForLeaseWithCallback">
    ///         RegisterForLeaseWithCallback
    ///     </see>
    ///     instead to provide a callback for when your lease is cancelled.
    /// </summary>
    /// <param name="internalPluginName">
    ///     The internal name of your plugin.<br />
    ///     Needs to be the actual internal name of your plugin, as it will be used
    ///     to check if your plugin is still loaded.
    /// </param>
    /// <param name="pluginName">
    ///     The name you want shown to Wrath users for options your plugin controls.
    /// </param>
    /// <returns>
    ///     Your lease ID to be used in <c>set</c> calls.<br />
    ///     Or <c>null</c> if your lease was not registered, which can happen for
    ///     multiple reasons:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A lease exists with the <c>pluginName</c>.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Your lease was revoked by the user recently.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 The IPC service is currently disabled.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    /// <remarks>
    ///     None of this will work correctly - or sometimes at all - with PvP.
    /// </remarks>
    /// <seealso cref="RegisterForLeaseWithCallback" />
    /// <seealso cref="RegisterForLease(string,string,Action{int,string})" />
    [EzIPC]
    public Guid? RegisterForLease
        (string internalPluginName, string pluginName)
    {
        // Bail if IPC is disabled
        if (Helper.CheckForBailConditionsAtSetTime(out _))
            return null;

        return Leasing.CreateRegistration(internalPluginName, pluginName);
    }

    /// <summary>
    ///     Register your plugin for control of Wrath Combo.<br />
    ///     IPC implementation of a callback for
    ///     <see cref="RegisterForLease(string,string)">RegisterForLease</see>.<br />
    ///     This is the main method to provide a callback for when your lease is
    ///     cancelled.
    /// </summary>
    /// <param name="internalPluginName">
    ///     See: <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <param name="pluginName">
    ///     See: <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <param name="ipcPrefixForCallback">
    ///     The prefix you want to use for your IPC calls.<br />
    ///     <c>null</c> if your <c>internalPluginName</c> is the same as your
    ///     IPC prefix.<br />
    ///     <c>string</c> would be the prefix you want to use for your IPC calls.
    /// </param>
    /// <returns>
    ///     See: <see cref="RegisterForLease(string,string)" />
    /// </returns>
    /// <remarks>
    ///     Requires you to provide an IPC method to be called when your lease is
    ///     cancelled, usually by the user.<br />
    ///     The <see cref="CancellationReason" /> (cast as an int) and a string with
    ///     any additional info will be passed to your method.<br />
    ///     The method should be of the form
    ///     <c>void WrathComboCallback(int, string)</c>.<br />
    ///     See <see cref="LeaseeIPC.WrathComboCallback" /> for the exact signature that
    ///     will be called.
    /// </remarks>
    /// <seealso cref="RegisterForLease(string,string)" />
    [EzIPC]
    public Guid? RegisterForLeaseWithCallback
        (string internalPluginName, string pluginName, string? ipcPrefixForCallback)
    {
        // Bail if IPC is disabled
        if (Helper.CheckForBailConditionsAtSetTime(out _))
            return null;

        // Assign the IPC prefix if indicated it is the same as the internal name
        ipcPrefixForCallback ??= internalPluginName;

        return Leasing.CreateRegistration(internalPluginName, pluginName,
            ipcPrefixForCallback: ipcPrefixForCallback);
    }

    /// <summary>
    ///     Register your plugin for control of Wrath Combo.<br />
    ///     Direct <c>Action</c> implementation of a callback for
    ///     <see cref="RegisterForLease(string,string)">RegisterForLease</see>.<br />
    ///     Primarily for testing, or where a callback is desired without providing
    ///     an IPC.
    /// </summary>
    /// <param name="internalPluginName">
    ///     See: <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <param name="pluginName">
    ///     See: <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <param name="leaseCancelledCallback">
    ///     Your method to be called when your lease is cancelled, usually
    ///     by the user.<br />
    ///     The <see cref="CancellationReason" /> (cast as an int) and a string with
    ///     any additional info will be passed to your method.
    /// </param>
    /// <returns>
    ///     See: <see cref="RegisterForLease(string,string)" />
    /// </returns>
    /// <seealso cref="RegisterForLease(string,string)" />
    public Guid? RegisterForLease
    (string internalPluginName, string pluginName,
        Action<int, string> leaseCancelledCallback)
    {
        // Bail if IPC is disabled
        if (Helper.CheckForBailConditionsAtSetTime(out _))
            return null;

        return Leasing.CreateRegistration(
            internalPluginName, pluginName, leaseCancelledCallback);
    }

    /// <summary>
    ///     Get the current state of the Auto-Rotation setting in Wrath Combo.
    /// </summary>
    /// <returns>Whether Auto-Rotation is enabled or disabled</returns>
    /// <remarks>
    ///     This is only the state of Auto-Rotation, not whether any combos are
    ///     enabled in Auto-Mode.
    /// </remarks>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public bool GetAutoRotationState() =>
        Leasing.CheckAutoRotationControlled() ??
        Service.Configuration.RotationConfig.Enabled;

    /// <summary>
    ///     Set the state of Auto-Rotation in Wrath Combo.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from
    ///     <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <param name="enabled">
    ///     Optionally whether to enable Auto-Rotation.<br />
    ///     Only used to disable Auto-Rotation, as enabling it is the default.
    /// </param>
    /// <returns>
    ///     The <see cref="SetResult" /> status code indicating the result of the
    ///     operation.
    /// </returns>
    /// <seealso cref="GetAutoRotationState" />
    /// <remarks>
    ///     This is only the state of Auto-Rotation, not whether any combos are
    ///     enabled in Auto-Mode.
    /// </remarks>
    [EzIPC]
    public SetResult SetAutoRotationState(Guid lease, bool enabled = true)
    {
        // Bail for standard conditions
        if (Helper.CheckForBailConditionsAtSetTime(out var result, lease))
            return result;

        return Leasing.AddRegistrationForAutoRotation(lease, enabled);
    }

    /// <summary>
    ///     The last time there was a full check for the current job's readiness.
    /// </summary>
    private DateTime _lastJobReadyCheck = DateTime.MinValue;

    /// <summary>
    ///     The state of the last <see cref="IsCurrentJobAutoRotationReady" /> check.
    /// </summary>
    private bool _lastJobReady;

    /// <summary>
    ///     Checks if the current job has a Single and Multi-Target combo configured
    ///     that are enabled in Auto-Mode.
    /// </summary>
    /// <returns>
    ///     If the user's current job is fully ready for Auto-Rotation.
    /// </returns>
    /// <seealso cref="IsCurrentJobConfiguredOn" />
    /// <seealso cref="IsCurrentJobAutoModeOn" />
    [EzIPC]
    public bool IsCurrentJobAutoRotationReady()
    {
        if (File.GetLastWriteTime(P.IPCSearch.ConfigFilePath) <= _lastJobReadyCheck &&
            (Leasing.CombosUpdated ?? DateTime.MinValue) <= _lastJobReadyCheck &&
            !EZ.Throttle("ipcJobReadyCheck", TS.FromSeconds(30)))
            return _lastJobReady;

        // Check if the current job has a Single and Multi-Target combo configured on
        var jobOn = IsCurrentJobConfiguredOn();
        // Check if the current job has those same combos configured on in Auto-Mode
        var jobAutoOn = InternalIsCurrentJobAutoModeOn(jobOn);

        // Check that all combos are configured and enabled in Auto-Mode
        var allGood = jobOn.All(x => x.Value is not null) &&
               jobAutoOn.All(x => x.Value is not null);

        // Log if not ready
        if (!allGood && EZ.Throttle("ipcJobReadyCheckLog", TS.FromSeconds(5)))
            Logging.Log(
                $"Current job is not fully ready for Auto-Rotation.\n" +
                $"jobOn: {JsonConvert.SerializeObject(jobOn)}\n" +
                $"jobAutoOn: {JsonConvert.SerializeObject(jobAutoOn)}"
            );

        _lastJobReadyCheck = DateTime.Now;
        _lastJobReady = allGood;
        return allGood;
    }

    /// <summary>
    ///     Sets up the user's current job for Auto-Rotation.<br />
    ///     This will enable the Single and Multi-Target combos, and enable them in
    ///     Auto-Mode.<br />
    ///     This will try to use the user's existing settings, only enabling default
    ///     states for jobs that are not configured.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from
    ///     <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <returns>
    ///     The <see cref="SetResult" /> status code indicating the result of the
    ///     operation.
    /// </returns>
    /// <remarks>
    ///     This will do the actual <c>set</c>ting asynchronously, and will take a
    ///     several seconds to complete.
    /// </remarks>
    [EzIPC]
    public SetResult SetCurrentJobAutoRotationReady(Guid lease)
    {
        // Bail for standard conditions
        if (Helper.CheckForBailConditionsAtSetTime(out var result, lease))
            return result;

        return Leasing.AddRegistrationForCurrentJob(lease);
    }

    /// <summary>
    ///     This cancels your lease, removing your control of Wrath Combo.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from
    ///     <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <remarks>
    ///     Will call your <c>leaseCancelledCallback</c> method if you provided one,
    ///     with the reason <see cref="CancellationReason.LeaseeReleased" />.
    /// </remarks>
    [EzIPC]
    public void ReleaseControl(Guid lease)
    {
        // Bail if the lease does not exist
        if (!Leasing.CheckLeaseExists(lease))
        {
            Logging.Warn(BailMessage.InvalidLease.Description);
            return;
        }

        Leasing.RemoveRegistration(lease, CancellationReason.LeaseeReleased);
    }

    #endregion

    #region Extra Job State Checks

    /// <summary>
    ///     Checks if the user's current job has a Single-Target and Multi-Target
    ///     combo configured.
    /// </summary>
    /// <returns>
    ///     <see cref="ComboTargetTypeKeys.SingleTarget" /> - a
    ///     <see cref="ComboSimplicityLevelKeys">SimplicityLevel?</see> indicating
    ///     what mode, if any, is enabled for Auto-Mode for Single-Target.<br />
    ///     <see cref="ComboTargetTypeKeys.MultiTarget" /> - a
    ///     <see cref="ComboSimplicityLevelKeys">SimplicityLevel?</see> indicating
    ///     what mode, if any, is enabled for Auto-Mode for Multi-Target.<br />
    /// </returns>
    /// <seealso cref="Helper.CheckCurrentJobModeIsEnabled" />
    [EzIPC]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?> IsCurrentJobConfiguredOn()
    {
        return new Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?>
        {
            {
                ComboTargetTypeKeys.SingleTarget,
                Helper.CheckCurrentJobModeIsEnabled(
                    ComboTargetTypeKeys.SingleTarget, ComboStateKeys.Enabled)
            },
            {
                ComboTargetTypeKeys.MultiTarget,
                Helper.CheckCurrentJobModeIsEnabled(
                    ComboTargetTypeKeys.MultiTarget, ComboStateKeys.Enabled)
            },
        };
    }

    /// <summary>
    ///     Checks if the user's current job has a Single-Target and Multi-Target
    ///     combo enabled in Auto-Mode.
    /// </summary>
    /// <returns>
    ///     <see cref="ComboTargetTypeKeys.SingleTarget" /> - a
    ///     <see cref="ComboSimplicityLevelKeys">SimplicityLevel?</see> indicating
    ///     what mode, if any, is enabled for Auto-Mode for Single-Target.<br />
    ///     <see cref="ComboTargetTypeKeys.MultiTarget" /> - a
    ///     <see cref="ComboSimplicityLevelKeys">SimplicityLevel?</see> indicating
    ///     what mode, if any, is enabled for Auto-Mode for Multi-Target.<br />
    /// </returns>
    [EzIPC]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?>
        IsCurrentJobAutoModeOn()
    {
        return new Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?>
        {
            {
                ComboTargetTypeKeys.SingleTarget,
                Helper.CheckCurrentJobModeIsEnabled(
                    ComboTargetTypeKeys.SingleTarget, ComboStateKeys.AutoMode)
            },
            {
                ComboTargetTypeKeys.MultiTarget,
                Helper.CheckCurrentJobModeIsEnabled(
                    ComboTargetTypeKeys.MultiTarget, ComboStateKeys.AutoMode)
            },
        };
    }

    /// <summary>
    ///     Same as <see cref="IsCurrentJobAutoModeOn" />, but with a parameter
    ///     to make sure that Auto-Mode is enabled for the same enabled combos.
    /// </summary>
    /// <param name="previousMatches">
    ///     The result of <see cref="IsCurrentJobConfiguredOn" />.
    /// </param>
    /// <returns></returns>
    /// <seealso cref="IsCurrentJobAutoModeOn"/>
    private Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?>
        InternalIsCurrentJobAutoModeOn
        (Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?> previousMatches)
    {
        return new Dictionary<ComboTargetTypeKeys, ComboSimplicityLevelKeys?>
        {
            {
                ComboTargetTypeKeys.SingleTarget,
                Helper.CheckCurrentJobModeIsEnabled(
                    ComboTargetTypeKeys.SingleTarget, ComboStateKeys.AutoMode,
                    previousMatches[ComboTargetTypeKeys.SingleTarget])
            },
            {
                ComboTargetTypeKeys.MultiTarget,
                Helper.CheckCurrentJobModeIsEnabled(
                    ComboTargetTypeKeys.MultiTarget, ComboStateKeys.AutoMode,
                    previousMatches[ComboTargetTypeKeys.MultiTarget])
            },
        };
    }

    #endregion

    #region Fine-Grained Combo Methods

    /// <summary>
    ///     Gets the internal names of all combos for the given job.
    /// </summary>
    /// <param name="jobID">
    ///     The <see cref="ECommons.ExcelServices.Job" /> to get combos for.
    /// </param>
    /// <returns>
    ///     A list of internal names for all combos and options for the given job.
    /// </returns>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public List<string>? GetComboNamesForJob(uint jobID) =>
        P.IPCSearch.ComboNamesByJob.GetValueOrDefault((Job)jobID);

    /// <summary>
    ///     Gets the names of all combo options for the given job.
    /// </summary>
    /// <param name="jobID">
    ///     The <see cref="ECommons.ExcelServices.Job" /> to get options for.
    /// </param>
    /// <returns>
    ///     A dictionary of combo internal names and under each, a list of options'
    ///     internal names, for the given job.
    /// </returns>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public Dictionary<string, List<string>>? GetComboOptionNamesForJob(uint jobID) =>
        P.IPCSearch.OptionNamesByJob.GetValueOrDefault((Job)jobID);

    /// <summary>
    ///     Get the current state of a combo in Wrath Combo.
    /// </summary>
    /// <param name="comboInternalName">
    ///     The internal name of the combo you want to check.<br />
    ///     See <see cref="Preset" /> or
    ///     <see cref="GetComboNamesForJob" />.<br />
    ///     Does also accept the ID of presets.
    /// </param>
    /// <returns>
    ///     <see cref="ComboStateKeys.Enabled" /> - a <c>bool</c> indicating if
    ///     the combo is enabled.<br />
    ///     <see cref="ComboStateKeys.AutoMode" /> - a <c>bool</c> indicating if the
    ///     combo is enabled in Auto-Mode.
    /// </returns>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public Dictionary<ComboStateKeys, bool>? GetComboState(string comboInternalName)
    {
        // Override if the combo is controlled by a lease
        var checkLeasing = Leasing.CheckComboControlled(comboInternalName);
        if (checkLeasing is not null)
        {
            return new Dictionary<ComboStateKeys, bool>
            {
                {
                    ComboStateKeys.Enabled, checkLeasing.Value.enabled
                },
                {
                    ComboStateKeys.AutoMode, checkLeasing.Value.autoMode
                }
            };
        }

        // Otherwise just the saved state
        return P.IPCSearch.PresetStates.GetValueOrDefault(comboInternalName);
    }

    /// <summary>
    ///     Set the state of a combo in Wrath Combo.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from
    ///     <see cref="RegisterForLease(string,string)" />
    /// </param>
    /// <param name="comboInternalName">
    ///     The internal name of the combo you want to set.<br />
    ///     See <see cref="Preset" /> or
    ///     <see cref="GetComboNamesForJob" />.<br />
    ///     Does also accept the ID of presets.
    /// </param>
    /// <param name="comboState">
    ///     Optionally whether to enable combo.<br />
    ///     Only used to disable the combo, as enabling it is the default.
    /// </param>
    /// <param name="autoState">
    ///     Optionally whether to enable the combo in Auto-Mode.<br />
    ///     Only used to disable the combo in Auto-Mode, as enabling it is the
    ///     default.
    /// </param>
    /// <returns>
    ///     The <see cref="SetResult" /> status code indicating the result of the
    ///     operation.
    /// </returns>
    /// <returns>
    ///     The <see cref="SetResult" /> status code indicating the result of the
    ///     operation.
    /// </returns>
    [EzIPC]
    public SetResult SetComboState
    (Guid lease, string comboInternalName,
        bool comboState = true, bool autoState = true)
    {
        // Bail for standard conditions
        if (Helper.CheckForBailConditionsAtSetTime(out var result, lease))
            return result;

        return Leasing.AddRegistrationForCombo(
            lease, comboInternalName, comboState, autoState);
    }

    /// <summary>
    ///     Gets the current state of a combo option in Wrath Combo.
    /// </summary>
    /// <param name="optionName">
    ///     The internal name of the combo option you want to check.<br />
    ///     Does also accept the ID of presets.
    /// </param>
    /// <returns>
    ///     A <c>bool</c> indicating if the combo option is enabled.
    /// </returns>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public bool GetComboOptionState(string optionName)
    {
        // Override if the combo option is controlled by a lease,
        // otherwise return the saved state
        return Leasing.CheckComboOptionControlled(optionName) ??
               P.IPCSearch.PresetStates.GetValueOrDefault(optionName)[
                   ComboStateKeys.Enabled];
    }

    /// <summary>
    ///     Sets the state of a combo option in Wrath Combo.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="RegisterForLease(string,string)" />.
    /// </param>
    /// <param name="optionName">
    ///     The internal name of the combo option you want to set.<br />
    ///     Does also accept the ID of presets.
    /// </param>
    /// <param name="state">
    ///     Optionally whether to enable the combo option.<br />
    ///     Only used to disable the combo option, as enabling it is the default.
    /// </param>
    /// <returns>
    ///     The <see cref="SetResult" /> status code indicating the result of the
    ///     operation.
    /// </returns>
    [EzIPC]
    public SetResult SetComboOptionState
        (Guid lease, string optionName, bool state = true)
    {
        // Bail for standard conditions
        if (Helper.CheckForBailConditionsAtSetTime(out var result, lease))
            return result;

        return Leasing.AddRegistrationForOption(lease, optionName, state);
    }

    #endregion
}
