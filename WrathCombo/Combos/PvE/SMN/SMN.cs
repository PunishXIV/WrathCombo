using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.SMN.Config;
namespace WrathCombo.Combos.PvE;

internal partial class SMN : Caster
{
    #region Small Features
    internal class SMN_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_Raise;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            if (IsOnCooldown(Role.Swiftcast))
                return IsEnabled(Preset.SMN_Raise_Retarget)
                    ? Resurrection.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Resurrection;
            return actionID;
        }
    }
    internal class SMN_Searing : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_Searing;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != SearingLight)
                return actionID;

            if (HasStatusEffect(Buffs.RubysGlimmer))
                return SearingFlash;

            return HasStatusEffect(Buffs.SearingLight, anyOwner: true) ? 11 : actionID;
        }
    }
    internal class SMN_RuinMobility : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_RuinMobility;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Ruin4)
                return actionID;
            bool furtherRuin = HasStatusEffect(Buffs.FurtherRuin);

            return !furtherRuin ? Ruin3 : actionID;
        }
    }
    internal class SMN_EDFester : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_EDFester;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Fester or Necrotize))
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();
            if (HasStatusEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergyDrain) && !gauge.HasAetherflowStacks && IsEnabled(Preset.SMN_EDFester_Ruin4))
                return Ruin4;

            if (LevelChecked(EnergyDrain) && !gauge.HasAetherflowStacks)
                return EnergyDrain;

            return actionID;
        }
    }
    internal class SMN_ESPainflare : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_ESPainflare;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Painflare)
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();

            if (!LevelChecked(Painflare) || gauge.HasAetherflowStacks)
                return actionID;

            if (HasStatusEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergySiphon) && IsEnabled(Preset.SMN_ESPainflare_Ruin4))
                return Ruin4;

            if (LevelChecked(EnergySiphon))
                return EnergySiphon;

            return LevelChecked(EnergyDrain) ? EnergyDrain : actionID;
        }
    }
    internal class SMN_CarbuncleReminder : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_CarbuncleReminder;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2 or Ruin3 or DreadwyrmTrance or
                 AstralFlow or EnkindleBahamut or SearingLight or
                 RadiantAegis or Outburst or Tridisaster or
                 PreciousBrilliance or Gemshine))
                return actionID;

            return NeedToSummon ? SummonCarbuncle : actionID;
        }
    }

    internal class SMN_Egi_AstralFlow : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_Egi_AstralFlow;

        protected override uint Invoke(uint actionID)
        {
            if ((actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 or SummonRuby or SummonIfrit or SummonIfrit2 && HasStatusEffect(Buffs.TitansFavor)) ||
                (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 && HasStatusEffect(Buffs.GarudasFavor)) ||
                (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonRuby or SummonIfrit or SummonIfrit2 && (HasStatusEffect(Buffs.IfritsFavor) || (ComboAction == CrimsonCyclone && InMeleeRange()))))
                return OriginalHook(AstralFlow);

            return actionID;
        }
    }
    internal class SMN_DemiAbilities : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_DemiAbilities;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Aethercharge or DreadwyrmTrance or SummonBahamut) &&
                actionID is not (SummonPhoenix or SummonSolarBahamut))
                return actionID;

            if (IsOffCooldown(EnkindleBahamut) && OriginalHook(Ruin) is AstralImpulse)
                return OriginalHook(EnkindleBahamut);

            if (IsOffCooldown(EnkindlePhoenix) && OriginalHook(Ruin) is FountainOfFire)
                return OriginalHook(EnkindlePhoenix);

            if (IsOffCooldown(EnkindleSolarBahamut) && OriginalHook(Ruin) is UmbralImpulse)
                return OriginalHook(EnkindleBahamut);

            if ((OriginalHook(AstralFlow) is Deathflare && IsOffCooldown(Deathflare)) || (OriginalHook(AstralFlow) is Rekindle && IsOffCooldown(Rekindle)))
                return OriginalHook(AstralFlow);

            if (OriginalHook(AstralFlow) is Sunflare && IsOffCooldown(Sunflare))
                return OriginalHook(Sunflare);

            return actionID;
        }
    }
    #endregion

    #region Simple
    internal class SMN_Simple_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_ST_Simple_Combo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2 or Ruin3))
                return actionID;

            if (NeedToSummon)
                return SummonCarbuncle;

            #region Variables
            var dontWaitForSearing = SearingCD > (Gauge.SummonTimerRemaining / 1000f) + GCDTotal;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCD
            //Emergency Demi Attack Dump, Probably not needed anymore without burst delay selection
            if (DemiExists && Gauge.SummonTimerRemaining <= 2500)
            {
                if (ActionReady(OriginalHook(EnkindleBahamut)))
                    return OriginalHook(EnkindleBahamut);

                if (ActionReady(AstralFlow) && DemiNotPheonix)
                    return OriginalHook(AstralFlow);
            }

            if (SummonerWeave)
            {
                // Searing Light
                if (ActionReady(SearingLight) && !HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                {
                    if (TraitLevelChecked(Traits.EnhancedDreadwyrmTrance))
                    {
                        if (DemiExists)
                            return SearingLight;
                    }
                    else if (!ActionReady(OriginalHook(Aethercharge)))
                        return SearingLight;
                }

                // Energy Drain
                if (!Gauge.HasAetherflowStacks && ActionReady(EnergyDrain))
                {
                    if (LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || SearingCD > 30)
                            return OriginalHook(EnergyDrain);
                    }
                    else
                        return OriginalHook(EnergyDrain);
                }

                // Demi Nuke
                if (DemiExists && (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || dontWaitForSearing) && (GetCooldown(EnergyDrain).CooldownRemaining >= 3 || !ActionReady(Fester)))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow))
                        return OriginalHook(AstralFlow);
                }

                // Fester Logic
                if (ActionReady(Fester) && !HasStatusEffect(Buffs.TitansFavor))
                {
                    if (LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                            return OriginalHook(Fester);
                    }
                    else
                        return OriginalHook(Fester);
                }

                // Searing Flash
                if (HasStatusEffect(Buffs.RubysGlimmer))
                    return SearingFlash;

                // Lux Solaris
                if (ActionReady(LuxSolaris) &&
                   (PlayerHealthPercentageHp() < 100 || (GetStatusEffectRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)))
                    return OriginalHook(LuxSolaris);

                // Self Shield Overcap
                if (!HasStatusEffect(Buffs.SearingLight) && !HasStatusEffect(Buffs.TitansFavor) &&
                    GetRemainingCharges(RadiantAegis) == 2 && ActionReady(RadiantAegis))
                    return RadiantAegis;

                // Lucid Dreaming
                if (Role.CanLucidDream(5000))
                    return Role.LucidDreaming;
            }
            #endregion

            #region Demi Summon
            if (PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
                return SearingBurstDriftCheck
                    ? OriginalHook(Ruin)
                    : OriginalHook(Aethercharge);
            #endregion

            #region Titan Phase
            if (IsTitanAttuned || OriginalHook(AstralFlow) is MountainBuster) //Titan attunement ends before last mountian buster
            {
                if (ActionReady(AstralFlow) && SummonerWeave)
                    return OriginalHook(AstralFlow);

                if (GemshineReady)
                    return OriginalHook(Gemshine);
            }
            #endregion

            #region Garuda Phase
            if (IsGarudaAttuned || OriginalHook(AstralFlow) is Slipstream)
            {
                if (HasStatusEffect(Buffs.GarudasFavor) && (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
                    return OriginalHook(AstralFlow);

                if (GemshineReady)
                    return OriginalHook(Gemshine);

                if (ActionReady(Ruin4) && IsMoving())
                    return Ruin4;
            }
            #endregion

            #region Ifrit Phase

            if (IsIfritAttuned || OriginalHook(AstralFlow) is CrimsonCyclone or CrimsonStrike)
            {
                if (GemshineReady && (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
                    return OriginalHook(Gemshine);

                if (HasStatusEffect(Buffs.IfritsFavor) || HasStatusEffect(Buffs.CrimsonStrike) && InMeleeRange())
                    return OriginalHook(AstralFlow);

                if (ActionReady(Ruin4) && !HasStatusEffect(Role.Buffs.Swiftcast) && GemshineReady)
                    return Ruin4;
            }
            #endregion

            #region Egi Priority
            // Egi Order
            if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
            {
                if (Gauge.IsTitanReady)
                    return OriginalHook(SummonTopaz);

                if (Gauge.IsGarudaReady)
                    return OriginalHook(SummonEmerald);

                if (Gauge.IsIfritReady)
                    return OriginalHook(SummonRuby);
            }
            #endregion

            #region Ruin 4 Dump
            // Ruin 4 Dump
            if (LevelChecked(Ruin4) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0 && HasStatusEffect(Buffs.FurtherRuin))
                return Ruin4;
            #endregion

            return actionID;
        }
    }
    internal class SMN_Simple_Combo_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_AoE_Simple_Combo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Outburst or Tridisaster))
                return actionID;

            if (NeedToSummon && ActionReady(SummonCarbuncle))
                return SummonCarbuncle;

            #region Variables
            var dontWaitForSearing = SearingCD > (Gauge.SummonTimerRemaining / 1000f) + GCDTotal;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCD
            //Emergency Demi Attack Dump, Probably not needed anymore without burst delay selection
            if (DemiExists && Gauge.SummonTimerRemaining <= 2500)
            {
                if (ActionReady(OriginalHook(EnkindleBahamut)))
                    return OriginalHook(EnkindleBahamut);

                if (ActionReady(AstralFlow) && DemiNotPheonix)
                    return OriginalHook(AstralFlow);
            }

            if (SummonerWeave)
            {
                // Searing Light
                if (ActionReady(SearingLight) && !HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                {
                    if (TraitLevelChecked(Traits.EnhancedDreadwyrmTrance))
                    {
                        if (DemiExists)
                            return SearingLight;
                    }
                    else if (!ActionReady(OriginalHook(Aethercharge)))
                        return SearingLight;
                }

                // Energy Drain
                if (!Gauge.HasAetherflowStacks && ActionReady(EnergyDrain))
                {
                    if (LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || SearingCD > 30)
                            return OriginalHook(EnergySiphon);
                    }
                    else
                        return LevelChecked(EnergySiphon) ? EnergySiphon : EnergyDrain; ;
                }

                // Demi Nuke
                if (DemiExists &&
                    (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || dontWaitForSearing) &&
                    (GetCooldown(EnergyDrain).CooldownRemaining >= 3 || !ActionReady(OriginalHook(Painflare))))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow))
                        return OriginalHook(AstralFlow);
                }

                // Fester Logic
                if (ActionReady(OriginalHook(Fester)) && !HasStatusEffect(Buffs.TitansFavor))
                {
                    if (LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                            return OriginalHook(Painflare);
                    }
                    else
                        return LevelChecked(Painflare) ? OriginalHook(Painflare) : OriginalHook(Fester);
                }

                // Searing Flash
                if (HasStatusEffect(Buffs.RubysGlimmer))
                    return SearingFlash;

                // Lux Solaris
                if (ActionReady(LuxSolaris) &&
                    (PlayerHealthPercentageHp() < 100 || (GetStatusEffectRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)))
                    return OriginalHook(LuxSolaris);

                // Self Shield Overcap
                if (!HasStatusEffect(Buffs.SearingLight) && !HasStatusEffect(Buffs.TitansFavor) &&
                    GetRemainingCharges(RadiantAegis) == 2 && ActionReady(RadiantAegis))
                    return RadiantAegis;

                // Lucid Dreaming
                if (Role.CanLucidDream(5000))
                    return Role.LucidDreaming;
            }
            #endregion

            #region Demi Summon
            // Demi
            if (PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
                return SearingBurstDriftCheck
                    ? OriginalHook(Outburst)
                    : OriginalHook(Aethercharge);
            #endregion

            #region Titan Phase
            if (IsTitanAttuned || OriginalHook(AstralFlow) is MountainBuster)
            {
                if (ActionReady(AstralFlow) && SummonerWeave)
                    return OriginalHook(AstralFlow);

                if (GemshineReady)
                    return OriginalHook(PreciousBrilliance);
            }
            #endregion

            #region Garuda Phase
            if (IsGarudaAttuned || OriginalHook(AstralFlow) is Slipstream)
            {
                if (HasStatusEffect(Buffs.GarudasFavor) && (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
                    return OriginalHook(AstralFlow);

                if (GemshineReady)
                    return OriginalHook(PreciousBrilliance);

                if (ActionReady(Ruin4) && IsMoving())
                    return Ruin4;
            }
            #endregion

            #region Ifrit Phase

            if (IsIfritAttuned || OriginalHook(AstralFlow) is CrimsonCyclone or CrimsonStrike)
            {
                if (GemshineReady && (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
                    return OriginalHook(PreciousBrilliance);

                if (HasStatusEffect(Buffs.IfritsFavor) || HasStatusEffect(Buffs.CrimsonStrike) && InMeleeRange())
                    return OriginalHook(AstralFlow);

                if (ActionReady(Ruin4) && !HasStatusEffect(Role.Buffs.Swiftcast) && GemshineReady)
                    return Ruin4;
            }
            #endregion

            // Egi Order
            if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
            {
                if (Gauge.IsTitanReady)
                    return OriginalHook(SummonTopaz);

                if (Gauge.IsGarudaReady)
                    return OriginalHook(SummonEmerald);

                if (Gauge.IsIfritReady)
                    return OriginalHook(SummonRuby);
            }

            // Ruin 4 Dump
            if (LevelChecked(Ruin4) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0 && HasStatusEffect(Buffs.FurtherRuin))
                return Ruin4;

            return actionID;
        }
    }
    #endregion

    #region Advanced
    internal class SMN_ST_Advanced_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_ST_Advanced_Combo;
        protected override uint Invoke(uint actionID)
        {
            bool allRuins = SMN_ST_Advanced_Combo_AltMode == 0;
            bool actionFound = allRuins && AllRuinsList.Contains(actionID) ||
                               !allRuins && NotRuin3List.Contains(actionID);
            if (!actionFound)
                return actionID;

            if (NeedToSummon)
                return SummonCarbuncle;

            #region Variables
            int lucidThreshold = SMN_ST_Lucid;
            int swiftcastPhase = SMN_ST_SwiftcastPhase;
            bool TitanAstralFlow = IsEnabled(Preset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && SMN_ST_Egi_AstralFlow[0];
            bool IfritAstralFlowCyclone = IsEnabled(Preset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && SMN_ST_Egi_AstralFlow[1];
            bool IfritAstralFlowStrike = IsEnabled(Preset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && SMN_ST_Egi_AstralFlow[3];
            bool GarudaAstralFlow = IsEnabled(Preset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && SMN_ST_Egi_AstralFlow[2];
            var dontWaitForSearing = SearingCD > (Gauge.SummonTimerRemaining / 1000f) + GCDTotal;
            var replacedActions = allRuins ? AllRuinsList.ToArray() : NotRuin3List.ToArray();
            #endregion

            #region Opener
            if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Balance_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCD
            //Emergency Demi Attack Dump, Probably not needed anymore without burst delay selection
            if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && DemiExists && Gauge.SummonTimerRemaining <= 2500)
            {
                if (ActionReady(OriginalHook(EnkindleBahamut)))
                    return OriginalHook(EnkindleBahamut);

                if (ActionReady(AstralFlow) && DemiNotPheonix)
                    return OriginalHook(AstralFlow);
            }

            if (SummonerWeave)
            {
                // Searing Light
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_SearingLight) && ActionReady(SearingLight) && !HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                {
                    if (IsEnabled(Preset.SMN_ST_Advanced_Combo_SearingLight_Burst) && TraitLevelChecked(Traits.EnhancedDreadwyrmTrance))
                    {
                        if (DemiExists)
                            return SearingLight;
                    }
                    else if (!ActionReady(OriginalHook(Aethercharge)))
                        return SearingLight;
                }

                // Energy Drain
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_EDFester) && !Gauge.HasAetherflowStacks && ActionReady(EnergyDrain))
                {
                    if (IsEnabled(Preset.SMN_ST_Advanced_Combo_oGCDPooling) && LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || SearingCD > 30)
                            return OriginalHook(EnergyDrain);
                    }
                    else if (!ActionReady(SearingLight))
                        return OriginalHook(EnergyDrain);
                }

                // Demi Nuke
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && DemiExists && (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || dontWaitForSearing) && (GetCooldown(EnergyDrain).CooldownRemaining >= 3 || !ActionReady(Fester)))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow))
                    {
                        if (DemiNotPheonix)
                            return OriginalHook(AstralFlow);

                        if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle) && DemiPheonix)
                        {
                            if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle_Retarget))
                                return OriginalHook(AstralFlow).Retarget(replacedActions,
                                    SimpleTarget.TargetsTarget.IfInParty() ??
                                    SimpleTarget.AnyTank.IfMissingHP() ??
                                    SimpleTarget.LowestHPPAlly.IfMissingHP() ??
                                    SimpleTarget.Self);
                            else return OriginalHook(AstralFlow);
                        }
                    }
                }

                // Fester Logic
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_EDFester) && ActionReady(Fester) && !HasStatusEffect(Buffs.TitansFavor))
                {
                    if (IsEnabled(Preset.SMN_ST_Advanced_Combo_oGCDPooling) && LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                            return OriginalHook(Fester);
                    }
                    else
                        return OriginalHook(Fester);
                }

                // Searing Flash
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_SearingFlash) && HasStatusEffect(Buffs.RubysGlimmer))
                    return SearingFlash;

                // Lux Solaris
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris) && ActionReady(LuxSolaris) &&
                    (PlayerHealthPercentageHp() < 100 || (GetStatusEffectRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)))
                    return OriginalHook(LuxSolaris);

                // Self Shield Overcap
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Radiant) &&
                    !HasStatusEffect(Buffs.SearingLight) && !HasStatusEffect(Buffs.TitansFavor) &&
                    GetRemainingCharges(RadiantAegis) == 2 && ActionReady(RadiantAegis))
                    return RadiantAegis;

                // Lucid Dreaming
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Lucid) && Role.CanLucidDream(lucidThreshold))
                    return Role.LucidDreaming;
                
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Addle) && 
                    Role.CanAddle()  &&
                    RaidWideCasting())
                    return Role.Addle;
            }
            #endregion

            #region Demi Summon
            // Demi
            if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons) && PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
                return IsEnabled(Preset.SMN_ST_Advanced_Combo_SearingLight_Burst) && SearingBurstDriftCheck
                    ? OriginalHook(Ruin)
                    : OriginalHook(Aethercharge);
            #endregion

            #region Titan Phase
            if (IsTitanAttuned || OriginalHook(AstralFlow) is MountainBuster) //Titan attunement ends before last mountian buster
            {
                if (TitanAstralFlow && ActionReady(AstralFlow) && SummonerWeave)
                    return OriginalHook(AstralFlow);

                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) && GemshineReady)
                    return OriginalHook(Gemshine);
            }
            #endregion

            #region Garuda Phase
            if (IsGarudaAttuned || OriginalHook(AstralFlow) is Slipstream)
            {
                if (GarudaAstralFlow && HasStatusEffect(Buffs.GarudasFavor))
                {
                    if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) && swiftcastPhase is 1 or 3 && Role.CanSwiftcast()) // Forced Swiftcast option
                        return Role.Swiftcast;

                    if (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast))
                        return OriginalHook(AstralFlow);
                }

                #region Special Ruin 3 rule lvl 54 - 72
                // Use Ruin III instead of Emerald Ruin III if enabled and Ruin Mastery III is not active
                if (IsEnabled(Preset.SMN_ST_Ruin3_Emerald_Ruin3) && !TraitLevelChecked(Traits.RuinMastery3) && LevelChecked(Ruin3) && !IsMoving())
                    return Ruin3;

                #endregion

                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) && GemshineReady)
                    return OriginalHook(Gemshine);

                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Ruin4) && HasStatusEffect(Buffs.FurtherRuin) && IsMoving())
                    return Ruin4;
            }
            #endregion

            #region Ifrit Phase
            if (IsIfritAttuned || OriginalHook(AstralFlow) is CrimsonCyclone or CrimsonStrike)
            {
                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) && swiftcastPhase is 2 or 3 && Role.CanSwiftcast())
                    return Role.Swiftcast;

                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) && GemshineReady &&
                    (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
                    return OriginalHook(Gemshine);

                if (IfritAstralFlowCyclone && HasStatusEffect(Buffs.IfritsFavor) &&
                    GetTargetDistance() <= SMN_ST_CrimsonCycloneMeleeDistance  //Melee Check
                   || IfritAstralFlowStrike && HasStatusEffect(Buffs.CrimsonStrike) && InMeleeRange()) //After Strike
                    return OriginalHook(AstralFlow);

                if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Ruin4) && HasStatusEffect(Buffs.FurtherRuin) && !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Ruin4;
            }
            #endregion

            #region Egi Priority
            foreach (var prio in SMN_ST_Egi_Priority.Items.OrderBy(x => x))
            {
                var index = SMN_ST_Egi_Priority.IndexOf(prio);
                var config = GetMatchingConfigST(index, OptionalTarget,
                    out var spell, out var enabled);

                if (!enabled) continue;

                if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
                    return spell;
            }
            #endregion

            #region Ruin 4 Dump
            if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Ruin4) && !IsAttunedAny && DemiNone && HasStatusEffect(Buffs.FurtherRuin))
                return Ruin4;
            #endregion

            return actionID;
        }
    }

    internal class SMN_Advanced_Combo_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_AoE_Advanced_Combo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Outburst or Tridisaster))
                return actionID;

            if (NeedToSummon)
                return SummonCarbuncle;

            #region Variables
            int lucidThreshold = SMN_AoE_Lucid;
            int swiftcastPhase = SMN_AoE_SwiftcastPhase;
            bool TitanAstralFlow = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && SMN_AoE_Egi_AstralFlow[0];
            bool IfritAstralFlowCyclone = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && SMN_AoE_Egi_AstralFlow[1];
            bool IfritAstralFlowStrike = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && SMN_AoE_Egi_AstralFlow[3];
            bool GarudaAstralFlow = IsEnabled(Preset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && SMN_AoE_Egi_AstralFlow[2];
            var dontWaitForSearing = SearingCD > (Gauge.SummonTimerRemaining / 1000f) + GCDTotal;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCD
            //Emergency Demi Attack Dump, Probably not needed anymore without burst delay selection
            if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && DemiExists && Gauge.SummonTimerRemaining <= 2500)
            {
                if (ActionReady(OriginalHook(EnkindleBahamut)))
                    return OriginalHook(EnkindleBahamut);

                if (ActionReady(AstralFlow) && DemiNotPheonix)
                    return OriginalHook(AstralFlow);
            }

            if (SummonerWeave)
            {
                // Searing Light
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_SearingLight) && ActionReady(SearingLight) && !HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                {
                    if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_SearingLight_Burst) && TraitLevelChecked(Traits.EnhancedDreadwyrmTrance))
                    {
                        if (DemiExists)
                            return SearingLight;
                    }
                    else if (!ActionReady(OriginalHook(Aethercharge)))
                        return SearingLight;
                }

                // Energy Drain
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_ESPainflare) && !Gauge.HasAetherflowStacks && ActionReady(EnergyDrain))
                {
                    if (IsEnabled(Preset.SMN_ST_Advanced_Combo_oGCDPooling) && LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || SearingCD > 30)
                            return OriginalHook(EnergySiphon);
                    }
                    else if (!ActionReady(SearingLight))
                        return LevelChecked(EnergySiphon) ? EnergySiphon : EnergyDrain; ;
                }

                // Demi Nuke
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && DemiExists &&
                    (HasStatusEffect(Buffs.SearingLight, anyOwner: true) || dontWaitForSearing) &&
                    (GetCooldown(EnergyDrain).CooldownRemaining >= 3 || !ActionReady(OriginalHook(Painflare))))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow))
                    {
                        if (DemiNotPheonix)
                            return OriginalHook(AstralFlow);

                        if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle) && DemiPheonix)
                        {
                            if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle_Retarget))
                                return OriginalHook(AstralFlow).Retarget([Outburst, Tridisaster],
                                    SimpleTarget.TargetsTarget.IfInParty() ??
                                    SimpleTarget.AnyTank.IfMissingHP() ??
                                    SimpleTarget.LowestHPPAlly.IfMissingHP() ??
                                    SimpleTarget.Self);
                            else return OriginalHook(AstralFlow);
                        }
                    }
                }

                // Fester Logic
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_ESPainflare) && ActionReady(OriginalHook(Fester)) && !HasStatusEffect(Buffs.TitansFavor))
                {
                    if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_oGCDPooling) && LevelChecked(SearingLight))
                    {
                        if (HasStatusEffect(Buffs.SearingLight, anyOwner: true))
                            return OriginalHook(Painflare);
                    }
                    else
                        return LevelChecked(Painflare) ? OriginalHook(Painflare) : OriginalHook(Fester);
                }


                // Searing Flash
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_SearingFlash) && HasStatusEffect(Buffs.RubysGlimmer))
                    return SearingFlash;

                // Lux Solaris
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris) && ActionReady(LuxSolaris) &&
                    (PlayerHealthPercentageHp() < 100 || (GetStatusEffectRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)))
                    return OriginalHook(LuxSolaris);

                // Self Shield Overcap
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_Radiant) &&
                    !HasStatusEffect(Buffs.SearingLight) && !HasStatusEffect(Buffs.TitansFavor) &&
                    GetRemainingCharges(RadiantAegis) == 2 && ActionReady(RadiantAegis))
                    return RadiantAegis;

                // Lucid Dreaming
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_Lucid) && Role.CanLucidDream(lucidThreshold))
                    return Role.LucidDreaming;
            }
            #endregion

            #region Demi Summon
            // Demi
            if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiSummons) && PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
                return IsEnabled(Preset.SMN_AoE_Advanced_Combo_SearingLight_Burst) && SearingBurstDriftCheck
                    ? OriginalHook(Outburst)
                    : OriginalHook(Aethercharge);
            #endregion

            #region Titan Phase
            if (IsTitanAttuned || OriginalHook(AstralFlow) is MountainBuster) //Titan attunement ends before last mountian buster
            {
                if (TitanAstralFlow && ActionReady(AstralFlow) && SummonerWeave)
                    return OriginalHook(AstralFlow);

                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) && GemshineReady)
                    return OriginalHook(PreciousBrilliance);

            }
            #endregion

            #region Garuda Phase
            if (IsGarudaAttuned || OriginalHook(AstralFlow) is Slipstream)
            {
                if (GarudaAstralFlow && HasStatusEffect(Buffs.GarudasFavor))
                {
                    if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) && swiftcastPhase is 1 or 3 && Role.CanSwiftcast()) // Forced Swiftcast option
                        return Role.Swiftcast;

                    if (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast))
                        return OriginalHook(AstralFlow);
                }

                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) && GemshineReady)
                    return OriginalHook(PreciousBrilliance);

                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_Ruin4) && HasStatusEffect(Buffs.FurtherRuin) && IsMoving())
                    return Ruin4;
            }

            #endregion

            #region Ifrit Phase
            if (IsIfritAttuned || OriginalHook(AstralFlow) is CrimsonCyclone or CrimsonStrike)
            {
                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) && swiftcastPhase is 2 or 3 && (Role.CanSwiftcast()))
                    return Role.Swiftcast;

                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) && GemshineReady &&
                    (!IsMoving() || HasStatusEffect(Role.Buffs.Swiftcast)))
                    return OriginalHook(PreciousBrilliance);

                if (IfritAstralFlowCyclone && HasStatusEffect(Buffs.IfritsFavor) &&
                   GetTargetDistance() <= SMN_AoE_CrimsonCycloneMeleeDistance //Melee Check
                   || IfritAstralFlowStrike && HasStatusEffect(Buffs.CrimsonStrike) && InMeleeRange()) //After Strike
                    return OriginalHook(AstralFlow);

                if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_Ruin4) && HasStatusEffect(Buffs.FurtherRuin) && !HasStatusEffect(Role.Buffs.Swiftcast))
                    return Ruin4;
            }
            #endregion

            #region Egi Priority
            foreach (var prio in SMN_AoE_Egi_Priority.Items.OrderBy(x => x))
            {
                var index = SMN_AoE_Egi_Priority.IndexOf(prio);
                var config = GetMatchingConfigAoE(index, OptionalTarget,
                    out var spell, out var enabled);

                if (!enabled) continue;

                if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
                    return spell;
            }
            #endregion

            #region Ruin 4 Dump
            // Ruin 4 Dump
            if (IsEnabled(Preset.SMN_AoE_Advanced_Combo_Ruin4) && !IsAttunedAny && DemiNone && HasStatusEffect(Buffs.FurtherRuin))
                return Ruin4;
            #endregion

            return actionID;
        }
    }
    #endregion
}
