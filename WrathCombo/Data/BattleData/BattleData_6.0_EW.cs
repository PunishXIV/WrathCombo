using ECommons.DalamudServices;
using System.Collections.Frozen;
using System.Linq;

namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        private static bool LoadEW()
        {
            bool dataLoaded = true;
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
                case 1070: //The Fell Court of Troia - Beatrice

                    _ignoreRaidwideAIDs = new uint[]
                    {
                        29821, 29828 //Gaze rings, triggers group damage warnings
                    }.ToFrozenSet();

                    _pauseActions = () =>
                    {
                        return CheckForCastTimeRemaining(29821) || CheckForCastTimeRemaining(29828);
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
