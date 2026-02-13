using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Data
{
    internal class TimeToKill
    {
        public ulong GameObjectID;

        public uint CurrentHp;

        public List<uint> Diffs = [];

        public long LastTimeChecked;

        public DateTime TimeDead;

        private bool FlagForRemoval;

        public TimeSpan TimeUntilDead => TimeDead - DateTime.Now;
        
        public float SecondsUntilDead => (float)TimeUntilDead.TotalSeconds;

        public uint AverageDPS => Diffs.Count == 0 ? 0 : (uint)Diffs.Average(x => x);

        public TimeToKill(ulong gameObjectId)
        {
            GameObjectID = gameObjectId;
            var obj = Svc.Objects.FirstOrDefault(x => x.GameObjectId == GameObjectID);
            if (obj is IBattleChara c)
            {
                CurrentHp = c.CurrentHp;
                LastTimeChecked = Environment.TickCount64;

                TimeToKills.Add(this);
            }
        }

        public static List<TimeToKill> TimeToKills = [];

        public static TimeToKill? GetTimeToKillByID(ulong? id)
        {
            if (TimeToKills.TryGetFirst(x => x.GameObjectID == id, out var val))
                return val;

            return null;
        }

        /// <remarks>
        ///     Returns <see cref="TimeSpan.MaxValue"/> if no target or no TTK
        ///     can be determined.
        /// </remarks>
        public static TimeSpan EstimatedTimeToKill(IGameObject? target = null)
        {
            target ??= CurrentTarget;
            if (target is null)
                return TimeSpan.MaxValue;

            if (GetTimeToKillByID(target?.SafeGameObjectId) is { } ttk)
                return ttk.TimeUntilDead;

            return TimeSpan.MaxValue;
        }

        public static void UpdateTimeToKills()
        {
            TimeToKills.RemoveAll(x => x.FlagForRemoval || x.GameObjectID.GetObject() is not { } c || !c.IsInCombat());

            foreach (var ttk in TimeToKills)
            {
                if (Environment.TickCount64 < (ttk.LastTimeChecked + 1000))
                    continue;

                var obj = ttk.GameObjectID.GetObject();
                if (obj is IBattleChara c)
                {
                    var current = c.CurrentHp;
                    var last = ttk.CurrentHp;

                    if (current > last) //Heal, dummy resets etc.
                    {
                        ttk.FlagForRemoval = true;
                        continue;
                    }

                    var diff = last - current;

                    ttk.Diffs.Add(diff);
                    if (ttk.AverageDPS > 0)
                    {
                        var secondsToKill = current / ttk.AverageDPS;
                        var ttd = TimeSpan.FromSeconds(secondsToKill);
                        ttk.TimeDead = DateTime.Now + ttd;
                        ttk.CurrentHp = current;
                    }

                    ttk.LastTimeChecked = Environment.TickCount64;
                }
            }
        }

        public static void AddEnemiesToTimeToKill()
        {
            foreach (var e in Svc.Objects.Where(x => x.IsHostile() && !x.IsDead && x.IsInCombat()))
            {
                var id = e.SafeGameObjectId;
                if (id == null)
                    continue;

                if (TimeToKills.Any(x => x.GameObjectID == id))
                    continue;

                new TimeToKill(id.Value);
            }
        }

        public static void Run()
        {
            AddEnemiesToTimeToKill();
            UpdateTimeToKills();
        }
    }
}
