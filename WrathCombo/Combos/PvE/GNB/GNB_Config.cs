using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    internal static class Config
    {
        public static UserInt
            GNB_ST_MitOptions = new("GNB_ST_MitOptions"),
            GNB_AoE_MitOptions = new("GNB_AoE_MitOptions"),
            GNB_ST_Advanced_MitOptions = new("GNB_ST_Advanced_MitOptions"),
            GNB_AoE_Advanced_MitOptions = new("GNB_AoE_Advanced_MitOptions"),
            GNB_Mitigation_Boss_Aurora_Health = new("GNB_Mitigation_Boss_Aurora_Health", 99),
            GNB_Mitigation_Boss_HeartOfStone_Health = new("GNB_Mitigation_Boss_HeartOfStone_Health", 80),
            
            GNB_Opener_StartChoice = new("GNB_Opener_StartChoice", 0),
            GNB_Opener_NM = new("GNB_Opener_NM", 0),
            GNB_ST_NoMercyStop = new("GNB_ST_NoMercyStop", 5),
            GNB_ST_NoMercy_SubOption = new("GNB_ST_NoMercy_SubOption", 1),
            GNB_ST_Overcap_Choice = new("GNB_ST_Overcap_Choice", 0),
            GNB_ST_HoldLightningShot = new("GNB_ST_HoldLightningShot", 0),
            GNB_ST_BurstStrike_Setup = new("GNB_ST_BurstStrike_Setup", 0),
            GNB_AoE_FatedCircle_BurstStrike = new("GNB_AoE_FatedCircle_BurstStrike", 1),
            GNB_AoE_Overcap_Choice = new("GNB_AoE_Overcap_Choice", 0),
            GNB_AoE_NoMercyStop = new("GNB_AoE_NoMercyStop", 5),
            GNB_AoE_FatedCircle_Setup = new("GNB_AoE_FatedCircle_Setup", 0),
            GNB_AoE_SonicBreak_EarlyOrLate = new("GNB_AoE_SonicBreak_EarlyOrLate", 0),
            GNB_NM_Features_Weave = new("GNB_NM_Feature_Weave", 0),
            GNB_GF_Features_Choice = new("GNB_GF_Choice", 0),
            GNB_GF_Overcap_Choice = new("GNB_GF_Overcap_Choice", 0),
            GNB_GF_BurstStrike_Setup = new("GNB_GF_BurstStrike_Setup", 0),
            GNB_ST_Balance_Content = new("GNB_ST_Balance_Content", 1),
            GNB_Mit_Superbolide_Health = new("GNB_Mit_Superbolide_Health", 30),
            GNB_Mit_Corundum_Health = new("GNB_Mit_Corundum_Health", 60),
            GNB_Mit_Aurora_Charges = new("GNB_Mit_Aurora_Charges", 0),
            GNB_Mit_Aurora_Health = new("GNB_Mit_Aurora_Health", 60),
            GNB_Mit_HeartOfLight_PartyRequirement = new("GNB_Mit_HeartOfLight_PartyRequirement", (int)PartyRequirement.Yes),
            GNB_Mit_ArmsLength_Boss = new("GNB_Mit_ArmsLength_Boss", (int)BossAvoidance.On),
            GNB_Mit_ArmsLength_EnemyCount = new("GNB_Mit_ArmsLength_EnemyCount", 0);

        public static UserFloat
            GNB_Mitigation_Boss_Camouflage_Threshold = new("GNB_Mitigation_Boss_Camouflage_Threshold", 80f),
            GNB_Mitigation_NonBoss_MitigationThreshold = new("GNB_Mitigation_NonBoss_MitigationThreshold", 20f);
        
        public static UserBool
            GNB_Mitigation_Boss_Camouflage_Align = new("GNB_Mitigation_Boss_Camouflage_Align", true),
            GNB_Mitigation_Boss_Nebula_First = new("GNB_Mitigation_Boss_Nebula_First", true);

        public static UserIntArray
            GNB_Mit_Priorities = new("GNB_Mit_Priorities");

        public static UserBoolArray
            GNB_Mitigation_Boss_HeartOfStone_OnCD_Difficulty = new("GNB_Mitigation_Boss_HeartOfStone_OnCD_Difficulty", [true, false]),
            GNB_Mitigation_Boss_HeartOfStone_TankBuster_Difficulty = new("GNB_Mitigation_Boss_HeartOfStone_TankBuster_Difficulty", [true, false]),
            GNB_Mitigation_Boss_Rampart_Difficulty = new("GNB_Mitigation_Boss_Rampart_Difficulty", [true, false]),
            GNB_Mitigation_Boss_Nebula_Difficulty = new("GNB_Mitigation_Boss_Nebula_Difficulty", [true, false]),
            GNB_Mitigation_Boss_Camouflage_Difficulty = new("GNB_Mitigation_Boss_Camouflage_Difficulty", [true, false]),
            GNB_Mitigation_Boss_HeartOfLight_Difficulty = new("GNB_Mitigation_Boss_HeartOfLight_Difficulty", [true, false]),
            GNB_Mitigation_Boss_Reprisal_Difficulty = new("GNB_Mitigation_Boss_Reprisal_Difficulty", [true, false]),
            GNB_Mit_Superbolide_Difficulty = new("GNB_Mit_Superbolide_Difficulty", [true, false]);

        public static readonly ContentCheck.ListSet 
            GNB_Boss_Mit_DifficultyListSet = ContentCheck.ListSet.CasualVSHard,
            GNB_Mit_Superbolide_DifficultyListSet = ContentCheck.ListSet.CasualVSHard;

        private const int NumMitigationOptions = 8;

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Combo Mitigations
                case Preset.GNB_ST_Simple:
                    DrawHorizontalRadioButton(GNB_ST_MitOptions, "Include Simple Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(GNB_ST_MitOptions, "Exclude Simple Mitigations", "Disables the use of mitigations.", 1);
                    break;
               
                case Preset.GNB_AoE_Simple:
                    DrawHorizontalRadioButton(GNB_AoE_MitOptions, "Include Simple Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_MitOptions, "Exclude Simple Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.GNB_ST_Advanced:
                    DrawHorizontalRadioButton(GNB_ST_Advanced_MitOptions, "Include Advanced Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Advanced_MitOptions, "Exclude Advanced Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.GNB_AoE_Advanced:
                    DrawHorizontalRadioButton(GNB_AoE_Advanced_MitOptions, "Include Advanced Mitigations", "Enables the use of mitigations.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Advanced_MitOptions, "Exclude Advanced Mitigations", "Disables the use of mitigations.", 1);
                    break;
                
                case Preset.GNB_Mitigation_NonBoss:
                    DrawSliderFloat(0, 100, GNB_Mitigation_NonBoss_MitigationThreshold, "Stop using when average health percentage of nearby enemies is below set. \n(Set to 0 to disable this check) ", decimals: 0);
                    break;
                
                case Preset.GNB_Mitigation_Boss_Aurora:
                    DrawSliderInt(1, 100, GNB_Mitigation_Boss_Aurora_Health, "Player HP% to use Aurora at or below (100 = Disable check)");
                    break;
                
                case Preset.GNB_Mitigation_Boss_HeartOfStone_OnCD:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_HeartOfStone_OnCD_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawSliderInt(1, 100, GNB_Mitigation_Boss_HeartOfStone_Health, "Player HP% to use Heart of Stone/Corundum at or below (100 = Disable check)");
                    break;
                
                case Preset.GNB_Mitigation_Boss_HeartOfStone_TankBuster:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_HeartOfStone_TankBuster_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.GNB_Mitigation_Boss_Rampart:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_Rampart_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                
                case Preset.GNB_Mitigation_Boss_Nebula:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_Nebula_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawAdditionalBoolChoice(GNB_Mitigation_Boss_Nebula_First, "Use Nebula First", "Uses Nebula before Rampart");
                    break;
                
                case Preset.GNB_Mitigation_Boss_Camouflage:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_Camouflage_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    DrawSliderFloat(1, 100, GNB_Mitigation_Boss_Camouflage_Threshold, "Will use Camouflage as extra tankbuster mitigation if under this HP%", decimals: 0);
                    DrawAdditionalBoolChoice(GNB_Mitigation_Boss_Camouflage_Align, "Align Camouflage", "Tries to align Camouflage with Rampart for tankbusters.");
                    break;
                
                case Preset.GNB_Mitigation_Boss_HeartOfLight:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_HeartOfLight_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                    
                case Preset.GNB_Mitigation_Boss_Reprisal:
                    DrawDifficultyMultiChoice(GNB_Mitigation_Boss_Reprisal_Difficulty, GNB_Boss_Mit_DifficultyListSet ,
                        "Select which difficulties the ability should be used in:");
                    break;
                #endregion
                
                #region Single-Target
                case Preset.GNB_ST_Opener:
                    DrawHorizontalRadioButton(GNB_Opener_NM,
                        $"Normal {NoMercy.ActionName()}", $"Uses {NoMercy.ActionName()} normally in all openers", 0);
                    DrawHorizontalRadioButton(GNB_Opener_NM,
                        $"Early {NoMercy.ActionName()}", $"Uses {NoMercy.ActionName()} as soon as possible in all openers", 1);
                    ImGui.Spacing();
                    if (DrawHorizontalRadioButton(GNB_Opener_StartChoice,
                        $"Normal Opener", $"Starts opener with {LightningShot.ActionName()}", 0))
                    {
                        if (!CustomComboFunctions.InCombat())
                            Opener().OpenerStep = 1;
                    }
                    DrawHorizontalRadioButton(GNB_Opener_StartChoice,
                        $"Early Opener", $"Starts opener with {KeenEdge.ActionName()} instead, skipping {LightningShot.ActionName()}", 1);
                    ImGui.Spacing();
                    DrawBossOnlyChoice(GNB_ST_Balance_Content);
                    break;

                case Preset.GNB_ST_NoMercy:
                    DrawHorizontalRadioButton(GNB_ST_NoMercy_SubOption,
                        "All content", $"Uses {CustomComboFunctions.GetActionName(NoMercy)} regardless of content", 0);
                    DrawHorizontalRadioButton(GNB_ST_NoMercy_SubOption,
                        "Boss encounters Only", $"Only uses {CustomComboFunctions.GetActionName(NoMercy)} when in Boss encounters", 1);

                    DrawSliderInt(0, 75, GNB_ST_NoMercyStop,
                        " Stop usage if Target HP% is below set value.\n  To disable this, set value to 0");
                    break;

                case Preset.GNB_ST_BurstStrike:
                    DrawHorizontalRadioButton(GNB_ST_Overcap_Choice,
                        "Include Overcap Protection", $"Includes {BurstStrike.ActionName()} to prevent overcapping on cartridges", 0);
                    DrawHorizontalRadioButton(GNB_ST_Overcap_Choice,
                        "Exclude Overcap Protection", $"Excludes {BurstStrike.ActionName()}, regardless of cartridge count", 1);
                    ImGui.Spacing();
                    DrawHorizontalRadioButton(GNB_ST_BurstStrike_Setup,
                        "Precede No Mercy", $"Allow preceding {NoMercy.ActionName()} with {BurstStrike.ActionName()} for a buffed {Hypervelocity.ActionName()} (BS->NM->HV) - ONLY APPLIES TO 2.50", 0);
                    DrawHorizontalRadioButton(GNB_ST_BurstStrike_Setup,
                        "Don't Precede No Mercy", $"Forbid preceding {NoMercy.ActionName()} with {BurstStrike.ActionName()}", 1);
                    break;

                case Preset.GNB_ST_RangedUptime:
                    DrawHorizontalRadioButton(GNB_ST_HoldLightningShot,
                        "Hold for Continuation", "Holds Lightning Shot if you have any Continuation procs available to avoid loss", 1);
                    DrawHorizontalRadioButton(GNB_ST_HoldLightningShot,
                        "Don't Hold for Continuation", "Uses Lightning Shot regardless of any Continuation procs currently available", 0);
                    break;

                #endregion

                #region AoE
                case Preset.GNB_AoE_NoMercy:
                    DrawSliderInt(0, 75, GNB_AoE_NoMercyStop,
                        " Stop usage if Target HP% is below set value.\n To disable this, set value to 0");
                    break;

                case Preset.GNB_AoE_FatedCircle:
                    DrawHorizontalRadioButton(GNB_AoE_Overcap_Choice,
                        "Include Overcap Protection", $"Includes {FatedCircle.ActionName()} to prevent overcapping on cartridges", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Overcap_Choice,
                        "Exclude Overcap Protection", $"Excludes {FatedCircle.ActionName()}, regardless of cartridge count", 1);
                    ImGui.Spacing();
                    DrawHorizontalRadioButton(GNB_AoE_FatedCircle_BurstStrike,
                        "Include Burst Strike", $"Includes {BurstStrike.ActionName()} instead when {FatedCircle.ActionName()} is unavailable", 0);
                    DrawHorizontalRadioButton(GNB_AoE_FatedCircle_BurstStrike,
                        "Exclude Burst Strike", $"Excludes {BurstStrike.ActionName()} when {FatedCircle.ActionName()} is unavailable", 1);
                    ImGui.Spacing();
                    DrawHorizontalRadioButton(GNB_AoE_FatedCircle_Setup,
                        "Precede No Mercy", $"Allow preceding {NoMercy.ActionName()} with {FatedCircle.ActionName()} for a buffed {FatedBrand.ActionName()} (FC->NM->FB) - ONLY APPLIES TO 2.50", 0);
                    DrawHorizontalRadioButton(GNB_AoE_FatedCircle_Setup,
                        "Don't Precede No Mercy", $"Forbid preceding {NoMercy.ActionName()} with {FatedCircle.ActionName()}", 1);
                    break;

                case Preset.GNB_AoE_SonicBreak:
                    DrawHorizontalRadioButton(GNB_AoE_SonicBreak_EarlyOrLate,
                        "Normal Usage", $"Uses {SonicBreak.ActionName()} normally", 0);
                    DrawHorizontalRadioButton(GNB_AoE_SonicBreak_EarlyOrLate,
                        "Late Usage", $"Uses {SonicBreak.ActionName()} as the last GCD in burst", 1);
                    break;
                #endregion

                #region One-Button Mitigation

                case Preset.GNB_Mit_Superbolide_Max:
                    DrawDifficultyMultiChoice(GNB_Mit_Superbolide_Difficulty, GNB_Mit_Superbolide_DifficultyListSet,
                        "Select what difficulties Superbolide should be used in:");
                    DrawSliderInt(1, 100, GNB_Mit_Superbolide_Health, "Player HP% to be \nless than or equal to:", 200, SliderIncrements.Fives);
                    break;

                case Preset.GNB_Mit_Corundum:
                    DrawSliderInt(1, 100, GNB_Mit_Corundum_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 0,
                        "Heart of Corundum Priority:");
                    break;

                case Preset.GNB_Mit_Aurora:
                    DrawSliderInt(0, 1, GNB_Mit_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");
                    DrawSliderInt(1, 100, GNB_Mit_Aurora_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 1,
                        "Aurora Priority:");
                    break;

                case Preset.GNB_Mit_Camouflage:
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 2,
                        "Camouflage Priority:");
                    break;

                case Preset.GNB_Mit_Reprisal:
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 3,
                        "Reprisal Priority:");
                    break;

                case Preset.GNB_Mit_HeartOfLight:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(GNB_Mit_HeartOfLight_PartyRequirement,
                        "Require party", "Will not use Heart of Light unless there are 2 or more party members.",
                        (int)PartyRequirement.Yes);
                    DrawHorizontalRadioButton(GNB_Mit_HeartOfLight_PartyRequirement,
                        "Use Always", "Will not require a party for Heart of Light.",
                        (int)PartyRequirement.No);
                    ImGui.Unindent();
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 4,
                        "Heart of Light Priority:");
                    break;

                case Preset.GNB_Mit_Rampart:
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 5,
                        "Rampart Priority:");
                    break;

                case Preset.GNB_Mit_ArmsLength:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(GNB_Mit_ArmsLength_Boss,
                        "All Enemies", "Will use Arm's Length regardless of the type of enemy.",
                        (int)BossAvoidance.Off, 125f);
                    DrawHorizontalRadioButton(
                        GNB_Mit_ArmsLength_Boss,
                        "Avoid Bosses", "Will try not to use Arm's Length when in a boss fight.",
                        (int)BossAvoidance.On, 125f);
                    ImGui.Unindent();
                    DrawSliderInt(0, 5, GNB_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 6,
                        "Arm's Length Priority:");
                    break;

                case Preset.GNB_Mit_Nebula:
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 7,
                        "Nebula Priority:");
                    break;
                #endregion

                #region Other
                case Preset.GNB_NM_Features:
                    DrawHorizontalRadioButton(GNB_NM_Features_Weave,
                        "Weave-Only", "Uses cooldowns only when inside a weave window (excludes No Mercy)", 0);
                    DrawHorizontalRadioButton(GNB_NM_Features_Weave,
                        "On Cooldown", "Uses cooldowns as soon as possible", 1);
                    break;

                case Preset.GNB_GF_Features:
                    DrawHorizontalRadioButton(GNB_GF_Features_Choice,
                        "Replace Gnashing Fang", $"Use this feature as intended on {GnashingFang.ActionName()}", 0);
                    DrawHorizontalRadioButton(GNB_GF_Features_Choice,
                        "Replace No Mercy", $"Use this feature instead on {NoMercy.ActionName()}\nWARNING: This WILL conflict with 'No Mercy Features'!", 1);
                    break;

                case Preset.GNB_GF_BurstStrike:
                    DrawHorizontalRadioButton(GNB_GF_Overcap_Choice,
                        "Include Overcap Protection", $"Includes {BurstStrike.ActionName()} to prevent overcapping on cartridges", 0);
                    DrawHorizontalRadioButton(GNB_GF_Overcap_Choice,
                        "Exclude Overcap Protection", $"Excludes {BurstStrike.ActionName()}, regardless of cartridge count", 1);
                    ImGui.Spacing();
                    DrawHorizontalRadioButton(GNB_GF_BurstStrike_Setup,
                        "Precede No Mercy", $"Allow preceding {NoMercy.ActionName()} with {BurstStrike.ActionName()} for a buffed {Hypervelocity.ActionName()} (BS->NM->HV) - ONLY APPLIES TO 2.50", 0);
                    DrawHorizontalRadioButton(GNB_GF_BurstStrike_Setup,
                        "Don't Precede No Mercy", $"Forbid preceding {NoMercy.ActionName()} with {BurstStrike.ActionName()}", 1);
                    break;

                    #endregion
            }
        }
    }
}
