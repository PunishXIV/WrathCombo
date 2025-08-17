#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using ECommons.Throttlers;
using WrathCombo.Combos.PvE;
using WrathCombo.Data;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

// ReSharper disable UnusedType.Global

#endregion

namespace WrathCombo.Extensions;

public static class RezMacro
{
    /// <summary>
    ///     Parameter building and refinement, and general logic for whether to
    ///     actually print a macro.
    /// </summary>
    /// <param name="action">
    ///     The action ID of the rez. Will be used to check
    ///     <see cref="ActionWatching.UsedOnDict" /> for the target.<br />
    ///     This or a target must be provided.
    /// </param>
    /// <param name="firstCall">
    ///     Whether this is the first call of the method.<br />
    ///     Should always be true (aka default, aka excluded).
    ///     Is iterated within the method for a single retry.
    /// </param>
    /// <param name="targetID">
    ///     The ID of the target.<br />
    ///     Should only be provided if the rez will absolutely be used on this
    ///     target.<br />
    ///     i.e. only from <see cref="AutoRotation.AutoRotationController" />.
    /// </param>
    /// <param name="targetObj">
    ///     The <see cref="IPlayerCharacter" /> of the target.<br />
    ///     Should only be provided if the rez will absolutely be used on this
    ///     target.<br />
    ///     i.e. only from <see cref="AutoRotation.AutoRotationController" />.
    /// </param>
    private static void CheckThenPrintMacro
    (uint? action = null,
        bool firstCall = true,
        ulong? targetID = null,
        IPlayerCharacter? targetObj = null)
    {
        // Error out if called incorrectly
        if (action is null && targetID is null && targetObj is null)
        {
            ErrorLog($"Called incorrectly; no action or target. StackTrace: {Environment.StackTrace}");
            return;
        }

        // Simple Bails
        if (!Player.Available)
            return;
#if !DEBUG
        if (!InCombat())
            return;
#endif

        // Respect user setting
        if (!Service.Configuration.OutputRezMacro)
            return;

        // Block macro spam
        if (!EzThrottler.Throttle("RezMacroSendThrottle", 3000))
        {
            VerboseLog("Throttled for all targets");
            return;
        }

        // Block non-rez actions
        if (action is not null && !_resurrectionActions.Contains(action.Value))
        {
            VerboseLog($"Action is not a rez (action: {action})");
            return;
        }

        // Only print the macro if rez was actually used
        if (action is not null && !JustUsed(action.Value))
            // Retry once
            if (firstCall)
            {
                DebugLog("Rez was not used; retrying once");
                TM.DelayNext(150);
                TM.Enqueue(() =>
                    CheckThenPrintMacro(action.Value, false, targetID));
                return;
            }
            // Bail if not used
            else
            {
                DebugLog("Rez was not used");
                return;
            }

        // Propagate the ID from targetObj, if provided
        if (targetObj is not null)
            targetID = targetObj.GameObjectId;

        // Fetch what target the rez went onto, if one was not provided
        if (targetID is null)
        {
            var mostRecentUse = ActionWatching.UsedOnDict
                .Where(kv => kv.Key.Item1 == action)
                .OrderByDescending(kv => kv.Value)
                .FirstOrDefault();
            targetID = mostRecentUse.Key.Item2;

            if (mostRecentUse.Key == default)
            {
                ErrorLog(
                    $"Cannot find the supposedly-JustUsed action in the dictionary (action: {action})");
                return;
            }
        }

        // Convert the target to an object
        if (targetObj is null)
        {
            targetObj = GetPlayerCharacter(targetID.Value);
            if (targetObj is null)
            {
                ErrorLog($"Cannot find who the target was (target: {targetID})");
                return;
            }
        }

        // Block macro spam (again, for same-target)
        if (!EzThrottler.Throttle($"RezMacro{targetObj.GameObjectId}SendThrottle", 5000))
        {
            VerboseLog($"Throttled for same target (target: {targetObj.GameObjectId})");
            return;
        }

        // Actually request the macro be printed
        PrintMacro(targetObj);
    }

