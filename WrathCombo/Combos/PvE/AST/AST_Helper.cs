using System;
using System.Collections.Frozen;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Core;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Lumina.Excel.Sheets;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace WrathCombo.Combos.PvE;

internal partial class AST
{
    internal static readonly List<uint>
        MaleficList = [Malefic, Malefic2, Malefic3, Malefic4, FallMalefic],
        GravityList = [Gravity, Gravity2];
    
    internal static readonly FrozenDictionary<uint, ushort> CombustList = new Dictionary<uint, ushort>
    {
        { Combust, Debuffs.Combust },
        { Combust2, Debuffs.Combust2 },
        { Combust3, Debuffs.Combust3 }
    }.ToFrozenDictionary();

    public static ASTGauge Gauge => GetJobGauge<ASTGauge>();
    public static CardType DrawnDPSCard => Gauge.DrawnCards[0];
    internal static bool HasNoCards => Gauge.DrawnCards.All(x => x is CardType.None);
    internal static bool HasNoDPSCard => DrawnDPSCard == CardType.None;
    internal static bool HasDPSCard => Gauge.DrawnCards[0] is not CardType.None;
    internal static bool HasLord => Gauge.DrawnCrownCard is CardType.Lord;
    internal static bool HasLady => Gauge.DrawnCrownCard is CardType.Lady;
    internal static bool HasSpire => Gauge.DrawnCards[2] == CardType.Spire;
    internal static bool HasEwer => Gauge.DrawnCards[2] == CardType.Ewer;
    internal static bool HasArrow => Gauge.DrawnCards[1] == CardType.Arrow;
    internal static bool HasBole => Gauge.DrawnCards[1] == CardType.Bole;
    internal static bool HasDivination=> HasStatusEffect(Buffs.Divination, anyOwner: true) || JustUsed(Divination);
    internal static float DivinationCD => GetCooldownRemainingTime(Divination);
    internal static float LightspeedChargeCD => GetCooldownChargeRemainingTime(Lightspeed);
    
    #region Dot Checker
    internal static bool NeedsDoT()
    {
        var dotAction = OriginalHook(Combust);
        var hpThreshold = IsNotEnabled(CustomComboPreset.AST_ST_Simple_DPS) && (Config.AST_ST_DPS_CombustSubOption == 1 || !InBossEncounter()) ? Config.AST_ST_DPS_CombustOption : 0;
        CombustList.TryGetValue(dotAction, out var dotDebuffID);
        var dotRefresh = IsNotEnabled(CustomComboPreset.AST_ST_Simple_DPS) ? Config.AST_ST_DPS_CombustUptime_Threshold : 2.5;
        var dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(dotAction) &&
               CanApplyStatus(CurrentTarget, dotDebuffID) &&
               !JustUsedOn(dotAction, CurrentTarget, 5f) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh;
    }
    #endregion
    
    #region Hidden Raidwides
    
    internal static bool RaidwideCollectiveUnconscious()
    {
        return IsEnabled(CustomComboPreset.AST_Raidwide_CollectiveUnconscious) && ActionReady(CollectiveUnconscious) && CanWeave() && RaidWideCasting();
    }
    internal static bool RaidwideNeutralSect()
    {
        return IsEnabled(CustomComboPreset.AST_Raidwide_NeutralSect) && ActionReady(OriginalHook(NeutralSect)) && CanWeave() && RaidWideCasting();
    }
    internal static bool RaidwideAspectedHelios()
    {
        return IsEnabled(CustomComboPreset.AST_Raidwide_AspectedHelios) && HasStatusEffect(Buffs.NeutralSect) && RaidWideCasting() && 
               !HasStatusEffect(Buffs.NeutralSectShield);
    }
    
    #endregion

