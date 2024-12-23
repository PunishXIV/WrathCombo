using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Functions.SliderIncrements;

namespace WrathCombo.Combos.PvE
{
    internal static partial class WHM
    {
        public static class Config
        {
            internal static UserInt
                WHM_STDPS_Lucid = new("WHMLucidDreamingFeature"),
                WHM_STDPS_MainCombo_DoT = new("WHM_ST_MainCombo_DoT"),
                WHM_AoEDPS_Lucid = new("WHM_AoE_Lucid", 6500),
                WHM_STHeals_Lucid = new("WHM_STHeals_Lucid"),
                WHM_STHeals_ThinAir = new("WHM_STHeals_ThinAir"),
                WHM_STHeals_Esuna = new("WHM_Cure2_Esuna", 100),
                WHM_STHeals_BenedictionHP = new("WHM_STHeals_BenedictionHP", 99),
                WHM_STHeals_TetraHP = new("WHM_STHeals_TetraHP", 99),
                WHM_STHeals_BenisonHP = new("WHM_STHeals_BenisonHP", 99),
                WHM_STHeals_AquaveilHP = new("WHM_STHeals_AquaveilHP", 99),
                WHM_AoEHeals_Lucid = new("WHM_AoEHeals_Lucid", 6500),
                WHM_AoEHeals_ThinAir = new("WHM_AoE_ThinAir"),
                WHM_AoEHeals_Cure3MP = new("WHM_AoE_Cure3MP"),
                WHM_Balance_Content = new("WHM_Balance_Content");
            internal static UserBool
                WHM_ST_MainCombo_DoT_Adv = new("WHM_ST_MainCombo_DoT_Adv"),
                WHM_ST_MainCombo_Adv = new("WHM_ST_MainCombo_Adv"),
                WHM_ST_MainCombo_Opener_Swiftcast = new("WHM_ST_Opener_Swiftcast"),
                WHM_AoEDPS_PresenceOfMindWeave = new("WHM_AoEDPS_PresenceOfMindWeave"),
                WHM_STHeals_UIMouseOver = new("WHM_STHeals_UIMouseOver"),
                WHM_STHeals_IncludeShields = new("WHM_STHeals_IncludeShields"),
                WHM_STHeals_BenedictionWeave = new("WHM_STHeals_BenedictionWeave"),
                WHM_STHeals_TetraWeave = new("WHM_STHeals_TetraWeave"),
                WHM_STHeals_BenisonWeave = new("WHM_STHeals_BenisonWeave"),
                WHM_STHeals_AquaveilWeave = new("WHM_STHeals_AquaveilWeave"),
                WHM_AoEHeals_PlenaryWeave = new("WHM_AoEHeals_PlenaryWeave"),
                WHM_AoEHeals_AssizeWeave = new("WHM_AoEHeals_AssizeWeave"),
                WHM_AoEHeals_MedicaMO = new("WHM_AoEHeals_MedicaMO");
            internal static UserFloat
                WHM_ST_MainCombo_DoT_Threshold = new("WHM_ST_MainCombo_DoT_Threshold"),
                WHM_STHeals_RegenTimer = new("WHM_STHeals_RegenTimer"),
                WHM_AoEHeals_MedicaTime = new("WHM_AoEHeals_MedicaTime");
            public static UserBoolArray
                WHM_ST_MainCombo_Adv_Actions = new("WHM_ST_MainCombo_Adv_Actions");
            internal static UserIntArray
                WHM_ST_Heals_Priority = new("WHM_ST_Heals_Priority"),
                WHM_AoE_Heals_Priority = new("WHM_AoE_Heals_Priority");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.WHM_ST_MainCombo_Opener:
                        DrawBossOnlyChoice(WHM_Balance_Content);
                        break;

                    case CustomComboPreset.WHM_ST_MainCombo:
                        DrawAdditionalBoolChoice(WHM_ST_MainCombo_Adv, "Advanced Action Options", "Change how actions are handled", isConditionalChoice: true);

