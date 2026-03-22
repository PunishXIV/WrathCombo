using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
namespace WrathCombo.Combos.PvE;

internal partial class All
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.ALL_Tank_Reprisal:
                    UserConfig.DrawSliderInt(0, 9, AllTankReprisalThreshold,
                        "Time Remaining on others Reprisal to allow within\n(0 = Reprisal must not be on the target)");
                    break;

                case Preset.ALL_Caster_Addle:
                    UserConfig.DrawSliderInt(0, 5, AllCasterAddleThreshold,
                        "Time Remaining on others Addle to allow within\n(0 = Addle must not be on the target)");
                    break;

                case Preset.ALL_Melee_Feint:
                    UserConfig.DrawSliderInt(0, 5, AllMeleeFeintThreshold,
                        "Time Remaining on others Feint to allow within\n(0 = Feint must not be on the target)");
                    break;

                case Preset.ALL_Ranged_Mitigation:
                    UserConfig.DrawSliderInt(0, 5, AllRangedMitigationThreshold,
                        "Time Remaining on others Troubadour / Tactician / Shield Samba to allow within\n(0 = Troubadour / Tactician / Shield Samba must not be on the target)");
                    break;

                case Preset.ALL_Healer_RescueRetargeting:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, "UI Mouseover > Field Mouseover > Focus Target > Soft Target > Hard Target");
                    ImGui.Unindent();
                    UserConfig.DrawHorizontalMultiChoice(AllHealerRescueRetargetingOptions, "Field Mouseover", "Will add Field Mouseover to the priority stack", 3, 0);
                    UserConfig.DrawHorizontalMultiChoice(AllHealerRescueRetargetingOptions, "Focus Target", "Will add Focus Target to the priority stack", 3, 1);
                    UserConfig.DrawHorizontalMultiChoice(AllHealerRescueRetargetingOptions, "Soft Target", "Will add Soft Target to the priority stack", 3, 2);
                    break;
            }
        }

        #region Variables

        public static readonly UserInt
            AllTankReprisalThreshold = new("AllTankReprisalThreshold"),
            AllCasterAddleThreshold = new("AllCasterAddleThreshold"),
            AllMeleeFeintThreshold = new("AllMeleeFeintThreshold"),
            AllRangedMitigationThreshold = new("AllRangedMitigationThreshold");

        public static readonly UserBoolArray AllHealerRescueRetargetingOptions = new("ALL_Healer_RescueRetargetingOptions");

        #endregion
    }
}
