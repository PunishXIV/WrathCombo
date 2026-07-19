using System.Linq;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        private static bool LoadSB()
        {
            bool dataLoaded = true;
            switch (_territoryID)
            {
                case 674: // Pool of Tribute
                    _invincibleCheck = (target, targetID, targetStatuses) =>
                    {
                        var fettered = GetPartyMembers() // Svc.Objects if debugging via ARR
                            .FirstOrDefault(x => HasStatusEffect(292, x.BattleChara, true));

                        if (targetID == 7200 && fettered is not null)
                        {
                            var distance = GetTargetDistance(target, fettered.BattleChara);
                            return Result(distance >= 1); // NPC Rock to kill is right on top, less than 1
                        }
                        return Invincible.False;
                    };

                    break;

                case 801 or 805 or 1122: // Interdimensional Rift (Omega 12 / Alphascape 4), Regular/Savage?/Ultimate?
                                         // Omega-M = 9339
                                         // Omega-F = 9340
                    _invincibleCheck = (_, targetID, targetStatuses) =>
                    {
                        if (targetID is 9339 or 9340) //numbers are for Regular
                        {
                            if (HasStatusEffect(1660)) return Result(targetID == 9339); // Packet Filter M
                            if (HasStatusEffect(1661)) return Result(targetID == 9340); // Packet Filter F
                            if (targetID is 9340 && targetStatuses.Contains(671)) return Invincible.True; // F being covered by M
                        }

                        //Savage/Ultimate? Not sure which omega fight uses 3499 and 3500
                        if ((targetStatuses.Contains(3454) is true && HasStatusEffect(3499)) ||
                            (targetStatuses.Contains(1675) is true && HasStatusEffect(3500)))
                            return Invincible.True;

                        return Invincible.CheckStatuses;
                    };
                    break;

                case 1174:
                    // The Ghimlyt Dark
                    // Colossus Rubricatus = 9511
                    // No point attacking anymore when it begins to cast self-detonate = 14574
                    _invincibleCheck = (target, targetID, _) =>
                    {
                        if (targetID is 9511 && target.IsCasting && target.CastActionId == 14574) return Invincible.True;
                        return Invincible.True;
                    };
                    break;
                default:
                    dataLoaded = false;
                    break;
            }
            return dataLoaded;
        }
    }
}
