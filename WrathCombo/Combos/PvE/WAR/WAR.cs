using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvE
{
    internal partial class WAR 
    {
        public const byte ClassID = 3; // Marauder (MRD) 
        public const byte JobID = 21; // Warrior (WAR)
        public const uint //Actions

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
                VengeanceRetaliation = 89, //applied by Vengeance to self, retaliation for physical attacks
                VengeanceDefense = 912, //applied by Vengeance to self, -30% damage taken
                Damnation = 3832, //applied by Damnation to self, -40% damage taken and retaliation for physical attacks
                PrimevalImpulse = 3900, //hot applied after hit under Damnation
                ThrillOfBattle = 87, //applied by Thrill of Battle to self
                Holmgang = 409, //applied by Holmgang to self
                EquilibriumRegen = 2681, //applied by Equilibrium to self, hp regen
                ShakeItOff = 1457, //applied by Shake It Off to self/target, damage shield
                ShakeItOffHOT = 2108, //applied by Shake It Off to self/target
                RawIntuition = 735, //applied by Raw Intuition to self
                NascentFlashSelf = 1857, //applied by Nascent Flash to self, heal on hit
                NascentFlashTarget = 1858, //applied by Nascent Flash to target, -10% damage taken + heal on hit
                BloodwhettingDefenseLong = 2678, //applied by Bloodwhetting to self, -10% damage taken + heal on hit for 8 sec
                BloodwhettingDefenseShort = 2679, //applied by Bloodwhetting, Nascent Flash to self/target, -10% damage taken for 4 sec
                BloodwhettingShield = 2680, //applied by Bloodwhetting, Nascent Flash to self/target, damage shield
                ArmsLength = 1209, //applied by Arm's Length to self
                Defiance = 91, //applied by Defiance to self, tank stance
                ShieldWall = 194, //applied by Shield Wall to self/target
                Stronghold = 195, //applied by Stronghold to self/target
                LandWaker = 863; //applied by Land Waker to self/target
            #endregion
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 1;
        }

        #region Simple Mode - Single Target
        internal class WAR_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Simple; 

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsPath)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) || 
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) || 
                                      JustUsed(Holmgang, 9f);

                    // Mitigation
                    if (InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Holmgang
                        if (ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                            return Holmgang;

                        // Vengeance / Damnation
                        if (ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Vengeance);

                        // Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        // Thrill
                        if (ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                            return ThrillOfBattle;

                        // Equilibrium
                        if (ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < 50) //Player's health is below 30%
                            return Equilibrium;

                        // Bloodwhetting
                        if (ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting
                            PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                            return OriginalHook(Bloodwhetting);
                    }

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    if (LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;
                    if (InCombat() && LevelChecked(Infuriate) && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks) && gauge <= 40 && CanWeave(actionID))
                        return Infuriate;
                    if (CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(StormsEye) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            if (ActionReady(OriginalHook(RawIntuition)) && PlayerHealthPercentageHp() <= 80)
                                return OriginalHook(RawIntuition);
                            if (ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (ActionReady(Upheaval) && LevelChecked(Upheaval))
                                return Upheaval;
                            if (HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                            if (LevelChecked(Onslaught) && GetRemainingCharges(Onslaught) > 1)
                            {
                                if (!IsMoving && GetTargetDistance() <= 1 && (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease)))
                                    return Onslaught;
                            }
                        }

                        if (HasEffect(Buffs.PrimalRendReady) && !JustUsed(InnerRelease) && ((!IsMoving && GetTargetDistance() <= 1) || GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD))
                            return PrimalRend;
                        if (HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination))
                            return PrimalRuination;

                        if (LevelChecked(InnerBeast))
                        {
                            if (HasEffect(Buffs.InnerReleaseStacks) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos)))
                                return OriginalHook(InnerBeast);

                            if (HasEffect(Buffs.NascentChaos) && !LevelChecked(InnerChaos) && gauge >= 50)
                                return OriginalHook(Decimate);
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (LevelChecked(InnerBeast) && (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && gauge >= 90)
                            return OriginalHook(InnerBeast);

                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                        {
                            return Maim;
                        }

                        if (lastComboMove == Maim && LevelChecked(StormsPath))
                        {
                            if (GetBuffRemainingTime(Buffs.SurgingTempest) <= 29 && LevelChecked(StormsEye))
                                return StormsEye;
                            return StormsPath;
                        }
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }
        #endregion

        #region Advanced Mode - Single Target
        internal class WAR_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced) && actionID == StormsPath)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var surgingThreshold = PluginConfiguration.GetCustomIntValue(Config.WAR_SurgingRefreshRange);
                    var onslaughtChargesRemaining = PluginConfiguration.GetCustomIntValue(Config.WAR_KeepOnslaughtCharges);
                    var infuriateChargesRemaining = PluginConfiguration.GetCustomIntValue(Config.WAR_KeepInfuriateCharges);
                    var fellCleaveGaugeSpend = PluginConfiguration.GetCustomIntValue(Config.WAR_FellCleaveGauge);
                    var infuriateGauge = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateSTGauge);
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    // Mitigation
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Mitigation) && //Mitigation option is enabled
                        InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Holmgang
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Holmgang) && //Holmgang option is enabled
                            ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Holmgang_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Holmgang_SubOption == 1 || //Holmgang is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Holmgang_SubOption == 2))) //Holmgang is enabled for bosses only
                            return Holmgang;

                        // Vengeance / Damnation
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Vengeance) && //Vengeance option is enabled
                            ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Vengeance_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Vengeance_SubOption == 1 || //Vengeance is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Vengeance_SubOption == 2))) //Vengeance is enabled for bosses only
                            return OriginalHook(Vengeance);

                        // Rampart
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Rampart) && //Rampart option is enabled
                            ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Rampart_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Rampart_SubOption == 1 || //Rampart is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Rampart_SubOption == 2))) //Rampart is enabled for bosses only
                            return All.Rampart;

                        // Thrill
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Thrill) && //Thrill option is enabled
                            ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Thrill_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Thrill_SubOption == 1 || //Thrill is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Thrill_SubOption == 2))) //Thrill is enabled for bosses only
                            return ThrillOfBattle;

                        // Equilibrium
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Equilibrium) && //Equilibrium option is enabled
                            ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Equilibrium_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Equilibrium_SubOption == 1 || //Equilibrium is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Equilibrium_SubOption == 2))) //Equilibrium is enabled for bosses only
                            return Equilibrium;

                        // Bloodwhetting
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Bloodwhetting) && //Bloodwhetting option is enabled
                            ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Bloodwhetting_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Bloodwhetting_SubOption == 1 || //Bloodwhetting is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Bloodwhetting_SubOption == 2))) //Bloodwhetting is enabled for bosses only
                            return OriginalHook(RawIntuition);
                    }

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) && 
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_RangedUptime) &&
                        LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Infuriate) && 
                        InCombat() && LevelChecked(Infuriate) && ActionReady(Infuriate) && 
                        !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks)
                        && gauge <= infuriateGauge && CanWeave(actionID) && GetRemainingCharges(Infuriate) > infuriateChargesRemaining)
                        return Infuriate;
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(StormsEye) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;


                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Upheaval) && ActionReady(Upheaval) && LevelChecked(Upheaval))
                                return Upheaval;
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalWrath) && HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught) && LevelChecked(Onslaught) && GetRemainingCharges(Onslaught) > onslaughtChargesRemaining)
                            {
                                if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught_MeleeSpender) ||
                                    (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught_MeleeSpender) && !IsMoving && GetTargetDistance() <= 1 && (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease))))
                                    return Onslaught;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && !JustUsed(InnerRelease))
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend_Late)
                                && GetBuffStacks(Buffs.InnerReleaseStacks) is 0 && GetBuffStacks(Buffs.BurgeoningFury) is 0
                                && !HasEffect(Buffs.Wrathful))
                                return PrimalRend;
                            if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend_Late) && ((!IsMoving && GetTargetDistance() <= 1) || GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination))
                            return PrimalRuination;

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && LevelChecked(InnerBeast))
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && LevelChecked(InnerBeast))
                            {
                                if (HasEffect(Buffs.InnerReleaseStacks) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos)))
                                    return OriginalHook(InnerBeast);

                                if (HasEffect(Buffs.NascentChaos) && !LevelChecked(InnerChaos) && gauge >= 50)
                                    return OriginalHook(Decimate);
                            }
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && LevelChecked(InnerBeast) && (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && gauge >= fellCleaveGaugeSpend)
                            return OriginalHook(InnerBeast);

                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                        {
                            return Maim;
                        }

                        if (lastComboMove == Maim && LevelChecked(StormsPath) && IsEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye))
                        {
                            if (GetBuffRemainingTime(Buffs.SurgingTempest) <= surgingThreshold && LevelChecked(StormsEye))
                                return StormsEye;
                            return StormsPath;
                        }
                        if (lastComboMove == Maim && LevelChecked(StormsPath) && IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye))
                        {
                            return StormsPath;
                        }
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }
        #endregion

        #region Simple Mode - AoE
        internal class WAR_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    // Mitigation
                    if (InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Holmgang
                        if (ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                            return Holmgang;

                        // Vengeance / Damnation
                        if (ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Vengeance);

                        // Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        // Thrill
                        if (ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                            return ThrillOfBattle;

                        // Equilibrium
                        if (ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < 50) //Player's health is below 30%
                            return Equilibrium;

                        // Bloodwhetting
                        if (ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting
                            PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                            return OriginalHook(Bloodwhetting);
                    }

                    if (InCombat() && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks) && gauge <= 40 && CanWeave(actionID))
                        return Infuriate;
                    if (CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(MythrilTempest) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            if (ActionReady(OriginalHook(RawIntuition)) && PlayerHealthPercentageHp() <= 80)
                                return OriginalHook(RawIntuition);
                            if (CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (ActionReady(Orogeny) && LevelChecked(Orogeny) && HasEffect(Buffs.SurgingTempest))
                                return Orogeny;
                            if (HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                        }

                        if (HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend))
                            return PrimalRend;
                        if (HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend) && GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD)
                            return PrimalRend;
                        if (HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && JustUsed(PrimalRend, 4f))
                            return PrimalRuination;
                        if (LevelChecked(SteelCyclone) && (gauge >= 90 || HasEffect(Buffs.InnerReleaseStacks) || HasEffect(Buffs.NascentChaos)))
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Overpower && LevelChecked(MythrilTempest))
                        {
                            return MythrilTempest;
                        }
                    }

                    return Overpower;
                }

                return actionID;
            }
        }
        #endregion

        #region Advanced Mode - AoE
        internal class WAR_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower)
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge;
                    var decimateGaugeSpend = PluginConfiguration.GetCustomIntValue(Config.WAR_DecimateGauge);
                    var infuriateGauge = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateAoEGauge);
                    float GCD = GetCooldown(HeavySwing).CooldownTotal;
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    // Mitigation
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Mitigation) && //Mitigation option is enabled
                        InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Holmgang
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Holmgang) && //Holmgang option is enabled
                            ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Holmgang_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Holmgang_SubOption == 1 || //Holmgang is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Holmgang_SubOption == 2))) //Holmgang is enabled for bosses only
                            return Holmgang;

                        // Vengeance / Damnation
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Vengeance) && //Vengeance option is enabled
                            ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Vengeance_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Vengeance_SubOption == 1 || //Vengeance is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Vengeance_SubOption == 2))) //Vengeance is enabled for bosses only
                            return OriginalHook(Vengeance);

                        // Rampart
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Rampart) && //Rampart option is enabled
                            ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Rampart_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Rampart_SubOption == 1 || //Rampart is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Rampart_SubOption == 2))) //Rampart is enabled for bosses only
                            return All.Rampart;

                        // Thrill
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Thrill) && //Thrill option is enabled
                            ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Thrill_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Thrill_SubOption == 1 || //Thrill is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Thrill_SubOption == 2))) //Thrill is enabled for bosses only
                            return ThrillOfBattle;

                        // Equilibrium
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Equilibrium) && //Equilibrium option is enabled
                            ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Equilibrium_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Equilibrium_SubOption == 1 || //Equilibrium is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Equilibrium_SubOption == 2))) //Equilibrium is enabled for bosses only
                            return Equilibrium;

                        // Bloodwhetting
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Bloodwhetting) && //Bloodwhetting option is enabled
                            ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Bloodwhetting_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Bloodwhetting_SubOption == 1 || //Bloodwhetting is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Bloodwhetting_SubOption == 2))) //Bloodwhetting is enabled for bosses only
                            return OriginalHook(RawIntuition);
                    }

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Infuriate) && InCombat() && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && !HasEffect(Buffs.InnerReleaseStacks) && gauge <= infuriateGauge && CanWeave(actionID))
                        return Infuriate;

                    //Sub Mythril Tempest level check
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk) && !LevelChecked(MythrilTempest) && InCombat())
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest) && InCombat())
                    {
                        if (CanWeave(actionID))
                        {
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_InnerRelease) && CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && LevelChecked(Berserk))
                                return OriginalHook(Berserk);
                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Orogeny) && ActionReady(Orogeny) && LevelChecked(Orogeny) && HasEffect(Buffs.SurgingTempest))
                                return Orogeny;
                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalWrath) && HasEffect(Buffs.Wrathful) && LevelChecked(PrimalWrath))
                                return PrimalWrath;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend))
                            return PrimalRend;
                        if (IsNotEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRend) && HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend) && GetBuffRemainingTime(Buffs.PrimalRendReady) <= GCD)
                            return PrimalRend;
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && LevelChecked(PrimalRuination) && JustUsed(PrimalRend, 4f))
                            return PrimalRuination;
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Decimate) && LevelChecked(SteelCyclone) && (gauge >= decimateGaugeSpend || HasEffect(Buffs.InnerReleaseStacks) || HasEffect(Buffs.NascentChaos)))
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove == Overpower && LevelChecked(MythrilTempest))
                        {
                            return MythrilTempest;
                        }
                    }

                    return Overpower;
                }

                return actionID;
            }
        }
        #endregion

        internal class War_ST_StormsEye : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.War_ST_StormsEye;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsEye)
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove == HeavySwing && LevelChecked(Maim))
                            return Maim;

                        if (lastComboMove == Maim && LevelChecked(StormsEye))
                            return StormsEye;
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }

        internal class WAR_NascentFlash : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_NascentFlash;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == NascentFlash)
                {
                    if (LevelChecked(NascentFlash))
                        return NascentFlash;
                    return RawIntuition;
                }

                return actionID;
            }
        }


        internal class WAR_ST_Advanced_PrimalCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Advanced_PrimalRend;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == InnerBeast || actionID == SteelCyclone)
                {
                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                    if (LevelChecked(PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady) && JustUsed(PrimalRend))
                        return PrimalRuination;
                }

                //fell cleave or decimate
                return OriginalHook(actionID);
            }
        }

        internal class WAR_InfuriateFellCleave : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_InfuriateFellCleave;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is InnerBeast or FellCleave or SteelCyclone or Decimate)
                {
                    var rageGauge = GetJobGauge<WARGauge>();
                    var rageThreshold = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateRange);
                    var hasNascent = HasEffect(Buffs.NascentChaos);
                    var hasInnerRelease = HasEffect(Buffs.InnerReleaseStacks);

                    if (InCombat() && rageGauge.BeastGauge <= rageThreshold && ActionReady(Infuriate) && !hasNascent
                    && ((!hasInnerRelease) || IsNotEnabled(CustomComboPreset.WAR_InfuriateFellCleave_IRFirst)))
                        return OriginalHook(Infuriate);
                }

                return actionID;
            }
        }

        internal class WAR_PrimalCombo_InnerRelease : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_PrimalCombo_InnerRelease;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Berserk or InnerRelease)
                {
                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                    if (LevelChecked(PrimalRuination) && HasEffect(Buffs.PrimalRuinationReady))
                        return PrimalRuination;
                }

                return actionID;
            }
        }
    }
}
