using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Data.ActionWatching;
using static WrathCombo.Window.Text;

namespace WrathCombo.Extensions;

internal static class UIntExtensions
{
    internal static bool LevelChecked(this uint value) => CustomComboFunctions.ActionLearned(value);

    internal static bool TraitLevelChecked(this uint value) => CustomComboFunctions.TraitLevelChecked(value);

    internal static string ActionName(this uint value) => ActionAndStatusLocalization.GetActionName(value);

    internal static string ItemName(this uint value) => ActionAndStatusLocalization.GetItemName(value);

    internal static ActionAttackType ActionAttackType(this uint value) => (ActionAttackType)(ActionSheet.TryGetRow(value, out var actSheet) ? actSheet.ActionCategory.RowId : 0);

    internal static float ActionRange(this uint value) =>
        ActionManager.GetActionRange(value);

    internal static bool IsGroundTargeted(this uint value) =>
        ActionSheet.TryGetRow(value, out var groundRow) && groundRow.TargetArea;

    internal static bool IsEnemyTargetable(this uint value) =>
        ActionSheet.TryGetRow(value, out var enemyRow) && enemyRow.CanTargetHostile;

    internal static bool IsFriendlyTargetable(this uint value) =>
        ActionSheet.TryGetRow(value, out var friendlyRow) && friendlyRow.CanTargetAlly;

    internal static string TraitName(this uint value) => ActionAndStatusLocalization.GetTraitName(value);
}

internal static class UShortExtensions
{
    internal static string StatusName(this ushort value) => ActionAndStatusLocalization.GetStatusName(value);

    internal static string TraitName(this ushort value) => ActionAndStatusLocalization.GetTraitName(value);
}