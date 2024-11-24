using Dalamud.Game.ClientState.Objects.Types;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal static partial class WHM
    {
        public static int GetMatchingConfigST(int i, IGameObject? optionalTarget, out uint action, out bool enabled)
        {
            //var healTarget = optionalTarget ?? GetHealTarget(Config.WHM_STHeals_UIMouseOver);
            //leaving incase Regen gets a slider and is added

            bool canWeave = CanWeave(OriginalHook(Stone1), 0.3);
            switch (i)
            {
                case 0:
                    action = Benediction;
                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Benediction) && (!Config.WHM_STHeals_BenedictionWeave || (Config.WHM_STHeals_BenedictionWeave && canWeave));
                    return Config.WHM_STHeals_BenedictionHP;
                case 1:
                    action = Tetragrammaton;
                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Tetragrammaton) && (!Config.WHM_STHeals_TetraWeave || (Config.WHM_STHeals_TetraWeave && canWeave));
                    return Config.WHM_STHeals_TetraHP;
                case 2:
                    action = DivineBenison;
                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Benison) && (!Config.WHM_STHeals_BenisonWeave || (Config.WHM_STHeals_BenisonWeave && canWeave));
                    return Config.WHM_STHeals_BenisonHP;
                case 3:
                    action = Aquaveil;
                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Aquaveil) && (!Config.WHM_STHeals_AquaveilWeave || (Config.WHM_STHeals_AquaveilWeave && canWeave));
                    return Config.WHM_STHeals_AquaveilHP;
            }

            enabled = false;
            action = 0;
            return 0;
        }
    }
}
