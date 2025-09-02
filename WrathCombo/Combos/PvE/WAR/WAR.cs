using System;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.WAR.Config;

namespace WrathCombo.Combos.PvE;

internal partial class WAR : Tank
{
    #region Simple Mode - Single Target
    internal class WAR_ST_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ST_Simple;

        protected override uint Invoke(uint action)
        {
            if (action != HeavySwing)
                return action;
            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Stuns
            if (Role.CanInterject())
                return Role.Interject;
            if (!TargetIsBoss()
                && Role.CanLowBlow()
                && !JustUsed(Role.Interject)
                && !InBossEncounter())
                return Role.LowBlow;
            #endregion

            #region Mitigations
            if (WAR_ST_MitsOptions == 0 && InCombat() && !MitUsed)
            {
                if (ActionReady(Holmgang) && PlayerHealthPercentageHp() < 30)
                    return Holmgang;
                if (IsPlayerTargeted())
                {
                    if (ActionReady(OriginalHook(Vengeance)) && PlayerHealthPercentageHp() < 60)
                        return OriginalHook(Vengeance);
                    if (Role.CanRampart(80))
                        return Role.Rampart;
                    if (Role.CanReprisal(90))
                        return Role.Reprisal;
                }
                if (ActionReady(ThrillOfBattle) && PlayerHealthPercentageHp() < 70)
                    return ThrillOfBattle;
                if (ActionReady(Equilibrium) && PlayerHealthPercentageHp() < 50)
                    return Equilibrium;
                if (ActionReady(OriginalHook(RawIntuition)) && PlayerHealthPercentageHp() < 90)
                    return OriginalHook(Bloodwhetting);
            }
            #endregion

            #region Rotation
            if (ShouldUseTomahawk)
                return Tomahawk;
            if (ShouldUseInnerRelease())
                return OriginalHook(Berserk);
            if (ShouldUseInfuriate())
                return Infuriate;
            if (ShouldUseUpheaval)
                return Upheaval;
            if (ShouldUsePrimalWrath)
                return PrimalWrath;
            if (ShouldUseOnslaught(1, 3.5f, !IsMoving()))
                return Onslaught;
            if (ShouldUsePrimalRend(3.5f, !IsMoving()))
                return PrimalRend;
            if (ShouldUsePrimalRuination)
                return PrimalRuination;
            if (ShouldUseFellCleave())
                return OriginalHook(InnerBeast);
            return STCombo;
            #endregion
        }
    }
    #endregion

    #region Advanced Mode - Single Target
    internal class WAR_ST_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_ST_Advanced;

        protected override uint Invoke(uint action)
        {
            if (action != HeavySwing)
                return action;
            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Stuns
            if (IsEnabled(Preset.WAR_ST_Interrupt)
                && HiddenFeaturesData.NonBlockingIsEnabledWith( // Only interrupt circle adds in 7
                    Preset.WAR_Hid_R7SCircleCastOnly,
                    () => HiddenFeaturesData.Content.InR7S,
                    () => HiddenFeaturesData.Targeting.R7SCircleCastingAdd)
                && Role.CanInterject())
                return Role.Interject;
            if (IsEnabled(Preset.WAR_ST_Stun)
                && !TargetIsBoss()
                && Role.CanLowBlow()
                && !JustUsed(Role.Interject)
                && !InBossEncounter())
                return Role.LowBlow;
            #endregion

            #region Mitigations
            if (IsEnabled(Preset.WAR_ST_Mitigation) && InCombat() && !MitUsed)
            {
                if (IsEnabled(Preset.WAR_ST_Holmgang) && ActionReady(Holmgang) &&
                    PlayerHealthPercentageHp() <= WAR_ST_Holmgang_Health &&
                    (WAR_ST_Holmgang_SubOption == 0 || (TargetIsBoss() && WAR_ST_Holmgang_SubOption == 1)))
                    return Holmgang;
                if (IsPlayerTargeted())
                {
                    if (IsEnabled(Preset.WAR_ST_Vengeance) && ActionReady(OriginalHook(Vengeance)) &&
                        PlayerHealthPercentageHp() <= WAR_ST_Vengeance_Health &&
                        (WAR_ST_Vengeance_SubOption == 0 || (TargetIsBoss() && WAR_ST_Vengeance_SubOption == 1)))
                        return OriginalHook(Vengeance);
                    if (IsEnabled(Preset.WAR_ST_Rampart) &&
                        Role.CanRampart(WAR_ST_Rampart_Health) &&
                        (WAR_ST_Rampart_SubOption == 0 || (TargetIsBoss() && WAR_ST_Rampart_SubOption == 1)))
                        return Role.Rampart;
                    if (IsEnabled(Preset.WAR_ST_Reprisal) &&
                        Role.CanReprisal(WAR_ST_Reprisal_Health) &&
                        (WAR_ST_Reprisal_SubOption == 0 || (TargetIsBoss() && WAR_ST_Reprisal_SubOption == 1)))
                        return Role.Reprisal;
                    if (IsEnabled(Preset.WAR_ST_ArmsLength) && Role.CanArmsLength() &&
                        PlayerHealthPercentageHp() <= WAR_ST_ArmsLength_Health)
                        return Role.ArmsLength;
                }
                if (IsEnabled(Preset.WAR_ST_Thrill) && ActionReady(ThrillOfBattle) &&
                    PlayerHealthPercentageHp() <= WAR_ST_Thrill_Health &&
                    (WAR_ST_Thrill_SubOption == 0 || (TargetIsBoss() && WAR_ST_Thrill_SubOption == 1)))
                    return ThrillOfBattle;
                if (IsEnabled(Preset.WAR_ST_Equilibrium) && ActionReady(Equilibrium) &&
                    PlayerHealthPercentageHp() <= WAR_ST_Equilibrium_Health &&
                    (WAR_ST_Equilibrium_SubOption == 0 || (TargetIsBoss() && WAR_ST_Equilibrium_SubOption == 1)))
                    return Equilibrium;
                if (IsEnabled(Preset.WAR_ST_Bloodwhetting) && ActionReady(OriginalHook(RawIntuition)) &&
                    PlayerHealthPercentageHp() <= WAR_AoE_Bloodwhetting_Health &&
                    (WAR_AoE_Bloodwhetting_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Bloodwhetting_SubOption == 1)))
                    return OriginalHook(RawIntuition);
            }

            #endregion

            #region Rotation
            if (IsEnabled(Preset.WAR_ST_BalanceOpener) && Opener().FullOpener(ref action))
                return action;
            if (IsEnabled(Preset.WAR_ST_RangedUptime) && ShouldUseTomahawk)
                return CanPRend(WAR_ST_PrimalRend_Distance, WAR_ST_PrimalRend_Movement == 1 || (WAR_ST_PrimalRend_Movement == 0 && !IsMoving())) ? PrimalRend : CanWeave() && CanOnslaught(WAR_ST_Onslaught_Charges, WAR_ST_Onslaught_Distance, WAR_ST_Onslaught_Movement == 1 || (WAR_ST_Onslaught_Movement == 0 && !IsMoving())) ? Onslaught : Tomahawk;
            if (IsEnabled(Preset.WAR_ST_InnerRelease) && ShouldUseInnerRelease(WAR_ST_IRStop))
                return OriginalHook(Berserk);
            if (IsEnabled(Preset.WAR_ST_Infuriate) && ShouldUseInfuriate(WAR_ST_Infuriate_Gauge, WAR_ST_Infuriate_Charges))
                return Infuriate;
            if (IsEnabled(Preset.WAR_ST_Upheaval) && ShouldUseUpheaval)
                return Upheaval;
            if (IsEnabled(Preset.WAR_ST_PrimalWrath) && ShouldUsePrimalWrath)
                return PrimalWrath;
            if (IsEnabled(Preset.WAR_ST_Onslaught) && (!IsEnabled(Preset.WAR_ST_InnerRelease) || (IsEnabled(Preset.WAR_ST_InnerRelease) && IR.Cooldown > 40)) &&
                ShouldUseOnslaught(WAR_ST_Onslaught_Charges, WAR_ST_Onslaught_Distance, WAR_ST_Onslaught_Movement == 1 || (WAR_ST_Onslaught_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_ST_Onslaught_TimeStill))))
                return Onslaught;
            if (IsEnabled(Preset.WAR_ST_PrimalRend) &&
                ShouldUsePrimalRend(WAR_ST_PrimalRend_Distance, (WAR_ST_PrimalRend_Movement == 1 || (WAR_ST_PrimalRend_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_ST_PrimalRend_TimeStill)))) &&
                (WAR_ST_PrimalRend_EarlyLate == 0 || (WAR_ST_PrimalRend_EarlyLate == 1 && (GetStatusEffectRemainingTime(Buffs.PrimalRendReady) <= 15 || (!HasIR.Stacks && !HasBF.Stacks && !HasWrath)))))
                return PrimalRend;
            if (IsEnabled(Preset.WAR_ST_PrimalRuination) && ShouldUsePrimalRuination)
                return PrimalRuination;
            if (IsEnabled(Preset.WAR_ST_FellCleave) && ShouldUseFellCleave(WAR_ST_FellCleave_Gauge))
                return OriginalHook(InnerBeast);
            return STCombo;
            #endregion
        }
    }

    #endregion

    #region Simple Mode - AoE
    internal class WAR_AoE_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_AoE_Simple;

        protected override uint Invoke(uint action)
        {
            if (action != Overpower)
                return action;
            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            if (Role.CanInterject())
                return Role.Interject;
            if (Role.CanLowBlow()
                && !JustUsed(Role.Interject))
                return Role.LowBlow;

            #region Mitigations
            if (WAR_AoE_MitsOptions != 1)
            {
                if (InCombat() && !MitUsed)
                {
                    if (ActionReady(Holmgang) && PlayerHealthPercentageHp() < 30)
                        return Holmgang;
                    if (IsPlayerTargeted())
                    {
                        if (ActionReady(OriginalHook(Vengeance)) && PlayerHealthPercentageHp() < 60)
                            return OriginalHook(Vengeance);
                        if (Role.CanRampart(80))
                            return Role.Rampart;
                        if (Role.CanReprisal(90, checkTargetForDebuff: false))
                            return Role.Reprisal;
                    }
                    if (ActionReady(ThrillOfBattle) && PlayerHealthPercentageHp() < 70)
                        return ThrillOfBattle;
                    if (ActionReady(Equilibrium) && PlayerHealthPercentageHp() < 50)
                        return Equilibrium;
                    if (ActionReady(OriginalHook(RawIntuition)) && PlayerHealthPercentageHp() < 90)
                        return OriginalHook(Bloodwhetting);
                }
            }
            #endregion

            #region Rotation
            if (ShouldUseInfuriate())
                return Infuriate;
            if (ShouldUseInnerRelease())
                return OriginalHook(Berserk);
            if (ShouldUseUpheaval)
                return LevelChecked(Orogeny) ? Orogeny : Upheaval;
            if (ShouldUsePrimalWrath)
                return PrimalWrath;
            if (ShouldUseOnslaught(1, 3.5f, !IsMoving()))
                return Onslaught;
            if (ShouldUsePrimalRend(3.5f, !IsMoving()))
                return PrimalRend;
            if (ShouldUsePrimalRuination)
                return PrimalRuination;
            if (ShouldUseDecimate())
                return OriginalHook(Decimate);
            return AOECombo;
            #endregion
        }
    }
    #endregion

    #region Advanced Mode - AoE
    internal class WAR_AoE_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_AoE_Advanced;

        protected override uint Invoke(uint action)
        {
            if (action != Overpower)
                return action; //Our button
            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            // If the Burst Holding for the Squirrels in 6 is enabled, check that
            // we are either not targeting a squirrel or the fight is after 275s
            var r6SReady = !HiddenFeaturesData.IsEnabledWith(
                Preset.WAR_Hid_R6SHoldSquirrelBurst,
                () => HiddenFeaturesData.Targeting.R6SSquirrel &&
                      CombatEngageDuration().TotalSeconds < 275);

            if (IsEnabled(Preset.WAR_AoE_Interrupt) && Role.CanInterject())
                return Role.Interject;
            if (IsEnabled(Preset.WAR_AoE_Stun) && !JustUsed(Role.Interject) && Role.CanLowBlow() && HiddenFeaturesData.NonBlockingIsEnabledWith( // Only stun the jabber, if in 6
                    Preset.WAR_Hid_R6SStunJabberOnly,
                    () => HiddenFeaturesData.Content.InR6S,
                    () => HiddenFeaturesData.Targeting.R6SJabber))
                return Role.LowBlow;

            #region Mitigations
            if (IsEnabled(Preset.WAR_AoE_Mitigation) && InCombat() && !MitUsed)
            {
                if (IsEnabled(Preset.WAR_AoE_Holmgang) && ActionReady(Holmgang) && PlayerHealthPercentageHp() <= WAR_AoE_Holmgang_Health &&
                    (WAR_AoE_Holmgang_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Holmgang_SubOption == 1)))
                    return Holmgang;
                if (IsPlayerTargeted())
                {
                    if (IsEnabled(Preset.WAR_AoE_Vengeance) && ActionReady(OriginalHook(Vengeance)) && PlayerHealthPercentageHp() <= WAR_AoE_Vengeance_Health &&
                        (WAR_AoE_Vengeance_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Vengeance_SubOption == 1)))
                        return OriginalHook(Vengeance);
                    if (IsEnabled(Preset.WAR_AoE_Rampart) && Role.CanRampart(WAR_AoE_Rampart_Health) &&
                        (WAR_AoE_Rampart_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Rampart_SubOption == 1)))
                        return Role.Rampart;
                    if (IsEnabled(Preset.WAR_AoE_Reprisal) && Role.CanReprisal(WAR_AoE_Reprisal_Health, checkTargetForDebuff: false) &&
                        HiddenFeaturesData.IsEnabledWith( // Skip mit if in 6
                                Preset.WAR_Hid_R6SNoAutoGroupMits,
                                () => !HiddenFeaturesData.Content.InR6S) &&
                        (WAR_AoE_Reprisal_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Reprisal_SubOption == 1)))
                        return Role.Reprisal;
                    if (IsEnabled(Preset.WAR_AoE_ArmsLength) && PlayerHealthPercentageHp() <= WAR_AoE_ArmsLength_Health && Role.CanArmsLength())
                        return Role.ArmsLength;
                }
                if (IsEnabled(Preset.WAR_AoE_Thrill) && ActionReady(ThrillOfBattle) && PlayerHealthPercentageHp() <= WAR_AoE_Thrill_Health &&
                    HiddenFeaturesData.IsEnabledWith( // Skip mit if in 6
                        Preset.WAR_Hid_R6SNoAutoGroupMits,
                        () => !HiddenFeaturesData.Content.InR6S) &&
                    (WAR_AoE_Thrill_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Thrill_SubOption == 1)))
                    return ThrillOfBattle;
                if (IsEnabled(Preset.WAR_AoE_Equilibrium) && ActionReady(Equilibrium) && PlayerHealthPercentageHp() <= WAR_AoE_Equilibrium_Health &&
                    (WAR_AoE_Equilibrium_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Equilibrium_SubOption == 1)))
                    return Equilibrium;
                if (IsEnabled(Preset.WAR_AoE_Bloodwhetting) && ActionReady(OriginalHook(RawIntuition)) && PlayerHealthPercentageHp() <= WAR_AoE_Bloodwhetting_Health &&
                    (WAR_AoE_Bloodwhetting_SubOption == 0 || (TargetIsBoss() && WAR_AoE_Bloodwhetting_SubOption == 1)))
                    return OriginalHook(RawIntuition);
            }
            #endregion

            if (!r6SReady) return AOECombo;

            #region Rotation
            if (IsEnabled(Preset.WAR_AoE_RangedUptime) && ShouldUseTomahawk)
                return CanPRend(WAR_AoE_PrimalRend_Distance, WAR_AoE_PrimalRend_Movement == 1 || (WAR_AoE_PrimalRend_Movement == 0 && !IsMoving())) ? PrimalRend : CanWeave() && CanOnslaught(WAR_AoE_Onslaught_Charges, WAR_AoE_Onslaught_Distance, WAR_AoE_Onslaught_Movement == 1 || (WAR_AoE_Onslaught_Movement == 0 && !IsMoving())) ? Onslaught : Tomahawk;
            if (IsEnabled(Preset.WAR_AoE_InnerRelease) && ShouldUseInnerRelease(WAR_AoE_IRStop))
                return OriginalHook(Berserk);
            if (IsEnabled(Preset.WAR_AoE_Infuriate) && ShouldUseInfuriate(WAR_AoE_Infuriate_Gauge, WAR_AoE_Infuriate_Charges))
                return Infuriate;
            if (IsEnabled(Preset.WAR_AoE_Orogeny) && ShouldUseUpheaval)
                return LevelChecked(Orogeny) ? Orogeny : Upheaval;
            if (IsEnabled(Preset.WAR_AoE_PrimalWrath) && ShouldUsePrimalWrath)
                return PrimalWrath;
            if (IsEnabled(Preset.WAR_AoE_Onslaught) && (!IsEnabled(Preset.WAR_AoE_InnerRelease) || (IsEnabled(Preset.WAR_AoE_InnerRelease) && IR.Cooldown > 40)) &&
                ShouldUseOnslaught(WAR_AoE_Onslaught_Charges, WAR_AoE_Onslaught_Distance, WAR_AoE_Onslaught_Movement == 1 || (WAR_AoE_Onslaught_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_AoE_Onslaught_TimeStill))))
                return Onslaught;
            if (IsEnabled(Preset.WAR_AoE_PrimalRend) && ShouldUsePrimalRend(WAR_AoE_PrimalRend_Distance, WAR_AoE_PrimalRend_Movement == 1 || (WAR_AoE_PrimalRend_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_AoE_PrimalRend_TimeStill))) &&
                (WAR_AoE_PrimalRend_EarlyLate == 0 || (WAR_AoE_PrimalRend_EarlyLate == 1 && (GetStatusEffectRemainingTime(Buffs.PrimalRendReady) <= 15 || (!HasIR.Stacks && !HasBF.Stacks && !HasWrath)))))
                return PrimalRend;
            if (IsEnabled(Preset.WAR_AoE_PrimalRuination) && ShouldUsePrimalRuination)
                return PrimalRuination;
            if (IsEnabled(Preset.WAR_AoE_Decimate) && ShouldUseDecimate(WAR_AoE_Decimate_Gauge))
                return OriginalHook(Decimate);
            return AOECombo;
            #endregion
        }
    }
    #endregion

    #region One-Button Mitigation
    internal class WAR_Mit_OneButton : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_Mit_OneButton;

        protected override uint Invoke(uint action)
        {
            if (action != ThrillOfBattle)
                return action;
            if (IsEnabled(Preset.WAR_Mit_Holmgang_Max) && ActionReady(Holmgang) &&
                PlayerHealthPercentageHp() <= WAR_Mit_Holmgang_Health &&
                ContentCheck.IsInConfiguredContent(WAR_Mit_Holmgang_Difficulty, WAR_Mit_Holmgang_DifficultyListSet))
                return Holmgang;
            foreach(int priority in WAR_Mit_Priorities.Items.OrderBy(x => x))
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
            if (IsEnabled(Preset.WAR_FC_InnerRelease) && ShouldUseInnerRelease(WAR_FC_IRStop))
                return OriginalHook(Berserk);
            if (IsEnabled(Preset.WAR_FC_Infuriate) && ShouldUseInfuriate(WAR_FC_Infuriate_Gauge, WAR_FC_Infuriate_Charges))
                return Infuriate;
            if (IsEnabled(Preset.WAR_FC_Upheaval) && ShouldUseUpheaval)
                return Upheaval;
            if (IsEnabled(Preset.WAR_FC_PrimalWrath) && ShouldUsePrimalWrath)
                return PrimalWrath;
            if (IsEnabled(Preset.WAR_FC_Onslaught) && (!IsEnabled(Preset.WAR_FC_InnerRelease) || (IsEnabled(Preset.WAR_FC_InnerRelease) && IR.Cooldown > 40)) &&
                ShouldUseOnslaught(WAR_FC_Onslaught_Charges, WAR_FC_Onslaught_Distance, WAR_FC_Onslaught_Movement == 1 || (WAR_FC_Onslaught_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_FC_Onslaught_TimeStill))))
                return Onslaught;
            if (IsEnabled(Preset.WAR_FC_PrimalRend) &&
                ShouldUsePrimalRend(WAR_FC_PrimalRend_Distance, WAR_FC_PrimalRend_Movement == 1 || (WAR_FC_PrimalRend_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(WAR_FC_PrimalRend_TimeStill))) &&
                (WAR_FC_PrimalRend_EarlyLate == 0 || (WAR_FC_PrimalRend_EarlyLate == 1 && (GetStatusEffectRemainingTime(Buffs.PrimalRendReady) <= 15 || (!HasIR.Stacks && !HasBF.Stacks && !HasWrath)))))
                return PrimalRend;
            if (IsEnabled(Preset.WAR_FC_PrimalRuination) && ShouldUsePrimalRuination)
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
            !HasNC && (!HasIR.Stacks || IsNotEnabled(Preset.WAR_InfuriateFellCleave_IRFirst))) ? OriginalHook(Infuriate) : action;
    }
    #endregion

    #region Nascent Flash -> Raw Intuition
    internal class WAR_NascentFlash : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_NascentFlash;
        protected override uint Invoke(uint actionID) => actionID != NascentFlash ? actionID : LevelChecked(NascentFlash) ? NascentFlash : RawIntuition;
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
            
            var target =
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
                return NascentFlash.Retarget([RawIntuition , Bloodwhetting], target);

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
        protected override uint Invoke(uint action) => action != ShakeItOff ? action : ActionReady(Role.Reprisal) ? Role.Reprisal : action;
    }
    #endregion

    #region MyRegion
    internal class WAR_RetargetHolmgang : CustomCombo
    {
        protected internal override Preset Preset => Preset.WAR_RetargetHolmgang;

        protected override uint Invoke(uint actionID) => actionID != Holmgang ? actionID : actionID.Retarget(SimpleTarget.Self, dontCull: true);
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
    #endregion
}
