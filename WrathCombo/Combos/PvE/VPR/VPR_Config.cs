#region

using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class VPR
{
    internal static class Config
    {
        public static UserInt
            VPR_ST_SecondWind_Threshold = new("VPR_ST_SecondWindThreshold", 25),
            VPR_ST_Bloodbath_Threshold = new("VPR_ST_BloodbathThreshold", 40),
            VPR_AoE_SecondWind_Threshold = new("VPR_AoE_SecondWindThreshold", 25),
            VPR_AoE_Bloodbath_Threshold = new("VPR_AoE_BloodbathThreshold", 40),
            VPR_ST_UncoiledFury_HoldCharges = new("VPR_ST_UncoiledFury_HoldCharges", 1),
            VPR_AoE_UncoiledFury_HoldCharges = new("VPR_AoE_UncoiledFury_HoldCharges", 0),
            VPR_ST_UncoiledFury_Threshold = new("VPR_ST_UncoiledFury_Threshold", 1),
            VPR_AoE_UncoiledFury_Threshold = new("VPR_AoE_UncoiledFury_Threshold", 1),
            VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton"),
            VPR_VariantCure = new("VPR_VariantCure");

        public static UserFloat
            VPR_ST_Reawaken_Usage = new("VPR_ST_Reawaken_Usage", 2),
            VPR_AoE_Reawaken_Usage = new("VPR_AoE_Reawaken_Usage", 2);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.VPR_ST_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_ST_UncoiledFury_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");
                    DrawSliderInt(0, 5, VPR_ST_UncoiledFury_Threshold, "Set a HP% Threshold to use all charges.");

                    break;

                case CustomComboPreset.VPR_AoE_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_AoE_UncoiledFury_HoldCharges,
                        "How many charges to keep ready? (0 = Use all)");
                    DrawSliderInt(0, 5, VPR_AoE_UncoiledFury_Threshold, "Set a HP% Threshold to use all charges.");

                    break;

                case CustomComboPreset.VPR_ST_Reawaken:
                    DrawRoundedSliderFloat(0, 10, VPR_ST_Reawaken_Usage,
                        $"Stop using {Reawaken.ActionName()} at Enemy HP %. Set to Zero to disable this check.",
                        digits: 1);

                    break;

                case CustomComboPreset.VPR_AoE_Reawaken:
                    DrawRoundedSliderFloat(0, 10, VPR_AoE_Reawaken_Usage,
                        $"Stop using {Reawaken.ActionName()} at Enemy HP %. Set to Zero to disable this check.",
                        digits: 1);

                    break;

                case CustomComboPreset.VPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, VPR_ST_SecondWind_Threshold,
                        $"HP percent threshold to use {All.SecondWind.ActionName()} (0 = Disabled)");

                    DrawSliderInt(0, 100, VPR_ST_Bloodbath_Threshold,
                        $"HP percent threshold to use {All.Bloodbath.ActionName()} (0 = Disabled)");

                    break;

                case CustomComboPreset.VPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, VPR_AoE_SecondWind_Threshold,
                        $"HP percent threshold to use {All.SecondWind.ActionName()} (0 = Disabled)");

                    DrawSliderInt(0, 100, VPR_AoE_Bloodbath_Threshold,
                        $"HP percent threshold to use {All.Bloodbath.ActionName()} (0 = Disabled)");

                    break;

                case CustomComboPreset.VPR_ReawakenLegacy:
                    DrawRadioButton(VPR_ReawakenLegacyButton, $"Replaces {Reawaken.ActionName()}",
                        $"Replaces {Reawaken.ActionName()} with Full Generation - Legacy combo.", 0);

                    DrawRadioButton(VPR_ReawakenLegacyButton, $"Replaces {SteelFangs.ActionName()}",
                        $"Replaces {SteelFangs.ActionName()} with Full Generation - Legacy combo.", 1);

                    break;

                case CustomComboPreset.VPR_Variant_Cure:
                    DrawSliderInt(1, 100, VPR_VariantCure, "HP% to be at or under", 200);

                    break;

                // PvP

                // Bloodcoil
                case CustomComboPreset.VPRPvP_Bloodcoil:
                    DrawSliderInt(10, 100, VPRPvP.Config.VPRPvP_Bloodcoil_TargetHP, "Target HP%", 210);
                    DrawSliderInt(10, 100, VPRPvP.Config.VPRPvP_Bloodcoil_PlayerHP, "Player HP%", 210);

                    break;

                // Uncoiled Fury
                case CustomComboPreset.VPRPvP_UncoiledFury:
                    DrawSliderInt(10, 100, VPRPvP.Config.VPRPvP_UncoiledFury_TargetHP, "Target HP%", 210);

                    break;

                // Backlash
                case CustomComboPreset.VPRPvP_Backlash:
                    DrawAdditionalBoolChoice(VPRPvP.Config.VPRPvP_Backlash_SubOption, "Empowered Only",
                        "Also requires Snake's Bane to be present.");

                    break;

                // Rattling Coil
                case CustomComboPreset.VPRPvP_RattlingCoil:
                    DrawHorizontalMultiChoice(VPRPvP.Config.VPRPvP_RattlingCoil_SubOptions, "No Uncoiled Fury",
                        "Must not have charges of Uncoiled Fury.", 2, 0);

                    DrawHorizontalMultiChoice(VPRPvP.Config.VPRPvP_RattlingCoil_SubOptions, "No Snake Scales",
                        "Snake Scales must be on cooldown.", 2, 1);

                    break;

                // Slither
                case CustomComboPreset.VPRPvP_Slither:
                    DrawSliderInt(0, 1, VPRPvP.Config.VPRPvP_Slither_Charges, "Charges to Keep", 178);
                    DrawSliderInt(6, 10, VPRPvP.Config.VPRPvP_Slither_Range, "Maximum Range", 173);

                    break;
            }
        }
    }
}