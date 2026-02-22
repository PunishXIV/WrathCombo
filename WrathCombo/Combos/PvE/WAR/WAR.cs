using Dalamud.Game.ClientState.Objects.Types;
using System;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.WAR.Config;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    #region Simple Combos
    internal class WAR_ST_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != HeavySwing)
                return actionID;
            
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.ST | Combo.Simple;
            
            if (WAR_ST_MitsOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.simple, ref actionID))
                    return actionID == Holmgang && IsEnabled(Preset.WAR_RetargetHolmgang)
                        ? actionID.Retarget(HeavySwing, SimpleTarget.Self)
                        : actionID;
            }
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            return STCombo;
        }
    }
    
    internal class WAR_AoE_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Overpower)
                return actionID;
           
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.AoE | Combo.Simple;
            
            if (WAR_AoE_MitsOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.simple, ref actionID))
                    return actionID == Holmgang && IsEnabled(Preset.WAR_RetargetHolmgang)
                        ? actionID.Retarget(Overpower, SimpleTarget.Self)
                        : actionID;
            }

            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            return AoECombo;
        }
    }
    #endregion
    
    #region Advanced Combos
    internal class WAR_ST_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ST_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not HeavySwing)
                return actionID;
            
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.ST | Combo.Adv;
            
            if (WAR_ST_Advanced_MitsOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.advanced, ref actionID))
                    return actionID == Holmgang && IsEnabled(Preset.WAR_RetargetHolmgang)
                        ? actionID.Retarget(HeavySwing, SimpleTarget.Self)
                        : actionID;
            }
            
            if (IsEnabled(Preset.WAR_ST_BalanceOpener) && Opener().FullOpener(ref actionID))
                return actionID;
            
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            return STCombo;
        }
    }
    
    internal class WAR_AoE_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_AoE_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Overpower)
                return actionID; //Our button
            
            // Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.AoE | Combo.Simple;
            
            if (WAR_AoE_Advanced_MitsOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.advanced, ref actionID))
                    return actionID == Holmgang && IsEnabled(Preset.WAR_RetargetHolmgang)
                        ? actionID.Retarget(Overpower, SimpleTarget.Self)
                        : actionID;
            }
            
            if (TryOGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID))
                return actionID;
            
            return AoECombo;
        }
    }
    #endregion

    #region Standalones

    #region One-Button Mitigation

    internal class WAR_Mit_OneButton : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_Mit_OneButton;

        protected override uint Invoke(uint action)
        {
            if (action != ThrillOfBattle)
                return action;

            if (IsEnabled(Preset.WAR_Mit_Holmgang_Max) &&
                ActionReady(Holmgang) &&
                PlayerHealthPercentageHp() <= WAR_Mit_Holmgang_Health &&
                ContentCheck.IsInConfiguredContent(WAR_Mit_Holmgang_Max_Difficulty, WAR_Mit_Holmgang_Max_DifficultyListSet))
                return Holmgang;

            foreach(int priority in WAR_Mit_Priorities.OrderBy(x => x))
            {
                int index = WAR_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint actionID))
                    return actionID;
            }
            return action;
        }
    }
    #endregion

    #region Fell Cleave Features
    internal class WAR_FC_Features : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_FC_Features;

        protected override uint Invoke(uint action)
        {
            if (action is not (InnerBeast or FellCleave))
                return action;
            if (IsEnabled(Preset.WAR_FC_InnerRelease) && ActionReady(OriginalHook(Berserk)) && CanWeave() && !HasWrathful && Minimal && GetTargetHPPercent() >= WAR_FC_IRStop && (HasSurgingTempest || !LevelChecked(StormsEye)))
                return OriginalHook(Berserk);
            if (IsEnabled(Preset.WAR_FC_Infuriate) && ActionReady(Infuriate) && CanWeave() && !HasNascentChaos && Minimal && !JustUsed(Infuriate) && !HasIR.Stacks && BeastGauge <= WAR_FC_Infuriate_Gauge && GetRemainingCharges(Infuriate) > WAR_FC_Infuriate_Charges)
                return Infuriate;
            if (IsEnabled(Preset.WAR_FC_Upheaval) && ActionReady(Upheaval) && CanWeave() && HasSurgingTempest && InMeleeRange() && Minimal)
                return Upheaval;
            if (IsEnabled(Preset.WAR_FC_PrimalWrath) && LevelChecked(PrimalWrath) && CanWeave() && HasWrathful && HasSurgingTempest && Minimal && GetTargetDistance() <= 4.99f)
                return PrimalWrath;
            if (IsEnabled(Preset.WAR_FC_Onslaught) && (!IsEnabled(Preset.WAR_FC_InnerRelease) || (IsEnabled(Preset.WAR_FC_InnerRelease) && IR.Cooldown > 40)) &&
                ActionReady(Onslaught) && GetRemainingCharges(Onslaught) > WAR_FC_Onslaught_Charges && GetTargetDistance() <= WAR_FC_Onslaught_Distance && 
                WAR_FC_Onslaught_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_FC_Onslaught_TimeStill) && CanWeave() && HasSurgingTempest)
                return Onslaught;
            if (IsEnabled(Preset.WAR_FC_PrimalRend) && HasStatusEffect(Buffs.PrimalRendReady) && HasSurgingTempest &&
                GetTargetDistance() <= WAR_FC_PrimalRend_Distance && 
                WAR_FC_PrimalRend_Movement == 1 || (WAR_FC_PrimalRend_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_FC_PrimalRend_TimeStill) && 
                (WAR_FC_PrimalRend_EarlyLate == 0 || (WAR_FC_PrimalRend_EarlyLate == 1 && (GetStatusEffectRemainingTime(Buffs.PrimalRendReady) <= 15 || (!HasIR.Stacks && !HasBF.Stacks && !HasWrathful))))))
                return PrimalRend;
            if (IsEnabled(Preset.WAR_FC_PrimalRuination) && LevelChecked(PrimalRuination) && HasSurgingTempest && Minimal && HasStatusEffect(Buffs.PrimalRuinationReady))
                return PrimalRuination;
            return action;
        }
    }
    #endregion

    #region Storm's Eye -> Storm's Path
    internal class WAR_EyePath : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_EyePath;
        protected override uint Invoke(uint action) => action != StormsPath ? action : GetStatusEffectRemainingTime(Buffs.SurgingTempest) <= WAR_EyePath_Refresh ? StormsEye : action;
    }
    #endregion

    #region Primal Combo -> Inner Release
    internal class WAR_PrimalCombo_InnerRelease : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_PrimalCombo_InnerRelease;

        protected override uint Invoke(uint action) => action is not (Berserk or InnerRelease) ? OriginalHook(action) :
            LevelChecked(PrimalRend) && HasStatusEffect(Buffs.PrimalRendReady) ? PrimalRend :
            LevelChecked(PrimalRuination) && HasStatusEffect(Buffs.PrimalRuinationReady) ? PrimalRuination : OriginalHook(action);
    }
    #endregion

    #region Infuriate -> Fell Cleave / Decimate
    internal class WAR_InfuriateFellCleave : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_InfuriateFellCleave;

        protected override uint Invoke(uint action) => action is not (InnerBeast or FellCleave or SteelCyclone or Decimate) ? action :
            (InCombat() && BeastGauge <= WAR_Infuriate_Range && GetRemainingCharges(Infuriate) > WAR_Infuriate_Charges && ActionReady(Infuriate) &&
             !HasNascentChaos && (!HasIR.Stacks || IsNotEnabled(Preset.WAR_InfuriateFellCleave_IRFirst))) ? OriginalHook(Infuriate) : action;
    }
    #endregion
    
    #region Nascent Flash -> Raw Intuition
    internal class WAR_NascentFlash : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_NascentFlash;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != NascentFlash)
                return actionID;
                    
            if (!LevelChecked(NascentFlash)) 
                return OriginalHook(RawIntuition);
            
            IGameObject? target =
                //Mouseover Retarget
                (IsEnabled(Preset.WAR_NascentFlash_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??
                //Hard Target
                SimpleTarget.HardTarget.IfInParty().IfNotThePlayer() ??
                //Target's Target Retarget
                (IsEnabled(Preset.WAR_NascentFlash_TT) && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfInParty().IfNotThePlayer()
                    : null);

            // Nascent if trying to heal an ally
            if (ActionReady(NascentFlash) &&
                target != null &&
                CanApplyStatus(target, Buffs.NascentFlashTarget))
                return NascentFlash.Retarget(NascentFlash, target);
            
            return actionID;
        }
    }
    #endregion

    #region Raw Intuition -> Nascent Flash
    internal class WAR_RawIntuition_Targeting : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_RawIntuition_Targeting;

        protected override uint Invoke(uint action)
        {
            if (action is not (RawIntuition or Bloodwhetting))
                return action;

            IGameObject? target =
                //Mouseover Retarget
                (IsEnabled(Preset.WAR_RawIntuition_Targeting_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??
                //Hard Target
                SimpleTarget.HardTarget.IfInParty().IfNotThePlayer() ??
                //Target's Target Retarget
                (IsEnabled(Preset.WAR_RawIntuition_Targeting_TT) && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfInParty().IfNotThePlayer()
                    : null);

            // Nascent if trying to heal an ally
            if (ActionReady(NascentFlash) &&
                target != null &&
                CanApplyStatus(target, Buffs.NascentFlashTarget))
                return NascentFlash.Retarget([RawIntuition, Bloodwhetting], target);

            return action;
        }
    }
    #endregion

    #region Thrill of Battle -> Equilibrium
    internal class WAR_ThrillEquilibrium : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ThrillEquilibrium;
        protected override uint Invoke(uint action) => action != Equilibrium ? action : ActionReady(ThrillOfBattle) ? ThrillOfBattle : action;
    }
    #endregion

    #region Reprisal -> Shake It Off
    internal class WAR_Mit_Party : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_Mit_Party;
        protected override uint Invoke(uint action) => action != ShakeItOff ? action : Role.CanReprisal() ? Role.Reprisal : action;
    }
    #endregion
    
    #region Double Knockback Resist Protection
    internal class WAR_ArmsLengthLockout : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ArmsLengthLockout;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.ArmsLength)
                return actionID;

            return InBossEncounter() && 
                   (GetPossessedStatusRemainingTime(Buffs.InnerStrength) > WAR_ArmsLengthLockout_Time || 
                    JustUsed(InnerRelease))
                ? All.SavageBlade
                : actionID;
        }
    }
    #endregion
    
    #region Onslaught Retargeting
    internal class WAR_RetargetOnslaught : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_RetargetOnslaught;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Onslaught)
                return actionID;
            
            IGameObject? target =
                // Mouseover
                SimpleTarget.Stack.MouseOver.IfHostile()
                    .IfWithinRange(Onslaught.ActionRange()) ??

                // Nearest Enemy to Mouseover
                SimpleTarget.NearestEnemyToTarget(SimpleTarget.Stack.MouseOver,
                    Onslaught.ActionRange()) ??
    
                CurrentTarget.IfHostile().IfWithinRange(Onslaught.ActionRange());
            
            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }
    #endregion
    
    #region Holmgang Retargeting
    internal class WAR_RetargetHolmgang : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_RetargetHolmgang;

        protected override uint Invoke(uint actionID) => actionID != Holmgang ? actionID : actionID.Retarget(SimpleTarget.Self);
    }
    #endregion

    #region Basic Combos
    internal class WAR_ST_StormsPathCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ST_StormsPathCombo;

        protected override uint Invoke(uint id) => (id != StormsPath) ? id :
            (ComboTimer > 0 && ComboAction == HeavySwing && LevelChecked(Maim)) ? Maim :
            (ComboTimer > 0 && ComboAction == Maim && LevelChecked(StormsPath)) ? StormsPath :
            HeavySwing;
    }

    internal class WAR_ST_StormsEyeCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ST_StormsEyeCombo;

        protected override uint Invoke(uint id) => (id != StormsEye) ? id :
            (ComboTimer > 0 && ComboAction == HeavySwing && LevelChecked(Maim)) ? Maim :
            (ComboTimer > 0 && ComboAction == Maim && LevelChecked(StormsEye)) ? StormsEye :
            HeavySwing;
    }
    
    internal class WAR_AoE_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_AoE_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MythrilTempest)
                return actionID;
            
            if (ComboAction is Overpower && ComboTimer > 0 && LevelChecked(MythrilTempest))
                return MythrilTempest;

            return Overpower;
        }
    }
    #endregion
    #endregion
}
