using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WrathCombo.Services;

namespace WrathCombo.Core
{
#nullable disable
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
        public string GameVersion;
        public string RetailVersion;
    }

    public class VersionToRetail
    {
        [JsonProperty("retail_version")]
        public string RetailVersion;

        [JsonProperty("version_string")]
        public string VersionString;
    }

#nullable enable

    public static class OpCodeConfigHelper
    {
        public unsafe static void UpdateOpCodes()
        {
            try
            {
                var gameVer = Framework.Instance()->GameVersionString;
                if (Service.Configuration.OpCodes.GameVersion == gameVer)
                    return;

                var file = P.HTTPClient.GetStringAsync("https://cdn.jsdelivr.net/gh/karashiiro/FFXIVOpcodes@latest/opcodes.json").Result;
                var config = JsonConvert.DeserializeObject<List<FFXIVOPCodes>>(file);

                if (config == null)
                    return;

                Service.Configuration.OpCodesBackup = config;

                var versionEndpoint = P.HTTPClient.GetStringAsync("https://raw.githubusercontent.com/xivdev/opcodediff/refs/heads/main/automation/ffxiv_versions_global.json").Result;
                var versions = JsonConvert.DeserializeObject<List<VersionToRetail>>(versionEndpoint);

                if (versions?.TryGetFirst(x => x.VersionString == gameVer, out var ver) == true)
                {
                    var codes = config.First(x => x.Version == ver.RetailVersion);
                    Service.Configuration.OpCodes.GameVersion = gameVer;
                    Service.Configuration.OpCodes.RetailVersion = ver.RetailVersion;

                    Service.Configuration.Save();
                }
            }
            catch(Exception ex)
            {
                ex.Log("Issue updating OP Codes");
            }
        }
    }
}
