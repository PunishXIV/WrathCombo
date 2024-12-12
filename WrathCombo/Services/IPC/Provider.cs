#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECommons.EzIpcManager;
using WrathCombo.Attributes;
using WrathCombo.Combos;

// ReSharper disable UnusedMember.Global

#endregion

namespace WrathCombo.Services.IPC;

/// <summary>
///     IPC service for other plugins to have user-overridable control of Wrath.<br />
///     See <see cref="Provider.RegisterForLease" /> for details on use.<br />
///     See the "Normal IPC Flow" region for the main IPC methods.
/// </summary>
/// <!--<remarks>
///     TODO:
///     <list type="bullet">
///         <item>
///             <description>
///                 nothing
///             </description>
///         </item>
///     </list>
/// </remarks>-->
public partial class Provider
{
    /// <summary>
    ///     Method to test IPC.
    /// </summary>
    [EzIPC]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void Test() => Logging.Log("IPC connection successful.");

    #region Helpers

    /// <summary>
    ///     Leasing services for the IPC, essentially a backer for <c>Set</c>
    ///     methods.
    /// </summary>
    private Leasing _leasing;

    /// <summary>
    ///     The helper services for the IPC provider.
    /// </summary>
    private Helper _helper;

    /// <summary>
    ///     Search services for the IPC provider, essentially a backer for <c>Get</c>
    ///     methods.
    /// </summary>
    private Search _search;

    /// <summary>
    ///     Initializes the class, and sets up the other parts of the IPC provider.
    /// </summary>
    internal Provider()
    {
        _leasing = new Leasing();
        _helper = new Helper(ref _leasing);
        _search = new Search(ref _leasing);
        EzIPC.Init(this, prefix: "WrathCombo");
    }

    #endregion

    #region Normal IPC Flow

    /// <summary>
    ///     Register your plugin for control of Wrath Combo.
    /// </summary>
    /// <param name="pluginName">
    ///     The name you want shown to Wrath users for options your plugin controls.
    /// </param>
    /// <param name="leaseCancelledCallback">
    ///     Your method to be called when your lease is cancelled, usually
    ///     by the user.<br />
    ///     The <see cref="CancellationReason" /> and a string with any additional
    ///     info will be passed to your method.
    /// </param>
    /// <returns>
    ///     Your lease ID to be used in <c>set</c> calls.<br />
    ///     Or <c>null</c> if your lease was not registered, which can happen for
    ///     multiple reasons:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A lease exists with the <c>pluginName</c>.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Your lease was revoked by the user recently.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 The IPC service is currently disabled.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    /// <remarks>
    ///     Each lease is limited to controlling <c>40</c> configurations.
    /// </remarks>
    /// <seealso cref="Leasing.MaxLeases" />
    [EzIPC]
    public Guid? RegisterForLease
    (string pluginName,
        Action<CancellationReason, string>? leaseCancelledCallback = null)
    {
        // Bail if IPC is disabled
        if (!_helper.IPCEnabled)
        {
            Logging.Warn(BailMessages.LiveDisabled);
            return null;
        }

        throw new NotImplementedException();
    }

    /// <summary>
    ///     Get the current state of the Auto-Rotation setting in Wrath Combo.
    /// </summary>
    /// <returns>Whether Auto-Rotation is enabled or disabled</returns>
    /// <remarks>
    ///     This is only the state of Auto-Rotation, not whether any combos are
    ///     enabled in Auto-Mode.
    /// </remarks>
    [EzIPC]
    public bool GetAutoRotationState() =>
        Service.Configuration.RotationConfig.Enabled;

    /// <summary>
    ///     Set the state of Auto-Rotation in Wrath Combo.
    /// </summary>
    /// <param name="lease">Your lease ID from <see cref="RegisterForLease" /></param>
    /// <param name="enable">
    ///     Optionally whether to enable Auto-Rotation.<br />
    ///     Only used to disable Auto-Rotation, as enabling it is the default.
    /// </param>
    /// <seealso cref="GetAutoRotationState" />
    /// <remarks>
    ///     This is only the state of Auto-Rotation, not whether any combos are
    ///     enabled in Auto-Mode.
    /// </remarks>
    /// <value>+1 <c>set</c></value>
    [EzIPC]
    public void SetAutoRotationState(Guid lease, bool enable = true)
    {
        #region Bails

        // Bail if IPC is disabled
        if (!_helper.IPCEnabled)
        {
            Logging.Warn(BailMessages.LiveDisabled);
            return;
        }

        // Bail if the lease is not valid
        if (!_leasing.CheckLeaseExists(lease))
        {
            Logging.Warn(BailMessages.InvalidLease);
            return;
        }

        // Bail if the lease does not have enough configuration left for this set
        if (_leasing.CheckLeaseConfigurationsAvailable(lease) >= 1)
        {
            Logging.Warn(BailMessages.NotEnoughConfigurations);
            return;
        }

        #endregion

        throw new NotImplementedException();
    }

