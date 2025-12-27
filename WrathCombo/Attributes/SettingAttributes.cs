#region

using System;
// ReSharper disable ClassNeverInstantiated.Global

#endregion

// ReSharper disable InconsistentNaming

namespace WrathCombo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class SettingCategory(SettingCategory.Category category) : Attribute
{
    public enum Category
    {
        Main_UI_Options,
        Rotation_Behavior_Options,
        Targeting_Options,
        Troubleshooting_Options,
    }

    internal Category TheCategory { get; } = category;
}

[AttributeUsage(AttributeTargets.Field)]
public class SettingGroup(
    string groupName,
    string nameSpace,
    bool shouldThisGroupGetDisabled = true) : Attribute
{
    internal string GroupName { get; } = groupName;
    internal string NameSpace { get; } = nameSpace;
    internal bool ShouldThisGroupGetDisabled { get; } = shouldThisGroupGetDisabled;
}
[AttributeUsage(AttributeTargets.Field)]
public class SettingCollapsibleGroup(string groupName) : Attribute
{
    internal string GroupName { get; } = groupName;
}

[AttributeUsage(AttributeTargets.Field)]
public class SettingParent(string parentSettingFieldName) : Attribute
{
    internal string ParentSettingFieldName { get; } = parentSettingFieldName;
}

[AttributeUsage(AttributeTargets.Field)]
public class Setting(
    string name,
    string helpMark,
    string recommendedValue,
    string defaultValue,
    string unitLabel = "DEFAULT",
    Setting.Type type = Setting.Type.Toggle,
    string extraHelpMark = "DEFAULT",
    string warningMark = "DEFAULT",
    string extraText = "DEFAULT",
    float sliderMin = float.NaN,
    float sliderMax = float.NaN,
    string[]? stackStringsToExclude = null) : Attribute
{
    public enum Type
    {
        Toggle,
        Color,
        Number_Int,
        Number_Float,
        Slider_Int,
        Slider_Float,
        Stack,
    }
    
    internal string Name { get; } = name;
    internal string HelpMark { get; } = helpMark;
    internal string RecommendedValue { get; } = recommendedValue;
    internal string DefaultValue { get; } = defaultValue;
    internal string? UnitLabel { get; } =
        unitLabel == "DEFAULT" ? null : unitLabel;
    internal Type TheType { get; } = type;
    internal string? ExtraHelpMark { get; } = 
        extraHelpMark == "DEFAULT" ? null : extraHelpMark;
    internal string? WarningMark { get; } =
        warningMark == "DEFAULT" ? null : warningMark;
    internal string? ExtraText { get; } = 
        extraText == "DEFAULT" ? null : extraText;
    internal float? SliderMin { get; } = 
        float.IsNaN(sliderMin) ? null : sliderMin;
    internal float? SliderMax { get; } = 
        float.IsNaN(sliderMax) ? null : sliderMax;
    internal string[]? StackStringsToExclude { get; } = stackStringsToExclude;
}

[AttributeUsage(AttributeTargets.Field)]
public class SettingUI_Space : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class SettingUI_Or : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class SettingUI_RetargetIcon : Attribute
{
}