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

    private bool CallIpc(Func<bool> ipc, string name)
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] " +
                            $"IPC is not enabled.");
            return false;
        }

        try
        {
            var result = ipc();
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{PluginName}] `{name}`: {result}");
            return result;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] " +
                              $"`{name}` failed: " +
                              e.ToStringFull());
            return false;
        }
    }

    private object? GetRebornRotationManager()
    {
        var rotationManager = Plugin.GetFoP("_rotation");
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

    private IEnumerable<object?> GetRebornModules()
    {
        var rotationManager = GetRebornRotationManager();
        if (rotationManager is null)
            yield break;

        var preset = rotationManager.GetFoP("Preset");
        var presetModules = preset?.GetFoP("Modules");
        if (presetModules is not null)
            foreach (var module in GetIndexedItems(presetModules))
                yield return module;

        var presets = rotationManager.GetFoP("ActiveModules");
        if (presets is null)
        {
            PluginLog.Debug(
                $"[ConflictingPlugins] [{PluginName}] Could not access Presets");
            yield break;
        }

        foreach (var item in GetIndexedItems(presets))
        {
            var module = item?.GetFoP("Module");
            if (module is not null)
                yield return module;
        }
    }

    private IEnumerable<object?> GetRebornModuleTypes()
    {
        foreach (var module in GetRebornModules())
        {
            if (module is null)
                continue;

            var moduleType = module.GetType().GetProperty("Type")?.GetValue(module);
            yield return moduleType;
        }
    }

    private IEnumerable<object?> GetRebornPresets()
    {
        var rotationManager = GetRebornRotationManager();
        if (rotationManager is null)
            yield break;

        yield return rotationManager.GetFoP("Preset");

        var presets = rotationManager.GetFoP("ActiveModules");
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
        if (!isReborn)
            return CallIpc(_isAutoTargetActive, "Rotation.IsAutoTargetActive");

        if (!IsPluginLoaded())
            return false;

        foreach (var preset in GetRebornPresets())
        {
            if (preset is null || preset.GetFoP<string>("Name") == "VBM Multibox")
                continue;

            if (IsAutoTargetModule(preset.GetFoP("Module")))
                return true;
        }

        return false;
    }

    public bool IsUsingCustomQueuing() =>
        CallIpc(_isManualQueueEnabled, "Rotation.ActionTweaks.IsManualQueueEnabled");

    public bool IsUsingAutorotation(bool isReborn)
    {
        if (!isReborn)
            return CallIpc(_isAutorotationActive, "Rotation.IsAutorotationActive");

        if (!IsPluginLoaded())
            return false;

        var modules = GetRebornModuleTypes();
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
        if (!isReborn)
            return CallIpc(_isAiAutoTargetEnabled, "AI.IsAutoTargetEnabled");

        if (!IsPluginLoaded())
            return false;

        var ai = Plugin.GetFoP("_ai");
        var beh = ai.GetType().GetField("Beh").GetValue(ai);
        if (beh is null)
            return false;

        var config = beh.GetFoP("_config");
        var target = config.GetFoP<bool>("ManualTarget");

        return !target;
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

    internal bool IsSmartTargetEnabled(bool isReborn)
    {
        if (!isReborn)
            return CallIpc(_isSmartTargetingEnabled, "Rotation.ActionTweaks.IsSmartTargetingEnabled");

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

    [EzIPC("BossMod.Rotation.IsAutoTargetActive", false)]
    private readonly Func<bool> _isAutoTargetActive = null!;

    [EzIPC("BossMod.Rotation.IsAutorotationActive", false)]
    private readonly Func<bool> _isAutorotationActive = null!;

    [EzIPC("BossMod.Rotation.ActionTweaks.IsManualQueueEnabled", false)]
    private readonly Func<bool> _isManualQueueEnabled = null!;

    [EzIPC("BossMod.Rotation.ActionTweaks.IsSmartTargetingEnabled", false)]
    private readonly Func<bool> _isSmartTargetingEnabled = null!;

    [EzIPC("BossMod.AI.IsAutoTargetEnabled", false)]
    private readonly Func<bool> _isAiAutoTargetEnabled = null!;
#pragma warning restore CS8618, CS0649
}