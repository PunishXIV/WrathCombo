using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using WrathCombo.Services.ActionRequestIPC;
using ECommonsJob = ECommons.ExcelServices.Job;

namespace WrathCombo.CustomComboNS;

/// <summary> Base class for each combo. </summary>
internal abstract partial class CustomCombo : CustomComboFunctions
{
    /// <summary> Initializes a new instance of the <see cref="CustomCombo"/> class. </summary>
    protected CustomCombo()
    {
        CustomComboInfoAttribute? presetInfo = Preset.GetAttribute<CustomComboInfoAttribute>();
        Job = presetInfo.Job;
    }

    protected IGameObject? OptionalTarget;

    /// <summary> Gets the preset associated with this combo. </summary>
    protected internal abstract Preset Preset { get; }

    /// <summary> Gets the job associated with this combo. </summary>
    protected ECommonsJob Job { get; }

    /// <summary>
    ///     This is a list of presets and their actions that are exceptions to
    ///     the rule that if "an action is unchanged, don't modify the hotbar".
    ///     <br />
    ///     These presets are those that replace actions that are changed by FF,
    ///     but that we want to have complete control over.<br />
    /// </summary>
    /// <value>
    ///     <b>Key</b>: The preset that is an exception to the rule.<br />
    ///     <b>Value</b>: The action ID that is allowed to be returned unchanged.<br />
    /// </value>
    /// <remarks>
    ///     If not excepted, these presets would be treated as not having
    ///     returned anything, and as such wouldn't be allowed to touch the
    ///     hotbar, meaning that whatever behavior they were trying to do will
    ///     not actually happen, and FF would change the action on us.<br />
    ///     Without the action also being checked, the preset would block all
    ///     other presets.
    /// </remarks>
    private readonly Dictionary<Preset, uint>
        _presetsAllowedToReturnUnchanged = new()
        {
            { Preset.DNC_DesirablePartner, DNC.ClosedPosition },
        };

    /// <summary> Performs various checks then attempts to invoke the combo. </summary>
    /// <param name="actionID"> Starting action ID. </param>
    /// <param name="newActionID"> Replacement action ID. </param>
    /// <param name="targetOverride"> Optional target override. </param>
    /// <returns> True if the action has changed, otherwise false. </returns>
    public unsafe bool TryInvoke(uint actionID, out uint newActionID, IGameObject? targetOverride = null)
    {
        newActionID = 0;

        if (!IsEnabled(Preset))
            return false;

        if (Player.Object is null) return false; //Safeguard. LocalPlayer shouldn't be null at this point anyways.
        if (Player.IsDead) return false; //Don't do combos while dead

        Job classJobID = Player.Job.GetUpgradedJob();

        if (classJobID is Job.MIN or Job.BTN or Job.FSH)
            classJobID = Job.MIN;

        if (Job != Job.ADV && Job != classJobID )
            return false;


        if(ActionRequestIPCProvider.TryGetRequestedAction(out var id))
        {
            newActionID = id;
            return true;
        }

        OptionalTarget = targetOverride;
        uint resultingActionID = Invoke(actionID);
        OptionalTarget = null;

        var presetException = _presetsAllowedToReturnUnchanged
            .TryGetValue(Preset, out var actionException);
        var hasException = presetException && resultingActionID == actionException;
        if (resultingActionID == 0 ||
            (actionID == resultingActionID && !hasException))
            return false;

        if (Service.Configuration.SuppressQueuedActions && !Svc.ClientState.IsPvP && ActionManager.Instance()->QueuedActionType == ActionType.Action && ActionManager.Instance()->QueuedActionId != actionID)
        {
            // todo: tauren: remember why this condition was in the if below:
            //      `&& WrathOpener.CurrentOpener?.OpenerStep <= 1`
            if (resultingActionID != All.SavageBlade)
                return false;
        }
        newActionID = resultingActionID;

        return true;
    }

    /// <summary> Invokes the combo. </summary>
    /// <param name="actionID"> Starting action ID. </param>
    /// 
    /// 
    /// 
    /// <returns>The replacement action ID. </returns>
    protected abstract uint Invoke(uint actionID);
}