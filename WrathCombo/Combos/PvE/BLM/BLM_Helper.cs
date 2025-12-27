using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.GameHelpers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.BLM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    #region Misc

    private static int MaxPolyglot =>
        TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
        TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;

    private static bool EndOfFirePhase =>
        FirePhase && !ActionReady(Despair) && !ActionReady(FireSpam) && !ActionReady(FlareStar);

    private static bool EndOfIcePhaseAoE =>
        IcePhase && HasMaxUmbralHeartStacks && TraitLevelChecked(Traits.EnhancedAstralFire);

    private static bool CanFlarestar =>
        LevelChecked(FlareStar) && AstralSoulStacks is 6;

    private static IStatus? ThunderDebuffST =>
        GetStatusEffect(ThunderList[OriginalHook(Thunder)],CurrentTarget);

    private static IStatus? ThunderDebuffAoE =>
        GetStatusEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget);

    private static float TimeSinceFirestarterBuff =>
        HasStatusEffect(Buffs.Firestarter) ? GetPartyMembers().First().TimeSinceBuffApplied(Buffs.Firestarter) : 0;

    private static bool HasMaxPolyglotStacks =>
        PolyglotStacks == MaxPolyglot;

    private static uint FireSpam =>
        ActionReady(Fire4)
            ? Fire4
            : Fire;

    private static uint BlizzardSpam =>
        ActionReady(Blizzard4)
            ? Blizzard4
            : Blizzard;

    private static bool HasMaxUmbralHeartStacks =>
        UmbralHearts is 3;

    private static int HPThresholdLeylines =>
        BLM_ST_LeyLinesBossOption == 1 || !InBossEncounter()
            ? BLM_ST_LeyLinesHPOption : 0;

    private static bool HasPolyglotStacks() =>
        PolyglotStacks > 0;

    #endregion

    #region Thunder

    internal static bool CanUseThunder()
    {
        uint dotAction = OriginalHook(Thunder);
        int hpThreshold = IsNotEnabled(Preset.BLM_ST_SimpleMode) ? ComputeHpThreshold() : 0;
        ThunderList.TryGetValue(dotAction, out ushort dotDebuffID);
        int dotRefresh = IsNotEnabled(Preset.BLM_ST_SimpleMode) ? BLM_ST_ThunderRefresh : 5;
        float dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(dotAction) &&
               CanApplyStatus(CurrentTarget, dotDebuffID) &&
               !JustUsedOn(dotAction, CurrentTarget, 5f) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh;
    }

    internal static int ComputeHpThreshold()
    {
        if (InBossEncounter())
            return TargetIsBoss() ? BLM_ST_ThunderBossOption : BLM_ST_ThunderBossAddsOption;

        return BLM_ST_ThunderTrashOption;
    }

    #endregion

    #region Movement Prio

    private static (uint Action, Preset Preset, System.Func<bool> Logic)[]
        PrioritizedMovement =>
    [
        //Triplecast
        (Triplecast, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[0] &&
                  ActionReady(Triplecast) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast) &&
                  !HasStatusEffect(Buffs.LeyLines)),

        // Paradox
        (OriginalHook(Paradox), Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[1] &&
                  ActionReady(Paradox) &&
                  FirePhase && ActiveParadox &&
                  !HasStatusEffect(Buffs.Firestarter) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast)),

        //Swiftcast
        (Role.Swiftcast, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[2] &&
                  ActionReady(Role.Swiftcast) &&
                  !HasStatusEffect(Buffs.Triplecast)),

        //Xeno
        (Xenoglossy, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[3] &&
                  HasPolyglotStacks() &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast)),

        //Scathe
        (Scathe, Preset.BLM_ST_Movement,
            () => BLM_ST_MovementOption[4] &&
                  ActionReady(Scathe) &&
                  !HasStatusEffect(Buffs.Triplecast) &&
                  !HasStatusEffect(Role.Buffs.Swiftcast))
    ];

    private static bool CheckMovementConfigMeetsRequirements
        (int index, out uint action)
    {
        action = PrioritizedMovement[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMovement[index].Logic() &&
               IsEnabled(PrioritizedMovement[index].Preset);
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked &&
            BLM_SelectedOpener == 0)
            return StandardOpener;

        if (FlareOpener.LevelChecked &&
            BLM_SelectedOpener == 1)
            return FlareOpener;

        return WrathOpener.Dummy;
    }

    internal static BLMStandardOpener StandardOpener = new();
    internal static BLMFlareOpener FlareOpener = new();

    internal class BLMStandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Fire3,
            HighThunder,
            Role.Swiftcast,
            Amplifier,
            Fire4,
            LeyLines,
            Fire4,
            Fire4,
            Fire4,
            Fire4,
            Xenoglossy,
            Manafont,
            Fire4,
            FlareStar,
            Fire4,
            Fire4,
            HighThunder,
            Fire4,
            Fire4,
            Fire4,
            Fire4,
            FlareStar,
            Despair,
            Transpose,
            Triplecast,
            Blizzard3,
            Blizzard4,
            Paradox,
            Transpose,
            Paradox,
            Fire3
        ];

        internal override UserData ContentCheckConfig => BLM_Balance_Content;

        public override List<int> DelayedWeaveSteps { get; set; } = [6];

        public override bool HasCooldowns() =>
            MP.Full &&
            IsOffCooldown(Manafont) &&
            GetRemainingCharges(Triplecast) >= 1 &&
            GetRemainingCharges(LeyLines) >= 1 &&
            IsOffCooldown(Role.Swiftcast) &&
            IsOffCooldown(Amplifier);
    }

    internal class BLMFlareOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Fire3,
            HighThunder,
            Role.Swiftcast,
            Amplifier,
            Fire4,
            LeyLines,
            Fire4,
            Xenoglossy,
            Fire4,
            Fire4,
            Despair,
            Manafont,
            Fire4,
            Fire4,
            FlareStar,
            Fire4,
            HighThunder,
            Fire4,
            Fire4,
            Fire4,
            Paradox,
            Triplecast,
            Flare,
            FlareStar,
            Transpose,
            Blizzard3,
            Blizzard4,
            Paradox,
            Transpose,
            Fire3
        ];

        internal override UserData ContentCheckConfig => BLM_Balance_Content;

        public override List<int> DelayedWeaveSteps { get; set; } = [6];

        public override bool HasCooldowns() =>
            MP.Full &&
            IsOffCooldown(Manafont) &&
            GetRemainingCharges(Triplecast) >= 1 &&
            GetRemainingCharges(LeyLines) >= 1 &&
            IsOffCooldown(Role.Swiftcast) &&
            IsOffCooldown(Amplifier);
    }

  #endregion

    #region Gauge

    private static BLMGauge Gauge => GetJobGauge<BLMGauge>();

    private static bool FirePhase => Gauge.InAstralFire;

    private static byte AstralFireStacks => Gauge.AstralFireStacks;

    private static bool IcePhase => Gauge.InUmbralIce;

    private static byte UmbralIceStacks => Gauge.UmbralIceStacks;

    private static byte UmbralHearts => Gauge.UmbralHearts;

    private static bool ActiveParadox => Gauge.IsParadoxActive;

    private static int AstralSoulStacks => Gauge.AstralSoulStacks;

    private static byte PolyglotStacks => Gauge.PolyglotStacks;

    private static int PolyglotTimer => Gauge.EnochianTimer / 1000;

    private static class MP
    {
        private static unsafe uint Max => Player.Character->MaxMana;

        internal static bool Full => Max == Cur;

        internal static unsafe uint Cur => Player.Character->Mana;

        internal static int FireI => GetResourceCost(OriginalHook(Fire));

        internal static int FireAoE => GetResourceCost(OriginalHook(Fire2));
    }

    private static readonly FrozenDictionary<uint, ushort> ThunderList = new Dictionary<uint, ushort>
    {
        { Thunder, Debuffs.Thunder },
        { Thunder2, Debuffs.Thunder2 },
        { Thunder3, Debuffs.Thunder3 },
        { Thunder4, Debuffs.Thunder4 },
        { HighThunder, Debuffs.HighThunder },
        { HighThunder2, Debuffs.HighThunder2 }
    }.ToFrozenDictionary();

    #endregion

    #region ID's

    public const uint
        Fire = 141,
        Blizzard = 142,
        Thunder = 144,
        Fire2 = 147,
        Transpose = 149,
        Fire3 = 152,
        Thunder3 = 153,
        Blizzard3 = 154,
        AetherialManipulation = 155,
        Scathe = 156,
        Manaward = 157,
        Manafont = 158,
        Freeze = 159,
        Flare = 162,
        LeyLines = 3573,
        Blizzard4 = 3576,
        Fire4 = 3577,
        BetweenTheLines = 7419,
        Thunder4 = 7420,
        Triplecast = 7421,
        Foul = 7422,
        Thunder2 = 7447,
        Despair = 16505,
        UmbralSoul = 16506,
        Xenoglossy = 16507,
        Blizzard2 = 25793,
        HighFire2 = 25794,
        HighBlizzard2 = 25795,
        Amplifier = 25796,
        Paradox = 25797,
        HighThunder = 36986,
        HighThunder2 = 36987,
        FlareStar = 36989;

    // Debuff Pairs of Actions and Debuff
    public static class Buffs
    {
        public const ushort
            Firestarter = 165,
            LeyLines = 737,
            CircleOfPower = 738,
            Triplecast = 1211,
            Thunderhead = 3870;
    }

    public static class Debuffs
    {
        public const ushort
            Thunder = 161,
            Thunder2 = 162,
            Thunder3 = 163,
            Thunder4 = 1210,
            HighThunder = 3871,
            HighThunder2 = 3872;
    }

    public static class Traits
    {
        public const uint
            UmbralHeart = 295,
            EnhancedPolyglot = 297,
            AspectMasteryIII = 459,
            EnhancedFoul = 461,
            EnhancedManafont = 463,
            Enochian = 460,
            EnhancedPolyglotII = 615,
            EnhancedAstralFire = 616;
    }

    #endregion
}
