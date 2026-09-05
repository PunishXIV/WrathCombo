#region References
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using WrathCombo.Data.BattleData;
using WrathCombo.Services;
using static WrathCombo.Data.StatusCache;
using static WrathCombo.Window.Text;
#endregion


namespace WrathCombo.Extensions
{
    /// <summary>
    /// Contains IStatus? extensions and IBattleChara Extensions revolving around status effects
    /// </summary>
    public static class StatusExtensions
    {
        extension(uint value)
        {
            public string StatusName() => ActionAndStatusLocalization.GetStatusName(value);
        }

        #region IStatus? Extensions
        /// <summary>
        /// Extensions applied to a NULLABLE status, allowing for a fluent chain even if the status is missing.
        /// </summary>
        /// <remarks>
        /// These extensions are designed to work with the result of <see cref="IBattleChara.Status(uint, bool)"/>,
        /// which retrieves a status effect from a character.
        /// </remarks>
        extension(IStatus? status)
        {
            /// <summary>
            /// Returns the stack count, or 0 if the status doesn't exist.
            /// Usage: ushort s = chara.Status(123).Stacks;
            /// </summary>
            public ushort Stacks => status?.Param ?? 0;

            /// <summary>
            /// Returns the name of the Status, empty if null
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public string Name
                => status is null ? string.Empty : ActionAndStatusLocalization.GetStatusName(status.StatusId);

