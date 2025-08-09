#region

using ECommons;
using ECommons.EzIpcManager;
using ECommons.Logging;
using ECommons.Reflection;
using System;

// ReSharper disable InlineTemporaryVariable

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

internal sealed class BossModIPC(
    string pluginName,
    Version validVersion)
    : ReusableIPC(pluginName, validVersion)
{
    public bool HasAutomaticActionsQueued()
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"IPC is not enabled.");
            return false;
        }

        try
        {
            var hasEntries = _hasEntries();
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{PluginName}] `ActionQueue.HasEntries`: " +
                hasEntries);
            return hasEntries;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"`ActionQueue.HasEntries` failed:" +
                              e.ToStringFull());
            return false;
        }
    }

    public bool IsAutoTargetingEnabled()
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var ai = Plugin.GetFoP("_ai");
        if (ai == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access _ai field");
            return false;
        }

        var aiConfig = ai.GetFoP("Config");
        if (aiConfig == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AI.Config field");
            return false;
        }

        var aiEnabled = aiConfig.GetFoP<bool>("Enabled");
        var aiDisableTargeting = aiConfig.GetFoP<bool>("ForbidActions");

        PluginLog.Verbose(
            $"[ConflictingPlugins] [{PluginName}] `AI.Enabled`: {aiEnabled}, " +
            $"`AI.DisableTargeting`: {aiDisableTargeting}");

        return aiEnabled && aiDisableTargeting != true;
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
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"`Configuration.LastModified` failed: " +
                              e.ToStringFull());
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