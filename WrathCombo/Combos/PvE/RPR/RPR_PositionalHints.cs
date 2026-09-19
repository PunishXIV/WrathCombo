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

        if (LocalPlayer.HasStatus(Buffs.Enshrouded, out var _, false))
        {
            ClearUpcomingPositional();
            return;
        }

        if (!LocalPlayer.HasStatus(Buffs.SoulReaver, out var _, false) && !LocalPlayer.HasStatus(Buffs.Executioner, out var _, false))
            return;

        if (!ActionLearned(Gibbet))
            return;

        switch (LocalPlayer.HasStatus(Buffs.EnhancedGibbet, out var _, false), LocalPlayer.HasStatus(Buffs.EnhancedGallows, out var _, false))
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

