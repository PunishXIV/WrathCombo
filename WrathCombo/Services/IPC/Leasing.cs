#region

// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using WrathCombo.Combos;
using WrathCombo.CustomComboNS.Functions;
using CancellationReasonEnum = WrathCombo.Services.IPC.CancellationReason;

// ReSharper disable UseSymbolAlias
// ReSharper disable UnusedMember.Global

#endregion

namespace WrathCombo.Services.IPC;

public class Lease(
    string internalPluginName,
    string pluginName,
    Action<int, string>? callback,
    string? ipcPrefixForCallback = null)
{
    /// <summary>
    ///     The identifier for this lease.<br />
    ///     Given to the plugin when registering for a lease to identify themselves.
    /// </summary>
    public Guid ID { get; } = Guid.NewGuid();
    /// <summary>
    ///     The internal name of the registering plugin.<br />
    ///     Used in <see cref="Leasing.CheckIfLeaseePluginsUnloaded"/> to check if the
    ///     plugin has been unloaded.
    /// </summary>
    public string InternalPluginName { get; } = internalPluginName;
    /// <summary>
    ///     The name to display for the registering plugin.
    /// </summary>
    public string PluginName { get; } = pluginName;
    /// <summary>
    ///     The callback to call when the lease is cancelled.<br />
    ///     Only from: <see cref="Provider.RegisterForLease(string,string,Action{int,string})" />
    /// </summary>
    public Action<int, string>? Callback { get; } = callback;
    /// <summary>
    ///     The IPC prefix to use for the callback.<br />
    ///     Only from: <see cref="Provider.RegisterForLeaseWithCallback" />
    /// </summary>
    public string? IPCPrefixForCallback { get; } = ipcPrefixForCallback;

    /// <summary>
    ///     The date and time this lease was created.
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    private DateTime Created { get; } = DateTime.Now;
    /// <summary>
    ///     The date and time this lease was last updated.
    /// </summary>
    internal DateTime LastUpdated { get; set; } = DateTime.Now;

    /// <summary>
    ///     A simple checksum of the configurations controlled by this registration.
    /// </summary>
    internal byte[] ConfigurationsHash
    {
        get
        {
            var allKeys = AutoRotationControlled.Keys
                .Select(k => k.ToString())
                .Concat(JobsControlled.Keys.Select(k => k.ToString()))
                .Concat(CombosControlled.Keys.Select(k => k.ToString()))
                .Concat(OptionsControlled.Keys.Select(k => k.ToString()))
                .ToArray();

            var concatenatedKeys = string.Join(",", allKeys);
            return SHA256.HashData(Encoding.UTF8.GetBytes(concatenatedKeys));
        }
    }

    /// <summary>
    ///     The number of sets leased by this registration currently.
    /// </summary>
    public int SetsLeased =>
        AutoRotationControlled.Count +
        JobsControlled.Count +
        CombosControlled.Count +
        OptionsControlled.Count;

    #region Configurations controlled by this lease

    internal Dictionary<byte, bool> AutoRotationControlled { get; set; } = new();

    internal Dictionary<AutoRotationConfigOption, int> AutoRotationConfigsControlled
    {
        get;
        set;
    } = new();

    internal Dictionary<Job, bool> JobsControlled { get; set; } = new();

    internal Dictionary<CustomComboPreset, (bool enabled, bool autoMode)>
        CombosControlled { get; set; } = new();

    internal Dictionary<CustomComboPreset, bool> OptionsControlled { get; set; } =
        new();

    #endregion

    /// <summary>
    ///     Cancels the lease.<br/>
    ///     Will invoke the callback if one was provided either as an
    ///     <see cref="Callback">Action</see> or a via
    ///     <see cref="IPCPrefixForCallback">IPC</see>.
    /// </summary>
    /// <param name="cancellationReason">
    ///     The <see cref="CancellationReasonEnum" /> for cancelling the lease.
    /// </param>
    /// <param name="additionalInfo">
    ///     Any additional information to provide with the cancellation.
    /// </param>
    /// <remarks>
    ///     Usually called by
    ///     <see cref="Leasing.RemoveRegistration">RemoveRegistration()</see>,
    ///     which is often called by <see cref="Provider.ReleaseControl" /> or
    ///     by the user via <see cref="UIHelper.RevokeControl"/>.
    /// </remarks>
    public void Cancel
        (CancellationReasonEnum cancellationReason, string additionalInfo = "")
    {
        Logging.Log(
            "Cancelling Lease for: "
            + PluginName
            + " (" + cancellationReason + ")" +
            (additionalInfo != ""
                ? "\n" + additionalInfo
                : "")
        );

        if (Callback is not null)
            Callback.Invoke((int)cancellationReason, additionalInfo);
        else if (IPCPrefixForCallback is not null)
            Helper.CallIPCCallback(IPCPrefixForCallback, cancellationReason, additionalInfo);
    }
}

