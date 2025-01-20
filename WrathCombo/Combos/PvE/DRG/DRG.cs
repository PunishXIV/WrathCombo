using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal class DRG_ST_FullThrustCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_FullThrustCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FullThrust or HeavensThrust))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return FangAndClaw;

                if (ComboAction is FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return OriginalHook(TrueThrust);
        }
    }

    internal class DRG_ST_ChaoticCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_ChaoticCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ChaosThrust or ChaoticSpring))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                    return OriginalHook(Disembowel);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return WheelingThrust;

                if (ComboAction is WheelingThrust && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return OriginalHook(TrueThrust);
        }
    }

    internal class DRG_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not TrueThrust)
                return actionID;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                 CanDRGWeave(Variant.VariantRampart))
                return Variant.VariantRampart;

            // Piercing Talon Uptime Option
            if (LevelChecked(PiercingTalon) &&
                !InMeleeRange() &&
                HasBattleTarget())
                return PiercingTalon;

            if (HasEffect(Buffs.PowerSurge))
            {
                //Lance Charge Feature
                if (ActionReady(LanceCharge) &&
                     CanDRGWeave(LanceCharge))
                    return LanceCharge;

                //Battle Litany Feature
                if (ActionReady(BattleLitany) &&
                     CanDRGWeave(BattleLitany))
                    return BattleLitany;

                //Life Surge Feature
                if (ActionReady(LifeSurge) &&
                    (GetCooldownRemainingTime(LifeSurge) < 40 || GetCooldownRemainingTime(BattleLitany) > 50) &&
                     CanDRGWeave(LifeSurge) &&
                    ((HasEffect(Buffs.LanceCharge) &&
                      !HasEffect(Buffs.LifeSurge) &&
                      ((JustUsed(WheelingThrust) && LevelChecked(Drakesbane)) ||
                       (JustUsed(FangAndClaw) && LevelChecked(Drakesbane)) ||
                       (JustUsed(OriginalHook(VorpalThrust)) && LevelChecked(FullThrust)))) ||
                     (!LevelChecked(LanceCharge) && JustUsed(VorpalThrust))))
                    return LifeSurge;

                //Geirskogul Feature
                if (ActionReady(Geirskogul) &&
                     CanDRGWeave(Geirskogul))
                    return Geirskogul;

                //Dragonfire Dive Feature
                if (ActionReady(DragonfireDive) &&
                     CanDRGWeave(DragonfireDive) &&
                    TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return DragonfireDive;

                //(High) Jump Feature   
                if (ActionReady(OriginalHook(Jump)) &&
                     CanDRGWeave(OriginalHook(Jump)) &&
                    TimeMoving.Ticks == 0)
                    return OriginalHook(Jump);

                //Wyrmwind Thrust Feature
                if (LevelChecked(WyrmwindThrust) &&
                     CanDRGWeave(WyrmwindThrust) &&
                    Gauge.FirstmindsFocusCount is 2)
                    return WyrmwindThrust;

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                     CanDRGWeave(Stardiver) &&
                    Gauge.IsLOTDActive && TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)

                    return Stardiver;

                //Starcross Feature
                if (LevelChecked(Starcross) &&
                     CanDRGWeave(Starcross) &&
                    HasEffect(Buffs.StarcrossReady))
                    return Starcross;

                //Rise of the Dragon Feature
                if (LevelChecked(RiseOfTheDragon) &&
                     CanDRGWeave(RiseOfTheDragon) &&
                    HasEffect(Buffs.DragonsFlight))
                    return RiseOfTheDragon;

                //Nastrond Feature
                if (LevelChecked(Nastrond) &&
                     CanDRGWeave(Nastrond) &&
                    HasEffect(Buffs.NastrondReady) &&
                    Gauge.IsLOTDActive)
                    return Nastrond;

                //Mirage Feature
                if (LevelChecked(MirageDive) &&
                     CanDRGWeave(MirageDive) &&
                    HasEffect(Buffs.DiveReady))
                    return MirageDive;
            }

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           ((ChaosDoTDebuff is null && LevelChecked(ChaosThrust)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                {
                    if (trueNorthReady && CanDRGWeave(All.TrueNorth) &&
                        !OnTargetsRear())
                        return All.TrueNorth;

                    return OriginalHook(ChaosThrust);
                }

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                {
                    if (trueNorthReady && CanDRGWeave(All.TrueNorth) &&
                        !OnTargetsRear())
                        return All.TrueNorth;

                    return WheelingThrust;
                }

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                {
                    if (trueNorthReady && CanDRGWeave(All.TrueNorth) &&
                        !OnTargetsFlank())
                        return All.TrueNorth;

                    return FangAndClaw;
                }

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return actionID;
        }
    }

    internal class DRG_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not TrueThrust)
                return actionID;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                 CanDRGWeave(Variant.VariantRampart))
                return Variant.VariantRampart;

            // Opener for DRG
            if (IsEnabled(CustomComboPreset.DRG_ST_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            // Piercing Talon Uptime Option
            if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) &&
                LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasEffect(Buffs.PowerSurge))
            {
                if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                {
                    //Lance Charge Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                        ActionReady(LanceCharge) &&
                         CanDRGWeave(LanceCharge) &&
                        GetTargetHPPercent() >= Config.DRG_ST_LanceChargeHP)
                        return LanceCharge;

                    //Battle Litany Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                        ActionReady(BattleLitany) &&
                         CanDRGWeave(BattleLitany) &&
                        GetTargetHPPercent() >= Config.DRG_ST_LitanyHP)
                        return BattleLitany;
                }

                if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                {
                    //Life Surge Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) &&
                        ActionReady(LifeSurge) &&
                        (GetCooldownRemainingTime(LifeSurge) < 40 || GetCooldownRemainingTime(BattleLitany) > 50) &&
                         CanDRGWeave(LifeSurge) &&
                        ((HasEffect(Buffs.LanceCharge) &&
                          !HasEffect(Buffs.LifeSurge) &&
                          ((JustUsed(WheelingThrust) && LevelChecked(Drakesbane)) ||
                           (JustUsed(FangAndClaw) && LevelChecked(Drakesbane)) ||
                           (JustUsed(OriginalHook(VorpalThrust)) && LevelChecked(FullThrust)))) ||
                         (!LevelChecked(LanceCharge) && JustUsed(VorpalThrust))))
                        return LifeSurge;

                    //Geirskogul Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Geirskogul) &&
                        ActionReady(Geirskogul) &&
                         CanDRGWeave(Geirskogul))
                        return Geirskogul;

                    //Dragonfire Dive Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive) &&
                        ActionReady(DragonfireDive) &&
                         CanDRGWeave(DragonfireDive) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Melee) ||
                         (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Melee) && TimeMoving.Ticks == 0 &&
                          GetTargetDistance() <= 1)))
                        return DragonfireDive;

                    //(High) Jump Feature   
                    if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                        ActionReady(OriginalHook(Jump)) &&
                         CanDRGWeave(OriginalHook(Jump)) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_HighJump_Melee) ||
                         (IsEnabled(CustomComboPreset.DRG_ST_HighJump_Melee) && TimeMoving.Ticks == 0 &&
                          GetTargetDistance() <= 1)))
                        return OriginalHook(Jump);

                    //Wyrmwind Thrust Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                        LevelChecked(WyrmwindThrust) &&
                         CanDRGWeave(WyrmwindThrust) &&
                        Gauge.FirstmindsFocusCount is 2)
                        return WyrmwindThrust;

                    //StarDiver Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) &&
                        ActionReady(Stardiver) &&
                         CanDRGWeave(Stardiver) &&
                        Gauge.IsLOTDActive &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_Stardiver_Melee) ||
                         (IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Melee) && TimeMoving.Ticks == 0 &&
                          GetTargetDistance() <= 1)))
                        return Stardiver;

                    //Starcross Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Starcross) &&
                        LevelChecked(Starcross) &&
                         CanDRGWeave(Starcross) &&
                        HasEffect(Buffs.StarcrossReady))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Dives_RiseOfTheDragon) &&
                         CanDRGWeave(RiseOfTheDragon) &&
                        HasEffect(Buffs.DragonsFlight))
                        return RiseOfTheDragon;

                    //Nastrond Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Nastrond) &&
                        LevelChecked(Nastrond) &&
                         CanDRGWeave(Nastrond) &&
                        HasEffect(Buffs.NastrondReady) &&
                        Gauge.IsLOTDActive)
                        return Nastrond;

                    //Mirage Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) &&
                        LevelChecked(MirageDive) &&
                         CanDRGWeave(MirageDive) &&
                        HasEffect(Buffs.DiveReady))
                        return MirageDive;
                }
            }

            // healing
            if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
            {
                if (PlayerHealthPercentageHp() <= Config.DRG_ST_SecondWind_Threshold && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (PlayerHealthPercentageHp() <= Config.DRG_ST_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                    return All.Bloodbath;
            }

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           ((ChaosDoTDebuff is null && LevelChecked(ChaosThrust)) ||
                            GetBuffRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                {
                    if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                        trueNorthReady && CanDRGWeave(All.TrueNorth) &&
                        !OnTargetsRear())
                        return All.TrueNorth;

                    return OriginalHook(ChaosThrust);
                }

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                {
                    if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                        trueNorthReady && CanDRGWeave(All.TrueNorth) &&
                        !OnTargetsRear())
                        return All.TrueNorth;

                    return WheelingThrust;
                }

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                {
                    if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                        trueNorthReady && CanDRGWeave(All.TrueNorth) &&
                        !OnTargetsFlank())
                        return All.TrueNorth;

                    return FangAndClaw;
                }

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return actionID;
        }
    }

    internal class DRG_AOE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AOE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not DoomSpike)
                return actionID;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                 CanDRGWeave(Variant.VariantRampart))
                return Variant.VariantRampart;

            // Piercing Talon Uptime Option
            if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasEffect(Buffs.PowerSurge))
            {
                //Lance Charge Feature
                if (ActionReady(LanceCharge) &&
                     CanDRGWeave(LanceCharge))
                    return LanceCharge;

                //Battle Litany Feature
                if (ActionReady(BattleLitany) &&
                     CanDRGWeave(BattleLitany))
                    return BattleLitany;

                //Life Surge Feature
                if (ActionReady(LifeSurge) &&
                     CanDRGWeave(LifeSurge) &&
                    !HasEffect(Buffs.LifeSurge) &&
                    ((JustUsed(SonicThrust) && LevelChecked(CoerthanTorment)) ||
                     (JustUsed(DoomSpike) && LevelChecked(SonicThrust)) ||
                     (JustUsed(DoomSpike) && !LevelChecked(SonicThrust))))
                    return LifeSurge;

                //Wyrmwind Thrust Feature
                if (LevelChecked(WyrmwindThrust) &&
                     CanDRGWeave(WyrmwindThrust) &&
                    Gauge.FirstmindsFocusCount is 2)
                    return WyrmwindThrust;

                //Geirskogul Feature
                if (ActionReady(Geirskogul) &&
                     CanDRGWeave(Geirskogul))
                    return Geirskogul;

                //(High) Jump Feature   
                if (ActionReady(OriginalHook(Jump)) &&
                     CanDRGWeave(OriginalHook(Jump)) &&
                    TimeMoving.Ticks == 0)
                    return OriginalHook(Jump);

                //Dragonfire Dive Feature
                if (ActionReady(DragonfireDive) &&
                     CanDRGWeave(DragonfireDive) &&
                    TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return DragonfireDive;

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                     CanDRGWeave(Stardiver) &&
                    Gauge.IsLOTDActive && TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return Stardiver;

                //Starcross Feature
                if (LevelChecked(Starcross) &&
                     CanDRGWeave(Starcross) &&
                    HasEffect(Buffs.StarcrossReady))
                    return OriginalHook(Stardiver);

                //Rise of the Dragon Feature
                if (LevelChecked(RiseOfTheDragon) &&
                     CanDRGWeave(RiseOfTheDragon) &&
                    HasEffect(Buffs.DragonsFlight))
                    return OriginalHook(DragonfireDive);

                //Mirage Feature
                if (LevelChecked(MirageDive) &&
                     CanDRGWeave(MirageDive) &&
                    HasEffect(Buffs.DiveReady))
                    return OriginalHook(HighJump);

                //Nastrond Feature
                if (LevelChecked(Nastrond) &&
                     CanDRGWeave(Nastrond) &&
                    HasEffect(Buffs.NastrondReady) &&
                    Gauge.IsLOTDActive)
                    return OriginalHook(Geirskogul);
            }

            if (ComboTimer > 0)
            {
                if (!SonicThrust.LevelChecked())
                {
                    if (ComboAction == TrueThrust && LevelChecked(Disembowel))
                        return Disembowel;

                    if (ComboAction == Disembowel && LevelChecked(ChaosThrust))
                        return OriginalHook(ChaosThrust);
                }

                else
                {
                    if (ComboAction is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                        return SonicThrust;

                    if (ComboAction == SonicThrust && LevelChecked(CoerthanTorment))
                        return CoerthanTorment;
                }
            }

            return !HasEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
                ? OriginalHook(TrueThrust)
                : actionID;
        }
    }

    internal class DRG_AOE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AOE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not DoomSpike)
                return actionID;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.DRG_Variant_Cure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.DRG_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                 CanDRGWeave(Variant.VariantRampart))
                return Variant.VariantRampart;

            // Piercing Talon Uptime Option
            if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) &&
                LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasEffect(Buffs.PowerSurge))
            {
                if (IsEnabled(CustomComboPreset.DRG_AoE_Buffs))
                {
                    //Lance Charge Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Lance) &&
                        ActionReady(LanceCharge) &&
                         CanDRGWeave(LanceCharge) &&
                        GetTargetHPPercent() >= Config.DRG_AoE_LanceChargeHP)
                        return LanceCharge;

                    //Battle Litany Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Litany) &&
                        ActionReady(BattleLitany) &&
                         CanDRGWeave(BattleLitany) &&
                        GetTargetHPPercent() >= Config.DRG_AoE_LitanyHP)
                        return BattleLitany;
                }

                if (IsEnabled(CustomComboPreset.DRG_AoE_CDs))
                {
                    //Life Surge Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                        ActionReady(LifeSurge) &&
                         CanDRGWeave(LifeSurge) && !HasEffect(Buffs.LifeSurge) &&
                        ((JustUsed(SonicThrust) && LevelChecked(CoerthanTorment)) ||
                         (JustUsed(DoomSpike) && LevelChecked(SonicThrust)) ||
                         (JustUsed(DoomSpike) && !LevelChecked(SonicThrust))))
                        return LifeSurge;

                    //Wyrmwind Thrust Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Wyrmwind) &&
                        LevelChecked(WyrmwindThrust) &&
                         CanDRGWeave(WyrmwindThrust) &&
                        Gauge.FirstmindsFocusCount is 2)
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Geirskogul) &&
                        ActionReady(Geirskogul) &&
                         CanDRGWeave(Geirskogul))

                        return Geirskogul;

                    //(High) Jump Feature   
                    if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                        ActionReady(OriginalHook(Jump)) &&
                         CanDRGWeave(OriginalHook(Jump)) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_HighJump_Melee) ||
                         (IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Melee) && TimeMoving.Ticks == 0 &&
                          GetTargetDistance() <= 1)))
                        return OriginalHook(Jump);

                    //Dragonfire Dive Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive) &&
                        ActionReady(DragonfireDive) &&
                         CanDRGWeave(DragonfireDive) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Melee) ||
                         (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Melee) && TimeMoving.Ticks == 0 &&
                          GetTargetDistance() <= 1)))
                        return DragonfireDive;

                    //StarDiver Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) &&
                        ActionReady(Stardiver) &&
                         CanDRGWeave(Stardiver) &&
                        Gauge.IsLOTDActive &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_Stardiver_Melee) ||
                         (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Melee) && TimeMoving.Ticks == 0 &&
                          GetTargetDistance() <= 1)))
                        return Stardiver;

                    //Starcross Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Starcross) &&
                        LevelChecked(Starcross) &&
                         CanDRGWeave(Starcross) &&
                        HasEffect(Buffs.StarcrossReady))
                        return OriginalHook(Stardiver);

                    //Rise of the Dragon Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RiseOfTheDragon) &&
                        LevelChecked(RiseOfTheDragon) &&
                         CanDRGWeave(RiseOfTheDragon) &&
                        HasEffect(Buffs.DragonsFlight))
                        return OriginalHook(DragonfireDive);

                    //Mirage Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) &&
                        LevelChecked(MirageDive) &&
                         CanDRGWeave(MirageDive) &&
                        HasEffect(Buffs.DiveReady))
                        return OriginalHook(HighJump);

                    //Nastrond Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Nastrond) &&
                        LevelChecked(Nastrond) &&
                         CanDRGWeave(Nastrond) &&
                        HasEffect(Buffs.NastrondReady) &&
                        Gauge.IsLOTDActive)
                        return OriginalHook(Geirskogul);
                }
            }

            // healing
            if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
            {
                if (PlayerHealthPercentageHp() <= Config.DRG_AoE_SecondWind_Threshold &&
                    ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (PlayerHealthPercentageHp() <= Config.DRG_AoE_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                    return All.Bloodbath;
            }

            if (ComboTimer > 0)
            {
                if (IsEnabled(CustomComboPreset.DRG_AoE_Disembowel) &&
                    !SonicThrust.LevelChecked())
                {
                    if (ComboAction == TrueThrust && LevelChecked(Disembowel))
                        return Disembowel;

                    if (ComboAction == Disembowel && LevelChecked(ChaosThrust))
                        return OriginalHook(ChaosThrust);
                }

                else
                {
                    if (ComboAction is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                        return SonicThrust;

                    if (ComboAction == SonicThrust && LevelChecked(CoerthanTorment))
                        return CoerthanTorment;
                }
            }

            return IsEnabled(CustomComboPreset.DRG_AoE_Disembowel) &&
                   !HasEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
                ? OriginalHook(TrueThrust)
                : actionID;
        }
    }

    internal class DRG_BurstCDFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_BurstCDFeature;

        protected override uint Invoke(uint actionID) => actionID is LanceCharge && IsOnCooldown(LanceCharge) && ActionReady(BattleLitany)
                ? BattleLitany
                : actionID;
    }
}
