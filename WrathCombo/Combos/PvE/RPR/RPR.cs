using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class RPR : Melee
{
    internal class RPR_ST_BasicCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not InfernalSlice)
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is Slice && LevelChecked(WaxingSlice))
                    return WaxingSlice;

                if (ComboAction is WaxingSlice && LevelChecked(InfernalSlice))
                    return InfernalSlice;
            }

            return Slice;
        }
    }
    
    internal class RPR_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not Slice)
                return actionID;

            //Soulsow
            if (LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) &&
                !PartyInCombat())
                return Soulsow;

            //Variant Cure
            if (Variant.CanCure(CustomComboPreset.RPR_Variant_Cure, Config.RPR_VariantCure))
                return Variant.Cure;

            //Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.RPR_Variant_Rampart))
                return Variant.Rampart;

            //All Weaves
            if (CanWeave())
            {
                //Arcane Cirlce
                if (ActionReady(ArcaneCircle) && InBossEncounter() &&
                    (LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) ||
                     !LevelChecked(Enshroud)))
                    return ArcaneCircle;

                //Enshroud
                if (UseEnshroud(Gauge))
                    return Enshroud;

                //Gluttony/Bloodstalk
                if (Gauge.Soul >= 50 &&
                    !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                    !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.PerfectioParata) &&
                    !IsComboExpiring(3))
                {
                    //Gluttony
                    if (ActionReady(Gluttony))
                        return Role.CanTrueNorth()
                            ? Role.TrueNorth
                            : Gluttony;

                    //Bloodstalk
                    if (LevelChecked(BloodStalk) &&
                        (!LevelChecked(Gluttony) ||
                         LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                         (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 4)))
                        return OriginalHook(BloodStalk);
                }

                //Enshroud Weaves
                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    //Sacrificium
                    if (Gauge.LemureShroud <= 4 && HasStatusEffect(Buffs.Oblatio) &&
                        (InBossEncounter() && GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 && !JustUsed(ArcaneCircle, 2) ||
                         !InBossEncounter() && IsOffCooldown(ArcaneCircle)))
                        return OriginalHook(Gluttony);

                    //Lemure's Slice
                    if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }

                //Healing
                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            //Ranged Attacks
            if (!InMeleeRange() && ActionReady(Harpe) && HasBattleTarget() &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.SoulReaver))
            {
                //Communio
                if (HasStatusEffect(Buffs.Enshrouded) && Gauge.LemureShroud is 1 &&
                    LevelChecked(Communio))
                    return Communio;

                return HasStatusEffect(Buffs.Soulsow)
                    ? HarvestMoon
                    : Harpe;
            }

            //Shadow Of Death
            if (UseShadowOfDeath())
                return ShadowOfDeath;

            //Perfectio
            if (HasStatusEffect(Buffs.PerfectioParata) && !IsComboExpiring(1))
                return OriginalHook(Communio);

            //Gibbet/Gallows
            if (LevelChecked(Gibbet) && !HasStatusEffect(Buffs.Enshrouded) &&
                (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
            {
                //Gibbet
                if (HasStatusEffect(Buffs.EnhancedGibbet))
                {
                    if (Role.CanTrueNorth() && !OnTargetsFlank() &&
                        CanDelayedWeave())
                        return Role.TrueNorth;

                    return OriginalHook(Gibbet);
                }

                //Gallows
                if (HasStatusEffect(Buffs.EnhancedGallows) ||
                    !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                {
                    if (Role.CanTrueNorth() && !OnTargetsRear() &&
                        CanDelayedWeave())
                        return Role.TrueNorth;

                    return OriginalHook(Gallows);
                }
            }

            //Plentiful Harvest
            if (!HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && HasStatusEffect(Buffs.ImmortalSacrifice) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            //Enshroud Combo
            if (HasStatusEffect(Buffs.Enshrouded))
            {
                //Communio
                if (Gauge.LemureShroud is 1 && LevelChecked(Communio))
                    return Communio;

                //Void Reaping
                if (HasStatusEffect(Buffs.EnhancedVoidReaping))
                    return OriginalHook(Gibbet);

                //Cross Reaping
                if (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                    !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping))
                    return OriginalHook(Gallows);
            }

            //Soul Slice
            if (Gauge.Soul <= 50 && ActionReady(SoulSlice) &&
                !IsComboExpiring(3) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.PerfectioParata) && !HasStatusEffect(Buffs.ImmortalSacrifice))
                return SoulSlice;

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                    return OriginalHook(WaxingSlice);

                if (ComboAction == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                    return OriginalHook(InfernalSlice);
            }

            return actionID;
        }
    }

    internal class RPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Slice)
                return actionID;

            int positionalChoice = Config.RPR_Positional;

            //Soulsow
            if (IsEnabled(CustomComboPreset.RPR_ST_SoulSow) &&
                LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            //Variant Cure
            if (Variant.CanCure(CustomComboPreset.RPR_Variant_Cure, Config.RPR_VariantCure))
                return Variant.Cure;

            //Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.RPR_Variant_Rampart))
                return Variant.Rampart;

            //RPR Opener
            if (IsEnabled(CustomComboPreset.RPR_ST_Opener) && 
                Opener().FullOpener(ref actionID))
                return actionID;

            //All Weaves
            if (CanWeave())
            {
                //Arcane Cirlce
                if (IsEnabled(CustomComboPreset.RPR_ST_ArcaneCircle) &&
                    ActionReady(ArcaneCircle) &&
                    (LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) ||
                     !LevelChecked(Enshroud)) &&
                    (Config.RPR_ST_ArcaneCircle_SubOption == 0 ||
                     Config.RPR_ST_ArcaneCircle_SubOption == 1 && InBossEncounter()))
                    return ArcaneCircle;

                //Enshroud
                if (IsEnabled(CustomComboPreset.RPR_ST_Enshroud) &&
                    UseEnshroud(Gauge))
                    return Enshroud;

                //Gluttony/Bloodstalk
                if (Gauge.Soul >= 50 &&
                    !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                    !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.PerfectioParata) &&
                    !IsComboExpiring(3))
                {
                    //Gluttony
                    if (IsEnabled(CustomComboPreset.RPR_ST_Gluttony) &&
                        ActionReady(Gluttony))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                            Role.CanTrueNorth())
                            return Role.TrueNorth;

                        return Gluttony;
                    }

                    //Bloodstalk
                    if (IsEnabled(CustomComboPreset.RPR_ST_Bloodstalk) &&
                        LevelChecked(BloodStalk) &&
                        (!LevelChecked(Gluttony) ||
                         LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                         (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 4)))
                        return OriginalHook(BloodStalk);
                }

                //Enshroud Weaves
                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    //Sacrificium
                    if (IsEnabled(CustomComboPreset.RPR_ST_Sacrificium) &&
                        Gauge.LemureShroud <= 4 && HasStatusEffect(Buffs.Oblatio) &&
                        (GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 && !JustUsed(ArcaneCircle, 2) &&
                         (Config.RPR_ST_ArcaneCircle_SubOption == 0 || Config.RPR_ST_ArcaneCircle_SubOption == 1 && InBossEncounter()) ||
                         Config.RPR_ST_ArcaneCircle_SubOption == 1 && !InBossEncounter() && IsOffCooldown(ArcaneCircle)))
                        return OriginalHook(Gluttony);

                    //Lemure's Slice
                    if (IsEnabled(CustomComboPreset.RPR_ST_Lemure) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }

                //Healing
                if (IsEnabled(CustomComboPreset.RPR_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(Config.RPR_STSecondWindThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(Config.RPR_STBloodbathThreshold))
                        return Role.Bloodbath;
                }
            }

            //Ranged Attacks
            if (IsEnabled(CustomComboPreset.RPR_ST_RangedFiller) &&
                ActionReady(Harpe) && !InMeleeRange() && HasBattleTarget() &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.SoulReaver))
            {
                //Communio
                if (HasStatusEffect(Buffs.Enshrouded) && Gauge.LemureShroud is 1 &&
                    LevelChecked(Communio))
                    return Communio;

                return IsEnabled(CustomComboPreset.RPR_ST_RangedFillerHarvestMoon) &&
                       HasStatusEffect(Buffs.Soulsow)
                    ? HarvestMoon
                    : Harpe;
            }

            //Shadow Of Death
            if (IsEnabled(CustomComboPreset.RPR_ST_SoD) &&
                UseShadowOfDeath() && GetTargetHPPercent() > Config.RPR_SoDThreshold)
                return ShadowOfDeath;

            //Perfectio
            if (IsEnabled(CustomComboPreset.RPR_ST_Perfectio) &&
                HasStatusEffect(Buffs.PerfectioParata) && !IsComboExpiring(1))
                return OriginalHook(Communio);

            //Gibbet/Gallows
            if (IsEnabled(CustomComboPreset.RPR_ST_GibbetGallows) &&
                LevelChecked(Gibbet) && !HasStatusEffect(Buffs.Enshrouded) &&
                (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
            {
                //Gibbet
                if (HasStatusEffect(Buffs.EnhancedGibbet) ||
                    positionalChoice is 1 && !HasStatusEffect(Buffs.EnhancedGibbet) &&
                    !HasStatusEffect(Buffs.EnhancedGallows))
                {
                    if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                        (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge) &&
                         GetRemainingCharges(Role.TrueNorth) < 2 ||
                         IsNotEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge)) &&
                        Role.CanTrueNorth() && !OnTargetsFlank() && CanDelayedWeave())
                        return Role.TrueNorth;

                    return OriginalHook(Gibbet);
                }

                //Gallows
                if (HasStatusEffect(Buffs.EnhancedGallows) ||
                    positionalChoice is 0 && !HasStatusEffect(Buffs.EnhancedGibbet) &&
                    !HasStatusEffect(Buffs.EnhancedGallows))
                {
                    if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                        (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge) &&
                         GetRemainingCharges(Role.TrueNorth) < 2 ||
                         IsNotEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge)) &&
                        Role.CanTrueNorth() && !OnTargetsRear() && CanDelayedWeave())
                        return Role.TrueNorth;

                    return OriginalHook(Gallows);
                }
            }

            //Plentiful Harvest
            if (IsEnabled(CustomComboPreset.RPR_ST_PlentifulHarvest) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && HasStatusEffect(Buffs.ImmortalSacrifice) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            //Enshroud Combo
            if (HasStatusEffect(Buffs.Enshrouded))
            {
                //Communio
                if (IsEnabled(CustomComboPreset.RPR_ST_Communio) &&
                    Gauge.LemureShroud is 1 && LevelChecked(Communio))
                    return Communio;

                //Void Reaping
                if (IsEnabled(CustomComboPreset.RPR_ST_Reaping) &&
                    HasStatusEffect(Buffs.EnhancedVoidReaping))
                    return OriginalHook(Gibbet);

                //Cross Reaping
                if (IsEnabled(CustomComboPreset.RPR_ST_Reaping) &&
                    (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                     !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping)))
                    return OriginalHook(Gallows);
            }

            //Soul Slice
            if (IsEnabled(CustomComboPreset.RPR_ST_SoulSlice) &&
                Gauge.Soul <= 50 && ActionReady(SoulSlice) &&
                !IsComboExpiring(3) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.PerfectioParata) && !HasStatusEffect(Buffs.ImmortalSacrifice))
                return SoulSlice;

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                    return OriginalHook(WaxingSlice);

                if (ComboAction == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                    return OriginalHook(InfernalSlice);
            }
            return actionID;
        }
    }

    internal class RPR_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not SpinningScythe)
                return actionID;

            //Soulsow
            if (LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (Variant.CanCure(CustomComboPreset.RPR_Variant_Cure, Config.RPR_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RPR_Variant_Rampart))
                return Variant.Rampart;

            if (CanWeave())
            {
                if (ActionReady(ArcaneCircle))
                    return ArcaneCircle;

                if (!HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.Executioner) &&
                    ActionReady(Enshroud) && (Gauge.Shroud >= 50 || HasStatusEffect(Buffs.IdealHost)) &&
                    !IsComboExpiring(6))
                    return Enshroud;

                if (ActionReady(Gluttony) && Gauge.Soul >= 50 && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice))
                    return Gluttony;

                if (LevelChecked(GrimSwathe) && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    !HasStatusEffect(Buffs.Executioner) && Gauge.Soul >= 50 &&
                    (!LevelChecked(Gluttony) || LevelChecked(Gluttony) &&
                        (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5)))
                    return GrimSwathe;

                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    if (Gauge.LemureShroud is 2 && Gauge.VoidShroud is 1 && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                        return OriginalHook(GrimSwathe);
                }

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (LevelChecked(WhorlOfDeath) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < 6 &&
                !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Executioner))
                return WhorlOfDeath;

            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (HasStatusEffect(Buffs.ImmortalSacrifice) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.Executioner) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            if (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))
                return OriginalHook(Guillotine);

            if (HasStatusEffect(Buffs.Enshrouded))
            {
                if (LevelChecked(Communio) &&
                    Gauge.LemureShroud is 1 && Gauge.VoidShroud is 0)
                    return Communio;

                if (Gauge.LemureShroud > 0)
                    return OriginalHook(Guillotine);
            }

            if (!HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.PerfectioParata) &&
                ActionReady(SoulScythe) && Gauge.Soul <= 50)
                return SoulScythe;

            return ComboAction == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                ? OriginalHook(NightmareScythe)
                : actionID;
        }
    }

    internal class RPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not SpinningScythe)
                return actionID;

            //Soulsow
            if (IsEnabled(CustomComboPreset.RPR_AoE_SoulSow) &&
                LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (Variant.CanCure(CustomComboPreset.RPR_Variant_Cure, Config.RPR_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.RPR_Variant_Rampart))
                return Variant.Rampart;

            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.RPR_AoE_ArcaneCircle) &&
                    ActionReady(ArcaneCircle))
                    return ArcaneCircle;

                if (IsEnabled(CustomComboPreset.RPR_AoE_Enshroud) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Enshrouded) &&
                    ActionReady(Enshroud) && (Gauge.Shroud >= 50 || HasStatusEffect(Buffs.IdealHost)) &&
                    !IsComboExpiring(6))
                    return Enshroud;

                if (IsEnabled(CustomComboPreset.RPR_AoE_Gluttony) &&
                    ActionReady(Gluttony) && Gauge.Soul >= 50 && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice))
                    return Gluttony;

                if (IsEnabled(CustomComboPreset.RPR_AoE_GrimSwathe) &&
                    LevelChecked(GrimSwathe) && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    Gauge.Soul >= 50 &&
                    (!LevelChecked(Gluttony) || LevelChecked(Gluttony) &&
                        (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5)))
                    return GrimSwathe;

                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    if (IsEnabled(CustomComboPreset.RPR_AoE_Sacrificium) &&
                        Gauge.LemureShroud is 2 && Gauge.VoidShroud is 1 && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(CustomComboPreset.RPR_AoE_Lemure) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                        return OriginalHook(GrimSwathe);
                }

                if (IsEnabled(CustomComboPreset.RPR_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(Config.RPR_AoESecondWindThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(Config.RPR_AoEBloodbathThreshold))
                        return Role.Bloodbath;
                }
            }

            if (IsEnabled(CustomComboPreset.RPR_AoE_WoD) &&
                ActionReady(WhorlOfDeath) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < 6 &&
                !HasStatusEffect(Buffs.SoulReaver) &&
                GetTargetHPPercent() > Config.RPR_WoDThreshold)
                return WhorlOfDeath;

            if (IsEnabled(CustomComboPreset.RPR_AoE_Perfectio) &&
                HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (IsEnabled(CustomComboPreset.RPR_AoE_PlentifulHarvest) &&
                HasStatusEffect(Buffs.ImmortalSacrifice) &&
                !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Enshrouded) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            if (IsEnabled(CustomComboPreset.RPR_AoE_Guillotine) &&
                (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) &&
                !HasStatusEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))
                return OriginalHook(Guillotine);

            if (HasStatusEffect(Buffs.Enshrouded))
            {
                if (IsEnabled(CustomComboPreset.RPR_AoE_Communio) &&
                    LevelChecked(Communio) &&
                    Gauge.LemureShroud is 1 && Gauge.VoidShroud is 0)
                    return Communio;

                if (IsEnabled(CustomComboPreset.RPR_AoE_Reaping) &&
                    Gauge.LemureShroud > 0)
                    return OriginalHook(Guillotine);
            }

            if (IsEnabled(CustomComboPreset.RPR_AoE_SoulScythe) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.PerfectioParata) &&
                ActionReady(SoulScythe) && Gauge.Soul <= 50)
                return SoulScythe;

            return ComboAction == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                ? OriginalHook(NightmareScythe)
                : actionID;
        }
    }

    internal class RPR_GluttonyBloodSwathe : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_GluttonyBloodSwathe;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case GrimSwathe:
                {
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_OGCD))
                    {
                        if (Gauge.Shroud >= 50 || HasStatusEffect(Buffs.IdealHost))
                            return Enshroud;

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            //Sacrificium
                            if (Gauge.LemureShroud is 2 && HasStatusEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            //Lemure's Slice
                            if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                                return OriginalHook(GrimSwathe);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasStatusEffect(Buffs.PerfectioParata))
                            return OriginalHook(Communio);

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            switch (Gauge.LemureShroud)
                            {
                                case 1 when Gauge.VoidShroud == 0 && LevelChecked(Communio):
                                    return Communio;

                                case 2 when Gauge.VoidShroud is 1 && HasStatusEffect(Buffs.Oblatio):
                                    return OriginalHook(Gluttony);
                            }

                            if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                                return OriginalHook(GrimSwathe);

                            if (Gauge.LemureShroud > 1)
                                return OriginalHook(Guillotine);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                        HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) && LevelChecked(Guillotine))
                        return Guillotine;

                    break;
                }


                case BloodStalk:
                {
                    if (IsEnabled(CustomComboPreset.RPR_TrueNorthGluttony) && Role.CanTrueNorth() &&
                        (GetStatusEffectStacks(Buffs.SoulReaver) is 2 || HasStatusEffect(Buffs.Executioner)))
                        return Role.TrueNorth;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_OGCD))
                    {
                        if (Gauge.Shroud >= 50 || HasStatusEffect(Buffs.IdealHost))
                            return Enshroud;

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            //Sacrificium
                            if (Gauge.LemureShroud is 2 && HasStatusEffect(Buffs.Oblatio))
                                return OriginalHook(Gluttony);

                            //Lemure's Slice
                            if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);
                        }
                    }

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasStatusEffect(Buffs.PerfectioParata))
                            return OriginalHook(Communio);

                        if (HasStatusEffect(Buffs.Enshrouded))
                        {
                            switch (Gauge.LemureShroud)
                            {
                                case 1 when Gauge.VoidShroud == 0 && LevelChecked(Communio):
                                    return Communio;

                                case 2 when Gauge.VoidShroud is 1 && HasStatusEffect(Buffs.Oblatio):
                                    return OriginalHook(Gluttony);
                            }

                            if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);

                            if (HasStatusEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                                !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gallows);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                        HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
                    {
                        if (HasStatusEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasStatusEffect(Buffs.EnhancedGallows) ||
                            !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                    }

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_ArcaneCirclePlentifulHarvest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.RPR_ArcaneCirclePlentifulHarvest;

        protected override uint Invoke(uint actionID) =>
            actionID is ArcaneCircle &&
            HasStatusEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest)
                ? PlentifulHarvest
                : actionID;
    }

    internal class RPR_Regress : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Regress;

        protected override uint Invoke(uint actionID) =>
            actionID is HellsEgress or HellsIngress &&
            GetStatusEffect(Buffs.Threshold)?.RemainingTime <= 9
                ? Regress
                : actionID;
    }

    internal class RPR_Soulsow : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Soulsow;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Harpe or Slice or SpinningScythe) &&
                actionID is not (ShadowOfDeath or BloodStalk))
                return actionID;

            bool[] soulSowOptions = Config.RPR_SoulsowOptions;
            bool soulsowReady = ActionReady(Soulsow) && !HasStatusEffect(Buffs.Soulsow);

            return soulSowOptions.Length > 0 &&
                   (actionID is Harpe && soulSowOptions[0] ||
                    actionID is Slice && soulSowOptions[1] ||
                    actionID is SpinningScythe && soulSowOptions[2] ||
                    actionID is ShadowOfDeath && soulSowOptions[3] ||
                    actionID is BloodStalk && soulSowOptions[4]) && soulsowReady && !InCombat() ||
                   IsEnabled(CustomComboPreset.RPR_Soulsow_Combat) && actionID is Harpe && !HasBattleTarget()
                ? Soulsow
                : actionID;
        }
    }

    internal class RPR_EnshroudProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudProtection;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Enshroud when IsEnabled(CustomComboPreset.RPR_TrueNorthEnshroud) &&
                                   (GetStatusEffectStacks(Buffs.SoulReaver) is 2 || HasStatusEffect(Buffs.Executioner)) &&
                                   Role.CanTrueNorth() && CanDelayedWeave():
                    return Role.TrueNorth;

                case Enshroud:
                {
                    if (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner))
                    {
                        if (HasStatusEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasStatusEffect(Buffs.EnhancedGallows) ||
                            !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                            return OriginalHook(Gallows);
                    }

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_CommunioOnGGG : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_CommunioOnGGG;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Gibbet or Gallows when HasStatusEffect(Buffs.Enshrouded):
                {
                    if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice) && CanWeave())
                        return OriginalHook(BloodStalk);

                    break;
                }

                case Guillotine when HasStatusEffect(Buffs.Enshrouded):
                {
                    if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave())
                        return OriginalHook(GrimSwathe);

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_EnshroudCommunio : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudCommunio;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Enshroud when HasStatusEffect(Buffs.PerfectioParata):
                    return OriginalHook(Communio);

                case Enshroud when HasStatusEffect(Buffs.Enshrouded):
                    return Communio;

                default:
                    return actionID;
            }
        }
    }
}
