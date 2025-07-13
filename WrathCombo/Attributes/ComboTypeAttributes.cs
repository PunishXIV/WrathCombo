using System;

namespace WrathCombo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class SimpleCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class AdvancedCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class BasicCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class HealingCombo : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
internal class MitigationCombo : Attribute
{
}

internal enum ComboType
{
    Simple = 0,
    Advanced = 1,
    Basic = 3,
    
    Healing = 7,
    Mitigation = 8,
    
    Feature = 11,
    Option = 12,
}