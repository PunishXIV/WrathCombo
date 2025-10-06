#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using WrathCombo.Attributes;
using WrathCombo.Core;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

#endregion

namespace WrathCombo.Window;

internal class FeaturesWindow : ConfigWindow
{
    public enum FeatureTab
    {
        Normal,
        Variant,
        Bozja,
        Eureka,
        OccultCrescent,
    }

    private const StringComparison Lower = StringComparison.OrdinalIgnoreCase;

    internal static int ColCount = 1;
    internal static FeatureTab CurrentTab = FeatureTab.Normal;
    internal static int CurrentPreset = 1;

    internal static float IndentWidth = 12f.Scale();
    internal static float LargerIndentWidth = IndentWidth + 42f.Scale();
    internal static float IconMaxSize = 34f.Scale();

    internal static Job? OpenJob
    {
        get;
        set
        {
            field = value;
            ClearAnySearches();
        }
    }

    internal static Job? OpenPvPJob
    {
        get;
        set
        {
            ClearAnySearches();
            field = value;
        }
    }

    internal static float VerticalCenteringPadding =>
        (IconMaxSize - ImGui.GetTextLineHeight()) / 2f;

    internal static float AvailableWidth => ImGui.GetContentRegionAvail().X;
    internal static float LetterWidth => ImGui.CalcTextSize("W").X.Scale();

    public static void DrawHeader(Job job, bool pvp = false)
    {
        var name = !pvp ? $"{OpenJob?.Name()}" : $"{OpenPvPJob?.Name()}";
        var icon = Icons.GetJobIcon(job);

        using var header = ImRaii.Child((pvp ? "PvP" : "") + "HeadingTab",
            new Vector2(AvailableWidth, IconMaxSize));
        if (!header)
            return;

        #region Back Button

        if (ImGui.Button("Back", new Vector2(0, 24f.Scale())))
        {
            if (!pvp)
                OpenJob = null;
            else
                OpenPvPJob = null;
            return;
        }

        ImGui.SameLine();

        #endregion

        #region Image Sizing

        var imgSize = Vector2.Zero;
        var imgPadSize = 0f;
        if (icon != null)
        {
            var imgScale = Math.Min(IconMaxSize / icon.Size.X,
                IconMaxSize / icon.Size.Y);
            imgSize = new Vector2(icon.Size.X * imgScale,
                icon.Size.Y * imgScale);
            imgPadSize = (IconMaxSize - imgSize.X) / 2f;
        }

        #endregion

        #region Centering

        var lineWidth =
            // image
            imgSize.X
            + imgPadSize
            // job name
            + ImGui.GetStyle().ItemSpacing.X
            + ImGui.CalcTextSize(name).X;
        ImGui.SetCursorPosX((AvailableWidth - lineWidth) / 2f);

        #endregion

        #region Job Icon

        if (icon != null)
        {
            if (imgPadSize > 0)
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + imgPadSize);
            ImGui.Image(icon.Handle, imgSize);
        }
        else
        {
            ImGui.Dummy(new Vector2(IconMaxSize, IconMaxSize));
        }

        ImGui.SameLine();

        #endregion

        ImGuiEx.Spacing(new Vector2(0, VerticalCenteringPadding - 2f.Scale()));
        ImGuiEx.Text(name);

        #region IPC Indicator

        if (!pvp && P.UIHelper.JobControlled(job) is not null)
        {
            ImGui.SameLine();
            P.UIHelper
                .ShowIPCControlledIndicatorIfNeeded(job);
        }

