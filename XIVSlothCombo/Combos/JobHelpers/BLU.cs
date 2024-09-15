using System;
using System.Collections.Generic;
using System.Linq;
using ECommons.Logging;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.JobHelpers;

internal class BLU
{
    internal class DoTs(
        UserInt hpRequirement,
        UserInt timeRequirement,
        PvE.BLU.DoT[] dotsToUse)
    {
        public bool AnyDotsWanted() => dotsToUse.Any(CheckDotWanted);

        public bool CheckDotWanted(PvE.BLU.DoT dot) =>
            // Check config preset is enabled
            CustomComboFunctions.IsEnabled(dot.GetConfigPreset()) &&
            // Check spell is ready
            CustomComboFunctions.IsSpellActive(dot.GetSpellID()) &&
            // Check debuff is not applied or remaining time is less than requirement
            (!CustomComboFunctions.TargetHasEffect(dot.GetDebuffID()) ||
             CustomComboFunctions.GetDebuffRemainingTime(dot.GetDebuffID()) <= timeRequirement) &&
            // Check target HP is above requirement
            CustomComboFunctions.GetTargetHPPercent() > hpRequirement;
    }
}

#region DoT Attributes

[AttributeUsage(AttributeTargets.Field)]
internal class DoTInfoAttribute(ushort debuffID, uint spellID, CustomComboPreset configPreset) : Attribute
{
    public ushort DebuffID { get; } = debuffID;
    public uint SpellID { get; } = spellID;
    public CustomComboPreset Config { get; } = configPreset;
}

internal static class DoTExtensions
{
    public static ushort GetDebuffID(this PvE.BLU.DoT dot)
    {
        var type = typeof(PvE.BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).DebuffID;
    }

    public static uint GetSpellID(this PvE.BLU.DoT dot)
    {
        var type = typeof(PvE.BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).SpellID;
    }

    public static CustomComboPreset GetConfigPreset(this PvE.BLU.DoT dot)
    {
        var type = typeof(PvE.BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).Config;
    }
}

#endregion
