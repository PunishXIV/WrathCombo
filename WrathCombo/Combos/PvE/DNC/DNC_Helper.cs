#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Options = WrathCombo.Combos.CustomComboPreset;

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
    ///     Dancer Gauge data, just consolidated.
    /// </summary>
    private static DNCGauge Gauge => GetJobGauge<DNCGauge>();

    /// <summary>
    ///     DNC's GCD, truncated to two decimal places.
    /// </summary>
    private static double GCD =>
        Math.Floor(GetCooldown(Cascade).CooldownTotal * 100) / 100;

    /// <summary>
    ///     Checks if any enemy is within 15 yalms.
    /// </summary>
    /// <remarks>
    ///     This is used for <see cref="StandardFinish2" />,
    ///     <see cref="TechnicalFinish4" />, <see cref="FinishingMove" />,
    ///     and <see cref="Tillana" />.
    /// </remarks>
    private static bool EnemyIn15Yalms => CanCircleAoe(15, true) > 0;

    /// <summary>
    ///     Checks if any enemy is within 8 yalms.
    /// </summary>
    /// <remarks>
    ///     This is used for <see cref="Improvisation" />.
    /// </remarks>
    private static bool EnemyIn8Yalms => CanCircleAoe(8, true) > 0;

    /// <summary>
    ///     Logic to pick different openers.
    /// </summary>
    /// <returns>The chosen Opener.</returns>
    internal static WrathOpener Opener()
    {
        if (Config.DNC_ST_OpenerSelection ==
            (int)Config.Openers.FifteenSecond &&
            Opener15S.LevelChecked)
            return Opener15S;

        if (Config.DNC_ST_OpenerSelection ==
            (int)Config.Openers.SevenSecond &&
            Opener07S.LevelChecked)
            return Opener07S;

        if (Config.DNC_ST_OpenerSelection ==
            (int)Config.Openers.ThirtySecondTech &&
            Opener30STech.LevelChecked)
            return Opener30STech;

        if (Config.DNC_ST_OpenerSelection ==
            (int)Config.Openers.SevenPlusSecondTech &&
            Opener07PlusSTech.LevelChecked)
            return Opener07PlusSTech;

        if (Config.DNC_ST_OpenerSelection ==
            (int)Config.Openers.SevenSecondTech &&
            Opener07STech.LevelChecked)
            return Opener07STech;

        return WrathOpener.Dummy;
    }

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

    /// <summary>
    ///     Hold or Return a dance's Finisher based on user options and enemy ranges.
    /// </summary>
    /// <param name="desiredFinish">
    ///     Which Finisher should be returned.<br />
    ///     Expects <see cref="StandardFinish2" /> or
    ///     <see cref="TechnicalFinish4" />.
    /// </param>
    /// <returns>
    ///     The Finisher to use, or if
    ///     <see cref="CustomComboPreset.DNC_ST_BlockFinishes" /> is enabled and
    ///     there is no enemy in range: <see cref="All.SavageBlade" />.
    /// </returns>
    private static uint FinishOrHold(uint desiredFinish)
    {
        // If the option to hold is not enabled
        if (IsNotEnabled(Options.DNC_ST_BlockFinishes))
            return desiredFinish;

        // Return the Finish if the dance is about to expire
        if (desiredFinish is StandardFinish2 &&
            GetStatusEffectRemainingTime(Buffs.StandardStep) < GCD * 1.5)
            return desiredFinish;
        if (desiredFinish is TechnicalFinish4 &&
            GetStatusEffectRemainingTime(Buffs.TechnicalStep) < GCD * 1.5)
            return desiredFinish;

        // If there is no enemy in range, hold the finish
        if (!EnemyIn15Yalms)
            return All.SavageBlade;

        // If there is an enemy in range, or as a fallback, return the desired finish
        return desiredFinish;
    }

    #region GCD Evaluation

    private static GCDRange GCDValue =>
        GCD switch
        {
            2.50 => GCDRange.Perfect,
            2.49 => GCDRange.NotGood,
            _ => GCDRange.Bad,
        };

    private enum GCDRange
    {
        Perfect,
        NotGood,
        Bad,
    }

    #endregion

    #region Dance Partner

    internal static ulong? CurrentDancePartner =>
        GetPartyMembers()
            .Where(HasMyPartner)
            .Select(x => (ulong?)x.GameObjectId)
            .FirstOrDefault();

    internal static ulong? DesiredDancePartner =>
        TryGetDancePartner(out var partner) ? partner.GameObjectId : null;

    private static bool TryGetDancePartner
        (out IGameObject? partner, bool? callingFromFeature = null)
    {
        partner = null;
        var playerID = LocalPlayer.GameObjectId;
        var party = GetPartyMembers()
            .Where(member => member.GameObjectId != playerID)
            .Where(member => !member.BattleChara.IsDead)
            .Where(member => IsInRange(member.BattleChara, 30))
            .Where(member => !HasAnyPartner(member) || HasMyPartner(member))
            .Select(member => member.BattleChara)
            .ToList();

        // Bails
        if (!Player.Available)
            return false;
        if (party.Count <= 1 && !HasCompanionPresent())
            return false;

        // Check if we have a target overriding any searching
        /*
         if (callingFromFeature is true &&
            IsEnabled(Options.DNC_Desirable_TargetOverride) &&
            LocalPlayer.TargetObject is IBattleChara &&
            !LocalPlayer.TargetObject.IsDead &&
            party.Any(x =>
                x.GameObjectId == LocalPlayer.TargetObject.GameObjectId) &&
            IsInRange(LocalPlayer.TargetObject, 30))
        {
            partner = LocalPlayer.TargetObject;
            return true;
        }*/

        // Search for a partner
        if (TryGetBestPartner(out var bestPartner))
        {
            partner = bestPartner;
            return true;
        }

        // Fallback to companion
        if (HasCompanionPresent())
        {
            partner = Svc.Buddies.CompanionBuddy.GameObject;
            return true;
        }

        // Fallback to first party slot that isn't the player
        if (party.Count > 1)
        {
            partner = party.First();
            return true;
        }

        return false;

        #region Sickness-checking shortcut methods

        bool SicknessFree(IGameObject target)
        {
            return !TargetHasRezWeakness(target);
        }

        bool BrinkFree(IGameObject target)
        {
            return !TargetHasRezWeakness(target, false);
        }

        #endregion

        bool TryGetBestPartner(out IGameObject? newBestPartner, int step = 0)
        {
            #region Variable Setup

            newBestPartner = null;
            var restrictions = PartnerPriority.RestrictionSteps[step];
            var filter = party;
            const int melee = (int)PartnerPriority.Role.Melee;
            const int ranged = (int)PartnerPriority.Role.Ranged;

            #endregion

            if (restrictions.HasFlag(PartnerPriority.Restrictions.MustBeMelee))
                filter = filter
                    .Where(x => x.ClassJob.RowId.Role() is melee).ToList();

            if (restrictions.HasFlag(PartnerPriority.Restrictions.MustBeDPS))
                filter = filter
                    .Where(x => x.ClassJob.RowId.Role() is melee or ranged)
                    .ToList();

            if (restrictions.HasFlag(PartnerPriority.Restrictions
                    .MustBeSicknessFree))
                filter = filter.Where(SicknessFree).ToList();

            if (restrictions.HasFlag(PartnerPriority.Restrictions.MustBeBrinkFree))
                filter = filter.Where(BrinkFree).ToList();

            if (filter.Count == 0 &&
                step < PartnerPriority.RestrictionSteps.Length - 1)
                return TryGetBestPartner(out newBestPartner, step + 1);
            if (filter.Count == 0 && step == 6)
                return false;

            filter = filter
                .OrderBy(x =>
                    PartnerPriority.RolePrio.GetValueOrDefault(
                        x.ClassJob.RowId.Role(), int.MaxValue))
                .ThenBy(x =>
                    Player.Level >= 90
                        ? PartnerPriority.Job090Prio.GetValueOrDefault(
                            x.ClassJob.RowId, int.MaxValue)
                        : int.MaxValue)
                .ThenBy(x =>
                    Player.Level >= 100
                        ? PartnerPriority.Job100Prio.GetValueOrDefault(
                            x.ClassJob.RowId, int.MaxValue)
                        : int.MaxValue)
                .ToList();

            newBestPartner = filter.First();
            return true;
        }
    }

    private static bool HasAnyPartner(WrathPartyMember target) =>
        HasStatusEffect(Buffs.Partner, target.BattleChara, true);

    private static bool HasMyPartner(WrathPartyMember target) =>
        HasStatusEffect(Buffs.Partner, target.BattleChara);

    #region Partner Priority Static Data

    private static class PartnerPriority
    {
        internal static readonly Dictionary<int, int> RolePrio = new()
        {
            { (int)Role.Melee, 1 },
            { (int)Role.Ranged, 1 },
            { (int)Role.Tank, 2 },
            { (int)Role.Healer, 3 },
        };

        internal static readonly Dictionary<uint, int> Job100Prio = new()
        {
            { PCT.JobID, 1 },
            { SAM.JobID, 1 },
            { RPR.JobID, 2 },
            { VPR.JobID, 2 },
            { MNK.JobID, 2 },
            { NIN.JobID, 2 },
            { DRG.JobID, 3 },
            { BLM.JobID, 3 },
            { RDM.JobID, 4 },
            { SMN.JobID, 5 },
            { MCH.JobID, 6 },
            { BRD.JobID, 7 },
            { JobID, 8 },
        };

        internal static readonly Dictionary<uint, int> Job090Prio = new()
        {
            { PCT.JobID, 0 },
            { SAM.JobID, 1 },
            { NIN.JobID, 2 },
            { MNK.JobID, 3 },
            { RPR.JobID, 4 },
            { BLM.JobID, 5 },
            { DRG.JobID, 6 },
            { VPR.JobID, 7 },
            { SMN.JobID, 8 },
            { RDM.JobID, 9 },
            { MCH.JobID, 10 },
            { BRD.JobID, 11 },
            { JobID, 12 },
        };

        internal static readonly Restrictions[] RestrictionSteps =
        [
            // Sickness-free DPS
            Restrictions.MustBeMelee | Restrictions.MustBeSicknessFree,
            Restrictions.MustBeDPS | Restrictions.MustBeSicknessFree,
            // Sick DPS
            Restrictions.MustBeMelee | Restrictions.MustBeBrinkFree,
            Restrictions.MustBeDPS | Restrictions.MustBeBrinkFree,
            // Sickness-free
            Restrictions.MustBeSicknessFree,
            // Sick
            Restrictions.MustBeBrinkFree,
            // :(
            Restrictions.ScrapeTheBottom,
        ];

        internal enum Role
        {
            Tank = 1,
            Melee = 2,

            /// Casters and Phys Ranged
            Ranged = 3,
            Healer = 4,
        }

        [Flags]
        internal enum Restrictions
        {
            MustBeMelee = 1 << 0, // 1
            MustBeDPS = 1 << 1, // 2
            MustBeSicknessFree = 1 << 2, // 4
            MustBeBrinkFree = 1 << 3, // 8
            ScrapeTheBottom = 1 << 4, // 16
        }
    }

    #endregion

    #endregion

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
    ///     The matching dance step the action was assigned to.<br />
    ///     Will be Savage Blade if used and was not a custom dance step.<br />
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

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([4], () => 7),
            ([5], () => 5),
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
                () => HasStatusEffect(Buffs.FlourishingStarfall)),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], LastDance, () => HasStatusEffect(Buffs.LastDanceReady)),
            ([21, 22, 23], Fountainfall, () =>
                HasStatusEffect(Buffs.SilkenFlow) || HasStatusEffect(Buffs.FlourishingFlow)),
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

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([4], () => 2),
            ([5], () => 2),
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
                () => HasStatusEffect(Buffs.FlourishingStarfall)),
            ([20, 21, 23], SaberDance, () => Gauge.Esprit >= 50),
            ([20, 21, 23], LastDance, () => HasStatusEffect(Buffs.LastDanceReady)),
            ([20, 21, 23], Fountainfall, () =>
                HasStatusEffect(Buffs.SilkenFlow) || HasStatusEffect(Buffs.FlourishingFlow)),
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

    internal static ThirtySecondTechOpener Opener30STech = new();

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
            LastDance,
            Flourish,
            FinishingMove, //15
            Tillana,
            DanceOfTheDawn,
            FanDance4,
            StarfallDance,
            FanDance3, //20
            ReverseCascade,
            ReverseCascade,
            ReverseCascade,
        ];

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([5], () => 1),
            ([6], () => 6),
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
            ([19], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit > 80),
            ([21, 22, 23], StarfallDance,
                () => HasStatusEffect(Buffs.FlourishingStarfall)),
            ([21, 22, 23], SaberDance, () => Gauge.Esprit >= 50),
            ([21, 22, 23], LastDance, () => HasStatusEffect(Buffs.LastDanceReady)),
            ([21, 22, 23], Fountainfall, () =>
                HasStatusEffect(Buffs.SilkenFlow) || HasStatusEffect(Buffs.FlourishingFlow)),
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

    internal static SevenPlusSecondTechOpener Opener07PlusSTech = new();

    internal class SevenPlusSecondTechOpener : WrathOpener
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
            TechnicalFinish4,
            Devilment,
            LastDance,
            Flourish,
            FinishingMove, //10
            Tillana,
            DanceOfTheDawn,
            FanDance4,
            StarfallDance,
            FanDance3, //15
            ReverseCascade,
            ReverseCascade,
            ReverseCascade,
        ];

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays
        {
            get;
            set;
        } = [];

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
                HasStatusEffect(Buffs.FlourishingStarfall)),
            ([16, 17, 18], SaberDance, () => Gauge.Esprit >= 50),
            ([16, 17, 18], LastDance, () => HasStatusEffect(Buffs.LastDanceReady)),
            ([16, 17, 18], Fountainfall, () =>
                HasStatusEffect(Buffs.SilkenFlow) || HasStatusEffect(Buffs.FlourishingFlow)),
        ];

        internal override UserData? ContentCheckConfig =>
            Config.DNC_ST_OpenerDifficulty;

        public override bool HasCooldowns()
        {
            if (ActionReady(StandardStep))
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

    internal static SevenSecondTechOpener Opener07STech = new();

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

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays
        {
            get;
            set;
        } =
        [
            ([7], () => 2),
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
                HasStatusEffect(Buffs.FlourishingStarfall)),
            ([16, 17, 18], SaberDance, () => Gauge.Esprit >= 50),
            ([16, 17, 18], LastDance, () => HasStatusEffect(Buffs.LastDanceReady)),
            ([16, 17, 18], Fountainfall, () =>
                HasStatusEffect(Buffs.SilkenFlow) || HasStatusEffect(Buffs.FlourishingFlow)),
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
        Ending = 18073,
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
            Partner = 1824,
            ShieldSamba = 1826,
            LastDanceReady = 3867,
            FinishingMoveReady = 3868,
            DanceOfTheDawnReady = 3869,
            Devilment = 1825;
    }

    #endregion
}
