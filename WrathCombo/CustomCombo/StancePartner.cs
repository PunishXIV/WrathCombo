#region

using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Threading.Tasks;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;

#endregion

namespace WrathCombo.CustomComboNS;

public static class StancePartner
{
    /// <summary>
    ///     The number of times that <see cref="CheckStancePartner" /> has been
    ///     called.
    /// </summary>
    private static int _stancePartnerRunTries;

    /// <summary>
    ///     The Action to check if an IPC is in control, after a territory change.
    /// </summary>
    /// <seealso cref="WrathCombo.ClientState_TerritoryChanged"/>
    public static readonly Action CheckForIPCControl = () =>
    {
        // Reset run count
        _stancePartnerRunTries = 0;

        // Wait (a limited amount of time) for the screen to be ready
        PluginLog.Verbose("OnIPCInstanceChange: Waiting for screen ...");
        byte count = 0;
        while (!GenericHelpers.IsScreenReady())
        {
            if (count > 50) return;
            count++;
            Task.Delay(400).Wait();
        }

        // Wait for any IPC to seize control, e.g. AutoDuty has a delay after
        // entering an instance for the first time
        PluginLog.Verbose("OnIPCInstanceChange: Waiting for any IPC ...");
        Task.Delay(4000).Wait();

        // If IPC-Controlled: Run Check() on the next tick
        if (P.UIHelper.AutoRotationStateControlled() is not null)
        {
            PluginLog.Verbose("OnIPCInstanceChange: Is IPC-Controlled");
            Svc.Framework.RunOnTick(CheckStancePartner!);
        }
        else
            PluginLog.Verbose("OnIPCInstanceChange: Not IPC-Controlled");
    };

    /// <summary>
    ///     The action to try casting the abilities for Tank Stance / Dance Partner.
    /// </summary>
    private static readonly Action CheckStancePartner = () =>
    {
        PluginLog.Verbose("OnIPCInstanceChange: Trying to run StancePartner ..");

        // Whether we'll loop again, passed to Cast below
        var callAgainToConfirm = false;

        #region Tank Stance

        Cast(Job.PLD, PLD.IronWill, PLD.Buffs.IronWill,
            null, ref callAgainToConfirm);

        Cast(Job.WAR, WAR.Defiance, WAR.Buffs.Defiance,
            null, ref callAgainToConfirm);

        Cast(Job.DRK, DRK.Grit, DRK.Buffs.Grit,
            null, ref callAgainToConfirm);

        Cast(Job.GNB, GNB.RoyalGuard, GNB.Buffs.RoyalGuard,
            null, ref callAgainToConfirm);

        #endregion

        #region Dance Partner

        Cast(Job.DNC, DNC.ClosedPosition, DNC.Buffs.ClosedPosition,
            DNC.DesiredDancePartner, ref callAgainToConfirm);

        #endregion

        // Give up trying after 10 calls
        if (_stancePartnerRunTries > 10)
            return;

        // Loop again to re-check
        if (!callAgainToConfirm) return;
        _stancePartnerRunTries++;
        Svc.Framework.RunOnTick(CheckStancePartner!,
            TimeSpan.FromSeconds(1));
    };

    /// <summary>
    ///     Method to try to use the requested ability.<br />
    ///     Will check if on the correct job, and if the ability is ready.<br />
    ///     Will also check if the ability is already active.<br />
    ///     If the cast failed or the buff from the ability still isn't active,
    ///     it will try again.
    /// </summary>
    /// <param name="job">
    ///     Job ID, from the class.
    ///     E.G. <see cref="PLD.JobID">PLD.JobID</see>
    /// </param>
    /// <param name="action">
    ///     The ability we want to cast.
    ///     E.G. <see cref="PLD.IronWill">PLD.IronWill</see>
    /// </param>
    /// <param name="buff">
    ///     The buff we want to check for, that should come from the
    ///     <paramref name="action" />.
    ///     E.G. <see cref="PLD.Buffs.IronWill">PLD.Buffs.IronWill</see>
    /// </param>
    /// <param name="target">
    ///     The target to cast the ability on.
    ///     Default is null, which will cast on self.
    /// </param>
    /// <param name="callAgain">
    ///     A reference to whether <see cref="CheckStancePartner" /> should be called again.
    /// </param>
    private static unsafe void Cast
    (Job job, uint action, ushort buff, ulong? target, ref bool
        callAgain)
    {
        if (WrathCombo.JobID != job || CustomComboFunctions.HasStatusEffect(buff))
            return;
        PluginLog.Verbose(
            $"OnIPCInstanceChange: Trying to cast {action.ActionName()}");

        callAgain = true;

        if (CustomComboFunctions.JustUsed(action, 0.5f))
            return;
        if (!CustomComboFunctions.ActionReady(action))
            return;

        PluginLog.Verbose(
            $"OnIPCInstanceChange: Casting {action.ActionName()} {target}");

        if (target is null)
            ActionManager.Instance()->UseAction(ActionType.Action, action);
        else
            ActionManager.Instance()->UseAction(ActionType.Action, action,
                (ulong)target);
    }
}
