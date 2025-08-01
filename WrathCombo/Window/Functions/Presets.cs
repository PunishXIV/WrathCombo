﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ImGuiNET;
using WrathCombo.Attributes;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using ECommons.Throttlers;

namespace WrathCombo.Window.Functions
{
    internal class Presets : ConfigWindow
    {
        internal static Dictionary<CustomComboPreset, PresetAttributes> Attributes = new();
        private static bool _animFrame = false;
        internal class PresetAttributes
        {
            private CustomComboPreset Preset;
            public bool IsPvP;
            public CustomComboPreset[] Conflicts;
            public CustomComboPreset? Parent;
            public BlueInactiveAttribute? BlueInactive;
            public VariantAttribute? Variant;
            public VariantParentAttribute? VariantParent;
            public PossiblyRetargetedAttribute? PossiblyRetargeted;
            public RetargetedAttribute? RetargetedAttribute;
            public uint[] RetargetedActions => 
                GetRetargetedActions(Preset, RetargetedAttribute, PossiblyRetargeted, Parent);
            public BozjaParentAttribute? BozjaParent;
            public EurekaParentAttribute? EurekaParent;
            public OccultCrescentAttribute? OccultCrescentJob;
            public HoverInfoAttribute? HoverInfo;
            public ReplaceSkillAttribute? ReplaceSkill;
            public CustomComboInfoAttribute? CustomComboInfo;
            public AutoActionAttribute? AutoAction;
            public RoleAttribute? RoleAttribute;
            public HiddenAttribute? Hidden;
            public ComboType ComboType;

            public PresetAttributes(CustomComboPreset preset)
            {
                Preset = preset;
                IsPvP = PresetStorage.IsPvP(preset);
                Conflicts = PresetStorage.GetConflicts(preset);
                Parent = PresetStorage.GetParent(preset);
                BlueInactive = preset.GetAttribute<BlueInactiveAttribute>();
                Variant = preset.GetAttribute<VariantAttribute>();
                VariantParent = preset.GetAttribute<VariantParentAttribute>();
                PossiblyRetargeted = preset.GetAttribute<PossiblyRetargetedAttribute>();
                RetargetedAttribute = preset.GetAttribute<RetargetedAttribute>();
                BozjaParent = preset.GetAttribute<BozjaParentAttribute>();
                EurekaParent = preset.GetAttribute<EurekaParentAttribute>();
                OccultCrescentJob = preset.GetAttribute<OccultCrescentAttribute>();
                HoverInfo = preset.GetAttribute<HoverInfoAttribute>();
                ReplaceSkill = preset.GetAttribute<ReplaceSkillAttribute>();
                CustomComboInfo = preset.GetAttribute<CustomComboInfoAttribute>();
                AutoAction = preset.GetAttribute<AutoActionAttribute>();
                RoleAttribute = preset.GetAttribute<RoleAttribute>();
                Hidden = preset.GetAttribute<HiddenAttribute>();
                ComboType = PresetStorage.GetComboType(preset);
            }
        }
        
        private static uint[] GetRetargetedActions
            (CustomComboPreset preset,
                RetargetedAttribute? retargetedAttribute,
                PossiblyRetargetedAttribute? possiblyRetargeted,
                CustomComboPreset? parent)
        {
            // Pick whichever Retargeted attribute is available
            RetargetedAttributeBase? retargetAttribute = null;
            if (retargetedAttribute != null)
                retargetAttribute = retargetedAttribute;
            else if (possiblyRetargeted != null)
                retargetAttribute = possiblyRetargeted;
            
            // Bail if the Preset is not Retargeted
            if (retargetAttribute == null)
                return [];
            
            try {
                // Bail if not actually enabled
                if (!Service.Configuration.EnabledActions.Contains(preset))
                    return [];
                // ReSharper disable once DuplicatedSequentialIfBodies
                if (parent != null &&
                    !Service.Configuration.EnabledActions
                        .Contains((CustomComboPreset)parent))
                    return [];
                if (parent?.Attributes()?.Parent is { } grandParent &&
                    !Service.Configuration.EnabledActions
                        .Contains(grandParent))
                    return [];
            
                // Bail if the Condition for PossiblyRetargeted is not satisfied
                if (retargetAttribute is PossiblyRetargetedAttribute attribute
                    && IsConditionSatisfied(attribute.PossibleCondition) != true)
                    return [];
            }
            catch (Exception e)
            {
                PluginLog.Error($"Failed to check if Preset {preset} is enabled: {e.ToStringFull()}");
                return [];
            }
            
            // Set the Retargeted Actions if all bails are passed
            return retargetAttribute.RetargetedActions;
        }

