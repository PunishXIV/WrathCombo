using WrathCombo.CustomComboNS;
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

            if (HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel))
            {
                if (CanDRGWeave())
                {
                    if (ActionReady(BattleLitany))
                        return BattleLitany;

                    //Lance Charge Feature
                    if (CanLanceCharge)
                        return LanceCharge;

                    //Life Surge Feature
                    if (CanLifeSurge())
                        return LifeSurge;

                    //Mirage Feature
                    if (CanMirageDive)
                        return MirageDive;

                    //Geirskogul Feature
                    if (CanUseGeirskogul())
                        return Geirskogul;

                    //Wyrmwind Thrust Feature
                    if (CanUseWyrmwind)
                        return WyrmwindThrust;

                    //Starcross Feature
                    if (ActionReady(Starcross) &&
                        HasStatusEffect(Buffs.StarcrossReady) &&
                        InActionRange(Starcross))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (ActionReady(RiseOfTheDragon) &&
                        HasStatusEffect(Buffs.DragonsFlight) &&
                        InActionRange(RiseOfTheDragon))
                        return RiseOfTheDragon;

                    //Nastrond Feature
                    if (ActionReady(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        LoTDActive && InActionRange(Nastrond))
                        return Nastrond;

                    if (Role.CanFeint() &&
                        GroupDamageIncoming())
                        return Role.Feint;

                    // healing
                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(30))
                        return Role.Bloodbath;

                    if (RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (CanDRGWeave(0.8f))
                {
                    //(High) Jump Feature   
                    if (ActionReady(OriginalHook(Jump)) &&
                        (!LevelChecked(HighJump) && IsOriginal(Jump) ||
                         LevelChecked(HighJump) && IsOriginal(HighJump)))
                        return OriginalHook(Jump);

                    //Dragonfire Dive Feature
                    if (ActionReady(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (LoTDTimerActive || !LevelChecked(Geirskogul)))
                        return DragonfireDive;
                }

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(1.5f, true) &&
                    LoTDActive && !HasStatusEffect(Buffs.StarcrossReady))
                    return Stardiver;
            }

            //1-2-3 Combo
            return !InMeleeRange() && HasBattleTarget()
                ? OutsideOfMelee(actionID, true)
                : BasicCombo(actionID, true);
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

            if (HasStatusEffect(Buffs.PowerSurge))
            {
                if (CanDRGWeave())
                {
                    if (ActionReady(BattleLitany))
                        return BattleLitany;

                    //Lance Charge Feature
                    if (ActionReady(LanceCharge))
                        return LanceCharge;

                    //Life Surge Feature
                    if (ActionReady(LifeSurge) &&
                        !HasStatusEffect(Buffs.LifeSurge) &&
                        InActionRange(DoomSpike) &&
                        (JustUsed(SonicThrust) && LevelChecked(CoerthanTorment) ||
                         JustUsed(DoomSpike) && LevelChecked(SonicThrust) ||
                         JustUsed(DoomSpike) && !LevelChecked(SonicThrust)))
                        return LifeSurge;

                    //Mirage Feature
                    if (ActionReady(MirageDive) &&
                        HasStatusEffect(Buffs.DiveReady) &&
                        OriginalHook(Jump) is MirageDive &&
                        InActionRange(MirageDive))
                        return MirageDive;

                    //Geirskogul Feature
                    if (ActionReady(Geirskogul) &&
                        !LoTDTimerActive && InActionRange(Geirskogul))
                        return Geirskogul;

                    //Wyrmwind Thrust Feature
                    if (CanUseWyrmwind)
                        return WyrmwindThrust;

                    //Starcross Feature
                    if (ActionReady(Starcross) &&
                        HasStatusEffect(Buffs.StarcrossReady) &&
                        InActionRange(Starcross))
                        return Starcross;

                    //Rise of the Dragon Feature
                    if (ActionReady(RiseOfTheDragon) &&
                        HasStatusEffect(Buffs.DragonsFlight) &&
                        InActionRange(RiseOfTheDragon))
                        return RiseOfTheDragon;

                    //Nastrond Feature
                    if (ActionReady(Nastrond) &&
                        HasStatusEffect(Buffs.NastrondReady) &&
                        LoTDActive && InActionRange(Nastrond))
                        return Nastrond;

                    // healing
                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(30))
                        return Role.Bloodbath;

                    if (RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (CanDRGWeave(0.8f))
                {
                    //(High) Jump Feature   
                    if (ActionReady(OriginalHook(Jump)) &&
                        (IsOriginal(Jump) || IsOriginal(HighJump)))
                        return OriginalHook(Jump);

                    //Dragonfire Dive Feature
                    if (ActionReady(DragonfireDive) &&
                        !HasStatusEffect(Buffs.DragonsFlight) &&
                        (LoTDTimerActive || !LevelChecked(Geirskogul)))
                        return DragonfireDive;
                }

                //StarDiver Feature
                if (ActionReady(Stardiver) &&
                    CanDRGWeave(1.5f, true) &&
                    LoTDActive && !HasStatusEffect(Buffs.StarcrossReady))
                    return Stardiver;
            }

            return !InActionRange(DoomSpike) && HasBattleTarget()
                ? OutsideOfMelee(actionID, isAoe: true, simpleMode: true)
                : BasicCombo(actionID, isAoE: true, simpleAoE: true);
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

            if (HasStatusEffect(Buffs.PowerSurge) || !LevelChecked(Disembowel))
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_ST_Buffs))
                    {
                        //Battle Litany Feature
                        if (IsEnabled(Preset.DRG_ST_BattleLitany) &&
                            ActionReady(BattleLitany) &&
                            GetTargetHPPercent() > HPThresholdSTBattleLitany)
                            return BattleLitany;

                        //Lance Charge Feature
                        if (IsEnabled(Preset.DRG_ST_LanceCharge) &&
                            CanLanceCharge &&
                            GetTargetHPPercent() > HPThresholdSTLanceCharge)
                            return LanceCharge;

                        //Life Surge Feature
                        if (IsEnabled(Preset.DRG_ST_LifeSurge) &&
                            CanLifeSurge())
                            return LifeSurge;
                    }

                    if (IsEnabled(Preset.DRG_ST_Damage))
                    {
                        //Mirage Feature
                        if (IsEnabled(Preset.DRG_ST_Mirage) &&
                            CanMirageDive)
                            return MirageDive;

                        //Geirskogul Feature
                        if (IsEnabled(Preset.DRG_ST_Geirskogul) &&
                            CanUseGeirskogul())
                            return Geirskogul;

                        //Wyrmwind Thrust Feature
                        if (IsEnabled(Preset.DRG_ST_Wyrmwind) &&
                            CanUseWyrmwind)
                            return WyrmwindThrust;

                        //Starcross Feature
                        if (IsEnabled(Preset.DRG_ST_Starcross) &&
                            ActionReady(Starcross) &&
                            HasStatusEffect(Buffs.StarcrossReady) &&
                            InActionRange(Starcross))
                            return Starcross;

                        //Rise of the Dragon Feature
                        if (IsEnabled(Preset.DRG_ST_RiseOfTheDragon) &&
                            ActionReady(RiseOfTheDragon) &&
                            HasStatusEffect(Buffs.DragonsFlight) &&
                            InActionRange(RiseOfTheDragon))
                            return RiseOfTheDragon;

                        //Nastrond Feature
                        if (IsEnabled(Preset.DRG_ST_Nastrond) &&
                            ActionReady(Nastrond) &&
                            HasStatusEffect(Buffs.NastrondReady) &&
                            LoTDActive && InActionRange(Nastrond))
                            return Nastrond;
                    }

                    if (IsEnabled(Preset.DRG_ST_Feint) &&
                        Role.CanFeint() &&
                        GroupDamageIncoming())
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
                        RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (IsEnabled(Preset.DRG_ST_Damage))
                {
                    if (CanDRGWeave(0.8f))
                    {
                        //(High) Jump Feature   
                        if (IsEnabled(Preset.DRG_ST_HighJump) &&
                            ActionReady(OriginalHook(Jump)) &&
                            (!DRG_ST_JumpMovingOrInRanged[0] || !IsMoving()) &&
                            (!DRG_ST_JumpMovingOrInRanged[1] || InMeleeRange()) &&
                            (!LevelChecked(HighJump) && IsOriginal(Jump) ||
                             LevelChecked(HighJump) && IsOriginal(HighJump) &&
                             (DRG_ST_DoubleMirage && (GetCooldownRemainingTime(Geirskogul) < 13 || LoTDTimerActive) || !DRG_ST_DoubleMirage)))
                            return OriginalHook(Jump);

                        //Dragonfire Dive Feature
                        if (IsEnabled(Preset.DRG_ST_DragonfireDive) &&
                            ActionReady(DragonfireDive) &&
                            (!DRG_ST_DragonfireDiveMovingOrInRanged[0] || !IsMoving()) &&
                            (!DRG_ST_DragonfireDiveMovingOrInRanged[1] || InMeleeRange()) &&
                            !HasStatusEffect(Buffs.DragonsFlight) &&
                            GetTargetHPPercent() > HPThresholdSTDragonfireDive &&
                            (LoTDTimerActive || !LevelChecked(Geirskogul)))
                            return DragonfireDive;
                    }

                    //StarDiver Feature
                    if (IsEnabled(Preset.DRG_ST_Stardiver) &&
                        ActionReady(Stardiver) &&
                        (!DRG_ST_StardiverMovingOrInRanged[0] || !IsMoving()) &&
                        (!DRG_ST_StardiverMovingOrInRanged[1] || InMeleeRange()) &&
                        CanDRGWeave(1.5f, true) &&
                        LoTDActive && !HasStatusEffect(Buffs.StarcrossReady))
                        return Stardiver;
                }
            }

            //1-2-3 Combo
            return !InMeleeRange() && HasBattleTarget()
                ? OutsideOfMelee(actionID)
                : BasicCombo(actionID, IsEnabled(Preset.DRG_TrueNorthDynamic));
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

            if (HasStatusEffect(Buffs.PowerSurge))
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_AoE_Buffs))
                    {
                        if (IsEnabled(Preset.DRG_AoE_BattleLitany) &&
                            ActionReady(BattleLitany) &&
                            GetTargetHPPercent() > DRG_AoE_BattleLitanyHPTreshold)
                            return BattleLitany;

                        //Lance Charge Feature
                        if (IsEnabled(Preset.DRG_AoE_LanceCharge) &&
                            ActionReady(LanceCharge) &&
                            GetTargetHPPercent() > DRG_AoE_LanceChargeHPTreshold)
                            return LanceCharge;

                        //Life Surge Feature
                        if (IsEnabled(Preset.DRG_AoE_LifeSurge) &&
                            ActionReady(LifeSurge) &&
                            !HasStatusEffect(Buffs.LifeSurge) &&
                            InActionRange(DoomSpike) &&
                            (JustUsed(SonicThrust) && LevelChecked(CoerthanTorment) ||
                             JustUsed(DoomSpike) && LevelChecked(SonicThrust) ||
                             JustUsed(DoomSpike) && !LevelChecked(SonicThrust)))
                            return LifeSurge;
                    }

                    if (IsEnabled(Preset.DRG_AoE_Damage))
                    {
                        //Mirage Feature
                        if (IsEnabled(Preset.DRG_AoE_Mirage) &&
                            ActionReady(MirageDive) &&
                            HasStatusEffect(Buffs.DiveReady) &&
                            OriginalHook(Jump) is MirageDive &&
                            InActionRange(MirageDive))
                            return MirageDive;

                        //Geirskogul Feature
                        if (IsEnabled(Preset.DRG_AoE_Geirskogul) &&
                            ActionReady(Geirskogul) &&
                            !LoTDTimerActive && InActionRange(Geirskogul) &&
                            GetTargetHPPercent() > DRG_AoE_GeirskogulHPTreshold)
                            return Geirskogul;

                        //Wyrmwind Thrust Feature
                        if (IsEnabled(Preset.DRG_AoE_Wyrmwind) &&
                            CanUseWyrmwind)
                            return WyrmwindThrust;

                        //Starcross Feature
                        if (IsEnabled(Preset.DRG_AoE_Starcross) &&
                            ActionReady(Starcross) &&
                            HasStatusEffect(Buffs.StarcrossReady) &&
                            InActionRange(Starcross))
                            return Starcross;

                        //Rise of the Dragon Feature
                        if (IsEnabled(Preset.DRG_AoE_RiseOfTheDragon) &&
                            ActionReady(RiseOfTheDragon) &&
                            HasStatusEffect(Buffs.DragonsFlight) &&
                            InActionRange(RiseOfTheDragon))
                            return RiseOfTheDragon;

                        //Nastrond Feature
                        if (IsEnabled(Preset.DRG_AoE_Nastrond) &&
                            ActionReady(Nastrond) &&
                            HasStatusEffect(Buffs.NastrondReady) &&
                            LoTDActive && InActionRange(Nastrond))
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
                        RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (IsEnabled(Preset.DRG_AoE_Damage))
                {
                    if (CanDRGWeave(0.8f))
                    {
                        //(High) Jump Feature   
                        if (IsEnabled(Preset.DRG_AoE_HighJump) &&
                            ActionReady(OriginalHook(Jump)) &&
                            (!DRG_AoE_JumpMovingOrInRanged[0] || !IsMoving()) &&
                            (!DRG_AoE_JumpMovingOrInRanged[1] || InMeleeRange()) &&
                            (IsOriginal(Jump) || IsOriginal(HighJump)))
                            return OriginalHook(Jump);

                        //Dragonfire Dive Feature
                        if (IsEnabled(Preset.DRG_AoE_DragonfireDive) &&
                            ActionReady(DragonfireDive) &&
                            (!DRG_AoE_DragonfireDiveMovingOrInRanged[0] || !IsMoving()) &&
                            (!DRG_AoE_DragonfireDiveMovingOrInRanged[1] || InMeleeRange()) &&
                            !HasStatusEffect(Buffs.DragonsFlight) &&
                            GetTargetHPPercent() > DRG_AoE_DragonfireDiveHPTreshold &&
                            (LoTDTimerActive || !LevelChecked(Geirskogul)))
                            return DragonfireDive;
                    }

                    //StarDiver Feature
                    if (IsEnabled(Preset.DRG_AoE_Stardiver) &&
                        ActionReady(Stardiver) &&
                        (!DRG_AoE_StardiverMovingOrInRanged[0] || !IsMoving()) &&
                        (!DRG_AoE_StardiverMovingOrInRanged[1] || InMeleeRange()) &&
                        CanDRGWeave(1.5f, true) &&
                        LoTDActive && !HasStatusEffect(Buffs.StarcrossReady))
                        return Stardiver;
                }
            }

            return !InActionRange(DoomSpike) && HasBattleTarget()
                ? OutsideOfMelee(actionID, isAoe: true)
                : BasicCombo(actionID, isAoE: true);
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
                    return DRG_ChaoticCombo && LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                            CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)]) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (DRG_ChaoticCombo && ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (DRG_ChaoticCombo && ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
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
