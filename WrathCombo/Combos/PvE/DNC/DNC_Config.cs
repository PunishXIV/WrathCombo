#region

using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DNC
{
    internal static class Config
    {
        /// <summary>
        ///     Draw the Anti-Drift options for the Single-Target Standard Step
        ///     option.
        /// </summary>
        private static void DrawAntiDriftOptions()
        {
            ImGui.Dummy(new Vector2(1f, 12f));
            ImGui.Indent(40f);
            ImGui.Text("Anti-Drift Options:     (hover each for more info)");
            ImGui.Unindent(40f);
            ImGui.NewLine();

            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "Forced Triple Weave",
                "Forces a triple weave of Flourish and Fan Dance 3 + 4 during non-opener burst windows." +
                "\nFixes SS/FM drift where you use a gcd when SS/FM is on a 0.5sec CD." +
                "\nRecommended anti-drift option.",
                outputValue: (int) AntiDrift.TripleWeave, descriptionAsTooltip: true);
            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "Hold before Standard Step",
                "Will hold GCDs for Standard Step if it is going to come off cooldown before your next GCD." +
                "\nThis WILL give you down-time." +
                "\nONLY recommended if you have extra skill speed, but can be used as an anti-drift option.",
                outputValue: (int) AntiDrift.Hold, descriptionAsTooltip: true);
            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "Both",
                "Will use both options from above." +
                "\nThis WILL give you down-time." +
                "\nNOT recommended, but can be the answer if neither of the above options work for you.",
                outputValue: (int) AntiDrift.Both, descriptionAsTooltip: true);
            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "None",
                "Will not use any anti-drift options." +
                "\nThis WILL cause drift. NOT recommended.",
                outputValue: (int) AntiDrift.None, descriptionAsTooltip: true);
        }

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.DNC_CustomDanceSteps:
                    ImGui.Indent(35f.Scale());

                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                    ImGui.TextWrapped(
                        "NO SUPPORT is provided for setting up this feature!");
                    ImGui.PopStyleColor();

                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                    ImGui.TextWrapped(
                        "\nYou can change the respective actions by inputting action IDs below for each dance step." +
                        "\nThe defaults are Cascade, Flourish, Fan Dance, and Fan Dance II." +
                        "\nIf set to 0, they will reset to these actions." +
                        "\n(You can get Action IDs with Garland Tools by searching for the action and clicking the cog.)");
                    ImGui.PopStyleColor();

                    int[] actions = Service.Configuration.DancerDanceCompatActionIDs
                        .Select(x => (int) x).ToArray();

                    bool inputChanged = false;
                    ImGui.SetNextItemWidth(50f.Scale());
                    inputChanged |= ImGui.InputInt(
                        "(Red) Emboite replacement Action ID",
                        ref actions[0], 0);
                    ImGui.SetNextItemWidth(50f.Scale());
                    inputChanged |= ImGui.InputInt(
                        "(Blue) Entrechat replacement Action ID",
                        ref actions[1], 0);
                    ImGui.SetNextItemWidth(50f.Scale());
                    inputChanged |= ImGui.InputInt(
                        "(Green) Jete replacement Action ID",
                        ref actions[2], 0);
                    ImGui.SetNextItemWidth(50f.Scale());
                    inputChanged |= ImGui.InputInt(
                        "(Yellow) Pirouette replacement Action ID",
                        ref actions[3], 0);

                    ImGui.Dummy(new Vector2(0f, 12f.Scale()));
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                    ImGui.TextWrapped(
                        "This WILL let you set up a conflict!");
                    ImGui.PopStyleColor();
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                    ImGui.TextWrapped("Double check the actions you are setting do not conflict with other combos you are using!");
                    ImGui.PopStyleColor();

                    if (inputChanged)
                    {
                        Service.Configuration.DancerDanceCompatActionIDs = actions
                            .Select(x => (uint) x).ToArray();
                        Service.Configuration.Save();
                    }

                    ImGui.Unindent(35f.Scale());
                    ImGui.Spacing();

                    break;

                #region Advanced Single Target UI

                case CustomComboPreset.DNC_ST_BalanceOpener:
                    UserConfig.DrawRadioButton(DNC_ST_OpenerSelection,
                        "15s Countdown",
                        "Requires at least a 15s cooldown\nand that you start Standard Step at 15s.",
                        0, descriptionAsTooltip: true);
                    UserConfig.DrawRadioButton(DNC_ST_OpenerSelection,
                        "7s Countdown",
                        "Requires at least a 7s cooldown\nand that you start Standard Step at 7s.\nPerforms worse than 15s.",
                        1, descriptionAsTooltip: true);
                    UserConfig.DrawBossOnlyChoice(DNC_ST_OpenerDifficulty);

                    break;

                case CustomComboPreset.DNC_ST_EspritOvercap:
                    UserConfig.DrawSliderInt(50, 100, DNCEspritThreshold_ST,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_ST_Adv_SS:
                    UserConfig.DrawSliderInt(0, 15, DNC_ST_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_SS_IncludeSS,
                        "Include Standard Step",
                        "Will include Standard Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int) IncludeStep.Yes,
                        itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_SS_IncludeSS,
                        "Exclude Standard Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Standard Step.",
                        outputValue: (int) IncludeStep.No,
                        itemWidth: 125f);

                    DrawAntiDriftOptions();

                    break;

                case CustomComboPreset.DNC_ST_Adv_TS:
                    UserConfig.DrawSliderInt(0, 15, DNC_ST_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_TS_IncludeTS,
                        "Include Technical Step",
                        "Will include Technical Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int) IncludeStep.Yes,
                        itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_TS_IncludeTS,
                        "Exclude Technical Step",
                        "Will ONLY include the dance Steps, and Finish;" +
                        "\nYOU will need to manually press Technical Step.",
                        outputValue: (int) IncludeStep.No,
                        itemWidth: 125f);

                    DrawAntiDriftOptions();

                    break;

                case CustomComboPreset.DNC_ST_Adv_Feathers:
                    UserConfig.DrawSliderInt(0, 5, DNC_ST_Adv_FeatherBurstPercent,
                        "Target HP% to dump all pooled feathers below",
                        itemWidth: 75f);

                    break;

                case CustomComboPreset.DNC_ST_Adv_Tillana:
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_TillanaUse,
                        "Use Tillana Normally",
                        "Will use Tillana as recommended by The Balance" +
                        "\nCan allow Tillana to drift out of burst windows.",
                        outputValue: (int) TillanaDriftProtection.None,
                        itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_TillanaUse,
                        "Favor Tillana over Esprit",
                        "Will perform Tillana over Saber or Dance of the Dawn, even if above 50 Esprit." +
                        "\nCan prevent Tillana from drifting out of burst windows." +
                        "\nShould be used with Saber Dance's Esprit slider being >50." +
                        "\nNOT recommended.",
                        outputValue: (int) TillanaDriftProtection.Favor,
                        itemWidth: 125f);

                    break;

                case CustomComboPreset.DNC_ST_Adv_SaberDance:
                    UserConfig.DrawSliderInt(50, 100,
                        DNC_ST_Adv_SaberThreshold,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_ST_Adv_PanicHeals:
                    UserConfig.DrawSliderInt(0, 80,
                        DNC_ST_Adv_PanicHealWaltzPercent,
                        "Curing Waltz HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawSliderInt(0, 80, DNC_ST_Adv_PanicHealWindPercent,
                        "Second Wind HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    break;

                #endregion

                #region Advanced AoE UI

                case CustomComboPreset.DNC_AoE_EspritOvercap:
                    UserConfig.DrawSliderInt(50, 100, DNCEspritThreshold_AoE,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_SS:
                    UserConfig.DrawSliderInt(0, 60, DNC_AoE_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_SS_IncludeSS,
                        "Include Standard Step",
                        "Will include Standard Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int) IncludeStep.Yes,
                        itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_SS_IncludeSS,
                        "Exclude Standard Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Standard Step.",
                        outputValue: (int) IncludeStep.No,
                        itemWidth: 125f);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_TS:
                    UserConfig.DrawSliderInt(0, 60, DNC_AoE_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_TS_IncludeTS,
                        "Include Technical Step",
                        "Will include Technical Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int) IncludeStep.Yes,
                        itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_TS_IncludeTS,
                        "Exclude Technical Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Technical Step.",
                        outputValue: (int) IncludeStep.No,
                        itemWidth: 125f);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_SaberDance:
                    UserConfig.DrawSliderInt(50, 100, DNC_AoE_Adv_SaberThreshold,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_PanicHeals:
                    UserConfig.DrawSliderInt(0, 80,
                        DNC_AoE_Adv_PanicHealWaltzPercent,
                        "Curing Waltz HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawSliderInt(0, 80,
                        DNC_AoE_Adv_PanicHealWindPercent,
                        "Second Wind HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    break;

                #endregion

                case CustomComboPreset.DNC_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 80, DNCVariantCurePercent,
                        "HP% to be at or under",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    break;

                #region PVP

                case CustomComboPreset.DNCPvP_BurstMode_CuringWaltz:
                    UserConfig.DrawSliderInt(0, 90,
                        DNCPvP.Config.DNCPvP_WaltzThreshold,
                        "Curing Waltz HP% - caps at 90 to prevent waste.");

                    break;

                case CustomComboPreset.DNCPvP_BurstMode_Dash:
                    UserConfig.DrawSliderInt(0, 3,
                        DNCPvP.Config.DNCPvP_EnAvantCharges,
                        "How many to save for manual");

                    break;

                    #endregion
            }
        }

        #region Constants

        public enum Openers
        {
            FifteenSecond,
            SevenSecond,
        }

        public enum IncludeStep
        {
            No,
            Yes,
        }

        public enum TillanaDriftProtection
        {
            None,
            Favor,
        }

        public enum AntiDrift
        {
            None,
            TripleWeave,
            Hold,
            Both,
        }

        #endregion

        #region Options

        #region Advanced Single Target

        /// <summary>
        ///     Difficulty of Opener for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.IsInBossOnlyContent" /> <br />
        ///     <b>Options</b>: All Content or
        ///     <see cref="ContentCheck.IsInBossOnlyContent" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_BalanceOpener" />
        public static readonly UserBoolArray DNC_ST_OpenerDifficulty =
            new("DNC_ST_OpenerDifficulty", [false, true]);

        /// <summary>
        ///     Opener selection for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="Openers.FifteenSecond" /> <br />
        ///     <b>Options</b>: <see cref="Openers">Openers Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_BalanceOpener" />
        public static readonly UserInt DNC_ST_OpenerSelection =
            new("DNC_ST_OpenerSelection", (int) Openers.FifteenSecond);

        /// <summary>
        ///     Esprit threshold for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_EspritOvercap" />
        public static readonly UserInt DNCEspritThreshold_ST =
            new("DNCEspritThreshold_ST", 50);

        /// <summary>
        ///     Target HP% to use Standard Step above for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 15 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_SS" />
        public static readonly UserInt DNC_ST_Adv_SSBurstPercent =
            new("DNC_ST_Adv_SSBurstPercent", 0);

        /// <summary>
        ///     Include Standard Step in rotation for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_SS" />
        public static readonly UserInt DNC_ST_ADV_SS_IncludeSS =
            new("DNC_ST_ADV_SS_IncludeSS", (int) IncludeStep.Yes);

        /// <summary>
        ///     Anti-Drift choice for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="AntiDrift.TripleWeave" /> <br />
        ///     <b>Options</b>: <see cref="AntiDrift">AntiDrift Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_SS" />
        public static readonly UserInt DNC_ST_ADV_AntiDrift =
            new("DNC_ST_ADV_AntiDrift", (int) AntiDrift.TripleWeave);

        /// <summary>
        ///     Include Technical Step in rotation for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_TS" />
        public static readonly UserInt DNC_ST_ADV_TS_IncludeTS =
            new("DNC_ST_ADV_TS_IncludeTS", (int) IncludeStep.Yes);

        /// <summary>
        ///     Target HP% to use Technical Step above for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 15 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_TS" />
        public static readonly UserInt DNC_ST_Adv_TSBurstPercent =
            new("DNC_ST_Adv_TSBurstPercent", 0);

        /// <summary>
        ///     Target HP% to dump all pooled feathers below for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 5 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_Feathers" />
        public static readonly UserInt DNC_ST_Adv_FeatherBurstPercent =
            new("DNC_ST_Adv_FeatherBurstPercent", 0);

        /// <summary>
        ///     Tillana drift protection for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="TillanaDriftProtection.None" /> <br />
        ///     <b>Options</b>: <see cref="TillanaDriftProtection" /> Enum.
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_Tillana" />
        public static readonly UserInt DNC_ST_ADV_TillanaUse =
            new("DNC_ST_ADV_TillanaUse", (int) TillanaDriftProtection.None);

        /// <summary>
        ///     Esprit threshold for Saber Dance in Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_SaberDance" />
        public static readonly UserInt DNC_ST_Adv_SaberThreshold =
            new("DNC_ST_Adv_SaberThreshold", 50);

        /// <summary>
        ///     Player HP% threshold for Curing Waltz in Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 0 - 80 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_PanicHeals" />
        public static readonly UserInt DNC_ST_Adv_PanicHealWaltzPercent =
            new("DNC_ST_Adv_PanicHealWaltzPercent", 30);

        /// <summary>
        ///     Player HP% threshold for Second Wind in Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 20 <br />
        ///     <b>Range</b>: 0 - 80 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_ST_Adv_PanicHeals" />
        public static readonly UserInt DNC_ST_Adv_PanicHealWindPercent =
            new("DNC_ST_Adv_PanicHealWindPercent", 20);

        #endregion

        #region Advanced AoE

        /// <summary>
        ///     Esprit threshold for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_EspritOvercap" />
        public static readonly UserInt DNCEspritThreshold_AoE =
            new("DNCEspritThreshold_AoE", 50);

        /// <summary>
        ///     Target HP% to use Standard Step above for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 40 <br />
        ///     <b>Range</b>: 0 - 60 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_SS" />
        public static readonly UserInt DNC_AoE_Adv_SSBurstPercent =
            new("DNC_AoE_Adv_SSBurstPercent", 40);

        /// <summary>
        ///     Include Standard Step in rotation for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_SS" />
        public static readonly UserInt DNC_AoE_Adv_SS_IncludeSS =
            new("DNC_AoE_Adv_SS_IncludeSS", (int) IncludeStep.Yes);

        /// <summary>
        ///     Target HP% to use Technical Step above for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 40 <br />
        ///     <b>Range</b>: 0 - 60 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_TS" />
        public static readonly UserInt DNC_AoE_Adv_TSBurstPercent =
            new("DNC_AoE_Adv_TSBurstPercent", 40);

        /// <summary>
        ///     Include Technical Step in rotation for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_TS" />
        public static readonly UserInt DNC_AoE_Adv_TS_IncludeTS =
            new("DNC_AoE_Adv_TS_IncludeTS", (int) IncludeStep.Yes);

        /// <summary>
        ///     Esprit threshold for Saber Dance in AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_SaberDance" />
        public static readonly UserInt DNC_AoE_Adv_SaberThreshold =
            new("DNC_AoE_Adv_SaberThreshold", 50);

        /// <summary>
        ///     Player HP% threshold for Curing Waltz in AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 30 <br />
        ///     <b>Range</b>: 0 - 80 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_PanicHeals" />
        public static readonly UserInt DNC_AoE_Adv_PanicHealWaltzPercent =
            new("DNC_AoE_Adv_PanicHealWaltzPercent", 30);

        /// <summary>
        ///     Player HP% threshold for Second Wind in AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 20 <br />
        ///     <b>Range</b>: 0 - 80 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_AoE_Adv_PanicHeals" />
        public static readonly UserInt DNC_AoE_Adv_PanicHealWindPercent =
            new("DNC_AoE_Adv_PanicHealWindPercent", 20);

        #endregion

        /// <summary>
        ///     HP% threshold for Variant Cure.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 1 <br />
        ///     <b>Range</b>: 1 - 80
        /// </value>
        /// <seealso cref="CustomComboPreset.DNC_Variant_Cure" />
        public static readonly UserInt DNCVariantCurePercent =
            new("DNCVariantCurePercent", 20);

        #endregion
    }
}
