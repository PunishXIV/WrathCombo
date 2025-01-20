using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;
using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    internal static class Config
    {
        private const int numberMitigationOptions = 8;

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
            WAR_InfuriateRange = new("WAR_InfuriateRange", 40),
            WAR_SurgingRefreshRange = new("WAR_SurgingRefreshRange", 10),
            WAR_KeepOnslaughtCharges = new("WAR_KeepOnslaughtCharges", 1),
            WAR_KeepInfuriateCharges = new("WAR_KeepInfuriateCharges", 0),
            WAR_FellCleaveGauge = new("WAR_FellCleaveGauge", 90),
            WAR_DecimateGauge = new("WAR_DecimateGauge", 90),
            WAR_InfuriateSTGauge = new("WAR_InfuriateSTGauge", 40),
            WAR_InfuriateAoEGauge = new("WAR_InfuriateAoEGauge", 40),
            WAR_EyePath_Refresh = new("WAR_EyePath", 10),
            WAR_ST_Bloodwhetting_Health = new("WAR_ST_BloodwhettingOption", 90),
            WAR_ST_Bloodwhetting_SubOption = new("WAR_ST_Bloodwhetting_SubOpt", 0),
            WAR_ST_Equilibrium_Health = new("WAR_ST_EquilibriumOption", 50),
            WAR_ST_Equilibrium_SubOption = new("WAR_ST_Equilibrium_SubOpt", 0),
            WAR_ST_Rampart_Health = new("WAR_ST_Rampart_Health", 80),
            WAR_ST_Rampart_SubOption = new("WAR_ST_Rampart_SubOption", 0),
            WAR_ST_Thrill_Health = new("WAR_ST_Thrill_Health", 70),
            WAR_ST_Thrill_SubOption = new("WAR_ST_Thrill_SubOpt", 0),
            WAR_ST_Vengeance_Health = new("WAR_ST_Vengeance_Health", 60),
            WAR_ST_Vengeance_SubOption = new("WAR_ST_Vengeance_SubOpt", 0),
            WAR_ST_Holmgang_Health = new("WAR_ST_Holmgang_Health", 30),
            WAR_ST_Holmgang_SubOption = new("WAR_ST_Holmgang_SubOpt", 0),
            WAR_ST_Reprisal_Health = new("WAR_ST_Reprisal_Health", 80),
            WAR_ST_Reprisal_SubOption = new("WAR_ST_Reprisal_SubOpt", 0),
            WAR_ST_ArmsLength_Health = new("WAR_ST_ArmsLength_Health", 80),
            WAR_AoE_Bloodwhetting_Health = new("WAR_AoE_BloodwhettingOption", 90),
            WAR_AoE_Bloodwhetting_SubOption = new("WAR_AoE_Bloodwhetting_SubOpt", 0),
            WAR_AoE_Equilibrium_Health = new("WAR_AoE_EquilibriumOption", 50),
            WAR_AoE_Equilibrium_SubOption = new("WAR_AoE_Equilibrium_SubOpt", 0),
            WAR_AoE_Rampart_Health = new("WAR_AoE_Rampart_Health", 80),
            WAR_AoE_Rampart_SubOption = new("WAR_AoE_Rampart_SubOpt", 0),
            WAR_AoE_Thrill_Health = new("WAR_AoE_Thrill_Health", 80),
            WAR_AoE_Thrill_SubOption = new("WAR_AoE_Thrill_SubOpt", 0),
            WAR_AoE_Vengeance_Health = new("WAR_AoE_Vengeance_Health", 60),
            WAR_AoE_Vengeance_SubOption = new("WAR_AoE_Vengeance_SubOpt", 0),
            WAR_AoE_Holmgang_Health = new("WAR_AoE_Holmgang_Health", 30),
            WAR_AoE_Holmgang_SubOption = new("WAR_AoE_Holmgang_SubOpt", 0),
            WAR_AoE_Reprisal_Health = new("WAR_AoE_Reprisal_Health", 80),
            WAR_AoE_Reprisal_SubOption = new("WAR_AoE_Reprisal_SubOpt", 0),
            WAR_AoE_ArmsLength_Health = new("WAR_AoE_ArmsLength_Health", 80),
            WAR_ST_MitsOptions = new("WAR_ST_MitsOptions", 0),
            WAR_AoE_MitsOptions = new("WAR_AoE_MitsOptions", 0),
            WAR_VariantCure = new("WAR_VariantCure"),
            WAR_BalanceOpener_Content = new("WAR_BalanceOpener_Content", 1),

            //One-Button Mitigation
            WAR_Mit_Holmgang_Health = new("WAR_Mit_Holmgang_Health", 30),
            WAR_Mit_Bloodwhetting_Health = new("WAR_Mit_Bloodwhetting_Health", 70),
            WAR_Mit_Equilibrium_Health = new("WAR_Mit_Equilibrium_Health", 45),
            WAR_Mit_ThrillOfBattle_Health = new("WAR_Mit_ThrillOfBattle_Health", 60),
            WAR_Mit_Rampart_Health = new("WAR_Mit_Rampart_Health", 65),
            WAR_Mit_ShakeItOff_PartyRequirement = new("WAR_Mit_ShakeItOff_PartyRequirement", (int) PartyRequirement.Yes),
            WAR_Mit_ArmsLength_Boss = new("WAR_Mit_ArmsLength_Boss", (int) BossAvoidance.On),
            WAR_Mit_ArmsLength_EnemyCount = new("WAR_Mit_ArmsLength_EnemyCount", 0),
            WAR_Mit_Vengeance_Health = new("WAR_Mit_Vengeance_Health", 50);

        public static UserIntArray
            WAR_Mit_Priorities = new("WAR_Mit_Priorities");

        public static UserBoolArray
            WAR_Mit_Holmgang_Difficulty = new("WAR_Mit_Holmgang_Difficulty",
                [true, false]);

        public static readonly ContentCheck.ListSet
            WAR_Mit_Holmgang_DifficultyListSet =
                ContentCheck.ListSet.Halved;

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.WAR_ST_Advanced_BalanceOpener:
                    UserConfig.DrawBossOnlyChoice(WAR_BalanceOpener_Content);
                    break;

                case CustomComboPreset.WAR_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, WAR_VariantCure,
                        "Player HP% to be \nless than or equal to:", 200);

                    break;

                case CustomComboPreset.WAR_InfuriateFellCleave:
                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateRange,
                        "Use when gauge is less than or equal to:");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_StormsEye:
                    UserConfig.DrawSliderInt(0, 30, WAR_SurgingRefreshRange,
                        $"Seconds remaining before refreshing {Buffs.SurgingTempest.StatusName()} buff");

                    break;

                case CustomComboPreset.WAR_EyePath:
                    UserConfig.DrawSliderInt(0, 30, WAR_EyePath_Refresh,
                        $"Seconds remaining before refreshing {Buffs.SurgingTempest.StatusName()} buff");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Onslaught:
                    UserConfig.DrawSliderInt(0, 2, WAR_KeepOnslaughtCharges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Infuriate:
                    UserConfig.DrawSliderInt(0, 2, WAR_KeepInfuriateCharges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateSTGauge,
                        "Use when gauge is less than or equal to:");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_FellCleave:
                    UserConfig.DrawSliderInt(50, 100, WAR_FellCleaveGauge,
                        "Minimum gauge required to spend");

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Decimate:
                    UserConfig.DrawSliderInt(50, 100, WAR_DecimateGauge,
                        "Minimum gauge required to spend");

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Infuriate:
                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateAoEGauge,
                        "Use when gauge is under or equal to");

                    break;

                case CustomComboPreset.WARPvP_BurstMode_Blota:
                    UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, $"Before {PrimalRend.ActionName()}", "", 0);
                    UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, $"After {PrimalRend.ActionName()}", "", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Bloodwhetting:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Bloodwhetting_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Bloodwhetting_SubOption,
                        "All Enemies",
                        $"Uses {Bloodwhetting.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Bloodwhetting_SubOption,
                        "Bosses Only",
                        $"Only uses {Bloodwhetting.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Bloodwhetting:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Bloodwhetting_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Bloodwhetting_SubOption,
                        "All Enemies",
                        $"Uses {Bloodwhetting.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Bloodwhetting_SubOption,
                        "Bosses Only",
                       $"Only uses {Bloodwhetting.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Equilibrium:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Equilibrium_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Equilibrium_SubOption,
                        "All Enemies",
                        $"Uses {Equilibrium.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Equilibrium_SubOption,
                        "Bosses Only",
                        $"Only uses {Equilibrium.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Equilibrium:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Equilibrium_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Equilibrium_SubOption,
                        "All Enemies",
                        $"Uses {Equilibrium.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Equilibrium_SubOption,
                        "Bosses Only",
                        $"Only uses {Equilibrium.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Rampart:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Rampart_SubOption,
                        "All Enemies",
                        $"Uses {All.Rampart.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Rampart_SubOption,
                        "Bosses Only",
                        $"Only uses {All.Rampart.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Rampart:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Rampart_SubOption,
                        "All Enemies",
                        $"Uses {All.Rampart.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Rampart_SubOption,
                        "Bosses Only",
                        $"Only uses {All.Rampart.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Thrill:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Thrill_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Thrill_SubOption,
                        "All Enemies",
                        $"Uses {ThrillOfBattle.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Thrill_SubOption,
                        "Bosses Only",
                        $"Only uses {ThrillOfBattle.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Thrill:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Thrill_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Thrill_SubOption,
                        "All Enemies",
                        $"Uses {ThrillOfBattle.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Thrill_SubOption,
                        "Bosses Only",
                        $"Only uses {ThrillOfBattle.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Vengeance:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Vengeance_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Vengeance_SubOption,
                        "All Enemies",
                        $"Uses {Vengeance.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Vengeance_SubOption,
                        "Bosses Only",
                        $"Only uses {Vengeance.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Vengeance:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Vengeance_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Vengeance_SubOption,
                        "All Enemies",
                        $"Uses {Vengeance.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Vengeance_SubOption,
                        "Bosses Only",
                        $"Only uses {Vengeance.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Holmgang:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Holmgang_SubOption,
                        "All Enemies",
                        $"Uses {Holmgang.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Holmgang_SubOption,
                        "Bosses Only",
                        $"Only uses {Holmgang.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Holmgang:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Holmgang_SubOption,
                        "All Enemies",
                        $"Uses {Holmgang.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Holmgang_SubOption,
                        "Bosses Only",
                        $"Only uses {Holmgang.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.WAR_ST_Simple:
                    UserConfig.DrawHorizontalRadioButton(WAR_ST_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                case CustomComboPreset.WAR_AoE_Simple:
                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                case CustomComboPreset.WAR_ST_Advanced_Reprisal:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Reprisal_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Reprisal_SubOption,
                        "All Enemies",
                        $"Uses {All.Reprisal.ActionName()} regardless of targeted enemy type.", 0);
                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Reprisal_SubOption,
                        "Bosses Only",
                        $"Only uses {All.Reprisal.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Reprisal:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Reprisal_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Reprisal_SubOption,
                        "All Enemies",
                        $"Uses {All.Reprisal.ActionName()} regardless of targeted enemy type.", 0);
                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Reprisal_SubOption,
                        "Bosses Only",
                        $"Only uses {All.Reprisal.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                #region One-Button Mitigation

                case CustomComboPreset.WAR_Mit_Holmgang_Max:
                    UserConfig.DrawDifficultyMultiChoice(
                        WAR_Mit_Holmgang_Difficulty,
                        WAR_Mit_Holmgang_DifficultyListSet,
                        "Select what difficulties Holmgang should be used in:"
                    );

                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:",
                        200, SliderIncrements.Fives);
                    break;

                case CustomComboPreset.WAR_Mit_Bloodwhetting:
                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_Bloodwhetting_Health,
                        "HP% to use at or below",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 0,
                        "Bloodwhetting Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_Equilibrium:
                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_Equilibrium_Health,
                        "HP% to use at or below",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 1,
                        "Equilibrium Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_Reprisal:
                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 2,
                        "Reprisal Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_ThrillOfBattle:
                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_ThrillOfBattle_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 3,
                        "Thrill Of Battle Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_Rampart:
                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_Rampart_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 4,
                        "Rampart Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_ShakeItOff:
                    ImGui.Dummy(new Vector2(15f.Scale(), 0f));
                    ImGui.SameLine();
                    UserConfig.DrawHorizontalRadioButton(
                        WAR_Mit_ShakeItOff_PartyRequirement,
                        "Require party",
                        "Will not use Shake It Off unless there are 2 or more party members.",
                        outputValue: (int) PartyRequirement.Yes);
                    UserConfig.DrawHorizontalRadioButton(
                        WAR_Mit_ShakeItOff_PartyRequirement,
                        "Use Always",
                        "Will not require a party for Shake It Off.",
                        outputValue: (int) PartyRequirement.No);

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 5,
                        "Shake It Off Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_ArmsLength:
                    ImGui.Dummy(new Vector2(15f.Scale(), 0f));
                    ImGui.SameLine();
                    UserConfig.DrawHorizontalRadioButton(
                        WAR_Mit_ArmsLength_Boss, "All Enemies",
                        "Will use Arm's Length regardless of the type of enemy.",
                        outputValue: (int) BossAvoidance.Off, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        WAR_Mit_ArmsLength_Boss, "Avoid Bosses",
                        "Will try not to use Arm's Length when in a boss fight.",
                        outputValue: (int) BossAvoidance.On, itemWidth: 125f);

                    UserConfig.DrawSliderInt(0, 3, WAR_Mit_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 6,
                        "Arm's Length Priority:");
                    break;

                case CustomComboPreset.WAR_Mit_Vengeance:
                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_Vengeance_Health,
                        "HP% to use at or below (100 = Disable check)",
                        sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawPriorityInput(WAR_Mit_Priorities,
                        numberMitigationOptions, 7,
                        "Vengeance Priority:");
                    break;

                    #endregion
            }
        }
    }
}
