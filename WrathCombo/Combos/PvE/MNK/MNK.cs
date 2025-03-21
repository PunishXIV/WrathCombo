using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class MNK : MeleeJob
{
    internal class MNK_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bootshine or LeapingOpo))
                return actionID;

            //Variant Cure
            if (Variant.CanCure(CustomComboPreset.MNK_Variant_Cure, Config.MNK_VariantCure))
                return Variant.Cure;

            if (LevelChecked(SteeledMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasEffect(Buffs.RiddleOfFire) &&
                !HasEffect(Buffs.WindsRumination) &&
                !HasEffect(Buffs.FiresRumination))
                return OriginalHook(SteeledMeditation);

            if (LevelChecked(FormShift) && !InCombat() &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance) &&
                !HasEffect(Buffs.OpoOpoForm) && !HasEffect(Buffs.RaptorForm) && !HasEffect(Buffs.CoeurlForm))
                return FormShift;

            if (ActionReady(RiddleOfFire) &&
                !HasEffect(Buffs.FiresRumination) &&
                CanDelayedWeave() && InBossEncounter())
                return RiddleOfFire;

            // OGCDs
            if (CanWeave())
            {
                //Variant Rampart
                if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                    return Variant.Rampart;

                if (ActionReady(Brotherhood) &&
                    InBossEncounter())
                    return Brotherhood;

                if (ActionReady(RiddleOfWind) &&
                    !HasEffect(Buffs.WindsRumination) &&
                    InBossEncounter())
                    return RiddleOfWind;

                //Perfect Balance
                if (UsePerfectBalance())
                    return PerfectBalance;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;

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
                InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (HasEffect(Buffs.FiresRumination) &&
                LevelChecked(FiresReply) &&
                !HasEffect(Buffs.FormlessFist) &&
                !JustUsed(RiddleOfFire, 4) &&
                (JustUsed(OriginalHook(Bootshine)) ||
                 JustUsed(DragonKick) ||
                 GetBuffRemainingTime(Buffs.FiresRumination) < 4 ||
                 !InMeleeRange()))
                return FiresReply;

            if (HasEffect(Buffs.WindsRumination) &&
                LevelChecked(WindsReply) &&
                (!InMeleeRange() || !HasEffect(Buffs.PerfectBalance)))
                return WindsReply;

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

            //Variant Cure
            if (Variant.CanCure(CustomComboPreset.MNK_Variant_Cure, Config.MNK_VariantCure))
                return Variant.Cure;

            if (IsEnabled(CustomComboPreset.MNK_STUseMeditation) &&
                LevelChecked(SteeledMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasEffect(Buffs.RiddleOfFire) &&
                !HasEffect(Buffs.WindsRumination) &&
                !HasEffect(Buffs.FiresRumination))
                return OriginalHook(SteeledMeditation);

            if (IsEnabled(CustomComboPreset.MNK_STUseFormShift) &&
                LevelChecked(FormShift) && !InCombat() &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance) &&
                !HasEffect(Buffs.OpoOpoForm) && !HasEffect(Buffs.RaptorForm) && !HasEffect(Buffs.CoeurlForm))
                return FormShift;

            if (IsEnabled(CustomComboPreset.MNK_STUseOpener))
                if (Opener().FullOpener(ref actionID))
                {
                    if (Opener().OpenerStep >= 9 &&
                        CanWeave() && Gauge.Chakra >= 5)
                        return TheForbiddenChakra;

                    return actionID;
                }

            if (IsEnabled(CustomComboPreset.MNK_STUseBuffs) &&
                IsEnabled(CustomComboPreset.MNK_STUseROF) &&
                !HasEffect(Buffs.FiresRumination) &&
                ActionReady(RiddleOfFire) &&
                CanDelayedWeave() &&
                (Config.MNK_ST_RiddleOfFire_SubOption == 0 ||
                 Config.MNK_ST_RiddleOfFire_SubOption == 1 && InBossEncounter()))
                return RiddleOfFire;

            // OGCDs
            if (CanWeave())
            {
                //Variant Rampart
                if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                    return Variant.Rampart;

                if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_STUseBrotherhood) &&
                        ActionReady(Brotherhood) &&
                        (Config.MNK_ST_Brotherhood_SubOption == 0 ||
                         Config.MNK_ST_Brotherhood_SubOption == 1 && InBossEncounter()))
                        return Brotherhood;

                    if (IsEnabled(CustomComboPreset.MNK_STUseROW) &&
                        !HasEffect(Buffs.WindsRumination) &&
                        ActionReady(RiddleOfWind) &&
                        (Config.MNK_ST_RiddleOfWind_SubOption == 0 ||
                         Config.MNK_ST_RiddleOfWind_SubOption == 1 && InBossEncounter()))
                        return RiddleOfWind;
                }

                //Perfect Balance
                if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance) &&
                    UsePerfectBalance())
                    return PerfectBalance;

                if (IsEnabled(CustomComboPreset.MNK_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(Config.MNK_ST_SecondWind_Threshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(Config.MNK_ST_Bloodbath_Threshold))
                        return Role.Bloodbath;
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
                !HasEffect(Buffs.PerfectBalance) && InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
            {
                if (IsEnabled(CustomComboPreset.MNK_STUseFiresReply) &&
                    HasEffect(Buffs.FiresRumination) &&
                    LevelChecked(FiresReply) &&
                    !HasEffect(Buffs.FormlessFist) &&
                    !JustUsed(RiddleOfFire, 4) &&
                    (JustUsed(OriginalHook(Bootshine)) ||
                     JustUsed(DragonKick) ||
                     GetBuffRemainingTime(Buffs.FiresRumination) < 4 ||
                     !InMeleeRange()))
                    return FiresReply;

                if (IsEnabled(CustomComboPreset.MNK_STUseWindsReply) &&
                    HasEffect(Buffs.WindsRumination) &&
                    LevelChecked(WindsReply) &&
                    (!InMeleeRange() || !HasEffect(Buffs.PerfectBalance)))
                    return WindsReply;
            }

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

            //Variant Cure
            if (Variant.CanCure(CustomComboPreset.MNK_Variant_Cure, Config.MNK_VariantCure))
                return Variant.Cure;

            if (LevelChecked(InspiritedMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasEffect(Buffs.RiddleOfFire) &&
                !HasEffect(Buffs.WindsRumination) &&
                !HasEffect(Buffs.FiresRumination))
                return OriginalHook(InspiritedMeditation);

            if (LevelChecked(FormShift) && !InCombat() &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance) &&
                !HasEffect(Buffs.OpoOpoForm) && !HasEffect(Buffs.RaptorForm) && !HasEffect(Buffs.CoeurlForm))
                return FormShift;

            if (ActionReady(RiddleOfFire) &&
                !HasEffect(Buffs.FiresRumination) &&
                CanDelayedWeave())
                return RiddleOfFire;

            // Buffs
            if (CanWeave())
            {
                //Variant Rampart
                if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                    return Variant.Rampart;

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


                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;

                if (Gauge.Chakra >= 5 &&
                    LevelChecked(InspiritedMeditation) &&
                    HasBattleTarget() && InCombat())
                    return OriginalHook(InspiritedMeditation);
            }

            // Masterful Blitz
            if (LevelChecked(MasterfulBlitz) &&
                !HasEffect(Buffs.PerfectBalance) &&
                InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (HasEffect(Buffs.FiresRumination) &&
                LevelChecked(FiresReply) &&
                !HasEffect(Buffs.FormlessFist) &&
                !JustUsed(RiddleOfFire, 4))
                return FiresReply;

            if (HasEffect(Buffs.WindsRumination) &&
                LevelChecked(WindsReply) &&
                HasEffect(Buffs.RiddleOfWind) &&
                !HasEffect(Buffs.PerfectBalance))
                return WindsReply;

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

            //Variant Cure
            if (Variant.CanCure(CustomComboPreset.MNK_Variant_Cure, Config.MNK_VariantCure))
                return Variant.Cure;

            if (IsEnabled(CustomComboPreset.MNK_AoEUseMeditation) &&
                LevelChecked(InspiritedMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasEffect(Buffs.RiddleOfFire) &&
                !HasEffect(Buffs.WindsRumination) &&
                !HasEffect(Buffs.FiresRumination))
                return OriginalHook(InspiritedMeditation);

            if (IsEnabled(CustomComboPreset.MNK_AoEUseFormShift) &&
                LevelChecked(FormShift) && !InCombat() &&
                !HasEffect(Buffs.FormlessFist) && !HasEffect(Buffs.PerfectBalance) &&
                !HasEffect(Buffs.OpoOpoForm) && !HasEffect(Buffs.RaptorForm) && !HasEffect(Buffs.CoeurlForm))
                return FormShift;

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
                if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                    return Variant.Rampart;

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

                if (IsEnabled(CustomComboPreset.MNK_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(Config.MNK_AoE_SecondWind_Threshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(Config.MNK_AoE_Bloodbath_Threshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(CustomComboPreset.MNK_AoEUseHowlingFist) &&
                    Gauge.Chakra >= 5 && HasBattleTarget() && InCombat() &&
                    LevelChecked(InspiritedMeditation))
                    return OriginalHook(InspiritedMeditation);
            }

            // Masterful Blitz
            if (IsEnabled(CustomComboPreset.MNK_AoEUseMasterfulBlitz) &&
                LevelChecked(MasterfulBlitz) &&
                !HasEffect(Buffs.PerfectBalance) &&
                InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
            {
                if (IsEnabled(CustomComboPreset.MNK_AoEUseFiresReply) &&
                    HasEffect(Buffs.FiresRumination) &&
                    LevelChecked(FiresReply) &&
                    !HasEffect(Buffs.FormlessFist) &&
                    !JustUsed(RiddleOfFire, 4))
                    return FiresReply;

                if (IsEnabled(CustomComboPreset.MNK_AoEUseWindsReply) &&
                    HasEffect(Buffs.WindsRumination) &&
                    LevelChecked(WindsReply) &&
                    HasEffect(Buffs.RiddleOfWind) &&
                    !HasEffect(Buffs.PerfectBalance))
                    return WindsReply;
            }

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

    internal class MNK_Brotherhood_Riddle : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_Brotherhood_Riddle;

        protected override uint Invoke(uint actionID) =>
            actionID is Brotherhood &&
            ActionReady(RiddleOfFire) && IsOnCooldown(Brotherhood)
                ? RiddleOfFire
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

    internal class MNK_PerfectBalanceProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNK_PerfectBalanceProtection;

        protected override uint Invoke(uint actionID) =>
            actionID is PerfectBalance && HasEffect(Buffs.PerfectBalance) && LevelChecked(PerfectBalance)
                ? All.SavageBlade
                : actionID;
    }
}
