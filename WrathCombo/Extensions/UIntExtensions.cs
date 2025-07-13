using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Data.ActionWatching;

namespace WrathCombo.Extensions;

internal static class UIntExtensions
{
    internal static bool LevelChecked(this uint value) => CustomComboFunctions.LevelChecked(value);

    internal static bool TraitLevelChecked(this uint value) => CustomComboFunctions.TraitLevelChecked(value);

    internal static string ActionName(this uint value) => CustomComboFunctions.GetActionName(value);

    internal static int Role(this uint value) => CustomComboFunctions.JobIDs.JobIDToRole(value);

    internal static ActionAttackType ActionAttackType(this uint value) => (ActionAttackType)ActionSheet[value].ActionCategory.RowId;
}

internal static class UShortExtensions
{
    internal static string StatusName(this ushort value) => StatusCache.GetStatusName(value);
}