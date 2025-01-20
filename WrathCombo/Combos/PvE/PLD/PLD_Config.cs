using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    internal static class Config
    {
        private const int numberMitigationOptions = 9;

        internal enum PartyRequirement
        {
            No,
            Yes
        }
        internal enum BossAvoidance
        {
            Off = 1,
            On = 2
        }

        public static UserInt
            PLD_ST_FoF_Trigger = new("PLD_ST_FoF_Trigger", 0),
            PLD_AoE_FoF_Trigger = new("PLD_AoE_FoF_Trigger", 0),
            PLD_ST_SheltronOption = new("PLD_ST_SheltronOption", 50),
            PLD_ST_Sheltron_SubOption = new("PLD_ST_Sheltron_SubOption", 1),
            PLD_ST_Rampart_Health = new("PLD_ST_Rampart_Health", 80),
            PLD_ST_Rampart_SubOption = new("PLD_ST_Rampart_SubOption", 1),
            PLD_ST_Sentinel_Health = new("PLD_ST_Sentinel_Health", 60),
            PLD_ST_Sentinel_SubOption = new("PLD_ST_Sentinel_SubOption", 1),
            PLD_ST_HallowedGround_Health = new("PLD_ST_HallowedGround_Health", 30),
            PLD_ST_HallowedGround_SubOption = new("PLD_ST_HallowedGround_SubOption", 1),
            PLD_AoE_SheltronOption = new("PLD_AoE_SheltronOption", 50),
            PLD_AoE_Sheltron_SubOption = new("PLD_AoE_Sheltron_SubOption", 1),
            PLD_AoE_Rampart_Health = new("PLD_AoE_Rampart_Health", 80),
            PLD_AoE_Rampart_SubOption = new("PLD_AoE_Rampart_SubOption", 1),
            PLD_AoE_Sentinel_Health = new("PLD_AoE_Sentinel_Health", 60),
            PLD_AoE_Sentinel_SubOption = new("PLD_AoE_Sentinel_SubOption", 1),
            PLD_AoE_HallowedGround_Health = new("PLD_AoE_HallowedGround_Health", 30),
            PLD_AoE_HallowedGround_SubOption = new("PLD_AoE_HallowedGround_SubOption", 1),
            PLD_Intervene_HoldCharges = new("PLD_Intervene_HoldCharges", 1),
            PLD_AoE_Intervene_HoldCharges = new("PLD_AoE_Intervene_HoldCharges", 1),
            PLD_Intervene_MeleeOnly = new("PLD_Intervene_MeleeOnly", 1),
            PLD_AoE_Intervene_MeleeOnly = new("PLD_AoE_Intervene_MeleeOnly", 1),
            PLD_ST_MP_Reserve = new("PLD_ST_MP_Reserve", 1000),
            PLD_AoE_MP_Reserve = new("PLD_AoE_MP_Reserve", 1000),
            PLD_ShieldLob_SubOption = new("PLD_ShieldLob_SubOption", 1),
            PLD_Requiescat_SubOption = new("PLD_Requiescat_SubOption", 1),
            PLD_SpiritsWithin_SubOption = new("PLD_SpiritsWithin_SubOption", 1),
            PLD_VariantCure = new("PLD_VariantCure"),
            PLD_Balance_Content = new("PLD_Balance_Content", 1),
            PLD_ST_MitsOptions = new("PLD_ST_MitsOptions", 0),
            PLD_AoE_MitsOptions = new("PLD_AoE_MitsOptions", 0),

            //One-Button Mitigation
            PLD_Mit_HallowedGround_Max_Health = new("PLD_Mit_HallowedGround_Max_Health", 20),
            PLD_Mit_DivineVeil_PartyRequirement = new("PLD_Mit_DivineVeil_PartyRequirement", (int) PartyRequirement.Yes),
            PLD_Mit_Rampart_Health = new("PLD_Mit_Rampart_Health", 65),
            PLD_Mit_Sentinel_Health = new("PLD_Mit_Sentinel_Health", 60),
            PLD_Mit_ArmsLength_Boss = new("PLD_Mit_ArmsLength_Boss", (int) BossAvoidance.On),
            PLD_Mit_ArmsLength_EnemyCount = new("PLD_Mit_ArmsLength_EnemyCount", 0),
            PLD_Mit_Bulwark_Health = new("PLD_Mit_Bulwark_Health", 50),
            PLD_Mit_HallowedGround_Health = new("PLD_Mit_HallowedGround_Health", 35),
            PLD_Mit_Clemency_Health = new("PLD_Mit_Clemency_Health", 40);

        public static UserIntArray
            PLD_Mit_Priorities = new("PLD_Mit_Priorities");

        public static UserBoolArray
            PLD_Mit_HallowedGround_Max_Difficulty = new(
                "PLD_Mit_HallowedGround_Max_Difficulty",
                [true, true]),
            PLD_Mit_HallowedGround_Difficulty = new(
                "PLD_Mit_HallowedGround_Difficulty",
                [true, false]);

        public static readonly ContentCheck.ListSet
            PLD_Mit_HallowedGround_Max_DifficultyListSet = ContentCheck.ListSet.Halved,
            PLD_Mit_HallowedGround_DifficultyListSet = ContentCheck.ListSet.Halved;

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.PLD_ST_AdvancedMode_BalanceOpener:
                    UserConfig.DrawBossOnlyChoice(PLD_Balance_Content);
                    break;

                // Fight or Flight
                case CustomComboPreset.PLD_ST_AdvancedMode_FoF:
                    UserConfig.DrawSliderInt(0, 50, PLD_ST_FoF_Trigger, "Target HP%", 200);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_FoF:
                    UserConfig.DrawSliderInt(0, 50, PLD_AoE_FoF_Trigger, "Target HP%", 200);

                    break;

                // Sheltron
                case CustomComboPreset.PLD_ST_AdvancedMode_Sheltron:
                    UserConfig.DrawSliderInt(50, 100, PLD_ST_SheltronOption, "Oath Gauge", 200, 5);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sheltron_SubOption, "All Enemies",
                        "Uses Sheltron regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sheltron_SubOption, "Bosses Only",
                        "Only uses Sheltron when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron:
                    UserConfig.DrawSliderInt(50, 100, PLD_AoE_SheltronOption, "Oath Gauge", 200, 5);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sheltron_SubOption, "All Enemies",
                        "Uses Sheltron regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sheltron_SubOption, "Bosses Only",
                        "Only uses Sheltron when the targeted enemy is a boss.", 2);

                    break;

                // Rampart
                case CustomComboPreset.PLD_ST_AdvancedMode_Rampart:
                    UserConfig.DrawSliderInt(1, 100, PLD_ST_Rampart_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Rampart_SubOption, "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Rampart_SubOption, "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Rampart:
                    UserConfig.DrawSliderInt(1, 100, PLD_AoE_Rampart_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Rampart_SubOption, "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Rampart_SubOption, "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                // Sentinel / Guardian
                case CustomComboPreset.PLD_ST_AdvancedMode_Sentinel:
                    UserConfig.DrawSliderInt(1, 100, PLD_ST_Sentinel_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sentinel_SubOption, "All Enemies",
                        "Uses Sentinel regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sentinel_SubOption, "Bosses Only",
                        "Only uses Sentinel when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Sentinel:
                    UserConfig.DrawSliderInt(1, 100, PLD_AoE_Sentinel_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sentinel_SubOption, "All Enemies",
                        "Uses Sentinel regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sentinel_SubOption, "Bosses Only",
                        "Only uses Sentinel when the targeted enemy is a boss.", 2);

                    break;

                // Hallowed Ground
                case CustomComboPreset.PLD_ST_AdvancedMode_HallowedGround:
                    UserConfig.DrawSliderInt(1, 100, PLD_ST_HallowedGround_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_HallowedGround_SubOption, "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_HallowedGround_SubOption, "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_HallowedGround:
                    UserConfig.DrawSliderInt(1, 100, PLD_AoE_HallowedGround_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_HallowedGround_SubOption, "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_HallowedGround_SubOption, "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                // Intervene
                case CustomComboPreset.PLD_ST_AdvancedMode_Intervene:
                    UserConfig.DrawSliderInt(0, 1, PLD_Intervene_HoldCharges, "Charges", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_Intervene_MeleeOnly, "Melee Range",
                        "Uses Intervene while within melee range.\n- May result in minor movement.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_Intervene_MeleeOnly, "No Movement",
                        "Only uses Intervene when it would not result in movement.\n- Requires target to be within zero distance.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Intervene:
                    UserConfig.DrawSliderInt(0, 1, PLD_AoE_Intervene_HoldCharges, "Charges", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Intervene_MeleeOnly, "Melee Range",
                        "Uses Intervene while within melee range.\n- May result in minor movement.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Intervene_MeleeOnly, "No Movement",
                        "Only uses Intervene when it would not result in movement.\n- Requires target to be within zero distance.", 2);

                    break;

                // Shield Lob
                case CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob:
                    UserConfig.DrawHorizontalRadioButton(PLD_ShieldLob_SubOption, "Shield Lob Only",
                        "", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ShieldLob_SubOption, "Add Holy Spirit",
                        "Attempts to hardcast Holy Spirit when not moving.\n- Requires sufficient MP to cast.", 2);

                    break;

                // MP Reservation
                case CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve:
                    UserConfig.DrawSliderInt(1000, 5000, PLD_ST_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve:
                    UserConfig.DrawSliderInt(1000, 5000, PLD_AoE_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                // Requiescat Spender Feature
                case CustomComboPreset.PLD_Requiescat_Options:
                    UserConfig.DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Normal Behavior",
                        "", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Add Fight or Flight",
                        "Adds Fight or Flight to the normal logic.\n- Requires Resquiescat to be ready.", 2);

                    break;

                // Spirits Within / Circle of Scorn Feature
                case CustomComboPreset.PLD_SpiritsWithin:
                    UserConfig.DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Normal Behavior",
                        "", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Add Drift Prevention",
                        "Prevents Spirits Within and Circle of Scorn from drifting.\n- Actions must be used within 5 seconds of each other.", 2);

                    break;

                // Variant Cure Feature
                case CustomComboPreset.PLD_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, PLD_VariantCure, "Player HP%", 200);

                    break;

                // Simple ST Mitigations Option
                case CustomComboPreset.PLD_ST_SimpleMode:
                    UserConfig.DrawHorizontalRadioButton(PLD_ST_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                // Simple AoE Mitigations Option
                case CustomComboPreset.PLD_AoE_SimpleMode:
                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                #region One-Button Mitigation

                case CustomComboPreset.PLD_Mit_HallowedGround_Max:
                    UserConfig.DrawDifficultyMultiChoice(
                        PLD_Mit_HallowedGround_Max_Difficulty,
                        PLD_Mit_HallowedGround_Max_DifficultyListSet,
                        "Select what difficulties Hallowed Ground should be used in:"
                    );

                    UserConfig.DrawSliderInt(1, 100, PLD_Mit_HallowedGround_Max_Health,
                        "Player HP% to be \nless than or equal to:",
                        200, SliderIncrements.Fives);
                    break;

                case CustomComboPreset.PLD_Mit_Sheltron:
                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 0,
                        "Sheltron Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_Reprisal:
                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 1,
                        "Reprisal Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_DivineVeil:
                    ImGui.Dummy(new Vector2(15f.Scale(), 0f));
                    ImGui.SameLine();
                    UserConfig.DrawHorizontalRadioButton(
                        PLD_Mit_DivineVeil_PartyRequirement,
                        "Require party",
                        "Will not use Divine Veil unless there are 2 or more party members.",
                        outputValue: (int) PartyRequirement.Yes);
                    UserConfig.DrawHorizontalRadioButton(
                        PLD_Mit_DivineVeil_PartyRequirement,
                        "Use Always",
                        "Will not require a party for Divine Veil.",
                        outputValue: (int) PartyRequirement.No);

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 2,
                        "Divine Veil Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_Rampart:
                    UserConfig.DrawSliderInt(1, 100, PLD_Mit_Rampart_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 3,
                        "Rampart Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_Sentinel:
                    UserConfig.DrawSliderInt(1, 100, PLD_Mit_Sentinel_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 4,
                        "Sentinel Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_ArmsLength:
                    ImGui.Dummy(new Vector2(15f.Scale(), 0f));
                    ImGui.SameLine();
                    UserConfig.DrawHorizontalRadioButton(
                        PLD_Mit_ArmsLength_Boss, "All Enemies",
                        "Will use Arm's Length regardless of the type of enemy.",
                        outputValue: (int) BossAvoidance.Off, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        PLD_Mit_ArmsLength_Boss, "Avoid Bosses",
                        "Will try not to use Arm's Length when in a boss fight.",
                        outputValue: (int) BossAvoidance.On, itemWidth: 125f);

                    UserConfig.DrawSliderInt(0, 3, PLD_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 5,
                        "Arm's Length Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_Bulwark:
                    UserConfig.DrawSliderInt(1, 100, PLD_Mit_Bulwark_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 6,
                        "Bulwark Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_HallowedGround:
                    UserConfig.DrawDifficultyMultiChoice(
                        PLD_Mit_HallowedGround_Difficulty,
                        PLD_Mit_HallowedGround_DifficultyListSet,
                        "Select what difficulties Hallowed Ground should be used in:"
                    );

                    UserConfig.DrawSliderInt(1, 100, PLD_Mit_HallowedGround_Health,
                        "HP% to use at or below",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 7,
                        "Hallowed Ground Priority:");
                    break;

                case CustomComboPreset.PLD_Mit_Clemency:
                    UserConfig.DrawSliderInt(1, 100, PLD_Mit_Clemency_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 8,
                        "Clemency Priority:");
                    break;

                    #endregion
            }
        }
    }
}
