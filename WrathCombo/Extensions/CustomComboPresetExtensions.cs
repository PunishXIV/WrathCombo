using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrathCombo.Combos;
using WrathCombo.Window.Functions;

namespace WrathCombo.Extensions
{
    internal static class CustomComboPresetExtensions
    {
        public static Presets.PresetAttributes Attributes(this CustomComboPreset preset)
        {
            return Presets.Attributes[preset];
        } 

    }
}
