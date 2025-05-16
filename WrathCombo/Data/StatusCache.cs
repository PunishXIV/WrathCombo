using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS.Functions;
using Status = Dalamud.Game.ClientState.Statuses.Status; // conflicts with structs if not defined

namespace WrathCombo.Data
{
    internal partial class CustomComboCache : IDisposable
    {
        //Invalidate this
        private readonly ConcurrentDictionary<(uint StatusID, ulong? TargetID, ulong? SourceID), Status?> statusCache = new();

        /// <summary> Finds a status on the given object. </summary>
        /// <param name="statusID"> Status effect ID. </param>
        /// <param name="obj"> Object to look for effects on. </param>
        /// <param name="sourceID"> Source object ID. </param>
        /// <returns> Status object or null. </returns>
        internal Status? GetStatus(uint statusID, IGameObject? obj, ulong? sourceID)
        {
            if (obj is null) return null;
            var key = (statusID, obj?.GameObjectId, sourceID);
            if (statusCache.TryGetValue(key, out Status? found))
                return found;

            if (obj is null)
                return statusCache[key] = null;

            if (obj is not IBattleChara chara)
                return statusCache[key] = null;

            foreach (Status? status in chara.StatusList)
            {
                if (status.StatusId == statusID && (!sourceID.HasValue || status.SourceId == 0 || status.SourceId == InvalidObjectID || status.SourceId == sourceID))
                    return statusCache[key] = status;
            }

            return statusCache[key] = null;
        }
    }

    internal class StatusCache
    {
        /// <summary>
        /// Lumina Status Sheet Dictionary
        /// </summary>
        private static readonly Dictionary<uint, Lumina.Excel.Sheets.Status> StatusSheet = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>()!
            .ToDictionary(i => i.RowId, i => i);

        /// <summary>
        /// Enum defining status effects with their name IDs.
        /// </summary>
        public enum StatusEffect
        {
            Amnesia = 5,
            Pacification = 6,
            Silence = 7,
            Weakness = 43,
            BrinkOfDeath = 44,
            DamageDown = 62
        }

        /// <summary>
        /// Cached dictionary mapping status effects to their HashSet of status IDs.
        /// </summary>
        private static readonly Dictionary<StatusEffect, HashSet<uint>> StatusIdCaches = InitializeStatusIdCaches();

        /// <summary>
        /// Initializes the status ID caches in a single StatusSheet iteration.
        /// </summary>
        private static Dictionary<StatusEffect, HashSet<uint>> InitializeStatusIdCaches()
        {
            var statusEffects = Enum.GetValues<StatusEffect>();
            var nameToEffect = statusEffects
                .Select(e => (Effect: e, Name: GetStatusName((uint)e)))
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .ToDictionary(x => x.Name, x => x.Effect, StringComparer.OrdinalIgnoreCase);

            var caches = statusEffects
                .SelectMany(e => StatusSheet
                    .Where(s => s.Value.Name.ToString().Equals(GetStatusName((uint)e), StringComparison.OrdinalIgnoreCase))
                    .Select(s => (Effect: e, Id: s.Key)))
                .GroupBy(x => x.Effect)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToHashSet());

            // Ensure all status effects have an entry
            foreach (var effect in statusEffects)
                caches.TryAdd(effect, new HashSet<uint>());

            var emptyCaches = caches
                .Where(kvp => kvp.Value.Count == 0)
                .Select(kvp => $"No status IDs found for status effect: {kvp.Key} ({GetStatusName((uint)kvp.Key)})");
            foreach (var warning in emptyCaches)
                Svc.Log.Warning(warning);

            return caches;
        }

        public static bool HasAmnesia() => HasStatusInCacheList(StatusEffect.Amnesia);

        public static bool HasPacification() => HasStatusInCacheList(StatusEffect.Pacification);

        public static bool HasSilence() => HasStatusInCacheList(StatusEffect.Silence);

        public static bool HasWeakness(IGameObject? target) => HasStatusInCacheList(StatusEffect.Weakness, target);

        public static bool HasBrinkOfDeath(IGameObject? target) => HasStatusInCacheList(StatusEffect.BrinkOfDeath, target);

        public static bool HasDamageDown(IGameObject? target) => HasStatusInCacheList(StatusEffect.DamageDown, target);

        /// <summary>
        /// A cached set of dispellable status IDs for quick lookup.
        /// </summary>
        private static readonly HashSet<uint> DispellableStatuses = [..
            StatusSheet
                .Where(kvp => kvp.Value.CanDispel)
                .Select(kvp => kvp.Key)];
        public static bool HasCleansableDebuff(IGameObject? target) => HasStatusInCacheList(DispellableStatuses, target);

