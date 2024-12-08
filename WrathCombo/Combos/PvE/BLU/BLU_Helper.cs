using System;
using System.Linq;
using WrathCombo.CustomComboNS.Functions;
using Functions = WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class BLU
{
    internal class DoTs(
        UserInt minHp,
        UserInt minTime,
        DoT[] dotsToUse)
    {
        public bool AnyDotsWanted() => dotsToUse.Any(CheckDotWanted);

        public bool CheckDotWanted(DoT dot) =>
            // Check config preset is enabled
            Functions.IsEnabled(dot.Preset()) &&
            // Check spell is ready
            Functions.IsSpellActive(dot.Action()) &&
            Functions.ActionReady(dot.Action()) &&
            !Functions.WasLastAction(dot.Action()) &&
            // Check debuff is not applied or remaining time is less than requirement
            (!Functions.TargetHasEffect(dot.Debuff()) ||
             Functions.GetDebuffRemainingTime(dot.Debuff()) <= minTime) &&
            // Check target HP is above requirement
            Functions.GetTargetHPPercent() > minHp;
    }
}

#region DoT Attributes

[AttributeUsage(AttributeTargets.Field)]
internal class DoTInfoAttribute(
    ushort debuffID,
    uint spellID,
    CustomComboPreset configPreset) : Attribute
{
    public ushort DebuffID { get; } = debuffID;
    public uint SpellID { get; } = spellID;
    public CustomComboPreset Config { get; } = configPreset;
}

internal static class DoTExtensions
{
    public static ushort Debuff(this BLU.DoT dot)
    {
        var type = typeof(BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes =
            memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).DebuffID;
    }

    public static uint Action(this BLU.DoT dot)
    {
        var type = typeof(BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes =
            memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).SpellID;
    }

    public static CustomComboPreset Preset(this BLU.DoT dot)
    {
        var type = typeof(BLU.DoT);
        var memInfo = type.GetMember(dot.ToString());
        var attributes =
            memInfo[0].GetCustomAttributes(typeof(DoTInfoAttribute), false);
        return ((DoTInfoAttribute)attributes[0]).Config;
    }
}

#endregion
