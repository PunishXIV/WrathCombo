using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

#region Role Actions (Static)
internal static class RoleActions
{
    public static class Magic
    {
        public const uint
            LucidDreaming = 7562,
            Swiftcast = 7561,
            Surecast = 7559;

        public static class Buffs
        {
            public const ushort
                Raise = 148,
                Swiftcast = 167,
                Surecast = 160;
        }

        public static bool CanLucidDream(int MPThreshold, bool spellweave = true) =>
            ActionReady(LucidDreaming) &&
            LocalPlayer.CurrentMp <= MPThreshold &&
            (!spellweave || CanSpellWeave());

        public static bool CanSwiftcast(bool spellweave = true) =>
            ActionReady(Swiftcast) && (!spellweave || CanSpellWeave());

        public static bool CanSurecast() =>
            ActionReady(Surecast);
    }

    public static class Caster
    {
        public const uint
            Skyshard = 203,
            Starstorm = 204,
            Addle = 7560,
            Sleep = 25880;

        public static class Debuffs
        {
            public const ushort
                Addle = 1203;
        }

        public static bool CanAddle() =>
            ActionReady(Addle) && !TargetHasEffectAny(Debuffs.Addle);

        public static bool CanSleep() =>
            ActionReady(Sleep);
    }

    public static class Healer
    {
        public const uint
            HealingWind = 206,
            BreathOfTheEarth = 207,
            Repose = 16560,
            Esuna = 7568,
            Rescue = 7571;

        public static bool CanRepose() =>
            ActionReady(Repose);

        public static bool CanEsuna() =>
            ActionReady(Esuna);

        public static bool CanRescue() =>
            ActionReady(Rescue);
    }

    public static class Physical
    {
        public const uint
            SecondWind = 7541,
            ArmsLength = 7548;

        public static class Buffs
        {
            public const ushort
                ArmsLength = 1209;
        }

        public static bool CanSecondWind(int healthpercent) =>
            ActionReady(SecondWind) && PlayerHealthPercentageHp() <= healthpercent;

        public static bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
            ActionReady(ArmsLength) && CanCircleAoe(7) >= enemyCount &&
            ((int)avoidanceSetting == (int)All.Enums.BossAvoidance.Off || !InBossEncounter());
    }

    public static class PhysRanged
    {
        public const uint
            BigShot = 4238,
            Desperado = 4239,
            LegGraze = 7554,
            FootGraze = 7553,
            Peloton = 7557,
            HeadGraze = 7551;

        public static class Buffs
        {
            public const ushort
                Peloton = 1199;
        }

        public static bool CanLegGraze() =>
            ActionReady(LegGraze);

        public static bool CanFootGraze() =>
            ActionReady(FootGraze);

        public static bool CanPeloton() =>
            ActionReady(Peloton);

        public static bool CanHeadGraze(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) =>
            IsEnabled(preset) && CanInterruptEnemy() && IsOffCooldown(HeadGraze) && CheckWeave(weave);
    }

    public static class Melee
    {
        public const uint
            Braver = 200,
            Bladedance = 201,
            LegSweep = 7863,
            Bloodbath = 7542,
            Feint = 7549,
            TrueNorth = 7546;

        public static class Buffs
        {
            public const ushort
                BloodBath = 84,
                TrueNorth = 1250;
        }

        public static class Debuffs
        {
            public const ushort
                Feint = 1195;
        }

        public static bool CanLegSweep() =>
            ActionReady(LegSweep);

        public static bool CanBloodBath(int healthpercent) =>
            ActionReady(Bloodbath) && PlayerHealthPercentageHp() <= healthpercent;

        public static bool CanFeint() =>
            ActionReady(Feint) && !TargetHasEffectAny(Debuffs.Feint);

        public static bool CanTrueNorth() =>
            ActionReady(TrueNorth) && TargetNeedsPositionals() && !HasEffect(Buffs.TrueNorth);
    }

