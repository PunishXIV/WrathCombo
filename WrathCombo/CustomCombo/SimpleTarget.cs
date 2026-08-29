#region

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.CustomComboNS.Functions.Jobs;
// ReSharper disable once RedundantUsingDirective
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

#endregion

namespace WrathCombo.CustomComboNS;

internal static class SimpleTarget
{
    #region Common Target Stacks

    /// <summary>
    ///     A collection of common targeting "stacks", used when you want a number of
    ///     different target options, with fallback values.<br />
    ///     (and overriding values)
    /// </summary>
    internal static class Stack
    {
        /// A stack of Mouse Over targets (including model mouseover).
        public static IBattleChara? MouseOver =>
            UIMouseOverTarget as IBattleChara ?? 
            ModelMouseOverTarget as IBattleChara;

        /// A very common stack that targets an ally or self, if there are no manual
        /// overrides targeted.
        /// <remarks>
        ///     "Overrides" include MouseOver at the top of the stack and the
        ///     Hard Target near the bottom.
        /// </remarks>
        public static IGameObject? OverridesAllies =>
            UIMouseOverTarget ?? FocusTarget.IfFriendly() ??
            SoftTarget.IfFriendly() ?? HardTarget.IfFriendly() ??
            Self;

        /// A very common stack that targets the player, if there are no manual
        /// overrides targeted.
        /// <remarks>
        ///     "Overrides" include MouseOver at the top of the stack and the
        ///     Hard Target near the bottom.
        /// </remarks>
        public static IGameObject? OverridesSelf =>
            UIMouseOverTarget ?? HardTarget ?? Self;

        /// A very common stack that targets an ally or self.
        public static IGameObject? Allies =>
            FocusTarget.IfFriendly() ?? SoftTarget.IfFriendly() ??
            HardTarget.IfFriendly() ?? Self;

        /// A little mask for Plugin Configuration to make the string a bit shorter.
        private static Configuration cfg =>
            Service.Configuration;

        /// <summary>
        ///     A very common stack to pick a heal target, whether the user is
        ///     using the Default or Custom Heal Stack.
        /// </summary>
        /// <seealso cref="DefaultHealStack" />
        /// <seealso cref="CustomHealStack" />
        public static IGameObject? AllyToHeal => GetStack();

        /// <summary>
        /// Used exclusively for one-button healing features where retargeting may be optional.
        /// </summary>
        /// <seealso cref="AllyToHeal"/>
        public static IGameObject? OneButtonHealLogic => AutoRotationController.AutorotHealTarget ?? AllyToHeal;

        /// <summary>
        ///     The Default Heal Stack, with customization options.
        /// </summary>
        /// <remarks>
        ///     LowestHPPAlly and FocusTarget are the only ones with a range check,
        ///     as the others are "intentional" at the time they are grabbed.
        /// </remarks>
        internal static IGameObject? DefaultHealStack =>
            GetStack(StackOption.DefaultHealStack);

        /// <summary>
        ///     The Custom Heal Stack, fully user-made.
        /// </summary>
        /// <seealso cref="Configuration.CustomHealStack" />
        /// <seealso cref="GetStack" />
        internal static IGameObject? CustomHealStack =>
            GetStack(StackOption.CustomHealStack);

        /// <summary>
        ///     The <see cref="AllyToHeal">Heal Stack</see>, but filtered to
        ///     those with a cleansable status effect, and falling back to
        ///     <see cref="AnyCleansableAlly"/> if no such status effect is
        ///     found in the stack.
        /// </summary>
        public static IGameObject? AllyToEsuna =>
            GetStack(logicForEachEntryInStack:
                target => target.IfHasCleansable()) ??
            AnyCleansableAlly;

        /// <summary>
        ///     The Customizable Raise Stack.
        /// </summary>
        public static IGameObject? AllyToRaise =>
            GetStack(StackOption.RaiseStack, target => target.IfCanUseOn(WHM.Raise));

        /// <summary>
        /// Same as <see cref="AllyToRaise"/> but for the OC raise actions to account for Forked Tower shenanigans.
        /// </summary>
        public static IGameObject? AllyToRaiseOccult =>
            GetStack(StackOption.RaiseStack, target => target.IfCanUseOn(OccultCrescent.Revive));

