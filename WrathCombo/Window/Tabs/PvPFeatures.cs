using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using ECommons.ExcelServices;
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

namespace WrathCombo.Window.Tabs;

internal class PvPFeatures : FeaturesWindow
{
    internal static new void Draw()
    {
        using (ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
        {
            var openPvPJob = OpenPvPJob; // Cache because back button will set it to null while running

            // The "Main Menu" of PvP features, showing each job to click on
            if (openPvPJob is null)
            {
                var userwarned = false;

                //Auto-Rotation warning
                if (P.IPC.GetAutoRotationState())
                {
                    ImGuiEx.LineCentered($"pvpWarning", () =>
                    {
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, $"{FontAwesomeIcon.ExclamationTriangle.ToIconString()}");
                        ImGui.PopFont();
                        ImGui.SameLine();
                        ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, FeaturesUI.Info_pvpAutoRotationWarning);
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, $"{FontAwesomeIcon.ExclamationTriangle.ToIconString()}");
                        ImGui.PopFont();
                    });
                    userwarned = true;
                }

                // Action Changing disabled warning
                if (!Service.Configuration.ActionChanging)
                {
                    ImGuiEx.LineCentered($"pvpWarning2", () =>
                    {
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, $"{FontAwesomeIcon.ExclamationTriangle.ToIconString()}");
                        ImGui.PopFont();
                        ImGui.SameLine();
                        ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, FeaturesUI.Info_pvpAutoRotationWarning);
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, $"{FontAwesomeIcon.ExclamationTriangle.ToIconString()}");
                        ImGui.PopFont();
                    });
                    userwarned = true;
                }

                // Add spacing if any warning was shown
                if (userwarned) ImGuiEx.Spacing(new Vector2(0, 15));

                ImGuiEx.LineCentered("pvpDesc", () =>
                {
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextWrapped($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
                    ImGui.PopFont();
                    ImGui.SameLine();
                    ImGui.TextWrapped(FeaturesUI.Info_pvpDesc);
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextWrapped($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
                    ImGui.PopFont();
                });
                ImGuiEx.LineCentered($"pvpDesc2", () =>
                {
                    ImGuiEx.TextUnderlined(FeaturesUI.Info_SelectAJob);
                });
                ImGui.Spacing();

                ColCount = Math.Max(1, (int)(ImGui.GetContentRegionAvail().X / 200f.Scale()));

                using (var tab = ImRaii.Table("PvPTable", ColCount))
                {
                    ImGui.TableNextColumn();

                    if (!tab)
                        return;

                    foreach (var (job, presetData) in groupedPresets
                        .Where(x => x.Value.Any(y => y.IsPvP && !y.ShouldBeHidden)))
                    {
                        var info = presetData[0].JobInfo;
                        string jobName = info.JobName;
                        string abbreviation = info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = info.Job;
                        IDalamudTextureWrap? icon = Icons.GetJobIcon(id);
                        ImGuiEx.Spacing(new Vector2(0, 2f.Scale()));
                        using (var disabled = ImRaii.Disabled(DisabledJobsPVP.Any(x => x == id)))
                        {
                            if (ImGui.Selectable($"###{header}", OpenPvPJob == job, ImGuiSelectableFlags.None, new Vector2(0, IconMaxSize)))
                            {
                                OpenPvPJob = job;
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
                        }

                        ImGui.TableNextColumn();
                    }
                }
            }
            else
            {
                // Draw Presets for a selected Job
                DrawHeader(openPvPJob.Value, true);
                DrawSearchBar();
                ImGuiEx.Spacing(new Vector2(0, 10));

                using var content = ImRaii.Child("PvPContent", Vector2.Zero);
                if (!content)
                    return;

                CurrentPreset = 1;

                try
                {
                    if (ImGui.BeginTabBar($"subTab{openPvPJob.Value.Name()}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                    {
                        if (ImGui.BeginTabItem(MiscUI.Normal))
                        {
                            DrawHeadingContents(openPvPJob.Value);
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error($"Error while drawing {openPvPJob} PvP UI:\n{e.ToStringFull()}");
                }
            }

        }
    }

    private static void DrawHeadingContents(Job job)
    {
        foreach (var presetData in groupedPresets[job].Where(x => x.IsPvP))
        {
            InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(presetData.Preset, presetData); } };

            if (IsSearching && !PvEFeatures.PresetMatchesSearch(presetData.Preset))
                continue;

            if (Service.Configuration.HideConflictedCombos && !IsSearching)
            {
                var conflictOriginals = presetData.Conflicts;                    // Presets that are contained within a ConflictedAttribute

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
                {
                    presetBox.Draw();
                }
            }

            else
            {
                presetBox.Draw();
                ImGuiEx.Spacing(new Vector2(0, 12));
            }
        }

        // Search for children if nothing was found at the root
        if (CurrentPreset == 1 && IsSearching)
        {
            List<Preset> alreadyShown = [];
            foreach (var preset in PresetStorage.AllPresets!.Where(x =>
                        x.Value.IsPvP &&
                        x.Value.JobInfo.Job == job))
            {
                var attributes = preset.Value;

                if (!PvEFeatures.PresetMatchesSearch(preset.Key))
                    continue;
                // Don't show things that were already shown under another preset
                if (alreadyShown.Any(y => y == attributes.Parent) ||
                    alreadyShown.Any(y => y == attributes.GrandParent) ||
                    alreadyShown.Any(y => y == attributes.GreatGrandParent))
                    continue;

                InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset.Key, attributes); } };
                presetBox.Draw();
                ImGuiEx.Spacing(new Vector2(0, 12));
                alreadyShown.Add(preset.Key);
            }

            // Show error message if still nothing was found
            if (CurrentPreset == 1)
            {
                ImGuiEx.LineCentered(() =>
                {
                    ImGui.TextUnformatted(FeaturesUI.Info_pvpNothing);
                });
            }
        }
    }
}