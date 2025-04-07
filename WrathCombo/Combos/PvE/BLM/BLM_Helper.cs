using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    public static readonly Dictionary<uint, ushort>
        ThunderList = new()
        {
            { Thunder, Debuffs.Thunder },
            { Thunder2, Debuffs.Thunder2 },
            { Thunder3, Debuffs.Thunder3 },
            { Thunder4, Debuffs.Thunder4 },
            { HighThunder, Debuffs.HighThunder },
            { HighThunder2, Debuffs.HighThunder2 }
        };

    internal static BLMGauge Gauge = GetJobGauge<BLMGauge>();
    internal static BLMStandardOpener StandardOpener = new();
    internal static BLMFlareOpener FlareOpener = new();

    internal static uint CurMp => GetPartyMembers().First().CurrentMP;

    internal static int MaxPolyglot =>
        TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
        TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;

    internal static bool HasMaxPolyglotStacks => PolyglotStacks == MaxPolyglot;

    internal static bool EndOfFirePhase => Gauge.InAstralFire && !ActionReady(Despair) && !ActionReady(FireSpam) && !ActionReady(FlareStar);
    internal static bool EndOfIcePhase => Gauge.InUmbralIce && CurMp == MP.MaxMP && HasMaxUmbralHeartStacks;
    internal static bool EndOfIcePhaseAoEMaxLevel => Gauge.InUmbralIce && HasMaxUmbralHeartStacks && TraitLevelChecked(Traits.EnhancedAstralFire);

    internal static int PolyglotStacks => Gauge.PolyglotStacks;

    internal static bool FlarestarReady => LevelChecked(FlareStar) && Gauge.AstralSoulStacks == 6;

    internal static int RemainingPolyglotCD => Math.Max(0, (MaxPolyglot - Gauge.PolyglotStacks) * 30000 + (Gauge.EnochianTimer - 30000));

    internal static Status? ThunderDebuffST => FindEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget, LocalPlayer?.GameObjectId);

    internal static Status? ThunderDebuffAoE => FindEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget, LocalPlayer?.GameObjectId);

    internal static uint FireSpam => LevelChecked(Fire4) ? Fire4 : Fire;
    internal static uint BlizzardSpam => LevelChecked(Blizzard4) ? Blizzard4 : Blizzard;

    internal static float TimeSinceFirestarterBuff => HasEffect(Buffs.Firestarter) ? GetPartyMembers().First().TimeSinceBuffApplied(Buffs.Firestarter) : 0;

    internal static bool HasPolyglotStacks() => PolyglotStacks > 0;

    internal static bool HasMaxUmbralHeartStacks => !TraitLevelChecked(Traits.UmbralHeart) || Gauge.UmbralHearts == 3; //Returns true before you can have Umbral Hearts out of design

    internal static WrathOpener Opener()
    {
        if (StandardOpener.LevelChecked && Config.BLM_SelectedOpener == 0)
            return StandardOpener;

        if (FlareOpener.LevelChecked && Config.BLM_SelectedOpener == 1)
            return FlareOpener;

        return WrathOpener.Dummy;
    }

    internal static bool DoubleBlizz()
    {
        List<uint> spells = ActionWatching.CombatActions.Where(x =>
            ActionWatching.GetAttackType(x) == ActionWatching.ActionAttackType.Spell &&
            x != OriginalHook(Thunder) && x != OriginalHook(Thunder2)).ToList();

        if (spells.Count < 1)
            return false;

        uint firstSpell = spells[^1];

        switch (firstSpell)
        {
            case Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2:
            {
                uint castedSpell = LocalPlayer.CastActionId;

                if (castedSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2)
                    return true;

                if (spells.Count >= 2)
                {
                    uint secondSpell = spells[^2];

                    switch (secondSpell)
                    {
                        case Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2:
                            return true;
                    }
                }

                break;
            }
        }

        return false;
    }

    #region Openers

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

        internal override UserData ContentCheckConfig => Config.BLM_Balance_Content;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            6
        ];

        public override bool HasCooldowns() =>
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

        internal override UserData ContentCheckConfig => Config.BLM_Balance_Content;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            6
        ];

        public override bool HasCooldowns() =>
            IsOffCooldown(Manafont) &&
            GetRemainingCharges(Triplecast) >= 1 &&
            GetRemainingCharges(LeyLines) >= 1 &&
            IsOffCooldown(Role.Swiftcast) &&
            IsOffCooldown(Amplifier);
    }

  #endregion

    #region ID's

    public const byte ClassID = 7;
    public const byte JobID = 25;

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
            Thundercloud = 164,
            Firestarter = 165,
            LeyLines = 737,
            CircleOfPower = 738,
            Sharpcast = 867,
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

    internal static class MP
    {
        internal const int MaxMP = 10000;

        internal const int AllMPSpells = 800; //"ALL MP" spell. Only caring about the absolute minimum.

        internal static int FireI => GetResourceCost(OriginalHook(Fire));

        internal static int FlareAoE => GetResourceCost(OriginalHook(Flare));

        internal static int FireAoE => GetResourceCost(OriginalHook(Fire2));

        internal static int FireIII => GetResourceCost(OriginalHook(Fire3));

        internal static int BlizzardAoE => GetResourceCost(OriginalHook(Blizzard2));

        internal static int BlizzardI => GetResourceCost(OriginalHook(Blizzard));

        internal static int Freeze => GetResourceCost(OriginalHook(BLM.Freeze));

        internal static int Despair => GetResourceCost(OriginalHook(BLM.Despair));
    }

    #endregion
}
