using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.PLD.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    #region Variables
    
    private static PLDGauge Gauge => GetJobGauge<PLDGauge>();

    private static bool HasDivineMight =>
        HasStatusEffect(Buffs.DivineMight);

    private static bool HasDivineMagicMP =>
        LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit);

    #endregion

    #region One Button Mitigation Priority

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
    ///     <see cref="IsEnabled(Preset)">is enabled</see>
    ///     and if the action is <see cref="ActionReady(uint,bool,bool)">ready</see> and
    ///     <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///     Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, Preset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Sheltron
        (OriginalHook(Sheltron), Preset.PLD_Mit_Sheltron,
            () => Gauge.OathGauge >= 50),

        // Reprisal
        (Role.Reprisal, Preset.PLD_Mit_Reprisal,
            () => Role.CanReprisal()),

        //Divine Veil
        (DivineVeil, Preset.PLD_Mit_DivineVeil,
            () => PLD_Mit_DivineVeil_PartyRequirement ==
                  (int)PartyRequirement.No ||
                  IsInParty()),

        //Rampart
        (Role.Rampart, Preset.PLD_Mit_Rampart,
            () => Role.CanRampart()),

        //Bulwark
        (Bulwark, Preset.PLD_Mit_Bulwark,
            () => true),

        //Arm's Length
        (Role.ArmsLength, Preset.PLD_Mit_ArmsLength,
            () => Role.CanArmsLength(PLD_Mit_ArmsLength_EnemyCount,
                PLD_Mit_ArmsLength_Boss)),

        //Sentinel
        (OriginalHook(Sentinel), Preset.PLD_Mit_Sentinel,
            () => true),

        //Clemency
        (Clemency, Preset.PLD_Mit_Clemency,
            () => LocalPlayer.CurrentMp >= 2000 &&
                  PlayerHealthPercentageHp() <= PLD_Mit_Clemency_Health)
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
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion

    #region Auto Mitigation System

    [Flags]
    private enum RotationMode
    {
        Simple = 1 << 0,
        Advanced = 1 << 1
    }

    private static bool TryUseMits(RotationMode rotationFlags, ref uint actionID) => CanUseNonBossMits(rotationFlags, ref actionID) || CanUseBossMits(rotationFlags, ref actionID);

    private static bool CanUseNonBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Variables

        bool mitigationRunning =
            HasStatusEffect(Role.Buffs.ArmsLength) ||
            HasStatusEffect(Role.Buffs.Rampart) ||
            HasStatusEffect(Buffs.HallowedGround) ||
            HasStatusEffect(Buffs.Bulwark) ||
            HasStatusEffect(Buffs.Sentinel) ||
            HasStatusEffect(Buffs.Guardian) ||
            HasStatusEffect(Role.Debuffs.Reprisal, CurrentTarget);

        bool justMitted =
            JustUsed(OriginalHook(Bulwark)) ||
            JustUsed(OriginalHook(Sentinel)) ||
            JustUsed(OriginalHook(Sheltron)) ||
            JustUsed(Role.ArmsLength) ||
            JustUsed(Role.Rampart) ||
            JustUsed(Role.Reprisal) ||
            JustUsed(HallowedGround);

        int numberOfEnemies = NumberOfEnemiesInRange(Role.Reprisal);

        #endregion

        #region Initial Bailout

        if (!InCombat() ||
            InBossEncounter() ||
            !IsEnabled(Preset.PLD_Mitigation_NonBoss) ||
            (CombatEngageDuration().TotalSeconds <= 15 && IsMoving()))
            return false;

        #endregion

        #region Hallowed Ground Invulnerability

        int hallowedThreshold = rotationFlags.HasFlag(RotationMode.Simple) ? 20 : PLD_Mitigation_NonBoss_HallowedGround_Health;

        if (IsEnabled(Preset.PLD_Mitigation_NonBoss_HallowedGroundEmergency) && ActionReady(HallowedGround) &&
            PlayerHealthPercentageHp() <= hallowedThreshold)
        {
            actionID = HallowedGround;
            return true;
        }

        #endregion

        #region Sheltron Use Always

        if (IsEnabled(Preset.PLD_Mitigation_NonBoss_Sheltron) && ActionReady(OriginalHook(Sheltron)) &&
            CanWeave() && !justMitted &&
            !IsMoving() && CanWeave() &&
            !HasStatusEffect(Buffs.Sheltron) && !HasStatusEffect(Buffs.HallowedGround) &&
            Gauge.OathGauge >= 50)
        {
            actionID = OriginalHook(Sheltron);
            return true;
        }

        #endregion

        #region Mitigation Threshold / Weave Bailout

        float mitigationThreshold = rotationFlags.HasFlag(RotationMode.Simple)
            ? 10
            : PLD_Mitigation_NonBoss_MitigationThreshold;

        if (GetAvgEnemyHPPercentInRange(5f) <= mitigationThreshold || !CanWeave() || justMitted) return false;

        #endregion

        #region Divine Veil Health Checked

        int divineVeilThreshold = rotationFlags.HasFlag(RotationMode.Simple)
            ? 80
            : PLD_Mitigation_NonBoss_DivineVeil_Health;

        if (IsEnabled(Preset.PLD_Mitigation_NonBoss_DivineVeil) && ActionReady(DivineVeil) &&
            PlayerHealthPercentageHp() <= divineVeilThreshold)
        {
            actionID = DivineVeil;
            return true;
        }

        #endregion

        if (mitigationRunning || numberOfEnemies <= 2) return false; //Bail if already Mitted or too few enemies

        #region Mitigation 5+

        if (numberOfEnemies >= 5)
        {
            if (ActionReady(HallowedGround) && IsEnabled(Preset.PLD_Mitigation_NonBoss_HallowedGround))
            {
                actionID = HallowedGround;
                return true;
            }
            if (ActionReady(OriginalHook(Sentinel)) && IsEnabled(Preset.PLD_Mitigation_NonBoss_Sentinel))
            {
                actionID = OriginalHook(Sentinel);
                return true;
            }
            if (ActionReady(Role.ArmsLength) && IsEnabled(Preset.PLD_Mitigation_NonBoss_ArmsLength))
            {
                actionID = Role.ArmsLength;
                return true;
            }
            if (IsEnabled(Preset.PLD_Mitigation_NonBoss_Reprisal) && ActionReady(Role.Reprisal))
            {
                actionID = Role.Reprisal;
                return true;
            }
        }

        #endregion

        #region Mitigation 3+

        if (Role.CanRampart() && IsEnabled(Preset.PLD_Mitigation_NonBoss_Rampart))
        {
            actionID = Role.Rampart;
            return true;
        }
        if (ActionReady(Bulwark) && IsEnabled(Preset.PLD_Mitigation_NonBoss_Bulwark))
        {
            actionID = Bulwark;
            return true;
        }

        #endregion

        return false;

        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.Simple))
                return true;

            return CustomComboFunctions.IsEnabled(preset);
        }
    }

    private static bool CanUseBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Initial Bailout

        if (!InCombat() || !CanWeave() || !InBossEncounter() || !IsEnabled(Preset.PLD_Mitigation_Boss)) return false;

        #endregion

        #region Sentinel

        bool sentinelFirst = rotationFlags.HasFlag(RotationMode.Simple)
            ? false
            : PLD_Mitigation_Boss_Sentinel_First;

        bool sentinelInMitigationContent = rotationFlags.HasFlag(RotationMode.Simple) ||
                                           ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Sentinel_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        if (IsEnabled(Preset.PLD_Mitigation_Boss_Sentinel) &&
            ActionReady(OriginalHook(Sentinel)) && sentinelInMitigationContent && HasIncomingTankBusterEffect() &&
            !JustUsed(Role.Rampart, 20f) && // Prevent double big mits
            (!ActionReady(Role.Rampart) || sentinelFirst)) //Sentinel First or don't use unless rampart is on cd.
        {
            actionID = OriginalHook(Sentinel);
            return true;
        }

        #endregion

        #region Rampart

        bool rampartInMitigationContent = rotationFlags.HasFlag(RotationMode.Simple) ||
                                          ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Rampart_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        if (IsEnabled(Preset.PLD_Mitigation_Boss_Rampart) &&
            ActionReady(Role.Rampart) && rampartInMitigationContent && HasIncomingTankBusterEffect()
            && !JustUsed(OriginalHook(Sentinel), 15f)) // Prevent double big mits
        {
            actionID = Role.Rampart;
            return true;
        }

        #endregion

        #region Sheltron

        int sheltronOathThreshold = rotationFlags.HasFlag(RotationMode.Simple)
            ? 95
            : PLD_Mitigation_Boss_SheltronOvercap_Threshold;
        
        int sheltronHealthThreshold = rotationFlags.HasFlag(RotationMode.Simple)
            ? 100
            : PLD_Mitigation_Boss_SheltronOvercap_HealthThreshold;
        
        int sheltronDelay= rotationFlags.HasFlag(RotationMode.Simple)
            ? 0
            : PLD_Mitigation_Boss_SheltronDelay;

        bool sheltronInMitigationContent = rotationFlags.HasFlag(RotationMode.Simple) ||
                                           ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_SheltronTankbuster_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        bool sheltronOvercap = IsEnabled(Preset.PLD_Mitigation_Boss_SheltronOvercap) && 
                               Gauge.OathGauge >= sheltronOathThreshold && IsPlayerTargeted() && PlayerHealthPercentageHp() <= sheltronHealthThreshold;
        bool sheltronTankbuster = IsEnabled(Preset.PLD_Mitigation_Boss_SheltronTankbuster) && Gauge.OathGauge >= 50 && sheltronInMitigationContent &&
                                  HasIncomingTankBusterEffect(out var incomingBusterAge) && incomingBusterAge >= sheltronDelay;

        if (ActionReady(OriginalHook(Sheltron)) &&
            !HasStatusEffect(Buffs.Sheltron) &&
            (sheltronOvercap || sheltronTankbuster))
        {
            actionID = OriginalHook(Sheltron);
            return true;
        }

        #endregion

        #region Bulwark

        float emergencyBulwarkThreshold = rotationFlags.HasFlag(RotationMode.Simple)
            ? 80
            : PLD_Mitigation_Boss_Bulwark_Threshold;

        bool alignBulwark = rotationFlags.HasFlag(RotationMode.Simple)
            ? true
            : PLD_Mitigation_Boss_Bulwark_Align;

        bool bulwarkInMitigationContent = rotationFlags.HasFlag(RotationMode.Simple) ||
                                          ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Bulwark_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        bool emergencyBulwark = PlayerHealthPercentageHp() <= emergencyBulwarkThreshold;
        bool noOtherMitsToUse = !ActionReady(OriginalHook(Sentinel)) && !JustUsed(OriginalHook(Sentinel), 13f) &&
                                !ActionReady(Role.Rampart) && !JustUsed(Role.Rampart, 18f);
        bool alignBulwarkWithRampart = JustUsed(Role.Rampart, 20f) && alignBulwark;


        if (IsEnabled(Preset.PLD_Mitigation_Boss_Bulwark) && ActionReady(Bulwark) && HasIncomingTankBusterEffect() && bulwarkInMitigationContent &&
            (emergencyBulwark || noOtherMitsToUse || alignBulwarkWithRampart))
        {
            actionID = Bulwark;
            return true;
        }

        #endregion

        #region Reprisal

        bool reprisalInMitigationContent = rotationFlags.HasFlag(RotationMode.Simple) ||
                                           ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Reprisal_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        if (IsEnabled(Preset.PLD_Mitigation_Boss_Reprisal) &&
            Role.CanReprisal(enemyCount: 1) && reprisalInMitigationContent && GroupDamageIncoming() &&
            !JustUsed(DivineVeil, 10f))
        {
            actionID = Role.Reprisal;
            return true;
        }

        #endregion

        #region Divine Veil

        bool divineVeilInMitigationContent = rotationFlags.HasFlag(RotationMode.Simple) ||
                                             ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_DivineVeil_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        if (IsEnabled(Preset.PLD_Mitigation_Boss_DivineVeil) &&
            divineVeilInMitigationContent && ActionReady(DivineVeil) && GroupDamageIncoming() &&
            !JustUsed(Role.Reprisal, 10f))
        {
            actionID = DivineVeil;
            return true;
        }

        #endregion

        return false;

        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.Simple))
                return true;

            return CustomComboFunctions.IsEnabled(preset);
        }
    }

    #endregion
    
    #region Rotation
    
    #region Flag Stuff
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
    #endregion
    
    #region OGCD Attacks
    private static bool TryOGCDAttacks(Combo flags, ref uint actionID)
    {
        #region Enables
        bool interruptEnabled =
             flags.HasFlag(Combo.Simple) ||
             IsSTEnabled(flags, Preset.PLD_ST_Interrupt) ||
             IsAoEEnabled(flags, Preset.PLD_AoE_Interrupt);
        
        bool lowBlowEnabled = 
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_LowBlow) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_LowBlow);
        
        bool requiescatEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_Requiescat) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_Requiescat);
        
        bool fightOrFlightEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_FoF) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_FoF);
        
        bool circleOfScornEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_CircleOfScorn) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_CircleOfScorn);
        
        bool spiritsWithinEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_SpiritsWithin) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_SpiritsWithin);
        
        bool bladeOfHonorEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_BladeOfHonor) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_BladeOfHonor);
        
        bool interveneEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_Intervene) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_Intervene);
        #endregion
        
        #region Configs
        int fightOrFlightThresholdST = PLD_ST_FoF_BossOption == 1 || !TargetIsBoss() ? PLD_ST_FoF_HPOption : 0; //Boss Check
        int fightOrFlightThresholdAoE = PLD_AoE_FoF_BossOption == 1 || !TargetIsBoss() ? PLD_AoE_FoF_HPOption : 0; //Boss Check
        int fightOrFlightStopThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? fightOrFlightThresholdST : fightOrFlightThresholdAoE;
        
        bool hasRequiescatMPSimple = LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6;

        bool poolCircleForManual = !flags.HasFlag(Combo.Simple) && 
                                (flags.HasFlag(Combo.ST) && PLD_ST_AdvancedMode_CircleOfScorn_ManualPooling ||
                                flags.HasFlag(Combo.AoE) && PLD_AoE_AdvancedMode_CircleOfScorn_ManualPooling);
        
        bool poolSpiritsForManual = !flags.HasFlag(Combo.Simple) && 
                                   (flags.HasFlag(Combo.ST) && PLD_ST_AdvancedMode_SpiritsWithin_ManualPooling ||
                                    flags.HasFlag(Combo.AoE) && PLD_AoE_AdvancedMode_SpiritsWithin_ManualPooling);
        
        bool poolInterveneForManual = !flags.HasFlag(Combo.Simple) && 
                                    (flags.HasFlag(Combo.ST) && PLD_ST_AdvancedMode_Intervene_ManualPooling ||
                                     flags.HasFlag(Combo.AoE) && PLD_AoE_AdvancedMode_Intervene_ManualPooling);
        
        bool hasRequiescatMpST =
            IsNotEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) && hasRequiescatMPSimple ||
            IsEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6 + PLD_ST_MP_Reserve;

        bool hasRequiescatMpAoE =
            IsNotEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) && hasRequiescatMPSimple ||
            IsEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6 + PLD_AoE_MP_Reserve;
        
        bool hasRequiescatMp = 
            flags.HasFlag(Combo.Simple) ? hasRequiescatMPSimple : 
            flags.HasFlag(Combo.ST) ? hasRequiescatMpST : hasRequiescatMpAoE;
        
        int interveneChargeThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PLD_ST_Intervene_Charges : PLD_AoE_Intervene_Charges;
        
        float interveneDistanceThreshold = 
            flags.HasFlag(Combo.Simple) ? 3 : 
            flags.HasFlag(Combo.ST) ? PLD_ST_Intervene_Distance : PLD_AoE_Intervene_Distance;
        
        int interveneMovement = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PLD_ST_Intervene_Movement : PLD_AoE_Intervene_Movement;
        
        float interveneTimeStoodStill = 
            flags.HasFlag(Combo.Simple) ? 2 : 
            flags.HasFlag(Combo.ST) ? PLD_ST_InterveneTimeStill : PLD_AoE_InterveneTimeStill;
        #endregion
        
        if (InCombat() && HasBattleTarget() && CanWeave())
        {
            if (interruptEnabled && Role.CanInterject())
            {
                actionID = Role.Interject;
                return true;
            }

            if (lowBlowEnabled && Role.CanLowBlow() && CanStunToInterruptEnemy())
            {
                actionID = Role.LowBlow;
                return true;
            }
            
            if (fightOrFlightEnabled && !HasWeaved() && CombatEngageDuration().TotalSeconds >= 8 && //Time to hold buffing for non opener pulls.
                OriginalHook(FightOrFlight) is FightOrFlight && ActionReady(FightOrFlight) && //To make sure it doesnt try to weave gcd Goring Blade
                (!LevelChecked(Requiescat) || hasRequiescatMp) && //Must Have Enough Mana to combo
                (InMeleeRange() || LevelChecked(Imperator) && InActionRange(Imperator) && IsOffCooldown(Imperator)) && // in melee or ready to start ranged combo with imperator
                GetTargetHPPercent() >= fightOrFlightStopThreshold) //Health Threshold Check, boss check built into config
            {
                actionID = FightOrFlight;
                return true;
            }
            
            if ((requiescatEnabled && ActionReady(OriginalHook(Requiescat)) && GetCooldownRemainingTime(FightOrFlight) > 50 && InActionRange(OriginalHook(Requiescat))) || //Requiescat Logic, in action range because Imperator gets 25y range
                (bladeOfHonorEnabled && LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)) //Blade of Honor Logic since it shares the button
            {
                actionID = OriginalHook(Requiescat);
                return true;
            }
                
            if (interveneEnabled && ActionReady(Intervene) && !JustUsed(Intervene, 2f) && 
                (!fightOrFlightEnabled && !poolInterveneForManual || GetCooldownRemainingTime(FightOrFlight) > 40) && //Buff Window Check
                GetRemainingCharges(Intervene) > interveneChargeThreshold && //Charge Check
                GetTargetDistance() <= interveneDistanceThreshold && //Distance Check
                (interveneMovement == 1 || //Time Standing Still Check
                 interveneMovement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(interveneTimeStoodStill))) 
            {
                actionID = Intervene;
                return true;
            }

            if (circleOfScornEnabled && ActionReady(CircleOfScorn) && NumberOfEnemiesInRange(CircleOfScorn) > 0 && //Enemy Check as it requires no target to fire
                (!fightOrFlightEnabled && !poolCircleForManual || GetCooldownRemainingTime(FightOrFlight) > 15))
            {
                actionID = CircleOfScorn;
                return true;
            }
            
            if (spiritsWithinEnabled && ActionReady(OriginalHook(SpiritsWithin)) && 
                (!fightOrFlightEnabled && !poolSpiritsForManual || GetCooldownRemainingTime(FightOrFlight) > 15))
            {
                actionID = OriginalHook(SpiritsWithin);
                return true;
            }
        }
        return false;
    }
    #endregion
    
    #region GCD Attacks
    private static bool TryGCDAttacks(Combo flags, ref uint actionID)
    {
        #region Enables
        bool goringBladeEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_GoringBlade) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_GoringBlade);
        
        bool confiteorComboEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_Confiteor) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_Confiteor);
        
        bool holySpellEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_HolySpirit) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_HolyCircle);
        
        bool atonementEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_Atonement);
        
        bool rangedUptimeEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_AdvancedMode_ShieldLob) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_AdvancedMode_ShieldLob);
        
        bool interuptStunEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PLD_ST_ShieldBash) ||
            IsAoEEnabled(flags, Preset.PLD_AoE_ShieldBash);
        #endregion
        
        #region Configs
        bool isAboveMPReserveAoE =
            IsNotEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) ||
            IsEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) + PLD_AoE_MP_Reserve;

        bool isAboveMPReserveST =
            IsNotEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) ||
            IsEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) + PLD_ST_MP_Reserve;
        
        bool isAboveMPReserve = 
            flags.HasFlag(Combo.Simple) ? LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) : 
            flags.HasFlag(Combo.ST) ? isAboveMPReserveST : isAboveMPReserveAoE;
        
        int goringBladePriority =
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PLD_ST_AdvancedMode_GoringBladePrioritize : PLD_AoE_AdvancedMode_GoringBladePrioritize;
        
        int holySpiritUptime =
            flags.HasFlag(Combo.Simple) ? 1 : 
            flags.HasFlag(Combo.ST) ? PLD_ST_ShieldLob_SubOption : PLD_AoE_ShieldLob_SubOption;
        
        
        
        #endregion
        
        #region Variables
        bool rangedUptimeRangeCheck = !InMeleeRange() && flags.HasFlag(Combo.ST) || 
                                      !InActionRange(TotalEclipse) && flags.HasFlag(Combo.AoE);
        
        bool inAtonementPhase = HasStatusEffect(Buffs.AtonementReady) || 
                                HasStatusEffect(Buffs.SupplicationReady) ||
                                HasStatusEffect(Buffs.SepulchreReady);
        
        bool isAtonementExpiring = HasStatusEffect(Buffs.AtonementReady) && GetStatusEffectRemainingTime(Buffs.AtonementReady) < 6 ||
                                   HasStatusEffect(Buffs.SupplicationReady) && GetStatusEffectRemainingTime(Buffs.SupplicationReady) < 6 ||
                                   HasStatusEffect(Buffs.SepulchreReady) && GetStatusEffectRemainingTime(Buffs.SepulchreReady) < 6;
        #endregion
        
        if (goringBladeEnabled &&  HasStatusEffect(Buffs.GoringBladeReady) && 
            InMeleeRange() && HasBattleTarget() &&
            (goringBladePriority == 1 || !HasStatusEffect(Buffs.Requiescat)) && //Option to allow it to go first if in melee range in case you need to move out after
            (flags.HasFlag(Combo.ST) || NumberOfEnemiesInRange(TotalEclipse) <= 3)) //Aoe limit on number of targets as dps loss at 4+
        {
            actionID = GoringBlade;
            return true;
        }
        
        if (confiteorComboEnabled && HasStatusEffect(Buffs.Requiescat) && HasDivineMagicMP && //Does not have a battle target check as un-targeting and fast blade retargeting breaks Cofefe combo. DO NOT ADD.
            (HasStatusEffect(Buffs.ConfiteorReady) || //Confiteor
             LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)) //Its combo
        {
            actionID = OriginalHook(Confiteor);
            return true;
        }
        
        
            
        if (holySpellEnabled && HasDivineMagicMP && isAboveMPReserve && HasBattleTarget() &&
            (HasStatusEffect(Buffs.Requiescat) || //Use if you have req stacks. Should only happen if You are under level for Cofefe Combo
             HasDivineMight && !InMeleeRange() || //Out of melee Use this before shield lob
             HasDivineMight && HasStatusEffect(Buffs.FightOrFlight) || // Burn in buff window
             HasDivineMight && ComboAction is RiotBlade && flags.HasFlag(Combo.ST)|| //Use if about to refresh Divine Might ST (Not combined with below for a reason)
             HasDivineMight && ComboAction is TotalEclipse && flags.HasFlag(Combo.AoE)|| //Use if about to refresh Divine Might AOE
             HasDivineMight && GetStatusEffectRemainingTime(Buffs.DivineMight) < 6)) //Use if expiring
        {
            if (flags.HasFlag(Combo.ST) && ActionReady(HolySpirit))
            {
                actionID = HolySpirit;
                return true;
            }
            if (flags.HasFlag(Combo.AoE) && ActionReady(HolyCircle))
            {
                actionID = HolyCircle;
                return true;
            }
        }

        if (rangedUptimeEnabled && LevelChecked(ShieldLob) && HasBattleTarget() && rangedUptimeRangeCheck)
        {
            //Holy Spirit Ranged Uptime Options
            if (ActionReady(HolySpirit) && holySpiritUptime == 1 && !IsMoving())
            {
                actionID = HolySpirit;
                return true;
            }

            // Otherwise Captain America
            {
                actionID = ShieldLob;
                return true;
            }
        }
        
        if (atonementEnabled && inAtonementPhase && flags.HasFlag(Combo.ST) && HasBattleTarget() &&
            (HasStatusEffect(Buffs.FightOrFlight) || //Will burn them in Buff window
             ComboAction is RiotBlade || //Will hold them until you are about to get more
             HasStatusEffect(Buffs.AtonementReady) || //Will use atonement Asap to Get the supplication ready
             isAtonementExpiring)) //Burn it if it is expiring soon
        {
            actionID = OriginalHook(Atonement);
            return true;
        }

        if (interuptStunEnabled && ActionReady(ShieldBash) && //Shield Bash interrupt
            InMeleeRange() && HasBattleTarget() &&
            !JustUsedOn(ShieldBash, CurrentTarget, 10) && CanStunToInterruptEnemy())
        {
            actionID = ShieldBash;
            return true;
        } 
        
        return false;
    }
    #endregion
    
    #region Basic Combos
    internal static uint STCombo
        => ComboTimer > 0 
            ? LevelChecked(RiotBlade) && ComboAction == FastBlade
                ? RiotBlade
                : LevelChecked(RageOfHalone) && ComboAction == RiotBlade 
                    ? OriginalHook(RageOfHalone)
                    : FastBlade
            : FastBlade;
    
    internal static uint AoECombo 
        => ComboTimer > 0 && LevelChecked(Prominence) && ComboAction == TotalEclipse 
            ? Prominence 
            : TotalEclipse;
    #endregion
    
    #endregion

    #region Openers

    internal static PLDLvl100StandardOpener Lvl100StandardOpener = new();
    internal static PLDLvl100EarlyBuffOpener Lvl100EarlyBuffOpener = new();

    internal static WrathOpener Opener()
    {
        if (PLD_SelectedOpener == 1 && Lvl100EarlyBuffOpener.LevelChecked)
            return Lvl100EarlyBuffOpener;

        return Lvl100StandardOpener.LevelChecked ? Lvl100StandardOpener : WrathOpener.Dummy;
    }

    internal class PLDLvl100StandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            HolySpirit,
            FastBlade,
            RiotBlade,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)),
            RoyalAuthority,
            FightOrFlight,
            Imperator,
            Confiteor,
            CircleOfScorn,
            Expiacion,
            BladeOfFaith,
            Intervene, //12
            BladeOfTruth,
            Intervene, //14
            BladeOfValor,
            BladeOfHonor,
            GoringBlade,
            Atonement,
            Supplication,
            Sepulchre,
            HolySpirit
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([12, 14], () => !HasCharges(Intervene) || PLD_ST_AdvancedMode_BalanceOpener_Intervene != 0)
        ];

        public override Preset Preset => Preset.PLD_ST_AdvancedMode_BalanceOpener;
        internal override UserData ContentCheckConfig => PLD_Balance_Content;
        internal override bool IncludePot => PLD_Opener_Potion;

        public override bool HasCooldowns() =>
            IsOffCooldown(FightOrFlight) &&
            IsOffCooldown(Imperator) &&
            IsOffCooldown(CircleOfScorn) &&
            IsOffCooldown(Expiacion) &&
            IsOffCooldown(GoringBlade);
    }

    internal class PLDLvl100EarlyBuffOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            HolySpirit,
            FastBlade,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Strength)),
            FightOrFlight,
            Imperator,
            RiotBlade,
            CircleOfScorn,
            Expiacion,
            RoyalAuthority,
            Intervene, //10
            GoringBlade,
            Intervene, //12
            Confiteor,
            BladeOfFaith,
            BladeOfTruth,
            BladeOfValor,
            BladeOfHonor,
            HolySpirit,
            Atonement,
            Supplication,
            Sepulchre
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([1], FastBlade, () => !HasTarget() || InCombat())
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([2], () => ComboAction == FastBlade),
            ([10, 12], () => !HasCharges(Intervene) || PLD_ST_AdvancedMode_BalanceOpener_Intervene != 0),
            ([19, 20, 21], () => !InMeleeRange())
        ];

        public override Preset Preset => Preset.PLD_ST_AdvancedMode_BalanceOpener;
        internal override UserData ContentCheckConfig => PLD_Balance_Content;
        internal override bool IncludePot => PLD_Opener_Potion;

        public override bool HasCooldowns() =>
            IsOffCooldown(FightOrFlight) &&
            IsOffCooldown(Imperator) &&
            IsOffCooldown(CircleOfScorn) &&
            IsOffCooldown(Expiacion) &&
            IsOffCooldown(GoringBlade);
    }

    #endregion

    #region ID's

    public const float CooldownThreshold = 0.5f;

    public const uint
        FastBlade = 9,
        RiotBlade = 15,
        ShieldBash = 16,
        Sentinel = 17,
        RageOfHalone = 21,
        Bulwark = 22,
        CircleOfScorn = 23,
        ShieldLob = 24,
        Cover = 27,
        IronWill = 28,
        SpiritsWithin = 29,
        HallowedGround = 30,
        GoringBlade = 3538,
        DivineVeil = 3540,
        PassageOfArms = 7385,
        RoyalAuthority = 3539,
        Guardian = 36920,
        TotalEclipse = 7381,
        Intervention = 7382,
        Requiescat = 7383,
        Imperator = 36921,
        HolySpirit = 7384,
        Prominence = 16457,
        HolyCircle = 16458,
        Confiteor = 16459,
        Expiacion = 25747,
        BladeOfFaith = 25748,
        BladeOfTruth = 25749,
        BladeOfValor = 25750,
        FightOrFlight = 20,
        Atonement = 16460,
        Supplication = 36918, // Second Atonement
        Sepulchre = 36919, // Third Atonement
        Intervene = 16461,
        BladeOfHonor = 36922,
        Sheltron = 3542,
        HolySheltron = 25746,
        Clemency = 3541;

    public static class Buffs
    {
        public const ushort
            IronWill = 79,
            HallowedGround = 82,
            Requiescat = 1368,
            AtonementReady = 1902, // First Atonement Buff
            SupplicationReady = 3827, // Second Atonement Buff
            SepulchreReady = 3828, // Third Atonement Buff
            GoringBladeReady = 3847,
            BladeOfHonor = 3831,
            FightOrFlight = 76,
            ConfiteorReady = 3019,
            DivineMight = 2673,
            HolySheltron = 2674,
            PassageOfArms = 1175,
            Sheltron = 1856,
            Intervention = 2020,
            Guardian = 3829,
            Sentinel = 74,
            Bulwark = 77;
    }

    public static class Debuffs
    {
        public const ushort
            BladeOfValor = 2721,
            GoringBlade = 725;
    }

    #endregion
}
