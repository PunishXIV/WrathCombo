#region

using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Common.Math;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WrathCombo.Core;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using WrathCombo.Services;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace WrathCombo.Window;

internal class MajorChangesWindow : Dalamud.Interface.Windowing.Window
{
    /// <summary>
    ///     Create a major changes window, with some settings about it.
    /// </summary>
    public MajorChangesWindow() : base("Wrath Combo | New Changes")
    {
        PluginLog.Debug(
            "MajorChangesWindow: " +
            $"IsVersionProblematic: {DoesVersionHaveChange}, " +
            $"IsSuggestionHiddenForThisVersion: {IsPopupHiddenForThisVersion}, " +
            $"WasUsingOldMCHTools: {WasUsingOldMCHTools}, " +
            $"WasUsingOldMNKBurstOptions: {WasUsingOldMNKConfigs}"
        );
        if (DoesVersionHaveChange &&
            !IsPopupHiddenForThisVersion)
            IsOpen = true;

        BringToFront();

        Flags = ImGuiWindowFlags.AlwaysAutoResize;
    }

    /// <summary>
    ///     Draw the settings change suggestion window.
    /// </summary>
    public override void Draw()
    {
        PadOutMinimumWidthFor("Wrath Combo | New Changes");

        #region MCH

        ImGuiEx.TextUnderlined("MCH Tools are now under their own Section");
        if (WasUsingOldMCHTools)
            ImGuiEx.Text(ImGuiColors.DalamudYellow,
                "You were using one of these options! Please Read!");
        ImGuiEx.Text(
            "All MCH Tool Options in the Single-Target Advanced Combo are now under their own section,\n" +
            "and this section needs enabled itself in order for the Tools Features to continue working.\n\n" +
            "You can find this new section under:\n" +
            "PvE Features > MCH > Single Target Advanced > Tools"
        );
        ImGui.NewLine();
        if (ImGui.Button("> Open Machinists Config##majorSettings1"))
            P.HandleOpenCommand(["MCH"], forceOpen: true);
        if (ImGui.Button("> Enable the new Section for me"))
        {
            PresetStorage.EnablePreset(Preset.MCH_ST_Adv_Tools);
            Service.Configuration.Save();
        }
        if (IsEnabled(Preset.MCH_ST_Adv_Tools))
        {
            ImGui.SameLine();
            FontAwesome.Print(ImGuiColors.HealerGreen, FontAwesomeIcon.Check);
            ImGui.SameLine();
            ImGuiEx.Text($"Enabled");
        }

        #endregion

        ImGuiEx.Spacing(new System.Numerics.Vector2(0, 10));
        ImGui.Separator();
        ImGuiEx.Spacing(new System.Numerics.Vector2(0, 10));

        #region Retargeting

        ImGuiEx.TextUnderlined("MNK Burst settings have moved slightly");
        if (WasUsingOldMNKConfigs)
            ImGuiEx.Text(ImGuiColors.DalamudYellow,
                "You were using one of these options! Please Read!");
        ImGuiEx.Text(
            "Monk's Burst settings (like the HP Slider and Boss-only option) are no longer on each Burst action,\n" +
            "and are instead now all together on the Burst section.\n" +
            "If you were using one of these options then you'll need to adjust these new settings.\n\n" +
            "You can find these moved settings here:\n" +
            "PvE Features > MNK > Single Target Advanced > Buffs Option");
        ImGui.NewLine();
        if (ImGui.Button("> Open Monks Config##majorSettings2"))
            P.HandleOpenCommand(["MNK"], forceOpen: true);

        #endregion

        #region Close and Do not Show again

        ImGuiEx.Spacing(new System.Numerics.Vector2(0, 20));
        ImGui.Separator();
        ImGuiHelpers.CenterCursorFor(
            ImGuiHelpers.GetButtonSize("Close and Do Not Show again").X
            //+ ImGui.GetStyle().ItemSpacing.X * 2
        );
        if (ImGui.Button("Close and Do Not Show again"))
        {
            Service.Configuration.HideMajorChangesForVersion = Version;
            Service.Configuration.Save();
            IsOpen = false;
        }

        #endregion

        if (_centeredWindow < 5)
            CenterWindow();
    }

    #region Minimum Width

