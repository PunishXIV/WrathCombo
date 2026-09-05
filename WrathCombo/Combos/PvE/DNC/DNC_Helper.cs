#region

using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.Combos.PvE.DNC.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

#endregion

namespace WrathCombo.Combos.PvE;

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
    private static bool EnemyIn15Yalms => NumberOfEnemiesInRange(FinishingMove) > 0;

    /// <summary>
    ///     Checks if any enemy is within 8 yalms.
    /// </summary>
    /// <remarks>
    ///     This is used for <see cref="Improvisation" />.
    /// </remarks>
    private static bool AlliesIn8Yalms => NumberOfAlliesInRange(Improvisation) > 2;

    /// <summary>
    ///     Logic to pick different openers.
    /// </summary>
    /// <returns>The chosen Opener.</returns>
    internal static WrathOpener Opener()
    {
        if (DNC_ST_OpenerSelection ==
            (int)Openers.FifteenSecond &&
            Opener15S.LevelChecked)
            return Opener15S;

        if (DNC_ST_OpenerSelection ==
            (int)Openers.SevenSecond &&
            Opener07S.LevelChecked)
            return Opener07S;

        if (DNC_ST_OpenerSelection ==
            (int)Openers.ThirtySecondTech &&
            Opener30STech.LevelChecked)
            return Opener30STech;

        if (DNC_ST_OpenerSelection ==
            (int)Openers.SevenPlusSecondTech &&
            Opener07PlusSTech.LevelChecked)
            return Opener07PlusSTech;

        if (DNC_ST_OpenerSelection ==
            (int)Openers.SevenSecondTech &&
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
                    ? Preset.DNC_ST_SimpleMode
                    : Preset.DNC_ST_AdvancedMode)
                : (simpleMode
                    ? Preset.DNC_AoE_SimpleMode
                    : Preset.DNC_AoE_AdvancedMode)
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
    ///     <see cref="Preset.DNC_ST_BlockFinishes" /> is enabled and
    ///     there is no enemy in range: <see cref="All.Cease" />.
    /// </returns>
    private static uint FinishOrHold(uint desiredFinish)
    {
        // If the option to hold is not enabled
        if (IsNotEnabled(Preset.DNC_ST_BlockFinishes))
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
            return All.Cease;

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

    internal static ulong? CurrentDancePartner
    {
        get
        {
            if (!EZ.Throttle("dncPartnerCurrentCheck", TS.FromSeconds(1.9)))
                return field;

            field = GetPartyMembers()
                .Where(HasMyPartner)
                .FirstOrDefault()?
                .GameObjectId;
            return field;
        }
    }

    internal static ulong? DesiredDancePartner
    {
        get
        {
            if (!EZ.Throttle("dncPartnerDesiredCheck", TS.FromSeconds(2)) &&
                field is not null)
            {
                if (IsDancePartnerReady(field.Value.GetObject()))
                    return field;
                // Cached partner no longer ready (cutscene, etc.) — refresh
            }

            if (Player.Object is null ||
                Player.Job != Job.DNC ||
                IsOccupied() ||
                !ActionLearned(ClosedPosition))
                return field = null;

            field = TryGetDancePartner(out var partner)
                ? partner.GameObjectId
                : null;
            return field;
        }
    }

    private static bool CurrentPartnerNonOptimal =>
        DesiredDancePartner is not null &&
        (
            // Have no partner and one is theoretically available
            (!HasStatusEffect(Buffs.ClosedPosition) &&
             (IsInParty() || HasCompanionPresent())) ||
            // Have a partner, but it's not the optimal one
            (CurrentDancePartner is not null &&
             DesiredDancePartner != CurrentDancePartner)
        );

    /// <summary>
    ///     True when Closed Position can actually land on the target
    ///     (not mid-cutscene / loading / otherwise untargetable).
    /// </summary>
    internal static bool IsDancePartnerReady(IGameObject? target) =>
        target is not null &&
        !target.IsDead &&
        target.IsTargetable &&
        // OnlineStatus 15 = Viewing Cutscene
        target is not IPlayerCharacter { OnlineStatus.RowId: 15 } &&
        target.CanUseOn(ClosedPosition);

    [ActionRetargeting.TargetResolver]
    internal static IGameObject? DancePartnerResolver()
    {
        var desired = DesiredDancePartner.GetObject();
        if (IsDancePartnerReady(desired))
            return desired;

        if (HasStatusEffect(Buffs.ClosedPosition))
            return null;

        var fallback = SimpleTarget.AnySelfishDPS ??
                       SimpleTarget.AnyMeleeDPS ??
                       SimpleTarget.AnyDPS;
        return IsDancePartnerReady(fallback) ? fallback : null;
    }

    private static bool TryGetDancePartner(out IGameObject? partner)
    {
        partner = null;

        if (!Player.Available)
            return false;

        #region Skip a new check, if the current partner is just out of range
        if (CurrentDancePartner is not null)
        {
            var currentPartner = CurrentDancePartner.GetObject();
            if (currentPartner is not null &&
                !currentPartner.IsWithinRange(30) &&
                !currentPartner.IsDead &&
                DamageDownFree(currentPartner))
                return false;
        }
        #endregion

        // Check if we have a target overriding any searching
        var focusTarget = SimpleTarget.FocusTarget;
        if (DNC_Partner_FocusOverride &&
            focusTarget is IBattleChara &&
            focusTarget.IsInParty() &&
            IsInRange(focusTarget, 30) &&
            SicknessFree(focusTarget) &&
            DamageDownFree(focusTarget) &&
            IsDancePartnerReady(focusTarget))
        {
            partner = focusTarget;
            return true;
        }

        var party = GetPartyMembers()
            .Where(member => member.GameObject.IsNotThePlayer() &&
                             member.BattleChara is not null &&
                             member.GameObject.IsWithinRange(30) &&
                             (!HasAnyPartner(member) ||
                              HasMyPartner(member)) &&
                             IsDancePartnerReady(member.BattleChara))
            .Select(member => member.BattleChara!)
            .ToList();

        if (party.Count < 1 && !HasCompanionPresent())
            return false;

        // Search for a partner
        if (TryGetBestPartner(out var bestPartner))
        {
            partner = bestPartner;
            return true;
        }

        // Fallback to companion
        if (HasCompanionPresent() &&
            IsDancePartnerReady(SimpleTarget.Chocobo))
        {
            partner = SimpleTarget.Chocobo;
            return true;
        }

        // Fallback to first party slot that isn't the player
        if (party.Count >= 1)
        {
            partner = party.First();
            return true;
        }

        return false;

        #region Status-checking shortcut methods

        // These are here so I don't have to add a ton of methods to DNC

        bool DamageDownFree(IGameObject? target) =>
            !TargetHasDamageDown(target);

        bool SicknessFree(IGameObject? target) =>
            !TargetHasRezWeakness(target);

        bool BrinkFree(IGameObject? target) =>
            !TargetHasRezWeakness(target, false);

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

            if (restrictions.HasFlag(PartnerPriority.Restrictions.Melee))
                filter = [.. filter.Where(x => x.ClassJob.Value.Role is melee)];

            if (restrictions.HasFlag(PartnerPriority.Restrictions.DPS))
                filter = [.. filter.Where(x => x.ClassJob.Value.Role is melee or ranged)];

            if (restrictions.HasFlag(PartnerPriority.Restrictions.NotDD))
                filter = [.. filter.Where(DamageDownFree)];

            if (restrictions.HasFlag(PartnerPriority.Restrictions.NotSick))
                filter = [.. filter.Where(SicknessFree)];

            if (restrictions.HasFlag(PartnerPriority.Restrictions.NotBrink))
                filter = [.. filter.Where(BrinkFree)];

            // Run the next step if no matches were found
            if (filter.Count == 0 &&
                step < PartnerPriority.RestrictionSteps.Length - 1)
                return TryGetBestPartner(out newBestPartner, step + 1);
            // If it's the last step and there are no matches found, bail
            if (filter.Count == 0)
                return false;
            // If there's only one match, return it
            if (filter.Count == 1)
            {
                newBestPartner = filter.First();
                return true;
            }

            var orderedFilter = filter
                .OrderBy(x =>
                    PartnerPriority.RolePrio.GetValueOrDefault(
                        x.ClassJob.Value.Role, int.MaxValue));

            switch (Svc.PlayerState.EffectiveLevel)
            {
                case < 80:
                    orderedFilter = orderedFilter
                        .ThenBy(x =>
                            PartnerPriority.Job070Prio.GetValueOrDefault(
                                (Job)x.ClassJob.RowId, int.MaxValue));
                    break;
                case < 90:
                    orderedFilter = orderedFilter
                        .ThenBy(x =>
                            PartnerPriority.Job080Prio.GetValueOrDefault(
                                (Job)x.ClassJob.RowId, int.MaxValue));
                    break;
                case < 100:
                    orderedFilter = orderedFilter
                        .ThenBy(x =>
                            PartnerPriority.Job090Prio.GetValueOrDefault(
                                (Job)x.ClassJob.RowId, int.MaxValue));
                    break;
                case >= 100:
                    orderedFilter = orderedFilter
                        .ThenBy(x =>
                            PartnerPriority.Job100Prio.GetValueOrDefault(
                                (Job)x.ClassJob.RowId, int.MaxValue));
                    break;
            }

            // Simple ilvl tie-breaker
            orderedFilter = orderedFilter.ThenByDescending(x => x.MaxHp);

            filter = orderedFilter.ToList();

            newBestPartner = filter.First();
            return true;
        }
    }

    #region DP-checking shortcut methods

    private static bool HasAnyPartner(WrathPartyMember target) =>
        HasStatusEffect(Buffs.Partner, target.BattleChara, true);

    private static bool HasMyPartner(WrathPartyMember target) =>
        HasStatusEffect(Buffs.Partner, target.BattleChara);

    #endregion

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

        internal static readonly Dictionary<Job, int> Job100Prio = new()
        {
            { Job.SAM, 1 },
            { Job.PCT, 2 },
            { Job.RPR, 2 },
            { Job.VPR, 2 },
            { Job.MNK, 2 },
            { Job.NIN, 2 },
            { Job.DRG, 3 },
            { Job.BLM, 3 },
            { Job.RDM, 4 },
            { Job.SMN, 5 },
            { Job.MCH, 6 },
            { Job.BRD, 7 },
            { Job.DNC, 8 },
        };

        internal static readonly Dictionary<Job, int> Job070Prio = new()
        {
            { Job.SMN, 1 },
            { Job.MNK, 2 },
            { Job.BLM, 3 },
            { Job.DRG, 4 },
            { Job.VPR, 4 },
            { Job.PCT, 4 },
            { Job.SAM, 4 },
            { Job.RPR, 4 },
            { Job.NIN, 4 },
            { Job.RDM, 4 },
            { Job.MCH, 4 },
            { Job.BRD, 5 },
            { Job.DNC, 6 },
        };

        internal static readonly Dictionary<Job, int> Job080Prio = new()
        {
            { Job.SAM, 1 },
            { Job.BLM, 2 },
            { Job.DRG, 3 },
            { Job.MNK, 3 },
            { Job.PCT, 4 },
            { Job.MCH, 5 },
            { Job.NIN, 6 },
            { Job.RDM, 6 },
            { Job.RPR, 6 },
            { Job.VPR, 6 },
            { Job.SMN, 6 },
            { Job.BRD, 7 },
            { Job.DNC, 8 },
        };

        internal static readonly Dictionary<Job, int> Job090Prio = new()
        {
            { Job.SAM, 1 },
            { Job.PCT, 1 },
            { Job.BLM, 2 },
            { Job.MNK, 3 },
            { Job.VPR, 3 },
            { Job.DRG, 3 },
            { Job.MCH, 4 },
            { Job.RPR, 4 },
            { Job.NIN, 4 },
            { Job.SMN, 5 },
            { Job.RDM, 5 },
            { Job.BRD, 6 },
            { Job.DNC, 7 },
        };

        internal static readonly Restrictions[] RestrictionSteps =
        [
            // Ailment-free DPS
            Restrictions.Melee | Restrictions.NotDD | Restrictions.NotSick,
            Restrictions.DPS | Restrictions.NotDD | Restrictions.NotSick,
            // Sickness-free DPS
            Restrictions.Melee | Restrictions.NotSick,
            Restrictions.DPS | Restrictions.NotSick,
            // Sick DPS
            Restrictions.Melee | Restrictions.NotBrink,
            Restrictions.DPS | Restrictions.NotBrink,
            // Ailment-free
            Restrictions.NotDD | Restrictions.NotSick,
            // Sickness-free
            Restrictions.NotSick,
            // Sick
            Restrictions.NotBrink,
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
            Melee = 1 << 0, // 1
            DPS = 1 << 1, // 2
            NotDD = 1 << 2, // 4
            NotSick = 1 << 3, // 8
            NotBrink = 1 << 4, // 16
            ScrapeTheBottom = 1 << 5, // 32
        }
    }

    #endregion

    #endregion

    #region Custom Dance Step Logic

    /// <summary>
    ///     Consolidating a few checks to reduce duplicate code.
    /// </summary>
    private static bool WantsCustomStepsOnSmallerFeatures =>
        IsEnabled(Preset.DNC_CustomDanceSteps) &&
        IsEnabled(Preset.DNC_CustomDanceSteps_Conflicts) &&
        Gauge.IsDancing;

    /// <summary>
    ///     Saved custom dance steps.
    /// </summary>
    /// <seealso cref="DNC_CustomDanceSteps.Invoke">CustomDanceSteps</seealso>
    private static uint[] CustomDanceStepActions =>
        Service.Configuration.DancerDanceCompatActionIDs;

    /// <summary>
    ///     Checks if the action is a custom dance step and replaces it with the
    ///     appropriate step if so.
    /// </summary>
    /// <param name="action">The action ID to check.</param>
    /// <param name="updatedAction">
    ///     The matching dance step the action was assigned to.<br />
    ///     Will be Cease if used and was not a custom dance step.<br />
    ///     Do not use this value if the return is <c>false</c>.
    /// </param>
    /// <returns>If the action was assigned as a custom dance step.</returns>
    private static bool GetCustomDanceStep(uint action, out uint updatedAction)
    {
        updatedAction = All.Cease;

        if (!CustomDanceStepActions.Contains(action))
            return false;

        for (var i = 0; i < CustomDanceStepActions.Length; i++)
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
                _ => updatedAction,
            };
        }

        return true;
    }

    #endregion

    #region Openers

    #region Standard Openers

    /// <summary>
    /// Base class for DNC openers containing common properties and logic shared across all DNC opener variants.
    /// </summary>
    internal abstract class DNCOpenerBase : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        public override Preset Preset => Preset.DNC_ST_BalanceOpener;
        internal override UserData? ContentCheckConfig => DNC_ST_OpenerDifficulty;
        internal override bool IncludePot => DNC_Opener_Potion;
        internal static uint ReverseCascadeSteps => Gauge.Esprit >= 80 ? SaberDance : ActionReady(StarfallDance) ? StarfallDance : Gauge.Esprit >= 50 ? SaberDance : ActionReady(LastDance) ? LastDance : ActionReady(Fountainfall) ? Fountainfall : ReverseCascade;

        /// <summary>
        /// Template method for HasCooldowns validation. Performs common checks then delegates to ValidateCountdown() for opener-specific validation.
        /// </summary>
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

            return true;
        }
    }

    internal static FifteenSecondOpener Opener15S = new();

    internal class FifteenSecondOpener : DNCOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => StandardStep, // 2
            () => Gauge.NextStep, // 3
            () => Gauge.NextStep, // 4
            () => Peloton, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)), // 6
            () => StandardFinish2, // 7
            () => TechnicalStep, // 8
            () => Gauge.NextStep, // 9
            () => Gauge.NextStep, // 10
            () => Gauge.NextStep, // 11
            () => Gauge.NextStep, // 12
            () => TechnicalFinish4, // 13
            () => Devilment, // 14
            () => Tillana, // 15
            () => Flourish, // 16
            () => DanceOfTheDawn, // 17
            () => FanDance4, // 18
            () => LastDance, // 19
            () => FanDance3, // 20
            () => FinishingMove, // 21
            () => StarfallDance, // 22
            () => ReverseCascadeSteps, // 23
            () => ReverseCascadeSteps, // 24
            () => ReverseCascadeSteps, // 25
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 15)),
            ([5], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 5)),
            ([6], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 1)),
            ([7], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining)),
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat() || !DNC_Opener_PrepullBlock),
            ([4], () => !DNC_ST_OpenerOption_Peloton),
        ];
    }

    internal static SevenSecondOpener Opener07S = new();

    internal class SevenSecondOpener : DNCOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => StandardStep, // 2
            () => Gauge.NextStep, // 3
            () => Gauge.NextStep, // 4
            () => Peloton, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)), // 6
            () => StandardFinish2, // 7
            () => TechnicalStep, // 8
            () => Gauge.NextStep, // 9
            () => Gauge.NextStep, // 10
            () => Gauge.NextStep, // 11
            () => Gauge.NextStep, // 12
            () => TechnicalFinish4, // 13
            () => Devilment, // 14
            () => Tillana, // 15
            () => Flourish, // 16
            () => DanceOfTheDawn, // 17
            () => FanDance4, // 18
            () => LastDance, // 19
            () => FanDance3, // 20
            () => StarfallDance, // 21
            () => ReverseCascadeSteps, // 22
            () => ReverseCascadeSteps, // 23
            () => FinishingMove, // 24
            () => ReverseCascadeSteps, // 25
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 7)),
            ([5], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 3)),
            ([6], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 1)),
            ([7], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat() || !DNC_Opener_PrepullBlock),
            ([5], () => !DNC_ST_OpenerOption_Peloton),
        ];
    }

    #endregion

    #region Technical Openers

    internal static ThirtySecondTechOpener Opener30STech = new();

    internal class ThirtySecondTechOpener : DNCOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => StandardStep, // 2
            () => Gauge.NextStep, // 3
            () => Gauge.NextStep, // 4
            () => StandardFinish2, // 5
            () => Peloton, // 6
            () => TechnicalStep, // 7
            () => Gauge.NextStep, // 8
            () => Gauge.NextStep, // 9
            () => Gauge.NextStep, // 10
            () => Gauge.NextStep, // 11
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)), // 12
            () => TechnicalFinish4, // 13
            () => Devilment, // 14
            () => LastDance, // 15
            () => Flourish, // 16
            () => FinishingMove, // 17
            () => Tillana, // 18
            () => DanceOfTheDawn, // 19
            () => FanDance4, // 20
            () => StarfallDance, // 21
            () => FanDance3, // 22
            () => ReverseCascadeSteps, // 23
            () => ReverseCascadeSteps, // 24
            () => ReverseCascadeSteps, // 25
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 30)),
            ([5], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 15)),
            ([6], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 13)),
            ([7], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 7)),
            ([12], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 1)),
            ([13], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining)),
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat() || !DNC_Opener_PrepullBlock),
            ([5], () => !DNC_ST_OpenerOption_Peloton),
        ];
    }

    internal static SevenPlusSecondTechOpener Opener07PlusSTech = new();

    internal class SevenPlusSecondTechOpener : DNCOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => TechnicalStep, // 2
            () => Gauge.NextStep, // 3
            () => Gauge.NextStep, // 4
            () => Gauge.NextStep, // 5
            () => Gauge.NextStep, // 6
            () => Peloton, // 7
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)), // 8
            () => TechnicalFinish4, // 9
            () => Devilment, // 10
            () => LastDance, // 11
            () => Flourish, // 12
            () => FinishingMove, // 13
            () => Tillana, // 14
            () => DanceOfTheDawn, // 15
            () => FanDance4, // 16
            () => StarfallDance, // 17
            () => FanDance3, // 18
            () => ReverseCascadeSteps, // 19
            () => ReverseCascadeSteps, // 20
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 7)),
            ([7], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 2)),
            ([8], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 1)),
            ([9], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat() || !DNC_Opener_PrepullBlock),
            ([7], () => !DNC_ST_OpenerOption_Peloton),
        ];

        public override bool HasCooldowns()
        {
            if (!ActionReady(TechnicalStep))
                return false;

            if (!IsOffCooldown(Devilment))
                return false;

            if (InCombat())
                return false;

            return true;
        }
    }

    internal static SevenSecondTechOpener Opener07STech = new();

    internal class SevenSecondTechOpener : DNCOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => TechnicalStep, // 2
            () => Gauge.NextStep, // 3
            () => Gauge.NextStep, // 4
            () => Gauge.NextStep, // 5
            () => Gauge.NextStep, // 6
            () => Peloton, // 7
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)), // 8
            () => TechnicalFinish4, // 9
            () => Devilment, // 10
            () => Tillana, // 11
            () => Flourish, // 12
            () => FinishingMove, // 13
            () => DanceOfTheDawn, // 14
            () => FanDance4, // 15
            () => StarfallDance, // 16
            () => FanDance3, // 17
            () => ReverseCascadeSteps, // 18
            () => ReverseCascadeSteps, // 19
            () => ReverseCascadeSteps, // 20

        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 7)),
            ([7], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 2)),
            ([8], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 1)),
            ([9], () => !DNC_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat() || !DNC_Opener_PrepullBlock),
            ([7], () => !DNC_ST_OpenerOption_Peloton),
        ];
    }

    #endregion

    #endregion

    #region IDs

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


