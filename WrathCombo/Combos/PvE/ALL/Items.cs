using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Native;
using Item = Lumina.Excel.Sheets.Item;
using ItemFood = Lumina.Excel.Sheets.ItemFood;

namespace WrathCombo.Combos.PvE.ALL
{
    internal class Items
    {
        /// <summary>
        /// Only for use by the custom action, please use the other version of this method in combos which passes an item ID.
        /// </summary>
        /// <param name="act"></param>
        internal unsafe static void UseItem(CustomAction act)
        {
            var hq = InventoryManager.Instance()->GetInventoryItemCount(act.ItemId, true) > 0;
            var id = hq ? act.ItemId + 1_000_000 : act.ItemId;

            if (id > 0 && ActionManager.Instance()->GetActionStatus(ActionType.Item, id) == 0)
            {
                ActionManager.Instance()->UseAction(ActionType.Item, id, extraParam: 65535);
            }
        }

        /// <summary>
        /// Returns a custom action which acts as a proxy for using items.
        /// </summary>
        /// <param name="item">The item ID of the item to be used.</param>
        /// <returns></returns>
        public unsafe static uint UseItem(uint item)
        {
            if (item == All.Items)
                return All.Items;

            if (Svc.Data.GetExcelSheet<Item>().TryGetRow(item, out var row))
            {
                SetItem(row);
                return All.Items + item;
            }

            return 0;
        }

        public unsafe static bool ItemReady(uint itemId)
        {
            var res = ActionManager.Instance()->GetActionStatus(ActionType.Item, itemId, checkCastingActive: false);
            var res2 = ActionManager.Instance()->GetActionStatus(ActionType.Item, itemId + 1_000_000, checkCastingActive: false);
            //Svc.Log.Debug($"{res} {res2}");
            return res is 0 || res2 is 0;
        }

        /// <summary>
        /// Checks if a custom action has been created for an item, and if not creates it.
        /// </summary>
        /// <param name="item"></param>
        private static void SetItem(Item item)
        {
            if (!P.CustomActions.Manager.Actions.Any(x => x.Id == All.Items + item.RowId))
            {
                var act = new CustomAction(All.Items + item.RowId, item.Name.ToString(), item.Description.ToString(), item.Icon, itemId: item.RowId);
                act.OnClick = () => UseItem(act);
                P.CustomActions.Manager.Register(act);
            }
        }

        public unsafe static Item? GetStrongestPotion(PotionType type, bool inInventory = true)
        {
            int t = (int)type;
            return AllPots.LastOrDefault(x => GetItemConsumableProperties(x)?.Params.Any(y => y.BaseParam.RowId == t) == true &&
                                        ((InventoryManager.Instance()->GetInventoryItemCount(x.RowId) + InventoryManager.Instance()->GetInventoryItemCount(x.RowId, true) > 0) || !inInventory));
        }

        public unsafe static uint GetStrongestPotionRow(PotionType type, bool inInventory = true)
        {
            var rowId = GetStrongestPotion(type, inInventory)?.RowId ?? 0;
            return rowId == 0 ? All.Items : rowId;
        }

        internal static ItemFood? GetItemConsumableProperties(Item item)
        {
            if (!item.ItemAction.IsValid)
                return null;
            var action = item.ItemAction.Value;
            var actionParams = action.Data; // [0] = status, [1] = extra == ItemFood row, [2] = duration
            if (actionParams[0] is not 48 and not 49)
                return null; // not 'well fed' or 'medicated'
            return Svc.Data.GetExcelSheet<ItemFood>()?.GetRow(actionParams[1]);
        }

        public static List<Item> AllPots
        {
            get
            {
                field ??= [];

                if (field.Count > 0)
                    return field;

                foreach (var item in Svc.Data.GetExcelSheet<Item>())
                {
                    if (item.ItemUICategory.RowId != 44)
                        continue;

                    field.Add(item);
                }
                return field;
            }
        }

        private static readonly Dictionary<uint, List<Item>> PotsByParam = [];

        private static List<Item> GetPotsByParam(uint baseParamId)
        {
            if (!PotsByParam.TryGetValue(baseParamId, out var pots))
            {
                pots = AllPots.Where(x =>
                    GetItemConsumableProperties(x)?.Params.Any(p => p.BaseParam.RowId == baseParamId) ?? false
                ).ToList();
                PotsByParam[baseParamId] = pots;
            }
            return pots;
        }

        public static List<Item> StrengthPots
        {
            get
            {
                field ??= [];
                return field.Count == 0 ? GetPotsByParam(1) : field;
            }
        }

        public static List<Item> DexPots
        {
            get
            {
                field ??= [];
                return field.Count == 0 ? GetPotsByParam(2) : field;
            }
        }

        public static List<Item> VitPots
        {
            get
            {
                field ??= [];
                return field.Count == 0 ? GetPotsByParam(3) : field;
            }
        }

        public static List<Item> IntPots
        {
            get
            {
                field ??= [];
                return field.Count == 0 ? GetPotsByParam(4) : field;
            }
        }

        public static List<Item> MindPots
        {
            get
            {
                field ??= [];
                return field.Count == 0 ? GetPotsByParam(5) : field;
            }
        }

        public static List<Item> PietyPots
        {
            get
            {
                field ??= [];
                return field.Count == 0 ? GetPotsByParam(6) : field;
            }
        }

        public enum PotionType
        {
            Strength = 1,
            Dex = 2,
            Vit = 3,
            Int = 4,
            Mind = 5,
            Piety = 6
        }
    }
}
