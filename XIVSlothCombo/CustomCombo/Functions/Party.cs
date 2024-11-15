﻿using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
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
        public static List<IBattleChara?> GetPartyMembers()
        {
            List<IBattleChara?> output = new List<IBattleChara?>();
            for (int i = 1; i <= 8; i++)
            {
                IGameObject? member = GetPartySlot(i);
                if (member != null)
                    output.Add(member as IBattleChara);
            }
            return output;
        }

        public static unsafe IGameObject? GetPartySlot(int slot)
        {
            try
            {
                GameObject* o = slot switch
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

        public static float GetPartyAvgHPPercent()
        {
            float HP = 0;
            byte Count = 0;
            for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
            {
                if (GetPartySlot(i) is not IBattleChara member) continue;
                if (member is null) continue; //Skip nulls/disconnected people
                if (member.IsDead) continue;

                HP += GetTargetHPPercent(member);
                Count++;
            }
            return Count == 0 ? 0 : (float)HP / Count; //Div by 0 check...just in case....
        }

        public static float GetPartyBuffPercent(ushort buff)
        {
            byte BuffCount = 0;
            byte PartyCount = 0;
            for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
            {
                if (GetPartySlot(i) is not IBattleChara member) continue;
                if (member is null) continue; //Skip nulls/disconnected people
                if (member.IsDead) continue;
                if (FindEffectOnMember(buff, member) is not null) BuffCount++;
                PartyCount++;
            }
            return PartyCount == 0 ? 0 : (float)BuffCount / PartyCount * 100f; //Div by 0 check...just in case....
        }
    }
}