        /// <summary>
        ///     The <see cref="AllyToHeal">Heal Stack</see>, but filtered to
        ///     those in Line of Sight.
        /// </summary>
        public static IGameObject? AllyToHealPVP =>
            GetStack(logicForEachEntryInStack:
                target => target.IfWithinLineOfSight());

        #region Custom Stack Resolving

        /// <summary>
        ///     Gets the desired Stack, and applies any custom logic to each entry in
        ///     the stack.
        /// </summary>
        /// <param name="stack">
        ///     Which <see cref="StackOption">Stack</see> to get.<br />
        ///     Defaults to <see cref="StackOption.UserChosenHealStack" />.
        /// </param>
        /// <param name="logicForEachEntryInStack">
        ///     A short method, probably of <see cref="GameObjectExtensions" />,
        ///     to apply to each entry in the stack.<br />
        ///     <see cref="AllyToEsuna" /> for an example.
        /// </param>
        /// <returns>
        ///     The first matching target in the stack, or <see langword="null" />.
        /// </returns>
        private static IGameObject? GetStack
        (StackOption stack = StackOption.UserChosenHealStack,
            Func<IGameObject?, IGameObject?>? logicForEachEntryInStack = null)
        {
            #region Default Heal Stack

            if (stack is StackOption.DefaultHealStack ||
                (stack is StackOption.UserChosenHealStack &&
                 !cfg.UseCustomHealStack))
                return
                    (cfg.UseUIMouseoverOverridesInDefaultHealStack
                        ? CustomLogic(UIMouseOverTarget.IfFriendly())
                        : null) ??
                    (cfg.UseFieldMouseoverOverridesInDefaultHealStack
                        ? CustomLogic(ModelMouseOverTarget.IfFriendly())
                        : null) ??
                    CustomLogic(SoftTarget.IfFriendly()) ??
                    CustomLogic(HardTarget.IfFriendly()) ??
                    (cfg.UseFocusTargetOverrideInDefaultHealStack
                        ? CustomLogic(FocusTarget.IfFriendly().IfWithinRange())
                        : null) ??
                    (cfg.UseLowestHPOverrideInDefaultHealStack
                        ? CustomLogic(LowestHPPAlly.IfWithinRange().IfMissingHP())
                        : null) ??
                   CustomLogic(Self);

            #endregion

            #region Custom Heal Stack

            if (stack is StackOption.CustomHealStack ||
                (stack is StackOption.UserChosenHealStack &&
                 cfg.UseCustomHealStack))
            {
                var logging = EZ.Throttle("customHealStackLog", TS.FromSeconds(10));

                foreach (var name in Service.Configuration.CustomHealStack)
                {
                    var resolved = GetSimpleTargetValueFromName(name);
                    var target =
                        CustomLogic(resolved.IfFriendly().IfTargetable()
                            .IfWithinRange());

                    // Only include Missing-HP options if they are missing HP
                    if (name.Contains("Missing"))
                        target = target.IfMissingHP();

                    if (logging)
                        PluginLog.Verbose(
                            $"[Custom Heal Stack] {name,-25} => " +
                            $"{resolved?.Name ?? "null",-30}" +
                            $" (friendly: {resolved.IsFriendly(),5}, " +
                            $"within range: {resolved.IsWithinRange(),5}, " +
                            $"missing HP: {resolved.IsMissingHP(),5})"
                        );

                    if (target != null) return target;
                }

                // Fall back to Self, if the stack is small and returned nothing
                if (Service.Configuration.CustomHealStack.Length <= 3)
                    return CustomLogic(Self);
            }

            #endregion

            #region Raise Stack

            if (stack is StackOption.RaiseStack)
            {
                var logging = EZ.Throttle("raiseStackLog", TS.FromSeconds(10));

                foreach (var name in Service.Configuration.RaiseStack)
                {
                    var resolved = GetSimpleTargetValueFromName(name);
                    var target = CustomLogic(resolved.IfTargetable().IfDead().IfWithinRange(30));

                    if (logging)
                        PluginLog.Verbose(
                            $"[Custom Raise Stack] {name,-25} => " +
                            $"{resolved?.Name ?? "null",-30}" +
                            $" (Can pop rez on: {resolved.IfCanUseOn(WHM.Raise),5}, " +
                            $"within range: {resolved.IsWithinRange(),5}, " +
                            $"is dead: {resolved.IsDead(),5})"
                        );

                    if (target != null) return target;
                }

                // Fall back to Hard Target, if the stack is small and returned nothing
                if (Service.Configuration.RaiseStack.Length <= 4)
                    return CustomLogic(HardTarget.IfDead()) ??
                           CustomLogic(AnyDeadPartyMember);
            }

            #endregion

            return null;

            IGameObject? CustomLogic(IGameObject? target)
            {
                if (target is null) return null;
                if (logicForEachEntryInStack is null) return target;

                return logicForEachEntryInStack(target);
            }
        }

