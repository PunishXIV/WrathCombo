using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WrathCombo.Services;

namespace WrathCombo.Core
{
    public class ClientLobbyIpcType
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("opcode")]
        public int Opcode;
    }

    public class ClientZoneIpcType
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("opcode")]
        public int Opcode;
    }

    public class Lists
    {
        [JsonProperty("ServerZoneIpcType")]
        public List<ServerZoneIpcType> ServerZoneIpcType;

        [JsonProperty("ClientZoneIpcType")]
        public List<ClientZoneIpcType> ClientZoneIpcType;

        [JsonProperty("ServerLobbyIpcType")]
        public List<ServerLobbyIpcType> ServerLobbyIpcType;

        [JsonProperty("ClientLobbyIpcType")]
        public List<ClientLobbyIpcType> ClientLobbyIpcType;

        [JsonProperty("ServerChatIpcType")]
        public List<object> ServerChatIpcType;

        [JsonProperty("ClientChatIpcType")]
        public List<object> ClientChatIpcType;
    }

    public class FFXIVOPCodes
    {
        [JsonProperty("version")]
        public string Version;

        [JsonProperty("region")]
        public string Region;

        [JsonProperty("lists")]
        public Lists Lists;
    }

    public class ServerLobbyIpcType
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("opcode")]
        public int Opcode;
    }

    public class ServerZoneIpcType
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("opcode")]
        public int Opcode;
    }

    public class OpCodeConfig
    {
        public string Version;
        public int UpdateHpMpTp;
        public int EffectResultBasic;
    }

    public class VersionToRetail
    {
        [JsonProperty("retail_version")]
        public string RetailVersion;

        [JsonProperty("version_string")]
        public string VersionString;
    }

    public static class OpCodeConfigHelper
    {
        public unsafe static void UpdateOpCodes()
        {
            var file = P.HTTPClient.GetStringAsync("https://cdn.jsdelivr.net/gh/karashiiro/FFXIVOpcodes@latest/opcodes.json").Result;
            var config = JsonConvert.DeserializeObject<List<FFXIVOPCodes>>(file);

            if (config == null)
                return;

            var versionEndpoint = P.HTTPClient.GetStringAsync("https://raw.githubusercontent.com/xivdev/opcodediff/refs/heads/main/automation/ffxiv_versions_global.json").Result;
            var versions = JsonConvert.DeserializeObject<List<VersionToRetail>>(versionEndpoint);
            var gameVer = Framework.Instance()->GameVersionString;

            if (versions?.TryGetFirst(x => x.VersionString == gameVer, out var ver) == true)
            {
                var codes = config.First(x => x.Version == ver.RetailVersion);
                Service.Configuration.OpCodes.Version = gameVer;
                Service.Configuration.OpCodes.UpdateHpMpTp = codes.Lists.ServerZoneIpcType.First(x => x.Name == "UpdateHpMpTp").Opcode;
                Service.Configuration.OpCodes.EffectResultBasic = codes.Lists.ServerZoneIpcType.First(x => x.Name == "EffectResultBasic").Opcode;

                Svc.Log.Debug($"[OpCodeConfig] {Service.Configuration.OpCodes.EffectResultBasic} {Service.Configuration.OpCodes.UpdateHpMpTp}");
                Service.Configuration.Save();
            }
        }
    }
}
