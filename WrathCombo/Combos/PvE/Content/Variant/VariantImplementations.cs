using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content
{
    internal static partial class Variant
    {
        internal class VariantRoles
        {
            // VariantTank: Cure, Ultimatum, Raise, SpiritDart
            internal static class Tank
            {
                public static ITankVariant Instance { get; } = new VariantTankImpl();

                private class VariantTankImpl : ITankVariant
                {
                    public uint Cure => Variant.VariantCure;
                    public uint Ultimatum => Variant.VariantUltimatum;
                    public uint Raise => Variant.VariantRaise;
                    public uint SpiritDart => Variant.VariantSpiritDart;

                    public bool CanCure(CustomComboPreset preset, int healthpercent) => Variant.CanCure(preset, healthpercent);
                    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanUltimatum(preset, weave);
                    public bool CanRaise(CustomComboPreset preset) => Variant.CanRaise(preset);
                    public bool CanSpiritDart(CustomComboPreset preset) => Variant.CanSpiritDart(preset);
                }
            }

            // VariantHealer: Ultimatum, SpiritDart, Rampart
            internal static class Healer
            {
                public static IHealerVariant Instance { get; } = new VariantHealerImpl();

                private class VariantHealerImpl : IHealerVariant
                {
                    public uint Ultimatum => Variant.VariantUltimatum;
                    public uint SpiritDart => Variant.VariantSpiritDart;
                    public uint Rampart => Variant.VariantRampart;

                    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanUltimatum(preset, weave);
                    public bool CanSpiritDart(CustomComboPreset preset) => Variant.CanSpiritDart(preset);
                    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanRampart(preset, weave);
                }
            }

            // VariantPhysRanged: Cure, Ultimatum, Raise, Rampart
            internal static class PhysicalRanged
            {
                public static IPhysicalRangedVariant Instance { get; } = new VariantPhysRangedImpl();

                private class VariantPhysRangedImpl : IPhysicalRangedVariant
                {
                    public uint Cure => Variant.VariantCure;
                    public uint Ultimatum => Variant.VariantUltimatum;
                    public uint Raise => Variant.VariantRaise;
                    public uint Rampart => Variant.VariantRampart;

                    public bool CanCure(CustomComboPreset preset, int healthpercent) => Variant.CanCure(preset, healthpercent);
                    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanUltimatum(preset, weave);
                    public bool CanRaise(CustomComboPreset preset) => Variant.CanRaise(preset);
                    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanRampart(preset, weave);
                }
            }

            // VariantMelee: Cure, Ultimatum, Raise, Rampart
            internal static class Melee
            {
                public static IMeleeVariant Instance { get; } = new VariantMeleeImpl();

                private class VariantMeleeImpl : IMeleeVariant
                {
                    public uint Cure => Variant.VariantCure;
                    public uint Ultimatum => Variant.VariantUltimatum;
                    public uint Raise => Variant.VariantRaise;
                    public uint Rampart => Variant.VariantRampart;

                    public bool CanCure(CustomComboPreset preset, int healthpercent) => Variant.CanCure(preset, healthpercent);
                    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanUltimatum(preset, weave);
                    public bool CanRaise(CustomComboPreset preset) => Variant.CanRaise(preset);
                    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanRampart(preset, weave);
                }
            }

            // VariantCaster (Magical Ranged DPS): Cure, Ultimatum, Raise, Rampart
            internal static class Caster
            {
                public static ICasterVariant Instance { get; } = new VariantCasterImpl();

                private class VariantCasterImpl : ICasterVariant
                {
                    public uint Cure => Variant.VariantCure;
                    public uint Ultimatum => Variant.VariantUltimatum;
                    public uint Raise => Variant.VariantRaise;
                    public uint Rampart => Variant.VariantRampart;

                    public bool CanCure(CustomComboPreset preset, int healthpercent) => Variant.CanCure(preset, healthpercent);
                    public bool CanUltimatum(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanUltimatum(preset, weave);
                    public bool CanRaise(CustomComboPreset preset) => Variant.CanRaise(preset);
                    public bool CanRampart(CustomComboPreset preset, WeaveTypes weave = WeaveTypes.None) => Variant.CanRampart(preset, weave);
                }
            }
        }
    }
}
