using System.Collections.Generic;
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

            List<string> listOfNamesToCheck = [
                "XIVSlothCombo",
                "WrathCombo",
            ];

            // Iterate over each name in the list
            foreach (string name in listOfNamesToCheck)
            {
                // Check if a manifest of this name exists
                var manifest = Path.Join(f.DirectoryName, Svc.PluginInterface.InternalName + ".json");
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

            // Default to the loaded manifest, whatever it was called
            var defaultRepoCheck = new RepoCheck
            {
                InstalledFromUrl = Svc.PluginInterface.Manifest.RepoUrl
            };
            return defaultRepoCheck;

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