public partial class Leasing
{
    /// <summary>
    ///     Active leases.
    /// </summary>
    internal Dictionary<Guid, Lease> Registrations = new();

    #region Cache Bust dates

    /// <summary>
    ///     When the Auto-Rotation state was last updated.<br />
    ///     Used to bust the UI cache.<br />
    ///     <c>null</c> if never updated.
    /// </summary>
    internal DateTime? AutoRotationStateUpdated;

    /// <summary>
    ///     When the Auto-Rotation configurations were last updated.<br />
    ///     Used to bust the UI cache.<br />
    ///     <c>null</c> if never updated.
    /// </summary>
    internal DateTime? AutoRotationConfigsUpdated;

    /// <summary>
    ///     When Jobs-controlled were last updated.<br />
    ///     Used to bust the UI cache.<br />
    ///     <c>null</c> if never updated.
    /// </summary>
    internal DateTime? JobsUpdated;

    /// <summary>
    ///     When Combos-controlled were last updated.<br />
    ///     Used to bust the UI cache.<br />
    ///     <c>null</c> if never updated.
    /// </summary>
    internal DateTime? CombosUpdated;

    /// <summary>
    ///     When Options-controlled were last updated.<br />
    ///     Used to bust the UI cache.<br />
    ///     <c>null</c> if never updated.
    /// </summary>
    internal DateTime? OptionsUpdated;

    #endregion

    #region Normal IPC Flow

    /// <summary>
    ///     Creates a new <see cref="Lease" /> and saves it to
    ///     <see cref="Registrations" />, ensuring the lease ID is unique.
    /// </summary>
    /// <param name="internalPluginName">
    ///     The internal name of the registering plugin.
    /// </param>
    /// <param name="pluginName">The name of the registering plugin.</param>
    /// <param name="callback">
    ///     The cancellation callback for that plugin.<br />
    ///     Note: only from
    ///     <see cref="Provider.RegisterForLease(string,string,Action{int,string})" />
    /// </param>
    /// <param name="ipcPrefixForCallback">
    ///     The cancellation callback for that plugin.<br />
    ///     Note: only from
    ///     <see cref="Provider.RegisterForLeaseWithCallback" />
    /// </param>
    /// <returns>
    ///     The lease ID to be used by the plugin in subsequent calls.<br />
    ///     Or <c>null</c> if the plugin is blacklisted.
    /// </returns>
    /// <seealso cref="Provider.RegisterForLease(string,string)" />
    /// <seealso cref="Provider.RegisterForLeaseWithCallback" />
    /// <seealso cref="Provider.RegisterForLease(string,string,Action{int,string})" />
    internal Guid? CreateRegistration
    (string internalPluginName, string pluginName,
        Action<int, string>? callback = null, string? ipcPrefixForCallback = null)
    {
        // Bail if the plugin is temporarily blacklisted
        if (CheckBlacklist(internalPluginName))
        {
            Logging.Warn(
                $"{pluginName}: Plugin is temporarily blacklisted, cannot register for a lease");
            return null;
        }

        // Make sure the lease ID is unique
        // (unnecessary, but could save a big headache)
        Lease lease;
        do
        {
            // Create a new lease
            lease = new Lease
            (internalPluginName, pluginName,
                callback, ipcPrefixForCallback);
        } while (CheckLeaseExists(lease.ID) || CheckBlacklist(lease.ID));

        // Save the lease
        Registrations.Add(lease.ID, lease);

        Logging.Log($"{pluginName}: Created Lease");

        // Provide the lease ID to the plugin
        return lease.ID;
    }

