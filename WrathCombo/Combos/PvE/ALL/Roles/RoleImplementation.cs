using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal static partial class RoleActions
{
        #region Role implementations

    internal static class Roles
    {
        internal static class Caster
        {
            public static ICaster Instance { get; } = new CasterImpl();

            private class CasterImpl : ICaster
            {
                public ICasterBuffs Buffs { get; } = new CasterBuffs();
                public ICasterDebuffs Debuffs { get; } = new CasterDebuffs();

                public uint LucidDreaming => Magic.LucidDreaming;
                public uint Swiftcast => Magic.Swiftcast;
                public uint Surecast => Magic.Surecast;
                public uint Addle => RoleActions.Caster.Addle;
                public uint Sleep => RoleActions.Caster.Sleep;

                public bool CanLucidDream(int MPThreshold, bool spellweave = true) =>
                    Magic.CanLucidDream(MPThreshold, spellweave);

                public bool CanSwiftcast(bool spellweave = true) =>
                    Magic.CanSwiftcast(spellweave);

                public bool CanSurecast() =>
                    Magic.CanSurecast();

                public bool CanAddle() =>
                    RoleActions.Caster.CanAddle();

                public bool CanSleep() =>
                    RoleActions.Caster.CanSleep();
            }
        }

        internal static class Healer
        {
            public static IHealer Instance { get; } = new HealerImpl();

            private class HealerImpl : IHealer
            {
                public IHealerBuffs Buffs { get; } = new HealerBuffs();

                public uint LucidDreaming => Magic.LucidDreaming;
                public uint Swiftcast => Magic.Swiftcast;
                public uint Surecast => Magic.Surecast;
                public uint Repose => RoleActions.Healer.Repose;
                public uint Esuna => RoleActions.Healer.Esuna;
                public uint Rescue => RoleActions.Healer.Rescue;

                public bool CanLucidDream(int MPThreshold, bool spellweave = true) =>
                    Magic.CanLucidDream(MPThreshold, spellweave);

                public bool CanSwiftcast(bool spellweave = true) =>
                    Magic.CanSwiftcast(spellweave);

                public bool CanSurecast() =>
                    Magic.CanSurecast();

                public bool CanRepose() =>
                    RoleActions.Healer.CanRepose();

                public bool CanEsuna() =>
                    RoleActions.Healer.CanEsuna();

                public bool CanRescue() =>
                    RoleActions.Healer.CanRescue();
            }
        }

        internal static class PhysicalRanged
        {
            public static IPhysicalRanged Instance { get; } = new PhysRangedImpl();

            private class PhysRangedImpl : IPhysicalRanged
            {
                public IPhysRangedBuffs Buffs { get; } = new PhysRangedBuffs();

                public uint SecondWind => Physical.SecondWind;
                public uint ArmsLength => Physical.ArmsLength;
                public uint LegGraze => PhysRanged.LegGraze;
                public uint FootGraze => PhysRanged.FootGraze;
                public uint Peloton => PhysRanged.Peloton;
                public uint HeadGraze => PhysRanged.HeadGraze;

                public bool CanSecondWind(int healthpercent) =>
                    Physical.CanSecondWind(healthpercent);

                public bool CanArmsLength() => CanArmsLength(3, All.Enums.BossAvoidance.On);

                public bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null) =>
                    Physical.CanArmsLength(enemyCount, (All.Enums.BossAvoidance)(avoidanceSetting ?? (int)All.Enums.BossAvoidance.Off));

                public bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
                    Physical.CanArmsLength(enemyCount, avoidanceSetting);

                public bool CanLegGraze() =>
                    PhysRanged.CanLegGraze();

                public bool CanFootGraze() =>
                    PhysRanged.CanFootGraze();

                public bool CanPeloton() =>
                    PhysRanged.CanPeloton();

                public bool CanHeadGraze(Preset preset, WeaveTypes weave = WeaveTypes.None) =>
                    PhysRanged.CanHeadGraze(preset, weave);

                public bool CanHeadGraze(bool simpleMode, WeaveTypes weave = WeaveTypes.None) =>
                    PhysRanged.CanHeadGraze(simpleMode, weave);
            }
        }

        internal static class Melee
        {
            public static IMelee Instance { get; } = new MeleeImpl();

            private class MeleeImpl : IMelee
            {
                public IMeleeBuffs Buffs { get; } = new MeleeBuffs();
                public IMeleeDebuffs Debuffs { get; } = new MeleeDebuffs();

                public uint SecondWind => Physical.SecondWind;
                public uint ArmsLength => Physical.ArmsLength;
                public uint LegSweep => RoleActions.Melee.LegSweep;
                public uint Bloodbath => RoleActions.Melee.Bloodbath;
                public uint Feint => RoleActions.Melee.Feint;
                public uint TrueNorth => RoleActions.Melee.TrueNorth;

                public bool CanSecondWind(int healthpercent) =>
                    Physical.CanSecondWind(healthpercent);

                public bool CanArmsLength() => CanArmsLength(3, All.Enums.BossAvoidance.On);

                public bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null) =>
                    Physical.CanArmsLength(enemyCount, (All.Enums.BossAvoidance)(avoidanceSetting ?? (int)All.Enums.BossAvoidance.Off));

                public bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
                    Physical.CanArmsLength(enemyCount, avoidanceSetting);

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

        internal static class Tank
        {
            public static ITank Instance { get; } = new TankImpl();

            private class TankImpl : ITank
            {
                public ITankBuffs Buffs { get; } = new TankBuffs();
                public ITankDebuffs Debuffs { get; } = new TankDebuffs();

                public uint SecondWind => Physical.SecondWind;
                public uint ArmsLength => Physical.ArmsLength;
                public uint Rampart => RoleActions.Tank.Rampart;
                public uint LowBlow => RoleActions.Tank.LowBlow;
                public uint Provoke => RoleActions.Tank.Provoke;
                public uint Interject => RoleActions.Tank.Interject;
                public uint Reprisal => RoleActions.Tank.Reprisal;
                public uint Shirk => RoleActions.Tank.Shirk;

                public bool CanSecondWind(int healthpercent) =>
                    Physical.CanSecondWind(healthpercent);

                public bool CanArmsLength() => CanArmsLength(3, All.Enums.BossAvoidance.On);

                public bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null) =>
                    Physical.CanArmsLength(enemyCount, (All.Enums.BossAvoidance)(avoidanceSetting ?? (int)All.Enums.BossAvoidance.Off));

                public bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
                    Physical.CanArmsLength(enemyCount, avoidanceSetting);

                public bool CanRampart(int healthPercent = 100) =>
                    RoleActions.Tank.CanRampart(healthPercent);

                public bool CanLowBlow() =>
                    RoleActions.Tank.CanLowBlow();

                public bool CanProvoke() =>
                    RoleActions.Tank.CanProvoke();

                public bool CanInterject() =>
                    RoleActions.Tank.CanInterject();

                public bool CanReprisal(int healthPercent = 101, int? enemyCount = null, bool checkTargetForDebuff = true, IGameObject? target = null) =>
                    RoleActions.Tank.CanReprisal(healthPercent, enemyCount, checkTargetForDebuff, target);

                public bool CanShirk() =>
                    RoleActions.Tank.CanShirk();
            }
        }
    }

        #endregion
        #region Buff and Debuff implementations

    internal class MagicBuffs : IMagicBuffs
    {
        public ushort Raise => Magic.Buffs.Raise;
        public ushort Swiftcast => Magic.Buffs.Swiftcast;
        public ushort Surecast => Magic.Buffs.Surecast;
    }

    internal class CasterBuffs : MagicBuffs, ICasterBuffs
    {
    }

    internal class CasterDebuffs : ICasterDebuffs
    {
        public ushort Addle => Caster.Debuffs.Addle;
    }

    internal class HealerBuffs : MagicBuffs, IHealerBuffs
    {
    }

    internal class PhysicalRoleBuffs : IPhysicalRoleBuffs
    {
        public ushort ArmsLength => Physical.Buffs.ArmsLength;
    }

    internal class PhysRangedBuffs : PhysicalRoleBuffs, IPhysRangedBuffs
    {
        public ushort Peloton => PhysRanged.Buffs.Peloton;
    }

    internal class MeleeBuffs : PhysicalRoleBuffs, IMeleeBuffs
    {
        public ushort BloodBath => Melee.Buffs.BloodBath;
        public ushort TrueNorth => Melee.Buffs.TrueNorth;
    }

    internal class MeleeDebuffs : IMeleeDebuffs
    {
        public ushort Feint => Melee.Debuffs.Feint;
    }

    internal class TankBuffs : PhysicalRoleBuffs, ITankBuffs
    {
        public ushort Rampart => Tank.Buffs.Rampart;
    }

    internal class TankDebuffs : ITankDebuffs
    {
        public ushort Reprisal => Tank.Debuffs.Reprisal;
    }

        #endregion
}
