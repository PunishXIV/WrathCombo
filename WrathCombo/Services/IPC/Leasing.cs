#region

// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ECommons.ExcelServices;
using WrathCombo.Combos;
using CancellationReasonEnum = WrathCombo.Services.IPC.CancellationReason;

// ReSharper disable UseSymbolAlias
// ReSharper disable UnusedMember.Global

#endregion

namespace WrathCombo.Services.IPC;

public class Lease(
    string pluginName,
    Action<CancellationReason, string>? callback)
{
    public Guid ID { get; } = Guid.NewGuid();
    public string PluginName { get; } = pluginName;
    public Action<CancellationReason, string>? Callback { get; } = callback;

    private DateTime Created { get; } = DateTime.Now;
    internal DateTime LastUpdated { get; } = DateTime.Now;

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
    ///     Maximum is <c>40</c>.
    /// </summary>
    /// <seealso cref="Provider.RegisterForLease" />
    /// <seealso cref="Leasing.MaxLeases" />
    public int SetsLeased =>
        AutoRotationControlled.Count +
        JobsControlled.Count * 6 +
        CombosControlled.Count * 2 +
        OptionsControlled.Count;

    internal Dictionary<byte, bool> AutoRotationControlled { get; set; } = new();

    internal Dictionary<AutoRotationConfigOption, int> AutoRotationConfigsControlled
    {
        get;
        set;
    } = new();

    internal Dictionary<Job, bool> JobsControlled { get; set; } = new();

    internal Dictionary<CustomComboPreset, bool> CombosControlled { get; set; } =
        new();

    internal Dictionary<CustomComboPreset, bool> OptionsControlled { get; set; } =
        new();

    /// <summary>
    ///     Cancels the lease, invoking the <see cref="Callback" /> if one was
    ///     provided.
    /// </summary>
    /// <param name="cancellationReason">
    ///     The <see cref="CancellationReason" /> for cancelling the lease.
    /// </param>
    /// <param name="additionalInfo">
    ///     Any additional information to provide with the cancellation.
    /// </param>
    /// <remarks>
    ///     Usually called by <see cref="Leasing.RemoveRegistration" />,
    ///     which is often called by <see cref="Provider.ReleaseControl" />.
    /// </remarks>
    public void Cancel
        (CancellationReason cancellationReason, string additionalInfo = "")
    {
        Logging.Log(
            "Cancelling Lease for: "
            + PluginName
            + " (" + cancellationReason + ")" +
            (additionalInfo != ""
                ? "\n" + additionalInfo
                : "")
        );
        Callback?.Invoke(cancellationReason, additionalInfo);
    }
}

public partial class Leasing
{
    /// <summary>
    ///     The number of sets allowed per lease.
    /// </summary>
    /// <seealso cref="Provider.RegisterForLease" />
    /// <seealso cref="CheckLeaseConfigurationsAvailable" />
    /// <seealso cref="Lease.SetsLeased" />
    internal const int MaxLeases = 40;

    internal Dictionary<Guid, Lease> Registrations = new();

    internal Guid? CreateRegistration(string pluginName, Action callback)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Checks if a lease exists.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease" />
    /// </param>
    /// <returns>Whether the lease exists.</returns>
    internal bool CheckLeaseExists(Guid lease) =>
        Registrations.ContainsKey(lease);

    /// <summary>
    ///     Checks how many sets are still available for a lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease" />
    /// </param>
    /// <returns>
    ///     The number of sets available for the lease, or <c>null</c> if the lease
    ///     does not exist.
    /// </returns>
    /// <seealso cref="MaxLeases" />
    internal int? CheckLeaseConfigurationsAvailable(Guid lease) =>
        Registrations.TryGetValue(lease, out var value)
            ? MaxLeases - value.SetsLeased
            : null;

    internal void AddRegistrationForCurrentJob(Guid lease)
    {
        throw new NotImplementedException();
    }

    internal void AddRegistrationForAutoRotation(Guid lease, bool newState)
    {
        throw new NotImplementedException();
    }

    internal void AddRegistrationForCombo
        (Guid lease, string combo, bool newState, bool newAutoState)
    {
        throw new NotImplementedException();
    }

    internal void AddRegistrationForOption
        (Guid lease, string combo, bool newState)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Removes a registration from the IPC service, cancelling the lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease" />
    /// </param>
    /// <param name="cancellationReason">
    ///     The <see cref="CancellationReason" /> for cancelling the lease.
    /// </param>
    /// <param name="additionalInfo">
    ///     Any additional information to log and provide with the cancellation.
    /// </param>
    /// <remarks>
    ///     Will call the <see cref="Lease.Callback" /> method if one was
    ///     provided.
    /// </remarks>
    internal void RemoveRegistration
    (Guid lease, CancellationReason cancellationReason,
        string additionalInfo = "")
    {
        Registrations[lease].Cancel(cancellationReason, additionalInfo);
        Registrations.Remove(lease);
    }

    internal string? CheckJobControlled()
    {
        throw new NotImplementedException();
    }

    internal string? CheckAutoRotationControlled()
    {
        throw new NotImplementedException();
    }

    internal string? CheckComboControlled(string combo)
    {
        throw new NotImplementedException();
    }

    internal string? CheckOptionControlled(string option)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Suspend all leases. Called when IPC is disabled remotely.
    /// </summary>
    /// <seealso cref="IPCEnabled" />
    /// <seealso cref="RemoveRegistration" />
    internal void SuspendLeases()
    {
        Logging.Warn(
            "IPC has been disabled remotely.\n" +
            "Suspending all leases."
        );

        // dispose every lease in _registrations
        foreach (var registration in Registrations.Values)
            RemoveRegistration(
                registration.ID, CancellationReason.AllServicesSuspended
            );
    }

    #region Blacklist functionality

    /// <summary>
    ///     List of plugin names that have been revoked by the user.<br />
    ///     Trys to prevent a plugin from immediately re-registering after being
    ///     revoked by the user.
    /// </summary>
    /// <value>
    ///     <b>Key:</b> The former lease ID of the plugin.<br />
    ///     <b>Values:</b><br />
    ///     <b>Item1:</b> The plugin name.<br />
    ///     <b>Item2:</b> The <see cref="Lease.ConfigurationsHash" /> of the
    ///     previous lease.<br />
    ///     <b>Item3:</b> The time the lease was revoked.
    /// </value>
    /// <remarks>
    ///     The blacklisting is cleared after 5 minutes.
    /// </remarks>
    private Dictionary<Guid, (string, byte[], DateTime)>
        _userRevokedTemporaryBlacklist = new();

    /// <summary>
    ///     Removes entries from the blacklist that are older than 5 minutes.
    /// </summary>
    private void CleanOutdatedBlacklistEntries()
    {
        var now = DateTime.Now;
        Dictionary<Guid, (string, byte[], DateTime)> blacklistCopy =
            new(_userRevokedTemporaryBlacklist);
        foreach (var (lease, (_, _, time)) in blacklistCopy)
            if (now - time > TimeSpan.FromMinutes(5))
                _userRevokedTemporaryBlacklist.Remove(lease);
    }

    /// <summary>
    ///     Checks if a lease was revoked by the user and is still blacklisted.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease" />.
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
    /// <param name="pluginName">The name of the plugin that was revoked.</param>
    /// <returns>If the plugin's name is blacklisted.</returns>
    internal bool CheckBlacklist(string pluginName)
    {
        CleanOutdatedBlacklistEntries();

        return _userRevokedTemporaryBlacklist.Values
            .Any(entry => entry.Item1 == pluginName);
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
