using Dalamud.Game.ClientState.JobGauge.Enums;
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

internal static partial class SAM
{
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
        Gyofu = 36963;

    public static int NumSen(SAMGauge gauge)
    {
        bool ka = gauge.Sen.HasFlag(Sen.KA);
        bool getsu = gauge.Sen.HasFlag(Sen.GETSU);
        bool setsu = gauge.Sen.HasFlag(Sen.SETSU);

        return (ka ? 1 : 0) + (getsu ? 1 : 0) + (setsu ? 1 : 0);
    }

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
            EnhancedMeikyoShishui2 = 593;
    }

    #endregion

    internal static SAMGauge gauge = GetJobGauge<SAMGauge>();
    internal static SAMOpenerMaxLevel1 Opener1 = new();

    internal static int MeikyoUsed => ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

    internal static bool TrueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static float GCD => GetCooldown(Hakaze).CooldownTotal;

    internal static int SenCount => GetSenCount();

    internal static bool ComboStarted => GetComboStarted();

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    private static int GetSenCount()
    {
        int senCount = 0;

        if (gauge.HasGetsu)
            senCount++;

        if (gauge.HasSetsu)
            senCount++;

        if (gauge.HasKa)
            senCount++;

        return senCount;
    }

    private static unsafe bool GetComboStarted()
    {
        uint comboAction = ActionManager.Instance()->Combo.Action;

        return comboAction == OriginalHook(Hakaze) ||
               comboAction == OriginalHook(Jinpu) ||
               comboAction == OriginalHook(Shifu);
    }

    internal static bool UseMeikyo()
    {
        int usedMeikyo = MeikyoUsed % 15;

        if (ActionReady(MeikyoShisui) && !ComboStarted)
        {
            //if no opener/before lvl 100
            if ((IsNotEnabled(CustomComboPreset.SAM_ST_Opener) ||
                !LevelChecked(TendoSetsugekka) ||
                (IsEnabled(CustomComboPreset.SAM_ST_Opener) && Config.SAM_Balance_Content == 1 && !InBossEncounter())) &&
                MeikyoUsed < 2 && !HasEffect(Buffs.MeikyoShisui) && !HasEffect(Buffs.TsubameReady))
                return true;

            if (MeikyoUsed >= 2)
            {
                if (LevelChecked(Ikishoten))
                {
                    if (GetCooldownRemainingTime(Ikishoten) is > 45 and < 71) //1min windows
                        switch (usedMeikyo)
                        {
                            case 1 or 8 when SenCount is 3:
                            case 3 or 10 when SenCount is 2:
                            case 5 or 12 when SenCount is 1:
                                return true;
                        }

                    if (GetCooldownRemainingTime(Ikishoten) > 80) //2min windows
                        switch (usedMeikyo)
                        {
                            case 2 or 9 when SenCount is 3:
                            case 4 or 11 when SenCount is 2:
                            case 6 or 13 when SenCount is 1:
                                return true;
                        }

                    if (usedMeikyo is 7 or 14 && !HasEffect(Buffs.MeikyoShisui))
                        return true;
                }

                if (!LevelChecked(Ikishoten))
                    return true;
            }
        }

        return false;
    }

    internal class SAMOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            MeikyoShisui,
            All.TrueNorth,
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
            Gyoten,
            Gyofu,
            Yukikaze,
            Shinten,
            TendoSetsugekka,
            TendoKaeshiSetsugekka
        ];
        internal override UserData? ContentCheckConfig => Config.SAM_Balance_Content;

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays { get; set; } =
            [
            ([2], 14),
            ];

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(MeikyoShisui) < 2)
                return false;

            if (GetRemainingCharges(All.TrueNorth) < 2)
                return false;

            if (!ActionReady(Senei))
                return false;

            if (!ActionReady(Ikishoten))
                return false;

            return true;
        }
    }
}