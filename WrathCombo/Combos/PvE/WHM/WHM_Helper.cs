#region

using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM
{
    internal static WHMGauge gauge = GetJobGauge<WHMGauge>();
    internal static byte BloodLilies = GetJobGauge<WHMGauge>().BloodLily;
    internal static WHMOpenerMaxLevel1 Opener1 = new();

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }


    internal class WHMOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Glare3,
            Dia,
            Glare3,
            Glare3,
            PresenceOfMind,
            Glare4,
            Assize,
            Glare4,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare4,
            Dia
        ];
        internal override UserData? ContentCheckConfig => Config.WHM_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!ActionReady(PresenceOfMind))
                return false;

            if (!ActionReady(Assize))
                return false;

            return true;
        }
    }

    internal static class WHMHelper
    {
        public static int GetMatchingConfigST(int i, IGameObject? optionalTarget, out uint action,
            out bool enabled)
        {
            //var healTarget = optionalTarget ?? GetHealTarget(Config.WHM_STHeals_UIMouseOver);
            //leaving incase Regen gets a slider and is added

            bool canWeave = CanWeave(0.3);

            switch (i)
            {
                case 0:
                    action = Benediction;

                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Benediction) &&
                              (!Config.WHM_STHeals_BenedictionWeave ||
                               (Config.WHM_STHeals_BenedictionWeave && canWeave));

                    return Config.WHM_STHeals_BenedictionHP;

                case 1:
                    action = Tetragrammaton;

                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Tetragrammaton) &&
                              (!Config.WHM_STHeals_TetraWeave || (Config.WHM_STHeals_TetraWeave && canWeave));

                    return Config.WHM_STHeals_TetraHP;

                case 2:
                    action = DivineBenison;

                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Benison) &&
                              (!Config.WHM_STHeals_BenisonWeave ||
                               (Config.WHM_STHeals_BenisonWeave && canWeave));

                    return Config.WHM_STHeals_BenisonHP;

                case 3:
                    action = Aquaveil;

                    enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Aquaveil) &&
                              (!Config.WHM_STHeals_AquaveilWeave ||
                               (Config.WHM_STHeals_AquaveilWeave && canWeave));

                    return Config.WHM_STHeals_AquaveilHP;
            }

            enabled = false;
            action = 0;

            return 0;
        }
    }
}