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

        if (ComboTimer <= 0)
        {
            ClearUpcomingPositional();
            return;
        }

        if (ComboAction == OriginalHook(Disembowel) && ActionLearned(ChaosThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 1);
        else if (ComboAction == OriginalHook(ChaosThrust) && ActionLearned(WheelingThrust))
            ReportUpcomingPositional(PositionalDirection.Rear, WheelingThrust, 1);
        else if (ComboAction == OriginalHook(FullThrust) && ActionLearned(FangAndClaw))
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 1);
        else if (ComboAction == OriginalHook(VorpalThrust) && ActionLearned(FullThrust) && ActionLearned(FangAndClaw))
            ReportUpcomingPositional(PositionalDirection.Flank, FangAndClaw, 2);
        else if (ComboAction is TrueThrust or RaidenThrust && ActionLearned(VorpalThrust))
        {
            var disembowelPath = ActionLearned(Disembowel) &&
                                 (ActionLearned(ChaosThrust) && ChaosDebuff is null &&
                                  CurrentTarget.CanApplyStatus(ChaoticList[OriginalHook(ChaosThrust)]) ||
                                  LocalPlayer.Status(Buffs.PowerSurge).RemainingTimeOrZero() < 15);

            if (disembowelPath && ActionLearned(ChaosThrust))
                ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(ChaosThrust), 2);
            else
                ClearUpcomingPositional();
        }
        else
            ClearUpcomingPositional();
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