        internal static Dictionary<CustomComboPreset, bool> GetJobAutorots => P
            .IPCSearch.AutoActions.Where(x => x.Key.Attributes().IsPvP == CustomComboFunctions.InPvP() && (Player.JobId == x.Key.Attributes().CustomComboInfo.JobID || CustomComboFunctions.JobIDs.ClassToJob((byte)Player.Job) == x.Key.Attributes().CustomComboInfo.JobID) && x.Value && CustomComboFunctions.IsEnabled(x.Key) && x.Key.Attributes().Parent == null).ToDictionary();

        internal static void DrawPreset(CustomComboPreset preset, CustomComboInfoAttribute info)
        {
            if (!Attributes.ContainsKey(preset))
            {
                PresetAttributes attributes = new(preset);
                Attributes[preset] = attributes;
            }
            bool enabled = PresetStorage.IsEnabled(preset);
            bool pvp = Attributes[preset].IsPvP;
            var conflicts = Attributes[preset].Conflicts;
            var parent = Attributes[preset].Parent;
            var blueAttr = Attributes[preset].BlueInactive;
            var variantParents = Attributes[preset].VariantParent;
            var bozjaParents = Attributes[preset].BozjaParent;
            var eurekaParents = Attributes[preset].EurekaParent;
            var auto = Attributes[preset].AutoAction;
            var hidden = Attributes[preset].Hidden;

            ImGui.Spacing();

            if (auto != null)
            {
                if (!Service.Configuration.AutoActions.ContainsKey(preset))
                    Service.Configuration.AutoActions[preset] = false;

                var label = "Auto-Mode";
                var labelSize = ImGui.CalcTextSize(label);
                ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X - labelSize.X.Scale() - 64f.Scale());
                bool autoOn = Service.Configuration.AutoActions[preset];
                if (P.UIHelper.ShowIPCControlledCheckboxIfNeeded
                        ($"###AutoAction{preset}", ref autoOn, preset, false))
                {
                    DebugFile.AddLog($"Set Auto-Mode for {preset} to {autoOn}");
                    P.IPCSearch.UpdateActiveJobPresets();
                    Service.Configuration.AutoActions[preset] = autoOn;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                ImGui.Text(label);
                ImGuiComponents.HelpMarker($"Add this feature to Auto-Rotation.\n" +
                    $"Auto-Rotation will automatically use the actions selected within the feature, allowing you to focus on movement. Configure the settings in the 'Auto-Rotation' section.");
                ImGui.Separator();
            }

            if (info.Name.Contains(" - AoE") || info.Name.Contains(" - Sin"))
                if (P.UIHelper.PresetControlled(preset) is not null)
                    P.UIHelper.ShowIPCControlledIndicatorIfNeeded(preset);

            if (P.UIHelper.ShowIPCControlledCheckboxIfNeeded
                    ($"{info.Name}###{preset}", ref enabled, preset, true))
            {
                if (enabled)
                {
                    PresetStorage.EnablePreset(preset);
                }
                else
                {
                    PresetStorage.DisablePreset(preset);
                }
                P.IPCSearch.UpdateActiveJobPresets();
                DebugFile.AddLog($"Set {preset} to {enabled}");

                Service.Configuration.Save();
            }

            DrawReplaceAttribute(preset);

            DrawRetargetedAttribute(preset);

            if (DrawOccultJobIcon(preset))
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 8f.Scale());

