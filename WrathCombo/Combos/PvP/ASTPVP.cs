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
                    if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DrawCard) && IsOffCooldown(MinorArcana) && (!HasEffect(Buffs.LadyOfCrowns) && !HasEffect(Buffs.LordOfCrowns)))
                        return MinorArcana;

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        var cardPlayOption = PluginConfiguration.GetCustomIntValue(AST.Config.ASTPvP_Burst_PlayCardOption);

                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_PlayCard) && CanWeave(actionID))
                        {
                            bool hasLadyOfCrowns = HasEffect(Buffs.LadyOfCrowns);
                            bool hasLordOfCrowns = HasEffect(Buffs.LordOfCrowns);

                            if ((cardPlayOption == 1 && (hasLadyOfCrowns || hasLordOfCrowns)) ||
                                (cardPlayOption == 2 && hasLordOfCrowns) ||
                                (cardPlayOption == 3 && hasLadyOfCrowns))
                            {
                                return OriginalHook(MinorArcana);
                            }
                        }


                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_Macrocosmos) && lastComboMove == DoubleGravity && IsOffCooldown(Macrocosmos))
                            return Macrocosmos;

                        if (IsEnabled(CustomComboPreset.ASTPvP_DoubleCast) && lastComboMove == Gravity && HasCharges(DoubleCast))
                            return DoubleGravity;

                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_Gravity) && IsOffCooldown(Gravity))
                            return Gravity;

                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleMalefic))
                        {
                            if (lastComboMove == Malefic && (GetRemainingCharges(DoubleCast) > 1 ||
                                GetCooldownRemainingTime(Gravity) > 7.5f) && CanWeave(actionID) && IsEnabled(CustomComboPreset.ASTPvP_DoubleCast))
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
