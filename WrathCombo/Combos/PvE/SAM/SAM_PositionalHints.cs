using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class SAM
{
    private static void ReportSAMPositionalHints(bool useGekko, bool useKasha)
    {
        if (!CanReportPositionalHints())
            return;

        if (LocalPlayer.HasStatus(Buffs.MeikyoShisui))
        {
            if (useGekko && ActionLearned(Gekko) && !HasGetsu || !LocalPlayer.HasStatus(Buffs.Fugetsu))
                ReportUpcomingPositional(PositionalDirection.Rear, Gekko, 1);
            else if (useKasha && ActionLearned(Kasha) && !HasKa || !LocalPlayer.HasStatus(Buffs.Fuka))
                ReportUpcomingPositional(PositionalDirection.Flank, Kasha, 1);
            else
                ClearUpcomingPositional();
            return;
        }

        if (ComboAction is Jinpu && ActionLearned(Gekko))
            ReportUpcomingPositional(PositionalDirection.Rear, Gekko, 1);
        else if (ComboAction is Shifu && ActionLearned(Kasha))
            ReportUpcomingPositional(PositionalDirection.Flank, Kasha, 1);
        else if (ComboAction is Hakaze or Gyofu)
            TryReportSAMFinisherPath(useGekko, useKasha, 2);
        else
            // Hakaze → Jinpu/Shifu → Gekko/Kasha (combo start / after finisher)
            TryReportSAMFinisherPath(useGekko, useKasha, 3);
    }

    /// <summary>
    ///     Same path choice as the Hakaze/Gyofu branch of the ST combo.
    /// </summary>
    private static bool TryReportSAMFinisherPath(bool useGekko, bool useKasha, int gcdsUntil)
    {
        if (useGekko &&
            ActionLearned(Jinpu) &&
            (!ActionLearned(Kasha) && ActionLearned(Gekko) ||
             (OnTargetsRear() || OnTargetsFront()) && !HasGetsu && ActionLearned(Gekko) ||
             OnTargetsFlank() && HasKa && ActionLearned(Gekko) ||
             !LocalPlayer.HasStatus(Buffs.Fugetsu)))
        {
            ReportUpcomingPositional(PositionalDirection.Rear, Gekko, gcdsUntil);
            return true;
        }

        if (useKasha &&
            ActionLearned(Shifu) &&
            ((OnTargetsFlank() || OnTargetsFront()) && !HasKa && ActionLearned(Kasha) ||
             OnTargetsRear() && HasGetsu && ActionLearned(Kasha) ||
             !LocalPlayer.HasStatus(Buffs.Fuka)))
        {
            ReportUpcomingPositional(PositionalDirection.Flank, Kasha, gcdsUntil);
            return true;
        }

        return false;
    }
}
