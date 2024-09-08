using System.Linq;
using ECommons.DalamudServices;

namespace XIVSlothCombo.Data;

public static class ConflictingPluginsCheck
{
    /// <summary>
    /// List of the most popular conflicting plugins.
    /// </summary>
    /// <remarks>
    /// The list is case-sensitive, and needs to be lowercase.
    /// </remarks>
    private static string[] conflictingPluginsNames = new string[]
    {
        "xivcombo",
        "xivcomboexpanded",
        "xivcomboexpandedest",
        "xivcombovx",
        "xivslothcombo",
    };

    /// <summary>
    /// Searches for any enabled conflicting plugins.
    /// </summary>
    /// <returns>
    /// <c>null</c> if no conflicts were found.<br/>
    /// <c>string[]</c> of conflicting plugins otherwise.<br/>
    /// </returns>
    /// <remarks>
    /// Each <c>string</c> would be <c>InternalName(Version)</c>
    /// </remarks>
    public static string[]? TryGetConflictingPlugins()
    {
        var conflictingPlugins = Svc.PluginInterface.InstalledPlugins
            .Where(x => conflictingPluginsNames.Contains(x.InternalName.ToLower()) && x.IsLoaded)
            .Select(x => $"{x.InternalName}({x.Version})")
            .ToArray();

        return conflictingPlugins.Length == 0 ? null : conflictingPlugins;
    }
}
