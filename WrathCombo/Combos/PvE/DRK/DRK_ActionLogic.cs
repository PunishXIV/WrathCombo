#region

using System;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.DRK.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK
{
    /// <remarks>
    ///     Actions in this Provider:
    ///     <list type="bullet">
    ///         <item>
    ///             <term>Disesteem</term>
    ///         </item>
    ///         <item>
    ///             <term>Living Shadow</term>
    ///         </item>
    ///         <item>
    ///             <term>Interject</term>
    ///         </item>
    ///         <item>
    ///             <term>Low Blow</term>
    ///         </item>
    ///         <item>
    ///             <term>Delirium / Blood Weapon</term>
    ///         </item>
    ///         <item>
    ///             <term>Salted Earth</term>
    ///         </item>
    ///         <item>
    ///             <term>Salt and Darkness</term>
    ///         </item>
    ///         <item>
    ///             <term>Shadowbringer</term>
    ///         </item>
    ///         <item>
    ///             <term>Carve and Spit</term>
    ///             <description>(ST only)</description>
    ///         </item>
    ///         <item>
    ///             <term>Abyssal Drain</term>
    ///             <description>(AoE only)</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    private class Cooldown : IActionProvider
    {
        public static bool ShouldDeliriumNext;

        public bool TryGetAction(Combo flags, ref uint action, bool? disesteemOnly)
        {
            #region Disesteem

            disesteemOnly ??= false;

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Disesteem) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_CD_Disesteem)) &&
                ActionReady(Disesteem) &&
                TraitLevelChecked(Traits.EnhancedShadowIII) &&
                HasStatusEffect(Buffs.Scorn) &&
                ((Gauge.DarksideTimeRemaining > 0 &&
                  GetStatusEffectRemainingTime(Buffs.Scorn) < 24) ||
                 GetStatusEffectRemainingTime(Buffs.Scorn) < 14))
                return (action = OriginalHook(Disesteem)) != 0;

            #endregion

            if (!CanWeave || Gauge.DarksideTimeRemaining <= 1) return false;
            if (disesteemOnly == true) return false;

            if (HiddenFeaturesData.IsEnabledWith(
                    Preset.DRK_Hid_R6SHoldSquirrelBurst,
                    () => HiddenFeaturesData.Targeting.R6SSquirrel &&
                          CombatEngageDuration().TotalSeconds < 275))
                return false;

            #region Living Shadow

            #region Variables

            var shadowContentHPThreshold = flags.HasFlag(Combo.ST)
                ? DRK_ST_LivingShadowThresholdDifficulty
                : DRK_AoE_LivingShadowThresholdDifficulty;
            var shadowInHPContent =
                flags.HasFlag(Combo.Adv) && ContentCheck.IsInConfiguredContent(
                    shadowContentHPThreshold, ContentCheck.ListSet.Halved);
            var shadowHPThreshold = flags.HasFlag(Combo.ST)
                ? DRK_ST_LivingShadowThreshold
                : DRK_AoE_LivingShadowThreshold;
            var shadowHPMatchesThreshold =
                flags.HasFlag(Combo.Simple) || !shadowInHPContent ||
                (shadowInHPContent &&
                 GetTargetHPPercent(Target(flags)) > shadowHPThreshold);

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Shadow) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_CD_Shadow)) &&
                IsOffCooldown(LivingShadow) &&
                LevelChecked(LivingShadow) &&
                shadowHPMatchesThreshold)
                return (action = LivingShadow) != 0;

            #endregion

            if (CombatEngageDuration().TotalSeconds <= 5) return false;

            #region Interrupting

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Interrupt) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Interrupt)) &&
                HiddenFeaturesData.NonBlockingIsEnabledWith(
                    Preset.DRK_Hid_R7SCircleCastOnly,
                    () => HiddenFeaturesData.Content.InR7S,
                    () => HiddenFeaturesData.Targeting.R7SCircleCastingAdd) &&
                Role.CanInterject())
                return (action = Role.Interject) != 0;

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Stun) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Stun)) &&
                !TargetIsBoss() &&
                !JustUsed(Role.Interject) &&
                Role.CanLowBlow() &&
                HiddenFeaturesData.NonBlockingIsEnabledWith(
                    Preset.DRK_Hid_R6SStunJabberOnly,
                    () => HiddenFeaturesData.Content.InR6S,
                    () => HiddenFeaturesData.Targeting.R6SJabber) &&
                !InBossEncounter())
                return (action = Role.LowBlow) != 0;

            #endregion

            #region Delirium (/Blood Weapon)

            #region Variables

            var deliriumContentHPThreshold = flags.HasFlag(Combo.ST)
                ? DRK_ST_DeliriumThresholdDifficulty
                : DRK_AoE_DeliriumThresholdDifficulty;
            var deliriumInHPContent =
                flags.HasFlag(Combo.Adv) && ContentCheck.IsInConfiguredContent(
                    deliriumContentHPThreshold, ContentCheck.ListSet.Halved);
            var deliriumHPThreshold = flags.HasFlag(Combo.ST)
                ? DRK_ST_DeliriumThreshold
                : DRK_AoE_DeliriumThreshold;
            var deliriumHPMatchesThreshold =
                flags.HasFlag(Combo.Simple) || !deliriumInHPContent ||
                (deliriumInHPContent &&
                 GetTargetHPPercent(Target(flags)) > deliriumHPThreshold);

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Delirium) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_CD_Delirium)) &&
                deliriumHPMatchesThreshold &&
                LevelChecked(BloodWeapon) &&
                GetCooldownRemainingTime(BloodWeapon) < GCD * 1.5)
                ShouldDeliriumNext = true;

            if (ShouldDeliriumNext &&
                IsOffCooldown(BloodWeapon))
            {
                ShouldDeliriumNext = false;
                return (action = OriginalHook(Delirium)) != 0;
            }

            #endregion

            #region Salted Earth

            #region Variables

            var saltStill =
                flags.HasFlag(Combo.Simple) || flags.HasFlag(Combo.ST) ||
                (flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.AoE) &&
                 IsNotEnabled(Preset.DRK_AoE_CD_SaltStill)) ||
                (flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.AoE) &&
                 IsEnabled(Preset.DRK_AoE_CD_SaltStill) && !IsMoving() &&
                 CombatEngageDuration().TotalSeconds >= 7);
            var saltHPThreshold =
                flags.HasFlag(Combo.AoE)
                    ? flags.HasFlag(Combo.Adv)
                        ? DRK_AoE_SaltThreshold
                        : 30
                    : 0;

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Salt) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_CD_Salt)) &&
                LevelChecked(SaltedEarth) &&
                IsOffCooldown(SaltedEarth) &&
                !HasStatusEffect(Buffs.SaltedEarth) &&
                saltStill &&
                GetTargetHPPercent(Target(flags)) >= saltHPThreshold)
                return (action = SaltedEarth) != 0;

            #endregion

            #region Salt and Darkness

            if ((flags.HasFlag(Combo.Simple) ||
                 flags.HasFlag(Combo.AoE) ||
                 IsEnabled(Preset.DRK_ST_CD_Darkness)) &&
                LevelChecked(SaltAndDarkness) &&
                IsOffCooldown(SaltAndDarkness) &&
                HasStatusEffect(Buffs.SaltedEarth) &&
                GetStatusEffectRemainingTime(Buffs.SaltedEarth) < 7)
                return (action = OriginalHook(SaltAndDarkness)) != 0;

            #endregion

            #region Shadowbringer

            #region Variables

            var bringerInBurst =
                flags.HasFlag(Combo.Simple) || flags.HasFlag(Combo.AoE) ||
                (flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.ST) &&
                 !IsEnabled(Preset.DRK_ST_CD_BringerBurst)) ||
                (flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.ST) &&
                 IsEnabled(Preset.DRK_ST_CD_BringerBurst) &&
                 GetCooldownRemainingTime(LivingShadow) >= 90 &&
                 !HasStatusEffect(Buffs.Scorn));

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Bringer) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_CD_Bringer)) &&
                ActionReady(Shadowbringer) &&
                bringerInBurst)
                return (action = Shadowbringer) != 0;

            #endregion

            #region Carve and Spit (ST only)

            if (flags.HasFlag(Combo.ST) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_ST_CD_Spit)) &&
                ActionReady(CarveAndSpit) &&
                (int)LocalPlayer.CurrentMp <= 9400 &&
                (!LevelChecked(LivingShadow) ||
                 GetCooldownRemainingTime(LivingShadow) > 20))
                return (action = CarveAndSpit) != 0;

            #endregion

            #region Abyssal Drain (AoE only)

            #region Variables

            var drainHPThreshold = flags.HasFlag(Combo.Adv)
                ? DRK_AoE_DrainThreshold
                : 60;

            #endregion

            if (flags.HasFlag(Combo.AoE) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_AoE_CD_Drain)) &&
                ActionReady(AbyssalDrain) &&
                PlayerHealthPercentageHp() <= drainHPThreshold)
                return (action = AbyssalDrain) != 0;

            #endregion

            return false;
        }
    }

    /// <remarks>
    ///     Actions in this Provider:
    ///     <list type="bullet">
    ///         <item>
    ///             <term>Living Dead</term>
    ///         </item>
    ///         <item>
    ///             <term>TBN</term>
    ///         </item>
    ///         <item>
    ///             <term>Oblation</term>
    ///         </item>
    ///         <item>
    ///             <term>Reprisal</term>
    ///         </item>
    ///         <item>
    ///             <term>Dark Missionary</term>
    ///             <description>(ST only)</description>
    ///         </item>
    ///         <item>
    ///             <term>Rampart</term>
    ///             <description>(AoE only)</description>
    ///         </item>
    ///         <item>
    ///             <term>Arms Length</term>
    ///             <description>(AoE only)</description>
    ///         </item>
    ///         <item>
    ///             <term>Shadowed Vigil</term>
    ///         </item>
    ///     </list>
    /// </remarks>
    private class Mitigation : IActionProvider
    {
        public bool TryGetAction(Combo flags, ref uint action, bool? _)
        {
            // Bail if we're trying to Invuln or actively Invulnerable
            if (HasStatusEffect(Buffs.LivingDead) ||
                HasStatusEffect(Buffs.WalkingDead) ||
                HasStatusEffect(Buffs.UndeadRebirth))
                return false;

            // Bail if Simple mode and mitigation is disabled
            if (flags.HasFlag(Combo.Simple) &&
                ((flags.HasFlag(Combo.ST) &&
                  (int)DRK_ST_SimpleMitigation ==
                  (int)SimpleMitigation.Off) ||
                 (flags.HasFlag(Combo.AoE) &&
                  (int)DRK_AoE_SimpleMitigation ==
                  (int)SimpleMitigation.Off)))
                return false;

            #region Living Dead

            #region Variables

            var bossRestrictionLivingDead = flags.HasFlag(Combo.Adv)
                ? (int)DRK_ST_LivingDeadBossRestriction
                : (int)BossAvoidance.Off;
            var livingDeadSelfThreshold = flags.HasFlag(Combo.Adv) ?
                flags.HasFlag(Combo.ST)
                    ? DRK_ST_LivingDeadSelfThreshold
                    : DRK_AoE_LivingDeadSelfThreshold :
                flags.HasFlag(Combo.ST) ? 15 : 20;
            var livingDeadTargetThreshold = flags.HasFlag(Combo.Adv) ?
                flags.HasFlag(Combo.ST)
                    ? DRK_ST_LivingDeadTargetThreshold
                    : DRK_AoE_LivingDeadTargetThreshold :
                flags.HasFlag(Combo.ST) ? 1 : 15;

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Mit_LivingDead) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Mit_LivingDead)) &&
                ActionReady(LivingDead) &&
                PlayerHealthPercentageHp() <= livingDeadSelfThreshold &&
                GetTargetHPPercent(Target(flags)) >= livingDeadTargetThreshold &&
                // Checking if the target matches the boss avoidance option
                ((bossRestrictionLivingDead is (int)BossAvoidance.On &&
                  InBossEncounter()) ||
                 bossRestrictionLivingDead is (int)BossAvoidance.Off))
                return (action = LivingDead) != 0;

            #endregion

            // Bail if we can't weave any other mitigations
            if (!CanWeave) return false;
            // Bail if we just used mitigation
            if (JustUsedMitigation) return false;

            #region TBN

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Mit_TBN) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Mit_TBN)) &&
                ActionReady(BlackestNight) &&
                LocalPlayer.CurrentMp >= 3000 &&
                ShouldTBNSelf(flags.HasFlag(Combo.AoE), flags.HasFlag(Combo.Simple)))
                return (action = BlackestNight) != 0;

            #endregion

            #region Oblation

            #region Variables

            var oblationCharges = flags.HasFlag(Combo.Adv)
                ? flags.HasFlag(Combo.ST)
                    ? DRK_ST_OblationCharges
                    : DRK_AoE_OblationCharges
                : 0;
            var oblationThreshold = flags.HasFlag(Combo.Adv)
                ? flags.HasFlag(Combo.ST)
                    ? DRK_ST_Mit_OblationThreshold
                    : DRK_AoE_Mit_OblationThreshold
                : 90;

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Mit_Oblation) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Mit_Oblation)) &&
                ActionReady(Oblation) &&
                !HasStatusEffect(Buffs.Oblation, anyOwner: true) &&
                GetRemainingCharges(Oblation) > oblationCharges &&
                PlayerHealthPercentageHp() <= oblationThreshold)
                return (action = Oblation) != 0;

            #endregion

            #region Reprisal

            #region Variables

            var reprisalThreshold =
                flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.AoE)
                    ? DRK_AoE_Mit_ReprisalThreshold
                    : 100;
            var reprisalTargetCount =
                flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.AoE)
                    ? DRK_AoE_ReprisalEnemyCount
                    : 1;
            var reprisalUseForRaidwides =
                flags.HasFlag(Combo.AoE) || RaidWideCasting();

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Mit_Reprisal) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Mit_Reprisal)) &&
                reprisalUseForRaidwides &&
                Role.CanReprisal(reprisalThreshold, reprisalTargetCount,
                    !flags.HasFlag(Combo.AoE)) &&
                HiddenFeaturesData.IsEnabledWith(
                    Preset.DRK_Hid_R6SNoAutoGroupMits,
                    () => !HiddenFeaturesData.Content.InR6S))
                return (action = Role.Reprisal) != 0;

            #endregion

            #region Dark Missionary (ST only)

            #region Variables

            var missionaryThreshold =
                flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.ST)
                    ? DRK_ST_Mit_MissionaryThreshold
                    : 100;
            var missionaryAvoidanceSatisfied =
                flags.HasFlag(Combo.AoE) ||
                flags.HasFlag(Combo.Simple) ||
                IsNotEnabled(Preset.DRK_ST_Mit_MissionaryAvoid) ||
                !HasStatusEffect(Role.Debuffs.Reprisal, Target(flags), true);

            #endregion

            if (flags.HasFlag(Combo.ST) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_ST_Mit_Missionary)) &&
                ActionReady(DarkMissionary) &&
                RaidWideCasting() &&
                missionaryAvoidanceSatisfied &&
                PlayerHealthPercentageHp() <= missionaryThreshold &&
                HiddenFeaturesData.IsEnabledWith(
                    Preset.DRK_Hid_R6SNoAutoGroupMits,
                    () => !HiddenFeaturesData.Content.InR6S))
                return (action = DarkMissionary) != 0;

            #endregion

            #region Dark Mind (AoE only)

            #region Variables

            var darkMindThreshold =
                flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.AoE)
                    ? DRK_AoE_Mit_DarkMindThreshold
                    : 100;

            #endregion

            if (flags.HasFlag(Combo.AoE) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_AoE_Mit_DarkMind)) &&
                ActionReady(DarkMind) &&
                PlayerHealthPercentageHp() <= darkMindThreshold)
                return (action = DarkMind) != 0;

            #endregion

            #region Rampart (AoE only)

            #region Variables

            var rampartThreshold =
                flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.AoE)
                    ? DRK_AoE_Mit_RampartThreshold
                    : 100;

            #endregion

            if (flags.HasFlag(Combo.AoE) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_AoE_Mit_Rampart)) &&
                Role.CanRampart(rampartThreshold))
                return (action = Role.Rampart) != 0;

            #endregion

            #region Arms Length (AoE only)

            #region Variables

            var armsLengthEnemyCount = flags.HasFlag(Combo.Adv)
                ? DRK_AoE_ArmsLengthEnemyCount
                : 3;

            #endregion

            if (flags.HasFlag(Combo.AoE) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_AoE_Mit_ArmsLength)) &&
                Role.CanArmsLength(armsLengthEnemyCount))
                return (action = Role.ArmsLength) != 0;

            #endregion

            #region Shadowed Vigil

            #region Variables

            var vigilHealthThreshold = flags.HasFlag(Combo.Adv) ?
                flags.HasFlag(Combo.ST)
                    ? DRK_ST_ShadowedVigilThreshold
                    : DRK_AoE_ShadowedVigilThreshold :
                flags.HasFlag(Combo.ST) ? 40 : 50;

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Mit_Vigil) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Mit_Vigil)) &&
                ActionReady(ShadowedVigil) &&
                PlayerHealthPercentageHp() <= vigilHealthThreshold)
                return (action = OriginalHook(ShadowWall)) != 0;

            #endregion

            return false;
        }
    }

    /// <remarks>
    ///     Actions in this Provider:
    ///     <list type="bullet">
    ///         <item>
    ///             <term>Bloodspiller</term>
    ///         </item>
    ///         <item>
    ///             <term>Quietus</term>
    ///         </item>
    ///         <item>
    ///             <term>Scarlet Delirium</term>
    ///         </item>
    ///         <item>
    ///             <term>Comeuppance</term>
    ///         </item>
    ///         <item>
    ///             <term>Torcleaver</term>
    ///         </item>
    ///         <item>
    ///             <term>Edge of Darkness</term>
    ///         </item>
    ///         <item>
    ///             <term>Edge of Shadow</term>
    ///         </item>
    ///         <item>
    ///             <term>Flood of Darkness</term>
    ///         </item>
    ///         <item>
    ///             <term>Flood of Shadow</term>
    ///         </item>
    ///     </list>
    /// </remarks>
    private class Spender : IActionProvider
    {
        public bool TryGetAction(Combo flags, ref uint action, bool? specialManaOnly)
        {
            if (TryGetManaAction(flags, ref action, specialManaOnly)) return true;
            if (specialManaOnly == true) return false;
            if (TryGetBloodAction(flags, ref action)) return true;

            return false;
        }

        private bool TryGetBloodAction(Combo flags, ref uint action)
        {
            if (ComboTimer > 0 && ComboTimer < GCD * 2) return false;

            #region Variables and readiness bails

            var bloodGCDReady =
                LevelChecked(Bloodspiller) &&
                GetCooldownRemainingTime(Bloodspiller) < GCD / 2;

            if (!bloodGCDReady) return false;

            #endregion

            #region Delirium Chain

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_ScarletChain) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_ImpalementChain)) &&
                HasStatusEffect(Buffs.EnhancedDelirium) &&
                GetStatusEffectStacks(Buffs.EnhancedDelirium) > 0)
                if (flags.HasFlag(Combo.ST))
                    return (action = OriginalHook(Bloodspiller)) != 0;
                else if (flags.HasFlag(Combo.AoE))
                    return (action = OriginalHook(Quietus)) != 0;

            #endregion

            #region Blood Spending during Delirium (Lower Levels)

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_Bloodspiller) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_Quietus)) &&
                GetStatusEffectStacks(Buffs.Delirium) > 0)
                if (flags.HasFlag(Combo.ST))
                    return (action = OriginalHook(Bloodspiller)) != 0;
                else if (flags.HasFlag(Combo.AoE))
                    return (action = OriginalHook(Quietus)) != 0;

            #endregion

            #region Blood Spending prior to Delirium (ST only)

            if (flags.HasFlag(Combo.ST) &&
                (flags.HasFlag(Combo.Simple) ||
                 IsEnabled(Preset.DRK_ST_CD_Delirium)) &&
                LevelChecked(Delirium) &&
                Gauge.Blood >= 70 &&
                Cooldown.ShouldDeliriumNext)
                return (action = Bloodspiller) != 0;

            #endregion

            if (HasStatusEffect(Buffs.Scorn)) return false;

            #region Blood Spending after Delirium Chain

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_Bloodspiller) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_Quietus)) &&
                Gauge.Blood >= 50 &&
                (GetCooldownRemainingTime(Delirium) > 37 || IsBursting))
                if (flags.HasFlag(Combo.ST))
                    return (action = Bloodspiller) != 0;
                else if (flags.HasFlag(Combo.AoE) && LevelChecked(Quietus))
                    return (action = Quietus) != 0;

            #endregion

            #region Blood Overcap

            #region Variables

            var overcapThreshold = flags.HasFlag(Combo.Adv)
                ? flags.HasFlag(Combo.ST)
                    ? DRK_ST_BloodOvercapThreshold
                    : DRK_AoE_BloodOvercapThreshold
                : 90;

            var beforeSouleater =
                flags.HasFlag(Combo.AoE) || ComboAction == SyphonStrike;

            #endregion

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_BloodOvercap) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_BloodOvercap)) &&
                Gauge.Blood >= overcapThreshold &&
                beforeSouleater)
                if (flags.HasFlag(Combo.ST))
                    return (action = Bloodspiller) != 0;
                else if (flags.HasFlag(Combo.AoE) && LevelChecked(Quietus))
                    return (action = Quietus) != 0;

            #endregion

            return false;
        }

        private bool TryGetManaAction(Combo flags, ref uint action, bool? onlyMaint)
        {
            // Bail if we can't weave anything else
            if (!CanWeave) return false;

            #region Variables and some Mana bails

            // Bail if it is too early into the fight
            if (CombatEngageDuration().TotalSeconds <= 5) return false;
            // Bail if mana spending is not available yet
            if (!LevelChecked(FloodOfDarkness)) return false;

            var mana = (int)LocalPlayer.CurrentMp;
            var manaPooling =
                ContentCheck.IsInConfiguredContent(
                    DRK_ST_ManaSpenderPoolingDifficulty,
                    DRK_ST_ManaSpenderPoolingDifficultyListSet);
            var manaPool = flags.HasFlag(Combo.Adv)
                ? flags.HasFlag(Combo.ST)
                    ? manaPooling ? (int)DRK_ST_ManaSpenderPooling : 0
                    : (int)DRK_AoE_ManaSpenderPooling
                : 0;

            // Set the pool to save a tbn in simple, if mitigation is enabled
            if (flags.HasFlag(Combo.Simple) &&
                ((flags.HasFlag(Combo.ST) &&
                  (int)DRK_ST_SimpleMitigation ==
                  (int)SimpleMitigation.On) ||
                 (flags.HasFlag(Combo.AoE) &&
                  (int)DRK_AoE_SimpleMitigation ==
                  (int)SimpleMitigation.On)))
                manaPool = 3000;

            var hasEnoughMana = mana >= (manaPool + 3000) || Gauge.HasDarkArts;
            var secondsBeforeBurst =
                flags.HasFlag(Combo.Adv) && flags.HasFlag(Combo.ST)
                    ? DRK_ST_BurstSoonThreshold
                    : 20;
            var evenBurstSoon =
                IsOnCooldown(LivingShadow) &&
                GetCooldownRemainingTime(LivingShadow) < secondsBeforeBurst;
            var darksideDropping = Gauge.DarksideTimeRemaining / 1000 < 10;

            // Bail if we don't have enough mana
            if (!hasEnoughMana) return false;

            #endregion

            #region Darkside Maintenance

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_EdgeDarkside) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_Flood)) &&
                darksideDropping)
                if (flags.HasFlag(Combo.ST) && LevelChecked(EdgeOfDarkness))
                    return (action = OriginalHook(EdgeOfDarkness)) != 0;
                else
                    return (action = OriginalHook(FloodOfDarkness)) != 0;

            #endregion

            // Bail if it is right before burst
            if (GetCooldownRemainingTime(LivingShadow) <
                Math.Min(6, secondsBeforeBurst) &&
                LevelChecked(LivingShadow) &&
                CombatEngageDuration().TotalSeconds > 20)
                return false;

            #region Mana Overcap

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_ManaOvercap) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_ManaOvercap)) &&
                mana >= 9400 &&
                !evenBurstSoon)
                if (flags.HasFlag(Combo.ST) && LevelChecked(EdgeOfDarkness))
                    return (action = OriginalHook(EdgeOfDarkness)) != 0;
                else
                    return (action = OriginalHook(FloodOfDarkness)) != 0;

            #endregion

            if (onlyMaint == true) return false;

            #region Burst Phase Spending

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_Edge) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_Flood)) &&
                IsBursting)
                if (flags.HasFlag(Combo.ST) && LevelChecked(EdgeOfDarkness))
                    return (action = OriginalHook(EdgeOfDarkness)) != 0;
                else
                    return (action = OriginalHook(FloodOfDarkness)) != 0;

            #endregion

            // Bail if it is too early into the fight
            if (CombatEngageDuration().TotalSeconds <= 10) return false;

            #region Mana Dark Arts Drop Prevention

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_Sp_DarkArts) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Sp_Flood)) &&
                Gauge.HasDarkArts && HasOwnTBN)
                if (flags.HasFlag(Combo.ST) && LevelChecked(EdgeOfDarkness))
                    return (action = OriginalHook(EdgeOfDarkness)) != 0;
                else
                    return (action = OriginalHook(FloodOfDarkness)) != 0;

            #endregion

            return false;
        }
    }

    /// <remarks>
    ///     Will almost always return <c>true</c>.<br />
    ///     Actions in this Provider:
    ///     <list type="bullet">
    ///         <item>
    ///             <term>Hard Slash</term>
    ///         </item>
    ///         <item>
    ///             <term>Syphon Strike</term>
    ///         </item>
    ///         <item>
    ///             <term>Souleater</term>
    ///         </item>
    ///         <item>
    ///             <term>Unleash</term>
    ///         </item>
    ///         <item>
    ///             <term>Stalwart Soul</term>
    ///         </item>
    ///     </list>
    /// </remarks>
    private class Core : IActionProvider
    {
        public bool TryGetAction(Combo flags, ref uint action, bool? _)
        {
            var comboRunning = ComboTimer > 0;
            var lastComboAction = ComboAction;

            #region Single-Target 1-2-3 Combo

            if (flags.HasFlag(Combo.ST))
                if (!comboRunning)
                    return (action = HardSlash) != 0;
                else if (lastComboAction == HardSlash &&
                         LevelChecked(SyphonStrike))
                    return (action = SyphonStrike) != 0;
                else if (lastComboAction == SyphonStrike &&
                         LevelChecked(Souleater))
                    return (action = Souleater) != 0;

            #endregion

            #region AoE 1-2 Combo

            if (flags.HasFlag(Combo.AoE))
                if (!comboRunning)
                    return (action = Unleash) != 0;
                else if (lastComboAction == Unleash &&
                         LevelChecked(StalwartSoul))
                    return (action = StalwartSoul) != 0;

            #endregion

            return false;
        }
    }

    #region JustUsedMit

    private static bool InSavagePlus => ContentCheck.IsInSavagePlusContent;

    /// <summary>
    ///     Whether mitigation was very recently used, depending on the duration and
    ///     strength of the mitigation.
    /// </summary>
    private static bool JustUsedMitigation =>
        JustUsed(BlackestNight, (InSavagePlus ? 3f : 4f)) ||
        JustUsed(Oblation, (InSavagePlus ? 6f : 4f)) ||
        JustUsed(DarkMind, (InSavagePlus ? 6f : 4f)) ||
        JustUsed(Role.Reprisal, (InSavagePlus ? 1f : 4f)) ||
        JustUsed(DarkMissionary, (InSavagePlus ? 0f : 5f)) ||
        JustUsed(Role.Rampart, 6f) ||
        JustUsed(Role.ArmsLength, (InSavagePlus ? 0f : 4f)) ||
        JustUsed(ShadowedVigil, (InSavagePlus ? 11f : 6f)) ||
        JustUsed(LivingDead, (InSavagePlus ? 13f : 7f));

    #endregion

    #region TBN

    /// <summary>
    ///     Whether the player has a shield from TBN from themselves.
    /// </summary>
    /// <seealso cref="Buffs.BlackestNightShield" />
    private static bool HasOwnTBN
    {
        get
        {
            var has = false;
            if (LocalPlayer is not null)
                has = HasStatusEffect(Buffs.BlackestNightShield);

            return has;
        }
    }

    /// <summary>
    ///     Whether the player has a shield from TBN from anyone.
    /// </summary>
    /// <seealso cref="Buffs.BlackestNightShield" />
    private static bool HasAnyTBN
    {
        get
        {
            var has = false;
            if (LocalPlayer is not null)
                has = HasStatusEffect(Buffs.BlackestNightShield, anyOwner: true);

            return has;
        }
    }

    /// <summary>
    ///     Decides if the player should use TBN on themselves,
    ///     based on general rules and the player's configuration.
    /// </summary>
    /// <param name="aoe">Whether AoE or ST options should be checked.</param>
    /// <param name="simple">Whether Simple mode options should be checked.</param>
    /// <returns>Whether TBN should be used on self.</returns>
    /// <seealso cref="BlackestNight" />
    /// <seealso cref="Buffs.BlackestNightShield" />
    /// <seealso cref="Preset.DRK_ST_Mit_TBN" />
    /// <seealso cref="Config.DRK_ST_TBNThreshold" />
    /// <seealso cref="Config.DRK_ST_TBNBossRestriction" />
    /// <seealso cref="Preset.DRK_AoE_Mit_TBN" />
    private static bool ShouldTBNSelf(bool aoe = false, bool simple = false)
    {
        // Bail if we're dead or unloaded
        if (LocalPlayer is null)
            return false;

        // Bail if we're at the status limit
        if (!CanApplyStatus(LocalPlayer, Buffs.BlackestNightShield))
            return false;

        // Bail if TBN is disabled
        if ((!aoe &&
             (simple &&
              (int)DRK_ST_SimpleMitigation !=
              (int)SimpleMitigation.On) ||
             (!simple &&
              (!IsEnabled(Preset.DRK_ST_Mitigation) ||
               !IsEnabled(Preset.DRK_ST_Mit_TBN)))) ||
            (aoe &&
             (simple &&
              (int)DRK_AoE_SimpleMitigation !=
              (int)SimpleMitigation.On) ||
             (!simple &&
              (!IsEnabled(Preset.DRK_AoE_Mitigation) ||
               !IsEnabled(Preset.DRK_AoE_Mit_TBN)))))
            return false;

        // Bail if we already have TBN
        if (HasOwnTBN)
            return false;

        // Bail if we have no target
        if (!HasBattleTarget())
            return false;

        var hpRemaining = PlayerHealthPercentageHp();
        var hpThreshold = !aoe ? (float)DRK_ST_TBNThreshold : 90f;

        // Bail if we're above the threshold
        if (hpRemaining > hpThreshold)
            return false;

        var bossRestriction = !aoe
            ? (int)DRK_ST_TBNBossRestriction
            : (int)BossAvoidance.Off; // Don't avoid bosses in AoE

        // Bail if we're trying to avoid bosses and we're in a boss fight
        if (bossRestriction is (int)BossAvoidance.On
            && InBossEncounter())
            return false;

        // Bail if we have a TBN and burst is >30s away ()
        if (GetCooldownRemainingTime(LivingShadow) > 30
            && HasAnyTBN)
            return false;

        return true;
    }

    #endregion

    #region One-Button Mitigation

    /// <summary>
    ///     The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///     The order of the list needs to match the order in
    ///     <see cref="Preset" />.
    /// </summary>
    /// <value>
    ///     <c>Action</c> is the action to use.<br />
    ///     <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///     <c>Logic</c> is the logic for whether to use the action.
    /// </value>
    /// <remarks>
    ///     Each logic check is already combined with checking if the preset
    ///     <see cref="IsEnabled">is enabled</see>
    ///     and if the action is <see cref="ActionReady(uint,bool,bool)">ready</see> and
    ///     <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///     Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, Preset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        (BlackestNight, Preset.DRK_Mit_TheBlackestNight,
            () => !HasAnyTBN && LocalPlayer.CurrentMp > 3000 &&
                  PlayerHealthPercentageHp() <= DRK_Mit_TBN_Health),
        (Oblation, Preset.DRK_Mit_Oblation,
            () => !((TargetIsFriendly() &&
                     HasStatusEffect(Buffs.Oblation, CurrentTarget, true)) ||
                    (!TargetIsFriendly() &&
                     HasStatusEffect(Buffs.Oblation, anyOwner: true))) &&
                  GetRemainingCharges(Oblation) > DRK_Mit_Oblation_Charges),
        (Role.Reprisal, Preset.DRK_Mit_Reprisal,
            () => Role.CanReprisal(checkTargetForDebuff: false)),
        (DarkMissionary, Preset.DRK_Mit_DarkMissionary,
            () => DRK_Mit_DarkMissionary_PartyRequirement ==
                (int)PartyRequirement.No || IsInParty()),
        (Role.Rampart, Preset.DRK_Mit_Rampart,
            () => Role.CanRampart(DRK_Mit_Rampart_Health)),
        (DarkMind, Preset.DRK_Mit_DarkMind, () => true),
        (Role.ArmsLength, Preset.DRK_Mit_ArmsLength,
            () => Role.CanArmsLength(DRK_Mit_ArmsLength_EnemyCount,
                DRK_Mit_ArmsLength_Boss)),
        (OriginalHook(ShadowWall), Preset.DRK_Mit_ShadowWall,
            () => PlayerHealthPercentageHp() <= DRK_Mit_ShadowWall_Health),
    ];

    /// <summary>
    ///     Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///     checks if the mitigation is ready and meets the provided requirements.
    /// </summary>
    /// <param name="index">
    ///     The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///     which is the order of the mitigation in <see cref="Preset" />.
    /// </param>
    /// <param name="action">
    ///     The variable to set to the action to, if the mitigation is set to be
    ///     used.
    /// </param>
    /// <returns>
    ///     Whether the mitigation is ready, enabled, and passes the provided logic
    ///     check.
    /// </returns>
    private static bool CheckMitigationConfigMeetsRequirements
        (int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion

    #region TryGet Setup

    /// <summary>
    ///     Flags to combine to provide to the `TryGet...Action` methods.
    /// </summary>
    [Flags]
    private enum Combo
    {
        // Target-type for combo
        ST = 1 << 0, // 1
        AoE = 1 << 1, // 2

        // Complexity of combo
        Adv = 1 << 2, // 4
        Simple = 1 << 3, // 8
        Basic = 1 << 4, // 16
    }

    private interface IActionProvider
    {
        bool TryGetAction(Combo flags, ref uint action, bool? extraParam = null);
    }

    /// <summary>
    ///     Checks whether a given preset is enabled, and the flags match it.
    /// </summary>
    private static bool IsSTEnabled(Combo flags, Preset preset) =>
        flags.HasFlag(Combo.ST) && IsEnabled(preset);

    /// <summary>
    ///     Checks whether a given preset is enabled, and the flags match it.
    /// </summary>
    private static bool IsAoEEnabled(Combo flags, Preset preset) =>
        flags.HasFlag(Combo.AoE) && IsEnabled(preset);

    /// <summary>
    ///     Signature for the TryGetAction&lt;ActionType&gt; methods.
    /// </summary>
    /// <param name="flags">
    ///     The flags to describe the combo executing this method.
    /// </param>
    /// <param name="action">The action to execute.</param>
    /// <param name="extraParam">Any extra parameter to pass through.</param>
    /// <returns>Whether the <c>action</c> was changed.</returns>
    /// <seealso cref="IActionProvider.TryGetAction" />
    /// <seealso cref="Mitigation" />
    /// <seealso cref="Spender" />
    /// <seealso cref="Cooldown" />
    /// <seealso cref="Core" />
    private static bool TryGetAction<T>(Combo flags, ref uint action,
        bool? extraParam = null)
        where T : IActionProvider, new() =>
        new T().TryGetAction(flags, ref action, extraParam);

    #endregion
}