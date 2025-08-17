using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using static WrathCombo.Combos.PvP.WHMPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class WHMPvP
{
        #region IDS

    internal class Role : PvPHealer;

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

        #endregion

        #region Config
    public static class Config
    {
        public static UserInt
            WHMPvP_PurgationThreshold = new("WHMPvP_PurgationThreshold"),
            WHMPvP_DiabrosisThreshold = new("WHMPvP_DiabrosisThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {                    
                case Preset.WHMPvP_Diabrosis:
                    UserConfig.DrawSliderInt(1, 100, WHMPvP_DiabrosisThreshold,
                        "Target HP% to use Diabrosis");

                    break;

                case Preset.WHMPvP_AfflatusPurgation:
                    UserConfig.DrawSliderInt(1, 100, WHMPvP_PurgationThreshold,
                        "Target HP% to use Line Aoe Limit Break");

                    break;
            }
        }
    }

        #endregion       

    internal class WHMPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHMPvP_Burst;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Glare)
            {
                if (!PvPCommon.TargetImmuneToDamage())
                {
                    //Limit break, with health slider
                    if (IsEnabled(Preset.WHMPvP_AfflatusPurgation) && IsLB1Ready && GetTargetHPPercent() <= WHMPvP_PurgationThreshold)
                        return AfflatusPurgation;

                    // Seraph Strike if enabled and off cooldown
                    if (IsEnabled(Preset.WHMPvP_Seraph_Strike) && IsOffCooldown(SeraphStrike))
                        return SeraphStrike;

                    // Weave conditions
                    if (CanWeave())
                    {
                        //Role Action Diabrosis Role action
                        if (IsEnabled(Preset.WHMPvP_Diabrosis) && PvPHealer.CanDiabrosis() && HasTarget() &&
                            GetTargetHPPercent() <= WHMPvP_DiabrosisThreshold)
                            return PvPHealer.Diabrosis;

                        // Miracle of Nature if enabled and off cooldown and inrange 
                        if (IsEnabled(Preset.WHMPvP_Mirace_of_Nature) && IsOffCooldown(MiracleOfNature) && InActionRange(MiracleOfNature))
                            return MiracleOfNature;
                    }

                    // Afflatus Misery if enabled and off cooldown
                    if (IsEnabled(Preset.WHMPvP_Afflatus_Misery) && IsOffCooldown(AfflatusMisery))
                        return AfflatusMisery;
                }
                // Prevent waste cure 3 option
                if (IsEnabled(Preset.WHMPvP_NoWasteCure) && HasStatusEffect(Buffs.Cure3Ready) && GetStatusEffectRemainingTime(Buffs.Cure3Ready) < 6)
                    return Cure3;
            }

            return actionID;
        }
    }
      
    internal class WHMPvP_Aquaveil : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHMPvP_Heals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Cure2)
            { 
                if (IsEnabled(Preset.WHMPvP_Cure3) && HasStatusEffect(Buffs.Cure3Ready))
                    return Cure3;

                if (IsEnabled(Preset.WHMPvP_Aquaveil) && IsOffCooldown(Aquaveil))
                    return Aquaveil;      
            }

            return actionID;
        }
    }
}