using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal static class Config
    {
        public static UserInt
            NIN_SimpleMudra_Choice = new("NIN_SimpleMudra_Choice", 1),
            Ninki_BhavaPooling = new("Ninki_BhavaPooling", 50),
            Trick_CooldownRemaining = new("Trick_CooldownRemaining", 10),
            Ninki_HellfrogPooling = new("Ninki_HellfrogPooling", 50),
            Ninki_BunshinPoolingST = new("Ninki_BunshinPoolingST", 50),
            Ninki_BunshinPoolingAoE = new("Ninki_BunshinPoolingAoE", 50),
            Advanced_Trick_Cooldown = new("Advanced_Trick_Cooldown", 15),
            Advanced_DotonTimer = new("Advanced_DotonTimer", 4),
            Advanced_DotonHP = new("Advanced_DotonHP", 20),
            BurnKazematoi = new("BurnKazematoi"),
            Advanced_TCJEnderAoE = new("Advanced_TCJEnderAoe", 0),
            Advanced_ChargePool = new("Advanced_ChargePool"),
            SecondWindThresholdST = new("SecondWindThresholdST", 20),
            ShadeShiftThresholdST = new("ShadeShiftThresholdST"),
            BloodbathThresholdST = new("BloodbathThresholdST"),
            SecondWindThresholdAoE = new("SecondWindThresholdAoE"),
            ShadeShiftThresholdAoE = new("ShadeShiftThresholdAoE"),
            BloodbathThresholdAoE = new("BloodbathThresholdAoE"),
            NIN_VariantCure = new("NIN_VariantCure"),
            NIN_Adv_Opener_Selection = new("NIN_Adv_Opener_Selection", 0);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.NIN_ST_AdvancedMode_BalanceOpener:
                    UserConfig.DrawRadioButton(NIN_Adv_Opener_Selection, $"Standard Opener - 4th GCD {KunaisBane.ActionName()}", "", 0);
                    UserConfig.DrawRadioButton(NIN_Adv_Opener_Selection, $"Standard Opener - 3rd GCD {Dokumori.ActionName()}", "", 1);
                    UserConfig.DrawRadioButton(NIN_Adv_Opener_Selection, $"Standard Opener - 3rd GCD {KunaisBane.ActionName()}", "", 2);

                    break;
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
