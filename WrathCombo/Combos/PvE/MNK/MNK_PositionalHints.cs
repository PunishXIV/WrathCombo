using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.API.Enum;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal static void TickPositionalHints()
    {
        if (IsEnabled(Preset.MNK_ST_SimpleMode) || IsEnabled(Preset.MNK_ST_AdvancedMode))
            ReportMNKPositionalHints();
    }

    private static void ReportMNKPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        if (!ActionLearned(TrueStrike) || LocalPlayer.HasStatus(Buffs.FormlessFist))
        {
            ClearUpcomingPositional();
            return;
        }

        if (TryReportOpenerPositionalHint(Opener(), TryReportMNKActionPositional))
            return;

        if (LocalPlayer.HasStatus(Buffs.PerfectBalance))
        {
            if (!SolarNadi && LunarNadi && Gauge.BeastChakra[0] is BeastChakra.None)
                ReportCoeurlPositional(1);
            else
                ClearUpcomingPositional();
            return;
        }

        bool justUsedCoeurlPositional =
            JustUsed(Demolish, GCD) || JustUsed(OriginalHook(SnapPunch), GCD);

        int gcdsUntil = (LocalPlayer.HasStatus(Buffs.CoeurlForm) && !justUsedCoeurlPositional) switch
        {
            true => 1,
            _ when LocalPlayer.HasStatus(Buffs.RaptorForm) && ActionLearned(TrueStrike) => 2,
            _ when LocalPlayer.HasStatus(Buffs.OpoOpoForm) || justUsedCoeurlPositional => 3,
            _ => 0,
        };

        if (gcdsUntil is 0)
            ClearUpcomingPositional();
        else
            ReportCoeurlPositional(gcdsUntil);
    }

    private static void ReportCoeurlPositional(int gcdsUntil)
    {
        if (CoeurlStacks is 0 && ActionLearned(Demolish))
            ReportUpcomingPositional(PositionalDirection.Rear, Demolish, gcdsUntil);
        else if (ActionLearned(SnapPunch))
            ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(SnapPunch), gcdsUntil);
        else
            ClearUpcomingPositional();
    }

    private static bool TryReportMNKActionPositional(uint action, int gcdsUntil)
    {
        uint snapPunch = OriginalHook(SnapPunch);

        switch (action)
        {
            case Demolish:
                ReportUpcomingPositional(PositionalDirection.Rear, Demolish, gcdsUntil);
                return true;

            case var _ when action == snapPunch:
                ReportUpcomingPositional(PositionalDirection.Flank, snapPunch, gcdsUntil);
                return true;

            default:
                return false;
        }
    }
}
