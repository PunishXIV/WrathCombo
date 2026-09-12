using WrathCombo.API.Enum;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.RPR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    internal static void TickPositionalHints()
    {
        if (IsEnabled(Preset.RPR_ST_SimpleMode) || IsEnabled(Preset.RPR_ST_AdvancedMode))
            ReportRPRPositionalHints();
    }

    private static void ReportRPRPositionalHints()
    {
        if (!CanReportPositionalHints())
            return;

        if (LocalPlayer.HasStatus(Buffs.Enshrouded))
        {
            ClearUpcomingPositional();
            return;
        }

        if (!LocalPlayer.HasStatus(Buffs.SoulReaver) && !LocalPlayer.HasStatus(Buffs.Executioner))
            return;

        if (!ActionLearned(Gibbet))
            return;

        switch (LocalPlayer.HasStatus(Buffs.EnhancedGibbet), LocalPlayer.HasStatus(Buffs.EnhancedGallows))
        {
            case (true, _):
                ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(Gibbet), 1);
                break;

            case (_, true):
                ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(Gallows), 1);
                break;

            // Simple / Advanced Rear First → Gallows; Advanced Flank First → Gibbet
            case (false, false) when IsEnabled(Preset.RPR_ST_AdvancedMode) && RPR_Positional == 1:
                ReportUpcomingPositional(PositionalDirection.Flank, OriginalHook(Gibbet), 1);
                break;

            default:
                ReportUpcomingPositional(PositionalDirection.Rear, OriginalHook(Gallows), 1);
                break;
        }
    }
}
