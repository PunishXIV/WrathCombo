using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.VPR_ST_Opener:
                    DrawBossOnlyChoice(VPR_Balance_Content);

                    DrawAdditionalBoolChoice(VPR_Opener_ExcludeUF,
                        $"Exclude {UncoiledFury.ActionName()}", "");
                    break;

                case Preset.VPR_ST_SerpentsIre:
                    DrawSliderInt(0, 50, VPR_ST_SerpentsIreHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(VPR_ST_SerpentsIreBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(VPR_ST_SerpentsIreBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.VPR_ST_Reawaken:
                    DrawSliderInt(0, 100, VPR_ST_ReawakenBossOption,
                        "Bosses Only. Stop using at Enemy HP %.");

                    DrawSliderInt(0, 100, VPR_ST_ReawakenBossAddsOption,
                        "Boss Encounter Non Bosses. Stop using at Enemy HP %.");

                    DrawSliderInt(0, 100, VPR_ST_ReawakenTrashOption,
                        "Non boss encounter. Stop using at Enemy HP %.");

                    DrawSliderInt(0, 5, VPR_ST_ReAwakenAlwaysUse,
                        $"Set a HP% threshold to use {Reawaken.ActionName()} whenever available. (Bosses Only)");
                    break;

                case Preset.VPR_ST_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_ST_UncoiledFuryHoldCharges,
                        $"How many charges of {UncoiledFury.ActionName()} to keep ready? (0 = Use all)");

                    DrawSliderInt(0, 5, VPR_ST_UncoiledFuryAlwaysUse,
                        $"Set a HP% Threshold to use all charges of {UncoiledFury.ActionName()}.");
                    break;

                case Preset.VPR_ST_Vicewinder:
                    DrawAdditionalBoolChoice(VPR_TrueNorthVicewinder,
                        $"{Role.TrueNorth.ActionName()} Option", "Adds True North when available.\n Respects the manual TN charge.");
                    break;

                case Preset.VPR_ST_VicewinderCombo:
                    DrawAdditionalBoolChoice(VPR_VicewinderBuffPrio,
                        "Buff Prio Option", "Forces HuntersCoil or SwiftskinsCoil if buff needs to be reapplied.");
                    break;

                case Preset.VPR_TrueNorthDynamic:
                    DrawSliderInt(0, 1, VPR_ManualTN,
                        "How many charges to keep for manual usage.");

                    DrawAdditionalBoolChoice(VPR_ST_TrueNorthDynamicHoldCharge,
                        "Hold True North for Vicewinder Option", "Will hold the last charge of True North for use with Vicewinder, even when out of position for normal GCD.\n" +
                                                                 "If Above Slider is set to 1, it will NOT use the remaining charge for Vicewinder, but for manual use.");
                    break;

                case Preset.VPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, VPR_ST_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, VPR_ST_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                #endregion

                #region AoE

                case Preset.VPR_AoE_SerpentsIre:
                    DrawSliderInt(0, 100, VPR_AoE_SerpentsIreHPThreshold,
                        "Stop using when target HP% is at or below (Set to 0 to Disable This Check)");
                    break;

                case Preset.VPR_AoE_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_AoE_UncoiledFuryHoldCharges,
                        $"How many charges of {UncoiledFury.ActionName()} to keep ready? (0 = Use all)");

                    DrawSliderInt(0, 5, VPR_AoE_UncoiledFuryAlwaysUse,
                        $"Set a HP% Threshold to use all charges of {UncoiledFury.ActionName()}.");
                    break;

                case Preset.VPR_AoE_Reawaken:
                    DrawHorizontalRadioButton(VPR_AoE_ReawakenRangecheck,
                        "In range", $"Adds range check for {Reawaken.ActionName()}, so it is used only when in range.", 0);

                    DrawHorizontalRadioButton(VPR_AoE_ReawakenRangecheck,
                        "Disable range check", $"Disables the range check for {Reawaken.ActionName()}, so it will be used even without a target selected.", 1);

                    DrawSliderInt(0, 100, VPR_AoE_ReawakenHPThreshold,
                        $"Stop using {Reawaken.ActionName()} at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case Preset.VPR_AoE_Vicepit:
                    DrawHorizontalRadioButton(VPR_AoE_VicepitRangeCheck,
                        "In range", $"Adds range check for {Vicepit.ActionName()}, so it is used only when in range.", 0);

                    DrawHorizontalRadioButton(VPR_AoE_VicepitRangeCheck,
                        "Disable range check", $"Disables the range check for {Vicepit.ActionName()}, so it will be used even without a target selected.", 1);
                    break;

                case Preset.VPR_AoE_VicepitCombo:
                    DrawHorizontalRadioButton(VPR_AoE_VicepitComboRangeCheck,
                        "In range", $"Adds range check for {HuntersDen.ActionName()} and {SwiftskinsDen.ActionName()}, so it is used only when in range.", 0);

                    DrawHorizontalRadioButton(VPR_AoE_VicepitComboRangeCheck,
                        "Disable range check", $"Disables the range check for {HuntersDen.ActionName()} and {SwiftskinsDen.ActionName()}, so it will be used even without a target selected.", 1);
                    break;

                case Preset.VPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, VPR_AoE_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, VPR_AoE_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                #endregion

                #region Misc

                case Preset.VPR_ReawakenLegacy:
                    DrawRadioButton(VPR_ReawakenLegacyButton,
                        $"Replaces {Reawaken.ActionName()}", $"Replaces {Reawaken.ActionName()} with Full Generation - Legacy combo.", 0);

                    DrawRadioButton(VPR_ReawakenLegacyButton,
                        $"Replaces {ReavingFangs.ActionName()}", $"Replaces {ReavingFangs.ActionName()} with Full Generation - Legacy combo.", 1);
                    break;

                case Preset.VPR_Retarget_Slither:
                    DrawAdditionalBoolChoice(VPR_Slither_FieldMouseover,
                        "Add Field Mouseover", "Add Field Mouseover targetting");
                    break;

                #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            VPR_Balance_Content = new("VPR_Balance_Content", 1),
            VPR_ST_UncoiledFuryHoldCharges = new("VPR_ST_UncoiledFuryHoldCharges", 1),
            VPR_ST_UncoiledFuryAlwaysUse = new("VPR_ST_UncoiledFuryAlwaysUse", 5),
            VPR_ST_ReawakenBossOption = new("VPR_ST_ReawakenBossOption"),
            VPR_ST_ReawakenBossAddsOption = new("VPR_ST_ReawakenBossAddsOption", 10),
            VPR_ST_ReawakenTrashOption = new("VPR_ST_ReawakenTrashOption", 25),
            VPR_ST_ReAwakenAlwaysUse = new("VPR_ST_ReAwakenAlwaysUse", 5),
            VPR_ST_SerpentsIreHPOption = new("VPR_ST_SerpentsIreHPOption", 25),
            VPR_ST_SerpentsIreBossOption = new("VPR_ST_SerpentsIreBossOption"),
            VPR_ManualTN = new("VPR_ManualTN"),
            VPR_ST_SecondWindHPThreshold = new("VPR_ST_SecondWindHPThreshold", 40),
            VPR_ST_BloodbathHPThreshold = new("VPR_ST_BloodbathHPThreshold", 30),

            //AoE
            VPR_AoE_SerpentsIreHPThreshold = new("VPR_AoE_SerpentsIreHPThreshold", 25),
            VPR_AoE_UncoiledFuryAlwaysUse = new("VPR_AoE_UncoiledFuryAlwaysUse", 5),
            VPR_AoE_UncoiledFuryHoldCharges = new("VPR_AoE_UncoiledFuryHoldCharges"),
            VPR_AoE_VicepitRangeCheck = new("VPR_AoE_VicepitRangeCheck"),
            VPR_AoE_VicepitComboRangeCheck = new("VPR_AoE_VicepitComboRangeCheck"),
            VPR_AoE_ReawakenHPThreshold = new("VPR_AoE_ReawakenHPThreshold", 25),
            VPR_AoE_ReawakenRangecheck = new("VPR_AoE_ReawakenRangecheck"),
            VPR_AoE_SecondWindHPThreshold = new("VPR_AoE_SecondWindHPThreshold", 40),
            VPR_AoE_BloodbathHPThreshold = new("VPR_AoE_BloodbathHPThreshold", 30),

            //Misc
            VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton");

        public static UserBool
            VPR_Opener_ExcludeUF = new("VPR_Opener_ExcludeUF"),
            VPR_TrueNorthVicewinder = new("VPR_TrueNorthVicewinder"),
            VPR_Slither_FieldMouseover = new("VPR_Slither_FieldMouseover"),
            VPR_ST_TrueNorthDynamicHoldCharge = new("VPR_ST_TrueNorthDynamicHoldCharge"),
            VPR_VicewinderBuffPrio = new("VPR_VicewinderBuffPrio");

        #endregion
    }
}
