#region

using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK
{
    internal class DRK_ST_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_ST_Combo;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            // Bail if not looking at the replaced action
            if (actionID != HardSlash) return actionID;

            var gauge = GetJobGauge<DRKGauge>();
            var inManaPoolingContent =
                ContentCheck.IsInConfiguredContent(
                    Config.DRK_ST_ManaSpenderPoolingDifficulty,
                    Config.DRK_ST_ManaSpenderPoolingDifficultyListSet
                );
            var mpRemaining = inManaPoolingContent
                ? Config.DRK_ST_ManaSpenderPooling
                : 0;
            var hpRemainingShadow = Config.DRK_ST_LivingShadowThreshold;
            var hpRemainingDelirium = Config.DRK_ST_DeliriumThreshold;
            var hpRemainingVigil = Config.DRK_ST_ShadowedVigilThreshold;
            var hpRemainingLivingDead = Config.DRK_ST_LivingDeadSelfThreshold;
            var hpRemainingLivingDeadTarget =
                Config.DRK_ST_LivingDeadTargetThreshold;
            var bossRestrictionLivingDead =
                (int)Config.DRK_ST_LivingDeadBossRestriction;

            // Variant Cure - Heal: Priority to save your life
            if (IsEnabled(CustomComboPreset.DRK_Variant_Cure)
                && IsEnabled(Variant.VariantCure)
                && PlayerHealthPercentageHp() <=
                GetOptionValue(Config.DRK_VariantCure))
                return Variant.VariantCure;

            // Unmend Option
            if (IsEnabled(CustomComboPreset.DRK_ST_RangedUptime)
                && LevelChecked(Unmend)
                && !InMeleeRange()
                && HasBattleTarget())
                return Unmend;

            // Bail if not in combat
            if (!InCombat()) return HardSlash;

            // Disesteem
            if (LevelChecked(LivingShadow)
                && LevelChecked(Disesteem)
                && IsEnabled(CustomComboPreset.DRK_ST_CDs_Disesteem)
                && HasEffect(Buffs.Scorn)
                && ((gauge.DarksideTimeRemaining > 0 // Optimal usage
                     && GetBuffRemainingTime(Buffs.Scorn) < 24)
                    || GetBuffRemainingTime(Buffs.Scorn) < 14) // Emergency usage
               )
                return OriginalHook(Disesteem);

            // oGCDs
            if (CanWeave(actionID))
            {
                var inMitigationContent =
                    ContentCheck.IsInConfiguredContent(
                        Config.DRK_ST_MitDifficulty,
                        Config.DRK_ST_MitDifficultyListSet
                    );
                // Mitigation first
                if (IsEnabled(CustomComboPreset.DRK_ST_Mitigation) &&
                    inMitigationContent)
                {
                    // TBN
                    if (IsEnabled(CustomComboPreset.DRK_ST_TBN)
                        && IsOffCooldown(BlackestNight)
                        && LevelChecked(BlackestNight)
                        && ShouldTBNSelf()
                        && LocalPlayer.CurrentMp >= 3000)
                        return BlackestNight;

                    // Shadowed Vigil
                    if (IsEnabled(CustomComboPreset.DRK_ST_ShadowedVigil)
                        && IsOffCooldown(ShadowedVigil)
                        && LevelChecked(ShadowedVigil)
                        && PlayerHealthPercentageHp() <= hpRemainingVigil)
                        return ShadowedVigil;

                    // Living Dead
                    if (IsEnabled(CustomComboPreset.DRK_ST_LivingDead)
                        && IsOffCooldown(LivingDead)
                        && LevelChecked(LivingDead)
                        && PlayerHealthPercentageHp() <= hpRemainingLivingDead
                        && GetTargetHPPercent() >= hpRemainingLivingDeadTarget
                        // Checking if the target matches the boss avoidance option
                        && ((bossRestrictionLivingDead is
                                 (int)Config.BossAvoidance.On
                             && LocalPlayer.TargetObject is not null
                             && TargetIsBoss())
                            || bossRestrictionLivingDead is
                                (int)Config.BossAvoidance.Off))
                        return LivingDead;
                }

                // Variant Spirit Dart - DoT
                var sustainedDamage =
                    FindTargetEffect(Variant.Debuffs.SustainedDamage);
                if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart)
                    && IsEnabled(Variant.VariantSpiritDart)
                    && (sustainedDamage is null ||
                        sustainedDamage.RemainingTime <= 3))
                    return Variant.VariantSpiritDart;

                // Variant Ultimatum - AoE Agro stun
                if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum)
                    && IsEnabled(Variant.VariantUltimatum)
                    && IsOffCooldown(Variant.VariantUltimatum))
                    return Variant.VariantUltimatum;

                // Mana Spenders
                if (IsEnabled(CustomComboPreset.DRK_ST_ManaOvercap)
                    && (CanWeave(actionID) || CanDelayedWeave(actionID))
                    && ((CombatEngageDuration().TotalSeconds < 10
                         && gauge.DarksideTimeRemaining == 0) // Initial Darkside
                        || CombatEngageDuration().TotalSeconds >= 10)) // Post Opener
                {
                    // Spend mana to limit when not near even minute burst windows
                    if (IsEnabled(CustomComboPreset.DRK_ST_ManaSpenderPooling)
                        && GetCooldownRemainingTime(LivingShadow) >= 45
                        && LocalPlayer.CurrentMp > (mpRemaining + 3000)
                        && LevelChecked(EdgeOfDarkness))
                        return OriginalHook(EdgeOfDarkness);

                    // Keep Darkside up
                    if (LocalPlayer.CurrentMp > 8500
                        || (gauge.DarksideTimeRemaining < 10000 &&
                            LocalPlayer.CurrentMp > (mpRemaining + 3000)))
                    {
                        // Return Edge of Darkness if available
                        if (LevelChecked(EdgeOfDarkness))
                            return OriginalHook(EdgeOfDarkness);
                        if (LevelChecked(FloodOfDarkness)
                            && !LevelChecked(EdgeOfDarkness))
                            return FloodOfDarkness;
                    }

                    // Spend Dark Arts
                    if (gauge.HasDarkArts
                        && LevelChecked(EdgeOfDarkness)
                        && CombatEngageDuration().TotalSeconds >= 25
                        && (gauge.ShadowTimeRemaining > 0 // In Burst
                            || (IsEnabled(CustomComboPreset
                                    .DRK_ST_DarkArtsDropPrevention)
                                && HasOwnTBN))) // TBN
                        return OriginalHook(EdgeOfDarkness);
                }

                // Bigger Cooldown Features
                if (gauge.DarksideTimeRemaining > 1)
                {
                    // Living Shadow
                    var inLivingShadowThresholdContent =
                        ContentCheck.IsInConfiguredContent(
                            Config.DRK_ST_LivingShadowThresholdDifficulty,
                            Config.DRK_ST_LivingShadowThresholdDifficultyListSet
                        );
                    if (IsEnabled(CustomComboPreset.DRK_ST_CDs)
                        && IsEnabled(CustomComboPreset.DRK_ST_CDs_LivingShadow)
                        && IsOffCooldown(LivingShadow)
                        && LevelChecked(LivingShadow)
                        && ((inLivingShadowThresholdContent
                             && GetTargetHPPercent() > hpRemainingShadow)
                            || !inLivingShadowThresholdContent))
                        return LivingShadow;

                    // Delirium
                    var inDeliriumThresholdContent =
                        ContentCheck.IsInConfiguredContent(
                            Config.DRK_ST_DeliriumThresholdDifficulty,
                            Config.DRK_ST_DeliriumThresholdDifficultyListSet
                        );
                    if (IsEnabled(CustomComboPreset.DRK_ST_Delirium)
                        && IsOffCooldown(BloodWeapon)
                        && LevelChecked(BloodWeapon)
                        && ((inDeliriumThresholdContent
                             && GetTargetHPPercent() > hpRemainingDelirium)
                            || !inDeliriumThresholdContent)
                        && ((CombatEngageDuration().TotalSeconds < 8 // Opener
                             && WasLastWeaponskill(Souleater))
                            || CombatEngageDuration().TotalSeconds > 8)) // Regular
                        return OriginalHook(Delirium);

                    // Big CDs
                    if (IsEnabled(CustomComboPreset.DRK_ST_CDs)
                        && ((CombatEngageDuration().TotalSeconds < 10 // Opener CDs
                             && !HasEffect(Buffs.Scorn)
                             && IsOnCooldown(LivingShadow))
                            || CombatEngageDuration().TotalSeconds > 10)) // Regular
                    {
                        // Salted Earth
                        if (IsEnabled(CustomComboPreset.DRK_ST_CDs_SaltedEarth))
                        {
                            // Cast Salted Earth
                            if (!HasEffect(Buffs.SaltedEarth)
                                && ActionReady(SaltedEarth))
                                return SaltedEarth;
                            //Cast Salt and Darkness
                            if (HasEffect(Buffs.SaltedEarth)
                                && GetBuffRemainingTime(Buffs.SaltedEarth) < 7
                                && ActionReady(SaltAndDarkness))
                                return OriginalHook(SaltAndDarkness);
                        }

                        // Shadowbringer
                        if (LevelChecked(Shadowbringer)
                            && IsEnabled(CustomComboPreset.DRK_ST_CDs_Shadowbringer))
                        {
                            if ((GetRemainingCharges(Shadowbringer) > 0
                                 && IsNotEnabled(CustomComboPreset
                                     .DRK_ST_CDs_ShadowbringerBurst)) // Dump
                                ||
                                (IsEnabled(CustomComboPreset
                                     .DRK_ST_CDs_ShadowbringerBurst)
                                 && GetRemainingCharges(Shadowbringer) > 0
                                 && gauge.ShadowTimeRemaining > 1
                                 && IsOnCooldown(LivingShadow)
                                 && !HasEffect(Buffs.Scorn))) // Burst
                                return Shadowbringer;
                        }

                        // Carve and Spit
                        if (IsEnabled(CustomComboPreset.DRK_ST_CDs_CarveAndSpit)
                            && IsOffCooldown(CarveAndSpit)
                            && LevelChecked(CarveAndSpit))
                            return CarveAndSpit;
                    }
                }
            }

            // Delirium Chain
            if (LevelChecked(Delirium)
                && LevelChecked(ScarletDelirium)
                && IsEnabled(CustomComboPreset.DRK_ST_Delirium_Chain)
                && HasEffect(Buffs.EnhancedDelirium)
                && gauge.DarksideTimeRemaining > 0)
                return OriginalHook(Bloodspiller);

            //Delirium Features
            if (LevelChecked(Delirium)
                && IsEnabled(CustomComboPreset.DRK_ST_Bloodspiller))
            {
                //Bloodspiller under Delirium
                var deliriumBuff = TraitLevelChecked(Traits.EnhancedDelirium)
                    ? Buffs.EnhancedDelirium
                    : Buffs.Delirium;
                if (GetBuffStacks(deliriumBuff) > 0)
                    return Bloodspiller;

                //Blood management outside of Delirium
                if (IsEnabled(CustomComboPreset.DRK_ST_Delirium)
                    && ((gauge.Blood >= 60 &&
                         GetCooldownRemainingTime(Delirium) is > 0
                             and < 3) // Prep for Delirium
                        || (gauge.Blood >= 50 &&
                            GetCooldownRemainingTime(Delirium) >
                            37))) // Regular Bloodspiller
                    return Bloodspiller;
            }

            // 1-2-3 combo
            if (!(comboTime > 0)) return HardSlash;
            if (lastComboMove == HardSlash && LevelChecked(SyphonStrike))
                return SyphonStrike;
            if (lastComboMove == SyphonStrike && LevelChecked(Souleater))
            {
                // Blood management
                if (IsEnabled(CustomComboPreset.DRK_ST_BloodOvercap)
                    && LevelChecked(Bloodspiller) && gauge.Blood >= 90)
                    return Bloodspiller;

                return Souleater;
            }

            return HardSlash;
        }
    }

    internal class DRK_AoE_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_AoE_Combo;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            // Bail if not looking at the replaced action
            if (actionID != Unleash) return actionID;

            var gauge = GetJobGauge<DRKGauge>();
            var hpRemainingShadow = Config.DRK_AoE_LivingShadowThreshold;
            var hpRemainingDelirium = Config.DRK_AoE_DeliriumThreshold;
            var hpRemainingVigil = Config.DRK_AoE_ShadowedVigilThreshold;
            var hpRemainingLivingDead =
                Config.DRK_AoE_LivingDeadSelfThreshold;
            var hpRemainingLivingDeadTarget =
                Config.DRK_AoE_LivingDeadTargetThreshold;

            // Variant Cure - Heal: Priority to save your life
            if (IsEnabled(CustomComboPreset.DRK_Variant_Cure)
                && IsEnabled(Variant.VariantCure)
                && PlayerHealthPercentageHp() <=
                GetOptionValue(Config.DRK_VariantCure))
                return Variant.VariantCure;

            // Disesteem
            if (LevelChecked(LivingShadow)
                && LevelChecked(Disesteem)
                && IsEnabled(CustomComboPreset.DRK_AoE_CDs_Disesteem)
                && HasEffect(Buffs.Scorn)
                && (gauge.DarksideTimeRemaining > 0 // Optimal usage
                    || GetBuffRemainingTime(Buffs.Scorn) < 5)) // Emergency usage
                return OriginalHook(Disesteem);

            // oGCDs
            if (CanWeave(actionID) || CanDelayedWeave(actionID))
            {
                // Mitigation first
                if (IsEnabled(CustomComboPreset.DRK_AoE_Mitigation))
                {
                    // TBN
                    if (IsEnabled(CustomComboPreset.DRK_AoE_TBN)
                        && IsOffCooldown(BlackestNight)
                        && LevelChecked(BlackestNight)
                        && ShouldTBNSelf(aoe: true)
                        && LocalPlayer.CurrentMp >= 3000)
                        return BlackestNight;

                    // Shadowed Vigil
                    if (IsEnabled(CustomComboPreset.DRK_AoE_ShadowedVigil)
                        && IsOffCooldown(ShadowedVigil)
                        && LevelChecked(ShadowedVigil)
                        && PlayerHealthPercentageHp() <= hpRemainingVigil)
                        return ShadowedVigil;

                    // Living Dead
                    if (IsEnabled(CustomComboPreset.DRK_AoE_LivingDead)
                        && IsOffCooldown(LivingDead)
                        && LevelChecked(LivingDead)
                        && PlayerHealthPercentageHp() <= hpRemainingLivingDead
                        && GetTargetHPPercent() >= hpRemainingLivingDeadTarget)
                        return LivingDead;
                }

                // Variant Spirit Dart - DoT
                var sustainedDamage =
                    FindTargetEffect(Variant.Debuffs.SustainedDamage);
                if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart)
                    && IsEnabled(Variant.VariantSpiritDart)
                    && (sustainedDamage is null ||
                        sustainedDamage.RemainingTime <= 3))
                    return Variant.VariantSpiritDart;

                // Variant Ultimatum - AoE Agro stun
                if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum)
                    && IsEnabled(Variant.VariantUltimatum)
                    && IsOffCooldown(Variant.VariantUltimatum))
                    return Variant.VariantUltimatum;

                // Mana Features
                if (IsEnabled(CustomComboPreset.DRK_AoE_ManaOvercap)
                    && LevelChecked(FloodOfDarkness)
                    && (LocalPlayer.CurrentMp > 8500 ||
                        (gauge.DarksideTimeRemaining < 10 &&
                         LocalPlayer.CurrentMp >= 3000)))
                    return OriginalHook(FloodOfDarkness);

                // Spend Dark Arts
                if (IsEnabled(CustomComboPreset.DRK_AoE_ManaOvercap)
                    && gauge.HasDarkArts
                    && LevelChecked(FloodOfDarkness))
                    return OriginalHook(FloodOfDarkness);

                // Living Shadow
                var inLivingShadowThresholdContent =
                    ContentCheck.IsInConfiguredContent(
                        Config.DRK_AoE_LivingShadowThresholdDifficulty,
                        Config.DRK_AoE_LivingShadowThresholdDifficultyListSet
                    );
                if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_LivingShadow)
                    && IsOffCooldown(LivingShadow)
                    && LevelChecked(LivingShadow)
                    && ((inLivingShadowThresholdContent
                         && GetTargetHPPercent() > hpRemainingShadow)
                        || !inLivingShadowThresholdContent))
                    return LivingShadow;

                // Delirium
                var inDeliriumThresholdContent =
                    ContentCheck.IsInConfiguredContent(
                        Config.DRK_AoE_DeliriumThresholdDifficulty,
                        Config.DRK_AoE_DeliriumThresholdDifficultyListSet
                    );
                if (IsEnabled(CustomComboPreset.DRK_AoE_Delirium)
                    && IsOffCooldown(BloodWeapon)
                    && LevelChecked(BloodWeapon)
                    && ((inDeliriumThresholdContent
                         && GetTargetHPPercent() > hpRemainingDelirium)
                        || !inDeliriumThresholdContent))
                    return OriginalHook(Delirium);

                if (gauge.DarksideTimeRemaining > 1)
                {
                    // Salted Earth
                    if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_SaltedEarth))
                    {
                        // Cast Salted Earth
                        if (!HasEffect(Buffs.SaltedEarth)
                            && ActionReady(SaltedEarth))
                            return SaltedEarth;
                        //Cast Salt and Darkness
                        if (HasEffect(Buffs.SaltedEarth)
                            && GetBuffRemainingTime(Buffs.SaltedEarth) < 9
                            && ActionReady(SaltAndDarkness))
                            return OriginalHook(SaltAndDarkness);
                    }

                    // Shadowbringer
                    if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_Shadowbringer)
                        && LevelChecked(Shadowbringer)
                        && GetRemainingCharges(Shadowbringer) > 0)
                        return Shadowbringer;

                    // Abyssal Drain
                    if (IsEnabled(CustomComboPreset.DRK_AoE_CDs_AbyssalDrain)
                        && LevelChecked(AbyssalDrain)
                        && IsOffCooldown(AbyssalDrain)
                        && PlayerHealthPercentageHp() <= 60)
                        return AbyssalDrain;
                }
            }

            // Delirium Chain
            if (LevelChecked(Delirium)
                && LevelChecked(Impalement)
                && IsEnabled(CustomComboPreset.DRK_AoE_Delirium_Chain)
                && HasEffect(Buffs.EnhancedDelirium)
                && gauge.DarksideTimeRemaining > 1)
                return OriginalHook(Quietus);

            // 1-2-3 combo
            if (!(comboTime > 0)) return Unleash;
            if (lastComboMove == Unleash && LevelChecked(StalwartSoul))
            {
                if (IsEnabled(CustomComboPreset.DRK_AoE_BloodOvercap)
                    && gauge.Blood >= 90
                    && LevelChecked(Quietus))
                    return Quietus;
                return StalwartSoul;
            }

            return Unleash;
        }
    }

    internal class DRK_oGCD : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.DRK_oGCD;

        protected override uint Invoke(uint actionID, uint lastComboMove,
            float comboTime, byte level)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (actionID == CarveAndSpit || actionID == AbyssalDrain)
            {
                if (IsOffCooldown(LivingShadow)
                    && LevelChecked(LivingShadow))
                    return LivingShadow;

                if (IsOffCooldown(SaltedEarth)
                    && LevelChecked(SaltedEarth))
                    return SaltedEarth;

                if (IsOffCooldown(CarveAndSpit)
                    && LevelChecked(AbyssalDrain))
                    return actionID;

                if (IsOffCooldown(SaltAndDarkness)
                    && HasEffect(Buffs.SaltedEarth)
                    && LevelChecked(SaltAndDarkness))
                    return SaltAndDarkness;

                if (IsEnabled(CustomComboPreset.DRK_Shadowbringer_oGCD)
                    && GetCooldownRemainingTime(Shadowbringer) < 60
                    && LevelChecked(Shadowbringer)
                    && gauge.DarksideTimeRemaining > 0)
                    return Shadowbringer;
            }

            return actionID;
        }
    }

    #region IDs

    public const byte JobID = 32;

    #region Actions

    public const uint

        #region Single-Target 1-2-3 Combo

        HardSlash = 3617,
        SyphonStrike = 3623,
        Souleater = 3632,

        #endregion

        #region AoE 1-2-3 Combo

        Unleash = 3621,
        StalwartSoul = 16468,

        #endregion

        #region Single-Target oGCDs

        CarveAndSpit = 3643, // With AbyssalDrain
        EdgeOfDarkness = 16467, // For MP
        EdgeOfShadow = 16470, // For MP // Upgrade of EdgeOfDarkness
        Bloodspiller = 7392, // For Blood
        ScarletDelirium = 36928, // Under Enhanced Delirium
        Comeuppance = 36929, // Under Enhanced Delirium
        Torcleaver = 36930, // Under Enhanced Delirium

        #endregion

        #region AoE oGCDs

        AbyssalDrain = 3641, // Cooldown shared with CarveAndSpit
        FloodOfDarkness = 16466, // For MP
        FloodOfShadow = 16469, // For MP // Upgrade of FloodOfDarkness
        Quietus = 7391, // For Blood
        SaltedEarth = 3639,
        SaltAndDarkness = 25755, // Recast of Salted Earth
        Impalement = 36931, // Under Delirium

        #endregion

        #region Buffing oGCDs

        BloodWeapon = 3625,
        Delirium = 7390,

        #endregion

        #region Burst Window

        LivingShadow = 16472,
        Shadowbringer = 25757,
        Disesteem = 36932,

        #endregion

        #region Ranged Option

        Unmend = 3624,

        #endregion

        #region Mitigation

        BlackestNight = 7393,
        LivingDead = 3638,
        ShadowedVigil = 36927;

    #endregion

    #endregion

    public static class Buffs
    {
        #region Main Buffs

        /// Tank Stance
        public const ushort Grit = 743;

        /// The lowest level buff, before Delirium
        public const ushort BloodWeapon = 742;

        /// The lower Delirium buff, with just the blood ability usage
        public const ushort Delirium = 1972;

        /// Different from Delirium, to do the Scarlet Delirium chain
        public const ushort EnhancedDelirium = 3836;

        /// The increased damage buff that should always be up - checked through gauge
        public const ushort Darkside = 741;

        #endregion

        #region "DoT" or Burst

        /// Ground DoT active status
        public const ushort SaltedEarth = 749;

        /// Charge to be able to use Disesteem
        public const ushort Scorn = 3837;

        #endregion

        #region Mitigation

        /// TBN Active - Dark arts checked through gauge
        public const ushort BlackestNightShield = 1178;

        /// The initial Invuln that needs procc'd
        public const ushort LivingDead = 810;

        /// The real, triggered Invuln that gives heals
        public const ushort WalkingDead = 811;

        /// Damage Reduction part of Vigil
        public const ushort ShadowedVigil = 3835;

        /// The triggered part of Vigil that needs procc'd to heal (happens below 50%)
        public const ushort ShadowedVigilant = 3902;

        #endregion
    }

    public static class Traits
    {
        public const uint
            EnhancedDelirium = 572;
    }

    #endregion
}
