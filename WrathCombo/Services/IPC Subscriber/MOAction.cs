#region

using System;
using System.Linq;
using ECommons;
using ECommons.EzIpcManager;
using ECommons.Logging;
using WrathCombo.Extensions;

// ReSharper disable InlineTemporaryVariable

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

internal sealed class MOAction(
    string? pluginName = null,
    Version? validVersion = null)
    : ReusableIPC(pluginName ?? "MOAction", validVersion ?? new Version(4, 7, 0, 1))
{
#pragma warning disable CS0649, CS8618 // Complaints of the method
    [EzIPC("RetargetedActions")] private readonly Func<uint[]> _retargetedActions = null!;
#pragma warning restore CS8618, CS0649

    public uint[] GetRetargetedActions()
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[{PluginName}] {PluginName} is not enabled.");
            return [];
        }

        try
        {
            var actions = _retargetedActions();
            PluginLog.Verbose(
                $"[{PluginName}] `RetargetedActions`: {string.Join(", ", actions.Select(x => x.ActionName()))}");
            return actions;
        }
        catch (Exception e)
        {
            e.Log();
            return [];
        }
    }
}