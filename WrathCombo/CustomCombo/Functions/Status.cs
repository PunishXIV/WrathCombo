using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Linq;
using WrathCombo.Data.BattleData;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary>
    /// Retrieves a Status object that is on the Player or specified Target, null if not found
    /// </summary>
    /// <param name="statusId">Status Effect ID</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <param name="target">Optional target</param>
    /// <returns>Status object or null.</returns>
    public static IStatus? GetStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)
    {
        // Default to LocalPlayer if no target/bad target
        target ??= LocalPlayer;

        // Use LocalPlayer's GameObjectId if playerOwned, null otherwise
        ulong? sourceId = !anyOwner ? LocalPlayer.GameObjectId : null;

        return Service.ComboCache.GetStatus(statusId, target, sourceId);
    }

    /// <summary>
    /// Checks to see if a status is on the Player or an optional target
    /// </summary>
    /// <param name="statusId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <returns>Boolean if the status effect exists or not</returns>
    public static bool HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)
    {
        // Default to LocalPlayer if no target provided
        target ??= LocalPlayer;
        return GetStatusEffect(statusId, target, anyOwner) is not null;
    }

    /// <summary>
    /// Checks to see if a status is on the Player or an optional target, and supplies the Status as well
    /// </summary>
    /// <param name="statusId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <param name="status">Retrieved Status object</param>
    /// <returns>Boolean if the status effect exists or not</returns>
    public static bool HasStatusEffect(ushort statusId, out IStatus? status, IGameObject? target = null, bool anyOwner = false)
    {
        target ??= LocalPlayer;
        status = GetStatusEffect(statusId, target, anyOwner);
        return status is not null;
    }

    /// <summary>
    /// Checks to see if any statuses are on the Player or an optional target
    /// </summary>
    /// <param name="statusId">List Of StatusIDs</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the statuses, true means anyone owns</param>
    /// <seealso cref="HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)"/>
    public static bool HasAnyStatusEffects(ushort[] status, IGameObject? target = null, bool anyOwner = false) =>
        status.Any(statusId => HasStatusEffect(statusId, target ?? LocalPlayer, anyOwner));

    /// <summary>
    /// Checks to see if all statuses are on the Player or an optional target
    /// </summary>
    /// <param name="statusId">List Of StatusIDs</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the statuses, true means anyone owns</param>
    /// <seealso cref="HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)"/>
    public static bool HasAllStatusEffects(ushort[] status, IGameObject? target = null, bool anyOwner = false) =>
        status.All(statusId => HasStatusEffect(statusId, target ?? LocalPlayer, anyOwner));

    /// <summary>
    /// Gets remaining time of a Status Effect
    /// </summary>
    /// <param name="effect">Dalamud Status object</param>
    /// <returns>Float representing remaining status effect time</returns>
    public unsafe static float GetStatusEffectRemainingTime(IStatus? effect)
    {
        if (effect is null) return 0;
        if (effect.RemainingTime < 0) return (effect.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
        return effect.RemainingTime;
    }

    /// <summary>
    /// Retrieves remaining time of a Status Effect on the Player or Optional Target
    /// </summary>
    /// <param name="effectId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <returns>Float representing remaining status effect time</returns>
    public unsafe static float GetStatusEffectRemainingTime(ushort effectId, IGameObject? target = null, bool anyOwner = false) =>
        GetStatusEffectRemainingTime(GetStatusEffect(effectId, target, anyOwner));

    /// <summary>
    ///     Same as <see cref="GetStatusEffectRemainingTime(ushort, IGameObject?, bool)"/>,
    ///     but returns NaN if the status effect is not found, failing
    ///     any comparisons.<br/>
    ///     As in: It will not return <c>0</c>, and pass less than checks.
    /// </summary>
    /// <seealso cref="GetStatusEffectRemainingTime(ushort, IGameObject?, bool)"/>
    public static float GetPossessedStatusRemainingTime
    (ushort effectId, IGameObject? target = null, bool anyOwner = false) =>
    HasStatusEffect(effectId, out var status, target, anyOwner)
        ? GetStatusEffectRemainingTime(status)
        : float.NaN;

    /// <summary>
    /// Retrieves remaining time of a Status Effect
    /// </summary>
    /// <param name="effect">Dalamud Status object</param>
    /// <returns>Integer representing status effect stack count</returns>
    public static ushort GetStatusEffectStacks(IStatus? effect) => effect?.Param ?? 0;

    /// <summary>
    /// Retrieves the status effect stack count
    /// </summary>
    /// <param name="effectId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <returns>Integer representing status effect stack count</returns>
    public static ushort GetStatusEffectStacks(ushort effectId, IGameObject? target = null, bool anyOwner = false) =>
        GetStatusEffectStacks(GetStatusEffect(effectId, target, anyOwner));


    /// <summary> Returns the name of a status effect from its ID. </summary>
    /// <param name="id"> ID of the status. </param>
    /// <returns></returns>
    public static string GetStatusName(uint id) => StatusCache.GetStatusName(id);

    public static bool TargetHasDamageDown(IGameObject? target) => StatusCache.HasDamageDown(target);

    public static bool TargetHasDamageUp(IGameObject? target) => StatusCache.HasDamageUp(target);

    public static bool TargetHasRezWeakness(IGameObject? target, bool checkForWeakness = true)
    {
        if (checkForWeakness && HasStatusEffect(43, target, true)) //Weakness = 43
            return true;

        return HasStatusEffect(44, target, true); //Brink of Death = 44
    }

    /// <summary>
    /// Checks if the target has a debuff that can be dispelled.
    /// </summary>
    /// <param name="target">The game object to check. Defaults to the current target if null.</param>
    /// <returns>True if the target has a cleansable debuff; otherwise, false.</returns>
    public static bool HasCleansableDebuff(IGameObject? target) => StatusCache.HasCleansableDebuff(target);

    /// <summary>
    /// Checks if the target has a beneficial status.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool HasBeneficialStatus(IGameObject? target) => StatusCache.HasBeneficialStatus(target);

    public static bool HasPhantomDispelStatus(IGameObject? target) => StatusCache.HasDamageUp(target) || StatusCache.HasEvasionUp(target) || HasStatusEffect(4355, target) || TargetIsInvincible(target);

    /// <summary>
    /// Checks to see if the player has a status that should stop all actions and unselect targets
    /// Acceleration bombs and Pyretics
    /// </summary>
    public static unsafe bool PlayerHasActionPenalty(bool fromAutorot)
    {
        bool hasActionPenalty = false;

        // Quick Content Check First
        hasActionPenalty = BattleData.PauseActions();
        if (!hasActionPenalty)
        {
            float userSetting = fromAutorot ? 1.5f : Service.Configuration.PenaltyPause;
            hasActionPenalty =
                Player.Status.Any(s =>
                    // Acceleration Bomb within Timeframe
                    (StatusCache.PausingStatuses.AccelerationBombs.Contains(s.StatusId) ||

                    // Pyretic
                    StatusCache.PausingStatuses.Pyretics.Contains(s.StatusId) ||

                    // Others
                    StatusCache.PausingStatuses.Misc.Contains(s.StatusId))

                    && GetStatusEffectRemainingTime(s) <= userSetting

                ) == true;
        }

        if (hasActionPenalty)
        {
            Svc.Targets.Target = null;
            //OverrideTarget = null;
            UIState.Instance()->Hotbar.CancelCast();
        }

        return hasActionPenalty;
    }

    /// <summary>
    /// Checks if the target is invincible due to status effects or encounter-specific mechanics.
    /// </summary>
    /// <param name="target">The game object to check.</param>
    /// <returns>True if the target is invincible; otherwise, false.</returns>
    public static bool TargetIsInvincible(IGameObject? target)
    {
        if (target is not IBattleChara tar)
            return false;

        if (tar.SafeStatusList is not { } statuses)
            return false;

        // Turn Target's status to uint hashset
        var targetStatuses = statuses.Select(s => s.StatusId).ToHashSet();
        uint targetID = tar.BaseId;

        // Returning False in each case because there should be no other General Invincibility Check needed
        // for specified areas

        return BattleData.IsInvincible(tar, targetID, targetStatuses) switch
        {
            // If target is invincible based on Battle Data
            BattleData.Invincible.True => true,
            // Are we to bother with checking statuses per Battle Data
            BattleData.Invincible.False => false,
            // General invincibility check
            // Due to large size of InvincibleStatuses, best to check process this way
            BattleData.Invincible.CheckStatuses => StatusCache.CompareLists(
                                StatusCache.InvincibleStatuses,
                                targetStatuses),
            _ => false,
        };
    }

    /// <summary>
    /// Checks if a target has the max number of entries in their status list.
    /// <para>30 for players, 60 for NPCs.</para>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static unsafe bool TargetIsStatusCapped(IGameObject? target)
    {
        try
        {
            target ??= LocalPlayer;
            if (target is IBattleChara bc)
                return bc.StatusList.Count(x => x.StatusId != 0) ==
                       bc.Struct()->StatusManager.NumValidStatuses;
        }
        // Catch issues with:
        // - Getting the StatusList from suddenly-stale GameObjects
        // - Getting the number of valid statuses from scuffed NPCs
        catch
        {
            // Ignored, assume false
        }

        return false;
    }

    /// <summary>
    /// Checks if the target has any remaining entries in the status list to be able to add a new status, or if the status is already on them from the player.
    /// <para>Does not actually validate status logic i.e player buffs on enemies isn't checked.</para>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statusId"></param>
    /// <returns></returns>
    public static bool CanApplyStatus(IGameObject? target, ushort statusId)
    {
        target ??= LocalPlayer;
        if (target is null)
            return false;

        //Check to see if it's a buff or debuff and therefore if the target is suitable for the status
        var status = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>().GetRow(statusId);
        if ((target.IsHostile() && status.StatusCategory != 2) || (target.IsFriendly() && status.StatusCategory != 1))
            return false;

        if (!TargetIsStatusCapped(target) || HasStatusEffect(statusId, target))
            return true;

        return false;
    }

    /// <summary>
    ///     Overload to accept a list of status IDs.
    /// </summary>
    /// <seealso cref="CanApplyStatus(IGameObject?,ushort)"/>
    public static bool CanApplyStatus(IGameObject? target, ushort[] status) =>
        status.Any(statusId => CanApplyStatus(target, statusId));

    public static bool HasCleansableDoom(IGameObject? target = null)
    {
        target ??= CurrentTarget;
        target ??= LocalPlayer;

        if (target is not IBattleChara { } chara)
            return false;

        return StatusCache.HasCleansableDoom(target);
    }

}