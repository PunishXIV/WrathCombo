using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using System.Linq;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Data.BattleData
{
	internal static partial class BattleData
	{
		private static void LoadARR()
		{
			switch (_territoryID)
			{
				case 174: // Labyrinth of the Ancients
					_invincibleCheck = (target, targetID, _) =>
					{
						// Thanatos, Spooky Ghosts Only
						if (targetID is 2350) return new InvincibleResult(!HasStatusEffect(398), false);
						// Allagan Bomb
						if (targetID is 2407)
							return new InvincibleResult(
								(NumberOfObjectsInRange<SelfCircle>(30,
									target, // 30 yalms radius range of Allagan Bomb
									checkInvincible: false) > 1),
								false);

						return new InvincibleResult(false, false);
					};
					break;

				case 189: // Amdapor Keep (Hard), Ferdidad
					_invincibleCheck = (_, targetID, targetStatuses) =>
					{
						if ((targetID is 3432) && (
							// Stoneskins or multiple adds
								targetStatuses.Contains(152) || NumberOfObjectsInRange<SelfCircle>(30, checkInvincible: false) > 1))
							return new InvincibleResult(true, false);
						return new InvincibleResult(true, false);
					};
					break;

				case 281: //Whorleater (Hard)
					_invincibleCheck = (_, targetID, targetStatuses) =>
					{
						bool inv = ((targetID is 2663 && Player.Job.IsPhysicalRangedDps() && targetStatuses.Contains(478)) ||
							(targetID is 2694 && (Player.Job.IsMagicalRangedDps() || Player.Job.IsHealer()) && targetStatuses.Contains(477)));
						return new InvincibleResult(inv, true);
					};
					break;

				case 292: // Ifrit Hard
					_invincibleCheck = (_, targetID, _) =>
						new InvincibleResult(targetID == 209 && Svc.Objects.Any(x => x.BaseId == 210 && !x.IsDead), true);
					break;

				case 295: // Ifrit Extreme
					_invincibleCheck = (_, targetID, _) =>
						new InvincibleResult(targetID == 211 && Svc.Objects.Any(x => x.BaseId == 212 && !x.IsDead), true);
					break;

				case 359: //Whorleater (Extreme)
					_invincibleCheck = (_, targetID, targetStatuses) =>
					{
						bool inv = (targetID is 2802 && Player.Job.IsPhysicalRangedDps() && targetStatuses.Contains(478) ||
								targetID is 2803 && (Player.Job.IsMagicalRangedDps() || Player.Job.IsHealer()) && targetStatuses.Contains(477));
						return new InvincibleResult(inv, true);
					};
					break;

				case 1045: // Ifrit Normal
					_invincibleCheck = (_, targetID, _) =>
						new InvincibleResult(targetID == 207 && Svc.Objects.Any(x => x.BaseId == 208 && !x.IsDead), true);
					break;

				case 1267: //Sunken Temple of Qarn Temple Guardian
					_invincibleCheck = (_, targetID, targetStatuses) =>
						new InvincibleResult(targetID is 18300 && targetStatuses.Contains(350), true);
					break;
			}
		}
	}
}