            /// <summary>
            /// Returns the remaining time, or NaN if the status doesn't exist. (will fail comparisons if doesn't exist)
            /// Usage: float t = chara.Status(123).RemainingTimeOrNaN();
            /// </summary>
            public unsafe float RemainingTimeOrNaN(bool checkAnimationLock = true)
            {
                if (status is null) return float.NaN;
                if (checkAnimationLock && status.RemainingTime < 0)
                    return (status.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
                return status.RemainingTime;
            }

            /// <summary>
            /// Returns the remaining time, or 0 if the status doesn't exist.
            /// Usage: float t = chara.Status(123).RemainingTimeOrZero();
            /// </summary>
            public unsafe float RemainingTimeOrZero(bool checkAnimationLock = true)
            {
                if (status is null) return 0;
                if (checkAnimationLock && status.RemainingTime < 0)
                    return (status.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
                return status.RemainingTime;
            }
        }
        #endregion

        #region IBattleChara Extensions
        /// <summary>
        /// Extensions applied to IBattleChara, revolving around status effects
        /// </summary>
        extension(IBattleChara chara)
        {
            /// <summary>
            /// Extracts a specific status effect from the character. Returns Null if it fails
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IStatus? Status(uint id, bool anyOwner = false)
            {
                // Determine the source ID for ownership filtering
                ulong? sourceId = !anyOwner ? Player.Object?.GameObjectId : null;
                return Service.ComboCache.GetStatus(id, chara, sourceId);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasStatus(uint id, bool anyOwner = false) =>
                chara.Status(id, anyOwner) is not null;


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasStatus(uint id, [NotNullWhen(true)] out IStatus? status, bool anyOwner = false)
            {
                status = chara.Status(id, anyOwner);
                return status != null;
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool HasStatusInCacheList(FrozenSet<uint> statusList)
            {
                var statuses = chara.SafeStatusList;
                if (statuses is null)
                    return false;

                foreach (var s in statuses)
                {
                    if (statusList.Contains(s.StatusId))
                        return true;
                }
                return false;
            }

            public bool HasDamageDown => chara.HasStatusInCacheList(Dictionaries.DamageDownStatuses);
            public bool HasDamageUp => chara.HasStatusInCacheList(Dictionaries.DamageUpStatuses);
            public bool HasEvasionUp => chara.HasStatusInCacheList(Dictionaries.EvasionUpStatuses);
            public bool HasRaiseInvincibility => chara.HasStatusInCacheList(Dictionaries.RaiseInvincibilityStatuses);
            public bool HasRaiseStatus => chara.HasStatusInCacheList(Dictionaries.RaiseStatuses);
            public bool HasCleansableDebuff => chara.HasStatusInCacheList(Dictionaries.DispellableStatuses);
            public bool HasCleansableDoom => chara.HasStatusInCacheList(Dictionaries.CleansableDoomStatuses);
            public bool HasBeneficialStatus => chara.HasStatusInCacheList(Dictionaries.BeneficialStatuses);
            public bool HasPhantomDispelStatus => chara.HasDamageUp || chara.HasEvasionUp || chara.HasStatus(OCDarkDefensesStatusId) || chara.IsInvincible;


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasRezWeakness(bool checkForWeakness = true)
            {
                if (checkForWeakness && chara.HasStatus(WeaknessStatusId, true))
                    return true;

                return chara.HasStatus(BrinkOfDeathStatusId, true);
            }

            /// <summary>
            /// Checks if the target is invincible due to status effects or encounter-specific mechanics.
            /// </summary>
            /// <param name="target">The game object to check.</param>
            /// <returns>True if the target is invincible; otherwise, false.</returns>
            public bool IsInvincible
            {
                get
                {
                    if (chara.SafeStatusList is not { } statuses)
                        return false;

                    // Turn Target's status to uint hashset
                    var targetStatuses = statuses.Select(s => s.StatusId).ToHashSet();
                    uint targetID = chara.BaseId;

                    // Returning False in each case because there should be no other General Invincibility Check needed
                    // for specified areas

                    return BattleData.IsInvincible(chara, targetID, targetStatuses) switch
                    {
                        // If target is invincible based on Battle Data
                        BattleData.Invincible.True => true,
                        // Are we to bother with checking statuses per Battle Data
                        BattleData.Invincible.False => false,
                        // General invincibility check, not using StatusCache.HasStatusInCacheList because statuses is derived from SafeStatusList
                        BattleData.Invincible.CheckStatuses => statuses.Any(s => Dictionaries.InvincibleStatuses.Contains(s.StatusId)),
                        _ => false,
                    };
                }
            }

            /// <summary>
            /// Checks if the character's status list is at maximum capacity.
            /// </summary>
            public unsafe bool IsStatusCapped
            {
                get
                {
                    var statusList = chara.SafeStatusList;
                    return statusList is not null && statusList.Count(x => x.StatusId != 0) == chara.Struct()->StatusManager.NumValidStatuses;
                }
            }

            /// <summary>
            /// Checks if a status can be applied to this character based on its category and current status cap.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanApplyStatus(uint statusId)
            {
                //Check to see if it's a buff or debuff and therefore if the target is suitable for the status
                var status = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Status>().GetRow(statusId);
                if ((chara.IsHostile() && status.StatusCategory != 2) || (chara.IsFriendly() && status.StatusCategory != 1))
                    return false;

                if (!chara.IsStatusCapped || chara.HasStatus(statusId))
                    return true;

                return false;
            }

            /// <summary>
            /// Overload to accept a list of status IDs. Uses a manual loop instead of LINQ for performance.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanApplyStatus(uint[] statuses)
            {
                foreach (var statusId in statuses)
                {
                    if (chara.CanApplyStatus(statusId))
                        return true;
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsImmuneToStatus(uint status) =>
                Service.Configuration.StatusBlacklist.Any(x => x.Status == status && x.BaseId == chara.BaseId);

            public bool HasStatusEffects(
                uint[] statusIds,
                bool anyOwner = false,
                bool matchAll = false)
            {
                var statuses = chara.SafeStatusList;
                if (statuses is null)
                    return false;

                ulong? sourceId = !anyOwner ? Player.Object?.GameObjectId : null;

                var statusIdSet = new HashSet<uint>(statusIds);

                if (matchAll)
                {
                    // Check that ALL status IDs we're looking for exist on the target
                    return statusIdSet.All(statusId => statuses.Any(s =>
                        s.StatusId == statusId &&
                        (!sourceId.HasValue || s.SourceId == 0 || s.SourceId == sourceId)));
                }
                else
                {
                    // Check if ANY status matches
                    return statuses.Any(s =>
                        statusIdSet.Contains(s.StatusId) &&
                        (!sourceId.HasValue || s.SourceId == 0 || s.SourceId == sourceId));
                }
            }
        }
        #endregion
    }
}