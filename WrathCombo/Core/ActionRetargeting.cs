#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Logging;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using WrathCombo.Services;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

#endregion

namespace WrathCombo.Core;

/// <summary>
///     Action Retargeting capabilities, to remove any dependence on Redirect or
///     Reaction.<br /><br />
///     See
///     <see cref="UIntExtensions.Retarget(uint,IGameObject?)">
///         Retarget(uint,IGameObject)
///     </see>
///     for simple Feature Retargeting.
///     <br />
///     See
///     <see cref="UIntExtensions.Retarget(uint,uint,Func{IGameObject?})">
///         Retarget(uint,uint,TargetResolverDelegate)
///     </see>
///     for more advanced Retargeting in a full combo.
///     <br />
///     See <see cref="TargetResolverAttribute" /> for an example on setting up more
///     advanced Retargeting.
/// </summary>
public class ActionRetargeting : IDisposable
{
    /// List of Retargets for actions, keyed with the action and replaced action(s).
    internal readonly Dictionary<uint, Retargeting> Retargets = [];

    public ActionRetargeting()
    {
        Configuration.ConfigChanged += ClearCachedRetargetsOnConfigChange;
    }

    /// <summary>
    ///     Register an action and its replaced actions as one you want Retargeted.
    /// </summary>
    /// <param name="action">The Action to retarget.</param>
    /// <param name="replacedActions">The Actions the combo is replacing</param>
    /// <param name="resolver">
    ///     The <see cref="TargetResolverAttribute">TargetResolver</see> to resolve
    ///     the target.<br />
    ///     Examples:
    ///     a <see cref="SimpleTarget">SimpleTarget</see> (like
    ///     <see cref="SimpleTarget.HardTarget">HardTarget</see>)
    ///     or
    ///     a <see cref="TargetResolverAttribute">custom method</see> (like:
    ///     <see cref="AST.CardResolver">AST.CardsResolver</see>)
    /// </param>
    /// <returns>
    ///     The <paramref name="action" /> that was registered.<br />
    ///     This only really returns to make
    ///     <see cref="UIntExtensions.Retarget(uint,Func{IGameObject?})">
    ///         (uint).Retarget()
    ///     </see>
    ///     simpler.
    /// </returns>
    /// <remarks>
    ///     Should only be called by
    ///     <see cref="UIntExtensions.Retarget(uint,Func{IGameObject?})">
    ///         (uint).Retarget()
    ///     </see>
    ///     .
    /// </remarks>
    internal uint Register
    (uint action,
        uint[] replacedActions,
        Func<IGameObject?> resolver)
    {
        // Make sure the action is not in replaced actions,
        // and there are no duplicates
        replacedActions = replacedActions.Distinct().ToArray();

        // Build the Retarget object
        var retarget = new Retargeting(action, replacedActions, resolver);

        //// Limit spam from the same actionID, mostly for debugging
        //if (!EZ.Throttle($"retargetFor{retarget.ID}", TS.FromSeconds(1)))
        //    return action;

        #region Replace existing Retargets

        var          partialOverwrite = false;
        string[]     overwriting      = [];
        Retargeting? oldRetarget      = null;
        foreach (var replacedAction in replacedActions.Concat([action]))
        {
            if (!Retargets.TryGetValue(replacedAction, out oldRetarget))
                continue;

            // Flag as a partial overwrite if `actionInReplaced` has different values
            if (replacedActions != oldRetarget.ReplacedActions)
                partialOverwrite = true;

            overwriting = [oldRetarget.ResolverName, retarget.ResolverName];
        }

        // Remove the old Retarget
        if (overwriting.Length != 0)
        {
            RemoveRetarget(oldRetarget?.ID ?? 0);

            // Elevate to a debug message if the overwriting method is not equivalent
            var logLevel = overwriting[0] == overwriting[1] ? 0 : 1;
#if DEBUG
            // Elevate to a warning if not equivalent and on a debug build
            logLevel = logLevel == 1 ? 2 : logLevel;
#endif
            var overwritingText = $"{overwriting[0]} -> {overwriting[1]}";
            if (!partialOverwrite)
                log("overwriting Retargeting for", logLevel: logLevel,
                    showAction: true,
                    retarget: retarget,
                    messageAfterAction: overwritingText);
        }

        #endregion

        // Save the Retarget
        var throttleTimespan = TS.FromSeconds(partialOverwrite ? 20 : 1);
        if (EZ.Throttle($"retargetFor{retarget.Action}", throttleTimespan))
            log("registering", showAction: true,
                messageAfterAction: "for Retargeting",
                showResolver: true, retarget: retarget);
        AddRetarget(retarget);
        return action;
    }

