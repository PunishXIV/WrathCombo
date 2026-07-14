using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
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
                if (o.GameObjectID.GetObject() is IBattleChara t)
                {
                    int hpDelta = ActionWatching.PendingHPChanges
                                .Where(x => x.gameObjectId == o.GameObjectID)
                                .Sum(x => x.positiveChange ? x.value : -x.value);

                    long currentHp = t.CurrentHp;
                    o.CurrentHP = (uint)Math.Clamp(currentHp + hpDelta, 0L, (long)t.MaxHp);
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

        internal static void UpdatePeriodicHealthChange(uint entityId, uint diff, bool damage)
        {
            if (TargetStates.TryGetFirst(x => x.GameObjectID == entityId, out var p))
            {
                if (Svc.Objects.Where(x => x.EntityId == entityId).Cast<IBattleChara>().First() is { } t)
                {
                    var val = damage ? -diff : diff;
                    if (p.CurrentHP + val < 0)
                        p.CurrentHP = 0;
                    else
                        p.CurrentHP = (uint)Math.Clamp(p.CurrentHP + val, 0, p.MaxHP);
                }
            }
        }
    }
}
