using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    internal static class Config
    {
        public static UserInt
            VPR_ST_SecondWind_Threshold = new("VPR_ST_SecondWindThreshold", 25),
            VPR_ST_SerpentsIre_SubOption = new("VPR_ST_SerpentsIre_SubOption", 1),
            VPR_ST_Bloodbath_Threshold = new("VPR_ST_BloodbathThreshold", 40),
            VPR_AoE_SecondWind_Threshold = new("VPR_AoE_SecondWindThreshold", 25),
            VPR_AoE_Bloodbath_Threshold = new("VPR_AoE_BloodbathThreshold", 40),
            VPR_ST_UncoiledFury_HoldCharges = new("VPR_ST_UncoiledFury_HoldCharges", 1),
            VPR_AoE_UncoiledFury_HoldCharges = new("VPR_AoE_UncoiledFury_HoldCharges", 0),
            VPR_ST_UncoiledFury_Threshold = new("VPR_ST_UncoiledFury_Threshold", 1),
            VPR_AoE_UncoiledFury_Threshold = new("VPR_AoE_UncoiledFury_Threshold", 1),
            VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton"),
            VPR_VariantCure = new("VPR_VariantCure"),
            VPR_Balance_Content = new("VPR_Balance_Content", 1);

        public static UserFloat
            VPR_ST_Reawaken_Usage = new("VPR_ST_Reawaken_Usage", 2),
            VPR_AoE_Reawaken_Usage = new("VPR_AoE_Reawaken_Usage", 2);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.VPR_ST_Opener:
                    DrawBossOnlyChoice(VPR_Balance_Content);
                    break;

                case CustomComboPreset.VPR_ST_SerpentsIre:
                    DrawHorizontalRadioButton(VPR_ST_SerpentsIre_SubOption,
                        "All content", $"Uses {SerpentsIre.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(VPR_ST_SerpentsIre_SubOption,
                        "Boss encounters Only", $"Only uses  {SerpentsIre.ActionName()} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.VPR_ST_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_ST_UncoiledFury_HoldCharges,
                        $"How many charges of {UncoiledFury.ActionName()} to keep ready? (0 = Use all)");
                    DrawSliderInt(0, 5, VPR_ST_UncoiledFury_Threshold, $"Set a HP% Threshold to use all charges of {UncoiledFury.ActionName()}.");

                    break;

                case CustomComboPreset.VPR_AoE_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_AoE_UncoiledFury_HoldCharges,
                        $"How many charges of {UncoiledFury.ActionName()} to keep ready? (0 = Use all)");
                    DrawSliderInt(0, 5, VPR_AoE_UncoiledFury_Threshold, $"Set a HP% Threshold to use all charges of {UncoiledFury.ActionName()}.");

                    break;

                case CustomComboPreset.VPR_ST_Reawaken:
                    DrawRoundedSliderFloat(0, 10, VPR_ST_Reawaken_Usage,
                        $"Stop using {Reawaken.ActionName()} at Enemy HP %. Set to Zero to disable this check.",
                        digits: 1);

                    break;

                case CustomComboPreset.VPR_AoE_Reawaken:
                    DrawRoundedSliderFloat(0, 10, VPR_AoE_Reawaken_Usage,
                        $"Stop using {Reawaken.ActionName()} at Enemy HP %. Set to Zero to disable this check.",
                        digits: 1);

                    break;

                case CustomComboPreset.VPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, VPR_ST_SecondWind_Threshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, VPR_ST_Bloodbath_Threshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.VPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, VPR_AoE_SecondWind_Threshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, VPR_AoE_Bloodbath_Threshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.VPR_ReawakenLegacy:
                    DrawRadioButton(VPR_ReawakenLegacyButton, $"Replaces {Reawaken.ActionName()}",
                        $"Replaces {Reawaken.ActionName()} with Full Generation - Legacy combo.", 0);

                    DrawRadioButton(VPR_ReawakenLegacyButton, $"Replaces {ReavingFangs.ActionName()}",
                        $"Replaces {ReavingFangs.ActionName()} with Full Generation - Legacy combo.", 1);

                    break;

                case CustomComboPreset.VPR_Variant_Cure:
                    DrawSliderInt(1, 100, VPR_VariantCure, "HP% to be at or under", 200);

                    break;

            }
        }
    }
}
