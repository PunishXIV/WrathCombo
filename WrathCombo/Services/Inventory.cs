#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Game;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ExcelServices.Enums;
using ECommons.GameHelpers;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using WrathCombo.Core;
using Action = System.Action;
using Item = Lumina.Excel.Sheets.Item;

#endregion

namespace WrathCombo.Services;

public class Inventory : IDisposable
{
    /// The
    /// <see cref="Item" />
    /// sheet, limited to items we are set up to use.
    private readonly Dictionary<uint, Item> _itemSheet =
        Svc.Data.GetExcelSheet<Item>(ClientLanguage.English)
            .Where(IsItemWeCareAbout)
            .ToDictionary(i => i.RowId);

    /// An
    /// <see cref="InventoryManager" />
    /// Instance.
    private readonly unsafe InventoryManager* _manager =
        InventoryManager.Instance();

    /// <summary>
    ///     All the user's items in their inventory that we are set up to use,
    ///     sorted under <see cref="Core.Item" /> and then the enum for that type
    ///     (<see cref="ItemType" />, <see cref="StatPotionType" />, etc.)
    /// </summary>
    /// <remarks>
    ///     WARNING: This does contain HQ and NQ item IDs.
    /// </remarks>
    private readonly Dictionary<Core.Item, Dictionary<int, uint[]>>
        _usersItems = [];

