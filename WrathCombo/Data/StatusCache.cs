﻿using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Status = Dalamud.Game.ClientState.Statuses.Status; // conflicts with structs if not defined

namespace WrathCombo.Data
{
    internal partial class CustomComboCache : IDisposable
    {
        private const uint InvalidStatusID = 0;

        //Invalidate this
        private readonly ConcurrentDictionary<(uint StatusID, ulong? TargetID, ulong? SourceID), Status?> statusCache = new();

        /// <summary> Finds a status on the given object. </summary>
        /// <param name="statusID"> Status effect ID. </param>
        /// <param name="obj"> Object to look for effects on. </param>
        /// <param name="sourceID"> Source object ID. </param>
        /// <returns> Status object or null. </returns>
        internal Status? GetStatus(uint statusID, IGameObject? obj, ulong? sourceID)
        {
            if (obj is null)
                return null;

            var key = (statusID, obj.GameObjectId, sourceID);

            if (statusCache.TryGetValue(key, out var found))
                return found;

            if (obj is not IBattleChara chara)
                return statusCache[key] = null;

            var statuses = chara.StatusList;
            foreach (var status in statuses)
            {
                if (status.StatusId == InvalidStatusID)
                    continue;

                if (status.StatusId == statusID &&
                    (!sourceID.HasValue || status.SourceId == 0 || status.SourceId == InvalidObjectID || status.SourceId == sourceID))
                {
                    return statusCache[key] = status;
                }
            }

            return statusCache[key] = null;
        }
    }

    internal class StatusCache
    {
        /// <summary>
        /// Lumina Status Sheet Dictionary
        /// </summary>
        private static readonly FrozenDictionary<uint, Lumina.Excel.Sheets.Status> StatusSheet = 
            Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>()
                .ToFrozenDictionary(i => i.RowId);

        private static readonly FrozenDictionary<uint, Lumina.Excel.Sheets.Status> ENStatusSheet =
            Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>(Dalamud.Game.ClientLanguage.English)
                .ToFrozenDictionary(i => i.RowId);

        private static readonly HashSet<uint> DamageDownStatuses =
            ENStatusSheet.TryGetValue(62, out var refRow)
                ? [.. ENStatusSheet
                .Where(x => x.Value.Name.ToString().Equals(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)] : [];

        public static bool HasDamageDown(IGameObject? target) => HasStatusInCacheList(DamageDownStatuses, target);

        private static readonly HashSet<uint> DamageUpStatuses =
            ENStatusSheet.TryGetValue(61, out var refRow)
                 ? [.. ENStatusSheet
                .Where(x => x.Value.Name.ToString().Contains(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)] : [];

        public static bool HasDamageUp(IGameObject? target) => HasStatusInCacheList(DamageUpStatuses, target);
        
        public static bool HasEvasionUp(IGameObject? target)
        {
            HashSet<uint> evasionUpStatuses =
                ENStatusSheet.TryGetValue(61, out var refRow)
                    ? [.. ENStatusSheet
                        .Where(x => x.Value.Name.ToString().Contains(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                        .Select(x => x.Key)]
                    : [];
            return HasStatusInCacheList(evasionUpStatuses, target);
        }

        /// <summary>
        /// A cached set of dispellable status IDs for quick lookup.
        /// </summary>
        private static readonly HashSet<uint> DispellableStatuses = [..
            StatusSheet
                .Where(kvp => kvp.Value.CanDispel)
                .Select(kvp => kvp.Key)];

        public static bool HasCleansableDebuff(IGameObject? target) => HasStatusInCacheList(DispellableStatuses, target);

        /// <summary>
        /// A cached set of beneficial status IDs for quick lookup.
        /// </summary>
        private static readonly HashSet<uint> BeneficialStatuses = [..
            StatusSheet
                .Where(kvp => kvp.Value.StatusCategory == 1)
                .Select(kvp => kvp.Key)];

        public static bool HasBeneficialStatus(IGameObject? targt) => HasStatusInCacheList(BeneficialStatuses, targt);

        /// <summary>
        /// A set of status effect IDs that grant general invincibility.
        /// </summary>
        /// <remarks>
        /// Includes statuses like Hallowed Ground (151), Living Dead (325), etc.
        /// </remarks>
        internal static readonly HashSet<uint> InvincibleStatuses = GenerateInv();

        private static HashSet<uint> GenerateInv()
        {
            //Search by Invincibility Icon
            uint targetIcon = 215024; //StatusSheet.FirstOrDefault(row => row.Value.RowId == 325).Value.Icon;
            //Svc.Log.Debug($"Invincible Icon: {targetIcon}");

            var invincibles = StatusSheet.Where(row => row.Value.Icon == targetIcon).Select(row => row.Key).ToHashSet();
            //Add Random Invulnerabilities not yet assigned to TerritoryType
            invincibles.UnionWith([151, 198, 469, 592, 1240, 1302, 1303, 1567, 1936, 2413, 2654, 3012, 3039, 3052, 3054, 4175]);
            return invincibles;
        }

        

        /// <summary>
        /// Looks up the name of a Status by ID in Lumina Sheets
        /// </summary>
        /// <param name="id">Status ID</param>
        /// <returns></returns>
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
        /// Checks a GameObject's Status list against a set of Status IDs
        /// </summary>
        /// <param name="statusList">Hashset of Status IDs to check</param>
        /// <param name="gameObject">GameObject to check</param>
        /// <returns></returns>
        internal static bool HasStatusInCacheList(HashSet<uint> statusList, IGameObject? gameObject = null)
        {
            if (gameObject is not IBattleChara chara)
                return false;

            var statuses = chara.StatusList;
            var targetStatuses = statuses.Select(s => s.StatusId).ToHashSet();
            return statusList.Count switch
            {
                0 => false,
                _ => CompareLists(statusList, targetStatuses)
            };
        }

        /// <summary>
        /// Compares two hashsets, in this case, used to compare a cached set of status IDs against a character's StatusID list
        /// </summary>
        /// <param name="statusList"></param>
        /// <param name="charaStatusList"></param>
        /// <returns></returns>
        internal static bool CompareLists(HashSet<uint> statusList, HashSet<uint> charaStatusList) => 
            charaStatusList.Any(id => statusList.Contains(id));
    }
}
