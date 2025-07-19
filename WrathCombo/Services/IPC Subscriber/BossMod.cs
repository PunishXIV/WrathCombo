#region

using System;
using ECommons;
using ECommons.EzIpcManager;
using ECommons.Logging;

// ReSharper disable InlineTemporaryVariable

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

internal sealed class BossModIPC(
    string? pluginName = null,
    Version? validVersion = null)
    : ReusableIPC(pluginName ?? "BossMod", validVersion ?? new Version(0, 3, 1, 0))
{
    public bool HasAutomaticActionsQueued()
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[{PluginName}] {PluginName} is not enabled.");
            return false;
        }

        try
        {
            var hasEntries = _hasEntries();
            PluginLog.Verbose(
                $"[{PluginName}] `ActionQueue.HasEntries`: {hasEntries} ");
            return hasEntries;
        }
        catch (Exception e)
        {
            e.Log();
            return false;
        }
    }

    public DateTime LastModified()
    {
        if (!IsEnabled) return DateTime.MinValue;

        try
        {
            return _lastModified();
        }
        catch (Exception e)
        {
            e.Log();
            return DateTime.MinValue;
        }
    }

#pragma warning disable CS0649, CS8618 // Complaints of the method
    [EzIPC("Rotation.ActionQueue.HasEntries")]
    private readonly Func<bool> _hasEntries = null!;

    [EzIPC("Configuration.LastModified")]
    private readonly Func<DateTime> _lastModified = null!;
#pragma warning restore CS8618, CS0649
}