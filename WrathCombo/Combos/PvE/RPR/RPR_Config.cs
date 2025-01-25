using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
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
            RPR_Positional = new("RPR_Positional", 0),
            RPR_VariantCure = new("RPRVariantCure"),
            RPR_STSecondWindThreshold = new("RPR_STSecondWindThreshold", 25),
            RPR_STBloodbathThreshold = new("RPR_STBloodbathThreshold", 40),
            RPR_AoESecondWindThreshold = new("RPR_AoESecondWindThreshold", 25),
            RPR_AoEBloodbathThreshold = new("RPR_AoEBloodbathThreshold", 40),
            RPR_Balance_Content = new("RPR_Balance_Content", 1);

        public static UserBoolArray
            RPR_SoulsowOptions = new("RPR_SoulsowOptions");

        public static UserBool
            RPR_ST_TrueNorth_Moving = new("RPR_ST_TrueNorth_Moving");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.RPR_ST_Opener:
                    DrawBossOnlyChoice(RPR_Balance_Content);
                    break;
                case CustomComboPreset.RPRPvP_Burst_ImmortalPooling:
                    DrawSliderInt(0, 8, RPRPvP.Config.RPRPvP_ImmortalStackThreshold,
                        "Set a value of Immortal Sacrifice Stacks to hold for burst.");

                    break;

                case CustomComboPreset.RPRPvP_Burst_ArcaneCircle:
                    DrawSliderInt(5, 90, RPRPvP.Config.RPRPvP_ArcaneCircleThreshold,
                        "Set a HP percentage value. Caps at 90 to prevent waste.");

                    break;

                case CustomComboPreset.RPR_ST_AdvancedMode:
                    DrawHorizontalRadioButton(RPR_Positional, "Rear First",
                        "First positional: Gallows.", 0);

                    DrawHorizontalRadioButton(RPR_Positional, "Flank First",
                        "First positional: Gibbet.", 1);

                    break;

                case CustomComboPreset.RPR_ST_SoD:
                    DrawSliderInt(4, 8, RPR_SoDRefreshRange,
                        "Seconds remaining before refreshing Death's Design.");

                    DrawSliderInt(0, 5, RPR_SoDThreshold,
                        "Set a HP% Threshold for when SoD will not be automatically applied to the target.");

                    break;

                case CustomComboPreset.RPR_AoE_WoD:
                    DrawSliderInt(0, 5, RPR_WoDThreshold,
                        "Set a HP% Threshold for when WoD will not be automatically applied to the target.");

                    break;

                case CustomComboPreset.RPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, RPR_STSecondWindThreshold,
                        "HP percent threshold to use Second Wind below (0 = Disabled)");

                    DrawSliderInt(0, 100, RPR_STBloodbathThreshold,
                        "HP percent threshold to use Bloodbath (0 = Disabled)");

                    break;

                case CustomComboPreset.RPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, RPR_AoESecondWindThreshold,
                        "HP percent threshold to use Second Wind below (0 = Disabled)");

                    DrawSliderInt(0, 100, RPR_AoEBloodbathThreshold,
                        "HP percent threshold to use Bloodbath below (0 = Disabled)");

                    break;

                case CustomComboPreset.RPR_Soulsow:
                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, "Harpe",
                        "Adds Soulsow to Harpe.",
                        5, 0);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, "Slice",
                        "Adds Soulsow to Slice.",
                        5, 1);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, "Spinning Scythe",
                        "Adds Soulsow to Spinning Scythe", 5, 2);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, "Shadow of Death",
                        "Adds Soulsow to Shadow of Death.", 5, 3);

                    DrawHorizontalMultiChoice(RPR_SoulsowOptions, "Blood Stalk",
                        "Adds Soulsow to Blood Stalk.", 5, 4);

                    break;

                case CustomComboPreset.RPR_Variant_Cure:
                    DrawSliderInt(1, 100, RPR_VariantCure, "HP% to be at or under", 200);

                    break;
            }
        }
    }
}
