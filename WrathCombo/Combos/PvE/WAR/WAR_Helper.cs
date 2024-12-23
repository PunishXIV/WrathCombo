using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    internal static WAROpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;
        return WrathOpener.Dummy;
    }

    internal class WAROpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Tomahawk,
           Infuriate,
            HeavySwing,
            Maim,
            StormsEye,
            InnerRelease,
            InnerChaos,
            Upheaval,
            Onslaught,
            PrimalRend,
            Onslaught,
            PrimalRuination,
            Onslaught,
            FellCleave,
            FellCleave,
            FellCleave,
            PrimalWrath,
            Infuriate,
            InnerChaos,
            HeavySwing,
            Maim,
            StormsPath,
            FellCleave,
            Infuriate,
            InnerChaos
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.WAR_BalanceOpener_Content;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(InnerRelease))
                return false;
            if (!CustomComboFunctions.ActionReady(Upheaval))
                return false;
            if (CustomComboFunctions.GetRemainingCharges(Infuriate) < 2)
                return false;
            if (CustomComboFunctions.GetRemainingCharges(Onslaught) < 3)
                return false;

            return true;
        }
    }
}
