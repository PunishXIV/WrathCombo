#region

using Dalamud.Game.ClientState.Conditions;
using WrathCombo.CustomComboNS;
using Preset = WrathCombo.Combos.CustomComboPreset;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace WrathCombo.Combos.PvE;

#endregion

internal partial class BLU
{
    #region Openers

    internal class BLU_NewMoonFluteOpener : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_NewMoonFluteOpener;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MoonFlute_Spell39) return actionID;

            if (!HasEffect(Buffs.MoonFlute))
            {
                if (IsSpellActive(Whistle_Spell64) &&
                    !HasEffect(Buffs.Whistle) &&
                    !WasLastAction(Whistle_Spell64))
                    return Whistle_Spell64;

                if (IsSpellActive(Tingle_Spell82) &&
                    !HasEffect(Buffs.Tingle))
                    return Tingle_Spell82;

                if (IsSpellActive(RoseofDestruction_Spell90) &&
                    GetCooldown(RoseofDestruction_Spell90)
                        .CooldownRemaining < 1f)
                    return RoseofDestruction_Spell90;

                if (IsSpellActive(MoonFlute_Spell39))
                    return MoonFlute_Spell39;
            }

            if (IsSpellActive(JKick_Spell80) && IsOffCooldown(JKick_Spell80))
                return JKick_Spell80;

            if (IsSpellActive(TripleTrident_Spell81) &&
                IsOffCooldown(TripleTrident_Spell81))
                return TripleTrident_Spell81;

            if (IsSpellActive(Nightbloom_Spell104) &&
                IsOffCooldown(Nightbloom_Spell104))
                return Nightbloom_Spell104;

            if (IsEnabled(Preset.BLU_NewMoonFluteOpener_DoTOpener))
            {
                if ((!TargetHasEffectAny(Debuffs.BreathOfMagic) &&
                     IsSpellActive(BreathofMagic_Spell109)) ||
                    (!TargetHasEffectAny(Debuffs.MortalFlame) &&
                     IsSpellActive(MortalFlame_Spell121)))
                {
                    if (IsSpellActive(Bristle_Spell12) &&
                        !HasEffect(Buffs.Bristle))
                        return Bristle_Spell12;

                    if (IsSpellActive(FeatherRain_Spell44) &&
                        IsOffCooldown(FeatherRain_Spell44))
                        return FeatherRain_Spell44;

                    if (IsSpellActive(SeaShanty_Spell122) &&
                        IsOffCooldown(SeaShanty_Spell122))
                        return SeaShanty_Spell122;

                    if (IsSpellActive(BreathofMagic_Spell109) &&
                        !TargetHasEffectAny(Debuffs.BreathOfMagic))
                        return BreathofMagic_Spell109;
                    if (IsSpellActive(MortalFlame_Spell121) &&
                        !TargetHasEffectAny(Debuffs.MortalFlame))
                        return MortalFlame_Spell121;
                }
            }
            else
            {
                if (IsSpellActive(WingedReprobation_Spell118) &&
                    IsOffCooldown(WingedReprobation_Spell118) &&
                    !WasLastSpell(WingedReprobation_Spell118) &&
                    !WasLastAbility(FeatherRain_Spell44) &&
                    (!HasEffect(Buffs.WingedReprobation) ||
                     FindEffect(Buffs.WingedReprobation)?.Param < 2))
                    return WingedReprobation_Spell118;

                if (IsSpellActive(FeatherRain_Spell44) &&
                    IsOffCooldown(FeatherRain_Spell44))
                    return FeatherRain_Spell44;

                if (IsSpellActive(SeaShanty_Spell122) &&
                    IsOffCooldown(SeaShanty_Spell122))
                    return SeaShanty_Spell122;
            }

            if (IsSpellActive(WingedReprobation_Spell118) &&
                IsOffCooldown(WingedReprobation_Spell118) &&
                !WasLastAbility(ShockStrike_Spell47) &&
                FindEffect(Buffs.WingedReprobation)?.Param < 2)
                return WingedReprobation_Spell118;

            if (IsSpellActive(ShockStrike_Spell47) &&
                IsOffCooldown(ShockStrike_Spell47))
                return ShockStrike_Spell47;

            if (IsSpellActive(BeingMortal_Spell124) &&
                IsOffCooldown(BeingMortal_Spell124) &&
                IsNotEnabled(Preset
                    .BLU_NewMoonFluteOpener_DoTOpener))
                return BeingMortal_Spell124;

            if (IsSpellActive(Bristle_Spell12) &&
                !HasEffect(Buffs.Bristle) &&
                IsOffCooldown(MatraMagic_Spell100) &&
                IsSpellActive(MatraMagic_Spell100))
                return Bristle_Spell12;

            if (IsOffCooldown(All.Swiftcast))
                return All.Swiftcast;

            if (IsSpellActive(Surpanakha_Spell78) &&
                GetRemainingCharges(Surpanakha_Spell78) > 0)
                return Surpanakha_Spell78;

            if (IsSpellActive(MatraMagic_Spell100) &&
                HasEffect(All.Buffs.Swiftcast))
                return MatraMagic_Spell100;

            if (IsSpellActive(BeingMortal_Spell124) &&
                IsOffCooldown(BeingMortal_Spell124) &&
                IsEnabled(Preset
                    .BLU_NewMoonFluteOpener_DoTOpener))
                return BeingMortal_Spell124;

            if (IsSpellActive(PhantomFlurry_Spell103) &&
                IsOffCooldown(PhantomFlurry_Spell103))
                return PhantomFlurry_Spell103;

            if (HasEffect(Buffs.PhantomFlurry) &&
                FindEffect(Buffs.PhantomFlurry)?.RemainingTime < 2)
                return OriginalHook(PhantomFlurry_Spell103);

            if (HasEffect(Buffs.MoonFlute))
                return All.SavageBlade;

            return actionID;
        }
    }

    #endregion

    #region Primal Combo

    internal class BLU_PrimalCombo : CustomCombo
    {
        internal static bool surpanakhaReady;

        protected internal override Preset Preset { get; } =
            Preset.BLU_PrimalCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FeatherRain_Spell44 or Eruption_Spell45))
                return actionID;

            if (HasEffect(Buffs.PhantomFlurry))
                return OriginalHook(PhantomFlurry_Spell103);

            if (HasEffect(Buffs.PhantomFlurry)) return actionID;

            if (IsEnabled(Preset
                    .BLU_PrimalCombo_WingedReprobation) &&
                FindEffect(Buffs.WingedReprobation)?.Param > 1 &&
                IsOffCooldown(WingedReprobation_Spell118))
                return OriginalHook(WingedReprobation_Spell118);

            if (IsOffCooldown(FeatherRain_Spell44) &&
                IsSpellActive(FeatherRain_Spell44) &&
                (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) ||
                 (IsEnabled(Preset.BLU_PrimalCombo_Pool) &&
                  (GetCooldownRemainingTime(Nightbloom_Spell104) > 30 ||
                   IsOffCooldown(Nightbloom_Spell104)))))
                return FeatherRain_Spell44;
            if (IsOffCooldown(Eruption_Spell45) &&
                IsSpellActive(Eruption_Spell45) &&
                (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) ||
                 (IsEnabled(Preset.BLU_PrimalCombo_Pool) &&
                  (GetCooldownRemainingTime(Nightbloom_Spell104) > 30 ||
                   IsOffCooldown(Nightbloom_Spell104)))))
                return Eruption_Spell45;
            if (IsOffCooldown(ShockStrike_Spell47) &&
                IsSpellActive(ShockStrike_Spell47) &&
                (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) ||
                 (IsEnabled(Preset.BLU_PrimalCombo_Pool) &&
                  (GetCooldownRemainingTime(Nightbloom_Spell104) > 60 ||
                   IsOffCooldown(Nightbloom_Spell104)))))
                return ShockStrike_Spell47;
            if (IsOffCooldown(RoseofDestruction_Spell90) &&
                IsSpellActive(RoseofDestruction_Spell90) &&
                (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) ||
                 (IsEnabled(Preset.BLU_PrimalCombo_Pool) &&
                  (GetCooldownRemainingTime(Nightbloom_Spell104) > 30 ||
                   IsOffCooldown(Nightbloom_Spell104)))))
                return RoseofDestruction_Spell90;
            if (IsOffCooldown(GlassDance_Spell48) &&
                IsSpellActive(GlassDance_Spell48) &&
                (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) ||
                 (IsEnabled(Preset.BLU_PrimalCombo_Pool) &&
                  (GetCooldownRemainingTime(Nightbloom_Spell104) > 90 ||
                   IsOffCooldown(Nightbloom_Spell104)))))
                return GlassDance_Spell48;
            if (IsEnabled(Preset.BLU_PrimalCombo_JKick) &&
                IsOffCooldown(JKick_Spell80) &&
                IsSpellActive(JKick_Spell80) &&
                (IsNotEnabled(Preset.BLU_PrimalCombo_Pool) ||
                 (IsEnabled(Preset.BLU_PrimalCombo_Pool) &&
                  (GetCooldownRemainingTime(Nightbloom_Spell104) > 60 ||
                   IsOffCooldown(Nightbloom_Spell104)))))
                return JKick_Spell80;
            if (IsEnabled(Preset
                    .BLU_PrimalCombo_Nightbloom) &&
                IsOffCooldown(Nightbloom_Spell104) &&
                IsSpellActive(Nightbloom_Spell104))
                return Nightbloom_Spell104;
            if (IsEnabled(Preset.BLU_PrimalCombo_Matra) &&
                IsOffCooldown(MatraMagic_Spell100) &&
                IsSpellActive(MatraMagic_Spell100))
                return MatraMagic_Spell100;
            if (IsEnabled(Preset
                    .BLU_PrimalCombo_Suparnakha) &&
                IsSpellActive(Surpanakha_Spell78))
            {
                if (GetRemainingCharges(Surpanakha_Spell78) == 4)
                    surpanakhaReady = true;
                if (surpanakhaReady &&
                    GetRemainingCharges(Surpanakha_Spell78) > 0)
                    return Surpanakha_Spell78;
                if (GetRemainingCharges(Surpanakha_Spell78) == 0)
                    surpanakhaReady = false;
            }

            if (IsEnabled(Preset
                    .BLU_PrimalCombo_WingedReprobation) &&
                IsSpellActive(WingedReprobation_Spell118) &&
                IsOffCooldown(WingedReprobation_Spell118))
                return OriginalHook(WingedReprobation_Spell118);

            if (IsEnabled(Preset.BLU_PrimalCombo_SeaShanty) &&
                IsSpellActive(SeaShanty_Spell122) &&
                IsOffCooldown(SeaShanty_Spell122))
                return SeaShanty_Spell122;

            if (IsEnabled(
                    Preset.BLU_PrimalCombo_PhantomFlurry) &&
                IsOffCooldown(PhantomFlurry_Spell103) &&
                IsSpellActive(PhantomFlurry_Spell103))
                return PhantomFlurry_Spell103;

            return actionID;
        }
    }

    #endregion

    #region General Combos

    internal class BLU_FinalSting : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_FinalSting;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FinalSting_Spell8) return actionID;

            if (IsEnabled(Preset.BLU_SoloMode) &&
                HasCondition(ConditionFlag.BoundByDuty) &&
                !HasEffect(Buffs.BasicInstinct) &&
                GetPartyMembers().Count == 0 &&
                IsSpellActive(BasicInstinct_Spell91))
                return BasicInstinct_Spell91;
            if (!HasEffect(Buffs.Whistle) &&
                IsSpellActive(Whistle_Spell64) &&
                !WasLastAction(Whistle_Spell64))
                return Whistle_Spell64;
            if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle_Spell82) &&
                !WasLastSpell(Tingle_Spell82))
                return Tingle_Spell82;
            if (!HasEffect(Buffs.MoonFlute) &&
                !WasLastSpell(MoonFlute_Spell39) &&
                IsSpellActive(MoonFlute_Spell39))
                return MoonFlute_Spell39;

            if (IsEnabled(Preset.BLU_Primals))
            {
                if (IsOffCooldown(RoseofDestruction_Spell90) &&
                    IsSpellActive(RoseofDestruction_Spell90))
                    return RoseofDestruction_Spell90;
                if (IsOffCooldown(FeatherRain_Spell44) &&
                    IsSpellActive(FeatherRain_Spell44))
                    return FeatherRain_Spell44;
                if (IsOffCooldown(Eruption_Spell45) &&
                    IsSpellActive(Eruption_Spell45))
                    return Eruption_Spell45;
                if (IsOffCooldown(MatraMagic_Spell100) &&
                    IsSpellActive(MatraMagic_Spell100))
                    return MatraMagic_Spell100;
                if (IsOffCooldown(GlassDance_Spell48) &&
                    IsSpellActive(GlassDance_Spell48))
                    return GlassDance_Spell48;
                if (IsOffCooldown(ShockStrike_Spell47) &&
                    IsSpellActive(ShockStrike_Spell47))
                    return ShockStrike_Spell47;
            }

            if (IsOffCooldown(All.Swiftcast) && LevelChecked(All.Swiftcast))
                return All.Swiftcast;
            if (IsSpellActive(FinalSting_Spell8))
                return FinalSting_Spell8;

            return actionID;
        }
    }

    internal class BLU_Ultravibrate : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_Ultravibrate;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Ultravibration_Spell92) return actionID;

            // Setup for Ultravibration
            if (IsEnabled(Preset.BLU_HydroPull) &&
                !InMeleeRange() && IsSpellActive(HydroPull_Spell97))
                return HydroPull_Spell97;
            if (!TargetHasEffectAny(Debuffs.DeepFreeze) &&
                IsOffCooldown(Ultravibration_Spell92) &&
                IsSpellActive(RamsVoice_Spell33))
                return RamsVoice_Spell33;

            if (!TargetHasEffectAny(Debuffs.DeepFreeze)) return actionID;

            // Ultravibration
            if (IsOffCooldown(All.Swiftcast))
                return All.Swiftcast;
            if (IsSpellActive(Ultravibration_Spell92) &&
                IsOffCooldown(Ultravibration_Spell92))
                return Ultravibration_Spell92;

            return actionID;
        }
    }

    #endregion

    #region DPS Combos

    internal class BLU_TridentCombo : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_TridentCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TripleTrident_Spell81) return actionID;

            // Show cooldown
            if (IsOnCooldown(TripleTrident_Spell81))
                return TripleTrident_Spell81;

            // Buff
            if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle_Spell64))
                return Whistle_Spell64;
            if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle_Spell82) &&
                HasEffect(Buffs.Whistle))
                return Tingle_Spell82;

            // Triple Trident
            if (IsSpellActive(TripleTrident_Spell81) &&
                HasEffect(Buffs.Tingle) && HasEffect(Buffs.Whistle))
                return TripleTrident_Spell81;

            return actionID;
        }
    }

    internal class BLU_DPS_DoT : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_DPS_DoT;

        public uint getDoT() => Invoke(BypassAction);

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SongofTorment_Spell9 or BypassAction))
                return actionID;

            var dotHelper = new DoTs(
                Config.BLU_DPS_DoT_WasteProtection_HP,
                Config.BLU_DPS_DoT_WasteProtection_Time);

            // Waste protection
            if (IsEnabled(Preset.BLU_Tank_DoT_WasteProtection) &&
                HasTarget() && !dotHelper.AnyDotsWanted() &&
                actionID != BypassAction)
                return All.SavageBlade;

            // Use DoT
            if (dotHelper.TryGet(out var dotAction))
                return dotAction;

            return actionID;
        }
    }

    #endregion

    #region Tank Combos

    internal class BLU_Tank_Advanced : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_Tank_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not GoblinPunch_Spell105) return actionID;

            // Pre Pull
            if (!InCombat() && HasTarget())
            {
                if (IsEnabled(Preset.BLU_Tank_Advanced_DoTs) &&
                    IsEnabled(Preset.BLU_Tank_DoT_Torment))
                    return SongofTorment_Spell9;

                if (IsEnabled(Preset.BLU_Tank_Advanced_Uptime))
                    return SonicBoom_Spell63;
            }

            // Surpanakha dump
            if (IsEnabled(Preset.BLU_Tank_Advanced_Surpanakha) &&
                WasLastAction(Surpanakha_Spell78) &&
                HasCharges(Surpanakha_Spell78))
                return Surpanakha_Spell78;

            #region oGCDs

            if (CanWeave())
            {
                // J Kick
                if (IsEnabled(Preset.BLU_Tank_Advanced_JKick) &&
                    IsOffCooldown(JKick_Spell80) &&
                    InMeleeRange() && IsSpellActive(JKick_Spell80))
                    return JKick_Spell80;

                // Surpanakha
                if (IsEnabled(Preset.BLU_Tank_Advanced_Surpanakha) &&
                    CanWeave() &&
                    GetRemainingCharges(Surpanakha_Spell78) > 3)
                    return Surpanakha_Spell78;

                // Lucid Dreaming
                if (Role.CanLucidDream(9000))
                    return Role.LucidDreaming;
            }

            #endregion

            // Uptime Sonic Boom
            if (IsEnabled(Preset.BLU_Tank_Advanced_Uptime) &&
                IsSpellActive(SonicBoom_Spell63) &&
                IsOffCooldown(SonicBoom_Spell63) &&
                !InMeleeRange())
                return SonicBoom_Spell63;

            // Include DoTs
            BLU_Tank_DoT DoTCheck = new();
            if (IsEnabled(Preset.BLU_Tank_Advanced_DoTs))
            {
                var DoTCheckOutput = DoTCheck.getDoT();
                if (DoTCheckOutput != BypassAction)
                    return DoTCheckOutput;
            }

            return actionID;
        }
    }

    internal class BLU_Tank_DoT : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_Tank_DoT;

        public uint getDoT() => Invoke(BypassAction);

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FeatherRain_Spell44 or BypassAction))
                return actionID;

            var dotHelper = new DoTs(
                Config.BLU_Tank_DoT_WasteProtection_HP,
                Config.BLU_Tank_DoT_WasteProtection_Time);

            // Waste protection
            if (IsEnabled(Preset.BLU_Tank_DoT_WasteProtection) &&
                HasTarget() && !dotHelper.AnyDotsWanted() &&
                actionID != BypassAction)
                return All.SavageBlade;

            // Use DoT
            if (dotHelper.TryGet(out var dotAction))
                return dotAction;

            return actionID;
        }
    }

    internal class BLU_DebuffCombo : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_DebuffCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Devour_Spell75) return actionID;

            // Offguard
            if (!TargetHasEffectAny(Debuffs.Offguard) &&
                IsOffCooldown(Offguard_Spell20) &&
                IsSpellActive(Offguard_Spell20))
                return Offguard_Spell20;

            // Bad Breath
            if (!TargetHasEffectAny(Debuffs.Malodorous) &&
                HasEffect(Buffs.TankMimicry) && IsSpellActive(BadBreath_Spell28))
                return BadBreath_Spell28;

            // Devour
            if (IsOffCooldown(Devour_Spell75) && HasEffect(Buffs.TankMimicry) &&
                IsSpellActive(Devour_Spell75))
                return Devour_Spell75;

            // Lucid Dreaming
            if (IsOffCooldown(All.LucidDreaming) &&
                LocalPlayer.CurrentMp <= 9000 &&
                LevelChecked(All.LucidDreaming))
                return All.LucidDreaming;

            return actionID;
        }
    }

    internal class BLU_Addle : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_Addle;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not MagicHammer_Spell60) return actionID;

            return (IsOnCooldown(MagicHammer_Spell60) &&
                    IsOffCooldown(All.Addle) &&
                    !TargetHasEffect(All.Debuffs.Addle) &&
                    !TargetHasEffect(Debuffs.Conked))
                ? All.Addle
                : actionID;
        }
    }

    #endregion

    #region Healer Combos

    //

    #endregion

    #region Unsorted Combos

    internal class BLU_KnightCombo : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_KnightCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not
                (WhiteKnightsTour_Spell65 or BlackKnightsTour_Spell66))
                return actionID;

            if (TargetHasEffect(Debuffs.Slow) &&
                IsSpellActive(BlackKnightsTour_Spell66))
                return BlackKnightsTour_Spell66;
            if (TargetHasEffect(Debuffs.Bind) &&
                IsSpellActive(WhiteKnightsTour_Spell65))
                return WhiteKnightsTour_Spell65;

            return actionID;
        }
    }

    internal class BLU_LightHeadedCombo : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_LightHeadedCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not PeripheralSynthesis_Spell101) return actionID;

            if (!TargetHasEffect(Debuffs.Lightheaded) &&
                IsSpellActive(PeripheralSynthesis_Spell101))
                return PeripheralSynthesis_Spell101;
            if (TargetHasEffect(Debuffs.Lightheaded) &&
                IsSpellActive(MustardBomb_Spell94))
                return MustardBomb_Spell94;

            return actionID;
        }
    }

    internal class BLU_PerpetualRayStunCombo : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_PerpetualRayStunCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not PerpetualRay_Spell69) return actionID;

            return ((TargetHasEffectAny(Debuffs.Stun) ||
                     WasLastAction(PerpetualRay_Spell69)) &&
                    IsSpellActive(SharpenedKnife_Spell15) && InMeleeRange())
                ? SharpenedKnife_Spell15
                : actionID;
        }
    }

    internal class BLU_MeleeCombo : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_MeleeCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SonicBoom_Spell63) return actionID;

            return (IsSpellActive(SharpenedKnife_Spell15) &&
                    GetTargetDistance() <= 3)
                ? SharpenedKnife_Spell15
                : actionID;
        }
    }

    internal class BLU_PeatClean : CustomCombo
    {
        protected internal override Preset Preset { get; } =
            Preset.BLU_PeatClean;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DeepClean_Spell112) return actionID;

            if (IsSpellActive(PeatPelt_Spell111) &&
                !TargetHasEffect(Debuffs.Begrimed))
                return PeatPelt_Spell111;

            return actionID;
        }
    }

    #endregion
}
