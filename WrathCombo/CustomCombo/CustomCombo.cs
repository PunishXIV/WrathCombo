using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.Attributes;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS
{
    /// <summary> Base class for each combo. </summary>
    internal abstract partial class CustomCombo : CustomComboFunctions
    {
        /// <summary> Initializes a new instance of the <see cref="CustomCombo"/> class. </summary>
        protected CustomCombo()
        {
            CustomComboInfoAttribute? presetInfo = Preset.GetAttribute<CustomComboInfoAttribute>();
            JobID = presetInfo.JobID;
            ClassID = JobIDs.JobToClass(JobID);
        }

        protected IGameObject? OptionalTarget;

        /// <summary> Gets the preset associated with this combo. </summary>
        protected internal abstract CustomComboPreset Preset { get; }

        /// <summary> Gets the class ID associated with this combo. </summary>
        protected byte ClassID { get; }

        /// <summary> Gets the job ID associated with this combo. </summary>
        protected uint JobID { get; }

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
        private readonly Dictionary<CustomComboPreset, uint>
            _presetsAllowedToReturnUnchanged = new()
            {
                { CustomComboPreset.DNC_DesirablePartner, DNC.ClosedPosition },
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

            uint classJobID = LocalPlayer!.ClassJob.RowId;

            if (classJobID is >= 8 and <= 15)
                classJobID = DOH.JobID;

            if (classJobID is >= 16 and <= 18)
                classJobID = DOL.JobID;

            if (JobID != ADV.JobID && ClassID != ADV.ClassID &&
                JobID != classJobID && ClassID != classJobID)
                return false;

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
}
