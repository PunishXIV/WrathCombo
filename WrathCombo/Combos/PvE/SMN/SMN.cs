using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvE;

internal partial class SMN : CasterJob
{
    internal class SMN_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_Raise;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            if (Variant.CanRaise(CustomComboPreset.SMN_Variant_Raise))
                return Variant.Raise;

            if (IsOnCooldown(Role.Swiftcast))
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

            if (Variant.CanCure(CustomComboPreset.SMN_Variant_Cure, Config.SMN_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SMN_Variant_Rampart, WeaveTypes.SpellWeave))
                return Variant.Rampart;

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

                if (Role.CanLucidDream(4000))
                    return Role.LucidDreaming;
            }

            if (ActionReady(Aethercharge))
                return OriginalHook(Aethercharge);

            if (ActionReady(Slipstream) || ActionReady(RubyRite))
            {
                if (ActionReady(Role.Swiftcast))
                    return Role.Swiftcast;

                if (HasEffect(Role.Buffs.Swiftcast))
                {
                    if (ActionReady(Slipstream))
                        return OriginalHook(Slipstream);

                    if (ActionReady(RubyRite))
                        return RubyRite;
                }
            }

            if ((HasEffect(Buffs.GarudasFavor) && Gauge.Attunement == 0) ||
                (HasEffect(Buffs.TitansFavor) && CanSpellWeave()) ||
                HasEffect(Buffs.IfritsFavor) || HasEffect(Buffs.CrimsonStrike))
                return OriginalHook(AstralFlow);

            if (HasEffect(Buffs.FurtherRuin) && ((!HasEffect(Role.Buffs.Swiftcast) && IsIfritAttuned && IsMoving()) || (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
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

            if (Variant.CanCure(CustomComboPreset.SMN_Variant_Cure, Config.SMN_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SMN_Variant_Rampart, WeaveTypes.SpellWeave))
                return Variant.Rampart;

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

                if (Role.CanLucidDream(4000))
                    return Role.LucidDreaming;
            }

            if (ActionReady(Aethercharge))
                return OriginalHook(Aethercharge);

            if (ActionReady(Slipstream) || ActionReady(RubyCata))
            {
                if (ActionReady(Role.Swiftcast))
                    return Role.Swiftcast;

                if (HasEffect(Role.Buffs.Swiftcast))
                {
                    if (ActionReady(Slipstream))
                        return OriginalHook(Slipstream);

                    if (ActionReady(RubyCata))
                        return RubyCata;
                }
            }

            if ((HasEffect(Buffs.GarudasFavor) && Gauge.Attunement == 0) ||
                (HasEffect(Buffs.TitansFavor) && CanSpellWeave()) ||
                HasEffect(Buffs.IfritsFavor) || HasEffect(Buffs.CrimsonStrike))
                return OriginalHook(AstralFlow);

            if (HasEffect(Buffs.FurtherRuin) && ((!HasEffect(Role.Buffs.Swiftcast) && IsIfritAttuned && IsMoving()) || (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
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

            int summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_PrimalChoice);
            int SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_BurstPhase);
            int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_Lucid);
            int swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_ST_SwiftcastPhase);
            int burstDelay = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling) ? PluginConfiguration.GetCustomIntValue(Config.SMN_ST_Burst_Delay) : 0;

            bool TitanAstralFlow = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[0];
            bool IfritAstralFlowCyclone = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[1];
            bool IfritAstralFlowStrike = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[3];
            bool GarudaAstralFlow = IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Egi_AstralFlow) && Config.SMN_ST_Egi_AstralFlow[2];

            var searingInSummon = GetCooldownRemainingTime(SearingLight) > (Gauge.SummonTimerRemaining / 1000f) + GCDTotal;

            DemiAttackCount = CurrentDemiSummon is not DemiSummon.None ? TimesUsedSinceOtherAction(OriginalHook(Aethercharge), [AstralImpulse, UmbralImpulse, FountainOfFire, AstralFlare, UmbralFlare, BrandOfPurgatory]) : 0;

            if (Variant.CanCure(CustomComboPreset.SMN_Variant_Cure, Config.SMN_VariantCure))
                return Variant.Cure;

            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Balance_Opener) && Opener().FullOpener(ref actionID))
                return actionID;

            if (Variant.CanRampart(CustomComboPreset.SMN_Variant_Rampart, WeaveTypes.SpellWeave))
                return Variant.Rampart;

            // Emergency priority Demi Nuke to prevent waste if you can't get demi attacks out to satisfy the slider check.
            if (CurrentDemiSummon is not DemiSummon.None && IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && Gauge.SummonTimerRemaining <= 2500)
            {
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow) && ((IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle) && CurrentDemiSummon is DemiSummon.Phoenix) || CurrentDemiSummon is not DemiSummon.Phoenix))
                        return OriginalHook(AstralFlow);
                }
            }

            if (CanSpellWeave())
            {
                // Searing Light
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight) && IsOffCooldown(SearingLight) && LevelChecked(SearingLight) && ((!HasEffectAny(Buffs.SearingLight) && Config.SMN_ST_Searing_Any) || !Config.SMN_ST_Searing_Any))
                {
                    if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight_Burst))
                    {
                        if ((SummonerBurstPhase is 0 or 1 && (CurrentDemiSummon is DemiSummon.Bahamut or DemiSummon.SolarBahamut)) ||
                            (SummonerBurstPhase == 2 && CurrentDemiSummon is DemiSummon.Phoenix) ||
                            SummonerBurstPhase == 3 && CurrentDemiSummon is not DemiSummon.None ||
                            (SummonerBurstPhase == 4))
                            return SearingLight;
                    }
                    else
                        return SearingLight;
                }

                // Energy Drain
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EDFester) && !Gauge.HasAetherflowStacks && ActionReady(EnergyDrain) &&
                    (!LevelChecked(DreadwyrmTrance) || DemiAttackCount >= burstDelay))
                    return EnergyDrain;

                // First set of Festers if Energy Drain is close to being off CD, or off CD while you have aetherflow stacks.
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EDFester) && IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling) && ActionReady(Fester) && GetCooldown(EnergyDrain).CooldownRemaining <= 3.2 &&
                    ((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                         (SummonerBurstPhase is not 4) ||
                        (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                    return OriginalHook(Fester);

                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingFlash) && HasEffect(Buffs.RubysGlimmer) && LevelChecked(SearingFlash))
                    return SearingFlash;

                // Demi Nuke
                if (CurrentDemiSummon is not DemiSummon.None && IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Attacks) && DemiAttackCount >= burstDelay && (IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_SearingLight_Burst) || HasEffect(Buffs.SearingLight) || searingInSummon))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow) && ((IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle) && CurrentDemiSummon is DemiSummon.Phoenix) || CurrentDemiSummon is not DemiSummon.Phoenix))
                        return OriginalHook(AstralFlow);
                }

                // Lux Solaris
                if (ActionReady(LuxSolaris) && IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris) &&
                    (PlayerHealthPercentageHp() < 100 || (GetBuffRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)))
                    return OriginalHook(LuxSolaris);

                // Fester
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EDFester))
                {
                    if (ActionReady(Fester))
                    {
                        if (IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_oGCDPooling))
                            return OriginalHook(Fester);

                        if (!LevelChecked(SearingLight))
                            return OriginalHook(Fester);

                        if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                             SummonerBurstPhase is 0 or 1 or 2 or 3 && DemiAttackCount >= burstDelay) ||
                            (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                            return OriginalHook(Fester);

                    }
                }

                // Lucid Dreaming
                if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Lucid) && Role.CanLucidDream(lucidThreshold))
                    return Role.LucidDreaming;
            }

            // Demi
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons) && PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
                return OriginalHook(Aethercharge);

            //Ruin4 in Egi Phases
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Ruin4) && ActionReady(Ruin4) &&
                ((!HasEffect(Role.Buffs.Swiftcast) && IsMoving() && ((HasEffect(Buffs.GarudasFavor) && !IsGarudaAttuned) || (IsIfritAttuned && ComboAction is not CrimsonCyclone))) ||
                 (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
                return Ruin4;

            // Egi Features
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_SwiftcastEgi))
            {
                // Swiftcast Garuda Feature
                if (swiftcastPhase is 0 or 1 or 3 && HasEffect(Buffs.GarudasFavor) && GarudaAstralFlow)
                {
                    if (Role.CanSwiftcast())
                        return Role.Swiftcast;

                    if (ActionReady(Slipstream) && HasEffect(Role.Buffs.Swiftcast))
                        return OriginalHook(AstralFlow);
                }

                // Swiftcast Ifrit Feature
                if (swiftcastPhase is 2 or 3)
                {
                    if (Role.CanSwiftcast(false) && ActionReady(RubyCata))
                        return Role.Swiftcast;
                }
            }

            // Precious Brilliance priority casting
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) &&
                ((IsIfritAttuned && ActionReady(RubyCata) && HasEffect(Role.Buffs.Swiftcast) && ComboAction is not CrimsonCyclone) ||
                 (HasEffect(Buffs.GarudasFavor) && !HasEffect(Role.Buffs.Swiftcast) && IsMoving())))
                return OriginalHook(PreciousBrilliance);

            if ((GarudaAstralFlow && HasEffect(Buffs.GarudasFavor)) ||
                (TitanAstralFlow && HasEffect(Buffs.TitansFavor) && CanSpellWeave()) ||
                (IfritAstralFlowCyclone && HasEffect(Buffs.IfritsFavor) && ((!Config.SMN_ST_CrimsonCycloneMelee) || (Config.SMN_ST_CrimsonCycloneMelee && InMeleeRange()))) ||
                (IfritAstralFlowStrike && HasEffect(Buffs.CrimsonStrike) && InMeleeRange()))
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
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_EgiSummons_Attacks) && ActionReady(Gemshine))
                return OriginalHook(Gemshine);

            // Egi Order
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiEgiMenu_EgiOrder) && Gauge.SummonTimerRemaining == 0)
            { 
                if (ActionReady(SummonEmerald) && (summonerPrimalChoice == 2 || !ActionReady(SummonTopaz)))
                    return OriginalHook(SummonEmerald);
            
                if (ActionReady(SummonTopaz))
                    return OriginalHook(SummonTopaz);

                if (ActionReady(SummonRuby))
                    return OriginalHook(SummonRuby);
            }

            // Ruin 4
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_Ruin4) && LevelChecked(Ruin4) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
                return Ruin4;

            return actionID;
        }
    }

    internal class SMN_Advanced_Combo_AoE : CustomCombo
    {
        internal static int DemiAttackCount = 0;
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMN_AoE_Advanced_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Outburst or Tridisaster))
                return actionID;

            int summonerPrimalChoice = PluginConfiguration.GetCustomIntValue(Config.SMN_AoE_PrimalChoice);
            int SummonerBurstPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_AoE_BurstPhase);
            int lucidThreshold = PluginConfiguration.GetCustomIntValue(Config.SMN_AoE_Lucid);
            int swiftcastPhase = PluginConfiguration.GetCustomIntValue(Config.SMN_AoE_SwiftcastPhase);
            int burstDelay = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling) ? PluginConfiguration.GetCustomIntValue(Config.SMN_AoE_Burst_Delay) : 0;

            bool TitanAstralFlow = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[0];
            bool IfritAstralFlowCyclone = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[1];
            bool IfritAstralFlowStrike = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[3];
            bool GarudaAstralFlow = IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Egi_AstralFlow) && Config.SMN_AoE_Egi_AstralFlow[2];

            var searingInSummon = GetCooldownRemainingTime(SearingLight) > (Gauge.SummonTimerRemaining / 1000f) + GCDTotal;

            DemiAttackCount = CurrentDemiSummon is not DemiSummon.None ? TimesUsedSinceOtherAction(OriginalHook(Aethercharge), [AstralImpulse, UmbralImpulse, FountainOfFire, AstralFlare, UmbralFlare, BrandOfPurgatory]) : 0;

            if (Variant.CanCure(CustomComboPreset.SMN_Variant_Cure, Config.SMN_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SMN_Variant_Rampart, WeaveTypes.SpellWeave))
                return Variant.Rampart;

            // Emergency priority Demi Nuke to prevent waste if you can't get demi attacks out to satisfy the slider check.
            if (CurrentDemiSummon is not DemiSummon.None && IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && Gauge.SummonTimerRemaining <= 2500)
            {
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow) && ((IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle) && CurrentDemiSummon is DemiSummon.Phoenix) || CurrentDemiSummon is not DemiSummon.Phoenix))
                        return OriginalHook(AstralFlow);
                }
            }

            if (CanSpellWeave())
            {
                // Searing Light
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight) && IsOffCooldown(SearingLight) && LevelChecked(SearingLight) && ((!HasEffectAny(Buffs.SearingLight) && Config.SMN_AoE_Searing_Any) || !Config.SMN_AoE_Searing_Any))
                {
                    if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight_Burst))
                    {
                        if ((SummonerBurstPhase is 0 or 1 && (CurrentDemiSummon is DemiSummon.Bahamut or DemiSummon.SolarBahamut)) ||
                            (SummonerBurstPhase == 2 && CurrentDemiSummon is DemiSummon.Phoenix) ||
                            SummonerBurstPhase == 3 && CurrentDemiSummon is not DemiSummon.None ||
                            (SummonerBurstPhase == 4))
                            return SearingLight;
                    }
                    else
                        return SearingLight;
                }

                // Energy Drain
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_ESPainflare) && !Gauge.HasAetherflowStacks && ActionReady(EnergyDrain) &&
                    (!LevelChecked(DreadwyrmTrance) || DemiAttackCount >= burstDelay))
                    return EnergyDrain;

                // First set of Painflares if Energy Drain is close to being off CD, or off CD while you have aetherflow stacks.
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_ESPainflare) && IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling) && ActionReady(Painflare) && GetCooldown(EnergyDrain).CooldownRemaining <= 3.2 &&
                    ((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                         (SummonerBurstPhase is not 4) ||
                        (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                    return OriginalHook(Painflare);

                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingFlash) && HasEffect(Buffs.RubysGlimmer) && LevelChecked(SearingFlash))
                    return SearingFlash;

                // Demi Nuke
                if (CurrentDemiSummon is not DemiSummon.None && IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Attacks) && DemiAttackCount >= burstDelay && (IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_SearingLight_Burst) || HasEffect(Buffs.SearingLight) || searingInSummon))
                {
                    if (ActionReady(OriginalHook(EnkindleBahamut)))
                        return OriginalHook(EnkindleBahamut);

                    if (ActionReady(AstralFlow) && ((IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiSummons_Rekindle) && CurrentDemiSummon is DemiSummon.Phoenix) || CurrentDemiSummon is not DemiSummon.Phoenix))
                        return OriginalHook(AstralFlow);
                }

                // Lux Solaris
                if (ActionReady(LuxSolaris) && IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons_LuxSolaris) &&
                    (PlayerHealthPercentageHp() < 100 || (GetBuffRemainingTime(Buffs.RefulgentLux) is < 3 and > 0)))
                    return OriginalHook(LuxSolaris);

                // Painflare
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_ESPainflare))
                {
                    if (ActionReady(Painflare))
                    {
                        if (IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_oGCDPooling))
                            return OriginalHook(Painflare);

                        if (!LevelChecked(SearingLight))
                            return OriginalHook(Painflare);

                        if ((((HasEffect(Buffs.SearingLight) && IsNotEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Burst_Any_Option)) || HasEffectAny(Buffs.SearingLight)) &&
                             SummonerBurstPhase is 0 or 1 or 2 or 3 && DemiAttackCount >= burstDelay) ||
                            (SummonerBurstPhase == 4 && !HasEffect(Buffs.TitansFavor)))
                            return OriginalHook(Painflare);

                    }
                }

                // Lucid Dreaming
                if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Lucid) && Role.CanLucidDream(lucidThreshold))
                    return Role.LucidDreaming;
            }

            // Demi
            if (IsEnabled(CustomComboPreset.SMN_ST_Advanced_Combo_DemiSummons) && PartyInCombat() && ActionReady(OriginalHook(Aethercharge)))
                return OriginalHook(Aethercharge);

            //Ruin4 in Egi Phases
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Ruin4) && ActionReady(Ruin4) &&
                ((!HasEffect(Role.Buffs.Swiftcast) && IsMoving() && ((HasEffect(Buffs.GarudasFavor) && !IsGarudaAttuned) || (IsIfritAttuned && ComboAction is not CrimsonCyclone))) ||
                 (GetCooldownRemainingTime(OriginalHook(Aethercharge)) is < 2.5f and > 0)))
                return Ruin4;

            // Egi Features
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_SwiftcastEgi))
            {
                // Swiftcast Garuda Feature
                if (swiftcastPhase is 0 or 1 or 3 && HasEffect(Buffs.GarudasFavor) && GarudaAstralFlow)
                {
                    if (Role.CanSwiftcast())
                        return Role.Swiftcast;

                    if (ActionReady(Slipstream) && HasEffect(Role.Buffs.Swiftcast))
                        return OriginalHook(AstralFlow);
                }

                // Swiftcast Ifrit Feature
                if (swiftcastPhase is 2 or 3)
                {
                    if (Role.CanSwiftcast(false) && ActionReady(RubyRite))
                        return Role.Swiftcast;
                }
            }

            // Precious Brilliance priority casting
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) &&
                ((IsIfritAttuned && ActionReady(RubyRite) && HasEffect(Role.Buffs.Swiftcast) && ComboAction is not CrimsonCyclone) ||
                 (HasEffect(Buffs.GarudasFavor) && !HasEffect(Role.Buffs.Swiftcast) && IsMoving())))
                return OriginalHook(PreciousBrilliance);

            if ((GarudaAstralFlow && HasEffect(Buffs.GarudasFavor)) ||
                (TitanAstralFlow && HasEffect(Buffs.TitansFavor) && CanSpellWeave()) ||
                (IfritAstralFlowCyclone && HasEffect(Buffs.IfritsFavor) && ((!Config.SMN_AoE_CrimsonCycloneMelee) || (Config.SMN_AoE_CrimsonCycloneMelee && InMeleeRange()))) ||
                (IfritAstralFlowStrike && HasEffect(Buffs.CrimsonStrike) && InMeleeRange()))
                return OriginalHook(AstralFlow);

            // Precious Brilliance
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_EgiSummons_Attacks) && ActionReady(PreciousBrilliance))
                return OriginalHook(PreciousBrilliance);

            // Egi Order
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_DemiEgiMenu_EgiOrder) && Gauge.SummonTimerRemaining == 0)
            {
                if (ActionReady(SummonEmerald) && (summonerPrimalChoice == 2 || !ActionReady(SummonTopaz)))
                    return OriginalHook(SummonEmerald);

                if (ActionReady(SummonTopaz))
                    return OriginalHook(SummonTopaz);

                if (ActionReady(SummonRuby))
                    return OriginalHook(SummonRuby);
            }

            // Ruin 4
            if (IsEnabled(CustomComboPreset.SMN_AoE_Advanced_Combo_Ruin4) && LevelChecked(Ruin4) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunmentTimerRemaining == 0 && HasEffect(Buffs.FurtherRuin))
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
