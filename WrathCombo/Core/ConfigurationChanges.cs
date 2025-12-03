#region

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ECommons.Logging;

#endregion

namespace WrathCombo.Core;

public partial class Configuration
{
    /// <summary>
    ///     Fired when a plugin setting, preset, or job setting is changed
    ///     in any way.
    /// </summary>
    /// <remarks>
    ///     TODO: Trigger on ipc stuff(?) and setting change via ui, etc.
    ///     <br />
    ///     TODO: Add Retarget-Clearing if it's a preset or config change.
    ///      (fix hanging Retargets)
    /// </remarks>
    public static event EventHandler<ConfigChangeEventArgs>? ConfigChanged;

    public sealed class ConfigChangeEventArgs(
        ConfigChangeType type,
        ConfigChangeSource source,
        string key,
        object newValue,
        string stack)
        : EventArgs
    {
        public ConfigChangeType Type { get; } = type;

        public ConfigChangeSource Source { get; } = source;

        public string Key { get; } = key;

        public object NewValue { get; } = newValue;

        public string Stack { get; } = stack;
    }

    #region Event Enums

    public enum ConfigChangeSource
    {
        UI,
        Command,
        IPC,
        Task,
        AutomaticReaction,
        Other,
    }

    public enum ConfigChangeType
    {
        Setting,
        AutoRotSetting,
        Preset,
        PresetAutoMode,
        UserData,
    }

    #endregion

    #region Event Methods

    /// <summary>
    ///     Safely invoke the <see cref="ConfigChanged" /> event,
    ///     Isolating subscriber exceptions.
    /// </summary>
    private static void RaiseUserConfigChanged(ConfigChangeEventArgs args)
    {
        var handlers = ConfigChanged;
        if (handlers == null) return;

        // Invoke each subscriber separately so one exception won't stop others.
        foreach (var d in handlers.GetInvocationList())
        {
            try
            {
                ((EventHandler<ConfigChangeEventArgs>)d).Invoke(null, args);
            }
            catch (Exception ex)
            {
                // Log but continue.
                PluginLog.Error($"UserConfigChanged handler threw: {ex}");
            }
        }
    }

    /// <summary>
    ///     Triggers the <see cref="ConfigChanged" /> event.
    /// </summary>
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void TriggerUserConfigChanged
    (ConfigChangeType type,
        ConfigChangeSource source,
        string key,
        object newValue,
        object? oldValue = null)
    {
        var trace = new StackTrace().ToString();
        var args = new ConfigChangeEventArgs(type, source, key,
            newValue, trace);
        RaiseUserConfigChanged(args);
    }

    #endregion
}