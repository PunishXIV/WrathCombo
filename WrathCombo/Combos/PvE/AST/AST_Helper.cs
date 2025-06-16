using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Core;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;
using ECommons;
using Lumina.Excel.Sheets;

namespace WrathCombo.Combos.PvE;

internal partial class AST
{
    internal static readonly List<uint>
        MaleficList = [Malefic, Malefic2, Malefic3, Malefic4, FallMalefic],
        GravityList = [Gravity, Gravity2];
    internal static Dictionary<uint, ushort>
        CombustList = new()
        {
            { Combust, Debuffs.Combust },
            { Combust2, Debuffs.Combust2 },
            { Combust3, Debuffs.Combust3 }
        };
    public static ASTOpenerMaxLevel1 Opener1 = new();

    public static ASTGauge Gauge => GetJobGauge<ASTGauge>();
    public static CardType DrawnDPSCard => Gauge.DrawnCards[0];

    public static int SpellsSinceDraw()
    {
        if (ActionWatching.CombatActions.Count == 0)
            return 0;

        uint spellToCheck = Gauge.ActiveDraw == DrawType.Astral ? UmbralDraw : AstralDraw;
        int idx = ActionWatching.CombatActions.LastIndexOf(spellToCheck);
        if (idx == -1)
            idx = 0;

        int ret = 0;
        for (int i = idx; i < ActionWatching.CombatActions.Count; i++)
        {
            if (ActionWatching.GetAttackType(ActionWatching.CombatActions[i]) == ActionWatching.ActionAttackType.Spell)
                ret++;
        }
        return ret;
    }

    public static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    #region Card Targeting

    [ActionRetargeting.TargetResolver]
    public static IGameObject? CardResolver () =>
        CardTarget ?? SimpleTarget.Self;

