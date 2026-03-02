using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.Throttlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.UI.Features;
using WrathCombo.Resources.Localization.UI.Misc;
using WrathCombo.Resources.Localization.UI.Settings;
using WrathCombo.Services;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using static WrathCombo.Core.PresetStorage;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.CustomComboNS.Functions.Jobs;
namespace WrathCombo.Window.Functions;

internal class Presets : ConfigWindow
{

    private static bool _animFrame = false;

    internal static Dictionary<Preset, bool> GetJobAutorots
    {
        get
        {
            var autoActions = P.IPCSearch.AutoActions;

            return autoActions
                .Where(kvp =>
                {
                    var preset = kvp.Key;
                    var attrs = preset.Attributes();

                    // PvP check
                    if (attrs.IsPvP != CustomComboFunctions.InPvP())
                        return false;

                    // Job check (including upgraded job)
                    var job = attrs.JobInfo.Job;
                    if (Player.Job != job && Player.Job.GetUpgradedJob() != job)
                        return false;

                    // Enabled & active
                    if (!kvp.Value || !CustomComboFunctions.IsEnabled(preset))
                        return false;

                    // Only top-level presets
                    if (attrs.Parent != null)
                        return false;

                    return true;
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    internal static void DrawPreset(Preset preset, PresetData presetData)
    {
        bool enabled = PresetStorage.IsEnabled(preset);
        var conflicts = presetData.Conflicts;
        var parent = presetData.Parent;
        var blueAttr = presetData.BlueInactive;
        var presetName = presetData.Name;
        var comboType = presetData.ComboType;

        ImGui.Spacing();

        if (presetData.AutoAction != null && (!presetData.IsPvP || HiddenFeaturesData.FeaturesEnabled))
        {
            Service.Configuration.AutoActions.TryAdd(preset, false);

            var label = "Auto-Mode";
            var labelSize = ImGui.CalcTextSize(label);
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X - labelSize.X.Scale() - 64f.Scale());
            bool autoOn = Service.Configuration.AutoActions[preset];
            if (P.UIHelper.ShowIPCControlledCheckboxIfNeeded
                ($"###AutoAction{preset}", ref autoOn, preset, false))
                PresetStorage.ToggleAutoModeForPreset(preset);
            ImGui.SameLine();
            ImGui.Text(label);
            ImGuiComponents.HelpMarker(FeaturesUI.Hover_AutoMode);
            ImGui.Separator();
        }

        var ipcControl = P.UIHelper.PresetControlled(preset);
        if (ipcControl is not null)
            enabled = ipcControl.Value.enabled;

        if (comboType is (ComboType.Advanced or ComboType.Simple))
            if (ipcControl is not null)
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded(preset);

        if (IsSearching)
            presetName = preset.NameWithFullLineage(presetData.JobInfo.Job);

        if (P.UIHelper.ShowIPCControlledCheckboxIfNeeded
            ($"{presetName}###{preset}", ref enabled, preset, true))
            PresetStorage.TogglePreset(preset);

        DrawReplaceAttribute(presetData);

        DrawRetargetedAttribute(presetData);

        if (DrawRoleIcon(presetData))
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 8f.Scale());

        if (DrawOccultJobIcon(presetData))
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 8f.Scale());

        Vector2 length = new();
        using (var styleCol = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
        {
            if (FeaturesWindow.CurrentPreset != -1)
            {
                string idForShow;
                if (Service.Configuration.UIShowPresetIDs)
                {
                    var idToShow = ((int)preset).ToString();
                    idForShow = $"#{idToShow}:".PadLeft(8);
                }
                else
                {
                    idForShow = " ".PadLeft(10);
                }
                ImGui.Text(idForShow);
                length = ImGui.CalcTextSize(idForShow);
                ImGui.SameLine();
                ImGui.PushItemWidth(length.Length());
            }

            ImGui.TextWrapped($"{presetData.Description}");

            if (presetData.HoverText != null)
            {
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(presetData.HoverText);
                    ImGui.EndTooltip();
                }
            }
        }


        ImGui.Spacing();

        if (conflicts.Length > 0)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, FeaturesUI.Label_ConflictsWith);
            ImGui.Indent();
            foreach (var conflict in conflicts)
                ImGuiEx.Text(GradientColor.Get(
                        ImGuiColors.DalamudRed,
                        CustomComboFunctions.IsEnabled(conflict)
                            ? ImGuiColors.HealerGreen
                            : ImGuiColors.DalamudRed, 1500),
                    $"- {conflict.NameWithFullLineage(presetData.JobInfo.Job)}");
            ImGui.Unindent();
            ImGui.Spacing();
        }

