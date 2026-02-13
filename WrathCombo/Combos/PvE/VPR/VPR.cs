using Dalamud.Game.ClientState.JobGauge.Enums;
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
            if (CanWeave())
            {
                // Death Rattle / Legacy Weaves
                if ((DeathRattleWeave && InActionRange(DeathRattle) ||
                     Legacyweaves && InActionRange(FirstLegacy)) &&
                    LevelChecked(SerpentsTail))
                    return OriginalHook(SerpentsTail);

                // Fury Twin Weaves
                if (HasStatusEffect(Buffs.PoisedForTwinfang))
                    return OriginalHook(Twinfang);

                if (HasStatusEffect(Buffs.PoisedForTwinblood))
                    return OriginalHook(Twinblood);


                //Vice Twin Weaves
                if (!HasStatusEffect(Buffs.Reawakened) && InMeleeRange())
                {
                    if (HasStatusEffect(Buffs.HuntersVenom))
                        return OriginalHook(Twinfang);

                    if (HasStatusEffect(Buffs.SwiftskinsVenom))
                        return OriginalHook(Twinblood);
                }

                //Serpents Ire
                if (InCombat() &&
                    !MaxCoils && ActionReady(SerpentsIre))
                    return SerpentsIre;

                if (Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                // healing
                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (Role.CanBloodBath(30))
                    return Role.Bloodbath;

                if (RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            //Vicewinder Combo
            if (CanVicewinderCombo(ref actionID))
                return actionID;

            //Reawakend Usage
            if (CanReawaken())
                return Reawaken;

            //Overcap protection
            if (MaxCoils &&
                (HasCharges(Vicewinder) && NoSTComboWeaves &&
                 !HasStatusEffect(Buffs.Reawakened) || //spend if Vicewinder is up, after Reawaken
                 IreCD <= GCD * 3)) //spend in case under Reawaken right as Ire comes up
                return UncoiledFury;

            //Vicewinder
            if (CanUseVicewinder)
                return Role.CanTrueNorth()
                    ? Role.TrueNorth
                    : Vicewinder;

            // Uncoiled Fury
            if (CanUseUncoiledFury())
                return UncoiledFury;

            //Ranged
            if (ActionReady(WrithingSnap) &&
                !InMeleeRange() && HasBattleTarget() &&
                !HasRattlingCoilStacks)
                return WrithingSnap;

            //Reawaken combo / 1-2-3 (4-5-6) Combo
            return HasStatusEffect(Buffs.Reawakened)
                ? ReawakenCombo(actionID)
                : DoBasicCombo(actionID, true);
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
                if ((LastLashWeave && InActionRange(LastLash) ||
                     Legacyweaves && InActionRange(FirstLegacy)) &&
                    LevelChecked(SerpentsTail))
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
                    if (!MaxCoils && ActionReady(SerpentsIre) &&
                        GetTargetHPPercent() > 25)
                        return SerpentsIre;
                }

                // healing
                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (Role.CanBloodBath(30))
                    return Role.Bloodbath;

                if (RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            //Vicepit combo
            if (!HasStatusEffect(Buffs.Reawakened))
            {
                if (UsedSwiftskinsDen &&
                    InActionRange(HuntersDen))
                    return HuntersDen;

                if (UsedVicepit &&
                    InActionRange(SwiftskinsDen))
                    return SwiftskinsDen;
            }

            //Reawakend Usage
            if (CanReawaken(true) &&
                InActionRange(Reawaken))
                return Reawaken;

            //Overcap protection
            if ((HasCharges(Vicepit) && NoAoEComboWeaves || IreCD <= GCD * 2) &&
                !HasStatusEffect(Buffs.Reawakened) && MaxCoils)
                return UncoiledFury;

            //Vicepit Usage
            if (ActionReady(Vicepit) && !HasStatusEffect(Buffs.Reawakened) &&
                InActionRange(Vicepit) && !JustUsed(Vicepit) &&
                (IreCD >= GCD * 4 || !LevelChecked(SerpentsIre)))
                return Vicepit;

            // Uncoiled Fury usage
            if (CanUseUncoiledFury(true))
                return UncoiledFury;

            //Reawaken combo / 1-2-3 combo
            return HasStatusEffect(Buffs.Reawakened)
                ? ReawakenCombo(actionID)
                : DoBasicCombo(actionID, isAoE: true);
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
                // Death Rattle / Legacy Weaves
                if ((IsEnabled(Preset.VPR_ST_SerpentsTail) && DeathRattleWeave && InActionRange(DeathRattle) ||
                     IsEnabled(Preset.VPR_ST_LegacyWeaves) && Legacyweaves && InActionRange(FirstLegacy)) &&
                    LevelChecked(SerpentsTail))
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

                //Serpents Ire
                if (IsEnabled(Preset.VPR_ST_SerpentsIre) && InCombat() &&
                    !MaxCoils && ActionReady(SerpentsIre) &&
                    GetTargetHPPercent() > HPThresholdSerpentsIre)
                    return SerpentsIre;

                if (IsEnabled(Preset.VPR_ST_Feint) &&
                    Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                // healing
                if (IsEnabled(Preset.VPR_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(VPR_ST_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(VPR_ST_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.VPR_ST_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            //Vicewinder Combo
            if (IsEnabled(Preset.VPR_ST_VicewinderCombo) &&
                CanVicewinderCombo(ref actionID))
                return actionID;

            //Reawakend Usage
            if (IsEnabled(Preset.VPR_ST_Reawaken) &&
                CanReawaken())
                return Reawaken;

            //Overcap protection
            if (IsEnabled(Preset.VPR_ST_UncoiledFury) && MaxCoils &&
                (HasCharges(Vicewinder) && NoSTComboWeaves &&
                 !HasStatusEffect(Buffs.Reawakened) || //spend if Vicewinder is up, after Reawaken
                 IreCD <= GCD * 3)) //spend in case under Reawaken right as Ire comes up
                return UncoiledFury;

            //Vicewinder
            if (IsEnabled(Preset.VPR_ST_Vicewinder) &&
                CanUseVicewinder)
                return VPR_TrueNorthVicewinder &&
                       GetRemainingCharges(Role.TrueNorth) > TnCharges &&
                       Role.CanTrueNorth()
                    ? Role.TrueNorth
                    : Vicewinder;

            // Uncoiled Fury
            if (IsEnabled(Preset.VPR_ST_UncoiledFury) &&
                CanUseUncoiledFury())
                return UncoiledFury;

            //Ranged
            if (!InMeleeRange() && HasBattleTarget() &&
                IsEnabled(Preset.VPR_ST_RangedUptime) &&
                ActionReady(WrithingSnap) &&
                (IsEnabled(Preset.VPR_ST_UncoiledFury) && !HasRattlingCoilStacks ||
                 IsNotEnabled(Preset.VPR_ST_UncoiledFury)))
                return WrithingSnap;

            //Reawaken combo / 1-2-3 (4-5-6) Combo
            return IsEnabled(Preset.VPR_ST_GenerationCombo) &&
                   HasStatusEffect(Buffs.Reawakened)
                ? ReawakenCombo(actionID)
                : DoBasicCombo(actionID, IsEnabled(Preset.VPR_TrueNorthDynamic));
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
                if ((IsEnabled(Preset.VPR_AoE_SerpentsTail) && LastLashWeave && InActionRange(LastLash) ||
                     IsEnabled(Preset.VPR_AoE_ReawakenCombo) && Legacyweaves && InActionRange(FirstLegacy)) &&
                    LevelChecked(SerpentsTail))
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
                            (InActionRange(TwinfangThresh) || VPR_AoE_VicepitComboRangeCheck == 1))
                            return OriginalHook(Twinfang);

                        if (HasStatusEffect(Buffs.FellskinsVenom) &&
                            (InActionRange(TwinbloodThresh) || VPR_AoE_VicepitComboRangeCheck == 1))
                            return OriginalHook(Twinblood);
                    }

                    //Serpents Ire usage
                    if (IsEnabled(Preset.VPR_AoE_SerpentsIre) &&
                        !MaxCoils && ActionReady(SerpentsIre) &&
                        GetTargetHPPercent() > VPR_AoE_SerpentsIreHPThreshold)
                        return SerpentsIre;
                }

                // healing
                if (IsEnabled(Preset.VPR_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(VPR_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(VPR_AoE_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.VPR_AoE_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            //Vicepit combo
            if (IsEnabled(Preset.VPR_AoE_VicepitCombo) &&
                !HasStatusEffect(Buffs.Reawakened))
            {
                if (UsedSwiftskinsDen &&
                    (InActionRange(HuntersDen) || VPR_AoE_VicepitComboRangeCheck == 1))
                    return HuntersDen;

                if (UsedVicepit &&
                    (InActionRange(SwiftskinsDen) || VPR_AoE_VicepitComboRangeCheck == 1))
                    return SwiftskinsDen;
            }

            //Reawakend Usage
            if (IsEnabled(Preset.VPR_AoE_Reawaken) &&
                CanReawaken(true) &&
                (InActionRange(Reawaken) || VPR_AoE_ReawakenRangecheck == 1))
                return Reawaken;

            //Overcap protection
            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                (HasCharges(Vicepit) && NoAoEComboWeaves || IreCD <= GCD * 2) &&
                !HasStatusEffect(Buffs.Reawakened) && MaxCoils)
                return UncoiledFury;

            //Vicepit Usage
            if (IsEnabled(Preset.VPR_AoE_Vicepit) &&
                ActionReady(Vicepit) && !HasStatusEffect(Buffs.Reawakened) &&
                !JustUsed(Vicepit) &&
                (InActionRange(Vicepit) || VPR_AoE_VicepitRangeCheck == 1) &&
                (IreCD >= GCD * 4 || !LevelChecked(SerpentsIre)))
                return Vicepit;

            // Uncoiled Fury usage
            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                CanUseUncoiledFury(true))
                return UncoiledFury;

            //Reawaken combo / 1-2-3 combo
            return IsEnabled(Preset.VPR_AoE_ReawakenCombo) &&
                   HasStatusEffect(Buffs.Reawakened)
                ? ReawakenCombo(actionID)
                : DoBasicCombo(actionID, false, true);
        }
    }

    internal class VPR_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ReavingFangs)
                return actionID;

            if (DeathRattleWeave &&
                LevelChecked(SerpentsTail) && InActionRange(DeathRattle))
                return OriginalHook(SerpentsTail);

            return DoBasicCombo(actionID);
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
                ? Slither.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.ModelMouseOverTarget ?? SimpleTarget.HardTarget)
                : Slither.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.HardTarget);
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
            if (UsedVicewinder && (!OnTargetsFlank() || !TargetNeedsPositionals()) || UsedHuntersCoil)
                return SwiftskinsCoil;

            // Hunter's Coil
            if (UsedVicewinder && (!OnTargetsRear() || !TargetNeedsPositionals()) || UsedSwiftskinsCoil)
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

            if (UsedSwiftskinsDen)
                return HuntersDen;

            if (UsedVicepit)
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
                    return IsEnabled(Preset.VPR_ReawakenLegacyWeaves) &&
                           TraitLevelChecked(Traits.SerpentsLegacy) &&
                           HasStatusEffect(Buffs.Reawakened) && Legacyweaves
                        ? OriginalHook(SerpentsTail)
                        : ReawakenCombo(actionID);
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
                SteelFangs when Gauge.SerpentCombo is SerpentCombo.FirstLegacy => OriginalHook(SerpentsTail),
                ReavingFangs when Gauge.SerpentCombo is SerpentCombo.SecondLegacy => OriginalHook(SerpentsTail),
                HuntersCoil when Gauge.SerpentCombo is SerpentCombo.ThirdLegacy => OriginalHook(SerpentsTail),
                SwiftskinsCoil when Gauge.SerpentCombo is SerpentCombo.FourthLegacy => OriginalHook(SerpentsTail),
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
                SteelFangs or ReavingFangs when DeathRattleWeave => OriginalHook(SerpentsTail),
                SteelMaw or ReavingMaw when LastLashWeave => OriginalHook(SerpentsTail),
                var _ => actionID
            };
        }
    }

    internal class VPR_VicewinderProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_VicewinderProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Vicewinder or Vicepit))
                return actionID;

            return (UsedVicewinder || UsedHuntersCoil || UsedSwiftskinsCoil ||
                    UsedVicepit || UsedHuntersDen || UsedSwiftskinsDen) &&
                   LevelChecked(Vicewinder)
                ? All.SavageBlade
                : actionID;
        }
    }
}
