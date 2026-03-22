#region

using System;
using System.Collections.Generic;
using System.Reflection;
using ECommons.Reflection;
using WrathCombo.Attributes;
using WrathCombo.Core;
using WrathCombo.Resources.Localization.UI.Settings;
using WrathCombo.Services;
using SettingType = WrathCombo.Attributes.Setting.Type;
using Category = WrathCombo.Attributes.SettingCategory.Category;
using ECommons.DalamudServices;

#endregion

namespace WrathCombo.Window.Functions;

public class Setting
{
    public Setting(string settingName)
    {
        if (ConfigurationType.GetField(settingName) is { } field)
            _field = field;
        else
            throw new ArgumentException(
                $"Setting '{settingName}' not found in Configuration class.");
        FieldName = settingName;

        #region Loading from Cache
        if (CachedSettings.TryGetValue(settingName, out var cachedSetting))
        {
            Category              = cachedSetting.Category;
            Name                  = cachedSetting.Name;
            HelpMark              = cachedSetting.HelpMark;
            RecommendedValue      = cachedSetting.RecommendedValue;
            DefaultValue          = cachedSetting.DefaultValue;
            Type                  = cachedSetting.Type;
            UnitLabel             = cachedSetting.UnitLabel;
            ExtraHelpMark         = cachedSetting.ExtraHelpMark;
            WarningMark           = cachedSetting.WarningMark;
            ExtraText             = cachedSetting.ExtraText;
            MinFLoat              = cachedSetting.MinFLoat;
            MaxFloat              = cachedSetting.MaxFloat;
            MinInt                = cachedSetting.MinInt;
            MaxInt                = cachedSetting.MaxInt;
            StackStringsToExclude = cachedSetting.StackStringsToExclude;
            GroupName             = cachedSetting.GroupName;
            GroupNameSpace        = cachedSetting.GroupNameSpace;
            GroupShouldBeDisabled = cachedSetting.GroupShouldBeDisabled;
            CollapsibleGroupName  = cachedSetting.CollapsibleGroupName;
            Parent                = cachedSetting.Parent;
            ShowSpace             = cachedSetting.ShowSpace;
            ShowOr                = cachedSetting.ShowOr;
            ShowRetarget          = cachedSetting.ShowRetarget;
            return;
        }

        #endregion

        #region Loading from Attributes

        var catAtt = _field.GetCustomAttribute<SettingCategory>() ??
                   throw new ArgumentException(
                       $"Setting `{settingName}` is missing required " +
                       $"`SettingCategory` attribute.");
        Category              = catAtt.TheCategory;
        CategoryName          = catAtt.LocalizedCategoryName;


        var setting = _field.GetCustomAttribute<Attributes.Setting>() ??
                      throw new ArgumentException(
                          $"Setting `{settingName}` is missing required " +
                          $"`Setting` attribute.");
        Name                  = Text.GetLocalizedString($"{settingName}_Name", SettingsCfgUI.ResourceManager)!;
        HelpMark              = Text.GetLocalizedString($"{settingName}_helpMark", SettingsCfgUI.ResourceManager)!;
        RecommendedValue      = Text.GetLocalizedString($"{settingName}_recommendedValue", SettingsCfgUI.ResourceManager)!;
        DefaultValue          = Text.GetLocalizedString($"{settingName}_defaultValue", SettingsCfgUI.ResourceManager)!;
        Type                  = setting.TheType;
        UnitLabel             = Text.GetLocalizedString($"{settingName}_unitLabel", SettingsCfgUI.ResourceManager, true);
        ExtraHelpMark         = Text.GetLocalizedString($"{settingName}_extraHelpMark", SettingsCfgUI.ResourceManager, true);
        WarningMark           = Text.GetLocalizedString($"{settingName}_warningMark", SettingsCfgUI.ResourceManager, true);
        ExtraText             = Text.GetLocalizedString($"{settingName}_extraText", SettingsCfgUI.ResourceManager, true);
        MinFLoat              = setting.MinFloat;
        MaxFloat              = setting.MaxFloat;
        MinInt                = setting.MinInt;
        MaxInt                = setting.MaxInt;
        StackStringsToExclude = setting.StackStringsToExclude;

        var group = _field.GetCustomAttribute<SettingGroup>();

        GroupName             = group?.GroupName;
        GroupNameSpace        = group?.NameSpace;
        GroupShouldBeDisabled = group?.ShouldThisGroupGetDisabled;


        var collapsibleGroup = _field.GetCustomAttribute<SettingCollapsibleGroup>();
        CollapsibleGroupName = collapsibleGroup?.GroupName;

        Parent = _field.GetCustomAttribute<SettingParent>()?.ParentSettingFieldName;

        ShowSpace = _field.GetCustomAttribute<SettingUI_Space>() is not null
            ? true
            : null;
        ShowOr = _field.GetCustomAttribute<SettingUI_Or>() is not null
            ? true
            : null;
        ShowRetarget = _field.GetCustomAttribute<SettingUI_RetargetIcon>() is not null
            ? true
            : null;

        #endregion

        if (!CachedSettings.ContainsKey(settingName))
            CachedSettings[settingName] = this;
    }

    public object Value
    {
        set
        {
            var targetType = _field.FieldType;

            if (!targetType.IsInstanceOfType(value))
            {
                try
                {
                    value = Convert.ChangeType(value, targetType);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"Cannot convert value of type {value.GetType()} to " +
                        $"{targetType}.", ex);
                }
            }

            var typedValue = Convert.ChangeType(value, targetType);

            Service.Configuration.TriggerUserConfigChanged(
                Configuration.ConfigChangeType.Setting,
                Configuration.ConfigChangeSource.UI,
                FieldName, typedValue);

            ConfigurationValues.SetFoP(FieldName, typedValue);
            ConfigurationValues.Save();
        }
        get => ConfigurationValues.GetFoP(FieldName);
    }

    #region Required Attribute Fields

    public Category    Category;
    public string      CategoryName;
    public string      DefaultValue;
    public string      FieldName;
    public string      HelpMark;
    public string      Name;
    public string      RecommendedValue;
    public SettingType Type;
    public string?     UnitLabel;
    public string?     ExtraHelpMark;
    public string?     WarningMark;
    public string?     ExtraText;
    public float?      MinFLoat;
    public float?      MaxFloat;
    public int?        MinInt;
    public int?        MaxInt;

    #endregion

    #region Optional Attribute Fields

    public string?   GroupName;
    public string?   GroupNameSpace;
    public bool?     GroupShouldBeDisabled;
    public string?   CollapsibleGroupName;
    public string?   Parent;
    public bool?     ShowSpace;
    public bool?     ShowOr;
    public bool?     ShowRetarget;
    public string[]? StackStringsToExclude;

    #endregion

    #region References

    private readonly        FieldInfo                   _field;
    public static readonly Dictionary<string, Setting>  CachedSettings = [];

    private static Type ConfigurationType => typeof(Configuration);
    private static Configuration ConfigurationValues => Service.Configuration;

    #endregion
}