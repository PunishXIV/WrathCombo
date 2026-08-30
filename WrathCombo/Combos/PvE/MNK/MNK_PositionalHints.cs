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

        // Perfect Balance / Formless replace the normal form loop.
        if (!ActionLearned(TrueStrike) ||
            LocalPlayer.HasStatus(Buffs.PerfectBalance) ||
            LocalPlayer.HasStatus(Buffs.FormlessFist))
        {
            ClearUpcomingPositional();
            return;
        }

        if (TryReportOpenerPositionalHint(Opener(), TryReportMNKActionPositional))
            return;

        // Form/stack status can lag one tick after a Coeurl GCD.
        bool justUsedCoeurlPositional =
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
            if (CoeurlStacks is 0 && ActionLearned(Demolish))
                ReportUpcomingPositional(PositionalDirection.Rear, Demolish, 3);
            else if (ActionLearned(SnapPunch))
                ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(SnapPunch), 3);
        }
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
