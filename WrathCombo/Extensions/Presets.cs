using Dalamud.Utility;
using System.Collections.Generic;
using WrathCombo.Attributes;
using WrathCombo.Combos;
namespace WrathCombo.Extensions;

internal static partial class PresetExtensions
{
    internal static Dictionary<Preset, ReplaceSkillAttribute> replaceSkillCache = [];
    internal static Dictionary<Preset, CustomComboInfoAttribute> comboInfoCache = [];
    internal static Dictionary<Preset, HoverInfoAttribute> hoverInfoCache = [];

    ///<summary> Retrieves the <see cref="ReplaceSkillAttribute"/> for the preset if it exists.</summary>
    internal static ReplaceSkillAttribute? GetReplaceAttribute(this Preset preset)
    {
        if (replaceSkillCache.TryGetValue(preset, out var replaceSkillAttribute))
        {
            return replaceSkillAttribute;
        }

        ReplaceSkillAttribute? att = preset.GetAttribute<ReplaceSkillAttribute>();
        return att != null && replaceSkillCache.TryAdd(preset, att) ? replaceSkillCache[preset] : null;
    }

    ///<summary> Retrieves the <see cref="CustomComboInfoAttribute"/> for the preset if it exists.</summary>
    internal static CustomComboInfoAttribute? GetComboAttribute(this Preset preset)
    {
        if (comboInfoCache.TryGetValue(preset, out var customComboInfoAttribute))
        {
            return customComboInfoAttribute;
        }

        CustomComboInfoAttribute? att = preset.GetAttribute<CustomComboInfoAttribute>();
        return att != null && comboInfoCache.TryAdd(preset, att) ? comboInfoCache[preset] : null;

    }

    ///<summary> Retrieves the <see cref="HoverInfoAttribute"/> for the preset if it exists.</summary>
    internal static HoverInfoAttribute? GetHoverAttribute(this Preset preset)
    {
        if (hoverInfoCache.TryGetValue(preset, out var hoverInfoAttribute))
        {
            return hoverInfoAttribute;
        }

        HoverInfoAttribute? att = preset.GetAttribute<HoverInfoAttribute>();
        return att != null && hoverInfoCache.TryAdd(preset, att) ? hoverInfoCache[preset] : null;

    }
}