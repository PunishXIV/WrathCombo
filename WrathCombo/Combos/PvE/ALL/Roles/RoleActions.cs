using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal static partial class RoleActions
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

        public static bool CanLucidDream(int MPThreshold, bool weave = true) =>
            ActionReady(LucidDreaming) &&
            LocalPlayer.CurrentMp <= MPThreshold &&
            (!weave || CanWeave());

        public static bool CanSwiftcast(bool weave = true) =>
            ActionReady(Swiftcast) && (!weave || CanWeave());

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
            ActionReady(Addle) && !HasStatusEffect(Debuffs.Addle, CurrentTarget, true) &&
            CanApplyStatus(CurrentTarget, Debuffs.Addle);

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

        public static bool CanSecondWind(int healthPercent) =>
            ActionReady(SecondWind) && PlayerHealthPercentageHp() <= healthPercent;

        public static bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting) =>
            ActionReady(ArmsLength) && NumberOfEnemiesInRange(ArmsLength) >= enemyCount &&
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

        public static bool CanHeadGraze(Preset preset, WeaveTypes weave = WeaveTypes.None) =>
            IsEnabled(preset) && CanInterruptEnemy() && ActionReady(HeadGraze) && CheckWeave(weave);

        public static bool CanHeadGraze(bool simpleMode, WeaveTypes weave = WeaveTypes.None) =>
            simpleMode && CanInterruptEnemy() && ActionReady(HeadGraze) && CheckWeave(weave);
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

        public static bool CanBloodBath(int healthPercent) =>
            ActionReady(Bloodbath) && PlayerHealthPercentageHp() <= healthPercent;

        public static bool CanFeint() =>
            ActionReady(Feint) && !HasStatusEffect(Debuffs.Feint, CurrentTarget, true) &&
            CanApplyStatus(CurrentTarget, Debuffs.Feint);

        public static bool CanTrueNorth() =>
            ActionReady(TrueNorth) && TargetNeedsPositionals() && !HasStatusEffect(Buffs.TrueNorth) && CanDelayedWeave();
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

        public static bool CanRampart(int healthPercent = 100) =>
            ActionReady(Rampart) && PlayerHealthPercentageHp() <= healthPercent;

        public static bool CanLowBlow() =>
            ActionReady(LowBlow) && TargetIsCasting() && (ICDTracker.StatusIsExpired(All.Debuffs.Stun, CurrentTarget.GameObjectId) || ICDTracker.NumberOfTimesApplied(All.Debuffs.Stun, CurrentTarget.GameObjectId) < 3);

        public static bool CanProvoke() =>
            ActionReady(Provoke);

        public static bool CanInterject() =>
            ActionReady(Interject) && CanInterruptEnemy();

        public static bool CanReprisal(int healthPercent = 100, int? enemyCount = null, bool checkTargetForDebuff = true) =>
            (checkTargetForDebuff && !HasStatusEffect(Debuffs.Reprisal, CurrentTarget, true) || !checkTargetForDebuff) &&
            (enemyCount is null ? InActionRange(Reprisal) : NumberOfEnemiesInRange(Reprisal) >= enemyCount) &&
            ActionReady(Reprisal) && PlayerHealthPercentageHp() <= healthPercent && CanApplyStatus(CurrentTarget, Debuffs.Reprisal);

        public static bool CanShirk() =>
            ActionReady(Shirk);
    }
}