    /// <summary>
    ///     When <see cref="CheckAutoRotationControlled"/> was last cached.
    /// </summary>
    private DateTime? _lastAutoRotationStateCheck;

    /// <summary>
    ///     Cached value of <see cref="CheckAutoRotationControlled"/>
    /// </summary>
    private bool? _autoRotationStateUpdated;

    /// <summary>
    ///     Checks if Auto-Rotation's state is controlled by a lease.
    /// </summary>
    /// <returns>
    ///     The state Auto-Rotation is controlled to, or <c>null</c> if it is not.
    /// </returns>
    /// <seealso cref="Provider.GetAutoRotationState" />
    internal bool? CheckAutoRotationControlled()
    {
        if (AutoRotationStateUpdated is null ||
            Registrations.Count == 0)
            return null;

        if (_lastAutoRotationStateCheck >= AutoRotationStateUpdated)
            return _autoRotationStateUpdated;

        var lease = Registrations.Values
            .Where(l => l.AutoRotationControlled.Count != 0)
            .OrderByDescending(l => l.LastUpdated)
            .FirstOrDefault();

        _lastAutoRotationStateCheck = DateTime.Now;
        _autoRotationStateUpdated = lease?.AutoRotationControlled[0];

        return _autoRotationStateUpdated;
    }

    /// <summary>
    ///     Adds a registration for Auto-Rotation control to a lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <param name="newState">Whether to enabled Auto-Rotation.</param>
    /// <seealso cref="Provider.SetAutoRotationState" />
    internal void AddRegistrationForAutoRotation(Guid lease, bool newState)
    {
        var registration = Registrations[lease];

        if (registration.AutoRotationConfigsControlled.Count > 0 &&
            registration.AutoRotationControlled[0] == newState)
        {
            Logging.Log(
                $"{registration.PluginName}: You are already controlling Auto-Rotation");
            return;
        }

        // Always [0], not an actual add
        registration.AutoRotationControlled[0] = newState;

        registration.LastUpdated = DateTime.Now;
        AutoRotationStateUpdated = DateTime.Now;

        Logging.Log($"{registration.PluginName}: Auto-Rotation state updated");
    }

    /// <summary>
    ///     Checks if a lease controls the current job.
    /// </summary>
    /// <returns>
    ///     The state the current job is controlled to, or <c>null</c> if it is not.
    /// </returns>
    /// <seealso cref="Provider.IsCurrentJobAutoRotationReady" />
    /// <seealso cref="Provider.IsCurrentJobConfiguredOn" />
    /// <seealso cref="Provider.IsCurrentJobAutoModeOn" />
    internal bool? CheckJobControlled(int? job = null)
    {
        if (CustomComboFunctions.LocalPlayer is null)
            return null;

        var currentJob = (Job)CustomComboFunctions.LocalPlayer.ClassJob.RowId;
        var resolvedJob = job is null ? currentJob : (Job)job;

        var lease = Registrations.Values
            .Where(l => l.JobsControlled.ContainsKey(resolvedJob))
            .OrderByDescending(l => l.LastUpdated)
            .FirstOrDefault();

        return lease?.JobsControlled[resolvedJob];
    }

