using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using PunishLib;
using PunishLib.ImGuiMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Colors;
using WrathCombo.Attributes;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.Data;
using WrathCombo.Window.Tabs;

namespace WrathCombo.Window
{
    /// <summary> Plugin configuration window. </summary>
    internal class ConfigWindow : Dalamud.Interface.Windowing.Window
    {
        internal static readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets = GetGroupedPresets();
        internal static readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren = GetPresetChildren();
        internal static int currentPreset = 1;
        internal static Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> GetGroupedPresets()
        {
            return Enum
            .GetValues<CustomComboPreset>()
            .Where(preset => (int)preset > 100)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && PresetStorage.GetParent(tpl.Preset) == null)
            .OrderByDescending(tpl => tpl.Info.JobID == 0)
            .ThenByDescending(tpl => tpl.Info.JobID == DOL.JobID)
            .ThenByDescending(tpl => tpl.Info.JobID == DOH.JobID)
            .ThenByDescending(tpl => tpl.Info.Role == 1)
            .ThenByDescending(tpl => tpl.Info.Role == 4)
            .ThenByDescending(tpl => tpl.Info.Role == 2)
            .ThenByDescending(tpl => tpl.Info.Role == 3)
            .ThenBy(tpl => tpl.Info.ClassJobCategory)
            .ThenBy(tpl => tpl.Info.JobName)
            .ThenBy(tpl => tpl.Info.Order)
            .GroupBy(tpl => tpl.Info.JobName)
            .ToDictionary(
                tpl => tpl.Key,
                tpl => tpl.ToList())!;
        }

