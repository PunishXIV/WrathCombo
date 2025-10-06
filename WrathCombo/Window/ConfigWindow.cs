using Dalamud.Interface;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.Throttlers;
using PunishLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using WrathCombo.Attributes;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.Data.Conflicts;
using WrathCombo.Services;
using WrathCombo.Window.Tabs;
using PunishGui = PunishLib.ImGuiMethods;
namespace WrathCombo.Window;

/// <summary> Plugin configuration window. </summary>
internal class ConfigWindow : Dalamud.Interface.Windowing.Window
{
    internal static readonly Dictionary<Job, List<(Preset Preset, CustomComboInfoAttribute Info)>> groupedPresets = GetGroupedPresets();
    internal static readonly Dictionary<Preset, (Preset Preset, CustomComboInfoAttribute Info)[]> presetChildren = GetPresetChildren();

    internal static float lastLeftColumnWidth;

    #region Search Variables
    internal static string Search = string.Empty;
    internal static string UsableSearch => Search.Trim().ToLowerInvariant();
    internal static bool SearchDescription = true;

    internal static bool IsSearching => !UsableSearch.IsNullOrWhitespace() &&
                                        UsableSearch.Length > 2;
    #endregion

    internal static Dictionary<Job, List<(Preset Preset, CustomComboInfoAttribute Info)>> GetGroupedPresets()
    {
        return Enum
            .GetValues<Preset>()
            .Where(preset => (int)preset > 100)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && PresetStorage.GetParent(tpl.Preset) == null)
            .OrderByDescending(tpl => tpl.Info.Role is JobRole.Tank)
            .ThenByDescending(tpl => tpl.Info.Role is JobRole.Healer)
            .ThenByDescending(tpl => tpl.Info.Role is JobRole.MeleeDPS)
            .ThenByDescending(tpl => tpl.Info.Role is JobRole.RangedDPS)
            .ThenByDescending(tpl => tpl.Info.Role is JobRole.MagicalDPS)
            .ThenByDescending(tpl => tpl.Info.Job is Job.ADV)
            .ThenByDescending(tpl => tpl.Info.Job is Job.MIN)
            //.ThenBy(tpl => tpl.Info.ClassJobCategory)
            .ThenBy(tpl => tpl.Info.Job)
            .ThenBy(tpl => tpl.Info.Order)
            .GroupBy(tpl => tpl.Info.Job)
            .ToDictionary(
                tpl => tpl.Key,
                tpl => tpl.ToList())!;
    }

    internal static Dictionary<Preset, (Preset Preset, CustomComboInfoAttribute Info)[]> GetPresetChildren()
    {
        var childCombos = Enum.GetValues<Preset>().ToDictionary(
            tpl => tpl,
            tpl => new List<Preset>());

        foreach (var preset in Enum.GetValues<Preset>())
        {
            var parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
            if (parent != null)
                childCombos[parent.Value].Add(preset);
        }

        return childCombos.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
                .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                .OrderBy(tpl => tpl.Info.Order).ToArray())!;
    }

    public OpenWindow OpenWindow
    {
        get;
        set
        {
            ClearAnySearches();
            field = value;
        }
    } = OpenWindow.PvE;

    /// <summary> Initializes a new instance of the <see cref="ConfigWindow"/> class. </summary>
    public ConfigWindow() : base($"{P.Name} {P.GetType().Assembly.GetName().Version}###WrathCombo")
    {
        RespectCloseHotkey = true;

        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(800, 650).Scale();
        SetMinSize();

        Svc.PluginInterface.UiBuilder.DefaultFontHandle.ImFontChanged += SetMinSize;
    }

    private void SetMinSize(IFontHandle? fontHandle = null, ILockedImFont? lockedFont = null) =>
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 100).Scale(),
        };

    public override void Draw()
    {
        var region = ImGui.GetContentRegionAvail();
        var topLeftSideHeight = region.Y;
        var columns = 2;
        var tableName = "###MainTable";
        if (Service.Configuration.UILeftColumnCollapsed)
        {
            columns = 1;
            tableName = "###NoSidebarMainTable";
        }

        using var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, new Vector2(4, 0).Scale());
        using (var table = ImRaii.Table(tableName, columns, ImGuiTableFlags.Resizable))
        {
            if (!table) return;

            if (!Service.Configuration.UILeftColumnCollapsed)
                DrawSidebar(topLeftSideHeight);
            else
                ImGui.Indent(45f.Scale());

            DrawBody();
        }

        DrawCollapseButton();
    }
    
    public static void ClearAnySearches()
    {
        Search = string.Empty;
        SearchDescription = true;
    }

    public override void OnClose()
    {
        ClearAnySearches();

        // Normal close
        base.OnClose();
    }

    private void DrawSidebar(float topLeftSideHeight)
    {
        var imageSize = new Vector2(125).Scale();
        var leftColumnFlags = ImGuiTableColumnFlags.WidthFixed;
        if (lastLeftColumnWidth < imageSize.X)
            leftColumnFlags |= ImGuiTableColumnFlags.NoResize;

        ImGui.TableSetupColumn("##LeftColumn", leftColumnFlags, imageSize.X + 10f.Scale());
        ImGui.TableNextColumn();

        var regionSize = ImGui.GetContentRegionAvail();
        lastLeftColumnWidth = regionSize.X;

        using var alignText = ImRaii.PushStyle(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));

        using var leftSide = (ImRaii.Child("###WrathLeftSide", regionSize with { Y = topLeftSideHeight }, false, ImGuiWindowFlags.NoDecoration));
        if (!leftSide)
        {
            ImGui.Dummy(Vector2.Zero);
            return;
        }

        string? imagePath;
        try
        {
            // Use the local image over a remote one
            imagePath = Path.Combine(
                Svc.PluginInterface.AssemblyLocation.Directory?.FullName!,
                "images\\wrathcombo.png");
            if (EzThrottler.Throttle("logTypeOfWrathIconUsed", 45000))
                PluginLog.Verbose("Using Local WrathCombo Icon");
        }
        catch (Exception)
        {
            // Fallback to the remote icon if there are any issues
            imagePath = Svc.PluginInterface.Manifest.IconUrl ?? "";
            if (EzThrottler.Throttle("logTypeOfWrathIconUsed", 45000))
                PluginLog.Verbose(
                    "Using Remote WrathCombo Icon\n             " +
                    Svc.PluginInterface.AssemblyLocation.Directory?.FullName! +
                    "images\\wrathcombo.png");
        }

        if (ThreadLoadImageHandler.TryGetTextureWrap(imagePath, out var logo))
            ImGuiEx.LineCentered("###WrathLogo", () =>
                ImGui.Image(logo.Handle, imageSize));

        ImGui.Spacing();
        ImGui.Separator();

        ImGui.Spacing();
        if (ImGui.Selectable("PvE Features", OpenWindow == OpenWindow.PvE))
            OpenWindow = OpenWindow.PvE;

        ImGui.Spacing();
        if (ImGui.Selectable("PvP Features", OpenWindow == OpenWindow.PvP))
            OpenWindow = OpenWindow.PvP;

        ImGui.Spacing();
        if (ImGui.Selectable("Auto-Rotation", OpenWindow == OpenWindow.AutoRotation))
            OpenWindow = OpenWindow.AutoRotation;

        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.Spacing();
        if (ImGui.Selectable("Settings", OpenWindow == OpenWindow.Settings))
            OpenWindow = OpenWindow.Settings;

        ImGui.Spacing();
        if (ImGui.Selectable("About", OpenWindow == OpenWindow.About))
            OpenWindow = OpenWindow.About;

