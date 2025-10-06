using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
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

namespace WrathCombo.Window.Tabs;

internal class PvPFeatures : FeaturesWindow
{
    internal static new void Draw()
    {
        using (ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
        {
            if (OpenPvPJob is null)
            {
                ImGuiEx.LineCentered("pvpDesc", () =>
                {
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextWrapped($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
                    ImGui.PopFont();
                    ImGui.SameLine();
                    ImGui.TextWrapped("These are PvP features. They will only work in PvP-enabled zones.");
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextWrapped($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
                    ImGui.PopFont();
                });
                ImGuiEx.LineCentered($"pvpDesc2", () =>
                {
                    ImGuiEx.TextUnderlined("Select a job from below to enable and configure features for it.");
                });
                ImGui.Spacing();

                ColCount = Math.Max(1, (int)(ImGui.GetContentRegionAvail().X / 200f.Scale()));

                using (var tab = ImRaii.Table("PvPTable", ColCount))
                {
                    ImGui.TableNextColumn();

                    if (!tab)
                        return;

                    foreach (Job job in groupedPresets.Where(x =>
                            x.Value.Any(y => PresetStorage.IsPvP(y.Preset) &&
                                             !PresetStorage.ShouldBeHidden(y.Preset)))
                        .Select(x => x.Key))
                    {
                        string jobName = groupedPresets[job].First().Info.JobName;
                        string abbreviation = groupedPresets[job].First().Info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = groupedPresets[job].First().Info.Job;
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
                            ImGui.Text($"{header} {(disabled ? "(Disabled due to update)" : "")}");
                        }

                        ImGui.TableNextColumn();
                    }
                }
            }
            else
            {
                var id = groupedPresets[OpenPvPJob.Value].First().Info.Job;

                DrawHeader(id, true);
                DrawSearchBar();
                ImGuiEx.Spacing(new Vector2(0, 10));
                
                using var content = ImRaii.Child("PvPContent", Vector2.Zero);
                if (!content)
                    return;

                CurrentPreset = 1;

                try
                {
                    if (ImGui.BeginTabBar($"subTab{OpenPvPJob.Value.Name()}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                    {
                        if (ImGui.BeginTabItem("Normal"))
                        {
                            DrawHeadingContents(OpenPvPJob.Value);
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error($"Error while drawing Job's PvP UI:\n{e.ToStringFull()}");
                }
            }

        }
    }

    private static void DrawHeadingContents(Job job)
    {
        foreach (var (preset, info) in groupedPresets[job].Where(x => PresetStorage.IsPvP(x.Preset)))
        {
            InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info); } };
            
            if (IsSearching && !PvEFeatures.PresetMatchesSearch(preset))
                continue;

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
                         PresetStorage.IsPvP(x) &&
                         x.Attributes().CustomComboInfo.Job == job))
            {
                var attributes = preset.Attributes();
                
                if (!PvEFeatures.PresetMatchesSearch(preset))
                    continue;
                // Don't show things that were already shown under another preset
                if (alreadyShown.Any(y => y == attributes.Parent) ||
                    alreadyShown.Any(y => y == attributes.GrandParent) ||
                    alreadyShown.Any(y => y == attributes.GreatGrandParent))
                    continue;
                
                var info = attributes.CustomComboInfo;
                InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info!); } };
                presetBox.Draw();
                ImGuiEx.Spacing(new Vector2(0, 12));
                alreadyShown.Add(preset);
            }

            // Show error message if still nothing was found
            if (CurrentPreset == 1) {
                ImGuiEx.LineCentered(() =>
                {
                    ImGui.TextUnformatted("Nothing matched your search.");
                });
            }
        }
    }
}