    /// <summary>
    ///     Resolve the target for an action via the provided
    ///     <see cref="TargetResolverAttribute">TargetResolver</see>.
    /// </summary>
    /// <param name="action">The Action or Replaced Action to Retarget.</param>
    /// <param name="target">
    ///     The output  <see cref="IGameObject">Game Object</see> of the target, if
    ///     the action was found to be Retargeted.
    /// </param>
    /// <param name="replacedWith"></param>
    /// <returns>
    ///     Whether the action is registered for Retargeting.
    /// </returns>
    public bool TryGetTargetFor(uint action, out IGameObject? target,
        out uint replacedWith)
    {
        target       = null;
        replacedWith = action;
        // Find the Retarget object
        if (!Retargets.TryGetValue(action, out var retarget) ||
            !Service.ActionReplacer.LastActionInvokeFor
                .TryGetValue(action, out var lastAction) ||
            lastAction != retarget.Action)
            return false;

        replacedWith = retarget.Action;
        log("Retargeting", showAction: true,
            showResolver: true, retarget: retarget);

        // Run the target resolver
        RemoveRetarget(retarget.ID);
        try
        {
            target = retarget.Resolver.Invoke();
        }
        catch (Exception ex)
        {
            log("error while resolving target for", logLevel: 3, showAction: true,
                showResolver: true, retarget: retarget,
                extraText: $":\n{ex}");
            return false;
        }

        // Return the results
        log("Retargeted", logLevel: 1, showAction: true,
            messageAfterAction: $"to '{target?.Name ?? "null"}'",
            showResolver: true, retarget: retarget);
        return target != null;
    }

    /// <summary>
    ///     A Retarget object, used to accurately store everything about a want to
    ///     Retarget an action.
    /// </summary>
    /// <param name="action">The action wanting to be Retargeted.</param>
    /// <param name="replacedActions">
    ///     The combo's replaced actions; what is actually being pressed on the hotbar
    ///     for the combo where the Retargeted action appears.
    /// </param>
    /// <param name="resolver">
    ///     The <see cref="TargetResolverAttribute">TargetResolver</see> that
    ///     resolves the target for the action.<br />
    ///     Can be a <see cref="SimpleTarget" />, but it gets wrapped in a delegate
    ///     in <see cref="UIntExtensions.Retarget(uint,IGameObject?)" />, or its
    ///     two overloads.
    /// </param>
    internal class Retargeting(
        uint action,
        uint[] replacedActions,
        Func<IGameObject?> resolver)
    {
        /// A unique identifier for the Retarget, to help with overwrites and removal.
        public int ID { get; } = HashCode.Combine(action, replacedActions);

        /// The action we actually want to Retarget.
        public uint Action { get; } = action;

        /// The actions that could be what is actually pressed when we want to Retarget the action.
        public uint[] ReplacedActions { get; } = replacedActions;

        /// The target resolver that will decide the new target for the action.
        public Func<IGameObject?> Resolver { get; } = resolver;

        /// The name of the resolver method, to help with debugging.
        public string ResolverName { get; } = GetMethodName(resolver);

        /// When this was created, to help with age-outs.
        public DateTime Created { get; } = DateTime.Now;

        /// <summary>
        ///     Gets the name for the resolver.
        /// </summary>
        /// <param name="resolver">The resolver we want the name of.</param>
        /// <returns>
        ///     The name of the resolver, whether determined from the class (for
        ///     <see cref="SimpleTarget">Simple Targets</see>) or the name of the
        ///     resolver itself.
        /// </returns>
        private static string GetMethodName(Func<IGameObject?> resolver)
        {
            var resolverName  = resolver.Method.Name;
            var resolverClass = resolver.Method.DeclaringType?.Name ?? "";

            // Standardize the names, and if a custom resolver is used,
            // try to provide the custom name if one was provided
            resolverName = resolverClass switch
            {
                _ when resolverClass.Contains("<>c") ||
                       resolverClass.Contains("__") =>
                    "<DirectGameObject>",
                _ => resolverName,
            };

            return $"`{resolverName}`";
        }
    }

