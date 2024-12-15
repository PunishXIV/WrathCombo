using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Attributes
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
        //// <param name="memeName"> Display meme name </param>
        //// <param name="memeDescription"> Meme description. </param>
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
        public uint JobID { get; }

        /// <summary> Gets the job role. </summary>
        public int Role => CustomComboFunctions.JobIDs.JobIDToRole(JobID);

        public uint ClassJobCategory => CustomComboFunctions.JobIDs.JobIDToClassJobCategory(JobID);

        private static int JobIDToRole(uint jobID)
        {
            if (Svc.Data.GetExcelSheet<ClassJob>().HasRow(jobID))
                return Svc.Data.GetExcelSheet<ClassJob>().GetRow(jobID).Role;

            return 0;
        }

        private static uint JobIDToClassJobCategory(uint jobID)
        {
            if (Svc.Data.GetExcelSheet<ClassJob>().HasRow(jobID))
                return Svc.Data.GetExcelSheet<ClassJob>().GetRow(jobID).ClassJobCategory.RowId;

            return 0;
        }

        /// <summary> Gets the display order. </summary>
        public int Order { get; }

        /// <summary> Gets the job name. </summary>
        public string JobName => CustomComboFunctions.JobIDs.JobIDToName(JobID);

        public string JobShorthand => JobIDToShorthand(JobID);

        private static string JobIDToShorthand(uint key)
        {
            if (key == 41)
                return "VPR";

            if (key == 0)
                return "";

            if (ClassJobs.TryGetValue(key, out var job))
            {
                return job.Abbreviation.ToString();
            }
            else
            {
                return "";
            }
        }

        private static readonly Dictionary<uint, ClassJob> ClassJobs = Svc.Data.GetExcelSheet<ClassJob>()!.ToDictionary(i => i.RowId, i => i);

        public static string JobIDToName(byte key)
        {
            if (key == 0)
                return "General/Multiple Jobs";

            //Override DOH/DOL
            if (key is DOH.JobID) key = 08; //Set to Carpenter
            if (key is DOL.JobID) key = 16; //Set to Miner
            if (ClassJobs.TryGetValue(key, out ClassJob job))
            {
                //Grab Category name for DOH/DOL, else the normal Name for the rest
                string jobname = key is 08 or 16 ? job.ClassJobCategory.Value.Name.ToString() : job.Name.ToString();
                //Job names are all lowercase by default. This capitalizes based on regional rules
                string cultureID = Svc.ClientState.ClientLanguage switch
                {
                    Dalamud.Game.ClientLanguage.French => "fr-FR",
                    Dalamud.Game.ClientLanguage.Japanese => "ja-JP",
                    Dalamud.Game.ClientLanguage.German => "de-DE",
                    _ => "en-us",
                };
                TextInfo textInfo = new CultureInfo(cultureID, false).TextInfo;
                jobname = textInfo.ToTitleCase(jobname);
                //if (key is 0) jobname = " " + jobname; //Adding space to the front of Global moves it to the top. Shit hack but works
                return jobname;

            } //Misc or unknown
            else return key == 99 ? "Global" : "Unknown";
        }
    }
}
