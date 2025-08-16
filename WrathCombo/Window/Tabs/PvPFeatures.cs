using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using System;
using System.Linq;
using System.Numerics;
using WrathCombo.Core;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

namespace WrathCombo.Window.Tabs;

internal class PvPFeatures : ConfigWindow
{
    internal static Job? OpenJob = null;
    internal static int ColCount = 1;

    internal static new void Draw()
    {
        using (var scrolling = ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
        {
            var indentwidth = 12f.Scale();
            var indentwidth2 = indentwidth + 42f.Scale();

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
                        using (var disabled = ImRaii.Disabled(DisabledJobsPVP.Any(x => x == id)))
                        {
                            if (ImGui.Selectable($"###{header}", OpenJob == job, ImGuiSelectableFlags.None,
                                icon == null ? new Vector2(0, 32).Scale() : new Vector2(0, icon.Size.Y / 2f).Scale()))
                            {
                                OpenJob = job;
                            }
                            ImGui.SameLine(indentwidth);
                            if (icon != null)
                            {
                                ImGui.Image(icon.Handle, new Vector2(icon.Size.X, icon.Size.Y).Scale() / 2f);
                                ImGui.SameLine(indentwidth2);
                            }
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

                using (var headingTab = ImRaii.Child("PvPHeadingTab", new Vector2(ImGui.GetContentRegionAvail().X, icon is null ? 24f.Scale() : (icon.Size.Y / 2f).Scale() + 4f)))
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
                            ImGui.Image(icon.Handle, new Vector2(icon.Size.X, icon.Size.Y).Scale() / 2f);
                            ImGui.SameLine();
                        }
                        ImGuiEx.Text($"{OpenJob.Value.Name()}");
                    });

                }

                using (var contents = ImRaii.Child("Contents", new Vector2(0)))
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
                    catch { }

                }
            }

        }
    }

    private static void DrawHeadingContents(Job job)
    {
        foreach (var (preset, info) in groupedPresets[job].Where(x => PresetStorage.IsPvP(x.Preset)))
        {
            InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f.Scale(), ContentsOffset = 8f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info); } };

            if (Service.Configuration.HideConflictedCombos)
            {
                var conflictOriginals = PresetStorage.GetConflicts(preset); // Presets that are contained within a ConflictedAttribute
                var conflictsSource = PresetStorage.GetAllConflicts();      // Presets with the ConflictedAttribute

                if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
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