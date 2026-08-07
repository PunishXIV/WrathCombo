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
        public static UserInt
            SAM_Balance_Content = new("SAM_Balance_Content", 1),
            SAM_ST_Opener_IncludeGyoten = new("SAM_ST_Opener_IncludeGyoten"),
            SAM_ST_HiganbanaHPOption = new("SAM_ST_HiganbanaHPOption"),
            SAM_ST_HiganbanaAddsHPOption = new("SAM_ST_HiganbanaAddsHPOption", 25),
            SAM_ST_HiganbanaTrashHPOption = new("SAM_ST_HiganbanaTrashHPOption", 100),
            SAM_ST_HiganbanaRefresh = new("SAM_ST_HiganbanaRefresh", 15),
            SAM_ST_ShintenKenkiOvercap = new("SAM_ST_ShintenKenkiOvercap", 65),
            SAM_ST_YukikazeCombo_Prio = new("SAM_ST_YukikazeCombo_Prio", 1),
            SAM_ST_ShintenExecuteHP = new("SAM_ST_ShintenExecuteHP", 5),
            SAM_ST_MeikyoExecuteHP = new("SAM_ST_MeikyoExecuteHP", 5),
            SAM_ST_TrueNorthCharges = new("SAM_ST_TrueNorthCharges"),
            SAM_ST_SecondWindOption = new("SAM_ST_SecondWindOption", 40),
            SAM_ST_BloodbathOption = new("SAM_ST_BloodbathOption", 30),
            SAM_AoE_KyutenKenkiOvercap = new("SAM_AoE_KyutenKenkiOvercap", 50),
            SAM_AoE_SecondWindOption = new("SAM_AoE_SecondWindOption", 40),
            SAM_AoE_BloodbathOption = new("SAM_AoE_BloodbathOption", 30),
            SAM_Gekko_KenkiOvercapAmount = new("SAM_Gekko_KenkiOvercapAmount", 65),
            SAM_Kasha_KenkiOvercapAmount = new("SAM_Kasha_KenkiOvercapAmount", 65),
            SAM_Yukikaze_KenkiOvercapAmount = new("SAM_Yukikaze_KenkiOvercapAmount", 65),
            SAM_Oka_KenkiOvercapAmount = new("SAM_Oka_KenkiOvercapAmount", 50),
            SAM_Mangetsu_KenkiOvercapAmount = new("SAM_Mangetsu_KenkiOvercapAmount", 50);

        public static UserBool
            SAM_ST_Opener_Potion = new("SAM_ST_Opener_Potion"),
            SAM_Gekko_KenkiOvercap = new("SAM_Gekko_KenkiOvercap"),
            SAM_Kasha_KenkiOvercap = new("SAM_Kasha_KenkiOvercap"),
            SAM_Yukikaze_KenkiOvercap = new("SAM_Yukikaze_KenkiOvercap"),
            SAM_Yukikaze_Gekko = new("SAM_Yukikaze_Gekko"),
            SAM_Yukikaze_Kasha = new("SAM_Yukikaze_Kasha"),
            SAM_Mangetsu_Oka = new("SAM_Mangetsu_Oka"),
            SAM_ST_Senei_Guren = new("SAM_ST_Senei_Guren"),
            SAM_ST_OgiNamikiri_Movement = new("SAM_ST_OgiNamikiri_Movement"),
            SAM_Oka_KenkiOvercap = new("SAM_Oka_KenkiOvercap"),
            SAM_Mangetsu_KenkiOvercap = new("SAM_Mangetsu_KenkiOvercap"),
            SAM_OgiShohaZanshin = new("SAM_OgiShohaZanshin");

        public static UserFloat
            SAM_ST_MeditateTimeStill = new("SAM_ST_MeditateTimeStill", 2.5f);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.SAM_ST_Adv_Opener:
                    DrawBossOnlyChoice(SAM_Balance_Content);
                    DrawOpenerPotionChoice(SAM_ST_Opener_Potion);

                    ImGui.TextWrapped(SAM_Config.SecondsDelayFromFirstStep);
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip(FormatAndCache(SAM_Config.DelaySavageBlade, All.Cease.ActionName()));

                    ImGuiEx.Spacing(new Vector2(0, 10));
                    ImGuiEx.TextUnderlined($"{Gyoten.ActionName()} Settings");
                    ImGui.Spacing();
                    DrawRadioButton(SAM_ST_Opener_IncludeGyoten,
                        FormatAndCache(SAM_Config.Include2x0, Gyoten.ActionName()),
                        FormatAndCache(SAM_Config.IncludeBoth0, Gyoten.ActionName()), 0, descriptionAsTooltip: true);
                    DrawRadioButton(SAM_ST_Opener_IncludeGyoten,
                        SAM_Config.SkipBoth,
                        FormatAndCache(SAM_Config.SkipBothUsageOf0, Gyoten.ActionName()), 1, descriptionAsTooltip: true);
                    DrawRadioButton(SAM_ST_Opener_IncludeGyoten,
                        SAM_Config.SkipFirst,
                        FormatAndCache(SAM_Config.SkipFirstUseOf0, Gyoten.ActionName()), 2, descriptionAsTooltip: true);
                    DrawRadioButton(SAM_ST_Opener_IncludeGyoten,
                        SAM_Config.SkipSecond,
                        FormatAndCache(SAM_Config.SkipSecondUseOf0, Gyoten.ActionName()), 3, descriptionAsTooltip: true);
                    break;

                case Preset.SAM_ST_Adv_Higanbana:
                    DrawSliderInt(0, 100, SAM_ST_HiganbanaHPOption,
                        Generics.BossOnlyHpPercent);

                    DrawSliderInt(0, 100, SAM_ST_HiganbanaAddsHPOption,
                        Generics.BossEncounterNonBossHpPercent);

                    DrawSliderInt(0, 100, SAM_ST_HiganbanaTrashHPOption,
                        Generics.NonBossHpPercent);

                    ImGui.Indent();
                    DrawSliderInt(0, 15, SAM_ST_HiganbanaRefresh,
                        FormatAndCache(Generics.DoTSecondsRemainingZeroDisable, Higanbana.ActionName()));
                    ImGui.Unindent();
                    break;

                case Preset.SAM_ST_Adv_Senei:
                    DrawAdditionalBoolChoice(SAM_ST_Senei_Guren,
                        FormatAndCache(Generics._0Option, Guren.ActionName()),
                        FormatAndCache(SAM_Config.Add0IfSeneiNotUnlocked, Guren.ActionName(), Senei.ActionName()));
                    break;

                case Preset.SAM_ST_Adv_OgiNamikiri:
                    DrawAdditionalBoolChoice(SAM_ST_OgiNamikiri_Movement,
                        Generics.MovementOption,
                        FormatAndCache(SAM_Config.Add0And1WhenNotMoving, OgiNamikiri.ActionName(), KaeshiNamikiri.ActionName()));
                    break;

                case Preset.SAM_ST_Adv_Shinten:
                    DrawSliderInt(50, 85, SAM_ST_ShintenKenkiOvercap,
                        SAM_Config.KenkiOvercapAmount);

                    DrawSliderInt(0, 100, SAM_ST_ShintenExecuteHP,
                        SAM_Config.HPPercentKenki);
                    break;

                case Preset.SAM_ST_Adv_Meikyo:
                    DrawSliderInt(0, 100, SAM_ST_MeikyoExecuteHP,
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

                    DrawAdditionalBoolChoice(SAM_Yukikaze_Gekko,
                        FormatAndCache(Generics.Add0Combo, Gekko.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Gekko.ActionName()));

                    DrawAdditionalBoolChoice(SAM_Yukikaze_Kasha,
                        FormatAndCache(Generics.Add0Combo, Kasha.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Kasha.ActionName()));

                    DrawAdditionalBoolChoice(SAM_Yukikaze_KenkiOvercap,
                        SAM_Config.KenkiOvercapProtection,
                        SAM_Config.KenkiOvercapAmount);

                    if (SAM_Yukikaze_KenkiOvercap)
                        DrawSliderInt(25, 100, SAM_Yukikaze_KenkiOvercapAmount,
                            SAM_Config.KenkiAmount, sliderIncrement: SliderIncrements.Fives);
                    break;

                case Preset.SAM_ST_Adv_TrueNorth:
                    DrawSliderInt(0, 1, SAM_ST_TrueNorthCharges,
                        Generics.ChargePool);
                    break;

                case Preset.SAM_ST_Adv_Meditate:
                    ImGui.SetCursorPosX(48f.Scale());
                    DrawSliderFloat(0, 3, SAM_ST_MeditateTimeStill,
                        Generics.StationaryDelayCheck, decimals: 1);
                    break;

                case Preset.SAM_ST_Adv_ComboHeals:
                    DrawSliderInt(0, 100, SAM_ST_SecondWindOption,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, SAM_ST_BloodbathOption,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                case Preset.SAM_AoE_Adv_Kyuten:
                    DrawSliderInt(25, 85, SAM_AoE_KyutenKenkiOvercap,
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

                case Preset.SAM_AoE_Adv_ComboHeals:
                    DrawSliderInt(0, 100, SAM_AoE_SecondWindOption,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, SAM_AoE_BloodbathOption,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                case Preset.SAM_OgiShoha:
                    DrawAdditionalBoolChoice(SAM_OgiShohaZanshin,
                        FormatAndCache(Generics.Add0, Zanshin.ActionName()),
                        FormatAndCache(Generics.Add0ComboWhenApplicable, Zanshin.ActionName()));
                    break;
            }
        }
    }
}
