using WrathCombo.CustomComboNS.Functions;
namespace WrathCombo.Extensions;

internal static class ByteExtensions
{
    internal static string JobAbbreviation(this byte jobId) => CustomComboFunctions.JobIDs.JobIDToShorthand(jobId);
}