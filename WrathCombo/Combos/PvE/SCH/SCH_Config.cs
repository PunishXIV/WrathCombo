using Dalamud.Interface.Colors;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using WrathCombo.Extensions;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;

internal partial class SCH
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region DPS
                case Preset.SCH_ST_ADV_DPS_Balance_Opener:
                    DrawBossOnlyChoice(SCH_ST_DPS_OpenerContent);
                    DrawOpenerPotionChoice(SCH_Opener_Potion);
                    ImGuiEx.TextUnderlined("Select Opener");
                    ImGui.Spacing();
                    DrawRadioButton(SCH_ST_DPS_OpenerOption, "Dissipation First", "Uses Dissipation first, then Aetherflow", 0, descriptionAsTooltip: true);
                    DrawRadioButton(SCH_ST_DPS_OpenerOption, "Aetherflow First", "Uses Aetherflow first, then Dissipation", 1, descriptionAsTooltip: true);
                    break;

                case Preset.SCH_ST_ADV_DPS:
                    DrawHorizontalRadioButton(SCH_ST_DPS_Adv_Actions, "On Ruin/Broils", "Apply options to Ruin and all Broils.", 0,
                        descriptionColor: ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(SCH_ST_DPS_Adv_Actions, "On Bio/Bio II/Biolysis", "Apply options to Bio and Biolysis.", 1,
                        descriptionColor: ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(SCH_ST_DPS_Adv_Actions, "On Broil II", "Apply options to Broil II.", 2,
                        descriptionColor: ImGuiColors.DalamudWhite);
                    break;

                case Preset.SCH_ST_ADV_DPS_Lucid:
                    DrawSliderInt(4000, 9500, SCH_ST_DPS_LucidOption, Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SCH_ST_ADV_DPS_Bio:
                    DrawSliderInt(0, 100, SCH_ST_DPS_BioBossOption, Generics.BossOnlyHpPercent);
                    DrawSliderInt(0, 100, SCH_ST_DPS_BioBossAddsOption, Generics.BossEncounterNonBossHpPercent);
                    DrawSliderInt(0, 100, SCH_ST_DPS_BioTrashOption, Generics.NonBossHpPercent);
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 4, SCH_ST_DPS_BioUptime_Threshold, Generics.DoTSecondsRemainingZeroDisable, digits: 1);
                    ImGui.Unindent();
                    DrawAdditionalBoolChoice(SCH_ST_ADV_DPS_Bio_TwoTarget, Generics.TwoTargetDotting, Generics.TwoTargetDottingDescription);
                    break;

                case Preset.SCH_ST_ADV_DPS_ChainStrat:

                    DrawSliderInt(0, 100, SCH_ST_DPS_ChainStratagemOption, Generics.StopEnemyHpPercent);

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(SCH_ST_DPS_ChainStratagemSubOption,
                        Generics.NonBosses, "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(SCH_ST_DPS_ChainStratagemSubOption,
                        Generics.AllEnemies, Generics.HPCheckAllEnemies, 1);

                    ImGui.Unindent();

                    break;

                case Preset.SCH_ST_ADV_DPS_EnergyDrain:
                    DrawSliderInt(0, 60, SCH_ST_DPS_EnergyDrain, "Aetherflow remaining cooldown");

                    DrawAdditionalBoolChoice(SCH_ST_DPS_EnergyDrain_Burst,
                        "Energy Drain Burst", "Holds Energy Drain when Chain Stratagem is ready or has less than 10 seconds cooldown remaining.");
                    break;

                case Preset.SCH_AoE_ADV_DPS_Lucid:
                    DrawSliderInt(4000, 9500, SCH_AoE_DPS_LucidOption, Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SCH_AoE_ADV_DPS_ChainStrat:
                    DrawAdditionalBoolChoice(SCH_AoE_DPS_ChainStratagemBanefulOption,
                        "Baneful Only", "Will only use Chain Strategem when high enough level to use Baneful Impaction");

                    DrawSliderInt(0, 100, SCH_AoE_DPS_ChainStratagemOption, Generics.StopEnemyHpPercent);

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(SCH_AoE_DPS_ChainStratagemSubOption,
                        Generics.NonBosses, "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(SCH_AoE_DPS_ChainStratagemSubOption,
                        Generics.AllEnemies, Generics.HPCheckAllEnemies, 1);

                    ImGui.Unindent();

                    break;

                case Preset.SCH_AoE_ADV_DPS_EnergyDrain:
                    DrawSliderInt(0, 60, SCH_AoE_DPS_EnergyDrain, "Aetherflow remaining cooldown");

                    DrawAdditionalBoolChoice(SCH_AoE_DPS_EnergyDrain_Burst,
                        "Energy Drain Burst", "Holds Energy Drain when Chain Stratagem is ready or has less than 10 seconds cooldown remaining.");
                    break;

                case Preset.SCH_AoE_ADV_DPS_DoT:
                    DrawSliderInt(0, 100, SCH_AoE_ADV_DPS_DoT_HPThreshold, "Target HP% to stop using (0 = Use Always, 100 = Never)");
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 5, SCH_AoE_ADV_DPS_DoT_Reapply, Generics.StopSeconds, digits: 1);
                    ImGui.Unindent();
                    DrawSliderInt(0, 10, SCH_AoE_ADV_DPS_DoT_MaxTargets, Generics.MaxTargetsMultiDot);
                    break;
                #endregion

                #region ST Healing
                case Preset.SCH_ST_Heal:

                    ImGui.Indent();
                    DrawAdditionalBoolChoice(SCH_ST_Heal_IncludeShields, "Advanced Option: Include Shields in HP Percent Sliders", "");
                    ImGui.Unindent();

                    break;

                case Preset.SCH_ST_Heal_Lucid:
                    DrawSliderInt(4000, 9500, SCH_ST_Heal_LucidOption, Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SCH_ST_Heal_Lustrate:
                    DrawSliderInt(0, 100, SCH_ST_Heal_LustrateOption, Generics.StopFriendlyHpPercent100);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 0, FormatAndCache(Generics.Action_Priority, Lustrate.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Excogitation:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ExcogitationOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ExcogitationBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ExcogitationTankOption, Generics.TanksOnly, Generics.WillOnlyUseOnTanks);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 1, FormatAndCache(Generics.Action_Priority, Excogitation.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Protraction:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ProtractionOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ProtractionBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ProtractionTankOption, Generics.TanksOnly, Generics.WillOnlyUseOnTanks);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 2, FormatAndCache(Generics.Action_Priority, Protraction.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Aetherpact:
                    DrawSliderInt(0, 100, SCH_ST_Heal_AetherpactOption, Generics.StopFriendlyHpPercent100);
                    DrawSliderInt(0, 100, SCH_ST_Heal_AetherpactDissolveOption, "Stop using when above HP %.");
                    DrawSliderInt(10, 100, SCH_ST_Heal_AetherpactFairyGauge, "Minimal Fairy Gauge to start using Aetherpact", sliderIncrement: Tens);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 3, FormatAndCache(Generics.Action_Priority, Aetherpact.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_WhisperingDawn:
                    DrawSliderInt(0, 100, SCH_ST_Heal_WhisperingDawnOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_WhisperingDawnBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 5, FormatAndCache(Generics.Action_Priority, WhisperingDawn.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_FeyIllumination:
                    DrawSliderInt(0, 100, SCH_ST_Heal_FeyIlluminationOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_FeyIlluminationBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 6, FormatAndCache(Generics.Action_Priority, FeyIllumination.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_FeyBlessing:
                    DrawSliderInt(0, 100, SCH_ST_Heal_FeyBlessingOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_FeyBlessingBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 7, FormatAndCache(Generics.Action_Priority, FeyBlessing.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Seraphism:
                    DrawSliderInt(0, 100, SCH_ST_Heal_SeraphismOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_SeraphismBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 8, FormatAndCache(Generics.Action_Priority, Seraphism.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Expedient:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ExpedientOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ExpedientBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 9, FormatAndCache(Generics.Action_Priority, Expedient.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_SummonSeraph:
                    DrawSliderInt(0, 100, SCH_ST_Heal_SummonSeraphOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_SummonSeraphBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 10, FormatAndCache(Generics.Action_Priority, SummonSeraph.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Consolation:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ConsolationOption, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ConsolationBossOption, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 11, FormatAndCache(Generics.Action_Priority, Consolation.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Adloquium:
                    DrawSliderInt(0, 100, SCH_ST_Heal_AdloquiumOption, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts,
                        FormatAndCache(Generics.Job0ShieldCheck, Job.SCH.Name()),
                        FormatAndCache(Generics.Job0ShieldCheckDesc, Job.SCH.Name()), 3, 0
                    );
                    DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts,
                        FormatAndCache(Generics.Job0ShieldCheck, Job.SGE.Name()),
                        FormatAndCache(Generics.Job0ShieldCheckDesc, Job.SGE.Name()), 3, 1
                    );
                    DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts, "Emergency Tactics", "Will use Emergency tactics before Adloquim when below set threshold", 3, 2);

                    if (SCH_ST_Heal_AldoquimOpts[2])
                    {
                        ImGui.Indent();
                        DrawSliderInt(0, 100, SCH_ST_Heal_AdloquiumOption_Emergency, "Start using Emergency Tactics when below HP %.");
                        ImGui.Unindent();
                    }

                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 4, FormatAndCache(Generics.Action_Priority, Adloquium.ActionName()));
                    break;

                case Preset.SCH_ST_Heal_Esuna:
                    DrawSliderInt(0, 100, SCH_ST_Heal_EsunaOption, Generics.StopFriendlyHpPercentZero);
                    break;

                #endregion

                #region AoE Healing
                case Preset.SCH_AoE_Heal_Lucid:
                    DrawSliderInt(4000, 9500, SCH_AoE_Heal_LucidOption, Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SCH_AoE_Heal:
                    ImGui.TextUnformatted("Note: Succor will always be available.");
                    ImGui.TextUnformatted("These options are to provide optional priority to Succor or to set up Emergency tactics option.");
                    DrawSliderInt(0, 100, SCH_AoE_Heal_SuccorShieldOption, "Shield Check: Will use when less than set percentage of party have shields.", sliderIncrement: 25);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 7, FormatAndCache(Generics.Action_Priority, Succor.ActionName()));
                    DrawHorizontalMultiChoice(SCH_AoE_Heal_Succor_Options, EmergencyTactics.ActionName(), "If more than the set percentage of the party has shields, will use Emergency Tactics before Succor", 2, 0);
                    DrawHorizontalMultiChoice(SCH_AoE_Heal_Succor_Options, Recitation.ActionName(), "Will use Recitation to buff Succor", 2, 1);
                    break;

                case Preset.SCH_AoE_Heal_WhisperingDawn:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_WhisperingDawnOption, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 0, FormatAndCache(Generics.Action_Priority, WhisperingDawn.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_FeyIllumination:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_FeyIlluminationOption, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 1, FormatAndCache(Generics.Action_Priority, FeyIllumination.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_FeyBlessing:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_FeyBlessingOption, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 2, FormatAndCache(Generics.Action_Priority, FeyBlessing.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_Consolation:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_ConsolationOption, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 3, FormatAndCache(Generics.Action_Priority, Consolation.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_SummonSeraph:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_SummonSeraph, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 6, FormatAndCache(Generics.Action_Priority, SummonSeraph.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_Seraphism:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_SeraphismOption, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 4, FormatAndCache(Generics.Action_Priority, Seraphism.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_Indomitability:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_IndomitabilityOption, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(SCH_AoE_Heal_Indomitability_Recitation, "Recitation Option", "Will use Recitation to buff Indomitability.");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 5, FormatAndCache(Generics.Action_Priority, Indomitability.ActionName()));
                    break;

                case Preset.SCH_AoE_Heal_Aetherflow:
                    DrawAdditionalBoolChoice(SCH_AoE_Heal_Aetherflow_Indomitability,
                        "Indomitability Ready Only Option", "Only uses Aetherflow if Indomitability is ready to use.");
                    break;

                case Preset.SCH_AoE_Heal_Dissipation:
                    DrawAdditionalBoolChoice(SCH_AoE_Heal_Dissipation_Indomitability,
                        "Indomitability Ready Only Option", "Only uses Dissipation if Indomitability is ready to use.");
                    break;

                #endregion

                #region Standalones
                case Preset.SCH_Dissipation:
                    DrawAdditionalBoolChoice(SCH_Dissipation_WastePrevention, Generics.WastePrevention, 
                        FormatAndCache(Generics.SavageBladeWaste, Dissipation.ActionName(), All.SavageBlade.ActionName()));
                    break;
                    
                case Preset.SCH_Aetherflow:
                    DrawRadioButton(SCH_Aetherflow_Display, "Show Aetherflow On Energy Drain Only", "", 0);
                    DrawRadioButton(SCH_Aetherflow_Display, "Show Aetherflow On All Aetherflow Skills", "", 1);
                    break;

                case Preset.SCH_Aetherflow_Recite:
                    DrawAdditionalBoolChoice(SCH_Aetherflow_Recite_Excog, "On Excogitation", "", isConditionalChoice: true);
                    if (SCH_Aetherflow_Recite_Excog)
                    {
                        ImGui.Indent();
                        ImGui.Spacing();
                        DrawRadioButton(SCH_Aetherflow_Recite_ExcogMode, "Only when out of Aetherflow Stacks", "", 0);
                        DrawRadioButton(SCH_Aetherflow_Recite_ExcogMode, "Always when available", "", 1);
                        ImGui.Unindent();
                    }

                    DrawAdditionalBoolChoice(SCH_Aetherflow_Recite_Indom, "On Indominability", "", isConditionalChoice: true);
                    if (SCH_Aetherflow_Recite_Indom)
                    {
                        ImGui.Indent();
                        ImGui.Spacing();
                        DrawRadioButton(SCH_Aetherflow_Recite_IndomMode, "Only when out of Aetherflow Stacks", "", 0);
                        DrawRadioButton(SCH_Aetherflow_Recite_IndomMode, "Always when available", "", 1);
                        ImGui.Unindent();
                    }
                    break;

                case Preset.SCH_Recitation:
                    DrawRadioButton(SCH_Recitation_Mode, Adloquium.ActionName(), "", 0);
                    DrawRadioButton(SCH_Recitation_Mode, Succor.ActionName(), "", 1);
                    DrawRadioButton(SCH_Recitation_Mode, Indomitability.ActionName(), "", 2);
                    DrawRadioButton(SCH_Recitation_Mode, Excogitation.ActionName(), "", 3);
                    break;

                case Preset.SCH_Raidwide_Succor:
                    DrawAdditionalBoolChoice(SCH_Raidwide_Succor_Recitation, "Recitation Option", "Use Recitation to buff before the Raidwide Succor.");
                    break;

                case Preset.SCH_Retarget_SacredSoil:
                    DrawHorizontalMultiChoice(SCH_Retarget_SacredSoilOptions, Generics.EnemyHardTarget, "Will place under hard target if it is an Enemy.", 2, 0);
                    DrawHorizontalMultiChoice(SCH_Retarget_SacredSoilOptions, Generics.AllyHardTarget, "Will place under hard target if it is an Ally.", 2, 1);
                    break;

                case Preset.SCH_Mit_ST:
                    DrawHorizontalMultiChoice(SCH_Mit_STOptions, Recitation.ActionName(), "Will Recitation before Adloquium if available.", 3, 0);
                    DrawHorizontalMultiChoice(SCH_Mit_STOptions, DeploymentTactics.ActionName(), "Will spread Adloquium crit shield if available.", 3, 1);
                    DrawHorizontalMultiChoice(SCH_Mit_STOptions, Excogitation.ActionName(), "Will use Excogitation if available.", 3, 2);
                    break;

                case Preset.SCH_Mit_AoE:
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, FeyIllumination.ActionName(), "Will activate Fey Illumination before Succor", 4, 0);
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, "Crit Adloquium Deployment", "Will Recitation into Adloquium and Deployment tactics in place of Succor" +
                        "\nThis will be targeted at yourself for simplicity and reliability.", 4, 1);
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, Expedient.ActionName(), "Will use Expedient if available.", 4, 2);
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, "Summon Seraph Consolation", "Will summon Seraph if available and use Consolation for more shield.", 4, 3);
                    break;

                    #endregion
            }
        }

        #region Options

        #region DPS

        internal static UserInt
            SCH_ST_DPS_LucidOption = new("SCH_ST_DPS_LucidOption", 6500),
            SCH_AoE_DPS_LucidOption = new("SCH_AoE_LucidOption", 6500),
            SCH_ST_DPS_OpenerOption = new("SCH_ST_DPS_OpenerOption"),
            SCH_ST_DPS_OpenerContent = new("SCH_ST_DPS_OpenerContent", 1),
            SCH_ST_DPS_ChainStratagemOption = new("SCH_ST_DPS_ChainStratagemOption", 10),
            SCH_ST_DPS_BioBossOption = new("SCH_ST_DPS_BioBossOption", 0),
            SCH_ST_DPS_BioBossAddsOption = new("SCH_ST_DPS_BioBossAddsOption", 100),
            SCH_ST_DPS_BioTrashOption = new("SCH_ST_DPS_BioTrashOption", 50),
            SCH_AoE_DPS_ChainStratagemOption = new("SCH_AoE_DPS_ChainStratagemOption", 10),
            SCH_ST_DPS_EnergyDrain = new("SCH_ST_DPS_EnergyDrain", 3),
            SCH_ST_DPS_ChainStratagemSubOption = new("SCH_ST_DPS_ChainStratagemSubOption", 1),
            SCH_AoE_DPS_EnergyDrain = new("SCH_AoE_DPS_EnergyDrain", 3),
            SCH_AoE_DPS_ChainStratagemSubOption = new("SCH_AoE_DPS_ChainStratagemSubOption", 1),
            SCH_AoE_ADV_DPS_DoT_HPThreshold = new("SCH_AoE_ADV_DPS_DoT_HPThreshold", 30),
            SCH_AoE_ADV_DPS_DoT_MaxTargets = new("SCH_AoE_ADV_DPS_DoT_MaxTargets", 4),
            SCH_ST_DPS_Adv_Actions = new("SCH_ST_DPS_Adv_Actions");



        internal static UserBool
            SCH_Opener_Potion = new("SCH_Opener_Potion"),
            SCH_ST_ADV_DPS_Bio_TwoTarget = new("SCH_ST_ADV_DPS_Bio_TwoTarget"),
            SCH_ST_DPS_EnergyDrain_Burst = new("SCH_ST_DPS_EnergyDrain_Burst"),
            SCH_AoE_DPS_EnergyDrain_Burst = new("SCH_AoE_DPS_EnergyDrain_Burst"),
            SCH_AoE_DPS_ChainStratagemBanefulOption = new("SCH_AoE_DPS_ChainStratagemBanefulOption"),
            SCH_AoE_Heal_Aetherflow_Indomitability = new("SCH_AoE_Heal_Aetherflow_Indomitability"),
            SCH_AoE_Heal_Dissipation_Indomitability = new("SCH_AoE_Heal_Dissipation_Indomitability"),
            SCH_Raidwide_Succor_Recitation = new("SCH_Raidwide_Succor_Recitation");


        internal static UserFloat
            SCH_ST_DPS_BioUptime_Threshold = new("SCH_ST_DPS_BioUptime_Threshold", 3.0f),
            SCH_AoE_ADV_DPS_DoT_Reapply = new("SCH_AoE_ADV_DPS_DoT_Reapply", 0);



        #endregion

        #region Healing

        public static UserInt

            SCH_AoE_Heal_LucidOption = new("SCH_AoE_Heal_LucidOption", 8000),
            SCH_AoE_Heal_SuccorShieldOption = new("SCH_AoE_Heal_SuccorShieldCount", 50),
            SCH_AoE_Heal_WhisperingDawnOption = new("SCH_AoE_Heal_WhisperingDawnOption", 70),
            SCH_AoE_Heal_FeyIlluminationOption = new("SCH_AoE_Heal_FeyIlluminationOption", 70),
            SCH_AoE_Heal_ConsolationOption = new("SCH_AoE_Heal_ConsolationOption", 70),
            SCH_AoE_Heal_FeyBlessingOption = new("SCH_AoE_Heal_FeyBlessingOption", 70),
            SCH_AoE_Heal_SeraphismOption = new("SCH_AoE_Heal_SeraphismOption", 70),
            SCH_AoE_Heal_IndomitabilityOption = new("SCH_AoE_Heal_IndomitabilityOption", 70),
            SCH_AoE_Heal_SummonSeraph = new("SCH_AoE_Heal_SummonSeraph", 70),
            SCH_ST_Heal_LucidOption = new("SCH_ST_Heal_LucidOption", 8000),
            SCH_ST_Heal_AdloquiumOption = new("SCH_ST_Heal_AdloquiumOption", 70),
            SCH_ST_Heal_AdloquiumOption_Emergency = new("SCH_ST_Heal_AdloquiumOption_Emergency", 30),
            SCH_ST_Heal_LustrateOption = new("SCH_ST_Heal_LustrateOption", 70),
            SCH_ST_Heal_ExcogitationOption = new("SCH_ST_Heal_ExcogitationOption", 70),
            SCH_ST_Heal_ProtractionOption = new("SCH_ST_Heal_ProtractionOption", 70),
            SCH_ST_Heal_AetherpactOption = new("SCH_ST_Heal_AetherpactOption", 70),
            SCH_ST_Heal_AetherpactDissolveOption = new("SCH_ST_Heal_AetherpactDissolveOption", 90),
            SCH_ST_Heal_AetherpactFairyGauge = new("SCH_ST_Heal_AetherpactFairyGauge", 50),
            SCH_ST_Heal_WhisperingDawnOption = new("SCH_ST_Heal_WhisperingDawnOption", 70),
            SCH_ST_Heal_FeyIlluminationOption = new("SCH_ST_Heal_FeyIlluminationOption", 70),
            SCH_ST_Heal_FeyBlessingOption = new("SCH_ST_Heal_FeyBlessingOption", 70),
            SCH_ST_Heal_SeraphismOption = new("SCH_ST_Heal_SeraphismOption", 70),
            SCH_ST_Heal_ExpedientOption = new("SCH_ST_Heal_ExpedientOption", 70),
            SCH_ST_Heal_SummonSeraphOption = new("SCH_ST_Heal_SummonSeraphOption", 70),
            SCH_ST_Heal_ConsolationOption = new("SCH_ST_Heal_ConsolationOption", 70),
            SCH_ST_Heal_EsunaOption = new("SCH_ST_Heal_EsunaOption", 40);
        public static UserIntArray
            SCH_ST_Heals_Priority = new("SCH_ST_Heals_Priority", [11,10,9,8,12,7,6,5,4,1,2,3]),
            SCH_AoE_Heals_Priority = new("SCH_AoE_Heals_Priority", [3,2,4,1,6,7,5,8]);

        public static UserBool
            SCH_ST_Heal_IncludeShields = new("SCH_ST_Heal_IncludeShields"),
            SCH_ST_Heal_WhisperingDawnBossOption = new("SCH_ST_Heal_WhisperingDawnBossOption", true),
            SCH_ST_Heal_FeyIlluminationBossOption = new("SCH_ST_Heal_FeyIlluminationBossOption", true),
            SCH_ST_Heal_FeyBlessingBossOption = new("SCH_ST_Heal_FeyBlessingBossOption", true),
            SCH_ST_Heal_ExcogitationBossOption = new("SCH_ST_Heal_ExcogitationBossOption"),
            SCH_ST_Heal_ExcogitationTankOption = new("SCH_ST_Heal_ExcogitationTankOption", true),
            SCH_ST_Heal_ProtractionBossOption = new("SCH_ST_Heal_ProtractionBossOption"),
            SCH_ST_Heal_ProtractionTankOption = new("SCH_ST_Heal_ProtractionTankOption", true),
            SCH_ST_Heal_SeraphismBossOption = new("SCH_ST_Heal_SeraphismBossOption", true),
            SCH_ST_Heal_ExpedientBossOption = new("SCH_ST_Heal_ExpedientBossOption", true),
            SCH_ST_Heal_SummonSeraphBossOption = new("SCH_ST_Heal_SummonSeraphBossOption", true),
            SCH_ST_Heal_ConsolationBossOption = new("SCH_ST_Heal_ConsolationBossOption", true),
            SCH_AoE_Heal_Indomitability_Recitation = new("SCH_AoE_Heal_Indomitability_Recitation");

        public static UserBoolArray
            SCH_ST_Heal_AldoquimOpts = new("SCH_ST_Heal_AldoquimOpts", [true, true, true]),
            SCH_AoE_Heal_Succor_Options = new("SCH_AoE_Heal_Succor_Options", [true, false]);

        #endregion

        #region Standalones

        internal static UserBool
            SCH_Dissipation_WastePrevention = new("SCH_Dissipation_WastePrevention"),
            SCH_Aetherflow_Recite_Indom = new("SCH_Aetherflow_Recite_Indom"),
            SCH_Aetherflow_Recite_Excog = new("SCH_Aetherflow_Recite_Excog");
        internal static UserInt
            SCH_Aetherflow_Display = new("SCH_Aetherflow_Display"),
            SCH_Aetherflow_Recite_ExcogMode = new("SCH_Aetherflow_Recite_ExcogMode"),
            SCH_Aetherflow_Recite_IndomMode = new("SCH_Aetherflow_Recite_IndomMode"),
            SCH_Recitation_Mode = new("SCH_Recitation_Mode");

        internal static UserBoolArray
            SCH_Retarget_SacredSoilOptions = new("SCH_Retarget_SacredSoilOptions"),
            SCH_Mit_STOptions = new("SCH_Mit_STOptions"),
            SCH_Mit_AoEOptions = new("SCH_Mit_AoEOptions");

        #endregion

        #endregion

    }
}