    public static class Tank
    {
        public const uint
            ShieldWall = 197,
            Stronghold = 198,
            Rampart = 7531,
            LowBlow = 7540,
            Provoke = 7533,
            Interject = 7538,
            Reprisal = 7535,
            Shirk = 7537;

        public static class Debuffs
        {
            public const ushort
                Reprisal = 1193;
        }

        public static bool CanRampart(int healthPercent) =>
            ActionReady(Rampart) && PlayerHealthPercentageHp() < healthPercent;

        public static bool CanLowBlow() =>
            ActionReady(LowBlow) && TargetIsCasting();

        public static bool CanProvoke() =>
            ActionReady(Provoke);

        public static bool CanInterject() =>
            ActionReady(Interject) && CanInterruptEnemy();

        public static bool CanReprisal(int healthPercent = 101, int? enemyCount = null, bool checkTargetForDebuff = true) =>
            (checkTargetForDebuff && !TargetHasEffectAny(Debuffs.Reprisal) || !checkTargetForDebuff) &&
            (enemyCount is null ? InActionRange(Reprisal) : CanCircleAoe(5) >= enemyCount) &&
            ActionReady(Reprisal) && PlayerHealthPercentageHp() < healthPercent;

        public static bool CanShirk() =>
            ActionReady(Shirk);
    }
}
#endregion

#region Action-Specific Interfaces
// Action-specific interfaces
internal interface ILucidDreaming
{
    uint LucidDreaming { get; }
    bool CanLucidDream(int MPThreshold, bool spellweave = true);
}

internal interface ISwiftcast
{
    uint Swiftcast { get; }
    bool CanSwiftcast(bool spellweave = true);
}

internal interface ISurecast
{
    uint Surecast { get; }
    bool CanSurecast();
}

internal interface IAddle
{
    uint Addle { get; }
    bool CanAddle();
}

internal interface ISleep
{
    uint Sleep { get; }
    bool CanSleep();
}

internal interface IRepose
{
    uint Repose { get; }
    bool CanRepose();
}

internal interface IEsuna
{
    uint Esuna { get; }
    bool CanEsuna();
}

internal interface IRescue
{
    uint Rescue { get; }
    bool CanRescue();
}

internal interface ISecondWind
{
    uint SecondWind { get; }
    bool CanSecondWind(int healthpercent);
}

internal interface IArmsLength
{
    uint ArmsLength { get; }
    bool CanArmsLength();
    bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null);
    bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting);
}

internal interface ILegGraze
{
    uint LegGraze { get; }
    bool CanLegGraze();
}

internal interface IFootGraze
{
    uint FootGraze { get; }
    bool CanFootGraze();
}

internal interface IPeloton
{
    uint Peloton { get; }
    bool CanPeloton();
}

internal interface IHeadGraze
{
    uint HeadGraze { get; }
    internal bool CanHeadGraze(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None);
}

internal interface ILegSweep
{
    uint LegSweep { get; }
    bool CanLegSweep();
}

internal interface IBloodbath
{
    uint Bloodbath { get; }
    bool CanBloodBath(int healthpercent);
}

internal interface IFeint
{
    uint Feint { get; }
    bool CanFeint();
}

internal interface ITrueNorth
{
    uint TrueNorth { get; }
    bool CanTrueNorth();
}

internal interface IRampart
{
    uint Rampart { get; }
    bool CanRampart(int healthPercent);
}

internal interface ILowBlow
{
    uint LowBlow { get; }
    bool CanLowBlow();
}

internal interface IProvoke
{
    uint Provoke { get; }
    bool CanProvoke();
}

internal interface IInterject
{
    uint Interject { get; }
    bool CanInterject();
}

internal interface IReprisal
{
    uint Reprisal { get; }
    bool CanReprisal(int healthPercent = 101, int? enemyCount = null, bool checkTargetForDebuff = true);
}

internal interface IShirk
{
    uint Shirk { get; }
    bool CanShirk();
}
#endregion

