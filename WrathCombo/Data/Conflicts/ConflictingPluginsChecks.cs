#region

using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Linq;
using Dalamud.Game.Config;
using WrathCombo.API.Enum;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services.IPC_Subscriber;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Data.Conflicts;

public static class ConflictingPluginsChecks
{
    private static bool _cancelConflictChecks;
    
    internal static readonly Action ForceRunChecks = () =>
    {
        if (_cancelConflictChecks)
            return;

        PluginLog.Verbose(
            "[ConflictingPlugins] Forcing immediate check for conflicting plugins");

        BossMod.CheckForConflict(true);
        BossModReborn.CheckForConflict(true);
        Redirect.CheckForConflict(true);
        ReAction.CheckForConflict(true);
        ReActionEx.CheckForConflict(true);
        MOAction.CheckForConflict(true);
        XIV.CheckForConflict(true);
    };

    private static readonly Action RunChecks = () =>
    {
        if (_cancelConflictChecks)
            return;

        PluginLog.Verbose(
            "[ConflictingPlugins] Periodic check for conflicting plugins");

        BossMod.CheckForConflict();
        BossModReborn.CheckForConflict();
        Redirect.CheckForConflict();
        ReAction.CheckForConflict();
        ReActionEx.CheckForConflict();
        MOAction.CheckForConflict();
        XIV.CheckForConflict();

        Svc.Framework.RunOnTick(RunChecks!, TS.FromSeconds(4.11));
    };

