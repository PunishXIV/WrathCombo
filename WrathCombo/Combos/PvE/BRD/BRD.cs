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

            const Combo comboFlags = Combo.AoE | Combo.Simple;

            if (TryOGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
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

            const Combo comboFlags = Combo.ST | Combo.Simple;

            if (TryOGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
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

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion
            
            const Combo comboFlags = Combo.AoE | Combo.Adv;

            if (TryOGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
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

            const Combo comboFlags = Combo.ST | Combo.Adv;

            if (TryOGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
            if (TryGCDAttacks(comboFlags, ref actionID) && ActionReady(actionID))
                return actionID;
            
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

                if (ActionReady(OriginalHook(Bloodletter)) && BRD_StraightShotUpgrade_OGCDs_Options[2] &&
                    (BloodletterCharges == 3 && TraitLevelChecked(Traits.EnhancedBloodletter) ||
                    BloodletterCharges == 2 && !TraitLevelChecked(Traits.EnhancedBloodletter)))
                    return OriginalHook(Bloodletter);
            }

            if (IsEnabled(Preset.BRD_DoTMaintainance) &&
                InCombat())
            {
                if (ActionReady(IronJaws) && Purple is not null && Blue is not null && 
                    (PurpleRemaining < 4 || BlueRemaining < 4))
                    return IronJaws;
                
                if (ActionReady(OriginalHook(Windbite)) && DebuffCapCanBlue && 
                    (Blue is null || !CanIronJaws && BlueRemaining < 4))
                    return OriginalHook(Windbite);
                
                if (ActionReady(OriginalHook(VenomousBite)) && DebuffCapCanPurple &&
                    (Purple is null || !CanIronJaws && PurpleRemaining < 4))
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

            if (ActionReady(IronJaws) && Purple is not null && Blue is not null && 
                (PurpleRemaining < 4 || BlueRemaining < 4))
                return IronJaws;
            
            if (ActionReady(OriginalHook(Windbite)) && DebuffCapCanBlue && 
                (Blue is null || !CanIronJaws && BlueRemaining < 4))
                return OriginalHook(Windbite);
            
            if (ActionReady(OriginalHook(VenomousBite)) && DebuffCapCanPurple &&
                (Purple is null || !CanIronJaws && PurpleRemaining < 4))
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

    internal class BRD_One_Button_Dot : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRD_OneButtonDots;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not VenomousBite and not CausticBite)
                return actionID;

            bool retargeted = IsEnabled(Preset.BRD_OneButtonDots_Retargeted);
            bool ironJaws = IsEnabled(Preset.BRD_OneButtonDots_IronJaws);
            bool savage = IsEnabled(Preset.BRD_OneButtonDots_SavageBlade);
            var blueDotAction = OriginalHook(Windbite);
            var purpleDotAction = OriginalHook(VenomousBite);
            BlueList.TryGetValue(blueDotAction, out var blueDotDebuffID);
            PurpleList.TryGetValue(purpleDotAction, out var purpleDotDebuffID);

            if (!retargeted)
            {
                var purpleDotRemaining = GetStatusEffectRemainingTime(purpleDotDebuffID, CurrentTarget);
                var blueDotRemaining = GetStatusEffectRemainingTime(blueDotDebuffID, CurrentTarget);

                if (ironJaws && purpleDotRemaining > 0 && blueDotRemaining > 0 && ActionReady(IronJaws))
                    return IronJaws;

                if (purpleDotRemaining <= blueDotRemaining && ActionReady(purpleDotAction))
                    return purpleDotAction;

                if (blueDotRemaining <= purpleDotRemaining && ActionReady(blueDotAction))
                    return blueDotAction;
            }
            else
            {
                var lowestPurple = SimpleTarget.TargetWithDoTLowestRemainingTimer(purpleDotAction, purpleDotDebuffID);
                var lowestBlue = SimpleTarget.TargetWithDoTLowestRemainingTimer(blueDotAction, blueDotDebuffID);
                var lowestPurpleRemaining = GetStatusEffectRemainingTime(purpleDotDebuffID, lowestPurple);
                var lowestBlueRemaining = GetStatusEffectRemainingTime(blueDotDebuffID, lowestBlue);

                var purpleDotTarget = SimpleTarget.DottableEnemy(purpleDotAction, purpleDotDebuffID, maxNumberOfEnemiesInRange: 99);
                var blueDotTarget = SimpleTarget.DottableEnemy(blueDotAction, blueDotDebuffID, maxNumberOfEnemiesInRange: 99);

                if (ironJaws && InCombat() && purpleDotTarget == null && blueDotTarget == null && ActionReady(IronJaws))
                {
                    if ((!savage) || (savage && lowestBlueRemaining <= 5 || lowestPurpleRemaining <= 5))
                    {
                        if (lowestPurpleRemaining <= lowestBlueRemaining)
                            return IronJaws.Retarget([CausticBite, VenomousBite], lowestPurple);
                        else
                            return IronJaws.Retarget([CausticBite, VenomousBite], lowestBlue);
                    }
                    else
                        return All.SavageBlade;
                }

                if (purpleDotTarget != null && ActionReady(purpleDotAction))
                    return purpleDotAction.Retarget([CausticBite, VenomousBite], purpleDotTarget);

                if (blueDotTarget != null && ActionReady(blueDotAction))
                    return blueDotAction.Retarget([CausticBite, VenomousBite], blueDotTarget);

                if (lowestPurple != null && lowestBlue != null)
                {
                    if ((!savage) || (savage && lowestPurpleRemaining <= 5 || lowestBlueRemaining <= 5))
                    {
                        if (lowestPurpleRemaining <= lowestBlueRemaining)
                            return purpleDotAction.Retarget([CausticBite, VenomousBite], lowestPurple);
                        else
                            return blueDotAction.Retarget([CausticBite, VenomousBite], lowestBlue);
                    }
                    else
                        return All.SavageBlade;
                }
            }

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

            if (ActionReady(OriginalHook(Bloodletter)))
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

                if (ActionReady(OriginalHook(Bloodletter)) && BRD_WideVolleyUpgrade_OGCDs_Options[2] &&
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

            if (ActionReady(WanderersMinuet) || gauge.Song == Song.WanderersMinuet && SongTimerInSeconds > 11)
                return WanderersMinuet;

            if (ActionReady(MagesBallad) || gauge.Song == Song.MagesBallad && SongTimerInSeconds > 2)
                return MagesBallad;

            if (ActionReady(ArmysPaeon) || gauge.Song == Song.ArmysPaeon && SongTimerInSeconds > 2)
                return ArmysPaeon;

            return actionID;
        }
    }
    #endregion
}