    internal static IGameObject? CardTarget
    {
        get
        {
            if (Svc.ClientState.LocalPlayer is null ||
                Svc.ClientState.LocalPlayer.ClassJob.RowId != JobID ||
                Svc.Condition[ConditionFlag.BetweenAreas] ||
                Svc.Condition[ConditionFlag.Unconscious] ||
                Gauge.DrawnCards[0] == CardType.None ||
                !LevelChecked(Play1) ||
                !IsInParty())
                return field = null;

            // Check if we have a target overriding any searching
            if (Config.AST_QuickTarget_Override != 0)
            {
                var targetOverride =
                    (int)Config.AST_QuickTarget_Override switch
                    {
                        1 => SimpleTarget.HardTarget,
                        2 => SimpleTarget.UIMouseOverTarget,
                        _ => SimpleTarget.Stack.MouseOver,
                    };
                if (targetOverride is IBattleChara &&
                    !targetOverride.IsDead &&
                    targetOverride.IsFriendly() &&
                    InCardRange(targetOverride) &&
                    targetOverride.IsInParty() &&
                    DamageDownFree(targetOverride) &&
                    SicknessFree(targetOverride))
                    return field = targetOverride;
            }

            var card = Gauge.DrawnCards[0];
            var party = GetPartyMembers(false)
                .Select(member => new { member.BattleChara, member.RealJob })
                .Where(member => !member.BattleChara.IsDead && member.BattleChara.IsNotThePlayer())
                .Where(x => InCardRange(x.BattleChara))
                .Where(x => ExistingCardBuffFree(x.BattleChara))
                .ToList();

            if (party.Count <= 1)
                return field = null;

            if (TryGetBestCardTarget(out var target))
                field = target;

            return field;

            #region Status-checking shortcut methods

            // These are here so I don't have to add a ton of methods to AST,
            // and my `.Where()`s can be short

            bool InCardRange(IGameObject? thisTarget) =>
                thisTarget != null && InActionRange(Balance, thisTarget);

            bool ExistingCardBuffFree(IGameObject? thisTarget) =>
                !HasStatusEffect(Buffs.BalanceBuff, thisTarget, true) &&
                !HasStatusEffect(Buffs.SpearBuff, thisTarget, true);

            bool IsMelee (ClassJob job) =>
                JobIDs.Melee.Contains((byte)job.RowId);

            bool IsRanged(ClassJob job) =>
                JobIDs.Ranged.Contains((byte)job.RowId);

            bool DamageDownFree(IGameObject? thisTarget) =>
                !TargetHasDamageDown(thisTarget);

            bool SicknessFree(IGameObject? thisTarget) =>
                !TargetHasRezWeakness(thisTarget);

            bool BrinkFree(IGameObject? thisTarget) =>
                !TargetHasRezWeakness(thisTarget, false);

            #endregion

            bool TryGetBestCardTarget(out IGameObject? bestTarget, int step = 0)
            {
                bestTarget = null;
                var restrictions = RestrictionSteps[step];
                var filter = party;

                if (restrictions.HasFlag(Restrictions.CardsRole))
                    filter = card switch
                    {
                        CardType.Balance => filter.Where(x => IsMelee(x.RealJob!.Value)).ToList(),
                        CardType.Spear => filter.Where(x => IsRanged(x.RealJob!.Value)).ToList(),
                        _ => filter,
                    };

                if (restrictions.HasFlag(Restrictions.NotDD))
                    filter = filter.Where(x => DamageDownFree(x.BattleChara)).ToList();

                if (restrictions.HasFlag(Restrictions.NotSick))
                    filter = filter.Where(x => SicknessFree(x.BattleChara)).ToList();

                if (restrictions.HasFlag(Restrictions.NotBrink))
                    filter = filter.Where(x => BrinkFree(x.BattleChara)).ToList();

                // Run the next step if no matches were found
                if (filter.Count == 0 &&
                    step < RestrictionSteps.Length - 1)
                    return TryGetBestCardTarget(out bestTarget, step + 1);
                // If it's the last step and there are no matches found, bail
                if (filter.Count == 0)
                    return false;

                // Order by job priority
                filter = filter
                    .OrderBy(x =>
                        _cardPriorities.GetValueOrDefault(
                            (byte)x.RealJob!.Value.RowId, byte.MaxValue))
                    .ThenByDescending(x => x.BattleChara.MaxHp)
                    .ToList();

                bestTarget = filter.First().BattleChara;
                return true;
            }
        }
    }

    #region Static Priority Data

    private static Dictionary<byte, int> _cardPriorities = new()
    {
        { SAM.JobID, 1 },
        { NIN.JobID, 2 },
        { VPR.JobID, 3 },
        { DRG.JobID, 4 },
        { MNK.JobID, 5 },
        { DRK.JobID, 6 },
        { RPR.JobID, 7 },
        { PCT.JobID, 8 },
        { SMN.JobID, 9 },
        { MCH.JobID, 10 },
        { BRD.JobID, 11 },
        { RDM.JobID, 12 },
        { DNC.JobID, 13 },
        { BLM.JobID, 14 }
    };

    private static readonly Restrictions[] RestrictionSteps =
    [
        // Correct DPS role for Card
        Restrictions.CardsRole | Restrictions.NotDD | Restrictions.NotSick, // Ailment-free
        // DPS
        Restrictions.DPS | Restrictions.NotDD | Restrictions.NotSick, // Ailment-free
        // Any Role
        Restrictions.NotDD | Restrictions.NotSick, // Ailment-free
        Restrictions.NotSick, // Sickness-free
        Restrictions.NotBrink, // Sick
        Restrictions.ScrapeTheBottom, // :(
    ];

    [Flags]
    private enum Restrictions
    {
        CardsRole = 1 << 0,
        DPS = 1 << 1,
        NotDD = 1 << 2,
        NotSick = 1 << 3,
        NotBrink = 1 << 4,
        ScrapeTheBottom = 1 << 5,
    }

    #endregion

    #endregion

