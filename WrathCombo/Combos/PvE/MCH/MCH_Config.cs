using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
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
            MCH_AoE_TurretUsage = new("MCH_AoE_TurretUsage", 100),
            MCH_ST_TurretUsage = new("MCH_ST_TurretUsage", 100),
            MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool", 0),
            MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool", 0),
            MCH_ST_WildfireHP = new("MCH_ST_WildfireHP", 1),
            MCH_ST_HyperchargeHP = new("MCH_ST_HyperchargeHP", 1),
            MCH_ST_QueenOverDrive = new("MCH_ST_QueenOverDrive"),
            MCH_Balance_Content = new("MCH_Balance_Content", 1),
            MCH_ST_Adv_Excavator_SubOption = new("MCH_ST_Adv_Excavator_SubOption", 1),
            MCH_ST_Adv_Turret_SubOption = new("MCH_ST_Adv_Turret_SubOption", 1),
            MCH_ST_Adv_BarrelStabiliser_SubOption = new("MCH_ST_Adv_BarrelStabiliser_SubOption", 1),
            MCH_ST_Adv_Wildfire_SubOption = new("MCH_ST_Adv_Wildfire_SubOption", 1),
            MCH_ST_Adv_FullMetalMachinist_SubOption = new("MCH_ST_Adv_FullMetalMachinist_SubOption", 1);

        public static UserBoolArray
            MCH_ST_Reassembled = new("MCH_ST_Reassembled"),
            MCH_AoE_Reassembled = new("MCH_AoE_Reassembled");

        public static UserBool
            MCH_AoE_Hypercharge = new("MCH_AoE_Hypercharge");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.MCH_ST_Adv_Opener:
                    DrawBossOnlyChoice(MCH_Balance_Content);
                    break;

                case CustomComboPreset.MCH_ST_Adv_Excavator:
                    DrawHorizontalRadioButton(MCH_ST_Adv_Excavator_SubOption,
                        "All content",
                        $"Uses {ActionWatching.GetActionName(Excavator)} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_Excavator_SubOption,
                        "Boss encounters Only",
                        $"Only uses {ActionWatching.GetActionName(Excavator)} logic when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MCH_ST_Adv_TurretQueen:
                    DrawHorizontalRadioButton(MCH_ST_Adv_Turret_SubOption,
                      "All content",
                      $"Uses {ActionWatching.GetActionName(AutomatonQueen)} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_Turret_SubOption,
                        "Boss encounters Only",
                        $"Only uses {ActionWatching.GetActionName(AutomatonQueen)} logic when in Boss encounters.", 1);

                    if (MCH_ST_Adv_Turret_SubOption == 1)
                    {
                        DrawSliderInt(50, 100, MCH_ST_TurretUsage,
                          $"Uses {ActionWatching.GetActionName(AutomatonQueen)} at this battery threshold outside of Boss encounter.\n Only counts for 'Boss encounters Only setting'.");
                    }

                    break;

                case CustomComboPreset.MCH_ST_Adv_Stabilizer:
                    DrawHorizontalRadioButton(MCH_ST_Adv_BarrelStabiliser_SubOption,
                        "All content",
                        $"Uses {ActionWatching.GetActionName(BarrelStabilizer)} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_BarrelStabiliser_SubOption,
                        "Boss encounters Only",
                        $"Only uses {ActionWatching.GetActionName(BarrelStabilizer)} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MCH_ST_Adv_WildFire:
                    DrawSliderInt(0, 15, MCH_ST_WildfireHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    DrawHorizontalRadioButton(MCH_ST_Adv_Wildfire_SubOption,
                        "All content",
                        $"Uses {ActionWatching.GetActionName(Wildfire)} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_Wildfire_SubOption,
                        "Boss encounters Only",
                        $"Only uses {ActionWatching.GetActionName(Wildfire)} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MCH_ST_Adv_Stabilizer_FullMetalField:
                    DrawHorizontalRadioButton(MCH_ST_Adv_FullMetalMachinist_SubOption,
                        "All content",
                        $"Uses {ActionWatching.GetActionName(FullMetalField)} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_FullMetalMachinist_SubOption,
                        "Boss encounters Only",
                        $"Only uses {ActionWatching.GetActionName(FullMetalField)} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MCH_ST_Adv_Reassemble:

                    DrawSliderInt(0, 1, MCH_ST_ReassemblePool, "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(Excavator)}", "", 5, 0);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(Chainsaw)}", "", 5, 1);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(AirAnchor)}", "", 5, 2);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(Drill)}", "", 5, 3);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {ActionWatching.GetActionName(CleanShot)}", "", 5, 4);

                    break;

                case CustomComboPreset.MCH_AoE_Adv_Reassemble:

                    DrawSliderInt(0, 1, MCH_AoE_ReassemblePool, "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(SpreadShot)}/{ActionWatching.GetActionName(Scattergun)}", "", 4, 0);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(AirAnchor)}", "", 4, 1);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(Chainsaw)}", "", 4, 2);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {ActionWatching.GetActionName(Excavator)}", "", 4, 3);

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
                    DrawHorizontalRadioButton(MCHPvP.Config.MCHPVP_FMFOption, "Full Metal Field Wildfire combo",
                        "Uses Full Metal Field when Wildfire is ready.", 1);

                    DrawHorizontalRadioButton(MCHPvP.Config.MCHPVP_FMFOption, "Full Metal Field only when Overheated",
                        "Only uses Full Metal Field while Overheated.", 2);

                    break;
            }
        }
    }
}