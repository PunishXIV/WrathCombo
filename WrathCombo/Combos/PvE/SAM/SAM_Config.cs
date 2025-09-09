using System.Numerics;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.SAM_ST_Opener:
                    DrawBossOnlyChoice(SAM_Balance_Content);
                    ImGui.NewLine();
                    DrawSliderInt(0, 13, SAM_Opener_PrePullDelay,
                        $"Delay from first {MeikyoShisui.ActionName()} to next step. (seconds)\nDelay is enforced by replacing your button with Savage Blade.");
                    break;

                case Preset.SAM_ST_CDs_Iaijutsu:
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Add {Higanbana.ActionName()}", "Will use Higanbana depending on suboptions.", 4, 0);
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Add {TenkaGoken.ActionName()}", "Will Use Tenka Goken when lvlsynched below lvl 50.", 4, 1);
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Use {MidareSetsugekka.ActionName()}", "Will use Midare Setsugekka and Tendo Setsugekka.", 4, 2);
                    DrawHorizontalMultiChoice(SAM_ST_CDs_IaijutsuOption, $"Use {TsubameGaeshi.ActionName()}", "Will use Tsubame-gaeshi and Tendo Kaeshi Setsugekka.", 4, 3);

                    if (SAM_ST_CDs_IaijutsuOption[0])
                    {
                        ImGui.Dummy(new Vector2(12f.Scale(), 0));
                        ImGui.SameLine();
                        DrawHorizontalRadioButton(SAM_ST_HiganbanaBossOption,
                            "All Enemies", $"Uses {Higanbana.ActionName()} regardless of targeted enemy type.", 0);

                        DrawHorizontalRadioButton(SAM_ST_HiganbanaBossOption,
                            "Bosses Only", $"Only uses {Higanbana.ActionName()} when the targeted enemy is a boss.", 1);

                        DrawSliderInt(0, 10, SAM_ST_HiganbanaHPThreshold,
                            $"Stop using {Higanbana.ActionName()} on targets below this HP % (0% = always use).");

                        DrawSliderInt(0, 15, SAM_ST_HiganbanaRefresh,
                            $"Seconds remaining before reapplying {Higanbana.ActionName()}. Set to Zero to disable this check.");
                    }
                    break;

                case Preset.SAM_ST_CDs_MeikyoShisui:
                    DrawHorizontalRadioButton(SAM_ST_MeikyoBossOption,
                        "Use The Balance Logic in all content", $"Uses {MeikyoShisui.ActionName()} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(SAM_ST_MeikyoBossOption,
                        "Use The Balance logic only in Boss encounters", $"Only uses {MeikyoShisui.ActionName()} logic when in Boss encounters." +
                                                                         $"\nWill use Meikyo every minute regardless of sen count outside of boss encounters.", 1);
                    break;

                case Preset.SAM_ST_ComboHeals:
                    DrawSliderInt(0, 100, SAM_STSecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, SAM_STBloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.SAM_AoE_ComboHeals:
                    DrawSliderInt(0, 100, SAM_AoESecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, SAM_AoEBloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.SAM_ST_CDs_Senei:
                    DrawAdditionalBoolChoice(SAM_ST_CDs_Guren,
                        "Guren Option", "Adds Guren to the rotation if Senei is not unlocked.");
                    break;

                case Preset.SAM_ST_CDs_OgiNamikiri:
                    DrawAdditionalBoolChoice(SAM_ST_CDs_OgiNamikiri_Movement,
                        "Movement Option", "Adds Ogi Namikiri and Kaeshi: Namikiri when you're not moving.");
                    break;

                case Preset.SAM_ST_Shinten:
                    DrawSliderInt(25, 85, SAM_ST_KenkiOvercapAmount,
                        "Set the Kenki overcap amount for ST combos.");

                    DrawSliderInt(0, 100, SAM_ST_ExecuteThreshold,
                        "HP percent threshold to not save Kenki");
                    break;

                case Preset.SAM_AoE_Kyuten:
                    DrawSliderInt(25, 85, SAM_AoE_KenkiOvercapAmount,
                        "Set the Kenki overcap amount for AOE combos.");
                    break;

                case Preset.SAM_ST_GekkoCombo:
                    DrawAdditionalBoolChoice(SAM_Gekko_KenkiOvercap,
                        "Kenki Overcap Protection", "Spends Kenki when at the set value or above.");

                    if (SAM_Gekko_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Gekko_KenkiOvercapAmount,
                            "Kenki Amount", sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_ST_KashaCombo:
                    DrawAdditionalBoolChoice(SAM_Kasha_KenkiOvercap,
                        "Kenki Overcap Protection", "Spends Kenki when at the set value or above.");

                    if (SAM_Kasha_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Kasha_KenkiOvercapAmount,
                            "Kenki Amount", sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_ST_YukikazeCombo:
                    DrawAdditionalBoolChoice(SAM_Yukaze_Gekko,
                        "Add Gekko Combo", "Adds Gekko combo when applicable.");

                    DrawAdditionalBoolChoice(SAM_Yukaze_Kasha,
                        "Add Kasha Combo", "Adds Kasha combo when applicable.");

                    DrawAdditionalBoolChoice(SAM_Yukaze_KenkiOvercap,
                        "Kenki Overcap Protection", "Spends Kenki when at the set value or above.");

                    if (SAM_Yukaze_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Yukaze_KenkiOvercapAmount,
                            "Kenki Amount", sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_AoE_OkaCombo:
                    DrawAdditionalBoolChoice(SAM_Oka_KenkiOvercap,
                        "Kenki Overcap Protection", "Spends Kenki when at the set value or above.");

                    if (SAM_Oka_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Oka_KenkiOvercapAmount,
                            "Kenki Amount", sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_AoE_MangetsuCombo:
                    DrawAdditionalBoolChoice(SAM_Mangetsu_Oka,
                        "Add Oka Combo", "Adds Oka combo when applicable.");

                    DrawAdditionalBoolChoice(SAM_Mangetsu_KenkiOvercap,
                        "Kenki Overcap Protection", "Spends Kenki when at the set value or above.");

                    if (SAM_Mangetsu_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Mangetsu_KenkiOvercapAmount,
                            "Kenki Amount", sliderIncrement: SliderIncrements.Fives);
                    break;
            }
        }
        #region Variables

        public static UserInt
            SAM_Balance_Content = new("SAM_Balance_Content", 1),
            SAM_Opener_PrePullDelay = new("SAM_Opener_PrePullDelay", 13),
            SAM_ST_KenkiOvercapAmount = new("SAM_ST_KenkiOvercapAmount", 65),
            SAM_ST_HiganbanaBossOption = new("SAM_ST_Higanbana_Suboption", 1),
            SAM_ST_MeikyoBossOption = new("SAM_ST_Meikyo_Suboption", 1),
            SAM_ST_HiganbanaHPThreshold = new("SAM_ST_Higanbana_HP_Threshold", 0),
            SAM_ST_HiganbanaRefresh = new("SAM_ST_Higanbana_Refresh", 15),
            SAM_ST_ExecuteThreshold = new("SAM_ST_ExecuteThreshold", 1),
            SAM_STSecondWindHPThreshold = new("SAM_STSecondWindThreshold", 40),
            SAM_STBloodbathHPThreshold = new("SAM_STBloodbathThreshold", 30),
            SAM_AoE_KenkiOvercapAmount = new("SAM_AoE_KenkiOvercapAmount", 50),
            SAM_AoESecondWindHPThreshold = new("SAM_AoESecondWindThreshold", 40),
            SAM_AoEBloodbathHPThreshold = new("SAM_AoEBloodbathThreshold", 30),
            SAM_Gekko_KenkiOvercapAmount = new("SAM_Gekko_KenkiOvercapAmount", 65),
            SAM_Kasha_KenkiOvercapAmount = new("SAM_Kasha_KenkiOvercapAmount", 65),
            SAM_Yukaze_KenkiOvercapAmount = new("SAM_Yukaze_KenkiOvercapAmount", 65),
            SAM_Oka_KenkiOvercapAmount = new("SAM_Oka_KenkiOvercapAmount", 50),
            SAM_Mangetsu_KenkiOvercapAmount = new("SAM_Mangetsu_KenkiOvercapAmount", 50);

        public static UserBool
            SAM_Gekko_KenkiOvercap = new("SAM_Gekko_KenkiOvercap"),
            SAM_Kasha_KenkiOvercap = new("SAM_Kasha_KenkiOvercap"),
            SAM_Yukaze_KenkiOvercap = new("SAM_Yukaze_KenkiOvercap"),
            SAM_Yukaze_Gekko = new("SAM_Yukaze_Gekko"),
            SAM_Yukaze_Kasha = new("SAM_Yukaze_Kasha"),
            SAM_Mangetsu_Oka = new("SAM_Mangetsu_Oka"),
            SAM_ST_CDs_Guren = new("SAM_ST_CDs_Guren"),
            SAM_ST_CDs_OgiNamikiri_Movement = new("SAM_ST_CDs_OgiNamikiri_Movement"),
            SAM_Oka_KenkiOvercap = new("SAM_Oka_KenkiOvercap"),
            SAM_Mangetsu_KenkiOvercap = new("SAM_Mangetsu_KenkiOvercap");

        public static UserBoolArray
            SAM_ST_CDs_IaijutsuOption = new("SAM_ST_CDs_IaijutsuOption");

        #endregion
    }
}
