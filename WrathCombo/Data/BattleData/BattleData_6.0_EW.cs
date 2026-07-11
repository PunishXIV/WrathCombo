using ECommons.DalamudServices;
using System.Linq;

namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        private static void LoadEW()
        {
            switch (_territoryID)
            {
                case 952: // Tower of Zot final bosses
                          // Technically not invincible, just need to ignore
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        if (targetID is (13298 or 13299) && Svc.Objects.Any(y => y.BaseId is 13297 && !y.IsDead))
                            return Invincible.True;
                        return Invincible.False;
                    };
                    break;
            }
        }
    }
}
