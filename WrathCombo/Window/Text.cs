using ECommons.DalamudServices;
using System.Globalization;
using System.Resources;
using WrathCombo.Resources.Localization;

namespace WrathCombo.Window
{
    internal class Text
    {
        // Pre-allocated cultures
        private static readonly CultureInfo Ja = new("ja");
        private static readonly CultureInfo En = new("en");
        private static readonly CultureInfo De = new("de");
        private static readonly CultureInfo Fr = new("fr");
        //private static readonly CultureInfo Zh = new("zh");
        //private static readonly CultureInfo Ko = new("ko");

        //Used to format job names based on region
        private static TextInfo? _cachedTextInfo;

        /// <summary>
        /// Gets the current culture based on FFXIV client language.
        /// Possibly supports unofficial CN/KR forks.
        /// </summary>
        private static CultureInfo GetGameCulture()
        {
            return (int)Svc.ClientState.ClientLanguage switch
            {
                0 => Ja,
                1 => En,
                2 => De,
                3 => Fr,

                // Fork support ?
                //4 => Zh,
                //5 => Ko,

                // Unknown / unexpected → use OS culture
                _ => CultureInfo.CurrentUICulture
            };
        }

        public static TextInfo GetTextInfo()
        {
            if (_cachedTextInfo is null)
            {
                // Reuse the same culture selection as Localization.GetGameCulture()
                var culture = GetGameCulture();

                // Get TextInfo for capitalization rules
                _cachedTextInfo = culture.TextInfo;
            }

            return _cachedTextInfo;
        }

        /// <summary>
        /// Core localized string resolver.
        /// Lets ResourceManager handle fallback chain.
        /// </summary>
        private static string GetLocalizedString(string key, ResourceManager rm)
        {
            var culture = GetGameCulture();

            var value = rm.GetString(key, culture);

            // If missing entirely, return key (debug-friendly)
            return value ?? key;
        }

        /// <summary>
        /// Preset localization
        /// </summary>
        public static string GetPresetString(string key) => GetLocalizedString(key, CustomComboPresets.ResourceManager);

        /// <summary>
        /// Settings UI localization
        /// </summary>
        public static string GetSettingsUIString(string key) => GetLocalizedString(key, Menu_Settings.ResourceManager);
    }
}
