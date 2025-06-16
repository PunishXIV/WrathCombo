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
            RDM_AoE_MoulinetRange = new("RDM_MoulinetRange"),
            RDM_BalanceOpener_Content = new("RDM_BalanceOpener_Content", 1),
            RDM_ST_Acceleration_Charges = new("RDM_ST_Acceleration_Charges", 0),
            RDM_ST_AccelerationMovement_Charges = new("RDM_ST_AccelerationMovement_Charges", 0),
            RDM_AoE_Acceleration_Charges = new("RDM_AoE_Acceleration_Charges", 0),
            RDM_AoE_AccelerationMovement_Charges = new("RDM_AoE_AccelerationMovement_Charges", 0);

        public static UserBool
            RDM_ST_oGCD_Engagement_Pooling = new("RDM_ST_oGCD_Engagement_Pooling"),
            RDM_ST_oGCD_CorpACorps_Melee = new("RDM_ST_oGCD_CorpACorps_Melee"),
            RDM_ST_oGCD_CorpACorps_Pooling = new("RDM_ST_oGCD_CorpACorps_Pooling"),

            RDM_AoE_oGCD_Engagement_Pooling = new("RDM_AoE_oGCD_Engagement_Pooling"),
            RDM_AoE_oGCD_CorpACorps_Melee = new("RDM_AoE_oGCD_CorpACorps_Melee"),
            RDM_AoE_oGCD_CorpACorps_Pooling = new("RDM_AoE_oGCD_CorpACorps_Pooling"),

            RDM_Riposte_oGCD_Engagement_Pooling = new ("RDM_Riposte_oGCD_Engagement_Pooling"),
            RDM_Riposte_oGCD_CorpACorps_Melee =   new ("RDM_Riposte_oGCD_CorpACorps_Melee"),
            RDM_Riposte_oGCD_CorpACorps_Pooling = new ("RDM_Riposte_oGCD_CorpACorps_Pooling"),

            RDM_Reprise_oGCD_Engagement_Pooling = new("RDM_Reprise_oGCD_Engagement_Pooling"),
            RDM_Reprise_oGCD_CorpACorps_Melee =   new("RDM_Reprise_oGCD_CorpACorps_Melee"),
            RDM_Reprise_oGCD_CorpACorps_Pooling = new("RDM_Reprise_oGCD_CorpACorps_Pooling");
        public static UserBoolArray
            RDM_ST_oGCD_Actions = new("RDM_ST_oGCD_Actions"),
            RDM_AoE_oGCD_Actions = new("RDM_AoE_oGCD_Actions"),
            RDM_Riposte_oGCD_Actions = new("RDM_Riposte_oGCD_Actions"),
            RDM_Reprise_oGCD_Actions = new("RDM_Reprise_oGCD_Actions");



        private static void DrawOGCDOptions(UserBoolArray oGCDs, UserBool engagementPool, UserBool corpPool, UserBool corpMelee)
        {
            DrawHorizontalMultiChoice(oGCDs, Fleche.ActionName(),       "", 6, 0);
            DrawHorizontalMultiChoice(oGCDs, ContreSixte.ActionName(),  "", 6, 1);
            DrawHorizontalMultiChoice(oGCDs, Engagement.ActionName(),   "", 6, 2);
            DrawHorizontalMultiChoice(oGCDs, Corpsacorps.ActionName(),  "", 6, 3);
            DrawHorizontalMultiChoice(oGCDs, ViceOfThorns.ActionName(), "", 6, 4);
            DrawHorizontalMultiChoice(oGCDs, Prefulgence.ActionName(),  "", 6, 5);

            if (oGCDs[2]) // Engagment
            {
                ImGui.Indent();
                ImGui.Spacing();
                DrawAdditionalBoolChoice(engagementPool, $"{Engagement.ActionName()}: Pool one charge for manual use.", "", isConditionalChoice: true);
                ImGui.Unindent();
            }

            if (oGCDs[3]) // Corps-a-Corps
            {
                ImGui.Indent();
                ImGui.Spacing();
                DrawAdditionalBoolChoice(corpPool, $"{Corpsacorps.ActionName()}: Pool one charge for manual use.", "", isConditionalChoice: true);
                DrawAdditionalBoolChoice(corpMelee, $"{Corpsacorps.ActionName()}: Use only in melee range.", "", isConditionalChoice: true);
                ImGui.Unindent();
            }
        }

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.RDM_Balance_Opener:
                    DrawBossOnlyChoice(RDM_BalanceOpener_Content);
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

                case CustomComboPreset.RDM_ST_oGCD:
                    DrawOGCDOptions(RDM_ST_oGCD_Actions, RDM_ST_oGCD_Engagement_Pooling, RDM_ST_oGCD_CorpACorps_Pooling, RDM_ST_oGCD_CorpACorps_Melee);
                    break;

                case CustomComboPreset.RDM_ST_Lucid:
                    DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, $"Add {Role.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                    break;

                case CustomComboPreset.RDM_AoE_oGCD:
                    DrawOGCDOptions(RDM_AoE_oGCD_Actions, RDM_AoE_oGCD_Engagement_Pooling, RDM_AoE_oGCD_CorpACorps_Pooling, RDM_AoE_oGCD_CorpACorps_Melee);
                    break;

                case CustomComboPreset.RDM_AoE_MeleeCombo:
                    DrawSliderInt(3, 8, RDM_AoE_MoulinetRange, $"Range to use first {Moulinet.ActionName()}, no range restrictions after first {Moulinet.ActionName()}", sliderIncrement: Ones);
                    break;

                case CustomComboPreset.RDM_AoE_Lucid:
                    DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, $"Add {Role.LucidDreaming.ActionName()} when below this MP", sliderIncrement: Hundreds);
                    break;

                case CustomComboPreset.RDM_Variant_Cure:
                    DrawSliderInt(1, 100, RDM_VariantCure, "HP% to be at or under", 200);
                    break;

                case CustomComboPreset.RDM_ST_Spell_Accel:
                    DrawSliderInt(0, 1, RDM_ST_Acceleration_Charges, "How many charges to keep ready?\n (0 = Use All)");
                    break;

                case CustomComboPreset.RDM_ST_Spell_Accel_Movement:
                    DrawSliderInt(0, 1, RDM_ST_AccelerationMovement_Charges, "How many charges to keep ready?\n (0 = Use All)");
                    break;

                case CustomComboPreset.RDM_AoE_Accel:
                    DrawSliderInt(0, 1, RDM_AoE_Acceleration_Charges, "How many charges to keep ready?\n (0 = Use All)");
                    break;

                case CustomComboPreset.RDM_AoE_Accel_Movement:
                    DrawSliderInt(0, 1, RDM_AoE_AccelerationMovement_Charges, "How many charges to keep ready?\n (0 = Use All)");
                    break;

                case CustomComboPreset.RDM_Riposte_oGCD:
                    DrawOGCDOptions(RDM_Riposte_oGCD_Actions, RDM_Riposte_oGCD_Engagement_Pooling, RDM_Riposte_oGCD_CorpACorps_Pooling, RDM_Riposte_oGCD_CorpACorps_Melee);
                    break;

                case CustomComboPreset.RDM_Reprise:
                    DrawOGCDOptions(RDM_Reprise_oGCD_Actions, RDM_Reprise_oGCD_Engagement_Pooling, RDM_Reprise_oGCD_CorpACorps_Pooling, RDM_Reprise_oGCD_CorpACorps_Melee);
                    break;

            }
        }
    }
}
