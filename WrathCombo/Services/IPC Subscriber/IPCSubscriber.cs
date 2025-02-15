using ECommons;
using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Common.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace WrathCombo.Services.IPC_Subscriber
{
    internal static class OrbwalkerIPC
    {
        private static EzIPCDisposalToken[] _disposalTokens = EzIPC.Init(typeof(OrbwalkerIPC), "Orbwalker", SafeWrapper.IPCException);

        internal static bool IsEnabled => DalamudReflector.TryGetDalamudPlugin("Orbwalker", out _, false, true);
        internal static Version? Version => DalamudReflector.TryGetDalamudPlugin("Orbwalker", out var dalamudPlugin, false, true) ? dalamudPlugin.GetType().Assembly.GetName().Version : new Version(0, 0, 0, 0);

        [EzIPC] public static readonly Func<bool> PluginEnabled;
        [EzIPC] public static readonly Func<bool> MovementLocked;
        [EzIPC] public static readonly Func<bool> IsSlideWindowAuto; //True for Automatic, False for Manual
        [EzIPC] public static readonly Func<bool> OrbwalkingMode; //True for Slidecast, False for Slidelock
        [EzIPC] public static readonly Func<bool> BufferEnabled;
        [EzIPC] public static readonly Func<bool> ControllerModeEnabled;
        [EzIPC] public static readonly Func<bool> MouseButtonReleaseEnabled;
        [EzIPC] public static readonly Func<bool> PvPEnabled;
        [EzIPC] public static readonly Func<List<uint>> EnabledJobs;


        [EzIPC] public static Action<bool> SetPluginEnabled;
        [EzIPC] public static Action<bool> SetSlideAuto;
        [EzIPC] public static Action<bool> SetOrbwalkingMode;
        [EzIPC] public static Action<bool> SetBuffer;
        [EzIPC] public static Action<bool> SetControllerMode;
        [EzIPC] public static Action<bool> SetMouseButtonRelease;
        [EzIPC] public static Action<bool> SetPvP;
        [EzIPC] public static Action<uint, bool> SetEnabledJob;

        public static bool CanOrbwalk => IsEnabled && PluginEnabled() && !MouseMoving && EnabledJobs().Any(x => x == (uint)Player.Job);

        private static unsafe InputManager.MouseButtonHoldState* _state => InputManager.GetMouseButtonHoldState() + 1;
        private static unsafe bool BothMousebuttonsHeld => _state->HasFlag(InputManager.MouseButtonHoldState.Left) && _state->HasFlag(InputManager.MouseButtonHoldState.Right);
        
        public static bool MouseMoving => MouseButtonReleaseEnabled() && BothMousebuttonsHeld;

        internal static void Dispose()
        {
            foreach (var token in _disposalTokens)
            {
                try
                {
                    token.Dispose();
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
        }
    }
}
