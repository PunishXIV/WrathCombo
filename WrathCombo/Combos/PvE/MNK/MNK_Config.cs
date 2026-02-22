using Dalamud.Interface.Colors;
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
                #region ST

                case Preset.MNK_STUseOpener:
                    DrawHorizontalRadioButton(MNK_SelectedOpener,
                        "Double Lunar", "Uses Lunar/Lunar opener.", 0);

                    DrawHorizontalRadioButton(MNK_SelectedOpener,
                        "Solar Lunar", "Uses Solar/Lunar opener.", 1);
                    ImGui.NewLine();

                    DrawHorizontalRadioButton(MNK_OpenerCountdown,
                        "Only with countdown", "Only use the opener when a countdown is active.", 0);

                    DrawHorizontalRadioButton(MNK_OpenerCountdown,
                        "Always", "Always use opener.", 1);

                    ImGui.NewLine();
                    DrawBossOnlyChoice(MNK_Balance_Content);
                    break;

                case Preset.MNK_STUseBrotherhood:

                    DrawSliderInt(0, 50, MNK_ST_BHHPThreshold,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(MNK_ST_BHBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(MNK_ST_BHBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.MNK_STUseROF:

                    DrawSliderInt(0, 50, MNK_ST_RoFHPThreshold,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(MNK_ST_RoFBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(MNK_ST_RoFBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.MNK_STUseROW:

                    DrawSliderInt(0, 50, MNK_ST_RoWHPThreshold,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(MNK_ST_RoWBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(MNK_ST_RoWBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    ImGui.Unindent();
                    break;

                case Preset.MNK_STUseTrueNorth:
                    DrawSliderInt(0, 1, MNK_ManualTN,
                        "How many charges to keep for manual usage.");
                    break;

                case Preset.MNK_ST_ComboHeals:
                    DrawSliderInt(0, 100, MNK_ST_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, MNK_ST_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                #endregion

                #region AoE

                case Preset.MNK_AoEUseBuffs:
                    DrawSliderInt(0, 100, MNK_AoE_BuffsHPThreshold,
                        "Stop Using Buffs When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MNK_AoEUsePerfectBalance:
                    DrawSliderInt(0, 100, MNK_AoE_PerfectBalanceHPThreshold,
                        $"Stop Using {PerfectBalance.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.MNK_AoE_ComboHeals:
                    DrawSliderInt(0, 100, MNK_AoE_SecondWindHPThreshold,
                        $"{Role.SecondWind.ActionName()} HP percentage threshold");

                    DrawSliderInt(0, 100, MNK_AoE_BloodbathHPThreshold,
                        $"{Role.Bloodbath.ActionName()} HP percentage threshold");
                    break;

                case Preset.MNK_ST_UseRoE:
                    DrawAdditionalBoolChoice(MNK_ST_EarthsReply,
                        $"Add {EarthsReply.ActionName()}", $"Add {EarthsReply.ActionName()} to the rotation.");

                    if (MNK_ST_EarthsReply)
                    {
                        DrawSliderInt(0, 100, MNK_ST_EarthsReplyHPThreshold,
                            $"Add {EarthsReply.ActionName()} when average HP% of the party is at or below.");
                    }
                    break;

                #endregion

                #region Misc

                case Preset.MNK_Brotherhood_Riddle:
                    DrawRadioButton(MNK_BH_RoF,
                        $"Replaces {Brotherhood.ActionName()}", $"Replaces {Brotherhood.ActionName()} with {RiddleOfFire.ActionName()} when {Brotherhood.ActionName()} is on cooldown.", 0);

                    DrawRadioButton(MNK_BH_RoF,
                        $"Replaces {RiddleOfFire.ActionName()}", $"Replaces {RiddleOfFire.ActionName()} with {Brotherhood.ActionName()}when {RiddleOfFire.ActionName()} is on cooldown.", 1);
                    break;

                case Preset.MNK_Retarget_Thunderclap:
                    DrawAdditionalBoolChoice(MNK_Thunderclap_FieldMouseover,
                        "Add Field Mouseover", "Add Field Mouseover targeting.");
                    break;

                case Preset.MNK_Basic_BeastChakras:
                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        "Opo-opo Option", "Replace Dragon Kick with Bootshine / Leaping Opo.", 3, 0);

                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        "Raptor Option", "Replace Twin Snakes with True Strike/Rising Raptor.", 3, 1);

                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        "Coeurl Option", "Replace Demolish with Snap Punch/Pouncing Coeurl.", 3, 2);
                    break;

                  #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            MNK_SelectedOpener = new("MNK_SelectedOpener"),
            MNK_OpenerCountdown = new("MNK_OpenerCountdown"),
            MNK_Balance_Content = new("MNK_Balance_Content", 1),
            MNK_ST_BHBossOption = new("MNK_ST_BHBossOption"),
            MNK_ST_BHHPThreshold = new("MNK_ST_BHHPThreshold", 25),
            MNK_ST_RoFBossOption = new("MNK_ST_RoFBossOption"),
            MNK_ST_RoFHPThreshold = new("MNK_ST_RoFHPThreshold", 25),
            MNK_ST_RoWBossOption = new("MNK_ST_RoWBossOption"),
            MNK_ST_RoWHPThreshold = new("MNK_ST_RoWHPThreshold", 25),
            MNK_ManualTN = new("MNK_ManualTN"),
            MNK_ST_EarthsReplyHPThreshold = new("MNK_ST_EarthsReplyHPThreshold", 25),
            MNK_ST_SecondWindHPThreshold = new("MNK_ST_SecondWindHPThreshold", 40),
            MNK_ST_BloodbathHPThreshold = new("MNK_ST_BloodbathHPThreshold", 30),

            //AoE
            MNK_AoE_BuffsHPThreshold = new("MNK_AoE_BuffsHPThreshold", 25),
            MNK_AoE_PerfectBalanceHPThreshold = new("MNK_AoE_PerfectBalanceHPThreshold", 25),
            MNK_AoE_SecondWindHPThreshold = new("MNK_AoE_SecondWindThreshold", 40),
            MNK_AoE_BloodbathHPThreshold = new("MNK_AoE_BloodbathThreshold", 30),

            //Misc
            MNK_BH_RoF = new("MNK_BH_RoF");

        public static UserBool
            MNK_Thunderclap_FieldMouseover = new("MNK_Thunderclap_FieldMouseover"),
            MNK_ST_EarthsReply = new("MNK_ST_EarthsReply");

        public static UserBoolArray
            MNK_BasicCombo = new("MNK_BasicCombo");

        #endregion
    }
}
