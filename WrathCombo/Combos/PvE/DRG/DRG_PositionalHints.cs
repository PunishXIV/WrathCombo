using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    private static void ReportDRGPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        if (TryReportOpenerPositionalHint(Opener(), TryReportDRGActionPositional))
            return;

        if (ComboAction == OriginalHook(Disembowel) && ActionLearned(ChaosThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 1);
        else if (ComboAction == OriginalHook(ChaosThrust) && ActionLearned(WheelingThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, WheelingThrust, 1);
        else if (ComboAction == OriginalHook(FullThrust) && ActionLearned(FangAndClaw))
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 1);
        else if (ComboAction == OriginalHook(VorpalThrust) && ActionLearned(FullThrust) && ActionLearned(FangAndClaw))
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 2);
        else if (ComboAction is TrueThrust or RaidenThrust && ActionLearned(VorpalThrust))
            TryReportDRGPathAfterTrueThrust();
        else
            // True Thrust → … (combo start / after Fang/Wheeling)
            TryReportDRGFreshComboPath();
    }

    private static bool IsDisembowelPath() =>
        ActionLearned(Disembowel) &&
        (ActionLearned(ChaosThrust) && ChaosDebuff is null &&
         CurrentTarget.CanApplyStatus(ChaoticList[OriginalHook(ChaosThrust)]) ||
         LocalPlayer.Status(Buffs.PowerSurge).RemainingTimeOrZero() < 15);

    /// <summary>
    ///     After True/Raiden: Disembowel→Chaos (2) or Vorpal→Full→Fang (3).
    /// </summary>
    private static void TryReportDRGPathAfterTrueThrust()
    {
        if (IsDisembowelPath() && ActionLearned(ChaosThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 2);
        else if (ActionLearned(FangAndClaw))
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 3);
    }

    /// <summary>
    ///     Before True Thrust: Chaos is 3 GCDs (TT→Disembowel→Chaos).
    ///     Fang is 4 (TT→Vorpal→FT→Fang) — outside the API max, so skip until after TT.
    /// </summary>
    private static void TryReportDRGFreshComboPath()
    {
        if (IsDisembowelPath() && ActionLearned(ChaosThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 3);
    }

    private static bool TryReportDRGActionPositional(uint action, int gcdsUntil)
    {
        if (action == OriginalHook(ChaosThrust))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, action, gcdsUntil);
            return true;
        }

        if (action == WheelingThrust)
        {
            ReportUpcomingPositional(PositionalDirection.Rear, WheelingThrust, gcdsUntil);
            return true;
        }

        if (action == FangAndClaw)
        {
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, gcdsUntil);
            return true;
        }

        return false;
    }
}
