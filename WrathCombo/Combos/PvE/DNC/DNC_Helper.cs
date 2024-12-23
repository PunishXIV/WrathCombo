#region

 using System;
 using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DNC
{
    internal static WrathOpener Opener()
    {
        if (Config.DNC_ST_OpenerSelection == 0 && Opener1.LevelChecked) return Opener1;
        if (Config.DNC_ST_OpenerSelection == 1 && Opener2.LevelChecked) return Opener2;

        return WrathOpener.Dummy;
    }

    private static DNCGauge Gauge => GetJobGauge<DNCGauge>();

    #region Openers

    internal static DNCOpenerMaxLevel1 Opener1 = new();

    internal class DNCOpenerMaxLevel1 : WrathOpener
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
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays { get; set; } =
        [
            ([4], 12)
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([2, 3, 6, 7, 8, 9], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 6, 7, 8, 9], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 6, 7, 8, 9], Pirouette, () => Gauge.NextStep == Pirouette),
            ([19], SaberDance, () => Gauge.Esprit >= 50),
            ([20, 21, 22], SaberDance, () => Gauge.Esprit > 80),
            ([20, 21, 22], StarfallDance, () => HasEffect(Buffs.FlourishingStarfall)),
            ([20, 21, 22], SaberDance  , () => Gauge.Esprit >= 50),
            ([20, 21, 22], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([20, 21, 22], Fountainfall, () =>
                HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow)),
        ];
        internal override UserData? ContentCheckConfig => Config.DNC_ST_OpenerDifficulty;

        public override bool HasCooldowns()
        {
            if (!ActionReady(StandardStep))
                return false;

            if (!ActionReady(TechnicalStep))
                return false;

            if (!ActionReady(Devilment))
                return false;

            return true;
        }
    }

    internal static DNCOpenerMaxLevel2 Opener2 = new();

    internal class DNCOpenerMaxLevel2 : WrathOpener
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
            ReverseCascade,
        ];

        public override List<(int[] Steps, int HoldDelay)> PrepullDelays { get; set; } =
        [
            ([4], 4)
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([2, 3, 6, 7, 8, 9], Entrechat, () => Gauge.NextStep == Entrechat),
            ([2, 3, 6, 7, 8, 9], Jete, () => Gauge.NextStep == Jete),
            ([2, 3, 6, 7, 8, 9], Pirouette, () => Gauge.NextStep == Pirouette),
            ([21], SaberDance, () => Gauge.Esprit >= 50),
            ([19, 20, 22], SaberDance, () => Gauge.Esprit > 80),
            ([19, 20, 22], StarfallDance, () => HasEffect(Buffs.FlourishingStarfall)),
            ([19, 20, 22], SaberDance  , () => Gauge.Esprit >= 50),
            ([19, 20, 22], LastDance, () => HasEffect(Buffs.LastDanceReady)),
            ([19, 20, 22], Fountainfall, () =>
                HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow)),
        ];

        internal override UserData? ContentCheckConfig => Config.DNC_ST_OpenerDifficulty;
        public override bool HasCooldowns()
        {
            if (!ActionReady(StandardStep))
                return false;

            if (!ActionReady(TechnicalStep))
                return false;

            if (!ActionReady(Devilment))
                return false;

            return true;
        }
    }

    #endregion
}
