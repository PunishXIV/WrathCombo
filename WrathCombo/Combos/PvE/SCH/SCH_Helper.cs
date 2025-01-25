using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal static partial class SCH
{
    #region ID's

    public const byte ClassID = 26;
    public const byte JobID = 28;

    internal const uint

        // Heals
        Physick = 190,
        Adloquium = 185,
        Succor = 186,
        Lustrate = 189,
        SacredSoil = 188,
        Indomitability = 3583,
        Excogitation = 7434,
        Consolation = 16546,
        Resurrection = 173,
        Protraction = 25867,
        Seraphism = 37014,

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
        EmergencyTactics = 3586;

    //Action Groups
    internal static readonly List<uint>
        BroilList = [Ruin, Broil, Broil2, Broil3, Broil4],
        AetherflowList = [EnergyDrain, Lustrate, SacredSoil, Indomitability, Excogitation],
        FairyList = [WhisperingDawn, FeyBlessing, FeyIllumination, Dissipation, Aetherpact, SummonSeraph];

    internal static class Buffs
    {
        internal const ushort
            Galvanize = 297,
            SacredSoil = 299,
            Dissipation = 791,
            Recitation = 1896,
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
    internal static readonly Dictionary<uint, ushort>
        BioList = new() {
                { Bio, Debuffs.Bio1 },
                { Bio2, Debuffs.Bio2 },
                { Biolysis, Debuffs.Biolysis }
        };

    #endregion

    // Class Gauge
    internal static SCHGauge Gauge => GetJobGauge<SCHGauge>();

    internal static bool HasAetherflow(this SCHGauge gauge) => gauge.Aetherflow > 0;

    internal static SCHOpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    public static int GetMatchingConfigST(int i, out uint action, out bool enabled)
    {
        switch (i)
        {
            case 0:
                action = Lustrate;
                enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Lustrate) && Gauge.HasAetherflow();
                return Config.SCH_ST_Heal_LustrateOption;
            case 1:
                action = Excogitation;
                enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Excogitation) && (Gauge.HasAetherflow() || HasEffect(Buffs.Recitation));
                return Config.SCH_ST_Heal_ExcogitationOption;
            case 2:
                action = Protraction;
                enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Protraction);
                return Config.SCH_ST_Heal_ProtractionOption;
            case 3:
                action = Aetherpact;
                enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Aetherpact) && Gauge.FairyGauge >= Config.SCH_ST_Heal_AetherpactFairyGauge && IsOriginal(Aetherpact);
                return Config.SCH_ST_Heal_AetherpactOption;

        }

        enabled = false;
        action = 0;
        return 0;
    }

    public static bool FairyDismissed => Gauge.DismissedFairy > 0;

    private static DateTime SummonTime
    {
        get
        {
            if (HasPetPresent() || FairyDismissed)
                return field = DateTime.Now.AddSeconds(1);

            return field;
        }
    }

    public static bool NeedToSummon => DateTime.Now > SummonTime && !HasPetPresent() && !FairyDismissed;

    public static IBattleChara? AetherPactTarget => Svc.Objects.Where(x => x is IBattleChara chara && chara.StatusList.Any(y => y.StatusId == 1223 && y.SourceObject.GameObjectId == Svc.Buddies.PetBuddy.ObjectId)).Cast<IBattleChara>().FirstOrDefault();

    public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
    {
        switch (i)
        {
            case 0:
                action = OriginalHook(WhisperingDawn);
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_WhisperingDawn) && HasPetPresent();
                return Config.SCH_AoE_Heal_WhisperingDawnOption;
            case 1:
                action = OriginalHook(FeyIllumination);
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_FeyIllumination) && HasPetPresent();
                return Config.SCH_AoE_Heal_FeyIlluminationOption;
            case 2:
                action = FeyBlessing;
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_FeyBlessing) && HasPetPresent();
                return Config.SCH_AoE_Heal_FeyBlessingOption;
            case 3:
                action = Consolation;
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_Consolation) && Gauge.SeraphTimer > 0;
                return Config.SCH_AoE_Heal_ConsolationOption;
            case 4:
                action = Seraphism;
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_Seraphism) && HasPetPresent();
                return Config.SCH_AoE_Heal_SeraphismOption;
            case 5:
                action = Indomitability;
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_Indomitability) && Gauge.HasAetherflow();
                return Config.SCH_AoE_Heal_IndomitabilityOption;
            case 6:
                action = OriginalHook(Succor);
                enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal) && GetPartyBuffPercent(Buffs.Galvanize) <= Config.SCH_AoE_Heal_SuccorShieldOption;
                return 100; //Don't HP Check
        }

        enabled = false;
        action = 0;
        return 0;
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
            ([3], Aetherflow, () => Config.SCH_ST_DPS_OpenerOption == 1),
            ([13], Dissipation, () => Config.SCH_ST_DPS_OpenerOption == 1),
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.SCH_ST_DPS_OpenerContent;

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
}