#if DEBUG
        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.Spacing();
        if (ImGui.Selectable("DEBUG", OpenWindow == OpenWindow.Debug))
            OpenWindow = OpenWindow.Debug;

        ImGui.Spacing();
#endif

        ConflictingPlugins.Draw();
    }

    private void DrawBody()
    {
        ImGui.TableSetupColumn("##RightColumn", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableNextColumn();

        using var rightChild = ImRaii.Child("###WrathRightSide", Vector2.Zero, false);
        if (!rightChild) return;

        if (OpenWindow == OpenWindow.None)
            OpenWindow = OpenWindow.PvE;

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
                PunishGui.AboutTab.Draw(P.Name);
                break;
            case OpenWindow.Debug:
                Debug.Draw();
                break;
            case OpenWindow.AutoRotation:
                AutoRotationTab.Draw();
                break;
        };
    }

    private static void DrawCollapseButton()
    {
        var collapsed = Service.Configuration.UILeftColumnCollapsed;

        // Go to the bottom of the window
        ImGui.SetCursorPos(ImGui.GetCursorPos() with
        {
            X = 12f.Scale(),
            Y = ImGui.GetContentRegionMax().Y - 45f.Scale(),
        });

        // Calculate the size needed for the button
        var fPad = ImGui.GetStyle().FramePadding;
        Vector2 faSz;
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            faSz = ImGui.CalcTextSize("\uF0D9");
        }

        // Draw a window for the button, so clicks don't leak behind it
        using var overlay = ImRaii.Child("ButtonOverlay",
            new Vector2(faSz.X * 2 + fPad.X * 2,
                faSz.Y + 10f.Scale() + fPad.Y * 2),
            false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
        if (!overlay) return;

        // Set up how the button should display
        var icon = FontAwesomeIcon.CaretLeft;
        var hoverText = "Collapse Sidebar";
        ImGui.SetWindowFontScale(1.5f.Scale());
        if (collapsed)
        {
            icon = FontAwesomeIcon.CaretRight;
            hoverText = "Expand Sidebar";
        }

        // Draw the button
        if (ImGuiEx.IconButton(icon, "CollapseButton"))
        {
            Service.Configuration.UILeftColumnCollapsed = !collapsed;
            Service.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(hoverText);


        ImGui.SetWindowFontScale(1f);
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