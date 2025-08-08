#region

using System;
using System.Collections.Generic;
using Dalamud.Plugin.Ipc;
using ECommons.DalamudServices;
using ECommons.Logging;

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

internal sealed class PandorasBoxIPC(
    string? pluginName = null,
    Version? validVersion = null)
    : ReusableIPC(pluginName ?? "PandorasBox", validVersion ?? new Version(1, 6, 3, 13))
{
    private ICallGateSubscriber<string, bool>? _getFeatureEnabled;
    private ICallGateSubscriber<string, string, bool>? _getConfigEnabled;

    public bool GetFeatureEnabled(string featureName)
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] is not enabled.");
            return false;
        }

        try
        {
            _getFeatureEnabled ??= Svc.PluginInterface.GetIpcSubscriber<string, bool>("PandorasBox.GetFeatureEnabled");
            var enabled = _getFeatureEnabled.InvokeFunc(featureName);
            PluginLog.Verbose($"[ConflictingPlugins] [{PluginName}] Feature '{featureName}' enabled: {enabled}");
            return enabled;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] GetFeatureEnabled failed: {e.Message}");
            return false;
        }
    }

    public bool GetConfigEnabled(string featureName, string configProperty)
    {
        if (!IsEnabled)
        {
            PluginLog.Debug($"[ConflictingPlugins] [{PluginName}] is not enabled.");
            return false;
        }

        try
        {
            _getConfigEnabled ??= Svc.PluginInterface.GetIpcSubscriber<string, string, bool>("PandorasBox.GetConfigEnabled");
            var enabled = _getConfigEnabled.InvokeFunc(featureName, configProperty);
            PluginLog.Verbose($"[ConflictingPlugins] [{PluginName}] Config '{featureName}.{configProperty}' enabled: {enabled}");
            return enabled;
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] GetConfigEnabled failed: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets all enabled action-related features that could conflict with WrathCombo
    /// </summary>
    public List<string> GetEnabledActionConflicts()
    {
        var conflicts = new List<string>();
        if (!IsEnabled) return conflicts;

        try
        {
            // Check for features that directly use ActionManager->UseAction() which could interfere
            string[] actionFeatures = 
            [
                "Auto Tank Stance",
                "Auto Peloton",
                "Auto Sprint",
                "Auto Fairy",
                "Auto Mount Combat",
                "Auto Cordial",
                "Auto Collect"
            ];

            foreach (var feature in actionFeatures)
            {
                if (GetFeatureEnabled(feature))
                    conflicts.Add(feature);
            }
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] GetEnabledActionConflicts failed: {e.Message}");
        }

        return conflicts;
    }

    /// <summary>
    /// Gets all enabled targeting-related features that could conflict with WrathCombo
    /// </summary>
    public List<string> GetEnabledTargetingConflicts()
    {
        var conflicts = new List<string>();
        if (!IsEnabled) return conflicts;

        try
        {
            // Check for targeting-related features that could interfere with WrathCombo's targeting
            string[] targetingFeatures = 
            [
                "Fate Targeting Mode",
                "Action Combat Targeting",
                "Auto Target",
                "Smart Targets"
            ];

            foreach (var feature in targetingFeatures)
            {
                if (GetFeatureEnabled(feature))
                    conflicts.Add(feature);
            }
        }
        catch (Exception e)
        {
            PluginLog.Warning($"[ConflictingPlugins] [{PluginName}] GetEnabledTargetingConflicts failed: {e.Message}");
        }

        return conflicts;
    }

    public override void Dispose()
    {
        _getFeatureEnabled = null;
        _getConfigEnabled = null;
        base.Dispose();
    }
}