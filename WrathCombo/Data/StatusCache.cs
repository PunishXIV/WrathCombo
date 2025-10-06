using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Status = Dalamud.Game.ClientState.Statuses.Status; // conflicts with structs if not defined
namespace WrathCombo.Data;

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

    private static readonly FrozenSet<uint> DamageDownStatuses =
        ENStatusSheet.TryGetValue(62, out var refRow)
            ? ENStatusSheet
                .Where(x => x.Value.Name.ToString().Equals(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)
                .ToFrozenSet()
            : [];

    public static bool HasDamageDown(IGameObject? target) => HasStatusInCacheList(DamageDownStatuses, target);

    private static readonly FrozenSet<uint> DamageUpStatuses =
        ENStatusSheet.TryGetValue(61, out var refRow)
            ? ENStatusSheet
                .Where(x => x.Value.Name.ToString().Contains(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)
                .ToFrozenSet()
            : [];

    public static bool HasDamageUp(IGameObject? target) => HasStatusInCacheList(DamageUpStatuses, target);

    private static readonly FrozenSet<uint> evasionUpStatuses =
        ENStatusSheet.TryGetValue(61, out var refRow)
            ? ENStatusSheet
                .Where(x => x.Value.Name.ToString().Contains(refRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Key)
                .ToFrozenSet()
            : [];

    public static bool HasEvasionUp(IGameObject? target) => HasStatusInCacheList(evasionUpStatuses, target);

    /// <summary>
    /// A cached set of dispellable status IDs for quick lookup.
    /// </summary>
    private static readonly FrozenSet<uint> DispellableStatuses =
        StatusSheet
            .Where(kvp => kvp.Value.CanDispel)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

    public static bool HasCleansableDebuff(IGameObject? target) => HasStatusInCacheList(DispellableStatuses, target);

    /// <summary>
    /// A cached set of beneficial status IDs for quick lookup.
    /// </summary>
    private static readonly FrozenSet<uint> BeneficialStatuses =
        StatusSheet
            .Where(kvp => kvp.Value.StatusCategory == 1)
            .Select(kvp => kvp.Key)
            .ToFrozenSet();

    public static bool HasBeneficialStatus(IGameObject? targt) => HasStatusInCacheList(BeneficialStatuses, targt);

    /// <summary>
    /// A set of status effect IDs that grant general invincibility.
    /// </summary>
    /// <remarks>
    /// Includes statuses like Hallowed Ground (151), Living Dead (325), etc.
    /// Icon from StatusSheet.FirstOrDefault(row => row.Value.RowId == 325).Value.Icon; (General Invincibility)
    /// </remarks>
    internal static readonly FrozenSet<uint> InvincibleStatuses =
        StatusSheet
            .Where(row => row.Value.Icon == 215024)
            .Select(row => row.Key)
            .Concat(new uint[] {
                151, 198, 469, 592, 1240, 1302, 1303,
                1567, 1936, 2413, 2654, 3012, 3039,
                3052, 3054, 4175
            })
            .ToFrozenSet();

    public static class PausingStatuses
    {
        internal static readonly FrozenSet<uint> AccelerationBombs =
            new HashSet<uint>(
                StatusSheet
                    .Where(row => row.Value.Icon == 215727) // Acceleration Bomb Icon
                    .Select(row => row.Key)
            )
            {
            4130 // Authority's Hold
            }.ToFrozenSet();

        internal static readonly FrozenSet<uint> Pyretics =
            new HashSet<uint>(
                StatusSheet
                    .Where(row => row.Value.Icon == 215647) // Pyretic Icon
                    .Select(row => row.Key)
            )
            {
            514 // Causality
            }.ToFrozenSet();

        internal static readonly FrozenSet<uint> Misc = new uint[] {
            1735 // The Orbonne Monastary - Heavenly Shield
        }.ToFrozenSet();

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
    internal static bool HasStatusInCacheList(FrozenSet<uint> statusList, IGameObject? gameObject = null)
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
    internal static bool CompareLists(FrozenSet<uint> statusList, HashSet<uint> charaStatusList) =>
        charaStatusList.Any(id => statusList.Contains(id));
}