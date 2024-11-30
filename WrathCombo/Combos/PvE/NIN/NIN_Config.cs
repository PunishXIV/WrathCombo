using WrathCombo.Combos.PvP;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal static class Config
    {
        public const string
            Trick_CooldownRemaining = "Trick_CooldownRemaining",
            Mug_NinkiGauge = "Mug_NinkiGauge",
            Ninki_BhavaPooling = "Ninki_BhavaPooling",
            Ninki_HellfrogPooling = "Ninki_HellfrogPooling",
            NIN_SimpleMudra_Choice = "NIN_SimpleMudra_Choice",
            Ninki_BunshinPoolingST = "Ninki_BunshinPoolingST",
            Ninki_BunshinPoolingAoE = "Ninki_BunshinPoolingAoE",
            Advanced_Trick_Cooldown = "Advanced_Trick_Cooldown",
            Advanced_DotonTimer = "Advanced_DotonTimer",
            Advanced_DotonHP = "Advanced_DotonHP",
            BurnKazematoi = "BurnKazematoi",
            Advanced_TCJEnderAoE = "Advanced_TCJEnderAoe",
            Advanced_ChargePool = "Advanced_ChargePool",
            SecondWindThresholdST = "SecondWindThresholdST",
            ShadeShiftThresholdST = "ShadeShiftThresholdST",
            BloodbathThresholdST = "BloodbathThresholdST",
            SecondWindThresholdAoE = "SecondWindThresholdAoE",
            ShadeShiftThresholdAoE = "ShadeShiftThresholdAoE",
            BloodbathThresholdAoE = "BloodbathThresholdAoE",
            NIN_VariantCure = "NIN_VariantCure";

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.NIN_Simple_Mudras:
                    UserConfig.DrawRadioButton(NIN_SimpleMudra_Choice, "Mudra Path Set 1",
                        "1. Ten Mudras -> Fuma Shuriken, Raiton/Hyosho Ranryu, Suiton (Doton under Kassatsu).\nChi Mudras -> Fuma Shuriken, Hyoton, Huton.\nJin Mudras -> Fuma Shuriken, Katon/Goka Mekkyaku, Doton",
                        1);

                    UserConfig.DrawRadioButton(NIN_SimpleMudra_Choice, "Mudra Path Set 2",
                        "2. Ten Mudras -> Fuma Shuriken, Hyoton/Hyosho Ranryu, Doton.\nChi Mudras -> Fuma Shuriken, Katon, Suiton.\nJin Mudras -> Fuma Shuriken, Raiton/Goka Mekkyaku, Huton (Doton under Kassatsu).",
                        2);

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode:
                    UserConfig.DrawSliderInt(0, 10, BurnKazematoi, "Target HP% to dump all pooled Kazematoi below");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra:
                    UserConfig.DrawSliderInt(50, 100, Ninki_BhavaPooling,
                        "Set the minimal amount of Ninki required to have before spending on Bhavacakra.");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack:
                    UserConfig.DrawSliderInt(0, 21, Trick_CooldownRemaining,
                        "Set the amount of time remaining on Trick Attack cooldown before trying to set up with Suiton.");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_Bunshin:
                    UserConfig.DrawSliderInt(50, 100, Ninki_BunshinPoolingST,
                        "Set the amount of Ninki required to have before spending on Bunshin.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin:
                    UserConfig.DrawSliderInt(50, 100, Ninki_BunshinPoolingAoE,
                        "Set the amount of Ninki required to have before spending on Bunshin.");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Cooldowns:
                    UserConfig.DrawSliderInt(0, 21, Advanced_Trick_Cooldown,
                        "Set the amount of time remaining on Trick Attack cooldown to start saving cooldowns.");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_SecondWind:
                    UserConfig.DrawSliderInt(0, 100, SecondWindThresholdST,
                        "Set a HP% threshold for when Second Wind will be used.");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_ShadeShift:
                    UserConfig.DrawSliderInt(0, 100, ShadeShiftThresholdST,
                        "Set a HP% threshold for when Shade Shift will be used.");

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_Bloodbath:
                    UserConfig.DrawSliderInt(0, 100, BloodbathThresholdST,
                        "Set a HP% threshold for when Bloodbath will be used.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_SecondWind:
                    UserConfig.DrawSliderInt(0, 100, SecondWindThresholdAoE,
                        "Set a HP% threshold for when Second Wind will be used.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_ShadeShift:
                    UserConfig.DrawSliderInt(0, 100, ShadeShiftThresholdAoE,
                        "Set a HP% threshold for when Shade Shift will be used.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_Bloodbath:
                    UserConfig.DrawSliderInt(0, 100, BloodbathThresholdAoE,
                        "Set a HP% threshold for when Bloodbath will be used.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_HellfrogMedium:
                    UserConfig.DrawSliderInt(50, 100, Ninki_HellfrogPooling,
                        "Set the amount of Ninki required to have before spending on Hellfrog Medium.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton:
                    UserConfig.DrawSliderInt(0, 18, Advanced_DotonTimer,
                        "Sets the amount of time remaining on Doton before casting again.");

                    UserConfig.DrawSliderInt(0, 100, Advanced_DotonHP,
                        "Sets the max remaining HP percentage of the current target to cast Doton.");

                    break;

                case CustomComboPreset.NIN_AoE_AdvancedMode_TCJ:
                    UserConfig.DrawRadioButton(Advanced_TCJEnderAoE, "Ten Chi Jin Ender 1",
                        "Ends Ten Chi Jin with Suiton.", 0);

                    UserConfig.DrawRadioButton(Advanced_TCJEnderAoE, "Ten Chi Jin Ender 2",
                        "Ends Ten Chi Jin with Doton.\nIf you have Doton enabled, Ten Chi Jin will be delayed according to the settings in that feature.",
                        1);

                    break;

                case CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton:
                    UserConfig.DrawAdditionalBoolChoice(Advanced_ChargePool, "Pool Charges",
                        "Waits until at least 2 seconds before your 2nd charge or if Trick Attack debuff is on your target before spending.");

                    break;

                case CustomComboPreset.NIN_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, NIN_VariantCure, "HP% to be at or under", 200);

                    break;

                //PVP
                case CustomComboPreset.NINPvP_ST_SeitonTenchu:
                    UserConfig.DrawSliderInt(1, 50, NINPvP.Config.NINPVP_SeitonTenchu, "Target's HP% to be at or under", 200);
                    break;
                case CustomComboPreset.NINPvP_AoE_SeitonTenchu:
                    UserConfig.DrawSliderInt(1, 50, NINPvP.Config.NINPVP_SeitonTenchuAoE, "Target's HP% to be at or under", 200);
                    break;
            }
        }
    }
}
