#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Options = WrathCombo.Combos.CustomComboPreset;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DNC
{
    /// <summary>
    ///     Logic to pick different openers.
    /// </summary>
    /// <returns>The chosen Opener.</returns>
    internal static WrathOpener Opener()
    {
        if (Config.DNC_ST_OpenerSelection == (int)Config.Openers.FifteenSecond &&
            Opener15S.LevelChecked)
            return Opener15S;
        if (Config.DNC_ST_OpenerSelection == (int)Config.Openers.SevenSecond &&
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

        for (var i = 0; i < CustomDanceStepActions.Length; i++)
        {
            if (CustomDanceStepActions[i] != action) continue;

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
}
