using System;
using System.Collections.Generic;
using System.Text;

namespace WrathCombo.Extensions
{
    public abstract class StringEnums
    {
        public string Value { get; }

        protected StringEnums(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}
