#region

using System;
using System.ComponentModel;
using WrathCombo.AutoRotation;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

#endregion

namespace WrathCombo.Services.IPC;

public static class BailMessages
{
    /// <summary>
    ///     The message to show when IPC services are disabled.
    /// </summary>
    public const string LiveDisabled =
        "IPC services are currently disabled.";

    /// <summary>
    ///     The message to show when the lease ID is not found.
    /// </summary>
    public const string InvalidLease =
        "Invalid lease.";

    /// <summary>
    ///     The message to show when the lease ID is blacklisted.
    /// </summary>
    public const string BlacklistedLease =
        "Blacklisted lease.";

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
public enum ComboStateKeys
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
public enum ComboTargetTypeKeys
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
    MultiTarget,

    /// <summary>
    ///     The key for conveying data about the Single-Target combo for a healer.
    /// </summary>
    HealST,

    /// <summary>
    ///     The key for conveying data about the Multi-Target combo for a healer.
    /// </summary>
    HealMT,

    /// <summary>
    ///     The key for a combo that is not specified as single or multi-target.
    /// </summary>
    Other,
}

public enum ComboSimplicityLevelKeys
{
    /// <summary>
    ///     The key for a simple combo.
    /// </summary>
    Simple,

    /// <summary>
    ///     The key for an Advanced combo.
    /// </summary>
    Advanced,

    /// <summary>
    ///     The key for a combo that is not simple nor advanced.
    /// </summary>
    Other,
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
                 "Please see the commit history for /res/ipc_status.txt.\n " +
                 "https://github.com/PunishXIV/WrathCombo/commits/main/res/ipc_status.txt")]
    AllServicesSuspended,
}

#region Auto-Rotation Configuration Options

/// <summary>
///     The subset of <see cref="AutoRotationConfig" /> options that can be set
///     via IPC.
/// </summary>
public enum AutoRotationConfigOption
{
    /// <seealso cref="AutoRotationConfig.InCombatOnly" />
    [ConfigValueType(typeof(bool))] InCombatOnly = 0,

    /// <seealso cref="AutoRotationConfig.DPSRotationMode" />
    [ConfigValueType(typeof(DPSRotationMode))]
    DPSRotationMode = 1,

    /// <seealso cref="AutoRotationConfig.HealerRotationMode" />
    [ConfigValueType(typeof(HealerRotationMode))]
    HealerRotationMode = 2,

    /// <seealso cref="DPSSettings.FATEPriority" />
    [ConfigValueType(typeof(bool))] FATEPriority = 3,

    /// <seealso cref="DPSSettings.QuestPriority" />
    [ConfigValueType(typeof(bool))] QuestPriority = 4,

    /// <seealso cref="HealerSettings.SingleTargetHPP" />
    [ConfigValueType(typeof(int))] SingleTargetHPP = 5,

    /// <seealso cref="HealerSettings.AoETargetHPP" />
    [ConfigValueType(typeof(int))] AoETargetHPP = 6,

    /// <seealso cref="HealerSettings.SingleTargetRegenHPP" />
    [ConfigValueType(typeof(int))] SingleTargetRegenHPP = 7,

    /// <seealso cref="HealerSettings.ManageKardia" />
    [ConfigValueType(typeof(bool))] ManageKardia = 8,

    /// <seealso cref="HealerSettings.AutoRez" />
    [ConfigValueType(typeof(bool))] AutoRez = 9,

    /// <seealso cref="HealerSettings.AutoRezDPSJobs" />
    [ConfigValueType(typeof(bool))] AutoRezDPSJobs = 10,

    /// <seealso cref="HealerSettings.AutoCleanse" />
    [ConfigValueType(typeof(bool))] AutoCleanse = 11,

    /// <seealso cref="HealerSettings.IncludeNPCs"/>
    [ConfigValueType(typeof(bool))] IncludeNPCs = 12,

    /// <seealso cref="DPSSettings.OnlyAttackInCombat" />
    [ConfigValueType(typeof(bool))] OnlyAttackInCombat = 13,
}

#region Type Attribute

/// <summary>
///     Attribute to define the type of value that should be set for a given
///     <see cref="AutoRotationConfigOption" />.
/// </summary>
/// <param name="valueType">The type necessary.</param>
[AttributeUsage(AttributeTargets.Field)]
internal sealed class ConfigValueTypeAttribute(Type valueType) : Attribute
{
    public Type ValueType { get; } = valueType;
}

#endregion

#endregion
