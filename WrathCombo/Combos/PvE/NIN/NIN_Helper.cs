using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal static NINGauge Gauge = GetJobGauge<NINGauge>();

    internal static bool InMudra = false;

    internal static bool OriginalJutsu => IsOriginal(Ninjutsu);

    internal static bool TrickDebuff => TargetHasTrickDebuff();

    internal static bool MugDebuff => TargetHasMugDebuff();

    private static bool TargetHasTrickDebuff() => TargetHasEffect(Debuffs.TrickAttack) ||
               TargetHasEffect(Debuffs.KunaisBane);

    private static bool TargetHasMugDebuff() => TargetHasEffect(Debuffs.Mug) ||
               TargetHasEffect(Debuffs.Dokumori);

    public static Status? MudraBuff => FindEffect(Buffs.Mudra);

    public static uint CurrentNinjutsu => OriginalHook(Ninjutsu);

    internal class MudraCasting
    {
        public enum MudraState
        {
            None,
            CastingFumaShuriken,
            CastingKaton,
            CastingRaiton,
            CastingHyoton,
            CastingHuton,
            CastingDoton,
            CastingSuiton,
            CastingGokaMekkyaku,
            CastingHyoshoRanryu
        }

        public MudraState CurrentMudra = MudraState.None;

        ///<summary> Checks if the player is in a state to be able to cast a ninjitsu.</summary>
        private static bool CanCast()
        {
            if (InMudra)
                return true;

            float gcd = GetCooldown(GustSlash).CooldownTotal;

            if (gcd == 0.5)
                return true;

            if (GetRemainingCharges(Ten) == 0 &&
                !HasEffect(Buffs.Mudra) &&
                !HasEffect(Buffs.Kassatsu))
                return false;

            return true;
        }

        /// <summary> Simple method of casting Fuma Shuriken.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastFumaShuriken(ref uint actionID)
        {
            if (FumaShuriken.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingFumaShuriken)
            {
                if (!CanCast() || ActionWatching.LastAction == FumaShuriken)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
                CurrentMudra = MudraState.CastingFumaShuriken;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Raiton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastRaiton(ref uint actionID)
        {
            if (Raiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingRaiton)
            {
                if (!CanCast() || ActionWatching.LastAction == Raiton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
                CurrentMudra = MudraState.CastingRaiton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Katon.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastKaton(ref uint actionID)
        {
            if (Katon.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingKaton)
            {
                if (!CanCast() || ActionWatching.LastAction == Katon)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Chi or ChiCombo)
                {
                    actionID = OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
                CurrentMudra = MudraState.CastingKaton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Hyoton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastHyoton(ref uint actionID)
        {
            if (Hyoton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoton)
            {
                if (!CanCast() || HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == Hyoton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
                CurrentMudra = MudraState.CastingHyoton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Huton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastHuton(ref uint actionID)
        {
            if (Huton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHuton)
            {
                if (!CanCast() || ActionWatching.LastAction == Huton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Chi or ChiCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
                CurrentMudra = MudraState.CastingHuton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Doton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastDoton(ref uint actionID)
        {
            if (Doton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingDoton)
            {
                if (!CanCast() || ActionWatching.LastAction == Doton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
                CurrentMudra = MudraState.CastingDoton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Suiton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastSuiton(ref uint actionID)
        {
            if (Suiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingSuiton)
            {
                if (!CanCast() || ActionWatching.LastAction == Suiton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
                CurrentMudra = MudraState.CastingSuiton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Goka Mekkyaku.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastGokaMekkyaku(ref uint actionID)
        {
            if (GokaMekkyaku.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingGokaMekkyaku)
            {
                if (!CanCast() || !HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == GokaMekkyaku)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
                CurrentMudra = MudraState.CastingGokaMekkyaku;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Hyosho Ranryu.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastHyoshoRanryu(ref uint actionID)
        {
            if (HyoshoRanryu.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoshoRanryu)
            {
                if (!CanCast() || !HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == HyoshoRanryu)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
                CurrentMudra = MudraState.CastingHyoshoRanryu;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        public bool ContinueCurrentMudra(ref uint actionID)
        {

            if (ActionWatching.TimeSinceLastAction.TotalSeconds > 1 && CurrentNinjutsu == Ninjutsu && CurrentMudra != MudraState.None)
            {
                InMudra = false;
                ActionWatching.LastAction = 0;
                CurrentMudra = MudraState.None;
            }

            if (ActionWatching.LastAction == FumaShuriken ||
                 ActionWatching.LastAction == Katon ||
                 ActionWatching.LastAction == Raiton ||
                 ActionWatching.LastAction == Hyoton ||
                 ActionWatching.LastAction == Huton ||
                 ActionWatching.LastAction == Doton ||
                 ActionWatching.LastAction == Suiton ||
                 ActionWatching.LastAction == GokaMekkyaku ||
                 ActionWatching.LastAction == HyoshoRanryu)
            {
                CurrentMudra = MudraState.None;
                InMudra = false;
            }

            return CurrentMudra switch
            {
                MudraState.None => false,
                MudraState.CastingFumaShuriken => CastFumaShuriken(ref actionID),
                MudraState.CastingKaton => CastKaton(ref actionID),
                MudraState.CastingRaiton => CastRaiton(ref actionID),
                MudraState.CastingHyoton => CastHyoton(ref actionID),
                MudraState.CastingHuton => CastHuton(ref actionID),
                MudraState.CastingDoton => CastDoton(ref actionID),
                MudraState.CastingSuiton => CastSuiton(ref actionID),
                MudraState.CastingGokaMekkyaku => CastGokaMekkyaku(ref actionID),
                MudraState.CastingHyoshoRanryu => CastHyoshoRanryu(ref actionID),
                _ => false
            };
        }
    }

    internal class NINOpenerLogic
    {
        private bool OpenerEventsSetup;
        public uint PrePullStep = 1;

        private static uint OpenerLevel => 100;

        public static bool LevelChecked => LocalPlayer?.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

        public OpenerState CurrentState
        {
            get;
            set
            {
                if (value != field)
                {
                    if (value == OpenerState.OpenerReady)
                        PrePullStep = 1;
                    if (value == OpenerState.InOpener)
                        OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        PrePullStep = 0;
                        OpenerStep = 0;
                    }

                    field = value;
                }
            }
        } = OpenerState.OpenerFinished;

        public uint OpenerStep
        {
            get;
            set
            {
                if (value != field)
                    Svc.Log.Debug($"{value}");
                field = value;
            }
        } = 1;

        private static bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1)
                return false;
            if (IsOnCooldown(Mug))
                return false;
            if (IsOnCooldown(TenChiJin))
                return false;
            if (IsOnCooldown(PhantomKamaitachi))
                return false;
            if (IsOnCooldown(Bunshin))
                return false;
            if (IsOnCooldown(DreamWithinADream))
                return false;
            if (IsOnCooldown(Kassatsu))
                return false;
            if (IsOnCooldown(TrickAttack))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID, MudraCasting mudraState)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0 && !InCombat())
                CurrentState = OpenerState.OpenerReady;

            if (CurrentState == OpenerState.OpenerReady)
            {
                if (WasLastAction(Suiton) && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1)
                    _ = mudraState.CastSuiton(ref actionID);

                ////Failure states
                //if (PrePullStep is (1 or 2) && .InCombat()) { mudraState.CurrentMudra = MudraCasting.MudraState.None; ResetOpener(); }

                return true;
            }

            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID, MudraCasting mudraState)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.InOpener)
            {
                bool inLateWeaveWindow = CanDelayedWeave(1, 0);

                if (WasLastAction(Kassatsu) && OpenerStep == 1)
                    OpenerStep++;
                else if (OpenerStep == 1)
                    actionID = OriginalHook(Kassatsu);

                if (WasLastAction(SpinningEdge) && OpenerStep == 2)
                    OpenerStep++;
                else if (OpenerStep == 2)
                    actionID = OriginalHook(SpinningEdge);

                if (WasLastAction(GustSlash) && OpenerStep == 3)
                    OpenerStep++;
                else if (OpenerStep == 3)
                    actionID = OriginalHook(GustSlash);

                if (WasLastAction(OriginalHook(Mug)) && OpenerStep == 4)
                    OpenerStep++;
                else if (OpenerStep == 4)
                    actionID = OriginalHook(Mug);

                if (WasLastAction(Bunshin) && OpenerStep == 5)
                    OpenerStep++;
                else if (OpenerStep == 5)
                    actionID = OriginalHook(Bunshin);

                if (WasLastAction(PhantomKamaitachi) && OpenerStep == 6)
                    OpenerStep++;
                else if (OpenerStep == 6)
                    actionID = OriginalHook(PhantomKamaitachi);

                if (WasLastAction(ArmorCrush) && OpenerStep == 7)
                    OpenerStep++;
                else if (OpenerStep == 7)
                    actionID = OriginalHook(ArmorCrush);

                if (WasLastAction(OriginalHook(TrickAttack)) &&
                    OpenerStep == 8)
                    OpenerStep++;
                else if (OpenerStep == 8 && inLateWeaveWindow)
                    actionID = OriginalHook(TrickAttack);

                if (WasLastAction(HyoshoRanryu) && OpenerStep == 9)
                    OpenerStep++;
                else if (OpenerStep == 9)
                    _ = mudraState.CastHyoshoRanryu(ref actionID);

                if (WasLastAction(DreamWithinADream) && OpenerStep == 10)
                    OpenerStep++;
                else if (OpenerStep == 10)
                    actionID = OriginalHook(DreamWithinADream);

                if (WasLastAction(Raiton) && OpenerStep == 11)
                    OpenerStep++;
                else if (OpenerStep == 11)
                    _ = mudraState.CastRaiton(ref actionID);

                if (WasLastAction(TenChiJin) && OpenerStep == 12)
                    OpenerStep++;
                else if (OpenerStep == 12)
                    actionID = OriginalHook(TenChiJin);

                if (WasLastAction(TCJFumaShurikenTen) && OpenerStep == 13)
                    OpenerStep++;
                else if (OpenerStep == 13)
                    actionID = OriginalHook(Ten);

                if (WasLastAction(TCJRaiton) && OpenerStep == 14)
                    OpenerStep++;
                else if (OpenerStep == 14)
                    actionID = OriginalHook(Chi);

                if (WasLastAction(TCJSuiton) && OpenerStep == 15)
                    OpenerStep++;
                else if (OpenerStep == 15)
                    actionID = OriginalHook(Jin);

                if (WasLastAction(Meisui) && OpenerStep == 16)
                    OpenerStep++;
                else if (OpenerStep == 16)
                    actionID = OriginalHook(Meisui);

                if (WasLastAction(FleetingRaiju) && OpenerStep == 17)
                    OpenerStep++;
                else if (OpenerStep == 17)
                    actionID = OriginalHook(FleetingRaiju);

                if (WasLastAction(ZeshoMeppo) && OpenerStep == 18)
                    OpenerStep++;
                else if (OpenerStep == 18)
                    actionID = OriginalHook(Bhavacakra);

                if (WasLastAction(TenriJendo) && OpenerStep == 19)
                    OpenerStep++;
                else if (OpenerStep == 19)
                    actionID = OriginalHook(TenriJendo);

                if (WasLastAction(FleetingRaiju) && OpenerStep == 20)
                    OpenerStep++;
                else if (OpenerStep == 20)
                    actionID = OriginalHook(FleetingRaiju);

                if (WasLastAction(OriginalHook(Bhavacakra)) &&
                    OpenerStep == 21)
                    OpenerStep++;
                else if (OpenerStep == 21)
                    actionID = OriginalHook(Bhavacakra);

                if (WasLastAction(Raiton) && OpenerStep == 22)
                    OpenerStep++;
                else if (OpenerStep == 22)
                    _ = mudraState.CastRaiton(ref actionID);

                if (WasLastAction(FleetingRaiju) && OpenerStep == 23)
                    CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 23)
                    actionID = OriginalHook(FleetingRaiju);

                //Failure states
                if ((OpenerStep is 8 && !HasEffect(Buffs.ShadowWalker)) ||
                    (OpenerStep is 18 or 21 && GetJobGauge<NINGauge>().Ninki < 40) ||
                    (OpenerStep is 17 or 20 && !HasEffect(Buffs.RaijuReady)) ||
                    (OpenerStep is 9 && !HasEffect(Buffs.Kassatsu)))
                    ResetOpener();

                return true;
            }

            return false;
        }

        private void ResetOpener() => CurrentState = OpenerState.FailedOpener;

        public bool DoFullOpener(ref uint actionID, MudraCasting mudraState)
        {
            if (!LevelChecked)
                return false;

            if (!OpenerEventsSetup)
            {
                Svc.Condition.ConditionChange += CheckCombatStatus;
                OpenerEventsSetup = true;
            }

            if (CurrentState == OpenerState.OpenerReady || CurrentState == OpenerState.FailedOpener)
                if (DoPrePullSteps(ref actionID, mudraState))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID, mudraState))
                    return true;

            if (CurrentState == OpenerState.OpenerFinished && !InCombat())
                ResetOpener();

            return false;
        }

        internal void Dispose() => Svc.Condition.ConditionChange -= CheckCombatStatus;

        private void CheckCombatStatus(ConditionFlag flag, bool value)
        {
            if (flag == ConditionFlag.InCombat && value == false)
                ResetOpener();
        }
    }
}
