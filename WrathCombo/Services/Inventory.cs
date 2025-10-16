#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dalamud.Game;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ExcelServices.Enums;
using ECommons.GameHelpers;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using WrathCombo.Core;
using Item = Lumina.Excel.Sheets.Item;

#endregion

namespace WrathCombo.Services;

public class Inventory
{
    private readonly Dictionary<uint, Item> _itemSheet =
        Svc.Data.GetExcelSheet<Item>(ClientLanguage.English)
            .Where(IsItemWeCareAbout)
            .ToDictionary(i => i.RowId);

    private readonly unsafe InventoryManager* _manager =
        InventoryManager.Instance();

    private readonly Dictionary<Core.Item, Dictionary<int, uint[]>>
        _usersItems = [];

    /// <summary>
    ///     Builds out the <see cref="_usersItems" /> Data Structure.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If there is a <see cref="Core.Item" /> Enum that is not also present in
    ///     <see cref="GetAssociatedSubEnum" />.
    /// </exception>
    public Inventory()
    {
        foreach (var typeOfItem in Enum.GetValues<Core.Item>())
        {
            _usersItems[typeOfItem] = new Dictionary<int, uint[]>();

            var enumToBuildOut = GetAssociatedSubEnum(typeOfItem);

            foreach (var itemType in Enum.GetValues(enumToBuildOut))
                _usersItems[typeOfItem][(int)itemType] = [];
        }
    }

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

    /// <summary>
    ///     Fills <see cref="_usersItems" /> with what is in the user's inventory.
    /// </summary>
    /// <returns>
    ///     Whether the inventory was filled correctly.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If there is a <see cref="Core.Item" /> Enum that is not also present in
    ///     <see cref="GetAssociatedAssociatedWhereMethod" />.
    /// </exception>
    private unsafe bool FillUserInventory()
    {
        try
        {
            if (!Player.Available || !_manager->Inventories->IsLoaded)
                return false;

            foreach (var typeOfItem in Enum.GetValues<Core.Item>())
            {
                var enumToFill = GetAssociatedSubEnum(typeOfItem);

                foreach (var itemType in Enum.GetValues(enumToFill))
                {
                    var whereFunc =
                        GetAssociatedAssociatedWhereMethod(enumToFill,
                            (int)itemType);
                    List<uint> foundItems = [];
                    var itemsToLookFor = _itemSheet
                        .Select(x => x.Value).Where(whereFunc)
                        .Select(x => x.RowId).ToArray();

                    foreach (var item in itemsToLookFor)
                    {
                        var nq = _manager->GetInventoryItemCount(item);
                        var hq = _manager->GetInventoryItemCount(item.HQ(), true);
                        if (nq > 0 || hq > 0)
                            foundItems.Add(item);
                    }

                    _usersItems[typeOfItem][(int)itemType] = foundItems.ToArray();
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    #region Utility Methods

    // ReSharper disable once EntityNameCapturedOnly.Local
    /// <summary>
    ///     Gets the associated <c>.Where()</c> associated with each entry in
    ///     <see cref="Core.Item" /> (and specific ones for
    ///     <see cref="StatPotionType" />).<br />
    ///     (<see cref="IsHPPotion" />, <see cref="IsStatPotionStr" />, etc)
    /// </summary>
    /// <param name="enumToMatch">
    ///     The <see cref="Core.Item" /> Enum to get the associated method for.
    /// </param>
    /// <param name="pot">
    ///     The ID of the Enum, to be used for <see cref="StatPotionType" />-specific
    ///     methods.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If there is a <see cref="Core.Item" /> or <see cref="StatPotionType" />
    ///     Enum that is not also present in the <see langword="switch" /> here.
    /// </exception>
    private Func<Item, bool>
        GetAssociatedAssociatedWhereMethod(Type enumToMatch, int pot) =>
        nameof(enumToMatch) switch
        {
            nameof(ItemType)          => IsPhoenixDown,
            nameof(ManaPotionType)    => IsMPPotion,
            nameof(HealingPotionType) => IsHPPotion,
            nameof(StatPotionType) when pot is (int)StatPotionType.Strength
                => IsStatPotionStr,
            nameof(StatPotionType) when pot is (int)StatPotionType.Dexterity
                => IsStatPotionDex,
            nameof(StatPotionType) when pot is (int)StatPotionType.Vitality
                => IsStatPotionVit,
            nameof(StatPotionType) when pot is (int)StatPotionType
                    .Intelligence
                => IsStatPotionInt,
            nameof(StatPotionType) when pot is (int)StatPotionType.Mind
                => IsStatPotionMnd,
            _ => throw new ArgumentOutOfRangeException("",
                "Core.ItemUsage.Item has an enum value not handled " +
                "in Services.Inventory.GetAssociatedWhereMethod()"),
        };

    /// <summary>
    ///     Gets the associated &gt;Item&lt;Type Enum associated with each entry in
    ///     <see cref="Core.Item" />.<br />
    ///     (<see cref="ItemType" />, <see cref="StatPotionType" />, etc)
    /// </summary>
    /// <param name="item">
    ///     The <see cref="Core.Item" /> Enum to get the associated Enum of.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If there is a <see cref="Core.Item" /> Enum that is not also present in
    ///     the <see langword="switch" /> here.
    /// </exception>
    private Type GetAssociatedSubEnum(Core.Item item) =>
        item switch
        {
            Core.Item.Item          => typeof(ItemType),
            Core.Item.StatPotion    => typeof(StatPotionType),
            Core.Item.ManaPotion    => typeof(ManaPotionType),
            Core.Item.HealingPotion => typeof(HealingPotionType),
            _ => throw new ArgumentOutOfRangeException(nameof(item),
                "Core.ItemUsage.Item has an enum value not handled " +
                "in Services.Inventory.GetAssociatedSubEnum()"),
        };

    #endregion

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

    #region Stat Potion-specific .Where() Methods

    private static bool IsStatPotionStr(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param is (uint)BaseParamEnum.Strength);

    private static bool IsStatPotionDex(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param is (uint)BaseParamEnum.Dexterity);

    private static bool IsStatPotionVit(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param is (uint)BaseParamEnum.Vitality);

    private static bool IsStatPotionInt(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param is (uint)BaseParamEnum.Intelligence);

    private static bool IsStatPotionMnd(Item item) =>
        (item.BaseParams() ?? []).Any(param =>
            param is (uint)BaseParamEnum.Mind);

    #endregion

    private static bool IsHPPotion(Item item) =>
        (item.BaseParams() ?? []).Any(param => param == (uint)BaseParamEnum.HP);

    private static bool IsMPPotion(Item item) =>
        (item.BaseParams() ?? []).Any(param => param == (uint)BaseParamEnum.MP);

    #endregion
}

#region Static Data

internal enum ItemStatus
{
    WellFed   = 48,
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

        var row         = item.FoodRow(hq);
        var foundParams = row?.Params.Select(x => x.BaseParam.RowId).ToArray();
        SavedBaseParams[itemID] = foundParams;

        return foundParams;
    }

    internal static uint HQ(this uint itemID) =>
        itemID + 1_000_000;

    internal static uint NQ(this uint itemID) =>
        itemID - 1_000_000;

    #region Static Data

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private enum DataKeys
    {
        Status         = 0,
        ItemFoodRowId  = 1,
        StatusDuration = 2,
    }

    #endregion
}