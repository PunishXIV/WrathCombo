using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Extensions;

namespace WrathCombo.Data
{
    internal class TimeToKill
    {
        public ulong GameObjectID;

        public uint CurrentHp;

        public List<uint> Diffs = [];

        public long LastTimeChecked;

        public DateTime TimeDead;

        public TimeSpan TimeUntilDead => TimeDead - DateTime.Now;

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

        public static TimeToKill? GetTimeToKillByID(ulong id)
        {
            if (TimeToKills.TryGetFirst(x => x.GameObjectID == id, out var val))
                return val;

            return null;
        }

        public static void UpdateTimeToKills()
        {
            TimeToKills.RemoveAll(x => x.GameObjectID.GetObject() is not { } c || !c.IsInCombat());

            foreach (var ttk in TimeToKills)
            {
                if (Environment.TickCount64 < ttk.LastTimeChecked + 1000)
                    continue;

                var obj = ttk.GameObjectID.GetObject();
                if (obj is IBattleChara c)
                {
                    var current = c.CurrentHp;
                    var last = ttk.CurrentHp;
                    var diff = last - current;

                    ttk.Diffs.Add(diff);
                    var secondsToKill = current / ttk.Diffs.Average(x => x);
                    var ttd = TimeSpan.FromSeconds(secondsToKill);
                    ttk.TimeDead = DateTime.Now + ttd;
                    ttk.CurrentHp = current;


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
