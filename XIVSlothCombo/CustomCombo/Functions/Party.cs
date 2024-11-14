using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using System.Linq;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Checks if the player is in a party. Optionally, refine by minimum party size. </summary>
        /// <param name="partySize"> The minimum amount of party members required. </param>
        public static bool IsInParty(int? partySize = null)
        {
            if (Svc.Party.PartyId > 0) return partySize == null || Svc.Party.Length >= partySize;

            else return false;
        }

        /// <summary> Gets the party list </summary>
        /// <returns> Current party list. </returns>
        public static IPartyList GetPartyMembers() => Svc.Party;

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
