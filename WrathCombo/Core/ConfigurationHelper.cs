using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ECommons.DalamudServices;
using ECommons.Logging;
using Newtonsoft.Json;
using WrathCombo.Extensions;
using WrathCombo.Services;
using Debug = WrathCombo.Window.Tabs.Debug;

namespace WrathCombo.Core;

public partial class Configuration
{
    internal void SetActionChanging(bool? newValue = null)
    {
        if (newValue is not null && newValue != ActionChanging)
        {
            ActionChanging = newValue.Value;
            Save();
        }

        // Checks if action replacing is not in line with the setting
        if (ActionChanging && !Service.ActionReplacer.getActionHook.IsEnabled)
            Service.ActionReplacer.getActionHook.Enable();
        if (!ActionChanging && Service.ActionReplacer.getActionHook.IsEnabled)
            Service.ActionReplacer.getActionHook.Disable();
    }

    #region Saving

    /// <summary>
    ///     The queue of items to be saved.
    /// </summary>
    internal static readonly Queue<(Configuration, StackTrace)> SaveQueue = [];

    /// <summary>
    ///     Whether an item is currently being saved.
    /// </summary>
    private static bool _isSaving;

    /// <summary>
    ///     Process the <see cref="SaveQueue"/>, trying to save each item.
    /// </summary>
    /// <seealso cref="Save"/>
    internal static void ProcessSaveQueue()
    {
        if (_isSaving || SaveQueue.Count == 0) return;

        _isSaving = true;
        var (config, trace) = SaveQueue.Dequeue();

        try
        {
            Svc.PluginInterface.SavePluginConfig(config);
            _isSaving = false;
        }
        catch (Exception)
        {
            Svc.Framework.Run(() => RetrySave(config, trace));
        }
    }

    internal static void RetrySave
        (Configuration config, StackTrace trace)
    {
        var success = false;
        var retryCount = 0;

        while (!success)
        {
            try
            {
                Svc.PluginInterface.SavePluginConfig(config);
                success = true;
            }
            catch (Exception e)
            {
                retryCount++;
                if (retryCount < 3)
                {
                    Task.Delay(20).Wait();
                    continue;
                }

                PluginLog.Error(
                    "Failed to save configuration after 3 retries.\n" +
                    e.Message + "\n" + trace);
                _isSaving = false;
                return;
            }
        }

        _isSaving = false;
    }

    /// <summary> Set the configuration to be saved to disk. </summary>
    /// <remarks>
    ///     Configurations set to be saved will be processed in the order they
    ///     were added, each frame.
    /// </remarks>
    /// <seealso cref="SaveQueue"/>
    public void Save()
    {
        if (Debug.DebugConfig)
            return;

        SaveQueue.Enqueue((this, new StackTrace()));
    }

    #endregion

    #region Preset Resetting

    [JsonProperty]
    private static Dictionary<string, bool> ResetFeatureCatalog { get; set; } = [];

    private static bool GetResetValues(string config)
    {
        if (ResetFeatureCatalog.TryGetValue(config, out var value)) return value;

        return false;
    }

    private static void SetResetValues(string config, bool value)
    {
        ResetFeatureCatalog[config] = value;
    }

    public void ResetFeatures(string config, int[] values)
    {
        Svc.Log.Debug($"{config} {GetResetValues(config)}");
        if (!GetResetValues(config))
        {
            bool needToResetMessagePrinted = false;

            var presets = Enum.GetValues<Preset>().Cast<int>();

            foreach (int value in values)
            {
                Svc.Log.Debug(value.ToString());
                if (presets.Contains(value))
                {
                    var preset = Enum.GetValues<Preset>()
                        .Where(preset => (int)preset == value)
                        .First();

                    if (!PresetStorage.IsEnabled(preset)) continue;

                    if (!needToResetMessagePrinted)
                    {
                        DuoLog.Error($"Some features have been disabled due to an internal configuration update:");
                        needToResetMessagePrinted = !needToResetMessagePrinted;
                    }

                    var info = preset.GetComboAttribute();
                    DuoLog.Error($"- {info.JobName}: {info.Name}");
                    EnabledActions.Remove(preset);
                }
            }

            if (needToResetMessagePrinted)
                DuoLog.Error($"Please re-enable these features to use them again. We apologise for the inconvenience");
        }
        SetResetValues(config, true);
        Save();
    }

