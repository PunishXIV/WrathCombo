using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Combo Mitigations
                case Preset.WAR_ST_Simple:
                    DrawHorizontalRadioButton(WAR_ST_MitsOptions, "Include Simple Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(WAR_ST_MitsOptions, "Exclude Simple Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.WAR_AoE_Simple:
                    DrawHorizontalRadioButton(WAR_AoE_MitsOptions, "Include Simple Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(WAR_AoE_MitsOptions, "Exclude Simple Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.WAR_ST_Advanced:
                    DrawHorizontalRadioButton(WAR_ST_Advanced_MitsOptions, "Include Advanced Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(WAR_ST_Advanced_MitsOptions, "Exclude Advanced Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.WAR_AoE_Advanced:
                    DrawHorizontalRadioButton(WAR_AoE_Advanced_MitsOptions, "Include Advanced Mitigations", "Enables the use of advanced mitigations.", 0);
                    DrawHorizontalRadioButton(WAR_AoE_Advanced_MitsOptions, "Exclude Advanced Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.WAR_Mitigation_NonBoss:
                    DrawSliderFloat(0, 100, WAR_Mitigation_NonBoss_MitigationThreshold, "Stop using when average health percentage of nearby enemies is below set. \n(Set to 0 to disable this check) ", decimals: 0);
                    break;
                
                case Preset.WAR_Mitigation_NonBoss_ShakeItOff:
                    DrawSliderInt(1, 100, WAR_Mitigation_NonBoss_ShakeItOff_Health, "Player HP% to use Shake It Off at or below");
                    break;
                
                case Preset.WAR_Mitigation_NonBoss_Equilibrium:
                    DrawSliderInt(1, 100, WAR_Mitigation_NonBoss_Equilibrium_Health, "Player HP% to use Equilibrium at or below");
                    break;
                
                case Preset.WAR_Mitigation_NonBoss_Holmgang:
                    DrawSliderInt(1, 100, WAR_Mitigation_NonBoss_Holmgang_Health, "Player HP% to use Holmgang at or below");
                    break;
                
                case Preset.WAR_Mitigation_Boss_Equilibrium:
                    DrawSliderInt(1, 100, WAR_Mitigation_Boss_Equilibrium_Health, "Player HP% to use Equilibrium at or below.");
                    DrawSliderInt(1, 100, WAR_Mitigation_Boss_Tankbuster_Equilibrium_Health, "Player HP% to use Equilibrium at or below when incoming tankbuster is detected.");
                    break;
                
                case Preset.WAR_Mitigation_Boss_RawIntuition_OnCD:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_RawIntuition_OnCD_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawSliderInt(1, 100, WAR_Mitigation_Boss_RawIntuition_Health, "Player HP% to use Raw Intuition/Bloodwhetting at or below");
                    break;
                
                case Preset.WAR_Mitigation_Boss_RawIntuition_TankBuster:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_RawIntuition_TankBuster_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.WAR_Mitigation_Boss_Rampart:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_Rampart_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.WAR_Mitigation_Boss_Vengeance:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_Vengeance_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawAdditionalBoolChoice(WAR_Mitigation_Boss_Vengeance_First, "Use Vengeance First", "Uses Vengeance before Rampart");
                    break;
                
                case Preset.WAR_Mitigation_Boss_ThrillOfBattle:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_ThrillOfBattle_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawSliderFloat(1, 100, WAR_Mitigation_Boss_ThrillOfBattle_Threshold, "Will use Thrill Of Battle as extra tankbuster mitigation if under this HP%", decimals: 0);
                    DrawAdditionalBoolChoice(WAR_Mitigation_Boss_ThrillOfBattle_Align, "Align Thrill Of Battle", "Tries to align Thrill Of Battle with Rampart for tankbusters.");
                    break;
                
                case Preset.WAR_Mitigation_Boss_ShakeItOff:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_ShakeItOff_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                    
                case Preset.WAR_Mitigation_Boss_Reprisal:
                    DrawDifficultyMultiChoice(WAR_Mitigation_Boss_Reprisal_Difficulty, WAR_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                #endregion
                
                #region Single-Target
                case Preset.WAR_ST_BalanceOpener:
                    DrawBossOnlyChoice(WAR_BalanceOpener_Content);
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(WAR_ST_BalanceOpener_GapcloserChoice, "No Gapclosers", "Skips Onslaughts use in opener.", 0);
                    DrawHorizontalRadioButton(WAR_ST_BalanceOpener_GapcloserChoice, "Use Gapclosers", "Uses Onslaughts use in opener.", 1);
                    break;

                case Preset.WAR_ST_StormsEye:
                    DrawSliderInt(0, 30, WAR_SurgingRefreshRange,
                        $" Seconds remaining before refreshing {Buffs.SurgingTempest.StatusName()} buff:");
                    break;

                case Preset.WAR_ST_InnerRelease:
                    DrawSliderInt(0, 75, WAR_ST_InnerRelease_Threshold,
                        "Stop usage if Target HP% is below set value.\n To disable this, set value to 0");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(WAR_ST_InnerRelease_Threshold_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(WAR_ST_InnerRelease_Threshold_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;

                case Preset.WAR_ST_Onslaught:
                    DrawHorizontalRadioButton(WAR_ST_Onslaught_Movement,
                        "Stationary Only", "Uses Onslaught only while stationary", 0);
                    DrawHorizontalRadioButton(WAR_ST_Onslaught_Movement,
                        "Any Movement", "Uses Onslaught regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (WAR_ST_Onslaught_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, WAR_ST_Onslaught_TimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    ImGui.SetCursorPosX(48);
                    DrawSliderInt(0, 2, WAR_ST_Onslaught_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, WAR_ST_Onslaught_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;

                case Preset.WAR_ST_Infuriate:
                    DrawSliderInt(0, 2, WAR_ST_Infuriate_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    DrawSliderInt(0, 50, WAR_ST_Infuriate_Gauge,
                        " Use when Beast Gauge is less than or equal to:");
                    break;

                case Preset.WAR_ST_FellCleave:
                    DrawAdditionalBoolChoice(WAR_ST_FellCleave_Pooling, "Burst Pooling ", "Pools gauge for use in burst windows. Will spend to prevent overcap, and dump after Berserk/Inner Release");
                    if (WAR_ST_FellCleave_Pooling)
                    {
                        DrawAdditionalBoolChoice(WAR_ST_FellCleave_Pooling_BossOnly, "Pool in boss encounter Only", "Will spend at 50 Gauge outside of Boss encounter");
                    }
                    if (!WAR_ST_FellCleave_Pooling)
                    {
                        DrawSliderInt(50, 100, WAR_ST_FellCleave_Gauge,
                            " Minimum Beast Gauge required to spend:");
                    }
                    break;

                case Preset.WAR_ST_PrimalRend:
                    DrawHorizontalRadioButton(WAR_ST_PrimalRend_EarlyLate,
                        "Early", "Uses Primal Rend ASAP", 0);
                    DrawHorizontalRadioButton(WAR_ST_PrimalRend_EarlyLate,
                        "Late", "Uses Primal Rend after consumption of all Inner Release stacks", 1);
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(WAR_ST_PrimalRend_Movement,
                        "Stationary Only", "Uses Primal Rend only while stationary", 0);
                    DrawHorizontalRadioButton(WAR_ST_PrimalRend_Movement,
                        "Any Movement", "Uses Primal Rend regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (WAR_ST_PrimalRend_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, WAR_ST_PrimalRend_TimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, WAR_ST_PrimalRend_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;
                #endregion

                #region AoE
                case Preset.WAR_AoE_Decimate:
                    DrawAdditionalBoolChoice(WAR_AoE_Decimate_Pooling, "Burst Pooling ", "Pools gauge for use in burst windows. Will spend to prevent overcap, and dump after Berserk/Inner Release");
                    if (WAR_AoE_Decimate_Pooling)
                    {
                        DrawAdditionalBoolChoice(WAR_AoE_Decimate_Pooling_BossOnly, "Pool in boss encounter Only", "Will spend at 50 Gauge outside of Boss encounter");
                    }
                    if (!WAR_AoE_Decimate_Pooling)
                    {
                        DrawSliderInt(50, 100, WAR_AoE_Decimate_Gauge,
                            " Minimum Beast Gauge required to spend:");
                    }
                    break;
                
                case Preset.WAR_AoE_InnerRelease:
                    DrawSliderInt(0, 75, WAR_AoE_InnerRelease_Threshold,
                        " Stop usage if Target HP% is below set value.\n To disable this, set value to 0");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(WAR_AoE_InnerRelease_Threshold_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(WAR_AoE_InnerRelease_Threshold_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;

                case Preset.WAR_AoE_Infuriate:
                    DrawSliderInt(0, 2, WAR_AoE_Infuriate_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    DrawSliderInt(0, 50, WAR_AoE_Infuriate_Gauge,
                        "Use when gauge is under or equal to");
                    break;

                case Preset.WAR_AoE_Onslaught:
                    DrawHorizontalRadioButton(WAR_AoE_Onslaught_Movement,
                        "Stationary Only", "Uses Onslaught only while stationary", 0);
                    DrawHorizontalRadioButton(WAR_AoE_Onslaught_Movement,
                        "Any Movement", "Uses Onslaught regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (WAR_AoE_Onslaught_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, WAR_AoE_Onslaught_TimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    DrawSliderInt(0, 2, WAR_AoE_Onslaught_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, WAR_AoE_Onslaught_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;

                case Preset.WAR_AoE_PrimalRend:
                    DrawHorizontalRadioButton(WAR_AoE_PrimalRend_EarlyLate,
                        "Early", "Uses Primal Rend ASAP", 0);
                    DrawHorizontalRadioButton(WAR_AoE_PrimalRend_EarlyLate,
                        "Late", "Uses Primal Rend after consumption of all Inner Release stacks", 1);
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(WAR_AoE_PrimalRend_Movement,
                        "Stationary Only", "Uses Primal Rend only while stationary", 0);
                    DrawHorizontalRadioButton(WAR_AoE_PrimalRend_Movement,
                        "Any Movement", "Uses Primal Rend regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (WAR_AoE_PrimalRend_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, WAR_AoE_PrimalRend_TimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, WAR_AoE_PrimalRend_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;
                #endregion

                #region One-Button Mitigation

                case Preset.WAR_Mit_Holmgang_Max:
                    DrawDifficultyMultiChoice(WAR_Mit_Holmgang_Max_Difficulty, WAR_Mit_Holmgang_Max_DifficultyListSet,
                        "Select what difficulties Holmgang should be used in:");

                    DrawSliderInt(1, 100, WAR_Mit_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:", 200, SliderIncrements.Fives);
                    break;

                case Preset.WAR_Mit_Bloodwhetting:
                    DrawSliderInt(1, 100, WAR_Mit_Bloodwhetting_Health,
                        "HP% to use at or below", sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 0,
                        "Bloodwhetting Priority:");
                    break;

                case Preset.WAR_Mit_Equilibrium:
                    DrawSliderInt(1, 100, WAR_Mit_Equilibrium_Health,
                        "HP% to use at or below", sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 1,
                        "Equilibrium Priority:");
                    break;

                case Preset.WAR_Mit_Reprisal:
                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 2,
                        "Reprisal Priority:");
                    break;

                case Preset.WAR_Mit_ThrillOfBattle:
                    DrawSliderInt(1, 100, WAR_Mit_ThrillOfBattle_Health,
                        "HP% to use at or below (100 = Disable check)", sliderIncrement: SliderIncrements.Ones);

                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 3,
                        "Thrill Of Battle Priority:");
                    break;

                case Preset.WAR_Mit_Rampart:
                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 4,
                        "Rampart Priority:");
                    break;

                case Preset.WAR_Mit_ShakeItOff:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(WAR_Mit_ShakeItOff_PartyRequirement,
                        "Require party", "Will not use Shake It Off unless there are 2 or more party members.",
                        (int)PartyRequirement.Yes);
                    DrawHorizontalRadioButton(WAR_Mit_ShakeItOff_PartyRequirement,
                        "Use Always", "Will not require a party for Shake It Off.",
                        (int)PartyRequirement.No);
                    ImGui.Unindent();
                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 5,
                        "Shake It Off Priority:");
                    break;

                case Preset.WAR_Mit_ArmsLength:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(WAR_Mit_ArmsLength_Boss,
                        "All Enemies", "Will use Arm's Length regardless of the type of enemy.",
                        (int)BossAvoidance.Off, 125f);
                    DrawHorizontalRadioButton(WAR_Mit_ArmsLength_Boss,
                        "Avoid Bosses", "Will try not to use Arm's Length when in a boss fight.",
                        (int)BossAvoidance.On, 125f);
                    ImGui.Unindent();
                    DrawSliderInt(0, 5, WAR_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");
                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 6, "Arm's Length Priority:");
                    break;

                case Preset.WAR_Mit_Vengeance:
                    DrawPriorityInput(WAR_Mit_Priorities, NumMitigationOptions, 7, "Vengeance Priority:");
                    break;

                #endregion

                #region Other
                case Preset.WAR_FC_InnerRelease:
                    DrawSliderInt(0, 75, WAR_FC_IRStop,
                        " Stop usage if Target HP% is below set value.\n To disable this, set value to 0");
                    break;

                case Preset.WAR_FC_Onslaught:
                    DrawHorizontalRadioButton(WAR_FC_Onslaught_Movement,
                        "Stationary Only", "Uses Onslaught only while stationary", 0);
                    DrawHorizontalRadioButton(WAR_FC_Onslaught_Movement,
                        "Any Movement", "Uses Onslaught regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (WAR_FC_Onslaught_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, WAR_FC_Onslaught_TimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    DrawSliderInt(0, 2, WAR_FC_Onslaught_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, WAR_FC_Onslaught_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;

                case Preset.WAR_FC_Infuriate:
                    DrawSliderInt(0, 2, WAR_FC_Infuriate_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    DrawSliderInt(0, 50, WAR_FC_Infuriate_Gauge,
                        " Use when Beast Gauge is less than or equal to:");
                    break;

                case Preset.WAR_FC_PrimalRend:
                    DrawHorizontalRadioButton(WAR_FC_PrimalRend_EarlyLate,
                        "Early", "Uses Primal Rend ASAP", 0);
                    DrawHorizontalRadioButton(WAR_FC_PrimalRend_EarlyLate,
                        "Late", "Uses Primal Rend after consumption of all Inner Release stacks", 1);
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(WAR_FC_PrimalRend_Movement,
                        "Stationary Only", "Uses Primal Rend only while stationary", 0);
                    DrawHorizontalRadioButton(WAR_FC_PrimalRend_Movement,
                        "Any Movement", "Uses Primal Rend regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);
                    ImGui.Spacing();
                    if (WAR_FC_PrimalRend_Movement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, WAR_FC_PrimalRend_TimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }
                    ImGui.SetCursorPosX(48);
                    DrawSliderFloat(1, 20, WAR_FC_PrimalRend_Distance,
                        " Use when Distance from target is less than or equal to:", decimals: 1);
                    break;

                case Preset.WAR_InfuriateFellCleave:
                    DrawSliderInt(0, 2, WAR_Infuriate_Charges,
                        " How many charges to keep ready?\n (0 = Use All)");
                    DrawSliderInt(0, 50, WAR_Infuriate_Range,
                        " Use when Beast Gauge is\n less than or equal to:");
                    break;

                case Preset.WAR_EyePath:
                    DrawSliderInt(0, 30, WAR_EyePath_Refresh,
                        $" Seconds remaining before refreshing {Buffs.SurgingTempest.StatusName()} buff:");
                    break;

                case Preset.WAR_RawIntuition_Targeting_TT:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey,
                        "Note: If you are Off-Tanking, and want to use Bloodwhetting on yourself, the expectation would be that you do so via the One-Button Mitigation Feature or the Mitigation options in your rotation.\n" +
                        "You could also mouseover yourself in the party to use Bloodwhetting or raw Intuition in this case.\n" +
                        "If you don't, Nascent Flash would replace the combo, and it would go to the main tank.\n" +
                        "If you don't use those Features for your personal mitigation, you may not want to enable this.");
                    ImGui.Unindent();
                    break;
                
                case Preset.WAR_ArmsLengthLockout:
                    DrawSliderInt(0, 5, WAR_ArmsLengthLockout_Time, "Time (In Seconds) remaining on Inner Strength to Lock out Arm's Length until.");
                    break;

                    #endregion
            }
        }
        #region Variables

        private const int NumMitigationOptions = 8;

        public static UserInt
            //Auto Mitigation
            WAR_ST_MitsOptions = new("WAR_ST_MitsOptions"),
            WAR_AoE_MitsOptions = new("WAR_AoE_MitsOptions"),
            WAR_ST_Advanced_MitsOptions = new("WAR_ST_Advanced_MitsOptions"),
            WAR_AoE_Advanced_MitsOptions = new("WAR_AoE_Advanced_MitsOptions"),
            WAR_Mitigation_NonBoss_ShakeItOff_Health = new("WAR_Mitigation_NonBoss_ShakeItOff_Health", 80),
            WAR_Mitigation_NonBoss_Equilibrium_Health = new("WAR_Mitigation_NonBoss_Equilibrium_Health", 50),
            WAR_Mitigation_NonBoss_Holmgang_Health = new("WAR_Mitigation_NonBoss_Holmgang_Health", 20),
            WAR_Mitigation_Boss_RawIntuition_Health = new("WAR_Mitigation_Boss_RawIntuition_Health", 99),
            WAR_Mitigation_Boss_Equilibrium_Health = new("WAR_Mitigation_Boss_Equilibrium_Health", 30),
            WAR_Mitigation_Boss_Tankbuster_Equilibrium_Health = new("WAR_Mitigation_Boss_Tankbuster_Equilibrium_Health", 80),

            //ST Rotation
            WAR_BalanceOpener_Content = new("WAR_BalanceOpener_Content", 1),
            WAR_ST_BalanceOpener_GapcloserChoice = new("WAR_ST_BalanceOpener_GapcloserChoice", 1),
            WAR_ST_InnerRelease_Threshold = new("WAR_ST_InnerRelease_Threshold", 10),
            WAR_ST_InnerRelease_Threshold_SubOption = new("WAR_ST_InnerRelease_Threshold_SubOption"),
            WAR_ST_Infuriate_Charges = new("WAR_ST_Infuriate_Charges"),
            WAR_ST_Infuriate_Gauge = new("WAR_ST_Infuriate_Gauge", 40),
            WAR_ST_Onslaught_Charges = new("WAR_ST_Onslaught_Charges"),
            WAR_ST_Onslaught_Movement = new("WAR_ST_Onslaught_Movement"),
            WAR_ST_FellCleave_Gauge = new("WAR_ST_FellCleave_Gauge", 50),
            WAR_ST_PrimalRend_Movement = new("WAR_ST_PrimalRend_Movement"),
            WAR_ST_PrimalRend_EarlyLate = new("WAR_ST_PrimalRend_EarlyLate"),

            //AoE Rotation
            WAR_AoE_InnerRelease_Threshold = new("WAR_AoE_InnerRelease_Threshold", 10),
            WAR_AoE_InnerRelease_Threshold_SubOption = new("WAR_AoE_InnerRelease_Threshold_SubOption"),
            WAR_AoE_Infuriate_Charges = new("WAR_AoE_Infuriate_Charges"),
            WAR_AoE_Infuriate_Gauge = new("WAR_AoE_Infuriate_Gauge", 40),
            WAR_AoE_Onslaught_Charges = new("WAR_AoE_Onslaught_Charges"),
            WAR_AoE_Onslaught_Movement = new("WAR_AoE_Onslaught_Movement"),
            WAR_AoE_Decimate_Gauge = new("WAR_AoE_Decimate_Gauge", 50),
            WAR_AoE_PrimalRend_Movement = new("WAR_AoE_PrimalRend_Movement"),
            WAR_AoE_PrimalRend_EarlyLate = new("WAR_AoE_PrimalRend_EarlyLate"),

            //Standalone Configs
            WAR_Infuriate_Charges = new("WAR_Infuriate_Charges"),
            WAR_Infuriate_Range = new("WAR_Infuriate_Range"),
            WAR_SurgingRefreshRange = new("WAR_SurgingRefreshRange", 10),
            WAR_EyePath_Refresh = new("WAR_EyePath", 10),
            WAR_FC_IRStop = new("WAR_FC_IRStop"),
            WAR_FC_Infuriate_Charges = new("WAR_FC_Infuriate_Charges"),
            WAR_FC_Infuriate_Gauge = new("WAR_FC_Infuriate_Gauge", 40),
            WAR_FC_Onslaught_Charges = new("WAR_FC_Onslaught_Charges"),
            WAR_FC_Onslaught_Movement = new("WAR_FC_Onslaught_Movement"),
            WAR_FC_PrimalRend_Movement = new("WAR_FC_PrimalRend_Movement"),
            WAR_FC_PrimalRend_EarlyLate = new("WAR_FC_PrimalRend_EarlyLate"),
            WAR_ArmsLengthLockout_Time = new("WAR_ArmsLengthLockout_Time", 3),

            //One Button Mitigation
            WAR_Mit_Holmgang_Health = new("WAR_Mit_Holmgang_Health", 20),
            WAR_Mit_Bloodwhetting_Health = new("WAR_Mit_Bloodwhetting_Health", 70),
            WAR_Mit_Equilibrium_Health = new("WAR_Mit_Equilibrium_Health", 45),
            WAR_Mit_ThrillOfBattle_Health = new("WAR_Mit_ThrillOfBattle_Health", 60),
            WAR_Mit_ShakeItOff_PartyRequirement = new("WAR_Mit_ShakeItOff_PartyRequirement", (int)PartyRequirement.Yes),
            WAR_Mit_ArmsLength_Boss = new("WAR_Mit_ArmsLength_Boss", (int)BossAvoidance.On),
            WAR_Mit_ArmsLength_EnemyCount = new("WAR_Mit_ArmsLength_EnemyCount");
            

        public static UserFloat
            WAR_Mitigation_NonBoss_MitigationThreshold = new("WAR_Mitigation_NonBoss_MitigationThreshold", 20f),
            WAR_Mitigation_Boss_ThrillOfBattle_Threshold = new("WAR_Mitigation_Boss_ThrillOfBattle_Threshold", 20f),
            
            WAR_ST_Onslaught_Distance = new("WAR_ST_Ons_Distance", 3.0f),
            WAR_ST_PrimalRend_Distance = new("WAR_ST_PR_Distance", 3.0f),
            WAR_ST_Onslaught_TimeStill = new("WAR_ST_Onslaught_TimeStill"),
            WAR_ST_PrimalRend_TimeStill = new("WAR_ST_PrimalRend_TimeStill"),
            
            WAR_AoE_Onslaught_Distance = new("WAR_AoE_Ons_Distance", 3.0f),
            WAR_AoE_PrimalRend_Distance = new("WAR_AoE_PR_Distance", 3.0f),
            WAR_AoE_Onslaught_TimeStill = new("WAR_AoE_Onslaught_TimeStill"),
            WAR_AoE_PrimalRend_TimeStill = new("WAR_AoE_PrimalRend_TimeStill"),
            
            WAR_FC_Onslaught_Distance = new("WAR_FC_Ons_Distance", 3.0f),
            WAR_FC_PrimalRend_Distance = new("WAR_FC_PR_Distance", 3.0f),
            WAR_FC_Onslaught_TimeStill = new("WAR_FC_Onslaught_TimeStill"),
            WAR_FC_PrimalRend_TimeStill = new("WAR_FC_PrimalRend_TimeStill");

        public static UserBool
            WAR_Mitigation_Boss_ThrillOfBattle_Align = new("WAR_Mitigation_Boss_ThrillOfBattle_Align", true),
            WAR_Mitigation_Boss_Vengeance_First = new("WAR_Mitigation_Boss_Vengeance_First", true),

            WAR_ST_FellCleave_Pooling = new("WAR_ST_FellCleave_Pooling"),
            WAR_ST_FellCleave_Pooling_BossOnly = new("WAR_ST_FellCleave_Pooling_BossOnly"),

            WAR_AoE_Decimate_Pooling = new("WAR_AoE_Decimate_Pooling"),
            WAR_AoE_Decimate_Pooling_BossOnly = new("WAR_AoE_Decimate_Pooling_BossOnly");
        public static UserIntArray
            WAR_Mit_Priorities = new("WAR_Mit_Priorities");

       public static UserBoolArray
            WAR_Mitigation_Boss_RawIntuition_OnCD_Difficulty = new("WAR_Mitigation_Boss_RawIntuition_Difficulty", [true, false]),
            WAR_Mitigation_Boss_RawIntuition_TankBuster_Difficulty = new("WAR_Mitigation_Boss_RawIntuition_TankBuster_Difficulty", [true, false]),
            WAR_Mitigation_Boss_Rampart_Difficulty = new("WAR_Mitigation_Boss_Rampart_Difficulty", [true, false]),
            WAR_Mitigation_Boss_Vengeance_Difficulty = new("WAR_Mitigation_Boss_Vengeance_Difficulty", [true, false]),
            WAR_Mitigation_Boss_ThrillOfBattle_Difficulty = new("WAR_Mitigation_Boss_ThrillOfBattle_Difficulty", [true, false]),
            WAR_Mitigation_Boss_ShakeItOff_Difficulty = new("WAR_Mitigation_Boss_ShakeItOff_Difficulty", [true, false]),
            WAR_Mitigation_Boss_Reprisal_Difficulty = new("WAR_Mitigation_Boss_Reprisal_Difficulty", [true, false]),
            WAR_Mit_Holmgang_Max_Difficulty = new("WAR_Mit_Holmgang_Max_Difficulty", [true, false]);

        public static readonly ContentCheck.ListSet
            WAR_Mit_Holmgang_Max_DifficultyListSet = ContentCheck.ListSet.CasualVSHard,
            WAR_Boss_Mit_DifficultyListSet = ContentCheck.ListSet.CasualVSHard;
        #endregion
    }
}
