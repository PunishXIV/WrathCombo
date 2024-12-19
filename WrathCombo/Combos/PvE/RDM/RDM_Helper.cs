using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal partial class RDM
    {
        private class RDMMana
        {
            private static RDMGauge Gauge => GetJobGauge<RDMGauge>();
            internal static int ManaStacks => Gauge.ManaStacks;
            internal static int Black => AdjustMana(Gauge.BlackMana);
            internal static int White => AdjustMana(Gauge.WhiteMana);
            internal static int Min => AdjustMana(Math.Min(Gauge.BlackMana, Gauge.WhiteMana));
            internal static int Max => AdjustMana(Math.Max(Gauge.BlackMana, Gauge.WhiteMana));
            private static int AdjustMana(byte mana)
            {
                if (LevelChecked(Manafication))
                {
                    byte magickedSword = GetBuffStacks(Buffs.MagickedSwordPlay);
                    byte magickedSwordMana = magickedSword switch
                    {
                        3 => 50,
                        2 => 30,
                        1 => 15,
                        _ => 0
                    };
                    return mana + magickedSwordMana;
                }
                else return mana;
            }
        }

        private static bool TryOGCDs(uint actionID, in bool SingleTarget, out uint newActionID, bool AdvMode = false)
        {
            var distance = GetTargetDistance();

            uint placeOGCD = 0;

            //Simple Settings
            bool fleche = true;
            bool contre = true;
            bool engagement = false;
            bool vice = true;
            bool prefulg = true;
            int engagementPool = 0;
            bool corpacorps = false;
            int corpsacorpsPool = 0;
            int corpacorpsRange = 25;

            if (AdvMode)
            {
                fleche = SingleTarget ? Config.RDM_ST_oGCD_Fleche : Config.RDM_AoE_oGCD_Fleche;
                contre = SingleTarget ? Config.RDM_ST_oGCD_ContreSixte : Config.RDM_AoE_oGCD_ContreSixte;
                engagement = SingleTarget ? Config.RDM_ST_oGCD_Engagement : Config.RDM_AoE_oGCD_Engagement;
                vice = SingleTarget ? Config.RDM_ST_oGCD_ViceOfThorns : Config.RDM_AoE_oGCD_ViceOfThorns;
                prefulg = SingleTarget ? Config.RDM_ST_oGCD_Prefulgence : Config.RDM_AoE_oGCD_Prefulgence;
                
                engagementPool = (SingleTarget && Config.RDM_ST_oGCD_Engagement_Pooling) || (!SingleTarget && Config.RDM_AoE_oGCD_Engagement_Pooling) ? 1 : 0;
                corpacorps = SingleTarget ? Config.RDM_ST_oGCD_CorpACorps : Config.RDM_AoE_oGCD_CorpACorps;
                corpsacorpsPool = (SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Pooling) || (!SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Pooling) ? 1 : 0;
                corpacorpsRange = (SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Melee) || (!SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Melee) ? 3 : 25;
            }

            //Grabs an oGCD to return based on radio options

            if (placeOGCD == 0
                && fleche
                && ActionReady(Fleche))
                placeOGCD = Fleche;

            if (placeOGCD == 0
                && contre
                && ActionReady(ContreSixte))
                placeOGCD = ContreSixte;
            
            if (placeOGCD == 0
                && engagement
                && (GetRemainingCharges(Engagement) > engagementPool
                    || (GetRemainingCharges(Engagement) == 1 && GetCooldownRemainingTime(Engagement) < 3))
                && LevelChecked(Engagement)
                && distance <= 3)
                placeOGCD = Engagement;

            if (placeOGCD == 0
                && corpacorps
                && (GetRemainingCharges(Corpsacorps) > corpsacorpsPool
                    || (GetRemainingCharges(Corpsacorps) == 1 && GetCooldownRemainingTime(Corpsacorps) < 3))
                && ((GetRemainingCharges(Corpsacorps) >= GetRemainingCharges(Engagement)) || !LevelChecked(Engagement)) // Try to alternate between Corps-a-corps and Engagement
                && LevelChecked(Corpsacorps)
                && distance <= corpacorpsRange)
                placeOGCD = Corpsacorps;

            if (placeOGCD == 0
                && vice
                && TraitLevelChecked(Traits.EnhancedEmbolden)
                && HasEffect(Buffs.ThornedFlourish))
                placeOGCD = ViceOfThorns;

            if (placeOGCD == 0 
                && prefulg
                && TraitLevelChecked(Traits.EnhancedManaficationIII)
                && HasEffect(Buffs.PrefulugenceReady))
                placeOGCD = Prefulgence;

            if (CanSpellWeave() && placeOGCD != 0)
            {
                newActionID = placeOGCD;
                return true;
            }

            if (actionID is Fleche && placeOGCD == 0) // All actions are on cooldown, determine the lowest CD to display on Fleche.
            {
                placeOGCD = Fleche;
                if (contre
                    && LevelChecked(ContreSixte)
                    && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(ContreSixte))
                    placeOGCD = ContreSixte;
                if (corpacorps
                    && LevelChecked(Corpsacorps)
                    && !HasCharges(Corpsacorps)
                    && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(Corpsacorps))
                    placeOGCD = Corpsacorps;
                if (engagement
                    && LevelChecked(Engagement)
                    && GetCooldownRemainingTime(Engagement) == 0
                    && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(Engagement))
                    placeOGCD = Engagement;
            }
            if (actionID is Fleche)
            {
                newActionID = placeOGCD;
                return true;
            }

            newActionID = 0;
            return false;
        }

        private static bool TryLucidDreaming(uint actionID, int MPThreshold, uint lastComboMove)
        {
            return
                All.CanUseLucid(actionID, MPThreshold)
                && InCombat()
                && !HasEffect(Buffs.Dualcast)
                && lastComboMove != EnchantedRiposte
                && lastComboMove != EnchantedZwerchhau
                && lastComboMove != EnchantedRedoublement
                && lastComboMove != Verflare
                && lastComboMove != Verholy
                && lastComboMove != Scorch; // Change abilities to Lucid Dreaming for entire weave window
        }

        private class MeleeCombo
        {
            internal static bool TrySTManaEmbolden(uint actionID, uint lastComboMove, byte level, out uint newActionID,
                //Simple Mode Values
                bool ManaEmbolden = true, bool GapCloser = true, bool DoubleCombo = true, bool UnBalanceMana = true )
            {
                //RDM_ST_MANAFICATIONEMBOLDEN
                if (ManaEmbolden
                    && LevelChecked(Embolden)
                    && HasCondition(ConditionFlag.InCombat)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration)
                    && (GetTargetDistance() <= 3 || (GapCloser && HasCharges(Corpsacorps))))
                {
                    //Situation 1: Manafication first
                    if (DoubleCombo
                        && level >= 90
                        && RDMMana.ManaStacks == 0
                        && lastComboMove is not Verflare
                        && lastComboMove is not Verholy
                        && lastComboMove is not Scorch
                        && RDMMana.Max <= 50
                        && (RDMMana.Max >= 42
                            || (UnBalanceMana && RDMMana.Black == RDMMana.White && RDMMana.Black >= 38 && HasCharges(Acceleration)))
                        && RDMMana.Min >= 31
                        && IsOffCooldown(Manafication)
                        && (IsOffCooldown(Embolden) || GetCooldownRemainingTime(Embolden) <= 3))
                    {
                        if (UnBalanceMana
                            && RDMMana.Black == RDMMana.White
                            && RDMMana.Black <= 44
                            && RDMMana.Black >= 38
                            && HasCharges(Acceleration))
                        {
                            newActionID = Acceleration;
                            return true;
                        }

                        newActionID = Manafication;
                        return true;
                    }
                    if (DoubleCombo
                        && level >= 90
                        && lastComboMove is Zwerchhau or EnchantedZwerchhau
                        && RDMMana.Max >= 57
                        && RDMMana.Min >= 46
                        && GetCooldownRemainingTime(Manafication) >= 100
                        && IsOffCooldown(Embolden))
                    {
                        newActionID = Embolden;
                        return true;
                    }

                    //Situation 2: Embolden first
                    if (DoubleCombo
                        && level >= 90
                        && lastComboMove is Zwerchhau or EnchantedZwerchhau
                        && RDMMana.Max <= 57
                        && RDMMana.Min <= 46
                        && (GetCooldownRemainingTime(Manafication) <= 7 || IsOffCooldown(Manafication))
                        && IsOffCooldown(Embolden))
                    {
                        newActionID = Embolden;
                        return true;
                    }
                    if (DoubleCombo
                        && level >= 90
                        && (RDMMana.ManaStacks == 0 || RDMMana.ManaStacks == 3)
                        && lastComboMove is not Verflare
                        && lastComboMove is not Verholy
                        && lastComboMove is not Scorch
                        && RDMMana.Max <= 50
                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden))
                        && IsOffCooldown(Manafication))
                    {
                        newActionID = Manafication;
                        return true;
                    }

                    //Situation 3: Just use them together
                    if ((!DoubleCombo || level < 90)
                        && ActionReady(Embolden)
                        && RDMMana.ManaStacks == 0
                        && RDMMana.Max <= 50
                        && (IsOffCooldown(Manafication) || !LevelChecked(Manafication)))
                    {
                        if (UnBalanceMana
                            && RDMMana.Black == RDMMana.White
                            && RDMMana.Black <= 44
                            && HasCharges(Acceleration))
                            {
                                newActionID = Acceleration;
                                return true;
                            }
                        {
                            newActionID = Embolden;
                            return true;
                        }
                    }
                    if ((!DoubleCombo || level < 90)
                        && ActionReady(Manafication)
                        && (RDMMana.ManaStacks == 0 || RDMMana.ManaStacks == 3)
                        && lastComboMove is not Verflare
                        && lastComboMove is not Verholy
                        && lastComboMove is not Scorch
                        && RDMMana.Max <= 50
                        && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                    {
                        newActionID = Manafication;
                        return true;
                    }

                    //Situation 4: Level 58 or 59
                    if (!LevelChecked(Manafication) &&
                        ActionReady(Embolden) &&
                        RDMMana.Min >= 50)
                    {
                        newActionID = Embolden;
                        return true;
                    }

                } //END_RDM_ST_MANAFICATIONEMBOLDEN
                newActionID = actionID;
                return false;
            }

            internal static bool TrySTMeleeCombo(uint actionID, uint lastComboMove, float comboTime, out uint newActionID,
                //Simple Mode Values
                bool MeleeEnforced = true, bool GapCloser = false, bool UnbalanceMana = true)
            {
                //Normal Combo
                if (GetTargetDistance() <= 3 || MeleeEnforced)
                {
                    if ((lastComboMove is Riposte or EnchantedRiposte)
                        && LevelChecked(Zwerchhau)
                        && comboTime > 0f)
                    {
                        newActionID = OriginalHook(Zwerchhau);
                        return true;
                    }

                    if (lastComboMove is Zwerchhau
                        && LevelChecked(Redoublement)
                        && comboTime > 0f)
                    { 
                        newActionID= OriginalHook(Redoublement);
                        return true;
                    }
                }

                if (((RDMMana.Min >= 50 && LevelChecked(Redoublement))
                    || (RDMMana.Min >= 35 && !LevelChecked(Redoublement))
                    || (RDMMana.Min >= 20 && !LevelChecked(Zwerchhau)))
                    && !HasEffect(Buffs.Dualcast))
                {
                    if (GapCloser
                        && ActionReady(Corpsacorps)
                        && GetTargetDistance() > 3)
                    {
                        newActionID = Corpsacorps;
                        return true;
                    }

                    if (UnbalanceMana
                        && LevelChecked(Acceleration)
                        && RDMMana.Black == RDMMana.White
                        && RDMMana.Black >= 50
                        && !HasEffect(Buffs.Embolden))
                    {
                        if (HasEffect(Buffs.Acceleration) || WasLastAction(Buffs.Acceleration))
                        {
                            //Run the Mana Balance Computer
                            #pragma warning disable IDE0042
                            var actions = SpellCombo.GetSpells();
                            #pragma warning restore IDE0042

                            if (actions.useAero && LevelChecked(OriginalHook(Veraero)))
                            {
                                newActionID = OriginalHook(Veraero);
                                return true;
                            }

                            if (actions.useThunder && LevelChecked(OriginalHook(Verthunder)))
                            { 
                                newActionID = OriginalHook(Verthunder); 
                                return true; 
                            }
                        }

                        if (HasCharges(Acceleration)) {
                            newActionID = Acceleration; return true; 
                        }

                    }
                    if (GetTargetDistance() <= 3)
                    {
                        newActionID = OriginalHook(Riposte);
                        return true;
                    }
                }

                newActionID = actionID;
                return false;
            }

            internal static bool TryAoEManaEmbolden(uint actionID, uint lastComboMove, out uint newActionID,
                //Simple Mode Values
                int MoulinetRange = 6)//idk just making this up
            {
                if (InCombat()
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration)
                    && ((GetTargetDistance() <= MoulinetRange && RDMMana.ManaStacks == 0) || RDMMana.ManaStacks > 0))
                {
                    if (ActionReady(Manafication))
                    {
                        //Situation 1: Embolden First (Double)
                        if (RDMMana.ManaStacks == 2
                            && RDMMana.Min >= 22
                            && IsOffCooldown(Embolden))
                        {
                            newActionID = Embolden;
                            return true;
                        }
                        if (((RDMMana.ManaStacks == 3 && RDMMana.Min >= 2) || (RDMMana.ManaStacks == 0 && RDMMana.Min >= 10))
                            && lastComboMove is not Verflare
                            && lastComboMove is not Verholy
                            && lastComboMove is not Scorch
                            && RDMMana.Max <= 50
                            && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                        {
                            newActionID = Manafication;
                            return true;
                        }

                        //Situation 2: Embolden First (Single)
                        if (RDMMana.ManaStacks == 0
                            && lastComboMove is not Verflare
                            && lastComboMove is not Verholy
                            && lastComboMove is not Scorch
                            && RDMMana.Max <= 50
                            && RDMMana.Min >= 10
                            && IsOffCooldown(Embolden))
                        {
                            newActionID = Embolden;
                            return true;
                        }
                        if (RDMMana.ManaStacks == 0
                            && lastComboMove is not Verflare
                            && lastComboMove is not Verholy
                            && lastComboMove is not Scorch
                            && RDMMana.Max <= 50
                            && RDMMana.Min >= 10
                            && (HasEffect(Buffs.Embolden) || WasLastAction(Embolden)))
                        {
                            newActionID = Manafication;
                            return true;
                        }
                    }

                    //Below Manafication Level
                    if (ActionReady(Embolden) && !LevelChecked(Manafication)
                        && RDMMana.Min >= 20)
                    {
                        newActionID = Embolden;
                        return true;
                    }
                }

                newActionID = actionID;
                return false;
            }

            internal static bool TryAoEMeleeCombo(uint actionID, uint lastComboMove, float comboTime, out uint newActionID,
                //Simple Mode Values
                int MoulinetRange = 6,
                bool GapCloser = false,
                bool MeleeEnforced = true)
            {
                if (GetTargetDistance() <= MoulinetRange || MeleeEnforced)
                {
                    //Finish the combo
                    if (LevelChecked(Moulinet)
                    && lastComboMove is EnchantedMoulinet or EnchantedMoulinetDeux
                    && comboTime > 0f)
                    {
                        newActionID = OriginalHook(Moulinet);
                        return true;
                    }
                }

                if (LevelChecked(Moulinet)
                    && LocalPlayer.IsCasting == false
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration)
                    && RDMMana.Min >= 50)
                {
                    if (GapCloser
                        && ActionReady(Corpsacorps)
                        && GetTargetDistance() > MoulinetRange)
                    {
                        newActionID = Corpsacorps;
                        return true;
                    }

                    if ((GetTargetDistance() <= MoulinetRange && RDMMana.ManaStacks == 0) || RDMMana.ManaStacks >= 1)
                    {
                        newActionID = OriginalHook(Moulinet);
                        return true;
                    }
                        
                }

                newActionID = actionID;
                return false;
            }

            internal static bool TryMeleeFinisher(uint lastComboMove, out uint actionID)
            {
                if (RDMMana.ManaStacks >= 3)
                {
                    if (RDMMana.Black >= RDMMana.White && LevelChecked(Verholy))
                    {
                        if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                            && !HasEffect(Buffs.VerfireReady)
                            && HasEffect(Buffs.VerstoneReady) && GetBuffRemainingTime(Buffs.VerstoneReady) >= 10
                            && (RDMMana.Black - RDMMana.White <= 18))
                        {
                            actionID = Verflare;
                            return true;
                        }
                        actionID = Verholy;
                        return true;
                    }
                    else if (LevelChecked(Verflare))
                    {
                        if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                            && HasEffect(Buffs.VerfireReady) && GetBuffRemainingTime(Buffs.VerfireReady) >= 10
                            && !HasEffect(Buffs.VerstoneReady)
                            && LevelChecked(Verholy)
                            && (RDMMana.White - RDMMana.Black <= 18))
                        {
                            actionID = Verholy;
                            return true;
                        }
                        actionID = Verflare;
                        return true;
                    }
                }
                if ((lastComboMove is Verflare or Verholy)
                    && LevelChecked(Scorch))
                {
                    actionID = Scorch;
                    return true;
                }

                if (lastComboMove is Scorch
                    && LevelChecked(Resolution))
                {
                    actionID = Resolution;
                    return true;
                }

                actionID = 0;
                return false;
            }

        }

        private class SpellCombo
        {
            private static bool TryGrandImpact(uint actionID, out uint newActionID)
            {
                if (TraitLevelChecked(Traits.EnhancedAccelerationII)
                    && HasEffect(Buffs.GrandImpactReady)
                    && !HasEffect(Buffs.Dualcast))
                {
                    newActionID = GrandImpact;
                    return true;
                }

                newActionID = actionID;
                return false;
            }
            internal static bool TryAcceleration(uint actionID, uint lastComboMove, out uint newActionID, bool swiftcast = true, bool AoEWeave = false)
            {
                //RDM_ST_ACCELERATION
                if (InCombat()
                    && LocalPlayer.IsCasting == false
                    && RDMMana.ManaStacks == 0
                    && lastComboMove is not Verflare //are these needed if the finisher is still going on?
                    && lastComboMove is not Verholy
                    && lastComboMove is not Scorch
                    && !WasLastAction(Embolden)
                    && (!AoEWeave || CanSpellWeave())
                    && !HasEffect(Buffs.VerfireReady)
                    && !HasEffect(Buffs.VerstoneReady)
                    && !HasEffect(Buffs.Acceleration)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast))
                {
                    if (ActionReady(Acceleration)
                        && GetCooldown(Acceleration).ChargeCooldownRemaining < 54.5)
                    {
                        newActionID = Acceleration;
                        return true;
                    }
                    if (swiftcast
                        && ActionReady(All.Swiftcast)
                        && !HasCharges(Acceleration))
                    {
                        newActionID = All.Swiftcast;
                        return true;
                    }
                }
                //Else
                newActionID = actionID; 
                return false;
            }
            internal static bool TrySTSpellRotation(uint actionID, out uint newActionID, bool FireStone = true, bool ThunderAero = true)
            {
                if (TryGrandImpact(actionID, out uint GrandID))
                {
                    newActionID = GrandID;
                    return true;
                }

                //SHUT UP ITS FINE
                #pragma warning disable IDE0042
                var actions = GetSpells();
                #pragma warning restore IDE0042

                //RDM_VERFIREVERSTONE
                if (FireStone
                    && !HasEffect(Buffs.Acceleration)
                    && !HasEffect(Buffs.Dualcast))
                {
                    //Run the Mana Balance Computer
                    if (actions.useFire) { newActionID = Verfire; return true; }
                    if (actions.useStone) { newActionID = Verstone; return true; }
                }
                //END_RDM_VERFIREVERSTONE

                //RDM_VERTHUNDERVERAERO
                if (ThunderAero)
                {
                    //Run the Mana Balance Computer
                    if (actions.useThunder) 
                    { 
                        newActionID = OriginalHook(Verthunder); 
                        return true;
                    }
                    if (actions.useAero)
                    {
                        newActionID = OriginalHook(Veraero);
                        return true;
                    }
                }
                newActionID = actionID; 
                return false;
            }
            internal static bool TryAoESpellRotation(uint actionID, out uint newActionID)
            {
                if (TryGrandImpact(actionID, out uint GrandID))
                {
                    newActionID = GrandID;
                    return true;
                }
                
                //SHUT UP ITS FINE
                #pragma warning disable IDE0042
                var actions = GetSpells();
                #pragma warning restore IDE0042

                if (actions.useThunder2)
                {
                    newActionID = OriginalHook(Verthunder2);
                    return true;
                }
                if (actions.useAero2)
                {
                    newActionID = OriginalHook(Veraero2);
                    return true;
                }

                newActionID = actionID;
                return false;
            }
            internal static (bool useFire, bool useStone, bool useThunder, bool useAero, bool useThunder2, bool useAero2) GetSpells()
            {
                //SYSTEM_MANA_BALANCING_MACHINE
                //Machine to decide which ver spell should be used.
                //Rules:
                //1.Avoid perfect balancing [NOT DONE]
                //   - Jolt adds 2/2 mana
                //   - Scatter/Impact adds 3/3 mana
                //   - Verstone/Verfire add 5 mana
                //   - Veraero/Verthunder add 6 mana
                //   - Veraero2/Verthunder2 add 7 mana
                //   - Verholy/Verflare add 11 mana
                //   - Scorch adds 4/4 mana
                //   - Resolution adds 4/4 mana
                //2.Stay within difference limit [DONE]
                //3.Strive to achieve correct mana for double melee combo burst [DONE]
                //Reset outputs
                bool useFire = false;
                bool useStone = false;
                bool useThunder = false;
                bool useAero = false;
                bool useThunder2 = false;
                bool useAero2 = false;

                //ST
                if (LevelChecked(Verthunder)
                    && (HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Acceleration)))
                {
                    if (RDMMana.Black <= RDMMana.White || HasEffect(Buffs.VerstoneReady)) useThunder = true;
                    if (RDMMana.White <= RDMMana.Black || HasEffect(Buffs.VerfireReady)) useAero = true;
                    if (!LevelChecked(Veraero)) useThunder = true;
                }
                if (!HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration))
                {
                    //Checking the time remaining instead of just the effect, to stop last second bad casts
                    bool VerFireReady = GetBuffRemainingTime(Buffs.VerfireReady) >= GetActionCastTime(Verfire);
                    bool VerStoneReady = GetBuffRemainingTime(Buffs.VerstoneReady) >= GetActionCastTime(Verstone);

                    //Prioritize mana balance
                    if (RDMMana.Black <= RDMMana.White && VerFireReady) useFire = true;
                    if (RDMMana.White <= RDMMana.Black && VerStoneReady) useStone = true;
                    //Else use the action if we can
                    if (!useFire && !useStone && VerFireReady) useFire = true;
                    if (!useFire && !useStone && VerStoneReady) useStone = true;
                }

                //AoE
                if (LevelChecked(Verthunder2)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration))
                {
                    if (RDMMana.Black <= RDMMana.White || !LevelChecked(Veraero2)) useThunder2 = true;
                    else useAero2 = true;
                }
                //END_SYSTEM_MANA_BALANCING_MACHINE

                return (useFire, useStone, useThunder, useAero, useThunder2, useAero2);
            }
        }

        internal static class RDMOpenerLogic
        {
            private static bool HasCooldowns()
            {
                if (GetRemainingCharges(Acceleration) < 2)
                    return false;

                if (GetRemainingCharges(Corpsacorps) < 2)
                    return false;

                if (GetRemainingCharges(Engagement) < 2)
                    return false;

                if (!ActionReady(Embolden))
                    return false;

                if (!ActionReady(Manafication))
                    return false;

                if (!ActionReady(Fleche))
                    return false;

                if (!ActionReady(ContreSixte))
                    return false;

                if (!ActionReady(All.Swiftcast))
                    return false;

                return true;
            }

            private static uint OpenerLevel => 100;

            public static uint PrePullStep = 0;

            public static uint OpenerStep = 0;

            public static bool LevelChecked => LocalPlayer.Level >= OpenerLevel;

            private static bool CanOpener => HasCooldowns() && LevelChecked;

            private static OpenerState currentState = OpenerState.OpenerReady;

            public static OpenerState CurrentState
            {
                get
                {
                    return currentState;
                }
                set
                {
                    if (value != currentState)
                    {
                        if (value == OpenerState.OpenerReady)
                        {
                            Svc.Log.Debug($"Entered PrePull Opener");
                        }
                        if (value == OpenerState.InOpener) OpenerStep = 1;
                        if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                        {
                            if (value == OpenerState.FailedOpener)
                                Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                            ResetOpener();
                        }
                        if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                        currentState = value;
                    }
                }
            }

            private static bool DoPrePullSteps(ref uint actionID)
            {
                if (!LevelChecked)
                    return false;

                if (CanOpener && PrePullStep == 0)
                {
                    PrePullStep = 1;
                }

                if (!HasCooldowns())
                {
                    PrePullStep = 0;
                }

                if (CurrentState == OpenerState.OpenerReady && PrePullStep > 0)
                {
                    if (LocalPlayer.CastActionId == Veraero3 && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = Veraero3;

                    if (ActionWatching.CombatActions.Count > 2 && InCombat())
                        CurrentState = OpenerState.FailedOpener;

                    return true;
                }
                PrePullStep = 0;
                return false;
            }

            private static bool DoOpener(ref uint actionID)
            {
                if (!LevelChecked)
                    return false;

                if (currentState == OpenerState.InOpener)
                {
                    if (WasLastAction(Verthunder3) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = Verthunder3;

                    if (WasLastAction(All.Swiftcast) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = All.Swiftcast;

                    if (WasLastAction(Verthunder3) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Verthunder3;

                    if (WasLastAction(Fleche) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Fleche;

                    if (WasLastAction(Acceleration) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = Acceleration;

                    if (WasLastAction(Verthunder3) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Verthunder3;

                    if (WasLastAction(Embolden) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Embolden;

                    if (WasLastAction(Manafication) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Manafication;

                    if (WasLastAction(EnchantedRiposte) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = EnchantedRiposte;

                    if (WasLastAction(ContreSixte) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = ContreSixte;

                    if (WasLastAction(EnchantedZwerchhau) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = EnchantedZwerchhau;

                    if (WasLastAction(Engagement) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = Engagement;

                    if (WasLastAction(EnchantedRedoublement) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = EnchantedRedoublement;

                    if (WasLastAction(Corpsacorps) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Corpsacorps;

                    if (WasLastAction(Verholy) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Verholy;

                    if (WasLastAction(ViceOfThorns) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = ViceOfThorns;

                    if (WasLastAction(Scorch) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = Scorch;

                    if (WasLastAction(Engagement) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = Engagement;

                    if (WasLastAction(Corpsacorps) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Corpsacorps;

                    if (WasLastAction(Resolution) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = Resolution;

                    if (WasLastAction(Prefulgence) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = Prefulgence;

                    if (WasLastAction(GrandImpact) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = GrandImpact;

                    if (WasLastAction(Acceleration) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = Acceleration;

                    if (WasLastAction(Verfire) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = Verfire;

                    if (WasLastAction(GrandImpact) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = GrandImpact;

                    if (WasLastAction(Verthunder3) && OpenerStep == 26) OpenerStep++;
                    else if (OpenerStep == 26) actionID = Verthunder3;

                    if (WasLastAction(Fleche) && OpenerStep == 27) OpenerStep++;
                    else if (OpenerStep == 27) actionID = Fleche;

                    if (WasLastAction(Veraero3) && OpenerStep == 28) OpenerStep++;
                    else if (OpenerStep == 28) actionID = Veraero3;

                    if (WasLastAction(Verfire) && OpenerStep == 29) OpenerStep++;
                    else if (OpenerStep == 29) actionID = Verfire;

                    if (WasLastAction(Verthunder3) && OpenerStep == 30) OpenerStep++;
                    else if (OpenerStep == 30) actionID = Verthunder3;

                    if (WasLastAction(Verstone) && OpenerStep == 31) OpenerStep++;
                    else if (OpenerStep == 31) actionID = Verstone;

                    if (WasLastAction(Veraero3) && OpenerStep == 32) OpenerStep++;
                    else if (OpenerStep == 32) actionID = Veraero3;

                    if (WasLastAction(All.Swiftcast) && OpenerStep == 33) OpenerStep++;
                    else if (OpenerStep == 33) actionID = All.Swiftcast;

                    if (WasLastAction(Veraero3) && OpenerStep == 34) OpenerStep++;
                    else if (OpenerStep == 34) actionID = Veraero3;

                    if (WasLastAction(ContreSixte) && OpenerStep == 35) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 35) actionID = ContreSixte;

                    if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                        CurrentState = OpenerState.FailedOpener;

                    if (((actionID == Embolden && IsOnCooldown(Embolden)) ||
                        (actionID == Manafication && IsOnCooldown(Manafication)) ||
                        (actionID == Fleche && IsOnCooldown(Fleche)) ||
                        (actionID == ContreSixte && IsOnCooldown(ContreSixte)) ||
                        (actionID == All.Swiftcast && IsOnCooldown(All.Swiftcast)) ||
                        (actionID == Acceleration && GetRemainingCharges(Acceleration) < 2) ||
                        (actionID == Corpsacorps && GetRemainingCharges(Corpsacorps) < 2) ||
                        (actionID == Engagement && GetRemainingCharges(Engagement) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                    {
                        CurrentState = OpenerState.FailedOpener;
                        return false;
                    }
                    return true;
                }
                return false;
            }

            private static void ResetOpener()
            {
                PrePullStep = 0;
                OpenerStep = 0;
            }

            public static bool DoFullOpener(ref uint actionID)
            {
                if (!LevelChecked)
                    return false;

                if (CurrentState == OpenerState.OpenerReady)
                    if (DoPrePullSteps(ref actionID))
                        return true;

                if (CurrentState == OpenerState.InOpener)
                {
                    if (DoOpener(ref actionID))
                        return true;
                }

                if (!InCombat())
                {
                    ResetOpener();
                    CurrentState = OpenerState.OpenerReady;
                }
                return false;
            }
        }
    }
}
