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

            //Reassemble to start before combat/after downtime
            if (CanReassemble(false) && !IsOverheated && !HasWeaved())
                return Reassemble;

            if (!IsOverheated &&
                ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (OvercapGaussRicochetProtection(out uint gaussRico))
                    return gaussRico;

                if (RobotActive && ActionReady(OriginalHook(RookOverdrive)) &&
                    GetTargetHPPercent() <= 1)
                    return OriginalHook(RookOverdrive);

                if (CanWildfireWeave(simpleMode: true))
                    return Wildfire;

                if (CanHypercharge(false))
                    return Hypercharge;

                if (GaussRicochetWeaves(out gaussRico, false, true))
                    return gaussRico;

                if (!IsOverheated)
                {
                    if (CanReassemble(false))
                        return Reassemble;

                    if (CanBarrelStabilizer(false, true))
                        return BarrelStabilizer;

                    if (CanQueen())
                        return OriginalHook(RookAutoturret);

                    if (GaussRicochetWeaves(out gaussRico, false, false))
                        return gaussRico;

                    if (ActionReady(Dismantle) &&
                        !HasStatusEffect(Debuffs.Dismantled, CurrentTarget, true) &&
                        CanApplyStatus(CurrentTarget, Debuffs.Dismantled) &&
                        !JustUsed(Tactician, 6) && GroupDamageIncoming())
                        return Dismantle;

                    // Healing
                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    // Interrupt
                    if (Role.CanHeadGraze(true))
                        return Role.HeadGraze;
                }
            }

            // Full Metal Field
            if (CanUseFullMetalField)
                return FullMetalField;

            //Tools
            if (CanUseTools(ref actionID, false) && !IsOverheated)
                return actionID;

            // Heatblast
            if (IsOverheated && ActionReady(OriginalHook(Heatblast)))
                return OverheatGCD(onAoE: false);

            return DoBasicCombo(actionID, true);
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
                if (OvercapGaussRicochetProtection(out uint gaussRico))
                    return gaussRico;

                if (CanHypercharge(true))
                    return Hypercharge;

                if (GaussRicochetWeaves(out gaussRico, true, true))
                    return gaussRico;

                if (!IsOverheated)
                {
                    if (CanReassemble(true))
                        return Reassemble;

                    if (CanBarrelStabilizer(true, true))
                        return BarrelStabilizer;

                    if (CanQueen(true, true))
                        return OriginalHook(RookAutoturret);

                    if (GaussRicochetWeaves(out gaussRico, true, false))
                        return gaussRico;

                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    // Interrupt
                    if (Role.CanHeadGraze(true))
                        return Role.HeadGraze;
                }
            }

            if (!IsOverheated)
            {
                if (CanUseFullMetalField)
                    return FullMetalField;

                if (ActionReady(Flamethrower) &&
                    !HasStatusEffect(Buffs.Reassembled) &&
                    !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(3))
                    return Flamethrower;

                if (CanUseTools(ref actionID, true))
                    return actionID;
            }

            if (ActionReady(OriginalHook(Heatblast)) && IsOverheated)
                return OverheatGCD(onAoE: true);

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
                (MCH_HaveTarget == 1 || HasBattleTarget()) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            //Reassemble to start before combat/after downtime
            if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) &&
                CanReassemble(false, MCH_ST_Adv_ReassembleChoice, MCH_ST_ReassemblePool, ReassembleHPThresholdST) &&
                !IsOverheated && !HasWeaved())
                return Reassemble;

            if (!IsOverheated &&
                ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // All weaves
            if (CanWeave())
            {
                if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                    MCH_ST_GaussOnlyOrBoth == 0 &&
                    OvercapGaussRicochetProtection(out uint gaussRico))
                    return gaussRico;

                if (IsEnabled(Preset.MCH_ST_Adv_QueenOverdrive) &&
                    RobotActive && ActionReady(OriginalHook(RookOverdrive)) &&
                    GetTargetHPPercent() <= MCH_ST_QueenOverDriveHPThreshold)
                    return OriginalHook(RookOverdrive);

                if (IsEnabled(Preset.MCH_ST_Adv_WildFire) && CanWildfireWeave())
                    return Wildfire;

                if (IsEnabled(Preset.MCH_ST_Adv_Hypercharge) &&
                    CanHypercharge(false, hpThreshold: HyperchargeHPThresholdST))
                    return Hypercharge;

                if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                    GaussRicochetWeaves(out gaussRico, false, true,
                        gaussOnlyOrBoth: MCH_ST_GaussOnlyOrBoth,
                        chargePool: MCH_ST_GaussRicoManualUse))
                    return gaussRico;

                if (!IsOverheated)
                {
                    if (IsEnabled(Preset.MCH_ST_Adv_Reassemble) &&
                        CanReassemble(false, MCH_ST_Adv_ReassembleChoice, MCH_ST_ReassemblePool, ReassembleHPThresholdST))
                        return Reassemble;

                    if (IsEnabled(Preset.MCH_ST_Adv_Stabilizer) &&
                        CanBarrelStabilizer(onAoE: false))
                        return BarrelStabilizer;

                    if (IsEnabled(Preset.MCH_ST_Adv_TurretQueen) &&
                        CanQueen())
                        return OriginalHook(RookAutoturret);

                    if (IsEnabled(Preset.MCH_ST_Adv_GaussRicochet) &&
                        GaussRicochetWeaves(out gaussRico, false, false,
                            gaussOnlyOrBoth: MCH_ST_GaussOnlyOrBoth,
                            chargePool: MCH_ST_GaussRicoManualUse))
                        return gaussRico;

                    if (IsEnabled(Preset.MCH_ST_Dismantle) &&
                        ActionReady(Dismantle) && GroupDamageIncoming() &&
                        !JustUsed(Tactician, 6) && CanApplyStatus(CurrentTarget, Debuffs.Dismantled) &&
                        GetStatusEffectRemainingTime(Debuffs.Dismantled, CurrentTarget, true) > MCH_DismantledDuration)
                        return Dismantle;

                    if (IsEnabled(Preset.MCH_ST_Adv_Tactician) &&
                        ActionReady(Tactician) && GroupDamageIncoming() &&
                        !JustUsed(Dismantle, 6) && NumberOfAlliesInRange(Tactician) >= GetPartyMembers().Count * .75 &&
                        !HasAnyStatusEffects([BRD.Buffs.Troubadour, DNC.Buffs.ShieldSamba, Buffs.Tactician], anyOwner: true))
                        return Tactician;

                    // Healing
                    if (IsEnabled(Preset.MCH_ST_Adv_SecondWind) &&
                        Role.CanSecondWind(MCH_ST_SecondWindHPThreshold))
                        return Role.SecondWind;

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_ST_Adv_Interrupt))
                        return Role.HeadGraze;
                }
                else
                {
                    // Queen in hypercharge
                    if (IsEnabled(Preset.MCH_ST_Adv_TurretQueen) &&
                        IsEnabled(Preset.MCH_ST_Adv_QueenInHypercharge) &&
                        CanQueen())
                        return OriginalHook(RookAutoturret);
                }
            }

            // Full Metal Field
            if (IsEnabled(Preset.MCH_ST_Adv_Stabilizer_FullMetalField) &&
                CanUseFullMetalField)
                return FullMetalField;

            //Tools
            if (IsEnabled(Preset.MCH_ST_Adv_Tools) &&
                GetTargetHPPercent() > HPThresholdTools &&
                CanUseTools(ref actionID, false) && !IsOverheated)
                return actionID;

            // Heatblast
            if (IsEnabled(Preset.MCH_ST_Adv_Heatblast) &&
                ActionReady(OriginalHook(Heatblast)) && IsOverheated)
                return OverheatGCD(onAoE: false);

            return DoBasicCombo(actionID, IsEnabled(Preset.MCH_ST_Adv_Reassemble),
                MCH_ST_Adv_ReassembleChoice, MCH_ST_ReassemblePool, ReassembleHPThresholdST);
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
                if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet) &&
                    OvercapGaussRicochetProtection(out uint gaussRico))
                    return gaussRico;

                if (IsEnabled(Preset.MCH_AoE_Adv_QueenOverdrive) &&
                    RobotActive && ActionReady(OriginalHook(RookOverdrive)) &&
                    GetTargetHPPercent() <= MCH_AoE_QueenOverDriveHPThreshold)
                    return OriginalHook(RookOverdrive);

                if (IsEnabled(Preset.MCH_AoE_Adv_Hypercharge) &&
                    CanHypercharge(true, MCH_AoE_AirAnchor, MCH_AoE_HyperchargeToolHold, MCH_AoE_HyperchargeHPThreshold))
                    return Hypercharge;

                if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet) &&
                    GaussRicochetWeaves(out gaussRico, true, true))
                    return gaussRico;

                if (!IsOverheated)
                {
                    if (IsEnabled(Preset.MCH_AoE_Adv_Reassemble) &&
                        CanReassemble(true, chargePool: MCH_AoE_ReassemblePool, hpThreshold: MCH_AoE_ReassembleHPThreshold))
                        return Reassemble;

                    if (IsEnabled(Preset.MCH_AoE_Adv_Stabilizer) &&
                        CanBarrelStabilizer(onAoE: true))
                        return BarrelStabilizer;

                    if (IsEnabled(Preset.MCH_AoE_Adv_Queen) &&
                        CanQueen(onAoE: true))
                        return OriginalHook(RookAutoturret);

                    if (IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet) &&
                        GaussRicochetWeaves(out gaussRico, true, false))
                        return gaussRico;

                    if (IsEnabled(Preset.MCH_AoE_Adv_SecondWind) &&
                        Role.CanSecondWind(MCH_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;

                    // Interrupt
                    if (Role.CanHeadGraze(Preset.MCH_AoE_Adv_Interrupt))
                        return Role.HeadGraze;
                }
            }

            if (!IsOverheated)
            {
                //Full Metal Field
                if (IsEnabled(Preset.MCH_AoE_Adv_Stabilizer_FullMetalField) &&
                    CanUseFullMetalField)
                    return FullMetalField;

                if (IsEnabled(Preset.MCH_AoE_Adv_FlameThrower) &&
                    ActionReady(Flamethrower) &&
                    !HasStatusEffect(Buffs.Reassembled) &&
                    (MCH_AoE_FlamethrowerMovement == 1 ||
                     MCH_AoE_FlamethrowerMovement == 0 && !IsMoving() &&
                     TimeStoodStill > TimeSpan.FromSeconds(MCH_AoE_FlamethrowerTimeStill)) &&
                    GetTargetHPPercent() > MCH_AoE_FlamethrowerHPOption)
                    return Flamethrower;

                if (IsEnabled(Preset.MCH_AoE_Adv_Tools) &&
                    GetTargetHPPercent() > MCH_AoE_ToolsHPThreshold &&
                    CanUseTools(ref actionID, true, MCH_AoE_AirAnchor))
                    return actionID;
            }

            if (ActionReady(OriginalHook(Heatblast)) && IsOverheated)
                return OverheatGCD(onAoE: true, gaussRicoEnabled: IsEnabled(Preset.MCH_AoE_Adv_GaussRicochet));

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

            return DoBasicCombo(OriginalHook(SplitShot));
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

            if (!IsOverheated &&
                (ActionReady(Hypercharge) || HasStatusEffect(Buffs.Hypercharged)))
                return Hypercharge;

            if (IsEnabled(Preset.MCH_Heatblast_GaussRound) &&
                CanWeave() &&
                GaussRicochetWeaves(out uint gaussRico, false, true))
                return gaussRico;

            if (IsOverheated && ActionReady(OriginalHook(Heatblast)))
                return OverheatGCD(onAoE: false);

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

            if (!IsOverheated &&
                (ActionReady(Hypercharge) || HasStatusEffect(Buffs.Hypercharged)))
                return Hypercharge;

            if (IsEnabled(Preset.MCH_AutoCrossbow_GaussRound) &&
                CanWeave() &&
                GaussRicochetWeaves(out uint gaussRico, true, true))
                return gaussRico;

            if (IsOverheated && ActionReady(AutoCrossbow))
                return OverheatGCD(onAoE: true, alwaysAutoCrossbow: true);

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
                GaussRound or DoubleCheck when MCH_GaussRico == 0 && ActionReady(OriginalHook(GaussRound)) && (CanGaussRound || !LevelChecked(Ricochet)) => OriginalHook(GaussRound),
                GaussRound or DoubleCheck when MCH_GaussRico == 0 && ActionReady(OriginalHook(Ricochet)) && CanRicochet => OriginalHook(Ricochet),
                Ricochet or CheckMate when MCH_GaussRico == 1 && ActionReady(OriginalHook(GaussRound)) && (CanGaussRound || !LevelChecked(Ricochet)) => OriginalHook(GaussRound),
                Ricochet or CheckMate when MCH_GaussRico == 1 && ActionReady(OriginalHook(Ricochet)) && CanRicochet => OriginalHook(Ricochet),
                var _ => actionID
            };
        }
    }
}
