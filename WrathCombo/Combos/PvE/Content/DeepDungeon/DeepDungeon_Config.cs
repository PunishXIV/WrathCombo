using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE.Content.DeepDungeons;

internal static partial class DeepDungeons
{
    internal static class Config
    {
        public static UserInt
            PoTD_SustainingPotion_HP = new("PoTD_SustainingPotion_HP", 50),
            HoH_SustainingPotion_HP = new("HoH_SustainingPotion_HP", 50),
            EO_SustainingPotion_HP = new("EO_SustainingPotion_HP", 50),
            PT_SustainingPotion_HP = new("PT_SustainingPotion_HP", 50);

        internal static void Draw(Preset preset)
        {
            switch(preset)
            {
                case Preset.PoTD_SustainingPotion:
                    DrawSliderInt(1, 100, PoTD_SustainingPotion_HP,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.HoH_EmpyreanPotion:
                    DrawSliderInt(1, 100, HoH_SustainingPotion_HP,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.EO_OrthosPotion:
                    DrawSliderInt(1, 100, EO_SustainingPotion_HP,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.PT_PilgrimsPotion:
                    DrawSliderInt(1, 100, PT_SustainingPotion_HP,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
            }
        }
    }

}

