using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.Throttlers;
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

        // todo: averaging over the entire fight is more prone to issues (mechs dropping huge chunks, LBs, deaths/low dmg times, etc):
        //       it's (maybe) more accurate for depicting times further away, but
        //       less accurate for near-term predictions (which we'll mostly use).
        //
        //       An improvement would be to get only the last 45s.
        //       More ideal though would be to get the last 90s, get the last 60s 
        //       if TTK<90, then 45 if TTK<60, then 30 if TTK<45; this would give
        //       improving accuracy as the TTK approaches.
        //       Most Ideal would be getting the last 120s with a moving average
        //       with weight increasing in steps as we approach current damage.
        //
        //       (another improvement would be to do something like normalizing
        //       >30% outlier diffs, to reduce the impact of recent LBs/Mechs)
        public List<(uint Diff, long TickRecorded)> Diffs = [];

        public long LastTimeChecked;

        public DateTime TimeDead;

        private bool FlagForRemoval;

        public float AverageThreshold = 90 * 1000; // To be updated as per above comments

        public TimeSpan TimeUntilDead => TimeDead - DateTime.Now;

        public float SecondsUntilDead => (float)TimeUntilDead.TotalSeconds;

        public uint AverageDPS => Diffs.Count == 0 ? 0 : (uint)Diffs.Where(x => (Environment.TickCount64 - x.TickRecorded) <= AverageThreshold).Average(x => x.Diff);

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

        /// <summary>
        ///     Gets the Estimated Timespan To Kill for a given target.
        /// </summary>
        /// <param name="target">
        ///     The target to get the Estimated Time To Kill for.<br/>
        ///     If null, will use the current target.
        /// </param>
        /// <returns>
        ///     The Estimated Timespan To Kill for the given target.<br/>
        ///     If the target is null or not tracked, returns
        ///     <see cref="TimeSpan.MaxValue"/>.
        /// </returns>
        /// <remarks>
        ///     Recommend using <see cref="EstimatedSecondsToKill"/> instead:
        ///     It gives <see cref="float.NaN"/>, failing comparisons
        ///     (which is more definitive than <see cref="TimeSpan.MaxValue"/>).
        /// </remarks>
        public static TimeSpan EstimatedTimeToKill(IGameObject? target = null)
        {
            target ??= CurrentTarget;
            if (target is null)
                return TimeSpan.MaxValue;

            if (GetTimeToKillByID(target.SafeGameObjectId) is { } ttk)
                return ttk.TimeUntilDead;

            return TimeSpan.MaxValue;
        }

        /// <summary>
        ///     Gets the Estimated Seconds To Kill for a given target.
        /// </summary>
        /// <param name="target">
        ///     The target to get the Estimated Time To Kill for.<br/>
        ///     If null, will use the current target.
        /// </param>
        /// <returns>
        ///     The Estimated Seconds To Kill for the given target.<br/>
        ///     If the target is null or not tracked, returns
        ///     <see cref="float.NaN"/>.
        /// </returns>
        public static float EstimatedSecondsToKill(IGameObject? target = null)
        {
            target ??= CurrentTarget;
            if (target is null)
                return float.NaN;

            if (GetTimeToKillByID(target.SafeGameObjectId) is { } ttk)
                return ttk.SecondsUntilDead;

            return float.NaN;
        }

        public static void UpdateTimeToKills()
        {
            TimeToKills.RemoveAll(x => x.FlagForRemoval || x.GameObjectID.GetObject() is not { } c || !c.IsInCombat());

            foreach (var ttk in TimeToKills)
            {
                if (Environment.TickCount64 < (ttk.LastTimeChecked + 1000))
                    continue;

                var obj = ttk.GameObjectID.GetObject();
                // Probably pretty rare, but may as well flag it since we need to check it anyway
                if (obj is not IBattleChara c)
                {
                    ttk.FlagForRemoval = true;
                    continue;
                }

                var current = c.CurrentHp;
                var last = ttk.CurrentHp;

                if (current > last ||                                                         //Heal, dummy resets etc.
                    (ttk.Diffs.Count >= 15 && ttk.Diffs.TakeLast(15).All(x => x.Diff == 0)))  //or if the last 15 seconds have had no change, assume cutscene or something
                {
                    ttk.FlagForRemoval = true;
                    continue;
                }

                var diff = last - current;

                ttk.Diffs.Add((diff, Environment.TickCount64));
                if (ttk.AverageDPS > 0)
                {
                    var secondsToKill = current / ttk.AverageDPS;
                    var ttd = TimeSpan.FromSeconds(secondsToKill);
                    ttk.TimeDead = DateTime.Now + ttd;
                    ttk.CurrentHp = current;

                    ttk.AverageThreshold = ttk.TimeUntilDead.TotalSeconds switch
                    {
                        <= 15 => 30 * 1000,
                        <= 30 => 45 * 1000,
                        <= 45 => 60 * 1000,
                        <= 60 => 75 * 1000,
                        _ => 90 * 1000,

                    };
                }

                ttk.LastTimeChecked = Environment.TickCount64;
            }
        }

        public static void AddEnemiesToTimeToKill()
        {
            foreach (var e in Svc.Objects.Where(x => x.IsHostile() && !x.IsDead && x.IsInCombat() && x.IsTargetable))
            {
                var id = e.SafeGameObjectId;
                if (id == null)
                    continue;

                if (TimeToKills.Any(x => x.GameObjectID == id))
                    continue;

                _ = new TimeToKill(id.Value); // (adds itself in the list)
            }
        }

        public static void Update(bool force = false, bool clear = false)
        {
            if (!force && !EzThrottler.Throttle("ttkUpdate", 105))
                return;

            if (clear)
            {
                TimeToKills.Clear();
                return;
            }

            AddEnemiesToTimeToKill();
            UpdateTimeToKills();
        }
    }
}
