#region

using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.DRK.Config;

// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK : Tank
{
    internal class DRK_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Souleater)
                return actionID;

            const Combo comboFlags = Combo.ST | Combo.Basic;
            var newAction = HardSlash;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return HardSlash;
        }
    }
    
    internal class DRK_ST_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_ST_Adv;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not HardSlash) return actionID;

            const Combo comboFlags = Combo.ST | Combo.Adv;
            var newAction = HardSlash;
            _ = IsBursting;

            // Unmend Option for Pulling
            var skipBecauseOpener =
                IsEnabled(Preset.DRK_ST_BalanceOpener) &&
                Opener().HasCooldowns() &&
                NumberOfObjectsInRange<SelfCircle>(20) < 2; // don't skip if add-pulling
            if (IsEnabled(Preset.DRK_ST_RangedUptime) &&
                ActionReady(Unmend) &&
                !InMeleeRange() &&
                HasBattleTarget() &&
                !skipBecauseOpener)
                return Unmend;

            // Opener
            if (IsEnabled(Preset.DRK_ST_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
            {
                handleEdgeCasts(Opener().CurrentOpenerAction, ref actionID,
                [
                    ScarletDelirium,
                    Comeuppance,
                    Torcleaver,
                    Bloodspiller,
                ]);
                return actionID;
            }

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            // Bail if not in combat
            if (!InCombat())
            {
                if (TryGetAction<Core>(comboFlags, ref newAction))
                    return newAction;
                return HardSlash;
            }

            // Unmend Option for Uptime
            if (IsEnabled(Preset.DRK_ST_RangedUptime) &&
                ActionReady(Unmend) &&
                !InMeleeRange() &&
                HasBattleTarget())
                return Unmend;

            var inMitigationContent =
                ContentCheck.IsInConfiguredContent(
                    DRK_ST_MitDifficulty,
                    DRK_ST_MitDifficultyListSet
                );

            if (IsEnabled(Preset.DRK_ST_Mitigation) &&
                inMitigationContent &&
                TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            var specialManaOnly = true;
            if (IsEnabled(Preset.DRK_ST_Spenders) &&
                TryGetAction<Spender>(comboFlags, ref newAction, specialManaOnly))
                return newAction;

            var cdBossRequirement =
                (int)DRK_ST_CDsBossRequirement ==
                (int)BossRequirement.On;
            var cdBossRequirementMet = !cdBossRequirement ||
                                       (cdBossRequirement && InBossEncounter());
            if (IsEnabled(Preset.DRK_ST_CDs) &&
                cdBossRequirementMet &&
                TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(Preset.DRK_ST_Spenders) &&
                TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return HardSlash;
        }
    }

    internal class DRK_ST_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not HardSlash) return actionID;

            const Combo comboFlags = Combo.ST | Combo.Simple;
            var newAction = HardSlash;
            _ = IsBursting;

            // Unmend Option
            if (ActionReady(Unmend) &&
                !InMeleeRange() &&
                HasBattleTarget())
                return Unmend;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            // Bail if not in combat
            if (!InCombat())
            {
                if (TryGetAction<Core>(comboFlags, ref newAction))
                    return newAction;
                return HardSlash;
            }

            if (TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Cooldown>(comboFlags, ref newAction, true))
                return newAction;

            if (TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return HardSlash;
        }
    }

    internal class DRK_AoE_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_AoE_Adv;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not Unleash) return actionID;

            const Combo comboFlags = Combo.AoE | Combo.Adv;
            var newAction = Unleash;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            // Bail if not in combat
            if (!InCombat())
            {
                if (TryGetAction<Core>(comboFlags, ref newAction))
                    return newAction;
                return Unleash;
            }

            if (IsEnabled(Preset.DRK_AoE_CDs) &&
                TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(Preset.DRK_AoE_Mitigation) &&
                TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(Preset.DRK_AoE_Spenders) &&
                TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return Unleash;
        }
    }

    internal class DRK_AoE_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not Unleash) return actionID;

            const Combo comboFlags = Combo.AoE | Combo.Simple;
            var newAction = Unleash;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            // Bail if not in combat
            if (!InCombat())
            {
                if (TryGetAction<Core>(comboFlags, ref newAction))
                    return newAction;
                return Unleash;
            }

            if (TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return Unleash;
        }
    }

    #region Multi-Button Combos

    internal class DRK_oGCDs : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_oGCD;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (CarveAndSpit or AbyssalDrain)) return actionID;

            if (IsEnabled(Preset.DRK_oGCD_Interrupt) &&
                Role.CanInterject())
                return Role.Interject;

            if (IsEnabled(Preset.DRK_oGCD_Delirium) &&
                ActionReady(BloodWeapon))
                return OriginalHook(Delirium);

            if (IsEnabled(Preset.DRK_oGCD_Shadow) &&
                IsOffCooldown(LivingShadow) &&
                LevelChecked(LivingShadow))
                return LivingShadow;

            if (IsEnabled(Preset.DRK_oGCD_Disesteem) &&
                IsOffCooldown(Disesteem) &&
                LevelChecked(Disesteem))
                return Disesteem;

            if (IsEnabled(Preset.DRK_oGCD_SaltedEarth) &&
                IsOffCooldown(SaltedEarth) &&
                LevelChecked(SaltedEarth) &&
                !HasStatusEffect(Buffs.SaltedEarth))
                return SaltedEarth;

            if (IsOffCooldown(CarveAndSpit) &&
                LevelChecked(AbyssalDrain))
                return actionID;

            if (IsEnabled(Preset.DRK_oGCD_SaltAndDarkness) &&
                IsOffCooldown(SaltAndDarkness) &&
                LevelChecked(SaltAndDarkness) &&
                HasStatusEffect(Buffs.SaltedEarth))
                return SaltAndDarkness;

            if (IsEnabled(Preset.DRK_oGCD_Shadowbringer) &&
                ActionReady(Shadowbringer))
                return Shadowbringer;

            return actionID;
        }
    }

    #region One-Button Mitigation

    internal class DRK_Mit_OneButton : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DarkMind) return actionID;

            if (IsEnabled(Preset.DRK_Mit_LivingDead_Max) &&
                ActionReady(LivingDead) &&
                PlayerHealthPercentageHp() <= DRK_Mit_LivingDead_Health &&
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_EmergencyLivingDead_Difficulty,
                    DRK_Mit_EmergencyLivingDead_DifficultyListSet
                ))
                return LivingDead;

            foreach (var priority in DRK_Mit_Priorities.Items.OrderBy(x => x))
            {
                var index = DRK_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out var action))
                    return action;
            }

            return actionID;
        }
    }

    internal class DRK_Mit_OneButton_Party : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_Mit_Party;

        protected override uint Invoke(uint action) =>
            action is not DarkMissionary
                ? action
                : ActionReady(Role.Reprisal) ? Role.Reprisal : action;
    }

    #endregion

    #region Standalones

    internal class DRK_RetargetTBN : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_Retarget_TBN;

        protected override uint Invoke(uint actionID) {
            if (actionID is not BlackestNight) return actionID;

            var target =
                SimpleTarget.UIMouseOverTarget.IfInParty() ??
                SimpleTarget.HardTarget.IfInParty() ??
                (IsEnabled(Preset.DRK_Retarget_TBN_TT) && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfInParty().IfNotThePlayer()
                    : null);

            if (target is not null &&
                CanApplyStatus(target, Buffs.BlackestNightShield))
                return actionID.Retarget(target, dontCull: true);

            return actionID;
        }
    }

    internal class DRK_RetargetOblation : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRK_Retarget_Oblation;

        protected override uint Invoke(uint actionID) {
            if (actionID is not Oblation) return actionID;

            var target =
                SimpleTarget.UIMouseOverTarget.IfInParty() ??
                SimpleTarget.HardTarget.IfInParty() ??
                (IsEnabled(Preset.DRK_Retarget_Oblation_TT) && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfInParty().IfNotThePlayer()
                    : null);

            var checkTarget = target ?? SimpleTarget.Self;
            if (IsEnabled(Preset.DRK_Retarget_Oblation_DoubleProtection) &&
                (HasStatusEffect(Buffs.Oblation, checkTarget, anyOwner: true) ||
                 JustUsedOn(Oblation, checkTarget)) &&
                CanApplyStatus(checkTarget, Buffs.Oblation))
                return All.SavageBlade;

            if (target is not null &&
                CanApplyStatus(target, Buffs.Oblation))
                return actionID.Retarget(target, dontCull: true);

            return actionID;
        }
    }

    #endregion

    #endregion
}
