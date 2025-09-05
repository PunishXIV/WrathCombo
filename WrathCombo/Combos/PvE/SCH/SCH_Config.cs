using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
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
                    DrawHorizontalRadioButton(SCH_ST_DPS_OpenerOption, "Dissipation First", "Uses Dissipation first, then Aetherflow", 0);
                    DrawHorizontalRadioButton(SCH_ST_DPS_OpenerOption, "Aetherflow First", "Uses Aetherflow first, then Dissipation", 1);
                    DrawBossOnlyChoice(SCH_ST_DPS_OpenerContent);
                    break;

                case Preset.SCH_ST_ADV_DPS:
                    DrawHorizontalRadioButton(SCH_ST_DPS_Adv_Actions, "On Ruin/Broils", "Apply options to Ruin and all Broils.", 0,
                        descriptionColor: ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(SCH_ST_DPS_Adv_Actions, "On Bio/Bio II/Biolysis", "Apply options to Bio and Biolysis.", 1,
                        descriptionColor: ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(SCH_ST_DPS_Adv_Actions, "On Ruin II", "Apply options to Ruin II.", 2,
                        descriptionColor: ImGuiColors.DalamudWhite);
                    break;

                case Preset.SCH_ST_ADV_DPS_Lucid:
                    DrawSliderInt(4000, 9500, SCH_ST_DPS_LucidOption, "MP Threshold", 150, Hundreds);
                    break;

                case Preset.SCH_ST_ADV_DPS_Bio:

                    DrawSliderInt(0, 50, SCH_DPS_BioOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(SCH_DPS_BioSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(SCH_DPS_BioSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    DrawRoundedSliderFloat(0, 4, SCH_DPS_BioUptime_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);

                    ImGui.Unindent();

                    break;

                case Preset.SCH_ST_ADV_DPS_ChainStrat:

                    DrawSliderInt(0, 100, SCH_ST_DPS_ChainStratagemOption, "Stop using at Enemy HP%. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(SCH_ST_DPS_ChainStratagemSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(SCH_ST_DPS_ChainStratagemSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();

                    break;

                case Preset.SCH_ST_ADV_DPS_EnergyDrain:
                    DrawSliderInt(0, 60, SCH_ST_DPS_EnergyDrain, "Aetherflow remaining cooldown");

                    DrawAdditionalBoolChoice(SCH_ST_DPS_EnergyDrain_Burst,
                        "Energy Drain Burst", "Holds Energy Drain when Chain Stratagem is ready or has less than 10 seconds cooldown remaining.");
                    break;

                case Preset.SCH_AoE_ADV_DPS_Lucid:
                    DrawSliderInt(4000, 9500, SCH_AoE_DPS_LucidOption, "MP Threshold", 150, Hundreds);
                    break;

                case Preset.SCH_AoE_ADV_DPS_ChainStrat:
                    DrawAdditionalBoolChoice(SCH_AoE_DPS_ChainStratagemBanefulOption,
                        "Baneful Only", "Will only use Chain Strategem when high enough level to use Baneful Impaction");

                    DrawSliderInt(0, 100, SCH_AoE_DPS_ChainStratagemOption, "Stop using at Enemy HP%. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(SCH_AoE_DPS_ChainStratagemSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(SCH_AoE_DPS_ChainStratagemSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

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
                    DrawRoundedSliderFloat(0, 5, SCH_AoE_ADV_DPS_DoT_Reapply, "Seconds remaining before reapplying (0 = Do not reapply early)", digits: 1);
                    ImGui.Unindent();
                    DrawSliderInt(0, 10, SCH_AoE_ADV_DPS_DoT_MaxTargets, "Maximum number of targets to employ multi-dotting ");
                    break;
                #endregion
                
                #region ST Healing
                case Preset.SCH_ST_Heal:

                    ImGui.Indent();
                    DrawAdditionalBoolChoice(SCH_ST_Heal_IncludeShields, "Advanced Option: Include Shields in HP Percent Sliders", "");
                    ImGui.Unindent();

                    break;

                case Preset.SCH_ST_Heal_Lucid:
                    DrawSliderInt(4000, 9500, SCH_ST_Heal_LucidOption, "MP Threshold", 150, Hundreds);
                    break;

                case Preset.SCH_ST_Heal_Lustrate:
                    DrawSliderInt(0, 100, SCH_ST_Heal_LustrateOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 0, $"{Lustrate.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_Excogitation:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ExcogitationOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ExcogitationBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ExcogitationTankOption, "Only on Tanks", "Will only use on a Tank.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 1, $"{Excogitation.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_Protraction:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ProtractionOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ProtractionBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ProtractionTankOption, "Only on Tanks", "Will only use on a Tank.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 2, $"{Protraction.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_Aetherpact:
                    DrawSliderInt(0, 100, SCH_ST_Heal_AetherpactOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawSliderInt(0, 100, SCH_ST_Heal_AetherpactDissolveOption, "Stop using when above HP %.");
                    DrawSliderInt(10, 100, SCH_ST_Heal_AetherpactFairyGauge, "Minimal Fairy Gauge to start using Aetherpact", sliderIncrement: Tens);
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 3, $"{Aetherpact.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_WhisperingDawn:
                    DrawSliderInt(0, 100, SCH_ST_Heal_WhisperingDawnOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_WhisperingDawnBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 5, $"{WhisperingDawn.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_FeyIllumination:
                    DrawSliderInt(0, 100, SCH_ST_Heal_FeyIlluminationOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_FeyIlluminationBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 6, $"{FeyIllumination.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_FeyBlessing:
                    DrawSliderInt(0, 100, SCH_ST_Heal_FeyBlessingOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_FeyBlessingBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 7, $"{FeyBlessing.ActionName()} Priority: ");
                    break;
                
                case Preset.SCH_ST_Heal_Seraphism:
                    DrawSliderInt(0, 100, SCH_ST_Heal_SeraphismOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_SeraphismBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 8, $"{Seraphism.ActionName()} Priority: ");
                    break;
                
                case Preset.SCH_ST_Heal_Expedient:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ExpedientOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ExpedientBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 9, $"{Expedient.ActionName()} Priority: ");
                    break;
                
                case Preset.SCH_ST_Heal_SummonSeraph:
                    DrawSliderInt(0, 100, SCH_ST_Heal_SummonSeraphOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_SummonSeraphBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 10, $"{SummonSeraph.ActionName()} Priority: ");
                    break;
                
                case Preset.SCH_ST_Heal_Consolation:
                    DrawSliderInt(0, 100, SCH_ST_Heal_ConsolationOption, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_ST_Heal_ConsolationBossOption, "Not on Bosses", "Will not use on ST in Boss encounters.");
                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 11, $"{Consolation.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_Adloquium:
                    DrawSliderInt(0, 100, SCH_ST_Heal_AdloquiumOption, "Start using when below HP %. Set to 100 to disable this check.");
                    DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts, "Scholar Shield Check", "Enable to not override an existing Scholar's shield.", 3, 0);
                    DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts, "Sage Shield Check", "Enable to not override an existing Sage's shield.", 3, 1);
                    DrawHorizontalMultiChoice(SCH_ST_Heal_AldoquimOpts, "Emergency Tactics", "Will use Emergency tactics before Adloquim when below set threshold", 3, 2);

                    if (SCH_ST_Heal_AldoquimOpts[2])
                    {
                        ImGui.Indent();
                        DrawSliderInt(0, 100, SCH_ST_Heal_AdloquiumOption_Emergency, "Start using Emergency Tactics when below HP %.");
                        ImGui.Unindent();
                    }

                    DrawPriorityInput(SCH_ST_Heals_Priority, 12, 4, $"{Adloquium.ActionName()} Priority: ");
                    break;

                case Preset.SCH_ST_Heal_Esuna:
                    DrawSliderInt(0, 100, SCH_ST_Heal_EsunaOption, "Stop using when below HP %. Set to Zero to disable this check");
                    break;
                
                #endregion

                #region AoE Healing
                case Preset.SCH_AoE_Heal_Lucid:
                    DrawSliderInt(4000, 9500, SCH_AoE_Heal_LucidOption, "MP Threshold", 150, Hundreds);
                    break;

                case Preset.SCH_AoE_Heal:
                    ImGui.TextUnformatted("Note: Succor will always be available.");
                    ImGui.TextUnformatted("These options are to provide optional priority to Succor or to set up Emergency tactics option.");
                    DrawSliderInt(0, 100, SCH_AoE_Heal_SuccorShieldOption, "Shield Check: Will use when less than set percentage of party have shields.", sliderIncrement: 25);
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 7, $"{Succor.ActionName()} Priority: ");
                    DrawHorizontalMultiChoice(SCH_AoE_Heal_Succor_Options, "Emergency Tactics", "If more than the set percentage of the party has shields, will use Emergency Tactics before Succor", 2, 0);
                    DrawHorizontalMultiChoice(SCH_AoE_Heal_Succor_Options, "Recitation", "Will use Recitation to buff Succor", 2, 1);
                    break;

                case Preset.SCH_AoE_Heal_WhisperingDawn:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_WhisperingDawnOption, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 0, $"{WhisperingDawn.ActionName()} Priority: ");
                    break;

                case Preset.SCH_AoE_Heal_FeyIllumination:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_FeyIlluminationOption, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 1, $"{FeyIllumination.ActionName()} Priority: ");
                    break;

                case Preset.SCH_AoE_Heal_FeyBlessing:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_FeyBlessingOption, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 2, $"{FeyBlessing.ActionName()} Priority: ");
                    break;

                case Preset.SCH_AoE_Heal_Consolation:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_ConsolationOption, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 3, $"{Consolation.ActionName()} Priority: ");
                    break;

                case Preset.SCH_AoE_Heal_SummonSeraph:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_SummonSeraph, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 6, $"{SummonSeraph.ActionName()} Priority: ");
                    break;

                case Preset.SCH_AoE_Heal_Seraphism:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_SeraphismOption, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 4, $"{Seraphism.ActionName()} Priority: ");
                    break;

                case Preset.SCH_AoE_Heal_Indomitability:
                    DrawSliderInt(0, 100, SCH_AoE_Heal_IndomitabilityOption, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(SCH_AoE_Heal_Indomitability_Recitation, "Recitation Option", "Will use Recitation to buff Indomitability.");
                    DrawPriorityInput(SCH_AoE_Heals_Priority, 8, 5, $"{Indomitability.ActionName()} Priority: ");
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
                    DrawRadioButton(SCH_Recitation_Mode, "Adloquium", "", 0);
                    DrawRadioButton(SCH_Recitation_Mode, "Succor", "", 1);
                    DrawRadioButton(SCH_Recitation_Mode, "Indomitability", "", 2);
                    DrawRadioButton(SCH_Recitation_Mode, "Excogitation", "", 3);
                    break;

                case Preset.SCH_Raidwide_Succor:
                    DrawAdditionalBoolChoice(SCH_Raidwide_Succor_Recitation, "Recitation Option", "Use Recitation to buff before the Raidwide Succor.");
                    break;

                case Preset.SCH_Retarget_SacredSoil:
                    DrawHorizontalMultiChoice(SCH_Retarget_SacredSoilOptions, "Enemy Hard Target", "Will place under hard target if it is an Enemy.", 2, 0);
                    DrawHorizontalMultiChoice(SCH_Retarget_SacredSoilOptions, "Ally Hard Target", "Will place under hard target if it is an Ally.", 2, 1);
                    break;

                case Preset.SCH_Mit_ST:
                    DrawHorizontalMultiChoice(SCH_Mit_STOptions, "Recitation", "Will Recitation before Adloquium if available.", 3, 0);
                    DrawHorizontalMultiChoice(SCH_Mit_STOptions, "Deployment Tactics", "Will spread Adloquium crit shield if available.", 3, 1);
                    DrawHorizontalMultiChoice(SCH_Mit_STOptions, "Excogitation", "Will use Excogitation if available.", 3, 2);
                    break;

                case Preset.SCH_Mit_AoE:
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, "Fey Illumination", "Will activate Fey Illumination before Succor", 4, 0);
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, "Crit Adloquium Deployment", "Will Recitation into Adloquium and Deployment tactics in place of Succor" +
                        "\nThis will be targeted at yourself for simplicity and reliability.", 4, 1);
                    DrawHorizontalMultiChoice(SCH_Mit_AoEOptions, "Expedient", "Will use Expedient if available.", 4, 2);
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
            SCH_AoE_DPS_ChainStratagemOption = new("SCH_AoE_DPS_ChainStratagemOption", 10),
            SCH_DPS_BioOption = new("SCH_DPS_BioOption"),
            SCH_DPS_BioSubOption = new("SCH_DPS_BioSubOption", 0),
            SCH_ST_DPS_EnergyDrain = new("SCH_ST_DPS_EnergyDrain", 3),
            SCH_ST_DPS_ChainStratagemSubOption = new("SCH_ST_DPS_ChainStratagemSubOption", 1),
            SCH_AoE_DPS_EnergyDrain = new("SCH_AoE_DPS_EnergyDrain", 3),
            SCH_AoE_DPS_ChainStratagemSubOption = new("SCH_AoE_DPS_ChainStratagemSubOption", 1),
            SCH_AoE_ADV_DPS_DoT_HPThreshold = new("SCH_AoE_ADV_DPS_DoT_HPThreshold", 30),
            SCH_AoE_ADV_DPS_DoT_MaxTargets = new("SCH_AoE_ADV_DPS_DoT_MaxTargets", 4),
            SCH_ST_DPS_Adv_Actions = new("SCH_ST_DPS_Adv_Actions");



        internal static UserBool
            SCH_ST_DPS_EnergyDrain_Burst = new("SCH_ST_DPS_EnergyDrain_Burst"),
            SCH_AoE_DPS_EnergyDrain_Burst = new("SCH_AoE_DPS_EnergyDrain_Burst"),
            SCH_AoE_DPS_ChainStratagemBanefulOption = new("SCH_AoE_DPS_ChainStratagemBanefulOption"),
            SCH_AoE_Heal_Aetherflow_Indomitability = new("SCH_AoE_Heal_Aetherflow_Indomitability"),
            SCH_AoE_Heal_Dissipation_Indomitability = new("SCH_AoE_Heal_Dissipation_Indomitability"),
            SCH_Raidwide_Succor_Recitation = new("SCH_Raidwide_Succor_Recitation");


        internal static UserFloat
            SCH_DPS_BioUptime_Threshold = new("SCH_DPS_BioUptime_Threshold", 3.0f),
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
            SCH_ST_Heals_Priority = new("SCH_ST_Heals_Priority", [11, 5, 6, 7, 12, 3, 2, 4, 8, 9, 10, 1]),
            SCH_AoE_Heals_Priority = new("SCH_AoE_Heals_Priority", [1, 2, 3, 4, 5, 6, 7, 8]);

        public static UserBool
            SCH_ST_Heal_IncludeShields = new("SCH_ST_Heal_IncludeShields"),
            SCH_ST_Heal_WhisperingDawnBossOption = new("SCH_ST_Heal_WhisperingDawnBossOption"),
            SCH_ST_Heal_FeyIlluminationBossOption = new("SCH_ST_Heal_FeyIlluminationBossOption"),
            SCH_ST_Heal_FeyBlessingBossOption = new("SCH_ST_Heal_FeyBlessingBossOption"),
            SCH_ST_Heal_ExcogitationBossOption = new("SCH_ST_Heal_ExcogitationBossOption"),
            SCH_ST_Heal_ExcogitationTankOption = new("SCH_ST_Heal_ExcogitationTankOption"),
            SCH_ST_Heal_ProtractionBossOption = new("SCH_ST_Heal_ProtractionBossOption"),
            SCH_ST_Heal_ProtractionTankOption = new("SCH_ST_Heal_ProtractionTankOption"),
            SCH_ST_Heal_SeraphismBossOption = new("SCH_ST_Heal_SeraphismBossOption"),
            SCH_ST_Heal_ExpedientBossOption = new("SCH_ST_Heal_ExpedientBossOption"),
            SCH_ST_Heal_SummonSeraphBossOption = new("SCH_ST_Heal_SummonSeraphBossOption"),
            SCH_ST_Heal_ConsolationBossOption = new("SCH_ST_Heal_ConsolationBossOption"),
            SCH_AoE_Heal_Indomitability_Recitation = new("SCH_AoE_Heal_Indomitability_Recitation");

        public static UserBoolArray
            SCH_ST_Heal_AldoquimOpts = new("SCH_ST_Heal_AldoquimOpts"),
            SCH_AoE_Heal_Succor_Options = new("SCH_AoE_Heal_Succor_Options");

        #endregion

        #region Standalones

        internal static UserBool
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
