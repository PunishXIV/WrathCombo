using WrathCombo.Combos;
using WrathCombo.Window.Functions;
namespace WrathCombo.Extensions;

internal static partial class PresetExtensions
{
    public static Presets.PresetAttributes? Attributes(this Preset preset)
    {
        if (Presets.Attributes.TryGetValue(preset, out var atts))
            return atts;
        
        return null;
    } 

}