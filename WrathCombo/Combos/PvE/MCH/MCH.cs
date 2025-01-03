using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal static partial class MCH
{
    internal class MCH_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SplitShot or HeatedSplitShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave())
                return Variant.VariantRampart;

            // Opener
            if (TargetIsHostile())
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            //Reassemble to start before combat
            if (!HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) &&
                !InCombat() && TargetIsHostile())
                return Reassemble;

            // Interrupt
            if (InterruptReady)
                return All.HeadGraze;

            // All weaves
            if (CanWeave())
            {
                if (!ActionWatching.HasDoubleWeaved())
                {
                    // Wildfire
                    if (JustUsed(Hypercharge) && ActionReady(Wildfire))
                        return Wildfire;

                    if (!Gauge.IsOverheated)
                    {
                        // BarrelStabilizer
                        if (ActionReady(BarrelStabilizer))
                            return BarrelStabilizer;

                        // Hypercharge
                        if ((Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && !IsComboExpiring(6) &&
                            LevelChecked(Hypercharge))
                        {
                            // Ensures Hypercharge is double weaved with WF
                            if ((LevelChecked(FullMetalField) && JustUsed(FullMetalField) &&
                                 (GetCooldownRemainingTime(Wildfire) < GCD || ActionReady(Wildfire))) ||
                                (!LevelChecked(FullMetalField) && ActionReady(Wildfire)) ||
                                !LevelChecked(Wildfire))
                                return Hypercharge;

                            // Only Hypercharge when tools are on cooldown
                            if (DrillCD && AnchorCD && SawCD &&
                                ((GetCooldownRemainingTime(Wildfire) > 40 && LevelChecked(Wildfire)) ||
                                 !LevelChecked(Wildfire)))
                                return Hypercharge;
                        }

                        //Queen
                        if (UseQueen(Gauge))
                            return OriginalHook(RookAutoturret);

                        // Reassemble
                        if (Reassembled(Gauge))
                            return Reassemble;

                        // Gauss Round and Ricochet outside HC
                        if (JustUsed(OriginalHook(AirAnchor), 2f) ||
                            JustUsed(Chainsaw, 2f) ||
                            JustUsed(Drill, 2f) ||
                            JustUsed(Excavator, 2f))
                        {
                            if (ActionReady(OriginalHook(GaussRound)) &&
                                !JustUsed(OriginalHook(GaussRound), 2f))
                                return OriginalHook(GaussRound);

                            if (ActionReady(OriginalHook(Ricochet)) &&
                                !JustUsed(OriginalHook(Ricochet), 2f))
                                return OriginalHook(Ricochet);
                        }

                        // Healing
                        if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                            return All.SecondWind;
                    }
                }

                // Gauss Round and Ricochet during HC
                if (JustUsed(OriginalHook(Heatblast), 1f) && HasNotWeaved)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >=
                        GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }
            }

            // Full Metal Field
            if (HasEffect(Buffs.FullMetalMachinist) &&
                (GetCooldownRemainingTime(Wildfire) <= GCD || ActionReady(Wildfire) ||
                 GetBuffRemainingTime(Buffs.FullMetalMachinist) <= 6) &&
                LevelChecked(FullMetalField))
                return FullMetalField;

            // Heatblast
            if (Gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                return OriginalHook(Heatblast);

            //Tools
            if (Tools(ref actionID))
                return actionID;

            // 1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                    return OriginalHook(SlugShot);

                if (ComboAction == OriginalHook(SlugShot) &&
                    !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble))
                    return Reassemble;

                if (ComboAction is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                    return OriginalHook(CleanShot);
            }
            return actionID;
        }
    }

    internal class MCH_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SplitShot or HeatedSplitShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave())
                return Variant.VariantRampart;

            // Opener
            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Opener) && TargetIsHostile())
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            //Reassemble to start before combat
            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) &&
                !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble) &&
                !InCombat() && TargetIsHostile())
                return Reassemble;

            // Interrupt
            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Interrupt) &&
                InterruptReady)
                return All.HeadGraze;

            // All weaves
            if (CanWeave())
            {
                if (!ActionWatching.HasDoubleWeaved())
                {
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_QueenOverdrive) &&
                        Gauge.IsRobotActive && GetTargetHPPercent() <= Config.MCH_ST_QueenOverDrive &&
                        ActionReady(OriginalHook(RookOverdrive)))
                        return OriginalHook(RookOverdrive);

                    // Wildfire
                    if (IsEnabled(CustomComboPreset.MCH_ST_Adv_WildFire) &&
                        JustUsed(Hypercharge) && ActionReady(Wildfire) &&
                        GetTargetHPPercent() >= Config.MCH_ST_WildfireHP)
                        return Wildfire;

                    if (!Gauge.IsOverheated)
                    {
                        // BarrelStabilizer
                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer) &&
                            ActionReady(BarrelStabilizer))
                            return BarrelStabilizer;

                        // Hypercharge
                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Hypercharge) &&
                            (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) &&
                            !IsComboExpiring(6) &&
                            LevelChecked(Hypercharge) &&
                            GetTargetHPPercent() >= Config.MCH_ST_HyperchargeHP)
                        {
                            // Ensures Hypercharge is double weaved with WF
                            if ((LevelChecked(FullMetalField) && JustUsed(FullMetalField) &&
                                 (GetCooldownRemainingTime(Wildfire) < GCD || ActionReady(Wildfire))) ||
                                (!LevelChecked(FullMetalField) && ActionReady(Wildfire)) ||
                                !LevelChecked(Wildfire))
                                return Hypercharge;

                            // Only Hypercharge when tools are on cooldown
                            if (DrillCD && AnchorCD && SawCD &&
                                ((GetCooldownRemainingTime(Wildfire) > 40 && LevelChecked(Wildfire)) ||
                                 !LevelChecked(Wildfire)))
                                return Hypercharge;
                        }

                        // Queen
                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_TurretQueen) &&
                            UseQueen(Gauge))
                            return OriginalHook(RookAutoturret);

                        // Reassemble
                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) &&
                            GetRemainingCharges(Reassemble) > Config.MCH_ST_ReassemblePool &&
                            Reassembled(Gauge))
                            return Reassemble;

                        // Gauss Round and Ricochet outside HC
                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                            (JustUsed(OriginalHook(AirAnchor), 2f) ||
                             JustUsed(Chainsaw, 2f) ||
                             JustUsed(Drill, 2f) ||
                             JustUsed(Excavator, 2f)))
                        {
                            if (ActionReady(OriginalHook(GaussRound)) &&
                                !JustUsed(OriginalHook(GaussRound), 2f))
                                return OriginalHook(GaussRound);

                            if (ActionReady(OriginalHook(Ricochet)) &&
                                !JustUsed(OriginalHook(Ricochet), 2f))
                                return OriginalHook(Ricochet);
                        }

                        // Healing
                        if (IsEnabled(CustomComboPreset.MCH_ST_Adv_SecondWind) &&
                            PlayerHealthPercentageHp() <= Config.MCH_ST_SecondWindThreshold &&
                            ActionReady(All.SecondWind))
                            return All.SecondWind;
                    }
                }

                // Gauss Round and Ricochet during HC
                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_GaussRicochet) &&
                    JustUsed(OriginalHook(Heatblast), 1f) && HasNotWeaved)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >=
                        GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) >
                        GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }
            }

            // Full Metal Field
            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Stabilizer_FullMetalField) &&
                HasEffect(Buffs.FullMetalMachinist) &&
                (GetCooldownRemainingTime(Wildfire) <= GCD || ActionReady(Wildfire) ||
                 GetBuffRemainingTime(Buffs.FullMetalMachinist) <= 6) &&
                LevelChecked(FullMetalField))
                return FullMetalField;

            // Heatblast
            if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Heatblast) &&
                Gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                return OriginalHook(Heatblast);

            //Tools
            if (Tools(ref actionID))
                return actionID;

            // 1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is SplitShot && LevelChecked(OriginalHook(SlugShot)))
                    return OriginalHook(SlugShot);

                if (IsEnabled(CustomComboPreset.MCH_ST_Adv_Reassemble) && Config.MCH_ST_Reassembled[4] &&
                    ComboAction == OriginalHook(SlugShot) &&
                    !LevelChecked(Drill) && !HasEffect(Buffs.Reassembled) && ActionReady(Reassemble))
                    return Reassemble;

                if (ComboAction is SlugShot && LevelChecked(OriginalHook(CleanShot)))
                    return OriginalHook(CleanShot);
            }
            return actionID;
        }
    }

    internal class MCH_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpreadShot or Scattergun))
                return actionID;

            if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                return Variant.VariantCure;

            if (HasEffect(Buffs.Flamethrower) || JustUsed(Flamethrower, 10f))
                return OriginalHook(11);

            if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave())
                return Variant.VariantRampart;

            // Interrupt
            if (InterruptReady)
                return All.HeadGraze;

            // All weaves
            if (CanWeave())
            {
                if (!ActionWatching.HasDoubleWeaved() && !Gauge.IsOverheated)
                {
                    // BarrelStabilizer
                    if (ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    if (Gauge.Battery == 100)
                        return OriginalHook(RookAutoturret);

                    // Hypercharge
                    if ((Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                        LevelChecked(AutoCrossbow) &&
                        ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) ||
                         !BioBlaster.LevelChecked()) &&
                        ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) ||
                         !Flamethrower.LevelChecked()))
                        return Hypercharge;

                    if (!HasEffect(Buffs.Wildfire) &&
                        !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) &&
                        !JustUsed(Flamethrower, 10f) &&
                        ((HasEffect(Buffs.ExcavatorReady) && Excavator.LevelChecked()) ||
                         (GetCooldownRemainingTime(Chainsaw) < 1 && Chainsaw.LevelChecked()) ||
                         (GetCooldownRemainingTime(AirAnchor) < 1 && AirAnchor.LevelChecked()) ||
                         Scattergun.LevelChecked()))
                        return Reassemble;

                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                        return All.SecondWind;
                }

                //AutoCrossbow, Gauss, Rico
                if ((JustUsed(OriginalHook(AutoCrossbow), 1f) ||
                     JustUsed(OriginalHook(Heatblast), 1f)) && HasNotWeaved)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >=
                        GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) >
                        GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }
            }

            //Full Metal Field
            if (HasEffect(Buffs.FullMetalMachinist) && LevelChecked(FullMetalField))
                return FullMetalField;

            if (ActionReady(BioBlaster) && !TargetHasEffect(Debuffs.Bioblaster) && !Gauge.IsOverheated)
                return OriginalHook(BioBlaster);

            if (ActionReady(Flamethrower) && !IsMoving())
                return OriginalHook(Flamethrower);

            if (LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                return OriginalHook(Chainsaw);

            if (LevelChecked(Chainsaw) &&
                (GetCooldownRemainingTime(Chainsaw) <= GetCooldownRemainingTime(OriginalHook(Scattergun)) + 0.25 ||
                 ActionReady(Chainsaw)))
                return Chainsaw;

            if (LevelChecked(AirAnchor) &&
                (GetCooldownRemainingTime(AirAnchor) <= GetCooldownRemainingTime(OriginalHook(Scattergun)) + 0.25 ||
                 ActionReady(AirAnchor)))
                return AirAnchor;

            if (LevelChecked(AutoCrossbow) && Gauge.IsOverheated && !LevelChecked(CheckMate))
                return AutoCrossbow;

            if (Gauge.IsOverheated && LevelChecked(CheckMate))
                return BlazingShot;

            return actionID;
        }
    }

    internal class MCH_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpreadShot or Scattergun))
                return actionID;

            bool reassembledScattergunAoE = IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) &&
                                            Config.MCH_AoE_Reassembled[0] && HasEffect(Buffs.Reassembled);

            bool reassembledChainsawAoE =
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[2] && HasEffect(Buffs.Reassembled)) ||
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[2] && !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble);

            bool reassembledExcavatorAoE =
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[3] && HasEffect(Buffs.Reassembled)) ||
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[3] && !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble);

            bool reassembledAirAnchorAoE =
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && Config.MCH_AoE_Reassembled[1] && HasEffect(Buffs.Reassembled)) ||
                (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !Config.MCH_AoE_Reassembled[1] && !HasEffect(Buffs.Reassembled)) ||
                (!HasEffect(Buffs.Reassembled) && GetRemainingCharges(Reassemble) <= Config.MCH_AoE_ReassemblePool) ||
                !IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble);

            if (IsEnabled(CustomComboPreset.MCH_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MCH_VariantCure)
                return Variant.VariantCure;

            if (HasEffect(Buffs.Flamethrower) || JustUsed(Flamethrower, 10f))
                return OriginalHook(11);

            if (IsEnabled(CustomComboPreset.MCH_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave())
                return Variant.VariantRampart;

            // Interrupt
            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Interrupt) && InterruptReady)
                return All.HeadGraze;

            // All weaves
            if (CanWeave())
            {
                if (!ActionWatching.HasDoubleWeaved() && !Gauge.IsOverheated)
                {
                    // BarrelStabilizer
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Stabilizer) &&
                        ActionReady(BarrelStabilizer))
                        return BarrelStabilizer;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Queen) &&
                        Gauge.Battery >= Config.MCH_AoE_TurretUsage)
                        return OriginalHook(RookAutoturret);

                    // Hypercharge
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Hypercharge) &&
                        (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)) && LevelChecked(Hypercharge) &&
                        LevelChecked(AutoCrossbow) &&
                        ((BioBlaster.LevelChecked() && GetCooldownRemainingTime(BioBlaster) > 10) ||
                         !BioBlaster.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster)) &&
                        ((Flamethrower.LevelChecked() && GetCooldownRemainingTime(Flamethrower) > 10) ||
                         !Flamethrower.LevelChecked() || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower)))
                        return Hypercharge;

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Reassemble) && !HasEffect(Buffs.Wildfire) &&
                        !HasEffect(Buffs.Reassembled) && HasCharges(Reassemble) && !JustUsed(Flamethrower, 10f) &&
                        GetRemainingCharges(Reassemble) > Config.MCH_AoE_ReassemblePool &&
                        ((Config.MCH_AoE_Reassembled[0] && Scattergun.LevelChecked()) ||
                         (Gauge.IsOverheated && Config.MCH_AoE_Reassembled[1] && AutoCrossbow.LevelChecked()) ||
                         (GetCooldownRemainingTime(Chainsaw) < 1 && Config.MCH_AoE_Reassembled[2] && Chainsaw.LevelChecked()) ||
                         (GetCooldownRemainingTime(OriginalHook(Chainsaw)) < 1 && Config.MCH_AoE_Reassembled[3] &&
                          Excavator.LevelChecked())))
                        return Reassemble;

                    //gauss and ricochet outside HC
                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) &&
                        Config.MCH_AoE_Hypercharge)
                    {
                        if (ActionReady(OriginalHook(GaussRound)) &&
                            !JustUsed(OriginalHook(GaussRound), 2.5f))
                            return OriginalHook(GaussRound);

                        if (ActionReady(OriginalHook(Ricochet)) &&
                            !JustUsed(OriginalHook(Ricochet), 2.5f))
                            return OriginalHook(Ricochet);
                    }

                    if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_SecondWind) &&
                        PlayerHealthPercentageHp() <= Config.MCH_AoE_SecondWindThreshold &&
                        ActionReady(All.SecondWind))
                        return All.SecondWind;
                }

                //AutoCrossbow, Gauss, Rico
                if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_GaussRicochet) &&
                    !Config.MCH_AoE_Hypercharge &&
                    (JustUsed(OriginalHook(AutoCrossbow), 1f) ||
                     JustUsed(OriginalHook(Heatblast), 1f)) && HasNotWeaved)
                {
                    if (ActionReady(OriginalHook(GaussRound)) &&
                        GetRemainingCharges(OriginalHook(GaussRound)) >=
                        GetRemainingCharges(OriginalHook(Ricochet)))
                        return OriginalHook(GaussRound);

                    if (ActionReady(OriginalHook(Ricochet)) &&
                        GetRemainingCharges(OriginalHook(Ricochet)) >
                        GetRemainingCharges(OriginalHook(GaussRound)))
                        return OriginalHook(Ricochet);
                }
            }

            //Full Metal Field
            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Stabilizer_FullMetalField) &&
                HasEffect(Buffs.FullMetalMachinist) && LevelChecked(FullMetalField))
                return FullMetalField;

            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Bioblaster) &&
                ActionReady(BioBlaster) && !TargetHasEffect(Debuffs.Bioblaster) && !Gauge.IsOverheated)
                return OriginalHook(BioBlaster);

            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_FlameThrower) &&
                ActionReady(Flamethrower) && !IsMoving())
                return OriginalHook(Flamethrower);

            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Excavator) &&
                reassembledExcavatorAoE &&
                LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                return OriginalHook(Chainsaw);

            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_Chainsaw) &&
                reassembledChainsawAoE &&
                LevelChecked(Chainsaw) &&
                (GetCooldownRemainingTime(Chainsaw) <= GCD + 0.25 || ActionReady(Chainsaw)))
                return Chainsaw;

            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_AirAnchor) &&
                reassembledAirAnchorAoE &&
                LevelChecked(AirAnchor) &&
                (GetCooldownRemainingTime(AirAnchor) <= GCD + 0.25 || ActionReady(AirAnchor)))
                return AirAnchor;

            if (reassembledScattergunAoE)
                return OriginalHook(Scattergun);

            if (LevelChecked(AutoCrossbow) && Gauge.IsOverheated &&
                (!LevelChecked(CheckMate) || IsNotEnabled(CustomComboPreset.MCH_AoE_Adv_BlazingShot)))
                return AutoCrossbow;

            if (IsEnabled(CustomComboPreset.MCH_AoE_Adv_BlazingShot) &&
                Gauge.IsOverheated && LevelChecked(CheckMate))
                return BlazingShot;

            return actionID;
        }
    }

    internal class MCH_HeatblastGaussRicochet : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Heatblast;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Heatblast or BlazingShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.MCH_Heatblast_AutoBarrel) &&
                ActionReady(BarrelStabilizer) && !Gauge.IsOverheated)
                return BarrelStabilizer;

            if (IsEnabled(CustomComboPreset.MCH_Heatblast_Wildfire) &&
                ActionReady(Wildfire) && JustUsed(Hypercharge))
                return Wildfire;

            if (!Gauge.IsOverheated && LevelChecked(Hypercharge) &&
                (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)))
                return Hypercharge;

            if (IsEnabled(CustomComboPreset.MCH_Heatblast_GaussRound) &&
                CanWeave() &&
                JustUsed(OriginalHook(Heatblast), 1f) &&
                HasNotWeaved)
            {
                if (ActionReady(OriginalHook(GaussRound)) &&
                    GetRemainingCharges(OriginalHook(GaussRound)) >=
                    GetRemainingCharges(OriginalHook(Ricochet)))
                    return OriginalHook(GaussRound);

                if (ActionReady(OriginalHook(Ricochet)) &&
                    GetRemainingCharges(OriginalHook(Ricochet)) >
                    GetRemainingCharges(OriginalHook(GaussRound)))
                    return OriginalHook(Ricochet);
            }

            if (Gauge.IsOverheated && LevelChecked(OriginalHook(Heatblast)))
                return OriginalHook(Heatblast);

            return actionID;
        }
    }

    internal class MCH_AutoCrossbowGaussRicochet : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_AutoCrossbow;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AutoCrossbow)
                return actionID;

            if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_AutoBarrel) &&
                ActionReady(BarrelStabilizer) && !Gauge.IsOverheated)
                return BarrelStabilizer;

            if (!Gauge.IsOverheated && LevelChecked(Hypercharge) &&
                (Gauge.Heat >= 50 || HasEffect(Buffs.Hypercharged)))
                return Hypercharge;

            if (IsEnabled(CustomComboPreset.MCH_AutoCrossbow_GaussRound) &&
                CanWeave() &&
                JustUsed(OriginalHook(AutoCrossbow), 1f) &&
                HasNotWeaved)
            {
                if (ActionReady(OriginalHook(GaussRound)) &&
                    GetRemainingCharges(OriginalHook(GaussRound)) >=
                    GetRemainingCharges(OriginalHook(Ricochet)))
                    return OriginalHook(GaussRound);

                if (ActionReady(OriginalHook(Ricochet)) &&
                    GetRemainingCharges(OriginalHook(Ricochet)) >
                    GetRemainingCharges(OriginalHook(GaussRound)))
                    return OriginalHook(Ricochet);
            }

            if (Gauge.IsOverheated && LevelChecked(OriginalHook(AutoCrossbow)))
                return OriginalHook(AutoCrossbow);

            return actionID;
        }
    }

    internal class MCH_GaussRoundRicochet : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_GaussRoundRicochet;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (GaussRound or Ricochet or CheckMate or DoubleCheck))
                return actionID;

            if (ActionReady(OriginalHook(GaussRound)) &&
                GetRemainingCharges(OriginalHook(GaussRound)) >=
                GetRemainingCharges(OriginalHook(Ricochet)))
                return OriginalHook(GaussRound);

            if (ActionReady(OriginalHook(Ricochet)) &&
                GetRemainingCharges(OriginalHook(Ricochet)) >
                GetRemainingCharges(OriginalHook(GaussRound)))
                return OriginalHook(Ricochet);

            return actionID;
        }
    }

    internal class MCH_Overdrive : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_Overdrive;

        protected override uint Invoke(uint actionID) =>
            actionID is RookAutoturret or AutomatonQueen && Gauge.IsRobotActive
                ? OriginalHook(QueenOverdrive)
                : actionID;
    }

    internal class MCH_HotShotDrillChainsawExcavator : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_HotShotDrillChainsawExcavator;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Drill or HotShot or AirAnchor or Chainsaw))
                return actionID;

            if (LevelChecked(Excavator) && HasEffect(Buffs.ExcavatorReady))
                return CalcBestAction(actionID, Excavator, Chainsaw, AirAnchor, Drill);

            if (LevelChecked(Chainsaw))
                return CalcBestAction(actionID, Chainsaw, AirAnchor, Drill);

            if (LevelChecked(AirAnchor))
                return CalcBestAction(actionID, AirAnchor, Drill);

            if (LevelChecked(Drill))
                return CalcBestAction(actionID, Drill, HotShot);

            if (!LevelChecked(Drill))
                return HotShot;

            return actionID;
        }
    }

    internal class MCH_DismantleTactician : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCH_DismantleTactician;

        protected override uint Invoke(uint actionID) =>
            actionID is Dismantle &&
            (IsOnCooldown(Dismantle) || !LevelChecked(Dismantle)) &&
            ActionReady(Tactician) && !HasEffect(Buffs.Tactician)
                ? Tactician
                : actionID;
    }

    internal class All_PRanged_Dismantle : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.All_PRanged_Dismantle;

        protected override uint Invoke(uint actionID) =>
            actionID is Dismantle && TargetHasEffectAny(Debuffs.Dismantled) && IsOffCooldown(Dismantle)
                ? OriginalHook(11)
                : actionID;
    }
}
