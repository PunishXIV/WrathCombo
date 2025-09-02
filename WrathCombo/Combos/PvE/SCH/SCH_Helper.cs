#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.SCH.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;
internal partial class SCH
{
    #region Lists
    internal static readonly FrozenDictionary<uint, ushort> BioList = new Dictionary<uint, ushort>
    {
        { Bio, Debuffs.Bio1 },
        { Bio2, Debuffs.Bio2},
        { Biolysis, Debuffs.Biolysis},
    }.ToFrozenDictionary();

    internal static readonly List<uint>
        BroilList = [Ruin, Broil, Broil2, Broil3, Broil4],
        AetherflowList = [EnergyDrain, Lustrate, SacredSoil, Indomitability, Excogitation],
        ReplacedActionsList = [Ruin, Broil, Broil2, Broil3, Broil4, Bio, Biolysis, Bio2, Ruin2, Succor, Concitation, Accession, Physick], //Used for Hidden Features Retarget Sacred Soil
        FairyList = [WhisperingDawn, FeyBlessing, FeyIllumination, Dissipation, Aetherpact, SummonSeraph, Seraphism];
    #endregion
    internal static SCHGauge Gauge => GetJobGauge<SCHGauge>();
    internal static IBattleChara? AetherPactTarget => Svc.Objects.Where(x => x is IBattleChara chara && chara.StatusList.Any(y => y.StatusId == 1223 && y.SourceObject.GameObjectId == Svc.Buddies.PetBuddy.ObjectId)).Cast<IBattleChara>().FirstOrDefault();
    internal static bool HasAetherflow => Gauge.Aetherflow > 0;
    internal static bool FairyDismissed => Gauge.DismissedFairy > 0;
    internal static bool FairyBusy => JustUsed(WhisperingDawn, 2f) || JustUsed(FeyIllumination, 2f) || JustUsed(FeyBlessing, 2f) || 
                                      JustUsed(Aetherpact, 2f) || JustUsed(Dissipation, 2f) || JustUsed(Consolation, 2f) || JustUsed(SummonSeraph, 2f);
    internal static bool EndAetherpact => OriginalHook(Aetherpact) is DissolveUnion &&  //Quick check to see if Fairy Aetherpact is Active
                                          AetherPactTarget is not null && //Null checking so GetTargetHPPercent doesn't fall back to CurrentTarget
                                          (IsEnabled(Preset.SCH_ST_Heal_Aetherpact) && IsEnabled(Preset.SCH_ST_Heal) && GetTargetHPPercent(AetherPactTarget) >= SCH_ST_Heal_AetherpactDissolveOption ||
                                           IsEnabled(Preset.SCH_Simple_ST_Heal) && GetTargetHPPercent(AetherPactTarget) >= 95) ;
                                            
                                            
    internal static bool ShieldCheck => GetPartyBuffPercent(Buffs.Galvanize) <= SCH_AoE_Heal_SuccorShieldOption &&
                                        GetPartyBuffPercent(SGE.Buffs.EukrasianPrognosis) <= SCH_AoE_Heal_SuccorShieldOption;
    internal static bool CanChainStrategem => ActionReady(ChainStratagem) &&
                                              CanApplyStatus(CurrentTarget, Debuffs.ChainStratagem) &&
                                              !HasStatusEffect(Debuffs.ChainStratagem, CurrentTarget, true);

    internal static float AetherflowCD => GetCooldownRemainingTime(Aetherflow);
    
    internal static float ChainStrategemCD => GetCooldownRemainingTime(ChainStratagem);
    
    #region Raidwides
    
    internal static bool RaidwideSacredSoil()
    {
        return IsEnabled(Preset.SCH_Raidwide_SacredSoil) && ActionReady(SacredSoil) && CanWeave() && RaidWideCasting();
    }
    internal static bool RaidwideExpedient()
    {
        return IsEnabled(Preset.SCH_Raidwide_Expedient) && ActionReady(Expedient) && CanWeave() && RaidWideCasting();
    }
    internal static bool RaidwideSuccor()
    {
        return IsEnabled(Preset.SCH_Raidwide_Succor) && ActionReady(OriginalHook(Succor)) && ShieldCheck && RaidWideCasting();
    }
    internal static bool RaidwideRecitation()
    {
        return SCH_Raidwide_Succor_Recitation&& ActionReady(Recitation);
    }
    #endregion
    
    #region Eos Summoner
    public static bool NeedToSummon => DateTime.Now > SummonTime && !HasPetPresent() && !FairyDismissed;
    private static DateTime SummonTime
    {
        get
        {
            if (HasPetPresent() || FairyDismissed)
                return field = DateTime.Now.AddSeconds(1);

            return field;
        }
    }
    
    #endregion
    
