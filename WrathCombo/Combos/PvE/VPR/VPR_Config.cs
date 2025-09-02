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
                case Preset.VPR_ST_Opener:
                    DrawBossOnlyChoice(VPR_Balance_Content);

                    DrawAdditionalBoolChoice(VPR_Opener_ExcludeUF,
                        $"Exclude {UncoiledFury.ActionName()}", "");
                    break;

                case Preset.VPR_ST_SerpentsIre:
                    DrawHorizontalRadioButton(VPR_ST_SerpentsIre_SubOption,
                        "All content", $"Uses {SerpentsIre.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(VPR_ST_SerpentsIre_SubOption,
                        "Boss encounters Only", $"Only uses  {SerpentsIre.ActionName()} when in Boss encounters.", 1);
                    break;

                case Preset.VPR_ST_Reawaken:
                    DrawHorizontalRadioButton(VPR_ST_ReAwaken_SubOption,
                        "All content", $"Uses {Reawaken.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(VPR_ST_ReAwaken_SubOption,
                        "Boss encounters Only", $"Only uses {Reawaken.ActionName()} when in Boss encounters.", 1);

                    DrawSliderInt(0, 5, VPR_ST_ReAwaken_Threshold,
                        $"Set a HP% threshold to use {Reawaken.ActionName()} whenever available. (Bosses Only)");
                    break;

                case Preset.VPR_ST_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_ST_UncoiledFury_HoldCharges,
                        $"How many charges of {UncoiledFury.ActionName()} to keep ready? (0 = Use all)");

                    DrawSliderInt(0, 5, VPR_ST_UncoiledFury_Threshold,
                        $"Set a HP% Threshold to use all charges of {UncoiledFury.ActionName()}.");
                    break;

                case Preset.VPR_ST_RangedUptime:
                    DrawAdditionalBoolChoice(VPR_ST_RangedUptimeUncoiledFury,
                        $"Include {UncoiledFury.ActionName()}", "Adds Uncoiled Fury to the rotation when you are out of melee range and have Rattling Coil charges.");
                    break;

                case Preset.VPR_ST_Vicewinder:
                    DrawAdditionalBoolChoice(VPR_TrueNortVicewinder,
                        $"{Role.TrueNorth.ActionName()} Option", "Adds True North when available.");
                    break;

                case Preset.VPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, VPR_ST_SecondWind_Threshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, VPR_ST_Bloodbath_Threshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.VPR_AoE_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_AoE_UncoiledFury_HoldCharges,
                        $"How many charges of {UncoiledFury.ActionName()} to keep ready? (0 = Use all)");

                    DrawSliderInt(0, 5, VPR_AoE_UncoiledFury_Threshold,
                        $"Set a HP% Threshold to use all charges of {UncoiledFury.ActionName()}.");
                    break;

                case Preset.VPR_AoE_Reawaken:
                    DrawHorizontalRadioButton(VPR_AoE_Reawaken_SubOption,
                        "In range", $"Adds range check for {Reawaken.ActionName()}, so it is used only when in range.", 0);

                    DrawHorizontalRadioButton(VPR_AoE_Reawaken_SubOption,
                        "Disable range check", $"Disables the range check for {Reawaken.ActionName()}, so it will be used even without a target selected.", 1);

                    DrawSliderInt(0, 100, VPR_AoE_Reawaken_Usage,
                        $"Stop using {Reawaken.ActionName()} at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case Preset.VPR_AoE_Vicepit:
                    DrawHorizontalRadioButton(VPR_AoE_Vicepit_SubOption,
                        "In range", $"Adds range check for {Vicepit.ActionName()}, so it is used only when in range.", 0);

                    DrawHorizontalRadioButton(VPR_AoE_Vicepit_SubOption,
                        "Disable range check", $"Disables the range check for {Vicepit.ActionName()}, so it will be used even without a target selected.", 1);
                    break;

                case Preset.VPR_AoE_VicepitCombo:
                    DrawHorizontalRadioButton(VPR_AoE_VicepitCombo_SubOption,
                        "In range", $"Adds range check for {HuntersDen.ActionName()} and {SwiftskinsDen.ActionName()}, so it is used only when in range.", 0);

                    DrawHorizontalRadioButton(VPR_AoE_VicepitCombo_SubOption,
                        "Disable range check", $"Disables the range check for {HuntersDen.ActionName()} and {SwiftskinsDen.ActionName()}, so it will be used even without a target selected.", 1);
                    break;

                case Preset.VPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, VPR_AoE_SecondWind_Threshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, VPR_AoE_Bloodbath_Threshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

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
            }
        }

        #region Variables

        public static UserInt
            VPR_Balance_Content = new("VPR_Balance_Content", 1),
            VPR_ST_SerpentsIre_SubOption = new("VPR_ST_SerpentsIre_SubOption", 1),
            VPR_ST_UncoiledFury_HoldCharges = new("VPR_ST_UncoiledFury_HoldCharges", 1),
            VPR_ST_UncoiledFury_Threshold = new("VPR_ST_UncoiledFury_Threshold", 1),
            VPR_ST_ReAwaken_SubOption = new("VPR_ST_ReAwaken_SubOption", 0),
            VPR_ST_ReAwaken_Threshold = new("VPR_ST_ReAwaken_Threshold", 1),
            VPR_ST_SecondWind_Threshold = new("VPR_ST_SecondWindThreshold", 40),
            VPR_ST_Bloodbath_Threshold = new("VPR_ST_BloodbathThreshold", 30),
            VPR_AoE_UncoiledFury_Threshold = new("VPR_AoE_UncoiledFury_Threshold", 1),
            VPR_AoE_UncoiledFury_HoldCharges = new("VPR_AoE_UncoiledFury_HoldCharges", 0),
            VPR_AoE_Vicepit_SubOption = new("VPR_AoE_Vicepit_SubOption", 0),
            VPR_AoE_VicepitCombo_SubOption = new("VPR_AoE_VicepitCombo_SubOption", 0),
            VPR_AoE_Reawaken_Usage = new("VPR_AoE_Reawaken_Usage", 20),
            VPR_AoE_Reawaken_SubOption = new("VPR_AoE_Reawaken_SubOption", 0),
            VPR_AoE_SecondWind_Threshold = new("VPR_AoE_SecondWindThreshold", 40),
            VPR_AoE_Bloodbath_Threshold = new("VPR_AoE_BloodbathThreshold", 30),
            VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton", 0);

        public static UserBool
            VPR_Opener_ExcludeUF = new("VPR_Opener_ExcludeUF"),
            VPR_ST_RangedUptimeUncoiledFury = new("VPR_ST_RangedUptimeUncoiledFury"),
            VPR_TrueNortVicewinder = new("VPR_TrueNortVicewinder"),
            VPR_Slither_FieldMouseover = new("VPR_Slither_FieldMouseover");

        #endregion
    }
}
