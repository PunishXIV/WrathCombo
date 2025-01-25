using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal class MNK_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bootshine or LeapingOpo))
                return actionID;

            if ((!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                !HasEffect(Buffs.RiddleOfFire) &&
                LevelChecked(SteeledMeditation))
                return OriginalHook(SteeledMeditation);

            if (!InCombat() && LevelChecked(FormShift) &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance))
                return FormShift;

            //Variant Cure
            if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                return Variant.VariantCure;

            if (ActionReady(RiddleOfFire) &&
                !HasEffect(Buffs.FiresRumination) &&
                CanDelayedWeave())
                return RiddleOfFire;

            // OGCDs
            if (CanWeave())
            {
                //Variant Rampart
                if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (ActionReady(Brotherhood))
                    return Brotherhood;

                if (ActionReady(RiddleOfWind) &&
                    !HasEffect(Buffs.WindsRumination))
                    return RiddleOfWind;

                //Perfect Balance
                if (UsePerfectBalance())
                    return PerfectBalance;

                if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                    return All.Bloodbath;

                if (Gauge.Chakra >= 5 && InCombat() && LevelChecked(SteeledMeditation))
                    return OriginalHook(SteeledMeditation);
            }

            // GCDs
            if (HasEffect(Buffs.FormlessFist))
                return Gauge.OpoOpoFury == 0
                    ? DragonKick
                    : OriginalHook(Bootshine);

            // Masterful Blitz
            if (LevelChecked(MasterfulBlitz) &&
                !HasEffect(Buffs.PerfectBalance) &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            // Perfect Balance
            if (HasEffect(Buffs.PerfectBalance))
            {
                #region Open Lunar

                if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
                    return Gauge.OpoOpoFury == 0
                        ? DragonKick
                        : OriginalHook(Bootshine);

                #endregion

                #region Open Solar

                if (!SolarNadi && !BothNadisOpen)
                {
                    if (CoeurlChakra == 0)
                        return Gauge.CoeurlFury == 0
                            ? Demolish
                            : OriginalHook(SnapPunch);

                    if (RaptorChakra == 0)
                        return Gauge.RaptorFury == 0
                            ? TwinSnakes
                            : OriginalHook(TrueStrike);

                    if (OpoOpoChakra == 0)
                        return Gauge.OpoOpoFury == 0
                            ? DragonKick
                            : OriginalHook(Bootshine);
                }

                #endregion
            }

            if (HasEffect(Buffs.FiresRumination) &&
                !HasEffect(Buffs.PerfectBalance) &&
                !HasEffect(Buffs.FormlessFist) &&
                (JustUsed(OriginalHook(Bootshine)) ||
                 JustUsed(DragonKick) ||
                 GetBuffRemainingTime(Buffs.FiresRumination) < 4))
                return FiresReply;

            if (HasEffect(Buffs.WindsRumination) &&
                LevelChecked(WindsReply) &&
                HasEffect(Buffs.RiddleOfWind) &&
                GetBuffRemainingTime(Buffs.WindsRumination) < 4)
                return WindsReply;

            // Standard Beast Chakras
            return DetermineCoreAbility(actionID, true);
        }
    }

    internal class MNK_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bootshine or LeapingOpo))
                return actionID;

            if (IsEnabled(CustomComboPreset.MNK_STUseMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                !HasEffect(Buffs.RiddleOfFire) &&
                LevelChecked(SteeledMeditation))
                return OriginalHook(SteeledMeditation);

            if (IsEnabled(CustomComboPreset.MNK_STUseFormShift) &&
                !InCombat() && LevelChecked(FormShift) &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance))
                return FormShift;

            if (IsEnabled(CustomComboPreset.MNK_STUseOpener))
                if (Opener().FullOpener(ref actionID))
                {
                    if (IsOnCooldown(RiddleOfWind) &&
                        CanWeave() && Gauge.Chakra >= 5)
                        return TheForbiddenChakra;

                    return actionID;
                }

            //Variant Cure
            if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.MNK_STUseBuffs) &&
                IsEnabled(CustomComboPreset.MNK_STUseROF) &&
                !HasEffect(Buffs.FiresRumination) &&
                ActionReady(RiddleOfFire) &&
                CanDelayedWeave() &&
                GetTargetHPPercent() >= Config.MNK_ST_RiddleOfFire_HP)
                return RiddleOfFire;

            // OGCDs
            if (CanWeave())
            {
                //Variant Rampart
                if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_STUseBrotherhood) &&
                        ActionReady(Brotherhood) &&
                        GetTargetHPPercent() >= Config.MNK_ST_Brotherhood_HP)
                        return Brotherhood;

                    if (IsEnabled(CustomComboPreset.MNK_STUseROW) &&
                        !HasEffect(Buffs.WindsRumination) &&
                        ActionReady(RiddleOfWind) &&
                        GetTargetHPPercent() >= Config.MNK_ST_RiddleOfWind_HP)
                        return RiddleOfWind;
                }

                //Perfect Balance
                if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance) &&
                    UsePerfectBalance())
                    return PerfectBalance;

                if (IsEnabled(CustomComboPreset.MNK_ST_ComboHeals))
                {
                    if (PlayerHealthPercentageHp() <= Config.MNK_ST_SecondWind_Threshold && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= Config.MNK_ST_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                        return All.Bloodbath;
                }

                if (IsEnabled(CustomComboPreset.MNK_STUseTheForbiddenChakra) &&
                    Gauge.Chakra >= 5 && InCombat() &&
                    LevelChecked(SteeledMeditation))
                    return OriginalHook(SteeledMeditation);
            }

            // GCDs
            if (HasEffect(Buffs.FormlessFist))
                return Gauge.OpoOpoFury == 0
                    ? DragonKick
                    : OriginalHook(Bootshine);

            // Masterful Blitz
            if (IsEnabled(CustomComboPreset.MNK_STUseMasterfulBlitz) &&
                LevelChecked(MasterfulBlitz) &&
                !HasEffect(Buffs.PerfectBalance) &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            // Perfect Balance
            if (HasEffect(Buffs.PerfectBalance))
            {
                #region Open Lunar

                if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
                    return Gauge.OpoOpoFury == 0
                        ? DragonKick
                        : OriginalHook(Bootshine);

                #endregion

                #region Open Solar

                if (!SolarNadi && !BothNadisOpen)
                {
                    if (CoeurlChakra == 0)
                        return Gauge.CoeurlFury == 0
                            ? Demolish
                            : OriginalHook(SnapPunch);

                    if (RaptorChakra == 0)
                        return Gauge.RaptorFury == 0
                            ? TwinSnakes
                            : OriginalHook(TrueStrike);

                    if (OpoOpoChakra == 0)
                        return Gauge.OpoOpoFury == 0
                            ? DragonKick
                            : OriginalHook(Bootshine);
                }

                #endregion
            }

            if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
            {
                if (IsEnabled(CustomComboPreset.MNK_STUseROF) &&
                    IsEnabled(CustomComboPreset.MNK_STUseFiresReply) &&
                    HasEffect(Buffs.FiresRumination) &&
                    !HasEffect(Buffs.PerfectBalance) &&
                    !HasEffect(Buffs.FormlessFist) &&
                    (JustUsed(OriginalHook(Bootshine)) ||
                     JustUsed(DragonKick) ||
                     GetBuffRemainingTime(Buffs.FiresRumination) < 4))
                    return FiresReply;

                if (IsEnabled(CustomComboPreset.MNK_STUseROW) &&
                    IsEnabled(CustomComboPreset.MNK_STUseWindsReply) &&
                    HasEffect(Buffs.WindsRumination) &&
                    LevelChecked(WindsReply) &&
                    HasEffect(Buffs.RiddleOfWind) &&
                    GetBuffRemainingTime(Buffs.WindsRumination) < 4)
                    return WindsReply;
            }

            // Standard Beast Chakras
            return DetermineCoreAbility(actionID, IsEnabled(CustomComboPreset.MNK_STUseTrueNorth));
        }
    }

    internal class MNK_AOE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ArmOfTheDestroyer or ShadowOfTheDestroyer))
                return actionID;

            if (!InCombat() && Gauge.Chakra < 5 &&
                LevelChecked(InspiritedMeditation))
                return OriginalHook(InspiritedMeditation);

            if (!InCombat() && LevelChecked(FormShift) &&
                !HasEffect(Buffs.FormlessFist) &&
                !HasEffect(Buffs.PerfectBalance))
                return FormShift;

            //Variant Cure
            if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                return Variant.VariantCure;

            if (ActionReady(RiddleOfFire) &&
                !HasEffect(Buffs.FiresRumination) &&
                CanDelayedWeave())
                return RiddleOfFire;

            // Buffs
            if (CanWeave())
            {
                //Variant Rampart
                if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (ActionReady(Brotherhood))
                    return Brotherhood;

                if (ActionReady(RiddleOfWind) &&
                    !HasEffect(Buffs.WindsRumination))
                    return RiddleOfWind;

                if (ActionReady(PerfectBalance) &&
                    !HasEffect(Buffs.PerfectBalance) &&
                    (GetRemainingCharges(PerfectBalance) == GetMaxCharges(PerfectBalance) ||
                     GetCooldownRemainingTime(PerfectBalance) <= 4 ||
                     HasEffect(Buffs.Brotherhood) ||
                     HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10 ||
                     GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                    return PerfectBalance;

                if (Gauge.Chakra >= 5 &&
                    LevelChecked(InspiritedMeditation) &&
                    HasBattleTarget() && InCombat())
                    return OriginalHook(InspiritedMeditation);

                if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                    return All.Bloodbath;
            }

            if (LevelChecked(FiresReply) &&
                HasEffect(Buffs.FiresRumination) &&
                !HasEffect(Buffs.PerfectBalance) &&
                !HasEffect(Buffs.FormlessFist))
                return FiresReply;

            if (HasEffect(Buffs.WindsRumination) &&
                LevelChecked(WindsReply) &&
                HasEffect(Buffs.RiddleOfWind) &&
                GetBuffRemainingTime(Buffs.WindsRumination) < 4)
                return WindsReply;

            // Masterful Blitz
            if (LevelChecked(MasterfulBlitz) && !HasEffect(Buffs.PerfectBalance) &&
                OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                return OriginalHook(MasterfulBlitz);

            // Perfect Balance
            if (HasEffect(Buffs.PerfectBalance))
            {
                #region Open Lunar

                if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
                    return LevelChecked(ShadowOfTheDestroyer)
                        ? ShadowOfTheDestroyer
                        : Rockbreaker;

                #endregion

                #region Open Solar

                if (!SolarNadi && !BothNadisOpen)
                    switch (GetBuffStacks(Buffs.PerfectBalance))
                    {
                        case 3:
                            return OriginalHook(ArmOfTheDestroyer);

                        case 2:
                            return FourPointFury;

                        case 1:
                            return Rockbreaker;
                    }

                #endregion
            }

            // Monk Rotation
            if (HasEffect(Buffs.OpoOpoForm))
                return OriginalHook(ArmOfTheDestroyer);

            if (HasEffect(Buffs.RaptorForm))
            {
                if (LevelChecked(FourPointFury))
                    return FourPointFury;

                if (LevelChecked(TwinSnakes))
                    return TwinSnakes;
            }

            if (HasEffect(Buffs.CoeurlForm) && LevelChecked(Rockbreaker))
                return Rockbreaker;

            return actionID;
        }
    }

    internal class MNK_AOE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_AOE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ArmOfTheDestroyer or ShadowOfTheDestroyer))
                return actionID;

            if (IsEnabled(CustomComboPreset.MNK_AoEUseMeditation) &&
                !InCombat() && Gauge.Chakra < 5 &&
                LevelChecked(InspiritedMeditation))
                return OriginalHook(InspiritedMeditation);

            if (IsEnabled(CustomComboPreset.MNK_AoEUseFormShift) &&
                !InCombat() && LevelChecked(FormShift) &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance))
                return FormShift;

            //Variant Cure
            if (IsEnabled(CustomComboPreset.MNK_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.MNK_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs) &&
                IsEnabled(CustomComboPreset.MNK_AoEUseROF) &&
                !HasEffect(Buffs.FiresRumination) &&
                ActionReady(RiddleOfFire) &&
                CanDelayedWeave() &&
                GetTargetHPPercent() >= Config.MNK_AoE_RiddleOfFire_HP)
                return RiddleOfFire;

            // Buffs
            if (CanWeave())
            {
                //Variant Rampart
                if (IsEnabled(CustomComboPreset.MNK_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_AoEUseBrotherhood) &&
                        ActionReady(Brotherhood) &&
                        GetTargetHPPercent() >= Config.MNK_AoE_Brotherhood_HP)
                        return Brotherhood;

                    if (IsEnabled(CustomComboPreset.MNK_AoEUseROW) &&
                        ActionReady(RiddleOfWind) &&
                        !HasEffect(Buffs.WindsRumination) &&
                        GetTargetHPPercent() >= Config.MNK_AoE_RiddleOfWind_HP)
                        return RiddleOfWind;
                }

                if (IsEnabled(CustomComboPreset.MNK_AoEUsePerfectBalance) &&
                    ActionReady(PerfectBalance) &&
                    !HasEffect(Buffs.PerfectBalance) &&
                    (GetRemainingCharges(PerfectBalance) == GetMaxCharges(PerfectBalance) ||
                     GetCooldownRemainingTime(PerfectBalance) <= 4 ||
                     HasEffect(Buffs.Brotherhood) ||
                     HasEffect(Buffs.RiddleOfFire) && GetBuffRemainingTime(Buffs.RiddleOfFire) < 10 ||
                     GetCooldownRemainingTime(RiddleOfFire) < 4 && GetCooldownRemainingTime(Brotherhood) < 8))
                    return PerfectBalance;

                if (IsEnabled(CustomComboPreset.MNK_AoEUseHowlingFist) &&
                    Gauge.Chakra >= 5 && HasBattleTarget() && InCombat() &&
                    LevelChecked(InspiritedMeditation))
                    return OriginalHook(InspiritedMeditation);

                if (IsEnabled(CustomComboPreset.MNK_AoE_ComboHeals))
                {
                    if (PlayerHealthPercentageHp() <= Config.MNK_AoE_SecondWind_Threshold &&
                        ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= Config.MNK_AoE_Bloodbath_Threshold &&
                        ActionReady(All.Bloodbath))
                        return All.Bloodbath;
                }
            }

            if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
            {
                if (IsEnabled(CustomComboPreset.MNK_AoEUseROF) &&
                    IsEnabled(CustomComboPreset.MNK_AoEUseFiresReply) &&
                    LevelChecked(FiresReply) &&
                    HasEffect(Buffs.FiresRumination) &&
                    !HasEffect(Buffs.PerfectBalance) &&
                    !HasEffect(Buffs.FormlessFist))
                    return FiresReply;

                if (IsEnabled(CustomComboPreset.MNK_AoEUseROW) &&
                    IsEnabled(CustomComboPreset.MNK_AoEUseWindsReply) &&
                    HasEffect(Buffs.WindsRumination) &&
                    LevelChecked(WindsReply) &&
                    HasEffect(Buffs.RiddleOfWind) &&
                    GetBuffRemainingTime(Buffs.WindsRumination) < 4)
                    return WindsReply;
            }

            // Masterful Blitz
            if (IsEnabled(CustomComboPreset.MNK_AoEUseMasterfulBlitz) &&
                LevelChecked(MasterfulBlitz) &&
                !HasEffect(Buffs.PerfectBalance) &&
                OriginalHook(MasterfulBlitz) != MasterfulBlitz)
                return OriginalHook(MasterfulBlitz);

            // Perfect Balance
            if (HasEffect(Buffs.PerfectBalance))
            {
                #region Open Lunar

                if (!LunarNadi || BothNadisOpen || !SolarNadi && !LunarNadi)
                    return LevelChecked(ShadowOfTheDestroyer)
                        ? ShadowOfTheDestroyer
                        : Rockbreaker;

                #endregion

                #region Open Solar

                if (!SolarNadi && !BothNadisOpen)
                    switch (GetBuffStacks(Buffs.PerfectBalance))
                    {
                        case 3:
                            return OriginalHook(ArmOfTheDestroyer);

                        case 2:
                            return FourPointFury;

                        case 1:
                            return Rockbreaker;
                    }

                #endregion
            }

            // Monk Rotation
            if (HasEffect(Buffs.OpoOpoForm))
                return OriginalHook(ArmOfTheDestroyer);

            if (HasEffect(Buffs.RaptorForm))
            {
                if (LevelChecked(FourPointFury))
                    return FourPointFury;

                if (LevelChecked(TwinSnakes))
                    return TwinSnakes;
            }

            if (HasEffect(Buffs.CoeurlForm) && LevelChecked(Rockbreaker))
                return Rockbreaker;

            return actionID;
        }
    }

    internal class MNK_PerfectBalance : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_PerfectBalance;

        protected override uint Invoke(uint actionID) =>
            actionID is PerfectBalance &&
            OriginalHook(MasterfulBlitz) != MasterfulBlitz &&
            LevelChecked(MasterfulBlitz)
                ? OriginalHook(MasterfulBlitz)
                : actionID;
    }

    internal class MNK_Riddle_Brotherhood : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_Riddle_Brotherhood;

        protected override uint Invoke(uint actionID) =>
            actionID is RiddleOfFire &&
            ActionReady(Brotherhood) && IsOnCooldown(RiddleOfFire)
                ? Brotherhood
                : actionID;
    }

    internal class MNK_BeastChakras : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.MNK_ST_BeastChakras;

        protected override uint Invoke(uint actionID)
        {
            if (IsEnabled(CustomComboPreset.MNK_BC_OPOOPO) &&
                actionID is Bootshine or LeapingOpo)
                return Gauge.OpoOpoFury == 0 && LevelChecked(DragonKick)
                    ? DragonKick
                    : OriginalHook(Bootshine);

            if (IsEnabled(CustomComboPreset.MNK_BC_RAPTOR) &&
                actionID is TrueStrike or RisingRaptor)
                return Gauge.RaptorFury == 0 && LevelChecked(TwinSnakes)
                    ? TwinSnakes
                    : OriginalHook(TrueStrike);

            if (IsEnabled(CustomComboPreset.MNK_BC_COEURL) &&
                actionID is SnapPunch or PouncingCoeurl)
                return Gauge.CoeurlFury == 0 && LevelChecked(Demolish)
                    ? Demolish
                    : OriginalHook(SnapPunch);

            return actionID;
        }
    }
}
