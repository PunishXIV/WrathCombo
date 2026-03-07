using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Lumina.Excel.Sheets;
using WrathCombo.Resources.Localization.UI.Misc;
using WrathCombo.Window;
using static WrathCombo.CustomComboNS.Functions.Jobs;
using static WrathCombo.Window.Text;

namespace WrathCombo.Extensions
{
    internal static class JobExtensions
    {
        public static string Shorthand(this Job job)
        {
            var sheet = Svc.Data.GetExcelSheet<ClassJob>(Text.LangFromCulture).GetRow((uint)job);
            return job switch
            {
                Job.ADV => string.Empty,
                Job.MIN or Job.BTN or Job.FSH => MiscUI.DOL,
                _ => sheet.Abbreviation.ToString()
            };
        }

        public static string Name(this Job job)
        {
            var sheet = Svc.Data.GetExcelSheet<ClassJob>(Text.LangFromCulture).GetRow((uint)job);
            string jobName = job switch
            {
                Job.ADV => MiscUI.Roles_and_Content,
                Job.MIN or Job.BTN or Job.FSH
                    => sheet.ClassJobCategory.Value.Name.ToString(),
                _ => sheet.Name.ToString()
            };

            return TextFormatting.ToTitleCase(jobName);
        }

        public static string Name(this ClassJob job)
        {
            Job j = (Job)job.RowId;
            return j.Name();
        }

        public static bool MatchesPlayerJob(this JobRole role)
        {
            if (role == JobRole.All)
                return true;

            return role == GetRoleFromJob(Player.Job);
        }

    }
}