    #region Dot Checker
    internal static bool NeedsDoT()
    {
        var dotAction = OriginalHook(Bio);
        var hpThreshold = IsNotEnabled(Preset.SCH_ST_Simple_DPS) &&
            (SCH_DPS_BioSubOption == 1 || !InBossEncounter())? SCH_DPS_BioOption : 0;
        BioList.TryGetValue(dotAction, out var dotDebuffID);
        var dotRefresh = IsNotEnabled(Preset.SCH_ST_Simple_DPS) ? SCH_DPS_BioUptime_Threshold : 2.5;
        var dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(dotAction) &&
               CanApplyStatus(CurrentTarget, dotDebuffID) &&
               !JustUsedOn(dotAction, CurrentTarget, 5f) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh;
    }
    #endregion
    
    #region Get ST Heals
    internal static int GetMatchingConfigST(int i, IGameObject? OptionalTarget, out uint action, out bool enabled)
    {
        IGameObject? healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
        bool ShieldCheck = !SCH_ST_Heal_AldoquimOpts[0] || 
                           !HasStatusEffect(Buffs.Galvanize, healTarget, true) || 
                           HasStatusEffect(Buffs.EmergencyTactics);
        bool SageShieldCheck = !SCH_ST_Heal_AldoquimOpts[1] ||
                               !HasStatusEffect(SGE.Buffs.EukrasianDiagnosis, healTarget, true) || 
                               !HasStatusEffect(SGE.Buffs.EukrasianPrognosis, healTarget, true) ||
                               HasStatusEffect(Buffs.EmergencyTactics);
        bool EmergencyAdlo = SCH_ST_Heal_AldoquimOpts[2] && ActionReady(EmergencyTactics) &&
                             GetTargetHPPercent(healTarget, SCH_ST_Heal_IncludeShields) <=
                             SCH_ST_Heal_AdloquiumOption_Emergency;
        
        switch (i)
        {
            case 0:
                action = Lustrate;
                enabled = IsEnabled(Preset.SCH_ST_Heal_Lustrate) && HasAetherflow;
                return SCH_ST_Heal_LustrateOption;
            case 1:
                action = Excogitation;
                enabled = IsEnabled(Preset.SCH_ST_Heal_Excogitation) && (HasAetherflow || HasStatusEffect(Buffs.Recitation)) &&
                          (!SCH_ST_Heal_ExcogitationBossOption || !InBossEncounter());;
                return SCH_ST_Heal_ExcogitationOption;
            case 2:
                action = Protraction;
                enabled = IsEnabled(Preset.SCH_ST_Heal_Protraction) &&
                          (!SCH_ST_Heal_ProtractionBossOption || !InBossEncounter());
                return SCH_ST_Heal_ProtractionOption;
            case 3:
                action = Aetherpact;
                enabled = IsEnabled(Preset.SCH_ST_Heal_Aetherpact) && Gauge.FairyGauge >= SCH_ST_Heal_AetherpactFairyGauge && IsOriginal(Aetherpact) && !FairyBusy;
                return SCH_ST_Heal_AetherpactOption;
            case 4:
                action = OriginalHook(Adloquium);
                enabled = IsEnabled(Preset.SCH_ST_Heal_Adloquium) &&
                          ActionReady(OriginalHook(Adloquium)) &&
                          GetTargetHPPercent(healTarget, SCH_ST_Heal_IncludeShields) <= SCH_ST_Heal_AdloquiumOption &&
                          (EmergencyAdlo || ShieldCheck && SageShieldCheck);
                return SCH_ST_Heal_AdloquiumOption;
            case 5:
                action = OriginalHook(WhisperingDawn);
                enabled = IsEnabled(Preset.SCH_ST_Heal_WhisperingDawn) && HasPetPresent() && !FairyBusy &&
                          (!SCH_ST_Heal_WhisperingDawnBossOption || !InBossEncounter());
                return SCH_ST_Heal_WhisperingDawnOption;
            case 6:
                action = OriginalHook(FeyIllumination);
                enabled = IsEnabled(Preset.SCH_ST_Heal_FeyIllumination) && HasPetPresent() && !FairyBusy &&
                          (!SCH_ST_Heal_FeyIlluminationBossOption || !InBossEncounter());
                return SCH_ST_Heal_FeyIlluminationOption;
            case 7:
                action = FeyBlessing;
                enabled = IsEnabled(Preset.SCH_ST_Heal_FeyBlessing) && HasPetPresent() && !FairyBusy &&
                          (!SCH_ST_Heal_FeyBlessingBossOption || !InBossEncounter());
                return SCH_ST_Heal_FeyBlessingOption;
        }

        enabled = false;
        action = 0;
        return 0;
    }
    #endregion
    
