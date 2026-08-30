using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    private static void ReportRPRPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        // Enshroud replaces Gibbet/Gallows — retract like MNK PB.
        if (LocalPlayer.HasStatus(Buffs.Enshrouded))
        {
            ClearUpcomingPositional();
            return;
        }

        if (LocalPlayer.HasStatus(Buffs.EnhancedGibbet))
            ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(Gibbet), 1);
        else if (LocalPlayer.HasStatus(Buffs.EnhancedGallows))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(Gallows), 1);
        else if ((LocalPlayer.HasStatus(Buffs.SoulReaver) || LocalPlayer.HasStatus(Buffs.Executioner)) &&
                 ActionLearned(Gibbet))
            ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(Gallows), 1);
        // Filler without reaver: leave last hint so heartbeat does not get Reset every tick
    }
}
