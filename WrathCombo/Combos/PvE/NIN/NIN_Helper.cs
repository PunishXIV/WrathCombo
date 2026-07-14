using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.NIN.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    static NINGauge gauge = GetJobGauge<NINGauge>();
    public static FrozenSet<uint> MudraSigns = [Ten, Chi, Jin, TenCombo, ChiCombo, JinCombo];
    public static FrozenSet<uint> NormalJutsus = [FumaShuriken, Raiton, Katon, Doton, Suiton, Hyoton, HyoshoRanryu, GokaMekkyaku, Rabbit];
    internal static bool STSimpleMode => IsEnabled(Preset.NIN_ST_SimpleMode);
    internal static bool AoESimpleMode => IsEnabled(Preset.NIN_AoE_SimpleMode);
    internal static bool BlockDueToLag
    {
        get
        {
            return ActionWatching.LastAction != LastMudra && MudraSigns.Any(x => x == ActionWatching.LastAction);
        }
    }

    #region Mudra Logic
    public enum MudraFlags
    {
        None = 0,
        TenFirst = 1,
        ChiFirst = 2,
        JinFirst = 3,
        TenSecond = 4,
        ChiSecond = 8,
        JinSecond = 12,
        TenThird = 16,
        ChiThird = 32,
        JinThird = 48,
        Rabbit = 255,
    }

    public static uint CurrentNinjutsu => OriginalHook(Ninjutsu);
    internal static bool InMudra => JutsuFromFlags > 0;

    internal static MudraFlags Flags => HasStatusEffect(Buffs.Mudra) ? (MudraFlags)(GetStatusEffect(Buffs.Mudra).Param) : HasStatusEffect(Buffs.TenChiJin) ? (MudraFlags)(GetStatusEffect(Buffs.TenChiJin).Param) : MudraFlags.None;
    internal static MudraFlags FirstMudra => Flags & MudraFlags.JinFirst;
    internal static MudraFlags SecondMudra => Flags & MudraFlags.JinSecond;
    internal static MudraFlags ThirdMudra => Flags & MudraFlags.JinThird;

    internal static uint LastMudra
    {
        get
        {
            int raw = (int)Flags;

            return ThirdMudra switch
            {
                not MudraFlags.None => ThirdMudra switch
                {
                    MudraFlags.TenThird => TenCombo,
                    MudraFlags.ChiThird => ChiCombo,
                    MudraFlags.JinThird => JinCombo,
                    _ => 0
                },

                _ => SecondMudra switch
                {
                    not MudraFlags.None => SecondMudra switch
                    {
                        MudraFlags.TenSecond => TenCombo,
                        MudraFlags.ChiSecond => ChiCombo,
                        MudraFlags.JinSecond => JinCombo,
                        _ => 0
                    },

                    _ => FirstMudra switch
                    {
                        MudraFlags.TenFirst => HasKassatsu ? TenCombo : Ten,
                        MudraFlags.ChiFirst => HasKassatsu ? ChiCombo : Chi,
                        MudraFlags.JinFirst => HasKassatsu ? JinCombo : Jin,
                        _ => 0
                    }
                }
            };
        }
    }

    internal static uint JutsuFromFlags
    {
        get
        {
            if (FailedJutsu || Flags == MudraFlags.Rabbit)
                return Rabbit;

            return ThirdMudra switch
            {
                not MudraFlags.None => ThirdMudra switch
                {
                    MudraFlags.TenThird => Huton,
                    MudraFlags.ChiThird => Doton,
                    MudraFlags.JinThird => Suiton,
                    _ => 0
                },

                _ => SecondMudra switch
                {
                    not MudraFlags.None => SecondMudra switch
                    {
                        MudraFlags.TenSecond => HasKassatsu ? GokaMekkyaku : Katon,
                        MudraFlags.ChiSecond => Raiton,
                        MudraFlags.JinSecond => HasKassatsu ? HyoshoRanryu : Hyoton,
                        _ => 0
                    },

                    _ => FirstMudra switch
                    {
                        MudraFlags.TenFirst => FumaShuriken,
                        MudraFlags.ChiFirst => FumaShuriken,
                        MudraFlags.JinFirst => FumaShuriken,
                        _ => 0
                    }
                }
            };
        }
    }

    internal static bool FailedJutsu =>
        (
            (FirstMudra == MudraFlags.TenFirst ? 1 : 0) +
            (SecondMudra == MudraFlags.TenSecond ? 1 : 0) +
            (ThirdMudra == MudraFlags.TenThird ? 1 : 0)
        ) > 1
        ||
        (
            (FirstMudra == MudraFlags.ChiFirst ? 1 : 0) +
            (SecondMudra == MudraFlags.ChiSecond ? 1 : 0) +
            (ThirdMudra == MudraFlags.ChiThird ? 1 : 0)
        ) > 1
        ||
        (
            (FirstMudra == MudraFlags.JinFirst ? 1 : 0) +
            (SecondMudra == MudraFlags.JinSecond ? 1 : 0) +
            (ThirdMudra == MudraFlags.JinThird ? 1 : 0)
        ) > 1;

    internal static List<uint> UnusedJutsus
    {
        get
        {
            List<uint> output = [];
            if (FirstMudra != MudraFlags.TenFirst && SecondMudra != MudraFlags.TenSecond && ThirdMudra != MudraFlags.TenThird)
                output.Add(Ten);
            if (FirstMudra != MudraFlags.ChiFirst && SecondMudra != MudraFlags.ChiSecond && ThirdMudra != MudraFlags.ChiThird)
                output.Add(Chi);
            if (FirstMudra != MudraFlags.JinFirst && SecondMudra != MudraFlags.JinSecond && ThirdMudra != MudraFlags.JinThird)
                output.Add(Jin);

            return output;
        }
    }

    internal static bool Rabbitting => GetStatusEffect(Buffs.Mudra)?.Param == 255;
    internal static bool MudraPhase => WasLastAction(Ten) || WasLastAction(Chi) || WasLastAction(Jin) || WasLastAction(TenCombo) || WasLastAction(ChiCombo) || WasLastAction(JinCombo);
    internal static uint MudraCharges => GetRemainingCharges(Ten);
    internal static bool MudraAlmostReady => MudraCharges == 1 && GetCooldownChargeRemainingTime(Ten) < 3;
    #endregion

    #region Ninjutsu Logic
    internal static bool HasDoton => HasStatusEffect(Buffs.Doton);
    internal static float DotonRemaining => GetStatusEffectRemainingTime(Buffs.Doton);
    internal static bool DotonStoppedMoving => TimeStoodStill >= TimeSpan.FromSeconds(DotonTimeStill);
    internal static float DotonTimeStill => AoESimpleMode ? 1.5f : NIN_AoE_AdvancedMode_Ninjitsus_Doton_TimeStill;

    internal static bool CanUseFumaShuriken => ActionReady(Ten);

    internal static bool CanUseRaiton => LevelChecked(Raiton) && ActionReady(Ten) &&
                                          (!HasKassatsu || IsNotEnabled(Preset.NIN_ST_AdvancedMode_Ninjitsus_Hyosho) && !STSimpleMode || !LevelChecked(HyoshoRanryu)) && //Use kassatsu on it if Hyosho isn't selected. 
                                           (TrickDebuff || // Buff Window
                                           !LevelChecked(Suiton) || //Dont Pool because of Suiton not learned yet
                                           GetCooldownChargeRemainingTime(Ten) < 1 && TrickCD > 18 || // Spend to avoid cap
                                           !NIN_ST_AdvancedMode_Ninjitsus_Raiton_Pooling && !STSimpleMode || //Dont Pool because of Raiton Option
                                           NIN_ST_AdvancedMode_Ninjitsus_Raiton_Uptime && !InMeleeRange() && GetCooldownChargeRemainingTime(Ten) <= TrickCD - 10); //Uptime option

    internal static bool CanUseKaton => LevelChecked(Katon) && ActionReady(Ten) &&
                                         (!HasKassatsu || IsNotEnabled(Preset.NIN_AoE_AdvancedMode_Ninjitsus_Goka) && !STSimpleMode) &&
                                         (TrickDebuff || //Buff Window
                                          !LevelChecked(Huton) || //Dont Pool because of Huton not learned yet
                                          GetCooldownChargeRemainingTime(Ten) < 1 || // Spend to avoid cap
                                          !NIN_AoE_AdvancedMode_Ninjitsus_Katon_Pooling && !AoESimpleMode || //Dont Pool
                                          NIN_AoE_AdvancedMode_Ninjitsus_Katon_Uptime && !InMeleeRange() &&
                                          GetCooldownChargeRemainingTime(Ten) <= TrickCD - 10); //Uptime option

    internal static bool CanUseDoton => LevelChecked(Doton) && ActionReady(Ten) && DotonStoppedMoving && !JustUsed(Doton) &&
                                        (!HasDoton || DotonRemaining <= 2) && //No doton down
                                        (TrickDebuff || GetCooldownChargeRemainingTime(Ten) < 3); //Pool for buff window

    internal static bool CanUseSuiton => LevelChecked(Suiton) && ActionReady(Ten) && !HasStatusEffect(Buffs.ShadowWalker);

    internal static bool CanUseHuton => LevelChecked(Huton) && ActionReady(Ten) && !HasStatusEffect(Buffs.ShadowWalker);

    internal static bool CanUseHyoshoRanryu => LevelChecked(HyoshoRanryu) && ActionReady(Ten) && HasKassatsu &&
                                               (BuffWindow || IsNotEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && !STSimpleMode || KassatsuRemaining < 3);

    internal static bool CanUseGokaMekkyaku => LevelChecked(GokaMekkyaku) && ActionReady(Ten) && HasKassatsu &&
                                               (BuffWindow || IsNotEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && !STSimpleMode || KassatsuRemaining < 3);
    #endregion

    #region GCD Logic
    internal static bool TNArmorCrush => !MudraPhase && !OnTargetsFlank() && TargetNeedsPositionals() && Role.CanTrueNorth();
    internal static bool TNAeolianEdge => !MudraPhase && !OnTargetsRear() && TargetNeedsPositionals() && Role.CanTrueNorth();
    internal static bool CanPhantomKamaitachi => !MudraPhase && HasStatusEffect(Buffs.PhantomReady) &&
                                                 (TrickDebuff && ComboAction != GustSlash ||
                                                  !TrickDebuff);
    internal static bool CanThrowingDaggers => !MudraPhase && ActionReady(ThrowingDaggers) && HasTarget() && !InMeleeRange() &&
                                               !HasStatusEffect(Buffs.RaijuReady);
    internal static bool CanThrowingDaggersAoE => !MudraPhase && ActionReady(ThrowingDaggers) && HasTarget() && GetTargetDistance() >= 4.5 && InActionRange(ThrowingDaggers) &&
                                                  !HasStatusEffect(Buffs.RaijuReady);
    internal static bool CanRaiju => !MudraPhase && HasStatusEffect(Buffs.RaijuReady);
    #endregion

    #region Buff Window Logic
    internal static bool TrickDisabledST => IsNotEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && !STSimpleMode;
    internal static bool TrickDisabledAoE => IsNotEnabled(Preset.NIN_AoE_AdvancedMode_TrickAttack) && !AoESimpleMode;
    internal static bool MugDisabledST => IsNotEnabled(Preset.NIN_ST_AdvancedMode_Mug) && !STSimpleMode;
    internal static bool MugDisabledAoE => IsNotEnabled(Preset.NIN_AoE_AdvancedMode_Mug) && !AoESimpleMode;
    internal static int STMugThreshold => NIN_ST_AdvancedMode_Mug_SubOption == 1 || !InBossEncounter() ? NIN_ST_AdvancedMode_Mug_Threshold : 0;
    internal static int AoEMugThreshold => NIN_AoE_AdvancedMode_Mug_SubOption == 1 || !InBossEncounter() ? NIN_AoE_AdvancedMode_Mug_Threshold : 0;
    internal static int STTrickThreshold => NIN_ST_AdvancedMode_TrickAttack_SubOption == 1 || !InBossEncounter() ? NIN_ST_AdvancedMode_TrickAttack_Threshold : 0;
    internal static int AoETrickThreshold => NIN_AoE_AdvancedMode_TrickAttack_SubOption == 1 || !InBossEncounter() ? NIN_AoE_AdvancedMode_TrickAttack_Threshold : 0;
    internal static bool BuffWindow => TrickDebuff || MugDebuff && TrickCD >= 30;
    internal static float TrickCD => GetCooldownRemainingTime(OriginalHook(TrickAttack));
    internal static float MugCD => GetCooldownRemainingTime(OriginalHook(Mug));

    internal static bool CanTrickST => ActionReady(OriginalHook(TrickAttack)) && CanWeave() && CanApplyStatus(CurrentTarget, [Debuffs.TrickAttack, Debuffs.KunaisBane]) && HasStatusEffect(Buffs.ShadowWalker) && !MudraPhase &&
                                     (MugDebuff || MugCD >= 45 || MugDisabledST);
    internal static bool CanTrickAoE => ActionReady(OriginalHook(TrickAttack)) && CanWeave() && CanApplyStatus(CurrentTarget, [Debuffs.TrickAttack, Debuffs.KunaisBane]) && HasStatusEffect(Buffs.ShadowWalker) && !MudraPhase &&
                                     (MugDebuff || MugCD >= 45 || MugDisabledAoE);

    internal static bool CanMugST => ActionReady(OriginalHook(Mug)) && CanApplyStatus(CurrentTarget, [Debuffs.Mug, Debuffs.Dokumori]) && CanDelayedWeave(1.25f, .6f, 10) && !MudraPhase &&
                                   (TrickCD <= 6 || TrickDisabledST) &&
                                   (LevelChecked(Dokumori) && InActionRange(Dokumori) || InMeleeRange());
    internal static bool CanMugAoE => ActionReady(OriginalHook(Mug)) && CanApplyStatus(CurrentTarget, [Debuffs.Mug, Debuffs.Dokumori]) && CanDelayedWeave(1.25f, .6f, 10) && !MudraPhase &&
                                   (TrickCD <= 6 || TrickDisabledAoE) &&
                                   (LevelChecked(Dokumori) && InActionRange(Dokumori) || InMeleeRange());

    internal static bool TrickDebuff => HasStatusEffect(Debuffs.TrickAttack, CurrentTarget) || HasStatusEffect(Debuffs.KunaisBane, CurrentTarget) || JustUsed(OriginalHook(TrickAttack));
    internal static bool MugDebuff => HasStatusEffect(Debuffs.Mug, CurrentTarget) || HasStatusEffect(Debuffs.Dokumori, CurrentTarget) || JustUsed(OriginalHook(Mug));
    #endregion

    #region Ninki Use Logic
    internal static bool NinkiWillOvercap => gauge.Ninki > 50;
    internal static bool CanBunshin => CanWeave() && !MudraPhase && ActionReady(Bunshin) && gauge.Ninki >= 50;
    internal static bool CanBhavacakra => CanWeave() && gauge.Ninki >= 50 && !MudraPhase &&
                                          (!HasStatusEffect(Buffs.Higi) || BuffWindow || TrickDisabledST);
    internal static bool CanHellfrogMedium => CanWeave() && gauge.Ninki >= 50 && LevelChecked(HellfrogMedium) && !MudraPhase &&
                                              (!HasStatusEffect(Buffs.Higi) || BuffWindow || TrickDisabledAoE);

    internal static bool NinkiPooling => gauge.Ninki >= NinkiPool();
    internal static int NinkiPool()
    {
        if (MugCD < 5)
            return 60;
        if (GetCooldownRemainingTime(Bunshin) < 15)
            return 85;
        if (TrickDebuff)
            return 50;
        if (HasStatusEffect(Buffs.Bunshin))
            return ComboAction == GustSlash ? 65 : 85;
        return ComboAction == GustSlash ? 80 : 90;
    }
    #endregion

    #region Kassatsu, Meisui, Assassinate, TenChiJin Logic
    internal static bool HasKassatsu => HasStatusEffect(Buffs.Kassatsu) || JustUsed(Kassatsu, 1);
    internal static float KassatsuRemaining => GetStatusEffectRemainingTime(Buffs.Kassatsu);
    internal static bool CanKassatsu => !MudraPhase && ActionReady(Kassatsu) && CanWeave() &&
                                        (TrickCD < 10 && HasStatusEffect(Buffs.ShadowWalker) ||
                                         BuffWindow ||
                                         TrickDisabledST);

    internal static bool CanKassatsuAoE => !MudraPhase && ActionReady(Kassatsu) && CanWeave() &&
                                        (TrickCD < 10 && HasStatusEffect(Buffs.ShadowWalker) ||
                                         BuffWindow ||
                                         TrickDisabledAoE);

    internal static bool CanMeisui => !MudraPhase && ActionReady(Meisui) && CanWeave() && HasStatusEffect(Buffs.ShadowWalker) &&
                                      (BuffWindow || TrickDisabledST);
    internal static bool CanMeisuiAoE => !MudraPhase && ActionReady(Meisui) && CanWeave() && HasStatusEffect(Buffs.ShadowWalker) &&
                                      (BuffWindow || TrickDisabledAoE);

    internal static bool CanAssassinate => !MudraPhase && ActionReady(OriginalHook(Assassinate)) && CanWeave() &&
                                           (BuffWindow || TrickDisabledST || !LevelChecked(Suiton));
    internal static bool CanAssassinateAoE => !MudraPhase && ActionReady(OriginalHook(Assassinate)) && CanWeave() &&
                                           (BuffWindow || TrickDisabledAoE || !LevelChecked(Huton));

    internal static bool CanTenChiJin => !MudraPhase && !MudraAlmostReady && ActionReady(TenChiJin) && CanWeave() &&
                                         (BuffWindow || TrickDisabledST);
    internal static bool CanTenChiJinAoE => !MudraPhase && !MudraAlmostReady && ActionReady(TenChiJin) && CanWeave() &&
                                            (BuffWindow || TrickDisabledAoE);

    internal static bool CanTenriJindo => CanWeave() && HasStatusEffect(Buffs.TenriJendoReady);

    internal static uint OriginalTen => HasKassatsu ? TenCombo : Ten;
    internal static uint OriginalJin => HasKassatsu ? JinCombo : Jin;
    internal static uint OriginalChi => HasKassatsu ? ChiCombo : Chi;
    #endregion

    #region TCJ Methods
    internal static bool STTenChiJin(ref uint actionID)
    {
        if (HasStatusEffect(Buffs.TenChiJin))
        {
            if (FirstMudra == MudraFlags.None)
            {
                actionID = TCJFumaShurikenTen;
                return true;
            }

            if (SecondMudra == MudraFlags.None)
            {
                if (FirstMudra == MudraFlags.TenFirst)
                    actionID = TCJRaiton;
                else if (FirstMudra == MudraFlags.JinFirst)
                    actionID = TCJKaton;
                else
                    actionID = TCJHyoton;

                return true;
            }
            else
            {
                if (UnusedJutsus.Contains(Ten))
                    actionID = TCJHuton;
                else if (UnusedJutsus.Contains(Chi))
                    actionID = TCJDoton;
                else
                    actionID = TCJSuiton;

                return true;
            }

        }
        return false;
    }
    internal static bool AoETenChiJin(ref uint actionID, bool advancedMode)
    {
        if (HasStatusEffect(Buffs.TenChiJin))
        {
            if (FirstMudra == MudraFlags.None)
            {
                if (DotonRemaining >= (advancedMode ? NIN_AoE_AdvancedMode_TCJ_Doton_Timer : 3))
                    actionID = TCJFumaShurikenTen;
                else
                    actionID = TCJFumaShurikenJin;

                return true;
            }

            if (SecondMudra == MudraFlags.None)
            {
                if (FirstMudra == MudraFlags.TenFirst)
                    actionID = TCJRaiton;
                else if (FirstMudra == MudraFlags.JinFirst)
                    actionID = TCJKaton;
                else
                    actionID = TCJHyoton;

                return true;
            }
            else
            {
                if (UnusedJutsus.Contains(Ten))
                    actionID = TCJHuton;
                else if (UnusedJutsus.Contains(Chi))
                    actionID = TCJDoton;
                else
                    actionID = TCJSuiton;

                return true;
            }

        }
        return false;
    }

    #endregion

    #region Mudra
    internal class MudraCasting
    {
        #region Mudra State Stuff

        public MudraState CurrentMudra = MudraState.None;

        public bool PresetEnabled => AssociatedPreset is { } pre && IsEnabled(pre);
        internal Preset? AssociatedPreset;

        public MudraCasting()
        {
            OnStatusChanged += StatusChanged;
        }

        private void StatusChanged(uint statusId, bool onPlayer)
        {
            if (!PresetEnabled) return;

            if (statusId == Buffs.Mudra)
            {
                if (InMudra)
                {
                    Svc.Log.Debug($"{AssociatedPreset} -> {CurrentMudra} set");
                }
                else
                {
                    Svc.Log.Debug($"{AssociatedPreset} -> {CurrentMudra} reset to none");
                    CurrentMudra = MudraState.None;
                    ActionWatching.LastAction = 0;
                }
            }
        }

        public enum MudraState
        {
            None,
            CastingFumaShuriken,
            CastingKaton,
            CastingRaiton,
            CastingHuton,
            CastingDoton,
            CastingSuiton,
            CastingGokaMekkyaku,
            CastingHyoshoRanryu
        }

        public bool ContinueCurrentMudra(ref uint actionID)
        {
            return CurrentMudra switch
            {
                MudraState.None => false,
                MudraState.CastingFumaShuriken => CastFumaShuriken(ref actionID),
                MudraState.CastingKaton => CastKaton(ref actionID),
                MudraState.CastingRaiton => CastRaiton(ref actionID),
                MudraState.CastingHuton => CastHuton(ref actionID),
                MudraState.CastingDoton => CastDoton(ref actionID),
                MudraState.CastingSuiton => CastSuiton(ref actionID),
                MudraState.CastingGokaMekkyaku => CastGokaMekkyaku(ref actionID),
                MudraState.CastingHyoshoRanryu => CastHyoshoRanryu(ref actionID),
                _ => false
            };
        }
        #endregion

        #region Fuma Shuriken
        public bool CastFumaShuriken(ref uint actionID) // Ten
        {
            if (CurrentMudra is MudraState.None or MudraState.CastingFumaShuriken)
            {
                // Finish the Mudra
                if (LastMudra is Ten or TenCombo)
                {
                    actionID = FumaShuriken;
                    return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingFumaShuriken;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? TenCombo : Ten;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Raiton
        public bool CastRaiton(ref uint actionID)  // Ten Chi
        {
            if (Raiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingRaiton)
            {
                // Finish the Mudra
                switch (LastMudra)
                {
                    case Ten or TenCombo or Jin or JinCombo:
                        actionID = ChiCombo;
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout Fuma
                        actionID = Raiton;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingRaiton;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? TenCombo : Ten;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Suiton
        public bool CastSuiton(ref uint actionID)  //Ten Chi Jin
        {
            if (Suiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingSuiton)
            {
                //Finish the Mudra
                switch (LastMudra)
                {
                    case Ten or TenCombo:
                        actionID = ChiCombo;
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout Hyoten
                        actionID = JinCombo;
                        return true;
                    case Jin or JinCombo: //Jin == Bailout Fuma
                        actionID = Suiton;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingSuiton;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? TenCombo : Ten;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Hyosho Ranryu 
        public bool CastHyoshoRanryu(ref uint actionID) // Ten Jin
        {
            if (HyoshoRanryu.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoshoRanryu)
            {
                //Finish the Mudra
                switch (LastMudra)
                {
                    case Ten or TenCombo or Chi or ChiCombo:
                        actionID = JinCombo;
                        return true;
                    case Jin or JinCombo: //Jin == Bailout to Fuma
                        actionID = HyoshoRanryu;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingHyoshoRanryu;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? TenCombo : Ten;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Katon
        public bool CastKaton(ref uint actionID) // Jin Ten
        {
            if (Katon.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingKaton)
            {
                //Finish the Mudra
                switch (LastMudra)
                {
                    case Jin or JinCombo or Chi or ChiCombo:
                        actionID = TenCombo;
                        return true;
                    case Ten or TenCombo: //Ten == Bailout to Fuma
                        actionID = Katon;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingKaton;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? ChiCombo : Chi;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Doton
        public bool CastDoton(ref uint actionID) // Jin Ten Chi
        {
            if (Doton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingDoton)
            {
                //Finish the Mudra
                switch (LastMudra)
                {
                    case Jin or JinCombo:
                        actionID = TenCombo;
                        return true;
                    case Ten or TenCombo: // Ten == Bailout to Raiton
                        actionID = ChiCombo;
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout Fuma
                        actionID = Doton;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingDoton;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? JinCombo : Jin;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Huton
        public bool CastHuton(ref uint actionID) // Jin Chi Ten
        {
            if (Huton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHuton)
            {
                //Finish the Mudra
                switch (LastMudra)
                {
                    case Jin or JinCombo:
                        actionID = ChiCombo;
                        return true;
                    case Chi or ChiCombo: //Chi == Bailout katon
                        actionID = TenCombo;
                        return true;
                    case Ten or TenCombo: // Ten == Bailout to Fuma
                        actionID = Huton;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingHuton;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? JinCombo : Jin;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion

        #region Goka Mekkyaku
        public bool CastGokaMekkyaku(ref uint actionID) // Jin Ten
        {
            if (GokaMekkyaku.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingGokaMekkyaku)
            {
                //Finish the Mudra
                switch (LastMudra)
                {
                    case Jin or JinCombo or Chi or ChiCombo:
                        actionID = TenCombo;
                        return true;
                    case Ten or TenCombo: // Ten == Bailout to Fuma
                        actionID = GokaMekkyaku;
                        return true;
                }
                // Start the Mudra
                CurrentMudra = MudraState.CastingGokaMekkyaku;
                actionID = HasStatusEffect(Buffs.Kassatsu) ? JinCombo : Jin;
                return true;
            }
            CurrentMudra = MudraState.None;
            return false;
        }
        #endregion
    }
    #endregion

    #region Mudra Standalone Logic
    // Single Target
    internal static uint UseFumaShuriken(ref uint actionId)
    {
        return actionId = LastMudra is Ten or Chi or Jin ? FumaShuriken : Ten;
    }
    internal static uint UseRaiton(ref uint actionId) // Ten Chi
    {
        if (LastMudra == OriginalTen || LastMudra == OriginalJin)
            actionId = ChiCombo;
        else if (LastMudra == ChiCombo)
            actionId = Raiton;

        return actionId;
    }
    internal static uint UseHyoshoRanryu(ref uint actionId) // Ten Jin
    {
        if (LastMudra is TenCombo or ChiCombo)
            actionId = JinCombo;
        else if (LastMudra == JinCombo)
            actionId = HyoshoRanryu;

        return actionId;
    }
    internal static uint UseSuiton(ref uint actionId) // Ten Chi Jin
    {
        if (LastMudra == OriginalTen)
            actionId = ChiCombo;
        else if (LastMudra == ChiCombo)
            actionId = JinCombo;
        else if (LastMudra == JinCombo)
            actionId = Suiton;

        return actionId;
    }
    //Multi Target
    internal static uint UseGokaMekkyaku(ref uint actionId) // Chi Ten
    {
        if (LastMudra == OriginalChi)
            actionId = TenCombo;
        else if (LastMudra is TenCombo)
            actionId = GokaMekkyaku;

        return actionId;
    }
    internal static uint UseKaton(ref uint actionId) // Chi Ten
    {
        if (LastMudra == OriginalChi)
            actionId = TenCombo;
        else if (LastMudra is TenCombo)
            actionId = Katon;

        return actionId;
    }
    internal static uint UseDoton(ref uint actionId)  //Jin Ten Chi
    {
        if (LastMudra == OriginalJin)
            actionId = TenCombo;
        else if (LastMudra is TenCombo)
            actionId = ChiCombo;
        else if (LastMudra is ChiCombo)
            actionId = Doton;

        return actionId;
    }

    internal static uint UseHuton(ref uint actionId) // Jin Chi Ten
    {
        if (LastMudra == OriginalChi)
            actionId = JinCombo;
        else if (LastMudra is JinCombo)
            actionId = TenCombo;
        else if (LastMudra is TenCombo)
            actionId = Huton;

        return actionId;
    }
    #endregion

    #region Opener
    internal static NINOpenerMaxLevel4thGCDKunai Opener1 = new();
    internal static NINOpenerMaxLevel3rdGCDDokumori Opener2 = new();
    internal static NINOpenerMaxLevel3rdGCDKunai Opener3 = new();
    internal static NINOpenerMaxLevelBuffRush Opener4 = new();

    internal static WrathOpener Opener()
    {
        if (IsEnabled(Preset.NIN_ST_AdvancedMode))
        {
            int selection = NIN_Adv_Opener_Selection;
            switch (selection)
            {
                case 0 when Opener1.LevelChecked:
                    return Opener1;
                case 1 when Opener2.LevelChecked:
                    return Opener2;
                case 2 when Opener3.LevelChecked:
                    return Opener3;
                case 3 when Opener4.LevelChecked:
                    return Opener4;
            }
        }

        return Opener1.LevelChecked ? Opener1 : WrathOpener.Dummy;
    }

    internal abstract class NINOpenerBase : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData ContentCheckConfig => NIN_Balance_Content;
        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

            return true;
        }
        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps
        { get; set; } = [([1, 2, 3], () => OriginalHook(Ninjutsu) == Suiton)];

        public override Preset Preset => Preset.NIN_ST_AdvancedMode_BalanceOpener;
    }

    internal class NINOpenerMaxLevel4thGCDKunai : NINOpenerBase
    {
        //4th GCD Kunai
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten, //1
            ChiCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            GustSlash, //7
            Dokumori, //8
            Bunshin, //9
            PhantomKamaitachi, //10
            ArmorCrush, //11
            KunaisBane, //12
            ChiCombo, //13
            JinCombo, //14
            HyoshoRanryu, //15
            DreamWithinADream, //16
            Ten, //17
            ChiCombo, //18
            Raiton, //19
            TenChiJin, //20
            TCJFumaShurikenTen, //21
            TCJRaiton, //22
            TCJSuiton, //23
            Meisui, //24
            FleetingRaiju, //25
            ZeshoMeppo, //26
            TenriJendo, //27
            FleetingRaiju, //28
            Bhavacakra, //29
            Ten, //30
            ChiCombo, //31
            Raiton, //32
            FleetingRaiju, //33
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            12
        ];
    }

    internal class NINOpenerMaxLevel3rdGCDDokumori : NINOpenerBase
    {
        //3rd GCD Dokumori
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten, //1
            ChiCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            GustSlash, //7
            ArmorCrush, //8
            Dokumori, //9
            Bunshin, //10
            PhantomKamaitachi, //11
            KunaisBane, //12
            ChiCombo, //13
            JinCombo, //14
            HyoshoRanryu, //15
            DreamWithinADream, //16
            Ten, //17
            ChiCombo, //18
            Raiton, //19
            TenChiJin, //20
            TCJFumaShurikenTen, //21
            TCJRaiton, //22
            TCJSuiton, //23
            Meisui, //24
            FleetingRaiju, //25
            ZeshoMeppo, //26
            TenriJendo, //27
            FleetingRaiju, //28
            Ten, //29
            ChiCombo, //30
            Raiton, //31
            FleetingRaiju, //32
            Bhavacakra, //33
            SpinningEdge //34
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            12
        ];
    }

    internal class NINOpenerMaxLevel3rdGCDKunai : NINOpenerBase
    {
        //3rd GCD Kunai
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten, //1
            ChiCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            GustSlash, //7
            Dokumori, //8
            Bunshin, //9
            PhantomKamaitachi, //10
            KunaisBane, //11
            ChiCombo, //12
            JinCombo, //13
            HyoshoRanryu, //14
            DreamWithinADream, //15
            Ten, //16
            ChiCombo, //17
            Raiton, //18
            TenChiJin, //19
            TCJFumaShurikenTen, //20
            TCJRaiton, //21
            TCJSuiton, //22
            Meisui, //23
            FleetingRaiju, //24
            ZeshoMeppo, //25
            TenriJendo, //26
            FleetingRaiju, //27
            ArmorCrush, //28
            Bhavacakra, //29
            Ten, //30
            ChiCombo, //31
            Raiton, //32
            FleetingRaiju, //33
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            11
        ];

    }

    internal class NINOpenerMaxLevelBuffRush : NINOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten, //1
            ChiCombo, //2
            JinCombo, //3
            Suiton, //4
            Kassatsu, //5
            SpinningEdge, //6
            Dokumori, //7
            GustSlash, //8
            Bunshin, //9
            KunaisBane, //10
            ChiCombo, //11
            JinCombo, //12
            HyoshoRanryu, //13
            DreamWithinADream, //14
            Ten, //15
            ChiCombo, //16
            Raiton, //17
            TenChiJin, //18
            TCJFumaShurikenTen, //19
            TCJRaiton, //20
            TCJSuiton, //21
            Meisui, //22
            FleetingRaiju, //23
            ZeshoMeppo, //24
            TenriJendo, //25
            FleetingRaiju, //26
            Ten, //27
            ChiCombo, //28
            Raiton, //29
            FleetingRaiju, //30
            PhantomKamaitachi, //31
            ArmorCrush, //32
            Bhavacakra, //33
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            7
        ];
    }
    #endregion

    #region ID's

    public const uint
        SpinningEdge = 2240,
        ShadeShift = 2241,
        GustSlash = 2242,
        Hide = 2245,
        Assassinate = 2246,
        ThrowingDaggers = 2247,
        Mug = 2248,
        DeathBlossom = 2254,
        AeolianEdge = 2255,
        TrickAttack = 2258,
        Shukuchi = 2262,
        Kassatsu = 2264,
        ArmorCrush = 3563,
        DreamWithinADream = 3566,
        TenChiJin = 7403,
        Bhavacakra = 7402,
        HakkeMujinsatsu = 16488,
        Meisui = 16489,
        Bunshin = 16493,
        PhantomKamaitachi = 25774,
        ForkedRaiju = 25777,
        FleetingRaiju = 25778,
        HellfrogMedium = 7401,
        HollowNozuchi = 25776,
        TenriJendo = 36961,
        KunaisBane = 36958,
        ZeshoMeppo = 36960,
        Dokumori = 36957,

        //Mudras
        Ninjutsu = 2260,
        Rabbit = 2272,

        //-- initial state mudras (the ones with charges)
        Ten = 2259,
        Chi = 2261,
        Jin = 2263,

        //-- mudras used for combos (the ones used while you have the mudra buff)
        TenCombo = 18805,
        ChiCombo = 18806,
        JinCombo = 18807,

        //Ninjutsu
        FumaShuriken = 2265,
        Hyoton = 2268,
        Doton = 2270,
        Katon = 2266,
        Suiton = 2271,
        Raiton = 2267,
        Huton = 2269,
        GokaMekkyaku = 16491,
        HyoshoRanryu = 16492,

        //TCJ Jutsus
        TCJFumaShurikenTen = 18873,
        TCJFumaShurikenChi = 18874,
        TCJFumaShurikenJin = 18875,
        TCJKaton = 18876,
        TCJRaiton = 18877,
        TCJHyoton = 18878,
        TCJHuton = 18879,
        TCJDoton = 18880,
        TCJSuiton = 18881;

    public static class Buffs
    {
        public const ushort
            Mudra = 496,
            Kassatsu = 497,
            Higi = 3850,
            TenriJendoReady = 3851,
            ShadowWalker = 3848,
            Hidden = 614,
            TenChiJin = 1186,
            AssassinateReady = 1955,
            RaijuReady = 2690,
            PhantomReady = 2723,
            Meisui = 2689,
            Doton = 501,
            Bunshin = 1954;
    }

    public static class Debuffs
    {
        public const ushort
            Dokumori = 3849,
            TrickAttack = 3254,
            KunaisBane = 3906,
            Mug = 638;
    }

    public static class Traits
    {
        public const uint
            EnhancedKasatsu = 250,
            MugMastery = 585;
    }

    #endregion
}

