#region

using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK
{
    internal class DRK_ST_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_ST_Adv;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not HardSlash) return actionID;

            const Combo comboFlags = Combo.ST | Combo.Adv;
            var newAction = HardSlash;

            // Unmend Option
            if (IsEnabled(CustomComboPreset.DRK_ST_RangedUptime)
                && LevelChecked(Unmend)
                && !InMeleeRange()
                && HasBattleTarget())
                return Unmend;

            // Opener
            if (IsEnabled(CustomComboPreset.DRK_ST_BalanceOpener)
                && Opener().FullOpener(ref actionID))
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

            // Bail if not in combat
            if (!InCombat()) return HardSlash;

            if (TryGetAction<Variant>(comboFlags, ref newAction))
                return newAction;

            var cdBossRequirement =
                (int)Config.DRK_ST_CDsBossRequirement ==
                (int)Config.BossRequirement.On;
            if (IsEnabled(CustomComboPreset.DRK_ST_CDs) &&
                ((cdBossRequirement && InBossEncounter()) ||
                 !cdBossRequirement) &&
                TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            var inMitigationContent =
                ContentCheck.IsInConfiguredContent(
                    Config.DRK_ST_MitDifficulty,
                    Config.DRK_ST_MitDifficultyListSet
                );
            if (IsEnabled(CustomComboPreset.DRK_ST_Mitigation) &&
                inMitigationContent &&
                TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(CustomComboPreset.DRK_ST_Spenders) &&
                TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return HardSlash;
        }
    }

    internal class DRK_ST_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not HardSlash) return actionID;

            const Combo comboFlags = Combo.ST | Combo.Simple;
            var newAction = HardSlash;

            // Unmend Option
            if (ActionReady(Unmend) &&
                !InMeleeRange() &&
                HasBattleTarget())
                return Unmend;

            // Bail if not in combat
            if (!InCombat()) return HardSlash;

            if (TryGetAction<Variant>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return HardSlash;
        }
    }

    internal class DRK_AoE_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_AoE_Adv;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not Unleash) return actionID;

            const Combo comboFlags = Combo.AoE | Combo.Adv;
            var newAction = Unleash;

            // Bail if not in combat
            if (!InCombat()) return Unleash;

            if (TryGetAction<Variant>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(CustomComboPreset.DRK_AoE_CDs) &&
                TryGetAction<Cooldown>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(CustomComboPreset.DRK_AoE_Mitigation) &&
                TryGetAction<Mitigation>(comboFlags, ref newAction))
                return newAction;

            if (IsEnabled(CustomComboPreset.DRK_AoE_Spenders) &&
                TryGetAction<Spender>(comboFlags, ref newAction))
                return newAction;

            if (TryGetAction<Core>(comboFlags, ref newAction))
                return newAction;

            return Unleash;
        }
    }

    internal class DRK_AoE_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            // Bail if not looking at the replaced action
            if (actionID is not Unleash) return actionID;

            const Combo comboFlags = Combo.AoE | Combo.Simple;
            var newAction = Unleash;

            // Bail if not in combat
            if (!InCombat()) return Unleash;

            if (TryGetAction<Variant>(comboFlags, ref newAction))
                return newAction;

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
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_oGCD;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (CarveAndSpit or AbyssalDrain)) return actionID;

            if (IsEnabled(CustomComboPreset.DRK_oGCD_Interrupt) &&
                ActionReady(All.Interject) &&
                CanInterruptEnemy())
                return All.Interject;

            if (IsEnabled(CustomComboPreset.DRK_oGCD_Delirium) &&
                ActionReady(BloodWeapon))
                return OriginalHook(Delirium);

            if (IsEnabled(CustomComboPreset.DRK_oGCD_Shadow) &&
                IsOffCooldown(LivingShadow) &&
                LevelChecked(LivingShadow))
                return LivingShadow;

            if (IsEnabled(CustomComboPreset.DRK_oGCD_Disesteem) &&
                IsOffCooldown(Disesteem) &&
                LevelChecked(Disesteem))
                return Disesteem;

            if (IsEnabled(CustomComboPreset.DRK_oGCD_SaltedEarth) &&
                IsOffCooldown(SaltedEarth) &&
                LevelChecked(SaltedEarth) &&
                !HasEffect(Buffs.SaltedEarth))
                return SaltedEarth;

            if (IsOffCooldown(CarveAndSpit) &&
                LevelChecked(AbyssalDrain))
                return actionID;

            if (IsEnabled(CustomComboPreset.DRK_oGCD_SaltAndDarkness) &&
                IsOffCooldown(SaltAndDarkness) &&
                LevelChecked(SaltAndDarkness) &&
                HasEffect(Buffs.SaltedEarth))
                return SaltAndDarkness;

            if (IsEnabled(CustomComboPreset.DRK_oGCD_Shadowbringer) &&
                ActionReady(Shadowbringer))
                return Shadowbringer;

            return actionID;
        }
    }

    #region One-Button Mitigation

    internal class DRK_Mit_OneButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DarkMind) return actionID;

            if (IsEnabled(CustomComboPreset.DRK_Mit_LivingDead_Max) &&
                ActionReady(LivingDead) &&
                PlayerHealthPercentageHp() <= Config.DRK_Mit_LivingDead_Health &&
                ContentCheck.IsInConfiguredContent(
                    Config.DRK_Mit_EmergencyLivingDead_Difficulty,
                    Config.DRK_Mit_EmergencyLivingDead_DifficultyListSet
                ))
                return LivingDead;

            foreach (var priority in Config.DRK_Mit_Priorities.Items.OrderBy(x => x))
            {
                var index = Config.DRK_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out var action))
                    return action;
            }

            return actionID;
        }
    }

    #endregion

    #endregion
}
