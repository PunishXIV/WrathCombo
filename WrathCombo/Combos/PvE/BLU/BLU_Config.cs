using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
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