    /// <summary>
    ///     Builds out the <see cref="_usersItems" /> Data Structure,
    ///     namely the Enum keys:<br />
    ///     <see cref="Core.Item" /> and then the enum for that type
    ///     (<see cref="ItemType" />, <see cref="StatPotionType" />, etc.)
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
            ", quantity pot: " + _manager->GetInventoryItemCount(38956u.HQ(), true)
        );
        PluginLog.Debug(
            "[InventoryService] Loaded User Inventory: " +
            JsonSerializer.Serialize(_usersItems,
                new JsonSerializerOptions { WriteIndented = true })
        );
        if (Svc.Data.GetExcelSheet<Item>().TryGetRow(23168u, out var item))
            PluginLog.Debug(
                "[InventoryService] super ether Info: " +
                $"cooldown remaining: {item.CooldownRemaining(false)}, " +
                $"is mp pot: {IsMPPotion(item)}, " +
                $"is med(false): {IsMedicine(item, false)}, " +
                $"is med(true): {IsMedicine(item, true)}, " +
                $"baseparams<IDs>: {string.Join(',', item.BaseParams<IDs>())}, " +
                $"valid action: {item.ItemAction.IsValid}, " +
                $"action Type: {item.ItemAction.Value.Action.Value.RowId}"
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
    ///     <see cref="GetAssociatedSubEnum" /> and also
    ///     <see cref="GetAssociatedAssociatedWhereMethod" />.
    /// </exception>
    private unsafe bool FillUserInventory()
    {
        try
        {
            if (!Player.Available)
            {
                PluginLog.Verbose("[InventoryService] [FillUserInventory] " +
                                  "Player not available");
                return false;
            }

            if (!_manager->Inventories->IsLoaded)
            {
                PluginLog.Verbose("[InventoryService] [FillUserInventory] " +
                                  "Inventory not available");
                return false;
            }

            // Each over the categories of items we care about
            foreach (var typeOfItem in Enum.GetValues<Core.Item>())
            {
                // Get the associated Sub-Enum
                var enumToFill = GetAssociatedSubEnum(typeOfItem);

                // Each over every subtype of item we care about
                foreach (var itemType in Enum.GetValues(enumToFill))
                {
                    List<uint> foundItems = [];
                    // Get a method to restrict the item sheet just to this subtype
                    var whereFunc =
                        GetAssociatedAssociatedWhereMethod(enumToFill.ToString(),
                            (int)itemType);
                    var itemsToLookFor = _itemSheet
                        .Select(x => x.Value).Where(whereFunc)
                        .Select(x => x.RowId).ToArray();

                    // Check each item from the sheet for presence in inventory
                    foreach (var item in itemsToLookFor)
                    {
                        var nq = _manager->GetInventoryItemCount(item);
                        if (nq > 0)
                            foundItems.Add(item);
                        var hq = _manager->GetInventoryItemCount(item.HQ(), true);
                        if (hq > 0)
                            foundItems.Add(item.HQ());
                    }

                    // Order those items found by how much stat they give
                    foundItems = foundItems
                        .OrderByDescending(x =>
                            x.BaseParams<Maxes>().FirstOrNull() ?? 0)
                        .ToList();

                    _usersItems[typeOfItem][(int)itemType] = foundItems.ToArray();
                }
            }

            PluginLog.Verbose("[InventoryService] [FillUserInventory] " +
                              "Inventory filled");
            DebugInventory();
            return true;
        }
        catch (Exception ex)
        {
            PluginLog.Error("[InventoryService] [FillUserInventory] " +
                            "Failed with error:\n" +
                            ex.ToStringFull());
            return false;
        }
    }

    #region Actions (for refreshing inventory)

    private int _inventoryFillAttempts;

    /// <summary>
    ///     Tries to <see cref="FillUserInventory">Load the User's Inventory</see>
    ///     on-tick, but across up to 20 attempts on different
    ///     ticks based on numerous bails and fail outs.
    /// </summary>
    /// <remarks>
    ///     Can be stopped by <see cref="_cancelChecks">Cancelling Checks</see>.
    /// </remarks>
    private readonly unsafe Action _tryFillInventory = () =>
    {
        // Bail if cancelled
        if (Service.Inventory._cancelChecks)
            return;

        // Check that our requirements are loaded
        if (!Player.Available)
        {
            PluginLog.Verbose("[InventoryService] [RefreshInventory] " +
                              "Waiting for player object ...");
            Svc.Framework.RunOnTick(Service.Inventory._tryFillInventory,
                TimeSpan.FromSeconds(1));
            return;
        }

        if (!Service.Inventory._manager->Inventories->IsLoaded)
        {
            PluginLog.Verbose("[InventoryService] [RefreshInventory] " +
                              "Waiting for inventory to load ...");
            Svc.Framework.RunOnTick(Service.Inventory._tryFillInventory,
                TimeSpan.FromSeconds(1));
            return;
        }

        // Fail out if too many attempts
        if (Service.Inventory._inventoryFillAttempts > 20)
        {
            PluginLog.Warning("[InventoryService] [RefreshInventory] " +
                              "Failed to load Inventory");
            return;
        }

        // Try to load the inventory
        PluginLog.Verbose("[InventoryService] [RefreshInventory] " +
                          "Trying to fill inventory ...");
        Service.Inventory._inventoryFillAttempts++;
        if (!Service.Inventory.FillUserInventory())
            Svc.Framework.RunOnTick(Service.Inventory._tryFillInventory,
                TimeSpan.FromSeconds(1)); // Try again if it failed
        else
            PluginLog.Verbose("[InventoryService] [RefreshInventory] " +
                              "Loaded Inventory");
    };

    /// <summary>
    ///     Can be used to stop <see cref="_tryFillInventory" /> and
    ///     <see cref="RefreshInventory" />.
    /// </summary>
    private bool _cancelChecks;

    /// <summary>
    ///     This action waits for the
    ///     <see cref="GenericHelpers.IsScreenReady">
    ///         Screen to be ready
    ///     </see>
    ///     and then begins trying to freshly
    ///     <see cref="_tryFillInventory">Load the User's Inventory</see>.
    /// </summary>
    /// <remarks>
    ///     Can be stopped by <see cref="_cancelChecks">Cancelling Checks</see>.
    /// </remarks>
    public readonly Action RefreshInventory = () =>
    {
        // Bail if cancelled
        if (Service.Inventory._cancelChecks)
            return;

        // Wait (a limited amount of time) for the screen to be ready
        PluginLog.Verbose("[InventoryService] [RefreshInventory] " +
                          "Waiting for screen ...");
        byte count = 0;
        while (!GenericHelpers.IsScreenReady())
        {
            if (count > 50) return;
            count++;
            Task.Delay(400).Wait();
        }

        Svc.Framework.RunOnTick(Service.Inventory._tryFillInventory);
    };

    public void Dispose()
    {
        _cancelChecks = true;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    ///     Gets the associated <c>.Where()</c> associated with each entry in
    ///     <see cref="Core.Item" /> (and specific ones for
    ///     <see cref="StatPotionType" />).<br />
    ///     (<see cref="IsHPPotion" />, <see cref="IsStatPotionStr" />, etc.)
    /// </summary>
    /// <param name="enumToMatch">
    ///     The <see cref="Core.Item" /> Enum (as a string) to get the associated
    ///     method for.
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
        GetAssociatedAssociatedWhereMethod(string enumToMatch, int pot) =>
        enumToMatch.Split('.').Last() switch
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
                "in Services.Inventory.GetAssociatedWhereMethod(): " +
                $"Got {enumToMatch.Split('.').Last()} ({pot})"),
        };

    /// <summary>
    ///     Gets the associated &gt;Item&lt;Type Enum associated with each entry in
    ///     <see cref="Core.Item" />.<br />
    ///     (<see cref="ItemType" />, <see cref="StatPotionType" />, etc.)
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
        item.ItemAction is
            { IsValid: true, RowId: (uint)ItemActionKnownRowID.PhoenixDown };

    private static bool IsMedicine(Item item, bool checkGivesMedicated) =>
        item.ItemUICategory.RowId == (uint)ItemUICategoryEnum.Medicine &&
        item.ItemAction.IsValid &&
        (!checkGivesMedicated || item.GivesStatus(ItemStatus.Medicated));

    private static bool IsItemWeCareAbout(Item item) =>
        IsPhoenixDown(item) ||
        (IsMedicine(item, false) &&
         (IsStatPotion(item) ||
          IsHPPotion(item) ||
          IsMPPotion(item)));

    private static bool IsStatPotion(Item item) =>
        IsMedicine(item, true) &&
        item.BaseParams<IDs>().Any(param =>
            param is (uint)BaseParamEnum.Strength or
                (uint)BaseParamEnum.Dexterity or
                (uint)BaseParamEnum.Vitality or
                (uint)BaseParamEnum.Intelligence or
                (uint)BaseParamEnum.Mind);

    #region Stat Potion-specific .Where() Methods

    private static bool IsStatPotionStr(Item item) =>
        item.BaseParams<IDs>().Any(param =>
            param is (uint)BaseParamEnum.Strength);

    private static bool IsStatPotionDex(Item item) =>
        item.BaseParams<IDs>().Any(param =>
            param is (uint)BaseParamEnum.Dexterity);

    private static bool IsStatPotionVit(Item item) =>
        item.BaseParams<IDs>().Any(param =>
            param is (uint)BaseParamEnum.Vitality);

    private static bool IsStatPotionInt(Item item) =>
        item.BaseParams<IDs>().Any(param =>
            param is (uint)BaseParamEnum.Intelligence);

    private static bool IsStatPotionMnd(Item item) =>
        item.BaseParams<IDs>().Any(param =>
            param is (uint)BaseParamEnum.Mind);

    #endregion

    private static bool IsHPPotion(Item item) =>
        item.BaseParams<IDs>().Any(param => param == (uint)BaseParamEnum.HP);

    private static bool IsMPPotion(Item item) =>
        item.BaseParams<IDs>().Any(param => param == (uint)BaseParamEnum.MP);

    #endregion
}

