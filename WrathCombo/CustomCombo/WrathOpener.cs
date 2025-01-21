using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.CustomComboNS
{
    public abstract class WrathOpener
    {
        private OpenerState currentState = OpenerState.OpenerNotReady;
        private int openerStep;
        private static WrathOpener? currentOpener;

        private void UpdateOpener(Dalamud.Plugin.Services.IFramework framework)
        {
            if (!Service.IconReplacer.getIconHook.IsEnabled)
            {
                uint _ = 0;
                FullOpener(ref _);
            }
        }

        public void ProgressOpener(uint actionId)
        {
            if (actionId == CurrentOpenerAction || (AllowUpgradeSteps.Any(x => x == OpenerStep) && OriginalHook(CurrentOpenerAction) == actionId))
            {
                OpenerStep++;
                if (OpenerStep > OpenerActions.Count)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    return;
                }

                PreviousOpenerAction = CurrentOpenerAction;
                CurrentOpenerAction = OpenerActions[OpenerStep - 1];
            }
        }

        public virtual OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    currentState = value;

                    if (value == OpenerState.OpenerNotReady)
                        Svc.Log.Debug($"Opener Not Ready");

                    if (value == OpenerState.OpenerReady)
                    {
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Information("Opener Now Ready");
                        else
                            Svc.Log.Debug($"Opener Now Ready");
                    }

                    if (value == OpenerState.FailedOpener)
                    {
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Error($"Opener Failed at step {OpenerStep}, {CurrentOpenerAction.ActionName()}");
                        else
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}, {CurrentOpenerAction.ActionName()}");

                        if (AllowReopener)
                        ResetOpener();
                    }

                    if (value == OpenerState.OpenerFinished)
                    {
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Information("Opener Finished");
                        else
                            Svc.Log.Debug($"Opener Finished");

                        if (AllowReopener)
                            ResetOpener();
                    }
                }
            }
        }

        public virtual int OpenerStep
        {
            get => openerStep;
            set
            {
                if (value != openerStep)
                {
                    Svc.Log.Debug($"Opener Step {value}");
                    openerStep = value;
                }
            }
        }

        public abstract List<uint> OpenerActions { get; set; }

        public virtual List<int> DelayedWeaveSteps { get; set; } = new List<int>();

        public virtual List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } = new();

        public virtual List<(int[] Steps, int HoldDelay)> PrepullDelays { get; set; } = new();

        public virtual List<int> AllowUpgradeSteps { get; set; } = new();

        private int DelayedStep = 0;
        private DateTime DelayedAt;

        public uint CurrentOpenerAction { get; set; }
        public uint PreviousOpenerAction { get; set; }

        public abstract int MinOpenerLevel { get; }

        public abstract int MaxOpenerLevel { get; }

        public virtual bool AllowReopener { get; set; } = false;

        internal abstract UserData? ContentCheckConfig { get; }

        public bool LevelChecked => Player.Level >= MinOpenerLevel && Player.Level <= MaxOpenerLevel;

        public abstract bool HasCooldowns();

        public unsafe bool FullOpener(ref uint actionID)
        {
            bool inContent = ContentCheckConfig is UserBoolArray ? ContentCheck.IsInConfiguredContent((UserBoolArray)ContentCheckConfig, ContentCheck.ListSet.BossOnly) : ContentCheckConfig is UserInt ? ContentCheck.IsInConfiguredContent((UserInt)ContentCheckConfig, ContentCheck.ListSet.BossOnly) : false;
            if (!LevelChecked || OpenerActions.Count == 0 || !inContent)
            {
                return false;
            }

            CurrentOpener = this;

            if (CurrentState == OpenerState.OpenerNotReady)
            {
                if (HasCooldowns())
                {
                    CurrentState = OpenerState.OpenerReady;
                    OpenerStep = 1;
                    CurrentOpenerAction = OpenerActions.First();
                }
            }

            if (CurrentState == OpenerState.OpenerReady)
            {
                if (!HasCooldowns() && OpenerStep == 1)
                {
                    ResetOpener();
                    return false;
                }

                if (OpenerStep > 1)
                {
                    var delay = PrepullDelays.FindFirst(x => x.Steps.Any(y => y == DelayedStep && y == OpenerStep), out var hold);
                    if ((!delay && InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= Service.Configuration.OpenerTimeout) || (delay && (DateTime.Now - DelayedAt).TotalSeconds > hold.HoldDelay + Service.Configuration.OpenerTimeout))
                    {
                        CurrentState = OpenerState.FailedOpener;
                        return false; 
                    }
                }

                if (OpenerStep < OpenerActions.Count)
                {
                    actionID = CurrentOpenerAction = OpenerActions[OpenerStep - 1];

                    if (DelayedWeaveSteps.Any(x => x == OpenerStep))
                    {
                        if (!CanDelayedWeave())
                        {
                            actionID = 11;
                            return true;
                        }
                    }

                    foreach (var (Steps, NewAction, Condition) in SubstitutionSteps.Where(x => x.Steps.Any(y => y == OpenerStep)))
                    {
                        if (Condition())
                        {
                            CurrentOpenerAction = actionID = NewAction;
                            break;
                        }
                        else
                            CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                    }

                    foreach (var (Steps, HoldDelay) in PrepullDelays.Where(x => x.Steps.Any(y => y == OpenerStep)))
                    {
                        if (DelayedStep != OpenerStep)
                        {
                            DelayedAt = DateTime.Now;
                            DelayedStep = OpenerStep;
                        }

                        if ((DateTime.Now - DelayedAt).TotalSeconds < HoldDelay && !InCombat())
                        {
                            ActionWatching.TimeLastActionUsed = DateTime.Now; //Hacky workaround for TN jobs
                            actionID = 11;
                            return true;
                        }
                    }

                    if (CurrentOpenerAction == All.TrueNorth && !TargetNeedsPositionals())
                    {
                        OpenerStep++;
                        CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                    }

                    while (OpenerStep > 1 && !ActionReady(CurrentOpenerAction) && ActionWatching.TimeSinceLastAction.TotalSeconds > 1.5)
                    {
                        if (OpenerStep >= OpenerActions.Count)
                            break;

                        Svc.Log.Debug($"Skipping {CurrentOpenerAction.ActionName()}");
                        OpenerStep++;

                        CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                    }


                    return true;
                }

            }

            return false;
        }

        public void ResetOpener()
        {
            Svc.Log.Debug($"Opener Reset");
            DelayedStep = 0;
            OpenerStep = 0;
            CurrentOpenerAction = 0;
            CurrentState = OpenerState.OpenerNotReady;
        }

        internal static void SelectOpener()
        {
            CurrentOpener = Player.JobId switch
            {
                AST.JobID => AST.Opener(),
                BLM.JobID => BLM.Opener(),
                BRD.JobID => BRD.Opener(),
                DRG.JobID => DRG.Opener(),
                DNC.JobID => DNC.Opener(),
                DRK.JobID => DRK.Opener(),
                GNB.JobID => GNB.Opener(),
                MCH.JobID => MCH.Opener(),
                MNK.JobID => MNK.Opener(),
                NIN.JobID => NIN.Opener(),
                PCT.JobID => PCT.Opener(),
                PLD.JobID => PLD.Opener(),
                RDM.JobID => RDM.Opener(),
                RPR.JobID => RPR.Opener(),
                SAM.JobID => SAM.Opener(),
                SMN.JobID => SMN.Opener(),
                SCH.JobID => SCH.Opener(),
                SGE.JobID => SGE.Opener(),
                VPR.JobID => VPR.Opener(),
                WAR.JobID => WAR.Opener(),
                WHM.JobID => WHM.Opener(),
                _ => Dummy
            };
        }

        public static WrathOpener? CurrentOpener
        {
            get => currentOpener;
            set
            {
                if (currentOpener != null && currentOpener != value)
                {
                    Svc.Framework.Update -= currentOpener.UpdateOpener;
                    OnCastInterrupted -= RevertInterruptedCasts;
                    Svc.Condition.ConditionChange -= ResetAfterCombat;
                    Svc.Log.Debug($"Removed update hook");
                }

                if (currentOpener != value)
                {
                    currentOpener = value;
                    Svc.Framework.Update += currentOpener.UpdateOpener;
                    OnCastInterrupted += RevertInterruptedCasts;
                    Svc.Condition.ConditionChange += ResetAfterCombat;
                }
            }
        }

        private static void ResetAfterCombat(ConditionFlag flag, bool value)
        {
            if (flag == ConditionFlag.InCombat && !value)
                CurrentOpener.ResetOpener();
        }

        private static void RevertInterruptedCasts(uint interruptedAction)
        {
            if (CurrentOpener?.CurrentState is OpenerState.OpenerReady)
            {
                if (CurrentOpener?.OpenerStep > 1 && interruptedAction == CurrentOpener.PreviousOpenerAction)
                    CurrentOpener.OpenerStep -= 1;
            }
        }

        public static WrathOpener Dummy = new DummyOpener();
    }

    public class DummyOpener : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } = new();
        public override int MinOpenerLevel => 1;
        public override int MaxOpenerLevel => 10000;

        internal override UserData? ContentCheckConfig => null;

        public override bool HasCooldowns() => false;
    }
}
