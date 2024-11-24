using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE
{
    internal static partial class SGE
    {
        public static class Config
        {
            #region DPS
            public static UserBool
                SGE_ST_DPS_Adv = new("SGE_ST_DPS_Adv"),
                SGE_ST_DPS_EDosis_Adv = new("SGE_ST_Dosis_EDosis_Adv");
            public static UserBoolArray
                SGE_ST_DPS_Movement = new("SGE_ST_DPS_Movement");
            public static UserInt
                SGE_ST_DPS_EDosisHPPer = new("SGE_ST_DPS_EDosisHPPer", 10),
                SGE_ST_DPS_Lucid = new("SGE_ST_DPS_Lucid", 6500),
                SGE_ST_DPS_Rhizo = new("SGE_ST_DPS_Rhizo"),
                SGE_ST_DPS_AddersgallProtect = new("SGE_ST_DPS_AddersgallProtect", 3),
                SGE_AoE_DPS_Lucid = new("SGE_AoE_Phlegma_Lucid", 6500),
                SGE_AoE_DPS_Rhizo = new("SGE_AoE_DPS_Rhizo"),
                SGE_AoE_DPS_AddersgallProtect = new("SGE_AoE_DPS_AddersgallProtect", 3);
            public static UserFloat
                SGE_ST_DPS_EDosisThreshold = new("SGE_ST_Dosis_EDosisThreshold", 3.0f);
            #endregion

            #region Healing
            public static UserBool
                SGE_ST_Heal_Adv = new("SGE_ST_Heal_Adv"),
                SGE_ST_Heal_UIMouseOver = new("SGE_ST_Heal_UIMouseOver"),
                SGE_ST_Heal_IncludeShields = new("SGE_ST_Heal_IncludeShields"),
                SGE_AoE_Heal_KeracholeTrait = new("SGE_AoE_Heal_KeracholeTrait");
            public static UserInt
                SGE_ST_Heal_Zoe = new("SGE_ST_Heal_Zoe"),
                SGE_ST_Heal_Haima = new("SGE_ST_Heal_Haima"),
                SGE_ST_Heal_Krasis = new("SGE_ST_Heal_Krasis"),
                SGE_ST_Heal_Pepsis = new("SGE_ST_Heal_Pepsis"),
                SGE_ST_Heal_Soteria = new("SGE_ST_Heal_Soteria"),
                SGE_ST_Heal_EDiagnosisHP = new("SGE_ST_Heal_EDiagnosisHP"),
                SGE_ST_Heal_Druochole = new("SGE_ST_Heal_Druochole"),
                SGE_ST_Heal_Taurochole = new("SGE_ST_Heal_Taurochole"),
                SGE_ST_Heal_Esuna = new("SGE_ST_Heal_Esuna");
            public static UserIntArray
                SGE_ST_Heals_Priority = new("SGE_ST_Heals_Priority"),
                SGE_AoE_Heals_Priority = new("SGE_AoE_Heals_Priority");
            public static UserBoolArray
                SGE_ST_Heal_EDiagnosisOpts = new("SGE_ST_Heal_EDiagnosisOpts");
            #endregion

            public static UserInt
                SGE_Eukrasia_Mode = new("SGE_Eukrasia_Mode");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.SGE_ST_DPS:
                        DrawAdditionalBoolChoice(SGE_ST_DPS_Adv, $"Apply all selected options to {Dosis2.ActionName()}", $"{Dosis.ActionName()} & {Dosis3.ActionName()} will behave normally.");
                        break;

                    case CustomComboPreset.SGE_ST_DPS_EDosis:
                        DrawSliderInt(0, 100, SGE_ST_DPS_EDosisHPPer, "Stop using at Enemy HP %. Set to Zero to disable this check");

                        DrawAdditionalBoolChoice(SGE_ST_DPS_EDosis_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (SGE_ST_DPS_EDosis_Adv)
                        {
                            ImGui.Indent();
                            DrawRoundedSliderFloat(0, 4, SGE_ST_DPS_EDosisThreshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SGE_ST_DPS_Lucid:
                        DrawSliderInt(4000, 9500, SGE_ST_DPS_Lucid, "MP Threshold", 150, Hundreds);
                        break;

                    case CustomComboPreset.SGE_ST_DPS_Rhizo:
                        DrawSliderInt(0, 1, SGE_ST_DPS_Rhizo, "Addersgall Threshold", 150, Ones);
                        break;

                    case CustomComboPreset.SGE_ST_DPS_AddersgallProtect:
                        DrawSliderInt(1, 3, SGE_ST_DPS_AddersgallProtect, "Addersgall Threshold", 150, Ones);
                        break;

                    case CustomComboPreset.SGE_ST_DPS_Movement:
                        DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Toxikon.ActionName(), $"Use {Toxikon.ActionName()} when Addersting == available.", 4, 0);
                        DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Dyskrasia.ActionName(), $"Use {Dyskrasia.ActionName()} when in range of a selected enemy target.", 4, 1);
                        DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Eukrasia.ActionName(), $"Use {Eukrasia.ActionName()}.", 4, 2);
                        DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Psyche.ActionName(), $"Use {Psyche.ActionName()}.", 4, 3);
                        break;

                    case CustomComboPreset.SGE_AoE_DPS_Lucid:
                        DrawSliderInt(4000, 9500, SGE_AoE_DPS_Lucid, "MP Threshold", 150, Hundreds);
                        break;

                    case CustomComboPreset.SGE_AoE_DPS_Rhizo:
                        DrawSliderInt(0, 1, SGE_AoE_DPS_Rhizo, "Addersgall Threshold", 150, Ones);
                        break;

                    case CustomComboPreset.SGE_AoE_DPS_AddersgallProtect:
                        DrawSliderInt(1, 3, SGE_AoE_DPS_AddersgallProtect, "Addersgall Threshold", 150, Ones);
                        break;

                    case CustomComboPreset.SGE_ST_Heal:
                        DrawAdditionalBoolChoice(SGE_ST_Heal_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (SGE_ST_Heal_Adv)
                        {
                            ImGui.Indent();
                            DrawAdditionalBoolChoice(SGE_ST_Heal_UIMouseOver,
                                "Party UI Mouseover Checking",
                                "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                                "To be used in conjunction with Redirect/Reaction/etc");
                            DrawAdditionalBoolChoice(SGE_ST_Heal_IncludeShields, "Include Shields in HP Percent Sliders", "");
                            ImGui.Unindent();
                        }
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Esuna:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Esuna, "Stop using when below HP %. Set to Zero to disable this check");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Soteria:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Soteria, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 0, $"{Soteria.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Zoe:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Zoe, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 1, $"{Zoe.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Pepsis:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Pepsis, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 2, $"{Pepsis.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Taurochole:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Taurochole, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 3, $"{Taurochole.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Haima:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Haima, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 4, $"{Haima.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Krasis:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Krasis, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 5, $"{Krasis.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_Druochole:
                        DrawSliderInt(0, 100, SGE_ST_Heal_Druochole, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawPriorityInput(SGE_ST_Heals_Priority, 7, 6, $"{Druochole.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_ST_Heal_EDiagnosis:
                        DrawSliderInt(0, 100, SGE_ST_Heal_EDiagnosisHP, "Start using when below HP %. Set to 100 to disable this check.");
                        DrawHorizontalMultiChoice(SGE_ST_Heal_EDiagnosisOpts, "Ignore Shield Check", $"Warning, will force the use of {EukrasianDiagnosis.ActionName()}, and normal {Diagnosis.ActionName()} will be unavailable.", 2, 0);
                        DrawHorizontalMultiChoice(SGE_ST_Heal_EDiagnosisOpts, "Check for Scholar Galvenize", "Enable to not override an existing Scholar's shield.", 2, 1);
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Kerachole:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 0, $"{Kerachole.ActionName()} Priority: ");
                        DrawAdditionalBoolChoice(SGE_AoE_Heal_KeracholeTrait,
                                                "Check for Enhanced Kerachole Trait (Heal over Time)",
                                                $"Enabling this will prevent {Kerachole.ActionName()} from being used when the Heal over Time trait == unavailable.");
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Ixochole:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 1, $"{Ixochole.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Physis:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 2, $"{Physis.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Holos:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 3, $"{Holos.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Panhaima:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 4, $"{Panhaima.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Pepsis:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 5, $"{Pepsis.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_AoE_Heal_Philosophia:
                        DrawPriorityInput(SGE_AoE_Heals_Priority, 7, 6, $"{Philosophia.ActionName()} Priority: ");
                        break;

                    case CustomComboPreset.SGE_Eukrasia:
                        DrawRadioButton(SGE_Eukrasia_Mode, $"{EukrasianDosis.ActionName()}", "", 0);
                        DrawRadioButton(SGE_Eukrasia_Mode, $"{EukrasianDiagnosis.ActionName()}", "", 1);
                        DrawRadioButton(SGE_Eukrasia_Mode, $"{EukrasianPrognosis.ActionName()}", "", 2);
                        DrawRadioButton(SGE_Eukrasia_Mode, $"{EukrasianDyskrasia.ActionName()}", "", 3);
                        break;
                }
            }
        }
    }
}