        /// <summary>
        /// A set of status effect IDs that grant general invincibility.
        /// </summary>
        /// <remarks>
        /// Includes statuses like Hallowed Ground (151), Living Dead (325), etc.
        /// Last updated: May 2025 for FFXIV patch 7.x.
        /// </remarks>
        private static readonly HashSet<uint> InvincibleStatuses =
        [
            151, 198, 325, 328, 385, 394, 469, 529, 592, 656, 671, 775, 776, 895, 969, 981,
            1240, 1302, 1303, 1567, 1570, 1697, 1829, 1936, 2413, 2654, 3012, 3039, 3052, 3054,
            4410, 4175
        ];
        private static readonly HashSet<uint> JeunoPlayerStatuses = [4192, 4194, 4196];
        private static readonly HashSet<uint> JeunoVulnerableStatuses = [4193, 4195, 4197];
        private static readonly HashSet<uint> YoRHaStatuses = [2409, 2410, 2411];
        private static readonly HashSet<uint> OmegaTargetStatuses = [1674, 3454, 1675];
        private static readonly HashSet<uint> OmegaPlayerStatuses = [1660, 3499, 1661, 3500];

        public static bool TargetIsInvincible(IGameObject? target)
        {
            if (target is not IBattleChara tar || tar.StatusList == null)
                return false;

            // General invincibility check

            byte method = 1;

            switch (method)
            {
                case 1:
                    if (HasStatusInCacheList(InvincibleStatuses, tar))
                        return true;
                    break;
                case 2:
                    {
                        var targetStatuses = tar.StatusList.Select(s => s.StatusId).ToHashSet();
                        if (targetStatuses.Any(id => InvincibleStatuses.Contains(id))) return true;
                        break;
                    }
            }

            // Jeuno Ark Angel Encounter
            //if (Svc.ClientState.TerritoryType == ?)
            //{
            if (HasStatusInCacheList(JeunoPlayerStatuses,Player.Object) && !HasStatusInCacheList(JeunoVulnerableStatuses, tar))
                return true;
            //}

            // YoRHa raid encounter
            //if (Svc.ClientState.TerritoryType == ?)
            //{
            var alliance = CustomComboFunctions.GetAllianceGroup();
            if ((alliance != AllianceGroup.GroupA && HasStatusInCacheList(YoRHaStatuses, tar)) ||
                (alliance != AllianceGroup.GroupB && HasStatusInCacheList(YoRHaStatuses, tar)) ||
                (alliance != AllianceGroup.GroupC && HasStatusInCacheList(YoRHaStatuses, tar)))
                return true;
            //}

            // Omega
            //if (Svc.ClientState.TerritoryType == ?)
            //{
            if ((HasStatusInCacheList(OmegaTargetStatuses, tar) && HasStatusInCacheList(OmegaPlayerStatuses, Player.Object)) ||
                (HasStatusInCacheList(OmegaTargetStatuses, tar) && HasStatusInCacheList(OmegaPlayerStatuses, Player.Object)))
                return true;
            //}

            return false;
        }

        public static string GetStatusName(uint id) => StatusSheet.TryGetValue(id, out var status) ? status.Name.ToString() : "Unknown Status";

        /// <summary>
        /// Returns an uint List of Status IDs based on Name.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static List<uint>? GetStatusesByName(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;
            var statusIds = StatusSheet
                .Where(x => x.Value.Name.ToString().Equals(status, StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)
                .ToList();
            return statusIds.Count != 0 ? statusIds : null;
        }

        /// <summary>
        /// Compares a list of Status IDs against the Player or optional Target.
        /// </summary>
        /// <param name="list">HashSet uint. Use list from StatusCache</param>
        /// <param name="target">Optional IGameObject to check</param>
        /// 
        /// <returns></returns>
        internal static bool HasStatusInCacheList(HashSet<uint> list, IGameObject? target)
        {
            if (list.Count is not 0)
            {
                if (target is not IBattleChara chara || chara.StatusList == null)
                    return false;

                foreach (uint status in list)
                {
                    if (CustomComboFunctions.HasStatusEffect(status, chara, true)) return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Checks if a target has any status effect for the given status effect.
        /// </summary>
        internal static bool HasStatusInCacheList(StatusEffect effect, IGameObject? target = null)
        {
            return HasStatusInCacheList(StatusIdCaches[effect], target);
        }

        //internal static bool HasStatusInCacheList(HashSet<uint> list) => list.Count switch
        //{
        //    0 => false,
        //    _ => HasStatusInCacheList(list, Player.Object)
        //};

    }
}
