using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using ECommons.Logging;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.SCH.Config;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;
namespace WrathCombo.Combos.PvE;

internal partial class SCH : Healer
{
    #region Simple DPS
    internal class SCH_ST_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_ST_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!BroilList.Contains(actionID))
                return actionID;

            if (NeedToSummon)
                return SummonEos;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Dissolve Union
            if (EndAetherpact)
                return DissolveUnion;
            #endregion

            if (InCombat() && CanWeave())
            {
                if (!WasLastAction(Dissipation) && ActionReady(Aetherflow) && !HasAetherflow)
                    return Aetherflow;

                if (HasStatusEffect(Buffs.ImpactImminent) && !JustUsed(ChainStratagem))
                    return BanefulImpaction;

                if (ActionWatching.NumberOfGcdsUsed > 3 && CanChainStrategem)
                    return ChainStratagem;

                if (ActionReady(EnergyDrain) && AetherflowCD <= 10 &&
                    (ChainStrategemCD > 10 || !LevelChecked(ChainStratagem)))
                    return EnergyDrain;

                if (Role.CanLucidDream(6500))
                    return Role.LucidDreaming;
            }
            //Bio/Biolysis
            if (NeedsDoT() && InCombat())
                return OriginalHook(Bio);

            //Ruin 2 Movement
            if (ActionReady(Ruin2) && IsMoving() && InCombat())
                return OriginalHook(Ruin2);

            return actionID;
        }
    }
    
    internal class SCH_AoE_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_AoE_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ArtOfWar or ArtOfWarII))
                return actionID;

            if (NeedToSummon)
                return SummonEos;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Dissolve Union
            if (EndAetherpact)
                return DissolveUnion;
            #endregion

            if (!WasLastAction(Dissipation) && ActionReady(Aetherflow) && !HasAetherflow && CanWeave())
                return Aetherflow;

            if (HasStatusEffect(Buffs.ImpactImminent) && !JustUsed(ChainStratagem) && CanWeave())
                return BanefulImpaction;

            if (ActionWatching.NumberOfGcdsUsed > 3 && CanChainStrategem && CanWeave())
                return ChainStratagem;

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_EnergyDrain) && ActionReady(EnergyDrain) &&
                AetherflowCD <= 10 && CanWeave())
                return EnergyDrain;

            var dotAction = OriginalHook(Bio);
            BioList.TryGetValue(dotAction, out var dotDebuffID);
            var target =
                SimpleTarget.DottableEnemy(dotAction, dotDebuffID, 30, 3, 4);

            if (ActionReady(dotAction) && target != null)
                return OriginalHook(Bio).Retarget([ArtOfWar, ArtOfWarII], target);

            if (Role.CanLucidDream(SCH_AoE_DPS_LucidOption) && CanWeave())
                return Role.LucidDreaming;

            return actionID;
        }
    }

    #endregion
    
    #region Simple Heals
    internal class SCH_Simple_ST_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Simple_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Physick)
                return actionID;
            
            #region Variables
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
            #endregion
            
            if (EndAetherpact)
                return DissolveUnion;
            
            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);
            
            if (ActionReady(Role.Esuna) && GetTargetHPPercent(healTarget) >= 40 &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(healTarget, Physick);
            
            if (ActionReady(Aetherflow) && !HasAetherflow &&
                InCombat())
                return Aetherflow;
            
            if (ActionReady(Dissipation) && !HasAetherflow &&
                InCombat() && !FairyBusy)
                return Dissipation;
            
            if (Role.CanLucidDream(6500) && CanWeave()) 
                return Role.LucidDreaming;
            
            if (Gauge.SeraphTimer > 0 && !FairyBusy && ActionReady(Consolation))
                return Consolation;
            
            if (ActionReady(Excogitation) &&
                GetTargetHPPercent(healTarget) <= 50)
                return Excogitation.RetargetIfEnabled(healTarget, Physick);
            
            if (ActionReady(Lustrate) &&
                GetTargetHPPercent(healTarget) <= 50)
                return Lustrate.RetargetIfEnabled(healTarget, Physick);
            
            if (ActionReady(SacredSoil) && !InBossEncounter() &&
                TimeStoodStill >= TS.FromSeconds(5))
                return SacredSoil.Retarget(Physick, SimpleTarget.Self);
            
            if (ActionReady(Protraction) && (healTarget.IsInParty() && healTarget.GetRole() is CombatRole.Tank || !IsInParty())) 
                return Protraction.RetargetIfEnabled(healTarget, Physick);
            
            if (Gauge.FairyGauge >= 50 && IsOriginal(Aetherpact) && !FairyBusy)
                return Aetherpact.RetargetIfEnabled(healTarget, Physick);

            if (!InBossEncounter() && HasPetPresent() && !FairyBusy)
            {
                if (ActionReady(WhisperingDawn))
                    return WhisperingDawn;
                
                if (ActionReady(FeyIllumination))
                    return FeyIllumination;
                
                if (ActionReady(FeyBlessing))
                    return FeyBlessing;
                
                if (ActionReady(OriginalHook(SummonSeraph)))
                    return OriginalHook(SummonSeraph);
                
                if (ActionReady(Seraphism))
                    return Seraphism;
            }

            if (ActionReady(Expedient) && !InBossEncounter())
                return Expedient;
            
            if (ActionReady(OriginalHook(Adloquium)))
                return ActionReady(OriginalHook(EmergencyTactics)) && (HasStatusEffect(Buffs.Galvanize, healTarget, true) || !HasStatusEffect(Buffs.EmergencyTactics))
                    ? OriginalHook(EmergencyTactics)
                    : OriginalHook(Adloquium).RetargetIfEnabled(healTarget, Physick);
            
            return actionID.RetargetIfEnabled(healTarget);
        }
    }
    
    internal class SCH_Simple_AoE_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Simple_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Succor or Concitation))
                return actionID;
            
            if (EndAetherpact)
                return DissolveUnion;
            
            if (ActionReady(Expedient) && RaidWideCasting())
                return Expedient;
            
            if (ActionReady(SacredSoil) && RaidWideCasting())
                return SacredSoil.Retarget([Succor, Concitation], SimpleTarget.Self);
            
            if (ActionReady(Aetherflow) && !HasAetherflow &&
                InCombat())
                return Aetherflow;
            
            if (Role.CanLucidDream(6500) && CanWeave()) 
                return Role.LucidDreaming;

            if (Gauge.SeraphTimer > 0 && !FairyBusy && ActionReady(Consolation))
                return Consolation;

            if (InCombat() && IsOffCooldown(Indomitability) && LevelChecked(Indomitability))
                return !HasAetherflow && ActionReady(Recitation)
                    ? Recitation
                    : Indomitability;

            if (HasPetPresent() && !FairyBusy)
            {
                if (ActionReady(WhisperingDawn))
                    return WhisperingDawn;
                
                if (ActionReady(FeyIllumination))
                    return FeyIllumination;
                
                if (ActionReady(FeyBlessing))
                    return FeyBlessing;
                
                if (ActionReady(OriginalHook(SummonSeraph)))
                    return OriginalHook(SummonSeraph);
                
                if (ActionReady(Seraphism))
                    return Seraphism;
            }
            
            return ActionReady(OriginalHook(EmergencyTactics)) && (GetPartyBuffPercent(Buffs.Galvanize) >= 50 || GetPartyBuffPercent(SGE.Buffs.EukrasianPrognosis) >= 50)
                ? OriginalHook(EmergencyTactics)
                : actionID;
        }
    }
    #endregion
    
    #region Advanced DPS
    internal class SCH_ST_ADV_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_ST_ADV_DPS;

        protected override uint Invoke(uint actionID)
        {
            bool actionFound = SCH_ST_DPS_Adv_Actions == 0 && BroilList.Contains(actionID) ||
                               SCH_ST_DPS_Adv_Actions == 1 && BioList.ContainsKey(actionID) ||
                               SCH_ST_DPS_Adv_Actions == 2 && actionID is Ruin2;

            if (!actionFound)
                return actionID;

            #region Variables
            int chainThreshold = SCH_ST_DPS_ChainStratagemSubOption == 1 || !InBossEncounter() ? SCH_ST_DPS_ChainStratagemOption : 0;
            #endregion

            if (!actionFound)
                return actionID;

            if (IsEnabled(Preset.SCH_ST_ADV_DPS_FairyReminder) && NeedToSummon)
                return SummonEos;
            //Opener
            if (IsEnabled(Preset.SCH_ST_ADV_DPS_Balance_Opener) && Opener().FullOpener(ref actionID))
                return actionID;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Healing Helpers
            if (EndAetherpact)
                return DissolveUnion;
            if (RaidwideSacredSoil())
                return SacredSoil.Retarget(ReplacedActionsList.ToArray(), SimpleTarget.Self);
            if (RaidwideExpedient())
                return Expedient;
            if (RaidwideSuccor())
                return RaidwideRecitation() ? Recitation : OriginalHook(Succor);
            #endregion

            if (InCombat() && CanWeave())
            {
                if (IsEnabled(Preset.SCH_ST_ADV_DPS_Aetherflow) && !WasLastAction(Dissipation) && ActionReady(Aetherflow) && !HasAetherflow)
                    return Aetherflow;

                if (IsEnabled(Preset.SCH_ST_ADV_DPS_BanefulImpact) && HasStatusEffect(Buffs.ImpactImminent) && !JustUsed(ChainStratagem))
                    return BanefulImpaction;

                if (IsEnabled(Preset.SCH_ST_ADV_DPS_ChainStrat) && ActionWatching.NumberOfGcdsUsed > 3 && CanChainStrategem &&
                    GetTargetHPPercent() > chainThreshold)
                    return ChainStratagem;

                if (IsEnabled(Preset.SCH_ST_ADV_DPS_EnergyDrain) && ActionReady(EnergyDrain) &&
                    AetherflowCD <= SCH_ST_DPS_EnergyDrain &&
                    (!SCH_ST_DPS_EnergyDrain_Burst ||
                     ChainStrategemCD > 10 ||
                     !LevelChecked(ChainStratagem)))
                    return EnergyDrain;

                if (IsEnabled(Preset.SCH_ST_ADV_DPS_Lucid) && Role.CanLucidDream(SCH_ST_DPS_LucidOption))
                    return Role.LucidDreaming;
            }

            //Bio/Biolysis
            if (IsEnabled(Preset.SCH_ST_ADV_DPS_Bio) && NeedsDoT() && InCombat())
                return OriginalHook(Bio);

            //Ruin 2 Movement
            if (IsEnabled(Preset.SCH_ST_ADV_DPS_Ruin2Movement) && ActionReady(Ruin2) && IsMoving() && InCombat())
                return OriginalHook(Ruin2);

            return actionID;
        }
    }
    
    internal class SCH_AoE_ADV_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_AoE_ADV_DPS;

        protected override uint Invoke(uint actionID)
        {
            #region Variables
            int chainThreshold = SCH_AoE_DPS_ChainStratagemSubOption == 1 || !InBossEncounter() ? SCH_AoE_DPS_ChainStratagemOption : 0;
            #endregion

            if (actionID is not (ArtOfWar or ArtOfWarII))
                return actionID;

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_FairyReminder) &&
                NeedToSummon)
                return SummonEos;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Healing Helpers
            if (EndAetherpact)
                return DissolveUnion;
            if (RaidwideSacredSoil())
                return SacredSoil.Retarget(ReplacedActionsList.ToArray(), SimpleTarget.Self);
            if (RaidwideExpedient())
                return Expedient;
            if (RaidwideSuccor())
                return RaidwideRecitation() ? Recitation : OriginalHook(Succor);
            #endregion

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_Aetherflow) && !WasLastAction(Dissipation) && ActionReady(Aetherflow) && !HasAetherflow && CanWeave())
                return Aetherflow;

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_BanefulImpact) && HasStatusEffect(Buffs.ImpactImminent) && !JustUsed(ChainStratagem) && CanWeave())
                return BanefulImpaction;

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_ChainStrat) && ActionWatching.NumberOfGcdsUsed > 3 && CanChainStrategem &&
                GetTargetHPPercent() > chainThreshold && CanWeave() &&
                (LevelChecked(BanefulImpaction) || !SCH_AoE_DPS_ChainStratagemBanefulOption))
                return ChainStratagem;

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_EnergyDrain) && ActionReady(EnergyDrain) &&
                AetherflowCD <= SCH_AoE_DPS_EnergyDrain && CanWeave() &&
                (!SCH_AoE_DPS_EnergyDrain_Burst ||
                 ChainStrategemCD > 10 ||
                 !LevelChecked(ChainStratagem)))
                return EnergyDrain;

            var dotAction = OriginalHook(Bio);
            BioList.TryGetValue(dotAction, out var dotDebuffID);
            var target = SimpleTarget.DottableEnemy(dotAction, dotDebuffID,
                SCH_AoE_ADV_DPS_DoT_HPThreshold,
                SCH_AoE_ADV_DPS_DoT_Reapply,
                SCH_AoE_ADV_DPS_DoT_MaxTargets);

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_DoT) &&
                ActionReady(dotAction) && target != null)
                return OriginalHook(Bio).Retarget([ArtOfWar, ArtOfWarII], target);

            if (IsEnabled(Preset.SCH_AoE_ADV_DPS_Lucid) && Role.CanLucidDream(SCH_AoE_DPS_LucidOption) && CanWeave())
                return Role.LucidDreaming;

            return actionID;
        }
    }

    #endregion
    
    #region Advanced Heals
    internal class SCH_ST_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Physick)
                return actionID;

            #region Variables
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
            #endregion

            #region Priority Cleansing
            
            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);

            if (IsEnabled(Preset.SCH_ST_Heal_Esuna) &&
                ActionReady(Role.Esuna) && cleansableTarget &&
                GetTargetHPPercent(healTarget, SCH_ST_Heal_IncludeShields) >= SCH_ST_Heal_EsunaOption)
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Physick);

            #endregion

            #region Healing Helpers
            if (EndAetherpact)
                return DissolveUnion;
            if (RaidwideSacredSoil())
                return SacredSoil.Retarget(ReplacedActionsList.ToArray(), SimpleTarget.Self);
            if (RaidwideExpedient())
                return Expedient;
            if (RaidwideSuccor())
                return RaidwideRecitation() ? Recitation : OriginalHook(Succor);
            #endregion

            // Aetherflow
            if (IsEnabled(Preset.SCH_ST_Heal_Aetherflow) &&
                ActionReady(Aetherflow) && !HasAetherflow &&
                InCombat() && CanWeave())
                return Aetherflow;

            // Dissipation
            if (IsEnabled(Preset.SCH_ST_Heal_Dissipation)
                && ActionReady(Dissipation)
                && !HasAetherflow
                && InCombat()
                && !FairyBusy)
                return Dissipation;

            // Lucid Dreaming
            if (IsEnabled(Preset.SCH_ST_Heal_Lucid) &&
                Role.CanLucidDream(SCH_ST_Heal_LucidOption))
                return Role.LucidDreaming;

            //Priority List
            for (int i = 0; i < SCH_ST_Heals_Priority.Count; i++)
            {
                int index = SCH_ST_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                {
                    if (SCH_ST_Heal_AldoquimOpts[2] && ActionReady(OriginalHook(EmergencyTactics)) &&
                        spell is Adloquium or Manifestation &&
                        GetTargetHPPercent(healTarget, SCH_ST_Heal_IncludeShields) <=
                        SCH_ST_Heal_AdloquiumOption_Emergency)
                        return OriginalHook(EmergencyTactics);

                    if (GetTargetHPPercent(healTarget, SCH_ST_Heal_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell.RetargetIfEnabled(OptionalTarget, Physick);
                }
            }
            return actionID
                .RetargetIfEnabled(OptionalTarget, Physick);
        }
    }
    
    internal class SCH_AoE_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Succor or Concitation or Accession))
                return actionID;

            #region Healing Helpers
            if (EndAetherpact)
                return DissolveUnion;
            if (RaidwideSacredSoil())
                return SacredSoil.Retarget(ReplacedActionsList.ToArray(), SimpleTarget.Self);
            if (RaidwideExpedient())
                return Expedient;
            if (RaidwideSuccor())
                return RaidwideRecitation() ? Recitation : OriginalHook(Succor);
            #endregion

            if (!HasAetherflow && InCombat())
            {
                if (IsEnabled(Preset.SCH_AoE_Heal_Aetherflow) && ActionReady(Aetherflow) &&
                    (!SCH_AoE_Heal_Aetherflow_Indomitability || GetCooldownRemainingTime(Indomitability) <= 1))
                    return Aetherflow;

                if (IsEnabled(Preset.SCH_AoE_Heal_Dissipation) && ActionReady(Dissipation) && !FairyBusy &&
                    (!SCH_AoE_Heal_Dissipation_Indomitability || GetCooldownRemainingTime(Indomitability) <= 1))
                    return Dissipation;
            }
            if (IsEnabled(Preset.SCH_AoE_Heal_Lucid) && Role.CanLucidDream(SCH_AoE_Heal_LucidOption))
                return Role.LucidDreaming;

            //Priority List
            float averagePartyHP = GetPartyAvgHPPercent();
            for (int i = 0; i < SCH_AoE_Heals_Priority.Count; i++)
            {
                int index = SCH_AoE_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);
                bool onIdom = SCH_AoE_Heal_Indomitability_Recitation && spell is Indomitability;
                bool onSuccor = SCH_AoE_Heal_Succor_Options[1] && spell is Succor or Concitation or Accession;

                if (enabled && averagePartyHP <= config && ActionReady(spell))
                    return ActionReady(Recitation) && (onIdom || onSuccor) ?
                       Recitation :
                       spell;
            }

            if (SCH_AoE_Heal_Succor_Options[0] && ActionReady(EmergencyTactics))
                return OriginalHook(EmergencyTactics);

            return !LevelChecked(Succor) ?
                WhisperingDawn :
                actionID;
        }
    }

    #endregion

    #region Standalone Features

    #region Fey Blessing to Consolation
    internal class SCH_Consolation : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Consolation;

        protected override uint Invoke(uint actionID)
            => actionID is FeyBlessing && LevelChecked(SummonSeraph) && Gauge.SeraphTimer > 0 ? Consolation : actionID;
    }
    #endregion

    #region Lustrate to Excogitation
    internal class SCH_Lustrate : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Lustrate;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Lustrate)
                return actionID;

            IGameObject? healStack = SimpleTarget.Stack.AllyToHeal;

            if (ActionReady(Excogitation))
                return IsEnabled(Preset.SCH_Retarget_Excogitation)
                    ? Excogitation.Retarget(Lustrate, healStack, true)
                    : Excogitation;

            return IsEnabled(Preset.SCH_Retarget_Lustrate)
                ? Lustrate.Retarget(healStack, true)
                : Lustrate;
        }
    }
    #endregion

    #region Recitation to Selected Option
    internal class SCH_Recitation : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Recitation;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Recitation)
                return actionID;

            IGameObject? healStack = SimpleTarget.Stack.AllyToHeal;

            if (ActionReady(Recitation))
                return Recitation;

            if (!HasStatusEffect(Buffs.Recitation) || !ActionReady(Recitation))
            {
                if (SCH_Recitation_Mode == 1 && ActionReady(OriginalHook(Succor)))
                    return OriginalHook(Succor);
                if (SCH_Recitation_Mode == 2 && ActionReady(OriginalHook(Indomitability)))
                    return OriginalHook(Indomitability);
                if (SCH_Recitation_Mode == 3)
                    return IsEnabled(Preset.SCH_Retarget_Excogitation) && ActionReady(OriginalHook(Excogitation))
                        ? Excogitation.Retarget(Recitation, healStack, true)
                        : Excogitation;
                if (SCH_Recitation_Mode == 0)
                    return IsEnabled(Preset.SCH_Retarget_Adloquium)
                        ? OriginalHook(Adloquium).Retarget(Recitation, healStack, true)
                        : OriginalHook(Adloquium);
            }
            return actionID;
        }
    }
    #endregion

    #region Aetherflow Reminder
    internal class SCH_Aetherflow : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Aetherflow;

        protected override uint Invoke(uint actionID)
        {
            if (!AetherflowList.Contains(actionID) || !LevelChecked(Aetherflow))
                return actionID;

            bool hasAetherFlows = HasAetherflow; //False if Zero stacks

            if (IsEnabled(Preset.SCH_Aetherflow_Recite) &&
                LevelChecked(Recitation) &&
                (IsOffCooldown(Recitation) || HasStatusEffect(Buffs.Recitation)))
            {
                //Recitation Indominability and Excogitation, with optional check against AF zero stack count
                bool alwaysShowReciteExcog = SCH_Aetherflow_Recite_ExcogMode == 1;

                if (SCH_Aetherflow_Recite_Excog &&
                    (alwaysShowReciteExcog ||
                     !alwaysShowReciteExcog && !hasAetherFlows) && actionID is Excogitation)
                {
                    //Do not merge this nested if with above. Won't procede with next set
                    return HasStatusEffect(Buffs.Recitation) && IsOffCooldown(Excogitation)
                        ? Excogitation
                        : Recitation;
                }

                bool alwaysShowReciteIndom = SCH_Aetherflow_Recite_IndomMode == 1;

                if (SCH_Aetherflow_Recite_Indom &&
                    (alwaysShowReciteIndom ||
                     !alwaysShowReciteIndom && !hasAetherFlows) && actionID is Indomitability)
                {
                    //Same as above, do not nest with above. It won't procede with the next set
                    return HasStatusEffect(Buffs.Recitation) && IsOffCooldown(Excogitation)
                        ? Indomitability
                        : Recitation;
                }
            }
            if (!hasAetherFlows)
            {
                bool showAetherflowOnAll = SCH_Aetherflow_Display == 1;

                if ((actionID is EnergyDrain && !showAetherflowOnAll || showAetherflowOnAll) &&
                    IsOffCooldown(actionID))
                {
                    if (IsEnabled(Preset.SCH_Aetherflow_Dissipation) &&
                        ActionReady(Dissipation) && IsOnCooldown(Aetherflow) && HasPetPresent())
                        //Dissipation requires fairy, can't seem to make it replace dissipation with fairy summon feature *shrug*
                        return Dissipation;

                    return Aetherflow;
                }
            }
            return actionID;
        }
    }
    #endregion

    #region Swiftcast to Raise
    internal class SCH_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(Preset.SCH_Raise_Retarget)
                    ? Resurrection.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Resurrection
                : actionID;
    }
    #endregion

    #region Fairy Reminder
    internal class SCH_FairyReminder : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_FairyReminder;

        protected override uint Invoke(uint actionID)
            => FairyList.Contains(actionID) && NeedToSummon ? SummonEos : actionID;
    }
    #endregion

    #region Deployment Tactics to Adloquium
    internal class SCH_DeploymentTactics : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_DeploymentTactics;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DeploymentTactics)
                return actionID;

            //Grab our target
            IGameObject? healStack = SimpleTarget.Stack.AllyToHeal;

            //Check for the Galvanize shield buff. Start applying if it doesn't exist
            if (!HasStatusEffect(Buffs.Galvanize, healStack))
            {
                if (IsEnabled(Preset.SCH_DeploymentTactics_Recitation) && ActionReady(Recitation))
                    return Recitation;

                return IsEnabled(Preset.SCH_Retarget_Adloquium)
                    ? OriginalHook(Adloquium).Retarget(DeploymentTactics, healStack, true)
                    : OriginalHook(Adloquium);
            }
            return IsEnabled(Preset.SCH_Retarget_DeploymentTactics)
                ? DeploymentTactics.Retarget(healStack, true)
                : actionID;
        }
    }
    #endregion

    #region Whispering Dawn to Fairy Abilities
    internal class SCH_Fairy_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Fairy_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not WhisperingDawn)
                return actionID;

            if (HasPetPresent())
            {
                // FeyIllumination
                if (ActionReady(FeyIllumination))
                    return OriginalHook(FeyIllumination);

                // FeyBlessing
                if (ActionReady(FeyBlessing) && !(Gauge.SeraphTimer > 0))
                    return OriginalHook(FeyBlessing);

                if (IsEnabled(Preset.SCH_Fairy_Combo_Consolation) && ActionReady(WhisperingDawn))
                    return OriginalHook(actionID);

                if (IsEnabled(Preset.SCH_Fairy_Combo_Consolation) && Gauge.SeraphTimer > 0 && GetRemainingCharges(Consolation) > 0)
                    return OriginalHook(Consolation);
            }
            return actionID;
        }
    }
    #endregion

    internal class SCH_Mit_ST : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Mit_ST;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Protraction)
                return actionID;

            IGameObject? healStack = SimpleTarget.Stack.AllyToHeal;

            if (ActionReady(Protraction))
                return IsEnabled(Preset.SCH_Retarget_Protraction)
                    ? Protraction.Retarget(healStack, dontCull: true)
                    : actionID;

            if (SCH_Mit_STOptions[0] &&
                ActionReady(Recitation))
                return Recitation;

            if (ActionReady(Adloquium) &&
                !HasStatusEffect(Buffs.Galvanize, healStack))
                return IsEnabled(Preset.SCH_Retarget_Adloquium)
                ? OriginalHook(Adloquium).Retarget(Protraction, healStack, true)
                : OriginalHook(Adloquium);

            if (SCH_Mit_STOptions[1] &&
                ActionReady(DeploymentTactics) &&
                HasStatusEffect(Buffs.Catalyze, healStack))
                return IsEnabled(Preset.SCH_Retarget_DeploymentTactics)
                    ? DeploymentTactics.Retarget(Protraction, healStack, true)
                    : DeploymentTactics;

            if (SCH_Mit_STOptions[2] && ActionReady(Excogitation))
                return IsEnabled(Preset.SCH_Retarget_Excogitation)
                    ? Excogitation.Retarget(Protraction, healStack, true)
                    : Excogitation;

            return actionID;
        }
    }
    internal class SCH_Mit_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Mit_AoE;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SacredSoil)
                return actionID;

            var soilTarget =
                (SCH_Retarget_SacredSoilOptions[0]
                    ? SimpleTarget.HardTarget.IfHostile()
                    : null) ??
                (SCH_Retarget_SacredSoilOptions[1]
                    ? SimpleTarget.HardTarget.IfFriendly()
                    : null) ??
                SimpleTarget.Self;

            if (ActionReady(SacredSoil))
                return IsEnabled(Preset.SCH_Retarget_SacredSoil)
                    ? SacredSoil.Retarget(soilTarget)
                    : actionID;

            if (SCH_Mit_AoEOptions[0] &&
                ActionReady(FeyIllumination) && HasPetPresent() && !FairyBusy)
                return FeyIllumination;

            if (SCH_Mit_AoEOptions[1])
            {
                if (ActionReady(Recitation) && ActionReady(DeploymentTactics))
                    return Recitation;

                if (HasStatusEffect(Buffs.Recitation))
                    return Adloquium.Retarget(SacredSoil, SimpleTarget.Self);

                if (ActionReady(DeploymentTactics) && HasStatusEffect(Buffs.Catalyze))
                    return DeploymentTactics.Retarget(SacredSoil, SimpleTarget.Self);
            }

            if (!HasStatusEffect(Buffs.Galvanize) &&
                !HasStatusEffect(SGE.Buffs.EukrasianPrognosis))
                return OriginalHook(Succor);

            if (SCH_Mit_AoEOptions[2] &&
                ActionReady(Expedient))
                return Expedient;

            if (SCH_Mit_AoEOptions[3] &&
                ActionReady(SummonSeraph) &&
                HasPetPresent() && !FairyBusy)
                return SummonSeraph;

            if (ActionReady(Consolation) &&
                !JustUsed(Consolation))
                return Consolation;

            return actionID;
        }
    }

    #region Retargeting Standalone
    internal class SCH_Retarget : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCH_Retarget;

        protected override uint Invoke(uint actionID)
        {
            if (!EZ.Throttle("SCHRetargetingFeature", TS.FromSeconds(.1)))
                return actionID;

            IGameObject? healStack = SimpleTarget.Stack.AllyToHeal;

            if (IsEnabled(Preset.SCH_Retarget_Adloquium))
                OriginalHook(Adloquium).Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_Physick))
                Physick.Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_Lustrate))
                Lustrate.Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_Excogitation))
                Excogitation.Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_DeploymentTactics))
                DeploymentTactics.Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_Protraction))
                Protraction.Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_Aetherpact))
                Aetherpact.Retarget(healStack, true);

            if (IsEnabled(Preset.SCH_Retarget_SacredSoil))
            {
                var soilTarget =
                    (SCH_Retarget_SacredSoilOptions[0]
                        ? SimpleTarget.HardTarget.IfHostile()
                        : null) ??
                    (SCH_Retarget_SacredSoilOptions[1]
                        ? SimpleTarget.HardTarget.IfFriendly()
                        : null) ??
                    SimpleTarget.Self;
                SacredSoil.Retarget(soilTarget, dontCull: true);
            }
            return actionID;
        }
    }
    #endregion

    #endregion
}
