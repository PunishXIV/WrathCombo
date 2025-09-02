using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
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
        public static UserBool
            WHMPvP_Burst_HealsRetarget = new("WHMPvP_Burst_HealsRetarget");
        public static UserInt
            WHMPvP_PurgationThreshold = new("WHMPvP_PurgationThreshold"),
            WHMPvP_Burst_HealsThreshold = new("WHMPvP_Burst_HealsThreshold"),
            WHMPvP_DiabrosisThreshold = new("WHMPvP_DiabrosisThreshold");
        
        public static UserBoolArray
            WHMPvP_Burst_Heals_Options = new("WHMPvP_Burst_Heals_Options"),
            WHMPvP_Heals_Options = new("WHMPvP_Heals_Options");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {                    
                case Preset.WHMPvP_Diabrosis:
                    DrawSliderInt(1, 100, WHMPvP_DiabrosisThreshold, "Target HP% to use Diabrosis");
                    break;

                case Preset.WHMPvP_AfflatusPurgation:
                    DrawSliderInt(1, 100, WHMPvP_PurgationThreshold, "Target HP% to use Line Aoe Limit Break");
                    break;
                
                case Preset.WHMPvP_Heals:
                    DrawHorizontalMultiChoice(WHMPvP_Heals_Options, "Retarget Cure 2","To the Heal Stack (In Wrath Settings)", 3, 0);
                    DrawHorizontalMultiChoice(WHMPvP_Heals_Options, "Retarget Cure 3","To the Heal Stack (In Wrath Settings)", 3, 1);
                    DrawHorizontalMultiChoice(WHMPvP_Heals_Options, "Retarget Aquaveil","To the Heal Stack (In Wrath Settings)", 3, 2);
                    break;
                
                case Preset.WHMPvP_Burst_Heals:
                    DrawAdditionalBoolChoice(WHMPvP_Burst_HealsRetarget, "Retarget", "Retargets  to the Heal Stack(In Wrath Settings)");
                    DrawSliderInt(1, 100, WHMPvP_Burst_HealsThreshold, "HP% to use Heals");
                    DrawHorizontalMultiChoice(WHMPvP_Burst_Heals_Options, "Cure 2","Adds Cure 2", 3, 0);
                    DrawHorizontalMultiChoice(WHMPvP_Burst_Heals_Options, "Cure 3","Adds Cure 3", 3, 1);
                    DrawHorizontalMultiChoice(WHMPvP_Burst_Heals_Options, "Aquaveil","Adds Aquaveil", 3, 2);
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
            if (actionID is not Glare) 
                return actionID;
            
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
            if (IsEnabled(Preset.WHMPvP_Burst_Heals) && !HasStatusEffect(Buffs.SacredSight))
            {
                IGameObject? healTarget = WHMPvP_Burst_HealsRetarget ? SimpleTarget.Stack.AllyToHealPVP : SimpleTarget.Stack.Allies;
                
                if (WHMPvP_Burst_Heals_Options[1] && HasStatusEffect(Buffs.Cure3Ready) && GetTargetHPPercent(healTarget) <= WHMPvP_Burst_HealsThreshold)
                    return WHMPvP_Burst_HealsRetarget
                        ? Cure3.Retarget(Glare, healTarget, true)
                        : Cure3;
                
                if (WHMPvP_Burst_Heals_Options[2] && ActionReady(Aquaveil) && CanWeave() && GetTargetHPPercent(healTarget) <= WHMPvP_Burst_HealsThreshold)
                    return WHMPvP_Burst_HealsRetarget
                        ? Aquaveil.Retarget(Glare, healTarget, true)
                        : Aquaveil;
                
                if (WHMPvP_Burst_Heals_Options[0] && ActionReady(Cure2) && GetTargetHPPercent(healTarget) <= WHMPvP_Burst_HealsThreshold)
                    return WHMPvP_Burst_HealsRetarget
                        ? Cure2.Retarget(Glare, healTarget, true)
                        : Cure2;
            }
            return actionID;
        }
    }
      
    internal class WHMPvP_Aquaveil : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHMPvP_Heals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cure2) 
                return actionID;
            
            if (IsEnabled(Preset.WHMPvP_Cure3) && HasStatusEffect(Buffs.Cure3Ready))
                return WHMPvP_Heals_Options[1]
                    ? Cure3.Retarget(Cure2, SimpleTarget.Stack.AllyToHealPVP, true)
                    : Cure3;

            if (IsEnabled(Preset.WHMPvP_Aquaveil) && IsOffCooldown(Aquaveil))
                return WHMPvP_Heals_Options[2]
                    ? Aquaveil.Retarget(Cure2, SimpleTarget.Stack.AllyToHealPVP, true)
                    : Aquaveil;

            return WHMPvP_Heals_Options[2]
                ? actionID.Retarget(Cure2, SimpleTarget.Stack.AllyToHealPVP, true)
                : actionID;
        }
    }
}