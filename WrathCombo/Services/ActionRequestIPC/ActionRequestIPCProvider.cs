using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster;

namespace WrathCombo.Services.ActionRequestIPC;
//this is static to maximize performance
public static class ActionRequestIPCProvider
{
    public static List<ActionRequest> ActionRequests = [];
    public static List<ActionRequest> ActionBlacklist = [];

    /// <summary>
    /// Requests an action to be blacklisted
    /// </summary>
    /// <param name="actionType">Action type</param>
    /// <param name="actionID">Action ID</param>
    /// <param name="timeMs">For how much time it should be blacklisted</param>
    [EzIPC]
    public static void RequestBlacklist(ActionType actionType, uint actionID, int timeMs)
    {
        ActionDescriptor descriptor = new(actionType, actionID);
        ActionBlacklist.Add(new(descriptor, Environment.TickCount64 + timeMs));
    }

    /// <summary>
    /// Resets blacklist for a specific action
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="actionID"></param>
    [EzIPC]
    public static void ResetBlacklist(ActionType actionType, uint actionID)
    {
        var descriptor = new ActionDescriptor(actionType, actionID);
        ActionBlacklist.RemoveAll(item => item.Descriptor == descriptor);
    }

    /// <summary>
    /// Clears entire blacklist
    /// </summary>
    [EzIPC]
    public static void ResetAllBlacklist()
    {
        ActionBlacklist.Clear();
    }

    public static void Initialize()
    {
        EzIPC.Init(typeof(ActionRequestIPCProvider), $"{Svc.PluginInterface.InternalName}.ActionRequest");
    }

    /// <summary>
    /// Retrieves artificial cooldown for a specific action; while doing so, purges requests that have expired
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="actionID"></param>
    /// <returns></returns>
    [EzIPC]
    public static float GetArtificialCooldown(ActionType actionType, uint actionID)
    {
        var d = new ActionDescriptor(actionType, actionID);
        var currentTick = Environment.TickCount64;
        for(var idx = ActionBlacklist.Count - 1; idx >= 0; idx--)
        {
            var maxDeadline = 0L;
            var currentRequest = ActionBlacklist[idx];
            if(!currentRequest.IsActive)
            {
                ActionBlacklist.RemoveAt(idx);
            }
            else
            {
                var currentDeadline = currentRequest.Deadline - currentTick;
                if(currentDeadline > maxDeadline) maxDeadline = currentDeadline;
            }
        }
        return 0;
    }

    [EzIPC]
    public static void RequestActionUse(ActionType actionType, uint actionID, int timeMs)
    {
        ActionDescriptor descriptor = new(actionType, actionID);
        ActionRequests.Add(new(descriptor, Environment.TickCount64 + timeMs));
    }

    [EzIPC]
    public static void ResetRequest(ActionType actionType, uint actionID)
    {
        var descriptor = new ActionDescriptor(actionType, actionID);
        ActionRequests.RemoveAll(item => item.Descriptor == descriptor);
    }

    [EzIPC]
    public static void ResetAllRequests(ActionType actionType, uint actionID)
    {
        ActionRequests.Clear();
    }

    public static IEnumerable<ActionDescriptor> GetRequestedActions()
    {
        if(ActionRequests.Count == 0)
        {
            yield break;
        }
        for(var idx = ActionRequests.Count - 1; idx >= 0; idx--)
        {
            var currentRequest = ActionRequests[idx];
            if(!currentRequest.IsActive)
            {
                ActionRequests.RemoveAt(idx);
            }
            else
            {
                yield return currentRequest.Descriptor;
            }
        }
    }

    private static uint InvokeRequest(uint originalActionId)
    {
        foreach(var action in GetRequestedActions())
        {
            if(action.ActionType == ActionType.Action)
            {
                if(CustomComboFunctions.ActionReady(action.ActionID))
                {
                    return action.ActionID;
                }
            }
        }
        return originalActionId;
    }

    public unsafe static bool TryInvoke(uint actionID, out uint newActionID, IGameObject? targetOverride = null)
    {
        newActionID = 0;

        if(Player.Object is null) return false; //Safeguard. LocalPlayer shouldn't be null at this point anyways.
        if(Player.IsDead) return false; //Don't do combos while dead

        Job classJobID = Player.Job.GetUpgradedJob();

        //OptionalTarget = targetOverride; //TODO: figure out
        uint resultingActionID = InvokeRequest(actionID);
        //OptionalTarget = null;

        if(resultingActionID == 0 || actionID == resultingActionID)
            return false;

        if(Service.Configuration.SuppressQueuedActions && !Svc.ClientState.IsPvP && ActionManager.Instance()->QueuedActionType == ActionType.Action && ActionManager.Instance()->QueuedActionId != actionID)
        {
            // todo: tauren: remember why this condition was in the if below:
            //      `&& WrathOpener.CurrentOpener?.OpenerStep <= 1`
            // todo: figure it out
            if(resultingActionID != All.SavageBlade)
                return false;
        }
        newActionID = resultingActionID;

        return true;
    }
}