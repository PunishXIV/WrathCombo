using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal static MNKGauge Gauge = GetJobGauge<MNKGauge>();
    internal static MNKOpenerLogicSL MNKOpenerSL = new();
    internal static MNKOpenerLogicLL MNKOpenerLL = new();

    internal static float GCD => GetCooldown(OriginalHook(Bootshine)).CooldownTotal;

    internal static bool BothNadisOpen => Gauge.Nadi.ToString() == "Lunar, Solar";

    internal static bool SolarNadi => Gauge.Nadi == Nadi.Solar;

    internal static bool LunarNadi => Gauge.Nadi == Nadi.Lunar;

    internal static int OpoOpoChakra => Gauge.BeastChakra.Count(x => x == BeastChakra.OpoOpo);

    internal static int RaptorChakra => Gauge.BeastChakra.Count(x => x == BeastChakra.Raptor);

    internal static int CoeurlChakra => Gauge.BeastChakra.Count(x => x == BeastChakra.Coeurl);


    #region 1-2-3

    internal static uint DetermineCoreAbility(uint actionId, bool useTrueNorthIfEnabled)
    {
        if (HasStatusEffect(Buffs.OpoOpoForm) || HasStatusEffect(Buffs.FormlessFist))
            return Gauge.OpoOpoFury == 0 && LevelChecked(DragonKick)
                ? DragonKick
                : OriginalHook(Bootshine);

        if (HasStatusEffect(Buffs.RaptorForm))
            return Gauge.RaptorFury == 0 && LevelChecked(TwinSnakes)
                ? TwinSnakes
                : OriginalHook(TrueStrike);

        if (HasStatusEffect(Buffs.CoeurlForm))
        {
            if (Gauge.CoeurlFury == 0 && LevelChecked(Demolish))
            {
                if (!OnTargetsRear() &&
                    TargetNeedsPositionals() &&
                    !HasStatusEffect(Buffs.TrueNorth) &&
                    ActionReady(TrueNorth) &&
                    useTrueNorthIfEnabled)
                    return TrueNorth;

                return Demolish;
            }

            if (LevelChecked(SnapPunch))
            {
                if (!OnTargetsFlank() &&
                    TargetNeedsPositionals() &&
                    !HasStatusEffect(Buffs.TrueNorth) &&
                    ActionReady(TrueNorth) &&
                    useTrueNorthIfEnabled)
                    return TrueNorth;

                return OriginalHook(SnapPunch);
            }
        }

        return actionId;
    }

    #endregion

    #region Masterfull Blitz

    internal static bool InMasterfulRange()
    {
        if (NumberOfEnemiesInRange(ElixirField, null) >= 1 &&
            (OriginalHook(MasterfulBlitz) == ElixirField ||
             OriginalHook(MasterfulBlitz) == FlintStrike ||
             OriginalHook(MasterfulBlitz) == ElixirBurst ||
             OriginalHook(MasterfulBlitz) == RisingPhoenix))
            return true;

        if (NumberOfEnemiesInRange(TornadoKick, CurrentTarget) >= 1 &&
            (OriginalHook(MasterfulBlitz) == TornadoKick ||
             OriginalHook(MasterfulBlitz) == CelestialRevolution ||
             OriginalHook(MasterfulBlitz) == PhantomRush))
            return true;

        return false;
    }

    #endregion

    #region PB

    internal static bool UsePerfectBalanceST()
    {
        if (ActionReady(PerfectBalance) && !HasStatusEffect(Buffs.PerfectBalance) && !HasStatusEffect(Buffs.FormlessFist))
        {
            // Odd window
            if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                !JustUsed(PerfectBalance, 20) &&
                HasStatusEffect(Buffs.RiddleOfFire) && !HasStatusEffect(Buffs.Brotherhood))
                return true;

            // Even window
            if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                (GetCooldownRemainingTime(Brotherhood) <= GCD * 3 || HasStatusEffect(Buffs.Brotherhood)) &&
                (GetCooldownRemainingTime(RiddleOfFire) <= GCD * 3 || HasStatusEffect(Buffs.RiddleOfFire)))
                return true;

            // Low level
            if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                (HasStatusEffect(Buffs.RiddleOfFire) && !LevelChecked(Brotherhood) ||
                 !LevelChecked(RiddleOfFire)))
                return true;
        }

        return false;
    }

    internal static bool UsePerfectBalanceAoE()
    {
        if (ActionReady(PerfectBalance) && !HasStatusEffect(Buffs.PerfectBalance) && !HasStatusEffect(Buffs.FormlessFist))
        {
            //Initial/Failsafe
            if (GetRemainingCharges(PerfectBalance) == GetMaxCharges(PerfectBalance))
                return true;

            // Odd window
            if (HasStatusEffect(Buffs.RiddleOfFire) && !HasStatusEffect(Buffs.Brotherhood))
                return true;

            // Even window
            if ((GetCooldownRemainingTime(Brotherhood) <= GCD * 3 || HasStatusEffect(Buffs.Brotherhood)) &&
                (GetCooldownRemainingTime(RiddleOfFire) <= GCD * 3 || HasStatusEffect(Buffs.RiddleOfFire)))
                return true;

            // Low level
            if (HasStatusEffect(Buffs.RiddleOfFire) && !LevelChecked(Brotherhood) ||
                !LevelChecked(RiddleOfFire))
                return true;
        }

        return false;
    }

    #endregion

    #region PB Combo

    internal static bool DoPerfectBalanceComboST(ref uint actionID)
    {
        if (HasStatusEffect(Buffs.PerfectBalance))
        {
        #region Open Lunar

            if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
            {
                switch (Gauge.OpoOpoFury)
                {
                    case 0:
                        actionID = DragonKick;
                        return true;

                    case > 0:
                        actionID = OriginalHook(Bootshine);
                        return true;
                }
            }

        #endregion

        #region Open Solar

            if (!SolarNadi && !BothNadisOpen)
            {
                if (CoeurlChakra == 0)
                {
                    switch (Gauge.CoeurlFury)
                    {
                        case 0:
                            actionID = Demolish;
                            return true;

                        case > 0:
                            actionID = OriginalHook(SnapPunch);
                            return true;
                    }
                }

                if (RaptorChakra == 0)
                {
                    switch (Gauge.RaptorFury)
                    {
                        case 0:
                            actionID = TwinSnakes;
                            return true;

                        case > 0:
                            actionID = OriginalHook(TrueStrike);
                            return true;
                    }
                }

                if (OpoOpoChakra == 0)
                {
                    switch (Gauge.OpoOpoFury)
                    {
                        case 0:
                            actionID = DragonKick;
                            return true;

                        case > 0:
                            actionID = OriginalHook(Bootshine);
                            return true;
                    }
                }
            }

        #endregion
        }
        return false;
    }

    internal static bool DoPerfectBalanceComboAoE(ref uint actionID)
    {
        if (HasStatusEffect(Buffs.PerfectBalance))
        {
        #region Open Lunar

            if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
            {
                if (LevelChecked(ShadowOfTheDestroyer))
                {
                    actionID = ShadowOfTheDestroyer;
                    return true;
                }

                if (!LevelChecked(ShadowOfTheDestroyer))
                {
                    actionID = Rockbreaker;
                    return true;
                }
            }

        #endregion

        #region Open Solar

            if (!SolarNadi && !BothNadisOpen)
            {
                switch (GetStatusEffectStacks(Buffs.PerfectBalance))
                {
                    case 3:
                        actionID = OriginalHook(ArmOfTheDestroyer);
                        return true;

                    case 2:
                        actionID = FourPointFury;
                        return true;

                    case 1:
                        actionID = Rockbreaker;
                        return true;
                }
            }

        #endregion
        }
        return false;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Config.MNK_SelectedOpener == 0)
            return MNKOpenerLL;

        if (Config.MNK_SelectedOpener == 1)
            return MNKOpenerSL;

        return WrathOpener.Dummy;
    }

    internal class MNKOpenerLogicSL : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            DragonKick,
            PerfectBalance,
            TwinSnakes,
            Demolish,
            Brotherhood,
            RiddleOfFire,
            LeapingOpo,
            TheForbiddenChakra,
            RiddleOfWind,
            RisingPhoenix,
            DragonKick,
            WindsReply,
            FiresReply,
            LeapingOpo,
            PerfectBalance,
            DragonKick,
            LeapingOpo,
            DragonKick,
            ElixirBurst,
            LeapingOpo
        ];

        internal override UserData ContentCheckConfig => Config.MNK_Balance_Content;

        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            Gauge.Nadi is Nadi.None &&
            Gauge.RaptorFury is 0 &&
            Gauge.CoeurlFury is 0;
    }

    internal class MNKOpenerLogicLL : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            DragonKick,
            PerfectBalance,
            LeapingOpo,
            DragonKick,
            Brotherhood,
            RiddleOfFire,
            LeapingOpo,
            TheForbiddenChakra,
            RiddleOfWind,
            ElixirBurst,
            DragonKick,
            WindsReply,
            FiresReply,
            LeapingOpo,
            PerfectBalance,
            DragonKick,
            LeapingOpo,
            DragonKick,
            ElixirBurst,
            LeapingOpo
        ];

        internal override UserData ContentCheckConfig => Config.MNK_Balance_Content;

        public override bool HasCooldowns() =>
            GetRemainingCharges(PerfectBalance) is 2 &&
            IsOffCooldown(Brotherhood) &&
            IsOffCooldown(RiddleOfFire) &&
            IsOffCooldown(RiddleOfWind) &&
            Gauge.Nadi is Nadi.None &&
            Gauge.RaptorFury is 0 &&
            Gauge.CoeurlFury is 0;
    }

    #endregion

    #region ID's

    public const byte ClassID = 2;
    public const byte JobID = 20;

    public const uint
        Bootshine = 53,
        TrueStrike = 54,
        SnapPunch = 56,
        TwinSnakes = 61,
        ArmOfTheDestroyer = 62,
        Demolish = 66,
        DragonKick = 74,
        Rockbreaker = 70,
        Thunderclap = 25762,
        HowlingFist = 25763,
        FourPointFury = 16473,
        FormShift = 4262,
        SixSidedStar = 16476,
        ShadowOfTheDestroyer = 25767,
        LeapingOpo = 36945,
        RisingRaptor = 36946,
        PouncingCoeurl = 36947,
        TrueNorth = 7546,

        //Blitzes
        PerfectBalance = 69,
        MasterfulBlitz = 25764,
        ElixirField = 3545,
        ElixirBurst = 36948,
        FlintStrike = 25882,
        RisingPhoenix = 25768,
        CelestialRevolution = 25765,
        TornadoKick = 3543,
        PhantomRush = 25769,

        //Riddles + Buffs
        RiddleOfEarth = 7394,
        EarthsReply = 36944,
        RiddleOfFire = 7395,
        FiresReply = 36950,
        RiddleOfWind = 25766,
        WindsReply = 36949,
        Brotherhood = 7396,
        Mantra = 65,

        //Meditations
        InspiritedMeditation = 36941,
        SteeledMeditation = 36940,
        EnlightenedMeditation = 36943,
        ForbiddenMeditation = 36942,
        TheForbiddenChakra = 3547,
        Enlightenment = 16474,
        SteelPeak = 25761;

    internal static class Buffs
    {
        public const ushort
            TwinSnakes = 101,
            OpoOpoForm = 107,
            RaptorForm = 108,
            CoeurlForm = 109,
            PerfectBalance = 110,
            RiddleOfFire = 1181,
            RiddleOfWind = 2687,
            FormlessFist = 2513,
            TrueNorth = 1250,
            WindsRumination = 3842,
            FiresRumination = 3843,
            Brotherhood = 1185;
    }

    #endregion
}
