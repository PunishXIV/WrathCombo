#region

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ExcelServices.Enums;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

#endregion

namespace WrathCombo.Services;

public class Inventory
{
    private readonly unsafe InventoryManager* _manager =
        InventoryManager.Instance();

    private readonly FrozenDictionary<uint, Item> _itemSheet =
        Svc.Data.GetExcelSheet<Item>(ClientLanguage.English)
            .Where(IsItemWeCareAbout)
            .ToFrozenDictionary(i => i.RowId);

    public unsafe void DebugInventory()
    {
        PluginLog.Debug(
            "[InventoryService] " +
            "inventory loaded: " + _manager->Inventories->IsLoaded +
            ", found items: " + _itemSheet.Count +
            ", item names: " + string.Join(", ", _itemSheet
                .Select(x => x.Value.GetName())) +
            ", quantity pot: " + _manager->GetInventoryItemCount(38956u.HQ(), true)
        );
    }

    #region .Where() Methods

    private static bool IsPhoenixDown(Item item) =>
        item.ItemUICategory.RowId == (uint)ItemUICategoryEnum.Medicine &&
        item.ItemAction is { IsValid: true, RowId: (uint)ItemAction.PhoenixDown };

    private static bool IsMedicine(Item item) =>
        item.ItemUICategory.RowId == (uint)ItemUICategoryEnum.Medicine &&
        item.ItemAction.IsValid &&
        item.GivesStatus(ItemStatus.Medicated);

    private static bool IsItemWeCareAbout(Item item) =>
        IsPhoenixDown(item) ||
        (IsMedicine(item) &&
         (IsStatPotion(item) ||
          IsHPPotion(item) ||
          IsMPPotion(item)));

    private static bool IsStatPotion(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param is (uint)BaseParamEnum.Strength or
                (uint)BaseParamEnum.Dexterity or
                (uint)BaseParamEnum.Vitality or
                (uint)BaseParamEnum.Intelligence or
                (uint)BaseParamEnum.Mind);

    private static bool IsHPPotion(Item item) =>
        (item.BaseParams() ?? []).Any(param => param == (uint)BaseParamEnum.HP);

    private static bool IsMPPotion(Item item) =>
        (item.BaseParams() ?? []).Any(param => param == (uint)BaseParamEnum.MP);

    #endregion
}

#region Static Data

internal enum ItemStatus
{
    WellFed = 48,
    Medicated = 49,
}

internal enum ItemAction
{
    PhoenixDown = 44,
}

#endregion

internal static class ItemExtensions
{
    private static readonly Dictionary<uint, uint[]?> SavedBaseParams = [];

    internal static bool GivesStatus(this Item item, ItemStatus status) =>
        item.ItemAction.IsValid &&
        (item.ItemAction.Value.Data[(int)DataKeys.Status] == (ushort)status ||
         item.ItemAction.Value.DataHQ[(int)DataKeys.Status] == (ushort)status);

    internal static ItemFood? FoodRow(this Item item, bool? hq = null)
    {
        if (!item.ItemAction.IsValid)
            return null;

        if (hq is null or false &&
            Svc.Data.GetExcelSheet<ItemFood>()
                .TryGetRow(item.ItemAction.Value.Data[(int)DataKeys.ItemFoodRowId],
                    out var row))
            return row;

        if (hq is null or true &&
            Svc.Data.GetExcelSheet<ItemFood>()
                .TryGetRow(item.ItemAction.Value.DataHQ[(int)DataKeys.ItemFoodRowId],
                    out var rowHQ))
            return rowHQ;

        return null;
    }

    internal static uint[]? BaseParams(this Item item, bool? hq = null)
    {
        var itemID = hq is true ? item.RowId.HQ() : item.RowId;

        // Return cached values
        if (SavedBaseParams.TryGetValue(itemID, out var savedParams))
            return savedParams;

        var row = item.FoodRow(hq);
        var foundParams = row?.Params.Select(x => x.BaseParam.RowId).ToArray();
        SavedBaseParams[itemID] = foundParams;

        return foundParams;
    }

    internal static uint HQ(this uint itemID) =>
        itemID + 1_000_000;

    #region Static Data

    private enum DataKeys
    {
        Status = 0,
        ItemFoodRowId = 1,
        StatusDuration = 2,
    }

    #endregion
}