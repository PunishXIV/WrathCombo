using Dalamud.Interface.Colors;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Functions.SliderIncrements;
using WrathCombo.Window.Functions;
using WrathCombo.Data;
using WrathCombo.Combos.PvP;

namespace WrathCombo.Combos.PvE
{
    internal partial class RDM
    {
        internal static class Config
        {
            public static UserInt
                RDM_VariantCure = new("RDM_VariantCure"),
                RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
                RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
                RDM_AoE_MoulinetRange = new("RDM_MoulinetRange"),
                RDMPvP_Burst_CorpsACorps = new("RDMPvP_Burst_CorpsACorps"),
                RDMPvP_Burst_Displacement = new("RDMPvP_Burst_Displacement"),
                RDM_BalanceOpener_Content = new("RDM_BalanceOpener_Content", 1);

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
                    case CustomComboPreset.RDM_Balance_Opener:
                        DrawBossOnlyChoice(RDM_BalanceOpener_Content);
                        break;

                    case CustomComboPreset.RDM_ST_oGCD:
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_OnAction_Adv, "Advanced Action Options.", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_ST_oGCD_OnAction_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, $"{Jolt.ActionName()}s", "", 4, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, Fleche.ActionName(), "", 4, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, Riposte.ActionName(), "", 4, 2, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_oGCD_OnAction, Reprise.ActionName(), "", 4, 3, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }

                        DrawAdditionalBoolChoice(RDM_ST_oGCD_Fleche, Fleche.ActionName(), "");
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_ContreSixte, ContreSixte.ActionName(), "");
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_Engagement, Engagement.ActionName(), "", isConditionalChoice: true);
                        if (RDM_ST_oGCD_Engagement)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_ST_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps, Corpsacorps.ActionName(), "", isConditionalChoice: true);
                        if (RDM_ST_oGCD_CorpACorps)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                            DrawAdditionalBoolChoice(RDM_ST_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_ViceOfThorns, ViceOfThorns.ActionName(), "");
                        DrawAdditionalBoolChoice(RDM_ST_oGCD_Prefulgence, Prefulgence.ActionName(), "");
                        break;

                    case CustomComboPreset.RDM_ST_MeleeCombo:
                        DrawAdditionalBoolChoice(RDM_ST_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_ST_MeleeCombo_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_ST_MeleeCombo_OnAction, $"{Jolt.ActionName()}s", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_MeleeCombo_OnAction, Riposte.ActionName(), "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.RDM_ST_MeleeFinisher:
                        DrawAdditionalBoolChoice(RDM_ST_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_ST_MeleeFinisher_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, $"{Jolt.ActionName()}s", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, Riposte.ActionName(), "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_ST_MeleeFinisher_OnAction, $"{Veraero.ActionName()} & {Verthunder.ActionName()}", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.RDM_ST_Lucid:
                        DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, $"Add {All.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                        break;

                    case CustomComboPreset.RDM_AoE_oGCD:
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_Fleche, Fleche.ActionName(), "");
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_ContreSixte, ContreSixte.ActionName(), "");
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_Engagement, Engagement.ActionName(), "", isConditionalChoice: true);
                        if (RDM_AoE_oGCD_Engagement)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_AoE_oGCD_Engagement_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps, Corpsacorps.ActionName(), "", isConditionalChoice: true);
                        if (RDM_AoE_oGCD_CorpACorps)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps_Melee, "Use only in melee range.", "");
                            DrawAdditionalBoolChoice(RDM_AoE_oGCD_CorpACorps_Pooling, "Pool one charge for manual use.", "");
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_ViceOfThorns, ViceOfThorns.ActionName(), "");
                        DrawAdditionalBoolChoice(RDM_AoE_oGCD_Prefulgence, Prefulgence.ActionName(), "");
                        break;

                    case CustomComboPreset.RDM_AoE_MeleeCombo:
                        DrawSliderInt(3, 8, RDM_AoE_MoulinetRange, $"Range to use first {Moulinet.ActionName()}, no range restrictions after first {Moulinet.ActionName()}", sliderIncrement: Ones);
                        DrawAdditionalBoolChoice(RDM_AoE_MeleeCombo_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_AoE_MeleeCombo_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeCombo_OnAction, $"{Scatter.ActionName()}/{Impact.ActionName()}", "", 2, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeCombo_OnAction, Moulinet.ActionName(), "", 2, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        DrawAdditionalBoolChoice(RDM_ST_MeleeEnforced, "Enforced Melee Check", "Once the melee combo has started, don't switch away even if target is out of range.");
                        break;

                    case CustomComboPreset.RDM_AoE_MeleeFinisher:
                        DrawAdditionalBoolChoice(RDM_AoE_MeleeFinisher_Adv, "Advanced Action Options", "Changes which action this option will replace.", isConditionalChoice: true);
                        if (RDM_AoE_MeleeFinisher_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, $"{Scatter.ActionName()}/{Impact.ActionName()}", "", 3, 0, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, Moulinet.ActionName(), "", 3, 1, descriptionColor: ImGuiColors.DalamudYellow);
                            DrawHorizontalMultiChoice(RDM_AoE_MeleeFinisher_OnAction, $"{Veraero2.ActionName()} & {Verthunder3.ActionName()}", "", 3, 2, descriptionColor: ImGuiColors.DalamudYellow);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.RDM_AoE_Lucid:
                        DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, $"Add {All.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                        break;

                    case CustomComboPreset.RDM_Variant_Cure:
                        DrawSliderInt(1, 100, RDM_VariantCure, "HP% to be at or under", 200);
                        break;

                    // PvP

                    // Resolution
                    case CustomComboPreset.RDMPvP_Resolution:
                        DrawSliderInt(10, 100, RDMPvP.Config.RDMPvP_Resolution_TargetHP, "Target HP%", 210);

                        break;

                    // Embolden / Prefulgence
                    case CustomComboPreset.RDMPvP_Embolden:
                        DrawAdditionalBoolChoice(RDMPvP.Config.RDMPvP_Embolden_SubOption, "Prefulgence Option",
                            "Uses Prefulgence when available.");

                        break;

                    // Corps-a-Corps
                    case CustomComboPreset.RDMPvP_Corps:
                        DrawSliderInt(0, 1, RDMPvP.Config.RDMPvP_Corps_Charges, "Charges to Keep", 178);
                        DrawSliderInt(5, 10, RDMPvP.Config.RDMPvP_Corps_Range, "Maximum Range", 173);

                        break;

                    // Displacement
                    case CustomComboPreset.RDMPvP_Displacement:
                        DrawSliderInt(0, 1, RDMPvP.Config.RDMPvP_Displacement_Charges, "Charges to Keep", 178);
                        ImGui.Spacing();
                        DrawAdditionalBoolChoice(RDMPvP.Config.RDMPvP_Displacement_SubOption, "No Movement Option",
                            "Uses Displacement only when not moving.");

                        break;

                    // Forte / Vice of Thorns
                    case CustomComboPreset.RDMPvP_Forte:
                        DrawSliderInt(10, 100, RDMPvP.Config.RDMPvP_Forte_PlayerHP, "Player HP%", 210);
                        ImGui.Spacing();
                        DrawAdditionalBoolChoice(RDMPvP.Config.RDMPvP_Forte_SubOption, "Vice of Thorns Option",
                            "Uses Vice of Thorns when available.");

                        break;
                }
            }
        }
    }
}
