#region

using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Logging;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
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
        public static IGameObject? MouseOver =>
            UIMouseOverTarget ?? ModelMouseOverTarget;

        /// A very common stack that targets an ally or self, if there are no manual
        /// overrides targeted.
        /// <remarks>
        ///     "Overrides" include MouseOver at the top of the stack and the
        ///     Hard Target near the bottom.
        /// </remarks>
        public static IGameObject? OverridesAllies =>
            UIMouseOverTarget ?? FocusTarget ?? SoftTarget ?? HardTarget ?? Self;

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
            FocusTarget ?? SoftTarget ?? HardTarget ?? Self;

        /// A little mask for Plugin Configuration to make the string a bit shorter.
        private static PluginConfiguration cfg =>
            Service.Configuration;

        /// <summary>
        ///     A very common stack to pick a heal target.
        /// </summary>
        /// <seealso cref="DefaultHealStack"/>
        /// <seealso cref="CustomHealStack"/>
        public static IGameObject? AllyToHeal =>
            cfg.UseCustomHealStack ? CustomHealStack : DefaultHealStack;

        /// <summary>
        ///     The Default Heal Stack, with customization options.
        /// </summary>
        /// <remarks>
        ///     LowestHPPAlly and FocusTarget are the only ones with a range check,
        ///     as the others are "intentional" at the time they are grabbed.
        /// </remarks>
        public static IGameObject? DefaultHealStack =>
            (cfg.UseUIMouseoverOverridesInDefaultHealStack
                ? UIMouseOverTarget.IfFriendly()
                : null) ??
            (cfg.UseFieldMouseoverOverridesInDefaultHealStack
                ? ModelMouseOverTarget.IfFriendly()
                : null) ??
            SoftTarget.IfFriendly() ??
            HardTarget.IfFriendly() ??
            (cfg.UseFocusTargetOverrideInDefaultHealStack
                ? FocusTarget.IfFriendly().IfWithinRange().IfMissingHP()
                : null) ??
            (cfg.UseLowestHPOverrideInDefaultHealStack
                ? LowestHPPAlly.IfWithinRange().IfMissingHP()
                : null) ??
            Self;

        #region Custom Heal Stack Resolving

        /// <summary>
        ///     The Custom Heal Stack, fully user-made.
        /// </summary>
        /// <seealso cref="PluginConfiguration.CustomHealStack"/>
        internal static IGameObject? CustomHealStack
        {
            get
            {
                var logging = EZ.Throttle("customHealStackLog", TS.FromSeconds(10));

                foreach (var name in Service.Configuration.CustomHealStack)
                {
                    var resolved = GetSimpleTargetValueFromName(name);
                    var target = resolved.IfFriendly().IfWithinRange();

                    // Only include Lowest HP Ally options if they are missing HP
                    if (name.Contains("Lowest"))
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
                    return Self;

                return null;
            }
        }

        private static IGameObject? GetSimpleTargetValueFromName (string name)
        {
            try
            {
                var property = typeof(SimpleTarget).GetProperty(name);
                if (property == null) return null;
                var value = property.GetValue(null);
                return value as IGameObject;
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Error getting target value from name: '{name}'. " +
                                  $"Edited value?\n{e}");
                return null;
            }
        }

        #endregion
    }

    #endregion

    #region Core Targets

    public static IGameObject? Self =>
        Player.Available ? Player.Object : null;

    public static IGameObject? HardTarget =>
        Svc.Targets.Target;

    public static IGameObject? SoftTarget =>
        Svc.Targets.SoftTarget;

    public static IGameObject? FocusTarget =>
        Svc.Targets.FocusTarget;

    public static IGameObject? TargetsTarget =>
        Svc.Targets.Target is { TargetObjectId: not 0xE0000000 }
            ? Svc.Targets.Target.TargetObject
            : null;

    public static IGameObject? UIMouseOverTarget
    {
        get
        {
            if (!PronounService.PronounsReady) return null;
            unsafe
            {
                return GameObjectExtensions.GetObjectFrom(
                    PronounService.Module->UiMouseOverTarget);
            }
        }
    }

    public static IGameObject? ModelMouseOverTarget =>
        Svc.Targets.MouseOverNameplateTarget ?? Svc.Targets.MouseOverTarget;

    public static IGameObject? Chocobo =>
        Svc.Buddies.CompanionBuddy?.GameObject;

    public static IGameObject? AnyEnemy =>
        Svc.Objects
            .OfType<IBattleChara>()
            .FirstOrDefault(x => x.IsHostile() && x.IsTargetable &&
                                 x.IsWithinRange());

    #region Previous Targets

    public static IGameObject? LastHardTarget =>
        PronounService.GetIGameObjectFromPronounID(1006);

    public static IGameObject? LastHostileHardTarget =>
        PronounService.GetIGameObjectFromPronounID(1084);

    public static IGameObject? MostRecentAttacker =>
        PronounService.GetIGameObjectFromPronounID(1008);

    #endregion

    #endregion

    #region Party Targets

    public static IGameObject? KardionTarget =>
        Svc.Objects
            .OfType<IBattleChara>()
            .FirstOrDefault(x =>
                CustomComboFunctions.HasStatusEffect(SGE.Buffs.Kardion, x));

    public static IGameObject? AnyDeadPartyMember =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.IsDead == true);

    #region HP-Based Targets

    public static IGameObject? LowestHPAlly =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x?.IsDead == false)
            .OrderBy(x => x?.CurrentHp)
            .FirstOrDefault();

    public static IGameObject? LowestHPPAlly =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x?.IsDead == false)
            .OrderBy(x => x?.CurrentHp / x?.MaxHp * 100)
            .FirstOrDefault();

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
    /// <remarks>IDs start at 44 and go to 51</remarks>
    public static IGameObject? GetPartyMemberInSlotSlot(int slot) =>
        slot switch
        {
            < 1 or > 8 => null,
            1 => Self,
            _ => PronounService.GetIGameObjectFromPronounID(42 + slot),
        };

    #endregion

    #endregion

    #region Role Targets (that are not the current player)

    /// Gets any Tank or Healer that is not the player.
    public static IGameObject? AnySupport =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.GetRole() is
                CombatRole.Tank or CombatRole.Healer);

    /// Gets any DPS that is not the player.
    public static IGameObject? AnyDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.GetRole() is CombatRole.DPS);

    #region Slightly More Specific Roles

    /// Gets any Tank that is not the player.
    public static IGameObject? AnyTank =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.GetRole() is CombatRole.Tank);

    /// Gets any Healer that is not the player.
    public static IGameObject? AnyHealer =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.GetRole() is CombatRole.Healer);

    /// Gets any Melee DPS that is not the player.
    public static IGameObject? AnyMeleeDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.ClassJob.RowId.Role() is 2);

    /// Gets any Physical Ranged DPS that is not the player.
    public static IGameObject? AnyRangedDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.ClassJob.RowId.Role() is 3);

    /// Gets any Magical DPS that is not the player.
    public static IGameObject? AnyPhysRangeDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x =>
                RoleAttribute.GetRoleFromJob(x?.ClassJob.RowId ?? 0) is
                    JobRole.RangedDPS);

    /// Gets any Magical DPS that is not the player.
    public static IGameObject? AnyMagicalDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x =>
                RoleAttribute.GetRoleFromJob(x?.ClassJob.RowId ?? 0) is
                    JobRole.MagicalDPS);

    #endregion

    #region More Specific Roles

    /// Gets any Pure Healer that is not the player.
    public static IGameObject? AnyPureHealer =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x =>
                x?.ClassJob.RowId is WHM.JobID or AST.JobID);

    /// Gets any Shield Healer that is not the player.
    public static IGameObject? AnyShieldHealer =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x =>
                x?.ClassJob.RowId is SCH.JobID or SGE.JobID);

    /// Gets any Selfish DPS that is not the player.
    public static IGameObject? AnySelfishDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .Where(x => x.IsNotThePlayer())
            .FirstOrDefault(x => x?.ClassJob.RowId is
                SAM.JobID or BLM.JobID or MCH.JobID or VPR.JobID);

    #endregion

    #endregion
}
