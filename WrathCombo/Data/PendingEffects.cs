using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace WrathCombo.Data;

public enum ActionEffectType : byte
{
    Nothing = 0,
    Miss = 1,
    FullResist = 2,
    Damage = 3,
    Heal = 4,
    BlockedDamage = 5,
    ParriedDamage = 6,
    Invulnerable = 7,
    NoEffectText = 8,
    FailMissingStatus = 9,
    MpLoss = 10, // 0x0A
    MpGain = 11, // 0x0B
    TpLoss = 12, // 0x0C
    TpGain = 13, // 0x0D
    ApplyStatusEffectTarget = 14, // 0x0E - dissector calls this "GpGain"
    ApplyStatusEffectSource = 15, // 0x0F
    RecoveredFromStatusEffect = 16, // 0x10
    LoseStatusEffectTarget = 17, // 0x11
    LoseStatusEffectSource = 18, // 0x12
    //Unknown_13 = 19, // 0x13 - sometimes part of pvp Purify & Empyrean Rain spells, related to afflictions removal?..
    StatusNoEffect = 20, // 0x14
    ThreatPosition = 24, // 0x18 - provoke
    EnmityAmountUp = 25, // 0x19 - ? summons
    EnmityAmountDown = 26, // 0x1A
    StartActionCombo = 27, // 0x1B
    Retaliation = 28, // 0x1C - 'vengeance' has value = 7, 'arms length' has value = 0
    Knockback = 31, // 0x1F
    Attract1 = 32, // 0x20
    Attract2 = 33, // 0x21
    AttractCustom1 = 34, // 0x22
    AttractCustom2 = 35, // 0x23
    AttractCustom3 = 36, // 0x24
    Mount = 39, // 0x27
    ReviveLB = 50, // 0x32 - heal lb3 revive with full hp; seen value == 1
    FullResistStatus = 55, // 0x37 - full resist status (e.g. 9 = resist 'arms length' slow, 2 = resist 'low blow' stun)
    //Unknown_39 = 57, // 0x39 - 'you have been sentenced to death!' message
    VFX = 59, // 0x3B
    //Unknown_3D = 61, // 0x3D - was called 'gauge', but i think it's incorrect
    Resource = 62, // 0x3E - value 0x34 = gain war gauge (amount == hitSeverity)
    SetModelState = 73, // 0x49 - value == model state
    SetHP = 74, // 0x4A - e.g. zodiark's kokytos
    PartialInvulnerable = 75, // 0x4B
    Interrupt = 76, // 0x4C
}

public enum DamageType
{
    Unknown,
    Slashing,
    Piercing,
    Blunt,
    Shot,
    Magic,
    Breath,
    Physical,
    LimitBreak,
}

public enum DamageElementType
{
    Unknown,
    Fire,
    Ice,
    Air,
    Earth,
    Lightning,
    Water,
    Unaspected,
}

public enum KnockbackDirection
{
    AwayFromSource = 0, // direction = target-source
    Arg = 1, // direction = arg.degrees()
    Random = 2, // direction = random(0, 2pi)
    SourceForward = 3, // direction = src.direction
    SourceRight = 4, // direction = src.direction - pi/2
    SourceLeft = 5, // direction = src.direction + pi/2
}

public enum ActionResourceType
{
    WARGauge = 0x34,
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ActionEffect
{
    public ActionEffectType Type;
    public byte Param0;
    public byte Param1;
    public byte Param2;
    public byte Param3;
    public byte Param4;
    public ushort Value;

    public readonly bool FromTarget => (Param4 & 0x20) != 0;
    public readonly bool AtSource => (Param4 & 0x80) != 0;
    public readonly DamageType DamageType => (DamageType)(Param1 & 0x0F); // for various damage effects
    public readonly DamageElementType DamageElement => (DamageElementType)(Param1 >> 4); // for various damage effects
    public readonly int DamageHealValue => Value + ((Param4 & 0x40) != 0 ? Param3 * 0x10000 : 0); // for damage/heal effects
}

// TODO: convert to inline array
public unsafe struct ActionEffects : IEnumerable<ActionEffect>
{
    public const int MaxCount = 8;

    private fixed ulong _effects[MaxCount];

    public ulong this[int index]
    {
        get => _effects[index];
        set => _effects[index] = value;
    }

    public IEnumerator<ActionEffect> GetEnumerator()
    {
        for (int i = 0; i < 8; ++i)
        {
            var eff = Build(i);
            if (eff.Type != ActionEffectType.Nothing)
                yield return eff;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private unsafe ActionEffect Build(int index)
    {
        fixed (ulong* p = _effects)
            return *(ActionEffect*)(p + index);
    }
}