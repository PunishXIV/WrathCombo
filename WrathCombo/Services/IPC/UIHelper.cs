#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using WrathCombo.AutoRotation;
using WrathCombo.Combos;
using WrathCombo.CustomComboNS.Functions;

#endregion

namespace WrathCombo.Services.IPC;

public class UIHelper(ref Leasing leasing, ref Search search)
{
    private readonly Leasing _leasing = leasing;
    private readonly Search _search = search;

    #region Checks for the UI

    #region Auto-Rotation

    private DateTime? _autoRotationUpdated;

    private (string controllers, bool state)
        AutoRotationControlled { get; set; } = (string.Empty, false);

    internal (string controllers, bool state)? AutoRotationStateControlled()
    {
        // Return the cached value if it is valid, fastest
        if (string.IsNullOrEmpty(AutoRotationControlled.controllers) &&
            _autoRotationUpdated is not null &&
            _autoRotationUpdated == _leasing.AutoRotationStateUpdated)
            return AutoRotationControlled;

        // Bail if the state is not controlled, fast
        var controlled = _leasing.CheckAutoRotationControlled();
        if (controlled is null)
            return null;

        // Re-populate the cache with the current state, slowest
        var controllers = _leasing.Registrations.Values
            .Where(l => l.AutoRotationControlled.Count != 0)
            .OrderByDescending(l => l.LastUpdated)
            .ToList();
        var controllingLeases = controllers
            .Select(l => l.PluginName);
        var controlledState = controllers
            .First().AutoRotationControlled[0];
        AutoRotationControlled =
            (string.Join(", ", controllingLeases), controlledState);
        _autoRotationUpdated = _leasing.AutoRotationStateUpdated;

        return AutoRotationControlled;
    }

    #endregion

    #region Jobs

    private DateTime? _jobsUpdated;

    private Dictionary<string, (string controllers, bool state)>
        JobsControlled { get; } = new();

    internal (string controllers, bool state)? JobControlled(uint job)
    {
        var jobName = CustomComboFunctions.JobIDs.JobIDToShorthand(job);

        if (_jobsUpdated != _leasing.JobsUpdated)
            JobsControlled.Clear();

        // Return the cached value if it is valid, fastest
        if (_jobsUpdated is not null &&
            _jobsUpdated == _leasing.JobsUpdated &&
            JobsControlled.TryGetValue(jobName, out var jobControlled))
        {
            if (string.IsNullOrEmpty(jobControlled.controllers))
                return null;
            return jobControlled;
        }

        // Bail if the job is not controlled, fast
        if ((JobsControlled.TryGetValue(jobName, out var jobNotControlled) &&
             string.IsNullOrEmpty(jobNotControlled.controllers)) ||
            _leasing.CheckJobControlled((int)job) is null)
        {
            if (string.IsNullOrEmpty(jobNotControlled.controllers))
            {
                JobsControlled[jobName] = (string.Empty, false);
                _jobsUpdated = _leasing.JobsUpdated;
            }

            return null;
        }

        // Re-populate the cache with the current set of controlled jobs, slowest
        JobsControlled.Clear();
        foreach (var jobListing in Enum.GetValues(typeof(Job)))
            JobsControlled[jobListing.ToString()!] = (string.Empty, false);
        foreach (var controlledJob in _search.AllJobsControlled)
            JobsControlled[controlledJob.Key.ToString()] =
                (string.Join(", ", controlledJob.Value.Keys), true);
        _jobsUpdated = _leasing.JobsUpdated;

        return JobsControlled[jobName];
    }

    #endregion

    #region Presets

    private DateTime? _presetsUpdated;

    private Dictionary<string, (string controllers, bool enabled, bool autoMode)>
        PresetsControlled { get; } = new();

