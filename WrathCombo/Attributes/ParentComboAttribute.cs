using System;
using WrathCombo.Combos;

namespace WrathCombo.Attributes;

/// <summary> Attribute documenting required combo relationships. </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class ParentComboAttribute : Attribute
{
    /// <summary> Initializes a new instance of the <see cref="ParentComboAttribute"/> class. </summary>
    /// <param name="parentPreset"> Presets that conflict with the given combo. </param>
    internal ParentComboAttribute(Preset parentPreset) => ParentPreset = parentPreset;

    /// <summary> Gets the display name. </summary>
    public Preset ParentPreset { get; }
}