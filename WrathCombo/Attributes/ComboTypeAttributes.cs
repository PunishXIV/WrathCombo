using System;

namespace WrathCombo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class SimpleDPSCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class AdvancedDPSCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class BasicCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class SimpleHealingCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class AdvancedHealingCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class MitigationCombo : Attribute
{
}

internal enum ComboType
{
    SimpleDPS = 0,
    AdvancedDPS = 1,
    Basic = 3,

    SimpleHealing = 6,
    AdvancedHealing = 7,
    Mitigation = 8,

    Feature = 11,
    Option = 12,
}