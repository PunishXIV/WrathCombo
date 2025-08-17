#region

using ECommons;
using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections;
using System.Linq;

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

internal sealed class RedirectIPC(
    string? pluginName = null,
    Version? validVersion = null)
    : ReusableIPC(pluginName ?? "Redirect",
        validVersion ?? new Version(1, 2, 5, 0),
        true)
{
    private object Configuration =>
        Plugin.GetFoP("Configuration");

    private bool AutoMouseoverBeneficial =>
        Configuration.GetFoP<bool>("DefaultMouseoverFriendly");

    private bool AutoMouseoverGround =>
        Configuration.GetFoP<bool>("DefaultMouseoverGround");

    private uint[] Redirections =>
        (Configuration.GetFoP("Redirections") as IDictionary)?
        .Keys.Cast<uint>().ToArray() ?? [];

    public uint[] GetRetargetedActions()
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] is not enabled.");
            return [];
        }

        try
        {
            var actions = Redirections;
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{PluginName}] Has `Redirections`: " +
                (actions.Length > 0));
            return actions;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"Getting `Redirections` failed: {e.ToStringFull()}");
            return [];
        }
    }

    public bool AreGroundTargetedActionsRedirected()
    {
        try
        {
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{PluginName}] " +
                $"Has `AutoMouseoverGround`: {AutoMouseoverGround}");

            return AutoMouseoverGround;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"Checking `AutoMouseoverGround` failed: " +
                              $"{e.ToStringFull()}");
            return false;
        }
    }

    public bool AreBeneficialActionsRedirected()
    {
        try
        {
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{PluginName}] " +
                $"Has `AutoMouseoverBeneficial`: {AutoMouseoverBeneficial}");

            return AutoMouseoverBeneficial;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"Checking `AutoMouseoverBeneficial` failed: " +
                              $"{e.ToStringFull()}");
            return false;
        }
    }
}