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

public unsafe class TmpBLMGauge
{

    public TmpBLMGauge()
    {
        Address = Svc.SigScanner.GetStaticAddressFromSig("48 8B 3D ?? ?? ?? ?? 33 ED") + 0x8;
        Struct = (DebugBLMGauge*)Address;
    }

    private protected DebugBLMGauge* Struct;
    private nint Address;

    /// <summary>
    /// Gets the time remaining for the Enochian time in milliseconds.
    /// </summary>
    public short EnochianTimer => this.Struct->EnochianTimer;

    /// <summary>
    /// Gets the number of Polyglot stacks remaining.
    /// </summary>
    public byte PolyglotStacks => this.Struct->PolyglotStacks;

    /// <summary>
    /// Gets the number of Umbral Hearts remaining.
    /// </summary>
    public int UmbralHearts => this.Struct->UmbralHearts;

    /// <summary>
    /// Gets the amount of Umbral Ice stacks.
    /// </summary>
    public int UmbralIceStacks => this.Struct->UmbralStacks;

    /// <summary>
    /// Gets the amount of Astral Fire stacks.
    /// </summary>
    public int AstralFireStacks => this.Struct->AstralStacks;

    /// <summary>
    /// Gets the amount of Astral Soul stacks.
    /// </summary>
    public int AstralSoulStacks => this.Struct->AstralSoulStacks;

    /// <summary>
    /// Gets a value indicating whether or not the player is in Umbral Ice.
    /// </summary>
    /// <returns><c>true</c> or <c>false</c>.</returns>
    public bool InUmbralIce => this.Struct->UmbralStacks > 0;

    /// <summary>
    /// Gets a value indicating whether or not the player is in Astral fire.
    /// </summary>
    /// <returns><c>true</c> or <c>false</c>.</returns>
    public bool InAstralFire => this.Struct->AstralStacks > 0;

    /// <summary>
    /// Gets a value indicating whether or not Enochian is active.
    /// </summary>
    /// <returns><c>true</c> or <c>false</c>.</returns>
    public bool IsEnochianActive => this.Struct->EnochianActive;

    /// <summary>
    /// Gets a value indicating whether Paradox is active.
    /// </summary>
    /// <returns><c>true</c> or <c>false</c>.</returns>
    public bool IsParadoxActive => this.Struct->ParadoxActive;
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

[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public struct DebugBLMGauge
{
    [FieldOffset(0x08)] public short EnochianTimer;
    [FieldOffset(0x0A)] public sbyte ElementStance;
    [FieldOffset(0x0B)] public byte UmbralHearts;
    [FieldOffset(0x0C)] public byte PolyglotStacks;
    [FieldOffset(0x0D)] public EnochianFlags EnochianFlags;

    public int UmbralStacks => ElementStance >= 0 ? 0 : ElementStance * -1;
    public int AstralStacks => ElementStance <= 0 ? 0 : ElementStance;
    public bool EnochianActive => EnochianFlags.HasFlag(EnochianFlags.Enochian);
    public bool ParadoxActive => EnochianFlags.HasFlag(EnochianFlags.Paradox);
    public int AstralSoulStacks => ((int)EnochianFlags >> 2) & 7;
}



[Flags]
public enum ElementalFlags : short
{
    None = 0,
    AstralFire1 = 1,
    AstralFire2 = 2,
    AstralFire3 = 3,
    UmbralIce1 = 255,
    UmbralIce2 = 254,
    UmbralIce3 = 253,
    UmbralHearts1 = 256,
    UmbralHearts2 = 512,
    UmbralHearts3 = 768,

}

[Flags]
public enum DebugEnochianFlags : byte
{
    None = 0,
    Enochian = 1,
    Paradox = 2,
    FlareStar1 = 4,
    FlareStar2 = 8,
    FlareStar3 = 12,
    FlareStar4 = 16,
    FlareStar5 = 20,
    FlareStar6 = 24,
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
