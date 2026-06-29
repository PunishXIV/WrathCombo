#region

using ECommons;
using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;

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

    private object? GetTickService()
    {
        var tickServiceType = Plugin.GetType().Assembly.GetType("BossMod.Services.TickService");
        if (tickServiceType is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access TickServiceType");
            return null;
        }

        var host = Plugin.GetType().BaseType.GetProperty("Host").GetValue(Plugin);
        if (host is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access Host");
            return null;
        }

        var services = host.GetType().GetProperty("Services").GetValue(host);
        if (services is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access Services");
            return null;
        }

        var tickServiceValue = services.GetType().GetMethod("GetService").Invoke(services, [tickServiceType]);
        if (tickServiceValue is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access TickServiceValue");
            return null;
        }

        return tickServiceValue;
    }

    private IEnumerable<object?> GetModules(bool isReborn)
    {
        var entryPoint = isReborn ? Plugin : GetTickService();

        var rotationManager = entryPoint.GetFoP("_rotation");
        if (rotationManager is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access RotationManager");
            yield return null;
        }

        if (isReborn)
        {
            var preset = rotationManager.GetFoP("Preset");
            if (preset != null)
            {
                var modules = preset.GetFoP("Modules");
                var c = (int)modules.GetType().GetProperty("Count")!.GetValue(modules)!;
                for (int i = 0; i < c; i++)
                {
                    var item = modules.GetType().GetProperty("Item")!.GetValue(modules, new object[] { i });
                    yield return item;
                }
            }
        }

        var presets = isReborn ? rotationManager.GetFoP("ActiveModules") : rotationManager.GetFoP("Presets");
        if (presets is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access Presets");
            yield return null;
        }


        var count = (int)presets.GetType().GetProperty("Count")!.GetValue(presets)!;
        for (int i = 0; i < count; i++)
        {
            var item = presets.GetType().GetProperty("Item")!.GetValue(presets, new object[] { i });
            if (item is null)
                continue;

            var modules = isReborn ? item?.GetFoP("Module") : item?.GetFoP("Modules");
            if (modules is null)
            {
                continue;
            }
            if (isReborn)
            {
                yield return modules;
            }
            else
            {
                var modCount = (int)modules.GetType().GetProperty("Count").GetValue(modules)!;

                for (int p = 0; p < modCount; p++)
                {
                    var module = modules.GetType().GetProperty("Item")!.GetValue(modules, new object[] { p });
                    yield return module;
                }
            }
        }
    }

    private IEnumerable<object?> GetModuleTypes(bool isReborn)
    {
        foreach (var module in GetModules(isReborn))
        {
            var moduleType = module.GetType().GetProperty("Type").GetValue(module);
            yield return moduleType;
        }
    }

    private IEnumerable<object?> GetPresets(bool isReborn)
    {
        var entryPoint = isReborn ? Plugin : GetTickService();

        var rotationManager = entryPoint.GetFoP("_rotation");
        if (rotationManager is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access RotationManager");
            yield return null;
        }

        if (isReborn)
        {
            var preset = rotationManager.GetFoP("Preset");
            yield return preset;
        }

        var presets = isReborn ? rotationManager.GetFoP("ActiveModules") : rotationManager.GetFoP("Presets");
        if (presets is null)
        {
            PluginLog.Debug(
                    $"[ConflictingPlugins] [{PluginName}] Could not access Presets");
            yield return null;
        }


        var count = (int)presets.GetType().GetProperty("Count")!.GetValue(presets)!;
        for (int i = 0; i < count; i++)
        {
            var item = presets.GetType().GetProperty("Item")!.GetValue(presets, new object[] { i });
            yield return item;
        }
    }

    public bool IsAutoTargetingEnabled(bool isReborn)
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var presets = GetPresets(isReborn);
        foreach (var preset in presets)
        {
            if (preset.GetFoP<string>("Name") == "VBM Multibox")
                continue;

            var modules = isReborn ? preset?.GetFoP("Module") : preset?.GetFoP("Modules");
            if (modules is null)
            {
                continue;
            }
            if (isReborn)
            {
                var moduleType = modules.GetType().GetProperty("Type").GetValue(modules);
                if (moduleType is Type t && t.FullName.Contains("AutoTarget"))
                    return true;
            }
            else
            {
                var modCount = (int)modules.GetType().GetProperty("Count").GetValue(modules)!;

                for (int p = 0; p < modCount; p++)
                {
                    var module = modules.GetType().GetProperty("Item")!.GetValue(modules, new object[] { p });
                    var moduleType = module.GetType().GetProperty("Type").GetValue(module);
                    if (moduleType is Type t && t.FullName.Contains("AutoTarget"))
                        return true;
                }
            }
        }

        return false;
    }

    public bool IsUsingCustomQueuing()
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var tickServiceValue = GetTickService();

        var amex = tickServiceValue.GetFoP("_amex");
        if (amex == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx field");
            return false;
        }

        var manualQueue = amex.GetFoP("_manualQueue");
        if (manualQueue == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual field");
            return false;
        }

        var manualQueueConfig = manualQueue.GetFoP("_config");
        if (manualQueueConfig == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual.Config field");
            return false;
        }

        var customQueuingEnabled = manualQueueConfig.GetFoP<bool>("UseManualQueue");

        PluginLog.Verbose(
            $"[ConflictingPlugins] [{PluginName}] `ManualQueue.Enabled`: {customQueuingEnabled}");

        return customQueuingEnabled;
    }

    public bool IsUsingAutorotation(bool isReborn)
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var modules = GetModuleTypes(isReborn);
        foreach (var module in modules)
        {
            if (module is Type mt && !mt.FullName.Contains("MiscAI"))
                return true;
        }

        return false;
    }

    public bool HasAutomaticActionsQueuedReborn()
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

    public bool IsUsingCustomQueuingReborn()
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var amex = Plugin.GetFoP("_amex");
        if (amex == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx field");
            return false;
        }

        var manualQueue = amex.GetFoP("_manualQueue");
        if (manualQueue == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual field");
            return false;
        }

        var manualQueueConfig = manualQueue.GetFoP("_config");
        if (manualQueueConfig == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual.Config field");
            return false;
        }

        var customQueuingEnabled = manualQueueConfig.GetFoP<bool>("UseManualQueue");

        PluginLog.Verbose(
            $"[ConflictingPlugins] [{PluginName}] `ManualQueue.Enabled`: {customQueuingEnabled}");

        return customQueuingEnabled;
    }

    public bool IsAITargetingEnabled(bool isReborn)
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var entryPoint = isReborn ? Plugin : GetTickService();
        if (isReborn)
        {
            var ai = Plugin.GetFoP("_ai");
            var beh = ai.GetType().GetField("Beh").GetValue(ai);
            var enabled = beh is not null;

            if (!enabled)
                return false;

            var config = beh.GetFoP("_config");
            var target = config.GetFoP<bool>("ManualTarget");

            return !target;
        }
        else
        {
            var rotationManager = entryPoint.GetFoP("_rotation");
            if (rotationManager is null)
            {
                PluginLog.Debug(
                        $"[ConflictingPlugins] [{PluginName}] Could not access RotationManager");
                return false;
            }

            var ai = isReborn ? Plugin.GetFoP("_aiConfig") : rotationManager.GetFoP("_aiConfig");

            var enabled = ai.GetFoP<bool>("Enabled");
            var target = ai.GetFoP<bool>("ForbidActions");

            return enabled && !target;
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
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"`Configuration.LastModified` failed: " +
                              e.ToStringFull());
            return DateTime.MinValue;
        }
    }

    internal bool IsSmartTargetEnabled()
    {
        if (!PluginIsLoaded)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"Plugin is not loaded.");
            return false;
        }

        var amex = Plugin.GetFoP("_amex");
        if (amex == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx field");
            return false;
        }

        var manualQueue = amex.GetFoP("_manualQueue");
        if (manualQueue == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual field");
            return false;
        }

        var manualQueueConfig = manualQueue.GetFoP("_config");
        if (manualQueueConfig == null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual.Config field");
            return false;
        }

        var smartTargetEnabled = manualQueueConfig.GetFoP<bool>("SmartTargeting");

        PluginLog.Verbose(
            $"[ConflictingPlugins] [{PluginName}] `SmartTarget.Enabled`: {smartTargetEnabled}");

        return smartTargetEnabled;
    }

#pragma warning disable CS0649, CS8618 // Complaints of the method
    [EzIPC("BossMod.Rotation.ActionQueue.HasEntries", false)]
    private readonly Func<bool> _hasEntries = null!;

    [EzIPC("BossMod.Configuration.LastModified", false)]
    private readonly Func<DateTime> _lastModified = null!;
#pragma warning restore CS8618, CS0649
}