    /// <summary>
    ///     Adds a registration for the current Job to a lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <param name="jobOverride">A manual override, only used in testing</param>
    /// <seealso cref="Provider.SetCurrentJobAutoRotationReady" />
    internal void AddRegistrationForCurrentJob(Guid lease, Job? jobOverride = null)
    {
        var registration = Registrations[lease];

        if (CustomComboFunctions.LocalPlayer is null)
        {
            Logging.Error(
                "Failed to register current job: player object does not exist!");
            return;
        }

        // Convert current job/class to a job, if it is a class
        var currentJobRow = CustomComboFunctions.LocalPlayer.ClassJob;
        var currentRealJob = currentJobRow.Value.RowId;
        if (currentJobRow.Value.ClassJobParent.RowId != currentJobRow.Value.RowId)
            currentRealJob =
                CustomComboFunctions.JobIDs.ClassToJob(currentJobRow.RowId);

        var currentJob = (Job)currentRealJob;
        if (jobOverride is not null)
            currentJob = jobOverride.Value;
        var job = currentJob.ToString();

        if (!registration.JobsControlled.TryAdd(currentJob, true))
        {
            Logging.Log(
                $"{registration.PluginName}: You are already controlling the current job ({job})");
            return;
        }

        Logging.Log(
            $"{registration.PluginName}: Registering Current Job ({job}) ...");

        Task.Run(() =>
        {
            bool locking;
            var combos = Helper.GetCombosToSetJobAutoRotationReady(currentJob, false)!;
            var options = Helper.GetCombosToSetJobAutoRotationReady(currentJob)!;
            string[] stringKeys;

            // Lock the job if it's already ready
            if (P.IPC.IsCurrentJobAutoRotationReady())
            {
                locking = true;
                stringKeys = [];
            }
            // Get the list of combos and options to enable
            else
            {
                locking = false;
                stringKeys = combos.Select(k => k.ToString())
                    .Concat(registration.CombosControlled.Keys
                        .Select(k => k.ToString()).ToArray())
                    .ToArray();
            }

            // Register all combos
            foreach (var combo in combos)
                AddRegistrationForCombo(lease, combo, true, true);

            // Register all options
            foreach (var option in options)
            {
                if (stringKeys.Contains(option)) continue;

                // Enable the option, or lock the option to its current state
                var state = true;
                if (locking)
                {
                    var ccpOption = (CustomComboPreset)
                        Enum.Parse(typeof(CustomComboPreset), option);
                    state = CustomComboFunctions.IsEnabled(ccpOption);
                }

                AddRegistrationForOption(lease, option, state);
            }

            var logText =
                $"{registration.PluginName}: Registered Current Job ({job})";
            if (locking)
                logText += " (was already ready: locked it)";

            Logging.Log(logText);

            registration.LastUpdated = DateTime.Now;
            JobsUpdated = DateTime.Now;
            CombosUpdated = DateTime.Now;
            OptionsUpdated = DateTime.Now;
        });
    }

    /// <summary>
    ///     Removes a registration from the IPC service, cancelling the lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <param name="cancellationReason">
    ///     The <see cref="CancellationReasonEnum" /> for cancelling the lease.
    /// </param>
    /// <param name="additionalInfo">
    ///     Any additional information to log and provide with the cancellation.
    /// </param>
    /// <remarks>
    ///     Will call the <see cref="Lease.Callback" /> method if one was
    ///     provided.
    /// </remarks>
    internal void RemoveRegistration
    (Guid lease, CancellationReasonEnum cancellationReason,
        string additionalInfo = "")
    {
        if (cancellationReason == CancellationReasonEnum.WrathUserManuallyCancelled)
            _userRevokedTemporaryBlacklist.Add(
                lease,
                (Registrations[lease].InternalPluginName,
                    Registrations[lease].ConfigurationsHash,
                    DateTime.Now)
            );

        Registrations[lease].Cancel(cancellationReason, additionalInfo);
        Registrations.Remove(lease);

        // Bust the UI cache
        AutoRotationStateUpdated = DateTime.Now;
        AutoRotationConfigsUpdated = DateTime.Now;
        JobsUpdated = DateTime.Now;
        CombosUpdated = DateTime.Now;
        OptionsUpdated = DateTime.Now;
    }

    #endregion

    #region Fine-Grained Combo Methods

    /// <summary>
    ///     Checks if a combo is controlled by a lease.
    /// </summary>
    /// <param name="combo">The combo internal name to check.</param>
    /// <returns>
    ///     The <see cref="ComboStateKeys">states</see> the combo is controlled to,
    ///     or <c>null</c> if it is not.
    /// </returns>
    /// <seealso cref="Provider.GetComboState" />
    internal (bool enabled, bool autoMode)? CheckComboControlled(string combo)
    {
        CustomComboPreset customComboPreset;
        try
        {
            customComboPreset = (CustomComboPreset)
                Enum.Parse(typeof(CustomComboPreset), combo, true);
        }
        catch
        {
            return null;
        }

        var lease = Registrations.Values
            .Where(l => l.CombosControlled.ContainsKey(customComboPreset))
            .OrderByDescending(l => l.LastUpdated)
            .FirstOrDefault();

        return lease?.CombosControlled[customComboPreset];
    }