        if (blueAttr != null)
        {
            blueAttr.GetActions();
            if (blueAttr.Actions.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, blueAttr.NoneSet ? ImGuiColors.DPSRed : ImGuiColors.DalamudOrange);
                ImGui.Text($"{(blueAttr.NoneSet ? FeaturesUI.Warning_BLUNoSpells : FeaturesUI.Warning_BLUMissingSpells)} {string.Join(", ", blueAttr.Actions.Select(x => ActionWatching.GetBLUIndex(x) + GetActionName(x)))}");
                ImGui.PopStyleColor();
            }

            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                ImGui.Text(FeaturesUI.Info_BLUAllGoodSpells);
                ImGui.PopStyleColor();
            }
        }

        // Draw UserOpts
        if (enabled)
        {
            if (!presetData.IsPvP)
            {
                switch (presetData.JobInfo.Job)
                {
                    case Job.ADV:
                        {
                            All.Config.Draw(preset);
                            Bozja.Config.Draw(preset);
                            Variant.Config.Draw(preset);
                            OccultCrescent.Config.Draw(preset);
                            break;
                        }
                    case Job.AST: AST.Config.Draw(preset); break;
                    case Job.BLM: BLM.Config.Draw(preset); break;
                    case Job.BLU: BLU.Config.Draw(preset); break;
                    case Job.BRD: BRD.Config.Draw(preset); break;
                    case Job.DNC: DNC.Config.Draw(preset); break;
                    case Job.MIN: DOL.Config.Draw(preset); break;
                    case Job.DRG: DRG.Config.Draw(preset); break;
                    case Job.DRK: DRK.Config.Draw(preset); break;
                    case Job.GNB: GNB.Config.Draw(preset); break;
                    case Job.MCH: MCH.Config.Draw(preset); break;
                    case Job.MNK: MNK.Config.Draw(preset); break;
                    case Job.NIN: NIN.Config.Draw(preset); break;
                    case Job.PCT: PCT.Config.Draw(preset); break;
                    case Job.PLD: PLD.Config.Draw(preset); break;
                    case Job.RPR: RPR.Config.Draw(preset); break;
                    case Job.RDM: RDM.Config.Draw(preset); break;
                    case Job.SAM: SAM.Config.Draw(preset); break;
                    case Job.SCH: SCH.Config.Draw(preset); break;
                    case Job.SGE: SGE.Config.Draw(preset); break;
                    case Job.SMN: SMN.Config.Draw(preset); break;
                    case Job.VPR: VPR.Config.Draw(preset); break;
                    case Job.WAR: WAR.Config.Draw(preset); break;
                    case Job.WHM: WHM.Config.Draw(preset); break;
                    default:
                        break;
                }
            }
            else
            {
                switch (presetData.JobInfo.Job)
                {
                    case Job.ADV: PvPCommon.Config.Draw(preset); break;
                    case Job.AST: ASTPvP.Config.Draw(preset); break;
                    case Job.BLM: BLMPvP.Config.Draw(preset); break;
                    case Job.BRD: BRDPvP.Config.Draw(preset); break;
                    case Job.DNC: DNCPvP.Config.Draw(preset); break;
                    case Job.DRG: DRGPvP.Config.Draw(preset); break;
                    case Job.DRK: DRKPvP.Config.Draw(preset); break;
                    case Job.GNB: GNBPvP.Config.Draw(preset); break;
                    case Job.MCH: MCHPvP.Config.Draw(preset); break;
                    case Job.MNK: MNKPvP.Config.Draw(preset); break;
                    case Job.NIN: NINPvP.Config.Draw(preset); break;
                    case Job.PCT: PCTPvP.Config.Draw(preset); break;
                    case Job.PLD: PLDPvP.Config.Draw(preset); break;
                    case Job.RPR: RPRPvP.Config.Draw(preset); break;
                    case Job.RDM: RDMPvP.Config.Draw(preset); break;
                    case Job.SAM: SAMPvP.Config.Draw(preset); break;
                    case Job.SCH: SCHPvP.Config.Draw(preset); break;
                    case Job.SGE: SGEPvP.Config.Draw(preset); break;
                    case Job.SMN: SMNPvP.Config.Draw(preset); break;
                    case Job.VPR: VPRPvP.Config.Draw(preset); break;
                    case Job.WAR: WARPvP.Config.Draw(preset); break;
                    case Job.WHM: WHMPvP.Config.Draw(preset); break;
                    default:
                        break;
                }
            }

        }

        ImGui.Spacing();
        FeaturesWindow.CurrentPreset++;

        presetChildren.TryGetValue(preset, out var children);

        if (children != null)
        {
            if (enabled || !Service.Configuration.HideChildren)
            {
                ImGui.Indent();

                foreach (var (childPreset, childInfo) in children)
                {
                    if (childInfo.ShouldBeHidden) continue;

                    if (presetChildren.TryGetValue(childPreset, out var grandchildren))
                    {
                        InfoBox box = new() { HasMaxWidth = true, CurveRadius = 4f, ContentsAction = () => { DrawPreset(childPreset, childInfo); } };
                        Action draw = grandchildren.Length > 0 && CustomComboFunctions.IsEnabled(childPreset) && Service.Configuration.ShowBorderAroundOptionsWithChildren
                            ? () => box.Draw()
                            : () => DrawPreset(childPreset, childInfo);

                        if (Service.Configuration.HideConflictedCombos)
                        {
                            var conflictOriginals = childInfo.Conflicts;    // Presets that are contained within a ConflictedAttribute

                            if (!ConflictingCombos.Where(x => x == childPreset || x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                draw();
                                if (grandchildren.Length > 0)
                                    ImGui.Spacing();
                                continue;
                            }

                            if (conflictOriginals.Any(CustomComboFunctions.IsEnabled))
                            {
                                // Keep conflicted items in the counter
                                FeaturesWindow.CurrentPreset += 1 + AllChildren(presetChildren[childPreset].ToArray());
                            }
                            else
                            {
                                draw();
                                if (grandchildren.Length > 0)
                                    ImGui.Spacing();
                            }
                        }
                        else
                        {
                            draw();
                            if (grandchildren.Length > 0)
                                ImGui.Spacing();
                        }
                    }
                }

                ImGui.Unindent();
            }
            else
            {
                FeaturesWindow.CurrentPreset += AllChildren(presetChildren[preset].ToArray());

            }
        }
    }

    private static void DrawReplaceAttribute(PresetData presetData)
    {
        if (presetData.ReplaceSkill is ReplaceSkillAttribute att)
        {
            string skills = string.Join(", ", att.ActionNames);

            ImGuiComponents.HelpMarker($"{MiscUI.Replaces}: {skills}");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                foreach (var icon in att.ActionIcons)
                {
                    var img = Svc.Texture.GetFromGameIcon(new(icon)).GetWrapOrEmpty();
                    ImGui.Image(img.Handle, (img.Size / 2f) * ImGui.GetIO().FontGlobalScale);
                    ImGui.SameLine();
                }
                ImGui.EndTooltip();
            }
        }
    }

    public static void DrawRetargetedSymbolForSettingsPage() =>
        DrawRetargetedAttribute(
            firstLine: SettingsUI.Hover_Retargetting1,
            secondLine: SettingsUI.Hover_Retargetting2,
            thirdLine: SettingsUI.Hover_Retargetting3);


    private static void DrawRetargetedAttribute
    (PresetData? presetdata = null,
        string? firstLine = null,
        string? secondLine = null,
        string? thirdLine = null)
    {
        // Determine what symbol to show
        var possiblyRetargeted = false;
        bool retargeted;
        if (presetdata is null)
            retargeted = true;
        else
        {
            possiblyRetargeted = presetdata.PossiblyRetargeted != null;
            retargeted = presetdata.RetargetedAttribute != null;
        }

        if (!possiblyRetargeted && !retargeted) return;

        // Resolved the conditions if possibly retargeted
        if (possiblyRetargeted)
            if (IsConditionSatisfied(presetdata.PossiblyRetargeted!.PossibleCondition) == true)
            {
                retargeted = true;
                possiblyRetargeted = false;
            }

        ImGui.SameLine();

        // Color the icon for whether it is possibly or certainly retargeted
        var color = retargeted
            ? ImGuiColors.ParsedGreen
            : ImGuiColors.DalamudYellow;

        using var col = new ImRaii.Color();
        col.Push(ImGuiCol.TextDisabled, color);

        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            ImGui.TextDisabled(FontAwesomeIcon.Random.ToIconString());
        }

        if (ImGui.IsItemHovered())
        {
            using (ImRaii.Tooltip())
            {
                using (ImRaii.TextWrapPos(ImGui.GetFontSize() * 35.0f))
                {
                    if (possiblyRetargeted)
                        ImGui.TextUnformatted(
                            FeaturesUI.Hover_Retargetting_MaybeRetargetted);
                    if (retargeted)
                        ImGui.TextUnformatted(
                            firstLine ??
                            FeaturesUI.Hover_Retargetting_IsRetargetted);

                    ImGui.TextUnformatted(
                        secondLine ??
                        FeaturesUI.Hover_Retargetting_Line2);

                    ImGui.TextUnformatted(
                        thirdLine ??
                        FeaturesUI.Hover_Retargetting_Line3);

                    var settingInfo = "";
                    if (presetdata is not null)
                        settingInfo =
                            presetdata.PossiblyRetargeted is not
                                null
                                ? presetdata.PossiblyRetargeted.SettingInfo
                                : "";
                    if (settingInfo != "")
                    {
                        ImGui.NewLine();
                        ImGui.TextUnformatted(
                            $"{FeaturesUI.Hover_Retargetting_ControllingSetting}\n" +
                            settingInfo);
                    }
                }
            }
        }
    }

    private static bool DrawRoleIcon(PresetData presetData)
    {
        if (presetData.JobInfo.RoleForIcon is not JobRole role) return false;
        if (presetData.Parent != null) return false;
        //if (jobID == -1) return false;
        var icon = Icons.Role.GetRoleIcon(role);
        if (icon is null) return false;
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3f.Scale());
        ImGui.Image(icon.Handle, (icon.Size / 2f) * ImGui.GetIO().FontGlobalScale);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f.Scale());
        return true;
    }

    private static bool DrawOccultJobIcon(PresetData? presetData, int? jobID = null)
    {
        int baseJobID;
        if (presetData is { } realPresetData)
        {
            if (realPresetData.OccultCrescentJob == null) return false;
            baseJobID = realPresetData.OccultCrescentJob.JobId;
            if (baseJobID == -1) return false;
        }
        else if (jobID is not null)
            baseJobID = jobID.Value;
        else
            return false;

        #region Error Handling
        string? error = null;

        // Flip _animFrame every 400ms via throttler
        if (EzThrottler.Throttle("AnimFrameUpdater", 400))
            _animFrame = !_animFrame;

        if (!Icons.Occult.JobSprites.Value.TryGetValue(baseJobID, out var frames))
            error = "FIND";

        if (frames is null || frames.Length < 2)
            error = "LOAD";

        var icon = (error == null) ? frames[_animFrame ? 1 : 0] : null;

        if (icon is null)
            error = "LOAD";

        if (error is not null)
        {
            PluginLog.Error($"Failed to {error} Occult Crescent job icon for Preset:{presetData.Preset} using JobID:{baseJobID}");
            return false;
        }
        #endregion

        var iconMaxSize = 32f.Scale();
        ImGui.SameLine();
        var scale = Math.Min(iconMaxSize / icon.Size.X, iconMaxSize / icon.Size.Y);
        var imgSize = new Vector2(icon.Size.X * scale, icon.Size.Y * scale);

        if (jobID is not null)
            imgSize *= 3f;

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 6f.Scale());
        ImGui.Image(icon.Handle, imgSize);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 6f.Scale());
        return true;
    }

    internal static void DrawOccultJobIcon(int jobID) =>
        DrawOccultJobIcon(null, jobID);

    internal static int AllChildren((Preset preset, PresetData presetData)[] children)
    {
        var output = 0;

        foreach (var (preset, presetData) in children)
        {
            output++;
            output += AllChildren(presetChildren[preset].ToArray());
        }

        return output;
    }
}