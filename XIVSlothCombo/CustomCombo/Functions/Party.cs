using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Linq;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Checks if player is in a party </summary>
        public static bool IsInParty() => (Svc.Party.PartyId > 0);

        /// <summary> Gets the party list </summary>
        /// <returns> Current party list. </returns>
        public static List<IBattleChara> GetPartyMembers()
        {
            var output = new List<IBattleChara>();
            for (int i = 1; i <= 8; i++)
            {
                var member = GetPartySlot(i);
                if (member != null)
                    output.Add(member as IBattleChara);
            }
            return output;
        }

        public unsafe static IGameObject? GetPartySlot(int slot)
        {
            try
            {
                var o = slot switch
                {
                    1 => GetTarget(TargetType.Self),
                    2 => GetTarget(TargetType.P2),
                    3 => GetTarget(TargetType.P3),
                    4 => GetTarget(TargetType.P4),
                    5 => GetTarget(TargetType.P5),
                    6 => GetTarget(TargetType.P6),
                    7 => GetTarget(TargetType.P7),
                    8 => GetTarget(TargetType.P8),
                    _ => GetTarget(TargetType.Self),
                };
                ulong i = PartyTargetingService.GetObjectID(o);
                return Svc.Objects.Where(x => x.GameObjectId == i).Any()
                    ? Svc.Objects.Where(x => x.GameObjectId == i).First()
                    : null;
            }

            catch
            {
                return null;
            }
        }
    }
}
