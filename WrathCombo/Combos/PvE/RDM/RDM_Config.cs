using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;
internal partial class RDM
{
    internal static class Config
    {
        #region Options
        public static UserBool RDM_ST_ThunderAero_Pull = new("RDM_ST_ThunderAero_Pull", true);
        
        public static UserInt
            RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
            RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
            RDM_BalanceOpener_Content = new("RDM_BalanceOpener_Content", 1),
            RDM_ST_Acceleration_Charges = new("RDM_ST_Acceleration_Charges", 0),
            RDM_AoE_Acceleration_Charges = new("RDM_AoE_Acceleration_Charges", 0),
            RDM_ST_Corpsacorps_Distance = new("RDM_ST_Corpsacorps_Distance", 25),
            RDM_ST_Corpsacorps_Time = new("RDM_ST_Corpsacorps_Time", 0),
            RDM_ST_GapCloseCorpsacorps_Time = new("RDM_ST_GapCloseCorpsacorps_Time", 0),
            RDM_ST_VerCureThreshold = new("RDM_ST_VerCureThreshold", 40),
            RDM_ST_MeleeCombo_IncludeReprise_Distance = new("RDM_ST_MeleeCombo_IncludeReprise_Distance", 5),
            RDM_ST_Embolden_Threshold = new("RDM_ST_Embolden_Threshold", 20),
            RDM_ST_Embolden_SubOption = new("RDM_ST_Embolden_SubOption"),
            RDM_ST_Manafication_Threshold = new("RDM_ST_Manafication_Threshold", 20),
            RDM_ST_Manafication_SubOption = new("RDM_ST_Manafication_SubOption"),
            RDM_AoE_Corpsacorps_Distance = new("RDM_AoE_Corpsacorps_Distance", 25),
            RDM_AoE_Corpsacorps_Time = new("RDM_AoE_Corpsacorps_Time", 0),
            RDM_AoE_GapCloseCorpsacorps_Time = new("RDM_AoE_GapCloseCorpsacorps_Time", 0),
            RDM_AoE_VerCureThreshold = new("RDM_AoE_VerCureThreshold", 40),
            RDM_AoE_Embolden_Threshold = new("RDM_AoE_Embolden_Threshold", 20),
            RDM_AoE_Embolden_SubOption = new("RDM_AoE_Embolden_SubOption"),
            RDM_AoE_Manafication_Threshold = new("RDM_AoE_Manafication_Threshold", 20),
            RDM_AoE_Manafication_SubOption = new("RDM_AoE_Manafication_SubOption"),
            RDM_Opener_Selection = new("RDM_Opener_Selection", 0),
            RDM_Riposte_Weaves_Options_EngagementCharges = new("RDM_Riposte_Weaves_Options_EngagementCharges", 0),
            RDM_Riposte_Weaves_Options_CorpsCharges = new("RDM_Riposte_Weaves_Options_CorpsCharges", 0),
            RDM_Riposte_Weaves_Options_Corpsacorps_Distance = new("RDM_Riposte_Weaves_Options_Corpsacorps_Distance", 25),
            RDM_Moulinet_Weaves_Options_EngagementCharges = new("RDM_Moulinet_Weaves_Options_EngagementCharges", 0),
            RDM_Moulinet_Weaves_Options_CorpsCharges = new("RDM_Moulinet_Weaves_Options_CorpsCharges", 0),
            RDM_Moulinet_Weaves_Options_Corpsacorps_Distance = new("RDM_Moulinet_Weaves_Options_Corpsacorps_Distance", 25),
            RDM_OGCDs_Options_CorpsCharges = new("RDM_OGCDs_Options_CorpsCharges", 0),
            RDM_OGCDs_Options_EngagementCharges = new("RDM_OGCDs_Options_EngagementCharges", 0),
            RDM_OGCDs_Options_Corpsacorps_Distance = new("RDM_OGCDs_Options_Corpsacorps_Distance", 25);

