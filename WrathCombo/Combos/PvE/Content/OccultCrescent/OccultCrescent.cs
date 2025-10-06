using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Combos.PvE.OccultCrescent.Config;
using ContentHelper = ECommons.GameHelpers;
using IntendedUse = ECommons.ExcelServices.TerritoryIntendedUseEnum;
namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    public static string ContentName => Svc.Data.GetExcelSheet<BannerBg>().GetRow(312).Name.ToString();

    /// In Occult Crescent (in the field or a field raid).
    public static bool IsInOccult => ContentHelper.Content.TerritoryIntendedUse == IntendedUse.Occult_Crescent && (ContentCheck.IsInFieldOperations || ContentCheck.IsInFieldRaids);

    #region Shorter variables

    private static bool IsMovingNow => IsMoving();
    private static bool InCombatNow => InCombat();
    private static bool CanWeaveNow => CanWeave();
    private static bool HasTargetNow => HasBattleTarget();
    private static float TargetDistance => GetTargetDistance();
    private static float TargetHP => GetTargetHPPercent();
    private static float PlayerHP => PlayerHealthPercentageHp();
    private static uint PlayerMP => LocalPlayer.CurrentMp;

    #endregion

    internal static bool TryGetPhantomAction(ref uint actionID)
    {
        if (!IsInOccult)
            return false;

        if (TryGetFreelancerAction(ref actionID)) return true;
        if (TryGetKnightAction(ref actionID)) return true;
        if (TryGetMonkAction(ref actionID)) return true;
        if (TryGetThiefAction(ref actionID)) return true;
        if (TryGetSamuraiAction(ref actionID)) return true;
        if (TryGetBerserkerAction(ref actionID)) return true;
        if (TryGetRangerAction(ref actionID)) return true;
        if (TryGetTimeMageAction(ref actionID)) return true;
        if (TryGetChemistAction(ref actionID)) return true;
        if (TryGetBardAction(ref actionID)) return true;
        if (TryGetOracleAction(ref actionID)) return true;
        if (TryGetCannoneerAction(ref actionID)) return true;
        if (TryGetGeomancerAction(ref actionID)) return true;

        return false;
    }

    private static bool TryGetFreelancerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Freelancer))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Freelancer_OccultResuscitation, OccultResuscitation) &&
            PlayerHP <= Phantom_Freelancer_Resuscitation_Health && !CanWeaveNow)
        {
            actionID = OccultResuscitation; // self-heal
            return true;
        }

        return false;
    }

    private static bool TryGetKnightAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Knight))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Knight_Pray, Pray) &&
            PlayerHP <= Phantom_Knight_Pray_Health && !HasStatusEffect(Buffs.Pray) && !CanWeaveNow)
        {
            actionID = Pray; // regen
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Knight_PhantomGuard, PhantomGuard) &&
            PlayerHP <= Phantom_Knight_PhantomGuard_Health)
        {
            actionID = PhantomGuard; // mit
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Knight_OccultHeal, OccultHeal) &&
            PlayerHP <= Phantom_Knight_OccultHeal_Health && PlayerMP >= 5000)
        {
            actionID = OccultHeal; // heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Knight_Pledge, Pledge) &&
            PlayerHP <= Phantom_Knight_Pledge_Health)
        {
            actionID = Pledge; // inv
            return true;
        }

        return false;
    }

    private static bool TryGetMonkAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Monk))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Monk_Counterstance, Counterstance) &&
            IsPlayerTargeted() && !HasStatusEffect(Buffs.Counterstance) && !CanWeaveNow)
        {
            actionID = Counterstance; // counterstance
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Monk_OccultChakra, OccultChakra) &&
            PlayerHP <= Phantom_Monk_OccultChakra_Health)
        {
            actionID = OccultChakra; // heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Monk_PhantomKick, PhantomKick) &&
            !IsMovingNow && TargetDistance <= 15f)
        {
            actionID = PhantomKick; // damage buff + dash
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Monk_OccultCounter, OccultCounter) &&
            TargetDistance <= 6f)
        {
            actionID = OccultCounter; // counter-attack
            return true;
        }

        return false;
    }

    private static bool TryGetThiefAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Thief))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Thief_Vigilance, Vigilance) &&
            !HasStatusEffect(Buffs.Vigilance) && !InCombatNow)
        {
            actionID = Vigilance; // damage buff out of combat
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Thief_OccultSprint, OccultSprint) &&
            IsMovingNow)
        {
            actionID = OccultSprint; // movement speed
            return true;
        }

        if (HasTargetNow && TargetDistance <= 5f)
        {
            if (IsEnabledAndUsable(Preset.Phantom_Thief_Steal, Steal) &&
                TargetHP <= Phantom_Thief_Steal_Health)
            {
                actionID = Steal; // drops items if used before death
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Thief_PilferWeapon, PilferWeapon) &&
                !HasStatusEffect(Debuffs.WeaponPlifered, CurrentTarget))
            {
                actionID = PilferWeapon; // weaken target
                return true;
            }
        }

        return false;
    }

    private static bool TryGetSamuraiAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Samurai))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Samurai_Shirahadori, Shirahadori) &&
            CanWeaveNow && BeingTargetedHostile)
        {
            actionID = Shirahadori; // inv against physical
            return true;
        }

        // GCDs
        if (!CanWeaveNow && HasTargetNow)
        {
            if (IsEnabledAndUsable(Preset.Phantom_Samurai_Mineuchi, Mineuchi) &&
                CanInterruptEnemy() && TargetDistance <= 5f)
            {
                actionID = Mineuchi; // stun
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Samurai_Zeninage, Zeninage) &&
                ActionWatching.NumberOfGcdsUsed > 4)
            {
                actionID = Zeninage; // burst
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Samurai_Iainuki, Iainuki) &&
                !IsMovingNow && TargetDistance <= 8f)
            {
                actionID = Iainuki; // cone
                return true;
            }
        }

        return false;
    }

    private static bool TryGetBerserkerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Berserker))
            return false;

        if (!HasTargetNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Berserker_Rage, Rage) &&
            TargetDistance <= 3f && CanWeaveNow)
        {
            actionID = Rage; // buff
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Berserker_DeadlyBlow, DeadlyBlow) &&
            GetStatusEffectRemainingTime(Buffs.PentupRage) <= 3f && TargetDistance <= 5f && !CanWeaveNow)
        {
            actionID = DeadlyBlow; // better when buff timer is low
            return true;
        }

        return false;
    }

    private static bool TryGetRangerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Ranger))
            return false;

        // Skip things we want to weave, if not in a weave window
        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Ranger_OccultUnicorn, OccultUnicorn) &&
            !HasStatusEffect(Buffs.OccultUnicorn, anyOwner: true) && PlayerHP <= Phantom_Ranger_OccultUnicorn_Health)
        {
            actionID = OccultUnicorn; // heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Ranger_PhantomAim, PhantomAim) &&
            TargetHP >= Phantom_Ranger_PhantomAim_Stop)
        {
            actionID = PhantomAim; // damage buff
            return true;
        }

        // Ground-target action OccultFalcon intentionally left commented as in original

        return false;
    }

    private static bool TryGetTimeMageAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_TimeMage))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultMageMasher, OccultMageMasher) &&
            HasTargetNow && !HasStatusEffect(Debuffs.OccultMageMasher, CurrentTarget) && CanWeaveNow)
        {
            actionID = OccultMageMasher; // weaken target's magic attack
            return true;
        }

        if (CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultQuick, OccultQuick) &&
            !HasStatusEffect(Buffs.OccultQuick) && ActionWatching.NumberOfGcdsUsed > 3)
        {
            actionID = OccultQuick; // damage buff
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultDispel, OccultDispel) &&
            HasTargetNow && HasPhantomDispelStatus(CurrentTarget))
        {
            actionID = OccultDispel; // cleanse
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultComet, OccultComet))
        {
            // Make the comet fast
            if (Phantom_TimeMage_Comet_RequireSpeed &&
                Phantom_TimeMage_Comet_UseSpeed &&
                !HasStatusEffect(Buffs.OccultQuick) && !JustUsed(OccultQuick) &&
                !HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) && !JustUsed(RoleActions.Magic.Swiftcast) &&
                !HasStatusEffect(BLM.Buffs.Triplecast) && !JustUsed(BLM.Triplecast) &&
                !HasStatusEffect(PLD.Buffs.Requiescat) && !JustUsed(PLD.Imperator) &&
                !HasStatusEffect(RDM.Buffs.Dualcast))
            {
                if (HasActionEquipped(OccultQuick) && ActionReady(OccultQuick))
                {
                    actionID = OccultQuick;
                    return true;
                }

                if (ActionReady(RoleActions.Magic.Swiftcast))
                {
                    actionID = RoleActions.Magic.Swiftcast;
                    return true;
                }
            }

            if (!Phantom_TimeMage_Comet_RequireSpeed ||
                HasStatusEffect(Buffs.OccultQuick) ||
                HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) ||
                HasStatusEffect(BLM.Buffs.Triplecast) ||
                HasStatusEffect(PLD.Buffs.Requiescat) ||
                HasStatusEffect(RDM.Buffs.Dualcast))
            {
                actionID = OccultComet; // damage
                return true;
            }
        }

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultSlowga, OccultSlowga) &&
            HasTargetNow && !HasStatusEffect(Debuffs.Slow, CurrentTarget) &&
            (IsNotEnabled(Preset.Phantom_TimeMage_OccultSlowga_Wait) ||
             (ICDTracker.TimeUntilExpired(Debuffs.Slow, CurrentTarget.GameObjectId) < TimeSpan.FromSeconds(1.5) ||
              ICDTracker.NumberOfTimesApplied(Debuffs.Slow, CurrentTarget.GameObjectId) < 3)))
        {
            actionID = OccultSlowga; // aoe slow
            return true;
        }

        return false;
    }

    private static bool TryGetChemistAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Chemist))
            return false;

        if (CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Chemist_Revive, Revive) &&
            TargetIsFriendly() && TargetIsDead())
        {
            actionID = Revive;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_OccultPotion, OccultPotion) &&
            PlayerHP <= Phantom_Chemist_OccultPotion_Health)
        {
            actionID = OccultPotion;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_OccultEther, OccultEther) &&
            PlayerMP <= Phantom_Chemist_OccultEther_MP)
        {
            actionID = OccultEther;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_OccultElixir, OccultElixir) &&
            GetPartyAvgHPPercent() <= Phantom_Chemist_OccultElixir_HP && InCombatNow &&
            (!Phantom_Chemist_OccultElixir_RequireParty || IsInParty()))
        {
            actionID = OccultElixir;
            return true;
        }

        return false;
    }

    private static bool TryGetBardAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Bard))
            return false;

        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Bard_HerosRime, HerosRime))
        {
            actionID = HerosRime; // burst song
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Bard_OffensiveAria, OffensiveAria) &&
            !HasStatusEffect(Buffs.OffensiveAria) && !HasStatusEffect(Buffs.HerosRime, anyOwner: true))
        {
            actionID = OffensiveAria; // off-song
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Bard_RomeosBallad, RomeosBallad) &&
            CanInterruptEnemy())
        {
            actionID = RomeosBallad; // interrupt
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Bard_MightyMarch, MightyMarch) &&
            !HasStatusEffect(Buffs.MightyMarch) && PlayerHP <= Phantom_Bard_MightyMarch_Health)
        {
            actionID = MightyMarch; // aoe heal
            return true;
        }

        return false;
    }

    private static bool TryGetOracleAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Oracle))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Predict, Predict) && InCombatNow && !CanWeaveNow &&
            !HasStatusEffect(Buffs.PredictionOfJudgment) && !HasStatusEffect(Buffs.PredictionOfCleansing) &&
            !HasStatusEffect(Buffs.PredictionOfBlessing) && !HasStatusEffect(Buffs.PredictionOfStarfall))
        {
            actionID = Predict; // start of the chain
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Blessing, Blessing) &&
            HasStatusEffect(Buffs.PredictionOfBlessing) && PlayerHP <= Phantom_Oracle_Blessing_Health)
        {
            actionID = Blessing; // heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_PhantomJudgment, PhantomJudgment) &&
            HasStatusEffect(Buffs.PredictionOfJudgment))
        {
            actionID = PhantomJudgment; // damage + heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Cleansing, Cleansing) &&
            HasStatusEffect(Buffs.PredictionOfCleansing)) // removed interrupt. it hits 20% harder than Judgment. 120k aoe.
        {
            actionID = Cleansing; // damage plus interrupt
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Starfall, Starfall) &&
            HasStatusEffect(Buffs.PredictionOfStarfall) && PlayerHP >= Phantom_Oracle_Starfall_Health)
        {
            actionID = Starfall; // damage + 90% total HP damage to self
            return true;
        }

        return false;
    }

    private static bool TryGetCannoneerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Cannoneer))
            return false;

        // GCDs
        if (CanWeaveNow || !HasTargetNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Cannoneer_SilverCannon, SilverCannon) &&
            (!HasStatusEffect(Debuffs.SilverSickness, CurrentTarget, anyOwner:true) || 
             GetStatusEffectRemainingTime(Debuffs.SilverSickness, CurrentTarget, anyOwner:true) < 30f) || 
             IsNotEnabled(Preset.Phantom_Cannoneer_HolyCannon))
        {
            actionID = SilverCannon; // debuff
            return true;
        }

        foreach ((Preset preset, uint action) in new[]
                 {
                     (Preset.Phantom_Cannoneer_PhantomFire, PhantomFire),
                     (Preset.Phantom_Cannoneer_HolyCannon, HolyCannon),
                     (Preset.Phantom_Cannoneer_DarkCannon, DarkCannon),
                     (Preset.Phantom_Cannoneer_ShockCannon, ShockCannon)
                 })
        {
            if (IsEnabledAndUsable(preset, action))
            {
                actionID = action;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetGeomancerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Geomancer))
            return false;
        
        if (IsEnabled(Preset.Phantom_Geomancer_Weather) && !CanWeaveNow)
        {
            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_Sunbath, Sunbath) &&
                PlayerHP <= Phantom_Geomancer_Sunbath_Health)
            {
                actionID = Sunbath; // heal
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_AetherialGain, AetherialGain) &&
                !HasStatusEffect(Buffs.AetherialGain))
            {
                actionID = AetherialGain; // damage buff
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_CloudyCaress, CloudyCaress) &&
                !HasStatusEffect(Buffs.CloudyCaress))
            {
                actionID = CloudyCaress; // Increases HP recovery
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_BlessedRain, BlessedRain) &&
                !HasStatusEffect(Buffs.BlessedRain))
            {
                actionID = BlessedRain; // shield
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_MistyMirage, MistyMirage) &&
                !HasStatusEffect(Buffs.MistyMirage))
            {
                actionID = MistyMirage; // evasion
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_HastyMirage, HastyMirage) &&
                !HasStatusEffect(Buffs.HastyMirage))
            {
                actionID = HastyMirage; // movement speed
                return true;
            }
        }
        
        // Skip things we want to weave, if not in a weave window
        if (!CanWeaveNow) return false;
        
        if (IsEnabledAndUsable(Preset.Phantom_Geomancer_BattleBell, BattleBell) &&
            !HasStatusEffect(Buffs.BattleBell))
        {
            actionID = BattleBell; // buff
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Geomancer_RingingRespite, RingingRespite) &&
            !HasStatusEffect(Buffs.RingingRespite))
        {
            actionID = RingingRespite; // heal after damage
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Geomancer_Suspend, Suspend) &&
            !HasStatusEffect(Buffs.Suspend))
        {
            actionID = Suspend; // float
            return true;
        }

        // GCDs

        return false;
    }
}
