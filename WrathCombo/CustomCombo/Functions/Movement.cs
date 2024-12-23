using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        private static DateTime? movementStarted;

        /// <summary> Checks player movement </summary>
        public static unsafe bool IsMoving()
        {
            bool isMoving = AgentMap.Instance() is not null && AgentMap.Instance()->IsPlayerMoving > 0;

            if (isMoving && movementStarted is null)
                movementStarted = DateTime.Now;

            if (!isMoving)
                movementStarted = null;

            return isMoving && (TimeMoving.TotalMilliseconds / 1000f) >= Service.Configuration.MovementLeeway;
        }

        public static TimeSpan TimeMoving => movementStarted is null ? TimeSpan.Zero : (DateTime.Now - movementStarted.Value);
    }
}
