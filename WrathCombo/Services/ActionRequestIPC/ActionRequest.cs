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
    /// Only applicable to action requests and only to OGCDs. If set to true, OGCD will be cast no matter what, otherwise - during next OGCD window only.
    /// </summary>
    public readonly bool Forced;

    public ActionRequest(ActionType actionType, uint actionID, long deadline) : this()
    {
        Descriptor = new(actionType, actionID);
        Deadline = deadline;
    }

    public ActionRequest(ActionDescriptor descriptor, long deadline) : this()
    {
        Descriptor = descriptor;
        Deadline = deadline;
    }

    public bool IsActive => Environment.TickCount64 < Deadline;
}