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
            $"WasUsingOldMouseOverConfigs: {WasUsingOldMouseOverConfigs}"
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

        #region MouseOver Options moved

        ImGuiEx.TextUnderlined("Healer MouseOver Options are Moved!");
        if (WasUsingOldMouseOverConfigs)
            ImGuiEx.Text(ImGuiColors.DalamudYellow,
                "You were using one of these options! Please Read!");
        ImGuiEx.Text(
            "The option for each healer's healing combos to check MouseOver are gone,\n" +
            "and now are replaced with a global mouseover option (and some new ones).\n\n" +
            "You can find this new setting under:\n" +
            "Settings > 'Target Options' > 'Heal Stack Customization Options'"
        );
        ImGui.NewLine();
        if (ImGui.Button("> Open the Settings Tab##majorSettings1"))
            P.OnOpenConfigUi();
        if (ImGui.Button("> Enable the new UI MouseOver option for me"))
        {
            Service.Configuration.UseUIMouseoverOverridesInDefaultHealStack = true;
            Service.Configuration.Save();
        }
        if (Service.Configuration.UseUIMouseoverOverridesInDefaultHealStack)
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

        ImGuiEx.TextUnderlined("New Feature: Action Retargeting!");
        ImGuiEx.Text(
            "Action Retargeting allows us to pick actions' targets for you, based on\n" +
            "The Balance's recommendations and your options, without you having to\n" +
            "setup Redirect or Reaction.");
        ImGuiComponents.HelpMarker(
            "Previously there were a few features (like AST's Earthly Star) that\n" +
            "required Redirect or Reaction to work, and Single-Target Healing combos\n" +
            "were checking HP of your MouseOver (optionally) > Soft Target > Hard Target,\n" +
            "which may not have lined up with your targeting, and used the 'wrong' heals.\n\n" +
            "Action Retargeting addresses that!"
        );
        ImGuiEx.Text(
            "Additionally, we have added the ability to control the 'Stack' of targets\n" +
            "that Healing combos will use to check HP and choose to cast different heals,\n" +
            "and an option to also Retarget all Single-Target Healing actions to that same Stack.\n" +
            "(This option, 'Retarget Healing Actions', is highly recommended!)");
        ImGuiEx.Text(
            "You can find these new settings under:\n" +
            "Settings > 'Target Options' (and the collapsed 'Heal Stack Customization Options')"
        );
        ImGui.NewLine();
        if (ImGui.Button("> Open the Settings Tab##majorSettings2"))
            P.OnOpenConfigUi();
        if (ImGui.Button("> Enable the Retarget Healing Actions option for me"))
        {
            Service.Configuration.RetargetHealingActionsToStack = true;
            Service.Configuration.Save();
        }
        if (Service.Configuration.RetargetHealingActionsToStack)
        {
            ImGui.SameLine();
            FontAwesome.Print(ImGuiColors.HealerGreen, FontAwesomeIcon.Check);
            ImGui.SameLine();
            ImGuiEx.Text($"Enabled");
        }
        ImGui.NewLine();
        ImGuiEx.Text(
            "You will find new symbols indicating if a Feature's actions are Retargeted:"
        );
        ImGuiEx.Text("Depending on settings, MAY be Retargeted:");
        ImGui.SameLine();
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudYellow))
                ImGui.Text(FontAwesomeIcon.Random.ToIconString());
        }
        ImGui.SameLine();
        ImGuiEx.Text("WILL always be Retargeted:");
        ImGui.SameLine();
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.ParsedGreen))
                ImGui.Text(FontAwesomeIcon.Random.ToIconString());
        }
        ImGui.NewLine();
        ImGuiEx.Text(ImGuiColors.DalamudYellow,
            "If you previously had Redirect/Reaction configured for actions that are\n" +
            "now Retargeted, or had Reaction's/Bossmod's Instant Ground Target options,\n" +
            "you may likely want to disable those options.");
        ImGuiEx.Text(
            "That incudes AST Cards, DNC Partner, and (if enabled:)\n" +
            "Single-Target Healing Actions");
        ImGuiComponents.HelpMarker(
            "Healing actions is up to preference whether you choose to enable that\n" +
            "in settings (highly recommended), but Dance Partner and Cards are now smarter\n" +
            "than simple retargeting of actions (following The Balance's priorities,\n" +
            "checking for damage downs, etc).");

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
        new(1, 0, 1, 6);

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
    ///     If the user was using MouseOver options.
    /// </summary>
    private static bool WasUsingOldMouseOverConfigs =>
        _getConfigValue("AST_ST_SimpleHeals_UIMouseOver") ||
        _getConfigValue("SCH_ST_Heal_UIMouseOver") ||
        _getConfigValue("SCH_DeploymentTactics_UIMouseOver") ||
        _getConfigValue("SGE_ST_Heal_UIMouseOver") ||
        _getConfigValue("WHM_STHeals_UIMouseOver");

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
