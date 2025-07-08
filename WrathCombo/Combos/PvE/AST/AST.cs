﻿using Dalamud.Game.ClientState.Statuses;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
namespace WrathCombo.Combos.PvE;

internal partial class AST : Healer
{
    internal class AST_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            #region Variables
            bool alternateMode = Config.AST_ST_DPS_AltMode > 0; //(0 or 1 radio values)
            bool actionFound = !alternateMode && MaleficList.Contains(actionID) ||
                               alternateMode && CombustList.ContainsKey(actionID);
            bool cardPooling = IsEnabled(CustomComboPreset.AST_DPS_CardPool);
            bool lordPooling = IsEnabled(CustomComboPreset.AST_DPS_LordPool);
            var replacedActions = alternateMode
                ? CombustList.Keys.ToArray()
                : MaleficList.ToArray();
            float refreshTimer = Config.AST_ST_DPS_CombustUptime_Threshold;
            int dotHPThreshold = Config.AST_ST_DPS_CombustSubOption == 1 || !InBossEncounter() ? Config.AST_ST_DPS_CombustOption : 0;
            int divHPThreshold = Config.AST_ST_DPS_DivinationSubOption == 1 || !InBossEncounter() ? Config.AST_ST_DPS_DivinationOption : 0;
            #endregion

            if (!actionFound)
                return actionID;

