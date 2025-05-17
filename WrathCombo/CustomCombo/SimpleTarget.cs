#region

using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;

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
        /// A stack of Mouse Over targets, primarily used for Overrides.
        public static IGameObject? MouseOver =>
            UIMouseOverTarget ?? ModelMouseOverTarget;

        /// A very common stack that targets an ally or self, if there are no manual
        /// overrides targeted.
        /// <remarks>
        ///     "Overrides" include MouseOver at the top of the stack and the
        ///     Hard Target near the bottom.
        /// </remarks>
        /// <!-- todo: maybe including focus target should be an option? -->
        public static IGameObject? OverridesAllies =>
            MouseOver ?? FocusTarget ?? SoftTarget ?? HardTarget ?? Self;

        /// A very common stack that targets the player, if there are no manual
        /// overrides targeted.
        /// <remarks>
        ///     "Overrides" include MouseOver at the top of the stack and the
        ///     Hard Target near the bottom.
        /// </remarks>
        public static IGameObject? OverridesSelf =>
            MouseOver ?? HardTarget ?? Self;

        /// A little mask for Plugin Configuration to make the string a bit shorter.
        private static PluginConfiguration cfg =>
            Service.Configuration;

        /// <summary>
        ///     A very common stack to pick a heal target.
        /// </summary>
        /// <remarks>
        ///     These Items need to be checked for removal: <br />
        ///     - <see cref="SCH.Config.SCH_ST_Heal_Adv"/><br />
        ///     - <see cref="SCH.Config.SCH_ST_Heal_UIMouseOver"/><br />
        ///     - <see cref="SCH.Config.SCH_DeploymentTactics_Adv"/><br />
        ///     - <see cref="SCH.Config.SCH_DeploymentTactics_UIMouseOver"/><br />
        ///     - <see cref="SGE.Config.SGE_ST_Heal_Adv"/><br />
        ///     - <see cref="SGE.Config.SGE_ST_Heal_UIMouseOver"/><br />
        ///     - <see cref="AST.Config.AST_ST_SimpleHeals_Adv"/><br />
        ///     - <see cref="AST.Config.AST_ST_SimpleHeals_UIMouseOver"/><br />
        ///     - <see cref="WHM.Config.WHM_STHeals_UIMouseOver"/><br />
        ///     - <see cref="WHM.Config.WHM_AoEHeals_MedicaMO"/>
        /// </remarks>
        public static IGameObject? AllyToHeal =>
            (cfg.UseMouseoverOverridesInDefaultHealStack
                ? MouseOver.IfFriendly()
                : null) ??
            SoftTarget.IfFriendly() ??
            HardTarget.IfFriendly() ??
            (cfg.UseFocusTargetOverrideInDefaultHealStack
                ? FocusTarget.IfFriendly()
                : null) ??
            (cfg.UseLowestHPOverrideInDefaultHealStack
                ? LowestHPPAlly.IfWithinRange()
                : null) ??
            Self;
        // LowestHPPAlly has the only range-check as the others are "intentional"
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
                                 CustomComboFunctions.IsInRange(x));

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

    #region Role Targets

    public static IGameObject? AnySupport =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.GetRole() is
                CombatRole.Tank or CombatRole.Healer);

    public static IGameObject? AnyDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.GetRole() is CombatRole.DPS);

    #region Slightly More Specific Roles

    public static IGameObject? AnyTank =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.GetRole() is CombatRole.Tank);

    public static IGameObject? AnyHealer =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.GetRole() is CombatRole.Healer);

    public static IGameObject? AnyMeleeDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.ClassJob.RowId.Role() is 2);

    public static IGameObject? AnyRangedDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.ClassJob.RowId.Role() is 3);

    public static IGameObject? AnyPhysRangeDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x =>
                RoleAttribute.GetRoleFromJob(x?.ClassJob.RowId ?? 0) is
                    JobRole.RangedDPS);

    public static IGameObject? AnyMagicalDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x =>
                RoleAttribute.GetRoleFromJob(x?.ClassJob.RowId ?? 0) is
                    JobRole.MagicalDPS);

    #endregion

    #region More Specific Roles

    public static IGameObject? AnyPureHealer =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x =>
                x?.ClassJob.RowId is WHM.JobID or AST.JobID);

    public static IGameObject? AnyShieldHealer =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x =>
                x?.ClassJob.RowId is SCH.JobID or SGE.JobID);

    public static IGameObject? AnySelfishDPS =>
        CustomComboFunctions
            .GetPartyMembers()
            .Select(x => x.BattleChara)
            .FirstOrDefault(x => x?.ClassJob.RowId is
                SAM.JobID or BLM.JobID or MCH.JobID or VPR.JobID);

    #endregion

    #endregion
}