#region Static Data

/// <summary>
///     Different status IDs that can be given, according to <c>ItemAction</c> data.
/// </summary>
/// <seealso cref="ItemExtensions.GivesStatus" />
internal enum ItemStatus
{
    WellFed   = 48,
    Medicated = 49,
}

/// <summary>
///     Known <c>ItemAction</c> Row IDs.
/// </summary>
internal enum ItemActionKnownRowID
{
    PhoenixDown = 44,
}

/// <summary>
///     Known <c>ItemAction</c> <c>Type</c> values.
/// </summary>
internal enum ItemActionKnownType
{
    HP = 847,
    MP = 848,
}

#endregion

internal static class ItemExtensions
{
    /// <summary>
    ///     Caching for items' <c>BaseParams</c> (including derived ones,
    ///     for HP/MP).
    /// </summary>
    /// <value>
    ///     <see cref="BaseParamKey" /> then <see cref="BaseParamSubKey" /><br />
    ///     (ID/Max/Value then NQ/HQ)
    /// </value>
    private static readonly
        Dictionary<uint,
            Dictionary<BaseParamKey,
                Dictionary<BaseParamSubKey,
                    uint[]>>>
        SavedBaseParams = [];

    /// <summary>
    ///     Gets the <see cref="Item" /> row and uses it to call
    ///     <see cref="BaseParams{T}(Item, bool?)" />.
    /// </summary>
    /// <param name="itemID">
    ///     (Will be corrected to the NQ version, if HQ)
    /// </param>
    /// <param name="hq">
    ///     (Not necessary, will be set by checking the <paramref name="itemID" />
    ///     provided)
    /// </param>
    /// <seealso cref="BaseParams{T}(Item, bool?)" />
    internal static uint[] BaseParams<T>(this uint itemID, bool? hq = null)
        where T : IBaseParamTypeToGet =>
        Svc.Data.GetExcelSheet<Item>().TryGetRow(itemID.SafeNQ(), out var item)
            ? item.BaseParams<T>(hq ?? itemID.IsHQ())
            : [];

