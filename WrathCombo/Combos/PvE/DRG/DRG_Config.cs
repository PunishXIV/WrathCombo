using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.DRG_ST_Opener:
                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        "Standard opener", "Uses Standard opener",
                        0);

                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        $"{PiercingTalon.ActionName()} opener", $"Uses {PiercingTalon.ActionName()} opener",
                        1);

                    ImGui.NewLine();
                    DrawBossOnlyChoice(DRG_Balance_Content);
                    break;

                case Preset.DRG_ST_Litany:
                    DrawHorizontalRadioButton(DRG_ST_LitanyBossOption,
                        "All content", $"Uses {BattleLitany.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(DRG_ST_LitanyBossOption,
                        "Boss encounters Only", $"Only uses {BattleLitany.ActionName()} when in Boss encounters.", 1);
                    break;

                case Preset.DRG_ST_Lance:

                    DrawHorizontalRadioButton(DRG_ST_LanceBossOption,
                        "All content", $"Uses {LanceCharge.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(DRG_ST_LanceBossOption,
                        "Boss encounters Only", $"Only uses {LanceCharge.ActionName()} when in Boss encounters.", 1);
                    break;

                case Preset.DRG_ST_HighJump:
                    DrawHorizontalMultiChoice(DRG_ST_JumpMovingOptions,
                        "No movement", $"Only uses {Jump.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_JumpMovingOptions,
                        "In Melee range", $"Only uses {Jump.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_ST_Mirage:
                    DrawAdditionalBoolChoice(DRG_ST_DoubleMirage,
                        "Burst Mirage Dive During LotD", "Adds Mirage Dive to the rotation when under Life of the Dragon.");
                    break;

                case Preset.DRG_ST_DragonfireDive:
                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDiveMovingOptions,
                        "No movement", $"Only uses {DragonfireDive.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDiveMovingOptions,
                        "In Melee range", $"Only uses {DragonfireDive.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_ST_Stardiver:
                    DrawHorizontalMultiChoice(DRG_ST_StardiverMovingOptions,
                        "No movement", $"Only uses {Stardiver.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_StardiverMovingOptions,
                        "In Melee range", $"Only uses {Stardiver.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_ST_ComboHeals:
                    DrawSliderInt(0, 100, DRG_ST_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, DRG_ST_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.DRG_AoE_Litany:
                    DrawSliderInt(0, 100, DRG_AoE_LitanyHPTreshold,
                        $"Stop Using {BattleLitany.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.DRG_AoE_Lance:
                    DrawSliderInt(0, 100, DRG_AoE_LanceChargeHPTreshold,
                        $"Stop Using {LanceCharge.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.DRG_AoE_HighJump:
                    DrawHorizontalMultiChoice(DRG_AoE_JumpMovingOptions,
                        "No movement", $"Only uses {Jump.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_JumpMovingOptions,
                        "In Melee range", $"Only uses {Jump.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_AoE_DragonfireDive:
                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDiveMovingOptions,
                        "No movement", $"Only uses {DragonfireDive.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDiveMovingOptions,
                        "In Melee range", $"Only uses {DragonfireDive.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_AoE_Stardiver:
                    DrawHorizontalMultiChoice(DRG_AoE_StardiverMovingOptions,
                        "No movement", $"Only uses {Stardiver.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_StardiverMovingOptions,
                        "In Melee range", $"Only uses {Stardiver.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_AoE_ComboHeals:
                    DrawSliderInt(0, 100, DRG_AoE_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, DRG_AoE_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.DRG_HeavensThrust:
                    DrawAdditionalBoolChoice(DRG_Heavens_Basic,
                        "Add Chaos Combo", "Adds Chaos combo when applicable.");
                    break;
            }
        }

        #region Variables

        public static UserInt
            DRG_SelectedOpener = new("DRG_SelectedOpener", 0),
            DRG_Balance_Content = new("DRG_Balance_Content", 1),
            DRG_ST_LitanyBossOption = new("DRG_ST_Litany_SubOption", 1),
            DRG_ST_LanceBossOption = new("DRG_ST_Lance_SubOption", 1),
            DRG_ST_SecondWindHPThreshold = new("DRG_STSecondWindThreshold", 40),
            DRG_ST_BloodbathHPThreshold = new("DRG_STBloodbathThreshold", 30),
            DRG_AoE_LitanyHPTreshold = new("DRG_AoE_LitanyHP", 20),
            DRG_AoE_LanceChargeHPTreshold = new("DRG_AoE_LanceChargeHP", 20),
            DRG_AoE_SecondWindHPThreshold = new("DRG_AoE_SecondWindThreshold", 40),
            DRG_AoE_BloodbathHPThreshold = new("DRG_AoE_BloodbathThreshold", 30);

        public static UserBool
            DRG_ST_DoubleMirage = new("DRG_ST_DoubleMirage"),
            DRG_Heavens_Basic = new("DRG_Heavens_Basic");

        public static UserBoolArray
            DRG_ST_JumpMovingOptions = new("DRG_ST_Jump_Options"),
            DRG_ST_DragonfireDiveMovingOptions = new("DRG_ST_DragonfireDive_Options"),
            DRG_ST_StardiverMovingOptions = new("DRG_ST_Stardiver_Options"),
            DRG_AoE_JumpMovingOptions = new("DRG_AoE_Jump_Options"),
            DRG_AoE_DragonfireDiveMovingOptions = new("DRG_AoE_DragonfireDive_Options"),
            DRG_AoE_StardiverMovingOptions = new("DRG_AoE_Stardiver_Options");

        #endregion
    }
}
