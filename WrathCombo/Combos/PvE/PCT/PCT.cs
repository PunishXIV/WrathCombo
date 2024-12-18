using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    internal class PCT_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is FireInRed)
            {
                bool canWeave = CanSpellWeave(ActionWatching.LastSpell) || CanSpellWeave(actionID);

                // Lvl 100 Opener
                if (StarPrism.LevelChecked())
                {
                    if (Opener().FullOpener(ref actionID))
                        return actionID;
                }
                /* Lvl 92 Opener
                else if (!StarPrism.LevelChecked() && RainbowDrip.LevelChecked())
                {
                    if (PCTOpenerLvl92.DoFullOpener(ref actionID))
                        return actionID;
                }
                // Lvl 90 Opener
                else if (!StarPrism.LevelChecked() && !RainbowDrip.LevelChecked() && CometinBlack.LevelChecked())
                {
                    if (PCTOpenerLvl90.DoFullOpener(ref actionID))
                        return actionID;
                }
                // Lvl 80 Opener
                else if (!StarPrism.LevelChecked() && !CometinBlack.LevelChecked() && HolyInWhite.LevelChecked())
                {
                    if (PCTOpenerLvl80.DoFullOpener(ref actionID))
                        return actionID;
                }
                // Lvl 70 Opener
                else if (!StarPrism.LevelChecked() && !CometinBlack.LevelChecked() && !HolyInWhite.LevelChecked() && StarryMuse.LevelChecked())
                {
                    if (PCTOpenerLvl70.DoFullOpener(ref actionID))
                        return actionID;
                }
                */

                // General Weaves
                if (InCombat() && canWeave)
                {
                    // ScenicMuse

                    if (ScenicMuse.LevelChecked() &&
                        Gauge.LandscapeMotifDrawn &&
                        Gauge.WeaponMotifDrawn &&
                        IsOffCooldown(ScenicMuse))
                    {
                        return OriginalHook(ScenicMuse);
                    }

                    // LivingMuse

                    if (LivingMuse.LevelChecked() &&
                        Gauge.CreatureMotifDrawn &&
                        (!(Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) ||
                        GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse)))
                    {
                        if (HasCharges(OriginalHook(LivingMuse)))
                        {
                            if (!ScenicMuse.LevelChecked() ||
                                GetCooldown(ScenicMuse).CooldownRemaining > GetCooldownChargeRemainingTime(LivingMuse))
                            {
                                return OriginalHook(LivingMuse);
                            }
                        }
                    }

                    // SteelMuse

                    if (SteelMuse.LevelChecked() &&
                        !HasEffect(Buffs.HammerTime) &&
                        Gauge.WeaponMotifDrawn &&
                        HasCharges(OriginalHook(SteelMuse)) &&
                        (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining ||
                            GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
                            !ScenicMuse.LevelChecked()))
                    {
                        return OriginalHook(SteelMuse);
                    }

                    // MogoftheAges

                    if (MogoftheAges.LevelChecked() &&
                        (Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) &&
                        IsOffCooldown(OriginalHook(MogoftheAges)) &&
                        (GetCooldownRemainingTime(StarryMuse) >= 60 || !ScenicMuse.LevelChecked()))
                    {
                        return OriginalHook(MogoftheAges);
                    }

                    // Swiftcast

                    if (IsMoving() &&
                        IsOffCooldown(All.Swiftcast) &&
                        All.Swiftcast.LevelChecked() &&
                        !HasEffect(Buffs.HammerTime) &&
                        Gauge.Paint < 1 &&
                        (!Gauge.CreatureMotifDrawn || !Gauge.WeaponMotifDrawn || !Gauge.LandscapeMotifDrawn))
                    {
                        return All.Swiftcast;
                    }

                    // SubtractivePalette

                    if (SubtractivePalette.LevelChecked() &&
                        !HasEffect(Buffs.SubtractivePalette) &&
                        !HasEffect(Buffs.MonochromeTones))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) || Gauge.PalleteGauge >= 50)
                        {
                            return SubtractivePalette;
                        }
                    }
                }

                // Swiftcast Motifs
                if (HasEffect(All.Buffs.Swiftcast))
                {
                    if (!Gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(CreatureMotif);
                    if (!Gauge.WeaponMotifDrawn && HammerMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(HammerMotif);
                    if (!Gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(LandscapeMotif);
                }

                // IsMoving logic
                if (IsMoving() && InCombat())
                {
                    if (HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (CometinBlack.LevelChecked() && Gauge.Paint >= 1 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) <= 3f))
                        return RainbowDrip;

                    if (HolyInWhite.LevelChecked() && Gauge.Paint >= 1)
                        return OriginalHook(HolyInWhite);
                }

                //Prepare for Burst
                if (GetCooldownRemainingTime(ScenicMuse) <= 20)
                {
                    if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn)
                        return OriginalHook(LandscapeMotif);

                    if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn)
                        return OriginalHook(CreatureMotif);

                    if (WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                        return OriginalHook(WeaponMotif);
                }

                // Burst 
                if (HasEffect(Buffs.StarryMuse))
                {

                    if (CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0)
                        return CometinBlack;

                    if (HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.Starstruck))
                        return OriginalHook(HammerStamp);

                    if (HasEffect(Buffs.Starstruck) || (HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) <= 3f))
                        return StarPrism;

                    if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) <= 3f))
                        return RainbowDrip;

                }

                if (HasEffect(Buffs.RainbowBright) && !HasEffect(Buffs.StarryMuse))
                    return RainbowDrip;

                if (CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 30f)
                    return OriginalHook(CometinBlack);

                if (HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (!HasEffect(Buffs.StarryMuse))
                {
                    // LandscapeMotif

                    if (LandscapeMotif.LevelChecked() &&
                        !Gauge.LandscapeMotifDrawn &&
                        GetCooldownRemainingTime(ScenicMuse) <= 20)
                    {
                        return OriginalHook(LandscapeMotif);
                    }

                    // CreatureMotif

                    if (CreatureMotif.LevelChecked() &&
                        !Gauge.CreatureMotifDrawn &&
                        (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                    {
                        return OriginalHook(CreatureMotif);
                    }

                    // WeaponMotif

                    if (WeaponMotif.LevelChecked() &&
                        !HasEffect(Buffs.HammerTime) &&
                        !Gauge.WeaponMotifDrawn &&
                        (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                    {
                        return OriginalHook(WeaponMotif);
                    }
                }

                if (All.LucidDreaming.LevelChecked() && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer?.CurrentMp <= 6500)
                    return All.LucidDreaming;

                if (BlizzardIIinCyan.LevelChecked() && HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardinCyan);
            }

            return actionID;
        }
    }

    internal class PCT_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is FireInRed)
            {
                bool canWeave = CanSpellWeave(ActionWatching.LastSpell) || CanSpellWeave(actionID);
                int creatureStop = PluginConfiguration.GetCustomIntValue(Config.PCT_ST_CreatureStop);
                int landscapeStop = PluginConfiguration.GetCustomIntValue(Config.PCT_ST_LandscapeStop);
                int weaponStop = PluginConfiguration.GetCustomIntValue(Config.PCT_ST_WeaponStop);

                // Variant Cure
                if (IsEnabled(CustomComboPreset.PCT_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= GetOptionValue(Config.PCT_VariantCure))
                    return Variant.VariantCure;

                // Variant Rampart
                if (IsEnabled(CustomComboPreset.PCT_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    canWeave)
                    return Variant.VariantRampart;

                // Prepull logic
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_PrePullMotifs))
                {
                    if (!InCombat() || (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_NoTargetMotifs) && InCombat() && CurrentTarget == null))
                    {
                        if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn)
                            return OriginalHook(CreatureMotif);
                        if (WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                            return OriginalHook(WeaponMotif);
                        if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(LandscapeMotif);
                    }
                }

                // Check if Openers are enabled and determine which opener to execute based on current level
                if (IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers))
                {
                    // Lvl 100 Opener
                    if (StarPrism.LevelChecked())
                    {
                        if (Opener().FullOpener(ref actionID))
                            return actionID;
                    }
                    /* Lvl 92 Opener
                    else if (!StarPrism.LevelChecked() && RainbowDrip.LevelChecked())
                    {
                        if (PCTOpenerLvl92.DoFullOpener(ref actionID))
                            return actionID;
                    }
                    // Lvl 90 Opener
                    else if (!StarPrism.LevelChecked() && !RainbowDrip.LevelChecked() && CometinBlack.LevelChecked())
                    {
                        if (PCTOpenerLvl90.DoFullOpener(ref actionID))
                            return actionID;
                    }
                    // Lvl 80 Opener
                    else if (!StarPrism.LevelChecked() && !CometinBlack.LevelChecked() && HolyInWhite.LevelChecked())
                    {
                        if (PCTOpenerLvl80.DoFullOpener(ref actionID))
                            return actionID;
                    }
                    // Lvl 70 Opener
                    else if (!StarPrism.LevelChecked() && !CometinBlack.LevelChecked() && !HolyInWhite.LevelChecked() && StarryMuse.LevelChecked())
                    {
                        if (PCTOpenerLvl70.DoFullOpener(ref actionID))
                            return actionID;
                    }
                    */
                }

                // General Weaves
                if (InCombat() && canWeave)
                {
                    // ScenicMuse
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_ScenicMuse))
                    {
                        if (ScenicMuse.LevelChecked() &&
                            Gauge.LandscapeMotifDrawn &&
                            Gauge.WeaponMotifDrawn &&
                            IsOffCooldown(ScenicMuse))
                        {
                            return OriginalHook(ScenicMuse);
                        }
                    }

                    // LivingMuse
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LivingMuse))
                    {
                        if (LivingMuse.LevelChecked() &&
                            Gauge.CreatureMotifDrawn &&
                            (!(Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) ||
                            GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse)))
                        {
                            if (HasCharges(OriginalHook(LivingMuse)))
                            {
                                if (!ScenicMuse.LevelChecked() ||
                                    GetCooldown(ScenicMuse).CooldownRemaining > GetCooldownChargeRemainingTime(LivingMuse))
                                {
                                    return OriginalHook(LivingMuse);
                                }
                            }
                        }
                    }

                    // SteelMuse
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SteelMuse))
                    {
                        if (SteelMuse.LevelChecked() &&
                            !HasEffect(Buffs.HammerTime) &&
                            Gauge.WeaponMotifDrawn &&
                            HasCharges(OriginalHook(SteelMuse)) &&
                            (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining ||
                             GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
                             !ScenicMuse.LevelChecked()))
                        {
                            return OriginalHook(SteelMuse);
                        }
                    }

                    // MogoftheAges
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MogOfTheAges))
                    {
                        if (MogoftheAges.LevelChecked() &&
                            (Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) &&
                            IsOffCooldown(OriginalHook(MogoftheAges)) &&
                            (GetCooldownRemainingTime(StarryMuse) >= 60 || !ScenicMuse.LevelChecked()))
                        {
                            return OriginalHook(MogoftheAges);
                        }
                    }

                    // SubtractivePalette
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SubtractivePalette))
                    {
                        if (SubtractivePalette.LevelChecked() &&
                            !HasEffect(Buffs.SubtractivePalette) &&
                            !HasEffect(Buffs.MonochromeTones))
                        {
                            if (HasEffect(Buffs.SubtractiveSpectrum) || Gauge.PalleteGauge >= 50)
                            {
                                return SubtractivePalette;
                            }
                        }
                    }
                }

                // Swiftcast Motifs
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SwiftMotifs) &&
                    HasEffect(All.Buffs.Swiftcast))
                {
                    if (!Gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse) && GetTargetHPPercent() > creatureStop)
                        return OriginalHook(CreatureMotif);
                    if (!Gauge.WeaponMotifDrawn && WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.StarryMuse) && GetTargetHPPercent() > weaponStop)
                        return OriginalHook(WeaponMotif);
                    if (!Gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse) && GetTargetHPPercent() > landscapeStop)
                        return OriginalHook(LandscapeMotif);

                }

                // IsMoving logic
                if (IsMoving() && InCombat())
                {
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HammerStampCombo) && HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_CometinBlack) && CometinBlack.LevelChecked() && Gauge.Paint >= 1 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_RainbowDrip))
                    {
                        if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) <= 3f))
                            return RainbowDrip;
                    }

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HolyInWhite) && HolyInWhite.LevelChecked() && Gauge.Paint >= 1)
                        return OriginalHook(HolyInWhite);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SwitfcastOption) && ActionReady(All.Swiftcast) &&
                        ((LevelChecked(CreatureMotif) && !Gauge.CreatureMotifDrawn) ||
                         (LevelChecked(WeaponMotif) && !Gauge.WeaponMotifDrawn) ||
                         (LevelChecked(LandscapeMotif) && !Gauge.LandscapeMotifDrawn)))
                        return All.Swiftcast;
                }

                //Prepare for Burst
                if (GetCooldownRemainingTime(ScenicMuse) <= 20)
                {
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif) && LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && GetTargetHPPercent() > landscapeStop)
                        return OriginalHook(LandscapeMotif);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif) && CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn && GetTargetHPPercent() > creatureStop)
                        return OriginalHook(CreatureMotif);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif) && WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime) && GetTargetHPPercent() > weaponStop)
                        return OriginalHook(WeaponMotif);
                }

                // Burst 
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_Phase) && HasEffect(Buffs.StarryMuse))
                {

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_CometInBlack) && CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0)
                        return CometinBlack;

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_HammerCombo) && HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.Starstruck))
                        return OriginalHook(HammerStamp);

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_StarPrism))
                    {
                        if (HasEffect(Buffs.Starstruck) || (HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) <= 3f))
                            return StarPrism;
                    }

                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_RainbowDrip))
                    {
                        if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) <= 3f))
                            return RainbowDrip;
                    }
                }

                if (HasEffect(Buffs.RainbowBright) && !HasEffect(Buffs.StarryMuse))
                    return RainbowDrip;

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CometinBlack) && CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 30f)
                    return OriginalHook(CometinBlack);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_HammerStampCombo) && HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (!HasEffect(Buffs.StarryMuse))
                {
                    // LandscapeMotif
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif) && GetTargetHPPercent() > landscapeStop)
                    {
                        if (LandscapeMotif.LevelChecked() &&
                            !Gauge.LandscapeMotifDrawn &&
                            GetCooldownRemainingTime(ScenicMuse) <= 20)
                        {
                            return OriginalHook(LandscapeMotif);
                        }
                    }

                    // CreatureMotif
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif) && GetTargetHPPercent() > creatureStop)
                    {
                        if (CreatureMotif.LevelChecked() &&
                            !Gauge.CreatureMotifDrawn &&
                            (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                        {
                            return OriginalHook(CreatureMotif);
                        }
                    }

                    // WeaponMotif
                    if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif) && GetTargetHPPercent() > weaponStop)
                    {
                        if (WeaponMotif.LevelChecked() &&
                            !HasEffect(Buffs.HammerTime) &&
                            !Gauge.WeaponMotifDrawn &&
                            (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                        {
                            return OriginalHook(WeaponMotif);
                        }
                    }
                }

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LucidDreaming) && All.LucidDreaming.LevelChecked() && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer?.CurrentMp <= Config.PCT_ST_AdvancedMode_LucidOption)
                    return All.LucidDreaming;

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_BlizzardInCyan) && BlizzardIIinCyan.LevelChecked() && HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardinCyan);
            }

            return actionID;
        }
    }

    internal class PCT_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is FireIIinRed)
            {
                bool canWeave = CanSpellWeave(ActionWatching.LastSpell);

                // Variant Cure
                if (IsEnabled(CustomComboPreset.PCT_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= GetOptionValue(Config.PCT_VariantCure))
                    return Variant.VariantCure;

                // Variant Rampart
                if (IsEnabled(CustomComboPreset.PCT_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    canWeave)
                    return Variant.VariantRampart;

                // Prepull logic

                if (!InCombat() || (InCombat() && CurrentTarget == null))
                {
                    if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn)
                        return OriginalHook(CreatureMotif);
                    if (WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                        return OriginalHook(WeaponMotif);
                    if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(LandscapeMotif);
                }

                // General Weaves
                if (InCombat() && canWeave)
                {
                    // LivingMuse

                    if (LivingMuse.LevelChecked() &&
                        Gauge.CreatureMotifDrawn &&
                        (!(Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) ||
                        GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse)))
                    {
                        if (HasCharges(OriginalHook(LivingMuse)))
                        {
                            if (!ScenicMuse.LevelChecked() ||
                                GetCooldown(ScenicMuse).CooldownRemaining > GetCooldownChargeRemainingTime(LivingMuse))
                            {
                                return OriginalHook(LivingMuse);
                            }
                        }
                    }

                    // ScenicMuse

                    if (ScenicMuse.LevelChecked() &&
                        Gauge.LandscapeMotifDrawn &&
                        Gauge.WeaponMotifDrawn &&
                        IsOffCooldown(ScenicMuse))
                    {
                        return OriginalHook(ScenicMuse);
                    }

                    // SteelMuse

                    if (SteelMuse.LevelChecked() &&
                        !HasEffect(Buffs.HammerTime) &&
                        Gauge.WeaponMotifDrawn &&
                        HasCharges(OriginalHook(SteelMuse)) &&
                        (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining ||
                            GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
                            !ScenicMuse.LevelChecked()))
                    {
                        return OriginalHook(SteelMuse);
                    }

                    // MogoftheAges

                    if (MogoftheAges.LevelChecked() &&
                        (Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) &&
                        (IsOffCooldown(OriginalHook(MogoftheAges)) || !ScenicMuse.LevelChecked()))
                    {
                        return OriginalHook(MogoftheAges);
                    }

                    if (IsMoving() &&
                        IsOffCooldown(All.Swiftcast) &&
                        All.Swiftcast.LevelChecked() &&
                        !HasEffect(Buffs.HammerTime) &&
                        Gauge.Paint < 1 &&
                        (!Gauge.CreatureMotifDrawn || !Gauge.WeaponMotifDrawn || !Gauge.LandscapeMotifDrawn))
                    {
                        return All.Swiftcast;
                    }

                    // Subtractive Palette
                    if (SubtractivePalette.LevelChecked() &&
                        !HasEffect(Buffs.SubtractivePalette) &&
                        !HasEffect(Buffs.MonochromeTones))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) || Gauge.PalleteGauge >= 50)
                            return SubtractivePalette;
                    }
                }

                if (HasEffect(All.Buffs.Swiftcast))
                {
                    if (!Gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(CreatureMotif);
                    if (!Gauge.WeaponMotifDrawn && HammerMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(HammerMotif);
                    if (!Gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse))
                        return OriginalHook(LandscapeMotif);
                }

                if (IsMoving() && InCombat())
                {
                    if (HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (CometinBlack.LevelChecked() && Gauge.Paint >= 1 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) < 3))
                        return RainbowDrip;

                    if (HolyInWhite.LevelChecked() && Gauge.Paint >= 1)
                        return OriginalHook(HolyInWhite);

                }

                //Prepare for Burst
                if (GetCooldownRemainingTime(ScenicMuse) <= 20)
                {
                    if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn)
                        return OriginalHook(LandscapeMotif);

                    if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn)
                        return OriginalHook(CreatureMotif);

                    if (WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                        return OriginalHook(WeaponMotif);
                }

                // Burst 
                if (HasEffect(Buffs.StarryMuse))
                {
                    // Check for CometInBlack
                    if (CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0)
                        return CometinBlack;

                    // Check for HammerTime 
                    if (HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.Starstruck))
                        return OriginalHook(HammerStamp);

                    // Check for Starstruck
                    if (HasEffect(Buffs.Starstruck) || (HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) < 3))
                        return StarPrism;

                    // Check for RainbowBright
                    if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) < 3))
                        return RainbowDrip;
                }

                if (HasEffect(Buffs.RainbowBright) && !HasEffect(Buffs.StarryMuse))
                    return RainbowDrip;

                if (CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 60)
                    return OriginalHook(CometinBlack);

                if (HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (!HasEffect(Buffs.StarryMuse))
                {
                    if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= 20)
                        return OriginalHook(LandscapeMotif);

                    if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                        return OriginalHook(CreatureMotif);

                    if (WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !Gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                        return OriginalHook(WeaponMotif);
                }
                //Saves one Charge of White paint for movement/Black paint.
                if (HolyInWhite.LevelChecked() && Gauge.Paint >= 2)
                    return OriginalHook(HolyInWhite);

                if (All.LucidDreaming.LevelChecked() && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer?.CurrentMp <= 6500)
                    return All.LucidDreaming;

                if (BlizzardIIinCyan.LevelChecked() && HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardIIinCyan);
            }
            return actionID;
        }
    }

    internal class PCT_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID is FireIIinRed)
            {
                bool canWeave = CanSpellWeave(ActionWatching.LastSpell);
                int creatureStop = PluginConfiguration.GetCustomIntValue(Config.PCT_AoE_CreatureStop);
                int landscapeStop = PluginConfiguration.GetCustomIntValue(Config.PCT_AoE_LandscapeStop);
                int weaponStop = PluginConfiguration.GetCustomIntValue(Config.PCT_AoE_WeaponStop);

                // Variant Cure
                if (IsEnabled(CustomComboPreset.PCT_Variant_Cure) &&
                    IsEnabled(Variant.VariantCure) &&
                    PlayerHealthPercentageHp() <= GetOptionValue(Config.PCT_VariantCure))
                    return Variant.VariantCure;

                // Variant Rampart
                if (IsEnabled(CustomComboPreset.PCT_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    canWeave)
                    return Variant.VariantRampart;

                // Prepull logic
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_PrePullMotifs))
                {
                    if (!InCombat() || (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_NoTargetMotifs) && InCombat() && CurrentTarget == null))
                    {
                        if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn)
                            return OriginalHook(CreatureMotif);
                        if (WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime))
                            return OriginalHook(WeaponMotif);
                        if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && !HasEffect(Buffs.StarryMuse))
                            return OriginalHook(LandscapeMotif);
                    }
                }

                // General Weaves
                if (InCombat() && canWeave)
                {
                    // LivingMuse
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LivingMuse))
                    {
                        if (LivingMuse.LevelChecked() &&
                            Gauge.CreatureMotifDrawn &&
                            (!(Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) ||
                            GetRemainingCharges(LivingMuse) == GetMaxCharges(LivingMuse)))
                        {
                            if (HasCharges(OriginalHook(LivingMuse)))
                            {
                                if (!ScenicMuse.LevelChecked() ||
                                    GetCooldown(ScenicMuse).CooldownRemaining > GetCooldownChargeRemainingTime(LivingMuse))
                                {
                                    return OriginalHook(LivingMuse);
                                }
                            }
                        }
                    }

                    // ScenicMuse
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_ScenicMuse))
                    {
                        if (ScenicMuse.LevelChecked() &&
                            Gauge.LandscapeMotifDrawn &&
                            Gauge.WeaponMotifDrawn &&
                            IsOffCooldown(ScenicMuse))
                        {
                            return OriginalHook(ScenicMuse);
                        }
                    }

                    // SteelMuse
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SteelMuse))
                    {
                        if (SteelMuse.LevelChecked() &&
                            !HasEffect(Buffs.HammerTime) &&
                            Gauge.WeaponMotifDrawn &&
                            HasCharges(OriginalHook(SteelMuse)) &&
                            (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining ||
                             GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
                             !ScenicMuse.LevelChecked()))
                        {
                            return OriginalHook(SteelMuse);
                        }
                    }

                    // MogoftheAges
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MogOfTheAges))
                    {
                        if (MogoftheAges.LevelChecked() &&
                            (Gauge.MooglePortraitReady || Gauge.MadeenPortraitReady) &&
                            (IsOffCooldown(OriginalHook(MogoftheAges)) || !ScenicMuse.LevelChecked()))
                        {
                            return OriginalHook(MogoftheAges);
                        }
                    }

                    // Subtractive Palette
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SubtractivePalette) &&
                        SubtractivePalette.LevelChecked() &&
                        !HasEffect(Buffs.SubtractivePalette) &&
                        !HasEffect(Buffs.MonochromeTones))
                    {
                        if (HasEffect(Buffs.SubtractiveSpectrum) || Gauge.PalleteGauge >= 50)
                            return SubtractivePalette;
                    }
                }

                if (HasEffect(All.Buffs.Swiftcast))
                {
                    if (!Gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse) && GetTargetHPPercent() > creatureStop)
                        return OriginalHook(CreatureMotif);
                    if (!Gauge.WeaponMotifDrawn && HammerMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.StarryMuse) && GetTargetHPPercent() > weaponStop)
                        return OriginalHook(HammerMotif);
                    if (!Gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasEffect(Buffs.StarryMuse) && GetTargetHPPercent() > landscapeStop)
                        return OriginalHook(LandscapeMotif);
                }

                if (IsMoving() && InCombat())
                {
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo) && HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                        return OriginalHook(HammerStamp);

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_CometinBlack) && CometinBlack.LevelChecked() && Gauge.Paint >= 1 && HasEffect(Buffs.MonochromeTones))
                        return OriginalHook(CometinBlack);

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_RainbowDrip))
                    {
                        if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) < 3))
                            return RainbowDrip;
                    }

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_HolyInWhite) && HolyInWhite.LevelChecked() && Gauge.Paint >= 1)
                        return OriginalHook(HolyInWhite);

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SwitfcastOption) && ActionReady(All.Swiftcast) &&
                        ((LevelChecked(CreatureMotif) && !Gauge.CreatureMotifDrawn) ||
                         (LevelChecked(WeaponMotif) && !Gauge.WeaponMotifDrawn) ||
                         (LevelChecked(LandscapeMotif) && !Gauge.LandscapeMotifDrawn)))
                        return All.Swiftcast;
                }

                //Prepare for Burst
                if (GetCooldownRemainingTime(ScenicMuse) <= 20)
                {
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif) && LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && GetTargetHPPercent() > landscapeStop)
                        return OriginalHook(LandscapeMotif);

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif) && CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn && GetTargetHPPercent() > creatureStop)
                        return OriginalHook(CreatureMotif);

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif) && WeaponMotif.LevelChecked() && !Gauge.WeaponMotifDrawn && !HasEffect(Buffs.HammerTime) && GetTargetHPPercent() > weaponStop)
                        return OriginalHook(WeaponMotif);
                }

                // Burst 
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_Phase) && HasEffect(Buffs.StarryMuse))
                {
                    // Check for CometInBlack
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_CometInBlack) && CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0)
                        return CometinBlack;

                    // Check for HammerTime 
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_HammerCombo) && HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime) && !HasEffect(Buffs.Starstruck))
                        return OriginalHook(HammerStamp);

                    // Check for Starstruck
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_StarPrism))
                    {
                        if (HasEffect(Buffs.Starstruck) || (HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) < 3))
                            return StarPrism;
                    }

                    // Check for RainbowBright
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_RainbowDrip))
                    {
                        if (HasEffect(Buffs.RainbowBright) || (HasEffect(Buffs.RainbowBright) && GetBuffRemainingTime(Buffs.StarryMuse) < 3))
                            return RainbowDrip;
                    }
                }

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_HolyinWhite) && !HasEffect(Buffs.StarryMuse) && !HasEffect(Buffs.MonochromeTones))
                {
                    if (Gauge.Paint > Config.PCT_AoE_AdvancedMode_HolyinWhiteOption ||
                        (Config.PCT_AoE_AdvancedMode_HolyinWhiteOption == 5 && Gauge.Paint == 5 && !HasEffect(Buffs.HammerTime) &&
                        (HasEffect(Buffs.RainbowBright) || WasLastSpell(AeroIIinGreen) || WasLastSpell(StoneIIinYellow))))
                        return OriginalHook(HolyInWhite);
                }

                if (HasEffect(Buffs.RainbowBright) && !HasEffect(Buffs.StarryMuse))
                    return RainbowDrip;

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CometinBlack) && CometinBlack.LevelChecked() && HasEffect(Buffs.MonochromeTones) && Gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 60)
                    return OriginalHook(CometinBlack);

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_HammerStampCombo) && HammerStamp.LevelChecked() && HasEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (!HasEffect(Buffs.StarryMuse))
                {
                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif) && GetTargetHPPercent() > landscapeStop)
                    {
                        if (LandscapeMotif.LevelChecked() && !Gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= 20)
                            return OriginalHook(LandscapeMotif);
                    }

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif) && GetTargetHPPercent() > creatureStop)
                    {
                        if (CreatureMotif.LevelChecked() && !Gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                            return OriginalHook(CreatureMotif);
                    }

                    if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif) && GetTargetHPPercent() > weaponStop)
                    {
                        if (WeaponMotif.LevelChecked() && !HasEffect(Buffs.HammerTime) && !Gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                            return OriginalHook(WeaponMotif);
                    }
                }

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LucidDreaming) && All.LucidDreaming.LevelChecked() && ActionReady(All.LucidDreaming) && CanSpellWeave(actionID) && LocalPlayer?.CurrentMp <= Config.PCT_ST_AdvancedMode_LucidOption)
                    return All.LucidDreaming;

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_BlizzardInCyan) && BlizzardIIinCyan.LevelChecked() && HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardIIinCyan);

            }
            return actionID;
        }
    }

    internal class CombinedAetherhues : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedAetherhues;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            int choice = Config.CombinedAetherhueChoices;

            if (actionID == FireInRed && choice is 0 or 1)
            {
                if (HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardinCyan);
            }

            if (actionID == FireIIinRed && choice is 0 or 2)
            {
                if (HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardIIinCyan);
            }

            return actionID;
        }
    }

    internal class CombinedMotifs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedMotifs;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID == CreatureMotif)
            {
                if ((Config.CombinedMotifsMog && Gauge.MooglePortraitReady) || (Config.CombinedMotifsMadeen && Gauge.MadeenPortraitReady && IsOffCooldown(OriginalHook(MogoftheAges))))
                    return OriginalHook(MogoftheAges);

                if (Gauge.CreatureMotifDrawn)
                    return OriginalHook(LivingMuse);
            }

            if (actionID == WeaponMotif)
            {
                if (Config.CombinedMotifsWeapon && HasEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (Gauge.WeaponMotifDrawn)
                    return OriginalHook(SteelMuse);
            }

            if (actionID == LandscapeMotif)
            {
                if (Config.CombinedMotifsLandscape && HasEffect(Buffs.Starstruck))
                    return OriginalHook(StarPrism);

                if (Gauge.LandscapeMotifDrawn)
                    return OriginalHook(ScenicMuse);
            }

            return actionID;
        }
    }

    internal class CombinedPaint : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedPaint;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID == HolyInWhite)
            {
                if (HasEffect(Buffs.MonochromeTones))
                    return CometinBlack;
            }

            return actionID;
        }
    }

    #region ID's

    public const byte JobID = 42;

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
        ThunderIIinMagenta = 34661,
        ThunderinMagenta = 34655,
        WaterinBlue = 34652,
        WeaponMotif = 34690,
        WingMotif = 34665;

    public static class Buffs
    {
        public const ushort
            SubtractivePalette = 3674,
            RainbowBright = 3679,
            HammerTime = 3680,
            MonochromeTones = 3691,
            StarryMuse = 3685,
            Hyperphantasia = 3688,
            Inspiration = 3689,
            SubtractiveSpectrum = 3690,
            Starstruck = 3681;
    }

    public static class Debuffs
    {

    }

    #endregion
}