    #region Get ST Heals
    internal static int GetMatchingConfigST(int i, IGameObject? OptionalTarget, out uint action, out bool enabled)
    {
        IGameObject? healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
        bool stopHot = Config.AST_ST_SimpleHeals_AspectedBeneficLow <= GetTargetHPPercent(healTarget, Config.AST_ST_SimpleHeals_IncludeShields);
        int refreshTime = Config.AST_ST_SimpleHeals_AspectedBeneficRefresh;
        Status? aspectedBeneficHoT = GetStatusEffect(Buffs.AspectedBenefic, healTarget);
        Status? neutralSectShield = GetStatusEffect(Buffs.NeutralSectShield, healTarget);
        
        switch (i)
        {
            case 0:
                action = CelestialIntersection;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_CelestialIntersection) &&
                          ActionReady(CelestialIntersection) && !(healTarget as IBattleChara)!.HasShield() &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveIntersection);
                return Config.AST_ST_SimpleHeals_CelestialIntersection;
            case 1:
                action = EssentialDignity;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_EssentialDignity) &&
                          ActionReady(EssentialDignity) &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveDignity);
                return Config.AST_ST_SimpleHeals_EssentialDignity;
            case 2:
                action = Exaltation;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_Exaltation) &&
                          ActionReady(Exaltation) &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveExalt);
                return Config.AST_ST_SimpleHeals_Exaltation;
            case 3:
                action = Bole;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_Bole) &&
                          HasBole &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveBole);
                return Config.AST_ST_SimpleHeals_Bole;
            case 4:
                action = Arrow;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_Arrow) &&
                          HasArrow &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveArrow);
                return Config.AST_ST_SimpleHeals_Arrow;
            case 5:
                action = Ewer;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_Ewer) &&
                          HasEwer &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveEwer);
                return Config.AST_ST_SimpleHeals_Ewer;
            case 6:
                action = Spire;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_Spire) &&
                          HasSpire &&
                          (CanWeave() || !Config.AST_ST_SimpleHeals_WeaveSpire);
                return Config.AST_ST_SimpleHeals_Spire;
            case 7:
                action = AspectedBenefic;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_AspectedBenefic) && 
                          ActionReady(AspectedBenefic) && stopHot &&
                          (aspectedBeneficHoT is null || 
                           aspectedBeneficHoT.RemainingTime <= refreshTime || 
                           neutralSectShield is null && HasStatusEffect(Buffs.NeutralSect));
                return Config.AST_ST_SimpleHeals_AspectedBeneficHigh;
            case 8:
                action = CelestialOpposition;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_CelestialOpposition) && ActionReady(CelestialOpposition) &&
                            (!Config.AST_ST_SimpleHeals_CelestialOppositionOptions[1] || !InBossEncounter()) &&
                            (!Config.AST_ST_SimpleHeals_CelestialOppositionOptions[0] || CanWeave());
                return Config.AST_ST_SimpleHeals_CelestialOpposition;
            case 9:
                action = CollectiveUnconscious;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_CollectiveUnconscious) && ActionReady(CollectiveUnconscious) &&
                          (!Config.AST_ST_SimpleHeals_CollectiveUnconsciousOptions[1] || !InBossEncounter()) &&
                          (!Config.AST_ST_SimpleHeals_CollectiveUnconsciousOptions[0] || CanWeave());
                return Config.AST_ST_SimpleHeals_CollectiveUnconscious;
            case 10:
                action = LadyOfCrown;
                enabled = IsEnabled(CustomComboPreset.AST_ST_Heals_SoloLady) && HasLady &&
                          (!Config.AST_ST_SimpleHeals_SoloLadyOptions[1] || !InBossEncounter()) &&
                          (!Config.AST_ST_SimpleHeals_SoloLadyOptions[0] || CanWeave());
                return Config.AST_ST_SimpleHeals_SoloLady;
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
                action = LadyOfCrown;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_LazyLady) &&
                          ActionReady(MinorArcana) && HasLady &&
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveLady);
                return Config.AST_AoE_SimpleHeals_LazyLady;
            case 1:
                action = CelestialOpposition;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_CelestialOpposition) &&
                          ActionReady(CelestialOpposition) &&
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveOpposition);
                return Config.AST_AoE_SimpleHeals_CelestialOpposition;
            case 2:
                action = Horoscope;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_Horoscope) && ActionReady(Horoscope) &&
                          !HasStatusEffect(Buffs.Horoscope) && !HasStatusEffect(Buffs.HoroscopeHelios) &&
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveHoroscope);
                return Config.AST_AoE_SimpleHeals_Horoscope;
            case 3:
                action = HoroscopeHeal;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_HoroscopeHeal) &&
                          HasStatusEffect(Buffs.HoroscopeHelios) &&
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveHoroscopeHeal);
                return Config.AST_AoE_SimpleHeals_HoroscopeHeal;
            case 4:
                action = NeutralSect;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_NeutralSect) &&
                          ActionReady(OriginalHook(NeutralSect)) &&
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveNeutralSect);
                return Config.AST_AoE_SimpleHeals_NeutralSect;
            case 5:
                action = StellarDetonation;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_StellarDetonation) && 
                          HasStatusEffect(Buffs.GiantDominance) && 
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveStellarDetonation);
                return Config.AST_AoE_SimpleHeals_StellarDetonation;
            case 6:
                action = OriginalHook(AspectedHelios);
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_Aspected) && ActionReady(AspectedHelios) &&
                          (LevelChecked(HeliosConjuction) && !HasStatusEffect(Buffs.HeliosConjunction) || 
                           !LevelChecked(HeliosConjuction) && !HasStatusEffect(Buffs.AspectedHelios) ||
                           HasStatusEffect(Buffs.NeutralSect) && !HasStatusEffect(Buffs.NeutralSectShield));
                return Config.AST_AoE_SimpleHeals_Aspected;
            
            case 7:
                action = Helios;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_Helios);
                return Config.AST_AoE_SimpleHeals_Helios;
            
            case 8:
                action = CollectiveUnconscious;
                enabled = IsEnabled(CustomComboPreset.AST_AoE_Heals_CollectiveUnconscious) &&
                          ActionReady(CollectiveUnconscious) &&
                          (CanWeave() || !Config.AST_AoE_SimpleHeals_WeaveCollectiveUnconscious);
                return Config.AST_AoE_SimpleHeals_CollectiveUnconscious;
        }

        enabled = false;
        action = 0;
        return 0;
    }
    #endregion

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
                .Where(member => member.BattleChara is not null && !member.BattleChara.IsDead && member.BattleChara.IsNotThePlayer())
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

            bool IsMeleeOrTank (ClassJob job) =>
                JobIDs.Melee.Contains((byte)job.RowId) ||
                JobIDs.Tank.Contains((byte)job.RowId);

            bool IsRangedOrHealer(ClassJob job) =>
                JobIDs.Ranged.Contains((byte)job.RowId) ||
                JobIDs.Healer.Contains((byte)job.RowId);

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
                        CardType.Balance => filter.Where(x => IsMeleeOrTank(x.RealJob!.Value)).ToList(),
                        CardType.Spear => filter.Where(x => IsRangedOrHealer(x.RealJob!.Value)).ToList(),
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
                
                //PluginLog.Debug($"names of each person still in the filter after job ordering: {string.Join(", ", filter.Select(x => x.BattleChara.Name))}");
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
        { GNB.JobID, 8 },
        { PLD.JobID, 9 },
        { WAR.JobID, 10 },
        { PCT.JobID, 11 },
        { SMN.JobID, 12 },
        { MCH.JobID, 13 },
        { BRD.JobID, 14 },
        { RDM.JobID, 15 },
        { DNC.JobID, 16 },
        { BLM.JobID, 17 },
        { WHM.JobID, 18 },
        { SGE.JobID, 19 },
        { SCH.JobID, 20 },
        
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
    
    #region Opener
    public static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }
    
    public static ASTOpenerMaxLevel1 Opener1 = new();
    
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
    #endregion

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
        StellarDetonation = 8324,

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
            Suntouched = 3895, 
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
