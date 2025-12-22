using System;
using System.Collections.Generic;
using WrathCombo.Services;

namespace WrathCombo.Attributes;

/// <summary> Attribute documenting which skill the feature uses the user does not have active currently. </summary>
[AttributeUsage(AttributeTargets.Field)]
public class BlueInactiveAttribute : Attribute
{
    /// <summary> List of each action the feature uses the user does not have active. Initializes a new instance of the <see cref="BlueInactiveAttribute"/> class. </summary>
    /// <param name="actionIDs"> List of actions the preset replaces. </param>
    internal BlueInactiveAttribute(params uint[] actionIDs)
    {
        foreach (uint id in actionIDs)
        {
            MasterActions.Add(id);
        }
    }

    internal void GetActions()
    {
        NoneSet = true;
        Actions.Clear();
        foreach (var action in MasterActions)
        {
            if (Service.Configuration.ActiveBLUSpells.Contains(action))
            {
                NoneSet = false;
                continue;
            }

            Actions.Add(action);
        }
    }
    internal List<uint> Actions { get; set; } = [];
    internal List<uint> MasterActions { get; set; } = [];
    internal bool NoneSet { get; set; } = false;
}

/// <summary>
///     Flags a Preset as a BLU DPS roled one.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class BlueDPSAttribute : Attribute
{
}

/// <summary>
///     Flags a Preset as a BLU DPS roled one.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class BlueTankAttribute : Attribute
{
}

/// <summary>
///     Flags a Preset as a BLU DPS roled one.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class BlueHealerAttribute : Attribute
{
}