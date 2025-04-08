using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class VPR : MeleeJob
{
    internal class VPR_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelFangs)
                return actionID;

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.VPR_Variant_Cure, Config.VPR_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.VPR_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            //oGCDs
            if (CanWeave())
            {
                //Serpents Ire - ForceWeave
                if (InCombat() && !CappedOnCoils &&
                    ActionReady(SerpentsIre) && InBossEncounter())
                    return SerpentsIre;

                // Legacy Weaves
                if (In5Y && TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                    && OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Fury Twin Weaves
                if (HasEffect(Buffs.PoisedForTwinfang))
                    return OriginalHook(Twinfang);

                if (HasEffect(Buffs.PoisedForTwinblood))
                    return OriginalHook(Twinblood);

                //Vice Twin Weaves
                if (!HasEffect(Buffs.Reawakened) && In5Y)
                {
                    if (HasEffect(Buffs.HuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.SwiftskinsVenom))
                        return OriginalHook(Twinblood);
                }
            }

            // Death Rattle - Force to avoid loss
            if (In5Y && LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle)
                return OriginalHook(SerpentsTail);

            //GCDs
            if (LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                return HasRattlingCoilStack(Gauge)
                    ? UncoiledFury
                    : WrithingSnap;

            //Vicewinder Combo
            if (!HasEffect(Buffs.Reawakened) && LevelChecked(Vicewinder) && InMeleeRange())
            {
                // Swiftskin's Coil
                if (VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals()) || HuntersCoilReady)
                    return SwiftskinsCoil;

                // Hunter's Coil
                if (VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals()) || SwiftskinsCoilReady)
                    return HuntersCoil;
            }

            //Reawakend Usage
            if (UseReawaken(Gauge))
                return Reawaken;

            //Overcap protection
            if (CappedOnCoils &&
                (HasCharges(Vicewinder) && !HasEffect(Buffs.SwiftskinsVenom) && !HasEffect(Buffs.HuntersVenom) &&
                 !HasEffect(Buffs.Reawakened) || //spend if Vicewinder is up, after Reawaken
                 IreCD <= GCD * 5)) //spend in case under Reawaken right as Ire comes up
                return UncoiledFury;

            //Vicewinder Usage
            if (HasEffect(Buffs.Swiftscaled) && !IsComboExpiring(3) &&
                ActionReady(Vicewinder) && !HasEffect(Buffs.Reawakened) && InMeleeRange() &&
                (IreCD >= GCD * 5 && InBossEncounter() || !InBossEncounter() || !LevelChecked(SerpentsIre)) &&
                !IsVenomExpiring(3) && !IsHoningExpiring(3))
                return Vicewinder;

            // Uncoiled Fury usage
            if (LevelChecked(UncoiledFury) && HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                !IsComboExpiring(2) &&
                Gauge.RattlingCoilStacks > 1 &&
                !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) &&
                !WasLastWeaponskill(Ouroboros) &&
                !IsEmpowermentExpiring(6) && !IsVenomExpiring(3) &&
                !IsHoningExpiring(3))
                return UncoiledFury;

            //Reawaken combo
            if (ReawakenComboST(ref actionID))
                return actionID;

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingFangs or SteelFangs)
                {
                    if (LevelChecked(HuntersSting) &&
                        (HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)))
                        return OriginalHook(SteelFangs);

                    if (LevelChecked(SwiftskinsSting) &&
                        (HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom) ||
                         !HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingFangs);
                }

                if (ComboAction is HuntersSting or SwiftskinsSting)
                {
                    if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.HindstungVenom)) &&
                        LevelChecked(FlanksbaneFang))
                    {
                        if (Role.CanTrueNorth() && !OnTargetsRear() && HasEffect(Buffs.HindstungVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        if (Role.CanTrueNorth() && !OnTargetsFlank() && HasEffect(Buffs.FlankstungVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        return OriginalHook(SteelFangs);
                    }

                    if ((HasEffect(Buffs.FlanksbaneVenom) || HasEffect(Buffs.HindsbaneVenom)) &&
                        LevelChecked(HindstingStrike))
                    {
                        if (Role.CanTrueNorth() && !OnTargetsRear() && HasEffect(Buffs.HindsbaneVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        if (Role.CanTrueNorth() && !OnTargetsFlank() && HasEffect(Buffs.FlanksbaneVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        return OriginalHook(ReavingFangs);
                    }
                }

                if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                    return LevelChecked(ReavingFangs) && HasEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingFangs)
                        : OriginalHook(SteelFangs);
            }

            //LowLevels
            if (LevelChecked(ReavingFangs) && (HasEffect(Buffs.HonedReavers) ||
                                               !HasEffect(Buffs.HonedReavers) && !HasEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingFangs);
            return actionID;
        }
    }

    internal class VPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelFangs)
                return actionID;

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.VPR_Variant_Cure, Config.VPR_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.VPR_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            // Opener for VPR
            if (IsEnabled(CustomComboPreset.VPR_ST_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            //oGCDs
            if (CanWeave())
            {
                //Serpents Ire
                if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsIre) && InCombat() &&
                    !CappedOnCoils && ActionReady(SerpentsIre) &&
                    (Config.VPR_ST_SerpentsIre_SubOption == 0 ||
                     Config.VPR_ST_SerpentsIre_SubOption == 1 && InBossEncounter()))
                    return SerpentsIre;

                // Death Rattle
                if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsTail) && In5Y &&
                    LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle)
                    return OriginalHook(SerpentsTail);

                // Legacy Weaves
                if (IsEnabled(CustomComboPreset.VPR_ST_LegacyWeaves) && In5Y &&
                    TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                    && OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Fury Twin Weaves
                if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFuryCombo))
                {
                    if (HasEffect(Buffs.PoisedForTwinfang))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.PoisedForTwinblood))
                        return OriginalHook(Twinblood);
                }

                //Vice Twin Weaves
                if (IsEnabled(CustomComboPreset.VPR_ST_VicewinderWeaves) &&
                    !HasEffect(Buffs.Reawakened) && In5Y)
                {
                    if (HasEffect(Buffs.HuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.SwiftskinsVenom))
                        return OriginalHook(Twinblood);
                }
            }

            // Death Rattle - Force to avoid loss
            if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsTail) && In5Y &&
                LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle)
                return OriginalHook(SerpentsTail);

            //GCDs
            if (IsEnabled(CustomComboPreset.VPR_ST_RangedUptime) &&
                LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                return IsEnabled(CustomComboPreset.VPR_ST_RangedUptimeUncoiledFury) &&
                       HasRattlingCoilStack(Gauge)
                    ? UncoiledFury
                    : WrithingSnap;

            //Vicewinder Combo
            if (IsEnabled(CustomComboPreset.VPR_ST_VicewinderCombo) &&
                !HasEffect(Buffs.Reawakened) && LevelChecked(Vicewinder) && InMeleeRange())
            {
                // Swiftskin's Coil
                if (VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals()) || HuntersCoilReady)
                    return SwiftskinsCoil;

                // Hunter's Coil
                if (VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals()) || SwiftskinsCoilReady)
                    return HuntersCoil;
            }

            //Reawakend Usage
            if (IsEnabled(CustomComboPreset.VPR_ST_Reawaken) &&
                UseReawaken(Gauge) &&
                (Config.VPR_ST_ReAwaken_SubOption == 0 ||
                 Config.VPR_ST_ReAwaken_SubOption == 1 && InBossEncounter()))
                return Reawaken;

            //Overcap protection
            if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFury) && CappedOnCoils &&
                (HasCharges(Vicewinder) && !HasEffect(Buffs.SwiftskinsVenom) && !HasEffect(Buffs.HuntersVenom) &&
                 !HasEffect(Buffs.Reawakened) || //spend if Vicewinder is up, after Reawaken
                 IreCD <= GCD * 5)) //spend in case under Reawaken right as Ire comes up
                return UncoiledFury;

            //Vicewinder Usage
            if (IsEnabled(CustomComboPreset.VPR_ST_Vicewinder) && HasEffect(Buffs.Swiftscaled) &&
                !IsComboExpiring(3) &&
                ActionReady(Vicewinder) && !HasEffect(Buffs.Reawakened) && InMeleeRange() &&
                (IreCD >= GCD * 5 && InBossEncounter() || !InBossEncounter() || !LevelChecked(SerpentsIre)) &&
                !IsVenomExpiring(3) && !IsHoningExpiring(3))
                return Vicewinder;

            // Uncoiled Fury usage
            if (IsEnabled(CustomComboPreset.VPR_ST_UncoiledFury) && !IsComboExpiring(2) &&
                LevelChecked(UncoiledFury) && HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                (Gauge.RattlingCoilStacks > Config.VPR_ST_UncoiledFury_HoldCharges ||
                 GetTargetHPPercent() < Config.VPR_ST_UncoiledFury_Threshold && HasRattlingCoilStack(Gauge)) &&
                !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.ReadyToReawaken) &&
                !WasLastWeaponskill(Ouroboros) &&
                !IsEmpowermentExpiring(3))
                return UncoiledFury;

            //Reawaken combo
            if (IsEnabled(CustomComboPreset.VPR_ST_GenerationCombo) &&
                ReawakenComboST(ref actionID))
                return actionID;

            // healing
            if (IsEnabled(CustomComboPreset.VPR_ST_ComboHeals))
            {
                if (Role.CanSecondWind(Config.VPR_ST_SecondWind_Threshold))
                    return Role.SecondWind;

                if (Role.CanBloodBath(Config.VPR_ST_Bloodbath_Threshold))
                    return Role.Bloodbath;
            }

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingFangs or SteelFangs)
                {
                    if (LevelChecked(HuntersSting) &&
                        (HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)))
                        return OriginalHook(SteelFangs);

                    if (LevelChecked(SwiftskinsSting) &&
                        (HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom) ||
                         !HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingFangs);
                }

                if (ComboAction is HuntersSting or SwiftskinsSting)
                {
                    if ((HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.HindstungVenom)) &&
                        LevelChecked(FlanksbaneFang))
                    {
                        if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                            Role.CanTrueNorth() && !OnTargetsRear() && HasEffect(Buffs.HindstungVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                            Role.CanTrueNorth() && !OnTargetsFlank() && HasEffect(Buffs.FlankstungVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        return OriginalHook(SteelFangs);
                    }

                    if ((HasEffect(Buffs.FlanksbaneVenom) || HasEffect(Buffs.HindsbaneVenom)) &&
                        LevelChecked(HindstingStrike))
                    {
                        if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                            Role.CanTrueNorth() && !OnTargetsRear() && HasEffect(Buffs.HindsbaneVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        if (IsEnabled(CustomComboPreset.VPR_TrueNorthDynamic) &&
                            Role.CanTrueNorth() && !OnTargetsFlank() && HasEffect(Buffs.FlanksbaneVenom) &&
                            CanDelayedWeave())
                            return Role.TrueNorth;

                        return OriginalHook(ReavingFangs);
                    }
                }

                if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                    return LevelChecked(ReavingFangs) && HasEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingFangs)
                        : OriginalHook(SteelFangs);
            }

            //LowLevels
            if (LevelChecked(ReavingFangs) && (HasEffect(Buffs.HonedReavers) ||
                                               !HasEffect(Buffs.HonedReavers) && !HasEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingFangs);

            return actionID;
        }
    }

    internal class VPR_AoE_Simplemode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelMaw)
                return actionID;

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.VPR_Variant_Cure, Config.VPR_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.VPR_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (CanWeave())
            {
                // Death Rattle
                if (LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is LastLash)
                    return OriginalHook(SerpentsTail);

                // Legacy Weaves
                if (TraitLevelChecked(Traits.SerpentsLegacy) &&
                    HasEffect(Buffs.Reawakened) &&
                    OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Uncoiled combo
                if (HasEffect(Buffs.PoisedForTwinfang))
                    return OriginalHook(Twinfang);

                if (HasEffect(Buffs.PoisedForTwinblood))
                    return OriginalHook(Twinblood);

                if (!HasEffect(Buffs.Reawakened))
                {
                    //Vicepit weaves
                    if (HasEffect(Buffs.FellhuntersVenom) && In5Y)
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.FellskinsVenom) && In5Y)
                        return OriginalHook(Twinblood);

                    //Serpents Ire usage
                    if (!CappedOnCoils && ActionReady(SerpentsIre))
                        return SerpentsIre;
                }
            }

            //Vicepit combo
            if (!HasEffect(Buffs.Reawakened) && In5Y)
            {
                if (SwiftskinsDenReady)
                    return HuntersDen;

                if (VicepitReady)
                    return SwiftskinsDen;
            }

            //Reawakend Usage
            if ((HasEffect(Buffs.ReadyToReawaken) || Gauge.SerpentOffering >= 50) && LevelChecked(Reawaken) &&
                HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                !HasEffect(Buffs.Reawakened) && In5Y &&
                !HasEffect(Buffs.FellhuntersVenom) && !HasEffect(Buffs.FellskinsVenom) &&
                !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))
                return Reawaken;

            //Overcap protection
            if ((HasCharges(Vicepit) && !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom) ||
                 IreCD <= GCD * 2) && !HasEffect(Buffs.Reawakened) && CappedOnCoils)
                return UncoiledFury;

            //Vicepit Usage
            if (ActionReady(Vicepit) && !HasEffect(Buffs.Reawakened) &&
                (IreCD >= GCD * 5 || !LevelChecked(SerpentsIre)) && In5Y)
                return Vicepit;

            // Uncoiled Fury usage
            if (LevelChecked(UncoiledFury) &&
                HasRattlingCoilStack(Gauge) &&
                HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.FellskinsVenom) &&
                !HasEffect(Buffs.FellhuntersVenom) &&
                !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) && !WasLastAbility(SerpentsIre))
                return UncoiledFury;

            //Reawaken combo
            if (ReawakenComboAoE(ref actionID))
                return actionID;

            // healing
            if (Role.CanSecondWind(25))
                return Role.SecondWind;

            if (Role.CanBloodBath(40))
                return Role.Bloodbath;

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingMaw or SteelMaw)
                {
                    if (LevelChecked(HuntersBite) &&
                        HasEffect(Buffs.GrimhuntersVenom))
                        return OriginalHook(SteelMaw);

                    if (LevelChecked(SwiftskinsBite) &&
                        (HasEffect(Buffs.GrimskinsVenom) ||
                         !HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is HuntersBite or SwiftskinsBite)
                {
                    if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                        return OriginalHook(SteelMaw);

                    if (HasEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is BloodiedMaw or JaggedMaw)
                    return LevelChecked(ReavingMaw) && HasEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingMaw)
                        : OriginalHook(SteelMaw);
            }

            //for lower lvls
            if (LevelChecked(ReavingMaw) && (HasEffect(Buffs.HonedReavers)
                                             || !HasEffect(Buffs.HonedReavers) && !HasEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingMaw);

            return actionID;
        }
    }

    internal class VPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelMaw)
                return actionID;

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.VPR_Variant_Cure, Config.VPR_VariantCure))
                return Variant.Cure;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.VPR_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (CanWeave())
            {
                // Death Rattle
                if (IsEnabled(CustomComboPreset.VPR_AoE_SerpentsTail) &&
                    LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is LastLash)
                    return OriginalHook(SerpentsTail);

                // Legacy Weaves
                if (IsEnabled(CustomComboPreset.VPR_AoE_ReawakenCombo) &&
                    TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                    && OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Uncoiled combo
                if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFuryCombo))
                {
                    if (HasEffect(Buffs.PoisedForTwinfang))
                        return OriginalHook(Twinfang);

                    if (HasEffect(Buffs.PoisedForTwinblood))
                        return OriginalHook(Twinblood);
                }

                if (!HasEffect(Buffs.Reawakened))
                {
                    //Vicepit weaves
                    if (IsEnabled(CustomComboPreset.VPR_AoE_VicepitWeaves) &&
                        (In5Y || IsEnabled(CustomComboPreset.VPR_AoE_VicepitCombo_DisableRange)))
                    {
                        if (HasEffect(Buffs.FellhuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.FellskinsVenom))
                            return OriginalHook(Twinblood);
                    }

                    //Serpents Ire usage
                    if (IsEnabled(CustomComboPreset.VPR_AoE_SerpentsIre) &&
                        !CappedOnCoils && ActionReady(SerpentsIre))
                        return SerpentsIre;
                }
            }

            //Vicepit combo
            if (IsEnabled(CustomComboPreset.VPR_AoE_VicepitCombo) &&
                !HasEffect(Buffs.Reawakened) &&
                (In5Y || IsEnabled(CustomComboPreset.VPR_AoE_VicepitCombo_DisableRange)))
            {
                if (SwiftskinsDenReady)
                    return HuntersDen;

                if (VicepitReady)
                    return SwiftskinsDen;
            }

            //Reawakend Usage
            if (IsEnabled(CustomComboPreset.VPR_AoE_Reawaken) &&
                GetTargetHPPercent() > Config.VPR_AoE_Reawaken_Usage &&
                (HasEffect(Buffs.ReadyToReawaken) || Gauge.SerpentOffering >= 50) && LevelChecked(Reawaken) &&
                HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                !HasEffect(Buffs.Reawakened) &&
                (In5Y || IsEnabled(CustomComboPreset.VPR_AoE_Reawaken_DisableRange)) &&
                !HasEffect(Buffs.FellhuntersVenom) && !HasEffect(Buffs.FellskinsVenom) &&
                !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang))
                return Reawaken;

            //Overcap protection
            if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFury) &&
                (HasCharges(Vicepit) && !HasEffect(Buffs.FellskinsVenom) && !HasEffect(Buffs.FellhuntersVenom) ||
                 IreCD <= GCD * 2) && !HasEffect(Buffs.Reawakened) && CappedOnCoils)
                return UncoiledFury;

            //Vicepit Usage
            if (IsEnabled(CustomComboPreset.VPR_AoE_Vicepit) &&
                ActionReady(Vicepit) && !HasEffect(Buffs.Reawakened) &&
                (In5Y || IsEnabled(CustomComboPreset.VPR_AoE_Vicepit_DisableRange)) &&
                (IreCD >= GCD * 5 || !LevelChecked(SerpentsIre)))
                return Vicepit;

            // Uncoiled Fury usage
            if (IsEnabled(CustomComboPreset.VPR_AoE_UncoiledFury) &&
                LevelChecked(UncoiledFury) &&
                (Gauge.RattlingCoilStacks > Config.VPR_AoE_UncoiledFury_HoldCharges ||
                 GetTargetHPPercent() < Config.VPR_AoE_UncoiledFury_Threshold &&
                 HasRattlingCoilStack(Gauge)) &&
                HasEffect(Buffs.Swiftscaled) && HasEffect(Buffs.HuntersInstinct) &&
                !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                !HasEffect(Buffs.Reawakened) && !HasEffect(Buffs.FellskinsVenom) &&
                !HasEffect(Buffs.FellhuntersVenom) &&
                !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) && !WasLastAbility(SerpentsIre))
                return UncoiledFury;

            //Reawaken combo
            if (IsEnabled(CustomComboPreset.VPR_AoE_ReawakenCombo) &&
                ReawakenComboAoE(ref actionID))
                return actionID;

            // healing
            if (IsEnabled(CustomComboPreset.VPR_AoE_ComboHeals))
            {
                if (Role.CanSecondWind(Config.VPR_AoE_SecondWind_Threshold))
                    return Role.SecondWind;

                if (Role.CanBloodBath(Config.VPR_AoE_Bloodbath_Threshold))
                    return Role.Bloodbath;
            }

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingMaw or SteelMaw)
                {
                    if (LevelChecked(HuntersBite) &&
                        HasEffect(Buffs.GrimhuntersVenom))
                        return OriginalHook(SteelMaw);

                    if (LevelChecked(SwiftskinsBite) &&
                        (HasEffect(Buffs.GrimskinsVenom) ||
                         !HasEffect(Buffs.Swiftscaled) && !HasEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is HuntersBite or SwiftskinsBite)
                {
                    if (HasEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                        return OriginalHook(SteelMaw);

                    if (HasEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is BloodiedMaw or JaggedMaw)
                    return LevelChecked(ReavingMaw) && HasEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingMaw)
                        : OriginalHook(SteelMaw);
            }

            //for lower lvls
            if (LevelChecked(ReavingMaw) && (HasEffect(Buffs.HonedReavers)
                                             || !HasEffect(Buffs.HonedReavers) && !HasEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingMaw);

            return actionID;
        }
    }

    internal class VPR_VicewinderCoils : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_VicewinderCoils;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Vicewinder:
                {
                    if (IsEnabled(CustomComboPreset.VPR_VicewinderCoils_oGCDs))
                    {
                        if (HasEffect(Buffs.HuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.SwiftskinsVenom))
                            return OriginalHook(Twinblood);
                    }

                    // Vicewinder Combo
                    if (LevelChecked(Vicewinder))
                    {
                        // Swiftskin's Coil
                        if (VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals()) || HuntersCoilReady)
                            return SwiftskinsCoil;

                        // Hunter's Coil
                        if (VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals()) || SwiftskinsCoilReady)
                            return HuntersCoil;
                    }

                    break;
                }
            }

            return actionID;
        }
    }

    internal class VPR_VicepitDens : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_VicepitDens;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Vicepit:
                {
                    if (IsEnabled(CustomComboPreset.VPR_VicepitDens_oGCDs))
                    {
                        if (HasEffect(Buffs.FellhuntersVenom))
                            return OriginalHook(Twinfang);

                        if (HasEffect(Buffs.FellskinsVenom))
                            return OriginalHook(Twinblood);
                    }

                    if (SwiftskinsDenReady)
                        return HuntersDen;

                    if (VicepitReady)
                        return SwiftskinsDen;

                    break;
                }
            }

            return actionID;
        }
    }

    internal class VPR_UncoiledTwins : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_UncoiledTwins;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case UncoiledFury when HasEffect(Buffs.PoisedForTwinfang):
                    return OriginalHook(Twinfang);

                case UncoiledFury when HasEffect(Buffs.PoisedForTwinblood):
                    return OriginalHook(Twinblood);

                default:
                    return actionID;
            }
        }
    }

    internal class VPR_ReawakenLegacy : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ReawakenLegacy;

        protected override uint Invoke(uint actionID)
        {
            int buttonChoice = Config.VPR_ReawakenLegacyButton;

            switch (buttonChoice)
            {
                case 0 when actionID is Reawaken && HasEffect(Buffs.Reawakened):
                case 1 when actionID is ReavingFangs && HasEffect(Buffs.Reawakened):
                {
                    // Legacy Weaves
                    if (IsEnabled(CustomComboPreset.VPR_ReawakenLegacyWeaves) &&
                        TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                        && OriginalHook(SerpentsTail) is not SerpentsTail)
                        return OriginalHook(SerpentsTail);

                    #region Pre Ouroboros

                    if (!TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        switch (Gauge.AnguineTribute)
                        {
                            case 4:
                                return OriginalHook(SteelFangs);

                            case 3:
                                return OriginalHook(ReavingFangs);

                            case 2:
                                return OriginalHook(HuntersCoil);

                            case 1:
                                return OriginalHook(SwiftskinsCoil);
                        }

                    #endregion

                    #region With Ouroboros

                    if (TraitLevelChecked(Traits.EnhancedSerpentsLineage))
                        switch (Gauge.AnguineTribute)
                        {
                            case 5:
                                return OriginalHook(SteelFangs);

                            case 4:
                                return OriginalHook(ReavingFangs);

                            case 3:
                                return OriginalHook(HuntersCoil);

                            case 2:
                                return OriginalHook(SwiftskinsCoil);

                            case 1:
                                return OriginalHook(Reawaken);
                        }

                    #endregion

                    break;
                }
            }

            return actionID;
        }
    }

    internal class VPR_TwinTails : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_TwinTails;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                // Death Rattle
                case SerpentsTail when LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is DeathRattle:
                case SerpentsTail when TraitLevelChecked(Traits.SerpentsLegacy) && HasEffect(Buffs.Reawakened)
                                                                                && OriginalHook(SerpentsTail) is not SerpentsTail:
                    return OriginalHook(SerpentsTail);

                // Legacy Weaves
                case SerpentsTail when HasEffect(Buffs.PoisedForTwinfang):
                    return OriginalHook(Twinfang);

                case SerpentsTail when HasEffect(Buffs.PoisedForTwinblood):
                    return OriginalHook(Twinblood);

                case SerpentsTail when HasEffect(Buffs.HuntersVenom):
                    return OriginalHook(Twinfang);

                case SerpentsTail when HasEffect(Buffs.SwiftskinsVenom):
                    return OriginalHook(Twinblood);

                case SerpentsTail when HasEffect(Buffs.FellhuntersVenom):
                    return OriginalHook(Twinfang);

                case SerpentsTail when HasEffect(Buffs.FellskinsVenom):
                    return OriginalHook(Twinblood);

                default:
                    return actionID;
            }
        }
    }

    internal class VPR_Legacies : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_Legacies;

        protected override uint Invoke(uint actionID)
        {
            if (!HasEffect(Buffs.Reawakened))
                return actionID;

            //Reawaken combo
            switch (actionID)
            {
                case SteelFangs when WasLastAction(OriginalHook(SteelFangs)) && Gauge.AnguineTribute is 4:
                case ReavingFangs when WasLastAction(OriginalHook(ReavingFangs)) && Gauge.AnguineTribute is 3:
                case HuntersCoil when WasLastAction(OriginalHook(HuntersCoil)) && Gauge.AnguineTribute is 2:
                case SwiftskinsCoil when WasLastAction(OriginalHook(SwiftskinsCoil)) && Gauge.AnguineTribute is 1:
                    return OriginalHook(SerpentsTail);
            }

            return actionID;
        }
    }

    internal class VPR_SerpentsTail : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_SerpentsTail;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case SteelFangs or ReavingFangs when
                    OriginalHook(SerpentsTail) is DeathRattle &&
                    (WasLastWeaponskill(FlankstingStrike) || WasLastWeaponskill(FlanksbaneFang) ||
                     WasLastWeaponskill(HindstingStrike) || WasLastWeaponskill(HindsbaneFang)):
                case SteelMaw or ReavingMaw when
                    OriginalHook(SerpentsTail) is LastLash &&
                    (WasLastWeaponskill(JaggedMaw) || WasLastWeaponskill(BloodiedMaw)):
                    return OriginalHook(SerpentsTail);

                default:
                    return actionID;
            }
        }
    }
}
