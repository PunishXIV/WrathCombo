using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class BST
{
    public static UserInt
        BST_Advanced_ShieldCharge = new("BST_Advanced_ShieldCharge", 3),
        BST_Advanced_Instinctual_TP = new("BST_Advanced_Instinctual_TP", 100),
        BST_Advanced_Intentional_TP = new("BST_Advanced_Intentional_TP", 100),
        BST_Advanced_RallyStacks = new("BST_Advanced_RallyStacks", 2),
        BST_Advanced_RallyingCheerStacks = new("BST_Advanced_RallyingCheerStacks", 1),
        BST_Instinctual_TpGauge = new("BST_Instinctual_TpGauge", 100),
        BST_Intentional_TpGauge = new("BST_Intentional_TpGauge", 100);
    public static UserBool
        BST_Advanced_Infinitive = new("BST_Advanced_Infinitive", false),
        BST_Intentional_Infinitive = new("BST_Intentional_Infinitive", false),
        BST_SimpleMode_CycleBeasts = new("BST_SimpleMode_CycleBeasts", false),
        BST_Borrow_OnlyCurrentHorn = new("BST_Borrow_OnlyCurrentHorn", false);
    public static UserBoolArray
        BST_Advanced_TemperedRelease = new("BST_Advanced_TemperedRelease", new bool[50]), //Yes, 50 is a lot
        BST_Advanced_BeastModes = new("BST_Advanced_BeastModes", new bool[8]);


    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.BST_SimpleMode:
                    DrawAdditionalBoolChoice(BST_SimpleMode_CycleBeasts, BST_Config.CycleBeasts, BST_Config.CycleBeastsDesc);
                    break;
                case Preset.BST_AdvancedMode_Intentional:
                    DrawAdditionalBoolChoice(BST_Advanced_Infinitive, BST_Config.AdvancedInfinitive, BST_Config.AdvancedInfinitiveDesc);
                    break;
                case Preset.BST_AdvancedMode_BeastMode:
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Beastskin", "", 8, 0);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Vileskin", "", 8, 1);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Cloud Skim", "", 8, 2);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Seedsower", "", 8, 3);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Quelling Wave", "", 8, 4);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Scaleskin", "", 8, 5);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Soul Crush", "", 8, 6);
                    DrawHorizontalMultiChoice(BST_Advanced_BeastModes, "Use Scouring Ash", "", 8, 7);
                    break;
                case Preset.BST_Instinctual_Combo:
                    DrawSliderInt(100, 250, BST_Instinctual_TpGauge, BST_Config.MinTPPlayerBeast, sliderIncrement: 10);
                    break;
                case Preset.BST_Intentional_Combo:
                    DrawSliderInt(100, 250, BST_Intentional_TpGauge, BST_Config.MinTPPlayerBeast, sliderIncrement: 10);
                    DrawAdditionalBoolChoice(BST_Intentional_Infinitive, BST_Config.Infinitive, BST_Config.InfinitiveDesc);
                    break;
                case Preset.BST_Borrow_Feature:
                    DrawAdditionalBoolChoice(BST_Borrow_OnlyCurrentHorn, BST_Config.OnlyCurrentHorn, BST_Config.OnlyCurrentHornDesc);
                    break;
            }
        }
    }
}