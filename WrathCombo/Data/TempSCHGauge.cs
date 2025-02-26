using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using System;
using System.Runtime.InteropServices;

namespace WrathCombo.Data;

public unsafe class TmpSCHGauge
{
    public byte Aetherflow => Struct->Aetherflow;

    public byte FairyGauge => Struct->FairyGauge;

    public short SeraphTimer => Struct->SeraphTimer;

    public DismissedFairy DismissedFairy => (DismissedFairy)Struct->DismissedFairy;

    private protected TmpScholarGauge* Struct;

    public TmpSCHGauge()
    {
        Struct = (TmpScholarGauge*)Svc.Gauges.Get<SCHGauge>().Address;
    }
}

public unsafe class TmpPCTGauge
{
    public byte PalleteGauge => Struct->PalleteGauge;

    public byte Paint => Struct->Paint;

    public bool CreatureMotifDrawn => Struct->CreatureMotifDrawn;

    public bool WeaponMotifDrawn => Struct->WeaponMotifDrawn;

    public bool LandscapeMotifDrawn => Struct->LandscapeMotifDrawn;

    public bool MooglePortraitReady => Struct->MooglePortraitReady;

    public bool MadeenPortraitReady => Struct->MadeenPortraitReady;

    public CreatureFlags Flags => Struct->CreatureFlags;

    private protected PictoGauge* Struct;

    public byte GetOffset(int offset)
    {
        var val = IntPtr.Add(Address, offset);
        return Marshal.ReadByte(val);
    }

    private nint Address;
    public TmpPCTGauge()
    {
        Address = Svc.SigScanner.GetStaticAddressFromSig("48 8B 3D ?? ?? ?? ?? 33 ED") + 0x8;
        Struct = (PictoGauge*)Address;
    }
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public struct TmpScholarGauge
{
    [FieldOffset(0x08)] public byte Aetherflow;
    [FieldOffset(0x09)] public byte FairyGauge;
    [FieldOffset(0x0A)] public short SeraphTimer;
    [FieldOffset(0x0C)] public byte DismissedFairy;
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public struct PictoGauge
{
    [FieldOffset(0x08)] public byte PalleteGauge;
    [FieldOffset(0x0A)] public byte Paint;
    [FieldOffset(0x0B)] public CanvasFlags CanvasFlags;
    [FieldOffset(0x0C)] public CreatureFlags CreatureFlags;

    public bool CreatureMotifDrawn => CanvasFlags.HasFlag(CanvasFlags.Pom) || CanvasFlags.HasFlag(CanvasFlags.Wing) || CanvasFlags.HasFlag(CanvasFlags.Claw) || CanvasFlags.HasFlag(CanvasFlags.Maw);
    public bool WeaponMotifDrawn => CanvasFlags.HasFlag(CanvasFlags.Weapon);
    public bool LandscapeMotifDrawn => CanvasFlags.HasFlag(CanvasFlags.Landscape);
    public bool MooglePortraitReady => CreatureFlags.HasFlag(CreatureFlags.MooglePortrait);
    public bool MadeenPortraitReady => CreatureFlags.HasFlag(CreatureFlags.MadeenPortrait);

}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public struct DebugSMNGauge
{
    [FieldOffset(0x8)] public ushort SummonTimer; // millis counting down
    [FieldOffset(0xA)] public ushort AttunementTimer; // millis counting down
    [FieldOffset(0xC)] public byte ReturnSummon; // Pet sheet (23=Carbuncle, the only option now)
    [FieldOffset(0xD)] public byte ReturnSummonGlam; // PetMirage sheet
    [FieldOffset(0xE)] public byte Attunement; // Count of "Attunement cost" resource
    [FieldOffset(0xF)] public DebugAetherFlags AetherFlags; // bitfield

    public byte AttunementCount => (byte)(Attunement >> 2);//new in 7.01,Attunement may be Bit Field
    public byte AttunementType => (byte)(Attunement & 0x3);//new in 7.01, 1 = Ifrit, 2 = Titan, 3 = Garuda
}

[Flags]
public enum DebugAetherFlags : byte
{
    None = 0,
    AetherFlow1 = 1 << 0,
    AetherFlow2 = 1 << 1,
    AetherFlow = AetherFlow1 | AetherFlow2,
    PhoenixNext = 1 << 2,
    SolarBahamutNext1 = 1 << 3,
    SolarBahamutNext2 = PhoenixNext | SolarBahamutNext1,
    IfritReady = 32,
    TitanReady = 64,
    GarudaReady = 128,
}

[Flags]
public enum CanvasFlags : byte
{
    Pom = 1,
    Wing = 2,
    Claw = 4,
    Maw = 8,
    Weapon = 16,
    Landscape = 32,
}

[Flags]
public enum CreatureFlags : byte
{
    Pom = 1,
    Wings = 2,
    Claw = 4,

    MooglePortrait = 16,
    MadeenPortrait = 32,
}
