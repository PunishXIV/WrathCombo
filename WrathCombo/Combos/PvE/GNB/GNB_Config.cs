using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    internal static class Config
    {
        private const int NumMitigationOptions = 8;
        public static UserInt
            GNB_ST_MitOptions = new("GNB_ST_MitOptions"),
            GNB_AoE_MitOptions = new("GNB_AoE_MitOptions"),
            GNB_ST_Advanced_MitOptions = new("GNB_ST_Advanced_MitOptions"),
            GNB_AoE_Advanced_MitOptions = new("GNB_AoE_Advanced_MitOptions"),
            GNB_Mit_Advanced_NonBoss_SuperBolide_Health = new("GNB_Mit_Advanced_NonBoss_SuperBolide_Health", 20),
            GNB_Mit_Advanced_Boss_Aurora_Health = new("GNB_Mit_Advanced_Boss_Aurora_Health", 99),
            GNB_Mit_Advanced_Boss_HeartOfStone_Health = new("GNB_Mit_Advanced_Boss_HeartOfStone_Health", 80),
            GNB_Mit_Advanced_Boss_HeartOfStoneDelay = new("GNB_Mit_Advanced_Boss_HeartOfStoneDelay"),
            GNB_Opener_NM = new("GNB_Opener_NM"),
            GNB_ST_NM_BossOption = new("GNB_ST_NM_BossOption"),
            GNB_ST_NM_HPOption = new("GNB_ST_NM_HPOption", 25),
            GNB_ST_Overcap_Choice = new("GNB_ST_Overcap_Choice"),
            GNB_ST_HoldLightningShot = new("GNB_ST_HoldLightningShot"),
            GNB_ST_HoldLightningShotInBurst = new("GNB_ST_HoldLightningShotInBurst"),
            GNB_ST_HoldGFCharge = new("GNB_ST_HoldGFCharge", 0),
            GNB_ST_BurstStrike_Setup = new("GNB_ST_BurstStrike_Setup"),
            GNB_AoE_FatedCircle_BurstStrike = new("GNB_AoE_FatedCircle_BurstStrike", 1),
            GNB_AoE_Overcap_Choice = new("GNB_AoE_Overcap_Choice"),
            GNB_AoE_NoMercyStop = new("GNB_AoE_NoMercyStop", 25),
            GNB_AoE_FatedCircle_Setup = new("GNB_AoE_FatedCircle_Setup"),
            GNB_AoE_SonicBreak_EarlyOrLate = new("GNB_AoE_SonicBreak_EarlyOrLate"),
            GNB_BS_DoubleDown_NMOnly = new("GNB_BS_DoubleDown_NMOnly", 0),
            GNB_FC_DoubleDown_NMOnly = new("GNB_FC_DoubleDown_NMOnly", 0),
            GNB_BS_Continuation_Procs = new("GNB_BS_Continuation_Procs", 0),
            GNB_FC_Continuation_Procs = new("GNB_FC_Continuation_Procs", 0),
            GNB_NM_Features_Weave = new("GNB_NM_Feature_Weave"),
            GNB_GF_Features_Choice = new("GNB_GF_Choice"),
            GNB_GF_Overcap_Choice = new("GNB_GF_Overcap_Choice"),
            GNB_GF_BurstStrike_Setup = new("GNB_GF_BurstStrike_Setup"),
            GNB_ST_Balance_Content = new("GNB_ST_Balance_Content", 1),
            GNB_Mit_OneButton_Superbolide_Health = new("GNB_Mit_OneButton_Superbolide_Health", 30),
            GNB_Mit_OneButton_Corundum_Health = new("GNB_Mit_OneButton_Corundum_Health", 60),
            GNB_Mit_OneButton_Aurora_Charges = new("GNB_Mit_OneButton_Aurora_Charges"),
            GNB_Mit_OneButton_Aurora_Health = new("GNB_Mit_OneButton_Aurora_Health", 60),
            GNB_Mit_OneButton_HeartOfLight_PartyRequirement = new("GNB_Mit_OneButton_HeartOfLight_PartyRequirement", (int)PartyRequirement.Yes),
            GNB_Mit_OneButton_ArmsLength_Boss = new("GNB_Mit_OneButton_ArmsLength_Boss", (int)BossAvoidance.On),
            GNB_Mit_OneButton_ArmsLength_EnemyCount = new("GNB_Mit_OneButton_ArmsLength_EnemyCount");

        public static UserFloat
            GNB_Mit_Advanced_Boss_Camouflage_Threshold = new("GNB_Mit_Advanced_Boss_Camouflage_Threshold", 80f),
            GNB_Mit_Advanced_NonBoss_MitigationThreshold = new("GNB_Mit_Advanced_NonBoss_MitigationThreshold", 20f);

        public static UserBool
            GNB_Mit_Advanced_Boss_Camouflage_Align = new("GNB_Mit_Advanced_Boss_Camouflage_Align", true),
            GNB_Mit_Advanced_Boss_Nebula_First = new("GNB_Mit_Advanced_Boss_Nebula_First", true);

        public static UserIntArray
            GNB_Mit_OneButton_Priorities = new("GNB_Mit_OneButton_Priorities");

        public static UserBoolArray
            GNB_Mit_Advanced_Boss_HeartOfStone_OnCD_Difficulty = new("GNB_Mit_Advanced_Boss_HeartOfStone_OnCD_Difficulty", [true, false]),
            GNB_Mit_Advanced_Boss_HeartOfStone_TankBuster_Difficulty = new("GNB_Mit_Advanced_Boss_HeartOfStone_TankBuster_Difficulty", [true, false]),
            GNB_Mit_Advanced_Boss_Rampart_Difficulty = new("GNB_Mit_Advanced_Boss_Rampart_Difficulty", [true, false]),
            GNB_Mit_Advanced_Boss_Nebula_Difficulty = new("GNB_Mit_Advanced_Boss_Nebula_Difficulty", [true, false]),
            GNB_Mit_Advanced_Boss_Camouflage_Difficulty = new("GNB_Mit_Advanced_Boss_Camouflage_Difficulty", [true, false]),
            GNB_Mit_Advanced_Boss_HeartOfLight_Difficulty = new("GNB_Mit_Advanced_Boss_HeartOfLight_Difficulty", [true, false]),
            GNB_Mit_Advanced_Boss_Reprisal_Difficulty = new("GNB_Mit_Advanced_Boss_Reprisal_Difficulty", [true, false]),
            GNB_Mit_OneButton_Superbolide_Difficulty = new("GNB_Mit_OneButton_Superbolide_Difficulty", [true, false]);

        public static readonly ContentCheck.ListSet
            GNB_Boss_Mit_DifficultyListSet = ContentCheck.ListSet.CasualVSHard,
            GNB_Mit_OneButton_Superbolide_DifficultyListSet = ContentCheck.ListSet.CasualVSHard;

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Combo Mitigations

                case Preset.GNB_ST_Simple:
                    DrawHorizontalRadioButton(GNB_ST_MitOptions, Generics.IncludeSimpleMitigations, Generics.EnablesTheUseOfMitigations, 0);
                    DrawHorizontalRadioButton(GNB_ST_MitOptions, Generics.ExcludeSimpleMitigations, Generics.DisablesTheUseOfMitigations, 1);
                    break;

                case Preset.GNB_AoE_Simple:
                    DrawHorizontalRadioButton(GNB_AoE_MitOptions, Generics.IncludeSimpleMitigations, Generics.EnablesTheUseOfMitigations, 0);
                    DrawHorizontalRadioButton(GNB_AoE_MitOptions, Generics.ExcludeSimpleMitigations, Generics.DisablesTheUseOfMitigations, 1);
                    break;

                case Preset.GNB_ST_Advanced:
                    DrawHorizontalRadioButton(GNB_ST_Advanced_MitOptions, Generics.IncludeAdvancedMitigations , Generics.EnablesTheUseOfMitigations, 0);
                    DrawHorizontalRadioButton(GNB_ST_Advanced_MitOptions, Generics.ExcludeAdvancedMitigations, Generics.DisablesTheUseOfMitigations, 1);
                    break;

                case Preset.GNB_AoE_Advanced:
                    DrawHorizontalRadioButton(GNB_AoE_Advanced_MitOptions, Generics.IncludeAdvancedMitigations , Generics.EnablesTheUseOfMitigations, 0);
                    DrawHorizontalRadioButton(GNB_AoE_Advanced_MitOptions, Generics.ExcludeAdvancedMitigations, Generics.DisablesTheUseOfMitigations, 1);
                    break;

                case Preset.GNB_Mit_Advanced_NonBoss:
                    DrawSliderFloat(0, 100, GNB_Mit_Advanced_NonBoss_MitigationThreshold, Generics.StopBelowAverageEnemyHP, decimals: 0);
                    break;

                case Preset.GNB_Mit_Advanced_NonBoss_SuperBolideEmergency:
                    DrawSliderInt(1, 100, GNB_Mit_Advanced_NonBoss_SuperBolide_Health, FormatAndCache(Generics.PlayerHPToUseAction, Superbolide.ActionName()));
                    break;

                case Preset.GNB_Mit_Advanced_Boss_Aurora:
                    DrawSliderInt(1, 100, GNB_Mit_Advanced_Boss_Aurora_Health, FormatAndCache(Generics.PlayerHPToUseAction, Aurora.ActionName()));
                    break;

                case Preset.GNB_Mit_Advanced_Boss_HeartOfStone_OnCD:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_HeartOfStone_OnCD_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    DrawSliderInt(1, 100, GNB_Mit_Advanced_Boss_HeartOfStone_Health, FormatAndCache(Generics.PlayerHPToUseAction, $"{HeartOfStone.ActionName()} / {HeartOfCorundum.ActionName()}"));
                    break;

                case Preset.GNB_Mit_Advanced_Boss_HeartOfStone_TankBuster:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_HeartOfStone_TankBuster_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    DrawSliderInt(0, 4, GNB_Mit_Advanced_Boss_HeartOfStoneDelay, FormatAndCache(Generics.DelayMit, HeartOfStone.ActionName()), sliderIncrement: 1);
                    break;

                case Preset.GNB_Mit_Advanced_Boss_Rampart:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_Rampart_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    break;

                case Preset.GNB_Mit_Advanced_Boss_Nebula:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_Nebula_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    DrawAdditionalBoolChoice(GNB_Mit_Advanced_Boss_Nebula_First, "Use Nebula First", "Uses Nebula before Rampart");
                    break;

                case Preset.GNB_Mit_Advanced_Boss_Camouflage:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_Camouflage_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    DrawSliderFloat(1, 100, GNB_Mit_Advanced_Boss_Camouflage_Threshold, "Will use Camouflage as extra tankbuster mitigation if under this HP%", decimals: 0);
                    DrawAdditionalBoolChoice(GNB_Mit_Advanced_Boss_Camouflage_Align, "Align Camouflage", "Tries to align Camouflage with Rampart for tankbusters.");
                    break;

                case Preset.GNB_Mit_Advanced_Boss_HeartOfLight:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_HeartOfLight_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    break;

                case Preset.GNB_Mit_Advanced_Boss_Reprisal:
                    DrawDifficultyMultiChoice(GNB_Mit_Advanced_Boss_Reprisal_Difficulty, GNB_Boss_Mit_DifficultyListSet,
                        Generics.SelectWhatKindOfContentThisOptionAppliesTo);
                    break;

                #endregion

                #region Single-Target

                case Preset.GNB_ST_Opener:
                    DrawHorizontalRadioButton(GNB_Opener_NM,
                        $"Normal {NoMercy.ActionName()}", $"Uses {NoMercy.ActionName()} normally in all openers", 0);
                    DrawHorizontalRadioButton(GNB_Opener_NM,
                        $"Early {NoMercy.ActionName()}", $"Uses {NoMercy.ActionName()} as soon as possible in all openers", 1);
                    DrawBossOnlyChoice(GNB_ST_Balance_Content);
                    break;

                case Preset.GNB_ST_NoMercy:
                    DrawSliderInt(0, 50, GNB_ST_NM_HPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(GNB_ST_NM_BossOption,
                        Generics.NonBosses, Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(GNB_ST_NM_BossOption,
                        Generics.AllEnemies, Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.GNB_ST_BurstStrike:
                    DrawHorizontalRadioButton(GNB_ST_Overcap_Choice,
                        "Include Overcap Protection", $"Includes {BurstStrike.ActionName()} to prevent overcapping on cartridges", 0);
                    DrawHorizontalRadioButton(GNB_ST_Overcap_Choice,
                        "Exclude Overcap Protection", $"Excludes {BurstStrike.ActionName()}, regardless of cartridge count", 1);
                    ImGui.Spacing();
                    DrawHorizontalRadioButton(GNB_ST_BurstStrike_Setup,
                        $"Precede {NoMercy.ActionName()}", $"Allow preceding {NoMercy.ActionName()} with {BurstStrike.ActionName()} for a buffed {Hypervelocity.ActionName()} (BS->NM->HV) - ONLY APPLIES TO 2.50", 0);
                    DrawHorizontalRadioButton(GNB_ST_BurstStrike_Setup,
                        $"Don't Precede {NoMercy.ActionName()}", $"Forbid preceding {NoMercy.ActionName()} with {BurstStrike.ActionName()}", 1);
                    break;

                case Preset.GNB_ST_GnashingFang:
                    DrawHorizontalRadioButton(GNB_ST_HoldGFCharge,
                        "Hold for Burst", $"Holds one charge of {GnashingFang.ActionName()} for at least one usage inside of {NoMercy.ActionName()}", 0);
                    DrawHorizontalRadioButton(GNB_ST_HoldGFCharge,
                        "Don't Hold for Burst", $"Does not hold any charges of {GnashingFang.ActionName()} for {NoMercy.ActionName()}", 1);
                    break;

                case Preset.GNB_ST_RangedUptime:
                    DrawHorizontalRadioButton(GNB_ST_HoldLightningShot,
                        $"Hold for {Continuation.ActionName()}", $"Holds {LightningShot.ActionName()} if you have any {Continuation.ActionName()} procs available to avoid loss", 1);
                    DrawHorizontalRadioButton(GNB_ST_HoldLightningShot,
                        $"Don't Hold for {Continuation.ActionName()}", $"Uses {LightningShot.ActionName()} regardless of any {Continuation.ActionName()} procs currently available", 0);
                    ImGui.Spacing();
                    DrawHorizontalRadioButton(GNB_ST_HoldLightningShotInBurst,
                        $"Hold under {NoMercy.ActionName()}", $"Holds {LightningShot.ActionName()} when under {NoMercy.ActionName()} buff", 1);
                    DrawHorizontalRadioButton(GNB_ST_HoldLightningShotInBurst,
                        $"Don't Hold under {NoMercy.ActionName()}", $"Uses {LightningShot.ActionName()} regardless of {NoMercy.ActionName()} buff", 0);

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

                case Preset.GNB_Mit_OneButton_Superbolide_Max:
                    DrawDifficultyMultiChoice(GNB_Mit_OneButton_Superbolide_Difficulty, GNB_Mit_OneButton_Superbolide_DifficultyListSet,
                        "Select what difficulties Superbolide should be used in:");
                    DrawSliderInt(1, 100, GNB_Mit_OneButton_Superbolide_Health, Generics.StopFriendlyHpPercent100, 200, SliderIncrements.Fives);
                    break;

                case Preset.GNB_Mit_OneButton_Corundum:
                    DrawSliderInt(1, 100, GNB_Mit_OneButton_Corundum_Health,
                        Generics.StopFriendlyHpPercent100,
                        sliderIncrement: SliderIncrements.Ones);
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 0,
                        "Heart of Corundum Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_Aurora:
                    DrawSliderInt(0, 1, GNB_Mit_OneButton_Aurora_Charges,
                        Generics.HowManyChargesToKeepReady);
                    DrawSliderInt(1, 100, GNB_Mit_OneButton_Aurora_Health,
                        Generics.StopFriendlyHpPercent100,
                        sliderIncrement: SliderIncrements.Ones);
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 1,
                        "Aurora Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_Camouflage:
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 2,
                        "Camouflage Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_Reprisal:
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 3,
                        "Reprisal Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_HeartOfLight:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(GNB_Mit_OneButton_HeartOfLight_PartyRequirement,
                        "Require party", "Will not use Heart of Light unless there are 2 or more party members.",
                        (int)PartyRequirement.Yes);
                    DrawHorizontalRadioButton(GNB_Mit_OneButton_HeartOfLight_PartyRequirement,
                        "Use Always", "Will not require a party for Heart of Light.",
                        (int)PartyRequirement.No);
                    ImGui.Unindent();
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 4,
                        "Heart of Light Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_Rampart:
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 5,
                        "Rampart Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_ArmsLength:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(GNB_Mit_OneButton_ArmsLength_Boss,
                        Generics.AllEnemies, "Will use Arm's Length regardless of the type of enemy.",
                        (int)BossAvoidance.Off, 125f);
                    DrawHorizontalRadioButton(
                        GNB_Mit_OneButton_ArmsLength_Boss,
                        "Avoid Bosses", "Will try not to use Arm's Length when in a boss fight.",
                        (int)BossAvoidance.On, 125f);
                    ImGui.Unindent();
                    DrawSliderInt(0, 5, GNB_Mit_OneButton_ArmsLength_EnemyCount,
                        "How many enemies should be nearby? (0 = No Requirement)");
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 6,
                        "Arm's Length Priority:");
                    break;

                case Preset.GNB_Mit_OneButton_Nebula:
                    DrawPriorityInput(GNB_Mit_OneButton_Priorities, NumMitigationOptions, 7,
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

                case Preset.GNB_FC_DoubleDown:
                    DrawHorizontalRadioButton(GNB_FC_DoubleDown_NMOnly,
                        "Hold for Burst", $"Holds {DoubleDown.ActionName()} until buffed by {NoMercy.ActionName()}", 0);
                    DrawHorizontalRadioButton(GNB_FC_DoubleDown_NMOnly,
                        "Don't Hold for Burst", $"Uses {DoubleDown.ActionName()} regardless of {NoMercy.ActionName()} buff", 1);
                    break;

                case Preset.GNB_BS_DoubleDown:
                    DrawHorizontalRadioButton(GNB_BS_DoubleDown_NMOnly,
                        "Hold for Burst", $"Holds {DoubleDown.ActionName()} until buffed by {NoMercy.ActionName()}", 0);
                    DrawHorizontalRadioButton(GNB_BS_DoubleDown_NMOnly,
                        "Don't Hold for Burst", $"Uses {DoubleDown.ActionName()} regardless of {NoMercy.ActionName()} buff", 1);
                    break;

                case Preset.GNB_BS_Continuation:
                    DrawHorizontalRadioButton(GNB_BS_Continuation_Procs,
                        "All Procs", $"Uses all {Continuation.ActionName()} procs available as soon as possible", 0);
                    DrawHorizontalRadioButton(GNB_BS_Continuation_Procs,
                        "Only Hypervelocity", $"Only uses {Hypervelocity.ActionName()} regardless of other {Continuation.ActionName()} procs currently available", 1);
                    break;

                case Preset.GNB_FC_Continuation:
                    DrawHorizontalRadioButton(GNB_FC_Continuation_Procs,
                        "All Procs", $"Uses all {Continuation.ActionName()} procs available as soon as possible", 0);
                    DrawHorizontalRadioButton(GNB_FC_Continuation_Procs,
                        "Only Fated Brand", $"Only uses {FatedBrand.ActionName()} regardless of other {Continuation.ActionName()} procs currently available", 1);
                    break;
                    #endregion
            }
        }
    }
}
