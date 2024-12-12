using System.ComponentModel;

namespace WrathCombo.Services.IPC;

public static class BailMessages
{
    /// <summary>
    ///     The message to show when IPC services are disabled.
    /// </summary>
    public const string LiveDisabled = "IPC services are currently disabled.";

    /// <summary>
    ///     The message to show when the lease ID is not found.
    /// </summary>
    public const string InvalidLease = "Invalid lease.";

    /// <summary>
    ///     The message to show when the lease ID does not have the available
    ///     configurations.
    /// </summary>
    public const string NotEnoughConfigurations =
        "Not enough configurations available.";
}

#region Standardized Dictionary Keys

/// <summary>
///     The keys for the states of a combo, enabled and enabled in Auto-Mode.
/// </summary>
public enum ComboState
{
    /// <summary>
    ///     The key for whether a combo is enabled.
    /// </summary>
    Enabled,

    /// <summary>
    ///     The key for whether a combo is enabled in Auto-Mode.
    /// </summary>
    AutoMode
}

/// <summary>
///     The keys for the types of target a combo is designed for, Single-Target or
///     Multi-Target.
/// </summary>
public enum ComboTargetType
{
    /// <summary>
    ///     The key for conveying data about the Single-Target portion of a job
    ///     configuration.
    /// </summary>
    SingleTarget,

    /// <summary>
    ///     The key for conveying data about the Multi-Target portion of a job
    ///     configuration.
    /// </summary>
    MultiTarget
}

#endregion

/// <summary>
///     Why a lease was cancelled.
/// </summary>
public enum CancellationReason
{
    [Description("The Wrath user manually elected to revoke your lease.")]
    WrathUserManuallyCancelled,

    [Description("Your plugin was detected as having been disabled, " +
                 "not that you're likely to see this.")]
    LeaseePluginDisabled,

    [Description("The Wrath plugin is being disabled.")]
    WrathPluginDisabled,

    [Description("Your lease was released by IPC call, " +
                 "theoretically this was done by you.")]
    LeaseeReleased,

    [Description("IPC Services have been disabled remotely. " +
                 "Please see the commit history for /res/ipc_status.txt. \n " +
                 "https://github.com/PunishXIV/WrathCombo/commits/main/res/ipc_status.txt")]
    AllServicesSuspended,
}
