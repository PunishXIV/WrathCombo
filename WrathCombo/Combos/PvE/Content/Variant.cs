using ECommons.DalamudServices;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content;

#region Variant Actions and Functions
// Static utility class for shared logic
internal static class VariantActions
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
        HasBattleTarget() && GetDebuffRemainingTime(Debuffs.SustainedDamage) <= 3;

    internal static bool CanCure(CustomComboPreset preset, int healthpercent) =>
        IsEnabled(preset) && ActionReady(VariantCure) &&
        PlayerHealthPercentageHp() <= healthpercent;

    internal static bool CanRaise(CustomComboPreset preset) =>
        IsEnabled(preset) && ActionReady(VariantRaise)
        && HasEffect(MagicRole.Buffs.Swiftcast);

    internal static bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) =>
        IsEnabled(preset) && ActionReady(VariantUltimatum)
        && CanCircleAoe(5) > 0 && CheckWeave(weave);
} 
#endregion

#region Variant Action Interfaces
// Base interface for shared functionality (if any)
internal interface IVariant
{
    // Add any shared properties/methods here if needed
    // Currently empty since all actions are role-specific
}

// Action-specific interfaces
internal interface IVariantCure
{
    uint Cure { get; }
    bool CanCure(CustomComboPreset preset, int healthpercent);
}

internal interface IVariantUltimatum
{
    uint Ultimatum { get; }
    bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None);
}

internal interface IVariantRaise
{
    uint Raise { get; }
    bool CanRaise(CustomComboPreset preset);
}

internal interface IVariantSpiritDart
{
    uint SpiritDart { get; }
    bool CanSpiritDart(CustomComboPreset preset);
}

internal interface IVariantRampart
{
    uint Rampart { get; }
    bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None);
}
#endregion

#region Variant Action Interface Groupings
// Role-specific variant interfaces
internal interface ITankVariant : IVariant, IVariantCure, IVariantUltimatum, IVariantRaise, IVariantSpiritDart
{
}

internal interface IHealerVariant : IVariant, IVariantUltimatum, IVariantSpiritDart, IVariantRampart
{
}

internal interface IMDPSVariant : IVariant, IVariantCure, IVariantUltimatum, IVariantRaise, IVariantRampart
{
}

internal interface IPDPSVariant : IVariant, IVariantCure, IVariantUltimatum, IVariantRaise, IVariantRampart
{
}

internal interface ICasterVariant : IVariant, IVariantCure, IVariantUltimatum, IVariantRaise, IVariantRampart
{
}
#endregion

#region Variant Jobs
// VariantTank: Cure, Ultimatum, Raise, SpiritDart
internal class VariantTank : ITankVariant
{
    public uint Cure => VariantActions.VariantCure;
    public uint Ultimatum => VariantActions.VariantUltimatum;
    public uint Raise => VariantActions.VariantRaise;
    public uint SpiritDart => VariantActions.VariantSpiritDart;

    public bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
    public bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);
}

// VariantHealer: Ultimatum, SpiritDart, Rampart
internal class VariantHealer : IHealerVariant
{
    public uint Ultimatum => VariantActions.VariantUltimatum;
    public uint SpiritDart => VariantActions.VariantSpiritDart;
    public uint Rampart => VariantActions.VariantRampart;

    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
    public bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);
    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
}

// VariantPDPS: Cure, Ultimatum, Raise, Rampart
internal class VariantPDPS : IPDPSVariant
{
    public uint Cure => VariantActions.VariantCure;
    public uint Ultimatum => VariantActions.VariantUltimatum;
    public uint Raise => VariantActions.VariantRaise;
    public uint Rampart => VariantActions.VariantRampart;

    public bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
    public bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
}

// VariantMDPS: Cure, Ultimatum, Raise, Rampart
internal class VariantMDPS : IMDPSVariant
{
    public uint Cure => VariantActions.VariantCure;
    public uint Ultimatum => VariantActions.VariantUltimatum;
    public uint Raise => VariantActions.VariantRaise;
    public uint Rampart => VariantActions.VariantRampart;

    public bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
    public bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
}

// VariantCaster (Magical Ranged DPS): Cure, Ultimatum, Raise, Rampart
internal class VariantCaster : ICasterVariant
{
    public uint Cure => VariantActions.VariantCure;
    public uint Ultimatum => VariantActions.VariantUltimatum;
    public uint Raise => VariantActions.VariantRaise;
    public uint Rampart => VariantActions.VariantRampart;

    public bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
    public bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
} 
#endregion