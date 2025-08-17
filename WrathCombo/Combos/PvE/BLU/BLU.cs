using Dalamud.Game.ClientState.Conditions;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class BLU : Caster
{
    public const uint
        RoseOfDestruction = 23275,
        ShockStrike = 11429,
        FeatherRain = 11426,
        JKick = 18325,
        Eruption = 11427,
        SharpenedKnife = 11400,
        GlassDance = 11430,
        SonicBoom = 18308,
        Surpanakha = 18323,
        Nightbloom = 23290,
        MoonFlute = 11415,
        Whistle = 18309,
        Tingle = 23265,
        TripleTrident = 23264,
        MatraMagic = 23285,
        FinalSting = 11407,
        Bristle = 11393,
        PhantomFlurry = 23288,
        PerpetualRay = 18314,
        AngelWhisper = 18317,
        SongOfTorment = 11386,
        RamsVoice = 11419,
        Ultravibration = 23277,
        Devour = 18320,
        Offguard = 11411,
        BadBreath = 11388,
        MagicHammer = 18305,
        WhiteKnightsTour = 18310,
        BlackKnightsTour = 18311,
        PeripheralSynthesis = 23286,
        BasicInstinct = 23276,
        HydroPull = 23282,
        MustardBomb = 23279,
        WingedReprobation = 34576,
        SeaShanty = 34580,
        BeingMortal = 34582,
        BreathOfMagic = 34567,
        MortalFlame = 34579,
        PeatPelt = 34569,
        DeepClean = 34570;

    public static class Buffs
    {
        public const ushort
            MoonFlute = 1718,
            Bristle = 1716,
            WaningNocturne = 1727,
            PhantomFlurry = 2502,
            Tingle = 2492,
            Whistle = 2118,
            TankMimicry = 2124,
            DPSMimicry = 2125,
            BasicInstinct = 2498,
            WingedReprobation = 3640;
    }

    public static class Debuffs
    {
        public const ushort
            Slow = 9,
            Bind = 13,
            Stun = 142,
            SongOfTorment = 273,
            DeepFreeze = 1731,
            Offguard = 1717,
            Malodorous = 1715,
            Conked = 2115,
            Lightheaded = 2501,
            MortalFlame = 3643,
            BreathOfMagic = 3712,
            Begrimed = 3636;
    }

    internal class BLU_BuffedSoT : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_BuffedSoT;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is SongOfTorment)
            {
                if (!HasStatusEffect(Buffs.Bristle) && IsSpellActive(Bristle))
                    return Bristle;
                if (IsSpellActive(SongOfTorment))
                    return SongOfTorment;
            }

            return actionID;
        }
    }

    internal class BLU_Opener : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_Opener;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is MoonFlute)
            {
                //If Triple Trident is saved for Crit/Det builds
                if (GetCooldownRemainingTime(TripleTrident) <= 3 && IsSpellActive(TripleTrident))
                {
                    if (!HasStatusEffect(Buffs.Whistle) && IsSpellActive(Whistle) && !WasLastSpell(Whistle) && IsOffCooldown(JKick))
                        return Whistle;
                    if (!HasStatusEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle) && IsOffCooldown(JKick))
                        return Tingle;
                    if (!HasStatusEffect(Buffs.MoonFlute) && !HasStatusEffect(Buffs.WaningNocturne) && IsSpellActive(MoonFlute) && !WasLastSpell(MoonFlute) && !JustUsed(MoonFlute))
                        return MoonFlute;
                    if (IsOffCooldown(JKick) && IsSpellActive(JKick))
                        return JKick;
                    if (IsOffCooldown(TripleTrident))
                        return TripleTrident;
                }

                //If Triple Trident is used on CD for Crit/Sps builds or Triple Trident isn't active
                if ((GetCooldownRemainingTime(TripleTrident) > 3 && IsSpellActive(TripleTrident)) || !IsSpellActive(TripleTrident))
                {
                    if (!HasStatusEffect(Buffs.Whistle) && IsOffCooldown(JKick) && !WasLastSpell(Whistle) && IsSpellActive(Whistle) && IsOffCooldown(JKick))
                        return Whistle;
                    if (!HasStatusEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle) && IsOffCooldown(JKick))
                        return Tingle;
                    if (!HasStatusEffect(Buffs.MoonFlute) && !HasStatusEffect(Buffs.WaningNocturne) && IsSpellActive(MoonFlute) && !JustUsed(MoonFlute))
                        return MoonFlute;
                    if (IsOffCooldown(JKick) && IsSpellActive(JKick))
                        return JKick;
                }

                if (IsOffCooldown(Nightbloom) && IsSpellActive(Nightbloom))
                    return Nightbloom;
                if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction))
                    return RoseOfDestruction;
                if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain))
                    return FeatherRain.Retarget(MoonFlute,
                        SimpleTarget.HardTarget.IfHostile() ??
                        SimpleTarget.LastHostileHardTarget);
                if (IsOffCooldown(Eruption) && IsSpellActive(Eruption))
                    return Eruption;
                if (!HasStatusEffect(Buffs.Bristle) && IsOffCooldown(Role.Swiftcast) && IsSpellActive(Bristle))
                    return Bristle;
                if (IsOffCooldown(Role.Swiftcast) && LevelChecked(Role.Swiftcast))
                    return Role.Swiftcast;
                if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance))
                    return GlassDance;
                if (GetCooldownRemainingTime(Surpanakha) < 95 && IsSpellActive(Surpanakha))
                    return Surpanakha;
                if (IsOffCooldown(MatraMagic) && HasStatusEffect(Buffs.DPSMimicry) && IsSpellActive(MatraMagic))
                    return MatraMagic;
                if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike))
                    return ShockStrike;
                if ((IsOffCooldown(PhantomFlurry) && IsSpellActive(PhantomFlurry)) || HasStatusEffect(Buffs.PhantomFlurry))
                    return PhantomFlurry;
            }

            return actionID;
        }
    }

    internal class BLU_FinalSting : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_FinalSting;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is FinalSting)
            {
                if (IsEnabled(Preset.BLU_SoloMode) && HasCondition(ConditionFlag.BoundByDuty) && !HasStatusEffect(Buffs.BasicInstinct) && GetPartyMembers().Count == 0 && IsSpellActive(BasicInstinct))
                    return BasicInstinct;
                if (!HasStatusEffect(Buffs.Whistle) && IsSpellActive(Whistle) && !WasLastAction(Whistle))
                    return Whistle;
                if (!HasStatusEffect(Buffs.Tingle) && IsSpellActive(Tingle) && !WasLastSpell(Tingle))
                    return Tingle;
                if (!HasStatusEffect(Buffs.MoonFlute) && !WasLastSpell(MoonFlute) && IsSpellActive(MoonFlute))
                    return MoonFlute;
                if (IsEnabled(Preset.BLU_Primals))
                {
                    if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction))
                        return RoseOfDestruction;
                    if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain))
                        return FeatherRain.Retarget(FinalSting,
                            SimpleTarget.HardTarget.IfHostile() ??
                            SimpleTarget.LastHostileHardTarget);
                    if (IsOffCooldown(Eruption) && IsSpellActive(Eruption))
                        return Eruption;
                    if (IsOffCooldown(MatraMagic) && IsSpellActive(MatraMagic))
                        return MatraMagic;
                    if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance))
                        return GlassDance;
                    if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike))
                        return ShockStrike;
                }

                if (IsOffCooldown(Role.Swiftcast) && LevelChecked(Role.Swiftcast))
                    return Role.Swiftcast;
                if (IsSpellActive(FinalSting))
                    return FinalSting;
            }

            return actionID;
        }
    }

    internal class BLU_Ultravibrate : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_Ultravibrate;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Ultravibration)
            {
                if (IsEnabled(Preset.BLU_HydroPull) && !InMeleeRange() && IsSpellActive(HydroPull))
                    return HydroPull;
                if (!HasStatusEffect(Debuffs.DeepFreeze, CurrentTarget, true) && IsOffCooldown(Ultravibration) && IsSpellActive(RamsVoice))
                    return RamsVoice;

                if (HasStatusEffect(Debuffs.DeepFreeze, CurrentTarget, true))
                {
                    if (IsOffCooldown(Role.Swiftcast))
                        return Role.Swiftcast;
                    if (IsSpellActive(Ultravibration) && IsOffCooldown(Ultravibration))
                        return Ultravibration;
                }
            }

            return actionID;
        }
    }

    internal class BLU_DebuffCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_DebuffCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Devour or Offguard or BadBreath)
            {
                if (!HasStatusEffect(Debuffs.Offguard, CurrentTarget, true) && IsOffCooldown(Offguard) && IsSpellActive(Offguard))
                    return Offguard;
                if (!HasStatusEffect(Debuffs.Malodorous, CurrentTarget, true) && HasStatusEffect(Buffs.TankMimicry) && IsSpellActive(BadBreath))
                    return BadBreath;
                if (IsOffCooldown(Devour) && HasStatusEffect(Buffs.TankMimicry) && IsSpellActive(Devour))
                    return Devour;
                if (Role.CanLucidDream(9000))
                    return Role.LucidDreaming;
            }

            return actionID;
        }
    }

    internal class BLU_Addle : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_Addle;

        protected override uint Invoke(uint actionID) => (actionID is MagicHammer && IsOnCooldown(MagicHammer) && IsOffCooldown(Role.Addle) && !HasStatusEffect(Role.Debuffs.Addle, CurrentTarget) && !HasStatusEffect(Debuffs.Conked, CurrentTarget)) ? Role.Addle : actionID;
    }

    internal class BLU_PrimalCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_PrimalCombo;
        internal static bool surpanakhaReady = false;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is FeatherRain or Eruption)
            {
                if (HasStatusEffect(Buffs.PhantomFlurry))
                    return OriginalHook(PhantomFlurry);

                if (!HasStatusEffect(Buffs.PhantomFlurry))
                {
                    if (IsEnabled(Preset.BLU_PrimalCombo_WingedReprobation) && GetStatusEffect(Buffs.WingedReprobation)?.Param > 1 && IsOffCooldown(WingedReprobation))
                        return OriginalHook(WingedReprobation);

                    if (IsOffCooldown(FeatherRain) && IsSpellActive(FeatherRain) &&
                        (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) || (IsEnabled(Preset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 30 || IsOffCooldown(Nightbloom)))))
                        return FeatherRain.Retarget([FeatherRain, Eruption],
                            SimpleTarget.HardTarget.IfHostile() ??
                            SimpleTarget.LastHostileHardTarget);
                    if (IsOffCooldown(Eruption) && IsSpellActive(Eruption) &&
                        (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) || (IsEnabled(Preset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 30 || IsOffCooldown(Nightbloom)))))
                        return Eruption;
                    if (IsOffCooldown(ShockStrike) && IsSpellActive(ShockStrike) &&
                        (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) || (IsEnabled(Preset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 60 || IsOffCooldown(Nightbloom)))))
                        return ShockStrike;
                    if (IsOffCooldown(RoseOfDestruction) && IsSpellActive(RoseOfDestruction) &&
                        (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) || (IsEnabled(Preset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 30 || IsOffCooldown(Nightbloom)))))
                        return RoseOfDestruction;
                    if (IsOffCooldown(GlassDance) && IsSpellActive(GlassDance) &&
                        (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) || (IsEnabled(Preset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 90 || IsOffCooldown(Nightbloom)))))
                        return GlassDance;
                    if (IsEnabled(Preset.BLU_PrimalCombo_JKick) && IsOffCooldown(JKick) && IsSpellActive(JKick) &&
                        (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) || (IsEnabled(Preset.BLU_PrimalCombo_Pool) && (GetCooldownRemainingTime(Nightbloom) > 60 || IsOffCooldown(Nightbloom)))))
                        return JKick;
                    if (IsEnabled(Preset.BLU_PrimalCombo_Nightbloom) && IsOffCooldown(Nightbloom) && IsSpellActive(Nightbloom))
                        return Nightbloom;
                    if (IsEnabled(Preset.BLU_PrimalCombo_Matra) && IsOffCooldown(MatraMagic) && IsSpellActive(MatraMagic))
                        return MatraMagic;
                    if (IsEnabled(Preset.BLU_PrimalCombo_Suparnakha) && IsSpellActive(Surpanakha))
                    {
                        if (GetRemainingCharges(Surpanakha) == 4)
                            surpanakhaReady = true;
                        if (surpanakhaReady && GetRemainingCharges(Surpanakha) > 0)
                            return Surpanakha;
                        if (GetRemainingCharges(Surpanakha) == 0)
                            surpanakhaReady = false;
                    }

                    if (IsEnabled(Preset.BLU_PrimalCombo_WingedReprobation) && IsSpellActive(WingedReprobation) && IsOffCooldown(WingedReprobation))
                        return OriginalHook(WingedReprobation);

                    if (IsEnabled(Preset.BLU_PrimalCombo_SeaShanty) && IsSpellActive(SeaShanty) && IsOffCooldown(SeaShanty))
                        return SeaShanty;

                    if (IsEnabled(Preset.BLU_PrimalCombo_PhantomFlurry) && IsOffCooldown(PhantomFlurry) && IsSpellActive(PhantomFlurry))
                        return PhantomFlurry;
                }
            }

            return actionID;
        }
    }

    internal class BLU_KnightCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_KnightCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is WhiteKnightsTour or BlackKnightsTour)
            {
                if (HasStatusEffect(Debuffs.Slow, CurrentTarget) && IsSpellActive(BlackKnightsTour))
                    return BlackKnightsTour;
                if (HasStatusEffect(Debuffs.Bind, CurrentTarget) && IsSpellActive(WhiteKnightsTour))
                    return WhiteKnightsTour;
            }

            return actionID;
        }
    }

    internal class BLU_LightHeadedCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_LightHeadedCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is PeripheralSynthesis)
            {
                if (!HasStatusEffect(Debuffs.Lightheaded, CurrentTarget) && IsSpellActive(PeripheralSynthesis))
                    return PeripheralSynthesis;
                if (HasStatusEffect(Debuffs.Lightheaded, CurrentTarget) && IsSpellActive(MustardBomb))
                    return MustardBomb;
            }

            return actionID;
        }
    }

    internal class BLU_PerpetualRayStunCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_PerpetualRayStunCombo;

        protected override uint Invoke(uint actionID) => (actionID is PerpetualRay && (HasStatusEffect(Debuffs.Stun, CurrentTarget, true) || WasLastAction(PerpetualRay)) && IsSpellActive(SharpenedKnife) && InMeleeRange()) ? SharpenedKnife : actionID;
    }

    internal class BLU_MeleeCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_MeleeCombo;

        protected override uint Invoke(uint actionID) => (actionID is SonicBoom && GetTargetDistance() <= 3 && IsSpellActive(SharpenedKnife)) ? SharpenedKnife : actionID;
    }

    internal class BLU_PeatClean : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_PeatClean;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is DeepClean)
            {
                if (IsSpellActive(PeatPelt) && !HasStatusEffect(Debuffs.Begrimed, CurrentTarget))
                    return PeatPelt;
            }

            return actionID;
        }
    }
    internal class BLU_NewMoonFluteOpener : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLU_NewMoonFluteOpener;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is MoonFlute)
            {
                if (!HasStatusEffect(Buffs.MoonFlute))
                {
                    if (IsSpellActive(Whistle) && !HasStatusEffect(Buffs.Whistle) && !WasLastAction(Whistle))
                        return Whistle;

                    if (IsSpellActive(Tingle) && !HasStatusEffect(Buffs.Tingle))
                        return Tingle;

                    if (IsSpellActive(RoseOfDestruction) && GetCooldown(RoseOfDestruction).CooldownRemaining < 1f)
                        return RoseOfDestruction;

                    if (IsSpellActive(MoonFlute) && !JustUsed(MoonFlute))
                        return MoonFlute;
                }

                if (IsSpellActive(JKick) && IsOffCooldown(JKick))
                    return JKick;

                if (IsSpellActive(TripleTrident) && IsOffCooldown(TripleTrident))
                    return TripleTrident;

                if (IsSpellActive(Nightbloom) && IsOffCooldown(Nightbloom))
                    return Nightbloom;

                if (IsEnabled(Preset.BLU_NewMoonFluteOpener_DoTOpener))
                {
                    if ((!HasStatusEffect(Debuffs.BreathOfMagic, CurrentTarget, true) && IsSpellActive(BreathOfMagic)) || (!HasStatusEffect(Debuffs.MortalFlame, CurrentTarget, true) && IsSpellActive(MortalFlame)))
                    {
                        if (IsSpellActive(Bristle) && !HasStatusEffect(Buffs.Bristle))
                            return Bristle;

                        if (IsSpellActive(FeatherRain) && IsOffCooldown(FeatherRain))
                            return FeatherRain.Retarget(MoonFlute,
                                SimpleTarget.HardTarget.IfHostile() ??
                                SimpleTarget.LastHostileHardTarget);

                        if (IsSpellActive(SeaShanty) && IsOffCooldown(SeaShanty))
                            return SeaShanty;

                        if (IsSpellActive(BreathOfMagic) && !HasStatusEffect(Debuffs.BreathOfMagic, CurrentTarget, true))
                            return BreathOfMagic;
                        else if (IsSpellActive(MortalFlame) && !HasStatusEffect(Debuffs.MortalFlame, CurrentTarget, true))
                            return MortalFlame;
                    }
                }
                else
                {
                    if (IsSpellActive(WingedReprobation) && IsOffCooldown(WingedReprobation) && !WasLastSpell(WingedReprobation) && !WasLastAbility(FeatherRain) && (!HasStatusEffect(Buffs.WingedReprobation) || GetStatusEffect(Buffs.WingedReprobation)?.Param < 2))
                        return WingedReprobation;

                    if (IsSpellActive(FeatherRain) && IsOffCooldown(FeatherRain))
                        return FeatherRain.Retarget(MoonFlute,
                            SimpleTarget.HardTarget.IfHostile() ??
                            SimpleTarget.LastHostileHardTarget);

                    if (IsSpellActive(SeaShanty) && IsOffCooldown(SeaShanty))
                        return SeaShanty;
                }

                if (IsSpellActive(WingedReprobation) && IsOffCooldown(WingedReprobation) && !WasLastAbility(ShockStrike) && GetStatusEffect(Buffs.WingedReprobation)?.Param < 2)
                    return WingedReprobation;

                if (IsSpellActive(ShockStrike) && IsOffCooldown(ShockStrike))
                    return ShockStrike;

                if (IsSpellActive(BeingMortal) && IsOffCooldown(BeingMortal) && IsNotEnabled(Preset.BLU_NewMoonFluteOpener_DoTOpener))
                    return BeingMortal;

                if (IsSpellActive(Bristle) && !HasStatusEffect(Buffs.Bristle) && IsOffCooldown(MatraMagic) && IsSpellActive(MatraMagic))
                    return Bristle;

                if (IsOffCooldown(Role.Swiftcast))
                    return Role.Swiftcast;

                if (IsSpellActive(Surpanakha))
                {
                    if (GetRemainingCharges(Surpanakha) > 0)
                        return Surpanakha;
                }

                if (IsSpellActive(MatraMagic) && HasStatusEffect(Role.Buffs.Swiftcast))
                    return MatraMagic;

                if (IsSpellActive(BeingMortal) && IsOffCooldown(BeingMortal) && IsEnabled(Preset.BLU_NewMoonFluteOpener_DoTOpener))
                    return BeingMortal;

                if (IsSpellActive(PhantomFlurry) && IsOffCooldown(PhantomFlurry))
                    return PhantomFlurry;

                if (HasStatusEffect(Buffs.PhantomFlurry) && GetStatusEffect(Buffs.PhantomFlurry)?.RemainingTime < 2)
                    return OriginalHook(PhantomFlurry);

                if (HasStatusEffect(Buffs.MoonFlute))
                    return All.SavageBlade;
            }

            return actionID;
        }
    }
}
