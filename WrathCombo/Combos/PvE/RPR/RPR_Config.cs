using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    internal static class Config
    {
        public static UserInt
            RPR_SoDThreshold = new("RPRSoDThreshold", 0),
            RPR_WoDThreshold = new("RPRWoDThreshold", 1),
            RPR_SoDRefreshRange = new("RPRSoDRefreshRange", 6),
            RPR_ST_ArcaneCircle_SubOption = new("RPR_ST_ArcaneCircle_SubOption", 1),
            RPR_Positional = new("RPR_Positional", 0),
            RPR_VariantCure = new("RPRVariantCure"),
            RPR_STSecondWindThreshold = new("RPR_STSecondWindThreshold", 25),
            RPR_STBloodbathThreshold = new("RPR_STBloodbathThreshold", 40),
            RPR_AoESecondWindThreshold = new("RPR_AoESecondWindThreshold", 25),
            RPR_AoEBloodbathThreshold = new("RPR_AoEBloodbathThreshold", 40),
            RPR_Balance_Content = new("RPR_Balance_Content", 1);

        public static UserBoolArray
            RPR_SoulsowOptions = new("RPR_SoulsowOptions");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.RPR_ST_Opener:
                    DrawBossOnlyChoice(RPR_Balance_Content);
                    break;

                case CustomComboPreset.RPR_ST_ArcaneCircle:
                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircle_SubOption,
                        "All content", $"Uses {ArcaneCircle.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(RPR_ST_ArcaneCircle_SubOption,
                        "Boss encounters Only", $"Only uses {ArcaneCircle.ActionName()} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.RPR_ST_AdvancedMode:
                    DrawHorizontalRadioButton(RPR_Positional, "Rear First",
                        $"First positional: {Gallows.ActionName()}.", 0);

                    DrawHorizontalRadioButton(RPR_Positional, "Flank First",
                        $"First positional: {Gibbet.ActionName()}.", 1);

                    break;

                case CustomComboPreset.RPR_ST_SoD:
                    DrawSliderInt(4, 8, RPR_SoDRefreshRange,
                        $"Seconds remaining before refreshing {ShadowOfDeath.ActionName()}.");

                    DrawSliderInt(0, 5, RPR_SoDThreshold,
                        $"Set a HP% Threshold for when {ShadowOfDeath.ActionName()} will not be automatically applied to the target.");

                    break;

                case CustomComboPreset.RPR_AoE_WoD:
                    DrawSliderInt(0, 5, RPR_WoDThreshold,
                        $"Set a HP% Threshold for when {WhorlOfDeath.ActionName()} will not be automatically applied to the target.");

                    break;

                case CustomComboPreset.RPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, RPR_STSecondWindThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, RPR_STBloodbathThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.RPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, RPR_AoESecondWindThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, RPR_AoEBloodbathThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.RPR_Soulsow:
                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, $"{Harpe.ActionName()}",
                        $"Adds {Soulsow.ActionName()} to {Harpe.ActionName()}.",
                        5, 0);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, $"{Slice.ActionName()}",
                        $"Adds {Soulsow.ActionName()} to {Slice.ActionName()}.",
                        5, 1);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, $"{SpinningScythe.ActionName()}",
                        $"Adds {Soulsow.ActionName()} to {SpinningScythe.ActionName()}", 5, 2);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, $"{ShadowOfDeath.ActionName()}",
                        $"Adds {Soulsow.ActionName()} to {ShadowOfDeath.ActionName()}.", 5, 3);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, $"{BloodStalk.ActionName()}",
                        $"Adds {Soulsow.ActionName()} to {BloodStalk.ActionName()}.", 5, 4);

                    break;

                case CustomComboPreset.RPR_Variant_Cure:
                    DrawSliderInt(1, 100, RPR_VariantCure, "HP% to be at or under", 200);

                    break;

                //PVP
                case CustomComboPreset.RPRPvP_Burst_ImmortalPooling:
                    DrawSliderInt(0, 8, RPRPvP.Config.RPRPvP_ImmortalStackThreshold,
                        "Set a value of Immortal Sacrifice Stacks to hold for burst.");

                    break;

                case CustomComboPreset.RPRPvP_Burst_ArcaneCircle:
                    DrawSliderInt(5, 90, RPRPvP.Config.RPRPvP_ArcaneCircleThreshold,
                        "Set a HP percentage value. Caps at 90 to prevent waste.");

                    break;
            }
        }
    }
}
