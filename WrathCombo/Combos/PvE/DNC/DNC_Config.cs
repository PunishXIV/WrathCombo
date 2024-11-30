using System.Linq;
using ImGuiNET;
using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

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

        public static readonly UserInt
            DNC_ST_Adv_SSBurstPercent = new("DNC_ST_Adv_SSBurstPercent", 0), // Standard Step - target HP% threshold
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
            DNC_AoE_Adv_TSBurstPercent = new("DNC_AoE_Adv_TSBurstPercent", 0), // Technical Step - target HP% threshold
            DNC_AoE_Adv_SaberThreshold = new("DNC_AoE_Adv_SaberThreshold", 50), // Saber Dance - Esprit threshold
            DNC_AoE_Adv_PanicHealWaltzPercent =
                new("DNC_AoE_Adv_PanicHealWaltzPercent", 30), // Curing Waltz - player HP% threshold
            DNC_AoE_Adv_PanicHealWindPercent =
                new("DNC_AoE_Adv_PanicHealWindPercent", 20); // Second Wind - player HP% threshold

        #endregion

        public static readonly UserInt
            DNCVariantCurePercent = new("DNCVariantCurePercent"); // Variant Cure - player HP% threshold
            
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

                case CustomComboPreset.DNC_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, DNCVariantCurePercent, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.DNC_ST_EspritOvercap:
                    UserConfig.DrawSliderInt(50, 100, DNCEspritThreshold_ST, "Esprit", 150,
                        SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_AoE_EspritOvercap:
                    UserConfig.DrawSliderInt(50, 100, DNCEspritThreshold_AoE, "Esprit", 150,
                        SliderIncrements.Fives);

                    break;

                case CustomComboPreset.DNC_ST_Adv_SS:
                    UserConfig.DrawSliderInt(0, 5, DNC_ST_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below", 75);

                    break;

                case CustomComboPreset.DNC_ST_Adv_TS:
                    UserConfig.DrawSliderInt(0, 5, DNC_ST_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below", 75);

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

                case CustomComboPreset.DNC_AoE_Adv_SS:
                    UserConfig.DrawSliderInt(0, 10, DNC_AoE_Adv_SSBurstPercent,
                        "Target HP% to stop using Standard Step below", 75);

                    break;

                case CustomComboPreset.DNC_AoE_Adv_TS:
                    UserConfig.DrawSliderInt(0, 10, DNC_AoE_Adv_TSBurstPercent,
                        "Target HP% to stop using Technical Step below", 75);

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

                case CustomComboPreset.DNCPvP_BurstMode_CuringWaltz:
                    UserConfig.DrawSliderInt(0, 90, DNCPvP.Config.DNCPvP_WaltzThreshold,
                        "Curing Waltz HP% - caps at 90 to prevent waste.");

                    break;

                case CustomComboPreset.DNCPvP_BurstMode_Dash:
                    UserConfig.DrawSliderInt(0, 3, DNCPvP.Config.DNCPvP_EnAvantCharges, "How many to save for manual");

                    break;
            }
        }
    }
}
