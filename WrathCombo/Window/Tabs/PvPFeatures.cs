using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using System;
using System.Linq;
using System.Numerics;
using ECommons;
using WrathCombo.Core;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

namespace WrathCombo.Window.Tabs;

internal class PvPFeatures : ConfigWindow
{
    internal static Job? OpenJob;
    internal static int ColCount = 1;

    internal static new void Draw()
    {
        using (ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
        {
            var indentWidth = 12f.Scale();
            var indentWidth2 = indentWidth + 42f.Scale();
            var iconMaxSize = 34f.Scale();
            var verticalCenteringPadding = (iconMaxSize - ImGui.GetTextLineHeight()) / 2f;

            if (OpenJob is null)
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
                            if (ImGui.Selectable($"###{header}", OpenJob == job, ImGuiSelectableFlags.None, new Vector2(0, iconMaxSize)))
                            {
                                OpenJob = job;
                            }
                            ImGui.SameLine(indentWidth);
                            if (icon != null)
                            {
                                var scale = Math.Min(iconMaxSize / icon.Size.X, iconMaxSize / icon.Size.Y);
                                var imgSize = new Vector2(icon.Size.X * scale, icon.Size.Y * scale);
                                var padSize = (iconMaxSize - imgSize.X) / 2f;
                                if (padSize > 0)
                                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padSize);
                                ImGui.Image(icon.Handle, imgSize);
                            }
                            else
                            {
                                ImGui.Dummy(new Vector2(iconMaxSize, iconMaxSize));
                            }
                            ImGui.SameLine(indentWidth2);
                            ImGuiEx.Spacing(new Vector2(0, verticalCenteringPadding));
                            ImGui.Text($"{header} {(disabled ? "(Disabled due to update)" : "")}");
                        }

                        ImGui.TableNextColumn();
                    }
                }
            }
            else
            {
                var id = groupedPresets[OpenJob.Value].First().Info.Job;
                IDalamudTextureWrap? icon = Icons.GetJobIcon(id);

                using (ImRaii.Child("PvPHeadingTab", new Vector2(ImGui.GetContentRegionAvail().X, iconMaxSize)))
                {
                    if (ImGui.Button("Back", new Vector2(0, 24f.Scale())))
                    {
                        OpenJob = null;
                        return;
                    }
                    ImGui.SameLine();
                    ImGuiEx.LineCentered(() =>
                    {
                        if (icon != null)
                        {
                            var scale = Math.Min(iconMaxSize / icon.Size.X, iconMaxSize / icon.Size.Y);
                            var imgSize = new Vector2(icon.Size.X * scale, icon.Size.Y * scale);
                            var padSize = (iconMaxSize - imgSize.X) / 2f;
                            if (padSize > 0)
                                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padSize);
                            ImGui.Image(icon.Handle, imgSize);
                        }
                        else
                        {
                            ImGui.Dummy(new Vector2(iconMaxSize, iconMaxSize));
                        }
                        ImGui.SameLine();
                        ImGuiEx.Spacing(new Vector2(0, verticalCenteringPadding-2f.Scale()));
                        ImGuiEx.Text($"{OpenJob.Value.Name()}");
                    });

                }

                using (ImRaii.Child("Contents", new Vector2(0)))
                {
                    currentPreset = 1;
                    try
                    {
                        if (ImGui.BeginTabBar($"subTab{OpenJob.Value.Name()}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                        {
                            if (ImGui.BeginTabItem("Normal"))
                            {
                                DrawHeadingContents(OpenJob.Value);
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
    }

    private static void DrawHeadingContents(Job job)
    {
        foreach (var (preset, info) in groupedPresets[job].Where(x => PresetStorage.IsPvP(x.Preset)))
        {
            InfoBox presetBox = new() { ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info); } };

            if (Service.Configuration.HideConflictedCombos)
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
                    currentPreset += 1 + Presets.AllChildren(presetChildren[parent]);
                }

                else
                {
                    presetBox.Draw();
                    continue;
                }
            }

            else
            {
                presetBox.Draw();
                ImGuiEx.Spacing(new Vector2(0, 12));
            }
        }
    }
}