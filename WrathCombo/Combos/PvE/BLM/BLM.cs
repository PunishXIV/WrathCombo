using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

//TODO Cleanup weaves + reorder everything
internal partial class BLM
{
    internal class BLM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            //Weaves
            if (CanSpellWeave())
            {
                if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (ActionReady(Amplifier) && RemainingPolyglotCD >= 20000)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines))
                    return LeyLines;
            }

            if (HasEffect(Buffs.Thunderhead) && GCDsInTimer > 1 && LevelChecked(Thunder) &&
                GetTargetHPPercent() >= Config.BLM_ST_ThunderHP &&
                (ThunderDebuffST is null || ThunderDebuffST.RemainingTime < 3))
                return OriginalHook(Thunder);

            if (IsMoving())
            {
                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < MaxPolyglot)
                    return Amplifier;

                if (HasPolyglotStacks(Gauge))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (Gauge.InAstralFire)
            {
                if (Gauge.IsParadoxActive && GCDsInTimer < 2 && CurMp >= MP.FireI)
                    return Paradox;

                if (HasEffect(Buffs.Firestarter) &&
                    (GCDsInTimer < 2 && CurMp >= MP.FireI ||
                     Gauge.AstralFireStacks < 3))
                    return Fire3;

                if (CurMp < MP.FireI && LevelChecked(Despair) && CurMp >= MP.Despair)
                    return Despair;

                if (CurMp == 0 && LevelChecked(FlareStar) && Gauge.AstralSoulStacks == 6)
                {
                    if (CanSpellWeave() && ActionReady(Triplecast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        ActionReady(Triplecast))
                        return Triplecast;

                    if (CanSpellWeave() && ActionReady(All.Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0)
                        return All.Swiftcast;

                    return FlareStar;
                }

                if (LevelChecked(Fire4))
                    if (GCDsInTimer > 1 && CurMp >= MP.FireI)
                    {
                        if (CanSpellWeave() && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            ActionReady(Triplecast))
                            return Triplecast;

                        if (HasEffect(Buffs.Thunderhead) && GCDsInTimer > 1 &&
                            (ThunderDebuffST is null || ThunderDebuffST.RemainingTime < 3))
                            return OriginalHook(Thunder);

                        if (HasPolyglotStacks(Gauge) &&
                            CanSpellWeave() && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            ActionReady(Triplecast))
                            return Xenoglossy.LevelChecked()
                                ? Xenoglossy
                                : Foul;

                        return Fire4;
                    }

                if (CurMp >= MP.FireI)
                    return Fire;

                if (ActionReady(Manafont))
                    return HasEffect(Buffs.Firestarter)
                        ? Fire3
                        : Manafont;

                if (ActionReady(Blizzard3) &&
                    (ActionReady(All.Swiftcast) || HasEffect(Buffs.Triplecast)))
                {
                    if (CanSpellWeave() && ActionReady(Transpose))
                        return Transpose;

                    if (HasEffect(Buffs.Thunderhead) &&
                        (ThunderDebuffST is null || ThunderDebuffST.RemainingTime < 3))
                        return OriginalHook(Thunder);

                    if (HasPolyglotStacks(Gauge))
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;
                }

                return LevelChecked(Blizzard3)
                    ? Blizzard3
                    : Transpose;
            }

            if (Gauge.InUmbralIce)
            {
                if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3 && TraitLevelChecked(Traits.UmbralHeart))
                {
                    if (HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Triplecast))
                        return Blizzard3;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && ActionReady(Triplecast))
                        return Triplecast;
                }

                if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Blizzard4;

                if (Gauge.IsParadoxActive)
                    return Paradox;

                if (HasPolyglotStacks(Gauge))
                {
                    if (!HasEffect(Buffs.Firestarter) ||
                        !(GetBuffRemainingTime(Buffs.Firestarter) <= 3))
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;

                    if (CurMp + NextMpGain <= 10000 || CurMp < 7500)
                        return Blizzard;

                    if (ActionReady(Transpose) && CanSpellWeave() &&
                        CurMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                        return Transpose;

                    if (LevelChecked(Fire3))
                        return Fire3;

                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
                }

                if (CurMp + NextMpGain >= 7500 &&
                    (LocalPlayer?.CastActionId == Blizzard ||
                     WasLastSpell(Blizzard) ||
                     WasLastSpell(Blizzard4)))
                    return LevelChecked(Fire3)
                        ? Fire3
                        : Fire;

                if (CurMp + NextMpGain <= 10000 || CurMp < 7500)
                    return Blizzard;

                if (ActionReady(Transpose) && CanSpellWeave() &&
                    CurMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                    return Transpose;

                return LevelChecked(Fire3)
                    ? Fire3
                    : Transpose;
            }

            if (Blizzard3.LevelChecked())
                return Blizzard3;
            return actionID;
        }
    }

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fire)
                return actionID;

            int polyglotStacks = Gauge.PolyglotStacks;
            float triplecastChargetime = GetCooldownChargeRemainingTime(Triplecast);

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (IsEnabled(CustomComboPreset.BLM_ST_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            //Weaves
            if (CanSpellWeave())
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && RemainingPolyglotCD >= 20000)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_LeyLines) &&
                    ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > Config.BLM_ST_LeyLinesCharges)
                    return LeyLines;
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                HasEffect(Buffs.Thunderhead) && GCDsInTimer > 1 && LevelChecked(Thunder) &&
                GetTargetHPPercent() >= Config.BLM_ST_ThunderHP &&
                (ThunderDebuffST is null || ThunderDebuffST.RemainingTime < 3))
                return OriginalHook(Thunder);

            if (IsMoving())
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && Gauge.PolyglotStacks < MaxPolyglot)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglotMoving) &&
                    polyglotStacks > Config.BLM_ST_UsePolyglotMoving_HoldCharges)
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (Gauge.InAstralFire)
            {
                if (Gauge.IsParadoxActive && GCDsInTimer < 2 && CurMp >= MP.FireI)
                    return Paradox;

                if (HasEffect(Buffs.Firestarter) &&
                    (GCDsInTimer < 2 && CurMp >= MP.FireI ||
                     Gauge.AstralFireStacks < 3))
                    return Fire3;

                if(IsEnabled(CustomComboPreset.BLM_ST_����) && HasEffect(Buffs.Firestarter) && !LevelChecked(Fire4)) {
                    return Fire3;
                }

                if (IsEnabled(CustomComboPreset.BLM_ST_Despair) &&
                    CurMp < MP.FireI && LevelChecked(Despair) && CurMp >= MP.Despair)
                    return Despair;

                if (IsEnabled(CustomComboPreset.BLM_ST_FlareStar) &&
                    CurMp == 0 && LevelChecked(FlareStar) && Gauge.AstralSoulStacks == 6)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        CanSpellWeave() && ActionReady(Triplecast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                         triplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                        CanSpellWeave() && ActionReady(All.Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0)
                        return All.Swiftcast;

                    return FlareStar;
                }

                if (LevelChecked(Fire4))
                    if (GCDsInTimer > 1 && CurMp >= MP.FireI)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            CanSpellWeave() && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                             triplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                            return Triplecast;

                        if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                            HasEffect(Buffs.Thunderhead) && GCDsInTimer > 1 &&
                            (ThunderDebuffST is null || ThunderDebuffST.RemainingTime < 3))
                            return OriginalHook(Thunder);

                        if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                            polyglotStacks > Config.BLM_ST_UsePolyglot_HoldCharges &&
                            IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            CanSpellWeave() && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                             triplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                            return Xenoglossy.LevelChecked()
                                ? Xenoglossy
                                : Foul;

                        return Fire4;
                    }

                if (CurMp >= MP.FireI)
                    return Fire;

                if (IsEnabled(CustomComboPreset.BLM_ST_Manafont) &&
                    ActionReady(Manafont))
                    return HasEffect(Buffs.Firestarter)
                        ? Fire3
                        : Manafont;

                if (ActionReady(Blizzard3) &&
                    (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) && ActionReady(All.Swiftcast) ||
                     HasEffect(Buffs.Triplecast)))
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        CanSpellWeave() && ActionReady(Transpose))
                        return Transpose;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                        HasEffect(Buffs.Thunderhead) &&
                        (ThunderDebuffST is null || ThunderDebuffST.RemainingTime < 3))
                        return OriginalHook(Thunder);

                    if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                        polyglotStacks > Config.BLM_ST_UsePolyglot_HoldCharges)
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;
                }

                return LevelChecked(Blizzard3)
                    ? Blizzard3
                    : Transpose;
            }

            if (Gauge.InUmbralIce)
            {
                if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3 && TraitLevelChecked(Traits.UmbralHeart))
                {
                    if (HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Triplecast))
                        return Blizzard3;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        LevelChecked(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                         triplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                        return Triplecast;
                }

                if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Blizzard4;

                if (Gauge.IsParadoxActive)
                    return Paradox;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                    polyglotStacks > Config.BLM_ST_UsePolyglot_HoldCharges)
                {
                    if (!HasEffect(Buffs.Firestarter) ||
                        !(GetBuffRemainingTime(Buffs.Firestarter) <= 3))
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;

                    if (CurMp + NextMpGain <= 10000 || CurMp < 7500)
                        return Blizzard;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) && CanSpellWeave() &&
                        CurMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                        return Transpose;

                    if (LevelChecked(Fire3))
                        return Fire3;

                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
                }

                if (CurMp + NextMpGain >= 7500 &&
                    (LocalPlayer?.CastActionId == Blizzard ||
                     WasLastSpell(Blizzard) ||
                     WasLastSpell(Blizzard4)))
                    return LevelChecked(Fire3)
                        ? Fire3
                        : Fire;

                if (CurMp + NextMpGain <= 10000 || CurMp < 7500)
                    return Blizzard;

                if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                    ActionReady(Transpose) && CanSpellWeave() &&
                    CurMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                    return Transpose;

                return LevelChecked(Fire3)
                    ? Fire3
                    : Transpose;
            }

            if (Blizzard3.LevelChecked())
                return Blizzard3;
            return actionID;
        }
    }

    internal class BLM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (WasLastSpell(UmbralSoul))
                return OriginalHook(Fire2);

            if (HasEffect(Buffs.Thunderhead) && GCDsInTimer > 1 && Thunder2.LevelChecked() &&
                (ThunderDebuffAoE is null || ThunderDebuffAoE.RemainingTime < 3))
                return OriginalHook(Thunder2);

            if (ActionReady(Amplifier) && RemainingPolyglotCD >= 20000 && CanSpellWeave())
                return Amplifier;

            if (IsMoving())
            {
                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < MaxPolyglot)
                    return Amplifier;

                if (HasPolyglotStacks(Gauge))
                    return Foul;
            }

            if (CanSpellWeave() &&
                ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines))
                return LeyLines;

            if (Gauge.InAstralFire)
            {
                if (CurMp == 0 && FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (!FlareStar.LevelChecked() && Fire2.LevelChecked() && CurMp >= MP.FireAoE &&
                    (Gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (Flare.LevelChecked() && CurMp >= MP.AllMPSpells)
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                        CanSpellWeave())
                        return Triplecast;
                    if (Flare.LevelChecked() && CurMp >= MP.FlareAoE)
                    {
                        if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                            CanSpellWeave())
                            return Triplecast;

                        return Flare;
                    }

                    if (Fire2.LevelChecked())
                        if (GCDsInTimer > 1 && CurMp >= MP.FireAoE)
                            return OriginalHook(Fire2);

                    if (ActionReady(Manafont))
                        return Manafont;

                    if (ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || CanSwiftF))
                        return Transpose;

                    if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                        return OriginalHook(Blizzard2);
                }
            }

            if (Gauge.InUmbralIce)
            {
                if (HasPolyglotStacks(Gauge))
                    return Foul;

                if (ActionWatching.WhichOfTheseActionsWasLast(OriginalHook(Fire2), OriginalHook(Freeze),
                        OriginalHook(Flare), OriginalHook(FlareStar)) == OriginalHook(Freeze) &&
                    FlareStar.LevelChecked())
                {
                    if (ActionReady(Transpose) && CanSpellWeave())
                        return Transpose;

                    return OriginalHook(Fire2);
                }

                if (ActionReady(OriginalHook(Blizzard2)) && Gauge.UmbralIceStacks < 3 &&
                    TraitLevelChecked(Traits.AspectMasteryIII))
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                        CanSpellWeave())
                        return Triplecast;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast) &&
                        CanSpellWeave())
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return OriginalHook(Blizzard2);
                }

                if (Gauge.UmbralIceStacks < 3 && ActionReady(OriginalHook(Blizzard2)))
                    return OriginalHook(Blizzard2);

                if (Freeze.LevelChecked() && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Freeze;

                if (DoubleBlizz() && Fire2.LevelChecked())
                    return OriginalHook(Fire2);

                if (CurMp < LocalPlayer?.MaxMp)
                    return Freeze.LevelChecked()
                        ? OriginalHook(Freeze)
                        : OriginalHook(Blizzard2);

                if (ActionReady(Transpose) && CanSpellWeave() &&
                    (Flare.LevelChecked() || !TraitLevelChecked(Traits.AspectMasteryIII)))
                    return Transpose;

                if (Fire2.LevelChecked() && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Fire2);
            }

            if (Blizzard2.LevelChecked())
                return OriginalHook(Blizzard2);

            return actionID;
        }
    }

    internal class BLM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            int polyglotStacks = Gauge.PolyglotStacks;
            float triplecastChargetime = GetCooldownChargeRemainingTime(Triplecast);

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            if (WasLastSpell(UmbralSoul))
                return OriginalHook(Fire2);

            if (IsEnabled(CustomComboPreset.BLM_AoE_Thunder) &&
                HasEffect(Buffs.Thunderhead) && GCDsInTimer > 1 && LevelChecked(Thunder2) &&
                GetTargetHPPercent() >= Config.BLM_AoE_ThunderHP &&
                (ThunderDebuffAoE is null || ThunderDebuffAoE.RemainingTime < 3))
                return OriginalHook(Thunder2);

            if (IsEnabled(CustomComboPreset.BLM_AoE_Amplifier) &&
                ActionReady(Amplifier) && RemainingPolyglotCD >= 20000 && CanSpellWeave())
                return Amplifier;

            if (IsMoving())
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_Amplifier) &&
                    ActionReady(Amplifier) && Gauge.PolyglotStacks < MaxPolyglot)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_AoE_UsePolyglotMoving) &&
                    polyglotStacks > Config.BLM_AoE_UsePolyglotMoving_HoldCharges)
                    return Foul;
            }

            if (IsEnabled(CustomComboPreset.BLM_AoE_LeyLines) &&
                CanSpellWeave() &&
                ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines) &&
                GetRemainingCharges(LeyLines) > Config.BLM_AoE_LeyLinesCharges)
                return LeyLines;

            if (Gauge.InAstralFire)
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_FlareStar) &&
                    CurMp == 0 && FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (!FlareStar.LevelChecked() && Fire2.LevelChecked() && CurMp >= MP.FireAoE &&
                    (Gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Flare) &&
                    Flare.LevelChecked() && CurMp >= MP.FlareAoE)
                {
                    if (LevelChecked(Triplecast) && CanSpellWeave() &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_AoE_Triplecast_HoldCharges ||
                         triplecastChargetime <= Config.BLM_AoE_Triplecast_ChargeTime))
                        return Triplecast;

                    return Flare;
                }

                if (Fire2.LevelChecked())
                    if (GCDsInTimer > 1 && CurMp >= MP.FireAoE)
                        return OriginalHook(Fire2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Manafont) &&
                    ActionReady(Manafont))
                    return Manafont;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || CanSwiftF))
                    return Transpose;

                if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Blizzard2);
            }

            if (Gauge.InUmbralIce)
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_UsePolyglot) &&
                    polyglotStacks > Config.BLM_AoE_UsePolyglot_HoldCharges)
                    return Foul;

                if (ActionWatching.WhichOfTheseActionsWasLast(OriginalHook(Fire2), OriginalHook(Freeze),
                        OriginalHook(Flare), OriginalHook(FlareStar)) == OriginalHook(Freeze) &&
                    FlareStar.LevelChecked())
                {
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        ActionReady(Transpose) && CanSpellWeave())
                        return Transpose;

                    return OriginalHook(Fire2);
                }

                if (ActionReady(OriginalHook(Blizzard2)) && Gauge.UmbralIceStacks < 3 &&
                    TraitLevelChecked(Traits.AspectMasteryIII))
                {
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Triplecast) &&
                        LevelChecked(Triplecast) && CanSpellWeave() &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_AoE_Triplecast_HoldCharges ||
                         triplecastChargetime <= Config.BLM_AoE_Triplecast_ChargeTime))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_AoE_Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast) &&
                        CanSpellWeave())
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return OriginalHook(Blizzard2);
                }

                if (Gauge.UmbralIceStacks < 3 && ActionReady(OriginalHook(Blizzard2)))
                    return OriginalHook(Blizzard2);

                if (Freeze.LevelChecked() && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Freeze;

                if (DoubleBlizz() && Fire2.LevelChecked())
                    return OriginalHook(Fire2);

                if (CurMp < LocalPlayer?.MaxMp)
                    return Freeze.LevelChecked()
                        ? OriginalHook(Freeze)
                        : OriginalHook(Blizzard2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && CanSpellWeave() &&
                    (Flare.LevelChecked() || !TraitLevelChecked(Traits.AspectMasteryIII)))
                    return Transpose;

                if (Fire2.LevelChecked() && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Fire2);
            }

            if (Blizzard2.LevelChecked())
                return OriginalHook(Blizzard2);
            return actionID;
        }
    }

    internal class BLM_Variant_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Variant_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID is All.Swiftcast && HasEffect(All.Buffs.Swiftcast) && IsEnabled(Variant.VariantRaise)
                ? Variant.VariantRaise
                : actionID;
    }

    internal class BLM_Scathe_Xeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Scathe_Xeno;

        protected override uint Invoke(uint actionID) =>
            actionID is Scathe && LevelChecked(Xenoglossy) && HasPolyglotStacks(Gauge)
                ? Xenoglossy
                : actionID;
    }

    internal class BLM_Blizzard_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_1to3;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Blizzard when LevelChecked(Freeze) && !Gauge.InUmbralIce:
                    return Blizzard3;

                case Freeze when !LevelChecked(Freeze):
                    return Blizzard2;

                default:
                    return actionID;
            }
        }
    }

    internal class BLM_Fire_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

        protected override uint Invoke(uint actionID) =>
            actionID is Fire &&
            (LevelChecked(Fire3) && !Gauge.InAstralFire ||
             HasEffect(Buffs.Firestarter))
                ? Fire3
                : actionID;
    }

    internal class BLM_Between_The_LeyLines : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Between_The_LeyLines;

        protected override uint Invoke(uint actionID) =>
            actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_Aetherial_Manipulation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID) =>
            actionID is AetherialManipulation && ActionReady(BetweenTheLines) &&
            HasEffect(Buffs.LeyLines) && !HasEffect(Buffs.CircleOfPower) && !IsMoving()
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_UmbralSoul : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_UmbralSoul;

        protected override uint Invoke(uint actionID) =>
            actionID is Transpose && Gauge.InUmbralIce && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
    }

    internal class BLM_TriplecastProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_TriplecastProtection;

        protected override uint Invoke(uint actionID) =>
            actionID is Triplecast && HasEffect(Buffs.Triplecast) && LevelChecked(Triplecast)
                ? All.SavageBlade
                : actionID;
    }

    internal class BLM_FireandIce : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_FireandIce;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case Fire4 when Gauge.InAstralFire && LevelChecked(Fire4):
                    return Fire4;

                case Fire4 when Gauge.InUmbralIce && LevelChecked(Blizzard4):
                    return Blizzard4;

                default:
                    return actionID;
            }
        }
    }

    internal class BLM_ST_AnimeMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_AnimeMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != Fire) {
                return actionID;
            }

            var maxPolyglot = TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 : TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;
            var maxPolyglotCD = maxPolyglot * 30000;
            var remainingPolyglotCD = Math.Max(0, ((maxPolyglot - Gauge.PolyglotStacks) * 30000) + (Gauge.EnochianTimer - 30000));
            var curMp = LocalPlayer.CurrentMp;

            var thunderDebuff = FindEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget, LocalPlayer.GameObjectId);
            var ����ʣ������ = Gauge.ElementTimeRemaining / 1000f;
            var �����ڻ��ܴ򼸸��� = Math.Floor(����ʣ������ / GetActionCastTime(Fire));
            var �����ڻ��ܴ򼸸��� = Math.Floor(����ʣ������ / GetActionCastTime(Fire3));
            var �������� = Gauge.UmbralHearts;

            bool ������()
            {
                if (Gauge.InAstralFire) {
                    return �����ڻ��ܴ򼸸��� < 2;
                }
                else if (Gauge.InUmbralIce) {
                    if (LevelChecked(Blizzard4)) {
                        //ʣ��ʱ���㹻��һ����һ��
                        float TimeNeed = GetActionCastTime(Blizzard4) + GetActionCastTime(Fire3);
                        return TimeNeed >= ����ʣ������;
                    }
                    else {
                        return false;
                    }

                }
                return false;
            }
            bool ��ת��()
            {
                if (Gauge.AstralSoulStacks == 3 && JustUsed(Blizzard4, 5f)) {
                    return true;
                }
                if (curMp == MP.MaxMP) {
                    //�б��� ��Ҫ�ñ���
                    if (LevelChecked(Blizzard4)) {
                        return �������� >= 3;
                    }
                    //û���� ��������
                    else {
                        return true;
                    }
                }
                return false;
            }

            #region ����Թ�
            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure) {
                return Variant.VariantCure;
            }

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave()) {
                return Variant.VariantRampart;
            }
            #endregion

            if (!HasBattleTarget()) {
                if (Gauge.InUmbralIce) {
                    if (LevelChecked(UmbralSoul) && ����ʣ������ <= 11) {
                        return UmbralSoul;
                    }
                    else {
                        if (����ʣ������ <= 7) {
                            return Transpose;
                        }
                    }
                }
                if (Gauge.InAstralFire && ����ʣ������ <= 7) {
                    return Transpose;
                }
            }

            //��������
            if (HasEffect(Buffs.Thunderhead) && !������()) {
                if (thunderDebuff is null || thunderDebuff.RemainingTime <= 3) {
                    return OriginalHook(Thunder);
                }
            }

            if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && CanSpellWeave() && (Gauge.InAstralFire || Gauge.InUmbralIce)) {
                return Amplifier;
            }

            if (remainingPolyglotCD < 6000 && �����ڻ��ܴ򼸸��� > 2 && Gauge.PolyglotStacks > 0) {
                return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;
            }

            if (IsMoving() && InActionRange(Fire3) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Triplecast)) {
                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < maxPolyglot && (Gauge.InAstralFire || Gauge.InUmbralIce)) {
                    return Amplifier;
                }

                if (Gauge.PolyglotStacks > 0) {
                    return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;
                }

                //�ڱ�����Դ�����һ��
                if (Gauge.IsParadoxActive && Gauge.InUmbralIce) {
                    return Paradox;
                }

                //�м��̲弴��
                if (ActionReady(All.Swiftcast) && !HasEffect(All.Buffs.Swiftcast)) {
                    return All.Swiftcast;
                }

                //������������
                if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast)) {
                    return Triplecast;
                }

                //ɶ��û���ˣ����㵱������ʦ��
                return Scathe;
            }

            if (CanSpellWeave() && ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines)) {
                return LeyLines;
            }

            //��׼ѭ�� - ���� - û����
            if (!Gauge.InAstralFire && !Gauge.InUmbralIce) {

                if (LevelChecked(Fire3)) {
                    if (curMp >= MP.FireIII) {
                        return Fire3;
                    }
                    if (curMp >= MP.BlizzardI) {
                        return Blizzard3;
                    }
                    else {
                        if (ActionReady(All.LucidDreaming)) {
                            return All.LucidDreaming;
                        }
                    }
                }
                else {
                    if (curMp >= MP.FireI) {
                        return Fire;
                    }
                    if (curMp >= MP.BlizzardI) {
                        return Blizzard;
                    }
                    else {
                        if (ActionReady(All.LucidDreaming)) {
                            return All.LucidDreaming;
                        }
                    }
                }
            }

            //�Ǽ���
            if (Gauge.InAstralFire) {

                //���������
                if (LevelChecked(Fire3) && HasEffect(Buffs.Firestarter) && Gauge.AstralFireStacks < 3) {
                    return Fire3;
                }

                //���������������
                //��Ҫ������
                if (������()) {
                    //������û���ˣ�ֱ�ӣ�������
                    if (Despair.LevelChecked() && curMp >= 800 && curMp < 2400) {
                        return Despair;
                    }
                    //����
                    if (Gauge.IsParadoxActive && curMp >= MP.FireI) {
                        return Paradox;
                    }
                    //�ô����� - ���Ƽ� - Ҫ�������� - ���Ƕ�����Ӱ�����
                    if (HasEffect(Buffs.Firestarter)) {
                        return Fire3;
                    }
                    //��˵ģ�����
                    if (curMp >= MP.FireI) {
                        return Fire;
                    }
                }
                //����Ҫ������
                else {
                    //û���ˣ�ֱ��
                    if (Despair.LevelChecked() && curMp >= 800 && curMp < 2400) {
                        return Despair;
                    }
                    //���ȶ��ˣ��������
                    if (FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6) {
                        return FlareStar;
                    }
                    //��������
                    if (Fire4.LevelChecked() && curMp >= MP.FireI) {
                        return Fire4;
                    }
                    //û���оͶ���
                    if (!LevelChecked(Fire4) && HasEffect(Buffs.Firestarter)) {
                        return Fire3;
                    }
                    //ɶ��û�У��͵�����
                    if (curMp >= MP.FireI) {
                        return Fire;
                    }
                }

                //û����������������1/��4���Ѿ������濼���ˣ�
                if (curMp < MP.FireI) {
                    //�ж�
                    if (FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6) {
                        return FlareStar;
                    }
                    //�ж�
                    if (ActionReady(Manafont)) {
                        return Manafont;
                    }
                    //ɶ��û�ˣ��������ж�
                    if (ActionReady(Blizzard3)) {
                        return Blizzard3;
                    }
                    //��û�У���λ����
                    if (ActionReady(Transpose)) {
                        return Transpose;
                    }
                }
            }

            //�鼫��
            if (Gauge.InUmbralIce) {
                //�������ˣ���Ȼ�������ӻ�3����Ͷ���
                if (������()) {
                    //�������ģ�����Ҳ�����ˣ�û�б�Ҫ����ֱ�ӽ���
                    if (��ת��()) {
                        if (HasEffect(Buffs.Firestarter)) {
                            //ֱ����λ ��Ϊ�������3��
                            return Transpose;
                        }
                        else {
                            //û ��Ҫ������
                            if (LevelChecked(Fire3)) {
                                return Fire3;
                            }
                            //�ȼ���������λ����
                            else {
                                return Transpose;
                            }
                        }
                    }
                    //����
                    if (Gauge.IsParadoxActive) {
                        return Paradox;
                    }
                    //����
                    if (LevelChecked(UmbralSoul)) {
                        return UmbralSoul;
                    }
                    //��� ����
                    return Blizzard;
                }
                else {
                    //����Ҫ�������
                    if (LevelChecked(Blizzard4) && �������� < 3) {
                        return Blizzard4;
                    }
                    //����Ҳ���ˣ���������
                    if (Gauge.IsParadoxActive) {
                        return Paradox;
                    }
                    //����������
                    if (Gauge.PolyglotStacks > 0) {
                        return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;
                    }
                    //ת��
                    if (��ת��()) {
                        if (HasEffect(Buffs.Firestarter)) {
                            //ֱ����λ ��Ϊ�������3��
                            return Transpose;
                        }
                        else {
                            //û ��Ҫ������
                            if (LevelChecked(Fire3)) {
                                return Fire3;
                            }
                            //�ȼ���������λ����
                            else {
                                return Transpose;
                            }
                        }
                    }
                    //��û�� ����������
                    else {
                        if (LevelChecked(Blizzard4)) {
                            return Blizzard4;
                        }
                        else {
                            return Blizzard;
                        }
                    }
                }
            }

            return actionID;
        }
    }

    internal class BLM_AoE_AnimeMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_AnimeMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2)) {
                return actionID;
            }

            
            var ����ʣ������ = Gauge.ElementTimeRemaining / 1000f;
            var maxPolyglot = TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 : TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;
            var maxPolyglotCD = maxPolyglot * 30000;
            var remainingPolyglotCD = Math.Max(0, ((maxPolyglot - Gauge.PolyglotStacks) * 30000) + (Gauge.EnochianTimer - 30000));
            var curMp = LocalPlayer.CurrentMp;
            int nextMpGain = Gauge.UmbralIceStacks switch
            {
                0 => 0,
                1 => 2500,
                2 => 5000,
                3 => 10000,
                _ => 0
            };
            var thunderDebuff = FindEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget, LocalPlayer.GameObjectId);
            var elementTimer = Gauge.ElementTimeRemaining / 1000f;
            var gcdsInTimer = Math.Floor(elementTimer / GetActionCastTime(ActionWatching.LastSpell));
            var canSwiftF = TraitLevelChecked(Traits.AspectMasteryIII) && (IsOffCooldown(All.Swiftcast) || ActionReady(Triplecast) || GetBuffStacks(Buffs.Triplecast) > 0);
            var canWeave = CanSpellWeave();
            var �������� = Gauge.UmbralHearts;

            bool ����()
            {
                if (Gauge.InAstralFire && LevelChecked(FlareStar) && Gauge.AstralSoulStacks == 6) {
                    return true;
                }

                return false;
            }

            bool ������()
            {
                if (Gauge.InAstralFire) {
                    if (����()) {
                        return elementTimer < (GetActionCastTime(FlareStar) + GetActionCastTime(Blizzard2));
                    }
                    else {
                        return false;
                    }
                }
                else if (Gauge.InUmbralIce) {
                    return elementTimer <= 3;
                }
                return false;
            }

            bool ��ת��()
            {
                if (LevelChecked(Blizzard4)) {
                    if (�������� >= 1) {
                        return curMp >= 2400;
                    }
                    return false;
                }
                else {
                    return curMp == MP.MaxMP;
                }
            }

            bool ��Ⱥ����()
            {
                //��Ⱥ����
                if (HasEffect(Buffs.Thunderhead) && !������()) {
                    if (thunderDebuff is null || thunderDebuff.RemainingTime <= 3) {
                        return true;
                    }
                }
                return false;
            }


            //����Թ�
            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure) {
                return Variant.VariantCure;
            }

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave) {
                return Variant.VariantRampart;
            }

            //ûĿ��Ĵ���
            if (!HasBattleTarget()) {
                if (Gauge.InUmbralIce) {
                    if (LevelChecked(UmbralSoul) && ����ʣ������ <= 11) {
                        return UmbralSoul;
                    }
                    else {
                        if (����ʣ������ <= 7) {
                            return Transpose;
                        }
                    }
                }
                if (Gauge.InAstralFire && ����ʣ������ <= 7) {
                    return Transpose;
                }
            }

            if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && canWeave && (Gauge.InAstralFire || Gauge.InUmbralIce)) {
                return Amplifier;
            }

            //�ƶ�����
            if (IsMoving() && InActionRange(Fire3) && !HasEffect(All.Buffs.Swiftcast) && !HasEffect(Buffs.Triplecast)) {
                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < maxPolyglot && (Gauge.InAstralFire || Gauge.InUmbralIce)) {
                    return Amplifier;
                }

                if (Gauge.PolyglotStacks > 0) {
                    return Foul;
                }

                //�ڱ�����Դ��������һ��
                if (Gauge.IsParadoxActive && Gauge.InUmbralIce) {
                    return Paradox;
                }

                //�м��̲弴��
                if (ActionReady(All.Swiftcast) && !HasEffect(All.Buffs.Swiftcast)) {
                    return All.Swiftcast;
                }

                //������������
                if (ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast)) {
                    return Triplecast;
                }

                //ɶ��û���ˣ����㵱������ʦ��
                return Scathe;
            }

            //
            if (canWeave && ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines)) {
                return LeyLines;
            }

            //�������
            if (Gauge.InAstralFire) {
                //�Ǽ���
                if (!������()) {
                    //���ȴ�
                    if (FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6) {
                        return FlareStar;
                    }
                    if (Gauge.UmbralHearts > 0) {
                        //�б��� ������                            
                        if (Flare.LevelChecked() && curMp >= MP.FlareAoE) {
                            /*��������/���� 
                             * ȡ�����ܴ򼸸�
                            //if (�������� == 3) {
                            //    //������
                            //    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 && canWeave)
                            //        return Triplecast;
                            //}
                            //else if (�������� == 0) {
                            //    //�弴��
                            //    if (ActionReady(All.Swiftcast) && !HasEffect(All.Buffs.Swiftcast) && GetBuffStacks(Buffs.Triplecast) == 0) {
                            //        return All.Swiftcast;
                            //    }
                            //}
                            */
                            return Flare;
                        }
                    }
                    else {
                        //����
                        if (Fire2.LevelChecked() && curMp >= (MP.FireAoE + MP.FlareAoE)) {
                            return OriginalHook(Fire2);
                        }
                        else {
                            //�����ˣ�ֱ��
                            if (Flare.LevelChecked() && curMp >= MP.FlareAoE) {
                                return Flare;
                            }
                        }
                    }

                    //û���˴�
                    if (ActionReady(Manafont)) {
                        return Manafont;
                    }
                    //Ҳû�ˣ�����
                    if (ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || canSwiftF)) {
                        if (��Ⱥ����()) {
                            return OriginalHook(Thunder2);
                        }
                        return Transpose;
                    }


                    if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII)) {
                        return OriginalHook(Blizzard2);
                    }
                }
                else {
                    //��Ҫ��������ܴ�
                    //û��
                    if (Flare.LevelChecked() && curMp >= MP.FlareAoE) {
                        //��������/���� ȡ�����ܴ򼸸�
                        if (�������� == 3) {
                            //������
                            if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 && canWeave) {
                                return Triplecast;
                            }
                        }
                        else if (�������� == 0) {
                            //�弴��
                            if (ActionReady(All.Swiftcast) && !HasEffect(All.Buffs.Swiftcast) && GetBuffStacks(Buffs.Triplecast) == 0) {
                                return All.Swiftcast;
                            }
                        }
                        return Flare;
                    }
                    //ɶ��û�д�
                    if (Fire2.LevelChecked() && curMp >= MP.FireAoE) {
                        return OriginalHook(Fire2);
                    }
                    //û���˴�
                    if (ActionReady(Manafont)) {
                        return Manafont;
                    }
                    //Ҳû�ˣ�����
                    if (ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || canSwiftF)) {
                        return Transpose;
                    }

                    if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII)) {
                        return OriginalHook(Blizzard2);
                    }
                }
            }
            if (Gauge.InUmbralIce) {
                //�鼫��

                if (!������()) {
                    //�д�����
                    if (LevelChecked(Blizzard4) && �������� < 3) {
                        return Freeze;
                    }
                    //�л��Ǵ����
                    if (Gauge.PolyglotStacks > 0) {
                        return Foul;
                    }
                    //�����˽���
                    if (��ת��()) {
                        if (��Ⱥ����()) {
                            return OriginalHook(Thunder2);
                        }
                        if (ActionReady(Transpose)) {
                            return Transpose;
                        }

                        if (ActionReady(OriginalHook(Fire2))) {
                            return OriginalHook(Fire2);
                        }
                    }
                    //��û����
                    else {
                        if (LevelChecked(Freeze)) {
                            return Freeze;
                        }
                        else {
                            return Blizzard2;
                        }
                    }
                }
                else {
                    //������ - ���
                    if (Gauge.IsParadoxActive && Gauge.InUmbralIce) {
                        return Paradox;
                    }
                    //���鼫����
                    if (LevelChecked(UmbralSoul)) {
                        return UmbralSoul;
                    }
                    //��� �ñ�����
                    return Blizzard2;
                }
            }

            //�����˵����
            if (Gauge.InUmbralIce && LevelChecked(Freeze)) {
                return Freeze;
            }

            //�����֣�����˷���
            if (curMp == MP.MaxMP && LevelChecked(Fire2)) {
                return OriginalHook(Fire2);
            }

            if (Blizzard2.LevelChecked()) {
                return OriginalHook(Blizzard2);
            }

            return actionID;
        }
    }
}
