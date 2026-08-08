using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using System.Linq;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Data.BattleData
{
    internal partial class BattleData
    {
        private static bool LoadHW()
        {
            bool dataLoaded = true;
            switch (_territoryID)
            {
                case 508: // The Void Ark
                    _invincibleCheck = (_, targetID, targetStatuses) =>
                    {
                        // Sawtooth 5103
                        // Irminsul 5105
                        if ((targetID is 5105 or 5103) &&
                            ((Player.Job.IsPhysicalRangedDps() && targetStatuses.Contains(941)) ||
                             (Player.Job.IsMagicalRangedDps() && targetStatuses.Contains(942))
                            )
                           ) return Invincible.True;
                        // Cuchulainn 5139, Checking one of the Stoneskins
                        if (targetID is 5139 && targetStatuses.Contains(152)) return Invincible.True;
                        return Invincible.False;
                    };
                    break;

                case 582: // Heart of the Creator
                    _invincibleCheck = (target, targetID, _) =>
                    {
                        if ((targetID is 6101) && // Plasma Shield
                            AngleToTarget(target) is not AttackAngle.Front) return Invincible.True;
                        return Invincible.False;
                    };
                    break;

                case 637: // Containment Bay Z1T9 Zurvan
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        // Note, would need a Player.CombatRole not Tank if this is built out further for Extreme/Unreal, presuming NPC base IDs are the same for Ex
                        if (targetID is 6556 or 6553) // Ignore Execrated Thew (6556) or Wills (6553) (mostly Wills) when Execrated Witts (6554) are around
                            return Result(Svc.Objects.GetBattleCharas().Any(x => x.BaseId is 6554 && !x.IsDead && x.IsCharacterVisible()));
                        return Invincible.False;
                    };
                    break;

                case 1114: // Baelsar's Wall
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        if (targetID is 6461) // The Griffin. Ignore when Restraint Collar shows (hiddden NPC helper)
                            return Result(Svc.Objects.GetBattleCharas().Any(x => x.BaseId is 6462 && !x.IsDead && x.IsCharacterVisible()));
                        return Invincible.False;
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
