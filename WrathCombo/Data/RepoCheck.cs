using ECommons.DalamudServices;
using Newtonsoft.Json;
using System.IO;

namespace WrathCombo.Data
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
            var manifest = Path.Join(f.DirectoryName, "WrathCombo.json");

            // Load the manifest
            try
            {
                RepoCheck? repo = JsonConvert.DeserializeObject<RepoCheck>(File.ReadAllText(manifest));

                // Check if we were able to read the manifest and its repo URL
                return repo?.InstalledFromUrl is null ? null : repo;
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public static bool IsFromPunishRepo()
        {
            RepoCheck? repo = FetchCurrentRepo();
            if (repo is null) return false;

            if (repo.InstalledFromUrl is null) return false;

            if (repo.InstalledFromUrl == "https://love.puni.sh/ment.json")
                return true;
            else
                return false;
        }
    }
}
