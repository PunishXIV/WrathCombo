#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using WrathCombo.Services;

#endregion

namespace WrathCombo.Window;

/// <summary>
///     A window to suggest and explain a settings change to the user.<br/>
///     Designed to be re-usable for future settings changes.<br/>
///     Just search for "UPDATE THIS VALUE" and update the value as needed.<br/>
///     (or follow the see-also links below)
/// </summary>
/// <seealso cref="SettingChangeWindow.Option"/>
/// <seealso cref="SettingChangeWindow.OptionSuggestedValue"/>
/// <seealso cref="SettingChangeWindow.VersionWhereProblemIntroduced"/>
/// <seealso cref="SettingChangeWindow.Explanation"/>
/// <seealso cref="SettingChangeWindow.Draw"/>
internal class SettingChangeWindow : Dalamud.Interface.Windowing.Window
{

    /// <summary>
    ///     The option value to be changed.
    /// </summary>
    /// <remarks>UPDATE THIS VALUE</remarks>
    private static double Option
    {
        get => Service.Configuration.MeleeOffset;
        set => Service.Configuration.MeleeOffset = value;
    }

    #region Option Value Checking

    /// <summary>
    ///     The current option value.<br/>
    ///     As a string to reduce code-change needed for future settings changes.
    /// </summary>
    /// <remarks>No need to update this value to re-use this window.</remarks>
    private static readonly string OptionValue =
        Option.ToString(CultureInfo.InvariantCulture).Trim();

    /// <summary>
    ///     The value this option should be.<br/>
    ///     As a string to reduce code-change needed for future settings changes.
    /// </summary>
    /// <remarks>UPDATE THIS VALUE</remarks>
    private const string OptionSuggestedValue = "0.0";

    /// <summary>
    ///     Whether the current value is problematic.
    /// </summary>
    /// <remarks>No need to update this value to re-use this window.</remarks>
    private static readonly bool IsValueProblematic =
        OptionValue != OptionSuggestedValue;

    #endregion

    #region Version Checking

    /// <summary>
    ///     The current plugin version.
    /// </summary>
    /// <remarks>No need to update this value to re-use this window.</remarks>
    private static readonly string Version =
        Svc.PluginInterface.Manifest.AssemblyVersion.ToString();

    /// <summary>
    ///     The version where the problem was introduced.
    /// </summary>
    /// <remarks>UPDATE THIS VALUE</remarks>
    private const string VersionWhereProblemIntroduced = "1.0.0.4";

    /// <summary>
    ///     Whether the current version is problematic.
    /// </summary>
    /// <remarks>No need to update this value to re-use this window.</remarks>
    private static readonly bool IsVersionProblematic =
        Version == VersionWhereProblemIntroduced;

    /// <summary>
    ///     Whether the suggestion should be hidden for this version.
    /// </summary>
    /// <remarks>No need to update this value to re-use this window.</remarks>
    private static readonly bool IsSuggestionHiddenForThisVersion =
        Service.Configuration.HideSettingsChangeSuggestionForVersion == Version;

    #endregion

    #region Explanation

    /// <summary>
    ///     The explanation for the setting change.
    /// </summary>
    /// <remarks>UPDATE THIS VALUE</remarks>
    private static readonly string Explanation =
        "The max melee range has been updated to correctly reflect the game's max melee range.\n" +
        "It has been changed from 3 to 3.5 yalms.\n\n" +
        "Your current setting for Melee Offset (in Misc Settings) is " +
        $"{Math.Round(Option, 1)} yalms.\n" +
        "This value used to make the maximum range for wrath be " +
        $"{Math.Round(Option+3, 1)} yalms, " +
        $"but would now make it {Math.Round(Option+3.5, 1)} yalms.\n" +
        "It is recommended you update this value, to prevent melee down-time.\n";

    #endregion

    /// <summary>
    ///     Create a new setting change window, with some settings about it.<br/>
    ///     Has logic to display the window, which does not need updated for future
    ///     settings changes.
    /// </summary>
    /// <remarks>No need to update this to re-use this window.</remarks>
    public SettingChangeWindow() : base("Wrath Combo | Setting Change")
    {
        if (IsVersionProblematic && IsValueProblematic &&
            !IsSuggestionHiddenForThisVersion)
            IsOpen = true;

        BringToFront();

        Flags = ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse
                | ImGuiWindowFlags.NoScrollbar;
    }

    /// <summary>
    ///     Set up the large WindowPadding to center the content.
    /// </summary>
    /// <remarks>No need to update this to re-use this window.</remarks>
    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(60, 60));
    }

    /// <summary>
    ///     Draw the settings change suggestion window.
    /// </summary>
    /// <remarks>UPDATE THIS VALUE (just 3 lines)</remarks>
    public override void Draw()
    {
        // UPDATE THIS VALUE
        var buttonText1 = "Update Melee Offset to 0.0";
        // UPDATE THIS VALUE
        var buttonText2 = "Close and Do Not Show again";

        var explanationText = Explanation.Split("\n");
        foreach (var line in explanationText)
            ImGuiHelpers.CenteredText(line);

        ImGuiHelpers.CenterCursorFor(
            ImGuiHelpers.GetButtonSize(buttonText1).X
            + ImGuiHelpers.GetButtonSize(buttonText2).X
            + ImGui.GetStyle().ItemSpacing.X * 2
        );

        if (ImGui.Button(buttonText1))
        {
            // UPDATE THIS VALUE (the cast)
            Option =
                double.Parse(OptionSuggestedValue, CultureInfo.InvariantCulture);
            Service.Configuration.Save();
            IsOpen = false;
        }

        ImGui.SameLine();

        if (ImGui.Button(buttonText2))
        {
            Service.Configuration.HideSettingsChangeSuggestionForVersion = Version;
            Service.Configuration.Save();
            IsOpen = false;
        }

        CenterWindow();
    }

    /// <summary>
    ///     Remove the large WindowPadding added in <see cref="PreDraw" />.
    /// </summary>
    public override void PostDraw()
    {
        ImGui.PopStyleVar(); // Remove the WindowPadding
    }

    #region Window Centering

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
        Position = rGameWindow.Position + new Vector2(
            rGameWindow.Size.X / 2 - vThisSize.X / 2,
            rGameWindow.Size.Y / 2 - vThisSize.Y / 2);
    }

    #endregion
}
