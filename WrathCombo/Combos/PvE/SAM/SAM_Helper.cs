using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    internal static SAMGauge gauge = GetJobGauge<SAMGauge>();
    internal static SAMOpenerLogic SAMOpener = new();

    internal static int MeikyoUsed => ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

    internal static bool trueNorthReady => TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
                                           !HasEffect(All.Buffs.TrueNorth);

    internal static float GCD => GetCooldown(Hakaze).CooldownTotal;

    internal class SAMHelper
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
                    {
                        if (usedMeikyo is 1 or 8 && SenCount is 3)
                            return true;

                        if (usedMeikyo is 3 or 10 && SenCount is 2)
                            return true;

                        if (usedMeikyo is 5 or 12 && SenCount is 1)
                            return true;
                    }

                    if (GetCooldownRemainingTime(Ikishoten) > 80) //2min windows
                    {
                        if (usedMeikyo is 2 or 9 && SenCount is 3)
                            return true;

                        if (usedMeikyo is 4 or 11 && SenCount is 2)
                            return true;

                        if (usedMeikyo is 6 or 13 && SenCount is 1)
                            return true;
                    }

                    if (usedMeikyo is 7 or 14 && !HasEffect(Buffs.MeikyoShisui))
                        return true;
                }
            }

            return false;
        }
    }

    internal class SAMOpenerLogic
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep = 1;

        public uint PrePullStep;

        private static uint OpenerLevel => 100;

        public static bool LevelChecked => LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasPrePullCooldowns() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (!ActionReady(Senei))
                return false;

            if (!ActionReady(Ikishoten))
                return false;

            return true;
        }

        public static bool HasPrePullCooldowns()
        {
            if (GetRemainingCharges(MeikyoShisui) < 2)
                return false;

            if (GetRemainingCharges(All.TrueNorth) < 2)
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (HasEffect(Buffs.MeikyoShisui) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = MeikyoShisui;

                if (HasEffect(All.Buffs.TrueNorth) && PrePullStep == 2)
                    currentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = All.TrueNorth;

                if (ActionWatching.CombatActions.Count > 2 && InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (WasLastAction(Gekko) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = Gekko;

                if (WasLastAction(Kasha) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = Kasha;

                if (WasLastAction(Ikishoten) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Ikishoten;

                if (WasLastAction(Yukikaze) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = Yukikaze;

                if (WasLastAction(TendoSetsugekka) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = TendoSetsugekka;

                if (WasLastAction(Senei) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Senei;

                if (WasLastAction(TendoKaeshiSetsugekka) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = TendoKaeshiSetsugekka;

                if (WasLastAction(MeikyoShisui) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = MeikyoShisui;

                if (WasLastAction(Gekko) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = Gekko;

                if (WasLastAction(Zanshin) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Zanshin;

                if (WasLastAction(Higanbana) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = Higanbana;

                if (WasLastAction(OgiNamikiri) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = OgiNamikiri;

                if (WasLastAction(Shoha) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = Shoha;

                if (WasLastAction(KaeshiNamikiri) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = KaeshiNamikiri;

                if (WasLastAction(Kasha) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Kasha;

                if (WasLastAction(Shinten) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Shinten;

                if (WasLastAction(Gekko) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = Gekko;

                if (WasLastAction(Gyoten) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Gyoten;

                if (WasLastAction(Gyofu) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Gyofu;

                if (WasLastAction(Yukikaze) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = Yukikaze;

                if (WasLastAction(Shinten) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Shinten;

                if (WasLastAction(TendoSetsugekka) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = TendoSetsugekka;

                if (WasLastAction(TendoKaeshiSetsugekka) && OpenerStep == 23)
                    CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 23) actionID = TendoKaeshiSetsugekka;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Senei && IsOnCooldown(Senei)) ||
                     (actionID == Ikishoten && IsOnCooldown(Ikishoten)) ||
                     (actionID == MeikyoShisui && GetRemainingCharges(MeikyoShisui) < 1)) &&
                    ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 1;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }
}