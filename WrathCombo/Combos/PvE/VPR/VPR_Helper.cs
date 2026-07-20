using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager;
using static WrathCombo.Combos.PvE.VPR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    #region Basic Combo

    private static bool IsTrueNorthReady(bool useTrueNorth, int trueNorthCharges = 0, bool dynamicHoldCharge = false) =>
        useTrueNorth &&
        (dynamicHoldCharge && GetRemainingCharges(Role.TrueNorth) is 2 ||
         !dynamicHoldCharge) &&
        GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
        Role.CanTrueNorth();

    private static uint DoBasicCombo(bool useTrueNorth = false, bool onAoE = false, int trueNorthCharges = 0,
        bool dynamicHoldCharge = false)
    {
        if (onAoE)
        {
            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is ReavingMaw or SteelMaw)
                {
                    if (LevelChecked(HuntersBite) &&
                        HasStatusEffect(Buffs.GrimhuntersVenom))
                        return OriginalHook(SteelMaw);

                    if (LevelChecked(SwiftskinsBite) &&
                        (HasStatusEffect(Buffs.GrimskinsVenom) ||
                         !HasStatusEffect(Buffs.Swiftscaled) && !HasStatusEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is HuntersBite or SwiftskinsBite)
                {
                    if (HasStatusEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                        return OriginalHook(SteelMaw);

                    if (HasStatusEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is BloodiedMaw or JaggedMaw)
                    return LevelChecked(ReavingMaw) && HasStatusEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingMaw)
                        : OriginalHook(SteelMaw);
            }

            //for lower lvls
            if (LevelChecked(ReavingMaw) &&
                (HasStatusEffect(Buffs.HonedReavers) ||
                 !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingMaw);

            return OriginalHook(SteelMaw);
        }

        //1-2-3 (4-5-6) Combo
        if (ComboTimer > 0)
        {
            if (ComboAction is ReavingFangs or SteelFangs)
            {
                if (LevelChecked(SwiftskinsSting) &&
                    (HasHindVenom || IsMissingSwiftscaled || IsMissingBasicComboVenom))
                    return OriginalHook(ReavingFangs);

                if (LevelChecked(HuntersSting) &&
                    (HasFlankVenom || IsMissingHuntersInstinct))
                    return OriginalHook(SteelFangs);
            }

            if (ComboAction is HuntersSting or SwiftskinsSting)
            {
                if ((HasStatusEffect(Buffs.FlanksbaneVenom) || HasStatusEffect(Buffs.HindsbaneVenom)) &&
                    LevelChecked(HindstingStrike))
                    return IsTrueNorthReady(useTrueNorth, trueNorthCharges, dynamicHoldCharge) &&
                           (!OnTargetsRear() && HasStatusEffect(Buffs.HindsbaneVenom) ||
                            !OnTargetsFlank() && HasStatusEffect(Buffs.FlanksbaneVenom))
                        ? Role.TrueNorth
                        : OriginalHook(ReavingFangs);

                if ((HasStatusEffect(Buffs.FlankstungVenom) || HasStatusEffect(Buffs.HindstungVenom)) &&
                    LevelChecked(FlanksbaneFang))
                    return IsTrueNorthReady(useTrueNorth, trueNorthCharges, dynamicHoldCharge) &&
                           (!OnTargetsRear() && HasStatusEffect(Buffs.HindstungVenom) ||
                            !OnTargetsFlank() && HasStatusEffect(Buffs.FlankstungVenom))
                        ? Role.TrueNorth
                        : OriginalHook(SteelFangs);
            }

            if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                return LevelChecked(ReavingFangs) && HasStatusEffect(Buffs.HonedReavers)
                    ? OriginalHook(ReavingFangs)
                    : OriginalHook(SteelFangs);
        }

        //LowLevels
        if (LevelChecked(ReavingFangs) &&
            (HasStatusEffect(Buffs.HonedReavers) ||
             !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
            return OriginalHook(ReavingFangs);

        return OriginalHook(SteelFangs);
    }

    #endregion

    #region Misc

    private static float IreCD =>
        GetCooldownRemainingTime(SerpentsIre);

    private static bool IsCoilsCapped =>
        TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoilStacks > 2 ||
        !TraitLevelChecked(Traits.EnhancedVipersRattle) && RattlingCoilStacks > 1;

    private static bool HasRattlingCoilStacks =>
        RattlingCoilStacks > 0;

    private static bool HasHindVenom =>
        HasStatusEffect(Buffs.HindstungVenom) ||
        HasStatusEffect(Buffs.HindsbaneVenom);

    private static bool HasFlankVenom =>
        HasStatusEffect(Buffs.FlankstungVenom) ||
        HasStatusEffect(Buffs.FlanksbaneVenom);

    private static bool IsMissingSwiftscaled =>
        !HasStatusEffect(Buffs.Swiftscaled);

    private static bool IsMissingHuntersInstinct =>
        !HasStatusEffect(Buffs.HuntersInstinct);

    private static bool IsMissingBasicComboVenom =>
        !HasStatusEffect(Buffs.FlanksbaneVenom) &&
        !HasStatusEffect(Buffs.FlankstungVenom) &&
        !HasStatusEffect(Buffs.HindsbaneVenom) &&
        !HasStatusEffect(Buffs.HindstungVenom);

    private static bool IsSTComboWeaveBlocked =>
        !HasStatusEffect(Buffs.HuntersVenom) &&
        !HasStatusEffect(Buffs.SwiftskinsVenom) &&
        !HasStatusEffect(Buffs.PoisedForTwinblood) &&
        !HasStatusEffect(Buffs.PoisedForTwinfang);

    private static bool IsAoEComboWeaveBlocked =>
        !HasStatusEffect(Buffs.FellhuntersVenom) &&
        !HasStatusEffect(Buffs.FellskinsVenom) &&
        !HasStatusEffect(Buffs.PoisedForTwinblood) &&
        !HasStatusEffect(Buffs.PoisedForTwinfang);

    private static bool HasBothBuffs =>
        HasStatusEffect(Buffs.Swiftscaled) &&
        HasStatusEffect(Buffs.HuntersInstinct);

    private static int BossHpThreshold(int hpBossOption, int hpOption, bool isBoss) =>
        hpBossOption == 1 || !isBoss ? hpOption : 0;

    private static int SerpentsIreHPThreshold =>
        BossHpThreshold(VPR_ST_SerpentsIreHPBossOption, VPR_ST_SerpentsIreHPOption, InBossEncounter());

    private const float IreDualWieldWindow = 10f;

    private const float IreOfferingSaveWindow = 15f;

    private static bool UsesBurstAlignment =>
        InBossEncounter();

    private static bool ShouldHoldTwinbladeForIre =>
        UsesBurstAlignment && LevelChecked(SerpentsIre) && IreCD > 0 && IreCD <= IreDualWieldWindow;

    private static bool InTwinbladeCombo =>
        UsedVicewinder || UsedHuntersCoil || UsedSwiftskinsCoil ||
        UsedVicepit || UsedHuntersDen || UsedSwiftskinsDen;

    private static bool ShouldHoldNewTwinblade =>
        ShouldHoldTwinbladeForIre && !InTwinbladeCombo && !IsEmpowermentExpiring(4);

    private static bool ShouldSaveOfferingForBurst =>
        UsesBurstAlignment &&
        (HasStatusEffect(Buffs.ReadyToReawaken) || IreCD > 0 && IreCD <= IreOfferingSaveWindow);

    #endregion

    #region Reawaken

    private static bool CanReawaken(
        bool onAoE = false,
        int hpThresholdUsage = 0,
        int hpThresholdDontSave = 5,
        int hpThresholdUsageAoE = 40)
    {
        if (onAoE)
        {
            if (!ActionReady(Reawaken) || GetTargetHPPercent() <= hpThresholdUsageAoE ||
                !HasStatusEffect(Buffs.Swiftscaled) || !HasStatusEffect(Buffs.HuntersInstinct) ||
                HasStatusEffect(Buffs.Reawakened) || !IsAoEComboWeaveBlocked)
                return false;

            if (UsesBurstAlignment && JustUsed(Ouroboros, GCD * 12) && SerpentOffering >= 50)
                return true;

            return HasStatusEffect(Buffs.ReadyToReawaken) || SerpentOffering >= 50;
        }

        if (!(ActionReady(Reawaken) && !HasStatusEffect(Buffs.Reawakened) &&
              InActionRange(Reawaken) && IsSTComboWeaveBlocked && HasBattleTarget() &&
              !IsEmpowermentExpiring(6) && !IsComboExpiring(6) &&
              GetTargetHPPercent() > hpThresholdUsage))
            return false;

        if (TargetIsBoss() &&
            GetTargetHPPercent() < hpThresholdDontSave)
            return true;

        if (!JustUsed(SerpentsIre, GCD) && HasStatusEffect(Buffs.ReadyToReawaken))
            return true;

        if (UsesBurstAlignment && JustUsed(Ouroboros, GCD * 12) && SerpentOffering >= 50)
            return true;

        if (SerpentOffering >= 100)
            return true;

        if (!InBossEncounter())
            return true;

        if (!LevelChecked(Ouroboros) && JustUsed(FourthGeneration))
            return true;

        if (IreCD is >= 50 and <= 62 &&
            SerpentOffering >= 50 &&
            (!UsesBurstAlignment || !ShouldSaveOfferingForBurst))
            return true;

        return false;
    }

    private const DreadCombo ReawakenFirstGeneration = (DreadCombo)7;
    private const DreadCombo ReawakenSecondGeneration = (DreadCombo)8;
    private const DreadCombo ReawakenThirdGeneration = (DreadCombo)9;
    private const DreadCombo ReawakenFourthGeneration = (DreadCombo)10;

    private static uint ReawakenCombo(uint actionId) =>
        DreadCombo switch
        {
            ReawakenFirstGeneration => FirstGeneration,
            ReawakenSecondGeneration => SecondGeneration,
            ReawakenThirdGeneration => ThirdGeneration,
            ReawakenFourthGeneration => FourthGeneration,
            0 => Ouroboros,
            var _ => actionId
        };

    private static int ReawakenHPThreshold()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? VPR_ST_ReawakenBossHPOption : VPR_ST_ReawakenBossAddsHPOption;

        return VPR_ST_ReawakenTrashHPOption;
    }

   #endregion

    #region Combos

    private static float GCD => GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;

    private static bool IsHoningExpiring(float times)
    {
        float gcd = GCD * times;

        return HasStatusEffect(Buffs.HonedSteel) && GetStatusEffectRemainingTime(Buffs.HonedSteel) < gcd ||
               HasStatusEffect(Buffs.HonedReavers) && GetStatusEffectRemainingTime(Buffs.HonedReavers) < gcd;
    }

    private static bool IsVenomExpiring(float times)
    {
        float gcd = GCD * times;

        return HasStatusEffect(Buffs.FlankstungVenom) && GetStatusEffectRemainingTime(Buffs.FlankstungVenom) < gcd ||
               HasStatusEffect(Buffs.FlanksbaneVenom) && GetStatusEffectRemainingTime(Buffs.FlanksbaneVenom) < gcd ||
               HasStatusEffect(Buffs.HindstungVenom) && GetStatusEffectRemainingTime(Buffs.HindstungVenom) < gcd ||
               HasStatusEffect(Buffs.HindsbaneVenom) && GetStatusEffectRemainingTime(Buffs.HindsbaneVenom) < gcd;
    }

    private static bool IsEmpowermentExpiring(float times)
    {
        float gcd = GCD * times;

        return GetStatusEffectRemainingTime(Buffs.Swiftscaled) < gcd || GetStatusEffectRemainingTime(Buffs.HuntersInstinct) < gcd;
    }

    private static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GCD * times;

        return Instance()->Combo.Timer != 0 && Instance()->Combo.Timer < gcd;
    }

    private static bool WithinGCD(uint actionId) =>
        LevelChecked(actionId) && (HasCharges(actionId) || GetCooldownRemainingTime(actionId) <= GCD);

    #endregion

    #region Weaves

    private static bool UseSerpentsTailWeave(bool onAoE, bool allowDeathRattle, bool allowLegacy) =>
        LevelChecked(SerpentsTail) &&
        ((allowDeathRattle &&
          (onAoE
              ? IsLastLashWeave && InActionRange(LastLash)
              : IsDeathRattleWeave && InActionRange(DeathRattle))) ||
         allowLegacy && IsLegacyWeaveReady && InActionRange(FirstLegacy));

    private static bool UsePoisedTwinWeaves(out uint action, bool enabled = true)
    {
        action = 0;

        if (!enabled)
            return false;

        if (HasStatusEffect(Buffs.PoisedForTwinfang))
        {
            action = OriginalHook(Twinfang);
            return true;
        }

        if (HasStatusEffect(Buffs.PoisedForTwinblood))
        {
            action = OriginalHook(Twinblood);
            return true;
        }

        return false;
    }

    private static bool UseViceTwinWeaves(out uint action, bool onAoE, bool enabled, bool requireMelee = true,
        bool ignoreRange = false)
    {
        action = 0;

        if (!enabled || HasStatusEffect(Buffs.Reawakened))
            return false;

        if (onAoE)
        {
            if (HasStatusEffect(Buffs.FellhuntersVenom) &&
                (ignoreRange || InActionRange(TwinfangThresh)))
            {
                action = OriginalHook(Twinfang);
                return true;
            }

            if (HasStatusEffect(Buffs.FellskinsVenom) &&
                (ignoreRange || InActionRange(TwinbloodThresh)))
            {
                action = OriginalHook(Twinblood);
                return true;
            }

            return false;
        }

        if (requireMelee && !InMeleeRange())
            return false;

        if (HasStatusEffect(Buffs.HuntersVenom))
        {
            action = OriginalHook(Twinfang);
            return true;
        }

        if (HasStatusEffect(Buffs.SwiftskinsVenom))
        {
            action = OriginalHook(Twinblood);
            return true;
        }

        return false;
    }

    private static bool CanSerpentsIre(int hpThreshold = 0) =>
        InCombat() && !IsCoilsCapped && ActionReady(SerpentsIre) &&
        GetTargetHPPercent() > hpThreshold;

    private static bool ShouldSpendCoilStacks(int holdCharges, int hpThreshold) =>
        RattlingCoilStacks > holdCharges ||
        GetTargetHPPercent() < hpThreshold && HasRattlingCoilStacks;

    private static bool CanUseUncoiledFuryInRotation(bool onAoE) =>
        !ShouldHoldNewTwinblade &&
        HasBothBuffs &&
        !HasStatusEffect(Buffs.Reawakened) && !HasStatusEffect(Buffs.ReadyToReawaken) &&
        !JustUsed(Ouroboros) &&
        (onAoE
            ? !UsedVicepit && !UsedHuntersDen && !UsedSwiftskinsDen && IsAoEComboWeaveBlocked &&
              !JustUsed(JaggedMaw, GCD) && !JustUsed(BloodiedMaw, GCD) && !JustUsed(SerpentsIre, GCD)
            : !UsedVicewinder && !UsedHuntersCoil && !UsedSwiftskinsCoil && IsSTComboWeaveBlocked &&
              !IsComboExpiring(2) && !IsVenomExpiring(2) && !IsHoningExpiring(2) && !IsEmpowermentExpiring(3));

    private static bool OvercapUncoiledFuryProtection(bool onAoE) =>
        IsCoilsCapped &&
        ActionReady(UncoiledFury) &&
        InActionRange(UncoiledFury) &&
        !HasStatusEffect(Buffs.Reawakened) &&
        (onAoE ? IsAoEComboWeaveBlocked : IsSTComboWeaveBlocked) &&
        (LevelChecked(SerpentsIre) && IreCD <= GCD * (onAoE ? 2 : 3) ||
         HasCharges(onAoE ? Vicepit : Vicewinder));

    private static bool CanUseUncoiledFury(
        bool onAoE = false,
        int stHoldCharges = 1,
        int stHpThreshold = 1,
        int aoeHoldCharges = 1,
        int aoeHpThreshold = 1)
    {
        if (!ActionReady(UncoiledFury) || !InActionRange(UncoiledFury))
            return false;

        if (!onAoE && HasRattlingCoilStacks && !InMeleeRange() && HasBattleTarget())
            return true;

        if (!CanUseUncoiledFuryInRotation(onAoE))
            return false;

        return ShouldSpendCoilStacks(onAoE ? aoeHoldCharges : stHoldCharges,
            onAoE ? aoeHpThreshold : stHpThreshold);
    }

    private static bool UseVicepitCombo(ref uint actionId, bool ignoreRange = false)
    {
        if (HasStatusEffect(Buffs.Reawakened))
            return false;

        if (UsedSwiftskinsDen &&
            (ignoreRange || InActionRange(HuntersDen)))
        {
            actionId = HuntersDen;
            return true;
        }

        if (UsedVicepit &&
            (ignoreRange || InActionRange(SwiftskinsDen)))
        {
            actionId = SwiftskinsDen;
            return true;
        }

        return false;
    }

    private static bool CanVicepit(bool ignoreRange = false) =>
        WithinGCD(Vicepit) && !HasStatusEffect(Buffs.Reawakened) && !JustUsed(Vicepit) &&
        !ShouldHoldNewTwinblade &&
        (ignoreRange || InActionRange(Vicepit)) &&
        (!HasBothBuffs || IreCD >= GCD * 4 || !LevelChecked(SerpentsIre));

    private static uint UseVicewinder(
        bool useSimpleTrueNorth = true,
        bool useDynamicTrueNorth = false,
        int trueNorthCharges = 0)
    {
        if (useSimpleTrueNorth)
            return Role.CanTrueNorth() ? Role.TrueNorth : Vicewinder;

        return VPR_TrueNorthVicewinder &&
               (useDynamicTrueNorth && GetRemainingCharges(Role.TrueNorth) > trueNorthCharges ||
                HasCharges(Role.TrueNorth)) &&
               Role.CanTrueNorth()
            ? Role.TrueNorth
            : Vicewinder;
    }

    private static uint UseCombo(
        uint actionId,
        bool onAoE,
        bool useReawakenCombo,
        bool useTrueNorth = false,
        int trueNorthCharges = 0,
        bool dynamicHoldCharge = false) =>
        useReawakenCombo && HasStatusEffect(Buffs.Reawakened)
            ? ReawakenCombo(actionId)
            : DoBasicCombo(useTrueNorth, onAoE, trueNorthCharges, dynamicHoldCharge);

    #endregion

    #region Vicewinder & Uncoiled Fury Combo

    private static bool CanUseVicewinder =>
        WithinGCD(Vicewinder) && InActionRange(Vicewinder) && InCombat() &&
        !ShouldHoldNewTwinblade &&
        !IsComboExpiring(6) && !IsVenomExpiring(4) && !IsHoningExpiring(4) &&
        !UsedVicewinder && !UsedHuntersCoil && !UsedSwiftskinsCoil &&
        !JustUsed(SerpentsIre, GCD * 4) && !JustUsed(Vicewinder) &&
        !JustUsed(Ouroboros) && !HasStatusEffect(Buffs.Reawakened) &&
        (!HasBothBuffs ||
         IsEmpowermentExpiring(4) ||
         IreCD >= GCD * 3 && InBossEncounter() || !InBossEncounter() || !LevelChecked(SerpentsIre));

    private static bool CanVicewinderCombo(ref uint actionId, bool vicewinderBuffPrio = false)
    {
        if ((UsedVicewinder || UsedSwiftskinsCoil || UsedHuntersCoil) &&
            LevelChecked(Vicewinder) && InActionRange(Vicewinder) &&
            !HasStatusEffect(Buffs.Reawakened))
        {
            if (UsedVicewinder &&
                (!HasStatusEffect(Buffs.Swiftscaled) ||
                 HasBothBuffs && (!OnTargetsFlank() || !TargetNeedsPositionals()) ||
                 vicewinderBuffPrio && GetStatusEffectRemainingTime(Buffs.Swiftscaled) < GCD * 6) ||
                UsedHuntersCoil)
            {
                actionId = SwiftskinsCoil;
                return true;
            }

            if (UsedVicewinder &&
                (!HasStatusEffect(Buffs.HuntersInstinct) ||
                 HasBothBuffs && (!OnTargetsRear() || !TargetNeedsPositionals()) ||
                 vicewinderBuffPrio && GetStatusEffectRemainingTime(Buffs.HuntersInstinct) < GCD * 6) ||
                UsedSwiftskinsCoil)
            {
                actionId = HuntersCoil;
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked && VPR_OpenerSelection == 0)
            return StandardOpener;

        if (EarlyBuffOpener.LevelChecked && VPR_OpenerSelection == 1)
            return EarlyBuffOpener;


        return WrathOpener.Dummy;
    }

    internal static VPRStandardOpener StandardOpener = new();
    internal static VPREarlyBuffOpener EarlyBuffOpener = new();

    internal class VPRStandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            ReavingFangs,
            SerpentsIre,
            SwiftskinsSting,
            Vicewinder,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            HuntersCoil,
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite,
            Reawaken,
            FirstGeneration,
            FirstLegacy,
            SecondGeneration,
            SecondLegacy,
            ThirdGeneration,
            ThirdLegacy,
            FourthGeneration,
            FourthLegacy,
            Ouroboros,
            UncoiledFury, //22
            UncoiledTwinfang, //23
            UncoiledTwinblood, //24
            UncoiledFury, //25
            UncoiledTwinfang, //26
            UncoiledTwinblood, //27
            HindstingStrike, //28
            DeathRattle, //29
            Vicewinder, //30
            HuntersCoil, //31
            TwinfangBite, //32
            TwinbloodBite, //33
            SwiftskinsCoil, //34
            TwinbloodBite, //35
            TwinfangBite //36
        ];

        public override Preset Preset => Preset.VPR_ST_Opener;

        internal override UserData ContentCheckConfig => VPR_Balance_Content;
        internal override bool IncludePot => VPR_Opener_Potion;

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([31], SwiftskinsCoil, OnTargetsRear),
            ([32], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom)),
            ([33], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([34], HuntersCoil, () => UsedSwiftskinsCoil),
            ([35], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([36], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([22, 23, 24, 25, 26, 27], () => VPR_Opener_ExcludeUF || !HasCharges(RattlingCoil)),
            ([28], () => ComboAction is not SwiftskinsSting),
            ([29], () => !IsDeathRattleWeave && !JustUsed(HindstingStrike))
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [5];

        public override bool HasCooldowns() =>
            IsOriginal(ReavingFangs) &&
            GetRemainingCharges(Vicewinder) is 2 &&
            IsOffCooldown(SerpentsIre);
    }

    internal class VPREarlyBuffOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Vicewinder,
            SerpentsIre,
            HuntersCoil,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            TwinfangBite,
            TwinbloodBite,
            SwiftskinsCoil,
            TwinbloodBite,
            TwinfangBite,
            Reawaken,
            FirstGeneration,
            FirstLegacy,
            SecondGeneration,
            SecondLegacy,
            ThirdGeneration,
            ThirdLegacy,
            FourthGeneration,
            FourthLegacy,
            Ouroboros,
            UncoiledFury, //20
            UncoiledTwinfang, //21
            UncoiledTwinblood, //22
            Vicewinder,
            HuntersCoil, //24
            TwinfangBite, //25
            TwinbloodBite, //26
            SwiftskinsCoil, //27
            TwinbloodBite, //28
            TwinfangBite, //29
            UncoiledFury, //30
            UncoiledTwinfang, //31
            UncoiledTwinblood, //32
            UncoiledFury, //33
            UncoiledTwinfang, //34
            UncoiledTwinblood //35
        ];

        public override Preset Preset => Preset.VPR_ST_Opener;

        internal override UserData ContentCheckConfig => VPR_Balance_Content;
        internal override bool IncludePot => VPR_Opener_Potion;

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([24], SwiftskinsCoil, OnTargetsRear),
            ([25], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom)),
            ([26], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([27], HuntersCoil, () => UsedSwiftskinsCoil),
            ([28], TwinfangBite, () => HasStatusEffect(Buffs.HuntersVenom)),
            ([29], TwinbloodBite, () => HasStatusEffect(Buffs.SwiftskinsVenom))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([20, 21, 22, 30, 31, 32, 33, 34, 35], () => VPR_Opener_ExcludeUF || !HasCharges(RattlingCoil))
        ];

        public override List<int> DelayedWeaveSteps { get; set; } = [4];

        public override bool HasCooldowns() =>
            IsOriginal(ReavingFangs) &&
            GetRemainingCharges(Vicewinder) is 2 &&
            IsOffCooldown(SerpentsIre);
    }

    #endregion

    #region Gauge

    private static VPRGauge Gauge => GetJobGauge<VPRGauge>();

    private static byte RattlingCoilStacks => Gauge.RattlingCoilStacks;

    private static byte SerpentOffering => Gauge.SerpentOffering;

    private static DreadCombo DreadCombo => Gauge.DreadCombo;

    private static bool UsedVicewinder => DreadCombo is DreadCombo.Dreadwinder;

    private static bool UsedHuntersCoil => DreadCombo is DreadCombo.HuntersCoil;

    private static bool UsedSwiftskinsCoil => DreadCombo is DreadCombo.SwiftskinsCoil;

    private static bool UsedVicepit => DreadCombo is DreadCombo.PitOfDread;

    private static bool UsedSwiftskinsDen => DreadCombo is DreadCombo.SwiftskinsDen;

    private static bool UsedHuntersDen => DreadCombo is DreadCombo.HuntersDen;

    private static SerpentCombo SerpentCombo => Gauge.SerpentCombo;

    private static bool IsLegacyWeaveReady =>
        HasStatusEffect(Buffs.Reawakened) &&
        (SerpentCombo.HasFlag(SerpentCombo.FirstLegacy) ||
         SerpentCombo.HasFlag(SerpentCombo.SecondLegacy) ||
         SerpentCombo.HasFlag(SerpentCombo.ThirdLegacy) ||
         SerpentCombo.HasFlag(SerpentCombo.FourthLegacy));

    private static bool IsDeathRattleWeave => Gauge.SerpentCombo is SerpentCombo.DeathRattle;

    private static bool IsLastLashWeave => Gauge.SerpentCombo is SerpentCombo.LastLash;

    #endregion

    #region ID's

    public const uint
        ReavingFangs = 34607,
        ReavingMaw = 34615,
        Vicewinder = 34620,
        HuntersCoil = 34621,
        HuntersDen = 34624,
        HuntersSnap = 39166,
        Vicepit = 34623,
        RattlingCoil = 39189,
        Reawaken = 34626,
        SerpentsIre = 34647,
        SerpentsTail = 35920,
        Slither = 34646,
        SteelFangs = 34606,
        SteelMaw = 34614,
        SwiftskinsCoil = 34622,
        SwiftskinsDen = 34625,
        Twinblood = 35922,
        Twinfang = 35921,
        UncoiledFury = 34633,
        WrithingSnap = 34632,
        SwiftskinsSting = 34609,
        TwinfangBite = 34636,
        TwinbloodBite = 34637,
        UncoiledTwinfang = 34644,
        UncoiledTwinblood = 34645,
        HindstingStrike = 34612,
        DeathRattle = 34634,
        HuntersSting = 34608,
        HindsbaneFang = 34613,
        FlankstingStrike = 34610,
        FlanksbaneFang = 34611,
        HuntersBite = 34616,
        JaggedMaw = 34618,
        SwiftskinsBite = 34617,
        BloodiedMaw = 34619,
        FirstGeneration = 34627,
        FirstLegacy = 34640,
        SecondGeneration = 34628,
        SecondLegacy = 34641,
        ThirdGeneration = 34629,
        ThirdLegacy = 34642,
        FourthGeneration = 34630,
        FourthLegacy = 34643,
        Ouroboros = 34631,
        LastLash = 34635,
        TwinfangThresh = 34638,
        TwinbloodThresh = 34639;

    public static class Buffs
    {
        public const ushort
            FellhuntersVenom = 3659,
            FellskinsVenom = 3660,
            FlanksbaneVenom = 3646,
            FlankstungVenom = 3645,
            HindstungVenom = 3647,
            HindsbaneVenom = 3648,
            GrimhuntersVenom = 3649,
            GrimskinsVenom = 3650,
            HuntersVenom = 3657,
            SwiftskinsVenom = 3658,
            HuntersInstinct = 3668,
            Swiftscaled = 3669,
            Reawakened = 3670,
            ReadyToReawaken = 3671,
            PoisedForTwinfang = 3665,
            PoisedForTwinblood = 3666,
            HonedReavers = 3772,
            HonedSteel = 3672;
    }

    public static class Debuffs
    {
    }

    public static class Traits
    {
        public const uint
            EnhancedVipersRattle = 530,
            EnhancedSerpentsLineage = 533,
            SerpentsLegacy = 534;
    }

    #endregion
}
