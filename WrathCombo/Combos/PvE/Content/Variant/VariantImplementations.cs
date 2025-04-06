using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content.Variant
{
    #region Variant Jobs
    // VariantTank: Cure, Ultimatum, Raise, SpiritDart
    internal static class VariantTank
    {
        public static ITankVariant Instance { get; } = new VariantTankImpl();

        private class VariantTankImpl : ITankVariant
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
    }

    // VariantHealer: Ultimatum, SpiritDart, Rampart
    internal static class VariantHealer
    {
        public static IHealerVariant Instance { get; } = new VariantHealerImpl();

        private class VariantHealerImpl : IHealerVariant
        {
            public uint Ultimatum => VariantActions.VariantUltimatum;
            public uint SpiritDart => VariantActions.VariantSpiritDart;
            public uint Rampart => VariantActions.VariantRampart;

            public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanUltimatum(preset, weave);
            public bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);
            public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => VariantActions.CanRampart(preset, weave);
        }
    }

    // VariantPhysRanged: Cure, Ultimatum, Raise, Rampart
    internal static class VariantPhysRanged
    {
        public static IPhysRangedVariant Instance { get; } = new VariantPhysRangedImpl();

        private class VariantPhysRangedImpl : IPhysRangedVariant
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
    }

    // VariantMelee: Cure, Ultimatum, Raise, Rampart
    internal static class VariantMelee
    {
        public static IMeleeVariant Instance { get; } = new VariantMeleeImpl();

        private class VariantMeleeImpl : IMeleeVariant
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
    }

    // VariantCaster (Magical Ranged DPS): Cure, Ultimatum, Raise, Rampart
    internal static class VariantCaster
    {
        public static ICasterVariant Instance { get; } = new VariantCasterImpl();

        private class VariantCasterImpl : ICasterVariant
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
    }
    #endregion
}
