using Dalamud.Game.ClientState.Objects.Types;
using System;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.PLD.Config;

namespace WrathCombo.Combos.PvE;

internal partial class PLD : Tank
{
    #region Simple Modes

    internal class PLD_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, FastBlade)) return actionID;

            const Combo comboFlags = Combo.ST | Combo.Simple;

            if (IsEnabled(Preset.PLD_BlockForWings) &&
                (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms)))
                return All.SavageBlade;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (PLD_ST_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.Simple, ref actionID))
                    return actionID;
            }
            
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;

            return STCombo;
        }
    }

    internal class PLD_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, TotalEclipse)) return actionID;

            const Combo comboFlags = Combo.AoE | Combo.Simple;

            if (IsEnabled(Preset.PLD_BlockForWings) &&
                (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms, 0.5f)))
                return All.SavageBlade;


            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (PLD_AoE_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.Simple, ref actionID))
                    return actionID;
            }
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;

            return AoECombo;
        }
    }

    #endregion

    #region Advanced Modes

    internal class PLD_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, FastBlade)) return actionID;

            const Combo comboFlags = Combo.ST | Combo.Adv;

            if (IsEnabled(Preset.PLD_BlockForWings) && (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms)))
                return All.SavageBlade;

            //Opener
            if (IsEnabled(Preset.PLD_ST_AdvancedMode_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (PLD_ST_Advanced_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.Advanced, ref actionID))
                    return actionID;
            }
            
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;

            return STCombo;
        }
    }

    internal class PLD_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, TotalEclipse)) return actionID;

            const Combo comboFlags = Combo.AoE | Combo.Adv;

            if (IsEnabled(Preset.PLD_BlockForWings) && (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms, 0.5f)))
                return All.SavageBlade;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (PLD_AoE_Advanced_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.Advanced, ref actionID))
                    return actionID;
            }
           
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;

            return AoECombo;
        }
    }

    #endregion

    #region Standalone Features

    internal class PLD_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (RageOfHalone or RoyalAuthority))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is FastBlade && LevelChecked(RiotBlade))
                    return RiotBlade;

                if (ComboAction is RiotBlade && LevelChecked(RageOfHalone))
                {
                    return HasDivineMight && HasDivineMagicMP && PLD_HolySpirit_Standalone
                        ? HolySpirit
                        : OriginalHook(RageOfHalone);
                }
            }

            return FastBlade;
        }
    }

    internal class PLD_AoE_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_AoE_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Prominence)
                return actionID;

            if (ComboAction is TotalEclipse && ComboTimer > 0 && LevelChecked(Prominence))
                return HasDivineMight && HasDivineMagicMP && PLD_HolyCircle_Standalone
                    ? HolyCircle
                    : OriginalHook(Prominence);

            return TotalEclipse;
        }
    }

    internal class PLD_Requiescat_Confiteor : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_Requiescat_Options;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Requiescat or Imperator))
                return actionID;

            // Fight or Flight
            if (PLD_Requiescat_SubOption == 1)
            {
                if (ActionReady(FightOrFlight) && ActionReady(OriginalHook(Requiescat)))
                    return FightOrFlight;

                if (PLD_Requiescat_SubOption_GoringBlade && HasStatusEffect(Buffs.GoringBladeReady) && InMeleeRange() &&
                    !HasStatusEffect(Buffs.Requiescat) && !ActionReady(OriginalHook(Requiescat)))
                    return GoringBlade;
            }

            // Confiteor & Blades
            if (HasStatusEffect(Buffs.ConfiteorReady) || LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)
                return OriginalHook(Confiteor);

            // Pre-Blades
            return HasStatusEffect(Buffs.Requiescat)
                // AoE
                ? LevelChecked(HolyCircle) && NumberOfEnemiesInRange(HolyCircle) > 2
                    ? HolyCircle
                    : HolySpirit
                : actionID;
        }
    }

    internal class PLD_CircleOfScorn : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_SpiritsWithin;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpiritsWithin or Expiacion))
                return actionID;

            if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                return OriginalHook(SpiritsWithin);

            if (ActionReady(CircleOfScorn) &&
                (!PLD_SpiritsWithin_SubOption || JustUsed(OriginalHook(SpiritsWithin), 5f)))
                return CircleOfScorn;

            return actionID;
        }
    }

    internal class PLD_ShieldLob_HolySpirit : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ShieldLob_Feature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ShieldLob)
                return actionID;
            
            IGameObject? target =
                //Mouseover Retarget
                (PLD_ShieldLob_Feature_FieldMO 
                    ? SimpleTarget.Stack.MouseOver.IfHostile().IfWithinRange(ShieldLob.ActionRange())
                    : null) ??
    
                (PLD_ShieldLob_Feature_SmartTargeting == 1
                    ? PLD_ShieldLob_Feature_SmartTargeting_NotTargetingPlayer
                        ? SimpleTarget.FurthestEnemyOver5YalmsAwayNotTargetingPlayer.IfWithinRange(ShieldLob.ActionRange())
                        : SimpleTarget.FurthestEnemyOver5YalmsAway.IfWithinRange(ShieldLob.ActionRange())
                    : null) ??

                (PLD_ShieldLob_Feature_SmartTargeting == 2 
                    ? PLD_ShieldLob_Feature_SmartTargeting_NotTargetingPlayer
                        ? SimpleTarget.NearestEnemyOver5YalmsAwayNotTargetingPlayer.IfWithinRange(ShieldLob.ActionRange())
                        : SimpleTarget.NearestEnemyOver5YalmsAway.IfWithinRange(ShieldLob.ActionRange())
                    : null);
            
            if (PLD_ShieldLob_Feature_HolySpirit && LevelChecked(HolySpirit) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && 
                (TimeMoving.Ticks == 0 || HasStatusEffect(Buffs.DivineMight)))
                return target != null 
                    ? HolySpirit.Retarget(ShieldLob, target)
                    : HolySpirit;
            
            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }

    internal class PLD_RetargetClemency : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetClemency;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Clemency)
                return actionID;

            int healthThreshold = PLD_RetargetClemency_Health;

            IGameObject? target =
                //Mouseover retarget option
                (IsEnabled(Preset.PLD_RetargetClemency_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Hard target
                SimpleTarget.HardTarget.IfFriendly() ??

                //Lowest HP option
                (IsEnabled(Preset.PLD_RetargetClemency_LowHP)
                 && PlayerHealthPercentageHp() > healthThreshold
                    ? SimpleTarget.LowestHPAlly.IfNotThePlayer().IfAlive()
                    : null);

            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }

    internal class PLD_RetargetSheltron : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetSheltron;

        protected override uint Invoke(uint action)
        {
            if (action is not (Sheltron or HolySheltron))
                return action;

            IGameObject? target =
                //Mouseover retarget option
                (IsEnabled(Preset.PLD_RetargetSheltron_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Hard target retarget
                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty() ??

                //Targets target retarget option
                (IsEnabled(Preset.PLD_RetargetSheltron_TT)
                 && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfNotThePlayer().IfInParty()
                    : null);

            // Intervention if trying to Buff an ally
            if (ActionReady(Intervention) &&
                target != null &&
                CanApplyStatus(target, Buffs.Intervention))
                return Intervention.Retarget([Sheltron, HolySheltron], target);

            return action;
        }
    }

    #region One-Button Mitigation

    internal class PLD_Mit_OneButton : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Bulwark)
                return actionID;

            if (IsEnabled(Preset.PLD_Mit_HallowedGround_Max) &&
                ActionReady(HallowedGround) &&
                PlayerHealthPercentageHp() <= PLD_Mit_HallowedGround_Max_Health &&
                ContentCheck.IsInConfiguredContent(
                    PLD_Mit_HallowedGround_Max_Difficulty,
                    PLD_Mit_HallowedGround_Max_DifficultyListSet
                ))
                return HallowedGround;

            foreach(int priority in PLD_Mit_Priorities.OrderBy(x => x))
            {
                int index = PLD_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }

    internal class PLD_Mit_OneButton_Party : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_Mit_Party;

        protected override uint Invoke(uint action)
        {
            if (action is not DivineVeil)
                return action;

            if (Role.CanReprisal())
                return Role.Reprisal;

            if (ActionReady(DivineVeil))
                return DivineVeil;

            if (ActionReady(PassageOfArms) &&
                IsEnabled(Preset.PLD_Mit_Party_Wings) &&
                !HasStatusEffect(Buffs.PassageOfArms, anyOwner: true))
                return PassageOfArms;

            return action;
        }
    }

    #endregion

    internal class PLD_RetargetShieldBash : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetShieldBash;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ShieldBash)
                return actionID;

            IGameObject? tar = SimpleTarget.StunnableEnemy(PLD_RetargetStunLockout ? PLD_RetargetShieldBash_Strength : 3);

            if (tar is not null)
                return ShieldBash.Retarget(actionID, tar);

            if (PLD_RetargetStunLockout)
                return All.SavageBlade;

            return actionID;
        }
    }

    internal class PLD_RetargetCover : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetCover;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Cover)
                return actionID;

            int healthThreshold = PLD_RetargetCover_Health;

            IGameObject? target =
                //Mouseover retarget option
                (IsEnabled(Preset.PLD_RetargetCover_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Hard target
                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty() ??

                //Lowest HP option
                (IsEnabled(Preset.PLD_RetargetCover_LowHP)
                 && SimpleTarget.LowestHPPAlly.HPP < healthThreshold
                    ? SimpleTarget.LowestHPPAlly.IfNotThePlayer().IfInParty()
                    : null);

            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }

    internal class PLD_RetargetIntervene : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetIntervene;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Intervene)
                return actionID;

            IGameObject? target =
                // Mouseover
                SimpleTarget.Stack.MouseOver.IfHostile()
                    .IfWithinRange(Intervene.ActionRange()) ??

                // Nearest Enemy to Mouseover
                SimpleTarget.NearestEnemyToTarget(SimpleTarget.Stack.MouseOver,
                    Intervene.ActionRange()) ??
                CurrentTarget.IfHostile().IfWithinRange(Intervene.ActionRange());

            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }

    #endregion
}
