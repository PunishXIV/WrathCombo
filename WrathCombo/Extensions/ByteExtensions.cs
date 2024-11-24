using Dalamud.Utility;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrathCombo.Extensions
{
    internal static class ByteExtensions
    {
        internal static string JobAbbreviation(this byte job)
        {
            return Svc.Data.Excel.GetSheet<ClassJob>().GetRow(job).Abbreviation.ToDalamudString().TextValue;
        }
    }
}
