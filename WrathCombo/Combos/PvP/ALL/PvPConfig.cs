using ECommons.GameHelpers;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvP
{
    internal static partial class PvPCommon
    {
        public class Config
        {
            public const string
                EmergencyGuardThreshold = "EmergencyGuardThreshold",
                QuickPurifyStatuses = "QuickPurifyStatuses";

            public static UserInt
                EmergencyHealThreshold = new("EmergencyHealThreshold");


            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    //I feel like this needs love, or simplify
                    case CustomComboPreset.PvP_EmergencyHeals:
                        if (Player.Object != null)
                        {
                            uint maxHP = Player.Object.MaxHp <= 15000 ? 0 : Player.Object.MaxHp - 15000;

                            if (maxHP > 0)
                            {
                                int setting = EmergencyHealThreshold;
                                float hpThreshold = (float)maxHP / 100 * setting;

                                DrawSliderInt(1, 100, EmergencyHealThreshold, $"Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.\nHP Value to be at or under: {hpThreshold}");
                            }

                            else
                            {
                                DrawSliderInt(1, 100, EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                            }
                        } else DrawSliderInt(1, 100, EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                        break;

                    case CustomComboPreset.PvP_EmergencyGuard:
                        DrawSliderInt(1, 100, EmergencyGuardThreshold, "Set the percentage to be at or under for the feature to kick in.");
                        break;

                    case CustomComboPreset.PvP_QuickPurify:
                        DrawPvPStatusMultiChoice(QuickPurifyStatuses);
                        break;
                }
            }
        }
    }
}
