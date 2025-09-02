#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.BRD.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
#endregion

namespace WrathCombo.Combos.PvE;
internal partial class BRD
{
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
    internal static bool SongWanderer => gauge.Song == Song.Wanderer;
    internal static bool SongMage => gauge.Song == Song.Mage;
    internal static bool SongArmy => gauge.Song == Song.Army;
    //Dot Management
    internal static Status? Purple => GetStatusEffect(Debuffs.CausticBite, CurrentTarget) ?? GetStatusEffect(Debuffs.VenomousBite, CurrentTarget);
    internal static Status? Blue => GetStatusEffect(Debuffs.Stormbite, CurrentTarget) ?? GetStatusEffect(Debuffs.Windbite, CurrentTarget);
    internal static float PurpleRemaining => Purple?.RemainingTime ?? 0;
    internal static float BlueRemaining => Blue?.RemainingTime ?? 0;
    internal static bool DebuffCapCanPurple => CanApplyStatus(CurrentTarget, Debuffs.CausticBite) || CanApplyStatus(CurrentTarget, Debuffs.VenomousBite);
    internal static bool DebuffCapCanBlue => CanApplyStatus(CurrentTarget, Debuffs.Stormbite) || CanApplyStatus(CurrentTarget, Debuffs.Windbite);

    //Useful Bools
    internal static bool BardHasTarget => HasBattleTarget();
    internal static bool CanBardWeave => CanWeave();
    internal static bool CanWeaveDelayed => CanDelayedWeave();
    internal static bool CanIronJaws => LevelChecked(IronJaws);
    internal static bool BuffTime => GetCooldownRemainingTime(RagingStrikes) < 2.7;
    internal static bool BuffWindow => HasStatusEffect(Buffs.RagingStrikes) && 
                                       (HasStatusEffect(Buffs.BattleVoice) || !LevelChecked(BattleVoice)) &&
                                       (HasStatusEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale));

    //Buff Tracking
    internal static float RagingCD => GetCooldownRemainingTime(RagingStrikes);
    internal static float BattleVoiceCD => GetCooldownRemainingTime(BattleVoice);
    internal static float EmpyrealCD => GetCooldownRemainingTime(EmpyrealArrow);
    internal static float RadiantCD => GetCooldownRemainingTime(RadiantFinale);
    internal static float RagingStrikesDuration => GetStatusEffectRemainingTime(Buffs.RagingStrikes);
    internal static float RadiantFinaleDuration => GetStatusEffectRemainingTime(Buffs.RadiantFinale);

    // Charge Tracking
    internal static uint RainOfDeathCharges => LevelChecked(RainOfDeath) ? GetRemainingCharges(RainOfDeath) : 0;
    internal static uint BloodletterCharges => GetRemainingCharges(Bloodletter);

    #endregion

    #region Functions

        #region Pooling
        // Pooled Apex Logic
        internal static bool UsePooledApex()
        {
            if (gauge.SoulVoice >= 80)
            {
                if (BuffWindow && RagingStrikesDuration < 18 || RagingCD >= 50 && RagingCD <= 62)
                    return true;
            }
            return false;
        }
    

        // Pitch Perfect Logic
        internal static bool PitchPerfected()
        {
           if (LevelChecked(PitchPerfect) && SongWanderer &&
                (gauge.Repertoire == 3 || LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && EmpyrealCD < 2))
                return true;
        
           return false;
        }

        //Sidewinder Logic
        internal static bool UsePooledSidewinder()
        {
            if (BuffWindow && RagingStrikesDuration < 18 || RagingCD > 30)
                    return true;
            
            return false;
        }

        //Bloodletter & Rain of Death Logic
        internal static bool UsePooledBloodRain()
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
        #endregion

        #region Dot Management
        //Iron Jaws dot refreshing
        internal static bool UseIronJaws()
        {
            return ActionReady(IronJaws) && Purple is not null && Blue is not null &&
                   (PurpleRemaining < 4 || BlueRemaining < 4);
        }
        //Blue dot application and low level refresh
        internal static bool ApplyBlueDot()
        {
            return ActionReady(Windbite) && DebuffCapCanBlue && (Blue is null || !CanIronJaws && BlueRemaining < 4);
        }
        //Purple dot application and low level refresh
        internal static bool ApplyPurpleDot()
        {
            return ActionReady(VenomousBite) && DebuffCapCanPurple && (Purple is null || !CanIronJaws && PurpleRemaining < 4);
        }
        //Raging jaws option dot refresh for snapshot
        internal static bool RagingJawsRefresh()
        {
            return ActionReady(IronJaws) && HasStatusEffect(Buffs.RagingStrikes) && PurpleRemaining < 35 && BlueRemaining < 35;
        }
        #endregion

        #region Buff Timing
        //RadiantFinale Buff
        internal static bool UseRadiantBuff()
        {
            return ActionReady(RadiantFinale) && RagingCD < 2.2 && CanWeaveDelayed && !HasStatusEffect(Buffs.RadiantEncoreReady);
        } 
        //BattleVoice Buff
        internal static bool UseBattleVoiceBuff()
        {
            return ActionReady(BattleVoice) && (HasStatusEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale));
        }
        //RagingStrikes Buff
        internal static bool UseRagingStrikesBuff()
        {
            return ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasStatusEffect(Buffs.BattleVoice));
        } 
        //Barrage Buff
        internal static bool UseBarrageBuff()
        {
            return ActionReady(Barrage) && HasStatusEffect(Buffs.RagingStrikes) && !HasStatusEffect(Buffs.ResonantArrowReady);
        }
        #endregion
    
        #region Songs
        internal static bool WandererSong()
        {
            if (ActionReady(WanderersMinuet))
            {
                if (SongNone) // No song, use wanderer first
                   return true;
                    
                if (SongArmy && (CanWeaveDelayed || !BardHasTarget) && (SongTimerInSeconds <= 12 || gauge.Repertoire == 4)) //Transition to wanderer as soon as it is ready
                    return true;
            }
            return false;
        }
        internal static bool MagesSong()
        {
            if (ActionReady(MagesBallad) && (CanBardWeave || !BardHasTarget))
            {
                if (SongNone && !ActionReady(WanderersMinuet)) //No song, Use Mages if wanderer is on cd or not aquaired yet
                    return true;

                if (SongWanderer && SongTimerInSeconds <= 3 && gauge.Repertoire == 0) //Swap to mages after wanderer and no pitch perfect to spend
                    return true;
            }
            return false;
        }
        internal static bool ArmySong()
        {
            if (ActionReady(ArmysPaeon) && (CanBardWeave || !BardHasTarget))
            {
                if (SongNone && !ActionReady(MagesBallad) && !ActionReady(WanderersMinuet)) //No song, Use army as last resort
                    return true;

                if (SongMage && SongTimerInSeconds <= 3) //Transition to army after mages
                    return true;
            }
            return false;
        }
        internal static bool SongChangeEmpyreal()
        {
            return SongMage && SongTimerInSeconds <= 3 && ActionReady(ArmysPaeon) && ActionReady(EmpyrealArrow) && BardHasTarget && CanBardWeave; // Uses Empyreal before transiitoning to Army if possible
        }
        internal static bool SongChangePitchPerfect()
        {
            return SongWanderer && SongTimerInSeconds <= 3 && gauge.Repertoire > 0 && BardHasTarget && CanBardWeave; // Dumps the Pitch perfect stacks before transition to mages
        }
        #endregion

        #region Warden Resolver
        [ActionRetargeting.TargetResolver]
        private static IGameObject? WardenResolver() =>
            GetPartyMembers()
                .Select(member => member.BattleChara)          
                .FirstOrDefault(member => member.IsNotThePlayer() && !member.IsDead && member.IsCleansable() && InActionRange(TheWardensPaeon, member));
        #endregion
        
    #endregion

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

    #region Openers
    public static BRDStandard Opener1 = new();
    public static BRDAdjusted Opener2 = new();
    public static BRDComfy Opener3 = new();
    internal static WrathOpener Opener()
    {
        if (IsEnabled(Preset.BRD_ST_AdvMode))
        {
            if (BRD_Adv_Opener_Selection == 0 && Opener1.LevelChecked) return Opener1;
            if (BRD_Adv_Opener_Selection == 1 && Opener2.LevelChecked) return Opener2;
            if (BRD_Adv_Opener_Selection == 2 && Opener3.LevelChecked) return Opener3;
        }
        return Opener1.LevelChecked ? Opener1 : WrathOpener.Dummy;
    }

    internal class BRDStandard : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite,
            WanderersMinuet,
            EmpyrealArrow,
            CausticBite,
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
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData ContentCheckConfig => BRD_Balance_Content;
        public override bool HasCooldowns() =>
            IsOffCooldown(WanderersMinuet) &&
            IsOffCooldown(BattleVoice) &&
            IsOffCooldown(RadiantFinale) &&
            IsOffCooldown(RagingStrikes) &&
            IsOffCooldown(Barrage) &&
            IsOffCooldown(Sidewinder);
    }
    internal class BRDAdjusted : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            HeartbreakShot,
            Stormbite,
            WanderersMinuet,
            EmpyrealArrow,
            CausticBite,
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
            ([7, 10, 17, 18, 20], RefulgentArrow, () => HasStatusEffect(Buffs.HawksEye))
        ];
        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            6
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData ContentCheckConfig => BRD_Balance_Content;
        public override bool HasCooldowns() =>
            IsOffCooldown(WanderersMinuet) &&
            IsOffCooldown(BattleVoice) &&
            IsOffCooldown(RadiantFinale) &&
            IsOffCooldown(RagingStrikes) &&
            IsOffCooldown(Barrage) &&
            IsOffCooldown(Sidewinder);
    }
    internal class BRDComfy : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite,
            HeartbreakShot,
            WanderersMinuet,
            CausticBite,
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
            ([7, 10, 16, 18, 20], RefulgentArrow, () => HasStatusEffect(Buffs.HawksEye))
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData ContentCheckConfig => BRD_Balance_Content;
        public override bool HasCooldowns() =>
            IsOffCooldown(WanderersMinuet) &&
            IsOffCooldown(BattleVoice) &&
            IsOffCooldown(RadiantFinale) &&
            IsOffCooldown(RagingStrikes) &&
            IsOffCooldown(Barrage) &&
            IsOffCooldown(Sidewinder);
    }
    #endregion
}
