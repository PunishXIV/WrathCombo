using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Window
{
    internal static class Icons
    {
        public static Dictionary<uint, IDalamudTextureWrap> CachedModdedIcons = new();
        public static Dictionary<int, IDalamudTextureWrap?> OccultIcons = [];
        private static int OccultIdx = -1; // Instead of 0 to show Freelancer
        public static IDalamudTextureWrap? GetJobIcon(uint jobId)
        {
            switch (jobId)
            {
                case All.JobID: jobId = 62146; break; //Adventurer / General
                case > All.JobID and <= 42: jobId += 62100; break; //Classes
                case DOL.JobID: jobId = 82096; break;
                case OccultCrescent.JobID: return GetOccultIcon();
                default: return null; //Unknown, return null
            }
            return GetTextureFromIconId(jobId);
        }

        private static IDalamudTextureWrap? GetOccultIcon()
        {
            if (OccultIcons.Count < 26)
            {
                for (int i = 0; i <= 25; i++)
                {
                    var uld = Svc.PluginInterface.UiBuilder.LoadUld("ui/uld/MKDSupportJob.uld");
                    OccultIcons[i] = uld.LoadTexturePart("ui/uld/MKDSupportJob_hr1.tex", i);
                }
                for (int i = 30; i <= 55; i++)
                {
                    var uld = Svc.PluginInterface.UiBuilder.LoadUld("ui/uld/MKDSupportJob.uld");
                    OccultIcons[i] = uld.LoadTexturePart("ui/uld/MKDSupportJob_hr1.tex", i);
                }
            }

            if (EzThrottler.Throttle("OccultAnimateIdx", 800))
                OccultIdx++;

            if (OccultIdx == 13) // Only cycle through the current ones, set to 26 after new ones added
                OccultIdx = 0;

            return OccultIcons[OccultIdx];
        }

        private static string ResolvePath(string path) => Svc.TextureSubstitution.GetSubstitutedPath(path);

        public static IDalamudTextureWrap? GetTextureFromIconId(uint iconId, uint stackCount = 0, bool hdIcon = true)
        {
            GameIconLookup lookup = new(iconId + stackCount, false, hdIcon);
            string path = Svc.Texture.GetIconPath(lookup);
            string resolvePath = ResolvePath(path);

            var wrap = Svc.Texture.GetFromFile(resolvePath);
            if (wrap.TryGetWrap(out var icon, out _))
                return icon;

            try
            {
                if (CachedModdedIcons.TryGetValue(iconId, out IDalamudTextureWrap? cachedIcon)) return cachedIcon;
                var tex = Svc.Data.GameData.GetFileFromDisk<TexFile>(resolvePath);
                var output = Svc.Texture.CreateFromRaw(RawImageSpecification.Rgba32(tex.Header.Width, tex.Header.Width), tex.GetRgbaImageData());
                if (output != null)
                {
                    CachedModdedIcons[iconId] = output;
                    return output;
                }
            }
            catch { }


            return Svc.Texture.GetFromGame(path).GetWrapOrDefault();
        }
    }
}
