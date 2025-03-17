using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal static class Config
    {
        public static UserInt
            MNK_ST_Brotherhood_SubOption = new("MNK_ST_Brotherhood_SubOption", 1),
            MNK_ST_RiddleOfFire_SubOption = new("MNK_ST_RiddleOfFire_SubOption", 1),
            MNK_ST_RiddleOfWind_SubOption = new("MNK_ST_RiddleOfWind_SubOption", 1),
            MNK_AoE_Brotherhood_HP = new("MNK_AoE_Brotherhood_HP", 5),
            MNK_AoE_RiddleOfWind_HP = new("MNK_AoE_RiddleOfWind_HP", 5),
            MNK_AoE_RiddleOfFire_HP = new("MNK_AoE_RiddleOfFire_HP", 5),
            MNK_ST_SecondWind_Threshold = new("MNK_ST_SecondWindThreshold", 25),
            MNK_ST_Bloodbath_Threshold = new("MNK_ST_BloodbathThreshold", 40),
            MNK_AoE_SecondWind_Threshold = new("MNK_AoE_SecondWindThreshold", 25),
            MNK_AoE_Bloodbath_Threshold = new("MNK_AoE_BloodbathThreshold", 40),
            MNK_VariantCure = new("MNK_Variant_Cure"),
            MNK_SelectedOpener = new("MNK_SelectedOpener", 0),
            MNK_Balance_Content = new("MNK_Balance_Content", 1);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.MNK_ST_ComboHeals:
                    DrawSliderInt(0, 100, MNK_ST_SecondWind_Threshold,
                        $"HP percent threshold to use {All.SecondWind.ActionName()} (0 = Disabled)");

                    DrawSliderInt(0, 100, MNK_ST_Bloodbath_Threshold,
                        $"HP percent threshold to use {All.Bloodbath.ActionName()} (0 = Disabled)");

                    break;

                case CustomComboPreset.MNK_AoE_ComboHeals:
                    DrawSliderInt(0, 100, MNK_AoE_SecondWind_Threshold,
                        $"HP percent threshold to use {All.SecondWind.ActionName()} (0 = Disabled)");

                    DrawSliderInt(0, 100, MNK_AoE_Bloodbath_Threshold,
                        $"HP percent threshold to use {All.Bloodbath.ActionName()} (0 = Disabled)");

                    break;

                case CustomComboPreset.MNK_STUseBrotherhood:
                    DrawHorizontalRadioButton(MNK_ST_Brotherhood_SubOption,
                        "All content", $"Uses {ActionWatching.GetActionName(Brotherhood)} regardless of content.", 0);

                    DrawHorizontalRadioButton(MNK_ST_Brotherhood_SubOption,
                        "Boss encounters Only", $"Only uses {ActionWatching.GetActionName(Brotherhood)} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MNK_STUseROF:
                    DrawHorizontalRadioButton(MNK_ST_RiddleOfFire_SubOption,
                        "All content", $"Uses {ActionWatching.GetActionName(RiddleOfFire)} regardless of content.", 0);

                    DrawHorizontalRadioButton(MNK_ST_RiddleOfFire_SubOption,
                        "Boss encounters Only", $"Only uses {ActionWatching.GetActionName(RiddleOfFire)} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MNK_STUseROW:
                    DrawHorizontalRadioButton(MNK_ST_RiddleOfWind_SubOption,
                        "All content", $"Uses {ActionWatching.GetActionName(RiddleOfWind)} regardless of content.", 0);

                    DrawHorizontalRadioButton(MNK_ST_RiddleOfWind_SubOption,
                        "Boss encounters Only", $"Only uses {ActionWatching.GetActionName(RiddleOfWind)} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MNK_AoEUseBrotherhood:
                    DrawSliderInt(0, 100, MNK_AoE_Brotherhood_HP,
                        $"Stop Using {Brotherhood.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_AoEUseROF:
                    DrawSliderInt(0, 100, MNK_AoE_RiddleOfFire_HP,
                        $"Stop Using {RiddleOfFire.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_AoEUseROW:
                    DrawSliderInt(0, 100, MNK_AoE_RiddleOfWind_HP,
                        $"Stop Using {RiddleOfWind.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MNK_STUseOpener:
                    DrawHorizontalRadioButton(MNK_SelectedOpener, "Double Lunar", "Uses Lunar/Lunar opener",
                        0);

                    DrawHorizontalRadioButton(MNK_SelectedOpener, "Solar Lunar", "Uses Solar/Lunar opener",
                        1);

                    ImGui.NewLine();
                    DrawBossOnlyChoice(MNK_Balance_Content);

                    break;

                case CustomComboPreset.MNK_Variant_Cure:
                    DrawSliderInt(1, 100, MNK_VariantCure, "HP% to be at or under", 200);

                    break;
            }
        }
    }
}
