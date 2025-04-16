using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class PCT : Caster
{
    internal class PCT_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireInRed)
                return actionID;

            PCTGauge gauge = GetJobGauge<PCTGauge>();
            bool canWeave = CanSpellWeave() || CanSpellWeave();

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
                    gauge.LandscapeMotifDrawn &&
                    gauge.WeaponMotifDrawn &&
                    IsOffCooldown(ScenicMuse))
                {
                    return OriginalHook(ScenicMuse);
                }

                // LivingMuse

                if (LivingMuse.LevelChecked() &&
                    gauge.CreatureMotifDrawn &&
                    (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) ||
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
                    !HasStatusEffect(Buffs.HammerTime) &&
                    gauge.WeaponMotifDrawn &&
                    HasCharges(OriginalHook(SteelMuse)) &&
                    (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining ||
                     GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
                     !ScenicMuse.LevelChecked()))
                {
                    return OriginalHook(SteelMuse);
                }

                // MogoftheAges

                if (MogoftheAges.LevelChecked() &&
                    (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) &&
                    IsOffCooldown(OriginalHook(MogoftheAges)) &&
                    (GetCooldownRemainingTime(StarryMuse) >= 60 || !ScenicMuse.LevelChecked()))
                {
                    return OriginalHook(MogoftheAges);
                }

                // Swiftcast

                if (IsMoving() &&
                    IsOffCooldown(Role.Swiftcast) &&
                    Role.Swiftcast.LevelChecked() &&
                    !HasStatusEffect(Buffs.HammerTime) &&
                    gauge.Paint < 1 &&
                    (!gauge.CreatureMotifDrawn || !gauge.WeaponMotifDrawn || !gauge.LandscapeMotifDrawn))
                {
                    return Role.Swiftcast;
                }

                // SubtractivePalette

                if (SubtractivePalette.LevelChecked() &&
                    !HasStatusEffect(Buffs.SubtractivePalette) &&
                    !HasStatusEffect(Buffs.MonochromeTones))
                {
                    if (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                    {
                        return SubtractivePalette;
                    }
                }
            }

            // Swiftcast Motifs
            if (HasStatusEffect(Role.Buffs.Swiftcast))
            {
                if (!gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(CreatureMotif);
                if (!gauge.WeaponMotifDrawn && HammerMotif.LevelChecked() && !HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(HammerMotif);
                if (!gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(LandscapeMotif);
            }

            // IsMoving logic
            if (IsMoving() && InCombat())
            {
                if (HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (CometinBlack.LevelChecked() && gauge.Paint >= 1 && HasStatusEffect(Buffs.MonochromeTones))
                    return OriginalHook(CometinBlack);

                if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) <= 3f))
                    return RainbowDrip;

                if (HolyInWhite.LevelChecked() && gauge.Paint >= 1)
                    return OriginalHook(HolyInWhite);
            }

            //Prepare for Burst
            if (GetCooldownRemainingTime(ScenicMuse) <= 20)
            {
                if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn)
                    return OriginalHook(LandscapeMotif);

                if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn)
                    return OriginalHook(CreatureMotif);

                if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(WeaponMotif);
            }

            // Burst
            if (HasStatusEffect(Buffs.StarryMuse))
            {

                if (CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0)
                    return CometinBlack;

                if (HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.Starstruck))
                    return OriginalHook(HammerStamp);

                if (HasStatusEffect(Buffs.Starstruck) || (HasStatusEffect(Buffs.Starstruck) && GetStatusEffectRemainingTime(Buffs.Starstruck) <= 3f))
                    return StarPrism;

                if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) <= 3f))
                    return RainbowDrip;

            }

            if (HasStatusEffect(Buffs.RainbowBright) && !HasStatusEffect(Buffs.StarryMuse))
                return RainbowDrip;

            if (CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 30f)
                return OriginalHook(CometinBlack);

            if (HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                return OriginalHook(HammerStamp);

            if (!HasStatusEffect(Buffs.StarryMuse))
            {
                // LandscapeMotif

                if (LandscapeMotif.LevelChecked() &&
                    !gauge.LandscapeMotifDrawn &&
                    GetCooldownRemainingTime(ScenicMuse) <= 20)
                {
                    return OriginalHook(LandscapeMotif);
                }

                // CreatureMotif

                if (CreatureMotif.LevelChecked() &&
                    !gauge.CreatureMotifDrawn &&
                    (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                {
                    return OriginalHook(CreatureMotif);
                }

                // WeaponMotif

                if (WeaponMotif.LevelChecked() &&
                    !HasStatusEffect(Buffs.HammerTime) &&
                    !gauge.WeaponMotifDrawn &&
                    (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                {
                    return OriginalHook(WeaponMotif);
                }
            }

            if (Role.CanLucidDream(6500))
                return Role.LucidDreaming;

            if (BlizzardIIinCyan.LevelChecked() && HasStatusEffect(Buffs.SubtractivePalette))
                return OriginalHook(BlizzardinCyan);

            return actionID;
        }
    }

    internal class PCT_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireInRed)
                return actionID;

            PCTGauge gauge = GetJobGauge<PCTGauge>();
            bool canWeave = CanSpellWeave() || CanSpellWeave();
            int creatureStop = Config.PCT_ST_CreatureStop;
            int landscapeStop = Config.PCT_ST_LandscapeStop;
            int weaponStop = Config.PCT_ST_WeaponStop;

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.PCT_Variant_Cure, Config.PCT_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.PCT_Variant_Rampart,WeaveTypes.SpellWeave))
                return Variant.Rampart;

            // Prepull logic
            if ((IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_PrePullMotifs) && !InCombat()) || (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_NoTargetMotifs) && InCombat() && CurrentTarget == null))
            {
                if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn)
                    return OriginalHook(CreatureMotif);
                if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(WeaponMotif);
                if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(LandscapeMotif);

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
                        gauge.LandscapeMotifDrawn &&
                        gauge.WeaponMotifDrawn &&
                        IsOffCooldown(ScenicMuse))
                    {
                        return OriginalHook(ScenicMuse);
                    }
                }

                // LivingMuse
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LivingMuse))
                {
                    if (LivingMuse.LevelChecked() &&
                        gauge.CreatureMotifDrawn &&
                        (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) ||
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
                        !HasStatusEffect(Buffs.HammerTime) &&
                        gauge.WeaponMotifDrawn &&
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
                        (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) &&
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
                        !HasStatusEffect(Buffs.SubtractivePalette) &&
                        !HasStatusEffect(Buffs.MonochromeTones))
                    {
                        if (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                        {
                            return SubtractivePalette;
                        }
                    }
                }
            }

            // Swiftcast Motifs
            if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SwiftMotifs) &&
                HasStatusEffect(Role.Buffs.Swiftcast))
            {
                if (!gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse) && GetTargetHPPercent() > creatureStop)
                    return OriginalHook(CreatureMotif);
                if (!gauge.WeaponMotifDrawn && WeaponMotif.LevelChecked() && !HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.StarryMuse) && GetTargetHPPercent() > weaponStop)
                    return OriginalHook(WeaponMotif);
                if (!gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse) && GetTargetHPPercent() > landscapeStop)
                    return OriginalHook(LandscapeMotif);

            }

            // IsMoving logic
            if (IsMoving() && InCombat())
            {
                //increase priority for using casts as soon as possible to avoid losing DPS and ensure all abilities fit within buff windows
                //previously, there were situations where Wrath prioritized using Hammer Combo over casts, which would prevent us from generating Rainbow Bright in time when movement is required
                //so, if we have Hyperphantasia stacks and Inspiration is active from standing in PCT LeyLines, we burn it all down
                bool hasPaint = gauge.Paint > 0;
                bool burnStacks = GetStatusEffectStacks(Buffs.Hyperphantasia) > 0 && HasStatusEffect(Buffs.Inspiration) && hasPaint; //use casts asap if we have Hyperphantasia stacks and Inspiration is active from standing in PCT LeyLines
                bool shouldHolyInWhite = IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HolyInWhite) && HolyInWhite.LevelChecked() && hasPaint & !HasStatusEffect(Buffs.MonochromeTones); //normal conditions for Holy In White
                bool shouldCometInBlack = IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_CometinBlack) && CometinBlack.LevelChecked() && hasPaint && HasStatusEffect(Buffs.MonochromeTones); //normal conditions for Comet in Black
                if (burnStacks && ((Config.BlackHyperphantasiaOption && shouldCometInBlack) || (Config.WhiteHyperphantasiaOption && shouldHolyInWhite)))
                    return HasStatusEffect(Buffs.MonochromeTones) ? OriginalHook(CometinBlack) : OriginalHook(HolyInWhite);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HammerStampCombo) && HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (shouldCometInBlack)
                    return OriginalHook(CometinBlack);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_RainbowDrip))
                {
                    if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) <= 3f))
                        return RainbowDrip;
                }

                if (shouldHolyInWhite)
                    return OriginalHook(HolyInWhite);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_SwitfcastOption) && ActionReady(Role.Swiftcast) &&
                    ((LevelChecked(CreatureMotif) && !gauge.CreatureMotifDrawn) ||
                     (LevelChecked(WeaponMotif) && !gauge.WeaponMotifDrawn) ||
                     (LevelChecked(LandscapeMotif) && !gauge.LandscapeMotifDrawn)))
                    return Role.Swiftcast;
            }

            //Prepare for Burst
            if (GetCooldownRemainingTime(ScenicMuse) <= 20)
            {
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif) && LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetTargetHPPercent() > landscapeStop)
                    return OriginalHook(LandscapeMotif);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif) && CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && GetTargetHPPercent() > creatureStop)
                    return OriginalHook(CreatureMotif);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif) && WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime) && GetTargetHPPercent() > weaponStop)
                    return OriginalHook(WeaponMotif);
            }

            // Burst
            if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_Phase) && HasStatusEffect(Buffs.StarryMuse))
            {

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_CometInBlack) && CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0)
                    return CometinBlack;

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_HammerCombo) && HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.Starstruck))
                    return OriginalHook(HammerStamp);

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_StarPrism))
                {
                    if (HasStatusEffect(Buffs.Starstruck) || (HasStatusEffect(Buffs.Starstruck) && GetStatusEffectRemainingTime(Buffs.Starstruck) <= 3f))
                        return StarPrism;
                }

                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_Burst_RainbowDrip))
                {
                    if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) <= 3f))
                        return RainbowDrip;
                }
            }

            if (HasStatusEffect(Buffs.RainbowBright) && !HasStatusEffect(Buffs.StarryMuse))
                return RainbowDrip;

            if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CometinBlack) && CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 30f)
                return OriginalHook(CometinBlack);

            if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_HammerStampCombo) && HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                return OriginalHook(HammerStamp);

            if (!HasStatusEffect(Buffs.StarryMuse))
            {
                // LandscapeMotif
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif) && GetTargetHPPercent() > landscapeStop)
                {
                    if (LandscapeMotif.LevelChecked() &&
                        !gauge.LandscapeMotifDrawn &&
                        GetCooldownRemainingTime(ScenicMuse) <= 20)
                    {
                        return OriginalHook(LandscapeMotif);
                    }
                }

                // CreatureMotif
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif) && GetTargetHPPercent() > creatureStop)
                {
                    if (CreatureMotif.LevelChecked() &&
                        !gauge.CreatureMotifDrawn &&
                        (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                    {
                        return OriginalHook(CreatureMotif);
                    }
                }

                // WeaponMotif
                if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif) && GetTargetHPPercent() > weaponStop)
                {
                    if (WeaponMotif.LevelChecked() &&
                        !HasStatusEffect(Buffs.HammerTime) &&
                        !gauge.WeaponMotifDrawn &&
                        (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                    {
                        return OriginalHook(WeaponMotif);
                    }
                }
            }

            if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_LucidDreaming) && Role.CanLucidDream(Config.PCT_ST_AdvancedMode_LucidOption))
                return Role.LucidDreaming;

            if (IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_BlizzardInCyan) && BlizzardIIinCyan.LevelChecked() && HasStatusEffect(Buffs.SubtractivePalette))
                return OriginalHook(BlizzardinCyan);

            return actionID;
        }
    }

    internal class PCT_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireIIinRed)
                return actionID;

            PCTGauge gauge = GetJobGauge<PCTGauge>();
            bool canWeave = CanSpellWeave();

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.PCT_Variant_Cure, Config.PCT_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.PCT_Variant_Rampart, WeaveTypes.SpellWeave))
                return Variant.Rampart;

            // Prepull logic

            if (!InCombat() || (InCombat() && CurrentTarget == null))
            {
                if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn)
                    return OriginalHook(CreatureMotif);
                if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(WeaponMotif);
                if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(LandscapeMotif);
            }

            // General Weaves
            if (InCombat() && canWeave)
            {
                // LivingMuse

                if (LivingMuse.LevelChecked() &&
                    gauge.CreatureMotifDrawn &&
                    (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) ||
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
                    gauge.LandscapeMotifDrawn &&
                    gauge.WeaponMotifDrawn &&
                    IsOffCooldown(ScenicMuse))
                {
                    return OriginalHook(ScenicMuse);
                }

                // SteelMuse

                if (SteelMuse.LevelChecked() &&
                    !HasStatusEffect(Buffs.HammerTime) &&
                    gauge.WeaponMotifDrawn &&
                    HasCharges(OriginalHook(SteelMuse)) &&
                    (GetCooldown(SteelMuse).CooldownRemaining < GetCooldown(ScenicMuse).CooldownRemaining ||
                     GetRemainingCharges(SteelMuse) == GetMaxCharges(SteelMuse) ||
                     !ScenicMuse.LevelChecked()))
                {
                    return OriginalHook(SteelMuse);
                }

                // MogoftheAges

                if (MogoftheAges.LevelChecked() &&
                    (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) &&
                    (IsOffCooldown(OriginalHook(MogoftheAges)) || !ScenicMuse.LevelChecked()))
                {
                    return OriginalHook(MogoftheAges);
                }

                if (IsMoving() &&
                    IsOffCooldown(Role.Swiftcast) &&
                    Role.Swiftcast.LevelChecked() &&
                    !HasStatusEffect(Buffs.HammerTime) &&
                    gauge.Paint < 1 &&
                    (!gauge.CreatureMotifDrawn || !gauge.WeaponMotifDrawn || !gauge.LandscapeMotifDrawn))
                {
                    return Role.Swiftcast;
                }

                // Subtractive Palette
                if (SubtractivePalette.LevelChecked() &&
                    !HasStatusEffect(Buffs.SubtractivePalette) &&
                    !HasStatusEffect(Buffs.MonochromeTones))
                {
                    if (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                        return SubtractivePalette;
                }
            }

            if (HasStatusEffect(Role.Buffs.Swiftcast))
            {
                if (!gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(CreatureMotif);
                if (!gauge.WeaponMotifDrawn && HammerMotif.LevelChecked() && !HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(HammerMotif);
                if (!gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse))
                    return OriginalHook(LandscapeMotif);
            }

            if (IsMoving() && InCombat())
            {
                if (HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (CometinBlack.LevelChecked() && gauge.Paint >= 1 && HasStatusEffect(Buffs.MonochromeTones))
                    return OriginalHook(CometinBlack);

                if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) < 3))
                    return RainbowDrip;

                if (HolyInWhite.LevelChecked() && gauge.Paint >= 1)
                    return OriginalHook(HolyInWhite);

            }

            //Prepare for Burst
            if (GetCooldownRemainingTime(ScenicMuse) <= 20)
            {
                if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn)
                    return OriginalHook(LandscapeMotif);

                if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn)
                    return OriginalHook(CreatureMotif);

                if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(WeaponMotif);
            }

            // Burst
            if (HasStatusEffect(Buffs.StarryMuse))
            {
                // Check for CometInBlack
                if (CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0)
                    return CometinBlack;

                // Check for HammerTime
                if (HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.Starstruck))
                    return OriginalHook(HammerStamp);

                // Check for Starstruck
                if (HasStatusEffect(Buffs.Starstruck) || (HasStatusEffect(Buffs.Starstruck) && GetStatusEffectRemainingTime(Buffs.Starstruck) < 3))
                    return StarPrism;

                // Check for RainbowBright
                if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) < 3))
                    return RainbowDrip;
            }

            if (HasStatusEffect(Buffs.RainbowBright) && !HasStatusEffect(Buffs.StarryMuse))
                return RainbowDrip;

            if (CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 60)
                return OriginalHook(CometinBlack);

            if (HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                return OriginalHook(HammerStamp);

            if (!HasStatusEffect(Buffs.StarryMuse))
            {
                if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= 20)
                    return OriginalHook(LandscapeMotif);

                if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                    return OriginalHook(CreatureMotif);

                if (WeaponMotif.LevelChecked() && !HasStatusEffect(Buffs.HammerTime) && !gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                    return OriginalHook(WeaponMotif);
            }
            //Saves one Charge of White paint for movement/Black paint.
            if (HolyInWhite.LevelChecked() && gauge.Paint >= 2)
                return OriginalHook(HolyInWhite);

            if (Role.CanLucidDream(6500))
                return Role.LucidDreaming;

            if (BlizzardIIinCyan.LevelChecked() && HasStatusEffect(Buffs.SubtractivePalette))
                return OriginalHook(BlizzardIIinCyan);
            return actionID;
        }
    }

    internal class PCT_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCT_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireIIinRed)
                return actionID;

            PCTGauge gauge = GetJobGauge<PCTGauge>();
            bool canWeave = CanSpellWeave();
            int creatureStop = Config.PCT_AoE_CreatureStop;
            int landscapeStop = Config.PCT_AoE_LandscapeStop;
            int weaponStop = Config.PCT_AoE_WeaponStop;

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.PCT_Variant_Cure, Config.PCT_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.PCT_Variant_Rampart, WeaveTypes.SpellWeave))
                return Variant.Rampart;

            // Prepull logic
            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_PrePullMotifs))
            {
                if (!InCombat() || (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_NoTargetMotifs) && InCombat() && CurrentTarget == null))
                {
                    if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn)
                        return OriginalHook(CreatureMotif);
                    if (WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime))
                        return OriginalHook(WeaponMotif);
                    if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && !HasStatusEffect(Buffs.StarryMuse))
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
                        gauge.CreatureMotifDrawn &&
                        (!(gauge.MooglePortraitReady || gauge.MadeenPortraitReady) ||
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
                        gauge.LandscapeMotifDrawn &&
                        gauge.WeaponMotifDrawn &&
                        IsOffCooldown(ScenicMuse))
                    {
                        return OriginalHook(ScenicMuse);
                    }
                }

                // SteelMuse
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SteelMuse))
                {
                    if (SteelMuse.LevelChecked() &&
                        !HasStatusEffect(Buffs.HammerTime) &&
                        gauge.WeaponMotifDrawn &&
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
                        (gauge.MooglePortraitReady || gauge.MadeenPortraitReady) &&
                        (IsOffCooldown(OriginalHook(MogoftheAges)) || !ScenicMuse.LevelChecked()))
                    {
                        return OriginalHook(MogoftheAges);
                    }
                }

                // Subtractive Palette
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SubtractivePalette) &&
                    SubtractivePalette.LevelChecked() &&
                    !HasStatusEffect(Buffs.SubtractivePalette) &&
                    !HasStatusEffect(Buffs.MonochromeTones))
                {
                    if (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                        return SubtractivePalette;
                }
            }

            if (HasStatusEffect(Role.Buffs.Swiftcast))
            {
                if (!gauge.CreatureMotifDrawn && CreatureMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse) && GetTargetHPPercent() > creatureStop)
                    return OriginalHook(CreatureMotif);
                if (!gauge.WeaponMotifDrawn && HammerMotif.LevelChecked() && !HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.StarryMuse) && GetTargetHPPercent() > weaponStop)
                    return OriginalHook(HammerMotif);
                if (!gauge.LandscapeMotifDrawn && LandscapeMotif.LevelChecked() && !HasStatusEffect(Buffs.StarryMuse) && GetTargetHPPercent() > landscapeStop)
                    return OriginalHook(LandscapeMotif);
            }

            if (IsMoving() && InCombat())
            {
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_HammerStampCombo) && HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_CometinBlack) && CometinBlack.LevelChecked() && gauge.Paint >= 1 && HasStatusEffect(Buffs.MonochromeTones))
                    return OriginalHook(CometinBlack);

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_RainbowDrip))
                {
                    if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) < 3))
                        return RainbowDrip;
                }

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_MovementOption_HolyInWhite) && HolyInWhite.LevelChecked() && gauge.Paint >= 1)
                    return OriginalHook(HolyInWhite);

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_SwitfcastOption) && ActionReady(Role.Swiftcast) &&
                    ((LevelChecked(CreatureMotif) && !gauge.CreatureMotifDrawn) ||
                     (LevelChecked(WeaponMotif) && !gauge.WeaponMotifDrawn) ||
                     (LevelChecked(LandscapeMotif) && !gauge.LandscapeMotifDrawn)))
                    return Role.Swiftcast;
            }

            //Prepare for Burst
            if (GetCooldownRemainingTime(ScenicMuse) <= 20)
            {
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif) && LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetTargetHPPercent() > landscapeStop)
                    return OriginalHook(LandscapeMotif);

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif) && CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && GetTargetHPPercent() > creatureStop)
                    return OriginalHook(CreatureMotif);

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif) && WeaponMotif.LevelChecked() && !gauge.WeaponMotifDrawn && !HasStatusEffect(Buffs.HammerTime) && GetTargetHPPercent() > weaponStop)
                    return OriginalHook(WeaponMotif);
            }

            // Burst
            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_Phase) && HasStatusEffect(Buffs.StarryMuse))
            {
                // Check for CometInBlack
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_CometInBlack) && CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0)
                    return CometinBlack;

                // Check for HammerTime
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_HammerCombo) && HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime) && !HasStatusEffect(Buffs.Starstruck))
                    return OriginalHook(HammerStamp);

                // Check for Starstruck
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_StarPrism))
                {
                    if (HasStatusEffect(Buffs.Starstruck) || (HasStatusEffect(Buffs.Starstruck) && GetStatusEffectRemainingTime(Buffs.Starstruck) < 3))
                        return StarPrism;
                }

                // Check for RainbowBright
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_Burst_RainbowDrip))
                {
                    if (HasStatusEffect(Buffs.RainbowBright) || (HasStatusEffect(Buffs.RainbowBright) && GetStatusEffectRemainingTime(Buffs.StarryMuse) < 3))
                        return RainbowDrip;
                }
            }

            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_HolyinWhite) && !HasStatusEffect(Buffs.StarryMuse) && !HasStatusEffect(Buffs.MonochromeTones))
            {
                if (gauge.Paint > Config.PCT_AoE_AdvancedMode_HolyinWhiteOption ||
                    (Config.PCT_AoE_AdvancedMode_HolyinWhiteOption == 5 && gauge.Paint == 5 && !HasStatusEffect(Buffs.HammerTime) &&
                     (HasStatusEffect(Buffs.RainbowBright) || WasLastSpell(AeroIIinGreen) || WasLastSpell(StoneIIinYellow))))
                    return OriginalHook(HolyInWhite);
            }

            if (HasStatusEffect(Buffs.RainbowBright) && !HasStatusEffect(Buffs.StarryMuse))
                return RainbowDrip;

            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CometinBlack) && CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && gauge.Paint > 0 && GetCooldownRemainingTime(StarryMuse) > 60)
                return OriginalHook(CometinBlack);

            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_HammerStampCombo) && HammerStamp.LevelChecked() && HasStatusEffect(Buffs.HammerTime))
                return OriginalHook(HammerStamp);

            if (!HasStatusEffect(Buffs.StarryMuse))
            {
                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif) && GetTargetHPPercent() > landscapeStop)
                {
                    if (LandscapeMotif.LevelChecked() && !gauge.LandscapeMotifDrawn && GetCooldownRemainingTime(ScenicMuse) <= 20)
                        return OriginalHook(LandscapeMotif);
                }

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif) && GetTargetHPPercent() > creatureStop)
                {
                    if (CreatureMotif.LevelChecked() && !gauge.CreatureMotifDrawn && (HasCharges(LivingMuse) || GetCooldownChargeRemainingTime(LivingMuse) <= 8))
                        return OriginalHook(CreatureMotif);
                }

                if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif) && GetTargetHPPercent() > weaponStop)
                {
                    if (WeaponMotif.LevelChecked() && !HasStatusEffect(Buffs.HammerTime) && !gauge.WeaponMotifDrawn && (HasCharges(SteelMuse) || GetCooldownChargeRemainingTime(SteelMuse) <= 8))
                        return OriginalHook(WeaponMotif);
                }
            }

            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_LucidDreaming) && Role.CanLucidDream(Config.PCT_ST_AdvancedMode_LucidOption))
                return Role.LucidDreaming;

            if (IsEnabled(CustomComboPreset.PCT_AoE_AdvancedMode_BlizzardInCyan) && BlizzardIIinCyan.LevelChecked() && HasStatusEffect(Buffs.SubtractivePalette))
                return OriginalHook(BlizzardIIinCyan);
            return actionID;
        }
    }

    internal class CombinedAetherhues : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedAetherhues;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FireInRed or FireIIinRed))
                return actionID;

            int choice = Config.CombinedAetherhueChoices;

            if (actionID == FireInRed && choice is 0 or 1)
            {
                if (HasStatusEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardinCyan);
            }

            if (actionID == FireIIinRed && choice is 0 or 2)
            {
                if (HasStatusEffect(Buffs.SubtractivePalette))
                    return OriginalHook(BlizzardIIinCyan);
            }

            return actionID;
        }
    }

    internal class CombinedMotifs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedMotifs;

        protected override uint Invoke(uint actionID)
        {
            PCTGauge gauge = GetJobGauge<PCTGauge>();

            if (actionID == CreatureMotif)
            {
                if ((Config.CombinedMotifsMog && gauge.MooglePortraitReady) || (Config.CombinedMotifsMadeen && gauge.MadeenPortraitReady && IsOffCooldown(OriginalHook(MogoftheAges))))
                    return OriginalHook(MogoftheAges);

                if (gauge.CreatureMotifDrawn)
                    return OriginalHook(LivingMuse);
            }

            if (actionID == WeaponMotif)
            {
                if (Config.CombinedMotifsWeapon && HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (gauge.WeaponMotifDrawn)
                    return OriginalHook(SteelMuse);
            }

            if (actionID == LandscapeMotif)
            {
                if (Config.CombinedMotifsLandscape && HasStatusEffect(Buffs.Starstruck))
                    return OriginalHook(StarPrism);

                if (gauge.LandscapeMotifDrawn)
                    return OriginalHook(ScenicMuse);
            }

            return actionID;
        }
    }

    internal class CombinedPaint : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CombinedPaint;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != HolyInWhite)
                return actionID;
            if (HasStatusEffect(Buffs.MonochromeTones))
                return CometinBlack;
            return actionID;
        }
    }
}
