using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WrathCombo.Extensions;

namespace WrathCombo.Data;

internal partial class CustomComboCache : IDisposable
{
    private const uint InvalidStatusID = 0;

    //Invalidate this
    private readonly ConcurrentDictionary<(uint StatusID, ulong? TargetID, ulong? SourceID), IStatus?> statusCache = new();

    /// <summary> Finds a status on the given object. </summary>
    /// <param name="statusID"> Status effect ID. </param>
    /// <param name="obj"> Object to look for effects on. </param>
    /// <param name="sourceID"> Source object ID. </param>
    /// <returns> Status object or null. </returns>
    internal IStatus? GetStatus(uint statusID, IGameObject? obj, ulong? sourceID)
    {
        if (obj is null)
            return null;

        var key = (statusID, obj.GameObjectId, sourceID);

        if (statusCache.TryGetValue(key, out var found))
            return found;

        if (obj is not IBattleChara chara)
            return statusCache[key] = null;

        var statuses = chara.SafeStatusList;

        if (statuses is null)
            return statusCache[key] = null;

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

public class StatusCache
{
    internal const uint WeaknessStatusId = 43;
    internal const uint BrinkOfDeathStatusId = 44;
    internal const uint OCDarkDefensesStatusId = 4355;
    internal const uint DamageDownStatusId = 62;
    internal const uint DamageUpStatusId = 61;
    internal const uint EvasionUpStatusId = 31;

    private static StatusDictionaries? _dictionaries;

    internal static StatusDictionaries Dictionaries
    {
        get => _dictionaries ?? throw new InvalidOperationException("StatusCache Dictionaries not initialized");
        set => _dictionaries = value;  // Make it private-accessible to this class only
    }


    public static bool HasDamageDown(IGameObject? target) => HasStatusInCacheList(Dictionaries.DamageDownStatuses, target);
    public static bool HasCleansableDoom(IGameObject? target) => HasStatusInCacheList(Dictionaries.CleansableDoomStatuses, target);
    public static bool HasDamageUp(IGameObject? target) => HasStatusInCacheList(Dictionaries.DamageUpStatuses, target);
    public static bool HasEvasionUp(IGameObject? target) => HasStatusInCacheList(Dictionaries.EvasionUpStatuses, target);
    public static bool HasCleansableDebuff(IGameObject? target) => HasStatusInCacheList(Dictionaries.DispellableStatuses, target);
    public static bool HasBeneficialStatus(IGameObject? target) => HasStatusInCacheList(Dictionaries.BeneficialStatuses, target);
    public static bool HasRaiseInvincibility(IBattleChara? target) => HasStatusInCacheList(Dictionaries.RaiseInvincibilityStatuses, target);
    public static bool HasRaiseStatus(IBattleChara? target) => HasStatusInCacheList(Dictionaries.RaiseStatuses, target);

    /// <summary>
    /// Looks up the name of a Status by ID in Lumina Sheets
    /// </summary>
    /// <param name="id">Status ID</param>
    /// <returns></returns>
    public static string GetStatusName(uint id) => Dictionaries.StatusSheet.TryGetValue(id, out var status) ? status.Name.ToString() : "Unknown Status";

    /// <summary>
    /// Returns an uint List of Status IDs based on Name.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static List<uint>? GetStatusesByName(string status)
    {
        if (string.IsNullOrEmpty(status))
            return null;
        var statusIds = Dictionaries.StatusSheet
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
    public static bool HasStatusInCacheList(FrozenSet<uint>? statusList, IGameObject? gameObject)
    {
        if (statusList is null)
            return false;

        if (gameObject is not IBattleChara chara)
            return false;

        var statuses = chara.SafeStatusList;
        if (statuses is null)
            return false;

        return statuses.Any(s => statusList.Contains(s.StatusId));
    }

}

public class StatusDictionaries
{
    internal FrozenDictionary<uint, Lumina.Excel.Sheets.Status> StatusSheet = null!;
    internal FrozenDictionary<uint, Lumina.Excel.Sheets.Status> ENStatusSheet = null!;
    internal FrozenSet<uint> DamageDownStatuses = null!;
    internal FrozenSet<uint> CleansableDoomStatuses = null!;
    internal FrozenSet<uint> DamageUpStatuses = null!;
    internal FrozenSet<uint> EvasionUpStatuses = null!;
    internal FrozenSet<uint> DispellableStatuses = null!;
    internal FrozenSet<uint> BeneficialStatuses = null!;
    internal FrozenSet<uint> InvincibleStatuses = null!;
    internal FrozenSet<uint> RaiseInvincibilityStatuses = null!;
    internal FrozenSet<uint> RaiseStatuses = null!;
    internal FrozenSet<uint> DoNotHealStatuses = null!;
    internal FrozenSet<uint> AccelerationBombs = null!;
    internal FrozenSet<uint> Pyretics = null!;
    internal FrozenSet<uint> MiscPausing = null!;

    // Init Dictionaries
    internal StatusDictionaries()
    {
        Parallel.Invoke(
            () => StatusSheet = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>()
                                .ToFrozenDictionary(i => i.RowId),

            () => ENStatusSheet = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>(Dalamud.Game.ClientLanguage.English)
                                    .ToFrozenDictionary(i => i.RowId)
            );

        Parallel.Invoke(
            () => DamageDownStatuses =
                    ENStatusSheet.TryGetValue(StatusCache.DamageDownStatusId, out var ddRow)
                    ? ENStatusSheet
                        .Where(x => x.Value.Name.ToString().Equals(ddRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                        .Select(x => x.Key)
                        .ToFrozenSet()
                    : [],

            () => CleansableDoomStatuses =
                    StatusSheet
                        .Where(x => x.Value.Icon == 215503 && x.Value.CanDispel)
                        .Select(x => x.Key)
                        .ToFrozenSet(),

            () => DamageUpStatuses =
                    ENStatusSheet.TryGetValue(StatusCache.DamageUpStatusId, out var duRow)
                        ? ENStatusSheet
                            .Where(x => x.Value.Name.ToString().Contains(duRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                            .Select(x => x.Key)
                            .ToFrozenSet()
                        : [],

            () => EvasionUpStatuses =
                    ENStatusSheet.TryGetValue(StatusCache.EvasionUpStatusId, out var euRow)
                        ? ENStatusSheet
                            .Where(x => x.Value.Name.ToString().Contains(euRow.Name.ToString(), StringComparison.CurrentCultureIgnoreCase))
                            .Select(x => x.Key)
                            .ToFrozenSet()
                        : [],

            () => DispellableStatuses =
                    StatusSheet
                        .Where(kvp => kvp.Value.CanDispel)
                        .Select(kvp => kvp.Key)
                        .ToFrozenSet(),

            () => BeneficialStatuses =
                    StatusSheet
                        .Where(kvp => kvp.Value.StatusCategory == 1)
                        .Select(kvp => kvp.Key)
                        .ToFrozenSet(),

            () => InvincibleStatuses =
                    StatusSheet
                        .Where(row => row.Value.Icon == 215024)
                        .Select(row => row.Key)
                        .Concat(new uint[] {
                            151, 198, 469, 592, 1240, 1302, 1303,
                            1567, 1936, 2413, 2654, 3012, 3039,
                            3052, 3054, 4175
                        })
                        .ToFrozenSet(),

            () => RaiseInvincibilityStatuses =
                    StatusSheet
                        .Where(row => row.Value.Icon == 215273) // Transcendant statuses, based on Icon
                        .Select(row => row.Key)
                        .ToFrozenSet(),

            () => RaiseStatuses =
                    StatusSheet
                        .Where(row => row.Value.Icon == 210406) // Raise statuses, based on Icon
                        .Select(row => row.Key)
                        .ToFrozenSet(),

            () => DoNotHealStatuses =
                    new uint[] {
                        2852,
                    }.ToFrozenSet(),

            () => AccelerationBombs =
                    new HashSet<uint>(
                        StatusSheet
                            .Where(row => row.Value.Icon == 215727) // Acceleration Bomb Icon
                            .Select(row => row.Key)
                    )
                    {
                        1132, // Baelsar's Wall - Extreme Caution
                        4130 // Authority's Hold

                    }.ToFrozenSet(),

            () => Pyretics =
                    new HashSet<uint>(
                        StatusSheet
                            .Where(row => row.Value.Icon == 215647) // Pyretic Icon
                            .Select(row => row.Key)
                    )
                    {
                        514 // Causality
                    }.ToFrozenSet(),

            () => MiscPausing =
                    new uint[] {
                        1735 // The Orbonne Monastary - Heavenly Shield
                    }.ToFrozenSet()
        );
    }
}