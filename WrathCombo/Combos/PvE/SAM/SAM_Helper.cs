#region

using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class SAM
{
    internal static SAMGauge gauge = GetJobGauge<SAMGauge>();
    internal static SAMOpenerMaxLevel1 Opener1 = new();

    internal static int MeikyoUsed => ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

    internal static bool trueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static float GCD => GetCooldown(Hakaze).CooldownTotal;

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal static class SAMHelper
    {
        internal static int SenCount => GetSenCount();

        internal static bool ComboStarted => GetComboStarted();

        private static int GetSenCount()
        {
            int senCount = 0;
            if (gauge.HasGetsu) senCount++;
            if (gauge.HasSetsu) senCount++;
            if (gauge.HasKa) senCount++;

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
                if ((IsNotEnabled(CustomComboPreset.SAM_ST_Opener) || !LevelChecked(TendoSetsugekka)) &&
                    MeikyoUsed < 2 && !HasEffect(Buffs.MeikyoShisui) && !HasEffect(Buffs.TsubameReady))
                    return true;

                if (MeikyoUsed >= 2)
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
            }

            return false;
        }
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