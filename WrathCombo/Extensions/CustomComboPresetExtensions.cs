#region

using System.Text;
using ECommons.ExcelServices;
using WrathCombo.Core;
using WrathCombo.Window.Functions;

#endregion

namespace WrathCombo.Extensions;

internal static partial class PresetExtensions
{
    public static Presets.PresetAttributes? Attributes(this Preset preset)
    {
        if (Presets.Attributes.TryGetValue(preset, out var atts))
            return atts;

        return null;
    }

    public static string Name(this Preset? preset) =>
        preset is null ? "" : preset.Value.Name();

    public static string Name(this Preset preset)
    {
        var attributes = preset.Attributes();

        if (attributes?.CustomComboInfo is null)
            return preset.ToString();

        return attributes.CustomComboInfo.Name;
    }

    public static string NameWithFullLineage
        (this Preset preset, Job? currentJob = null)
    {
        var attributes = preset.Attributes();

        if (attributes?.CustomComboInfo is null)
            return preset.ToString();

        var name = new StringBuilder(preset.Name());

        Preset? inspectingPreset = preset;
        while (inspectingPreset is not null)
        {
            var parent = Presets.Attributes[inspectingPreset.Value].Parent;
            if (parent is not null)
            {
                name.Insert(0, $"{parent.Name()} > ");
            }
            // Insert the job name at the beginning
            else
            {
                string lastPresetJob;
                var lastPresetInfo = inspectingPreset.Value.Attributes()
                    .CustomComboInfo;

                if (currentJob is not null && lastPresetInfo.Job == currentJob)
                    break;

                if (lastPresetInfo.Job == Job.ADV)
                {
                    lastPresetJob = "[Roles And Content] ";
                    if (PresetStorage.IsVariant(inspectingPreset.Value))
                        lastPresetJob += "Variant > ";
                    if (PresetStorage.IsOccultCrescent(inspectingPreset.Value))
                        lastPresetJob += "Occult Crescent > ";
                }
                else
                    lastPresetJob = $"[{lastPresetInfo.JobShorthand}] ";

                name.Insert(0, lastPresetJob);
            }

            inspectingPreset = parent;
        }

        return name.ToString();
    }
}