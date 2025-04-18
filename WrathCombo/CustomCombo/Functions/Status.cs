using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE;
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
        public static string GetStatusName(uint id) => ActionWatching.GetStatusName(id);

        /// <summary> Checks if the character has the Silence status. </summary>
        /// <returns></returns>
        public static bool HasSilence()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(7)))
            {
                if (HasStatusEffect((ushort)status, anyOwner: true)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Pacification status. </summary>
        /// <returns></returns>
        public static bool HasPacification()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(6)))
            {
                if (HasStatusEffect((ushort)status, anyOwner: true)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Amnesia status. </summary>
        /// <returns></returns>
        public static bool HasAmnesia()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(5)))
            {
                if (HasStatusEffect((ushort)status, anyOwner: true)) return true;
            }

            return false;
        }

        public static bool TargetHasDamageDown(IGameObject? target)
        {
            foreach (var status in ActionWatching.GetStatusesByName(GetStatusName(62)))
            {
                if (HasStatusEffect((ushort)status, target, true)) return true;
            }

            return false;
        }

        public static bool TargetHasRezWeakness(IGameObject? target, bool checkForWeakness = true)
        {
            if (checkForWeakness)
                foreach (var status in ActionWatching.GetStatusesByName(
                             GetStatusName(All.Debuffs.Weakness)))
                    if (HasStatusEffect((ushort)status, target, true)) return true;

            foreach (var status in ActionWatching.GetStatusesByName(
                         GetStatusName(All.Debuffs.BrinkOfDeath)))
                if (HasStatusEffect((ushort)status, target, true)) return true;

            return false;
        }

        public static bool HasCleansableDebuff(IGameObject? target = null)
        {
            target ??= CurrentTarget;
            if (target is null) return false;
            if ((target is not IBattleChara chara)) return false;

            try
            {
                if (chara.StatusList is null || chara.StatusList.Length == 0) return false;

                foreach (var status in chara.StatusList.Where(x => x is not null && x.StatusId > 0))
                    if (ActionWatching.StatusSheet.TryGetValue(status.StatusId,
                            out var statusItem) && statusItem.CanDispel)
                        return true;
            }
            catch (Exception ex) // Accessing invalid status lists
            {
                ex.Log();
                return false;
            }

            return false;
        }

        public static bool NoBlockingStatuses(uint actionId)
        {
            switch (ActionWatching.GetAttackType(actionId))
            {
                case ActionWatching.ActionAttackType.Weaponskill:
                    if (HasPacification()) return false;
                    return true;
                case ActionWatching.ActionAttackType.Spell:
                    if (HasSilence()) return false;
                    return true;
                case ActionWatching.ActionAttackType.Ability:
                    if (HasAmnesia()) return false;
                    return true;

            }

            return true;
        }

        private static List<uint> InvincibleStatuses = new()
        {
            151,
            198,
            325,
            328,
            385,
            394,
            469,
            529,
            592,
            656,
            671,
            775,
            776,
            895,
            969,
            981,
            1240,
            1302,
            1303,
            1567,
            1570,
            1697,
            1829,
            1936,
            2413,
            2654,
            3012,
            3039,
            3052,
            3054,
            4410,
            4175
        };

        public static bool TargetIsInvincible(IGameObject target)
        {
            var tar = (target as IBattleChara);
            bool invinceStatus = tar.StatusList.Any(y => InvincibleStatuses.Any(x => x == y.StatusId));
            if (invinceStatus)
                return true;

            //Jeuno Ark Angel Encounter
            if ((HasStatusEffect(4192) && !tar.StatusList.Any(x => x.StatusId == 4193)) ||
                (HasStatusEffect(4194) && !tar.StatusList.Any(x => x.StatusId == 4195)) ||
                (HasStatusEffect(4196) && !tar.StatusList.Any(x => x.StatusId == 4197)))
                return true;

            // Yorha raid encounter
            if ((GetAllianceGroup() != AllianceGroup.GroupA && tar.StatusList.Any(x => x.StatusId == 2409)) ||
                (GetAllianceGroup() != AllianceGroup.GroupB && tar.StatusList.Any(x => x.StatusId == 2410)) ||
                (GetAllianceGroup() != AllianceGroup.GroupC && tar.StatusList.Any(x => x.StatusId == 2411)))
                return true;

            // Omega
            if ((tar.StatusList.Any(x => x.StatusId == 1674 || x.StatusId == 3454) && (HasStatusEffect(1660) || HasStatusEffect(3499))) ||
                (tar.StatusList.Any(x => x.StatusId == 1675) && (HasStatusEffect(1661) || HasStatusEffect(3500))))
                return true;


            return false;
        }
    }
}
