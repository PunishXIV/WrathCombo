using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class WHMPvP
    {
        public const byte JobID = 24;

        public const uint
            Glare = 29223,
            Cure2 = 29224,
            Cure3 = 29225,
            AfflatusMisery = 29226,
            Aquaveil = 29227,
            MiracleOfNature = 29228,
            SeraphStrike = 29229,
            AfflatusPurgation = 29230;

        internal class Buffs
        {
            internal const ushort
                Cure3Ready = 3083,
                SacredSight = 4326;
        }

        internal class Config
        {
            internal static UserInt
                WHMPVP_HealOrder = new("WHMPVP_HealOrder");
        }

        internal class WHMPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Glare)
                {
                    if (!PvPCommon.TargetImmuneToDamage())
                    {
                        var tar = OptionalTarget as IBattleChara ?? Svc.Targets.Target as IBattleChara;
                        if (IsEnabled(CustomComboPreset.WHMPvP_AfflatusPurgation) && LimitBreakLevel == 1 && tar?.CurrentHp <= 40000)
                            return AfflatusPurgation;

                        // Afflatus Misery if enabled and off cooldown
                        if (IsEnabled(CustomComboPreset.WHMPvP_Afflatus_Misery) && IsOffCooldown(AfflatusMisery))
                            return AfflatusMisery;

                        // Weave conditions
                        if (CanWeave())
                        {
                            // Miracle of Nature if enabled and off cooldown and inrange 
                            if (IsEnabled(CustomComboPreset.WHMPvP_Mirace_of_Nature) && IsOffCooldown(MiracleOfNature) && InActionRange(MiracleOfNature))
                                return MiracleOfNature;

                            // Seraph Strike if enabled and off cooldown
                            if (IsEnabled(CustomComboPreset.WHMPvP_Seraph_Strike) && IsOffCooldown(SeraphStrike))
                                return SeraphStrike;
                        }
                    }
                }

                return actionID;
            }
        }
        internal class WHMPvP_Aquaveil : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHMPvP_Heals;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Cure2)
                {
                    bool aquaveil = IsEnabled(CustomComboPreset.WHMPvP_Aquaveil) && IsOffCooldown(Aquaveil);
                    bool cure3 = IsEnabled(CustomComboPreset.WHMPvP_Cure3) && HasEffect(Buffs.Cure3Ready);

                    if (Config.WHMPVP_HealOrder == 0)
                    {
                        if (aquaveil)
                            return Aquaveil;

                        if (cure3)
                            return Cure3;
                    }
                    else
                    {
                        if (cure3)
                            return Cure3;

                        if (aquaveil)
                            return Aquaveil;
                    }
                    
                }

                return actionID;
            }
        }
    }
}
