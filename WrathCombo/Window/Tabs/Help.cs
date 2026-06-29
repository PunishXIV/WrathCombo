#region

using System.Numerics;
using ECommons.ImGuiMethods;
using WrathCombo.Resources.Localization.UI.Help;

// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace WrathCombo.Window.Tabs;

internal class Help : ConfigWindow
{
    public enum Tab
    {
        Tips,
        GettingStarted,
        Specific,
        Optimization,
        WhatIsWrath,
        Troubleshooting,
        Development,
    }

    public enum SpecificTab
    {
        AutoRotation,
        Retargeting,
        ContentSpecificActions,
        OtherPlugins,
        PvP,
    }

    public static Tab?         OpenToSpecificTab;
    public static SpecificTab? OpenToSpecificSpecificTab;

    public static bool ShouldShowDevelopmentTab;

    public static Tab ActiveTab { get; private set; } = Tab.Tips;

    internal new static void Draw()
    {
        ImGui.TextWrapped(GetI18nText("section"));

        Spacing();
        ImGui.Separator();
        Spacing();

        if (!ImGui.BeginTabBar("helpTabs"))
        {
            ImGui.TextWrapped(GetI18nText("tabError"));
            return;
        }

        var requestedTab = OpenToSpecificTab;
        OpenToSpecificTab = null;

        var inDebugMode = false;
#if DEBUG
        inDebugMode = true;
#endif

        DrawTab(Tab.Tips, requestedTab == Tab.Tips);
        DrawTab(Tab.GettingStarted, requestedTab == Tab.GettingStarted);
        DrawTab(Tab.Specific, requestedTab == Tab.Specific);
        DrawTab(Tab.WhatIsWrath, requestedTab == Tab.WhatIsWrath);
        DrawTab(Tab.Optimization, requestedTab == Tab.Optimization);
        DrawTab(Tab.Troubleshooting, requestedTab == Tab.Troubleshooting);

        if (inDebugMode || requestedTab == Tab.Development)
        {
            ShouldShowDevelopmentTab = true;
            DrawTab(Tab.Development, requestedTab == Tab.Development);
        }

        ImGui.EndTabBar();
    }

    #region Helper Methods

    private static void DrawTab(Tab tab, bool selectTab)
    {
        var key = $"tab{tab}";
        if (ImGui.BeginTabItem(GetI18nText(key) + $"##{key}",
                selectTab ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None))
        {
            ActiveTab = tab;

            for (var i = 1; i <= 20; i++)
            {
                var textBlock = Text.GetLocalizedString($"{tab}Text{i}",
                    HelpUI.ResourceManager, true);
                if (textBlock is not null)
                    ImGui.TextWrapped(textBlock);
                else if (i == 1)
                    ImGui.TextWrapped(GetI18nText("noTabText"));
            }

            ImGui.EndTabItem();
        }
    }

    private static void Spacing(float height = 10, float width = 0) =>
        ImGuiEx.Spacing(new Vector2(width, height));

    private static string GetI18nText(string textName) =>
        Text.GetLocalizedString(textName,
            HelpUI.ResourceManager, true) ??
        $"DEFAULT [HELP] TEXT '{textName}'. PLEASE REPORT!";

    #endregion
}
