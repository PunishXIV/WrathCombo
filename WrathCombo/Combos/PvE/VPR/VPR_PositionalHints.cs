using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    private static void ReportVPRPositionalHints()
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

        if (TryReportVicewinderCoilPositionalHints())
            return;

        if (ComboTimer <= 0)
        {
            ClearUpcomingPositional();
            return;
        }

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
            else
                ClearUpcomingPositional();
        }
        else if (ComboAction is ReavingFangs or SteelFangs)
        {
            if (ActionLearned(SwiftskinsSting) &&
                (HasHindVenom || IsMissingSwiftscaled || IsMissingBasicComboVenom))
                ReportUpcomingPositional(PositionalDirection.Rear, UpcomingHindFinisher(), 2);
            else if (ActionLearned(HuntersSting) &&
                     (HasFlankVenom || IsMissingHuntersInstinct))
                ReportUpcomingPositional(PositionalDirection.Flank, UpcomingFlankFinisher(), 2);
            else
                ClearUpcomingPositional();
        }
        else
            ClearUpcomingPositional();
    }

    private static bool TryReportVicewinderCoilPositionalHints()
    {
        if (!ActionLearned(Vicewinder) ||
            !(UsedVicewinder || UsedHuntersCoil || UsedSwiftskinsCoil))
            return false;

        if (UsedHuntersCoil)
        {
            ReportUpcomingPositional(PositionalDirection.Rear, SwiftskinsCoil, 1);
            return true;
        }

        if (UsedSwiftskinsCoil)
        {
            ReportUpcomingPositional(PositionalDirection.Flank, HuntersCoil, 1);
            return true;
        }

        if (!OnTargetsFlank() || !TargetNeedsPositionals())
            ReportUpcomingPositional(PositionalDirection.Rear, SwiftskinsCoil, 1);
        else if (!OnTargetsRear() || !TargetNeedsPositionals())
            ReportUpcomingPositional(PositionalDirection.Flank, HuntersCoil, 1);
        else
            ClearUpcomingPositional();

        return true;
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
