#region

using System;
using System.Linq;
using WrathCombo.API.Enum;

#endregion

namespace WrathCombo.Services.IPC;

public partial class Leasing
{
    /// <summary>
    ///     Checks if Auto-Rotation's state is controlled by a lease.
    /// </summary>
    /// <param name="option">
    ///     The Auto-Rotation configuration option to check.
    /// </param>
    /// <returns>
    ///     The state the Auto-Rotation configuration is controlled to, or
    ///     <c>null</c> if it is not.
    /// </returns>
    /// <seealso cref="Provider.GetAutoRotationConfigState" />
    internal int? CheckAutoRotationConfigControlled
        (AutoRotationConfigOption option)
    {
        var lease = Registrations.Values
            .Where(l => l.AutoRotationConfigsControlled.ContainsKey(option))
            .OrderByDescending(l => l.LastUpdated)
            .FirstOrDefault();

        return lease?.AutoRotationConfigsControlled[option];
    }

    /// <summary>
    ///     Adds a registration for Auto-Rotation Config control to a lease.
    /// </summary>
    /// <param name="lease">
    ///     Your lease ID from <see cref="Provider.RegisterForLease(string,string)" />
    /// </param>
    /// <param name="option">The Auto-Rotation option to set.</param>
    /// <param name="value">The type-juggled value to set it to.</param>
    /// <seealso cref="Provider.SetAutoRotationConfigState" />
    internal SetResult AddRegistrationForAutoRotationConfig
        (Guid lease, AutoRotationConfigOption option, int value)
    {
        var registration = Registrations[lease];

        if (registration.AutoRotationConfigsControlled.ContainsKey(option) &&
            registration.AutoRotationConfigsControlled[option] == value)
            return SetResult.Duplicate;

        registration.AutoRotationConfigsControlled[option] = value;

        registration.LastUpdated = DateTime.Now;
        AutoRotationConfigsUpdated = DateTime.Now;

        Logging.Log($"{registration.PluginName}: Registered Auto-Rotation Config ({option} to {value})");
        return SetResult.Okay;
    }
}