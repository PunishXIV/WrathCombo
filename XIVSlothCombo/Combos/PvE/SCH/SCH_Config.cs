using ImGuiNET;
using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.Extensions.UIntExtensions;
using static XIVSlothCombo.Window.Functions.UserConfig;
using static XIVSlothCombo.Window.Functions.SliderIncrements;

namespace XIVSlothCombo.Combos.PvE
{
    internal static partial class SCH
    {
        internal static class Config
        {
            #region DPS
            public static UserInt
                SCH_ST_DPS_AltMode = new("SCH_ST_DPS_AltMode"),
                SCH_ST_DPS_LucidOption = new("SCH_ST_DPS_LucidOption", 6500),
                SCH_ST_DPS_BioOption = new("SCH_ST_DPS_BioOption", 10),
                SCH_ST_DPS_ChainStratagemOption = new("SCH_ST_DPS_ChainStratagemOption", 10);
            public static UserBool
                SCH_ST_DPS_Adv = new("SCH_ST_DPS_Adv"),
                SCH_ST_DPS_Bio_Adv = new("SCH_ST_DPS_Bio_Adv"),
                SCH_ST_DPS_EnergyDrain_Adv = new("SCH_ST_DPS_EnergyDrain_Adv");
            public static UserFloat
                SCH_ST_DPS_Bio_Threshold = new("SCH_ST_DPS_Bio_Threshold", 3.0f),
                SCH_ST_DPS_EnergyDrain = new("SCH_ST_DPS_EnergyDrain", 3.0f);
            public static UserBoolArray
                SCH_ST_DPS_Adv_Actions = new("SCH_ST_DPS_Adv_Actions");
            #endregion

            #region Healing
            public static UserInt
                SCH_AoE_LucidOption = new("SCH_AoE_LucidOption", 6500),
                SCH_AoE_Heal_LucidOption = new("SCH_AoE_Heal_LucidOption", 6500),
                SCH_ST_Heal_LucidOption = new("SCH_ST_Heal_LucidOption", 6500),
                SCH_ST_Heal_AdloquiumOption = new("SCH_ST_Heal_AdloquiumOption"),
                SCH_ST_Heal_LustrateOption = new("SCH_ST_Heal_LustrateOption"),
                SCH_ST_Heal_ExcogitationOption = new("SCH_ST_Heal_ExcogitationOption"),
                SCH_ST_Heal_ProtractionOption = new("SCH_ST_Heal_ProtractionOption"),
                SCH_ST_Heal_AetherpactOption = new("SCH_ST_Heal_AetherpactOption"),
                SCH_ST_Heal_AetherpactFairyGauge = new("SCH_ST_Heal_AetherpactFairyGauge"),
                SCH_ST_Heal_EsunaOption = new("SCH_ST_Heal_EsunaOption");
            public static UserIntArray
                SCH_ST_Heals_Priority = new("SCH_ST_Heals_Priority"),
                SCH_AoE_Heals_Priority = new ("SCH_AoE_Heals_Priority");
            public static UserBool
                SCH_ST_Heal_Adv = new("SCH_ST_Heal_Adv"),
                SCH_ST_Heal_UIMouseOver = new("SCH_ST_Heal_UIMouseOver"),
                SCH_ST_Heal_IncludeShields = new("SCH_ST_Heal_IncludeShields"),
                SCH_DeploymentTactics_Adv = new("SCH_DeploymentTactics_Adv"),
                SCH_DeploymentTactics_UIMouseOver = new("SCH_DeploymentTactics_UIMouseOver");
            public static UserBoolArray
                SCH_ST_Heal_AldoquimOpts = new("SCH_ST_Heal_AldoquimOpts");
            #endregion

