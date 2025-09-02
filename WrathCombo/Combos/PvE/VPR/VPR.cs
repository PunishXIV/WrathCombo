using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.VPR.Config;
namespace WrathCombo.Combos.PvE;

internal partial class VPR : Melee
{
    internal class VPR_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelFangs)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //oGCDs
            if (CanWeave()
                )
            {
                //Serpents Ire
                if (InCombat() && !CappedOnCoils() &&
                    ActionReady(SerpentsIre))
                    return SerpentsIre;

                // Legacy Weaves
                if (OriginalHook(SerpentsTail) is not SerpentsTail &&
                    InRange())
                    return OriginalHook(SerpentsTail);

                // Fury Twin Weaves
                if (HasStatusEffect(Buffs.PoisedForTwinfang))
                    return OriginalHook(Twinfang);

                if (HasStatusEffect(Buffs.PoisedForTwinblood))
                    return OriginalHook(Twinblood);

                //Vice Twin Weaves
                if (!HasStatusEffect(Buffs.Reawakened) && InRange())
                {
                    if (HasStatusEffect(Buffs.HuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasStatusEffect(Buffs.SwiftskinsVenom))
                        return OriginalHook(Twinblood);
                }
            }

            //Ranged
            if (LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                return HasRattlingCoilStack()
                    ? UncoiledFury
                    : WrithingSnap;

            //Vicewinder Combo
            if (!HasStatusEffect(Buffs.Reawakened) &&
                LevelChecked(Vicewinder) && InMeleeRange())
            {
                // Swiftskin's Coil
                if (VicewinderReady &&
                    (!OnTargetsFlank() ||
                     !TargetNeedsPositionals()) ||
                    HuntersCoilReady)
                    return SwiftskinsCoil;

                // Hunter's Coil
                if (VicewinderReady &&
                    (!OnTargetsRear() ||
                     !TargetNeedsPositionals()) ||
                    SwiftskinsCoilReady)
                    return HuntersCoil;
            }

            //Reawakend Usage
            if (UseReawaken())
                return Reawaken;

            //Overcap protection
            if (CappedOnCoils() &&
                (HasCharges(Vicewinder) && !HasStatusEffect(Buffs.SwiftskinsVenom) &&
                 !HasStatusEffect(Buffs.HuntersVenom) && !HasStatusEffect(Buffs.Reawakened) || //spend if Vicewinder is up, after Reawaken
                 IreCD <= GCD * 5)) //spend in case under Reawaken right as Ire comes up
                return UncoiledFury;

            //Vicewinder Usage
            if (HasStatusEffect(Buffs.Swiftscaled) && !IsComboExpiring(3) &&
                ActionReady(Vicewinder) && !HasStatusEffect(Buffs.Reawakened) &&
                InMeleeRange() &&
                (IreCD >= GCD * 5 || !LevelChecked(SerpentsIre)) &&
                !IsVenomExpiring(3) && !IsHoningExpiring(3))
                return Role.CanTrueNorth()
                    ? Role.TrueNorth
                    : Vicewinder;

            // Uncoiled Fury usage
            if (ActionReady(UncoiledFury) &&
                HasStatusEffect(Buffs.Swiftscaled) &&
                HasStatusEffect(Buffs.HuntersInstinct) &&
                !IsComboExpiring(2) &&
                RattlingCoilStacks > 1 &&
                !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                !HasStatusEffect(Buffs.Reawakened) && !HasStatusEffect(Buffs.ReadyToReawaken) &&
                !WasLastWeaponskill(Ouroboros) &&
                !IsEmpowermentExpiring(6) && !IsVenomExpiring(3) &&
                !IsHoningExpiring(3))
                return UncoiledFury;

            //Reawaken combo
            if (ReawakenComboST(ref actionID))
                return actionID;

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasStatusEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingFangs or SteelFangs)
                {
                    if (LevelChecked(SwiftskinsSting) &&
                        (HasHindVenom() || NoSwiftscaled() || NoVenom()))
                        return OriginalHook(ReavingFangs);

                    if (LevelChecked(HuntersSting) && (HasFlankVenom() || NoHuntersInstinct()))
                        return OriginalHook(SteelFangs);
                }

                if (ComboAction is HuntersSting or SwiftskinsSting)
                {
                    if ((HasStatusEffect(Buffs.FlankstungVenom) || HasStatusEffect(Buffs.HindstungVenom)) &&
                        LevelChecked(FlanksbaneFang))
                        return Role.CanTrueNorth() &&
                               (!OnTargetsRear() && HasStatusEffect(Buffs.HindstungVenom) ||
                                !OnTargetsFlank() && HasStatusEffect(Buffs.FlankstungVenom))
                            ? Role.TrueNorth
                            : OriginalHook(SteelFangs);


                    if ((HasStatusEffect(Buffs.FlanksbaneVenom) || HasStatusEffect(Buffs.HindsbaneVenom)) &&
                        LevelChecked(HindstingStrike))
                        return Role.CanTrueNorth() &&
                               (!OnTargetsRear() && HasStatusEffect(Buffs.HindsbaneVenom) ||
                                OnTargetsFlank() && HasStatusEffect(Buffs.FlanksbaneVenom))
                            ? Role.TrueNorth
                            : OriginalHook(ReavingFangs);
                }

                if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                    return LevelChecked(ReavingFangs) && HasStatusEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingFangs)
                        : OriginalHook(SteelFangs);
            }

            //LowLevels
            if (LevelChecked(ReavingFangs) &&
                (HasStatusEffect(Buffs.HonedReavers) ||
                 !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingFangs);

            return actionID;
        }
    }

    internal class VPR_AoE_Simplemode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelMaw)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                // Death Rattle / Legacy Weaves
                if (LevelChecked(SerpentsTail) &&
                    OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Uncoiled combo
                if (HasStatusEffect(Buffs.PoisedForTwinfang))
                    return OriginalHook(Twinfang);

                if (HasStatusEffect(Buffs.PoisedForTwinblood))
                    return OriginalHook(Twinblood);

                if (!HasStatusEffect(Buffs.Reawakened))
                {
                    //Vicepit weaves
                    if (HasStatusEffect(Buffs.FellhuntersVenom) &&
                        InActionRange(TwinfangThresh))
                        return OriginalHook(Twinfang);

                    if (HasStatusEffect(Buffs.FellskinsVenom) &&
                        InActionRange(TwinbloodThresh))
                        return OriginalHook(Twinblood);

                    //Serpents Ire usage
                    if (!CappedOnCoils() && ActionReady(SerpentsIre))
                        return SerpentsIre;
                }
            }

            //Vicepit combo
            if (!HasStatusEffect(Buffs.Reawakened))
            {
                if (SwiftskinsDenReady &&
                    InActionRange(HuntersDen))
                    return HuntersDen;

                if (VicepitReady &&
                    InActionRange(SwiftskinsDen))
                    return SwiftskinsDen;
            }

            //Reawakend Usage
            if ((HasStatusEffect(Buffs.ReadyToReawaken) || SerpentOffering >= 50) &&
                LevelChecked(Reawaken) && InActionRange(Reawaken) &&
                HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                !HasStatusEffect(Buffs.Reawakened) &&
                !HasStatusEffect(Buffs.FellhuntersVenom) && !HasStatusEffect(Buffs.FellskinsVenom) &&
                !HasStatusEffect(Buffs.PoisedForTwinblood) && !HasStatusEffect(Buffs.PoisedForTwinfang))
                return Reawaken;

            //Overcap protection
            if ((HasCharges(Vicepit) && !HasStatusEffect(Buffs.FellskinsVenom) && !HasStatusEffect(Buffs.FellhuntersVenom) ||
                 IreCD <= GCD * 2) && !HasStatusEffect(Buffs.Reawakened) && CappedOnCoils())
                return UncoiledFury;

            //Vicepit Usage
            if (ActionReady(Vicepit) &&
                !HasStatusEffect(Buffs.Reawakened) &&
                InActionRange(Vicepit) &&
                (IreCD >= GCD * 5 || !LevelChecked(SerpentsIre)))
                return Vicepit;

            // Uncoiled Fury usage
            if (ActionReady(UncoiledFury) &&
                HasRattlingCoilStack() &&
                HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                !HasStatusEffect(Buffs.Reawakened) && !HasStatusEffect(Buffs.FellskinsVenom) &&
                !HasStatusEffect(Buffs.FellhuntersVenom) &&
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
            if (ComboTimer > 0 && !HasStatusEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingMaw or SteelMaw)
                {
                    if (LevelChecked(HuntersBite) &&
                        HasStatusEffect(Buffs.GrimhuntersVenom))
                        return OriginalHook(SteelMaw);

                    if (LevelChecked(SwiftskinsBite) &&
                        (HasStatusEffect(Buffs.GrimskinsVenom) ||
                         !HasStatusEffect(Buffs.Swiftscaled) && !HasStatusEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is HuntersBite or SwiftskinsBite)
                {
                    if (HasStatusEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                        return OriginalHook(SteelMaw);

                    if (HasStatusEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is BloodiedMaw or JaggedMaw)
                    return LevelChecked(ReavingMaw) && HasStatusEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingMaw)
                        : OriginalHook(SteelMaw);
            }

            //for lower lvls
            if (LevelChecked(ReavingMaw) &&
                (HasStatusEffect(Buffs.HonedReavers) ||
                 !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingMaw);

            return actionID;
        }
    }

    internal class VPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelFangs)
                return actionID;

            // Opener for VPR
            if (IsEnabled(Preset.VPR_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //oGCDs
            if (CanWeave())
            {
                //Serpents Ire
                if (IsEnabled(Preset.VPR_ST_SerpentsIre) && InCombat() &&
                    !CappedOnCoils() && ActionReady(SerpentsIre) &&
                    (VPR_ST_SerpentsIre_SubOption == 0 || InBossEncounter()))
                    return SerpentsIre;

                // Death Rattle / Legacy Weaves
                if ((IsEnabled(Preset.VPR_ST_SerpentsTail) ||
                     IsEnabled(Preset.VPR_ST_LegacyWeaves)) &&
                    LevelChecked(SerpentsTail) && InRange() &&
                    OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Fury Twin Weaves
                if (IsEnabled(Preset.VPR_ST_UncoiledFuryCombo))
                {
                    if (HasStatusEffect(Buffs.PoisedForTwinfang))
                        return OriginalHook(Twinfang);

                    if (HasStatusEffect(Buffs.PoisedForTwinblood))
                        return OriginalHook(Twinblood);
                }

                //Vice Twin Weaves
                if (IsEnabled(Preset.VPR_ST_VicewinderWeaves) &&
                    !HasStatusEffect(Buffs.Reawakened) && InMeleeRange())
                {
                    if (HasStatusEffect(Buffs.HuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasStatusEffect(Buffs.SwiftskinsVenom))
                        return OriginalHook(Twinblood);
                }

                if (IsEnabled(Preset.VPR_ST_Feint) &&
                    Role.CanFeint() &&
                    RaidWideCasting())
                    return Role.Feint;

                // healing
                if (IsEnabled(Preset.VPR_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(VPR_ST_SecondWind_Threshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(VPR_ST_Bloodbath_Threshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.VPR_ST_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep() &&
                    !TargetIsBoss() && TargetIsCasting())
                    return Role.LegSweep;
            }

            //Ranged
            if (IsEnabled(Preset.VPR_ST_RangedUptime) &&
                LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                return VPR_ST_RangedUptimeUncoiledFury &&
                       HasRattlingCoilStack()
                    ? UncoiledFury
                    : WrithingSnap;

            //Vicewinder Combo
            if (IsEnabled(Preset.VPR_ST_VicewinderCombo) &&
                !HasStatusEffect(Buffs.Reawakened) &&
                LevelChecked(Vicewinder) && InMeleeRange())
            {
                // Swiftskin's Coil
                if (VicewinderReady &&
                    (!OnTargetsFlank() ||
                     !TargetNeedsPositionals()) ||
                    HuntersCoilReady)
                    return SwiftskinsCoil;

                // Hunter's Coil
                if (VicewinderReady &&
                    (!OnTargetsRear() ||
                     !TargetNeedsPositionals()) ||
                    SwiftskinsCoilReady)
                    return HuntersCoil;
            }

            //Reawakend Usage
            if (IsEnabled(Preset.VPR_ST_Reawaken) &&
                UseReawaken() &&
                (VPR_ST_ReAwaken_SubOption == 0 || InBossEncounter()))
                return Reawaken;

            //Overcap protection
            if (IsEnabled(Preset.VPR_ST_UncoiledFury) && CappedOnCoils() &&
                (HasCharges(Vicewinder) && !HasStatusEffect(Buffs.SwiftskinsVenom) &&
                 !HasStatusEffect(Buffs.HuntersVenom) && !HasStatusEffect(Buffs.Reawakened) || //spend if Vicewinder is up, after Reawaken
                 IreCD <= GCD * 5)) //spend in case under Reawaken right as Ire comes up
                return UncoiledFury;

            //Vicewinder Usage
            if (IsEnabled(Preset.VPR_ST_Vicewinder) &&
                HasStatusEffect(Buffs.Swiftscaled) && !IsComboExpiring(3) &&
                ActionReady(Vicewinder) && !HasStatusEffect(Buffs.Reawakened) && InMeleeRange() &&
                (IreCD >= GCD * 5 && InBossEncounter() || !InBossEncounter() || !LevelChecked(SerpentsIre)) &&
                !IsVenomExpiring(3) && !IsHoningExpiring(3))
                return VPR_TrueNortVicewinder &&
                       Role.CanTrueNorth()
                    ? Role.TrueNorth
                    : Vicewinder;

            // Uncoiled Fury usage
            if (IsEnabled(Preset.VPR_ST_UncoiledFury) && !IsComboExpiring(2) &&
                ActionReady(UncoiledFury) && HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                (RattlingCoilStacks > VPR_ST_UncoiledFury_HoldCharges ||
                 GetTargetHPPercent() < VPR_ST_UncoiledFury_Threshold && HasRattlingCoilStack()) &&
                !VicewinderReady && !HuntersCoilReady && !SwiftskinsCoilReady &&
                !HasStatusEffect(Buffs.Reawakened) && !HasStatusEffect(Buffs.ReadyToReawaken) &&
                !WasLastWeaponskill(Ouroboros) && !IsEmpowermentExpiring(3))
                return UncoiledFury;

            //Reawaken combo
            if (IsEnabled(Preset.VPR_ST_GenerationCombo) &&
                ReawakenComboST(ref actionID))
                return actionID;

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasStatusEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingFangs or SteelFangs)
                {
                    if (LevelChecked(SwiftskinsSting) &&
                        (HasHindVenom() || NoSwiftscaled() || NoVenom()))
                        return OriginalHook(ReavingFangs);

                    if (LevelChecked(HuntersSting) && (HasFlankVenom() || NoHuntersInstinct()))
                        return OriginalHook(SteelFangs);
                }


                if (ComboAction is HuntersSting or SwiftskinsSting)
                {
                    if ((HasStatusEffect(Buffs.FlanksbaneVenom) || HasStatusEffect(Buffs.HindsbaneVenom)) &&
                        LevelChecked(HindstingStrike))
                        return IsEnabled(Preset.VPR_TrueNorthDynamic) &&
                               Role.CanTrueNorth() &&
                               (!OnTargetsRear() && HasStatusEffect(Buffs.HindsbaneVenom) ||
                                !OnTargetsFlank() && HasStatusEffect(Buffs.FlanksbaneVenom))
                            ? Role.TrueNorth
                            : OriginalHook(ReavingFangs);

                    if ((HasStatusEffect(Buffs.FlankstungVenom) || HasStatusEffect(Buffs.HindstungVenom)) &&
                        LevelChecked(FlanksbaneFang))
                        return IsEnabled(Preset.VPR_TrueNorthDynamic) &&
                               Role.CanTrueNorth() &&
                               (!OnTargetsRear() && HasStatusEffect(Buffs.HindstungVenom) ||
                                !OnTargetsFlank() && HasStatusEffect(Buffs.FlankstungVenom))
                            ? Role.TrueNorth
                            : OriginalHook(SteelFangs);
                }

                if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                    return LevelChecked(ReavingFangs) && HasStatusEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingFangs)
                        : OriginalHook(SteelFangs);
            }

            //LowLevels
            if (LevelChecked(ReavingFangs) &&
                (HasStatusEffect(Buffs.HonedReavers) ||
                 !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingFangs);

            return actionID;
        }
    }

    internal class VPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SteelMaw)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                // Death Rattle / Legacy Weaves
                if (IsEnabled(Preset.VPR_AoE_SerpentsTail) &&
                    LevelChecked(SerpentsTail) &&
                    OriginalHook(SerpentsTail) is not SerpentsTail)
                    return OriginalHook(SerpentsTail);

                // Uncoiled combo
                if (IsEnabled(Preset.VPR_AoE_UncoiledFuryCombo))
                {
                    if (HasStatusEffect(Buffs.PoisedForTwinfang))
                        return OriginalHook(Twinfang);

                    if (HasStatusEffect(Buffs.PoisedForTwinblood))
                        return OriginalHook(Twinblood);
                }

                if (!HasStatusEffect(Buffs.Reawakened))
                {
                    //Vicepit weaves
                    if (IsEnabled(Preset.VPR_AoE_VicepitWeaves))
                    {
                        if (HasStatusEffect(Buffs.FellhuntersVenom) &&
                            (InActionRange(TwinfangThresh) || VPR_AoE_VicepitCombo_SubOption == 1))
                            return OriginalHook(Twinfang);

                        if (HasStatusEffect(Buffs.FellskinsVenom) &&
                            (InActionRange(TwinbloodThresh) || VPR_AoE_VicepitCombo_SubOption == 1))
                            return OriginalHook(Twinblood);
                    }

                    //Serpents Ire usage
                    if (IsEnabled(Preset.VPR_AoE_SerpentsIre) &&
                        !CappedOnCoils() && ActionReady(SerpentsIre))
                        return SerpentsIre;
                }

                // healing
                if (IsEnabled(Preset.VPR_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(VPR_AoE_SecondWind_Threshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(VPR_AoE_Bloodbath_Threshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.VPR_AoE_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep() &&
                    !TargetIsBoss() && TargetIsCasting())
                    return Role.LegSweep;
            }

            //Vicepit combo
            if (IsEnabled(Preset.VPR_AoE_VicepitCombo) &&
                !HasStatusEffect(Buffs.Reawakened))
            {
                if (SwiftskinsDenReady &&
                    (InActionRange(HuntersDen) || VPR_AoE_VicepitCombo_SubOption == 1))
                    return HuntersDen;

                if (VicepitReady &&
                    (InActionRange(SwiftskinsDen) || VPR_AoE_VicepitCombo_SubOption == 1))
                    return SwiftskinsDen;
            }

            //Reawakend Usage
            if (IsEnabled(Preset.VPR_AoE_Reawaken) &&
                GetTargetHPPercent() > VPR_AoE_Reawaken_Usage &&
                (HasStatusEffect(Buffs.ReadyToReawaken) || SerpentOffering >= 50) &&
                LevelChecked(Reawaken) &&
                HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                !HasStatusEffect(Buffs.Reawakened) &&
                (InActionRange(Reawaken) || VPR_AoE_Reawaken_SubOption == 1) &&
                !HasStatusEffect(Buffs.FellhuntersVenom) && !HasStatusEffect(Buffs.FellskinsVenom) &&
                !HasStatusEffect(Buffs.PoisedForTwinblood) && !HasStatusEffect(Buffs.PoisedForTwinfang))
                return Reawaken;

            //Overcap protection
            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                (HasCharges(Vicepit) && !HasStatusEffect(Buffs.FellskinsVenom) && !HasStatusEffect(Buffs.FellhuntersVenom) ||
                 IreCD <= GCD * 2) && !HasStatusEffect(Buffs.Reawakened) && CappedOnCoils())
                return UncoiledFury;

            //Vicepit Usage
            if (IsEnabled(Preset.VPR_AoE_Vicepit) &&
                ActionReady(Vicepit) && !HasStatusEffect(Buffs.Reawakened) &&
                (InActionRange(Vicepit) || VPR_AoE_Vicepit_SubOption == 1) &&
                (IreCD >= GCD * 5 || !LevelChecked(SerpentsIre)))
                return Vicepit;

            // Uncoiled Fury usage
            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                ActionReady(UncoiledFury) &&
                (RattlingCoilStacks > VPR_AoE_UncoiledFury_HoldCharges ||
                 GetTargetHPPercent() < VPR_AoE_UncoiledFury_Threshold &&
                 HasRattlingCoilStack()) &&
                HasStatusEffect(Buffs.Swiftscaled) && HasStatusEffect(Buffs.HuntersInstinct) &&
                !VicepitReady && !HuntersDenReady && !SwiftskinsDenReady &&
                !HasStatusEffect(Buffs.Reawakened) && !HasStatusEffect(Buffs.FellskinsVenom) &&
                !HasStatusEffect(Buffs.FellhuntersVenom) &&
                !WasLastWeaponskill(JaggedMaw) && !WasLastWeaponskill(BloodiedMaw) && !WasLastAbility(SerpentsIre))
                return UncoiledFury;

            //Reawaken combo
            if (IsEnabled(Preset.VPR_AoE_ReawakenCombo) &&
                ReawakenComboAoE(ref actionID))
                return actionID;

            //1-2-3 (4-5-6) Combo
            if (ComboTimer > 0 && !HasStatusEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingMaw or SteelMaw)
                {
                    if (LevelChecked(HuntersBite) &&
                        HasStatusEffect(Buffs.GrimhuntersVenom))
                        return OriginalHook(SteelMaw);

                    if (LevelChecked(SwiftskinsBite) &&
                        (HasStatusEffect(Buffs.GrimskinsVenom) ||
                         !HasStatusEffect(Buffs.Swiftscaled) && !HasStatusEffect(Buffs.HuntersInstinct)))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is HuntersBite or SwiftskinsBite)
                {
                    if (HasStatusEffect(Buffs.GrimhuntersVenom) && LevelChecked(JaggedMaw))
                        return OriginalHook(SteelMaw);

                    if (HasStatusEffect(Buffs.GrimskinsVenom) && LevelChecked(BloodiedMaw))
                        return OriginalHook(ReavingMaw);
                }

                if (ComboAction is BloodiedMaw or JaggedMaw)
                    return LevelChecked(ReavingMaw) && HasStatusEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingMaw)
                        : OriginalHook(SteelMaw);
            }

            //for lower lvls
            if (LevelChecked(ReavingMaw) &&
                (HasStatusEffect(Buffs.HonedReavers) ||
                 !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingMaw);

            return actionID;
        }
    }

    internal class VPR_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ReavingFangs)
                return actionID;

            if (ComboTimer > 0 && !HasStatusEffect(Buffs.Reawakened))
            {
                if (ComboAction is ReavingFangs or SteelFangs)
                {
                    if (LevelChecked(SwiftskinsSting) &&
                        (HasHindVenom() || NoSwiftscaled() || NoVenom()))
                        return OriginalHook(ReavingFangs);

                    if (LevelChecked(HuntersSting) && (HasFlankVenom() || NoHuntersInstinct()))
                        return OriginalHook(SteelFangs);
                }

                if (ComboAction is HuntersSting or SwiftskinsSting)
                {
                    if ((HasStatusEffect(Buffs.FlankstungVenom) || HasStatusEffect(Buffs.HindstungVenom)) &&
                        LevelChecked(FlanksbaneFang))
                        return OriginalHook(SteelFangs);

                    if ((HasStatusEffect(Buffs.FlanksbaneVenom) || HasStatusEffect(Buffs.HindsbaneVenom)) &&
                        LevelChecked(HindstingStrike))
                        return OriginalHook(ReavingFangs);
                }

                if (ComboAction is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang)
                    return LevelChecked(ReavingFangs) && HasStatusEffect(Buffs.HonedReavers)
                        ? OriginalHook(ReavingFangs)
                        : OriginalHook(SteelFangs);
            }

            //LowLevels
            if (LevelChecked(ReavingFangs) &&
                (HasStatusEffect(Buffs.HonedReavers) ||
                 !HasStatusEffect(Buffs.HonedReavers) && !HasStatusEffect(Buffs.HonedSteel)))
                return OriginalHook(ReavingFangs);
            return actionID;
        }
    }

    internal class VPR_Retarget_Slither : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_Retarget_Slither;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Slither)
                return actionID;

            return VPR_Slither_FieldMouseover
                ? Slither.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.ModelMouseOverTarget ?? SimpleTarget.HardTarget, true)
                : Slither.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.HardTarget, true);
        }
    }

    internal class VPR_VicewinderCoils : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_VicewinderCoils;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Vicewinder)
                return actionID;

            if (IsEnabled(Preset.VPR_VicewinderCoils_oGCDs))
            {
                if (HasStatusEffect(Buffs.HuntersVenom))
                    return OriginalHook(Twinfang);

                if (HasStatusEffect(Buffs.SwiftskinsVenom))
                    return OriginalHook(Twinblood);
            }

            // Swiftskin's Coil
            if (VicewinderReady && (!OnTargetsFlank() || !TargetNeedsPositionals()) || HuntersCoilReady)
                return SwiftskinsCoil;

            // Hunter's Coil
            if (VicewinderReady && (!OnTargetsRear() || !TargetNeedsPositionals()) || SwiftskinsCoilReady)
                return HuntersCoil;

            return actionID;
        }
    }

    internal class VPR_VicepitDens : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_VicepitDens;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Vicepit)
                return actionID;

            if (IsEnabled(Preset.VPR_VicepitDens_oGCDs))
            {
                if (HasStatusEffect(Buffs.FellhuntersVenom))
                    return OriginalHook(Twinfang);

                if (HasStatusEffect(Buffs.FellskinsVenom))
                    return OriginalHook(Twinblood);
            }

            if (SwiftskinsDenReady)
                return HuntersDen;

            if (VicepitReady)
                return SwiftskinsDen;

            return actionID;
        }
    }

    internal class VPR_UncoiledTwins : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_UncoiledTwins;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not UncoiledFury)
                return actionID;

            if (HasStatusEffect(Buffs.PoisedForTwinfang))
                return OriginalHook(Twinfang);

            if (HasStatusEffect(Buffs.PoisedForTwinblood))
                return OriginalHook(Twinblood);

            return actionID;
        }
    }

    internal class VPR_ReawakenLegacy : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ReawakenLegacy;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Reawaken or ReavingFangs))
                return actionID;

            switch (actionID)
            {
                case Reawaken when VPR_ReawakenLegacyButton == 0 && HasStatusEffect(Buffs.Reawakened):
                case ReavingFangs when VPR_ReawakenLegacyButton == 1 && HasStatusEffect(Buffs.Reawakened):
                    {
                        // Legacy Weaves
                        if (IsEnabled(Preset.VPR_ReawakenLegacyWeaves) &&
                            TraitLevelChecked(Traits.SerpentsLegacy) && HasStatusEffect(Buffs.Reawakened)
                            && OriginalHook(SerpentsTail) is not SerpentsTail)
                            return OriginalHook(SerpentsTail);

                        if (ReawakenComboST(ref actionID))
                            return actionID;
                        break;
                    }
            }

            return actionID;
        }
    }

    internal class VPR_TwinTails : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_TwinTails;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SerpentsTail)
                return actionID;

            if (LevelChecked(SerpentsTail) && OriginalHook(SerpentsTail) is not SerpentsTail)
                return OriginalHook(SerpentsTail);

            if (HasStatusEffect(Buffs.PoisedForTwinfang) ||
                HasStatusEffect(Buffs.HuntersVenom) ||
                HasStatusEffect(Buffs.FellhuntersVenom))
                return OriginalHook(Twinfang);

            if (HasStatusEffect(Buffs.PoisedForTwinblood) ||
                HasStatusEffect(Buffs.SwiftskinsVenom) ||
                HasStatusEffect(Buffs.FellskinsVenom))
                return OriginalHook(Twinblood);

            return actionID;
        }
    }

    internal class VPR_Legacies : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_Legacies;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SteelFangs or ReavingFangs or HuntersCoil or SwiftskinsCoil) || !HasStatusEffect(Buffs.Reawakened))
                return actionID;

            //Reawaken combo
            return actionID switch
            {
                SteelFangs when JustUsed(OriginalHook(SteelFangs)) && AnguineTribute is 4 => OriginalHook(SerpentsTail),
                ReavingFangs when JustUsed(OriginalHook(ReavingFangs)) && AnguineTribute is 3 => OriginalHook(SerpentsTail),
                HuntersCoil when JustUsed(OriginalHook(HuntersCoil)) && AnguineTribute is 2 => OriginalHook(SerpentsTail),
                SwiftskinsCoil when JustUsed(OriginalHook(SwiftskinsCoil)) && AnguineTribute is 1 => OriginalHook(SerpentsTail),
                var _ => actionID
            };
        }
    }

    internal class VPR_SerpentsTail : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_SerpentsTail;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SteelFangs or ReavingFangs or SteelMaw or ReavingMaw))
                return actionID;

            return actionID switch
            {
                SteelFangs or ReavingFangs when OriginalHook(SerpentsTail) is DeathRattle &&
                                                (JustUsed(FlankstingStrike) ||
                                                 JustUsed(FlanksbaneFang) ||
                                                 JustUsed(HindstingStrike) ||
                                                 JustUsed(HindsbaneFang)) => OriginalHook(SerpentsTail),
                SteelMaw or ReavingMaw when OriginalHook(SerpentsTail) is LastLash &&
                                            (JustUsed(JaggedMaw) ||
                                             JustUsed(BloodiedMaw)) => OriginalHook(SerpentsTail),
                var _ => actionID
            };
        }
    }
}
