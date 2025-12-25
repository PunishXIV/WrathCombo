using System;
using WrathCombo.Combos.PvE;

namespace WrathCombo.Attributes;

/// <summary> Attribute designating Occult Crescent actions. </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class OccultCrescentAttribute : Attribute
{
    internal OccultCrescentAttribute(
        OccultCrescent.JobIDs job = OccultCrescent.JobIDs.N_A)
    {
        JobId = (int)job;
    }

    public int JobId { get; }
}