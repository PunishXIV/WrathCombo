using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    internal static class Config
    {
        public static UserInt
            SMN_ST_Advanced_Combo_AltMode = new("SMN_ST_Advanced_Combo_AltMode"),
            SMN_ST_Lucid = new("SMN_ST_Lucid", 8000),
            SMN_ST_BurstPhase = new("SMN_ST_BurstPhase", 1),
            SMN_ST_SwiftcastPhase = new("SMN_SwiftcastPhase", 1),
            SMN_ST_Burst_Delay = new("SMN_Burst_Delay", 0),
            SMN_Opener_SkipSwiftcast = new("SMN_Opener_SkipSwiftcast", 1),

            SMN_AoE_Lucid = new("SMN_AoE_Lucid", 8000),
            SMN_AoE_BurstPhase = new("SMN_AoE_BurstPhase", 1),
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

        internal static UserIntArray 
            SMN_ST_Egi_Priority = new("SMN_ST_Egi_Priority"),
            SMN_AoE_Egi_Priority = new ("SMN_AoE_Egi_Priority");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.SMN_ST_Advanced_Combo:
                    DrawRadioButton(SMN_ST_Advanced_Combo_AltMode, $"On Ruin 1, 2, and 3", "", 0);
                    DrawRadioButton(SMN_ST_Advanced_Combo_AltMode, $"On Ruin 1 and 2 Only", $"Alternative DPS Mode. Leaves Ruin 3 alone for pure DPS.", 1);
                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_Balance_Opener:
                    
                    UserConfig.DrawBossOnlyChoice(SMN_Balance_Content);

                    ImGui.NewLine();

                    UserConfig.DrawHorizontalRadioButton(SMN_Opener_SkipSwiftcast, "Use Swiftcast",
                        "Will use Swiftcast in opener to try and snapshot in pots for lower gcds", 1);

                    UserConfig.DrawHorizontalRadioButton(SMN_Opener_SkipSwiftcast, "Skip Swiftcast",
                        "Will not use swiftcast in opener for higher gcds", 2);
                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_Titan:                    
                    UserConfig.DrawPriorityInput(SMN_ST_Egi_Priority, 3, 0,
                        $"{SummonTopaz.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_Garuda:
                    UserConfig.DrawPriorityInput(SMN_ST_Egi_Priority, 3, 1,
                        $"{SummonEmerald.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.SMN_ST_Advanced_Combo_Ifrit:
                    UserConfig.DrawPriorityInput(SMN_ST_Egi_Priority, 3, 2,
                        $"{SummonRuby.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_Titan:
                    UserConfig.DrawPriorityInput(SMN_AoE_Egi_Priority, 3, 0,
                        $"{SummonTopaz.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_Garuda:
                    UserConfig.DrawPriorityInput(SMN_AoE_Egi_Priority, 3, 1,
                        $"{SummonEmerald.ActionName()} Priority: ");
                    break;

                case CustomComboPreset.SMN_AoE_Advanced_Combo_Ifrit:
                    UserConfig.DrawPriorityInput(SMN_AoE_Egi_Priority, 3, 2,
                        $"{SummonRuby.ActionName()} Priority: ");
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
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Mountain Buster", "", 4, 0);
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Crimson Cyclone", "", 4, 1);
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Crimson Strike", "", 4, 3);
                        UserConfig.DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Slipstream", "", 4, 2);

                        if (SMN_ST_Egi_AstralFlow[1])
                            UserConfig.DrawAdditionalBoolChoice(SMN_ST_CrimsonCycloneMelee,
                                "Enforced Crimson Cyclone Melee Check", "Only uses Crimson Cyclone within melee range.");

                        break;
                    }

                case CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow:
                    {
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Mountain Buster", "", 4, 0);
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Crimson Cyclone", "", 4, 1);
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Crimson Strike", "", 4, 3);
                        UserConfig.DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Slipstream", "", 4, 2);

                        if (SMN_AoE_Egi_AstralFlow[1])
                            UserConfig.DrawAdditionalBoolChoice(SMN_AoE_CrimsonCycloneMelee,
                                "Enforced Crimson Cyclone Melee Check", "Only uses Crimson Cyclone within melee range.");

                        break;
                    }  
            }
        }
    }
}
