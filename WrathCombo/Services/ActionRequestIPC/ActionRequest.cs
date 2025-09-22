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
    /// Until what moment this ActionRequest is valid. Value is <see cref="Environment.TickCount64"/>.
    /// </summary>
    public readonly long Deadline;
    /// <summary>
    /// If not present (0), use it "as is". 
    /// </summary>
    public readonly uint TargetEntityID;
    /// <summary>
    /// If TargetEntityID is not specified, TargetLocation must be provided for location-based skills. 
    /// </summary>
    public readonly Vector3 TargetLocation;

    public ActionRequest(long deadline) : this()
    {
        Deadline = deadline;
    }

    public ActionRequest(long deadline, uint targetEntityID) : this()
    {
        Deadline = deadline;
        TargetEntityID = targetEntityID;
    }

    public ActionRequest(long deadline, Vector3 targetLocation) : this()
    {
        Deadline = deadline;
        TargetLocation = targetLocation;
    }

    public bool IsActive => Environment.TickCount64 < Deadline;
}