    private void PadOutMinimumWidthFor(string windowName)
    {
        using (ImRaii.PushColor(ImGuiCol.Text, new Vector4(0)))
        {
            using (ImRaii.PushFont(UiBuilder.IconFont))
            {
                ImGui.Text(FontAwesomeIcon.CaretDown.ToIconString());
            }

            ImGui.SameLine();
            ImGui.Text(windowName);
            ImGui.SameLine();
            using (ImRaii.PushFont(UiBuilder.IconFont))
            {
                ImGui.Text(FontAwesomeIcon.Bars.ToIconString());
            }

            ImGui.SameLine();
            using (ImRaii.PushFont(UiBuilder.IconFont))
            {
                ImGui.Text(FontAwesomeIcon.Times.ToIconString());
            }
        }
    }

    #endregion

    #region Version Checking

    /// <summary>
    ///     The current plugin version.
    /// </summary>
    private static readonly Version Version =
        Svc.PluginInterface.Manifest.AssemblyVersion;

    /// <summary>
    ///     The version where the problem was introduced.
    /// </summary>
    private static readonly Version VersionWhereChangeIntroduced =
        new(1, 0, 2, 15);

    /// <summary>
    ///     Whether the current version is problematic.
    /// </summary>
    /// <remarks>No need to update this value to re-use this window.</remarks>
    private static readonly bool DoesVersionHaveChange =
        Version >= VersionWhereChangeIntroduced;

    /// <summary>
    ///     Whether the suggestion should be hidden for this version.
    /// </summary>
    private static readonly bool IsPopupHiddenForThisVersion =
        Service.Configuration.HideMajorChangesForVersion >= VersionWhereChangeIntroduced;

    #endregion

    #region Specific Info to Display for Update

    private static bool _getConfigValue(string config) =>
        PluginConfiguration.GetCustomBoolValue(config);

    /// <summary>
    ///     If the user was using MCH's Tools Presets.
    /// </summary>
    private static bool WasUsingOldMCHTools =>
        IsEnabled(Preset.MCH_ST_Adv_Drill) ||
        IsEnabled(Preset.MCH_ST_Adv_AirAnchor) ||
        IsEnabled(Preset.MCH_ST_Adv_Chainsaw) ||
        IsEnabled(Preset.MCH_ST_Adv_Excavator);

    /// <summary>
    ///     If the user was using MNK's old Burst Configs
    /// </summary>
    private static bool WasUsingOldMNKConfigs =>
        _getConfigValue("MNK_ST_BrotherhoodBossOption") ||
        _getConfigValue("MNK_ST_RiddleOfFire_SubOption") ||
        _getConfigValue("MNK_ST_RiddleOfWind_SubOption");

    #endregion

    #region Window Centering

    private static int _centeredWindow;

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(HandleRef hWnd, out Rect lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left; // x position of upper-left corner
        public int Top; // y position of upper-left corner
        public int Right; // x position of lower-right corner
        public int Bottom; // y position of lower-right corner
        public Vector2 Position => new Vector2(Left, Top);
        public Vector2 Size => new Vector2(Right - Left, Bottom - Top);
    }

    /// <summary>
    ///     Centers the GUI window to the game window.
    /// </summary>
    private void CenterWindow()
    {
        // Get the pointer to the window handle.
        var hWnd = IntPtr.Zero;
        foreach (var pList in Process.GetProcesses())
            if (pList.ProcessName is "ffxiv_dx11" or "ffxiv")
                hWnd = pList.MainWindowHandle;

        // If failing to get the handle then abort.
        if (hWnd == IntPtr.Zero)
            return;

        // Get the game window rectangle
        GetWindowRect(new HandleRef(null, hWnd), out var rGameWindow);

        // Get the size of the current window.
        var vThisSize = ImGui.GetWindowSize();

        // Set the position.
        var centeredPosition = rGameWindow.Position + new Vector2(
            rGameWindow.Size.X / 2 - vThisSize.X / 2,
            rGameWindow.Size.Y / 2 - vThisSize.Y / 2);
        ImGui.SetWindowPos(centeredPosition);
        Position = centeredPosition;
        PositionCondition = ImGuiCond.Once;

        _centeredWindow++;
    }

    #endregion
}
