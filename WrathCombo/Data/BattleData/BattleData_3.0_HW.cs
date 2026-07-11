using ECommons.ExcelServices;
using ECommons.GameHelpers;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Data.BattleData
{
    internal partial class BattleData
    {
        private static void LoadHW()
        {
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
            }
        }
    }
}
