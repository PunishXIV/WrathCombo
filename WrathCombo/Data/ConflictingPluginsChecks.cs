#region

using System;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Logging;
using WrathCombo.Combos.PvE;
using WrathCombo.Services.IPC_Subscriber;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Data;

public static class ConflictingPluginsChecks
{
    private static bool _cancelConflictChecks;

    private static readonly Action RunChecks = () =>
    {
        if (_cancelConflictChecks)
            return;

        PluginLog.Verbose(
            "[ConflictingPlugins] Periodic check for conflicting plugins");
        BossMod.CheckForConflict();
        BossModReborn.CheckForConflict();
        MOAction.CheckForConflict();

        Svc.Framework.RunOnTick(RunChecks!, TS.FromSeconds(4.11));
    };

    internal static BossModCheck BossMod { get; } = new();
    internal static BossModCheck BossModReborn { get; } = new(true);
    internal static MOActionCheck MOAction { get; } = new();

    public static void Begin()
    {
        // ReSharper disable once RedundantAssignment
        var ts = TS.FromMinutes(1); // 1m initial delay after plugin launch
#if DEBUG
        ts = TS.FromSeconds(10); // 10s for debug mode
#endif

        Svc.Framework.RunOnTick(RunChecks, ts);
    }

    public static void Dispose()
    {
        BossMod.Dispose();
        BossModReborn.Dispose();
        MOAction.Dispose();
        _cancelConflictChecks = true;
    }

    internal sealed class BossModCheck(bool reborn = false)
        : ConflictCheck(reborn
            ? new BossModIPC()
            : new BossModIPC("BossModReborn", new Version(7, 2, 5, 90)))
    {
        private DateTime? _conflictFirstSeen;
        private DateTime? _conflictRegistered;
        private int _conflictsInARow;
        private int _maxConflictsInARow = 4;

        protected override BossModIPC IPC => (BossModIPC)_ipc;

        public override void CheckForConflict()
        {
            // Throttle the check
            if (!EZ.Throttle($"conflictCheck{Name}", TS.FromSeconds(8)) ||
                !Svc.Condition[ConditionFlag.InCombat])
                return;

            PluginLog.Verbose($"[ConflictingPlugins] [{Name}] Performing Check ...");

#if DEBUG
            _maxConflictsInARow = 1;
#endif

            // Reset the conflict timer, must exceed the threshold within 2 minutes
            if (_conflictFirstSeen is not null &&
                _conflictFirstSeen - DateTime.Now > TS.FromMinutes(2))
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Resetting Conflict Check");
                _conflictFirstSeen = null;
                _conflictsInARow = 0;
            }

            // Clear the conflict
            if (!IPC.IsEnabled || // disabled
                (_conflictRegistered is not null && // there is a conflict marked
                 IPC.LastModified() > _conflictRegistered)) // bm config changed
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] IPC not enabled, or config updated");
                Conflicted = false;
                _conflictsInARow = 0;
                return;
            }

            // Add a conflict note
            if (IPC.HasAutomaticActionsQueued())
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Actions are Queued");
                _conflictFirstSeen ??= DateTime.Now; // Set the time seen if empty
                _conflictsInARow++;
            }

            // Save a complete conflict
            if (_conflictsInARow > _maxConflictsInARow)
            {
                PluginLog.Debug($"[ConflictingPlugins] [{Name}] Marked Conflict!");
                Conflicted = true;
                _conflictRegistered = DateTime.Now;
            }
        }
    }

    internal sealed class MOActionCheck() : ConflictCheck(new MOAction())
    {
        protected override MOAction IPC => (MOAction)_ipc;

        public override void CheckForConflict()
        {
            // Throttle the check
            if (!EZ.Throttle($"conflictCheck{Name}", TS.FromSeconds(20)))
                return;
            
            PluginLog.Verbose($"[ConflictingPlugins] [{Name}] Performing Check ...");

            var retargetedActions = IPC.GetRetargetedActions();
            if (retargetedActions.Contains(DRK.BlackestNight))
                Conflicted = true;
        }
    }

    internal abstract class ConflictCheck : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        protected readonly ReusableIPC _ipc;

        protected ConflictCheck(ReusableIPC ipc)
        {
            _ipc = ipc;
            if (_ipc.IsEnabled)
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Setup for Checking " +
                    $"(v{ipc.InstalledVersion})");
        }

        // ReSharper disable once UnusedMemberInSuper
        protected abstract ReusableIPC IPC { get; }

        public bool Conflicted { get; protected set; }

        protected string Name => _ipc.PluginName;

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public virtual void Dispose() => _ipc.Dispose();

        public abstract void CheckForConflict();
    }
}