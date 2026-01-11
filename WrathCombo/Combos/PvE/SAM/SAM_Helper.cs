using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager;
using static WrathCombo.Combos.PvE.SAM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Data.ActionWatching;
using ActionType = FFXIVClientStructs.FFXIV.Client.Game.ActionType;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    #region Basic Combo

    private static uint DoBasicCombo(uint actionId, bool useTrueNorth = true, bool simpleMode = false)
    {
        if (ComboTimer > 0)
        {
            if (ComboAction is Hakaze or Gyofu)
            {
                if ((simpleMode || IsEnabled(Preset.SAM_ST_Yukikaze)) &&
                    !HasSetsu && LevelChecked(Yukikaze) &&
                    (GetStatusEffectRemainingTime(Buffs.Fugetsu) > 7 || IsNotEnabled(Preset.SAM_ST_Gekko)) &&
                    (GetStatusEffectRemainingTime(Buffs.Fuka) > 7 || IsNotEnabled(Preset.SAM_ST_Kasha)))
                    return Yukikaze;

                if ((simpleMode || IsEnabled(Preset.SAM_ST_Kasha)) &&
                    LevelChecked(Shifu) &&
                    ((OnTargetsFlank() || OnTargetsFront()) && !HasKa ||
                     OnTargetsRear() && HasGetsu ||
                     !HasStatusEffect(Buffs.Fuka) ||
                     SenCount is 3 && RefreshFuka))
                    return Shifu;

                if ((simpleMode || IsEnabled(Preset.SAM_ST_Gekko)) &&
                    LevelChecked(Jinpu) &&
                    ((OnTargetsRear() || OnTargetsFront()) && !HasGetsu ||
                     OnTargetsFlank() && HasKa ||
                     !HasStatusEffect(Buffs.Fugetsu) ||
                     SenCount is 3 && RefreshFugetsu))
                    return Jinpu;
            }

            if (ComboAction is Jinpu && LevelChecked(Gekko))
                return !OnTargetsRear() &&
                       Role.CanTrueNorth() &&
                       useTrueNorth
                    ? Role.TrueNorth
                    : Gekko;

            if (ComboAction is Shifu && LevelChecked(Kasha))
                return !OnTargetsFlank() &&
                       Role.CanTrueNorth() &&
                       useTrueNorth
                    ? Role.TrueNorth
                    : Kasha;
        }
        return actionId;
    }

    #endregion

    #region Iaijutsu

    private static bool CanUseIaijutsu(bool useHiganbana, bool useTenkaGoken, bool useMidare)
    {
        if (LevelChecked(Iaijutsu) && InActionRange(OriginalHook(Iaijutsu)))
        {
            //Higanbana
            if (useHiganbana &&
                SenCount is 1 &&
                CanUseHiganbana())
                return true;

            //Tenka Goken
            if (useTenkaGoken &&
                SenCount is 2 &&
                !LevelChecked(MidareSetsugekka))
                return true;

            //Midare Setsugekka
            if (useMidare &&
                SenCount is 3 &&
                LevelChecked(MidareSetsugekka) && !HasStatusEffect(Buffs.TsubameReady))
                return true;
        }
        return false;
    }

    #endregion

    #region Rescourses

    private static class SAMKenki
    {
        internal static int Zanshin => GetResourceCost(SAM.Zanshin);

        internal static int Senei => GetResourceCost(SAM.Senei);

        internal static int Shinten => GetResourceCost(SAM.Shinten);

        internal static int Gyoten => GetResourceCost(SAM.Gyoten);
    }

    #endregion

    #region Higanbana

    private static bool CanUseHiganbana()
    {
        int hpThreshold = IsNotEnabled(Preset.SAM_ST_SimpleMode) ? ComputeHpThresholdHiganbana() : 0;
        double dotRefresh = IsNotEnabled(Preset.SAM_ST_SimpleMode) ? SAM_ST_HiganbanaRefresh : 15;
        float dotRemaining = GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget);

        return ActionReady(Higanbana) && SenCount is 1 &&
               CanApplyStatus(CurrentTarget, Debuffs.Higanbana) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh &&
               HasStatusEffect(Buffs.Fuka) && HasStatusEffect(Buffs.Fugetsu) &&
               (EnhancedSenei && (JustUsed(Senei, 35f) || JustUsed(Ikishoten, 35f) || !HasStatusEffect(Debuffs.Higanbana, CurrentTarget)) ||
                !EnhancedSenei);
    }

    private static int ComputeHpThresholdHiganbana()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? SAM_ST_HiganbanaBossOption : SAM_ST_HiganbanaBossAddsOption;

        return SAM_ST_HiganbanaTrashOption;
    }

    #endregion

    #region Misc

    private static bool RefreshFugetsu =>
        GetStatusEffectRemainingTime(Buffs.Fugetsu) <=
        GetStatusEffectRemainingTime(Buffs.Fuka);

    private static bool RefreshFuka =>
        GetStatusEffectRemainingTime(Buffs.Fuka) <=
        GetStatusEffectRemainingTime(Buffs.Fugetsu);

    private static bool EnhancedSenei =>
        TraitLevelChecked(Traits.EnhancedHissatsu);

    private static int SenCount =>
        GetSenCount();

    private static bool CanUseThirdEye =>
        ActionReady(OriginalHook(ThirdEye)) &&
        (GroupDamageIncoming(2f) || !IsInParty());

    //Auto Meditate
    private static bool CanUseMeditate =>
        ActionReady(Meditate) &&
        !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(SAM_ST_MeditateTimeStill) &&
        InCombat() && !HasBattleTarget();

    #endregion

    #region Meikyo

    private static bool CanMeikyo()
    {
        float gcd = GetAdjustedRecastTime(ActionType.Action, Hakaze) / 100f;

        if (ActionReady(MeikyoShisui) &&
            !HasStatusEffect(Buffs.Tendo) &&
            !HasStatusEffect(Buffs.MeikyoShisui) &&
            (JustUsed(Yukikaze, 2f) || HasSetsu && (JustUsed(Gekko, 2f) || JustUsed(Kasha, 2f))))
        {
            if (InBossEncounter())
            {
                switch (EnhancedSenei)
                {
                    case true when GetRemainingCharges(MeikyoShisui) is 1 &&
                                   GetCooldownChargeRemainingTime(MeikyoShisui) < GetCooldownRemainingTime(Senei) - 10:

                    case true when SenCount is 0 && GetCooldownRemainingTime(Senei) <= 14 && JustUsed(MidareSetsugekka, 5f) ||
                                   SenCount is 0 && GetCooldownRemainingTime(Senei) <= 10 && JustUsed(Higanbana, 5f) ||
                                   SenCount is 1 && GetCooldownRemainingTime(Senei) <= 8 ||
                                   SenCount is 2 && GetCooldownRemainingTime(Senei) <= 6 ||
                                   SenCount is 3 && GetCooldownRemainingTime(Senei) <= 4:

                    // Pre 94
                    case false when
                        GetCooldownRemainingTime(Senei) <= gcd ||
                        GetCooldownRemainingTime(Senei) is > 50 and < 65:
                        return true;
                }
            }

            if (!InBossEncounter() && SenCount is 3)
                return true;
        }

        return false;
    }

    private static uint DoMeikyoCombo(uint actionId, bool useTrueNorth = true, bool simpleMode = false)
    {
        if ((simpleMode || IsEnabled(Preset.SAM_ST_Yukikaze)) &&
            LevelChecked(Yukikaze) && !HasSetsu &&
            (HasKa || IsNotEnabled(Preset.SAM_ST_Gekko)) &&
            (HasGetsu || IsNotEnabled(Preset.SAM_ST_Kasha)))
            return Yukikaze;

        if ((simpleMode || IsEnabled(Preset.SAM_ST_Gekko)) &&
            LevelChecked(Gekko) &&
            (!HasStatusEffect(Buffs.Fugetsu) ||
             (OnTargetsRear() || OnTargetsFront()) && !HasGetsu ||
             OnTargetsFlank() && HasKa))
            return !OnTargetsRear() &&
                   Role.CanTrueNorth() &&
                   useTrueNorth
                ? Role.TrueNorth
                : Gekko;

        if ((simpleMode || IsEnabled(Preset.SAM_ST_Kasha)) &&
            LevelChecked(Kasha) &&
            (!HasStatusEffect(Buffs.Fuka) ||
             (OnTargetsFlank() || OnTargetsFront()) && !HasKa ||
             OnTargetsRear() && HasGetsu))
            return !OnTargetsFlank() &&
                   Role.CanTrueNorth() &&
                   useTrueNorth
                ? Role.TrueNorth
                : Kasha;

        return actionId;
    }

    #endregion

    #region Burst Management

    private static bool CanIkishoten() =>
        ActionReady(Ikishoten) &&
        !HasStatusEffect(Buffs.ZanshinReady) && Kenki <= 50 &&
        (NumberOfGcdsUsed is 2 ||
         JustUsed(Senei, 15f) ||
         !LevelChecked(Senei));

    private static bool CanSenei() =>
        ActionReady(Senei) && NumberOfGcdsUsed >= 4 &&
        InActionRange(Senei) &&
        (LevelChecked(TendoKaeshiSetsugekka) &&
         (SenCount is 3 && HasStatusEffect(Buffs.Tendo) || JustUsed(TendoSetsugekka, 15f)) ||
         !LevelChecked(TendoKaeshiSetsugekka));

    private static bool CanTsubame() =>
        LevelChecked(TsubameGaeshi) && ActionReady(TsubameGaeshi) &&
        (HasStatusEffect(Buffs.TendoKaeshiSetsugekkaReady) ||
         HasStatusEffect(Buffs.TsubameReady)) &&
        (SenCount is 3 ||
         EnhancedSenei && GetCooldownRemainingTime(Senei) > 33 ||
         GetStatusEffectRemainingTime(Buffs.TsubameReady) < 5);

    private static bool CanShoha() =>
        ActionReady(Shoha) && MeditationStacks is 3 &&
        InActionRange(Shoha) &&
        (MeditationStacks is 3 && SenCount is 3 ||
         MeditationStacks is 3 && HasStatusEffect(Buffs.OgiNamikiriReady) ||
         EnhancedSenei && JustUsed(Senei, 20f) ||
         !EnhancedSenei && JustUsed(KaeshiSetsugekka, 10f));

    //TODO Buffcheck
    private static bool CanZanshin() =>
        ActionReady(Zanshin) && Kenki >= SAMKenki.Zanshin &&
        InActionRange(Zanshin) && HasStatusEffect(Buffs.ZanshinReady) &&
        (JustUsed(Senei, 20f) || GetStatusEffectRemainingTime(Buffs.ZanshinReady) <= 8);

    private static bool CanShinten()
    {
        int shintenTreshhold = SAM_ST_ExecuteThreshold;
        float gcd = GetCooldown(OriginalHook(Hakaze)).CooldownTotal;

        if (ActionReady(Shinten) && Kenki >= SAMKenki.Shinten && InActionRange(Shinten))
        {
            if (GetTargetHPPercent() < shintenTreshhold)
                return true;

            if (Kenki is 100 && ComboAction == OriginalHook(Gyofu) ||
                Kenki >= 95 && (ComboAction is Jinpu or Shifu || SenCount is 3) ||
                Kenki >= 80 && !HasSetsu && (JustUsed(MidareSetsugekka, 5f) || JustUsed(Higanbana, 5f)))
                return true;

            if (EnhancedSenei &&
                !HasStatusEffect(Buffs.ZanshinReady))
            {
                if (GetCooldownRemainingTime(Senei) < gcd * 2 &&
                    Kenki >= 90)
                    return true;

                if (JustUsed(Senei, 20f) &&
                    !JustUsed(Ikishoten))
                    return true;

                if (Kenki >= 95 && JustUsed(MeikyoShisui))
                    return true;

                if (Kenki >= 90 && JustUsed(MeikyoShisui) && ComboAction is Yukikaze)
                    return true;

                if (Kenki >= 65 && SenCount >= 2 && (HasStatusEffect(Buffs.Tendo) || JustUsed(TendoKaeshiSetsugekka, 5f)))
                    return true;

                if (GetCooldownRemainingTime(Senei) >= 25 &&
                    Kenki >= SAM_ST_KenkiOvercapAmount)
                    return true;
            }

            if (!EnhancedSenei)
            {
                if (GetCooldownRemainingTime(Ikishoten) > 10 && Kenki >= SAM_ST_KenkiOvercapAmount)
                    return true;

                if (GetCooldownRemainingTime(Ikishoten) <= 10 && Kenki > 50)
                    return true;
            }
        }
        return false;
    }

    private static bool CanOgi(bool simpleMode = false)
    {
        if (NamikiriReady)
            return true;

        if (ActionReady(OgiNamikiri) && InActionRange(OriginalHook(OgiNamikiri)) &&
            HasStatusEffect(Buffs.OgiNamikiriReady) && NumberOfGcdsUsed >= 5)
        {
            if (GetStatusEffectRemainingTime(Buffs.OgiNamikiriReady) <= 8)
                return true;

            if (!simpleMode &&
                IsNotEnabled(Preset.SAM_ST_CDs_UseHiganbana) && JustUsed(Ikishoten, 20f))
                return true;

            if (JustUsed(TendoKaeshiSetsugekka, 20f) &&
                GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget) > 8)
                return true;

            if (!simpleMode &&
                SAM_ST_HiganbanaBossOption == 1 && !TargetIsBoss())
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

    internal class SAMLvl70Opener : WrathOpener
    {
        public override int MinOpenerLevel => 70;

        public override int MaxOpenerLevel => 70;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui,
            Role.TrueNorth, //2
            Gekko,
            Kasha,
            Ikishoten,
            Yukikaze,
            Shinten,
            MidareSetsugekka,
            Shinten,
            Hakaze,
            Guren,
            Yukikaze,
            Shinten,
            Higanbana
        ];

        internal override UserData ContentCheckConfig => SAM_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => SAM_Opener_PrePullDelay)
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([2], 11, () => !TargetNeedsPositionals())
        ];

        public override Preset Preset => Preset.SAM_ST_Opener;

        public override bool HasCooldowns() =>
            IsOffCooldown(MeikyoShisui) &&
            GetRemainingCharges(Role.TrueNorth) >= 1 &&
            IsOffCooldown(Guren) &&
            IsOffCooldown(Ikishoten) &&
            SenCount is 0;
    }

    internal class SAMLvl80Opener : WrathOpener
    {
        public override int MinOpenerLevel => 80;

        public override int MaxOpenerLevel => 80;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui,
            Role.TrueNorth, //2
            Gekko,
            Ikishoten,
            Kasha,
            Yukikaze,
            MidareSetsugekka,
            Senei,
            KaeshiSetsugekka,
            MeikyoShisui,
            Gekko,
            Higanbana,
            Gekko,
            Kasha,
            Hakaze,
            Yukikaze,
            MidareSetsugekka,
            Shoha,
            KaeshiSetsugekka
        ];

        internal override UserData ContentCheckConfig => SAM_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => SAM_Opener_PrePullDelay)
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([2], 11, () => !TargetNeedsPositionals())
        ];

        public override Preset Preset => Preset.SAM_ST_Opener;

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            GetRemainingCharges(Role.TrueNorth) >= 1 &&
            IsOffCooldown(Senei) &&
            IsOffCooldown(Ikishoten) &&
            SenCount is 0;
    }

    internal class SAMLvl90Opener : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui,
            Role.TrueNorth, //2
            Gekko,
            Ikishoten,
            Kasha,
            Yukikaze,
            MidareSetsugekka,
            Senei,
            KaeshiSetsugekka,
            MeikyoShisui,
            Gekko,
            Higanbana,
            OgiNamikiri,
            Shoha,
            KaeshiNamikiri,
            Kasha,
            Gekko,
            Hakaze,
            Yukikaze,
            MidareSetsugekka,
            KaeshiSetsugekka
        ];

        internal override UserData ContentCheckConfig => SAM_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => SAM_Opener_PrePullDelay)
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([2], 11, () => !TargetNeedsPositionals())
        ];

        public override Preset Preset => Preset.SAM_ST_Opener;

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            GetRemainingCharges(Role.TrueNorth) >= 1 &&
            IsOffCooldown(Senei) &&
            IsOffCooldown(Ikishoten) &&
            SenCount is 0;
    }

    internal class SAMLvl100Opener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui,
            Role.TrueNorth, //2
            Gekko,
            Kasha,
            Ikishoten,
            Yukikaze,
            TendoSetsugekka,
            Senei,
            TendoKaeshiSetsugekka,
            MeikyoShisui,
            Gekko,
            Zanshin,
            Higanbana,
            OgiNamikiri,
            Shoha,
            KaeshiNamikiri,
            Kasha,
            Shinten, //18
            Gekko,
            Gyoten, //20
            Gyofu,
            Yukikaze,
            Shinten, //23
            TendoSetsugekka,
            Gyoten, //25
            TendoKaeshiSetsugekka
        ];

        internal override UserData ContentCheckConfig => SAM_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => SAM_Opener_PrePullDelay)
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([2], 11, () => !TargetNeedsPositionals())
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([18, 23], () => Kenki < SAMKenki.Shinten),
            ([20, 25], () => Kenki < SAMKenki.Gyoten || IsOnCooldown(Gyoten) || SAM_Opener_IncludeGyoten == 1)
        ];

        public override Preset Preset => Preset.SAM_ST_Opener;

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            GetRemainingCharges(Role.TrueNorth) >= 1 &&
            IsOffCooldown(Senei) &&
            IsOffCooldown(Ikishoten) &&
            SenCount is 0;
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

    private static bool NamikiriReady => Kaeshi is Kaeshi.Namikiri;

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
