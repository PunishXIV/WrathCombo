#region Dependencies

using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.GNB.Config;

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class GNB : Tank
{
    #region Simple Mode - Single Target
    internal class GNB_ST_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != KeenEdge)
                return actionID;

            #region Non-Rotation

            if (Role.CanInterject() &&
                IsEnabled(Preset.GNB_ST_Interrupt))
                return Role.Interject;

            if (Role.CanLowBlow() &&
                IsEnabled(Preset.GNB_ST_Stun))
                return Role.LowBlow;

            if (BozjaActions() != 0)
                return BozjaActions();

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            var mitigationsOn =
                GNB_ST_MitsOptions != 1 ||
                (P.UIHelper.PresetControlled(Preset)?.enabled == true);
            if (mitigationsOn)
            {
                if (!MitUsed &&
                    InCombat())
                {
                    if (HPP < 30 &&
                        ActionReady(Superbolide))
                        return Superbolide;

                    if (IsPlayerTargeted())
                    {
                        if (HPP < 60 && 
                            ActionReady(OriginalHook(Nebula)))
                            return OriginalHook(Nebula);

                        if (HPP < 80 &&
                            ActionReady(Role.Rampart))
                            return Role.Rampart;

                        if (Role.CanReprisal(90))
                            return Role.Reprisal;
                    }
                    if (HPP < 70 &&
                        ActionReady(Camouflage))
                        return Camouflage;

                    if (HPP < 90 && 
                        ActionReady(OriginalHook(HeartOfStone)))
                        return OriginalHook(HeartOfStone);

                    if (HPP < 85 &&
                        ActionReady(Aurora) && 
                        !(HasStatusEffect(Buffs.Aurora) || HasStatusEffect(Buffs.Aurora, CurrentTarget, true)))
                        return Aurora;
                }
            }
            #endregion

            #endregion

            #region Rotation

            //Lightning Shot
            if (ShouldUseLightningShot(Preset.GNB_ST_Simple, 0))
                return LightningShot;

            //MAX PRIORITY - just clip it, it's better than just losing it altogether
            //Continuation procs (Hypervelocity, Jugular Rip, Abdomen Tear, Eye Gouge)
            if ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) &&
                RemainingGCD < 0.6f &&
                IsEnabled(Preset.GNB_ST_Continuation))
                return OriginalHook(Continuation);

            //No Mercy
            if (ShouldUseNoMercy(Preset.GNB_ST_Simple, 0, 0))
                return NoMercy;

            //Bloodfest
            if (ShouldUseBloodfest(Preset.GNB_ST_Simple))
                return Bloodfest;

            //HIGH PRIORITY - within late weave window, send now
            //Continuation procs (Hypervelocity, Jugular Rip, Abdomen Tear, Eye Gouge)
            if ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) &&
                CanDelayedWeave())
                return OriginalHook(Continuation);

            //Hypervelocity
            //if No Mercy is imminent, then we want to aim for buffing HV right after using Burst Strike (BS^NM^HV>GF>etc.)
            if (JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasStatusEffect(Buffs.ReadyToBlast) &&
                NMcd is > 1.3f)
                return Hypervelocity;

            //Bow Shock & Zone
            //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
            //without SKS, we don't really care since both usually remain static
            if (Slow ? ShouldUseBowShock(Preset.GNB_ST_Simple) : ShouldUseZone(Preset.GNB_ST_Simple))
                return Slow ? BowShock : OriginalHook(DangerZone);
            if (Slow ? ShouldUseZone(Preset.GNB_ST_Simple) : ShouldUseBowShock(Preset.GNB_ST_Simple))
                return Slow ? OriginalHook(DangerZone) : BowShock;

            //NORMAL PRIORITY - within weave weave window
            //Gnashing Fang procs (Jugular Rip, Abdomen Tear, Eye Gouge)
            if (CanContinue &&
                CanWeave())
                return OriginalHook(Continuation);

            //Gnashing Fang - burst
            if (ShouldUseGnashingFangBurst(Preset.GNB_ST_Simple))
                return GnashingFang;

            //Double Down
            if (ShouldUseDoubleDown(Preset.GNB_ST_Simple))
                return DoubleDown;

            //Sonic Break
            if (ShouldUseSonicBreak(Preset.GNB_ST_Simple))
                return SonicBreak;

            //Reign of Beasts
            if (ShouldUseReignOfBeasts(Preset.GNB_ST_Simple))
                return OriginalHook(ReignOfBeasts);

            //Gnashing Fang 2 - filler boogaloo
            if (ShouldUseGnashingFangFiller(Preset.GNB_ST_Simple))
                return GnashingFang;

            //Savage Claw & Wicked Talon
            if (GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //Noble Blood & Lion Heart
            if (GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            //Burst Strike
            if (ShouldUseBurstStrike(Preset.GNB_ST_Simple))
                return BurstStrike;

            //1-2-3
            return STCombo(0);

            #endregion
        }
    }

    #endregion

    #region Advanced Mode - Single Target
    internal class GNB_ST_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_ST_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != KeenEdge)
                return actionID;

            #region Non-Rotation

            if (Role.CanInterject() && 
                IsEnabled(Preset.GNB_ST_Interrupt))
                return Role.Interject;

            if (Role.CanLowBlow() &&
                IsEnabled(Preset.GNB_ST_Stun))
                return Role.LowBlow;

            if (BozjaActions() != 0)
                return BozjaActions();

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            if (IsEnabled(Preset.GNB_ST_Mitigation) && InCombat() && !MitUsed)
            {
                if (IsEnabled(Preset.GNB_ST_Superbolide) && 
                    ActionReady(Superbolide) && 
                    HPP < GNB_ST_Superbolide_Health &&
                    (GNB_ST_Superbolide_SubOption == 0 || TargetIsBoss() && GNB_ST_Superbolide_SubOption == 1))
                    return Superbolide;

                if (IsPlayerTargeted())
                {
                    if (IsEnabled(Preset.GNB_ST_Nebula) && 
                        ActionReady(OriginalHook(Nebula)) && 
                        HPP < GNB_ST_Nebula_Health &&
                        (GNB_ST_Nebula_SubOption == 0 || TargetIsBoss() && GNB_ST_Nebula_SubOption == 1))
                        return OriginalHook(Nebula);

                    if (IsEnabled(Preset.GNB_ST_Rampart) && 
                        Role.CanRampart(GNB_ST_Rampart_Health) &&
                        (GNB_ST_Rampart_SubOption == 0 || TargetIsBoss() && GNB_ST_Rampart_SubOption == 1))
                        return Role.Rampart;

                    if (IsEnabled(Preset.GNB_ST_Reprisal) && 
                        Role.CanReprisal(GNB_ST_Reprisal_Health) &&
                        (GNB_ST_Reprisal_SubOption == 0 || TargetIsBoss() && GNB_ST_Reprisal_SubOption == 1))
                        return Role.Reprisal;

                    if (IsEnabled(Preset.GNB_ST_ArmsLength) &&
                        HPP < GNB_AoE_ArmsLength_Health &&
                        Role.CanArmsLength())
                        return Role.ArmsLength;
                }

                if (IsEnabled(Preset.GNB_ST_Camouflage) && 
                    ActionReady(Camouflage) &&
                    HPP < GNB_ST_Camouflage_Health &&
                    (GNB_ST_Camouflage_SubOption == 0 || TargetIsBoss() && GNB_ST_Camouflage_SubOption == 1))
                    return Camouflage;

                if (IsEnabled(Preset.GNB_ST_Corundum) && 
                    ActionReady(OriginalHook(HeartOfStone)) && 
                    HPP < GNB_ST_Corundum_Health &&
                    (GNB_ST_Corundum_SubOption == 0 || TargetIsBoss() && GNB_ST_Corundum_SubOption == 1))
                    return OriginalHook(HeartOfStone);

                if (IsEnabled(Preset.GNB_ST_Aurora) && 
                    ActionReady(Aurora) &&
                    !(HasStatusEffect(Buffs.Aurora) || HasStatusEffect(Buffs.Aurora, CurrentTarget, true)) &&
                    GetRemainingCharges(Aurora) > GNB_ST_Aurora_Charges && HPP < GNB_ST_Aurora_Health &&
                    (GNB_ST_Aurora_SubOption == 0 || TargetIsBoss() && GNB_ST_Aurora_SubOption == 1))
                    return Aurora;
            }
            #endregion

            #endregion

            #region Rotation

            //Openers
            if (IsEnabled(Preset.GNB_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            //Lightning Shot
			if (ShouldUseLightningShot(Preset.GNB_ST_RangedUptime, GNB_ST_HoldLightningShot))
                return LightningShot;

            //MAX PRIORITY - just clip it, it's better than just losing it altogether
            //Continuation procs (Hypervelocity, Jugular Rip, Abdomen Tear, Eye Gouge)
            if ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) &&
                RemainingGCD < 0.6f &&
                IsEnabled(Preset.GNB_ST_Continuation))
                return OriginalHook(Continuation);

            //No Mercy
			if (ShouldUseNoMercy(Preset.GNB_ST_NoMercy, GNB_ST_NoMercyStop, GNB_ST_NoMercy_SubOption))
				return NoMercy;

            //Bloodfest
            if (ShouldUseBloodfest(Preset.GNB_ST_Bloodfest))
				return Bloodfest;

            //HIGH PRIORITY - within late weave window, send now
            //Continuation procs (Hypervelocity, Jugular Rip, Abdomen Tear, Eye Gouge)
            if ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) &&
                CanDelayedWeave() &&
                IsEnabled(Preset.GNB_ST_Continuation))
                return OriginalHook(Continuation);

            //Hypervelocity
            //if No Mercy is imminent, then we want to aim for buffing HV right after using Burst Strike (BS^NM^HV>GF>etc.)
            if (IsEnabled(Preset.GNB_ST_Continuation) &&
                IsEnabled(Preset.GNB_ST_NoMercy) &&
				JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasStatusEffect(Buffs.ReadyToBlast) &&
				NMcd is > 1.3f)
				return Hypervelocity;

            //Bow Shock & Zone
			//with SKS, we want Zone first because it can drift really bad while Bow usually remains static
			//without SKS, we don't really care since both usually remain static
			if (Slow ? ShouldUseBowShock(Preset.GNB_ST_BowShock) : ShouldUseZone(Preset.GNB_ST_Zone))
				return Slow ? BowShock : OriginalHook(DangerZone);
			if (Slow ? ShouldUseZone(Preset.GNB_ST_Zone) : ShouldUseBowShock(Preset.GNB_ST_BowShock))
				return Slow ? OriginalHook(DangerZone) : BowShock;

            //NORMAL PRIORITY - within weave weave window
            //Gnashing Fang procs (Jugular Rip, Abdomen Tear, Eye Gouge)
            if (CanContinue &&
                IsEnabled(Preset.GNB_ST_Continuation) && 
				CanWeave())
				return OriginalHook(Continuation);

            //Gnashing Fang - burst
            if (ShouldUseGnashingFangBurst(Preset.GNB_ST_GnashingFang))
				return GnashingFang;

            //Double Down
            if (ShouldUseDoubleDown(Preset.GNB_ST_DoubleDown))
				return DoubleDown;

            //Sonic Break
            if (ShouldUseSonicBreak(Preset.GNB_ST_SonicBreak))
				return SonicBreak;

            //Reign of Beasts
            if (ShouldUseReignOfBeasts(Preset.GNB_ST_Reign))
				return OriginalHook(ReignOfBeasts);

            //Gnashing Fang 2 - filler boogaloo
            if (ShouldUseGnashingFangFiller(Preset.GNB_ST_GnashingFang))
                return GnashingFang;

            //Savage Claw & Wicked Talon
			if (IsEnabled(Preset.GNB_ST_GnashingFang) && 
                GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //Noble Blood & Lion Heart
            if (IsEnabled(Preset.GNB_ST_Reign) &&
                GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            //Burst Strike
            if (ShouldUseBurstStrike(Preset.GNB_ST_BurstStrike))
                return BurstStrike;

            //1-2-3
            return STCombo(GNB_ST_Overcap_Choice);

            #endregion
        }
    }
    #endregion

    #region Simple Mode - AoE
    internal class GNB_AoE_Simple : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != DemonSlice)
                return actionID;

            #region Non-Rotation

            if (Role.CanInterject())
                return Role.Interject;

            if (Role.CanLowBlow())
                return Role.LowBlow;

            if (BozjaActions() != 0)
                return BozjaActions();

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            var mitigationsOn =
                GNB_AoE_MitsOptions != 1 ||
                (P.UIHelper.PresetControlled(Preset)?.enabled == true);
            if (mitigationsOn)
            {
                if (InCombat() && !MitUsed)
                {
                    if (ActionReady(Superbolide) && HPP < 30)
                        return Superbolide;
                    if (IsPlayerTargeted())
                    {
                        if (ActionReady(OriginalHook(Nebula)) && HPP < 60)
                            return OriginalHook(Nebula);
                        if (Role.CanRampart(80))
                            return Role.Rampart;
                        if (Role.CanReprisal(90, checkTargetForDebuff: false))
                            return Role.Reprisal;
                    }
                    if (ActionReady(Camouflage) && HPP < 70)
                        return Camouflage;
                    if (ActionReady(OriginalHook(HeartOfStone)) && HPP < 90)
                        return OriginalHook(HeartOfStone);
                    if (ActionReady(Aurora) && !(HasStatusEffect(Buffs.Aurora) || HasStatusEffect(Buffs.Aurora, CurrentTarget, true)) && HPP < 85)
                        return Aurora;
                }
            }
            #endregion

            #endregion

            #region Rotation

            if (InCombat())
            {
                if (CanWeave())
                {
                    if (ShouldUseNoMercy(Preset.GNB_AoE_NoMercy, 10, 0))
                        return NoMercy;

                    if (LevelChecked(FatedBrand) && 
                        HasStatusEffect(Buffs.ReadyToRaze))
                        return FatedBrand;
                }
                if (ShouldUseBowShock(Preset.GNB_AoE_Simple))
                    return BowShock;

                if (ShouldUseZone(Preset.GNB_AoE_Simple))
                    return OriginalHook(DangerZone);

                if (ShouldUseBloodfest(Preset.GNB_AoE_Simple))
                    return Bloodfest;

                if (CanSB && HasNM && !HasStatusEffect(Buffs.ReadyToRaze))
                    return SonicBreak;

                if (CanDD && HasNM)
                    return DoubleDown;

                if ((CanReign && HasNM) || GunStep is 3 or 4)
                    return OriginalHook(ReignOfBeasts);

                if (ShouldUseFatedCircle(Preset.GNB_AoE_Simple))
                    return LevelChecked(FatedCircle) ? FatedCircle : BurstStrike;
            }

            return AOECombo(GNB_AoE_Overcap_Choice, GNB_AoE_FatedCircle_BurstStrike);

            #endregion
        }
    }
    #endregion

    #region Advanced Mode - AoE
    internal class GNB_AoE_Advanced : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_AoE_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != DemonSlice)
                return actionID;

            #region Non-Rotation

            if (IsEnabled(Preset.GNB_AoE_Interrupt) && Role.CanInterject())
                return Role.Interject;

            if (IsEnabled(Preset.GNB_AoE_Stun) && Role.CanLowBlow())
                return Role.LowBlow;

            if (BozjaActions() != 0)
                return BozjaActions();

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            if (IsEnabled(Preset.GNB_AoE_Mitigation) && InCombat() && !MitUsed)
            {
                if (IsEnabled(Preset.GNB_AoE_Superbolide) && 
                    ActionReady(Superbolide) && 
                    HPP < GNB_AoE_Superbolide_Health &&
                    (GNB_AoE_Superbolide_SubOption == 0 || TargetIsBoss() && GNB_AoE_Superbolide_SubOption == 1))
                    return Superbolide;

                if (IsPlayerTargeted())
                {
                    if (IsEnabled(Preset.GNB_AoE_Nebula) && 
                        ActionReady(OriginalHook(Nebula)) &&
                        HPP < GNB_AoE_Nebula_Health &&
                        (GNB_AoE_Nebula_SubOption == 0 || TargetIsBoss() && GNB_AoE_Nebula_SubOption == 1))
                        return OriginalHook(Nebula);

                    if (IsEnabled(Preset.GNB_AoE_Rampart) && 
                        Role.CanRampart(GNB_AoE_Rampart_Health) &&
                        (GNB_AoE_Rampart_SubOption == 0 || TargetIsBoss() && GNB_AoE_Rampart_SubOption == 1))
                        return Role.Rampart;

                    if (IsEnabled(Preset.GNB_AoE_Reprisal) &&
                        Role.CanReprisal(GNB_AoE_Reprisal_Health, checkTargetForDebuff: false) &&
                        (GNB_AoE_Reprisal_SubOption == 0 || TargetIsBoss() && GNB_AoE_Reprisal_SubOption == 1))
                        return Role.Reprisal;

                    if (IsEnabled(Preset.GNB_AoE_ArmsLength) &&
                        HPP < GNB_AoE_ArmsLength_Health &&
                        Role.CanArmsLength())
                        return Role.ArmsLength;
                }

                if (IsEnabled(Preset.GNB_AoE_Camouflage) &&
                    ActionReady(Camouflage) && 
                    HPP < GNB_AoE_Camouflage_Health &&
                    (GNB_AoE_Camouflage_SubOption == 0 || TargetIsBoss() && GNB_AoE_Camouflage_SubOption == 1))
                    return Camouflage;
                if (IsEnabled(Preset.GNB_AoE_Corundum) &&
                    ActionReady(OriginalHook(HeartOfStone)) && 
                    HPP < GNB_AoE_Corundum_Health &&
                    (GNB_AoE_Corundum_SubOption == 0 || TargetIsBoss() && GNB_AoE_Corundum_SubOption == 1))
                    return OriginalHook(HeartOfStone);

                if (IsEnabled(Preset.GNB_AoE_Aurora) && 
                    ActionReady(Aurora) && 
                    GetRemainingCharges(Aurora) > GNB_AoE_Aurora_Charges &&
                    !(HasStatusEffect(Buffs.Aurora) || HasStatusEffect(Buffs.Aurora, CurrentTarget, true)) && 
                    HPP < GNB_AoE_Aurora_Health &&
                    (GNB_AoE_Aurora_SubOption == 0 || TargetIsBoss() && GNB_AoE_Aurora_SubOption == 1))
                    return Aurora;
            }

            #endregion
            
            #endregion

            #region Rotation

            var aoe = AOECombo(GNB_AoE_Overcap_Choice, GNB_AoE_FatedCircle_BurstStrike);
            if (InCombat())
            {
                if (CanWeave())
                {
                    if (ShouldUseNoMercy(Preset.GNB_AoE_NoMercy, GNB_AoE_NoMercyStop, 0))
                        return NoMercy;

                    if (LevelChecked(FatedBrand) && 
                        HasStatusEffect(Buffs.ReadyToRaze))
                        return FatedBrand;

                    if (ShouldUseBowShock(Preset.GNB_AoE_BowShock))
                        return BowShock;

                    if (ShouldUseZone(Preset.GNB_AoE_Zone))
                        return OriginalHook(DangerZone);

                    if (ShouldUseBloodfest(Preset.GNB_AoE_Bloodfest))
                        return Bloodfest;
                }

                if (IsEnabled(Preset.GNB_AoE_SonicBreak) &&
                    CanSB &&
                    HasNM &&
                    !HasStatusEffect(Buffs.ReadyToRaze))
                    return SonicBreak;

                if (IsEnabled(Preset.GNB_AoE_DoubleDown) &&
                    CanDD && 
                    HasNM)
                    return DoubleDown;

                if (IsEnabled(Preset.GNB_AoE_Reign) && 
                    ((CanReign && HasNM) || GunStep is 3 or 4))
                    return OriginalHook(ReignOfBeasts);

                if (ShouldUseFatedCircle(Preset.GNB_AoE_FatedCircle))
                    return 
                        LevelChecked(FatedCircle) ? FatedCircle :
                        LevelChecked(BurstStrike) && GNB_AoE_FatedCircle_BurstStrike == 0 ? BurstStrike : 
                        aoe;
            }

            return aoe;
            #endregion
        }
    }
    #endregion

    #region Gnashing Fang Features
    internal class GNB_GF_Features : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_GF_Features;

        protected override uint Invoke(uint actionID)
        {
            var GFchoice = GNB_GF_Features_Choice == 0; //Gnashing Fang as button
			var NMchoice = GNB_GF_Features_Choice == 1; //No Mercy as button
            if ((GFchoice && actionID != GnashingFang) ||
                (NMchoice && actionID != NoMercy))
                return actionID;

            //MAX PRIORITY - just clip it, it's better than just losing it altogether
            //Continuation procs (Hypervelocity, Jugular Rip, Abdomen Tear, Eye Gouge)
            if ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) &&
                CanDelayedWeave(0.6f, 0.0f) &&
                IsEnabled(Preset.GNB_GF_Continuation))
                return OriginalHook(Continuation);

            //No Mercy
            if (ShouldUseNoMercy(Preset.GNB_GF_NoMercy, 0, 0))
                return NoMercy;

            //Bloodfest
            if (ShouldUseBloodfest(Preset.GNB_GF_Bloodfest))
                return Bloodfest;

            //HIGH PRIORITY - within late weave window, send now
            //Continuation procs (Hypervelocity, Jugular Rip, Abdomen Tear, Eye Gouge)
            if ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) &&
                CanDelayedWeave() &&
                IsEnabled(Preset.GNB_GF_Continuation))
                return OriginalHook(Continuation);

            //Hypervelocity
            //if No Mercy is imminent, then we want to aim for buffing HV right after using Burst Strike (BS^NM^HV>GF>etc.)
            if (IsEnabled(Preset.GNB_GF_Continuation) &&
                IsEnabled(Preset.GNB_GF_NoMercy) &&
                JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasStatusEffect(Buffs.ReadyToBlast) &&
                NMcd is > 1.3f)
                return Hypervelocity;

            //Bow Shock & Zone
            //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
            //without SKS, we don't really care since both usually remain static
            if (Slow ? ShouldUseBowShock(Preset.GNB_GF_BowShock) : ShouldUseZone(Preset.GNB_GF_Zone))
                return Slow ? BowShock : OriginalHook(DangerZone);
            if (Slow ? ShouldUseZone(Preset.GNB_GF_Zone) : ShouldUseBowShock(Preset.GNB_GF_BowShock))
                return Slow ? OriginalHook(DangerZone) : BowShock;

            //NORMAL PRIORITY - within weave weave window
            //Gnashing Fang procs (Jugular Rip, Abdomen Tear, Eye Gouge)
            if (CanContinue &&
                IsEnabled(Preset.GNB_GF_Continuation) &&
                CanWeave())
                return OriginalHook(Continuation);

            //Sonic Break
            if (ShouldUseSonicBreak(Preset.GNB_GF_SonicBreak))
                return SonicBreak;

            //Gnashing Fang - burst
            if (ShouldUseGnashingFangBurst(Preset.GNB_GF_Features))
                return GnashingFang;

            //Double Down
            if (ShouldUseDoubleDown(Preset.GNB_GF_DoubleDown))
                return DoubleDown;

            //Reign of Beasts
            if (ShouldUseReignOfBeasts(Preset.GNB_GF_Reign))
                return OriginalHook(ReignOfBeasts);

            //Gnashing Fang 2 - filler boogaloo
            if (ShouldUseGnashingFangFiller(Preset.GNB_GF_Features))
                return GnashingFang;

            //Savage Claw & Wicked Talon
            if (IsEnabled(Preset.GNB_GF_Features) &&
                GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //Noble Blood & Lion Heart
            if (IsEnabled(Preset.GNB_GF_Reign) &&
                GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            //Burst Strike
            if (ShouldUseBurstStrike(Preset.GNB_GF_BurstStrike))
                return BurstStrike;

            return actionID;
        }
    }
    #endregion

    #region Burst Strike Features
    internal class GNB_BS_Features : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_BS_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != BurstStrike)
                return actionID;

            if (IsEnabled(Preset.GNB_BS_Continuation))
            {
                if (IsEnabled(Preset.GNB_BS_Hypervelocity) &&
                    LevelChecked(Hypervelocity) &&
                    (JustUsed(BurstStrike, 1) || HasStatusEffect(Buffs.ReadyToBlast)))
                    return Hypervelocity;

                if (!IsEnabled(Preset.GNB_BS_Hypervelocity) && 
                    (CanContinue || (LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast))))
                    return OriginalHook(Continuation);
            }

            if (ShouldUseBloodfest(Preset.GNB_BS_Bloodfest))
                return Bloodfest;

            var useDD = IsEnabled(Preset.GNB_BS_DoubleDown) && CanDD;
            if (useDD && Ammo >= 2)
                return DoubleDown;

            if (IsEnabled(Preset.GNB_BS_GnashingFang) && (CanGF || GunStep is 1 or 2))
                return OriginalHook(GnashingFang);

            if (useDD && Ammo >= 2)
                return DoubleDown;

            if (IsEnabled(Preset.GNB_BS_Reign) && (CanReign || GunStep is 3 or 4))
                return OriginalHook(ReignOfBeasts);

            return actionID;
        }
    }
    #endregion

    #region Fated Circle Features
    internal class GNB_FC_Features : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_FC_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != FatedCircle)
                return actionID;

            if (IsEnabled(Preset.GNB_FC_Continuation) && 
                HasStatusEffect(Buffs.ReadyToRaze) &&
                LevelChecked(FatedBrand))
                return FatedBrand;

            if (IsEnabled(Preset.GNB_FC_DoubleDown) && 
                IsEnabled(Preset.GNB_FC_DoubleDown_NM) &&
                CanDD && HasNM)
                return DoubleDown;

            if (ShouldUseBloodfest(Preset.GNB_FC_Bloodfest))
                return Bloodfest;

            if (IsEnabled(Preset.GNB_FC_BowShock) && CanUse(BowShock))
                return BowShock;

            if (IsEnabled(Preset.GNB_FC_DoubleDown) && !IsEnabled(Preset.GNB_FC_DoubleDown_NM) && CanDD)
                return DoubleDown;

            if (IsEnabled(Preset.GNB_FC_Reign) && (CanReign || GunStep is 3 or 4))
                return OriginalHook(ReignOfBeasts);

            return actionID;
        }
    }
    #endregion

    #region No Mercy Features
    internal class GNB_NM_Features : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_NM_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != NoMercy)
                return actionID;
            if (GNB_NM_Features_Weave == 0 && CanWeave() || GNB_NM_Features_Weave == 1)
            {
                var useZone = IsEnabled(Preset.GNB_NM_Zone) && CanUse(OriginalHook(DangerZone)) && NMcd is < 57.5f and > 17f;
                var useBow = IsEnabled(Preset.GNB_NM_BowShock) && CanUse(BowShock) && NMcd is < 57.5f and >= 40;
                if (IsEnabled(Preset.GNB_NM_Continuation) && CanContinue && 
                    (HasStatusEffect(Buffs.ReadyToRip) || HasStatusEffect(Buffs.ReadyToTear) || HasStatusEffect(Buffs.ReadyToGouge) || (LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast) || (LevelChecked(FatedBrand) && HasStatusEffect(Buffs.ReadyToRaze)))))
                    return OriginalHook(Continuation);
                if (IsEnabled(Preset.GNB_NM_Bloodfest) && HasBattleTarget() && CanUse(Bloodfest))
                    return Bloodfest;
                //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
                //without SKS, we don't really care since both usually remain static
                if (Slow ? useBow : useZone)
                    return Slow ? BowShock : OriginalHook(DangerZone);
                if (Slow ? useZone : useBow)
                    return Slow ? OriginalHook(DangerZone) : BowShock;
            }
            return actionID;
        }
    }

    #endregion

    #region One-Button Mitigation
    internal class GNB_Mit_OneButton : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Camouflage)
                return actionID;
            if (IsEnabled(Preset.GNB_Mit_Superbolide_Max) && ActionReady(Superbolide) &&
                HPP <= GNB_Mit_Superbolide_Health &&
                ContentCheck.IsInConfiguredContent(GNB_Mit_Superbolide_Difficulty, GNB_Mit_Superbolide_DifficultyListSet))
                return Superbolide;
            foreach(int priority in GNB_Mit_Priorities.OrderBy(x => x))
            {
                int index = GNB_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }
            return actionID;
        }
    }
    #endregion

    #region Reprisal -> Heart of Light
    internal class GNB_Mit_Party : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_Mit_Party;
        protected override uint Invoke(uint action) => action != HeartOfLight ? action : Role.CanReprisal() ? Role.Reprisal : action;
    }
    #endregion

    #region Aurora Protection and Retargetting
    internal class GNB_AuroraProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_AuroraProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Aurora)
                return actionID;

            var target =
                //Mouseover retarget option
                (IsEnabled(Preset.GNB_RetargetAurora_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfFriendly()
                    : null) ??

                //Hard target
                SimpleTarget.HardTarget.IfFriendly() ??

                //Partner Tank
                (IsEnabled(Preset.GNB_RetargetAurora_TT) && !PlayerHasAggro && InCombat()
                    ? SimpleTarget.TargetsTarget.IfFriendly()
                    : null);

            if (target != null && CanApplyStatus(target, Buffs.Aurora))
            {
                return !HasStatusEffect(Buffs.Aurora, target, true)
                    ? actionID.Retarget(target)
                    : All.SavageBlade;
            }

            return !HasStatusEffect(Buffs.Aurora, SimpleTarget.Self, true)
                ? actionID
                : All.SavageBlade;
        }
    }
    #endregion
    
    #region Heart of Corundum Retarget

    internal class GNB_RetargetHeartofStone : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_RetargetHeartofStone;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeartOfStone or HeartOfCorundum))
                return actionID;

            var target =
                SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty() ??
                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty() ??
                (IsEnabled(Preset.GNB_RetargetHeartofStone_TT) && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfNotThePlayer().IfInParty()
                    : null);

            if (target is not null && CanApplyStatus(target, Buffs.HeartOfStone))
                return OriginalHook(actionID).Retarget([HeartOfStone,HeartOfCorundum], target);

            return actionID;

        }
    }

    #endregion

    #region Basic Combo
    internal class GNB_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_ST_BasicCombo;

        protected override uint Invoke(uint actionID) => actionID != SolidBarrel ? actionID :
            ComboTimer > 0 && ComboAction is KeenEdge && LevelChecked(BrutalShell) ? BrutalShell :
            ComboTimer > 0 && ComboAction is BrutalShell && LevelChecked(SolidBarrel) ? SolidBarrel : KeenEdge;
    }
    #endregion
}
