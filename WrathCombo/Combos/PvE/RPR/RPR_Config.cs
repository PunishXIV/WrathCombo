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
                case Preset.RPR_ST_Opener:

                    if (DrawHorizontalRadioButton(RPR_Opener_StartChoice,
                        "Normal Opener", $"Starts opener with {Harpe.ActionName()}", 0))
                    {
                        if (!CustomComboFunctions.InCombat())
                            Opener().OpenerStep = 1;
                    }

                    DrawHorizontalRadioButton(RPR_Opener_StartChoice,
                        "Early Opener", $"Starts opener with {ShadowOfDeath.ActionName()} instead, skipping {Harpe.ActionName()}", 1);

                    ImGui.Spacing();

                    DrawBossOnlyChoice(RPR_Balance_Content);
                    break;

                case Preset.RPR_ST_ArcaneCircle:
                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircleBossOption,
                        "All content", $"Uses {ArcaneCircle.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircleBossOption,
                        "Boss encounters Only", $"Only uses {ArcaneCircle.ActionName()} when in Boss encounters.", 1);
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
                    DrawAdditionalBoolChoice(RPR_ST_TrueNorthDynamic_HoldCharge,
                        "Hold True North for Gluttony Option", "Will hold the last charge of True North for use with Gluttony, even when out of position for Gibbet/Gallows.");
                    break;

                case Preset.RPR_ST_RangedFiller:
                    DrawAdditionalBoolChoice(RPR_ST_RangedFillerHarvestMoon,
                        "Add Harvest Moon", "Adds Harvest Moon if available, when outside of melee range. Will not override Communio.");
                    break;

                case Preset.RPR_AoE_WoD:
                    DrawSliderInt(0, 100, RPR_WoDHPThreshold,
                        $"Set a HP% Threshold for when {WhorlOfDeath.ActionName()} will not be automatically applied to the target.");
                    break;

                case Preset.RPR_AoE_ArcaneCircle:
                    DrawSliderInt(0, 100, RPR_AoE_ArcaneCircleHPThreshold,
                        $"Stop Using {ArcaneCircle.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.RPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, RPR_STSecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, RPR_STBloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.RPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, RPR_AoESecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, RPR_AoEBloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
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
            }
        }

        #region Variables

        public static UserInt
            RPR_Positional = new("RPR_Positional", 0),
            RPR_Opener_StartChoice = new("RPR_Opener_StartChoice", 0),
            RPR_Balance_Content = new("RPR_Balance_Content", 1),
            RPR_SoDRefreshRange = new("RPR_SoDRefreshRange", 6),
            RPR_SoDHPThreshold = new("RPR_SoDThreshold", 0),
            RPR_ST_ArcaneCircleBossOption = new("RPR_ST_ArcaneCircle_SubOption", 1),
            RPR_STSecondWindHPThreshold = new("RPR_STSecondWindThreshold", 40),
            RPR_STBloodbathHPThreshold = new("RPR_STBloodbathThreshold", 30),
            RPR_WoDHPThreshold = new("RPR_WoDThreshold", 40),
            RPR_AoE_ArcaneCircleHPThreshold = new("RPR_AoE_ArcaneCircleHPThreshold", 40),
            RPR_AoESecondWindHPThreshold = new("RPR_AoESecondWindThreshold", 40),
            RPR_AoEBloodbathHPThreshold = new("RPR_AoEBloodbathThreshold", 30);

        public static UserBool
            RPR_ST_TrueNorthDynamic_HoldCharge = new("RPR_ST_TrueNorthDynamic_HoldCharge"),
            RPR_ST_RangedFillerHarvestMoon = new("RPR_ST_RangedFillerHarvestMoon");

        public static UserBoolArray
            RPR_SoulsowOptions = new("RPR_SoulsowOptions");

        #endregion
    }
}
