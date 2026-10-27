#region

using Dalamud;
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
    private bool IsPluginLoaded()
    {
        if (PluginIsLoaded)
            return true;

        PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                        $"Plugin is not loaded.");
        return false;
    }

    private bool HasAutomaticActionsQueuedCore()
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

    private object? GetRotationManager(bool isReborn)
    {
        var entryPoint = isReborn ? Plugin : GetTickService();
        var wndRotation = entryPoint?.GetFoP("_wndRotation");
        var rotationManager = isReborn
            ? entryPoint?.GetFoP("_rotation")
            : wndRotation?.GetFoP("_mgr");

        if (rotationManager is not null)
            return rotationManager;

        PluginLog.Debug(
            $"[ConflictingPlugins] [{PluginName}] Could not access RotationManager");
        return null;
    }

    private static IEnumerable<object?> GetIndexedItems(object collection)
    {
        var collectionType = collection.GetType();
        var count = (int)collectionType.GetProperty("Count")!.GetValue(collection)!;
        var itemProperty = collectionType.GetProperty("Item")!;

        for (var index = 0; index < count; index++)
            yield return itemProperty.GetValue(collection, [index]);
    }

    private object? GetManualQueueConfig(object? amex)
    {
        if (amex is null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx field");
            return null;
        }

        var manualQueue = amex.GetFoP("_manualQueue");
        if (manualQueue is null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual field");
            return null;
        }

        var manualQueueConfig = manualQueue.GetFoP("_config");
        if (manualQueueConfig is null)
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access AMEx.Manual.Config field");

        return manualQueueConfig;
    }

    private bool IsManualQueueSettingEnabled(
        object? amex,
        string settingName,
        string settingLogName)
    {
        var manualQueueConfig = GetManualQueueConfig(amex);
        if (manualQueueConfig is null)
            return false;

        var enabled = manualQueueConfig.GetFoP<bool>(settingName);
        PluginLog.Verbose(
            $"[ConflictingPlugins] [{PluginName}] `{settingLogName}`: {enabled}");
        return enabled;
    }

    public bool HasAutomaticActionsQueued() => HasAutomaticActionsQueuedCore();

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
        var rotationManager = GetRotationManager(isReborn);
        if (rotationManager is null)
            yield break;

        if (isReborn)
        {
            var preset = rotationManager.GetFoP("Preset");
            var modules = preset?.GetFoP("Modules");
            if (modules is not null)
                foreach (var module in GetIndexedItems(modules))
                    yield return module;
        }

        var presets = rotationManager.GetFoP(isReborn ? "ActiveModules" : "Presets");
        if (presets is null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access Presets");
            yield break;
        }

        foreach (var item in GetIndexedItems(presets))
        {
            if (item is null)
                continue;

            var modules = item.GetFoP(isReborn ? "Module" : "Modules");
            if (modules is null)
                continue;

            if (isReborn)
            {
                yield return modules;
                continue;
            }

            foreach (var module in GetIndexedItems(modules))
                yield return module;
        }
    }

    private IEnumerable<object?> GetModuleTypes(bool isReborn)
    {
        foreach (var module in GetModules(isReborn))
        {
            if (module is null)
                continue;

            var moduleType = module.GetType().GetProperty("Type")?.GetValue(module);
            yield return moduleType;
        }
    }

    private IEnumerable<object?> GetPresets(bool isReborn)
    {
        var rotationManager = GetRotationManager(isReborn);
        if (rotationManager is null)
            yield break;

        if (isReborn)
            yield return rotationManager.GetFoP("Preset");

        var presets = rotationManager.GetFoP(isReborn ? "ActiveModules" : "Presets");
        if (presets is null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access Presets");
            yield break;
        }

        foreach (var preset in GetIndexedItems(presets))
            yield return preset;
    }

    private static bool IsAutoTargetModule(object? module)
    {
        var moduleType = module?.GetType().GetProperty("Type")?.GetValue(module);
        return moduleType is Type type && type.FullName.Contains("AutoTarget");
    }

    public bool IsAutoTargetingEnabled(bool isReborn)
    {
        if (!IsPluginLoaded())
            return false;

        foreach (var preset in GetPresets(isReborn))
        {
            if (preset is null || preset.GetFoP<string>("Name") == "VBM Multibox")
                continue;

            var modules = preset.GetFoP(isReborn ? "Module" : "Modules");
            if (modules is null)
                continue;

            if (isReborn)
            {
                if (IsAutoTargetModule(modules))
                    return true;
            }
            else
            {
                foreach (var module in GetIndexedItems(modules))
                    if (IsAutoTargetModule(module))
                        return true;
            }
        }

        return false;
    }

    public bool IsUsingCustomQueuing()
    {
        if (!IsPluginLoaded())
            return false;

        var wndRotation = GetTickService()?.GetFoP("_wndRotation");
        var amex = wndRotation?.GetFoP("_amex");
        return IsManualQueueSettingEnabled(amex, "UseManualQueue", "ManualQueue.Enabled");
    }

    public bool IsUsingAutorotation(bool isReborn)
    {
        if (!IsPluginLoaded())
            return false;

        var modules = GetModuleTypes(isReborn);
        foreach (var module in modules)
        {
            if (module is Type mt && !mt.FullName.Contains("MiscAI"))
                return true;
        }

        return false;
    }

    public bool HasAutomaticActionsQueuedReborn() => HasAutomaticActionsQueuedCore();

    public bool IsUsingCustomQueuingReborn()
    {
        if (!IsPluginLoaded())
            return false;

        return IsManualQueueSettingEnabled(
            Plugin.GetFoP("_amex"), "UseManualQueue", "ManualQueue.Enabled");
    }

    public bool IsAITargetingEnabled(bool isReborn)
    {
        if (!IsPluginLoaded())
            return false;

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
            var wndRotation = entryPoint.GetFoP("_wndRotation");

            var rotationManager = wndRotation?.GetFoP("_mgr");
            if (rotationManager is null)
            {
                PluginLog.Debug(
                        $"[ConflictingPlugins] [{PluginName}] Could not access RotationManager");
                return false;
            }

            var ai = rotationManager.GetFoP("_aiConfig");

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
        if (!IsPluginLoaded())
            return false;

        return IsManualQueueSettingEnabled(
            Plugin.GetFoP("_amex"), "SmartTargeting", "SmartTarget.Enabled");
    }

#pragma warning disable CS0649, CS8618 // Complaints of the method
    [EzIPC("BossMod.Rotation.ActionQueue.HasEntries", false)]
    private readonly Func<bool> _hasEntries = null!;

    [EzIPC("BossMod.Configuration.LastModified", false)]
    private readonly Func<DateTime> _lastModified = null!;
#pragma warning restore CS8618, CS0649
}