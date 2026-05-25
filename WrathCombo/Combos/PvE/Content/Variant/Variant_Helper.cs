using ECommons.DalamudServices;
using System.Linq;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal static partial class Variant
    {
        #region Variant Actions
        public const uint
            Ultimatum = 29730,
            Raise = 29731,
            Raise2 = 29734,
            EagleEyeShot = 46942;

        public static uint Cure => TerritoryID switch
        {
            1069 => 29729,
            1137 or 1176 => 33862,
            1315 or 1316 => 46939,
            _ => 0,
        };

        public static uint SpiritDart => TerritoryID switch
        {
            1069 => 29732,
            1137 or 1176 => 33863,
            1315 or 1316 => 46940,
            _ => 0,
        };

        public static uint Rampart => TerritoryID switch
        {
            1069 => 29733,
            1137 or 1176 => 33864,
            1315 or 1316 => 46941,
            _ => 0,
        };

        public static class Buffs
        {
            internal const ushort
                EmnityUp = 3358,
                VulnDown = 3360,
                Rehabilitation = 3367,
                DamageBarrier = 3405;
        }

        public static class Debuffs
        {
            internal const ushort
                SustainedDamage = 3359;
        }

        #endregion

        #region Variant Action Checks

        private static bool CheckRampart(Preset preset) =>
            IsEnabled(preset) && ActionReady(Rampart);

        private static bool CheckSpiritDart(Preset preset) =>
            IsEnabled(preset) && ActionReady(SpiritDart) &&
            HasBattleTarget() && EnemiesInRange(SpiritDart).Any(x => GetStatusEffectRemainingTime(Debuffs.SustainedDamage, x) <= 3);

        private static bool CheckCure(Preset preset, int healthpercent) =>
            IsEnabled(preset) && ActionReady(Cure) &&
            PlayerHealthPercentageHp() <= healthpercent;

        private static bool CheckRaise(Preset preset) =>
            IsEnabled(preset) && ActionReady(Raise) &&
            TargetIsFriendly() && TargetIsDead();

        private static bool CheckUltimatum(Preset preset) =>
            IsEnabled(preset) && ActionReady(Ultimatum)
            && NumberOfEnemiesInRange(Ultimatum) > 0;

        private static bool CheckEagleEyeShot(Preset preset) =>
            IsEnabled(preset) && ActionReady(EagleEyeShot)
            && HasBattleTarget();

        #endregion

    }
}