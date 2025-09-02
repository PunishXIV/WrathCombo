using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.MNK_STUseOpener:
                    DrawHorizontalRadioButton(MNK_SelectedOpener,
                        "Double Lunar", "Uses Lunar/Lunar opener",
                        0);

                    DrawHorizontalRadioButton(MNK_SelectedOpener,
                        "Solar Lunar", "Uses Solar/Lunar opener",
                        1);

                    ImGui.NewLine();
                    DrawBossOnlyChoice(MNK_Balance_Content);
                    break;

                case Preset.MNK_STUseBrotherhood:
                    DrawHorizontalRadioButton(MNK_ST_BrotherhoodBossOption,
                        "All content", $"Uses {Brotherhood.ActionName()} regardless of content.", 0);

                    DrawHorizontalRadioButton(MNK_ST_BrotherhoodBossOption,
                        "Boss encounters Only", $"Only uses {Brotherhood.ActionName()} when in Boss encounters.", 1);
                    break;

                case Preset.MNK_STUseROF:
                    DrawHorizontalRadioButton(MNK_ST_RiddleOfFireBossOption,
                        "All content", $"Uses {RiddleOfFire.ActionName()}  regardless of content.", 0);

                    DrawHorizontalRadioButton(MNK_ST_RiddleOfFireBossOption,
                        "Boss encounters Only", $"Only uses {RiddleOfFire.ActionName()}  when in Boss encounters.", 1);
                    break;

                case Preset.MNK_STUseROW:
                    DrawHorizontalRadioButton(MNK_ST_RiddleOfWindBossOption,
                        "All content", $"Uses {RiddleOfWind.ActionName()}  regardless of content.", 0);

                    DrawHorizontalRadioButton(MNK_ST_RiddleOfWindBossOption,
                        "Boss encounters Only", $"Only uses {RiddleOfWind.ActionName()} when in Boss encounters.", 1);
                    break;

                case Preset.MNK_ST_ComboHeals:
                    DrawSliderInt(0, 100, MNK_ST_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, MNK_ST_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.MNK_AoEUseBrotherhood:
                    DrawSliderInt(0, 100, MNK_AoE_BrotherhoodHPThreshold,
                        $"Stop Using {Brotherhood.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MNK_AoEUseROF:
                    DrawSliderInt(0, 100, MNK_AoE_RiddleOfFireHPTreshold,
                        $"Stop Using {RiddleOfFire.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MNK_AoEUseROW:
                    DrawSliderInt(0, 100, MNK_AoE_RiddleOfWindHPTreshold,
                        $"Stop Using {RiddleOfWind.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MNK_AoE_ComboHeals:
                    DrawSliderInt(0, 100, MNK_AoE_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, MNK_AoE_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.MNK_Brotherhood_Riddle:
                    DrawRadioButton(MNK_BH_RoF,
                        $"Replaces {Brotherhood.ActionName()}", $"Replaces {Brotherhood.ActionName()} with {RiddleOfFire.ActionName()} when {Brotherhood.ActionName()} is on cooldown.", 0);

                    DrawRadioButton(MNK_BH_RoF,
                        $"Replaces {RiddleOfFire.ActionName()}", $"Replaces {RiddleOfFire.ActionName()} with {Brotherhood.ActionName()}when {RiddleOfFire.ActionName()} is on cooldown.", 1);
                    break;

                case Preset.MNK_Retarget_Thunderclap:
                    DrawAdditionalBoolChoice(MNK_Thunderclap_FieldMouseover,
                        "Add Field Mouseover", "Add Field Mouseover targetting");
                    break;

                case Preset.MNK_ST_BeastChakras:
                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        "Opo-opo Option", "Replace Bootshine / Leaping Opo with Dragon Kick.", 3, 0);

                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        "Raptor Option", "Replace True Strike/Rising Raptor with Twin Snakes.", 3, 1);

                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        "Coeurl Option", "Replace Snap Punch/Pouncing Coeurl with Demolish.", 3, 2);
                    break;
            }
        }
        #region Variables

        public static UserInt
            MNK_SelectedOpener = new("MNK_SelectedOpener", 0),
            MNK_Balance_Content = new("MNK_Balance_Content", 1),
            MNK_ST_BrotherhoodBossOption = new("MNK_ST_Brotherhood_SubOption", 1),
            MNK_ST_RiddleOfFireBossOption = new("MNK_ST_RiddleOfFire_SubOption", 1),
            MNK_ST_RiddleOfWindBossOption = new("MNK_ST_RiddleOfWind_SubOption", 1),
            MNK_ST_SecondWindHPThreshold = new("MNK_ST_SecondWindThreshold", 40),
            MNK_ST_BloodbathHPThreshold = new("MNK_ST_BloodbathThreshold", 30),
            MNK_AoE_BrotherhoodHPThreshold = new("MNK_AoE_Brotherhood_HP", 20),
            MNK_AoE_RiddleOfWindHPTreshold = new("MNK_AoE_RiddleOfWind_HP", 20),
            MNK_AoE_RiddleOfFireHPTreshold = new("MNK_AoE_RiddleOfFire_HP", 20),
            MNK_AoE_SecondWindHPThreshold = new("MNK_AoE_SecondWindThreshold", 40),
            MNK_AoE_BloodbathHPThreshold = new("MNK_AoE_BloodbathThreshold", 30),
            MNK_BH_RoF = new("MNK_BH_RoF", 0);

        public static UserBool
            MNK_Thunderclap_FieldMouseover = new("MNK_Thunderclap_FieldMouseover");

        public static UserBoolArray
            MNK_BasicCombo = new("MNK_BasicCombo");

        #endregion
    }
}
