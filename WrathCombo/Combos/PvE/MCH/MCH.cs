using Dalamud.Game.ClientState.Statuses;
using System;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.MCH.Config;
namespace WrathCombo.Combos.PvE;

internal partial class MCH : PhysicalRanged
{
    internal class MCH_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SplitShot or HeatedSplitShot))
                return actionID;

            //Reassemble to start before combat
            if (!HasStatusEffect(Buffs.Reassembled) && ActionReady(Reassemble) &&
                !InCombat() && HasBattleTarget() &&
                (ActionReady(Excavator) ||
                 ActionReady(Chainsaw) ||
                 LevelChecked(AirAnchor) && IsOffCooldown(AirAnchor) ||
                 ActionReady(Drill)))
                return Reassemble;

            if (!HasStatusEffect(Buffs.Reassembled) && ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                // Wildfire
                if (JustUsed(Hypercharge) &&
                    ActionReady(Wildfire) &&
                    !HasStatusEffect(Buffs.Wildfire) && TargetIsBoss() &&
                    CanApplyStatus(CurrentTarget, Debuffs.Wildfire))
                    return Wildfire;

                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (ActionReady(BarrelStabilizer) && TargetIsBoss() &&
                        !HasStatusEffect(Buffs.FullMetalMachinist))
                        return BarrelStabilizer;

                    // Hypercharge
                    if ((Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) &&
                        !IsComboExpiring(6) && ActionReady(Hypercharge))
                    {
                        // Ensures Hypercharge is double weaved with WF
                        if (LevelChecked(FullMetalField) && JustUsed(FullMetalField) ||
                            !LevelChecked(FullMetalField) && ActionReady(Wildfire) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        // Only Hypercharge when tools are on cooldown
                        if (DrillCD && AnchorCD && SawCD &&
                            (!LevelChecked(Wildfire) ||
                             LevelChecked(Wildfire) &&
                             (GetCooldownRemainingTime(Wildfire) > 40 ||
                              IsOffCooldown(Wildfire) && !HasStatusEffect(Buffs.FullMetalMachinist))))
                            return Hypercharge;
                    }

                    //Queen
                    if (UseQueen())
                        return OriginalHook(RookAutoturret);

                    // Reassemble
                    if (Reassembled())
                        return Reassemble;

                    // Gauss Round and Ricochet outside HC
                    if (JustUsed(OriginalHook(AirAnchor), 2f) ||
                        JustUsed(Chainsaw, 2f) ||
                        JustUsed(Drill, 2f) ||
                        JustUsed(Excavator, 2f))
                    {
                        if (ActionReady(GaussRound) &&
                            !JustUsed(OriginalHook(GaussRound), 2f))
                            return OriginalHook(GaussRound);

                        if (ActionReady(Ricochet) &&
                            !JustUsed(OriginalHook(Ricochet), 2f))
                            return OriginalHook(Ricochet);
                    }

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_ST_SimpleMode, WeaveTypes.DelayWeave))
                        return Role.HeadGraze;

                    // Healing
                    if (Role.CanSecondWind(5))
                        return Role.SecondWind;
                }

                // Gauss Round and Ricochet during HC
                if (JustUsed(OriginalHook(Heatblast), 1f) && HasNotWeaved)
                {
                    if (ActionReady(GaussRound) &&
                        (UseGaussRound || !LevelChecked(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(Ricochet) && UseRicochet)
                        return OriginalHook(Ricochet);
                }
            }

            //Tools
            if (Tools(ref actionID))
                return actionID;

            // Full Metal Field
            if (HasStatusEffect(Buffs.FullMetalMachinist, out Status? fullMetal) &&
                TargetIsBoss() && !JustUsed(BarrelStabilizer) &&
                (ActionReady(Wildfire) || fullMetal.RemainingTime <= 6))
                return FullMetalField;

            // Heatblast
            if (IsOverheated && ActionReady(Heatblast))
                return OriginalHook(Heatblast);

            // 1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(SlugShot))
                    return OriginalHook(SlugShot);

                if (ComboAction is SlugShot &&
                    !LevelChecked(Drill) && !HasStatusEffect(Buffs.Reassembled) && ActionReady(Reassemble))
                    return Reassemble;

                if (ComboAction is SlugShot && LevelChecked(CleanShot))
                    return OriginalHook(CleanShot);
            }
            return actionID;
        }
    }

    internal class MCH_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpreadShot or Scattergun))
                return actionID;

            if (HasStatusEffect(Buffs.Flamethrower) || JustUsed(Flamethrower, GCD))
                return All.SavageBlade;

            // Interrupt
            if (Role.CanHeadGraze(Preset.MCH_AoE_SimpleMode, WeaveTypes.DelayWeave))
                return Role.HeadGraze;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (ActionReady(BarrelStabilizer) &&
                        !HasStatusEffect(Buffs.FullMetalMachinist))
                        return BarrelStabilizer;

                    if (Battery is 100)
                        return OriginalHook(RookAutoturret);

                    // Hypercharge
                    if ((Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                        LevelChecked(AutoCrossbow) &&
                        (LevelChecked(BioBlaster) && GetCooldownRemainingTime(BioBlaster) > 10 ||
                         !LevelChecked(BioBlaster)) &&
                        (LevelChecked(Flamethrower) && GetCooldownRemainingTime(Flamethrower) > 10 ||
                         !LevelChecked(Flamethrower)))
                        return Hypercharge;

                    if (ActionReady(Reassemble) &&
                        !HasStatusEffect(Buffs.Wildfire) &&
                        !HasStatusEffect(Buffs.Reassembled) &&
                        !JustUsed(Flamethrower, 10f) &&
                        (HasStatusEffect(Buffs.ExcavatorReady) && LevelChecked(Excavator) ||
                         GetCooldownRemainingTime(Chainsaw) < 1 && LevelChecked(Chainsaw) ||
                         GetCooldownRemainingTime(AirAnchor) < 1 && LevelChecked(AirAnchor) ||
                         LevelChecked(Scattergun)))
                        return Reassemble;

                    if (Role.CanSecondWind(25))
                        return Role.SecondWind;
                }

                //AutoCrossbow, Gauss, Rico
                if ((JustUsed(OriginalHook(AutoCrossbow), 1f) ||
                     JustUsed(OriginalHook(Heatblast), 1f)) && HasNotWeaved)
                {
                    if (ActionReady(GaussRound) &&
                        (UseGaussRound || !LevelChecked(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(Ricochet) && UseRicochet)
                        return OriginalHook(Ricochet);
                }
            }

            if (!IsOverheated)
            {
                //Full Metal Field
                if (HasStatusEffect(Buffs.FullMetalMachinist) &&
                    LevelChecked(FullMetalField))
                    return FullMetalField;

                if (ActionReady(BioBlaster) &&
                    !HasStatusEffect(Debuffs.Bioblaster, CurrentTarget) &&
                    !IsOverheated && !HasStatusEffect(Buffs.Reassembled) &&
                    CanApplyStatus(CurrentTarget, Debuffs.Bioblaster))
                    return OriginalHook(BioBlaster);

                if (ActionReady(Flamethrower) &&
                    !HasStatusEffect(Buffs.Reassembled) &&
                    !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(2.5f) &&
                    GetTargetHPPercent() > 50)
                    return OriginalHook(Flamethrower);

                if (LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady))
                    return Excavator;

                if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
                    return Chainsaw;

                if (LevelChecked(AirAnchor) && IsOffCooldown(AirAnchor))
                    return AirAnchor;
            }

            if (ActionReady(BlazingShot) && IsOverheated)
                return HasBattleTarget() &&
                       (!LevelChecked(CheckMate) ||
                        LevelChecked(CheckMate) &&
                        NumberOfEnemiesInRange(AutoCrossbow, CurrentTarget) >= 5)
                    ? AutoCrossbow
                    : BlazingShot;

            return actionID;
        }
    }

    internal class MCH_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SplitShot or HeatedSplitShot))
                return actionID;

            // Opener
            if (IsEnabled(Preset.MCH_ST_Adv_Opener) &&
                HasBattleTarget() &&
                Opener().FullOpener(ref actionID))
                return actionID;

            //Reassemble to start before combat
            if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) &&
                !HasStatusEffect(Buffs.Reassembled) && ActionReady(Reassemble) &&
                !InCombat() && HasBattleTarget() &&
                (ActionReady(Excavator) && MCH_ST_Reassembled[0] ||
                 ActionReady(Chainsaw) && MCH_ST_Reassembled[1] ||
                 LevelChecked(AirAnchor) && IsOffCooldown(AirAnchor) && MCH_ST_Reassembled[2] ||
                 ActionReady(Drill) && MCH_ST_Reassembled[3]))
                return Reassemble;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (IsEnabled(Preset.MCH_ST_Adv_QueenOverdrive) &&
                    RobotActive && ActionReady(RookOverdrive) &&
                    GetTargetHPPercent() <= MCH_ST_QueenOverDriveHPThreshold)
                    return OriginalHook(RookOverdrive);

                // Wildfire
                if (IsEnabled(Preset.MCH_ST_Adv_WildFire) &&
                    (MCH_ST_WildfireBossOption == 0 || TargetIsBoss()) &&
                    CanApplyStatus(CurrentTarget, Debuffs.Wildfire) &&
                    JustUsed(Hypercharge) && ActionReady(Wildfire) &&
                    !HasStatusEffect(Buffs.Wildfire))
                    return Wildfire;

                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (IsEnabled(Preset.MCH_ST_Adv_Stabilizer) &&
                        (MCH_ST_BarrelStabilizerBossOption == 0 || TargetIsBoss()) &&
                        ActionReady(BarrelStabilizer) && !HasStatusEffect(Buffs.FullMetalMachinist))
                        return BarrelStabilizer;

                    // Hypercharge
                    if (IsEnabled(Preset.MCH_ST_Adv_Hypercharge) &&
                        (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) &&
                        !IsComboExpiring(6) && ActionReady(Hypercharge) &&
                        GetTargetHPPercent() > HPThresholdHyperchargeST)
                    {
                        // Ensures Hypercharge is double weaved with WF
                        if (LevelChecked(FullMetalField) && JustUsed(FullMetalField) ||
                            !LevelChecked(FullMetalField) && ActionReady(Wildfire) ||
                            !LevelChecked(Wildfire))
                            return Hypercharge;

                        // Only Hypercharge when tools are on cooldown
                        if (DrillCD && AnchorCD && SawCD &&
                            (!LevelChecked(Wildfire) ||
                             LevelChecked(Wildfire) &&
                             (GetCooldownRemainingTime(Wildfire) > 40 ||
                              IsOffCooldown(Wildfire) && !HasStatusEffect(Buffs.FullMetalMachinist))))
                            return Hypercharge;
                    }

                    // Queen
                    if (IsEnabled(Preset.MCH_ST_Adv_TurretQueen) &&
                        UseQueen())
                        return OriginalHook(RookAutoturret);

                    // Reassemble
                    if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) &&
                        GetRemainingCharges(Reassemble) > MCH_ST_ReassemblePool &&
                        Reassembled() &&
                        GetTargetHPPercent() > HPThresholdReassembleST)
                        return Reassemble;

                    // Gauss Round and Ricochet outside HC
                    if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                        (JustUsed(OriginalHook(AirAnchor), 2f) ||
                         JustUsed(Chainsaw, 2f) ||
                         JustUsed(Drill, 2f) ||
                         JustUsed(Excavator, 2f)))
                    {
                        if (ActionReady(GaussRound) &&
                            GetRemainingCharges(OriginalHook(GaussRound)) > MCH_ST_GaussRicoPool &&
                            !JustUsed(OriginalHook(GaussRound), 2f))
                            return OriginalHook(GaussRound);

                        if (ActionReady(Ricochet) &&
                            GetRemainingCharges(OriginalHook(Ricochet)) > MCH_ST_GaussRicoPool &&
                            !JustUsed(OriginalHook(Ricochet), 2f))
                            return OriginalHook(Ricochet);
                    }

                    if (ActionReady(Tactician) &&
                        IsEnabled(Preset.MCH_ST_Adv_Tactician) && RaidWideCasting() &&
                        NumberOfAlliesInRange(Tactician) >= GetPartyMembers().Count * .75 &&
                        !HasAnyStatusEffects([BRD.Buffs.Troubadour, DNC.Buffs.ShieldSamba, Buffs.Tactician], anyOwner: true))
                        return Tactician;

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_ST_Adv_Interrupt, WeaveTypes.DelayWeave))
                        return Role.HeadGraze;

                    if (IsEnabled(Preset.MCH_ST_Dismantle) &&
                        ActionReady(Dismantle) &&
                        !HasStatusEffect(Debuffs.Dismantled, CurrentTarget, true) &&
                        CanApplyStatus(CurrentTarget, Debuffs.Dismantled) &&
                        RaidWideCasting())
                        return Dismantle;

                    // Healing
                    if (IsEnabled(Preset.MCH_ST_Adv_SecondWind) &&
                        Role.CanSecondWind(MCH_ST_SecondWindHPThreshold))
                        return Role.SecondWind;
                }

                // Gauss Round and Ricochet during HC
                if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                    JustUsed(OriginalHook(Heatblast), 1f) && HasNotWeaved)
                {
                    if (ActionReady(GaussRound) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) > MCH_ST_GaussRicoPool &&
                        (UseGaussRound || !LevelChecked(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(Ricochet) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) > MCH_ST_GaussRicoPool &&
                        UseRicochet)
                        return OriginalHook(Ricochet);
                }
            }

            //Tools
            if (Tools(ref actionID))
                return actionID;

            // Full Metal Field
            if (IsEnabled(Preset.MCH_ST_Adv_Stabilizer_FullMetalField) &&
                HasStatusEffect(Buffs.FullMetalMachinist, out Status? fullMetal) &&
                !JustUsed(BarrelStabilizer) &&
                (ActionReady(Wildfire) ||
                 fullMetal.RemainingTime <= 6))
                return FullMetalField;

            // Heatblast
            if (IsEnabled(Preset.MCH_ST_Adv_Heatblast) &&
                IsOverheated && ActionReady(Heatblast))
                return OriginalHook(Heatblast);

            // 1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(SlugShot))
                    return OriginalHook(SlugShot);

                if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) && MCH_ST_Reassembled[4] &&
                    ComboAction is SlugShot &&
                    !LevelChecked(Drill) && !HasStatusEffect(Buffs.Reassembled) && ActionReady(Reassemble))
                    return Reassemble;

                if (ComboAction is SlugShot && LevelChecked(CleanShot))
                    return OriginalHook(CleanShot);
            }
            return actionID;
        }
    }

    internal class MCH_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpreadShot or Scattergun))
                return actionID;

            bool reassembledScattergunAoE = IsEnabled(Preset.MCH_AoE_Adv_Reassemble) &&
                                            MCH_AoE_Reassembled[0] && HasStatusEffect(Buffs.Reassembled);

            bool reassembledChainsawAoE =
                IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[2] && HasStatusEffect(Buffs.Reassembled) ||
                IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && !MCH_AoE_Reassembled[2] && !HasStatusEffect(Buffs.Reassembled) ||
                !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_AoE_ReassemblePool ||
                !IsEnabled(Preset.MCH_AoE_Adv_Reassemble);

            bool reassembledExcavatorAoE =
                IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[3] && HasStatusEffect(Buffs.Reassembled) ||
                IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && !MCH_AoE_Reassembled[3] && !HasStatusEffect(Buffs.Reassembled) ||
                !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_AoE_ReassemblePool ||
                !IsEnabled(Preset.MCH_AoE_Adv_Reassemble);

            bool reassembledAirAnchorAoE =
                IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && MCH_AoE_Reassembled[1] && HasStatusEffect(Buffs.Reassembled) ||
                IsEnabled(Preset.MCH_AoE_Adv_Reassemble) && !MCH_AoE_Reassembled[1] && !HasStatusEffect(Buffs.Reassembled) ||
                !HasStatusEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= MCH_AoE_ReassemblePool ||
                !IsEnabled(Preset.MCH_AoE_Adv_Reassemble);

            if (HasStatusEffect(Buffs.Flamethrower) || JustUsed(Flamethrower, GCD))
                return All.SavageBlade;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (!IsOverheated)
                {
                    if (IsEnabled(Preset.MCH_AoE_Adv_QueenOverdrive) &&
                        Gauge.IsRobotActive && ActionReady(RookOverdrive) &&
                        GetTargetHPPercent() <= MCH_AoE_QueenOverDriveHPThreshold)
                        return OriginalHook(RookOverdrive);

                    // BarrelStabilizer
                    if (IsEnabled(Preset.MCH_AoE_Adv_Stabilizer) &&
                        ActionReady(BarrelStabilizer) && !HasStatusEffect(Buffs.FullMetalMachinist) &&
                        GetTargetHPPercent() > MCH_AoE_BarrelStabilizerHPThreshold)
                        return BarrelStabilizer;

                    if (IsEnabled(Preset.MCH_AoE_Adv_Queen) &&
                        Battery >= MCH_AoE_TurretBatteryUsage &&
                        GetTargetHPPercent() > MCH_AoE_QueenHpThreshold)
                        return OriginalHook(RookAutoturret);

                    // Hypercharge
                    if (IsEnabled(Preset.MCH_AoE_Adv_Hypercharge) &&
                        (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                        LevelChecked(AutoCrossbow) &&
                        GetTargetHPPercent() > MCH_AoE_HyperchargeHPThreshold &&
                        (LevelChecked(BioBlaster) && GetCooldownRemainingTime(BioBlaster) > 10 ||
                         !LevelChecked(BioBlaster) || IsNotEnabled(Preset.MCH_AoE_Adv_Bioblaster)) &&
                        (LevelChecked(Flamethrower) && GetCooldownRemainingTime(Flamethrower) > 10 ||
                         !LevelChecked(Flamethrower) || IsNotEnabled(Preset.MCH_AoE_Adv_FlameThrower)))
                        return Hypercharge;

                    if (IsEnabled(Preset.MCH_AoE_Adv_Reassemble) &&
                        GetTargetHPPercent() > MCH_AoE_ReassembleHPThreshold &&
                        ActionReady(Reassemble) && !HasStatusEffect(Buffs.Wildfire) &&
                        !HasStatusEffect(Buffs.Reassembled) && !JustUsed(Flamethrower, 10f) &&
                        GetRemainingCharges(Reassemble) > MCH_AoE_ReassemblePool &&
                        (MCH_AoE_Reassembled[0] && LevelChecked(Scattergun) ||
                         IsOverheated && MCH_AoE_Reassembled[1] && LevelChecked(AutoCrossbow) ||
                         GetCooldownRemainingTime(Chainsaw) < 1 && MCH_AoE_Reassembled[2] && LevelChecked(Chainsaw) ||
                         GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && MCH_AoE_Reassembled[3] &&
                         LevelChecked(Excavator)))
                        return Reassemble;

                    //gauss and ricochet outside HC
                    if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet))
                    {
                        if (ActionReady(GaussRound) &&
                            !JustUsed(OriginalHook(GaussRound), 2.5f))
                            return OriginalHook(GaussRound);

                        if (ActionReady(Ricochet) &&
                            !JustUsed(OriginalHook(Ricochet), 2.5f))
                            return OriginalHook(Ricochet);
                    }

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_AoE_Adv_Interrupt, WeaveTypes.DelayWeave))
                        return Role.HeadGraze;

                    if (IsEnabled(Preset.MCH_AoE_Adv_SecondWind) &&
                        Role.CanSecondWind(MCH_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;
                }

                //AutoCrossbow, Gauss, Rico
                if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet) &&
                    IsOverheated &&
                    (JustUsed(OriginalHook(AutoCrossbow), 1f) ||
                     JustUsed(OriginalHook(Heatblast), 1f)) && HasNotWeaved)
                {
                    if (ActionReady(GaussRound) &&
                        (UseGaussRound || !LevelChecked(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(Ricochet) && UseRicochet)
                        return OriginalHook(Ricochet);
                }
            }

            if (!IsOverheated)
            {
                //Full Metal Field
                if (IsEnabled(Preset.MCH_AoE_Adv_Stabilizer_FullMetalField) &&
                    HasStatusEffect(Buffs.FullMetalMachinist) && LevelChecked(FullMetalField))
                    return FullMetalField;

                if (IsEnabled(Preset.MCH_AoE_Adv_Bioblaster) &&
                    ActionReady(BioBlaster) && !HasStatusEffect(Debuffs.Bioblaster, CurrentTarget) &&
                    !IsOverheated && !HasStatusEffect(Buffs.Reassembled) &&
                    CanApplyStatus(CurrentTarget, Debuffs.Bioblaster))
                    return OriginalHook(BioBlaster);

                if (IsEnabled(Preset.MCH_AoE_Adv_FlameThrower) &&
                    ActionReady(Flamethrower) &&
                    !HasStatusEffect(Buffs.Reassembled) &&
                    (MCH_AoE_FlamethrowerMovement == 1 ||
                     MCH_AoE_FlamethrowerMovement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(MCH_AoE_FlamehrowerTimeStill)) &&
                    GetTargetHPPercent() > MCH_AoE_FlamethrowerHPOption)
                    return OriginalHook(Flamethrower);

                if (IsEnabled(Preset.MCH_AoE_Adv_Excavator) &&
                    reassembledExcavatorAoE &&
                    LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady))
                    return Excavator;

                if (IsEnabled(Preset.MCH_AoE_Adv_Chainsaw) &&
                    reassembledChainsawAoE &&
                    GetTargetHPPercent() >= MCH_AoE_ChainsawHPThreshold &&
                    ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
                    return Chainsaw;

                if (IsEnabled(Preset.MCH_AoE_Adv_AirAnchor) &&
                    reassembledAirAnchorAoE &&
                    LevelChecked(AirAnchor) && IsOffCooldown(AirAnchor))
                    return AirAnchor;

                if (reassembledScattergunAoE)
                    return OriginalHook(Scattergun);
            }

            if (ActionReady(BlazingShot) && IsOverheated)
                return HasBattleTarget() &&
                       (!LevelChecked(CheckMate) ||
                        LevelChecked(CheckMate) &&
                        NumberOfEnemiesInRange(AutoCrossbow, CurrentTarget) >= 5)
                    ? AutoCrossbow
                    : BlazingShot;

            return actionID;
        }
    }

    internal class MCH_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (CleanShot or HeatedCleanShot))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(SlugShot))
                    return OriginalHook(SlugShot);

                if (ComboAction is SlugShot && LevelChecked(CleanShot))
                    return OriginalHook(CleanShot);
            }

            return OriginalHook(SplitShot);
        }
    }

    internal class MCH_DismantleProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_DismantleProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Dismantle)
                return actionID;

            return HasStatusEffect(Debuffs.Dismantled, CurrentTarget, true) && IsOffCooldown(Dismantle)
                ? All.SavageBlade
                : actionID;
        }
    }

    internal class MCH_DismantleTactician : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_DismantleTactician;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Dismantle)
                return actionID;

            return (IsOnCooldown(Dismantle) || !LevelChecked(Dismantle) || !HasBattleTarget()) &&
                   ActionReady(Tactician) && !HasStatusEffect(Buffs.Tactician)
                ? Tactician
                : actionID;
        }
    }

    internal class MCH_HeatblastGaussRicochet : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_Heatblast;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Heatblast or BlazingShot))
                return actionID;

            if (IsEnabled(Preset.MCH_Heatblast_AutoBarrel) &&
                ActionReady(BarrelStabilizer) && !IsOverheated &&
                !HasStatusEffect(Buffs.FullMetalMachinist))
                return BarrelStabilizer;

            if (IsEnabled(Preset.MCH_Heatblast_Wildfire) &&
                ActionReady(Wildfire) && JustUsed(Hypercharge) &&
                !HasStatusEffect(Buffs.Wildfire) &&
                CanApplyStatus(CurrentTarget, Debuffs.Wildfire))
                return Wildfire;

            if (!IsOverheated && LevelChecked(Hypercharge) &&
                (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)))
                return Hypercharge;

            if (IsEnabled(Preset.MCH_Heatblast_GaussRound) &&
                CanWeave() &&
                JustUsed(OriginalHook(Heatblast), 1f) &&
                HasNotWeaved)
            {
                if (ActionReady(GaussRound) &&
                    (UseGaussRound || !LevelChecked(Ricochet)))
                    return OriginalHook(GaussRound);

                if (ActionReady(Ricochet) && UseRicochet)
                    return OriginalHook(Ricochet);
            }

            if (IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                return OriginalHook(Heatblast);

            return actionID;
        }
    }

    internal class MCH_AutoCrossbowGaussRicochet : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_AutoCrossbow;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AutoCrossbow)
                return actionID;

            if (IsEnabled(Preset.MCH_AutoCrossbow_AutoBarrel) &&
                ActionReady(BarrelStabilizer) && !IsOverheated &&
                !HasStatusEffect(Buffs.FullMetalMachinist))
                return BarrelStabilizer;

            if (!IsOverheated && LevelChecked(Hypercharge) &&
                (Heat >= 50 || HasStatusEffect(Buffs.Hypercharged)))
                return Hypercharge;

            if (IsEnabled(Preset.MCH_AutoCrossbow_GaussRound) &&
                CanWeave() && JustUsed(OriginalHook(AutoCrossbow), 1f) && HasNotWeaved)
            {
                if (ActionReady(GaussRound) &&
                    UseGaussRound || !LevelChecked(Ricochet))
                    return OriginalHook(GaussRound);

                if (ActionReady(Ricochet) && UseRicochet)
                    return OriginalHook(Ricochet);
            }

            if (IsOverheated && ActionReady(AutoCrossbow))
                return OriginalHook(AutoCrossbow);

            return actionID;
        }
    }

    internal class MCH_Overdrive : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_Overdrive;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (AutomatonQueen or RookAutoturret))
                return actionID;

            return RobotActive
                ? OriginalHook(QueenOverdrive)
                : actionID;
        }
    }

    internal class MCH_BigHitter : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_BigHitter;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not HotShot)
                return actionID;

            return actionID switch
            {
                HotShot when LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady) => CalcBestAction(actionID, Excavator, Chainsaw, AirAnchor, Drill),
                HotShot when LevelChecked(Chainsaw) => CalcBestAction(actionID, Chainsaw, AirAnchor, Drill),
                HotShot when LevelChecked(AirAnchor) => CalcBestAction(actionID, AirAnchor, Drill),
                HotShot when LevelChecked(Drill) => CalcBestAction(actionID, Drill, HotShot),
                HotShot when !LevelChecked(Drill) => HotShot,
                var _ => actionID
            };
        }
    }

    internal class MCH_GaussRoundRicochet : CustomCombo
    {
        protected internal override Preset Preset => Preset.MCH_GaussRoundRicochet;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (GaussRound or Ricochet or CheckMate or DoubleCheck))
                return actionID;

            return actionID switch
            {
                GaussRound or DoubleCheck when MCH_GaussRico == 0 && ActionReady(GaussRound) && (UseGaussRound || !LevelChecked(Ricochet)) => OriginalHook(GaussRound),
                GaussRound or DoubleCheck when MCH_GaussRico == 0 && ActionReady(Ricochet) && UseRicochet => OriginalHook(Ricochet),
                Ricochet or CheckMate when MCH_GaussRico == 1 && ActionReady(GaussRound) && (UseGaussRound || !LevelChecked(Ricochet)) => OriginalHook(GaussRound),
                Ricochet or CheckMate when MCH_GaussRico == 1 && ActionReady(Ricochet) && UseRicochet => OriginalHook(Ricochet),
                var _ => actionID
            };
        }
    }
}