        #endregion
    }

    public static void DrawSearchBar()
    {
        if (!Service.Configuration.UIShowSearchBar)
            return;

        using var id = ImRaii.Child("SearchBar",
            new Vector2(AvailableWidth, 22f.Scale()));
        if (!id)
            return;

        var searchLabelText = "Search:";
        var searchHintText = "Option name, ID, Internal Name, Description, etc";
        var searchDescriptionText = "Descriptions";

        var searchWidth = LetterWidth * 30f + 4f.Scale();
        // line width for the search bar
        var lineWidth = searchWidth
                        // label
                        + ImGui.CalcTextSize(searchLabelText).X
                        + ImGui.GetStyle().ItemSpacing.X
                        // checkbox
                        + ImGui.GetStyle().ItemSpacing.X * 2
                        + ImGui.GetFrameHeight()
                        + ImGui.GetStyle().FramePadding.X * 2
                        + ImGui.CalcTextSize(searchDescriptionText).X;
        ImGui.SetCursorPosX((AvailableWidth - lineWidth) / 2f);

        ImGui.Text(searchLabelText);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(searchWidth);
        ImGui.InputTextWithHint(
            "", searchHintText,
            ref Search, 30,
            ImGuiInputTextFlags.AutoSelectAll);
        ImGui.SameLine();
        ImGui.Checkbox(searchDescriptionText, ref SearchDescription);
    }

    public static void SetCurrentTab(FeatureTab tab)
    {
        // Reset Search if changing tabs
        if (CurrentTab != tab)
            ClearAnySearches();

        CurrentTab = tab;
        CurrentPreset = 1;
    }

    internal static bool PresetMatchesSearch(Preset preset)
    {
        if (!IsSearching)
            return false;

        if (PresetStorage.ShouldBeHidden(preset))
            return false;

        if (!Presets.Attributes.TryGetValue(preset, out var attributes))
            attributes = new Presets.PresetAttributes(preset);
        
        if (UsableSearch == "erp")
            return false;

        // Keyword matching
        if (TryFindKeywordsInSearch(preset, out var matchesKeyWords))
            return matchesKeyWords;

        // ID matching
        if (UsableSearch.Replace(" ", "").All(char.IsDigit) &&
            int.TryParse(UsableSearch.Replace("_", ""), out var searchNum) &&
            (int)preset == searchNum)
            return true;

        // Internal name matching
        if (preset.ToString().Contains(UsableSearch, Lower))
            return true;

        // Internal name matching (without underscores)
        if (preset.ToString().Replace("_", "")
            .Contains(UsableSearch.Replace("_", ""), Lower))
            return true;

        // Title matching
        if (attributes.CustomComboInfo.Name.Contains(UsableSearch, Lower))
            return true;

        // Title matching (without spaces)
        if (attributes.CustomComboInfo.Name.Replace(" ", "")
            .Contains(UsableSearch.Replace(" ", ""), Lower))
            return true;

        // Title matching (without punctuation or spaces)
        if (new string(attributes.CustomComboInfo.Name.Replace(" ", "")
                .Where(c => c == '!' || !char.IsPunctuation(c))
                .ToArray())
            .Contains(new string(UsableSearch.Replace(" ", "")
                .Where(c => c == '!' || !char.IsPunctuation(c))
                .ToArray()), Lower))
            return true;

        if (SearchDescription)
        {
            // Description matching
            if (attributes.CustomComboInfo.Description.Contains(UsableSearch, Lower))
                return true;

            // Description matching (without spaces)
            if (attributes.CustomComboInfo.Description.Replace(" ", "")
                .Contains(UsableSearch.Replace(" ", ""), Lower))
                return true;

            // Description matching (without punctuation or spaces)
            if (new string(attributes.CustomComboInfo.Description.Replace(" ", "")
                    .Where(c => c == '!' || !char.IsPunctuation(c))
                    .ToArray())
                .Contains(new string(UsableSearch.Replace(" ", "")
                    .Where(c => c == '!' || !char.IsPunctuation(c))
                    .ToArray()), Lower))
                return true;
        }

        return false;
    }

    private static bool TryFindKeywordsInSearch
        (Preset preset, out bool matchesKeyWords)
    {
        matchesKeyWords = false;
        var search = new string(UsableSearch
            .Replace(" ", "")
            .Where(c => c == '!' || !char.IsPunctuation(c))
            .ToArray());
        var attributes = preset.Attributes();

        switch (search)
        {
            case "!auto":
            case "!automode":
            case "!autorotation":
            case "!autorot":
                matchesKeyWords = attributes.AutoAction is not null;
                return true;

            case "!secret":
            case "!hidden":
                matchesKeyWords = Service.Configuration.ShowHiddenFeatures &&
                                  attributes.Hidden is not null;
                return true;

            case "!retargeting":
            case "!retargeted":
                matchesKeyWords = attributes.RetargetedAttribute is not null ||
                                  attributes.PossiblyRetargeted is not null;
                return true;

            case "!maincombo":
            case "!maincombos":
                matchesKeyWords = attributes.ComboType is
                    ComboType.Advanced or ComboType.Simple or ComboType.Healing;
                return true;

            case "!combo":
            case "!combos":
                matchesKeyWords = attributes.ComboType is not
                    (ComboType.Feature or ComboType.Option);
                return true;

            case "!feature":
            case "!features":
                matchesKeyWords = attributes.ComboType is ComboType.Feature;
                return true;
        }

        return false;
    }

    internal static void SearchMorePresets
        (Preset[] presetsToSearch, List<Preset>? alreadyShown = null)
    {
        alreadyShown ??= [];

        foreach (var preset in presetsToSearch)
        {
            var attributes = preset.Attributes();

            if (!PresetMatchesSearch(preset))
                continue;

            // Don't show things that were already shown under another preset
            if (alreadyShown.Any(y => y == preset) ||
                alreadyShown.Any(y => y == attributes.Parent) ||
                alreadyShown.Any(y => y == attributes.GrandParent) ||
                alreadyShown.Any(y => y == attributes.GreatGrandParent))
                continue;

            var info = attributes.CustomComboInfo;
            InfoBox presetBox = new()
            {
                ContentsOffset = 5f.Scale(),
                ContentsAction = () => { Presets.DrawPreset(preset, info!); }
            };
            presetBox.Draw();
            ImGuiEx.Spacing(new Vector2(0, 12));
            alreadyShown.Add(preset);
        }
    }

    internal static void ShowSearchErrorIfNoResults()
    {
        if (CurrentPreset > 1 || !IsSearching)
            return;
        
        if (UsableSearch == "erp")
        {
            ImGuiEx.LineCentered(() => { ImGui.Text("Behave!"); });
            return;
        }

        var error = "Nothing matched your search.";

        if (UsableSearch.StartsWith('!'))
            error += "\nMake sure your keyword is valid.";

        ImGuiEx.LineCentered(() => { ImGui.Text(error); });
    }
}