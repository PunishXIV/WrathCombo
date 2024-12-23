using Dalamud.Configuration;
using ECommons.DalamudServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons.Logging;
using WrathCombo.AutoRotation;
using WrathCombo.Combos;
using WrathCombo.Extensions;
using WrathCombo.Window;

namespace WrathCombo.Core
{
    /// <summary> Plugin configuration. </summary>
    [Serializable]
    public class PluginConfiguration : IPluginConfiguration
    {
        #region Version

        /// <summary> Gets or sets the configuration version. </summary>
        public int Version { get; set; } = 5;

        #endregion

        #region EnabledActions

        /// <summary> Gets or sets the collection of enabled combos. </summary>
        [JsonProperty("EnabledActionsV6")]
        public HashSet<CustomComboPreset> EnabledActions { get; set; } = [];

        #endregion

        #region Settings Options

        /// <summary> Gets or sets a value indicating whether to output combat log to the chatbox. </summary>
        public bool EnabledOutputLog { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether to hide combos which conflict with enabled presets. </summary>
        public bool HideConflictedCombos { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether to hide the children of a feature if it is disabled. </summary>
        public bool HideChildren { get; set; } = false;

        /// <summary> Gets or sets the offset of the melee range check. Default is 0. </summary>
        public double MeleeOffset { get; set; } = 0;

        public bool BlockSpellOnMove = false;
        public Vector4 TargetHighlightColor { get; set; } = new() { W = 1, X = 0.5f, Y = 0.5f, Z = 0.5f };

        public bool OutputOpenerLogs;

        public float MovementLeeway = 0f;

        #endregion

        #region AutoAction Settings
        public Dictionary<CustomComboPreset, bool> AutoActions { get; set; } = [];

        public AutoRotationConfig RotationConfig { get; set; } = new();

        public Dictionary<uint, uint> IgnoredNPCs { get; set; } = new();

        #endregion

        #region Custom Float Values

        [JsonProperty("CustomFloatValuesV6")]
        internal static Dictionary<string, float> CustomFloatValues { get; set; } = [];

        /// <summary> Gets a custom float value. </summary>
        public static float GetCustomFloatValue(string config, float defaultMinValue = 0)
        {
            if (!CustomFloatValues.TryGetValue(config, out float configValue))
            {
                SetCustomFloatValue(config, defaultMinValue);
                return defaultMinValue;
            }

            return configValue;
        }

        /// <summary> Sets a custom float value. </summary>
        public static void SetCustomFloatValue(string config, float value) => CustomFloatValues[config] = value;

        #endregion

        #region Custom Int Values

        [JsonProperty("CustomIntValuesV6")]
        internal static Dictionary<string, int> CustomIntValues { get; set; } = [];

        /// <summary> Gets a custom integer value. </summary>
        public static int GetCustomIntValue(string config, int defaultMinVal = 0)
        {
            if (!CustomIntValues.TryGetValue(config, out int configValue))
            {
                SetCustomIntValue(config, defaultMinVal);
                return defaultMinVal;
            }

            return configValue;
        }

        /// <summary> Sets a custom integer value. </summary>
        public static void SetCustomIntValue(string config, int value) => CustomIntValues[config] = value;

        #endregion

        #region Custom Int Array Values
        [JsonProperty("CustomIntArrayValuesV6")]
        internal static Dictionary<string, int[]> CustomIntArrayValues { get; set; } = [];

        /// <summary> Gets a custom integer array value. </summary>
        public static int[] GetCustomIntArrayValue(string config)
        {
            if (!CustomIntArrayValues.TryGetValue(config, out int[]? configValue))
            {
                SetCustomIntArrayValue(config, []);
                return [];
            }

            return configValue;
        }

        /// <summary> Sets a custom integer array value. </summary>
        public static void SetCustomIntArrayValue(string config, int[] value) => CustomIntArrayValues[config] = value;

        #endregion

        #region Custom Bool Values

        [JsonProperty("CustomBoolValuesV6")]
        internal static Dictionary<string, bool> CustomBoolValues { get; set; } = [];

        /// <summary> Gets a custom boolean value. </summary>
        public static bool GetCustomBoolValue(string config)
        {
            if (!CustomBoolValues.TryGetValue(config, out bool configValue))
            {
                SetCustomBoolValue(config, false);
                return false;
            }

            return configValue;
        }

        /// <summary> Sets a custom boolean value. </summary>
        public static void SetCustomBoolValue(string config, bool value) => CustomBoolValues[config] = value;

        #endregion

        #region Custom Bool Array Values

        [JsonProperty("CustomBoolArrayValuesV6")]
        internal static Dictionary<string, bool[]> CustomBoolArrayValues { get; set; } = [];

        /// <summary> Gets a custom boolean array value. </summary>
        public static bool[] GetCustomBoolArrayValue(string config)
        {
            if (!CustomBoolArrayValues.TryGetValue(config, out bool[]? configValue))
            {
                SetCustomBoolArrayValue(config, Array.Empty<bool>());
                return Array.Empty<bool>();
            }

            return configValue;
        }

        /// <summary> Sets a custom boolean array value. </summary>
        public static void SetCustomBoolArrayValue(string config, bool[] value) => CustomBoolArrayValues[config] = value;

        #endregion

        #region Job-specific

        /// <summary> Gets active Blue Mage (BLU) spells. </summary>
        public List<uint> ActiveBLUSpells { get; set; } = [];

        /// <summary> Gets or sets an array of 4 ability IDs to interact with the <see cref="CustomComboPreset.DNC_DanceComboReplacer"/> combo. </summary>
        public uint[] DancerDanceCompatActionIDs { get; set; } = [ 0, 0, 0, 0, ];

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

                var presets = Enum.GetValues<CustomComboPreset>().Cast<int>();

                foreach (int value in values)
                {
                    Svc.Log.Debug(value.ToString());
                    if (presets.Contains(value))
                    {
                        var preset = Enum.GetValues<CustomComboPreset>()
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

        #region Other (SpecialEvent, MotD, Save)

        /// <summary> Hides the message of the day. </summary>
        public bool HideMessageOfTheDay { get; set; } = false;

        /// <summary>
        ///     Whether the Setting Change Suggestion window was hidden for a
        ///     specific version.
        /// </summary>
        /// <seealso cref="SettingChangeWindow"/>
        public string HideSettingsChangeSuggestionForVersion { get; set; } = "";

        /// <summary> Save the configuration to disk. </summary>
        public void Save() => Svc.PluginInterface.SavePluginConfig(this);

        #endregion
    }
}
