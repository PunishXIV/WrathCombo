using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.RPR.Config;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class RPR : Melee
{
    internal class RPR_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Slice)) return actionID;

            ReportRPRPositionalHints(simpleMode: true);

            if (ActionLearned(Soulsow) &&
                !LocalPlayer.HasStatus(Buffs.Soulsow) &&
                !PartyInCombat())
                return Soulsow;

            if (!LocalPlayer.HasStatus(Buffs.Executioner) &&
                !LocalPlayer.HasStatus(Buffs.SoulReaver) &&
                ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseArcaneCircle())
                    return ArcaneCircle;

                if (UseEnshroud())
                    return Enshroud;

                if (UseBurstGluttony())
                    return Gluttony;

                if (UseTrueNorthForGluttony())
                    return Role.TrueNorth;

                if (UseGluttony())
                    return Gluttony;

                if (UseBloodstalk())
                    return OriginalHook(BloodStalk);

                if (UseEnshroudWeaves(ref actionID, false))
                    return actionID;

                if (Role.CanFeint() && GroupDamageIncoming())
                    return Role.Feint;

                if (UseArcaneCrest())
                    return ArcaneCrest;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (UsePerfectio())
                return PerfectioAction;

            if (PostBurstGCD(false) is var postBurstGcd and not 0)
                return postBurstGcd;

            if (UseShadowOfDeath())
                return ShadowOfDeath;

            if (UseGibbetGallowsGCD() &&
                UseGibbetGallows(ref actionID))
                return actionID;

            if (UsePlentifulHarvest())
                return PlentifulHarvest;

            if (UseEnshroudComboGCD(ref actionID, false))
                return actionID;

            if (UseSoulSliceScythe(false))
                return SoulSlice;

            return !InMeleeRange() && HasBattleTarget() &&
                   !LocalPlayer.HasStatus(Buffs.Executioner) && !LocalPlayer.HasStatus(Buffs.SoulReaver)
                ? RangedAttack(actionID, true, true)
                : DoBasicCombo();
        }
    }

    internal class RPR_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, SpinningScythe)) return actionID;

            if (ActionLearned(Soulsow) &&
                !LocalPlayer.HasStatus(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseArcaneCircle(onAoE: true))
                    return ArcaneCircle;

                if (UseEnshroud(true))
                    return Enshroud;

                if (UseBurstGluttony(onAoE: true) ||
                    UseGluttony(onAoE: true))
                    return Gluttony;

                if (UseGrimSwathe(true))
                    return GrimSwathe;

                if (UseEnshroudWeaves(ref actionID, true))
                    return actionID;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (UseWhorlOfDeath())
                return WhorlOfDeath;

            if (UsePerfectio())
                return PerfectioAction;

            if (PostBurstGCD(true) is var postBurstGcd and not 0)
                return postBurstGcd;

            if (UsePlentifulHarvest())
                return PlentifulHarvest;

            if (UseGuillotine())
                return OriginalHook(Guillotine);

            if (UseEnshroudComboGCD(ref actionID, true))
                return actionID;

            if (UseSoulSliceScythe(true))
                return SoulScythe;

            return DoBasicCombo(onAoE: true);
        }
    }

    internal class RPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Slice)) return actionID;

            ReportRPRPositionalHints();

            int positionalChoice = RPR_Positional;

            if (IsEnabled(Preset.RPR_ST_SoulSow) &&
                ActionLearned(Soulsow) &&
                !LocalPlayer.HasStatus(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (IsEnabled(Preset.RPR_ST_Opener) &&
                Opener().FullOpener(ref actionID) && HasBattleTarget())
                return actionID;

            if (!LocalPlayer.HasStatus(Buffs.Executioner) &&
                !LocalPlayer.HasStatus(Buffs.SoulReaver) &&
                ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.RPR_ST_ArcaneCircle) &&
                    UseArcaneCircle(hpThreshold: ArcaneCircleHPThreshold))
                    return ArcaneCircle;

                if (IsEnabled(Preset.RPR_ST_Enshroud) &&
                    UseEnshroud())
                    return Enshroud;

                if (IsEnabled(Preset.RPR_ST_TrueNorthDynamic) &&
                    IsEnabled(Preset.RPR_ST_Gluttony) &&
                    UseTrueNorthForGluttony(true, RPR_ManualTN))
                    return Role.TrueNorth;

                if (IsEnabled(Preset.RPR_ST_Gluttony) &&
                    UseGluttony(enshroudEnabled: IsEnabled(Preset.RPR_ST_Enshroud)))
                    return Gluttony;

                if (IsEnabled(Preset.RPR_ST_Bloodstalk) &&
                    UseBloodstalk(
                        IsEnabled(Preset.RPR_ST_Gluttony),
                        IsEnabled(Preset.RPR_ST_Enshroud)))
                    return OriginalHook(BloodStalk);

                if (UseEnshroudWeaves(ref actionID, false,
                    IsEnabled(Preset.RPR_ST_Sacrificium),
                    IsEnabled(Preset.RPR_ST_Lemure),
                    arcaneCircleEnabled: IsEnabled(Preset.RPR_ST_ArcaneCircle),
                    arcaneCircleBossOption: RPR_ST_ArcaneCircleHPBossOption))
                    return actionID;

                if (IsEnabled(Preset.RPR_ST_Feint) &&
                    Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                if (IsEnabled(Preset.RPR_ST_ArcaneCrest) &&
                    UseArcaneCrest())
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
                UsePerfectio())
                return PerfectioAction;

            if (PostBurstGCD(false,
                IsEnabled(Preset.RPR_ST_SoulSlice)) is var postBurstGcd and not 0)
                return postBurstGcd;

            if (IsEnabled(Preset.RPR_ST_SoD) &&
                UseShadowOfDeath(RPR_SoDRefreshRange, RPR_ST_ArcaneCircleHPBossOption == 1,
                    IsEnabled(Preset.RPR_ST_ArcaneCircle)) &&
                GetTargetHPPercent() > RPR_SoDHPThreshold)
                return ShadowOfDeath;

            //Gibbet/Gallows
            if (IsEnabled(Preset.RPR_ST_GibbetGallows) &&
                UseGibbetGallowsGCD(enshroudEnabled: IsEnabled(Preset.RPR_ST_Enshroud)) &&
                UseGibbetGallows(ref actionID, positionalChoice,
                    false,
                    IsEnabled(Preset.RPR_ST_TrueNorthDynamic),
                    RPR_ManualTN,
                    RPR_ST_TrueNorthDynamicHoldCharge))
                return actionID;

            if (IsEnabled(Preset.RPR_ST_PlentifulHarvest) &&
                UsePlentifulHarvest())
                return PlentifulHarvest;

            if (UseEnshroudComboGCD(ref actionID, false,
                IsEnabled(Preset.RPR_ST_Communio),
                IsEnabled(Preset.RPR_ST_Reaping)))
                return actionID;

            if (IsEnabled(Preset.RPR_ST_SoulSlice) &&
                UseSoulSliceScythe(false))
                return SoulSlice;

            return !InMeleeRange() && HasBattleTarget() &&
                   !LocalPlayer.HasStatus(Buffs.Executioner) && !LocalPlayer.HasStatus(Buffs.SoulReaver)
                ? RangedAttack(actionID,
                    IsEnabled(Preset.RPR_ST_RangedFillerHarvestMoon),
                    IsEnabled(Preset.RPR_ST_RangedFiller),
                    RPR_ST_EnhancedHarpe,
                    !RPR_ST_EnhancedHarpe)
                : DoBasicCombo();
        }
    }

    internal class RPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, SpinningScythe)) return actionID;

            if (IsEnabled(Preset.RPR_AoE_SoulSow) &&
                ActionLearned(Soulsow) &&
                !LocalPlayer.HasStatus(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.RPR_AoE_ArcaneCircle) &&
                    UseArcaneCircle(true, RPR_AoE_ArcaneCircleHPThreshold))
                    return ArcaneCircle;

                if (IsEnabled(Preset.RPR_AoE_Enshroud) &&
                    UseEnshroud(true))
                    return Enshroud;

                if (IsEnabled(Preset.RPR_AoE_Gluttony) &&
                    UseGluttony(IsEnabled(Preset.RPR_AoE_Enshroud), true))
                    return Gluttony;

                if (IsEnabled(Preset.RPR_AoE_GrimSwathe) &&
                    UseGrimSwathe(true, IsEnabled(Preset.RPR_AoE_Enshroud)))
                    return GrimSwathe;

                if (UseEnshroudWeaves(ref actionID, true,
                    IsEnabled(Preset.RPR_AoE_Sacrificium),
                    IsEnabled(Preset.RPR_AoE_Lemure),
                    false))
                    return actionID;

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
                UsePerfectio())
                return PerfectioAction;

            if (PostBurstGCD(true,
                IsEnabled(Preset.RPR_AoE_SoulScythe)) is var postBurstGcd and not 0)
                return postBurstGcd;

            if (IsEnabled(Preset.RPR_AoE_WoD) &&
                UseWhorlOfDeath(hpThreshold: RPR_WoDHPThreshold))
                return WhorlOfDeath;

            if (IsEnabled(Preset.RPR_AoE_PlentifulHarvest) &&
                UsePlentifulHarvest())
                return PlentifulHarvest;

            if (IsEnabled(Preset.RPR_AoE_Guillotine) &&
                UseGuillotine(enshroudEnabled: IsEnabled(Preset.RPR_AoE_Enshroud)))
                return OriginalHook(Guillotine);

            if (UseEnshroudComboGCD(ref actionID, true,
                IsEnabled(Preset.RPR_AoE_Communio),
                IsEnabled(Preset.RPR_AoE_Reaping)))
                return actionID;

            if (IsEnabled(Preset.RPR_AoE_SoulScythe) &&
                UseSoulSliceScythe(true))
                return SoulScythe;

            return DoBasicCombo(onAoE: true);
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
                CurrentTarget.Status(Debuffs.DeathsDesign).RemainingTimeOrZero() < RPR_SoDRefreshRangeBasicCombo)
                return ShadowOfDeath;

            if (ComboTimer > 0)
            {
                if (ComboAction is Slice && ActionLearned(WaxingSlice))
                    return WaxingSlice;

                if (ComboAction is WaxingSlice && ActionLearned(InfernalSlice))
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
                CurrentTarget.Status(Debuffs.DeathsDesign).RemainingTimeOrZero() < RPR_WoDRefreshRangeBasicCombo)
                return WhorlOfDeath;

            if (ComboTimer > 0)
            {
                if (ComboAction is SpinningScythe && ActionLearned(NightmareScythe))
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
                            if (ActionReady(Enshroud) || LocalPlayer.HasStatus(Buffs.IdealHost))
                                return Enshroud;

                            if (LocalPlayer.HasStatus(Buffs.Enshrouded))
                            {
                                if (Lemure is 2 && LocalPlayer.HasStatus(Buffs.Oblatio))
                                    return OriginalHook(Gluttony);

                                if (Void >= 2 && ActionLearned(LemuresScythe))
                                    return OriginalHook(GrimSwathe);
                            }
                        }

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud))
                        {
                            if (UseBloodStalkGrimSwatheEnshroudGCD(ref actionID))
                                return actionID;
                        }

                        if (ActionReady(Gluttony) && !LocalPlayer.HasStatus(Buffs.Enshrouded) && !LocalPlayer.HasStatus(Buffs.SoulReaver))
                            return Gluttony;

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                            LocalPlayer.HasStatus(Buffs.Enshrouded) && LocalPlayer.HasStatus(Buffs.Oblatio))
                            return OriginalHook(Gluttony);

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                            UseBloodStalkGrimSwatheSoulReaverGCD(ref actionID,
                                IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud)))
                            return actionID;

                        break;
                    }

                case BloodStalk:
                    {
                        if (IsEnabled(Preset.RPR_TrueNorthGluttony) && Role.CanTrueNorth() &&
                            (LocalPlayer.Status(Buffs.SoulReaver).Stacks is 2 || LocalPlayer.HasStatus(Buffs.Executioner)))
                            return Role.TrueNorth;

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_OGCD))
                        {
                            if (ActionReady(Enshroud) || LocalPlayer.HasStatus(Buffs.IdealHost))
                                return Enshroud;

                            if (LocalPlayer.HasStatus(Buffs.Enshrouded))
                            {
                                if (Lemure is 2 && LocalPlayer.HasStatus(Buffs.Oblatio))
                                    return OriginalHook(Gluttony);

                                if (Void >= 2 && ActionLearned(LemuresSlice))
                                    return OriginalHook(BloodStalk);
                            }
                        }

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud))
                        {
                            if (UseBloodStalkGrimSwatheEnshroudGCD(ref actionID))
                                return actionID;
                        }

                        if (ActionReady(Gluttony) && !LocalPlayer.HasStatus(Buffs.Enshrouded) && !LocalPlayer.HasStatus(Buffs.SoulReaver))
                            return Gluttony;

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                            LocalPlayer.HasStatus(Buffs.Enshrouded) && LocalPlayer.HasStatus(Buffs.Oblatio))
                            return OriginalHook(Gluttony);

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                            UseBloodStalkGrimSwatheSoulReaverGCD(ref actionID,
                                IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud)))
                            return actionID;

                        break;
                    }
            }

            return actionID;
        }
    }

    internal class RPR_BloodStalkEnshroudCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_BloodStalkEnshroudCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (BloodStalk or GrimSwathe))
                return actionID;

            bool enshroudEnabled = IsEnabled(Preset.RPR_BloodStalkEnshroudCombo_Enshroud);

            if (enshroudEnabled &&
                UseBloodStalkGrimSwatheEnshroudGCD(ref actionID))
                return actionID;

            if (IsEnabled(Preset.RPR_BloodStalkEnshroudCombo_BloodSwatheCombo))
                UseBloodStalkGrimSwatheSoulReaverGCD(ref actionID, enshroudEnabled);

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

            bool soulsowReady = ActionReady(Soulsow) && !LocalPlayer.HasStatus(Buffs.Soulsow);

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

            return HasImmortalSacrificeStacks && ActionLearned(PlentifulHarvest)
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

            return LocalPlayer.Status(Buffs.Threshold)?.RemainingTime <= 9
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
                (LocalPlayer.Status(Buffs.SoulReaver).Stacks is 2 || LocalPlayer.HasStatus(Buffs.Executioner)) &&
                Role.CanTrueNorth())
                return Role.TrueNorth;

            if (LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner))
            {
                if (LocalPlayer.HasStatus(Buffs.EnhancedGibbet))
                    return OriginalHook(Gibbet);

                if (LocalPlayer.HasStatus(Buffs.EnhancedGallows) ||
                    !LocalPlayer.HasStatus(Buffs.EnhancedGibbet) && !LocalPlayer.HasStatus(Buffs.EnhancedGallows))
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

            if (LocalPlayer.HasStatus(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (LocalPlayer.HasStatus(Buffs.Enshrouded))
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
                case Gibbet or Gallows when LocalPlayer.HasStatus(Buffs.Enshrouded):
                    {
                        if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && ActionLearned(Communio))
                            return Communio;

                        if (IsEnabled(Preset.RPR_LemureOnGGG) &&
                            Void >= 2 && ActionLearned(LemuresSlice) && CanWeave())
                            return OriginalHook(BloodStalk);

                        break;
                    }

                case Guillotine when LocalPlayer.HasStatus(Buffs.Enshrouded):
                    {
                        if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && ActionLearned(Communio))
                            return Communio;

                        if (IsEnabled(Preset.RPR_LemureOnGGG) &&
                            Void >= 2 && ActionLearned(LemuresScythe) && CanWeave())
                            return OriginalHook(GrimSwathe);

                        break;
                    }
            }

            return actionID;
        }
    }
}
