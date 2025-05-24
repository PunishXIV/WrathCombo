using System;
using WrathCombo.Core;

namespace WrathCombo.Attributes;

/// <summary>
///     Flags a Preset as one that is Retargeted with
///     <see cref="ActionRetargeting"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class RetargetedAttribute : Attribute
{
}

/// <summary>
///     Flags a Preset as one that may be Retargeted with
///     <see cref="ActionRetargeting"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class PossiblyRetargetedAttribute : Attribute
{
}
