using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class AST : Healer
{
    internal class AST_Benefic : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Benefic;

        protected override uint Invoke(uint actionID) =>
            actionID is Benefic2 && !ActionReady(Benefic2)
                ? Benefic
                : actionID;
    }

    internal class AST_Lightspeed : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Lightspeed_Protection;       

        protected override uint Invoke(uint actionID) =>
           actionID is Lightspeed && HasStatusEffect(Buffs.Lightspeed)
               ? All.SavageBlade
               : actionID;
    }

    internal class AST_Raise_Alternative : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Raise_Alternative;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(CustomComboPreset.AST_Raise_Alternative_Retarget)
                    ? Ascend.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Ascend
                : actionID;
    }

    internal class AST_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            bool alternateMode = Config.AST_DPS_AltMode > 0; //(0 or 1 radio values)
            bool actionFound = !alternateMode && MaleficList.Contains(actionID) ||
                               alternateMode && CombustList.ContainsKey(actionID);
            bool cardPooling = IsEnabled(CustomComboPreset.AST_DPS_CardPool);
            bool lordPooling = IsEnabled(CustomComboPreset.AST_DPS_LordPool);

            if (!actionFound)
                return actionID;

            var replacedActions = alternateMode
                ? CombustList.Keys.ToArray()
                : MaleficList.ToArray();

            // Out of combat Card Draw
            if (!InCombat())
            {
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) &&
                    (Gauge.DrawnCards.All(x => x is CardType.None) ||
                     (DrawnDPSCard == CardType.None && Config.AST_ST_DPS_OverwriteCards)))
                    return OriginalHook(AstralDraw);
            }

            if (IsEnabled(CustomComboPreset.AST_ST_DPS_Opener) &&
                Opener().FullOpener(ref actionID))
            {
                if (actionID is EarthlyStar && IsEnabled(CustomComboPreset.AST_ST_DPS_EarthlyStar))
                    return actionID.Retarget(replacedActions,
                        SimpleTarget.AnyEnemy ?? SimpleTarget.Stack.Allies);
                if (actionID is (Balance or Spear) && IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards))
                    return actionID.Retarget(replacedActions, CardResolver);

                return actionID;
            }

            //In combat
            if (InCombat())
            {
                //Variant stuff
                if (Variant.CanRampart(CustomComboPreset.AST_Variant_Rampart))
                    return Variant.Rampart;

                if (Variant.CanSpiritDart(CustomComboPreset.AST_Variant_SpiritDart) && HasBattleTarget())
                    return Variant.SpiritDart;

                if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                    ActionReady(Lightspeed) &&
                    GetTargetHPPercent() > Config.AST_DPS_LightSpeedOption &&
                    IsMoving() &&
                    !HasStatusEffect(Buffs.Lightspeed) &&
                    (IsNotEnabled(CustomComboPreset.AST_DPS_LightSpeedHold) ||
                    GetCooldownChargeRemainingTime(Lightspeed) < GetCooldownRemainingTime(Divination) ||
                    !LevelChecked(Divination)))
                    return Lightspeed;                

                if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                    Role.CanLucidDream(Config.AST_LucidDreaming))
                    return Role.LucidDreaming;

                //Play Card with pooling option
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                    ActionReady(Play1) &&
                    Gauge.DrawnCards[0] is not CardType.None &&
                    CanSpellWeave() &&
                    (cardPooling && HasStatusEffect(Buffs.Divination, anyOwner: true) ||
                    !cardPooling ||
                    !LevelChecked(Divination)))
                    return IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards)
                        ? OriginalHook(Play1).Retarget(replacedActions, CardResolver)
                        : OriginalHook(Play1);

                //Minor Arcana / Lord of Crowns
                if (ActionReady(OriginalHook(MinorArcana)) &&
                    IsEnabled(CustomComboPreset.AST_DPS_LazyLord) &&
                    Gauge.DrawnCrownCard is CardType.Lord &&
                    HasBattleTarget() && CanDelayedWeave() &&
                    (lordPooling && HasStatusEffect(Buffs.Divination, anyOwner: true) ||
                    !lordPooling ||
                    !LevelChecked(Divination)))
                    return OriginalHook(MinorArcana);

                //Card Draw
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) &&
                    (Gauge.DrawnCards.All(x => x is CardType.None) ||
                     (DrawnDPSCard == CardType.None && Config.AST_ST_DPS_OverwriteCards)) &&
                    CanDelayedWeave())
                    return OriginalHook(AstralDraw);

                //Lightspeed Burst
                if (IsEnabled(CustomComboPreset.AST_DPS_LightspeedBurst) &&
                    ActionReady(Lightspeed) &&
                    !HasStatusEffect(Buffs.Lightspeed) &&
                    GetCooldownRemainingTime(Divination) < 5 &&
                    CanSpellWeave())
                    return Lightspeed;

                //Divination
                if (IsEnabled(CustomComboPreset.AST_DPS_Divination) &&
                    ActionReady(Divination) &&
                    !HasStatusEffect(Buffs.Divination, anyOwner: true) && //Overwrite protection
                    !HasStatusEffect(Buffs.Divining) &&
                    GetTargetHPPercent() > Config.AST_DPS_DivinationOption &&
                    CanDelayedWeave() &&
                    ActionWatching.NumberOfGcdsUsed >= 3)
                    return Divination;

                //Earthly Star
                if (IsEnabled(CustomComboPreset.AST_ST_DPS_EarthlyStar) &&
                    !HasStatusEffect(Buffs.EarthlyDominance) &&
                    ActionReady(EarthlyStar) &&
                    IsOffCooldown(EarthlyStar) &&
                    CanSpellWeave())
                    return EarthlyStar.Retarget(replacedActions,
                        SimpleTarget.AnyEnemy ?? SimpleTarget.Stack.Allies);

                if (IsEnabled(CustomComboPreset.AST_DPS_Oracle) &&
                    HasStatusEffect(Buffs.Divining) &&
                    CanSpellWeave())
                    return Oracle;

                if (HasBattleTarget())
                {
                    //Combust
                    if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                        !GravityList.Contains(actionID) &&
                        LevelChecked(Combust) &&
                        CombustList.TryGetValue(OriginalHook(Combust), out ushort dotDebuffID))
                    {   
                        float refreshTimer = Config.AST_ST_DPS_CombustUptime_Threshold;
                        int hpThreshold = Config.AST_ST_DPS_CombustSubOption == 1 || !InBossEncounter() ? Config.AST_DPS_CombustOption : 0;
                        if (GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget) <= refreshTimer &&
                            GetTargetHPPercent() > hpThreshold &&
                            CanApplyStatus(CurrentTarget,dotDebuffID))
                            return OriginalHook(Combust);

                        //Alternate Mode (idles as Malefic)
                        if (alternateMode)
                            return OriginalHook(Malefic);
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
            bool cardPooling = IsEnabled(CustomComboPreset.AST_AOE_CardPool);
            bool lordPooling = IsEnabled(CustomComboPreset.AST_AOE_LordPool);

            if (!GravityList.Contains(actionID))
                return actionID;

            //Variant stuff
            if (Variant.CanRampart(CustomComboPreset.AST_Variant_Rampart))
                return Variant.Rampart;

            if (Variant.CanSpiritDart(CustomComboPreset.AST_Variant_SpiritDart))
                return Variant.SpiritDart;

            if (IsEnabled(CustomComboPreset.AST_AOE_LightSpeed) &&
                ActionReady(Lightspeed) &&
                GetTargetHPPercent() > Config.AST_AOE_LightSpeedOption &&
                IsMoving() &&
                !HasStatusEffect(Buffs.Lightspeed) &&
                (IsNotEnabled(CustomComboPreset.AST_AOE_LightSpeedHold) ||
                GetCooldownChargeRemainingTime(Lightspeed) < GetCooldownRemainingTime(Divination)  ||
                !LevelChecked(Divination)))
                return Lightspeed;            

            if (IsEnabled(CustomComboPreset.AST_AOE_Lucid) &&
                Role.CanLucidDream(Config.AST_LucidDreaming))
                return Role.LucidDreaming;

            //Play Card with Pooling
            if (IsEnabled(CustomComboPreset.AST_AOE_AutoPlay) &&
                ActionReady(Play1) &&
                Gauge.DrawnCards[0] is not CardType.None &&
                CanSpellWeave() &&
                (cardPooling && HasStatusEffect(Buffs.Divination, anyOwner: true) ||
                !cardPooling ||
                !LevelChecked(Divination)))
                return IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards)
                    ? OriginalHook(Play1).Retarget(GravityList.ToArray(),
                        CardResolver)
                    : OriginalHook(Play1);

            //Minor Arcana / Lord of Crowns
            if (ActionReady(OriginalHook(MinorArcana)) &&
                IsEnabled(CustomComboPreset.AST_AOE_LazyLord) && Gauge.DrawnCrownCard is CardType.Lord &&
                HasBattleTarget() && CanDelayedWeave() &&
                (lordPooling && HasStatusEffect(Buffs.Divination, anyOwner: true) ||
                !lordPooling ||
                !LevelChecked(Divination)))
                return OriginalHook(MinorArcana);

            //Card Draw
            if (IsEnabled(CustomComboPreset.AST_AOE_AutoDraw) &&
                ActionReady(OriginalHook(AstralDraw)) &&
                (Gauge.DrawnCards.All(x => x is CardType.None) ||
                 (DrawnDPSCard == CardType.None && Config.AST_AOE_DPS_OverwriteCards)) &&
                CanDelayedWeave())
                return OriginalHook(AstralDraw);
            
            //Lightspeed Burst
            if (IsEnabled(CustomComboPreset.AST_AOE_LightspeedBurst) &&
                ActionReady(Lightspeed) &&
                !HasStatusEffect(Buffs.Lightspeed) &&
                GetCooldownRemainingTime(Divination) < 5 &&
                ActionWatching.NumberOfGcdsUsed >= 3 &&
                CanSpellWeave())
                return Lightspeed;

            //Divination
            if (IsEnabled(CustomComboPreset.AST_AOE_Divination) &&
                ActionReady(Divination) &&
                !HasStatusEffect(Buffs.Divination, anyOwner: true) && //Overwrite protection
                GetTargetHPPercent() > Config.AST_AOE_DivinationOption &&
                CanDelayedWeave() &&
                ActionWatching.NumberOfGcdsUsed >= 3)
                return Divination;

            //Earthly Star
            if (IsEnabled(CustomComboPreset.AST_AOE_DPS_EarthlyStar) && !IsMoving() &&
                !HasStatusEffect(Buffs.EarthlyDominance) &&
                ActionReady(EarthlyStar) &&
                IsOffCooldown(EarthlyStar) &&
                CanSpellWeave() &&
                ActionWatching.NumberOfGcdsUsed >= 3)
                return EarthlyStar.Retarget(GravityList.ToArray(),
                    SimpleTarget.AnyEnemy ?? SimpleTarget.Stack.Allies);            
                
            if (IsEnabled(CustomComboPreset.AST_AOE_Oracle) &&
                HasStatusEffect(Buffs.Divining) &&
                CanSpellWeave())
                return Oracle;

            if (IsEnabled(CustomComboPreset.AST_AOE_DPS_MacroCosmos) &&
                ActionReady(Macrocosmos) &&
                !HasStatusEffect(Buffs.Macrocosmos) &&
                ActionWatching.NumberOfGcdsUsed >= 3 &&
                (Config.AST_AOE_DPS_MacroCosmos_SubOption == 1 ||
                !InBossEncounter()))
                return Macrocosmos;

            return actionID;
        }
    }

    internal class AST_AoE_SimpleHeals_AspectedHelios : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_AoE_SimpleHeals_AspectedHelios;

        protected override uint Invoke(uint actionID)
        {
            bool nonAspectedMode = Config.AST_AoEHeals_AltMode > 0; //(0 or 1 radio values)

            if ((!nonAspectedMode || actionID is not Helios) &&
                (nonAspectedMode || actionID is not (AspectedHelios or HeliosConjuction)))
                return actionID;

            bool canWeaveLady = Config.AST_AoE_SimpleHeals_WeaveLady && CanSpellWeave() || !Config.AST_AoE_SimpleHeals_WeaveLady;            
            bool canWeaveHoroscope = Config.AST_AoE_SimpleHeals_Horoscope && CanSpellWeave() || !Config.AST_AoE_SimpleHeals_Horoscope;
            bool canWeaveOppose = Config.AST_AoE_SimpleHeals_Opposition && CanSpellWeave() || !Config.AST_AoE_SimpleHeals_Opposition;
            bool canWeaveSect = Config.AST_AoE_SimpleHeals_NeutralSectWeave && CanSpellWeave() || !Config.AST_AoE_SimpleHeals_NeutralSectWeave;
            bool hasHealthLady = Config.AST_AoE_SimpleHeals_LazyLadyThreshold >= GetPartyAvgHPPercent();
            bool hasHealthHoroscope = Config.AST_AoE_SimpleHeals_HoroscopeThreshold >= GetPartyAvgHPPercent();
            bool hasHealthOppose = Config.AST_AoE_SimpleHeals_CelestialOppositionThreshold >= GetPartyAvgHPPercent();
            bool hasHealthSect = Config.AST_AoE_SimpleHeals_NeutralSectThreshold >= GetPartyAvgHPPercent();


            if (!LevelChecked(AspectedHelios)) //Level check to return helios immediately below 40
                return Helios;

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_LazyLady) &&
                ActionReady(MinorArcana) &&
                Gauge.DrawnCrownCard is CardType.Lady
                && canWeaveLady && hasHealthLady)
                return OriginalHook(MinorArcana);

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition) &&
                ActionReady(CelestialOpposition) &&
                canWeaveOppose && hasHealthOppose)
                return CelestialOpposition;

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Horoscope))
            {
                if (ActionReady(Horoscope) &&
                    !HasStatusEffect(Buffs.Horoscope) &&
                    !HasStatusEffect(Buffs.HoroscopeHelios) &&
                    canWeaveHoroscope && hasHealthHoroscope)
                    return Horoscope;

                if (HasStatusEffect(Buffs.HoroscopeHelios) &&
                    canWeaveHoroscope && hasHealthHoroscope)
                    return HoroscopeHeal;
            }

            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_NeutralSect) &&
                ActionReady(OriginalHook(NeutralSect)) && canWeaveSect && hasHealthSect)                
                return OriginalHook(NeutralSect);

            // Only check for our own HoTs
            Status? hotCheck = HeliosConjuction.LevelChecked() ? GetStatusEffect(Buffs.HeliosConjunction) : GetStatusEffect(Buffs.AspectedHelios);
            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_Aspected) && nonAspectedMode || // Helios mode: option must be on
                !nonAspectedMode) // Aspected mode: option is not required
            {
                if (ActionReady(AspectedHelios)
                    && hotCheck is null
                    || HasStatusEffect(Buffs.NeutralSect) && !HasStatusEffect(Buffs.NeutralSectShield))
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
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
            bool canWeaveDignity = Config.AST_ST_SimpleHeals_WeaveDignity && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveDignity;
            bool canWeaveIntersect = Config.AST_ST_SimpleHeals_WeaveIntersection && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveIntersection;
            bool canWeaveExalt = Config.AST_ST_SimpleHeals_WeaveExalt && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveExalt;
            bool canWeaveEwer = Config.AST_ST_SimpleHeals_WeaveEwer && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveEwer;
            bool canWeaveSpire = Config.AST_ST_SimpleHeals_WeaveSpire && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveSpire;
            bool canWeaveBole = Config.AST_ST_SimpleHeals_WeaveBole && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveBole;
            bool canWeaveArrow = Config.AST_ST_SimpleHeals_WeaveArrow && CanSpellWeave() || !Config.AST_ST_SimpleHeals_WeaveArrow;
            bool startHot = Config.AST_ST_SimpleHeals_AspectedBeneficHigh >= GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields);
            bool stopHot = Config.AST_ST_SimpleHeals_AspectedBeneficLow <= GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields);
            int refreshTime = Config.AST_ST_SimpleHeals_AspectedBeneficRefresh;

            //Grab our target


            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Esuna) && ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) >= Config.AST_ST_SimpleHeals_Esuna &&
                HasCleansableDebuff(healTarget))
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Spire) &&
                Gauge.DrawnCards[2] == CardType.Spire &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Spire &&
                ActionReady(Play3) &&
                canWeaveSpire)
                return OriginalHook(Play3)
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Ewer) &&
                Gauge.DrawnCards[2] == CardType.Ewer &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Ewer &&
                ActionReady(Play3) &&
                canWeaveEwer)
                return OriginalHook(Play3)
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Arrow) &&
                Gauge.DrawnCards[1] == CardType.Arrow &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Arrow &&
                ActionReady(Play2) &&
                canWeaveArrow)
                return OriginalHook(Play2)
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Bole) &&
                Gauge.DrawnCards[1] == CardType.Bole &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_Bole &&
                ActionReady(Play2) &&
                canWeaveBole)
                return OriginalHook(Play2)
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity) &&
                ActionReady(EssentialDignity) &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= Config.AST_EssentialDignity &&
                canWeaveDignity)
                return EssentialDignity
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Exaltation) &&
                ActionReady(Exaltation) &&
                canWeaveExalt)
                return Exaltation
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection) &&
                ActionReady(CelestialIntersection) &&
                canWeaveIntersect &&
                !(healTarget as IBattleChara)!.HasShield())
                return CelestialIntersection
                    .RetargetIfEnabled(OptionalTarget, Benefic2);

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic) && ActionReady(AspectedBenefic) &&
                startHot && stopHot)
            {
                //Possibly a good use for new HasStatusEffect with Status Out
                //HasStatusEffect(Buffs.AspectedBenefic, out Status? aspectedBeneficHoT, healTarget);
                Status? aspectedBeneficHoT = GetStatusEffect(Buffs.AspectedBenefic, healTarget);
                Status? neutralSectShield = GetStatusEffect(Buffs.NeutralSectShield, healTarget);
                Status? neutralSectBuff = GetStatusEffect(Buffs.NeutralSect, healTarget);
                if (aspectedBeneficHoT is null || aspectedBeneficHoT.RemainingTime <= refreshTime
                                               || neutralSectShield is null && neutralSectBuff is not null)
                    return AspectedBenefic
                        .RetargetIfEnabled(OptionalTarget, Benefic2);
            }
            return actionID
                .RetargetIfEnabled(OptionalTarget, Benefic2);
        }
    }

    internal class AST_RetargetEssentialDignity : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_RetargetEssentialDignity;

        protected override uint Invoke(uint actionID) =>
            actionID is not EssentialDignity
                ? actionID
                : actionID.Retarget(SimpleTarget.Stack.AllyToHeal, dontCull: true);
    }

    internal class AST_RetargetManualCards : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_Cards_QuickTargetCards;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Play1 ||
                !Config.AST_QuickTarget_Manuals)
                return actionID;

            OriginalHook(Play1).Retarget(Play1, CardResolver, dontCull: true);

            return actionID;
        }
    }
}
