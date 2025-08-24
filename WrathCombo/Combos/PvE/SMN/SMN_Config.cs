using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    internal static class Config
    {
        #region Options
        public static UserInt
            SMN_ST_Advanced_Combo_AltMode = new("SMN_ST_Advanced_Combo_AltMode"),
            SMN_ST_Lucid = new("SMN_ST_Lucid", 8000),
            SMN_ST_SwiftcastPhase = new("SMN_SwiftcastPhase", 1),
            SMN_ST_CrimsonCycloneMeleeDistance = new("SMN_ST_CrimsonCycloneMeleeDistance", 25),
            SMN_Opener_SkipSwiftcast = new("SMN_Opener_SkipSwiftcast", 1),

            SMN_AoE_Lucid = new("SMN_AoE_Lucid", 8000),
            SMN_AoE_CrimsonCycloneMeleeDistance = new("SMN_AoE_CrimsonCycloneMeleeDistance", 25),
            SMN_AoE_SwiftcastPhase = new("SMN_AoE_SwiftcastPhase", 1),

            SMN_Balance_Content = new("SMN_Balance_Content", 1);

        public static UserBoolArray
            SMN_ST_Egi_AstralFlow = new("SMN_ST_Egi_AstralFlow"),
            SMN_AoE_Egi_AstralFlow = new("SMN_AoE_Egi_AstralFlow");

        internal static UserIntArray
            SMN_ST_Egi_Priority = new("SMN_ST_Egi_Priority"),
            SMN_AoE_Egi_Priority = new("SMN_AoE_Egi_Priority");
        #endregion
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target
                case Preset.SMN_ST_Advanced_Combo:
                    DrawRadioButton(SMN_ST_Advanced_Combo_AltMode, "On Ruin 1, 2, and 3", "", 0);
                    DrawRadioButton(SMN_ST_Advanced_Combo_AltMode, "On Ruin 1 and 2 Only", "Alternative DPS Mode. Leaves Ruin 3 alone for pure DPS.", 1);
                    break;

                case Preset.SMN_ST_Advanced_Combo_Balance_Opener:
                    DrawBossOnlyChoice(SMN_Balance_Content);
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(SMN_Opener_SkipSwiftcast, "Use Swiftcast",
                        "Will use Swiftcast in opener to try and snapshot in pots for lower gcds", 1);
                    DrawHorizontalRadioButton(SMN_Opener_SkipSwiftcast, "Skip Swiftcast",
                        "Will not use swiftcast in opener for higher gcds", 2);
                    break;

                case Preset.SMN_ST_Advanced_Combo_Titan:
                    DrawPriorityInput(SMN_ST_Egi_Priority, 3, 0,
                        $"{SummonTopaz.ActionName()} Priority: ");
                    break;

                case Preset.SMN_ST_Advanced_Combo_Garuda:
                    DrawPriorityInput(SMN_ST_Egi_Priority, 3, 1,
                        $"{SummonEmerald.ActionName()} Priority: ");
                    break;

                case Preset.SMN_ST_Advanced_Combo_Ifrit:
                    DrawPriorityInput(SMN_ST_Egi_Priority, 3, 2,
                        $"{SummonRuby.ActionName()} Priority: ");
                    break;

                case Preset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi:
                    DrawHorizontalRadioButton(SMN_ST_SwiftcastPhase, "Garuda", "Swiftcasts Slipstream", 1);
                    DrawHorizontalRadioButton(SMN_ST_SwiftcastPhase, "Ifrit", "Swiftcasts Ruby Ruin/Ruby Rite", 2);
                    DrawHorizontalRadioButton(SMN_ST_SwiftcastPhase, "Flexible (SpS) Option",
                        "Swiftcasts the first available Egi when Swiftcast is ready.", 3);
                    break;

                case Preset.SMN_ST_Advanced_Combo_Lucid:
                    DrawSliderInt(4000, 9500, SMN_ST_Lucid, "Set value for your MP to be at or under for this feature to take effect.", 150,
                        SliderIncrements.Hundreds);
                    break;

                case Preset.SMN_ST_Advanced_Combo_Egi_AstralFlow:
                    DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Mountain Buster", "", 4, 0);
                    DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Crimson Cyclone", "", 4, 1);
                    DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Crimson Strike", "", 4, 3);
                    DrawHorizontalMultiChoice(SMN_ST_Egi_AstralFlow, "Add Slipstream", "", 4, 2);

                    if (SMN_ST_Egi_AstralFlow[1])
                    {
                        DrawSliderInt(0, 25, SMN_ST_CrimsonCycloneMeleeDistance, " Maximum range to use Crimson Cyclone.");
                    }
                    break;
                #endregion

                #region AoE
                case Preset.SMN_AoE_Advanced_Combo_Titan:
                    DrawPriorityInput(SMN_AoE_Egi_Priority, 3, 0,
                        $"{SummonTopaz.ActionName()} Priority: ");
                    break;

                case Preset.SMN_AoE_Advanced_Combo_Garuda:
                    DrawPriorityInput(SMN_AoE_Egi_Priority, 3, 1,
                        $"{SummonEmerald.ActionName()} Priority: ");
                    break;

                case Preset.SMN_AoE_Advanced_Combo_Ifrit:
                    DrawPriorityInput(SMN_AoE_Egi_Priority, 3, 2,
                        $"{SummonRuby.ActionName()} Priority: ");
                    break;

                case Preset.SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi:
                    DrawHorizontalRadioButton(SMN_AoE_SwiftcastPhase, "Garuda", "Swiftcasts Slipstream", 1);
                    DrawHorizontalRadioButton(SMN_AoE_SwiftcastPhase, "Ifrit", "Swiftcasts Ruby Ruin/Ruby Rite", 2);
                    DrawHorizontalRadioButton(SMN_AoE_SwiftcastPhase, "Flexible (SpS) Option",
                        "Swiftcasts the first available Egi when Swiftcast is ready.", 3);
                    break;

                case Preset.SMN_AoE_Advanced_Combo_Lucid:
                    DrawSliderInt(4000, 9500, SMN_AoE_Lucid, "Set value for your MP to be at or under for this feature to take effect.", 150,
                        SliderIncrements.Hundreds);
                    break;

                case Preset.SMN_AoE_Advanced_Combo_Egi_AstralFlow:
                    DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Mountain Buster", "", 4, 0);
                    DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Crimson Cyclone", "", 4, 1);
                    DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Crimson Strike", "", 4, 3);
                    DrawHorizontalMultiChoice(SMN_AoE_Egi_AstralFlow, "Add Slipstream", "", 4, 2);

                    if (SMN_AoE_Egi_AstralFlow[1])
                    {
                        DrawSliderInt(0, 25, SMN_AoE_CrimsonCycloneMeleeDistance, " Maximum range to use Crimson Cyclone.");
                    }
                    break;
                    #endregion

                    #region Standalones
                    #endregion
            }
        }
    }
}