    /// <summary>
    ///     Get the actual stat that gets boosted, and by how much, for a food item.
    /// </summary>
    /// <param name="item">
    ///     The <see cref="Item" /> to get the BaseParams for.
    /// </param>
    /// <param name="hq">
    ///     Whether the High Quality Stats should be returned.
    /// </param>
    /// <typeparam name="T">
    ///     The <see cref="IBaseParamTypeToGet" />-implementing class to specify
    ///     the type of stats to return.<br />
    ///     (<see cref="IDs" />, <see cref="Maxes" />, etc.)
    /// </typeparam>
    /// <returns>
    ///     An Array of the values requested.<br />
    ///     You can just use
    ///     <see
    ///         cref="System.Linq.Enumerable.First{TSource}(System.Collections.Generic.IEnumerable{TSource})">
    ///         Linq.Enumerable.First()
    ///     </see>
    ///     for things other than <see cref="IDs" /> (and even then, you can for
    ///     pots, but not food).
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If the <typeparamref name="T" /> used was not a
    ///     <see cref="IBaseParamTypeToGet" />-implementing class handled in the
    ///     <see langword="switch" /> here.
    /// </exception>
    internal static uint[] BaseParams<T>(this Item item, bool? hq = null)
        where T : IBaseParamTypeToGet
    {
        var mainKey = typeof(T) switch
        {
            var t when t == typeof(IDs)    => BaseParamKey.IDs,
            var t when t == typeof(Maxes)  => BaseParamKey.Maxes,
            var t when t == typeof(Values) => BaseParamKey.Values,
            _ => throw new ArgumentOutOfRangeException(nameof(T),
                "Services.Inventory.ItemExtensions.BaseParams() was called with " +
                "a Type it does not support. Use a class implementing " +
                "`IBaseParamTypeToGet`."),
        };
        var subKey = hq == true ? BaseParamSubKey.HQ : BaseParamSubKey.NQ;

        // Return cached values
        if (SavedBaseParams.TryGetValue(item.RowId, out var savedParams))
            return savedParams[mainKey][subKey];

        var row       = item.FoodRow(hq);
        var actionRow = item.ActionRow();

        var ids     = Extract(p => p.BaseParam.RowId);
        var maxNQ   = Extract(p => (uint)p.Max);
        var maxHQ   = Extract(p => (uint)p.MaxHQ);
        var valueNQ = Extract(p => (uint)p.Value);
        var valueHQ = Extract(p => (uint)p.ValueHQ);

        #region Populate Params for HP/MP Potions

        if (row is null && actionRow is not null)
        {
            if (actionRow.Value.Action.RowId == (ushort)ItemActionKnownType.HP)
            {
                ids     = [(uint)BaseParamEnum.HP];
                maxNQ   = [actionRow.Value.Data[(int)OtherDataKeys.Amount]];
                maxHQ   = [actionRow.Value.DataHQ[(int)OtherDataKeys.Amount]];
                valueNQ = [actionRow.Value.Data[(int)OtherDataKeys.PercentageIfHP]];
                valueHQ =
                    [actionRow.Value.DataHQ[(int)OtherDataKeys.PercentageIfHP]];
            }

            if (actionRow.Value.Action.RowId == (ushort)ItemActionKnownType.MP)
            {
                ids   = [(uint)BaseParamEnum.MP];
                maxNQ = [actionRow.Value.Data[(int)OtherDataKeys.Amount]];
                maxHQ = [actionRow.Value.DataHQ[(int)OtherDataKeys.Amount]];
            }
        }

        #endregion

        var foundData =
            new Dictionary<BaseParamKey, Dictionary<BaseParamSubKey, uint[]>>
            {
                [BaseParamKey.IDs] = new()
                {
                    [BaseParamSubKey.NQ] = ids ?? [],
                    [BaseParamSubKey.HQ] = ids ?? [],
                },

                [BaseParamKey.Maxes] = new()
                {
                    [BaseParamSubKey.NQ] = maxNQ ?? [],
                    [BaseParamSubKey.HQ] = maxHQ ?? [],
                },

                [BaseParamKey.Values] = new()
                {
                    [BaseParamSubKey.NQ] = valueNQ ?? [],
                    [BaseParamSubKey.HQ] = valueHQ ?? [],
                },
            };

        SavedBaseParams[item.RowId] = foundData;

        return foundData[mainKey][subKey];

        #region Food Row LINQ Helper

        uint[]? Extract(Func<ItemFood.ParamsStruct, uint> selector) =>
            row?.Params
                .Select(selector)
                .Where(v => v != 0)
                .ToArray();

        #endregion
    }