    #endregion

    #region UserConfig Method Access

    #region Custom Floats

    /// <summary> Gets a custom float value. </summary>
    public static float GetCustomFloatValue(string config, float value = 0)
    {
        if (!CustomFloatValues.TryGetValue(config, out float configValue))
        {
            SetCustomFloatValue(config, value, true);
            return value;
        }

        return configValue;
    }

    /// <summary> Sets a custom float value. </summary>
    /// <returns> The Set value.</returns>
    public static float SetCustomFloatValue
        (string config, float value, bool shouldBatch = false)
    {
        CustomFloatValues[config] = value;

        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.UserData, ConfigChangeSource.UI,
            config, value);

        // todo: add batching logic, for initial plugin loading

        Service.Configuration.Save();
        return value;
    }

    #endregion

    #region Custom Ints

    /// <summary> Gets a custom integer value. </summary>
    public static int GetCustomIntValue(string config, int value = 0)
    {
        if (!CustomIntValues.TryGetValue(config, out int configValue))
        {
            SetCustomIntValue(config, value, true);
            return value;
        }

        return configValue;
    }

    /// <summary> Sets a custom integer value. </summary>
    /// <returns> The Set value.</returns>
    public static int SetCustomIntValue
        (string config, int value, bool shouldBatch = false)
    {
        CustomIntValues[config] = value;

        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.UserData, ConfigChangeSource.UI,
            config, value);

        // todo: add batching logic, for initial plugin loading

        Service.Configuration.Save();
        return value;
    }

    #endregion

    #region Custom Bools

    /// <summary> Gets a custom boolean value. </summary>
    public static bool GetCustomBoolValue(string config)
    {
        if (!CustomBoolValues.TryGetValue(config, out bool configValue))
        {
            SetCustomBoolValue(config, false, true);
            return false;
        }

        return configValue;
    }

    /// <summary> Sets a custom boolean value. </summary>
    /// <returns> The Set value.</returns>
    public static bool SetCustomBoolValue
        (string config, bool value, bool shouldBatch = false)
    {
        CustomBoolValues[config] = value;

        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.UserData, ConfigChangeSource.UI,
            config, value);

        // todo: add batching logic, for initial plugin loading

        Service.Configuration.Save();
        return value;
    }

    #endregion

    #region Custom Int Arrays

    /// <summary> Gets a custom integer array value. </summary>
    public static int[] GetCustomIntArrayValue(string config)
    {
        if (!CustomIntArrayValues.TryGetValue(config, out int[]? configValue))
        {
            SetCustomIntArrayValue(config, [], true);
            return [];
        }

        return configValue;
    }

    /// <summary> Sets a custom integer array value. </summary>
    /// <returns> The Set value.</returns>
    public static int[] SetCustomIntArrayValue
        (string config, int[] value, bool shouldBatch = false)
    {
        CustomIntArrayValues[config] = value;

        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.UserData, ConfigChangeSource.UI,
            config, value);

        // todo: add batching logic, for initial plugin loading

        Service.Configuration.Save();
        return value;
    }

    #endregion

    #region Custom Bool Arrays

    /// <summary> Gets a custom boolean array value. </summary>
    public static bool[] GetCustomBoolArrayValue(string config)
    {
        if (!CustomBoolArrayValues.TryGetValue(config, out bool[]? configValue))
        {
            SetCustomBoolArrayValue(config, [], true);
            return [];
        }

        return configValue;
    }

    /// <summary> Sets a custom boolean array value. </summary>
    /// <returns> The Set value.</returns>
    public static bool[] SetCustomBoolArrayValue
        (string config, bool[] value, bool shouldBatch = false)
    {
        CustomBoolArrayValues[config] = value;

        Service.Configuration.TriggerUserConfigChanged(
            ConfigChangeType.UserData, ConfigChangeSource.UI,
            config, value);

        // todo: add batching logic, for initial plugin loading

        Service.Configuration.Save();
        return value;
    }

    #endregion

    #endregion
}