            // Out-of-combat Card Draw
            if (!InCombat())
            {
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) &&
                    (HasNoCards || HasNoDPSCard && Config.AST_ST_DPS_OverwriteHealCards))
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

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            //In combat
            if (InCombat())
            {
                //Variant stuff
                if (Variant.CanRampart(CustomComboPreset.AST_Variant_Rampart))
                    return Variant.Rampart;

                if (Variant.CanSpiritDart(CustomComboPreset.AST_Variant_SpiritDart) && HasBattleTarget())
                    return Variant.SpiritDart;

                //Lightspeed Movement
                if (IsEnabled(CustomComboPreset.AST_DPS_LightSpeed) &&
                    ActionReady(Lightspeed) &&
                    GetTargetHPPercent() > Config.AST_ST_DPS_LightSpeedOption &&
                    IsMoving() && !HasStatusEffect(Buffs.Lightspeed) &&
                    (IsNotEnabled(CustomComboPreset.AST_DPS_LightSpeedHold) ||
                    LightspeedChargeCD < DivinationCD ||
                    !LevelChecked(Divination)))
                    return Lightspeed;  
                
                #region Hidden Feature Raidwide

                if (HiddenCollectiveUnconscious())
                    return CollectiveUnconscious;
                if (HiddenNeutralSect())
                    return OriginalHook(NeutralSect);
                if (HiddenAspectedHelios())
                    return OriginalHook(AspectedHelios);
           
                #endregion

                //Lucid Dreaming
                if (IsEnabled(CustomComboPreset.AST_DPS_Lucid) &&
                    Role.CanLucidDream(Config.AST_ST_DPS_LucidDreaming))
                    return Role.LucidDreaming;

                //Play Card
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoPlay) &&
                    HasDPSCard && CanSpellWeave() &&
                    (HasDivination || !cardPooling || !LevelChecked(Divination)))
                    return IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards)
                        ? OriginalHook(Play1).Retarget(replacedActions, CardResolver)
                        : OriginalHook(Play1);

                //Minor Arcana / Lord of Crowns
                if (ActionReady(OriginalHook(MinorArcana)) &&
                    IsEnabled(CustomComboPreset.AST_DPS_LazyLord) &&
                    HasLord && HasBattleTarget() && CanSpellWeave() &&
                    (HasDivination || !lordPooling || !LevelChecked(Divination)))
                    return OriginalHook(MinorArcana);

                //Card Draw
                if (IsEnabled(CustomComboPreset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) &&
                    (HasNoCards || HasNoDPSCard && Config.AST_ST_DPS_OverwriteHealCards) &&
                    CanSpellWeave())
                    return OriginalHook(AstralDraw);

                //Lightspeed Burst
                if (IsEnabled(CustomComboPreset.AST_DPS_LightspeedBurst) &&
                    ActionReady(Lightspeed) && !HasStatusEffect(Buffs.Lightspeed) &&
                    DivinationCD < 5 && CanSpellWeave())
                    return Lightspeed;

                //Divination
                if (IsEnabled(CustomComboPreset.AST_DPS_Divination) && HasBattleTarget() &&
                    ActionReady(Divination) && !HasDivination && //Overwrite protection
                    !HasStatusEffect(Buffs.Divining) &&
                    GetTargetHPPercent() > divHPThreshold &&
                    CanSpellWeave() && ActionWatching.NumberOfGcdsUsed >= 3)
                    return Divination;

                //Earthly Star
                if (IsEnabled(CustomComboPreset.AST_ST_DPS_EarthlyStar) &&
                    !HasStatusEffect(Buffs.EarthlyDominance) && ActionReady(EarthlyStar) &&
                    IsOffCooldown(EarthlyStar) && CanSpellWeave())
                    return EarthlyStar.Retarget(replacedActions,
                        SimpleTarget.AnyEnemy ?? SimpleTarget.Stack.Allies);

                //Oracle
                if (IsEnabled(CustomComboPreset.AST_DPS_Oracle) &&
                    HasStatusEffect(Buffs.Divining) && CanSpellWeave())
                    return Oracle;

                //Dot Code
                if (HasBattleTarget())
                {
                    //Combust
                    if (IsEnabled(CustomComboPreset.AST_ST_DPS_CombustUptime) &&
                        !GravityList.Contains(actionID) &&
                        LevelChecked(Combust) &&
                        CombustList.TryGetValue(OriginalHook(Combust), out ushort dotDebuffID))
                    {   
                        if (GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget) <= refreshTimer &&
                            GetTargetHPPercent() > dotHPThreshold &&
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
            #region Variables
            bool cardPooling = IsEnabled(CustomComboPreset.AST_AOE_CardPool);
            bool lordPooling = IsEnabled(CustomComboPreset.AST_AOE_LordPool);
            int divHPThreshold = Config.AST_ST_DPS_DivinationSubOption == 1 || !InBossEncounter() ? Config.AST_ST_DPS_DivinationOption : 0;
            #endregion

            if (!GravityList.Contains(actionID))
                return actionID;

            //Variant stuff
            if (Variant.CanRampart(CustomComboPreset.AST_Variant_Rampart))
                return Variant.Rampart;

            if (Variant.CanSpiritDart(CustomComboPreset.AST_Variant_SpiritDart))
                return Variant.SpiritDart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            //Lightspeed Movement
            if (IsEnabled(CustomComboPreset.AST_AOE_LightSpeed) &&
                ActionReady(Lightspeed) &&
                GetTargetHPPercent() > Config.AST_AOE_LightSpeedOption &&
                IsMoving() && !HasStatusEffect(Buffs.Lightspeed) &&
                (IsNotEnabled(CustomComboPreset.AST_AOE_LightSpeedHold) ||
                LightspeedChargeCD < DivinationCD  ||
                !LevelChecked(Divination)))
                return Lightspeed;  
            
            #region Hidden Feature Raidwide

            if (HiddenCollectiveUnconscious())
                return CollectiveUnconscious;
            if (HiddenNeutralSect())
                return OriginalHook(NeutralSect);
            if (HiddenAspectedHelios())
                return OriginalHook(AspectedHelios);
           
            #endregion

            //Lucid Dreaming
            if (IsEnabled(CustomComboPreset.AST_AOE_Lucid) &&
                Role.CanLucidDream(Config.AST_AOE_LucidDreaming))
                return Role.LucidDreaming;

            //Play Card
            if (IsEnabled(CustomComboPreset.AST_AOE_AutoPlay) &&
                HasDPSCard && CanSpellWeave() && 
                (HasDivination || !cardPooling || !LevelChecked(Divination)))
                return IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards)
                    ? OriginalHook(Play1).Retarget(GravityList.ToArray(),
                        CardResolver)
                    : OriginalHook(Play1);

            //Minor Arcana / Lord of Crowns
            if (ActionReady(OriginalHook(MinorArcana)) &&
                IsEnabled(CustomComboPreset.AST_AOE_LazyLord) && HasLord &&
                HasBattleTarget() && CanSpellWeave() &&
                (HasDivination || !lordPooling || !LevelChecked(Divination)))
                return OriginalHook(MinorArcana);

            //Card Draw
            if (IsEnabled(CustomComboPreset.AST_AOE_AutoDraw) &&
                ActionReady(OriginalHook(AstralDraw)) &&
                (HasNoCards || HasNoDPSCard && Config.AST_AOE_DPS_OverwriteHealCards) &&
                CanSpellWeave())
                return OriginalHook(AstralDraw);
            
            //Lightspeed Burst
            if (IsEnabled(CustomComboPreset.AST_AOE_LightspeedBurst) &&
                ActionReady(Lightspeed) && !HasStatusEffect(Buffs.Lightspeed) &&
                DivinationCD < 5 && ActionWatching.NumberOfGcdsUsed >= 3 &&
                CanSpellWeave())
                return Lightspeed;

            //Divination
            if (IsEnabled(CustomComboPreset.AST_AOE_Divination) && HasBattleTarget() &&
                ActionReady(Divination) && !HasDivination && //Overwrite protection
                GetTargetHPPercent() > divHPThreshold && CanSpellWeave() &&
                ActionWatching.NumberOfGcdsUsed >= 3)
                return Divination;

            //Earthly Star
            if (IsEnabled(CustomComboPreset.AST_AOE_DPS_EarthlyStar) && !IsMoving() &&
                !HasStatusEffect(Buffs.EarthlyDominance) && ActionReady(EarthlyStar) &&
                IsOffCooldown(EarthlyStar) && CanSpellWeave() &&
                ActionWatching.NumberOfGcdsUsed >= 3)
                return EarthlyStar.Retarget(GravityList.ToArray(),
                    SimpleTarget.AnyEnemy ?? SimpleTarget.Stack.Allies);            
            
            //Oracle
            if (IsEnabled(CustomComboPreset.AST_AOE_Oracle) &&
                HasStatusEffect(Buffs.Divining) && CanSpellWeave())
                return Oracle;

            //MacroCosmos
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

    internal class AST_ST_SimpleHeals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_ST_SimpleHeals;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Benefic2)
                return actionID;
            
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
            
            #region Hidden Feature Raidwide

            if (HiddenCollectiveUnconscious())
                return CollectiveUnconscious;
            if (HiddenNeutralSect())
                return OriginalHook(NeutralSect);
            if (HiddenAspectedHelios())
                return OriginalHook(AspectedHelios);
           
            #endregion

            if (IsEnabled(CustomComboPreset.AST_ST_SimpleHeals_Esuna) && ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) >= Config.AST_ST_SimpleHeals_Esuna &&
                HasCleansableDebuff(healTarget))
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Benefic2);
            
            //Priority List
            for(int i = 0; i < Config.AST_ST_SimpleHeals_Priority.Count; i++)
            {
                int index = Config.AST_ST_SimpleHeals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                {
                    if (GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell.RetargetIfEnabled(OptionalTarget, Benefic2);
                }
            }
            return LevelChecked(Benefic2) ?
                actionID.RetargetIfEnabled(OptionalTarget, Benefic2):
                Benefic.RetargetIfEnabled(OptionalTarget, Benefic2);
        }
    }

    internal class AST_AoE_SimpleHeals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AST_AoE_SimpleHeals;

        protected override uint Invoke(uint actionID)
        {
            bool nonAspectedMode = Config.AST_AoE_SimpleHeals_AltMode > 0; //(0 or 1 radio values)

            if ((!nonAspectedMode || actionID is not Helios) &&
                (nonAspectedMode || actionID is not (AspectedHelios or HeliosConjuction)))
                return actionID;
            
            //Level check to return helios immediately below 40
            if (!LevelChecked(AspectedHelios)) 
                return Helios;
            
            #region Hidden Feature Raidwide

            if (HiddenCollectiveUnconscious())
                return CollectiveUnconscious;
            if (HiddenNeutralSect())
                return OriginalHook(NeutralSect);
            if (HiddenAspectedHelios())
                return OriginalHook(AspectedHelios);
           
            #endregion
            
            //Horoscope check to trigger the ability to do the larger Horoscope Heal
            if (HasStatusEffect(Buffs.Horoscope))
                return HasStatusEffect(Buffs.HeliosConjunction) || HasStatusEffect(Buffs.AspectedHelios)
                    ? Helios
                    : OriginalHook(AspectedHelios);
            
            if (IsEnabled(CustomComboPreset.AST_AoE_SimpleHeals_NeutralSect) && HasStatusEffect(Buffs.Suntouched) && CanWeave())
                return SunSign;
            
            //Priority List
            float averagePartyHP = GetPartyAvgHPPercent();
            for(int i = 0; i < Config.AST_AoE_SimpleHeals_Priority.Count; i++)
            {
                int index = Config.AST_AoE_SimpleHeals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);

                if (enabled && averagePartyHP <= config && ActionReady(spell))
                    return spell;
            }
            
            //Hot Check for if you are in Aspected Helios Mode
            Status? hotCheck = HeliosConjuction.LevelChecked() ? GetStatusEffect(Buffs.HeliosConjunction) : GetStatusEffect(Buffs.AspectedHelios);
            if (!nonAspectedMode && hotCheck is not null && hotCheck.RemainingTime > GetActionCastTime(OriginalHook(AspectedHelios)) + 1f)
                return Helios;
            
            return 
                actionID;
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
                    ? Ascend
                        .Retarget(Role.Swiftcast,
                            SimpleTarget.Stack.AllyToRaise)
                        .AndRunMacro()
                    : Ascend.AndRunMacro()
                : actionID;
    }
}
