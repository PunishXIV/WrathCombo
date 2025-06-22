using System;
using System.Linq;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using ContentHelper = ECommons.GameHelpers;
using IntendedUse = ECommons.ExcelServices.TerritoryIntendedUseEnum;


namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    public const byte JobID = 100;
    internal static uint BestPhantomAction()
    {
        if (!IsInOccult)
            return 0; //not in Occult Crescent

        #region Bard
        if (IsEnabled(CustomComboPreset.Phantom_Bard) &&
            CanWeave())
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_HerosRime, HerosRime))
                return HerosRime; //burst song
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_OffensiveAria, OffensiveAria) && !HasStatusEffect(Buffs.OffensiveAria) && !HasStatusEffect(Buffs.HerosRime, anyOwner: true))
                return OffensiveAria; //off-song
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_RomeosBallad, RomeosBallad) && CanInterruptEnemy())
                return RomeosBallad; //interrupt
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_MightyMarch, MightyMarch) && !HasStatusEffect(Buffs.MightyMarch) && PlayerHealthPercentageHp() <= Config.Phantom_Bard_MightyMarch_Health)
                return MightyMarch; //heal
        }
        #endregion

        #region Berserker
        if (IsEnabled(CustomComboPreset.Phantom_Berserker))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Berserker_Rage, Rage) && CanWeave())
                return Rage; //buff
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Berserker_DeadlyBlow, DeadlyBlow) && GetStatusEffectRemainingTime(Buffs.PentupRage) <= 3f)
                return DeadlyBlow; //action that is better when buff timer is low
        }
        #endregion

        #region Cannoneer
        if (IsEnabled(CustomComboPreset.Phantom_Cannoneer))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Cannoneer_SilverCannon, SilverCannon))
                return SilverCannon; //debuff
            //if (IsEnabledAndUsable(CustomComboPreset.Phantom_Cannoneer_HolyCannon, HolyCannon) && TargetIsUndead())
            //    return HolyCannon; //better on Undead targets____ they dont share a cooldown you fire all the cannons so doesnt really matter if undead. 

            foreach (var (preset, action) in new[]
            { (CustomComboPreset.Phantom_Cannoneer_PhantomFire, PhantomFire),
            (CustomComboPreset.Phantom_Cannoneer_HolyCannon, HolyCannon),
            (CustomComboPreset.Phantom_Cannoneer_DarkCannon, DarkCannon),
            (CustomComboPreset.Phantom_Cannoneer_ShockCannon, ShockCannon), })
                if (IsEnabledAndUsable(preset, action))
                    return action;
        }
        #endregion

        #region Chemist
        if (IsEnabled(CustomComboPreset.Phantom_Chemist))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_Revive, Revive) && TargetIsFriendly() && GetTargetHPPercent() == 0)
                return Revive;

            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_OccultPotion, OccultPotion) && PlayerHealthPercentageHp() <= Config.Phantom_Chemist_OccultPotion_Health)
                return OccultPotion;

            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_OccultEther, OccultEther) && LocalPlayer.CurrentMp <= Config.Phantom_Chemist_OccultEther_MP)
                return OccultEther;

            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_OccultElixir, OccultElixir) && GetPartyAvgHPPercent() <= Config.Phantom_Chemist_OccultElixir_HP && InCombat() && (!Config.Phantom_Chemist_OccultElixir_RequireParty || IsInParty()))
                return OccultElixir;
        }
        #endregion

        #region Freelancer
        if (IsEnabled(CustomComboPreset.Phantom_Freelancer) &&
            IsEnabledAndUsable(CustomComboPreset.Phantom_Freelancer_OccultResuscitation, OccultResuscitation) &&
            PlayerHealthPercentageHp() <= Config.Phantom_Freelancer_Resuscitation_Health)
            return OccultResuscitation;
        #endregion

        #region Geomancer
        if (IsEnabled(CustomComboPreset.Phantom_Geomancer))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_BattleBell, BattleBell) && !HasStatusEffect(Buffs.BattleBell))
                return BattleBell; //buff
            if (IsEnabled(CustomComboPreset.Phantom_Geomancer_Weather))
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_Sunbath, Sunbath) && PlayerHealthPercentageHp() <= Config.Phantom_Geomancer_Sunbath_Health)
                    return Sunbath; //heal
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_AetherialGain, AetherialGain) && !HasStatusEffect(Buffs.AetherialGain))
                    return AetherialGain; //damage buff
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_CloudyCaress, CloudyCaress) && !HasStatusEffect(Buffs.CloudyCaress))
                    return CloudyCaress; //Increases HP recovery
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_BlessedRain, BlessedRain) && !HasStatusEffect(Buffs.BlessedRain))
                    return BlessedRain; //shield
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_MistyMirage, MistyMirage) && !HasStatusEffect(Buffs.MistyMirage))
                    return MistyMirage; //evasion
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_HastyMirage, HastyMirage) && !HasStatusEffect(Buffs.HastyMirage))
                    return HastyMirage; //movement speed
            }
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_RingingRespite, RingingRespite) && !HasStatusEffect(Buffs.RingingRespite))
                return RingingRespite; //heal after damage
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_Suspend, Suspend) && !HasStatusEffect(Buffs.Suspend))
                return Suspend; //float
        }
        #endregion

        #region Knight
        if (IsEnabled(CustomComboPreset.Phantom_Knight))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_PhantomGuard, PhantomGuard) && PlayerHealthPercentageHp() <= Config.Phantom_Knight_PhantomGuard_Health && InCombat())
                return PhantomGuard; //mit
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_Pray, Pray) && PlayerHealthPercentageHp() <= Config.Phantom_Knight_Pray_Health && !HasStatusEffect(Buffs.Pray))
                return Pray; //regen
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_OccultHeal, OccultHeal) && PlayerHealthPercentageHp() <= Config.Phantom_Knight_OccultHeal_Health && LocalPlayer.CurrentMp >= 5000)
                return OccultHeal; //heal
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_Pledge, Pledge) && PlayerHealthPercentageHp() <= Config.Phantom_Knight_Pledge_Health && InCombat())
                return Pledge; //inv
        }
        #endregion

        #region Monk
        if (IsEnabled(CustomComboPreset.Phantom_Monk))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_OccultChakra, OccultChakra) && CanWeave() && PlayerHealthPercentageHp() <= Config.Phantom_Monk_OccultChakra_Health)
                return OccultChakra; //heal
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_Counterstance, Counterstance) && !HasStatusEffect(Buffs.Counterstance))
                return Counterstance; //counterstance
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_PhantomKick, PhantomKick) && CanWeave())
                return PhantomKick; //damage buff + dash
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_OccultCounter, OccultCounter) && CanWeave() && HasStatusEffect(Buffs.Counterstance) && ActionReady(OccultCounter))
                return OccultCounter; //counter attack
        }
        #endregion

        #region Oracle
        if (IsEnabled(CustomComboPreset.Phantom_Oracle))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Predict, Predict) && !HasStatusEffect(Buffs.PredictionOfJudgment) && !HasStatusEffect(Buffs.PredictionOfCleansing) && !HasStatusEffect(Buffs.PredictionOfBlessing) && !HasStatusEffect(Buffs.PredictionOfStarfall))
                return Predict; //start of the chain
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Blessing, Blessing) && HasStatusEffect(Buffs.PredictionOfBlessing) && PlayerHealthPercentageHp() <= Config.Phantom_Oracle_Blessing_Health)
                return Blessing; //heal
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_PhantomJudgment, PhantomJudgment) && HasStatusEffect(Buffs.PredictionOfJudgment))
                return PhantomJudgment; //damage + heal
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Cleansing, Cleansing) && HasStatusEffect(Buffs.PredictionOfCleansing)) // removed interupt. it hits 20% harder than Judgement. 120k aoe.
                return Cleansing; //damage plus interrupt
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Starfall, Starfall) && HasStatusEffect(Buffs.PredictionOfStarfall) && PlayerHealthPercentageHp() <= Config.Phantom_Oracle_Starfall_Health)
                return Starfall; //damage to targets + 90% total HP damage to self
        }
        #endregion

        #region Ranger
        if (IsEnabled(CustomComboPreset.Phantom_Ranger))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Ranger_OccultUnicorn, OccultUnicorn) && CanWeave() && !HasStatusEffect(Buffs.OccultUnicorn, anyOwner: true) &&
                PlayerHealthPercentageHp() <= Config.Phantom_Ranger_OccultUnicorn_Health)
                return OccultUnicorn; //heal
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Ranger_PhantomAim, PhantomAim) && CanWeave() && !HasStatusEffect(Buffs.PhantomAim, anyOwner:true) &&
                GetTargetHPPercent() >= Config.Phantom_Ranger_PhantomAim_Stop)
                return PhantomAim; //damage buff
            //if (IsEnabledAndUsable(CustomComboPreset.Phantom_Ranger_OccultFalcon, OccultFalcon) && CanWeave() && !HasStatusEffect(Buffs.OccultUnicorn)) //TODO: add something for ground targeting? atm needs something like Reaction to work correctly
            //    return OccultFalcon; //stun + ground targeting
        }
        #endregion

        #region Samurai
        if (IsEnabled(CustomComboPreset.Phantom_Samurai))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Mineuchi, Mineuchi) && CanWeave() && CanInterruptEnemy())
                return Mineuchi; //stun
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Shirahadori, Shirahadori) && CanWeave() && TargetIsCasting(0.7f))
                return Shirahadori; //inv against physical
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Zeninage, Zeninage) && ActionReady(Zeninage))
                return Zeninage; //burst
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Iainuki, Iainuki) && ActionReady(Iainuki) && !IsMoving())
                return Iainuki; //cone
        }
        #endregion

        #region Thief
        if (IsEnabled(CustomComboPreset.Phantom_Thief))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_OccultSprint, OccultSprint))
                return OccultSprint; //movement speed
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_Steal, Steal) && CanWeave() && GetTargetHPPercent() <= Config.Phantom_Thief_Steal_Health)
                return Steal; //drops items if used before death
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_Vigilance, Vigilance) && !InCombat() && HasBattleTarget() && GetTargetDistance() <= 10f)
                return Vigilance; //damage buff out of combat
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_PilferWeapon, PilferWeapon) && CanWeave() && !HasStatusEffect(Debuffs.WeaponPlifered, CurrentTarget))
                return PilferWeapon; //weaken target
        }
        #endregion

        #region Time Mage
        if (IsEnabled(CustomComboPreset.Phantom_TimeMage))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultQuick, OccultQuick) && !HasStatusEffect(Buffs.OccultQuick))
                return OccultQuick; //damage buff
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultDispel, OccultDispel) && TargetIsHostile() && HasPhantomDispelStatus(CurrentTarget))
                return OccultDispel; //cleanse
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultMageMasher, OccultMageMasher) && !HasStatusEffect(Debuffs.OccultMageMasher, CurrentTarget))
                return OccultMageMasher; //weaken target's magic attack
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultComet, OccultComet))
                return OccultComet; //damage
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultSlowga, OccultSlowga) && !HasStatusEffect(Debuffs.Slow, CurrentTarget) &&
                (IsNotEnabled(CustomComboPreset.Phantom_TimeMage_OccultSlowga_Wait) || (ICDTracker.TimeUntilExpired(Debuffs.Slow, CurrentTarget.GameObjectId) < TimeSpan.FromSeconds(1.5) || ICDTracker.NumberOfTimesApplied(Debuffs.Slow, CurrentTarget.GameObjectId) < 3)))
                return OccultSlowga; //aoe slow
        }
        #endregion

        return 0; //no conditions met
    }

    public static bool ShouldUsePhantomActions() => BestPhantomAction() != 0;

    /// In Occult Crescent, in the field or a field raid.
    public static bool IsInOccult => ContentHelper.Content.TerritoryIntendedUse == IntendedUse.Occult_Crescent && (ContentCheck.IsInFieldOperations || ContentCheck.IsInFieldRaids);

}
