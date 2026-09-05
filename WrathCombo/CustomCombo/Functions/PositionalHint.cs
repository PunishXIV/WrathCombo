using System;
using WrathCombo.API.Enum;
using WrathCombo.Combos.PvE.Enums;
using WrathCombo.CustomComboNS;
using WrathCombo.Services.IPC;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary> Publish an upcoming positional for overlay plugins. </summary>
    internal static void ReportUpcomingPositional(
        PositionalDirection direction,
        uint actionId,
        int gcdsUntil) =>
        UpcomingPositionalHintService.Report(direction, actionId, gcdsUntil);

    /// <summary> Retract the current upcoming positional hint. </summary>
    internal static void ClearUpcomingPositional() =>
        UpcomingPositionalHintService.Reset();

    /// <summary>
    ///     True when there is a positional battle target. Does not clear an existing hint.
    /// </summary>
    internal static bool CanReportPositionalHints() =>
        HasBattleTarget() && TargetNeedsPositionals();

    /// <summary> Report the opener step when it is a known positional action. </summary>
    internal static bool TryReportOpenerPositionalHint(
        WrathOpener opener,
        Func<uint, int, bool> tryReportAction)
    {
        if (opener.CurrentState is not OpenerState.InOpener || opener.OpenerStep < 1)
            return false;

        var action = OriginalHook(opener.CurrentOpenerAction);
        return action is not 0 && tryReportAction(action, 1);
    }
}
