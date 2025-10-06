using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons;
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
using System.Text;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.Attributes.PossiblyRetargetedAttribute;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Window.Functions;

internal class Presets : ConfigWindow
{
    internal static Dictionary<Preset, PresetAttributes> Attributes = new();
    private static bool _animFrame = false;
    internal class PresetAttributes
    {
        private Preset Preset;
        public bool IsPvP;
        public Preset[] Conflicts;
        public Preset? Parent;
        public Preset? GrandParent;
        public Preset? GreatGrandParent;
        public BlueInactiveAttribute? BlueInactive;
        public VariantAttribute? Variant;
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

        public PresetAttributes(Preset preset)
        {
            Preset = preset;
            IsPvP = PresetStorage.IsPvP(preset);
            Conflicts = PresetStorage.GetConflicts(preset);
            Parent = PresetStorage.GetParent(preset);
            BlueInactive = preset.GetAttribute<BlueInactiveAttribute>();
            Variant = preset.GetAttribute<VariantAttribute>();
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

            if (Parent is not null)
                GrandParent = PresetStorage.GetParent(Parent.Value);
            if (GrandParent is not null)
                GreatGrandParent = PresetStorage.GetParent(GrandParent.Value);
        }
    }

    private static uint[] GetRetargetedActions
    (Preset preset,
        RetargetedAttribute? retargetedAttribute,
        PossiblyRetargetedAttribute? possiblyRetargeted,
        Preset? parent)
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

        try
        {
            // Bail if not actually enabled
            if (!Service.Configuration.EnabledActions.Contains(preset))
                return [];
            // ReSharper disable once DuplicatedSequentialIfBodies
            if (parent != null &&
                !Service.Configuration.EnabledActions
                    .Contains((Preset)parent))
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

    internal static Dictionary<Preset, bool> GetJobAutorots => P
        .IPCSearch.AutoActions.Where(x => x.Key.Attributes().IsPvP == CustomComboFunctions.InPvP() && (Player.Job == x.Key.Attributes().CustomComboInfo.Job || Player.Job.GetUpgradedJob() == x.Key.Attributes().CustomComboInfo.Job) && x.Value && CustomComboFunctions.IsEnabled(x.Key) && x.Key.Attributes().Parent == null).ToDictionary();

    internal static void DrawPreset(Preset preset, CustomComboInfoAttribute info)
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
        var bozjaParents = Attributes[preset].BozjaParent;
        var eurekaParents = Attributes[preset].EurekaParent;
        var auto = Attributes[preset].AutoAction;
        var hidden = Attributes[preset].Hidden;
        var presetName = info.Name;
        var currentJob = Attributes[preset].CustomComboInfo.Job;

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

        if (IsSearching)
            presetName = preset.NameWithFullLineage(currentJob);

        if (P.UIHelper.ShowIPCControlledCheckboxIfNeeded
            ($"{presetName}###{preset}", ref enabled, preset, true))
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

        if (DrawRoleIcon(preset))
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 8f.Scale());

        if (DrawOccultJobIcon(preset))
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
            ImGui.Indent();
            foreach (var conflict in conflicts)
                ImGuiEx.Text(GradientColor.Get(
                        ImGuiColors.DalamudRed,
                        IsEnabled(conflict)
                            ? ImGuiColors.HealerGreen
                            : ImGuiColors.DalamudRed, 1500),
                    $"- {conflict.NameWithFullLineage(currentJob)}");
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
            ImGui.TextWrapped($"Part of normal combo{(eurekaParents.ParentPresets.Length > 1 ? "s" : "")}:");
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
                switch (info.Job)
                {
                    case Job.ADV:
                        {
                            All.Config.Draw(preset);
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
                switch (info.Job)
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
                    if (PresetStorage.ShouldBeHidden(childPreset)) continue;

                    presetChildren.TryGetValue(childPreset, out var grandchildren);
                    InfoBox box = new() { HasMaxWidth = true, CurveRadius = 4f, ContentsAction = () => { DrawPreset(childPreset, childInfo); } };
                    Action draw = grandchildren?.Count() > 0 && IsEnabled(childPreset) && Service.Configuration.ShowBorderAroundOptionsWithChildren
                        ? () => box.Draw()
                        : () => DrawPreset(childPreset, childInfo);

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
                            FeaturesWindow.CurrentPreset += 1 + AllChildren(presetChildren[childPreset]);
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
                FeaturesWindow.CurrentPreset += AllChildren(presetChildren[preset]);

            }
        }
    }

    private static void DrawReplaceAttribute(Preset preset)
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
                    ImGui.Image(img.Handle, (img.Size / 2f) * ImGui.GetIO().FontGlobalScale);
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
    (Preset? preset = null,
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

    private static bool DrawRoleIcon(Preset preset)
    {
        if (preset.Attributes().RoleAttribute == null) return false;
        if (preset.Attributes().Parent != null) return false;
        var role = preset.Attributes().RoleAttribute.Role;
        //if (jobID == -1) return false;
        var icon = Icons.Role.GetRoleIcon(role);
        if (icon is null) return false;
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3f.Scale());
        ImGui.Image(icon.Handle, (icon.Size / 2f) * ImGui.GetIO().FontGlobalScale);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f.Scale());
        return true;
    }

    private static bool DrawOccultJobIcon(Preset preset)
    {
        if (preset.Attributes().OccultCrescentJob == null) return false;
        var baseJobID = preset.Attributes().OccultCrescentJob.JobId;
        if (baseJobID == -1) return false;

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
            PluginLog.Error($"Failed to {error} Occult Crescent job icon for Preset:{preset} using JobID:{baseJobID}");
            return false;
        }
        #endregion

        var iconMaxSize = 32f.Scale();
        ImGui.SameLine();
        var scale = Math.Min(iconMaxSize / icon.Size.X, iconMaxSize / icon.Size.Y);
        var imgSize = new Vector2(icon.Size.X * scale, icon.Size.Y * scale);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 6f.Scale());
        ImGui.Image(icon.Handle, imgSize);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 6f.Scale());
        return true;
    }


    internal static int AllChildren((Preset Preset, CustomComboInfoAttribute Info)[] children)
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