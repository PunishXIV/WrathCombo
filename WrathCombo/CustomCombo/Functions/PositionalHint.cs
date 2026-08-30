using System;
using WrathCombo.API.Enum;
using WrathCombo.Combos.PvE.Enums;
using WrathCombo.CustomComboNS;
using WrathCombo.Services.IPC;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary>
    ///     Reports an upcoming positional requirement for external overlay plugins.
    /// </summary>
    internal static void ReportUpcomingPositional(
        PositionalDirection direction,
        uint actionId,
        int gcdsUntil) =>
        UpcomingPositionalHintService.Report(direction, actionId, gcdsUntil);

    /// <summary>
    ///     Retracts any currently published upcoming positional hint.
    /// </summary>
    internal static void ClearUpcomingPositional() =>
        UpcomingPositionalHintService.Reset();

    /// <summary>
    ///     Returns <see langword="true"/> when there is a positional battle target.
    ///     Otherwise clears any published hint and returns <see langword="false"/>.
    /// </summary>
    internal static bool CanReportPositionalHints()
    {
        if (HasBattleTarget() && TargetNeedsPositionals())
            return true;

        ClearUpcomingPositional();
        return false;
    }

    /// <summary>
    ///     Reports the opener's current step when it is a known positional action.
    /// </summary>
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
