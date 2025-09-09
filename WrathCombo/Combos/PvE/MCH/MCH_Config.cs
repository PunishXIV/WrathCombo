using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.MCH_ST_Adv_Opener:
                    ImGui.Indent();
                    DrawBossOnlyChoice(MCH_Balance_Content);
                    break;

                case Preset.MCH_ST_Adv_WildFire:
                    DrawHorizontalRadioButton(MCH_ST_WildfireBossOption,
                        "All content", $"Uses {Wildfire.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_WildfireBossOption,
                        "Bosses Only", $"Only uses {Wildfire.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case Preset.MCH_ST_Adv_Stabilizer:
                    DrawHorizontalRadioButton(MCH_ST_BarrelStabilizerBossOption,
                        "All content", $"Uses {BarrelStabilizer.ActionName()} regardless of content.", 0);
                    DrawHorizontalRadioButton(MCH_ST_BarrelStabilizerBossOption,
                        "Bosses Only", $"Only uses {BarrelStabilizer.ActionName()} when the targeted enemy is a boss.", 1);
                    break;

                case Preset.MCH_ST_Adv_Hypercharge:
                    DrawSliderInt(0, 50, MCH_ST_HyperchargeHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(MCH_ST_HyperchargeBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(MCH_ST_HyperchargeBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.MCH_ST_Adv_TurretQueen:
                    DrawHorizontalRadioButton(MCH_ST_QueenBossOption,
                        "Use The Balance Logic in all content", $"Uses {AutomatonQueen.ActionName()} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(MCH_ST_QueenBossOption,
                        "Use The Balance logic only in Boss encounters", $"Only uses {AutomatonQueen.ActionName()} logic when in Boss encounters.", 1);

                    if (MCH_ST_QueenBossOption == 1)
                    {
                        DrawSliderInt(50, 100, MCH_ST_TurretUsage,
                            $"Uses {AutomatonQueen.ActionName()} at this battery threshold outside of Boss encounter.");
                    }
                    break;


                case Preset.MCH_ST_Adv_GaussRicochet:
                    DrawSliderInt(0, 2, MCH_ST_GaussRicoPool,
                        "Number of Charges of to Save for Manual Use");
                    break;

                case Preset.MCH_ST_Adv_Reassemble:

                    DrawSliderInt(0, 50, MCH_ST_ReassembleHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(MCH_ST_ReassembleBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(MCH_ST_ReassembleBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();

                    DrawSliderInt(0, 1, MCH_ST_ReassemblePool,
                        "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {Excavator.ActionName()}", "", 5, 0);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {Chainsaw.ActionName()}", "", 5, 1);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {AirAnchor.ActionName()}", "", 5, 2);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {Drill.ActionName()}", "", 5, 3);
                    DrawHorizontalMultiChoice(MCH_ST_Reassembled, $"Use on {CleanShot.ActionName()}", "", 5, 4);
                    break;



                case Preset.MCH_ST_Adv_Chainsaw:

                    DrawSliderInt(0, 50, MCH_ST_ChainsawHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(MCH_ST_ChainsawBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(MCH_ST_ChainsawBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.MCH_ST_Adv_QueenOverdrive:
                    DrawSliderInt(0, 100, MCH_ST_QueenOverDriveHPThreshold,
                        "HP% for the target to be at or under");
                    break;

                case Preset.MCH_ST_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_ST_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");
                    break;

                //AoE
                case Preset.MCH_AoE_Adv_Reassemble:
                    DrawSliderInt(0, 100, MCH_AoE_ReassembleHPThreshold,
                        $"Stop Using {Reassemble.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    DrawSliderInt(0, 2, MCH_AoE_ReassemblePool,
                        "Number of Charges to Save for Manual Use");

                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {SpreadShot.ActionName()}/{Scattergun.ActionName()}", "", 4, 0);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {AirAnchor.ActionName()}", "", 4, 1);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {Chainsaw.ActionName()}", "", 4, 2);
                    DrawHorizontalMultiChoice(MCH_AoE_Reassembled, $"Use on {Excavator.ActionName()}", "", 4, 3);
                    break;

                case Preset.MCH_AoE_Adv_QueenOverdrive:
                    DrawSliderInt(0, 100, MCH_AoE_QueenOverDriveHPThreshold,
                        "HP% for the target to be at or under");
                    break;

                case Preset.MCH_AoE_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_AoE_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");
                    break;

                case Preset.MCH_AoE_Adv_Queen:
                    DrawSliderInt(0, 100, MCH_AoE_QueenHpThreshold,
                        $"Stop Using {RookAutoturret.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    DrawSliderInt(50, 100, MCH_AoE_TurretBatteryUsage,
                        "Battery threshold", sliderIncrement: 5);
                    break;

                case Preset.MCH_AoE_Adv_FlameThrower:

                    DrawHorizontalRadioButton(MCH_AoE_FlamethrowerMovement,
                        "Stationary Only", $"Uses {Flamethrower.ActionName()} only while stationary", 0);

                    DrawHorizontalRadioButton(MCH_AoE_FlamethrowerMovement,
                        "Any Movement", $"Uses {Flamethrower.ActionName()} regardless of any movement conditions.", 1);

                    ImGui.Spacing();
                    if (MCH_AoE_FlamethrowerMovement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, MCH_AoE_FlamehrowerTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }

                    DrawSliderInt(0, 50, MCH_AoE_FlamethrowerHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    ImGui.Indent();
                    break;

                case Preset.MCH_AoE_Adv_Hypercharge:
                    DrawSliderInt(0, 100, MCH_AoE_HyperchargeHPThreshold,
                        $"Stop Using {Hypercharge.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MCH_AoE_Adv_Chainsaw:
                    DrawSliderInt(0, 100, MCH_AoE_ChainsawHPThreshold,
                        $"Stop Using {Chainsaw.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MCH_AoE_Adv_Stabilizer:
                    DrawSliderInt(0, 100, MCH_AoE_BarrelStabilizerHPThreshold,
                        $"Stop Using {BarrelStabilizer.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;


                case Preset.MCH_GaussRoundRicochet:
                    DrawHorizontalRadioButton(MCH_GaussRico,
                        $"Change {GaussRound.ActionName()} / {DoubleCheck.ActionName()}", $"Changes to {Ricochet.ActionName()} / {CheckMate.ActionName()} depending on charges and what was used last", 0);

                    DrawHorizontalRadioButton(MCH_GaussRico,
                        $"Change {Ricochet.ActionName()} / {CheckMate.ActionName()}", $"Changes to {GaussRound.ActionName()} / {DoubleCheck.ActionName()} depending on charges and what was used last", 1);
                    break;
            }
        }

        #region Variables

        public static UserInt
            MCH_Balance_Content = new("MCH_Balance_Content", 1),
            MCH_ST_QueenOverDriveHPThreshold = new("MCH_ST_QueenOverDrive", 1),
            MCH_ST_QueenBossOption = new("MCH_ST_QueenBossOption", 0),
            MCH_ST_QueenHPOption = new("MCH_ST_QueenHPOption", 10),
            MCH_ST_BarrelStabilizerBossOption = new("MCH_ST_BarrelStabilizerBossOption", 1),
            MCH_ST_WildfireBossOption = new("MCH_ST_WildfireBossOption", 1),
            MCH_ST_HyperchargeBossOption = new("MCH_ST_HyperchargeBossOption", 0),
            MCH_ST_HyperchargeHPOption = new("MCH_ST_HyperchargeHPOption", 10),
            MCH_ST_ReassembleBossOption = new("MCH_ST_ReassembleBossOption", 0),
            MCH_ST_ReassembleHPOption = new("MCH_ST_ReassembleHPOption", 10),
            MCH_ST_ChainsawBossOption = new("MCH_ST_ChainsawBossOption", 0),
            MCH_ST_ChainsawHPOption = new("MCH_ST_ChainsawHPOption", 10),
            MCH_ST_TurretUsage = new("MCH_ST_TurretUsage", 100),
            MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool", 0),
            MCH_ST_GaussRicoPool = new("MCH_ST_GaussRicoPool", 0),
            MCH_ST_SecondWindHPThreshold = new("MCH_ST_SecondWindThreshold", 40),
            MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool", 0),
            MCH_AoE_TurretBatteryUsage = new("MCH_AoE_TurretUsage", 100),
            MCH_AoE_FlamethrowerMovement = new("MCH_AoE_FlamethrowerMovement", 0),
            MCH_AoE_FlamethrowerHPOption = new("MCH_AoE_FlamethrowerHPOption", 40),
            MCH_AoE_HyperchargeHPThreshold = new("MCH_AoE_HyperchargeHPThreshold", 40),
            MCH_AoE_ReassembleHPThreshold = new("MCH_AoE_ReassembleHPThreshold", 40),
            MCH_AoE_ChainsawHPThreshold = new("MCH_AoE_ChainsawHPThreshold", 40),
            MCH_AoE_QueenHpThreshold = new("MCH_AoE_QueenHpThreshold", 40),
            MCH_AoE_BarrelStabilizerHPThreshold = new("MCH_AoE_BarrelStabilizerHPThreshold", 40),
            MCH_AoE_QueenOverDriveHPThreshold = new("MCH_AoE_QueenOverDrive", 20),
            MCH_AoE_SecondWindHPThreshold = new("MCH_AoE_SecondWindThreshold", 40),
            MCH_GaussRico = new("MCHGaussRico", 0);

        public static UserFloat
            MCH_AoE_FlamehrowerTimeStill = new("MCH_AoE_FlamehrowerTimeStill", 2.5f);

        public static UserBoolArray
            MCH_ST_Reassembled = new("MCH_ST_Reassembled"),
            MCH_AoE_Reassembled = new("MCH_AoE_Reassembled");

        #endregion
    }
}
