using Dalamud.Game.ClientState.Conditions;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using WrathCombo.Native;

namespace WrathCombo.Combos.PvE;

internal partial class BLU : Caster
{
    #region DPS

    internal class BLU_ST_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, SonicBoom))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.SingleTargetDPS) &&
                IsEnabled(Preset.BLU_ST_Tank) &&
                HasTankMimicry)
                return actionID;

            return DoDPS(actionID, actionID, false);
        }
    }

    internal class BLU_AoE_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_AoE_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Electrogenesis))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.AoEDPS) &&
                IsEnabled(Preset.BLU_AoE_Tank) &&
                HasTankMimicry)
                return actionID;

            return DoDPS(actionID, actionID, true);
        }
    }

    #endregion

    #region Tank

    internal class BLU_ST_Tank : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_ST_Tank;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, GoblinPunch))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.SingleTargetDPS) &&
                IsEnabled(Preset.BLU_ST_DPS) &&
                !HasTankMimicry)
                return actionID;

            return DoTank(actionID, actionID, false);
        }
    }

    internal class BLU_AoE_Tank : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_AoE_Tank;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, RightRound))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.AoEDPS) &&
                IsEnabled(Preset.BLU_AoE_DPS) &&
                !HasTankMimicry)
                return actionID;

            return DoTank(actionID, actionID, true);
        }
    }

    #endregion

    #region Healer

    internal class BLU_ST_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetHeals, PomCure))
                return actionID;

            return DoHeal(actionID, false);
        }
    }

    internal class BLU_AoE_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEHeals, WhiteWind))
                return actionID;

            return DoHeal(actionID, true);
        }
    }

    #endregion

    #region Miscellaneous

    internal class BLU_FinalSting : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_FinalSting;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is FinalSting)
            {
                if (IsEnabled(Preset.BLU_SoloMode) && HasCondition(ConditionFlag.BoundByDuty) && !LocalPlayer.HasStatus(Buffs.BasicInstinct) && GetPartyMembers().Count == 0 && ActionReady(BasicInstinct))
                    return BasicInstinct;
                if (!LocalPlayer.HasStatus(Buffs.Whistle) && ActionReady(Whistle) && !WasLastAction(Whistle))
                    return Whistle;
                if (!LocalPlayer.HasStatus(Buffs.Tingle) && ActionReady(Tingle) && !WasLastSpell(Tingle))
                    return Tingle;
                if (!LocalPlayer.HasStatus(Buffs.MoonFlute) && !WasLastSpell(MoonFlute) && ActionReady(MoonFlute))
                    return MoonFlute;
                if (IsEnabled(Preset.BLU_Primals))
                {
                    if (ActionReady(RoseOfDestruction))
                        return RoseOfDestruction;
                    if (ActionReady(FeatherRain))
                        return FeatherRain.Retarget(FinalSting,
                            SimpleTarget.HardTarget.IfHostile() ??
                            SimpleTarget.LastHostileHardTarget);
                    if (ActionReady(Eruption))
                        return Eruption;
                    if (ActionReady(MatraMagic))
                        return MatraMagic;
                    if (ActionReady(GlassDance))
                        return GlassDance;
                    if (ActionReady(ShockStrike))
                        return ShockStrike;
                }

                if (ActionReady(Role.Swiftcast))
                    return Role.Swiftcast;
                if (ActionReady(FinalSting))
                    return FinalSting;
            }

            return actionID;
        }
    }

    internal class BLU_Ultravibrate : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_Ultravibrate;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Ultravibration)
            {
                if (IsEnabled(Preset.BLU_HydroPull) && !InMeleeRange() && ActionReady(HydroPull))
                    return HydroPull;
                if (!CurrentTarget.HasStatus(Debuffs.DeepFreeze, true) && IsOffCooldown(Ultravibration) && ActionReady(RamsVoice))
                    return RamsVoice;

                if (CurrentTarget.HasStatus(Debuffs.DeepFreeze, true))
                {
                    if (ActionReady(Role.Swiftcast))
                        return Role.Swiftcast;
                    if (ActionReady(Ultravibration))
                        return Ultravibration;
                }
            }

            return actionID;
        }
    }

    internal class BLU_DebuffCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_DebuffCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Devour or Offguard or BadBreath)
            {
                if (!CurrentTarget.HasStatus(Debuffs.Offguard, true) && ActionReady(Offguard))
                    return Offguard;
                if (!CurrentTarget.HasStatus(Debuffs.Malodorous, true) && LocalPlayer.HasStatus(Buffs.TankMimicry) && ActionReady(BadBreath))
                    return BadBreath;
                if (ActionReady(Devour) && LocalPlayer.HasStatus(Buffs.TankMimicry))
                    return Devour;
                if (Role.CanLucidDream(9000))
                    return Role.LucidDreaming;
            }

            return actionID;
        }
    }

    internal class BLU_Addle : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_Addle;

        protected override uint Invoke(uint actionID) => actionID is MagicHammer && IsOnCooldown(MagicHammer) && ActionReady(Role.Addle) && !CurrentTarget.HasStatus(Role.Debuffs.Addle) && !CurrentTarget.HasStatus(Debuffs.Conked) ? Role.Addle : actionID;
    }

    internal class BLU_KnightCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_KnightCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is WhiteKnightsTour or BlackKnightsTour)
            {
                if (CurrentTarget.HasStatus(Debuffs.Slow) && ActionReady(BlackKnightsTour))
                    return BlackKnightsTour;
                if (CurrentTarget.HasStatus(Debuffs.Bind) && ActionReady(WhiteKnightsTour))
                    return WhiteKnightsTour;
            }

            return actionID;
        }
    }

    internal class BLU_LightHeadedCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_LightHeadedCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is PeripheralSynthesis)
            {
                if (!CurrentTarget.HasStatus(Debuffs.Lightheaded) && ActionReady(PeripheralSynthesis))
                    return PeripheralSynthesis;
                if (CurrentTarget.HasStatus(Debuffs.Lightheaded) && ActionReady(MustardBomb))
                    return MustardBomb;
            }

            return actionID;
        }
    }

    internal class BLU_PerpetualRayStunCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_PerpetualRayStunCombo;

        protected override uint Invoke(uint actionID) => actionID is PerpetualRay && (CurrentTarget.HasStatus(Debuffs.Stun, true) || WasLastAction(PerpetualRay)) && ActionReady(SharpenedKnife) && InMeleeRange() ? SharpenedKnife : actionID;
    }

    internal class BLU_PeatClean : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_PeatClean;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is DeepClean)
            {
                if (ActionReady(PeatPelt) && !CurrentTarget.HasStatus(Debuffs.Begrimed))
                    return PeatPelt;
            }

            return actionID;
        }
    }

    #endregion
}
