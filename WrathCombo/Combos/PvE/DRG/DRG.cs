using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.DRG.Config;
namespace WrathCombo.Combos.PvE;

internal partial class DRG : Melee
{
    internal class DRG_BasicCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.DRG_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FullThrust or HeavensThrust))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           ((LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                             CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)])) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return OriginalHook(TrueThrust);
        }
    }

    internal class DRG_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.DRG_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TrueThrust)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, DRG_Variant_Cure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.DRG_Variant_Rampart) && CanDRGWeave(Variant.Rampart))
                return Variant.Rampart;

            // Piercing Talon Uptime Option
            if (ActionReady(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel))
            {
                //Battle Litany Feature
                if (ActionReady(BattleLitany) &&
                    CanDRGWeave(BattleLitany))
                    return BattleLitany;

                //Lance Charge Feature
                if (ActionReady(LanceCharge) &&
                    CanDRGWeave(LanceCharge))
                    return LanceCharge;

                //Life Surge Feature
                if (UseLifeSurge())
                    return LifeSurge;

                //Mirage Feature
                if (ActionReady(MirageDive) &&
                    CanDRGWeave(MirageDive) &&
                    HasStatusEffect(Buffs.DiveReady) &&
                    (OriginalHook(Jump) is MirageDive) &&
                    (LoTDActive ||
                     (GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                      GetCooldownRemainingTime(Geirskogul) > 3)))
                    return MirageDive;

                //Wyrmwind Thrust Feature
                if (ActionReady(WyrmwindThrust) &&
                    CanDRGWeave(WyrmwindThrust) &&
                    FirstmindsFocus is 2 &&
                    (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                    return WyrmwindThrust;

                //Geirskogul Feature
                if (ActionReady(Geirskogul) &&
                    CanDRGWeave(Geirskogul) &&
                    !LoTDActive)
                    return Geirskogul;

                //(High) Jump Feature   
                if (ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump) &&
                    CanDRGWeave(OriginalHook(Jump)))
                {
                    if (!LevelChecked(HighJump))
                        return Jump;

                    if (LevelChecked(HighJump) &&
                        (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDActive))
                        return (HighJump);
                }

                //Dragonfire Dive Feature
                if (ActionReady(DragonfireDive) &&
                    CanDRGWeave(DragonfireDive) &&
                    !HasStatusEffect(Buffs.DragonsFlight) &&
                    (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)) &&
                    InMeleeRange())
                    return DragonfireDive;

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(Stardiver) &&
                    !HasStatusEffect(Buffs.StarcrossReady) &&
                    LoTDActive && InMeleeRange())
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
                    LoTDActive)
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
                           ((LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                             CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)])) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return Role.CanTrueNorth() &&
                           CanDRGWeave(Role.TrueNorth) &&
                           !OnTargetsRear()
                        ? Role.TrueNorth
                        : OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return Role.CanTrueNorth() &&
                           CanDRGWeave(Role.TrueNorth) &&
                           !OnTargetsRear()
                        ? Role.TrueNorth
                        : WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return Role.CanTrueNorth() &&
                           CanDRGWeave(Role.TrueNorth) &&
                           !OnTargetsFlank()
                        ? Role.TrueNorth
                        : FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return actionID;
        }
    }

    internal class DRG_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.DRG_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TrueThrust)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, DRG_Variant_Cure))
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

            if (HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel))
            {
                if (IsEnabled(CustomComboPreset.DRG_ST_Buffs))
                {
                    //Battle Litany Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Litany) &&
                        ActionReady(BattleLitany) &&
                        CanDRGWeave(BattleLitany) &&
                        (DRG_ST_Litany_SubOption == 0 ||
                         DRG_ST_Litany_SubOption == 1 && InBossEncounter()))
                        return BattleLitany;

                    //Lance Charge Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Lance) &&
                        ActionReady(LanceCharge) &&
                        CanDRGWeave(LanceCharge) &&
                        (DRG_ST_Lance_SubOption == 0 ||
                         DRG_ST_Lance_SubOption == 1 && InBossEncounter()))
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
                        (OriginalHook(Jump) is MirageDive) &&
                        ((IsEnabled(CustomComboPreset.DRG_ST_DoubleMirage) &&
                          (LoTDActive ||
                           (GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                            GetCooldownRemainingTime(Geirskogul) > 3))) ||
                         IsNotEnabled(CustomComboPreset.DRG_ST_DoubleMirage)))
                        return MirageDive;

                    //Wyrmwind Thrust Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) &&
                        ActionReady(WyrmwindThrust) &&
                        CanDRGWeave(WyrmwindThrust) &&
                        FirstmindsFocus is 2 &&
                        (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Geirskogul) &&
                        ActionReady(Geirskogul) &&
                        CanDRGWeave(Geirskogul) &&
                        !LoTDActive)
                        return Geirskogul;

                    //(High) Jump Feature   
                    if (IsEnabled(CustomComboPreset.DRG_ST_HighJump) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_HighJump_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_ST_HighJump_Melee) && InMeleeRange()) &&
                        ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump) &&
                        CanDRGWeave(OriginalHook(Jump)))
                    {
                        if (!LevelChecked(HighJump))
                            return Jump;

                        if (LevelChecked(HighJump) &&
                            ((IsEnabled(CustomComboPreset.DRG_ST_DoubleMirage) &&
                              (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDActive)) ||
                             IsNotEnabled(CustomComboPreset.DRG_ST_DoubleMirage)))
                            return (HighJump);
                    }

                    //Dragonfire Dive Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_ST_DragonfireDive_Melee) && InMeleeRange()) &&
                        ActionReady(DragonfireDive) &&
                        CanDRGWeave(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)))
                        return DragonfireDive;

                    //StarDiver Feature
                    if (IsEnabled(CustomComboPreset.DRG_ST_Stardiver) &&
                        (IsNotEnabled(CustomComboPreset.DRG_ST_Stardiver_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_ST_Stardiver_Melee) && InMeleeRange()) &&
                        ActionReady(Stardiver) &&
                        CanDRGWeave(Stardiver) &&
                        LoTDActive &&
                        !HasStatusEffect(Buffs.StarcrossReady))
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
                        LoTDActive)
                        return Nastrond;
                }
            }

            // healing
            if (IsEnabled(CustomComboPreset.DRG_ST_ComboHeals))
            {
                if (Role.CanSecondWind(DRG_ST_SecondWind_Threshold))
                    return Role.SecondWind;

                if (Role.CanBloodBath(DRG_ST_Bloodbath_Threshold))
                    return Role.Bloodbath;
            }

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           ((LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                             CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)])) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                           Role.CanTrueNorth() &&
                           CanDRGWeave(Role.TrueNorth) &&
                           !OnTargetsRear()
                        ? Role.TrueNorth
                        : OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                           Role.CanTrueNorth() &&
                           CanDRGWeave(Role.TrueNorth) &&
                           !OnTargetsRear()
                        ? Role.TrueNorth
                        : WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return IsEnabled(CustomComboPreset.DRG_TrueNorthDynamic) &&
                           Role.CanTrueNorth() &&
                           CanDRGWeave(Role.TrueNorth) &&
                           !OnTargetsFlank()
                        ? Role.TrueNorth
                        : FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return actionID;
        }
    }

    internal class DRG_AOE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.DRG_AOE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DoomSpike)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, DRG_Variant_Cure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.DRG_Variant_Rampart) &&
                CanDRGWeave(Variant.Rampart))
                return Variant.Rampart;

            // Piercing Talon Uptime Option
            if (LevelChecked(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
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
                    FirstmindsFocus is 2 &&
                    (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                    return WyrmwindThrust;

                //Geirskogul Feature
                if (ActionReady(Geirskogul) &&
                    CanDRGWeave(Geirskogul) &&
                    !LoTDActive)
                    return Geirskogul;

                //(High) Jump Feature   
                if (ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump) &&
                    CanDRGWeave(OriginalHook(Jump)))
                    return (LevelChecked(HighJump))
                        ? HighJump
                        : Jump;

                //Dragonfire Dive Feature
                if (ActionReady(DragonfireDive) &&
                    CanDRGWeave(DragonfireDive) &&
                    !HasStatusEffect(Buffs.DragonsFlight) && InMeleeRange() &&
                    (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)))
                    return DragonfireDive;

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(Stardiver) &&
                    !HasStatusEffect(Buffs.StarcrossReady) &&
                    LoTDActive && InMeleeRange())
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

                if (ActionReady(MirageDive) &&
                    CanDRGWeave(MirageDive) &&
                    HasStatusEffect(Buffs.DiveReady) &&
                    (OriginalHook(Jump) is MirageDive) &&
                    (LoTDActive ||
                     (GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                      GetCooldownRemainingTime(Geirskogul) > 3)))
                    return MirageDive;

                //Nastrond Feature
                if (ActionReady(Nastrond) &&
                    CanDRGWeave(Nastrond) &&
                    HasStatusEffect(Buffs.NastrondReady) &&
                    LoTDActive)
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
        protected internal override CustomComboPreset Preset => CustomComboPreset.DRG_AOE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DoomSpike)
                return actionID;

            if (Variant.CanCure(CustomComboPreset.DRG_Variant_Cure, DRG_Variant_Cure))
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
                        GetTargetHPPercent() >= DRG_AoE_LanceChargeHP)
                        return LanceCharge;

                    //Battle Litany Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Litany) &&
                        ActionReady(BattleLitany) &&
                        CanDRGWeave(BattleLitany) &&
                        GetTargetHPPercent() >= DRG_AoE_LitanyHP)
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
                        FirstmindsFocus is 2 &&
                        (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Geirskogul) &&
                        ActionReady(Geirskogul) &&
                        CanDRGWeave(Geirskogul) &&
                        !LoTDActive)
                        return Geirskogul;

                    //(High) Jump Feature   
                    if (IsEnabled(CustomComboPreset.DRG_AoE_HighJump) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_HighJump_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_AoE_HighJump_Melee) && InMeleeRange()) &&
                        ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump) &&
                        CanDRGWeave(OriginalHook(Jump)))
                        return (LevelChecked(HighJump))
                            ? HighJump
                            : Jump;

                    //Dragonfire Dive Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_AoE_DragonfireDive_Melee) && InMeleeRange()) &&
                        ActionReady(DragonfireDive) &&
                        CanDRGWeave(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)))
                        return DragonfireDive;

                    //StarDiver Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Stardiver) &&
                        (IsNotEnabled(CustomComboPreset.DRG_AoE_Stardiver_Melee) ||
                         IsEnabled(CustomComboPreset.DRG_AoE_Stardiver_Melee) && InMeleeRange()) &&
                        ActionReady(Stardiver) &&
                        CanDRGWeave(Stardiver) &&
                        LoTDActive &&
                        !HasStatusEffect(Buffs.StarcrossReady))
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
                        HasStatusEffect(Buffs.DiveReady) &&
                        (OriginalHook(Jump) is MirageDive) &&
                        (LoTDActive ||
                         (GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                          GetCooldownRemainingTime(Geirskogul) > 3)))
                        return MirageDive;

                    //Nastrond Feature
                    if (IsEnabled(CustomComboPreset.DRG_AoE_Nastrond) &&
                        ActionReady(Nastrond) &&
                        CanDRGWeave(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        LoTDActive)
                        return Nastrond;
                }
            }

            // healing
            if (IsEnabled(CustomComboPreset.DRG_AoE_ComboHeals))
            {
                if (Role.CanSecondWind(DRG_AoE_SecondWind_Threshold))
                    return Role.SecondWind;

                if (Role.CanBloodBath(DRG_AoE_Bloodbath_Threshold))
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
        protected internal override CustomComboPreset Preset => CustomComboPreset.DRG_BurstCDFeature;

        protected override uint Invoke(uint actionID) =>
            actionID is LanceCharge && IsOnCooldown(LanceCharge) && ActionReady(BattleLitany)
                ? BattleLitany
                : actionID;
    }
}
