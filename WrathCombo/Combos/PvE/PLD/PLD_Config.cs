using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    internal static class Config
    {
        private const int numberMitigationOptions = 9;

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
            PLD_RetargetClemency_Health = new("PLD_RetargetClemency_Health", 30),
            PLD_Balance_Content = new("PLD_Balance_Content", 1),
            PLD_ST_MitsOptions = new("PLD_ST_MitsOptions", 0),
            PLD_AoE_MitsOptions = new("PLD_AoE_MitsOptions", 0),
            PLD_RetargetShieldBash_Strength = new("PLD_RetargetShieldBash_Strength", 3),

            //One-Button Mitigation
            PLD_Mit_HallowedGround_Max_Health = new("PLD_Mit_HallowedGround_Max_Health", 20),
            PLD_Mit_DivineVeil_PartyRequirement = new("PLD_Mit_DivineVeil_PartyRequirement", (int)PartyRequirement.Yes),
            PLD_Mit_Rampart_Health = new("PLD_Mit_Rampart_Health", 65),
            PLD_Mit_Sentinel_Health = new("PLD_Mit_Sentinel_Health", 60),
            PLD_Mit_ArmsLength_Boss = new("PLD_Mit_ArmsLength_Boss", (int)BossAvoidance.On),
            PLD_Mit_ArmsLength_EnemyCount = new("PLD_Mit_ArmsLength_EnemyCount", 0),
            PLD_Mit_Bulwark_Health = new("PLD_Mit_Bulwark_Health", 50),
            PLD_Mit_HallowedGround_Health = new("PLD_Mit_HallowedGround_Health", 35),
            PLD_Mit_Clemency_Health = new("PLD_Mit_Clemency_Health", 40);


        public static UserBool
            PLD_RetargetStunLockout = new("PLD_RetargetStunLockout");

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

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.PLD_ST_AdvancedMode_BalanceOpener:
                    DrawBossOnlyChoice(PLD_Balance_Content);
                    break;

                // Fight or Flight
                case Preset.PLD_ST_AdvancedMode_FoF:
                    DrawSliderInt(0, 50, PLD_ST_FoF_Trigger, "Target HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_FoF:
                    DrawSliderInt(0, 50, PLD_AoE_FoF_Trigger, "Target HP%", 200);

                    break;

                // Sheltron
                case Preset.PLD_ST_AdvancedMode_Sheltron:
                    DrawSliderInt(50, 100, PLD_ST_SheltronOption, "Oath Gauge", 200, 5);

                    DrawHorizontalRadioButton(PLD_ST_Sheltron_SubOption, "All Enemies",
                        "Uses Sheltron regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_ST_Sheltron_SubOption, "Bosses Only",
                        "Only uses Sheltron when the targeted enemy is a boss.", 2);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Sheltron:
                    DrawSliderInt(50, 100, PLD_AoE_SheltronOption, "Oath Gauge", 200, 5);

                    DrawHorizontalRadioButton(PLD_AoE_Sheltron_SubOption, "All Enemies",
                        "Uses Sheltron regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_AoE_Sheltron_SubOption, "Bosses Only",
                        "Only uses Sheltron when the targeted enemy is a boss.", 2);

                    break;

                // Rampart
                case Preset.PLD_ST_AdvancedMode_Rampart:
                    DrawSliderInt(1, 100, PLD_ST_Rampart_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_ST_Rampart_SubOption, "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_ST_Rampart_SubOption, "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Rampart:
                    DrawSliderInt(1, 100, PLD_AoE_Rampart_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_AoE_Rampart_SubOption, "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_AoE_Rampart_SubOption, "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                // Sentinel / Guardian
                case Preset.PLD_ST_AdvancedMode_Sentinel:
                    DrawSliderInt(1, 100, PLD_ST_Sentinel_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_ST_Sentinel_SubOption, "All Enemies",
                        "Uses Sentinel regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_ST_Sentinel_SubOption, "Bosses Only",
                        "Only uses Sentinel when the targeted enemy is a boss.", 2);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Sentinel:
                    DrawSliderInt(1, 100, PLD_AoE_Sentinel_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_AoE_Sentinel_SubOption, "All Enemies",
                        "Uses Sentinel regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_AoE_Sentinel_SubOption, "Bosses Only",
                        "Only uses Sentinel when the targeted enemy is a boss.", 2);

                    break;

                // Hallowed Ground
                case Preset.PLD_ST_AdvancedMode_HallowedGround:
                    DrawSliderInt(1, 100, PLD_ST_HallowedGround_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_ST_HallowedGround_SubOption, "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_ST_HallowedGround_SubOption, "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                case Preset.PLD_AoE_AdvancedMode_HallowedGround:
                    DrawSliderInt(1, 100, PLD_AoE_HallowedGround_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_AoE_HallowedGround_SubOption, "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    DrawHorizontalRadioButton(PLD_AoE_HallowedGround_SubOption, "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                // Intervene
                case Preset.PLD_ST_AdvancedMode_Intervene:
                    DrawSliderInt(0, 1, PLD_Intervene_HoldCharges, "Charges", 200);

                    DrawHorizontalRadioButton(PLD_Intervene_MeleeOnly, "Melee Range",
                        "Uses Intervene while within melee range.\n- May result in minor movement.", 1);

                    DrawHorizontalRadioButton(PLD_Intervene_MeleeOnly, "No Movement",
                        "Only uses Intervene when it would not result in movement.\n- Requires target to be within zero distance.", 2);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Intervene:
                    DrawSliderInt(0, 1, PLD_AoE_Intervene_HoldCharges, "Charges", 200);

                    DrawHorizontalRadioButton(PLD_AoE_Intervene_MeleeOnly, "Melee Range",
                        "Uses Intervene while within melee range.\n- May result in minor movement.", 1);

                    DrawHorizontalRadioButton(PLD_AoE_Intervene_MeleeOnly, "No Movement",
                        "Only uses Intervene when it would not result in movement.\n- Requires target to be within zero distance.", 2);

                    break;

                // Shield Lob
                case Preset.PLD_ST_AdvancedMode_ShieldLob:
                    DrawHorizontalRadioButton(PLD_ShieldLob_SubOption, "Shield Lob Only",
                        "", 1);

                    DrawHorizontalRadioButton(PLD_ShieldLob_SubOption, "Add Holy Spirit",
                        "Attempts to hardcast Holy Spirit when not moving.\n- Requires sufficient MP to cast.", 2);

                    break;

                // MP Reservation
                case Preset.PLD_ST_AdvancedMode_MP_Reserve:
                    DrawSliderInt(1000, 5000, PLD_ST_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                case Preset.PLD_AoE_AdvancedMode_MP_Reserve:
                    DrawSliderInt(1000, 5000, PLD_AoE_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                // Requiescat Spender Feature
                case Preset.PLD_Requiescat_Options:
                    DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Normal Behavior",
                        "", 1);

                    DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Add Fight or Flight",
                        "Adds Fight or Flight to the normal logic.\n- Requires Resquiescat to be ready.", 2);

                    break;

                // Spirits Within / Circle of Scorn Feature
                case Preset.PLD_SpiritsWithin:
                    DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Normal Behavior",
                        "", 1);

                    DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Add Drift Prevention",
                        "Prevents Spirits Within and Circle of Scorn from drifting.\n- Actions must be used within 5 seconds of each other.", 2);

                    break;

                // Retarget Clemency Feature
                case Preset.PLD_RetargetClemency_LowHP:
                    DrawSliderInt(1, 100, PLD_RetargetClemency_Health, "Player HP%", 200);

                    break;

                // Simple ST Mitigations Option
                case Preset.PLD_ST_SimpleMode:
                    DrawHorizontalRadioButton(PLD_ST_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    DrawHorizontalRadioButton(PLD_ST_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                // Simple AoE Mitigations Option
                case Preset.PLD_AoE_SimpleMode:
                    DrawHorizontalRadioButton(PLD_AoE_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    DrawHorizontalRadioButton(PLD_AoE_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                case Preset.PLD_RetargetSheltron_TT:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey,
                        "Note: If you are Off-Tanking, and want to use Sheltron on yourself, the expectation would be that you do so via the One-Button Mitigation Feature or the Mitigation options in your rotation.\n" +
                        "You could also mouseover yourself in the party to use Sheltron in this case.\n" +
                        "If you don't, intervention would replace the combo, and it would go to the main tank.\n" +
                        "If you don't use those Features for your personal mitigation, you may not want to enable this.");
                    ImGui.Unindent();
                    break;
                case Preset.PLD_RetargetShieldBash:
                    DrawAdditionalBoolChoice(PLD_RetargetStunLockout, "Lockout Action", "If no stunnable targets are found, lock the action with Savage Blade");
                    if (PLD_RetargetStunLockout)
                        DrawSliderInt(1, 3, PLD_RetargetShieldBash_Strength, "Lockout when stun has been applied this many times");
                    break;

                #region One-Button Mitigation

                case Preset.PLD_Mit_HallowedGround_Max:
                    DrawDifficultyMultiChoice(
                        PLD_Mit_HallowedGround_Max_Difficulty,
                        PLD_Mit_HallowedGround_Max_DifficultyListSet,
                        "Select what difficulties Hallowed Ground should be used in:"
                    );

                    DrawSliderInt(1, 100, PLD_Mit_HallowedGround_Max_Health,
                        "Player HP% to be \nless than or equal to:",
                        200, SliderIncrements.Fives);
                    break;

                case Preset.PLD_Mit_Sheltron:
                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 0,
                        "Sheltron Priority:");
                    break;

                case Preset.PLD_Mit_Reprisal:
                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 1,
                        "Reprisal Priority:");
                    break;

                case Preset.PLD_Mit_DivineVeil:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(
                        PLD_Mit_DivineVeil_PartyRequirement,
                        "Require party",
                        "Will not use Divine Veil unless there are 2 or more party members.",
                        outputValue: (int)PartyRequirement.Yes);
                    DrawHorizontalRadioButton(
                        PLD_Mit_DivineVeil_PartyRequirement,
                        "Use Always",
                        "Will not require a party for Divine Veil.",
                        outputValue: (int)PartyRequirement.No);
                    ImGui.Unindent();

                    ImGui.NewLine();
                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 2,
                        "Divine Veil Priority:");
                    break;

                case Preset.PLD_Mit_Rampart:
                    DrawSliderInt(1, 100, PLD_Mit_Rampart_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 3,
                        "Rampart Priority:");
                    break;

                case Preset.PLD_Mit_Sentinel:
                    DrawSliderInt(1, 100, PLD_Mit_Sentinel_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 4,
                        "Sentinel Priority:");
                    break;

                case Preset.PLD_Mit_ArmsLength:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(
                        PLD_Mit_ArmsLength_Boss, "All Enemies",
                        "Will use Arm's Length regardless of the type of enemy.",
                        outputValue: (int)BossAvoidance.Off, itemWidth: 125f);
                    DrawHorizontalRadioButton(
                        PLD_Mit_ArmsLength_Boss, "Avoid Bosses",
                        "Will try not to use Arm's Length when in a boss fight.",
                        outputValue: (int)BossAvoidance.On, itemWidth: 125f);
                    ImGui.Unindent();

                    ImGui.NewLine();
                    DrawSliderInt(0, 3, PLD_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");

                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 5,
                        "Arm's Length Priority:");
                    break;

                case Preset.PLD_Mit_Bulwark:
                    DrawSliderInt(1, 100, PLD_Mit_Bulwark_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 6,
                        "Bulwark Priority:");
                    break;

                case Preset.PLD_Mit_HallowedGround:
                    DrawDifficultyMultiChoice(
                        PLD_Mit_HallowedGround_Difficulty,
                        PLD_Mit_HallowedGround_DifficultyListSet,
                        "Select what difficulties Hallowed Ground should be used in:"
                    );

                    DrawSliderInt(1, 100, PLD_Mit_HallowedGround_Health,
                        "HP% to use at or below",
                        sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 7,
                        "Hallowed Ground Priority:");
                    break;

                case Preset.PLD_Mit_Clemency:
                    DrawSliderInt(1, 100, PLD_Mit_Clemency_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(PLD_Mit_Priorities,
                        numberMitigationOptions, 8,
                        "Clemency Priority:");
                    break;

                    #endregion
            }
        }
    }
}
