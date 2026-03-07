using ECommons.ExcelServices;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.Jobs;
using static WrathCombo.Window.Text;

namespace WrathCombo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class JobInfoAttribute : Attribute
{
    /// <summary> Initializes a new instance of the <see cref="JobInfoAttribute"/> class. </summary>
    /// <param name="job"> Associated job ID. </param>
    /// <param name="jobRoleIcon"> Job role to use for the optional icon. Only available for top level presets. </param>
    /// <param name="order"> Display order. </param>
    ///
    internal JobInfoAttribute(
        Job job,
        JobRole jobRoleIcon = JobRole.All,
        [CallerLineNumber] int order = 0)
    {
        Job = job switch
        {
            Job.BTN or Job.MIN or Job.FSH => Job.MIN,
            _ => job
        };

        Order = order;
        Role = GetRoleFromJob(Job);
        RoleForIcon = jobRoleIcon == JobRole.All ? null : jobRoleIcon;
    }

    /// <summary> Associated job ID (with gathering jobs mapped to MIN). </summary>
    public Job Job { get; }

    /// <summary> Display order (auto-filled via CallerLineNumber). </summary>
    public int Order { get; }

    /// <summary> Gets the job role. </summary>
    public JobRole Role { get; }

    /// <summary> Gets the job role used for adding an icon override. Not used on child presets </summary>
    public JobRole? RoleForIcon { get; }

    /// <summary> Gets the job name. </summary>
    public string JobName => JobNameLocalization.GetJobName(Job);

    /// <summary> Gets the job shorthand. </summary>
    public string JobShorthand => JobNameLocalization.GetJobShortName(Job);
}
