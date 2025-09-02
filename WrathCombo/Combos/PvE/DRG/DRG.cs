using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.DRG.Config;
namespace WrathCombo.Combos.PvE;

internal partial class DRG : Melee
{
    internal class DRG_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TrueThrust)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // Piercing Talon Uptime Option
            if (ActionReady(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel))
            {
                if (CanDRGWeave())
                {
                    //Battle Litany Feature
                    if (ActionReady(BattleLitany))
                        return BattleLitany;

                    //Lance Charge Feature
                    if (ActionReady(LanceCharge))
                        return LanceCharge;

                    //Life Surge Feature
                    if (UseLifeSurge())
                        return LifeSurge;

                    //Mirage Feature
                    if (ActionReady(MirageDive) &&
                        HasStatusEffect(Buffs.DiveReady) &&
                        OriginalHook(Jump) is MirageDive &&
                        (LoTDActive ||
                         GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                         GetCooldownRemainingTime(Geirskogul) > 3))
                        return MirageDive;

                    //Wyrmwind Thrust Feature
                    if (ActionReady(WyrmwindThrust) &&
                        FirstmindsFocus is 2 &&
                        (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (ActionReady(Geirskogul) &&
                        !LoTDActive)
                        return Geirskogul;

                    //Starcross Feature
                    if (ActionReady(Starcross) &&
                        HasStatusEffect(Buffs.StarcrossReady))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (ActionReady(RiseOfTheDragon) &&
                        HasStatusEffect(Buffs.DragonsFlight))
                        return RiseOfTheDragon;

                    //Nastrond Feature
                    if (ActionReady(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        LoTDActive)
                        return Nastrond;

                    if (Role.CanSecondWind(25))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(40))
                        return Role.Bloodbath;
                }

                if (CanDRGWeave(0.8f))
                {
                    //(High) Jump Feature   
                    if (ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump))
                    {
                        if (!LevelChecked(HighJump))
                            return Jump;

                        if (LevelChecked(HighJump) &&
                            (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDActive))
                            return (HighJump);
                    }

                    //Dragonfire Dive Feature
                    if (ActionReady(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)) &&
                        InMeleeRange())
                        return DragonfireDive;
                }

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(1.5f, true) &&
                    !HasStatusEffect(Buffs.StarcrossReady) &&
                    LoTDActive && InMeleeRange())
                    return Stardiver;
            }

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                            CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)]) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                        ? Role.TrueNorth
                        : OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                        ? Role.TrueNorth
                        : WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsFlank()
                        ? Role.TrueNorth
                        : FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return actionID;
        }
    }

    internal class DRG_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DoomSpike)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // Piercing Talon Uptime Option
            if (LevelChecked(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge))
            {
                if (CanDRGWeave())
                {
                    //Lance Charge Feature
                    if (ActionReady(LanceCharge))
                        return LanceCharge;

                    //Battle Litany Feature
                    if (ActionReady(BattleLitany))
                        return BattleLitany;

                    //Life Surge Feature
                    if (ActionReady(LifeSurge) &&
                        !HasStatusEffect(Buffs.LifeSurge) &&
                        (JustUsed(SonicThrust) && LevelChecked(CoerthanTorment) ||
                         JustUsed(DoomSpike) && LevelChecked(SonicThrust) ||
                         JustUsed(DoomSpike) && !LevelChecked(SonicThrust)))
                        return LifeSurge;

                    //Wyrmwind Thrust Feature
                    if (ActionReady(WyrmwindThrust) &&
                        FirstmindsFocus is 2 &&
                        (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                        return WyrmwindThrust;

                    //Geirskogul Feature
                    if (ActionReady(Geirskogul) &&
                        !LoTDActive)
                        return Geirskogul;

                    //Starcross Feature
                    if (ActionReady(Starcross) &&
                        HasStatusEffect(Buffs.StarcrossReady))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (ActionReady(RiseOfTheDragon) &&
                        HasStatusEffect(Buffs.DragonsFlight))
                        return RiseOfTheDragon;

                    if (ActionReady(MirageDive) &&
                        HasStatusEffect(Buffs.DiveReady) &&
                        OriginalHook(Jump) is MirageDive &&
                        (LoTDActive ||
                         GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                         GetCooldownRemainingTime(Geirskogul) > 3))
                        return MirageDive;

                    //Nastrond Feature
                    if (ActionReady(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        LoTDActive)
                        return Nastrond;

                    if (Role.CanSecondWind(25))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(40))
                        return Role.Bloodbath;
                }

                if (CanDRGWeave(0.8f))
                {
                    //(High) Jump Feature   
                    if (ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump))
                        return (LevelChecked(HighJump))
                            ? HighJump
                            : Jump;

                    //Dragonfire Dive Feature
                    if (ActionReady(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) && InMeleeRange() &&
                        (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)))
                        return DragonfireDive;
                }

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(1.5f, true) &&
                    !HasStatusEffect(Buffs.StarcrossReady) &&
                    LoTDActive && InMeleeRange())
                    return Stardiver;
            }

            if (ComboTimer > 0)
            {
                if (!LevelChecked(SonicThrust))
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

    internal class DRG_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TrueThrust)
                return actionID;

            // Opener for DRG
            if (IsEnabled(Preset.DRG_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // Piercing Talon Uptime Option
            if (IsEnabled(Preset.DRG_ST_RangedUptime) &&
                ActionReady(PiercingTalon) &&
                !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel))
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_ST_Buffs))
                    {
                        //Battle Litany Feature
                        if (IsEnabled(Preset.DRG_ST_Litany) &&
                            ActionReady(BattleLitany) &&
                            (DRG_ST_LitanyBossOption == 0 || InBossEncounter()))
                            return BattleLitany;

                        //Lance Charge Feature
                        if (IsEnabled(Preset.DRG_ST_Lance) &&
                            ActionReady(LanceCharge) &&
                            (DRG_ST_LanceBossOption == 0 || InBossEncounter()))
                            return LanceCharge;
                    }

                    if (IsEnabled(Preset.DRG_ST_CDs))
                    {
                        //Life Surge Feature
                        if (IsEnabled(Preset.DRG_ST_LifeSurge) &&
                            UseLifeSurge())
                            return LifeSurge;

                        //Mirage Feature
                        if (IsEnabled(Preset.DRG_ST_Mirage) &&
                            ActionReady(MirageDive) &&
                            HasStatusEffect(Buffs.DiveReady) &&
                            (OriginalHook(Jump) is MirageDive) &&
                            (DRG_ST_DoubleMirage &&
                             (LoTDActive ||
                              GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                              GetCooldownRemainingTime(Geirskogul) > 3) ||
                             !DRG_ST_DoubleMirage))
                            return MirageDive;

                        //Wyrmwind Thrust Feature
                        if (IsEnabled(Preset.DRG_ST_Wyrmwind) &&
                            ActionReady(WyrmwindThrust) &&
                            FirstmindsFocus is 2 &&
                            (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                            return WyrmwindThrust;

                        //Geirskogul Feature
                        if (IsEnabled(Preset.DRG_ST_Geirskogul) &&
                            ActionReady(Geirskogul) &&
                            !LoTDActive)
                            return Geirskogul;

                        //Starcross Feature
                        if (IsEnabled(Preset.DRG_ST_Starcross) &&
                            ActionReady(Starcross) &&
                            HasStatusEffect(Buffs.StarcrossReady))
                            return Starcross;

                        //Rise of the Dragon Feature
                        if (IsEnabled(Preset.DRG_ST_Dives_RiseOfTheDragon) &&
                            ActionReady(RiseOfTheDragon) &&
                            HasStatusEffect(Buffs.DragonsFlight))
                            return RiseOfTheDragon;

                        //Nastrond Feature
                        if (IsEnabled(Preset.DRG_ST_Nastrond) &&
                            ActionReady(Nastrond) &&
                            HasStatusEffect(Buffs.NastrondReady) &&
                            LoTDActive)
                            return Nastrond;
                    }

                    if (IsEnabled(Preset.DRG_ST_Feint) &&
                        Role.CanFeint() &&
                        RaidWideCasting())
                        return Role.Feint;

                    // healing
                    if (IsEnabled(Preset.DRG_ST_ComboHeals))
                    {
                        if (Role.CanSecondWind(DRG_ST_SecondWindHPThreshold))
                            return Role.SecondWind;

                        if (Role.CanBloodBath(DRG_ST_BloodbathHPThreshold))
                            return Role.Bloodbath;
                    }

                    if (IsEnabled(Preset.DRG_ST_StunInterupt) &&
                        RoleActions.Melee.CanLegSweep() &&
                        !TargetIsBoss() && TargetIsCasting())
                        return Role.LegSweep;
                }

                if (IsEnabled(Preset.DRG_ST_CDs))
                {
                    if (CanDRGWeave(0.8f))
                    {
                        //(High) Jump Feature   
                        if (IsEnabled(Preset.DRG_ST_HighJump) &&
                            (!DRG_ST_JumpMovingOptions[0] ||
                             DRG_ST_JumpMovingOptions[0] && !IsMoving()) &&
                            (!DRG_ST_JumpMovingOptions[1] ||
                             DRG_ST_JumpMovingOptions[1] && InMeleeRange()) &&
                            ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump))
                        {
                            if (!LevelChecked(HighJump))
                                return Jump;

                            if (LevelChecked(HighJump) &&
                                (DRG_ST_DoubleMirage &&
                                 (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDActive) ||
                                 !DRG_ST_DoubleMirage))
                                return (HighJump);
                        }

                        //Dragonfire Dive Feature
                        if (IsEnabled(Preset.DRG_ST_DragonfireDive) &&
                            (!DRG_ST_DragonfireDiveMovingOptions[0] ||
                             DRG_ST_DragonfireDiveMovingOptions[0] && !IsMoving()) &&
                            (!DRG_ST_DragonfireDiveMovingOptions[1] ||
                             DRG_ST_DragonfireDiveMovingOptions[1] && InMeleeRange()) &&
                            ActionReady(DragonfireDive) &&
                            !HasStatusEffect(Buffs.DragonsFlight) &&
                            (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)))
                            return DragonfireDive;
                    }

                    //StarDiver Feature
                    if (IsEnabled(Preset.DRG_ST_Stardiver) &&
                        (!DRG_ST_StardiverMovingOptions[0] ||
                         DRG_ST_StardiverMovingOptions[0] && !IsMoving()) &&
                        (!DRG_ST_StardiverMovingOptions[1] ||
                         DRG_ST_StardiverMovingOptions[1] && InMeleeRange()) &&
                        ActionReady(Stardiver) &&
                        CanDRGWeave(1.5f, true) &&
                        LoTDActive &&
                        !HasStatusEffect(Buffs.StarcrossReady))
                        return Stardiver;
                }
            }

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                            CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)]) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return IsEnabled(Preset.DRG_TrueNorthDynamic) &&
                           Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                        ? Role.TrueNorth
                        : OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return IsEnabled(Preset.DRG_TrueNorthDynamic) &&
                           Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsRear()
                        ? Role.TrueNorth
                        : WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return IsEnabled(Preset.DRG_TrueNorthDynamic) &&
                           Role.CanTrueNorth() && CanDRGWeave() && !OnTargetsFlank()
                        ? Role.TrueNorth
                        : FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return actionID;
        }
    }

    internal class DRG_AOE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DoomSpike)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // Piercing Talon Uptime Option
            if (IsEnabled(Preset.DRG_AoE_RangedUptime) &&
                LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                return PiercingTalon;

            if (HasStatusEffect(Buffs.PowerSurge))
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_AoE_Buffs))
                    {
                        //Lance Charge Feature
                        if (IsEnabled(Preset.DRG_AoE_Lance) &&
                            ActionReady(LanceCharge) &&
                            GetTargetHPPercent() >= DRG_AoE_LanceChargeHPTreshold)
                            return LanceCharge;

                        //Battle Litany Feature
                        if (IsEnabled(Preset.DRG_AoE_Litany) &&
                            ActionReady(BattleLitany) &&
                            GetTargetHPPercent() >= DRG_AoE_LitanyHPTreshold)
                            return BattleLitany;
                    }

                    if (IsEnabled(Preset.DRG_AoE_CDs))
                    {
                        //Life Surge Feature
                        if (IsEnabled(Preset.DRG_AoE_LifeSurge) &&
                            ActionReady(LifeSurge) &&
                            !HasStatusEffect(Buffs.LifeSurge) &&
                            (JustUsed(SonicThrust) && LevelChecked(CoerthanTorment) ||
                             JustUsed(DoomSpike) && LevelChecked(SonicThrust) ||
                             JustUsed(DoomSpike) && !LevelChecked(SonicThrust)))
                            return LifeSurge;

                        //Wyrmwind Thrust Feature
                        if (IsEnabled(Preset.DRG_AoE_Wyrmwind) &&
                            ActionReady(WyrmwindThrust) &&
                            FirstmindsFocus is 2 &&
                            (LoTDActive || HasStatusEffect(Buffs.DraconianFire)))
                            return WyrmwindThrust;

                        //Geirskogul Feature
                        if (IsEnabled(Preset.DRG_AoE_Geirskogul) &&
                            ActionReady(Geirskogul) &&
                            !LoTDActive)
                            return Geirskogul;

                        //Starcross Feature
                        if (IsEnabled(Preset.DRG_AoE_Starcross) &&
                            ActionReady(Starcross) &&
                            HasStatusEffect(Buffs.StarcrossReady))
                            return Starcross;

                        //Rise of the Dragon Feature
                        if (IsEnabled(Preset.DRG_AoE_RiseOfTheDragon) &&
                            ActionReady(RiseOfTheDragon) &&
                            HasStatusEffect(Buffs.DragonsFlight))
                            return RiseOfTheDragon;

                        //Mirage Feature
                        if (IsEnabled(Preset.DRG_AoE_Mirage) &&
                            ActionReady(MirageDive) &&
                            HasStatusEffect(Buffs.DiveReady) &&
                            OriginalHook(Jump) is MirageDive &&
                            (LoTDActive ||
                             GetStatusEffectRemainingTime(Buffs.DiveReady) <= 1.2f &&
                             GetCooldownRemainingTime(Geirskogul) > 3))
                            return MirageDive;

                        //Nastrond Feature
                        if (IsEnabled(Preset.DRG_AoE_Nastrond) &&
                            ActionReady(Nastrond) &&
                            HasStatusEffect(Buffs.NastrondReady) &&
                            LoTDActive)
                            return Nastrond;
                    }

                    // healing
                    if (IsEnabled(Preset.DRG_AoE_ComboHeals))
                    {
                        if (Role.CanSecondWind(DRG_AoE_SecondWindHPThreshold))
                            return Role.SecondWind;

                        if (Role.CanBloodBath(DRG_AoE_BloodbathHPThreshold))
                            return Role.Bloodbath;
                    }

                    if (IsEnabled(Preset.DRG_AoE_StunInterupt) &&
                        RoleActions.Melee.CanLegSweep() &&
                        !TargetIsBoss() && TargetIsCasting())
                        return Role.LegSweep;
                }

                if (IsEnabled(Preset.DRG_AoE_CDs))
                {
                    if (CanDRGWeave(0.8f))
                    {
                        //(High) Jump Feature   
                        if (IsEnabled(Preset.DRG_AoE_HighJump) &&
                            (!DRG_AoE_JumpMovingOptions[0] ||
                             DRG_AoE_JumpMovingOptions[0] && !IsMoving()) &&
                            (!DRG_AoE_JumpMovingOptions[1] ||
                             DRG_AoE_JumpMovingOptions[1] && InMeleeRange()) &&
                            ActionReady(Jump) && (OriginalHook(Jump) is Jump or HighJump))
                            return (LevelChecked(HighJump))
                                ? HighJump
                                : Jump;

                        //Dragonfire Dive Feature
                        if (IsEnabled(Preset.DRG_AoE_DragonfireDive) &&
                            (!DRG_AoE_DragonfireDiveMovingOptions[0] ||
                             DRG_AoE_DragonfireDiveMovingOptions[0] && !IsMoving()) &&
                            (!DRG_AoE_DragonfireDiveMovingOptions[1] ||
                             DRG_AoE_DragonfireDiveMovingOptions[1] && InMeleeRange()) &&
                            ActionReady(DragonfireDive) &&
                            !HasStatusEffect(Buffs.DragonsFlight) &&
                            (LoTDActive || !TraitLevelChecked(Traits.LifeOfTheDragon)))
                            return DragonfireDive;
                    }

                    //StarDiver Feature
                    if (IsEnabled(Preset.DRG_AoE_Stardiver) &&
                        (!DRG_AoE_StardiverMovingOptions[0] ||
                         DRG_AoE_StardiverMovingOptions[0] && !IsMoving()) &&
                        (!DRG_AoE_StardiverMovingOptions[1] ||
                         DRG_AoE_StardiverMovingOptions[1] && InMeleeRange()) &&
                        ActionReady(Stardiver) &&
                        CanDRGWeave(1.5f, true) &&
                        LoTDActive &&
                        !HasStatusEffect(Buffs.StarcrossReady))
                        return Stardiver;
                }
            }

            if (ComboTimer > 0)
            {
                if (IsEnabled(Preset.DRG_AoE_Disembowel) &&
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

            return IsEnabled(Preset.DRG_AoE_Disembowel) &&
                   !HasStatusEffect(Buffs.PowerSurge) && !LevelChecked(SonicThrust)
                ? OriginalHook(TrueThrust)
                : actionID;
        }
    }

    internal class DRG_HeavensThrust : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_HeavensThrust;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FullThrust or HeavensThrust))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return DRG_Heavens_Basic && LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                            CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)]) ||
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

    internal class DRG_ChaoticSpring : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ChaoticSpring;

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

                if (ComboAction == WheelingThrust && LevelChecked(Drakesbane))
                    return Drakesbane;
            }

            return OriginalHook(TrueThrust);
        }
    }

    internal class DRG_BurstCDFeature : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_BurstCDFeature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not LanceCharge)
                return actionID;

            return IsOnCooldown(LanceCharge) && ActionReady(BattleLitany)
                ? BattleLitany
                : actionID;
        }
    }
}
