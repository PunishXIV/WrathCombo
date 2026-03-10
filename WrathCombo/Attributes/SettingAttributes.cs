#region

using System;
using WrathCombo.Resources.Localization.UI.Settings;
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

    public string LocalizedCategoryName
    {
        get
        {
            return TheCategory switch
            {
                Category.Main_UI_Options => SettingsCfgUI.MainUIOptions_Category,
                Category.Rotation_Behavior_Options => SettingsCfgUI.RotationBehaviourOptions_Category,
                Category.Targeting_Options => SettingsCfgUI.TargetingOptions_Category,
                Category.Troubleshooting_Options => SettingsCfgUI.TroubleshootingOptions_Category,
                _ => "UNKNOWN"
            };
        }
    }
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
    Setting.Type type,
    float minFloat = float.NaN,
    float maxFloat = float.NaN,
    int minInt = int.MinValue,
    int maxInt = int.MaxValue,
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
    
    internal string Name { get; }
    internal string HelpMark { get; } 
    internal string RecommendedValue { get; } 
    internal string DefaultValue { get; } 
    internal string? UnitLabel { get; }
    internal Type TheType { get; } = type;
    internal string? ExtraHelpMark { get; }
    internal string? WarningMark { get; }
    internal string? ExtraText { get; }
    internal float? MinFloat { get; } = 
        float.IsNaN(minFloat) ? null : minFloat;
    internal float? MaxFloat { get; } = 
        float.IsNaN(maxFloat) ? null : maxFloat;
    internal int? MinInt { get; } =
        minInt == int.MinValue ? null : minInt;
    internal int? MaxInt { get; } =
        maxInt == int.MaxValue ? null : maxInt;

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