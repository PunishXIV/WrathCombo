using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
namespace WrathCombo.Combos.PvE;

internal partial class All
{
    internal static class Config
    {
        public static readonly UserInt ALL_Tank_Reprisal_Threshold =
            new("ALL_Tank_Reprisal_Threshold");
        
        public static readonly UserBoolArray ALL_Healer_RescueRetargetingOptions = new("ALL_Healer_RescueRetargetingOptions");
            

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.ALL_Tank_Reprisal:
                    UserConfig.DrawSliderInt(0, 9, ALL_Tank_Reprisal_Threshold,
                        "Time Remaining on others' Reprisal to allow within\n(0=Reprisal must not be on the target)");
                    break;
                
                case Preset.ALL_Healer_RescueRetargeting:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow,"UI Mouseover > Field Mouseover > Focus Target > Soft Target > Hard Target");
                    ImGui.Unindent();
                    UserConfig.DrawHorizontalMultiChoice(ALL_Healer_RescueRetargetingOptions,"Field Mouseover", "Will add Field Mouseover to the priority stack", 3, 0);
                    UserConfig.DrawHorizontalMultiChoice(ALL_Healer_RescueRetargetingOptions,"Focus Target", "Will add Focus Target to the priority stack", 3, 1);
                    UserConfig.DrawHorizontalMultiChoice(ALL_Healer_RescueRetargetingOptions,"Soft Target", "Will add Soft Target to the priority stack", 3, 2);
                    break;
            }
        }
    }
}
