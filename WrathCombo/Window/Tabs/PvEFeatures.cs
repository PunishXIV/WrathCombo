using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using System;
using System.Linq;
using System.Numerics;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using WrathCombo.Window.MessagesNS;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions.JobIDs;
namespace WrathCombo.Window.Tabs;

internal class PvEFeatures : ConfigWindow
{
    internal static string OpenJob = string.Empty;
    internal static int ColCount = 1;
    internal static new void Draw()
    {
        //#if !DEBUG
        if (ActionReplacer.ClassLocked())
        {
            ImGui.TextWrapped("Equip your job stone to re-unlock features.");
            return;
        }
        //#endif

        using (var scrolling = ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
        {
            var indentwidth = 12f.Scale();
            var indentwidth2 = indentwidth + 42f.Scale();
            var iconMaxSize = 34f.Scale();
            var verticalCenteringPadding = (iconMaxSize - ImGui.GetTextLineHeight()) / 2f;

            if (OpenJob == string.Empty)
            {
                ImGui.SameLine(indentwidth);
                ImGuiEx.LineCentered(() =>
                {
                    ImGuiEx.TextUnderlined("Select a job from below to enable and configure features for it.");
                });

                ColCount = Math.Max(1, (int)(ImGui.GetContentRegionAvail().X / 200f.Scale()));

                using (var tab = ImRaii.Table("PvETable", ColCount))
                {
                    ImGui.TableNextColumn();

                    if (!tab)
                        return;

                    foreach (string? jobName in groupedPresets.Keys)
                    {
                        string abbreviation = groupedPresets[jobName].First().Info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = groupedPresets[jobName].First().Info.JobID;
                        IDalamudTextureWrap? icon = Icons.GetJobIcon(id);
                        ImGuiEx.Spacing(new Vector2(0, 2f.Scale()));
                        using (var disabled = ImRaii.Disabled(DisabledJobsPVE.Any(x => x == id)))
                        {
                            if (ImGui.Selectable($"###{header}", OpenJob == jobName, ImGuiSelectableFlags.None, new Vector2(0, iconMaxSize)))
                            {
                                OpenJob = jobName;
                            }
                            ImGui.SameLine(indentwidth);
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
                            ImGui.SameLine(indentwidth2);

                            ImGuiEx.Spacing(new Vector2(0, verticalCenteringPadding));
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
                var id = groupedPresets[OpenJob].First().Info.JobID;
                IDalamudTextureWrap? icon = Icons.GetJobIcon(id);

                using (ImRaii.Child("HeadingTab", new Vector2(ImGui.GetContentRegionAvail().X, iconMaxSize)))
                {
                    if (ImGui.Button("Back", new Vector2(0, 24f.Scale())))
                    {
                        OpenJob = "";
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
                        ImGuiEx.Text($"{OpenJob}");
                    });

                    if (P.UIHelper.JobControlled(id) is not null)
                    {
                        ImGui.SameLine();
                        P.UIHelper
                            .ShowIPCControlledIndicatorIfNeeded(id);
                    }

                }

                using (var contents = ImRaii.Child("Contents", new Vector2(0)))
                {
                    currentPreset = 1;
                    try
                    {
                        if (ImGui.BeginTabBar($"subTab{OpenJob}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                        {
                            if (ImGui.BeginTabItem("Normal"))
                            {
                                DrawHeadingContents(OpenJob);
                                ImGui.EndTabItem();
                            }

                            if (groupedPresets[OpenJob].Any(x => PresetStorage.IsVariant(x.Preset)))
                            {
                                if (ImGui.BeginTabItem("Variant Dungeons"))
                                {
                                    DrawVariantContents(OpenJob);
                                    ImGui.EndTabItem();
                                }
                            }

                            if (groupedPresets[OpenJob].Any(x => PresetStorage.IsBozja(x.Preset)))
                            {
                                if (ImGui.BeginTabItem("Bozja"))
                                {
                                    DrawBozjaContents(OpenJob);
                                    ImGui.EndTabItem();
                                }
                            }

                            if (groupedPresets[OpenJob].Any(x => PresetStorage.IsEureka(x.Preset)))
                            {
                                if (ImGui.BeginTabItem("Eureka"))
                                {
                                    ImGui.EndTabItem();
                                }
                            }

                            ImGui.EndTabBar();
                        }
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"Error while drawing Job's UI:\n{e}");
                    }

                }
            }

        }
    }

    private static void DrawVariantContents(string jobName)
    {
        foreach (var (preset, info) in groupedPresets[jobName].Where(x =>
            PresetStorage.IsVariant(x.Preset) &&
            !PresetStorage.ShouldBeHidden(x.Preset)))
        {
            InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }
    }
    private static void DrawBozjaContents(string jobName)
    {
        foreach (var (preset, info) in groupedPresets[jobName].Where(x =>
            PresetStorage.IsBozja(x.Preset) &&
            !PresetStorage.ShouldBeHidden(x.Preset)))
        {
            InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info); } };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
        }
    }

    internal static void DrawHeadingContents(string jobName)
    {
        if (!Messages.PrintBLUMessage(jobName)) return;

        foreach (var (preset, info) in groupedPresets[jobName].Where(x =>
            !PresetStorage.IsPvP(x.Preset) &&
            !PresetStorage.IsVariant(x.Preset) &&
            !PresetStorage.IsBozja(x.Preset) &&
            !PresetStorage.IsEureka(x.Preset) &&
            !PresetStorage.ShouldBeHidden(x.Preset)))
        {
            InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 2f.Scale(), ContentsOffset = 5f.Scale(), ContentsAction = () => { Presets.DrawPreset(preset, info); } };

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
            OpenJob = JobIDToName(DOL.JobID);
            return;
        }

        var job = JobIDToName(ClassToJob((uint)Player.Job));
        if (groupedPresets.TryGetValue(job, out var foundJob))
            OpenJob = foundJob.First().Info.JobName;
    }
}