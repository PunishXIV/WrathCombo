using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    internal static class Config
    {
        public static UserInt
            VPR_ST_SecondWind_Threshold = new("VPR_ST_SecondWindThreshold", 25),
            VPR_ST_Bloodbath_Threshold = new("VPR_ST_BloodbathThreshold", 40),
            VPR_AoE_SecondWind_Threshold = new("VPR_AoE_SecondWindThreshold", 25),
            VPR_AoE_Bloodbath_Threshold = new("VPR_AoE_BloodbathThreshold", 40),
            VPR_ST_UncoiledFury_HoldCharges = new("VPR_ST_UncoiledFury_HoldCharges", 1),
            VPR_AoE_UncoiledFury_HoldCharges = new("VPR_AoE_UncoiledFury_HoldCharges", 0),
            VPR_ST_UncoiledFury_Threshold = new("VPR_ST_UncoiledFury_Threshold", 1),
            VPR_AoE_UncoiledFury_Threshold = new("VPR_AoE_UncoiledFury_Threshold", 1),
            VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton"),
            VPR_VariantCure = new("VPR_VariantCure");

        public static UserFloat
            VPR_ST_Reawaken_Usage = new("VPR_ST_Reawaken_Usage", 2),
            VPR_AoE_Reawaken_Usage = new("VPR_AoE_Reawaken_Usage", 2);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.VPR_ST_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_ST_UncoiledFury_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");
                    DrawSliderInt(0, 5, VPR_ST_UncoiledFury_Threshold, "Set a HP% Threshold to use all charges.");

                    break;

                case CustomComboPreset.VPR_AoE_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_AoE_UncoiledFury_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");
                    DrawSliderInt(0, 5, VPR_AoE_UncoiledFury_Threshold, "Set a HP% Threshold to use all charges.");

                    break;

                case CustomComboPreset.VPR_ST_Reawaken:
                    DrawRoundedSliderFloat(0, 10, VPR_ST_Reawaken_Usage,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.", digits: 1);

                    break;

                case CustomComboPreset.VPR_AoE_Reawaken:
                    DrawRoundedSliderFloat(0, 10, VPR_AoE_Reawaken_Usage,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.", digits: 1);

                    break;

                case CustomComboPreset.VPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, VPR_ST_SecondWind_Threshold,
                        "Second Wind HP percentage threshold (0 = Disabled)");

                    DrawSliderInt(0, 100, VPR_ST_Bloodbath_Threshold,
                        "Bloodbath HP percentage threshold (0 = Disabled)");

                    break;

                case CustomComboPreset.VPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, VPR_AoE_SecondWind_Threshold,
                        "Second Wind HP percentage threshold (0 = Disabled)");

                    DrawSliderInt(0, 100, VPR_AoE_Bloodbath_Threshold,
                        "Bloodbath HP percentage threshold (0 = Disabled)");

                    break;

                case CustomComboPreset.VPR_ReawakenLegacy:
                    DrawRadioButton(VPR_ReawakenLegacyButton, "Replaces Reawaken",
                        "Replaces Reawaken with Full Generation - Legacy combo.", 0);

                    DrawRadioButton(VPR_ReawakenLegacyButton, "Replaces Steel Fangs",
                        "Replaces Steel Fangs with Full Generation - Legacy combo.", 1);

                    break;

                case CustomComboPreset.VPR_Variant_Cure:
                    DrawSliderInt(1, 100, VPR_VariantCure, "HP% to be at or under", 200);

                    break;
            }
        }
    }
}
