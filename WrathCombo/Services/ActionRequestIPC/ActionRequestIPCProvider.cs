using ECommons.EzIpcManager;
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
    public static Dictionary<ActionDescriptor, List<ActionRequest>> ActionRequests = [];
    public static Dictionary<ActionDescriptor, List<ActionRequest>> ActionBlacklist = [];

    [EzIPC]
    public static void RequestBlacklist(ActionType actionType, uint actionID, int timeMs)
    {
        ActionDescriptor d = new(actionType, actionID);
        if(!ActionBlacklist.TryGetValue(d, out var v))
        {
            v = [];
            ActionBlacklist[d] = v;
        }
        v.Add(new(Environment.TickCount64 + timeMs));
    }

    [EzIPC]
    public static void RequestAction(ActionType actionType, uint actionID, int timeMs)
    {
        ActionDescriptor d = new(actionType, actionID);
        if(!ActionRequests.TryGetValue(d, out var v))
        {
            v = [];
            ActionRequests[d] = v;
        }
        v.Add(new(Environment.TickCount64 + timeMs));
    }

    [EzIPC]
    public static void RequestActionOnTarget(ActionType actionType, uint actionID, uint targetEntityId, int timeMs)
    {
        ActionDescriptor d = new(actionType, actionID);
        if(!ActionRequests.TryGetValue(d, out var v))
        {
            v = [];
            ActionRequests[d] = v;
        }
        v.Add(new(Environment.TickCount64 + timeMs, targetEntityId));
    }

    [EzIPC]
    public static void RequestActionLocation(ActionType actionType, uint actionID, Vector3 targetLocation, int timeMs)
    {
        ActionDescriptor d = new(actionType, actionID);
        if(!ActionRequests.TryGetValue(d, out var v))
        {
            v = [];
            ActionRequests[d] = v;
        }
        v.Add(new(Environment.TickCount64 + timeMs, targetLocation));
    }
}