using ImGuiNET;
using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    internal static class Config
    {
        public static UserInt
            BLM_VariantCure = new("BLM_VariantCure"),
            BLM_VariantRampart = new("BLM_VariantRampart"),
            BLM_ST_Triplecast_HoldCharges = new("BLM_ST_Triplecast_HoldCharges", 0),
            BLM_ST_UsePolyglot_HoldCharges = new("BLM_ST_UsePolyglot_HoldCharges", 1),
            BLM_ST_UsePolyglotMoving_HoldCharges = new("BLM_ST_UsePolyglotMoving_HoldCharges", 0),
            BLM_ST_ThunderHP = new("BHP", 0),
            BLM_ST_LeyLinesCharges = new("BLM_ST_LeyLinesCharges", 1),
            BLM_AoE_Triplecast_HoldCharges = new("BLM_AoE_Triplecast_HoldCharges", 0),
            BLM_AoE_UsePolyglot_HoldCharges = new("BLM_AoE_UsePolyglot_HoldCharges", 1),
            BLM_AoE_UsePolyglotMoving_HoldCharges = new("BLM_AoE_UsePolyglotMoving_HoldCharges", 0),
            BLM_AoE_LeyLinesCharges = new("BLM_AoE_LeyLinesCharges", 1),
            BLM_AoE_ThunderHP = new("BLM_AoE_ThunderHP", 5),
            BLMPvP_BurstMode_WreathOfIce = new("BLMPvP_BurstMode_WreathOfIce", 0),
            BLMPvP_BurstMode_WreathOfFireExecute = new("BLMPvP_BurstMode_WreathOfFireExecute", 0);

        public static UserFloat
            BLM_ST_Triplecast_ChargeTime = new("BLM_ST_Triplecast_ChargeTime", 20),
            BLM_AoE_Triplecast_ChargeTime = new("BLM_AoE_Triplecast_ChargeTime", 20);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.BLM_Variant_Cure:
                    DrawSliderInt(1, 100, BLM_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.BLM_Variant_Rampart:
                    DrawSliderInt(1, 100, BLM_VariantRampart, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.BLM_ST_Triplecast:
                    DrawSliderInt(0, 1, BLM_ST_Triplecast_HoldCharges, "How many charges to keep ready? (0 = Use all)");

                    DrawSliderInt(10, 20, BLM_ST_Triplecast_ChargeTime,
                        "Set the amount of time remaining on Triplecast charge before using.(Only when at threshold)");

                    break;

                case CustomComboPreset.BLM_ST_UsePolyglot:
                    DrawSliderInt(0, 2, BLM_ST_UsePolyglot_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_ST_UsePolyglotMoving:
                    DrawSliderInt(0, 2, BLM_ST_UsePolyglotMoving_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_ST_LeyLines:
                    DrawSliderInt(0, 1, BLM_ST_LeyLinesCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_ST_Thunder:
                    DrawSliderInt(0, 10, BLM_ST_ThunderHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                case CustomComboPreset.BLM_AoE_Triplecast:
                    DrawSliderInt(0, 1, BLM_AoE_Triplecast_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    DrawSliderInt(10, 20, BLM_AoE_Triplecast_ChargeTime,
                        "Set the amount of time remaining on Triplecast charge before using.(Only when at threshold)");

                    break;

                case CustomComboPreset.BLM_AoE_UsePolyglot:
                    DrawSliderInt(0, 2, BLM_AoE_UsePolyglot_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_AoE_UsePolyglotMoving:
                    DrawSliderInt(0, 2, BLM_AoE_UsePolyglotMoving_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_AoE_LeyLines:
                    DrawSliderInt(0, 1, BLM_AoE_LeyLinesCharges,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.BLM_AoE_Thunder:
                    DrawSliderInt(0, 10, BLM_AoE_ThunderHP,
                        "Stop Using When Target HP% is at or Below (Set to 0 to Disable This Check)");

                    break;

                // PvP

                // Burst
                case CustomComboPreset.BLMPvP_Burst:
                    DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_Burst_SubOption, "Defensive Burst",
                        "Also uses Burst when under 50%% HP.\n- Will not use outside combat.");

                    break;

                // Elemental Weave
                case CustomComboPreset.BLMPvP_ElementalWeave:
                    DrawSliderInt(10, 100, BLMPvP.Config.BLMPvP_ElementalWeave_PlayerHP, "Player HP%", 180);
                    ImGui.Spacing();
                    DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_ElementalWeave_SubOption, "Defensive Elemental Weave",
                        "When under, uses Wreath of Ice instead.\n- Will not use outside combat.");

                    break;

                // Lethargy
                case CustomComboPreset.BLMPvP_Lethargy:
                    DrawSliderInt(10, 100, BLMPvP.Config.BLMPvP_Lethargy_TargetHP, "Target HP%", 180);
                    ImGui.Spacing();
                    DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_Lethargy_SubOption, "Defensive Lethargy",
                        "Also uses Lethargy when under 50%% HP.\n- Uses only when targeted by enemy.");

                    break;

                // Xenoglossy
                case CustomComboPreset.BLMPvP_Xenoglossy:
                    DrawSliderInt(10, 100, BLMPvP.Config.BLMPvP_Xenoglossy_TargetHP, "Target HP%", 180);
                    ImGui.Spacing();
                    DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_Xenoglossy_SubOption, "Defensive Xenoglossy",
                        "Also uses Xenoglossy when under 50%% HP.");

                    break;
            }
        }
    }
}