#region Status Effect Interfaces
// Buff and Debuff interfaces
internal interface IMagicBuffs
{
    ushort Raise { get; }
    ushort Swiftcast { get; }
    ushort Surecast { get; }
}

internal interface ICasterBuffs : IMagicBuffs
{
}

internal interface IHealerBuffs : IMagicBuffs
{
}

internal interface IPhysicalRoleBuffs
{
    ushort ArmsLength { get; }
}

internal interface IPhysRangedBuffs : IPhysicalRoleBuffs
{
    ushort Peloton { get; }
}

internal interface IMeleeBuffs : IPhysicalRoleBuffs
{
    ushort BloodBath { get; }
    ushort TrueNorth { get; }
}

internal interface ITankBuffs : IPhysicalRoleBuffs
{
}

internal interface ICasterDebuffs
{
    ushort Addle { get; }
}

internal interface IMeleeDebuffs
{
    ushort Feint { get; }
}

internal interface ITankDebuffs
{
    ushort Reprisal { get; }
}
#endregion

#region Role Interfaces
// Base interface for shared functionality / lists/ idfk
public interface IRoleAction
{
}

// Shared interfaces for Inheritance
internal interface IMagicShared : IRoleAction, ILucidDreaming, ISwiftcast, ISurecast
{
}

internal interface IPhysicalRoleShared : IRoleAction, ISecondWind, IArmsLength
{
}

// Role-specific interfaces using inheritance
internal interface IMagic : IMagicShared
{
    IMagicBuffs Buffs { get; }
}

internal interface ICaster : IMagicShared, IAddle, ISleep
{
    ICasterBuffs Buffs { get; }
    ICasterDebuffs Debuffs { get; }
}

internal interface IHealer : IMagicShared, IRepose, IEsuna, IRescue
{
    IHealerBuffs Buffs { get; }
}

internal interface IPhysicalRole : IPhysicalRoleShared
{
    IPhysicalRoleBuffs Buffs { get; }
}

internal interface IPhysRanged : IPhysicalRoleShared, ILegGraze, IFootGraze, IPeloton, IHeadGraze
{
    IPhysRangedBuffs Buffs { get; }
}

internal interface IMelee : IPhysicalRoleShared, ILegSweep, IBloodbath, IFeint, ITrueNorth
{
    IMeleeBuffs Buffs { get; }
    IMeleeDebuffs Debuffs { get; }
}

internal interface ITank : IPhysicalRoleShared, IRampart, ILowBlow, IProvoke, IInterject, IReprisal, IShirk
{
    ITankBuffs Buffs { get; }
    ITankDebuffs Debuffs { get; }
} 
#endregion

#region Buff and Debuff implementations
internal class MagicBuffs : IMagicBuffs
{
    public ushort Raise => RoleActions.Magic.Buffs.Raise;
    public ushort Swiftcast => RoleActions.Magic.Buffs.Swiftcast;
    public ushort Surecast => RoleActions.Magic.Buffs.Surecast;
}

internal class CasterBuffs : MagicBuffs, ICasterBuffs
{
}

internal class CasterDebuffs : ICasterDebuffs
{
    public ushort Addle => RoleActions.Caster.Debuffs.Addle;
}

internal class HealerBuffs : MagicBuffs, IHealerBuffs
{
}

internal class PhysicalRoleBuffs : IPhysicalRoleBuffs
{
    public ushort ArmsLength => RoleActions.Physical.Buffs.ArmsLength;
}

internal class PhysRangedBuffs : PhysicalRoleBuffs, IPhysRangedBuffs
{
    public ushort Peloton => RoleActions.PhysRanged.Buffs.Peloton;
}

internal class MeleeBuffs : PhysicalRoleBuffs, IMeleeBuffs
{
    public ushort BloodBath => RoleActions.Melee.Buffs.BloodBath;
    public ushort TrueNorth => RoleActions.Melee.Buffs.TrueNorth;
}

internal class MeleeDebuffs : IMeleeDebuffs
{
    public ushort Feint => RoleActions.Melee.Debuffs.Feint;
}