        internal static UserBoolArray
            RDM_OGCDs_Options = new("RDM_OGCDs_Options"),
            RDM_Riposte_Weaves_Options = new("RDM_Riposte_Weaves_Options"),
            RDM_Moulinet_Weaves_Options = new("RDM_Moulinet_Weaves_Options"),
            RDM_VerAero_Options = new("RDM_VerAero_Options"),
            RDM_VerThunder_Options = new("RDM_VerThunder_Options"),
            RDM_VerAero2_Options = new("RDM_VerAero2_Options"),
            RDM_VerThunder2_Options = new("RDM_VerThunder2_Options");

        #endregion

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target
                case Preset.RDM_Balance_Opener:
                    DrawBossOnlyChoice(RDM_BalanceOpener_Content);
                    ImGui.NewLine();
                    DrawRadioButton(RDM_Opener_Selection, "Standard Opener", 
                        "Opener Failure Timeout (in Settings Tab) Must be set to 5+ seconds for opener to function due to long initial spell cast.", 0, descriptionAsTooltip: true);
                    DrawRadioButton(RDM_Opener_Selection, "GapClosing Adjusted Standard Opener", 
                        "Opener Failure Timeout (in Settings Tab) Must be set to 5+ seconds for opener to function due to long initial spell cast.", 1, descriptionAsTooltip: true);
                    break;
                
                case Preset.RDM_ST_ThunderAero:
                    DrawAdditionalBoolChoice(RDM_ST_ThunderAero_Pull, "Pull with Thunder/Aero", "Starts with Thunder/Aero if out of combat.");
                    break;
                
                case Preset.RDM_ST_VerCure:
                    DrawSliderInt(1, 100, RDM_ST_VerCureThreshold, "HP% to be at or under", 200);
                    break;
                    
                case Preset.RDM_ST_Embolden:
                    DrawSliderInt(0, 100, RDM_ST_Embolden_Threshold,
                        "Stop using Embolden on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(RDM_ST_Embolden_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(RDM_ST_Embolden_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;
                
                case Preset.RDM_ST_Manafication:
                    DrawSliderInt(0, 100, RDM_ST_Manafication_Threshold,
                        "Stop using Manafication on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(RDM_ST_Manafication_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(RDM_ST_Manafication_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;

                case Preset.RDM_ST_MeleeCombo:
                    if (CustomComboFunctions.IsNotEnabled(Preset.RDM_ST_MeleeCombo_IncludeRiposte))
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
                    
                    DrawSliderInt(0, 5, RDM_ST_Corpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;
                
                case Preset.RDM_ST_MeleeCombo_GapCloser:
                    DrawSliderInt(0, 5, RDM_ST_GapCloseCorpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;
                
                case Preset.RDM_ST_MeleeCombo_IncludeReprise:
                    DrawSliderInt(4, 25, RDM_ST_MeleeCombo_IncludeReprise_Distance,
                        "Use when Distance from target is greater than or equal to:");
                    break;
                    
                #endregion

                #region AOE
                case Preset.RDM_AoE_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_AoE_Corpsacorps_Distance,
                        " Use when Distance from target is less than or equal to:");
                    DrawSliderInt(0, 5, RDM_AoE_Corpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;
                
                case Preset.RDM_AoE_MeleeCombo_GapCloser:
                    DrawSliderInt(0, 5, RDM_AoE_GapCloseCorpsacorps_Time,
                        " How long you need to be stationary to use. Zero to disable");
                    break;
                    
                case Preset.RDM_AoE_Embolden:
                    DrawSliderInt(0, 100, RDM_AoE_Embolden_Threshold,
                        $"Stop using Embolden on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(RDM_AoE_Embolden_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(RDM_AoE_Embolden_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;
                
                case Preset.RDM_AoE_Manafication:
                    DrawSliderInt(0, 100, RDM_AoE_Manafication_Threshold,
                        $"Stop using Manafication on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(RDM_AoE_Manafication_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(RDM_AoE_Manafication_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;
                
                case Preset.RDM_AoE_VerCure:
                    DrawSliderInt(1, 100, RDM_AoE_VerCureThreshold, "HP% to be at or under", 200);
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
                case Preset.RDM_Riposte_Weaves:
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, "Fleche", "Adds to the OGCD button", 6, 0);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, "Contre Sixte", "Adds to the OGCD button", 6, 1);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, "Vice Of Thorns", "Adds to the OGCD button", 6, 2);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, "Prefulgence", "Adds to the OGCD button", 6, 3);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, "Engagement", "Adds to the OGCD button", 6, 4);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, "Corps-a-corps", "Adds to the OGCD button", 6, 5);

