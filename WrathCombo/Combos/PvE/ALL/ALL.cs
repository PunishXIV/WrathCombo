using ECommons.ExcelServices;
using ECommons.GameHelpers;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class All
{
    /// Used to block user input.
    public const uint SavageBlade = 11;

    public static class Buffs
    {
        public const ushort
            Raised = 148,
            Transcendent = 2648;
    }

    public static class Enums
    {
        /// <summary>
        ///     Whether abilities should be restricted to Bosses or not.
        /// </summary>
        internal enum BossAvoidance
        {
            Off = 1,
            On = 2,
        }

        /// <summary>
        ///     Whether abilities should be restricted to while in a party or not.
        /// </summary>
        internal enum PartyRequirement
        {
            No,
            Yes,
        }
    }

    public static class Debuffs
    {
        public const ushort
            Stun = 2,
            Weakness = 43,
            BrinkOfDeath = 44;
    }

    //Tank Features
    internal class ALL_Tank_Interrupt : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Tank_Interrupt;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (RoleActions.Tank.Interject or RoleActions.Tank.LowBlow or PLD.ShieldBash))
                return actionID;

            var tar = IsEnabled(Preset.ALL_Tank_Interrupt_Retarget) ? SimpleTarget.InterruptableEnemy : CurrentTarget;
            switch (actionID)
            {
                case RoleActions.Tank.LowBlow or PLD.ShieldBash when CanInterruptEnemy(null, tar) && ActionReady(RoleActions.Tank.Interject):
                    return RoleActions.Tank.Interject.Retarget(actionID, tar);

                case RoleActions.Tank.LowBlow or PLD.ShieldBash when TargetIsCasting() && ActionReady(RoleActions.Tank.LowBlow) && !TargetIsBoss() && !JustUsed(RoleActions.Tank.Interject):
                    return RoleActions.Tank.LowBlow.Retarget(actionID, tar);

                case PLD.ShieldBash when IsOnCooldown(RoleActions.Tank.LowBlow) && !JustUsed(RoleActions.Tank.Interject) && !JustUsed(PLD.ShieldBash, 7):
                    return PLD.ShieldBash.Retarget(actionID, tar);

                default:
                    return actionID;
            }
        }
    }

    internal class ALL_Tank_Reprisal : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Tank_Reprisal;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Tank.Reprisal && GetStatusEffectRemainingTime(RoleActions.Tank.Debuffs.Reprisal, CurrentTarget, true) > Config.ALL_Tank_Reprisal_Threshold && IsOffCooldown(RoleActions.Tank.Reprisal)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Tank_Shirk : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Tank_ShirkRetargeting;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not RoleActions.Tank.Shirk)
                return actionID;

            var target =
                IsNotEnabled(Preset.ALL_Tank_ShirkRetargeting_Healer)
                    ? SimpleTarget.AnyLivingTank
                    : SimpleTarget.AnyLivingHealer;

            if (IsEnabled(Preset.ALL_Tank_ShirkRetargeting_Fallback))
                target ??= SimpleTarget.AnyLivingSupport;

            RoleActions.Tank.Shirk.Retarget(target, dontCull: true);

            return actionID;
        }
    }

    //Healer Features
    internal class ALL_Healer_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Healer_Raise;

        protected override uint Invoke(uint actionID)
        {
            List<uint> replacedActions =
                [WHM.Raise, AST.Ascend, SGE.Egeiro, SCH.Resurrection];
            if (!replacedActions.Contains(actionID))
                return actionID;
            if (actionID is SCH.Resurrection &&
                Player.Job is not Job.SCH)
                return actionID;

            if (ActionReady(RoleActions.Magic.Swiftcast))
                return RoleActions.Magic.Swiftcast;

            if (actionID == WHM.Raise &&
                IsEnabled(Preset.WHM_ThinAirRaise) &&
                ActionReady(WHM.ThinAir) &&
                !HasStatusEffect(WHM.Buffs.ThinAir))
                return WHM.ThinAir;

            if (IsEnabled(Preset.ALL_Healer_Raise_Retarget))
                return actionID.Retarget(replacedActions.ToArray(),
                    SimpleTarget.Stack.AllyToRaise, dontCull: true);

            return actionID;
        }
    }

    internal class ALL_Healer_EsunaRetargeting : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Healer_EsunaRetargeting;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not RoleActions.Healer.Esuna)
                return actionID;

            var target = SimpleTarget.UIMouseOverTarget.IfHasCleansable() ??
                         SimpleTarget.ModelMouseOverTarget.IfHasCleansable() ??
                         SimpleTarget.HardTarget.IfHasCleansable() ??
                         GetPartyMembers().FirstOrDefault(x => x.BattleChara.IfHasCleansable() != null)?.BattleChara;

            return RoleActions.Healer.Esuna.Retarget(target, dontCull: true);
        }
    }

    internal class ALL_Healer_RescueRetargeting : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Healer_RescueRetargeting;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not RoleActions.Healer.Rescue)
                return actionID;

            var target =
                SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty() ??

                //Field Mouseover
                (Config.ALL_Healer_RescueRetargetingOptions[0]
                    ? SimpleTarget.ModelMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Focus target retarget
                (Config.ALL_Healer_RescueRetargetingOptions[1]
                    ? SimpleTarget.FocusTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Focus target retarget
                (Config.ALL_Healer_RescueRetargetingOptions[2]
                    ? SimpleTarget.SoftTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty();

            return actionID.Retarget(target);
        }
    }

    //Caster Features
    internal class ALL_Caster_Addle : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Caster_Addle;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Caster.Addle && HasStatusEffect(RoleActions.Caster.Debuffs.Addle, CurrentTarget, true) && IsOffCooldown(RoleActions.Caster.Addle)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Caster_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Caster_Raise;

        protected override uint Invoke(uint actionID)
        {
            List<uint> replacedActions =
                [BLU.AngelWhisper, RDM.Verraise, SMN.Resurrection];
            if (!replacedActions.Contains(actionID))
                return actionID;
            if (actionID is SMN.Resurrection &&
                Player.Job is not Job.SMN)
                return actionID;

            if (HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) ||
                HasStatusEffect(RDM.Buffs.Dualcast))
                if (IsEnabled(Preset.ALL_Caster_Raise_Retarget))
                    return actionID.Retarget(replacedActions.ToArray(),
                        SimpleTarget.Stack.AllyToRaise, dontCull: true);
                else
                    return actionID;

            if (IsOffCooldown(RoleActions.Magic.Swiftcast))
                return RoleActions.Magic.Swiftcast;

            if (Player.Job is Job.RDM &&
                ActionReady(RDM.Vercure))
                return RDM.Vercure;

            if (IsEnabled(Preset.ALL_Caster_Raise_Retarget))
                return actionID.Retarget(replacedActions.ToArray(),
                    SimpleTarget.Stack.AllyToRaise, dontCull: true);

            return actionID;
        }
    }

    //Melee DPS Features
    internal class ALL_Melee_Feint : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Melee_Feint;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Melee.Feint && HasStatusEffect(RoleActions.Melee.Debuffs.Feint, CurrentTarget, true) && IsOffCooldown(RoleActions.Melee.Feint)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Melee_TrueNorth : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Melee_TrueNorth;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Melee.TrueNorth && HasStatusEffect(RoleActions.Melee.Buffs.TrueNorth)
                ? SavageBlade
                : actionID;
    }

    //Ranged Physical Features
    internal class ALL_Ranged_Mitigation : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Ranged_Mitigation;

        protected override uint Invoke(uint actionID) =>
            actionID is BRD.Troubadour or MCH.Tactician or DNC.ShieldSamba &&
            (HasStatusEffect(BRD.Buffs.Troubadour, anyOwner: true) || HasStatusEffect(MCH.Buffs.Tactician, anyOwner: true) ||
             HasStatusEffect(DNC.Buffs.ShieldSamba, anyOwner: true)) &&
            IsOffCooldown(actionID)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Ranged_Interrupt : CustomCombo
    {
        protected internal override Preset Preset => Preset.ALL_Ranged_Interrupt;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.PhysRanged.FootGraze && CanInterruptEnemy() && ActionReady(RoleActions.PhysRanged.HeadGraze)
                ? RoleActions.PhysRanged.HeadGraze
                : actionID;
    }
}
