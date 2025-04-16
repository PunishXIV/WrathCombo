using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content
{
    internal static partial class Variant
    {
        #region Variant Action Interfaces
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
        internal interface ITankVariant : IVariantCure, IVariantUltimatum, IVariantRaise, IVariantSpiritDart
        {
        }

        internal interface IHealerVariant : IVariantUltimatum, IVariantSpiritDart, IVariantRampart
        {
        }

        internal interface IMeleeVariant : IVariantCure, IVariantUltimatum, IVariantRaise, IVariantRampart
        {
        }

        internal interface IPhysicalRangedVariant : IVariantCure, IVariantUltimatum, IVariantRaise, IVariantRampart
        {
        }

        internal interface ICasterVariant : IVariantCure, IVariantUltimatum, IVariantRaise, IVariantRampart
        {
        }
        #endregion
    }
}
