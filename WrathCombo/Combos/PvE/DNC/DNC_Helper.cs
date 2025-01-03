#region

using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Options = WrathCombo.Combos.CustomComboPreset;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DNC
{
    #region IDs

    public const byte JobID = 38;

    #region Actions

    public const uint
        // Single Target
        Cascade = 15989,
        Fountain = 15990,
        ReverseCascade = 15991,
        Fountainfall = 15992,
        StarfallDance = 25792,
        // AoE
        Windmill = 15993,
        Bladeshower = 15994,
        RisingWindmill = 15995,
        Bloodshower = 15996,
        Tillana = 25790,
        // Dancing
        StandardStep = 15997,
        TechnicalStep = 15998,
        StandardFinish0 = 16003,
        StandardFinish1 = 16191,
        StandardFinish2 = 16192,
        TechnicalFinish0 = 16004,
        TechnicalFinish1 = 16193,
        TechnicalFinish2 = 16194,
        TechnicalFinish3 = 16195,
        TechnicalFinish4 = 16196,
        Emboite = 15999,
        Entrechat = 16000,
        Jete = 16001,
        Pirouette = 16002,
        // Fan Dances
        FanDance1 = 16007,
        FanDance2 = 16008,
        FanDance3 = 16009,
        FanDance4 = 25791,
        // Other
        Peloton = 7557,
        SaberDance = 16005,
        ClosedPosition = 16006,
        EnAvant = 16010,
        Devilment = 16011,
        ShieldSamba = 16012,
        Flourish = 16013,
        Improvisation = 16014,
        CuringWaltz = 16015,
        LastDance = 36983,
        FinishingMove = 36984,
        DanceOfTheDawn = 36985;

    #endregion

    public static class Buffs
    {
        public const ushort
            // Flourishing & Silken (procs)
            FlourishingCascade = 1814,
            FlourishingFountain = 1815,
            FlourishingWindmill = 1816,
            FlourishingShower = 1817,
            FlourishingFanDance = 2021,
            SilkenSymmetry = 2693,
            SilkenFlow = 2694,
            FlourishingFinish = 2698,
            FlourishingStarfall = 2700,
            FlourishingSymmetry = 3017,
            FlourishingFlow = 3018,
            // Dances
            StandardStep = 1818,
            TechnicalStep = 1819,
            StandardFinish = 1821,
            TechnicalFinish = 1822,
            // Fan Dances
            ThreeFoldFanDance = 1820,
            FourFoldFanDance = 2699,
            // Other
            Peloton = 1199,
            ClosedPosition = 1823,
            ShieldSamba = 1826,
            LastDanceReady = 3867,
            FinishingMoveReady = 3868,
            DanceOfTheDawnReady = 3869,
            Devilment = 1825;
    }

    #endregion

    /// <summary>
    ///     Logic to pick different openers.
    /// </summary>
    /// <returns>The chosen Opener.</returns>
    internal static WrathOpener Opener()
    {
        if (Config.DNC_ST_OpenerSelection == (int) Config.Openers.FifteenSecond &&
            Opener15S.LevelChecked)
            return Opener15S;

        if (Config.DNC_ST_OpenerSelection == (int) Config.Openers.SevenSecond &&
            Opener07S.LevelChecked)
            return Opener07S;

        return WrathOpener.Dummy;
    }

    /// <summary>
    ///     Dancer Gauge data, just consolidated.
    /// </summary>
    private static DNCGauge Gauge => GetJobGauge<DNCGauge>();

    /// <summary>
    ///     Check if the rotation is in Auto-Rotation.
    /// </summary>
    /// <param name="singleTarget">
    ///     <c>true</c> if checking Single-Target combos.<br />
    ///     <c>false</c> if checking AoE combos.
    /// </param>
    /// <param name="simpleMode">
    ///     <c>true</c> if checking Simple Mode.<br />
    ///     <c>false</c> if checking Advanced Mode.
    /// </param>
    /// <returns>
    ///     Whether the Combo is in Auto-Mode and Auto-Rotation is enabled
    ///     (whether by user settings or another plugin).
    /// </returns>
    private static bool InAutoMode(bool singleTarget, bool simpleMode) =>
        P.IPC.GetAutoRotationState() && P.IPC.GetComboState(
            (singleTarget
                ? (simpleMode ? Options.DNC_ST_SimpleMode : Options.DNC_ST_AdvancedMode)
                : (simpleMode ? Options.DNC_AoE_SimpleMode : Options.DNC_AoE_AdvancedMode)
            ).ToString()
        )!.Values.Last();

    #region Openers

    internal static FifteenSecondOpener Opener15S = new();

    internal class FifteenSecondOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            StandardStep,
            Emboite,
            Emboite,
            StandardFinish2,
            TechnicalStep, //5
            Emboite,
            Emboite,
            Emboite,
            Emboite,
            TechnicalFinish4, //10
            Devilment,
            Tillana,
            Flourish,
            DanceOfTheDawn,
            FanDance4, //15
            LastDance,
            FanDance3,
            FinishingMove,
            StarfallDance,
            ReverseCascade, //20
            ReverseCascade,
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([4], 12)
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            ([2, 3, 6, 7, 8, 9], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 6, 7, 8, 9], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 6, 7, 8, 9], Pirouette, () => Gauge.NextStep == Pirouette),
            ([19], SaberDance, () => Gauge.Esprit >= 50),
            ([20, 21, 22], SaberDance, () => Gauge.Esprit > 80),
            ([20, 21, 22], StarfallDance,
                () => HasEffect(Buffs.FlourishingStarfall)),
            ([20, 21, 22], SaberDance, () => Gauge.Esprit >= 50),
            ([20, 21, 22], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([20, 21, 22], Fountainfall, () =>
                HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow)),
        ];

        internal override UserData? ContentCheckConfig =>
            Config.DNC_ST_OpenerDifficulty;

        public override bool HasCooldowns()
        {
            if (!ActionReady(StandardStep))
                return false;

            if (!ActionReady(TechnicalStep))
                return false;

            if (!ActionReady(Devilment))
                return false;

            if (InCombat())
                return false;

            if (!CountdownActive)
                return false;

            // go at 15s, with some leeway
            if (CountdownRemaining is > 13.5f and < 16f)
                return false;

            return true;
        }
    }

    internal static SevenSecondOpener Opener07S = new();

    internal class SevenSecondOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            StandardStep,
            Emboite,
            Emboite,
            StandardFinish2,
            TechnicalStep, //5
            Emboite,
            Emboite,
            Emboite,
            Emboite,
            TechnicalFinish4, //10
            Devilment,
            Tillana,
            Flourish,
            DanceOfTheDawn,
            FanDance4, //15
            LastDance,
            FanDance3,
            StarfallDance,
            ReverseCascade,
            ReverseCascade, //20
            FinishingMove,
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([4], 4)
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            ([2, 3, 6, 7, 8, 9], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 6, 7, 8, 9], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 6, 7, 8, 9], Pirouette, () => Gauge.NextStep == Pirouette),
            ([21], SaberDance, () => Gauge.Esprit >= 50),
            ([19, 20, 22], SaberDance, () => Gauge.Esprit > 80),
            ([19, 20, 22], StarfallDance,
                () => HasEffect(Buffs.FlourishingStarfall)),
            ([19, 20, 22], SaberDance, () => Gauge.Esprit >= 50),
            ([19, 20, 22], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([19, 20, 22], Fountainfall, () =>
                HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow)),
        ];

        internal override UserData? ContentCheckConfig =>
            Config.DNC_ST_OpenerDifficulty;

        public override bool HasCooldowns()
        {
            if (!ActionReady(StandardStep))
                return false;

            if (!ActionReady(TechnicalStep))
                return false;

            if (!ActionReady(Devilment))
                return false;

            if (InCombat())
                return false;

            if (!CountdownActive)
                return false;

            // go at 7s, with some leeway
            if (CountdownRemaining is > 5.5f and < 8f)
                return false;

            return true;
        }
    }

    #endregion
}
