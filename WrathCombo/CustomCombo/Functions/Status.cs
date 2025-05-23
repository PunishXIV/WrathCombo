using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.Data;
using WrathCombo.Services;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary>
        /// Retrieves a Status object that is on the Player or specified Target, null if not found
        /// </summary>
        /// <param name="statusId">Status Effect ID</param>
        /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
        /// <param name="target">Optional target</param>
        /// <returns>Status object or null.</returns>
        public static Status? GetStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)
        {
            // Default to LocalPlayer if no target/bad target
            target ??= LocalPlayer;

            // Use LocalPlayer's GameObjectId if playerOwned, null otherwise
            ulong? sourceId = !anyOwner ? LocalPlayer.GameObjectId : null; 

            return Service.ComboCache.GetStatus(statusId, target, sourceId);
        }

        /// <summary>
        /// Checks to see if a status is on the Player or an optional target
        /// </summary>
        /// <param name="statusId">Status Effect ID</param>
        /// <param name="target">Optional Target</param>
        /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
        /// <returns>Boolean if the status effect exists or not</returns>
        public static bool HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)
        {
            // Default to LocalPlayer if no target provided
            target ??= LocalPlayer;
            return GetStatusEffect(statusId, target, anyOwner) is not null;
        }

        /// <summary>
        /// Checks to see if a status is on the Player or an optional target, and supplies the Status as well
        /// </summary>
        /// <param name="statusId">Status Effect ID</param>
        /// <param name="target">Optional Target</param>
        /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
        /// <param name="status">Retrieved Status object</param>
        /// <returns>Boolean if the status effect exists or not</returns>
        public static bool HasStatusEffect(ushort statusId, out Status? status, IGameObject? target = null, bool anyOwner = false)
        {
            target ??= LocalPlayer;
            status = GetStatusEffect(statusId, target, anyOwner);
            return status is not null;
        }

        /// <summary>
        /// Gets remaining time of a Status Effect
        /// </summary>
        /// <param name="effect">Dalamud Status object</param>
        /// <returns>Float representing remaining status effect time</returns>
        public unsafe static float GetStatusEffectRemainingTime(Status? effect)
        {
            if (effect is null) return 0;
            if (effect.RemainingTime < 0) return (effect.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
            return effect.RemainingTime;
        }

        /// <summary>
        /// Retrieves remaining time of a Status Effect on the Player or Optional Target
        /// </summary>
        /// <param name="effectId">Status Effect ID</param>
        /// <param name="target">Optional Target</param>
        /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
        /// <returns>Float representing remaining status effect time</returns>
        public unsafe static float GetStatusEffectRemainingTime(ushort effectId, IGameObject? target = null, bool anyOwner = false) => 
            GetStatusEffectRemainingTime(GetStatusEffect(effectId, target, anyOwner));

        /// <summary>
        /// Retrieves remaining time of a Status Effect
        /// </summary>
        /// <param name="effect">Dalamud Status object</param>
        /// <returns>Integer representing status effect stack count</returns>
        public static ushort GetStatusEffectStacks(Status? effect) => effect?.Param ?? 0;

        /// <summary>
        /// Retrieves the status effect stack count
        /// </summary>
        /// <param name="effectId">Status Effect ID</param>
        /// <param name="target">Optional Target</param>
        /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
        /// <returns>Integer representing status effect stack count</returns>
        public static ushort GetStatusEffectStacks(ushort effectId, IGameObject? target = null, bool anyOwner = false) =>
            GetStatusEffectStacks(GetStatusEffect(effectId, target, anyOwner));


        /// <summary> Returns the name of a status effect from its ID. </summary>
        /// <param name="id"> ID of the status. </param>
        /// <returns></returns>
        public static string GetStatusName(uint id) => StatusCache.GetStatusName(id);

        public static bool TargetHasDamageDown(IGameObject? target) => StatusCache.HasDamageDown(target);

        public static bool TargetHasRezWeakness(IGameObject? target, bool checkForWeakness = true)
        {
            if (checkForWeakness && HasStatusEffect(43, target, true)) //Weakness = 43
                return true;

            return HasStatusEffect(44, target, true); //Brink of Death = 44
        }

        /// <summary>
        /// Checks if the target has a debuff that can be dispelled.
        /// </summary>
        /// <param name="target">The game object to check. Defaults to the current target if null.</param>
        /// <returns>True if the target has a cleansable debuff; otherwise, false.</returns>
        public static bool HasCleansableDebuff(IGameObject? target) => StatusCache.HasCleansableDebuff(target);

        /// <summary>
        /// Checks if the target is invincible due to status effects or encounter-specific mechanics.
        /// </summary>
        /// <param name="target">The game object to check.</param>
        /// <returns>True if the target is invincible; otherwise, false.</returns>
        public static bool TargetIsInvincible(IGameObject? target) => StatusCache.TargetIsInvincible(target);
    }
}