internal class TankBuffs : PhysicalRoleBuffs, ITankBuffs
{
}

internal class TankDebuffs : ITankDebuffs
{
    public ushort Reprisal => RoleActions.Tank.Debuffs.Reprisal;
} 
#endregion

#region Role implementations
static class Caster
{
    public static ICaster Instance { get; } = new CasterImpl();

    private class CasterImpl : ICaster
    {
        public ICasterBuffs Buffs { get; } = new CasterBuffs();
        public ICasterDebuffs Debuffs { get; } = new CasterDebuffs();

        public uint LucidDreaming => RoleActions.Magic.LucidDreaming;
        public uint Swiftcast => RoleActions.Magic.Swiftcast;
        public uint Surecast => RoleActions.Magic.Surecast;
        public uint Addle => RoleActions.Caster.Addle;
        public uint Sleep => RoleActions.Caster.Sleep;

        public bool CanLucidDream(int MPThreshold, bool spellweave = true) =>
            RoleActions.Magic.CanLucidDream(MPThreshold, spellweave);

        public bool CanSwiftcast(bool spellweave = true) =>
            RoleActions.Magic.CanSwiftcast(spellweave);

        public bool CanSurecast() =>
            RoleActions.Magic.CanSurecast();

        public bool CanAddle() =>
            RoleActions.Caster.CanAddle();

        public bool CanSleep() =>
            RoleActions.Caster.CanSleep();
    }
}

static class Healer
{
    public static IHealer Instance { get; } = new HealerImpl();

    private class HealerImpl : IHealer
    {
        public IHealerBuffs Buffs { get; } = new HealerBuffs();

        public uint LucidDreaming => RoleActions.Magic.LucidDreaming;
        public uint Swiftcast => RoleActions.Magic.Swiftcast;
        public uint Surecast => RoleActions.Magic.Surecast;
        public uint Repose => RoleActions.Healer.Repose;
        public uint Esuna => RoleActions.Healer.Esuna;
        public uint Rescue => RoleActions.Healer.Rescue;

        public bool CanLucidDream(int MPThreshold, bool spellweave = true) =>
            RoleActions.Magic.CanLucidDream(MPThreshold, spellweave);

        public bool CanSwiftcast(bool spellweave = true) =>
            RoleActions.Magic.CanSwiftcast(spellweave);

        public bool CanSurecast() =>
            RoleActions.Magic.CanSurecast();

        public bool CanRepose() =>
            RoleActions.Healer.CanRepose();

        public bool CanEsuna() =>
            RoleActions.Healer.CanEsuna();

        public bool CanRescue() =>
            RoleActions.Healer.CanRescue();
    }
}

static class PhysRanged
{
    public static IPhysRanged Instance { get; } = new PhysRangedImpl();

    private class PhysRangedImpl : IPhysRanged
    {
        public IPhysRangedBuffs Buffs { get; } = new PhysRangedBuffs();

        public uint SecondWind => RoleActions.Physical.SecondWind;
        public uint ArmsLength => RoleActions.Physical.ArmsLength;
        public uint LegGraze => RoleActions.PhysRanged.LegGraze;
        public uint FootGraze => RoleActions.PhysRanged.FootGraze;
        public uint Peloton => RoleActions.PhysRanged.Peloton;
        public uint HeadGraze => RoleActions.PhysRanged.HeadGraze;

        public bool CanSecondWind(int healthpercent) =>
            RoleActions.Physical.CanSecondWind(healthpercent);

        public bool CanArmsLength() => CanArmsLength(3, All.Enums.BossAvoidance.On);

        public bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null) =>
            RoleActions.Physical.CanArmsLength(enemyCount, (All.Enums.BossAvoidance)(avoidanceSetting ?? (int)All.Enums.BossAvoidance.Off));

        public bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
            RoleActions.Physical.CanArmsLength(enemyCount, avoidanceSetting);

        public bool CanLegGraze() =>
            RoleActions.PhysRanged.CanLegGraze();

