using ECommons.DalamudServices;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using WrathCombo.Core;
using WrathCombo.Resources.Localization.Presets;
using WrathCombo.Resources.Localization.UI.MainWindow;
using WrathCombo.Resources.Localization.UI.Settings;

namespace WrathCombo.Window
{
    internal static class Text
    {
        // Cache for localized preset info, keyed by preset
        private sealed record LocalizedPresetInfo(string Name, string Description);
        private static FrozenDictionary<Preset, LocalizedPresetInfo>? presetCache;
        private static readonly Lock presetCacheLock = new();

        // For Reference: Dalamud supports these languages, and Ottercorp (CN)
        // https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Localization.cs#L21
        // ApplicableLangCodes = ["de", "ja", "fr", "it", "es", "ko", "no", "ru", "zh", "tw"];
        // https://github.com/ottercorp/Dalamud/blob/master/Dalamud/Localization.cs#L21
        // ApplicableLangCodes = ["de", "ja", "fr", "it", "es", "ko", "no", "ru", "zh", "tw"];

        // Pre-allocated cultures
        private static readonly CultureInfo En = new("en");
        private static readonly CultureInfo De = new("de");
        private static readonly CultureInfo Ja = new("ja");
        private static readonly CultureInfo Fr = new("fr");
        private static readonly CultureInfo It = new("it");
        private static readonly CultureInfo Es = new("es");
        private static readonly CultureInfo Ko = new("ko");
        private static readonly CultureInfo No = new("no");
        private static readonly CultureInfo Ru = new("ru");
        private static readonly CultureInfo Zh = new("zh-Hans"); // Simplified
        private static readonly CultureInfo Tw = new("zh-Hant"); // Traditional

        // Cache the game culture.
        private static CultureInfo GameCulture = Svc.PluginInterface.UiLanguage.ToCulture();

        // Expose TextInfo for formatting purposes (Job Names)
        public static TextInfo TextFormatting => GameCulture.TextInfo;

        internal static void OnLanguageChanged(string newLang)
        {
            // Update the global culture
            GameCulture = newLang.ToCulture();

            // Update any static cultures in resource managers
            MainWindow.Culture = GameCulture;
            SettingsUI.Culture = GameCulture;

            // Invalidate the preset cache safely
            lock (presetCacheLock)
            {
                presetCache = null;
            }
        }

        /// <summary>
        /// Takes known Dalamud string codes and maps to CultureInfo, with a fallback to English.
        /// </summary>
        /// <param name="uiLang"></param>
        /// <returns></returns>
        internal static CultureInfo ToCulture(this string uiLang)
        {
            // Map specific language codes
            return uiLang switch
            {
                "de" => De,
                "ja" => Ja,
                "fr" => Fr,
                "it" => It,
                "es" => Es,
                "ko" => Ko,
                "no" => No,
                "ru" => Ru,
                "zh" => Zh,
                "tw" => Tw,
                _ => En // handles "en" and any unexpected codes by falling back to English
            };
        }

        internal static class PresetLocalization
        {
            public static string GetName(Preset preset)
                => GetCache()[preset].Name;

            public static string GetDescription(Preset preset)
                => GetCache()[preset].Description;

            private static FrozenDictionary<Preset, LocalizedPresetInfo> GetCache()
            {
                lock (presetCacheLock)
                {
                    presetCache ??= BuildCache();
                    return presetCache;
                }
            }

            /// <summary>
            /// Rebuilds the cache of Preset Strings, called on first access and whenever language changes.
            /// </summary>
            /// <returns></returns>
            private static FrozenDictionary<Preset, LocalizedPresetInfo> BuildCache()
            {
                var dict = new Dictionary<Preset, LocalizedPresetInfo>(
                    PresetStorage.AllPresets.Count);

                foreach (var preset in PresetStorage.AllPresets.Keys)
                {
                    dict[preset] = new LocalizedPresetInfo(
                        // To Do: process string for magic placeholders that'll pull from sheets
                        GetLocalizedString($"{preset}_Name", CustomComboPresets.ResourceManager).ProcessSheetLookups(),
                        GetLocalizedString($"{preset}_Desc", CustomComboPresets.ResourceManager).ProcessSheetLookups()
                    );
                }

                return dict.ToFrozenDictionary();
            }
        }

        /// <summary>
        /// Processes any magic placeholders in the string that pull from game data sheets.
        /// </summary>
        /// <param name="astring"></param>
        /// <returns></returns>
        private static string ProcessSheetLookups(this string astring)
        {
            // To Do: implement actual lookup processing. For now, just return the string.
            return astring;
        }

        /// <summary>
        /// Core localized string resolver.
        /// Lets ResourceManager handle fallback chain.
        /// </summary>
        private static string GetLocalizedString(string key, ResourceManager rm)
        {
            var value = rm.GetString(key, GameCulture);

            // If missing entirely, return key (debug-friendly)
            return value ?? key;
        }
    }
}