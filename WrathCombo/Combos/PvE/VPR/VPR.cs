using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.VPR.Config;
namespace WrathCombo.Combos.PvE;

internal partial class VPR : Melee
{
    internal class VPR_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, SteelFangs)) return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(false, true, true))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(out uint twin) ||
                    UseViceTwinWeaves(out twin, false, true))
                    return twin;

                if (CanSerpentsIre())
                    return SerpentsIre;

                if (Role.CanFeint() && GroupDamageIncoming())
                    return Role.Feint;

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (Role.CanBloodBath(30))
                    return Role.Bloodbath;

                if (RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (CanVicewinderCombo(ref actionID))
                return actionID;

            if (UncoiledFuryOvercapProtection(false))
                return UncoiledFury;

            if (CanReawaken())
                return Reawaken;

            if (CanUseVicewinder)
                return UseVicewinder();

            if (CanUseUncoiledFury())
                return UncoiledFury;

            if (ActionReady(WrithingSnap) &&
                !InMeleeRange() && HasBattleTarget() &&
                !HasRattlingCoilStacks)
                return WrithingSnap;

            return UseCombo(actionID, false, true, true);
        }
    }

    internal class VPR_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, SteelMaw)) return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(true, true, true))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(out uint twin) ||
                    UseViceTwinWeaves(out twin, true, true))
                    return twin;

                if (CanSerpentsIre(25))
                    return SerpentsIre;

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (Role.CanBloodBath(30))
                    return Role.Bloodbath;

                if (RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (UseVicepitCombo(ref actionID))
                return actionID;

            if (UncoiledFuryOvercapProtection(true))
                return UncoiledFury;

            if (CanReawaken(true) && InActionRange(Reawaken))
                return Reawaken;

            if (CanVicepit())
                return Vicepit;

            return CanUseUncoiledFury(true)
                ? UncoiledFury
                : UseCombo(actionID, true, true);
        }
    }

    internal class VPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, SteelFangs)) return actionID;

            if (IsEnabled(Preset.VPR_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(false,
                    IsEnabled(Preset.VPR_ST_SerpentsTail),
                    IsEnabled(Preset.VPR_ST_LegacyWeaves)))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(out uint twin, IsEnabled(Preset.VPR_ST_UncoiledFuryCombo)) ||
                    UseViceTwinWeaves(out twin, false, IsEnabled(Preset.VPR_ST_VicewinderWeaves)))
                    return twin;

                if (IsEnabled(Preset.VPR_ST_SerpentsIre) && CanSerpentsIre(SerpentsIreHPThreshold))
                    return SerpentsIre;

                if (IsEnabled(Preset.VPR_ST_Feint) &&
                    Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

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

            if (IsEnabled(Preset.VPR_ST_VicewinderCombo) &&
                CanVicewinderCombo(ref actionID, VPR_VicewinderBuffPrio))
                return actionID;

            if (IsEnabled(Preset.VPR_ST_UncoiledFury) &&
                UncoiledFuryOvercapProtection(false))
                return UncoiledFury;

            if (IsEnabled(Preset.VPR_ST_Reawaken) &&
                CanReawaken(hpThresholdUsage: ReawakenHPThreshold(), hpThresholdDontSave: VPR_ST_ReAwakenAlwaysUse))
                return Reawaken;

            if (IsEnabled(Preset.VPR_ST_Vicewinder) &&
                CanUseVicewinder)
                return UseVicewinder(
                    false,
                    IsEnabled(Preset.VPR_TrueNorthDynamic),
                    VPR_ManualTN);

            if (IsEnabled(Preset.VPR_ST_UncoiledFury) &&
                CanUseUncoiledFury(stHoldCharges: VPR_ST_UncoiledFuryHoldCharges, stHpThreshold: VPR_ST_UncoiledFuryAlwaysUse))
                return UncoiledFury;

            if (!InMeleeRange() && HasBattleTarget() &&
                IsEnabled(Preset.VPR_ST_RangedUptime) &&
                ActionReady(WrithingSnap) &&
                (IsEnabled(Preset.VPR_ST_UncoiledFury) && !HasRattlingCoilStacks ||
                 IsNotEnabled(Preset.VPR_ST_UncoiledFury)))
                return WrithingSnap;

            return UseCombo(actionID, false,
                IsEnabled(Preset.VPR_ST_GenerationCombo),
                IsEnabled(Preset.VPR_TrueNorthDynamic),
                VPR_ManualTN,
                VPR_ST_TrueNorthDynamicHoldCharge);
        }
    }

    internal class VPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, SteelMaw)) return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(true,
                    IsEnabled(Preset.VPR_AoE_SerpentsTail),
                    IsEnabled(Preset.VPR_AoE_ReawakenCombo)))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(out uint twin, IsEnabled(Preset.VPR_AoE_UncoiledFuryCombo)) ||
                    UseViceTwinWeaves(out twin, true, IsEnabled(Preset.VPR_AoE_VicepitWeaves), ignoreRange: VPR_AoE_VicepitComboRangeCheck == 1))
                    return twin;

                if (IsEnabled(Preset.VPR_AoE_SerpentsIre) &&
                    CanSerpentsIre(VPR_AoE_SerpentsIreHPThreshold))
                    return SerpentsIre;

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

            if (IsEnabled(Preset.VPR_AoE_VicepitCombo) &&
                UseVicepitCombo(ref actionID, VPR_AoE_VicepitComboRangeCheck == 1))
                return actionID;

            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                UncoiledFuryOvercapProtection(true))
                return UncoiledFury;

            if (IsEnabled(Preset.VPR_AoE_Reawaken) &&
                CanReawaken(true, hpThresholdUsageAoE: VPR_AoE_ReawakenHPThreshold) &&
                (InActionRange(Reawaken) || VPR_AoE_ReawakenRangecheck == 1))
                return Reawaken;

            if (IsEnabled(Preset.VPR_AoE_Vicepit) &&
                CanVicepit(VPR_AoE_VicepitRangeCheck == 1))
                return Vicepit;

            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                CanUseUncoiledFury(true, aoeHoldCharges: VPR_AoE_UncoiledFuryHoldCharges, aoeHpThreshold: VPR_AoE_UncoiledFuryAlwaysUse))
                return UncoiledFury;

            return UseCombo(actionID, true, IsEnabled(Preset.VPR_AoE_ReawakenCombo));
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

            return DoBasicCombo();
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

            if (IsEnabled(Preset.VPR_VicewinderCoils_oGCDs) &&
                UseViceTwinWeaves(out uint twin, false, true, false))
                return twin;

            if (UsedVicewinder && (!OnTargetsFlank() || !TargetNeedsPositionals()) || UsedHuntersCoil)
                return SwiftskinsCoil;

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

            if (IsEnabled(Preset.VPR_VicepitDens_oGCDs) &&
                UseViceTwinWeaves(out uint twin, true, true, false, true))
                return twin;

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

            return UsePoisedTwinWeaves(out uint twin)
                ? twin
                : actionID;
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
                    return IsEnabled(Preset.VPR_ReawakenLegacyWeaves) &&
                           TraitLevelChecked(Traits.SerpentsLegacy) &&
                           HasStatusEffect(Buffs.Reawakened) && LegacyWeaves
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
