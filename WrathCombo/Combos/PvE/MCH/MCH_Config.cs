using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    internal static class Config
    {
        public static UserInt
            MCH_Balance_Content = new("MCH_Balance_Content", 1),
            MCH_ST_QueenOverDrive = new("MCH_ST_QueenOverDrive"),
            MCH_ST_Adv_Excavator_SubOption = new("MCH_ST_Adv_Excavator_SubOption", 1),
            MCH_ST_Adv_Turret_SubOption = new("MCH_ST_Adv_Turret_SubOption", 1),
            MCH_ST_Adv_BarrelStabiliser_SubOption = new("MCH_ST_Adv_BarrelStabiliser_SubOption", 1),
            MCH_ST_Adv_Wildfire_SubOption = new("MCH_ST_Adv_Wildfire_SubOption", 1),
            MCH_ST_Adv_FullMetalMachinist_SubOption = new("MCH_ST_Adv_FullMetalMachinist_SubOption", 1),
            MCH_ST_TurretUsage = new("MCH_ST_TurretUsage", 100),
            MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool", 0),
            MCH_ST_GaussRicoPool = new("MCH_ST_GaussRicoPool", 0),
            MCH_ST_SecondWindThreshold = new("MCH_ST_SecondWindThreshold", 25),
            MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool", 0),
            MCH_AoE_TurretUsage = new("MCH_AoE_TurretUsage", 100),
            MCH_AoE_SecondWindThreshold = new("MCH_AoE_SecondWindThreshold", 25),
            MCH_VariantCure = new("MCH_VariantCure");

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
                    ImGui.Indent();
                    DrawBossOnlyChoice(MCH_Balance_Content);

                    break;

                case CustomComboPreset.MCH_ST_Adv_WildFire:
                    DrawHorizontalRadioButton(MCH_ST_Adv_Wildfire_SubOption,
                        "All content",
                        $"Uses {Wildfire.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_Wildfire_SubOption,
                        "Boss encounters Only",
                        $"Only uses {Wildfire.ActionName()} when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MCH_ST_Adv_Stabilizer:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(MCH_ST_Adv_BarrelStabiliser_SubOption,
                        "All content",
                        $"Uses {BarrelStabilizer.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_BarrelStabiliser_SubOption,
                        "Boss encounters Only",
                        $"Only uses {BarrelStabilizer.ActionName()} when in Boss encounters.", 1);
                    ImGui.Unindent();

                    break;

                case CustomComboPreset.MCH_ST_Adv_Stabilizer_FullMetalField:
                    ImGui.Indent();
                    DrawHorizontalRadioButton(MCH_ST_Adv_FullMetalMachinist_SubOption,
                        "All content",
                        $"Uses {FullMetalField.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_FullMetalMachinist_SubOption,
                        "Boss encounters Only",
                        $"Only uses {FullMetalField.ActionName()} when in Boss encounters.", 1);
                    ImGui.Unindent();

                    break;

                case CustomComboPreset.MCH_ST_Adv_TurretQueen:
                    DrawHorizontalRadioButton(MCH_ST_Adv_Turret_SubOption,
                        "All content",
                        $"Uses {AutomatonQueen.ActionName()} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_Turret_SubOption,
                        "Boss encounters Only",
                        $"Only uses {AutomatonQueen.ActionName()} logic when in Boss encounters.", 1);

                    if (MCH_ST_Adv_Turret_SubOption == 1)
                    {
                        DrawSliderInt(50, 100, MCH_ST_TurretUsage,
                            $"Uses {AutomatonQueen.ActionName()} at this battery threshold outside of Boss encounter.\n Only counts for 'Boss encounters Only setting'.");
                    }

                    break;

                case CustomComboPreset.MCH_ST_Adv_Excavator:
                    DrawHorizontalRadioButton(MCH_ST_Adv_Excavator_SubOption,
                        "All content",
                        $"Uses {Excavator.ActionName()} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_Excavator_SubOption,
                        "Boss encounters Only",
                        $"Only uses {Excavator.ActionName()} logic when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.MCH_ST_Adv_GaussRicochet:
                    DrawSliderInt(0, 2, MCH_ST_GaussRicoPool, "Number of Charges of to Save for Manual Use");

                    break;

                case CustomComboPreset.MCH_ST_Adv_Reassemble:
                    DrawSliderInt(0, 1, MCH_ST_ReassemblePool, "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {Excavator.ActionName()}", "", 5, 0);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {Chainsaw.ActionName()}", "", 5, 1);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {AirAnchor}", "", 5, 2);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {Drill.ActionName()}", "", 5, 3);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {CleanShot.ActionName()}", "", 5, 4);

                    break;

                case CustomComboPreset.MCH_ST_Adv_QueenOverdrive:
                    DrawSliderInt(1, 10, MCH_ST_QueenOverDrive, "HP% for the target to be at or under");

                    break;

                case CustomComboPreset.MCH_ST_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_ST_SecondWindThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    break;

                //AoE
                case CustomComboPreset.MCH_AoE_Adv_Reassemble:
                    DrawSliderInt(0, 1, MCH_AoE_ReassemblePool, "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {SpreadShot.ActionName()}/{Scattergun.ActionName()}", "", 4, 0);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {AirAnchor.ActionName()}", "", 4, 1);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {Chainsaw.ActionName()}", "", 4, 2);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {Excavator.ActionName()}", "", 4, 3);

                    break;

                case CustomComboPreset.MCH_AoE_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_AoE_SecondWindThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    break;

                case CustomComboPreset.MCH_AoE_Adv_Queen:
                    DrawSliderInt(50, 100, MCH_AoE_TurretUsage, "Battery threshold", sliderIncrement: 5);

                    break;

                case CustomComboPreset.MCH_AoE_Adv_GaussRicochet:
                    DrawAdditionalBoolChoice(MCH_AoE_Hypercharge,
                        $"Use Outwith {Hypercharge.ActionName()}", "");

                    break;

                //Variant
                case CustomComboPreset.MCH_Variant_Cure:
                    DrawSliderInt(1, 100, MCH_VariantCure, "HP% to be at or under", 200);

                    break;


            }
        }
    }
}
