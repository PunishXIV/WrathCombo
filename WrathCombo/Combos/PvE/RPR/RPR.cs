using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.RPR.Config;
namespace WrathCombo.Combos.PvE;

internal partial class RPR : Melee
{
    internal class RPR_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Slice)
                return actionID;

            //Soulsow
            if (LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) &&
                !PartyInCombat())
                return Soulsow;

            if (!HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.SoulReaver) &&
                ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //All Weaves
            if (CanWeave())
            {
                if (CanArcaneCircleWeave())
                    return ArcaneCircle;

                if (CanEnshroud())
                    return Enshroud;

                if (CanPostPerfectioGluttonyWeave())
                    return Gluttony;

                if (CanTrueNorthForGluttony())
                    return Role.TrueNorth;

                if (CanGluttonyWeave())
                    return Gluttony;

                if (CanBloodstalkOverflow())
                    return OriginalHook(BloodStalk);

                if (UseEnshroudWeaves(out uint weave, false))
                    return weave;

                if (Role.CanFeint() && GroupDamageIncoming())
                    return Role.Feint;

                if (CanUseArcaneCrest)
                    return ArcaneCrest;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (CanPerfectioGCD())
                return PerfectioAction;

            if (UsePostPerfectioGCD(actionID, false) is var postPerfectioGcd and not 0)
                return postPerfectioGcd;

            if (CanUseShadowOfDeath())
                return ShadowOfDeath;

            if (CanGibbetGallowsGCD())
            {
                uint gg = GibbetGallowsAction(simpleMode: true);
                if (gg != 0)
                    return gg;
            }

            if (CanPlentifulHarvest())
                return PlentifulHarvest;

            if (EnshroudComboGCD(false) is var enshroudGcd and not 0)
                return enshroudGcd;

            if (CanSoulSliceScythe(false))
                return SoulSlice;

            return !InMeleeRange() && HasBattleTarget() &&
                   !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.SoulReaver)
                ? RangedAttack(actionID, true)
                : DoBasicCombo(actionID);
        }
    }

    internal class RPR_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningScythe)
                return actionID;

            //Soulsow
            if (LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (CanArcaneCircleWeave(onAoE: true))
                    return ArcaneCircle;

                if (CanEnshroud(true))
                    return Enshroud;

                if (CanPostPerfectioGluttonyWeave(true))
                    return Gluttony;

                if (CanGluttonyWeave(true))
                    return Gluttony;

                if (CanGrimSwatheOverflow(true))
                    return GrimSwathe;

                if (UseEnshroudWeaves(out uint weave, true))
                    return weave;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (CanWhorlOfDeath())
                return WhorlOfDeath;

            if (CanPerfectioGCD())
                return PerfectioAction;

            if (UsePostPerfectioGCD(actionID, true) is var postPerfectioGcd and not 0)
                return postPerfectioGcd;

            if (CanPlentifulHarvest())
                return PlentifulHarvest;

            if (CanGuillotineGCD())
                return OriginalHook(Guillotine);

            if (EnshroudComboGCD(true) is var enshroudGcd and not 0)
                return enshroudGcd;

            if (CanSoulSliceScythe(true))
                return SoulScythe;

            return DoBasicCombo(actionID, true);
        }
    }

    internal class RPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Slice)
                return actionID;

            int positionalChoice = RPR_Positional;

            //Soulsow
            if (IsEnabled(Preset.RPR_ST_SoulSow) &&
                LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            //RPR Opener
            if (IsEnabled(Preset.RPR_ST_Opener) &&
                Opener().FullOpener(ref actionID) && HasBattleTarget())
                return actionID;

            if (!HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.SoulReaver) &&
                ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //All Weaves
            if (CanWeave())
            {
                if (IsEnabled(Preset.RPR_ST_ArcaneCircle) &&
                    CanArcaneCircleWeave(hpThreshold: ArcaneCircleHPThreshold))
                    return ArcaneCircle;

                if (IsEnabled(Preset.RPR_ST_Enshroud) &&
                    CanEnshroud())
                    return Enshroud;

                if (IsEnabled(Preset.RPR_ST_TrueNorthDynamic) &&
                    IsEnabled(Preset.RPR_ST_Gluttony) &&
                    CanTrueNorthForGluttony(true, RPR_ManualTN))
                    return Role.TrueNorth;

                if (IsEnabled(Preset.RPR_ST_Gluttony) &&
                    CanGluttonyWeave())
                    return Gluttony;

                if (IsEnabled(Preset.RPR_ST_Bloodstalk) &&
                    CanBloodstalkOverflow(gluttonyEnabled: IsEnabled(Preset.RPR_ST_Gluttony)))
                    return OriginalHook(BloodStalk);

                if (UseEnshroudWeaves(out uint weave, false,
                    IsEnabled(Preset.RPR_ST_Sacrificium),
                    IsEnabled(Preset.RPR_ST_Lemure)))
                    return weave;

                if (IsEnabled(Preset.RPR_ST_Feint) &&
                    Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                if (IsEnabled(Preset.RPR_ST_ArcaneCrest) &&
                    CanUseArcaneCrest)
                    return ArcaneCrest;

                if (IsEnabled(Preset.RPR_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(RPR_ST_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(RPR_ST_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.RPR_ST_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.RPR_ST_Perfectio) &&
                CanPerfectioGCD())
                return OriginalHook(Communio);

            if (IsEnabled(Preset.RPR_ST_SoD) &&
                CanUseShadowOfDeath(RPR_SoDRefreshRange, RPR_ST_ArcaneCircleHPBossOption == 1) &&
                GetTargetHPPercent() > RPR_SoDHPThreshold)
                return ShadowOfDeath;

            if (IsEnabled(Preset.RPR_ST_GibbetGallows) &&
                CanGibbetGallowsGCD())
            {
                uint gg = GibbetGallowsAction(false, positionalChoice,
                    IsEnabled(Preset.RPR_ST_TrueNorthDynamic),
                    RPR_ManualTN, RPR_ST_TrueNorthDynamicHoldCharge);
                if (gg != 0)
                    return gg;
            }

            if (IsEnabled(Preset.RPR_ST_PlentifulHarvest) &&
                CanPlentifulHarvest())
                return PlentifulHarvest;

            if (EnshroudComboGCD(false,
                IsEnabled(Preset.RPR_ST_Communio),
                IsEnabled(Preset.RPR_ST_Reaping)) is var enshroudGcd and not 0)
                return enshroudGcd;

            if (IsEnabled(Preset.RPR_ST_SoulSlice) &&
                CanSoulSliceScythe(false))
                return SoulSlice;

            return !InMeleeRange() && HasBattleTarget() &&
                   !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.SoulReaver)
                ? RangedAttack(actionID)
                : DoBasicCombo(actionID);
        }
    }

    internal class RPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningScythe)
                return actionID;

            //Soulsow
            if (IsEnabled(Preset.RPR_AoE_SoulSow) &&
                LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.RPR_AoE_ArcaneCircle) &&
                    CanArcaneCircleWeave(true, RPR_AoE_ArcaneCircleHPThreshold))
                    return ArcaneCircle;

                if (IsEnabled(Preset.RPR_AoE_Enshroud) &&
                    CanEnshroud(true))
                    return Enshroud;

                if (IsEnabled(Preset.RPR_AoE_Gluttony) &&
                    CanGluttonyWeave(true))
                    return Gluttony;

                if (IsEnabled(Preset.RPR_AoE_GrimSwathe) &&
                    CanGrimSwatheOverflow(true))
                    return GrimSwathe;

                if (UseEnshroudWeaves(out uint weave, true,
                    IsEnabled(Preset.RPR_AoE_Sacrificium),
                    IsEnabled(Preset.RPR_AoE_Lemure),
                    false))
                    return weave;

                if (IsEnabled(Preset.RPR_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(RPR_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(RPR_AoE_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.RPR_AoE_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.RPR_AoE_Perfectio) &&
                CanPerfectioGCD())
                return OriginalHook(Communio);

            if (IsEnabled(Preset.RPR_AoE_WoD) &&
                CanWhorlOfDeath(hpThreshold: RPR_WoDHPThreshold))
                return WhorlOfDeath;

            if (IsEnabled(Preset.RPR_AoE_PlentifulHarvest) &&
                CanPlentifulHarvest())
                return PlentifulHarvest;

            if (IsEnabled(Preset.RPR_AoE_Guillotine) &&
                CanGuillotineGCD())
                return OriginalHook(Guillotine);

            if (EnshroudComboGCD(true,
                IsEnabled(Preset.RPR_AoE_Communio),
                IsEnabled(Preset.RPR_AoE_Reaping)) is var enshroudGcd and not 0)
                return enshroudGcd;

            if (IsEnabled(Preset.RPR_AoE_SoulScythe) &&
                CanSoulSliceScythe(true))
                return SoulScythe;

            return DoBasicCombo(actionID, true);
        }
    }

    internal class RPR_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not InfernalSlice)
                return actionID;

            if (IsEnabled(Preset.RPR_ST_BasicCombo_SoD) &&
                ActionReady(ShadowOfDeath) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < RPR_SoDRefreshRangeBasicCombo)
                return ShadowOfDeath;

            if (ComboTimer > 0)
            {
                if (ComboAction is Slice && LevelChecked(WaxingSlice))
                    return WaxingSlice;

                if (ComboAction is WaxingSlice && LevelChecked(InfernalSlice))
                    return InfernalSlice;
            }

            return Slice;
        }
    }

    internal class RPR_AoE_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not NightmareScythe)
                return actionID;

            if (IsEnabled(Preset.RPR_AoE_BasicCombo_WoD) &&
                ActionReady(WhorlOfDeath) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < RPR_WoDRefreshRangeBasicCombo)
                return WhorlOfDeath;

            if (ComboTimer > 0)
            {
                if (ComboAction is SpinningScythe && LevelChecked(NightmareScythe))
                    return NightmareScythe;
            }

            return SpinningScythe;
        }
    }

    internal class RPR_GluttonyBloodSwathe : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_GluttonyBloodSwathe;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (BloodStalk or GrimSwathe))
                return actionID;

            switch (actionID)
            {
                case GrimSwathe:
                {
                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_OGCD))
                    {
                        if (ActionReady(Enshroud) || HasStatusEffect(Buffs.IdealHost))
                            return Enshroud;

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            //Sacrificium
                            if (Lemure is 2 && HasStatusEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            //Lemure's Slice
                            if (Void >= 2 && LevelChecked(LemuresScythe))
                                return OriginalHook(GrimSwathe);
                        }
                    }

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasStatusEffect(Buffs.PerfectioParata))
                            return OriginalHook(Communio);

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            switch (Lemure)
                            {
                                case 1 when Void == 0 && LevelChecked(Communio):
                                    return Communio;

                                case 2 when Void is 1 && HasStatusEffect(Buffs.Oblatio):
                                    return OriginalHook(Gluttony);
                            }

                            if (Void >= 2 && LevelChecked(LemuresScythe))
                                return OriginalHook(GrimSwathe);

                            if (Lemure > 1)
                                return OriginalHook(Guillotine);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                        HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) && LevelChecked(Guillotine))
                        return Guillotine;

                    break;
                }

                case BloodStalk:
                {
                    if (IsEnabled(Preset.RPR_TrueNorthGluttony) && Role.CanTrueNorth() &&
                        (GetStatusEffectStacks(Buffs.SoulReaver) is 2 || HasStatusEffect(Buffs.Executioner)))
                        return Role.TrueNorth;

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_OGCD))
                    {
                        if (ActionReady(Enshroud) || HasStatusEffect(Buffs.IdealHost))
                            return Enshroud;

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            //Sacrificium
                            if (Lemure is 2 && HasStatusEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            //Lemure's Slice
                            if (Void >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);
                        }
                    }

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasStatusEffect(Buffs.PerfectioParata))
                            return OriginalHook(Communio);

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            switch (Lemure)
                            {
                                case 1 when Void == 0 && LevelChecked(Communio):
                                    return Communio;

                                case 2 when Void is 1 && HasStatusEffect(Buffs.Oblatio):
                                    return OriginalHook(Gluttony);
                            }

                            if (Void >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);

                            if (HasStatusEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                                !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gallows);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                        HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
                    {
                        if (HasStatusEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasStatusEffect(Buffs.EnhancedGallows) ||
                            !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                    }

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_Soulsow : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_Soulsow;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Harpe or Slice or SpinningScythe) &&
                actionID is not (ShadowOfDeath or BloodStalk))
                return actionID;

            bool soulsowReady = ActionReady(Soulsow) && !HasStatusEffect(Buffs.Soulsow);

            if (soulsowReady && !InCombat() && IsSoulsowEnabledForAction(actionID))
                return Soulsow;

            if (IsEnabled(Preset.RPR_Soulsow_Combat) &&
                actionID is Harpe && !HasBattleTarget() && soulsowReady)
                return Soulsow;

            return actionID;
        }
    }

    internal class RPR_ArcaneCirclePlentifulHarvest : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ArcaneCirclePlentifulHarvest;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ArcaneCircle)
                return actionID;

            return HasStatusEffect(Buffs.ImmortalSacrifice) &&
                   LevelChecked(PlentifulHarvest)
                ? PlentifulHarvest
                : actionID;
        }
    }

    internal class RPR_Regress : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_Regress;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HellsEgress or HellsIngress))
                return actionID;

            return GetStatusEffect(Buffs.Threshold)?.RemainingTime <= 9
                ? Regress
                : actionID;
        }
    }

    internal class RPR_EnshroudProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_EnshroudProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Enshroud)
                return actionID;

            if (IsEnabled(Preset.RPR_TrueNorthEnshroud) &&
                (GetStatusEffectStacks(Buffs.SoulReaver) is 2 || HasStatusEffect(Buffs.Executioner)) &&
                Role.CanTrueNorth())
                return Role.TrueNorth;

            if (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner))
            {
                if (HasStatusEffect(Buffs.EnhancedGibbet))
                    return OriginalHook(Gibbet);

                if (HasStatusEffect(Buffs.EnhancedGallows) ||
                    !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                    return OriginalHook(Gallows);
            }

            return actionID;
        }
    }

    internal class RPR_EnshroudCommunio : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_EnshroudCommunio;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Enshroud)
                return actionID;

            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (HasStatusEffect(Buffs.Enshrouded))
                return Communio;

            return actionID;
        }
    }

    internal class RPR_CommunioOnGGG : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_CommunioOnGGG;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Gibbet or Gallows or Guillotine))
                return actionID;

            switch (actionID)
            {
                case Gibbet or Gallows when HasStatusEffect(Buffs.Enshrouded):
                {
                    if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(Preset.RPR_LemureOnGGG) &&
                        Void >= 2 && LevelChecked(LemuresSlice) && CanWeave())
                        return OriginalHook(BloodStalk);

                    break;
                }

                case Guillotine when HasStatusEffect(Buffs.Enshrouded):
                {
                    if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(Preset.RPR_LemureOnGGG) &&
                        Void >= 2 && LevelChecked(LemuresScythe) && CanWeave())
                        return OriginalHook(GrimSwathe);

                    break;
                }
            }

            return actionID;
        }
    }
}
