using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
    internal static class Config
    {
        public const string
            BRD_RagingJawsRenewTime = "ragingJawsRenewTime",
            BRD_NoWasteHPPercentage = "noWasteHpPercentage",
            BRD_AoENoWasteHPPercentage = "AoENoWasteHpPercentage",
            BRD_STSecondWindThreshold = "BRD_STSecondWindThreshold",
            BRD_AoESecondWindThreshold = "BRD_AoESecondWindThreshold",
            BRD_VariantCure = "BRD_VariantCure";

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.BRD_Adv_RagingJaws:
                    DrawSliderInt(3, 10, BRD_RagingJawsRenewTime,
                        "Remaining time (In seconds). Recommended 5, increase little by little if refresh is outside of radiant window");

                    break;

                case CustomComboPreset.BRD_Adv_NoWaste:
                    DrawSliderInt(1, 10, BRD_NoWasteHPPercentage, "Remaining target HP percentage");

                    break;

                case CustomComboPreset.BRD_AoE_Adv_NoWaste:
                    DrawSliderInt(1, 10, BRD_AoENoWasteHPPercentage,
                        "Remaining target HP percentage");

                    break;

                case CustomComboPreset.BRD_ST_SecondWind:
                    DrawSliderInt(0, 100, BRD_STSecondWindThreshold,
                        "HP percent threshold to use Second Wind below.");

                    break;

                case CustomComboPreset.BRD_AoE_SecondWind:
                    DrawSliderInt(0, 100, BRD_AoESecondWindThreshold,
                        "HP percent threshold to use Second Wind below.");

                    break;

                case CustomComboPreset.BRD_Variant_Cure:
                    DrawSliderInt(1, 100, BRD_VariantCure, "HP% to be at or under", 200);

                    break;
            }
        }
    }
}
