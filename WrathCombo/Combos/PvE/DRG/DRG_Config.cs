using Dalamud.Bindings.ImGui;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal static class Config
    {
        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.DRG_ST_Opener:
                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        "Standard opener", "Uses Standard opener",
                        0);

                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        $"{PiercingTalon.ActionName()} opener", $"Uses {PiercingTalon.ActionName()} opener",
                        1);

                    ImGui.NewLine();
                    DrawBossOnlyChoice(DRG_Balance_Content);
                    break;

                case CustomComboPreset.DRG_ST_Litany:
                    DrawHorizontalRadioButton(DRG_ST_Litany_SubOption,
                        "All content", $"Uses {BattleLitany.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(DRG_ST_Litany_SubOption,
                        "Boss encounters Only", $"Only uses {BattleLitany.ActionName()} when in Boss encounters.", 1);
                    break;

                case CustomComboPreset.DRG_ST_Lance:

                    DrawHorizontalRadioButton(DRG_ST_Lance_SubOption,
                        "All content", $"Uses {LanceCharge.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(DRG_ST_Lance_SubOption,
                        "Boss encounters Only", $"Only uses {LanceCharge.ActionName()} when in Boss encounters.", 1);
                    break;

                case CustomComboPreset.DRG_ST_HighJump:
                    DrawHorizontalMultiChoice(DRG_ST_Jump_Options,
                        "No movement", $"Only uses {Jump.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_Jump_Options,
                        "In Melee range", $"Only uses {Jump.ActionName()} when in melee range.", 2, 1);
                    break;

                case CustomComboPreset.DRG_ST_Mirage:
                    DrawAdditionalBoolChoice(DRG_ST_DoubleMirage,
                        "Burst Mirage Dive During LotD", "Adds Mirage Dive to the rotation when under Life of the Dragon.");
                    break;

                case CustomComboPreset.DRG_ST_DragonfireDive:
                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDive_Options,
                        "No movement", $"Only uses {DragonfireDive.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDive_Options,
                        "In Melee range", $"Only uses {DragonfireDive.ActionName()} when in melee range.", 2, 1);
                    break;

                case CustomComboPreset.DRG_ST_Stardiver:
                    DrawHorizontalMultiChoice(DRG_ST_Stardiver_Options,
                        "No movement", $"Only uses {Stardiver.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_Stardiver_Options,
                        "In Melee range", $"Only uses {Stardiver.ActionName()} when in melee range.", 2, 1);
                    break;

                case CustomComboPreset.DRG_ST_ComboHeals:
                    DrawSliderInt(0, 100, DRG_ST_SecondWind_Threshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, DRG_ST_Bloodbath_Threshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case CustomComboPreset.DRG_AoE_Litany:
                    DrawSliderInt(0, 100, DRG_AoE_LitanyHP,
                        $"Stop Using {BattleLitany.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case CustomComboPreset.DRG_AoE_Lance:
                    DrawSliderInt(0, 100, DRG_AoE_LanceChargeHP,
                        $"Stop Using {LanceCharge.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case CustomComboPreset.DRG_AoE_HighJump:
                    DrawHorizontalMultiChoice(DRG_AoE_Jump_Options,
                        "No movement", $"Only uses {Jump.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_Jump_Options,
                        "In Melee range", $"Only uses {Jump.ActionName()} when in melee range.", 2, 1);
                    break;

                case CustomComboPreset.DRG_AoE_DragonfireDive:
                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDive_Options,
                        "No movement", $"Only uses {DragonfireDive.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDive_Options,
                        "In Melee range", $"Only uses {DragonfireDive.ActionName()} when in melee range.", 2, 1);
                    break;

                case CustomComboPreset.DRG_AoE_Stardiver:
                    DrawHorizontalMultiChoice(DRG_AoE_Stardiver_Options,
                        "No movement", $"Only uses {Stardiver.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_Stardiver_Options,
                        "In Melee range", $"Only uses {Stardiver.ActionName()} when in melee range.", 2, 1);
                    break;

                case CustomComboPreset.DRG_AoE_ComboHeals:
                    DrawSliderInt(0, 100, DRG_AoE_SecondWind_Threshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, DRG_AoE_Bloodbath_Threshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case CustomComboPreset.DRG_Variant_Cure:
                    DrawSliderInt(1, 100, DRG_Variant_Cure,
                        "HP% to be at or under", 200);
                    break;
            }
        }

        #region Variables

        public static UserInt
            DRG_SelectedOpener = new("DRG_SelectedOpener", 0),
            DRG_Balance_Content = new("DRG_Balance_Content", 1),
            DRG_ST_Litany_SubOption = new("DRG_ST_Litany_SubOption", 1),
            DRG_ST_Lance_SubOption = new("DRG_ST_Lance_SubOption", 1),
            DRG_ST_SecondWind_Threshold = new("DRG_STSecondWindThreshold", 40),
            DRG_ST_Bloodbath_Threshold = new("DRG_STBloodbathThreshold", 30),
            DRG_AoE_LitanyHP = new("DRG_AoE_LitanyHP", 20),
            DRG_AoE_LanceChargeHP = new("DRG_AoE_LanceChargeHP", 20),
            DRG_AoE_SecondWind_Threshold = new("DRG_AoE_SecondWindThreshold", 40),
            DRG_AoE_Bloodbath_Threshold = new("DRG_AoE_BloodbathThreshold", 30),
            DRG_Variant_Cure = new("DRG_Variant_Cure", 50);

        public static UserBool
            DRG_ST_DoubleMirage = new("DRG_ST_DoubleMirage");

        public static UserBoolArray
            DRG_ST_Jump_Options = new("DRG_ST_Jump_Options"),
            DRG_ST_DragonfireDive_Options = new("DRG_ST_DragonfireDive_Options"),
            DRG_ST_Stardiver_Options = new("DRG_ST_Stardiver_Options"),
            DRG_AoE_Jump_Options = new("DRG_AoE_Jump_Options"),
            DRG_AoE_DragonfireDive_Options = new("DRG_AoE_DragonfireDive_Options"),
            DRG_AoE_Stardiver_Options = new("DRG_AoE_Stardiver_Options");

        #endregion
    }
}
