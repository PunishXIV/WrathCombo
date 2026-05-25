using Dalamud.Plugin.Ipc;
using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using ECommons.Reflection;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace WrathCombo.Services.IPC_Subscriber
{
    // Requires the use of 1PP "PingPlugin" because I really can't be bothered implementing all the same checks myself
    // Maybe that can be an ECommons thing at some point instead?
    internal static class PingPluginIPC
    {

        /// <summary>
        /// Value of -1 indicates either PingPlugin is not enabled or has yet to return a ping value as it's throttled. Use <see cref="CanGetPing"/> to check if plugin is enabled.
        /// </summary>
        public static int LastPing = -1;

        /// <summary>
        /// Value of -1 indicates either PingPlugin is not enabled or has yet to return a ping value as it's throttled. Use <see cref="CanGetPing"/> to check if plugin is enabled.
        /// </summary>
        public static int AveragePing = -1;

        public static ICallGateSubscriber<object, object>? IPCSubscriber;
        public static void Init()
        {
            IPCSubscriber = Svc.PluginInterface.GetIpcSubscriber<object, object>("PingPlugin.Ipc");
            IPCSubscriber?.Subscribe(GetPing);
        }

        /// <summary>
        /// Gets the message from the IPCSubscriber and sets the ping.
        /// </summary>
        /// <param name="obj">JObject representing the message payload</param>
        private static void GetPing(object obj)
        {
            var j = obj as JObject;
            LastPing = j.Value<int>("LastRTT");
            AveragePing = j.Value<int>("AverageRTT");

            Svc.Log.Verbose($"[PingPluginIPC] Last Ping: {LastPing}, Average Ping: {AveragePing}");
        }

        public static void Dispose()
        {
            IPCSubscriber?.Unsubscribe(GetPing);
        }

        /// <summary>
        /// Checks if PingPlugin is enabled. If it is not, all values reset to default.
        /// </summary>
        public static bool CanGetPing
        {
            get
            {
                bool ret = DalamudReflector.TryGetDalamudPlugin("PingPlugin", out _, false, true);
                if (!ret)
                {
                    LastPing = -1;
                    AveragePing = -1;
                }
                return ret;
            }
        }
    }
}