    #region Utilities

    /// <summary>
    ///     Method to resolve a target for an action using custom logic.<br />
    ///     Should return <c>IGameObject?</c>.
    /// </summary>
    /// <returns>
    ///     The <see langword="null" />able
    ///     <see cref="IGameObject">Game Object</see> of the target.
    /// </returns>
    /// <example>
    ///     <code>
    ///     [ActionRetargeting.TargetResolver]
    ///     public static IGameObject? ExampleTargetResolver () {
    ///         if (Config.Example_ShouldUseFocusTarget)
    ///             return SimpleTarget.FocusTarget;
    ///         return SimpleTarget.Self;
    ///     };
    ///     </code>
    /// </example>
    /// <seealso cref="AST.CardResolver">
    ///     AST.CardsResolver
    /// </seealso>
    /// <seealso cref="DNC.DancePartnerResolver">
    ///     DNC.DancePartnerResolver
    /// </seealso>
    [AttributeUsage(AttributeTargets.Method)]
    public class TargetResolverAttribute : Attribute
    {
    }

    /// <summary>
    ///     Prevents the action itself from being retargeted unless excepted,
    ///     which stops retargets from leaking from combos as much.<br />
    ///     Should also add to
    ///     <see cref="CustomCombo._presetsAllowedToReturnUnchanged" /> if the
    ///     action is one that FF changes.
    /// </summary>
    private static readonly uint[] _actionsAllowedToBeReplacedAction =
    [
        // Features that only Retarget themselves
        DNC.ClosedPosition,
        AST.EssentialDignity,
        DRK.BlackestNight,
        DRK.Oblation,
        GNB.HeartOfStone,
        GNB.HeartOfCorundum,
        PLD.Clemency,

        // Resurrection abilities from the "All" feature
        WHM.Raise,
        AST.Ascend,
        SGE.Egeiro,
        SCH.Resurrection,
        BLU.AngelWhisper,
        RDM.Verraise,
        SMN.Resurrection,
    ];

    /// <summary>
    ///     Adds a new Retarget to both lists of Retargets.
    /// </summary>
    /// <param name="retarget">The Retarget to add.</param>
    private void AddRetarget(Retargeting retarget)
    {
        // Only retarget the action as well as the replaced actions if excepted
        if (_actionsAllowedToBeReplacedAction.Contains(retarget.Action))
            Retargets[retarget.Action] = retarget;

        foreach (var replacedAction in retarget.ReplacedActions)
            Retargets[replacedAction] = retarget;
    }

    /// <summary>
    ///     Removes all Retargets for an action.
    /// </summary>
    /// <param name="retargetID">The ID of the Retarget object to remove.</param>
    private void RemoveRetarget(int retargetID)
    {
        var retarget = Retargets.Values
            .FirstOrDefault(x => x.ID == retargetID);
        if (retarget == null)
            return;

        var actionsToRemove = retarget.ReplacedActions.Concat([retarget.Action]);
        foreach (var replacedAction in actionsToRemove)
            Retargets.Remove(replacedAction);
    }

    /// Simple Flag to stop clearing the cache when the plugin is disposed.
    // ReSharper disable once RedundantDefaultMemberInitializer
    private bool CancelCacheClearing { get; set; } = false;

