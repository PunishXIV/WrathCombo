using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.BRD.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
    #region ID's

    public const uint
        HeavyShot = 97,
        StraightShot = 98,
        VenomousBite = 100,
        RagingStrikes = 101,
        QuickNock = 106,
        Barrage = 107,
        Bloodletter = 110,
        Windbite = 113,
        MagesBallad = 114,
        ArmysPaeon = 116,
        RainOfDeath = 117,
        BattleVoice = 118,
        EmpyrealArrow = 3558,
        WanderersMinuet = 3559,
        IronJaws = 3560,
        TheWardensPaeon = 3561,
        Sidewinder = 3562,
        PitchPerfect = 7404,
        Troubadour = 7405,
        CausticBite = 7406,
        Stormbite = 7407,
        NaturesMinne = 7408,
        RefulgentArrow = 7409,
        BurstShot = 16495,
        ApexArrow = 16496,
        Shadowbite = 16494,
        Ladonsbite = 25783,
        BlastArrow = 25784,
        RadiantFinale = 25785,
        WideVolley = 36974,
        HeartbreakShot = 36975,
        ResonantArrow = 36976,
        RadiantEncore = 36977;

    public static class Buffs
    {
        public const ushort
            RagingStrikes = 125,
            Barrage = 128,
            MagesBallad = 135,
            ArmysPaeon = 137,
            BattleVoice = 141,
            NaturesMinne = 1202,
            WanderersMinuet = 2216,
            Troubadour = 1934,
            BlastArrowReady = 2692,
            RadiantFinale = 2722,
            ShadowbiteReady = 3002,
            HawksEye = 3861,
            ResonantArrowReady = 3862,
            RadiantEncoreReady = 3863;
    }

    public static class Debuffs
    {
        public const ushort
            VenomousBite = 124,
            Windbite = 129,
            CausticBite = 1200,
            Stormbite = 1201;
    }

    internal static class Traits
    {
        internal const ushort
            EnhancedBloodletter = 445;
    }

    #endregion

    #region Variables
    internal static readonly FrozenDictionary<uint, ushort> PurpleList = new Dictionary<uint, ushort>
    {
        { VenomousBite, Debuffs.VenomousBite },
        { CausticBite, Debuffs.CausticBite }
    }.ToFrozenDictionary();

    internal static readonly FrozenDictionary<uint, ushort> BlueList = new Dictionary<uint, ushort>
    {
        { Windbite, Debuffs.Windbite },
        { Stormbite, Debuffs.Stormbite }
    }.ToFrozenDictionary();

    // Gauge Stuff
    internal static BRDGauge? gauge = GetJobGauge<BRDGauge>();
    internal static int SongTimerInSeconds => gauge.SongTimer / 1000;
    internal static bool SongNone => gauge.Song == Song.None;
    internal static bool SongWanderer => gauge.Song == Song.WanderersMinuet;
    internal static bool SongMage => gauge.Song == Song.MagesBallad;
    internal static bool SongArmy => gauge.Song == Song.ArmysPaeon;
    //Dot Management
    internal static IStatus? Purple => GetStatusEffect(Debuffs.CausticBite, CurrentTarget) ?? GetStatusEffect(Debuffs.VenomousBite, CurrentTarget);
    internal static IStatus? Blue => GetStatusEffect(Debuffs.Stormbite, CurrentTarget) ?? GetStatusEffect(Debuffs.Windbite, CurrentTarget);
    internal static float PurpleRemaining => Purple?.RemainingTime ?? 0;
    internal static float BlueRemaining => Blue?.RemainingTime ?? 0;
    internal static bool DebuffCapCanPurple => CanApplyStatus(CurrentTarget, Debuffs.CausticBite) || CanApplyStatus(CurrentTarget, Debuffs.VenomousBite);
    internal static bool DebuffCapCanBlue => CanApplyStatus(CurrentTarget, Debuffs.Stormbite) || CanApplyStatus(CurrentTarget, Debuffs.Windbite);

    //Useful Bools
    internal static bool BardHasTarget => HasBattleTarget();
    internal static bool JustSangSong => JustUsed(WanderersMinuet) || JustUsed(MagesBallad) || JustUsed(ArmysPaeon);
    internal static bool CanBardWeave => CanWeave();
    internal static bool CanWeaveDelayed => CanDelayedWeave();
    internal static bool CanIronJaws => LevelChecked(IronJaws);
    internal static bool BuffWindow => HasStatusEffect(Buffs.RagingStrikes) &&
                                       (HasStatusEffect(Buffs.BattleVoice) || !LevelChecked(BattleVoice)) &&
                                       (HasStatusEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale));

    //Buff Tracking
    internal static float RagingCD => GetCooldownRemainingTime(RagingStrikes);
    internal static float BattleVoiceCD => GetCooldownRemainingTime(BattleVoice);
    internal static float EmpyrealCD => GetCooldownRemainingTime(EmpyrealArrow);
    internal static float RagingStrikesDuration => GetStatusEffectRemainingTime(Buffs.RagingStrikes);

    // Charge Tracking
    internal static uint BloodletterCharges => GetRemainingCharges(OriginalHook(Bloodletter));

    // Pitch Perfect Logic
    internal static bool PitchPerfected()
    {
        if (LevelChecked(PitchPerfect) && SongWanderer &&
             (gauge.Repertoire == 3 || LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && EmpyrealCD < 2))
            return true;

        return false;

    }

    #region Warden Resolver
    [ActionRetargeting.TargetResolver]
    private static IGameObject? WardenResolver() =>
        GetPartyMembers()
            .Select(member => member.BattleChara)
            .FirstOrDefault(member => member.IsNotThePlayer() && !member.IsDead && member.IsCleansable() && InActionRange(TheWardensPaeon, member));
    #endregion

    #endregion

    #region Rotation

    #region Flag Stuff
    [Flags]
    private enum Combo
    {
        // Target-type for combo
        ST = 1 << 0, // 1
        AoE = 1 << 1, // 2

        // Complexity of combo
        Adv = 1 << 2, // 4
        Simple = 1 << 3, // 8
        Basic = 1 << 4, // 16
    }

    /// <summary>
    ///     Checks whether a given preset is enabled, and the flags match it.
    /// </summary>
    private static bool IsSTEnabled(Combo flags, Preset preset) =>
        flags.HasFlag(Combo.ST) && IsEnabled(preset);

    /// <summary>
    ///     Checks whether a given preset is enabled, and the flags match it.
    /// </summary>
    private static bool IsAoEEnabled(Combo flags, Preset preset) =>
        flags.HasFlag(Combo.AoE) && IsEnabled(preset);
    #endregion

    #region OGCD Attacks
    private static bool TryOGCDAttacks(Combo flags, ref uint actionID)
    {
        #region Enables
        bool songsEnabled =
             flags.HasFlag(Combo.Simple) ||
             IsSTEnabled(flags, Preset.BRD_Adv_Song) ||
             IsAoEEnabled(flags, Preset.BRD_AoE_Adv_Songs);

        bool buffsEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_Buffs) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_Adv_Buffs);

        bool oGCDEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_ST_Adv_oGCD) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_Adv_oGCD);

        bool poolingEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_Pooling) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_Pooling);

        bool headGrazeEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_Interrupt) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_Adv_Interrupt);

        bool troubadourEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_Troubadour);

        bool naturesMinneEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_NaturesMinne);

        bool secondWindEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_ST_SecondWind) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_SecondWind);

        bool wardensEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_ST_Wardens) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_Wardens);
        #endregion

        #region Configs

        int buffsThresholdST = BRD_Adv_Buffs_SubOption == 1 || !InBossEncounter() ? BRD_Adv_Buffs_Threshold : 0; //Boss Check
        int buffsThresholdAoE = BRD_AoE_Adv_Buffs_SubOption == 1 || !InBossEncounter() ? BRD_AoE_Adv_Buffs_Threshold : 0; //Boss Check
        int buffsThreshold =
            flags.HasFlag(Combo.Simple) ? 0 :
            flags.HasFlag(Combo.ST) ? buffsThresholdST : buffsThresholdAoE;



        bool ragingEnabled = flags.HasFlag(Combo.Simple) ||
                             flags.HasFlag(Combo.ST) && BRD_Adv_Buffs_Options[0] ||
                             flags.HasFlag(Combo.AoE) && BRD_AoE_Adv_Buffs_Options[0];

        bool battleVoiceEnabled = flags.HasFlag(Combo.Simple) ||
                                  flags.HasFlag(Combo.ST) && BRD_Adv_Buffs_Options[1] ||
                                  flags.HasFlag(Combo.AoE) && BRD_AoE_Adv_Buffs_Options[1];

        bool barrageEnabled = flags.HasFlag(Combo.Simple) ||
                              flags.HasFlag(Combo.ST) && BRD_Adv_Buffs_Options[2] ||
                              flags.HasFlag(Combo.AoE) && BRD_AoE_Adv_Buffs_Options[2];

        bool radiantEnabled = flags.HasFlag(Combo.Simple) ||
                              flags.HasFlag(Combo.ST) && BRD_Adv_Buffs_Options[3] ||
                              flags.HasFlag(Combo.AoE) && BRD_AoE_Adv_Buffs_Options[3];

        bool allBuffsEnabled = flags.HasFlag(Combo.Simple) ||
                               radiantEnabled && battleVoiceEnabled && ragingEnabled && barrageEnabled;

        int seconWindThreshold =
            flags.HasFlag(Combo.Simple) ? 40 :
            flags.HasFlag(Combo.ST) ? BRD_STSecondWindThreshold : BRD_AoESecondWindThreshold;

        bool autoWardenST = flags.HasFlag(Combo.Simple) ||
                            flags.HasFlag(Combo.ST) && BRD_ST_Wardens_Auto ||
                            flags.HasFlag(Combo.AoE) && BRD_AoE_Wardens_Auto;
        #endregion

        #region Functions

        bool WandererSong()
        {
            if (ActionReady(WanderersMinuet) && !JustSangSong)
            {
                if (SongNone) // No song, use wanderer first
                    return true;

                var canTransition = IsEnabled(Preset.BRD_Hidden_Song_Extension)
                    ? SongTimerInSeconds <= 3
                    : SongTimerInSeconds <= 12 || gauge.Repertoire == 4;

                if (SongArmy && (CanWeaveDelayed || !BardHasTarget) && canTransition) //Transition to wanderer as soon as it is ready
                    return true;
            }
            return false;
        }

        bool MagesSong()
        {
            if (ActionReady(MagesBallad) && !JustSangSong && (CanBardWeave || !BardHasTarget))
            {
                if (SongNone && !ActionReady(WanderersMinuet)) //No song, Use Mages if wanderer is on cd or not aquaired yet
                    return true;

                if (SongWanderer && SongTimerInSeconds <= 3 && gauge.Repertoire == 0) //Swap to mages after wanderer and no pitch perfect to spend
                    return true;
            }
            return false;
        }

        bool ArmySong()
        {
            if (ActionReady(ArmysPaeon) && !JustSangSong && (CanBardWeave || !BardHasTarget))
            {
                if (SongNone && !ActionReady(MagesBallad) && !ActionReady(WanderersMinuet)) //No song, Use army as last resort
                    return true;

                if (SongMage && SongTimerInSeconds <= 3) //Transition to army after mages
                    return true;
            }
            return false;
        }

        bool SongChangeEmpyreal()
        {
            return SongMage && SongTimerInSeconds <= 3 && ActionReady(ArmysPaeon) && ActionReady(EmpyrealArrow) && BardHasTarget && CanBardWeave; // Uses Empyreal before transiitoning to Army if possible
        }

        bool SongChangePitchPerfect()
        {
            return SongWanderer && SongTimerInSeconds <= 3 && gauge.Repertoire > 0 && BardHasTarget && CanBardWeave; // Dumps the Pitch perfect stacks before transition to mages
        }

        //Bloodletter & Rain of Death Logic
        bool UsePooledBloodRain()
        {
            if ((!WasLastAbility(Bloodletter) || !WasLastAbility(RainOfDeath) || !WasLastAbility(HeartbreakShot)) &&
                (EmpyrealCD > 2 || !LevelChecked(EmpyrealArrow)))
            {
                if (BloodletterCharges == 3 && TraitLevelChecked(Traits.EnhancedBloodletter) ||
                    BloodletterCharges == 2 && !TraitLevelChecked(Traits.EnhancedBloodletter) ||
                    BloodletterCharges > 0 && (BuffWindow || RagingCD > 30))
                    return true;
            }
            return false;
        }

        //Sidewinder Logic
        bool UsePooledSidewinder()
        {
            if (BuffWindow && RagingStrikesDuration < 18 || RagingCD > 30)
                return true;

            return false;
        }

        #endregion

        #region Songs
        if (songsEnabled && InCombat() && (CanWeave() || !BardHasTarget))
        {
            if (SongChangePitchPerfect())
            {
                actionID = PitchPerfect;
                return true;
            }
            if (SongChangeEmpyreal())
            {
                actionID = EmpyrealArrow;
                return true;
            }
            if (WandererSong())
            {
                actionID = WanderersMinuet;
                return true;
            }
            if (MagesSong())
            {
                actionID = MagesBallad;
                return true;
            }
            if (ArmySong())
            {
                actionID = ArmysPaeon;
                return true;
            }
        }
        #endregion

        #region Buffs
        if (buffsEnabled && CanWeave() && GetTargetHPPercent() > buffsThreshold)
        {
            if (allBuffsEnabled && !SongNone && LevelChecked(MagesBallad))
            {
                if (ActionReady(RadiantFinale) && RagingCD < 2.2 && CanWeaveDelayed && !HasStatusEffect(Buffs.RadiantEncoreReady))
                {
                    actionID = RadiantFinale;
                    return true;
                }
                if (ActionReady(BattleVoice) && (HasStatusEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                {
                    actionID = BattleVoice;
                    return true;
                }
                if (ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasStatusEffect(Buffs.BattleVoice)))
                {
                    actionID = RagingStrikes;
                    return true;
                }
                if (ActionReady(Barrage) && HasStatusEffect(Buffs.RagingStrikes) && !HasStatusEffect(Buffs.ResonantArrowReady))
                {
                    actionID = Barrage;
                    return true;
                }
            }

            if (!allBuffsEnabled || !LevelChecked(MagesBallad))
            {
                if (ActionReady(RadiantFinale) && radiantEnabled)
                {
                    actionID = RadiantFinale;
                    return true;
                }
                if (ActionReady(BattleVoice) && battleVoiceEnabled)
                {
                    actionID = BattleVoice;
                    return true;
                }
                if (ActionReady(RagingStrikes) && ragingEnabled)
                {
                    actionID = RagingStrikes;
                    return true;
                }
                if (ActionReady(Barrage) && barrageEnabled)
                {
                    actionID = Barrage;
                    return true;
                }
            }
        }
        #endregion

        #region OGCD Attacks
        if (Role.CanHeadGraze(headGrazeEnabled, WeaveTypes.DelayWeave))
        {
            actionID = Role.HeadGraze;
            return true;
        }

        if (CanWeave() && oGCDEnabled && (GetCooldownRemainingTime(RagingStrikes) > 2.7 || !buffsEnabled))
        {
            if (ActionReady(EmpyrealArrow))
            {
                actionID = EmpyrealArrow;
                return true;
            }

            if (PitchPerfected())
            {
                actionID = OriginalHook(PitchPerfect);
                return true;
            }

            if (ActionReady(Sidewinder) && (UsePooledSidewinder() || !poolingEnabled))
            {
                actionID = Sidewinder;
                return true;
            }

            if (flags.HasFlag(Combo.ST) && ActionReady(OriginalHook(Bloodletter)) && (UsePooledBloodRain() || !poolingEnabled))
            {
                actionID = OriginalHook(Bloodletter);
                return true;
            }

            if (flags.HasFlag(Combo.AoE) && ActionReady(RainOfDeath) && (UsePooledBloodRain() || !poolingEnabled))
            {
                if (NumberOfEnemiesInRange(RainOfDeath) >= 2)
                {
                    actionID = OriginalHook(RainOfDeath);
                    return true;
                }
                {
                    actionID = OriginalHook(Bloodletter);
                    return true;
                }
            }

            if (!LevelChecked(RainOfDeath) && !WasLastAction(Bloodletter) && BloodletterCharges > 0) //Low Level Just send it
            {
                actionID = OriginalHook(Bloodletter);
                return true;
            }
        }
        #endregion

        #region Mitigation and Healing

        if (flags.HasFlag(Combo.ST) && troubadourEnabled && ActionReady(Troubadour) && GroupDamageIncoming() && CanWeave() &&
            NumberOfAlliesInRange(Troubadour) >= GetPartyMembers().Count * .75 &&
            !JustUsed(NaturesMinne) && !HasAnyStatusEffects([Buffs.Troubadour, Buffs.NaturesMinne, DNC.Buffs.ShieldSamba, MCH.Buffs.Tactician], anyOwner: true))
        {
            actionID = Troubadour;
            return true;
        }

        if (flags.HasFlag(Combo.ST) && naturesMinneEnabled && ActionReady(NaturesMinne) && GroupDamageIncoming() && CanWeave() &&
            NumberOfAlliesInRange(NaturesMinne) >= GetPartyMembers().Count * .75 &&
            !JustUsed(Troubadour) && !HasAnyStatusEffects([Buffs.Troubadour, Buffs.NaturesMinne], anyOwner: true))
        {
            actionID = NaturesMinne;
            return true;
        }

        if (secondWindEnabled && Role.CanSecondWind(seconWindThreshold) && CanWeave())
        {
            actionID = Role.SecondWind;
            return true;
        }

        if (wardensEnabled && ActionReady(TheWardensPaeon) && CanWeave())
        {
            if (HasCleansableDebuff(LocalPlayer))
            {
                actionID = TheWardensPaeon;
                return true;
            }

            if (autoWardenST && WardenResolver() is not null)
            {
                actionID = TheWardensPaeon.Retarget(actionID, WardenResolver);
                return true;
            }
        }

        #endregion

        return false;
    }
    #endregion

    #region GCD Attacks
    private static bool TryGCDAttacks(Combo flags, ref uint actionID)
    {
        #region Enables
        bool stDotEnabled =
            flags.HasFlag(Combo.Simple) || IsSTEnabled(flags, Preset.BRD_Adv_DoT);

        bool ironjawsEnabled =
            flags.HasFlag(Combo.Simple) || BRD_Adv_DoT_Options[0];

        bool dotApplicationEnabled =
            flags.HasFlag(Combo.Simple) || BRD_Adv_DoT_Options[1];

        bool ragingjawsEnabled =
            flags.HasFlag(Combo.Simple) || BRD_Adv_DoT_Options[2];

        bool multidotEnabled =
            flags.HasFlag(Combo.Simple) || IsSTEnabled(flags, Preset.BRD_Adv_DoT) && BRD_Adv_DoT_Options[3] || IsAoEEnabled(flags, Preset.BRD_AoE_Adv_Multidot);

        bool ragingEnabled = flags.HasFlag(Combo.Simple) ||
                             flags.HasFlag(Combo.ST) && BRD_Adv_Buffs_Options[0] ||
                             flags.HasFlag(Combo.AoE) && BRD_AoE_Adv_Buffs_Options[0];

        bool radiantEncoreEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_BuffsEncore) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_BuffsEncore);

        bool apexComboEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_ST_ApexArrow) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_ApexArrow);

        bool apexPoolingEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_ApexPooling) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_ApexPooling);

        bool resonantArrowEnabled =
            flags.HasFlag(Combo.Simple) ||
            IsSTEnabled(flags, Preset.BRD_Adv_BuffsResonant) ||
            IsAoEEnabled(flags, Preset.BRD_AoE_BuffsResonant);

        #endregion

        #region Configs
        int ComputeMultidotHpThreshold(IGameObject? x)
        {
            if (x is null)
                return 0;

            if (flags.HasFlag(Combo.Simple))
                return 50;

            if (flags.HasFlag(Combo.ST))
            {
                if (InBossEncounter())
                {
                    return x.IsBoss() ? BRD_ST_DPS_DotBossOption : BRD_ST_DPS_DotBossAddsOption;
                }
                return BRD_ST_DPS_DotTrashOption;
            }

            if (flags.HasFlag(Combo.AoE))
            {
                if (InBossEncounter())
                {
                    return x.IsBoss() ? BRD_AoE_Adv_MultidotBossOption : BRD_AoE_Adv_MultidotBossAddsOption;
                }
                return BRD_AoE_Adv_MultidotTrashOption;
            }

            return 0;
        }

        int computeMultidotRefresh = flags.HasFlag(Combo.Simple) ? 5 : flags.HasFlag(Combo.ST) ? BRD_Adv_DoT_Refresh : BRD_AoE_Adv_Multidot_Refresh;

        int ragingJawsRenewTime =
            flags.HasFlag(Combo.Simple) ? 5 : BRD_RagingJawsRenewTime;

        #endregion

        #region Functions
        int computeRefresh() => IsEnabled(Preset.BRD_ST_SimpleMode) ? 4 : BRD_Adv_DoT_Refresh;

        //Iron Jaws dot refreshing
        bool UseIronJaws()
        {
            return ActionReady(IronJaws) && Purple is not null && Blue is not null &&
                   (PurpleRemaining < computeRefresh() || BlueRemaining < computeRefresh());
        }
        //Blue dot application and low level refresh
        bool ApplyBlueDot()
        {
            return ActionReady(OriginalHook(Windbite)) && DebuffCapCanBlue && (Blue is null || !CanIronJaws && BlueRemaining < computeRefresh());
        }
        //Purple dot application and low level refresh
        bool ApplyPurpleDot()
        {
            return ActionReady(OriginalHook(VenomousBite)) && DebuffCapCanPurple && (Purple is null || !CanIronJaws && PurpleRemaining < computeRefresh());
        }
        //Raging jaws option dot refresh for snapshot
        bool RagingJawsRefresh()
        {
            return ActionReady(IronJaws) && HasStatusEffect(Buffs.RagingStrikes) && PurpleRemaining < 35 && BlueRemaining < 35;
        }
        int ComputeHpThreshold(IGameObject? x)
        {
            if (x is null)
                return 0;

            if (InBossEncounter())
            {
                return x.IsBoss() ? BRD_ST_DPS_DotBossOption : BRD_ST_DPS_DotBossAddsOption;
            }
            return BRD_ST_DPS_DotTrashOption;
        }

        // Pooled Apex Logic
        bool UsePooledApex()
        {
            if (gauge.SoulVoice >= 80)
            {
                if (BuffWindow && RagingStrikesDuration < 18 || RagingCD >= 50 && RagingCD <= 62)
                    return true;
            }
            return false;
        }
        #endregion

        #region ST Dot Management
        if (flags.HasFlag(Combo.ST) && stDotEnabled && GetTargetHPPercent() > ComputeHpThreshold(CurrentTarget))
        {
            if (ironjawsEnabled && UseIronJaws())
            {
                actionID = OriginalHook(IronJaws);
                return true;
            }

            if (dotApplicationEnabled && ApplyBlueDot())
            {
                actionID = OriginalHook(Stormbite);
                return true;
            }

            if (dotApplicationEnabled && ApplyPurpleDot())
            {
                actionID = OriginalHook(VenomousBite);
                return true;
            }

            if (ragingjawsEnabled && RagingJawsRefresh() && RagingStrikesDuration < ragingJawsRenewTime)
            {
                actionID = OriginalHook(IronJaws);
                return true;
            }
        }
        #endregion

        #region GCDS

        var widevolleyEnemyCount = NumberOfEnemiesInRange(OriginalHook(WideVolley));

        if (flags.HasFlag(Combo.AoE) && HasStatusEffect(Buffs.Barrage) && widevolleyEnemyCount >= 3)
        {
            actionID = OriginalHook(WideVolley); //Uses on 3 or more. 
            return true;
        }
        if (HasStatusEffect(Buffs.Barrage))
        {
            actionID = OriginalHook(StraightShot); // Use on two or less
            return true;
        }
        if (radiantEncoreEnabled && HasStatusEffect(Buffs.RadiantEncoreReady) && GetStatusEffectRemainingTime(Buffs.RadiantFinale) < 16 &&
            (HasStatusEffect(Buffs.RagingStrikes) || !ragingEnabled))
        {
            actionID = OriginalHook(RadiantEncore);
            return true;
        }
        if (apexComboEnabled)
        {
            if (HasStatusEffect(Buffs.BlastArrowReady))
            {
                actionID = BlastArrow;
                return true;
            }

            if (UsePooledApex() || !apexPoolingEnabled && gauge.SoulVoice == 100)
            {
                actionID = ApexArrow;
                return true;
            }
        }

        if (resonantArrowEnabled && HasStatusEffect(Buffs.ResonantArrowReady))
        {
            actionID = ResonantArrow;
            return true;
        }

        if (flags.HasFlag(Combo.AoE) && HasStatusEffect(Buffs.HawksEye) && widevolleyEnemyCount >= 2)
        {
            actionID = OriginalHook(WideVolley); //Uses on 2 or more. 
            return true;
        }
        if (HasStatusEffect(Buffs.HawksEye))
        {
            actionID = OriginalHook(StraightShot);
            return true;
        }

        #endregion

        #region Multidot Management
        if (multidotEnabled)
        {
            #region Dottable Variables
            var blueDotAction = OriginalHook(Windbite);
            var purpleDotAction = OriginalHook(VenomousBite);
            BlueList.TryGetValue(blueDotAction, out var blueDotDebuffID);
            PurpleList.TryGetValue(purpleDotAction, out var purpleDotDebuffID);
            var ironTarget = SimpleTarget.BardRefreshableEnemy(IronJaws, blueDotDebuffID, purpleDotDebuffID, ComputeMultidotHpThreshold, computeMultidotRefresh);
            var blueTarget = SimpleTarget.DottableEnemy(blueDotAction, blueDotDebuffID, ComputeMultidotHpThreshold, computeMultidotRefresh);
            var purpleTarget = SimpleTarget.DottableEnemy(purpleDotAction, purpleDotDebuffID, ComputeMultidotHpThreshold, computeMultidotRefresh);
            #endregion

            if (ironTarget is not null && LevelChecked(IronJaws))
            {
                actionID = IronJaws.Retarget(actionID, ironTarget);
                return true;
            }

            if (blueTarget is not null && LevelChecked(Windbite))
            {
                actionID = blueDotAction.Retarget(actionID, blueTarget);
                return true;
            }

            if (purpleTarget is not null && LevelChecked(VenomousBite))
            {
                actionID = purpleDotAction.Retarget(actionID, purpleTarget);
                return true;
            }
        }
        #endregion

        return false;
    }
    #endregion

    #endregion

    #region Openers
    public static BRDStandard Opener1 = new();
    public static BRDAdjusted Opener2 = new();
    public static BRDComfy Opener3 = new();
    public static BRDEarly Opener4 = new();
    internal static WrathOpener Opener()
    {
        if (IsEnabled(Preset.BRD_ST_AdvMode))
        {
            if (BRD_Adv_Opener_Selection == 0 && Opener1.LevelChecked) return Opener1;
            if (BRD_Adv_Opener_Selection == 1 && Opener2.LevelChecked) return Opener2;
            if (BRD_Adv_Opener_Selection == 2 && Opener3.LevelChecked) return Opener3;
            if (BRD_Adv_Opener_Selection == 3 && Opener3.LevelChecked) return Opener4;
        }
        return Opener1.LevelChecked ? Opener1 : WrathOpener.Dummy;
    }

    internal abstract class BRDOpenerBase : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override Preset Preset => Preset.BRD_ST_Adv_Balance_Standard;

        internal override UserData ContentCheckConfig => BRD_Balance_Content;
        internal override bool IncludePot => BRD_Opener_Potion;
        public override bool HasCooldowns() =>
            IsOffCooldown(WanderersMinuet) &&
            IsOffCooldown(BattleVoice) &&
            IsOffCooldown(RadiantFinale) &&
            IsOffCooldown(RagingStrikes) &&
            IsOffCooldown(Barrage) &&
            IsOffCooldown(Sidewinder);
    }

    internal class BRDStandard : BRDOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite,
            WanderersMinuet,
            EmpyrealArrow,
            CausticBite,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            BattleVoice,
            BurstShot,
            RadiantFinale,
            RagingStrikes,
            BurstShot,
            RadiantEncore,
            Barrage,
            RefulgentArrow,
            Sidewinder,
            ResonantArrow,
            EmpyrealArrow,
            BurstShot,
            BurstShot,
            IronJaws,
            BurstShot
        ];
        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([6, 9, 16, 17, 19], RefulgentArrow, () => HasStatusEffect(Buffs.HawksEye))
        ];
        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            5
        ];
    }
    internal class BRDAdjusted : BRDOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            HeartbreakShot,
            Stormbite,
            WanderersMinuet,
            EmpyrealArrow,
            CausticBite,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            BattleVoice,
            BurstShot,
            RadiantFinale,
            RagingStrikes,
            BurstShot,
            Barrage,
            RefulgentArrow,
            Sidewinder,
            RadiantEncore,
            ResonantArrow,
            EmpyrealArrow,
            BurstShot,
            BurstShot,
            IronJaws,
            BurstShot
        ];
        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([8, 11, 18, 19, 21], RefulgentArrow, () => HasStatusEffect(Buffs.HawksEye))
        ];
        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            7
        ];
    }
    internal class BRDComfy : BRDOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite,
            HeartbreakShot,
            WanderersMinuet,
            CausticBite,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            EmpyrealArrow,
            RadiantFinale,
            BurstShot,
            BattleVoice,
            RagingStrikes,
            BurstShot,
            Barrage,
            RefulgentArrow,
            Sidewinder,
            RadiantEncore,
            ResonantArrow,
            BurstShot,
            EmpyrealArrow,
            BurstShot,
            IronJaws,
            BurstShot
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([8, 11, 17, 19, 21], RefulgentArrow, () => HasStatusEffect(Buffs.HawksEye))
        ];
    }
    internal class BRDEarly : BRDOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite, //0
                WanderersMinuet,
                BattleVoice,
            CausticBite, //2.5
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
                RagingStrikes,
                RadiantFinale,
            BurstShot, //5
                EmpyrealArrow,
            BurstShot, //7.5
                Barrage,
            RefulgentArrow, //10
                Sidewinder,
            RadiantEncore, //12.5
            ResonantArrow, //15
            BurstShot, //17.5
            IronJaws, //20
                EmpyrealArrow,
            BurstShot, //22.5
            BurstShot, //25
        ];
        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([8, 10, 16, 19, 20], RefulgentArrow, () => HasStatusEffect(Buffs.HawksEye))
        ];
    }
    #endregion
}
