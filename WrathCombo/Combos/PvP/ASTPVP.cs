using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.AST;

namespace WrathCombo.Combos.PvP
{
    internal static class ASTPvP
    {
        internal const byte JobID = 33;

        internal const uint
            Malefic = 29242,
            AspectedBenefic = 29243,
            Gravity = 29244,
            DoubleCast = 29245,
            DoubleMalefic = 29246,
            NocturnalBenefic = 29247,
            DoubleGravity = 29248,
            Draw = 29249,
            Macrocosmos = 29253,
            Microcosmos = 29254,
            MinorArcana = 41503,
            Epicycle = 41506;

        internal class Buffs
        {
            internal const ushort
                    LadyOfCrowns = 4328,
                    LordOfCrowns = 4329,
                    RetrogradeReady = 4331;

        }

        internal class ASTPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Malefic)
                {
                    // Card Draw
                    if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DrawCard) && IsOffCooldown(MinorArcana) && (!HasEffect(Buffs.LadyOfCrowns) && !HasEffect(Buffs.LordOfCrowns)))
                        return MinorArcana;                                      
                   
                    var cardPlayOption = PluginConfiguration.GetCustomIntValue(AST.Config.ASTPvP_Burst_PlayCardOption);

                    if (IsEnabled(CustomComboPreset.ASTPvP_Burst_PlayCard))
                    {
                        bool hasLadyOfCrowns = HasEffect(Buffs.LadyOfCrowns);
                        bool hasLordOfCrowns = HasEffect(Buffs.LordOfCrowns);

                        // Card Playing Split so Lady can still be used if target is immune
                        if ((cardPlayOption == 1 && hasLordOfCrowns && !PvPCommon.IsImmuneToDamage()) ||
                            (cardPlayOption == 1 && hasLadyOfCrowns) ||
                            (cardPlayOption == 2 && hasLordOfCrowns && !PvPCommon.IsImmuneToDamage()) ||
                            (cardPlayOption == 3 && hasLadyOfCrowns))

                            return OriginalHook(MinorArcana);
                    }    
                        
                    if (!PvPCommon.IsImmuneToDamage())
                    { 
                        // Macrocosmos only with double gravity or on coodlown when double gravity is disabled
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_Macrocosmos) && IsOffCooldown(Macrocosmos) &&
                           (lastComboMove == DoubleGravity || !IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleGravity)))
                            return Macrocosmos;

                        // Double Gravity
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleGravity) && lastComboMove == Gravity && HasCharges(DoubleCast))
                            return DoubleGravity;

                        // Gravity on cd
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_Gravity) && IsOffCooldown(Gravity))
                            return Gravity;

                        // Double Malefic logic to not leave gravity without a charge
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleMalefic))
                        {
                            if (lastComboMove == Malefic && (GetRemainingCharges(DoubleCast) > 1 ||
                                GetCooldownRemainingTime(Gravity) > 7.5f) && CanWeave(actionID))
                                return DoubleMalefic;
                        }

                    }

                }

                return actionID;
            }

            internal class ASTPvP_Epicycle : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Epicycle;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if(actionID is Epicycle)
                    {
                        if (IsOffCooldown(MinorArcana))
                            return MinorArcana;

                        if (HasEffect(Buffs.RetrogradeReady))
                        {
                            if (HasEffect(Buffs.LordOfCrowns))
                                return OriginalHook(MinorArcana);
                            if (IsOffCooldown(Macrocosmos))
                                return Macrocosmos;
                        }
                    }

                    return actionID;
                }
            }

            internal class ASTPvP_Heal : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Heal;

                protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
                {
                    if (actionID is AspectedBenefic && CanWeave(actionID) &&
                        lastComboMove == AspectedBenefic &&
                        HasCharges(DoubleCast))
                        return OriginalHook(DoubleCast);

                    return actionID;
                }
            }
        }
    }
}
