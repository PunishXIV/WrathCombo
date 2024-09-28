using Dalamud.Interface.Colors;
using ImGuiNET;
using XIVSlothCombo.CustomComboNS.Functions;
using static XIVSlothCombo.Extensions.UIntExtensions;
using static XIVSlothCombo.Window.Functions.UserConfig;
using static XIVSlothCombo.Window.Functions.SliderIncrements;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace XIVSlothCombo.Combos.PvE
{
    internal partial class RDM
    {
        internal static class Config
        {
            public static UserInt
                RDM_VariantCure = new("RDM_VariantCure"),
                RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
                RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
                RDM_AoE_MoulinetRange = new("RDM_MoulinetRange");
            public static UserBool
                RDM_ST_oGCD_OnAction_Adv = new("RDM_ST_oGCD_OnAction_Adv"),
                RDM_ST_oGCD_Fleche = new("RDM_ST_oGCD_Fleche"),
                RDM_ST_oGCD_ContreSixte = new("RDM_ST_oGCD_ContreSixte"),
                RDM_ST_oGCD_Engagement = new("RDM_ST_oGCD_Engagement"),
                RDM_ST_oGCD_Engagement_Pooling = new("RDM_ST_oGCD_Engagement_Pooling"),
                RDM_ST_oGCD_CorpACorps = new("RDM_ST_oGCD_CorpACorps"),
                RDM_ST_oGCD_CorpACorps_Melee = new("RDM_ST_oGCD_CorpACorps_Melee"),
                RDM_ST_oGCD_CorpACorps_Pooling = new("RDM_ST_oGCD_CorpACorps_Pooling"),
                RDM_ST_oGCD_ViceOfThorns = new("RDM_ST_oGCD_ViceOfThorns"),
                RDM_ST_oGCD_Prefulgence = new("RDM_ST_oGCD_Prefulgence"),
                RDM_ST_MeleeCombo_Adv = new("RDM_ST_MeleeCombo_Adv"),
                RDM_ST_MeleeFinisher_Adv = new("RDM_ST_MeleeFinisher_Adv"),
                RDM_ST_MeleeEnforced = new("RDM_ST_MeleeEnforced"),

                RDM_AoE_oGCD_OnAction_Adv = new("RDM_AoE_oGCD_OnAction_Adv"),
                RDM_AoE_oGCD_Fleche = new("RDM_AoE_oGCD_Fleche"),
                RDM_AoE_oGCD_ContreSixte = new("RDM_AoE_oGCD_ContreSixte"),
                RDM_AoE_oGCD_Engagement = new("RDM_AoE_oGCD_Engagement"),
                RDM_AoE_oGCD_Engagement_Pooling = new("RDM_AoE_oGCD_Engagement_Pooling"),
                RDM_AoE_oGCD_CorpACorps = new("RDM_AoE_oGCD_CorpACorps"),
                RDM_AoE_oGCD_CorpACorps_Melee = new("RDM_AoE_oGCD_CorpACorps_Melee"),
                RDM_AoE_oGCD_CorpACorps_Pooling = new("RDM_AoE_oGCD_CorpACorps_Pooling"),
                RDM_AoE_oGCD_ViceOfThorns = new("RDM_AoE_oGCD_ViceOfThorns"),
                RDM_AoE_oGCD_Prefulgence = new("RDM_AoE_oGCD_Prefulgence"),
                RDM_AoE_MeleeCombo_Adv = new("RDM_AoE_MeleeCombo_Adv"),
                RDM_AoE_MeleeFinisher_Adv = new("RDM_AoE_MeleeFinisher_Adv");
            public static UserBoolArray
                RDM_ST_oGCD_OnAction = new("RDM_ST_oGCD_OnAction"),
                RDM_ST_MeleeCombo_OnAction = new("RDM_ST_MeleeCombo_OnAction"),
                RDM_ST_MeleeFinisher_OnAction = new("RDM_ST_MeleeFinisher_OnAction"),

                RDM_AoE_oGCD_OnAction = new("RDM_AoE_oGCD_OnAction"),
                RDM_AoE_MeleeCombo_OnAction = new("RDM_AoE_MeleeCombo_OnAction"),
                RDM_AoE_MeleeFinisher_OnAction = new("RDM_AoE_MeleeFinisher_OnAction");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.RDM_ST_oGCD:
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_OnAction_Adv, "Advanced Action Options.", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_ST_oGCD_OnAction_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Jolts", "", 4, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Fleche", "", 4, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Riposte", "", 4, 2, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, "Reprise", "", 4, 3, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }

                        DrawAdditionalBoolChoice(RDM_ST_oGCD_Fleche, "Fleche", "");
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_ContreSixte, "Contre Sixte", "");
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_Engagement, "Engagement", "", isConditionalChoice: true);
                        if (RDM_ST_oGCD_Engagement)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_ST_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps, "Corp-a-Corps", "", isConditionalChoice: true);
                        if (RDM_ST_oGCD_CorpACorps)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                            DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_ViceOfThorns, "Vice of Thorns", "");
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_Prefulgence, "Prefulgence", "");
                        break;

                    case CustomComboPreset.RDM_ST_MeleeCombo:
                        DrawAdditionalBoolChoice(RDM_ST_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_ST_MeleeCombo_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_ST_MeleeCombo_OnAction, "Jolts", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_MeleeCombo_OnAction, "Riposte", "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.RDM_ST_MeleeFinisher:
                        DrawAdditionalBoolChoice(RDM_ST_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_ST_MeleeFinisher_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, "Jolts", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, "Riposte", "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, "VerAero & VerThunder", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.RDM_ST_Lucid:
                        DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, "Add Lucid Dreaming when below this MP", sliderIncrement: Hundreds);
                        break;

                    case CustomComboPreset.RDM_AoE_oGCD:
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_Fleche, "Fleche", "");
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_ContreSixte, "Contre Sixte", "");
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_Engagement, "Engagement", "", isConditionalChoice: true);
                        if (RDM_AoE_oGCD_Engagement)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_AoE_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps, "Corp-a-Corps", "", isConditionalChoice: true);
                        if (RDM_AoE_oGCD_CorpACorps)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                            DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_ViceOfThorns, "Vice of Thorns", "");
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_Prefulgence, "Prefulgence", "");
                        break;

                    case CustomComboPreset.RDM_AoE_MeleeCombo:
                        DrawSliderInt(3, 8, RDM_AoE_MoulinetRange, "Range to use first Moulinet; no range restrictions after first Moulinet", Ones);
                        DrawAdditionalBoolChoice(RDM_AoE_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_AoE_MeleeCombo_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeCombo_OnAction, "Scatter/Impact", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeCombo_OnAction, "Moulinet", "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_ST_MeleeEnforced, "Enforced Melee Check", "Once the melee combo has started, don't switch away even if target is out of range.");
                        break;

                    case CustomComboPreset.RDM_AoE_MeleeFinisher:
                        DrawAdditionalBoolChoice(RDM_AoE_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_AoE_MeleeFinisher_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, "Scatter/Impact", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, "Moulinet", "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, "VerAero II & VerThunder II", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.RDM_AoE_Lucid:
                        DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, "Add Lucid Dreaming when below this MP", sliderIncrement: Hundreds);
                        break;

                    case CustomComboPreset.RDM_Variant_Cure:
                        DrawSliderInt(1, 100, RDM_VariantCure, "HP% to be at or under", 200);
                        break;
                }
            }
        }
    }
}