        private static IBattleChara? GetSimpleTargetValueFromName(string name)
        {
            try
            {
                var property = typeof(SimpleTarget).GetProperty(name);
                if (property == null) return null;
                var value = property.GetValue(null);
                return value as IBattleChara;
            }
            catch (Exception e)
            {
                PluginLog.Warning(
                    $"Error getting target value from name: '{name}'. " +
                    $"Edited value?\n{e}");
                return null;
            }
        }

        private enum StackOption
        {
            UserChosenHealStack,
            DefaultHealStack,
            CustomHealStack,
            RaiseStack,
        }

        #endregion
    }

    #endregion

    #region Core Targets

    public static IPlayerCharacter? Self =>
        Player.Available ? Player.Object : null;

    public static IBattleChara? HardTarget =>
        Svc.Targets.Target as IBattleChara;

    public static IBattleChara? SoftTarget =>
        Svc.Targets.SoftTarget as IBattleChara;

    public static IBattleChara? SoftTargetIfMissingHP =>
        (Svc.Targets.SoftTarget as IBattleChara).IfMissingHP();

    public static IBattleChara? FocusTarget =>
        Svc.Targets.FocusTarget as IBattleChara;

    public static IBattleChara? FocusTargetIfMissingHP =>
        (Svc.Targets.FocusTarget as IBattleChara).IfMissingHP();

    public static IBattleChara? TargetsTarget =>
        Svc.Targets.Target is { TargetObjectId: not 0xE0000000 }
            ? Svc.Targets.Target.TargetObject as IBattleChara
            : null;

    public static IBattleChara? UIMouseOverTarget => PronounService.UIMouseOverTarget as IBattleChara;

    public static IBattleChara? ModelMouseOverTarget =>
        Svc.Targets.MouseOverNameplateTarget as IBattleChara ?? 
        Svc.Targets.MouseOverTarget as IBattleChara;

    public static IGameObject? Chocobo =>
        Svc.Buddies.CompanionBuddy?.GameObject;

    #region Enemies

    private static IEnumerable<IBattleChara> GetValidEnemies(float range = 25, bool checkInvuln = true) =>
        Svc.Objects.GetBattleCharas().Where(x =>
            x.IsHostile() &&
            x.IsTargetable &&
            x.IsWithinRange(range) &&
            (!checkInvuln || x.IsNotInvincible()));

    public static IBattleChara? AnyEnemy =>
        GetValidEnemies(checkInvuln: false)
            .FirstOrDefault();

    public static IBattleChara? NearestEnemyTarget =>
        GetValidEnemies()
            .Where(x => x.IsInCombat())
            .OrderBy(x => GetTargetDistance(x))
            .FirstOrDefault();

    public static IBattleChara? NearestEnemyOver5YalmsAway =>
        GetValidEnemies()
            .Where(x => x.IsAtLeastFiveYalmsAway())
            .OrderBy(x => GetTargetDistance(x))
            .FirstOrDefault();

    public static IBattleChara? NearestEnemyOver5YalmsAwayNotTargetingPlayer =>
        GetValidEnemies()
            .Where(x => x.IsAtLeastFiveYalmsAway() &&
                        x.TargetObjectId != LocalPlayer?.GameObjectId)
            .OrderBy(x => GetTargetDistance(x))
            .FirstOrDefault();

    public static IBattleChara? FurthestEnemyOver5YalmsAway =>
        GetValidEnemies()
            .Where(x => x.IsAtLeastFiveYalmsAway())
            .OrderByDescending(x => GetTargetDistance(x))
            .FirstOrDefault();

