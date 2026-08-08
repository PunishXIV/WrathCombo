#region

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Linq;
using Dalamud.Game.ClientState.Statuses;
using ECommons.Logging;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Extensions;

public static class GameObjectExtensions
{
    extension(IGameObject? obj)
    {

        /// <summary>
        ///     Gets the combat role of the object (Tank, Healer, DPS, or NonCombat).
        /// </summary>
        /// <remarks>
        ///     Returns <see cref="CombatRole.NonCombat"/> if the object is not a character or has no class job assigned.
        /// </remarks>
        public CombatRole Role
        {
            get
            {
                if (obj is not ICharacter chara ||
                    chara.ClassJob.ValueNullable is null)
                    return CombatRole.NonCombat;

                return chara.GetRole();
            }
        }

        /// <summary>
        ///     Gets the current HP percentage of the object (0-100).
        /// </summary>
        /// <remarks>
        ///     Returns <see cref="float.NaN"/> if the object is not a battle character or if HP cannot be determined.
        /// </remarks>
        public float HPP
        {
            get {
                if (obj is not IBattleChara battleChara)
                    return float.NaN;

                return (float)battleChara.CurrentHp / battleChara.MaxHp * 100f;
            }
        }

        /// <summary>
        ///     Checks if the object is dead, and should be raised.<br />
        ///     Checks for them being dead but targetable, not having Transcendence or
        ///     a Raise, and having been dead for more than 2 seconds.
        /// </summary>
        private bool IsDeadEnoughToRaise()
        {
            return obj.IsDead &&
                   obj.IsAPlayer() &&
                   !HasStatusEffect(2648, obj, true) && // just rezzed
                   !HasStatusEffect(148, obj, true) && // pending rezz
                   obj.IsTargetable &&
                   TimeSpentDead(obj.GameObjectId).TotalSeconds > 2;
        }

