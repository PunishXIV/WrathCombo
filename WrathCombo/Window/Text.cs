using Dalamud.Game;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.Sheets;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using WrathCombo.Core;
using WrathCombo.Resources.Localization.JobConfigs;
using WrathCombo.Resources.Localization.Misc;
using WrathCombo.Resources.Localization.Presets;
using WrathCombo.Resources.Localization.UI.AutoRotation;
using WrathCombo.Resources.Localization.UI.Features;
using WrathCombo.Resources.Localization.UI.MainWindow;
using WrathCombo.Resources.Localization.UI.Misc;
using WrathCombo.Resources.Localization.UI.Settings;
using WrathCombo.Window.Functions;
using WrathCombo.Window.Tabs;

namespace WrathCombo.Window
{
    internal static class Text
    {
        // Cache for localized strings with format parameters that read game data
        private static readonly ConcurrentDictionary<string, string> FormatCache = new();

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
        private static CultureInfo _gameCulture = Svc.PluginInterface.UiLanguage.ToCulture();

        public static ClientLanguage LangFromCulture = Svc.ClientState.ClientLanguage;

        // Expose TextInfo for formatting purposes (Job Names)
        public static TextInfo TextFormatting => _gameCulture.TextInfo;

        internal static void OnLanguageChanged(string newLang)
        {
            // Update the global culture
            _gameCulture = newLang.ToCulture();

            // Update cultures in resource managers
            // UI
            AutoRotationUI.Culture = _gameCulture;
            FeaturesUI.Culture = _gameCulture;
            MainWindowUI.Culture = _gameCulture;
            MiscUI.Culture = _gameCulture;
            SettingsUI.Culture = _gameCulture;
            SettingsCfgUI.Culture = _gameCulture;
            MiscStrings.Culture = _gameCulture;

            // Job Configs
            Generics.Culture = _gameCulture;
            AST_Config.Culture = _gameCulture;
            BLM_Config.Culture = _gameCulture;
            BLU_Config.Culture = _gameCulture;
            BRD_Config.Culture = _gameCulture;
            DNC_Config.Culture = _gameCulture;
            DOL_Config.Culture = _gameCulture;
            DRG_Config.Culture = _gameCulture;
            DRK_Config.Culture = _gameCulture;
            GNB_Config.Culture = _gameCulture;
            MCH_Config.Culture = _gameCulture;
            MNK_Config.Culture = _gameCulture;
            NIN_Config.Culture = _gameCulture;
            PCT_Config.Culture = _gameCulture;
            PLD_Config.Culture = _gameCulture;
            RPR_Config.Culture = _gameCulture;
            RDM_Config.Culture = _gameCulture;
            SAM_Config.Culture = _gameCulture;
            SCH_Config.Culture = _gameCulture;
            SGE_Config.Culture = _gameCulture;
            SMN_Config.Culture = _gameCulture;
            VPR_Config.Culture = _gameCulture;
            WAR_Config.Culture = _gameCulture;
            WHM_Config.Culture = _gameCulture;

            LangFromCulture = _gameCulture.TwoLetterISOLanguageName switch
            {
                "en" => ClientLanguage.English,
                "de" => ClientLanguage.German,
                "ja" => ClientLanguage.Japanese,
                "fr" => ClientLanguage.French,
                "zh-Hans" or "zh-Hant" => (ClientLanguage)4,
                _ => LangFromCulture
            };

            Svc.Log.Debug($"LangFromCulture {LangFromCulture}");

            // Invalidate the caches safely
            PresetLocalization.Clear();
            Misc.Clear();
            JobNameLocalization.Clear();
            ActionAndStatusLocalization.Clear();
            Settings.SettingsList.Clear();
            Setting.CachedSettings.Clear();
            FormatCache.Clear();
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
            private sealed record LocalizedPresetInfo(string Name, string Description);
            private static FrozenDictionary<Preset, LocalizedPresetInfo>? _presetCache;
            private static readonly Lock PresetCacheLock = new();

            public static string GetName(Preset preset)
                => GetCache()[preset].Name;

            public static string GetDescription(Preset preset)
                => GetCache()[preset].Description;

            private static FrozenDictionary<Preset, LocalizedPresetInfo> GetCache()
            {
                lock (PresetCacheLock)
                {
                    _presetCache ??= BuildCache();
                    return _presetCache;
                }
            }

            public static void Clear()
            {
                lock (PresetCacheLock)
                {
                    _presetCache = null;
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
                        GetLocalizedString($"{preset}_Name", CustomComboPresets.ResourceManager)!.ProcessSheetLookups(),
                        GetLocalizedString($"{preset}_Desc", CustomComboPresets.ResourceManager)!.ProcessSheetLookups()
                    );
                }

                return dict.ToFrozenDictionary();
            }
        }