    /// <summary>
    ///     Adds a registration for a combo to a lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <param name="combo">The combo internal name to register control of.</param>
    /// <param name="newState">The state to set the preset to.</param>
    /// <param name="newAutoState">The state to set the Auto-Mode to.</param>
    /// <seealso cref="Provider.SetComboState" />
    internal void AddRegistrationForCombo
        (Guid lease, string combo, bool newState, bool newAutoState)
    {
        var registration = Registrations[lease];
        var preset = (CustomComboPreset)
            Enum.Parse(typeof(CustomComboPreset), combo, true);

        registration.CombosControlled[preset] = (newState, newAutoState);

        if (CheckBlacklist(Registrations[lease].ConfigurationsHash) &&
            Registrations[lease].SetsLeased > 4)
            RemoveRegistration(lease, CancellationReasonEnum.WrathUserManuallyCancelled,
                "Matched currently-blacklisted configuration");

        registration.LastUpdated = DateTime.Now;
        CombosUpdated = DateTime.Now;

        Logging.Log($"{registration.PluginName}: Registered Combo ({combo})");
    }

    /// <summary>
    ///     Checks if a combo option is controlled by a lease.
    /// </summary>
    /// <param name="option">The combo option internal name to check.</param>
    /// <returns>
    ///     The state the combo option is controlled to, or <c>null</c> if it is not.
    /// </returns>
    /// <seealso cref="Provider.GetComboOptionState" />
    internal bool? CheckComboOptionControlled(string option)
    {
        CustomComboPreset customComboPreset;
        try
        {
            customComboPreset = (CustomComboPreset)
                Enum.Parse(typeof(CustomComboPreset), option, true);
        }
        catch
        {
            return null;
        }

        var lease = Registrations.Values
            .Where(l => l.OptionsControlled.ContainsKey(customComboPreset))
            .OrderByDescending(l => l.LastUpdated)
            .FirstOrDefault();

        return lease?.OptionsControlled[customComboPreset];
    }

