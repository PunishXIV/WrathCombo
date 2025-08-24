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
            GNB_Opener_StartChoice = new("GNB_Opener_StartChoice", 0),
            GNB_Opener_NM = new("GNB_Opener_NM", 0),
            GNB_ST_MitsOptions = new("GNB_ST_MitsOptions", 0),
            GNB_ST_Corundum_Health = new("GNB_ST_CorundumOption", 90),
            GNB_ST_Corundum_SubOption = new("GNB_ST_Corundum_Option", 0),
            GNB_ST_Aurora_Health = new("GNB_ST_Aurora_Health", 99),
            GNB_ST_Aurora_Charges = new("GNB_ST_Aurora_Charges", 0),
            GNB_ST_Aurora_SubOption = new("GNB_ST_Aurora_Option", 0),
            GNB_ST_Rampart_Health = new("GNB_ST_Rampart_Health", 80),
            GNB_ST_Rampart_SubOption = new("GNB_ST_Rampart_Option", 0),
            GNB_ST_Camouflage_Health = new("GNB_ST_Camouflage_Health", 70),
            GNB_ST_Camouflage_SubOption = new("GNB_ST_Camouflage_Option", 0),
            GNB_ST_Nebula_Health = new("GNB_ST_Nebula_Health", 60),
            GNB_ST_Nebula_SubOption = new("GNB_ST_Nebula_Option", 0),
            GNB_ST_Superbolide_Health = new("GNB_ST_Superbolide_Health", 30),
            GNB_ST_Superbolide_SubOption = new("GNB_ST_Superbolide_Option", 0),
            GNB_ST_Reprisal_Health = new("GNB_ST_Reprisal_Health", 0),
            GNB_ST_Reprisal_SubOption = new("GNB_ST_Reprisal_Option", 0),
            GNB_ST_ArmsLength_Health = new("GNB_ST_ArmsLength_Health", 0),
            GNB_ST_NoMercyStop = new("GNB_ST_NoMercyStop", 5),
            GNB_ST_NoMercy_SubOption = new("GNB_ST_NoMercy_SubOption", 1),
            GNB_ST_Overcap_Choice = new("GNB_ST_Overcap_Choice", 0),
            GNB_AoE_MitsOptions = new("GNB_AoE_MitsOptions", 0),
            GNB_AoE_Corundum_Health = new("GNB_AoE_CorundumOption", 90),
            GNB_AoE_Corundum_SubOption = new("GNB_AoE_Corundum_Option", 0),
            GNB_AoE_Aurora_Health = new("GNB_AoE_Aurora_Health", 99),
            GNB_AoE_Aurora_Charges = new("GNB_AoE_Aurora_Charges", 0),
            GNB_AoE_Aurora_SubOption = new("GNB_AoE_Aurora_Option", 0),
            GNB_AoE_Rampart_Health = new("GNB_AoE_Rampart_Health", 80),
            GNB_AoE_Rampart_SubOption = new("GNB_AoE_Rampart_Option", 10),
            GNB_AoE_Camouflage_Health = new("GNB_AoE_Camouflage_Health", 80),
            GNB_AoE_Camouflage_SubOption = new("GNB_AoE_Camouflage_Option", 0),
            GNB_AoE_Nebula_Health = new("GNB_AoE_Nebula_Health", 60),
            GNB_AoE_Nebula_SubOption = new("GNB_AoE_Nebula_Option", 0),
            GNB_AoE_Superbolide_Health = new("GNB_AoE_Superbolide_Health", 30),
            GNB_AoE_Superbolide_SubOption = new("GNB_AoE_Superbolide_Option", 0),
            GNB_AoE_Reprisal_Health = new("GNB_AoE_Reprisal_Health", 0),
            GNB_AoE_Reprisal_SubOption = new("GNB_AoE_Reprisal_Option", 0),
            GNB_AoE_ArmsLength_Health = new("GNB_AoE_ArmsLength_Health", 0),
            GNB_AoE_FatedCircle_BurstStrike = new("GNB_AoE_FatedCircle_BurstStrike", 1),
            GNB_AoE_Overcap_Choice = new("GNB_AoE_Overcap_Choice", 0),
            GNB_AoE_NoMercyStop = new("GNB_AoE_NoMercyStop", 5),
            GNB_NM_Features_Weave = new("GNB_NM_Feature_Weave", 0),
            GNB_GF_Features_Choice = new("GNB_GF_Choice", 0),
            GNB_GF_Overcap_Choice = new("GNB_GF_Overcap_Choice", 0),
            GNB_ST_Balance_Content = new("GNB_ST_Balance_Content", 1),
            GNB_Mit_Superbolide_Health = new("GNB_Mit_Superbolide_Health", 30),
            GNB_Mit_Corundum_Health = new("GNB_Mit_Corundum_Health", 60),
            GNB_Mit_Aurora_Charges = new("GNB_Mit_Aurora_Charges", 0),
            GNB_Mit_Aurora_Health = new("GNB_Mit_Aurora_Health", 60),
            GNB_Mit_HeartOfLight_PartyRequirement = new("GNB_Mit_HeartOfLight_PartyRequirement", (int)PartyRequirement.Yes),
            GNB_Mit_Rampart_Health = new("GNB_Mit_Rampart_Health", 65),
            GNB_Mit_ArmsLength_Boss = new("GNB_Mit_ArmsLength_Boss", (int)BossAvoidance.On),
            GNB_Mit_ArmsLength_EnemyCount = new("GNB_Mit_ArmsLength_EnemyCount", 0),
            GNB_Mit_Nebula_Health = new("GNB_Mit_Nebula_Health", 50),
            GNB_Bozja_LostCure_Health = new("GNB_Bozja_LostCure_Health", 50),
            GNB_Bozja_LostCure2_Health = new("GNB_Bozja_LostCure2_Health", 50),
            GNB_Bozja_LostCure3_Health = new("GNB_Bozja_LostCure3_Health", 50),
            GNB_Bozja_LostCure4_Health = new("GNB_Bozja_LostCure4_Health", 50),
            GNB_Bozja_LostAethershield_Health = new("GNB_Bozja_LostAethershield_Health", 70),
            GNB_Bozja_LostReraise_Health = new("GNB_Bozja_LostReraise_Health", 10);

        public static UserIntArray
            GNB_Mit_Priorities = new("GNB_Mit_Priorities");

        public static UserBoolArray
            GNB_Mit_Superbolide_Difficulty = new("GNB_Mit_Superbolide_Difficulty", [true, false]);

        public static readonly ContentCheck.ListSet GNB_Mit_Superbolide_DifficultyListSet = ContentCheck.ListSet.Halved;

        private const int NumMitigationOptions = 8;

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single-Target
                case Preset.GNB_ST_Opener:
                    DrawHorizontalRadioButton(GNB_Opener_NM,
                        $"Normal {NoMercy.ActionName()}", $"Uses {NoMercy.ActionName()} normally in all openers", 0);
                    DrawHorizontalRadioButton(GNB_Opener_NM,
                        $"Early {NoMercy.ActionName()}", $"Uses {NoMercy.ActionName()} as soon as possible in all openers", 1);

                    if (DrawHorizontalRadioButton(GNB_Opener_StartChoice,
                        $"Normal Opener", $"Starts opener with {LightningShot.ActionName()}", 0))
                    {
                        if (!CustomComboFunctions.InCombat())
                            Opener().OpenerStep = 1;
                    }
                    DrawHorizontalRadioButton(GNB_Opener_StartChoice,
                        $"Early Opener", $"Starts opener with {KeenEdge.ActionName()} instead, skipping {LightningShot.ActionName()}", 1);

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
                    break;
                #endregion

                #region Mitigations
                case Preset.GNB_ST_Corundum:
                    DrawSliderInt(1, 100, GNB_ST_Corundum_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_ST_Corundum_SubOption,
                        "All Enemies", $"Uses {HeartOfCorundum.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Corundum_SubOption,
                        "Bosses Only", $"Only ses {HeartOfCorundum.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Corundum:
                    DrawSliderInt(1, 100, GNB_AoE_Corundum_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_AoE_Corundum_SubOption,
                        "All Enemies", $"Uses {HeartOfCorundum.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Corundum_SubOption,
                        "Bosses Only", $"Only uses {HeartOfCorundum.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_ST_Aurora:
                    DrawSliderInt(1, 100, GNB_ST_Aurora_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawSliderInt(0, 1, GNB_ST_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");
                    DrawHorizontalRadioButton(GNB_ST_Aurora_SubOption,
                        "All Enemies", $"Uses {Aurora.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Aurora_SubOption,
                        "Bosses Only", $"Only uses {Aurora.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Aurora:
                    DrawSliderInt(1, 100, GNB_AoE_Aurora_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawSliderInt(0, 1, GNB_AoE_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");
                    DrawHorizontalRadioButton(GNB_AoE_Aurora_SubOption,
                        "All Enemies", $"Uses {Aurora.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Aurora_SubOption,
                        "Bosses Only", $"Only uses {Aurora.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_ST_Rampart:
                    DrawSliderInt(1, 100, GNB_ST_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_ST_Rampart_SubOption,
                        "All Enemies", $"Uses {Role.Rampart.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Rampart_SubOption,
                        "Bosses Only", $"Only uses {Role.Rampart.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Rampart:
                    DrawSliderInt(1, 100, GNB_AoE_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_AoE_Rampart_SubOption,
                        "All Enemies", $"Uses {Role.Rampart.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Rampart_SubOption,
                        "Bosses Only", $"Only uses {Role.Rampart.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_ST_Camouflage:
                    DrawSliderInt(1, 100, GNB_ST_Camouflage_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_ST_Camouflage_SubOption,
                        "All Enemies", $"Uses {Camouflage.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Camouflage_SubOption,
                        "Bosses Only", $"Only uses {Camouflage.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Camouflage:
                    DrawSliderInt(1, 100, GNB_AoE_Camouflage_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_AoE_Camouflage_SubOption,
                        "All Enemies", $"Uses {Camouflage.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Camouflage_SubOption,
                        "Bosses Only", $"Only uses {Camouflage.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_ST_Nebula:
                    DrawSliderInt(1, 100, GNB_ST_Nebula_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_ST_Nebula_SubOption,
                        "All Enemies", $"Uses {Nebula.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Nebula_SubOption,
                        "Bosses Only", $"Only uses {Nebula.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Nebula:
                    DrawSliderInt(1, 100, GNB_AoE_Nebula_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_AoE_Nebula_SubOption,
                        "All Enemies", $"Uses {Nebula.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Nebula_SubOption,
                        "Bosses Only", $"Only uses {Nebula.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_ST_Superbolide:
                    DrawSliderInt(1, 100, GNB_ST_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_ST_Superbolide_SubOption,
                        "All Enemies", $"Uses {Superbolide.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Superbolide_SubOption,
                        "Bosses Only", $"Only uses {Superbolide.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Superbolide:
                    DrawSliderInt(1, 100, GNB_AoE_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_AoE_Superbolide_SubOption,
                        "All Enemies", $"Uses {Superbolide.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Superbolide_SubOption,
                        "Bosses Only", $"Only uses {Superbolide.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_ST_Reprisal:
                    DrawSliderInt(1, 100, GNB_ST_Reprisal_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_ST_Reprisal_SubOption,
                        "All Enemies", $"Uses {Role.Reprisal.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_ST_Reprisal_SubOption,
                        "Bosses Only", $"Only uses {Role.Reprisal.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.GNB_AoE_Reprisal:
                    DrawSliderInt(1, 100, GNB_AoE_Reprisal_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    DrawHorizontalRadioButton(GNB_AoE_Reprisal_SubOption,
                        "All Enemies", $"Uses {Role.Reprisal.ActionName()} regardless of targeted enemy type.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_Reprisal_SubOption,
                        "Bosses Only", $"Only uses {Role.Reprisal.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

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
                    ImGui.NewLine();
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 4,
                        "Heart of Light Priority:");
                    break;

                case Preset.GNB_Mit_Rampart:
                    DrawSliderInt(1, 100, GNB_Mit_Rampart_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);
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
                    ImGui.NewLine();
                    DrawSliderInt(0, 3, GNB_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 6,
                        "Arm's Length Priority:");
                    break;

                case Preset.GNB_Mit_Nebula:
                    DrawSliderInt(1, 100, GNB_Mit_Nebula_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);
                    DrawPriorityInput(GNB_Mit_Priorities, NumMitigationOptions, 7,
                        "Nebula Priority:");
                    break;
                #endregion

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
                    break;

                case Preset.GNB_ST_Simple:
                    DrawHorizontalRadioButton(GNB_ST_MitsOptions,
                        "Include Mitigations", "Enables the use of mitigations in Simple Mode.", 0);
                    DrawHorizontalRadioButton(GNB_ST_MitsOptions,
                        "Exclude Mitigations", "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                case Preset.GNB_AoE_Simple:
                    DrawHorizontalRadioButton(GNB_AoE_MitsOptions,
                        "Include Mitigations", "Enables the use of mitigations in Simple Mode.", 0);
                    DrawHorizontalRadioButton(GNB_AoE_MitsOptions,
                        "Exclude Mitigations", "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                case Preset.GNB_Bozja_LostCure:
                    DrawSliderInt(1, 100, GNB_Bozja_LostCure_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.GNB_Bozja_LostCure2:
                    DrawSliderInt(1, 100, GNB_Bozja_LostCure2_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.GNB_Bozja_LostCure3:
                    DrawSliderInt(1, 100, GNB_Bozja_LostCure3_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.GNB_Bozja_LostCure4:
                    DrawSliderInt(1, 100, GNB_Bozja_LostCure4_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.GNB_Bozja_LostAethershield:
                    DrawSliderInt(1, 100, GNB_Bozja_LostAethershield_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.GNB_Bozja_LostReraise:
                    DrawSliderInt(1, 100, GNB_Bozja_LostReraise_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                    #endregion
            }
        }
    }
}
