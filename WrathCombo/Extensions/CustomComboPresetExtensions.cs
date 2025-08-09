using WrathCombo.Combos;
using WrathCombo.Window.Functions;
namespace WrathCombo.Extensions;

internal static class CustomComboPresetExtensions
{
    public static Presets.PresetAttributes? Attributes(this CustomComboPreset preset)
    {
        if (Presets.Attributes.TryGetValue(preset, out var atts))
            return atts;
        return null;
    } 

}