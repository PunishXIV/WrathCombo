using ECommons.EzIpcManager;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Services.IPC;

public partial class Provider
{
    [EzIPC]
    public bool CanWeave(float? estimatedWeaveTime)
    {
        estimatedWeaveTime ??= CustomComboFunctions.BaseAnimationLock;
        return CustomComboFunctions.CanWeave(estimatedWeaveTime.Value);
    }

    [EzIPC]
    public bool CanDelayedWeave(float? weaveStart, float? weaveEnd)
    {
        weaveStart ??= 1.25f;
        weaveEnd ??= CustomComboFunctions.BaseAnimationLock;
        return CustomComboFunctions.CanDelayedWeave(weaveStart.Value, weaveEnd.Value);
    }

    [EzIPC]
    public bool ActionReady(uint actionId, bool? recastCheck, bool? castCheck)
    {
        recastCheck ??= false;
        castCheck ??= false;
        return CustomComboFunctions.ActionReady(actionId, recastCheck.Value, castCheck.Value);
    }

    [EzIPC]
    public bool JustUsed(uint actionID, float? variance)
    {
        variance ??= 3f;
        return CustomComboFunctions.JustUsed(actionID, variance.Value);
    }
}