            Vector2 length = new();
            using (var styleCol = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
            {
                if (currentPreset != -1)
                {
                    ImGui.Text($"#{currentPreset}: ");
                    length = ImGui.CalcTextSize($"#{currentPreset}: ");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(length.Length());
                }

                ImGui.TextWrapped($"{info.Description}");

                if (Attributes[preset].HoverInfo != null)
                {
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(Attributes[preset].HoverInfo.HoverText);
                        ImGui.EndTooltip();
                    }
                }
            }


            ImGui.Spacing();

            if (conflicts.Length > 0)
            {
                ImGui.TextColored(ImGuiColors.DalamudRed, "Conflicts with:");
                StringBuilder conflictBuilder = new();
                ImGui.Indent();
                foreach (var conflict in conflicts)
                {
                    var comboInfo = Attributes[conflict].CustomComboInfo;
                    conflictBuilder.Insert(0, $"{comboInfo.Name}");
                    var par2 = conflict;

                    while (PresetStorage.GetParent(par2) != null)
                    {
                        var subpar = PresetStorage.GetParent(par2);
                        if (subpar != null)
                        {
                            conflictBuilder.Insert(0, $"{Attributes[subpar.Value].CustomComboInfo.Name} -> ");
                            par2 = subpar!.Value;
                        }

                    }

                    if (!string.IsNullOrEmpty(comboInfo.JobShorthand))
                        conflictBuilder.Insert(0, $"[{comboInfo.JobShorthand}] ");

                    ImGuiEx.Text(GradientColor.Get(ImGuiColors.DalamudRed, CustomComboFunctions.IsEnabled(conflict) ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, 1500), $"- {conflictBuilder}");
                    conflictBuilder.Clear();
                }
                ImGui.Unindent();
                ImGui.Spacing();
            }

