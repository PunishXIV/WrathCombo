using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    private static void ReportMNKPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        // PB / Formless replace the form loop — retract so heartbeat cannot keep a stale gcds=2/3 alive.
        if (!ActionLearned(TrueStrike) ||
            LocalPlayer.HasStatus(Buffs.PerfectBalance) ||
            LocalPlayer.HasStatus(Buffs.FormlessFist))
        {
            ClearUpcomingPositional();
            return;
        }

        if (TryReportOpenerPositionalHint(Opener(), TryReportMNKActionPositional))
            return;

        // After a Coeurl GCD, form/stack status can lag one tick and look like Demolish gcds=1.
        var justUsedCoeurlPositional =
            JustUsed(Demolish, GCD) || JustUsed(OriginalHook(SnapPunch), GCD);

        if (LocalPlayer.HasStatus(Buffs.CoeurlForm) && !justUsedCoeurlPositional)
        {
            if (CoeurlStacks is 0 && ActionLearned(Demolish))
                ReportUpcomingPositional(PositionalDirection.Rear, Demolish, 1);
            else if (ActionLearned(SnapPunch))
                ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(SnapPunch), 1);
        }
        else if (LocalPlayer.HasStatus(Buffs.RaptorForm) && ActionLearned(TrueStrike))
        {
            if (CoeurlStacks is 0 && ActionLearned(Demolish))
                ReportUpcomingPositional(PositionalDirection.Rear, Demolish, 2);
            else if (ActionLearned(SnapPunch))
                ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(SnapPunch), 2);
        }
        else if (LocalPlayer.HasStatus(Buffs.OpoOpoForm) || justUsedCoeurlPositional)
        {
            // Opo → Raptor → Coeurl positional
            if (CoeurlStacks is 0 && ActionLearned(Demolish))
                ReportUpcomingPositional(PositionalDirection.Rear, Demolish, 3);
            else if (ActionLearned(SnapPunch))
                ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(SnapPunch), 3);
        }
        // Form buff gaps / unknown: leave the last published hint for its TTL
    }

    private static bool TryReportMNKActionPositional(uint action, int gcdsUntil)
    {
        if (action == Demolish)
        {
            ReportUpcomingPositional(PositionalDirection.Rear, Demolish, gcdsUntil);
            return true;
        }

        if (action == OriginalHook(SnapPunch))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(SnapPunch), gcdsUntil);
            return true;
        }

        return false;
    }
}
