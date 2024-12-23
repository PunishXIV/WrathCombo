#region

using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DNC
{
    internal class DNC_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            if (actionID is not Cascade) return actionID;

            #region Variables

            var flow = HasEffect(Buffs.SilkenFlow) ||
                       HasEffect(Buffs.FlourishingFlow);
            var symmetry = HasEffect(Buffs.SilkenSymmetry) ||
                           HasEffect(Buffs.FlourishingSymmetry);
            var targetHpThresholdFeather = Config.DNC_ST_Adv_FeatherBurstPercent;
            var targetHpThresholdStandard = Config.DNC_ST_Adv_SSBurstPercent;
            var targetHpThresholdTechnical = Config.DNC_ST_Adv_TSBurstPercent;
            var gcd = GetCooldown(Fountain).CooldownTotal;
            var tillanaDriftProtectionActive =
                Config.DNC_ST_ADV_TillanaUse == (int)Config.TillanaDriftProtection.Favor;

            // Thresholds to wait for TS/SS to come off CD
            var longAlignmentThreshold = 0.6f;
            var shortAlignmentThreshold = 0.3f;
            if (Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.TripleWeave ||
                Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Both)
            {
                longAlignmentThreshold = 0.3f;
                shortAlignmentThreshold = 0.1f;
            }

            var needToTech =
                IsEnabled(CustomComboPreset.DNC_ST_Adv_TS) &&
                Config.DNC_ST_ADV_TS_IncludeTS == (int)Config.IncludeStep.Yes &&
                GetCooldownRemainingTime(TechnicalStep) <
                longAlignmentThreshold && // Up or about to be (some anti-drift)
                !HasEffect(Buffs.StandardStep) && // After Standard
                IsOnCooldown(StandardStep) &&
                GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                LevelChecked(TechnicalStep);

            var needToStandardOrFinish =
                GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                LevelChecked(StandardStep);

            // More Threshold, but only for SS
            if (Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Hold ||
                Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Both)
            {
                longAlignmentThreshold = gcd;
                shortAlignmentThreshold = gcd;
            }

            var needToFinish =
                IsEnabled(CustomComboPreset.DNC_ST_Adv_FM) &&
                HasEffect(Buffs.FinishingMoveReady) &&
                !HasEffect(Buffs.LastDanceReady) &&
                ((GetCooldownRemainingTime(StandardStep) < longAlignmentThreshold &&
                  HasEffect(Buffs.TechnicalFinish)) ||// Aggressive anti-drift
                 (!HasEffect(Buffs.TechnicalFinish) && // Anti-Drift outside of Tech
                  GetCooldownRemainingTime(StandardStep) <
                  shortAlignmentThreshold));

            var needToStandard =
                IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                Config.DNC_ST_ADV_SS_IncludeSS == (int)Config.IncludeStep.Yes &&
                GetCooldownRemainingTime(StandardStep) <
                longAlignmentThreshold && // Up or about to be (some anti-drift)
                !HasEffect(Buffs.FinishingMoveReady) &&
                (IsOffCooldown(Flourish) ||
                 GetCooldownRemainingTime(Flourish) > 5) &&
                !HasEffect(Buffs.TechnicalFinish);

            #endregion

            #region Opener

            // Opener
            if (IsEnabled(CustomComboPreset.DNC_ST_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            #endregion

            #region Pre-pull

            if (!InCombat() && TargetIsHostile())
            {
                // Dance Partner
                var dpEnabled = P.IPC.GetComboState(
                        CustomComboPreset.DNC_ST_AdvancedMode.ToString())!
                    .Values.First();
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Partner) &&
                    ActionReady(ClosedPosition) &&
                    !HasEffect(Buffs.ClosedPosition) &&
                    (GetPartyMembers().Count > 1 || HasCompanionPresent()) &&
                    !(dpEnabled && P.IPC.GetAutoRotationState())) // Disable feature in Auto-Rotation
                    // todo: do not disable for auto-rotation, provide targeting
                    return ClosedPosition;

                // ST Standard Step (Pre-pull)
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                    IsEnabled(CustomComboPreset.DNC_ST_Adv_SS_Prepull) &&
                    Config.DNC_ST_ADV_SS_IncludeSS == (int)Config.IncludeStep.Yes &&
                    ActionReady(StandardStep) &&
                    !HasEffect(Buffs.FinishingMoveReady) &&
                    !HasEffect(Buffs.TechnicalFinish) &&
                    IsOffCooldown(TechnicalStep) &&
                    IsOffCooldown(StandardStep))
                    return StandardStep;

                // ST Standard Steps (Pre-pull)
                if ((IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                     IsEnabled(CustomComboPreset.DNC_ST_Adv_SS_Prepull)) &&
                    HasEffect(Buffs.StandardStep) &&
                    Gauge.CompletedSteps < 2)
                    return Gauge.NextStep;

                // ST Peloton
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Peloton) &&
                    !HasEffectAny(Buffs.Peloton) &&
                    GetBuffRemainingTime(Buffs.StandardStep) > 5)
                    return Peloton;
            }

            #endregion

            #region Dance Fills

            // ST Standard (Dance) Steps & Fill
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                HasEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : StandardFinish2;

            // ST Technical (Dance) Steps & Fill
            if ((IsEnabled(CustomComboPreset.DNC_ST_Adv_TS)) &&
                HasEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : TechnicalFinish4;

            #endregion

            #region Weaves

            // ST Devilment
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Devilment) &&
                CanWeave(actionID) &&
                LevelChecked(Devilment) &&
                GetCooldownRemainingTime(Devilment) < 0.05 &&
                (HasEffect(Buffs.TechnicalFinish) ||
                 WasLastAction(TechnicalFinish4) ||
                 !LevelChecked(TechnicalStep)))
                return Devilment;

            // ST Flourish
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Flourish) &&
                CanWeave(actionID) &&
                ActionReady(Flourish) &&
                !WasLastWeaponskill(TechnicalFinish4) &&
                IsOnCooldown(Devilment) &&
                (GetCooldownRemainingTime(Devilment) > 50 ||
                 (HasEffect(Buffs.Devilment) &&
                  GetBuffRemainingTime(Buffs.Devilment) < 19)) &&
                !HasEffect(Buffs.ThreeFoldFanDance) &&
                !HasEffect(Buffs.FourFoldFanDance) &&
                !HasEffect(Buffs.FlourishingSymmetry) &&
                !HasEffect(Buffs.FlourishingFlow) &&
                !HasEffect(Buffs.FinishingMoveReady) &&
                ((CombatEngageDuration().TotalSeconds < 20 &&
                  HasEffect(Buffs.TechnicalFinish)) ||
                 CombatEngageDuration().TotalSeconds > 20))
                return Flourish;

            if ((Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.TripleWeave ||
                 Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Both) &&
                (HasEffect(Buffs.ThreeFoldFanDance) ||
                 HasEffect(Buffs.FourFoldFanDance)) &&
                CombatEngageDuration().TotalSeconds > 20 &&
                HasEffect(Buffs.TechnicalFinish) &&
                GetCooldownRemainingTime(Flourish) > 58)
            {
                if (HasEffect(Buffs.ThreeFoldFanDance) &&
                    CanDelayedWeave(actionID))
                    return FanDance3;
                if (HasEffect(Buffs.FourFoldFanDance))
                    return FanDance4;
            }

            // ST Interrupt
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Interrupt) &&
                CanInterruptEnemy() &&
                ActionReady(All.HeadGraze) &&
                !HasEffect(Buffs.TechnicalFinish))
                return All.HeadGraze;

            // Variant Cure
            if (IsEnabled(CustomComboPreset.DNC_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <=
                GetOptionValue(Config.DNCVariantCurePercent))
                return Variant.VariantCure;

            // Variant Rampart
            if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave(actionID))
                return Variant.VariantRampart;

            if (CanWeave(actionID) && !WasLastWeaponskill(TechnicalFinish4))
            {
                if (HasEffect(Buffs.ThreeFoldFanDance))
                    return FanDance3;

                if (HasEffect(Buffs.FourFoldFanDance))
                    return FanDance4;

                // ST Feathers & Fans
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Feathers) &&
                    LevelChecked(FanDance1))
                {
                    // FD1 HP% Dump
                    if (GetTargetHPPercent() <= targetHpThresholdFeather &&
                        Gauge.Feathers > 0)
                        return FanDance1;

                    if (LevelChecked(TechnicalStep))
                    {
                        // Burst FD1
                        if (HasEffect(Buffs.TechnicalFinish) &&
                            Gauge.Feathers > 0)
                            return FanDance1;

                        // FD1 Pooling
                        if (Gauge.Feathers > 3 &&
                            (HasEffect(Buffs.SilkenSymmetry) ||
                             HasEffect(Buffs.SilkenFlow))
                           )

                            return FanDance1;
                    }

                    // FD1 Non-pooling & under burst level
                    if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                        return FanDance1;
                }

                // ST Panic Heals
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_PanicHeals))
                {
                    if (ActionReady(CuringWaltz) &&
                        PlayerHealthPercentageHp() <
                        Config.DNC_ST_Adv_PanicHealWaltzPercent)
                        return CuringWaltz;

                    if (ActionReady(All.SecondWind) &&
                        PlayerHealthPercentageHp() <
                        Config.DNC_ST_Adv_PanicHealWindPercent)
                        return All.SecondWind;
                }

                // ST Improvisation
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Improvisation) &&
                    ActionReady(Improvisation) &&
                    !HasEffect(Buffs.TechnicalFinish) &&
                    InCombat())
                    return Improvisation;
            }

            #endregion

            #region GCD

            // ST Technical Step
            if (needToTech)
                return TechnicalStep;

            // ST Last Dance
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_LD) && // Enabled
                HasEffect(Buffs.LastDanceReady) && // Ready
                (HasEffect(Buffs.TechnicalFinish) || // Has Tech
                 !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                   GetCooldownRemainingTime(TechnicalStep) < 20 &&
                   GetBuffRemainingTime(Buffs.LastDanceReady) >
                   GetCooldownRemainingTime(TechnicalStep) + 4) ||
                 GetBuffRemainingTime(Buffs.LastDanceReady) <
                 4)) // Or last second
                return LastDance;

            // ST Standard Step (Finishing Move)
            if (needToStandardOrFinish && needToFinish)
                return OriginalHook(FinishingMove);

            // ST Standard Step
            if (needToStandardOrFinish && needToStandard)
                return StandardStep;

            // Emergency Starfall usage
            if (HasEffect(Buffs.FlourishingStarfall) &&
                GetBuffRemainingTime(Buffs.FlourishingStarfall) < 4)
                return StarfallDance;

            // ST Dance of the Dawn
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_DawnDance) &&
                HasEffect(Buffs.DanceOfTheDawnReady) &&
                LevelChecked(DanceOfTheDawn) &&
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep)) && // Tech is up
                (Gauge.Esprit >= Config.DNC_ST_Adv_SaberThreshold || // >esprit threshold use
                 (HasEffect(Buffs.TechnicalFinish) && // will overcap with Tillana if not used
                  !tillanaDriftProtectionActive && Gauge.Esprit >= 50) ||
                 (GetBuffRemainingTime(Buffs.DanceOfTheDawnReady) < 5 &&
                  Gauge.Esprit >= 50))) // emergency use
                return OriginalHook(DanceOfTheDawn);

            // ST Saber Dance (Emergency Use)
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                (Gauge.Esprit >=
                 Config
                     .DNC_ST_Adv_SaberThreshold || // above esprit threshold use
                 (HasEffect(Buffs
                      .TechnicalFinish) && // will overcap with Tillana if not used
                  !tillanaDriftProtectionActive && Gauge.Esprit >= 50)))
                return LevelChecked(DanceOfTheDawn) &&
                       HasEffect(Buffs.DanceOfTheDawnReady)
                    ? OriginalHook(DanceOfTheDawn)
                    : SaberDance;

            if (HasEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            // ST Tillana
            if (HasEffect(Buffs.FlourishingFinish) &&
                IsEnabled(CustomComboPreset.DNC_ST_Adv_Tillana))
                return Tillana;

            // ST Saber Dance
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                Gauge.Esprit >=
                Config.DNC_ST_Adv_SaberThreshold || // Above esprit threshold use
                (HasEffect(Buffs.TechnicalFinish) &&
                 Gauge.Esprit >= 50) && // Burst
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep))) // Tech is up
                return SaberDance;

            // ST combos and burst attacks
            if (LevelChecked(Fountain) &&
                lastComboMove is Cascade &&
                comboTime is < 2 and > 0)
                return Fountain;

            if (LevelChecked(Fountainfall) && flow)
                return Fountainfall;
            if (LevelChecked(ReverseCascade) && symmetry)
                return ReverseCascade;
            if (LevelChecked(Fountain) && lastComboMove is Cascade &&
                comboTime > 0)
                return Fountain;

            #endregion

            return actionID;
        }
    }

    internal class DNC_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            if (actionID is not Windmill) return actionID;

            #region Variables

            bool flow = HasEffect(Buffs.SilkenFlow) ||
                        HasEffect(Buffs.FlourishingFlow);
            bool symmetry = HasEffect(Buffs.SilkenSymmetry) ||
                            HasEffect(Buffs.FlourishingSymmetry);
            var targetHpThresholdStandard = Config.DNC_AoE_Adv_SSBurstPercent;
            var targetHpThresholdTechnical = Config.DNC_AoE_Adv_TSBurstPercent;

            var needToTech =
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_TS) &&
                Config.DNC_AoE_Adv_TS_IncludeTS == (int)Config.IncludeStep.Yes &&
                ActionReady(TechnicalStep) && // Up
                !HasEffect(Buffs.StandardStep) && // After Standard
                IsOnCooldown(StandardStep) &&
                GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                LevelChecked(TechnicalStep);

            var needToStandardOrFinish =
                ActionReady(StandardStep) && // Up
                GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                (IsOffCooldown(
                     TechnicalStep) || // Checking burst is ready for standard
                 GetCooldownRemainingTime(TechnicalStep) > 5) && // Don't mangle
                LevelChecked(StandardStep);

            var needToFinish =
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_FM) && // Enabled
                HasEffect(Buffs.FinishingMoveReady) &&
                !HasEffect(Buffs.LastDanceReady);

            var needToStandard =
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_SS) && // Enabled
                Config.DNC_AoE_Adv_SS_IncludeSS == (int)Config.IncludeStep.Yes &&
                !HasEffect(Buffs.FinishingMoveReady) &&
                (IsOffCooldown(Flourish) ||
                 GetCooldownRemainingTime(Flourish) > 5) &&
                !HasEffect(Buffs.TechnicalFinish);

            #endregion

            #region Prepull

            // Dance Partner
            if (!InCombat() &&
                TargetIsHostile() &&
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_Partner) &&
                ActionReady(ClosedPosition) &&
                !HasEffect(Buffs.ClosedPosition) &&
                (GetPartyMembers().Count > 1 || HasCompanionPresent()) &&
                !(Service.Configuration.AutoActions[
                      CustomComboPreset.DNC_AoE_AdvancedMode] &&
                  Service.Configuration.RotationConfig
                      .Enabled)) // Disabled in Auto-Rotation
                // todo: do not disable for auto-rotation, provide targeting
                return ClosedPosition;

            #endregion

            #region Dance Fills

            // AoE Standard (Dance) Steps & Fill
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_SS) &&
                HasEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : StandardFinish2;

            // AoE Technical (Dance) Steps & Fill
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_TS) &&
                HasEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : TechnicalFinish4;

            #endregion

            #region Weaves

            // AoE Devilment
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Devilment) &&
                CanWeave(actionID) &&
                LevelChecked(Devilment) &&
                GetCooldownRemainingTime(Devilment) < 0.05 &&
                (HasEffect(Buffs.TechnicalFinish) ||
                 WasLastAction(TechnicalFinish4) ||
                 !LevelChecked(TechnicalStep)))
                return Devilment;

            // AoE Flourish
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Flourish) &&
                CanWeave(actionID) &&
                ActionReady(Flourish) &&
                !WasLastWeaponskill(TechnicalFinish4) &&
                IsOnCooldown(Devilment) &&
                (GetCooldownRemainingTime(Devilment) > 50 ||
                 (HasEffect(Buffs.Devilment) &&
                  GetBuffRemainingTime(Buffs.Devilment) < 19)) &&
                !HasEffect(Buffs.ThreeFoldFanDance) &&
                !HasEffect(Buffs.FourFoldFanDance) &&
                !HasEffect(Buffs.FlourishingSymmetry) &&
                !HasEffect(Buffs.FlourishingFlow) &&
                !HasEffect(Buffs.FinishingMoveReady))
                return Flourish;

            // AoE Interrupt
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Interrupt) &&
                CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                !HasEffect(Buffs.TechnicalFinish))
                return All.HeadGraze;

            if (IsEnabled(CustomComboPreset.DNC_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <=
                GetOptionValue(Config.DNCVariantCurePercent))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave(actionID))
                return Variant.VariantRampart;

            if (CanWeave(actionID) && !WasLastWeaponskill(TechnicalFinish4))
            {
                // AoE Feathers & Fans
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Feathers) &&
                    LevelChecked(FanDance1))
                {
                    // FD3
                    if (HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;

                    if (LevelChecked(FanDance2))
                    {
                        if (LevelChecked(TechnicalStep))
                        {
                            // Burst FD2
                            if (HasEffect(Buffs.TechnicalFinish) &&
                                Gauge.Feathers > 0)
                                return FanDance2;

                            // FD2 Pooling
                            if (Gauge.Feathers > 3 &&
                                (HasEffect(Buffs.SilkenSymmetry) ||
                                 HasEffect(Buffs.SilkenFlow)))
                                return FanDance2;
                        }

                        // FD2 Non-pooling & under burst level
                        if (!LevelChecked(TechnicalStep) &&
                            Gauge.Feathers > 0)
                            return FanDance2;
                    }

                    // FD1 Replacement for Lv.30-49
                    if (!LevelChecked(FanDance2) &&
                        Gauge.Feathers > 0)
                        return FanDance1;
                }

                if (HasEffect(Buffs.FourFoldFanDance))
                    return FanDance4;

                // AoE Panic Heals
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_PanicHeals))
                {
                    if (ActionReady(CuringWaltz) &&
                        PlayerHealthPercentageHp() <
                        Config.DNC_AoE_Adv_PanicHealWaltzPercent)
                        return CuringWaltz;

                    if (ActionReady(All.SecondWind) &&
                        PlayerHealthPercentageHp() <
                        Config.DNC_AoE_Adv_PanicHealWindPercent)
                        return All.SecondWind;
                }

                // AoE Improvisation
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Improvisation) &&
                    ActionReady(Improvisation) &&
                    !HasEffect(Buffs.TechnicalStep) &&
                    InCombat())
                    return Improvisation;
            }

            #endregion

            #region GCD

            // AoE Technical Step
            if (needToTech)
                return TechnicalStep;

            // AoE Last Dance
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_LD) && // Enabled
                HasEffect(Buffs.LastDanceReady) && // Ready
                (HasEffect(Buffs.TechnicalFinish) || // Has Tech
                 !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                   GetCooldownRemainingTime(TechnicalStep) < 20 &&
                   GetBuffRemainingTime(Buffs.LastDanceReady) >
                   GetCooldownRemainingTime(TechnicalStep) + 4) ||
                 GetBuffRemainingTime(Buffs.LastDanceReady) <
                 4)) // Or last second
                return LastDance;

            // AoE Standard Step (Finishing Move)
            if (needToStandardOrFinish && needToFinish)
                return OriginalHook(FinishingMove);

            // AoE Standard Step
            if (needToStandardOrFinish && needToStandard)
                return StandardStep;

            // Emergency Starfall usage
            if (HasEffect(Buffs.FlourishingStarfall) &&
                GetBuffRemainingTime(Buffs.FlourishingStarfall) < 4)
                return StarfallDance;

            // AoE Dance of the Dawn
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_DawnDance) &&
                HasEffect(Buffs.DanceOfTheDawnReady) &&
                LevelChecked(DanceOfTheDawn) &&
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep)) && // Tech is up
                (Gauge.Esprit >=
                 Config
                     .DNC_AoE_Adv_SaberThreshold || // above esprit threshold use
                 (HasEffect(Buffs.TechnicalFinish) &&
                  Gauge.Esprit >= 50) || // will overcap with Tillana if not used
                 (GetBuffRemainingTime(Buffs.DanceOfTheDawnReady) < 5 &&
                  Gauge.Esprit >= 50))) // emergency use
                return OriginalHook(DanceOfTheDawn);

            // AoE Saber Dance (Emergency Use)
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                (Gauge.Esprit >=
                 Config
                     .DNC_AoE_Adv_SaberThreshold || // above esprit threshold use
                 (HasEffect(Buffs.TechnicalFinish) &&
                  Gauge.Esprit >=
                  50)) && // will overcap with Tillana if not used
                ActionReady(SaberDance))
                return SaberDance;

            if (HasEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            // AoE Tillana
            if (HasEffect(Buffs.FlourishingFinish) &&
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_Tillana))
                return Tillana;

            // AoE Saber Dance
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                Gauge.Esprit >=
                Config.DNC_ST_Adv_SaberThreshold || // Above esprit threshold use
                (HasEffect(Buffs.TechnicalFinish) &&
                 Gauge.Esprit >= 50) && // Burst
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep))) // Tech is up
                return SaberDance;

            // AoE combos and burst attacks
            if (LevelChecked(Bladeshower) &&
                lastComboMove is Windmill &&
                comboTime is < 2 and > 0)
                return Bladeshower;

            if (LevelChecked(Bloodshower) && flow)
                return Bloodshower;
            if (LevelChecked(RisingWindmill) && symmetry)
                return RisingWindmill;
            if (LevelChecked(Bladeshower) && lastComboMove is Windmill &&
                comboTime > 0)
                return Bladeshower;

            #endregion

            return actionID;
        }
    }

    #region MultiButton Combos

    internal class DNC_ST_MultiButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_ST_MultiButton;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            if (actionID is Cascade)
            {
                #region Types

                bool flow = HasEffect(Buffs.SilkenFlow) ||
                            HasEffect(Buffs.FlourishingFlow);
                bool symmetry = HasEffect(Buffs.SilkenSymmetry) ||
                                HasEffect(Buffs.FlourishingSymmetry);

                #endregion

                // ST Esprit overcap protection
                if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                    LevelChecked(DanceOfTheDawn) &&
                    HasEffect(Buffs.DanceOfTheDawnReady) &&
                    Gauge.Esprit >= Config.DNCEspritThreshold_ST)
                    return OriginalHook(DanceOfTheDawn);
                if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                    LevelChecked(SaberDance) &&
                    Gauge.Esprit >= Config.DNCEspritThreshold_ST)
                    return SaberDance;

                if (CanWeave(actionID))
                {
                    // ST Fan Dance overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                        LevelChecked(FanDance1) && Gauge.Feathers is 4 &&
                        (HasEffect(Buffs.SilkenSymmetry) ||
                         HasEffect(Buffs.SilkenFlow)))
                        return FanDance1;

                    // ST Fan Dance 3/4 on combo
                    if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34))
                    {
                        if (HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;
                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;
                    }
                }

                // ST base combos
                if (LevelChecked(Fountainfall) && flow)
                    return Fountainfall;
                if (LevelChecked(ReverseCascade) && symmetry)
                    return ReverseCascade;
                if (LevelChecked(Fountain) && lastComboMove is Cascade)
                    return Fountain;
            }

            return actionID;
        }
    }

    internal class DNC_AoE_MultiButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_AoE_MultiButton;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            if (actionID is Windmill)
            {
                #region Types

                bool flow = HasEffect(Buffs.SilkenFlow) ||
                            HasEffect(Buffs.FlourishingFlow);
                bool symmetry = HasEffect(Buffs.SilkenSymmetry) ||
                                HasEffect(Buffs.FlourishingSymmetry);

                #endregion

                // AoE Esprit overcap protection
                if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                    LevelChecked(DanceOfTheDawn) &&
                    HasEffect(Buffs.DanceOfTheDawnReady) &&
                    Gauge.Esprit >= Config.DNCEspritThreshold_ST)
                    return OriginalHook(DanceOfTheDawn);
                if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                    LevelChecked(SaberDance) &&
                    Gauge.Esprit >= Config.DNCEspritThreshold_AoE)
                    return SaberDance;

                if (CanWeave(actionID))
                {
                    // AoE Fan Dance overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap) &&
                        LevelChecked(FanDance2) && Gauge.Feathers is 4 &&
                        (HasEffect(Buffs.SilkenSymmetry) ||
                         HasEffect(Buffs.SilkenFlow)))
                        return FanDance2;

                    // AoE Fan Dance 3/4 on combo
                    if (IsEnabled(CustomComboPreset.DNC_AoE_FanDance34))
                    {
                        if (HasEffect(Buffs.ThreeFoldFanDance))
                            return FanDance3;
                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;
                    }
                }

                // AoE base combos
                if (LevelChecked(Bloodshower) && flow)
                    return Bloodshower;
                if (LevelChecked(RisingWindmill) && symmetry)
                    return RisingWindmill;
                if (LevelChecked(Bladeshower) && lastComboMove is Windmill)
                    return Bladeshower;
            }

            return actionID;
        }
    }

    #region Smaller Features

    #region Dance Features

    internal class DNC_DanceStepCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_DanceStepCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            // Standard Step
            if (actionID is StandardStep && Gauge.IsDancing &&
                HasEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : StandardFinish2;

            // Technical Step
            if (actionID is TechnicalStep && Gauge.IsDancing &&
                HasEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : TechnicalFinish4;

            return actionID;
        }
    }

    internal class DNC_DanceComboReplacer : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_DanceComboReplacer;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            if (!GetJobGauge<DNCGauge>().IsDancing) return actionID;

            var actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

            if (actionID == actionIDs[0] ||
                (actionIDs[0] == 0 && actionID == Cascade)) // Default
                return Emboite;

            if (actionID == actionIDs[1] ||
                (actionIDs[1] == 0 && actionID == Flourish)) // Default
                return Entrechat;

            if (actionID == actionIDs[2] ||
                (actionIDs[2] == 0 && actionID == FanDance1)) // Default
                return Jete;

            if (actionID == actionIDs[3] ||
                (actionIDs[3] == 0 && actionID == FanDance2)) // Default
                return Pirouette;

            return actionID;
        }
    }

    #endregion

    #region Fan Features

    internal class DNC_FlourishingFanDances : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_FlourishingFanDances;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            // Fan Dance 3 & 4 on Flourish
            if (actionID is not Flourish || !CanWeave(actionID)) return actionID;

            if (IsEnabled(CustomComboPreset.DNC_Flourishing_FD3) &&
                HasEffect(Buffs.ThreeFoldFanDance))
                return FanDance3;

            if (HasEffect(Buffs.FourFoldFanDance))
                return FanDance4;

            return actionID;
        }
    }

    internal class DNC_FanDanceCombos : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_FanDanceCombos;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            // FD 1 --> 3, FD 1 --> 4
            if (actionID is FanDance1)
            {
                if (IsEnabled(CustomComboPreset.DNC_FanDance_1to3_Combo) &&
                    HasEffect(Buffs.ThreeFoldFanDance))
                    return FanDance3;
                if (IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo) &&
                    HasEffect(Buffs.FourFoldFanDance))
                    return FanDance4;
            }

            // FD 2 --> 3, FD 2 --> 4
            if (actionID is FanDance2)
            {
                if (IsEnabled(CustomComboPreset.DNC_FanDance_2to3_Combo) &&
                    HasEffect(Buffs.ThreeFoldFanDance))
                    return FanDance3;
                if (IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo) &&
                    HasEffect(Buffs.FourFoldFanDance))
                    return FanDance4;
            }

            return actionID;
        }
    }

    #endregion

    internal class DNC_Starfall_Devilment : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_Starfall_Devilment;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level) =>
            actionID is Devilment && HasEffect(Buffs.FlourishingStarfall)
                ? StarfallDance
                : actionID;
    }

    internal class DNC_StandardStep_LastDance : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_StandardStep_LastDance;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level) =>
            actionID is StandardStep or FinishingMove &&
            HasEffect(Buffs.LastDanceReady)
                ? LastDance
                : actionID;
    }

    internal class DNC_TechnicalStep_Devilment : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_TechnicalStep_Devilment;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level) =>
            actionID is TechnicalStep &&
            WasLastWeaponskill(TechnicalFinish4) &&
            HasEffect(Buffs.TechnicalFinish)
                ? Devilment
                : actionID;
    }

    internal class DNC_Procc_Bladeshower : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_Procc_Bladeshower;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level) =>
            actionID is Bloodshower &&
            !HasEffect(Buffs.FlourishingFlow) &&
            !HasEffect(Buffs.SilkenFlow)
                ? Bladeshower
                : actionID;
    }

    internal class DNC_Procc_Windmill : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_Procc_Windmill;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level) =>
            actionID is RisingWindmill &&
            !HasEffect(Buffs.FlourishingSymmetry) &&
            !HasEffect(Buffs.SilkenSymmetry)
                ? Windmill
                : actionID;
    }

    #endregion

    #endregion

    #region IDs

    public const byte JobID = 38;

    #region Actions

    public const uint
        // Single Target
        Cascade = 15989,
        Fountain = 15990,
        ReverseCascade = 15991,
        Fountainfall = 15992,
        StarfallDance = 25792,
        // AoE
        Windmill = 15993,
        Bladeshower = 15994,
        RisingWindmill = 15995,
        Bloodshower = 15996,
        Tillana = 25790,
        // Dancing
        StandardStep = 15997,
        TechnicalStep = 15998,
        StandardFinish0 = 16003,
        StandardFinish1 = 16191,
        StandardFinish2 = 16192,
        TechnicalFinish0 = 16004,
        TechnicalFinish1 = 16193,
        TechnicalFinish2 = 16194,
        TechnicalFinish3 = 16195,
        TechnicalFinish4 = 16196,
        Emboite = 15999,
        Entrechat = 16000,
        Jete = 16001,
        Pirouette = 16002,
        // Fan Dances
        FanDance1 = 16007,
        FanDance2 = 16008,
        FanDance3 = 16009,
        FanDance4 = 25791,
        // Other
        Peloton = 7557,
        SaberDance = 16005,
        ClosedPosition = 16006,
        EnAvant = 16010,
        Devilment = 16011,
        ShieldSamba = 16012,
        Flourish = 16013,
        Improvisation = 16014,
        CuringWaltz = 16015,
        LastDance = 36983,
        FinishingMove = 36984,
        DanceOfTheDawn = 36985;

    #endregion

    public static class Buffs
    {
        public const ushort
            // Flourishing & Silken (procs)
            FlourishingCascade = 1814,
            FlourishingFountain = 1815,
            FlourishingWindmill = 1816,
            FlourishingShower = 1817,
            FlourishingFanDance = 2021,
            SilkenSymmetry = 2693,
            SilkenFlow = 2694,
            FlourishingFinish = 2698,
            FlourishingStarfall = 2700,
            FlourishingSymmetry = 3017,
            FlourishingFlow = 3018,
            // Dances
            StandardStep = 1818,
            TechnicalStep = 1819,
            StandardFinish = 1821,
            TechnicalFinish = 1822,
            // Fan Dances
            ThreeFoldFanDance = 1820,
            FourFoldFanDance = 2699,
            // Other
            Peloton = 1199,
            ClosedPosition = 1823,
            ShieldSamba = 1826,
            LastDanceReady = 3867,
            FinishingMoveReady = 3868,
            DanceOfTheDawnReady = 3869,
            Devilment = 1825;
    }

    #endregion
}