    /// Clears Retargets older than 20 seconds every 11 seconds.
    /// <seealso cref="Retargets" />
    internal Action ClearOldRetargets = () =>
    {
        try
        {
            // This doesn't even need set at Dispose() technically,
            // since it becomes un-reference-able once the plugin is unloaded.
            // Either way, it's an easy check, for sure.
            if (P.ActionRetargeting.CancelCacheClearing)
                return;
        }
        // When unloading the plugin, just kill the task
        catch (NullReferenceException)
        {
            return;
        }

        // Find old Retargets that are allowed to be culled
        var oldRetargets = P.ActionRetargeting.Retargets.Values
            .Where(x => (DateTime.Now - x.Created) > TS.FromSeconds(20))
            .ToList();

        // Cull each unique Retarget
        if (oldRetargets.Count > 0)
            foreach (var old in oldRetargets)
            {
                P.ActionRetargeting.RemoveRetarget(old.ID);
                // Make sure not to spam if the same retarget is culled more than once
                if (!EZ.Throttle($"retargetClr{old.ID}", TS.FromSeconds(3)))
                    log("cleared old Retarget for", showAction: true, retarget: old);
            }

        Svc.Framework.RunOnTick(P.ActionRetargeting.ClearOldRetargets,
            TS.FromSeconds(11));
    };

    /// Clears all cached Retargets.
    internal void ClearCachedRetargets()
    {
        Retargets.Clear();
        log("cleared cached Retargets", logLevel: 1);
    }

    private void ClearCachedRetargetsOnConfigChange
        (object? _, Configuration.ConfigChangeEventArgs args)
    {
        if (args.Type == Configuration.ConfigChangeType.Preset)
            // todo: restrict this to only clear the retargets associated
            //  with the preset, once Retargeting is based on Presets.
            ClearCachedRetargets();
    }

    public void Dispose()
    {
        Configuration.ConfigChanged -= ClearCachedRetargetsOnConfigChange;
        Retargets.Clear();
        CancelCacheClearing = true;
    }

    private static void log(string message,
        string messageAfterAction = "",
        int logLevel = 0,
        bool showAction = false,
        bool showResolver = false,
        Retargeting? retarget = null,
        string extraText = "")
    {
        message = $"[ActionRetargeting] {message}";
        if (showAction)
            message += $" '{retarget?.Action.ActionName()}'";
        if (messageAfterAction != "")
            message += $" {messageAfterAction}";
        if (showResolver)
            message += $" (with {retarget?.ResolverName})";
        if (extraText != "")
            message += $"{extraText}";

        sendLog(message, logLevel);

        return;

        void sendLog(string msg, int ll)
        {
            switch (ll)
            {
                case 0:
                    PluginLog.Verbose(msg);
                    break;
                case 1:
                    PluginLog.Debug(msg);
                    break;
                case 2:
                    PluginLog.Warning(msg);
                    break;
                default:
                    PluginLog.Error(msg);
                    break;
            }
        }
    }

    #endregion
}

internal static class FuncIGameObjectExtensions
{
    /// <summary>
    ///     A simple extension to throw a warning if the target resolver does not
    ///     have the <see cref="ActionRetargeting.TargetResolverAttribute" />.
    /// </summary>
    public static Func<IGameObject?> CheckForAttribute
        (this Func<IGameObject?> target)
    {
        var hasTargetResolverAttr = target.Method
            .GetCustomAttributes(typeof(ActionRetargeting.TargetResolverAttribute),
                false)
            .Length > 0;
        if (!hasTargetResolverAttr &&
            EZ.Throttle("retargetAttributeWarning", TS.FromSeconds(15)))
            PluginLog.Warning(
                "[ActionRetargeting] Custom Target Resolver provided, " +
                "but lacks the `TargetResolver` Attribute.\n" +
                "StackTrace:\n" + Environment.StackTrace);

        return target;
    }
}

internal static class UIntExtensions
{
    // One for "no replaced action specified" (simple features),
    // "one replaced action specified" (simple combos),
    // and "multiple replaced actions specified" (complex combos, like healers),
    // each accepting a direct target or a target resolver.

