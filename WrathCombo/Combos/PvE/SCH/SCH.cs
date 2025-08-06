using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class SCH : Healer
{
    #region Simple ST DPS
    internal class SCH_ST_Simple_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_ST_Simple_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (!BroilList.Contains(actionID))
                return actionID;

            if (NeedToSummon)
                return SummonEos;
            
            #region Special Content
            if (Variant.CanRampart(CustomComboPreset.SCH_DPS_Variant_Rampart))
                return Variant.Rampart;
            
            if (Variant.CanSpiritDart(CustomComboPreset.SCH_DPS_Variant_SpiritDart))
                return Variant.SpiritDart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
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
    #endregion
    
    #region Advanced ST DPS
    internal class SCH_ST_ADV_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_ST_ADV_DPS;
        protected override uint Invoke(uint actionID)
        {
            bool actionFound = Config.SCH_ST_DPS_Adv_Actions == 0 && BroilList.Contains(actionID) ||
                               Config.SCH_ST_DPS_Adv_Actions == 1 && BioList.ContainsKey(actionID) ||
                               Config.SCH_ST_DPS_Adv_Actions == 2 && actionID is Ruin2;
            
            if (!actionFound)
                return actionID;
            
            #region Variables
            int chainThreshold = Config.SCH_ST_DPS_ChainStratagemSubOption == 1 || !InBossEncounter() ? Config.SCH_ST_DPS_ChainStratagemOption : 0;
            #endregion
            
            if (!actionFound)
                return actionID;

            if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_FairyReminder) && NeedToSummon)
                return SummonEos;
            //Opener
            if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_Balance_Opener) && Opener().FullOpener(ref actionID))
                return actionID;
            
            #region Special Content
            if (Variant.CanRampart(CustomComboPreset.SCH_DPS_Variant_Rampart))
                return Variant.Rampart;
            
            if (Variant.CanSpiritDart(CustomComboPreset.SCH_DPS_Variant_SpiritDart))
                return Variant.SpiritDart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
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
                if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_Aetherflow) && !WasLastAction(Dissipation) && ActionReady(Aetherflow) && !HasAetherflow)
                    return Aetherflow;
                
                if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_BanefulImpact) && HasStatusEffect(Buffs.ImpactImminent) && !JustUsed(ChainStratagem))
                    return BanefulImpaction;
                
                if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_ChainStrat) && ActionWatching.NumberOfGcdsUsed > 3 && CanChainStrategem &&
                    GetTargetHPPercent() > chainThreshold)
                    return ChainStratagem;
                    
                if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_EnergyDrain) && ActionReady(EnergyDrain) && 
                    AetherflowCD <= Config.SCH_ST_DPS_EnergyDrain &&
                    (!Config.SCH_ST_DPS_EnergyDrain_Burst ||
                     ChainStrategemCD > 10 ||
                     !LevelChecked(ChainStratagem)))
                    return EnergyDrain;
                
                if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_Lucid) && Role.CanLucidDream(Config.SCH_ST_DPS_LucidOption))
                    return Role.LucidDreaming;
            }
            
            //Bio/Biolysis
            if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_Bio) && NeedsDoT() && InCombat())
                return OriginalHook(Bio);

            //Ruin 2 Movement
            if (IsEnabled(CustomComboPreset.SCH_ST_ADV_DPS_Ruin2Movement) && ActionReady(Ruin2) && IsMoving() && InCombat())
                return OriginalHook(Ruin2);
            
            return actionID;
        }
    }
    #endregion
    
    #region Simple AoE DPS
    internal class SCH_AoE_Simple_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_AoE_Simple_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ArtOfWar or ArtOfWarII))
                return actionID;

            if (NeedToSummon)
                return SummonEos;
            
            #region Special Content
            if (Variant.CanRampart(CustomComboPreset.SCH_DPS_Variant_Rampart))
                return Variant.Rampart;

            if (Variant.CanSpiritDart(CustomComboPreset.SCH_DPS_Variant_SpiritDart))
                return Variant.SpiritDart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
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
                    
            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_EnergyDrain) && ActionReady(EnergyDrain) && 
                AetherflowCD <= 10 && CanWeave())
                return EnergyDrain;
            
            var dotAction = OriginalHook(Bio);
            BioList.TryGetValue(dotAction, out var dotDebuffID);
            var target =
                SimpleTarget.DottableEnemy(dotAction, dotDebuffID, 30, 3, 4);

            if (ActionReady(dotAction) && target != null)
                return OriginalHook(Bio).Retarget([ArtOfWar, ArtOfWarII], target);
                
            if (Role.CanLucidDream(Config.SCH_AoE_DPS_LucidOption) && CanWeave())
                return Role.LucidDreaming;

            return actionID;
        }
    }

    #endregion
    
    #region Advanced AoE DPS
    internal class SCH_AoE_ADV_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_AoE_ADV_DPS;
        protected override uint Invoke(uint actionID)
        {
            #region Variables
            int chainThreshold = Config.SCH_AoE_DPS_ChainStratagemSubOption == 1 || !InBossEncounter() ? Config.SCH_AoE_DPS_ChainStratagemOption : 0;
            #endregion
            
            if (actionID is not (ArtOfWar or ArtOfWarII))
                return actionID;

            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_FairyReminder) &&
                NeedToSummon)
                return SummonEos;
            
            #region Special Content
            if (Variant.CanRampart(CustomComboPreset.SCH_DPS_Variant_Rampart))
                return Variant.Rampart;

            if (Variant.CanSpiritDart(CustomComboPreset.SCH_DPS_Variant_SpiritDart))
                return Variant.SpiritDart;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();
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
            
            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_Aetherflow) && !WasLastAction(Dissipation) && ActionReady(Aetherflow) && !HasAetherflow && CanWeave())
                return Aetherflow;
                
            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_BanefulImpact) && HasStatusEffect(Buffs.ImpactImminent) && !JustUsed(ChainStratagem) && CanWeave())
                return BanefulImpaction;
                
            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_ChainStrat) && ActionWatching.NumberOfGcdsUsed > 3 && CanChainStrategem && 
                GetTargetHPPercent() > chainThreshold && CanWeave() &&
                (LevelChecked(BanefulImpaction)|| !Config.SCH_AoE_DPS_ChainStratagemBanefulOption))
                return ChainStratagem;
                    
            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_EnergyDrain) && ActionReady(EnergyDrain) && 
                AetherflowCD <= Config.SCH_AoE_DPS_EnergyDrain && CanWeave() &&
                (!Config.SCH_AoE_DPS_EnergyDrain_Burst ||
                 ChainStrategemCD > 10 ||
                 !LevelChecked(ChainStratagem)))
                return EnergyDrain;
            
            var dotAction = OriginalHook(Bio);
            BioList.TryGetValue(dotAction, out var dotDebuffID);
            var target = SimpleTarget.DottableEnemy(dotAction, dotDebuffID,
                Config.SCH_AoE_ADV_DPS_DoT_HPThreshold,
                Config.SCH_AoE_ADV_DPS_DoT_Reapply,
                Config.SCH_AoE_ADV_DPS_DoT_MaxTargets);

            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_DoT) &&
                ActionReady(dotAction) && target != null)
                return OriginalHook(Bio).Retarget([ArtOfWar, ArtOfWarII], target);
                
            if (IsEnabled(CustomComboPreset.SCH_AoE_ADV_DPS_Lucid) && Role.CanLucidDream(Config.SCH_AoE_DPS_LucidOption) && CanWeave())
                return Role.LucidDreaming;

            return actionID;
        }
    }

    #endregion
    
    #region ST Heal
    internal class SCH_ST_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_ST_Heal;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Physick)
                return actionID;
            
            #region Variables
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
            #endregion
            
            #region Priority Cleansing
            
            if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Esuna) && 
                ActionReady(Role.Esuna) && HasCleansableDebuff(healTarget) &&
                GetTargetHPPercent(healTarget, Config.SCH_ST_Heal_IncludeShields) >= Config.SCH_ST_Heal_EsunaOption)
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
            if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Aetherflow) &&
                ActionReady(Aetherflow) && !HasAetherflow &&
                InCombat() && CanWeave())
                return Aetherflow;

            // Dissipation
            if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Dissipation)
                && ActionReady(Dissipation)
                && !HasAetherflow
                && InCombat()
                && !FairyBusy)
                return Dissipation;

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SCH_ST_Heal_Lucid) &&
                Role.CanLucidDream(Config.SCH_ST_Heal_LucidOption))
                return Role.LucidDreaming;

            //Priority List
            for(int i = 0; i < Config.SCH_ST_Heals_Priority.Count; i++)
            {
                int index = Config.SCH_ST_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                {
                    if (Config.SCH_ST_Heal_AldoquimOpts[2] && ActionReady(OriginalHook(EmergencyTactics)) &&
                        spell is Adloquium or Manifestation && 
                        GetTargetHPPercent(healTarget, Config.SCH_ST_Heal_IncludeShields) <=
                        Config.SCH_ST_Heal_AdloquiumOption_Emergency)
                        return OriginalHook(EmergencyTactics);
                    
                    if (GetTargetHPPercent(healTarget, Config.SCH_ST_Heal_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell.RetargetIfEnabled(OptionalTarget, Physick);
                }
            }
            return actionID
                .RetargetIfEnabled(OptionalTarget, Physick);
        }
    }
    #endregion
    
    #region Aoe Heal 
    internal class SCH_AoE_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_AoE_Heal;
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
                if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Aetherflow) && ActionReady(Aetherflow) && 
                    (!Config.SCH_AoE_Heal_Aetherflow_Indomitability || GetCooldownRemainingTime(Indomitability) <= 1))
                    return Aetherflow;

                if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Dissipation) && ActionReady(Dissipation) && !FairyBusy &&
                    (!Config.SCH_AoE_Heal_Dissipation_Indomitability || GetCooldownRemainingTime(Indomitability) <= 1))
                    return Dissipation;
            }
            if (IsEnabled(CustomComboPreset.SCH_AoE_Heal_Lucid) && Role.CanLucidDream(Config.SCH_AoE_Heal_LucidOption))
                return Role.LucidDreaming;

            //Priority List
            float averagePartyHP = GetPartyAvgHPPercent();
            for(int i = 0; i < Config.SCH_AoE_Heals_Priority.Count; i++)
            {
                int index = Config.SCH_AoE_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);
                bool onIdom = Config.SCH_AoE_Heal_Indomitability_Recitation && spell is Indomitability;
                bool onSuccor = Config.SCH_AoE_Heal_Succor_Options[1] && spell is Succor or Concitation or Accession;

                if (enabled && averagePartyHP <= config && ActionReady(spell))
                     return ActionReady(Recitation) && (onIdom || onSuccor) ? 
                        Recitation :
                        spell;
            }

            if (Config.SCH_AoE_Heal_Succor_Options[0] && ActionReady(EmergencyTactics))
                return OriginalHook(EmergencyTactics);
            
            return !LevelChecked(Succor)?
                WhisperingDawn:
                actionID;
        }
    }
    
    #endregion
    
    #region Standalone Features
    
    #region Sacred Soil Retarget
    internal class SCH_SacredSoil : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_SacredSoil;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SacredSoil)
                return actionID;
            
            var sacredSoilTarget =
                    (IsEnabled(CustomComboPreset.SCH_SacredSoil_Enemy)
                        ? SimpleTarget.HardTarget
                        : null) ??
                    (IsEnabled(CustomComboPreset.SCH_SacredSoil_Allies)
                        ? SimpleTarget.Stack.OverridesAllies
                        : null) ??
                    SimpleTarget.Self;
                
            return IsEnabled(CustomComboPreset.SCH_SacredSoil)? SacredSoil.Retarget(sacredSoilTarget) : actionID;
        }
    }
    #endregion
    
    #region Fey Blessing to Consolation
    internal class SCH_Consolation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Consolation;
        protected override uint Invoke(uint actionID)
            => actionID is FeyBlessing && LevelChecked(SummonSeraph) && Gauge.SeraphTimer > 0 ? Consolation : actionID;
    }
    #endregion
    
    #region Lustrate to Excogitation
    internal class SCH_Lustrate : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Lustrate;
        protected override uint Invoke(uint actionID) =>
            actionID is Lustrate &&
            LevelChecked(Excogitation) && IsOffCooldown(Excogitation)
                ? Excogitation
                : actionID;
    }
    #endregion
    
    #region Recitation to Selected Option
    internal class SCH_Recitation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Recitation;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Recitation || !HasStatusEffect(Buffs.Recitation))
                return actionID;

            return Config.SCH_Recitation_Mode.Value switch
            {
                0 => OriginalHook(Adloquium),
                1 => OriginalHook(Succor),
                2 => OriginalHook(Indomitability),
                3 => OriginalHook(Excogitation),
                _ => actionID,
            };
        }
    }
    #endregion

    #region Aetherflow Reminder
    internal class SCH_Aetherflow : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Aetherflow;
        protected override uint Invoke(uint actionID)
        {
            if (!AetherflowList.Contains(actionID) || !LevelChecked(Aetherflow))
                return actionID;

            bool hasAetherFlows = HasAetherflow; //False if Zero stacks

            if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Recite) &&
                LevelChecked(Recitation) &&
                (IsOffCooldown(Recitation) || HasStatusEffect(Buffs.Recitation)))
            {
                //Recitation Indominability and Excogitation, with optional check against AF zero stack count
                bool alwaysShowReciteExcog = Config.SCH_Aetherflow_Recite_ExcogMode == 1;

                if (Config.SCH_Aetherflow_Recite_Excog &&
                    (alwaysShowReciteExcog ||
                     !alwaysShowReciteExcog && !hasAetherFlows) && actionID is Excogitation)
                {
                    //Do not merge this nested if with above. Won't procede with next set
                    return HasStatusEffect(Buffs.Recitation) && IsOffCooldown(Excogitation)
                        ? Excogitation
                        : Recitation;
                }

                bool alwaysShowReciteIndom = Config.SCH_Aetherflow_Recite_IndomMode == 1;

                if (Config.SCH_Aetherflow_Recite_Indom &&
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
                bool showAetherflowOnAll = Config.SCH_Aetherflow_Display == 1;

                if ((actionID is EnergyDrain && !showAetherflowOnAll || showAetherflowOnAll) &&
                    IsOffCooldown(actionID))
                {
                    if (IsEnabled(CustomComboPreset.SCH_Aetherflow_Dissipation) &&
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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Raise;
        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(CustomComboPreset.SCH_Raise_Retarget)
                    ? Resurrection
                        .Retarget(Role.Swiftcast,
                            SimpleTarget.Stack.AllyToRaise)
                        .AndRunMacro()
                    : Resurrection.AndRunMacro()
                : actionID;
    }
    #endregion
    
    #region Fairy Reminder
    internal class SCH_FairyReminder : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_FairyReminder;
        protected override uint Invoke(uint actionID)
            => FairyList.Contains(actionID) && NeedToSummon ? SummonEos : actionID;
    }
    #endregion
    
    #region Deployment Tactics to Adloquium
    internal class SCH_DeploymentTactics : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_DeploymentTactics;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DeploymentTactics || !ActionReady(DeploymentTactics))
                return actionID;

            //Grab our target
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            //Check for the Galvanize shield buff. Start applying if it doesn't exist
            if (!HasStatusEffect(Buffs.Galvanize, healTarget))
            {
                if (IsEnabled(CustomComboPreset.SCH_DeploymentTactics_Recitation) && ActionReady(Recitation))
                    return Recitation;

                return OriginalHook(Adloquium);
            }
            return actionID;
        }
    }
    #endregion
    
    #region Whispering Dawn to Fairy Abilities
    internal class SCH_Fairy_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCH_Fairy_Combo;
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

                if (IsEnabled(CustomComboPreset.SCH_Fairy_Combo_Consolation) && ActionReady(WhisperingDawn))
                    return OriginalHook(actionID);

                if (IsEnabled(CustomComboPreset.SCH_Fairy_Combo_Consolation) && Gauge.SeraphTimer > 0 && GetRemainingCharges(Consolation) > 0)
                    return OriginalHook(Consolation);
            }
            return actionID;
        }
    }
    #endregion
    
    #endregion
}
