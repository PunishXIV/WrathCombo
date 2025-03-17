using ECommons.ExcelServices;
using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    internal static class Config
    {
        public static UserInt
            SMN_ST_Lucid = new("SMN_ST_Lucid", 8000),
            SMN_ST_BurstPhase = new("SMN_ST_BurstPhase", 1),
            SMN_ST_PrimalChoice = new("SMN_PrimalChoice", 1),
            SMN_ST_SwiftcastPhase = new("SMN_SwiftcastPhase", 1),
            SMN_ST_Burst_Delay = new("SMN_Burst_Delay", 0),

            SMN_AoE_Lucid = new("SMN_AoE_Lucid", 8000),
            SMN_AoE_BurstPhase = new("SMN_AoE_BurstPhase", 1),
            SMN_AoE_PrimalChoice = new("SMN_AoE_PrimalChoice", 1),
            SMN_AoE_SwiftcastPhase = new("SMN_AoE_SwiftcastPhase", 1),
            SMN_AoE_Burst_Delay = new("SMN_AoE_Burst_Delay", 0),

            SMN_VariantCure = new("SMN_VariantCure"),
            SMN_Balance_Content = new("SMN_Balance_Content", 1);

        public static UserBoolArray
            SMN_ST_Egi_AstralFlow = new("SMN_ST_Egi_AstralFlow"),
            SMN_AoE_Egi_AstralFlow = new("SMN_AoE_Egi_AstralFlow");

        public static UserBool
            SMN_ST_CrimsonCycloneMelee = new("SMN_ST_CrimsonCycloneMelee"),
            SMN_AoE_CrimsonCycloneMelee = new("SMN_AoE_CrimsonCycloneMelee"),
            SMN_ST_Searing_Any = new("SMN_ST_Searing_Any"),
            SMN_AoE_Searing_Any = new("SMN_AoE_Searing_Any");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.SMN_ST_Advanced_Combo_Balance_Opener:
                    UserConfig.DrawBossOnlyChoice(SMN_Balance_Content);
                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_EgiOrder:
                    UserConfig.DrawHorizontalRadioButton(SMN_ST_PrimalChoice, "Titan first",
                        "Summons Titan, Garuda then Ifrit.", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_ST_PrimalChoice, "Garuda first",
                        "Summons Garuda, Titan then Ifrit.", 2);

                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_EgiOrder:
                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_PrimalChoice, "Titan first",
                        "Summons Titan, Garuda then Ifrit.", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_PrimalChoice, "Garuda first",
                        "Summons Garuda, Titan then Ifrit.", 2);

                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling:
                    UserConfig.DrawHorizontalRadioButton(SMN_ST_BurstPhase, "Solar Bahamut/Bahamut",
                        "Bursts during Bahamut phase.\nBahamut burst phase becomes Solar Bahamut at Lv100.", 1);
                    UserConfig.DrawHorizontalRadioButton(SMN_ST_BurstPhase, "Phoenix", "Bursts during Phoenix phase.", 2);

                    UserConfig.DrawHorizontalRadioButton(SMN_ST_BurstPhase, "Any Demi Phase",
                        "Bursts during any Demi Summon phase.", 3);

                    UserConfig.DrawHorizontalRadioButton(SMN_ST_BurstPhase, "Flexible (SpS) Option",
                        "Bursts when Searing Light is ready, regardless of phase.", 4);

                    UserConfig.DrawSliderInt(0, 3, SMN_ST_Burst_Delay,
                        "Sets the amount of GCDs under Demi summon to wait for oGCD use.");

                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling:
                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_BurstPhase, "Solar Bahamut/Bahamut",
                        "Bursts during Bahamut phase.\nBahamut burst phase becomes Solar Bahamut at Lv100.", 1);
                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_BurstPhase, "Phoenix", "Bursts during Phoenix phase.", 2);

                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_BurstPhase, "Any Demi Phase",
                        "Bursts during any Demi Summon phase.", 3);

                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_BurstPhase, "Flexible (SpS) Option",
                        "Bursts when Searing Light is ready, regardless of phase.", 4);

                    UserConfig.DrawSliderInt(0, 3, SMN_AoE_Burst_Delay,
                        "Sets the amount of GCDs under Demi summon to wait for oGCD use.");

                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi:
                    UserConfig.DrawHorizontalRadioButton(SMN_ST_SwiftcastPhase, "Garuda", "Swiftcasts Slipstream", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_ST_SwiftcastPhase, "Ifrit", "Swiftcasts Ruby Ruin/Ruby Rite",
                        2);

                    UserConfig.DrawHorizontalRadioButton(SMN_ST_SwiftcastPhase, "Flexible (SpS) Option",
                        "Swiftcasts the first available Egi when Swiftcast is ready.", 3);

                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi:
                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_SwiftcastPhase, "Garuda", "Swiftcasts Slipstream", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_SwiftcastPhase, "Ifrit", "Swiftcasts Ruby Ruin/Ruby Rite",
                        2);

                    UserConfig.DrawHorizontalRadioButton(SMN_AoE_SwiftcastPhase, "Flexible (SpS) Option",
                        "Swiftcasts the first available Egi when Swiftcast is ready.", 3);

                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_Lucid:
                    UserConfig.DrawSliderInt(4000, 9500, SMN_ST_Lucid,
                        "Set value for your MP to be at or under for this feature to take effect.", 150,
                        SliderIncrements.Hundreds);

                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_Lucid:
                    UserConfig.DrawSliderInt(4000, 9500, SMN_AoE_Lucid,
                        "Set value for your MP to be at or under for this feature to take effect.", 150,
                        SliderIncrements.Hundreds);

                    break;

                case CustomComboPreset.SMN_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, SMN_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow:
                    {
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Mountain Buster", "", 3, 0);
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Crimson Cyclone", "", 3, 1);
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Slipstream", "", 3, 2);

                        if (SMN_ST_Egi_AstralFlow[1])
                            UserConfig.DrawAdditionalBoolChoice(SMN_ST_CrimsonCycloneMelee,
                                "Enforced Crimson Cyclone Melee Check", "Only uses Crimson Cyclone within melee range.");

                        break;
                    }

                case CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow:
                    {
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Mountain Buster", "", 3, 0);
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Crimson Cyclone", "", 3, 1);
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Slipstream", "", 3, 2);

                        if (SMN_AoE_Egi_AstralFlow[1])
                            UserConfig.DrawAdditionalBoolChoice(SMN_AoE_CrimsonCycloneMelee,
                                "Enforced Crimson Cyclone Melee Check", "Only uses Crimson Cyclone within melee range.");

                        break;
                    }

                case CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight:
                    UserConfig.DrawAdditionalBoolChoice(SMN_ST_Searing_Any, $"Do not user when under another {Job.SMN.GetData().Abbreviation}'s {Buffs.SearingLight.StatusName()} buff.", $"Saves your {SearingLight.ActionName()} if you already have the buff from another {Job.SMN.GetData().Abbreviation}.");
                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight:
                    UserConfig.DrawAdditionalBoolChoice(SMN_AoE_Searing_Any, $"Do not user when under another {Job.SMN.GetData().Abbreviation}'s {Buffs.SearingLight.StatusName()} buff.", $"Saves your {SearingLight.ActionName()} if you already have the buff from another {Job.SMN.GetData().Abbreviation}.");
                    break;

                case CustomComboPreset.SMNPvP_BurstMode_RadiantAegis:
                    UserConfig.DrawSliderInt(0, 90, SMNPvP.Config.SMNPvP_RadiantAegisThreshold,
                        "Caps at 90 to prevent waste.");

                    break;
            }
        }
    }
}
