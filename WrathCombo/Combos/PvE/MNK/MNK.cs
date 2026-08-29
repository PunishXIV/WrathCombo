using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.MNK.Config;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class MNK : Melee
{
    internal class MNK_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Bootshine, LeapingOpo))
                return actionID;

            if (UseMeditate())
                return OriginalHook(SteeledMeditation);

            if (UseFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave() && (InCombat() || ComboAction > 0))
            {
                if (ShouldUsePBAfterBurstHolding(false))
                    return PerfectBalance;

                if (UseBrotherhood())
                    return Brotherhood;

                if (UseRoF())
                    return RiddleOfFire;

                if (UsePerfectBalance(false))
                    return PerfectBalance;

                if (UseRoW())
                    return RiddleOfWind;

                if (UseChakra())
                    return OriginalHook(SteelPeak);

                if (Role.CanFeint() && GroupDamageIncoming())
                    return Role.Feint;

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (UseMasterfulBlitz(false))
                return OriginalHook(MasterfulBlitz);

            if (LocalPlayer.HasStatus(Buffs.FormlessFist) ||
                ForceSecondOpo(false))
                return ForcedOpoGCD(false);

            if (UseFiresReply())
                return FiresReply;

            if (UseWindsReply())
                return WindsReply;

            return DoPerfectBalanceCombo(ref actionID)
                ? actionID
                : DoBasicCombo();
        }
    }

    internal class MNK_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, ArmOfTheDestroyer, ShadowOfTheDestroyer))
                return actionID;

            if (UseMeditate(true))
                return OriginalHook(InspiritedMeditation);

            if (UseFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave() && (InCombat() || ComboAction > 0))
            {
                if (ShouldUsePBAfterBurstHolding(true))
                    return PerfectBalance;

                if (UseBrotherhood())
                    return Brotherhood;

                if (UseRoF())
                    return RiddleOfFire;

                if (UsePerfectBalance(true))
                    return PerfectBalance;

                if (UseRoW())
                    return RiddleOfWind;

                if (UseChakra(true))
                    return OriginalHook(HowlingFist);

                if (Role.CanSecondWind(25))
                    return Role.SecondWind;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }

            if (UseMasterfulBlitz(true))
                return OriginalHook(MasterfulBlitz);

            if (LocalPlayer.HasStatus(Buffs.FormlessFist) ||
                ForceSecondOpo(true))
                return ForcedOpoGCD(true);

            if (UseFiresReply(true))
                return FiresReply;

            if (UseWindsReply())
                return WindsReply;

            return DoPerfectBalanceCombo(ref actionID, true)
                ? actionID
                : DoBasicCombo(onAoE: true);
        }
    }

    internal class MNK_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, Bootshine, LeapingOpo))
                return actionID;

            if (IsEnabled(Preset.MNK_STUseOpener) &&
                Opener().FullOpener(ref actionID))
                return Opener().OpenerStep > 11 &&
                       CanWeave() && Chakra >= 5
                    ? TheForbiddenChakra
                    : actionID;

            if (IsEnabled(Preset.MNK_STUseMeditation) &&
                UseMeditate())
                return OriginalHook(SteeledMeditation);

            if (IsEnabled(Preset.MNK_STUseFormShift) &&
                UseFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave() && (InCombat() || ComboAction > 0))
            {
                bool burstHolding = IsEnabled(Preset.MNK_STUsePerfectBalance) &&
                                    !IsEnabled(Preset.MNK_STUseBrotherhood) &&
                                    !IsEnabled(Preset.MNK_STUseROF);

                if (IsEnabled(Preset.MNK_STUsePerfectBalance) &&
                    ShouldUsePBAfterBurstHolding(false))
                    return PerfectBalance;

                if (IsEnabled(Preset.MNK_STUseBuffs))
                {
                    if (IsEnabled(Preset.MNK_STUseBrotherhood) &&
                        GetTargetHPPercent() > BrotherhoodHPThreshold &&
                        UseBrotherhood())
                        return Brotherhood;

                    if (IsEnabled(Preset.MNK_STUseROF) &&
                        GetTargetHPPercent() > RiddleOfFireHPThreshold &&
                        UseRoF())
                        return RiddleOfFire;
                }

                if (IsEnabled(Preset.MNK_STUsePerfectBalance) &&
                    UsePerfectBalance(false, IsEnabled(Preset.MNK_STUseOpener), burstHolding,
                        useFiresReply: IsEnabled(Preset.MNK_STUseFiresReply)))
                    return PerfectBalance;

                if (IsEnabled(Preset.MNK_STUseBuffs) &&
                    IsEnabled(Preset.MNK_STUseROW) &&
                    GetTargetHPPercent() > RiddleOfWindHPThreshold &&
                    UseRoW())
                    return RiddleOfWind;

                if (IsEnabled(Preset.MNK_STUseTheForbiddenChakra) &&
                    UseChakra())
                    return OriginalHook(SteelPeak);

                if (IsEnabled(Preset.MNK_ST_UseMantra) &&
                    UseMantra())
                    return Mantra;

                if (IsEnabled(Preset.MNK_ST_UseRoE) &&
                    (UseRoE() ||
                     MNK_ST_EarthsReply && UseEarthsReply(MNK_ST_EarthsReplyHPThreshold)))
                    return OriginalHook(RiddleOfEarth);

                if (IsEnabled(Preset.MNK_ST_Feint) &&
                    Role.CanFeint() && GroupDamageIncoming())
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

            if (IsEnabled(Preset.MNK_STUseMasterfulBlitz) &&
                UseMasterfulBlitz(false))
                return OriginalHook(MasterfulBlitz);

            if (LocalPlayer.HasStatus(Buffs.FormlessFist) ||
                ForceSecondOpo(false, IsEnabled(Preset.MNK_STUseFiresReply)))
                return ForcedOpoGCD(false);

            if (IsEnabled(Preset.MNK_STUseFiresReply) &&
                UseFiresReply())
                return FiresReply;

            if (IsEnabled(Preset.MNK_STUseWindsReply) &&
                UseWindsReply())
                return WindsReply;

            return DoPerfectBalanceCombo(ref actionID)
                ? actionID
                : DoBasicCombo(IsEnabled(Preset.MNK_STUseTrueNorth), trueNorthCharges: MNK_ManualTN);
        }
    }

    internal class MNK_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, ArmOfTheDestroyer, ShadowOfTheDestroyer)) return actionID;

            if (IsEnabled(Preset.MNK_AoEUseMeditation) &&
                UseMeditate(true))
                return OriginalHook(InspiritedMeditation);

            if (IsEnabled(Preset.MNK_AoEUseFormShift) &&
                UseFormshift())
                return FormShift;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave() && (InCombat() || ComboAction > 0))
            {
                bool burstHolding = IsEnabled(Preset.MNK_AoEUsePerfectBalance) &&
                                    !IsEnabled(Preset.MNK_AoEUseBrotherhood) &&
                                    !IsEnabled(Preset.MNK_AoEUseROF);

                if (IsEnabled(Preset.MNK_AoEUsePerfectBalance) &&
                    ShouldUsePBAfterBurstHolding(true, MNK_AoE_PerfectBalanceHPThreshold))
                    return PerfectBalance;

                if (IsEnabled(Preset.MNK_AoEUseBuffs) &&
                    GetTargetHPPercent() >= MNK_AoE_BuffsHPThreshold)
                {
                    if (IsEnabled(Preset.MNK_AoEUseBrotherhood) &&
                        UseBrotherhood())
                        return Brotherhood;

                    if (IsEnabled(Preset.MNK_AoEUseROF) &&
                        UseRoF())
                        return RiddleOfFire;
                }

                if (IsEnabled(Preset.MNK_AoEUsePerfectBalance) &&
                    UsePerfectBalance(true, isBurstHolding: burstHolding,
                        perfectBalanceHpThreshold: MNK_AoE_PerfectBalanceHPThreshold,
                        useFiresReply: IsEnabled(Preset.MNK_AoEUseFiresReply)))
                    return PerfectBalance;

                if (IsEnabled(Preset.MNK_AoEUseBuffs) &&
                    IsEnabled(Preset.MNK_AoEUseROW) &&
                    GetTargetHPPercent() >= MNK_AoE_BuffsHPThreshold &&
                    UseRoW())
                    return RiddleOfWind;

                if (IsEnabled(Preset.MNK_AoEUseHowlingFist) &&
                    UseChakra(true))
                    return OriginalHook(HowlingFist);

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

            if (IsEnabled(Preset.MNK_AoEUseMasterfulBlitz) &&
                UseMasterfulBlitz(true))
                return OriginalHook(MasterfulBlitz);

            if (LocalPlayer.HasStatus(Buffs.FormlessFist) ||
                ForceSecondOpo(true, IsEnabled(Preset.MNK_AoEUseFiresReply)))
                return ForcedOpoGCD(true);

            if (IsEnabled(Preset.MNK_AoEUseFiresReply) &&
                UseFiresReply(true))
                return FiresReply;

            if (IsEnabled(Preset.MNK_AoEUseWindsReply) &&
                UseWindsReply())
                return WindsReply;

            return DoPerfectBalanceCombo(ref actionID, true)
                ? actionID
                : DoBasicCombo(onAoE: true);
        }
    }

    internal class MNK_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SnapPunch or PouncingCoeurl))
                return actionID;

            if (MNK_BasicCombo_Chakra &&
                Chakra >= 5 && ActionLearned(SteeledMeditation) && CanWeave() &&
                InActionRange(OriginalHook(SteeledMeditation)))
                return OriginalHook(SteelPeak);

            if (DoPerfectBalanceCombo(ref actionID))
                return actionID;

            if (LocalPlayer.HasStatus(Buffs.PerfectBalance))
                return OriginalHook(Bootshine);

            if (MNK_BasicCombo_MasterfulBlitz &&
                ActionLearned(MasterfulBlitz) &&
                !IsOriginal(MasterfulBlitz))
                return OriginalHook(MasterfulBlitz);

            if (!ActionLearned(TrueStrike))
                return Bootshine;

            if (LocalPlayer.HasStatus(Buffs.OpoOpoForm) || LocalPlayer.HasStatus(Buffs.FormlessFist))
                return OpoFormGCD();

            if (LocalPlayer.HasStatus(Buffs.RaptorForm))
                return RaptorFormGCD();

            if (LocalPlayer.HasStatus(Buffs.CoeurlForm))
                return CoeurlFormGCD();

            return OriginalHook(Bootshine);
        }
    }

    internal class MNK_BeastChakras : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNK_Basic_BeastChakras;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (DragonKick or TwinSnakes or Demolish))
                return actionID;

            if (MNK_BasicCombo[0] &&
                actionID is DragonKick)
                return OpoFormGCD();

            if (MNK_BasicCombo[1] &&
                actionID is TwinSnakes)
                return RaptorFormGCD();

            if (MNK_BasicCombo[2] &&
                actionID is Demolish)
                return CoeurlFormGCD();

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
                   ActionLearned(MasterfulBlitz)
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
                Brotherhood when MNK_BH_RoF == 0 && ActionReady(OriginalHook(RiddleOfFire)) && !ActionReady(Brotherhood) => OriginalHook(RiddleOfFire),
                RiddleOfFire when MNK_BH_RoF == 1 && ActionReady(Brotherhood) && !ActionReady(RiddleOfFire) => Brotherhood,
                _ => actionID
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

            return LocalPlayer.HasStatus(Buffs.PerfectBalance) &&
                   ActionLearned(PerfectBalance)
                ? All.Cease
                : actionID;
        }
    }
}
