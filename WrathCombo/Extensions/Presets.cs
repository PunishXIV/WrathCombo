using WrathCombo.Attributes;
using WrathCombo.Core;
namespace WrathCombo.Extensions;

internal static partial class PresetExtensions
{
    ///<summary> Retrieves the <see cref="ReplaceSkillAttribute"/> for the preset if it exists.</summary>
    internal static ReplaceSkillAttribute? GetReplaceAttribute(this Preset preset)
    {
        return PresetStorage.AllPresets[preset].ReplaceSkill;
    }

    ///<summary> Retrieves the <see cref="CustomComboInfoAttribute"/> for the preset if it exists.</summary>
    internal static CustomComboInfoAttribute? GetComboAttribute(this Preset preset)
    {
        return PresetStorage.AllPresets[preset].CustomComboInfo;
    }

    ///<summary> Retrieves the <see cref="HoverInfoAttribute"/> for the preset if it exists.</summary>
    internal static HoverInfoAttribute? GetHoverAttribute(this Preset preset)
    {
        return PresetStorage.AllPresets[preset].HoverInfo;
    }
}