    /// <summary>
    ///     Adds a registration for a combo option to a lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <param name="option">The option internal name to register control of.</param>
    /// <param name="newState">The state to set the preset to.</param>
    /// <seealso cref="Provider.SetComboOptionState" />
    internal void AddRegistrationForOption
        (Guid lease, string option, bool newState)
    {
        var registration = Registrations[lease];
        var preset = (CustomComboPreset)
            Enum.Parse(typeof(CustomComboPreset), option, true);

        registration.OptionsControlled[preset] = newState;

        if (CheckBlacklist(Registrations[lease].ConfigurationsHash) &&
            Registrations[lease].SetsLeased > 4)
            RemoveRegistration(lease, CancellationReasonEnum.WrathUserManuallyCancelled,
                "Matched currently-blacklisted configuration");

        registration.LastUpdated = DateTime.Now;
        OptionsUpdated = DateTime.Now;

        Logging.Log($"{registration.PluginName}: Registered Option ({option})");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    ///     Checks if a lease exists.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <returns>Whether the lease exists.</returns>
    internal bool CheckLeaseExists(Guid lease) =>
        Registrations.ContainsKey(lease);

    /// <summary>
    ///     Suspend all leases. Called when IPC is disabled remotely.
    /// </summary>
    /// <param name="reason">
    ///     The <see cref="CancellationReasonEnum">reason</see> for suspending leases.
    /// </param>
    /// <seealso cref="Helper.IPCEnabled" />
    /// <seealso cref="RemoveRegistration" />
    internal void SuspendLeases(CancellationReasonEnum? reason = null)
    {
        var reasonToUse = reason ?? CancellationReasonEnum.AllServicesSuspended;

        Logging.Warn("Suspending all leases.");

        // dispose every lease in _registrations
        foreach (var registration in Registrations.Values)
            RemoveRegistration(
                registration.ID, reasonToUse
            );
    }

    #region Checking for plugin being unloaded

    private int _framesSinceLastCheck;

    private bool _checkingLeaseePluginsUnloaded;

    /// <summary>
    ///     Initializes the Leasing service, and registers leasee unloading checks.
    /// </summary>
    public Leasing()
    {
        Svc.Framework.Update += CheckIfLeaseePluginsUnloaded;
    }

    /// <summary>
    ///     Checks currently loaded plugins against leases.<br />
    ///     Will run every 500 frames and check if the leasees plugin is still
    ///     loaded.<br />
    ///     This method is registered to trigger off those events in the
    ///     <see cref="Leasing()">ctor</see>.
    /// </summary>
    private void CheckIfLeaseePluginsUnloaded(IFramework _)
    {
        if (_framesSinceLastCheck < 500 || _checkingLeaseePluginsUnloaded)
        {
            _framesSinceLastCheck++;
            return;
        }

        _checkingLeaseePluginsUnloaded = true;

        var plugins = Svc.PluginInterface
            .InstalledPlugins
            .Where(p => p.IsLoaded)
            .Select(p => p.InternalName).ToList();
        var leasesCopy = new Dictionary<Guid, Lease>(Registrations);

        foreach (var (lease, registration) in leasesCopy)
            if (!plugins.Contains(registration.InternalPluginName))
                RemoveRegistration(
                    lease, CancellationReasonEnum.LeaseePluginDisabled
                );

        _checkingLeaseePluginsUnloaded = false;
        _framesSinceLastCheck = 0;
    }

    #endregion

    #endregion

    #region Blacklist functionality

    /// <summary>
    ///     List of plugin names that have been revoked by the user.<br />
    ///     Trys to prevent a plugin from immediately re-registering after being
    ///     revoked by the user.
    /// </summary>
    /// <value>
    ///     <b>Key:</b> The former lease ID of the plugin.<br />
    ///     <b>Values:</b><br />
    ///     <b>Item1:</b> The internal plugin name.<br />
    ///     <b>Item2:</b> The <see cref="Lease.ConfigurationsHash" /> of the
    ///     previous lease.<br />
    ///     <b>Item3:</b> The time the lease was revoked.
    /// </value>
    /// <remarks>
    ///     The blacklisting is cleared after 2 minutes.
    /// </remarks>
    private readonly Dictionary<Guid, (string, byte[], DateTime)>
        _userRevokedTemporaryBlacklist = new();

    /// <summary>
    ///     Removes entries from the blacklist that are older than 2 minutes.
    /// </summary>
    private void CleanOutdatedBlacklistEntries()
    {
        var now = DateTime.Now;
        Dictionary<Guid, (string, byte[], DateTime)> blacklistCopy =
            new(_userRevokedTemporaryBlacklist);
        foreach (var (lease, (_, _, time)) in blacklistCopy)
            if (now - time > TimeSpan.FromMinutes(2))
                _userRevokedTemporaryBlacklist.Remove(lease);
    }

    /// <summary>
    ///     Checks if a lease was revoked by the user and is still blacklisted.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />.
    /// </param>
    /// <returns>If the lease is blacklisted.</returns>
    internal bool CheckBlacklist(Guid lease)
    {
        CleanOutdatedBlacklistEntries();

        return _userRevokedTemporaryBlacklist.ContainsKey(lease);
    }

    /// <summary>
    ///     Checks if a plugin name was revoked by the user and is still blacklisted.
    /// </summary>
    /// <param name="internalPluginName">
    ///     The internal name of the plugin that was revoked.
    /// </param>
    /// <returns>If the plugin's name is blacklisted.</returns>
    internal bool CheckBlacklist(string internalPluginName)
    {
        CleanOutdatedBlacklistEntries();

        return _userRevokedTemporaryBlacklist.Values
            .Any(entry => entry.Item1 == internalPluginName);
    }

    /// <summary>
    ///     Checks if a configuration hash revoked by the user and is still
    ///     blacklisted.<br />
    ///     The only blacklist check that can trigger after establishing a new lease.
    /// </summary>
    /// <param name="hash">
    ///     The configuration hash of the plugin that was revoked.
    /// </param>
    /// <returns>If the hash is blacklisted.</returns>
    internal bool CheckBlacklist(byte[] hash)
    {
        CleanOutdatedBlacklistEntries();

        return _userRevokedTemporaryBlacklist.Values
            .Any(entry => entry.Item2.SequenceEqual(hash));
    }

    #endregion
}
