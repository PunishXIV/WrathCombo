using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.RPR.Config;
namespace WrathCombo.Combos.PvE;

internal partial class RPR : Melee
{
    internal class RPR_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Slice)
                return actionID;

            //Soulsow
            if (LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) &&
                !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //All Weaves
            if (CanWeave())
            {
                //Arcane Cirlce
                if (ActionReady(ArcaneCircle) &&
                    (LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) ||
                     !LevelChecked(Enshroud)))
                    return ArcaneCircle;

                //Enshroud
                if (UseEnshroud())
                    return Enshroud;

                //Gluttony/Bloodstalk
                if (Soul >= 50 &&
                    !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                    !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.PerfectioParata) &&
                    !IsComboExpiring(3))
                {
                    if (GetCooldownRemainingTime(Gluttony) <= GCD && Role.CanTrueNorth())
                        return Role.TrueNorth;

                    //Gluttony
                    if (LevelChecked(Gluttony) &&
                        GetCooldownRemainingTime(Gluttony) <= GCD / 2)
                        return Gluttony;

                    //Bloodstalk
                    if (LevelChecked(BloodStalk) &&
                        (!LevelChecked(Gluttony) ||
                         LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                         (Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 4)))
                        return OriginalHook(BloodStalk);
                }

                //Enshroud Weaves
                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    //Sacrificium
                    if (Lemure <= 4 && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    //Lemure's Slice
                    if (Void >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }

                //Healing
                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            //Ranged Attacks
            if (!InMeleeRange() && ActionReady(Harpe) && HasBattleTarget() &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.SoulReaver))
            {
                //Communio
                if (HasStatusEffect(Buffs.Enshrouded) && Lemure is 1 &&
                    LevelChecked(Communio))
                    return Communio;

                return HasStatusEffect(Buffs.Soulsow)
                    ? HarvestMoon
                    : Harpe;
            }

            //Shadow Of Death
            if (UseShadowOfDeath())
                return ShadowOfDeath;

            //Perfectio
            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            //Gibbet/Gallows
            if (LevelChecked(Gibbet) && !HasStatusEffect(Buffs.Enshrouded) &&
                (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
            {
                //Gibbet
                if (HasStatusEffect(Buffs.EnhancedGibbet))
                    return Role.CanTrueNorth() && !OnTargetsFlank()
                        ? Role.TrueNorth
                        : OriginalHook(Gibbet);

                //Gallows
                if (HasStatusEffect(Buffs.EnhancedGallows) ||
                    (!HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows)))
                    return Role.CanTrueNorth() && !OnTargetsRear()
                        ? Role.TrueNorth
                        : OriginalHook(Gallows);
            }

            //Plentiful Harvest
            if (!HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && HasStatusEffect(Buffs.ImmortalSacrifice) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            //Enshroud Combo
            if (HasStatusEffect(Buffs.Enshrouded))
            {
                //Communio
                if (Lemure is 1 && LevelChecked(Communio))
                    return Communio;

                //Void Reaping
                if (HasStatusEffect(Buffs.EnhancedVoidReaping))
                    return OriginalHook(Gibbet);

                //Cross Reaping
                if (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                    !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping))
                    return OriginalHook(Gallows);
            }

            //Soul Slice
            if (Soul <= 50 && ActionReady(SoulSlice) &&
                !IsComboExpiring(3) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.PerfectioParata) && !HasStatusEffect(Buffs.ImmortalSacrifice))
                return SoulSlice;

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                    return OriginalHook(WaxingSlice);

                if (ComboAction == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                    return OriginalHook(InfernalSlice);
            }

            return actionID;
        }
    }

    internal class RPR_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningScythe)
                return actionID;

            //Soulsow
            if (LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (ActionReady(ArcaneCircle))
                    return ArcaneCircle;

                if (!HasStatusEffect(Buffs.SoulReaver) &&
                    !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.Executioner) &&
                    ActionReady(Enshroud) &&
                    !IsComboExpiring(6) &&
                    (Shroud >= 50 || HasStatusEffect(Buffs.IdealHost)))
                    return Enshroud;

                if (LevelChecked(Gluttony) && Soul >= 50 && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    GetCooldownRemainingTime(Gluttony) <= GCD)
                    return Gluttony;

                if (LevelChecked(GrimSwathe) && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    !HasStatusEffect(Buffs.Executioner) && Soul >= 50 &&
                    (!LevelChecked(Gluttony) || LevelChecked(Gluttony) &&
                        (Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5)))
                    return GrimSwathe;

                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    if (Lemure is 2 && Void is 1 && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (Void >= 2 && LevelChecked(LemuresScythe))
                        return OriginalHook(GrimSwathe);
                }

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (LevelChecked(WhorlOfDeath) &&
                CanApplyStatus(CurrentTarget, Debuffs.DeathsDesign) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < 6 &&
                !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Executioner))
                return WhorlOfDeath;

            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (HasStatusEffect(Buffs.ImmortalSacrifice) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.Executioner) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            if (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))
                return OriginalHook(Guillotine);

            if (HasStatusEffect(Buffs.Enshrouded))
            {
                if (LevelChecked(Communio) &&
                    Lemure is 1 && Void is 0)
                    return Communio;

                if (Lemure > 0)
                    return OriginalHook(Guillotine);
            }

            if (!HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.PerfectioParata) &&
                ActionReady(SoulScythe) && Soul <= 50)
                return SoulScythe;

            return ComboAction == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                ? OriginalHook(NightmareScythe)
                : actionID;
        }
    }

    internal class RPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Slice)
                return actionID;

            int positionalChoice = RPR_Positional;

            //RPR Opener
            if (IsEnabled(Preset.RPR_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            //Soulsow
            if (IsEnabled(Preset.RPR_ST_SoulSow) &&
                LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            //All Weaves
            if (CanWeave())
            {
                //Arcane Cirlce
                if (IsEnabled(Preset.RPR_ST_ArcaneCircle) &&
                    ActionReady(ArcaneCircle) &&
                    (LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) ||
                     !LevelChecked(Enshroud)) &&
                    (RPR_ST_ArcaneCircleBossOption == 0 || InBossEncounter()))
                    return ArcaneCircle;

                //Enshroud
                if (IsEnabled(Preset.RPR_ST_Enshroud) &&
                    UseEnshroud())
                    return Enshroud;

                //Gluttony/Bloodstalk
                if (Soul >= 50 &&
                    !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                    !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.PerfectioParata) &&
                    !IsComboExpiring(3))
                {
                    if (IsEnabled(Preset.RPR_ST_TrueNorthDynamic) &&
                        GetCooldownRemainingTime(Gluttony) <= GCD && Role.CanTrueNorth())
                        return Role.TrueNorth;

                    //Gluttony
                    if (IsEnabled(Preset.RPR_ST_Gluttony) &&
                        LevelChecked(Gluttony) &&
                        GetCooldownRemainingTime(Gluttony) <= GCD / 2)
                        return Gluttony;

                    //Bloodstalk
                    if (IsEnabled(Preset.RPR_ST_Bloodstalk) &&
                        LevelChecked(BloodStalk) &&
                        (!LevelChecked(Gluttony) ||
                         LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                         (Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 4)))
                        return OriginalHook(BloodStalk);
                }

                //Enshroud Weaves
                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    //Sacrificium
                    if (IsEnabled(Preset.RPR_ST_Sacrificium) &&
                        Lemure <= 4 && HasStatusEffect(Buffs.Oblatio) &&
                        ((GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 && !JustUsed(ArcaneCircle, 2) &&
                          (RPR_ST_ArcaneCircleBossOption == 0 ||
                           InBossEncounter() ||
                           RPR_ST_ArcaneCircleBossOption == 1 && !InBossEncounter() && IsOffCooldown(ArcaneCircle))) ||
                         IsNotEnabled(Preset.RPR_ST_ArcaneCircle)))
                        return OriginalHook(Gluttony);

                    //Lemure's Slice
                    if (IsEnabled(Preset.RPR_ST_Lemure) &&
                        Void >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }

                if (IsEnabled(Preset.RPR_ST_Feint) &&
                    Role.CanFeint() &&
                    RaidWideCasting())
                    return Role.Feint;

                //Healing
                if (IsEnabled(Preset.RPR_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(RPR_STSecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(RPR_STBloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.RPR_ST_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep() &&
                    !TargetIsBoss() && TargetIsCasting())
                    return Role.LegSweep;
            }

            //Ranged Attacks
            if (IsEnabled(Preset.RPR_ST_RangedFiller) &&
                ActionReady(Harpe) && !InMeleeRange() && HasBattleTarget() &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.SoulReaver))
            {
                //Communio
                if (HasStatusEffect(Buffs.Enshrouded) && Lemure is 1 &&
                    LevelChecked(Communio))
                    return Communio;

                return RPR_ST_RangedFillerHarvestMoon &&
                       HasStatusEffect(Buffs.Soulsow)
                    ? HarvestMoon
                    : Harpe;
            }

            //Shadow Of Death
            if (IsEnabled(Preset.RPR_ST_SoD) &&
                UseShadowOfDeath() && GetTargetHPPercent() > RPR_SoDHPThreshold)
                return ShadowOfDeath;

            //Perfectio
            if (IsEnabled(Preset.RPR_ST_Perfectio) &&
                HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            //Gibbet/Gallows
            if (IsEnabled(Preset.RPR_ST_GibbetGallows) &&
                LevelChecked(Gibbet) && !HasStatusEffect(Buffs.Enshrouded) &&
                (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
            {
                //Gibbet
                if (HasStatusEffect(Buffs.EnhancedGibbet) ||
                    (positionalChoice is 1 && !HasStatusEffect(Buffs.EnhancedGibbet) &&
                     !HasStatusEffect(Buffs.EnhancedGallows)))
                {
                    return IsEnabled(Preset.RPR_ST_TrueNorthDynamic) &&
                           (RPR_ST_TrueNorthDynamic_HoldCharge &&
                            GetRemainingCharges(Role.TrueNorth) < 2 ||
                            !RPR_ST_TrueNorthDynamic_HoldCharge) &&
                           Role.CanTrueNorth() && !OnTargetsFlank()
                        ? Role.TrueNorth
                        : OriginalHook(Gibbet);
                }

                //Gallows
                if (HasStatusEffect(Buffs.EnhancedGallows) ||
                    (positionalChoice is 0 && !HasStatusEffect(Buffs.EnhancedGibbet) &&
                     !HasStatusEffect(Buffs.EnhancedGallows)))
                {
                    return IsEnabled(Preset.RPR_ST_TrueNorthDynamic) &&
                           (RPR_ST_TrueNorthDynamic_HoldCharge &&
                            GetRemainingCharges(Role.TrueNorth) < 2 ||
                            !RPR_ST_TrueNorthDynamic_HoldCharge) &&
                           Role.CanTrueNorth() && !OnTargetsRear()
                        ? Role.TrueNorth
                        : OriginalHook(Gallows);
                }
            }

            //Plentiful Harvest
            if (IsEnabled(Preset.RPR_ST_PlentifulHarvest) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && HasStatusEffect(Buffs.ImmortalSacrifice) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            //Enshroud Combo
            if (HasStatusEffect(Buffs.Enshrouded))
            {
                //Communio
                if (IsEnabled(Preset.RPR_ST_Communio) &&
                    Lemure is 1 && LevelChecked(Communio))
                    return Communio;

                //Void Reaping
                if (IsEnabled(Preset.RPR_ST_Reaping) &&
                    HasStatusEffect(Buffs.EnhancedVoidReaping))
                    return OriginalHook(Gibbet);

                //Cross Reaping
                if (IsEnabled(Preset.RPR_ST_Reaping) &&
                    (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                     !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping)))
                    return OriginalHook(Gallows);
            }

            //Soul Slice
            if (IsEnabled(Preset.RPR_ST_SoulSlice) &&
                Soul <= 50 && ActionReady(SoulSlice) &&
                !IsComboExpiring(3) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.IdealHost) && !HasStatusEffect(Buffs.Executioner) &&
                !HasStatusEffect(Buffs.PerfectioParata) && !HasStatusEffect(Buffs.ImmortalSacrifice))
                return SoulSlice;

            //1-2-3 Combo
            if (ComboTimer > 0)
            {
                if (ComboAction == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                    return OriginalHook(WaxingSlice);

                if (ComboAction == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                    return OriginalHook(InfernalSlice);
            }
            return actionID;
        }
    }

    internal class RPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningScythe)
                return actionID;

            //Soulsow
            if (IsEnabled(Preset.RPR_AoE_SoulSow) &&
                LevelChecked(Soulsow) &&
                !HasStatusEffect(Buffs.Soulsow) && !PartyInCombat())
                return Soulsow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (IsEnabled(Preset.RPR_AoE_ArcaneCircle) &&
                    ActionReady(ArcaneCircle))
                    return ArcaneCircle;

                if (IsEnabled(Preset.RPR_AoE_Enshroud) &&
                    !HasStatusEffect(Buffs.SoulReaver) &&
                    !HasStatusEffect(Buffs.Enshrouded) &&
                    !IsComboExpiring(6) &&
                    ActionReady(Enshroud) &&
                    (Shroud >= 50 || HasStatusEffect(Buffs.IdealHost)))
                    return Enshroud;

                if (IsEnabled(Preset.RPR_AoE_Gluttony) &&
                    LevelChecked(Gluttony) && Soul >= 50 && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    GetCooldownRemainingTime(Gluttony) <= GCD)
                    return Gluttony;

                if (IsEnabled(Preset.RPR_AoE_GrimSwathe) &&
                    LevelChecked(GrimSwathe) && !HasStatusEffect(Buffs.Enshrouded) &&
                    !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.ImmortalSacrifice) &&
                    Soul >= 50 &&
                    (!LevelChecked(Gluttony) ||
                     (LevelChecked(Gluttony) && (Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 5))))
                    return GrimSwathe;

                if (HasStatusEffect(Buffs.Enshrouded))
                {
                    if (IsEnabled(Preset.RPR_AoE_Sacrificium) &&
                        Lemure is 2 && Void is 1 && HasStatusEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(Preset.RPR_AoE_Lemure) &&
                        Void >= 2 && LevelChecked(LemuresScythe))
                        return OriginalHook(GrimSwathe);
                }

                if (IsEnabled(Preset.RPR_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(RPR_AoESecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(RPR_AoEBloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.RPR_AoE_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep() &&
                    !TargetIsBoss() && TargetIsCasting())
                    return Role.LegSweep;
            }

            if (IsEnabled(Preset.RPR_AoE_WoD) &&
                ActionReady(WhorlOfDeath) &&
                CanApplyStatus(CurrentTarget, Debuffs.DeathsDesign) &&
                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < 6 &&
                !HasStatusEffect(Buffs.SoulReaver) &&
                GetTargetHPPercent() > RPR_WoDHPThreshold)
                return WhorlOfDeath;

            if (IsEnabled(Preset.RPR_AoE_Perfectio) &&
                HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (IsEnabled(Preset.RPR_AoE_PlentifulHarvest) &&
                HasStatusEffect(Buffs.ImmortalSacrifice) &&
                !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Enshrouded) &&
                (GetStatusEffectRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                return PlentifulHarvest;

            if (IsEnabled(Preset.RPR_AoE_Guillotine) &&
                (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) &&
                !HasStatusEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))
                return OriginalHook(Guillotine);

            if (HasStatusEffect(Buffs.Enshrouded))
            {
                if (IsEnabled(Preset.RPR_AoE_Communio) &&
                    LevelChecked(Communio) &&
                    Lemure is 1 && Void is 0)
                    return Communio;

                if (IsEnabled(Preset.RPR_AoE_Reaping) &&
                    Lemure > 0)
                    return OriginalHook(Guillotine);
            }

            if (IsEnabled(Preset.RPR_AoE_SoulScythe) &&
                !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver) &&
                !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.PerfectioParata) &&
                ActionReady(SoulScythe) && Soul <= 50)
                return SoulScythe;

            return ComboAction == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                ? OriginalHook(NightmareScythe)
                : actionID;
        }
    }

    internal class RPR_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not InfernalSlice)
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is Slice && LevelChecked(WaxingSlice))
                    return WaxingSlice;

                if (ComboAction is WaxingSlice && LevelChecked(InfernalSlice))
                    return InfernalSlice;
            }

            return Slice;
        }
    }

    internal class RPR_GluttonyBloodSwathe : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_GluttonyBloodSwathe;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (BloodStalk or GrimSwathe))
                return actionID;

            switch (actionID)
            {
                case GrimSwathe:
                    {
                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_OGCD))
                        {
                            if (Shroud >= 50 || HasStatusEffect(Buffs.IdealHost))
                                return Enshroud;

                            if (HasStatusEffect(Buffs.Enshrouded))
                            {
                                //Sacrificium
                                if (Lemure is 2 && HasStatusEffect(Buffs.Oblatio))
                                    return OriginalHook(Gluttony);

                                //Lemure's Slice
                                if (Void >= 2 && LevelChecked(LemuresScythe))
                                    return OriginalHook(GrimSwathe);
                            }
                        }

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud))
                        {
                            if (HasStatusEffect(Buffs.PerfectioParata))
                                return OriginalHook(Communio);

                            if (HasStatusEffect(Buffs.Enshrouded))
                            {
                                switch (Lemure)
                                {
                                    case 1 when Void == 0 && LevelChecked(Communio):
                                        return Communio;

                                    case 2 when Void is 1 && HasStatusEffect(Buffs.Oblatio):
                                        return OriginalHook(Gluttony);
                                }

                                if (Void >= 2 && LevelChecked(LemuresScythe))
                                    return OriginalHook(GrimSwathe);

                                if (Lemure > 1)
                                    return OriginalHook(Guillotine);
                            }
                        }

                        if (ActionReady(Gluttony) && !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver))
                            return Gluttony;

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                            HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio))
                            return OriginalHook(Gluttony);

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                            (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)) && LevelChecked(Guillotine))
                            return Guillotine;

                        break;
                    }


                case BloodStalk:
                    {
                        if (IsEnabled(Preset.RPR_TrueNorthGluttony) && Role.CanTrueNorth() &&
                            (GetStatusEffectStacks(Buffs.SoulReaver) is 2 || HasStatusEffect(Buffs.Executioner)))
                            return Role.TrueNorth;

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_OGCD))
                        {
                            if (Shroud >= 50 || HasStatusEffect(Buffs.IdealHost))
                                return Enshroud;

                            if (HasStatusEffect(Buffs.Enshrouded))
                            {
                                //Sacrificium
                                if (Lemure is 2 && HasStatusEffect(Buffs.Oblatio))
                                    return OriginalHook(Gluttony);

                                //Lemure's Slice
                                if (Void >= 2 && LevelChecked(LemuresSlice))
                                    return OriginalHook(BloodStalk);
                            }
                        }

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Enshroud))
                        {
                            if (HasStatusEffect(Buffs.PerfectioParata))
                                return OriginalHook(Communio);

                            if (HasStatusEffect(Buffs.Enshrouded))
                            {
                                switch (Lemure)
                                {
                                    case 1 when Void == 0 && LevelChecked(Communio):
                                        return Communio;

                                    case 2 when Void is 1 && HasStatusEffect(Buffs.Oblatio):
                                        return OriginalHook(Gluttony);
                                }

                                if (Void >= 2 && LevelChecked(LemuresSlice))
                                    return OriginalHook(BloodStalk);

                                if (HasStatusEffect(Buffs.EnhancedVoidReaping))
                                    return OriginalHook(Gibbet);

                                if (HasStatusEffect(Buffs.EnhancedCrossReaping) ||
                                    !HasStatusEffect(Buffs.EnhancedCrossReaping) && !HasStatusEffect(Buffs.EnhancedVoidReaping))
                                    return OriginalHook(Gallows);
                            }
                        }

                        if (ActionReady(Gluttony) && !HasStatusEffect(Buffs.Enshrouded) && !HasStatusEffect(Buffs.SoulReaver))
                            return Gluttony;

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                            HasStatusEffect(Buffs.Enshrouded) && HasStatusEffect(Buffs.Oblatio))
                            return OriginalHook(Gluttony);

                        if (IsEnabled(Preset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                            (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner)))
                        {
                            if (HasStatusEffect(Buffs.EnhancedGibbet))
                                return OriginalHook(Gibbet);

                            if (HasStatusEffect(Buffs.EnhancedGallows) ||
                                !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                                return OriginalHook(Gallows);
                        }

                        break;
                    }
            }

            return actionID;
        }
    }

    internal class RPR_Soulsow : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_Soulsow;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Harpe or Slice or SpinningScythe) &&
                actionID is not (ShadowOfDeath or BloodStalk))
                return actionID;

            bool[] soulSowOptions = RPR_SoulsowOptions;
            bool soulsowReady = ActionReady(Soulsow) && !HasStatusEffect(Buffs.Soulsow);

            return soulSowOptions.Length > 0 &&
                   (actionID is Harpe && soulSowOptions[0] ||
                    actionID is Slice && soulSowOptions[1] ||
                    actionID is SpinningScythe && soulSowOptions[2] ||
                    actionID is ShadowOfDeath && soulSowOptions[3] ||
                    actionID is BloodStalk && soulSowOptions[4]) && soulsowReady && !InCombat() ||
                   IsEnabled(Preset.RPR_Soulsow_Combat) && actionID is Harpe && !HasBattleTarget()
                ? Soulsow
                : actionID;
        }
    }

    internal class RPR_ArcaneCirclePlentifulHarvest : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_ArcaneCirclePlentifulHarvest;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ArcaneCircle)
                return actionID;

            return HasStatusEffect(Buffs.ImmortalSacrifice) &&
                   LevelChecked(PlentifulHarvest)
                ? PlentifulHarvest
                : actionID;
        }
    }

    internal class RPR_Regress : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_Regress;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HellsEgress or HellsIngress))
                return actionID;

            return GetStatusEffect(Buffs.Threshold)?.RemainingTime <= 9
                ? Regress
                : actionID;
        }
    }

    internal class RPR_EnshroudProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_EnshroudProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Enshroud)
                return actionID;

            if (IsEnabled(Preset.RPR_TrueNorthEnshroud) &&
                (GetStatusEffectStacks(Buffs.SoulReaver) is 2 || HasStatusEffect(Buffs.Executioner)) &&
                Role.CanTrueNorth())
                return Role.TrueNorth;

            if (HasStatusEffect(Buffs.SoulReaver) || HasStatusEffect(Buffs.Executioner))
            {
                if (HasStatusEffect(Buffs.EnhancedGibbet))
                    return OriginalHook(Gibbet);

                if (HasStatusEffect(Buffs.EnhancedGallows) ||
                    !HasStatusEffect(Buffs.EnhancedGibbet) && !HasStatusEffect(Buffs.EnhancedGallows))
                    return OriginalHook(Gallows);
            }

            return actionID;
        }
    }

    internal class RPR_EnshroudCommunio : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_EnshroudCommunio;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Enshroud)
                return actionID;

            if (HasStatusEffect(Buffs.PerfectioParata))
                return OriginalHook(Communio);

            if (HasStatusEffect(Buffs.Enshrouded))
                return Communio;

            return actionID;
        }
    }

    internal class RPR_CommunioOnGGG : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPR_CommunioOnGGG;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Gibbet or Gallows or Guillotine))
                return actionID;

            switch (actionID)
            {
                case Gibbet or Gallows when HasStatusEffect(Buffs.Enshrouded):
                    {
                        if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                            return Communio;

                        if (IsEnabled(Preset.RPR_LemureOnGGG) &&
                            Void >= 2 && LevelChecked(LemuresSlice) && CanWeave())
                            return OriginalHook(BloodStalk);

                        break;
                    }

                case Guillotine when HasStatusEffect(Buffs.Enshrouded):
                    {
                        if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                            return Communio;

                        if (IsEnabled(Preset.RPR_LemureOnGGG) &&
                            Void >= 2 && LevelChecked(LemuresScythe) && CanWeave())
                            return OriginalHook(GrimSwathe);

                        break;
                    }
            }

            return actionID;
        }
    }
}
