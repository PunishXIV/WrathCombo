using ECommons.ImGuiMethods;
using System.Numerics;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.SAM_ST_Opener:
                    ImGui.Indent();
                    DrawBossOnlyChoice(SAM_Balance_Content);
                    ImGui.Unindent();
                    ImGuiEx.Spacing(new Vector2(0, 10));

                    DrawSliderInt(0, 13, SAM_Opener_PrePullDelay,
                        FormatAndCache(SAM_Config.SecondsDelayFromFirstStep, MeikyoShisui.ActionName()), 75f.Scale());

                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip(FormatAndCache(SAM_Config.DelaySavageBlade, All.SavageBlade.ActionName()));

                    ImGuiEx.Spacing(new Vector2(0, 10));
                    ImGui.NewLine();

                    DrawHorizontalRadioButton(SAM_Opener_IncludeGyoten,
                        FormatAndCache(SAM_Config.Include2x0, Gyoten.ActionName()),
                        FormatAndCache(SAM_Config.IncludeBoth0, Gyoten.ActionName()), 0);

                    DrawHorizontalRadioButton(SAM_Opener_IncludeGyoten,
                        SAM_Config.SkipBoth,
                        FormatAndCache(SAM_Config.SkipBothUsageOf0, Gyoten.ActionName()), 1);

                    DrawHorizontalRadioButton(SAM_Opener_IncludeGyoten,
                        SAM_Config.SkipFirst,
                        FormatAndCache(SAM_Config.SkipFirstUseOf0, Gyoten.ActionName()), 2);

                    DrawHorizontalRadioButton(SAM_Opener_IncludeGyoten,
                        SAM_Config.SkipSecond,
                        FormatAndCache(SAM_Config.SkipSecondUseOf0, Gyoten.ActionName()), 3);
                    break;

                case Preset.SAM_ST_CDs_UseHiganbana:
                    DrawSliderInt(0, 100, SAM_ST_HiganbanaBossOption,
                        Generics.BossOnlyHpPercent);

                    DrawSliderInt(0, 100, SAM_ST_HiganbanaBossAddsOption,
                        Generics.BossEncounterNonBossHpPercent);

                    DrawSliderInt(0, 100, SAM_ST_HiganbanaTrashOption,
                        Generics.NonBossHpPercent);

                    ImGui.Indent();
                    DrawSliderInt(0, 15, SAM_ST_HiganbanaRefresh,
                        FormatAndCache(Generics.DoTSecondsRemainingZeroDisable, Higanbana.ActionName()));
                    ImGui.Unindent();
                    break;

                case Preset.SAM_ST_CDs_Senei:
                    DrawAdditionalBoolChoice(SAM_ST_CDs_Guren,
                        FormatAndCache(Generics._0Option, Guren.ActionName()),
                        FormatAndCache(SAM_Config.Add0IfSeneiNotUnlocked, Guren.ActionName(), Senei.ActionName()));
                    break;

                case Preset.SAM_ST_CDs_OgiNamikiri:
                    DrawAdditionalBoolChoice(SAM_ST_CDs_OgiNamikiri_Movement,
                        Generics.MovementOption,
                        FormatAndCache(SAM_Config.Add0And1WhenNotMoving, OgiNamikiri.ActionName(), KaeshiNamikiri.ActionName()));
                    break;

                case Preset.SAM_ST_Shinten:
                    DrawSliderInt(50, 85, SAM_ST_KenkiOvercapAmount,
                        SAM_Config.KenkiOvercapAmount);

                    DrawSliderInt(0, 100, SAM_ST_ExecuteThreshold,
                        SAM_Config.HPPercentKenki);
                    break;

                case Preset.SAM_ST_CDs_MeikyoShisui:
                    DrawSliderInt(0, 100, SAM_ST_MeikyoExecuteThreshold,
                        FormatAndCache(SAM_Config.HPPercentMeikyo, MeikyoShisui.ActionName()));
                    break;


                case Preset.SAM_ST_GekkoCombo:
                    DrawAdditionalBoolChoice(SAM_Gekko_KenkiOvercap,
                        SAM_Config.KenkiOvercapProtection,
                        SAM_Config.KenkiOvercapAmount);

                    if (SAM_Gekko_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Gekko_KenkiOvercapAmount,
                            SAM_Config.KenkiAmount, sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_ST_KashaCombo:
                    DrawAdditionalBoolChoice(SAM_Kasha_KenkiOvercap,
                        SAM_Config.KenkiOvercapProtection,
                        SAM_Config.KenkiOvercapAmount);

                    if (SAM_Kasha_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Kasha_KenkiOvercapAmount,
                            SAM_Config.KenkiAmount, sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_ST_YukikazeCombo:
                    DrawHorizontalRadioButton(SAM_ST_YukikazeCombo_Prio,
                        SAM_Config.PrioSenGen,
                        SAM_Config.PrioSenGenDesc, 0);

                    DrawHorizontalRadioButton(SAM_ST_YukikazeCombo_Prio,
                        Generics.PrioBuffUpkeep,
                        SAM_Config.PrioBuffUpkeepDesc, 1);

                    DrawAdditionalBoolChoice(SAM_Yukaze_Gekko,
                        FormatAndCache(Generics.Add0Combo, Gekko.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Gekko.ActionName()));

                    DrawAdditionalBoolChoice(SAM_Yukaze_Kasha,
                        FormatAndCache(Generics.Add0Combo, Kasha.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Kasha.ActionName()));

                    DrawAdditionalBoolChoice(SAM_Yukaze_KenkiOvercap,
                        SAM_Config.KenkiOvercapProtection,
                        SAM_Config.KenkiOvercapAmount);

                    if (SAM_Yukaze_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Yukaze_KenkiOvercapAmount,
                            SAM_Config.KenkiAmount, sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_ST_TrueNorth:
                    DrawSliderInt(0, 1, SAM_ST_ManualTN,
                        Generics.ChargePool);
                    break;

                case Preset.SAM_ST_Meditate:
                    ImGui.SetCursorPosX(48f.Scale());
                    DrawSliderFloat(0, 3, SAM_ST_MeditateTimeStill,
                        Generics.StationaryDelayCheck, decimals: 1);
                    break;

                case Preset.SAM_ST_ComboHeals:
                    DrawSliderInt(0, 100, SAM_STSecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, SAM_STBloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region AoE

                case Preset.SAM_AoE_Kyuten:
                    DrawSliderInt(25, 85, SAM_AoE_KenkiOvercapAmount,
                        SAM_Config.KenkiOvercapAmount);
                    break;

                case Preset.SAM_AoE_OkaCombo:
                    DrawAdditionalBoolChoice(SAM_Oka_KenkiOvercap,
                        SAM_Config.KenkiOvercapProtection,
                        SAM_Config.KenkiOvercapAmount);

                    if (SAM_Oka_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Oka_KenkiOvercapAmount,
                            SAM_Config.KenkiAmount, sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_AoE_MangetsuCombo:
                    DrawAdditionalBoolChoice(SAM_Mangetsu_Oka,
                        FormatAndCache(Generics.Add0Combo, Oka.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Oka.ActionName()));

                    DrawAdditionalBoolChoice(SAM_Mangetsu_KenkiOvercap,
                        SAM_Config.KenkiOvercapProtection,
                        SAM_Config.KenkiOvercapAmount);

                    if (SAM_Mangetsu_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Mangetsu_KenkiOvercapAmount,
                            SAM_Config.KenkiAmount, sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_AoE_ComboHeals:
                    DrawSliderInt(0, 100, SAM_AoESecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, SAM_AoEBloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region Misc

                case Preset.SAM_OgiShoha:
                    DrawAdditionalBoolChoice(SAM_OgiShohaZanshin,
                        FormatAndCache(Generics.Add0, Zanshin.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Zanshin.ActionName()));
                    break;

                #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            SAM_Balance_Content = new("SAM_Balance_Content", 1),
            SAM_Opener_PrePullDelay = new("SAM_Opener_PrePullDelay", 13),
            SAM_Opener_IncludeGyoten = new("SAM_Opener_IncludeGyoten"),
            SAM_ST_HiganbanaBossOption = new("SAM_ST_HiganbanaBossOption"),
            SAM_ST_HiganbanaBossAddsOption = new("SAM_ST_HiganbanaBossAddsOption", 25),
            SAM_ST_HiganbanaTrashOption = new("SAM_ST_HiganbanaTrashOption", 100),
            SAM_ST_HiganbanaRefresh = new("SAM_ST_Higanbana_Refresh", 15),
            SAM_ST_KenkiOvercapAmount = new("SAM_ST_KenkiOvercapAmount", 65),
            SAM_ST_YukikazeCombo_Prio = new("SAM_ST_YukikazeCombo_Prio", 1),
            SAM_ST_ExecuteThreshold = new("SAM_ST_ExecuteThreshold", 5),
            SAM_ST_MeikyoExecuteThreshold = new("SAM_ST_MeikyoExecuteThreshold", 5),
            SAM_ST_ManualTN = new("SAM_ST_ManualTN"),
            SAM_STSecondWindHPThreshold = new("SAM_STSecondWindThreshold", 40),
            SAM_STBloodbathHPThreshold = new("SAM_STBloodbathThreshold", 30),

            //AoE
            SAM_AoE_KenkiOvercapAmount = new("SAM_AoE_KenkiOvercapAmount", 50),
            SAM_AoESecondWindHPThreshold = new("SAM_AoESecondWindThreshold", 40),
            SAM_AoEBloodbathHPThreshold = new("SAM_AoEBloodbathThreshold", 30),

            //Misc
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
            SAM_Mangetsu_KenkiOvercap = new("SAM_Mangetsu_KenkiOvercap"),
            SAM_OgiShohaZanshin = new("SAM_OgiShohaZanshin");

        public static UserFloat
            SAM_ST_MeditateTimeStill = new("SAM_ST_MeditateTimeStill", 2.5f);

        #endregion
    }
}