                        if (WHM_ST_MainCombo_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawHorizontalMultiChoice(WHM_ST_MainCombo_Adv_Actions, "On Stone/Glare", "Apply options to all Stones and Glares.", 3, 0);
                            DrawHorizontalMultiChoice(WHM_ST_MainCombo_Adv_Actions, "On Aero/Dia", "Apply options to Aeros and Dia.", 3, 1);
                            DrawHorizontalMultiChoice(WHM_ST_MainCombo_Adv_Actions, $"On {Stone2.ActionName()}", $"Apply options to On {Stone2.ActionName()}.", 3, 2);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.WHM_ST_MainCombo_Lucid:
                        DrawSliderInt(4000, 9500, WHM_STDPS_Lucid, "Set value for your MP to be at or under for this feature to work.", 150, Hundreds);
                        break;

                    case CustomComboPreset.WHM_ST_MainCombo_DoT:
                        DrawSliderInt(0, 100, WHM_STDPS_MainCombo_DoT, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                        DrawAdditionalBoolChoice(WHM_ST_MainCombo_DoT_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (WHM_ST_MainCombo_DoT_Adv)
                        {
                            ImGui.Indent();
                            DrawRoundedSliderFloat(0, 4, WHM_ST_MainCombo_DoT_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.WHM_AoE_DPS_Lucid:
                        DrawSliderInt(4000, 9500, WHM_AoEDPS_Lucid, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                        break;

                    case CustomComboPreset.WHM_AoE_DPS_PresenceOfMind:
                        DrawAdditionalBoolChoice(WHM_AoEDPS_PresenceOfMindWeave, "Only Weave or Use Whilst Moving.", "Holy has no weave window so this can only be used to weave against healing or single target damage spells.\n\n" +
                                                "This will minimize the amount of GCD delay you will get but reduce the frequency it will be used.");
                        break;
                    case CustomComboPreset.WHM_AoEHeals_Lucid:
                        DrawSliderInt(4000, 9500, WHM_AoEHeals_Lucid, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                        break;

                    case CustomComboPreset.WHM_STHeals_Lucid:
                        DrawSliderInt(4000, 9500, WHM_STHeals_Lucid, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                        break;

                    case CustomComboPreset.WHM_STHeals_Esuna:
                        DrawSliderInt(0, 100, WHM_STHeals_Esuna, "Stop using when below HP %. Set to Zero to disable this check");
                        break;

                    case CustomComboPreset.WHM_AoEHeals_ThinAir:
                        DrawSliderInt(0, 1, WHM_AoEHeals_ThinAir, "How many charges to keep ready? (0 = Use all)");
                        break;

                    case CustomComboPreset.WHM_AoEHeals_Cure3:
                        DrawSliderInt(1500, 8500, WHM_AoEHeals_Cure3MP, "Use when MP is above", sliderIncrement: 500);
                        break;

                    case CustomComboPreset.WHM_STHeals:
                        DrawAdditionalBoolChoice(WHM_STHeals_UIMouseOver, "Party UI Mouseover Checking", "Check party member's HP & Debuffs by using mouseover on the party list.\nTo be used in conjunction with Redirect/Reaction/etc.");
                        DrawAdditionalBoolChoice(WHM_STHeals_IncludeShields, "Include Shields in HP Percent Sliders", "");
                        break;

                    case CustomComboPreset.WHM_STHeals_ThinAir:
                        DrawSliderInt(0, 1, WHM_STHeals_ThinAir, "How many charges to keep ready? (0 = Use all)");
                        break;

                    case CustomComboPreset.WHM_STHeals_Regen:
                        DrawRoundedSliderFloat(0f, 6f, WHM_STHeals_RegenTimer, "Time Remaining Before Refreshing");
                        break;

                    case CustomComboPreset.WHM_STHeals_Benediction:
                        DrawAdditionalBoolChoice(WHM_STHeals_BenedictionWeave, "Only Weave", "");
                        DrawSliderInt(1, 100, WHM_STHeals_BenedictionHP, "Use when target HP% is at or below.");
                        DrawPriorityInput(WHM_ST_Heals_Priority, 4, 0, $"{Benediction.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.WHM_STHeals_Tetragrammaton:
                        DrawAdditionalBoolChoice(WHM_STHeals_TetraWeave, "Only Weave", "");
                        DrawSliderInt(1, 100, WHM_STHeals_TetraHP, "Use when target HP% is at or below.");
                        DrawPriorityInput(WHM_ST_Heals_Priority, 4, 1, $"{Tetragrammaton.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.WHM_STHeals_Benison:
                        DrawAdditionalBoolChoice(WHM_STHeals_BenisonWeave, "Only Weave", "");
                        DrawSliderInt(1, 100, WHM_STHeals_BenisonHP, "Use when target HP% is at or below.");
                        DrawPriorityInput(WHM_ST_Heals_Priority, 4, 2, $"{DivineBenison.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.WHM_STHeals_Aquaveil:
                        DrawAdditionalBoolChoice(WHM_STHeals_AquaveilWeave, "Only Weave", "");
                        DrawSliderInt(1, 100, WHM_STHeals_AquaveilHP, "Use when target HP% is at or below.");
                        DrawPriorityInput(WHM_ST_Heals_Priority, 4, 3, $"{Aquaveil.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.WHM_AoEHeals_Assize:
                        DrawAdditionalBoolChoice(WHM_AoEHeals_AssizeWeave, "Only Weave", "");
                        break;

                    case CustomComboPreset.WHM_AoEHeals_Plenary:
                        DrawAdditionalBoolChoice(WHM_AoEHeals_PlenaryWeave, "Only Weave", "");
                        break;

                    case CustomComboPreset.WHM_AoEHeals_Medica2:
                        DrawRoundedSliderFloat(0f, 6f, WHM_AoEHeals_MedicaTime, "Time Remaining on Buff to Renew");
                        DrawAdditionalBoolChoice(WHM_AoEHeals_MedicaMO, "Party UI Mousover Checking", "Check your mouseover target for the Medica II/III buff.\nTo be used in conjunction with Redirect/Reaction/etc.");
                        break;
                }

            }
        }
    }
}
