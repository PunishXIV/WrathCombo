#region

using ECommons.ExcelServices;
using System.Collections.Generic;
using System.Text;
using WrathCombo.Core;
using WrathCombo.Resources.Localization.UI.Misc;

#endregion

namespace WrathCombo.Extensions;

internal static partial class PresetExtensions
{
    public static PresetStorage.PresetData Attributes(this Preset preset)
    {

        return PresetStorage.AllPresets[preset];
    }

    public static string Name(this Preset? preset) =>
        preset is null ? "" : preset.Value.Name();

    extension(Preset preset)
    {
        public string Name()
        {
            return PresetStorage.AllPresets.TryGetValue(preset, out var attr)
                ? attr.Name
                : preset.ToString();
        }

        public string NameWithFullLineage(Job? currentJob = null)
        {
            var segments = new List<string>
            {
                // Start with the current preset name
                preset.Name()
            };

            var current = preset;

            while (true)
            {
                var attr = current.Attributes();
                var parent = attr.Parent;

                if (parent is not null)
                {
                    segments.Add(parent.Value.Name());
                    current = parent.Value;
                    continue;
                }

                // We hit root level — now add job header
                if (currentJob is not null && attr.JobInfo.Job == currentJob)
                    break;

                if (attr.JobInfo.Job == Job.ADV)
                {
                    var header = MiscUI.Roles_and_Content;

                    if (attr.IsVariant)
                        header += $" {MiscUI.Variant}";

                    else if (attr.IsBozja)
                        header += $" {MiscUI.Bozja}";

                    else if (attr.OccultCrescentJob is not null)
                        header += $" {MiscUI.Occult_Crescent}";

                    else header += $" {MiscUI.Job_Roles}";

                    segments.Add(header);
                }
                else
                {
                    segments.Add($"[{attr.JobInfo.JobShorthand}]");
                }

                break;
            }

            segments.Reverse();
            return string.Join(" > ", segments);
        }

        public bool Enabled() =>
            PresetStorage.IsEnabled(preset);

        public bool FullLineageEnabled()
        {
            Preset? inspectingPreset = preset;
            while (inspectingPreset is not null)
            {
                if (!PresetStorage.IsEnabled(inspectingPreset.Value))
                    return false;

                var parent = PresetStorage.AllPresets[inspectingPreset.Value].Parent;
                inspectingPreset = parent;
            }

            return true;
        }
    }
}