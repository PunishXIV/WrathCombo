using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    internal static class Config
    {
        public static UserInt
            SMN_Lucid = new("SMN_Lucid"),
            SMN_BurstPhase = new("SMN_BurstPhase"),
            SMN_PrimalChoice = new("SMN_PrimalChoice"),
            SMN_SwiftcastPhase = new("SMN_SwiftcastPhase"),
            SMN_Burst_Delay = new("SMN_Burst_Delay"),
            SMN_VariantCure = new("SMN_VariantCure");

        public static UserBoolArray
            SMN_ST_Egi_AstralFlow = new("SMN_ST_Egi_AstralFlow");

        public static UserBool
            SMN_ST_CrimsonCycloneMelee = new("SMN_ST_CrimsonCycloneMelee");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.SMN_DemiEgiMenu_EgiOrder:
                    UserConfig.DrawHorizontalRadioButton(SMN_PrimalChoice, "Titan first",
                        "Summons Titan, Garuda then Ifrit.", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_PrimalChoice, "Garuda first",
                        "Summons Garuda, Titan then Ifrit.", 2);

                    break;

                case CustomComboPreset.SMN_DemiEgiMenu_oGCDPooling:
                    UserConfig.DrawSliderInt(0, 3, SMN_Burst_Delay,
                        "Sets the amount of GCDs under Demi summon to wait for oGCD use.");

                    UserConfig.DrawHorizontalRadioButton(SMN_BurstPhase, "Solar Bahamut/Bahamut",
                        "Bursts during Bahamut phase.\nBahamut burst phase becomes Solar Bahamut at Lv100.", 1);
                    UserConfig.DrawHorizontalRadioButton(SMN_BurstPhase, "Phoenix", "Bursts during Phoenix phase.", 2);

                    UserConfig.DrawHorizontalRadioButton(SMN_BurstPhase, "Any Demi Phase",
                        "Bursts during any Demi Summon phase.", 3);

                    UserConfig.DrawHorizontalRadioButton(SMN_BurstPhase, "Flexible (SpS) Option",
                        "Bursts when Searing Light is ready, regardless of phase.", 4);

                    break;

                case CustomComboPreset.SMN_DemiEgiMenu_SwiftcastEgi:
                    UserConfig.DrawHorizontalRadioButton(SMN_SwiftcastPhase, "Garuda", "Swiftcasts Slipstream", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_SwiftcastPhase, "Ifrit", "Swiftcasts Ruby Ruin/Ruby Rite",
                        2);

                    UserConfig.DrawHorizontalRadioButton(SMN_SwiftcastPhase, "Flexible (SpS) Option",
                        "Swiftcasts the first available Egi when Swiftcast is ready.", 3);

                    break;

                case CustomComboPreset.SMN_Lucid:
                    UserConfig.DrawSliderInt(4000, 9500, SMN_Lucid,
                        "Set value for your MP to be at or under for this feature to take effect.", 150,
                        SliderIncrements.Hundreds);

                    break;

                case CustomComboPreset.SMN_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, SMN_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.SMN_ST_Egi_AstralFlow:
                {
                    UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Mountain Buster", "", 3, 0);
                    UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Crimson Cyclone", "", 3, 1);
                    UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Slipstream", "", 3, 2);

                    if (SMN_ST_Egi_AstralFlow[1])
                        UserConfig.DrawAdditionalBoolChoice(SMN_ST_CrimsonCycloneMelee,
                            "Enforced Crimson Cyclone Melee Check", "Only uses Crimson Cyclone within melee range.");

                    break;
                }

                case CustomComboPreset.SMNPvP_BurstMode:
                    UserConfig.DrawSliderInt(50, 100, SMNPvP.Config.SMNPvP_FesterThreshold,
                        "Target HP% to cast Fester below.\nSet to 100 use Fester as soon as it's available.");

                    break;

                case CustomComboPreset.SMNPvP_BurstMode_RadiantAegis:
                    UserConfig.DrawSliderInt(0, 90, SMNPvP.Config.SMNPvP_RadiantAegisThreshold,
                        "Caps at 90 to prevent waste.");

                    break;
            }
        }
    }
}
