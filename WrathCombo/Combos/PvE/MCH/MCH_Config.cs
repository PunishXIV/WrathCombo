using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    internal static class Config
    {
        public static UserInt
            MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold", 25),
            MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold", 25),
            MCH_VariantCure = new("MCH_VariantCure"),
            MCH_AoE_TurretUsage = new("MCH_AoE_TurretUsage"),
            MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool", 0),
            MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool", 0),
            MCH_ST_WildfireHP = new("MCH_ST_WildfireHP", 1),
            MCH_ST_HyperchargeHP = new("MCH_ST_HyperchargeHP", 1),
            MCH_ST_QueenOverDrive = new("MCH_ST_QueenOverDrive");

        public static UserBoolArray
            MCH_ST_Reassembled = new("MCH_ST_Reassembled"),
            MCH_AoE_Reassembled = new("MCH_AoE_Reassembled");

        public static UserBool
            MCH_AoE_Hypercharge = new("MCH_AoE_Hypercharge");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.MCH_ST_Adv_Reassemble:

                    DrawSliderInt(0, 1, MCH_ST_ReassemblePool, "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(Excavator)}",
                        "", 5, 0);

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(Chainsaw)}",
                        "", 5, 1);

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(AirAnchor)}",
                        "", 5, 2);

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(Drill)}", "",
                        5, 3);

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(CleanShot)}",
                        "", 5, 4);

                    break;

                case CustomComboPreset.MCH_AoE_Adv_Reassemble:

                    DrawSliderInt(0, 1, MCH_AoE_ReassemblePool, "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled,
                        $"Use on {ActionWatching.GetActionName(SpreadShot)}/{ActionWatching.GetActionName(Scattergun)}",
                        "", 4, 0);

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled,
                        $"Use on {ActionWatching.GetActionName(AutoCrossbow)}", "", 4, 1);

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(Chainsaw)}",
                        "", 4, 2);

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(Excavator)}",
                        "", 4, 3);

                    break;

                case CustomComboPreset.MCH_ST_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_ST_SecondWindThreshold,
                        $"{ActionWatching.GetActionName(All.SecondWind)} HP percentage threshold");

                    break;

                case CustomComboPreset.MCH_AoE_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_AoE_SecondWindThreshold,
                        $"{ActionWatching.GetActionName(All.SecondWind)} HP percentage threshold");

                    break;

                case CustomComboPreset.MCH_AoE_Adv_Queen:
                    DrawSliderInt(50, 100, MCH_AoE_TurretUsage, "Battery threshold", sliderIncrement: 5);

                    break;

                case CustomComboPreset.MCH_AoE_Adv_GaussRicochet:
                    DrawAdditionalBoolChoice(MCH_AoE_Hypercharge,
                        $"Use Outwith {ActionWatching.GetActionName(Hypercharge)}", "");

                    break;

                case CustomComboPreset.MCH_Variant_Cure:
                    DrawSliderInt(1, 100, MCH_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.MCH_ST_Adv_QueenOverdrive:
                    DrawSliderInt(1, 10, MCH_ST_QueenOverDrive, "HP% for the target to be at or under");

                    break;

                case CustomComboPreset.MCH_ST_Adv_WildFire:
                    DrawSliderInt(0, 15, MCH_ST_WildfireHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.MCH_ST_Adv_Hypercharge:
                    DrawSliderInt(0, 15, MCH_ST_HyperchargeHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                //PVP
                case CustomComboPreset.MCHPvP_BurstMode_MarksmanSpite:
                    DrawSliderInt(0, 36000, MCHPvP.Config.MCHPVP_MarksmanSpite,
                        "Use Marksman's Spite when the target is below set HP");
                    break;

                case CustomComboPreset.MCHPvP_BurstMode_FullMetalField:
                    UserConfig.DrawHorizontalRadioButton(MCHPvP.Config.MCHPVP_FMFOption, "Full Metal Field Wildfire combo",
                        "Uses Full Metal Field when Wildfire is ready.", 1);

                    UserConfig.DrawHorizontalRadioButton(MCHPvP.Config.MCHPVP_FMFOption, "Full Metal Field only when Overheated",
                        "Only uses Full Metal Field while Overheated.", 2);

                    break;

            }
        }
    }
}
