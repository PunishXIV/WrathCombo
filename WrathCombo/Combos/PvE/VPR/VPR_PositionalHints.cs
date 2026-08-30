using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    private static void ReportVPRPositionalHints(bool vicewinderBuffPrio)
    {
        if (!CanReportPositionalHints())
            return;

        // Reawaken replaces the sting/coil loop — retract like MNK PB/Formless.
        if (LocalPlayer.HasStatus(Buffs.Reawakened))
        {
            ClearUpcomingPositional();
            return;
        }

        if (TryReportOpenerPositionalHint(Opener(), TryReportVPRActionPositional))
            return;

        if (TryReportVicewinderCoilPositionalHints(vicewinderBuffPrio))
            return;

        if (ComboAction is HuntersSting or SwiftskinsSting)
        {
            if (LocalPlayer.HasStatus(Buffs.HindsbaneVenom) && ActionLearned(HindsbaneFang))
                ReportUpcomingPositional(PositionalDirection.Rear, HindsbaneFang, 1);
            else if (LocalPlayer.HasStatus(Buffs.FlanksbaneVenom) && ActionLearned(FlanksbaneFang))
                ReportUpcomingPositional(PositionalDirection.Flank, FlanksbaneFang, 1);
            else if (LocalPlayer.HasStatus(Buffs.HindstungVenom) && ActionLearned(HindstingStrike))
                ReportUpcomingPositional(PositionalDirection.Rear, HindstingStrike, 1);
            else if (LocalPlayer.HasStatus(Buffs.FlankstungVenom) && ActionLearned(FlankstingStrike))
                ReportUpcomingPositional(PositionalDirection.Flank, FlankstingStrike, 1);
        }
        else if (ComboAction is ReavingFangs or SteelFangs)
            TryReportVPRFinisherPath(2);
        else
            // Fangs → Sting → Finisher (combo start / after finisher)
            TryReportVPRFinisherPath(3);
    }

    private static bool TryReportVPRFinisherPath(int gcdsUntil)
    {
        if (ActionLearned(SwiftskinsSting) &&
            (HasHindVenom || IsMissingSwiftscaled || IsMissingBasicComboVenom))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, UpcomingHindFinisher(), gcdsUntil);
            return true;
        }

        if (ActionLearned(HuntersSting) &&
            (HasFlankVenom || IsMissingHuntersInstinct))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, UpcomingFlankFinisher(), gcdsUntil);
            return true;
        }

        return false;
    }

    private static bool TryReportVicewinderCoilPositionalHints(bool vicewinderBuffPrio)
    {
        if (!ActionLearned(Vicewinder) || LocalPlayer.HasStatus(Buffs.Reawakened))
            return false;

        // Advanced: only when Vicewinder features are on (Simple always uses them).
        var vicewinderInRotation = !IsEnabled(Preset.VPR_ST_AdvancedMode) ||
                                   IsEnabled(Preset.VPR_ST_Vicewinder) ||
                                   IsEnabled(Preset.VPR_ST_VicewinderCombo);

        if (TryGetNextVicewinderCoil(vicewinderBuffPrio, out var coil))
        {
            ReportVicewinderCoil(coil, 1);
            return true;
        }

        // About to press Vicewinder: same first-coil choice the rotation will make after VW.
        if (vicewinderInRotation && UseVicewinder() &&
            TryGetFirstVicewinderCoil(vicewinderBuffPrio, out coil))
        {
            ReportVicewinderCoil(coil, 2);
            return true;
        }

        return false;
    }

    private static void ReportVicewinderCoil(uint coil, int gcdsUntil)
    {
        if (coil == SwiftskinsCoil)
            ReportUpcomingPositional(PositionalDirection.Rear, SwiftskinsCoil, gcdsUntil);
        else if (coil == HuntersCoil)
            ReportUpcomingPositional(PositionalDirection.Flank, HuntersCoil, gcdsUntil);
    }

    private static bool TryReportVPRActionPositional(uint action, int gcdsUntil)
    {
        switch (action)
        {
            case HuntersCoil:
                ReportUpcomingPositional(PositionalDirection.Flank, HuntersCoil, gcdsUntil);
                return true;
            case SwiftskinsCoil:
                ReportUpcomingPositional(PositionalDirection.Rear, SwiftskinsCoil, gcdsUntil);
                return true;
            case HindstingStrike:
            case HindsbaneFang:
                ReportUpcomingPositional(PositionalDirection.Rear, action, gcdsUntil);
                return true;
            case FlankstingStrike:
            case FlanksbaneFang:
                ReportUpcomingPositional(PositionalDirection.Flank, action, gcdsUntil);
                return true;
            default:
                return false;
        }
    }

    private static uint UpcomingHindFinisher() =>
        LocalPlayer.HasStatus(Buffs.HindsbaneVenom) && ActionLearned(HindsbaneFang)
            ? HindsbaneFang
            : HindstingStrike;

    private static uint UpcomingFlankFinisher() =>
        LocalPlayer.HasStatus(Buffs.FlanksbaneVenom) && ActionLearned(FlanksbaneFang)
            ? FlanksbaneFang
            : FlankstingStrike;
}
