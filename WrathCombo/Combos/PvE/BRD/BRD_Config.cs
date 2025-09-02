using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
    internal static class Config
    {
        #region Options

        public static UserBool
            BRD_AoE_Wardens_Auto = new("BRD_AoE_Wardens_Auto"),
            BRD_ST_Wardens_Auto = new("BRD_ST_Wardens_Auto"),
            BRD_IronJaws_Apex = new("BRD_IronJaws_Apex"),
            BRD_IronJaws_Alternate = new("BRD_IronJaws_Alternate");

        public static UserInt
            BRD_RagingJawsRenewTime = new("ragingJawsRenewTime", 5),
            BRD_STSecondWindThreshold = new("BRD_STSecondWindThreshold", 40),
            BRD_AoESecondWindThreshold = new("BRD_AoESecondWindThreshold", 40),
            BRD_Adv_Opener_Selection = new("BRD_Adv_Opener_Selection", 0),
            BRD_Balance_Content = new("BRD_Balance_Content", 1),
            BRD_Adv_DoT_Threshold = new("BRD_Adv_DoT_Threshold", 30),
            BRD_Adv_DoT_SubOption = new("BRD_Adv_DoT_SubOption", 0),
            BRD_Adv_Buffs_Threshold = new("BRD_Adv_Buffs_Threshold", 30),
            BRD_Adv_Buffs_SubOption = new("BRD_Adv_Buffs_SubOption", 0),
            BRD_AoE_Adv_Buffs_Threshold = new("BRD_AoE_Adv_Buffs_Threshold", 30),
            BRD_AoE_Adv_Buffs_SubOption = new("BRD_AoE_Adv_Buffs_SubOption", 0),
            BRD_AoE_Adv_Multidot_HPThreshold = new("BRD_AoE_Adv_Multidot_HPThreshold", 40);

        public static UserBoolArray
            BRD_AoE_Adv_Buffs_Options = new("BRD_AoE_Adv_Buffs_Options"),
            BRD_Adv_Buffs_Options = new("BRD_Adv_Buffs_Options"),
            BRD_Adv_DoT_Options = new("BRD_Adv_DoT_Options"),
            BRD_StraightShotUpgrade_OGCDs_Options = new("BRD_StraightShotUpgrade_OGCDs_Options"),
            BRD_WideVolleyUpgrade_OGCDs_Options = new("BRD_WideVolleyUpgrade_OGCDs_Options");
        #endregion

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target
                case Preset.BRD_ST_Adv_Balance_Standard:
                    DrawRadioButton(BRD_Adv_Opener_Selection, "Standard Opener", "", 0);
                    DrawRadioButton(BRD_Adv_Opener_Selection, "2.48 Adjusted Standard Opener", "", 1);
                    DrawRadioButton(BRD_Adv_Opener_Selection, "2.49 Standard Comfy", "", 2);
                    ImGui.Indent();
                    DrawBossOnlyChoice(BRD_Balance_Content);
                    ImGui.Unindent();
                    break;

                case Preset.BRD_Adv_DoT:
                    DrawSliderInt(0, 100, BRD_Adv_DoT_Threshold,
                        $"Stop using Dots on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(BRD_Adv_DoT_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(BRD_Adv_DoT_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(BRD_Adv_DoT_Options, "Iron Jaws Option", "Enable the refreshing of dots with Ironjaws", 4, 0);
                    DrawHorizontalMultiChoice(BRD_Adv_DoT_Options, "Dot Application Option", "Enable the application of dots outside of the opener", 4, 1);
                    DrawHorizontalMultiChoice(BRD_Adv_DoT_Options, "Raging Jaws Optionn", "Enable the snapshotting of DoTs, within the remaining time of Raging Strikes", 4, 2);
                    DrawHorizontalMultiChoice(BRD_Adv_DoT_Options, "MultiDot Option", "Will maintain dots on up to 3 targets.", 4, 3);

                    if (BRD_Adv_DoT_Options[2])
                    {
                        DrawSliderInt(3, 10, BRD_RagingJawsRenewTime, "Raging Jaws: Renew time (In seconds). \nRecommended 5, increase little by little if refresh is outside of radiant window");
                    }
                    break;

                case Preset.BRD_Adv_Buffs:
                    DrawSliderInt(0, 100, BRD_Adv_Buffs_Threshold,
                       $"Stop using Buffs on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(BRD_Adv_Buffs_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(BRD_Adv_Buffs_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(BRD_Adv_Buffs_Options, "Raging Strikes Option", "Adds Raging Strikes", 4, 0);
                    DrawHorizontalMultiChoice(BRD_Adv_Buffs_Options, "Battlevoice Option", "Adds Battle Voice", 4, 1);
                    DrawHorizontalMultiChoice(BRD_Adv_Buffs_Options, "Barrage Option", "Adds Barrage", 4, 2);
                    DrawHorizontalMultiChoice(BRD_Adv_Buffs_Options, "Radiant Finale Option", "Adds Radiant Finale", 4, 3);
                    break;

                case Preset.BRD_ST_SecondWind:
                    DrawSliderInt(0, 100, BRD_STSecondWindThreshold,
                        "HP percent threshold to use Second Wind below.");
                    break;

                case Preset.BRD_ST_Wardens:
                    DrawAdditionalBoolChoice(BRD_ST_Wardens_Auto, "Party Cleanse Option", "Uses Wardens Paeon when someone in the party has a cleansable debuff using the Retargeting Function following party list.");
                    break;
                #endregion

                #region AOE
                case Preset.BRD_AoE_Adv_Buffs:
                    DrawSliderInt(0, 100, BRD_AoE_Adv_Buffs_Threshold,
                        $"Stop using Buffs on targets below this HP % (0 = always use, 100 = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(BRD_AoE_Adv_Buffs_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(BRD_AoE_Adv_Buffs_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(BRD_AoE_Adv_Buffs_Options, "Raging Strikes Option", "Adds Raging Strikes", 4, 0);
                    DrawHorizontalMultiChoice(BRD_AoE_Adv_Buffs_Options, "Battlevoice Option", "Adds Battle Voice", 4, 1);
                    DrawHorizontalMultiChoice(BRD_AoE_Adv_Buffs_Options, "Barrage Option", "Adds Barrage", 4, 2);
                    DrawHorizontalMultiChoice(BRD_AoE_Adv_Buffs_Options, "Radiant Finale Option", "Adds Radiant Finale", 4, 3);
                    break;

                case Preset.BRD_AoE_SecondWind:
                    DrawSliderInt(0, 100, BRD_AoESecondWindThreshold,
                        "HP percent threshold to use Second Wind below.");
                    break;

                case Preset.BRD_AoE_Adv_Multidot:
                    DrawSliderInt(0, 100, BRD_AoE_Adv_Multidot_HPThreshold, "Target HP% to stop using (0 = Use Always, 100 = Never)");
                    break;

                case Preset.BRD_AoE_Wardens:
                    DrawAdditionalBoolChoice(BRD_AoE_Wardens_Auto, "Party Cleanse Option", "Uses Wardens Paeon when someone in the party has a cleansable debuff using the Retargeting Function following party list.");
                    break;
                #endregion
                
                #region Standalone
                case Preset.BRD_StraightShotUpgrade_OGCDs:
                    DrawHorizontalMultiChoice(BRD_StraightShotUpgrade_OGCDs_Options, "Empyreal Arrow", "Adds Empyreal Arrow", 4, 0);
                    DrawHorizontalMultiChoice(BRD_StraightShotUpgrade_OGCDs_Options, "Pitch Perfect", "Adds Pitch Perfect", 4, 1);
                    DrawHorizontalMultiChoice(BRD_StraightShotUpgrade_OGCDs_Options, "Bloodletter", "Adds Bloodletter when at max charges", 4, 2);
                    DrawHorizontalMultiChoice(BRD_StraightShotUpgrade_OGCDs_Options, "Sidewinder", "Adds Sidewinder", 4, 3);
                    break;
                
                case Preset.BRD_WideVolleyUpgrade_OGCDs:
                    DrawHorizontalMultiChoice(BRD_WideVolleyUpgrade_OGCDs_Options, "Empyreal Arrow", "Adds Empyreal Arrow", 4, 0);
                    DrawHorizontalMultiChoice(BRD_WideVolleyUpgrade_OGCDs_Options, "Pitch Perfect", "Adds Pitch Perfect", 4, 1);
                    DrawHorizontalMultiChoice(BRD_WideVolleyUpgrade_OGCDs_Options, "Rain Of Death", "Adds Rain of Death when at max charges, or bloodletter below level.", 4, 2);
                    DrawHorizontalMultiChoice(BRD_WideVolleyUpgrade_OGCDs_Options, "Sidewinder", "Adds Sidewinder", 4, 3);
                    break;
                
                case Preset.BRD_IronJaws:
                    DrawAdditionalBoolChoice(BRD_IronJaws_Alternate, "Iron Jaws Alternate", "Iron Jaws will only show up when debuffs are about to expire.", 0);
                    DrawAdditionalBoolChoice(BRD_IronJaws_Apex, "Apex Option", "Adds Apex and Blast Arrow to Iron Jaws when available.", 0);
                    break;
                
                #endregion
            }
        }
    }
}
