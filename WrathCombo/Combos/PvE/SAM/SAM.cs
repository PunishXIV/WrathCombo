using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.SAM.Config;
namespace WrathCombo.Combos.PvE;

internal partial class SAM : Melee
{
    internal class SAM_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Hakaze, Gyofu)) return actionID;

            //Meikyo to start before combat
            if (ActionReady(MeikyoShisui) &&
                !HasStatusEffect(Buffs.MeikyoShisui) &&
                !InCombat() && HasBattleTarget())
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //oGCDs
            if (CanWeave())
            {
                //Meikyo Feature
                if (CanMeikyo())
                    return MeikyoShisui;

                //Ikishoten Feature
                if (CanIkishoten())
                    return Ikishoten;

                if (GetTargetHPPercent() < 1)
                    return UseKenkiSpender(actionID, true, true, true);

                //Senei Feature
                if (CanSenei())
                    return Senei;

                //Guren if no Senei
                if (!LevelChecked(Senei) &&
                    ActionReady(Guren) && InActionRange(Guren))
                    return Guren;

                //Zanshin Usage
                if (CanZanshin())
                    return Zanshin;

                //Shoha Usage
                if (CanShoha())
                    return Shoha;

                //Shinten Usage
                if (CanShinten())
                    return Shinten;

                if (Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                //Auto Third Eye
                if (CanThirdEye())
                    return OriginalHook(ThirdEye);

                // healing
                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;

                if (RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (CanTsubame())
                return OriginalHook(TsubameGaeshi);

            //Ogi Namikiri feature
            if (CanOgiNamikiri(onlyWhenStationary: true))
                return OriginalHook(OgiNamikiri);

            // Iaijutsu feature
            if (CanStIaijutsu(true, true, true, true))
                return OriginalHook(Iaijutsu);

            //Ranged
            if (ActionReady(Enpi) && !InMeleeRange() && HasBattleTarget())
                return Enpi;

            return DoCombo(false);
        }
    }

    internal class SAM_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Fuga, Fuko)) return actionID;

            //Meikyo to start before combat
            if (ActionReady(MeikyoShisui) &&
                !HasStatusEffect(Buffs.MeikyoShisui) &&
                !InCombat() && HasBattleTarget())
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //oGCD feature
            if (CanWeave())
            {
                if (CanAoEHagakure())
                    return Hagakure;

                if (CanAoEMeikyo())
                    return MeikyoShisui;

                if (CanAoEIkishotenKenki(out uint kenkiAction))
                    return kenkiAction;

                if (CanAoEZanshin())
                    return Zanshin;

                if (CanAoEGuren())
                    return Guren;

                if (CanAoEShoha())
                    return Shoha;

                if (CanAoEKyuten())
                    return Kyuten;

                // healing
                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (CanAoEOgiNamikiri(onlyWhenStationary: true))
                return OriginalHook(OgiNamikiri);

            return CanAoESenGCD(out uint senAction, onlyWhenStationary: true)
                ? senAction
                : DoCombo(onAoE: true);
        }
    }

    internal class SAM_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Hakaze, Gyofu)) return actionID;

            // Opener for SAM
            if (IsEnabled(Preset.SAM_ST_Opener) &&
                Opener().FullOpener(ref actionID) &&
                HasBattleTarget())
                return actionID;

            //Meikyo to start before combat
            if (IsEnabled(Preset.SAM_ST_CDs) &&
                IsEnabled(Preset.SAM_ST_CDs_MeikyoShisui) &&
                ActionReady(MeikyoShisui) &&
                !HasStatusEffect(Buffs.MeikyoShisui) &&
                !InCombat() && HasBattleTarget() &&
                !JustUsed(MeikyoShisui))
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //oGCDs
            if (CanWeave())
            {
                if (IsEnabled(Preset.SAM_ST_CDs))
                {
                    //Meikyo feature
                    if (IsEnabled(Preset.SAM_ST_CDs_MeikyoShisui) &&
                        CanMeikyo(SAM_ST_MeikyoExecuteThreshold))
                        return MeikyoShisui;

                    //Ikishoten feature
                    if (IsEnabled(Preset.SAM_ST_CDs_Ikishoten) &&
                        CanIkishoten())
                        return Ikishoten;
                }

                if (IsEnabled(Preset.SAM_ST_Damage))
                {
                    if (GetTargetHPPercent() < SAM_ST_ExecuteThreshold)
                        return UseKenkiSpender(actionID,
                            IsEnabled(Preset.SAM_ST_CDs_Zanshin),
                            IsEnabled(Preset.SAM_ST_CDs_Senei),
                            IsEnabled(Preset.SAM_ST_Shinten));

                    //Senei feature
                    if (IsEnabled(Preset.SAM_ST_CDs_Senei))
                    {
                        if (CanSenei())
                            return Senei;

                        //Guren if no Senei
                        if (SAM_ST_CDs_Guren &&
                            !LevelChecked(Senei) &&
                            ActionReady(Guren) && InActionRange(Guren))
                            return Guren;
                    }

                    //Zanshin Usage
                    if (IsEnabled(Preset.SAM_ST_CDs_Zanshin) &&
                        CanZanshin())
                        return Zanshin;

                    if (IsEnabled(Preset.SAM_ST_CDs_Shoha) &&
                        CanShoha(SAM_ST_HiganbanaRefresh))
                        return Shoha;

                    if (IsEnabled(Preset.SAM_ST_Shinten) &&
                        CanShinten(SAM_ST_ExecuteThreshold, SAM_ST_KenkiOvercapAmount))
                        return Shinten;
                }

                if (IsEnabled(Preset.SAM_ST_Feint) &&
                    Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                //Auto Third Eye
                if (IsEnabled(Preset.SAM_ST_ThirdEye) &&
                    CanThirdEye())
                    return OriginalHook(ThirdEye);

                //Auto Meditate
                if (IsEnabled(Preset.SAM_ST_Meditate) &&
                    CanMeditate())
                    return Meditate;

                // healing
                if (IsEnabled(Preset.SAM_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(SAM_ST_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(SAM_ST_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.SAM_ST_StunInterrupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.SAM_ST_Damage))
            {
                if (IsEnabled(Preset.SAM_ST_CDs_Iaijutsu) &&
                    IsEnabled(Preset.SAM_ST_CDs_UseTsubame) &&
                    CanTsubame())
                    return OriginalHook(TsubameGaeshi);

                //Ogi Namikiri Feature
                if (IsEnabled(Preset.SAM_ST_CDs_OgiNamikiri) &&
                    CanOgiNamikiri(
                        respectMovementOption: SAM_ST_CDs_OgiNamikiri_Movement,
                        useHiganbanaBurstRules: IsEnabled(Preset.SAM_ST_CDs_UseHiganbana),
                        higanbanaBossOnly: SAM_ST_HiganbanaBossHPOption == 1))
                    return OriginalHook(OgiNamikiri);

                // Iaijutsu Feature
                if (IsEnabled(Preset.SAM_ST_CDs_Iaijutsu) &&
                    CanStIaijutsu(
                        IsEnabled(Preset.SAM_ST_CDs_UseHiganbana),
                        IsEnabled(Preset.SAM_ST_CDs_UseTenkaGoken),
                        IsEnabled(Preset.SAM_ST_CDs_UseMidare),
                        IsEnabled(Preset.SAM_ST_CDs_Iaijutsu_Movement),
                        HiganbanaHPThreshold(),
                        SAM_ST_HiganbanaRefresh))
                    return OriginalHook(Iaijutsu);

                //Ranged
                if (IsEnabled(Preset.SAM_ST_RangedUptime) &&
                    ActionReady(Enpi) && !InMeleeRange() && HasBattleTarget())
                    return Enpi;
            }

            return DoCombo(
                false,
                IsEnabled(Preset.SAM_ST_TrueNorth),
                IsEnabled(Preset.SAM_ST_Yukikaze),
                IsEnabled(Preset.SAM_ST_Kasha),
                IsEnabled(Preset.SAM_ST_Gekko),
                trueNorthCharges: SAM_ST_ManualTN);
        }
    }

    internal class SAM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Fuga, Fuko)) return actionID;

            //Meikyo to start before combat
            if (IsEnabled(Preset.SAM_AoE_CDs) &&
                IsEnabled(Preset.SAM_AoE_MeikyoShisui) &&
                ActionReady(MeikyoShisui) &&
                !HasStatusEffect(Buffs.MeikyoShisui) &&
                !InCombat() && HasBattleTarget() &&
                !JustUsed(MeikyoShisui))
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //oGCD feature
            if (CanWeave())
            {
                if (IsEnabled(Preset.SAM_AoE_Hagakure) && CanAoEHagakure())
                    return Hagakure;

                if (IsEnabled(Preset.SAM_AoE_CDs))
                {
                    if (IsEnabled(Preset.SAM_AoE_MeikyoShisui) && CanAoEMeikyo())
                        return MeikyoShisui;

                    if (IsEnabled(Preset.SAM_AoE_CDs_Ikishoten) &&
                        CanAoEIkishotenKenki(out uint kenkiAction))
                        return kenkiAction;
                }

                if (IsEnabled(Preset.SAM_AoE_Damage))
                {
                    if (IsEnabled(Preset.SAM_AoE_Zanshin) && CanAoEZanshin())
                        return Zanshin;

                    if (IsEnabled(Preset.SAM_AoE_Guren) && CanAoEGuren())
                        return Guren;

                    if (IsEnabled(Preset.SAM_AoE_Shoha) && CanAoEShoha())
                        return Shoha;
                }

                if (IsEnabled(Preset.SAM_AoE_Kyuten) &&
                    CanAoEKyuten(SAM_AoE_KenkiOvercapAmount))
                    return Kyuten;

                if (IsEnabled(Preset.SAM_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(SAM_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(SAM_AoE_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.SAM_AoE_StunInterrupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.SAM_AoE_Damage))
            {
                if (IsEnabled(Preset.SAM_AoE_OgiNamikiri) &&
                    CanAoEOgiNamikiri())
                    return OriginalHook(OgiNamikiri);

                if (IsEnabled(Preset.SAM_AoE_TenkaGoken) &&
                    CanAoESenGCD(out uint senAction, onlyWhenStationary: true))
                    return senAction;
            }

            return DoCombo(true, useOka: IsEnabled(Preset.SAM_AoE_Oka));
        }
    }

    internal class SAM_ST_YukikazeCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_YukikazeCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Yukikaze)
                return actionID;

            if (SAM_Yukaze_KenkiOvercap && CanWeave() &&
                Kenki >= SAM_Yukaze_KenkiOvercapAmount && LevelChecked(Shinten))
                return OriginalHook(Shinten);

            if (HasStatusEffect(Buffs.MeikyoShisui))
            {
                if (LevelChecked(Yukikaze) && !HasSetsu &&
                    (HasKa || !SAM_Yukaze_Gekko) &&
                    (HasGetsu || !SAM_Yukaze_Kasha))
                    return Yukikaze;

                if (SAM_Yukaze_Gekko &&
                    LevelChecked(Gekko) &&
                    ((OnTargetsRear() || OnTargetsFront()) && !HasGetsu ||
                     OnTargetsFlank() && HasKa ||
                     !HasStatusEffect(Buffs.Fugetsu) && !HasGetsu))
                    return Gekko;

                if (SAM_Yukaze_Kasha &&
                    LevelChecked(Kasha) &&
                    ((OnTargetsFlank() || OnTargetsFront()) && !HasKa ||
                     OnTargetsRear() && HasGetsu ||
                     !HasStatusEffect(Buffs.Fuka) && !HasKa))
                    return Kasha;
            }

            if (ComboTimer > 0)
            {
                if (ComboAction is Hakaze or Gyofu)
                {
                    if (LevelChecked(Yukikaze) &&
                        !HasSetsu &&
                        (SAM_ST_YukikazeCombo_Prio == 0 ||
                         (HasStatusEffect(Buffs.Fugetsu) || !SAM_Yukaze_Gekko) &&
                         (HasStatusEffect(Buffs.Fuka) || !SAM_Yukaze_Kasha)))
                        return Yukikaze;

                    if (SAM_Yukaze_Gekko &&
                        LevelChecked(Jinpu) &&
                        (!LevelChecked(Kasha) && LevelChecked(Gekko) ||
                         (OnTargetsRear() || OnTargetsFront()) && !HasGetsu && LevelChecked(Gekko) ||
                         HasKa && !HasGetsu && LevelChecked(Gekko) ||
                         SAM_ST_YukikazeCombo_Prio == 1 && !HasStatusEffect(Buffs.Fugetsu) ||
                         SenCount is 3 && ShouldRefreshFugetsu))
                        return Jinpu;

                    if (SAM_Yukaze_Kasha &&
                        LevelChecked(Shifu) &&
                        ((OnTargetsFlank() || OnTargetsFront()) && !HasKa && LevelChecked(Kasha) ||
                         HasGetsu && !HasKa && LevelChecked(Kasha) ||
                         SAM_ST_YukikazeCombo_Prio == 1 && !HasStatusEffect(Buffs.Fuka) ||
                         SenCount is 3 && ShouldRefreshFuka))
                        return Shifu;
                }

                if (SAM_Yukaze_Gekko &&
                    ComboAction is Jinpu && LevelChecked(Gekko))
                    return Gekko;

                if (SAM_Yukaze_Kasha &&
                    ComboAction is Shifu && LevelChecked(Kasha))
                    return Kasha;
            }

            return OriginalHook(Hakaze);
        }
    }

    internal class SAM_ST_KashaCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_KashaCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Kasha)
                return actionID;

            if (SAM_Kasha_KenkiOvercap && CanWeave() &&
                Kenki >= SAM_Kasha_KenkiOvercapAmount && LevelChecked(Shinten))
                return OriginalHook(Shinten);

            if (HasStatusEffect(Buffs.MeikyoShisui) && LevelChecked(Kasha))
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

    internal class SAM_ST_GekkoCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_GekkoCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Gekko)
                return actionID;

            if (SAM_Gekko_KenkiOvercap && CanWeave() &&
                Kenki >= SAM_Gekko_KenkiOvercapAmount && LevelChecked(Shinten))
                return OriginalHook(Shinten);

            if (HasStatusEffect(Buffs.MeikyoShisui) && LevelChecked(Gekko))
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

    internal class SAM_AoE_OkaCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_AoE_OkaCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Oka)
                return actionID;

            if (SAM_Oka_KenkiOvercap &&
                Kenki >= SAM_Oka_KenkiOvercapAmount &&
                LevelChecked(Kyuten) && CanWeave())
                return Kyuten;

            if (HasStatusEffect(Buffs.MeikyoShisui) ||
                ComboTimer > 0 && LevelChecked(Oka) &&
                ComboAction == OriginalHook(Fuko))
                return Oka;

            return OriginalHook(Fuko);
        }
    }

    internal class SAM_AoE_MangetsuCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_AoE_MangetsuCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Mangetsu)
                return actionID;

            if (SAM_Mangetsu_KenkiOvercap && Kenki >= SAM_Mangetsu_KenkiOvercapAmount &&
                LevelChecked(Kyuten) && CanWeave())
                return Kyuten;

            if (ComboTimer > 0 && ComboAction is Fuko or Fuga ||
                HasStatusEffect(Buffs.MeikyoShisui))
            {
                if (SAM_Mangetsu_Oka &&
                    LevelChecked(Oka) &&
                    (!HasKa ||
                     !HasStatusEffect(Buffs.Fuka) ||
                     SenCount is 2 or 3 && ShouldRefreshFuka))
                    return Oka;

                if (LevelChecked(Mangetsu) &&
                    HasStatusEffect(Buffs.Fuka) &&
                    (!HasGetsu ||
                     !SAM_Mangetsu_Oka ||
                     !HasStatusEffect(Buffs.Fugetsu) ||
                     !LevelChecked(Oka) ||
                     SenCount is 2 or 3 && ShouldRefreshFugetsu))
                    return Mangetsu;
            }

            return OriginalHook(Fuko);
        }
    }

    internal class SAM_MeikyoSens : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_MeikyoSens;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MeikyoShisui || !HasStatusEffect(Buffs.MeikyoShisui))
                return actionID;

            if (!HasStatusEffect(Buffs.Fugetsu) ||
                !HasGetsu)
                return Gekko;

            if (!HasStatusEffect(Buffs.Fuka) ||
                !HasKa)
                return Kasha;

            if (!HasSetsu)
                return Yukikaze;

            return actionID;
        }
    }

    internal class SAM_MeikyoShisuiProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_MeikyoShisuiProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MeikyoShisui)
                return actionID;

            return HasStatusEffect(Buffs.MeikyoShisui) &&
                   ActionReady(MeikyoShisui)
                ? All.SavageBlade
                : actionID;
        }
    }

    internal class SAM_Iaijutsu : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_Iaijutsu;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Iaijutsu)
                return actionID;

            bool canAddShoha = IsEnabled(Preset.SAM_Iaijutsu_Shoha) &&
                               ActionReady(Shoha) &&
                               MeditationStacks is 3;

            if (canAddShoha && CanWeave())
                return Shoha;

            if (IsEnabled(Preset.SAM_Iaijutsu_OgiNamikiri) &&
                (ActionReady(OriginalHook(OgiNamikiri)) && HasStatusEffect(Buffs.OgiNamikiriReady) || IsNamikiriReady))
                return OriginalHook(OgiNamikiri);

            if (IsEnabled(Preset.SAM_Iaijutsu_TsubameGaeshi) &&
                SenCount is not 1 &&
                (LevelChecked(TsubameGaeshi) &&
                 (HasStatusEffect(Buffs.TsubameReady) ||
                  HasStatusEffect(Buffs.KaeshiGokenReady)) ||
                 LevelChecked(TendoKaeshiSetsugekka) &&
                 (HasStatusEffect(Buffs.TendoKaeshiSetsugekkaReady) ||
                  HasStatusEffect(Buffs.TendoKaeshiGokenReady))))
                return OriginalHook(TsubameGaeshi);

            if (canAddShoha)
                return Shoha;

            return actionID;
        }
    }

    internal class SAM_Shinten : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_Shinten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Shinten)
                return actionID;

            if (IsEnabled(Preset.SAM_Shinten_Shoha) &&
                ActionReady(Shoha) &&
                MeditationStacks is 3)
                return Shoha;

            if (IsEnabled(Preset.SAM_Shinten_Ikishoten) &&
                ActionReady(Ikishoten) &&
                Gauge.Kenki < 50)
                return Ikishoten;

            if (IsEnabled(Preset.SAM_Shinten_Senei) &&
                ActionReady(Senei))
                return Senei;

            if (IsEnabled(Preset.SAM_Shinten_Zanshin) &&
                ActionReady(Zanshin) &&
                HasStatusEffect(Buffs.ZanshinReady))
                return Zanshin;

            return actionID;
        }
    }

    internal class SAM_Kyuten : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_Kyuten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Kyuten)
                return actionID;

            if (IsEnabled(Preset.SAM_Kyuten_Shoha) &&
                ActionReady(Shoha) &&
                MeditationStacks is 3)
                return Shoha;

            if (IsEnabled(Preset.SAM_Kyuten_Ikishoten) &&
                ActionReady(Ikishoten) &&
                Gauge.Kenki < 50)
                return Ikishoten;

            if (IsEnabled(Preset.SAM_Kyuten_Guren) &&
                ActionReady(Guren))
                return Guren;

            if (IsEnabled(Preset.SAM_Kyuten_Zanshin) &&
                ActionReady(Zanshin) &&
                HasStatusEffect(Buffs.ZanshinReady))
                return Zanshin;

            return actionID;
        }
    }

    internal class SAM_Ikishoten : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_Ikishoten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Ikishoten)
                return actionID;

            if (IsEnabled(Preset.SAM_Ikishoten_Shoha) &&
                ActionReady(Shoha) &&
                HasStatusEffect(Buffs.OgiNamikiriReady) &&
                MeditationStacks is 3)
                return Shoha;

            if (IsEnabled(Preset.SAM_Ikishoten_Namikiri) &&
                ActionReady(OriginalHook(OgiNamikiri)) &&
                (HasStatusEffect(Buffs.OgiNamikiriReady) || IsNamikiriReady))
                return OriginalHook(OgiNamikiri);

            return actionID;
        }
    }

    internal class SAM_GyotenYaten : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_GyotenYaten;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Gyoten)
                return actionID;

            if (Kenki >= 10)
            {
                if (InMeleeRange())
                    return Yaten;

                if (!InMeleeRange())
                    return Gyoten;
            }

            return actionID;
        }
    }

    internal class SAM_SeneiGuren : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_SeneiGuren;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Senei)
                return actionID;

            return !LevelChecked(Senei)
                ? Guren
                : actionID;
        }
    }

    internal class SAM_OgiShoha : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_OgiShoha;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not OgiNamikiri)
                return actionID;

            if (LevelChecked(Shoha) && MeditationStacks is 3)
                return Shoha;

            if (LevelChecked(OgiNamikiri) &&
                (HasStatusEffect(Buffs.OgiNamikiriReady) || IsNamikiriReady))
                return OriginalHook(OgiNamikiri);

            if (LevelChecked(Zanshin) &&
                SAM_OgiShohaZanshin && HasStatusEffect(Buffs.ZanshinReady))
                return Zanshin;

            return actionID;
        }
    }
}
