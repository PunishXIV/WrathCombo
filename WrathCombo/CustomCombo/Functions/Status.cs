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
        /// <summary> Find if an effect on the player exists. The effect may be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool HasEffect(ushort effectID) => FindEffect(effectID) is not null;

        public static ushort GetBuffStacks(ushort effectId)
        {
            Status? eff = FindEffect(effectId);
            return eff?.Param ?? 0;
        }

        /// <summary> Gets the duration of a status effect on the player. By default, the effect must be owned by the player or unowned. </summary>
        /// <param name="effectId"> Status effect ID. </param>
        /// <param name="isPlayerOwned"> Whether the status effect must be owned by the player or can be owned by anyone. </param>
        /// <returns> The duration of the status effect. </returns>
        public unsafe static float GetBuffRemainingTime(ushort effectId, bool isPlayerOwned = true)
        {
            Status? eff = (isPlayerOwned == true)
                ? FindEffect(effectId)
                : FindEffectAny(effectId);

            if (eff is null) return 0;
            if (eff.RemainingTime < 0) return (eff.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
            return eff.RemainingTime;
        }

        /// <summary> Finds an effect on the player. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffect(ushort effectID) => FindEffect(effectID, LocalPlayer, LocalPlayer?.GameObjectId);

        /// <summary> Find if an effect on the target exists. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool TargetHasEffect(ushort effectID) => FindTargetEffect(effectID) is not null;

        /// <summary> Finds an effect on the current target. The effect must be owned by the player or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindTargetEffect(ushort effectID) => FindEffect(effectID, CurrentTarget, LocalPlayer?.GameObjectId);

        /// <summary> Gets the duration of a status effect on the current target. By default, the effect must be owned by the player or unowned. </summary>
        /// <param name="effectId"> Status effect ID. </param>
        /// <param name="isPlayerOwned"> Whether the status effect must be owned by the player or can be owned by anyone. </param>
        /// <returns> The duration of the status effect. </returns>
        public static unsafe float GetDebuffRemainingTime(ushort effectId, bool isPlayerOwned = true)
        {
            Status? eff = (isPlayerOwned == true)
                ? FindTargetEffect(effectId)
                : FindTargetEffectAny(effectId);

            if (eff is null) return 0;
            if (eff.RemainingTime < 0) return (eff.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
            return eff.RemainingTime;
        }

        /// <summary> Find if an effect on the player exists. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool HasEffectAny(ushort effectID) => FindEffectAny(effectID) is not null;

        /// <summary> Finds an effect on the player. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffectAny(ushort effectID) => FindEffect(effectID, LocalPlayer, null);

        /// <summary> Find if an effect on the target exists. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> A value indicating if the effect exists. </returns>
        public static bool TargetHasEffectAny(ushort effectID) => FindTargetEffectAny(effectID) is not null;

        public static bool TargetHasEffectAny(ushort effectID, IGameObject target) => FindTargetEffectAny(effectID, target) is not null;

        /// <summary> Finds an effect on the current target. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindTargetEffectAny(ushort effectID) => FindEffect(effectID, CurrentTarget, null);

        public static Status? FindTargetEffectAny(ushort effectID, IGameObject target) => FindEffect(effectID, target, null);

        /// <summary> Finds an effect on the given object. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <param name="obj"> Object to look for effects on. </param>
        /// <param name="sourceID"> Source object ID. </param>
        /// <returns> Status object or null. </returns>
        public static Status? FindEffect(ushort effectID, IGameObject? obj, ulong? sourceID) => Service.ComboCache.GetStatus(effectID, obj, sourceID);

        ///<summary> Checks a member object for an effect. The effect may be owned by anyone or unowned. </summary>
        /// <param name="effectID"> Status effect ID. </param>
        /// <param name="obj"></param>
        /// <param name="playerOwned"> Checks if the player created the status effect</param>
        /// <return> Status object or null. </return>
        public static Status? FindEffectOnMember(ushort effectID, IGameObject? obj, bool playerOwned = false) => Service.ComboCache.GetStatus(effectID, obj, playerOwned ? Player.Object.GameObjectId : null);

        /// <summary>
        /// Checks if a specific object has a certain status and returns the status as an out parameter
        /// </summary>
        /// <param name="effectID"></param>
        /// <param name="obj"></param>
        /// <param name="playerOwned"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool MemberHasEffect(ushort effectID, IGameObject? obj, bool playerOwned, out Status? status)
        {
            status = Service.ComboCache.GetStatus(effectID, obj, playerOwned ? LocalPlayer.GameObjectId : null);
            return status != null;
        }

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
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Pacification status. </summary>
        /// <returns></returns>
        public static bool HasPacification()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(6)))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        /// <summary> Checks if the character has the Amnesia status. </summary>
        /// <returns></returns>
        public static bool HasAmnesia()
        {
            foreach (uint status in ActionWatching.GetStatusesByName(ActionWatching.GetStatusName(5)))
            {
                if (HasEffectAny((ushort)status)) return true;
            }

            return false;
        }

        public static bool TargetHasDamageDown(IGameObject? target)
        {
            foreach (var status in ActionWatching.GetStatusesByName(GetStatusName(62)))
            {
                if (FindEffectOnMember((ushort)status, target) is not null) return true;
            }

            return false;
        }

        public static bool TargetHasRezWeakness(IGameObject? target, bool checkForWeakness = true)
        {
            if (checkForWeakness)
                foreach (var status in ActionWatching.GetStatusesByName(
                             GetStatusName(All.Debuffs.Weakness)))
                    if (FindEffectOnMember((ushort)status, target) is not null) return true;

            foreach (var status in ActionWatching.GetStatusesByName(
                         GetStatusName(All.Debuffs.BrinkOfDeath)))
                if (FindEffectOnMember((ushort)status, target) is not null) return true;

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
            if ((HasEffect(4192) && !tar.StatusList.Any(x => x.StatusId == 4193)) ||
                (HasEffect(4194) && !tar.StatusList.Any(x => x.StatusId == 4195)) ||
                (HasEffect(4196) && !tar.StatusList.Any(x => x.StatusId == 4197)))
                return true;

            // Yorha raid encounter
            if ((GetAllianceGroup() != AllianceGroup.GroupA && tar.StatusList.Any(x => x.StatusId == 2409)) ||
                (GetAllianceGroup() != AllianceGroup.GroupB && tar.StatusList.Any(x => x.StatusId == 2410)) ||
                (GetAllianceGroup() != AllianceGroup.GroupC && tar.StatusList.Any(x => x.StatusId == 2411)))
                return true;

            // Omega
            if ((tar.StatusList.Any(x => x.StatusId == 1674 || x.StatusId == 3454) && (HasEffect(1660) || HasEffect(3499))) ||
                (tar.StatusList.Any(x => x.StatusId == 1675) && (HasEffect(1661) || HasEffect(3500))))
                return true;


            return false;
        }
    }
}
