using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal class NIN_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ST_AdvancedMode;

        protected internal MudraCasting mudraState = new();

        protected internal static NINOpenerMaxLevel4thGCDKunai NINOpener = new();

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningEdge)
                return actionID;

            NINGauge gauge = GetJobGauge<NINGauge>();
            bool canWeave = CanWeave();
            bool canDelayedWeave = CanDelayedWeave();
            bool inTrickBurstSaveWindow = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Cooldowns) && IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack) && GetCooldownRemainingTime(TrickAttack) <= GetOptionValue(Config.Advanced_Trick_Cooldown);
            bool useBhakaBeforeTrickWindow = GetCooldownRemainingTime(TrickAttack) >= 3;
            bool setupSuitonWindow = GetCooldownRemainingTime(OriginalHook(TrickAttack)) <= GetOptionValue(Config.Trick_CooldownRemaining) && !HasEffect(Buffs.ShadowWalker);
            bool setupKassatsuWindow = GetCooldownRemainingTime(TrickAttack) <= 10 && HasEffect(Buffs.ShadowWalker);
            bool chargeCheck = IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_ChargeHold) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_ChargeHold) && (InMudra || GetRemainingCharges(Ten) == 2 || (GetRemainingCharges(Ten) == 1 && GetCooldownChargeRemainingTime(Ten) < 3)));
            bool poolCharges = !GetOptionBool(Config.Advanced_ChargePool) || (GetRemainingCharges(Ten) == 1 && GetCooldownChargeRemainingTime(Ten) < 2) || TrickDebuff || InMudra;
            bool raitonUptime = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Raiton_Uptime);
            bool suitonUptime = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Suiton_Uptime);
            int bhavaPool = GetOptionValue(Config.Ninki_BhavaPooling);
            int bunshinPool = GetOptionValue(Config.Ninki_BunshinPoolingST);
            int burnKazematoi = GetOptionValue(Config.BurnKazematoi);
            int SecondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.SecondWindThresholdST);
            int ShadeShiftThreshold = PluginConfiguration.GetCustomIntValue(Config.ShadeShiftThresholdST);
            int BloodbathThreshold = PluginConfiguration.GetCustomIntValue(Config.BloodbathThresholdST);
            double playerHP = PlayerHealthPercentageHp();
            bool phantomUptime = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Phantom_Uptime);
            var comboLength = GetCooldown(GustSlash).CooldownTotal * 3;
            bool trueNorthArmor = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrueNorth) && TargetNeedsPositionals() && !OnTargetsFlank() && GetRemainingCharges(All.TrueNorth) > 0 && All.TrueNorth.LevelChecked() && !HasEffect(All.Buffs.TrueNorth) && canDelayedWeave;
            bool trueNorthEdge = IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrueNorth) && TargetNeedsPositionals() && Config.Advanced_TrueNorth == 0 && !OnTargetsRear() && GetRemainingCharges(All.TrueNorth) > 0 && All.TrueNorth.LevelChecked() && !HasEffect(All.Buffs.TrueNorth) && canDelayedWeave;
            bool dynamic = Config.Advanced_TrueNorth == 0;

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_BalanceOpener) && Opener().FullOpener(ref actionID))
                return actionID;

            if (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus) || (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat()))
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) && IsOnCooldown(TrickAttack) && mudraState.CurrentMudra == MudraCasting.MudraState.CastingSuiton && !setupSuitonWindow)
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) && IsOnCooldown(TrickAttack) && mudraState.CurrentMudra != MudraCasting.MudraState.CastingSuiton && setupSuitonWindow)
                mudraState.CurrentMudra = MudraCasting.MudraState.CastingSuiton;

            if (OriginalHook(Ninjutsu) is Rabbit)
                return OriginalHook(Ninjutsu);

            if (InMudra)
            {
                if (mudraState.ContinueCurrentMudra(ref actionID))
                    return actionID;
            }

            if (!Suiton.LevelChecked()) //For low level
            {
                if (Raiton.LevelChecked() && IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton)) //under 45 will only use Raiton
                {
                    if (mudraState.CastRaiton(ref actionID))
                        return actionID;
                }
                else if (!Raiton.LevelChecked() && mudraState.CastFumaShuriken(ref actionID) && IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_FumaShuriken)) // 30-35 will use only fuma
                    return actionID;
            }

            if (HasEffect(Buffs.TenChiJin))
            {
                if (OriginalHook(Ten) == TCJFumaShurikenTen)
                    return OriginalHook(Ten);
                if (OriginalHook(Chi) == TCJRaiton)
                    return OriginalHook(Chi);
                if (OriginalHook(Jin) == TCJSuiton)
                    return OriginalHook(Jin);
            }

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Kassatsu_HyoshoRaynryu) &&
                HasEffect(Buffs.Kassatsu) &&
                TrickDebuff &&
                mudraState.CastHyoshoRanryu(ref actionID))
                return actionID;

            if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                return Variant.VariantCure;

            if (InCombat() && !InMeleeRange())
            {
                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bunshin_Phantom) &&
                    HasEffect(Buffs.PhantomReady) &&
                    ((GetCooldownRemainingTime(TrickAttack) > GetBuffRemainingTime(Buffs.PhantomReady)) || TrickDebuff || (HasEffect(Buffs.Bunshin) && MugDebuff)) &&
                    PhantomKamaitachi.LevelChecked()
                    && phantomUptime)
                    return OriginalHook(PhantomKamaitachi);

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) &&
                    setupSuitonWindow &&
                    TrickAttack.LevelChecked() &&
                    !HasEffect(Buffs.ShadowWalker) &&
                    chargeCheck &&
                    suitonUptime &&
                    mudraState.CastSuiton(ref actionID))
                    return actionID;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton) &&
                    !inTrickBurstSaveWindow &&
                    chargeCheck &&
                    poolCharges &&
                    raitonUptime &&
                    mudraState.CastRaiton(ref actionID))
                    return actionID;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_RangedUptime) && ThrowingDaggers.LevelChecked() && HasTarget() && !HasEffect(Buffs.RaijuReady))
                    return OriginalHook(ThrowingDaggers);
            }

            if (canWeave && !InMudra)
            {
                if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) &&
                    IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug_AlignBefore) &&
                    HasEffect(Buffs.ShadowWalker) &&
                    GetCooldownRemainingTime(TrickAttack) <= 3 &&
                    ((IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed) && InCombat() &&
                      CombatEngageDuration().TotalSeconds > 6) ||
                     IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed)) &&
                    IsOffCooldown(Mug) &&
                    canDelayedWeave &&
                    Mug.LevelChecked())
                {
                    if (Dokumori.LevelChecked() && gauge.Ninki >= 60)
                        return OriginalHook(Bhavacakra);
                    return OriginalHook(Mug);
                }

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack) &&
                    HasEffect(Buffs.ShadowWalker) &&
                    IsOffCooldown(TrickAttack) &&
                    canDelayedWeave &&
                    ((IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed) && InCombat() && CombatEngageDuration().TotalSeconds > 8) ||
                     IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TrickAttack_Delayed)))
                    return OriginalHook(TrickAttack);

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TenriJindo) && HasEffect(Buffs.TenriJendo) && ((TrickDebuff && MugDebuff) || GetBuffRemainingTime(Buffs.TenriJendo) <= 3))
                    return OriginalHook(TenriJendo);

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bunshin) && Bunshin.LevelChecked() && IsOffCooldown(Bunshin) && gauge.Ninki >= bunshinPool)
                    return OriginalHook(Bunshin);

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Kassatsu) && (TrickDebuff || setupKassatsuWindow) && IsOffCooldown(Kassatsu) && Kassatsu.LevelChecked())
                    return OriginalHook(Kassatsu);

                //healing - please move if not appropriate priority
                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_SecondWind) && All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                    return All.SecondWind;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_ShadeShift) && ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                    return ShadeShift;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bloodbath) && All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                    return All.Bloodbath;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) &&
                    ((TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki >= 85)) &&
                    (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))) &&
                    Bhavacakra.LevelChecked())
                    return OriginalHook(Bhavacakra);

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) &&
                    ((TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki >= 60)) &&
                    (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))) &&
                    !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                    return OriginalHook(Hellfrog);

                if (!inTrickBurstSaveWindow)
                {
                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOffCooldown(Mug) && Mug.LevelChecked())
                    {
                        if (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug_AlignAfter) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug_AlignAfter) && TrickDebuff))
                            return OriginalHook(Mug);
                    }

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Meisui) && HasEffect(Buffs.ShadowWalker) && gauge.Ninki <= 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                        return OriginalHook(Meisui);

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) && gauge.Ninki >= bhavaPool && Bhavacakra.LevelChecked())
                        return OriginalHook(Bhavacakra);

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bhavacakra) && gauge.Ninki >= bhavaPool && !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                        return OriginalHook(Hellfrog);

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_AssassinateDWAD) && IsOffCooldown(OriginalHook(Assassinate)) && Assassinate.LevelChecked())
                        return OriginalHook(Assassinate);

                    if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_TCJ) && IsOffCooldown(TenChiJin) && TenChiJin.LevelChecked())
                        return OriginalHook(TenChiJin);
                }

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_SecondWind) && All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                    return All.SecondWind;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_ShadeShift) && ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                    return ShadeShift;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bloodbath) && All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                    return All.Bloodbath;
            }

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Raiju) && HasEffect(Buffs.RaijuReady))
            {
                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Raiju_Forked) && !InMeleeRange())
                    return OriginalHook(ForkedRaiju);
                return OriginalHook(FleetingRaiju);
            }

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Kassatsu_HyoshoRaynryu) &&
                !inTrickBurstSaveWindow &&
                (IsNotEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) || (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Mug) && IsOnCooldown(Mug))) &&
                mudraState.CastHyoshoRanryu(ref actionID))
                return actionID;

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus))
            {
                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Suiton) &&
                    setupSuitonWindow &&
                    TrickAttack.LevelChecked() &&
                    !HasEffect(Buffs.ShadowWalker) &&
                    chargeCheck &&
                    mudraState.CastSuiton(ref actionID))
                    return actionID;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_Raiton) &&
                    !inTrickBurstSaveWindow &&
                    chargeCheck &&
                    poolCharges &&
                    mudraState.CastRaiton(ref actionID))
                    return actionID;

                if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Ninjitsus_FumaShuriken) &&
                    !Raiton.LevelChecked() &&
                    chargeCheck &&
                    mudraState.CastFumaShuriken(ref actionID))
                    return actionID;
            }

            if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode_Bunshin_Phantom) &&
                HasEffect(Buffs.PhantomReady) &&
                ((GetCooldownRemainingTime(TrickAttack) > GetBuffRemainingTime(Buffs.PhantomReady)) || TrickDebuff || (HasEffect(Buffs.Bunshin) && MugDebuff) || GetBuffRemainingTime(Buffs.PhantomReady) < 6) &&
                PhantomKamaitachi.LevelChecked())
                return OriginalHook(PhantomKamaitachi);

            if (ComboTimer > 1f)
            {
                if (ComboAction == SpinningEdge && GustSlash.LevelChecked())
                    return OriginalHook(GustSlash);

                if (ComboAction == GustSlash && ArmorCrush.LevelChecked())
                {
                    if (gauge.Kazematoi == 0)
                    {
                        if (trueNorthArmor)
                            return All.TrueNorth;

                        return ArmorCrush;
                    }

                    if (GetTargetHPPercent() <= burnKazematoi && gauge.Kazematoi > 0)
                    {
                        if (trueNorthEdge)
                            return All.TrueNorth;

                        return AeolianEdge;
                    }

                    if (dynamic)
                    {
                        if (gauge.Kazematoi >= 4)
                        {
                            if (trueNorthEdge)
                                return All.TrueNorth;

                            return AeolianEdge;
                        }

                        if (OnTargetsFlank())
                            return ArmorCrush;
                        else
                            return AeolianEdge;

                    }
                    else
                    {
                        if (gauge.Kazematoi < 3)
                        {
                            if (trueNorthArmor)
                                return All.TrueNorth;

                            return ArmorCrush;
                        }

                        return AeolianEdge;
                    }
                }
                if (ComboAction == GustSlash && !ArmorCrush.LevelChecked() && AeolianEdge.LevelChecked())
                {
                    if (trueNorthEdge)
                        return OriginalHook(All.TrueNorth);
                    else
                        return OriginalHook(AeolianEdge);
                }
            }
            return OriginalHook(SpinningEdge);
        }
    }

    internal class NIN_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_AoE_AdvancedMode;

        protected internal MudraCasting mudraState = new();

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DeathBlossom)
                return actionID;

            Status? dotonBuff = FindEffect(Buffs.Doton);
            NINGauge? gauge = GetJobGauge<NINGauge>();
            bool canWeave = CanWeave();
            bool chargeCheck = IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_ChargeHold) || (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_ChargeHold) && GetRemainingCharges(Ten) == 2);
            bool inMudraState = InMudra;
            int hellfrogPool = GetOptionValue(Config.Ninki_HellfrogPooling);
            int dotonTimer = GetOptionValue(Config.Advanced_DotonTimer);
            int dotonThreshold = GetOptionValue(Config.Advanced_DotonHP);
            int tcjPath = GetOptionValue(Config.Advanced_TCJEnderAoE);
            int bunshingPool = GetOptionValue(Config.Ninki_BunshinPoolingAoE);
            int SecondWindThreshold = PluginConfiguration.GetCustomIntValue(Config.SecondWindThresholdAoE);
            int ShadeShiftThreshold = PluginConfiguration.GetCustomIntValue(Config.ShadeShiftThresholdAoE);
            int BloodbathThreshold = PluginConfiguration.GetCustomIntValue(Config.BloodbathThresholdAoE);
            double playerHP = PlayerHealthPercentageHp();

            if (IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus) || (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat()))
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (OriginalHook(Ninjutsu) is Rabbit)
                return OriginalHook(Ninjutsu);

            if (InMudra)
            {
                if (mudraState.ContinueCurrentMudra(ref actionID))
                    return actionID;
            }

            if (HasEffect(Buffs.TenChiJin))
            {
                if (tcjPath == 0)
                {
                    if (OriginalHook(Chi) == TCJFumaShurikenChi)
                        return OriginalHook(Chi);
                    if (OriginalHook(Ten) == TCJKaton)
                        return OriginalHook(Ten);
                    if (OriginalHook(Jin) == TCJSuiton)
                        return OriginalHook(Jin);
                }
                else
                {
                    if (OriginalHook(Jin) == TCJFumaShurikenJin)
                        return OriginalHook(Jin);
                    if (OriginalHook(Ten) == TCJKaton)
                        return OriginalHook(Ten);
                    if (OriginalHook(Chi) == TCJDoton)
                        return OriginalHook(Chi);
                }
            }

            if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_GokaMekkyaku) && HasEffect(Buffs.Kassatsu))
                mudraState.CurrentMudra = MudraCasting.MudraState.CastingGokaMekkyaku;

            if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_KunaisBane))
            {
                if (!HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && GetCooldownRemainingTime(KunaisBane) < 5 && mudraState.CastHuton(ref actionID))
                    return actionID;

                if (HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && IsOffCooldown(KunaisBane) && canWeave)
                    return KunaisBane;
            }

            if (canWeave && !inMudraState)
            {
                if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_TenriJindo) && HasEffect(Buffs.TenriJendo))
                    return OriginalHook(TenriJendo);

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin) && Bunshin.LevelChecked() && IsOffCooldown(Bunshin) && gauge.Ninki >= bunshingPool)
                    return OriginalHook(Bunshin);

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_HellfrogMedium) && gauge.Ninki >= hellfrogPool && Hellfrog.LevelChecked())
                {
                    if (HasEffect(Buffs.Meisui) && TraitLevelChecked(440))
                        return OriginalHook(Bhavacakra);

                    return OriginalHook(Hellfrog);
                }

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_HellfrogMedium) && gauge.Ninki >= hellfrogPool && !Hellfrog.LevelChecked() && Bhavacakra.LevelChecked())
                {
                    return OriginalHook(Bhavacakra);
                }

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Kassatsu) &&
                    IsOffCooldown(Kassatsu) &&
                    Kassatsu.LevelChecked() &&
                    ((IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) && (dotonBuff != null || GetTargetHPPercent() < dotonThreshold)) ||
                     IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton)))
                    return OriginalHook(Kassatsu);

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Meisui) && HasEffect(Buffs.ShadowWalker) && gauge.Ninki <= 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                    return OriginalHook(Meisui);

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_AssassinateDWAD) && IsOffCooldown(OriginalHook(Assassinate)) && Assassinate.LevelChecked())
                    return OriginalHook(Assassinate);

                // healing - please move if not appropriate priority
                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_SecondWind) && All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                    return All.SecondWind;

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_ShadeShift) && ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                    return ShadeShift;

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Bloodbath) && All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                    return All.Bloodbath;

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_TCJ) &&
                    IsOffCooldown(TenChiJin) &&
                    TenChiJin.LevelChecked())
                {
                    if ((IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) && tcjPath == 1 &&
                         (dotonBuff?.RemainingTime <= dotonTimer || dotonBuff is null) &&
                         GetTargetHPPercent() >= dotonThreshold &&
                         !WasLastAction(Doton)) ||
                        tcjPath == 0)
                        return OriginalHook(TenChiJin);
                }
            }

            if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_GokaMekkyaku) &&
                mudraState.CastGokaMekkyaku(ref actionID))
                return actionID;

            if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus))
            {
                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) &&
                    (dotonBuff?.RemainingTime <= dotonTimer || dotonBuff is null) &&
                    GetTargetHPPercent() >= dotonThreshold &&
                    chargeCheck &&
                    !(WasLastAction(Doton) || WasLastAction(TCJDoton) || dotonBuff is not null) &&
                    mudraState.CastDoton(ref actionID))
                    return actionID;

                if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Katon) &&
                    chargeCheck &&
                    ((IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton) && (dotonBuff != null || GetTargetHPPercent() < dotonThreshold)) ||
                     IsNotEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Ninjitsus_Doton)) &&
                    mudraState.CastKaton(ref actionID))
                    return actionID;
            }

            if (IsEnabled(CustomComboPreset.NIN_AoE_AdvancedMode_Bunshin_Phantom) && HasEffect(Buffs.PhantomReady) && PhantomKamaitachi.LevelChecked())
                return OriginalHook(PhantomKamaitachi);

            if (ComboTimer > 1f)
            {
                if (ComboAction is DeathBlossom && HakkeMujinsatsu.LevelChecked())
                    return OriginalHook(HakkeMujinsatsu);
            }

            return OriginalHook(DeathBlossom);
        }
    }

    internal class NIN_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ST_SimpleMode;

        protected internal MudraCasting mudraState = new();

        protected internal static NINOpenerMaxLevel4thGCDKunai NINOpener = new();

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningEdge)
                return actionID;

            NINGauge gauge = GetJobGauge<NINGauge>();
            bool canWeave = CanWeave();
            bool canDelayedWeave = CanDelayedWeave();
            bool inTrickBurstSaveWindow = GetCooldownRemainingTime(TrickAttack) <= 20;
            bool useBhakaBeforeTrickWindow = GetCooldownRemainingTime(TrickAttack) >= 3;
            bool setupSuitonWindow = GetCooldownRemainingTime(OriginalHook(TrickAttack)) <= 18 && !HasEffect(Buffs.ShadowWalker);
            bool setupKassatsuWindow = GetCooldownRemainingTime(TrickAttack) <= 10 && HasEffect(Buffs.ShadowWalker);
            bool poolCharges = (GetRemainingCharges(Ten) == 1 && GetCooldownChargeRemainingTime(Ten) < 2) || TrickDebuff || InMudra;
            bool raitonUptime = true;
            int bhavaPool = 50;
            int bunshinPool = 50;
            int SecondWindThreshold = 50;
            int ShadeShiftThreshold = 50;
            int BloodbathThreshold = 50;
            double playerHP = PlayerHealthPercentageHp();
            bool phantomUptime = true;
            _ = GetCooldown(GustSlash).CooldownTotal * 3;
            bool trueNorthArmor = TargetNeedsPositionals() && !OnTargetsFlank() && GetRemainingCharges(All.TrueNorth) > 0 && All.TrueNorth.LevelChecked() && !HasEffect(All.Buffs.TrueNorth) && canDelayedWeave;
            bool trueNorthEdge = TargetNeedsPositionals() && !OnTargetsRear() && GetRemainingCharges(All.TrueNorth) > 0 && All.TrueNorth.LevelChecked() && !HasEffect(All.Buffs.TrueNorth) && canDelayedWeave;
            bool dynamic = true;

            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat())
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (IsOnCooldown(TrickAttack) && mudraState.CurrentMudra == MudraCasting.MudraState.CastingSuiton && !setupSuitonWindow)
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (IsOnCooldown(TrickAttack) && mudraState.CurrentMudra != MudraCasting.MudraState.CastingSuiton && setupSuitonWindow)
                mudraState.CurrentMudra = MudraCasting.MudraState.CastingSuiton;

            if (OriginalHook(Ninjutsu) is Rabbit)
                return OriginalHook(Ninjutsu);

            if (InMudra)
            {
                if (mudraState.ContinueCurrentMudra(ref actionID))
                    return actionID;
            }

            if (IsOffCooldown(Mug) && Mug.LevelChecked())
            {
                if ((GetCooldown(TrickAttack).CooldownRemaining < 3 || TrickDebuff) &&
                    CombatEngageDuration().TotalSeconds > 5 &&
                    canDelayedWeave)
                    return OriginalHook(Mug);
            }

            if (HasEffect(Buffs.Kassatsu) &&
                TrickDebuff &&
                mudraState.CastHyoshoRanryu(ref actionID))
                return actionID;

            if (!Suiton.LevelChecked()) //For low level
            {
                if (Raiton.LevelChecked()) //under 45 will only use Raiton
                {
                    if (mudraState.CastRaiton(ref actionID))
                        return actionID;
                }
                else if (!Raiton.LevelChecked() && mudraState.CastFumaShuriken(ref actionID)) // 30-35 will use only fuma
                    return actionID;
            }

            if (HasEffect(Buffs.TenChiJin))
            {
                if (OriginalHook(Ten) == TCJFumaShurikenTen)
                    return OriginalHook(Ten);
                if (OriginalHook(Chi) == TCJRaiton)
                    return OriginalHook(Chi);
                if (OriginalHook(Jin) == TCJSuiton)
                    return OriginalHook(Jin);
            }

            if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                return Variant.VariantCure;

            if (InCombat() && !InMeleeRange())
            {
                if (HasEffect(Buffs.PhantomReady) &&
                    ((GetCooldownRemainingTime(TrickAttack) > GetBuffRemainingTime(Buffs.PhantomReady) && GetBuffRemainingTime(Buffs.PhantomReady) < 5) || TrickDebuff || (HasEffect(Buffs.Bunshin) && MugDebuff)) &&
                    PhantomKamaitachi.LevelChecked()
                    && phantomUptime)
                    return OriginalHook(PhantomKamaitachi);

                if (setupSuitonWindow &&
                    TrickAttack.LevelChecked() &&
                    !HasEffect(Buffs.ShadowWalker) &&
                    mudraState.CastSuiton(ref actionID))
                    return actionID;

                if (!inTrickBurstSaveWindow &&
                    poolCharges &&
                    raitonUptime &&
                    mudraState.CastRaiton(ref actionID))
                    return actionID;

                if (ThrowingDaggers.LevelChecked() && HasTarget() && !HasEffect(Buffs.RaijuReady))
                    return OriginalHook(ThrowingDaggers);
            }

            if (canWeave && !InMudra)
            {
                if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (HasEffect(Buffs.ShadowWalker) &&
                    IsOffCooldown(TrickAttack) &&
                    InCombat() && CombatEngageDuration().TotalSeconds > 8 &&
                    canDelayedWeave)
                    return OriginalHook(TrickAttack);

                if (HasEffect(Buffs.TenriJendo) && (TrickDebuff || GetBuffRemainingTime(Buffs.TenriJendo) <= 3))
                    return OriginalHook(TenriJendo);

                if (Bunshin.LevelChecked() && IsOffCooldown(Bunshin) && gauge.Ninki >= bunshinPool)
                    return OriginalHook(Bunshin);

                if ((TrickDebuff || setupKassatsuWindow) && IsOffCooldown(Kassatsu) && Kassatsu.LevelChecked())
                    return OriginalHook(Kassatsu);

                //healing - please move if not appropriate priority
                if (All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                    return All.SecondWind;

                if (ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                    return ShadeShift;

                if (All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                    return All.Bloodbath;

                if (((TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki >= 85)) &&
                    Bhavacakra.LevelChecked())
                    return OriginalHook(Bhavacakra);

                if (((TrickDebuff && gauge.Ninki >= 50) || (useBhakaBeforeTrickWindow && gauge.Ninki >= 60)) &&
                    !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                    return OriginalHook(Hellfrog);

                if (!inTrickBurstSaveWindow)
                {
                    if (HasEffect(Buffs.ShadowWalker) && gauge.Ninki <= 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                        return OriginalHook(Meisui);

                    if (gauge.Ninki >= bhavaPool && Bhavacakra.LevelChecked())
                        return OriginalHook(Bhavacakra);

                    if (gauge.Ninki >= bhavaPool && !Bhavacakra.LevelChecked() && Hellfrog.LevelChecked())
                        return OriginalHook(Hellfrog);

                    if (IsOffCooldown(OriginalHook(Assassinate)) && Assassinate.LevelChecked())
                        return OriginalHook(Assassinate);

                    if (IsOffCooldown(TenChiJin) && TenChiJin.LevelChecked())
                        return OriginalHook(TenChiJin);
                }

                if (All.SecondWind.LevelChecked() && playerHP <= SecondWindThreshold && IsOffCooldown(All.SecondWind))
                    return All.SecondWind;

                if (ShadeShift.LevelChecked() && playerHP <= ShadeShiftThreshold && IsOffCooldown(ShadeShift))
                    return ShadeShift;

                if (All.Bloodbath.LevelChecked() && playerHP <= BloodbathThreshold && IsOffCooldown(All.Bloodbath))
                    return All.Bloodbath;
            }

            if (HasEffect(Buffs.RaijuReady) && InMeleeRange())
            {
                return OriginalHook(FleetingRaiju);
            }

            if (!inTrickBurstSaveWindow &&
                IsOnCooldown(Mug) &&
                mudraState.CastHyoshoRanryu(ref actionID))
                return actionID;

            if (setupSuitonWindow &&
                TrickAttack.LevelChecked() &&
                !HasEffect(Buffs.ShadowWalker) &&
                mudraState.CastSuiton(ref actionID))
                return actionID;

            if (
                !inTrickBurstSaveWindow &&
                poolCharges &&
                mudraState.CastRaiton(ref actionID))
                return actionID;

            if (HasEffect(Buffs.PhantomReady) &&
                ((GetCooldownRemainingTime(TrickAttack) > GetBuffRemainingTime(Buffs.PhantomReady) && GetBuffRemainingTime(Buffs.PhantomReady) < 5) || TrickDebuff || (HasEffect(Buffs.Bunshin) && TargetHasEffect(Debuffs.Mug))) &&
                PhantomKamaitachi.LevelChecked())
                return OriginalHook(PhantomKamaitachi);

            if (!Raiton.LevelChecked() &&
                mudraState.CastFumaShuriken(ref actionID))
                return actionID;

            if (ComboTimer > 1f)
            {
                if (ComboAction == SpinningEdge && GustSlash.LevelChecked())
                    return OriginalHook(GustSlash);

                if (ComboAction == GustSlash && ArmorCrush.LevelChecked())
                {
                    if (gauge.Kazematoi == 0)
                    {
                        if (trueNorthArmor)
                            return All.TrueNorth;

                        return ArmorCrush;
                    }

                    if (dynamic)
                    {
                        if (gauge.Kazematoi >= 4)
                        {
                            if (trueNorthEdge)
                                return All.TrueNorth;

                            return AeolianEdge;
                        }

                        if (OnTargetsFlank())
                            return ArmorCrush;
                        else
                            return AeolianEdge;

                    }
                    else
                    {
                        if (gauge.Kazematoi < 3)
                        {
                            if (trueNorthArmor)
                                return All.TrueNorth;

                            return ArmorCrush;
                        }

                        return AeolianEdge;
                    }
                }
                if (ComboAction == GustSlash && !ArmorCrush.LevelChecked() && AeolianEdge.LevelChecked())
                {
                    if (trueNorthEdge)
                        return OriginalHook(All.TrueNorth);
                    else
                        return OriginalHook(AeolianEdge);
                }
            }
            return OriginalHook(SpinningEdge);
        }
    }

    internal class NIN_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_AoE_SimpleMode;

        private MudraCasting mudraState = new();

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DeathBlossom)
                return actionID;

            Status? dotonBuff = FindEffect(Buffs.Doton);
            NINGauge gauge = GetJobGauge<NINGauge>();
            bool canWeave = CanWeave();

            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat())
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (OriginalHook(Ninjutsu) is Rabbit)
                return OriginalHook(Ninjutsu);

            if (InMudra)
            {
                if (mudraState.ContinueCurrentMudra(ref actionID))
                    return actionID;
            }

            if (HasEffect(Buffs.TenChiJin))
            {
                if (WasLastAction(TCJFumaShurikenJin))
                    return OriginalHook(Ten);
                if (WasLastAction(TCJKaton) || WasLastAction(HollowNozuchi))
                    return OriginalHook(Chi);
                return OriginalHook(Jin);
            }

            if (HasEffect(Buffs.Kassatsu))
            {
                if (GokaMekkyaku.LevelChecked())
                {
                    mudraState.CurrentMudra = MudraCasting.MudraState.CastingGokaMekkyaku;
                    if (mudraState.CastGokaMekkyaku(ref actionID))
                        return actionID;
                }
                else
                {
                    mudraState.CurrentMudra = MudraCasting.MudraState.CastingKaton;
                    if (mudraState.CastKaton(ref actionID))
                        return actionID;
                }
            }

            if (IsEnabled(CustomComboPreset.NIN_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.NIN_VariantCure))
                return Variant.VariantCure;

            if (!HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && GetCooldownRemainingTime(KunaisBane) < 5 && mudraState.CastHuton(ref actionID))
                return actionID;

            if (HasEffect(Buffs.ShadowWalker) && KunaisBane.LevelChecked() && IsOffCooldown(KunaisBane) && canWeave)
                return KunaisBane;

            if (GetTargetHPPercent() > 20 && (dotonBuff is null || dotonBuff?.RemainingTime <= GetCooldownChargeRemainingTime(Ten)) && !JustUsed(Doton) && IsOnCooldown(TenChiJin))
            {
                if (mudraState.CastDoton(ref actionID))
                    return actionID;
            }
            else if (mudraState.CurrentMudra == MudraCasting.MudraState.CastingDoton)
                mudraState.CurrentMudra = MudraCasting.MudraState.None;

            if (mudraState.CastKaton(ref actionID))
                return actionID;

            if (canWeave && !InMudra)
            {
                if (IsEnabled(CustomComboPreset.NIN_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (IsOffCooldown(TenChiJin) && TenChiJin.LevelChecked())
                    return OriginalHook(TenChiJin);

                if (HasEffect(Buffs.TenriJendo))
                    return TenriJendo;

                if (IsOffCooldown(Bunshin) && gauge.Ninki >= 50 && Bunshin.LevelChecked())
                    return OriginalHook(Bunshin);

                if (HasEffect(Buffs.ShadowWalker) && gauge.Ninki < 50 && IsOffCooldown(Meisui) && Meisui.LevelChecked())
                    return OriginalHook(Meisui);

                if (HasEffect(Buffs.Meisui) && gauge.Ninki >= 50)
                    return OriginalHook(Bhavacakra);

                if (gauge.Ninki >= 50 && Hellfrog.LevelChecked())
                    return OriginalHook(Hellfrog);

                if (gauge.Ninki >= 50 && !Hellfrog.LevelChecked() && Bhavacakra.LevelChecked())
                    return OriginalHook(Bhavacakra);

                if (IsOffCooldown(Kassatsu) && Kassatsu.LevelChecked())
                    return OriginalHook(Kassatsu);
            }
            else
            {
                if (HasEffect(Buffs.PhantomReady))
                    return OriginalHook(PhantomKamaitachi);
            }

            if (ComboTimer > 1f)
            {
                if (ComboAction is DeathBlossom && HakkeMujinsatsu.LevelChecked())
                    return OriginalHook(HakkeMujinsatsu);
            }

            return OriginalHook(DeathBlossom);
        }
    }

    internal class NIN_ArmorCrushCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_ArmorCrushCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ArmorCrush)
                return actionID;
            if (ComboTimer > 0f)
            {
                if (ComboAction == SpinningEdge && GustSlash.LevelChecked())
                {
                    return GustSlash;
                }

                if (ComboAction == GustSlash && ArmorCrush.LevelChecked())
                {
                    return ArmorCrush;
                }
            }
            return SpinningEdge;
        }
    }

    internal class NIN_HideMug : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_HideMug;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Hide)
                return actionID;

            if (HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
            {
                return OriginalHook(Mug);
            }

            if (HasEffect(Buffs.Hidden))
            {
                return OriginalHook(TrickAttack);
            }

            return actionID;
        }
    }

    internal class NIN_KassatsuChiJin : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_KassatsuChiJin;

        protected override uint Invoke(uint actionID)
        {
            if (actionID == Chi && TraitLevelChecked(250) && HasEffect(Buffs.Kassatsu))
            {
                return Jin;
            }
            return actionID;
        }
    }

    internal class NIN_KassatsuTrick : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_KassatsuTrick;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Kassatsu)
                return actionID;
            if (HasEffect(Buffs.ShadowWalker) || HasEffect(Buffs.Hidden))
            {
                return OriginalHook(TrickAttack);
            }
            return OriginalHook(Kassatsu);
        }
    }

    internal class NIN_TCJMeisui : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.NIN_TCJMeisui;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TenChiJin)
                return actionID;

            if (HasEffect(Buffs.ShadowWalker))
                return Meisui;

            if (HasEffect(Buffs.TenChiJin) && IsEnabled(CustomComboPreset.NIN_TCJ))
            {
                float tcjTimer = FindEffectAny(Buffs.TenChiJin).RemainingTime;

                if (tcjTimer > 5)
                    return OriginalHook(Ten);

                if (tcjTimer > 4)
                    return OriginalHook(Chi);

                if (tcjTimer > 3)
                    return OriginalHook(Jin);
            }
            return actionID;
        }
    }

    internal class NIN_Simple_Mudras : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NIN_Simple_Mudras;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ten or Chi or Jin) || !HasEffect(Buffs.Mudra))
                return actionID;

            int mudrapath = GetOptionValue(Config.NIN_SimpleMudra_Choice);

            if (mudrapath == 1)
            {
                if (Ten.LevelChecked() && actionID == Ten)
                {
                    if (Jin.LevelChecked() && (OriginalHook(Ninjutsu) is Raiton))
                    {
                        return OriginalHook(JinCombo);
                    }

                    if (Chi.LevelChecked() && (OriginalHook(Ninjutsu) is HyoshoRanryu))
                    {
                        return OriginalHook(ChiCombo);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        if (HasEffect(Buffs.Kassatsu) && Traits.EnhancedKasatsu.TraitLevelChecked())
                            return JinCombo;

                        if (Chi.LevelChecked())
                            return OriginalHook(ChiCombo);

                        if (Jin.LevelChecked())
                            return OriginalHook(JinCombo);
                    }
                }

                if (Chi.LevelChecked() && actionID == Chi)
                {
                    if (OriginalHook(Ninjutsu) is Hyoton)
                    {
                        return OriginalHook(TenCombo);
                    }

                    if (Jin.LevelChecked() && OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        return OriginalHook(JinCombo);
                    }
                }

                if (Jin.LevelChecked() && actionID == Jin)
                {
                    if (OriginalHook(Ninjutsu) is GokaMekkyaku or Katon)
                    {
                        return OriginalHook(ChiCombo);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        return OriginalHook(TenCombo);
                    }
                }

                return OriginalHook(Ninjutsu);
            }

            if (mudrapath == 2)
            {
                if (Ten.LevelChecked() && actionID == Ten)
                {
                    if (Chi.LevelChecked() && (OriginalHook(Ninjutsu) is Hyoton or HyoshoRanryu))
                    {
                        return OriginalHook(Chi);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        if (Jin.LevelChecked())
                            return OriginalHook(JinCombo);

                        else if (Chi.LevelChecked())
                            return OriginalHook(ChiCombo);
                    }
                }

                if (Chi.LevelChecked() && actionID == Chi)
                {
                    if (Jin.LevelChecked() && (OriginalHook(Ninjutsu) is Katon or GokaMekkyaku))
                    {
                        return OriginalHook(Jin);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        return OriginalHook(Ten);
                    }
                }

                if (Jin.LevelChecked() && actionID == Jin)
                {
                    if (OriginalHook(Ninjutsu) is Raiton)
                    {
                        return OriginalHook(Ten);
                    }

                    if (OriginalHook(Ninjutsu) == GokaMekkyaku)
                    {
                        return OriginalHook(Chi);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        if (HasEffect(Buffs.Kassatsu) && Traits.EnhancedKasatsu.TraitLevelChecked())
                            return OriginalHook(Ten);
                        return OriginalHook(Chi);
                    }
                }

                return OriginalHook(Ninjutsu);
            }

            return actionID;
        }
    }
}
