using System;
using System.Runtime.CompilerServices;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Attributes
{
    /// <summary> Attribute documenting additional information for each combo. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class CustomComboInfoAttribute : Attribute
    {
        /// <summary> Initializes a new instance of the <see cref="CustomComboInfoAttribute"/> class. </summary>
        /// <param name="name"> Display name. </param>
        /// <param name="description"> Combo description. </param>
        /// <param name="jobID"> Associated job ID. </param>
        /// <param name="order"> Display order. </param>
        /// <param name="memeName"> Display meme name </param>
        /// <param name="memeDescription"> Meme description. </param>
        internal CustomComboInfoAttribute(string name, string description, byte jobID, [CallerLineNumber] int order = 0)
        {
            Name = name;
            Description = description;
            JobID = jobID;
            Order = order;
        }

        /// <summary> Gets the display name. </summary>
        public string Name { get; }

        /// <summary> Gets the description. </summary>
        public string Description { get; }

        /// <summary> Gets the job ID. </summary>
        public byte JobID { get; }

        /// <summary> Gets the job role. </summary>
        public int Role => CustomComboFunctions.JobIDs.JobIDToRole(JobID);

        public uint ClassJobCategory => CustomComboFunctions.JobIDs.JobIDToClassJobCategory(JobID);

        /// <summary> Gets the display order. </summary>
        public int Order { get; }

        /// <summary> Gets the job name. </summary>
        public string JobName => CustomComboFunctions.JobIDs.JobIDToName(JobID);

        public string JobShorthand => CustomComboFunctions.JobIDs.JobIDToShorthand(JobID);
    }
}