                    if (RDM_Riposte_Weaves_Options[4])
                    {
                        DrawSliderInt(0, 1, RDM_Riposte_Weaves_Options_EngagementCharges, "How many charges of Engagement to keep for manual use");
                    }

                    if (RDM_Riposte_Weaves_Options[5])
                    {
                        DrawSliderInt(0, 1, RDM_Riposte_Weaves_Options_CorpsCharges, "How many charges of Corps to keep for manual use");
                        DrawSliderInt(0, 25, RDM_Riposte_Weaves_Options_Corpsacorps_Distance, "Use Corps when distance is less than or equal to:");
                    }
                    break;

                case Preset.RDM_Moulinet_Weaves:
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, "Fleche", "Adds to the OGCD button", 6, 0);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, "Contre Sixte", "Adds to the OGCD button", 6, 1);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, "Vice Of Thorns", "Adds to the OGCD button", 6, 2);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, "Prefulgence", "Adds to the OGCD button", 6, 3);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, "Engagement", "Adds to the OGCD button", 6, 4);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, "Corps-a-corps", "Adds to the OGCD button", 6, 5);

                    if (RDM_Moulinet_Weaves_Options[4])
                    {
                        DrawSliderInt(0, 1, RDM_Moulinet_Weaves_Options_EngagementCharges, "How many charges of Engagement to keep for manual use");
                    }

                    if (RDM_Moulinet_Weaves_Options[5])
                    {
                        DrawSliderInt(0, 1, RDM_Moulinet_Weaves_Options_CorpsCharges, "How many charges of Corps to keep for manual use");
                        DrawSliderInt(0, 25, RDM_Moulinet_Weaves_Options_Corpsacorps_Distance, "Use Corps when distance is less than or equal to:");
                    }
                    break;

                case Preset.RDM_OGCDs:
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, "Contre Sixte", "Adds to the OGCD button", 5, 0);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, "Vice Of Thorns", "Adds to the OGCD button", 5, 1);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, "Prefulgence", "Adds to the OGCD button", 5, 2);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, "Engagement", "Adds to the OGCD button", 5, 3);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, "Corps-a-corps", "Adds to the OGCD button", 5, 4);

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

                case Preset.RDM_VerAero:
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, "Holy Flare Combo", "Adds smart Holy/Flare", 4, 0);
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, "VerStone", "Adds VerStone", 4, 1);
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, "Scorch Combo", "Adds Scorch/Resolution Finishers", 4, 2);
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, "Jolt", "Adds Jolt", 4, 3);
                    break;

                case Preset.RDM_VerThunder:
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, "Holy Flare Combo", "Adds smart Holy/Flare", 4, 0);
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, "VerFire", "Adds VerFire", 4, 1);
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, "Scorch Combo", "Adds Scorch/Resolution Finishers", 4, 2);
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, "Jolt", "Adds Jolt", 4, 3);
                    break;

                case Preset.RDM_VerAero2:
                    DrawHorizontalMultiChoice(RDM_VerAero2_Options, "Holy Flare Combo", "Adds smart Holy/Flare", 3, 0);
                    DrawHorizontalMultiChoice(RDM_VerAero2_Options, "Scorch Combo", "Adds Scorch/Resolution Finishers", 3, 1);
                    DrawHorizontalMultiChoice(RDM_VerAero2_Options, "Impact", "Adds Impact", 3, 2);
                    break;

                case Preset.RDM_VerThunder2:
                    DrawHorizontalMultiChoice(RDM_VerThunder2_Options, "Holy Flare Combo", "Adds smart Holy/Flare", 3, 0);
                    DrawHorizontalMultiChoice(RDM_VerThunder2_Options, "Scorch Combo", "Adds Scorch/Resolution Finishers", 3, 1);
                    DrawHorizontalMultiChoice(RDM_VerThunder2_Options, "Impact", "Adds Impact", 3, 2);
                    break;
                    #endregion
            }
        }
    }
}
