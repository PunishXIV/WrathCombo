using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WrathCombo.Core;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.UI.Features;
using WrathCombo.Resources.Localization.UI.Misc;
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
            ImGui.TextWrapped(FeaturesUI.Warning_EquipJobStone);
            return;
        }
        //#endif

        using (ImRaii.Child("scrolling", new Vector2(AvailableWidth, ImGui.GetContentRegionAvail().Y), true))
        {
            var openJob = OpenJob; // Cache because back button will set it to null while this is running
            if (openJob is null)
            {
                // The "Main Menu" of PvE features, showing each job to click on
                ImGui.SameLine(IndentWidth);
                ImGuiEx.LineCentered(() =>
                {
                    ImGuiEx.TextUnderlined(FeaturesUI.Info_SelectAJob);
                });

                ColCount = Math.Max(1, (int)(AvailableWidth / 200f.Scale()));

                using (var tab = ImRaii.Table("PvETable", ColCount))
                {
                    ImGui.TableNextColumn();

                    if (!tab)
                        return;

                    foreach (var (job, presetData) in groupedPresets)
                    {
                        var info = presetData[0].JobInfo;
                        string jobName = info.JobName;
                        string abbreviation = info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = info.Job;
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
                            ImGui.Text($"{header} {(disabled ? FeaturesUI.Warning_DisabledDueToUpdate : "")}");

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
                // Draw Presets for a selected Job
                DrawHeader(openJob.Value);
                DrawSearchBar();
                ImGuiEx.Spacing(new Vector2(0, 10));

                using var content = ImRaii.Child(MiscUI.Content, Vector2.Zero);
                if (!content)
                    return;

                CurrentPreset = 1;

                try
                {
                    if (!ImGui.BeginTabBar($"subTab{openJob.Value.Name()}",
                            ImGuiTabBarFlags.Reorderable |
                            ImGuiTabBarFlags.AutoSelectNewTabs))
                        return;

                    string mainTabName = openJob.Value is Job.ADV ? MiscUI.Job_Roles : MiscUI.Normal;
                    if (ImGui.BeginTabItem(mainTabName))
                    {
                        SetCurrentTab(FeatureTab.Normal);
                        DrawHeadingContents(openJob.Value); // This draws all the normal PvE Combos for a job
                        ImGui.EndTabItem();
                    }

                    if (openJob is Job.ADV)
                    {
                        if (groupedPresets[openJob.Value].Any(x => x.IsVariant))
                        {
                            if (ImGui.BeginTabItem(MiscUI.Variant_Dungeons))
                            {
                                SetCurrentTab(FeatureTab.Variant);
                                DrawVariantContents(openJob.Value);
                                ImGui.EndTabItem();
                            }
                        }

                        if (groupedPresets[openJob.Value].Any(x => x.IsBozja))
                        {
                            if (ImGui.BeginTabItem(MiscUI.Bozja))
                            {
                                SetCurrentTab(FeatureTab.Bozja);
                                DrawBozjaContents(openJob.Value);
                                ImGui.EndTabItem();
                            }
                        }

                        if (groupedPresets[openJob.Value].Any(x =>
                                x.IsOccultCrescent))
                        {
                            if (ImGui.BeginTabItem(MiscUI.Occult_Crescent))
                            {
                                SetCurrentTab(FeatureTab.OccultCrescent);
                                DrawOccultContents(openJob.Value);
                                ImGui.EndTabItem();
                            }
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
        foreach (var presetData in groupedPresets[job].Where(x =>
            x.IsVariant &&
            !x.IsHidden))
        {
            if (IsSearching && !PresetMatchesSearch(presetData.Preset))
                continue;
            alreadyShown.Add(presetData.Preset);

            InfoBox presetBox = new() { CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(presetData.Preset, presetData); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets([.. PresetStorage.AllPresets!
                .Where(kvp =>
                    kvp.Value.IsVariant &&
                    !kvp.Value.ShouldBeHidden &&
                    kvp.Value.JobInfo.Job == job)
                .Select(x => x.Key)],
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    private static void DrawBozjaContents(Job job)
    {
        List<Preset> alreadyShown = [];
        foreach (var presetData in groupedPresets[job].Where(x =>
            x.IsBozja &&
            !x.IsHidden))
        {
            if (IsSearching && !PresetMatchesSearch(presetData.Preset))
                continue;
            alreadyShown.Add(presetData.Preset);

            InfoBox presetBox = new() { CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(presetData.Preset, presetData); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets([.. PresetStorage.AllPresets!
                .Where(kvp =>
                    kvp.Value.IsBozja &&
                    !kvp.Value.ShouldBeHidden &&
                    kvp.Value.JobInfo.Job == job)
                .Select(kvp => kvp.Key)],
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    private static void DrawOccultContents(Job job)
    {
        List<Preset> alreadyShown = [];
        foreach (var presetData in groupedPresets[job].Where(x =>
            x.IsOccultCrescent &&
            !x.ShouldBeHidden))
        {
            if (IsSearching && !PresetMatchesSearch(presetData.Preset))
                continue;
            alreadyShown.Add(presetData.Preset);

            InfoBox presetBox = new() { CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(presetData.Preset, presetData); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }

        // Search for children if nothing was found at the root
        if (IsSearching)
            SearchMorePresets([.. PresetStorage.AllPresets!
                .Where(kvp =>
                    kvp.Value.IsOccultCrescent &&
                    !kvp.Value.ShouldBeHidden &&
                    kvp.Value.JobInfo.Job == job)
                .Select(kvp => kvp.Key)],
                alreadyShown);
        ShowSearchErrorIfNoResults();
    }

    // This draws all the normal PvE Combos for a job
    internal static void DrawHeadingContents(Job job)
    {
        if (!Messages.PrintBLUMessage(job)) return;

        static bool IsPvECombo(PresetStorage.PresetData presetData)
        {
            return !presetData.IsPvP &&
                   !presetData.IsVariant &&
                   !presetData.IsBozja &&
                   !presetData.IsOccultCrescent &&
                   !presetData.ShouldBeHidden;
        }

        List<Preset> alreadyShown = [];
        foreach (var presetData in groupedPresets[job].Where(IsPvECombo))
        {
            if (IsSearching && !PresetMatchesSearch(presetData.Preset))
                continue;
            alreadyShown.Add(presetData.Preset);

            InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(presetData.Preset, presetData); } };

            if (Service.Configuration.HideConflictedCombos && !IsSearching)
            {
                var conflictOriginals = presetData.Conflicts; // Presets that are contained within a ConflictedAttribute

                if (PresetStorage.ConflictingCombos.All(x => x != presetData.Preset) || conflictOriginals.Length == 0)
                {
                    presetBox.Draw();
                    ImGuiEx.Spacing(new Vector2(0, 12));
                    continue;
                }

                if (conflictOriginals.Any(PresetStorage.IsEnabled))
                {
                    // Keep conflicted items in the counter
                    var parent = presetData.Parent ?? presetData.Preset;
                    CurrentPreset += 1 + Presets.AllChildren(presetChildren[parent].ToArray());
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
            SearchMorePresets([.. PresetStorage.AllPresets!
                .Where(kvp =>
                    IsPvECombo(kvp.Value) &&
                    kvp.Value.JobInfo.Job == job)
                .Select(x => x.Key)],
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