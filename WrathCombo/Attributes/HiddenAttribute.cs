using System;

namespace WrathCombo.Attributes;

/// <summary> Attribute documenting combos that are not shown by default. </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class HiddenAttribute : Attribute
{
}