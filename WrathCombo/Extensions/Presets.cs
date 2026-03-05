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

    ///<summary> Retrieves the <see cref="JobInfoAttribute"/> for the preset if it exists.</summary>
    internal static JobInfoAttribute? GetComboAttribute(this Preset preset)
    {
        return PresetStorage.AllPresets[preset].JobInfo;
    }
}