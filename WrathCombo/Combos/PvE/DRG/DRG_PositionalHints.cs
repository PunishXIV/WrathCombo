using WrathCombo.API.Enum;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal static void TickPositionalHints()
    {
        if (IsEnabled(Preset.DRG_ST_SimpleMode) || IsEnabled(Preset.DRG_ST_AdvancedMode))
            ReportDRGPositionalHints();
    }

    private static void ReportDRGPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        if (TryReportOpenerPositionalHint(Opener(), TryReportDRGActionPositional))
            return;

        switch (ComboAction)
        {
            case var action when action == OriginalHook(Disembowel) && ActionLearned(ChaosThrust):
                ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 1);
                break;

            case var action when action == OriginalHook(ChaosThrust) && ActionLearned(WheelingThrust):
                ReportUpcomingPositional(PositionalDirection.Rear, WheelingThrust, 1);
                break;

            case var action when action == OriginalHook(FullThrust) && ActionLearned(FangAndClaw):
                ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 1);
                break;

            case var action when action == OriginalHook(VorpalThrust) &&
                                 ActionLearned(FullThrust) && ActionLearned(FangAndClaw):
                ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 2);
                break;

            case TrueThrust or RaidenThrust when ActionLearned(VorpalThrust):
                ReportDRGPathAfterTrueThrust();
                break;

            case FangAndClaw or WheelingThrust:
                ClearUpcomingPositional();
                break;

            default:
                ReportDRGFreshComboPath();
                break;
        }
    }

    private static bool IsDisembowelPath() =>
        ActionLearned(Disembowel) &&
        (ActionLearned(ChaosThrust) && ChaosDebuff is null &&
         CurrentTarget.CanApplyStatus(ChaoticList[OriginalHook(ChaosThrust)]) ||
         LocalPlayer.Status(Buffs.PowerSurge).RemainingTimeOrZero() < 15);

    private static void ReportDRGPathAfterTrueThrust()
    {
        if (IsDisembowelPath() && ActionLearned(ChaosThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 2);
        else if (ActionLearned(FangAndClaw))
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 3);
        else
            ClearUpcomingPositional();
    }

    // Fang is 4 GCDs from a fresh True Thrust (beyond the API max of 3).
    private static void ReportDRGFreshComboPath()
    {
        if (IsDisembowelPath() && ActionLearned(ChaosThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 3);
        else
            ClearUpcomingPositional();
    }

    private static bool TryReportDRGActionPositional(uint action, int gcdsUntil)
    {
        switch (action)
        {
            case var _ when action == OriginalHook(ChaosThrust):
                ReportUpcomingPositional(PositionalDirection.Rear, action, gcdsUntil);
                return true;

            case WheelingThrust:
                ReportUpcomingPositional(PositionalDirection.Rear, WheelingThrust, gcdsUntil);
                return true;

            case FangAndClaw:
                ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, gcdsUntil);
                return true;

            default:
                return false;
        }
    }
}
