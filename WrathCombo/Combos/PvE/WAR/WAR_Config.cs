using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    internal static class Config
    {
        public static UserInt
            WAR_InfuriateRange = new("WarInfuriateRange"),
            WAR_SurgingRefreshRange = new("WarSurgingRefreshRange"),
            WAR_KeepOnslaughtCharges = new("WarKeepOnslaughtCharges"),
            WAR_KeepInfuriateCharges = new("WarKeepInfuriateCharges"),
            WAR_VariantCure = new("WAR_VariantCure"),
            WAR_FellCleaveGauge = new("WAR_FellCleaveGauge"),
            WAR_DecimateGauge = new("WAR_DecimateGauge"),
            WAR_InfuriateSTGauge = new("WAR_InfuriateSTGauge"),
            WAR_InfuriateAoEGauge = new("WAR_InfuriateAoEGauge");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.WAR_ST_Advanced_StormsEye:
                    UserConfig.DrawSliderInt(0, 30, WAR_SurgingRefreshRange,
                        "Seconds remaining before refreshing Surging Tempest.");

                    break;

                case CustomComboPreset.WAR_InfuriateFellCleave:
                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateRange,
                        "Set how much rage to be at or under to use this feature.");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Onslaught:
                    UserConfig.DrawSliderInt(0, 2, WAR_KeepOnslaughtCharges,
                        "How many charges to keep ready? (0 = Use All)");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Infuriate:
                    UserConfig.DrawSliderInt(0, 2, WAR_KeepInfuriateCharges,
                        "How many charges to keep ready? (0 = Use All)");

                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateSTGauge, "Use when gauge is under or equal to");

                    break;

                case CustomComboPreset.WAR_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, WAR_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_FellCleave:
                    UserConfig.DrawSliderInt(50, 100, WAR_FellCleaveGauge, "Minimum gauge to spend");

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Decimate:
                    UserConfig.DrawSliderInt(50, 100, WAR_DecimateGauge, "Minimum gauge to spend");

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Infuriate:
                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateAoEGauge, "Use when gauge is under or equal to");

                    break;

                case CustomComboPreset.WARPvP_BurstMode_Blota:
                    UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, "Before Primal Rend", "", 0);
                    UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, "After Primal Rend", "", 1);

                    break;
            }
        }
    }
}
