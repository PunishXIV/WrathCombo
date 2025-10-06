using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ECommons;
using WrathCombo.Core;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using WrathCombo.Window.MessagesNS;

namespace WrathCombo.Window.Tabs;

internal class PvEFeatures : FeaturesWindow
{
    internal static new void Draw()
    {
        //#if !DEBUG
        if (ActionReplacer.ClassLocked())
        {
            ImGui.TextWrapped("Equip your job stone to re-unlock features.");
            return;
        }
        //#endif

        using (ImRaii.Child("scrolling", new Vector2(AvailableWidth, ImGui.GetContentRegionAvail().Y), true))
        {
            if (OpenJob is null)
            {
                ImGui.SameLine(IndentWidth);
                ImGuiEx.LineCentered(() =>
                {
                    ImGuiEx.TextUnderlined("Select a job from below to enable and configure features for it.");
                });

                ColCount = Math.Max(1, (int)(AvailableWidth / 200f.Scale()));

                using (var tab = ImRaii.Table("PvETable", ColCount))
                {
                    ImGui.TableNextColumn();

                    if (!tab)
                        return;

                    foreach (Job job in groupedPresets.Keys)
                    {
                        string jobName = groupedPresets[job].First().Info.JobName;
                        string abbreviation = groupedPresets[job].First().Info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = groupedPresets[job].First().Info.Job;
                        IDalamudTextureWrap? icon = Icons.GetJobIcon(id);
                        ImGuiEx.Spacing(new Vector2(0, 2f.Scale()));
                        using (var disabled = ImRaii.Disabled(DisabledJobsPVE.Any(x => x == id)))
                        {
                            if (ImGui.Selectable($"###{header}", OpenJob == job, ImGuiSelectableFlags.None, new Vector2(0, IconMaxSize)))
                            {
                                OpenJob = job;
                            }
                            ImGui.SameLine(IndentWidth);
                            if (icon != null)
                            {
                                var scale = Math.Min(IconMaxSize / icon.Size.X, IconMaxSize / icon.Size.Y);
                                var imgSize = new Vector2(icon.Size.X * scale, icon.Size.Y * scale);
                                var padSize = (IconMaxSize - imgSize.X) / 2f;
                                if (padSize > 0)
                                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padSize);
                                ImGui.Image(icon.Handle, imgSize);
                            }
                            else
                            {
                                ImGui.Dummy(new Vector2(IconMaxSize, IconMaxSize));
                            }
                            ImGui.SameLine(LargerIndentWidth);
                            ImGuiEx.Spacing(new Vector2(0, VerticalCenteringPadding));
                            ImGui.Text($"{header} {(disabled ? "(Disabled due to update)" : "")}");

                            if (!string.IsNullOrEmpty(abbreviation) &&
                                P.UIHelper.JobControlled(id) is not null)
                            {
                                ImGui.SameLine();
                                P.UIHelper
                                    .ShowIPCControlledIndicatorIfNeeded(id, false, ColCount > 1);
                            }
                        }

                        ImGui.TableNextColumn();
                    }
                }
            }
            else
            {
                var openJob = OpenJob.Value;
                var id = groupedPresets[openJob].First().Info.Job;

                DrawHeader(id);
                DrawSearchBar();
                ImGuiEx.Spacing(new Vector2(0, 10));
                
                using var content = ImRaii.Child("Content", Vector2.Zero);
                if (!content)
                    return;

                CurrentPreset = 1;

                try
                {
                    if (!ImGui.BeginTabBar($"subTab{openJob.Name()}",
                            ImGuiTabBarFlags.Reorderable |
                            ImGuiTabBarFlags.AutoSelectNewTabs))
                        return;

                    var mainTabName = OpenJob is Job.ADV ? "Job Roles" : "Normal";
                    if (ImGui.BeginTabItem(mainTabName))
                    {
                        SetCurrentTab(FeatureTab.Normal);
                        DrawHeadingContents(openJob);
                        ImGui.EndTabItem();
                    }

                    if (groupedPresets[openJob].Any(x =>
                            PresetStorage.IsVariant(x.Preset)))
                    {
                        if (ImGui.BeginTabItem("Variant Dungeons"))
                        {
                            SetCurrentTab(FeatureTab.Variant);
                            DrawVariantContents(openJob);
                            ImGui.EndTabItem();
                        }
                    }

                    if (groupedPresets[openJob].Any(x =>
                            PresetStorage.IsBozja(x.Preset)))
                    {
                        if (ImGui.BeginTabItem("Bozja"))
                        {
                            SetCurrentTab(FeatureTab.Bozja);
                            DrawBozjaContents(openJob);
                            ImGui.EndTabItem();
                        }
                    }

                    if (groupedPresets[openJob].Any(x =>
                            PresetStorage.IsEureka(x.Preset)))
                    {
                        if (ImGui.BeginTabItem("Eureka"))
                        {
                            SetCurrentTab(FeatureTab.Eureka);
                            //DrawEurekaContents(openJob);
                            ImGui.EndTabItem();
                        }
                    }

                    if (groupedPresets[openJob].Any(x =>
                            PresetStorage.IsOccultCrescent(x.Preset)))
                    {
                        if (ImGui.BeginTabItem("Occult Crescent"))
                        {
                            SetCurrentTab(FeatureTab.OccultCrescent);
                            DrawOccultContents(openJob);
                            ImGui.EndTabItem();
                        }
                    }

                    ImGui.EndTabBar();
                }
                catch (Exception e)
                {
                    PluginLog.Error(
                        $"Error while drawing Job's UI:\n{e.ToStringFull()}");
                }
            }

        }
    }

    private static void DrawVariantContents(Job job)
    {
        List<Preset> alreadyShown = [];
        foreach (var (preset, info) in groupedPresets[job].Where(x =>
            PresetStorage.IsVariant(x.Preset) &&
            !PresetStorage.ShouldBeHidden(x.Preset)))
        {
            if (IsSearching && !PresetMatchesSearch(preset))
                continue;
            alreadyShown.Add(preset);

            InfoBox presetBox = new() { CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets(PresetStorage.AllPresets!
                .Where(x =>
                    PresetStorage.IsVariant(x) &&
                    !PresetStorage.ShouldBeHidden(x) &&
                    x.Attributes().CustomComboInfo.Job == job)
                .ToArray(),
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    private static void DrawBozjaContents(Job job)
    {
        List<Preset> alreadyShown = [];
        foreach (var (preset, info) in groupedPresets[job].Where(x =>
            PresetStorage.IsBozja(x.Preset) &&
            !PresetStorage.ShouldBeHidden(x.Preset)))
        {
            if (IsSearching && !PresetMatchesSearch(preset))
                continue;
            alreadyShown.Add(preset);

            InfoBox presetBox = new() { CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets(PresetStorage.AllPresets!
                .Where(x =>
                    PresetStorage.IsBozja(x) &&
                    !PresetStorage.ShouldBeHidden(x) &&
                    x.Attributes().CustomComboInfo.Job == job)
                .ToArray(),
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    private static void DrawOccultContents(Job job)
    {
        List<Preset> alreadyShown = [];
        foreach (var (preset, info) in groupedPresets[job].Where(x =>
            PresetStorage.IsOccultCrescent(x.Preset) &&
            !PresetStorage.ShouldBeHidden(x.Preset)))
        {
            if (IsSearching && !PresetMatchesSearch(preset))
                continue;
            alreadyShown.Add(preset);

            InfoBox presetBox = new() { CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets(PresetStorage.AllPresets!
                .Where(x =>
                    PresetStorage.IsOccultCrescent(x) &&
                    !PresetStorage.ShouldBeHidden(x) &&
                    x.Attributes().CustomComboInfo.Job == job)
                .ToArray(),
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    internal static void DrawHeadingContents(Job job)
    {
        if (!Messages.PrintBLUMessage(job)) return;

        bool IsPvECombo(Preset preset)
        {
            return !PresetStorage.IsPvP(preset) &&
                   !PresetStorage.IsVariant(preset) &&
                   !PresetStorage.IsBozja(preset) &&
                   !PresetStorage.IsEureka(preset) &&
                   !PresetStorage.IsOccultCrescent(preset) &&
                   !PresetStorage.ShouldBeHidden(preset);
        }

        List<Preset> alreadyShown = [];
        foreach (var (preset, info) in groupedPresets[job].Where(x =>
                     IsPvECombo(x.Preset)))
        {
            if (IsSearching && !PresetMatchesSearch(preset))
                continue;
            alreadyShown.Add(preset);
            
            InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info); } };

            if (Service.Configuration.HideConflictedCombos && !IsSearching)
            {
                var conflictOriginals = PresetStorage.GetConflicts(preset); // Presets that are contained within a ConflictedAttribute
                var conflictsSource = PresetStorage.GetAllConflicts();      // Presets with the ConflictedAttribute

                if (conflictsSource.All(x => x != preset) || conflictOriginals.Length == 0)
                {
                    presetBox.Draw();
                    ImGuiEx.Spacing(new Vector2(0, 12));
                    continue;
                }

                if (conflictOriginals.Any(PresetStorage.IsEnabled))
                {
                    if (DateTime.UtcNow - LastPresetDeconflictTime > TimeSpan.FromSeconds(3))
                    {
                        if (Service.Configuration.EnabledActions.Remove(preset))
                        {
                            PluginLog.Debug($"Removed {preset} due to conflict");
                            Service.Configuration.Save();
                        }
                        LastPresetDeconflictTime = DateTime.UtcNow;
                    }

                    // Keep removed items in the counter
                    var parent = PresetStorage.GetParent(preset) ?? preset;
                    CurrentPreset += 1 + Presets.AllChildren(presetChildren[parent]);
                }

                else
                    presetBox.Draw();
            }

            else
            {
                presetBox.Draw();
                ImGuiEx.Spacing(new Vector2(0, 12));
            }
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets(PresetStorage.AllPresets!
                .Where(x =>
                    IsPvECombo(x) &&
                    x.Attributes().CustomComboInfo.Job == job)
                .ToArray(),
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    internal static void OpenToCurrentJob(bool onJobChange)
    {
        if ((!onJobChange || !Service.Configuration.OpenToCurrentJobOnSwitch) &&
            (onJobChange || !Service.Configuration.OpenToCurrentJob ||
             !Player.Available)) return;

        if (onJobChange && !P.ConfigWindow.IsOpen)
            return;

        if (Player.Job.IsDoh())
            return;

        if (Player.Job.IsDol())
        {
            OpenJob = Job.MIN;
            return;
        }

        var job = Player.Job.GetUpgradedJob();
        if (groupedPresets.ContainsKey(job))
            OpenJob = job;
    }
}