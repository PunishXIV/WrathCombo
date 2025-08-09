using System;

namespace WrathCombo.Attributes;

/// <summary> Attribute designating Occult Crescent actions. </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class OccultCrescentAttribute : Attribute
{
    internal OccultCrescentAttribute(int jobId = -1)
    {
        JobId = jobId;
    }

    public int JobId { get; }
}