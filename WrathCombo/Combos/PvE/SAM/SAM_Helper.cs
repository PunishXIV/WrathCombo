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

internal partial class SAM
{
    internal static SAMGauge Gauge = GetJobGauge<SAMGauge>();
    internal static SAMOpenerMaxLevel1 Opener1 = new();

    internal static int MeikyoUsed => ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

    internal static bool TrueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static float GCD => GetCooldown(Hakaze).CooldownTotal;

    internal static int SenCount => GetSenCount();
    
    internal static bool ComboStarted => GetComboStarted();
    
    internal static int NumSen => GetNumSen();

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

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

    private static unsafe bool GetComboStarted()
    {
        uint comboAction = ActionManager.Instance()->Combo.Action;

        return comboAction == OriginalHook(Hakaze) ||
               comboAction == Jinpu ||
               comboAction == Shifu;
    }

    private static int GetNumSen()
    {
        bool ka = Gauge.Sen.HasFlag(Sen.KA);
        bool getsu = Gauge.Sen.HasFlag(Sen.GETSU);
        bool setsu = Gauge.Sen.HasFlag(Sen.SETSU);

        return (ka ? 1 : 0) + (getsu ? 1 : 0) + (setsu ? 1 : 0);
    }

    internal static bool UseMeikyo()
    {
        if (ActionReady(MeikyoShisui) &&
            (WasLastWeaponskill(Gekko) || WasLastWeaponskill(Kasha) || WasLastWeaponskill(Yukikaze)) &&
            (!HasEffect(Buffs.Tendo) || !LevelChecked(TendoSetsugekka)))
        {
            //if no opener/before lvl 100
            if ((IsNotEnabled(CustomComboPreset.SAM_ST_Opener) ||
                 !LevelChecked(TendoSetsugekka) ||
                 IsEnabled(CustomComboPreset.SAM_ST_Opener) && Config.SAM_Balance_Content == 1 && !InBossEncounter()) &&
                MeikyoUsed < 2 && !HasEffect(Buffs.MeikyoShisui) && !HasEffect(Buffs.TsubameReady))
                return true;

            //double meikyo
            if (TraitLevelChecked(Traits.EnhancedMeikyoShishui) && HasEffect(Buffs.TsubameReady))
            {
                //2min windows
                if ((GetCooldownRemainingTime(Ikishoten) > 80 || GetCooldownRemainingTime(Ikishoten) < GCD * 2 ||
                     IsOffCooldown(Ikishoten) || JustUsed(Ikishoten, 5f)) &&
                    (MeikyoUsed % 7 is 2 && SenCount is 3 ||
                     MeikyoUsed % 7 is 4 && SenCount is 2 ||
                     MeikyoUsed % 7 is 6 && SenCount is 1))
                    return true;

                //1min windows
                if (GetCooldownRemainingTime(Ikishoten) is > 35 and < 71 &&
                    (MeikyoUsed % 7 is 1 && SenCount is 3 ||
                     MeikyoUsed % 7 is 3 && SenCount is 2 ||
                     MeikyoUsed % 7 is 5 && SenCount is 1))
                    return true;
            }

            // reset meikyo
            if (MeikyoUsed % 7 is 0 && !HasEffect(Buffs.MeikyoShisui) && WasLastWeaponskill(Yukikaze))
                return true;

            //Pre double meikyo
            if (!TraitLevelChecked(Traits.EnhancedMeikyoShishui))
                return true;
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
        internal override UserData ContentCheckConfig => Config.SAM_Balance_Content;

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], 13)
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([2], 11, () => !TargetNeedsPositionals())
        ];

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(MeikyoShisui) < 2)
                return false;

            if (GetRemainingCharges(All.TrueNorth) < 2)
                return false;

            if (!IsOffCooldown(Senei))
                return false;

            if (!IsOffCooldown(Ikishoten))
                return false;

            return true;
        }
    }

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
