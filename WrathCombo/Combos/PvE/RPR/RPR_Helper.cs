using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.RPR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    #region SoD

    private static bool CanUseShadowOfDeath(int dotRefresh = 8, bool trashOnly = true, bool arcaneCircleEnabled = true)
    {
        if (LevelChecked(ShadowOfDeath) && !HasStatusEffect(Buffs.SoulReaver) &&
            !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.PerfectioParata) &&
            !HasStatusEffect(Buffs.ImmortalSacrifice) && !IsComboExpiring(3) &&
            CanApplyStatus(CurrentTarget, Debuffs.DeathsDesign) &&
            !JustUsed(ShadowOfDeath) && InActionRange(ShadowOfDeath))
        {
            if (trashOnly && !InBossEncounter() &&
                !HasStatusEffect(Buffs.Enshrouded) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= dotRefresh)
                return true;

            if (!trashOnly || InBossEncounter() || !arcaneCircleEnabled)
            {
                if (LevelChecked(PlentifulHarvest) && !HasStatusEffect(Buffs.Enshrouded) &&
                    UsesBurstAlignment && (AcCD.InRange(58f, 62f) || AcCD.InRange(28f, 32f)) &&
                    GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < 32)
                    return true;

                //Double enshroud
                if (LevelChecked(PlentifulHarvest) && HasStatusEffect(Buffs.Enshrouded) &&
                    AcCD <= GCD && GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < 32 &&
                    (JustUsed(VoidReaping, 2f) || JustUsed(CrossReaping, 2f)))
                    return true;

                //lvl 88+ general use
                if (LevelChecked(PlentifulHarvest) && !HasStatusEffect(Buffs.Enshrouded) &&
                    GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= dotRefresh &&
                    (AcCD > GCD * 8 || IsOffCooldown(ArcaneCircle)))
                    return true;

                //below lvl 88 use
                if (!LevelChecked(PlentifulHarvest) &&
                    GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= dotRefresh)
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
            ActionReady(HarvestMoon) && HasStatusEffect(Buffs.Soulsow))
            return HarvestMoon;

        if (IsPerfectioReady && InActionRange(PerfectioAction) &&
            (!InMeleeRange() || ShouldSpendPerfectioNow()))
            return PerfectioAction;

        if (useRangedFiller &&
            ActionReady(OriginalHook(Harpe)))
        {
            if (HasStatusEffect(Buffs.Enshrouded) && Lemure is 1 &&
                LevelChecked(Communio))
                return Communio;

            if (enhancedHarpeOnly && HasStatusEffect(Buffs.EnhancedHarpe) ||
                (!enhancedHarpeOnly || allowHarpeWhileMoving) &&
                (!IsMoving() || HasStatusEffect(Buffs.EnhancedHarpe)))
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
                ComboAction == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe))
                return OriginalHook(NightmareScythe);

            return OriginalHook(SpinningScythe);
        }

        if (ComboTimer > 0)
        {
            if (ComboAction == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                return OriginalHook(WaxingSlice);

            if (ComboAction == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
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
        !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
        !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
        !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.PerfectioParata);

    private static bool CanEnshroud(bool onAoE = false)
    {
        if (onAoE && IsComboExpiring(6))
            return false;

        if ((ActionReady(Enshroud) || HasStatusEffect(Buffs.IdealHost)) &&
            !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Executioner) && HasBattleTarget() &&
            !HasStatusEffect(Buffs.PerfectioParata) && !HasStatusEffect(Buffs.Enshrouded))
        {
            if (!LevelChecked(PlentifulHarvest))
                return true;

            if (HasStatusEffect(Buffs.ArcaneCircle))
                return true;

            if (LevelChecked(PlentifulHarvest) &&
                AcCD <= GCD + 1.5f)
                return true;

            if (LevelChecked(PlentifulHarvest) &&
                JustUsed(PlentifulHarvest, 5))
                return true;

            if (!HasStatusEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                AcCD.InRange(49, 66))
                return true;

            if (!HasStatusEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                Soul >= 90)
                return true;
        }

        return false;
    }

    private static bool IsShroudCapped =>
        Shroud >= MaxShroud;

    private static bool IsShroudOvercapping(bool enshroudEnabled = true, bool onAoE = false) =>
        IsShroudCapped && (!enshroudEnabled || !CanEnshroud(onAoE));

    #endregion

    #region Weaves

    private static bool CanArcaneCircleWeave(bool onAoE = false, int hpThreshold = 0) =>
        ActionReady(ArcaneCircle) && GetTargetHPPercent() > hpThreshold &&
        (onAoE || LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) || !LevelChecked(Enshroud));

    private static bool CanGluttonyWeave(bool enshroudEnabled = true) =>
        CanBurstGluttonyWeave(enshroudEnabled) ||
        !IsShroudOvercapping(enshroudEnabled) &&
        ActionReady(Gluttony) && InNormalRotation && !IsComboExpiring(3) &&
        !(InPostBurstSequence && Soul < 50);

    private static bool CanTrueNorthForGluttony(bool advanced = false, int tnChargePool = 0) =>
        !InPostBurstSequence &&
        LevelChecked(Gluttony) && GetCooldownRemainingTime(Gluttony) <= GCD && Role.CanTrueNorth() &&
        (!advanced || GetRemainingCharges(Role.TrueNorth) > tnChargePool);

    private static bool CanSoulOverflowWeave() =>
        !ShouldHoldSoulOverflowWeave &&
        InNormalRotation &&
        !IsComboExpiring(3);

    private static bool ShouldSpendSoulOvercapST(bool gluttonyEnabled)
    {
        if (!LevelChecked(Gluttony))
            return true;

        if (Soul is 100)
            return true;

        if (!gluttonyEnabled)
            return false;

        return IsOnCooldown(Gluttony) && GetCooldownRemainingTime(Gluttony) > GCD * 4;
    }

    private static bool ShouldSpendSoulOvercapAoE() =>
        !LevelChecked(Gluttony) ||
        Soul is 100 ||
        GetCooldownRemainingTime(Gluttony) > GCD * 5;

    private static bool CanBloodstalkWeave(bool gluttonyEnabled = true, bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled) &&
        CanSoulOverflowWeave() &&
        ActionReady(OriginalHook(BloodStalk)) &&
        ShouldSpendSoulOvercapST(gluttonyEnabled);

    private static bool CanGrimSwatheWeave(bool onAoE = false, bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled, onAoE) &&
        CanSoulOverflowWeave() &&
        ActionReady(GrimSwathe) &&
        InActionRange(onAoE ? OriginalHook(GrimSwathe) : GrimSwathe) &&
        ShouldSpendSoulOvercapAoE();

    private static bool CanSacrificiumWeave(
        bool onAoE = false,
        bool useArcaneCircleBoss = true,
        bool arcaneCircleEnabled = true,
        int arcaneCircleBossOption = 0) =>
        HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio) &&
        (onAoE
            ? Lemure is 2 && Void is 1
            : Lemure <= 4) &&
        (!useArcaneCircleBoss || onAoE ||
         GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 && !JustUsed(ArcaneCircle, 2) &&
         (arcaneCircleBossOption == 0 ||
          InBossEncounter() ||
          arcaneCircleBossOption == 1 && !InBossEncounter() && IsOffCooldown(ArcaneCircle)) ||
         !arcaneCircleEnabled);

    private static bool CanLemureWeave(bool onAoE = false) =>
        HasStatusEffect(Buffs.Enshrouded) && Void >= 2 &&
        LevelChecked(onAoE ? LemuresScythe : LemuresSlice) &&
        (!onAoE || InActionRange(OriginalHook(GrimSwathe)));

    private static bool UseEnshroudWeaves(out uint action, bool onAoE, bool sacrificium = true, bool lemure = true,
        bool useArcaneCircleBoss = true, bool arcaneCircleEnabled = true, int arcaneCircleBossOption = 0)
    {
        action = 0;

        if (!HasStatusEffect(Buffs.Enshrouded))
            return false;

        if (sacrificium && CanSacrificiumWeave(onAoE, useArcaneCircleBoss, arcaneCircleEnabled, arcaneCircleBossOption))
        {
            action = OriginalHook(Gluttony);
            return true;
        }

        if (lemure && CanLemureWeave(onAoE))
        {
            action = OriginalHook(onAoE ? GrimSwathe : BloodStalk);
            return true;
        }

        return false;
    }

    #endregion

    #region GCD Burst

    private static bool WithinGCD(uint actionId) =>
        LevelChecked(actionId) && (HasCharges(actionId) || GetCooldownRemainingTime(actionId) <= GCD);

    private static bool IsPerfectioReady =>
        HasStatusEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio);

    private static uint PerfectioAction =>
        WithinGCD(Perfectio) ? Perfectio : OriginalHook(Communio);

    private static bool ShouldSpendPerfectioNow() =>
        IsPerfectioReady;

    private static bool CanPerfectioGCD() =>
        IsPerfectioReady && ShouldSpendPerfectioNow() && InActionRange(PerfectioAction);

    private static bool InPostBurstSequence =>
        JustUsed(Perfectio, GCD * 8) ||
        JustUsed(OriginalHook(Communio), GCD * 2) && !IsPerfectioReady && !HasStatusEffect(Buffs.Enshrouded);

    private static bool HasBurstComboContinue() =>
        InPostBurstSequence &&
        !IsComboExpiring(2) &&
        ComboTimer > 0;

    private static bool CanBurstGluttonyWeave(bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled) &&
        InPostBurstSequence && Soul >= 50 && ActionReady(Gluttony) &&
        !HasBurstComboContinue();

    private static bool OvercapSoulSliceProtection(bool onAoE)
    {
        if (Soul >= 100)
            return false;

        uint action = onAoE ? SoulScythe : SoulSlice;
        if (!ActionReady(action))
            return false;

        if (GetRemainingCharges(action) >= GetMaxCharges(action))
            return true;

        return GetRemainingCharges(action) >= 1 &&
               GetCooldownChargeRemainingTime(action) <= GCD * 2;
    }

    private static bool CanBurstSoulSliceScythe(bool onAoE = false) =>
        InPostBurstSequence &&
        !HasBurstComboContinue() &&
        !JustUsed(onAoE ? SoulScythe : SoulSlice, GCD) &&
        (Soul <= 50 || OvercapSoulSliceProtection(onAoE)) &&
        (onAoE
            ? ActionReady(SoulScythe) && InActionRange(SoulScythe)
            : ActionReady(SoulSlice) && InActionRange(SoulSlice) && !IsComboExpiring(2));

    private static bool ShouldHoldSoulOverflowWeave =>
        Soul < 100 &&
        InPostBurstSequence && !JustUsed(Gluttony, GCD * 8);

    private static uint PostBurstGCD(bool onAoE, bool soulSliceEnabled = true)
    {
        if (!InPostBurstSequence)
            return 0;

        if (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner) ||
            HasStatusEffect(Buffs.ImmortalSacrifice))
            return 0;

        if (HasBurstComboContinue())
            return ContinueBasicCombo(onAoE);

        if (soulSliceEnabled && CanBurstSoulSliceScythe(onAoE))
            return onAoE ? SoulScythe : SoulSlice;

        return ContinueBasicCombo(onAoE);
    }

    private static bool HasImmortalSacrificeStacks =>
        HasStatusEffect(Buffs.ImmortalSacrifice) && GetStatusEffectStacks(Buffs.ImmortalSacrifice) > 0;

    private static bool CanPlentifulHarvest() =>
        !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
        !HasStatusEffect(Buffs.Executioner) && HasImmortalSacrificeStacks &&
        (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio));

    private static bool CanWhorlOfDeath(int refreshThreshold = 6, int hpThreshold = 0) =>
        LevelChecked(WhorlOfDeath) && InActionRange(WhorlOfDeath) &&
        CanApplyStatus(CurrentTarget, Debuffs.DeathsDesign) &&
        GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < refreshThreshold &&
        !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Executioner) &&
        GetTargetHPPercent() > hpThreshold;

    private static bool CanGuillotineGCD(bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled, true) &&
        (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) &&
        !HasStatusEffect(Buffs.Enshrouded) && LevelChecked(Guillotine) &&
        InActionRange(OriginalHook(Guillotine));

    private static bool CanGibbetGallowsGCD(bool enshroudEnabled = true) =>
        !IsShroudOvercapping(enshroudEnabled) &&
        LevelChecked(Gibbet) && !HasStatusEffect(Buffs.Enshrouded) &&
        (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner));

    private static uint GibbetGallowsAction(
        int positionalChoice = 1,
        bool useSimpleTrueNorth = true,
        bool useDynamicTrueNorth = false,
        int tnChargePool = 0,
        bool holdTnCharge = false)
    {
        bool neitherEnhanced = !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows);

        if (HasStatusEffect(Buffs.EnhancedGibbet) ||
            useSimpleTrueNorth && neitherEnhanced ||
            !useSimpleTrueNorth && positionalChoice is 1 && neitherEnhanced)
        {
            if (useSimpleTrueNorth && Role.CanTrueNorth() && !OnTargetsFlank())
                return Role.TrueNorth;

            if (useDynamicTrueNorth &&
                (holdTnCharge && GetRemainingCharges(Role.TrueNorth) is 2 || !holdTnCharge) &&
                Role.CanTrueNorth() && !OnTargetsFlank() &&
                GetRemainingCharges(Role.TrueNorth) > tnChargePool)
                return Role.TrueNorth;

            return OriginalHook(Gibbet);
        }

        if (HasStatusEffect(Buffs.EnhancedGallows) ||
            useSimpleTrueNorth && neitherEnhanced ||
            !useSimpleTrueNorth && positionalChoice is 0 && neitherEnhanced)
        {
            if (useSimpleTrueNorth && Role.CanTrueNorth() && !OnTargetsRear())
                return Role.TrueNorth;

            if (useDynamicTrueNorth &&
                (holdTnCharge && GetRemainingCharges(Role.TrueNorth) is 2 || !holdTnCharge) &&
                Role.CanTrueNorth() && !OnTargetsRear() &&
                GetRemainingCharges(Role.TrueNorth) > tnChargePool)
                return Role.TrueNorth;

            return OriginalHook(Gallows);
        }

        return 0;
    }

    private static uint EnshroudComboGCD(bool onAoE, bool communio = true, bool reaping = true)
    {
        if (!HasStatusEffect(Buffs.Enshrouded))
            return 0;

        if (onAoE)
        {
            if (communio && LevelChecked(Communio) && Lemure is 1 && Void is 0)
                return Communio;

            if (reaping && Lemure > 0 && InActionRange(OriginalHook(Guillotine)))
                return OriginalHook(Guillotine);

            return 0;
        }

        if (communio && Lemure is 1 && LevelChecked(Communio))
            return Communio;

        if (reaping && HasStatusEffect(Buffs.EnhancedVoidReaping))
            return OriginalHook(Gibbet);

        if (reaping &&
            (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
             !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping)))
            return OriginalHook(Gallows);

        return 0;
    }

    private static uint BloodStalkGrimSwatheEnshroudGCD(uint actionId)
    {
        if (actionId is GrimSwathe)
        {
            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (!HasStatusEffect(Buffs.Enshrouded))
                return 0;

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
        else if (actionId is BloodStalk)
        {
            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (!HasStatusEffect(Buffs.Enshrouded))
                return 0;

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

        return 0;
    }

    private static uint BloodStalkGrimSwatheSoulReaverGCD(uint actionId, bool enshroudEnabled = true)
    {
        if (IsShroudOvercapping(enshroudEnabled, actionId is GrimSwathe))
            return 0;

        if (actionId is GrimSwathe &&
            (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) &&
            LevelChecked(Guillotine))
            return Guillotine;

        if (actionId is BloodStalk &&
            (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
        {
            if (HasStatusEffect(Buffs.EnhancedGibbet))
                return OriginalHook(Gibbet);

            if (HasStatusEffect(Buffs.EnhancedGallows) ||
                !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                return OriginalHook(Gallows);
        }

        return 0;
    }

    private static bool CanSoulSliceScythe(bool onAoE) =>
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
            var _ => false
        };
    }

    #endregion

    #region Misc

    private static bool CanUseArcaneCrest =>
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

        return HasStatusEffect(Debuffs.DeathsDesign, CurrentTarget) && GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < gcd;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpenerLvl100.LevelChecked &&
            RPR_SelectedOpener == 0)
            return StandardOpenerLvl100;

        if (FirstGcdBuffsOpenerLvl100.LevelChecked &&
            RPR_SelectedOpener == 1)
            return FirstGcdBuffsOpenerLvl100;

        if (StandardOpenerLvl90.LevelChecked)
            return StandardOpenerLvl90;

        return WrathOpener.Dummy;
    }

    internal static RPRStandardOpenerLvl100 StandardOpenerLvl100 = new();

    internal static RPRFirstGcdBuffsOpenerLvl100 FirstGcdBuffsOpenerLvl100 = new();

    internal static RPRStandardOpenerLvl90 StandardOpenerLvl90 = new();

    internal class RPRStandardOpenerLvl100 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            Harpe,
            ShadowOfDeath,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)),
            SoulSlice,
            ArcaneCircle,
            Gluttony,
            ExecutionersGibbet, //7
            ExecutionersGallows, //8
            SoulSlice,
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            Sacrificium,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            Perfectio,
            UnveiledGibbet, //21
            Gibbet, //22
            ShadowOfDeath,
            Slice
        ];

        public override Preset Preset => Preset.RPR_ST_Opener;

        internal override UserData ContentCheckConfig => RPR_Balance_Content;

        internal override bool IncludePot => RPR_Opener_Potion;

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([7], ExecutionersGallows, OnTargetsRear),
            ([8], ExecutionersGibbet, () => HasStatusEffect(Buffs.EnhancedGibbet)),
            ([21], UnveiledGallows, () => HasStatusEffect(Buffs.EnhancedGallows)),
            ([22], Gallows, () => HasStatusEffect(Buffs.EnhancedGallows))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => InMeleeRange())
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [3];


        public override bool HasCooldowns() =>
            GetRemainingCharges(SoulSlice) is 2 &&
            IsOffCooldown(ArcaneCircle) &&
            IsOffCooldown(Gluttony) &&
            Void is 0 &&
            Soul is 0;
    }

    internal class RPRFirstGcdBuffsOpenerLvl100 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            SoulSlice,
            ArcaneCircle,
            ShadowOfDeath,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)),
            Gluttony,
            ExecutionersGibbet, //6
            ExecutionersGallows, //7
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            Sacrificium,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            Perfectio,
            SoulSlice,
            UnveiledGibbet, //20
            Gibbet, //21
            ShadowOfDeath,
            Slice
        ];

        public override Preset Preset => Preset.RPR_ST_Opener;

        internal override UserData ContentCheckConfig => RPR_Balance_Content;

        internal override bool IncludePot => RPR_Opener_Potion;

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([6], ExecutionersGallows, OnTargetsRear),
            ([7], ExecutionersGibbet, () => HasStatusEffect(Buffs.EnhancedGibbet)),
            ([20], UnveiledGallows, () => HasStatusEffect(Buffs.EnhancedGallows)),
            ([21], Gallows, () => HasStatusEffect(Buffs.EnhancedGallows))
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [4];

        public override bool HasCooldowns() =>
            GetRemainingCharges(SoulSlice) is 2 &&
            IsOffCooldown(ArcaneCircle) &&
            IsOffCooldown(Gluttony) &&
            Void is 0 &&
            Soul is 0;
    }

    internal class RPRStandardOpenerLvl90 : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            Harpe,
            ShadowOfDeath,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)),
            ArcaneCircle,
            SoulSlice,
            SoulSlice,
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            HarvestMoon,
            Gluttony,
            Gibbet, //17
            Gallows, //18
            UnveiledGibbet, //19
            Gibbet //20
        ];

        public override Preset Preset => Preset.RPR_ST_Opener;

        internal override UserData ContentCheckConfig => RPR_Balance_Content;

        internal override bool IncludePot => RPR_Opener_Potion;

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([17], Gallows, OnTargetsRear),
            ([18], Gibbet, () => HasStatusEffect(Buffs.EnhancedGibbet)),
            ([19], UnveiledGallows, () => HasStatusEffect(Buffs.EnhancedGallows)),
            ([20], Gallows, () => HasStatusEffect(Buffs.EnhancedGallows))
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [3];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => InMeleeRange())
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(SoulSlice) is 2 &&
            IsOffCooldown(ArcaneCircle) &&
            IsOffCooldown(Gluttony) &&
            Void is 0 &&
            Soul is 0;
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
