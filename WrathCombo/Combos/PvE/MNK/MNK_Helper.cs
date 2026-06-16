using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.MNK.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

using static MNKExtensions;

internal partial class MNK
{
    #region PB Combo

    private static bool DoPerfectBalanceCombo(ref uint actionID, bool onAoE = false)
    {
        if (!HasStatusEffect(Buffs.PerfectBalance))
            return false;

        if (onAoE)
        {
            // Open Lunar
            if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
            {
                actionID = LevelChecked(ShadowOfTheDestroyer) ? ShadowOfTheDestroyer : Rockbreaker;
                return true;
            }

            // Open Solar
            if (!SolarNadi && LunarNadi)
            {
                if (Gauge.BeastChakra[0] is BeastChakra.None)
                {
                    actionID = OriginalHook(ArmOfTheDestroyer);
                    return true;
                }

                if (Gauge.BeastChakra[1] is BeastChakra.None)
                {
                    actionID = FourPointFury;
                    return true;
                }

                if (Gauge.BeastChakra[2] is BeastChakra.None)
                {
                    actionID = Rockbreaker;
                    return true;
                }
            }

            return false;
        }

        // Open Lunar
        if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
        {
            actionID = OpoFormGCD();
            return true;
        }

        // Open Solar
        if (!SolarNadi && LunarNadi)
        {
            if (Gauge.BeastChakra[0] is BeastChakra.None)
            {
                actionID = CoeurlFormGCD();
                return true;
            }

            if (Gauge.BeastChakra[1] is BeastChakra.None)
            {
                actionID = RaptorFormGCD();
                return true;
            }

            if (Gauge.BeastChakra[2] is BeastChakra.None)
            {
                actionID = OpoFormGCD();
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Basic Combo

    private static uint OpoFormGCD() =>
        OpoOpoStacks is 0 && LevelChecked(DragonKick)
            ? DragonKick
            : OriginalHook(Bootshine);

    private static uint RaptorFormGCD() =>
        RaptorStacks is 0 && LevelChecked(TwinSnakes)
            ? TwinSnakes
            : OriginalHook(TrueStrike);

    private static uint CoeurlFormGCD() =>
        CoeurlStacks is 0 && LevelChecked(Demolish)
            ? Demolish
            : OriginalHook(SnapPunch);

    private static uint DoBasicCombo(uint actionId, bool useTrueNorth = true)
    {
        int tnCharges = IsNotEnabled(Preset.MNK_ST_SimpleMode) ? MNK_ManualTN : 0;

        if (!LevelChecked(TrueStrike))
            return Bootshine;

        if (HasStatusEffect(Buffs.OpoOpoForm) || HasStatusEffect(Buffs.FormlessFist))
            return OpoFormGCD();

        if (HasStatusEffect(Buffs.RaptorForm))
            return RaptorFormGCD();

        if (HasStatusEffect(Buffs.CoeurlForm))
        {
            if (CoeurlStacks is 0 && LevelChecked(Demolish))
                return !OnTargetsRear() &&
                       Role.CanTrueNorth() &&
                       GetRemainingCharges(Role.TrueNorth) > tnCharges &&
                       useTrueNorth
                    ? Role.TrueNorth
                    : Demolish;

            if (LevelChecked(SnapPunch))
                return !OnTargetsFlank() &&
                       Role.CanTrueNorth() &&
                       GetRemainingCharges(Role.TrueNorth) > tnCharges &&
                       useTrueNorth
                    ? Role.TrueNorth
                    : OriginalHook(SnapPunch);
        }

        return actionId;
    }

    #endregion

    #region PB

    private static bool JustUsedOpoGCD(float window, bool onAoE = false) =>
        onAoE
            ? JustUsed(ShadowOfTheDestroyer, window) ||
              JustUsed(OriginalHook(ArmOfTheDestroyer), window) ||
              !LevelChecked(ShadowOfTheDestroyer) && JustUsed(Rockbreaker, window)
            : JustUsed(OriginalHook(Bootshine), window) ||
              JustUsed(DragonKick, window);

    private static bool IsRoFCDInPerfectBalanceWindow() =>
        GetCooldownRemainingTime(RiddleOfFire) is >= 2 and <= 7;

    private static bool IsBrotherhoodCDInPerfectBalanceWindow() =>
        GetCooldownRemainingTime(Brotherhood) is >= 2 and <= 7;

    private static bool IsEvenWindowApproaching() =>
        IsRoFCDInPerfectBalanceWindow() &&
        IsBrotherhoodCDInPerfectBalanceWindow();

    private static bool IsDoubleLunarOpener(bool useOpenerBalance) =>
        useOpenerBalance && MNK_SelectedOpener == 0;

    private static bool ShouldUsePreRoFPerfectBalance(bool useOpenerBalance)
    {
        if (!useOpenerBalance)
            return ShouldUsePreRoFPerfectBalanceDefault();

        if (!IsRoFCDInPerfectBalanceWindow())
            return false;

        // Even window first PB — RoF and BH both 2-7s on Opo GCD
        if (IsEvenWindowApproaching())
            return true;

        // Double Lunar odd minutes use post-RoF PB instead of pre-RoF
        if (IsDoubleLunarOpener(useOpenerBalance) && GetCooldownRemainingTime(Brotherhood) > 7)
            return false;

        // Solar odd — RoF 2-7s only
        return true;
    }

    // Simple ST, Advanced ST without opener, and AoE — RoF 2-7s pre-burst PB.
    private static bool ShouldUsePreRoFPerfectBalanceDefault() =>
        IsRoFCDInPerfectBalanceWindow();

    private static bool ShouldUsePostRoFLunarOddPerfectBalance(bool useOpenerBalance) =>
        IsDoubleLunarOpener(useOpenerBalance) &&
        HasStatusEffect(Buffs.RiddleOfFire) &&
        !HasStatusEffect(Buffs.Brotherhood);

    private static bool HasUsedBlitzRecently(float window) =>
        JustUsed(ElixirBurst, window) || JustUsed(RisingPhoenix, window) ||
        JustUsed(PhantomRush, window) || JustUsed(ElixirField, window) ||
        JustUsed(FlintStrike, window) || JustUsed(TornadoKick, window) ||
        JustUsed(CelestialRevolution, window);

    private static bool UsesFiresReply(bool onAoE) =>
        onAoE
            ? IsEnabled(Preset.MNK_AOE_SimpleMode) || IsEnabled(Preset.MNK_AoEUseFiresReply)
            : IsEnabled(Preset.MNK_ST_SimpleMode) || IsEnabled(Preset.MNK_STUseFiresReply);

    private static bool HasElapsedSinceBlitz(float minGcds) =>
        HasUsedBlitzRecently(GCD * 12) && !HasUsedBlitzRecently(GCD * minGcds);

    private static uint ForcedOpoGCD(bool onAoE)
    {
        if (onAoE)
            return LevelChecked(ShadowOfTheDestroyer)
                ? ShadowOfTheDestroyer
                : Rockbreaker;

        return OpoFormGCD();
    }

    private static bool ForceSecondOpo(bool onAoE)
    {
        if (UsesFiresReply(onAoE))
            return false;

        if (!HasStatusEffect(Buffs.Brotherhood) || !HasStatusEffect(Buffs.RiddleOfFire))
            return false;

        if (HasStatusEffect(Buffs.PerfectBalance) || HasStatusEffect(Buffs.FormlessFist))
            return false;

        if (!IsOriginal(MasterfulBlitz) || GetRemainingCharges(PerfectBalance) >= GetMaxCharges(PerfectBalance))
            return false;

        if (!HasUsedBlitzRecently(GCD * 12))
            return false;

        // First post-blitz Opo (Formless) done — insert a second Opo before 2nd PB
        if (!HasElapsedSinceBlitz(1f) || HasElapsedSinceBlitz(4f))
            return false;

        // Second Opo just used — PB weave is next
        if (JustUsedOpoGCD(GCD, onAoE) && HasElapsedSinceBlitz(2f))
            return false;

        return true;
    }

    private static bool UseSecondPerfectBalance(bool useFiresReply)
    {
        if (!HasStatusEffect(Buffs.Brotherhood) || !HasStatusEffect(Buffs.RiddleOfFire))
            return false;

        if (!IsOriginal(MasterfulBlitz))
            return false;

        if (GetRemainingCharges(PerfectBalance) >= GetMaxCharges(PerfectBalance))
            return false;

        if (!HasUsedBlitzRecently(GCD * 12))
            return false;

        if (useFiresReply)
            return !HasStatusEffect(Buffs.FiresRumination) && JustUsed(FiresReply, GCD * 6);

        // FR disabled: Blitz -> Opo -> Opo -> PB (skip the FR + Formless GCDs)
        return HasElapsedSinceBlitz(2.5f);
    }

    private static bool IsBurstHolding(bool onAoE) =>
        onAoE
            ? IsNotEnabled(Preset.MNK_AOE_SimpleMode) &&
              IsEnabled(Preset.MNK_AoEUsePerfectBalance) &&
              !IsEnabled(Preset.MNK_AoEUseBrotherhood) &&
              !IsEnabled(Preset.MNK_AoEUseROF)
            : IsNotEnabled(Preset.MNK_ST_SimpleMode) &&
              IsEnabled(Preset.MNK_STUsePerfectBalance) &&
              !IsEnabled(Preset.MNK_STUseBrotherhood) &&
              !IsEnabled(Preset.MNK_STUseROF);

    // Both burst buffs are ready but PB must weave first (burst-hold release / drift recovery).
    private static bool IsBurstHoldReleaseReady()
    {
        if (!ActionReady(PerfectBalance) || HasStatusEffect(Buffs.PerfectBalance) ||
            HasStatusEffect(Buffs.FormlessFist) || JustUsed(PerfectBalance))
            return false;

        if (!ActionReady(Brotherhood) || !ActionReady(RiddleOfFire))
            return false;

        if (HasStatusEffect(Buffs.Brotherhood) || HasStatusEffect(Buffs.RiddleOfFire))
            return false;

        // Pre-RoF PB timing already handles ordering when CDs are in the 2-7s window.
        if (IsRoFCDInPerfectBalanceWindow())
            return false;

        return true;
    }

    private static bool UsePBAfterBurstHolding(bool onAoE)
    {
        if (IsBurstHolding(onAoE) || !IsBurstHoldReleaseReady())
            return false;

        if (!HasBattleTarget() || !JustUsedOpoGCD(GCD, onAoE))
            return false;

        if (onAoE && GetTargetHPPercent() < MNK_AoE_PerfectBalanceHPThreshold)
            return false;

        if (JustUsed(PerfectBalance, 20 + GCD * 5))
            return false;

        return true;
    }

    private static bool CanPerfectBalance(bool onAoE, bool useOpenerBalance = false)
    {
        if (IsBurstHolding(onAoE))
            return false;

        if (!ActionReady(PerfectBalance) || HasStatusEffect(Buffs.PerfectBalance) ||
            HasStatusEffect(Buffs.FormlessFist) || !IsOriginal(MasterfulBlitz) ||
            !HasBattleTarget() || JustUsed(PerfectBalance) || !JustUsedOpoGCD(GCD, onAoE))
            return false;

        if (onAoE && GetTargetHPPercent() < MNK_AoE_PerfectBalanceHPThreshold)
            return false;

        if (!JustUsed(PerfectBalance, 20 + GCD * 5))
        {
            if (onAoE)
            {
                if (ShouldUsePreRoFPerfectBalanceDefault())
                    return true;
            }
            else
            {
                if (ShouldUsePreRoFPerfectBalance(useOpenerBalance))
                    return true;

                if (ShouldUsePostRoFLunarOddPerfectBalance(useOpenerBalance))
                    return true;
            }
        }

        if (UseSecondPerfectBalance(UsesFiresReply(onAoE)))
            return true;

        if (!LevelChecked(RiddleOfFire) ||
            HasStatusEffect(Buffs.RiddleOfFire) && !LevelChecked(Brotherhood))
            return JustUsedOpoGCD(GCD * 3, onAoE);

        return onAoE && CanPerfectBalanceMaxChargeAoE();
    }

    // Last-resort AoE PB — only at full charges when normal timing does not apply.
    private static bool CanPerfectBalanceMaxChargeAoE()
    {
        if (GetRemainingCharges(PerfectBalance) != GetMaxCharges(PerfectBalance))
            return false;

        if (IsBurstHoldReleaseReady())
            return false;

        if (IsRoFCDInPerfectBalanceWindow())
            return false;

        return true;
    }

    #endregion

    #region Misc

    private static float GCD =>
        GetCooldown(OriginalHook(Bootshine)).CooldownTotal;

    private static int HPThresholdBH =>
        MNK_ST_BHBossOption == 1 ||
        !InBossEncounter() ? MNK_ST_BHHPThreshold : 0;

    private static int HPThresholdRoF =>
        MNK_ST_RoFBossOption == 1 ||
        !InBossEncounter() ? MNK_ST_RoFHPThreshold : 0;

    private static int HPThresholdRoW =>
        MNK_ST_RoWBossOption == 1 ||
        !InBossEncounter() ? MNK_ST_RoWHPThreshold : 0;

    private static bool CanMantra() =>
        ActionReady(Mantra) &&
        !HasStatusEffect(Buffs.Mantra) &&
        GroupDamageIncoming(3f);

    private static bool CanRoE() =>
        ActionReady(OriginalHook(RiddleOfEarth)) &&
        GroupDamageIncoming(2f) &&
        !HasStatusEffect(Buffs.RiddleOfEarth) &&
        !HasStatusEffect(Buffs.EarthsRumination);

    private static bool CanEarthsReply() =>
        HasStatusEffect(Buffs.EarthsRumination) &&
        NumberOfAlliesInRange(EarthsReply) >= GetPartyMembers().Count * .75 &&
        GetPartyAvgHPPercent() <= MNK_ST_EarthsReplyHPThreshold;

    #endregion

    #region Masterful Blitz

    private static bool CanMasterfulBlitz(bool onAoE)
    {
        if (IsBurstHoldReleaseReady())
            return false;

        if (!LevelChecked(MasterfulBlitz) || HasStatusEffect(Buffs.PerfectBalance) ||
            !InMasterfulRange() || !IsOriginal(MasterfulBlitz))
            return false;

        if (onAoE)
            return true;

        if (BlitzTimer <= GCD * 3)
            return true;

        return HasStatusEffect(Buffs.RiddleOfFire);
    }

    internal static bool InMasterfulRange() =>
        NumberOfEnemiesInRange(ElixirField) >= 1 &&
        OriginalHook(MasterfulBlitz) is ElixirField or FlintStrike or ElixirBurst or RisingPhoenix ||
        NumberOfEnemiesInRange(TornadoKick, CurrentTarget) >= 1 &&
        OriginalHook(MasterfulBlitz) is TornadoKick or CelestialRevolution or PhantomRush;

    #endregion

    #region Chakra

    private static bool CanFormshift() =>
        LevelChecked(FormShift) && !InCombat() &&
        !HasStatusEffect(Buffs.FormlessFist) &&
        !HasStatusEffect(Buffs.PerfectBalance) &&
        !HasStatusEffect(Buffs.OpoOpoForm) &&
        !HasStatusEffect(Buffs.RaptorForm) &&
        !HasStatusEffect(Buffs.CoeurlForm);

    private static bool CanMeditate(bool onAoE = false)
    {
        uint meditation = onAoE ? InspiritedMeditation : SteeledMeditation;
        uint rangeCheck = onAoE ? ArmOfTheDestroyer : Bootshine;

        return LevelChecked(meditation) &&
               (!InCombat() || NumberOfEnemiesInRange(rangeCheck) < 1) &&
               Chakra < 5 &&
               IsOriginal(MasterfulBlitz) &&
               !HasStatusEffect(Buffs.RiddleOfFire) &&
               !HasStatusEffect(Buffs.WindsRumination) &&
               !HasStatusEffect(Buffs.FiresRumination);
    }

    private static bool CanUseChakra(bool onAoE = false)
    {
        if (CanBrotherhood() || CanRoF())
            return false;

        uint meditation = onAoE ? InspiritedMeditation : SteeledMeditation;

        return Chakra >= 5 &&
               (!onAoE || HasBattleTarget()) &&
               !JustUsed(Brotherhood) &&
               !JustUsed(RiddleOfFire) &&
               InActionRange(OriginalHook(meditation));
    }

    #endregion

    #region Buffs

    //RoF
    private static bool CanRoF() =>
        !IsBurstHoldReleaseReady() &&
        ActionReady(RiddleOfFire) &&
        !HasStatusEffect(Buffs.FiresRumination) &&
        !HasStatusEffect(Buffs.RiddleOfFire) &&
        (!LevelChecked(Brotherhood) ||
         JustUsed(Brotherhood, GCD * 5) ||
         HasStatusEffect(Buffs.Brotherhood) ||
         GetCooldownRemainingTime(Brotherhood) is > 50 and < 65 ||
         !ActionReady(Brotherhood));

    private static bool CanFiresReply(bool onAoE = false) =>
        HasStatusEffect(Buffs.FiresRumination) &&
        !HasStatusEffect(Buffs.FormlessFist) &&
        IsOriginal(MasterfulBlitz) &&
        InActionRange(FiresReply) &&
        !JustUsed(RiddleOfFire, GCD) &&
        !HasStatusEffect(Buffs.PerfectBalance) &&
        (JustUsedOpoGCD(GCD * 1.5f, onAoE) ||
         GetStatusEffectRemainingTime(Buffs.FiresRumination) < GCD * 2 ||
         !InMeleeRange());

    //Brotherhood
    private static bool CanBrotherhood() =>
        !IsBurstHoldReleaseReady() &&
        ActionReady(Brotherhood) &&
        ActionReady(RiddleOfFire) &&
        !HasStatusEffect(Buffs.Brotherhood) &&
        (InBossEncounter() || TimeStoodStill.Seconds >= 2);

    //RoW
    private static bool CanRoW() =>
        ActionReady(RiddleOfWind) &&
        !HasStatusEffect(Buffs.WindsRumination);

    private static bool CanWindsReply() =>
        HasStatusEffect(Buffs.WindsRumination) &&
        InActionRange(WindsReply) &&
        !HasStatusEffect(Buffs.FiresRumination) &&
        (GetCooldownRemainingTime(RiddleOfFire) > 10 ||
         HasStatusEffect(Buffs.RiddleOfFire) ||
         GetStatusEffectRemainingTime(Buffs.WindsRumination) < GCD * 2 ||
         !InMeleeRange());

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (MNK_SelectedOpener == 0)
        {
            if (Lvl100LLOpener.LevelChecked)
                return Lvl100LLOpener;

            if (Lvl90LLOpener.LevelChecked)
                return Lvl90LLOpener;
        }

        if (MNK_SelectedOpener == 1)
        {
            if (Lvl100SLOpener.LevelChecked)
                return Lvl100SLOpener;

            if (Lvl90SLOpener.LevelChecked)
                return Lvl90SLOpener;
        }

        return WrathOpener.Dummy;
    }

    internal static MNKLvl90LLOpener Lvl90LLOpener = new();
    internal static MNKLvl100LLOpener Lvl100LLOpener = new();
    internal static MNKLvl90SLOpener Lvl90SLOpener = new();
    internal static MNKLvl100SLOpener Lvl100SLOpener = new();

    internal class MNKLvl90LLOpener : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            ForbiddenMeditation,
            FormShift,
            DragonKick,
            PerfectBalance,
            Bootshine,
            DragonKick,
            Bootshine,
            RiddleOfFire,
            Brotherhood,
            ElixirField,
            DragonKick,
            PerfectBalance,
            Bootshine,
            DragonKick,
            Bootshine,
            ElixirField,
            DragonKick
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => Chakra >= 5),
            ([2], () => JustUsed(FormShift, 30f))
        ];

