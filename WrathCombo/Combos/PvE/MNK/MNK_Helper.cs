using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static ECommons.DalamudServices.Svc;
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
                actionID = ActionLearned(ShadowOfTheDestroyer) ? ShadowOfTheDestroyer : Rockbreaker;
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

                if (Gauge.BeastChakra[1] is BeastChakra.None &&
                    ActionLearned(FourPointFury))
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
        OpoOpoStacks is 0 && ActionLearned(DragonKick)
            ? DragonKick
            : OriginalHook(Bootshine);

    private static uint RaptorFormGCD() =>
        RaptorStacks is 0 && ActionLearned(TwinSnakes)
            ? TwinSnakes
            : OriginalHook(TrueStrike);

    private static uint CoeurlFormGCD() =>
        CoeurlStacks is 0 && ActionLearned(Demolish)
            ? Demolish
            : OriginalHook(SnapPunch);

    private static uint DoBasicCombo(bool useTrueNorth = true, bool onAoE = false, int trueNorthCharges = 0)
    {
        if (onAoE)
        {
            if (HasStatusEffect(Buffs.OpoOpoForm))
                return OriginalHook(ArmOfTheDestroyer);

            if (HasStatusEffect(Buffs.RaptorForm))
            {
                if (ActionLearned(FourPointFury))
                    return FourPointFury;

                if (ActionLearned(TwinSnakes))
                    return TwinSnakes;
            }

            if (HasStatusEffect(Buffs.CoeurlForm) && ActionLearned(Rockbreaker))
                return Rockbreaker;

            return OriginalHook(ArmOfTheDestroyer);
        }

        if (!ActionLearned(TrueStrike))
            return Bootshine;

        if (HasStatusEffect(Buffs.OpoOpoForm) || HasStatusEffect(Buffs.FormlessFist))
            return OpoFormGCD();

        if (HasStatusEffect(Buffs.RaptorForm))
            return RaptorFormGCD();

        if (HasStatusEffect(Buffs.CoeurlForm))
        {
            if (CoeurlStacks is 0 && ActionLearned(Demolish))
                return !OnTargetsRear() &&
                       Role.CanTrueNorth() &&
                       GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
                       useTrueNorth
                    ? Role.TrueNorth
                    : Demolish;

            if (ActionLearned(SnapPunch))
                return !OnTargetsFlank() &&
                       Role.CanTrueNorth() &&
                       GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
                       useTrueNorth
                    ? Role.TrueNorth
                    : OriginalHook(SnapPunch);
        }

        return OriginalHook(Bootshine);
    }

    #endregion

    #region PB

    private static bool JustUsedOpoGCD(float window, bool onAoE = false) =>
        onAoE
            ? JustUsed(ShadowOfTheDestroyer, window) ||
              JustUsed(OriginalHook(ArmOfTheDestroyer), window) ||
              !ActionLearned(ShadowOfTheDestroyer) && JustUsed(Rockbreaker, window)
            : JustUsed(OriginalHook(Bootshine), window) ||
              JustUsed(DragonKick, window);

    private static bool IsRoFInPerfectBalanceWindow() =>
        GetCooldownRemainingTime(RiddleOfFire) is >= 2 and <= 7;

    private static bool IsBrotherhoodInPerfectBalanceWindow() =>
        GetCooldownRemainingTime(Brotherhood) is >= 2 and <= 7;

    private static bool IsEvenWindowApproaching() =>
        IsRoFInPerfectBalanceWindow() &&
        IsBrotherhoodInPerfectBalanceWindow();

    private static bool IsDoubleLunarOpener(bool useOpenerBalance) =>
        useOpenerBalance &&
        (MNK_SelectedOpener != 1 || ClientState.TerritoryType == 1363);

    private static bool ShouldUsePreRoFPerfectBalance(bool useOpenerBalance)
    {
        if (!useOpenerBalance)
            return ShouldUsePreRoFPerfectBalanceDefault();

        if (!IsRoFInPerfectBalanceWindow())
            return false;

        if (IsEvenWindowApproaching())
            return true;

        if (IsDoubleLunarOpener(useOpenerBalance) && GetCooldownRemainingTime(Brotherhood) > 7)
            return false;

        return true;
    }

    private static bool ShouldUsePreRoFPerfectBalanceDefault() =>
        IsRoFInPerfectBalanceWindow();

    private static bool ShouldUsePostRoFLunarOddPerfectBalance(bool useOpenerBalance) =>
        IsDoubleLunarOpener(useOpenerBalance) &&
        HasStatusEffect(Buffs.RiddleOfFire) &&
        !HasStatusEffect(Buffs.Brotherhood);

    private static bool HasUsedBlitzRecently(float window) =>
        JustUsed(ElixirBurst, window) || JustUsed(RisingPhoenix, window) ||
        JustUsed(PhantomRush, window) || JustUsed(ElixirField, window) ||
        JustUsed(FlintStrike, window) || JustUsed(TornadoKick, window) ||
        JustUsed(CelestialRevolution, window);

    private static bool HasElapsedSinceBlitz(float minGcds) =>
        HasUsedBlitzRecently(GCD * 12) && !HasUsedBlitzRecently(GCD * minGcds);

    private static uint ForcedOpoGCD(bool onAoE)
    {
        if (onAoE)
            return ActionLearned(ShadowOfTheDestroyer)
                ? ShadowOfTheDestroyer
                : Rockbreaker;

        return OpoFormGCD();
    }

    private static bool ForceSecondOpo(bool onAoE, bool useFiresReply = true)
    {
        if (useFiresReply && ActionLearned(FiresReply))
            return false;

        if (!HasStatusEffect(Buffs.Brotherhood) || !HasStatusEffect(Buffs.RiddleOfFire))
            return false;

        if (HasStatusEffect(Buffs.PerfectBalance) || HasStatusEffect(Buffs.FormlessFist))
            return false;

        if (!IsOriginal(MasterfulBlitz) || GetRemainingCharges(PerfectBalance) >= GetMaxCharges(PerfectBalance))
            return false;

        if (!HasUsedBlitzRecently(GCD * 12))
            return false;

        if (HasStatusEffect(Buffs.FiresRumination) ||
            JustUsed(FiresReply, GCD * 12))
            return false;

        if (!HasElapsedSinceBlitz(1f) || HasElapsedSinceBlitz(4f))
            return false;

        if (JustUsedOpoGCD(GCD, onAoE) && HasElapsedSinceBlitz(2f))
            return false;

        return true;
    }

    private static bool ShouldUseSecondPerfectBalance(bool useFiresReply)
    {
        if (!HasStatusEffect(Buffs.Brotherhood) || !HasStatusEffect(Buffs.RiddleOfFire))
            return false;

        if (!IsOriginal(MasterfulBlitz))
            return false;

        if (GetRemainingCharges(PerfectBalance) >= GetMaxCharges(PerfectBalance))
            return false;

        if (!HasUsedBlitzRecently(GCD * 12))
            return false;

        if (useFiresReply && ActionLearned(FiresReply))
            return JustUsed(FiresReply, GCD * 6) && !HasStatusEffect(Buffs.FiresRumination);

        return HasElapsedSinceBlitz(2.5f);
    }

    private static bool ShouldUsePBAfterBurstHolding(bool onAoE, int perfectBalanceHpThreshold = 0)
    {
        if (!IsBurstHoldReleaseReady())
            return false;

        if (!HasBattleTarget() || !JustUsedOpoGCD(GCD, onAoE))
            return false;

        if (onAoE && perfectBalanceHpThreshold > 0 && GetTargetHPPercent() < perfectBalanceHpThreshold)
            return false;

        if (JustUsed(PerfectBalance, 20 + GCD * 5))
            return false;

        return true;
    }

    private static bool IsBurstHoldReleaseReady()
    {
        if (!ActionReady(PerfectBalance) || HasStatusEffect(Buffs.PerfectBalance) ||
            HasStatusEffect(Buffs.FormlessFist) || JustUsed(PerfectBalance))
            return false;

        if (!ActionReady(Brotherhood) || !ActionReady(RiddleOfFire))
            return false;

        if (HasStatusEffect(Buffs.Brotherhood) || HasStatusEffect(Buffs.RiddleOfFire))
            return false;

        if (IsRoFInPerfectBalanceWindow())
            return false;

        return true;
    }

    private static bool UsePerfectBalance(
        bool onAoE,
        bool useOpenerBalance = false,
        bool isBurstHolding = false,
        int perfectBalanceHpThreshold = 0,
        bool useFiresReply = true)
    {
        if (isBurstHolding && !IsBurstHoldReleaseReady())
            return false;

        if (!ActionReady(PerfectBalance) || HasStatusEffect(Buffs.PerfectBalance) ||
            HasStatusEffect(Buffs.FormlessFist) || !IsOriginal(MasterfulBlitz) ||
            !HasBattleTarget() || JustUsed(PerfectBalance) || !JustUsedOpoGCD(GCD, onAoE))
            return false;

        if (onAoE && perfectBalanceHpThreshold > 0 && GetTargetHPPercent() < perfectBalanceHpThreshold)
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

        if (ShouldUseSecondPerfectBalance(useFiresReply))
            return true;

        if (!ActionLearned(RiddleOfFire) ||
            HasStatusEffect(Buffs.RiddleOfFire) && !ActionLearned(Brotherhood))
            return JustUsedOpoGCD(GCD * 3, onAoE);

        return onAoE && UsePerfectBalanceMaxChargeAoE();
    }

    private static bool UsePerfectBalanceMaxChargeAoE()
    {
        if (GetRemainingCharges(PerfectBalance) != GetMaxCharges(PerfectBalance))
            return false;

        if (IsBurstHoldReleaseReady())
            return false;

        if (IsRoFInPerfectBalanceWindow())
            return false;

        return true;
    }

    #endregion

    #region Misc

    private static float GCD =>
        GetCooldown(OriginalHook(Bootshine)).CooldownTotal;

    private static int BossHpThreshold(int hpBossOption, int hpOption, bool isBoss) =>
        hpBossOption == 1 || !isBoss ? hpOption : 0;

    private static int BrotherhoodHPThreshold =>
        BossHpThreshold(MNK_ST_BHHPBossOption, MNK_ST_BHHPOption, InBossEncounter());

    private static int RiddleOfFireHPThreshold =>
        BossHpThreshold(MNK_ST_RoFHPBossOption, MNK_ST_RoFHPOption, InBossEncounter());

    private static int RiddleOfWindHPThreshold =>
        BossHpThreshold(MNK_ST_RoWHPBossOption, MNK_ST_RoWHPOption, InBossEncounter());

    private static bool UseMantra() =>
        ActionReady(Mantra) &&
        !HasStatusEffect(Buffs.Mantra) &&
        GroupDamageIncoming(3f);

    private static bool UseRoE() =>
        ActionReady(OriginalHook(RiddleOfEarth)) &&
        GroupDamageIncoming(2f) &&
        !HasStatusEffect(Buffs.RiddleOfEarth) &&
        !HasStatusEffect(Buffs.EarthsRumination);

    private static bool UseEarthsReply(int earthsReplyHpThreshold = 25) =>
        HasStatusEffect(Buffs.EarthsRumination) &&
        NumberOfAlliesInRange(EarthsReply) >= GetPartyMembers().Count * .75 &&
        GetPartyAvgHPPercent() <= earthsReplyHpThreshold;

    #endregion

    #region Masterful Blitz

    private static bool ShouldSpendMasterfulBlitz(bool onAoE)
    {
        if (BlitzTimer <= GCD * 3)
            return true;

        if (IsBurstHoldReleaseReady())
            return false;

        if (onAoE)
            return true;

        if (HasStatusEffect(Buffs.RiddleOfFire))
            return true;

        return !ActionLearned(RiddleOfFire);
    }

    private static bool UseMasterfulBlitz(bool onAoE)
    {
        if (!ActionLearned(MasterfulBlitz) || !InMasterfulRange() || IsOriginal(MasterfulBlitz))
            return false;

        if (HasStatusEffect(Buffs.PerfectBalance))
            return true;

        return ShouldSpendMasterfulBlitz(onAoE);
    }

    internal static bool InMasterfulRange() =>
        NumberOfEnemiesInRange(ElixirField) >= 1 &&
        OriginalHook(MasterfulBlitz) is ElixirField or FlintStrike or ElixirBurst or RisingPhoenix ||
        NumberOfEnemiesInRange(TornadoKick, CurrentTarget) >= 1 &&
        OriginalHook(MasterfulBlitz) is TornadoKick or CelestialRevolution or PhantomRush;

    #endregion

    #region Chakra

    private static bool UseFormshift() =>
        ActionLearned(FormShift) && !InCombat() &&
        !HasStatusEffect(Buffs.FormlessFist) &&
        !HasStatusEffect(Buffs.PerfectBalance) &&
        !HasStatusEffect(Buffs.OpoOpoForm) &&
        !HasStatusEffect(Buffs.RaptorForm) &&
        !HasStatusEffect(Buffs.CoeurlForm);

    private static bool UseMeditate(bool onAoE = false)
    {
        uint meditation = onAoE ? InspiritedMeditation : SteeledMeditation;
        uint rangeCheck = onAoE ? ArmOfTheDestroyer : Bootshine;

        return ActionLearned(meditation) &&
               (!InCombat() || NumberOfEnemiesInRange(rangeCheck) < 1) &&
               Chakra < 5 &&
               IsOriginal(MasterfulBlitz) &&
               !HasStatusEffect(Buffs.RiddleOfFire) &&
               !HasStatusEffect(Buffs.WindsRumination) &&
               !HasStatusEffect(Buffs.FiresRumination);
    }

    private static bool UseChakra(bool onAoE = false)
    {
        if (UseBrotherhood() || UseRoF())
            return false;

        if (!HasStatusEffect(Buffs.Brotherhood) &&
            ActionReady(RiddleOfFire) && ActionLearned(Brotherhood) &&
            GetCooldownRemainingTime(Brotherhood) <= GCD)
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

    private static bool UseRoF() =>
        !IsBurstHoldReleaseReady() &&
        ActionReady(RiddleOfFire) &&
        !HasStatusEffect(Buffs.FiresRumination) &&
        !HasStatusEffect(Buffs.RiddleOfFire) &&
        (!ActionLearned(Brotherhood) ||
         JustUsed(Brotherhood, GCD * 5) ||
         HasStatusEffect(Buffs.Brotherhood) ||
         GetCooldownRemainingTime(Brotherhood) is > 50 and < 65 ||
         !ActionLearned(Brotherhood));

    private static bool UseFiresReply(bool onAoE = false) =>
        ActionLearned(FiresReply) &&
        HasStatusEffect(Buffs.FiresRumination) &&
        !HasStatusEffect(Buffs.FormlessFist) &&
        IsOriginal(MasterfulBlitz) &&
        InActionRange(FiresReply) &&
        !JustUsed(RiddleOfFire, GCD) &&
        !HasStatusEffect(Buffs.PerfectBalance) &&
        (JustUsedOpoGCD(GCD * 1.5f, onAoE) ||
         GetStatusEffectRemainingTime(Buffs.FiresRumination) < GCD * 2 ||
         !InMeleeRange());

    private static bool UseBrotherhood() =>
        !IsBurstHoldReleaseReady() &&
        ActionReady(Brotherhood) &&
        ActionReady(RiddleOfFire) &&
        !HasStatusEffect(Buffs.Brotherhood) &&
        (InBossEncounter() || TimeStoodStill.Seconds >= 2);

    private static bool UseRoW() =>
        ActionReady(RiddleOfWind) &&
        !HasStatusEffect(Buffs.WindsRumination);

    private static bool UseWindsReply() =>
        HasStatusEffect(Buffs.WindsRumination) &&
        InActionRange(WindsReply) &&
        (GetStatusEffectRemainingTime(Buffs.WindsRumination) <= 3f ||
         !HasStatusEffect(Buffs.FiresRumination) &&
         (GetCooldownRemainingTime(RiddleOfFire) > 10 ||
          HasStatusEffect(Buffs.RiddleOfFire) ||
          GetStatusEffectRemainingTime(Buffs.WindsRumination) < GCD * 2 ||
          !InMeleeRange()));

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (DMUOpener.LevelChecked &&
            ClientState.TerritoryType == 1363)
            return DMUOpener;

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
    internal static MNKLvl100DMUOpener DMUOpener = new();

    internal abstract class MNKOpenerBase : WrathOpener
    {
        public override Preset Preset => Preset.MNK_STUseOpener;

        internal override UserData ContentCheckConfig => MNK_Balance_Content;
        internal override bool IncludePot => MNK_Opener_Potion;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat()),
            ([2], () => Chakra >= 5),
            ([3], () => HasStatusEffect(Buffs.FormlessFist) || JustUsed(FormShift))
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => CountdownRemaining - 8),
            ([3], () => CountdownRemaining - 5)
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            NadiFlag is None &&
            OpoOpoStacks is 0 &&
            RaptorStacks is 0 &&
            CoeurlStacks is 0;
    }

    internal class MNKLvl90LLOpener : MNKOpenerBase
    {
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 95;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => ForbiddenMeditation, // 2
            () => FormShift, // 3
            () => DragonKick, // 4
            () => PerfectBalance, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 6
            () => Bootshine, // 7
            () => DragonKick, // 8
            () => Bootshine, // 9
            () => RiddleOfFire, // 10
            () => Brotherhood, // 11
            () => ElixirField, // 12
            () => DragonKick, // 13
            () => PerfectBalance, // 14
            () => Bootshine, // 15
            () => DragonKick, // 16
            () => Bootshine, // 17
            () => ElixirField, // 18
            () => DragonKick // 19
        ];

        public override List<int> AllowUpgradeSteps { get; set; } = [7, 9, 12, 15, 17, 18];
    }

    internal class MNKLvl90SLOpener : MNKOpenerBase
    {
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 95;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => ForbiddenMeditation, // 2
            () => FormShift, // 3
            () => DragonKick, // 4
            () => PerfectBalance, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 6
            () => Bootshine, // 7
            () => DragonKick, // 8
            () => Bootshine, // 9
            () => Brotherhood, // 10
            () => RiddleOfFire, // 11
            () => ElixirField, // 12
            () => DragonKick, // 13
            () => PerfectBalance, // 14
            () => Bootshine, // 15
            () => TwinSnakes, // 16
            () => Demolish, // 17
            () => RisingPhoenix, // 18
            () => DragonKick // 19
        ];

        public override List<int> AllowUpgradeSteps { get; set; } = [7, 9, 12, 15];
    }

    internal class MNKLvl100LLOpener : MNKOpenerBase
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => ForbiddenMeditation, // 2
            () => FormShift, // 3
            () => DragonKick, // 4
            () => PerfectBalance, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 6
            () => LeapingOpo, // 7
            () => DragonKick, // 8
            () => Brotherhood, // 9
            () => RiddleOfFire, // 10
            () => LeapingOpo, // 11
            () => TheForbiddenChakra, // 12
            () => RiddleOfWind, // 13
            () => ElixirBurst, // 14
            () => DragonKick, // 15
            () => WindsReply, // 16
            () => FiresReply, // 17
            () => LeapingOpo, // 18
            () => PerfectBalance, // 19
            () => DragonKick, // 20
            () => LeapingOpo, // 21
            () => DragonKick, // 22
            () => ElixirBurst, // 23
            () => LeapingOpo // 24
        ];

        public MNKLvl100LLOpener() => SkipSteps.Add(([12], () => Chakra < 5));
    }

    internal class MNKLvl100SLOpener : MNKOpenerBase
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => ForbiddenMeditation, // 2
            () => FormShift, // 3
            () => DragonKick, // 4
            () => PerfectBalance, // 5
            () => TwinSnakes, // 6
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 7
            () => Demolish, // 8
            () => Brotherhood, // 9
            () => RiddleOfFire, // 10
            () => LeapingOpo, // 11
            () => TheForbiddenChakra, // 12
            () => RiddleOfWind, // 13
            () => RisingPhoenix, // 14
            () => DragonKick, // 15
            () => WindsReply, // 16
            () => FiresReply, // 17
            () => LeapingOpo, // 18
            () => PerfectBalance, // 19
            () => DragonKick, // 20
            () => LeapingOpo, // 21
            () => DragonKick, // 22
            () => ElixirBurst, // 23
            () => LeapingOpo // 24
        ];

        public MNKLvl100SLOpener() => SkipSteps.Add(([12], () => Chakra < 5));
    }

    internal class MNKLvl100DMUOpener : MNKOpenerBase
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => ForbiddenMeditation, // 2
            () => FormShift, // 3
            () => RiddleOfWind, // 4
            () => DragonKick, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 6
            () => Brotherhood, // 7
            () => RiddleOfFire, // 8
            () => FiresReply, // 9
            () => PerfectBalance, // 10
            () => TheForbiddenChakra, // 11
            () => WindsReply, // 12
            () => LeapingOpo, // 13
            () => DragonKick, // 14
            () => LeapingOpo, // 15
            () => ElixirBurst, // 16
            () => DragonKick, // 17
            () => PerfectBalance, // 18
            () => LeapingOpo, // 19
            () => DragonKick, // 20
            () => LeapingOpo, // 21
            () => ElixirBurst, // 22
            () => DragonKick // 23
        ];

        public MNKLvl100DMUOpener()
        {
            SkipSteps.Add(([11], () => Chakra < 5));
            PrepullDelays.Add(([4], () => CountdownRemaining - 2));
        }
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