    public static IBattleChara? FurthestEnemyOver5YalmsAwayNotTargetingPlayer =>
        GetValidEnemies()
            .Where(x => x.IsAtLeastFiveYalmsAway() &&
                        x.TargetObjectId != LocalPlayer?.GameObjectId)
            .OrderByDescending(x => GetTargetDistance(x))
            .FirstOrDefault();

    public static IBattleChara? NearestEnemyToTarget
        (IGameObject? target, float maximumRangeFromPlayer = 35f) =>
        GetValidEnemies(maximumRangeFromPlayer)
            .OrderBy(x => GetTargetDistance(x, target ?? CurrentTarget))
            .FirstOrDefault();

    public static IBattleChara? LowestHPEnemy =>
        GetValidEnemies(checkInvuln: false)
            .OrderBy(x => x.CurrentHp)
            .FirstOrDefault();

    public static IBattleChara? LowestHPEnemyIfNotInvuln =>
        GetValidEnemies()
            .OrderBy(x => x.CurrentHp)
            .FirstOrDefault();

    public static IBattleChara? LowestHPPEnemy =>
        GetValidEnemies(checkInvuln: false)
            .OrderBy(x => (float)x.CurrentHp / x.MaxHp)
            .FirstOrDefault();

    public static IBattleChara? LowestHPPEnemyIfNotInvuln =>
        GetValidEnemies()
            .OrderBy(x => (float)x.CurrentHp / x.MaxHp)
            .FirstOrDefault();

    public static IBattleChara? InterruptableEnemy =>
        GetValidEnemies(3)
            .Where(x => x.IsCastInterruptible)
            .OrderByDescending(x =>
                Svc.Targets.Target?.GameObjectId == x.GameObjectId)
            .FirstOrDefault();

    public static IBattleChara? StunnableEnemy(int reStunCheck = 3) =>
        GetValidEnemies(3)
            .Where(x => !x.IsBoss() &&
                        !x.HasStatus(All.Debuffs.Stun) &&
                        (ICDTracker.StatusIsExpired(All.Debuffs.Stun,
                             x.GameObjectId) ||
                         ICDTracker.Trackers.FirstOrDefault(y =>
                                 y.StatusID == All.Debuffs.Stun &&
                                 x.GameObjectId == y.GameObjectId)?
                             .TimesApplied < reStunCheck))
            .OrderByDescending(x =>
                Svc.Targets.Target?.GameObjectId == x.GameObjectId)
            .FirstOrDefault();

    public static IBattleChara? DottableEnemy
    (uint dotAction,
        ushort dotDebuff,
        int minHPPercent = 10,
        float reapplyThreshold = 1,
        int maxNumberOfEnemiesInRange = 3) => DottableEnemy(dotAction, dotDebuff, _ => minHPPercent, reapplyThreshold, maxNumberOfEnemiesInRange);


    public static IBattleChara? DottableEnemy
    (uint dotAction,
        ushort dotDebuff,
        Func<IBattleChara?, int> minHPPercent,
        float reapplyThreshold = 1,
        int maxNumberOfEnemiesInRange = 3)
    {
        var range = dotAction.ActionRange();
        var nearbyEnemies = GetValidEnemies(range)
            .Where(x => x.IsInCombat())
            .ToArray();

        if (nearbyEnemies.Length > maxNumberOfEnemiesInRange)
            return null;

        return nearbyEnemies
            .Select(x => new {
                Enemy = x,
                Time = x.Status(dotDebuff).RemainingTimeOrZero(),
            })
            .Where(item => item.Enemy.CanUseOn(dotAction) &&
                        item.Enemy.Health * 100f > minHPPercent(item.Enemy) &&
                        !JustUsedOn(dotAction, item.Enemy) &&
                        IsInLineOfSight(item.Enemy) &&
                        item.Time <= reapplyThreshold &&
                        item.Enemy.CanApplyStatus(dotDebuff))
            .OrderBy(item => item.Time)
            .ThenByDescending(item => item.Enemy.Health)
            .Select(item => item.Enemy)
            .FirstOrDefault();
    }

