using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal static partial class AST
{  
    internal class AST_Benefic : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Benefic;

        protected override uint Invoke(uint actionID)
            => actionID is Benefic2 && !ActionReady(Benefic2) ? Benefic : actionID;
    }

    internal class AST_Raise_Alternative : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Raise_Alternative;

        protected override uint Invoke(uint actionID)
            => actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast) ? Ascend : actionID;
    }

    internal class AST_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_DPS;
        internal static int MaleficCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(Malefic));
        internal static int CombustCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(Combust));

        protected override uint Invoke(uint actionID)
        {
            bool AlternateMode = GetIntOptionAsBool(Config.AST_DPS_AltMode); //(0 or 1 radio values)
            bool actionFound = (!AlternateMode && MaleficList.Contains(actionID)) ||
                (AlternateMode && CombustList.ContainsKey(actionID));

            if (!actionFound)
                return actionID;

            // Out of combat Card Draw
            if (!InCombat())
            {
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) && (Gauge.DrawnCards.All(x => x is CardType.NONE) || (DrawnCard == CardType.NONE && Config.AST_ST_DPS_OverwriteCards)))
                    return OriginalHook(AstralDraw);
            }

            if (IsEnabled(CustomComboPreset.AST_ST_DPS_Opener) && Opener().FullOpener(ref actionID))
                return actionID;

            //In combat
            if (InCombat())
            {
                //Variant stuff
                if (IsEnabled(CustomComboPreset.AST_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart) &&
                    CanSpellWeave())
                    return Variant.VariantRampart;

                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                    IsEnabled(Variant.VariantSpiritDart) &&
                    (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                    CanSpellWeave())
                    return Variant.VariantSpiritDart;

                if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                    ActionReady(Lightspeed) &&
                    GetTargetHPPercent() > Config.AST_DPS_LightSpeedOption &&
                    IsMoving() &&
                    !HasEffect(Buffs.Lightspeed))
                    return Lightspeed;


                if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                    ActionReady(All.LucidDreaming) &&
                    LocalPlayer.CurrentMp <= Config.AST_LucidDreaming &&
                    CanSpellWeave())
                    return All.LucidDreaming;


                //Play Card
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                    ActionReady(Play1) &&
                    Gauge.DrawnCards[0] is not CardType.NONE &&
                    CanSpellWeave())
                    return OriginalHook(Play1);

                //Card Draw
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) &&
                    (Gauge.DrawnCards.All(x => x is CardType.NONE) || (DrawnCard == CardType.NONE && Config.AST_ST_DPS_OverwriteCards)) &&
                    CanDelayedWeave())
                    return OriginalHook(AstralDraw);

                //Divination
                if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                    ActionReady(Divination) &&
                    !HasEffectAny(Buffs.Divination) && //Overwrite protection
                    GetTargetHPPercent() > Config.AST_DPS_DivinationOption &&
                    CanDelayedWeave() &&
                    ActionWatching.NumberOfGcdsUsed >= 3)
                    return Divination;

                //Earthly Star
                if (IsEnabled(CustomComboPreset.AST_ST_DPS_EarthlyStar) &&
                    ActionReady(EarthlyStar) &&
                    CanSpellWeave())
                    return EarthlyStar;

                if (IsEnabled(CustomComboPreset.AST_DPS_Oracle) &&
                    HasEffect(Buffs.Divining) &&
                    CanSpellWeave())
                    return Oracle;

                //Minor Arcana / Lord of Crowns
                if (ActionReady(OriginalHook(MinorArcana)) &&
                    IsEnabled(CustomComboPreset.AST_DPS_LazyLord) && Gauge.DrawnCrownCard is CardType.LORD &&
                    HasBattleTarget() &&
                    CanDelayedWeave())
                    return OriginalHook(MinorArcana);                                       

                if (HasBattleTarget())
                {
                    //Combust
                    if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                        !GravityList.Contains(actionID) &&
                        LevelChecked(Combust) &&
                        CombustList.TryGetValue(OriginalHook(Combust), out ushort dotDebuffID))
                    {
                        if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3 &&
                            CanSpellWeave())
                            return Variant.VariantSpiritDart;
                        
                        float refreshTimer = Config.AST_ST_DPS_CombustUptime_Adv ? Config.AST_ST_DPS_CombustUptime_Threshold : 3;
                        if (GetDebuffRemainingTime(dotDebuffID) <= refreshTimer &&
                            GetTargetHPPercent() > Config.AST_DPS_CombustOption)
                            return OriginalHook(Combust);

                        //Alternate Mode (idles as Malefic)
                        if (AlternateMode) return OriginalHook(Malefic);
                    }
                }
            }
            return actionID;
        }
    }
    internal class AST_AOE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_AOE_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (!GravityList.Contains(actionID))
                return actionID;

            //Variant stuff
            if (IsEnabled(CustomComboPreset.AST_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
            if (IsEnabled(CustomComboPreset.AST_Variant_SpiritDart) &&
                IsEnabled(Variant.VariantSpiritDart) &&
                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3) &&
                CanSpellWeave() &&
                IsEnabled(CustomComboPreset.AST_AOE_DPS) && GravityList.Contains(actionID))
                return Variant.VariantSpiritDart;

            if (IsEnabled(CustomComboPreset.AST_AOE_LightSpeed) &&
                ActionReady(Lightspeed) &&
                GetTargetHPPercent() > Config.AST_AOE_LightSpeedOption &&
                IsMoving() &&
                !HasEffect(Buffs.Lightspeed))
                return Lightspeed;

            if (IsEnabled(CustomComboPreset.AST_AOE_Lucid) &&
                ActionReady(All.LucidDreaming) &&
                LocalPlayer.CurrentMp <= Config.AST_LucidDreaming &&
                CanSpellWeave())
                return All.LucidDreaming;

            //Play Card
            if (IsEnabled(CustomComboPreset.AST_AOE_AutoPlay) &&
                ActionReady(Play1) &&
                Gauge.DrawnCards[0] is not CardType.NONE &&
                CanSpellWeave())
                return OriginalHook(Play1);

            //Card Draw
            if (IsEnabled(CustomComboPreset.AST_AOE_AutoDraw) &&
                ActionReady(OriginalHook(AstralDraw)) &&
                (Gauge.DrawnCards.All(x => x is CardType.NONE) || (DrawnCard == CardType.NONE && Config.AST_AOE_DPS_OverwriteCards)) &&
                CanDelayedWeave())
                return OriginalHook(AstralDraw);

            //Divination
            if (IsEnabled(CustomComboPreset.AST_AOE_Divination) &&
                ActionReady(Divination) &&
                !HasEffectAny(Buffs.Divination) && //Overwrite protection
                GetTargetHPPercent() > Config.AST_AOE_DivinationOption &&
                CanDelayedWeave() &&
                ActionWatching.NumberOfGcdsUsed >= 3)
                return Divination;
            //Earthly Star
            if (IsEnabled(CustomComboPreset.AST_AOE_DPS_EarthlyStar) && !IsMoving() &&
                ActionReady(EarthlyStar) &&
                CanSpellWeave())
                return EarthlyStar;

            if (IsEnabled(CustomComboPreset.AST_AOE_Oracle) &&
                HasEffect(Buffs.Divining) &&
                CanSpellWeave())
                return Oracle;

            //Minor Arcana / Lord of Crowns
            if (ActionReady(OriginalHook(MinorArcana)) &&
                IsEnabled(CustomComboPreset.AST_AOE_LazyLord) && Gauge.DrawnCrownCard is CardType.LORD &&
                HasBattleTarget() &&
                CanDelayedWeave())
                return OriginalHook(MinorArcana);
            return actionID;
        }
    }
    internal class AST_AoE_SimpleHeals_AspectedHelios : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_AoE_SimpleHeals_AspectedHelios;

        protected override uint Invoke(uint actionID)
        {
            bool NonaspectedMode = GetIntOptionAsBool(Config.AST_AoEHeals_AltMode); //(0 or 1 radio values)

            if ((!NonaspectedMode || actionID is not Helios) &&
                (NonaspectedMode || actionID is not (AspectedHelios or HeliosConjuction)))
                return actionID;

            var canLady = (Config.AST_AoE_SimpleHeals_WeaveLady && CanSpellWeave()) || !Config.AST_AoE_SimpleHeals_WeaveLady;
            var canHoroscope = (Config.AST_AoE_SimpleHeals_Horoscope && CanSpellWeave()) || !Config.AST_AoE_SimpleHeals_Horoscope;
            var canOppose = (Config.AST_AoE_SimpleHeals_Opposition && CanSpellWeave()) || !Config.AST_AoE_SimpleHeals_Opposition;

            if (!LevelChecked(AspectedHelios)) //Level check to return helios immediately below 40
                return Helios;

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_LazyLady) &&
                ActionReady(MinorArcana) &&
                Gauge.DrawnCrownCard is CardType.LADY
                && canLady)
                return OriginalHook(MinorArcana);

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                ActionReady(CelestialOpposition) &&
                canOppose)
                return CelestialOpposition;

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
            {
                if (ActionReady(Horoscope) &&
                    canHoroscope)
                    return Horoscope;

                if (HasEffect(Buffs.HoroscopeHelios) &&
                    canHoroscope)
                    return OriginalHook(Horoscope);
            }

            // Only check for our own HoTs
            var hotCheck = HeliosConjuction.LevelChecked() ? FindEffect(Buffs.HeliosConjunction, LocalPlayer, LocalPlayer?.GameObjectId) : FindEffect(Buffs.AspectedHelios, LocalPlayer, LocalPlayer?.GameObjectId);

            if ((IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Aspected) && NonaspectedMode) || // Helios mode: option must be on
                !NonaspectedMode) // Aspected mode: option is not required
            {
                if ((ActionReady(AspectedHelios)
                     && hotCheck is null)
                    || (HasEffect(Buffs.NeutralSect) && !HasEffect(Buffs.NeutralSectShield)))
                    return OriginalHook(AspectedHelios);
            }

            if (hotCheck is not null && hotCheck.RemainingTime > GetActionCastTime(OriginalHook(AspectedHelios)) + 1f)
                return Helios;

            return actionID;
        }
    }


    internal class AST_ST_SimpleHeals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_SimpleHeals;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Benefic2)
                return actionID;

            var canDignity = (Config.AST_ST_SimpleHeals_WeaveDignity && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveDignity;
            var canIntersect = (Config.AST_ST_SimpleHeals_WeaveIntersection && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveIntersection;
            var canExalt = (Config.AST_ST_SimpleHeals_WeaveExalt && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveExalt;
            var canEwer = (Config.AST_ST_SimpleHeals_WeaveEwer && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveEwer;
            var canSpire = (Config.AST_ST_SimpleHeals_WeaveSpire && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveSpire;
            var canBole = (Config.AST_ST_SimpleHeals_WeaveBole && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveBole;
            var canArrow = (Config.AST_ST_SimpleHeals_WeaveArrow && CanSpellWeave()) || !Config.AST_ST_SimpleHeals_WeaveArrow;

            //Grab our target (Soft->Hard->Self)
            IGameObject? healTarget = this.OptionalTarget ?? GetHealTarget(Config.AST_ST_SimpleHeals_Adv && Config.AST_ST_SimpleHeals_UIMouseOver);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Esuna) && ActionReady(All.Esuna) &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) >= Config.AST_ST_SimpleHeals_Esuna &&
                HasCleansableDebuff(healTarget))
                return All.Esuna;

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Spire) &&
                Gauge.DrawnCards[2] == CardType.SPIRE &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Spire &&
                ActionReady(Play3) &&
                canSpire)
                return OriginalHook(Play3);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Ewer) &&
                Gauge.DrawnCards[2] == CardType.EWER &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Ewer &&
                ActionReady(Play3) &&
                canEwer)
                return OriginalHook(Play3);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Arrow) &&
                Gauge.DrawnCards[1] == CardType.ARROW &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Arrow &&
                ActionReady(Play2) &&
                canArrow)
                return OriginalHook(Play2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Bole) &&
                Gauge.DrawnCards[1] == CardType.BOLE &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Bole &&
                ActionReady(Play2) &&
                canBole)
                return OriginalHook(Play2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                ActionReady(EssentialDignity) &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_EssentialDignity &&
                canDignity)
                return EssentialDignity;

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) &&
                ActionReady(Exaltation) &&
                canExalt)
                return Exaltation;

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                ActionReady(CelestialIntersection) &&
                canIntersect &&
                !(healTarget as IBattleChara)!.HasShield())
                return CelestialIntersection;

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && ActionReady(AspectedBenefic))
            {
                Status? aspectedBeneficHoT = FindEffect(Buffs.AspectedBenefic, healTarget, LocalPlayer?.GameObjectId);
                Status? NeutralSectShield = FindEffect(Buffs.NeutralSectShield, healTarget, LocalPlayer?.GameObjectId);
                Status? NeutralSectBuff = FindEffect(Buffs.NeutralSect, healTarget, LocalPlayer?.GameObjectId);
                if ((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3)
                                                 || ((NeutralSectShield is null) && (NeutralSectBuff is not null)))
                    return AspectedBenefic;
            }
            return actionID;
        }
    }
}