        internal static class JobNameLocalization
        {
            private sealed record LocalizedJobInfo(string Name, string ShortName);
            private static readonly ConcurrentDictionary<Job, LocalizedJobInfo> _jobNameCache = [];

            public static string GetJobName(Job job)
                => _jobNameCache.GetOrAdd(job, BuildEntry).Name;

            public static string GetJobShortName(Job job)
                => _jobNameCache.GetOrAdd(job, BuildEntry).ShortName;

            private static LocalizedJobInfo BuildEntry(Job job)
            {
                // Name
                var sheet = Svc.Data.GetExcelSheet<ClassJob>(LangFromCulture).GetRow((uint)job);
                string jobName = job switch
                {
                    Job.ADV => MiscUI.Roles_and_Content,
                    Job.MIN or Job.BTN or Job.FSH
                        => sheet.ClassJobCategory.Value.Name.ToString(),
                    _ => sheet.Name.ToString()
                };

                jobName = TextFormatting.ToTitleCase(jobName);

                // Abbreviation / Short Name
                string shortName = job switch
                {
                    Job.ADV => string.Empty,
                    Job.MIN or Job.BTN or Job.FSH => MiscUI.DOL,
                    _ => sheet.Abbreviation.ToString()
                };

                return new LocalizedJobInfo(jobName, shortName);
            }

            public static void Clear() => _jobNameCache.Clear();
        }

        internal static class ActionAndStatusLocalization
        {
            private static readonly ConcurrentDictionary<uint, string> _actionNameCache = new();
            private static readonly ConcurrentDictionary<uint, string> _traitNameCache = new();
            private static readonly ConcurrentDictionary<uint, string> _statusNameCache = new();

            public static string GetActionName(uint actionId)
                => _actionNameCache.GetOrAdd(actionId, Svc.Data.GetExcelSheet<Action>(LangFromCulture).GetRowOrDefault(actionId)?.Name.ToString() ?? "Unknown Action");

            public static string GetTraitName(uint traitId)
                => _traitNameCache.GetOrAdd(traitId, Svc.Data.GetExcelSheet<Trait>(LangFromCulture).GetRowOrDefault(traitId)?.Name.ToString() ?? "Unknown Trait");

            public static string GetStatusName(uint statusId)
                => _statusNameCache.GetOrAdd(statusId, Svc.Data.GetExcelSheet<Status>(LangFromCulture).GetRowOrDefault(statusId)?.Name.ToString() ?? "Unknown Status");

            public static void Clear()
            {
                _actionNameCache.Clear();
                _traitNameCache.Clear();
                _statusNameCache.Clear();
            }
        }

        internal static class Misc
        {
            private static FrozenDictionary<Strings, string>? _miscCache;
            private static readonly Lock MiscCacheLock = new();

            public enum Strings
            {
                OccultCrescentContentName,
                OccultPhantomChemist,
            }

            public static string GetString(Strings key)
                => GetCache()[key];

            private static FrozenDictionary<Strings, string> GetCache()
            {
                lock (MiscCacheLock)
                {
                    _miscCache ??= BuildCache();
                    return _miscCache;
                }
            }

            public static void Clear()
            {
                lock (MiscCacheLock)
                {
                    _miscCache = null;
                }
            }

            private static FrozenDictionary<Strings, string> BuildCache()
            {
                var dict = new Dictionary<Strings, string>
                {
                    [Strings.OccultCrescentContentName] = Svc.Data.GetExcelSheet<BannerBg>(LangFromCulture).GetRow(312).Name.ToString(),
                    [Strings.OccultPhantomChemist]      = Svc.Data.GetExcelSheet<MKDSupportJob>(LangFromCulture).GetRow(10).Name.ToString(),
                };

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
        public static string? GetLocalizedString(string key, ResourceManager rm, bool returnNull = false)
        {
            var value = rm.GetString(key, _gameCulture);

            // If missing entirely, return key (debug-friendly)
            if (!returnNull)
                return value ?? key;
            else
                return value ?? null;
        }

        /// <summary>
        /// String.Format, but caches!
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatAndCache(string format, params object[] args)
        {
            // Create a unique cache key based on the format string and arguments
            var key = format + "|" + string.Join("|", args);
            return FormatCache.GetOrAdd(key, _ => string.Format(format, args));
        }
    }
}