    #region Get Aoe Heals
    public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
    {
        switch (i)
        {
            case 0:
                action = OriginalHook(WhisperingDawn);
                enabled = IsEnabled(Preset.SCH_AoE_Heal_WhisperingDawn) && HasPetPresent() && !FairyBusy;
                return SCH_AoE_Heal_WhisperingDawnOption;
            case 1:
                action = OriginalHook(FeyIllumination);
                enabled = IsEnabled(Preset.SCH_AoE_Heal_FeyIllumination) && HasPetPresent() && !FairyBusy;
                return SCH_AoE_Heal_FeyIlluminationOption;
            case 2:
                action = FeyBlessing;
                enabled = IsEnabled(Preset.SCH_AoE_Heal_FeyBlessing) && HasPetPresent() && !FairyBusy;
                return SCH_AoE_Heal_FeyBlessingOption;
            case 3:
                action = Consolation;
                enabled = IsEnabled(Preset.SCH_AoE_Heal_Consolation) && Gauge.SeraphTimer > 0 && !FairyBusy;
                return SCH_AoE_Heal_ConsolationOption;
            case 4:
                action = Seraphism;
                enabled = IsEnabled(Preset.SCH_AoE_Heal_Seraphism) && HasPetPresent();
                return SCH_AoE_Heal_SeraphismOption;
            case 5:
                action = Indomitability;
                enabled = IsEnabled(Preset.SCH_AoE_Heal_Indomitability) && HasAetherflow;
                return SCH_AoE_Heal_IndomitabilityOption;
            case 6:
                action = SummonSeraph;
                enabled = IsEnabled(Preset.SCH_AoE_Heal_SummonSeraph) && HasPetPresent() && !FairyBusy;
                return SCH_AoE_Heal_SummonSeraph;
            
            case 7:
                action = OriginalHook(Succor);
                enabled = IsEnabled(Preset.SCH_AoE_Heal) && ShieldCheck && LevelChecked(Succor);
                return 100; //Don't HP Check
        }

        enabled = false;
        action = 0;
        return 0;
    }
    #endregion
    
    #region Openers
    
    internal static SCHOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class SCHOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Broil4,
            Biolysis,
            Dissipation,
            Broil4,
            ChainStratagem,
            Broil4,
            EnergyDrain,
            Broil4,
            EnergyDrain,
            Broil4,
            EnergyDrain,
            Broil4,
            Aetherflow,
            Broil4,
            BanefulImpaction,
            Broil4,
            EnergyDrain,
            Broil4,
            EnergyDrain,
            Broil4,
            EnergyDrain,
            Biolysis
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([3], Aetherflow, () => SCH_ST_DPS_OpenerOption == 1),
            ([13], Dissipation, () => SCH_ST_DPS_OpenerOption == 1)
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData ContentCheckConfig => SCH_ST_DPS_OpenerContent;

        public override bool HasCooldowns()
        {
            if (!HasPetPresent())
                return false;

            if (!IsOffCooldown(ChainStratagem) ||
                !IsOffCooldown(Dissipation) ||
                !IsOffCooldown(Aetherflow))
                return false;

            return true;
        }
    }
    
    #endregion

    #region ID's

    internal const uint

        // Heals
        Physick = 190,
        Adloquium = 185,
        Succor = 186,
        Concitation = 37013,
        Lustrate = 189,
        SacredSoil = 188,
        Indomitability = 3583,
        Excogitation = 7434,
        Consolation = 16546,
        Resurrection = 173,
        Protraction = 25867,
        Seraphism = 37014,
        Manifestation = 37015,
        Accession = 37016,

        // Offense
        Bio = 17864,
        Bio2 = 17865,
        Biolysis = 16540,
        Ruin = 17869,
        Ruin2 = 17870,
        Broil = 3584,
        Broil2 = 7435,
        Broil3 = 16541,
        Broil4 = 25865,
        EnergyDrain = 167,
        ArtOfWar = 16539,
        ArtOfWarII = 25866,
        BanefulImpaction = 37012,

        // Faerie
        SummonSeraph = 16545,
        SummonEos = 17215,
        WhisperingDawn = 16537,
        FeyIllumination = 16538,
        Dissipation = 3587,
        Aetherpact = 7437,
        DissolveUnion = 7869,
        FeyBlessing = 16543,

        // Other
        Aetherflow = 166,
        Recitation = 16542,
        ChainStratagem = 7436,
        DeploymentTactics = 3585,
        Expedient = 25868,
        EmergencyTactics = 3586;

    //Action Groups


    internal static class Buffs
    {
        internal const ushort
            Galvanize = 297,
            SacredSoil = 299,
            Dissipation = 791,
            EmergencyTactics = 792,
            Excogitation =1220,
            Recitation = 1896,
            SeraphicVeil = 1917,
            Catalyze = 1918,
            ImpactImminent = 3882;
    }

    internal static class Debuffs
    {
        internal const ushort
            Bio1 = 179,
            Bio2 = 189,
            Biolysis = 1895,
            ChainStratagem = 1221;
    }

    //Debuff Pairs of Actions and Debuff

    #endregion
}
