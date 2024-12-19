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
            if (actionId == CurrentOpenerAction)
            {
                OpenerStep++;
                if (OpenerStep > OpenerActions.Count)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    return;
                }

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
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Information("Opener Now Ready");
                        else
                            Svc.Log.Debug($"Opener Now Ready");

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            if (Service.Configuration.OutputOpenerLogs)
                                DuoLog.Error($"Opener Failed at step {OpenerStep}, {CurrentOpenerAction.ActionName()}");
                            else
                                Svc.Log.Information($"Opener Failed at step {OpenerStep}, {CurrentOpenerAction.ActionName()}");

                        ResetOpener();
                    }

                    if (value == OpenerState.OpenerFinished)
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Information("Opener Finished");
                        else
                            Svc.Log.Debug($"Opener Finished");
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

        public virtual List<(int[] Steps, uint NewAction, Func<bool> Condition)> ProcSteps { get; set; } = new();

        public uint CurrentOpenerAction { get; set; }

        public abstract int MinOpenerLevel { get; }

        public abstract int MaxOpenerLevel { get; }

        public bool LevelChecked => Player.Level >= MinOpenerLevel && Player.Level <= MaxOpenerLevel;

        public abstract bool HasCooldowns();

        public bool FullOpener(ref uint actionID)
        {
            if (!LevelChecked || OpenerActions.Count == 0)
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

                if (OpenerStep > 1 && ActionWatching.TimeSinceLastAction.TotalSeconds >= 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }


                if (DelayedWeaveSteps.Any(x => x == OpenerStep))
                {
                    if (CustomComboFunctions.CanDelayedWeave())
                    {
                        actionID = CurrentOpenerAction;
                        return true;
                    }
                    else
                    {
                        actionID = 11;
                        return true;
                    }
                }

                if (ProcSteps.FindFirst(x => x.Steps.Any(y => y == OpenerStep), out var condtional))
                {
                    if (condtional.Condition())
                    {
                        CurrentOpenerAction = actionID = condtional.NewAction;
                        return true;
                    }

                    CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                }

                actionID = CurrentOpenerAction;
                return true;

            }

            return false;
        }

        public void ResetOpener()
        {
            Svc.Log.Debug($"Opener Reset");
            OpenerStep = 0;
            CurrentOpenerAction = 0;
            CurrentState = OpenerState.OpenerNotReady;
        }

        internal static void SelectOpener(uint jobId)
        {
            CurrentOpener = jobId switch
            {
                AST.JobID => AST.Opener(),
                BLM.JobID => BLM.Opener(),
                BRD.JobID => BRD.Opener(),
                DRG.JobID => DRG.Opener(),
                GNB.JobID => GNB.Opener(),
                MCH.JobID => MCH.Opener(),
                MNK.JobID => MNK.Opener(),
                NIN.JobID => NIN.Opener(),
                PCT.JobID => PCT.Opener(),
                PLD.JobID => PLD.Opener(),
                RPR.JobID => RPR.Opener(),
                SAM.JobID => SAM.Opener(),
                SGE.JobID => SGE.Opener(),
                VPR.JobID => VPR.Opener(),
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
                    Svc.Log.Debug($"Removed update hook");
                }

                if (currentOpener != value)
                {
                    currentOpener = value;
                    Svc.Framework.Update += currentOpener.UpdateOpener;
                }
            }
        }

        public static WrathOpener Dummy = new DummyOpener();
    }

    public class DummyOpener : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } = new();
        public override int MinOpenerLevel => 1;
        public override int MaxOpenerLevel => 10000;
        public override bool HasCooldowns() => false;
    }
}
