using ECommons.DalamudServices;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content;

internal static class VariantActions
{
    internal const uint
        VariantUltimatum = 29730,
        VariantRaise = 29731,
        VariantRaise2 = 29734;

    //1069 = The Sil'dihn Subterrane
    //1137 = Mount Rokkon
    //1176 = Aloalo Island
    internal static uint VariantCure => Svc.ClientState.TerritoryType switch
    {
        1069 => 29729,
        1137 or 1176 => 33862,
        _ => 0
    };

    internal static uint VariantSpiritDart => Svc.ClientState.TerritoryType switch
    {
        1069 => 29732,
        1137 or 1176 => 33863,
        _ => 0
    };

    internal static uint VariantRampart => Svc.ClientState.TerritoryType switch
    {
        1069 => 29733,
        1137 or 1176 => 33864,
        _ => 0
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
        IsEnabled(preset) && IsEnabled(VariantRampart) && ActionReady(VariantRampart) &&
        CheckWeave(weave);

    internal static bool CanSpiritDart(CustomComboPreset preset) =>
        IsEnabled(preset) && IsEnabled(VariantSpiritDart) && ActionReady(VariantSpiritDart) &&
        HasBattleTarget() && GetDebuffRemainingTime(Debuffs.SustainedDamage) <= 3;

    internal static bool CanCure(CustomComboPreset preset, int healthpercent) =>
        IsEnabled(preset) && IsEnabled(VariantCure) && ActionReady(VariantCure) &&
        PlayerHealthPercentageHp() <= healthpercent;

    internal static bool CanRaise(CustomComboPreset preset) =>
        IsEnabled(preset) && IsEnabled(VariantRaise) &&
        ActionReady(VariantRaise) && HasEffect(MagicRole.Buffs.Swiftcast);

    internal static bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => 
        IsEnabled(preset) && IsEnabled(VariantUltimatum) &&
        ActionReady(VariantUltimatum) && CanCircleAoe(5) > 0 && CheckWeave(weave);

}

public class VariantTank
{
    internal static uint Cure => VariantActions.VariantCure;
    internal static uint Ultimatum => VariantActions.VariantUltimatum;
    internal static uint Raise => VariantActions.VariantRaise;
    internal static uint SpiritDart => VariantActions.VariantSpiritDart;

    internal static bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    internal static bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
    internal static bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    internal static bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);

}

public class VariantHealer
{
    internal static uint SpiritDart => VariantActions.VariantSpiritDart;
    internal static uint Rampart => VariantActions.VariantRampart;
    internal static uint Ultimatum => VariantActions.VariantUltimatum;

    internal static bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);
    internal static bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
    internal static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
}
public class VariantPDPS
{
    internal static uint Cure => VariantActions.VariantCure;
    internal static uint Ultimatum => VariantActions.VariantUltimatum;
    internal static uint Raise => VariantActions.VariantRaise;
    internal static uint Rampart => VariantActions.VariantRampart;

    internal static bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    internal static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
    internal static bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    internal static bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
}

public class VariantMDPS
{
    internal static uint Cure => VariantActions.VariantCure;
    internal static uint Ultimatum => VariantActions.VariantUltimatum;
    internal static uint Raise => VariantActions.VariantRaise;
    internal static uint Rampart => VariantActions.VariantRampart;

    internal static bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    internal static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
    internal static bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    internal static bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
}
