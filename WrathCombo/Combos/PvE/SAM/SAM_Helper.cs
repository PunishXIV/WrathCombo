using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager;
using static WrathCombo.Combos.PvE.SAM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using ActionType = FFXIVClientStructs.FFXIV.Client.Game.ActionType;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    #region Combo

    private static uint WithTrueNorth(
        uint action,
        bool onPositional,
        bool useTrueNorth = true,
        int trueNorthCharges = 0) =>
        !onPositional &&
        useTrueNorth &&
        Role.CanTrueNorth() &&
        GetRemainingCharges(Role.TrueNorth) > trueNorthCharges &&
        TargetNeedsPositionals()
            ? Role.TrueNorth
            : action;

    private static uint DoMeikyoCombo(
        uint actionID,
        bool onAoE,
        bool useTrueNorth = true,
        bool useYukikaze = true,
        bool useKasha = true,
        bool useGekko = true,
        bool useOka = true,
        int trueNorthCharges = 0)
    {
        if (onAoE)
        {
            float fugetsuRemaining = GetStatusEffectRemainingTime(Buffs.Fugetsu);
            float fukaRemaining = GetStatusEffectRemainingTime(Buffs.Fuka);
            bool refreshFugetsu = fugetsuRemaining <= fukaRemaining;
            bool refreshFuka = fukaRemaining <= fugetsuRemaining;

            if (useOka &&
                (!HasKa || !HasStatusEffect(Buffs.Fuka) ||
                 SenCount is 2 or 3 && refreshFuka) &&
                LevelChecked(Oka))
                return Oka;

            if (LevelChecked(Mangetsu) &&
                (!HasGetsu || !HasStatusEffect(Buffs.Fugetsu) || !useOka || !LevelChecked(Oka) ||
                 SenCount is 2 or 3 && refreshFugetsu))
                return Mangetsu;

            return actionID;
        }

        if (useKasha &&
            LevelChecked(Kasha) &&
            (!HasStatusEffect(Buffs.Fuka) ||
             (OnTargetsFlank() || OnTargetsFront()) && !HasKa ||
             OnTargetsRear() && HasGetsu && !HasKa ||
             !HasKa && (!useGekko || !LevelChecked(Gekko) || HasGetsu)))
            return WithTrueNorth(Kasha, OnTargetsFlank(), useTrueNorth, trueNorthCharges);

        if (useGekko &&
            LevelChecked(Gekko) &&
            (!HasStatusEffect(Buffs.Fugetsu) ||
             (OnTargetsRear() || OnTargetsFront()) && !HasGetsu ||
             OnTargetsFlank() && HasKa && !HasGetsu ||
             !HasGetsu && (!useKasha || !LevelChecked(Kasha) || HasKa)))
            return WithTrueNorth(Gekko, OnTargetsRear(), useTrueNorth, trueNorthCharges);

        if (useYukikaze &&
            LevelChecked(Yukikaze) &&
            !HasSetsu &&
            (!useGekko || !LevelChecked(Gekko) || HasGetsu) &&
            (!useKasha || !LevelChecked(Kasha) || HasKa))
            return Yukikaze;

        return actionID;
    }

    private static bool UseTsubame(bool onAoE)
    {
        if (!ActionReady(OriginalHook(TsubameGaeshi)) ||
            !InActionRange(OriginalHook(TsubameGaeshi)))
            return false;

        if (onAoE)
            return HasStatusEffect(Buffs.TsubameReady) ||
                   HasStatusEffect(Buffs.KaeshiGokenReady) ||
                   HasStatusEffect(Buffs.TendoKaeshiGokenReady);

        if (HasStatusEffect(Buffs.TendoKaeshiSetsugekkaReady))
            return true;

        if (!HasStatusEffect(Buffs.TsubameReady))
            return false;

        if (SenCount is 3 ||
            GetStatusEffectRemainingTime(Buffs.TsubameReady) < 3 ||
            !InBossEncounter())
            return true;

        return LevelChecked(Senei) && GetCooldownRemainingTime(Senei) < 7f;
    }

    private static bool UseIaiJutsu(
        bool onAoE,
        bool useHiganbana = true,
        bool useTenkaGoken = true,
        bool useMidare = true,
        bool onlyWhenStationary = true,
        int higanbanaHpThreshold = 0,
        int higanbanaDotRefresh = 15)
    {
        if (onlyWhenStationary && IsMoving() ||
            !ActionReady(OriginalHook(Iaijutsu)) ||
            !InActionRange(OriginalHook(Iaijutsu)) ||
            !HasStatusEffect(Buffs.Fuka) ||
            !HasStatusEffect(Buffs.Fugetsu))
            return false;

        if (onAoE)
        {
            if (useTenkaGoken &&
                SenCount is 2 &&
                OriginalHook(Iaijutsu) is TenkaGoken or TendoGoken)
                return true;

            if (useMidare &&
                SenCount is 3 &&
                OriginalHook(Iaijutsu) is MidareSetsugekka or TendoSetsugekka)
                return true;

            return false;
        }

        if (useHiganbana && SenCount is 1 &&
            UseHiganbana(higanbanaHpThreshold, higanbanaDotRefresh))
            return true;

        if (useMidare && SenCount is 3 && !HasStatusEffect(Buffs.TsubameReady) ||
            useTenkaGoken && SenCount is 2 && !LevelChecked(MidareSetsugekka))
            return true;

        return false;
    }

    private static bool UseHiganbana(int hpThreshold = 0, int dotRefresh = 15)
    {
        if (!HasBattleTarget() ||
            !CanApplyStatus(CurrentTarget, Debuffs.Higanbana) ||
            GetTargetHPPercent() <= hpThreshold)
            return false;

        float remaining = GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget);

        if (!HasStatusEffect(Debuffs.Higanbana, CurrentTarget))
            return true;

        if (remaining > dotRefresh)
            return false;

        if (remaining <= GCD * 2)
            return true;

        if (LevelChecked(Senei) && GetCooldownRemainingTime(Senei) < 7f)
            return false;

        if (HasEnhancedSenei)
            return JustUsed(Senei, 35f) || JustUsed(Ikishoten, 35f);

        return true;
    }

    private static int HiganbanaHPThreshold()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? SAM_ST_HiganbanaHPOption : SAM_ST_HiganbanaAddsHPOption;

        return SAM_ST_HiganbanaTrashHPOption;
    }

    private static bool UsePrepullMeikyo(bool requireNotJustUsed = false) =>
        !InCombat() && HasBattleTarget() &&
        ActionReady(MeikyoShisui) &&
        !HasStatusEffect(Buffs.MeikyoShisui) &&
        (!requireNotJustUsed || !JustUsed(MeikyoShisui));

    private static bool UseMeikyo(bool onAoE, int meikyoExecuteThreshold = 5)
    {
        if (!ActionReady(MeikyoShisui) ||
            HasStatusEffect(Buffs.MeikyoShisui) ||
            HasStatusEffect(Buffs.Tendo) ||
            JustUsed(MeikyoShisui))
            return false;

        if (onAoE)
            return ComboTimer is 0;

        bool afterFinisher =
            JustUsed(Yukikaze, 2f) || JustUsed(Gekko, 2f) || JustUsed(Kasha, 2f);
        bool afterKaeshi =
            JustUsed(KaeshiSetsugekka, 2f) || JustUsed(TendoKaeshiSetsugekka, 2f);

        if (TargetIsBoss() && GetTargetHPPercent() < meikyoExecuteThreshold && afterFinisher)
            return true;

        if (!LevelChecked(Senei))
            return afterFinisher;

        float seneiCd = GetCooldownRemainingTime(Senei);
        bool seneiSoon = seneiCd < 7f;
        bool oddMinutePreEnhanced = !HasEnhancedSenei && seneiCd is > 50 and < 65;
        uint meikyoCharges = GetRemainingCharges(MeikyoShisui);

        float higanbanaRemaining = GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget);
        bool higanbanaUrgent =
            afterFinisher &&
            SenCount < 3 &&
            SenCount is not 1 &&
            (!HasStatusEffect(Debuffs.Higanbana, CurrentTarget) || higanbanaRemaining <= 15);

        if (higanbanaUrgent)
            return true;

        if (HasEnhancedSenei &&
            meikyoCharges >= 2 &&
            JustUsed(KaeshiNamikiri, 10f) &&
            afterFinisher &&
            seneiCd > GCD * 10 && seneiCd < 50)
            return true;

        if (TraitLevelChecked(Traits.EnhancedMeikyoShishui) &&
            meikyoCharges >= 1 &&
            afterFinisher &&
            GetCooldownChargeRemainingTime(MeikyoShisui) <= GCD * 2 &&
            !seneiSoon)
            return true;

        if (!seneiSoon && !oddMinutePreEnhanced)
            return false;

        return afterKaeshi || afterFinisher;
    }

    private static bool UseIkishoten() =>
        ActionReady(Ikishoten) &&
        !HasStatusEffect(Buffs.ZanshinReady) &&
        Kenki <= 50 &&
        (!LevelChecked(Senei) ||
         JustUsed(Senei, 20f) ||
         GetCooldownRemainingTime(Senei) <= GCD * 3);

    private static bool UseZanshin(bool holdForBurst = true) =>
        ActionReady(Zanshin) &&
        InActionRange(Zanshin) &&
        HasStatusEffect(Buffs.ZanshinReady) &&
        (!holdForBurst || !UseSenei() && !ActionReady(Senei)) &&
        (GetStatusEffectRemainingTime(Buffs.ZanshinReady) <= 8 ||
         JustUsed(Senei, 20f) ||
         !holdForBurst);

    private static bool UseShoha(bool holdForBurst = true) =>
        ActionReady(Shoha) &&
        MeditationStacks is 3 &&
        InActionRange(Shoha) &&
        (!holdForBurst || !LevelChecked(Senei) || GetCooldownRemainingTime(Senei) >= 7f);

    private static bool ShouldRefreshFugetsu =>
        GetStatusEffectRemainingTime(Buffs.Fugetsu) <=
        GetStatusEffectRemainingTime(Buffs.Fuka);

    private static bool ShouldRefreshFuka =>
        GetStatusEffectRemainingTime(Buffs.Fuka) <=
        GetStatusEffectRemainingTime(Buffs.Fugetsu);

    private static bool UseFeatureKenkiOvercap(ref uint actionID, bool enabled, int amount, uint spender)
    {
        if (!enabled || !CanWeave() || Kenki < amount || !LevelChecked(spender))
            return false;

        actionID = OriginalHook(spender);
        return true;
    }

    private static bool UseThirdEye() =>
        ActionReady(OriginalHook(ThirdEye)) &&
        (GroupDamageIncoming(2f) || !IsInParty());

    private static bool UseMeditate() =>
        ActionReady(Meditate) &&
        !IsMoving() &&
        TimeStoodStill > TimeSpan.FromSeconds(SAM_ST_MeditateTimeStill) &&
        InCombat() &&
        !HasBattleTarget();

    private static bool UseOgiNamikiri(bool onAoE, bool respectMovement = true)
    {
        if (IsNamikiriReady)
            return ActionReady(OriginalHook(OgiNamikiri)) &&
                   InActionRange(OriginalHook(OgiNamikiri));

        if (!ActionReady(OriginalHook(OgiNamikiri)) ||
            !InActionRange(OriginalHook(OgiNamikiri)) ||
            !HasStatusEffect(Buffs.OgiNamikiriReady) ||
            respectMovement && IsMoving() ||
            ActionWatching.NumberOfGcdsUsed < 5)
            return false;

        if (onAoE)
            return true;

        if (GetStatusEffectRemainingTime(Buffs.OgiNamikiriReady) <= 8)
            return true;

        if (JustUsed(Higanbana, 8f))
            return true;

        float higanbanaRemaining = GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget);
        return JustUsed(Ikishoten, 20f) &&
               HasStatusEffect(Debuffs.Higanbana, CurrentTarget) &&
               higanbanaRemaining > 15;
    }

    private static bool NeedKenkiForSenei() =>
        LevelChecked(Senei) &&
        GetCooldownRemainingTime(Senei) < 7f;

    private static bool NeedKenkiRoomForIkishoten() =>
        LevelChecked(Ikishoten) &&
        !HasStatusEffect(Buffs.ZanshinReady) &&
        Kenki > 50 &&
        (ActionReady(Ikishoten) || GetCooldownRemainingTime(Ikishoten) <= GCD * 5);

    private static bool CanDumpKenki(int kenkiOvercapAmount = 50, bool holdForBurst = true)
    {
        if (Kenki >= 95)
            return true;

        if (HasStatusEffect(Buffs.ZanshinReady) &&
            LevelChecked(Zanshin) &&
            Kenki < 75)
            return false;

        if (holdForBurst && NeedKenkiForSenei() && Kenki < 70)
            return false;

        if (holdForBurst && ActionReady(Senei) && Kenki < 70)
            return false;

        if (NeedKenkiRoomForIkishoten() && !(holdForBurst && ActionReady(Senei)))
            return true;

        if (LevelChecked(Guren) && GetCooldownRemainingTime(Guren) <= GCD * 6)
            return Kenki >= 75;

        return Kenki >= kenkiOvercapAmount;
    }

    private static bool UseSenei() =>
        ActionReady(Senei) &&
        InActionRange(Senei) &&
        ActionWatching.NumberOfGcdsUsed >= 4 &&
        (!LevelChecked(TendoSetsugekka) ||
         HasStatusEffect(Buffs.Tendo) && SenCount >= 2 ||
         JustUsed(TendoSetsugekka, GCD * 3) ||
         JustUsed(TendoKaeshiSetsugekka, GCD * 3));

    private static bool UseGuren() =>
        ActionReady(Guren) && InActionRange(Guren);

    private static bool UseShinten(
        int executeThreshold = 1,
        int kenkiOvercapAmount = 50,
        bool holdForBurst = true) =>
        ActionReady(Shinten) &&
        InActionRange(Shinten) &&
        (GetTargetHPPercent() < executeThreshold ||
         CanDumpKenki(kenkiOvercapAmount, holdForBurst));

    private static bool UseKyuten(int kenkiOvercapAmount = 50, bool holdForBurst = true) =>
        ActionReady(Kyuten) &&
        InActionRange(Kyuten) &&
        CanDumpKenki(kenkiOvercapAmount, holdForBurst);

    private static bool UseKenki(ref uint actionID, bool onAoE, bool holdForBurst = true)
    {
        if (onAoE)
        {
            if (UseGuren())
            {
                actionID = Guren;
                return true;
            }

            if (UseKyuten(holdForBurst: holdForBurst))
            {
                actionID = Kyuten;
                return true;
            }

            return false;
        }

        if (UseSenei())
        {
            actionID = Senei;
            return true;
        }

        if (!LevelChecked(Senei) && UseGuren())
        {
            actionID = Guren;
            return true;
        }

        if (NeedKenkiRoomForIkishoten() &&
            !(holdForBurst && ActionReady(Senei)) &&
            UseShinten(holdForBurst: holdForBurst))
        {
            actionID = Shinten;
            return true;
        }

        if (UseShinten(holdForBurst: holdForBurst))
        {
            actionID = Shinten;
            return true;
        }

        return false;
    }

    private static uint DoBasicCombo(
        bool onAoE,
        bool useTrueNorth = true,
        bool useYukikaze = true,
        bool useKasha = true,
        bool useGekko = true,
        bool useOka = true,
        int trueNorthCharges = 0)
    {
        if (onAoE)
        {
            if (ComboTimer > 0 && ComboAction is Fuko or Fuga)
            {
                float fugetsuRemaining = GetStatusEffectRemainingTime(Buffs.Fugetsu);
                float fukaRemaining = GetStatusEffectRemainingTime(Buffs.Fuka);
                bool refreshFugetsu = fugetsuRemaining <= fukaRemaining;
                bool refreshFuka = fukaRemaining <= fugetsuRemaining;

                if (useOka &&
                    (!HasKa || !HasStatusEffect(Buffs.Fuka) ||
                     SenCount is 2 or 3 && refreshFuka) &&
                    LevelChecked(Oka))
                    return Oka;

                if (LevelChecked(Mangetsu) &&
                    HasStatusEffect(Buffs.Fuka) &&
                    (!HasGetsu || !HasStatusEffect(Buffs.Fugetsu) || !useOka || !LevelChecked(Oka) ||
                     SenCount is 2 or 3 && refreshFugetsu))
                    return Mangetsu;
            }

            return OriginalHook(Fuga);
        }

        if (ComboTimer > 0)
        {
            if (ComboAction is Hakaze or Gyofu)
            {
                float fugetsuRemaining = GetStatusEffectRemainingTime(Buffs.Fugetsu);
                float fukaRemaining = GetStatusEffectRemainingTime(Buffs.Fuka);
                bool refreshFugetsu = fugetsuRemaining <= fukaRemaining;
                bool refreshFuka = fukaRemaining <= fugetsuRemaining;

                if (!LevelChecked(Gekko))
                {
                    if (useKasha && LevelChecked(Shifu) &&
                        (!HasStatusEffect(Buffs.Fuka) ||
                         HasStatusEffect(Buffs.Fugetsu) && refreshFuka))
                        return Shifu;

                    if (useGekko && LevelChecked(Jinpu))
                        return Jinpu;

                    if (useKasha && LevelChecked(Shifu))
                        return Shifu;
                }

                if (useYukikaze &&
                    LevelChecked(Yukikaze) && !HasSetsu &&
                    (!useGekko || !LevelChecked(Gekko) || fugetsuRemaining > 7) &&
                    (!useKasha || !LevelChecked(Kasha) || fukaRemaining > 7))
                    return Yukikaze;

                if (useKasha &&
                    LevelChecked(Shifu) &&
                    ((OnTargetsFlank() || OnTargetsFront()) && !HasKa && LevelChecked(Kasha) ||
                     OnTargetsRear() && HasGetsu && LevelChecked(Kasha) ||
                     !HasStatusEffect(Buffs.Fuka) ||
                     SenCount is 3 && refreshFuka ||
                     !LevelChecked(Gekko)))
                    return Shifu;

                if (useGekko &&
                    LevelChecked(Jinpu) &&
                    (!LevelChecked(Kasha) && LevelChecked(Gekko) ||
                     (OnTargetsRear() || OnTargetsFront()) && !HasGetsu && LevelChecked(Gekko) ||
                     OnTargetsFlank() && HasKa && LevelChecked(Gekko) ||
                     !HasStatusEffect(Buffs.Fugetsu) ||
                     SenCount is 3 && refreshFugetsu))
                    return Jinpu;
            }

            if (useGekko && ComboAction is Jinpu && LevelChecked(Gekko))
                return WithTrueNorth(Gekko, OnTargetsRear(), useTrueNorth, trueNorthCharges);

            if (useKasha && ComboAction is Shifu && LevelChecked(Kasha))
                return WithTrueNorth(Kasha, OnTargetsFlank(), useTrueNorth, trueNorthCharges);
        }

        return OriginalHook(Hakaze);
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Lvl70.LevelChecked)
            return Lvl70;

        if (Lvl80.LevelChecked)
            return Lvl80;

        if (Lvl90.LevelChecked)
            return Lvl90;

        if (Lvl100.LevelChecked)
            return Lvl100;

        return WrathOpener.Dummy;
    }

    internal static SAMLvl70Opener Lvl70 = new();
    internal static SAMLvl80Opener Lvl80 = new();
    internal static SAMLvl90Opener Lvl90 = new();
    internal static SAMLvl100Opener Lvl100 = new();

    internal abstract class SAMOpenerBase : WrathOpener
    {
        public override Preset Preset => Preset.SAM_ST_Adv_Opener;

        internal override UserData ContentCheckConfig => SAM_Balance_Content;
        internal override bool IncludePot => SAM_ST_Opener_Potion;

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([1], () => CountdownRemaining - 13),
            ([2], () => CountdownRemaining - 5),
            ([3], () => CountdownRemaining - 0.5f)
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([2], () => !TargetNeedsPositionals())
        ];

        protected static bool SharedOpenerCooldowns() =>
            GetRemainingCharges(Role.TrueNorth) >= 1 &&
            IsOffCooldown(Ikishoten) &&
            SenCount is 0;
    }

    internal class SAMLvl70Opener : SAMOpenerBase
    {
        public override int MinOpenerLevel => 70;
        public override int MaxOpenerLevel => 70;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui, // 1
            Role.TrueNorth, // 2
            Gekko, // 3
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 4
            Kasha, // 5
            Ikishoten, // 6
            Yukikaze, // 7
            Shinten, // 8
            MidareSetsugekka, // 9
            Shinten, // 10
            Hakaze, // 11
            Guren, // 12
            Yukikaze, // 13
            Shinten, // 14
            Higanbana // 15
        ];

        public override bool HasCooldowns() =>
            IsOffCooldown(MeikyoShisui) &&
            IsOffCooldown(Guren) &&
            SharedOpenerCooldowns();
    }

    internal class SAMLvl80Opener : SAMOpenerBase
    {
        public override int MinOpenerLevel => 80;
        public override int MaxOpenerLevel => 80;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui, // 1
            Role.TrueNorth, // 2
            Gekko, // 3
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 4
            Ikishoten, // 5
            Kasha, // 6
            Yukikaze, // 7
            MidareSetsugekka, // 8
            Senei, // 9
            KaeshiSetsugekka, // 10
            MeikyoShisui, // 11
            Gekko, // 12
            Higanbana, // 13
            Gekko, // 14
            Kasha, // 15
            Hakaze, // 16
            Yukikaze, // 17
            MidareSetsugekka, // 18
            Shoha, // 19
            KaeshiSetsugekka // 20
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            IsOffCooldown(Senei) &&
            SharedOpenerCooldowns();
    }

    internal class SAMLvl90Opener : SAMOpenerBase
    {
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui, // 1
            Role.TrueNorth, // 2
            Gekko, // 3
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 4
            Ikishoten, // 5
            Kasha, // 6
            Yukikaze, // 7
            MidareSetsugekka, // 8
            Senei, // 9
            KaeshiSetsugekka, // 10
            MeikyoShisui, // 11
            Gekko, // 12
            Higanbana, // 13
            OgiNamikiri, // 14
            Shoha, // 15
            KaeshiNamikiri, // 16
            Kasha, // 17
            Gekko, // 18
            Hakaze, // 19
            Yukikaze, // 20
            MidareSetsugekka, // 21
            KaeshiSetsugekka // 22
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            IsOffCooldown(Senei) &&
            SharedOpenerCooldowns();
    }

    internal class SAMLvl100Opener : SAMOpenerBase
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui, // 1
            Role.TrueNorth, // 2
            Gekko, // 3
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)), // 4
            Kasha, // 5
            Ikishoten, // 6
            Yukikaze, // 7
            TendoSetsugekka, // 8
            Senei, // 9
            TendoKaeshiSetsugekka, // 10
            MeikyoShisui, // 11
            Gekko, // 12
            Zanshin, // 13
            Higanbana, // 14
            OgiNamikiri, // 15
            Shoha, // 16
            KaeshiNamikiri, // 17
            Kasha, // 18
            Shinten, // 19
            Gekko, // 20
            Gyoten, // 21
            Gyofu, // 22
            Yukikaze, // 23
            Shinten, // 24
            TendoSetsugekka, // 25
            Gyoten, // 26
            TendoKaeshiSetsugekka // 27
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([2], () => !TargetNeedsPositionals()),
            ([19, 24], () => !ActionReady(Shinten)),
            ([21], () => !ActionReady(Gyoten) || (int)SAM_ST_Opener_IncludeGyoten is 1 or 2),
            ([26], () => !ActionReady(Gyoten) || (int)SAM_ST_Opener_IncludeGyoten is 1 or 3),
            ([8, 25], () => SenCount is not 3 && !(SenCount is 2 && JustUsed(Yukikaze))),
            ([10, 27], () => !HasStatusEffect(Buffs.TsubameReady) && !JustUsed(TendoSetsugekka)),
            ([14], () => SenCount is not 1 && !(SenCount is 2 && JustUsed(Gekko)))
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            IsOffCooldown(Senei) &&
            SharedOpenerCooldowns();
    }

    #endregion

    #region Gauge

    private static float GCD =>
        GetAdjustedRecastTime(ActionType.Action, Hakaze) / 1000f;

    private static SAMGauge Gauge => GetJobGauge<SAMGauge>();

    private static bool HasEnhancedSenei =>
        TraitLevelChecked(Traits.EnhancedHissatsu);

    private static bool HasGetsu => Gauge.HasGetsu;

    private static bool HasSetsu => Gauge.HasSetsu;

    private static bool HasKa => Gauge.HasKa;

    private static byte Kenki => Gauge.Kenki;

    private static byte MeditationStacks => Gauge.MeditationStacks;

    private static Kaeshi Kaeshi => Gauge.Kaeshi;

    private static bool IsNamikiriReady => Kaeshi is Kaeshi.Namikiri;

    private static int SenCount =>
        (HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0);

    #endregion

    #region ID's

    public const uint
        Hakaze = 7477,
        Yukikaze = 7480,
        Gekko = 7481,
        Enpi = 7486,
        Jinpu = 7478,
        Kasha = 7482,
        Shifu = 7479,
        Mangetsu = 7484,
        Fuga = 7483,
        Oka = 7485,
        Higanbana = 7489,
        TenkaGoken = 7488,
        MidareSetsugekka = 7487,
        Shinten = 7490,
        Kyuten = 7491,
        Hagakure = 7495,
        Guren = 7496,
        Meditate = 7497,
        Senei = 16481,
        MeikyoShisui = 7499,
        Seigan = 7501,
        ThirdEye = 7498,
        Iaijutsu = 7867,
        TsubameGaeshi = 16483,
        KaeshiHiganbana = 16484,
        Shoha = 16487,
        Ikishoten = 16482,
        Fuko = 25780,
        OgiNamikiri = 25781,
        KaeshiNamikiri = 25782,
        Yaten = 7493,
        Gyoten = 7492,
        KaeshiSetsugekka = 16486,
        TendoGoken = 36965,
        TendoKaeshiSetsugekka = 36968,
        Zanshin = 36964,
        TendoSetsugekka = 36966,
        Tengentsu = 7498,
        Gyofu = 36963;

    public static class Buffs
    {
        public const ushort
            MeikyoShisui = 1233,
            EnhancedEnpi = 1236,
            EyesOpen = 1252,
            Meditate = 1231,
            OgiNamikiriReady = 2959,
            Fuka = 1299,
            Fugetsu = 1298,
            TsubameReady = 4216,
            TendoKaeshiSetsugekkaReady = 4218,
            KaeshiGokenReady = 3852,
            TendoKaeshiGokenReady = 4217,
            ZanshinReady = 3855,
            Tengentsu = 3853,
            Tendo = 3856;
    }

    public static class Debuffs
    {
        public const ushort
            Higanbana = 1228;
    }

    public static class Traits
    {
        public const ushort
            EnhancedHissatsu = 591,
            EnhancedMeikyoShishui = 443,
            EnhancedMeikyoShishui2 = 593;
    }

    #endregion
}