    public static IBattleChara? TargetWithDoTLowestRemainingTimer
        (uint dotAction,
        ushort dotDebuff)
    {
        var range = dotAction.ActionRange();
        var nearbyEnemies = GetValidEnemies(range)
            .Where(x => x.IsInCombat())
            .ToArray();

        return nearbyEnemies
            .Select(x => new {
                Enemy = x,
                Time = x.Status(dotDebuff).RemainingTimeOrZero()
            })
            .Where(item => item.Enemy.CanUseOn(dotAction) &&
                        IsInLineOfSight(item.Enemy) &&
                        item.Time > 0 &&
                        item.Enemy.CanApplyStatus(dotDebuff))
            .OrderBy(item => item.Time)
            .Select(item => item.Enemy)
            .FirstOrDefault();
    }

    public static IBattleChara? BardRefreshableEnemy
    (uint refreshAction,
        ushort dotDebuff1,
        ushort dotDebuff2,
        int minHPPercent = 10,
        float minTime = 1,
        int maxNumberOfEnemiesInRange = 3) => BardRefreshableEnemy(refreshAction, dotDebuff1, dotDebuff2, _ => minHPPercent, minTime, maxNumberOfEnemiesInRange);


    public static IBattleChara? BardRefreshableEnemy
    (uint refreshAction,
        ushort dotDebuff1,
        ushort dotDebuff2,
        Func<IBattleChara?, int> minHPPercent,
        float minTime = 1,
        int maxNumberOfEnemiesInRange = 3)
    {
        var range = refreshAction.ActionRange();
        var nearbyEnemies = GetValidEnemies(range)
            .Where(x => x.IsInCombat())
            .ToArray();

        if (nearbyEnemies.Length > maxNumberOfEnemiesInRange)
            return null;

        return nearbyEnemies
            // Cache the IBattleChara and it's Statuses to avoid multiple lookups
            .Select(x => new {
                Enemy = x,
                dot1status = x.Status(dotDebuff1),
                dot2status = x.Status(dotDebuff2)
            })
            .Where(item => item.Enemy.CanUseOn(refreshAction) &&
                            item.Enemy.Health * 100f > minHPPercent(item.Enemy) &&
                            item.dot1status is not null &&
                            item.dot2status is not null &&
                            (item.dot1status.RemainingTimeOrZero() <= minTime ||
                             item.dot2status.RemainingTimeOrZero() <= minTime) &&
                            item.Enemy.CanApplyStatus(dotDebuff1) &&
                            item.Enemy.CanApplyStatus(dotDebuff2))
            .OrderBy(item => item.dot1status.RemainingTimeOrZero())
            .ThenByDescending(item => item.Enemy.Health)
            .Select(item => item.Enemy)
            .FirstOrDefault();
    }

    #endregion

    #region Previous Targets

    public static IGameObject? LastHardTarget =>
        PronounService.GetByPlaceHolder("<lt>");

    public static IGameObject? LastHostileHardTarget =>
        PronounService.GetByPlaceHolder("<le>");

    public static IGameObject? MostRecentAttacker =>
        PronounService.GetByPlaceHolder("<la>");

    #endregion

    #endregion

    #region Party Targets

