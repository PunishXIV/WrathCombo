#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.PCT.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    #region Variables
    internal static PCTGauge gauge = GetJobGauge<PCTGauge>();
    internal static bool HasPaint => gauge.Paint > 0;
    internal static bool CreatureMotifReady => !gauge.CreatureMotifDrawn && LevelChecked(CreatureMotif) && !HasStatusEffect(Buffs.StarryMuse);
    internal static bool WeaponMotifReady => !gauge.WeaponMotifDrawn && LevelChecked(WeaponMotif) && !HasStatusEffect(Buffs.StarryMuse) && !HasStatusEffect(Buffs.HammerTime);
    internal static bool LandscapeMotifReady => !gauge.LandscapeMotifDrawn && LevelChecked(LandscapeMotif) && !HasStatusEffect(Buffs.StarryMuse);
    internal static float ScenicCD => GetCooldownRemainingTime(StarryMuse);
    internal static float SteelCD => GetCooldownRemainingTime(StrikingMuse);
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
    
    #region OGCD Spells
    private static bool TryOGCDSpells(Combo flags, ref uint actionID)
    {
        #region Enables
        bool subtractivePaletteEnabled =
             flags.HasFlag(Combo.Simple) ||
             IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_SubtractivePalette) ||
             IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_SubtractivePalette);
        
        bool scenicMuseEnabled = 
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_ScenicMuse) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_ScenicMuse);
        
        bool livingMuseEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_LivingMuse) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_LivingMuse);
        
        bool steelMuseEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_SteelMuse) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_SteelMuse);
        
        bool hammerStampMovementEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MovementOption_HammerStampCombo) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo);
        
        bool portraitEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MogOfTheAges) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MogOfTheAges);
        
        bool LucidDreamingEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_LucidDreaming) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_LucidDreaming);
        
        #endregion
        
        #region Configs
        int scenicThresholdST = PCT_ST_AdvancedMode_ScenicMuse_SubOption == 1 || !InBossEncounter() ? PCT_ST_AdvancedMode_ScenicMuse_Threshold : 0; //Boss Check
        int scenicThresholdAoE = PCT_AoE_AdvancedMode_ScenicMuse_SubOption == 1 || !InBossEncounter() ? PCT_AoE_AdvancedMode_ScenicMuse_Threshold : 0; //Boss Check
        int scenicStop = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? scenicThresholdST : scenicThresholdAoE;
        
        bool scenicMovementPrevention = 
            flags.HasFlag(Combo.Simple) ? false : 
            flags.HasFlag(Combo.ST) ? PCT_ST_AdvancedMode_ScenicMuse_MovementOption : PCT_AoE_AdvancedMode_ScenicMuse_MovementOption;
        
        int lucidThreshold = 
            flags.HasFlag(Combo.Simple) ? 6500 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_AdvancedMode_LucidOption : PCT_AoE_AdvancedMode_LucidOption;
        
        int burnBossThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_AdvancedMode_BurnBoss : PCT_AoE_AdvancedMode_BurnBoss;
        
        bool scenicMuseReady = ActionReady(ScenicMuse) && gauge.LandscapeMotifDrawn; 
        bool livingMuseReady = ActionReady(LivingMuse) && gauge.CreatureMotifDrawn;
        bool steelMuseReady = ActionReady(SteelMuse)  && gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime);
        bool portraitReady = ActionReady(MogoftheAges) && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady); //Check for either portrait being ready
        bool paletteReady = LevelChecked(SubtractivePalette) && 
                            !HasStatusEffect(Buffs.SubtractivePalette) && !HasStatusEffect(Buffs.MonochromeTones) && //Don't overwrite self of comet in black
                                         (HasStatusEffect(Buffs.SubtractiveSpectrum) || //Free use from Starry Muse
                                          gauge.PalleteGauge >= 50 && ScenicCD > 35 || //Use freely before pooling
                                          gauge.PalleteGauge == 100 && HasStatusEffect(Buffs.Aetherhues2)||  //Pool but don't overcap
                                          gauge.PalleteGauge >= 50 && ScenicCD < 3 && scenicMuseEnabled); //Use As it is time to start buff window

        bool almostCappedOrCappedSteelMuse = GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
            GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) - 1 && GetCooldownChargeRemainingTime(SteelMuse) < 3;
       
        #endregion
        
        if (InCombat() && HasBattleTarget())
        {
            // SubtractivePalette
            if (subtractivePaletteEnabled && CanWeave() && paletteReady)
            {
                actionID = SubtractivePalette;
                return true;
            }

            // ScenicMuse
            if (scenicMuseEnabled && scenicMuseReady && CanDelayedWeave() && 
                GetTargetHPPercent() > scenicStop && //Threshold Check
                (!IsMoving() || !scenicMovementPrevention)) //Movement Prevention
            {
                actionID = OriginalHook(ScenicMuse);
                return true;
            }

            // LivingMuse
            if (livingMuseEnabled && livingMuseReady && CanWeave() && 
                !JustUsed(StarryMuse) && //Buff propagation issue prevention
                (!portraitReady || GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse)) && //Overcap Prevention
                (TargetIsBoss() && GetTargetHPPercent() < burnBossThreshold || //Burn Boss Threshold
                 !LevelChecked(ScenicMuse) || //Low Level no Scenic
                 ScenicCD > GetCooldownChargeRemainingTime(LivingMuse) || // Hold for buffs
                 !scenicMuseEnabled)) //Dont Hold for Buffs
            {
                actionID = OriginalHook(LivingMuse);
                return true;
            }

            // SteelMuse
            if (steelMuseEnabled && steelMuseReady && 
                (TargetIsBoss() && GetTargetHPPercent() < burnBossThreshold || //Burn Boss Threshold
                 HasStatusEffect(Buffs.StarryMuse) && CanWeave() || //Use in burst if you need to
                 hammerStampMovementEnabled && IsMoving() && ScenicCD >= 30  || //Use When Moving but not if itll get in way of burst
                 !hammerStampMovementEnabled && ScenicCD > SteelCD && ScenicCD >= 40|| //
                 almostCappedOrCappedSteelMuse && CanWeave() || //Use because Capped
                 ScenicCD < 40 && SteelCD < 40 && ScenicCD > SteelCD || //Use charge before the burst prep
                 !LevelChecked(ScenicMuse) && CanWeave())) //Low Level no Scenic
            {
                actionID = OriginalHook(SteelMuse);
                return true;
            }

            // Portrait Mog or Madeen
            if (portraitEnabled && portraitReady && CanWeave() && IsOffCooldown(OriginalHook(MogoftheAges)) &&
                !JustUsed(StarryMuse) && //Buff propagation issue prevention
                (TargetIsBoss() && GetTargetHPPercent() < burnBossThreshold || //Burn Boss Threshold
                 ScenicCD >= 60 || //wait for scenic
                 !LevelChecked(ScenicMuse) || //Low Level no Scenic
                 !scenicMuseEnabled)) //Hold for Buffs
            {
                actionID = OriginalHook(MogoftheAges);
                return true;
            }
        }
        //LucidDreaming
        if (LucidDreamingEnabled && Role.CanLucidDream(lucidThreshold))
        {
            actionID = Role.LucidDreaming;
            return true;
        }
        return false;
    }
    #endregion
    
    #region Mitigation
    private static bool TryMitigation(Combo flags, ref uint actionID)
    {
        #region Enables
        bool addleEnabled =
            flags.HasFlag(Combo.Simple) || IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_Addle);

        bool tempuraEnabled =
            flags.HasFlag(Combo.Simple) || IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_Tempura);
        #endregion

        if (addleEnabled && Role.CanAddle() && CanWeave() && GroupDamageIncoming() && HasBattleTarget())
        {
            actionID = Role.Addle;
            return true;
        }

        if (tempuraEnabled && CanWeave() && GroupDamageIncoming() && !JustUsed(Role.Addle, 6))
        {
            if (LevelChecked(TempuraCoat) && IsOffCooldown(TempuraCoat))
            {
                actionID = TempuraCoat;
                return true;
            }
                    
            if (LevelChecked(TempuraGrassa) && IsInParty() &&
                NumberOfAlliesInRange(TempuraGrassa) >= GetPartyMembers().Count * .75 && //75% of group in range for Spreading your Tempura
                HasStatusEffect(Buffs.TempuraCoat))
            {
                actionID = TempuraGrassa;
                return true;
            }
        }
        return false;
    }
    #endregion
    
    #region Movement

    private static bool TryMovementOption(Combo flags, ref uint actionID)
    {
        #region Enables
        bool movementEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MovementFeature) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MovementFeature);

        bool rainbowDripEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_RainbowDrip) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_RainbowDrip);
        
        bool starPrismEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_StarPrism) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_StarPrism);
            
        bool hammerStampEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MovementOption_HammerStampCombo) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo);
            
        bool cometInBlackEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MovementOption_CometinBlack) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MovementOption_CometinBlack);
            
        bool holyInWhiteEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MovementOption_HolyInWhite) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MovementOption_HolyInWhite);

        bool swiftcastEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_SwiftcastOption) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_SwiftcastOption);

        #endregion

        if (!movementEnabled || !IsMoving() || !InCombat()) return false; //Quick Bailout
        
        if (rainbowDripEnabled && HasStatusEffect(Buffs.RainbowBright)) //Needs to be here in case you are moving in back half of Burst window
        {
            actionID = OriginalHook(RainbowDrip);
            return true;
        }

        if (hammerStampEnabled && LevelChecked(HammerStamp) && !HasStatusEffect(Buffs.Hyperphantasia) &&
            HasStatusEffect(Buffs.HammerTime))
        {
            actionID = OriginalHook(HammerStamp);
            return true;
        }
        
        if (starPrismEnabled && HasStatusEffect(Buffs.Starstruck)) //Move with Starstruck, will spend Hyper Fantasia
        {
            actionID = StarPrism;
            return true;
        }

        if (cometInBlackEnabled && HasStatusEffect(Buffs.MonochromeTones) && HasPaint) //Move with Comet, will spend Hyper Fantasia
        {
            actionID = OriginalHook(CometinBlack);
            return true;
        }

        if (swiftcastEnabled && ActionReady(Role.Swiftcast) && !HasStatusEffect(Buffs.StarryMuse) &&
            (CreatureMotifReady || WeaponMotifReady || LandscapeMotifReady))
        {
            actionID = Role.Swiftcast;
            return true;
        }
        
        if (holyInWhiteEnabled && HasPaint) //Move with Holy, will spend Hyper Fantasia though not ideal. Sit still kids
        {
            actionID = OriginalHook(HolyInWhite);
            return true;
        }
        return false;
    }
    #endregion
    
    #region GCD Spells
    private static bool TryGCDSpells(Combo flags, ref uint actionID)
    {
        #region Enables
        bool starPrismEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_StarPrism) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_StarPrism);
        
        bool rainbowDripEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_RainbowDrip) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_RainbowDrip);
        
        bool cometInBlackEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_CometinBlack) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_CometinBlack);
        
        bool hammerStampComboEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_HammerStampCombo) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_HammerStampCombo);
        
        bool scenicMuseEnabled = 
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_ScenicMuse) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_ScenicMuse);
        #endregion
        
        #region Configs
        float TimeRemainingToUseHammer= 
            flags.HasFlag(Combo.Simple) ? 30 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_AdvancedMode_HammerStampCombo_Timing : PCT_AoE_AdvancedMode_HammerStampCombo_Timing;
        
        int burnBossThreshold = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_AdvancedMode_BurnBoss : PCT_AoE_AdvancedMode_BurnBoss;
            
        #endregion
        
        //Star Prism
        if (starPrismEnabled && HasStatusEffect(Buffs.Starstruck) && 
            !JustUsed(StarryMuse)) //Buff propagation issue prevention
        {
            actionID = StarPrism;
            return true;
        }

        //Rainbow Drip
        if (rainbowDripEnabled && HasStatusEffect(Buffs.RainbowBright)) 
        {
            actionID = RainbowDrip;
            return true;
        }
       
        //Comet in Black
        if (cometInBlackEnabled && HasStatusEffect(Buffs.MonochromeTones) && HasPaint && 
            !JustUsed(StarryMuse) && //Buff propagation issue prevention
            (!HasStatusEffect(Buffs.StarryMuse) || HasStatusEffect(Buffs.Hyperphantasia)) && //Only use for hyperfantasia in the window
            (ScenicCD > 10 || !LevelChecked(ScenicMuse) || !scenicMuseEnabled)) //Hold for Buffs if close
        {
            actionID = OriginalHook(CometinBlack);
            return true;
        }
        
        //Hammer Stamp Combo
        if (hammerStampComboEnabled && ActionReady(OriginalHook(HammerStamp)) &&
            !HasStatusEffect(Buffs.Hyperphantasia) && //Dont use until hyperfantasia is spent
            (ScenicCD >= 10 || !LevelChecked(ScenicMuse)) &&  // Dont use if close to window. 
            (TargetIsBoss() && GetTargetHPPercent() < burnBossThreshold || //Burn Boss Threshold
             HasStatusEffect(Buffs.StarryMuse) || //Use in window
             GetStatusEffectRemainingTime(Buffs.HammerTime) <= TimeRemainingToUseHammer || //Use when time is almost up on Hammer time
             ScenicCD <= 30)) //But dont hold so long you mess with burst prep
        {
            actionID = OriginalHook(HammerStamp);
            return true;
        }
        
        return false;
    }
    #endregion
    
    #region Motifs
    private static bool TryDrawMotif(Combo flags, ref uint actionID)
    {
        #region Enables
        bool motifsEnabled = 
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_MotifFeature) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_MotifFeature);
            
        bool prepullEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_PrePullMotifs) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_PrePullMotifs);
        
        bool noTargetEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_NoTargetMotifs) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_NoTargetMotifs);
        
        bool swiftcastEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_SwiftMotifs) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_SwiftMotifs);
        
        bool creatureEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_CreatureMotif) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_CreatureMotif);
        
        bool weaponEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_WeaponMotif) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_WeaponMotif);
        
        bool landscapeEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_LandscapeMotif) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_LandscapeMotif);
        
        #endregion
        
        #region Configs
        
        int creatureStop = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_CreatureStop : PCT_AoE_CreatureStop;
            
        bool creatureHealthCheck = GetTargetHPPercent() > creatureStop;
        bool hasLivingMuseCharges = HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8;
        
        int weaponStop = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_WeaponStop : PCT_AoE_WeaponStop;
            
        bool weaponHealthCheck = GetTargetHPPercent() > weaponStop;
        bool hasSteelMuseCharges = HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8;
        
        int landscapeStop = 
            flags.HasFlag(Combo.Simple) ? 0 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_LandscapeStop : PCT_AoE_LandscapeStop;
            
        bool landscapeHealthCheck = GetTargetHPPercent() > landscapeStop;
        
        #endregion
        
        if (motifsEnabled)
        {
            if (creatureEnabled && CreatureMotifReady &&
                (prepullEnabled && !InCombat() || //Prepull Motifs
                 noTargetEnabled && InCombat() && CurrentTarget == null || //Downtime Motifs
                 swiftcastEnabled && HasStatusEffect(Role.Buffs.Swiftcast) && creatureHealthCheck || //Swiftcast Motifs
                 LevelChecked(ScenicMuse) && ScenicCD <= 20 && creatureHealthCheck || //Burst Prep
                 hasLivingMuseCharges && creatureHealthCheck)) //Standard Use
            {
                actionID = OriginalHook(CreatureMotif);
                return true;
            }

            if (weaponEnabled && WeaponMotifReady &&
                (prepullEnabled && !InCombat() || //Prepull Motifs
                 noTargetEnabled && InCombat() && CurrentTarget == null || //Downtime Motifs
                 swiftcastEnabled && HasStatusEffect(Role.Buffs.Swiftcast) && weaponHealthCheck || //Swiftcast Motifs
                 LevelChecked(ScenicMuse) && ScenicCD <= 20 && weaponHealthCheck || //Burst Prep
                 hasSteelMuseCharges && weaponHealthCheck)) //Standard Use
            {
                actionID = OriginalHook(WeaponMotif);
                return true;
            }

            if (landscapeEnabled && LandscapeMotifReady &&
                (prepullEnabled && !InCombat() || //Prepull Motifs
                 noTargetEnabled && InCombat() && CurrentTarget == null || //Downtime Motifs
                 swiftcastEnabled && HasStatusEffect(Role.Buffs.Swiftcast) && landscapeHealthCheck || //Swiftcast Motifs
                 LevelChecked(ScenicMuse) && ScenicCD <= 20 && landscapeHealthCheck)) //Standard Use is Burst prep
            {
                actionID = OriginalHook(LandscapeMotif);
                return true;
            }
        }
        return false;
    }
    #endregion
    
    #region SubCombos and Holy in White
    private static bool TryCombos(Combo flags, ref uint actionID)
    {
        #region Enables
        bool subComboEnabled = 
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_BlizzardInCyan) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_BlizzardInCyan);
            
        bool holyInWhiteEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.PCT_ST_AdvancedMode_HolyinWhite) ||
            IsAoEEnabled(flags, Preset.PCT_AoE_AdvancedMode_HolyinWhite);
        #endregion
        
        #region Configs
        int holdPaintCharges =
            flags.HasFlag(Combo.Simple) ? 2 : 
            flags.HasFlag(Combo.ST) ? PCT_ST_AdvancedMode_HolyinWhiteOption : PCT_AoE_AdvancedMode_HolyinWhiteOption;
                
        #endregion

        if (flags.HasFlag(Combo.ST))
        {
            if (subComboEnabled && HasStatusEffect(Buffs.SubtractivePalette))
            {
                actionID = OriginalHook(BlizzardinCyan);
                return true;
            }
            if (holyInWhiteEnabled && !HasStatusEffect(Buffs.MonochromeTones) && 
                gauge.Paint > holdPaintCharges && //Charge retention check
                NumberOfEnemiesInRange(HolyInWhite) > 1) //Only use on 2 or more targets for a gain
            {
                actionID = OriginalHook(HolyInWhite);
                return true;
            }
            return false;
        }
        
        if (flags.HasFlag(Combo.AoE))
        {
            if (subComboEnabled && HasStatusEffect(Buffs.SubtractivePalette))
            {
                actionID = OriginalHook(BlizzardIIinCyan);
                return true;
            }

            if (holyInWhiteEnabled && !HasStatusEffect(Buffs.MonochromeTones) && 
                gauge.Paint > holdPaintCharges) //Charge retention check
            {
                actionID = OriginalHook(HolyInWhite);
                return true;
            }
        }
        return false;
    }
    #endregion
    
    #endregion

    #region ID's
    public const uint
        BlizzardinCyan = 34653,
        StoneinYellow = 34654,
        BlizzardIIinCyan = 34659,
        ClawMotif = 34666,
        ClawedMuse = 34672,
        CometinBlack = 34663,
        CreatureMotif = 34689,
        FireInRed = 34650,
        AeroInGreen = 34651,
        WaterInBlue = 34652,
        FireIIinRed = 34656,
        AeroIIinGreen = 34657,
        HammerMotif = 34668,
        WingedMuse = 34671,
        StrikingMuse = 34674,
        StarryMuse = 34675,
        HammerStamp = 34678,
        HammerBrush = 34679,
        PolishingHammer = 34680,
        HolyInWhite = 34662,
        StarrySkyMotif = 34669,
        LandscapeMotif = 34691,
        LivingMuse = 35347,
        MawMotif = 34667,
        MogoftheAges = 34676,
        PomMotif = 34664,
        PomMuse = 34670,
        RainbowDrip = 34688,
        RetributionoftheMadeen = 34677,
        ScenicMuse = 35349,
        Smudge = 34684,
        StarPrism = 34681,
        SteelMuse = 35348,
        SubtractivePalette = 34683,
        StoneIIinYellow = 34660,
        TempuraCoat = 34685,
        TempuraGrassa = 34686,
        ThunderIIinMagenta = 34661,
        ThunderinMagenta = 34655,
        WaterinBlue = 34652,
        WeaponMotif = 34690,
        WingMotif = 34665;

    public static class Buffs
    {
        public const ushort
            SubtractivePalette = 3674,
            Aetherhues2 = 3676,
            RainbowBright = 3679,
            HammerTime = 3680,
            MonochromeTones = 3691,
            StarryMuse = 3685,
            TempuraCoat = 3686,
            Hyperphantasia = 3688,
            Inspiration = 3689,
            SubtractiveSpectrum = 3690,
            Starstruck = 3681;
    }

    public static class Debuffs
    {

    }
    #endregion

    #region Openers
    internal static PCT2ndStarryMaxLvl SecondStarryMaxLvl = new();
    internal static PCT3rdStarryMaxLvl ThirdStarryMaxLvl = new();
    internal static PCT2ndStarryLvl90 SecondStarryLvl90 = new();
    internal static PCT3rdStarryLvl90 ThirdStarryLvl90 = new();
    
    internal static WrathOpener Opener()
    {
        if (SecondStarryLvl90.LevelChecked && PCT_Opener_Choice == 0)
            return SecondStarryLvl90;

        if (ThirdStarryLvl90.LevelChecked && PCT_Opener_Choice == 1)
            return ThirdStarryLvl90;
        
        if (SecondStarryMaxLvl.LevelChecked && PCT_Opener_Choice == 0)
            return SecondStarryMaxLvl;
        
        if (ThirdStarryMaxLvl.LevelChecked && PCT_Opener_Choice == 1)
            return ThirdStarryMaxLvl;
        
        return WrathOpener.Dummy;
    }
    
    public static bool HasMotifs()
    {

        if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Pom))
            return false;

        if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Weapon))
            return false;

        if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Landscape))
            return false;

        return true;
    }
    
    internal class PCT2ndStarryMaxLvl : WrathOpener
    {
        //2nd GCD Starry Opener
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        public override List<uint> OpenerActions { get; set; } =
        [
            RainbowDrip,
            PomMuse,
            StrikingMuse,
            WingMotif,
            StarryMuse, //5
            HammerStamp,
            SubtractivePalette,
            BlizzardinCyan,
            StoneinYellow,
            ThunderinMagenta,//10
            CometinBlack,
            WingedMuse,
            MogoftheAges,
            StarPrism,
            HammerBrush,//15
            PolishingHammer,
            RainbowDrip,
            Role.Swiftcast,
            ClawMotif,
            ClawedMuse,//20
        ];
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;
        public override Preset Preset => Preset.PCT_ST_Advanced_Openers;
        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            5
        ];
        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
