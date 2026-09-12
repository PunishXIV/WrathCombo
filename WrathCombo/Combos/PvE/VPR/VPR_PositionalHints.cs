using WrathCombo.API.Enum;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    internal static void TickPositionalHints()
    {
        if (IsEnabled(Preset.VPR_ST_SimpleMode))
            ReportVPRPositionalHints(vicewinderBuffPrio: false);
        else if (IsEnabled(Preset.VPR_ST_AdvancedMode))
            ReportVPRPositionalHints(Config.VPR_VicewinderBuffPrio);
    }

    private static void ReportVPRPositionalHints(bool vicewinderBuffPrio)
    {
        if (!CanReportPositionalHints())
            return;

        if (LocalPlayer.HasStatus(Buffs.Reawakened))
        {
            ClearUpcomingPositional();
            return;
        }

        if (TryReportOpenerPositionalHint(Opener(), TryReportVPRActionPositional))
            return;

        switch (ComboAction)
        {
            case HuntersSting or SwiftskinsSting:
                if (!TryReportVPRStingFinisher(1))
                    ClearUpcomingPositional();
                return;

            case ReavingFangs or SteelFangs:
                if (!TryReportVicewinderCoilPositionalHints(vicewinderBuffPrio))
                    ReportVPRFinisherPath(2);
                return;

            default:
                if (!TryReportVicewinderCoilPositionalHints(vicewinderBuffPrio))
                    ReportVPRFinisherPath(3);
                return;
        }
    }

    private static bool TryReportVPRStingFinisher(int gcdsUntil)
    {
        if (LocalPlayer.HasStatus(Buffs.HindsbaneVenom) && ActionLearned(HindsbaneFang))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, HindsbaneFang, gcdsUntil);
            return true;
        }

        if (LocalPlayer.HasStatus(Buffs.FlanksbaneVenom) && ActionLearned(FlanksbaneFang))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, FlanksbaneFang, gcdsUntil);
            return true;
        }

        if (LocalPlayer.HasStatus(Buffs.HindstungVenom) && ActionLearned(HindstingStrike))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, HindstingStrike, gcdsUntil);
            return true;
        }

        if (LocalPlayer.HasStatus(Buffs.FlankstungVenom) && ActionLearned(FlankstingStrike))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, FlankstingStrike, gcdsUntil);
            return true;
        }

        return false;
    }

    private static void ReportVPRFinisherPath(int gcdsUntil)
    {
        if (ActionLearned(SwiftskinsSting) &&
            (HasHindVenom || IsMissingSwiftscaled || IsMissingBasicComboVenom))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, UpcomingHindFinisher(), gcdsUntil);
            return;
        }

        if (ActionLearned(HuntersSting) &&
            (HasFlankVenom || IsMissingHuntersInstinct))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, UpcomingFlankFinisher(), gcdsUntil);
            return;
        }

        ClearUpcomingPositional();
    }

    private static bool TryReportVicewinderCoilPositionalHints(bool vicewinderBuffPrio)
    {
        if (!ActionLearned(Vicewinder) || LocalPlayer.HasStatus(Buffs.Reawakened))
            return false;

        bool vicewinderInRotation = !IsEnabled(Preset.VPR_ST_AdvancedMode) ||
                                    IsEnabled(Preset.VPR_ST_Vicewinder);

        if (TryGetNextVicewinderCoil(vicewinderBuffPrio, out uint coil))
        {
            ReportVicewinderCoil(coil, 1);
            return true;
        }

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
        switch (coil)
        {
            case SwiftskinsCoil:
                ReportUpcomingPositional(PositionalDirection.Rear, SwiftskinsCoil, gcdsUntil);
                break;

            case HuntersCoil:
                ReportUpcomingPositional(PositionalDirection.Flank, HuntersCoil, gcdsUntil);
                break;
        }
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
