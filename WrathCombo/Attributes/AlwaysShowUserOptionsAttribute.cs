using System;

namespace WrathCombo.Attributes
{
    /// <summary> Attribute forcing User Options to always show, regardless of preset being enabled </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class AlwaysShowUserOptsAttribute : Attribute
    {
    }
}
