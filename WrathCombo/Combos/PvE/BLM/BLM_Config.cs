using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    internal static class Config
    {
        public static UserInt
            BLM_VariantCure = new("BLM_VariantCure"),
            BLM_VariantRampart = new("BLM_VariantRampart"),
            BLM_ST_LeyLinesCharges = new("BLM_ST_LeyLinesCharges", 1),
            BLM_ST_Triplecast_UseCharges = new("BLM_ST_Triplecast_UseCharges", 1),
            BLM_AoE_Triplecast_HoldCharges = new("BLM_AoE_Triplecast_HoldCharges", 0),
            BLM_AoE_LeyLinesCharges = new("BLM_AoE_LeyLinesCharges", 1),
            BLM_AoE_ThunderHP = new("BLM_AoE_ThunderHP", 5),
            BLM_SelectedOpener = new("BLM_SelectedOpener", 0),
            BLM_ST_Thunder_SubOption = new("BLM_ST_Thunder_SubOption", 1),
            BLM_Balance_Content = new("BLM_Balance_Content", 1);
        
        public static UserBoolArray
            BLM_ST_MovementOption = new("BLM_ST_MovementOption");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.BLM_ST_Opener:
                    DrawHorizontalRadioButton(BLM_SelectedOpener, "Standard opener", "Uses Standard opener",
                        0);

                    DrawHorizontalRadioButton(BLM_SelectedOpener, $"{Flare.ActionName()} opener", $"Uses {Flare.ActionName()} opener",
                        1);

                    DrawBossOnlyChoice(BLM_Balance_Content);
                    break;

                case CustomComboPreset.BLM_ST_LeyLines:
                    DrawSliderInt(0, 1, BLM_ST_LeyLinesCharges,
                        $"How many charges of {LeyLines.ActionName()} to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_ST_Triplecast:
                    DrawSliderInt(1, 2, BLM_ST_Triplecast_UseCharges, $"How many charges of {Triplecast.ActionName()} to use?");
                    break;

                case CustomComboPreset.BLM_ST_Movement:

                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Triplecast.ActionName()}", "", 4, 0);
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Paradox.ActionName()}", "", 4, 1);
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Role.Swiftcast.ActionName()}", "", 4, 2);
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Foul.ActionName()} / {Xenoglossy.ActionName()}", "", 4, 3);
                    break;

                case CustomComboPreset.BLM_ST_Thunder:
                    DrawHorizontalRadioButton(BLM_ST_Thunder_SubOption,
                        "All content", $"Uses {Thunder.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(BLM_ST_Thunder_SubOption,
                        "Boss encounters Only", $"Only uses {Thunder.ActionName()} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.BLM_AoE_LeyLines:
                    DrawSliderInt(0, 1, BLM_AoE_LeyLinesCharges,
                        $"How many charges of {LeyLines.ActionName()} to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_AoE_Triplecast:
                    DrawSliderInt(0, 1, BLM_AoE_Triplecast_HoldCharges, $"How many charges of {Triplecast.ActionName()} to keep ready? (0 = Use all)");
                    break;

                case CustomComboPreset.BLM_AoE_Thunder:
                    DrawSliderInt(0, 10, BLM_AoE_ThunderHP,
                        $"Stop Using {Thunder2.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.BLM_Variant_Cure:
                    DrawSliderInt(1, 100, BLM_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.BLM_Variant_Rampart:
                    DrawSliderInt(1, 100, BLM_VariantRampart, "HP% to be at or under", 200);

                    break;
            }
        }
    }
}
