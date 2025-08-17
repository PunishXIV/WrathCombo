using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Data.Files;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Attributes;
namespace WrathCombo.Window;

internal static class Icons
{
    public static Dictionary<uint, IDalamudTextureWrap> CachedModdedIcons = new();
    
    public static class Occult
    {
        private const int MaxJobId = 12; // Change later when new Occult classes have sprites
        private const int AnimationOffset = 30;

        public static readonly Lazy<FrozenDictionary<int, IDalamudTextureWrap?[]>> JobSprites =
            new(() => LoadOccultJobIcons());

        private static FrozenDictionary<int, IDalamudTextureWrap?[]> LoadOccultJobIcons()
        {
            var dict = new Dictionary<int, IDalamudTextureWrap?[]>();

            var uld = Svc.PluginInterface.UiBuilder.LoadUld("ui/uld/MKDSupportJob.uld");

            for (int jobId = 0; jobId <= MaxJobId; jobId++)
            {
                var frames = new IDalamudTextureWrap?[2];
                frames[0] = uld.LoadTexturePart("ui/uld/MKDSupportJob_hr1.tex", jobId);
                frames[1] = uld.LoadTexturePart("ui/uld/MKDSupportJob_hr1.tex", jobId + AnimationOffset);

                dict[jobId] = frames;
            }

            return dict.ToFrozenDictionary();
        }
    }

    public static class Role
    {
        private const uint RoleBaseIconID = 62580;

        public static IDalamudTextureWrap? GetRoleIcon(JobRole jobRole)
        {
            uint? iconID = jobRole switch
            {
                JobRole.Tank => RoleBaseIconID + 1,
                JobRole.Healer => RoleBaseIconID + 2,
                JobRole.MeleeDPS => RoleBaseIconID + 4,
                JobRole.RangedDPS => RoleBaseIconID + 6,
                JobRole.MagicalDPS => RoleBaseIconID + 7,
                _ => null
            };
            return iconID is null ? null : GetTextureFromIconId(iconID.Value);
        }
    }

    public static IDalamudTextureWrap? GetJobIcon(Job job)
    {
        uint iconID = job switch
        {
            Job.ADV => 62146,
            Job.MIN or Job.BTN or Job.FSH => 82096,
            _ => (uint)ExcelJobHelper.GetIcon(job)
        };

        return GetTextureFromIconId(iconID);
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