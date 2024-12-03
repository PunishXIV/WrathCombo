using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal static class Config
    {
        public static UserInt
            DRG_Variant_Cure = new("DRG_VariantCure"),
            DRG_ST_LitanyHP = new("DRG_ST_LitanyHP", 0),
            DRG_ST_LanceChargeHP = new("DRG_ST_LanceChargeHP", 0),
            DRG_ST_SecondWind_Threshold = new("DRG_STSecondWindThreshold", 25),
            DRG_ST_Bloodbath_Threshold = new("DRG_STBloodbathThreshold", 40),
            DRG_AoE_LitanyHP = new("DRG_AoE_LitanyHP", 5),
            DRG_AoE_LanceChargeHP = new("DRG_AoE_LanceChargeHP", 5),
            DRG_AoE_SecondWind_Threshold = new("DRG_AoE_SecondWindThreshold", 25),
            DRG_AoE_Bloodbath_Threshold = new("DRG_AoE_BloodbathThreshold", 40);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.DRG_ST_ComboHeals:
                    DrawSliderInt(0, 100, DRG_ST_SecondWind_Threshold,
                        "Second Wind HP percentage threshold (0 = Disabled)");

                    DrawSliderInt(0, 100, DRG_ST_Bloodbath_Threshold,
                        "Bloodbath HP percentage threshold (0 = Disabled)");

                    break;

                case CustomComboPreset.DRG_AoE_ComboHeals:
                    DrawSliderInt(0, 100, DRG_AoE_SecondWind_Threshold,
                        "Second Wind HP percentage threshold (0 = Disabled)");

                    DrawSliderInt(0, 100, DRG_AoE_Bloodbath_Threshold,
                        "Bloodbath HP percentage threshold (0 = Disabled)");

                    break;

                case CustomComboPreset.DRG_Variant_Cure:
                    DrawSliderInt(1, 100, DRG_Variant_Cure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.DRG_ST_Litany:
                    DrawSliderInt(0, 100, DRG_ST_LitanyHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.DRG_ST_Lance:
                    DrawSliderInt(0, 100, DRG_ST_LanceChargeHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.DRG_AoE_Litany:
                    DrawSliderInt(0, 100, DRG_AoE_LitanyHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.DRG_AoE_Lance:
                    DrawSliderInt(0, 100, DRG_AoE_LanceChargeHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.DRGPvP_Nastrond:
                    DrawSliderInt(0, 100, DRGPvP.Config.DRGPvP_LOTD_HPValue,
                        "Ends Life of the Dragon if HP falls below the set percentage");

                    DrawSliderInt(2, 8, DRGPvP.Config.DRGPvP_LOTD_Duration,
                        "Seconds remaining of Life of the Dragon buff before using Nastrond if you are still above the set HP percentage.");

                    break;

                case CustomComboPreset.DRGPvP_ChaoticSpringSustain:
                    DrawSliderInt(0, 101, DRGPvP.Config.DRGPvP_CS_HP_Threshold,
                        "Chaotic Spring HP percentage threshold. Set to 100 to use on cd");

                    break;

                case CustomComboPreset.DRGPvP_WyrmwindThrust:
                    DrawSliderInt(0, 20, DRGPvP.Config.DRGPvP_Distance_Threshold,
                        "Minimum Distance to use Wyrmwind Thrust. Maximum damage at 15 or more");

                    break;
            }
        }
    }
}