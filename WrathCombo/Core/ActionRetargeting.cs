#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Logging;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
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
///     <see cref="UIntExtensions.Retarget(uint,IGameObject?,bool)">
///         Retarget(uint,IGameObject) for simple Feature Retargeting.
///     </see>
///     <br />
///     See
///     <see cref="UIntExtensions.Retarget(uint,uint,TargetResolverDelegate,bool)">
///         Retarget(uint,uint,TargetResolverDelegate) for more advanced
///         Retargeting in a full combo.
///     </see>
///     <br />
///     See <see cref="TargetResolverDelegate" /> for an example on setting up more
///     advanced Retargeting.
/// </summary>
public class ActionRetargeting : IDisposable
{
    /// List of Retargets for actions, keyed with the action and replaced action(s).
    private readonly Dictionary<uint, Retargeting>
        _retargets = [];

    /// <summary>
    ///     The Name of your
    ///     <see cref="TargetResolverDelegate">Custom Target Resolver</see>.<br />
    ///     If you are using a custom resolver, it should set this variable with its
    ///     name, to help debug logs.
    /// </summary>
    public string? MyResolverMethodName { get; set; }

    /// <summary>
    ///     Register an action and its replaced actions as one you want Retargeted.
    /// </summary>
    /// <param name="action">The Action to retarget.</param>
    /// <param name="replacedActions">The Actions the combo is replacing</param>
    /// <param name="resolver">
    ///     The <see cref="TargetResolverDelegate" /> to resolve the target.<br />
    ///     Examples:
    ///     a <see cref="SimpleTarget">SimpleTarget</see> (like
    ///     <see cref="SimpleTarget.HardTarget">HardTarget</see>)
    ///     or
    ///     a <see cref="TargetResolverDelegate">custom delegate</see> (like:
    ///     <see cref="Combos.PvE.AST.DPSCardsTargetResolver">AST.CardsResolver</see>)
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     <see cref="Retargeting" /> params for further explanation.
    /// </param>
    /// <returns>
    ///     The <paramref name="action" /> that was registered.<br />
    ///     This only really returns to make
    ///     <see cref="UIntExtensions.Retarget(uint,TargetResolverDelegate,bool)">
    ///         (uint).Retarget()
    ///     </see>
    ///     simpler.
    /// </returns>
    /// <remarks>
    ///     Should only be called by
    ///     <see cref="UIntExtensions.Retarget(uint,TargetResolverDelegate,bool)">
    ///         (uint).Retarget()
    ///     </see>
    ///     .
    /// </remarks>
    internal uint Register
    (uint action,
        uint[] replacedActions,
        TargetResolverDelegate resolver,
        bool dontCull = false)
    {
        // Check the complexity of this Retarget, for overwriting purposes
        var actionInReplaced = replacedActions.Contains(action);

        // Make sure the action is not in replaced actions,
        // and there are no duplicates
        replacedActions = replacedActions.Where(a => a != action)
            .Distinct().ToArray();
        // Build the Retarget object
        var retarget = new Retargeting(action, replacedActions, resolver,
            actionInReplaced, dontCull);

        // Limit spam from the same actionID, mostly for debugging
        if (!EZ.Throttle($"retargetFor{retarget.ID}", TS.FromSeconds(1)))
            return action;

        #region Replace existing Retargets

        var partialOverwrite = false;
        string[] overwriting = [];
        Retargeting? oldRetarget = null;
        foreach (var replacedAction in replacedActions.Concat([action]))
        {
            if (!_retargets.TryGetValue(replacedAction, out oldRetarget))
                continue;

            // Keep the old Retarget if it's the same resolver
            if (oldRetarget.ResolverName == retarget.ResolverName)
                return action;

            // Flag as a partial overwrite if `actionInReplaced` has different values
            if (replacedActions != oldRetarget.ReplacedActions)
                partialOverwrite = true;

            overwriting = [oldRetarget.ResolverName, retarget.ResolverName];
        }

        // Remove the old Retarget
        if (overwriting.Length != 0)
        {
            RemoveRetarget(oldRetarget.ID);

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
    ///     <see cref="TargetResolverDelegate" />.
    /// </summary>
    /// <param name="action">The Action or Replaced Action to Retarget.</param>
    /// <param name="target">
    ///     The output  <see cref="IGameObject">Game Object</see> of the target, if
    ///     the action was found to be Retargeted.
    /// </param>
    /// <returns>
    ///     Whether the action is registered for Retargeting.
    /// </returns>
    public bool TryGetTargetFor(uint action, out IGameObject? target)
    {
        target = null;

        // Find the Retarget object
        if (!_retargets.TryGetValue(action, out var retarget))
            return false;

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
    ///     The <see cref="TargetResolverDelegate" /> that resolves the target for the
    ///     action.<br />
    ///     Can be a <see cref="SimpleTarget" />, but it gets wrapped in a delegate
    ///     in <see cref="UIntExtensions.Retarget(uint,IGameObject?,bool)" />, or its
    ///     two overloads.
    /// </param>
    /// <param name="actionInReplaced">
    ///     Whether the action was listed in the Replaced Actions originally.<br />
    ///     Only used to flag partial overwrites, to save a little bit of log spam.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this Retarget should be exempt from periodic culls.<br />
    ///     Should only be set (to <see langword="true" />) for Features; combo
    ///     Retargets should always be marked for culling.
    /// </param>
    private class Retargeting(
        uint action,
        uint[] replacedActions,
        TargetResolverDelegate resolver,
        bool actionInReplaced,
        bool dontCull = false)
    {
        /// A unique identifier for the Retarget, to help with overwrites and removal.
        public int ID { get; } = HashCode.Combine(action, replacedActions);

        /// The action we actually want to Retarget.
        public uint Action { get; } = action;

        /// The actions that could be what is actually pressed when we want to Retarget the action.
        public uint[] ReplacedActions { get; } = replacedActions;

        /// A simple flag for whether the action is in the Replaced Actions.<br />
        /// Only used to help with log spam.
        public bool ActionInReplaced { get; } = actionInReplaced;

        /// The target resolver that will decide the new target for the action.
        public TargetResolverDelegate Resolver { get; } = resolver;

        /// The name of the resolver method, to help with debugging.
        public string ResolverName { get; } = GetMethodName(resolver);

        /// Whether this Retarget should be removed by periodic culls.
        /// <see cref="ClearOldRetargets" />
        public bool DontCull { get; } = dontCull;

        /// When this was created, to help with age-outs.
        public DateTime Created { get; } = DateTime.Now;

        /// <summary>
        ///     Gets the name for the resolver.
        /// </summary>
        /// <param name="resolver">The resolver we want the name of.</param>
        /// <returns>
        ///     The name of the resolver, whether determined from the class (for
        ///     <see cref="SimpleTarget">Simple Targets</see>) or the given name from the
        ///     resolver itself.
        /// </returns>
        private static string GetMethodName(TargetResolverDelegate resolver)
        {
            try
            {
                resolver.Invoke();
            }
            catch (Exception ex)
            {
                log("error while resolving name for method",
                    logLevel: 3,
                    extraText: $":\n{ex}");
                return "<ErrorResolvingNameOfCustomResolver>";
            }

            var resolverName = resolver.Method.Name;
            var resolverClass = resolver.Method.DeclaringType?.Name ?? "";

            // Standardize the names, and if a custom resolver is used,
            // try to provide the custom name if one was provided
            resolverName = resolverClass switch
            {
                "SimpleTarget" => "SimpleTarget." + resolverName,
                "Stack" => "SimpleTarget.Stack." + resolverName,
                "<>c" => P.ActionRetargeting.MyResolverMethodName ??
                         "<UnnamedCustomResolver>",
                // todo: fix for drk
                _ => resolverName,
            };
            P.ActionRetargeting.MyResolverMethodName = null;

            return $"`{resolverName}`";
        }
    }

    #region Utilities

    /// <summary>
    ///     Adds a new Retarget to both lists of Retargets.
    /// </summary>
    /// <param name="retarget">The Retarget to add.</param>
    private void AddRetarget(Retargeting retarget)
    {
        _retargets[retarget.Action] = retarget;

        foreach (var replacedAction in retarget.ReplacedActions)
            _retargets[replacedAction] = retarget;
    }

    /// <summary>
    ///     Removes all Retargets for an action.
    /// </summary>
    /// <param name="retargetID">The ID of the Retarget object to remove.</param>
    private void RemoveRetarget(int retargetID)
    {
        var retarget = _retargets.Values
            .FirstOrDefault(x => x.ID == retargetID);
        if (retarget == null)
            return;

        var actionsToRemove = retarget.ReplacedActions.Concat([retarget.Action]);
        foreach (var replacedAction in actionsToRemove)
            _retargets.Remove(replacedAction);
    }

    /// Simple Flag to stop clearing the cache when the plugin is disposed.
    private bool CancelCacheClearing;

    /// Clears Retargets older than 20 seconds every 11 seconds.
    /// <seealso cref="_retargets" />
    internal Action ClearOldRetargets = () =>
    {
        if (P.ActionRetargeting.CancelCacheClearing)
            return;

        // Find old Retargets that are allowed to be culled
        var oldRetargets = P.ActionRetargeting._retargets.Values
            .Where(x => !x.DontCull &&
                        (DateTime.Now - x.Created) > TS.FromSeconds(20))
            .ToList();

        // Cull each unique Retarget
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
        _retargets.Clear();
        log("cleared cached Retargets", logLevel: 1);
    }

    public void Dispose()
    {
        _retargets.Clear();
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

/// <summary>
///     Delegate function to resolve a target for an action.<br />
///     Should set <see cref="ActionRetargeting.MyResolverMethodName" /> to the
///     name of the method that is being used to resolve the target.
/// </summary>
/// <returns>
///     The <see cref="IGameObject">Game Object</see> of the target.
/// </returns>
/// <remarks>
///     This resolver is run at initialization and when the action is used,
///     it should not try to do anything except return the target.<br />
///     As in, it shouldn't modify the state of your combo.
/// </remarks>
/// <example>
///     For inline delegate-error checking purposes it is suggested you write your
///     resolvers as static field lambdas in your Job's class or JobHelper class.
///     <code>
///     public static TargetResolverDelegate ExampleTargetResolver = () => {
///         P.ActionRetargeting.MyResolverMethodName =
///             "ExampleTargetResolver";
///         if (Config.HelpfulGroundTargetAbility_TargetOther)
///             return SimpleTargets.FocusTarget;
///     };
///     </code>
/// </example>
/// <seealso cref="Combos.PvE.AST.DPSCardsTargetResolver">
///     AST.CardsResolver
/// </seealso>
public delegate IGameObject? TargetResolverDelegate();

internal static class UIntExtensions
{
    // One for "no replaced action specified" (simple features),
    // "one replaced action specified" (simple combos),
    // and "multiple replaced actions specified" (complex combos, like healers),
    // each accepting a direct target or a target resolver.

    /// <summary>
    ///     Retargets the action to the target specified.<br />
    ///     Only works if the <paramref name="action" /> is the Replaced Action
    ///     for the combo (i.e. Features, not generally main Combos).
    /// </summary>
    /// <param name="action">The action ID to Retarget.</param>
    /// <param name="target">
    ///     The target to Retarget the action onto.<br />
    ///     Should be a <see cref="SimpleTarget" /> property.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     See <see cref="ActionRetargeting.Retargeting" /> params for further
    ///     explanation.
    /// </param>
    /// <returns>The <paramref name="action" />.</returns>
    internal static uint Retarget
        (this uint action, IGameObject? target, bool dontCull = false) =>
        P.ActionRetargeting.Register(action, [action], () => target, dontCull);

    /// <summary>
    ///     Retargets the action to the target specified.<br />
    ///     Only works if the <paramref name="action" /> is the Replaced Action
    ///     for the combo (i.e. Features, not generally main Combos).
    /// </summary>
    /// <param name="action">The action ID to Retarget.</param>
    /// <param name="target">
    ///     The <see cref="TargetResolverDelegate">Target Resolver</see> that
    ///     decides the target to Retarget the action onto.<br />
    ///     Make sure to set <see cref="ActionRetargeting.MyResolverMethodName" /> to
    ///     the name of your resolver method.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     See <see cref="ActionRetargeting.Retargeting" /> params for further
    ///     explanation.
    /// </param>
    /// <returns>The <paramref name="action" />.</returns>
    internal static uint Retarget
        (this uint action, TargetResolverDelegate target, bool dontCull = false) =>
        P.ActionRetargeting.Register(action, [action], target, dontCull);

    /// <summary>
    ///     Retargets the action to the target specified.
    /// </summary>
    /// <param name="action">The action ID to retarget.</param>
    /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
    /// <param name="target">
    ///     The target to Retarget the action onto.<br />
    ///     Should be a <see cref="SimpleTarget" /> property.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     See <see cref="ActionRetargeting.Retargeting" /> params for further
    ///     explanation.
    /// </param>
    /// <returns>The <paramref name="action" />.</returns>
    /// <remarks>
    ///     Used when the <paramref name="action" /> is not the same as the
    ///     combo's Replaced Action (i.e. main Combos, not usually Features).
    /// </remarks>
    internal static uint Retarget
    (this uint action,
        uint replaced,
        IGameObject? target,
        bool dontCull = false) =>
        P.ActionRetargeting.Register(action, [replaced], () => target, dontCull);

    /// <summary>
    ///     Retargets the action to the target specified.
    /// </summary>
    /// <param name="action">The action ID to retarget.</param>
    /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
    /// <param name="target">
    ///     The <see cref="TargetResolverDelegate">Target Resolver</see> that
    ///     decides the target to Retarget the action onto.<br />
    ///     Make sure to set <see cref="ActionRetargeting.MyResolverMethodName" /> to
    ///     the name of your  resolver method.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     See <see cref="ActionRetargeting.Retargeting" /> params for further
    ///     explanation.
    /// </param>
    /// <returns>The <paramref name="action" />.</returns>
    /// <remarks>
    ///     Used when the <paramref name="action" /> is not the same as the
    ///     combo's Replaced Action (i.e. main Combos, not usually Features).
    /// </remarks>
    internal static uint Retarget
    (this uint action, uint replaced, TargetResolverDelegate target, bool
        dontCull = false) =>
        P.ActionRetargeting.Register(action, [replaced], target, dontCull);

    /// <summary>
    ///     Retargets the action to the target specified.
    /// </summary>
    /// <param name="action">The action ID to retarget.</param>
    /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
    /// <param name="target">
    ///     The target to Retarget the action onto.<br />
    ///     Should be a <see cref="SimpleTarget" /> property.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     See <see cref="ActionRetargeting.Retargeting" /> params for further
    ///     explanation.
    /// </param>
    /// <returns>The <paramref name="action" />.</returns>
    /// <remarks>
    ///     Used when the <paramref name="action" /> is not the same as the
    ///     combo's Replaced Actions, and there are multiple options for which
    ///     actions to replace (i.e. main combos on healers, not usually features).
    /// </remarks>
    internal static uint Retarget
    (this uint action,
        uint[] replaced,
        IGameObject? target,
        bool dontCull = false) =>
        P.ActionRetargeting.Register(action, replaced, () => target, dontCull);

    /// <summary>
    ///     Retargets the action to the target specified.
    /// </summary>
    /// <param name="action">The action ID to retarget.</param>
    /// <param name="replaced">The action ID of the combo's Replaced Action.</param>
    /// <param name="target">
    ///     The <see cref="TargetResolverDelegate">Target Resolver</see> that
    ///     decides the target to Retarget the action onto.<br />
    ///     Make sure to set <see cref="ActionRetargeting.MyResolverMethodName" /> to
    ///     the name of your resolver method.
    /// </param>
    /// <param name="dontCull">
    ///     Whether this method should be exempt from periodic culls.<br />
    ///     See <see cref="ActionRetargeting.Retargeting" /> params for further
    ///     explanation.
    /// </param>
    /// <returns>The <paramref name="action" />.</returns>
    /// <remarks>
    ///     Used when the <paramref name="action" /> is not the same as the
    ///     combo's Replaced Actions, and there are multiple options for which
    ///     actions to replace (i.e. main combos on healers, not usually features).
    /// </remarks>
    internal static uint Retarget
    (this uint action,
        uint[] replaced,
        TargetResolverDelegate target,
        bool dontCull = false) =>
        P.ActionRetargeting.Register(action, replaced, target, dontCull);
}
