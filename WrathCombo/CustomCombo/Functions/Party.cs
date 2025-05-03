using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
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
        public static unsafe List<WrathPartyMember> GetPartyMembers()
        {
            if (!Player.Available) return [];
            if (!EzThrottler.Throttle("PartyUpdateThrottle", 2000))
                return _partyList;

            var existingIds = _partyList.Select(x => x.GameObjectId).ToHashSet();

            for (int i = 1; i <= 8; i++)
            {
                var member = GetPartySlot(i);
                if (member is IBattleChara chara && !existingIds.Contains(chara.GameObjectId))
                {
                    WrathPartyMember wmember = new()
                    {
                        GameObjectId = chara.GameObjectId,
                        CurrentHP = chara.CurrentHp
                    };
                    _partyList.Add(wmember);
                    existingIds.Add(chara.GameObjectId);
                }
            }

            if (AutoRotationController.cfg?.Enabled == true && AutoRotationController.cfg.HealerSettings.IncludeNPCs && Player.Job.IsHealer())
            {
                foreach (var npc in Svc.Objects.OfType<IBattleChara>().Where(x => x is not IPlayerCharacter && !existingIds.Contains(x.GameObjectId)))
                {
                    if (ActionManager.CanUseActionOnTarget(RoleActions.Healer.Esuna, npc.GameObject()))
                    {
                        WrathPartyMember wmember = new()
                        {
                            GameObjectId = npc.GameObjectId,
                            CurrentHP = npc.CurrentHp
                        };
                        _partyList.Add(wmember);
                        existingIds.Add(npc.GameObjectId);
                    }
                }
            }

            _partyList.RemoveAll(x => x.BattleChara is null);
            return _partyList;
        }

        private static List<WrathPartyMember> _partyList = new();

        public static unsafe IGameObject? GetPartySlot(int slot)
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
                    _ => null,
                };

                return o != null ? Svc.Objects.FirstOrDefault(x => x.GameObjectId == o->GetGameObjectId()) : null;
            }
            catch (Exception ex)
            {
                ex.Log();
                return null;
            }
        }

        public static float GetPartyAvgHPPercent()
        {
            float totalHP = 0;
            int count = 0;

            for (int i = 1; i <= 8; i++)
            {
                if (GetPartySlot(i) is IBattleChara member && !member.IsDead)
                {
                    totalHP += GetTargetHPPercent(member);
                    count++;
                }
            }

            return count == 0 ? 0 : totalHP / count;
        }

        public static float GetPartyBuffPercent(ushort buff)
        {
            int buffCount = 0;
            int partyCount = 0;

            for (int i = 1; i <= 8; i++)
            {
                if (GetPartySlot(i) is IBattleChara member && !member.IsDead)
                {
                    if (HasStatusEffect(buff, member, true)) buffCount++;
                    partyCount++;
                }
            }

            return partyCount == 0 ? 0 : (float)buffCount / partyCount * 100f;
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
        public IBattleChara? BattleChara => Svc.Objects.FirstOrDefault(x => x.GameObjectId == GameObjectId) as IBattleChara;
        public Dictionary<ushort, long> BuffsGainedAt = new();

        private uint _currentHP;
        public uint CurrentHP
        {
            get
            {
                if (BattleChara != null)
                {
                    if ((_currentHP > BattleChara.CurrentHp && !HPUpdatePending) || _currentHP < BattleChara.CurrentHp)
                        _currentHP = BattleChara.CurrentHp;
                }
                return _currentHP;
            }
            set => _currentHP = value;
        }

        private uint _currentMP;
        public uint CurrentMP
        {
            get
            {
                if (BattleChara != null)
                {
                    if ((_currentMP > BattleChara.CurrentMp && !MPUpdatePending) || _currentMP < BattleChara.CurrentMp)
                        _currentMP = BattleChara.CurrentMp;
                }
                return _currentMP;
            }
            set => _currentMP = value;
        }

        public float TimeSinceBuffApplied(ushort buff)
        {
            return BuffsGainedAt.TryGetValue(buff, out var timestamp) ? (Environment.TickCount64 - timestamp) / 1000f : 0;
        }
    }
}
