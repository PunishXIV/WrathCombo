using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.MNK.Config;
namespace WrathCombo.Combos.PvE;

internal partial class MNK : Melee
{
    internal class MNK_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bootshine or LeapingOpo))
                return actionID;

            if (CanMeditate())
                return OriginalHook(SteeledMeditation);

            if (CanFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // OGCDs
            if (CanWeave() && InCombat())
            {
                if (CanBrotherhood())
                    return Brotherhood;

                if (CanRoF())
                    return RiddleOfFire;

                if (CanPerfectBalance())
                    return PerfectBalance;

                if (CanRoW())
                    return RiddleOfWind;

                if (CanUseChakra())
                    return OriginalHook(SteeledMeditation);

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            // GCDs
            if (HasStatusEffect(Buffs.FormlessFist))
                return OpoOpoStacks is 0
                    ? DragonKick
                    : OriginalHook(Bootshine);

            // Masterful Blitz
            if (CanMasterfulBlitz(false))
                return OriginalHook(MasterfulBlitz);

            if (CanWindsReply())
                return WindsReply;

            if (CanFiresReply())
                return FiresReply;

            // Perfect Balance or Standard Beast Chakra's
            return DoPerfectBalanceCombo(ref actionID)
                ? actionID
                : DoBasicCombo(actionID);
        }
    }

    internal class MNK_AOE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_AOE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ArmOfTheDestroyer or ShadowOfTheDestroyer))
                return actionID;

            if (CanMeditate(true))
                return OriginalHook(InspiritedMeditation);

            if (CanFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // OGCD's
            if (CanWeave() && InCombat())
            {
                if (CanBrotherhood())
                    return Brotherhood;

                if (CanRoF())
                    return RiddleOfFire;

                if (CanPerfectBalance(true))
                    return PerfectBalance;

                if (CanRoW())
                    return RiddleOfWind;

                if (CanUseChakra(true))
                    return OriginalHook(InspiritedMeditation);

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            // Masterful Blitz
            if (CanMasterfulBlitz(true))
                return OriginalHook(MasterfulBlitz);

            if (HasStatusEffect(Buffs.FiresRumination) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                !HasStatusEffect(Buffs.FormlessFist) &&
                !JustUsed(RiddleOfFire, 4))
                return FiresReply;

            if (HasStatusEffect(Buffs.WindsRumination) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                (GetCooldownRemainingTime(RiddleOfFire) > 5 ||
                 HasStatusEffect(Buffs.RiddleOfFire)))
                return WindsReply;

            // Perfect Balance
            if (DoPerfectBalanceCombo(ref actionID, true))
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

    internal class MNK_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bootshine or LeapingOpo))
                return actionID;

            if (IsEnabled(Preset.MNK_STUseOpener) &&
                Opener().FullOpener(ref actionID))
                return Opener().OpenerStep >= 9 &&
                       CanWeave() && Chakra >= 5
                    ? TheForbiddenChakra
                    : actionID;

            if (IsEnabled(Preset.MNK_STUseMeditation) &&
                CanMeditate())
                return OriginalHook(SteeledMeditation);

            if (IsEnabled(Preset.MNK_STUseFormShift) &&
                CanFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // OGCDs
            if (CanWeave() && InCombat())
            {
                if (IsEnabled(Preset.MNK_STUseBuffs) &&
                    GetTargetHPPercent() > HPThresholdBuffs)
                {
                    if (IsEnabled(Preset.MNK_STUseBrotherhood) &&
                        CanBrotherhood())
                        return Brotherhood;

                    if (IsEnabled(Preset.MNK_STUseROF) &&
                        CanRoF())
                        return RiddleOfFire;
                }

                if (IsEnabled(Preset.MNK_STUsePerfectBalance) &&
                    CanPerfectBalance())
                    return PerfectBalance;

                if (IsEnabled(Preset.MNK_STUseBuffs) &&
                    IsEnabled(Preset.MNK_STUseROW) &&
                    GetTargetHPPercent() > HPThresholdBuffs &&
                    CanRoW())
                    return RiddleOfWind;

                if (IsEnabled(Preset.MNK_STUseTheForbiddenChakra) &&
                    CanUseChakra())
                    return OriginalHook(SteeledMeditation);

                if (IsEnabled(Preset.MNK_ST_UseMantra) &&
                    CanMantra())
                    return Mantra;

                if (IsEnabled(Preset.MNK_ST_UseRoE) &&
                    (CanRoE() ||
                     MNK_ST_EarthsReply && CanEarthsReply()))
                    return OriginalHook(RiddleOfEarth);

                if (IsEnabled(Preset.MNK_ST_Feint) &&
                    Role.CanFeint() && RaidWideCasting())
                    return Role.Feint;

                if (IsEnabled(Preset.MNK_ST_ComboHeals))
                {
                    if (Role.CanSecondWind(MNK_ST_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(MNK_ST_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.MNK_ST_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            // GCDs
            if (HasStatusEffect(Buffs.FormlessFist))
                return OpoOpoStacks is 0
                    ? DragonKick
                    : OriginalHook(Bootshine);

            // Masterful Blitz
            if (IsEnabled(Preset.MNK_STUseMasterfulBlitz) &&
                CanMasterfulBlitz(false))
                return OriginalHook(MasterfulBlitz);

            if (IsEnabled(Preset.MNK_STUseWindsReply) &&
                CanWindsReply())
                return WindsReply;

            if (IsEnabled(Preset.MNK_STUseFiresReply) &&
                CanFiresReply())
                return FiresReply;

            // Perfect Balance or Standard Beast Chakra's
            return DoPerfectBalanceCombo(ref actionID)
                ? actionID
                : DoBasicCombo(actionID, IsEnabled(Preset.MNK_STUseTrueNorth));
        }
    }

    internal class MNK_AOE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_AOE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ArmOfTheDestroyer or ShadowOfTheDestroyer))
                return actionID;

            if (IsEnabled(Preset.MNK_AoEUseMeditation) &&
                CanMeditate(true))
                return OriginalHook(InspiritedMeditation);

            if (IsEnabled(Preset.MNK_AoEUseFormShift) &&
                CanFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            // OGCD's
            if (CanWeave() && InCombat())
            {
                if (IsEnabled(Preset.MNK_AoEUseBuffs) &&
                    GetTargetHPPercent() >= MNK_AoE_BuffsHPThreshold)
                {
                    if (IsEnabled(Preset.MNK_AoEUseBrotherhood) &&
                        CanBrotherhood())
                        return Brotherhood;

                    if (IsEnabled(Preset.MNK_AoEUseROF) &&
                        CanRoF())
                        return RiddleOfFire;
                }

                if (IsEnabled(Preset.MNK_AoEUsePerfectBalance) &&
                    CanPerfectBalance(true))
                    return PerfectBalance;

                if (IsEnabled(Preset.MNK_AoEUseBuffs) &&
                    IsEnabled(Preset.MNK_AoEUseROW) &&
                    GetTargetHPPercent() >= MNK_AoE_BuffsHPThreshold &&
                    CanRoW())
                    return RiddleOfWind;

                if (IsEnabled(Preset.MNK_AoEUseHowlingFist) &&
                    CanUseChakra(true))
                    return OriginalHook(InspiritedMeditation);

                if (IsEnabled(Preset.MNK_AoE_ComboHeals))
                {
                    if (Role.CanSecondWind(MNK_AoE_SecondWindHPThreshold))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(MNK_AoE_BloodbathHPThreshold))
                        return Role.Bloodbath;
                }

                if (IsEnabled(Preset.MNK_AoE_StunInterupt) &&
                    RoleActions.Melee.CanLegSweep())
                    return Role.LegSweep;
            }

            // Masterful Blitz
            if (IsEnabled(Preset.MNK_AoEUseMasterfulBlitz) &&
                CanMasterfulBlitz(true))
                return OriginalHook(MasterfulBlitz);

            if (IsEnabled(Preset.MNK_AoEUseFiresReply) &&
                HasStatusEffect(Buffs.FiresRumination) &&
                !HasStatusEffect(Buffs.FormlessFist) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                !JustUsed(RiddleOfFire, 4))
                return FiresReply;

            if (IsEnabled(Preset.MNK_AoEUseWindsReply) &&
                HasStatusEffect(Buffs.WindsRumination) &&
                !HasStatusEffect(Buffs.PerfectBalance) &&
                (GetCooldownRemainingTime(RiddleOfFire) > 5 ||
                 HasStatusEffect(Buffs.RiddleOfFire)))
                return WindsReply;

            // Perfect Balance
            if (DoPerfectBalanceCombo(ref actionID, true))
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

    internal class MNK_BeastChakras : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_ST_BeastChakras;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bootshine or LeapingOpo or TrueStrike or RisingRaptor or SnapPunch or PouncingCoeurl))
                return actionID;

            if (MNK_BasicCombo[0] &&
                actionID is Bootshine or LeapingOpo)
                return OpoOpoStacks is 0 && LevelChecked(DragonKick)
                    ? DragonKick
                    : OriginalHook(Bootshine);

            if (MNK_BasicCombo[1] &&
                actionID is TrueStrike or RisingRaptor)
                return RaptorStacks is 0 && LevelChecked(TwinSnakes)
                    ? TwinSnakes
                    : OriginalHook(TrueStrike);

            if (MNK_BasicCombo[2] &&
                actionID is SnapPunch or PouncingCoeurl)
                return CoeurlStacks is 0 && LevelChecked(Demolish)
                    ? Demolish
                    : OriginalHook(SnapPunch);

            return actionID;
        }
    }

    internal class MNK_Retarget_Thunderclap : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_Retarget_Thunderclap;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Thunderclap)
                return actionID;

            return MNK_Thunderclap_FieldMouseover
                ? Thunderclap.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.ModelMouseOverTarget ?? SimpleTarget.HardTarget)
                : Thunderclap.Retarget(SimpleTarget.UIMouseOverTarget ?? SimpleTarget.HardTarget);
        }
    }

    internal class MNK_PerfectBalance : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_PerfectBalance;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not PerfectBalance)
                return actionID;

            return OriginalHook(MasterfulBlitz) != MasterfulBlitz &&
                   LevelChecked(MasterfulBlitz)
                ? OriginalHook(MasterfulBlitz)
                : actionID;
        }
    }

    internal class MNK_Brotherhood_Riddle : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_Brotherhood_Riddle;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Brotherhood or RiddleOfFire))
                return actionID;

            return actionID switch
            {
                Brotherhood when MNK_BH_RoF == 0 && ActionReady(RiddleOfFire) && IsOnCooldown(Brotherhood) => OriginalHook(RiddleOfFire),
                RiddleOfFire when MNK_BH_RoF == 1 && ActionReady(Brotherhood) && IsOnCooldown(RiddleOfFire) => Brotherhood,
                var _ => actionID
            };
        }
    }

    internal class MNK_PerfectBalanceProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_PerfectBalanceProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not PerfectBalance)
                return actionID;

            return HasStatusEffect(Buffs.PerfectBalance) &&
                   LevelChecked(PerfectBalance)
                ? All.SavageBlade
                : actionID;
        }
    }
}
