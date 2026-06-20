using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.DRG.Config;
namespace WrathCombo.Combos.PvE;

internal partial class DRG : Melee
{
    internal class DRG_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TrueThrust)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (CanBattleLitany())
                        return BattleLitany;

                    if (CanLanceCharge())
                        return LanceCharge;

                    if (CanLifeSurge())
                        return LifeSurge;

                    if (CanMirageDive(ignoreDoubleMirageHold: true))
                        return MirageDive;

                    if (CanUseGeirskogul())
                        return Geirskogul;

                    if (CanUseWyrmwind())
                        return WyrmwindThrust;

                    if (CanStarcross())
                        return Starcross;

                    if (CanRiseOfTheDragon())
                        return RiseOfTheDragon;

                    if (CanNastrond())
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
                    if (CanHighJump(allowDoubleMirageHold: false))
                        return OriginalHook(Jump);

                    if (CanDragonfireDive())
                        return DragonfireDive;
                }

                if (CanStardiver() && CanDRGWeave(1.5f, true))
                    return Stardiver;
            }

            return !InMeleeRange() && HasBattleTarget()
                ? OutsideOfMelee(actionID, OutsideOfMeleeOptions.SimpleSt)
                : DoBasicCombo(actionID, true);
        }
    }

    internal class DRG_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DoomSpike)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (CanBattleLitany())
                        return BattleLitany;

                    if (CanLanceCharge())
                        return LanceCharge;

                    if (CanLifeSurge(true))
                        return LifeSurge;

                    if (CanMirageDive(true, true))
                        return MirageDive;

                    if (CanUseGeirskogul(true))
                        return Geirskogul;

                    if (CanUseWyrmwind())
                        return WyrmwindThrust;

                    if (CanStarcross())
                        return Starcross;

                    if (CanRiseOfTheDragon())
                        return RiseOfTheDragon;

                    if (CanNastrond())
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
                    if (CanHighJump(true))
                        return OriginalHook(Jump);

                    if (CanDragonfireDive())
                        return DragonfireDive;
                }

                if (CanStardiver() && CanDRGWeave(1.5f, true))
                    return Stardiver;
            }

            return !InActionRange(DoomSpike) && HasBattleTarget()
                ? OutsideOfMelee(actionID, OutsideOfMeleeOptions.SimpleAoE)
                : DoBasicCombo(actionID, onAoE: true, includeDisembowel: true);
        }
    }

    internal class DRG_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TrueThrust)
                return actionID;

            if (IsEnabled(Preset.DRG_ST_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_ST_Buffs))
                    {
                        if (IsEnabled(Preset.DRG_ST_BattleLitany) &&
                            CanBattleLitany(BattleLitanyHPThreshold))
                            return BattleLitany;

                        if (IsEnabled(Preset.DRG_ST_LanceCharge) &&
                            CanLanceCharge(LanceChargeHPThreshold))
                            return LanceCharge;

                        if (IsEnabled(Preset.DRG_ST_LifeSurge) &&
                            CanLifeSurge())
                            return LifeSurge;
                    }

                    if (IsEnabled(Preset.DRG_ST_Damage))
                    {
                        if (IsEnabled(Preset.DRG_ST_Mirage) &&
                            CanMirageDive())
                            return MirageDive;

                        if (IsEnabled(Preset.DRG_ST_Geirskogul) &&
                            CanUseGeirskogul(hpThreshold: GeirskogulHPThreshold()))
                            return Geirskogul;

                        if (IsEnabled(Preset.DRG_ST_Wyrmwind) &&
                            CanUseWyrmwind())
                            return WyrmwindThrust;

                        if (IsEnabled(Preset.DRG_ST_Starcross) &&
                            CanStarcross())
                            return Starcross;

                        if (IsEnabled(Preset.DRG_ST_RiseOfTheDragon) &&
                            CanRiseOfTheDragon())
                            return RiseOfTheDragon;

                        if (IsEnabled(Preset.DRG_ST_Nastrond) &&
                            CanNastrond())
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
                            CanHighJump(holdOptions: DRG_ST_JumpMovingOrInRanged, allowDoubleMirageHold: false))
                            return OriginalHook(Jump);

                        if (IsEnabled(Preset.DRG_ST_DragonfireDive) &&
                            CanDragonfireDive(DRG_ST_DragonfireDiveMovingOrInRanged,
                                DragonfireDiveHPThreshold))
                            return DragonfireDive;
                    }

                    if (IsEnabled(Preset.DRG_ST_Stardiver) &&
                        CanStardiver(holdOptions: DRG_ST_StardiverMovingOrInRanged) &&
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
                : DoBasicCombo(actionID, IsEnabled(Preset.DRG_TrueNorthDynamic), trueNorthCharges: DRG_ManualTN);
        }
    }

    internal class DRG_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRG_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DoomSpike)
                return actionID;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (CanWeaveOgcds())
            {
                if (CanDRGWeave())
                {
                    if (IsEnabled(Preset.DRG_AoE_Buffs))
                    {
                        if (IsEnabled(Preset.DRG_AoE_BattleLitany) &&
                            CanBattleLitany(DRG_AoE_BattleLitanyHPThreshold))
                            return BattleLitany;

                        if (IsEnabled(Preset.DRG_AoE_LanceCharge) &&
                            CanLanceCharge(DRG_AoE_LanceChargeHPThreshold))
                            return LanceCharge;

                        if (IsEnabled(Preset.DRG_AoE_LifeSurge) &&
                            CanLifeSurge(true))
                            return LifeSurge;
                    }

                    if (IsEnabled(Preset.DRG_AoE_Damage))
                    {
                        if (IsEnabled(Preset.DRG_AoE_Mirage) &&
                            CanMirageDive(true))
                            return MirageDive;

                        if (IsEnabled(Preset.DRG_AoE_Geirskogul) &&
                            CanUseGeirskogul(true, DRG_AoE_GeirskogulHPThreshold))
                            return Geirskogul;

                        if (IsEnabled(Preset.DRG_AoE_Wyrmwind) &&
                            CanUseWyrmwind())
                            return WyrmwindThrust;

                        if (IsEnabled(Preset.DRG_AoE_Starcross) &&
                            CanStarcross())
                            return Starcross;

                        if (IsEnabled(Preset.DRG_AoE_RiseOfTheDragon) &&
                            CanRiseOfTheDragon())
                            return RiseOfTheDragon;

                        if (IsEnabled(Preset.DRG_AoE_Nastrond) &&
                            CanNastrond())
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
                            CanHighJump(true))
                            return OriginalHook(Jump);

                        if (IsEnabled(Preset.DRG_AoE_DragonfireDive) &&
                            CanDragonfireDive(DRG_AoE_DragonfireDiveMovingOrInRanged, DRG_AoE_DragonfireDiveHPThreshold))
                            return DragonfireDive;
                    }

                    if (IsEnabled(Preset.DRG_AoE_Stardiver) &&
                        CanStardiver(DRG_AoE_StardiverMovingOrInRanged) &&
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
                : DoBasicCombo(actionID, onAoE: true, includeDisembowel: IsEnabled(Preset.DRG_AoE_Disembowel));
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
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                    return DRG_ChaoticCombo && LevelChecked(Disembowel) &&
                           (LevelChecked(ChaosThrust) && ChaosDebuff is null &&
                            CanApplyStatus(CurrentTarget, ChaoticList[OriginalHook(ChaosThrust)]) ||
                            GetStatusEffectRemainingTime(Buffs.PowerSurge) < 15)
                        ? OriginalHook(Disembowel)
                        : OriginalHook(VorpalThrust);

                if (DRG_ChaoticCombo && ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (DRG_ChaoticCombo && ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return WheelingThrust;

                if (ComboAction == OriginalHook(VorpalThrust) && LevelChecked(FullThrust))
                    return OriginalHook(FullThrust);

                if (ComboAction == OriginalHook(FullThrust) && LevelChecked(FangAndClaw))
                    return FangAndClaw;

                if (ComboAction is WheelingThrust or FangAndClaw && LevelChecked(Drakesbane))
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
                if (ComboAction is TrueThrust or RaidenThrust && LevelChecked(Disembowel))
                    return OriginalHook(Disembowel);

                if (ComboAction == OriginalHook(Disembowel) && LevelChecked(ChaosThrust))
                    return OriginalHook(ChaosThrust);

                if (ComboAction == OriginalHook(ChaosThrust) && LevelChecked(WheelingThrust))
                    return WheelingThrust;

                if (ComboAction == WheelingThrust && LevelChecked(Drakesbane))
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
