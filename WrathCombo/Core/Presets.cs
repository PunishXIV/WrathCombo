using Dalamud.Utility;
using ECommons;
using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Attributes;
using WrathCombo.Combos;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using static FFXIVClientStructs.FFXIV.Client.UI.RaptureAtkHistory.Delegates;

namespace WrathCombo.Core
{
    internal static class PresetStorage
    {
        private static HashSet<CustomComboPreset>? PvPCombos;
        private static HashSet<CustomComboPreset>? VariantCombos;
        private static HashSet<CustomComboPreset>? BozjaCombos;
        private static HashSet<CustomComboPreset>? EurekaCombos;
        private static Dictionary<CustomComboPreset, CustomComboPreset[]>? ConflictingCombos;
        private static Dictionary<CustomComboPreset, CustomComboPreset?>? ParentCombos;  // child: parent

        public static HashSet<CustomComboPreset>? AllPresets;

        public static void Init()
        {
            // Secret combos
            PvPCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<PvPCustomComboAttribute>() != default)
                .ToHashSet();

            VariantCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<VariantAttribute>() != default)
                .ToHashSet();

            BozjaCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<BozjaAttribute>() != default)
                .ToHashSet();

            EurekaCombos = Enum.GetValues<CustomComboPreset>()
                .Where(preset => preset.GetAttribute<EurekaAttribute>() != default)
                .ToHashSet();

            // Conflicting combos
            ConflictingCombos = Enum.GetValues<CustomComboPreset>()
                .ToDictionary(
                    preset => preset,
                    preset => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? Array.Empty<CustomComboPreset>());

            // Parent combos
            ParentCombos = Enum.GetValues<CustomComboPreset>()
                .ToDictionary(
                    preset => preset,
                    preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset);

            AllPresets = Enum.GetValues<CustomComboPreset>().ToHashSet();

            foreach (var preset in Enum.GetValues<CustomComboPreset>())
            {
                Presets.Attributes.Add(preset, new Presets.PresetAttributes(preset));
            }
            Svc.Log.Information($"Cached {Presets.Attributes.Count} preset attributes.");
        }


        /// <summary> Gets a value indicating whether a preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsEnabled(CustomComboPreset preset) => Service.Configuration.EnabledActions.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsPvP(CustomComboPreset preset) => PvPCombos.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsVariant(CustomComboPreset preset) => VariantCombos.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsBozja(CustomComboPreset preset) => BozjaCombos.Contains(preset);

        /// <summary> Gets a value indicating whether a preset is secret. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The boolean representation. </returns>
        public static bool IsEureka(CustomComboPreset preset) => EurekaCombos.Contains(preset);

        /// <summary> Gets the parent combo preset if it exists, or null. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The parent preset. </returns>
        public static CustomComboPreset? GetParent(CustomComboPreset preset) => ParentCombos[preset];

        /// <summary> Gets an array of conflicting combo presets. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> The conflicting presets. </returns>
        public static CustomComboPreset[] GetConflicts(CustomComboPreset preset) => ConflictingCombos[preset];

        /// <summary> Gets the full list of conflicted combos. </summary>
        public static List<CustomComboPreset> GetAllConflicts() => ConflictingCombos.Keys.ToList();

        /// <summary> Get all the info from conflicted combos. </summary>
        public static List<CustomComboPreset[]> GetAllConflictOriginals() => ConflictingCombos.Values.ToList();

        public static CustomComboPreset? GetPresetByString(string value)
        {
            if (Enum.GetValues<CustomComboPreset>().TryGetFirst(x => x.ToString().ToLower() == value.ToLower(), out var pre))
            {
                return pre;
            }
            return null;
        }

        public static CustomComboPreset? GetPresetByInt(int value)
        {
            if (Enum.GetValues<CustomComboPreset>().TryGetFirst(x => (int)x == value, out var pre))
            {
                return pre;
            }
            return null;
        }

        /// <summary> Iterates up a preset's parent tree, enabling each of them. </summary>
        /// <param name="preset"> Combo preset to enabled. </param>
        public static void EnableParentPresets(CustomComboPreset preset)
        {
            var parentMaybe = GetParent(preset);

            while (parentMaybe != null)
            {
                EnablePreset(parentMaybe.Value);
                parentMaybe = GetParent(parentMaybe.Value);
            }
        }

        public static void DisableAllConflicts(CustomComboPreset preset)
        {
            var conflicts = GetConflicts(preset);
            foreach (var conflict in conflicts)
            {
                Service.Configuration.EnabledActions.Remove(conflict);
            }
        }

        public static bool EnablePreset(string preset, bool outputLog = false)
        {
            var pre = GetPresetByString(preset);
            if (pre != null)
            {
                return EnablePreset(pre.Value, outputLog);
            }
            return false;
        }

        public static bool EnablePreset(int preset, bool outputLog = false)
        {
            var pre = GetPresetByInt(preset);
            if (pre != null)
            {
                return EnablePreset(pre.Value, outputLog);
            }
            return false;
        }

        public static bool EnablePreset(CustomComboPreset preset, bool outputLog = false)
        {
            var ctrlText = GetControlledText(preset);
            EnableParentPresets(preset);
            var ret = Service.Configuration.EnabledActions.Add(preset);
            DisableAllConflicts(preset);

            if (outputLog)
                DuoLog.Information($"{(int)preset} - {preset} SET{ctrlText}");

            return ret;
        }


        public static bool DisablePreset(string preset, bool outputLog = false)
        {
            var pre = GetPresetByString(preset);
            if (pre != null)
            {
                return DisablePreset(pre.Value, outputLog);
            }
            return false;
        }

        public static bool DisablePreset(int preset, bool outputLog = false)
        {
            var pre = GetPresetByInt(preset);
            if (pre != null)
            {
                return DisablePreset(pre.Value, outputLog);
            }
            return false;
        }

        public static bool DisablePreset(CustomComboPreset preset, bool outputLog = false)
        {
            if (Service.Configuration.EnabledActions.Remove(preset))
            {
                var ctrlText = GetControlledText(preset);

                if (outputLog)
                    DuoLog.Information($"{(int)preset} - {preset} UNSET{ctrlText}");

                return true;
            }

            return false;
        }

        private static object GetControlledText(CustomComboPreset preset)
        {
            var controlled = P.UIHelper.PresetControlled(preset) is not null;
            var ctrlText = controlled ? " " + OptionControlledByIPC : "";

            return ctrlText;
        }

        public static bool TogglePreset(CustomComboPreset preset, bool outputLog = false)
        {
            var ctrlText = GetControlledText(preset);
            if (!Service.Configuration.EnabledActions.Remove(preset))
            {
                var ret = EnablePreset(preset);
                if (outputLog)
                    DuoLog.Information($"{(int)preset} - {preset} SET{ctrlText}");
            }
            else if (outputLog)
            {
                DuoLog.Information($"{(int)preset} - {preset} UNSET{ctrlText}");
            }
            return false;
        }

        public static bool TogglePreset(string preset, bool outputLog = false)
        {
            var pre = GetPresetByString(preset);
            if (pre != null)
            {
                return TogglePreset(pre.Value, outputLog);
            }
            return false;
        }

        public static bool TogglePreset(int preset, bool outputLog = false)
        {
            var pre = GetPresetByInt(preset);
            if (pre != null)
            {
                return TogglePreset(pre.Value, outputLog);
            }
            return false;
        }
    }
}
