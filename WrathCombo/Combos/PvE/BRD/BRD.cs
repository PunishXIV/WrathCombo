using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
    #region Song status
    internal static bool SongIsNotNone(Song value) => value != Song.NONE;
    internal static bool SongIsNone(Song value) => value == Song.NONE;
    internal static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
    #endregion

    #region Smaller features
    internal class BRD_StraightShotUpgrade : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_StraightShotUpgrade;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_DoTMaintainance))
            {
                if (InCombat())
                {
                    bool canIronJaws = LevelChecked(IronJaws);
                    Status? purple = FindTargetEffect(Debuffs.CausticBite) ?? FindTargetEffect(Debuffs.VenomousBite);
                    Status? blue = FindTargetEffect(Debuffs.Stormbite) ?? FindTargetEffect(Debuffs.Windbite);
                    float purpleRemaining = purple?.RemainingTime ?? 0;
                    float blueRemaining = blue?.RemainingTime ?? 0;

                    if (purple is not null && purpleRemaining < 4)
                        return canIronJaws ? IronJaws : VenomousBite;
                    if (blue is not null && blueRemaining < 4)
                        return canIronJaws ? IronJaws : Windbite;
                }
            }

            if (IsEnabled(CustomComboPreset.BRD_ApexST))
            {
                BRDGauge? gauge = GetJobGauge<BRDGauge>();

                if (gauge.SoulVoice == 100)
                    return ApexArrow;
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;
            }

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            return actionID;
        }
    }

    internal class BRD_IronJaws : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not IronJaws)
                return actionID;

            Status? purple = FindTargetEffect(Debuffs.CausticBite) ?? FindTargetEffect(Debuffs.VenomousBite);
            Status? blue = FindTargetEffect(Debuffs.Stormbite) ?? FindTargetEffect(Debuffs.Windbite);
            float purpleRemaining = purple?.RemainingTime ?? 0;
            float blueRemaining = blue?.RemainingTime ?? 0;

            // Before Iron Jaws: Alternate between DoTs
            if (!LevelChecked(IronJaws))
                return LevelChecked(Windbite) && blueRemaining <= purpleRemaining ? Windbite : VenomousBite;

            // At least Lv56 (Iron Jaws) from here on...

            // DoT application takes priority, as Iron Jaws always cuts ticks
            if (blue is null && LevelChecked(Windbite))
                return OriginalHook(Windbite);
            if (purple is null && LevelChecked(VenomousBite))
                return OriginalHook(VenomousBite);

            // DoT refresh over Apex Option
            if (purpleRemaining < 4 || blueRemaining < 4)
                return IronJaws;

            // Apex Option
            if (IsEnabled(CustomComboPreset.BRD_IronJawsApex))
            {
                BRDGauge? gauge = GetJobGauge<BRDGauge>();

                if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;
                if (gauge.SoulVoice == 100)
                    return ApexArrow;
            }
            return actionID;
        }
    }

    internal class BRD_IronJaws_Alternate : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws_Alternate;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not IronJaws)
                return actionID;

            Status? purple = FindTargetEffect(Debuffs.CausticBite) ?? FindTargetEffect(Debuffs.VenomousBite);
            Status? blue = FindTargetEffect(Debuffs.Stormbite) ?? FindTargetEffect(Debuffs.Windbite);
            float purpleRemaining = purple?.RemainingTime ?? 0;
            float blueRemaining = blue?.RemainingTime ?? 0;

            // Iron Jaws only if it is applicable
            if (LevelChecked(IronJaws) && (
                    (purple is not null && purpleRemaining < 4) ||
                    (blue is not null && blueRemaining < 4)))
                return IronJaws;

            // Otherwise alternate between DoTs as needed
            return LevelChecked(Windbite) && blueRemaining <= purpleRemaining ?
                OriginalHook(Windbite) :
                OriginalHook(VenomousBite);
        }
    }

    internal class BRD_AoE_oGCD : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_oGCD;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not RainOfDeath)
                return actionID;

            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            bool songArmy = gauge.Song == Song.ARMY;
            bool songWanderer = gauge.Song == Song.WANDERER;

            if (IsEnabled(CustomComboPreset.BRD_AoE_oGCD_Songs) && (gauge.SongTimer < 1 || songArmy))
            {
                if (ActionReady(WanderersMinuet))
                    return WanderersMinuet;
                if (ActionReady(MagesBallad))
                    return MagesBallad;
                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            if (songWanderer && gauge.Repertoire == 3)
                return OriginalHook(PitchPerfect);
            if (ActionReady(EmpyrealArrow))
                return EmpyrealArrow;
            if (ActionReady(RainOfDeath))
                return RainOfDeath;
            if (ActionReady(Sidewinder))
                return Sidewinder;

            return actionID;
        }
    }

    internal class BRD_ST_oGCD : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_oGCD;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bloodletter or HeartbreakShot))
                return actionID;

            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            bool songArmy = gauge.Song == Song.ARMY;
            bool songWanderer = gauge.Song == Song.WANDERER;

            if (IsEnabled(CustomComboPreset.BRD_ST_oGCD_Songs) && (gauge.SongTimer < 1 || songArmy))
            {
                if (ActionReady(WanderersMinuet))
                    return WanderersMinuet;
                if (ActionReady(MagesBallad))
                    return MagesBallad;
                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            if (songWanderer && gauge.Repertoire == 3)
                return OriginalHook(PitchPerfect);
            if (ActionReady(EmpyrealArrow))
                return EmpyrealArrow;
            if (ActionReady(Sidewinder))
                return Sidewinder;
            if (ActionReady(Bloodletter))
                return OriginalHook(Bloodletter);

            return actionID;
        }
    }

    internal class BRD_AoE_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (QuickNock or Ladonsbite))
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_Apex))
            {
                BRDGauge? gauge = GetJobGauge<BRDGauge>();

                if (gauge.SoulVoice == 100)
                    return ApexArrow;
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;
            }

            if (IsEnabled(CustomComboPreset.BRD_AoE_Combo) && ActionReady(WideVolley) && HasEffect(Buffs.HawksEye))
                return OriginalHook(WideVolley);

            return actionID;
        }
    }

    internal class BRD_Buffs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Buffs;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Barrage)
                return actionID;

            if (ActionReady(RagingStrikes))
                return RagingStrikes;
            if (ActionReady(BattleVoice))
                return BattleVoice;
            if (ActionReady(RadiantFinale))
                return RadiantFinale;

            return actionID;
        }
    }

    internal class BRD_OneButtonSongs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_OneButtonSongs;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not WanderersMinuet)
                return actionID;

            // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            int songTimerInSeconds = gauge.SongTimer / 1000;

            if (ActionReady(WanderersMinuet) || (gauge.Song == Song.WANDERER && songTimerInSeconds > 11))
                return WanderersMinuet;

            if (ActionReady(MagesBallad) || (gauge.Song == Song.MAGE && songTimerInSeconds > 2))
                return MagesBallad;

            if (ActionReady(ArmysPaeon) || (gauge.Song == Song.ARMY && songTimerInSeconds > 2))
                return ArmysPaeon;

            return actionID;
        }
    }
    #endregion

    #region Advanced Modes
    internal class BRD_AoE_AdvMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_AdvMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            bool canWeave = CanWeave() && !ActionWatching.HasDoubleWeaved();
            bool canWeaveDelayed = CanDelayedWeave(0.9) && !ActionWatching.HasDoubleWeaved();
            int songTimerInSeconds = gauge.SongTimer / 1000;
            bool songNone = gauge.Song == Song.NONE;
            bool songWanderer = gauge.Song == Song.WANDERER;
            bool songMage = gauge.Song == Song.MAGE;
            bool songArmy = gauge.Song == Song.ARMY;
            int targetHPThreshold = PluginConfiguration.GetCustomIntValue(Config.BRD_AoENoWasteHPPercentage);
            bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_AoE_Adv_NoWaste) || GetTargetHPPercent() > targetHPThreshold;
            bool hasTarget = HasBattleTarget();

            #region Variants

            if (IsEnabled(CustomComboPreset.BRD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BRD_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BRD_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;

            #endregion

            #region Songs
            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Songs))
            {

                // Limit optimisation to when you are high enough level to benefit from it.
                if (LevelChecked(WanderersMinuet))
                {
                    if (canWeave || !hasTarget)
                    {
                        if (songNone && InCombat())
                        {
                            // Logic to determine first song
                            if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                                return WanderersMinuet;
                            if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;
                            if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }

                        if (songWanderer)
                        {
                            if (songTimerInSeconds <= 3 && gauge.Repertoire > 0 && hasTarget) // Spend any repertoire before switching to next song
                                return OriginalHook(PitchPerfect);
                            if (songTimerInSeconds <= 3 && ActionReady(MagesBallad))          // Move to Mage's Ballad if <= 3 seconds left on song
                                return MagesBallad;
                        }

                        if (songMage)
                        {

                            // Move to Army's Paeon if < 3 seconds left on song
                            if (songTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                            {
                                // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                                if (ActionReady(EmpyrealArrow) && hasTarget)
                                    return EmpyrealArrow;
                                return ArmysPaeon;
                            }
                        }
                    }

                    if (songArmy && (canWeaveDelayed || !hasTarget))
                    {
                        // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                        if (songTimerInSeconds <= 12 || (ActionReady(WanderersMinuet) && gauge.Repertoire == 4))
                            return WanderersMinuet;
                    }
                }
                else if (songTimerInSeconds <= 3 && canWeaveDelayed)
                {
                    if (ActionReady(MagesBallad))
                        return MagesBallad;
                    if (ActionReady(ArmysPaeon))
                        return ArmysPaeon;
                }
            }
            #endregion

            #region Buffs

            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs) && (!songNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
            {
                float ragingCD = GetCooldownRemainingTime(RagingStrikes);

               // Radiant First with late weave for tighter grouping
                if (canWeaveDelayed && ActionReady(RadiantFinale) && ragingCD < 2.3 &&
                !HasEffect(Buffs.RadiantEncoreReady) &&
                (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (canWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (canWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice)))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (canWeave && ActionReady(Barrage) && HasEffect(Buffs.RagingStrikes) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;

            }

            #endregion

            #region OGCDS

            if (canWeave && IsEnabled(CustomComboPreset.BRD_AoE_Adv_oGCD))
            {

                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                // Pitch perfect logic. Uses when full, or at 2 stacks before Empy arrow to prevent overcap
                if (LevelChecked(PitchPerfect) && songWanderer &&
                    (gauge.Repertoire == 3 || (LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && GetCooldownRemainingTime(EmpyrealArrow) < 2)))
                    return OriginalHook(PitchPerfect);

                // Sidewinder Logic to stay in the buff window on 2 min, but on cd with the 1 min
                if (ActionReady(Sidewinder))
                {
                    if (songWanderer)
                    {
                        if ((HasEffect(Buffs.RagingStrikes) || GetCooldownRemainingTime(RagingStrikes) > 10) &&
                            (HasEffect(Buffs.BattleVoice) || GetCooldownRemainingTime(BattleVoice) > 10) &&
                            (HasEffect(Buffs.RadiantFinale) || GetCooldownRemainingTime(RadiantFinale) > 10 ||
                             !LevelChecked(RadiantFinale)))
                            return Sidewinder;
                    }
                    else
                        return Sidewinder;
                }
            }

            // Interupt Logic, set to delayed weave. Let someone else do it if they want. Better to be last line of defense and stay off cd.
            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && canWeaveDelayed)
                return All.HeadGraze;

            // Rain of death Logic
            if (canWeave && IsEnabled(CustomComboPreset.BRD_AoE_Adv_oGCD))
            {
                if (LevelChecked(RainOfDeath) && !WasLastAction(RainOfDeath) && (GetCooldownRemainingTime(EmpyrealArrow) > 1 || !LevelChecked(EmpyrealArrow)))
                {

                    uint rainOfDeathCharges = LevelChecked(RainOfDeath) ? GetRemainingCharges(RainOfDeath) : 0;

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Pooling) && LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        if (songWanderer) //Stop pooling for buff window
                        {
                            if (((HasEffect(Buffs.RagingStrikes) || GetCooldownRemainingTime(RagingStrikes) > 10) &&
                                 (HasEffect(Buffs.BattleVoice) || GetCooldownRemainingTime(BattleVoice) > 10 ||
                                  !LevelChecked(BattleVoice)) &&
                                 (HasEffect(Buffs.RadiantFinale) || GetCooldownRemainingTime(RadiantFinale) > 10 ||
                                  !LevelChecked(RadiantFinale)) &&
                                 rainOfDeathCharges > 0) || rainOfDeathCharges > 2)
                                return OriginalHook(RainOfDeath);
                        }

                        if (songArmy && (rainOfDeathCharges == 3 || ((gauge.SongTimer / 1000) > 30 && rainOfDeathCharges > 0))) //Start pooling in Armys
                            return OriginalHook(RainOfDeath);
                        if (songMage && rainOfDeathCharges > 0) // Dont poolin mages
                            return OriginalHook(RainOfDeath);
                        if (songNone && rainOfDeathCharges == 3) //Pool when no song
                            return OriginalHook(RainOfDeath);
                    }
                    else if (rainOfDeathCharges > 0) //Dont pool when not enabled
                        return OriginalHook(RainOfDeath);
                }
                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && GetRemainingCharges(Bloodletter) > 0))
                    return OriginalHook(Bloodletter);
            }

            #endregion

            #region Self Care

            if (canWeave)
            {
                if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind))
                {
                    if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.BRD_STSecondWindThreshold) && ActionReady(All.SecondWind))
                        return All.SecondWind;
                }

                if (IsEnabled(CustomComboPreset.BRD_ST_Wardens))
                {
                    if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer)) // Could be upgraded with a targetting system in the future
                        return OriginalHook(TheWardensPaeon);
                }
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(WideVolley);

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsEncore))
            {
                if (HasEffect(Buffs.RadiantEncoreReady) && GetBuffRemainingTime(Buffs.RadiantFinale) < 15) // Delay Encore enough for buff window
                    return OriginalHook(RadiantEncore);
            }

            if (IsEnabled(CustomComboPreset.BRD_ST_ApexArrow)) // Apex Logic to time song in buff window and in mages.
            {
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (LevelChecked(ApexArrow))
                {
                    if (songMage && gauge.SoulVoice == 100)
                        return ApexArrow;
                    if (songMage && gauge.SoulVoice >= 80 &&
                        songTimerInSeconds > 18 && songTimerInSeconds < 22)
                        return ApexArrow;
                    if (songWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                        (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                        return ApexArrow;
                }
            }

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsResonant))
            {
                if (HasEffect(Buffs.ResonantArrowReady))
                    return ResonantArrow;
            }

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_AdvMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_AdvMode;
        internal static bool usedStraightShotReady = false;
        internal static bool usedPitchPerfect = false;
        internal delegate bool DotRecast(int value);

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            bool canWeave = CanWeave() && !ActionWatching.HasDoubleWeaved();
            bool canWeaveDelayed = CanDelayedWeave(0.9) && !ActionWatching.HasDoubleWeaved();
            bool songNone = gauge.Song == Song.NONE;
            bool songWanderer = gauge.Song == Song.WANDERER;
            bool songMage = gauge.Song == Song.MAGE;
            bool songArmy = gauge.Song == Song.ARMY;
            int songTimerInSeconds = gauge.SongTimer / 1000;
            int targetHPThreshold = PluginConfiguration.GetCustomIntValue(Config.BRD_NoWasteHPPercentage);
            bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_Adv_NoWaste) || GetTargetHPPercent() > targetHPThreshold;
            bool hasTarget = HasBattleTarget();
            bool buffTime = GetCooldownRemainingTime(RagingStrikes) < 2.7;

            #region Variants

            if (IsEnabled(CustomComboPreset.BRD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BRD_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BRD_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;

            #endregion

            if (IsEnabled(CustomComboPreset.BRD_ST_Adv_Balance_Standard) &&
                Opener().FullOpener(ref actionID))
            {
                if (ActionWatching.GetAttackType(Opener().CurrentOpenerAction) != ActionWatching.ActionAttackType.Ability && canWeave)
                {
                    if (HasEffect(Buffs.RagingStrikes) && (gauge.Repertoire == 3 || (gauge.Repertoire == 2 && GetCooldownRemainingTime(EmpyrealArrow) < 2)))
                        return OriginalHook(PitchPerfect);

                    if (ActionReady(HeartbreakShot) && HasEffect(Buffs.RagingStrikes))
                        return HeartbreakShot;
                }

                return actionID;

            }

            #region Songs

            if (IsEnabled(CustomComboPreset.BRD_Adv_Song) && isEnemyHealthHigh)
            {

                // Limit optimisation to when you are high enough level to benefit from it.
                if (LevelChecked(WanderersMinuet))
                {
                    if (canWeave || !hasTarget)
                    {
                        if (songNone && InCombat())
                        {
                            // Logic to determine first song
                            if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                                return WanderersMinuet;
                            if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;
                            if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }

                        if (songWanderer)
                        {
                            if (songTimerInSeconds <= 3 && gauge.Repertoire > 0 && hasTarget) // Spend any repertoire before switching to next song
                                return OriginalHook(PitchPerfect);
                            if (songTimerInSeconds <= 3 && ActionReady(MagesBallad)) // Move to Mage's Ballad if <= 3 seconds left on song
                                return MagesBallad;
                        }

                        if (songMage)
                        {

                            // Move to Army's Paeon if <= 3 seconds left on song
                            if (songTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                            {
                                // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                                if (ActionReady(EmpyrealArrow) && hasTarget)
                                    return EmpyrealArrow;
                                return ArmysPaeon;
                            }
                        }
                    }

                    if (songArmy && (canWeaveDelayed || !hasTarget))
                    {
                        // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                        if (songTimerInSeconds <= 12 || (ActionReady(WanderersMinuet) && gauge.Repertoire == 4))
                            return WanderersMinuet;
                    }
                }

                else if (songTimerInSeconds <= 3 && canWeaveDelayed) // Before you get Wanderers, it just toggles the two songs.
                {
                    if (ActionReady(MagesBallad))
                        return MagesBallad;
                    if (ActionReady(ArmysPaeon))
                        return ArmysPaeon;
                }
            }

            #endregion

            #region Buffs

            if (IsEnabled(CustomComboPreset.BRD_Adv_Buffs) && (!songNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
            {
                float ragingCD = GetCooldownRemainingTime(RagingStrikes);

                // Radiant First with late weave for tighter grouping
                if (canWeaveDelayed && ActionReady(RadiantFinale) && ragingCD < 2.3 &&
                !HasEffect(Buffs.RadiantEncoreReady) &&
                (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (canWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (canWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice)))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (canWeave && ActionReady(Barrage) && HasEffect(Buffs.RagingStrikes) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;

            }

            #endregion

            #region OGCD

            if (canWeave && IsEnabled(CustomComboPreset.BRD_ST_Adv_oGCD) &&
                (!buffTime || !IsEnabled(CustomComboPreset.BRD_Adv_Buffs)))
            {
                // Pitch Perfect logic to use when full or when Empyreal arrow might overcap it.
                if (LevelChecked(PitchPerfect) && songWanderer &&
                    (gauge.Repertoire == 3 || (LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && GetCooldownRemainingTime(EmpyrealArrow) < 2)))
                    return OriginalHook(PitchPerfect);

                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                // Sidewinder logic to use in burst window with buffs or on cd on the 1 minutes
                if (ActionReady(Sidewinder))
                {
                    if (IsEnabled(CustomComboPreset.BRD_Adv_Pooling))
                    {
                        if (songWanderer)
                        {
                            if ((HasEffect(Buffs.RagingStrikes) || GetCooldownRemainingTime(RagingStrikes) > 10) &&
                                (HasEffect(Buffs.BattleVoice) || GetCooldownRemainingTime(BattleVoice) > 10) &&
                                (HasEffect(Buffs.RadiantFinale) || GetCooldownRemainingTime(RadiantFinale) > 10 ||
                                 !LevelChecked(RadiantFinale)))
                                return Sidewinder;
                        }
                        else
                            return Sidewinder;
                    }
                    else
                        return Sidewinder;
                }
            }
            //Interupt Logic, set to delayed weave. Let someone else do it if they want. Better to be last line of defense and stay off cd.
            if (IsEnabled(CustomComboPreset.BRD_Adv_Interrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && canWeaveDelayed)
                return All.HeadGraze;

            // Bloodletter pooling logic. Will Pool as buffs are coming up.
            if (canWeave && IsEnabled(CustomComboPreset.BRD_ST_Adv_oGCD))
            {
                if (ActionReady(Bloodletter) && !(WasLastAction(Bloodletter) || WasLastAction(HeartbreakShot)) && (GetCooldownRemainingTime(EmpyrealArrow) > 1 || !LevelChecked(EmpyrealArrow)))
                {
                    uint bloodletterCharges = GetRemainingCharges(Bloodletter);

                    if (IsEnabled(CustomComboPreset.BRD_Adv_Pooling) && LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        if (songWanderer) // Pool until buffs go out in wanderers
                        {
                            if (((HasEffect(Buffs.RagingStrikes) || GetCooldownRemainingTime(RagingStrikes) > 10) &&
                                 (HasEffect(Buffs.BattleVoice) || GetCooldownRemainingTime(BattleVoice) > 10 ||
                                  !LevelChecked(BattleVoice)) &&
                                 (HasEffect(Buffs.RadiantFinale) || GetCooldownRemainingTime(RadiantFinale) > 10 ||
                                  !LevelChecked(RadiantFinale)) &&
                                 bloodletterCharges > 0) || bloodletterCharges > 2)
                                return OriginalHook(Bloodletter);
                        }
                        if (songArmy && (bloodletterCharges == 3 || ((gauge.SongTimer / 1000) > 30 && bloodletterCharges > 0))) // Start pooling in Army
                            return OriginalHook(Bloodletter);
                        if (songMage && bloodletterCharges > 0) //Don't pool in Mages
                            return OriginalHook(Bloodletter);
                        if (songNone && bloodletterCharges == 3) //Pool with no song
                            return OriginalHook(Bloodletter);
                    }
                    else if (bloodletterCharges > 0)
                        return OriginalHook(Bloodletter);
                }
            }

            #endregion

            #region Self Care
            if (canWeave)
            {
                if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind))
                {
                    if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.BRD_STSecondWindThreshold) && ActionReady(All.SecondWind))
                        return All.SecondWind;
                }

                if (IsEnabled(CustomComboPreset.BRD_ST_Wardens))
                {
                    if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer)) // Could be upgraded with a targetting system in the future
                        return OriginalHook(TheWardensPaeon);
                }
            }
            #endregion

            #region Dot Management

            if (isEnemyHealthHigh)
            {
                bool canIronJaws = LevelChecked(IronJaws);
                Status? purple = FindTargetEffect(Debuffs.CausticBite) ?? FindTargetEffect(Debuffs.VenomousBite);
                Status? blue = FindTargetEffect(Debuffs.Stormbite) ?? FindTargetEffect(Debuffs.Windbite);
                float purpleRemaining = purple?.RemainingTime ?? 0;
                float blueRemaining = blue?.RemainingTime ?? 0;
                float ragingStrikesDuration = GetBuffRemainingTime(Buffs.RagingStrikes);
                int ragingJawsRenewTime = PluginConfiguration.GetCustomIntValue(Config.BRD_RagingJawsRenewTime);

                if (IsEnabled(CustomComboPreset.BRD_Adv_DoT))
                {
                    if (purple is not null && purpleRemaining < 4)
                        return canIronJaws ? IronJaws : VenomousBite;
                    if (blue is not null && blueRemaining < 4)
                        return canIronJaws ? IronJaws : Windbite;
                    if (blue is null && LevelChecked(Windbite))
                        return OriginalHook(Windbite);
                    if (purple is null && LevelChecked(VenomousBite))
                        return OriginalHook(VenomousBite);

                    if (IsEnabled(CustomComboPreset.BRD_Adv_RagingJaws) && ActionReady(IronJaws) && HasEffect(Buffs.RagingStrikes) &&
                        ragingStrikesDuration < ragingJawsRenewTime && // Raging Jaws Slider Check
                        purpleRemaining < 35 && blueRemaining < 35)    // Prevention of double refreshing dots
                    {
                        return IronJaws;
                    }
                }
            }
            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsEncore))
            {
                if (HasEffect(Buffs.RadiantEncoreReady) && HasEffect(Buffs.RagingStrikes)) // Delay Encore enough for buff window
                    return OriginalHook(RadiantEncore);
            }

            if (IsEnabled(CustomComboPreset.BRD_ST_ApexArrow)) // Apex Logic to time song in buff window and in mages.
            {
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (LevelChecked(ApexArrow))
                {
                    if (songMage && gauge.SoulVoice == 100)
                        return ApexArrow;
                    if (songMage && gauge.SoulVoice >= 80 &&
                        songTimerInSeconds > 18 && songTimerInSeconds < 22)
                        return ApexArrow;
                    if (songWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                        (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                        return ApexArrow;
                }
            }

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsResonant))
            {
                if (HasEffect(Buffs.ResonantArrowReady))
                    return ResonantArrow;
            }
            #endregion

            return actionID;
        }
    }
    #endregion

    #region Simple Modes
    internal class BRD_AoE_SimpleMode : CustomCombo
    {
        internal static bool inOpener = false;
        internal static bool openerFinished = false;

        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            bool canWeave = CanWeave() && !ActionWatching.HasDoubleWeaved();
            bool canWeaveDelayed = CanDelayedWeave(0.9) && !ActionWatching.HasDoubleWeaved();
            int songTimerInSeconds = gauge.SongTimer / 1000;
            bool songNone = gauge.Song == Song.NONE;
            bool songWanderer = gauge.Song == Song.WANDERER;
            bool songMage = gauge.Song == Song.MAGE;
            bool songArmy = gauge.Song == Song.ARMY;
            int targetHPThreshold = PluginConfiguration.GetCustomIntValue(Config.BRD_AoENoWasteHPPercentage);
            bool isEnemyHealthHigh = GetTargetHPPercent() > 5;
            bool hasTarget = HasBattleTarget();

            #region Variants

            if (IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= 50)
                return Variant.VariantCure;

            if (IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;

            #endregion

            #region Songs

            // Limit optimisation to when you are high enough level to benefit from it.
            if (LevelChecked(WanderersMinuet))
            {
                if (canWeave || !hasTarget)
                {
                    if (songNone && InCombat())
                    {
                        // Logic to determine first song
                        if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                            return WanderersMinuet;
                        if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                            return MagesBallad;
                        if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                            return ArmysPaeon;
                    }

                    if (songWanderer)
                    {
                        if (songTimerInSeconds <= 3 && gauge.Repertoire > 0 && hasTarget) // Spend any repertoire before switching to next song
                            return OriginalHook(PitchPerfect);
                        if (songTimerInSeconds <= 3 && ActionReady(MagesBallad))          // Move to Mage's Ballad if <= 3 seconds left on song
                            return MagesBallad;
                    }

                    if (songMage)
                    {

                        // Move to Army's Paeon if < 3 seconds left on song
                        if (songTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                        {
                            // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                            if (ActionReady(EmpyrealArrow) && hasTarget)
                                return EmpyrealArrow;
                            return ArmysPaeon;
                        }
                    }
                }

                if (songArmy && (canWeaveDelayed || !hasTarget))
                {
                    // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                    if ((songTimerInSeconds <= 12 || gauge.Repertoire == 4) && ActionReady(WanderersMinuet))
                        return WanderersMinuet;
                }
            }
            else if (songTimerInSeconds <= 3 && canWeaveDelayed) // Not high enough for wanderers Minuet yet
            {
                if (ActionReady(MagesBallad))
                    return MagesBallad;
                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            #endregion

            #region Buffs

            if ((!songNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
            {
                float ragingCD = GetCooldownRemainingTime(RagingStrikes);

                // Radiant First with late weave for tighter grouping
                if (canWeaveDelayed && ActionReady(RadiantFinale) && ragingCD < 2.3 &&
                !HasEffect(Buffs.RadiantEncoreReady) &&
                (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (canWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (canWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice)))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (canWeave && ActionReady(Barrage) && HasEffect(Buffs.RagingStrikes) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;

            }

            #endregion

            #region OGCDS and Selfcare

            if (canWeave)
            {
                float battleVoiceCD = GetCooldownRemainingTime(BattleVoice);
                float empyrealCD = GetCooldownRemainingTime(EmpyrealArrow);
                float ragingCD = GetCooldownRemainingTime(RagingStrikes);
                float radiantCD = GetCooldownRemainingTime(RadiantFinale);

                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                // Pitch Perfect logic to use when full or when Empy arrow can cause an overcap
                if (LevelChecked(PitchPerfect) && songWanderer &&
                    (gauge.Repertoire == 3 || (LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && empyrealCD < 2)))
                    return OriginalHook(PitchPerfect);

                // Sidewinder Logic to use in Window and on the 1 min
                if (ActionReady(Sidewinder))
                {
                    if (songWanderer)
                    {
                        if ((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10) &&
                            (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                             !LevelChecked(RadiantFinale)))
                            return Sidewinder;

                    }
                    else
                        return Sidewinder;
                }

                // Interupt
                if (CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && canWeaveDelayed)
                    return All.HeadGraze;

                // Pooling logic for rain of death basied on song
                if (LevelChecked(RainOfDeath) && !WasLastAction(RainOfDeath) && (empyrealCD > 1 || !LevelChecked(EmpyrealArrow)))
                {
                    uint rainOfDeathCharges = LevelChecked(RainOfDeath) ? GetRemainingCharges(RainOfDeath) : 0;

                    if (LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        if (songWanderer)
                        {
                            if (((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                                 (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10 ||
                                  !LevelChecked(BattleVoice)) &&
                                 (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                                  !LevelChecked(RadiantFinale)) &&
                                 rainOfDeathCharges > 0) || rainOfDeathCharges > 2)
                                return OriginalHook(RainOfDeath);
                        }

                        if (songArmy && (rainOfDeathCharges == 3 || ((gauge.SongTimer / 1000) > 30 && rainOfDeathCharges > 0)))
                            return OriginalHook(RainOfDeath);
                        if (songMage && rainOfDeathCharges > 0)
                            return OriginalHook(RainOfDeath);
                        if (songNone && rainOfDeathCharges == 3)
                            return OriginalHook(RainOfDeath);
                    }
                    else if (rainOfDeathCharges > 0)
                        return OriginalHook(RainOfDeath);
                }

                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && GetRemainingCharges(Bloodletter) > 0))
                    return OriginalHook(Bloodletter);

                // Self care section for healing and debuff removal

                if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }
            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))  //Ahead of other gcds because of higher risk of losing a proc than a ready buff
                return OriginalHook(WideVolley);

            if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100)
                return ApexArrow;

            if (HasEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.RadiantEncoreReady))
                return OriginalHook(RadiantEncore);

            #endregion

            return actionID;
        }
    }
    internal class BRD_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_SimpleMode;
        internal static bool usedStraightShotReady = false;
        internal static bool usedPitchPerfect = false;
        internal delegate bool DotRecast(int value);

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            BRDGauge? gauge = GetJobGauge<BRDGauge>();
            bool canWeave = CanWeave() && !ActionWatching.HasDoubleWeaved();
            bool canWeaveDelayed = CanDelayedWeave(0.9) && !ActionWatching.HasDoubleWeaved();
            bool songNone = gauge.Song == Song.NONE;
            bool songWanderer = gauge.Song == Song.WANDERER;
            bool songMage = gauge.Song == Song.MAGE;
            bool songArmy = gauge.Song == Song.ARMY;
            bool isEnemyHealthHigh = GetTargetHPPercent() > 1;
            int songTimerInSeconds = gauge.SongTimer / 1000;
            bool hasTarget = HasBattleTarget();

            #region Variants
            if (IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= 50)
                return Variant.VariantCure;

            if (IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;
            #endregion

            #region Songs

            // Limit optimisation to when you are high enough level to benefit from it.
            if (LevelChecked(WanderersMinuet))
            {
                // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army's Paeon

                if (ActionReady(EmpyrealArrow) && JustUsed(WanderersMinuet))
                    return EmpyrealArrow;

                if (canWeave || !hasTarget)
                {
                    if (songNone && InCombat())
                    {
                        // Logic to determine first song
                        if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                            return WanderersMinuet;
                        if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                            return MagesBallad;
                        if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                            return ArmysPaeon;
                    }

                    if (songWanderer)
                    {
                        if (songTimerInSeconds <= 3 && gauge.Repertoire > 0 && hasTarget) // Spend any repertoire before switching to next song
                            return OriginalHook(PitchPerfect);
                        if (songTimerInSeconds <= 3 && ActionReady(MagesBallad))          // Move to Mage's Ballad if <= 3 seconds left on song
                            return MagesBallad;
                    }

                    if (songMage)
                    {

                        // Move to Army's Paeon if <= 3 seconds left on song
                        if (songTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                        {
                            // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                            if (ActionReady(EmpyrealArrow) && hasTarget)
                                return EmpyrealArrow;
                            return ArmysPaeon;
                        }
                    }
                }

                if (songArmy && (canWeaveDelayed || !hasTarget))
                {
                    // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                    if (songTimerInSeconds <= 12 || (ActionReady(WanderersMinuet) && gauge.Repertoire == 4))
                        return WanderersMinuet;
                }
            }
            else if (songTimerInSeconds <= 3 && canWeaveDelayed)
            {
                if (ActionReady(MagesBallad))
                    return MagesBallad;
                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            #endregion

            #region Buffs

            if ((!songNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
            {
                float ragingCD = GetCooldownRemainingTime(RagingStrikes);

                // Radiant First with late weave for tighter grouping
                if (canWeaveDelayed && ActionReady(RadiantFinale) && ragingCD < 2.3 &&
                !HasEffect(Buffs.RadiantEncoreReady) &&
                (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (canWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (canWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice)))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (canWeave && ActionReady(Barrage) && HasEffect(Buffs.RagingStrikes) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;

            }

            #endregion

            #region OGCDS

            if (canWeave)
            {
                float battleVoiceCD = GetCooldownRemainingTime(BattleVoice);
                float empyrealCD = GetCooldownRemainingTime(EmpyrealArrow);
                float ragingCD = GetCooldownRemainingTime(RagingStrikes);
                float radiantCD = GetCooldownRemainingTime(RadiantFinale);

                // Empyreal Arrow first to minimize drift
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                //Pitch Perfect Logic to not let Empyreal arrow overcap
                if (LevelChecked(PitchPerfect) && songWanderer &&
                    (gauge.Repertoire == 3 || (LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && empyrealCD < 2)))
                    return OriginalHook(PitchPerfect);

                // Sidewinder Logic for burst window and 1 min
                if (ActionReady(Sidewinder))
                {
                    if (songWanderer)
                    {
                        if ((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10) &&
                            (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                             !LevelChecked(RadiantFinale)))
                            return Sidewinder;
                    }
                    else
                        return Sidewinder;
                }

                //Interupt delayered weave
                if (CanInterruptEnemy() && IsOffCooldown(All.HeadGraze) && canWeaveDelayed)
                    return All.HeadGraze;

                // Bloodletter pooling logic
                if (ActionReady(Bloodletter) && !(WasLastAction(Bloodletter) || WasLastAction(HeartbreakShot)) && (empyrealCD > 1 || !LevelChecked(EmpyrealArrow)))
                {
                    uint bloodletterCharges = GetRemainingCharges(Bloodletter);

                    if (LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        if (songWanderer) // Stop pooling in burst window
                        {
                            if (((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                                 (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10 ||
                                  !LevelChecked(BattleVoice)) &&
                                 (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                                  !LevelChecked(RadiantFinale)) &&
                                 bloodletterCharges > 0) || bloodletterCharges > 2)
                                return OriginalHook(Bloodletter);
                        }

                        if (songArmy && (bloodletterCharges == 3 || ((gauge.SongTimer / 1000) > 30 && bloodletterCharges > 0))) // Start pooling in army
                            return OriginalHook(Bloodletter);
                        if (songMage && bloodletterCharges > 0) // Dont pool in mages
                            return OriginalHook(Bloodletter);
                        if (songNone && bloodletterCharges == 3) // No song pooling
                            return OriginalHook(Bloodletter);
                    }
                    else if (bloodletterCharges > 0)
                        return OriginalHook(Bloodletter);
                }

                // Self Care

                if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }
            #endregion

            #region Dot Management

            if (isEnemyHealthHigh)
            {
                bool canIronJaws = LevelChecked(IronJaws);
                Status? purple = FindTargetEffect(Debuffs.CausticBite) ?? FindTargetEffect(Debuffs.VenomousBite);
                Status? blue = FindTargetEffect(Debuffs.Stormbite) ?? FindTargetEffect(Debuffs.Windbite);
                float purpleRemaining = purple?.RemainingTime ?? 0;
                float blueRemaining = blue?.RemainingTime ?? 0;
                float ragingStrikesDuration = GetBuffRemainingTime(Buffs.RagingStrikes);
                int ragingJawsRenewTime = 6;

                // Iron jaws Dot refresh, or low level manaul dot refresh
                if (purple is not null && purpleRemaining < 4)
                    return canIronJaws ? IronJaws : VenomousBite;
                if (blue is not null && blueRemaining < 4)
                    return canIronJaws ? IronJaws : Windbite;

                // Dot application
                if (blue is null && LevelChecked(Windbite))
                    return OriginalHook(Windbite);
                if (purple is null && LevelChecked(VenomousBite))
                    return OriginalHook(VenomousBite);

                // Raging jaws dot snapshotting logic
                if (ActionReady(IronJaws) && HasEffect(Buffs.RagingStrikes) &&
                ragingStrikesDuration < ragingJawsRenewTime && // Raging Jaws 
                purpleRemaining < 35 && blueRemaining < 35)    // Prevention of double refreshing dots
                {
                    return IronJaws;
                }
            }
            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (LevelChecked(ApexArrow)) //Apex Logic to use in the burst window and around the 1 min.
            {
                if (songMage && gauge.SoulVoice == 100)
                    return ApexArrow;
                if (songMage && gauge.SoulVoice >= 80 &&
                    songTimerInSeconds > 18 && songTimerInSeconds < 22)
                    return ApexArrow;
                if (songWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                    (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                    return ApexArrow;
            }

            if (HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.RadiantEncoreReady) && !JustUsed(RadiantFinale) && GetCooldownElapsed(BattleVoice) >= 4.2f)
                return OriginalHook(RadiantEncore);
            #endregion
            return actionID;
        }
    }
    #endregion
}
