using Dalamud.Interface.Colors;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
using Preset = WrathCombo.Combos.CustomComboPreset;

namespace WrathCombo.Combos.PvE;
internal partial class RDM
{
    internal static class Config
    {
        #region Options
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
            RDM_Opener_Selection = new("RDM_Opener_Selection", 0),
            RDM_OGCDs_Options_CorpsCharges = new("RDM_OGCDs_Options_CorpsCharges", 0),
            RDM_OGCDs_Options_EngagementCharges = new("RDM_OGCDs_Options_EngagementCharges", 0),
            RDM_OGCDs_Options_Corpsacorps_Distance = new("RDM_OGCDs_Options_Corpsacorps_Distance", 25);
        
        internal static UserBoolArray
            RDM_OGCDs_Options = new("RDM_OGCDs_Options");
        
        #endregion

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target
                case Preset.RDM_Balance_Opener:
                    DrawHorizontalRadioButton(RDM_Opener_Selection, "Standard Opener", "Balance Standard Opener", 0);
                    DrawHorizontalRadioButton(RDM_Opener_Selection, "GapClosing Adjusted Standard Opener", "Shifts the melee a little bit to put a gapcloser in", 1);

                    ImGui.Indent();
                    DrawBossOnlyChoice(RDM_BalanceOpener_Content);
                    ImGui.Unindent();
                    break;
                
                case Preset.RDM_Variant_Cure:
                    DrawSliderInt(1, 100, RDM_VariantCure, "HP% to be at or under", 200);
                    break;

                case Preset.RDM_ST_MeleeCombo:
                    if (P.IPCSearch.AutoActions[Preset.RDM_ST_DPS] == true && 
                        CustomComboFunctions.IsNotEnabled(Preset.RDM_ST_MeleeCombo_IncludeRiposte))
                    {
                        ImGui.Indent();
                        ImGui.TextColored(ImGuiColors.DalamudRed, "WARNING: RIPOSTE IS NOT ENABLED.");
                        ImGui.TextColored(ImGuiColors.DalamudRed, "AUTO ROTATION WILL NOT START THE MELEE COMBO AUTOMATICALLY");
                        ImGui.Unindent();
                    }
                    break;
                
                case Preset.RDM_ST_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_ST_Corpsacorps_Distance,
                        " Use when Distance from target is less than or equal to:");
                    
                    DrawSliderInt(0, 3, RDM_ST_Corpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;
                #endregion
                
                #region AOE
                case Preset.RDM_AoE_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_AoE_Corpsacorps_Distance,
                        " Use when Distance from target is less than or equal to:");
                    DrawSliderInt(0, 3, RDM_AoE_Corpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;

                case Preset.RDM_ST_Lucid:
                    DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, $"Add {Role.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                    break;

                case Preset.RDM_AoE_Lucid:
                    DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, $"Add {Role.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                    break;
                
                case Preset.RDM_ST_Acceleration:
                    DrawSliderInt(0, 1, RDM_ST_Acceleration_Charges, "How many charges to keep ready\n (0 = Use All)");
                    break;

                case Preset.RDM_AoE_Acceleration:
                    DrawSliderInt(0, 1, RDM_AoE_Acceleration_Charges, "How many charges to keep ready?\n (0 = Use All)");
                    break;
                #endregion
                
                #region Standalones
                case Preset.RDM_OGCDs:
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options,"Contre Sixte", "Adds to the OGCD button", 5, 0);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options,"Vice Of Thorns", "Adds to the OGCD button", 5, 1);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options,"Prefulgence", "Adds to the OGCD button", 5, 2);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options,"Engagement", "Adds to the OGCD button", 5, 3);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options,"Corps-a-corps", "Adds to the OGCD button", 5, 4);

                    if (RDM_OGCDs_Options[3])
                    {
                        DrawSliderInt(0, 1, RDM_OGCDs_Options_EngagementCharges, "How many charges of Engagement to keep for manual use");
                    }
                    
                    if (RDM_OGCDs_Options[4])
                    {
                        DrawSliderInt(0, 1, RDM_OGCDs_Options_CorpsCharges, "How many charges of Corps to keep for manual use");
                        DrawSliderInt(0, 25, RDM_OGCDs_Options_Corpsacorps_Distance, "Use Corps when distance is less than or equal to:");
                    }
                    break;
                #endregion
            }
        }
    }
}
