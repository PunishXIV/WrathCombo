using ECommons.DalamudServices;
using static WrathCombo.Combos.PvE.RoleActions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal static partial class Variant
    {
        #region Variant Actions
        public const uint
            Ultimatum = 29730,
            Raise = 29731,
            Raise2 = 29734;

        public static uint Cure => Svc.ClientState.TerritoryType switch
        {
            1069 => 29729,
            1137 or 1176 => 33862,
            var _ => 0
        };

        public static uint SpiritDart => Svc.ClientState.TerritoryType switch
        {
            1069 => 29732,
            1137 or 1176 => 33863,
            var _ => 0
        };

        public static uint Rampart => Svc.ClientState.TerritoryType switch
        {
            1069 => 29733,
            1137 or 1176 => 33864,
            var _ => 0
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
            HasBattleTarget() && GetStatusEffectRemainingTime(Debuffs.SustainedDamage, CurrentTarget) <= 3;

        private static bool CheckCure(Preset preset, int healthpercent) =>
            IsEnabled(preset) && ActionReady(Cure) &&
            PlayerHealthPercentageHp() <= healthpercent;

        private static bool CheckRaise(Preset preset) =>
            IsEnabled(preset) && ActionReady(Raise) &&
            TargetIsFriendly() && TargetIsDead();

        private static bool CheckUltimatum(Preset preset) =>
            IsEnabled(preset) && ActionReady(Ultimatum)
            && NumberOfEnemiesInRange(Ultimatum) > 0;

        #endregion

    }
}