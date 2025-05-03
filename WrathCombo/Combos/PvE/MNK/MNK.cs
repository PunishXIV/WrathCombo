using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class MNK : Melee
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

            //Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                return Variant.Rampart;

            if (LevelChecked(SteeledMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasStatusEffect(Buffs.RiddleOfFire) &&
                !HasStatusEffect(Buffs.WindsRumination) &&
                !HasStatusEffect(Buffs.FiresRumination))
                return OriginalHook(SteeledMeditation);

            if (LevelChecked(FormShift) && !InCombat() &&
                !HasStatusEffect(Buffs.FormlessFist) && !HasStatusEffect(Buffs.PerfectBalance) &&
                !HasStatusEffect(Buffs.OpoOpoForm) && !HasStatusEffect(Buffs.RaptorForm) && !HasStatusEffect(Buffs.CoeurlForm))
                return FormShift;

            if (ActionReady(RiddleOfFire) &&
                !HasStatusEffect(Buffs.FiresRumination) &&
                CanDelayedWeave() && InBossEncounter())
                return RiddleOfFire;

            // OGCDs
            if (CanWeave())
            {
                if (ActionReady(Brotherhood) &&
                    InBossEncounter())
                    return Brotherhood;

                if (ActionReady(RiddleOfWind) &&
                    !HasStatusEffect(Buffs.WindsRumination) &&
                    InBossEncounter())
                    return RiddleOfWind;

                //Perfect Balance
                if (UsePerfectBalanceST())
                    return PerfectBalance;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;

                if (Gauge.Chakra >= 5 && InCombat() && LevelChecked(SteeledMeditation))
                    return OriginalHook(SteeledMeditation);
            }

            // GCDs
            if (HasStatusEffect(Buffs.FormlessFist))
                return Gauge.OpoOpoFury == 0
                    ? DragonKick
                    : OriginalHook(Bootshine);

            // Masterful Blitz
            if (LevelChecked(MasterfulBlitz) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                InMasterfulRange() && !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (HasStatusEffect(Buffs.FiresRumination) &&
                !HasStatusEffect(Buffs.FormlessFist) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                !JustUsed(RiddleOfFire, 4) &&
                (JustUsed(OriginalHook(Bootshine)) ||
                 JustUsed(DragonKick) ||
                 GetStatusEffectRemainingTime(Buffs.FiresRumination) < 4 ||
                 !InMeleeRange()))
                return FiresReply;

            if (HasStatusEffect(Buffs.WindsRumination) &&
                (!InMeleeRange() || !HasStatusEffect(Buffs.PerfectBalance)))
                return WindsReply;

            // Perfect Balance
            if (DoPerfectBalanceComboST(ref actionID))
                return actionID;

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

            //Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                return Variant.Rampart;

            if (IsEnabled(CustomComboPreset.MNK_STUseMeditation) &&
                LevelChecked(SteeledMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasStatusEffect(Buffs.RiddleOfFire) &&
                !HasStatusEffect(Buffs.WindsRumination) &&
                !HasStatusEffect(Buffs.FiresRumination))
                return OriginalHook(SteeledMeditation);

            if (IsEnabled(CustomComboPreset.MNK_STUseFormShift) &&
                LevelChecked(FormShift) && !InCombat() &&
                !HasStatusEffect(Buffs.FormlessFist) && !HasStatusEffect(Buffs.PerfectBalance) &&
                !HasStatusEffect(Buffs.OpoOpoForm) && !HasStatusEffect(Buffs.RaptorForm) && !HasStatusEffect(Buffs.CoeurlForm))
                return FormShift;

            if (IsEnabled(CustomComboPreset.MNK_STUseOpener) &&
                Opener().FullOpener(ref actionID))
                return Opener().OpenerStep >= 9 &&
                       CanWeave() && Gauge.Chakra >= 5
                    ? TheForbiddenChakra
                    : actionID;


            if (IsEnabled(CustomComboPreset.MNK_STUseBuffs) &&
                IsEnabled(CustomComboPreset.MNK_STUseROF) &&
                !HasStatusEffect(Buffs.FiresRumination) &&
                ActionReady(RiddleOfFire) &&
                CanDelayedWeave() &&
                (Config.MNK_ST_RiddleOfFire_SubOption == 0 ||
                 Config.MNK_ST_RiddleOfFire_SubOption == 1 && InBossEncounter()))
                return RiddleOfFire;

            // OGCDs
            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_STUseBrotherhood) &&
                        ActionReady(Brotherhood) &&
                        (Config.MNK_ST_Brotherhood_SubOption == 0 ||
                         Config.MNK_ST_Brotherhood_SubOption == 1 && InBossEncounter()))
                        return Brotherhood;

                    if (IsEnabled(CustomComboPreset.MNK_STUseROW) &&
                        !HasStatusEffect(Buffs.WindsRumination) &&
                        ActionReady(RiddleOfWind) &&
                        (Config.MNK_ST_RiddleOfWind_SubOption == 0 ||
                         Config.MNK_ST_RiddleOfWind_SubOption == 1 && InBossEncounter()))
                        return RiddleOfWind;
                }

                //Perfect Balance
                if (IsEnabled(CustomComboPreset.MNK_STUsePerfectBalance) &&
                    UsePerfectBalanceST())
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
            if (HasStatusEffect(Buffs.FormlessFist))
                return Gauge.OpoOpoFury == 0
                    ? DragonKick
                    : OriginalHook(Bootshine);

            // Masterful Blitz
            if (IsEnabled(CustomComboPreset.MNK_STUseMasterfulBlitz) &&
                LevelChecked(MasterfulBlitz) &&
                !HasStatusEffect(Buffs.PerfectBalance) && InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (IsEnabled(CustomComboPreset.MNK_STUseBuffs))
            {
                if (IsEnabled(CustomComboPreset.MNK_STUseFiresReply) &&
                    HasStatusEffect(Buffs.FiresRumination) &&
                    !HasStatusEffect(Buffs.FormlessFist) &&
                    !HasStatusEffect(Buffs.PerfectBalance) &&
                    !JustUsed(RiddleOfFire, 4) &&
                    (JustUsed(OriginalHook(Bootshine)) ||
                     JustUsed(DragonKick) ||
                     GetStatusEffectRemainingTime(Buffs.FiresRumination) < 4 ||
                     !InMeleeRange()))
                    return FiresReply;

                if (IsEnabled(CustomComboPreset.MNK_STUseWindsReply) &&
                    HasStatusEffect(Buffs.WindsRumination) &&
                    (!InMeleeRange() || !HasStatusEffect(Buffs.PerfectBalance)))
                    return WindsReply;
            }

            // Perfect Balance
            if (DoPerfectBalanceComboST(ref actionID))
                return actionID;

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

            //Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                return Variant.Rampart;

            if (LevelChecked(InspiritedMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasStatusEffect(Buffs.RiddleOfFire) &&
                !HasStatusEffect(Buffs.WindsRumination) &&
                !HasStatusEffect(Buffs.FiresRumination))
                return OriginalHook(InspiritedMeditation);

            if (LevelChecked(FormShift) && !InCombat() &&
                !HasStatusEffect(Buffs.FormlessFist) && !HasStatusEffect(Buffs.PerfectBalance) &&
                !HasStatusEffect(Buffs.OpoOpoForm) && !HasStatusEffect(Buffs.RaptorForm) && !HasStatusEffect(Buffs.CoeurlForm))
                return FormShift;

            if (ActionReady(RiddleOfFire) &&
                !HasStatusEffect(Buffs.FiresRumination) &&
                CanDelayedWeave())
                return RiddleOfFire;

            // Buffs
            if (CanWeave())
            {
                if (ActionReady(Brotherhood))
                    return Brotherhood;

                if (ActionReady(RiddleOfWind) &&
                    !HasStatusEffect(Buffs.WindsRumination))
                    return RiddleOfWind;

                if (UsePerfectBalanceAoE())
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
                !HasStatusEffect(Buffs.PerfectBalance) &&
                InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (HasStatusEffect(Buffs.FiresRumination) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                !HasStatusEffect(Buffs.FormlessFist) &&
                !JustUsed(RiddleOfFire, 4))
                return FiresReply;

            if (HasStatusEffect(Buffs.WindsRumination) &&
                HasStatusEffect(Buffs.RiddleOfWind) &&
                !HasStatusEffect(Buffs.PerfectBalance))
                return WindsReply;

            // Perfect Balance
            if (DoPerfectBalanceComboAoE(ref actionID))
                return actionID;

            // Monk Rotation
            if (HasStatusEffect(Buffs.OpoOpoForm))
                return OriginalHook(ArmOfTheDestroyer);

            if (HasStatusEffect(Buffs.RaptorForm))
            {
                if (LevelChecked(FourPointFury))
                    return FourPointFury;

                if (LevelChecked(TwinSnakes))
                    return TwinSnakes;
            }

            if (HasStatusEffect(Buffs.CoeurlForm) && LevelChecked(Rockbreaker))
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

            //Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.MNK_Variant_Rampart))
                return Variant.Rampart;

            if (IsEnabled(CustomComboPreset.MNK_AoEUseMeditation) &&
                LevelChecked(InspiritedMeditation) &&
                (!InCombat() || !InMeleeRange()) &&
                Gauge.Chakra < 5 &&
                IsOriginal(MasterfulBlitz) &&
                !HasStatusEffect(Buffs.RiddleOfFire) &&
                !HasStatusEffect(Buffs.WindsRumination) &&
                !HasStatusEffect(Buffs.FiresRumination))
                return OriginalHook(InspiritedMeditation);

            if (IsEnabled(CustomComboPreset.MNK_AoEUseFormShift) &&
                LevelChecked(FormShift) && !InCombat() &&
                !HasStatusEffect(Buffs.FormlessFist) && !HasStatusEffect(Buffs.PerfectBalance) &&
                !HasStatusEffect(Buffs.OpoOpoForm) && !HasStatusEffect(Buffs.RaptorForm) && !HasStatusEffect(Buffs.CoeurlForm))
                return FormShift;

            if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs) &&
                IsEnabled(CustomComboPreset.MNK_AoEUseROF) &&
                !HasStatusEffect(Buffs.FiresRumination) &&
                ActionReady(RiddleOfFire) &&
                CanDelayedWeave() &&
                GetTargetHPPercent() >= Config.MNK_AoE_RiddleOfFire_HP)
                return RiddleOfFire;

            // Buffs
            if (CanWeave())
            {
                if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
                {
                    if (IsEnabled(CustomComboPreset.MNK_AoEUseBrotherhood) &&
                        ActionReady(Brotherhood) &&
                        GetTargetHPPercent() >= Config.MNK_AoE_Brotherhood_HP)
                        return Brotherhood;

                    if (IsEnabled(CustomComboPreset.MNK_AoEUseROW) &&
                        ActionReady(RiddleOfWind) &&
                        !HasStatusEffect(Buffs.WindsRumination) &&
                        GetTargetHPPercent() >= Config.MNK_AoE_RiddleOfWind_HP)
                        return RiddleOfWind;
                }

                if (IsEnabled(CustomComboPreset.MNK_AoEUsePerfectBalance) &&
                    UsePerfectBalanceAoE())
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
                !HasStatusEffect(Buffs.PerfectBalance) &&
                InMasterfulRange() &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (IsEnabled(CustomComboPreset.MNK_AoEUseBuffs))
            {
                if (IsEnabled(CustomComboPreset.MNK_AoEUseFiresReply) &&
                    HasStatusEffect(Buffs.FiresRumination) &&
                    !HasStatusEffect(Buffs.FormlessFist) &&
                    !HasStatusEffect(Buffs.PerfectBalance) &&
                    !JustUsed(RiddleOfFire, 4))
                    return FiresReply;

                if (IsEnabled(CustomComboPreset.MNK_AoEUseWindsReply) &&
                    HasStatusEffect(Buffs.WindsRumination) &&
                    HasStatusEffect(Buffs.RiddleOfWind) &&
                    !HasStatusEffect(Buffs.PerfectBalance))
                    return WindsReply;
            }

            // Perfect Balance
            if (DoPerfectBalanceComboAoE(ref actionID))
                return actionID;

            // Monk Rotation
            if (HasStatusEffect(Buffs.OpoOpoForm))
                return OriginalHook(ArmOfTheDestroyer);

            if (HasStatusEffect(Buffs.RaptorForm))
            {
                if (LevelChecked(FourPointFury))
                    return FourPointFury;

                if (LevelChecked(TwinSnakes))
                    return TwinSnakes;
            }

            if (HasStatusEffect(Buffs.CoeurlForm) && LevelChecked(Rockbreaker))
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
            actionID is PerfectBalance && HasStatusEffect(Buffs.PerfectBalance) && LevelChecked(PerfectBalance)
                ? All.SavageBlade
                : actionID;
    }
}
