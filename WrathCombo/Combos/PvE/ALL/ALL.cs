using ECommons.DalamudServices;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvE;

internal partial class All
{
    public const byte JobID = 0;

    /// Used to block user input.
    public const uint SavageBlade = 11;

    public const uint
        Sprint = 3;

    private const uint
        IsleSprint = 31314;

    public static class Buffs{}

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
            Weakness = 43,
            BrinkOfDeath = 44;
    }

    internal class ALL_IslandSanctuary_Sprint : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_IslandSanctuary_Sprint;

        protected override uint Invoke(uint actionID) =>
            actionID is Sprint && Svc.ClientState.TerritoryType is 1055
                ? IsleSprint
                : actionID;
    }

    //Tank Features
    internal class ALL_Tank_Interrupt : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Tank_Interrupt;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case RoleActions.Tank.LowBlow or PLD.ShieldBash when CanInterruptEnemy() && ActionReady(RoleActions.Tank.Interject):
                    return RoleActions.Tank.Interject;

                case RoleActions.Tank.LowBlow or PLD.ShieldBash when TargetIsCasting() && ActionReady(RoleActions.Tank.LowBlow):
                    return RoleActions.Tank.LowBlow;

                case PLD.ShieldBash when IsOnCooldown(RoleActions.Tank.LowBlow):
                default:
                    return actionID;
            }
        }
    }

    internal class ALL_Tank_Reprisal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Tank_Reprisal;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Tank.Reprisal && TargetHasEffectAny(RoleActions.Tank.Debuffs.Reprisal) && IsOffCooldown(RoleActions.Tank.Reprisal)
                ? SavageBlade
                : actionID;
    }

    //Healer Features
    internal class ALL_Healer_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Healer_Raise;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case WHM.Raise or AST.Ascend or SGE.Egeiro:
                case SCH.Resurrection when LocalPlayer.ClassJob.Value.RowId is SCH.JobID:
                {
                    if (ActionReady(RoleActions.Magic.Swiftcast))
                        return RoleActions.Magic.Swiftcast;

                    if (actionID == WHM.Raise && IsEnabled(CustomComboPreset.WHM_ThinAirRaise) &&
                        ActionReady(WHM.ThinAir) && !HasEffect(WHM.Buffs.ThinAir))
                        return WHM.ThinAir;

                    return actionID;
                }

                default:
                    return actionID;
            }
        }
    }

    //Caster Features
    internal class ALL_Caster_Addle : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Caster_Addle;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Caster.Addle && TargetHasEffectAny(RoleActions.Caster.Debuffs.Addle) && IsOffCooldown(RoleActions.Caster.Addle)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Caster_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Caster_Raise;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case BLU.AngelWhisper or RDM.Verraise:
                case SMN.Resurrection when LocalPlayer.ClassJob.RowId is SMN.JobID:
                {
                    if (HasEffect(RoleActions.Magic.Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast))
                        return actionID;

                    if (IsOffCooldown(RoleActions.Magic.Swiftcast))
                        return RoleActions.Magic.Swiftcast;

                    if (LocalPlayer.ClassJob.RowId is RDM.JobID &&
                        ActionReady(RDM.Vercure))
                        return RDM.Vercure;

                    break;
                }
            }

            return actionID;
        }
    }

    //Melee DPS Features
    internal class ALL_Melee_Feint : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Melee_Feint;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Melee.Feint && TargetHasEffectAny(RoleActions.Melee.Debuffs.Feint) && IsOffCooldown(RoleActions.Melee.Feint)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Melee_TrueNorth : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Melee_TrueNorth;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.Melee.TrueNorth && HasEffect(RoleActions.Melee.Buffs.TrueNorth)
                ? SavageBlade
                : actionID;
    }

    //Ranged Physical Features
    internal class ALL_Ranged_Mitigation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Ranged_Mitigation;

        protected override uint Invoke(uint actionID) =>
            actionID is BRD.Troubadour or MCH.Tactician or DNC.ShieldSamba &&
            (HasEffectAny(BRD.Buffs.Troubadour) || HasEffectAny(MCH.Buffs.Tactician) ||
             HasEffectAny(DNC.Buffs.ShieldSamba)) &&
            IsOffCooldown(actionID)
                ? SavageBlade
                : actionID;
    }

    internal class ALL_Ranged_Interrupt : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Ranged_Interrupt;

        protected override uint Invoke(uint actionID) =>
            actionID is RoleActions.PhysRanged.FootGraze && CanInterruptEnemy() && ActionReady(RoleActions.PhysRanged.HeadGraze)
                ? RoleActions.PhysRanged.HeadGraze
                : actionID;
    }
}
