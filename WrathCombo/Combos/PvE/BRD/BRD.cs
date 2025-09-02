using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.BRD.Config;
namespace WrathCombo.Combos.PvE;

internal partial class BRD : PhysicalRanged
{
    #region Simple Modes
    internal class BRD_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Songs
            // Limit optimization to when you are high enough level to benefit from it.
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

                if (Role.CanHeadGraze(true, WeaveTypes.DelayWeave))
                    return Role.HeadGraze;

                if (ActionReady(RainOfDeath) && UsePooledBloodRain())
                    return OriginalHook(RainOfDeath);

                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && BloodletterCharges > 0))
                    return OriginalHook(Bloodletter);

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(TheWardensPaeon))
                {
                    if (HasCleansableDebuff(LocalPlayer))
                        return TheWardensPaeon;

                    else if (WardenResolver() is not null)
                        return TheWardensPaeon.Retarget([Ladonsbite, QuickNock], WardenResolver);
                }
            }
            #endregion

            #region GCDS
            if (HasStatusEffect(Buffs.Barrage))
                return OriginalHook(WideVolley);

            if (HasStatusEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (UsePooledApex())
                return ApexArrow;

            if (HasStatusEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasStatusEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16 && HasStatusEffect(Buffs.RagingStrikes))
                return OriginalHook(RadiantEncore);

            if (HasStatusEffect(Buffs.HawksEye) && ActionReady(WideVolley))
                return OriginalHook(WideVolley);

            #region Multidot Management

            #region Dottable Variables
            var blueDotAction = OriginalHook(Windbite);
            var purpleDotAction = OriginalHook(VenomousBite);
            BlueList.TryGetValue(blueDotAction, out var blueDotDebuffID);
            PurpleList.TryGetValue(purpleDotAction, out var purpleDotDebuffID);
            var ironTarget = SimpleTarget.BardRefreshableEnemy(IronJaws, blueDotDebuffID, purpleDotDebuffID, 50, 6);
            var blueTarget = SimpleTarget.DottableEnemy(blueDotAction, blueDotDebuffID, 50, 0);
            var purpleTarget = SimpleTarget.DottableEnemy(purpleDotAction, purpleDotDebuffID, 50, 0);
            #endregion

            if (ironTarget is not null && LevelChecked(IronJaws))
                return IronJaws.Retarget([Ladonsbite, QuickNock], ironTarget);

            if (blueTarget is not null && LevelChecked(Windbite))
                return blueDotAction.Retarget([Ladonsbite, QuickNock], blueTarget);

            if (purpleTarget is not null && LevelChecked(VenomousBite))
                return purpleDotAction.Retarget([Ladonsbite, QuickNock], purpleTarget);

            #endregion

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_ST_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Songs
            // Limit optimization to when you are high enough level to benefit from it.
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

                if (Role.CanHeadGraze(true, WeaveTypes.DelayWeave))
                    return Role.HeadGraze;

                if (ActionReady(Bloodletter) && UsePooledBloodRain())
                    return OriginalHook(Bloodletter);

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(TheWardensPaeon))
                {
                    if (HasCleansableDebuff(LocalPlayer))
                        return TheWardensPaeon;
                    else if (WardenResolver() is not null)
                        return TheWardensPaeon.Retarget([HeavyShot, BurstShot], WardenResolver);
                }
            }
            #endregion

            #region Primary Dot Management

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
            if (HasStatusEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (HasStatusEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (UsePooledApex())
                return ApexArrow;

            if (HasStatusEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasStatusEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16 && HasStatusEffect(Buffs.RagingStrikes))
                return OriginalHook(RadiantEncore);

            if (HasStatusEffect(Buffs.HawksEye))
                return OriginalHook(StraightShot);

            #region Multidot Management

            #region Dottable Variables
            var blueDotAction = OriginalHook(Windbite);
            var purpleDotAction = OriginalHook(VenomousBite);
            BlueList.TryGetValue(blueDotAction, out var blueDotDebuffID);
            PurpleList.TryGetValue(purpleDotAction, out var purpleDotDebuffID);
            var ironTarget = SimpleTarget.BardRefreshableEnemy(IronJaws, blueDotDebuffID, purpleDotDebuffID, 50, 4);
            var blueTarget = SimpleTarget.DottableEnemy(blueDotAction, blueDotDebuffID, 50, 0);
            var purpleTarget = SimpleTarget.DottableEnemy(purpleDotAction, purpleDotDebuffID, 50, 0);
            #endregion

            if (ironTarget is not null && LevelChecked(IronJaws))
                return IronJaws.Retarget([HeavyShot, BurstShot], ironTarget);

            if (blueTarget is not null && LevelChecked(Windbite))
                return blueDotAction.Retarget([HeavyShot, BurstShot], blueTarget);

            if (purpleTarget is not null && LevelChecked(VenomousBite))
                return purpleDotAction.Retarget([HeavyShot, BurstShot], purpleTarget);

            #endregion

            #endregion

            return actionID;
        }
    }
    #endregion

    #region Advanced Modes
    internal class BRD_AoE_AdvMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_AoE_AdvMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            #region Variables
            bool ragingEnabled = BRD_AoE_Adv_Buffs_Options[0];
            bool battleVoiceEnabled = BRD_AoE_Adv_Buffs_Options[1];
            bool barrageEnabled = BRD_AoE_Adv_Buffs_Options[2];
            bool radiantEnabled = BRD_AoE_Adv_Buffs_Options[3];
            bool allBuffsEnabled = radiantEnabled && battleVoiceEnabled && ragingEnabled && barrageEnabled;
            int buffThreshold = BRD_AoE_Adv_Buffs_SubOption == 1 || !InBossEncounter() ? BRD_AoE_Adv_Buffs_Threshold : 0;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Songs
            if (IsEnabled(Preset.BRD_AoE_Adv_Songs) && InCombat() && (CanBardWeave || !BardHasTarget))
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
            if (IsEnabled(Preset.BRD_AoE_Adv_Buffs) && CanBardWeave && GetTargetHPPercent() > buffThreshold)
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
            if (CanBardWeave && IsEnabled(Preset.BRD_AoE_Adv_oGCD) &&
               (!BuffTime || !IsEnabled(Preset.BRD_AoE_Adv_Buffs)))
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                if (PitchPerfected())
                    return OriginalHook(PitchPerfect);

                if (ActionReady(Sidewinder) &&
                    (IsEnabled(Preset.BRD_AoE_Pooling) && UsePooledSidewinder() || !IsEnabled(Preset.BRD_AoE_Pooling)))
                    return Sidewinder;

                if (Role.CanHeadGraze(Preset.BRD_AoE_Adv_Interrupt, WeaveTypes.DelayWeave))
                    return Role.HeadGraze;

                if (ActionReady(RainOfDeath) &&
                    (IsEnabled(Preset.BRD_AoE_Pooling) && UsePooledBloodRain() || !IsEnabled(Preset.BRD_AoE_Pooling)))
                    return OriginalHook(RainOfDeath);

                if (!LevelChecked(RainOfDeath) && !WasLastAction(Bloodletter) && BloodletterCharges > 0)
                    return OriginalHook(Bloodletter);
            }
            #endregion

            #region Self Care
            if (CanBardWeave)
            {
                if (IsEnabled(Preset.BRD_AoE_SecondWind) && Role.CanSecondWind(BRD_STSecondWindThreshold))
                    return Role.SecondWind;

                if (IsEnabled(Preset.BRD_AoE_Wardens) && ActionReady(TheWardensPaeon))
                {
                    if (HasCleansableDebuff(LocalPlayer))
                        return TheWardensPaeon;

                    if (BRD_AoE_Wardens_Auto && WardenResolver() is not null)
                        return TheWardensPaeon.Retarget([Ladonsbite, QuickNock], WardenResolver);
                }
            }
            #endregion

            #region GCDS
            if (HasStatusEffect(Buffs.Barrage))
                return OriginalHook(WideVolley);

            if (IsEnabled(Preset.BRD_AoE_BuffsEncore) && HasStatusEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16 &&
               (HasStatusEffect(Buffs.RagingStrikes) || !ragingEnabled))
                return OriginalHook(RadiantEncore);

            if (IsEnabled(Preset.BRD_AoE_ApexArrow))
            {
                if (HasStatusEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (IsEnabled(Preset.BRD_AoE_ApexPooling) && UsePooledApex() || !IsEnabled(Preset.BRD_AoE_ApexPooling) && gauge.SoulVoice == 100)
                    return ApexArrow;
            }

            if (IsEnabled(Preset.BRD_AoE_BuffsResonant))
            {
                if (HasStatusEffect(Buffs.ResonantArrowReady))
                    return ResonantArrow;
            }

            if (HasStatusEffect(Buffs.HawksEye) && ActionReady(WideVolley))
                return OriginalHook(WideVolley);

            #region Multidot Management
            if (IsEnabled(Preset.BRD_AoE_Adv_Multidot))
            {
                #region Dottable Variables
                var blueDotAction = OriginalHook(Windbite);
                var purpleDotAction = OriginalHook(VenomousBite);
                BlueList.TryGetValue(blueDotAction, out var blueDotDebuffID);
                PurpleList.TryGetValue(purpleDotAction, out var purpleDotDebuffID);
                var ironTarget = SimpleTarget.BardRefreshableEnemy(IronJaws, blueDotDebuffID, purpleDotDebuffID, BRD_AoE_Adv_Multidot_HPThreshold, 6);
                var blueTarget = SimpleTarget.DottableEnemy(blueDotAction, blueDotDebuffID, BRD_AoE_Adv_Multidot_HPThreshold, 0);
                var purpleTarget = SimpleTarget.DottableEnemy(purpleDotAction, purpleDotDebuffID, BRD_AoE_Adv_Multidot_HPThreshold, 0);
                #endregion

                if (ironTarget is not null && LevelChecked(IronJaws))
                    return IronJaws.Retarget([Ladonsbite, QuickNock], ironTarget);

                if (blueTarget is not null && LevelChecked(Windbite))
                    return blueDotAction.Retarget([Ladonsbite, QuickNock], blueTarget);

                if (purpleTarget is not null && LevelChecked(VenomousBite))
                    return purpleDotAction.Retarget([Ladonsbite, QuickNock], purpleTarget);


            }
            #endregion

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_AdvMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_ST_AdvMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            #region Variables
            int ragingJawsRenewTime = BRD_RagingJawsRenewTime;
            bool ragingEnabled = BRD_Adv_Buffs_Options[0];
            bool battleVoiceEnabled = BRD_Adv_Buffs_Options[1];
            bool barrageEnabled = BRD_Adv_Buffs_Options[2];
            bool radiantEnabled = BRD_Adv_Buffs_Options[3];
            bool allBuffsEnabled = radiantEnabled && battleVoiceEnabled && ragingEnabled && barrageEnabled;
            int dotThreshold = BRD_Adv_DoT_SubOption == 1 || !InBossEncounter() ? BRD_Adv_DoT_Threshold : 0;
            int buffThreshold = BRD_Adv_Buffs_SubOption == 1 || !InBossEncounter() ? BRD_Adv_Buffs_Threshold : 0;
            #endregion

            #region Opener
            if (IsEnabled(Preset.BRD_ST_Adv_Balance_Standard) && HasBattleTarget() &&
                Opener().FullOpener(ref actionID))
            {
                if (ActionWatching.GetAttackType(Opener().CurrentOpenerAction) != ActionWatching.ActionAttackType.Ability && CanBardWeave)
                {
                    if (HasStatusEffect(Buffs.RagingStrikes) && (gauge.Repertoire == 3 || gauge.Repertoire == 2 && EmpyrealCD < 2))
                        return OriginalHook(PitchPerfect);

                    if (ActionReady(HeartbreakShot) && HasStatusEffect(Buffs.RagingStrikes))
                        return HeartbreakShot;
                }

                return actionID;
            }
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Songs
            if (IsEnabled(Preset.BRD_Adv_Song) && InCombat())
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
            if (IsEnabled(Preset.BRD_Adv_Buffs) && CanBardWeave && GetTargetHPPercent() > buffThreshold)
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
            if (CanBardWeave && IsEnabled(Preset.BRD_ST_Adv_oGCD) &&
                (!BuffTime || !IsEnabled(Preset.BRD_Adv_Buffs)))
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                if (PitchPerfected())
                    return OriginalHook(PitchPerfect);

                if (ActionReady(Sidewinder) &&
                    (IsEnabled(Preset.BRD_Adv_Pooling) && UsePooledSidewinder() || !IsEnabled(Preset.BRD_Adv_Pooling)))
                    return Sidewinder;

                if (Role.CanHeadGraze(Preset.BRD_Adv_Interrupt, WeaveTypes.DelayWeave))
                    return Role.HeadGraze;

                if (ActionReady(Bloodletter) &&
                    (IsEnabled(Preset.BRD_Adv_Pooling) && UsePooledBloodRain() || !IsEnabled(Preset.BRD_Adv_Pooling)))
                    return OriginalHook(Bloodletter);
                
                if (ActionReady(Troubadour) &&
                    IsEnabled(Preset.BRD_Adv_Troubadour) && !RaidWideCasting() && 
                    NumberOfAlliesInRange(Troubadour) >= GetPartyMembers().Count * .75 &&
                    !HasAnyStatusEffects ([Buffs.Troubadour, DNC.Buffs.ShieldSamba, MCH.Buffs.Tactician, Buffs.WanderersMinuet], anyOwner: true))
                    return Troubadour;
                
                if (ActionReady(NaturesMinne) &&
                   IsEnabled(Preset.BRD_Adv_NaturesMinne) && !RaidWideCasting() && 
                   NumberOfAlliesInRange(NaturesMinne) >= GetPartyMembers().Count * .75 &&
                   !HasAnyStatusEffects ([Buffs.Troubadour, Buffs.NaturesMinne, Buffs.WanderersMinuet], anyOwner: true))
                    return NaturesMinne;
            }
            #endregion

            #region Self Care
            if (CanBardWeave || !InCombat())
            {
                if (IsEnabled(Preset.BRD_ST_SecondWind) && Role.CanSecondWind(BRD_STSecondWindThreshold))
                    return Role.SecondWind;

                if (IsEnabled(Preset.BRD_ST_Wardens) && ActionReady(TheWardensPaeon))
                {
                    if (HasCleansableDebuff(LocalPlayer))
                        return TheWardensPaeon;

                    else if (BRD_ST_Wardens_Auto && WardenResolver() is not null)
                        return TheWardensPaeon.Retarget([HeavyShot, BurstShot], WardenResolver);
                }
            }
            #endregion

            #region Dot Management
            if (IsEnabled(Preset.BRD_Adv_DoT) && GetTargetHPPercent() > dotThreshold)
            {
                if (BRD_Adv_DoT_Options[0] && UseIronJaws())
                    return IronJaws;

                if (BRD_Adv_DoT_Options[1])
                {
                    if (ApplyBlueDot())
                        return OriginalHook(Windbite);

                    if (ApplyPurpleDot())
                        return OriginalHook(VenomousBite);
                }
                if (BRD_Adv_DoT_Options[2] && RagingJawsRefresh() && RagingStrikesDuration < ragingJawsRenewTime)
                    return IronJaws;
            }
            #endregion

            #region GCDS
            if (HasStatusEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (IsEnabled(Preset.BRD_Adv_BuffsEncore) && HasStatusEffect(Buffs.RadiantEncoreReady) && RadiantFinaleDuration < 16 &&
               (HasStatusEffect(Buffs.RagingStrikes) || !ragingEnabled))
                return OriginalHook(RadiantEncore);

            if (IsEnabled(Preset.BRD_ST_ApexArrow))
            {
                if (HasStatusEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (IsEnabled(Preset.BRD_Adv_ApexPooling) && UsePooledApex() || !IsEnabled(Preset.BRD_Adv_ApexPooling) && gauge.SoulVoice == 100)
                    return ApexArrow;
            }

            if (IsEnabled(Preset.BRD_Adv_BuffsResonant) && HasStatusEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasStatusEffect(Buffs.HawksEye))
                return OriginalHook(StraightShot);

            #region Multidot Management
            if (BRD_Adv_DoT_Options[3])
            {
                #region Dottable Variables

                var blueDotAction = OriginalHook(Windbite);
                var purpleDotAction = OriginalHook(VenomousBite);
                BlueList.TryGetValue(blueDotAction, out var blueDotDebuffID);
                PurpleList.TryGetValue(purpleDotAction, out var purpleDotDebuffID);
                var ironTarget = SimpleTarget.BardRefreshableEnemy(IronJaws, blueDotDebuffID, purpleDotDebuffID, dotThreshold, 4);
                var blueTarget = SimpleTarget.DottableEnemy(blueDotAction, blueDotDebuffID, dotThreshold, 0);
                var purpleTarget = SimpleTarget.DottableEnemy(purpleDotAction, purpleDotDebuffID, dotThreshold, 0);

                #endregion

                if (ironTarget is not null && LevelChecked(IronJaws))
                    return IronJaws.Retarget([HeavyShot, BurstShot], ironTarget);

                if (blueTarget is not null && LevelChecked(Windbite))
                    return blueDotAction.Retarget([HeavyShot, BurstShot], blueTarget);

                if (purpleTarget is not null && LevelChecked(VenomousBite))
                    return purpleDotAction.Retarget([HeavyShot, BurstShot], purpleTarget);
            }
            #endregion
            #endregion

            return actionID;
        }
    }
    #endregion

    #region Smaller features
    internal class BRD_StraightShotUpgrade : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_StraightShotUpgrade;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (StraightShot or RefulgentArrow))
                return actionID;
            
            if (CanBardWeave && IsEnabled(Preset.BRD_StraightShotUpgrade_OGCDs))
            {
                if (ActionReady(EmpyrealArrow) && BRD_StraightShotUpgrade_OGCDs_Options[0])
                    return EmpyrealArrow;
                
                if (PitchPerfected() && BRD_StraightShotUpgrade_OGCDs_Options[1])
                    return OriginalHook(PitchPerfect);
                
                if (ActionReady(Sidewinder) && BRD_StraightShotUpgrade_OGCDs_Options[3])
                    return Sidewinder;

                if (ActionReady(Bloodletter) && BRD_StraightShotUpgrade_OGCDs_Options[2] && 
                    (BloodletterCharges == 3 && TraitLevelChecked(Traits.EnhancedBloodletter) || 
                    BloodletterCharges == 2 && !TraitLevelChecked(Traits.EnhancedBloodletter)))
                    return OriginalHook(Bloodletter);
            }

            if (IsEnabled(Preset.BRD_DoTMaintainance) &&
                InCombat())
            {
                if (UseIronJaws())
                    return IronJaws;
                if (ApplyBlueDot())
                    return OriginalHook(Windbite);
                if (ApplyPurpleDot())
                    return OriginalHook(VenomousBite);
            }

            return HasStatusEffect(Buffs.HawksEye) || HasStatusEffect(Buffs.Barrage)
                ? actionID
                : OriginalHook(HeavyShot);
        }
    }
    
    internal class BRD_IronJaws : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_IronJaws;
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
            if (BRD_IronJaws_Apex)
            {
                if (LevelChecked(BlastArrow) && HasStatusEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (gauge.SoulVoice == 100)
                    return ApexArrow;
            }
            
            if (BRD_IronJaws_Alternate)
                return LevelChecked(Windbite) && BlueRemaining <= PurpleRemaining ?
                    OriginalHook(Windbite) :
                    OriginalHook(VenomousBite);
            
            return actionID;
        }
    }
    
    internal class BRD_AoE_oGCD : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_AoE_oGCD;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not RainOfDeath)
                return actionID;

            if (IsEnabled(Preset.BRD_AoE_oGCD_Songs) && (gauge.SongTimer < 1 || SongArmy))
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
        protected internal override Preset Preset => Preset.BRD_ST_oGCD;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bloodletter or HeartbreakShot))
                return actionID;

            if (IsEnabled(Preset.BRD_ST_oGCD_Songs) && (gauge.SongTimer < 1 || SongArmy))
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
        protected internal override Preset Preset => Preset.BRD_WideVolleyUpgrade;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (WideVolley or Shadowbite))
                return actionID;
            
            if (CanBardWeave && IsEnabled(Preset.BRD_WideVolleyUpgrade_OGCDs))
            {
                if (ActionReady(EmpyrealArrow) && BRD_WideVolleyUpgrade_OGCDs_Options[0])
                    return EmpyrealArrow;
                
                if (PitchPerfected() && BRD_WideVolleyUpgrade_OGCDs_Options[1])
                    return OriginalHook(PitchPerfect);
                
                if (ActionReady(Sidewinder) && BRD_WideVolleyUpgrade_OGCDs_Options[3])
                    return Sidewinder;

                if (ActionReady(Bloodletter) && BRD_WideVolleyUpgrade_OGCDs_Options[2] && 
                    (BloodletterCharges == 3 && TraitLevelChecked(Traits.EnhancedBloodletter) || 
                     BloodletterCharges == 2 && !TraitLevelChecked(Traits.EnhancedBloodletter)))
                    return LevelChecked(RainOfDeath)
                        ? RainOfDeath
                        : OriginalHook(Bloodletter);
            }

            if (IsEnabled(Preset.BRD_WideVolleyUpgrade_Apex))
            {
                if (gauge.SoulVoice == 100)
                    return ApexArrow;

                if (HasStatusEffect(Buffs.BlastArrowReady))
                    return BlastArrow;
            }
            
            return LevelChecked(WideVolley) && (HasStatusEffect(Buffs.HawksEye) || HasStatusEffect(Buffs.Barrage))
                ? actionID
                : OriginalHook(QuickNock);

        }
    }
    
    internal class BRD_Buffs : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_Buffs;
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
        protected internal override Preset Preset => Preset.BRD_OneButtonSongs;
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
}
