using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Determine if the given preset is enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is enabled. </returns>
        public static bool IsEnabled(CustomComboPreset preset) => (int)preset < 100 || PresetStorage.IsEnabled(preset);

        /// <summary> Determine if the given preset is not enabled. </summary>
        /// <param name="preset"> Preset to check. </param>
        /// <returns> A value indicating whether the preset is not enabled. </returns>
        public static bool IsNotEnabled(CustomComboPreset preset) => !IsEnabled(preset);

        public class JobIDs
        {
            public static int JobIDToRole(byte jobID)
            {
                if (Svc.Data.GetExcelSheet<ClassJob>().HasRow(jobID))
                    return Svc.Data.GetExcelSheet<ClassJob>().GetRow(jobID).Role;

                return 0;
            }

            public static string JobIDToShorthand(byte key)
            {
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

            public static uint JobIDToClassJobCategory(byte jobID)
            {
                if (Svc.Data.GetExcelSheet<ClassJob>().HasRow(jobID))
                    return Svc.Data.GetExcelSheet<ClassJob>().GetRow(jobID).ClassJobCategory.RowId;

                return 0;
            }

            //  Job IDs     ClassIDs (no jobstone) (Lancer, Pugilist, etc)
            public static readonly List<byte> Melee =
            [
                DRG.JobID, DRG.ClassID,
                MNK.JobID, MNK.ClassID,
                NIN.JobID, NIN.ClassID,
                VPR.JobID,
                RPR.JobID,
                SAM.JobID
            ];

            public static readonly List<byte> Ranged =
            [
                BLM.JobID, BLM.ClassID,
                BRD.JobID, BRD.ClassID,
                SMN.JobID, SMN.ClassID,
                PCT.JobID,
                MCH.JobID,
                RDM.JobID,
                DNC.JobID,
                BLU.JobID
            ];

            public static readonly List<byte> Tank =
            [
                PLD.JobID, PLD.ClassID,
                WAR.JobID, WAR.ClassID,
                DRK.JobID,
                GNB.JobID
            ];

            public static readonly List<byte> Healer =
            [
                WHM.JobID, WHM.ClassID,
                SCH.JobID,
                AST.JobID,
                SGE.JobID
            ];

            public static byte JobToClass(uint jobID)
            {
                return jobID switch
                {
                    ADV.JobID => ADV.ClassID,
                    BLM.JobID => BLM.ClassID,
                    BRD.JobID => BRD.ClassID,
                    DRG.JobID => DRG.ClassID,
                    MNK.JobID => MNK.ClassID,
                    NIN.JobID => NIN.ClassID,
                    PLD.JobID => PLD.ClassID,
                    SCH.JobID => SCH.ClassID,
                    SMN.JobID => SMN.ClassID,
                    WAR.JobID => WAR.ClassID,
                    WHM.JobID => WHM.ClassID,
                    _ => (byte)jobID,
                };
            }

            public static byte ClassToJob(uint classId)
            {
                return classId switch
                {
                    ADV.ClassID => ADV.JobID,
                    BLM.ClassID => BLM.JobID,
                    BRD.ClassID => BRD.JobID,
                    DRG.ClassID => DRG.JobID,
                    MNK.ClassID => MNK.JobID,
                    NIN.ClassID => NIN.JobID,
                    PLD.ClassID => PLD.JobID,
                    SMN.ClassID => SMN.JobID,
                    WAR.ClassID => WAR.JobID,
                    WHM.ClassID => WHM.JobID,
                    _ => (byte)classId,
                };
            }

        }
    }
}
