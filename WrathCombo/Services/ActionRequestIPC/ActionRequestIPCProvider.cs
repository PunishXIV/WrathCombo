using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WrathCombo.Services.ActionRequestIPC;
//this is static to maximize performance
public static class ActionRequestIPCProvider
{
    //public static Dictionary<ActionDescriptor, List<ActionRequest>> ActionRequests = [];
    public static Dictionary<ActionDescriptor, List<ActionRequest>> ActionBlacklist = [];

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
        if(!ActionBlacklist.TryGetValue(descriptor, out var value))
        {
            value = [];
            ActionBlacklist[descriptor] = value;
        }
        value.Add(new(Environment.TickCount64 + timeMs));
    }

    /// <summary>
    /// Resets blacklist for a specific action
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="actionID"></param>
    [EzIPC]
    public static void ResetBlacklist(ActionType actionType, uint actionID)
    {
        ActionDescriptor descriptor = new(actionType, actionID);
        ActionBlacklist.Remove(descriptor);
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
        var currentTick = Environment.TickCount64;
        if(ActionBlacklist.TryGetValue(new(actionType, actionID), out var request))
        {
            var maxDeadline = 0L;
            for(var idx = request.Count - 1; idx >= 0; idx--)
            {
                var currentRequest = request[idx];
                var currentDeadline = currentRequest.Deadline - currentTick;
                if(currentDeadline > maxDeadline) maxDeadline = currentDeadline;
                if(currentDeadline < 0) request.RemoveAt(idx);
            }
            return maxDeadline;
        }
        return 0;
    }
}