    extension(Item item)
    {
        /// <summary>
        ///     Check if an item will provide the given <see cref="ItemStatus" />.
        /// </summary>
        internal bool GivesStatus(ItemStatus status) =>
            item.ItemAction.IsValid &&
            (item.ItemAction.Value.Data[(int)DataKeys.Status] == (ushort)status ||
             item.ItemAction.Value.DataHQ[(int)DataKeys.Status] == (ushort)status);

        /// <summary>
        ///     Get the associated <see cref="ItemAction" /> Row for a given item.
        /// </summary>
        internal ItemAction? ActionRow
            ()
        {
            if (!item.ItemAction.IsValid)
                return null;

            if (Svc.Data.GetExcelSheet<ItemAction>()
                .TryGetRow(item.ItemAction.Value.RowId, out var row))
                return row;

            return null;
        }

        /// <summary>
        ///     Get the associated <see cref="ItemFood" /> Row for a given item.
        /// </summary>
        /// <param name="hq">
        ///     Whether to get the <see cref="ItemFood" /> Row associated with the
        ///     normal or high quality version of the item.<br />
        ///     Only necessary to provide if looking specifically for the high quality
        ///     stats; defaults to normal quality, if that is found.
        /// </param>
        internal ItemFood? FoodRow(bool? hq = null)
        {
            if (!item.ItemAction.IsValid)
                return null;

            if (hq is null or false &&
                Svc.Data.GetExcelSheet<ItemFood>()
                    .TryGetRow(
                        item.ItemAction.Value.Data[(int)DataKeys.ItemFoodRowId],
                        out var row))
                return row;

            if (hq is null or true &&
                Svc.Data.GetExcelSheet<ItemFood>()
                    .TryGetRow(
                        item.ItemAction.Value.DataHQ[(int)DataKeys.ItemFoodRowId],
                        out var rowHQ))
                return rowHQ;

            return null;
        }
        
        private unsafe float CooldownTotal(bool? hq = null) =>
            ActionManager.Instance()->GetRecastTime(ActionType.Item,
                hq is true ? item.RowId.SafeHQ() : item.RowId.SafeNQ());
        
