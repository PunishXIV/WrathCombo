using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class BLU
{
    internal static class Config
    {
        public static UserInt
            BLU_DoTHP = new("BLU_DoTHP", 2),
            BLU_DoTTime = new("BLU_DoTTime", 3),
            BLU_Balance_Content = new("BLU_Balance_Content", 1),
            BLU_SelectedOpener = new("BLU_SelectedOpener", 0);
        public static UserBool
            BLU_Opener_PrepullBlock = new("BLU_Opener_PrepullBlock", true),
            BLU_ManualJKick = new("BLU_ManualJKick", false);
        public static UserBoolArray
            BLU_PrimalCombo_Spells = new("BLU_PrimalCombo_Spells", [true, true, true, true, true]);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.BLU_ST_DPS_Opener:
                    DrawBossOnlyChoice(BLU_Balance_Content);
                    ImGuiEx.TextUnderlined("Select Opener");
                    ImGui.Spacing();
                    DrawRadioButton(BLU_SelectedOpener,
                        "Winged Opener",
                        "Winged Reprobation opener. Standard 2.50 spell speed.", 0, descriptionAsTooltip: true);
                    DrawRadioButton(BLU_SelectedOpener,
                        "DoT Opener",
                        "Mortal Flame or Breath of Magic instead of Winged Reprobation. Requires 2.20 or faster spell speed.",
                        1, descriptionAsTooltip: true);

                    DrawOpenerPrepullBlockChoice(BLU_Opener_PrepullBlock);
                    DrawAdditionalBoolChoice(BLU_ManualJKick,
                        "Input J Kick yourself",
                        "Skips J Kick in the opener and 1-button primals so you can gap-close with it. Reopeners will not wait for J Kick.");
                    break;

                case Preset.BLU_NewMoonFluteOpener:
                    DrawAdditionalBoolChoice(BLU_ManualJKick,
                        "Input J Kick yourself",
                        "Skips J Kick in this opener so you can gap-close with it. Reopeners will not wait for J Kick.");
                    break;

                case Preset.BLU_PrimalCombo:
                    ImGuiEx.TextUnderlined("Primals to use on this button");
                    DrawHorizontalMultiChoice(BLU_PrimalCombo_Spells, FeatherRain.ActionName(), "Use Feather Rain.", 5, 0);
                    DrawHorizontalMultiChoice(BLU_PrimalCombo_Spells, Eruption.ActionName(), "Use Eruption.", 5, 1);
                    DrawHorizontalMultiChoice(BLU_PrimalCombo_Spells, ShockStrike.ActionName(), "Use Shock Strike.", 5, 2);
                    DrawHorizontalMultiChoice(BLU_PrimalCombo_Spells, RoseOfDestruction.ActionName(), "Use Rose of Destruction.", 5, 3);
                    DrawHorizontalMultiChoice(BLU_PrimalCombo_Spells, GlassDance.ActionName(), "Use Glass Dance.", 5, 4);
                    break;

                case Preset.BLU_ST_DPS_SongOfTorment:
                case Preset.BLU_ST_DPS_Breath:
                case Preset.BLU_ST_DPS_Flame:
                case Preset.BLU_ST_Tank_SongOfTorment:
                    DrawSliderInt(0, 100, BLU_DoTHP, Generics.StopEnemyHpPercent);
                    DrawSliderInt(0, 15, BLU_DoTTime, Generics.StopSeconds);
                    break;
            }
        }
    }
}
