using ECommons.ExcelServices;
using System;
using System.Runtime.CompilerServices;
using WrathCombo.Extensions;
using ECommonsJob = ECommons.ExcelServices.Job;

namespace WrathCombo.Attributes;

/// <summary> Attribute documenting additional information for each combo. </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class CustomComboInfoAttribute : Attribute
{
    /// <summary> Initializes a new instance of the <see cref="CustomComboInfoAttribute"/> class. </summary>
    /// <param name="name"> Display name. </param>
    /// <param name="description"> Combo description. </param>
    /// <param name="job"> Associated job ID. </param>
    /// <param name="order"> Display order. </param>
    //// <param name="memeName"> Display meme name </param>
    //// <param name="memeDescription"> Meme description. </param>
    internal CustomComboInfoAttribute(string name, string description, Job job, [CallerLineNumber] int order = 0)
    {
        Name = name;
        Description = description;
        Job = job switch
        {
            Job.BTN or Job.MIN or Job.FSH => Job.MIN,
            _ => job
        };
        Order = order;
    }

    /// <summary> Gets the display name. </summary>
    public string Name { get; }

    /// <summary> Gets the description. </summary>
    public string Description { get; }

    /// <summary> Gets the job enum. </summary>
    public ECommonsJob Job { get; }

    /// <summary> Gets the display order. </summary>
    public int Order { get; }

    /// <summary> Gets the job role. </summary>
    public JobRole Role => RoleAttribute.GetRoleFromJob(Job);

    /// <summary> Gets the job name. </summary>
    public string JobName => Job.Name();

    /// <summary> Gets the job shorthand. </summary>
    public string JobShorthand => Job.Shorthand();
}