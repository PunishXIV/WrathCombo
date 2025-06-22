#region

using System.Numerics;
using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using ImGuiNET;
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
        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                #region Single Target DPS

                case CustomComboPreset.WHM_ST_MainCombo:
                    DrawAdditionalBoolChoice(WHM_ST_MainCombo_Adv,
                        "Advanced Action Options",
                        "Change how actions are handled",
                        isConditionalChoice: true);

                    if (WHM_ST_MainCombo_Adv)
                    {
                        ImGui.Indent();
                        ImGui.Spacing();
                        DrawHorizontalMultiChoice(WHM_ST_MainCombo_Adv_Actions,
                            "On Stone/Glare",
                            "Apply options to all Stones and Glares.",
                            3, 0);
                        DrawHorizontalMultiChoice(WHM_ST_MainCombo_Adv_Actions,
                            "On Aero/Dia",
                            "Apply options to Aeros and Dia.",
                            3, 1);
                        DrawHorizontalMultiChoice(WHM_ST_MainCombo_Adv_Actions,
                            $"On {Stone2.ActionName()}",
                            $"Apply options to On {Stone2.ActionName()}.",
                            3, 2);
                        ImGui.Unindent();
                    }

                    break;

                case CustomComboPreset.WHM_ST_MainCombo_Opener:
                    DrawBossOnlyChoice(WHM_Balance_Content);
                    break;

                case CustomComboPreset.WHM_ST_MainCombo_DoT:
                    DrawSliderInt(0, 50, WHM_ST_DPS_AeroOption,
                        targetStopUsingAtDescription,
                        itemWidth: medium);

                    ImGui.Indent();

                    ImGui.TextWrapped(
                        "Select what kind of enemies the HP check should be applied to:");
                    ImGui.NewLine();

                    DrawHorizontalRadioButton(WHM_ST_DPS_AeroOptionSubOption,
                        "Non-Bosses",
                        "Only applies the HP check above to non-bosses.\n" +
                        "Allows you to only stop DoTing early when it's not a boss.",
                        (int)BossAvoidance.On,
                        descriptionColor: ImGuiColors.DalamudWhite);

                    DrawHorizontalRadioButton(WHM_ST_DPS_AeroOptionSubOption,
                        "All Enemies",
                        "Applies the HP check above to all enemies.",
                        (int)BossAvoidance.Off,
                        descriptionColor: ImGuiColors.DalamudWhite);

                    DrawRoundedSliderFloat(0, 4, WHM_ST_MainCombo_DoT_Threshold,
                        reapplyTimeRemainingDescription,
                        itemWidth: little, digits: 1);

                    ImGui.Unindent();
                    break;

                case CustomComboPreset.WHM_ST_MainCombo_Lucid:
                    DrawSliderInt(4000, 9500, WHM_STDPS_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;

                #endregion

                #region AoE DPS

                case CustomComboPreset.WHM_AoE_DPS_Lucid:
                    DrawSliderInt(4000, 9500, WHM_AoEDPS_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;

                #endregion

                #region Single Target Heals

                case CustomComboPreset.WHM_STHeals:
                    DrawAdditionalBoolChoice(WHM_STHeals_IncludeShields,
                        "Include Shields in HP Percent Sliders",
                        "");
                    break;

                case CustomComboPreset.WHM_STHeals_Regen:
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0f, 6f, WHM_STHeals_RegenTimer,
                        reapplyTimeRemainingDescription,
                        itemWidth: little);
                    ImGui.Unindent();
                    DrawSliderInt(0, 100, WHM_STHeals_RegenHPLower,
                        targetStopUsingAtDescription);
                    DrawSliderInt(0, 100, WHM_STHeals_RegenHPUpper,
                        targetStartUsingAtDescription);
                    break;

                case CustomComboPreset.WHM_STHeals_Benediction:
                    DrawAdditionalBoolChoice(WHM_STHeals_BenedictionWeave,
                        weaveDescription, "");
                    DrawSliderInt(1, 100, WHM_STHeals_BenedictionHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 5, 0,
                        $"{Benediction.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.WHM_STHeals_ThinAir:
                    DrawSliderInt(0, 1, WHM_STHeals_ThinAir,
                        chargesToKeepDescription);
                    break;

                case CustomComboPreset.WHM_STHeals_Tetragrammaton:
                    DrawAdditionalBoolChoice(WHM_STHeals_TetraWeave,
                        weaveDescription, "");
                    DrawSliderInt(1, 100, WHM_STHeals_TetraHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 5, 1,
                        $"{Tetragrammaton.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.WHM_STHeals_Benison:
                    DrawAdditionalBoolChoice(WHM_STHeals_BenisonWeave,
                        weaveDescription, "");
                    DrawSliderInt(1, 100, WHM_STHeals_BenisonHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 5, 2,
                        $"{DivineBenison.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.WHM_STHeals_Aquaveil:
                    DrawAdditionalBoolChoice(WHM_STHeals_AquaveilWeave,
                        weaveDescription, "");
                    DrawSliderInt(1, 100, WHM_STHeals_AquaveilHP,
                        targetStartUsingAtDescription);
                    DrawPriorityInput(WHM_ST_Heals_Priority, 5, 3,
                        $"{Aquaveil.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.WHM_STHeals_Lucid:
                    DrawSliderInt(4000, 9500, WHM_STHeals_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;

                case CustomComboPreset.WHM_STHeals_Temperance:
                    DrawSliderInt(1, 100, WHM_STHeals_TemperanceHP,
                        targetStartUsingAtDescription);
                    DrawDifficultyMultiChoice(WHM_STHeals_TemperanceDifficulty, WHM_STHeals_TemperanceDifficultyListSet,
                        "Select what content difficulties Temperance should be used in:");
                    DrawAdditionalBoolChoice(WHM_STHeals_TemperanceWeave,
                        weaveDescription, "");
                    DrawPriorityInput(WHM_ST_Heals_Priority, 5, 4,
                        $"{Temperance.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.WHM_STHeals_Esuna:
                    DrawSliderInt(0, 100, WHM_STHeals_Esuna,
                        targetStopUsingAtDescription);
                    break;

                #endregion

                #region AoE Heals

                case CustomComboPreset.WHM_AoEHeals_ThinAir:
                    DrawSliderInt(0, 1, WHM_AoEHeals_ThinAir,
                        chargesToKeepDescription);
                    break;

                case CustomComboPreset.WHM_AoEHeals_Cure3:
                    DrawSliderInt(1500, 8500, WHM_AoEHeals_Cure3MP,
                        "MP to be over",
                        sliderIncrement: 500);
                    break;

                case CustomComboPreset.WHM_AoEHeals_Assize:
                    DrawAdditionalBoolChoice(WHM_AoEHeals_AssizeWeave,
                        weaveDescription, "");
                    break;

                case CustomComboPreset.WHM_AoEHeals_Plenary:
                    DrawAdditionalBoolChoice(WHM_AoEHeals_PlenaryWeave,
                        weaveDescription,
                        "");
                    break;

                case CustomComboPreset.WHM_AoEHeals_Temperance:
                    DrawSliderInt(1, 100, WHM_AoEHeals_TemperanceHP,
                        "Average party HP% to use at or below");
                    DrawDifficultyMultiChoice(WHM_AoEHeals_TemperanceDifficulty, WHM_AoEHeals_TemperanceDifficultyListSet,
                        "Select what content difficulties Temperance should be used in:");
                    ImGui.Spacing();
                    
                    DrawAdditionalBoolChoice(WHM_AoEHeals_TemperanceWeave,
                        weaveDescription,
                        "");
                    
                    DrawAdditionalBoolChoice(WHM_AoEHeals_TemperanceRaidwide,
                        "Also use for Raidwides",
                        "Will also use for mitigation before raidwides (and a healing boost after them), if the party is low enough.",
                        indentDescription: true);
                    if (WHM_AoEHeals_TemperanceRaidwide)
                    {
                        ImGui.Indent();
                        DrawDifficultyMultiChoice(WHM_AoEHeals_TemperanceRaidwideDifficulty, WHM_AoEHeals_TemperanceRaidwideDifficultyListSet,
                            "Select what content difficulties the Raidwide option should apply to:");
                    
                        DrawAdditionalBoolChoice(WHM_AoEHeals_TemperanceRaidwidePrioritization,
                            "Prioritize use for Raidwides",
                            "Will ignore the Party HP% check for Raidwides, essentially using Temperance for mitigation.\n" +
                            "Not advised in higher-end content.",
                            indentDescription: true);
                        ImGui.Unindent();
                    }
                    break;

                case CustomComboPreset.WHM_AoEHeals_Lucid:
                    DrawSliderInt(4000, 9500, WHM_AoEHeals_Lucid,
                        mpThresholdDescription,
                        itemWidth: medium, SliderIncrements.Hundreds);
                    break;

                case CustomComboPreset.WHM_AoEHeals_Medica2:
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0f, 6f, WHM_AoEHeals_MedicaTime,
                        reapplyTimeRemainingDescription,
                        itemWidth: little);
                    ImGui.Unindent();
                    break;

                case CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell:
                    DrawDifficultyMultiChoice(WHM_AoEHeals_LiturgyDifficulty, WHM_AoEHeals_LiturgyDifficultyListSet,
                        "Select what content difficulties Liturgy of the Bell should be used in:");
                    ImGui.NewLine();

                    DrawAdditionalBoolChoice(WHM_AoEHeals_LiturgyRaidwideOnly,
                        "Only use when a Raidwide is casting",
                        "Will not use Liturgy of the Bell in the rotation unless we detect a Raidwide is casting.",
                        indentDescription: true);

                    if (WHM_AoEHeals_LiturgyRaidwideOnly)
                    {
                        ImGuiEx.Spacing(new Vector2(30f, 0f));
                        ImGui.Text("Against what enemies should we check for Raidwides:");
                        ImGui.NewLine();
                        DrawRadioButton(
                            WHM_AoEHeals_LiturgyRaidwideOnlyBoss, "All Enemies",
                            "Will check for a Raidwide before using Bell at all times, on all enemies.",
                            outputValue: (int) BossRequirement.Off, itemWidth: 125f,
                            descriptionAsTooltip: true);
                        DrawRadioButton(
                            WHM_AoEHeals_LiturgyRaidwideOnlyBoss, "Only Bosses",
                            "Will try to only check for Raidwide when fighting bosses.\n" +
                            "(will use on cooldown versus regular enemies)\n" +
                            "(Note: don't rely on this 100%, square sometimes marks enemies inconsistently)",
                            outputValue: (int) BossRequirement.On, itemWidth: 125f,
                            descriptionAsTooltip: true);
                    }
                    break;

                case CustomComboPreset.WHM_AoEHeals_Asylum:
                    DrawAdditionalBoolChoice(WHM_AoEHeals_AsylumRaidwideOnly,
                        "Only use when a Raidwide is casting",
                        "Will not use Asylum in the rotation unless we detect a Raidwide is casting.");
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

        /// Bar Description for target HP% to start using plus disable text
        private const string targetStopUsingAtDescription =
            "Target HP% to stop using (0 = Use Always)";

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

        private const string mouseoverCheckingDescription =
            "Party UI Mouseover Checking";

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
        internal enum BossAvoidance
        {
            On = 0,
            Off = 1,
        }

        #endregion

        #region Options

        #region Single Target DPS

        /// <summary>
        ///     Enable advanced replacement action options for single target combo.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo" />
        internal static UserBool WHM_ST_MainCombo_Adv =
            new("WHM_ST_MainCombo_Adv");

        /// <summary>
        ///     Advanced action replacement options for main combo.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: [] <br />
        ///     <b>Options</b>: Boolean array for action replacement selections
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo" />
        public static UserBoolArray WHM_ST_MainCombo_Adv_Actions =
            new("WHM_ST_MainCombo_Adv_Actions");

        /// <summary>
        ///     Content type of Balance Opener.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: All Content<br />
        ///     <b>Options</b>: All Content or
        ///     <see cref="ContentCheck.IsInBossOnlyContent" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo_Opener" />
        internal static UserInt WHM_Balance_Content =
            new("WHM_Balance_Content", 0);

        /// <summary>
        ///     HP threshold to stop applying DoTs.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 50 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo_DoT" />
        internal static UserInt WHM_ST_DPS_AeroOption =
            new("WHM_ST_DPS_AeroOption");

        /// <summary>
        ///     Time threshold in seconds before reapplying DoT.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 4 <br />
        ///     <b>Step</b>: 0.1
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo_DoT" />
        internal static UserFloat WHM_ST_MainCombo_DoT_Threshold =
            new("WHM_ST_MainCombo_DoT_Threshold", 0);

        /// <summary>
        ///     Enemy type to apply the HP threshold check to.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="BossAvoidance.On" /> <br />
        ///     <b>Options</b>: <see cref="BossAvoidance">BossAvoidance Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo_DoT" />
        internal static UserInt WHM_ST_DPS_AeroOptionSubOption =
            new("WHM_ST_DPS_AeroOptionSubOption", (int)BossAvoidance.On);

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in single target rotations.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_ST_MainCombo_Lucid" />
        internal static UserInt WHM_STDPS_Lucid =
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
        /// <seealso cref="CustomComboPreset.WHM_AoE_DPS_Lucid" />
        internal static UserInt WHM_AoEDPS_Lucid =
            new("WHM_AoE_Lucid", 6500);

        #endregion

        #region Single Target Heals

        /// <summary>
        ///     Include shields when calculating HP percentages.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals" />
        internal static UserBool WHM_STHeals_IncludeShields =
            new("WHM_STHeals_IncludeShields", false);

        /// <summary>
        ///     Priority order for single target healing abilities.
        /// </summary>
        internal static UserIntArray WHM_ST_Heals_Priority =
            new("WHM_ST_Heals_Priority");

        /// <summary>
        ///     Time threshold in seconds before refreshing Regen.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 6 <br />
        ///     <b>Step</b>: 0.1
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Regen" />
        internal static UserFloat WHM_STHeals_RegenTimer =
            new("WHM_STHeals_RegenTimer", 0);

        /// <summary>
        ///     Lower HP threshold to stop using Regen.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Regen" />
        internal static UserInt WHM_STHeals_RegenHPLower =
            new("WHM_STHeals_RegenHPLower", 30);
        
        /// <summary>
        ///     Upper HP threshold to start using Regen.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Regen" />
        internal static UserInt WHM_STHeals_RegenHPUpper =
            new("WHM_STHeals_RegenHPUpper", 100);

        /// <summary>
        ///     Only use Benediction when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Benediction" />
        internal static UserBool WHM_STHeals_BenedictionWeave =
            new("WHM_STHeals_BenedictionWeave", false);

        /// <summary>
        ///     HP threshold to use Benediction.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Benediction" />
        internal static UserInt WHM_STHeals_BenedictionHP =
            new("WHM_STHeals_BenedictionHP", 40);

        /// <summary>
        ///     Number of Thin Air charges to reserve.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 1 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_ThinAir" />
        internal static UserInt WHM_STHeals_ThinAir =
            new("WHM_STHeals_ThinAir", 0);

        /// <summary>
        ///     Only use Tetragrammaton when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Tetragrammaton" />
        internal static UserBool WHM_STHeals_TetraWeave =
            new("WHM_STHeals_TetraWeave", false);

        /// <summary>
        ///     HP threshold to use Tetragrammaton.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Tetragrammaton" />
        internal static UserInt WHM_STHeals_TetraHP =
            new("WHM_STHeals_TetraHP", 50);

        /// <summary>
        ///     Only use Divine Benison when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Benison" />
        internal static UserBool WHM_STHeals_BenisonWeave =
            new("WHM_STHeals_BenisonWeave", false);

        /// <summary>
        ///     HP threshold to use Divine Benison.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Benison" />
        internal static UserInt WHM_STHeals_BenisonHP =
            new("WHM_STHeals_BenisonHP", 99);

        /// <summary>
        ///     HP threshold to use Aquaveil.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 99 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Aquaveil" />
        internal static UserInt WHM_STHeals_AquaveilHP =
            new("WHM_STHeals_AquaveilHP", 90);

        /// <summary>
        ///     Only use Aquaveil when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Aquaveil" />
        internal static UserBool WHM_STHeals_AquaveilWeave =
            new("WHM_STHeals_AquaveilWeave", false);

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in single target healing.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Lucid" />
        internal static UserInt WHM_STHeals_Lucid =
            new("WHM_STHeals_Lucid", 6500);

        /// <summary>
        ///     Only use Temperance when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Temperance" />
        internal static UserBool WHM_STHeals_TemperanceWeave =
            new("WHM_STHeals_TemperanceWeave", false);

        /// <summary>
        ///     HP threshold to use Temperance.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 75 <br />
        ///     <b>Range</b>: 1 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Temperance" />
        internal static UserInt WHM_STHeals_TemperanceHP =
            new("WHM_STHeals_TemperanceHP", 75);

        /// <summary>
        ///     Content difficulty selector for ST Temperance.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /> <br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Temperance" />
        internal static UserBoolArray WHM_STHeals_TemperanceDifficulty =
            new("WHM_STHeals_TemperanceDifficulty", [true, false]);

        /// <summary>
        ///     Content difficulty list set for ST Temperance, set by
        ///     <see cref="WHM_STHeals_TemperanceDifficulty" />.
        /// </summary>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Temperance" />
        internal static readonly ContentCheck.ListSet
            WHM_STHeals_TemperanceDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     HP threshold to stop using Esuna.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 40 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_STHeals_Esuna" />
        internal static UserInt WHM_STHeals_Esuna =
            new("WHM_Cure2_Esuna", 40);

        #endregion

        #region AoE Heals

        /// <summary>
        ///     Number of Thin Air charges to reserve in AoE healing.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 1 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_ThinAir" />
        internal static UserInt WHM_AoEHeals_ThinAir =
            new("WHM_AoE_ThinAir");

        /// <summary>
        ///     MP threshold to use Cure III.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 1500 - 8500 <br />
        ///     <b>Step</b>: 500
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Cure3" />
        internal static UserInt WHM_AoEHeals_Cure3MP =
            new("WHM_AoE_Cure3MP");

        /// <summary>
        ///     Only use Assize when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Assize" />
        internal static UserBool WHM_AoEHeals_AssizeWeave =
            new("WHM_AoEHeals_AssizeWeave");

        /// <summary>
        ///     Only use Plenary Indulgence when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Plenary" />
        internal static UserBool WHM_AoEHeals_PlenaryWeave =
            new("WHM_AoEHeals_PlenaryWeave");

        /// <summary>
        ///     Only use Temperance when weaving.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static UserBool WHM_AoEHeals_TemperanceWeave =
            new("WHM_AoEHeals_TemperanceWeave");

        /// <summary>
        ///     Will also use Temperance for Raidwides, if the party is low enough.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static UserBool WHM_AoEHeals_TemperanceRaidwide =
            new("WHM_AoEHeals_TemperanceRaidwide");

        /// <summary>
        ///     MP threshold to use Lucid Dreaming in AoE healing.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 6500 <br />
        ///     <b>Range</b>: 4000 - 9500 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Lucid" />
        internal static UserInt WHM_AoEHeals_Lucid =
            new("WHM_AoEHeals_Lucid", 6500);

        /// <summary>
        ///     Time threshold in seconds before refreshing Medica II/III.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 6 <br />
        ///     <b>Step</b>: 0.1
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Medica2" />
        internal static UserFloat WHM_AoEHeals_MedicaTime =
            new("WHM_AoEHeals_MedicaTime");

        /// <summary>
        ///     Only use Liturgy of the Bell vs a Raidwide.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell" />
        internal static UserBool WHM_AoEHeals_LiturgyRaidwideOnly =
            new("WHM_AoEHeals_LiturgyRaidwideOnly");

        /// <summary>
        ///     Boss Requirement for Liturgy of the Bell, to only check for raidwides
        ///     in boss fights, or not.<br />
        ///     Raidwides check controlled by
        ///     <see cref="WHM_AoEHeals_LiturgyRaidwideOnly" />.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="BossRequirement.On" /> <br />
        ///     <b>Options</b>: <see cref="BossRequirement">BossRequirement Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell" />
        public static readonly UserInt
            WHM_AoEHeals_LiturgyRaidwideOnlyBoss =
                new("WHM_AoEHeals_LiturgyRaidwideOnlyBoss", (int) BossRequirement.On);

        /// <summary>
        ///     Content difficulty selector for Liturgy of the Bell.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /> <br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell" />
        internal static UserBoolArray WHM_AoEHeals_LiturgyDifficulty =
            new("WHM_AoEHeals_LiturgyDifficulty", [true, false]);

        /// <summary>
        ///     Content difficulty list set for Liturgy of the Bell, set by
        ///     <see cref="WHM_AoEHeals_LiturgyDifficulty" />.
        /// </summary>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell" />
        internal static readonly ContentCheck.ListSet
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
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static UserInt WHM_AoEHeals_TemperanceHP =
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
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static UserBoolArray WHM_AoEHeals_TemperanceDifficulty =
            new("WHM_AoEHeals_TemperanceDifficulty", [true, true]);

        /// <summary>
        ///     Content difficulty list set for Temperance, set by
        ///     <see cref="WHM_AoEHeals_TemperanceDifficulty" />.
        /// </summary>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static readonly ContentCheck.ListSet
            WHM_AoEHeals_TemperanceDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     Content difficulty selector for Temperance raidwide option.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /><br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static UserBoolArray WHM_AoEHeals_TemperanceRaidwideDifficulty =
            new("WHM_AoEHeals_TemperanceRaidwideDifficulty", [true, false]);

        /// <summary>
        ///     Content difficulty list set for Temperance raidwide option, set by
        ///     <see cref="WHM_AoEHeals_TemperanceRaidwideDifficulty" />.
        /// </summary>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static readonly ContentCheck.ListSet
            WHM_AoEHeals_TemperanceRaidwideDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     Prioritizes Temperance for raidwides, by ignoring the HP% check then.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Temperance" />
        internal static UserBool WHM_AoEHeals_TemperanceRaidwidePrioritization =
            new("WHM_AoEHeals_TemperanceRaidwidePrioritization");

        /// <summary>
        ///     Only use Asylum vs a Raidwide.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="CustomComboPreset.WHM_AoEHeals_Asylum" />
        internal static UserBool WHM_AoEHeals_AsylumRaidwideOnly =
            new("WHM_AoEHeals_AsylumRaidwideOnly");

        #endregion

        #endregion
    }
}
