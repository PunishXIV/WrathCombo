using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using ContentHelper = ECommons.GameHelpers;
using IntendedUse = ECommons.ExcelServices.TerritoryIntendedUseEnum;

namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    public const byte JobID = 100;
    public static string ContentName => Svc.Data.GetExcelSheet<BannerBg>().GetRow(312).Name.ToString();

    /// In Occult Crescent, in the field or a field raid.
    public static bool IsInOccult => ContentHelper.Content.TerritoryIntendedUse == IntendedUse.Occult_Crescent && (ContentCheck.IsInFieldOperations || ContentCheck.IsInFieldRaids);
    internal static uint BestPhantomAction()
    {
        if (!IsInOccult)
            return 0; //not in Occult Crescent

        bool isMoving = IsMoving();
        bool inCombat = InCombat();
        bool canWeave = CanWeave();
        bool hasTarget = HasBattleTarget();
        float targetDistance = GetTargetDistance();
        float targetHP = GetTargetHPPercent();
        float playerHP = PlayerHealthPercentageHp();
        uint playerMP = LocalPlayer.CurrentMp;

        #region Bard

        if (IsEnabled(CustomComboPreset.Phantom_Bard))
        {
            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_HerosRime, HerosRime))
                    return HerosRime; //burst song

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_OffensiveAria, OffensiveAria) &&
                    !HasStatusEffect(Buffs.OffensiveAria) && !HasStatusEffect(Buffs.HerosRime, anyOwner: true))
                    return OffensiveAria; //off-song

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_RomeosBallad, RomeosBallad) &&
                    CanInterruptEnemy())
                    return RomeosBallad; //interrupt

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Bard_MightyMarch, MightyMarch) &&
                    !HasStatusEffect(Buffs.MightyMarch) && playerHP <= Config.Phantom_Bard_MightyMarch_Health)
                    return MightyMarch; //aoe heal
            }
        }

        #endregion

        #region Berserker

        if (IsEnabled(CustomComboPreset.Phantom_Berserker))
        {
            if (hasTarget)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Berserker_Rage, Rage) &&
                    targetDistance <= 3f && canWeave)
                    return Rage; //buff

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Berserker_DeadlyBlow, DeadlyBlow) &&
                    GetStatusEffectRemainingTime(Buffs.PentupRage) <= 3f && targetDistance <= 5f && !canWeave)
                    return DeadlyBlow; //action that is better when buff timer is low
            }
        }

        #endregion

        #region Cannoneer

        if (IsEnabled(CustomComboPreset.Phantom_Cannoneer))
        {
            // GCDs
            if (!canWeave && hasTarget)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Cannoneer_SilverCannon, SilverCannon))
                    return SilverCannon; //debuff

                //if (IsEnabledAndUsable(CustomComboPreset.Phantom_Cannoneer_HolyCannon, HolyCannon) && TargetIsUndead())
                //    return HolyCannon; //better on Undead targets____ they dont share a cooldown you fire all the cannons so doesnt really matter if undead. 

                foreach((CustomComboPreset preset, uint action) in new[]
                {
                    (CustomComboPreset.Phantom_Cannoneer_PhantomFire, PhantomFire),
                    (CustomComboPreset.Phantom_Cannoneer_HolyCannon, HolyCannon),
                    (CustomComboPreset.Phantom_Cannoneer_DarkCannon, DarkCannon),
                    (CustomComboPreset.Phantom_Cannoneer_ShockCannon, ShockCannon)
                })
                {
                    if (IsEnabledAndUsable(preset, action))
                        return action;
                }
            }
        }

        #endregion

        #region Chemist

        if (IsEnabled(CustomComboPreset.Phantom_Chemist))
        {
            // GCDs
            if (!canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_Revive, Revive) &&
                    TargetIsFriendly() && TargetIsDead())
                    return Revive;

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_OccultPotion, OccultPotion) &&
                    playerHP <= Config.Phantom_Chemist_OccultPotion_Health)
                    return OccultPotion;

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_OccultEther, OccultEther) &&
                    playerMP <= Config.Phantom_Chemist_OccultEther_MP)
                    return OccultEther;

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Chemist_OccultElixir, OccultElixir) &&
                    GetPartyAvgHPPercent() <= Config.Phantom_Chemist_OccultElixir_HP && inCombat &&
                    (!Config.Phantom_Chemist_OccultElixir_RequireParty || IsInParty()))
                    return OccultElixir;
            }
        }

        #endregion

        #region Freelancer

        if (IsEnabled(CustomComboPreset.Phantom_Freelancer))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Freelancer_OccultResuscitation, OccultResuscitation) &&
                playerHP <= Config.Phantom_Freelancer_Resuscitation_Health && !canWeave)
                return OccultResuscitation; //self heal
        }

        #endregion

        #region Geomancer

        if (IsEnabled(CustomComboPreset.Phantom_Geomancer))
        {
            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_BattleBell, BattleBell) &&
                    !HasStatusEffect(Buffs.BattleBell))
                    return BattleBell; //buff

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_RingingRespite, RingingRespite) &&
                    !HasStatusEffect(Buffs.RingingRespite))
                    return RingingRespite; //heal after damage

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_Suspend, Suspend) &&
                    !HasStatusEffect(Buffs.Suspend))
                    return Suspend; //float
            }

            // GCDs
            if (IsEnabled(CustomComboPreset.Phantom_Geomancer_Weather) && !canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_Sunbath, Sunbath) &&
                    playerHP <= Config.Phantom_Geomancer_Sunbath_Health)
                    return Sunbath; //heal

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_AetherialGain, AetherialGain) &&
                    !HasStatusEffect(Buffs.AetherialGain))
                    return AetherialGain; //damage buff

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_CloudyCaress, CloudyCaress) &&
                    !HasStatusEffect(Buffs.CloudyCaress))
                    return CloudyCaress; //Increases HP recovery

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_BlessedRain, BlessedRain) &&
                    !HasStatusEffect(Buffs.BlessedRain))
                    return BlessedRain; //shield

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_MistyMirage, MistyMirage) &&
                    !HasStatusEffect(Buffs.MistyMirage))
                    return MistyMirage; //evasion

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Geomancer_HastyMirage, HastyMirage) &&
                    !HasStatusEffect(Buffs.HastyMirage))
                    return HastyMirage; //movement speed
            }
        }

        #endregion

        #region Knight

        if (IsEnabled(CustomComboPreset.Phantom_Knight))
        {
            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_PhantomGuard, PhantomGuard) &&
                    playerHP <= Config.Phantom_Knight_PhantomGuard_Health)
                    return PhantomGuard; //mit

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_OccultHeal, OccultHeal) &&
                    playerHP <= Config.Phantom_Knight_OccultHeal_Health && playerMP >= 5000)
                    return OccultHeal; //heal

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_Pledge, Pledge) &&
                    playerHP <= Config.Phantom_Knight_Pledge_Health)
                    return Pledge; //inv
            }

            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Knight_Pray, Pray) &&
                playerHP <= Config.Phantom_Knight_Pray_Health && !HasStatusEffect(Buffs.Pray) && !canWeave)
                return Pray; //regen
        }

        #endregion

        #region Monk

        if (IsEnabled(CustomComboPreset.Phantom_Monk))
        {
            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_OccultChakra, OccultChakra) &&
                    playerHP <= Config.Phantom_Monk_OccultChakra_Health)
                    return OccultChakra; //heal

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_PhantomKick, PhantomKick) &&
                    !isMoving && targetDistance <= 15f)
                    return PhantomKick; //damage buff + dash

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_OccultCounter, OccultCounter) &&
                    targetDistance <= 6f)
                    return OccultCounter; //counter attack
            }

            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Monk_Counterstance, Counterstance) &&
                IsPlayerTargeted() && !HasStatusEffect(Buffs.Counterstance) && !canWeave)
                return Counterstance; //counterstance
        }

        #endregion

        #region Oracle

        if (IsEnabled(CustomComboPreset.Phantom_Oracle))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Predict, Predict) && inCombat && !canWeave &&
                !HasStatusEffect(Buffs.PredictionOfJudgment) && !HasStatusEffect(Buffs.PredictionOfCleansing) &&
                !HasStatusEffect(Buffs.PredictionOfBlessing) && !HasStatusEffect(Buffs.PredictionOfStarfall))
                return Predict; //start of the chain

            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Blessing, Blessing) &&
                    HasStatusEffect(Buffs.PredictionOfBlessing) && playerHP <= Config.Phantom_Oracle_Blessing_Health)
                    return Blessing; //heal

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_PhantomJudgment, PhantomJudgment) &&
                    HasStatusEffect(Buffs.PredictionOfJudgment))
                    return PhantomJudgment; //damage + heal

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Cleansing, Cleansing) &&
                    HasStatusEffect(Buffs.PredictionOfCleansing)) // removed interupt. it hits 20% harder than Judgement. 120k aoe.
                    return Cleansing; //damage plus interrupt

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Oracle_Starfall, Starfall) &&
                    HasStatusEffect(Buffs.PredictionOfStarfall) && playerHP >= Config.Phantom_Oracle_Starfall_Health)
                    return Starfall; //damage to targets + 90% total HP damage to self
            }
        }

        #endregion

        #region Ranger

        if (IsEnabled(CustomComboPreset.Phantom_Ranger))
        {
            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Ranger_OccultUnicorn, OccultUnicorn) &&
                    !HasStatusEffect(Buffs.OccultUnicorn, anyOwner: true) && playerHP <= Config.Phantom_Ranger_OccultUnicorn_Health)
                    return OccultUnicorn; //heal

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Ranger_PhantomAim, PhantomAim) &&
                    targetHP >= Config.Phantom_Ranger_PhantomAim_Stop)
                    return PhantomAim; //damage buff
            }

            //if (IsEnabledAndUsable(CustomComboPreset.Phantom_Ranger_OccultFalcon, OccultFalcon) &&
            //    !HasStatusEffect(Buffs.OccultUnicorn) && !canWeave) //TODO: add something for ground targeting? atm needs something like Reaction to work correctly
            //    return OccultFalcon; //stun + ground targeting
        }

        #endregion

        #region Samurai

        if (IsEnabled(CustomComboPreset.Phantom_Samurai))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Shirahadori, Shirahadori) &&
                canWeave && BeingTargetedHostile)
                return Shirahadori; //inv against physical

            // GCDs
            if (!canWeave && hasTarget)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Mineuchi, Mineuchi) &&
                    CanInterruptEnemy() && targetDistance <= 5f)
                    return Mineuchi; //stun

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Zeninage, Zeninage) &&
                    ActionWatching.NumberOfGcdsUsed > 4)
                    return Zeninage; //burst (30y range somehow)

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Samurai_Iainuki, Iainuki) &&
                    !isMoving && targetDistance <= 8f)
                    return Iainuki; //cone
            }
        }

        #endregion

        #region Thief

        if (IsEnabled(CustomComboPreset.Phantom_Thief))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_Vigilance, Vigilance) &&
                !HasStatusEffect(Buffs.Vigilance) && !inCombat)
                return Vigilance; //damage buff out of combat

            // Abilities
            if (canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_OccultSprint, OccultSprint) &&
                    isMoving)
                    return OccultSprint; //movement speed

                if (hasTarget && targetDistance <= 5f)
                {
                    if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_Steal, Steal) &&
                        targetHP <= Config.Phantom_Thief_Steal_Health)
                        return Steal; //drops items if used before death

                    if (IsEnabledAndUsable(CustomComboPreset.Phantom_Thief_PilferWeapon, PilferWeapon) &&
                        !HasStatusEffect(Debuffs.WeaponPlifered, CurrentTarget))
                        return PilferWeapon; //weaken target
                }
            }
        }

        #endregion

        #region Time Mage

        if (IsEnabled(CustomComboPreset.Phantom_TimeMage))
        {
            if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultMageMasher, OccultMageMasher) &&
                hasTarget && !HasStatusEffect(Debuffs.OccultMageMasher, CurrentTarget) && canWeave)
                return OccultMageMasher; //weaken target's magic attack

            // GCDs
            if (!canWeave)
            {
                if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultQuick, OccultQuick) &&
                    !HasStatusEffect(Buffs.OccultQuick) && ActionWatching.NumberOfGcdsUsed > 3)
                    return OccultQuick; //damage buff

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultDispel, OccultDispel) &&
                    hasTarget && HasPhantomDispelStatus(CurrentTarget))
                    return OccultDispel; //cleanse

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultComet, OccultComet))
                {
                    // Make comet fast
                    if (Config.Phantom_TimeMage_Comet_RequireSpeed &&
                        Config.Phantom_TimeMage_Comet_UseSpeed &&
                        !HasStatusEffect(Buffs.OccultQuick) && !JustUsed(OccultQuick) &&
                        !HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) && !JustUsed(RoleActions.Magic.Swiftcast) &&
                        !HasStatusEffect(BLM.Buffs.Triplecast) && !JustUsed(BLM.Triplecast) &&
                        !HasStatusEffect(PLD.Buffs.Requiescat) && !JustUsed(PLD.Imperator) &&
                        !HasStatusEffect(RDM.Buffs.Dualcast))
                    {
                        if (HasActionEquipped(OccultQuick) && ActionReady(OccultQuick))
                            return OccultQuick;

                        if (ActionReady(RoleActions.Magic.Swiftcast))
                            return RoleActions.Magic.Swiftcast;
                    }

                    if (!Config.Phantom_TimeMage_Comet_RequireSpeed ||
                        HasStatusEffect(Buffs.OccultQuick) ||
                        HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) ||
                        HasStatusEffect(BLM.Buffs.Triplecast) ||
                        HasStatusEffect(PLD.Buffs.Requiescat) ||
                        HasStatusEffect(RDM.Buffs.Dualcast))
                        return OccultComet; // damage
                }

                if (IsEnabledAndUsable(CustomComboPreset.Phantom_TimeMage_OccultSlowga, OccultSlowga) &&
                    hasTarget && !HasStatusEffect(Debuffs.Slow, CurrentTarget) &&
                    (IsNotEnabled(CustomComboPreset.Phantom_TimeMage_OccultSlowga_Wait) ||
                     (ICDTracker.TimeUntilExpired(Debuffs.Slow, CurrentTarget.GameObjectId) < TimeSpan.FromSeconds(1.5) ||
                      ICDTracker.NumberOfTimesApplied(Debuffs.Slow, CurrentTarget.GameObjectId) < 3)))
                    return OccultSlowga; //aoe slow
            }
        }

        #endregion

        return 0; //no conditions met
    }

    public static bool ShouldUsePhantomActions() => BestPhantomAction() != 0;
}
