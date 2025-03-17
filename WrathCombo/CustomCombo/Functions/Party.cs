using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Checks if the player is in a party. Optionally, refine by minimum party size. </summary>
        /// <param name="partySize"> The minimum amount of party members required. </param>
        public static bool IsInParty(int partySize = 2) => GetPartyMembers().Count >= partySize;

        /// <summary> Gets the party list </summary>
        /// <returns> Current party list. </returns>
        public unsafe static List<WrathPartyMember> GetPartyMembers()
        {
            if (!Player.Available) return new();
            if (!EzThrottler.Throttle("PartyUpdateThrottle", 2000))
                return _partyList;

            for (int i = 1; i <= 8; i++)
            {
                var member = GetPartySlot(i);
                if (member != null)
                {
                    var chara = (member as IBattleChara);
                    WrathPartyMember wmember = new()
                    {
                        GameObjectId = chara.GameObjectId,
                        BattleChara = chara,
                        CurrentHP = chara.CurrentHp
                    };

                    if (!_partyList.Any(x => x.BattleChara.GameObjectId == chara.GameObjectId))
                        _partyList.Add(wmember);

                }
            }

            if (AutoRotationController.cfg is not null)
            {
                if (AutoRotationController.cfg.Enabled && AutoRotationController.cfg.HealerSettings.IncludeNPCs && Player.Job.IsHealer())
                {
                    foreach (var npc in Svc.Objects.Where(x => x is IBattleChara && x is not IPlayerCharacter).Cast<IBattleChara>())
                    {
                        if (ActionManager.CanUseActionOnTarget(All.Esuna, npc.GameObject()) && !_partyList.Any(x => x.BattleChara == npc))
                        {
                            WrathPartyMember wmember = new()
                            {
                                GameObjectId = npc.GameObjectId,
                                BattleChara = npc,
                                CurrentHP = npc.CurrentHp
                            };

                            if (!_partyList.Any(x => x.BattleChara.GameObjectId == npc.GameObjectId))
                                _partyList.Add(wmember);
                        }
                    }
                }
            }

            _partyList.RemoveAll(x => !Svc.Objects.Any(y => y.GameObjectId == x.GameObjectId));
            return _partyList;
        }

        private static List<WrathPartyMember> _partyList = new();

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
                return Svc.Objects.FirstOrDefault(x => x.GameObjectId == o->GetGameObjectId());
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

        public static bool PartyInCombat() => PartyEngageDuration().Ticks > 0;
    }

    public enum AllianceGroup
    {
        GroupA,
        GroupB,
        GroupC,
        NotInAlliance
    }

    public class WrathPartyMember
    {
        public bool HPUpdatePending = false;
        public bool MPUpdatePending = false;
        public ulong GameObjectId;
        public IBattleChara BattleChara = null!;
        public uint CurrentHP
        {
            get
            {
                if ((field > BattleChara.CurrentHp && !HPUpdatePending) || field < BattleChara.CurrentHp)
                    field = BattleChara.CurrentHp;

                return field;
            }

            set;
        }

        public uint CurrentMP
        {
            get
            {
                if ((field > BattleChara.CurrentMp && !MPUpdatePending) || field < BattleChara.CurrentMp)
                    field = BattleChara.CurrentMp;

                return field;
            }
            set;
        }
    }
}
