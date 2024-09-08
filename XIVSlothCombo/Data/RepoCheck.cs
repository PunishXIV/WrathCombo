using ECommons.DalamudServices;
using Newtonsoft.Json;
using System.IO;

namespace XIVSlothCombo.Data
{
    public class RepoCheck
    {
        public string? InstalledFromUrl { get; set; }
    }

    public static class RepoCheckFunctions
    {
        public static RepoCheck? FetchCurrentRepo()
        {
            FileInfo? f = Svc.PluginInterface.AssemblyLocation;

            // Flag as self-built if in dev mode
            if (Svc.PluginInterface.IsDev)
            {
                return new RepoCheck
                {
                    InstalledFromUrl = "!! Self-Built !!"
                };
            }

            string[] listOfNamesToCheck = [
                Svc.PluginInterface.InternalName,
                "XIVSlothCombo",
                "WrathCombo",
            ];

            // Iterate over each name in the list
            foreach (var name in listOfNamesToCheck)
            {
                // Check if a manifest of this name exists
                var manifest = Path.Join(f.DirectoryName, name + ".json");
                if (!File.Exists(manifest)) continue;

                // Load the manifest
                try
                {
                    RepoCheck? repo = JsonConvert.DeserializeObject<RepoCheck>(File.ReadAllText(manifest));

                    // Check if we were able to read the manifest and its repo URL
                    if (repo?.InstalledFromUrl is null) continue;

                    return repo;
                }
                catch { }
            }

            return null;

        }

        public static bool IsFromSlothRepo()
        {
            RepoCheck? repo = FetchCurrentRepo();

            if (repo?.InstalledFromUrl is null) return false;

            if (repo.InstalledFromUrl == "https://raw.githubusercontent.com/Nik-Potokar/MyDalamudPlugins/main/pluginmaster.json")
                return true;

            return false;
        }
    }
}
