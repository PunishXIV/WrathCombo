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
                #region Combo Mitigations
                case Preset.PLD_ST_SimpleMode:
                    DrawHorizontalRadioButton(PLD_ST_MitOptions, "Include Simple Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(PLD_ST_MitOptions, "Exclude Simple Mitigations", "Disables the use of mitigations.", 1);
                    break;
               
                case Preset.PLD_AoE_SimpleMode:
                    DrawHorizontalRadioButton(PLD_AoE_MitOptions, "Include Simple Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(PLD_AoE_MitOptions, "Exclude Simple Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.PLD_ST_AdvancedMode:
                    DrawHorizontalRadioButton(PLD_ST_Advanced_MitOptions, "Include Advanced Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(PLD_ST_Advanced_MitOptions, "Exclude Advanced Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.PLD_AoE_AdvancedMode:
                    DrawHorizontalRadioButton(PLD_AoE_Advanced_MitOptions, "Include Advanced Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(PLD_AoE_Advanced_MitOptions, "Exclude Advanced Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.PLD_Mitigation_NonBoss:
                    DrawSliderFloat(0, 100, PLD_Mitigation_NonBoss_MitigationThreshold, "Stop using when average health percentage of nearby enemies is below set. \n(Set to 0 to disable this check) ", decimals: 0);
                    break;
                case Preset.PLD_Mitigation_NonBoss_DivineVeil:
                    DrawSliderInt(1,100, PLD_Mitigation_NonBoss_DivineVeil_Health, "Player HP% to use Divine Veil at or below (100 = Disable check)");
                    break;
                case Preset.PLD_Mitigation_Boss_SheltronOvercap:
                    DrawSliderInt(50, 100, PLD_Mitigation_Boss_SheltronOvercap_Threshold, "Oath Gauge required to Use Sheltron and prevent Overcap.");
                    break;
                case Preset.PLD_Mitigation_Boss_SheltronTankbuster:
                    DrawDifficultyMultiChoice(PLD_Mitigation_Boss_SheltronTankbuster_Difficulty, PLD_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.PLD_Mitigation_Boss_DivineVeil:
                    DrawDifficultyMultiChoice(PLD_Mitigation_Boss_DivineVeil_Difficulty, PLD_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                    
                case Preset.PLD_Mitigation_Boss_Reprisal:
                    DrawDifficultyMultiChoice(PLD_Mitigation_Boss_Reprisal_Difficulty, PLD_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.PLD_Mitigation_Boss_Rampart:
                    DrawDifficultyMultiChoice(PLD_Mitigation_Boss_Rampart_Difficulty, PLD_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.PLD_Mitigation_Boss_Sentinel:
                    DrawDifficultyMultiChoice(PLD_Mitigation_Boss_Sentinel_Difficulty, PLD_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawAdditionalBoolChoice(PLD_Mitigation_Boss_Sentinel_First, "Use Sentinel First", "Uses Sentinel before Rampart");
                    break;
                
                case Preset.PLD_Mitigation_Boss_Bulwark:
                    DrawDifficultyMultiChoice(PLD_Mitigation_Boss_Bulwark_Difficulty, PLD_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawSliderFloat(1, 100, PLD_Mitigation_Boss_Bulwark_Threshold, "Will use Bulwark as extra tankbuster mitigation if under this HP%", decimals: 0);
                    DrawAdditionalBoolChoice(PLD_Mitigation_Boss_Bulwark_Align, "Align Bulwark", "Tries to align Bulwark with Rampart for tankbusters.");
                    break;
                
                #endregion
                
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
                

                // Intervene
                case Preset.PLD_ST_AdvancedMode_Intervene:
                    DrawHorizontalRadioButton(PLD_ST_Intervene_Movement,
                        "Stationary Only", "Uses Intervene only while stationary", 0);

                    DrawHorizontalRadioButton(PLD_ST_Intervene_Movement,
                        "Any Movement", "Uses Intervene regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);

                    ImGui.Spacing();
                    if (PLD_ST_Intervene_Movement == 0)
                    {
                        DrawSliderFloat(0, 3, PLD_ST_InterveneTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }

                    DrawSliderInt(0, 2, PLD_ST_Intervene_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");

                    DrawSliderInt(1, 20, PLD_ST_Intervene_Distance,
                        " Use when Distance from target is less than or equal to:");
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

                case Preset.PLD_AoE_AdvancedMode_FoF:
                    DrawSliderInt(0, 50, PLD_AoE_FoF_Trigger, "Target HP%", 200);
                    break;

                case Preset.PLD_AoE_AdvancedMode_Intervene:
                    DrawHorizontalRadioButton(PLD_AoE_Intervene_Movement,
                        "Stationary Only", "Uses Intervene only while stationary", 0);

                    DrawHorizontalRadioButton(PLD_AoE_Intervene_Movement,
                        "Any Movement", "Uses Intervene regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);

                    ImGui.Spacing();
                    if (PLD_AoE_Intervene_Movement == 0)
                    {
                        DrawSliderFloat(0, 3, PLD_AoE_InterveneTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }

                    DrawSliderInt(0, 2, PLD_AoE_Intervene_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");

                    DrawSliderInt(1, 20, PLD_AoE_Intervene_Distance,
                        " Use when Distance from target is less than or equal to:");
                    break;

                case Preset.PLD_AoE_AdvancedMode_MP_Reserve:
                    DrawSliderInt(1000, 5000, PLD_AoE_MP_Reserve, "Minimum MP", sliderIncrement: 100);

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
            //Mitigations
            PLD_ST_MitOptions = new("PLD_ST_MitOptions"),
            PLD_AoE_MitOptions = new("PLD_AoE_MitOptions"),
            PLD_ST_Advanced_MitOptions = new("PLD_ST_Advanced_MitOptions"),
            PLD_AoE_Advanced_MitOptions = new("PLD_AoE_Advanced_MitOptions"),
            PLD_Mitigation_NonBoss_DivineVeil_Health = new("PLD_Mitigation_NonBoss_DivineVeil_Health", 80),
            PLD_Mitigation_Boss_SheltronOvercap_Threshold = new("PLD_Mitigation_Boss_SheltronOvercap_Threshold", 100),
            
            //ST
            PLD_Balance_Content = new("PLD_Balance_Content", 1),
            PLD_ST_Intervene_Charges = new("PLD_ST_Intervene_Charges"),
            PLD_ST_Intervene_Movement = new("PLD_ST_Intervene_Movement"),
            PLD_ST_Intervene_Distance = new("PLD_ST_Intervene_Distance", 3),
            PLD_ST_MP_Reserve = new("PLD_ST_MP_Reserve", 1000),
            PLD_ST_FoF_BossOption = new("PLD_ST_FoF_BossOption"),
            PLD_ST_FoF_HPOption = new("PLD_ST_FoF_HPOption", 10),
            PLD_ST_ShieldLob_SubOption = new("PLD_ST_ShieldLob_SubOption"),
            
            //AoE
            PLD_AoE_FoF_Trigger = new("PLD_AoE_FoF_Trigger", 25),
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
            PLD_Mitigation_NonBoss_MitigationThreshold = new("PLD_Mitigation_NonBoss_MitigationThreshold", 20f),
            PLD_Mitigation_Boss_Bulwark_Threshold = new("PLD_Mitigation_Boss_Bulwark_Threshold", 80f),
            PLD_ST_InterveneTimeStill = new("PLD_ST_InterveneTimeStill", 2.5f),
            PLD_AoE_InterveneTimeStill = new("PLD_AoE_InterveneTimeStill", 2.5f);

        public static UserBool
            PLD_RetargetStunLockout = new("PLD_RetargetStunLockout"),
            PLD_Mitigation_Boss_Bulwark_Align = new("PLD_Mitigation_Boss_Bulwark_Align"),
            PLD_Mitigation_Boss_Sentinel_First = new("PLD_Mitigation_Boss_Sentinel_First");

        public static UserIntArray
            PLD_Mit_Priorities = new("PLD_Mit_Priorities");

        public static UserBoolArray
            PLD_Mitigation_Boss_DivineVeil_Difficulty = new("PLD_Mitigation_Boss_DivineVeil_Difficulty", [true, false]),
            PLD_Mitigation_Boss_Reprisal_Difficulty = new("PLD_Mitigation_Boss_Reprisal_Difficulty", [true, false]),
            PLD_Mitigation_Boss_SheltronTankbuster_Difficulty = new("PLD_Mitigation_Boss_SheltronTankbuster_Difficulty", [true, false]),
            PLD_Mitigation_Boss_Sentinel_Difficulty = new("PLD_Mitigation_Boss_Sentinel_Difficulty", [true, false]),
            PLD_Mitigation_Boss_Rampart_Difficulty = new("PLD_Mitigation_Boss_Rampart_Difficulty", [true, false]),
            PLD_Mitigation_Boss_Bulwark_Difficulty = new("PLD_Mitigation_Boss_Bulwark_Difficulty", [true, false]),
            PLD_Mit_HallowedGround_Max_Difficulty = new("PLD_Mit_HallowedGround_Max_Difficulty", [true, false]);

        public static readonly ContentCheck.ListSet
            PLD_Mit_HallowedGround_Max_DifficultyListSet = ContentCheck.ListSet.CasualVSHard,
            PLD_Boss_Mit_DifficultyListSet = ContentCheck.ListSet.CasualVSHard;

        #endregion
    }
}
