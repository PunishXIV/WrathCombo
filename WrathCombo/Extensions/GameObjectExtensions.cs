using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Extensions;

public static class GameObjectExtensions
{
    /// <summary>
    ///     Converts a GameObject pointer to an IGameObject from the object table.
    /// </summary>
    /// <param name="ptr">The GameObject pointer to convert.</param>
    /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
    public static unsafe IGameObject? GetObjectFrom(GameObject* ptr) =>
        ptr == null ? null : Svc.Objects
            .FirstOrDefault(x => x.Address == (IntPtr)ptr);

    #region Target Classification

    /// <summary>
    ///     Can be chained onto a <see cref="IGameObject" /> to make it return
    ///     <see langword="null" /> if the target is not friendly.
    /// </summary>
    /// <remarks>
    ///     See <see cref="SimpleTarget.Stack.AllyToHeal"/> for a use case.
    /// </remarks>
    public static IGameObject? IfFriendly (this IGameObject? obj) =>
        obj != null && CustomComboFunctions.TargetIsFriendly(obj) ? obj : null;

    /// <summary>
    ///     Can be chained onto a <see cref="IGameObject" /> to make it return
    ///     <see langword="null" /> if the target is not hostile.
    /// </summary>
    public static IGameObject? IfHostile (this IGameObject? obj) =>
        obj != null && obj.IsHostile() ? obj : null;

    /// <summary>
    ///     Can be chained onto a <see cref="IGameObject" /> to make it return
    ///     <see langword="null" /> if the target is not a boss.
    /// </summary>
    public static IGameObject? IfBoss (this IGameObject? obj) =>
        obj != null && CustomComboFunctions.IsBoss(obj) ? obj : null;

    /// <summary>
    ///     Can be chained onto a <see cref="IGameObject" /> to make it return
    ///     <see langword="null" /> if the target is not a quest mob
    /// </summary>
    public static IGameObject? IfQuestMob (this IGameObject? obj) =>
        obj != null && CustomComboFunctions.IsQuestMob(obj) ? obj : null;

    /// <summary>
    ///     Can be chained onto a <see cref="IGameObject" /> to make it return
    ///     <see langword="null" /> if the target does not need positionals.
    /// </summary>
    public static IGameObject? IfNeedsPositionals (this IGameObject? obj) =>
        obj != null && CustomComboFunctions.TargetNeedsPositionals(obj) ? obj : null;

    /// <summary>
    ///     Can be chained onto a <see cref="IGameObject" /> to make it return
    ///     <see langword="null" /> if the target is not within range.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="range">The range to check against. Defaults to 25 yalms.</param>
    public static IGameObject? IfWithinRange
        (this IGameObject? obj, float range = 25) =>
    obj != null && CustomComboFunctions.IsInRange(obj, range) ? obj : null;

    #endregion
}
