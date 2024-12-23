using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    internal static PLDOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked) return Opener1;
        return WrathOpener.Dummy;
    }


    internal class PLDOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            HolySpirit,
            FastBlade,
            RiotBlade,
            RoyalAuthority,
            FightOrFlight,
            Imperator,
            Confiteor,
            CircleOfScorn,
            Expiacion,
            BladeOfFaith,
            Intervene,
            BladeOfTruth,
            Intervene,
            BladeOfValor,
            BladeOfHonor,
            GoringBlade,
            Atonement,
            Supplication,
            Sepulchre,
            HolySpirit
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.PLD_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(FightOrFlight)) return false;
            if (!CustomComboFunctions.ActionReady(Imperator)) return false;
            if (!CustomComboFunctions.ActionReady(CircleOfScorn)) return false;
            if (!CustomComboFunctions.ActionReady(Expiacion)) return false;
            if (CustomComboFunctions.GetRemainingCharges(Intervene) < 2) return false;
            if (!CustomComboFunctions.ActionReady(GoringBlade)) return false;

            return true;
        }
    }
}
