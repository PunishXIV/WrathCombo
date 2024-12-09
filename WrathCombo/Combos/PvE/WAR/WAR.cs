using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvE
{
    internal partial class WAR
    {
        public const byte ClassID = 3; //Marauder (MRD) 
        public const byte JobID = 21; //Warrior (WAR)
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
        internal class WAR_ST_Simple : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsPath) //Our button
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    #region Variant
                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) &&
                        IsEnabled(Variant.VariantUltimatum) &&
                        CanWeave(actionID) &&
                        ActionReady(Variant.VariantUltimatum))
                        return Variant.VariantUltimatum;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        CanWeave(actionID) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    #endregion

                    #region Mitigations
                    if (InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        //Holmgang
                        if (ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                            return Holmgang;

                        //Vengeance / Damnation
                        if (ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Vengeance);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Thrill
                        if (ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                            return ThrillOfBattle;

                        //Equilibrium
                        if (ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < 50) //Player's health is below 30%
                            return Equilibrium;

                        //Bloodwhetting
                        if (ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting
                            PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                            return OriginalHook(Bloodwhetting);
                    }
                    #endregion

                    if (LevelChecked(Tomahawk) && //Tomahawk is available
                        !InMeleeRange() && //not in melee range
                        HasBattleTarget()) //has a target
                        return Tomahawk;

                    if (CanWeave(actionID)) //in weave window
                    {
                        if (InCombat() && //in combat
                            ActionReady(Infuriate) && //Infuriate is ready
                            !HasEffect(Buffs.NascentChaos) && //does not have Nascent Chaos
                            !HasEffect(Buffs.InnerReleaseStacks) && //does not have Inner Release stacks
                            gauge <= 40) //gauge is less than or equal to 40
                            return Infuriate;

                        //pre-Surging Tempest IR
                        if (InCombat() && //in combat
                            ActionReady(OriginalHook(Berserk)) && //Berserk is ready
                            !LevelChecked(StormsEye)) //does not have Storm's Eye
                            return OriginalHook(Berserk);
                    }

                    if (HasEffect(Buffs.SurgingTempest) && //has Surging Tempest
                        InCombat()) //in combat
                    {
                        if (CanWeave(actionID)) //in weave window
                        {
                            if (ActionReady(OriginalHook(Berserk))) //Berserk is ready
                                return OriginalHook(Berserk);

                            if (ActionReady(Upheaval)) //Upheaval is ready
                                return Upheaval;

                            if (LevelChecked(PrimalWrath) && //Primal Wrath is available
                                HasEffect(Buffs.Wrathful)) //has Wrathful
                                return PrimalWrath;

                            if (LevelChecked(Onslaught) && //Onslaught is available
                                GetRemainingCharges(Onslaught) > 1) //has more than 1 charge
                            {
                                if (!IsMoving && //not moving
                                    GetTargetDistance() <= 1 && //within 1y of target
                                    (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease))) //IR is not ready or available
                                    return Onslaught;
                            }
                        }

                        if (HasEffect(Buffs.PrimalRendReady) && //has Primal Rend ready
                            !JustUsed(InnerRelease) && //has not just used IR
                            !IsMoving && //not moving
                            GetTargetDistance() <= 1) //within 1y of target
                            return PrimalRend;

                        if (HasEffect(Buffs.PrimalRuinationReady) && //has Primal Ruination ready
                            LevelChecked(PrimalRuination)) //Primal Ruination is available
                            return PrimalRuination;

                        if (LevelChecked(InnerBeast)) //Inner Beast is available
                        {
                            if (HasEffect(Buffs.InnerReleaseStacks) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos))) //has IR stacks or Nascent Chaos and Inner Chaos
                                return OriginalHook(InnerBeast);

                            if (HasEffect(Buffs.NascentChaos) && //has Nascent Chaos
                                !LevelChecked(InnerChaos) && //does not have Inner Chaos
                                gauge >= 50) //gauge is greater than or equal to 50
                                return OriginalHook(Decimate);
                        }
                    }

                    if (comboTime > 0) //in combo window
                    {
                        if (LevelChecked(InnerBeast) && //Inner Beast is available
                            gauge >= 90 && //gauge is greater than or equal to 90
                            (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest))) //does not have Storm's Eye or has Surging Tempest
                            return OriginalHook(InnerBeast);

                        if (LevelChecked(Maim) && //Maim is available
                            lastComboMove == HeavySwing) //last combo move was Heavy Swing
                            return Maim;

                        if (LevelChecked(StormsPath) && //Storm's Path is available
                            lastComboMove == Maim) //last combo move was Maim
                        {
                            if (LevelChecked(StormsEye) && //Storm's Eye is available
                                GetBuffRemainingTime(Buffs.SurgingTempest) <= 29) //Surging Tempest is about to expire
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
        internal class WAR_ST_Advanced : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsPath) //Our button
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    #region Variant
                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) &&
                        IsEnabled(Variant.VariantUltimatum) &&
                        CanWeave(actionID) &&
                        ActionReady(Variant.VariantUltimatum))
                        return Variant.VariantUltimatum;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        CanWeave(actionID) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    #endregion

                    #region Mitigations
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Mitigation) && //Mitigation option is enabled
                        InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        //Holmgang
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Holmgang) && //Holmgang option is enabled
                            ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Holmgang_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Holmgang_SubOption == 1 || //Holmgang is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Holmgang_SubOption == 2))) //Holmgang is enabled for bosses only
                            return Holmgang;

                        //Vengeance / Damnation
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Vengeance) && //Vengeance option is enabled
                            ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Vengeance_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Vengeance_SubOption == 1 || //Vengeance is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Vengeance_SubOption == 2))) //Vengeance is enabled for bosses only
                            return OriginalHook(Vengeance);

                        //Rampart
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Rampart) && //Rampart option is enabled
                            ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Rampart_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Rampart_SubOption == 1 || //Rampart is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Rampart_SubOption == 2))) //Rampart is enabled for bosses only
                            return All.Rampart;

                        //Thrill
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Thrill) && //Thrill option is enabled
                            ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Thrill_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Thrill_SubOption == 1 || //Thrill is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Thrill_SubOption == 2))) //Thrill is enabled for bosses only
                            return ThrillOfBattle;

                        //Equilibrium
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Equilibrium) && //Equilibrium option is enabled
                            ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < Config.WAR_ST_Equilibrium_Health && //Player's health is below selected threshold
                            (Config.WAR_ST_Equilibrium_SubOption == 1 || //Equilibrium is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_ST_Equilibrium_SubOption == 2))) //Equilibrium is enabled for bosses only
                            return Equilibrium;

                        //Bloodwhetting
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Bloodwhetting) && //Bloodwhetting option is enabled
                            ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Bloodwhetting_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Bloodwhetting_SubOption == 1 || //Bloodwhetting is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Bloodwhetting_SubOption == 2))) //Bloodwhetting is enabled for bosses only
                            return OriginalHook(RawIntuition);
                    }
                    #endregion

                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_RangedUptime) && //Ranged uptime option is enabled
                        LevelChecked(Tomahawk) && //Tomahawk is available
                        !InMeleeRange() && //not in melee range
                        HasBattleTarget()) //has a target
                        return Tomahawk;

                    if (CanWeave(actionID)) //in weave window
                    {
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Infuriate) && //Infuriate option is enabled
                            InCombat() && //in combat
                            ActionReady(Infuriate) && //Infuriate is ready
                            !HasEffect(Buffs.NascentChaos) && //does not have Nascent Chaos
                            !HasEffect(Buffs.InnerReleaseStacks) && //does not have Inner Release stacks
                            gauge <= Config.WAR_InfuriateSTGauge && //gauge is less than or equal to selected threshold
                            GetRemainingCharges(Infuriate) > Config.WAR_KeepInfuriateCharges) //has more than selected charges
                            return Infuriate;

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_InnerRelease) && //Inner Release option is enabled 
                            InCombat() && //in combat
                            ActionReady(OriginalHook(Berserk)) && //Berserk is ready
                            !LevelChecked(StormsEye)) //does not have Storm's Eye
                            return OriginalHook(Berserk);
                    }

                    if (InCombat() && //in combat
                        HasEffect(Buffs.SurgingTempest)) //has Surging Tempest
                    {
                        if (CanWeave(actionID)) //in weave window
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_InnerRelease) && //Inner Release option is enabled
                                ActionReady(OriginalHook(Berserk))) //Berserk is ready
                                return OriginalHook(Berserk);

                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Upheaval) && //Upheaval option is enabled
                                ActionReady(Upheaval)) //Upheaval is ready
                                return Upheaval;

                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalWrath) && //Primal Wrath option is enabled
                                LevelChecked(PrimalWrath) && //Primal Wrath is available
                                HasEffect(Buffs.Wrathful)) //has Wrathful
                                return PrimalWrath;

                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught) && //Onslaught option is enabled
                                LevelChecked(Onslaught) && //Onslaught is available
                                GetRemainingCharges(Onslaught) > Config.WAR_KeepOnslaughtCharges) //has more than selected charges
                            {
                                if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught_MeleeSpender) || //Melee spender option is disabled
                                    (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Onslaught_MeleeSpender) && //Melee spender option is enabled
                                    !IsMoving && GetTargetDistance() <= 1 && //not moving and within 1y of target
                                    (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease)))) //IR is not ready or available
                                    return Onslaught;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend) && //Primal Rend option is enabled
                            HasEffect(Buffs.PrimalRendReady) && //has Primal Rend ready
                            !JustUsed(InnerRelease)) //has not just used IR
                        {
                            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend_Late) && //Primal Rend late option is enabled
                                GetBuffStacks(Buffs.InnerReleaseStacks) is 0 && //does not have IR stacks
                                GetBuffStacks(Buffs.BurgeoningFury) is 0 && //does not have Burgeoning Fury stacks
                                !HasEffect(Buffs.Wrathful)) //does not have Wrathful
                                return PrimalRend;

                            if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRend_Late) && //Primal Rend late option is disabled
                                !IsMoving && GetTargetDistance() <= 1) //not moving & within 1y of target
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_PrimalRuination) && //Primal Ruination option is enabled
                            LevelChecked(PrimalRuination) && //Primal Ruination is available
                            HasEffect(Buffs.PrimalRuinationReady)) //has Primal Ruination ready
                            return PrimalRuination;

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && //Fell Cleave option is enabled
                            LevelChecked(InnerBeast)) //Inner Beast is available
                        {
                            if (HasEffect(Buffs.InnerReleaseStacks) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos))) //has IR stacks or Nascent Chaos and Inner Chaos
                                return OriginalHook(InnerBeast);

                            if (HasEffect(Buffs.NascentChaos) && //has Nascent Chaos
                                !LevelChecked(InnerChaos) && //Inner Chaos is not available
                                gauge >= 50) //gauge is greater than or equal to 50
                                return OriginalHook(Decimate);
                        }
                    }

                    if (comboTime > 0) //in combo window
                    {
                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && //Fell Cleave option is enabled
                            LevelChecked(InnerBeast) && //Inner Beast is available
                            (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && //does not have Storm's Eye or has Surging Tempest
                            gauge >= Config.WAR_FellCleaveGauge) //gauge is greater than or equal to selected threshold
                            return OriginalHook(InnerBeast);

                        if (LevelChecked(Maim) && //Maim is available
                            lastComboMove == HeavySwing) //last combo move was Heavy Swing
                            return Maim;

                        if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye) && //Storm's Eye option is enabled
                            LevelChecked(StormsPath) &&  //Storm's Path is available
                            lastComboMove == Maim) //last combo move was Maim
                        {
                            if (LevelChecked(StormsEye) && //Storm's Eye is available
                                GetBuffRemainingTime(Buffs.SurgingTempest) <= Config.WAR_SurgingRefreshRange) //Surging Tempest less than or equal to selected threshold
                                return StormsEye; //Storm's Eye
                            return StormsPath; //Storm's Path instead if conditions are not met
                        }
                        if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye) && //Storm's Eye option is disabled
                            LevelChecked(StormsPath) && //Storm's Path is available
                            lastComboMove == Maim) //last combo move was Maim
                            return StormsPath;
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }
        #endregion

        #region Simple Mode - AoE
        internal class WAR_AoE_Simple : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower) //Our button
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    #region Variant
                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) &&
                        IsEnabled(Variant.VariantUltimatum) &&
                        CanWeave(actionID) &&
                        ActionReady(Variant.VariantUltimatum))
                        return Variant.VariantUltimatum;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        CanWeave(actionID) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    #endregion

                    #region Mitigations
                    if (InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        //Holmgang
                        if (ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                            return Holmgang;

                        //Vengeance / Damnation
                        if (ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Vengeance);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Thrill
                        if (ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                            return ThrillOfBattle;

                        //Equilibrium
                        if (ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < 50) //Player's health is below 30%
                            return Equilibrium;

                        //Bloodwhetting
                        if (ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting
                            PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                            return OriginalHook(Bloodwhetting);
                    }
                    #endregion

                    if (CanWeave(actionID)) //in weave window
                    {
                        if (InCombat() && //in combat
                           ActionReady(Infuriate) && //Infuriate is ready
                           !HasEffect(Buffs.NascentChaos) && //does not have Nascent Chaos
                           !HasEffect(Buffs.InnerReleaseStacks) && //does not have Inner Release stacks 
                           gauge <= 40) //gauge is less than or equal to 40
                            return Infuriate;

                        if (InCombat() && //in combat
                            ActionReady(OriginalHook(Berserk)) && //Berserk is ready 
                            !LevelChecked(MythrilTempest)) //does not have Mythril Tempest
                            return OriginalHook(Berserk);
                    }

                    if (InCombat() && //in combat
                        HasEffect(Buffs.SurgingTempest)) //has Surging Tempest
                    {
                        if (CanWeave(actionID)) //in weave window
                        {
                            if (ActionReady(OriginalHook(Berserk))) //Berserk is ready
                                return OriginalHook(Berserk);

                            if (ActionReady(Orogeny)) //Orogeny is ready
                                return Orogeny;

                            if (LevelChecked(PrimalWrath) && //Primal Wrath is available
                                HasEffect(Buffs.Wrathful)) //has Wrathful
                                return PrimalWrath;
                        }

                        if (LevelChecked(PrimalRend) && //Primal Rend is available
                            HasEffect(Buffs.PrimalRendReady)) //has Primal Rend ready
                            return PrimalRend;

                        if (LevelChecked(PrimalRuination) && //Primal Ruination is available
                            HasEffect(Buffs.PrimalRuinationReady)) //has Primal Ruination ready
                            return PrimalRuination;

                        if (LevelChecked(SteelCyclone) && //Steel Cyclone is available
                            (gauge >= 90 || HasEffect(Buffs.InnerReleaseStacks) || HasEffect(Buffs.NascentChaos))) //gauge is greater than or equal to 90 or has IR stacks or Nascent Chaos
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0) //in combo window
                    {
                        if (LevelChecked(MythrilTempest) && //Mythril Tempest is available
                            lastComboMove == Overpower) //last combo move was Overpower
                            return MythrilTempest;
                    }

                    return Overpower;
                }

                return actionID;
            }
        }
        #endregion

        #region Advanced Mode - AoE
        internal class WAR_AoE_Advanced : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Overpower) //Our button
                {
                    var gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
                    bool justMitted = JustUsed(OriginalHook(RawIntuition), 4f) ||
                                      JustUsed(OriginalHook(Vengeance), 5f) ||
                                      JustUsed(ThrillOfBattle, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Holmgang, 9f);

                    #region Variant
                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                    if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                        IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) &&
                        IsEnabled(Variant.VariantUltimatum) &&
                        CanWeave(actionID) &&
                        ActionReady(Variant.VariantUltimatum))
                        return Variant.VariantUltimatum;

                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        CanWeave(actionID) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;
                    #endregion

                    #region Mitigations
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Mitigation) && //Mitigation option is enabled
                        InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        //Holmgang
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Holmgang) && //Holmgang option is enabled
                            ActionReady(Holmgang) && //Holmgang is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Holmgang_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Holmgang_SubOption == 1 || //Holmgang is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Holmgang_SubOption == 2))) //Holmgang is enabled for bosses only
                            return Holmgang;

                        //Vengeance / Damnation
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Vengeance) && //Vengeance option is enabled
                            ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Vengeance_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Vengeance_SubOption == 1 || //Vengeance is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Vengeance_SubOption == 2))) //Vengeance is enabled for bosses only
                            return OriginalHook(Vengeance);

                        //Rampart
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Rampart) && //Rampart option is enabled
                            ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Rampart_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Rampart_SubOption == 1 || //Rampart is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Rampart_SubOption == 2))) //Rampart is enabled for bosses only
                            return All.Rampart;

                        //Thrill
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Thrill) && //Thrill option is enabled
                            ActionReady(ThrillOfBattle) && //Thrill is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Thrill_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Thrill_SubOption == 1 || //Thrill is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Thrill_SubOption == 2))) //Thrill is enabled for bosses only
                            return ThrillOfBattle;

                        //Equilibrium
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Equilibrium) && //Equilibrium option is enabled
                            ActionReady(Equilibrium) && //Equilibrium is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Equilibrium_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Equilibrium_SubOption == 1 || //Equilibrium is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Equilibrium_SubOption == 2))) //Equilibrium is enabled for bosses only
                            return Equilibrium;

                        //Bloodwhetting
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Bloodwhetting) && //Bloodwhetting option is enabled
                            ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting is ready
                            PlayerHealthPercentageHp() < Config.WAR_AoE_Bloodwhetting_Health && //Player's health is below selected threshold
                            (Config.WAR_AoE_Bloodwhetting_SubOption == 1 || //Bloodwhetting is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.WAR_AoE_Bloodwhetting_SubOption == 2))) //Bloodwhetting is enabled for bosses only
                            return OriginalHook(RawIntuition);
                    }
                    #endregion

                    if (CanWeave(actionID)) //in weave window
                    {
                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Infuriate) && //Infuriate option is enabled
                            InCombat() && //in combat
                            ActionReady(Infuriate) && //Infuriate is ready
                            !HasEffect(Buffs.NascentChaos) && //does not have Nascent Chaos
                            !HasEffect(Buffs.InnerReleaseStacks) && //does not have Inner Release stacks
                            gauge <= Config.WAR_InfuriateSTGauge) //gauge is less than or equal to selected threshold
                            return Infuriate;

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_InnerRelease) && //Inner Release option is enabled
                            InCombat() && //in combat
                            ActionReady(OriginalHook(Berserk)) && //Berserk is ready
                            !LevelChecked(MythrilTempest)) //does not have Mythril Tempest
                            return OriginalHook(Berserk);
                    }

                    if (InCombat() && //in combat
                        HasEffect(Buffs.SurgingTempest)) //has Surging Tempest
                    {
                        if (CanWeave(actionID)) //in weave window
                        {
                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_InnerRelease) && //Inner Release option is enabled
                                ActionReady(OriginalHook(Berserk))) //Berserk is ready
                                return OriginalHook(Berserk);

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Orogeny) && //Orogeny option is enabled
                                ActionReady(Orogeny)) //Orogeny is ready
                                return Orogeny;

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalWrath) && //Primal Wrath option is enabled
                                LevelChecked(PrimalWrath) && //Primal Wrath is available
                                HasEffect(Buffs.Wrathful)) //has Wrathful
                                return PrimalWrath;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRend) && //Primal Rend option is enabled
                            LevelChecked(PrimalRend) && //Primal Rend is available
                            HasEffect(Buffs.PrimalRendReady)) //has Primal Rend ready
                            return PrimalRend;

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_PrimalRuination) && //Primal Ruination option is enabled
                            HasEffect(Buffs.PrimalRuinationReady) && //has Primal Ruination ready
                            LevelChecked(PrimalRuination)) //Primal Ruination is available
                            return PrimalRuination;

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Decimate) && //Decimate option is enabled
                            LevelChecked(SteelCyclone) && //Steel Cyclone is available 
                            (gauge >= Config.WAR_DecimateGauge || HasEffect(Buffs.InnerReleaseStacks) || HasEffect(Buffs.NascentChaos))) //gauge is greater than or equal to selected threshold or has IR stacks or Nascent Chaos
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0) //in combo window
                    {
                        if (LevelChecked(MythrilTempest) && //Mythril Tempest is available
                            lastComboMove == Overpower) //last combo move was Overpower
                            return MythrilTempest;
                    }

                    return Overpower;
                }

                return actionID;
            }
        }
        #endregion

        #region Storm's Eye -> Storm's Path
        internal class WAR_EyePath : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_EyePath;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsPath)
                {
                    if (GetBuffRemainingTime(Buffs.SurgingTempest) <= Config.WAR_EyePath_Refresh) //Surging Tempest less than or equal to selected threshold
                        return StormsEye;
                }

                return actionID;
            }
        }
        #endregion


        #region Storm's Eye Combo -> Storm's Eye
        internal class WAR_StormsEye : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_StormsEye;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsEye)
                {
                    if (comboTime > 0) //In combo
                    {
                        if (lastComboMove == HeavySwing && //Last move was Heavy Swing
                            LevelChecked(Maim)) //Maim is available
                            return Maim;

                        if (lastComboMove == Maim && //Last move was Maim
                            LevelChecked(StormsEye)) //Storm's Eye is available
                            return StormsEye;
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }
        #endregion

        #region Primal Combo -> Inner Release
        internal class WAR_PrimalCombo_InnerRelease : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_PrimalCombo_InnerRelease;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == InnerBeast || actionID == SteelCyclone)
                {
                    if (LevelChecked(PrimalRend) && //Primal Rend is available
                        HasEffect(Buffs.PrimalRendReady)) //Primal Rend is ready
                        return PrimalRend;

                    if (LevelChecked(PrimalRuination) && //Primal Ruination is available
                        HasEffect(Buffs.PrimalRuinationReady)) //Primal Ruination is ready
                        return PrimalRuination;
                }

                //fell cleave or decimate
                return OriginalHook(actionID);
            }
        }
        #endregion

        #region Infuriate -> Fell Cleave / Decimate
        internal class WAR_InfuriateFellCleave : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_InfuriateFellCleave;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is InnerBeast or FellCleave or SteelCyclone or Decimate)
                {
                    var gauge = GetJobGauge<WARGauge>();
                    var hasNascent = HasEffect(Buffs.NascentChaos);
                    var hasInnerRelease = HasEffect(Buffs.InnerReleaseStacks);

                    if (InCombat() && //is in combat
                        gauge.BeastGauge <= Config.WAR_InfuriateRange && //Beast Gauge is below selected threshold
                        ActionReady(Infuriate) && //Infuriate is ready
                        !hasNascent //does not have Nascent Chaos
                        && ((!hasInnerRelease) || IsNotEnabled(CustomComboPreset.WAR_InfuriateFellCleave_IRFirst))) //does not have Inner Release stacks or IRFirst option is disabled
                        return OriginalHook(Infuriate);
                }

                return actionID;
            }
        }
        #endregion

        #region One-Button Mitigation
        internal class WAR_Mit_OneButton : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_Mit_OneButton;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is ThrillOfBattle) //Our button
                {
                    if (IsEnabled(CustomComboPreset.WAR_Mit_Holmgang) && //Holmgang option is enabled
                        IsEnabled(CustomComboPreset.WAR_Mit_Holmgang_Max) && //Holmgang Savior option is enabled
                        PlayerHealthPercentageHp() <= Config.WAR_Mit_Holmgang_Health && //Player's health is below selected threshold
                        ActionReady(Holmgang)) //Holmgang is ready
                        return Holmgang;

                    if (IsEnabled(CustomComboPreset.WAR_Mit_ThrillOfBattleFirst) && //ThrillOfBattle First option is enabled
                        ActionReady(ThrillOfBattle)) //ThrillOfBattle is ready
                        return ThrillOfBattle;

                    if (IsEnabled(CustomComboPreset.WAR_Mit_Bloodwhetting) && //Bloodwhetting option is enabled
                        ActionReady(OriginalHook(RawIntuition))) //Bloodwhetting is ready
                        return OriginalHook(RawIntuition);

                    if (IsEnabled(CustomComboPreset.WAR_Mit_Vengeance) && //Vengeance option is enabled
                        ActionReady(OriginalHook(Vengeance))) //Vengeance is ready
                        return OriginalHook(Vengeance);

                    if (IsEnabled(CustomComboPreset.WAR_Mit_Rampart) && //Rampart option is enabled
                        ActionReady(All.Rampart)) //Rampart is ready
                        return All.Rampart;

                    if (!IsEnabled(CustomComboPreset.WAR_Mit_ThrillOfBattleFirst) && //ThrillOfBattle First option is disabled
                        ActionReady(ThrillOfBattle)) //ThrillOfBattle is ready
                        return ThrillOfBattle;

                    if (IsEnabled(CustomComboPreset.WAR_Mit_Equilibrium) && //Equilibrium option is enabled
                        ActionReady(Equilibrium)) //Equilibrium is ready
                        return Equilibrium;

                    if (IsEnabled(CustomComboPreset.WAR_Mit_Reprisal) && //Reprisal option is enabled
                        GetTargetDistance() <= 5 && //Target is within 5y
                        ActionReady(All.Reprisal)) //Reprisal is ready
                        return All.Reprisal;

                    if (IsEnabled(CustomComboPreset.WAR_Mit_ShakeItOff) && //Shake It Off option is enabled
                        ActionReady(ShakeItOff)) //Shake It Off is ready
                        return ShakeItOff;

                    if (IsEnabled(CustomComboPreset.WAR_Mit_Holmgang) && //Holmgang option is enabled
                        !IsEnabled(CustomComboPreset.WAR_Mit_Holmgang_Max) && //Holmgang Max Priority option is disabled
                        ActionReady(Holmgang)) //Holmgang is ready
                        return Holmgang;
                }
                return actionID;
            }
        }
        #endregion

        #region Nascent Flash -> Raw Intuition
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
        #endregion

        #region Equilibrium -> Thrill of Battle
        internal class WAR_ThrillEquilibrium : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ThrillEquilibrium;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == ThrillOfBattle)
                {
                    if (!IsEnabled(CustomComboPreset.WAR_ThrillEquilibrium_BuffOnly) &&
                        IsOnCooldown(ThrillOfBattle))
                        return Equilibrium;

                    if (IsEnabled(CustomComboPreset.WAR_ThrillEquilibrium_BuffOnly) &&
                        HasEffect(Buffs.ThrillOfBattle))
                        return Equilibrium;

                    return ThrillOfBattle;
                }

                return actionID;
            }
        }
        #endregion
    }
}
