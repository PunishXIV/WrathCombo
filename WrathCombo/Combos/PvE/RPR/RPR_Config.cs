using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.RPR_ST_Opener:
                    DrawBossOnlyChoice(RPR_Balance_Content);
                    break;

                case Preset.RPR_ST_ArcaneCircle:
                    DrawSliderInt(0, 50, RPR_ST_ArcaneCircleHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircleBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircleBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.RPR_ST_AdvancedMode:
                    DrawHorizontalRadioButton(RPR_Positional,
                        RPR_Config.RearFirst,
                        FormatAndCache(RPR_Config.FirstPositional0, Gallows.ActionName()), 0);

                    DrawHorizontalRadioButton(RPR_Positional,
                        RPR_Config.FlankFirst,
                        FormatAndCache(RPR_Config.FirstPositional0, Gibbet.ActionName()), 1);
                    break;

                case Preset.RPR_ST_SoD:
                    DrawSliderInt(0, 10, RPR_SoDRefreshRange,
                        FormatAndCache(RPR_Config.SecondsBeforeRefreshing0, ShadowOfDeath.ActionName()));

                    DrawSliderInt(0, 100, RPR_SoDHPThreshold,
                        FormatAndCache(Generics.HPPercentageFor0NotBeApplied, ShadowOfDeath.ActionName()));
                    break;

                case Preset.RPR_ST_TrueNorthDynamic:
                    DrawSliderInt(0, 1, RPR_ManualTN,
                        Generics.ChargePool);

                    DrawAdditionalBoolChoice(RPR_ST_TrueNorthDynamicHoldCharge,
                        FormatAndCache(Generics.Hold0For1, Role.TrueNorth.ActionName(), Gluttony.ActionName()),
                        FormatAndCache(RPR_Config.WillHoldTNforGluttony, Role.TrueNorth.ActionName(), Gluttony.ActionName(), Gibbet.ActionName(), Gallows.ActionName()));
                    break;

                case Preset.RPR_ST_RangedFiller:
                    DrawAdditionalBoolChoice(RPR_ST_EnhancedHarpe,
                        FormatAndCache(Generics.OnlyUseWith0, Buffs.EnhancedHarpe.StatusName()),
                        FormatAndCache(RPR_Config.OnlyUse0WhileUGot1, Harpe.ActionName(), Buffs.EnhancedHarpe.StatusName()));
                    break;

                case Preset.RPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, RPR_STSecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, RPR_STBloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region AoE

                case Preset.RPR_AoE_WoD:
                    DrawSliderInt(0, 100, RPR_WoDHPThreshold,
                        FormatAndCache(Generics.HPPercentageFor0NotBeApplied, WhorlOfDeath.ActionName()));
                    break;

                case Preset.RPR_AoE_ArcaneCircle:
                    DrawSliderInt(0, 100, RPR_AoE_ArcaneCircleHPThreshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, ArcaneCircle.ActionName()));
                    break;

                case Preset.RPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, RPR_AoESecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, RPR_AoEBloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region Misc

                case Preset.RPR_ST_BasicCombo_SoD:
                    DrawSliderInt(0, 10, RPR_SoDRefreshRangeBasicCombo,
                        FormatAndCache(RPR_Config.SecondsBeforeRefreshing0, ShadowOfDeath.ActionName()));
                    break;

                case Preset.RPR_AoE_BasicCombo_WoD:
                    DrawSliderInt(0, 10, RPR_WoDRefreshRangeBasicCombo,
                        FormatAndCache(RPR_Config.SecondsBeforeRefreshing0, ShadowOfDeath.ActionName()));
                    break;

                case Preset.RPR_Soulsow:
                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        FormatAndCache(RPR_Config.On0, Harpe.ActionName()),
                        FormatAndCache(Generics.Adds0To1, Soulsow.ActionName(), Harpe.ActionName()), 5, 0);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        FormatAndCache(RPR_Config.On0, Slice.ActionName()),
                        FormatAndCache(Generics.Adds0To1, Soulsow.ActionName(), Slice.ActionName()), 5, 1);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        FormatAndCache(RPR_Config.On0, SpinningScythe.ActionName()),
                        FormatAndCache(Generics.Adds0To1, Soulsow.ActionName(), SpinningScythe.ActionName()), 5, 2);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        FormatAndCache(RPR_Config.On0, ShadowOfDeath.ActionName()),
                        FormatAndCache(Generics.Adds0To1, Soulsow.ActionName(), ShadowOfDeath.ActionName()), 5, 3);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        FormatAndCache(RPR_Config.On0, BloodStalk.ActionName()),
                        FormatAndCache(Generics.Adds0To1, Soulsow.ActionName(), BloodStalk.ActionName()), 5, 4);
                    break;

                #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            RPR_Positional = new("RPR_Positional"),
            RPR_Balance_Content = new("RPR_Balance_Content", 1),
            RPR_ST_ArcaneCircleHPOption = new("RPR_ST_ArcaneCircleHPOption", 25),
            RPR_ST_ArcaneCircleBossOption = new("RPR_ST_ArcaneCircleBossOption"),
            RPR_SoDRefreshRange = new("RPR_SoDRefreshRange", 6),
            RPR_SoDHPThreshold = new("RPR_SoDThreshold"),
            RPR_ManualTN = new("RPR_ManualTN"),
            RPR_STSecondWindHPThreshold = new("RPR_STSecondWindThreshold", 40),
            RPR_STBloodbathHPThreshold = new("RPR_STBloodbathThreshold", 30),

            //AoE
            RPR_WoDHPThreshold = new("RPR_WoDThreshold", 40),
            RPR_AoE_ArcaneCircleHPThreshold = new("RPR_AoE_ArcaneCircleHPThreshold", 40),
            RPR_AoESecondWindHPThreshold = new("RPR_AoESecondWindThreshold", 40),
            RPR_AoEBloodbathHPThreshold = new("RPR_AoEBloodbathThreshold", 30),

            //Misc
            RPR_SoDRefreshRangeBasicCombo = new("RPR_SoDRefreshRangeBasicCombo", 6),
            RPR_WoDRefreshRangeBasicCombo = new("RPR_WoDRefreshRangeBasicCombo", 6);

        public static UserBool
            RPR_ST_TrueNorthDynamicHoldCharge = new("RPR_ST_TrueNorthDynamicHoldCharge"),
            RPR_ST_EnhancedHarpe = new("RPR_ST_EnhancedHarpe");

        public static UserBoolArray
            RPR_SoulsowOptions = new("RPR_SoulsowOptions");

        #endregion
    }
}
