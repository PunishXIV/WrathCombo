using ECommons.ExcelServices;
using ECommons.GameHelpers;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        private static void LoadShB()
        {
            switch (_territoryID)
            {
                case 821: //Dohn Mheg Final Boss Lyre
                    //Unfooled means you can attack the Lyre
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        if (targetID is 3939 && !HasStatusEffect(386))
                            return InvincibleResult.True; //Unfooled means you can attack the Lyre
                        return InvincibleResult.False;
                    };
                    break;

                case 887: // The Epic of Alexander (Ultimate)
                          // Jagd Doll = NameId 3759
                          // Technically not invincible, but killing one wipes the raid;
                          // ignore them once below the 25% HP feed threshold
                    _invincibleCheck = (target, targetID, _) =>
                    {
                        if (targetID is 11338 && GetTargetHPPercent(target) < 25)
                            return InvincibleResult.True;
                        return InvincibleResult.CheckGeneric;
                    };
                    break;

                case 917: //Puppet's Bunker, Flight Mechs
                          // 724P Alpha = 11792 (A)
                          // 767P Beta  = 11793 (B)
                          // 772P Chi   = 11794 (C)
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        if (targetID is 11792 or 11793 or 11794)
                        {
                            if (HasStatusEffect(2288)) return Result(targetID != 11792);
                            if (HasStatusEffect(2289)) return Result(targetID != 11793);
                            if (HasStatusEffect(2290)) return Result(targetID != 11794);
                        }
                        return InvincibleResult.False;
                    };
                    break;

                case 966: //The Tower at Paradigm's Breach, Hansel & Gretel
                          // Hansel = 12709
                          // Gretel = 12708
                          // If boss has shield or too close, boss gains 680 Parry, so easier to check for that
                          // 680 Directional Parry
                          // 2538 Strong of Shield
                          // 2539 Stronger Together
                    _invincibleCheck = (target, targetID, targetStatuses) =>
                    {
                        if (targetID is 12709 or 12708)
                        {
                            bool isTank = Player.Job.IsTank();
                            bool bossHasParry = HasStatusEffect(680, target);
                            bool isFrontFacing = AngleToTarget(target) is AttackAngle.Front;

                            // Non Tanks should just ignore parrying boss(s)
                            // Tanks should only ignore their target if it has the buff and they aren't in front.
                            if (bossHasParry && (!isTank || !isFrontFacing)) return InvincibleResult.True;
                        }
                        return InvincibleResult.False;
                    };
                    break;

            }
        }
    }
}
