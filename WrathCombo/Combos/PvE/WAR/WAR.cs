using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    #region Simple Mode - Single Target
    internal class WAR_ST_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not HeavySwing)
                return actionID; //Our button

            byte gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
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
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;

            if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                CanWeave() &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                return Variant.VariantCure;
            #endregion

            #region Mitigations
            if (Config.WAR_ST_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //Holmgang
                    if (ActionReady(Holmgang) && //Holmgang is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return Holmgang;

                    if (IsPlayerTargeted())
                    {
                        //Vengeance / Damnation
                        if (ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Vengeance);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            InActionRange(All.Reprisal) && //Player is in range of Reprisal
                            PlayerHealthPercentageHp() < 90) //Player's health is below 90%
                            return All.Reprisal;
                    }

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
            }
            #endregion

            if (LevelChecked(Tomahawk) && //Tomahawk is available
                !InMeleeRange() && //not in melee range
                HasBattleTarget()) //has a target
                return Tomahawk;

            if (CanWeave()) //in weave window
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
                if (CanWeave()) //in weave window
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
                        if (!IsMoving() && //not moving
                            GetTargetDistance() <= 1 && //within 1y of target
                            (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease))) //IR is not ready or available
                            return Onslaught;
                    }
                }

                if (HasEffect(Buffs.PrimalRendReady) && //has Primal Rend ready
                    !JustUsed(InnerRelease) && //has not just used IR
                    !IsMoving() && //not moving
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

            if (ComboTimer > 0) //in combo window
            {
                if (LevelChecked(InnerBeast) && //Inner Beast is available
                    gauge >= 90 && //gauge is greater than or equal to 90
                    (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest))) //does not have Storm's Eye or has Surging Tempest
                    return OriginalHook(InnerBeast);

                if (LevelChecked(Maim) && //Maim is available
                    ComboAction == HeavySwing) //last combo move was Heavy Swing
                    return Maim;

                if (LevelChecked(StormsPath) && //Storm's Path is available
                    ComboAction == Maim) //last combo move was Maim
                {
                    if (LevelChecked(StormsEye) && //Storm's Eye is available
                        GetBuffRemainingTime(Buffs.SurgingTempest) <= 29) //Surging Tempest is about to expire
                        return StormsEye;
                    return StormsPath;
                }
            }

            return HeavySwing;
        }
    }
    #endregion

    #region Advanced Mode - Single Target
    internal class WAR_ST_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not HeavySwing)
                return actionID; //Our button

            byte gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
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
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;

            if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                CanWeave() &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                return Variant.VariantCure;
            #endregion

            #region Mitigations
            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Mitigation) && //Mitigation option is enabled
                InCombat() && //Player is in combat
                !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
            {
                //Holmgang
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Holmgang) && //Holmgang option is enabled
                    ActionReady(Holmgang) && //Holmgang is ready
                    PlayerHealthPercentageHp() <= Config.WAR_ST_Holmgang_Health && //Player's health is below selected threshold
                    (Config.WAR_ST_Holmgang_SubOption == 0 || //Holmgang is enabled for all targets
                     (TargetIsBoss() && Config.WAR_ST_Holmgang_SubOption == 1))) //Holmgang is enabled for bosses only
                    return Holmgang;

                if (IsPlayerTargeted())
                {
                    //Vengeance / Damnation
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Vengeance) && //Vengeance option is enabled
                        ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                        PlayerHealthPercentageHp() <= Config.WAR_ST_Vengeance_Health && //Player's health is below selected threshold
                        (Config.WAR_ST_Vengeance_SubOption == 0 || //Vengeance is enabled for all targets
                         (TargetIsBoss() && Config.WAR_ST_Vengeance_SubOption == 1))) //Vengeance is enabled for bosses only
                        return OriginalHook(Vengeance);

                    //Rampart
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Rampart) && //Rampart option is enabled
                        ActionReady(All.Rampart) && //Rampart is ready
                        PlayerHealthPercentageHp() <= Config.WAR_ST_Rampart_Health && //Player's health is below selected threshold
                        (Config.WAR_ST_Rampart_SubOption == 0 || //Rampart is enabled for all targets
                         (TargetIsBoss() && Config.WAR_ST_Rampart_SubOption == 1))) //Rampart is enabled for bosses only
                        return All.Rampart;

                    //Reprisal
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Reprisal) && //Reprisal option is enabled
                        ActionReady(All.Reprisal) && //Reprisal is ready
                        InActionRange(All.Reprisal) && //Player is in range of Reprisal
                        PlayerHealthPercentageHp() <= Config.WAR_ST_Reprisal_Health && //Player's health is below selected threshold
                        (Config.WAR_ST_Reprisal_SubOption == 0 || //Reprisal is enabled for all targets
                         (TargetIsBoss() && Config.WAR_ST_Reprisal_SubOption == 1))) //Reprisal is enabled for bosses only
                        return All.Reprisal;

                    //Arms Length
                    if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_ArmsLength) && //Arms Length option is enabled
                        ActionReady(All.ArmsLength) && //Arms Length is ready
                        PlayerHealthPercentageHp() <= Config.WAR_ST_ArmsLength_Health && //Player's health is below selected threshold
                        !InBossEncounter()) //target is not a boss
                        return All.ArmsLength;
                }

                //Thrill
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Thrill) && //Thrill option is enabled
                    ActionReady(ThrillOfBattle) && //Thrill is ready
                    PlayerHealthPercentageHp() <= Config.WAR_ST_Thrill_Health && //Player's health is below selected threshold
                    (Config.WAR_ST_Thrill_SubOption == 0 || //Thrill is enabled for all targets
                     (TargetIsBoss() && Config.WAR_ST_Thrill_SubOption == 1))) //Thrill is enabled for bosses only
                    return ThrillOfBattle;

                //Equilibrium
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Equilibrium) && //Equilibrium option is enabled
                    ActionReady(Equilibrium) && //Equilibrium is ready
                    PlayerHealthPercentageHp() <= Config.WAR_ST_Equilibrium_Health && //Player's health is below selected threshold
                    (Config.WAR_ST_Equilibrium_SubOption == 0 || //Equilibrium is enabled for all targets
                     (TargetIsBoss() && Config.WAR_ST_Equilibrium_SubOption == 1))) //Equilibrium is enabled for bosses only
                    return Equilibrium;

                //Bloodwhetting
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_Bloodwhetting) && //Bloodwhetting option is enabled
                    ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting is ready
                    PlayerHealthPercentageHp() <= Config.WAR_AoE_Bloodwhetting_Health && //Player's health is below selected threshold
                    (Config.WAR_AoE_Bloodwhetting_SubOption == 0 || //Bloodwhetting is enabled for all targets
                     (TargetIsBoss() && Config.WAR_AoE_Bloodwhetting_SubOption == 1))) //Bloodwhetting is enabled for bosses only
                    return OriginalHook(RawIntuition);
            }
            #endregion

            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_RangedUptime) && //Ranged uptime option is enabled
                LevelChecked(Tomahawk) && //Tomahawk is available
                !InMeleeRange() && //not in melee range
                HasBattleTarget()) //has a target
                return Tomahawk;

            if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_BalanceOpener) &&
                ContentCheck.IsInConfiguredContent(Config.WAR_BalanceOpener_Content, ContentCheck.ListSet.BossOnly))
            {
                if (Opener().FullOpener(ref actionID))
                    return actionID;
            }

            if (CanWeave()) //in weave window
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
                if (CanWeave()) //in weave window
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
                             !IsMoving() && GetTargetDistance() <= 1 && //not moving and within 1y of target
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
                        !IsMoving() && GetTargetDistance() <= 1) //not moving & within 1y of target
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

            if (ComboTimer > 0) //in combo window
            {
                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_FellCleave) && //Fell Cleave option is enabled
                    LevelChecked(InnerBeast) && //Inner Beast is available
                    (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && //does not have Storm's Eye or has Surging Tempest
                    gauge >= Config.WAR_FellCleaveGauge) //gauge is greater than or equal to selected threshold
                    return OriginalHook(InnerBeast);

                if (LevelChecked(Maim) && //Maim is available
                    ComboAction == HeavySwing) //last combo move was Heavy Swing
                    return Maim;

                if (IsEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye) && //Storm's Eye option is enabled
                    LevelChecked(StormsPath) &&  //Storm's Path is available
                    ComboAction == Maim) //last combo move was Maim
                {
                    if (LevelChecked(StormsEye) && //Storm's Eye is available
                        GetBuffRemainingTime(Buffs.SurgingTempest) <= Config.WAR_SurgingRefreshRange) //Surging Tempest less than or equal to selected threshold
                        return StormsEye; //Storm's Eye
                    return StormsPath; //Storm's Path instead if conditions are not met
                }
                if (IsNotEnabled(CustomComboPreset.WAR_ST_Advanced_StormsEye) && //Storm's Eye option is disabled
                    LevelChecked(StormsPath) && //Storm's Path is available
                    ComboAction == Maim) //last combo move was Maim
                    return StormsPath;
            }

            return HeavySwing;
        }
    }
    #endregion

    #region Simple Mode - AoE
    internal class WAR_AoE_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Overpower)
                return actionID; //Our button

            byte gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
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
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;

            if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                CanWeave() &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                return Variant.VariantCure;
            #endregion

            #region Mitigations
            if (Config.WAR_AoE_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //Holmgang
                    if (ActionReady(Holmgang) && //Holmgang is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return Holmgang;

                    if (IsPlayerTargeted())
                    {
                        //Vengeance / Damnation
                        if (ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Vengeance);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            GetTargetDistance() <= 5 && //within 5y of target
                            PlayerHealthPercentageHp() < 90) //Player's health is below 90%
                            return All.Reprisal;
                    }

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
            }
            #endregion

            if (CanWeave()) //in weave window
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
                if (CanWeave()) //in weave window
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

            if (ComboTimer > 0) //in combo window
            {
                if (LevelChecked(MythrilTempest) && //Mythril Tempest is available
                    ComboAction == Overpower) //last combo move was Overpower
                    return MythrilTempest;
            }

            return Overpower;
        }
    }
    #endregion

    #region Advanced Mode - AoE
    internal class WAR_AoE_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Overpower)
                return actionID; //Our button

            byte gauge = GetJobGauge<WARGauge>().BeastGauge; //WAR gauge
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
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;

            if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                CanWeave() &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                return Variant.VariantCure;
            #endregion

            #region Mitigations
            if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Mitigation) && //Mitigation option is enabled
                InCombat() && //Player is in combat
                !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
            {
                //Holmgang
                if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Holmgang) && //Holmgang option is enabled
                    ActionReady(Holmgang) && //Holmgang is ready
                    PlayerHealthPercentageHp() <= Config.WAR_AoE_Holmgang_Health && //Player's health is below selected threshold
                    (Config.WAR_AoE_Holmgang_SubOption == 0 || //Holmgang is enabled for all targets
                     (TargetIsBoss() && Config.WAR_AoE_Holmgang_SubOption == 1))) //Holmgang is enabled for bosses only
                    return Holmgang;

                if (IsPlayerTargeted())
                {
                    //Vengeance / Damnation
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Vengeance) && //Vengeance option is enabled
                        ActionReady(OriginalHook(Vengeance)) && //Vengeance is ready
                        PlayerHealthPercentageHp() <= Config.WAR_AoE_Vengeance_Health && //Player's health is below selected threshold
                        (Config.WAR_AoE_Vengeance_SubOption == 0 || //Vengeance is enabled for all targets
                         (TargetIsBoss() && Config.WAR_AoE_Vengeance_SubOption == 1))) //Vengeance is enabled for bosses only
                        return OriginalHook(Vengeance);

                    //Rampart
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Rampart) && //Rampart option is enabled
                        ActionReady(All.Rampart) && //Rampart is ready
                        PlayerHealthPercentageHp() <= Config.WAR_AoE_Rampart_Health && //Player's health is below selected threshold
                        (Config.WAR_AoE_Rampart_SubOption == 0 || //Rampart is enabled for all targets
                         (TargetIsBoss() && Config.WAR_AoE_Rampart_SubOption == 1))) //Rampart is enabled for bosses only
                        return All.Rampart;

                    //Reprisal
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Reprisal) && //Reprisal option is enabled
                        ActionReady(All.Reprisal) && //Reprisal is ready
                        GetTargetDistance() <= 5 && //within 5y of target
                        PlayerHealthPercentageHp() <= Config.WAR_AoE_Reprisal_Health && //Player's health is below selected threshold
                        (Config.WAR_AoE_Reprisal_SubOption == 0 || //Reprisal is enabled for all targets
                         (TargetIsBoss() && Config.WAR_AoE_Reprisal_SubOption == 1))) //Reprisal is enabled for bosses only
                        return All.Reprisal;

                    //Arms Length
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_ArmsLength) && //Arms Length option is enabled
                        ActionReady(All.ArmsLength) && //Arms Length is ready
                        PlayerHealthPercentageHp() <= Config.WAR_AoE_ArmsLength_Health && //Player's health is below selected threshold
                        !InBossEncounter()) //target is not a boss
                        return All.ArmsLength;
                }
                //Thrill
                if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Thrill) && //Thrill option is enabled
                    ActionReady(ThrillOfBattle) && //Thrill is ready
                    PlayerHealthPercentageHp() <= Config.WAR_AoE_Thrill_Health && //Player's health is below selected threshold
                    (Config.WAR_AoE_Thrill_SubOption == 0 || //Thrill is enabled for all targets
                     (TargetIsBoss() && Config.WAR_AoE_Thrill_SubOption == 1))) //Thrill is enabled for bosses only
                    return ThrillOfBattle;

                //Equilibrium
                if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Equilibrium) && //Equilibrium option is enabled
                    ActionReady(Equilibrium) && //Equilibrium is ready
                    PlayerHealthPercentageHp() <= Config.WAR_AoE_Equilibrium_Health && //Player's health is below selected threshold
                    (Config.WAR_AoE_Equilibrium_SubOption == 0 || //Equilibrium is enabled for all targets
                     (TargetIsBoss() && Config.WAR_AoE_Equilibrium_SubOption == 1))) //Equilibrium is enabled for bosses only
                    return Equilibrium;

                //Bloodwhetting
                if (IsEnabled(CustomComboPreset.WAR_AoE_Advanced_Bloodwhetting) && //Bloodwhetting option is enabled
                    ActionReady(OriginalHook(RawIntuition)) && //Bloodwhetting is ready
                    PlayerHealthPercentageHp() <= Config.WAR_AoE_Bloodwhetting_Health && //Player's health is below selected threshold
                    (Config.WAR_AoE_Bloodwhetting_SubOption == 0 || //Bloodwhetting is enabled for all targets
                     (TargetIsBoss() && Config.WAR_AoE_Bloodwhetting_SubOption == 1))) //Bloodwhetting is enabled for bosses only
                    return OriginalHook(RawIntuition);
            }
            #endregion

            if (CanWeave()) //in weave window
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
                if (CanWeave()) //in weave window
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

            if (ComboTimer > 0) //in combo window
            {
                if (LevelChecked(MythrilTempest) && //Mythril Tempest is available
                    ComboAction == Overpower) //last combo move was Overpower
                    return MythrilTempest;
            }

            return Overpower;
        }
    }
    #endregion

    #region Storm's Eye -> Storm's Path
    internal class WAR_EyePath : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_EyePath;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != StormsPath)
                return actionID;

            if (GetBuffRemainingTime(Buffs.SurgingTempest) <= Config.WAR_EyePath_Refresh) //Surging Tempest less than or equal to selected threshold
                return StormsEye;

            return actionID;
        }
    }
    #endregion

    #region Storm's Eye Combo -> Storm's Eye
    internal class WAR_StormsEye : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_StormsEye;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not StormsEye)
                return actionID;

            if (ComboTimer > 0) //In combo
            {
                if (ComboAction == HeavySwing && //Last move was Heavy Swing
                    LevelChecked(Maim)) //Maim is available
                    return Maim;

                if (ComboAction == Maim && //Last move was Maim
                    LevelChecked(StormsEye)) //Storm's Eye is available
                    return StormsEye;
            }

            return HeavySwing;
        }
    }
    #endregion

    #region Primal Combo -> Inner Release
    internal class WAR_PrimalCombo_InnerRelease : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_PrimalCombo_InnerRelease;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is not (Berserk or InnerRelease))
                    return OriginalHook(actionID);

            if (LevelChecked(PrimalRend) && //Primal Rend is available
                HasEffect(Buffs.PrimalRendReady)) //Primal Rend is ready
                return PrimalRend;

            if (LevelChecked(PrimalRuination) && //Primal Ruination is available
                HasEffect(Buffs.PrimalRuinationReady)) //Primal Ruination is ready
                return PrimalRuination;

            return OriginalHook(actionID);
        }
    }
    #endregion

    #region Infuriate -> Fell Cleave / Decimate
    internal class WAR_InfuriateFellCleave : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_InfuriateFellCleave;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (InnerBeast or FellCleave or SteelCyclone or Decimate))
                return actionID;

            WARGauge gauge = GetJobGauge<WARGauge>();
            bool hasNascent = HasEffect(Buffs.NascentChaos);
            bool hasInnerRelease = HasEffect(Buffs.InnerReleaseStacks);

            if (InCombat() && //is in combat
                gauge.BeastGauge <= Config.WAR_InfuriateRange && //Beast Gauge is below selected threshold
                ActionReady(Infuriate) && //Infuriate is ready
                !hasNascent //does not have Nascent Chaos
                && ((!hasInnerRelease) || IsNotEnabled(CustomComboPreset.WAR_InfuriateFellCleave_IRFirst))) //does not have Inner Release stacks or IRFirst option is disabled
                return OriginalHook(Infuriate);

            return actionID;
        }
    }
    #endregion

    #region One-Button Mitigation
    internal class WAR_Mit_OneButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ThrillOfBattle)
                return actionID; //Our button

            if (IsEnabled(CustomComboPreset.WAR_Mit_Holmgang_Max) &&
                ActionReady(Holmgang) &&
                PlayerHealthPercentageHp() <= Config.WAR_Mit_Holmgang_Health &&
                ContentCheck.IsInConfiguredContent(
                    Config.WAR_Mit_Holmgang_Difficulty,
                    Config.WAR_Mit_Holmgang_DifficultyListSet
                ))
                return Holmgang;

            foreach (int priority in Config.WAR_Mit_Priorities.Items.OrderBy(x => x))
            {
                int index = Config.WAR_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }
    #endregion

    #region Nascent Flash -> Raw Intuition
    internal class WAR_NascentFlash : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_NascentFlash;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not NascentFlash)
                return actionID;

            if (LevelChecked(NascentFlash))
                return NascentFlash;
            return RawIntuition;
        }
    }
    #endregion

    #region Equilibrium -> Thrill of Battle
    internal class WAR_ThrillEquilibrium : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ThrillEquilibrium;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ThrillOfBattle)
                return actionID;

            if (!IsEnabled(CustomComboPreset.WAR_ThrillEquilibrium_BuffOnly) &&
                IsOnCooldown(ThrillOfBattle))
                return Equilibrium;

            if (IsEnabled(CustomComboPreset.WAR_ThrillEquilibrium_BuffOnly) &&
                HasEffect(Buffs.ThrillOfBattle))
                return Equilibrium;

            return ThrillOfBattle;
        }
    }
    #endregion
}
