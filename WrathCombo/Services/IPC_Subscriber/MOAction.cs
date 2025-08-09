#region

using ECommons;
using ECommons.EzIpcManager;
using ECommons.Logging;
using System;

// ReSharper disable InlineTemporaryVariable

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

internal sealed class MOActionIPC(
    string? pluginName = null,
    Version? validVersion = null)
    : ReusableIPC(pluginName ?? "MOAction", validVersion ?? new Version(4, 7, 1, 0))
{
#pragma warning disable CS0649, CS8618 // Complaints of the method
    [EzIPC("RetargetedActions")]
    private readonly Func<uint[]> _retargetedActions = null!;
#pragma warning restore CS8618, CS0649

    public uint[] GetRetargetedActions()
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] is not enabled.");
            return [];
        }

        try
        {
            var actions = _retargetedActions();
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{PluginName}] Has `RetargetedActions`: " +
                (actions.Length > 0));
            return actions;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"`RetargetedActions` failed: {e.ToStringFull()}");
            return [];
        }
    }
}