    internal static BossModCheck BossMod { get; } = new();
    internal static BossModCheck BossModReborn { get; } = new(true);
    internal static RedirectCheck Redirect { get; } = new();
    internal static ReActionCheck ReAction { get; } = new();
    internal static ReActionCheck ReActionEx { get; } = new(true);
    internal static MOActionCheck MOAction { get; } = new();
    internal static XIVCheck XIV { get; } = new();

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
        _cancelConflictChecks = true;
        BossMod.Dispose();
        BossModReborn.Dispose();
        Redirect.Dispose();
        ReAction.Dispose();
        ReActionEx.Dispose();
        MOAction.Dispose();
        XIV.Dispose();
    }

    internal sealed class BossModCheck(bool reborn = false)
        : ConflictCheck(!reborn
            ? new BossModIPC("BossMod", new Version(0, 3, 1, 0))
            : new BossModIPC("BossModReborn", new Version(7, 2, 5, 90)))
    {
        private DateTime? _conflictFirstSeen;
        private DateTime? _conflictRegistered;
        private int _conflictsInARow;
        private int _maxConflictsInARow = 4;

        public bool TargetingSettingConflicted;
        public bool QueueSettingConflicted;

        protected override BossModIPC IPC => (BossModIPC)_ipc;

        public override void CheckForConflict(bool forceRefresh = false)
        {
            if (!ThrottlePassed(8, false, forceRefresh))
                return;
#if DEBUG
            _maxConflictsInARow = 1;
#endif

            // Reset the conflict timer, must exceed the threshold within 2 minutes
            if (_conflictFirstSeen is not null &&
                DateTime.Now - _conflictFirstSeen > TS.FromMinutes(2))
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
                    $"[ConflictingPlugins] [{Name}] IPC not enabled, " +
                    $"or config updated");
                Conflicted = false;
                _conflictsInARow = 0;
                _conflictRegistered = null;
                return;
            }

            // Check for a targeting conflict
            TargetingSettingConflicted =
                IPC.IsAutoTargetingEnabled() &&
                AutoRotationController.cfg.DPSRotationMode != DPSRotationMode.Manual;

            // Check for a queue conflict
            QueueSettingConflicted = IPC.IsUsingCustomQueuing();

            // Check for a combo conflict
            if (IPC.HasAutomaticActionsQueued())
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Actions are Queued");
                _conflictFirstSeen ??= DateTime.Now; // Set the time seen if empty
                _conflictsInARow++;
            }

            // Save a complete conflict
            // ReSharper disable once InvertIf
            if (_conflictsInARow > _maxConflictsInARow)
            {
                _conflictRegistered = DateTime.Now;
                MarkConflict();
            }
        }
    }

    internal sealed class MOActionCheck() : ConflictCheck(new MOActionIPC())
    {
        public uint[] ConflictingActions = [];
        protected override MOActionIPC IPC => (MOActionIPC)_ipc;

        public override void CheckForConflict(bool forceRefresh = false)
        {
            if (!ThrottlePassed(forceRefresh: forceRefresh))
                return;

            var moActionRetargeted = IPC.GetRetargetedActions().ToHashSet();
            if (moActionRetargeted.Count != 0)
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] {moActionRetargeted.Count} Retargeted Actions Found");

            var wrathRetargeted = PresetStorage.AllRetargetedActions.ToHashSet();
            if (moActionRetargeted.Overlaps(wrathRetargeted))
            {
                ConflictingActions =
                    moActionRetargeted.Intersect(wrathRetargeted).ToArray();
                MarkConflict();
            }
            else
            {
                ConflictingActions = [];
                Conflicted = false;
            }
        }
    }

    internal sealed class RedirectCheck() : ConflictCheck(new RedirectIPC())
    {
        /// <summary>
        ///     The meta actions and actual actions that conflict with Wrath.
        /// </summary>
        /// <remarks>
        ///     <b>Key <c>0</c></b> is Ground Targeting enabled meta action,<br />
        ///     <b>Key <c>1</c></b> is Beneficial Actions enabled meta action,<br />
        ///     <b>Key <c>3</c>+</b> are all overlapping action retargets.
        /// </remarks>
        public uint[] ConflictingActions = [0, 0];

        public bool BunnyConflict;

        private DateTime _lastBunnyReload = DateTime.Now;

        protected override RedirectIPC IPC => (RedirectIPC)_ipc;

        public override void CheckForConflict(bool forceRefresh = false)
        {
            if (!IPC.IsEnabled)
            {
                BunnyConflict = false;
                _lastBunnyReload = DateTime.Now;
            }
            if (!ThrottlePassed(forceRefresh: forceRefresh))
                return;

            ConflictingActions = [0, 0];

            // Check if the user has bunny recently
            if (CustomComboFunctions.JustUsed(NIN.Rabbit, 45) &&
                DateTime.Now - _lastBunnyReload > TS.FromSeconds(30))
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Recent Bunny Detected");
                BunnyConflict = true;
            }
            else
                BunnyConflict = false;

            var conflictedThisCheck = false;
            var wrathRetargeted = PresetStorage.AllRetargetedActions.ToHashSet();

            // Check if all Ground Targeted Actions are redirected
            if (IPC.AreGroundTargetedActionsRedirected() &&
                wrathRetargeted.Any(x => x.IsGroundTargeted()))
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Ground Targeted Actions are Redirected");
                ConflictingActions[0] = 1;
                MarkConflict();
                conflictedThisCheck = true;
            }

            // Check if all Beneficial Actions are redirected
            if (IPC.AreBeneficialActionsRedirected() &&
                wrathRetargeted.Any(x => x.IsFriendlyTargetable()))
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Beneficial Actions are Redirected");
                ConflictingActions[1] = 1;
                MarkConflict();
                conflictedThisCheck = true;
            }

            // Check for individual Actions Retargeted
            var redirectRetargeted = IPC.GetRetargetedActions().ToHashSet();
            if (redirectRetargeted.Count != 0)
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] {redirectRetargeted.Count} " +
                    "Retargeted Actions Found");
            if (redirectRetargeted.Overlaps(wrathRetargeted))
            {
                var intersection = redirectRetargeted.Intersect(wrathRetargeted)
                    .ToArray();
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] " +
                    $"{intersection.Length} Overlapping Retargeted Actions Found");
                ConflictingActions =
                    ConflictingActions.Concat(intersection).ToArray();
                MarkConflict();
                conflictedThisCheck = true;
            }

            // Remove conflict if none were found this check
            // ReSharper disable once InvertIf
            if (!conflictedThisCheck && Conflicted)
                Conflicted = false;
        }
    }

    internal sealed class ReActionCheck(bool expanded = false)
        : ConflictCheck(!expanded
            ? new ReActionIPC("ReAction", new Version(1, 3, 4, 1))
            : new ReActionIPC("ReActionEx", new Version(1, 0, 0, 8)))
    {
        /// <summary>
        ///     The meta actions and actual actions that conflict with Wrath.
        /// </summary>
        /// <remarks>
        ///     <b>Key <c>0</c></b> is Auto Targeting enabled meta action,<br />
        ///     <b>Key <c>1</c></b> is All Actions enabled meta action,<br />
        ///     <b>Key <c>2</c></b> is Harmful Actions enabled meta action,<br />
        ///     <b>Key <c>3</c></b> is Beneficial Actions enabled meta action,<br />
        ///     <b>Key <c>4</c>+</b> are all overlapping action retargets.
        /// </remarks>
        public (uint Action, string stackName)[] ConflictingActions = [];

        protected override ReActionIPC IPC => (ReActionIPC)_ipc;

        public override void CheckForConflict(bool forceRefresh = false)
        {
            if (!ThrottlePassed(forceRefresh: forceRefresh))
                return;

            ConflictingActions = [];
            var conflictedThisCheck = false;
            var wrathRetargeted = PresetStorage.AllRetargetedActions.ToHashSet();
            // ReSharper disable once InlineOutVariableDeclaration
            string stackName;

            #region Auto Targeting Enabled

            if (IPC.IsAutoTargetingEnabled() &&
                AutoRotationController.cfg.DPSRotationMode != DPSRotationMode.Manual)
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Auto Targeting is Enabled");
                ConflictingActions = [(1, "")];
                MarkConflict();
                conflictedThisCheck = true;
            }
            else
                ConflictingActions = [(0, "")];

            #endregion

            #region All Actions Retargeted

            if (IPC.AreAllActionsRetargeted(out stackName) &&
                wrathRetargeted.Count > 0)
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] All Actions are Retargeted");
                ConflictingActions = ConflictingActions
                    .Concat([(1u, stackName)]).ToArray();
                MarkConflict();
                conflictedThisCheck = true;
            }
            else
                ConflictingActions = ConflictingActions.Concat([(0u, "")]).ToArray();

            #endregion

            #region All Harmful Actions Retargeted

            if (IPC.AreHarmfulActionsRetargeted(out stackName) &&
                wrathRetargeted.Any(x => x.IsEnemyTargetable()))
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Harmful Actions are Retargeted");
                ConflictingActions = ConflictingActions
                    .Concat([(1u, stackName)]).ToArray();
                MarkConflict();
                conflictedThisCheck = true;
            }
            else
                ConflictingActions = ConflictingActions.Concat([(0u, "")]).ToArray();

            #endregion

            #region All Beneficial Actions Retargeted

            if (IPC.AreBeneficialActionsRetargeted(out stackName) &&
                wrathRetargeted.Any(x => x.IsFriendlyTargetable()))
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] " +
                    $"Beneficial Actions are Retargeted");
                ConflictingActions = ConflictingActions
                    .Concat([(1u, stackName)]).ToArray();
                MarkConflict();
                conflictedThisCheck = true;
            }
            else
                ConflictingActions = ConflictingActions.Concat([(0u, "")]).ToArray();

            #endregion

            #region Individual Retargeted Actions Overlap

            var reactionRetargeted = IPC.GetRetargetedActions();
            if (reactionRetargeted.Length > 0)
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] {reactionRetargeted.Length} " +
                    "Retargeted Actions Found");

            var intersection = reactionRetargeted
                .Where(x => wrathRetargeted.Contains(x.Action))
                .ToArray();

            if (intersection.Length > 0)
            {
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] " +
                    $"{intersection.Length} Overlapping Retargeted Actions Found");
                ConflictingActions =
                    ConflictingActions.Concat(intersection).ToArray();
                MarkConflict();
                conflictedThisCheck = true;
            }

            #endregion

            // Remove conflict if none were found this check
            // ReSharper disable once InvertIf
            if (!conflictedThisCheck && Conflicted)
                Conflicted = false;
        }
    }

    internal sealed class XIVCheck() : ConflictCheck()
    {
        protected override ReusableIPC IPC => null!;

        public bool GroundTargetingPlacementConflicted;
        public bool AutoFaceTargetConflicted;

        public override void CheckForConflict(bool forceRefresh = false)
        {
            if (!ThrottlePassed(forceRefresh: forceRefresh))
                return;

            #region Ground Targeting Placement Conflict

            bool doublePressGroundActions;
            try
            {
                if (!Svc.GameConfig.TryGet(
                        UiConfigOption.GroundTargetActionExcuteType,
                        out doublePressGroundActions))
                    throw new AccessViolationException();
            }
            catch
            {
                PluginLog.Warning(
                    $"[ConflictingPlugins] [{Name}] " +
                    $"Could not access UIConfig.DoublePressGroundActions");
                doublePressGroundActions = false;
            }
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{Name}] `UIConfig.DoublePressGroundActions`: {doublePressGroundActions}");
            
            var wrathRetargeted = PresetStorage.AllRetargetedActions.ToHashSet();
            
            GroundTargetingPlacementConflicted =
                doublePressGroundActions &&
                wrathRetargeted.Any(x => x.IsGroundTargeted());

            #endregion

            #region Auto Face Target Conflict

            bool autoFaceEnabled;
            try
            {
                if (!Svc.GameConfig.TryGet(
                        UiControlOption.AutoFaceTargetOnAction,
                        out autoFaceEnabled))
                    throw new AccessViolationException();
            }
            catch
            {
                PluginLog.Warning(
                    $"[ConflictingPlugins] [{Name}] " +
                    $"Could not access UIControl.AutoFace");
                autoFaceEnabled = false;
            }
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{Name}] `UIControl.AutoFace`: {autoFaceEnabled}");
            
            AutoFaceTargetConflicted = !autoFaceEnabled;

            #endregion

            if (GroundTargetingPlacementConflicted || AutoFaceTargetConflicted)
                MarkConflict();
            else
                Conflicted = false;
        }
    }

    internal abstract class ConflictCheck : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        protected readonly ReusableIPC _ipc;

        protected ConflictCheck()
        {
            _ipc = new XIVSettingsIPC();
            PluginLog.Verbose(
                $"[ConflictingPlugins] [{Name}] Setup for Checking");
        }

        protected ConflictCheck(ReusableIPC ipc)
        {
            _ipc = ipc;
            if (_ipc.IsEnabled)
                PluginLog.Verbose(
                    $"[ConflictingPlugins] [{Name}] Setup for Checking " +
                    $"(v{ipc.InstalledVersion})");
        }

        // ReSharper disable once UnusedMemberInSuper.Global
        protected abstract ReusableIPC IPC { get; }

        public bool Conflicted { get; protected set; }

        protected string Name => _ipc.PluginName;

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public virtual void Dispose() => _ipc.Dispose();

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract void CheckForConflict(bool forceRefresh = false);

        /// <summary>
        ///     Checks if an EZ Throttle passes, and if the plugin is enabled.
        /// </summary>
        /// <param name="frequency">
        ///     The frequency - in seconds - that must have passed since the last
        ///     check.
        /// </param>
        /// <param name="enabledCheck">
        ///     Whether to check if the plugin is enabled as well.
        /// </param>
        /// <param name="forceRefresh">
        ///     Whether to skip the throttle check.
        /// </param>
        /// <returns>
        ///     If the <see cref="CheckForConflict" /> should be run or not.
        /// </returns>
        protected bool ThrottlePassed
            (int frequency = 5, bool enabledCheck = true, bool forceRefresh = false)
        {
            if (!EZ.Throttle($"conflictCheck{Name}",
                    TS.FromSeconds(frequency)) &&
                !forceRefresh)
                return false;
            if (enabledCheck && !_ipc.IsEnabled)
            {
                Conflicted = false;
                return false;
            }

            PluginLog.Verbose($"[ConflictingPlugins] [{Name}] Performing Check ...");

            return true;
        }

        /// <summary>
        ///     Marks the plugin as conflicted, and logs the event.
        /// </summary>
        protected void MarkConflict()
        {
            if (!Conflicted)
                PluginLog.Information($"[ConflictingPlugins] [{Name}] " +
                                      "Marked Conflict!");
            Conflicted = true;
        }

        private class XIVSettingsIPC() : ReusableIPC("XIV", new Version(0, 0))
        {
        }
    }
}