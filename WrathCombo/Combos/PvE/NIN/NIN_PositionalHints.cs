using WrathCombo.API.Enum;
using static WrathCombo.Combos.PvE.NIN.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal static void TickPositionalHints()
    {
        if (IsEnabled(Preset.NIN_ST_SimpleMode) || IsEnabled(Preset.NIN_ST_AdvancedMode))
            ReportNINPositionalHints();
    }

    private static void ReportNINPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        if (MudraPhase)
        {
            ClearUpcomingPositional();
            return;
        }

        if (TryReportOpenerPositionalHint(Opener(), TryReportNINActionPositional))
            return;

        switch (ComboAction)
        {
            case GustSlash:
                ReportNINFinisherHint(1);
                break;

            case SpinningEdge when ActionLearned(GustSlash):
                ReportNINFinisherHint(2);
                break;

            default:
                if (ActionLearned(GustSlash))
                    ReportNINFinisherHint(3);
                else
                    ClearUpcomingPositional();
                break;
        }
    }

    private static void ReportNINFinisherHint(int gcdsUntil)
    {
        int burnHp = IsEnabled(Preset.NIN_ST_AdvancedMode)
            ? NIN_ST_AdvancedMode_BurnKazematoi
            : 10;

        if (GetTargetHPPercent() <= burnHp && gauge.Kazematoi > 0 && ActionLearned(AeolianEdge))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
            return;
        }

        switch (gauge.Kazematoi)
        {
            case 0 when ActionLearned(ArmorCrush):
                ReportUpcomingPositional(PositionalDirection.Flank, ArmorCrush, gcdsUntil);
                break;

            case >= 4 when ActionLearned(AeolianEdge):
                ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
                break;

            case var _ when ActionLearned(ArmorCrush) && ActionLearned(AeolianEdge):
                if (OnTargetsFlank() || !TargetNeedsPositionals())
                    ReportUpcomingPositional(PositionalDirection.Flank, ArmorCrush, gcdsUntil);
                else
                    ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
                break;

            case var _ when ActionLearned(AeolianEdge):
                ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
                break;

            default:
                ClearUpcomingPositional();
                break;
        }
    }

    private static bool TryReportNINActionPositional(uint action, int gcdsUntil)
    {
        switch (action)
        {
            case ArmorCrush:
                ReportUpcomingPositional(PositionalDirection.Flank, ArmorCrush, gcdsUntil);
                return true;

            case AeolianEdge:
                ReportUpcomingPositional(PositionalDirection.Rear, AeolianEdge, gcdsUntil);
                return true;

            default:
                return false;
        }
    }
}
