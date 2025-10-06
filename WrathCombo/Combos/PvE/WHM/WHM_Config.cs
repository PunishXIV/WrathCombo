#region
using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Window.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.UserConfig;

// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable GrammarMistakeInComment
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM
{
    public static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target DPS

                case Preset.WHM_ST_MainCombo:
                    DrawHorizontalRadioButton(WHM_ST_MainCombo_Actions, "On Stones/Glares", "Apply options to all Stones and Glares.", 0,
                        descriptionColor:ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(WHM_ST_MainCombo_Actions, "On Aeros/Dia", "Apply options to all Aeros And Dia.", 1,
                        descriptionColor:ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(WHM_ST_MainCombo_Actions, "On Stone II", "Apply options to Stone II.", 2,
                        descriptionColor:ImGuiColors.DalamudWhite);
                    break;

                case Preset.WHM_ST_MainCombo_Opener:
                    DrawBossOnlyChoice(WHM_Balance_Content);
                    break;

                case Preset.WHM_ST_MainCombo_DoT:
                    DrawSliderInt(0, 100, WHM_ST_DPS_AeroOptionBoss,
                        targetStopUsingOnBossAtDescription,
                        itemWidth: medium);

                    ImGui.Spacing();
                    DrawSliderInt(0, 100, WHM_ST_DPS_AeroOptionNonBoss,
                        targetStopUsingAtDescription,
                        itemWidth: medium);
      
                    ImGui.Indent();
                    ImGui.TextUnformatted("For Non-Bosses, select what kind of content this applies to:");
                    ImGui.NewLine();
                    ImGui.Indent();
                    DrawHorizontalRadioButton(
                        WHM_ST_DPS_AeroOptionSubOption, "All Content",
                        "Apply the HP% for Non-Bosses to all content.",
                        outputValue: (int)EnemyRestriction.AllEnemies,
                        descriptionColor: ImGuiColors.DalamudWhite
                    );
                    DrawHorizontalRadioButton(
                        WHM_ST_DPS_AeroOptionSubOption, "Boss Only Content",
                        "Apply the HP% for Non-Bosses, only in Boss content (to adds).\nAlways applies DoTs at any HP outside of Boss content.",
                        outputValue: (int)EnemyRestriction.OnlyBosses,
                        descriptionColor: ImGuiColors.DalamudWhite
                    );
                    DrawHorizontalRadioButton(
                        WHM_ST_DPS_AeroOptionSubOption, "Non-Boss Only Content",
                        "Apply the HP% for Non-Bosses, only outside Boss content.\nAlways applies DoTs at any HP to adds in boss content.",
                        outputValue: (int)EnemyRestriction.NonBosses,
                        descriptionColor: ImGuiColors.DalamudWhite
                    );

                    ImGui.Spacing();
                    ImGui.Unindent();
                    DrawRoundedSliderFloat(0, 4, WHM_ST_MainCombo_DoT_Threshold,
                        reapplyTimeRemainingDescription,
                        itemWidth: little, digits: 1);

                    ImGui.Unindent();
                    break;
                
                case Preset.WHM_ST_MainCombo_Misery:
                    DrawHorizontalRadioButton(WHM_ST_MainCombo_Misery_Option, "Hold for Burst", "Will attempt to hold for burst as long as possible without overcapping. \nWill prevent afflatus heals from being possible when at full Blood Lily stacks.", 0, descriptionColor: ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(WHM_ST_MainCombo_Misery_Option, "Use Immediately", "Will Use Immediately to make sure you are free to use Afflatus heals. ", 1, descriptionColor: ImGuiColors.DalamudWhite);
                    break;
                
                case Preset.WHM_ST_MainCombo_LilyOvercap:
                    DrawSliderInt(0, 10, WHM_STDPS_LilyOvercap, "Time in Seconds to use Afflatus Rapture before overcapping Lily stacks", itemWidth: medium);
                    break;

                case Preset.WHM_ST_MainCombo_Lucid:
                    DrawSliderInt(4000, 9500, WHM_STDPS_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;

                #endregion

                #region AoE DPS

                case Preset.WHM_AoE_DPS_Lucid:
                    DrawSliderInt(4000, 9500, WHM_AoEDPS_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;
                
                case Preset.WHM_AoE_MainCombo_DoT:
                    DrawSliderInt(0, 100, WHM_AoE_MainCombo_DoT_HPThreshold,
                        targetStopUsingAtDescription);
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 5, WHM_AoE_MainCombo_DoT_Reapply,
                        reapplyTimeRemainingDescription,
                        itemWidth: little, digits: 1);
                    ImGui.Unindent();
                    DrawSliderInt(0, 10, WHM_AoE_MainCombo_DoT_MaxTargets,
                        "Maximum number of targets to employ multi-dotting ");
                    break;
                
                case Preset.WHM_AoE_DPS_Misery:
                    DrawHorizontalRadioButton(WHM_AoE_DPS_Misery_Option, "Hold for Burst", "Will attempt to hold for burst as long as possible without overcapping. \nWill prevent afflatus heals from being possible when at full Blood Lily stacks.", 0 ,descriptionColor: ImGuiColors.DalamudWhite);
                    DrawHorizontalRadioButton(WHM_AoE_DPS_Misery_Option, "Use Immediately", "Will Use Immediately to make sure you are free to use Afflatus heals. ", 1 ,descriptionColor: ImGuiColors.DalamudWhite);
                    break;
                
                case Preset.WHM_AoE_DPS_LilyOvercap:
                    DrawSliderInt(0, 10, WHM_AoEDPS_LilyOvercap, "Time in Seconds to use Afflatus Rapture before overcapping Lily stacks", itemWidth: medium);
                    break;

                #endregion

                #region Single Target Heals

                case Preset.WHM_STHeals:
                    DrawAdditionalBoolChoice(WHM_STHeals_IncludeShields,
                        "Include Shields in HP Percent Sliders",
                        "");
                    break;
                
                case Preset.WHM_STHeals_Benediction: 
                    DrawAdditionalBoolChoice(WHM_STHeals_BenedictionWeave,
                        weaveDescription, "");
                    DrawSliderInt(1, 100, WHM_STHeals_BenedictionHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 0,
                        $"{Benediction.ActionName()} Priority: ");
                    break;

                case Preset.WHM_STHeals_Tetragrammaton:
                    DrawAdditionalBoolChoice(WHM_STHeals_TetraBalance, 
                        "Balance Charges Option", "Will only use if Tetra Charges are greater than or equal to Divine Benison Charges.");
                    DrawAdditionalBoolChoice(WHM_STHeals_TetraWeave,
                        weaveDescription, "");
                    DrawSliderInt(1, 100, WHM_STHeals_TetraHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 1,
                        $"{Tetragrammaton.ActionName()} Priority: ");
                    break;

                case Preset.WHM_STHeals_Benison:
                    DrawAdditionalBoolChoice(WHM_STHeals_BenisonBalance, 
                        "Balance Charges Option", "Will only use if Divine Benison Charges are greater than or equal to Tetragrammaton Charges.");
                    DrawAdditionalBoolChoice(WHM_STHeals_BenisonWeave,
                        weaveDescription, "");
                    DrawSliderInt(0, 1, WHM_STHeals_BenisonCharges, 
                        chargesToKeepDescription);
                    DrawSliderInt(1, 100, WHM_STHeals_BenisonHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 2,
                        $"{DivineBenison.ActionName()} Priority: ");
                    break;

                case Preset.WHM_STHeals_Aquaveil:
                    DrawHorizontalMultiChoice(WHM_STHeals_AquaveilOptions,"Only Weave", weaveDescription, 3, 0);
                    DrawHorizontalMultiChoice(WHM_STHeals_AquaveilOptions,"Not On Bosses", nonBossesDescription, 3, 1);
                    DrawHorizontalMultiChoice(WHM_STHeals_AquaveilOptions,"Tanks Only", "Only on Tanks", 3, 2);
                    DrawSliderInt(1, 100, WHM_STHeals_AquaveilHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 3,
                        $"{Aquaveil.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_STHeals_Solace:
                    DrawSliderInt(1, 100, WHM_STHeals_SolaceHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 4,
                        $"{AfflatusSolace.ActionName()} Priority: ");
                    break;

                case Preset.WHM_STHeals_Regen:
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0f, 6f, WHM_STHeals_RegenTimer,
                        reapplyTimeRemainingDescription,
                        itemWidth: little);
                    ImGui.Unindent();
                    DrawSliderInt(0, 100, WHM_STHeals_RegenHPLower,
                        targetStopUsingAtDescription);
                    DrawSliderInt(0, 100, WHM_STHeals_RegenHPUpper,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 5,
                        $"{Regen.ActionName()} Priority: ");
                    break;

                case Preset.WHM_STHeals_Temperance:
                    DrawSliderInt(1, 100, WHM_STHeals_TemperanceHP,
                        targetStartUsingAtDescription);
                    DrawHorizontalMultiChoice(WHM_STHeals_TemperanceOptions,"Only Weave", weaveDescription, 2, 0);
                    DrawHorizontalMultiChoice(WHM_STHeals_TemperanceOptions,"Not On Bosses", nonBossesDescription, 2, 1);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 6,
                        $"{Temperance.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_STHeals_Asylum:
                    DrawSliderInt(1, 100, WHM_STHeals_AsylumHP,
                        targetStartUsingAtDescription);
                    DrawHorizontalMultiChoice(WHM_STHeals_AsylumOptions,"Only Weave", weaveDescription, 2, 0);
                    DrawHorizontalMultiChoice(WHM_STHeals_AsylumOptions,"Not On Bosses", nonBossesDescription, 2, 1);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 7,
                        $"{Asylum.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_STHeals_LiturgyOfTheBell:
                    DrawSliderInt(1, 100, WHM_STHeals_LiturgyOfTheBellHP,
                        targetStartUsingAtDescription);
                    DrawHorizontalMultiChoice(WHM_STHeals_LiturgyOfTheBellOptions,"Only Weave", weaveDescription, 2, 0);
                    DrawHorizontalMultiChoice(WHM_STHeals_LiturgyOfTheBellOptions,"Not On Bosses", nonBossesDescription, 2, 1);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 9, 8,
                        $"{LiturgyOfTheBell.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_STHeals_ThinAir:
                    DrawSliderInt(0, 1, WHM_STHeals_ThinAir,
                        chargesToKeepDescription);
                    break;
                
                case Preset.WHM_STHeals_Lucid:
                    DrawSliderInt(4000, 9500, WHM_STHeals_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;

                case Preset.WHM_STHeals_Esuna:
                    DrawSliderInt(0, 100, WHM_STHeals_Esuna,
                        targetStopUsingAtDescription);
                    break;

                #endregion

                #region AoE Heals
                
                case Preset.WHM_AoEHeals_Medica2: 
                    DrawSliderInt(1, 100, WHM_AoEHeals_Medica2HP,
                        partyStartUsingAtDescription);
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0f, 6f, WHM_AoEHeals_MedicaTime,
                        reapplyTimeRemainingDescription,
                        itemWidth: little);
                    ImGui.Unindent();
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 0,
                        $"{Medica2.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_Cure3:
                    DrawSliderInt(1, 100, WHM_AoEHeals_Cure3HP,
                        partyStartUsingAtDescription);
                    DrawSliderInt(2, 8, WHM_AoEHeals_Cure3Allies,
                        "Minimum Number of allies in range of Cure 3 target");
                    DrawSliderInt(1500, 8500, WHM_AoEHeals_Cure3MP,
                        "MP to be over",
                        sliderIncrement: 500);
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 1,
                        $"{Cure3.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_Plenary:
                    DrawSliderInt(1, 100, WHM_AoEHeals_PlenaryHP,
                        partyStartUsingAtDescription);
                    DrawAdditionalBoolChoice(WHM_AoEHeals_PlenaryWeave,
                        weaveDescription,
                        "");
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 2,
                        $"{PlenaryIndulgence.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_Temperance:
                    DrawSliderInt(1, 100, WHM_AoEHeals_TemperanceHP, 
                        partyStartUsingAtDescription);
                    DrawAdditionalBoolChoice(WHM_AoEHeals_TemperanceWeave,
                        weaveDescription,
                        "");
                    DrawDifficultyMultiChoice(WHM_AoEHeals_TemperanceDifficulty,
                        WHM_AoEHeals_TemperanceDifficultyListSet,
                        "Select what content difficulties Temperance should be used in:");
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 3,
                        $"{Temperance.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_Asylum:
                    DrawSliderInt(1, 100, WHM_AoEHeals_AsylumHP, 
                        partyStartUsingAtDescription);
                    DrawAdditionalBoolChoice(WHM_AoEHeals_AsylumWeave,
                        weaveDescription,
                        "");
                    DrawDifficultyMultiChoice(WHM_AoEHeals_AsylumDifficulty,
                        WHM_AoEHeals_AsylumDifficultyListSet,
                        "Select what content difficulties Asylum should be used in:");
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 4,
                        $"{Asylum.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_LiturgyOfTheBell:
                    DrawSliderInt(1, 100, WHM_AoEHeals_LiturgyHP, 
                        partyStartUsingAtDescription);
                    DrawAdditionalBoolChoice(WHM_AoEHeals_LiturgyWeave,
                        weaveDescription,
                        "");
                    DrawDifficultyMultiChoice(WHM_AoEHeals_LiturgyDifficulty,
                        WHM_AoEHeals_LiturgyDifficultyListSet,
                        "Select what content difficulties LiturgyOfTheBell should be used in:");
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 5,
                        $"{LiturgyOfTheBell.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_Rapture:
                    DrawSliderInt(1, 100, WHM_AoEHeals_RaptureHP,
                        partyStartUsingAtDescription);
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 6,
                        $"{AfflatusRapture.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_Assize:
                    DrawSliderInt(1, 100, WHM_AoEHeals_AssizeHP,
                        partyStartUsingAtDescription);
                    DrawAdditionalBoolChoice(WHM_AoEHeals_AssizeWeave,
                        weaveDescription, "");
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 7,
                        $"{Assize.ActionName()} Priority: ");
                    break;
                
                case Preset.WHM_AoEHeals_DivineCaress:
                    DrawSliderInt(1, 100, WHM_AoEHeals_DivineCaressHP,
                        partyStartUsingAtDescription);
                    DrawAdditionalBoolChoice(WHM_AoEHeals_DivineCaressWeave,
                        weaveDescription, "");
                    DrawPriorityInput(WHM_AoE_Heals_Priority, 9, 8,
                        $"{DivineCaress.ActionName()} Priority: ");
                    break;

                case Preset.WHM_AoEHeals_ThinAir:
                    DrawSliderInt(0, 1, WHM_AoEHeals_ThinAir,
                        chargesToKeepDescription);
                    break;

                case Preset.WHM_AoEHeals_Lucid:
                    DrawSliderInt(4000, 9500, WHM_AoEHeals_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;
                
                #endregion

                #region Mitigation Features

                case Preset.WHM_Mit_ST:
                    DrawHorizontalMultiChoice(WHM_AquaveilOptions,
                        "Include Divine Benison", "Will add Divine Benison for more mitigation.", 2, 0);
                    ImGui.NewLine();
                    DrawHorizontalMultiChoice(WHM_AquaveilOptions,
                        "Include Tetragrammaton", "Will add Tetragrammaton to top off targets health.", 2, 1);
                    if (WHM_AquaveilOptions[1])
                    {
                        ImGui.Indent();
                        DrawSliderInt(1, 100, WHM_Aquaveil_TetraThreshold,
                            "Target HP% to use Tetra below)");
                        ImGui.Unindent();
                    }
                    break;

                #endregion
                
                #region Retargeting Features
                
                case Preset.WHM_Re_Asylum:
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudGrey, "Options to try to Retarget Asylum to before Self:");
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(WHM_AsylumOptions,
                        "Enemy Hard Target", "Will place at hard target if enemy", 3, 0);
                    DrawHorizontalMultiChoice(WHM_AsylumOptions,
                        "Ally Hard Target", "Will place at hard target if ally", 3, 1);
                    break;
                
                case Preset.WHM_Re_LiturgyOfTheBell:
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudGrey, "Options to try to Retarget Liturgy of the Bell to before Self:");
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(WHM_LiturgyOfTheBellOptions,
                        "Enemy Hard Target", "Will place at hard target if enemy", 2, 0);
                    DrawHorizontalMultiChoice(WHM_LiturgyOfTheBellOptions,
                        "Ally Hard Target", "Will place at hard target if ally", 2, 1);
                    break;
                    
                
                #endregion
            }
        }

        #region Constants

        /// Smallest bar width
        private const float little = 100f;

        /// 2nd smallest bar width
        private const float medium = 150f;

        /// Bar Description for target HP% to start using plus disable text
        private const string targetStartUsingAtDescription =
            "Target HP% to use at or below (100 = Disable check)";
        
        /// Bar Description for Party HP%  Average to start using plus disable text
        private const string partyStartUsingAtDescription =
            "Start using when below party average HP% (100 = Disable check)";

        /// Bar Description for target HP% to start using plus disable text
        private const string targetStopUsingAtDescription =
            " Target HP% to stop using (0 = Use Always, 100 = Never)";

        /// Bar Description for target HP% to start using plus disable text
        private const string targetStopUsingOnBossAtDescription =
            " Bosses HP% to stop using (0 = Use Always, 100 = Never)";

        /// Description for MP threshold
        private const string mpThresholdDescription =
            "MP to be at or below";

        /// Description for reapplication of Buff/DoT time remaining
        private const string reapplyTimeRemainingDescription =
            "Seconds remaining before reapplying (0 = Do not reapply early)";

        /// Description for charges to keep
        private const string chargesToKeepDescription =
            "# charges to keep (0 = Use All)";

        /// Description for only weaving
        private const string weaveDescription =
            "Only Weave";

        private const string nonBossesDescription =
            "Will not use on ST in Boss encounters.";

        /// <summary>
        ///     Whether abilities should be restricted to bosses or not.
        /// </summary>
        internal enum BossRequirement
        {
            Off = 1,
            On = 2,
        }

        /// <summary>
        ///     Enemy type restriction for HP threshold checks.
        /// </summary>
        internal enum EnemyRestriction
        {
            NonBosses = 0,
            AllEnemies = 1,
            OnlyBosses = 2,
        }

        #endregion

        #region Options

        #region Single Target DPS

        /// <summary>
        ///     Button Selection for single target DPS.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo" />
        public static UserInt WHM_ST_MainCombo_Actions =
            new("WHM_ST_MainCombo_Actions");

        /// <summary>
        ///     Content type of Balance Opener.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: All Content<br />
        ///     <b>Options</b>: All Content or
        ///     <see cref="ContentCheck.IsInBossOnlyContent" />
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo_Opener" />
        public static UserInt WHM_Balance_Content =
            new("WHM_Balance_Content", 0);

        /// <summary>
        ///     HP threshold to stop applying DoTs on Bosses.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo_DoT" />
        public static UserInt WHM_ST_DPS_AeroOptionBoss =
            new("WHM_ST_DPS_AeroOptionBoss");

        /// <summary>
        ///     HP threshold to stop applying DoTs on Non-Bosses.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo_DoT" />
        public static UserInt WHM_ST_DPS_AeroOptionNonBoss =
            new("WHM_ST_DPS_AeroOptionNonBoss", 50);

        /// <summary>
        ///     Time threshold in seconds before reapplying DoT.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 4 <br />
        ///     <b>Step</b>: 0.1
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo_DoT" />
        public static UserFloat WHM_ST_MainCombo_DoT_Threshold =
            new("WHM_ST_MainCombo_DoT_Threshold", 0);

        /// <summary>
        ///     Enemy type to apply the HP threshold check to.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="EnemyRestriction.AllEnemies" /> <br />
        ///     <b>Options</b>: <see cref="EnemyRestriction">EnemyRestriction Enum</see>
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo_DoT" />
        public static UserInt WHM_ST_DPS_AeroOptionSubOption =
            new("WHM_ST_DPS_AeroOptionSubOption", (int)EnemyRestriction.AllEnemies);
        
        /// <summary>
        ///     Pooling option for Afflatus Misery. Default 1 = Use Immediately, 0 = Hold for Burst
        /// </summary>
        /// <seealso cref="Preset.WHM_ST_MainCombo_Misery" />
        public static UserInt WHM_ST_MainCombo_Misery_Option = 
            new("WHM_ST_MainCombo_Misery_Option", 1);
        
        /// <summary>
        ///     Overcap Prevention Slider for Afflatus Rapture
        ///     Default 8 = Use Rapture if Lily will overcap in 8 seconds
        /// </summary>
        /// <seealso cref="Preset.WHM_ST_MainCombo_LilyOvercap" />
        public static UserInt WHM_STDPS_LilyOvercap = 
            new("WHM_STDPS_LilyOvercap", 8);

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in single target rotations.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="Preset.WHM_ST_MainCombo_Lucid" />
        public static UserInt WHM_STDPS_Lucid =
            new("WHMLucidDreamingFeature", 6500);

        #endregion

        #region AoE DPS

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in AoE rotations.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoE_DPS_Lucid" />
        public static UserInt WHM_AoEDPS_Lucid =
            new("WHM_AoE_Lucid", 6500);
        
        /// <summary>
        ///     Reapplication Threshold for AoE Multi-DoTing
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 5<br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoE_MainCombo_DoT" />
        public static UserFloat WHM_AoE_MainCombo_DoT_Reapply =
            new("WHM_AoE_MainCombo_DoT_Reapply", 0);
        
        /// <summary>
        ///     Health Threshold to stop Multi-DoTing
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 0 - 100<br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoE_MainCombo_DoT" />
        public static UserInt WHM_AoE_MainCombo_DoT_HPThreshold = 
            new("WHM_AoE_MainCombo_DoT_HPThreshold", 50);
        
        /// <summary>
        ///     Max Targets for AoE Multi-DoTing
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: 4 <br />
        ///     <b>Range</b>: 0 - 10<br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoE_MainCombo_DoT" />
        public static UserInt WHM_AoE_MainCombo_DoT_MaxTargets = 
            new("WHM_AoE_MainCombo_DoT_MaxTargets", 4);
        
        /// <summary>
        ///     Pooling option for Afflatus Misery. Default 1 = Use Immediately, 0 = Hold for Burst
        /// </summary>
        /// <seealso cref="Preset.WHM_AoE_DPS_Misery" />
        public static UserInt WHM_AoE_DPS_Misery_Option = 
            new("WHM_AoE_DPS_Misery_Option", 1);
        
        /// <summary>
        ///     Overcap Prevention Slider for Afflatus Rapture
        ///     Default 8 = Use Rapture if Lily will overcap in 8 seconds
        /// </summary>
        /// <seealso cref="Preset.WHM_AoE_DPS_LilyOvercap" />
        public static UserInt WHM_AoEDPS_LilyOvercap = 
            new("WHM_AoEDPS_LilyOvercap", 8);

        #endregion

        #region Single Target Heals 

        /// <summary>
        ///     Include shields when calculating HP percentages.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals" />
        public static UserBool WHM_STHeals_IncludeShields =
            new("WHM_STHeals_IncludeShields", false);

        /// <summary>
        ///     Priority order for single target healing abilities.
        /// </summary>
        public static UserIntArray WHM_ST_Heals_Priority =
            new("WHM_ST_Heals_Priority", [1,8,7,6,9,5,2,3,4]);

        /// <summary>
        ///     Time threshold in seconds before refreshing Regen.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 6 <br />
        ///     <b>Step</b>: 0.1
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Regen" />
        public static UserFloat WHM_STHeals_RegenTimer =
            new("WHM_STHeals_RegenTimer", 0);

        /// <summary>
        ///     Lower HP threshold to stop using Regen.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Regen" />
        public static UserInt WHM_STHeals_RegenHPLower =
            new("WHM_STHeals_RegenHPLower", 30);

        /// <summary>
        ///     Upper HP threshold to start using Regen.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Regen" />
        public static UserInt WHM_STHeals_RegenHPUpper =
            new("WHM_STHeals_RegenHPUpper", 100);

        /// <summary>
        ///     Only use Benediction when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Benediction" />
        public static UserBool WHM_STHeals_BenedictionWeave =
            new("WHM_STHeals_BenedictionWeave", false);

        /// <summary>
        ///     HP threshold to use Benediction.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Benediction" />
        public static UserInt WHM_STHeals_BenedictionHP =
            new("WHM_STHeals_BenedictionHP", 40);
        
        /// <summary>
        ///     HP threshold to use Afflatus Solace.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Solace" />
        
        public static UserInt WHM_STHeals_SolaceHP = 
            new("WHM_STHeals_SolaceHP", 80);

        /// <summary>
        ///     Number of Thin Air charges to reserve.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 1 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_ThinAir" />
        public static UserInt WHM_STHeals_ThinAir =
            new("WHM_STHeals_ThinAir", 0);

        /// <summary>
        ///     Only use Tetragrammaton when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Tetragrammaton" />
        public static UserBool WHM_STHeals_TetraWeave =
            new("WHM_STHeals_TetraWeave", false);
        
        
        /// <summary>
        ///     Only use Tetragrammaton when it has greater than or equal charges to divine Benison
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Tetragrammaton" />
        public static UserBool WHM_STHeals_TetraBalance = new("WHM_STHeals_TetraBalance", false);

        /// <summary>
        ///     HP threshold to use Tetragrammaton.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Tetragrammaton" />
        public static UserInt WHM_STHeals_TetraHP =
            new("WHM_STHeals_TetraHP", 50);

        /// <summary>
        ///     Only use Divine Benison when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Benison" />
        public static UserBool WHM_STHeals_BenisonWeave =
            new("WHM_STHeals_BenisonWeave", false);
        
        /// <summary>
        ///     Only use Divine Benison when it has greater than or equal charges to Tetragrammaton
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Benison" />
        
        public static UserBool WHM_STHeals_BenisonBalance = new("WHM_STHeals_BenisonBalance", false);
        
        /// <summary>
        ///     Charges to keep of Divine Benison.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 1 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Benison" />
        public static UserInt WHM_STHeals_BenisonCharges =
            new("WHM_STHeals_BenisonCharges", 0);

        /// <summary>
        ///     HP threshold to use Divine Benison.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Benison" />
        public static UserInt WHM_STHeals_BenisonHP =
            new("WHM_STHeals_BenisonHP", 99);

        /// <summary>
        ///     HP threshold to use Aquaveil.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Aquaveil" />
        public static UserInt WHM_STHeals_AquaveilHP =
            new("WHM_STHeals_AquaveilHP", 90);

        /// <summary>
        ///     Aquaveil weaving and boss options
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Aquaveil" />
        public static UserBoolArray WHM_STHeals_AquaveilOptions =
            new("WHM_STHeals_AquaveilOptions");

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in single target healing.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Lucid" />
        public static UserInt WHM_STHeals_Lucid =
            new("WHM_STHeals_Lucid", 6500);

        /// <summary>
        ///     Weaving and boss selection options for Temperance.
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Temperance" />
        public static UserBoolArray WHM_STHeals_TemperanceOptions =
            new("WHM_STHeals_TemperanceOptions");

        /// <summary>
        ///     HP threshold to use Temperance.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 75 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Temperance" />
        public static UserInt WHM_STHeals_TemperanceHP =
            new("WHM_STHeals_TemperanceHP", 75);

        /// <summary>
        ///     Weaving and boss selection options for Asylum.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Asylum" />
        public static UserBoolArray WHM_STHeals_AsylumOptions =
            new("WHM_STHeals_AsylumOptions");

        /// <summary>
        ///     HP threshold to use Asylum.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 75 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Asylum" />
        public static UserInt WHM_STHeals_AsylumHP =
            new("WHM_STHeals_AsylumHP", 75);
        
        /// <summary>
        ///     Weaving and boss selection options for LiturgyOfTheBell.
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_LiturgyOfTheBell" />
        public static UserBoolArray WHM_STHeals_LiturgyOfTheBellOptions =
            new("WHM_STHeals_LiturgyOfTheBellOptions");

        /// <summary>
        ///     HP threshold to use LiturgyOfTheBell.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 75 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_LiturgyOfTheBell" />
        public static UserInt WHM_STHeals_LiturgyOfTheBellHP =
            new("WHM_STHeals_LiturgyOfTheBellHP", 75);

        /// <summary>
        ///     HP threshold to stop using Esuna.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 40 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_STHeals_Esuna" />
        public static UserInt WHM_STHeals_Esuna =
            new("WHM_Cure2_Esuna", 40);

        #endregion

        #region AoE Heals
        
        /// <summary>
        ///     Priority order for AoE healing abilities.
        /// </summary>
        public static UserIntArray WHM_AoE_Heals_Priority =
            new("WHM_AoE_Heals_Priority",[9,8,6,5,3,4,7,2,1]);

        /// <summary>
        ///     Number of Thin Air charges to reserve in AoE healing.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 1 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_ThinAir" />
        public static UserInt WHM_AoEHeals_ThinAir =
            new("WHM_AoE_ThinAir");
        
        /// <summary>
        ///     Average party HP% threshold to use Cure3.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Cure3" />
        
        public static UserInt WHM_AoEHeals_Cure3HP = 
            new("WHM_AoEHeals_Cure3HP", 100);
        
        /// <summary>
        ///     Minimum Party Members In range of target to use Cure 3.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 2 <br />
        ///     <b>Range</b>: 2 - 8 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Cure3" />
        
        public static UserInt WHM_AoEHeals_Cure3Allies = 
            new("WHM_AoEHeals_Cure3Allies", 2);

        /// <summary>
        ///     MP threshold to use Cure III.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 1500 - 8500 <br />
        ///     <b>Step</b>: 500
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Cure3" />
        public static UserInt WHM_AoEHeals_Cure3MP =
            new("WHM_AoE_Cure3MP");

        /// <summary>
        ///     Average party HP% threshold to use Assize.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Assize" />
        public static UserInt WHM_AoEHeals_AssizeHP = 
            new("WHM_AoEHeals_AssizeHP", 100);
        
        /// <summary>
        ///     Only use Assize when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Assize" />
        public static UserBool WHM_AoEHeals_AssizeWeave =
            new("WHM_AoEHeals_AssizeWeave");
        
        /// <summary>
        ///     Average party HP% threshold to use Plenary.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Plenary" />
        
        public static UserInt WHM_AoEHeals_PlenaryHP = 
            new("WHM_AoEHeals_PlenaryHP", 100);

        /// <summary>
        ///     Only use Plenary Indulgence when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Plenary" />
        public static UserBool WHM_AoEHeals_PlenaryWeave =
            new("WHM_AoEHeals_PlenaryWeave");

        /// <summary>
        ///     Only use Temperance when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Temperance" />
        public static UserBool WHM_AoEHeals_TemperanceWeave =
            new("WHM_AoEHeals_TemperanceWeave");

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in AoE healing.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Lucid" />
        public static UserInt WHM_AoEHeals_Lucid =
            new("WHM_AoEHeals_Lucid", 6500);

        /// <summary>
        ///     Time threshold in seconds before refreshing Medica II/III.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 6 <br />
        ///     <b>Step</b>: 0.1
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Medica2" />
        public static UserFloat WHM_AoEHeals_MedicaTime =
            new("WHM_AoEHeals_MedicaTime");
        
        /// <summary>
        ///     Average party HP% threshold to use Medica2.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Medica2" />
        
        public static UserInt WHM_AoEHeals_Medica2HP = 
            new("WHM_AoEHeals_Medica2HP", 100);
        
        /// <summary>
        ///     Average party HP% threshold to use Rapture.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Rapture" />
        
        public static UserInt WHM_AoEHeals_RaptureHP = 
            new("WHM_AoEHeals_RaptureHP", 100);
        
        /// <summary>
        ///     Average party HP% threshold to use Divine Caress.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_DivineCaress" />
        
        public static UserInt WHM_AoEHeals_DivineCaressHP = 
            new("WHM_AoEHeals_DivineCaressHP", 100);
        
        /// <summary>
        ///     Only use Divine Caress when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_DivineCaress" />
        public static UserBool WHM_AoEHeals_DivineCaressWeave =
            new("WHM_AoEHeals_DivineCaressWeave");
        
        /// <summary>
        ///     Average party HP% threshold to use LiturgyOfTheBell.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_LiturgyOfTheBell" />
        public static UserInt WHM_AoEHeals_LiturgyHP =
            new("WHM_AoEHeals_LiturgyHP", 30);
        
        /// <summary>
        ///     Only use Liturgy when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_LiturgyOfTheBell" />
        public static UserBool WHM_AoEHeals_LiturgyWeave =
            new("WHM_AoEHeals_LiturgyWeave");

        /// <summary>
        ///     Content difficulty selector for Liturgy of the Bell.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /> <br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_LiturgyOfTheBell" />
        public static UserBoolArray WHM_AoEHeals_LiturgyDifficulty =
            new("WHM_AoEHeals_LiturgyDifficulty", [true, false]);

        /// <summary>
        ///     Content difficulty list set for Liturgy of the Bell, set by
        ///     <see cref="WHM_AoEHeals_LiturgyDifficulty" />.
        /// </summary>
        /// <seealso cref="Preset.WHM_AoEHeals_LiturgyOfTheBell" />
        public static readonly ContentCheck.ListSet
            WHM_AoEHeals_LiturgyDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     Average party HP% threshold to use Temperance.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Temperance" />
        public static UserInt WHM_AoEHeals_TemperanceHP =
            new("WHM_AoEHeals_TemperanceHP", 30);

        /// <summary>
        ///     Content difficulty selector for Temperance.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and <see cref="ContentCheck.TopHalfContent" /><br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Temperance" />
        public static UserBoolArray WHM_AoEHeals_TemperanceDifficulty =
            new("WHM_AoEHeals_TemperanceDifficulty", [true, true]);

        /// <summary>
        ///     Content difficulty list set for Temperance, set by
        ///     <see cref="WHM_AoEHeals_TemperanceDifficulty" />.
        /// </summary>
        /// <seealso cref="Preset.WHM_AoEHeals_Temperance" />
        public static readonly ContentCheck.ListSet
            WHM_AoEHeals_TemperanceDifficultyListSet =
                ContentCheck.ListSet.Halved;
        
        /// <summary>
        ///     Average party HP% threshold to use Asylum.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Asylum" />
        public static UserInt WHM_AoEHeals_AsylumHP =
            new("WHM_AoEHeals_AsylumHP", 30);
        
        /// <summary>
        ///     Only use Asylum when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Asylum" />
        public static UserBool WHM_AoEHeals_AsylumWeave =
            new("WHM_AoEHeals_AsylumWeave");

        /// <summary>
        ///     Content difficulty selector for Asylum.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and <see cref="ContentCheck.TopHalfContent" /><br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="Preset.WHM_AoEHeals_Asylum" />
        public static UserBoolArray WHM_AoEHeals_AsylumDifficulty =
            new("WHM_AoEHeals_AsylumDifficulty", [true, true]);

        /// <summary>
        ///     Content difficulty list set for Asylum, set by
        ///     <see cref="WHM_AoEHeals_AsylumDifficulty" />.
        /// </summary>
        /// <seealso cref="Preset.WHM_AoEHeals_Asylum" />
        public static readonly ContentCheck.ListSet
            WHM_AoEHeals_AsylumDifficultyListSet =
                ContentCheck.ListSet.Halved;

        #endregion
        
        #region Standalone Features

        /// <summary>
        ///     Hard target Retargeting Options for Asylum Standalone Feature
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: True True
        /// </value>
        /// <seealso cref="Preset.WHM_Re_Asylum" />
        public static UserBoolArray WHM_AsylumOptions = 
            new("WHM_AsylumOptions", [true, true]);
        
        /// <summary>
        ///     Hard target Retargeting Options for Liturgy Of The Bell
        ///     Standalone Feature
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: True True
        /// </value>
        /// <seealso cref="Preset.WHM_Re_LiturgyOfTheBell" />
        public static UserBoolArray WHM_LiturgyOfTheBellOptions = 
            new ("WHM_LiturgyOfTheBellOptions", [true, true]);
        
        /// <summary>
        ///     Options for Aquaveil Standalone Feature
        /// </summary> 
        /// <value>
        ///     <b>Default</b>: True True
        /// </value>
        /// <seealso cref="Preset.WHM_Mit_ST" />
        public static UserBoolArray WHM_AquaveilOptions = 
            new ("WHM_AquaveilOptions", [true, true]);
        
        /// <summary>
        ///     Tetra threshold for Aquaveil standalone feature
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 100 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="Preset.WHM_Mit_ST" />
        public static UserInt WHM_Aquaveil_TetraThreshold 
            = new("WHM_Aquaveil_TetraThreshold", 100);
        
        #endregion

        #endregion
    }
}