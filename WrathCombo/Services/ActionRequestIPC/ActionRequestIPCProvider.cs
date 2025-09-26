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

    public static void Initialize()
    {
        EzIPC.Init(typeof(ActionRequestIPCProvider), $"{Svc.PluginInterface.InternalName}.ActionRequest");
    }

    public static float GetArtificialCooldown(ActionType actionType, uint actionID)
    {
        var currentTick = Environment.TickCount64;
        if(ActionBlacklist.TryGetValue(new(actionType, actionID), out var request))
        {
            var ret = 0L;
            for(var i = request.Count - 1; i >= 0; i--)
            {
                var x = request[i];
                var d = x.Deadline - currentTick;
                if(d > ret) ret = d;
                if(d < 0) request.RemoveAt(i);
            }
            return ret;
        }
        return 0;
    }
}