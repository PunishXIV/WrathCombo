using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    private unsafe static Hate* HateParty => (Hate*)((IntPtr)UIState.Instance() + 0x08);
    private unsafe static Hater* HateEnemies => (Hater*)((IntPtr)UIState.Instance() + 0x110);

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
            for (int i = 0; i < HateEnemies->HaterArrayLength; i++)
            {
                var h = ((HaterInfo*)HateEnemies->HaterArray) + i;
                if (h->EntityId == 0)
                    continue;

                field.TryAdd(h->EntityId, h->Enmity);
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

[StructLayout(LayoutKind.Explicit, Size = 0x908)]
public unsafe struct Hater
{
    [FieldOffset(0x00)] public fixed byte HaterArray[0x48 * 32];
    [FieldOffset(0x900)] public int HaterArrayLength;

    public ReadOnlySpan<HaterInfo> HaterSpan
    {
        get
        {
            fixed (byte* ptr = HaterArray)
            {
                return new ReadOnlySpan<HaterInfo>(ptr, HaterArrayLength);
            }
        }
    }
}