using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrathCombo.Services.ActionRequestIPC;
public readonly record struct ActionDescriptor
{
    public readonly ActionType ActionType;
    public readonly uint ActionID;

    public ActionDescriptor(ActionType actionType, uint actionID)
    {
        ActionType = actionType;
        ActionID = actionID;
    }
}