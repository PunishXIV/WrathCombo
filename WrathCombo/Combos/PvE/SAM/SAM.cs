using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class SAM : MeleeJob
{
    internal class SAM_ST_YukikazeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_YukikazeCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Yukikaze)
                return actionID;

            if (Config.SAM_Yukaze_KenkiOvercap && CanWeave() &&
                Gauge.Kenki >= Config.SAM_Yukaze_KenkiOvercapAmount && LevelChecked(Shinten))
                return OriginalHook(Shinten);

            if (HasEffect(Buffs.MeikyoShisui) && LevelChecked(Yukikaze))
                return OriginalHook(Yukikaze);

            if (ComboTimer > 0 && ComboAction == OriginalHook(Hakaze) && LevelChecked(Yukikaze))
                return OriginalHook(Yukikaze);

            return OriginalHook(Hakaze);
        }
    }

    internal class SAM_ST_KashaCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_KashaCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Kasha)
                return actionID;

            if (Config.SAM_Kasha_KenkiOvercap && CanWeave() &&
                Gauge.Kenki >= Config.SAM_Kasha_KenkiOvercapAmount && LevelChecked(Shinten))
                return OriginalHook(Shinten);

            if (HasEffect(Buffs.MeikyoShisui) && LevelChecked(Kasha))
                return OriginalHook(Kasha);

            if (ComboTimer > 0)
            {
                if (ComboAction == OriginalHook(Hakaze) && LevelChecked(Shifu))
                    return OriginalHook(Shifu);

                if (ComboAction is Shifu && LevelChecked(Kasha))
                    return OriginalHook(Kasha);
            }

            return OriginalHook(Hakaze);
        }
    }

    internal class SAM_ST_GeckoCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_GekkoCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Gekko)
                return actionID;

            if (Config.SAM_Gekko_KenkiOvercap && CanWeave() &&
                Gauge.Kenki >= Config.SAM_Gekko_KenkiOvercapAmount && LevelChecked(Shinten))
                return OriginalHook(Shinten);

            if (HasEffect(Buffs.MeikyoShisui) && LevelChecked(Gekko))
                return OriginalHook(Gekko);

            if (ComboTimer > 0)
            {
                if (ComboAction == OriginalHook(Hakaze) && LevelChecked(Jinpu))
                    return OriginalHook(Jinpu);

                if (ComboAction is Jinpu && LevelChecked(Gekko))
                    return OriginalHook(Gekko);
            }

            return OriginalHook(Hakaze);
        }
    }

    internal class SAM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Hakaze or Gyofu))
                return actionID;

            //Meikyo to start before combat
            if (!HasEffect(Buffs.MeikyoShisui) && ActionReady(MeikyoShisui) &&
                !InCombat() && TargetIsHostile())
                return MeikyoShisui;

            if (Variant.CanCure(CustomComboPreset.SAM_Variant_Cure, Config.SAM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SAM_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (ActionReady(Enpi) &&
                !InMeleeRange() && HasBattleTarget())
                return Enpi;

            //Meikyo Features
            if (UseMeikyo())
                return MeikyoShisui;

            //oGCDs
            if (CanWeave())
            {
                //Ikishoten Features
                //TODO Revisit when Raidbuffs are in
                if (ActionReady(Ikishoten) && !HasEffect(Buffs.ZanshinReady))
                {
                    switch (Gauge.Kenki)
                    {
                        //Dumps Kenki in preparation for Ikishoten
                        case >= 50:
                            return Shinten;

                        case < 50 when SenCount is 1 || JustUsed(Higanbana, 5):
                            return Ikishoten;
                    }
                }

                //Senei Features
                if (HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka) && Gauge.Kenki >= 25)
                {
                    if (ActionReady(Senei) &&
                        (TraitLevelChecked(Traits.EnhancedHissatsu) &&
                         (JustUsed(KaeshiSetsugekka, 5f) ||
                          JustUsed(TendoSetsugekka, 5f)) ||
                         !TraitLevelChecked(Traits.EnhancedHissatsu)))
                        return Senei;

                    //Guren if no Senei
                    if (!LevelChecked(Senei) && ActionReady(Guren) && InActionRange(Guren))
                        return Guren;
                }

                //Zanshin Usage
                if (ActionReady(Zanshin) && Gauge.Kenki >= 50 &&
                    InActionRange(Zanshin) &&
                    HasEffect(Buffs.ZanshinReady) &&
                    (JustUsed(Higanbana, 5) ||
                     GetBuffRemainingTime(Buffs.ZanshinReady) <= 6))
                    return Zanshin;

                if (ActionReady(Shoha) && Gauge.MeditationStacks is 3 &&
                    InActionRange(Shoha))
                    return Shoha;

                if (ActionReady(Shinten) &&
                    !HasEffect(Buffs.ZanshinReady) &&
                    (Gauge.Kenki >= 65 ||
                     GetTargetHPPercent() <= 1 && Gauge.Kenki >= 25))
                    return Shinten;

                // healing
                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
            {
                //Ogi Namikiri Features
                if (ActionReady(OgiNamikiri) && InActionRange(OriginalHook(OgiNamikiri)) &&
                    (JustUsed(Higanbana, 5f) && HasEffect(Buffs.OgiNamikiriReady) ||
                     HasEffect(Buffs.OgiNamikiriReady) && !TargetIsBoss() ||
                     GetBuffRemainingTime(Buffs.OgiNamikiriReady) <= 8) ||
                    Gauge.Kaeshi == Kaeshi.Namikiri)
                    return OriginalHook(OgiNamikiri);

                // Iaijutsu Features
                if (LevelChecked(Iaijutsu))
                {
                    if (LevelChecked(TendoKaeshiSetsugekka) && HasEffect(Buffs.TendoKaeshiSetsugekkaReady))
                        return OriginalHook(TsubameGaeshi);

                    if (LevelChecked(TsubameGaeshi) && HasEffect(Buffs.TsubameReady) &&
                        (TraitLevelChecked(Traits.EnhancedHissatsu) && GetCooldownRemainingTime(Senei) > 33 ||
                         SenCount is 3))
                        return OriginalHook(TsubameGaeshi);

                    if (SenCount is 1 && TargetIsBoss() &&
                        (GetDebuffRemainingTime(Debuffs.Higanbana) <= 10 && JustUsed(Gekko) && JustUsed(MeikyoShisui, 15f) ||
                         !TargetHasEffect(Debuffs.Higanbana)))
                        return OriginalHook(Iaijutsu);

                    if (SenCount is 2 && !LevelChecked(MidareSetsugekka))
                        return OriginalHook(Iaijutsu);

                    if (SenCount is 3 && LevelChecked(MidareSetsugekka) && !HasEffect(Buffs.TsubameReady))
                        return OriginalHook(Iaijutsu);
                }
            }

            if (HasEffect(Buffs.MeikyoShisui))
            {
                if (Role.CanTrueNorth() && CanDelayedWeave())
                    return Role.TrueNorth;

                if (LevelChecked(Gekko) &&
                    (!HasEffect(Buffs.Fugetsu) ||
                     !Gauge.HasGetsu && HasEffect(Buffs.Fuka)))
                    return Gekko;

                if (LevelChecked(Kasha) &&
                    (!HasEffect(Buffs.Fuka) ||
                     !Gauge.HasKa && HasEffect(Buffs.Fugetsu)))
                    return Kasha;

                if (LevelChecked(Yukikaze) && !Gauge.HasSetsu)
                    return Yukikaze;
            }

            if (ComboTimer > 0)
            {
                if (ComboAction is Hakaze or Gyofu && LevelChecked(Jinpu))
                {
                    if (!Gauge.HasSetsu && LevelChecked(Yukikaze) && HasEffect(Buffs.Fugetsu) &&
                        HasEffect(Buffs.Fuka))
                        return Yukikaze;

                    if (!LevelChecked(Kasha) &&
                        (GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) ||
                         !HasEffect(Buffs.Fugetsu)) ||
                        LevelChecked(Kasha) &&
                        (!HasEffect(Buffs.Fugetsu) ||
                         HasEffect(Buffs.Fuka) && !Gauge.HasGetsu ||
                         SenCount is 3 && GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka)))
                        return Jinpu;

                    if (LevelChecked(Shifu) &&
                        (!LevelChecked(Kasha) &&
                         (GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) ||
                          !HasEffect(Buffs.Fuka)) ||
                         LevelChecked(Kasha) &&
                         (!HasEffect(Buffs.Fuka) ||
                          HasEffect(Buffs.Fugetsu) && !Gauge.HasKa ||
                          SenCount is 3 && GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu))))
                        return Shifu;
                }

                if (ComboAction is Jinpu && LevelChecked(Gekko))
                    return Gekko;

                if (ComboAction is Shifu && LevelChecked(Kasha))
                    return Kasha;
            }
            return actionID;
        }
    }

    internal class SAM_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Hakaze or Gyofu))
                return actionID;

            int kenkiOvercap = Config.SAM_ST_KenkiOvercapAmount;
            int shintenTreshhold = Config.SAM_ST_ExecuteThreshold;
            int higanbanaThreshold = Config.SAM_ST_Higanbana_Threshold;

            //Meikyo to start before combat
            if (IsEnabled(CustomComboPreset.SAM_ST_CDs) &&
                IsEnabled(CustomComboPreset.SAM_ST_CDs_MeikyoShisui) &&
                ActionReady(MeikyoShisui) &&
                !HasEffect(Buffs.MeikyoShisui) &&
                !InCombat() && TargetIsHostile())
                return MeikyoShisui;

            if (Variant.CanCure(CustomComboPreset.SAM_Variant_Cure, Config.SAM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SAM_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            // Opener for SAM
            if (IsEnabled(CustomComboPreset.SAM_ST_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            if (IsEnabled(CustomComboPreset.SAM_ST_RangedUptime) &&
                ActionReady(Enpi) && !InMeleeRange() && HasBattleTarget())
                return Enpi;

            //Meikyo Features
            if (IsEnabled(CustomComboPreset.SAM_ST_CDs) &&
                IsEnabled(CustomComboPreset.SAM_ST_CDs_MeikyoShisui) &&
                UseMeikyo())
                return MeikyoShisui;

            //oGCDs
            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.SAM_ST_CDs))
                {
                    //Ikishoten Features
                    //TODO Revisit when Raidbuffs are in
                    if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Ikishoten) &&
                        ActionReady(Ikishoten) && !HasEffect(Buffs.ZanshinReady))
                    {
                        switch (Gauge.Kenki)
                        {
                            //Dumps Kenki in preparation for Ikishoten
                            case >= 50:
                                return Shinten;

                            case < 50 when SenCount is 1 || JustUsed(Higanbana, 5):
                                return Ikishoten;
                        }
                    }

                    //Senei Features
                    if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Senei) &&
                        HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka) && Gauge.Kenki >= 25)
                    {
                        if (ActionReady(Senei) &&
                            (TraitLevelChecked(Traits.EnhancedHissatsu) &&
                             (JustUsed(KaeshiSetsugekka, 5f) ||
                              JustUsed(TendoSetsugekka, 5f)) ||
                             !TraitLevelChecked(Traits.EnhancedHissatsu)))
                            return Senei;

                        //Guren if no Senei
                        if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Guren) &&
                            !LevelChecked(Senei) && ActionReady(Guren) &&
                            InActionRange(Guren))
                            return Guren;
                    }

                    //Zanshin Usage
                    if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Zanshin) &&
                        ActionReady(Zanshin) && Gauge.Kenki >= 50 &&
                        InActionRange(Zanshin) &&
                        HasEffect(Buffs.ZanshinReady) &&
                        (JustUsed(Higanbana, 5) ||
                         GetBuffRemainingTime(Buffs.ZanshinReady) <= 6))
                        return Zanshin;

                    if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Shoha) &&
                        LevelChecked(Shoha) && Gauge.MeditationStacks is 3 &&
                        InActionRange(Shoha))
                        return Shoha;
                }

                if (IsEnabled(CustomComboPreset.SAM_ST_Shinten) &&
                    ActionReady(Shinten) && !HasEffect(Buffs.ZanshinReady) &&
                    (Gauge.Kenki >= kenkiOvercap || GetTargetHPPercent() <= shintenTreshhold && Gauge.Kenki >= 25))
                    return Shinten;

                // healing
                if (IsEnabled(CustomComboPreset.SAM_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(Config.SAM_STSecondWindThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(Config.SAM_STBloodbathThreshold))
                        return Role.Bloodbath;
                }
            }

            if (IsEnabled(CustomComboPreset.SAM_ST_CDs) &&
                HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
            {
                //Ogi Namikiri Features
                if (IsEnabled(CustomComboPreset.SAM_ST_CDs_OgiNamikiri) &&
                    (!IsEnabled(CustomComboPreset.SAM_ST_CDs_OgiNamikiri_Movement) ||
                     IsEnabled(CustomComboPreset.SAM_ST_CDs_OgiNamikiri_Movement) && !IsMoving()) &&
                    ActionReady(OgiNamikiri) && InActionRange(OriginalHook(OgiNamikiri)) &&
                    HasEffect(Buffs.OgiNamikiriReady) &&
                    (JustUsed(Higanbana, 5f) ||
                     Config.SAM_ST_Higanbana_Suboption == 1 && !TargetIsBoss() ||
                     GetBuffRemainingTime(Buffs.OgiNamikiriReady) <= 8) ||
                    Gauge.Kaeshi == Kaeshi.Namikiri)
                    return OriginalHook(OgiNamikiri);

                // Iaijutsu Features
                if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu) &&
                    LevelChecked(Iaijutsu))
                {
                    if (LevelChecked(TendoKaeshiSetsugekka) && HasEffect(Buffs.TendoKaeshiSetsugekkaReady))
                        return OriginalHook(TsubameGaeshi);

                    if (LevelChecked(TsubameGaeshi) && HasEffect(Buffs.TsubameReady) &&
                        (TraitLevelChecked(Traits.EnhancedHissatsu) && GetCooldownRemainingTime(Senei) > 33 ||
                         SenCount is 3))
                        return OriginalHook(TsubameGaeshi);

                    if (!IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu_Movement) ||
                        IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu_Movement) && !IsMoving())
                    {
                        if (SenCount is 1 && GetTargetHPPercent() > higanbanaThreshold &&
                            (Config.SAM_ST_Higanbana_Suboption == 0 ||
                             Config.SAM_ST_Higanbana_Suboption == 1 && TargetIsBoss()) &&
                            (GetDebuffRemainingTime(Debuffs.Higanbana) <= 10 && JustUsed(Gekko) && JustUsed(MeikyoShisui, 15f) ||
                             !TargetHasEffect(Debuffs.Higanbana)))
                            return OriginalHook(Iaijutsu);

                        if (SenCount is 2 && !LevelChecked(MidareSetsugekka))
                            return OriginalHook(Iaijutsu);

                        if (SenCount is 3 && LevelChecked(MidareSetsugekka) && !HasEffect(Buffs.TsubameReady))
                            return OriginalHook(Iaijutsu);
                    }
                }
            }

            if (HasEffect(Buffs.MeikyoShisui))
            {
                if (IsEnabled(CustomComboPreset.SAM_ST_TrueNorth) &&
                    Role.CanTrueNorth() && CanDelayedWeave())
                    return Role.TrueNorth;

                if (LevelChecked(Gekko) &&
                    (!HasEffect(Buffs.Fugetsu) ||
                     !Gauge.HasGetsu && HasEffect(Buffs.Fuka)))
                    return Gekko;

                if (IsEnabled(CustomComboPreset.SAM_ST_Kasha) &&
                    LevelChecked(Kasha) &&
                    (!HasEffect(Buffs.Fuka) ||
                     !Gauge.HasKa && HasEffect(Buffs.Fugetsu)))
                    return Kasha;

                if (IsEnabled(CustomComboPreset.SAM_ST_Yukikaze) &&
                    LevelChecked(Yukikaze) && !Gauge.HasSetsu)
                    return Yukikaze;
            }

            if (ComboTimer > 0)
            {
                if (ComboAction is Hakaze or Gyofu && LevelChecked(Jinpu))
                {
                    if (IsEnabled(CustomComboPreset.SAM_ST_Yukikaze) &&
                        !Gauge.HasSetsu && LevelChecked(Yukikaze) && HasEffect(Buffs.Fugetsu) &&
                        HasEffect(Buffs.Fuka))
                        return Yukikaze;

                    if (!LevelChecked(Kasha) &&
                        (GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) ||
                         !HasEffect(Buffs.Fugetsu)) ||
                        LevelChecked(Kasha) &&
                        (!HasEffect(Buffs.Fugetsu) ||
                         HasEffect(Buffs.Fuka) && !Gauge.HasGetsu ||
                         SenCount is 3 && GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka)))
                        return Jinpu;

                    if (IsEnabled(CustomComboPreset.SAM_ST_Kasha) &&
                        LevelChecked(Shifu) &&
                        (!LevelChecked(Kasha) &&
                         (GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) ||
                          !HasEffect(Buffs.Fuka)) ||
                         LevelChecked(Kasha) &&
                         (!HasEffect(Buffs.Fuka) ||
                          HasEffect(Buffs.Fugetsu) && !Gauge.HasKa ||
                          SenCount is 3 && GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu))))
                        return Shifu;
                }

                if (ComboAction is Jinpu && LevelChecked(Gekko))
                    return Gekko;

                if (IsEnabled(CustomComboPreset.SAM_ST_Kasha) &&
                    ComboAction is Shifu && LevelChecked(Kasha))
                    return Kasha;
            }
            return actionID;
        }
    }

    internal class SAM_AoE_OkaCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_OkaCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Oka)
                return actionID;

            if (Config.SAM_Oka_KenkiOvercap && Gauge.Kenki >= Config.SAM_Oka_KenkiOvercapAmount &&
                LevelChecked(Kyuten) && CanWeave())
                return Kyuten;

            if (HasEffect(Buffs.MeikyoShisui))
                return Oka;

            if (ComboTimer > 0 && LevelChecked(Oka) &&
                ComboAction == OriginalHook(Fuko))
                return Oka;

            return OriginalHook(Fuko);
        }
    }

    internal class SAM_AoE_MangetsuCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_MangetsuCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Mangetsu)
                return actionID;

            if (Config.SAM_Mangetsu_KenkiOvercap && Gauge.Kenki >= Config.SAM_Mangetsu_KenkiOvercapAmount &&
                LevelChecked(Kyuten) && CanWeave())
                return Kyuten;

            if (HasEffect(Buffs.MeikyoShisui))
                return Mangetsu;

            if (ComboTimer > 0 && LevelChecked(Mangetsu) &&
                ComboAction == OriginalHook(Fuko))
                return Mangetsu;

            return OriginalHook(Fuko);
        }
    }

    internal class SAM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            // Don't change anything if not basic skill
            if (actionID is not (Fuga or Fuko))
                return actionID;

            if (Variant.CanCure(CustomComboPreset.SAM_Variant_Cure, Config.SAM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SAM_Variant_Rampart))
                return Variant.Rampart;

            //oGCD Features
            if (CanWeave())
            {
                if (OriginalHook(Iaijutsu) is MidareSetsugekka && LevelChecked(Hagakure))
                    return Hagakure;

                if (ActionReady(Ikishoten) && !HasEffect(Buffs.ZanshinReady))
                {
                    switch (Gauge.Kenki)
                    {
                        //Dumps Kenki in preparation for Ikishoten
                        case > 50:
                            return Kyuten;

                        case <= 50:
                            return Ikishoten;
                    }
                }

                if (ActionReady(MeikyoShisui) && !HasEffect(Buffs.MeikyoShisui))
                    return MeikyoShisui;

                if (ActionReady(Zanshin) && HasEffect(Buffs.ZanshinReady) && Gauge.Kenki >= 50)
                    return Zanshin;

                if (ActionReady(Guren) && Gauge.Kenki >= 25)
                    return Guren;

                if (ActionReady(Shoha) && Gauge.MeditationStacks is 3)
                    return Shoha;

                if (ActionReady(Kyuten) && Gauge.Kenki >= 50)
                    return Kyuten;

                // healing
                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (ActionReady(OgiNamikiri) &&
                !IsMoving() && (HasEffect(Buffs.OgiNamikiriReady) || Gauge.Kaeshi is Kaeshi.Namikiri))
                return OriginalHook(OgiNamikiri);

            if (LevelChecked(TenkaGoken) && !IsMoving())
            {
                if (LevelChecked(TsubameGaeshi) &&
                    (HasEffect(Buffs.KaeshiGokenReady) || HasEffect(Buffs.TendoKaeshiGokenReady)))
                    return OriginalHook(TsubameGaeshi);

                if (OriginalHook(Iaijutsu) is TenkaGoken)
                    return OriginalHook(Iaijutsu);

                if (LevelChecked(TendoGoken) && OriginalHook(Iaijutsu) is TendoGoken)
                    return OriginalHook(Iaijutsu);
            }

            if (HasEffect(Buffs.MeikyoShisui))
            {
                if (!Gauge.HasGetsu && HasEffect(Buffs.Fuka) ||
                    !HasEffect(Buffs.Fugetsu))
                    return Mangetsu;

                if (!Gauge.HasKa && HasEffect(Buffs.Fugetsu) ||
                    !HasEffect(Buffs.Fuka))
                    return Oka;
            }

            if (ComboTimer > 0 &&
                ComboAction is Fuko or Fuga && LevelChecked(Mangetsu))
            {
                if (!Gauge.HasGetsu ||
                    GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) ||
                    !HasEffect(Buffs.Fugetsu) || !LevelChecked(Oka))
                    return Mangetsu;

                if (LevelChecked(Oka) &&
                    (!Gauge.HasKa ||
                     GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) ||
                     !HasEffect(Buffs.Fuka)))
                    return Oka;
            }

            return actionID;
        }
    }

    internal class SAM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Fuga or Fuko))
                return actionID;

            float kenkiOvercap = Config.SAM_AoE_KenkiOvercapAmount;

            if (Variant.CanCure(CustomComboPreset.SAM_Variant_Cure, Config.SAM_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.SAM_Variant_Rampart))
                return Variant.Rampart;

            //oGCD Features
            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.SAM_AoE_Hagakure) &&
                    OriginalHook(Iaijutsu) is MidareSetsugekka && LevelChecked(Hagakure))
                    return Hagakure;

                if (IsEnabled(CustomComboPreset.SAM_AOE_CDs_Ikishoten) &&
                    ActionReady(Ikishoten) && !HasEffect(Buffs.ZanshinReady))
                {
                    switch (Gauge.Kenki)
                    {
                        //Dumps Kenki in preparation for Ikishoten
                        case > 50:
                            return Kyuten;

                        case <= 50:
                            return Ikishoten;
                    }
                }

                if (IsEnabled(CustomComboPreset.SAM_AoE_MeikyoShisui) &&
                    ActionReady(MeikyoShisui) && !HasEffect(Buffs.MeikyoShisui))
                    return MeikyoShisui;

                if (IsEnabled(CustomComboPreset.SAM_AoE_Zanshin) &&
                    ActionReady(Zanshin) && HasEffect(Buffs.ZanshinReady) && Gauge.Kenki >= 50)
                    return Zanshin;

                if (IsEnabled(CustomComboPreset.SAM_AoE_Guren) &&
                    ActionReady(Guren) && Gauge.Kenki >= 25)
                    return Guren;

                if (IsEnabled(CustomComboPreset.SAM_AoE_Shoha) &&
                    ActionReady(Shoha) && Gauge.MeditationStacks is 3)
                    return Shoha;

                if (IsEnabled(CustomComboPreset.SAM_AoE_Kyuten) &&
                    ActionReady(Kyuten) && Gauge.Kenki >= kenkiOvercap)
                    return Kyuten;

                if (IsEnabled(CustomComboPreset.SAM_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(Config.SAM_AoESecondWindThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(Config.SAM_AoEBloodbathThreshold))
                        return Role.Bloodbath;
                }
            }

            if (IsEnabled(CustomComboPreset.SAM_AoE_OgiNamikiri) &&
                ActionReady(OgiNamikiri) &&
                (!IsMoving() && HasEffect(Buffs.OgiNamikiriReady) ||
                 Gauge.Kaeshi is Kaeshi.Namikiri))
                return OriginalHook(OgiNamikiri);

            if (IsEnabled(CustomComboPreset.SAM_AoE_TenkaGoken) &&
                LevelChecked(TenkaGoken))
            {
                if (LevelChecked(TsubameGaeshi) &&
                    (HasEffect(Buffs.KaeshiGokenReady) || HasEffect(Buffs.TendoKaeshiGokenReady)))
                    return OriginalHook(TsubameGaeshi);

                if (!IsMoving() && OriginalHook(Iaijutsu) is TenkaGoken)
                    return OriginalHook(Iaijutsu);

                if (!IsMoving() && LevelChecked(TendoGoken) && OriginalHook(Iaijutsu) is TendoGoken)
                    return OriginalHook(Iaijutsu);
            }

            if (HasEffect(Buffs.MeikyoShisui))
            {
                if (!Gauge.HasGetsu && HasEffect(Buffs.Fuka) || !HasEffect(Buffs.Fugetsu))
                    return Mangetsu;

                if (IsEnabled(CustomComboPreset.SAM_AoE_Oka) &&
                    (!Gauge.HasKa && HasEffect(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka)))
                    return Oka;
            }

            if (ComboTimer > 0 &&
                ComboAction is Fuko or Fuga && LevelChecked(Mangetsu))
            {
                if (IsNotEnabled(CustomComboPreset.SAM_AoE_Oka) ||
                    !Gauge.HasGetsu ||
                    GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) ||
                    !HasEffect(Buffs.Fugetsu) || !LevelChecked(Oka))
                    return Mangetsu;

                if (IsEnabled(CustomComboPreset.SAM_AoE_Oka) &&
                    LevelChecked(Oka) &&
                    (!Gauge.HasKa ||
                     GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) ||
                     !HasEffect(Buffs.Fuka)))
                    return Oka;
            }
            return actionID;
        }
    }

    internal class SAM_MeikyoSens : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SAM_MeikyoSens;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MeikyoShisui || !HasEffect(Buffs.MeikyoShisui))
                return actionID;

            if (!HasEffect(Buffs.Fugetsu) ||
                !Gauge.HasGetsu)
                return Gekko;

            if (!HasEffect(Buffs.Fuka) ||
                !Gauge.HasKa)
                return Kasha;

            if (!Gauge.HasSetsu)
                return Yukikaze;

            return actionID;
        }
    }

    internal class SAM_Iaijutsu : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Iaijutsu;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Iaijutsu)
                return actionID;

            bool canAddShoha = IsEnabled(CustomComboPreset.SAM_Iaijutsu_Shoha) &&
                               ActionReady(Shoha) &&
                               Gauge.MeditationStacks is 3;

            if (canAddShoha && CanWeave())
                return Shoha;

            if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_OgiNamikiri) &&
                (ActionReady(OgiNamikiri) && HasEffect(Buffs.OgiNamikiriReady) ||
                 Gauge.Kaeshi == Kaeshi.Namikiri))
                return OriginalHook(OgiNamikiri);

            if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_TsubameGaeshi) &&
                (LevelChecked(TsubameGaeshi) &&
                 (HasEffect(Buffs.TsubameReady) ||
                  HasEffect(Buffs.KaeshiGokenReady)) ||
                 LevelChecked(TendoKaeshiSetsugekka) &&
                 (HasEffect(Buffs.TendoKaeshiSetsugekkaReady) ||
                  HasEffect(Buffs.TendoKaeshiGokenReady))))
                return OriginalHook(TsubameGaeshi);

            if (canAddShoha)
                return Shoha;

            return actionID;
        }
    }

    internal class SAM_Shinten : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Shinten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Shinten)
            {
                if (IsEnabled(CustomComboPreset.SAM_Shinten_Senei) &&
                    ActionReady(Senei))
                    return Senei;

                if (IsEnabled(CustomComboPreset.SAM_Shinten_Zanshin) &&
                    HasEffect(Buffs.ZanshinReady))
                    return Zanshin;

                if (IsEnabled(CustomComboPreset.SAM_Shinten_Shoha) &&
                    ActionReady(Shoha) && Gauge.MeditationStacks is 3)
                    return Shoha;
            }

            return actionID;
        }
    }

    internal class SAM_Kyuten : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Kyuten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Kyuten)
            {
                if (IsEnabled(CustomComboPreset.SAM_Kyuten_Guren) &&
                    ActionReady(Guren))
                    return Guren;

                if (IsEnabled(CustomComboPreset.SAM_Kyuten_Zanshin) &&
                    HasEffect(Buffs.ZanshinReady))
                    return Zanshin;

                if (IsEnabled(CustomComboPreset.SAM_Kyuten_Shoha) &&
                    ActionReady(Shoha) && Gauge.MeditationStacks is 3)
                    return Shoha;
            }
            return actionID;
        }
    }

    internal class SAM_Ikishoten : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Ikishoten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Ikishoten)
                return actionID;

            if (IsEnabled(CustomComboPreset.SAM_Ikishoten_Shoha) &&
                ActionReady(Shoha) &&
                HasEffect(Buffs.OgiNamikiriReady) &&
                Gauge.MeditationStacks is 3)
                return Shoha;

            if (IsEnabled(CustomComboPreset.SAM_Ikishoten_Namikiri) &&
                (ActionReady(OgiNamikiri) && HasEffect(Buffs.OgiNamikiriReady) ||
                 Gauge.Kaeshi == Kaeshi.Namikiri))
                return OriginalHook(OgiNamikiri);

            return actionID;
        }
    }

    internal class SAM_GyotenYaten : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_GyotenYaten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Gyoten && Gauge.Kenki >= 10)
            {
                if (InMeleeRange())
                    return Yaten;

                if (!InMeleeRange())
                    return Gyoten;
            }

            return actionID;
        }
    }

    internal class SAM_MeikyoShisuiProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_MeikyoShisuiProtection;

        protected override uint Invoke(uint actionID) =>
            actionID is MeikyoShisui &&
            HasEffect(Buffs.MeikyoShisui) &&
            ActionReady(MeikyoShisui)
                ? All.SavageBlade
                : actionID;
    }

    internal class SAM_SeneiGuren : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_SeneiGuren;

        protected override uint Invoke(uint actionID) =>
            actionID is Senei && !LevelChecked(Senei)
                ? Guren
                : actionID;
    }
}
