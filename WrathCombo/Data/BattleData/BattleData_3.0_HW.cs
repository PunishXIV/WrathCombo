using ECommons.GameHelpers;
using ECommons.ExcelServices;
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
                        bool inv1 = (targetID is 5105 or 5103) &&
                            ((Player.Job.IsPhysicalRangedDps() && targetStatuses.Contains(941)) ||
                            (Player.Job.IsMagicalRangedDps() && targetStatuses.Contains(942)));

                        // Cuchulainn 5139, Checking one of the Stoneskins
                        bool inv2= targetID is 5139 && targetStatuses.Contains(152);

                        return new(inv1 || inv2, false);
                    };
                    break;

                case 582: // Heart of the Creator
                    _invincibleCheck = (target, targetID, _) =>
                        new(
                            (targetID is 6101) && // Plasma Shield
                            AngleToTarget(target) is not AttackAngle.Front,
                            false);
                    break;
            }
        }
    }
}
