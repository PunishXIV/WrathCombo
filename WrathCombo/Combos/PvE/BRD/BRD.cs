using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
namespace WrathCombo.Combos.PvE;

internal partial class BRD : PhysRangedJob
{
    #region Smaller features

    internal class BRD_StraightShotUpgrade : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_StraightShotUpgrade;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_DoTMaintainance) &&
                InCombat())
            {
                if (Purple is not null && PurpleRemaining < 4)
                    return CanIronJaws
                        ? IronJaws
                        : VenomousBite;

                if (Blue is not null && BlueRemaining < 4)
                    return CanIronJaws
                        ? IronJaws
                        : Windbite;
            }

            if (IsEnabled(CustomComboPreset.BRD_ApexST))
            {
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

            if (UseIronJaws())
                return IronJaws;

            if (ApplyBlueDot())
                return OriginalHook(Windbite);

            if (ApplyPurpleDot())
                return OriginalHook(VenomousBite);

            // Apex Option
            if (IsEnabled(CustomComboPreset.BRD_IronJawsApex))
            {
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

            if (UseIronJaws())
                return IronJaws;

            return LevelChecked(Windbite) && BlueRemaining <= PurpleRemaining ?
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

            if (IsEnabled(CustomComboPreset.BRD_AoE_oGCD_Songs) && (gauge.SongTimer < 1 || SongArmy))
            {
                if (ActionReady(WanderersMinuet))
                    return WanderersMinuet;

                if (ActionReady(MagesBallad))
                    return MagesBallad;

                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            if (ActionReady(EmpyrealArrow))
                return EmpyrealArrow;

            if (PitchPerfected())
                return OriginalHook(PitchPerfect);

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

            if (IsEnabled(CustomComboPreset.BRD_ST_oGCD_Songs) && (gauge.SongTimer < 1 || SongArmy))
            {
                if (ActionReady(WanderersMinuet))
                    return WanderersMinuet;

                if (ActionReady(MagesBallad))
                    return MagesBallad;

                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            if (PitchPerfected())
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

            if (ActionReady(WanderersMinuet) || gauge.Song == Song.Wanderer && SongTimerInSeconds > 11)
                return WanderersMinuet;

            if (ActionReady(MagesBallad) || gauge.Song == Song.Mage && SongTimerInSeconds > 2)
                return MagesBallad;

            if (ActionReady(ArmysPaeon) || gauge.Song == Song.Army && SongTimerInSeconds > 2)
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

            #region Variables

            int targetHPThreshold = Config.BRD_AoENoWasteHPPercentage;
            bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_AoE_Adv_NoWaste) || GetTargetHPPercent() > targetHPThreshold;
            bool ragingEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_Raging);
            bool battleVoiceEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_Battlevoice);
            bool barrageEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_Barrage);
            bool radiantEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_RadiantFinale);
            bool allBuffsEnabled = radiantEnabled && battleVoiceEnabled && ragingEnabled && barrageEnabled;
            #endregion

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, Config.BRD_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Songs

            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Songs) && isEnemyHealthHigh && InCombat() && (CanBardWeave || !BardHasTarget))
            {
                if (SongChangePitchPerfect())
                    return PitchPerfect;

                if (SongChangeEmpyreal())
                    return EmpyrealArrow;

                if (WandererSong())
                    return WanderersMinuet;

                if (MagesSong())
                    return MagesBallad;

                if (ArmySong())
                    return ArmysPaeon;

            }
            #endregion

            #region Buffs

            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs) && CanBardWeave && isEnemyHealthHigh)
            {
                if (allBuffsEnabled && !SongNone && LevelChecked(MagesBallad))
                {
                    if (UseRadiantBuff())
                        return RadiantFinale;

                    if (UseBattleVoiceBuff())
                        return BattleVoice;

                    if (UseRagingStrikesBuff())
                        return RagingStrikes;

                    if (UseBarrageBuff())
                        return Barrage;
                }

                if (!allBuffsEnabled || !LevelChecked(MagesBallad))
                {
                    if (ActionReady(RadiantFinale) && radiantEnabled)
                        return RadiantFinale;

                    if (ActionReady(BattleVoice) && battleVoiceEnabled)
                        return BattleVoice;

                    if (ActionReady(RagingStrikes) && ragingEnabled)
                        return RagingStrikes;

                    if (ActionReady(Barrage) && barrageEnabled)
                        return Barrage;
                }
            }

            #endregion

            #region OGCDS

            if (CanBardWeave && IsEnabled(CustomComboPreset.BRD_AoE_Adv_oGCD) &&
               (!BuffTime || !IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs)))
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                if (PitchPerfected())
                    return OriginalHook(PitchPerfect);

                if (ActionReady(Sidewinder) &&
                    (IsEnabled(CustomComboPreset.BRD_AoE_Pooling) && UsePooledSidewinder() || !IsEnabled(CustomComboPreset.BRD_AoE_Pooling)))
                    return Sidewinder;
            
                if (Role.CanHeadGraze(CustomComboPreset.BRD_AoE_Adv_Interrupt) && CanWeaveDelayed)
                    return Role.HeadGraze;

                if (ActionReady(RainOfDeath) &&
                    (IsEnabled(CustomComboPreset.BRD_AoE_Pooling) && UsePooledBloodRain() || !IsEnabled(CustomComboPreset.BRD_AoE_Pooling)))
                    return OriginalHook(RainOfDeath);

                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && BloodletterCharges > 0))
                    return OriginalHook(Bloodletter);
            }

            #endregion

            #region Self Care

            if (CanBardWeave)
            {
                if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind) && Role.CanSecondWind(Config.BRD_STSecondWindThreshold))
                    return Role.SecondWind;

                // Could be upgraded with a targetting system in the future
                if (IsEnabled(CustomComboPreset.BRD_ST_Wardens) && ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.Barrage))
                return OriginalHook(WideVolley);

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsEncore) && HasEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16)
                return OriginalHook(RadiantEncore);

            if (IsEnabled(CustomComboPreset.BRD_AoE_ApexArrow))
            {
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (IsEnabled(CustomComboPreset.BRD_AoE_ApexPooling) && UsePooledApex() || !IsEnabled(CustomComboPreset.BRD_AoE_ApexPooling) && gauge.SoulVoice == 100)
                    return ApexArrow;
            }

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsResonant))
            {
                if (HasEffect(Buffs.ResonantArrowReady))
                    return ResonantArrow;
            }

            if (HasEffect(Buffs.HawksEye))
                return OriginalHook(WideVolley);

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_AdvMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_AdvMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            #region Variables
            int targetHPThreshold = Config.BRD_NoWasteHPPercentage;
            int ragingJawsRenewTime = Config.BRD_RagingJawsRenewTime;
            bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_Adv_NoWaste) || GetTargetHPPercent() > targetHPThreshold;
            bool ragingEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_Raging);
            bool battleVoiceEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_Battlevoice);
            bool barrageEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_Barrage);
            bool radiantEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_RadiantFinale);
            bool allBuffsEnabled = radiantEnabled && battleVoiceEnabled && ragingEnabled && barrageEnabled;
            #endregion

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, Config.BRD_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Opener

            if (IsEnabled(CustomComboPreset.BRD_ST_Adv_Balance_Standard) &&
                Opener().FullOpener(ref actionID))
            {
                if (ActionWatching.GetAttackType(Opener().CurrentOpenerAction) != ActionWatching.ActionAttackType.Ability && CanBardWeave)
                {
                    if (HasEffect(Buffs.RagingStrikes) && (gauge.Repertoire == 3 || gauge.Repertoire == 2 && EmpyrealCD < 2))
                        return OriginalHook(PitchPerfect);

                    if (ActionReady(HeartbreakShot) && HasEffect(Buffs.RagingStrikes))
                        return HeartbreakShot;
                }

                return actionID;
            }
            #endregion

            #region Songs

            if (IsEnabled(CustomComboPreset.BRD_Adv_Song) && isEnemyHealthHigh && InCombat())
            {
                if (SongChangePitchPerfect())
                    return PitchPerfect;

                if (SongChangeEmpyreal())
                    return EmpyrealArrow;

                if (WandererSong())
                    return WanderersMinuet;

                if (MagesSong())
                    return MagesBallad;

                if (ArmySong())
                    return ArmysPaeon;

            }

            #endregion

            #region Buffs

            if (IsEnabled(CustomComboPreset.BRD_Adv_Buffs) && CanBardWeave && isEnemyHealthHigh)
            {
                if (allBuffsEnabled && !SongNone && LevelChecked(MagesBallad))
                {                    
                    if (UseRadiantBuff())
                        return RadiantFinale;

                    if (UseBattleVoiceBuff())
                        return BattleVoice;
                                        
                    if (UseRagingStrikesBuff())
                        return RagingStrikes;

                    if (UseBarrageBuff())
                        return Barrage;
                }

                if (!allBuffsEnabled || !LevelChecked(MagesBallad))
                {
                    if (ActionReady(RadiantFinale) && radiantEnabled)
                        return RadiantFinale;

                    if (ActionReady(BattleVoice) && battleVoiceEnabled)
                        return BattleVoice;

                    if (ActionReady(RagingStrikes) && ragingEnabled)
                        return RagingStrikes;

                    if (ActionReady(Barrage) && barrageEnabled)
                        return Barrage;
                }
            }
            #endregion

            #region OGCD

            if (CanBardWeave && IsEnabled(CustomComboPreset.BRD_ST_Adv_oGCD) &&
                (!BuffTime || !IsEnabled(CustomComboPreset.BRD_Adv_Buffs)))
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                if (PitchPerfected())
                    return OriginalHook(PitchPerfect);

                if (ActionReady(Sidewinder) && 
                    (IsEnabled(CustomComboPreset.BRD_Adv_Pooling) && UsePooledSidewinder() || !IsEnabled(CustomComboPreset.BRD_Adv_Pooling)))
                    return Sidewinder;

                if (Role.CanHeadGraze(CustomComboPreset.BRD_Adv_Interrupt) && CanWeaveDelayed)
                    return Role.HeadGraze;

                if (ActionReady(Bloodletter) &&
                    (IsEnabled(CustomComboPreset.BRD_Adv_Pooling) && UsePooledBloodRain() || !IsEnabled(CustomComboPreset.BRD_Adv_Pooling)))
                    return OriginalHook(Bloodletter);
            }

            #endregion

            #region Self Care

            if (CanBardWeave)
            {
                if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind) && Role.CanSecondWind(Config.BRD_STSecondWindThreshold))
                    return Role.SecondWind;

                if (IsEnabled(CustomComboPreset.BRD_ST_Wardens) && ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region Dot Management

            if (isEnemyHealthHigh)
            {
                if (IsEnabled(CustomComboPreset.BRD_Adv_DoT))
                {
                    if (IsEnabled(CustomComboPreset.BRD_Adv_IronJaws) && UseIronJaws())
                        return IronJaws;

                    if (IsEnabled(CustomComboPreset.BRD_Adv_ApplyDots))
                    {
                        if (ApplyBlueDot())
                            return OriginalHook(Windbite);

                        if (ApplyPurpleDot())
                            return OriginalHook(VenomousBite);
                    }   
                    if (IsEnabled(CustomComboPreset.BRD_Adv_RagingJaws) && RagingJawsRefresh() && RagingStrikesDuration < ragingJawsRenewTime)
                        return IronJaws;
                }
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsEncore) && HasEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16)
                return OriginalHook(RadiantEncore);

            if (IsEnabled(CustomComboPreset.BRD_ST_ApexArrow))
            {
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (IsEnabled(CustomComboPreset.BRD_Adv_ApexPooling) && UsePooledApex() || !IsEnabled(CustomComboPreset.BRD_Adv_ApexPooling) && gauge.SoulVoice == 100)
                    return ApexArrow;
            }

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsResonant) && HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.HawksEye))
                return OriginalHook(StraightShot);

            #endregion

            return actionID;
        }
    }

    #endregion

    #region Simple Modes

    internal class BRD_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, 50))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Songs

            // Limit optimisation to when you are high enough level to benefit from it.
            if (InCombat() && (CanBardWeave || !BardHasTarget))
            {
                if (SongChangePitchPerfect())
                    return PitchPerfect;

                if (SongChangeEmpyreal())
                    return EmpyrealArrow;

                if (WandererSong())
                    return WanderersMinuet;

                if (MagesSong())
                    return MagesBallad;

                if (ArmySong())
                    return ArmysPaeon;

            }

            #endregion

            #region Buffs

            if (CanBardWeave)
            {
                if (!SongNone && LevelChecked(MagesBallad))
                {
                    if (UseRadiantBuff())
                        return RadiantFinale;

                    if (UseBattleVoiceBuff())
                        return BattleVoice;

                    if (UseRagingStrikesBuff())
                        return RagingStrikes;

                    if (UseBarrageBuff())
                        return Barrage;
                }

                if (!LevelChecked(MagesBallad))
                {
                    if (ActionReady(RadiantFinale))
                        return RadiantFinale;

                    if (ActionReady(BattleVoice))
                        return BattleVoice;

                    if (ActionReady(RagingStrikes))
                        return RagingStrikes;

                    if (ActionReady(Barrage))
                        return Barrage;
                }
            }

            #endregion

            #region OGCDS and Selfcare

            if (CanBardWeave)
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                if (PitchPerfected())
                    return OriginalHook(PitchPerfect);

                if (ActionReady(Sidewinder) && UsePooledSidewinder())
                    return Sidewinder;

                if (Role.CanHeadGraze(CustomComboPreset.BRD_AoE_SimpleMode) && CanWeaveDelayed)
                    return Role.HeadGraze;

                if (ActionReady(RainOfDeath) && UsePooledBloodRain())
                    return OriginalHook(RainOfDeath);

                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && BloodletterCharges > 0))
                    return OriginalHook(Bloodletter);

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.Barrage))
                return OriginalHook(WideVolley);

            if (HasEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (UsePooledApex())
                return ApexArrow;

            if (HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16)
                return OriginalHook(RadiantEncore);

            if (HasEffect(Buffs.HawksEye))
                return OriginalHook(WideVolley);

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, 50))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Songs

            // Limit optimisation to when you are high enough level to benefit from it.
            if (InCombat() && (CanBardWeave || !BardHasTarget))
            {
                if (SongChangePitchPerfect())
                    return PitchPerfect;

                if (SongChangeEmpyreal())
                    return EmpyrealArrow;

                if (WandererSong())
                    return WanderersMinuet;

                if (MagesSong())
                    return MagesBallad;

                if (ArmySong())
                    return ArmysPaeon;

            }

            #endregion

            #region Buffs

            if (CanBardWeave)
            {
                if (!SongNone && LevelChecked(MagesBallad))
                {
                    if (UseRadiantBuff())
                        return RadiantFinale;

                    if (UseBattleVoiceBuff())
                        return BattleVoice;

                    if (UseRagingStrikesBuff())
                        return RagingStrikes;

                    if (UseBarrageBuff())
                        return Barrage;
                }

                if (!LevelChecked(MagesBallad))
                {
                    if (ActionReady(RadiantFinale))
                        return RadiantFinale;

                    if (ActionReady(BattleVoice))
                        return BattleVoice;

                    if (ActionReady(RagingStrikes))
                        return RagingStrikes;

                    if (ActionReady(Barrage))
                        return Barrage;
                }
            }

            #endregion

            #region OGCDS

            if (CanBardWeave)
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                if (PitchPerfected())
                    return OriginalHook(PitchPerfect);

                if (ActionReady(Sidewinder) && UsePooledSidewinder())
                    return Sidewinder;            
               
                if (Role.CanHeadGraze(CustomComboPreset.BRD_ST_SimpleMode) && CanWeaveDelayed)
                    return Role.HeadGraze;

                if (ActionReady(Bloodletter) && UsePooledBloodRain())
                    return OriginalHook(Bloodletter);

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region Dot Management

            if (UseIronJaws())
                return IronJaws;

            if (ApplyBlueDot())
                return OriginalHook(Windbite);

            if (ApplyPurpleDot())
                return OriginalHook(VenomousBite);

            if (RagingJawsRefresh() && RagingStrikesDuration < 6)
                return IronJaws;


            #endregion

            #region GCDS

            if (HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (HasEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (UsePooledApex())
                return ApexArrow;

            if (HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16)
                return OriginalHook(RadiantEncore);

            if (HasEffect(Buffs.HawksEye))
                return OriginalHook(StraightShot);

            #endregion

            return actionID;
        }
    }

    #endregion
}