    /// <summary>
    ///     Checks if the current job has a Single and Multi-Target combo configured
    ///     that are enabled in Auto-Mode.
    /// </summary>
    /// <returns>
    ///     If the user's current job is fully ready for Auto-Rotation.
    /// </returns>
    /// <seealso cref="IsCurrentJobConfiguredOn" />
    /// <seealso cref="IsCurrentJobAutoModeOn" />
    [EzIPC]
    public bool IsCurrentJobAutoRotationReady()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Sets up the user's current job for Auto-Rotation.<br />
    ///     This will enable the Single and Multi-Target combos, and enable them in
    ///     Auto-Mode.<br />
    ///     This will try to use the user's existing settings, only enabling default
    ///     states for jobs that are not configured.
    /// </summary>
    /// <value>+6 <c>set</c></value>
    /// <param name="lease">Your lease ID from <see cref="RegisterForLease" /></param>
    [EzIPC]
    public void SetCurrentJobAutoRotationReady(Guid lease)
    {
        #region Bails

        // Bail if IPC is disabled
        if (!_helper.IPCEnabled)
        {
            Logging.Warn(BailMessages.LiveDisabled);
            return;
        }

        // Bail if the lease is not valid
        if (!_leasing.CheckLeaseExists(lease))
        {
            Logging.Warn(BailMessages.InvalidLease);
            return;
        }

        // Bail if the lease does not have enough configuration left for this set
        if (_leasing.CheckLeaseConfigurationsAvailable(lease) >= 6)
        {
            Logging.Warn(BailMessages.NotEnoughConfigurations);
            return;
        }

        #endregion

        throw new NotImplementedException();
    }

    /// <summary>
    ///     This cancels your lease, removing your control of Wrath Combo.
    /// </summary>
    /// <param name="lease">Your lease ID from <see cref="RegisterForLease" /></param>
    /// <remarks>
    ///     Will call your <c>leaseCancelledCallback</c> method if you provided one,
    ///     with the reason <see cref="CancellationReason.LeaseeReleased" />.
    /// </remarks>
    [EzIPC]
    public void ReleaseControl(Guid lease)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Extra Job State Checks

    /// <summary>
    ///     Checks if the user's current job has a Single-Target and Multi-Target
    ///     combo configured.
    /// </summary>
    /// <returns>
    ///     <see cref="ComboTargetType.SingleTarget" /> - a <c>bool</c> indicating if
    ///     a Single-Target combo is configured.<br />
    ///     <see cref="ComboTargetType.MultiTarget" /> - a <c>bool</c> indicating if
    ///     a Multi-Target combo is configured.
    /// </returns>
    [EzIPC]
    public Dictionary<string, bool> IsCurrentJobConfiguredOn()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Checks if the user's current job has a Single-Target and Multi-Target
    ///     combo enabled in Auto-Mode.
    /// </summary>
    /// <returns>
    ///     <see cref="ComboTargetType.SingleTarget" /> - a <c>bool</c> indicating if
    ///     a Single-Target combo is enabled in Auto-Mode.<br />
    ///     <see cref="ComboTargetType.MultiTarget" /> - a <c>bool</c> indicating if
    ///     a Multi-Target combo is enabled in Auto-Mode.
    /// </returns>
    [EzIPC]
    public Dictionary<string, bool> IsCurrentJobAutoModeOn()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Fine-Grained Combo Methods

