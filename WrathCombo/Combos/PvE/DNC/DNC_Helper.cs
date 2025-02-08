#region

using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Options = WrathCombo.Combos.CustomComboPreset;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

internal partial class DNC
{
    /// <summary>
    ///     Logic to pick different openers.
    /// </summary>
    /// <returns>The chosen Opener.</returns>
    internal static WrathOpener Opener()
    {
        if (Config.DNC_ST_OpenerSelection ==
            (int) Config.Openers.FifteenSecond &&
            Opener15S.LevelChecked)
            return Opener15S;

        if (Config.DNC_ST_OpenerSelection ==
            (int) Config.Openers.SevenSecond &&
            Opener07S.LevelChecked)
            return Opener07S;

        if (Config.DNC_ST_OpenerSelection ==
            (int) Config.Openers.ThirtySecondTech &&
            Opener30TechS.LevelChecked)
            return Opener30TechS;

        if (Config.DNC_ST_OpenerSelection ==
            (int) Config.Openers.SevenSecondTech &&
            Opener07TechS.LevelChecked)
            return Opener07TechS;

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
                ? (simpleMode
                    ? Options.DNC_ST_SimpleMode
                    : Options.DNC_ST_AdvancedMode)
                : (simpleMode
                    ? Options.DNC_AoE_SimpleMode
                    : Options.DNC_AoE_AdvancedMode)
            ).ToString()
        )!.Values.Last();

    #region Custom Dance Step Logic

    /// <summary>
    ///     Consolidating a few checks to reduce duplicate code.
    /// </summary>
    private static bool WantsCustomStepsOnSmallerFeatures =>
        IsEnabled(Options.DNC_CustomDanceSteps) &&
        IsEnabled(Options.DNC_CustomDanceSteps_Conflicts) &&
        Gauge.IsDancing;

    /// <summary>
    ///     Saved custom dance steps.
    /// </summary>
    /// <seealso cref="DNC_DanceComboReplacer.Invoke">DanceComboReplacer</seealso>
    private static uint[] CustomDanceStepActions =>
        Service.Configuration.DancerDanceCompatActionIDs;

    /// <summary>
    ///     Checks if the action is a custom dance step and replaces it with the
    ///     appropriate step if so.
    /// </summary>
    /// <param name="action">The action ID to check.</param>
    /// <param name="updatedAction">
    ///     The matching dance step the action was assigned to.<br/>
    ///     Will be Savage Blade if used and was not a custom dance step.<br/>
    ///     Do not use this value if the return is <c>false</c>.
    /// </param>
    /// <returns>If the action was assigned as a custom dance step.</returns>
    private static bool GetCustomDanceStep(uint action, out uint updatedAction)
    {
        updatedAction = All.SavageBlade;

        if (!CustomDanceStepActions.Contains(action))
            return false;

        for (int i = 0; i < CustomDanceStepActions.Length; i++)
        {
            if (CustomDanceStepActions[i] != action)
                continue;

            // This is simply the order of the UI
            updatedAction = i switch
            {
                0 => Emboite,
                1 => Entrechat,
                2 => Jete,
                3 => Pirouette,
                _ => updatedAction
            };
        }

        return false;
    }

    #endregion

    #region Openers

    #region Standard Openers

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
            Peloton,
            StandardFinish2, //5
            TechnicalStep,
            Emboite,
            Emboite,
            Emboite,
            Emboite, //10
            TechnicalFinish4,
            Devilment,
            Tillana,
            Flourish,
            DanceOfTheDawn, //15
            FanDance4,
            LastDance,
            FanDance3,
            FinishingMove,
            StarfallDance, //20
            ReverseCascade,
            ReverseCascade,
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([4], 7),
            ([5], 5),
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            ([2, 3, 7, 8, 9, 10], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 7, 8, 9, 10], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 7, 8, 9, 10], Pirouette, () => Gauge.NextStep == Pirouette),
            ([20], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit > 80),
            ([21, 22, 23], StarfallDance,
                () => HasEffect(Buffs.FlourishingStarfall)),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([21, 22, 23], Fountainfall, () =>
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

            if (!IsOffCooldown(Devilment))
                return false;

            if (InCombat())
                return false;

            if (!CountdownActive)
                return false;

            // go at 15s, with some leeway
            if (CountdownRemaining is < 13.5f or > 16f)
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
            Peloton,
            StandardFinish2, //5
            TechnicalStep,
            Emboite,
            Emboite,
            Emboite,
            Emboite, //10
            TechnicalFinish4,
            Devilment,
            Tillana,
            Flourish,
            DanceOfTheDawn, //15
            FanDance4,
            LastDance,
            FanDance3,
            StarfallDance,
            ReverseCascade, //20
            ReverseCascade,
            FinishingMove,
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([4], 2),
            ([5], 2),
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            ([2, 3, 7, 8, 9, 10], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 7, 8, 9, 10], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 7, 8, 9, 10], Pirouette, () => Gauge.NextStep == Pirouette),
            ([22], SaberDance, () => Gauge.Esprit >= 50),
            ([20, 21, 23], SaberDance, () => Gauge.Esprit > 80),
            ([20, 21, 23], StarfallDance,
                () => HasEffect(Buffs.FlourishingStarfall)),
            ([20, 21, 23], SaberDance, () => Gauge.Esprit >= 50),
            ([20, 21, 23], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([20, 21, 23], Fountainfall, () =>
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

            if (!IsOffCooldown(Devilment))
                return false;

            if (InCombat())
                return false;

            if (!CountdownActive)
                return false;

            // go at 7s, with some leeway
            if (CountdownRemaining is < 5.5f or > 8f)
                return false;

            return true;
        }
    }

    #endregion

    #region Technical Openers

    internal static ThirtySecondTechOpener Opener30TechS = new();

    internal class ThirtySecondTechOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            StandardStep,
            Emboite,
            Emboite,
            StandardFinish2,
            Peloton, //5
            TechnicalStep,
            Emboite,
            Emboite,
            Emboite,
            Emboite, //10
            TechnicalFinish4,
            Devilment,
            Tillana,
            Flourish,
            DanceOfTheDawn, //15
            FanDance4,
            LastDance,
            FanDance3,
            FinishingMove,
            StarfallDance, //20
            ReverseCascade,
            ReverseCascade,
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([5], 1),
            ([6], 6),
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            ([2, 3, 7, 8, 9, 10], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 7, 8, 9, 10], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 7, 8, 9, 10], Pirouette, () => Gauge.NextStep == Pirouette),
            ([20], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit > 80),
            ([21, 22, 23], StarfallDance,
                () => HasEffect(Buffs.FlourishingStarfall)),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([21, 22, 23], Fountainfall, () =>
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

            if (!IsOffCooldown(Devilment))
                return false;

            if (InCombat())
                return false;

            if (!CountdownActive)
                return false;

            // go at 30s, with some leeway
            if (CountdownRemaining < 28.5f)
                return false;

            return true;
        }
    }

    internal static SevenSecondTechOpener Opener07TechS = new();

    internal class SevenSecondTechOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            TechnicalStep,
            Emboite,
            Emboite,
            Emboite,
            Emboite, //5
            Peloton,
            TechnicalFinish4,
            Devilment,
            Tillana,
            Flourish, //10
            FinishingMove,
            DanceOfTheDawn,
            FanDance4,
            StarfallDance,
            FanDance3, //15
            ReverseCascade,
            ReverseCascade,
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([7], 2),
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            ([2, 3, 4, 5], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 4, 5], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 4, 5], Pirouette, () => Gauge.NextStep == Pirouette),
            ([14], SaberDance, () => Gauge.Esprit >= 50),
            ([16, 17, 18], SaberDance, () => Gauge.Esprit > 80),
            ([16, 17, 18], StarfallDance, () =>
                HasEffect(Buffs.FlourishingStarfall)),
            ([16, 17, 18], SaberDance, () => Gauge.Esprit >= 50),
            ([16, 17, 18], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([16, 17, 18], Fountainfall, () =>
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

            if (!IsOffCooldown(Devilment))
                return false;

            if (InCombat())
                return false;

            if (!CountdownActive)
                return false;

            // go at 7s, with some leeway
            if (CountdownRemaining is < 5.5f or > 8f)
                return false;

            return true;
        }
    }

    #endregion

    #endregion

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
}
