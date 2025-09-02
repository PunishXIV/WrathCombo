#region

using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using System.Linq;
using System.Numerics;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;

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
            ImGuiEx.Spacing(new Vector2(40, 12));
            ImGui.Text("Anti-Drift Options:     (hover each for more info)");

            #region Show a colored display of the user's current detected GCD

            var color = GCDValue switch
            {
                GCDRange.Perfect => ImGuiColors.HealerGreen,
                GCDRange.NotGood => ImGuiColors.DalamudYellow,
                _ => ImGuiColors.DalamudRed,
            };
            ImGui.SameLine();
            ImGui.Text("GCD: ");
            ImGui.SameLine();
            ImGui.TextColored(color, $"{GCD:0.00}");
            ImGui.NewLine();
            #endregion

            var t = ImGui.GetCursorPos();
            const string texTrip = "Forced Triple Weave";
            DrawRadioButton(
                DNC_ST_ADV_AntiDrift, texTrip,
                "Forces a triple weave of Flourish and Fan Dance 3 + 4 during non-opener burst windows." +
                "\nFixes SS/FM drift where you use a gcd when SS/FM is on a 0.5sec CD." +
                "\nRecommended anti-drift option.",
                outputValue: (int)AntiDrift.TripleWeave, descriptionAsTooltip: true);
            var h = ImGui.GetCursorPos();
            const string texHold = "Hold before Standard Step";
            DrawRadioButton(
                DNC_ST_ADV_AntiDrift, texHold,
                "Will hold GCDs for Standard Step if it is going to come off cooldown before your next GCD." +
                "\nThis WILL give you down-time." +
                "\nONLY recommended if you have extra skill speed, but can be used as an anti-drift option.",
                outputValue: (int)AntiDrift.Hold, descriptionAsTooltip: true);
            DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "Both",
                "Will use both options from above." +
                "\nThis WILL give you down-time." +
                "\nNOT recommended, but can be the answer if neither of the above options work for you.",
                outputValue: (int)AntiDrift.Both, descriptionAsTooltip: true);
            DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "None",
                "Will not use any anti-drift options." +
                "\nThis WILL cause drift. NOT recommended.",
                outputValue: (int)AntiDrift.None, descriptionAsTooltip: true);

            #region Show recommended setting, based on GCD

            // Save the current cursor position
            var pos = ImGui.GetCursorPos();

            // Determine which recommendation text to show
            const string rec = "(Recommended)";
            var recTriple = GCDValue is GCDRange.Perfect ? rec : "";
            var recHold = GCDValue is not GCDRange.Perfect ? rec : "";

            // Set the position of (any) Triple-Weave recommendation text
            var texSize = ImGui.CalcTextSize(texHold);
            ImGui.SetCursorPos(
                t with { X = t.X + texSize.X + 110f.Scale(), Y = t.Y - texSize.Y - 2f.Scale() });
            ImGui.TextColored(ImGuiColors.DalamudGrey, recTriple);

            // Set the position of (any) Hold recommendation text
            ImGui.SetCursorPos(
                h with { X = h.X + texSize.X + 110f.Scale(), Y = h.Y - 2f.Scale() });
            ImGui.TextColored(ImGuiColors.DalamudGrey, recHold);

            // Reset to where the cursor was
            ImGui.SetCursorPos(pos);

            #endregion
        }

        private static void DrawPartnerInfo()
        {
            ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey,
                "This will check through your party members, and select the most desirable Partner, based on The Balance's priority list as well as stuff like Rez Sickness and Damage Downs.");
        }

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.DNC_CustomDanceSteps:
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
                        .Select(x => (int)x).ToArray();

                    bool inputChanged = false;
                    ImGuiEx.SetNextItemWidthScaled(50);
                    inputChanged |= ImGui.InputInt(
                        "(Red) Emboite replacement Action ID",
                        ref actions[0], 0);
                    ImGuiEx.SetNextItemWidthScaled(50);
                    inputChanged |= ImGui.InputInt(
                        "(Blue) Entrechat replacement Action ID",
                        ref actions[1], 0);
                    ImGuiEx.SetNextItemWidthScaled(50);
                    inputChanged |= ImGui.InputInt(
                        "(Green) Jete replacement Action ID",
                        ref actions[2], 0);
                    ImGuiEx.SetNextItemWidthScaled(50);
                    inputChanged |= ImGui.InputInt(
                        "(Yellow) Pirouette replacement Action ID",
                        ref actions[3], 0);

                    ImGuiEx.Spacing(new Vector2(0, 12));

                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                    ImGui.TextWrapped(
                        "This WILL let you set up a conflict!");
                    ImGui.PopStyleColor();
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                    ImGui.TextWrapped("Double check the actions you are setting do not conflict with other combos you are using, or enable the feature below!");
                    ImGui.PopStyleColor();

                    if (inputChanged)
                    {
                        Service.Configuration.DancerDanceCompatActionIDs = actions
                            .Select(x => (uint)x).ToArray();
                        Service.Configuration.Save();
                    }

                    ImGui.Unindent(35f.Scale());
                    ImGui.Spacing();

                    break;

                #region Advanced Single Target UI

                case Preset.DNC_ST_BalanceOpener:
                    ImGui.Indent();
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                    ImGui.TextWrapped(
                        "Opener variations:     (hover each for more info)");
                    ImGui.PopStyleColor();
                    ImGui.Unindent();

                    ImGui.NewLine();
                    DrawRadioButton(DNC_ST_OpenerSelection,
                        "Standard: 15s Countdown",
                        "Requires at least a 15s cooldown\nand that you start Standard Step at 15s.",
                        (int)Openers.FifteenSecond, descriptionAsTooltip: true);
                    DrawRadioButton(DNC_ST_OpenerSelection,
                        "Standard: 7s Countdown",
                        "Requires at least a 7s cooldown\nand that you start Standard Step at 7s.\nPerforms worse than 15s.",
                        (int)Openers.SevenSecond, descriptionAsTooltip: true);
                    DrawRadioButton(DNC_ST_OpenerSelection,
                        "Technical: 30s Countdown",
                        "Requires a 30s cooldown\nand that you start Standard Step at 30s.\nNot generally recommended.\nWill align buffs worse than Standard 15s.",
                        (int)Openers.ThirtySecondTech, descriptionAsTooltip: true);
                    DrawRadioButton(DNC_ST_OpenerSelection,
                        "Technical: 7+s Countdown",
                        "Requires at least a 7s cooldown\nand that you complete Standard Step beforehand.\nDoes not include Peloton.\nWill NOT Standard Step for you.\nNot generally recommended.\nWill align buffs worse than Standard 7s.",
                        (int)Openers.SevenPlusSecondTech, descriptionAsTooltip: true);
                    DrawRadioButton(DNC_ST_OpenerSelection,
                        "Technical: 7s Countdown",
                        "Requires at least a 7s cooldown\nand that you start Technical Step at 7s.\nNOT recommended.",
                        (int)Openers.SevenSecondTech, descriptionAsTooltip: true);

                    ImGui.Indent();
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                    ImGui.TextWrapped(
                        "Opener options:");
                    ImGui.PopStyleColor();

                    DrawAdditionalBoolChoice(DNC_ST_OpenerOption_Peloton,
                        $"Include {Peloton.ActionName()}", "");

                    DrawBossOnlyChoice(DNC_ST_OpenerDifficulty, "Select what kind of content to use this opener in:");
                    ImGui.Unindent();

                    break;

                case Preset.DNC_ST_Adv_PartnerAuto:
                    DrawAdditionalBoolChoice(DNC_Partner_FocusOverride,
                        "Prioritize your Focus Target##DPFocusOver0",
                        "If you have a focus target that is within range, it will be prioritized over The Balance's suggested Dance Partner.",
                        indentDescription: true);

                    break;

                case Preset.DNC_ST_Adv_AutoPartner:
                    ImGui.Indent(29f.Scale());
                    DrawPartnerInfo();
                    ImGui.Unindent(29f.Scale());

                    DrawAdditionalBoolChoice(DNC_Partner_FocusOverride,
                        "Prioritize your Focus Target##DPFocusOver1",
                        "If you have a focus target that is within range, alive, and has no rez sickness or damage down, it will be prioritized over The Balance's suggested Dance Partner.",
                        indentDescription: true);

                    break;

                case Preset.DNC_ST_EspritOvercap:
                    DrawSliderInt(50, 100, DNCEspritThreshold_ST,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case Preset.DNC_ST_Adv_SS:
                    DrawSliderInt(0, 15, DNC_ST_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    ImGuiEx.Spacing(new Vector2(30, 0));
                    DrawHorizontalRadioButton(
                        DNC_ST_ADV_SS_IncludeSS,
                        "Include Standard Step",
                        "Will include Standard Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes,
                        itemWidth: 125f);
                    DrawHorizontalRadioButton(
                        DNC_ST_ADV_SS_IncludeSS,
                        "Exclude Standard Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Standard Step.",
                        outputValue: (int)IncludeStep.No,
                        itemWidth: 125f);

                    DrawAntiDriftOptions();

                    break;

                case Preset.DNC_ST_Adv_TS:
                    DrawSliderInt(0, 15, DNC_ST_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    ImGuiEx.Spacing(new Vector2(30, 0));
                    DrawHorizontalRadioButton(
                        DNC_ST_ADV_TS_IncludeTS,
                        "Include Technical Step",
                        "Will include Technical Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes,
                        itemWidth: 125f);
                    DrawHorizontalRadioButton(
                        DNC_ST_ADV_TS_IncludeTS,
                        "Exclude Technical Step",
                        "Will ONLY include the dance Steps, and Finish;" +
                        "\nYOU will need to manually press Technical Step.",
                        outputValue: (int)IncludeStep.No,
                        itemWidth: 125f);

                    DrawAntiDriftOptions();

                    break;

                case Preset.DNC_ST_Adv_Feathers:
                    DrawSliderInt(0, 5, DNC_ST_Adv_FeatherBurstPercent,
                        "Target HP% to dump all pooled feathers below",
                        itemWidth: 75f);

                    break;

                case Preset.DNC_ST_Adv_Tillana:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(
                        DNC_ST_ADV_TillanaUse,
                        "Use Tillana Normally",
                        "Will use Tillana as recommended by The Balance" +
                        "\nCan allow Tillana to drift out of burst windows.",
                        outputValue: (int)TillanaDriftProtection.None,
                        itemWidth: 125f);
                    DrawHorizontalRadioButton(
                        DNC_ST_ADV_TillanaUse,
                        "Favor Tillana over Esprit",
                        "Will perform Tillana over Saber or Dance of the Dawn, even if above 50 Esprit." +
                        "\nCan prevent Tillana from drifting out of burst windows." +
                        "\nShould be used with Saber Dance's Esprit slider being >50." +
                        "\nNOT recommended.",
                        outputValue: (int)TillanaDriftProtection.Favor,
                        itemWidth: 125f);
                    ImGui.Unindent();

                    break;

                case Preset.DNC_ST_Adv_SaberDance:
                    DrawSliderInt(50, 100,
                        DNC_ST_Adv_SaberThreshold,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case Preset.DNC_ST_Adv_PanicHeals:
                    DrawSliderInt(0, 80,
                        DNC_ST_Adv_PanicHealWaltzPercent,
                        "Curing Waltz HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    DrawSliderInt(0, 80, DNC_ST_Adv_PanicHealWindPercent,
                        "Second Wind HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    break;

                #endregion

                #region Advanced AoE UI

                case Preset.DNC_AoE_EspritOvercap:
                    DrawSliderInt(50, 100, DNCEspritThreshold_AoE,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case Preset.DNC_AoE_Adv_SS:
                    DrawSliderInt(0, 60, DNC_AoE_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    ImGuiEx.Spacing(new Vector2(30, 0));
                    DrawHorizontalRadioButton(
                        DNC_AoE_Adv_SS_IncludeSS,
                        "Include Standard Step",
                        "Will include Standard Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes,
                        itemWidth: 125f);
                    DrawHorizontalRadioButton(
                        DNC_AoE_Adv_SS_IncludeSS,
                        "Exclude Standard Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Standard Step.",
                        outputValue: (int)IncludeStep.No,
                        itemWidth: 125f);

                    break;

                case Preset.DNC_AoE_Adv_TS:
                    DrawSliderInt(0, 60, DNC_AoE_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below",
                        itemWidth: 75f, sliderIncrement: SliderIncrements.Fives);

                    ImGuiEx.Spacing(new Vector2(30, 0));
                    DrawHorizontalRadioButton(
                        DNC_AoE_Adv_TS_IncludeTS,
                        "Include Technical Step",
                        "Will include Technical Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes,
                        itemWidth: 125f);
                    DrawHorizontalRadioButton(
                        DNC_AoE_Adv_TS_IncludeTS,
                        "Exclude Technical Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Technical Step.",
                        outputValue: (int)IncludeStep.No,
                        itemWidth: 125f);

                    break;

                case Preset.DNC_AoE_Adv_SaberDance:
                    DrawSliderInt(50, 100, DNC_AoE_Adv_SaberThreshold,
                        "Esprit",
                        itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                    break;

                case Preset.DNC_AoE_Adv_PanicHeals:
                    DrawSliderInt(0, 80,
                        DNC_AoE_Adv_PanicHealWaltzPercent,
                        "Curing Waltz HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    DrawSliderInt(0, 80,
                        DNC_AoE_Adv_PanicHealWindPercent,
                        "Second Wind HP%",
                        itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);

                    break;

                #endregion

                case Preset.DNC_DesirablePartner:
                    ImGui.Indent(35f.Scale());
                    DrawPartnerInfo();
                    ImGui.Unindent(35f.Scale());
                    ImGuiEx.Spacing(new Vector2(0, 12));

                    DrawAdditionalBoolChoice(DNC_Partner_FocusOverride,
                        "Prioritize your Focus Target##DPFocusOver2",
                        "If you have a focus target that is within range, alive, and has no rez sickness or damage down, it will be prioritized over The Balance's suggested Dance Partner.",
                        indentDescription: true);

                    ImGuiEx.Spacing(new Vector2(29, 12));
                    ImGui.Text("Action to Show when Partner is Optimal Options:     (hover each for more info)");
                    ImGui.NewLine();
                    DrawRadioButton(
                        DNC_Partner_ActionToShow, "Let Game Decide",
                        "Will not change the action shown in the hotbar from what FFXIV puts there.\n" +
                        "When you have a Dance Partner, it will show Ending, as usual.\n\n" +
                        "This is the default behavior.",
                        outputValue: (int)PartnerShowAction.Default,
                        descriptionAsTooltip: true);
                    DrawRadioButton(
                        DNC_Partner_ActionToShow, "Closed Position",
                        "When your current partner is optimal Closed Position will be shown.\n" +
                        "This will block you from using Closed Position or Ending\n(unless you hard target a friendly other than your partner).\n\n" +
                        "This is less distracting than the Savage Blade option.",
                        outputValue: (int)PartnerShowAction.ClosedPosition,
                        descriptionAsTooltip: true);
                    DrawRadioButton(
                        DNC_Partner_ActionToShow, "Savage Blade",
                        "When your current partner is optimal Savage Blade will be shown.\n" +
                        "Savage Blade is a removed action that we use to block input.\n" +
                        "This will block you from using Closed Position or Ending.\n\n" +
                        "This is the recommended option, to keep you from mistakenly switching partners.",
                        outputValue: (int)PartnerShowAction.SavageBlade,
                        descriptionAsTooltip: true);

                    break;

            }
        }

        #region Constants

        public enum Openers
        {
            FifteenSecond,
            SevenSecond,
            ThirtySecondTech,
            SevenPlusSecondTech,
            SevenSecondTech,
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
        /// <seealso cref="Preset.DNC_ST_BalanceOpener" />
        public static readonly UserBoolArray DNC_ST_OpenerDifficulty =
            new("DNC_ST_OpenerDifficulty", [false, true]);

        /// <summary>
        ///     Opener selection for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="Openers.FifteenSecond" /> <br />
        ///     <b>Options</b>: <see cref="Openers">Openers Enum</see>
        /// </value>
        /// <seealso cref="Preset.DNC_ST_BalanceOpener" />
        public static readonly UserInt DNC_ST_OpenerSelection =
            new("DNC_ST_OpenerSelection", (int)Openers.FifteenSecond);

        /// <summary>
        ///     Whether to include Peloton in the opener.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see langword="true"/><br />
        ///     <b>Options</b>: <see langword="true"/> or <see langword="false"/>
        /// </value>
        /// <seealso cref="Preset.DNC_ST_BalanceOpener" />
        public static readonly UserBool DNC_ST_OpenerOption_Peloton =
            new("DNC_ST_OpenerOption_Peloton", true);

        /// <summary>
        ///     Esprit threshold for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="Preset.DNC_ST_EspritOvercap" />
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
        /// <seealso cref="Preset.DNC_ST_Adv_SS" />
        public static readonly UserInt DNC_ST_Adv_SSBurstPercent =
            new("DNC_ST_Adv_SSBurstPercent", 0);

        /// <summary>
        ///     Include Standard Step in rotation for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="Preset.DNC_ST_Adv_SS" />
        public static readonly UserInt DNC_ST_ADV_SS_IncludeSS =
            new("DNC_ST_ADV_SS_IncludeSS", (int)IncludeStep.Yes);

        /// <summary>
        ///     Anti-Drift choice for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="AntiDrift.TripleWeave" /> <br />
        ///     <b>Options</b>: <see cref="AntiDrift">AntiDrift Enum</see>
        /// </value>
        /// <seealso cref="Preset.DNC_ST_Adv_SS" />
        public static readonly UserInt DNC_ST_ADV_AntiDrift =
            new("DNC_ST_ADV_AntiDrift", (int)AntiDrift.TripleWeave);

        /// <summary>
        ///     Include Technical Step in rotation for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="Preset.DNC_ST_Adv_TS" />
        public static readonly UserInt DNC_ST_ADV_TS_IncludeTS =
            new("DNC_ST_ADV_TS_IncludeTS", (int)IncludeStep.Yes);

        /// <summary>
        ///     Target HP% to use Technical Step above for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 0 <br />
        ///     <b>Range</b>: 0 - 15 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="Preset.DNC_ST_Adv_TS" />
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
        /// <seealso cref="Preset.DNC_ST_Adv_Feathers" />
        public static readonly UserInt DNC_ST_Adv_FeatherBurstPercent =
            new("DNC_ST_Adv_FeatherBurstPercent", 0);

        /// <summary>
        ///     Tillana drift protection for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="TillanaDriftProtection.None" /> <br />
        ///     <b>Options</b>: <see cref="TillanaDriftProtection" /> Enum.
        /// </value>
        /// <seealso cref="Preset.DNC_ST_Adv_Tillana" />
        public static readonly UserInt DNC_ST_ADV_TillanaUse =
            new("DNC_ST_ADV_TillanaUse", (int)TillanaDriftProtection.None);

        /// <summary>
        ///     Esprit threshold for Saber Dance in Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="Preset.DNC_ST_Adv_SaberDance" />
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
        /// <seealso cref="Preset.DNC_ST_Adv_PanicHeals" />
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
        /// <seealso cref="Preset.DNC_ST_Adv_PanicHeals" />
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
        /// <seealso cref="Preset.DNC_AoE_EspritOvercap" />
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
        /// <seealso cref="Preset.DNC_AoE_Adv_SS" />
        public static readonly UserInt DNC_AoE_Adv_SSBurstPercent =
            new("DNC_AoE_Adv_SSBurstPercent", 40);

        /// <summary>
        ///     Include Standard Step in rotation for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="Preset.DNC_AoE_Adv_SS" />
        public static readonly UserInt DNC_AoE_Adv_SS_IncludeSS =
            new("DNC_AoE_Adv_SS_IncludeSS", (int)IncludeStep.Yes);

        /// <summary>
        ///     Target HP% to use Technical Step above for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 40 <br />
        ///     <b>Range</b>: 0 - 60 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="Preset.DNC_AoE_Adv_TS" />
        public static readonly UserInt DNC_AoE_Adv_TSBurstPercent =
            new("DNC_AoE_Adv_TSBurstPercent", 40);

        /// <summary>
        ///     Include Technical Step in rotation for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="IncludeStep.Yes" /> <br />
        ///     <b>Options</b>: <see cref="IncludeStep">IncludeStep Enum</see>
        /// </value>
        /// <seealso cref="Preset.DNC_AoE_Adv_TS" />
        public static readonly UserInt DNC_AoE_Adv_TS_IncludeTS =
            new("DNC_AoE_Adv_TS_IncludeTS", (int)IncludeStep.Yes);

        /// <summary>
        ///     Esprit threshold for Saber Dance in AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 50 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="Preset.DNC_AoE_Adv_SaberDance" />
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
        /// <seealso cref="Preset.DNC_AoE_Adv_PanicHeals" />
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
        /// <seealso cref="Preset.DNC_AoE_Adv_PanicHeals" />
        public static readonly UserInt DNC_AoE_Adv_PanicHealWindPercent =
            new("DNC_AoE_Adv_PanicHealWindPercent", 20);

        #endregion

        #region Smaller Features

        /// <summary>
        ///     Whether the Focus Target should override the desired partner, while
        ///     still valid.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: false
        /// </value>
        /// <seealso cref="Preset.DNC_DesirablePartner" />
        public static readonly UserBool DNC_Partner_FocusOverride =
            new("DNC_Partner_FocusOverride", false);

        public enum PartnerShowAction
        {
            Default,
            ClosedPosition,
            SavageBlade,
        }

        /// <summary>
        ///     What action should be shown on the hotbar when the current dance
        ///     partner is considered optimal.
        /// </summary>
        /// <value>
        ///     Default: 0 <br />
        ///     Options: <see cref="PartnerShowAction" /> Enum
        /// </value>
        public static readonly UserInt DNC_Partner_ActionToShow =
            new("DNC_Partner_ActionToShow", (int)PartnerShowAction.Default);

        #endregion

        #endregion
    }
}
