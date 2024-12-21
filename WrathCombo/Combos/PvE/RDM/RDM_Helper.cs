using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal partial class RDM
    {
        internal static RDMOpenerMaxLevel1 Opener1 = new();
        internal static WrathOpener Opener()
        {
            if (Opener1.LevelChecked) return Opener1;

            return WrathOpener.Dummy;
        }

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

        internal class RDMOpenerMaxLevel1 : WrathOpener
        {
            public override List<uint> OpenerActions { get; set; } =
            [
                Veraero3,
                Verthunder3,
                All.Swiftcast,
                Verthunder3,
                Fleche,
                Acceleration,
                Verthunder3,
                Embolden,
                Manafication,
                EnchantedRiposte,
                ContreSixte,
                EnchantedZwerchhau,
                Engagement,
                EnchantedRedoublement,
                Corpsacorps,
                Verholy,
                ViceOfThorns,
                Scorch,
                Engagement,
                Corpsacorps,
                Resolution,
                Prefulgence,
                GrandImpact,
                Acceleration,
                Verfire,
                GrandImpact,
                Verthunder3,
                Fleche,
                Veraero3,
                Verfire,
                Verthunder3,
                Verstone,
                Veraero3,
                All.Swiftcast,
                Veraero3,
                ContreSixte
            ];
            public override int MinOpenerLevel => 100;
            public override int MaxOpenerLevel => 109;

            internal override UserData? ContentCheckConfig => Config.RDM_BalanceOpener_Content;

            public override bool HasCooldowns()
            {
                if (!ActionsReady([All.Swiftcast, Fleche, Embolden, Manafication, ContreSixte]) || GetRemainingCharges(Acceleration) < 2 ||
                    GetRemainingCharges(Engagement) < 2 ||
                    GetRemainingCharges(Corpsacorps) < 2)
                    return false;

                return true;
            }
        }

    }
}
