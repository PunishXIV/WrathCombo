using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Extensions;

namespace WrathCombo.Data
{
    public class SimpleTargetState
    {
        public uint CurrentHP;
        public uint MaxHP;
        public ulong GameObjectID;

        public SimpleTargetState(uint currentHP, uint maxHP, ulong gameObjectID)
        {
            CurrentHP = currentHP;
            MaxHP = maxHP;
            GameObjectID = gameObjectID;
        }

        public static List<SimpleTargetState> TargetStates = [];

        public unsafe static void ManageStateList()
        {
            foreach (var o in Svc.Objects.Where(x => x is IBattleChara).Cast<IBattleChara>())
            {
                //if (!o.Struct()->InCombat)
                //{
                //    TargetStates.RemoveAll(x => x.GameObjectID == o.GameObjectId);
                //    continue;
                //}

                TargetStates.RemoveAll(x => o.IsDead && o.GameObjectId == x.GameObjectID);

                if (TargetStates.Any(x => x.GameObjectID == o.GameObjectId) || !o.IsTargetable)
                    continue;

                SimpleTargetState target = new(o.CurrentHp, o.MaxHp, o.GameObjectId);
                TargetStates.Add(target);
            }
            TargetStates.RemoveAll(x => !Svc.Objects.Any(y => y.GameObjectId == x.GameObjectID));
            UpdatePendingHP();
        }

        public static void UpdateNaturalRegenTick(ulong gameObjectId, uint newHealth)
        {
            if (TargetStates.TryGetFirst(x => x.GameObjectID == gameObjectId, out var p))
            {
                p.CurrentHP = newHealth;
            }

        }

        public unsafe static void UpdatePendingHP()
        {
            foreach (var o in TargetStates)
            {
                //if (o.GameObjectID.GetObject() is { } t)
                //{
                //    var b = (BattleChara*)t.Address;
                //    if (!b->InCombat)
                //        continue;
                //}

                var copy = ActionWatching.PendingHPChanges;
                for (int i = 0; i < ActionWatching.PendingHPChanges.Count; i++)
                {
                    var p = ActionWatching.PendingHPChanges[i];
                    if (p.gameObjectId != o.GameObjectID)
                        continue;

                    Svc.Log.Verbose($"Processing GS {p.globalSequence}");
                    o.CurrentHP = (uint)Math.Clamp(o.CurrentHP + (p.positiveChange ? p.value : -p.value), 0, o.MaxHP);
                    p.processed = true;
                    ActionWatching.PendingHPChanges[i] = p;
                }
            }
            ActionWatching.PendingHPChanges.RemoveAll(x => x.processed);
        }

        internal static void UpdateDotDamage(uint entityId, uint diff)
        {
            if (TargetStates.TryGetFirst(x => x.GameObjectID == entityId, out var p))
            {
                if (Svc.Objects.Where(x => x.EntityId == entityId).Cast<IBattleChara>().First() is { } t)
                {
                    p.CurrentHP = Math.Max(p.CurrentHP - diff, 0);
                }
            }
        }

        internal static void RemoveDueToDroppedCombat(uint entityId)
        {
            Svc.Framework.RunOnTick(() =>
            {
                TargetStates.ForEach(x => { if (x.GameObjectID == entityId) x.CurrentHP = x.MaxHP; });
                ActionWatching.PendingHPChanges.RemoveAll(x => x.gameObjectId == entityId);
            }, TimeSpan.FromTicks(100));
        }

        internal static void UpdateHotHeal(uint entityId, uint diff)
        {
            if (TargetStates.TryGetFirst(x => x.GameObjectID == entityId, out var p))
            {
                if (Svc.Objects.Where(x => x.EntityId == entityId).Cast<IBattleChara>().First() is { } t)
                {
                    p.CurrentHP = Math.Min(p.CurrentHP + diff, p.MaxHP);
                }
            }
        }
    }
}
