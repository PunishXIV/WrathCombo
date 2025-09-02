#region

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;

#endregion

namespace WrathCombo.Core;

/// <summary> This class facilitates action+icon replacement. </summary>
internal sealed class ActionReplacer : IDisposable
{
    public delegate uint GetActionDelegate(IntPtr actionManager, uint actionID);

    public readonly List<CustomCombo> CustomCombos;
    public readonly Hook<GetActionDelegate> getActionHook;

    private readonly Hook<IsActionReplaceableDelegate> isActionReplaceableHook;

    public readonly Dictionary<uint, uint> LastActionInvokeFor = [];

    /// <summary>
    ///     Critical for the hook, do not remove or modify.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private IntPtr _actionManager = IntPtr.Zero;

    /// <summary> Initializes a new instance of the <see cref="ActionReplacer" /> class. </summary>
    public ActionReplacer()
    {
        CustomCombos = Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
            .Where(t => !t.IsAbstract && t.BaseType == typeof(CustomCombo))
            .Select(Activator.CreateInstance)
            .Cast<CustomCombo>()
            .OrderByDescending(x => x.Preset)
            .ToList();

        // ReSharper disable once RedundantCast
        // Must keep the nint cast
        getActionHook = Svc.Hook.HookFromAddress<GetActionDelegate>((nint) ActionManager.Addresses.GetAdjustedActionId.Value, GetAdjustedActionDetour);
        isActionReplaceableHook = Svc.Hook.HookFromAddress<IsActionReplaceableDelegate>(Service.Address.IsActionIdReplaceable, IsActionReplaceableDetour);

        getActionHook.Enable();
        isActionReplaceableHook.Enable();
    }

    public void Dispose()
    {
        getActionHook.Disable();
        getActionHook.Dispose();
        isActionReplaceableHook.Disable();
        isActionReplaceableHook.Dispose();
    }

    private ulong IsActionReplaceableDetour(uint actionID) => 1;

    /// <summary> Calls the original hook. </summary>
    /// <param name="actionID"> Action ID. </param>
    /// <returns> The result from the hook. </returns>
    internal uint OriginalHook(uint actionID) =>
        getActionHook.Original(_actionManager, actionID);

#pragma warning disable CS1573
    /// <summary>
    ///     Throttles access to <see cref="GetAdjustedAction(uint)" />.
    /// </summary>
    /// <param name="actionID">The action a combo replaces.</param>
    /// <returns>The action a combo returns.</returns>
    /// <remarks>
    ///     The <see langword="IntPtr" /> parameter is necessary for the hook
    ///     delegate, but is not used in the method.<br />
    ///     Do not remove or modify the <see langword="IntPtr" /> parameter.
    /// </remarks>
    private uint GetAdjustedActionDetour(IntPtr _, uint actionID)
    {
        try
        {
            if (FilteredCombos is null)
                UpdateFilteredCombos();

            // Bail if not wanting to replace actions in this manner
            if (Service.Configuration.PerformanceMode)
                return OriginalHook(actionID);
            if (Svc.ClientState.LocalPlayer == null)
                return OriginalHook(actionID);

            // Only refresh every so often
            if (!EzThrottler.Throttle("Actions" + actionID,
                    Service.Configuration.Throttle))
                return LastActionInvokeFor[actionID];

            //This is for the low level Archer quest(s) where you have to use Heavy Shot on an action. Best to just not run any combos here so things can run as planned.
            if (actionID == BRD.HeavyShot && Svc.Objects.Any(x => x.Name.TextValue.Equals(Svc.Data.GetExcelSheet<EObjName>()[2000925].Singular.ToString(), StringComparison.InvariantCultureIgnoreCase) && x.IsTargetable))
                return LastActionInvokeFor[BRD.HeavyShot] = BRD.HeavyShot;

            // Actually get the action
            LastActionInvokeFor[actionID] = GetAdjustedAction(actionID);
            return LastActionInvokeFor[actionID];
        }
        catch (Exception e)
        {
            e.Log();
            return actionID;
        }
    }
#pragma warning restore CS1573

    /// <summary>
    ///     Replaces an action with the result from a combo.
    /// </summary>
    /// <param name="actionID">The action a combo replaces.</param>
    /// <returns>The action a combo returns.</returns>
    private unsafe uint GetAdjustedAction(uint actionID)
    {
        try
        {
            if (ClassLocked() ||
                (DisabledJobsPVE.Any(x => x == Player.Job) && !Svc.ClientState.IsPvP) ||
                (DisabledJobsPVP.Any(x => x == Player.Job) && Svc.ClientState.IsPvP))
                return OriginalHook(actionID);

            foreach (CustomCombo? combo in FilteredCombos)
            {
                if (combo.TryInvoke(actionID, out uint newActionID))
                {
                    if (Service.Configuration.BlockSpellOnMove &&
                        ActionManager.GetAdjustedCastTime(ActionType.Action, newActionID) > 0 &&
                        CustomComboFunctions.TimeMoving.Ticks > 0)
                    {
                        return All.SavageBlade;
                    }

                    return newActionID;
                }
            }

            return OriginalHook(actionID);
        }

        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Preset error");
            return OriginalHook(actionID);
        }
    }

    internal static bool DisableJobCheck = false;

    /// <summary>
    ///     Checks if the player could be on a job instead of a class.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the user could be on a job instead.
    /// </returns>
    public static unsafe bool ClassLocked()
    {
        if (DisableJobCheck) return false;
        
        if (Player.Object is null) return false;

        if (Player.Level <= 35) return false;

        // DoL and higher except arcanist and rogue
        if (Player.Job is >= Job.MIN and not (Job.ACN or Job.ROG))
            return false;

        if (!UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(66049))
            return false;

        if ((Player.Job is Job.GLA or Job.PGL or Job.MRD or Job.LNC or Job.ARC or Job.CNJ or Job.THM or Job.ACN or Job.ROG) &&
            Svc.Condition[ConditionFlag.BoundByDuty56] && // in an instance duty
            Player.Level > 35) return true;

        return false;
    }

    private delegate ulong IsActionReplaceableDelegate(uint actionID);

    #region Restrict combos to current job

    public static IEnumerable<CustomCombo>? FilteredCombos;

    public void UpdateFilteredCombos()
    {
        FilteredCombos = CustomCombos.Where(x =>
            x.Preset.Attributes() is not null && x.Preset.Attributes().IsPvP == CustomComboFunctions.InPvP() &&
            ((x.Preset.Attributes().RoleAttribute is not null && x.Preset.Attributes().RoleAttribute.PlayerIsRole()) ||
             x.Preset.Attributes().CustomComboInfo.Job == Player.Job.GetUpgradedJob()));
        var filteredCombos = FilteredCombos as CustomCombo[] ?? FilteredCombos.ToArray();
        Svc.Log.Debug(
            $"Now running {filteredCombos.Count()} combos\n{string.Join("\n", filteredCombos.Select(x => x.Preset.Attributes().CustomComboInfo.Name))}");
    }

    #endregion
}