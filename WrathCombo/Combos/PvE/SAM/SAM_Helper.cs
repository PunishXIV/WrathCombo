using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    internal static SAMGauge Gauge = GetJobGauge<SAMGauge>();
    internal static SAMOpenerMaxLevel1 Opener1 = new();

    internal static bool RefreshFugetsu => GetStatusEffectRemainingTime(Buffs.Fugetsu) < GetStatusEffectRemainingTime(Buffs.Fuka);

    internal static bool RefreshFuka => GetStatusEffectRemainingTime(Buffs.Fuka) < GetStatusEffectRemainingTime(Buffs.Fugetsu);

    internal static int SenCount => GetSenCount();

    private static int GetSenCount()
    {
        int senCount = 0;

        if (Gauge.HasGetsu)
            senCount++;

        if (Gauge.HasSetsu)
            senCount++;

        if (Gauge.HasKa)
            senCount++;

        return senCount;
    }

    internal static bool UseMeikyo()
    {
        float gcd = ActionManager.GetAdjustedRecastTime(ActionType.Action, Hakaze) / 100f;
        int meikyoUsed = ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

        if (ActionReady(MeikyoShisui) &&
            (WasLastWeaponskill(Gekko) || WasLastWeaponskill(Kasha) || WasLastWeaponskill(Yukikaze)) &&
            (!HasStatusEffect(Buffs.Tendo) || !LevelChecked(TendoSetsugekka)))
        {
            //if no opener/before lvl 100
            if ((IsNotEnabled(CustomComboPreset.SAM_ST_Opener) ||
                 !LevelChecked(TendoSetsugekka) ||
                 IsEnabled(CustomComboPreset.SAM_ST_Opener) && Config.SAM_Balance_Content == 1 && !InBossEncounter()) &&
                meikyoUsed < 2 && !HasStatusEffect(Buffs.MeikyoShisui) && !HasStatusEffect(Buffs.TsubameReady))
                return true;

            //double meikyo
            if (TraitLevelChecked(Traits.EnhancedMeikyoShishui) && HasStatusEffect(Buffs.TsubameReady))
            {
                switch (gcd)
                {
                    //Even windows
                    case >= 2.09f when meikyoUsed % 7 is 2 && SenCount is 3 && (GetCooldownRemainingTime(Ikishoten) <= gcd * 4 || IsOffCooldown(Ikishoten)) ||
                                       meikyoUsed % 7 is 4 && SenCount is 2 && (GetCooldownRemainingTime(Ikishoten) <= gcd * 5 || IsOffCooldown(Ikishoten)) ||
                                       meikyoUsed % 7 is 6 && SenCount is 1 && (GetCooldownRemainingTime(Ikishoten) <= gcd * 6 || IsOffCooldown(Ikishoten)):
                    //Odd windows
                    case >= 2.09f when GetCooldownRemainingTime(Ikishoten) is <= 85 and > 40 &&
                                       (meikyoUsed % 7 is 1 && SenCount is 3 ||
                                        meikyoUsed % 7 is 3 && SenCount is 2 ||
                                        meikyoUsed % 7 is 5 && SenCount is 1):
                    //Even windows
                    case <= 2.08f when GetCooldownRemainingTime(Ikishoten) <= gcd * 4 && SenCount is 3:

                    //Odd windows
                    case <= 2.08f when GetCooldownRemainingTime(Ikishoten) is <= 65 and > 50 && SenCount is 3:
                        return true;
                }
            }

            // reset meikyo
            if (gcd >= 2.09f && meikyoUsed % 7 is 0 && !HasStatusEffect(Buffs.MeikyoShisui) && WasLastWeaponskill(Yukikaze))
                return true;

            //Pre double meikyo / Overcap protection
            if (GetRemainingCharges(MeikyoShisui) == GetMaxCharges(MeikyoShisui) && !HasStatusEffect(Buffs.TsubameReady))
                return true;
        }

        return false;
    }

    // Iaijutsu Features
    internal static bool UseIaijutsu(ref uint actionID)
    {
        int higanbanaThreshold = Config.SAM_ST_Higanbana_Threshold;

        if (LevelChecked(Iaijutsu))
        {
            if (IsEnabled(CustomComboPreset.SAM_ST_SimpleMode))
            {
                if (LevelChecked(TsubameGaeshi) && HasStatusEffect(Buffs.TsubameReady) &&
                    (TraitLevelChecked(Traits.EnhancedHissatsu) && GetCooldownRemainingTime(Senei) > 33 || SenCount is 3) ||
                    LevelChecked(TendoKaeshiSetsugekka) && HasStatusEffect(Buffs.TendoKaeshiSetsugekkaReady))
                {
                    actionID = OriginalHook(TsubameGaeshi);
                    return true;
                }

                if (!IsMoving())
                {
                    if (SenCount is 1 && GetTargetHPPercent() > 1 && TargetIsBoss() &&
                        (GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget) <= 10 && JustUsed(Gekko) && JustUsed(MeikyoShisui, 15f) ||
                         !HasStatusEffect(Debuffs.Higanbana, CurrentTarget)))
                    {
                        actionID = OriginalHook(Iaijutsu);
                        return true;
                    }

                    if (SenCount is 2 && !LevelChecked(MidareSetsugekka))
                    {
                        actionID = OriginalHook(Iaijutsu);
                        return true;
                    }

                    if (SenCount is 3 && LevelChecked(MidareSetsugekka) && !HasStatusEffect(Buffs.TsubameReady))
                    {
                        actionID = OriginalHook(Iaijutsu);
                        return true;
                    }
                }
            }

            if (IsEnabled(CustomComboPreset.SAM_ST_AdvancedMode))
            {
                if (Config.SAM_ST_CDs_IaijutsuOption[3] &&
                    (LevelChecked(TsubameGaeshi) && HasStatusEffect(Buffs.TsubameReady) &&
                     (TraitLevelChecked(Traits.EnhancedHissatsu) && GetCooldownRemainingTime(Senei) > 33 || SenCount is 3) ||
                     LevelChecked(TendoKaeshiSetsugekka) && HasStatusEffect(Buffs.TendoKaeshiSetsugekkaReady)))
                {
                    actionID = OriginalHook(TsubameGaeshi);
                    return true;
                }

                if (!IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu_Movement) ||
                    IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu_Movement) && !IsMoving())
                {
                    if (Config.SAM_ST_CDs_IaijutsuOption[0] &&
                        SenCount is 1 && GetTargetHPPercent() > higanbanaThreshold &&
                        (Config.SAM_ST_Higanbana_Suboption == 0 ||
                         Config.SAM_ST_Higanbana_Suboption == 1 && TargetIsBoss()) &&
                        (GetStatusEffectRemainingTime(Debuffs.Higanbana, CurrentTarget) <= 10 && JustUsed(Gekko) && JustUsed(MeikyoShisui, 15f) ||
                         !HasStatusEffect(Debuffs.Higanbana, CurrentTarget)))
                    {
                        actionID = OriginalHook(Iaijutsu);
                        return true;
                    }

                    if (Config.SAM_ST_CDs_IaijutsuOption[1] &&
                        SenCount is 2 && !LevelChecked(MidareSetsugekka))
                    {
                        actionID = OriginalHook(Iaijutsu);
                        return true;
                    }

                    if (Config.SAM_ST_CDs_IaijutsuOption[2] &&
                        SenCount is 3 && LevelChecked(MidareSetsugekka) && !HasStatusEffect(Buffs.TsubameReady))
                    {
                        actionID = OriginalHook(Iaijutsu);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class SAMOpenerMaxLevel1 : WrathOpener
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
            Shinten,
            Gekko,
            Gyoten, //20
            Gyofu,
            Yukikaze,
            Shinten,
            TendoSetsugekka,
            TendoKaeshiSetsugekka
        ];

        internal override UserData ContentCheckConfig => Config.SAM_Balance_Content;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => Config.SAM_Opener_PrePullDelay)
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([2], 11, () => !TargetNeedsPositionals())
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(MeikyoShisui) is 2 &&
            GetRemainingCharges(Role.TrueNorth) is 2 &&
            IsOffCooldown(Senei) &&
            IsOffCooldown(Ikishoten);
    }

    #endregion

    #region ID's

    public const byte JobID = 34;

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
