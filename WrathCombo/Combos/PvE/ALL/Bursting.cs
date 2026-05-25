#region

using System.Linq;
using ECommons.ExcelServices;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Combos.PvE;

public class Bursting
{
    #region Burst Phase

    public static bool InGoodBurstPhase
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_GoodPhase", TS.FromSeconds(1)))
                return field;

            var threshold = ContentCheck.IsInBottomHalfContent()
                ? BurstJobsInParty * 0.3
                : BurstJobsInParty * 0.6;
            var anyDeathDebuffs = GetPartyMembers().Any(member =>
                HasStatusEffect(43, member.GameObject, true) || // weakness
                HasStatusEffect(44, member.GameObject, true));  // brink

            field = NumberOfPartyMembersBursting > threshold &&
                    !anyDeathDebuffs;
            return field;
        }
    }

    public static bool InBurstPhase
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_Phase", TS.FromSeconds(1)))
                return field;

            var threshold = ContentCheck.IsInBottomHalfContent()
                ? BurstJobsInParty * 0.25
                : BurstJobsInParty * 0.4;

            field = NumberOfPartyMembersBursting > threshold;
            return field;
        }
    }

    #endregion

    #region Self Checks

    /// <summary>
    ///     Similar to <see cref="PlayerIsBursting" /> but for any damage buff
    ///     at all, e.g. <see cref="PLD.FightOrFlight" />.
    /// </summary>
    public static bool PlayerIsDamageBuffed
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_Self_Any", TS.FromSeconds(2.5)))
                return field;

            field = PartyIsBursting ||
                    DRK.Gauge.DarksideTimeRemaining > 0 ||
                    HasBuff.Self(PLD.Buffs.FightOrFlight) ||
                    HasBuff.Self(WAR.Buffs.SurgingTempest) ||
                    HasBuff.Self(WAR.Buffs.InnerReleaseBuff) ||
                    HasBuff.Self(WAR.Buffs.Berserk) ||
                    HasBuff.Self(GNB.Buffs.NoMercy) ||
                    HasBuff.Self(AST.Buffs.BalanceBuff, anyOwner: true) ||
                    HasBuff.Self(AST.Buffs.SpearBuff, anyOwner: true) ||
                    HasBuff.Self(DRG.Buffs.LanceCharge) ||
                    HasBuff.Target(NIN.Debuffs.TrickAttack) ||
                    HasBuff.Target(NIN.Debuffs.KunaisBane) ||
                    HasBuff.Self(BRD.Buffs.RagingStrikes) ||
                    HasBuff.Self(BRD.Buffs.RadiantFinale) ||
                    HasBuff.Self(BRD.Buffs.WanderersMinuet) ||
                    HasBuff.Self(BRD.Buffs.MagesBallad) ||
                    HasBuff.Self(DNC.Buffs.Devilment) ||
                    HasBuff.Self(BLM.Buffs.LeyLines);

            return field;
        }
    }

    public static bool PlayerIsBursting
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_Self", TS.FromSeconds(1)))
                return field;

            field = Buffs.Self.Any(buff =>
                        HasBuff.Self(buff, anyOwner: false)) ||
                    Buffs.Target.Any(buff =>
                        HasBuff.Target(buff, anyOwner: false)) ||
                    GetCooldownRemainingTime(DRK.LivingShadow) > 98;

            return field;
        }
    }

    #endregion

    #region Party Checks

    public static bool PartyIsBursting
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_Others", TS.FromSeconds(1.5)))
                return field;

            field = Buffs.Self.Any(buff =>
                        HasBuff.Self(buff, anyOwner: true)) ||
                    Buffs.Target.Any(buff =>
                        HasBuff.Target(buff, anyOwner: true));

            return field;
        }
    }

    public static int NumberOfPartyMembersBursting
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_Others_Amount", TS.FromSeconds(2)))
                return field;

            var count = Buffs.Self.Count(buff =>
                            HasBuff.Self(buff, anyOwner: true)) +
                        Buffs.Target.Count(buff =>
                            HasBuff.Target(buff, anyOwner: true)) +
                        (GetCooldownRemainingTime(DRK.LivingShadow) > 98
                            ? 1
                            : 0);

            field = count;
            return field;
        }
    }

    #endregion

    #region Helpers

    private static class Buffs
    {
        internal static readonly Job[] Jobs =
        [
            Job.AST,
            Job.BRD,
            Job.DNC,
            Job.DRG,
            Job.MNK,
            Job.NIN,
            Job.PCT,
            Job.RDM,
            Job.RPR,
            Job.SCH,
            Job.SMN,
        ];

        internal static readonly ushort[] Self =
        [
            AST.Buffs.Divination,
            BRD.Buffs.BattleVoice,
            DNC.Buffs.TechnicalFinish,
            DRG.Buffs.BattleLitany,
            MNK.Buffs.RiddleOfFire,
            MNK.Buffs.Brotherhood,
            PCT.Buffs.StarryMuse,
            RDM.Buffs.Embolden,
            RDM.Buffs.EmboldenOthers,
            RPR.Buffs.ArcaneCircle,
            SMN.Buffs.SearingLight,
        ];

        internal static readonly ushort[] Target =
        [
            NIN.Debuffs.Mug,
            NIN.Debuffs.Dokumori,
            SCH.Debuffs.ChainStratagem,
        ];
    }

    private class HasBuff
    {
        protected internal static bool Target
            (ushort buff, bool anyOwner = false) =>
            GetPossessedStatusRemainingTime(buff, CurrentTarget,
                anyOwner: anyOwner) > 0;

        protected internal static bool Self
            (ushort buff, bool anyOwner = false) =>
            GetPossessedStatusRemainingTime(buff,
                anyOwner: anyOwner) > 0;
    }

    private static int BurstJobsInParty
    {
        get
        {
            if (!EZ.Throttle("BurstCheck_BurstMembers", TS.FromSeconds(10)))
                return field;

            var count = GetPartyMembers().Count(member =>
                Buffs.Jobs.Contains((Job) (member.RealJob?.RowId ?? 0)));

            field = count;
            return field;
        }
    }

    #endregion
}
