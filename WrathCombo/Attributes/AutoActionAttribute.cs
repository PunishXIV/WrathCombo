using System;

namespace WrathCombo.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class AutoActionAttribute : Attribute
    {
        public bool IsAoE;
        public bool IsHeal;
        internal AutoActionAttribute(bool isAoE, bool isHeal) 
        { 
            IsAoE = isAoE;
            IsHeal = isHeal;
        }
    }
}
