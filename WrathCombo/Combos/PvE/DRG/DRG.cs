using WrathCombo.CustomComboNS;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.DRG.Config;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class DRG : Melee
{
    internal class DRG_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, TrueThrust))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (UseBattleLitany())
                        return BattleLitany;

                    if (UseLanceCharge())
                        return LanceCharge;

                    if (UseLifeSurge())
                        return LifeSurge;

                    if (UseMirageDive(ignoreDoubleMirageHold: true))
                        return MirageDive;

                    if (UseGeirskogul())
                        return Geirskogul;

                    if (UseWyrmwind())
                        return WyrmwindThrust;

                    if (UseStarcross())
                        return Starcross;

                    if (UseRiseOfTheDragon())
                        return RiseOfTheDragon;

                    if (UseNastrond())
                        return Nastrond;

                    if (Role.CanFeint() && GroupDamageIncoming())
                        return Role.Feint;

                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(30))
                        return Role.Bloodbath;

                    if (RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (CanDRGWeave(0.8f))
                {
                    if (UseHighJump(allowDoubleMirageHold: false))
                        return OriginalHook(Jump);

                    if (UseDragonfireDive())
                        return DragonfireDive;
                }

                if (UseStardiver() && CanDRGWeave(1.5f, true))
                    return Stardiver;
            }

            return !InMeleeRange() && HasBattleTarget()
                ? OutsideOfMelee(actionID, OutsideOfMeleeOptions.SimpleSt)
                : DoBasicCombo(useTrueNorth: true);
        }
    }

    internal class DRG_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, DoomSpike))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (UseBattleLitany())
                        return BattleLitany;

                    if (UseLanceCharge())
                        return LanceCharge;

                    if (UseLifeSurge(true))
                        return LifeSurge;

                    if (UseMirageDive(true, true))
                        return MirageDive;

                    if (UseGeirskogul())
                        return Geirskogul;

                    if (UseWyrmwind())
                        return WyrmwindThrust;

                    if (UseStarcross())
                        return Starcross;

                    if (UseRiseOfTheDragon())
                        return RiseOfTheDragon;

                    if (UseNastrond())
                        return Nastrond;

                    if (Role.CanSecondWind(40))
                        return Role.SecondWind;

                    if (Role.CanBloodBath(30))
                        return Role.Bloodbath;

                    if (RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (CanDRGWeave(0.8f))
                {
                    if (UseHighJump(true))
                        return OriginalHook(Jump);

                    if (UseDragonfireDive())
                        return DragonfireDive;
                }

                if (UseStardiver() && CanDRGWeave(1.5f, true))
                    return Stardiver;
            }

            return !InActionRange(DoomSpike) && HasBattleTarget()
                ? OutsideOfMelee(actionID, OutsideOfMeleeOptions.SimpleAoE)
                : DoBasicCombo(onAoE: true, includeDisembowel: true);
        }
    }

    internal class DRG_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, TrueThrust))
                return actionID;

            if (IsEnabled(Preset.DRG_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_ST_Buffs))
                    {
                        if (IsEnabled(Preset.DRG_ST_BattleLitany) &&
                            UseBattleLitany(BattleLitanyHPThreshold))
                            return BattleLitany;

                        if (IsEnabled(Preset.DRG_ST_LanceCharge) &&
                            UseLanceCharge(LanceChargeHPThreshold))
                            return LanceCharge;

                        if (IsEnabled(Preset.DRG_ST_LifeSurge) &&
                            UseLifeSurge())
                            return LifeSurge;
                    }

                    if (IsEnabled(Preset.DRG_ST_Damage))
                    {
                        if (IsEnabled(Preset.DRG_ST_Mirage) &&
                            UseMirageDive())
                            return MirageDive;

                        if (IsEnabled(Preset.DRG_ST_Geirskogul) &&
                            UseGeirskogul(hpThreshold: GeirskogulHPThreshold()))
                            return Geirskogul;

                        if (IsEnabled(Preset.DRG_ST_Wyrmwind) &&
                            UseWyrmwind())
                            return WyrmwindThrust;

                        if (IsEnabled(Preset.DRG_ST_Starcross) &&
                            UseStarcross())
                            return Starcross;

                        if (IsEnabled(Preset.DRG_ST_RiseOfTheDragon) &&
                            UseRiseOfTheDragon())
                            return RiseOfTheDragon;

                        if (IsEnabled(Preset.DRG_ST_Nastrond) &&
                            UseNastrond())
                            return Nastrond;
                    }

                    if (IsEnabled(Preset.DRG_ST_Feint) &&
                        Role.CanFeint() &&
                        GroupDamageIncoming())
                        return Role.Feint;

                    if (IsEnabled(Preset.DRG_ST_ComboHeals))
                    {
                        if (Role.CanSecondWind(DRG_ST_SecondWindHPThreshold))
                            return Role.SecondWind;

                        if (Role.CanBloodBath(DRG_ST_BloodbathHPThreshold))
                            return Role.Bloodbath;
                    }

                    if (IsEnabled(Preset.DRG_ST_StunInterupt) &&
                        RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (IsEnabled(Preset.DRG_ST_Damage))
                {
                    if (CanDRGWeave(0.8f))
                    {
                        if (IsEnabled(Preset.DRG_ST_HighJump) &&
                            UseHighJump(holdOptions: DRG_ST_JumpMovingOrInRanged, allowDoubleMirageHold: false))
                            return OriginalHook(Jump);

                        if (IsEnabled(Preset.DRG_ST_DragonfireDive) &&
                            UseDragonfireDive(DRG_ST_DragonfireDiveMovingOrInRanged,
                                DragonfireDiveHPThreshold))
                            return DragonfireDive;
                    }

                    if (IsEnabled(Preset.DRG_ST_Stardiver) &&
                        UseStardiver(holdOptions: DRG_ST_StardiverMovingOrInRanged) &&
                        CanDRGWeave(1.5f, true))
                        return Stardiver;
                }
            }

            OutsideOfMeleeOptions stRanged = new()
            {
                GeirskogulHpThreshold = GeirskogulHPThreshold(),
                UseDamage = IsEnabled(Preset.DRG_ST_Damage),
                UseMirage = IsEnabled(Preset.DRG_ST_Mirage),
                UseWyrmwind = IsEnabled(Preset.DRG_ST_Wyrmwind),
                UseStarcross = IsEnabled(Preset.DRG_ST_Starcross),
                UseRiseOfTheDragon = IsEnabled(Preset.DRG_ST_RiseOfTheDragon),
                UseGeirskogul = IsEnabled(Preset.DRG_ST_Geirskogul),
                UseNastrond = IsEnabled(Preset.DRG_ST_Nastrond),
                UseRangedUptime = IsEnabled(Preset.DRG_ST_RangedUptime),
                UseTrueNorth = IsEnabled(Preset.DRG_TrueNorthDynamic),
                TrueNorthCharges = DRG_ManualTN
            };

            return !InMeleeRange() && HasBattleTarget()
                ? OutsideOfMelee(actionID, stRanged)
                : DoBasicCombo(IsEnabled(Preset.DRG_TrueNorthDynamic), trueNorthCharges: DRG_ManualTN);
        }
    }

    internal class DRG_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, DoomSpike))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_AoE_Buffs))
                    {
                        if (IsEnabled(Preset.DRG_AoE_BattleLitany) &&
                            UseBattleLitany(DRG_AoE_BattleLitanyHPThreshold))
                            return BattleLitany;

                        if (IsEnabled(Preset.DRG_AoE_LanceCharge) &&
                            UseLanceCharge(DRG_AoE_LanceChargeHPThreshold))
                            return LanceCharge;

                        if (IsEnabled(Preset.DRG_AoE_LifeSurge) &&
                            UseLifeSurge(true))
                            return LifeSurge;
                    }

                    if (IsEnabled(Preset.DRG_AoE_Damage))
                    {
                        if (IsEnabled(Preset.DRG_AoE_Mirage) &&
                            UseMirageDive(true))
                            return MirageDive;

                        if (IsEnabled(Preset.DRG_AoE_Geirskogul) &&
                            UseGeirskogul(DRG_AoE_GeirskogulHPThreshold))
                            return Geirskogul;

                        if (IsEnabled(Preset.DRG_AoE_Wyrmwind) &&
                            UseWyrmwind())
                            return WyrmwindThrust;

                        if (IsEnabled(Preset.DRG_AoE_Starcross) &&
                            UseStarcross())
                            return Starcross;

                        if (IsEnabled(Preset.DRG_AoE_RiseOfTheDragon) &&
                            UseRiseOfTheDragon())
                            return RiseOfTheDragon;

                        if (IsEnabled(Preset.DRG_AoE_Nastrond) &&
                            UseNastrond())
                            return Nastrond;
                    }

                    if (IsEnabled(Preset.DRG_AoE_ComboHeals))
                    {
                        if (Role.CanSecondWind(DRG_AoE_SecondWindHPThreshold))
                            return Role.SecondWind;

                        if (Role.CanBloodBath(DRG_AoE_BloodbathHPThreshold))
                            return Role.Bloodbath;
                    }

                    if (IsEnabled(Preset.DRG_AoE_StunInterupt) &&
                        RoleActions.Melee.CanLegSweep())
                        return Role.LegSweep;
                }

                if (IsEnabled(Preset.DRG_AoE_Damage))
                {
                    if (CanDRGWeave(0.8f))
                    {
                        if (IsEnabled(Preset.DRG_AoE_HighJump) &&
                            UseHighJump(true))
                            return OriginalHook(Jump);

                        if (IsEnabled(Preset.DRG_AoE_DragonfireDive) &&
                            UseDragonfireDive(DRG_AoE_DragonfireDiveMovingOrInRanged, DRG_AoE_DragonfireDiveHPThreshold))
                            return DragonfireDive;
                    }

                    if (IsEnabled(Preset.DRG_AoE_Stardiver) &&
                        UseStardiver(DRG_AoE_StardiverMovingOrInRanged) &&
                        CanDRGWeave(1.5f, true))
                        return Stardiver;
                }
            }

            OutsideOfMeleeOptions aoeRanged = new()
            {
                OnAoE = true,
                GeirskogulHpThreshold = DRG_AoE_GeirskogulHPThreshold,
                UseDamage = IsEnabled(Preset.DRG_AoE_Damage),
                UseMirage = IsEnabled(Preset.DRG_AoE_Mirage),
                UseWyrmwind = IsEnabled(Preset.DRG_AoE_Wyrmwind),
                UseStarcross = IsEnabled(Preset.DRG_AoE_Starcross),
                UseRiseOfTheDragon = IsEnabled(Preset.DRG_AoE_RiseOfTheDragon),
                UseGeirskogul = IsEnabled(Preset.DRG_AoE_Geirskogul),
                UseNastrond = IsEnabled(Preset.DRG_AoE_Nastrond),
                UseRangedUptime = IsEnabled(Preset.DRG_AoE_RangedUptime),
                IncludeDisembowel = IsEnabled(Preset.DRG_AoE_Disembowel)
            };

            return !InActionRange(DoomSpike) && HasBattleTarget()
                ? OutsideOfMelee(actionID, aoeRanged)
                : DoBasicCombo(onAoE: true, includeDisembowel: IsEnabled(Preset.DRG_AoE_Disembowel));
        }
    }

    internal class DRG_HeavensThrust : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_HeavensThrust;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FullThrust or HeavensThrust))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && ActionLearned(VorpalThrust))
                    return DRG_ChaoticCombo && ActionLearned(Disembowel) &&
                           (ActionLearned(ChaosThrust) && ChaosDebuff is null &&
                            CurrentTarget.CanApplyStatus(ChaoticList[OriginalHook(ChaosThrust)]) ||
                            LocalPlayer.Status(Buffs.PowerSurge).RemainingTimeOrZero() < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (DRG_ChaoticCombo && ComboAction == OriginalHook(Disembowel) && ActionLearned(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (DRG_ChaoticCombo && ComboAction == OriginalHook(ChaosThrust) && ActionLearned(WheelingThrust))
                    return WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && ActionLearned(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && ActionLearned(FangAndClaw))
                    return FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && ActionLearned(Drakesbane))
                    return Drakesbane;
            }

            return OriginalHook(TrueThrust);
        }
    }

    internal class DRG_ChaoticSpring : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ChaoticSpring;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ChaosThrust or ChaoticSpring))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is TrueThrust or RaidenThrust && ActionLearned(Disembowel))
                    return OriginalHook(Disembowel);

                if (ComboAction == OriginalHook(Disembowel) && ActionLearned(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && ActionLearned(WheelingThrust))
                    return WheelingThrust;

                if (ComboAction == WheelingThrust && ActionLearned(Drakesbane))
                    return Drakesbane;
            }

            return OriginalHook(TrueThrust);
        }
    }

    internal class DRG_BurstCDFeature : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_BurstCDFeature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not LanceCharge)
                return actionID;

            return IsOnCooldown(LanceCharge) && ActionReady(BattleLitany)
                ? BattleLitany
                : actionID;
        }
    }
}