            #region Utility
            internal static UserBool
                SCH_Aetherflow_Recite_Indom = new("SCH_Aetherflow_Recite_Indom"),
                SCH_Aetherflow_Recite_Excog = new("SCH_Aetherflow_Recite_Excog");
            internal static UserInt
                SCH_Aetherflow_Display = new("SCH_Aetherflow_Display"),
                SCH_Aetherflow_Recite_ExcogMode = new("SCH_Aetherflow_Recite_ExcogMode"),
                SCH_Aetherflow_Recite_IndomMode = new("SCH_Aetherflow_Recite_IndomMode"),
                SCH_Recitation_Mode = new("SCH_Recitation_Mode");
            #endregion

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.SCH_DPS:
                        DrawAdditionalBoolChoice(SCH_ST_DPS_Adv, "Advanced Action Options", "Change how actions are handled", isConditionalChoice: true);
                        if (SCH_ST_DPS_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(SCH_ST_DPS_Adv_Actions, "On Ruin/Broils", "Apply options to Ruin and all Broils.", 3, 0);
                            DrawHorizontalMultiChoice(SCH_ST_DPS_Adv_Actions, "On Bio/Bio II/Biolysis", "Apply options to Bio and Biolysis.", 3, 1);
                            DrawHorizontalMultiChoice(SCH_ST_DPS_Adv_Actions, "On Ruin II", "Apply options to Ruin II.", 3, 2);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SCH_DPS_Lucid:
                        DrawSliderInt(4000, 9500, SCH_ST_DPS_LucidOption, "MP Threshold", 150, Hundreds);
                        break;

                    case CustomComboPreset.SCH_DPS_Bio:
                        DrawSliderInt(0, 100, SCH_ST_DPS_BioOption, "Stop using at Enemy HP%. Set to Zero to disable this check.");

                        DrawAdditionalBoolChoice(SCH_ST_DPS_Bio_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (SCH_ST_DPS_Bio_Adv)
                        {
                            ImGui.Indent();
                            DrawRoundedSliderFloat(0, 4, SCH_ST_DPS_Bio_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SCH_DPS_ChainStrat:
                        DrawSliderInt(0, 100, SCH_ST_DPS_ChainStratagemOption, "Stop using at Enemy HP%. Set to Zero to disable this check.");
                        break;

                    case CustomComboPreset.SCH_DPS_EnergyDrain:
                        DrawAdditionalBoolChoice(SCH_ST_DPS_EnergyDrain_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (SCH_ST_DPS_EnergyDrain_Adv)
                        {
                            ImGui.Indent();
                            DrawRoundedSliderFloat(0, 60, SCH_ST_DPS_EnergyDrain, "Aetherflow remaining cooldown:", digits: 1);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SCH_ST_Heal:
                        DrawAdditionalBoolChoice(SCH_ST_Heal_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (SCH_ST_Heal_Adv)
                        {
                            ImGui.Indent();
                            DrawAdditionalBoolChoice(SCH_ST_Heal_UIMouseOver,
                                "Party UI Mouseover Checking",
                                "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                                "To be used in conjunction with Redirect/Reaction/etc");
                            DrawAdditionalBoolChoice(SCH_ST_Heal_IncludeShields,"Include Shields in HP Percent Sliders","");
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Lucid:
                        DrawSliderInt(4000, 9500, SCH_ST_Heal_LucidOption, "MP Threshold", 150, Hundreds);
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Adloquium:
                        DrawSliderInt(0, 100, SCH_ST_Heal_AdloquiumOption, $"Start using when below HP %. Set to 100 to disable this check.");
                        DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts, "Ignore Shield Check", $"Warning, will force the use of {Adloquium.ActionName()}, and normal {Physick.ActionName()} maybe unavailable.", 2, 0);
                        DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts, $"Check for Sage {SGE.EukrasianDiagnosis.ActionName()}/{SGE.EukrasianPrognosis.ActionName()}", "Enable to not override an existing Sage's shield.", 2, 1);
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Lustrate:
                        DrawSliderInt(0, 100, SCH_ST_Heal_LustrateOption, "Start using when below HP %. Set to 100 to disable this check");
                        DrawPriorityInput(SCH_ST_Heals_Priority, 4, 0, $"{Lustrate.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Excogitation:
                        DrawSliderInt(0, 100, SCH_ST_Heal_ExcogitationOption, "Start using when below HP %. Set to 100 to disable this check");
                        DrawPriorityInput(SCH_ST_Heals_Priority, 4, 1, $"{Excogitation.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Protraction:
                        DrawSliderInt(0, 100, SCH_ST_Heal_ProtractionOption, "Start using when below HP %. Set to 100 to disable this check");
                        DrawPriorityInput(SCH_ST_Heals_Priority, 4, 2, $"{Protraction.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Aetherpact:
                        DrawSliderInt(0, 100, SCH_ST_Heal_AetherpactOption, "Start using when below HP %. Set to 100 to disable this check");
                        DrawSliderInt(10, 100, SCH_ST_Heal_AetherpactFairyGauge, "Minimal Fairy Gauge to start using Aetherpact", sliderIncrement: Tens);
                        DrawPriorityInput(SCH_ST_Heals_Priority, 4, 3, $"{Aetherpact.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_ST_Heal_Esuna:
                        DrawSliderInt(0, 100, SCH_ST_Heal_EsunaOption, "Stop using when below HP %. Set to Zero to disable this check");
                        break;

                    case CustomComboPreset.SCH_AoE_Lucid:
                        DrawSliderInt(4000, 9500, SCH_AoE_LucidOption, "MP Threshold", 150, Hundreds);
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_Lucid:
                        DrawSliderInt(4000, 9500, SCH_AoE_Heal_LucidOption, "MP Threshold", 150, Hundreds);
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_WhisperingDawn:
                        DrawPriorityInput(SCH_AoE_Heals_Priority, 6, 0, $"{WhisperingDawn.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_FeyIllumination:
                        DrawPriorityInput(SCH_AoE_Heals_Priority, 6, 1, $"{FeyIllumination.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_FeyBlessing:
                        DrawPriorityInput(SCH_AoE_Heals_Priority, 6, 2, $"{FeyBlessing.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_Consolation:
                        DrawPriorityInput(SCH_AoE_Heals_Priority, 6, 3, $"{Consolation.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_Seraphism:
                        DrawPriorityInput(SCH_AoE_Heals_Priority, 6, 4, $"{Seraphism.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_AoE_Heal_Indomitability:
                        DrawPriorityInput(SCH_AoE_Heals_Priority, 6, 5, $"{Indomitability.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SCH_DeploymentTactics:
                        DrawAdditionalBoolChoice(SCH_DeploymentTactics_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (SCH_DeploymentTactics_Adv)
                        {
                            ImGui.Indent();
                            DrawAdditionalBoolChoice(SCH_DeploymentTactics_UIMouseOver,
                                "Party UI Mouseover Checking",
                                "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                                "To be used in conjunction with Redirect/Reaction/etc");
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SCH_Aetherflow:
                        DrawRadioButton(SCH_Aetherflow_Display, "Show Aetherflow On Energy Drain Only", "", 0);
                        DrawRadioButton(SCH_Aetherflow_Display, "Show Aetherflow On All Aetherflow Skills", "", 1);
                        break;

                    case CustomComboPreset.SCH_Aetherflow_Recite:
                        DrawAdditionalBoolChoice(SCH_Aetherflow_Recite_Excog, "On Excogitation", "", isConditionalChoice: true);
                        if (SCH_Aetherflow_Recite_Excog)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawRadioButton(SCH_Aetherflow_Recite_ExcogMode, "Only when out of Aetherflow Stacks", "", 0);
                            DrawRadioButton(SCH_Aetherflow_Recite_ExcogMode, "Always when available", "", 1);
                            ImGui.Unindent();
                        }

                        DrawAdditionalBoolChoice(SCH_Aetherflow_Recite_Indom, "On Indominability", "", isConditionalChoice: true);
                        if (SCH_Aetherflow_Recite_Indom)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawRadioButton(SCH_Aetherflow_Recite_IndomMode, "Only when out of Aetherflow Stacks", "", 0);
                            DrawRadioButton(SCH_Aetherflow_Recite_IndomMode, "Always when available", "", 1);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SCH_Recitation:
                        DrawRadioButton(SCH_Recitation_Mode, "Adloquium", "", 0);
                        DrawRadioButton(SCH_Recitation_Mode, "Succor", "", 1);
                        DrawRadioButton(SCH_Recitation_Mode, "Indomitability", "", 2);
                        DrawRadioButton(SCH_Recitation_Mode, "Excogitation", "", 3);
                        break;
                }

            }
        }
    }
}
