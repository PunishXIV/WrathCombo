#region

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ExcelServices.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Status = FFXIVClientStructs.FFXIV.Client.Game.Status;

#endregion

namespace WrathCombo.Services;

public static class Inventory
{
    private static readonly FrozenDictionary<uint, Item> ItemSheet =
        Svc.Data.GetExcelSheet<Item>(ClientLanguage.English)
            .Where(IsItemWeCareAbout)
            .ToFrozenDictionary(i => i.RowId);

    private static unsafe InventoryManager* Manager => InventoryManager.Instance();

    public static unsafe void t()
    {
        var l = Manager->Inventories;
        var o = l->IsLoaded;
    }

    #region .Where() Methods

    private static bool IsMedicine(Item item) =>
        item.ItemUICategory.RowId == (uint)ItemUICategoryEnum.Medicine &&
        item.ItemAction.IsValid &&
        item.GivesStatus(ItemStatus.Medicated);

    private static bool IsItemWeCareAbout(Item item)
    {
        if (!IsMedicine(item))
            return false;

        return true;
    }

    private static bool IsStatPotion(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param == (uint)BaseParamEnum.Strength ||
            param == (uint)BaseParamEnum.Dexterity ||
            param == (uint)BaseParamEnum.Vitality ||
            param == (uint)BaseParamEnum.Intelligence ||
            param == (uint)BaseParamEnum.Mind);

    #endregion
}

internal enum ItemStatus
{
    WellFed = 48,
    Medicated = 49,
}

internal static class ItemExtensions
{
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

    private static readonly Dictionary<uint, uint[]?> SavedBaseParams = [];
    
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

    private enum DataKeys
    {
        Status = 0,
        ItemFoodRowId = 1,
        StatusDuration = 2,
    }
}