    internal (string controllers, bool enabled, bool autoMode)?
        PresetControlled(CustomComboPreset preset)
    {
        var presetName = preset.ToString();

        var presetsUpdated = (DateTime)
            (_leasing.CombosUpdated > _leasing
                .OptionsUpdated
                ? _leasing.CombosUpdated
                : _leasing.OptionsUpdated ?? DateTime.MinValue);

        if (_presetsUpdated != presetsUpdated)
            PresetsControlled.Clear();

        // Return the cached value if it is valid, fastest
        if (_presetsUpdated is not null &&
            _presetsUpdated == presetsUpdated &&
            PresetsControlled.TryGetValue(presetName, out var presetControlled))
        {
            if (string.IsNullOrEmpty(presetControlled.controllers))
                return null;
            return presetControlled;
        }

        // Bail if the preset is not controlled, fast-ish
        if ((PresetsControlled.TryGetValue(presetName,
                 out var presetNotControlled) &&
             string.IsNullOrEmpty(presetNotControlled.controllers)) ||
            (_leasing.CheckComboControlled(presetName) is null &&
             _leasing.CheckComboOptionControlled(presetName) is null))
        {
            if (string.IsNullOrEmpty(presetNotControlled.controllers))
            {
                PresetsControlled[presetName] =
                    (string.Empty, false, false);
                _presetsUpdated = presetsUpdated;
            }

            return null;
        }

        // Re-populate the cache with the current set of controlled presets, slowest
        PresetsControlled.Clear();
        foreach (var controlledPreset in _search.AllPresetsControlled)
            PresetsControlled[controlledPreset.Key.ToString()] =
                (string.Join(", ", controlledPreset.Value.Keys),
                    controlledPreset.Value.Values.First().enabled,
                    controlledPreset.Value.Values.First().autoMode);

        if (!PresetsControlled.ContainsKey(presetName))
            PresetsControlled[presetName] =
                (string.Empty, false, false);
        _presetsUpdated = presetsUpdated;

        return PresetsControlled[presetName];
    }

    #endregion

    #region Auto-Rotation Configs

    private DateTime? _autoRotationConfigsUpdated;

    private Dictionary<string, (string controllers, int state)>
        AutoRotationConfigsControlled { get; } = new();

    internal (string controllers, int state)? AutoRotationConfigControlled(
        string configName)
    {
        var configOption = Enum.Parse<AutoRotationConfigOption>(configName);

        if (_autoRotationConfigsUpdated != _leasing.AutoRotationConfigsUpdated)
            AutoRotationConfigsControlled.Clear();

        // Return the cached value if it is valid, fastest
        if (_autoRotationConfigsUpdated is not null &&
            _autoRotationConfigsUpdated == _leasing.AutoRotationConfigsUpdated &&
            AutoRotationConfigsControlled.TryGetValue(configName,
                out var configControlled))
        {
            if (string.IsNullOrEmpty(configControlled.controllers))
                return null;
            return AutoRotationConfigsControlled[configName];
        }

        // Bail if the config is not controlled, fast-ish
        if ((AutoRotationConfigsControlled.TryGetValue(configName,
                 out var configNotControlled) &&
             string.IsNullOrEmpty(configNotControlled.controllers)) ||
            _leasing.CheckAutoRotationConfigControlled(configOption) is null)
        {
            if (string.IsNullOrEmpty(configNotControlled.controllers))
            {
                AutoRotationConfigsControlled[configName] = (string.Empty, 0);
                _autoRotationConfigsUpdated = _leasing.AutoRotationConfigsUpdated;
            }

            return null;
        }

        // Re-populate the cache with the current set of controlled configs, slowest
        AutoRotationConfigsControlled.Clear();
        foreach (var controlledConfig in _search.AllAutoRotationConfigsControlled)
        {
            var controllers = string.Join(", ", controlledConfig.Value.Keys);
            var state = controlledConfig.Value.Values.First();
            AutoRotationConfigsControlled[controlledConfig.Key.ToString()] =
                (controllers, state);
        }

        _autoRotationConfigsUpdated = _search.LastCacheUpdateForAutoRotationConfigs;

        return AutoRotationConfigsControlled[configName];
    }

