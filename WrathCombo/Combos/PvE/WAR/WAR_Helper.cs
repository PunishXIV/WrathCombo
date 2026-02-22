#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.WAR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WAR : Tank
{
    #region Variables
    internal static WARGauge Gauge = GetJobGauge<WARGauge>(); //WAR gauge
    internal static int BeastGauge => Gauge.BeastGauge;
    internal static (float Cooldown, float Status, int Stacks) IR => (GetCooldownRemainingTime(OriginalHook(Berserk)), GetStatusEffectRemainingTime(Buffs.InnerReleaseBuff), GetStatusEffectStacks(Buffs.InnerReleaseStacks));
    internal static (float Status, int Stacks) BF => (GetStatusEffectRemainingTime(Buffs.BurgeoningFury), GetStatusEffectStacks(Buffs.BurgeoningFury));
    internal static (bool Status, bool Stacks) HasIR => (IR.Status > 0, IR.Stacks > 0 || HasStatusEffect(Buffs.InnerReleaseStacks));
    internal static (bool Status, bool Stacks) HasBF => (BF.Status > 0 || HasStatusEffect(Buffs.BurgeoningFury), (BF.Stacks > 0 || HasStatusEffect(Buffs.BurgeoningFury)));
    internal static bool HasSurgingTempest => !LevelChecked(StormsEye) || HasStatusEffect(Buffs.SurgingTempest);
    internal static bool HasNascentChaos => HasStatusEffect(Buffs.NascentChaos);
    internal static bool HasWrathful => HasStatusEffect(Buffs.Wrathful);
    internal static bool Minimal => InCombat() && HasBattleTarget();
    #endregion

    #region Openers
    
    internal static WAROpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class WAROpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Tomahawk,
            Infuriate,
            HeavySwing,
            Maim,
            StormsEye,
            InnerRelease,
            InnerChaos,
            Upheaval,
            Onslaught, //9
            FellCleave,
            Onslaught, //11
            FellCleave,
            Onslaught, //13
            FellCleave,
            PrimalWrath,
            Infuriate,
            PrimalRend,
            PrimalRuination,
            InnerChaos,
            HeavySwing,
            Maim,
            StormsPath,
            FellCleave,
            Infuriate,
            InnerChaos
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([9, 11, 13], () => !HasCharges(Onslaught) || WAR_ST_BalanceOpener_GapcloserChoice == 0)
        ];
        public override Preset Preset => Preset.WAR_ST_BalanceOpener;
        internal override UserData ContentCheckConfig => WAR_BalanceOpener_Content;
        public override bool HasCooldowns() => IsOffCooldown(InnerRelease) && IsOffCooldown(Upheaval) && GetRemainingCharges(Infuriate) >= 2 && GetRemainingCharges(Onslaught) >= 3;
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
             IsSTEnabled(flags, Preset.WAR_ST_Interrupt) ||
             IsAoEEnabled(flags, Preset.WAR_AoE_Interrupt);
        
        bool lowBlowEnabled = 
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_Stun) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_Stun);
        
        bool innerReleaseEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_InnerRelease) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_InnerRelease);
        
        bool infuriateEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_Infuriate) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_Infuriate);
        
        bool onslaughtEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_Onslaught) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_Onslaught);
        
        bool upheavalEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_Upheaval) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_Orogeny);
        
        bool primalWrathEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_PrimalWrath) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_PrimalWrath);
        
        #endregion
        
        #region Configs
        int innerReleaseThresholdST = WAR_ST_InnerRelease_Threshold_SubOption == 1 || !InBossEncounter() ? WAR_ST_InnerRelease_Threshold : 0; //Boss Check
        int innerReleaseThresholdAoE = WAR_AoE_InnerRelease_Threshold_SubOption == 1 || !InBossEncounter() ? WAR_AoE_InnerRelease_Threshold : 0; //Boss Check
        int innerReleaseStopThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? innerReleaseThresholdST : innerReleaseThresholdAoE;
        
        int infuriateGaugeThreshold = 
            flags.HasFlag(Combo.Simple) ? 40 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_Infuriate_Gauge : WAR_AoE_Infuriate_Gauge;
        
        int infuriateChargeThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_Infuriate_Charges : WAR_AoE_Infuriate_Charges;
        
        int onslaughtChargeThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_Onslaught_Charges : WAR_AoE_Onslaught_Charges;
        
        float onslaughtDistanceThreshold = 
            flags.HasFlag(Combo.Simple) ? 3 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_Onslaught_Distance : WAR_AoE_Onslaught_Distance;
        
        int onslaughtMovement = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_Onslaught_Movement : WAR_AoE_Onslaught_Movement;
        
        float onslaughtTimeStoodStill = 
            flags.HasFlag(Combo.Simple) ? 2 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_Onslaught_TimeStill : WAR_AoE_Onslaught_TimeStill;
        #endregion
        
        if (InCombat() && HasBattleTarget() && CanWeave())
        {
            if (interruptEnabled && Role.CanInterject())
            {
                actionID = Role.Interject;
                return true;
            }

            if (lowBlowEnabled && Role.CanLowBlow())
            {
                actionID = Role.LowBlow;
                return true;
            }
            
            if (innerReleaseEnabled &&
                ActionReady(OriginalHook(Berserk)) && HasSurgingTempest && !HasWrathful &&
                GetTargetHPPercent() >= innerReleaseStopThreshold) //Health Threshold Check, boss check built into config
            {
                actionID = OriginalHook(Berserk);
                return true;
            }
            
            if (infuriateEnabled && 
                ActionReady(Infuriate) && !HasNascentChaos && !JustUsed(Infuriate) && !HasIR.Stacks && 
                BeastGauge <= infuriateGaugeThreshold && //Gauge slider check
                GetRemainingCharges(Infuriate) > infuriateChargeThreshold) //Charge slider check
            {
                actionID = Infuriate;
                return true;
            }
                
            if (onslaughtEnabled && 
                ActionReady(Onslaught) && HasSurgingTempest &&
                (!innerReleaseEnabled || innerReleaseEnabled && IR.Cooldown > 40) && //Buff Window Check
                GetRemainingCharges(Onslaught) > onslaughtChargeThreshold &&  //Charge Slider Check
                GetTargetDistance() <= onslaughtDistanceThreshold && //Distance Check
                (onslaughtMovement == 1 || //Any Movement
                 onslaughtMovement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(onslaughtTimeStoodStill))) //How long standing still
            {
                actionID = Onslaught;
                return true;
            }

            if (upheavalEnabled &&
                ActionReady(Upheaval) && HasSurgingTempest && InActionRange(Upheaval))
            {
                if (flags.HasFlag(Combo.ST) || 
                    flags.HasFlag(Combo.AoE) && !LevelChecked(Orogeny)) //Use Upheaval if too low level
                {
                    actionID = Upheaval;
                    return true;
                }
                if (flags.HasFlag(Combo.AoE))
                {
                    actionID = Orogeny;
                    return true;
                }
            }

            if (primalWrathEnabled && 
                LevelChecked(PrimalWrath)  && HasWrathful && HasSurgingTempest && GetTargetDistance() <= 4.99f)
            {
                actionID = PrimalWrath;
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
        bool primalRendEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_PrimalRend) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_PrimalRend);
        
        bool rangedUptimeEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_RangedUptime) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_RangedUptime);
        
        bool primalRuinationEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_PrimalRuination) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_PrimalRuination);
        
        bool fellCleaveEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.WAR_ST_FellCleave) ||
            IsAoEEnabled(flags, Preset.WAR_AoE_Decimate);
        #endregion
        
        #region Configs
        int fellCleaveGaugeThresholdST =
            WAR_ST_FellCleave_Pooling //is pooling enabled
                ? !WAR_ST_FellCleave_Pooling_BossOnly || InBossEncounter() //Is pooling not boss only or in a boss encounter
                    ? !JustUsed(OriginalHook(Berserk), 20f) //not in Burst Window
                        ? ComboAction is HeavySwing
                            ? 100 //Use to prevent Overcap from Maim
                            : ComboAction is Maim  
                                ? 90 //Use to prevent Overcap
                                : 110 //Dont use it
                        : 50 // In Burst Window, Dump at 50
                    : 50 //Option not selected and not in Boss encounter, dump at 50
                : WAR_ST_FellCleave_Gauge; //Pooling not enabled, follow slider
            
        int decimateGaugeThresholdAoE = 
            WAR_AoE_Decimate_Pooling //is pooling enabled
                ? !WAR_AoE_Decimate_Pooling_BossOnly || InBossEncounter() //Is pooling not boss only or in a boss encounter
                    ? !JustUsed(OriginalHook(Berserk), 20f) //not in Burst Window
                        ?ComboAction is Overpower 
                            ? 90 //Use to prevent Overcap
                            : 110 //Dont use it
                        : 50 // In Burst Window, Dump at 50
                    : 50 //Option not selected and not in Boss encounter, dump at 50
                : WAR_AoE_Decimate_Gauge; //Pooling not enabled, follow slider
        
        int spenderGaugeThreshold = 
            flags.HasFlag(Combo.Simple) ? 50 : 
            flags.HasFlag(Combo.ST) ? fellCleaveGaugeThresholdST : decimateGaugeThresholdAoE;
        
        float primalRendDistanceThreshold = 
            flags.HasFlag(Combo.Simple) ? 3 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_PrimalRend_Distance : WAR_AoE_PrimalRend_Distance;
        
        int primalRendMovement = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_PrimalRend_Movement : WAR_AoE_PrimalRend_Movement;
        
        float primalRendTimeStoodStill = 
            flags.HasFlag(Combo.Simple) ? 2 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_PrimalRend_TimeStill : WAR_AoE_PrimalRend_TimeStill;
        
        int primalRendTiming = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? WAR_ST_PrimalRend_EarlyLate : WAR_AoE_PrimalRend_EarlyLate;

            
        #endregion
        
        if (HasBattleTarget())  
        {
            #region Primal Rend
            if (primalRendEnabled && HasSurgingTempest && HasStatusEffect(Buffs.PrimalRendReady) && 
                GetTargetDistance() <= primalRendDistanceThreshold && //Distance Slider Check
                (primalRendMovement == 1 || //Any Movement
                 primalRendMovement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(primalRendTimeStoodStill)) && //Time Stood Still Slider Check
                (primalRendTiming == 0 || //Use Asap
                 GetStatusEffectRemainingTime(Buffs.PrimalRendReady) <= 15 || //Use if buff gets below 15
                 !HasIR.Stacks && !HasBF.Stacks && !HasWrathful)) //Use when all your other stuff is spent
            {
                actionID = PrimalRend;
                return true;
            }
            #endregion

            #region Primal Ruination
            if (primalRuinationEnabled && HasSurgingTempest && HasStatusEffect(Buffs.PrimalRuinationReady))
            {
                actionID = PrimalRuination;
                return true;
            }
            #endregion

            #region Inner Beast/Fell Cleave/ Decimate
            if (fellCleaveEnabled && HasSurgingTempest && LevelChecked(OriginalHook(InnerBeast)) &&
                (HasStatusEffect(Buffs.InnerReleaseStacks) || //Use if you have IR stacks
                 BeastGauge >= spenderGaugeThreshold || //Use if you have enough gauge, See config
                 HasNascentChaos)) //Use if you have Nascent Buff
            {
                int enemyCount = NumberOfEnemiesInRange(Decimate);
                
                if (InMeleeRange() && //Melee range check for single target
                    (flags.HasFlag(Combo.ST) || //Fell Cleave in ST
                     flags.HasFlag(Combo.AoE) && (!LevelChecked(Decimate) || //Fell Cleave in Aoe if Decimate too low level
                                                  enemyCount < 4 && TraitLevelChecked(Traits.MeleeMastery2)))) //Fell Cleave in Aoe if decimate less than 4 targets
                {
                    actionID = FellCleave;
                    return true;
                }

                if (flags.HasFlag(Combo.AoE) &&
                    (enemyCount >= 3 && !TraitLevelChecked(Traits.MeleeMastery2) || //3 Targets without trait
                     enemyCount >= 4)) //4 Targets
                {
                    actionID = Decimate;
                    return true;
                }
            }
            #endregion

            #region Ranged Uptime
            if (rangedUptimeEnabled && ActionReady(Tomahawk) && !InMeleeRange())
            {
                actionID = Tomahawk;
                return true;
            }
            #endregion
        }
        return false;
    }
    #endregion
    
    #region Basic Combos
    internal static uint STCombo
        => ComboTimer > 0 
            ? LevelChecked(Maim) && ComboAction == HeavySwing // Logic for Combo 2
                ? Maim
                : LevelChecked(StormsPath) && ComboAction == Maim //Logic for Combos 3.1 and 3.2
                    ? LevelChecked(StormsEye) && ((IsEnabled(Preset.WAR_ST_Simple) && GetStatusEffectRemainingTime(Buffs.SurgingTempest) <= 29) || 
                                                  (IsEnabled(Preset.WAR_ST_Advanced) && IsEnabled(Preset.WAR_ST_StormsEye) && GetStatusEffectRemainingTime(Buffs.SurgingTempest) <= WAR_SurgingRefreshRange))
                        ? StormsEye //return if ST is needed
                        : StormsPath //return if ST is not needed
                    : HeavySwing  //return if cant Storms Path
            : HeavySwing; //Return of cant Maim
    
    internal static uint AoECombo 
        => ComboTimer > 0 && LevelChecked(MythrilTempest) && ComboAction == Overpower 
            ? MythrilTempest 
            : Overpower;
    #endregion
    
    #endregion

    #region One-Button Mitigation Combo Priorities

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
    ///     Each logic check is already combined with checking if the preset is
    ///     enabled and if the action is <see cref="ActionReady(uint)">ready</see>
    ///     and <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///     Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, Preset Preset, Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Bloodwhetting
        (OriginalHook(RawIntuition), Preset.WAR_Mit_Bloodwhetting,
            () => !HasStatusEffect(Buffs.RawIntuition) && !HasStatusEffect(Buffs.BloodwhettingDefenseLong) && PlayerHealthPercentageHp() <= WAR_Mit_Bloodwhetting_Health),
        //Equilibrium
        (Equilibrium, Preset.WAR_Mit_Equilibrium,
            () => PlayerHealthPercentageHp() <= WAR_Mit_Equilibrium_Health),
        // Reprisal
        (Role.Reprisal, Preset.WAR_Mit_Reprisal,
            () => Role.CanReprisal(checkTargetForDebuff: false)),
        //Thrill of Battle
        (ThrillOfBattle, Preset.WAR_Mit_ThrillOfBattle,
            () => PlayerHealthPercentageHp() <= WAR_Mit_ThrillOfBattle_Health),
        //Rampart
        (Role.Rampart, Preset.WAR_Mit_Rampart,
            () => Role.CanRampart()),
        //Shake it Off
        (ShakeItOff, Preset.WAR_Mit_ShakeItOff,
            () => !HasStatusEffect(Buffs.ShakeItOff) && (WAR_Mit_ShakeItOff_PartyRequirement == (int)PartyRequirement.No || IsInParty())),
        //Arm's Length
        (Role.ArmsLength, Preset.WAR_Mit_ArmsLength,
            () => Role.CanArmsLength(WAR_Mit_ArmsLength_EnemyCount, WAR_Mit_ArmsLength_Boss)),
        //Vengeance
        (OriginalHook(Vengeance), Preset.WAR_Mit_Vengeance,
            () => true)
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
    ///     The variable to set to the action to, if the mitigation is set to be used.
    /// </param>
    /// <returns>
    ///     Whether the mitigation is ready, enabled, and passes the provided logic check.
    /// </returns>
    private static bool CheckMitigationConfigMeetsRequirements(int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion
    
    #region Auto Mitigation System
    
    [Flags]
    private enum RotationMode{
        simple = 1 << 0,
        advanced = 1 << 1
    }
    
    private static bool TryUseMits(RotationMode rotationFlags, ref uint actionID) => CanUseNonBossMits(rotationFlags, ref actionID) || CanUseBossMits(rotationFlags, ref actionID);
    
    private static bool CanUseNonBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Variables
        var numberOfEnemies = NumberOfEnemiesInRange(Role.Reprisal);
        
        var mitigationRunning = HasStatusEffect(Role.Buffs.ArmsLength) ||
                                HasStatusEffect(Role.Buffs.Rampart) || 
                                HasStatusEffect(Buffs.Holmgang) ||
                                HasStatusEffect(Buffs.ThrillOfBattle) ||
                                HasStatusEffect(Buffs.Vengeance) || 
                                HasStatusEffect(Buffs.Damnation) ||
                                HasStatusEffect(Role.Debuffs.Reprisal, CurrentTarget);
        
        var justMitted = JustUsed(OriginalHook(ThrillOfBattle)) ||
                          JustUsed(OriginalHook(Vengeance)) ||
                          JustUsed(OriginalHook(RawIntuition)) ||
                          JustUsed(Role.Reprisal) ||
                          JustUsed(Role.ArmsLength) ||
                          JustUsed(Role.Rampart) ||
                          JustUsed(Holmgang);
        #endregion
        
        #region Initial Bailout
        if (!InCombat() ||  
            InBossEncounter() || 
            !IsEnabled(Preset.WAR_Mitigation_NonBoss) || 
            (CombatEngageDuration().TotalSeconds <= 15 && IsMoving()))
            return false;
        #endregion
        
        #region HolmGang Invulnerability
        var holmgangThreshold = rotationFlags.HasFlag(RotationMode.simple) ? 10 : WAR_Mitigation_NonBoss_Holmgang_Health;
        
        if (IsEnabled(Preset.WAR_Mitigation_NonBoss_Holmgang) && ActionReady(Holmgang) &&
            PlayerHealthPercentageHp() <= holmgangThreshold)
        {
            actionID = Holmgang;
            return true;
        }
        #endregion
        
        #region Raw Intuition/Bloodwhetting
        if (IsEnabled(Preset.WAR_Mitigation_NonBoss_RawIntuition) && 
            ActionReady(OriginalHook(RawIntuition)) && CanWeave() && !justMitted)
        {
            actionID = OriginalHook(RawIntuition);
            return true;
        }
        #endregion
        
        #region Mitigation Threshold Bailout Canweave/Justmitted Check
        float mitigationThreshold = rotationFlags.HasFlag(RotationMode.simple) 
            ? 10 
            : WAR_Mitigation_NonBoss_MitigationThreshold;
        if (GetAvgEnemyHPPercentInRange(10f) <= mitigationThreshold || !CanWeave() || justMitted) 
            return false;
        #endregion
        
        #region Equilibrium
        var equilibriumThreshold = rotationFlags.HasFlag(RotationMode.simple) ? 65 : WAR_Mitigation_NonBoss_Equilibrium_Health;
        
        if (IsEnabled(Preset.WAR_Mitigation_NonBoss_Equilibrium) && 
            ActionReady(Equilibrium) &&
            PlayerHealthPercentageHp() <= equilibriumThreshold)
        {
            actionID = Equilibrium;
            return true;
        }
        #endregion
        
        #region Shake It Off
        var shakeItOffThreshold = rotationFlags.HasFlag(RotationMode.simple) ? 80 : WAR_Mitigation_NonBoss_ShakeItOff_Health;
        var safeToShakeItOff = !HasAnyStatusEffects([Buffs.ThrillOfBattle, Buffs.Damnation, Buffs.Vengeance, Buffs.BloodwhettingDefenseLong]);
        
        if (IsEnabled(Preset.WAR_Mitigation_NonBoss_ShakeItOff) && 
            ActionReady(ShakeItOff) && safeToShakeItOff &&
            PlayerHealthPercentageHp() <= shakeItOffThreshold)
        {
            actionID = ShakeItOff;
            return true;
        }
        #endregion
        
        if (mitigationRunning || numberOfEnemies <= 2) return false; //Bail if already Mitted or too few enemies
        
        #region Mitigation 5+
        if (numberOfEnemies >= 5)
        {
            if (ActionReady(OriginalHook(Vengeance)) && IsEnabled(Preset.WAR_Mitigation_NonBoss_Vengeance))
            {
                actionID = OriginalHook(Vengeance);
                return true;
            }
            if (ActionReady(Role.ArmsLength) && IsEnabled(Preset.WAR_Mitigation_NonBoss_ArmsLength))
            {
                actionID = Role.ArmsLength;
                return true;
            }
            if (ActionReady(Role.Reprisal) && IsEnabled(Preset.WAR_Mitigation_NonBoss_Reprisal))
            {
                actionID = Role.Reprisal;
                return true;
            }
        }
        #endregion
        
        #region Mitigation 3+
        if (Role.CanRampart() && IsEnabled(Preset.WAR_Mitigation_NonBoss_Rampart))
        {
            actionID = Role.Rampart;
            return true;
        }
            
        if (ActionReady(ThrillOfBattle) && IsEnabled(Preset.WAR_Mitigation_NonBoss_ThrillOfBattle))
        {
            actionID = ThrillOfBattle;
            return true;
        }
        #endregion
        
        return false;
        
        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.simple))
                return true;
            
            return CustomComboFunctions.IsEnabled(preset);
        }
    }
    
    private static bool CanUseBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Initial Bailout
        if (!InCombat() || !CanWeave() || !InBossEncounter() || !IsEnabled(Preset.WAR_Mitigation_Boss)) return false;
        #endregion
        
        #region Vengeance
        var vengeanceFirst = rotationFlags.HasFlag(RotationMode.simple)
            ? false
            : WAR_Mitigation_Boss_Vengeance_First;
        
        var vengeanceInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                           ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_Vengeance_Difficulty, WAR_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.WAR_Mitigation_Boss_Vengeance) && 
            ActionReady(OriginalHook(Vengeance)) && vengeanceInMitigationContent && HasIncomingTankBusterEffect() 
            && !JustUsed(Role.Rampart, 20f) && // Prevent double big mits
            (!ActionReady(Role.Rampart) || vengeanceFirst)) //Vengeance First or don't use unless rampart is on cd.
        {
            actionID = OriginalHook(Vengeance);
            return true;
        }
        #endregion
        
        #region Rampart
        var rampartInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_Rampart_Difficulty, WAR_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.WAR_Mitigation_Boss_Rampart) && 
            ActionReady(Role.Rampart) && rampartInMitigationContent && HasIncomingTankBusterEffect() && 
            !JustUsed(OriginalHook(Vengeance), 15f)) // Prevent double big mits
        {
            actionID = Role.Rampart;
            return true;
        }
        #endregion
        
        #region Raw Intuition/Bloodwhetting
        var RawIntuitionOnCDInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                              ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_RawIntuition_OnCD_Difficulty, WAR_Boss_Mit_DifficultyListSet);
        
        var RawIntuitionTankBusterInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                              ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_RawIntuition_TankBuster_Difficulty, WAR_Boss_Mit_DifficultyListSet);
        var RawIntuitionHealthThreshold = rotationFlags.HasFlag(RotationMode.simple) 
            ? 80
            : WAR_Mitigation_Boss_RawIntuition_Health;
        
        bool rawIntuitionOnCD = IsEnabled(Preset.WAR_Mitigation_Boss_RawIntuition_OnCD) &&  PlayerHealthPercentageHp() <= RawIntuitionHealthThreshold && IsPlayerTargeted() && RawIntuitionOnCDInMitigationContent;
        bool RawIntuitionTankbuster = IsEnabled(Preset.WAR_Mitigation_Boss_RawIntuition_TankBuster) && HasIncomingTankBusterEffect() && RawIntuitionTankBusterInMitigationContent;
            
        if (ActionReady(OriginalHook(RawIntuition)) && (rawIntuitionOnCD || RawIntuitionTankbuster))
        {
            actionID = OriginalHook(RawIntuition);
            return true;
        }
        #endregion
        
        #region Thrill of Battle
        float emergencyThrillThreshold = rotationFlags.HasFlag(RotationMode.simple)
            ? 80
            : WAR_Mitigation_Boss_ThrillOfBattle_Threshold;
        
        var alignThrillOfBattle = rotationFlags.HasFlag(RotationMode.simple)
            ? true
            : WAR_Mitigation_Boss_ThrillOfBattle_Align;
        
        var ThrillOfBattleInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_ThrillOfBattle_Difficulty, WAR_Boss_Mit_DifficultyListSet);

        bool emergencyThrillOfBattle = PlayerHealthPercentageHp() <= emergencyThrillThreshold;
        bool noOtherMitsToUse = !ActionReady(OriginalHook(Vengeance)) && !JustUsed(OriginalHook(Vengeance), 13f) && !ActionReady(Role.Rampart) && !JustUsed(Role.Rampart, 18f);
        bool alignThrillOfBattleWithRampart = JustUsed(Role.Rampart, 20f) && alignThrillOfBattle;
        
        if (IsEnabled(Preset.WAR_Mitigation_Boss_ThrillOfBattle) && ActionReady(ThrillOfBattle) && HasIncomingTankBusterEffect() && ThrillOfBattleInMitigationContent &&
            (emergencyThrillOfBattle || noOtherMitsToUse || alignThrillOfBattleWithRampart))
        {
            actionID = ThrillOfBattle;
            return true;
        }
        #endregion
        
        #region Equilibrium
        var equilibriumEmergencyThreshold = rotationFlags.HasFlag(RotationMode.simple) ? 30 : WAR_Mitigation_Boss_Equilibrium_Health;
        var equilibriumTankbusterThreshold = rotationFlags.HasFlag(RotationMode.simple) ? 80 : WAR_Mitigation_Boss_Tankbuster_Equilibrium_Health;
        if (IsEnabled(Preset.WAR_Mitigation_Boss_Equilibrium) && 
            ActionReady(Equilibrium) &&
            (PlayerHealthPercentageHp() <= equilibriumEmergencyThreshold || 
            (PlayerHealthPercentageHp() <= equilibriumTankbusterThreshold && HasIncomingTankBusterEffect())))
        {
            actionID = Equilibrium;
            return true;
        }
        #endregion
        
        #region Reprisal
        var ReprisalInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                          ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_Reprisal_Difficulty, WAR_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.WAR_Mitigation_Boss_Reprisal) && 
            Role.CanReprisal(enemyCount:1) && GroupDamageIncoming() && ReprisalInMitigationContent &&
            !JustUsed(ShakeItOff, 10f))
        {
            actionID = Role.Reprisal;
            return true;
        }
        #endregion
        
        #region Shake it Off
        var ShakeItOffInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                            ContentCheck.IsInConfiguredContent(WAR_Mitigation_Boss_ShakeItOff_Difficulty, WAR_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.WAR_Mitigation_Boss_ShakeItOff) && 
            !JustUsed(Role.Reprisal, 10f) && GroupDamageIncoming() && ShakeItOffInMitigationContent &&
            ActionReady(ShakeItOff))
        {
            actionID = ShakeItOff;
            return true;
        }
        #endregion
       
        return false;
        
        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.simple))
                return true;
            
            return CustomComboFunctions.IsEnabled(preset);
        }
    }
    
    #endregion

    #region IDs

    #region Actions

    public const uint
    #region Offensive

        HeavySwing = 31, //Lv1, instant, GCD, range 3, single-target, targets=Hostile
        Maim = 37, //Lv4, instant, GCD, range 3, single-target, targets=Hostile
        Berserk = 38, //Lv6, instant, 60.0s CD (group 10), range 0, single-target, targets=Self
        Overpower = 41, //Lv10, instant, GCD, range 0, AOE 5 circle, targets=Self
        Tomahawk = 46, //Lv15, instant, GCD, range 20, single-target, targets=Hostile
        StormsPath = 42, //Lv26, instant, GCD, range 3, single-target, targets=Hostile
        InnerBeast = 49, //Lv35, instant, GCD, range 3, single-target, targets=Hostile
        MythrilTempest = 16462, //Lv40, instant, GCD, range 0, AOE 5 circle, targets=Self
        SteelCyclone = 51, //Lv45, instant, GCD, range 0, AOE 5 circle, targets=Self
        StormsEye = 45, //Lv50, instant, GCD, range 3, single-target, targets=Hostile
        Infuriate = 52, //Lv50, instant, 60.0s CD (group 19/70) (2 charges), range 0, single-target, targets=Self
        FellCleave = 3549, //Lv54, instant, GCD, range 3, single-target, targets=Hostile
        Decimate = 3550, //Lv60, instant, GCD, range 0, AOE 5 circle, targets=Self
        Onslaught = 7386, //Lv62, instant, 30.0s CD (group 7/71) (2-3 charges), range 20, single-target, targets=Hostile
        Upheaval = 7387, //Lv64, instant, 30.0s CD (group 8), range 3, single-target, targets=Hostile
        InnerRelease = 7389, //Lv70, instant, 60.0s CD (group 11), range 0, single-target, targets=Self
        ChaoticCyclone = 16463, //Lv72, instant, GCD, range 0, AOE 5 circle, targets=Self
        InnerChaos = 16465, //Lv80, instant, GCD, range 3, single-target, targets=Hostile
        Orogeny = 25752, //Lv86, instant, 30.0s CD (group 8), range 0, AOE 5 circle, targets=Self
        PrimalRend = 25753, //Lv90, instant, GCD, range 20, AOE 5 circle, targets=Hostile, animLock=1.150
        PrimalWrath = 36924, //Lv96, instant, 1.0s CD (group 0), range 0, AOE 5 circle, targets=Self
        PrimalRuination = 36925, //Lv100, instant, GCD, range 3, AOE 5 circle, targets=Hostile

    #endregion
    #region Defensive

        Defiance = 48, //Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=Self
        ReleaseDefiance = 32066, //Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=Self
        ThrillOfBattle = 40, //Lv30, instant, 90.0s CD (group 15), range 0, single-target, targets=Self
        Vengeance = 44, //Lv38, instant, 120.0s CD (group 21), range 0, single-target, targets=Self
        Holmgang = 43, //Lv42, instant, 240.0s CD (group 24), range 6, single-target, targets=Self/Hostile
        RawIntuition = 3551, //Lv56, instant, 25.0s CD (group 6), range 0, single-target, targets=Self
        Equilibrium = 3552, //Lv58, instant, 60.0s CD (group 13), range 0, single-target, targets=Self
        ShakeItOff = 7388, //Lv68, instant, 90.0s CD (group 14), range 0, AOE 30 circle, targets=Self
        NascentFlash = 16464, //Lv76, instant, 25.0s CD (group 6), range 30, single-target, targets=Party
        Bloodwhetting = 25751, //Lv82, instant, 25.0s CD (group 6), range 0, single-target, targets=Self
        Damnation = 36923, //Lv92, instant, 120.0s CD (group 21), range 0, single-target, targets=Self

    #endregion

        //Limit Break
        LandWaker = 4240; //LB3, instant, range 0, AOE 50 circle, targets=Self, animLock=3.860

    #endregion

    #region Traits

    public static class Traits
    {
        public const ushort
            None = 0,
            TankMastery = 318, // L1
            TheBeastWithin = 249, // L35, gauge generation
            InnerBeastMastery = 265, // L54, IB->FC upgrade
            SteelCycloneMastery = 266, // L60, steel cyclone -> decimate upgrade
            EnhancedInfuriate = 157, // L66, gauge spenders reduce cd by 5
            BerserkMastery = 218, // L70, berserk -> IR upgrade
            NascentChaos = 267, // L72, decimate -> chaotic cyclone after infuriate
            MasteringTheBeast = 268, // L74, mythril tempest gives gauge
            EnhancedShakeItOff = 417, // L76, adds heal
            EnhancedThrillOfBattle = 269, // L78, adds incoming heal buff
            RawIntuitionMastery = 418, // L82, raw intuition -> bloodwhetting
            EnhancedNascentFlash = 419, // L82, duration increase
            EnhancedEquilibrium = 420, // L84, adds hot
            MeleeMastery1 = 505, // L84, potency increase
            EnhancedOnslaught = 421, // L88, 3rd onslaught charge
            VengeanceMastery = 567, // L92, vengeance -> damnation
            EnhancedRampart = 639, // L94, adds incoming heal buff
            MeleeMastery2 = 654, // L94, potency increase
            EnhancedInnerRelease = 568, // L96, primal wrath mechanic
            EnhancedReprisal = 640, // L98, extend duration to 15s
            EnhancedPrimalRend = 569; // L100, primal ruination mechanic
    }

    #endregion

    #region Buffs

    public static class Buffs
    {
        public const ushort
        #region Offensive

            SurgingTempest = 2677, //applied by Storm's Eye, Mythril Tempest to self, damage buff
            NascentChaos = 1897, //applied by Infuriate to self, converts next FC to IC
            Berserk = 86, //applied by Berserk to self, next 3 GCDs are crit dhit
            InnerReleaseStacks = 1177, //applied by Inner Release to self, next 3 GCDs should be free FCs
            InnerReleaseBuff = 1303, //applied by Inner Release to self, 15s buff
            PrimalRendReady = 2624, //applied by Inner Release to self, allows casting PR
            InnerStrength = 2663, //applied by Inner Release to self, immunes
            BurgeoningFury = 3833, //applied by Fell Cleave to self, 3 stacks turns into wrathful
            Wrathful = 3901, //3rd stack of Burgeoning Fury turns into this, allows Primal Wrath
            PrimalRuinationReady = 3834, //applied by Primal Rend to self

        #endregion
        #region Defensive

            Vengeance = 89, //applied by Vengeance to self this is the whole buff
            Damnation = 3832, //applied by Damnation to self, -40% damage taken and retaliation for physical attacks
            PrimevalImpulse = 3900, //hot applied after hit under Damnation
            ThrillOfBattle = 87, //applied by Thrill of Battle to self
            Holmgang = 409, //applied by Holmgang to self
            EquilibriumRegen = 2681, //applied by Equilibrium to self, hp regen
            ShakeItOff = 1457, //applied by Shake It Off to self/target, damage shield
            ShakeItOffHot = 2108, //applied by Shake It Off to self/target
            RawIntuition = 735, //applied by Raw Intuition to self
            NascentFlashSelf = 1857, //applied by Nascent Flash to self, heal on hit
            NascentFlashTarget = 1858, //applied by Nascent Flash to target, -10% damage taken + heal on hit
            BloodwhettingDefenseLong = 2678, //applied by Bloodwhetting to self, -10% damage taken + heal on hit for 8 sec
            BloodwhettingDefenseShort = 2679, //applied by Bloodwhetting, Nascent Flash to self/target, -10% damage taken for 4 sec
            BloodwhettingShield = 2680, //applied by Bloodwhetting, Nascent Flash to self/target, damage shield
            Defiance = 91, //applied by Defiance to self, tank stance
            ShieldWall = 194, //applied by Shield Wall to self/target
            Stronghold = 195, //applied by Stronghold to self/target
            LandWaker = 863; //applied by Land Waker to self/target

        #endregion
    }

    #endregion

    #region Debuffs

    public static class Debuffs
    {
        public const ushort
            Placeholder = 1;
    }

    #endregion

    #endregion
}
