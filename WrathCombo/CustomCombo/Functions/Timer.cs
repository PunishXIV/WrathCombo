using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        private static DateTime combatStart = DateTime.Now;
        private static DateTime partyCombat = DateTime.Now;
        private static bool partyInCombat = false;

        public static Dictionary<ulong, long> Deadtionary { get; set; } = new();

        /// <summary> Tells the elapsed time since the combat started. </summary>
        /// <returns> Combat time in seconds. </returns>
        public static TimeSpan CombatEngageDuration() => InCombat() ? DateTime.Now - combatStart : TimeSpan.Zero;

        public unsafe static TimeSpan PartyEngageDuration() => partyInCombat ? DateTime.Now - partyCombat : TimeSpan.Zero;

        public unsafe static TimeSpan TimeSpentDead(ulong partyMemberObjectId) => TimeSpentDead((uint)partyMemberObjectId);

        public unsafe static TimeSpan TimeSpentDead(uint partyMemberObjectId) => Deadtionary.ContainsKey(partyMemberObjectId) ? TimeSpan.FromMilliseconds((Environment.TickCount64 - Deadtionary[partyMemberObjectId])) : TimeSpan.Zero;

        public static void TimerSetup()
        {
            Svc.Condition.ConditionChange += OnCombat;
            Svc.Framework.Update += UpdatePartyTimer;
            Svc.Framework.Update += UpdateDeadtionary;
        }

        private static void UpdateDeadtionary(IFramework framework)
        {
            foreach (var member in GetPartyMembers().Where(x => x.IsDead))
            {
                if (!Deadtionary.ContainsKey(member.GameObjectId))
                    Deadtionary[member.GameObjectId] = Environment.TickCount64;
            }

            var deadCopy = Deadtionary.ToList();
            foreach (var member in deadCopy)
            {
                if (!Svc.Objects.Any(x => x.GameObjectId == member.Key) || !Svc.Objects.First(x => x.GameObjectId == member.Key).IsDead)
                    Deadtionary.Remove(member.Key);
            }
        }

        private unsafe static void UpdatePartyTimer(IFramework framework)
        {
            if (GetPartyMembers().Any(x => x.Struct()->InCombat) && !partyInCombat)
            {
                partyInCombat = true;
                partyCombat = DateTime.Now;
            }
            else if (!GetPartyMembers().Any(x => x.Struct()->InCombat))
            {
                partyInCombat = false;
            }
        }

        public static void TimerDispose()
        {
            Svc.Condition.ConditionChange -= OnCombat;
            Svc.Framework.Update -= UpdatePartyTimer;
            Svc.Framework.Update -= UpdateDeadtionary;
        }

        internal static void OnCombat(ConditionFlag flag, bool value)
        {
            if (flag == ConditionFlag.InCombat && value)
                combatStart = DateTime.Now;
        }
    }
}