    #endregion

    #endregion

    #region Helper methods for the UI

    #region Style Variables

    private readonly Vector4 _backgroundColor = new(0.68f, 0.77f, 0.80f, 1);
    private readonly Vector4 _hoverColor = new(0.84f, 0.92f, 0.96f, 1);
    private readonly Vector4 _textColor = new(0.05f, 0.05f, 0.05f, 1);
    private readonly Vector2 _padding = new(14f.Scale(), 2f);
    private readonly Vector2 _spacing = new(0, 0);
    private readonly float _rounding = 8f;

    #endregion

    #region Tooltips

    private const string IndicatorTooltip =
        "This option is controlled by another plugin.\n" +
        "Click the X to revoke control.";

    private const string OptionTooltip =
        "This option is controlled by another plugin.\n" +
        "There is a 'Controlled by:' label above this option,\n" +
        "which lists the plugins controlling this option,\n" +
        "and a 'X' button to revoke control.";

    #endregion

    internal int ShowNumberOfLeasees () => _leasing.Registrations.Count;

    internal (string pluginName, int configurationsCount)[] ShowLeasees () =>
        _leasing.Registrations.Values
            .Select(l => (l.PluginName, l.SetsLeased))
            .ToArray();

    // Method to display the controlled indicator, which lists the plugins
    private bool ShowIPCControlledIndicator
    (bool? forAutoRotation = null,
        uint? forJob = null,
        CustomComboPreset? forPreset = null,
        string? forAutoRotationConfig = null,
        bool showX = true)
    {
        (string controllers, object state)? controlled = null;
        var revokeID = "RevokeControl";

        #region Bail if not needed

        if (forAutoRotation is not null)
        {
            if ((controlled = AutoRotationStateControlled()) is null)
                return false;
            revokeID += "ar" + forAutoRotation;
        }

        if (forJob is not null)
        {
            if ((controlled = JobControlled((uint)forJob)) is null)
                return false;
            revokeID += "jb" + forJob;
        }

        if (forPreset is not null)
        {
            var check = PresetControlled((CustomComboPreset)forPreset);
            if (check is null)
                return false;
            controlled = (check.Value.controllers, check.Value.enabled);
            revokeID += "pr" + forPreset;
        }

        if (forAutoRotationConfig is not null)
        {
            if ((controlled =
                    AutoRotationConfigControlled(forAutoRotationConfig)) is null)
                return false;
            revokeID += "ac" + forAutoRotationConfig;
        }

        if (controlled is null)
            return false;

        #endregion

        revokeID += controlled.Value.controllers + controlled.Value.state;

        ImGui.BeginGroup();
        ImGui.PushStyleColor(ImGuiCol.Button, _backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Text, _textColor);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, _padding);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, _rounding);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, _spacing);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);

        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, _backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, _backgroundColor);
        ImGui.SmallButton($"Controlled by: {controlled.Value.controllers}");
        ImGui.PopStyleColor(2);

        if (showX)
        {
            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, _hoverColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, _hoverColor);
            ImGui.PushStyleColor(ImGuiCol.Text, _textColor);
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - _rounding.Scale() -
                                3f.Scale());
            if (ImGui.SmallButton("X###" + revokeID))
                RevokeControl(controlled.Value.controllers);
            ImGui.PopStyleColor(3);

            ImGui.PopStyleVar(4);
            ImGui.PopStyleColor(2);
            ImGui.EndGroup();

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(IndicatorTooltip);
        }
        else
        {
            ImGui.PopStyleVar(4);
            ImGui.PopStyleColor(2);
            ImGui.EndGroup();

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    IndicatorTooltip.Split("X")[0]
                    + "Job to revoke control inside.");
        }

        return true;
    }

    private DateTime _lastLogged = DateTime.MinValue;
    // Method to display a differently-styled and disabled checkbox if controlled
    private bool ShowIPCControlledCheckbox
    (string label, ref bool backupVar,
        bool? forAutoRotation = null,
        uint? forJob = null,
        CustomComboPreset? forPreset = null,
        bool presetShowState = true,
        string? forAutoRotationConfig = null)
    {
        (string controllers, bool state)? controlled = null;

        #region Bail if not needed

        if (forAutoRotation is not null)
            controlled = AutoRotationStateControlled();
        if (forJob is not null)
            controlled = JobControlled((uint)forJob);
        if (forPreset is not null)
        {
            var check =
                PresetControlled((CustomComboPreset)forPreset);
            if (check is null)
                controlled = null;
            else
            {
                controlled = presetShowState
                    ? (check?.controllers ?? "", check?.enabled ?? false)
                    : (check?.controllers ?? "", check?.autoMode ?? false);
            }
        }

        if (forAutoRotationConfig is not null)
        {
            var check = AutoRotationConfigControlled(forAutoRotationConfig);
            if (check is null)
                controlled = null;
            else
                controlled = (check.Value.controllers, check.Value.state == 1);
        }

        if (controlled is null)
        {
            return ImGui.Checkbox(
                label.Contains("Auto") ? "Auto-Mode" : label, ref backupVar);
        }

        #endregion

        ImGui.BeginGroup();
        ImGui.PushStyleColor(ImGuiCol.FrameBg, _backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.CheckMark, _textColor);

        ImGui.BeginDisabled();
        var hold = false;
        var _ = controlled.Value.state;
        if (forPreset is null)
        {
            ImGui.Checkbox("", ref _);
        }
        else
            if (label.StartsWith('#'))
                hold = ImGui.Checkbox(label, ref _);
            else
                hold = ImGui.Checkbox("", ref _);
        ImGui.EndDisabled();

        if (forPreset is null)
        {
            ImGui.SameLine();
            ImGuiHelpers.SafeTextColoredWrapped(ImGuiColors.DalamudGrey, label);
        }
        else
        {
            ImGui.SameLine();
            ImGuiHelpers.SafeTextColoredWrapped(ImGuiColors.DalamudGrey,
                label.Contains("Auto") ? "Auto-Mode" : label.Split('#')[0]);
        }

        ImGui.PopStyleColor(2);
        ImGui.EndGroup();

        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(OptionTooltip);

        return hold;
    }

    private bool ShowIPCControlledSlider
        (string label, ref int backupVar, string? forAutoRotationConfig = null)
    {
        (string controllers, int state)? controlled = null;

        #region Bail if not needed

        if (forAutoRotationConfig is not null)
            controlled = AutoRotationConfigControlled(forAutoRotationConfig);

        if (controlled is null)
        {
            ImGui.SetNextItemWidth(200f.Scale());
            return ImGuiEx.SliderInt(
                label, ref backupVar, 1, 99, "%d%%");
        }

        #endregion

        ImGui.BeginGroup();
        ImGui.PushStyleColor(ImGuiCol.FrameBg, _backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Text, _textColor);
        ImGui.PushStyleColor(ImGuiCol.SliderGrab, _backgroundColor);

        var _ = controlled.Value.state;
        ImGui.BeginDisabled();
        ImGui.SetNextItemWidth(200f.Scale());
        ImGuiEx.SliderInt("", ref _, 1, 99, "%d%%");
        ImGui.EndDisabled();

        ImGui.SameLine();
        ImGuiHelpers.SafeTextColoredWrapped(ImGuiColors.DalamudGrey, label);

        ImGui.PopStyleColor(3);
        ImGui.EndGroup();

        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(OptionTooltip);

        return false;
    }

    private bool ShowIPCControlledCombo
    (string label, bool useDPSVar,
        ref DPSRotationMode dpsVar, ref HealerRotationMode healVar,
        string? forAutoRotationConfig = null)
    {
        (string controller, int state)? controlled = null;


        #region Bail if not needed

        if (forAutoRotationConfig is not null)
            controlled = AutoRotationConfigControlled(forAutoRotationConfig);

        if (controlled is null)
        {
            return useDPSVar
                ? ImGuiEx.EnumCombo(label, ref dpsVar)
                : ImGuiEx.EnumCombo(label, ref healVar);
        }

        #endregion

        ImGui.PushStyleColor(ImGuiCol.FrameBg, _backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Text, _textColor);

        var option = Enum.Parse<AutoRotationConfigOption>(forAutoRotationConfig!);
        var valueType = option.GetType()
            .GetField(option.ToString())
            .GetCustomAttributes(typeof(ConfigValueTypeAttribute), false)
            .Cast<ConfigValueTypeAttribute>()
            .First().ValueType!;
        var value =
            Enum.Parse(valueType, controlled.Value.state.ToString());
        string valueString = value.ToString()!.Replace("_", " ");
        string[] values = [valueString];

        var _ = 0;
        ImGui.BeginDisabled();
        ImGui.Combo("", ref _, values, values.Length);
        ImGui.EndDisabled();

        ImGui.PopStyleColor(2);

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
            ImGui.SetTooltip(OptionTooltip);

        return false;
    }

    /// <summary>
    ///     Button click method for Indicator to cancel plugin control.
    /// </summary>
    /// <param name="controllers">The displayed list of plugins to revoke.</param>
    private void RevokeControl(string controllers)
    {
        var controllerNames = controllers.Split(", ");
        var leases = _leasing.Registrations.Values
            .Where(l => controllerNames.Contains(l.PluginName))
            .Select(l => l.ID)
            .ToList();
        foreach (var lease in leases)
            _leasing.RemoveRegistration(
                lease, CancellationReason.WrathUserManuallyCancelled);
    }

    #region Actual UI Method overloads

    #region Indicator

    public bool ShowIPCControlledIndicatorIfNeeded() =>
        ShowIPCControlledIndicator(forAutoRotation: true);

    public bool ShowIPCControlledIndicatorIfNeeded(uint job, bool showX = true) =>
        ShowIPCControlledIndicator(forJob: job, showX: showX);

    public bool ShowIPCControlledIndicatorIfNeeded(CustomComboPreset preset) =>
        ShowIPCControlledIndicator(forPreset: preset);

    public bool ShowIPCControlledIndicatorIfNeeded(string configName) =>
        ShowIPCControlledIndicator(forAutoRotationConfig: configName);

    #endregion

    #region Disabled Inputs

    public bool ShowIPCControlledCheckboxIfNeeded
        (string label, ref bool backupVar) =>
        ShowIPCControlledCheckbox(label, ref backupVar, forAutoRotation: true);

    public bool ShowIPCControlledCheckboxIfNeeded
        (string label, ref bool backupVar, uint job) =>
        ShowIPCControlledCheckbox(label, ref backupVar, forJob: job);

    public bool ShowIPCControlledCheckboxIfNeeded
    (string label, ref bool backupVar, CustomComboPreset preset,
        bool showState) =>
        ShowIPCControlledCheckbox
            (label, ref backupVar, forPreset: preset, presetShowState: showState);

    public bool ShowIPCControlledCheckboxIfNeeded
        (string label, ref bool backupVar, string configName) =>
        ShowIPCControlledCheckbox(label, ref backupVar,
            forAutoRotationConfig: configName);

    public bool ShowIPCControlledSliderIfNeeded
        (string label, ref int backupVar, string configName) =>
        ShowIPCControlledSlider(
            label, ref backupVar, forAutoRotationConfig: configName);

    public bool ShowIPCControlledComboIfNeeded
    (string label, bool useDPSVar,
        ref DPSRotationMode dpsVar, ref HealerRotationMode healVar,
        string? configName = null) =>
        ShowIPCControlledCombo(
            label, useDPSVar, ref dpsVar, ref healVar,
            forAutoRotationConfig: configName);

    #endregion

    #endregion

    #endregion
}
