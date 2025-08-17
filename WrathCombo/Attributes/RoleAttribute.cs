using ECommons.ExcelServices;
using ECommons.GameHelpers;
using System;

namespace WrathCombo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class RoleAttribute : Attribute
{
    public JobRole Role;

    internal RoleAttribute(JobRole role)
    {
        Role = role;
    }

    internal bool PlayerIsRole()
    {
        if (Role == JobRole.All)
            return true;

        return Role == GetRoleFromJob(Player.Job);
    }

    public static JobRole GetRoleFromJob(uint job) =>
        GetRoleFromJob((Job)job);

    public static JobRole GetRoleFromJob(Job job) =>
        job switch
        {
            Job.GLA or
                Job.PLD or
                Job.MRD or
                Job.WAR or
                Job.DRK or
                Job.GNB =>
                JobRole.Tank,
            Job.CNJ or
                Job.WHM or
                Job.AST or
                Job.SCH or
                Job.SGE =>
                JobRole.Healer,
            Job.ARC or
                Job.BRD or
                Job.MCH or
                Job.DNC =>
                JobRole.RangedDPS,
            Job.THM or
                Job.BLM or
                Job.ACN or
                Job.SMN or
                Job.RDM or
                Job.PCT or
                Job.BLU =>
                JobRole.MagicalDPS,
            Job.LNC or
                Job.DRG or
                Job.PGL or
                Job.MNK or
                Job.ROG or
                Job.NIN or
                Job.SAM or
                Job.VPR or
                Job.RPR =>
                JobRole.MeleeDPS,
            Job.BTN or
                Job.MIN or
                Job.FSH =>
                JobRole.DoL,
            Job.CRP or
                Job.GSM or
                Job.LTW or
                Job.CUL or
                Job.BSM or
                Job.ARM or
                Job.ALC or
                Job.WVR =>
                JobRole.DoH,
            _ => JobRole.All,
        };
}

public enum JobRole
{
    //Please don't change the order of items All -> Healer
    //The order lines up with Role numbers from FFXIV Sheets
    All,
    Tank,
    MeleeDPS,
    RangedDPS,
    Healer,
    //
    MagicalDPS,
    DoH,
    DoL,
}