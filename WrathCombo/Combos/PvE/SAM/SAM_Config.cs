using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    internal static class Config
    {
        public static UserInt
            SAM_Balance_Content = new("SAM_Balance_Content", 1),
            SAM_Opener_PrePullDelay = new("SAM_Opener_PrePullDelay", 13),
            SAM_ST_KenkiOvercapAmount = new(nameof(SAM_ST_KenkiOvercapAmount), 65),
            SAM_ST_Higanbana_Suboption = new("SAM_ST_Higanbana_Suboption", 1),
            SAM_ST_Higanbana_Threshold = new("SAM_ST_Higanbana_Threshold", 0),
            SAM_ST_ExecuteThreshold = new("SAM_ST_ExecuteThreshold", 1),
            SAM_STSecondWindThreshold = new("SAM_STSecondWindThreshold", 25),
            SAM_STBloodbathThreshold = new("SAM_STBloodbathThreshold", 40),
            SAM_AoE_KenkiOvercapAmount = new(nameof(SAM_AoE_KenkiOvercapAmount), 50),
            SAM_AoESecondWindThreshold = new("SAM_AoESecondWindThreshold", 25),
            SAM_AoEBloodbathThreshold = new("SAM_AoEBloodbathThreshold", 40),
            SAM_Gekko_KenkiOvercapAmount = new(nameof(SAM_Gekko_KenkiOvercapAmount), 65),
            SAM_Kasha_KenkiOvercapAmount = new(nameof(SAM_Kasha_KenkiOvercapAmount), 65),
            SAM_Yukaze_KenkiOvercapAmount = new(nameof(SAM_Yukaze_KenkiOvercapAmount), 65),
            SAM_Oka_KenkiOvercapAmount = new(nameof(SAM_Oka_KenkiOvercapAmount), 50),
            SAM_Mangetsu_KenkiOvercapAmount = new(nameof(SAM_Mangetsu_KenkiOvercapAmount), 50),
            SAM_VariantCure = new("SAM_VariantCure");

        public static UserBool
            SAM_Gekko_KenkiOvercap = new(nameof(SAM_Gekko_KenkiOvercap)),
            SAM_Kasha_KenkiOvercap = new(nameof(SAM_Kasha_KenkiOvercap)),
            SAM_Yukaze_KenkiOvercap = new(nameof(SAM_Yukaze_KenkiOvercap)),
            SAM_Oka_KenkiOvercap = new(nameof(SAM_Oka_KenkiOvercap)),
            SAM_Mangetsu_KenkiOvercap = new(nameof(SAM_Mangetsu_KenkiOvercap));

        public static UserBoolArray
            SAM_ST_CDs_IaijutsuOption = new("SAM_ST_CDs_IaijutsuOption");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.SAM_ST_Opener:
                    DrawBossOnlyChoice(SAM_Balance_Content);
                    ImGui.NewLine();
                    DrawSliderInt(0, 13, SAM_Opener_PrePullDelay, $"Delay from first {MeikyoShisui.ActionName()} to next step. (seconds)\nDelay is enforced by replacing your button with Savage Blade.");
                    break;

                case CustomComboPreset.SAM_ST_CDs_Iaijutsu:
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Add {Higanbana.ActionName()}", "Will use Higanbana depending on suboptions.", 4, 0);
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Add {TenkaGoken.ActionName()}", "Will Use Tenka Goken when lvlsynched below lvl 50.", 4, 1);
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Use {MidareSetsugekka.ActionName()}", "Will use Midare Setsugekka and Tendo Setsugekka.", 4, 2);
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Use {TsubameGaeshi.ActionName()}", "Will use Tsubame-gaeshi and Tendo Kaeshi Setsugekka.", 4, 3);

                    if (SAM_ST_CDs_IaijutsuOption[0])
                    {
                        DrawSliderInt(0, 10, SAM_ST_Higanbana_Threshold,
                            $"Stop using {Higanbana.ActionName()} on targets below this HP % (0% = always use).");

                        ImGui.Indent();
                        DrawHorizontalRadioButton(SAM_ST_Higanbana_Suboption,
                            "All Enemies",
                            $"Uses {Higanbana.ActionName()} regardless of targeted enemy type.", 0);

                        DrawHorizontalRadioButton(SAM_ST_Higanbana_Suboption,
                            "Bosses Only",
                            $"Only uses {Higanbana.ActionName()} when the targeted enemy is a boss.", 1);
                        ImGui.Unindent();
                    }

                    break;

                case CustomComboPreset.SAM_ST_ComboHeals:
                    DrawSliderInt(0, 100, SAM_STSecondWindThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, SAM_STBloodbathThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.SAM_AoE_ComboHeals:
                    DrawSliderInt(0, 100, SAM_AoESecondWindThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, SAM_AoEBloodbathThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.SAM_ST_Shinten:
                    DrawSliderInt(25, 85, SAM_ST_KenkiOvercapAmount,
                        "Set the Kenki overcap amount for ST combos.");
                    DrawSliderInt(0, 100, SAM_ST_ExecuteThreshold, "HP percent threshold to not save Kenki");

                    break;

                case CustomComboPreset.SAM_AoE_Kyuten:
                    DrawSliderInt(25, 85, SAM_AoE_KenkiOvercapAmount,
                        "Set the Kenki overcap amount for AOE combos.");

                    break;

                case CustomComboPreset.SAM_Variant_Cure:
                    DrawSliderInt(1, 100, SAM_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.SAM_ST_GekkoCombo:
                {
                    DrawAdditionalBoolChoice(SAM_Gekko_KenkiOvercap, "Kenki Overcap Protection",
                        "Spends Kenki when at the set value or above.");

                    if (SAM_Gekko_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Gekko_KenkiOvercapAmount, "Kenki Amount",
                            sliderIncrement: SliderIncrements.Fives);

                    break;
                }

                case CustomComboPreset.SAM_ST_KashaCombo:
                {
                    DrawAdditionalBoolChoice(SAM_Kasha_KenkiOvercap, "Kenki Overcap Protection",
                        "Spends Kenki when at the set value or above.");

                    if (SAM_Kasha_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Kasha_KenkiOvercapAmount, "Kenki Amount",
                            sliderIncrement: SliderIncrements.Fives);

                    break;
                }

                case CustomComboPreset.SAM_ST_YukikazeCombo:
                {
                    DrawAdditionalBoolChoice(SAM_Yukaze_KenkiOvercap, "Kenki Overcap Protection",
                        "Spends Kenki when at the set value or above.");

                    if (SAM_Yukaze_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Yukaze_KenkiOvercapAmount, "Kenki Amount",
                            sliderIncrement: SliderIncrements.Fives);

                    break;
                }

                case CustomComboPreset.SAM_AoE_OkaCombo:
                {
                    DrawAdditionalBoolChoice(SAM_Oka_KenkiOvercap, "Kenki Overcap Protection",
                        "Spends Kenki when at the set value or above.");

                    if (SAM_Oka_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Oka_KenkiOvercapAmount, "Kenki Amount",
                            sliderIncrement: SliderIncrements.Fives);

                    break;
                }

                case CustomComboPreset.SAM_AoE_MangetsuCombo:
                {
                    DrawAdditionalBoolChoice(SAM_Mangetsu_KenkiOvercap, "Kenki Overcap Protection",
                        "Spends Kenki when at the set value or above.");

                    if (SAM_Mangetsu_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Mangetsu_KenkiOvercapAmount, "Kenki Amount",
                            sliderIncrement: SliderIncrements.Fives);

                    break;
                }
            }
        }
    }
}
