#region

using System;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.DRK.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
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
                Role.CanInterject())
                return (action = Role.Interject) != 0;

            if ((flags.HasFlag(Combo.Simple) ||
                 IsSTEnabled(flags, Preset.DRK_ST_CD_Stun) ||
                 IsAoEEnabled(flags, Preset.DRK_AoE_Stun)) &&
                Role.CanLowBlow())
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
            #region Variables

            var preset = flags.HasFlag(Combo.ST)
                ? flags.HasFlag(Combo.Adv)
                    ? Preset.DRK_ST_Adv
                    : Preset.DRK_ST_Simple
                : flags.HasFlag(Combo.Adv)
                    ? Preset.DRK_AoE_Adv
                    : Preset.DRK_AoE_Simple;

            var config = flags.HasFlag(Combo.ST)
                ? flags.HasFlag(Combo.Adv)
                    ? DRK_ST_AdvancedMitigation
                    : DRK_ST_SimpleMitigation
                : flags.HasFlag(Combo.Adv)
                    ? DRK_AoE_AdvancedMitigation
                    : DRK_AoE_SimpleMitigation;

            #endregion

            // Bail if Mitigation is not enabled for this combo
            // (unless IPC-controlled)
            if (config != 1 || P.UIHelper.PresetControlled(preset)?.enabled == true)
                return false;

            if (InBossEncounter())
            {
                if (TryGetBossMitigation(flags, ref action))
                    return true;
            }
            else if (TryGetNonBossMitigation(flags, ref action))
                return true;

            return false;
        }

        private static bool TryGetNonBossMitigation(Combo flags, ref uint action)
        {
            // Bail if non-boss mitigation is not enabled
            if (!IsEnabled(Preset.DRK_Mitigation_NonBoss))
                return false;

            #region Living Dead

            #region Variables

            var livingDeadThreshold = flags.HasFlag(Combo.Simple)
                ? 10
                : DRK_Mit_NonBoss_LivingDead_Health;

            #endregion

            if (IsEnabled(Preset.DRK_Mitigation_NonBoss_LivingDead) &&
                ActionReady(LivingDead) &&
                PlayerHealthPercentageHp() <= livingDeadThreshold)
                return (action = LivingDead) != 0;

            #endregion

            // Bail if we can't weave any other mitigations
            if (!CanWeave) return false;
            // Bail if we just used mitigation
            if (JustUsedMitigation) return false;

            var numberOfEnemies = NumberOfEnemiesInRange(Role.Reprisal);

            #region TBN

            if (IsEnabled(Preset.DRK_Mitigation_NonBoss_BlackestNight) &&
                ActionReady(BlackestNight) &&
                // Read others' TBNs as our own, unless burst is near (need darkside)
                (!HasAnyTBN || GetCooldownRemainingTime(LivingShadow) < 30))
                return (action = BlackestNight) != 0;

            #endregion

            #region Dark Missionary

            if (IsEnabled(Preset.DRK_Mitigation_NonBoss_DarkMissionary) &&
                ActionReady(DarkMissionary) &&
                numberOfEnemies > 4 &&
                !JustUsed(OriginalHook(ShadowWall), 15f))
                return (action = DarkMissionary) != 0;

            #endregion

            #region Oblation

            if (ActionReady(Oblation) &&
                IsEnabled(Preset.DRK_Mitigation_NonBoss_Oblation) &&
                numberOfEnemies > 4 &&
                !JustUsed(OriginalHook(Oblation), 10f) &&
                !JustUsed(OriginalHook(ShadowWall), 15f))
                return (action = Oblation) != 0;

            #endregion

            // Bail if average enemy HP% is below threshold
            if (GetAvgEnemyHPPercentInRange(10f) <=
                (flags.HasFlag(Combo.Simple) ? 10 : DRK_Mit_NonBoss_Threshold))
                return false;
            //Bail if already Mitted or too few enemies
            if (MitigationRunning || numberOfEnemies < 3)
                return false;

            #region Mitigation 5+

            if (numberOfEnemies >= 5)
            {
                if (IsEnabled(Preset.DRK_Mitigation_NonBoss_ShadowWall) &&
                    ActionReady(OriginalHook(ShadowWall)))
                    return (action = OriginalHook(ShadowWall)) != 0;

                if (IsEnabled(Preset.DRK_Mitigation_NonBoss_Reprisal) &&
                    ActionReady(Role.Reprisal))
                    return (action = Role.Reprisal) != 0;

                if (IsEnabled(Preset.DRK_Mitigation_NonBoss_ArmsLength) &&
                    ActionReady(Role.ArmsLength))
                    return (action = Role.ArmsLength) != 0;
            }

            #endregion

            #region Mitigation 3+

            if (IsEnabled(Preset.DRK_Mitigation_NonBoss_DarkMind) &&
                ActionReady(DarkMind))
                return (action = DarkMind) != 0;

            if (Role.CanRampart() &&
                IsEnabled(Preset.DRK_Mitigation_NonBoss_Rampart))
                return (action = Role.Rampart) != 0;

            #endregion

            return false;

            bool IsEnabled(Preset preset) =>
                flags.HasFlag(Combo.Simple) ||
                CustomComboFunctions.IsEnabled(preset);
        }

        private static bool TryGetBossMitigation(Combo flags, ref uint action)
        {
            // Bail if boss mitigation is not enabled
            if (!IsEnabled(Preset.DRK_Mitigation_Boss)) return false;
            // Bail if we can't weave any other mitigations
            if (!CanWeave) return false;

            #region Blackest Night (Tank Buster)

            #region Variables

            var blackestNightInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_BlackestNight_TankBuster_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (ActionReady(BlackestNight) &&
                IsEnabled(Preset.DRK_Mitigation_Boss_BlackestNight_TB) &&
                HasIncomingTankBusterEffect() &&
                blackestNightInMitigationContent)
                return (action = BlackestNight) != 0;

            #endregion

            #region Shadow Wall

            #region Variables

            var shadowWallFirst = flags.HasFlag(Combo.Simple)
                ? false
                : DRK_Mit_Boss_ShadowWall_First;

            var shadowWallInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_ShadowWall_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (IsEnabled(Preset.DRK_Mitigation_Boss_ShadowWall) &&
                ActionReady(OriginalHook(ShadowWall)) &&
                shadowWallInMitigationContent &&
                HasIncomingTankBusterEffect() &&
                !JustUsed(Role.Rampart, 20f) &&
                (!ActionReady(Role.Rampart) || shadowWallFirst))
                return (action = OriginalHook(ShadowWall)) != 0;

            #endregion

            #region Rampart

            #region Variables

            var rampartInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_Rampart_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (IsEnabled(Preset.DRK_Mitigation_Boss_Rampart) &&
                ActionReady(Role.Rampart) && rampartInMitigationContent &&
                HasIncomingTankBusterEffect() &&
                !JustUsed(OriginalHook(ShadowWall), 15f))
                return (action = Role.Rampart) != 0;

            #endregion

            #region Blackest Night (on CD)

            #region Variables

            var blackestNightOnCDInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_BlackestNight_OnCD_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            var blackestNightHealthThreshold = flags.HasFlag(Combo.Simple)
                ? 25
                : DRK_Mit_Boss_BlackestNight_Health;

            #endregion

            if (ActionReady(BlackestNight) &&
                IsEnabled(Preset.DRK_Mitigation_Boss_BlackestNight_OnCD) &&
                PlayerHealthPercentageHp() <= blackestNightHealthThreshold &&
                IsPlayerTargeted() &&
                blackestNightOnCDInMitigationContent)
                return (action = BlackestNight) != 0;

            #endregion

            #region Oblation

            #region Variables

            var oblationInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_Oblation_TankBuster_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (ActionReady(Oblation) &&
                IsEnabled(Preset.DRK_Mitigation_Boss_Oblation) &&
                HasIncomingTankBusterEffect() &&
                !JustUsed(OriginalHook(Oblation), 10f) &&
                oblationInMitigationContent)
                return (action = Oblation) != 0;

            #endregion

            #region Dark Mind

            #region Variables

            float emergencyDarkMindThreshold = flags.HasFlag(Combo.Simple)
                ? 80
                : DRK_Mit_Boss_DarkMind_Threshold;

            var alignDarkMind = flags.HasFlag(Combo.Simple)
                ? true
                : DRK_Mit_Boss_DarkMind_Align;

            var darkMindInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_DarkMind_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (IsEnabled(Preset.DRK_Mitigation_Boss_DarkMind) &&
                ActionReady(DarkMind) && HasIncomingTankBusterEffect() &&
                darkMindInMitigationContent &&
                (PlayerHealthPercentageHp() <= emergencyDarkMindThreshold ||
                 (!ActionReady(OriginalHook(ShadowWall)) &&
                  !JustUsed(OriginalHook(ShadowWall), 13f) &&
                  !ActionReady(Role.Rampart) &&
                  !JustUsed(Role.Rampart, 18f)) ||
                 (JustUsed(Role.Rampart, 20f) &&
                  alignDarkMind)))
                return (action = DarkMind) != 0;

            #endregion

            #region Reprisal

            #region Variables

            var reprisalInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_Reprisal_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (IsEnabled(Preset.DRK_Mitigation_Boss_Reprisal) &&
                reprisalInMitigationContent &&
                !JustUsed(DarkMissionary, 10f) &&
                Role.CanReprisal(enemyCount: 1) &&
                GroupDamageIncoming())
                return (action = Role.Reprisal) != 0;

            #endregion

            #region Dark Missionary

            #region Variables

            var darkMissionaryInMitigationContent =
                flags.HasFlag(Combo.Simple) ||
                ContentCheck.IsInConfiguredContent(
                    DRK_Mit_Boss_DarkMissionary_Difficulty,
                    DRK_Boss_Mit_DifficultyListSet);

            #endregion

            if (IsEnabled(Preset.DRK_Mitigation_Boss_DarkMissionary) &&
                darkMissionaryInMitigationContent &&
                !JustUsed(Role.Reprisal, 10f) &&
                ActionReady(DarkMissionary) &&
                GroupDamageIncoming())
                return (action = DarkMissionary) != 0;

            #endregion

            return false;

            bool IsEnabled(Preset preset) =>
                flags.HasFlag(Combo.Simple) ||
                CustomComboFunctions.IsEnabled(preset);
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
            () => !((TargetIsFriendly() &&
                     HasStatusEffect(Buffs.BlackestNightShield,
                         CurrentTarget,
                         anyOwner: true)) ||
                    (!TargetIsFriendly() &&
                     HasStatusEffect(Buffs.BlackestNightShield,
                         anyOwner: true))) &&
                  LocalPlayer.CurrentMp > 3000),
        (Oblation, Preset.DRK_Mit_Oblation,
            () => !((TargetIsFriendly() &&
                     HasStatusEffect(Buffs.Oblation,
                         CurrentTarget,
                         anyOwner: true)) ||
                    (!TargetIsFriendly() &&
                     HasStatusEffect(Buffs.Oblation,
                         anyOwner: true))) &&
                  GetRemainingCharges(Oblation) > DRK_Mit_Oblation_Charges),
        (Role.Reprisal, Preset.DRK_Mit_Reprisal,
            () => Role.CanReprisal()),
        (DarkMissionary, Preset.DRK_Mit_DarkMissionary,
            () => DRK_Mit_DarkMissionary_PartyRequirement ==
                (int)PartyRequirement.No || IsInParty()),
        (Role.Rampart, Preset.DRK_Mit_Rampart,
            () => Role.CanRampart()),
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