            if (blueAttr != null)
            {
                blueAttr.GetActions();
                if (blueAttr.Actions.Count > 0)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, blueAttr.NoneSet ? ImGuiColors.DPSRed : ImGuiColors.DalamudOrange);
                    ImGui.Text($"{(blueAttr.NoneSet ? "No Required Spells Active:" : "Missing active spells:")} {string.Join(", ", blueAttr.Actions.Select(x => ActionWatching.GetBLUIndex(x) + GetActionName(x)))}");
                    ImGui.PopStyleColor();
                }

                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                    ImGui.Text("All required spells active!");
                    ImGui.PopStyleColor();
                }
            }

            if (variantParents is not null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                ImGui.TextWrapped($"Part of normal combo{(variantParents.ParentPresets.Length > 1 ? "s" : "")}:");
                StringBuilder builder = new();
                foreach (var par in variantParents.ParentPresets)
                {
                    builder.Insert(0, $"{(Attributes.ContainsKey(par) ? Attributes[par].CustomComboInfo.Name : par.GetAttribute<CustomComboInfoAttribute>().Name)}");
                    var par2 = par;
                    while (PresetStorage.GetParent(par2) != null)
                    {
                        var subpar = PresetStorage.GetParent(par2);
                        if (subpar != null)
                        {
                            builder.Insert(0, $"{(Attributes.ContainsKey(subpar.Value) ? Attributes[subpar.Value].CustomComboInfo.Name : subpar?.GetAttribute<CustomComboInfoAttribute>().Name)} -> ");
                            par2 = subpar!.Value;
                        }

                    }

                    ImGui.TextWrapped($"- {builder}");
                    builder.Clear();
                }
                ImGui.PopStyleColor();
            }

            if (bozjaParents is not null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                ImGui.TextWrapped($"Part of normal combo{(bozjaParents.ParentPresets.Length > 1 ? "s" : "")}:");
                StringBuilder builder = new();
                foreach (var par in bozjaParents.ParentPresets)
                {
                    builder.Insert(0, $"{(Attributes.ContainsKey(par) ? Attributes[par].CustomComboInfo.Name : par.GetAttribute<CustomComboInfoAttribute>().Name)}");
                    var par2 = par;
                    while (PresetStorage.GetParent(par2) != null)
                    {
                        var subpar = PresetStorage.GetParent(par2);
                        if (subpar != null)
                        {
                            builder.Insert(0, $"{(Attributes.ContainsKey(subpar.Value) ? Attributes[subpar.Value].CustomComboInfo.Name : subpar?.GetAttribute<CustomComboInfoAttribute>().Name)} -> ");
                            par2 = subpar!.Value;
                        }
                    }

                    ImGui.TextWrapped($"- {builder}");
                    builder.Clear();
                }
                ImGui.PopStyleColor();
            }

            if (eurekaParents is not null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                ImGui.TextWrapped($"Part of normal combo{(variantParents.ParentPresets.Length > 1 ? "s" : "")}:");
                StringBuilder builder = new();
                foreach (var par in eurekaParents.ParentPresets)
                {
                    builder.Insert(0, $"{(Attributes.ContainsKey(par) ? Attributes[par].CustomComboInfo.Name : par.GetAttribute<CustomComboInfoAttribute>().Name)}");
                    var par2 = par;
                    while (PresetStorage.GetParent(par2) != null)
                    {
                        var subpar = PresetStorage.GetParent(par2);
                        if (subpar != null)
                        {
                            builder.Insert(0, $"{(Attributes.ContainsKey(subpar.Value) ? Attributes[subpar.Value].CustomComboInfo.Name : subpar?.GetAttribute<CustomComboInfoAttribute>().Name)} -> ");
                            par2 = subpar!.Value;
                        }

                    }

                    ImGui.TextWrapped($"- {builder}");
                    builder.Clear();
                }
                ImGui.PopStyleColor();
            }
            if (enabled)
            {
                if (!pvp)
                {
                    switch (info.JobID)
                    {
                        case All.JobID: All.Config.Draw(preset); break;
                        case AST.JobID: AST.Config.Draw(preset); break;
                        case BLM.JobID: BLM.Config.Draw(preset); break;
                        case BLU.JobID: BLU.Config.Draw(preset); break;
                        case BRD.JobID: BRD.Config.Draw(preset); break;
                        case DNC.JobID: DNC.Config.Draw(preset); break;
                        case DOL.JobID: DOL.Config.Draw(preset); break;
                        case DRG.JobID: DRG.Config.Draw(preset); break;
                        case DRK.JobID: DRK.Config.Draw(preset); break;
                        case GNB.JobID: GNB.Config.Draw(preset); break;
                        case MCH.JobID: MCH.Config.Draw(preset); break;
                        case MNK.JobID: MNK.Config.Draw(preset); break;
                        case NIN.JobID: NIN.Config.Draw(preset); break;
                        case PCT.JobID: PCT.Config.Draw(preset); break;
                        case PLD.JobID: PLD.Config.Draw(preset); break;
                        case RPR.JobID: RPR.Config.Draw(preset); break;
                        case RDM.JobID: RDM.Config.Draw(preset); break;
                        case SAM.JobID: SAM.Config.Draw(preset); break;
                        case SCH.JobID: SCH.Config.Draw(preset); break;
                        case SGE.JobID: SGE.Config.Draw(preset); break;
                        case SMN.JobID: SMN.Config.Draw(preset); break;
                        case VPR.JobID: VPR.Config.Draw(preset); break;
                        case WAR.JobID: WAR.Config.Draw(preset); break;
                        case WHM.JobID: WHM.Config.Draw(preset); break;
                        case OccultCrescent.JobID: OccultCrescent.Config.Draw(preset); break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (info.JobID)
                    {
                        case All.JobID: PvPCommon.Config.Draw(preset); break;
                        case AST.JobID: ASTPvP.Config.Draw(preset); break;
                        case BLM.JobID: BLMPvP.Config.Draw(preset); break;
                        //case BLU.JobID: BLU.Config.Draw(preset); break;
                        case BRD.JobID: BRDPvP.Config.Draw(preset); break;
                        case DNC.JobID: DNCPvP.Config.Draw(preset); break;
                        case DRG.JobID: DRGPvP.Config.Draw(preset); break;
                        case DRK.JobID: DRKPvP.Config.Draw(preset); break;
                        case GNB.JobID: GNBPvP.Config.Draw(preset); break;
                        case MCH.JobID: MCHPvP.Config.Draw(preset); break;
                        case MNK.JobID: MNKPvP.Config.Draw(preset); break;
                        case NIN.JobID: NINPvP.Config.Draw(preset); break;
                        case PCT.JobID: PCTPvP.Config.Draw(preset); break;
                        case PLD.JobID: PLDPvP.Config.Draw(preset); break;
                        case RPR.JobID: RPRPvP.Config.Draw(preset); break;
                        case RDM.JobID: RDMPvP.Config.Draw(preset); break;
                        case SAM.JobID: SAMPvP.Config.Draw(preset); break;
                        case SCH.JobID: SCHPvP.Config.Draw(preset); break;
                        case SGE.JobID: SGEPvP.Config.Draw(preset); break;
                        case SMN.JobID: SMNPvP.Config.Draw(preset); break;
                        case VPR.JobID: VPRPvP.Config.Draw(preset); break;
                        case WAR.JobID: WARPvP.Config.Draw(preset); break;
                        case WHM.JobID: WHMPvP.Config.Draw(preset); break;
                        default:
                            break;
                    }
                }

            }

            ImGui.Spacing();
            currentPreset++;

            presetChildren.TryGetValue(preset, out var children);

            if (children != null)
            {
                if (enabled || !Service.Configuration.HideChildren)
                {
                    ImGui.Indent();

                    foreach (var (childPreset, childInfo) in children)
                    {
                        if (PresetStorage.ShouldBeHidden(childPreset)) continue;

                        presetChildren.TryGetValue(childPreset, out var grandchildren);
                        InfoBox box = new() { HasMaxWidth = true, Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 4f, ContentsAction = () => { DrawPreset(childPreset, childInfo); } };
                        Action draw = grandchildren?.Count() > 0 ? () => box.Draw() : () => DrawPreset(childPreset, childInfo);

                        if (Service.Configuration.HideConflictedCombos)
                        {
                            var conflictOriginals = PresetStorage.GetConflicts(childPreset);    // Presets that are contained within a ConflictedAttribute
                            var conflictsSource = PresetStorage.GetAllConflicts();              // Presets with the ConflictedAttribute

                            if (!conflictsSource.Where(x => x == childPreset || x == preset).Any() || conflictOriginals.Length == 0)
                            {
                                draw();
                                if (grandchildren?.Count() > 0)
                                    ImGui.Spacing();
                                continue;
                            }

                            if (conflictOriginals.Any(PresetStorage.IsEnabled))
                            {
                                if (DateTime.UtcNow - LastPresetDeconflictTime > TimeSpan.FromSeconds(3))
                                {
                                    if (Service.Configuration.EnabledActions.Remove(childPreset))
                                    {
                                        PluginLog.Debug($"Removed {childPreset} due to conflict with {preset}");
                                        Service.Configuration.Save();
                                    }
                                    LastPresetDeconflictTime = DateTime.UtcNow;
                                }

                                // Keep removed items in the counter
                                currentPreset += 1 + AllChildren(presetChildren[childPreset]);
                            }

                            else
                            {
                                draw();
                                if (grandchildren?.Count() > 0)
                                    ImGui.Spacing();
                            }
                        }
                        else
                        {
                            draw();
                            if (grandchildren?.Count() > 0)
                                ImGui.Spacing();
                        }
                    }

                    ImGui.Unindent();
                }
                else
                {
                    currentPreset += AllChildren(presetChildren[preset]);

                }
            }
        }

        private static void DrawReplaceAttribute(CustomComboPreset preset)
        {
            var att = Attributes[preset].ReplaceSkill;
            if (att != null)
            {
                string skills = string.Join(", ", att.ActionNames);

                ImGuiComponents.HelpMarker($"Replaces: {skills}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    foreach (var icon in att.ActionIcons)
                    {
                        var img = Svc.Texture.GetFromGameIcon(new(icon)).GetWrapOrEmpty();
                        ImGui.Image(img.ImGuiHandle, (img.Size / 2f) * ImGui.GetIO().FontGlobalScale);
                        ImGui.SameLine();
                    }
                    ImGui.EndTooltip();
                }
            }
        }

        public static void DrawRetargetedSymbolForSettingsPage() =>
            DrawRetargetedAttribute(
                firstLine: "This Feature will involve retargeting actions if enabled.",
                secondLine: "The actions this Feature affects will automatically be\n" +
                            "targeted onto the targets in the priority you have configured.",
                thirdLine: "Using plugins like Redirect or Reaction with configurations\n" +
                           "affecting the same actions will Conflict and may cause issues.");

        private static void DrawRetargetedAttribute
            (CustomComboPreset? preset = null,
                string? firstLine = null,
                string? secondLine = null,
                string? thirdLine = null)
        {
            // Determine what symbol to show
            var possiblyRetargeted = false;
            bool retargeted;
            if (preset is null)
                retargeted = true;
            else
            {
                possiblyRetargeted =
                    Attributes[preset.Value].PossiblyRetargeted != null;
                retargeted =
                    Attributes[preset.Value].RetargetedAttribute != null;
            }

            if (!possiblyRetargeted && !retargeted) return;

            // Resolved the conditions if possibly retargeted
            if (possiblyRetargeted)
                if (IsConditionSatisfied(Attributes[preset!.Value]
                        .PossiblyRetargeted!.PossibleCondition) == true)
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
                                "This Feature's actions may be retargeted.");
                        if (retargeted)
                            ImGui.TextUnformatted(
                                firstLine ??
                                "This Feature's actions are retargeted.");

                        ImGui.TextUnformatted(
                            secondLine ??
                            "The actions from this Feature will automatically be\n" +
                            "targeted onto what the developers feel is the best target\n" +
                            "(following The Balance where applicable).");

                        ImGui.TextUnformatted(
                            thirdLine ??
                            "Using plugins like Redirect or Reaction with configurations\n" +
                            "affecting this action will Conflict and may cause issues.");

                        var settingInfo = "";
                        if (preset.HasValue)
                            settingInfo =
                                Attributes[preset.Value].PossiblyRetargeted is not
                                    null
                                    ? Attributes[preset.Value].PossiblyRetargeted.SettingInfo
                                    : "";
                        if (settingInfo != "")
                        {
                            ImGui.NewLine();
                            ImGui.TextUnformatted(
                                "The setting that controls if this action is retargeted is:\n" +
                                settingInfo);
                        }
                    }
                }
            }
        }

        private static bool DrawOccultJobIcon(CustomComboPreset preset)
        {
            if (preset.Attributes().OccultCrescentJob == null) return false;
            var jobID = preset.Attributes().OccultCrescentJob.JobId;
            if (jobID == -1) return false;
            
            #region Error Handling
            string? error = null;

            if (_animFrame)
                jobID += 30;

            if (EzThrottler.Throttle("AnimFrameUpdater", 400))
                _animFrame = !_animFrame;

            if (!Icons.OccultIcons.TryGetValue(jobID, out var icon))
                error = "FIND";
            if (icon is null)
                error = "LOAD";
            if (error is not null)
            {
                PluginLog.Error($"Failed to {error} Occult Crescent job icon for Preset:{preset} using JobID:{jobID}");
                return false;
            }
            #endregion

            var iconMaxSize = 32f.Scale();
            ImGui.SameLine();
            var scale = Math.Min(iconMaxSize / icon.Size.X, iconMaxSize / icon.Size.Y);
            var imgSize = new Vector2(icon.Size.X * scale, icon.Size.Y * scale);

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 6f.Scale());
            ImGui.Image(icon.ImGuiHandle, imgSize);
            return true;
        }

        internal static int AllChildren((CustomComboPreset Preset, CustomComboInfoAttribute Info)[] children)
        {
            var output = 0;

            foreach (var (Preset, Info) in children)
            {
                output++;
                output += AllChildren(presetChildren[Preset]);
            }

            return output;
        }
    }
}
