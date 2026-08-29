using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.Combos.PvE.OccultCrescent.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using ContentHelper = ECommons.GameHelpers;
using IntendedUse = ECommons.ExcelServices.TerritoryIntendedUseEnum;
namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    /// In Occult Crescent (in the field or a field raid).
    public static bool IsInOccult =>
        ContentHelper.Content.TerritoryIntendedUse == IntendedUse.Occult_Crescent &&
        (ContentCheck.IsInFieldOperations || ContentCheck.IsInFieldRaids);

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
        if (TryGetDancerAction(ref actionID)) return true;
        if (TryGetMysticKnightAction(ref actionID)) return true;
        if (TryGetGladiatorAction(ref actionID)) return true;
        if (TryGetNinjaAction(ref actionID)) return true;
        if (TryGetWhiteMageAction(ref actionID)) return true;
        if (TryGetBlackMageAction(ref actionID)) return true;
        if (TryGetDragoonAction(ref actionID)) return true;
        if (TryGetSummonerAction(ref actionID)) return true;
        if (TryGetBlueMageAction(ref actionID)) return true;
        if (TryGetRedMageAction(ref actionID)) return true;
        if (TryGetNecromancerAction(ref actionID)) return true;

        return false;
    }

    private static bool TryGetFreelancerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Freelancer))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Freelancer_OccultResuscitation, OccultResuscitation) &&
            PlayerHP <= Phantom_Freelancer_Resuscitation_Health && !CanWeave())
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
            !HasStatusEffect(Buffs.Pray) && !CanWeave() &&
            (Phantom_Knight_Pray_KeepUp || PlayerHP <= Phantom_Knight_Pray_Health))
        {
            actionID = Pray; // regen
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeave()) return false;

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
            TryUsePledge(ref actionID))
            return true;

        return false;
    }

    private static bool TryGetMonkAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Monk))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Monk_Counterstance, Counterstance) &&
            InCombat() && !HasStatusEffect(Buffs.Counterstance) && !CanWeave())
        {
            actionID = Counterstance; // counterstance
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeave()) return false;

        if (IsEnabledAndUsable(Preset.Phantom_Monk_OccultChakra, OccultChakra) &&
            (PlayerHP <= Phantom_Monk_OccultChakra_Health || PlayerMP <= Phantom_Monk_OccultChakra_MP))
        {
            actionID = OccultChakra; // heal / MP recovery
            return true;
        }

        // Skip if no damage buff, and user wants things under buffs
        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Monk_PhantomKick, PhantomKick) &&
            !IsMoving() && InActionRange(PhantomKick) &&
            GetTargetDistance() <= Phantom_Monk_PhantomKick_Distance)
        {
            actionID = PhantomKick; // damage buff + dash
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Monk_OccultCounter, OccultCounter) &&
            InActionRange(OccultCounter))
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
            !HasStatusEffect(Buffs.Vigilance) && !InCombat())
        {
            actionID = Vigilance; // damage buff out of combat
            return true;
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeave()) return false;

        if (IsEnabledAndUsable(Preset.Phantom_Thief_OccultSprint, OccultSprint) &&
            IsMoving())
        {
            actionID = OccultSprint; // movement speed
            return true;
        }

        if (HasBattleTarget() && InActionRange(Steal))
        {
            if (IsEnabledAndUsable(Preset.Phantom_Thief_Steal, Steal) &&
                TargetHP <= Phantom_Thief_Steal_Health)
            {
                actionID = Steal; // drops items if used before death
                return true;
            }

            // Skip if no damage buff, and user wants things under buffs
            if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
                !Bursting.PlayerIsDamageBuffed)
                return false;

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
            CanWeave() && BeingTargetedHostile)
        {
            actionID = Shirahadori; // inv against physical
            return true;
        }

        // GCDs
        if (!CanWeave() && HasBattleTarget())
        {
            if (IsEnabledAndUsable(Preset.Phantom_Samurai_Mineuchi, Mineuchi) &&
                CanInterruptEnemy() && InActionRange(Mineuchi))
            {
                actionID = Mineuchi; // stun
                return true;
            }

            // Skip if no damage buff, and user wants things under buffs
            if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
                !Bursting.PlayerIsDamageBuffed)
                return false;

            if (IsEnabledAndUsable(Preset.Phantom_Samurai_Zeninage, Zeninage) &&
                ActionWatching.NumberOfGcdsUsed > 4)
            {
                actionID = Zeninage; // burst
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Samurai_Iainuki, Iainuki) &&
                !IsMoving() && InActionRange(Iainuki))
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

        if (!HasBattleTarget()) return false;

        // Skip if no damage buff, and user wants things under buffs
        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Berserker_Rage, Rage) &&
            InActionRange(Rage) && CanWeave())
        {
            actionID = Rage; // buff
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Berserker_DeadlyBlow, DeadlyBlow) &&
            InActionRange(DeadlyBlow) && !CanWeave() &&
            (CurrentJobLevel < 3 ||
             !IsEnabled(Preset.Phantom_Berserker_Rage) ||
             GetStatusEffectRemainingTime(Buffs.PentupRage) <= 3f && HasStatusEffect(Buffs.PentupRage) ||
             !HasStatusEffect(Buffs.PentupRage) && !ActionReady(Rage)))
        {
            actionID = DeadlyBlow; // better when buff timer is low / Rage on CD
            return true;
        }

        return false;
    }

    private static bool TryGetRangerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Ranger))
            return false;

        // Skip things we want to weave, if not in a weave window
        if (!CanWeave()) return false;

        if (IsEnabledAndUsable(Preset.Phantom_Ranger_OccultUnicorn, OccultUnicorn) &&
            !HasStatusEffect(Buffs.OccultUnicorn, anyOwner: true) && PlayerHP <= Phantom_Ranger_OccultUnicorn_Health)
        {
            actionID = OccultUnicorn; // heal
            return true;
        }

        // Skip if no damage buff, and user wants things under buffs
        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

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
            HasBattleTarget() && !HasStatusEffect(Debuffs.OccultMageMasher, CurrentTarget) && CanWeave())
        {
            actionID = OccultMageMasher; // weaken target's magic attack
            return true;
        }

        if (CanWeave()) return false;

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultQuick, OccultQuick) &&
            !HasStatusEffect(Buffs.OccultQuick) && ActionWatching.NumberOfGcdsUsed > 3 &&
            !ShouldHoldOccultQuick())
        {
            actionID = OccultQuick; // damage buff
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultDispel, OccultDispel) &&
            HasBattleTarget() && HasPhantomDispelStatus(CurrentTarget))
        {
            actionID = OccultDispel; // cleanse
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultComet, OccultComet))
        {

            // Skip if no damage buff, and user wants things under buffs
            if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
                !Bursting.PlayerIsDamageBuffed)
                return false;

            // Make the comet fast
            if (Phantom_TimeMage_Comet_RequireSpeed &&
                Phantom_TimeMage_Comet_UseSpeed &&
                !HasStatusEffect(Buffs.OccultQuick) && !JustUsed(OccultQuick) &&
                !HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) && !JustUsed(RoleActions.Magic.Swiftcast) &&
                !HasStatusEffect(BLM.Buffs.Triplecast) && !JustUsed(BLM.Triplecast) &&
                !HasStatusEffect(PLD.Buffs.Requiescat) && !JustUsed(PLD.Imperator) &&
                !HasStatusEffect(RDM.Buffs.Dualcast) &&
                !HasStatusEffect(Buffs.Dualcast))
            {
                if (ActionReady(OccultQuick))
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
                HasStatusEffect(RDM.Buffs.Dualcast) ||
                HasStatusEffect(Buffs.Dualcast))
            {
                actionID = OccultComet; // damage
                return true;
            }
        }

        var canDebuff = EnemiesInRange(OccultSlowga).Any(x => !ImmuneToStatus(x, Debuffs.Slow)
        && !HasStatusEffect(Debuffs.Slow, x)
        && (ICDTracker.StatusIsExpired(Debuffs.Slow, x.GameObjectId)
        || (ICDTracker.NumberOfTimesApplied(Debuffs.Slow, x.GameObjectId) < 3) && IsNotEnabled(Preset.Phantom_TimeMage_OccultSlowga_Wait)));

        if (IsEnabledAndUsable(Preset.Phantom_TimeMage_OccultSlowga, OccultSlowga) &&
            canDebuff)
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

        if (CanWeave()) return false;

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_Revive, Revive) &&
            TryRetargetPhantomRaise(ref actionID, Revive))
            return true;

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_OccultPotion, OccultPotion) &&
            ChemistNeedsPotion())
        {
            actionID = OccultPotion;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_OccultEther, OccultEther) &&
            ChemistNeedsEther())
        {
            actionID = OccultEther;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Chemist_OccultElixir, OccultElixir) &&
            GetPartyAvgHPPercent() <= Phantom_Chemist_OccultElixir_HP && InCombat() &&
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

        if (!CanWeave()) return false;

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

        // Skip if no damage buff, and user wants things under buffs
        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

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

        return false;
    }

    private static bool TryGetOracleAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Oracle))
            return false;

        if (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed)
        {
            if (IsEnabledAndUsable(Preset.Phantom_Oracle_Predict, Predict) && InCombat() && !CanWeave() &&
                !HasStatusEffect(Buffs.PredictionOfJudgment) && !HasStatusEffect(Buffs.PredictionOfCleansing) &&
                !HasStatusEffect(Buffs.PredictionOfBlessing) && !HasStatusEffect(Buffs.PredictionOfStarfall))
            {
                ResetOracleDeck();
                actionID = Predict; // start of the chain
                return true;
            }
        }

        // Skip things we want to weave, if not in a weave window
        if (!CanWeave()) return false;

        UpdateOracleDeck();
        bool lastCard = OracleRemainingCards.Count <= 1;
        bool starfallStillInDeck = OracleRemainingCards.Contains(Buffs.PredictionOfStarfall);
        bool canStillInvulnForStarfall =
            Phantom_Oracle_SaveInvulnForStarfall &&
            IsEnabled(Preset.Phantom_Oracle_Invulnerability) &&
            ActionReady(Invulnerability) &&
            !HasStatusEffect(Buffs.Invulnerability);
        bool holdForStarfall = !lastCard && starfallStillInDeck && canStillInvulnForStarfall &&
                               GetStatusEffectRemainingTime(OracleCurrentCard) > 3f;
        bool tanking = PlayerHasTankStance();
        bool raidwideIncoming = GroupDamageIncoming();
        float partyAvgHp = GetPartyAvgHPPercent();
        bool needsBlessingHeal = PlayerHP <= Phantom_Oracle_Blessing_Health ||
                                 partyAvgHp <= Phantom_Oracle_Blessing_Health;
        bool needsJudgmentHeal = PlayerHP <= Phantom_Oracle_Judgment_PartyHP ||
                                 partyAvgHp <= Phantom_Oracle_Judgment_PartyHP;

        if (HasStatusEffect(Buffs.PredictionOfStarfall))
        {
            if (IsEnabledAndUsable(Preset.Phantom_Oracle_Invulnerability, Invulnerability) &&
                canStillInvulnForStarfall && InCombat())
            {
                actionID = Invulnerability;
                return true;
            }

            bool canStarfallSafely =
                HasStatusEffect(Buffs.Invulnerability) ||
                (PlayerHP >= Phantom_Oracle_Starfall_Health &&
                 !canStillInvulnForStarfall &&
                 !raidwideIncoming);

            if (IsEnabledAndUsable(Preset.Phantom_Oracle_Starfall, Starfall) &&
                canStarfallSafely &&
                (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed ||
                 HasStatusEffect(Buffs.Invulnerability) || lastCard))
            {
                MarkOracleCardPlayed(Buffs.PredictionOfStarfall);
                actionID = Starfall; // damage + 90% total HP damage to self
                return true;
            }

            // Hold Starfall if another card remains and we aren't safe yet
            if (!lastCard && !canStarfallSafely)
                return false;
        }

        // While tanking with Starfall still saved, prefer Cleansing (potency without self-harm)
        if (tanking && holdForStarfall &&
            IsEnabledAndUsable(Preset.Phantom_Oracle_Cleansing, Cleansing) &&
            HasStatusEffect(Buffs.PredictionOfCleansing))
        {
            MarkOracleCardPlayed(Buffs.PredictionOfCleansing);
            actionID = Cleansing;
            return true;
        }

        // Dispel / interrupt before heal cards
        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Recuperation, Recuperation) &&
            HasCleansableDoom())
        {
            actionID = Recuperation;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Cleansing, Cleansing) &&
            HasStatusEffect(Buffs.PredictionOfCleansing) && CanInterruptEnemy())
        {
            MarkOracleCardPlayed(Buffs.PredictionOfCleansing);
            actionID = Cleansing;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Blessing, Blessing) &&
            HasStatusEffect(Buffs.PredictionOfBlessing) &&
            (needsBlessingHeal || lastCard) &&
            (!holdForStarfall || lastCard || tanking))
        {
            MarkOracleCardPlayed(Buffs.PredictionOfBlessing);
            actionID = Blessing; // heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_PhantomRejuvenation, PhantomRejuvenation) &&
            PlayerHP <= Phantom_Oracle_PhantomRejuvenation_Health)
        {
            actionID = PhantomRejuvenation;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_PhantomDoom, PhantomDoom) && HasBattleTarget())
        {
            actionID = PhantomDoom;
            return true;
        }

        // Judgment as heal when party/self is low — skip RestrictToBuff
        if (IsEnabledAndUsable(Preset.Phantom_Oracle_PhantomJudgment, PhantomJudgment) &&
            HasStatusEffect(Buffs.PredictionOfJudgment) &&
            needsJudgmentHeal &&
            (!holdForStarfall || lastCard || tanking))
        {
            MarkOracleCardPlayed(Buffs.PredictionOfJudgment);
            actionID = PhantomJudgment;
            return true;
        }

        // Skip if no damage buff, and user wants things under buffs
        if (!lastCard &&
            IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_PhantomJudgment, PhantomJudgment) &&
            HasStatusEffect(Buffs.PredictionOfJudgment) &&
            (!holdForStarfall || lastCard))
        {
            MarkOracleCardPlayed(Buffs.PredictionOfJudgment);
            actionID = PhantomJudgment; // damage + heal
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Cleansing, Cleansing) &&
            HasStatusEffect(Buffs.PredictionOfCleansing) &&
            (!holdForStarfall || lastCard))
        {
            MarkOracleCardPlayed(Buffs.PredictionOfCleansing);
            actionID = Cleansing; // damage plus interrupt
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Oracle_Invulnerability, Invulnerability) &&
            !Phantom_Oracle_SaveInvulnForStarfall && InCombat() &&
            !HasStatusEffect(Buffs.Invulnerability) && PlayerHP <= Phantom_Oracle_Invulnerability_Health)
        {
            actionID = Invulnerability;
            return true;
        }

        return false;
    }

    private static bool TryGetCannoneerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Cannoneer))
            return false;

        // GCDs
        if (CanWeave() || !HasBattleTarget()) return false;

        // Skip if no damage buff, and user wants things under buffs
        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Cannoneer_SilverCannon, SilverCannon) &&
            ((!HasStatusEffect(Debuffs.SilverSickness, CurrentTarget, true) ||
              GetStatusEffectRemainingTime(Debuffs.SilverSickness, CurrentTarget, true) < 15f) ||
             IsNotEnabled(Preset.Phantom_Cannoneer_HolyCannon)))
        {
            actionID = SilverCannon; // debuff
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Cannoneer_HolyCannon, HolyCannon))
        {
            actionID = HolyCannon;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Cannoneer_PhantomFire, PhantomFire))
        {
            actionID = PhantomFire;
            return true;
        }

        bool darkOk = IsEnabledAndUsable(Preset.Phantom_Cannoneer_DarkCannon, DarkCannon);
        bool shockOk = IsEnabledAndUsable(Preset.Phantom_Cannoneer_ShockCannon, ShockCannon);
        if (darkOk || shockOk)
        {
            bool canBlind = CanApplyStatus(CurrentTarget, Debuffs.Blind);
            bool canPara = CanApplyStatus(CurrentTarget, Debuffs.Paralysis);

            if (canBlind && canPara && darkOk && shockOk)
            {
                actionID = Phantom_Cannoneer_DarkShockPrefer == 1 ? ShockCannon : DarkCannon;
                return true;
            }

            if (canBlind && darkOk)
            {
                actionID = DarkCannon;
                return true;
            }

            if (canPara && shockOk)
            {
                actionID = ShockCannon;
                return true;
            }

            if (darkOk && (Phantom_Cannoneer_DarkShockImmunePrefer == 0 || !shockOk))
            {
                actionID = DarkCannon;
                return true;
            }

            if (shockOk)
            {
                actionID = ShockCannon;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetGeomancerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Geomancer))
            return false;

        if (IsEnabled(Preset.Phantom_Geomancer_Weather) && !CanWeave())
        {
            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_Sunbath, Sunbath) &&
                PlayerHP <= Phantom_Geomancer_Sunbath_Health)
            {
                actionID = Sunbath; // heal
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Geomancer_AetherialGain, AetherialGain) &&
                !HasStatusEffect(Buffs.AetherialGain) &&
                (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed))
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
        if (!CanWeave()) return false;

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
            !HasStatusEffect(Buffs.Suspend) &&
            (InCombat() && Phantom_Geomancer_Suspend_InCombat ||
             !InCombat() && Phantom_Geomancer_Suspend_OutOfCombat))
        {
            actionID = Suspend; // float
            return true;
        }

        // GCDs

        return false;
    }

    private static bool TryGetMysticKnightAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_MysticKnight))
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_MysticKnight_MagicShell, MagicShell) && CanWeave() && InCombat())
        {
            actionID = MagicShell;
            return true;
        }

        if (CanWeave()) return false;

        // Skip if no damage buff, and user wants things under buffs
        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_MysticKnight_BlazingSpellblade, BlazingSpellblade) && !CanWeave() &&
            HasBattleTarget() && InActionRange(BlazingSpellblade) &&
            (!HasStatusEffect(Buffs.BlazingSpellblade) || GetStatusEffectRemainingTime(Buffs.BlazingSpellblade) <= 15))
        {
            actionID = BlazingSpellblade;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_MysticKnight_HolySpellblade, HolySpellblade) && !CanWeave() &&
            HasBattleTarget() && InActionRange(BlazingSpellblade))
        {
            actionID = HolySpellblade;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_MysticKnight_SunderingSpellblade, SunderingSpellblade) && !CanWeave() &&
            HasBattleTarget() && InActionRange(SunderingSpellblade))
        {
            actionID = SunderingSpellblade;
            return true;
        }

        return false;
    }

    private static bool TryGetDancerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Dancer))
            return false;

        if (CanWeave())
        {
            if (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed)
            {
                if (IsEnabledAndUsable(Preset.Phantom_Dancer_Dance, Dance))
                {
                    actionID = Dance;
                    return true;
                }
            }

            if (IsEnabledAndUsable(Preset.Phantom_Dancer_Mesmerize, Mesmerize) && InCombat())
            {
                actionID = Mesmerize; //Damage Debuff
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Dancer_SteadfastStance, SteadfastStance) &&
                InCombat() && !HasStatusEffect(Buffs.SteadfastStance))
            {
                actionID = SteadfastStance; // barrier
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_Dancer_QuickStep, Quickstep) &&
                !HasStatusEffect(Buffs.Quickstep))
            {
                actionID = Quickstep; //Evasion self buff
                return true;
            }

            return false;
        }

        // Skip if no damage buff, and user wants things under buffs
        if (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed)
        {
            if (IsEnabled(Preset.Phantom_Dancer_Dance) && HasStatusEffect(Buffs.PoisedToSwordDance))
            {
                actionID = PoisedToSwordDance;
                return true;
            }
            if (IsEnabled(Preset.Phantom_Dancer_Dance) && HasStatusEffect(Buffs.TemptedToTango))
            {
                actionID = TemptedToTango;
                return true;
            }
            if (IsEnabled(Preset.Phantom_Dancer_Dance) && HasStatusEffect(Buffs.Jitterbugged))
            {
                actionID = Jitterbug;
                return true;
            }
            if (IsEnabled(Preset.Phantom_Dancer_Dance) && HasStatusEffect(Buffs.WillingToWaltz))
            {
                actionID = WillingToWaltz;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetGladiatorAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Gladiator))
            return false;

        if (CanWeave())
        {
            if (IsEnabledAndUsable(Preset.Phantom_Gladiator_Defend, Defend) && InCombat() &&
                (!Phantom_Gladiator_DefendOnlyAtMaxFervor ||
                 GetStatusEffectStacks(Buffs.FinishingFervor) >= 4))
            {
                actionID = Defend;
                return true;
            }

            return false;
        }

        if (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed)
        {
            if (IsEnabledAndUsable(Preset.Phantom_Gladiator_Finisher, Finisher) && HasBattleTarget() && InMeleeRange())
            {
                actionID = Finisher;
                return true;
            }
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) &&
            !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Gladiator_LongReach, LongReach) && HasBattleTarget())
        {
            actionID = LongReach;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Gladiator_BladeBlitz, BladeBlitz) && InCombat() && InActionRange(BladeBlitz))
        {
            actionID = BladeBlitz;
            return true;
        }

        return false;
    }
    private static bool TryGetNinjaAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Ninja))
            return false;
        if (!CanWeave())
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Ninja_Smoke, Smoke) && InCombat() &&
            !HasStatusEffect(Buffs.Smoke))
        {
            actionID = Smoke;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Ninja_Image, Image) && InCombat())
        {
            actionID = Image;
            return true;
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Ninja_FumaShuriken, FumaShuriken) && HasBattleTarget())
        {
            actionID = FumaShuriken;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Ninja_LightningScroll, LightningScroll) &&
            HasBattleTarget() && HasStatusEffect(Debuffs.LightningWeakness, CurrentTarget, true))
        {
            actionID = LightningScroll;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Ninja_FlameScroll, FlameScroll) &&
            HasBattleTarget() && HasStatusEffect(Debuffs.FireWeakness, CurrentTarget, true))
        {
            actionID = FlameScroll;
            return true;
        }
        if (IsEnabledAndUsable(Preset.Phantom_Ninja_FlameScroll, FlameScroll) && HasBattleTarget())
        {
            actionID = FlameScroll;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Ninja_LightningScroll, LightningScroll) && HasBattleTarget())
        {
            actionID = LightningScroll;
            return true;
        }

        return false;
    }

    private static bool TryGetWhiteMageAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_WhiteMage))
            return false;
        if (IsEnabledAndUsable(Preset.Phantom_WhiteMage_OccultBlink, OccultBlink) && InCombat() && CanWeave() &&
            !HasStatusEffect(Buffs.OccultBlink))
        {
            actionID = OccultBlink;
            return true;
        }
        if (CanWeave())
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_WhiteMage_OccultRaise, OccultRaise) &&
            TryRetargetPhantomRaise(ref actionID, OccultRaise))
            return true;

        if (IsEnabledAndUsable(Preset.Phantom_WhiteMage_OccultCureIII, OccultCureIII) &&
            PlayerHP <= Phantom_WhiteMage_OccultCureIII_Health)
        {
            actionID = OccultCureIII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_WhiteMage_OccultCureII, OccultCureII) &&
            PlayerHP <= Phantom_WhiteMage_OccultCureII_Health)
        {
            actionID = OccultCureII;
            return true;
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_WhiteMage_OccultHoly, OccultHoly) && HasBattleTarget())
        {
            actionID = OccultHoly;
            return true;
        }

        return false;
    }

    private static bool TryGetBlackMageAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_BlackMage))
            return false;
        if (CanWeave())
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultToad, OccultToad) && InCombat() &&
            (!Phantom_BlackMage_OccultToad_RequireAoE ||
             GroupDamageIncoming() ||
             NumberOfEnemiesInRange(OccultToad) >= 2))
        {
            actionID = OccultToad;
            return true;
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultFlare, OccultFlare) && HasBattleTarget())
        {
            actionID = OccultFlare;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultFireIII, OccultFireIII) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.FireWeakness))
        {
            actionID = OccultFireIII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultBlizzardIII, OccultBlizzardIII) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.IceWeakness))
        {
            actionID = OccultBlizzardIII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultThunderIII, OccultThunderIII) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.LightningWeakness))
        {
            actionID = OccultThunderIII;
            return true;
        }
        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultThunderIII, OccultThunderIII) && HasBattleTarget())
        {
            actionID = OccultThunderIII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultBlizzardIII, OccultBlizzardIII) && HasBattleTarget())
        {
            actionID = OccultBlizzardIII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlackMage_OccultFireIII, OccultFireIII) && HasBattleTarget())
        {
            actionID = OccultFireIII;
            return true;
        }

        return false;
    }

    private static bool TryGetDragoonAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Dragoon))
            return false;
        if (CanWeave())
        {
            if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
                return false;

            if (IsEnabledAndUsable(Preset.Phantom_Dragoon_Lance, Lance) && HasBattleTarget())
            {
                actionID = Lance;
                return true;
            }

            return false;
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Dragoon_OccultJump, OccultJump) &&
            HasBattleTarget() && CanUseOccultJumpHoldOptions())
        {
            actionID = OccultJump;
            return true;
        }

        return false;
    }

    private static bool TryGetSummonerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Summoner))
            return false;
        if (CanWeave())
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_EarthenWall, EarthenWall) && InCombat() &&
            !HasStatusEffect(Buffs.EarthenWall) && GroupDamageIncoming() && !IsMoving())
        {
            actionID = EarthenWall;
            return true;
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_Megaflare, Megaflare) && HasBattleTarget())
        {
            actionID = Megaflare;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_Thunderstorm, Thunderstorm) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.WindWeakness))
        {
            actionID = Thunderstorm;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_JudgmentBolt, JudgmentBolt) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.LightningWeakness))
        {
            actionID = JudgmentBolt;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_Hellfire, Hellfire) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.FireWeakness))
        {
            actionID = Hellfire;
            return true;
        }
        if (IsEnabledAndUsable(Preset.Phantom_Summoner_Thunderstorm, Thunderstorm) && HasBattleTarget())
        {
            actionID = Thunderstorm;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_JudgmentBolt, JudgmentBolt) && HasBattleTarget())
        {
            actionID = JudgmentBolt;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Summoner_Hellfire, Hellfire) && HasBattleTarget())
        {
            actionID = Hellfire;
            return true;
        }

        return false;
    }

    private static bool TryGetBlueMageAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_BlueMage))
            return false;
        if (CanWeave())
            return false;

        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultMightyGuard, OccultMightyGuard) && InCombat() &&
            !HasStatusEffect(Buffs.OccultMightyGuard))
        {
            actionID = OccultMightyGuard;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultWhiteWind, OccultWhiteWind) &&
            PlayerHP <= Phantom_BlueMage_OccultWhiteWind_Health)
        {
            actionID = OccultWhiteWind;
            return true;
        }

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;
        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultMissile, OccultMissile) &&
            HasBattleTarget() && !ContentCheck.IsInFieldRaids)
        {
            actionID = OccultMissile;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultAquaBreath, OccultAquaBreath) && HasBattleTarget())
        {
            actionID = OccultAquaBreath;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultAeroIII, OccultAeroIII) && HasBattleTarget())
        {
            actionID = OccultAeroIII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultAeroII, OccultAeroII) && HasBattleTarget())
        {
            actionID = OccultAeroII;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_BlueMage_OccultAero, OccultAero) && HasBattleTarget())
        {
            actionID = OccultAero;
            return true;
        }

        return false;
    }

    private static bool TryGetRedMageAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_RedMage))
            return false;
        if (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultLibra, OccultLibra))
        {
            var canDebuff = EnemiesInRange(OccultLibra).Any(x => x.IsInCombat() && x.IsTargetable && !HasLibraWeakness(x) && CanApplyLibraWeakness(x));
            if (canDebuff && (!IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed))
            {
                actionID = OccultLibra;
                return true;
            }
        }

        if (!IsMoving())
        {
            if (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultCureII, OccultCureII_RDM) &&
                TryRetargetPhantomCure(ref actionID,
                OccultCureII_RDM, Phantom_RedMage_OccultCureII_Health,
                IsEnabled(Preset.Phantom_RedMage_OccultCureII_Retarget),
                Phantom_RedMage_Retarget_OutOfParty))
            {
                return true;
            }

            if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
                return false;

            var weakness = GetElementalWeaknesses(CurrentTarget);
            if (weakness.Length == 0 || ((IsEnabledAndUsable(Preset.Phantom_RedMage_OccultFireII, OccultFireII) && weakness.Any(x => x == Debuffs.FireWeakness)) ||
                (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultBlizzardII, OccultBlizzardII) && weakness.Any(x => x == Debuffs.IceWeakness)) ||
                (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultThunderII, OccultThunderII) && weakness.Any(x => x == Debuffs.LightningWeakness))) && !HasLibraWeakness(CurrentTarget))
                return false;

            if (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultBlizzardII, OccultBlizzardII) && HasBattleTarget() &&
                HasSpecificWeakness(CurrentTarget, Debuffs.IceWeakness))
            {
                actionID = OccultBlizzardII;
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultThunderII, OccultThunderII) && HasBattleTarget() &&
                HasSpecificWeakness(CurrentTarget, Debuffs.LightningWeakness))
            {
                actionID = OccultThunderII;
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultFireII, OccultFireII) && HasBattleTarget() &&
                HasSpecificWeakness(CurrentTarget, Debuffs.FireWeakness))
            {
                actionID = OccultFireII;
                return true;
            }

            if (IsEnabledAndUsable(Preset.Phantom_RedMage_OccultFireII, OccultFireII) && HasBattleTarget())
            {
                actionID = OccultFireII;
                return true;
            }
        }

        return false;
    }

    private static bool HasLibraWeakness(IGameObject? tar)
    {
        var statuses = tar?.SafeStatusList;
        if (statuses == null) return false;

        foreach (var s in statuses)
        {
            if (s.StatusId is Debuffs.FireWeakness or Debuffs.IceWeakness or Debuffs.WindWeakness or Debuffs.LightningWeakness)
                return true;
        }
        return false;
    }

    private static bool CanApplyLibraWeakness(IGameObject? tar)
    {
        var statuses = tar?.SafeStatusList;
        if (statuses == null) return false;

        if (CanApplyStatus(tar, Debuffs.FireWeakness) || CanApplyStatus(tar, Debuffs.IceWeakness) || CanApplyStatus(tar, Debuffs.LightningWeakness) || CanApplyStatus(tar, Debuffs.WindWeakness))
            return true;

        return false;
    }

    private static bool TryGetNecromancerAction(ref uint actionID)
    {
        if (!IsEnabled(Preset.Phantom_Necromancer))
            return false;
        if (CanWeave())
        {
            if (!IsEnabledAndUsable(Preset.Phantom_Necromancer_DrainTouch, DrainTouch) || !HasBattleTarget())
                return false;
            
            if (ShouldUseDrainTouch())
            {
                actionID = DrainTouch;
                return true;
            }

            return false;
        }

        if (!CanUseNecromancerSpells())
            return false;

        if (IsEnabled(Preset.Phantom_RestrictToBuff) && !Bursting.PlayerIsDamageBuffed)
            return false;
        if (IsEnabledAndUsable(Preset.Phantom_Necromancer_ChaosDrive, ChaosDrive) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.LightningWeakness))
        {
            actionID = ChaosDrive;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Necromancer_HellWind, HellWind) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.WindWeakness))
        {
            actionID = HellWind;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Necromancer_DeepFreeze, DeepFreeze) && HasBattleTarget() &&
            HasSpecificWeakness(CurrentTarget, Debuffs.IceWeakness))
        {
            actionID = DeepFreeze;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Necromancer_Doomsday, Doomsday) && HasBattleTarget())
        {
            actionID = Doomsday;
            return true;
        }

        if (IsEnabledAndUsable(Preset.Phantom_Necromancer_DeepFreeze, DeepFreeze) && HasBattleTarget())
        {
            actionID = DeepFreeze;
            return true;
        }

        return false;
    }
    private static bool CanUseNecromancerSpells()
    {
        if (Phantom_Necromancer_SpellDuringDrainTouch == 1)
            return HasStatusEffect(Buffs.DrainTouch);
        if (Phantom_Necromancer_SpellDuringDrainTouch == 2)
            return true;
        return !HasStatusEffect(Buffs.DrainTouch);
    }

    private static bool ShouldUseDrainTouch()
    {
        if (Phantom_Necromancer_DrainTouch_Mode == 1)
            return PlayerHP <= Phantom_Necromancer_DrainTouch_Health;
        if (Phantom_Necromancer_DrainTouch_Mode == 2)
            return PlayerHP <= Phantom_Necromancer_DrainTouch_EmergencyHealth;

        // DPS mode
        return !IsEnabled(Preset.Phantom_RestrictToBuff) || Bursting.PlayerIsDamageBuffed;
    }

    private static bool TryUsePledge(ref uint actionID)
    {
        if (Phantom_Knight_Pledge_SelfOnly)
        {
            if (PlayerHP > Phantom_Knight_Pledge_Health)
                return false;
            actionID = Pledge;
            return true;
        }

        var ally = SimpleTarget.LowestHPPAlly;
        if (ally is not null &&
            ally.GameObjectId != LocalPlayer.GameObjectId &&
            GetPartyMembers().Any(m => m.BattleChara?.GameObjectId == ally.GameObjectId) &&
            GetTargetHPPercent(ally) <= Phantom_Knight_Pledge_Health)
        {
            actionID = Pledge.Retarget(actionID, ally);
            return true;
        }

        if (PlayerHP <= Phantom_Knight_Pledge_Health)
        {
            actionID = Pledge;
            return true;
        }

        return false;
    }

    private static bool ShouldHoldOccultQuick() =>
        HasStatusEffect(RDM.Buffs.Manafication) ||
        HasStatusEffect(RDM.Buffs.Embolden) ||
        HasStatusEffect(RDM.Buffs.MagickedSwordPlay) ||
        HasStatusEffect(RDM.Buffs.GrandImpactReady);

    private static bool TryRetargetPhantomRaise(ref uint actionID, uint raiseAction)
    {
        var raiseTarget = SimpleTarget.Stack.AllyToRaiseOccult;
        if (raiseTarget is null)
            return false;

        var originalId = actionID;
        actionID = raiseAction.Retarget(originalId, raiseTarget);
        return true;
    }

    private static bool TryRetargetPhantomCure(ref uint actionID, uint cureAction, float HPThreshold, bool retargeting, bool outOfParty)
    {
        var originalId = actionID;
        // If player HP is at or below threshold, heal self
        if (PlayerHP <= HPThreshold)
        {
            actionID = cureAction.Retarget(originalId, SimpleTarget.Self);
            return true;
        }

        // If retargeting is disabled, no action taken
        if (!retargeting)
            return false;

        // Try to find lowest HP ally at or below threshold
        var allyTarget = SimpleTarget.LowestHPAlly?.IfMissingHP(HPThreshold).IfInSightInRangeCanUseOn(cureAction);
        if (allyTarget != null)
        {
            actionID = cureAction.Retarget(originalId, allyTarget);
            return true;
        }

        // If no in-party ally found and out-of-party is allowed, try out-of-party allies
        if (outOfParty)
        {
            var outOfPartyTarget = SimpleTarget.LowestHPAllyOutOfParty?.IfMissingHP(HPThreshold).IfInSightInRangeCanUseOn(cureAction);
            if (outOfPartyTarget != null)
            {
                Svc.Log.Debug($"Healing OOP {outOfPartyTarget.Name}");
                actionID = cureAction.Retarget(originalId, outOfPartyTarget);
                return true;
            }
        }

        return false;
    }

    public static bool CanPhantomRaise() =>
        IsInOccult &&
        (IsEnabledAndUsable(Preset.Phantom_Chemist_Revive, Revive) ||
         IsEnabledAndUsable(Preset.Phantom_WhiteMage_OccultRaise, OccultRaise));

    private static readonly HashSet<uint> OracleRemainingCards = [];
    private static uint OracleCurrentCard;

    private static void ResetOracleDeck()
    {
        OracleRemainingCards.Clear();
        OracleRemainingCards.Add(Buffs.PredictionOfBlessing);
        OracleRemainingCards.Add(Buffs.PredictionOfCleansing);
        OracleRemainingCards.Add(Buffs.PredictionOfJudgment);
        OracleRemainingCards.Add(Buffs.PredictionOfStarfall);
        OracleCurrentCard = 0;
    }

    private static void UpdateOracleDeck()
    {
        uint card = 0;
        if (HasStatusEffect(Buffs.PredictionOfBlessing))
            card = Buffs.PredictionOfBlessing;
        else if (HasStatusEffect(Buffs.PredictionOfCleansing))
            card = Buffs.PredictionOfCleansing;
        else if (HasStatusEffect(Buffs.PredictionOfJudgment))
            card = Buffs.PredictionOfJudgment;
        else if (HasStatusEffect(Buffs.PredictionOfStarfall))
            card = Buffs.PredictionOfStarfall;

        if (card == 0)
        {
            if (OracleCurrentCard != 0)
            {
                OracleRemainingCards.Remove(OracleCurrentCard);
                OracleCurrentCard = 0;
            }
            return;
        }

        if (OracleRemainingCards.Count == 0)
            OracleRemainingCards.Add(card);

        if (OracleCurrentCard != 0 && OracleCurrentCard != card)
            OracleRemainingCards.Remove(OracleCurrentCard);

        OracleCurrentCard = card;
    }

    private static void MarkOracleCardPlayed(uint card)
    {
        OracleRemainingCards.Remove(card);
        if (OracleCurrentCard == card)
            OracleCurrentCard = 0;
    }

    private static bool ChemistNeedsPotion()
    {
        if (Phantom_Chemist_OccultPotion_SelfOnly)
            return PlayerHP <= Phantom_Chemist_OccultPotion_Health;

        return GetPartyMinHPPercent() <= Phantom_Chemist_OccultPotion_Health;
    }

    private static bool ChemistNeedsEther()
    {
        if (Phantom_Chemist_OccultEther_SelfOnly)
            return PlayerMP <= Phantom_Chemist_OccultEther_MP;

        return GetPartyMembers().Any(m =>
            m.BattleChara is not null && !m.BattleChara.IsDead &&
            m.BattleChara.CurrentMp <= Phantom_Chemist_OccultEther_MP);
    }

    private static float GetPartyMinHPPercent()
    {
        float min = PlayerHP;
        foreach (var member in GetPartyMembers())
        {
            if (member.BattleChara is null || member.BattleChara.IsDead)
                continue;
            float hp = GetTargetHPPercent(member.BattleChara);
            if (hp < min)
                min = hp;
        }

        return min;
    }

    #region Elemental Weakness Caching

    public static bool StatusIsElementalWeakness(uint statusId) =>
        statusId is Debuffs.FireWeakness or Debuffs.IceWeakness or Debuffs.LightningWeakness or Debuffs.WindWeakness;

    /// <summary>
    /// Gets all cached elemental weaknesses for a target by its BaseId.
    /// </summary>
    /// <param name="target">The target to check weakness for</param>
    /// <returns>Array of weakness debuff IDs, or empty array if no weaknesses are cached</returns>
    private static uint[] GetCachedWeaknesses(IGameObject? target)
    {
        if (target?.BaseId is null or 0) return [];

        if (Service.Configuration.ElementalWeaknessCache.TryGetValue(target.BaseId, out var weaknesses))
            return weaknesses ?? [];

        return [];
    }

    /// <summary>
    /// Adds an elemental weakness to the cache for an enemy, avoiding duplicates.
    /// </summary>
    /// <param name="target">The target to cache weakness for</param>
    /// <param name="weaknessId">The weakness debuff ID to cache</param>
    public static void CacheWeakness(IGameObject? target, uint weaknessId)
    {
        if (target?.BaseId is null or 0 || weaknessId == 0) return;

        if (!Service.Configuration.ElementalWeaknessCache.TryGetValue(target.BaseId, out var weaknesses))
        {
            weaknesses = [];
            Service.Configuration.ElementalWeaknessCache[target.BaseId] = weaknesses;
        }

        // Add only if not already cached
        if (!weaknesses.Contains(weaknessId))
        {
            var newWeaknesses = new List<uint>(weaknesses) { weaknessId };
            Service.Configuration.ElementalWeaknessCache[target.BaseId] = newWeaknesses.ToArray();
            Service.Configuration.Save();
        }
    }

    /// <summary>
    /// Gets all elemental weaknesses for a target, checking cache first then current status effects.
    /// Automatically caches newly detected weaknesses.
    /// </summary>
    /// <param name="target">The target to check weakness for</param>
    /// <returns>Array of weakness debuff IDs (Fire/Ice/Lightning/Wind), or empty array if no weaknesses detected</returns>
    private static uint[] GetElementalWeaknesses(IGameObject? target)
    {
        if (target == null) return [];

        var result = new HashSet<uint>();

        // Check cache first
        var cached = GetCachedWeaknesses(target);
        foreach (var weakness in cached)
            result.Add(weakness);

        // Check current status effects and add any not yet cached
        var statuses = target.SafeStatusList;
        if (statuses != null)
        {
            foreach (var status in statuses)
            {
                if (status.StatusId is Debuffs.FireWeakness or Debuffs.IceWeakness
                    or Debuffs.LightningWeakness or Debuffs.WindWeakness)
                {
                    if (!result.Contains(status.StatusId))
                    {
                        CacheWeakness(target, status.StatusId);
                        result.Add(status.StatusId);
                    }
                }
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Checks if a target has a specific elemental weakness (cached or active).
    /// </summary>
    private static bool HasSpecificWeakness(IGameObject? tar, uint weaknessId)
    {
        if (tar == null) return false;

        var weaknesses = GetElementalWeaknesses(tar);
        return weaknesses.Contains(weaknessId);
    }

    /// <summary>
    /// Clears the elemental weakness cache for a specific enemy.
    /// </summary>
    /// <param name="baseId">The BaseId of the enemy to clear cache for</param>
    private static void ClearWeaknessCache(uint baseId)
    {
        if (baseId != 0)
        {
            Service.Configuration.ElementalWeaknessCache.Remove(baseId);
            Service.Configuration.Save();
        }
    }

    /// <summary>
    /// Clears all cached elemental weaknesses.
    /// </summary>
    private static void ClearAllWeaknessCache()
    {
        Service.Configuration.ElementalWeaknessCache.Clear();
        Service.Configuration.Save();
    }

    #endregion

    #region Shorter variables

    private static float TargetHP => GetTargetHPPercent();
    private static float PlayerHP => PlayerHealthPercentageHp();
    private static uint PlayerMP => LocalPlayer.CurrentMp;

    #endregion
}
