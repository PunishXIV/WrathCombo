#region

using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using ImGuiNET;
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
        public static readonly UserInt
            DNCEspritThreshold_ST = new("DNCEspritThreshold_ST", 50); // ST - Esprit threshold

        public static readonly UserInt
            DNCEspritThreshold_AoE = new("DNCEspritThreshold_AoE", 50); // AoE - Esprit threshold

        #region Advanced ST Sliders

        public static readonly UserBoolArray DNC_ST_OpenerDifficulty =
            new("DNC_ST_OpenerDifficulty", [false, true]);
        public static readonly UserInt DNC_ST_OpenerSelection =
            new("DNC_ST_OpenerSelection", 0);
        public static readonly ContentCheck.ListSet DNC_ST_OpenerDifficultyListSet =
            ContentCheck.ListSet.BossOnly;

        public static readonly UserInt
            DNC_ST_Adv_SSBurstPercent = new("DNC_ST_Adv_SSBurstPercent", 0), // Standard Step - target HP% threshold
            DNC_ST_ADV_SS_IncludeSS = new("DNC_ST_ADV_SS_IncludeSS", (int)IncludeStep.Yes), // Include Standard Step
            DNC_ST_ADV_AntiDrift = new("DNC_ST_ADV_AntiDrift", (int)AntiDrift.TripleWeave), // Anti-Drift choice
            DNC_ST_ADV_TS_IncludeTS = new("DNC_ST_ADV_TS_IncludeTS", (int)IncludeStep.Yes), // Include Technical Step
            DNC_ST_Adv_TSBurstPercent = new("DNC_ST_Adv_TSBurstPercent", 0), // Technical Step - target  HP% threshold
            DNC_ST_Adv_FeatherBurstPercent =
                new("DNC_ST_Adv_FeatherBurstPercent", 0), // Feather burst -  target HP% threshold
            DNC_ST_Adv_SaberThreshold = new("DNC_ST_Adv_SaberThreshold", 50), // Saber Dance - Esprit  threshold
            DNC_ST_Adv_PanicHealWaltzPercent =
                new("DNC_ST_Adv_PanicHealWaltzPercent", 30), // Curing Waltz - player HP% threshold
            DNC_ST_Adv_PanicHealWindPercent =
                new("DNC_ST_Adv_PanicHealWindPercent", 20); // Second Wind - player HP% threshold

        #endregion

        #region Advanced AoE Sliders

        public static readonly UserInt
            DNC_AoE_Adv_SSBurstPercent = new("DNC_AoE_Adv_SSBurstPercent", 0), // Standard Step - target HP% threshold
            DNC_AoE_Adv_SS_IncludeSS = new("DNC_AoE_Adv_SS_IncludeSS", (int)IncludeStep.Yes), // Include Standard Step
            DNC_AoE_Adv_TSBurstPercent = new("DNC_AoE_Adv_TSBurstPercent", 0), // Technical Step - target HP% threshold
            DNC_AoE_Adv_TS_IncludeTS = new("DNC_AoE_Adv_TS_IncludeTS", (int)IncludeStep.Yes), // Include Technical Step
            DNC_AoE_Adv_SaberThreshold = new("DNC_AoE_Adv_SaberThreshold", 50), // Saber Dance - Esprit threshold
            DNC_AoE_Adv_PanicHealWaltzPercent =
                new("DNC_AoE_Adv_PanicHealWaltzPercent", 30), // Curing Waltz - player HP% threshold
            DNC_AoE_Adv_PanicHealWindPercent =
                new("DNC_AoE_Adv_PanicHealWindPercent", 20); // Second Wind - player HP% threshold

        #endregion

        public enum IncludeStep
        {
            No,
            Yes,
        }

        public enum AntiDrift
        {
            None,
            TripleWeave,
            Hold,
            Both,
        }

        public static readonly UserInt
            DNCVariantCurePercent = new("DNCVariantCurePercent"); // Variant Cure - player HP% threshold

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
                "\nRecommended to help prevent drift.",
                outputValue: (int)AntiDrift.TripleWeave, descriptionAsTooltip: true);
            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "Hold before Standard Step",
                "Will hold GCDs for Standard Step if it is going to come off cooldown before your next GCD." +
                "\nThis WILL give you down-time." +
                "\nONLY recommended if you have extra skill speed, but can be used as an anti-drift option.",
                outputValue: (int)AntiDrift.Hold, descriptionAsTooltip: true);
            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "Both",
                "Will use both options from above." +
                "\nThis WILL give you down-time." +
                "\nNOT recommended, but can be the answer if neither of the above options work for you.",
                outputValue: (int)AntiDrift.Both, descriptionAsTooltip: true);
            UserConfig.DrawRadioButton(
                DNC_ST_ADV_AntiDrift, "None",
                "Will not use any anti-drift options." +
                "\nThis WILL cause drift. NOT recommended.",
                outputValue: (int)AntiDrift.None, descriptionAsTooltip: true);
        }
            
        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.DNC_DanceComboReplacer:
                {
                    int[]? actions = Service.Configuration.DancerDanceCompatActionIDs.Select(x => (int)x).ToArray();

                    bool inputChanged = false;

                    inputChanged |= ImGui.InputInt("Emboite (Red) ActionID", ref actions[0], 0);
                    inputChanged |= ImGui.InputInt("Entrechat (Blue) ActionID", ref actions[1], 0);
                    inputChanged |= ImGui.InputInt("Jete (Green) ActionID", ref actions[2], 0);
                    inputChanged |= ImGui.InputInt("Pirouette (Yellow) ActionID", ref actions[3], 0);

                    if (inputChanged)
                    {
                        //Service.Configuration.DancerDanceCompatActionIDs = actions.Cast<uint>().ToArray();
                        Service.Configuration.DancerDanceCompatActionIDs = actions.Select(x => (uint)x).ToArray();
                        Service.Configuration.Save();
                    }

                    ImGui.Spacing();

                    break;
                }

                #region ST UI

                case CustomComboPreset.DNC_ST_BalanceOpener:
                    UserConfig.DrawRadioButton(DNC_ST_OpenerSelection,
                        "15s Countdown",
                        "Requires at least a 15s cooldown\nand that you start Standard Step at 15s.",
                        0, descriptionAsTooltip: true);
                    UserConfig.DrawRadioButton(DNC_ST_OpenerSelection,
                        "7s Countdown",
                        "Requires at least a 7s cooldown\nand that you start Standard Step at 7s.",
                        1, descriptionAsTooltip: true);
                    UserConfig.DrawBossOnlyChoice(DNC_ST_OpenerDifficulty);

                    break;

                case CustomComboPreset.DNC_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, DNCVariantCurePercent, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.DNC_ST_EspritOvercap:
                    UserConfig.DrawSliderInt(50, 100, DNCEspritThreshold_ST, "Esprit", 150,
                        SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_ST_Adv_SS:
                    UserConfig.DrawSliderInt(0, 5, DNC_ST_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below", 75);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_SS_IncludeSS,
                        "Include Standard Step",
                        "Will include Standard Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_SS_IncludeSS,
                        "Exclude Standard Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Standard Step.",
                        outputValue: (int)IncludeStep.No, itemWidth: 125f);

                    DrawAntiDriftOptions();

                    break;

                case CustomComboPreset.DNC_ST_Adv_TS:
                    UserConfig.DrawSliderInt(0, 5, DNC_ST_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below", 75);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_TS_IncludeTS,
                        "Include Technical Step",
                        "Will include Technical Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_ST_ADV_TS_IncludeTS,
                        "Exclude Technical Step",
                        "Will ONLY include the dance Steps, and Finish;" +
                        "\nYOU will need to manually press Technical Step.",
                        outputValue: (int)IncludeStep.No, itemWidth: 125f);

                    DrawAntiDriftOptions();

                    break;

                case CustomComboPreset.DNC_ST_Adv_Feathers:
                    UserConfig.DrawSliderInt(0, 5, DNC_ST_Adv_FeatherBurstPercent,
                        "Target HP% to dump all pooled feathers below", 75);

                    break;

                case CustomComboPreset.DNC_ST_Adv_SaberDance:
                    UserConfig.DrawSliderInt(50, 100, DNC_ST_Adv_SaberThreshold, "Esprit", 150,
                        SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_ST_Adv_PanicHeals:
                    UserConfig.DrawSliderInt(0, 100, DNC_ST_Adv_PanicHealWaltzPercent, "Curing Waltz HP%",
                        200);

                    UserConfig.DrawSliderInt(0, 100, DNC_ST_Adv_PanicHealWindPercent, "Second Wind HP%",
                        200);

                    break;

                #endregion

                #region AoE UI

                case CustomComboPreset.DNC_AoE_EspritOvercap:
                    UserConfig.DrawSliderInt(50, 100, DNCEspritThreshold_AoE, "Esprit", 150,
                        SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_SS:
                    UserConfig.DrawSliderInt(0, 10, DNC_AoE_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below", 75);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_SS_IncludeSS,
                        "Include Standard Step",
                        "Will include Standard Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_SS_IncludeSS,
                        "Exclude Standard Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Standard Step.",
                        outputValue: (int)IncludeStep.No, itemWidth: 125f);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_TS:
                    UserConfig.DrawSliderInt(0, 10, DNC_AoE_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below", 75);

                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_TS_IncludeTS,
                        "Include Technical Step",
                        "Will include Technical Step itself," +
                        "\ndance steps, and Finish into the rotation.",
                        outputValue: (int)IncludeStep.Yes, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        DNC_AoE_Adv_TS_IncludeTS,
                        "Exclude Technical Step",
                        "Will ONLY include the dance steps, and Finish;" +
                        "\nYOU will need to manually press Technical Step.",
                        outputValue: (int)IncludeStep.No, itemWidth: 125f);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_SaberDance:
                    UserConfig.DrawSliderInt(50, 100, DNC_AoE_Adv_SaberThreshold, "Esprit", 150,
                        SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_PanicHeals:
                    UserConfig.DrawSliderInt(0, 100, DNC_AoE_Adv_PanicHealWaltzPercent, "Curing Waltz HP%",
                        200);

                    UserConfig.DrawSliderInt(0, 100, DNC_AoE_Adv_PanicHealWindPercent, "Second Wind HP%", 200);

                    break;

                #endregion

                #region PVP

                case CustomComboPreset.DNCPvP_BurstMode_CuringWaltz:
                    UserConfig.DrawSliderInt(0, 90, DNCPvP.Config.DNCPvP_WaltzThreshold,
                        "Curing Waltz HP% - caps at 90 to prevent waste.");

                    break;

                case CustomComboPreset.DNCPvP_BurstMode_Dash:
                    UserConfig.DrawSliderInt(0, 3, DNCPvP.Config.DNCPvP_EnAvantCharges, "How many to save for manual");

                    break;

                #endregion
            }
        }
    }
}
