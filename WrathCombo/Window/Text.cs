using ECommons.DalamudServices;
using System.Globalization;

namespace WrathCombo.Window
{
    internal class Text
    {
        private static TextInfo? _cachedTextInfo;
        
        //Used to format job names based on region
        public static TextInfo GetTextInfo()
        {
            // Use cached TextInfo if available
            // Otherwise create new and cache for future use
            if (_cachedTextInfo is null)
            {
                // Job names are lowercase by default
                // This capitalizes based on regional rules
                var cultureId = Svc.ClientState.ClientLanguage switch
                {
                    Dalamud.Game.ClientLanguage.French => "fr-FR",
                    Dalamud.Game.ClientLanguage.Japanese => "ja-JP",
                    Dalamud.Game.ClientLanguage.German => "de-DE",
                    _ => "en-US",
                };

                _cachedTextInfo = new CultureInfo(cultureId, useUserOverride: false).TextInfo;
            }

            return _cachedTextInfo;
        }
    }
}
