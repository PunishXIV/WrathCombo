using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class DRG : Melee
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

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.DRG_Variant_Rampart) && CanDRGWeave(Variant.Rampart))
                return Variant.Rampart;

            // Piercing Talon Uptime Option
            if (ActionReady(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge))
            {
                //Battle Litany Feature
                if (ActionReady(BattleLitany) &&
                    CanDRGWeave(BattleLitany) &&
                    InBossEncounter())
                    return BattleLitany;

                //Lance Charge Feature
                if (ActionReady(LanceCharge) &&
                    CanDRGWeave(LanceCharge) &&
                    InBossEncounter())
                    return LanceCharge;

                //Life Surge Feature
                if (UseLifeSurge())
                    return LifeSurge;

                //Mirage Feature
                if (ActionReady(MirageDive) &&
                    CanDRGWeave(MirageDive) &&
                    HasStatusEffect(Buffs.DiveReady) &&
                    Gauge.IsLOTDActive)
                    return MirageDive;

                //Wyrmwind Thrust Feature
                if (ActionReady(WyrmwindThrust) &&
                    CanDRGWeave(WyrmwindThrust) &&
                    Gauge.FirstmindsFocusCount is 2 &&
                    (Gauge.IsLOTDActive ||
                     !Gauge.IsLOTDActive && HasStatusEffect(Buffs.DraconianFire)))
                    return WyrmwindThrust;

                //Geirskogul Feature
                if (ActionReady(Geirskogul) &&
                    CanDRGWeave(Geirskogul) &&
                    !Gauge.IsLOTDActive)
                    return Geirskogul;

                //(High) Jump Feature   
                if (ActionReady(OriginalHook(Jump)) &&
                    CanDRGWeave(OriginalHook(Jump)) &&
                    !HasStatusEffect(Buffs.DiveReady) &&
                    (LevelChecked(HighJump) &&
                     (GetCooldownRemainingTime(Geirskogul) < 15 ||
                      Gauge.IsLOTDActive) ||
                     !LevelChecked(HighJump)) &&
                    TimeMoving.Ticks == 0)
                    return OriginalHook(Jump);

                //Dragonfire Dive Feature
                if (ActionReady(DragonfireDive) &&
                    CanDRGWeave(DragonfireDive) &&
                    !HasStatusEffect(Buffs.DragonsFlight) &&
                    TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return DragonfireDive;

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(Stardiver) &&
                    !HasStatusEffect(Buffs.StarcrossReady) &&
                    Gauge.IsLOTDActive &&
                    TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return Stardiver;

                //Starcross Feature
                if (ActionReady(Starcross) &&
                    CanDRGWeave(Starcross) &&
                    HasStatusEffect(Buffs.StarcrossReady))
                    return Starcross;

                //Rise of the Dragon Feature
                if (ActionReady(RiseOfTheDragon) &&
                    CanDRGWeave(RiseOfTheDragon) &&
                    HasStatusEffect(Buffs.DragonsFlight))
                    return RiseOfTheDragon;

                //Nastrond Feature
                if (ActionReady(Nastrond) &&
                    CanDRGWeave(Nastrond) &&
                    HasStatusEffect(Buffs.NastrondReady) &&
                    Gauge.IsLOTDActive)
                    return Nastrond;
            }

            if (Role.CanSecondWind(25))
                return Role.SecondWind;

            if (Role.CanBloodBath(40))
                return Role.Bloodbath;

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDoTDebuff is null ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                {
                    if (Role.CanTrueNorth() &&
                        CanDRGWeave(Role.TrueNorth) &&
                        !OnTargetsRear())
                        return Role.TrueNorth;

                    return OriginalHook(ChaosThrust);
                }

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                {
                    if (Role.CanTrueNorth() &&
                        CanDRGWeave(Role.TrueNorth) &&
                        !OnTargetsRear())
                        return Role.TrueNorth;

                    return WheelingThrust;
                }

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                {
                    if (Role.CanTrueNorth() &&
                        CanDRGWeave(Role.TrueNorth) &&
                        !OnTargetsFlank())
                        return Role.TrueNorth;

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

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.DRG_Variant_Rampart) && CanDRGWeave(Variant.Rampart))
                return Variant.Rampart;

            // Opener for DRG
            if (IsEnabled(CustomComboPreset.DRG_ST_Opener) && 
                Opener().FullOpener(ref actionID))
                return actionID;

            // Piercing Talon Uptime Option
            if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) &&
                ActionReady(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge))
            {
                if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                {
                    //Battle Litany Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                        ActionReady(BattleLitany) &&
                        CanDRGWeave(BattleLitany) &&
                        (Config.DRG_ST_Litany_SubOption == 0 ||
                         Config.DRG_ST_Litany_SubOption == 1 && InBossEncounter()))
                        return BattleLitany;

                    //Lance Charge Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                        ActionReady(LanceCharge) &&
                        CanDRGWeave(LanceCharge) &&
                        (Config.DRG_ST_Lance_SubOption == 0 ||
                         Config.DRG_ST_Lance_SubOption == 1 && InBossEncounter()))
                        return LanceCharge;
                }

                if (IsEnabled(CustomComboPreset.DRG_ST_CDs))
                {
                    //Life Surge Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) &&
                        UseLifeSurge())
                        return LifeSurge;

                    //Mirage Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Mirage) &&
                        ActionReady(MirageDive) &&
                        CanDRGWeave(MirageDive) &&
                        HasStatusEffect(Buffs.DiveReady) &&
                        (IsEnabled(CustomComboPreset.DRG_ST_DoubleMirage) && Gauge.IsLOTDActive ||
                         IsNotEnabled(CustomComboPreset.DRG_ST_DoubleMirage)))
                        return MirageDive;

                    //Wyrmwind Thrust Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                        ActionReady(WyrmwindThrust) &&
                        CanDRGWeave(WyrmwindThrust) &&
                        Gauge.FirstmindsFocusCount is 2 &&
                        (Gauge.IsLOTDActive ||
                         !Gauge.IsLOTDActive && HasStatusEffect(Buffs.DraconianFire)))
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Geirskogul) &&
                        ActionReady(Geirskogul) &&
                        CanDRGWeave(Geirskogul) &&
                        !Gauge.IsLOTDActive)
                        return Geirskogul;

                    //(High) Jump Feature   
                    if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                        ActionReady(OriginalHook(Jump)) &&
                        CanDRGWeave(OriginalHook(Jump)) &&
                        !HasStatusEffect(Buffs.DiveReady) &&
                        (LevelChecked(HighJump) &&
                         (IsEnabled(CustomComboPreset.DRG_ST_DoubleMirage) &&
                          (GetCooldownRemainingTime(Geirskogul) < 15 || Gauge.IsLOTDActive) ||
                          IsNotEnabled(CustomComboPreset.DRG_ST_DoubleMirage)) ||
                         !LevelChecked(HighJump)) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_HighJump_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_ST_HighJump_Melee) &&
                         TimeMoving.Ticks == 0 && GetTargetDistance() <= 1))
                        return OriginalHook(Jump);

                    //Dragonfire Dive Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive) &&
                        ActionReady(DragonfireDive) &&
                        CanDRGWeave(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Melee) && TimeMoving.Ticks == 0 &&
                         GetTargetDistance() <= 1))
                        return DragonfireDive;

                    //StarDiver Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) &&
                        ActionReady(Stardiver) &&
                        CanDRGWeave(Stardiver) &&
                        Gauge.IsLOTDActive &&
                        !HasStatusEffect(Buffs.StarcrossReady) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_Stardiver_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Melee) && TimeMoving.Ticks == 0 &&
                         GetTargetDistance() <= 1))
                        return Stardiver;

                    //Starcross Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Starcross) &&
                        ActionReady(Starcross) &&
                        CanDRGWeave(Starcross) &&
                        HasStatusEffect(Buffs.StarcrossReady))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Dives_RiseOfTheDragon) &&
                        ActionReady(RiseOfTheDragon) &&
                        CanDRGWeave(RiseOfTheDragon) &&
                        HasStatusEffect(Buffs.DragonsFlight))
                        return RiseOfTheDragon;

                    //Nastrond Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Nastrond) &&
                        ActionReady(Nastrond) &&
                        CanDRGWeave(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        Gauge.IsLOTDActive)
                        return Nastrond;
                }
            }

            // healing
            if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
            {
                if (Role.CanSecondWind(Config.DRG_ST_SecondWind_Threshold))
                    return Role.SecondWind;

                if (Role.CanBloodBath(Config.DRG_ST_Bloodbath_Threshold))
                    return Role.Bloodbath;
            }

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDoTDebuff is null ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                {
                    if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                        Role.CanTrueNorth() &&
                        CanDRGWeave(Role.TrueNorth) &&
                        !OnTargetsRear())
                        return Role.TrueNorth;

                    return OriginalHook(ChaosThrust);
                }

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                {
                    if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                        Role.CanTrueNorth() &&
                        CanDRGWeave(Role.TrueNorth) &&
                        !OnTargetsRear())
                        return Role.TrueNorth;

                    return WheelingThrust;
                }

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                {
                    if (IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                        Role.CanTrueNorth() &&
                        CanDRGWeave(Role.TrueNorth) &&
                        !OnTargetsFlank())
                        return Role.TrueNorth;

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

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.DRG_Variant_Rampart) &&
                CanDRGWeave(Variant.Rampart))
                return Variant.Rampart;

            // Piercing Talon Uptime Option
            if (LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge))
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
                    !HasStatusEffect(Buffs.LifeSurge) &&
                    (JustUsed(SonicThrust) && LevelChecked(CoerthanTorment) ||
                     JustUsed(DoomSpike) && LevelChecked(SonicThrust) ||
                     JustUsed(DoomSpike) && !LevelChecked(SonicThrust)))
                    return LifeSurge;

                //Wyrmwind Thrust Feature
                if (ActionReady(WyrmwindThrust) &&
                    CanDRGWeave(WyrmwindThrust) &&
                    Gauge.FirstmindsFocusCount is 2 &&
                    (Gauge.IsLOTDActive ||
                     !Gauge.IsLOTDActive && HasStatusEffect(Buffs.DraconianFire)))
                    return WyrmwindThrust;

                //Geirskogul Feature
                if (ActionReady(Geirskogul) &&
                    CanDRGWeave(Geirskogul) &&
                    !Gauge.IsLOTDActive)
                    return Geirskogul;

                //(High) Jump Feature   
                if (ActionReady(OriginalHook(Jump)) &&
                    CanDRGWeave(OriginalHook(Jump)) &&
                    !HasStatusEffect(Buffs.DiveReady) &&
                    TimeMoving.Ticks == 0)
                    return OriginalHook(Jump);

                //Dragonfire Dive Feature
                if (ActionReady(DragonfireDive) &&
                    CanDRGWeave(DragonfireDive) &&
                    !HasStatusEffect(Buffs.DragonsFlight) &&
                    TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return DragonfireDive;

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(Stardiver) &&
                    !HasStatusEffect(Buffs.StarcrossReady) &&
                    Gauge.IsLOTDActive && TimeMoving.Ticks == 0 && GetTargetDistance() <= 1)
                    return Stardiver;

                //Starcross Feature
                if (ActionReady(Starcross) &&
                    CanDRGWeave(Starcross) &&
                    HasStatusEffect(Buffs.StarcrossReady))
                    return Starcross;

                //Rise of the Dragon Feature
                if (ActionReady(RiseOfTheDragon) &&
                    CanDRGWeave(RiseOfTheDragon) &&
                    HasStatusEffect(Buffs.DragonsFlight))
                    return RiseOfTheDragon;

                //Mirage Feature
                if (ActionReady(MirageDive) &&
                    CanDRGWeave(MirageDive) &&
                    HasStatusEffect(Buffs.DiveReady))
                    return MirageDive;

                //Nastrond Feature
                if (ActionReady(Nastrond) &&
                    CanDRGWeave(Nastrond) &&
                    HasStatusEffect(Buffs.NastrondReady) &&
                    Gauge.IsLOTDActive)
                    return Nastrond;
            }
            if (Role.CanSecondWind(25))
                return Role.SecondWind;

            if (Role.CanBloodBath(40))
                return Role.Bloodbath;

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

            return !HasStatusEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
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

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, Config.DRG_Variant_Cure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.DRG_Variant_Rampart) &&
                CanDRGWeave(Variant.Rampart))
                return Variant.Rampart;

            // Piercing Talon Uptime Option
            if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) &&
                LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge))
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
                        CanDRGWeave(LifeSurge) && !HasStatusEffect(Buffs.LifeSurge) &&
                        (JustUsed(SonicThrust) && LevelChecked(CoerthanTorment) ||
                         JustUsed(DoomSpike) && LevelChecked(SonicThrust) ||
                         JustUsed(DoomSpike) && !LevelChecked(SonicThrust)))
                        return LifeSurge;

                    //Wyrmwind Thrust Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Wyrmwind) &&
                        ActionReady(WyrmwindThrust) &&
                        CanDRGWeave(WyrmwindThrust) &&
                        Gauge.FirstmindsFocusCount is 2 &&
                        (Gauge.IsLOTDActive ||
                         !Gauge.IsLOTDActive && HasStatusEffect(Buffs.DraconianFire)))
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Geirskogul) &&
                        ActionReady(Geirskogul) &&
                        CanDRGWeave(Geirskogul) &&
                        !Gauge.IsLOTDActive)
                        return Geirskogul;

                    //(High) Jump Feature   
                    if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                        ActionReady(OriginalHook(Jump)) &&
                        CanDRGWeave(OriginalHook(Jump)) &&
                        !HasStatusEffect(Buffs.DiveReady) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_HighJump_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Melee) && TimeMoving.Ticks == 0 &&
                         GetTargetDistance() <= 1))
                        return OriginalHook(Jump);

                    //Dragonfire Dive Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive) &&
                        ActionReady(DragonfireDive) &&
                        CanDRGWeave(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Melee) && TimeMoving.Ticks == 0 &&
                         GetTargetDistance() <= 1))
                        return DragonfireDive;

                    //StarDiver Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) &&
                        ActionReady(Stardiver) &&
                        CanDRGWeave(Stardiver) &&
                        Gauge.IsLOTDActive &&
                        !HasStatusEffect(Buffs.StarcrossReady) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_Stardiver_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Melee) && TimeMoving.Ticks == 0 &&
                         GetTargetDistance() <= 1))
                        return Stardiver;

                    //Starcross Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Starcross) &&
                        ActionReady(Starcross) &&
                        CanDRGWeave(Starcross) &&
                        HasStatusEffect(Buffs.StarcrossReady))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RiseOfTheDragon) &&
                        ActionReady(RiseOfTheDragon) &&
                        CanDRGWeave(RiseOfTheDragon) &&
                        HasStatusEffect(Buffs.DragonsFlight))
                        return RiseOfTheDragon;

                    //Mirage Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) &&
                        ActionReady(MirageDive) &&
                        CanDRGWeave(MirageDive) &&
                        HasStatusEffect(Buffs.DiveReady))
                        return MirageDive;

                    //Nastrond Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Nastrond) &&
                        ActionReady(Nastrond) &&
                        CanDRGWeave(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        Gauge.IsLOTDActive)
                        return Nastrond;
                }
            }

            // healing
            if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
            {
                if (Role.CanSecondWind(Config.DRG_AoE_SecondWind_Threshold))
                    return Role.SecondWind;

                if (Role.CanBloodBath(Config.DRG_AoE_Bloodbath_Threshold))
                    return Role.Bloodbath;
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
                   !HasStatusEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
                ? OriginalHook(TrueThrust)
                : actionID;
        }
    }

    internal class DRG_BurstCDFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_BurstCDFeature;

        protected override uint Invoke(uint actionID) =>
            actionID is LanceCharge && IsOnCooldown(LanceCharge) && ActionReady(BattleLitany)
                ? BattleLitany
                : actionID;
    }
}