        public bool CanFootGraze() =>
            RoleActions.PhysRanged.CanFootGraze();

        public bool CanPeloton() =>
            RoleActions.PhysRanged.CanPeloton();

        public bool CanHeadGraze(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) =>
            RoleActions.PhysRanged.CanHeadGraze(preset, weave);
    }
}

static class Melee
{
    public static IMelee Instance { get; } = new MeleeImpl();

    private class MeleeImpl : IMelee
    {
        public IMeleeBuffs Buffs { get; } = new MeleeBuffs();
        public IMeleeDebuffs Debuffs { get; } = new MeleeDebuffs();

        public uint SecondWind => RoleActions.Physical.SecondWind;
        public uint ArmsLength => RoleActions.Physical.ArmsLength;
        public uint LegSweep => RoleActions.Melee.LegSweep;
        public uint Bloodbath => RoleActions.Melee.Bloodbath;
        public uint Feint => RoleActions.Melee.Feint;
        public uint TrueNorth => RoleActions.Melee.TrueNorth;

        public bool CanSecondWind(int healthpercent) =>
            RoleActions.Physical.CanSecondWind(healthpercent);

        public bool CanArmsLength() => CanArmsLength(3, All.Enums.BossAvoidance.On);

        public bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null) =>
            RoleActions.Physical.CanArmsLength(enemyCount, (All.Enums.BossAvoidance)(avoidanceSetting ?? (int)All.Enums.BossAvoidance.Off));

        public bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
            RoleActions.Physical.CanArmsLength(enemyCount, avoidanceSetting);

        public bool CanLegSweep() =>
            RoleActions.Melee.CanLegSweep();

        public bool CanBloodBath(int healthpercent) =>
            RoleActions.Melee.CanBloodBath(healthpercent);

        public bool CanFeint() =>
            RoleActions.Melee.CanFeint();

        public bool CanTrueNorth() =>
            RoleActions.Melee.CanTrueNorth();
    }
}

static class Tank
{
    public static ITank Instance { get; } = new TankImpl();

    private class TankImpl : ITank
    {
        public ITankBuffs Buffs { get; } = new TankBuffs();
        public ITankDebuffs Debuffs { get; } = new TankDebuffs();

        public uint SecondWind => RoleActions.Physical.SecondWind;
        public uint ArmsLength => RoleActions.Physical.ArmsLength;
        public uint Rampart => RoleActions.Tank.Rampart;
        public uint LowBlow => RoleActions.Tank.LowBlow;
        public uint Provoke => RoleActions.Tank.Provoke;
        public uint Interject => RoleActions.Tank.Interject;
        public uint Reprisal => RoleActions.Tank.Reprisal;
        public uint Shirk => RoleActions.Tank.Shirk;

        public bool CanSecondWind(int healthpercent) =>
            RoleActions.Physical.CanSecondWind(healthpercent);

        public bool CanArmsLength() => CanArmsLength(3, All.Enums.BossAvoidance.On);

        public bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null) =>
            RoleActions.Physical.CanArmsLength(enemyCount, (All.Enums.BossAvoidance)(avoidanceSetting ?? (int)All.Enums.BossAvoidance.Off));

        public bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
            RoleActions.Physical.CanArmsLength(enemyCount, avoidanceSetting);

        public bool CanRampart(int healthPercent) =>
            RoleActions.Tank.CanRampart(healthPercent);

        public bool CanLowBlow() =>
            RoleActions.Tank.CanLowBlow();

        public bool CanProvoke() =>
            RoleActions.Tank.CanProvoke();

        public bool CanInterject() =>
            RoleActions.Tank.CanInterject();

        public bool CanReprisal(int healthPercent = 101, int? enemyCount = null, bool checkTargetForDebuff = true) =>
            RoleActions.Tank.CanReprisal(healthPercent, enemyCount, checkTargetForDebuff);

        public bool CanShirk() =>
            RoleActions.Tank.CanShirk();
    }
} 
#endregion