    internal class ASTOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            EarthlyStar,
            FallMalefic,
            Combust3,
            Lightspeed,
            FallMalefic,
            FallMalefic,
            Divination,
            Balance,
            FallMalefic,
            LordOfCrowns,
            UmbralDraw,
            FallMalefic,
            Spear,
            Oracle,
            FallMalefic,
            FallMalefic,
            FallMalefic,
            FallMalefic,
            FallMalefic,
            Combust3,
            FallMalefic
        ];
        public override int MinOpenerLevel => 92;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.AST_ST_DPS_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetCooldown(EarthlyStar).CooldownElapsed >= 4f)
                return false;

            if (!ActionReady(Lightspeed))
                return false;

            if (!ActionReady(Divination))
                return false;

            if (!ActionReady(Balance))
                return true;

            if (!ActionReady(LordOfCrowns))
                return false;

            if (!ActionReady(UmbralDraw))
                return false;

            return true;
        }
    }

    #region ID's

    internal const byte JobID = 33;

    internal const uint
        //DPS
        Malefic = 3596,
        Malefic2 = 3598,
        Malefic3 = 7442,
        Malefic4 = 16555,
        FallMalefic = 25871,
        Gravity = 3615,
        Gravity2 = 25872,
        Oracle = 37029,
        EarthlyStar = 7439,
        DetonateStar = 8324,

        //Cards
        AstralDraw = 37017,
        UmbralDraw = 37018,
        Play1 = 37019,
        Play2 = 37020,
        Play3 = 37021,
        Arrow = 37024,
        Balance = 37023,
        Bole = 37027,
        Ewer = 37028,
        Spear = 37026,
        Spire = 37025,
        MinorArcana = 37022,
        LordOfCrowns = 7444,
        LadyOfCrown = 7445,

        //Utility
        Divination = 16552,
        Lightspeed = 3606,

        //DoT
        Combust = 3599,
        Combust2 = 3608,
        Combust3 = 16554,

        //Healing
        Benefic = 3594,
        Benefic2 = 3610,
        AspectedBenefic = 3595,
        Helios = 3600,
        AspectedHelios = 3601,
        HeliosConjuction = 37030,
        Ascend = 3603,
        EssentialDignity = 3614,
        CelestialOpposition = 16553,
        CelestialIntersection = 16556,
        Horoscope = 16557,
        HoroscopeHeal = 16558,
        Exaltation = 25873,
        Macrocosmos = 25874,
        Synastry = 3612,
        NeutralSect = 16559,
        SunSign = 37031,
        CollectiveUnconscious = 3613;

    //Action Groups


    internal static class Buffs
    {
        internal const ushort
            AspectedBenefic = 835,
            AspectedHelios = 836,
            HeliosConjunction = 3894,
            Horoscope = 1890,
            HoroscopeHelios = 1891,
            NeutralSect = 1892,
            NeutralSectShield = 1921,
            Divination = 1878,
            LordOfCrownsDrawn = 2054,
            LadyOfCrownsDrawn = 2055,
            GiantDominance = 1248,
            ClarifyingDraw = 2713,
            Macrocosmos = 2718,
            //The "Buff" that shows when you're holding onto the card
            BalanceDrawn = 913,
            BoleDrawn = 914,
            ArrowDrawn = 915,
            SpearDrawn = 916,
            EwerDrawn = 917,
            SpireDrawn = 918,
            //The actual buff that buffs players
            BalanceBuff = 3887,
            BoleBuff = 3890,
            ArrowBuff = 3888,
            SpearBuff = 3889,
            EwerBuff = 3891,
            SpireBuff = 3892,
            Lightspeed = 841,
            SelfSynastry = 845,
            TargetSynastry = 846,
            Divining = 3893,
            EarthlyDominance = 1224;
    }

    internal static class Debuffs
    {
        internal const ushort
            Combust = 838,
            Combust2 = 843,
            Combust3 = 1881;
    }

    //Debuff Pairs of Actions and Debuff

    #endregion
}