        #region Target Classification

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not friendly.
        /// </summary>
        /// <remarks>
        ///     See <see cref="SimpleTarget.Stack.AllyToHeal" /> for a use case.
        /// </remarks>
        public IGameObject? IfFriendly() =>
            obj != null && TargetIsFriendly(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not in the player's party.
        /// </summary>
        public IGameObject? IfInParty() =>
            obj != null &&
            GetPartyMembers()
                .Any(x => x.GameObjectId == obj.GameObjectId)
                ? obj
                : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not hostile.
        /// </summary>
        public IGameObject? IfHostile() =>
            obj != null && obj.IsHostile() ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not a boss.
        /// </summary>
        public IGameObject? IfBoss() =>
            obj != null && TargetIsBoss(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not a quest mob
        /// </summary>
        public IGameObject? IfQuestMob() =>
            obj != null && CustomComboFunctions.IsQuestMob(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target does not need positionals.
        /// </summary>
        public IGameObject? IfNeedsPositionals() =>
            obj != null && TargetNeedsPositionals(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is the player.
        /// </summary>
        public IGameObject? IfNotThePlayer() =>
            obj != null && obj.GameObjectId != Player.Object.GameObjectId
                ? obj
                : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not within range.
        /// </summary>
        /// <param name="range">The range to check against. Defaults to 25 yalms.</param>
        public IGameObject? IfWithinRange
            (float range = 25) =>
            obj != null && IsInRange(obj, range) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not within range.
        /// </summary>
        /// <param name="range">The range to check against. Defaults to 25 yalms.</param>
        public IGameObject? IfWithinActionRange
            (uint actionId) =>
            obj != null && InActionRange(actionId, obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is within action range.
        /// </summary>
        /// <param name="actionId">The action ID to check range for.</param>
        public bool IsWithinActionRange
            (uint actionId) =>
            obj != null && InActionRange(actionId, obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not within line of sight.
        /// </summary>
        public IGameObject? IfWithinLineOfSight
            () =>
            obj != null && IsInLineOfSight(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is within line of sight.
        /// </summary>
        public bool IsWithinLineOfSight
            () =>
            obj != null && IsInLineOfSight(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not in sight, in range, or cannot be affected by the action.
        /// </summary>
        /// <remarks>
        ///     This is a convenience method that combines checks for line of sight, action range, and action usability.
        /// </remarks>
        /// <param name="actionId">The action ID to check against the target.</param>
        public IGameObject? IfInSightInRangeCanUseOn
            (uint actionId) =>
            obj != null && obj.CanUseOn(actionId) && obj.IsWithinActionRange(actionId) && obj.IsWithinLineOfSight() ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not below 99% HP.
        /// </summary>
        public IGameObject? IfMissingHP(float missingHpp = 99) =>
            obj is IBattleChara battle &&
            GetTargetHPPercent(battle) <= missingHpp
                ? obj
                : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not invulnerable/invincible.
        /// </summary>
        public IGameObject? IfNotInvincible() =>
            obj != null && !TargetIsInvincible(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target does not have a cleansable
        ///     debuff.
        /// </summary>
        public IGameObject? IfHasCleansable() =>
            obj != null && HasCleansableDebuff(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is dead.
        /// </summary>
        public IGameObject? IfAlive() =>
            obj != null && !obj.IsDead ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not dead enough.
        /// </summary>
        /// <seealso cref="IsDeadEnoughToRaise" />
        public IGameObject? IfDead() =>
            obj != null && IsDeadEnoughToRaise(obj) ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not targetable.
        /// </summary>
        public IGameObject? IfTargetable() =>
            obj != null && obj.IsTargetable ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not a real player.
        /// </summary>
        public IGameObject? IfAPlayer() =>
            obj != null && obj is IPlayerCharacter ? obj : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target cannot be affected by the action.
        /// </summary>
        public unsafe IGameObject? IfCanUseOn(uint actionId) =>
            obj != null && ActionManager.CanUseActionOnTarget(actionId, obj.Struct())
                ? obj
                : null;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not in combat.
        /// </summary>
        public unsafe IGameObject? IfInCombat() =>
            obj != null && obj is IBattleChara c && c.Struct()->InCombat
                ? obj
                : null;

        #endregion

        #region Target Checking (same as above, but returns a boolean)

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is friendly.
        /// </summary>
        public bool IsFriendly() =>
            obj != null && TargetIsFriendly(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is in the player's party.
        /// </summary>
        public bool IsInParty() =>
            obj != null &&
            GetPartyMembers()
                .Any(x => x.GameObjectId == obj.GameObjectId);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is a boss.
        /// </summary>
        public bool IsBoss() =>
            obj != null && TargetIsBoss(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is a quest mob.
        /// </summary>
        public bool IsQuestMob() =>
            obj != null && CustomComboFunctions.IsQuestMob(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target needs positionals.
        /// </summary>
        public bool NeedsPositionals() =>
            obj != null && TargetNeedsPositionals(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object is the player.
        /// </summary>
        public bool IsNotThePlayer() =>
            obj != null && obj.GameObjectId != Player.Object.GameObjectId;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is within range.
        /// </summary>
        public bool IsWithinRange(float range = 25) =>
            obj != null && IsInRange(obj, range);
        
        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is outside of range.
        /// </summary>
        public bool IsAtLeastFiveYalmsAway(float range = 5) =>
            obj != null && !IsInRange(obj, range);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the target is below 99% HP.
        /// </summary>
        public bool IsMissingHP() =>
            obj is IBattleChara battle && battle.CurrentHp / battle.MaxHp * 100 < 99;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object is not invulnerable/invincible.
        /// </summary>
        public bool IsNotInvincible() =>
            obj != null && !TargetIsInvincible(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object has a cleansable debuff.
        /// </summary>
        public bool IsCleansable() =>
            obj != null && HasCleansableDebuff(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object is dead enough.
        /// </summary>
        /// <seealso cref="IsDeadEnoughToRaise" />
        public bool IsDead() =>
            obj != null && IsDeadEnoughToRaise(obj);

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object is a player.
        /// </summary>
        public bool IsAPlayer() =>
            obj != null && obj is IPlayerCharacter;

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object can be affected by the action.
        /// </summary>
        public unsafe bool CanUseOn(uint actionId) =>
            obj != null &&
            ActionManager.CanUseActionOnTarget(actionId, obj.Struct());

        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it a quick
        ///     boolean check for if the object is in combat.
        /// </summary>
        public unsafe bool IsInCombat() =>
            obj != null && obj is IBattleChara c && c.Struct()->InCombat;

        // `IsHostile` already exists, and works the exact same as we would write here

        #endregion

        #region Safe Access to Members
        
        /// <summary>
        ///     Safely gets the GameObjectId of the object, handling cases where the object may have been deleted.
        /// </summary>
        /// <remarks>
        ///     This property performs safety checks to ensure the object still exists before attempting to access its ID.
        ///     Returns <see langword="null"/> if the object is no longer valid, returns null ID, or if an exception occurs.
        /// </remarks>
        public ulong? SafeGameObjectId
        {
            get
            {
                try
                {
                    var safeObj = obj.Address.GetObject();
                    if (safeObj is null)
                    {
                        PluginLog.Verbose("[ObjectSafety] Would have failed " +
                                          "accessing any member, object gone");
                        return null;
                    }

                    var id = safeObj.GameObjectId;
                    if (id is 0)
                        return null;

                    return id;
                }
                catch
                {
                    PluginLog.Verbose("[ObjectSafety] Would have failed " +
                                      "doing SafeGameObjectId");
                }
                return null;
            }
        }

        /// <summary>
        ///     Safely gets the status list of the object, handling cases where the object may have been deleted or is not a battle character.
        /// </summary>
        /// <remarks>
        ///     This property performs safety checks to ensure the object still exists and is a battle character before attempting to access its status list.
        ///     Returns <see langword="null"/> if the object is no longer valid, is not a battle character, or if an exception occurs.
        /// </remarks>
        public StatusList? SafeStatusList
        {
            get
            {
                try
                {
                    var safeObj = obj.Address.GetObject();
                    if (safeObj is null)
                    {
                        PluginLog.Verbose("[ObjectSafety] Would have failed " +
                                          "accessing any member, object gone");
                        return null;
                    }
                    if (safeObj is IBattleChara battleChara)
                        return battleChara.StatusList;
                }
                catch
                {
                    PluginLog.Verbose("[ObjectSafety] Would have failed " +
                                      "doing SafeStatusList");
                }
                return null;
            }
        }

        #endregion
    }

    #region Get Objects More Conveniently

    /// <summary>
    ///     Converts a GameObject pointer to an IGameObject from the object table.
    /// </summary>
    /// <param name="ptr">The GameObject pointer to convert.</param>
    /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
    public static unsafe IGameObject? GetObjectFrom(GameObject* ptr) =>
       ptr == null
           ? null
           : Svc.Objects
               .FirstOrDefault(x => x.Address == (IntPtr)ptr);

    /// <summary>
    ///     Converts a GameObjectID to an IGameObject from the object table.
    /// </summary>
    /// <param name="id">The GameObjectID to convert.</param>
    /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
    public static IGameObject? GetObjectFrom(ulong id) =>
        Svc.Objects.SearchById(id);

    /// <summary>
    ///     Converts a GameObjectID to an IGameObject from the object table.
    /// </summary>
    /// <param name="id">The GameObjectID to convert.</param>
    /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
    public static IGameObject? GetObject(this ulong id) =>
        id == 0xE0000000 ? Player.Object :
        GetObjectFrom(id);

    /// <summary>
    ///     Converts a GameObjectID(?) to an IGameObject from the object table.
    /// </summary>
    /// <param name="id">The GameObjectID to convert.</param>
    /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
    public static IGameObject? GetObject(this ulong? id) =>
        id == null ? null : GetObjectFrom((ulong)id);
    
    /// <summary>
    ///     Converts a GameObject pointer to an IGameObject from the object table.<br />
    ///     Primarily for safely accessing object members, since the address is
    ///     always set.
    /// </summary>
    /// <param name="address">The GameObject pointer to convert.</param>
    /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
    public static IGameObject? GetObject(this IntPtr address) =>
        address != IntPtr.Zero ? Svc.Objects.FirstOrDefault(x => x.Address == address) : null;

    #endregion
}