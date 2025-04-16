using ECommons.DalamudServices;
using static WrathCombo.Combos.PvE.RoleActions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content;

#region Variant Actions and Functions
// Static utility class for shared logic
internal static partial class Variant
{
    internal const uint
        VariantUltimatum = 29730,
        VariantRaise = 29731,
        VariantRaise2 = 29734;

    // 1069 = The Sil'dihn Subterrane
    // 1137 = Mount Rokkon
    // 1176 = Aloalo Island
    internal static uint VariantCure => Svc.ClientState.TerritoryType switch
    {
        1069 => 29729,
        1137 or 1176 => 33862,
        var _ => 0
    };

    internal static uint VariantSpiritDart => Svc.ClientState.TerritoryType switch
    {
        1069 => 29732,
        1137 or 1176 => 33863,
        var _ => 0
    };

    internal static uint VariantRampart => Svc.ClientState.TerritoryType switch
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

    internal static bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) =>
        IsEnabled(preset) && ActionReady(VariantRampart) &&
        CheckWeave(weave);

    internal static bool CanSpiritDart(CustomComboPreset preset) =>
        IsEnabled(preset) && ActionReady(VariantSpiritDart) &&
        HasBattleTarget() && GetStatusEffectRemainingTime(Debuffs.SustainedDamage, CurrentTarget) <= 3;

    internal static bool CanCure(CustomComboPreset preset, int healthpercent) =>
        IsEnabled(preset) && ActionReady(VariantCure) &&
        PlayerHealthPercentageHp() <= healthpercent;

    internal static bool CanRaise(CustomComboPreset preset) =>
        IsEnabled(preset) && ActionReady(VariantRaise)
        && HasStatusEffect(Magic.Buffs.Swiftcast);

    internal static bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) =>
        IsEnabled(preset) && ActionReady(VariantUltimatum)
        && CanCircleAoe(5) > 0 && CheckWeave(weave);
}
#endregion