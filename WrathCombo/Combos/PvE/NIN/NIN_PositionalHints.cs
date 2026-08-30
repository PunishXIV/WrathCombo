using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    private static void ReportNINPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        // Mudra replaces the GCDs that would be Edge/Crush — retract like MNK PB.
        if (MudraPhase)
        {
            ClearUpcomingPositional();
            return;
        }

        switch (ComboAction)
        {
            case GustSlash:
                ReportNINFinisherHint(1);
                break;

            case SpinningEdge when ActionLearned(GustSlash):
                ReportNINFinisherHint(2);
                break;

            default:
                // Spinning Edge → Gust Slash → Edge/Crush (combo start / after finisher)
                if (ActionLearned(GustSlash))
                    ReportNINFinisherHint(3);
                break;
        }
    }

    private static void ReportNINFinisherHint(int gcdsUntil)
    {
        if (gauge.Kazematoi is 0 && ActionLearned(ArmorCrush))
            ReportUpcomingPositional(PositionalDirection.Flank, ArmorCrush, gcdsUntil);
        else if (gauge.Kazematoi >= 4 && ActionLearned(AeolianEdge))
            ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
        else if (ActionLearned(ArmorCrush) && ActionLearned(AeolianEdge))
        {
            if (OnTargetsFlank() || !TargetNeedsPositionals())
                ReportUpcomingPositional(PositionalDirection.Flank, ArmorCrush, gcdsUntil);
            else
                ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
        }
        else if (ActionLearned(AeolianEdge))
            ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
    }
}