    /// <summary>
    ///     Actually print the rez macro.
    /// </summary>
    /// <param name="target">The player to treat as <c>&lt;t&gt;</c></param>
    /// <param name="channel">
    ///     The channel to print into.<br />
    ///     Should only be specified when used for testing, to go to echo chat.
    /// </param>
    private static void PrintMacro
        (IPlayerCharacter target, XivChatType channel = XivChatType.Party)
    {
        // Handled like this for wider compatibility, if another message sending method is used
        List<Payload> payloads = [];

        // Split the macro text into parts (to replace "<t>" with the player link)
        var macroText = Service.Configuration.RezMacro
            .Split(["<t>", "<T>"], StringSplitOptions.None);

        // Build the Player link
        var playerPayload =
            new PlayerPayload(target.Name.TextValue, target.HomeWorld.RowId);

        // Replace <t> with the player link
        foreach (var text in macroText)
        {
            payloads.Add(new TextPayload(text));
            payloads.Add(playerPayload);
        }

        // Remove the last player link
        payloads.RemoveAt(payloads.Count - 1);

        // String-Only Payloads
        var payloadString = "";
        if (channel == XivChatType.Party)
            payloadString += "/p ";
        else
            payloadString += "/e ";
        payloadString = "/e "; // todo: temporary to ensure only echo
        foreach (var payload in payloads)
        {
            if (payload is TextPayload textLoad)
                payloadString += textLoad.Text;
            if (payload is PlayerPayload playerLoad)
                payloadString += playerLoad.PlayerName;
        }

        Chat.SendMessage(payloadString);
    }

    #region Methods to initiate a Macro

    /// <summary>
    ///     The main way to call the rez macro, as an extension of a rez action ID.
    /// </summary>
    /// <param name="action">
    ///     The rez macro this is an extension of, not actually provided.
    /// </param>
    /// <returns>
    ///     The same Action ID, so this can be used in <see langword="return" />
    ///     statements.
    /// </returns>
    public static uint AndRunMacro(this uint action)
    {
        TM.DelayNext(2000);
        TM.Enqueue(() => CheckThenPrintMacro(action));

        return action;
    }

    /// <summary>
    ///     Try to run a macro for a rez action.<br />
    ///     This call should ONLY be made when the rez will absolutely be used with
    ///     the given target.<br />
    ///     i.e. only from <see cref="AutoRotation.AutoRotationController" />.
    /// </summary>
    /// <param name="target">The target object.</param>
    public static void RunMacro(IPlayerCharacter target)
    {
        TM.Enqueue(() => CheckThenPrintMacro(targetObj: target));
    }

    /// <see cref="RunMacro(IPlayerCharacter)" />
    /// with a target ID instead.
    /// <seealso cref="RunMacro(IPlayerCharacter)" />
    public static void RunMacro(ulong targetID)
    {
        TM.Enqueue(() => CheckThenPrintMacro(targetID: targetID));
    }

    #endregion

    #region Utility Methods

    private static uint[] _resurrectionActions = [
        WHM.Raise,
        AST.Ascend,
        SGE.Egeiro,
        SCH.Resurrection,
        BLU.AngelWhisper,
        RDM.Verraise,
        SMN.Resurrection,
    ];

    /// <summary>
    ///     Print the rez macro, formatted, to echo chat for testing.
    /// </summary>
    /// <seealso cref="PrintMacro" />
    internal static void PrintTestMacro()
    {
        if (!Player.Available)
        {
            DuoLog.Error("Player character is not available for testing.");
            return;
        }

        PrintMacro(Player.Object, XivChatType.Echo);
    }

    /// <summary>
    ///     Convert an object ID back to a game object (as a
    ///     <see cref="IPlayerCharacter" />).
    /// </summary>
    /// <param name="target">
    ///     The object ID to convert.
    /// </param>
    /// <returns>
    ///     The <see cref="IPlayerCharacter" /> object, or
    ///     <see langword="null" /> if not
    ///     found.
    /// </returns>
    private static IPlayerCharacter? GetPlayerCharacter(ulong target)
    {
        try
        {
            return Svc.Objects
                    .FirstOrDefault(x => x.GameObjectId == target)
                as IPlayerCharacter;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Standardizing logging for this extension.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void ErrorLog(string message) =>
        PluginLog.Error($"[RezMacro] {message}");

    /// <summary>
    ///     Standardizing debug logging for this extension.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void DebugLog(string message) =>
        PluginLog.Debug($"[RezMacro] {message}");

    /// <summary>
    ///     Standardizing verbose logging for this extension.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void VerboseLog(string message) =>
        PluginLog.Verbose($"[RezMacro] {message}");

    #endregion
}
