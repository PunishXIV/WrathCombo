using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    private unsafe static Hate* HateParty => (Hate*)((IntPtr)UIState.Instance() + 0x08);
    private unsafe static Hate* HateEnemies => (Hate*)((IntPtr)UIState.Instance() + 0x110);

    public unsafe static Dictionary<ulong, int>? EnmityDictParty
    {
        get
        {
            field ??= new();
            field.Clear();
            foreach (var h in HateParty->HateInfo)
            {
                if (h.EntityId == 0)
                    continue;

                field.TryAdd(h.EntityId, h.Enmity);
            }
            return field;
        }
    }

    public unsafe static Dictionary<ulong, int>? EnmityEnemies
    {
        get
        {
            field ??= new();
            field.Clear();
            foreach (var h in HateEnemies->HateInfo)
            {
                if (h.EntityId == 0)
                    continue;

                field.TryAdd(h.EntityId, h.Enmity);
            }
            return field;
        }
    }

    public static IGameObject? StrongestDPS()
    {
        foreach (var dps in EnmityDictParty?.OrderByDescending(x => x.Value))
        {
            var obj = Svc.Objects.First(x => x.GameObjectId == dps.Key) as IBattleChara;
            if (obj?.GetRole() is CombatRole.DPS)
                return obj;

        }

        return null;
    }

    public static bool PlayerHasAggro => EnmityEnemies != null && EnmityEnemies.Any(x => x.Value == 100);

}