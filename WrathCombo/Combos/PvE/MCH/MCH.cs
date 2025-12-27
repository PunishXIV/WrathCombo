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

            //Reassemble
            if (CanReassemble() &&
                !InCombat())
                return Reassemble;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (OvercapGaussRound)
                    return OriginalHook(GaussRound);

                if (OvercapRicochet)
                    return OriginalHook(Ricochet);

                if (RobotActive && ActionReady(RookOverdrive) &&
                    GetTargetHPPercent() <= 1)
                    return OriginalHook(RookOverdrive);

                // Wildfire
                if (TargetIsBoss() &&
                    CanApplyStatus(CurrentTarget, Debuffs.Wildfire) &&
                    ActionReady(Wildfire) && JustUsed(Hypercharge, GCD + 0.9f) &&
                    !HasStatusEffect(Buffs.Wildfire))
                    return Wildfire;

                // Gauss Round and Ricochet during HC
                if (JustUsed(OriginalHook(Heatblast), 1f) && !HasWeaved())
                {
                    if (CanGaussRound || !LevelChecked(Ricochet))
                        return OriginalHook(GaussRound);

                    if (CanRicochet)
                        return OriginalHook(Ricochet);
                }

                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (TargetIsBoss() &&
                        DrillCD && AirAnchorCD && ChainSawCD &&
                        GetCooldownRemainingTime(Wildfire) <= GCD &&
                        ActionReady(BarrelStabilizer) &&
                        !HasStatusEffect(Buffs.FullMetalMachinist))
                        return BarrelStabilizer;

                    // Queen
                    if (CanQueen())
                        return OriginalHook(RookAutoturret);

                    // Reassemble
                    if (CanReassemble())
                        return Reassemble;

                    // Hypercharge
                    if (CanHypercharge())
                        return Hypercharge;

                    // Gauss Round and Ricochet outside HC
                    if (JustUsed(OriginalHook(AirAnchor), 2f) ||
                        JustUsed(Chainsaw, 2f) ||
                        JustUsed(Drill, 2f) ||
                        JustUsed(Excavator, 2f))
                    {
                        if (CanGaussRound && (!JustUsed(OriginalHook(GaussRound), 2f) || !LevelChecked(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (CanRicochet && !JustUsed(OriginalHook(Ricochet), 2f))
                            return OriginalHook(Ricochet);
                    }

                    // Healing
                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    // Interrupt
                    if (Role.CanHeadGraze(true))
                        return Role.HeadGraze;
                }
            }

            // Full Metal Field
            if (HasStatusEffect(Buffs.FullMetalMachinist, out IStatus? fullMetal) &&
                !IsOverheated &&
                (ActionReady(Wildfire) ||
                 GetCooldownRemainingTime(Wildfire) > 90 ||
                 fullMetal.RemainingTime <= 6))
                return FullMetalField;

            //Tools
            if (CanUseTools(ref actionID) && !IsOverheated)
                return actionID;

            // Heatblast
            if (IsOverheated && ActionReady(Heatblast))
                return OriginalHook(Heatblast);

            // 1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(SlugShot))
                    return OriginalHook(SlugShot);

                if (ComboAction is SlugShot && !LevelChecked(Drill) &&
                    LevelChecked(CleanShot) && !HasStatusEffect(Buffs.Reassembled) &&
                    ActionReady(Reassemble))
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

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (OvercapGaussRound)
                    return OriginalHook(GaussRound);

                if (OvercapRicochet)
                    return OriginalHook(Ricochet);

                //AutoCrossbow, Gauss, Rico
                if (IsOverheated && !HasWeaved() &&
                    (JustUsed(OriginalHook(AutoCrossbow), 1f) ||
                     JustUsed(OriginalHook(Heatblast), 1f)))
                {
                    if (CanGaussRound || !LevelChecked(Ricochet))
                        return OriginalHook(GaussRound);

                    if (CanRicochet)
                        return OriginalHook(Ricochet);
                }

                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (ActionReady(BarrelStabilizer) &&
                        !HasStatusEffect(Buffs.FullMetalMachinist))
                        return BarrelStabilizer;

                    if (Battery is 100)
                        return OriginalHook(RookAutoturret);

                    if (ActionReady(Reassemble) && !HasStatusEffect(Buffs.Wildfire) &&
                        !HasStatusEffect(Buffs.Reassembled) && !JustUsed(Flamethrower, 10f) &&
                        GetRemainingCharges(Reassemble) > MCH_AoE_ReassemblePool &&
                        (LevelChecked(Scattergun) ||
                         GetCooldownRemainingTime(AirAnchor) < GCD && LevelChecked(AirAnchor) ||
                         GetCooldownRemainingTime(Chainsaw) < GCD && LevelChecked(Chainsaw) ||
                         GetCooldownRemainingTime(OriginalHook(Chainsaw)) < GCD && LevelChecked(Excavator)))
                        return Reassemble;

                    // Hypercharge
                    if (CanHypercharge(true))
                        return Hypercharge;

                    //gauss and ricochet outside HC
                    if (CanGaussRound &&
                        (!JustUsed(OriginalHook(GaussRound), 2.5f) || !LevelChecked(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (CanRicochet &&
                        !JustUsed(OriginalHook(Ricochet), 2.5f))
                        return OriginalHook(Ricochet);

                    // Interrupt
                    if (Role.CanHeadGraze(true))
                        return Role.HeadGraze;

                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;
                }
            }

            if (!IsOverheated)
            {
                //Full Metal Field
                if (LevelChecked(FullMetalField) &&
                    HasStatusEffect(Buffs.FullMetalMachinist))
                    return FullMetalField;

                if (ActionReady(Flamethrower) &&
                    !HasStatusEffect(Buffs.Reassembled) &&
                    !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(3))
                    return OriginalHook(Flamethrower);

                if (ActionReady(BioBlaster) && !HasStatusEffect(Debuffs.Bioblaster, CurrentTarget) &&
                    !HasStatusEffect(Buffs.Reassembled) && CanApplyStatus(CurrentTarget, Debuffs.Bioblaster))
                    return OriginalHook(BioBlaster);

                if (LevelChecked(Excavator) &&
                    HasStatusEffect(Buffs.ExcavatorReady))
                    return Excavator;

                if (ActionReady(Chainsaw) &&
                    !HasStatusEffect(Buffs.ExcavatorReady))
                    return Chainsaw;

                if (LevelChecked(AirAnchor) &&
                    IsOffCooldown(AirAnchor))
                    return AirAnchor;

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
                !InCombat() && CanReassemble())
                return Reassemble;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet))
                {
                    if (OvercapGaussRound)
                        return OriginalHook(GaussRound);

                    if (OvercapRicochet)
                        return OriginalHook(Ricochet);
                }

                if (IsEnabled(Preset.MCH_ST_Adv_QueenOverdrive) &&
                    RobotActive && ActionReady(RookOverdrive) &&
                    GetTargetHPPercent() <= MCH_ST_QueenOverDriveHPThreshold)
                    return OriginalHook(RookOverdrive);

                // Wildfire
                if (IsEnabled(Preset.MCH_ST_Adv_WildFire) &&
                    (MCH_ST_WildfireBossOption == 0 && GetTargetHPPercent() > HPThresholdWildFire || TargetIsBoss()) &&
                    CanApplyStatus(CurrentTarget, Debuffs.Wildfire) &&
                    ActionReady(Wildfire) && JustUsed(Hypercharge, GCD + 0.9f) &&
                    !HasStatusEffect(Buffs.Wildfire))
                    return Wildfire;

                // Gauss Round and Ricochet during HC
                if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                    JustUsed(OriginalHook(Heatblast), 1f) && !HasWeaved())
                {
                    if (GetRemainingCharges(OriginalHook(GaussRound)) > MCH_ST_GaussRicoPool &&
                        (CanGaussRound || !LevelChecked(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (GetRemainingCharges(OriginalHook(Ricochet)) > MCH_ST_GaussRicoPool &&
                        CanRicochet)
                        return OriginalHook(Ricochet);
                }

                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (IsEnabled(Preset.MCH_ST_Adv_Stabilizer) &&
                        (MCH_ST_BarrelStabilizerBossOption == 0 && GetTargetHPPercent() > HPThresholdBarrelStabilizer || TargetIsBoss()) &&
                        DrillCD && AirAnchorCD && ChainSawCD && GetCooldownRemainingTime(Wildfire) <= GCD &&
                        ActionReady(BarrelStabilizer) && !HasStatusEffect(Buffs.FullMetalMachinist))
                        return BarrelStabilizer;

                    // Queen
                    if (IsEnabled(Preset.MCH_ST_Adv_TurretQueen) &&
                        CanQueen())
                        return OriginalHook(RookAutoturret);

                    // Reassemble
                    if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) &&
                        GetRemainingCharges(Reassemble) > MCH_ST_ReassemblePool &&
                        GetTargetHPPercent() > HPThresholdReassemble &&
                        CanReassemble())
                        return Reassemble;

                    // Hypercharge
                    if (IsEnabled(Preset.MCH_ST_Adv_Hypercharge) &&
                        GetTargetHPPercent() > HPThresholdHypercharge &&
                        CanHypercharge())
                        return Hypercharge;

                    // Gauss Round and Ricochet outside HC
                    if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                        (JustUsed(OriginalHook(AirAnchor), 2f) ||
                         JustUsed(Chainsaw, 2f) ||
                         JustUsed(Drill, 2f) ||
                         JustUsed(Excavator, 2f)))
                    {
                        if (GetRemainingCharges(OriginalHook(GaussRound)) > MCH_ST_GaussRicoPool &&
                            CanGaussRound && (!JustUsed(OriginalHook(GaussRound), 2f) || !LevelChecked(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (GetRemainingCharges(OriginalHook(Ricochet)) > MCH_ST_GaussRicoPool &&
                            CanRicochet && !JustUsed(OriginalHook(Ricochet), 2f))
                            return OriginalHook(Ricochet);
                    }

                    if (ActionReady(Tactician) &&
                        IsEnabled(Preset.MCH_ST_Adv_Tactician) && RaidWideCasting() &&
                        NumberOfAlliesInRange(Tactician) >= GetPartyMembers().Count * .75 &&
                        !HasAnyStatusEffects([BRD.Buffs.Troubadour, DNC.Buffs.ShieldSamba, Buffs.Tactician], anyOwner: true))
                        return Tactician;

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

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_ST_Adv_Interrupt))
                        return Role.HeadGraze;
                }
            }

            // Full Metal Field
            if (IsEnabled(Preset.MCH_ST_Adv_Stabilizer_FullMetalField) &&
                HasStatusEffect(Buffs.FullMetalMachinist, out IStatus? fullMetal) &&
                !IsOverheated &&
                (ActionReady(Wildfire) ||
                 GetCooldownRemainingTime(Wildfire) > 90 ||
                 fullMetal.RemainingTime <= 6))
                return FullMetalField;

            //Tools
            if (IsEnabled(Preset.MCH_ST_Adv_Tools) &&
                GetTargetHPPercent() > HPThresholdTools &&
                CanUseTools(ref actionID) && !IsOverheated)
                return actionID;

            // Heatblast
            if (IsEnabled(Preset.MCH_ST_Adv_Heatblast) &&
                ActionReady(Heatblast) && IsOverheated)
                return OriginalHook(Heatblast);

            // 1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(SlugShot))
                    return OriginalHook(SlugShot);

                if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) &&
                    ComboAction is SlugShot && !LevelChecked(Drill) && LevelChecked(CleanShot) &&
                    !HasStatusEffect(Buffs.Reassembled) && ActionReady(Reassemble))
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

            if (HasStatusEffect(Buffs.Flamethrower) || JustUsed(Flamethrower, GCD))
                return All.SavageBlade;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet))
                {
                    if (OvercapGaussRound)
                        return OriginalHook(GaussRound);

                    if (OvercapRicochet)
                        return OriginalHook(Ricochet);
                }

                if (IsEnabled(Preset.MCH_AoE_Adv_QueenOverdrive) &&
                    Gauge.IsRobotActive && ActionReady(RookOverdrive) &&
                    GetTargetHPPercent() <= MCH_AoE_QueenOverDriveHPThreshold)
                    return OriginalHook(RookOverdrive);

                //AutoCrossbow, Gauss, Rico
                if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet) &&
                    IsOverheated &&
                    (JustUsed(OriginalHook(AutoCrossbow), 1f) ||
                     JustUsed(OriginalHook(Heatblast), 1f)) && !HasWeaved())
                {
                    if (CanGaussRound || !LevelChecked(Ricochet))
                        return OriginalHook(GaussRound);

                    if (CanRicochet)
                        return OriginalHook(Ricochet);
                }

                if (!IsOverheated)
                {
                    // BarrelStabilizer
                    if (IsEnabled(Preset.MCH_AoE_Adv_Stabilizer) &&
                        ActionReady(BarrelStabilizer) && !HasStatusEffect(Buffs.FullMetalMachinist) &&
                        GetTargetHPPercent() > MCH_AoE_BarrelStabilizerHPThreshold)
                        return BarrelStabilizer;

                    if (IsEnabled(Preset.MCH_AoE_Adv_Queen) &&
                        Battery >= MCH_AoE_TurretBatteryUsage &&
                        GetTargetHPPercent() > MCH_AoE_QueenHpThreshold)
                        return OriginalHook(RookAutoturret);

                    if (IsEnabled(Preset.MCH_AoE_Adv_Reassemble) &&
                        GetTargetHPPercent() > MCH_AoE_ReassembleHPThreshold &&
                        ActionReady(Reassemble) && !HasStatusEffect(Buffs.Reassembled) &&
                        !JustUsed(Flamethrower, 10f) &&
                        GetRemainingCharges(Reassemble) > MCH_AoE_ReassemblePool &&
                        (LevelChecked(Scattergun) ||
                         GetCooldownRemainingTime(AirAnchor) < GCD && LevelChecked(AirAnchor) ||
                         GetCooldownRemainingTime(Chainsaw) < GCD && LevelChecked(Chainsaw) ||
                         GetCooldownRemainingTime(OriginalHook(Chainsaw)) < GCD && LevelChecked(Excavator)))
                        return Reassemble;

                    // Hypercharge
                    if (IsEnabled(Preset.MCH_AoE_Adv_Hypercharge) &&
                        GetTargetHPPercent() > MCH_AoE_HyperchargeHPThreshold &&
                        CanHypercharge(true))
                        return Hypercharge;

                    //gauss and ricochet outside HC
                    if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet))
                    {
                        if (CanGaussRound && (!JustUsed(OriginalHook(GaussRound), 2.5f) || !LevelChecked(Ricochet)))
                            return OriginalHook(GaussRound);

                        if (CanRicochet && !JustUsed(OriginalHook(Ricochet), 2.5f))
                            return OriginalHook(Ricochet);
                    }

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_AoE_Adv_Interrupt))
                        return Role.HeadGraze;

                    if (IsEnabled(Preset.MCH_AoE_Adv_SecondWind) &&
                        Role.CanSecondWind(MCH_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;
                }
            }

            if (!IsOverheated)
            {
                //Full Metal Field
                if (IsEnabled(Preset.MCH_AoE_Adv_Stabilizer_FullMetalField) &&
                    LevelChecked(FullMetalField) && HasStatusEffect(Buffs.FullMetalMachinist))
                    return FullMetalField;

                if (IsEnabled(Preset.MCH_AoE_Adv_FlameThrower) &&
                    ActionReady(Flamethrower) &&
                    !HasStatusEffect(Buffs.Reassembled) &&
                    (MCH_AoE_FlamethrowerMovement == 1 ||
                     MCH_AoE_FlamethrowerMovement == 0 && !IsMoving() &&
                     TimeStoodStill > TimeSpan.FromSeconds(MCH_AoE_FlamethrowerTimeStill)) &&
                    GetTargetHPPercent() > MCH_AoE_FlamethrowerHPOption)
                    return OriginalHook(Flamethrower);

                if (IsEnabled(Preset.MCH_AoE_Adv_Tools) &&
                    GetTargetHPPercent() >= MCH_AoE_ToolsHPThreshold)
                {
                    if (ActionReady(BioBlaster) && !HasStatusEffect(Debuffs.Bioblaster, CurrentTarget) &&
                        !HasStatusEffect(Buffs.Reassembled) && CanApplyStatus(CurrentTarget, Debuffs.Bioblaster))
                        return OriginalHook(BioBlaster);

                    if (LevelChecked(Excavator) && HasStatusEffect(Buffs.ExcavatorReady))
                        return Excavator;

                    if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
                        return Chainsaw;

                    if (LevelChecked(AirAnchor) && IsOffCooldown(AirAnchor))
                        return AirAnchor;
                }
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
                !HasWeaved())
            {
                if (ActionReady(GaussRound) &&
                    (CanGaussRound || !LevelChecked(Ricochet)))
                    return OriginalHook(GaussRound);

                if (ActionReady(Ricochet) && CanRicochet)
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
                CanWeave() && JustUsed(OriginalHook(AutoCrossbow), 1f) && !HasWeaved())
            {
                if (ActionReady(GaussRound) &&
                    CanGaussRound || !LevelChecked(Ricochet))
                    return OriginalHook(GaussRound);

                if (ActionReady(Ricochet) && CanRicochet)
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
                GaussRound or DoubleCheck when MCH_GaussRico == 0 && ActionReady(GaussRound) && (CanGaussRound || !LevelChecked(Ricochet)) => OriginalHook(GaussRound),
                GaussRound or DoubleCheck when MCH_GaussRico == 0 && ActionReady(Ricochet) && CanRicochet => OriginalHook(Ricochet),
                Ricochet or CheckMate when MCH_GaussRico == 1 && ActionReady(GaussRound) && (CanGaussRound || !LevelChecked(Ricochet)) => OriginalHook(GaussRound),
                Ricochet or CheckMate when MCH_GaussRico == 1 && ActionReady(Ricochet) && CanRicochet => OriginalHook(Ricochet),
                var _ => actionID
            };
        }
    }
}
