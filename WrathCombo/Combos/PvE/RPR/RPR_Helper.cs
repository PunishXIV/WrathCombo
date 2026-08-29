using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.API.Enum;
using static ECommons.DalamudServices.Svc;
using static WrathCombo.Combos.PvE.RPR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    #region SoD

    private static bool UseShadowOfDeath(int dotRefresh = 8, bool trashOnly = true, bool arcaneCircleEnabled = true)
    {
        if (ActionLearned(ShadowOfDeath) && !LocalPlayer.HasStatus(Buffs.SoulReaver) &&
            !LocalPlayer.HasStatus(Buffs.Executioner) && !LocalPlayer.HasStatus(Buffs.PerfectioParata) &&
            !LocalPlayer.HasStatus(Buffs.ImmortalSacrifice) && !IsComboExpiring(3) &&
            CurrentTarget.CanApplyStatus(Debuffs.DeathsDesign) &&
            !JustUsed(ShadowOfDeath) && InActionRange(ShadowOfDeath))
        {
            float ddRemaining = CurrentTarget.Status(Debuffs.DeathsDesign).RemainingTimeOrZero();
            bool deathsDesignMissing = !CurrentTarget.HasStatus(Debuffs.DeathsDesign);

            if (trashOnly && !InBossEncounter() &&
                !LocalPlayer.HasStatus(Buffs.Enshrouded) &&
                ddRemaining <= dotRefresh)
                return true;

            if (!trashOnly || InBossEncounter() || !arcaneCircleEnabled)
            {
                //Pre-burst
                if (ActionLearned(PlentifulHarvest) && !LocalPlayer.HasStatus(Buffs.Enshrouded) &&
                    UsesBurstAlignment && AcCD <= 9f + GCD * 2 &&
                    ddRemaining < 30)
                    return true;

                //Double enshroud
                if (ActionLearned(PlentifulHarvest) && LocalPlayer.HasStatus(Buffs.Enshrouded) &&
                    AcCD <= GCD && Lemure is 4 &&
                    (JustUsed(VoidReaping, 2f) || JustUsed(CrossReaping, 2f)))
                    return true;

                //lvl 88+ general use
                if (ActionLearned(PlentifulHarvest) && !LocalPlayer.HasStatus(Buffs.Enshrouded) &&
                    ddRemaining <= dotRefresh &&
                    (deathsDesignMissing || AcCD > GCD * 8 || IsOffCooldown(ArcaneCircle)))
                    return true;

                //below lvl 88 use
                if (!ActionLearned(PlentifulHarvest) &&
                    ddRemaining <= dotRefresh)
                    return true;
            }
        }

        return false;
    }

    #endregion

    #region Ranged Attack

    private static uint RangedAttack(
        uint actionId,
        bool useHarvestMoon = false,
        bool useRangedFiller = false,
        bool enhancedHarpeOnly = false,
        bool allowHarpeWhileMoving = true)
    {
        if (useHarvestMoon &&
            ActionReady(HarvestMoon) && LocalPlayer.HasStatus(Buffs.Soulsow))
            return HarvestMoon;

        if (IsPerfectioReady && InActionRange(PerfectioAction) &&
            (!InMeleeRange() || ShouldSpendPerfectioNow()))
            return PerfectioAction;

        if (useRangedFiller &&
            ActionReady(OriginalHook(Harpe)))
        {
            if (LocalPlayer.HasStatus(Buffs.Enshrouded) && Lemure is 1 &&
                ActionLearned(Communio))
                return Communio;

            if (enhancedHarpeOnly && LocalPlayer.HasStatus(Buffs.EnhancedHarpe) ||
                (!enhancedHarpeOnly || allowHarpeWhileMoving) &&
                (!IsMoving() || LocalPlayer.HasStatus(Buffs.EnhancedHarpe)))
                return OriginalHook(Harpe);
        }

        return actionId;
    }

    #endregion

    #region Basic Combo

    private static uint ContinueBasicCombo(bool onAoE = false)
    {
        if (onAoE)
        {
            if (ComboTimer > 0 &&
                ComboAction == OriginalHook(SpinningScythe) && ActionLearned(NightmareScythe))
                return OriginalHook(NightmareScythe);

            return OriginalHook(SpinningScythe);
        }

        if (ComboTimer > 0)
        {
            if (ComboAction == OriginalHook(Slice) && ActionLearned(WaxingSlice))
                return OriginalHook(WaxingSlice);

            if (ComboAction == OriginalHook(WaxingSlice) && ActionLearned(InfernalSlice))
                return OriginalHook(InfernalSlice);
        }

        return OriginalHook(Slice);
    }

    private static uint DoBasicCombo(bool onAoE = false) =>
        ContinueBasicCombo(onAoE);

    #endregion

    #region Enshroud

    private static float AcCD =>
        GetCooldownRemainingTime(ArcaneCircle);

    private static bool UsesBurstAlignment =>
        InBossEncounter();

    private static bool InNormalRotation =>
        !LocalPlayer.HasStatus(Buffs.Enshrouded) && !LocalPlayer.HasStatus(Buffs.SoulReaver) &&
        !LocalPlayer.HasStatus(Buffs.Executioner) && !LocalPlayer.HasStatus(Buffs.ImmortalSacrifice) &&
        !LocalPlayer.HasStatus(Buffs.IdealHost) && !LocalPlayer.HasStatus(Buffs.PerfectioParata);

    private static bool UseEnshroud(bool onAoE = false)
    {
        if (onAoE && IsComboExpiring(6))
            return false;

        if ((ActionReady(Enshroud) || LocalPlayer.HasStatus(Buffs.IdealHost)) &&
            !LocalPlayer.HasStatus(Buffs.SoulReaver) && !LocalPlayer.HasStatus(Buffs.Executioner) && HasBattleTarget() &&
            !LocalPlayer.HasStatus(Buffs.PerfectioParata) && !LocalPlayer.HasStatus(Buffs.Enshrouded))
        {
            if (!ActionLearned(PlentifulHarvest))
                return true;

            if (LocalPlayer.HasStatus(Buffs.ArcaneCircle))
                return true;

            if (ActionLearned(PlentifulHarvest) &&
                AcCD <= GCD + 1.5f)
                return true;

            if (ActionLearned(PlentifulHarvest) &&
                JustUsed(PlentifulHarvest, 5))
                return true;

            if (!LocalPlayer.HasStatus(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                AcCD.InRange(49, 66))
                return true;

            if (!LocalPlayer.HasStatus(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                Soul >= 90)
                return true;
        }

        return false;
    }

    private static bool IsShroudCapped =>
        Shroud >= MaxShroud;

    private static bool IsShroudOvercapping(bool enshroudEnabled = true, bool onAoE = false) =>
        IsShroudCapped && (!enshroudEnabled || !UseEnshroud(onAoE));

    #endregion

    #region Weaves

    private static bool UseArcaneCircle(bool onAoE = false, int hpThreshold = 0) =>
        ActionReady(ArcaneCircle) && GetTargetHPPercent() > hpThreshold &&
        (onAoE || ActionLearned(Enshroud) && JustUsed(ShadowOfDeath) || !ActionLearned(Enshroud));

    private static bool UseGluttony(bool enshroudEnabled = true, bool onAoE = false) =>
        UseBurstGluttony(enshroudEnabled, onAoE) ||
        !IsShroudOvercapping(enshroudEnabled, onAoE) &&
        ActionReady(Gluttony) && InNormalRotation && !IsComboExpiring(3) &&
        !(InPostBurstSequence && Soul < 50);

    private static bool UseTrueNorthForGluttony(bool advanced = false, int tnChargePool = 0) =>
        !InPostBurstSequence &&
        !LocalPlayer.HasStatus(Buffs.Enshrouded) &&
        ActionLearned(Gluttony) && GetCooldownRemainingTime(Gluttony) <= GCD && Role.CanTrueNorth() &&
        (!advanced || GetRemainingCharges(Role.TrueNorth) > tnChargePool);

    private static bool UseSoulOverflow() =>
        !ShouldHoldSoulOverflowWeave &&
        InNormalRotation &&
        !IsComboExpiring(3);

    private static bool ShouldSpendSoulOvercapST(bool gluttonyEnabled)
    {
        if (!ActionLearned(Gluttony))
            return true;

        if (Soul is 100)
            return true;

        if (!gluttonyEnabled)
            return false;

        return IsOnCooldown(Gluttony) && GetCooldownRemainingTime(Gluttony) > GCD * 4;
    }

    private static bool ShouldSpendSoulOvercapAoE() =>
        !ActionLearned(Gluttony) ||
        Soul is 100 ||
        GetCooldownRemainingTime(Gluttony) > GCD * 5;

    private static bool UseBloodstalk(bool gluttonyEnabled = true, bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled) &&
        UseSoulOverflow() &&
        ActionReady(OriginalHook(BloodStalk)) &&
        ShouldSpendSoulOvercapST(gluttonyEnabled);

    private static bool UseGrimSwathe(bool onAoE = false, bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled, onAoE) &&
        UseSoulOverflow() &&
        ActionReady(GrimSwathe) &&
        InActionRange(onAoE ? OriginalHook(GrimSwathe) : GrimSwathe) &&
        ShouldSpendSoulOvercapAoE();

    private static bool UseSacrificium(
        bool onAoE = false,
        bool useArcaneCircleBoss = true,
        bool arcaneCircleEnabled = true,
        int arcaneCircleBossOption = 0) =>
        LocalPlayer.HasStatus(Buffs.Enshrouded) && LocalPlayer.HasStatus(Buffs.Oblatio) &&
        (onAoE
            ? Lemure is 2 && Void is 1
            : Lemure <= 4) &&
        (!useArcaneCircleBoss || onAoE ||
         GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 && !JustUsed(ArcaneCircle, 2) &&
         (arcaneCircleBossOption == 0 ||
          InBossEncounter() ||
          arcaneCircleBossOption == 1 && !InBossEncounter() && IsOffCooldown(ArcaneCircle)) ||
         !arcaneCircleEnabled);

    private static bool UseLemure(bool onAoE = false) =>
        LocalPlayer.HasStatus(Buffs.Enshrouded) && Void >= 2 &&
        ActionLearned(onAoE ? LemuresScythe : LemuresSlice) &&
        (!onAoE || InActionRange(OriginalHook(GrimSwathe)));

    private static bool UseEnshroudWeaves(ref uint actionID, bool onAoE, bool sacrificium = true, bool lemure = true,
        bool useArcaneCircleBoss = true, bool arcaneCircleEnabled = true, int arcaneCircleBossOption = 0)
    {
        if (!LocalPlayer.HasStatus(Buffs.Enshrouded))
            return false;

        if (sacrificium && UseSacrificium(onAoE, useArcaneCircleBoss, arcaneCircleEnabled, arcaneCircleBossOption))
        {
            actionID = OriginalHook(Gluttony);
            return true;
        }

        if (lemure && UseLemure(onAoE))
        {
            actionID = OriginalHook(onAoE ? GrimSwathe : BloodStalk);
            return true;
        }

        return false;
    }

    #endregion

    #region GCD Burst

    private static bool WithinGCD(uint actionId) =>
        ActionLearned(actionId) && (HasCharges(actionId) || GetCooldownRemainingTime(actionId) <= GCD);

    private static bool IsPerfectioReady =>
        LocalPlayer.HasStatus(Buffs.PerfectioParata) && ActionLearned(Perfectio);

    private static uint PerfectioAction =>
        WithinGCD(Perfectio) ? Perfectio : OriginalHook(Communio);

    private static bool ShouldSpendPerfectioNow() =>
        IsPerfectioReady;

    private static bool UsePerfectio() =>
        IsPerfectioReady && ShouldSpendPerfectioNow() && InActionRange(PerfectioAction);

    private static bool InPostBurstSequence =>
        JustUsed(Perfectio, GCD * 8);

    private static bool HasBurstComboContinue(bool onAoE = false) =>
        InPostBurstSequence &&
        IsComboExpiring(2) &&
        ComboTimer > 0 &&
        (onAoE
            ? ComboAction == OriginalHook(SpinningScythe)
            : ComboAction == OriginalHook(Slice) || ComboAction == OriginalHook(WaxingSlice));

    private static bool UseBurstGluttony(bool enshroudEnabled = true, bool onAoE = false) =>
        !IsShroudOvercapping(enshroudEnabled) &&
        InPostBurstSequence && Soul >= 50 && ActionReady(Gluttony) &&
        !HasBurstComboContinue(onAoE);

    private static bool OvercapSoulSliceProtection(bool onAoE)
    {
        uint action = onAoE ? SoulScythe : SoulSlice;

        if (Soul >= 100)
            return false;

        if (!ActionReady(action))
            return false;

        if (GetRemainingCharges(action) >= GetMaxCharges(action))
            return true;

        return GetRemainingCharges(action) >= 1 &&
               GetCooldownChargeRemainingTime(action) <= GCD * 2;
    }

    private static bool UseBurstSoulSliceScythe(bool onAoE = false) =>
        InPostBurstSequence &&
        !HasBurstComboContinue(onAoE) &&
        !JustUsed(onAoE ? SoulScythe : SoulSlice, GCD) &&
        (onAoE
            ? ActionReady(SoulScythe) && InActionRange(SoulScythe)
            : ActionReady(SoulSlice) && InActionRange(SoulSlice)) &&
        (OvercapSoulSliceProtection(onAoE) || Soul < 50 && ActionReady(Gluttony));

    private static bool ShouldHoldSoulOverflowWeave =>
        Soul < 100 &&
        InPostBurstSequence && !JustUsed(Gluttony, GCD * 8);

    private static uint PostBurstGCD(bool onAoE, bool soulSliceEnabled = true)
    {
        if (!InPostBurstSequence)
            return 0;

        if (LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner) ||
            LocalPlayer.HasStatus(Buffs.ImmortalSacrifice))
            return 0;

        if (ActionLearned(onAoE ? WhorlOfDeath : ShadowOfDeath) &&
            CurrentTarget.Status(Debuffs.DeathsDesign).RemainingTimeOrZero() <= GCD)
            return 0;

        if (HasBurstComboContinue(onAoE))
            return ContinueBasicCombo(onAoE);

        if (soulSliceEnabled && UseBurstSoulSliceScythe(onAoE))
            return onAoE ? SoulScythe : SoulSlice;

        return 0;
    }

    private static bool HasImmortalSacrificeStacks =>
        LocalPlayer.HasStatus(Buffs.ImmortalSacrifice) && LocalPlayer.Status(Buffs.ImmortalSacrifice).Stacks > 0;

    private static bool UsePlentifulHarvest() =>
        !LocalPlayer.HasStatus(Buffs.Enshrouded) && !LocalPlayer.HasStatus(Buffs.SoulReaver) &&
        !LocalPlayer.HasStatus(Buffs.Executioner) && HasImmortalSacrificeStacks &&
        (LocalPlayer.Status(Buffs.BloodsownCircle).RemainingTimeOrZero() <= 1 || JustUsed(Communio));

    private static bool UseWhorlOfDeath(int refreshThreshold = 6, int hpThreshold = 0) =>
        ActionLearned(WhorlOfDeath) && InActionRange(WhorlOfDeath) &&
        CurrentTarget.CanApplyStatus(Debuffs.DeathsDesign) &&
        CurrentTarget.Status(Debuffs.DeathsDesign).RemainingTimeOrZero() < refreshThreshold &&
        !LocalPlayer.HasStatus(Buffs.SoulReaver) && !LocalPlayer.HasStatus(Buffs.Executioner) &&
        GetTargetHPPercent() > hpThreshold;

    private static bool UseGuillotine(bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled, true) &&
        (LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner)) &&
        !LocalPlayer.HasStatus(Buffs.Enshrouded) && ActionLearned(Guillotine) &&
        InActionRange(OriginalHook(Guillotine));

    private static bool UseGibbetGallowsGCD(bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled) &&
        ActionLearned(Gibbet) && !LocalPlayer.HasStatus(Buffs.Enshrouded) &&
        (LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner));

    private static bool UseGibbetGallows(ref uint actionID,
        int positionalChoice = 1,
        bool useSimpleTrueNorth = true,
        bool useDynamicTrueNorth = false,
        int tnChargePool = 0,
        bool holdTnCharge = false)
    {
        bool neitherEnhanced = !LocalPlayer.HasStatus(Buffs.EnhancedGibbet) && !LocalPlayer.HasStatus(Buffs.EnhancedGallows);

        if (LocalPlayer.HasStatus(Buffs.EnhancedGibbet) ||
            useSimpleTrueNorth && neitherEnhanced ||
            !useSimpleTrueNorth && positionalChoice is 1 && neitherEnhanced)
        {
            if (useSimpleTrueNorth && Role.CanTrueNorth() && !OnTargetsFlank() || useDynamicTrueNorth &&
                (holdTnCharge && GetRemainingCharges(Role.TrueNorth) is 2 || !holdTnCharge) &&
                Role.CanTrueNorth() && !OnTargetsFlank() &&
                GetRemainingCharges(Role.TrueNorth) > tnChargePool)
            {
                actionID = Role.TrueNorth;
                return true;
            }

            actionID = OriginalHook(Gibbet);
            return true;
        }

        if (LocalPlayer.HasStatus(Buffs.EnhancedGallows) ||
            useSimpleTrueNorth && neitherEnhanced ||
            !useSimpleTrueNorth && positionalChoice is 0 && neitherEnhanced)
        {
            if (useSimpleTrueNorth && Role.CanTrueNorth() && !OnTargetsRear() || useDynamicTrueNorth &&
                (holdTnCharge && GetRemainingCharges(Role.TrueNorth) is 2 || !holdTnCharge) &&
                Role.CanTrueNorth() && !OnTargetsRear() &&
                GetRemainingCharges(Role.TrueNorth) > tnChargePool)
            {
                actionID = Role.TrueNorth;
                return true;
            }

            actionID = OriginalHook(Gallows);
            return true;
        }

        return false;
    }

    private static bool UseEnshroudComboGCD(ref uint actionID, bool onAoE, bool communio = true, bool reaping = true)
    {
        if (!LocalPlayer.HasStatus(Buffs.Enshrouded))
            return false;

        if (onAoE)
        {
            if (communio && ActionLearned(Communio) && Lemure is 1 && Void is 0)
            {
                actionID = Communio;
                return true;
            }

            if (reaping && Lemure > 0 && InActionRange(OriginalHook(Guillotine)))
            {
                actionID = OriginalHook(Guillotine);
                return true;
            }

            return false;
        }

        if (communio && Lemure is 1 && ActionLearned(Communio))
        {
            actionID = Communio;
            return true;
        }

        if (reaping && LocalPlayer.HasStatus(Buffs.EnhancedVoidReaping))
        {
            actionID = OriginalHook(Gibbet);
            return true;
        }

        if (reaping &&
            (LocalPlayer.HasStatus(Buffs.EnhancedCrossReaping) ||
             !LocalPlayer.HasStatus(Buffs.EnhancedCrossReaping) && !LocalPlayer.HasStatus(Buffs.EnhancedVoidReaping)))
        {
            actionID = OriginalHook(Gallows);
            return true;
        }

        return false;
    }

    private static bool UseBloodStalkGrimSwatheEnshroudGCD(ref uint actionID)
    {
        switch (actionID)
        {
            case GrimSwathe when LocalPlayer.HasStatus(Buffs.PerfectioParata):
                actionID = OriginalHook(Communio);
                return true;
            case GrimSwathe when !LocalPlayer.HasStatus(Buffs.Enshrouded):
                return false;
            case GrimSwathe:
                {
                    switch (Lemure)
                    {
                        case 1 when Void == 0 && ActionLearned(Communio):
                            actionID = Communio;
                            return true;

                        case 2 when Void is 1 && LocalPlayer.HasStatus(Buffs.Oblatio):
                            actionID = OriginalHook(Gluttony);
                            return true;
                    }

                    if (Void >= 2 && ActionLearned(LemuresScythe))
                    {
                        actionID = OriginalHook(GrimSwathe);
                        return true;
                    }

                    if (Lemure > 1)
                    {
                        actionID = OriginalHook(Guillotine);
                        return true;
                    }
                    break;
                }
            case BloodStalk when LocalPlayer.HasStatus(Buffs.PerfectioParata):
                actionID = OriginalHook(Communio);
                return true;

            case BloodStalk when !LocalPlayer.HasStatus(Buffs.Enshrouded):
                break;

            case BloodStalk:
                {
                    switch (Lemure)
                    {
                        case 1 when Void == 0 && ActionLearned(Communio):
                            actionID = Communio;
                            return true;

                        case 2 when Void is 1 && LocalPlayer.HasStatus(Buffs.Oblatio):
                            actionID = OriginalHook(Gluttony);
                            return true;
                    }

                    if (Void >= 2 && ActionLearned(LemuresSlice))
                    {
                        actionID = OriginalHook(BloodStalk);
                        return true;
                    }

                    if (LocalPlayer.HasStatus(Buffs.EnhancedVoidReaping))
                    {
                        actionID = OriginalHook(Gibbet);
                        return true;
                    }

                    if (LocalPlayer.HasStatus(Buffs.EnhancedCrossReaping) ||
                        !LocalPlayer.HasStatus(Buffs.EnhancedCrossReaping) && !LocalPlayer.HasStatus(Buffs.EnhancedVoidReaping))
                    {
                        actionID = OriginalHook(Gallows);
                        return true;
                    }
                    break;
                }
        }

        return false;
    }

    private static bool UseBloodStalkGrimSwatheSoulReaverGCD(ref uint actionID, bool enshroudEnabled = true)
    {
        if (IsShroudOvercapping(enshroudEnabled, actionID is GrimSwathe))
            return false;

        if (actionID is GrimSwathe &&
            (LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner)) &&
            ActionLearned(Guillotine))
        {
            actionID = OriginalHook(Guillotine);
            return true;
        }

        if (actionID is BloodStalk &&
            (LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner)))
        {
            if (LocalPlayer.HasStatus(Buffs.EnhancedGibbet))
            {
                actionID = OriginalHook(Gibbet);
                return true;
            }

            if (LocalPlayer.HasStatus(Buffs.EnhancedGallows) ||
                !LocalPlayer.HasStatus(Buffs.EnhancedGibbet) && !LocalPlayer.HasStatus(Buffs.EnhancedGallows))
            {
                actionID = OriginalHook(Gallows);
                return true;
            }
        }

        return false;
    }

    private static bool UseSoulSliceScythe(bool onAoE) =>
        !InPostBurstSequence &&
        InNormalRotation && !IsComboExpiring(3) &&
        (Soul <= 50 || OvercapSoulSliceProtection(onAoE)) &&
        (onAoE
            ? ActionReady(SoulScythe) && InActionRange(SoulScythe)
            : ActionReady(SoulSlice) && InActionRange(SoulSlice));

    #endregion

    #region Soulsow

    private const int SoulsowOnHarpe = 0;
    private const int SoulsowOnSlice = 1;
    private const int SoulsowOnSpinningScythe = 2;
    private const int SoulsowOnShadowOfDeath = 3;
    private const int SoulsowOnBloodStalk = 4;

    private static bool IsSoulsowEnabledForAction(uint actionId)
    {
        bool[] options = RPR_SoulsowOptions;
        if (options.Length == 0)
            return false;

        return actionId switch
        {
            Harpe => options.Length > SoulsowOnHarpe && options[SoulsowOnHarpe],
            Slice => options.Length > SoulsowOnSlice && options[SoulsowOnSlice],
            SpinningScythe => options.Length > SoulsowOnSpinningScythe && options[SoulsowOnSpinningScythe],
            ShadowOfDeath => options.Length > SoulsowOnShadowOfDeath && options[SoulsowOnShadowOfDeath],
            BloodStalk => options.Length > SoulsowOnBloodStalk && options[SoulsowOnBloodStalk],
            _ => false
        };
    }

    internal static void ReportRPRPositionalHints(bool simpleMode = false)
    {
        if (!TargetNeedsPositionals() || !HasBattleTarget())
        {
            ClearUpcomingPositional();
            return;
        }

        if (LocalPlayer.HasStatus(Buffs.Enshrouded))
        {
            ClearUpcomingPositional();
            return;
        }

        if (LocalPlayer.HasStatus(Buffs.EnhancedGibbet))
            ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(Gibbet), 1);
        else if (LocalPlayer.HasStatus(Buffs.EnhancedGallows))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(Gallows), 1);
        else if ((LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner)) &&
                 ActionLearned(Gibbet))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(Gallows), 1);
        else
            ClearUpcomingPositional();
    }

    #endregion

    #region Misc

    private static bool UseArcaneCrest() =>
        ActionReady(ArcaneCrest) && InCombat() &&
        (GroupDamageIncoming(3f) ||
         !IsInParty() && IsPlayerTargeted());

    private static int BossHpThreshold(int hpBossOption, int hpOption, bool isBoss) =>
        hpBossOption == 1 || !isBoss ? hpOption : 0;

    private static int ArcaneCircleHPThreshold =>
        BossHpThreshold(RPR_ST_ArcaneCircleHPBossOption, RPR_ST_ArcaneCircleHPOption, InBossEncounter());

    #endregion

    #region Combos

    private static float GCD => GetCooldown(Slice).CooldownTotal;

    private static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GCD * times;

        return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < gcd;
    }

    private static bool IsDebuffExpiring(float times)
    {
        float gcd = GCD * times;

        return CurrentTarget.HasStatus(Debuffs.DeathsDesign) && CurrentTarget.Status(Debuffs.DeathsDesign).RemainingTimeOrZero() < gcd;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (DMUOpener.LevelChecked &&
            ClientState.TerritoryType == 1363)
            return DMUOpener;

        if (StandardOpenerLvl100.LevelChecked)
            return StandardOpenerLvl100;

        if (StandardOpenerLvl90.LevelChecked)
            return StandardOpenerLvl90;

        return WrathOpener.Dummy;
    }

    internal static RPRStandardOpenerLvl100 StandardOpenerLvl100 = new();
    internal static RPRDMUOpenerLvl100 DMUOpener = new();
    internal static RPRStandardOpenerLvl90 StandardOpenerLvl90 = new();

    internal abstract class RPROpenerBase : WrathOpener
    {
        public override Preset Preset => Preset.RPR_ST_Opener;

        internal override UserData ContentCheckConfig => RPR_Balance_Content;
        internal override bool IncludePot => RPR_Opener_Potion;

        public override bool HasCooldowns() =>
            GetRemainingCharges(SoulSlice) is 2 &&
            IsOffCooldown(ArcaneCircle) &&
            IsOffCooldown(Gluttony) &&
            Void is 0 && Soul is 0;
    }

    internal class RPRStandardOpenerLvl100 : RPROpenerBase
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => Harpe, // 1
            () => ShadowOfDeath, // 2
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 3
            () => SoulSlice, // 4
            () => ArcaneCircle, // 5
            () => Gluttony, // 6
            () => ExecutionersGibbet, // 7
            () => ExecutionersGallows, // 8
            () => SoulSlice, // 9
            () => PlentifulHarvest, // 10
            () => Enshroud, // 11
            () => VoidReaping, // 12
            () => Sacrificium, // 13
            () => CrossReaping, // 14
            () => LemuresSlice, // 15
            () => VoidReaping, // 16
            () => CrossReaping, // 17
            () => LemuresSlice, // 18
            () => Communio, // 19
            () => Perfectio, // 20
            () => UnveiledGibbet, // 21
            () => Gibbet, // 22
            () => ShadowOfDeath, // 23
            () => Slice // 24
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([7], ExecutionersGallows, OnTargetsRear),
            ([8], ExecutionersGibbet, () => LocalPlayer.HasStatus(Buffs.EnhancedGibbet)),
            ([21], UnveiledGallows, () => LocalPlayer.HasStatus(Buffs.EnhancedGallows)),
            ([22], Gallows, () => LocalPlayer.HasStatus(Buffs.EnhancedGallows))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => InMeleeRange())
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([1], () => CountdownRemaining - 1)
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [3];
    }

    internal class RPRDMUOpenerLvl100 : RPROpenerBase
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => SoulSlice, // 1
            () => ArcaneCircle, // 2
            () => ShadowOfDeath, // 3
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 4
            () => Gluttony, // 5
            () => ExecutionersGibbet, // 6
            () => ExecutionersGallows, // 7
            () => PlentifulHarvest, // 8
            () => Enshroud, // 9
            () => VoidReaping, // 10
            () => Sacrificium, // 11
            () => CrossReaping, // 12
            () => LemuresSlice, // 13
            () => VoidReaping, // 14
            () => CrossReaping, // 15
            () => LemuresSlice, // 16
            () => Communio, // 17
            () => Perfectio, // 18
            () => SoulSlice, // 19
            () => UnveiledGibbet, // 20
            () => Gibbet, // 21
            () => ShadowOfDeath, // 22
            () => Slice // 23
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([6], ExecutionersGallows, OnTargetsRear),
            ([7], ExecutionersGibbet, () => LocalPlayer.HasStatus(Buffs.EnhancedGibbet)),
            ([20], UnveiledGallows, () => LocalPlayer.HasStatus(Buffs.EnhancedGallows)),
            ([21], Gallows, () => LocalPlayer.HasStatus(Buffs.EnhancedGallows))
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [4];
    }

    internal class RPRStandardOpenerLvl90 : RPROpenerBase
    {
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 90;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => Harpe, // 1
            () => ShadowOfDeath, // 2
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 3
            () => ArcaneCircle, // 4
            () => SoulSlice, // 5
            () => SoulSlice, // 6
            () => PlentifulHarvest, // 7
            () => Enshroud, // 8
            () => VoidReaping, // 9
            () => CrossReaping, // 10
            () => LemuresSlice, // 11
            () => VoidReaping, // 12
            () => CrossReaping, // 13
            () => LemuresSlice, // 14
            () => Communio, // 15
            () => HarvestMoon, // 16
            () => Gluttony, // 17
            () => Gibbet, // 18
            () => Gallows, // 19
            () => UnveiledGibbet, // 20
            () => Gibbet // 21
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([17], Gallows, OnTargetsRear),
            ([18], Gibbet, () => LocalPlayer.HasStatus(Buffs.EnhancedGibbet)),
            ([19], UnveiledGallows, () => LocalPlayer.HasStatus(Buffs.EnhancedGallows)),
            ([20], Gallows, () => LocalPlayer.HasStatus(Buffs.EnhancedGallows))
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [3];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => InMeleeRange())
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([1], () => CountdownRemaining - 1)
        ];
    }

    #endregion

    #region Gauge

    private const byte MaxShroud = 100;

    private static RPRGauge Gauge => GetJobGauge<RPRGauge>();

    private static byte Soul => Gauge.Soul;

    private static byte Shroud => Gauge.Shroud;

    private static byte Lemure => Gauge.LemureShroud;

    private static byte Void => Gauge.VoidShroud;

    #endregion

    #region ID's

    public const uint

        // Single Target
        Slice = 24373,
        WaxingSlice = 24374,
        InfernalSlice = 24375,
        ShadowOfDeath = 24378,
        SoulSlice = 24380,

        // AoE
        SpinningScythe = 24376,
        NightmareScythe = 24377,
        WhorlOfDeath = 24379,
        SoulScythe = 24381,

        // Unveiled
        Gibbet = 24382,
        Gallows = 24383,
        Guillotine = 24384,
        UnveiledGibbet = 24390,
        UnveiledGallows = 24391,
        ExecutionersGibbet = 36970,
        ExecutionersGallows = 36971,
        ExecutionersGuillotine = 36972,

        // Reaver
        BloodStalk = 24389,
        GrimSwathe = 24392,
        Gluttony = 24393,

        // Sacrifice
        ArcaneCircle = 24405,
        PlentifulHarvest = 24385,

        // Enshroud
        Enshroud = 24394,
        Communio = 24398,
        LemuresSlice = 24399,
        LemuresScythe = 24400,
        VoidReaping = 24395,
        CrossReaping = 24396,
        GrimReaping = 24397,
        Sacrificium = 36969,
        Perfectio = 36973,

        // Miscellaneous
        HellsIngress = 24401,
        HellsEgress = 24402,
        Regress = 24403,
        ArcaneCrest = 24404,
        Harpe = 24386,
        Soulsow = 24387,
        HarvestMoon = 24388;

    public static class Buffs
    {
        public const ushort
            SoulReaver = 2587,
            ImmortalSacrifice = 2592,
            ArcaneCircle = 2599,
            EnhancedGibbet = 2588,
            EnhancedGallows = 2589,
            EnhancedVoidReaping = 2590,
            EnhancedCrossReaping = 2591,
            EnhancedHarpe = 2845,
            Enshrouded = 2593,
            Soulsow = 2594,
            Threshold = 2595,
            BloodsownCircle = 2972,
            IdealHost = 3905,
            Oblatio = 3857,
            Executioner = 3858,
            PerfectioParata = 3860;
    }

    public static class Debuffs
    {
        public const ushort
            DeathsDesign = 2586;
    }

    #endregion
}
