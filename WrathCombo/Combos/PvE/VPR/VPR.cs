using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.VPR.Config;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class VPR : Melee
{
    internal class VPR_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, SteelFangs))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(false, true, true))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(ref actionID) ||
                    UseViceTwinWeaves(ref actionID, false, true))
                    return actionID;

                if (UseSerpentsIre())
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

            if (UseVicewinderCombo(ref actionID, preferRangedWhenOor: true))
                return actionID;

            if (UseReawaken())
                return Reawaken;

            if (OvercapUncoiledFuryProtection(false))
                return UncoiledFury;

            if (UseVicewinder())
                return DoVicewinder();

            if (UseUncoiledFury())
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
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, SteelMaw))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(true, true, true))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(ref actionID) ||
                    UseViceTwinWeaves(ref actionID, true, true))
                    return actionID;

                if (UseSerpentsIre(25))
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

            if (UseReawaken(true) && InActionRange(Reawaken))
                return Reawaken;

            if (OvercapUncoiledFuryProtection(true))
                return UncoiledFury;

            if (UseVicepit())
                return Vicepit;

            return UseUncoiledFury(true)
                ? UncoiledFury
                : UseCombo(actionID, true, true);
        }
    }

    internal class VPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, SteelFangs) ||
                IsEnabled(Preset.VPR_ST_Opener) && Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(false,
                    IsEnabled(Preset.VPR_ST_SerpentsTail),
                    IsEnabled(Preset.VPR_ST_LegacyWeaves)))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(ref actionID, IsEnabled(Preset.VPR_ST_UncoiledFuryCombo)) ||
                    UseViceTwinWeaves(ref actionID, false, IsEnabled(Preset.VPR_ST_VicewinderWeaves)))
                    return actionID;

                if (IsEnabled(Preset.VPR_ST_SerpentsIre) && UseSerpentsIre(SerpentsIreHPThreshold))
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
                UseVicewinderCombo(ref actionID, VPR_VicewinderBuffPrio,
                    IsEnabled(Preset.VPR_ST_UncoiledFury) ||
                    IsEnabled(Preset.VPR_ST_RangedUptime)))
                return actionID;

            if (IsEnabled(Preset.VPR_ST_Reawaken) &&
                UseReawaken(hpThresholdUsage: ReawakenHPThreshold(), hpThresholdDontSave: VPR_ST_ReAwakenAlwaysUse))
                return Reawaken;

            if (IsEnabled(Preset.VPR_ST_UncoiledFury) &&
                OvercapUncoiledFuryProtection(false))
                return UncoiledFury;

            if (IsEnabled(Preset.VPR_ST_Vicewinder) &&
                UseVicewinder())
                return DoVicewinder(
                    false,
                    IsEnabled(Preset.VPR_TrueNorthDynamic),
                    VPR_ManualTN);

            if (IsEnabled(Preset.VPR_ST_UncoiledFury) &&
                UseUncoiledFury(stHoldCharges: VPR_ST_UncoiledFuryHoldCharges, stHpThreshold: VPR_ST_UncoiledFuryAlwaysUse))
                return UncoiledFury;

            if (!InMeleeRange() && HasBattleTarget() &&
                IsEnabled(Preset.VPR_ST_RangedUptime) &&
                ActionReady(WrithingSnap) &&
                (IsEnabled(Preset.VPR_ST_UncoiledFury) && !HasRattlingCoilStacks ||
                 !IsEnabled(Preset.VPR_ST_UncoiledFury)))
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
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, SteelMaw))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseSerpentsTailWeave(true,
                    IsEnabled(Preset.VPR_AoE_SerpentsTail),
                    IsEnabled(Preset.VPR_AoE_ReawakenCombo)))
                    return OriginalHook(SerpentsTail);

                if (UsePoisedTwinWeaves(ref actionID, IsEnabled(Preset.VPR_AoE_UncoiledFuryCombo)) ||
                    UseViceTwinWeaves(ref actionID, true, IsEnabled(Preset.VPR_AoE_VicepitWeaves), ignoreRange: VPR_AoE_VicepitComboRangeCheck == 1))
                    return actionID;

                if (IsEnabled(Preset.VPR_AoE_SerpentsIre) &&
                    UseSerpentsIre(VPR_AoE_SerpentsIreHPThreshold))
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

            if (IsEnabled(Preset.VPR_AoE_Reawaken) &&
                UseReawaken(true, hpThresholdUsageAoE: VPR_AoE_ReawakenHPThreshold) &&
                (InActionRange(Reawaken) || VPR_AoE_ReawakenRangecheck == 1))
                return Reawaken;

            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                OvercapUncoiledFuryProtection(true))
                return UncoiledFury;

            if (IsEnabled(Preset.VPR_AoE_Vicepit) &&
                UseVicepit(VPR_AoE_VicepitRangeCheck == 1))
                return Vicepit;

            if (IsEnabled(Preset.VPR_AoE_UncoiledFury) &&
                UseUncoiledFury(true, aoeHoldCharges: VPR_AoE_UncoiledFuryHoldCharges, aoeHpThreshold: VPR_AoE_UncoiledFuryAlwaysUse))
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

            if (IsDeathRattleWeave &&
                ActionLearned(SerpentsTail) && InActionRange(DeathRattle))
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
                UseViceTwinWeaves(ref actionID, false, true, false))
                return actionID;

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
                UseViceTwinWeaves(ref actionID, true, true, false, true))
                return actionID;

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

            UsePoisedTwinWeaves(ref actionID);
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
                case Reawaken when VPR_ReawakenLegacyButton == 0 && LocalPlayer.HasStatus(Buffs.Reawakened):
                case ReavingFangs when VPR_ReawakenLegacyButton == 1 && LocalPlayer.HasStatus(Buffs.Reawakened):
                    {
                        return IsEnabled(Preset.VPR_ReawakenLegacyWeaves) &&
                               TraitLevelChecked(Traits.SerpentsLegacy) &&
                               LocalPlayer.HasStatus(Buffs.Reawakened) && IsLegacyWeaveReady
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

            if (ActionLearned(SerpentsTail) && OriginalHook(SerpentsTail) is not SerpentsTail)
                return OriginalHook(SerpentsTail);

            if (LocalPlayer.HasStatus(Buffs.PoisedForTwinfang) ||
                LocalPlayer.HasStatus(Buffs.HuntersVenom) ||
                LocalPlayer.HasStatus(Buffs.FellhuntersVenom))
                return OriginalHook(Twinfang);

            if (LocalPlayer.HasStatus(Buffs.PoisedForTwinblood) ||
                LocalPlayer.HasStatus(Buffs.SwiftskinsVenom) ||
                LocalPlayer.HasStatus(Buffs.FellskinsVenom))
                return OriginalHook(Twinblood);

            return actionID;
        }
    }

    internal class VPR_Legacies : CustomCombo
    {
        protected internal override Preset Preset => Preset.VPR_Legacies;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SteelFangs or ReavingFangs or HuntersCoil or SwiftskinsCoil) || !LocalPlayer.HasStatus(Buffs.Reawakened))
                return actionID;

            return actionID switch
            {
                SteelFangs when Gauge.SerpentCombo is SerpentCombo.FirstLegacy => OriginalHook(SerpentsTail),
                ReavingFangs when Gauge.SerpentCombo is SerpentCombo.SecondLegacy => OriginalHook(SerpentsTail),
                HuntersCoil when Gauge.SerpentCombo is SerpentCombo.ThirdLegacy => OriginalHook(SerpentsTail),
                SwiftskinsCoil when Gauge.SerpentCombo is SerpentCombo.FourthLegacy => OriginalHook(SerpentsTail),
                _ => actionID
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
                SteelFangs or ReavingFangs when IsDeathRattleWeave => OriginalHook(SerpentsTail),
                SteelMaw or ReavingMaw when IsLastLashWeave => OriginalHook(SerpentsTail),
                _ => actionID
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
                   ActionLearned(Vicewinder)
                ? All.Cease
                : actionID;
        }
    }
}
