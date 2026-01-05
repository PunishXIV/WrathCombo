using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal static partial class Bozja
{
    internal static class Config
    {
        public static UserInt
            Bozja_Tank_LostCure_Health = new("Bozja_Tank_LostCure_Health", 50),
            Bozja_Tank_LostCure2_Health = new("Bozja_Tank_LostCure2_Health", 50),
            Bozja_Tank_LostCure3_Health = new("Bozja_Tank_LostCure3_Health", 50),
            Bozja_Tank_LostCure4_Health = new("Bozja_Tank_LostCure4_Health", 50),
            Bozja_Tank_LostAethershield_Health = new("Bozja_Tank_LostAethershield_Health", 70),
            Bozja_Tank_LostReraise_Health = new("Bozja_Tank_LostReraise_Health", 10);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.Bozja_Tank_LostCure:
                    DrawSliderInt(1, 100, Bozja_Tank_LostCure_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Bozja_Tank_LostCure2:
                    DrawSliderInt(1, 100, Bozja_Tank_LostCure2_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Bozja_Tank_LostCure3:
                    DrawSliderInt(1, 100, Bozja_Tank_LostCure3_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Bozja_Tank_LostCure4:
                    DrawSliderInt(1, 100, Bozja_Tank_LostCure4_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Bozja_Tank_LostAethershield:
                    DrawSliderInt(1, 100, Bozja_Tank_LostAethershield_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Bozja_Tank_LostReraise:
                    DrawSliderInt(1, 100, Bozja_Tank_LostReraise_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
            }
        }
    }
}
