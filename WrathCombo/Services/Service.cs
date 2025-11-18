using WrathCombo.Core;
using WrathCombo.Data;
namespace WrathCombo.Services;

/// <summary> Dalamud and plugin services. </summary>
internal class Service
{
    /// <summary> Gets or sets the plugin address resolver. </summary>
    internal static AddressResolver Address { get; set; } = null!;

    /// <summary> Gets or sets the plugin caching mechanism. </summary>
    internal static CustomComboCache ComboCache { get; set; } = null!;

    /// <summary> Gets or sets the plugin configuration. </summary>
    internal static Configuration Configuration { get; set; } = null!;

    /// <summary> Gets or sets the plugin icon replacer. </summary>
    internal static ActionReplacer ActionReplacer { get; set; } = null!;
}