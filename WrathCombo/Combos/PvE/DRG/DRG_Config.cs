using Dalamud.Interface.Colors;
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
                        "Standard opener", "Uses Standard opener", 0);

                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        $"{PiercingTalon.ActionName()} opener", $"Uses {PiercingTalon.ActionName()} opener", 1);
                    ImGui.NewLine();
                    DrawBossOnlyChoice(DRG_BalanceContent);
                    break;

                case Preset.DRG_ST_BattleLitany:
                    DrawSliderInt(0, 50, DRG_ST_BattleLitanyHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(DRG_ST_BattleLitanyBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(DRG_ST_BattleLitanyBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    ImGui.Unindent();
                    break;

                case Preset.DRG_ST_LanceCharge:
                    DrawSliderInt(0, 50, DRG_ST_LanceChargeHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(DRG_ST_LanceChargeBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(DRG_ST_LanceChargeBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    ImGui.Unindent();
                    break;

                case Preset.DRG_ST_HighJump:
                    DrawHorizontalMultiChoice(DRG_ST_JumpMovingOrInRanged,
                        "No movement", $"Only uses {Jump.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_JumpMovingOrInRanged,
                        "In Melee range", $"Only uses {Jump.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_ST_Mirage:
                    DrawAdditionalBoolChoice(DRG_ST_DoubleMirage,
                        "Burst Mirage Dive During LotD", "Adds Mirage Dive to the rotation when under Life of the Dragon.\nWorks best on 2.50 GCD.");
                    break;

                case Preset.DRG_ST_Geirskogul:
                    DrawSliderInt(0, 100, DRG_ST_GeirskogulBossOption,
                        "Bosses Only. Stop using at Enemy HP %.");

                    DrawSliderInt(0, 100, DRG_ST_GeirskogulBossAddsOption,
                        "Boss Encounter Non Bosses. Stop using at Enemy HP %.");

                    DrawSliderInt(0, 100, DRG_ST_GeirskogulTrashOption,
                        "Non boss encounter. Stop using at Enemy HP %.");
                    break;

                case Preset.DRG_ST_DragonfireDive:
                    DrawSliderInt(0, 50, DRG_ST_DragonfireDiveHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(DRG_ST_DragonfireDiveBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(DRG_ST_DragonfireDiveBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    ImGui.Unindent();

                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDiveMovingOrInRanged,
                        "No movement", $"Only uses {DragonfireDive.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDiveMovingOrInRanged,
                        "In Melee range", $"Only uses {DragonfireDive.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_ST_Stardiver:
                    DrawHorizontalMultiChoice(DRG_ST_StardiverMovingOrInRanged,
                        "No movement", $"Only uses {Stardiver.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_StardiverMovingOrInRanged,
                        "In Melee range", $"Only uses {Stardiver.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_TrueNorthDynamic:
                    DrawSliderInt(0, 1, DRG_ManualTN,
                        "How many charges to keep for manual usage.");
                    break;

                case Preset.DRG_ST_ComboHeals:
                    DrawSliderInt(0, 100, DRG_ST_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, DRG_ST_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.DRG_AoE_BattleLitany:
                    DrawSliderInt(0, 100, DRG_AoE_BattleLitanyHPTreshold,
                        "Stop using when target HP% is at or below (Set to 0 to Disable This Check)");
                    break;

                case Preset.DRG_AoE_LanceCharge:
                    DrawSliderInt(0, 100, DRG_AoE_LanceChargeHPTreshold,
                        "Stop using when target HP% is at or below (Set to 0 to Disable This Check)");
                    break;

                case Preset.DRG_AoE_Geirskogul:
                    DrawSliderInt(0, 100, DRG_AoE_GeirskogulHPTreshold,
                        "Stop using when target HP% is at or below (Set to 0 to Disable This Check)");
                    break;

                case Preset.DRG_AoE_HighJump:
                    DrawHorizontalMultiChoice(DRG_AoE_JumpMovingOrInRanged,
                        "No movement", $"Only uses {Jump.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_JumpMovingOrInRanged,
                        "In Melee range", $"Only uses {Jump.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_AoE_DragonfireDive:
                    DrawSliderInt(0, 100, DRG_AoE_DragonfireDiveHPTreshold,
                        "Stop using when target HP% is at or below (Set to 0 to Disable This Check)");

                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDiveMovingOrInRanged,
                        "No movement", $"Only uses {DragonfireDive.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDiveMovingOrInRanged,
                        "In Melee range", $"Only uses {DragonfireDive.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_AoE_Stardiver:
                    DrawHorizontalMultiChoice(DRG_AoE_StardiverMovingOrInRanged,
                        "No movement", $"Only uses {Stardiver.ActionName()} when not moving.", 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_StardiverMovingOrInRanged,
                        "In Melee range", $"Only uses {Stardiver.ActionName()} when in melee range.", 2, 1);
                    break;

                case Preset.DRG_AoE_ComboHeals:
                    DrawSliderInt(0, 100, DRG_AoE_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, DRG_AoE_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.DRG_HeavensThrust:
                    DrawAdditionalBoolChoice(DRG_ChaoticCombo,
                        "Add Chaos Combo", "Adds Chaos combo when applicable.");
                    break;
            }
        }

        #region Variables

        public static UserInt
            DRG_SelectedOpener = new("DRG_SelectedOpener"),
            DRG_BalanceContent = new("DRG_BalanceContent", 1),
            DRG_ST_BattleLitanyHPOption = new("DRG_ST_BattleLitanyHPOption", 25),
            DRG_ST_BattleLitanyBossOption = new("DRG_ST_BattleLitanyBossOption"),
            DRG_ST_LanceChargeHPOption = new("DRG_ST_LanceChargeHPOption", 25),
            DRG_ST_LanceChargeBossOption = new("DRG_ST_LanceChargeBossOption"),
            DRG_ST_GeirskogulBossOption = new("DRG_ST_GeirskogulBossOption"),
            DRG_ST_GeirskogulBossAddsOption = new("DRG_ST_GeirskogulBossAddsOption", 10),
            DRG_ST_GeirskogulTrashOption = new("DRG_ST_GeirskogulTrashOption", 25),
            DRG_ST_DragonfireDiveHPOption = new("DRG_ST_DragonfireDiveHPOption", 25),
            DRG_ST_DragonfireDiveBossOption = new("DRG_ST_DragonfireDiveBossOption"),
            DRG_ManualTN = new("DRG_ManualTN"),
            DRG_ST_SecondWindHPThreshold = new("DRG_ST_SecondWindHPThreshold", 40),
            DRG_ST_BloodbathHPThreshold = new("DRG_ST_BloodbathHPThreshold", 30),
            DRG_AoE_BattleLitanyHPTreshold = new("DRG_AoE_BattleLitanyHPTreshold", 25),
            DRG_AoE_LanceChargeHPTreshold = new("DRG_AoE_LanceChargeHPTreshold", 25),
            DRG_AoE_GeirskogulHPTreshold = new("DRG_AoE_GeirskogulHPTreshold", 25),
            DRG_AoE_DragonfireDiveHPTreshold = new("DRG_AoE_DragonfireDiveHPTreshold", 25),
            DRG_AoE_SecondWindHPThreshold = new("DRG_AoE_SecondWindHPThreshold", 40),
            DRG_AoE_BloodbathHPThreshold = new("DRG_AoE_BloodbathHPThreshold", 30);

        public static UserBool
            DRG_ST_DoubleMirage = new("DRG_ST_DoubleMirage"),
            DRG_ChaoticCombo = new("DRG_ChaoticCombo");

        public static UserBoolArray
            DRG_ST_JumpMovingOrInRanged = new("DRG_ST_JumpMovingOrInRanged"),
            DRG_ST_DragonfireDiveMovingOrInRanged = new("DRG_ST_DragonfireDiveMovingOrInRanged"),
            DRG_ST_StardiverMovingOrInRanged = new("DRG_ST_StardiverMovingOrInRanged"),
            DRG_AoE_JumpMovingOrInRanged = new("DRG_AoE_JumpMovingOrInRanged"),
            DRG_AoE_DragonfireDiveMovingOrInRanged = new("DRG_AoE_DragonfireDiveMovingOrInRanged"),
            DRG_AoE_StardiverMovingOrInRanged = new("DRG_AoE_StardiverMovingOrInRanged");

        #endregion
    }
}
