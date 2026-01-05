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
            $"WasUsingOldDRKOptions: {WasUsingOldDRKConfigs}, " +
            $"WasUsingOldGNBOptions: {WasUsingOldGNBConfigs}, " +
            $"WasUsingOldPLDOptions: {WasUsingOldPLDConfigs}, " +
            $"WasUsingOldWAROptions: {WasUsingOldWARConfigs}"
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

        #region Tanks

        ImGuiEx.TextUnderlined("Tank In-Combo Mitigation Has Been Overhauled and " +
            "Moved");
        if (WasUsingOldDRKConfigs || WasUsingOldGNBConfigs ||
            WasUsingOldPLDConfigs || WasUsingOldWARConfigs)
            ImGuiEx.Text(ImGuiColors.DalamudYellow,
                "You had one of these tanks configured! Please Read!");
        ImGuiEx.Text(
            "Tanks have had their In-Combo Mitigation completely overhauled.\n" +
            "No longer is their mitigation settings in each combo, and " +
            "no longer is mitigation entirely triggered by HP%.\n" +
            "Mitigation is now in its own Section, under the Advanced Combos, " +
            "and those options apply to the Advanced Combos\nif they have " +
            "'Include Mitigations' checked.\n" +
            "(Simple Modes will use the recommended values for these settings, " +
            "if 'Include Mitigations' is checked there)" +
            "\n\n" +
            "You can find these moved mitigation settings here:\n" +
            "PvE Features > [Tank of choice] > Advanced Mitigation Options\n\n" +
            "Make sure Mitigation is enabled in combos for these to work:\n" +
            "PvE Features > [Tank of choice] > Advanced/Simple Combo > Include " +
            "Mitigations");
        ImGui.NewLine();
        if (ImGui.Button("> Open Dark Knight's Config##majorSettings2"))
            P.HandleOpenCommand(["DRK"], forceOpen: true);
        ImGui.SameLine();
        ImGui.Text("(then just search for 'Mitigation')");
        if (ImGui.Button("> Open Gun Breaker's Config##majorSettings2"))
            P.HandleOpenCommand(["GNB"], forceOpen: true);
        if (ImGui.Button("> Open Paladin's Config##majorSettings3"))
            P.HandleOpenCommand(["PLD"], forceOpen: true);
        if (ImGui.Button("> Open Warrior's Config##majorSettings4"))
            P.HandleOpenCommand(["WAR"], forceOpen: true);

        #endregion

        ImGuiEx.Spacing(new System.Numerics.Vector2(0, 10));
        ImGui.Separator();
        ImGuiEx.Spacing(new System.Numerics.Vector2(0, 10));

        #region Raidwides/Tankbusters
        
        ImGuiEx.TextUnderlined("Raidwide-Detection Improved, Tankbuster Detection Added");
        ImGuiEx.Text(
            "Raidwide detection has been improved to now also check for nearby\n" +
            "stack marker visual effects, in addition to the existing raidwide " +
            "cast detection.\n" +
            "(No change by you is needed for this, it's just expanded now)" +
            "\n\n" +
            "Tankbuster detection has been added by checking for visual effects!\n" +
            "Options to mitigate as a tank for these is in the new mitigation " +
            "options above.\n\n" +
            "Find the new tankbuster mitigation options here:\n" +
            "PvE Features > [Tank of choice] > Advanced Mitigation Options > " +
            "Boss Encounter\n" +
            "(can also search for 'Tankbuster' in your tank's configuration)"
        );

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
        new(1, 0, 3, 4);

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
        Configuration.GetCustomIntValue(config) > 0;

    /// <summary>
    ///     If the user was using DRK's old Burst Configs
    /// </summary>
    private static bool WasUsingOldDRKConfigs =>
        _getConfigValue("DRK_ST_MitDifficulty") ||
        _getConfigValue("DRK_ST_TBNThreshold") ||
        _getConfigValue("DRK_ST_TBNBossRestriction") ||
        _getConfigValue("DRK_ST_OblationCharges") ||
        _getConfigValue("DRK_ST_Mit_OblationThreshold") ||
        _getConfigValue("DRK_ST_Mit_MissionaryThreshold") ||
        _getConfigValue("DRK_ST_ShadowedVigilThreshold") ||
        _getConfigValue("DRK_ST_LivingDeadSelfThreshold") ||
        _getConfigValue("DRK_ST_LivingDeadTargetThreshold") ||
        _getConfigValue("DRK_ST_LivingDeadBossRestriction") ||
        _getConfigValue("DRK_AoE_OblationCharges") ||
        _getConfigValue("DRK_AoE_Mit_OblationThreshold") ||
        _getConfigValue("DRK_AoE_ReprisalEnemyCount") ||
        _getConfigValue("DRK_AoE_Mit_DarkMindThreshold") ||
        _getConfigValue("DRK_AoE_Mit_RampartThreshold") ||
        _getConfigValue("DRK_AoE_Mit_ReprisalThreshold") ||
        _getConfigValue("DRK_AoE_ArmsLengthEnemyCount") ||
        _getConfigValue("DRK_AoE_ShadowedVigilThreshold") ||
        _getConfigValue("DRK_AoE_LivingDeadSelfThreshold") ||
        _getConfigValue("DRK_AoE_LivingDeadTargetThreshold");

    /// <summary>
    ///     If the user was using GNB's old Burst Configs
    /// </summary>
    private static bool WasUsingOldGNBConfigs =>
        _getConfigValue("GNB_ST_Corundum_Health") ||
        _getConfigValue("GNB_ST_Corundum_SubOption") ||
        _getConfigValue("GNB_ST_Aurora_Health") ||
        _getConfigValue("GNB_ST_Aurora_Charges") ||
        _getConfigValue("GNB_ST_Aurora_SubOption") ||
        _getConfigValue("GNB_ST_Rampart_Health") ||
        _getConfigValue("GNB_ST_Rampart_SubOption") ||
        _getConfigValue("GNB_ST_Camouflage_Health") ||
        _getConfigValue("GNB_ST_Camouflage_SubOption") ||
        _getConfigValue("GNB_ST_Nebula_Health") ||
        _getConfigValue("GNB_ST_Nebula_SubOption") ||
        _getConfigValue("GNB_ST_Superbolide_Health") ||
        _getConfigValue("GNB_ST_Superbolide_SubOption") ||
        _getConfigValue("GNB_ST_Reprisal_Health") ||
        _getConfigValue("GNB_ST_Reprisal_SubOption") ||
        _getConfigValue("GNB_ST_ArmsLength_Health") ||
        _getConfigValue("GNB_AoE_Corundum_Health") ||
        _getConfigValue("GNB_AoE_Corundum_SubOption") ||
        _getConfigValue("GNB_AoE_Aurora_Health") ||
        _getConfigValue("GNB_AoE_Aurora_Charges") ||
        _getConfigValue("GNB_AoE_Aurora_SubOption") ||
        _getConfigValue("GNB_AoE_Rampart_Health") ||
        _getConfigValue("GNB_AoE_Rampart_SubOption") ||
        _getConfigValue("GNB_AoE_Camouflage_Health") ||
        _getConfigValue("GNB_AoE_Camouflage_SubOption") ||
        _getConfigValue("GNB_AoE_Nebula_Health") ||
        _getConfigValue("GNB_AoE_Nebula_SubOption") ||
        _getConfigValue("GNB_AoE_Superbolide_Health") ||
        _getConfigValue("GNB_AoE_Superbolide_SubOption") ||
        _getConfigValue("GNB_AoE_Reprisal_Health") ||
        _getConfigValue("GNB_AoE_Reprisal_SubOption") ||
        _getConfigValue("GNB_AoE_ArmsLength_Health ");

    /// <summary>
    ///     If the user was using PLD's old Burst Configs
    /// </summary>
    private static bool WasUsingOldPLDConfigs =>
        _getConfigValue("PLD_ST_SheltronOption") ||
        _getConfigValue("PLD_ST_Sheltron_Health") ||
        _getConfigValue("PLD_ST_Sentinel_Health") ||
        _getConfigValue("PLD_ST_Bulwark_Health") ||
        _getConfigValue("PLD_ST_HallowedGround_Health") ||
        _getConfigValue("PLD_ST_MitHallowedGroundBoss") ||
        _getConfigValue("PLD_ST_MitSheltronBoss") ||
        _getConfigValue("PLD_AoE_SheltronOption") ||
        _getConfigValue("PLD_AoE_Sheltron_Health") ||
        _getConfigValue("PLD_AoE_DivineVeil_Health") ||
        _getConfigValue("PLD_AoE_Rampart_Health") ||
        _getConfigValue("PLD_AoE_Reprisal_Health") ||
        _getConfigValue("PLD_AoE_Reprisal_Count") ||
        _getConfigValue("PLD_AoE_ArmsLength_Count") ||
        _getConfigValue("PLD_AoE_Sentinel_Health") ||
        _getConfigValue("PLD_AoE_Bulwark_Health") ||
        _getConfigValue("PLD_AoE_HallowedGround_Health") ||
        _getConfigValue("PLD_Mit_HallowedGround_Max_Difficulty") ||
        _getConfigValue("PLD_ST_Mit_Difficulty");


    /// <summary>
    ///     If the user was using WAR's old Burst Configs
    /// </summary>
    private static bool WasUsingOldWARConfigs =>
        _getConfigValue("WAR_ST_Bloodwhetting_Health") ||
        _getConfigValue("WAR_ST_Bloodwhetting_Boss") ||
        _getConfigValue("WAR_ST_Equilibrium_Health") ||
        _getConfigValue("WAR_ST_Thrill_Health") ||
        _getConfigValue("WAR_ST_Vengeance_Health") ||
        _getConfigValue("WAR_ST_Holmgang_Health") ||
        _getConfigValue("WAR_AoE_Bloodwhetting_Health") ||
        _getConfigValue("WAR_AoE_Equilibrium_Health") ||
        _getConfigValue("WAR_AoE_Rampart_Health") ||
        _getConfigValue("WAR_AoE_Thrill_Health") ||
        _getConfigValue("WAR_AoE_Vengeance_Health") ||
        _getConfigValue("WAR_AoE_Holmgang_Health") ||
        _getConfigValue("WAR_AoE_Reprisal_Health") ||
        _getConfigValue("WAR_AoE_ShakeItOff_Health") ||
        _getConfigValue("WAR_AoE_Reprisal_Count") ||
        _getConfigValue("WAR_AoE_ArmsLength_Count") ||
        _getConfigValue("WAR_AoE_MitsOptions") ||
        _getConfigValue("WAR_ST_HolmgangBoss");
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
