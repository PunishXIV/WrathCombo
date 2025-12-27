using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvE.Enums;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.CustomComboNS;

public abstract class WrathOpener
{
    private OpenerState currentState = OpenerState.OpenerNotReady;
    private int openerStep;
    private static WrathOpener? currentOpener;

    private void UpdateOpener(Dalamud.Plugin.Services.IFramework framework)
    {
        if (Service.Configuration.PerformanceMode)
        {
            CurrentOpener = this;
            uint _ = 0;
            CurrentOpener.FullOpener(ref _);
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
        get => currentState switch
        {
            OpenerState.OpenerReady when openerStep > 1 &&
                                         openerStep <= OpenerActions.Count =>
                OpenerState.InOpener,
            _ => currentState,
        };
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

                    if (AllowReopener || !InCombat())
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
    public virtual List<int> VeryDelayedWeaveSteps { get; set; } = new List<int>(); //for very late-weaving

    public virtual List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } = new();

    public virtual List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } = new();

    public virtual List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = new();

    public virtual List<int> AllowUpgradeSteps { get; set; } = new();

    private int DelayedStep = 0;
    private DateTime DelayedAt;

    public uint CurrentOpenerAction
    {
        get;
        set
        {
            if (value != All.SavageBlade)
                field = value;
        }
    }
    public uint PreviousOpenerAction { get; set; }

    public abstract int MinOpenerLevel { get; }

    public abstract int MaxOpenerLevel { get; }

    public virtual bool AllowReopener { get; set; } = false;

    internal abstract UserData? ContentCheckConfig { get; }

    public bool LevelChecked => Svc.PlayerState.EffectiveLevel >= MinOpenerLevel && Svc.PlayerState.EffectiveLevel <= MaxOpenerLevel;

    public abstract bool HasCooldowns();

    public bool CacheReady = false;

    public unsafe bool FullOpener(ref uint actionID)
    {
        bool inContent = ContentCheckConfig is UserBoolArray ? ContentCheck.IsInConfiguredContent((UserBoolArray)ContentCheckConfig, ContentCheck.ListSet.BossOnly) : ContentCheckConfig is UserInt ? ContentCheck.IsInConfiguredContent((UserInt)ContentCheckConfig, ContentCheck.ListSet.BossOnly) : false;
        if (!LevelChecked || OpenerActions.Count == 0 || !inContent || !CacheReady)
        {
            return false;
        }

        CurrentOpener = this;

        if (CurrentState == OpenerState.OpenerNotReady)
        {
            if (HasCooldowns() && !InCombat())
            {
                CurrentState = OpenerState.OpenerReady;
                OpenerStep = 1;
                CurrentOpenerAction = OpenerActions.First();
            }
        }

        if (CurrentState is OpenerState.OpenerReady or OpenerState.InOpener)
        {
            if (!HasCooldowns() && OpenerStep == 1)
            {
                ResetOpener();
                return false;
            }

            if (OpenerStep > 1)
            {
                bool prevStepSkipping = SkipSteps.FindFirst(x => x.Steps.FindFirst(y => y == OpenerStep  - 1, out var t), out var p);
                if (prevStepSkipping)
                    prevStepSkipping = p.Condition();

                bool delay = PrepullDelays.FindFirst(x => x.Steps.Any(y => y == DelayedStep && y == OpenerStep), out var hold);
                if ((!delay && !prevStepSkipping && ActionWatching.TimeSinceLastAction.TotalSeconds >= Service.Configuration.OpenerTimeout) || (delay && (DateTime.Now - DelayedAt).TotalSeconds > hold.HoldDelay() + Service.Configuration.OpenerTimeout))
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }
            }

            if (OpenerStep <= OpenerActions.Count)
            {
                foreach (var (Step, Condition) in SkipSteps.Where(x => x.Steps.Any(y => y == OpenerStep)))
                {
                    if (Condition())
                        OpenerStep++;
                }

                actionID = CurrentOpenerAction = AllowUpgradeSteps.Any(x => x == OpenerStep) ? OriginalHook(OpenerActions[OpenerStep - 1]) : OpenerActions[OpenerStep - 1];

                float startValue = (VeryDelayedWeaveSteps.Any(x => x == OpenerStep)) ? 1f : 1.25f;
                if ((DelayedWeaveSteps.Any(x => x == OpenerStep) || VeryDelayedWeaveSteps.Any(x => x == OpenerStep)) && !CanDelayedWeave(startValue))
                {
                    actionID = All.SavageBlade;
                    return true;
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

                    if ((DateTime.Now - DelayedAt).TotalSeconds < HoldDelay() && !PartyInCombat())
                    {
                        ActionWatching.TimeLastActionUsed = DateTime.Now; //Hacky workaround for TN jobs
                        actionID = All.SavageBlade;
                        return true;
                    }
                }

                if (CurrentOpenerAction == RoleActions.Melee.TrueNorth && !TargetNeedsPositionals())
                {
                    OpenerStep++;
                    CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                }

                while (OpenerStep > 1 && !ActionReady(CurrentOpenerAction) &&
                       !SkipSteps.Any(x => x.Steps.Any(y => y == OpenerStep)) &&
                       ActionWatching.TimeSinceLastAction.TotalSeconds > Math.Max(1.5, Math.Max(GCDTotal, Player.Object.TotalCastTime + 0.2f)))
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
        CurrentOpener = Player.Job switch
        {
            Job.AST => AST.Opener(),
            Job.BLM => BLM.Opener(),
            Job.BRD => BRD.Opener(),
            Job.DRG => DRG.Opener(),
            Job.DNC => DNC.Opener(),
            Job.DRK => DRK.Opener(),
            Job.GNB => GNB.Opener(),
            Job.MCH => MCH.Opener(),
            Job.MNK => MNK.Opener(),
            Job.NIN => NIN.Opener(),
            Job.PCT => PCT.Opener(),
            Job.PLD => PLD.Opener(),
            Job.RDM => RDM.Opener(),
            Job.RPR => RPR.Opener(),
            Job.SAM => SAM.Opener(),
            Job.SMN => SMN.Opener(),
            Job.SCH => SCH.Opener(),
            Job.SGE => SGE.Opener(),
            Job.VPR => VPR.Opener(),
            Job.WAR => WAR.Opener(),
            Job.WHM => WHM.Opener(),
            _ => Dummy
        };
        CurrentOpener?.CacheReady = true;
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
                Svc.Log.Debug($"Removed update hook {value.GetType()} {currentOpener.GetType()}");
            }

            if (currentOpener != value)
            {
                Svc.Log.Debug($"Setting CurrentOpener");
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
        if (CurrentOpener?.CurrentState is OpenerState.OpenerReady or OpenerState.InOpener)
        {
            if (CurrentOpener?.OpenerStep > 1 && interruptedAction == CurrentOpener.PreviousOpenerAction)
                CurrentOpener.OpenerStep -= 1;
        }
    }

    public static WrathOpener Dummy = new DummyOpener();
}

public class DummyOpener : WrathOpener
{
    public override List<uint> OpenerActions { get; set; } = [];
    public override int MinOpenerLevel => 1;
    public override int MaxOpenerLevel => 10000;

    internal override UserData? ContentCheckConfig => null;

    public override bool HasCooldowns() => false;
}