        internal static Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> GetPresetChildren()
        {
            var childCombos = Enum.GetValues<CustomComboPreset>().ToDictionary(
                tpl => tpl,
                tpl => new List<CustomComboPreset>());

            foreach (CustomComboPreset preset in Enum.GetValues<CustomComboPreset>())
            {
                CustomComboPreset? parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
                if (parent != null)
                    childCombos[parent.Value].Add(preset);
            }

            return childCombos.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
                    .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                    .OrderBy(tpl => tpl.Info.Order).ToArray())!;
        }

        public OpenWindow OpenWindow { get; set; } = OpenWindow.PvE;

        /// <summary> Initializes a new instance of the <see cref="ConfigWindow"/> class. </summary>
        public ConfigWindow() : base($"{P.Name} {P.GetType().Assembly.GetName().Version}###WrathCombo")
        {
            RespectCloseHotkey = true;

            SizeCondition = ImGuiCond.FirstUseEver;
            Size = new Vector2(800, 650);
            SetMinSize();

            Svc.PluginInterface.UiBuilder.DefaultFontHandle.ImFontChanged += SetMinSize;
        }

        private void SetMinSize(IFontHandle? fontHandle = null, ILockedImFont? lockedFont = null)
        {
            SizeConstraints = new()
            {
                MinimumSize = new Vector2(700f.Scale(), 10f.Scale())
            };
        }

        public override void Draw()
        {
            var region = ImGui.GetContentRegionAvail();
            var itemSpacing = ImGui.GetStyle().ItemSpacing;

            var topLeftSideHeight = region.Y;

            using var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, new Vector2(4, 0));
            using var table = ImRaii.Table("###MainTable", 2, ImGuiTableFlags.Resizable);
            if (!table)
                return;


            ImGui.TableSetupColumn("##LeftColumn", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 3);

            ImGui.TableNextColumn();

            var regionSize = ImGui.GetContentRegionAvail();

            ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));

            using (var leftChild = ImRaii.Child($"###WrathLeftSide", regionSize with { Y = topLeftSideHeight }, false, ImGuiWindowFlags.NoDecoration))
            {
                string? imagePath;
                try
                {
                    // Use the local image over a remote one
                    imagePath = Path.Combine(
                        Svc.PluginInterface.AssemblyLocation.Directory?.FullName!,
                        "images\\wrathcombo.png");
                    if (!File.Exists(imagePath))
                        throw new FileNotFoundException();
                }
                catch (Exception)
                {
                    // Fallback to the remote icon if there are any issues
                    imagePath = PunishLibMain.PluginManifest.IconUrl ?? "";
                }

                if (ThreadLoadImageHandler.TryGetTextureWrap(imagePath, out var logo))
                {
                    ImGuiEx.LineCentered("###WrathLogo", () =>
                    {
                        ImGui.Image(logo.ImGuiHandle, new(125f.Scale(), 125f.Scale()));
                    });

                }
                ImGui.Spacing();
                ImGui.Separator();

                if (ImGui.Selectable("PvE Features", OpenWindow == OpenWindow.PvE))
                {
                    OpenWindow = OpenWindow.PvE;
                }
                ImGui.Spacing();
                if (ImGui.Selectable("PvP Features", OpenWindow == OpenWindow.PvP))
                {
                    OpenWindow = OpenWindow.PvP;
                }
                ImGui.Spacing();
                if (ImGui.Selectable("Misc. Settings", OpenWindow == OpenWindow.Settings))
                {
                    OpenWindow = OpenWindow.Settings;
                }
                ImGui.Spacing();
                if (ImGui.Selectable("Auto-Rotation", OpenWindow == OpenWindow.AutoRotation))
                {
                    OpenWindow = OpenWindow.AutoRotation;
                }
                ImGui.Spacing();
                if (ImGui.Selectable("About", OpenWindow == OpenWindow.About))
                {
                    OpenWindow = OpenWindow.About;
                }

#if DEBUG
                ImGui.Spacing();
                if (ImGui.Selectable("DEBUG", OpenWindow == OpenWindow.Debug))
                {
                    OpenWindow = OpenWindow.Debug;
                }
                ImGui.Spacing();
#endif

                var conflictingPlugins = ConflictingPluginsCheck.TryGetConflictingPlugins();
                if (conflictingPlugins != null)
                {
                    ImGui.Spacing();
                    ImGui.Spacing();
                    const string conflictStringStart = "Conflicting Combo";
                    const string conflictStringEnd = "Plugins Detected!";

                    // Chop the text in half if it doesn't fit
                    ImGuiEx.LineCentered("###ConflictingPlugins", () =>
                    {
                        if (ImGui.GetColumnWidth() < ImGui.CalcTextSize(conflictStringStart + " " + conflictStringEnd).X.Scale())
                            ImGui.TextColored(ImGuiColors.DalamudYellow, conflictStringStart + "\n" + conflictStringEnd);
                        else
                            ImGui.TextColored(ImGuiColors.DalamudYellow, conflictStringStart + " " + conflictStringEnd);

                        // Tooltip with explanation
                        if (ImGui.IsItemHovered())
                        {
                            var conflictingPluginsText = "- " + string.Join("\n- ", conflictingPlugins);
                            var tooltipText =
                                "The following plugins are known to conflict " +
                                $"with {Svc.PluginInterface.InternalName}:\n" +
                                conflictingPluginsText +
                                "\n\nIt is recommended you disable these plugins to prevent\n" +
                                "unexpected behavior and bugs.";

                            ImGui.SetTooltip(tooltipText);
                        }
                    });
                }

            }

            ImGui.PopStyleVar();
            ImGui.TableNextColumn();
            using var rightChild = ImRaii.Child($"###WrathRightSide", Vector2.Zero, false);
            switch (OpenWindow)
            {
                case OpenWindow.PvE:
                    PvEFeatures.Draw();
                    break;
                case OpenWindow.PvP:
                    PvPFeatures.Draw();
                    break;
                case OpenWindow.Settings:
                    Settings.Draw();
                    break;
                case OpenWindow.About:
                    AboutTab.Draw(P.Name);
                    break;
                case OpenWindow.Debug:
                    Debug.Draw();
                    break;
                case OpenWindow.AutoRotation:
                    AutoRotationTab.Draw();
                    break;
                default:
                    break;
            };
        }



        public void Dispose()
        {
            Svc.PluginInterface.UiBuilder.DefaultFontHandle.ImFontChanged -= SetMinSize;
        }
    }

    public enum OpenWindow
    {
        None = 0,
        PvE = 1,
        PvP = 2,
        Settings = 3,
        AutoRotation = 4,
        About = 5,
        Debug = 6,
    }
}
