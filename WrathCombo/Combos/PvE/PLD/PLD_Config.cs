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
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.PLD_ST_AdvancedMode_BalanceOpener:
                    DrawBossOnlyChoice(PLD_Balance_Content);
                    break;

                // Fight or Flight
                case Preset.PLD_ST_AdvancedMode_FoF:
                    DrawSliderInt(0, 50, PLD_ST_FoF_HPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(PLD_ST_FoF_BossOption,
                        "Non-Bosses", "Only apply the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(PLD_ST_FoF_BossOption,
                        "All Enemies", "Apply the HP check above to all enemies.", 1);
                    ImGui.Unindent();
                    break;

                case Preset.PLD_ST_AdvancedMode_Mitigation:
                    DrawDifficultyMultiChoice(PLD_ST_Mit_Difficulty, PLD_ST_Mit_DifficultyListSet,
                        "Select what difficulties mitigations should be used in:");
                    break;

                // Sheltron
                case Preset.PLD_ST_AdvancedMode_Sheltron:
                    DrawSliderInt(50, 100, PLD_ST_SheltronOption, "Oath Gauge", 200, 5);

                    DrawSliderInt(1, 100, PLD_ST_Sheltron_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_ST_MitSheltronBoss,
                        "All Enemies", "Will use Sheltron regardless of the type of enemy.", (int)BossAvoidance.Off, 125f);

                    DrawHorizontalRadioButton(PLD_ST_MitSheltronBoss,
                        "Avoid Bosses", "Will try not to use Sheltron when in a boss fight.", (int)BossAvoidance.On, 125f);

                    break;

                // Sentinel / Guardian
                case Preset.PLD_ST_AdvancedMode_Sentinel:
                    DrawSliderInt(1, 100, PLD_ST_Sentinel_Health, "Player HP%", 200);

                    break;

                // Bulwark
                case Preset.PLD_ST_AdvancedMode_Bulwark:
                    DrawSliderInt(1, 100, PLD_ST_Bulwark_Health, "Player HP%", 200);

                    break;

                // Hallowed Ground
                case Preset.PLD_ST_AdvancedMode_HallowedGround:
                    DrawSliderInt(1, 100, PLD_ST_HallowedGround_Health, "Player HP%", 200);

                    DrawHorizontalRadioButton(PLD_ST_MitHallowedGroundBoss,
                        "All Enemies", "Will use Hallowed Ground regardless of the type of enemy.", (int)BossAvoidance.Off, 125f);

                    DrawHorizontalRadioButton(PLD_ST_MitHallowedGroundBoss,
                        "Avoid Bosses", "Will try not to use Hallowed Ground when in a boss fight.", (int)BossAvoidance.On, 125f);

                    break;

                // Intervene
                case Preset.PLD_ST_AdvancedMode_Intervene:
                    DrawHorizontalRadioButton(PLD_ST_Intervene_Movement,
                        "Stationary Only", "Uses Intervene only while stationary", 0);
                    DrawHorizontalRadioButton(PLD_ST_Intervene_Movement,
                        "Any Movement", "Uses Intervene regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (PLD_ST_Intervene_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, PLD_ST_InterveneTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    ImGui.SetCursorPosX(48);
                    DrawSliderInt(0, 2, PLD_ST_Intervene_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, PLD_ST_Intervene_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;

                // Shield Lob
                case Preset.PLD_ST_AdvancedMode_ShieldLob:
                    DrawHorizontalRadioButton(PLD_ST_ShieldLob_SubOption, "Shield Lob Only",
                        "", 0);

                    DrawHorizontalRadioButton(PLD_ST_ShieldLob_SubOption, "Add Holy Spirit",
                        "Attempts to hardcast Holy Spirit when not moving.\n- Requires sufficient MP to cast.", 1);

                    break;

                // MP Reservation
                case Preset.PLD_ST_AdvancedMode_MP_Reserve:
                    DrawSliderInt(1000, 5000, PLD_ST_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                #endregion

                #region AoE

                case Preset.PLD_AoE_AdvancedMode_Sheltron:

                    DrawSliderInt(1, 100, PLD_AoE_Sheltron_Health, "Player HP%", 200);

                    DrawSliderInt(50, 100, PLD_AoE_SheltronOption, "Oath Gauge", 200, 5);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Reprisal:

                    DrawSliderInt(1, 100, PLD_AoE_Reprisal_Health, "Player HP%", 200);

                    DrawSliderInt(1, 5, PLD_AoE_Reprisal_Count, "# enemies in range", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_DivineVeil:
                    DrawSliderInt(1, 100, PLD_AoE_DivineVeil_Health, "Player HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Rampart:
                    DrawSliderInt(1, 100, PLD_AoE_Rampart_Health, "Player HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_ArmsLength:
                    DrawSliderInt(1, 5, PLD_AoE_ArmsLength_Count, "# enemies in range", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Sentinel:
                    DrawSliderInt(1, 100, PLD_AoE_Sentinel_Health, "Player HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Bulwark:
                    DrawSliderInt(1, 100, PLD_AoE_Bulwark_Health, "Player HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_FoF:
                    DrawSliderInt(0, 50, PLD_AoE_FoF_Trigger, "Target HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_HallowedGround:
                    DrawSliderInt(1, 100, PLD_AoE_HallowedGround_Health, "Player HP%", 200);

                    break;

                case Preset.PLD_AoE_AdvancedMode_Intervene:
                    DrawHorizontalRadioButton(PLD_AoE_Intervene_Movement,
                        "Stationary Only", "Uses Intervene only while stationary", 0);
                    DrawHorizontalRadioButton(PLD_AoE_Intervene_Movement,
                        "Any Movement", "Uses Intervene regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (PLD_AoE_Intervene_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, PLD_AoE_InterveneTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    ImGui.SetCursorPosX(48);
                    DrawSliderInt(0, 2, PLD_AoE_Intervene_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, PLD_AoE_Intervene_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;

                case Preset.PLD_AoE_AdvancedMode_MP_Reserve:
                    DrawSliderInt(1000, 5000, PLD_AoE_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                #endregion

                #region Simple Modes

                // Simple ST Mitigations Option
                case Preset.PLD_ST_SimpleMode:
                    DrawHorizontalRadioButton(PLD_ST_MitOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    DrawHorizontalRadioButton(PLD_ST_MitOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                // Simple AoE Mitigations Option
                case Preset.PLD_AoE_SimpleMode:
                    DrawHorizontalRadioButton(PLD_AoE_MitOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    DrawHorizontalRadioButton(PLD_AoE_MitOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                #endregion

                #region Standalones

                // Requiescat Spender Feature
                case Preset.PLD_Requiescat_Options:
                    DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Normal Behavior",
                        "", 0);

                    DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Add Fight or Flight",
                        "Adds Fight or Flight to the normal logic.\n- Requires Resquiescat to be ready.", 1);

                    break;

                // Spirits Within / Circle of Scorn Feature
                case Preset.PLD_SpiritsWithin:
                    DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Normal Behavior",
                        "", 0);

                    DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Add Drift Prevention",
                        "Prevents Spirits Within and Circle of Scorn from drifting.\n- Actions must be used within 5 seconds of each other.", 1);

                    break;

                // Retarget Clemency Feature
                case Preset.PLD_RetargetClemency_LowHP:
                    DrawSliderInt(1, 100, PLD_RetargetClemency_Health, "Player HP%", 200);
                    break;

                // Retarget Cover Feature
                case Preset.PLD_RetargetCover_LowHP:
                    DrawSliderInt(1, 100, PLD_RetargetCover_Health, "Ally HP%", 200);
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

                #endregion

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
                        NumberMitigationOptions, 0,
                        "Sheltron Priority:");
                    break;

                case Preset.PLD_Mit_Reprisal:
                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 1,
                        "Reprisal Priority:");
                    break;

                case Preset.PLD_Mit_DivineVeil:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(
                        PLD_Mit_DivineVeil_PartyRequirement,
                        "Require party",
                        "Will not use Divine Veil unless there are 2 or more party members.",
                        (int)PartyRequirement.Yes);

                    DrawHorizontalRadioButton(
                        PLD_Mit_DivineVeil_PartyRequirement,
                        "Use Always",
                        "Will not require a party for Divine Veil.",
                        (int)PartyRequirement.No);
                    ImGui.Unindent();

                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 2,
                        "Divine Veil Priority:");
                    break;

                case Preset.PLD_Mit_Rampart:
                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 3,
                        "Rampart Priority:");
                    break;

                case Preset.PLD_Mit_Bulwark:
                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 4,
                        "Bulwark Priority:");
                    break;

                case Preset.PLD_Mit_ArmsLength:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(PLD_Mit_ArmsLength_Boss,
                        "All Enemies", "Will use Arm's Length regardless of the type of enemy.", (int)BossAvoidance.Off, 125f);

                    DrawHorizontalRadioButton(PLD_Mit_ArmsLength_Boss,
                        "Avoid Bosses", "Will try not to use Arm's Length when in a boss fight.", (int)BossAvoidance.On, 125f);
                    ImGui.Unindent();

                    DrawSliderInt(0, 5, PLD_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");

                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 5,
                        "Arm's Length Priority:");
                    break;

                case Preset.PLD_Mit_Sentinel:
                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 6,
                        "Sentinel Priority:");
                    break;

                case Preset.PLD_Mit_Clemency:
                    DrawSliderInt(1, 100, PLD_Mit_Clemency_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(PLD_Mit_Priorities,
                        NumberMitigationOptions, 7,
                        "Clemency Priority:");
                    break;

                    #endregion
            }
        }

        #region Variables

        private const int NumberMitigationOptions = 8;

        public static UserInt
            //ST
            PLD_Balance_Content = new("PLD_Balance_Content", 1),
            PLD_ST_Intervene_Charges = new("PLD_ST_Intervene_Charges"),
            PLD_ST_Intervene_Movement = new("PLD_ST_Intervene_Movement"),
            PLD_ST_Intervene_Distance = new("PLD_ST_Intervene_Distance", 3),
            PLD_ST_MP_Reserve = new("PLD_ST_MP_Reserve", 1000),
            PLD_ST_MitOptions = new("PLD_ST_MitOptions"),
            PLD_ST_SheltronOption = new("PLD_ST_SheltronOption", 50),
            PLD_ST_Sheltron_Health = new("PLD_ST_Sheltron_Health", 85),
            PLD_ST_Sentinel_Health = new("PLD_ST_Sentinel_Health", 50),
            PLD_ST_Bulwark_Health = new("PLD_ST_Bulwark_Health", 60),
            PLD_ST_HallowedGround_Health = new("PLD_ST_HallowedGround_Health", 30),
            PLD_ST_FoF_BossOption = new("PLD_ST_FoF_BossOption"),
            PLD_ST_FoF_HPOption = new("PLD_ST_FoF_HPOption", 10),
            PLD_ST_ShieldLob_SubOption = new("PLD_ST_ShieldLob_SubOption"),
            PLD_ST_MitHallowedGroundBoss = new("PLD_ST_MitHallowedGroundBoss", (int)BossAvoidance.On),
            PLD_ST_MitSheltronBoss = new("PLD_ST_MitSheltronBoss", (int)BossAvoidance.Off),

            //AoE
            PLD_AoE_FoF_Trigger = new("PLD_AoE_FoF_Trigger", 25),
            PLD_AoE_MitOptions = new("PLD_AoE_MitOptions"),
            PLD_AoE_SheltronOption = new("PLD_AoE_SheltronOption", 50),
            PLD_AoE_Sheltron_Health = new("PLD_AoE_Sheltron_Health", 85),
            PLD_AoE_DivineVeil_Health = new("PLD_AoE_DivineVeil_Health", 75),
            PLD_AoE_Rampart_Health = new("PLD_AoE_Rampart_Health", 50),
            PLD_AoE_Reprisal_Health = new("PLD_AoE_Reprisal_Health", 80),
            PLD_AoE_Reprisal_Count = new("PLD_AoE_Reprisal_Count", 3),
            PLD_AoE_ArmsLength_Count = new("PLD_AoE_ArmsLength_Count", 3),
            PLD_AoE_Sentinel_Health = new("PLD_AoE_Sentinel_Health", 50),
            PLD_AoE_Bulwark_Health = new("PLD_AoE_Bulwark_Health", 60),
            PLD_AoE_HallowedGround_Health = new("PLD_AoE_HallowedGround_Health", 30),
            PLD_AoE_Intervene_Charges = new("PLD_AoE_Intervene_Charges"),
            PLD_AoE_Intervene_Movement = new("PLD_AoE_Intervene_Movement"),
            PLD_AoE_Intervene_Distance = new("PLD_AoE_Intervene_Distance", 3),
            PLD_AoE_MP_Reserve = new("PLD_AoE_MP_Reserve", 1000),

            //Standalone
            PLD_Requiescat_SubOption = new("PLD_Requiescat_SubOption"),
            PLD_SpiritsWithin_SubOption = new("PLD_SpiritsWithin_SubOption", 1),

            //Retarget
            PLD_RetargetClemency_Health = new("PLD_RetargetClemency_Health", 30),
            PLD_RetargetShieldBash_Strength = new("PLD_RetargetShieldBash_Strength", 3),
            PLD_RetargetCover_Health = new("PLD_RetargetCover_Health", 30),

            //One-Button Mitigation
            PLD_Mit_HallowedGround_Max_Health = new("PLD_Mit_HallowedGround_Max_Health", 20),
            PLD_Mit_DivineVeil_PartyRequirement = new("PLD_Mit_DivineVeil_PartyRequirement", (int)PartyRequirement.Yes),
            PLD_Mit_ArmsLength_Boss = new("PLD_Mit_ArmsLength_Boss", (int)BossAvoidance.On),
            PLD_Mit_ArmsLength_EnemyCount = new("PLD_Mit_ArmsLength_EnemyCount", 5),
            PLD_Mit_Clemency_Health = new("PLD_Mit_Clemency_Health", 40);

        public static UserFloat
            PLD_ST_InterveneTimeStill = new("PLD_ST_InterveneTimeStill", 2.5f),
            PLD_AoE_InterveneTimeStill = new("PLD_AoE_InterveneTimeStill", 2.5f);

        public static UserBool
            PLD_RetargetStunLockout = new("PLD_RetargetStunLockout");

        public static UserIntArray
            PLD_Mit_Priorities = new("PLD_Mit_Priorities");

        public static UserBoolArray
            PLD_Mit_HallowedGround_Max_Difficulty = new("PLD_Mit_HallowedGround_Max_Difficulty", [true, false]),
            PLD_ST_Mit_Difficulty = new("PLD_ST_Mit_Difficulty", [true, false]);

        public static readonly ContentCheck.ListSet
            PLD_Mit_HallowedGround_Max_DifficultyListSet = ContentCheck.ListSet.Halved,
            PLD_ST_Mit_DifficultyListSet = ContentCheck.ListSet.Halved;

        #endregion
    }
}
