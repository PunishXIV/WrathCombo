using System;
using System.IO;
using Dalamud.Plugin;
using ECommons.Logging;
using DateTime = System.DateTime;

namespace XIVSlothCombo.Core;

internal static class Transition // From Sloth
{
    /// <summary>
    /// Get the path to XIVLauncher/pluginConfigs
    /// </summary>
    /// <param name="plugins">
    /// The <see cref="IDalamudPluginInterface">pluginInterface</see> from
    /// <see cref="XIVSlothCombo"/>.
    /// </param>
    /// <returns></returns>
    private static string GetPluginConfigPath(IDalamudPluginInterface plugins)
    {
        // Build the pluginConfigs path
        var pluginConfig = plugins.GetPluginConfigDirectory();
        // Remove the last directory separator
        if (Path.EndsInDirectorySeparator(pluginConfig))
            pluginConfig = Path.TrimEndingDirectorySeparator(pluginConfig);
        // Remove everything after the last directory separator
        pluginConfig = pluginConfig[..pluginConfig.LastIndexOf(Path.DirectorySeparatorChar)];

        return pluginConfig;
    }

    /// <summary>
    /// Check if the user has usable Sloth settings
    /// </summary>
    /// <param name="plugins">
    /// The <see cref="IDalamudPluginInterface">pluginInterface</see> from
    /// <see cref="XIVSlothCombo"/>.
    /// </param>
    /// <returns>
    /// Whether the user both <b>has</b> sloth settings and those settings are
    /// <b>from Dawntrail</b>.
    /// </returns>
    public static bool HasSlothSettings(IDalamudPluginInterface plugins)
    {
        // Bail if the file doesn't exist
        var pluginConfig = GetPluginConfigPath(plugins);
        var slothConfigFile = Path.Combine(pluginConfig, "XIVSlothCombo.json");
        if (!File.Exists(slothConfigFile)) return false;

        PluginLog.Information("Sloth settings found, checking...");

        // Return true if the settings are from Dawntrail
        var lastWriteTime = File.GetLastWriteTime(slothConfigFile);
        return lastWriteTime > new DateTime(2024, 7, 5);
    }

    /// <summary>
    /// Transition the Sloth settings to Wrath settings.
    /// </summary>
    /// <param name="plugins">
    /// The <see cref="IDalamudPluginInterface">pluginInterface</see> from
    /// <see cref="XIVSlothCombo"/>.
    /// </param>
    /// <returns>
    /// Whether the transition was successful.
    /// </returns>
    public static bool ConvertSlothSettings(IDalamudPluginInterface plugins)
    {
        // Build the file paths
        var pluginConfig = GetPluginConfigPath(plugins);
        var slothConfigFile = Path.Combine(pluginConfig, "XIVSlothCombo.json");
        var wrathConfigFile = Path.Combine(pluginConfig, "WrathCombo.json");

        PluginLog.Information("Sloth settings found, transitioning...");

        try
        {
            // Rename the Sloth settings to Wrath settings
            File.Move(slothConfigFile, wrathConfigFile);
            PluginLog.Information("Sloth settings transitioned successfully.");

            // Remove the WrathCombo folder that was created
            var slothComboFolder = Path.Combine(pluginConfig, "XIVSlothCombo");
            if (Directory.Exists(slothComboFolder))
                Directory.Delete(slothComboFolder);
            var wrathComboFolder = Path.Combine(pluginConfig, "WrathCombo");
            if (Directory.Exists(wrathComboFolder))
                Directory.Delete(wrathComboFolder);

            return true;
        }
        catch (Exception e)
        {
            PluginLog.Warning("Failed to transition Sloth settings." + "\n" + e);
            return false;
        }
    }
}
