using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System.Linq;
namespace WrathCombo.Services;

internal unsafe static class BlueMageService
{
    public static void PopulateBLUSpells()
    {
        var prevList = Service.Configuration.ActiveBLUSpells.ToList();
        Service.Configuration.ActiveBLUSpells.Clear();

        for (int i = 0; i < 24; i++)
        {
            var id = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
            if (id != 0)
                Service.Configuration.ActiveBLUSpells.Add(id);
        }

        if (Service.Configuration.ActiveBLUSpells.Except(prevList).Any())
            Service.Configuration.Save();
    }

    public static bool HasFreeSpellSlot()
    {
        for (int i = 0; i < 24; i++)
        {
            var id = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
            if (id == 0)
                return true;
        }
        return false;
    }

    public static int GetFreeSpellSlot()
    {
        for (int i = 0; i < 24; i++)
        {
            var id = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
            if (id == 0)
                return i;
        }
        return -1;
    }

    public static void AssignSpell(uint actionId)
    {
        if (!HasFreeSpellSlot())
            return;

        var slot = GetFreeSpellSlot();
        if (slot == -1)
            return;
        
        ActionManager.Instance()->AssignBlueMageActionToSlot(slot, actionId);
    }

    public static int GetBLUIndex(uint id)
    {
        var aozKey = Svc.Data.GetExcelSheet<AozAction>()!.First(x => x.Action.RowId == id).RowId;
        var index = Svc.Data.GetExcelSheet<AozActionTransient>().GetRow(aozKey).Number;

        return index;
    }

    public static bool SpellUnlocked(uint actionId)
    {
        var action = Svc.Data.GetExcelSheet<AozAction>()!.First(x => x.Action.RowId == actionId).Action.Value;
        return UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(action.UnlockLink.RowId);
    }
}