        private unsafe float CooldownElapsed(bool? hq = null) =>
            ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item,
                hq is true ? item.RowId.SafeHQ() : item.RowId.SafeNQ());
        
        public float CooldownRemaining(bool? hq = null) =>
            Math.Max(0, item.CooldownTotal(hq) - item.CooldownElapsed(hq));
    }

    extension(uint itemID)
    {
        /// <summary>
        ///     Checks if a given Item ID is High-Quality or not.
        /// </summary>
        internal bool IsHQ() =>
            itemID > 1_000_000;

        /// <summary>
        ///     Converts a given Item ID to its High-Quality variant.
        /// </summary>
        internal uint HQ() =>
            itemID + 1_000_000;

        /// <summary>
        ///     Converts a given Item ID to its Normal-Quality variant.
        /// </summary>
        internal uint NQ() =>
            itemID - 1_000_000;

        /// <summary>
        ///     Converts a given Item ID to its High-Quality variant.<br />
        ///     (If it is not already)
        /// </summary>
        internal uint SafeHQ() =>
            itemID.IsHQ() ? itemID : itemID.HQ();

        /// <summary>
        ///     Converts a given Item ID to its Normal-Quality variant.<br />
        ///     (If it is not already)
        /// </summary>
        internal uint SafeNQ() =>
            itemID.IsHQ() ? itemID.NQ() : itemID;
    }

    #region Static Data

    /// <summary>
    ///     Keys for navigating the <c>Data</c> values directly on <c>ItemAction</c>
    ///     rows, for (most) items (at least food and stat pots).
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private enum DataKeys
    {
        /// <summary>
        ///     This key is used to access the status (<see cref="ItemStatus" />).
        /// </summary>
        Status = 0,

        /// <summary>
        ///     This key is used to access the Row ID for the associated
        ///     <c>ItemFood</c> row (<see cref="FoodRow" />).
        /// </summary>
        ItemFoodRowId = 1,

        /// <summary>
        ///     This key is used to access the duration of the <see cref="Status" />.
        /// </summary>
        StatusDuration = 2,
    }

    /// <summary>
    ///     Keys for navigating the <c>Data</c> values directly on <c>ItemAction</c>
    ///     rows, for HP and MP pot items.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private enum OtherDataKeys
    {
        /// <summary>
        ///     This key is used to access the percentage stat gain
        ///     (<see cref="BaseParamKey.Values" />) for HP pot items.
        /// </summary>
        PercentageIfHP = 0,

        /// <summary>
        ///     This key is used to access the actual maximum amount of stat gain
        ///     (<see cref="BaseParamKey.Maxes" />) for HP <i>and</i> MP pot items.
        /// </summary>
        Amount = 1,
    }

    #endregion
}

#region Base Params call Classes

/// <summary>
///     An interface for empty classes meant to map to <see cref="BaseParamKey" />.
/// </summary>
/// <seealso cref="IDs" />
/// <seealso cref="Maxes" />
/// <seealso cref="Values" />
internal interface IBaseParamTypeToGet
{
}

/// <seealso cref="BaseParamKey.IDs" />
// ReSharper disable once InconsistentNaming
internal class IDs : IBaseParamTypeToGet
{
}

/// <seealso cref="BaseParamKey.Maxes" />
internal class Maxes : IBaseParamTypeToGet
{
}

/// <seealso cref="BaseParamKey.Values" />
internal class Values : IBaseParamTypeToGet
{
}

#endregion

#region Saved Params Keys

/// <summary>
///     The primary key for <see cref="ItemExtensions.SavedBaseParams" />.
/// </summary>
internal enum BaseParamKey
{
    // ReSharper disable once InconsistentNaming
    /// The stat IDs that can be provided.
    IDs = 0,

    /// The maximum stat this can provide.
    Maxes = 1,

    /// The amount that will be given, up to the maximum. Normally a percentage.
    Values = 2,
}

/// <summary>
///     The secondary key for <see cref="ItemExtensions.SavedBaseParams" />.
/// </summary>
internal enum BaseParamSubKey
{
    NQ = 0,
    HQ = 1,
}

#endregion