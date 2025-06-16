using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Extensions;
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
        private static readonly Dictionary<uint, Lumina.Excel.Sheets.Status> StatusSheet = 
            Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>().ToDictionary(i => i.RowId, i => i);


        private static readonly HashSet<uint> DamageDownStatuses =
            StatusSheet.TryGetValue(62, out var refRow)
                ? [.. StatusSheet
                .Where(x => x.Value.Name.ToString().Equals(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)] : [];

        public static bool HasDamageDown(IGameObject? target) => HasStatusInCacheList(DamageDownStatuses, target);

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
        /// </remarks>
        private static readonly HashSet<uint> InvincibleStatuses = GenerateInv();

        private static HashSet<uint> GenerateInv()
        {
            //Search by Invincibility Icon
            uint targetIcon = 215024; //StatusSheet.FirstOrDefault(row => row.Value.RowId == 325).Value.Icon;
            //Svc.Log.Debug($"Invincible Icon: {targetIcon}");

            var invincibles = StatusSheet.Where(row => row.Value.Icon == targetIcon).Select(row => row.Key).ToHashSet();
            //Add Random Invulnerabilities not yet assigned to TerritoryType
            invincibles.UnionWith([151, 198, 385, 469, 592, 1240, 1302, 1303, 1567, 1936, 2413, 2654, 3012, 3039, 3052, 3054, 4175]);
            return invincibles;
        }

        public static bool TargetIsInvincible(IGameObject? target)
        {
            if (target is not IBattleChara tar)
                return false;

            // Turn Target's status to uint hashset
            var targetStatuses = tar.StatusList.Select(s => s.StatusId).ToHashSet();
            uint targetID = tar.DataId;

            switch (Svc.ClientState.TerritoryType)
            {
                case 174:   // Labyrinth of the Ancients
                    // Thanatos, Spooky Ghosts Only
                    if (targetID is 2350) return !HasStatusEffect(398);

                    // Allagan Bomb
                    if (targetID is 2407) return NumberOfEnemiesInRange(30) > 1;

                    return false;
                case 1248:  // Jeuno 1 Ark Angels
                    // ArkAngel HM = 1804
                    if (targetID is 18049 && HasStatusEffect(4410, tar, true)) return true;

                    // ArkAngel MR = 18051 (A)
                    // ArkAngel GK = 18053 (B)
                    // ArkAngel TT = 18052 (C)
                    if (targetID is 18051 or 18052 or 18053)
                    {
                        if (HasStatusEffect(4192)) return targetID != 18051; // Alliance A Red Epic
                        if (HasStatusEffect(4194)) return targetID != 18053; // Alliance B Yellow Fated
                        if (HasStatusEffect(4196)) return targetID != 18052; // Alliance C Blue Vaunted
                    }
                    return false;
                case 917:   //Puppet's Bunker, Flight Mechs
                    // 724P Alpha = 11792 (A)
                    // 767P Beta  = 11793 (B)
                    // 772P Chi   = 11794 (C)
                    if (targetID is 11792 or 11793 or 11794)
                    {
                        if (HasStatusEffect(2288)) return targetID != 11792;
                        if (HasStatusEffect(2289)) return targetID != 11793;
                        if (HasStatusEffect(2290)) return targetID != 11794;
                    }
                    return false;
                case 801 or 805 or 1122: //Interdimensional Rift (Omega 12 / Alphascape 4), Regular/Savage?/Ultimate?
                    // Omega-M = 9339
                    // Omega-F = 9340
                    if (targetID is 9339 or 9340) //numbers are for Regular
                    {
                        if (HasStatusEffect(1660)) return targetID == 9339; // Packet Filter M
                        if (HasStatusEffect(1661)) return targetID == 9340; // Packet Filter F
                        if (targetID is 9340) return HasStatusEffect(671, tar, true); // F being covered by M
                    }

                    //Savage/Ultimate? Not sure which omega fight uses 3499 and 3500.
                    //Also, SE, why use a new Omega-M status and reuse the old Omega-F? -_-'
                    //Wonder if targetIDs are the same......
                    if ((tar.StatusList.Any(x => x.StatusId == 3454) && HasStatusEffect(3499)) ||
                        (tar.StatusList.Any(x => x.StatusId == 1675) && HasStatusEffect(3500)))
                        return true;

                    //Check for any ol invincibility
                    if (CompareLists(InvincibleStatuses, targetStatuses)) return true;

                    return false;
                case 952:  //ToZ final boss (technically not invincible)
                    if (targetID is (13298 or 13299) && Svc.Objects.Any(y => y.DataId is 13297 && !y.IsDead))
                        return true;

                    return false;
            }

            // General invincibility check
            // Due to large size of InvincibleStatuses, best to check process this way
            if (CompareLists(InvincibleStatuses, targetStatuses)) return true;
            return false;
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
        private static bool CompareLists(HashSet<uint> statusList, HashSet<uint> charaStatusList) => 
            charaStatusList.Any(id => statusList.Contains(id));
    }
}