        internal override UserData ContentCheckConfig => MNK_Balance_Content;
        public override Preset Preset => Preset.MNK_STUseOpener;
        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            (MNK_OpenerCountdown == 1 || CountdownActive) &&
            NadiFlag is None &&
            OpoOpoStacks is 0 &&
            RaptorStacks is 0 &&
            CoeurlStacks is 0;
    }


    internal class MNKLvl90SLOpener : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            ForbiddenMeditation,
            FormShift,
            DragonKick,
            PerfectBalance,
            Bootshine,
            DragonKick,
            Bootshine,
            Brotherhood,
            RiddleOfFire,
            ElixirField,
            DragonKick,
            PerfectBalance,
            Bootshine,
            TwinSnakes,
            Demolish,
            RisingPhoenix,
            DragonKick
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => Chakra >= 5),
            ([2], () => JustUsed(FormShift, 30f))
        ];

        internal override UserData ContentCheckConfig => MNK_Balance_Content;
        public override Preset Preset => Preset.MNK_STUseOpener;
        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            (MNK_OpenerCountdown == 1 || CountdownActive) &&
            NadiFlag is None &&
            OpoOpoStacks is 0 &&
            RaptorStacks is 0 &&
            CoeurlStacks is 0;
    }

    internal class MNKLvl100LLOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            ForbiddenMeditation,
            FormShift,
            DragonKick,
            PerfectBalance,
            LeapingOpo,
            DragonKick,
            Brotherhood,
            RiddleOfFire,
            LeapingOpo,
            TheForbiddenChakra,
            RiddleOfWind,
            ElixirBurst,
            DragonKick,
            WindsReply,
            FiresReply,
            LeapingOpo,
            PerfectBalance,
            DragonKick,
            LeapingOpo,
            DragonKick,
            ElixirBurst,
            LeapingOpo
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => Chakra >= 5),
            ([2], () => JustUsed(FormShift, 30f))
        ];

        internal override UserData ContentCheckConfig => MNK_Balance_Content;
        public override Preset Preset => Preset.MNK_STUseOpener;
        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            (MNK_OpenerCountdown == 1 || CountdownActive) &&
            NadiFlag is None &&
            OpoOpoStacks is 0 &&
            RaptorStacks is 0 &&
            CoeurlStacks is 0;
    }

    internal class MNKLvl100SLOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            ForbiddenMeditation,
            FormShift,
            DragonKick,
            PerfectBalance,
            TwinSnakes,
            Demolish,
            Brotherhood,
            RiddleOfFire,
            LeapingOpo,
            TheForbiddenChakra,
            RiddleOfWind,
            RisingPhoenix,
            DragonKick,
            WindsReply,
            FiresReply,
            LeapingOpo,
            PerfectBalance,
            DragonKick,
            LeapingOpo,
            DragonKick,
            ElixirBurst,
            LeapingOpo
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => Chakra >= 5),
            ([2], () => JustUsed(FormShift, 30f))
        ];

        internal override UserData ContentCheckConfig => MNK_Balance_Content;
        public override Preset Preset => Preset.MNK_STUseOpener;
        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            (MNK_OpenerCountdown == 1 || CountdownActive) &&
            NadiFlag is None &&
            OpoOpoStacks is 0 &&
            RaptorStacks is 0 &&
            CoeurlStacks is 0;
    }

    #endregion

    #region Gauge

    private static MNKGauge Gauge => GetJobGauge<MNKGauge>();

    private static byte Chakra => Gauge.Chakra;

    private static int OpoOpoStacks => Gauge.OpoOpoFury;

    private static int RaptorStacks => Gauge.RaptorFury;

    private static int CoeurlStacks => Gauge.CoeurlFury;

    private static Nadi NadiFlag => Gauge.Nadi;

    private static bool BothNadisOpen => NadiFlag.HasFlag(Nadi.Lunar) && NadiFlag.HasFlag(Nadi.Solar);

    private static bool SolarNadi => NadiFlag is Nadi.Solar;

    private static bool LunarNadi => NadiFlag is Nadi.Lunar;

    private static int BlitzTimer => Gauge.BlitzTimeRemaining / 1000;

    #endregion

    #region ID's

    public const uint
        Bootshine = 53,
        TrueStrike = 54,
        SnapPunch = 56,
        TwinSnakes = 61,
        ArmOfTheDestroyer = 62,
        Demolish = 66,
        DragonKick = 74,
        Rockbreaker = 70,
        Thunderclap = 25762,
        HowlingFist = 25763,
        FourPointFury = 16473,
        FormShift = 4262,
        SixSidedStar = 16476,
        ShadowOfTheDestroyer = 25767,
        LeapingOpo = 36945,
        RisingRaptor = 36946,
        PouncingCoeurl = 36947,

        //Blitzes
        PerfectBalance = 69,
        MasterfulBlitz = 25764,
        ElixirField = 3545,
        ElixirBurst = 36948,
        FlintStrike = 25882,
        RisingPhoenix = 25768,
        CelestialRevolution = 25765,
        TornadoKick = 3543,
        PhantomRush = 25769,

        //Riddles + Buffs
        RiddleOfEarth = 7394,
        EarthsReply = 36944,
        RiddleOfFire = 7395,
        FiresReply = 36950,
        RiddleOfWind = 25766,
        WindsReply = 36949,
        Brotherhood = 7396,
        Mantra = 65,

        //Meditations
        InspiritedMeditation = 36941,
        SteeledMeditation = 36940,
        EnlightenedMeditation = 36943,
        ForbiddenMeditation = 36942,
        TheForbiddenChakra = 3547,
        Enlightenment = 16474,
        SteelPeak = 25761;

    internal static class Buffs
    {
        public const ushort
            TwinSnakes = 101,
            Mantra = 102,
            OpoOpoForm = 107,
            RaptorForm = 108,
            CoeurlForm = 109,
            PerfectBalance = 110,
            RiddleOfEarth = 1179,
            RiddleOfFire = 1181,
            Brotherhood = 1185,
            FormlessFist = 2513,
            RiddleOfWind = 2687,
            EarthsRumination = 3841,
            WindsRumination = 3842,
            FiresRumination = 3843;
    }

    #endregion
}

internal static class MNKExtensions
{
    public const Nadi None = 0;
}
