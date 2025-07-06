using Dalamud.Interface.Colors;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;
internal partial class RDM
{
    internal static class Config
    {
        public static UserInt
            RDM_VariantCure = new("RDM_VariantCure"),
            RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
            RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
            RDM_BalanceOpener_Content = new("RDM_BalanceOpener_Content", 1),
            RDM_ST_Acceleration_Charges = new("RDM_ST_Acceleration_Charges", 0),
            RDM_AoE_Acceleration_Charges = new("RDM_AoE_Acceleration_Charges", 0),
            RDM_ST_Corpsacorps_Distance = new("RDM_ST_Corpsacorps_Distance", 25),
            RDM_ST_Corpsacorps_Time = new("RDM_ST_Corpsacorps_Time", 0),
            RDM_AoE_Corpsacorps_Distance = new("RDM_AoE_Corpsacorps_Distance", 25),
            RDM_AoE_Corpsacorps_Time = new("RDM_AoE_Corpsacorps_Time", 0),
            RDM_Opener_Selection = new("RDM_Opener_Selection", 0);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.RDM_Balance_Opener:
                    DrawHorizontalRadioButton(RDM_Opener_Selection, "Standard Opener", "Balance Standard Opener", 0);
                    DrawHorizontalRadioButton(RDM_Opener_Selection, "GapClosing Adjusted Standard Opener", "Shifts the melee a little bit to put a gapcloser in", 1);

                    ImGui.Indent();
                    DrawBossOnlyChoice(RDM_BalanceOpener_Content);
                    ImGui.Unindent();
                    break;
                
                case CustomComboPreset.RDM_Variant_Cure:
                    DrawSliderInt(1, 100, RDM_VariantCure, "HP% to be at or under", 200);
                    break;

                case CustomComboPreset.RDM_ST_MeleeCombo:
                    if (P.IPCSearch.AutoActions[CustomComboPreset.RDM_ST_DPS] == true && 
                        CustomComboFunctions.IsNotEnabled(CustomComboPreset.RDM_ST_MeleeCombo_IncludeRiposte))
                    {
                        ImGui.Indent();
                        ImGui.TextColored(ImGuiColors.DalamudRed, "WARNING: RIPOSTE IS NOT ENABLED.");
                        ImGui.TextColored(ImGuiColors.DalamudRed, "AUTO ROTATION WILL NOT START THE MELEE COMBO AUTOMATICALLY");
                        ImGui.Unindent();
                    }
                    break;
                
                case CustomComboPreset.RDM_ST_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_ST_Corpsacorps_Distance,
                        " Use when Distance from target is less than or equal to:");
                    
                    DrawSliderInt(0, 3, RDM_ST_Corpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;
                
                case CustomComboPreset.RDM_AoE_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_AoE_Corpsacorps_Distance,
                        " Use when Distance from target is less than or equal to:");
                    DrawSliderInt(0, 3, RDM_AoE_Corpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;

                case CustomComboPreset.RDM_ST_Lucid:
                    DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, $"Add {Role.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                    break;

                case CustomComboPreset.RDM_AoE_Lucid:
                    DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, $"Add {Role.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                    break;
                
                case CustomComboPreset.RDM_ST_Acceleration:
                    DrawSliderInt(0, 1, RDM_ST_Acceleration_Charges, "How many charges to keep ready\n (0 = Use All)");
                    break;

                case CustomComboPreset.RDM_AoE_Acceleration:
                    DrawSliderInt(0, 1, RDM_AoE_Acceleration_Charges, "How many charges to keep ready?\n (0 = Use All)");
                    break;
            }
        }
    }
}
