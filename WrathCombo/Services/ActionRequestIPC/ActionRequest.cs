using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WrathCombo.Services.ActionRequestIPC;
public readonly record struct ActionRequest
{
    /// <summary>
    /// Action
    /// </summary>
    public readonly ActionDescriptor Descriptor;
    /// <summary>
    /// Until what moment this ActionRequest is valid. Value is <see cref="Environment.TickCount64"/>.
    /// </summary>
    public readonly long Deadline;
    /// <summary>
    /// If not present (0), use it "as is". Not used for now.
    /// </summary>
    public readonly uint TargetEntityID;
    /// <summary>
    /// If TargetEntityID is not specified, TargetLocation must be provided for location-based skills. Not used for now.
    /// </summary>
    public readonly Vector3 TargetLocation;
    /// <summary>
    /// Whether to cast action at GCD window:
    /// true - only cast during GCD window
    /// false - only cast during OGCD window
    /// null - cast asap any time
    /// </summary>
    public readonly bool? IsGCD;

    public ActionRequest(ActionType actionType, uint actionID, long deadline, bool? isGCD) : this()
    {
        Descriptor = new(actionType, actionID);
        Deadline = deadline;
        IsGCD = isGCD;
    }

    public ActionRequest(ActionDescriptor descriptor, long deadline, bool? isGCD) : this()
    {
        Descriptor = descriptor;
        Deadline = deadline;
        IsGCD = isGCD;
    }

    public bool IsActive => Environment.TickCount64 < Deadline;
}