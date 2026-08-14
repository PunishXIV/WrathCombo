using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.SAM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Data.ActionWatching;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    #region Iaijutsu

    private static bool CanIaijutsu(
        bool useHiganbana,
        bool useTenkaGoken,
        bool useMidare,
        int higanbanaHpThreshold = 0,
        double higanbanaDotRefresh = 15)
    {
        if (LevelChecked(Iaijutsu) && InActionRange(OriginalHook(Iaijutsu)))
        {
            if (useHiganbana &&
                SenCount is 1 &&
                CanHiganbana(higanbanaHpThreshold, higanbanaDotRefresh))
                return true;

            if (useTenkaGoken &&
                SenCount is 2 &&
                !LevelChecked(MidareSetsugekka))
                return true;

            if (useMidare &&
                SenCount is 3 &&
                LevelChecked(MidareSetsugekka) && !HasStatusEffect(Buffs.TsubameReady))
                return true;
        }
        return false;
    }

    #endregion

    #region Basic Combo

    private static uint WithTrueNorth(uint action, bool onPositional, bool useTrueNorth, int trueNorthCharges) =>
        !onPositional &&
        useTrueNorth &&
        Role.CanTrueNorth() &&
        GetRemainingCharges(Role.TrueNorth) > trueNorthCharges
            ? Role.TrueNorth
            : action;

    private static bool TryAoEComboFinisher(out uint action, bool useOka = true)
    {
        action = 0;

        if (useOka && LevelChecked(Oka) &&
            (!HasKa || !HasStatusEffect(Buffs.Fuka) ||
             SenCount is 2 or 3 && ShouldRefreshFuka))
        {
            action = Oka;
            return true;
        }

        if (LevelChecked(Mangetsu) &&
            HasStatusEffect(Buffs.Fuka) &&
            (!HasGetsu || !HasStatusEffect(Buffs.Fugetsu) || !useOka || !LevelChecked(Oka) ||
             SenCount is 2 or 3 && ShouldRefreshFugetsu))
        {
            action = Mangetsu;
            return true;
        }

        return false;
    }

    private static uint ContinueBasicCombo(
        bool onAoE = false,
        bool useTrueNorth = true,
        bool useYukikaze = true,
        bool useKasha = true,
        bool useGekko = true,
        bool useOka = true,
        int trueNorthCharges = 0)
    {
        if (onAoE)
        {
            if (ComboTimer is 0)
                return OriginalHook(Fuga);

            if (ComboAction is Fuko or Fuga &&
                TryAoEComboFinisher(out uint finisher, useOka))
                return finisher;

            return OriginalHook(Fuga);
        }

        if (ComboTimer > 0)
        {
            if (ComboAction is Hakaze or Gyofu)
            {
                if (useYukikaze &&
                    !HasSetsu && LevelChecked(Yukikaze) &&
                    (GetStatusEffectRemainingTime(Buffs.Fugetsu) > 7 || !useGekko || !LevelChecked(Kasha)) &&
                    (GetStatusEffectRemainingTime(Buffs.Fuka) > 7 || !useKasha || !LevelChecked(Kasha)))
                    return Yukikaze;

                if (useKasha &&
                    LevelChecked(Shifu) &&
                    ((OnTargetsFlank() || OnTargetsFront()) && !HasKa && LevelChecked(Kasha) ||
                     OnTargetsRear() && HasGetsu && LevelChecked(Kasha) ||
                     !HasStatusEffect(Buffs.Fuka) ||
                     SenCount is 3 && ShouldRefreshFuka ||
                     !LevelChecked(Gekko)))
                    return Shifu;

                if (useGekko &&
                    LevelChecked(Jinpu) &&
                    (!LevelChecked(Gekko) ||
                     !LevelChecked(Kasha) && LevelChecked(Gekko) ||
                     (OnTargetsRear() || OnTargetsFront()) && !HasGetsu && LevelChecked(Gekko) ||
                     OnTargetsFlank() && HasKa && LevelChecked(Gekko) ||
                     !HasStatusEffect(Buffs.Fugetsu) ||
                     SenCount is 3 && ShouldRefreshFugetsu))
                    return Jinpu;
            }

            if (ComboAction is Jinpu && LevelChecked(Gekko))
                return WithTrueNorth(Gekko, OnTargetsRear(), useTrueNorth, trueNorthCharges);

            if (ComboAction is Shifu && LevelChecked(Kasha))
                return WithTrueNorth(Kasha, OnTargetsFlank(), useTrueNorth, trueNorthCharges);
        }

        return OriginalHook(Hakaze);
    }

    #endregion

    #region Higanbana

    private static bool CanHiganbana(int hpThreshold = 0, double dotRefresh = 15)
    {
        float dotRemaining = GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget);

        return ActionReady(Higanbana) && SenCount is 1 &&
               CanApplyStatus(CurrentTarget, Debuffs.Higanbana) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh &&
               HasStatusEffect(Buffs.Fuka) && HasStatusEffect(Buffs.Fugetsu) &&
               (!HasEnhancedSenei ||
                JustUsed(Senei, 35f) || JustUsed(Ikishoten, 35f) ||
                !HasStatusEffect(Debuffs.Higanbana, CurrentTarget));
    }

    private static int HiganbanaHPThreshold()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? SAM_ST_HiganbanaBossHPOption : SAM_ST_HiganbanaBossAddsHPOption;

        return SAM_ST_HiganbanaTrashHPOption;
    }

    #endregion

    #region Misc

    private static bool ShouldRefreshFugetsu =>
        GetStatusEffectRemainingTime(Buffs.Fugetsu) <=
        GetStatusEffectRemainingTime(Buffs.Fuka);

    private static bool ShouldRefreshFuka =>
        GetStatusEffectRemainingTime(Buffs.Fuka) <=
        GetStatusEffectRemainingTime(Buffs.Fugetsu);

    private static bool HasEnhancedSenei =>
        TraitLevelChecked(Traits.EnhancedHissatsu);

    private static int SenCount =>
        GetSenCount();

    private static bool CanThirdEye() =>
        ActionReady(OriginalHook(ThirdEye)) &&
        (GroupDamageIncoming(2f) || !IsInParty());

    private static bool CanMeditate() =>
        ActionReady(Meditate) &&
        !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(SAM_ST_MeditateTimeStill) &&
        InCombat() && !HasBattleTarget();

    private static bool CanShoha(double higanbanaDotRefresh = 15) =>
        ActionReady(Shoha) &&
        MeditationStacks is 3 &&
        InActionRange(Shoha) &&
        (SenCount is 3 ||
         SenCount is 1 && GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget) < higanbanaDotRefresh ||
         HasStatusEffect(Buffs.OgiNamikiriReady) ||
         HasEnhancedSenei && JustUsed(Senei, 30f) ||
         !HasEnhancedSenei && JustUsed(KaeshiSetsugekka, 20f));

    private static bool CanUseShinten() =>
        ActionReady(Shinten) && InActionRange(Shinten);

    private static bool ShouldSpendKenkiUrgent() =>
        Kenki is 100 && ComboAction == OriginalHook(Gyofu) ||
        Kenki >= 95 && (ComboAction is Jinpu or Shifu || SenCount is 3) ||
        Kenki >= 80 && !HasSetsu && (JustUsed(MidareSetsugekka, 5f) || JustUsed(Higanbana, 5f));

    private static bool ShouldUseSenei(int kenkiOvercapAmount)
    {
        if (!HasEnhancedSenei || HasStatusEffect(Buffs.ZanshinReady))
            return false;

        return GetCooldownRemainingTime(Senei) < GCDTotal * 2 && Kenki >= 90 ||
               JustUsed(Senei, 20f) && !JustUsed(Ikishoten) ||
               Kenki >= 95 && JustUsed(MeikyoShisui) ||
               Kenki >= 90 && JustUsed(MeikyoShisui) && ComboAction is Yukikaze ||
               Kenki >= 65 && SenCount >= 2 &&
               (HasStatusEffect(Buffs.Tendo) || JustUsed(TendoKaeshiSetsugekka, 5f)) ||
               GetCooldownRemainingTime(Senei) >= 25 && Kenki >= kenkiOvercapAmount;
    }

    private static bool ShouldSpendKenkiPreEnhanced(int kenkiOvercapAmount) =>
        !HasEnhancedSenei &&
        (GetCooldownRemainingTime(Ikishoten) > 10 && Kenki >= kenkiOvercapAmount ||
         GetCooldownRemainingTime(Ikishoten) <= 10 && Kenki > 50);

    private static bool CanShinten(int executeThreshold = 1, int kenkiOvercapAmount = 65)
    {
        if (!CanUseShinten())
            return false;

        if (GetTargetHPPercent() < executeThreshold)
            return true;

        return ShouldSpendKenkiUrgent() ||
               ShouldUseSenei(kenkiOvercapAmount) ||
               ShouldSpendKenkiPreEnhanced(kenkiOvercapAmount);
    }

    #endregion

    #region Combo Resolution

    private static uint DoCombo(
        bool onAoE,
        bool useTrueNorth = true,
        bool useYukikaze = true,
        bool useKasha = true,
        bool useGekko = true,
        bool useOka = true,
        int trueNorthCharges = 0) =>
        HasStatusEffect(Buffs.MeikyoShisui)
            ? DoMeikyoCombo(onAoE, useTrueNorth, useYukikaze, useKasha, useGekko, useOka, trueNorthCharges)
            : ContinueBasicCombo(onAoE, useTrueNorth, useYukikaze, useKasha, useGekko, useOka, trueNorthCharges);

    private static bool CanAoESenGCD(
        out uint action,
        bool allowTsubame = true,
        bool allowIaijutsu = true,
        bool onlyWhenStationary = false)
    {
        action = 0;

        if (allowTsubame && LevelChecked(TenkaGoken) && LevelChecked(TsubameGaeshi) &&
            (HasStatusEffect(Buffs.KaeshiGokenReady) ||
             HasStatusEffect(Buffs.TsubameReady) ||
             HasStatusEffect(Buffs.TendoKaeshiGokenReady)))
        {
            action = OriginalHook(TsubameGaeshi);
            return true;
        }

        if (allowIaijutsu && LevelChecked(TenkaGoken) &&
            (!onlyWhenStationary || !IsMoving()) &&
            (OriginalHook(Iaijutsu) is TenkaGoken or MidareSetsugekka or TendoGoken))
        {
            action = OriginalHook(Iaijutsu);
            return true;
        }

        return false;
    }

    private static bool CanStIaijutsu(
        bool useHiganbana,
        bool useTenkaGoken,
        bool useMidare,
        bool onlyWhenStationary = false,
        int higanbanaHpThreshold = 0,
        double higanbanaDotRefresh = 15) =>
        (!onlyWhenStationary || !IsMoving()) &&
        CanIaijutsu(useHiganbana, useTenkaGoken, useMidare, higanbanaHpThreshold, higanbanaDotRefresh);

    #endregion

    #region AoE Weaves

    private static bool CanAoEHagakure() =>
        OriginalHook(Iaijutsu) is MidareSetsugekka or TendoSetsugekka && LevelChecked(Hagakure);

    private static bool CanAoEMeikyo() =>
        ActionReady(MeikyoShisui) && !HasStatusEffect(Buffs.MeikyoShisui) &&
        !JustUsed(MeikyoShisui) && ComboTimer is 0;

    private static bool CanAoEIkishotenKenki(out uint action)
    {
        action = 0;

        if (!ActionReady(Ikishoten) || HasStatusEffect(Buffs.ZanshinReady))
            return false;

        action = Kenki >= 50 ? Kyuten : Ikishoten;
        return true;
    }

    private static bool CanAoEZanshin() =>
        ActionReady(Zanshin) && HasStatusEffect(Buffs.ZanshinReady);

    private static bool CanAoEGuren() => ActionReady(Guren);

    private static bool CanAoEShoha() =>
        ActionReady(Shoha) && MeditationStacks is 3;

    private static bool CanAoEKyuten(float kenkiThreshold = 50) =>
        ActionReady(Kyuten) && Kenki >= kenkiThreshold && !ActionReady(Guren);

    private static bool CanAoEOgiNamikiri(bool onlyWhenStationary = false) =>
        ActionReady(OriginalHook(OgiNamikiri)) &&
        (onlyWhenStationary
            ? !IsMoving() && (HasStatusEffect(Buffs.OgiNamikiriReady) || IsNamikiriReady)
            : IsNamikiriReady || HasStatusEffect(Buffs.OgiNamikiriReady) && !IsMoving());

    #endregion

    #region Meikyo

    private static bool CanMeikyo(int meikyoExecuteThreshold = 5)
    {
        if (!ActionReady(MeikyoShisui) || HasStatusEffect(Buffs.Tendo) ||
            HasStatusEffect(Buffs.MeikyoShisui) || JustUsed(MeikyoShisui))
            return false;

        if (TargetIsBoss() && GetTargetHPPercent() < meikyoExecuteThreshold &&
            (JustUsed(Yukikaze, 2f) || JustUsed(Gekko, 2f) || JustUsed(Kasha, 2f)))
            return true;

        if (!(JustUsed(Yukikaze, 2f) ||
              HasSetsu && (JustUsed(Gekko, 2f) || JustUsed(Kasha, 2f) ||
                           JustUsed(KaeshiSetsugekka, 2f) && SenCount is 3)))
            return false;

        if (InBossEncounter())
        {
            if (HasEnhancedSenei)
            {
                if (GetRemainingCharges(MeikyoShisui) >= 1 &&
                    JustUsed(KaeshiNamikiri, 10f) &&
                    GetCooldownChargeRemainingTime(MeikyoShisui) is >= 35 and <= 43)
                    return true;

                if ((SenCount is 0 && GetCooldownRemainingTime(Senei) <= 14 && JustUsed(MidareSetsugekka, 5f)) ||
                    (SenCount is 0 && GetCooldownRemainingTime(Senei) <= 11 && JustUsed(Higanbana, 5f)) ||
                    SenCount is 1 && GetCooldownRemainingTime(Senei) <= 9 ||
                    SenCount is 2 && GetCooldownRemainingTime(Senei) <= 7 ||
                    SenCount is 3 && GetCooldownRemainingTime(Senei) <= 5)
                    return true;
            }
            else if (GetCooldownRemainingTime(Senei) <= GCDTotal ||
                     GetCooldownRemainingTime(Senei) is > 50 and < 65)
                return true;
        }
        else if (SenCount is 3)
            return true;

        return false;
    }

    private static uint DoMeikyoCombo(
        bool onAoE = false,
        bool useTrueNorth = true,
        bool useYukikaze = true,
        bool useKasha = true,
        bool useGekko = true,
        bool useOka = true,
        int trueNorthCharges = 0)
    {
        if (onAoE)
        {
            if (TryAoEComboFinisher(out uint finisher, useOka))
                return finisher;

            return OriginalHook(Fuga);
        }

        if (useYukikaze &&
            LevelChecked(Yukikaze) && !HasSetsu &&
            (HasKa || !useGekko) &&
            (HasGetsu || !useKasha))
            return Yukikaze;

        if (useGekko &&
            LevelChecked(Gekko) &&
            (!LevelChecked(Kasha) ||
             !HasStatusEffect(Buffs.Fugetsu) ||
             (OnTargetsRear() || OnTargetsFront()) && !HasGetsu ||
             OnTargetsFlank() && HasKa))
            return WithTrueNorth(Gekko, OnTargetsRear(), useTrueNorth, trueNorthCharges);

        if (useKasha &&
            LevelChecked(Kasha) &&
            (!HasStatusEffect(Buffs.Fuka) ||
             (OnTargetsFlank() || OnTargetsFront()) && !HasKa ||
             OnTargetsRear() && HasGetsu))
            return WithTrueNorth(Kasha, OnTargetsFlank(), useTrueNorth, trueNorthCharges);

        return OriginalHook(Hakaze);
    }

    #endregion

    #region Burst Management

    private static bool CanPrepullMeikyo(bool requireNotJustUsed = false) =>
        ActionReady(MeikyoShisui) &&
        !HasStatusEffect(Buffs.MeikyoShisui) &&
        !InCombat() && HasBattleTarget() &&
        (!requireNotJustUsed || !JustUsed(MeikyoShisui));

    private static bool TryFeatureKenkiOvercap(out uint action, bool enabled, int amount, uint spender)
    {
        action = 0;

        if (!enabled || !CanWeave() || Kenki < amount || !LevelChecked(spender))
            return false;

        action = OriginalHook(spender);
        return true;
    }

    private static bool CanIkishoten() =>
        ActionReady(Ikishoten) &&
        !HasStatusEffect(Buffs.ZanshinReady) && Kenki <= 50 &&
        (NumberOfGcdsUsed >= 2 ||
         JustUsed(Senei, 15f) ||
         !LevelChecked(Senei));

    private static bool CanSenei() =>
        ActionReady(Senei) && NumberOfGcdsUsed >= 4 &&
        InActionRange(Senei) &&
        (LevelChecked(TendoKaeshiSetsugekka) &&
         (SenCount >= 2 && HasStatusEffect(Buffs.Tendo) ||
          JustUsed(TendoSetsugekka, 15f) ||
          JustUsed(TendoKaeshiSetsugekka, 15f)) ||
         !LevelChecked(TendoKaeshiSetsugekka));

    private static bool CanTsubame() =>
        ActionReady(OriginalHook(TsubameGaeshi)) &&
        (HasStatusEffect(Buffs.TendoKaeshiSetsugekkaReady) ||
         HasStatusEffect(Buffs.TsubameReady)) &&
        (GetStatusEffectRemainingTime(Buffs.TsubameReady) < 5 ||
         SenCount is 3 ||
         HasEnhancedSenei && GetCooldownRemainingTime(Senei) > 33);

    private static bool CanZanshin() =>
        ActionReady(Zanshin) &&
        InActionRange(Zanshin) &&
        HasStatusEffect(Buffs.ZanshinReady) &&
        (GetStatusEffectRemainingTime(Buffs.ZanshinReady) <= 8 ||
         JustUsed(Senei, 20f));

    private static bool CanOgiNamikiri(
        bool onlyWhenStationary = false,
        bool respectMovementOption = false,
        bool useHiganbanaBurstRules = true,
        bool higanbanaBossOnly = false)
    {
        if (IsNamikiriReady)
            return true;

        if (ActionReady(OriginalHook(OgiNamikiri)) && InActionRange(OriginalHook(OgiNamikiri)) &&
            HasStatusEffect(Buffs.OgiNamikiriReady) && NumberOfGcdsUsed >= 5 &&
            (onlyWhenStationary
                ? !IsMoving()
                : !respectMovementOption || !IsMoving()))
        {
            if (GetStatusEffectRemainingTime(Buffs.OgiNamikiriReady) <= 8)
                return true;

            if (!onlyWhenStationary &&
                !useHiganbanaBurstRules && JustUsed(Ikishoten, 20f))
                return true;

            if (JustUsed(TendoKaeshiSetsugekka, 20f) &&
                GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget) > 8)
                return true;

            if (!onlyWhenStationary &&
                higanbanaBossOnly && !TargetIsBoss())
                return true;
        }
        return false;
    }

    private static bool TryKenkiSpender(
        out uint action,
        bool useZanshin = false,
        bool useSenei = false,
        bool useShinten = false)
    {
        action = 0;

        if (useZanshin &&
            ActionReady(Zanshin) && HasStatusEffect(Buffs.ZanshinReady))
        {
            action = Zanshin;
            return true;
        }

        if (useSenei &&
            ActionReady(Senei) && InActionRange(Senei))
        {
            action = Senei;
            return true;
        }

        if (useShinten &&
            ActionReady(Shinten) && InActionRange(Shinten))
        {
            action = Shinten;
            return true;
        }

        return false;
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
        public override Preset Preset => Preset.SAM_ST_Opener;

        internal override UserData ContentCheckConfig => SAM_Balance_Content;
        internal override bool IncludePot => SAM_Opener_Potion;

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([1], () => CountdownRemaining - 13),
            ([2], () => CountdownRemaining - 5)
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
            ([21], () => !ActionReady(Gyoten) || (int)SAM_Opener_IncludeGyoten is 1 or 2),
            ([26], () => !ActionReady(Gyoten) || (int)SAM_Opener_IncludeGyoten is 1 or 3),
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

    private static SAMGauge Gauge => GetJobGauge<SAMGauge>();

    private static bool HasGetsu => Gauge.HasGetsu;

    private static bool HasSetsu => Gauge.HasSetsu;

    private static bool HasKa => Gauge.HasKa;

    private static byte Kenki => Gauge.Kenki;

    private static byte MeditationStacks => Gauge.MeditationStacks;

    private static Kaeshi Kaeshi => Gauge.Kaeshi;

    private static bool IsNamikiriReady => Kaeshi is Kaeshi.Namikiri;

    private static int GetSenCount()
    {
        int senCount = 0;

        if (HasGetsu)
            senCount++;

        if (HasSetsu)
            senCount++;

        if (HasKa)
            senCount++;

        return senCount;
    }

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