    /// <param name="action">The action ID to Retarget.</param>
    extension(uint action)
    {
        /// <summary>
        ///     Retargets the action to the target specified.<br />
        ///     Only works if the <paramref name="action" /> is the Replaced Action
        ///     for the combo (i.e. Features, not generally main Combos).
        /// </summary>
        /// <param name="target">
        ///     The target to Retarget the action onto.<br />
        ///     Should be a <see cref="SimpleTarget" /> property.
        /// </param>
        /// <returns>The <paramref name="action" />.</returns>
        internal uint Retarget
            (IGameObject? target) =>
            P.ActionRetargeting.Register(action, [action], () => target);

        /// <summary>
        ///     Retargets the action to the target specified.<br />
        ///     Only works if the <paramref name="action" /> is the Replaced Action
        ///     for the combo (i.e. Features, not generally main Combos).
        /// </summary>
        /// <param name="target">
        ///     The
        ///     <see cref="ActionRetargeting.TargetResolverAttribute">
        ///         Target Resolver
        ///     </see>
        ///     that provides the <see cref="IGameObject">target</see> you want.
        /// </param>
        /// <returns>The <paramref name="action" />.</returns>
        internal uint Retarget
            (Func<IGameObject?> target) =>
            P.ActionRetargeting.Register(action, [action],
                target.CheckForAttribute());

        /// <summary>
        ///     Retargets the action to the target specified.
        /// </summary>
        /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
        /// <param name="target">
        ///     The target to Retarget the action onto.<br />
        ///     Should be a <see cref="SimpleTarget" /> property.
        /// </param>
        /// <returns>The <paramref name="action" />.</returns>
        /// <remarks>
        ///     Used when the <paramref name="action" /> is not the same as the
        ///     combo's Replaced Action (i.e. main Combos, not usually Features).
        /// </remarks>
        internal uint Retarget
        (uint replaced,
            IGameObject? target) =>
            P.ActionRetargeting.Register(action, [replaced], () => target);

        /// <summary>
        ///     Retargets the action to the target specified.
        /// </summary>
        /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
        /// <param name="target">
        ///     The target to Retarget the action onto.<br />
        ///     Should be a <see cref="SimpleTarget" /> property.
        /// </param>
        /// <returns>The <paramref name="action" />.</returns>
        /// <remarks>
        ///     Used when the <paramref name="action" /> is not the same as the
        ///     combo's Replaced Action (i.e. main Combos, not usually Features).
        /// </remarks>
        internal uint Retarget
        (uint replaced,
            Func<IGameObject?> target) =>
            P.ActionRetargeting.Register(action, [replaced],
                target.CheckForAttribute());

        /// <summary>
        ///     Retargets the action to the target specified.
        /// </summary>
        /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
        /// <param name="target">
        ///     The target to Retarget the action onto.<br />
        ///     Should be a <see cref="SimpleTarget" /> property.
        /// </param>
        /// <returns>The <paramref name="action" />.</returns>
        /// <remarks>
        ///     Used when the <paramref name="action" /> is not the same as the
        ///     combo's Replaced Actions, and there are multiple options for which
        ///     actions to replace (i.e. main combos on healers, not usually features).
        /// </remarks>
        internal uint Retarget
        (uint[] replaced,
            IGameObject? target) =>
            P.ActionRetargeting.Register(action, replaced, () => target);

        /// <summary>
        ///     Retargets the action to the target specified.
        /// </summary>
        /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
        /// <param name="target">
        ///     The target to Retarget the action onto.<br />
        ///     Should be a <see cref="SimpleTarget" /> property.
        /// </param>
        /// <returns>The <paramref name="action" />.</returns>
        /// <remarks>
        ///     Used when the <paramref name="action" /> is not the same as the
        ///     combo's Replaced Actions, and there are multiple options for which
        ///     actions to replace (i.e. main combos on healers, not usually features).
        /// </remarks>
        internal uint Retarget
        (uint[] replaced,
            Func<IGameObject?> target) =>
            P.ActionRetargeting.Register(action, replaced,
                target.CheckForAttribute());
    }
}