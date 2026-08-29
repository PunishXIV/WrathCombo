using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.Sheets;
using System.Linq;
using WrathCombo.Native;
using Contents = ECommons.GameHelpers.Content;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static FFXIVClientStructs.FFXIV.Client.Game.InstanceContent.InstanceContentDeepDungeon;

namespace WrathCombo.Combos.PvE.Content.DeepDungeons;

internal static partial class DeepDungeons
{
    public const uint
        PoTDSustainingPotion = 20309, //Item, not an action, PoTD
        HoHEmpyreanPotion = 23163, //Item, not an action, HoH
        OrthosPotion = 38944, //Item, not an action, Eureka Orthos
        PilgrimsPotion = 47102; //Item, not an action, Pilgrim's Traverse


    public static class Buffs
    {
        public const uint
            Rehabilitation = 648,
            DamageUp = 687;
    }

    public static class Debuffs
    {
        public const uint
            ItemPenalty = 1094;
    }

    public enum Pomanders
    {
        PomanderOfSafety = 1,
        PomanderOfSight = 2,
        PomanderOfStrength = 3,
        PomanderOfSteel = 4,
        PomanderOfAffluence = 5,
        PomanderOfFlight = 6,
        PomanderOfAlteration = 7,
        PomanderOfPurity = 8,
        PomanderOfFortune = 9,
        PomanderOfWitching = 10,
        PomanderOfSerenity = 11,
        PomanderOfRage = 12,
        PomanderOfLust = 13,
        PomanderOfIntuition = 14,
        PomanderOfRaising = 15,
        PomanderOfResolution = 16,
        PomanderOfFrailty = 17,
        PomanderOfConcealment = 18,
        PomanderOfPetrification = 19,
        ProtomanderOfLethargy = 20,
        ProtomanderOfStorms = 21,
        ProtomanderOfDread = 22,
        ProtomanderOfSafety = 23,
        ProtomanderOfSight = 24,
        ProtomanderOfStrength = 25,
        ProtomanderOfSteel = 26,
        ProtomanderOfAffluence = 27,
        ProtomanderOfFlight = 28,
        ProtomanderOfAlteration = 29,
        ProtomanderOfPurity = 30,
        ProtomanderOfFortune = 31,
        ProtomanderOfWitching = 32,
        ProtomanderOfSerenity = 33,
        ProtomanderOfIntuition = 34,
        ProtomanderOfRaising = 35,
        PomanderOfHaste = 36,
        PomanderOfPurification = 37,
        PomanderOfDevotion = 38,

    }

    public unsafe static uint UsePomander(Pomanders pomander)
    {
        var dd = EventFramework.Instance()->GetInstanceContentDeepDungeon();
        if (dd == null) return 0;
        var deepDungeonSheet = Svc.Data.GetExcelSheet<DeepDungeon>().GetRow(dd->DeepDungeonId);
        if (deepDungeonSheet.PomanderSlot.TryGetFirst(x => x.RowId == (uint)pomander, out var item))
        {
            var count = PomanderCount(pomander);
            if (count == 0)
                return 0;

            SetPomander(item.Value);
            return All.Pomanders + item.Value.RowId;
        }

        return 0;
    }

    public unsafe static int PomanderCount(Pomanders pomander)
    {
        var dd = EventFramework.Instance()->GetInstanceContentDeepDungeon();
        if (dd == null) return 0;
        var deepDungeonSheet = Svc.Data.GetExcelSheet<DeepDungeon>().GetRow(dd->DeepDungeonId);
        if (deepDungeonSheet.PomanderSlot.TryGetFirst(x => x.RowId == (uint)pomander, out var item))
        {
            var count = dd->Items.ToArray().FirstOrDefault(x => x.ItemId == item.Value.RowId).Count;
            return count;
        }
        return 0;
    }

    private static void SetPomander(DeepDungeonItem item)
    {
        if (!P.CustomActions.Manager.Actions.Any(x => x.Id == All.Pomanders + item.RowId))
        {
            var act = new CustomAction(All.Pomanders + item.RowId, item.Name.ToString(), item.Tooltip.ToString(), item.Icon);
            act.OnClick = () => UsePomander(act);
            P.CustomActions.Manager.Register(act);
        }
    }

    /// <summary>
    /// Only for use by the custom action, please use the other version of this method in combos which passes an item ID.
    /// </summary>
    /// <param name="act"></param>
    internal unsafe static void UsePomander(CustomAction act)
    {
        var dd = EventFramework.Instance()->GetInstanceContentDeepDungeon();
        if (dd == null) return;

        if (Player.AnimationLock != 0) return;
        uint slot = (uint)dd->Items.ToArray().IndexOf(x => x.ItemId == act.Id - All.Pomanders);
        Svc.Log.Debug($"Using Pomander {act.Id} in slot {slot}");
        dd->UsePomander(slot);

    }

    internal unsafe static DeepDungeonItemInfo GetDDItemInfo(Pomanders pomander)
    {
        var dd = EventFramework.Instance()->GetInstanceContentDeepDungeon();
        if (dd == null) return default;
        var deepDungeonSheet = Svc.Data.GetExcelSheet<DeepDungeon>().GetRow(dd->DeepDungeonId);
        if (deepDungeonSheet.PomanderSlot.TryGetFirst(x => x.RowId == (uint)pomander, out var item))
        {
            return dd->Items.ToArray().FirstOrDefault(x => x.ItemId == item.Value.RowId);
        }
        return default;
    }

    internal static bool PomanderReady(Pomanders pomander) => PomanderCount(pomander) > 0 && GetDDItemInfo(pomander).IsUsable;
}



