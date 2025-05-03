using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Data.Parsing.Layer;
using System;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        private static DateTime? movementStarted;
        private static DateTime? movementStopped;

        /// <summary> Checks if the player is moving. </summary>
        public static unsafe bool IsMoving()
        {
            var agentMap = AgentMap.Instance();
            if (agentMap is null)
                return false;

            bool isMoving = agentMap->IsPlayerMoving;

            if (isMoving)
            {
                if (movementStarted is null)
                    movementStarted = DateTime.Now;

                movementStopped = null;
            }
            else
            {
                if (movementStopped is null)
                    movementStopped = DateTime.Now;

                movementStarted = null;
            }

            return isMoving && TimeMoving.TotalSeconds >= Service.Configuration.MovementLeeway;
        }

        public static TimeSpan TimeMoving => movementStarted is null ? TimeSpan.Zero : (DateTime.Now - movementStarted.Value);

        public static TimeSpan TimeStoodStill => movementStopped is null ? TimeSpan.Zero : (DateTime.Now - movementStopped.Value);
    }
}
