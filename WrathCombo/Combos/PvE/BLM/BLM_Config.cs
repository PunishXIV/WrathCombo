using Dalamud.Interface.Colors;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    internal static class Config
    {
        public static UserInt
            BLM_SelectedOpener = new("BLM_SelectedOpener", 0),
            BLM_Balance_Content = new("BLM_Balance_Content", 1),
            BLM_ST_LeyLinesCharges = new("BLM_ST_LeyLinesCharges", 1),
            BLM_ST_ThunderOption = new("BLM_ST_ThunderOption", 10),
            BLM_ST_Thunder_SubOption = new("BLM_ST_Thunder_SubOption", 0),
            BLM_ST_ThunderUptime_Threshold = new("BLM_ST_ThunderUptime_Threshold", 5),
            BLM_ST_Triplecast_Movement = new("BLM_ST_Triplecast_Movement", 1),
            BLM_ST_Polyglot_Movement = new("BLM_ST_Polyglot_Movement", 1),
            BLM_ST_Polyglot_Save = new("BLM_ST_Polyglot_Save", 0),
            BLM_ST_Manaward_Threshold = new("BLM_ST_Manaward_Threshold", 40),
            BLM_AoE_Triplecast_HoldCharges = new("BLM_AoE_Triplecast_HoldCharges", 0),
            BLM_AoE_LeyLinesCharges = new("BLM_AoE_LeyLinesCharges", 1),
            BLM_AoE_ThunderHP = new("BLM_AoE_ThunderHP", 20),
            BLM_VariantCure = new("BLM_VariantCure", 50);

        public static UserBoolArray
            BLM_ST_MovementOption = new("BLM_ST_MovementOption");

        public static UserIntArray
            BLM_ST_Movement_Priority = new("BLM_ST_Movement_Priority");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.BLM_ST_Opener:
                    DrawHorizontalRadioButton(BLM_SelectedOpener,
                        "Standard opener", "Uses Standard opener",
                        0);

                    DrawHorizontalRadioButton(BLM_SelectedOpener,
                        $"{Flare.ActionName()} opener", $"Uses {Flare.ActionName()} opener",
                        1);

                    DrawBossOnlyChoice(BLM_Balance_Content);
                    break;

                case CustomComboPreset.BLM_ST_LeyLines:
                    DrawSliderInt(0, 1, BLM_ST_LeyLinesCharges,
                        $"How many charges of {LeyLines.ActionName()} to keep ready?");

                    break;

                case CustomComboPreset.BLM_ST_Movement:
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Triplecast.ActionName()}", "", 4, 0);
                    DrawPriorityInput(BLM_ST_Movement_Priority, 4, 0, $"{Triplecast.ActionName()} Priority: ");
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Paradox.ActionName()}", "", 4, 1);
                    DrawPriorityInput(BLM_ST_Movement_Priority, 4, 1, $"{Paradox.ActionName()} Priority: ");
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Role.Swiftcast.ActionName()}", "", 4, 2);
                    DrawPriorityInput(BLM_ST_Movement_Priority, 4, 2, $"{Role.Swiftcast.ActionName()} Priority: ");
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption, $"Use {Foul.ActionName()} / {Xenoglossy.ActionName()}", "", 4, 3);
                    DrawPriorityInput(BLM_ST_Movement_Priority, 4, 3, $"{Xenoglossy.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.BLM_ST_UsePolyglot:
                    if (DrawSliderInt(0, 3, BLM_ST_Polyglot_Save,
                        "How many charges to save for manual use?"))
                        if (BLM_ST_Polyglot_Movement > 3 - BLM_ST_Polyglot_Save)
                            BLM_ST_Polyglot_Movement.Value = 3 - BLM_ST_Polyglot_Save;

                    if (DrawSliderInt(0, 3, BLM_ST_Polyglot_Movement,
                        "How many charges to save for movement?"))
                        if (BLM_ST_Polyglot_Save > 3 - BLM_ST_Polyglot_Movement)
                            BLM_ST_Polyglot_Save.Value = 3 - BLM_ST_Polyglot_Movement;

                    break;

                case CustomComboPreset.BLM_ST_Triplecast:
                    if (BLM_ST_MovementOption[0])
                        DrawSliderInt(1, 2, BLM_ST_Triplecast_Movement,
                            "How many charges to save for movement?");
                    break;


                case CustomComboPreset.BLM_ST_Thunder:

                    DrawSliderInt(0, 50, BLM_ST_ThunderOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(BLM_ST_Thunder_SubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(BLM_ST_Thunder_SubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    DrawSliderInt(0, 5, BLM_ST_ThunderUptime_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.");

                    ImGui.Unindent();

                    break;

                case CustomComboPreset.BLM_ST_Manaward:
                    DrawSliderInt(0, 100, BLM_ST_Manaward_Threshold,
                        $"{Manaward.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.BLM_AoE_LeyLines:
                    DrawSliderInt(0, 1, BLM_AoE_LeyLinesCharges,
                        $"How many charges of {LeyLines.ActionName()} to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_AoE_Triplecast:
                    DrawSliderInt(0, 1, BLM_AoE_Triplecast_HoldCharges,
                        $"How many charges of {Triplecast.ActionName()} to keep ready? (0 = Use all)");
                    break;

                case CustomComboPreset.BLM_AoE_Thunder:
                    DrawSliderInt(0, 50, BLM_AoE_ThunderHP,
                        $"Stop Using {Thunder2.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.BLM_Variant_Cure:
                    DrawSliderInt(1, 100, BLM_VariantCure,
                        "HP% to be at or under", 200);

                    break;
            }
        }
    }
}