    /// <summary>
    ///     Gets the internal names of all combos and options for the given job.
    /// </summary>
    /// <param name="jobAbbreviation">
    ///     The all-caps, 3-letter abbreviation for the job you want to search for.
    ///     <br />
    ///     Defaults to the user's current job if not provided.<br />
    ///     See <see cref="CustomComboInfoAttribute.JobIDToShorthand" />.
    /// </param>
    /// <returns>
    ///     A list of internal names for all combos and options for the given job.
    /// </returns>
    /// <seealso cref="Search.SearchForCombosInAutoMode" />
    [EzIPC]
    public List<string> GetComboNamesForJob(string? jobAbbreviation)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Get the current state of a combo in Wrath Combo.
    /// </summary>
    /// <param name="comboInternalName">
    ///     The internal name of the combo you want to check.<br />
    ///     See <see cref="CustomComboPreset" /> or <see cref="GetComboNamesForJob" />.
    /// </param>
    /// <returns>
    ///     <see cref="ComboState.Enabled" /> - a <c>bool</c> indicating if
    ///     the combo is enabled.<br />
    ///     <see cref="ComboState.AutoMode" /> - a <c>bool</c> indicating if the
    ///     combo is enabled in Auto-Mode.
    /// </returns>
    [EzIPC]
    public Dictionary<string, bool> GetComboState(string comboInternalName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Set the state of a combo in Wrath Combo.
    /// </summary>
    /// <value>+2 <c>set</c></value>
    /// <param name="lease">Your lease ID from <see cref="RegisterForLease" /></param>
    /// <param name="comboInternalName">
    ///     The internal name of the combo you want to set.<br />
    ///     See <see cref="CustomComboPreset" /> or <see cref="GetComboNamesForJob" />.
    /// </param>
    /// <param name="comboState">
    ///     Optionally whether to enable combo.<br />
    ///     Only used to disable the combo, as enabling it is the default.
    /// </param>
    /// <param name="autoState">
    ///     Optionally whether to enable the combo in Auto-Mode.<br />
    ///     Only used to disable the combo in Auto-Mode, as enabling it is the
    ///     default.
    /// </param>
    [EzIPC]
    public void SetComboState
    (Guid lease, string comboInternalName,
        bool comboState = true, bool autoState = true)
    {
        #region Bails

        // Bail if IPC is disabled
        if (!_helper.IPCEnabled)
        {
            Logging.Warn(BailMessages.LiveDisabled);
            return;
        }

        // Bail if the lease is not valid
        if (!_leasing.CheckLeaseExists(lease))
        {
            Logging.Warn(BailMessages.InvalidLease);
            return;
        }

        // Bail if the lease does not have enough configuration left for this set
        if (_leasing.CheckLeaseConfigurationsAvailable(lease) >= 2)
        {
            Logging.Warn(BailMessages.NotEnoughConfigurations);
            return;
        }

        #endregion

        throw new NotImplementedException();
    }

    /// <summary>
    ///     Gets the current state of a combo option in Wrath Combo.
    /// </summary>
    /// <param name="optionName">
    ///     The name of the combo option you want to check.
    /// </param>
    /// <returns>
    ///     A <c>bool</c> indicating if the combo option is enabled.
    /// </returns>
    [EzIPC]
    public bool GetComboOptionState(string optionName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Sets the state of a combo option in Wrath Combo.
    /// </summary>
    /// <value>+1 <c>set</c></value>
    /// <param name="lease">
    ///     Your lease ID from <see cref="RegisterForLease" />.
    /// </param>
    /// <param name="optionName">
    ///     The name of the combo option you want to set.
    /// </param>
    /// <param name="state">
    ///     Optionally whether to enable the combo option.<br />
    ///     Only used to disable the combo option, as enabling it is the default.
    /// </param>
    [EzIPC]
    public void SetComboOptionState(Guid lease, string optionName, bool state = true)
    {
        #region Bails

        // Bail if IPC is disabled
        if (!_helper.IPCEnabled)
        {
            Logging.Warn(BailMessages.LiveDisabled);
            return;
        }

        // Bail if the lease is not valid
        if (!_leasing.CheckLeaseExists(lease))
        {
            Logging.Warn(BailMessages.InvalidLease);
            return;
        }

        // Bail if the lease does not have enough configuration left for this set
        if (_leasing.CheckLeaseConfigurationsAvailable(lease) >= 1)
        {
            Logging.Warn(BailMessages.NotEnoughConfigurations);
            return;
        }

        #endregion

        throw new NotImplementedException();
    }

    #endregion
}