[
            ([8, 9, 10], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
            ([8, 9, 10], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([8, 9, 10], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([11], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
        ];
        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([17], () => !HasStatusEffect(Buffs.RainbowBright))];

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(StarryMuse))
                return false;

            if (GetRemainingCharges(LivingMuse) < 3)
                return false;

            if (GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!HasMotifs())
                return false;

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(Role.Swiftcast))
                return false;

            return true;
        }
    }
    
    internal class PCT3rdStarryMaxLvl : WrathOpener
    {
        //3rd GCD Starry Opener
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        public override List<uint> OpenerActions { get; set; } =
        [
            RainbowDrip,
            StrikingMuse,
            HolyInWhite,
            PomMuse,
            WingMotif, //5 
            StarryMuse,
            HammerStamp,
            SubtractivePalette,
            BlizzardinCyan,
            BlizzardinCyan, //10
            BlizzardinCyan,
            CometinBlack,
            WingedMuse,
            MogoftheAges,
            StarPrism, //15
            HammerBrush,
            PolishingHammer,
            RainbowDrip,
            FireInRed,
            Role.Swiftcast, //20
            ClawMotif,
            ClawedMuse
        ];
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;
        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            6
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([18], () => !HasStatusEffect(Buffs.RainbowBright))];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([3], CometinBlack, () => HasStatusEffect(Buffs.MonochromeTones)),
            ([9, 10, 11], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
             ([9, 10, 11], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([9, 10, 11], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([12], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
        ];
        public override Preset Preset => Preset.PCT_ST_Advanced_Openers;
        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(StarryMuse))
                return false;

            if (GetRemainingCharges(LivingMuse) < 3)
                return false;

            if (GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!HasMotifs())
                return false;

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(Role.Swiftcast))
                return false;

            return true;
        }
    }
    
     internal class PCT2ndStarryLvl90 : WrathOpener
    {
        //2nd GCD Starry Opener
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 90;
        
        public override List<uint> OpenerActions { get; set; } =
        [
            FireInRed,
            StrikingMuse,
            AeroInGreen,
            StarryMuse,
            HammerStamp,
            PomMuse,
            SubtractivePalette,
            WingMotif,
            WingedMuse,
            HammerBrush,
            MogoftheAges,
            PolishingHammer,
            ThunderinMagenta,
            BlizzardinCyan,
            StoneinYellow,//15
            CometinBlack,
            WaterInBlue,
            FireInRed//20
        ];
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
[
            ([13, 14, 15], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
            ([13, 14, 15], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([13, 14, 15], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([16], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
        ];
        public override Preset Preset => Preset.PCT_ST_Advanced_Openers;
        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(StarryMuse))
                return false;

            if (GetRemainingCharges(LivingMuse) < 2)
                return false;

            if (GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!HasMotifs())
                return false;

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return false;

            return true;
        }
    }
     
     internal class PCT3rdStarryLvl90 : WrathOpener
    {
        //3rd GCD Starry Opener
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 90;
        public override List<uint> OpenerActions { get; set; } =
        [
            FireInRed,
            StrikingMuse,
            AeroInGreen,
            WaterInBlue,
            StarryMuse,
            HammerStamp,
            PomMuse,
            SubtractivePalette,
            WingMotif,
            WingedMuse,
            HammerBrush,
            MogoftheAges,
            PolishingHammer,
            BlizzardinCyan,
            StoneinYellow,
            ThunderinMagenta,
            CometinBlack,
            FireInRed,
            AeroInGreen,
            Role.Swiftcast, //20
            WaterInBlue
        ];
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;
        
        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([14, 15, 16], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
             ([14, 15, 16], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([14,15,16], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([17], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
        ];
        public override Preset Preset => Preset.PCT_ST_Advanced_Openers;
        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(StarryMuse))
                return false;

            if (GetRemainingCharges(LivingMuse) < 2)
                return false;

            if (GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!HasMotifs())
                return false;

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(Role.Swiftcast))
                return false;

            return true;
        }
    }
     
#endregion
}