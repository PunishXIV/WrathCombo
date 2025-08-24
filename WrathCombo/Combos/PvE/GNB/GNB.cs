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
            #region Stuns
            if (Role.CanInterject())
                return Role.Interject;
            if (!TargetIsBoss()
                && Role.CanLowBlow()
                && !JustUsed(Role.Interject)
                && !InBossEncounter())
                return Role.LowBlow;
            #endregion

            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;


            #region Mitigations
            if (GNB_ST_MitsOptions != 1)
            {
                if (InCombat() && !MitUsed)
                {
                    if (ActionReady(Superbolide) && HPP < 30)
                        return Superbolide;
                    if (IsPlayerTargeted())
                    {
                        if (ActionReady(OriginalHook(Nebula)) && HPP < 60)
                            return OriginalHook(Nebula);
                        if (ActionReady(Role.Rampart) && HPP < 80)
                            return Role.Rampart;
                        if (Role.CanReprisal(90))
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
            //Priority hack for increasing Continuation priority when inside late weave window
            if (CanDelayedWeave())
            {
                if (ShouldUseContinuation)
                    return OriginalHook(Continuation);
            }
            if (ShouldUseLightningShot)
                return LightningShot;
            if (ShouldUseBloodfest)
                return Bloodfest;
            if (ShouldUseNoMercy)
                return NoMercy;
            if (JustUsed(BurstStrike, 5f) && LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast))
            {
                if (NMcd is > 1.5f || //hold if No Mercy is imminent
                    CanDelayedWeave(0.6f, 0f)) //send asap if about to lose due to GCD
                    return Hypervelocity;
            }
            //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
            //without SKS, we don't really care since both usually remain static
            if (SlowGNB ? ShouldUseBowShock : ShouldUseZone)
                return SlowGNB ? BowShock : OriginalHook(DangerZone);
            if (SlowGNB ? ShouldUseZone : ShouldUseBowShock)
                return SlowGNB ? OriginalHook(DangerZone) : BowShock;
            if (ShouldUseContinuation &&
                (CanWeave() || //normal
                CanDelayedWeave(0.6f, 0f))) //send asap if about to lose due to GCD
                return OriginalHook(Continuation);
            if (LevelChecked(DoubleDown) && JustUsed(NoMercy, 5f) && GunStep == 0 && ComboAction is BrutalShell && Ammo == 1)
                return SolidBarrel;
            if (ShouldUseGnashingFang)
                return GnashingFang;
            if (ShouldUseDoubleDown)
                return DoubleDown;
            if (ShouldUseSonicBreak)
                return SonicBreak;
            if (ShouldUseReignOfBeasts)
                return ReignOfBeasts;
            if (ShouldUseBurstStrike ||
                LevelChecked(DoubleDown) &&
                NMcd < 1 && Ammo == 3 && !InOdd)
                return BurstStrike;
            if (GunStep is 1 or 2)
                return OriginalHook(GnashingFang);
            if (GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);
            if (ComboTimer > 0)
            {
                if (LevelChecked(BrutalShell) && ComboAction == KeenEdge)
                    return BrutalShell;
                if (LevelChecked(SolidBarrel) && ComboAction == BrutalShell)
                {
                    if (Ammo == MaxCartridges() && LevelChecked(BurstStrike))
                        return BurstStrike;
                    return SolidBarrel;
                }
            }
            return STCombo;
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
            #region Stuns
            if (IsEnabled(Preset.GNB_ST_Interrupt) && Role.CanInterject())
                return Role.Interject;
            if (IsEnabled(Preset.GNB_ST_Stun)
                && !TargetIsBoss()
                && Role.CanLowBlow()
                && !JustUsed(Role.Interject)
                && !InBossEncounter())
                return Role.LowBlow;
            #endregion

            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            if (IsEnabled(Preset.GNB_ST_Mitigation) && InCombat() && !MitUsed)
            {
                if (IsEnabled(Preset.GNB_ST_Superbolide) && ActionReady(Superbolide) && HPP < GNB_ST_Superbolide_Health &&
                    (GNB_ST_Superbolide_SubOption == 0 || TargetIsBoss() && GNB_ST_Superbolide_SubOption == 1))
                    return Superbolide;
                if (IsPlayerTargeted())
                {
                    if (IsEnabled(Preset.GNB_ST_Nebula) && ActionReady(OriginalHook(Nebula)) && HPP < GNB_ST_Nebula_Health &&
                        (GNB_ST_Nebula_SubOption == 0 || TargetIsBoss() && GNB_ST_Nebula_SubOption == 1))
                        return OriginalHook(Nebula);
                    if (IsEnabled(Preset.GNB_ST_Rampart) && Role.CanRampart(GNB_ST_Rampart_Health) &&
                        (GNB_ST_Rampart_SubOption == 0 || TargetIsBoss() && GNB_ST_Rampart_SubOption == 1))
                        return Role.Rampart;
                    if (IsEnabled(Preset.GNB_ST_Reprisal) && Role.CanReprisal(GNB_ST_Reprisal_Health) &&
                        (GNB_ST_Reprisal_SubOption == 0 || TargetIsBoss() && GNB_ST_Reprisal_SubOption == 1))
                        return Role.Reprisal;
                    if (IsEnabled(Preset.GNB_ST_ArmsLength) &&
                        HPP < GNB_AoE_ArmsLength_Health &&
                        Role.CanArmsLength())
                        return Role.ArmsLength;
                }
                if (IsEnabled(Preset.GNB_ST_Camouflage) && ActionReady(Camouflage) && HPP < GNB_ST_Camouflage_Health &&
                    (GNB_ST_Camouflage_SubOption == 0 || TargetIsBoss() && GNB_ST_Camouflage_SubOption == 1))
                    return Camouflage;
                if (IsEnabled(Preset.GNB_ST_Corundum) && ActionReady(OriginalHook(HeartOfStone)) && HPP < GNB_ST_Corundum_Health &&
                    (GNB_ST_Corundum_SubOption == 0 || TargetIsBoss() && GNB_ST_Corundum_SubOption == 1))
                    return OriginalHook(HeartOfStone);
                if (IsEnabled(Preset.GNB_ST_Aurora) && ActionReady(Aurora) && !(HasStatusEffect(Buffs.Aurora) || HasStatusEffect(Buffs.Aurora, CurrentTarget, true)) && GetRemainingCharges(Aurora) > GNB_ST_Aurora_Charges && HPP < GNB_ST_Aurora_Health &&
                    (GNB_ST_Aurora_SubOption == 0 || TargetIsBoss() && GNB_ST_Aurora_SubOption == 1))
                    return Aurora;
            }

            #endregion

            #endregion

            #region Rotation
            if (IsEnabled(Preset.GNB_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;
            
            //Priority hack for ensuring Continuation is used on late weave at the very latest
            if (CanDelayedWeave())
            {
                if (IsEnabled(Preset.GNB_ST_Continuation) &&
                    ShouldUseContinuation)
                    return OriginalHook(Continuation);
            }
            if (IsEnabled(Preset.GNB_ST_RangedUptime) && ShouldUseLightningShot)
                return LightningShot;
            if (IsEnabled(Preset.GNB_ST_Advanced))
            {
                if (IsEnabled(Preset.GNB_ST_Bloodfest) && ShouldUseBloodfest)
                    return Bloodfest;
                if (IsEnabled(Preset.GNB_ST_NoMercy) && ShouldUseNoMercy && GetTargetHPPercent() > STStopNM &&
                    (GNB_ST_NoMercy_SubOption == 0 || GNB_ST_NoMercy_SubOption == 1 && InBossEncounter()))
                    return NoMercy;
                if (IsEnabled(Preset.GNB_ST_Continuation) && IsEnabled(Preset.GNB_ST_NoMercy) &&
                    JustUsed(BurstStrike, 5f) && LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast))
                {
                    if (NMcd is > 1.5f || //hold if No Mercy is imminent
                        CanDelayedWeave(0.6f, 0f)) //send asap if about to lose due to GCD
                        return Hypervelocity;
                }
            }
            if (IsEnabled(Preset.GNB_ST_Scuffed) &&
                LevelChecked(DoubleDown) && JustUsed(NoMercy, 5f) && GunStep == 0 && ComboAction is BrutalShell && Ammo == 1)
                return SolidBarrel;
            if (IsEnabled(Preset.GNB_ST_Advanced))
            {
                //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
                //without SKS, we don't really care since both usually remain static
                if (SlowGNB ? IsEnabled(Preset.GNB_ST_BowShock) && ShouldUseBowShock : IsEnabled(Preset.GNB_ST_Zone) && ShouldUseZone)
                    return SlowGNB ? BowShock : OriginalHook(DangerZone);
                if (SlowGNB ? IsEnabled(Preset.GNB_ST_Zone) && ShouldUseZone : IsEnabled(Preset.GNB_ST_BowShock) && ShouldUseBowShock)
                    return SlowGNB ? OriginalHook(DangerZone) : BowShock;
                if (IsEnabled(Preset.GNB_ST_Continuation) && ShouldUseContinuation &&
                    (CanWeave() || //normal
                    CanDelayedWeave(0.6f, 0f))) //send asap if about to lose due to GCD
                    return OriginalHook(Continuation);
                if (IsEnabled(Preset.GNB_ST_GnashingFang) && ShouldUseGnashingFang)
                    return GnashingFang;
                if (IsEnabled(Preset.GNB_ST_DoubleDown) && ShouldUseDoubleDown)
                    return DoubleDown;
                if (IsEnabled(Preset.GNB_ST_SonicBreak) && ShouldUseSonicBreak)
                    return SonicBreak;
                if (IsEnabled(Preset.GNB_ST_Reign) && ShouldUseReignOfBeasts)
                    return OriginalHook(ReignOfBeasts);
                if (IsEnabled(Preset.GNB_ST_BurstStrike))
                {
                    if (ShouldUseBurstStrike ||
                        IsEnabled(Preset.GNB_ST_NoMercy) &&
                        LevelChecked(DoubleDown) && NMcd < 1 && Ammo == 3 && !InOdd)
                        return BurstStrike;
                }
            }
            if (IsEnabled(Preset.GNB_ST_GnashingFang) && GunStep is 1 or 2)
                return OriginalHook(GnashingFang);
            if (IsEnabled(Preset.GNB_ST_Reign) && GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);
            return STCombo;
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
            if (Role.CanLowBlow() && !JustUsed(Role.Interject))
                return Role.LowBlow;
            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            if (GNB_AoE_MitsOptions != 1)
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
                    if (ActionReady(NoMercy) && GetTargetHPPercent() > 10)
                        return NoMercy;
                    if (LevelChecked(FatedBrand) && HasStatusEffect(Buffs.ReadyToRaze))
                        return FatedBrand;
                }
                if (ShouldUseBowShock)
                    return BowShock;
                if (ShouldUseZone)
                    return OriginalHook(DangerZone);
                if (ShouldUseBloodfest)
                    return Bloodfest;
                if (CanSB && HasNM && !HasStatusEffect(Buffs.ReadyToRaze))
                    return SonicBreak;
                if (CanDD && HasNM)
                    return DoubleDown;
                if (CanReign || GunStep is 3 or 4)
                    return OriginalHook(ReignOfBeasts);
                if (CanBS && ((HasNM && (IsOnCooldown(DoubleDown) || !LevelChecked(DoubleDown)) && GunStep == 0) || BFcd < 6 || (ComboAction == DemonSlice && Ammo == MaxCartridges())))
                    return LevelChecked(FatedCircle) ? FatedCircle : BurstStrike;
            }
            return AOECombo;
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
            if (IsEnabled(Preset.GNB_AoE_Stun) && Role.CanLowBlow() && !JustUsed(Role.Interject))
                return Role.LowBlow;
            if (ShouldUseOther)
                return OtherAction;
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Mitigations
            if (IsEnabled(Preset.GNB_AoE_Mitigation) && InCombat() && !MitUsed)
            {
                if (IsEnabled(Preset.GNB_AoE_Superbolide) && ActionReady(Superbolide) && HPP < GNB_AoE_Superbolide_Health &&
                    (GNB_AoE_Superbolide_SubOption == 0 || TargetIsBoss() && GNB_AoE_Superbolide_SubOption == 1))
                    return Superbolide;
                if (IsPlayerTargeted())
                {
                    if (IsEnabled(Preset.GNB_AoE_Nebula) && ActionReady(OriginalHook(Nebula)) && HPP < GNB_AoE_Nebula_Health &&
                        (GNB_AoE_Nebula_SubOption == 0 || TargetIsBoss() && GNB_AoE_Nebula_SubOption == 1))
                        return OriginalHook(Nebula);
                    if (IsEnabled(Preset.GNB_AoE_Rampart) && Role.CanRampart(GNB_AoE_Rampart_Health) &&
                        (GNB_AoE_Rampart_SubOption == 0 || TargetIsBoss() && GNB_AoE_Rampart_SubOption == 1))
                        return Role.Rampart;
                    if (IsEnabled(Preset.GNB_AoE_Reprisal) && Role.CanReprisal(GNB_AoE_Reprisal_Health, checkTargetForDebuff: false) &&
                        (GNB_AoE_Reprisal_SubOption == 0 || TargetIsBoss() && GNB_AoE_Reprisal_SubOption == 1))
                        return Role.Reprisal;
                    if (IsEnabled(Preset.GNB_AoE_ArmsLength) &&
                        HPP < GNB_AoE_ArmsLength_Health &&
                        Role.CanArmsLength())
                        return Role.ArmsLength;
                }

                if (IsEnabled(Preset.GNB_AoE_Camouflage) && ActionReady(Camouflage) && HPP < GNB_AoE_Camouflage_Health &&
                    (GNB_AoE_Camouflage_SubOption == 0 || TargetIsBoss() && GNB_AoE_Camouflage_SubOption == 1))
                    return Camouflage;
                if (IsEnabled(Preset.GNB_AoE_Corundum) && ActionReady(OriginalHook(HeartOfStone)) && HPP < GNB_AoE_Corundum_Health &&
                    (GNB_AoE_Corundum_SubOption == 0 || TargetIsBoss() && GNB_AoE_Corundum_SubOption == 1))
                    return OriginalHook(HeartOfStone);
                if (IsEnabled(Preset.GNB_AoE_Aurora) && ActionReady(Aurora) && GetRemainingCharges(Aurora) > GNB_AoE_Aurora_Charges &&
                    !(HasStatusEffect(Buffs.Aurora) || HasStatusEffect(Buffs.Aurora, CurrentTarget, true)) && HPP < GNB_AoE_Aurora_Health &&
                    (GNB_AoE_Aurora_SubOption == 0 || TargetIsBoss() && GNB_AoE_Aurora_SubOption == 1))
                    return Aurora;
            }

            #endregion

            #endregion

            #region Rotation
            if (InCombat())
            {
                if (CanWeave())
                {
                    if (IsEnabled(Preset.GNB_AoE_NoMercy) && ShouldUseNoMercy && GetTargetHPPercent() > AoEStopNM)
                        return NoMercy;
                    if (IsEnabled(Preset.GNB_AoE_BowShock) && ShouldUseBowShock)
                        return BowShock;
                    if (IsEnabled(Preset.GNB_AoE_Zone) && ShouldUseZone)
                        return OriginalHook(DangerZone);
                    if (IsEnabled(Preset.GNB_AoE_Bloodfest) && ShouldUseBloodfest)
                        return Bloodfest;
                    if (LevelChecked(FatedBrand) && HasStatusEffect(Buffs.ReadyToRaze))
                        return FatedBrand;
                }
                if (IsEnabled(Preset.GNB_AoE_SonicBreak) && CanSB && HasNM && !HasStatusEffect(Buffs.ReadyToRaze))
                    return SonicBreak;
                if (IsEnabled(Preset.GNB_AoE_DoubleDown) && CanDD && HasNM)
                    return DoubleDown;
                if (IsEnabled(Preset.GNB_AoE_Reign) && (CanReign || GunStep is 3 or 4))
                    return OriginalHook(ReignOfBeasts);
                if (IsEnabled(Preset.GNB_AoE_FatedCircle) && CanBS)
                {
                    if ((HasNM && (IsOnCooldown(DoubleDown) || !LevelChecked(DoubleDown) || !IsEnabled(Preset.GNB_AoE_DoubleDown)) && GunStep == 0) || //burst
                        (LevelChecked(Bloodfest) && IsEnabled(Preset.GNB_AoE_Bloodfest) && BFcd < 6) || //Bloodfest prep
                        (GNB_AoE_Overcap_Choice == 0 && ComboAction == DemonSlice && Ammo == MaxCartridges()))
                        return LevelChecked(FatedCircle) ? FatedCircle : GNB_AoE_FatedCircle_BurstStrike == 0 ? BurstStrike : ComboAction == DemonSlice ? DemonSlaughter : DemonSlice;
                }
            }
            return AOECombo;
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
            bool GFchoice = GNB_GF_Features_Choice == 0; //Gnashing Fang as button
            bool NMchoice = GNB_GF_Features_Choice == 1; //No Mercy as button
            if ((GFchoice && actionID != GnashingFang) || (NMchoice && actionID != NoMercy))
                return actionID;
            if (IsEnabled(Preset.GNB_GF_Features))
            {
                //Priority hack for ensuring Continuation is used on late weave at the very latest
                if (CanDelayedWeave())
                {
                    if (IsEnabled(Preset.GNB_GF_Continuation) &&
                        ShouldUseContinuation)
                        return OriginalHook(Continuation);
                }
                if (IsEnabled(Preset.GNB_GF_Bloodfest) && ShouldUseBloodfest)
                    return Bloodfest;
                if (IsEnabled(Preset.GNB_GF_NoMercy) && ShouldUseNoMercy)
                    return NoMercy;
                if (IsEnabled(Preset.GNB_GF_Continuation) && JustUsed(BurstStrike, 5f) && LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast))
                {
                    if (NMcd is > 1.5f || //hold if No Mercy is imminent
                        CanDelayedWeave(0.6f, 0f)) //send asap if about to lose due to GCD
                        return Hypervelocity;
                }
                if (IsEnabled(Preset.GNB_GF_Continuation) && ShouldUseContinuation &&
                    (CanWeave() || //normal
                    CanDelayedWeave(0.6f, 0f))) //send asap if about to lose due to GCD
                    return OriginalHook(Continuation);
                //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
                //without SKS, we don't really care since both usually remain static
                if (SlowGNB ? IsEnabled(Preset.GNB_GF_BowShock) && ShouldUseBowShock : IsEnabled(Preset.GNB_GF_Zone) && ShouldUseZone)
                    return SlowGNB ? BowShock : OriginalHook(DangerZone);
                if (SlowGNB ? IsEnabled(Preset.GNB_GF_Zone) && ShouldUseZone : IsEnabled(Preset.GNB_GF_BowShock) && ShouldUseBowShock)
                    return SlowGNB ? OriginalHook(DangerZone) : BowShock;
                if (ShouldUseGnashingFang)
                    return GnashingFang;
                if (IsEnabled(Preset.GNB_GF_DoubleDown) && ShouldUseDoubleDown)
                    return DoubleDown;
                if (IsEnabled(Preset.GNB_GF_SonicBreak) && ShouldUseSonicBreak)
                    return SonicBreak;
                if (IsEnabled(Preset.GNB_GF_Reign) && ShouldUseReignOfBeasts)
                    return OriginalHook(ReignOfBeasts);
                if (IsEnabled(Preset.GNB_GF_Features) &&
                    IsEnabled(Preset.GNB_GF_BurstStrike))
                {
                    if (ShouldUseBurstStrike ||
                        IsEnabled(Preset.GNB_GF_NoMercy) &&
                        LevelChecked(DoubleDown) && NMcd < 1 && Ammo == 3 && !InOdd)
                        return BurstStrike;
                }
                if (IsEnabled(Preset.GNB_GF_Features) && GunStep is 1 or 2)
                    return OriginalHook(GnashingFang);
                if (IsEnabled(Preset.GNB_GF_Reign) && GunStep is 3 or 4)
                    return OriginalHook(ReignOfBeasts);
            }
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
            var useDD = IsEnabled(Preset.GNB_BS_DoubleDown) && CanDD;
            if (actionID != BurstStrike)
                return actionID;
            if (IsEnabled(Preset.GNB_BS_Continuation))
            {
                if (IsEnabled(Preset.GNB_BS_Hypervelocity) && LevelChecked(Hypervelocity) && (JustUsed(BurstStrike, 1) || HasStatusEffect(Buffs.ReadyToBlast)))
                    return Hypervelocity;
                if (!IsEnabled(Preset.GNB_BS_Hypervelocity) && CanContinue && (HasStatusEffect(Buffs.ReadyToRip) || HasStatusEffect(Buffs.ReadyToTear) || HasStatusEffect(Buffs.ReadyToGouge) || LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast)))
                    return OriginalHook(Continuation);
            }
            if (IsEnabled(Preset.GNB_BS_Bloodfest) && ShouldUseBloodfest)
                return Bloodfest;
            if (useDD && Ammo == 1)
                return DoubleDown;
            if (IsEnabled(Preset.GNB_BS_GnashingFang) && (CanGF || GunStep is 1 or 2))
                return OriginalHook(GnashingFang);
            if (useDD && Ammo > 1)
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
            if (IsEnabled(Preset.GNB_FC_Continuation) && HasStatusEffect(Buffs.ReadyToRaze) && LevelChecked(FatedBrand))
                return FatedBrand;
            if (IsEnabled(Preset.GNB_FC_DoubleDown) && IsEnabled(Preset.GNB_FC_DoubleDown_NM) && CanDD && HasNM)
                return DoubleDown;
            if (IsEnabled(Preset.GNB_FC_Bloodfest) && ShouldUseBloodfest)
                return Bloodfest;
            if (IsEnabled(Preset.GNB_FC_BowShock) && CanBow)
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
                var useZone = IsEnabled(Preset.GNB_NM_Zone) && CanZone && NMcd is < 57.5f and > 17f;
                var useBow = IsEnabled(Preset.GNB_NM_BowShock) && CanBow && NMcd is < 57.5f and >= 40;
                if (IsEnabled(Preset.GNB_NM_Continuation) && CanContinue && 
                    (HasStatusEffect(Buffs.ReadyToRip) || HasStatusEffect(Buffs.ReadyToTear) || HasStatusEffect(Buffs.ReadyToGouge) || (LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast) || (LevelChecked(FatedBrand) && HasStatusEffect(Buffs.ReadyToRaze)))))
                    return OriginalHook(Continuation);
                if (IsEnabled(Preset.GNB_NM_Bloodfest) && HasBattleTarget() && CanBF && Ammo == 0)
                    return Bloodfest;
                //with SKS, we want Zone first because it can drift really bad while Bow usually remains static
                //without SKS, we don't really care since both usually remain static
                if (SlowGNB ? useBow : useZone)
                    return SlowGNB ? BowShock : OriginalHook(DangerZone);
                if (SlowGNB ? useZone : useBow)
                    return SlowGNB ? OriginalHook(DangerZone) : BowShock;
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
            foreach(int priority in GNB_Mit_Priorities.Items.OrderBy(x => x))
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
        protected override uint Invoke(uint action) => action != HeartOfLight ? action : ActionReady(Role.Reprisal) ? Role.Reprisal : action;
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
