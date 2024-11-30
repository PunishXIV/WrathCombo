using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal static class Config
    {
        public static UserInt
            MNK_ST_Brotherhood_HP = new("MNK_ST_Brotherhood_HP", 0),
            MNK_ST_RiddleOfFire_HP = new("MNK_ST_RiddleOfFire_HP", 0),
            MNK_AoE_Brotherhood_HP = new("MNK_AoE_Brotherhood_HP", 0),
            MNK_AoE_RiddleOfFire_HP = new("MNK_AoE_RiddleOfFire_HP", 0),
            MNK_ST_SecondWind_Threshold = new("MNK_ST_SecondWindThreshold", 25),
            MNK_ST_Bloodbath_Threshold = new("MNK_ST_BloodbathThreshold", 40),
            MNK_AoE_SecondWind_Threshold = new("MNK_AoE_SecondWindThreshold", 25),
            MNK_AoE_Bloodbath_Threshold = new("MNK_AoE_BloodbathThreshold", 40),
            MNK_VariantCure = new("MNK_Variant_Cure"),
            MNK_SelectedOpener = new("MNK_SelectedOpener");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.MNK_ST_ComboHeals:
                    DrawSliderInt(0, 100, MNK_ST_SecondWind_Threshold,
                        "Second Wind HP percentage threshold (0 = Disabled)");

                    DrawSliderInt(0, 100, MNK_ST_Bloodbath_Threshold,
                        "Bloodbath HP percentage threshold (0 = Disabled)");

                    break;

                case CustomComboPreset.MNK_AoE_ComboHeals:
                    DrawSliderInt(0, 100, MNK_AoE_SecondWind_Threshold,
                        "Second Wind HP percentage threshold (0 = Disabled)");

                    DrawSliderInt(0, 100, MNK_AoE_Bloodbath_Threshold,
                        "Bloodbath HP percentage threshold (0 = Disabled)");

                    break;

                case CustomComboPreset.MNK_STUseBrotherhood:
                    DrawSliderInt(0, 100, MNK_ST_Brotherhood_HP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_STUseROW:
                    DrawSliderInt(0, 100, MNK_ST_RiddleOfFire_HP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_AoEUseBrotherhood:
                    DrawSliderInt(0, 100, MNK_AoE_Brotherhood_HP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_AoEUseROW:
                    DrawSliderInt(0, 100, MNK_AoE_RiddleOfFire_HP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_STUseOpener:
                    DrawHorizontalRadioButton(MNK_SelectedOpener, "Double Lunar", "Uses Lunar/Lunar opener",
                        0);

                    DrawHorizontalRadioButton(MNK_SelectedOpener, "Solar Lunar", "Uses Solar/Lunar opener",
                        1);

                    break;

                case CustomComboPreset.MNK_Variant_Cure:
                    DrawSliderInt(1, 100, MNK_VariantCure, "HP% to be at or under", 200);

                    break;
            }
        }
    }
}