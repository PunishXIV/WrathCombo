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
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Hakaze, Gyofu))
                return actionID;

            if (UsePrepullMeikyo())
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseMeikyo(false))
                    return MeikyoShisui;

                if (UseSenei())
                    return Senei;

                if (!LevelChecked(Senei) && UseGuren())
                    return Guren;

                if (NeedKenkiRoomForIkishoten() && !ActionReady(Senei) && UseShinten())
                    return Shinten;

                if (UseIkishoten())
                    return Ikishoten;

                if (UseSenei())
                    return Senei;

                if (UseZanshin())
                    return Zanshin;

                if (UseShoha())
                    return Shoha;

                if (UseKenki(ref actionID, false))
                    return actionID;
            }

            if (UseTsubame(false))
                return OriginalHook(TsubameGaeshi);

            if (UseOgiNamikiri(false))
                return OriginalHook(OgiNamikiri);

            if (UseIaiJutsu(false))
                return OriginalHook(Iaijutsu);

            return HasStatusEffect(Buffs.MeikyoShisui)
                ? DoMeikyoCombo(actionID, false)
                : DoBasicCombo(false);
        }
    }

    internal class SAM_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Fuga, Fuko))
                return actionID;

            if (UsePrepullMeikyo())
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseMeikyo(true))
                    return MeikyoShisui;

                if (UseGuren())
                    return Guren;

                if (UseIkishoten())
                    return Ikishoten;

                if (UseZanshin())
                    return Zanshin;

                if (UseShoha())
                    return Shoha;

                if (UseKenki(ref actionID, true))
                    return actionID;
            }

            if (UseTsubame(true))
                return OriginalHook(TsubameGaeshi);

            if (UseOgiNamikiri(true))
                return OriginalHook(OgiNamikiri);

            if (UseIaiJutsu(true))
                return OriginalHook(Iaijutsu);

            return HasStatusEffect(Buffs.MeikyoShisui)
                ? DoMeikyoCombo(actionID, true)
                : DoBasicCombo(true);
        }
    }

    internal class SAM_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Hakaze, Gyofu))
                return actionID;

            if (IsEnabled(Preset.SAM_ST_Adv_Opener) &&
                Opener().FullOpener(ref actionID) &&
                HasBattleTarget())
                return actionID;

            if (IsEnabled(Preset.SAM_ST_Adv_CDs) &&
                IsEnabled(Preset.SAM_ST_Adv_Meikyo) &&
                UsePrepullMeikyo(requireNotJustUsed: true))
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.SAM_ST_Adv_CDs))
                {
                    if (IsEnabled(Preset.SAM_ST_Adv_Meikyo) &&
                        UseMeikyo(false, SAM_ST_MeikyoExecuteHP))
                        return MeikyoShisui;
                }

                if (IsEnabled(Preset.SAM_ST_Adv_Damage))
                {
                    if (IsEnabled(Preset.SAM_ST_Adv_Senei))
                    {
                        if (UseSenei())
                            return Senei;

                        if (SAM_ST_Senei_Guren &&
                            !LevelChecked(Senei) &&
                            UseGuren())
                            return Guren;
                    }
                }

                if (IsEnabled(Preset.SAM_ST_Adv_CDs))
                {
                    if (IsEnabled(Preset.SAM_ST_Adv_Shinten) &&
                        NeedKenkiRoomForIkishoten() &&
                        !ActionReady(Senei) &&
                        UseShinten(SAM_ST_ShintenExecuteHP, SAM_ST_ShintenKenkiOvercap))
                        return Shinten;

                    if (IsEnabled(Preset.SAM_ST_Adv_Ikishoten) &&
                        UseIkishoten())
                        return Ikishoten;
                }

                if (IsEnabled(Preset.SAM_ST_Adv_Damage))
                {
                    if (IsEnabled(Preset.SAM_ST_Adv_Senei) &&
                        UseSenei())
                        return Senei;

                    if (IsEnabled(Preset.SAM_ST_Adv_Zanshin) &&
                        UseZanshin())
                        return Zanshin;

                    if (IsEnabled(Preset.SAM_ST_Adv_Shoha) &&
                        UseShoha())
                        return Shoha;

                    if (IsEnabled(Preset.SAM_ST_Adv_Shinten) &&
                        UseShinten(SAM_ST_ShintenExecuteHP, SAM_ST_ShintenKenkiOvercap))
                        return Shinten;
                }

                if (IsEnabled(Preset.SAM_ST_Adv_Feint) &&
                    Role.CanFeint() &&
                    GroupDamageIncoming())
                    return Role.Feint;

                if (IsEnabled(Preset.SAM_ST_Adv_ThirdEye) &&
                    UseThirdEye())
                    return OriginalHook(ThirdEye);

                if (IsEnabled(Preset.SAM_ST_Adv_Meditate) &&
                    UseMeditate())
                    return Meditate;

                if (IsEnabled(Preset.SAM_ST_Adv_ComboHeals))
                {
                    if (Role.CanSecondWind(SAM_ST_SecondWindOption))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(SAM_ST_BloodbathOption))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.SAM_ST_Adv_StunInterrupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.SAM_ST_Adv_Damage))
            {
                if (IsEnabled(Preset.SAM_ST_Adv_Iaijutsu) &&
                    IsEnabled(Preset.SAM_ST_Adv_Tsubame) &&
                    UseTsubame(false))
                    return OriginalHook(TsubameGaeshi);

                if (IsEnabled(Preset.SAM_ST_Adv_OgiNamikiri) &&
                    UseOgiNamikiri(false, respectMovement: SAM_ST_OgiNamikiri_Movement))
                    return OriginalHook(OgiNamikiri);

                if (IsEnabled(Preset.SAM_ST_Adv_Iaijutsu) &&
                    UseIaiJutsu(
                        false,
                        useHiganbana: IsEnabled(Preset.SAM_ST_Adv_Higanbana),
                        useTenkaGoken: IsEnabled(Preset.SAM_ST_Adv_TenkaGoken),
                        useMidare: IsEnabled(Preset.SAM_ST_Adv_Midare),
                        onlyWhenStationary: IsEnabled(Preset.SAM_ST_Adv_Iaijutsu_Movement),
                        higanbanaHpThreshold: HiganbanaHPThreshold(),
                        higanbanaDotRefresh: SAM_ST_HiganbanaRefresh))
                    return OriginalHook(Iaijutsu);

                if (IsEnabled(Preset.SAM_ST_Adv_RangedUptime) &&
                    ActionReady(Enpi) && !InMeleeRange() && HasBattleTarget())
                    return Enpi;
            }

            return HasStatusEffect(Buffs.MeikyoShisui)
                ? DoMeikyoCombo(
                    actionID,
                    false,
                    useTrueNorth: IsEnabled(Preset.SAM_ST_Adv_TrueNorth),
                    useYukikaze: IsEnabled(Preset.SAM_ST_Adv_Yukikaze),
                    useKasha: IsEnabled(Preset.SAM_ST_Adv_Kasha),
                    useGekko: IsEnabled(Preset.SAM_ST_Adv_Gekko),
                    trueNorthCharges: SAM_ST_TrueNorthCharges)
                : DoBasicCombo(
                    false,
                    useTrueNorth: IsEnabled(Preset.SAM_ST_Adv_TrueNorth),
                    useYukikaze: IsEnabled(Preset.SAM_ST_Adv_Yukikaze),
                    useKasha: IsEnabled(Preset.SAM_ST_Adv_Kasha),
                    useGekko: IsEnabled(Preset.SAM_ST_Adv_Gekko),
                    trueNorthCharges: SAM_ST_TrueNorthCharges);
        }
    }

    internal class SAM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, Fuga, Fuko))
                return actionID;

            if (IsEnabled(Preset.SAM_AoE_Adv_CDs) &&
                IsEnabled(Preset.SAM_AoE_Adv_Meikyo) &&
                UsePrepullMeikyo(requireNotJustUsed: true))
                return MeikyoShisui;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.SAM_AoE_Adv_CDs))
                {
                    if (IsEnabled(Preset.SAM_AoE_Adv_Meikyo) &&
                        UseMeikyo(true))
                        return MeikyoShisui;

                    if (IsEnabled(Preset.SAM_AoE_Adv_Ikishoten) &&
                        UseIkishoten())
                        return Ikishoten;
                }

                if (IsEnabled(Preset.SAM_AoE_Adv_Damage))
                {
                    if (IsEnabled(Preset.SAM_AoE_Adv_Zanshin) &&
                        UseZanshin())
                        return Zanshin;

                    if (IsEnabled(Preset.SAM_AoE_Adv_Guren) &&
                        UseGuren())
                        return Guren;

                    if (IsEnabled(Preset.SAM_AoE_Adv_Shoha) &&
                        UseShoha())
                        return Shoha;
                }

                if (IsEnabled(Preset.SAM_AoE_Adv_Kyuten) &&
                    UseKyuten(SAM_AoE_KyutenKenkiOvercap))
                    return Kyuten;

                if (IsEnabled(Preset.SAM_AoE_Adv_ComboHeals))
                {
                    if (Role.CanSecondWind(SAM_AoE_SecondWindOption))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(SAM_AoE_BloodbathOption))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.SAM_AoE_Adv_StunInterrupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.SAM_AoE_Adv_Damage))
            {
                if (IsEnabled(Preset.SAM_AoE_Adv_TenkaGoken) &&
                    UseTsubame(true))
                    return OriginalHook(TsubameGaeshi);

                if (IsEnabled(Preset.SAM_AoE_Adv_OgiNamikiri) &&
                    UseOgiNamikiri(true))
                    return OriginalHook(OgiNamikiri);

                if (IsEnabled(Preset.SAM_AoE_Adv_TenkaGoken) &&
                    UseIaiJutsu(true))
                    return OriginalHook(Iaijutsu);
            }

            return HasStatusEffect(Buffs.MeikyoShisui)
                ? DoMeikyoCombo(actionID, true, useOka: IsEnabled(Preset.SAM_AoE_Adv_Oka))
                : DoBasicCombo(true, useOka: IsEnabled(Preset.SAM_AoE_Adv_Oka));
        }
    }

    internal class SAM_ST_YukikazeCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SAM_ST_YukikazeCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Yukikaze)
                return actionID;

            if (UseFeatureKenkiOvercap(ref actionID, SAM_Yukikaze_KenkiOvercap, SAM_Yukikaze_KenkiOvercapAmount, Shinten))
                return actionID;

            if (HasStatusEffect(Buffs.MeikyoShisui))
            {
                if (LevelChecked(Yukikaze) && !HasSetsu &&
                    (HasKa || !SAM_Yukikaze_Gekko) &&
                    (HasGetsu || !SAM_Yukikaze_Kasha))
                    return Yukikaze;

                if (SAM_Yukikaze_Gekko &&
                    LevelChecked(Gekko) &&
                    ((OnTargetsRear() || OnTargetsFront()) && !HasGetsu ||
                     OnTargetsFlank() && HasKa ||
                     !HasStatusEffect(Buffs.Fugetsu) && !HasGetsu))
                    return Gekko;

                if (SAM_Yukikaze_Kasha &&
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
                         (HasStatusEffect(Buffs.Fugetsu) || !SAM_Yukikaze_Gekko) &&
                         (HasStatusEffect(Buffs.Fuka) || !SAM_Yukikaze_Kasha)))
                        return Yukikaze;

                    if (SAM_Yukikaze_Gekko &&
                        LevelChecked(Jinpu) &&
                        (!LevelChecked(Gekko) ||
                         !LevelChecked(Kasha) && LevelChecked(Gekko) ||
                         (OnTargetsRear() || OnTargetsFront()) && !HasGetsu && LevelChecked(Gekko) ||
                         HasKa && !HasGetsu && LevelChecked(Gekko) ||
                         SAM_ST_YukikazeCombo_Prio == 1 && !HasStatusEffect(Buffs.Fugetsu) ||
                         SenCount is 3 && ShouldRefreshFugetsu))
                        return Jinpu;

                    if (SAM_Yukikaze_Kasha &&
                        LevelChecked(Shifu) &&
                        ((OnTargetsFlank() || OnTargetsFront()) && !HasKa && LevelChecked(Kasha) ||
                         HasGetsu && !HasKa && LevelChecked(Kasha) ||
                         SAM_ST_YukikazeCombo_Prio == 1 && !HasStatusEffect(Buffs.Fuka) ||
                         SenCount is 3 && ShouldRefreshFuka ||
                         !LevelChecked(Gekko)))
                        return Shifu;
                }

                if (SAM_Yukikaze_Gekko &&
                    ComboAction is Jinpu && LevelChecked(Gekko))
                    return Gekko;

                if (SAM_Yukikaze_Kasha &&
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

            if (UseFeatureKenkiOvercap(ref actionID, SAM_Kasha_KenkiOvercap, SAM_Kasha_KenkiOvercapAmount, Shinten))
                return actionID;

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

            if (UseFeatureKenkiOvercap(ref actionID, SAM_Gekko_KenkiOvercap, SAM_Gekko_KenkiOvercapAmount, Shinten))
                return actionID;

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

            if (UseFeatureKenkiOvercap(ref actionID, SAM_Oka_KenkiOvercap, SAM_Oka_KenkiOvercapAmount, Kyuten))
                return actionID;

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

            if (UseFeatureKenkiOvercap(ref actionID, SAM_Mangetsu_KenkiOvercap, SAM_Mangetsu_KenkiOvercapAmount, Kyuten))
                return actionID;

            if (ComboTimer > 0 && ComboAction is Fuko or Fuga ||
                HasStatusEffect(Buffs.MeikyoShisui))
                return DoMeikyoCombo(OriginalHook(Fuko), true, useOka: SAM_Mangetsu_Oka);

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
                ? All.Cease
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
                Kenki < 50)
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
                Kenki < 50)
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
                return InMeleeRange() ? Yaten : Gyoten;

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