    public static IBattleChara? KardionTarget =>
        GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x is not null && x.HasStatus(SGE.Buffs.Kardion));

    public static IBattleChara? AnyDeadPartyMember =>
        GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.IsDead() == true);

    public static IBattleChara? AnyDeadNonPartyMember =>
        Svc.Objects
            .GetBattleCharas()
            .Where(x => x.IsAPlayer() && x.IsTargetable &&
                        !x.IsInParty())
            .FirstOrDefault(x => x.IsDead());

    public static IBattleChara? AnyCleansableAlly =>
        GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x is not null && x.IsDead() == false && x.HasCleansableDebuff && IsInLineOfSight(x));

    #region HP-Based Targets

    public static IBattleChara? LowestHPAlly =>
        GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x is not null && x.IsDead() == false)
            .OrderBy(x => x.CurrentHp)
            .FirstOrDefault();

    public static IGameObject? LowestHPAllyIfMissingHP =>
        LowestHPAlly?.IfMissingHP();

    public static IBattleChara? LowestHPPAlly =>
        GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x is not null && x.IsDead() == false)
            .OrderBy(x => (float)x.CurrentHp / x.MaxHp)
            .FirstOrDefault();

    public static IGameObject? LowestHPPAllyIfMissingHP =>
        LowestHPPAlly?.IfMissingHP();

    public static IBattleChara? LowestHPAllyOutOfParty =>
        Svc.Objects.GetBattleCharas()
            .Where(x => x is not null && x.IsAPlayer() && !x.IsInParty() && x.IsDead() == false)
            .OrderBy(x => x.CurrentHp)
            .FirstOrDefault();

    public static IGameObject? LowestHPAllyIfMissingHPOutOfParty =>
        LowestHPAllyOutOfParty?.IfMissingHP();

    public static IGameObject? LowestHPPAllyOutOfParty =>
        Svc.Objects.GetBattleCharas()
            .Where(x => x is not null && x.IsAPlayer() && !x.IsInParty() && x.IsDead() == false)
            .OrderBy(x => (float)x.CurrentHp / x.MaxHp)
            .FirstOrDefault();

    public static IGameObject? LowestHPPAllyIfMissingHPOutOfParty =>
        LowestHPPAllyOutOfParty?.IfMissingHP();


    #endregion

    #region Party Slots

    public static IGameObject? PartyMember1 => GetPartyMemberInSlotSlot(1);
    public static IGameObject? PartyMember2 => GetPartyMemberInSlotSlot(2);
    public static IGameObject? PartyMember3 => GetPartyMemberInSlotSlot(3);
    public static IGameObject? PartyMember4 => GetPartyMemberInSlotSlot(4);
    public static IGameObject? PartyMember5 => GetPartyMemberInSlotSlot(5);
    public static IGameObject? PartyMember6 => GetPartyMemberInSlotSlot(6);
    public static IGameObject? PartyMember7 => GetPartyMemberInSlotSlot(7);
    public static IGameObject? PartyMember8 => GetPartyMemberInSlotSlot(8);

    /// <summary>
    ///     Tries to get a party member, by slot number (1–8).
    /// </summary>
    /// <param name="slot">
    ///     The party slot (1 for local player, 2–8 for party members).
    /// </param>
    /// <returns>
    ///     An <see cref="IGameObject" /> for the party member if found;
    /// </returns>
    public static IGameObject? GetPartyMemberInSlotSlot(int slot) =>
        slot switch
        {
            < 1 or > 8 => null,
            1 => Self,
            _ => PronounService.GetByPlaceHolder($"<{slot}>"),
        };

    #endregion

    #endregion

    #region Role Targets (that are not the current player)

    /// Gets any Tank or Healer that is not the player.
    public static IBattleChara? AnySupport =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.GetRole() is
                CombatRole.Tank or CombatRole.Healer)?.BattleChara;

    /// Gets any living Tank or Healer that is not the player.
    public static IBattleChara? AnyLivingSupport =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer() && !x.BattleChara.IsDead)
            .FirstOrDefault(x => x.GetRole() is
                CombatRole.Tank or CombatRole.Healer)?.BattleChara;

    /// Gets any DPS that is not the player.
    public static IBattleChara? AnyDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.GetRole() is CombatRole.DPS)?.BattleChara;

    #region Slightly More Specific Roles

    /// Gets any Tank that is not the player.
    public static IGameObject? AnyTank =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.GetRole() is CombatRole.Tank)?.BattleChara;

    /// Gets any living Tank that is not the player.
    public static IGameObject? AnyLivingTank =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer() && !x.BattleChara.IsDead)
            .FirstOrDefault(x => x.GetRole() is CombatRole.Tank)?.BattleChara;

    /// Gets any Healer that is not the player.
    public static IGameObject? AnyHealer =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.GetRole() is CombatRole.Healer)?.BattleChara;

    /// Gets any living Healer that is not the player.
    public static IGameObject? AnyLivingHealer =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer() && !x.BattleChara.IsDead)
            .FirstOrDefault(x => x.GetRole() is CombatRole.Healer)?.BattleChara;

    /// Gets any Raiser (Healer or DPS) that is not the player.
    public static IGameObject? AnyRaiser =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.GetRole() is CombatRole.Healer ||
                                 x.RealJob?.GetJob() is Job.SMN or Job.RDM)
            ?.BattleChara;

    /// Gets any Raiser DPS that is not the player.
    public static IGameObject? AnyRaiserDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.RealJob?.GetJob() is Job.SMN or Job.RDM)
            ?.BattleChara;

    /// Gets any Melee DPS that is not the player.
    public static IGameObject? AnyMeleeDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.RealJob?.Role is 2)?.BattleChara;

    /// Gets any Physical Ranged DPS that is not the player.
    public static IGameObject? AnyRangedDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.RealJob?.Role is 3)?.BattleChara;

    /// Gets any Magical DPS that is not the player.
    public static IGameObject? AnyPhysRangeDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x =>
                GetRoleFromJob(x.RealJob?.RowId ?? 0) is
                    JobRole.RangedDPS)?.BattleChara;

    /// Gets any Magical DPS that is not the player.
    public static IGameObject? AnyMagicalDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x =>
                GetRoleFromJob(x.RealJob?.RowId ?? 0) is
                    JobRole.MagicalDPS)?.BattleChara;

    #endregion

    #region Slightly More Specific Roles, with Additions

    /// Gets any Tank that is dead (but only if all tanks are dead).
    public static IBattleChara? AnyDeadTankIfNoneAlive
    {
        get
        {
            var tanks = GetPartyMembers()
                .Where(x =>
                    x.BattleChara.IsNotThePlayer() && x.GetRole() is CombatRole.Tank)
                .ToArray();
            var deadTanks =
                tanks.Where(x => x.BattleChara.IsDead()).ToArray();

            if (deadTanks.Length == 0)
                return null;
            if (tanks.Any(x => !x.BattleChara.IsDead()))
                return null;

            return deadTanks.FirstOrDefault()?.BattleChara;
        }
    }

    /// Gets any Healer that is dead (but only if all healers are dead).
    public static IBattleChara? AnyDeadHealerIfNoneAlive
    {
        get
        {
            var healers = GetPartyMembers()
                .Where(x =>
                    x.BattleChara.IsNotThePlayer() &&
                    x.GetRole() is CombatRole.Healer)
                .ToArray();
            var deadHealers =
                healers.Where(x => x.BattleChara.IsDead()).ToArray();

            if (deadHealers.Length == 0)
                return null;
            if (healers.Any(x => x.BattleChara.IsDead() == false))
                return null;

            return deadHealers.FirstOrDefault()?.BattleChara;
        }
    }

    /// Gets any Raiser (Healer or DPS) that is dead (but only if all Raisers are dead).
    public static IBattleChara? AnyDeadRaiserIfNoneAlive
    {
        get
        {
            var raisers = GetPartyMembers()
                .Where(x => x.BattleChara.IsNotThePlayer() &&
                            (x.GetRole() is CombatRole.Healer ||
                             x.RealJob?.GetJob() is Job.SMN or Job.RDM))
                .ToArray();
            var deadRaisers =
                raisers.Where(x => x.BattleChara.IsDead()).ToArray();

            if (deadRaisers.Length == 0)
                return null;
            if (raisers.Any(x => x.BattleChara.IsDead() == false))
                return null;

            return deadRaisers.FirstOrDefault()?.BattleChara;
        }
    }

    /// Gets any Raiser DPS that is dead (but only if all Raiser DPS are dead).
    public static IBattleChara? AnyDeadRaiserDPSIfNoneAlive
    {
        get
        {
            var raisers = GetPartyMembers()
                .Where(x => x.BattleChara.IsNotThePlayer() &&
                            (x.RealJob?.GetJob()) is Job.SMN or Job.RDM)
                .ToArray();
            var deadRaisers =
                raisers.Where(x => x.BattleChara.IsDead()).ToArray();

            if (deadRaisers.Length == 0)
                return null;
            if (raisers.Any(x => x.BattleChara.IsDead() == false))
                return null;

            return deadRaisers.FirstOrDefault()?.BattleChara;
        }
    }

    #endregion

    #region More Specific Roles

    /// Gets any Pure Healer that is not the player.
    public static IBattleChara? AnyPureHealer =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x =>
                x.RealJob?.GetJob() is Job.WHM or Job.AST)?.BattleChara;

    /// Gets any Shield Healer that is not the player.
    public static IBattleChara? AnyShieldHealer =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x =>
                x.RealJob?.GetJob() is Job.SCH or Job.SGE)?.BattleChara;

    /// Gets any Selfish DPS that is not the player.
    public static IBattleChara? AnySelfishDPS =>
        GetPartyMembers()
            .Where(x => x.BattleChara.IsNotThePlayer())
            .FirstOrDefault(x => x.RealJob?.GetJob() is
                Job.SAM or Job.BLM or Job.MCH or Job.VPR)?.BattleChara;

    #endregion

    #endregion
}