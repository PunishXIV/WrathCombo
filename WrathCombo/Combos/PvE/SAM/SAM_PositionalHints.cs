using WrathCombo.API.Enum;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    internal static void TickPositionalHints()
    {
        if (IsEnabled(Preset.SAM_ST_SimpleMode))
            ReportSAMPositionalHints(true, true);
        else if (IsEnabled(Preset.SAM_ST_AdvancedMode))
            ReportSAMPositionalHints(
                IsEnabled(Preset.SAM_ST_Adv_Gekko),
                IsEnabled(Preset.SAM_ST_Adv_Kasha));
    }

    private static void ReportSAMPositionalHints(bool useGekko, bool useKasha)
    {
        if (!CanReportPositionalHints())
            return;

        if (TryReportOpenerPositionalHint(Opener(), TryReportSAMActionPositional))
            return;

        if (LocalPlayer.HasStatus(Buffs.MeikyoShisui))
        {
            ReportSAMMeikyoHints(useGekko, useKasha);
            return;
        }

        switch (ComboAction)
        {
            case Jinpu when useGekko && ActionLearned(Gekko):
                ReportUpcomingPositional(PositionalDirection.Rear, Gekko, 1);
                break;

            case Shifu when useKasha && ActionLearned(Kasha):
                ReportUpcomingPositional(PositionalDirection.Flank, Kasha, 1);
                break;

            case Hakaze or Gyofu:
                ReportSAMFinisherPath(useGekko, useKasha, 2);
                break;

            default:
                ReportSAMFinisherPath(useGekko, useKasha, 3);
                break;
        }
    }

    private static void ReportSAMMeikyoHints(bool useGekko, bool useKasha)
    {
        if (useGekko && ActionLearned(Gekko) && (!HasGetsu || !LocalPlayer.HasStatus(Buffs.Fugetsu)))
            ReportUpcomingPositional(PositionalDirection.Rear, Gekko, 1);
        else if (useKasha && ActionLearned(Kasha) && (!HasKa || !LocalPlayer.HasStatus(Buffs.Fuka)))
            ReportUpcomingPositional(PositionalDirection.Flank, Kasha, 1);
        else if (SenCount is 3)
        {
            // Burst Meikyo: Iaijutsu this GCD, then Gekko/Kasha. Publish that positional now.
            if (useGekko && ActionLearned(Gekko))
                ReportUpcomingPositional(PositionalDirection.Rear, Gekko, 2);
            else if (useKasha && ActionLearned(Kasha))
                ReportUpcomingPositional(PositionalDirection.Flank, Kasha, 2);
            else
                ClearUpcomingPositional();
        }
        else
            ClearUpcomingPositional();
    }

    // Same order as DoBasicCombo Hakaze branch: Yukikaze → Kasha → Gekko.
    private static void ReportSAMFinisherPath(bool useGekko, bool useKasha, int gcdsUntil)
    {
        float fugetsuRemaining = LocalPlayer.Status(Buffs.Fugetsu).RemainingTimeOrZero();
        float fukaRemaining = LocalPlayer.Status(Buffs.Fuka).RemainingTimeOrZero();
        bool refreshFugetsu = fugetsuRemaining <= fukaRemaining;
        bool refreshFuka = fukaRemaining <= fugetsuRemaining;

        if (ActionLearned(Yukikaze) && !HasSetsu &&
            (!useGekko || !ActionLearned(Gekko) || fugetsuRemaining > 7) &&
            (!useKasha || !ActionLearned(Kasha) || fukaRemaining > 7))
        {
            ClearUpcomingPositional();
            return;
        }

        if (useKasha &&
            ActionLearned(Shifu) &&
            ((OnTargetsFlank() || OnTargetsFront()) && !HasKa && ActionLearned(Kasha) ||
             OnTargetsRear() && HasGetsu && ActionLearned(Kasha) ||
             !LocalPlayer.HasStatus(Buffs.Fuka) ||
             SenCount is 3 && refreshFuka ||
             !ActionLearned(Gekko)))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, Kasha, gcdsUntil);
            return;
        }

        if (useGekko &&
            ActionLearned(Jinpu) &&
            (!ActionLearned(Kasha) && ActionLearned(Gekko) ||
             (OnTargetsRear() || OnTargetsFront()) && !HasGetsu && ActionLearned(Gekko) ||
             OnTargetsFlank() && HasKa && ActionLearned(Gekko) ||
             !LocalPlayer.HasStatus(Buffs.Fugetsu) ||
             SenCount is 3 && refreshFugetsu))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, Gekko, gcdsUntil);
            return;
        }

        ClearUpcomingPositional();
    }

    private static bool TryReportSAMActionPositional(uint action, int gcdsUntil)
    {
        switch (action)
        {
            case Gekko:
                ReportUpcomingPositional(PositionalDirection.Rear, Gekko, gcdsUntil);
                return true;

            case Kasha:
                ReportUpcomingPositional(PositionalDirection.Flank, Kasha, gcdsUntil);
                return true;

            default:
                return false;
        }
    }
}
