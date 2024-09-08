using System.Linq;
using ECommons.DalamudServices;

namespace XIVSlothCombo.Data;

public static class ConflictingPluginsCheck
{
    private static string[] conflictingPluginsNames = new string[]
    {
        "xivcombo",
        "xivcomboexpanded",
        "xivcomboexpandedest",
        "xivcombovx",
    };

    public static string[]? TryGetConflictingPlugins()
    {
        var conflictingPlugins = Svc.PluginInterface.InstalledPlugins
            .Where(x => conflictingPluginsNames.Contains(x.InternalName.ToLower()))
            .Select(x => $"{x.InternalName}({x.Version})")
            .ToArray();

        return conflictingPlugins.Length == 0 ? null : conflictingPlugins;
    }
}
