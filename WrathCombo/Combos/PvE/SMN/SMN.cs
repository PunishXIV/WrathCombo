using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvE;

internal partial class SMN
{
    internal class SMN_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_Raise;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != All.Swiftcast)
                return actionID;

            if (HasEffect(All.Buffs.Swiftcast) && IsEnabled(CustomComboPreset.SMN_Variant_Raise) && IsEnabled(Variant.VariantRaise))
                return Variant.VariantRaise;

            if (IsOnCooldown(All.Swiftcast))
                return Resurrection;
            return actionID;
        }
    }

    internal class SMN_RuinMobility : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_RuinMobility;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Ruin4)
                return actionID;
            bool furtherRuin = HasEffect(Buffs.FurtherRuin);

            if (!furtherRuin)
                return Ruin3;
            return actionID;
        }
    }

    internal class SMN_EDFester : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_EDFester;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Fester or Necrotize))
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();
            if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergyDrain) && !gauge.HasAetherflowStacks && IsEnabled(CustomComboPreset.SMN_EDFester_Ruin4))
                return Ruin4;

            if (LevelChecked(EnergyDrain) && !gauge.HasAetherflowStacks)
                return EnergyDrain;

            return actionID;
        }
    }

    internal class SMN_ESPainflare : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_ESPainflare;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Painflare)
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();

            if (!LevelChecked(Painflare) || gauge.HasAetherflowStacks)
                return actionID;

            if (HasEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergySiphon) && IsEnabled(CustomComboPreset.SMN_ESPainflare_Ruin4))
                return Ruin4;

            if (LevelChecked(EnergySiphon))
                return EnergySiphon;

            if (LevelChecked(EnergyDrain))
                return EnergyDrain;

            return actionID;
        }
    }

    internal class SMN_Simple_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_ST_Simple_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2))
                return actionID;

            if (IsEnabled(CustomComboPreset.SMN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SMN_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.SMN_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (NeedToSummon && ActionReady(SummonCarbuncle))
                return SummonCarbuncle;

            if (CanWeave())
            {
                if (ActionReady(SearingLight) && !HasEffect(Buffs.RubysGlimmer) && (CurrentDemiSummon is DemiSummon.Dreadwyrm or DemiSummon.Bahamut or DemiSummon.SolarBahamut))
                    return SearingLight;

                if (!Gauge.HasAetherflowStacks && ActionReady(EnergyDrain))
                    return EnergyDrain;

                if (ActionReady(SearingFlash) && HasEffect(Buffs.RubysGlimmer))
                    return SearingFlash;

                if (CurrentDemiSummon is DemiSummon.Bahamut or DemiSummon.Phoenix or DemiSummon.SolarBahamut)
                {
                    if (ActionReady(EnkindleBahamut))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow))
                        return OriginalHook(AstralFlow);

                    if (ActionReady(LuxSolaris))
                        return OriginalHook(LuxSolaris);
                }

                if (ActionReady(Fester))
                {
                    if (!LevelChecked(SearingLight) || HasEffect(Buffs.SearingLight) || GetCooldown(EnergyDrain).CooldownRemaining < 6)
                        return OriginalHook(Fester);
                }

                if (ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 4000)
                    return All.LucidDreaming;
            }

            if (ActionReady(Aethercharge))
                return OriginalHook(Aethercharge);

            if (ActionReady(Slipstream) || ActionReady(RubyRite))
            {
                if (ActionReady(All.Swiftcast))
                    return All.Swiftcast;

                if (HasEffect(All.Buffs.Swiftcast))
                {
                    if (ActionReady(Slipstream))
                        return OriginalHook(Slipstream);

                    if (ActionReady(RubyRite))
                        return RubyRite;
                }
            }

            if ((HasEffect(Buffs.GarudasFavor) && Gauge.Attunement is 0) ||
                (HasEffect(Buffs.TitansFavor) && ComboAction is TopazRite or TopazCata && CanSpellWeave()) ||
                (HasEffect(Buffs.IfritsFavor) && (IsMoving() || Gauge.Attunement is 0)) || (ComboAction == CrimsonCyclone && InMeleeRange()))
                return OriginalHook(AstralFlow);

            if (HasEffect(Buffs.FurtherRuin) && ((!HasEffect(All.Buffs.Swiftcast) && IsIfritAttuned && IsMoving()) || (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
                return Ruin4;

            if (IsAttunedAny)
                return OriginalHook(Gemshine);

            if (Gauge.SummonTimerRemaining == 0)
            {
                if (ActionReady(SummonTopaz))
                    return OriginalHook(SummonTopaz);

                if (ActionReady(SummonEmerald))
                    return OriginalHook(SummonEmerald);

                if (ActionReady(SummonRuby))
                    return OriginalHook(SummonRuby);
            }

            if (LevelChecked(Ruin4) && !IsAttunedAny && CurrentDemiSummon is DemiSummon.None && HasEffect(Buffs.FurtherRuin))
                return Ruin4;

            return actionID;
        }
    }

    internal class SMN_Simple_Combo_AoE : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_AoE_Simple_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Outburst)
                return actionID;

            if (IsEnabled(CustomComboPreset.SMN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SMN_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.SMN_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (NeedToSummon && ActionReady(SummonCarbuncle))
                return SummonCarbuncle;

            if (CanSpellWeave())
            {
                if (ActionReady(SearingLight) && !HasEffect(Buffs.RubysGlimmer) && (CurrentDemiSummon is DemiSummon.Dreadwyrm or DemiSummon.Bahamut or DemiSummon.SolarBahamut))
                    return SearingLight;

                if (!Gauge.HasAetherflowStacks)
                {
                    if (ActionReady(EnergySiphon))
                        return EnergySiphon;

                    if (ActionReady(EnergyDrain))
                        return EnergyDrain;
                }

                if (ActionReady(SearingFlash) && HasEffect(Buffs.RubysGlimmer))
                    return SearingFlash;

                if (CurrentDemiSummon is not DemiSummon.None)
                {
                    if (ActionReady(EnkindleBahamut))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow))
                        return OriginalHook(AstralFlow);

                    if (ActionReady(LuxSolaris))
                        return OriginalHook(LuxSolaris);
                }

                if (ActionReady(Painflare))
                {
                    if (!LevelChecked(SearingLight) || HasEffect(Buffs.SearingLight) || GetCooldown(EnergyDrain).CooldownRemaining < 6)
                        return Painflare;
                }

                if (ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= 4000)
                    return All.LucidDreaming;
            }

            if (ActionReady(Aethercharge))
                return OriginalHook(Aethercharge);

            if (ActionReady(Slipstream) || ActionReady(RubyCata))
            {
                if (ActionReady(All.Swiftcast))
                    return All.Swiftcast;

                if (HasEffect(All.Buffs.Swiftcast))
                {
                    if (ActionReady(Slipstream))
                        return OriginalHook(Slipstream);

                    if (ActionReady(RubyCata))
                        return RubyCata;
                }
            } 

            if ((HasEffect(Buffs.GarudasFavor) && Gauge.Attunement is 0) ||
                (HasEffect(Buffs.TitansFavor) && ComboAction is TopazRite or TopazCata && CanSpellWeave()) ||
                (HasEffect(Buffs.IfritsFavor) && (IsMoving() || Gauge.Attunement is 0)) || (ComboAction == CrimsonCyclone && InMeleeRange()))
                return OriginalHook(AstralFlow);

            if (HasEffect(Buffs.FurtherRuin) && ((!HasEffect(All.Buffs.Swiftcast) && IsIfritAttuned && IsMoving()) || (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
                return Ruin4;

            if (IsAttunedAny && ActionReady(PreciousBrilliance))
                return OriginalHook(PreciousBrilliance);

            if (Gauge.SummonTimerRemaining == 0)
            { 
                if (ActionReady(SummonTopaz))
                    return OriginalHook(SummonTopaz);

                if (ActionReady(SummonEmerald))
                    return OriginalHook(SummonEmerald);
            
                if (ActionReady(SummonRuby))
                    return OriginalHook(SummonRuby);
            }

            if (LevelChecked(Ruin4) && !IsAttunedAny && CurrentDemiSummon is DemiSummon.None && HasEffect(Buffs.FurtherRuin))
                return Ruin4;

            return actionID;
        }
    }
    internal class SMN_ST_Advanced_Combo : CustomCombo
    {
        internal static int DemiAttackCount = 0;
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_ST_Advanced_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2))
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();
            int summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_PrimalChoice);
            int SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_BurstPhase);
            int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_Lucid);
            int swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_SwiftcastPhase);
            int burstDelay = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_Burst_Delay);

            bool TitanAstralFlow = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[0];
            bool IfritAstralFlow = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[1];
            bool GarudaAstralFlow = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[2];

            DemiAttackCount = TimesUsedSinceOtherAction(OriginalHook(Aethercharge), [AstralImpulse, UmbralImpulse, FountainOfFire, AstralFlare, UmbralFlare, BrandOfPurgatory]);

            if (IsEnabled(CustomComboPreset.SMN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SMN_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Balance_Opener) && Opener().FullOpener(ref actionID))
                return actionID;

            if (IsEnabled(CustomComboPreset.SMN_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (CanSpellWeave())
            {
                // Searing Light
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight) && IsOffCooldown(SearingLight) && LevelChecked(SearingLight))
                {
                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight_Burst))
                    {
                        if ((SummonerBurstPhase is 0 or 1 && (ActionReady(AstralImpulse) || ActionReady(UmbralImpulse))) ||
                            (SummonerBurstPhase == 2 && ActionReady(FountainOfFire)) ||
                            SummonerBurstPhase == 3 && ActionReady(AstralImpulse) || ActionReady(UmbralImpulse) || ActionReady(FountainOfFire) ||
                            (SummonerBurstPhase == 4))
                            return SearingLight;
                    }
                    else
                        return SearingLight;
                }

                // Emergency priority Demi Nuke to prevent waste if you can't get demi attacks out to satisfy the slider check.
                if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire &&
                    IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && GetCooldown(OriginalHook(Aethercharge)).CooldownElapsed >= 12.5)
                {
                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks))
                    {
                        if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                            return OriginalHook(EnkindleBahamut);

                        if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse)
                            return OriginalHook(AstralFlow);

                        if (IsOffCooldown(OriginalHook(EnkindlePhoenix)) && LevelChecked(SummonPhoenix))
                            return OriginalHook(EnkindlePhoenix);

                        if (IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Rekindle))
                            if (IsOffCooldown(Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                                return OriginalHook(AstralFlow);

                        if (IsOffCooldown(OriginalHook(EnkindleSolarBahamut)) && LevelChecked(SummonSolarBahamut))
                            return OriginalHook(EnkindleSolarBahamut);

                        if (IsOffCooldown(Sunflare) && LevelChecked(Sunflare) && OriginalHook(Ruin) is UmbralImpulse)
                            return OriginalHook(AstralFlow);
                    }
                }

                // Energy Drain
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EDFester) && !gauge.HasAetherflowStacks && IsOffCooldown(EnergyDrain) && LevelChecked(EnergyDrain) &&
                    (!LevelChecked(DreadwyrmTrance) || DemiAttackCount >= burstDelay))
                    return EnergyDrain;

                // First set of Festers if Energy Drain is close to being off CD, or off CD while you have aetherflow stacks.
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EDFester) && IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling) && gauge.HasAetherflowStacks)
                {
                    if (GetCooldown(EnergyDrain).CooldownRemaining <= 3.2)
                    {
                        if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                             (SummonerBurstPhase is not 4)) ||
                            (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                            return OriginalHook(Fester);

                    }
                }

                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingFlash) && HasEffect(Buffs.RubysGlimmer) && LevelChecked(SearingFlash))
                    return SearingFlash;

                // Demi Nuke
                if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire)
                {
                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && IsBahamutReady && (LevelChecked(SummonSolarBahamut) || DemiAttackCount >= burstDelay)
                        && (IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight_Burst) || LevelChecked(SummonSolarBahamut) || HasEffect(Buffs.SearingLight)))
                    {
                        if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                            return OriginalHook(EnkindleBahamut);

                        if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse)
                            return OriginalHook(AstralFlow);
                    }

                    // Demi Nuke 2: Electric Boogaloo
                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && IsPhoenixReady)
                    {
                        if (IsOffCooldown(OriginalHook(EnkindlePhoenix)) && LevelChecked(SummonPhoenix) && OriginalHook(Ruin) is FountainOfFire)
                            return OriginalHook(EnkindlePhoenix);

                        if (IsOffCooldown(Rekindle) && IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                            return OriginalHook(AstralFlow);
                    }

                    // Demi Nuke 3: More Boogaloo
                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && IsSolarBahamutReady && DemiAttackCount >= burstDelay &&
                        (IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight_Burst) || HasEffect(Buffs.SearingLight)))
                    {
                        if (IsOffCooldown(OriginalHook(EnkindleSolarBahamut)) && LevelChecked(SummonSolarBahamut))
                            return OriginalHook(EnkindleSolarBahamut);

                        if (IsOffCooldown(Sunflare) && LevelChecked(Sunflare) && OriginalHook(Ruin) is UmbralImpulse)
                            return OriginalHook(AstralFlow);
                    }
                }

                // Lux Solaris
                if (IsOffCooldown(LuxSolaris) && IsEnabled(CustomComboPreset.SMN_Advanced_Combo_DemiSummons_LuxSolaris) && HasEffect(Buffs.RefulgentLux) &&
                    (PlayerHealthPercentageHp() < 100 || GetBuffRemainingTime(Buffs.RefulgentLux) is < 3 and > 0))
                    return OriginalHook(LuxSolaris);
            }

            // Fester
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EDFester))
            {
                if (gauge.HasAetherflowStacks && CanSpellWeave())
                {
                    if (IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling))
                        return OriginalHook(Fester);

                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling))
                    {
                        if (!LevelChecked(SearingLight))
                            return OriginalHook(Fester);

                        if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                             SummonerBurstPhase is 0 or 1 or 2 or 3 && DemiAttackCount >= burstDelay) ||
                            (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                            return OriginalHook(Fester);
                    }
                }
            }

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Lucid) && CanSpellWeave() && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold)
                return All.LucidDreaming;

            // Demi
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons))
            {
                if (InCombat() && gauge.SummonTimerRemaining == 0 && ActionReady(OriginalHook(Aethercharge)))
                    return OriginalHook(Aethercharge);
            }

            //Ruin4 in Egi Phases
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Ruin4) && HasEffect(Buffs.FurtherRuin) &&
                ((!HasEffect(All.Buffs.Swiftcast) && IsMoving() && ((HasEffect(Buffs.GarudasFavor) && !IsGarudaAttuned) || (IsIfritAttuned && ComboAction is not CrimsonCyclone))) ||
                 GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0))
                return Ruin4;

            // Egi Features
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) && LevelChecked(All.Swiftcast))
            {
                // Swiftcast Garuda Feature
                if (swiftcastPhase is 0 or 1 && LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                {
                    if (CanSpellWeave() && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (GarudaAstralFlow &&
                        ((HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast)) || (gauge.Attunement == 0)))     // Astral Flow if Swiftcast is not ready throughout Garuda
                        return OriginalHook(AstralFlow);
                }

                // Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                if (swiftcastPhase == 2)
                {
                    if (IsOffCooldown(All.Swiftcast) && IsIfritAttuned && ComboAction is not CrimsonCyclone)
                    {
                        if (!IfritAstralFlow || (IfritAstralFlow && gauge.Attunement >= 1))
                            return All.Swiftcast;
                    }
                }
                // SpS Swiftcast
                if (swiftcastPhase == 3)
                {
                    // Swiftcast Garuda Feature
                    if (LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                    {
                        if (CanSpellWeave() && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                            return All.Swiftcast;

                        if (GarudaAstralFlow &&
                            ((HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast)) || (gauge.Attunement == 0)))     // Astral Flow if Swiftcast is not ready throughout Garuda
                            return OriginalHook(AstralFlow);
                    }

                    // Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                    if (IsOffCooldown(All.Swiftcast) && IsIfritAttuned && ComboAction is not CrimsonCyclone)
                    {
                        if (!IfritAstralFlow || (IfritAstralFlow && gauge.Attunement >= 1))
                            return All.Swiftcast;
                    }
                }
            }

            // Gemshine priority casting
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) &&
                ((IsIfritAttuned && gauge.Attunement >= 1 && HasEffect(All.Buffs.Swiftcast) && ComboAction is not CrimsonCyclone) ||
                 (HasEffect(Buffs.GarudasFavor) && gauge.Attunement >= 1 && !HasEffect(All.Buffs.Swiftcast) && IsMoving())))
                return OriginalHook(Gemshine);

            if ((GarudaAstralFlow && HasEffect(Buffs.GarudasFavor) && (IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi) || swiftcastPhase == 2)) ||                 // Garuda
                (TitanAstralFlow && HasEffect(Buffs.TitansFavor) && ComboAction is TopazRite or TopazCata && CanSpellWeave()) ||                                  // Titan
                (IfritAstralFlow && ((HasEffect(Buffs.IfritsFavor) && !Config.SMN_ST_CrimsonCycloneMelee && (IsMoving() || gauge.Attunement == 0)) || (ComboAction is CrimsonCyclone && InMeleeRange()))) ||
                (IfritAstralFlow && HasEffect(Buffs.IfritsFavor) && Config.SMN_ST_CrimsonCycloneMelee && InMeleeRange()))  // Ifrit
                return OriginalHook(AstralFlow);

            if (IsGarudaAttuned)
            {
                // Use Ruin III instead of Emerald Ruin III if enabled and Ruin Mastery III is not active
                if (IsEnabled(CustomComboPreset.SMN_ST_Ruin3_Emerald_Ruin3) && !TraitLevelChecked(Traits.RuinMastery3) && LevelChecked(Ruin3))
                {
                    if (!IsMoving())
                        return Ruin3;
                }
            }

            // Gemshine
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) && (IsGarudaAttuned || IsTitanAttuned || IsIfritAttuned))
                return OriginalHook(Gemshine);

            // Egi Order
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_EgiOrder) && gauge.SummonTimerRemaining == 0)
            {
                if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && LevelChecked(SummonRuby))
                    return OriginalHook(SummonRuby);

                if (summonerPrimalChoice is 0 or 1)
                {
                    if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                        return OriginalHook(SummonTopaz);

                    if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                        return OriginalHook(SummonEmerald);
                }

                if (summonerPrimalChoice == 2)
                {
                    if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                        return OriginalHook(SummonEmerald);

                    if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                        return OriginalHook(SummonTopaz);
                }
            }

            // Ruin 4
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Ruin4) && LevelChecked(Ruin4) && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                return Ruin4;

            return actionID;
        }
    }

    internal class SMN_Advanced_Combo_AoE : CustomCombo
    {
        internal static int DemiAttackCount = 0;
        internal static bool UsedDemiAttack = false;
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_AoE_Advanced_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Outburst)
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();
            int summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_PrimalChoice);
            int SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_BurstPhase);
            int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_Lucid);
            int swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_SwiftcastPhase);
            int burstDelay = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_Burst_Delay);
            bool inOpener = CombatEngageDuration().TotalSeconds < 40;
            bool IsGarudaAttuned = OriginalHook(Gemshine) is EmeralRuin1 or EmeralRuin2 or EmeralRuin3 or EmeraldRite;
            bool IsTitanAttuned = OriginalHook(Gemshine) is TopazRuin1 or TopazRuin2 or TopazRuin3 or TopazRite;
            bool IsIfritAttuned = OriginalHook(Gemshine) is RubyRuin1 or RubyRuin2 or RubyRuin3 or RubyRite;
            bool IsBahamutReady = OriginalHook(Aethercharge) is SummonBahamut;
            bool IsPhoenixReady = OriginalHook(Aethercharge) is SummonPhoenix;
            bool IsSolarBahamutReady = OriginalHook(Aethercharge) is SummonSolarBahamut;
            bool TitanAstralFlow = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[0];
            bool IfritAstralFlow = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[1];
            bool GarudaAstralFlow = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[2];

            if (WasLastAction(OriginalHook(Aethercharge)))
                DemiAttackCount = 0;    // Resets counter

            if (IsEnabled(CustomComboPreset.SMN_Advanced_Burst_Delay_Option_AoE) && !inOpener)
                DemiAttackCount = 6; // If SMN_Advanced_Burst_Delay_Option is active and outside opener window, set DemiAttackCount to 6 to ignore delayed oGCDs 

            if (GetCooldown(OriginalHook(Aethercharge)).CooldownElapsed >= 12.5)
                DemiAttackCount = 6; // Sets DemiAttackCount to 6 if for whatever reason you're in a position that you can't demi attack to prevent ogcd waste.

            if (gauge.SummonTimerRemaining == 0 && !InCombat())
                DemiAttackCount = 0;

            //CHECK_DEMIATTACK_USE
            if (UsedDemiAttack == false && ComboAction is AstralImpulse or UmbralImpulse or FountainOfFire or AstralFlare or UmbralFlare or BrandOfPurgatory && DemiAttackCount is not 6 && GetCooldownRemainingTime(AstralImpulse) > 1)
            {
                UsedDemiAttack = true;      // Registers that a Demi Attack was used and blocks further incrementation of DemiAttackCountCount
                DemiAttackCount++;          // Increments DemiAttack counter
            }

            //CHECK_DEMIATTACK_USE_RESET
            if (UsedDemiAttack && GetCooldownRemainingTime(AstralImpulse) < 1)
                UsedDemiAttack = false;  // Resets block to allow CHECK_DEMIATTACK_USE

            if (IsEnabled(CustomComboPreset.SMN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.SMN_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.SMN_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (CanSpellWeave())
            {
                // Searing Light
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight) && IsOffCooldown(SearingLight) && LevelChecked(SearingLight))
                {
                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight_Burst))
                    {
                        if ((SummonerBurstPhase is 0 or 1 && ((!LevelChecked(SummonSolarBahamut) && OriginalHook(Ruin) is AstralImpulse) || OriginalHook(Ruin) is UmbralImpulse) && DemiAttackCount >= 1) ||
                            (SummonerBurstPhase == 2 && OriginalHook(Ruin) == FountainOfFire) ||
                            (SummonerBurstPhase == 3 && OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire) ||
                            (SummonerBurstPhase == 4))
                            return SearingLight;
                    }
                    else
                        return SearingLight;
                }

                // Emergency priority Demi Nuke to prevent waste if you can't get demi attacks out to satisfy the slider check.
                if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire &&
                    IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && GetCooldown(OriginalHook(Aethercharge)).CooldownElapsed >= 12.5)
                {
                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks))
                    {
                        if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                            return OriginalHook(EnkindleBahamut);

                        if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse)
                            return OriginalHook(AstralFlow);

                        if (IsOffCooldown(OriginalHook(EnkindlePhoenix)) && LevelChecked(SummonPhoenix))
                            return OriginalHook(EnkindlePhoenix);

                        if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle))
                            if (IsOffCooldown(Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                                return OriginalHook(AstralFlow);

                        if (IsOffCooldown(OriginalHook(EnkindleSolarBahamut)) && LevelChecked(SummonSolarBahamut))
                            return OriginalHook(EnkindleSolarBahamut);

                        if (IsOffCooldown(Sunflare) && LevelChecked(Sunflare) && OriginalHook(Ruin) is UmbralImpulse)
                            return OriginalHook(AstralFlow);
                    }
                }

                // Energy Siphon
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_ESPainflare) && !gauge.HasAetherflowStacks && IsOffCooldown(EnergySiphon) && LevelChecked(EnergySiphon) &&
                    (!LevelChecked(DreadwyrmTrance) || !inOpener || DemiAttackCount >= burstDelay))
                    return EnergySiphon;

                // First set of Painflares if Energy Siphon is close to being off CD, or off CD while you have aetherflow stacks.
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_ESPainflare) && IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling) && gauge.HasAetherflowStacks && LevelChecked(Painflare))
                {
                    if (GetCooldown(EnergySiphon).CooldownRemaining <= 3.2)
                    {
                        if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                             (SummonerBurstPhase is not 4)) ||
                            (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                            return OriginalHook(Painflare);

                    }
                }

                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingFlash) && HasEffect(Buffs.RubysGlimmer) && LevelChecked(SearingFlash))
                    return SearingFlash;

                // Demi Nuke
                if (OriginalHook(Ruin) is AstralImpulse or UmbralImpulse or FountainOfFire)
                {
                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && IsBahamutReady && (LevelChecked(SummonSolarBahamut) || DemiAttackCount >= burstDelay)
                        && (IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight_Burst) || LevelChecked(SummonSolarBahamut) || HasEffect(Buffs.SearingLight)))
                    {
                        if (IsOffCooldown(OriginalHook(EnkindleBahamut)) && LevelChecked(SummonBahamut))
                            return OriginalHook(EnkindleBahamut);

                        if (IsOffCooldown(Deathflare) && LevelChecked(Deathflare) && OriginalHook(Ruin) is AstralImpulse)
                            return OriginalHook(AstralFlow);
                    }

                    // Demi Nuke 2: Electric Boogaloo
                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && IsPhoenixReady)
                    {
                        if (IsOffCooldown(OriginalHook(EnkindlePhoenix)) && LevelChecked(SummonPhoenix) && OriginalHook(Ruin) is FountainOfFire)
                            return OriginalHook(EnkindlePhoenix);

                        if (IsOffCooldown(Rekindle) && IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle) && OriginalHook(Ruin) is FountainOfFire)
                            return OriginalHook(AstralFlow);
                    }

                    // Demi Nuke 3: More Boogaloo
                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && IsSolarBahamutReady && DemiAttackCount >= burstDelay &&
                        (IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight_Burst) || HasEffect(Buffs.SearingLight)))
                    {
                        if (IsOffCooldown(OriginalHook(EnkindleSolarBahamut)) && LevelChecked(SummonSolarBahamut))
                            return OriginalHook(EnkindleSolarBahamut);

                        if (IsOffCooldown(Sunflare) && LevelChecked(Sunflare) && OriginalHook(Ruin) is UmbralImpulse)
                            return OriginalHook(AstralFlow);
                    }
                }

                // Lux Solaris
                if (IsOffCooldown(LuxSolaris) && IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_LuxSolaris) && HasEffect(Buffs.RefulgentLux) &&
                    (PlayerHealthPercentageHp() < 100 || GetBuffRemainingTime(Buffs.RefulgentLux) is < 3 and > 0))
                    return OriginalHook(LuxSolaris);
            }

            // Painflare
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_ESPainflare) && LevelChecked(Painflare))
            {
                if (gauge.HasAetherflowStacks && CanSpellWeave())
                {
                    if (IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling))
                        return OriginalHook(Painflare);

                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling))
                    {
                        if (!LevelChecked(SearingLight))
                            return OriginalHook(Painflare);

                        if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                             SummonerBurstPhase is 0 or 1 or 2 or 3 && DemiAttackCount >= burstDelay) ||
                            (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                            return OriginalHook(Painflare);
                    }
                }
            }

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Lucid) && CanSpellWeave() && ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= lucidThreshold)
                return All.LucidDreaming;

            // Demi
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons))
            {
                if (InCombat() && gauge.SummonTimerRemaining == 0 && IsOffCooldown(OriginalHook(Aethercharge)) &&
                    ((LevelChecked(Aethercharge) && !LevelChecked(SummonBahamut)) ||   // Pre-Bahamut Phase
                     (IsBahamutReady && LevelChecked(SummonBahamut)) ||            // Bahamut Phase
                     (IsPhoenixReady && LevelChecked(SummonPhoenix)) ||            // Phoenix Phase
                     (IsSolarBahamutReady && LevelChecked(SummonSolarBahamut))))   // Solar Bahamut Phase
                    return OriginalHook(Aethercharge);
            }

            //Ruin4 in Egi Phases
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Ruin4) && HasEffect(Buffs.FurtherRuin) &&
                ((!HasEffect(All.Buffs.Swiftcast) && IsMoving() && ((HasEffect(Buffs.GarudasFavor) && !IsGarudaAttuned) || (IsIfritAttuned && ComboAction is not CrimsonCyclone))) ||
                 GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0))
                return Ruin4;

            // Egi Features
            if (IsEnabled(CustomComboPreset.SMN_AoE_DemiEgiMenu_SwiftcastEgi) && LevelChecked(All.Swiftcast))
            {
                // Swiftcast Garuda Feature
                if (swiftcastPhase is 0 or 1 && LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                {
                    if (CanSpellWeave() && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (Config.SMN_ST_Egi_AstralFlow[2] &&
                        ((HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast)) || (gauge.Attunement == 0)))     // Astral Flow if Swiftcast is not ready throughout Garuda
                        return OriginalHook(AstralFlow);
                }

                // Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                if (swiftcastPhase == 2)
                {
                    if (IsOffCooldown(All.Swiftcast) && IsIfritAttuned && ComboAction is not CrimsonCyclone)
                    {
                        if (!Config.SMN_ST_Egi_AstralFlow[1] || (Config.SMN_ST_Egi_AstralFlow[1] && gauge.Attunement >= 1))
                            return All.Swiftcast;
                    }
                }

                // SpS Swiftcast
                if (swiftcastPhase == 3)
                {
                    // Swiftcast Garuda Feature
                    if (LevelChecked(Slipstream) && HasEffect(Buffs.GarudasFavor))
                    {
                        if (CanSpellWeave() && IsGarudaAttuned && IsOffCooldown(All.Swiftcast))
                            return All.Swiftcast;

                        if (Config.SMN_ST_Egi_AstralFlow[2] &&
                            ((HasEffect(Buffs.GarudasFavor) && HasEffect(All.Buffs.Swiftcast)) || (gauge.Attunement == 0)))     // Astral Flow if Swiftcast is not ready throughout Garuda
                            return OriginalHook(AstralFlow);
                    }

                    // Swiftcast Ifrit Feature (Conditions to allow for SpS Ruins to still be under the effect of Swiftcast)
                    if (IsOffCooldown(All.Swiftcast) && IsIfritAttuned && ComboAction is not CrimsonCyclone)
                    {
                        if (!Config.SMN_ST_Egi_AstralFlow[1] || (Config.SMN_ST_Egi_AstralFlow[1] && gauge.Attunement >= 1))
                            return All.Swiftcast;
                    }
                }
            }

            // Precious Brilliance priority casting
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) && LevelChecked(PreciousBrilliance) &&
                ((IsIfritAttuned && gauge.Attunement >= 1 && HasEffect(All.Buffs.Swiftcast) && ComboAction is not CrimsonCyclone) ||
                 (HasEffect(Buffs.GarudasFavor) && gauge.Attunement >= 1 && !HasEffect(All.Buffs.Swiftcast) && IsMoving())))
                return OriginalHook(PreciousBrilliance);

            if ((Config.SMN_ST_Egi_AstralFlow[2] && HasEffect(Buffs.GarudasFavor) && (IsNotEnabled(CustomComboPreset.SMN_AoE_DemiEgiMenu_SwiftcastEgi) || swiftcastPhase == 2)) ||                 // Garuda
                (Config.SMN_ST_Egi_AstralFlow[0] && HasEffect(Buffs.TitansFavor) && ComboAction is TopazRite or TopazCata && CanSpellWeave()) ||                                  // Titan
                (Config.SMN_ST_Egi_AstralFlow[1] && ((HasEffect(Buffs.IfritsFavor) && !Config.SMN_ST_CrimsonCycloneMelee && (IsMoving() || gauge.Attunement == 0)) || (ComboAction is CrimsonCyclone && InMeleeRange()))) ||
                (Config.SMN_ST_Egi_AstralFlow[1] && HasEffect(Buffs.IfritsFavor) && Config.SMN_ST_CrimsonCycloneMelee && InMeleeRange()))  // Ifrit
                return OriginalHook(AstralFlow);

            // Precious Brilliance
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) && LevelChecked(PreciousBrilliance) && (IsGarudaAttuned || IsTitanAttuned || IsIfritAttuned))
                return OriginalHook(PreciousBrilliance);

            // Egi Order
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_EgiOrder) && gauge.SummonTimerRemaining == 0)
            {
                if (gauge.IsIfritReady && !gauge.IsTitanReady && !gauge.IsGarudaReady && LevelChecked(SummonRuby))
                    return OriginalHook(SummonRuby);

                if (summonerPrimalChoice is 0 or 1)
                {
                    if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                        return OriginalHook(SummonTopaz);

                    if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                        return OriginalHook(SummonEmerald);
                }

                if (summonerPrimalChoice == 2)
                {
                    if (gauge.IsGarudaReady && LevelChecked(SummonEmerald))
                        return OriginalHook(SummonEmerald);

                    if (gauge.IsTitanReady && LevelChecked(SummonTopaz))
                        return OriginalHook(SummonTopaz);
                }
            }

            // Ruin 4
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Ruin4) && LevelChecked(Ruin4) && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                return Ruin4;
            return actionID;
        }
    }

    internal class SMN_CarbuncleReminder : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_CarbuncleReminder;
        
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2 or Ruin3 or DreadwyrmTrance or
                 AstralFlow or EnkindleBahamut or SearingLight or
                 RadiantAegis or Outburst or Tridisaster or
                 PreciousBrilliance or Gemshine))
                return actionID;

            if (NeedToSummon && ActionReady(SummonCarbuncle))
                return SummonCarbuncle;

            return actionID;
        }
    }

    internal class SMN_Egi_AstralFlow : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_Egi_AstralFlow;

        protected override uint Invoke(uint actionID)
        {
            if ((actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 or SummonRuby or SummonIfrit or SummonIfrit2 && HasEffect(Buffs.TitansFavor)) ||
                (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 && HasEffect(Buffs.GarudasFavor)) ||
                (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonRuby or SummonIfrit or SummonIfrit2 && (HasEffect(Buffs.IfritsFavor) || (ComboAction == CrimsonCyclone && InMeleeRange()))))
                return OriginalHook(AstralFlow);

            return actionID;
        }
    }

    internal class SMN_DemiAbilities : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SMN_DemiAbilities;

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
}
