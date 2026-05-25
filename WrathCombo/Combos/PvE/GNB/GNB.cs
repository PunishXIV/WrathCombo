#region Dependencies

using Dalamud.Game.ClientState.Objects.Types;
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
            if (Role.CanInterject())
                return Role.Interject;

            if (Role.CanLowBlow())
                return Role.LowBlow;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (GNB_ST_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.simple, ref actionID))
                    return actionID;
            }           
            #endregion

            #region Rotation
            //Lightning Shot
            if (ShouldUseLightningShot(Preset.GNB_ST_Simple, 1, 0))
                return LightningShot;

            //Continuation (MAX PRIORITY): just clip it - it's better than just losing it altogether
            if (ShouldContinue(Preset.GNB_ST_Simple, CanContinue || CanHV, RemainingGCD < 0.6f))
                return OriginalHook(Continuation);

            //No Mercy
            if (ShouldUseNoMercy(Preset.GNB_ST_Simple, 0))
                return NoMercy;

            //Bloodfest
            if (ShouldUseBloodfest(Preset.GNB_ST_Simple))
                return Bloodfest;

            //Continuation (HIGH PRIORITY): within late weave window, just send now
            if (ShouldContinue(Preset.GNB_ST_Simple, CanContinue || CanHV, CanDelayedWeave()))
                return OriginalHook(Continuation);

            //Hypervelocity
            //2.5 - if No Mercy is imminent, then we want to aim for buffing HV after using Burst Strike instead of sending it right away (BS^NM^HV>GF>etc.)
            //2.4x - just use it after Burst Strike
            if (JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasStatusEffect(Buffs.ReadyToBlast) &&
                (!Slow || NMcd > 1.3f))
                return Hypervelocity;

            //Bow Shock & Zone
            //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
            //without SKS, we don't really care since both usually remain static
            if (Slow ? ShouldUseBowShock(Preset.GNB_ST_Simple) : ShouldUseZone(Preset.GNB_ST_Simple))
                return Slow ? BowShock : OriginalHook(DangerZone);
            if (Slow ? ShouldUseZone(Preset.GNB_ST_Simple) : ShouldUseBowShock(Preset.GNB_ST_Simple))
                return Slow ? OriginalHook(DangerZone) : BowShock;

            //Continuation (NORMAL PRIORITY): just send whenever
            if (ShouldContinue(Preset.GNB_ST_Simple, CanContinue, CanWeave()))
                return OriginalHook(Continuation);

            //Burst - at Lv100 we want to send Reign as soon as we enter No Mercy (unless we're in opener/reopener with max GF charges), else we send Gnashing Fang (since no Reign)
            var wantGF = GetCooldownRemainingTime(GnashingFang) < 0.5f || !LevelChecked(ReignOfBeasts);
            if (wantGF ? ShouldUseGnashingFangBurst(Preset.GNB_ST_Simple) : ShouldUseReignOfBeasts(Preset.GNB_ST_Simple))
                return wantGF ? GnashingFang : ReignOfBeasts;

            //Double Down
            if (ShouldUseDoubleDown(Preset.GNB_ST_Simple))
                return DoubleDown;

            //Sonic Break
            if (ShouldUseSonicBreak(Preset.GNB_ST_Simple))
                return SonicBreak;

            //Gnashing Fang 2 - filler boogaloo
            if (ShouldUseGnashingFangFiller(Preset.GNB_ST_Simple, 0))
                return GnashingFang;

            //Noble Blood & Lion Heart
            if (GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            //Savage Claw & Wicked Talon
            if (GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //Burst Strike
            if (ShouldUseBurstStrike(Preset.GNB_ST_Simple, 0))
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

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (GNB_ST_Advanced_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.advanced, ref actionID))
                    return actionID;
            }
            #endregion

            #region Rotation
            //Openers
            if (IsEnabled(Preset.GNB_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            //Lightning Shot
            if (ShouldUseLightningShot(Preset.GNB_ST_RangedUptime, GNB_ST_HoldLightningShot, GNB_ST_HoldLightningShotInBurst))
                return LightningShot;

            //Continuation (MAX PRIORITY): just clip it - it's better than just losing it altogether
            if (ShouldContinue(Preset.GNB_ST_Continuation, CanContinue || CanHV, RemainingGCD < 0.6f))
                return OriginalHook(Continuation);

            //No Mercy
            if (ShouldUseNoMercy(Preset.GNB_ST_NoMercy, HPThresholdNM))
                return NoMercy;

            //Bloodfest
            if (ShouldUseBloodfest(Preset.GNB_ST_Bloodfest))
                return Bloodfest;

            //Continuation (HIGH PRIORITY): within late weave window, send now
            if (ShouldContinue(Preset.GNB_ST_Continuation, CanContinue || CanHV, CanDelayedWeave()))
                return OriginalHook(Continuation);

            //Hypervelocity
            //2.5 - if No Mercy is imminent, then we want to aim for buffing HV after using Burst Strike instead of sending it right away (BS^NM^HV>GF>etc.)
            //2.4x - just use it after Burst Strike
            if (IsEnabled(Preset.GNB_ST_Continuation) &&
                JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasStatusEffect(Buffs.ReadyToBlast) &&
                (!Slow || (IsEnabled(Preset.GNB_ST_NoMercy) && NMcd > 1.3f)))
                return Hypervelocity;

            //Bow Shock & Zone
            //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
            //without SKS, we don't really care since both usually remain static
            if (Slow ? ShouldUseBowShock(Preset.GNB_ST_BowShock) : ShouldUseZone(Preset.GNB_ST_Zone))
                return Slow ? BowShock : OriginalHook(DangerZone);
            if (Slow ? ShouldUseZone(Preset.GNB_ST_Zone) : ShouldUseBowShock(Preset.GNB_ST_BowShock))
                return Slow ? OriginalHook(DangerZone) : BowShock;

            //Continuation (NORMAL PRIORITY): just send whenever
            if (ShouldContinue(Preset.GNB_ST_Continuation, CanContinue, CanWeave()))
                return OriginalHook(Continuation);

            //Burst - at Lv100 we want to send Reign as soon as we enter No Mercy (unless we're in opener/reopener with max GF charges), else we send Gnashing Fang (since no Reign)
            var wantGF = GetCooldownRemainingTime(GnashingFang) < 0.5f || !LevelChecked(ReignOfBeasts);
            if (wantGF ? ShouldUseGnashingFangBurst(Preset.GNB_ST_GnashingFang) : ShouldUseReignOfBeasts(Preset.GNB_ST_Reign))
                return wantGF ? GnashingFang : ReignOfBeasts;

            //Double Down
            if (ShouldUseDoubleDown(Preset.GNB_ST_DoubleDown))
                return DoubleDown;

            //Sonic Break
            if (ShouldUseSonicBreak(Preset.GNB_ST_SonicBreak))
                return SonicBreak;

            //Gnashing Fang 2 - filler boogaloo
            if (ShouldUseGnashingFangFiller(Preset.GNB_ST_GnashingFang, GNB_ST_HoldGFCharge))
                return GnashingFang;

            //Noble Blood & Lion Heart
            if (IsEnabled(Preset.GNB_ST_Reign) &&
                InActionRange(OriginalHook(ReignOfBeasts)) &&
                GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            //Savage Claw & Wicked Talon
            if (IsEnabled(Preset.GNB_ST_GnashingFang) && 
                InActionRange(OriginalHook(GnashingFang)) &&
                GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //Burst Strike
            if (ShouldUseBurstStrike(Preset.GNB_ST_BurstStrike, GNB_ST_BurstStrike_Setup))
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

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (GNB_AoE_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.simple, ref actionID))
                    return actionID;
            }
            #endregion

            #region Rotation
            if (InCombat())
            {
                if (ShouldContinue(Preset.GNB_AoE_Simple, CanFB, RemainingGCD < 0.6f))
                    return OriginalHook(Continuation);

                if (ShouldUseNoMercy(Preset.GNB_AoE_Simple, 10))
                    return NoMercy;

                if (ShouldUseBloodfest(Preset.GNB_AoE_Simple))
                    return Bloodfest;

                if (ShouldContinue(Preset.GNB_AoE_Simple, CanFB, CanDelayedWeave()))
                    return OriginalHook(Continuation);

                if (ShouldUseBowShock(Preset.GNB_AoE_Simple))
                    return BowShock;

                if (ShouldUseZone(Preset.GNB_AoE_Simple))
                    return OriginalHook(DangerZone);

                if (ShouldContinue(Preset.GNB_AoE_Simple, CanFB, CanWeave()))
                    return OriginalHook(Continuation);

                if (ShouldUseReignOfBeasts(Preset.GNB_AoE_Simple))
                    return ReignOfBeasts;

                if (ShouldUseDoubleDown(Preset.GNB_AoE_Simple))
                    return DoubleDown;

                if (ShouldUseSonicBreak(Preset.GNB_AoE_Simple) && !HasStatusEffect(Buffs.ReadyToRaze))
                    return SonicBreak;

                if (GunStep is 3 or 4)
                    return OriginalHook(ReignOfBeasts);

                if (ShouldUseFatedCircle(Preset.GNB_AoE_Simple, 0))
                    return LevelChecked(FatedCircle) ? FatedCircle : BurstStrike;
            }

            return AOECombo(0, 0);
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

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (GNB_AoE_Advanced_MitOptions != 1 || P.UIHelper.PresetControlled(Preset)?.enabled == true)
            {
                if (TryUseMits(RotationMode.advanced, ref actionID))
                    return actionID;
            }
            
            #endregion

            #region Rotation
            var aoe = AOECombo(GNB_AoE_Overcap_Choice, GNB_AoE_FatedCircle_BurstStrike);
            if (InCombat())
            {
                if (ShouldContinue(Preset.GNB_AoE_FatedBrand, CanFB, RemainingGCD < 0.6f))
                    return OriginalHook(Continuation);

                if (ShouldUseNoMercy(Preset.GNB_AoE_NoMercy, GNB_AoE_NoMercyStop))
                    return NoMercy;

                if (ShouldUseBloodfest(Preset.GNB_AoE_Bloodfest))
                    return Bloodfest;

                if (ShouldContinue(Preset.GNB_AoE_FatedBrand, CanFB, CanDelayedWeave()))
                    return OriginalHook(Continuation);

                if (ShouldUseBowShock(Preset.GNB_AoE_BowShock))
                    return BowShock;

                if (ShouldUseZone(Preset.GNB_AoE_Zone))
                    return OriginalHook(DangerZone);

                if (ShouldContinue(Preset.GNB_AoE_FatedBrand, CanFB, CanWeave()))
                    return OriginalHook(Continuation);

                if (ShouldUseReignOfBeasts(Preset.GNB_AoE_Reign))
                    return ReignOfBeasts;

                if (ShouldUseDoubleDown(Preset.GNB_AoE_DoubleDown))
                    return DoubleDown;

                if (IsEnabled(Preset.GNB_AoE_SonicBreak) && CanSB &&
                    ((GNB_AoE_SonicBreak_EarlyOrLate == 0) ||
                    (GNB_AoE_SonicBreak_EarlyOrLate == 1 && GetStatusEffectRemainingTime(Buffs.ReadyToBreak) <= (GCDLength + 10.000f))) &&
                    !HasStatusEffect(Buffs.ReadyToRaze))
                    return SonicBreak;

                if (IsEnabled(Preset.GNB_AoE_Reign) &&
                    GunStep is 3 or 4)
                    return OriginalHook(ReignOfBeasts);

                if (ShouldUseFatedCircle(Preset.GNB_AoE_FatedCircle, GNB_AoE_FatedCircle_Setup))
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

            //Continuation (MAX PRIORITY): just clip it - it's better than just losing it altogether
            if (ShouldContinue(Preset.GNB_GF_Continuation, CanHV || CanContinue, RemainingGCD < 0.6f))
                return OriginalHook(Continuation);

            //No Mercy
            if (ShouldUseNoMercy(Preset.GNB_GF_NoMercy, 0))
                return NoMercy;

            //Bloodfest
            if (ShouldUseBloodfest(Preset.GNB_GF_Bloodfest))
                return Bloodfest;

            //Continuation (HIGH PRIORITY): within late weave window, send now
            if (ShouldContinue(Preset.GNB_GF_Continuation, CanHV || CanContinue, CanDelayedWeave()))
                return OriginalHook(Continuation);

            //Hypervelocity
            //2.5 - if No Mercy is imminent, then we want to aim for buffing HV after using Burst Strike instead of sending it right away (BS^NM^HV>GF>etc.)
            //2.4x - just use it after Burst Strike
            if (IsEnabled(Preset.GNB_GF_Continuation) &&
                JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasStatusEffect(Buffs.ReadyToBlast) &&
                (!Slow || (IsEnabled(Preset.GNB_GF_NoMercy) && NMcd > 1.3f)))
                return Hypervelocity;

            //Bow Shock & Zone
            //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
            //without SKS, we don't really care since both usually remain static
            if (Slow ? ShouldUseBowShock(Preset.GNB_GF_BowShock) : ShouldUseZone(Preset.GNB_GF_Zone))
                return Slow ? BowShock : OriginalHook(DangerZone);
            if (Slow ? ShouldUseZone(Preset.GNB_GF_Zone) : ShouldUseBowShock(Preset.GNB_GF_BowShock))
                return Slow ? OriginalHook(DangerZone) : BowShock;

            //Continuation (NORMAL PRIORITY): just send whenever
            if (ShouldContinue(Preset.GNB_GF_Continuation, CanContinue, CanWeave()))
                return OriginalHook(Continuation);

            //Burst - at Lv100 we want to send Reign as soon as we enter No Mercy (unless we're in opener/reopener with max GF charges), else we send Gnashing Fang (since no Reign)
            var wantGF = GetCooldownRemainingTime(GnashingFang) < 0.5f || !LevelChecked(ReignOfBeasts);
            if (wantGF ? ShouldUseGnashingFangBurst(Preset.GNB_GF_Features) : ShouldUseReignOfBeasts(Preset.GNB_GF_Reign))
                return wantGF ? GnashingFang : ReignOfBeasts;

            //Double Down
            if (ShouldUseDoubleDown(Preset.GNB_GF_DoubleDown))
                return DoubleDown;

            //Sonic Break
            if (ShouldUseSonicBreak(Preset.GNB_GF_SonicBreak))
                return SonicBreak;

            //Gnashing Fang 2 - filler boogaloo
            if (ShouldUseGnashingFangFiller(Preset.GNB_GF_Features, 1))
                return GnashingFang;

            //Noble Blood & Lion Heart
            if (IsEnabled(Preset.GNB_GF_Reign) &&
                GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            //Savage Claw & Wicked Talon
            if (IsEnabled(Preset.GNB_GF_Features) &&
                GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //Burst Strike
            if (ShouldUseBurstStrike(Preset.GNB_GF_BurstStrike, GNB_GF_BurstStrike_Setup))
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

            var canContinue = GNB_BS_Continuation_Procs == 1 ? CanHV : CanContinueAny;
            if (ShouldContinue(Preset.GNB_BS_Continuation, canContinue, RemainingGCD < 0.6f))
                return OriginalHook(Continuation);

            if (ShouldUseBloodfest(Preset.GNB_BS_Bloodfest))
                return Bloodfest;

            if (ShouldContinue(Preset.GNB_BS_Continuation, canContinue, CanWeave()))
                return OriginalHook(Continuation);

            var wantGF = GetCooldownRemainingTime(GnashingFang) < 0.5f || !LevelChecked(ReignOfBeasts);
            if (wantGF ? ShouldUseGnashingFangBurst(Preset.GNB_BS_GnashingFang) : ShouldUseReignOfBeasts(Preset.GNB_BS_Reign))
                return wantGF ? GnashingFang : ReignOfBeasts;
            if (ShouldUseDoubleDown(Preset.GNB_BS_DoubleDown))
                return DoubleDown;
            if (ShouldUseGnashingFangFiller(Preset.GNB_BS_GnashingFang, 1))
                return GnashingFang;

            //combos
            if (IsEnabled(Preset.GNB_BS_Reign) && GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);
            if (IsEnabled(Preset.GNB_BS_GnashingFang) && GunStep is 1 or 2)
                return OriginalHook(GnashingFang);

            //failsafe
            return 
                IsEnabled(Preset.GNB_BS_DoubleDown) && CanDD
                    ? DoubleDown
                : IsEnabled(Preset.GNB_BS_GnashingFang) && CanGF 
                    ? GnashingFang 
                    : actionID;
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

            var canContinue = GNB_FC_Continuation_Procs == 1 ? CanFB : CanContinueAny;
            if (ShouldContinue(Preset.GNB_FC_Continuation, canContinue, RemainingGCD < 0.6f))
                return OriginalHook(Continuation);

            if (ShouldUseBloodfest(Preset.GNB_FC_Bloodfest))
                return Bloodfest;

            if (ShouldContinue(Preset.GNB_FC_Continuation, canContinue, CanDelayedWeave()))
                return OriginalHook(Continuation);

            if (CanUseOGCD(BowShock, Preset.GNB_FC_BowShock))
                return BowShock;

            if (ShouldContinue(Preset.GNB_FC_Continuation, canContinue, CanWeave()))
                return OriginalHook(Continuation);

            if (ShouldUseReignOfBeasts(Preset.GNB_FC_Reign))
                return ReignOfBeasts;

            if (ShouldUseDoubleDown(Preset.GNB_FC_DoubleDown) && GNB_FC_DoubleDown_NMOnly == 0)
                return DoubleDown;

            if (IsEnabled(Preset.GNB_FC_DoubleDown) && CanDD && GNB_FC_DoubleDown_NMOnly == 1)
                return DoubleDown;

            if (IsEnabled(Preset.GNB_FC_Reign) &&
                GunStep is 3 or 4)
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

            if (ShouldContinue(Preset.GNB_FC_Continuation, CanContinueAny, RemainingGCD < 0.6f))
                return OriginalHook(Continuation);

            if ((GNB_NM_Features_Weave == 0 && CanWeave()) || GNB_NM_Features_Weave == 1)
            {

                if (ShouldUseBloodfest(Preset.GNB_NM_Bloodfest))
                    return Bloodfest;

                //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
                //without SKS, we don't really care since both usually remain static
                var useZone = CanUseOGCD(OriginalHook(DangerZone), Preset.GNB_NM_Zone) && NMcd is < 57.5f and > 17f;
                var useBow = CanUseOGCD(BowShock, Preset.GNB_NM_BowShock) && NMcd is < 57.5f and >= 40;
                if (Slow ? useBow : useZone)
                    return Slow ? BowShock : OriginalHook(DangerZone);
                if (Slow ? useZone : useBow)
                    return Slow ? OriginalHook(DangerZone) : BowShock;
            }

            if (ShouldContinue(Preset.GNB_FC_Continuation, CanContinueAny, CanWeave()))
                return OriginalHook(Continuation);

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

            if (IsEnabled(Preset.GNB_Mit_OneButton_Superbolide_Max) && ActionReady(Superbolide) &&
                PlayerHealthPercentageHp() <= GNB_Mit_OneButton_Superbolide_Health &&
                ContentCheck.IsInConfiguredContent(GNB_Mit_OneButton_Superbolide_Difficulty, GNB_Mit_OneButton_Superbolide_DifficultyListSet))
                return Superbolide;

            foreach (var priority in GNB_Mit_OneButton_Priorities.OrderBy(x => x))
            {
                var index = GNB_Mit_OneButton_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }
    #endregion

    #region Reprisal -> Heart of Light
    internal class GNB_Mit_OneButton_Party : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_Mit_OneButton_Party;
        protected override uint Invoke(uint action) => action != HeartOfLight ? action : Role.CanReprisal() ? Role.Reprisal : action;
    }
    #endregion

    #region Aurora Features
    internal class GNB_Aurora_Features : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_Aurora_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Aurora)
                return actionID;

            var target =
                (IsEnabled(Preset.GNB_Aurora_Features_RetargetMO) ? SimpleTarget.UIMouseOverTarget.IfFriendly() : null) ??
                SimpleTarget.HardTarget.IfFriendly() ??
                (IsEnabled(Preset.GNB_Aurora_Features_RetargetTT) && !PlayerHasAggro && InCombat() ? SimpleTarget.TargetsTarget.IfFriendly() : null);

            return target != null && CanApplyStatus(target, Buffs.Aurora)
                ? !HasStatusEffect(Buffs.Aurora, target, true)
                    ? actionID.Retarget(target)
                    : All.SavageBlade
                : !HasStatusEffect(Buffs.Aurora, SimpleTarget.Self, true)
                ? actionID
                : All.SavageBlade;
        }
    }
    #endregion

    #region Heart of Corundum Retarget
    internal class GNB_HoC_Features_RetargetMO : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_HoC_Features_RetargetMO;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeartOfStone or HeartOfCorundum))
                return actionID;

            var target =
                SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty() ??
                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty() ??
                (IsEnabled(Preset.GNB_HoC_Features_RetargetTT) && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfNotThePlayer().IfInParty()
                    : null);

            return target is not null && CanApplyStatus(target, Buffs.HeartOfStone)
                ? OriginalHook(actionID).Retarget([HeartOfStone, HeartOfCorundum], target)
                : actionID;
        }
    }
    #endregion

    #region Trajectory Retarget
    internal class GNB_RetargetTrajectory: CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_RetargetTrajectory;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Trajectory)
                return actionID;
            
            var target =
                SimpleTarget.Stack.MouseOver.IfHostile().IfWithinRange(Trajectory.ActionRange()) ?? //Mouseover
                SimpleTarget.NearestEnemyToTarget(SimpleTarget.Stack.MouseOver, Trajectory.ActionRange()) ?? //Nearest Enemy to Mouseover
                CurrentTarget.IfHostile().IfWithinRange(Trajectory.ActionRange());
            
            return target != null ? actionID.Retarget(target) : actionID;
        }
    }
    #endregion

    #region Basic Combos
    internal class GNB_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_ST_BasicCombo;

        protected override uint Invoke(uint actionID) 
            => actionID != SolidBarrel 
                ? actionID 
                : ComboTimer > 0 && ComboAction is KeenEdge && LevelChecked(BrutalShell) 
                    ? BrutalShell 
                : ComboTimer > 0 && ComboAction is BrutalShell && LevelChecked(SolidBarrel) 
                    ? SolidBarrel 
                : KeenEdge;
    }
    internal class GNB_AoE_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.GNB_AoE_BasicCombo;

        protected override uint Invoke(uint actionID) 
            => actionID is not DemonSlaughter
                ? actionID
                : ComboAction is DemonSlice && ComboTimer > 0 && LevelChecked(DemonSlaughter) 
                    ? DemonSlaughter 
                : DemonSlice;
    }
    #endregion
}
