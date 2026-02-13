using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
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
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircleBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircleBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.RPR_ST_AdvancedMode:
                    DrawHorizontalRadioButton(RPR_Positional, "Rear First",
                        $"First positional: {Gallows.ActionName()}.", 0);

                    DrawHorizontalRadioButton(RPR_Positional, "Flank First",
                        $"First positional: {Gibbet.ActionName()}.", 1);
                    break;

                case Preset.RPR_ST_SoD:
                    DrawSliderInt(0, 10, RPR_SoDRefreshRange,
                        $"Seconds remaining before refreshing {ShadowOfDeath.ActionName()}.\nRecommended is 6.");

                    DrawSliderInt(0, 100, RPR_SoDHPThreshold,
                        $"Set a HP% Threshold for when {ShadowOfDeath.ActionName()} will not be automatically applied to the target.");
                    break;

                case Preset.RPR_ST_TrueNorthDynamic:
                    DrawSliderInt(0, 1, RPR_ManualTN,
                        "How many charges to keep for manual usage.");

                    DrawAdditionalBoolChoice(RPR_ST_TrueNorthDynamicHoldCharge,
                        "Hold True North for Gluttony Option", "Will hold the last charge of True North for use with Gluttony, even when out of position for Gibbet/Gallows.\n" +
                                                               "If Above Slider is set to 1, it will NOT use the remaining charge for Gluttony, but for manual use.");
                    break;

                case Preset.RPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, RPR_STSecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, RPR_STBloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                #endregion

                #region AoE

                case Preset.RPR_AoE_WoD:
                    DrawSliderInt(0, 100, RPR_WoDHPThreshold,
                        $"Set a HP% Threshold for when {WhorlOfDeath.ActionName()} will not be automatically applied to the target.");
                    break;

                case Preset.RPR_AoE_ArcaneCircle:
                    DrawSliderInt(0, 100, RPR_AoE_ArcaneCircleHPThreshold,
                        $"Stop Using {ArcaneCircle.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.RPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, RPR_AoESecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, RPR_AoEBloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                #endregion

                #region Misc

                case Preset.RPR_ST_BasicCombo_SoD:
                    DrawSliderInt(0, 10, RPR_SoDRefreshRangeBasicCombo,
                        $"Seconds remaining before refreshing {ShadowOfDeath.ActionName()}.");
                    break;

                case Preset.RPR_Soulsow:
                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        $"{Harpe.ActionName()}", $"Adds {Soulsow.ActionName()} to {Harpe.ActionName()}.",
                        5, 0);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        $"{Slice.ActionName()}", $"Adds {Soulsow.ActionName()} to {Slice.ActionName()}.",
                        5, 1);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        $"{SpinningScythe.ActionName()}", $"Adds {Soulsow.ActionName()} to {SpinningScythe.ActionName()}", 5, 2);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        $"{ShadowOfDeath.ActionName()}", $"Adds {Soulsow.ActionName()} to {ShadowOfDeath.ActionName()}.", 5, 3);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions,
                        $"{BloodStalk.ActionName()}", $"Adds {Soulsow.ActionName()} to {BloodStalk.ActionName()}.", 5, 4);
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
            RPR_SoDRefreshRangeBasicCombo = new("RPR_SoDRefreshRangeBasicCombo", 6);

        public static UserBool
            RPR_ST_TrueNorthDynamicHoldCharge = new("RPR_ST_TrueNorthDynamicHoldCharge");

        public static UserBoolArray
            RPR_SoulsowOptions = new("RPR_SoulsowOptions");

        #endregion
    }
}
