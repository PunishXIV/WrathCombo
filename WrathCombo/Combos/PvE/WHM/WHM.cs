#region
using System.Linq;
using ECommons.GameFunctions;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.WHM.Config;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;


// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM : Healer
{
    #region Simple DPS

    internal class WHM_ST_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_ST_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            var actionFound = StoneGlareList.Contains(actionID);

            if (!actionFound)
                return actionID;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (!PartyInCombat()) return actionID;

            #region Weaves

            if (CanWeave())
            {
                if (ActionReady(PresenceOfMind) &&
                    ActionWatching.NumberOfGcdsUsed >= 3 &&
                    !HasStatusEffect(Buffs.SacredSight))
                    return PresenceOfMind;

                if (ActionReady(Assize) &&
                    HasBattleTarget() && GetTargetDistance() <= 20)
                    return Assize;

                if (Role.CanLucidDream(7500))
                    return Role.LucidDreaming;
            }

            #endregion

            #region GCDS and Casts

            // DoTs
            if (NeedsDoT())
                return OriginalHook(Aero);
            
            // Blood Lily Spend
            if (BloodLilyReady)
                return AfflatusMisery;

            // Glare IV
            if (HasStatusEffect(Buffs.SacredSight))
                return Glare4;

            // Lily Heal Overcap
            if (ActionReady(AfflatusRapture) &&
                (FullLily || AlmostFullLily))
                return AfflatusRapture;

            #region Movement Options

            if (IsMoving())
            {
                var dotAction = OriginalHook(Aero);
                AeroList.TryGetValue(dotAction, out var dotDebuffID);
                var target = SimpleTarget.DottableEnemy(
                    dotAction, dotDebuffID, 0, 20, 99);
                if (IsEnabled(Preset.WHM_ST_MainCombo_Move_DoT) &&
                    target is not null)
                    return dotAction.Retarget(StoneGlareList.ToArray(), target);
            }

            #endregion

            return actionID;

            #endregion
        }
    }

    internal class WHM_AoE_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_AoE_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Holy or Holy3))
                return actionID;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Weaves

            if (CanWeave() || IsMoving())
            {
                if (ActionReady(Assize) &&
                    HasBattleTarget() && GetTargetDistance() <= 20)
                    return Assize;

                if (ActionReady(PresenceOfMind) &&
                    ActionWatching.NumberOfGcdsUsed >= 4 &&
                    !HasStatusEffect(Buffs.SacredSight))
                    return PresenceOfMind;

                if (Role.CanLucidDream(7500))
                    return Role.LucidDreaming;
            }

            #endregion

            #region GCDS and Casts
            
            if (HasBattleTarget() && BloodLilyReady)
                return AfflatusMisery;
          
            if (HasStatusEffect(Buffs.SacredSight))
                return OriginalHook(Glare4);

            if (ActionReady(AfflatusRapture) &&
                (FullLily || AlmostFullLily))
                return AfflatusRapture;

            var dotAction = OriginalHook(Aero);
            AeroList.TryGetValue(dotAction, out var dotDebuffID);
            var target =
                SimpleTarget.DottableEnemy(dotAction, dotDebuffID, 30, 3, 4);

            if (ActionReady(dotAction) && target != null)
                return OriginalHook(Aero).Retarget([Holy, Holy3], target);

            #endregion

            return actionID;
        }
    }

    #endregion

    #region DPS

    internal class WHM_ST_MainCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_ST_MainCombo;

        protected override uint Invoke(uint actionID)
        {
            #region Button Selection

            var replacedAction = (int)WHM_ST_MainCombo_Actions switch
            {
                1 => AeroList.Keys.ToArray(),
                2 => [Stone2],
                _ => StoneGlareList.ToArray(),
            };

            if (!replacedAction.Contains(actionID))
                return actionID;

            #endregion

            #region Opener

            if (IsEnabled(Preset.WHM_ST_MainCombo_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            #endregion

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (!PartyInCombat()) return actionID;

            #region Special Feature Raidwide

            if (RaidwideTemperance())
                return OriginalHook(Temperance);
            if (RaidwideAsylum())
                return Asylum.Retarget(actionID, SimpleTarget.Self);
            if (RaidwideLiturgyOfTheBell())
                return LiturgyOfTheBell.Retarget(actionID, SimpleTarget.Self);

            #endregion

            #region Weaves

            if (CanWeave())
            {
                if (IsEnabled(Preset.WHM_ST_MainCombo_PresenceOfMind) &&
                    ActionReady(PresenceOfMind) &&
                    ActionWatching.NumberOfGcdsUsed >= 3 &&
                    !HasStatusEffect(Buffs.SacredSight))
                    return PresenceOfMind;

                if (IsEnabled(Preset.WHM_ST_MainCombo_Assize) &&
                    ActionReady(Assize) &&
                    HasBattleTarget() && GetTargetDistance() <= 20)
                    return Assize;

                if (IsEnabled(Preset.WHM_ST_MainCombo_Lucid) &&
                    Role.CanLucidDream(WHM_STDPS_Lucid))
                    return Role.LucidDreaming;
            }

            #endregion

            #region GCDS and Casts

            // DoTs
            if (IsEnabled(Preset.WHM_ST_MainCombo_DoT) && NeedsDoT())
                return OriginalHook(Aero);
            
            // Blood Lily Spend
            if (IsEnabled(Preset.WHM_ST_MainCombo_Misery) && BloodLilyReady && 
                (AlmostFullLily || HasStatusEffect(Buffs.PresenceOfMind) || WHM_ST_MainCombo_Misery_Option == 1))
                return AfflatusMisery;

            // Glare IV
            if (IsEnabled(Preset.WHM_ST_MainCombo_GlareIV) &&
                HasStatusEffect(Buffs.SacredSight))
                return Glare4;

            // Lily Heal Overcap
            if (IsEnabled(Preset.WHM_ST_MainCombo_LilyOvercap) && ActionReady(AfflatusRapture) &&
                LevelChecked(AfflatusMisery) &&  !BloodLilyReady &&
                (FullLily || gauge.Lily == 2 && 20000 - gauge.LilyTimer <= WHM_STDPS_LilyOvercap * 1000))
                return AfflatusRapture;

            #region Movement Options

            if (IsMoving())
            {
                var dotAction = OriginalHook(Aero);
                AeroList.TryGetValue(dotAction, out var dotDebuffID);
                var target = SimpleTarget.DottableEnemy(
                    dotAction, dotDebuffID, 0, 30, 99);
                if (IsEnabled(Preset.WHM_ST_MainCombo_Move_DoT) &&
                    target is not null)
                    return dotAction.Retarget(replacedAction, target);
            }
            #endregion

            // Needed Because of Button Selection
            return OriginalHook(Stone1);

            #endregion
        }
    }

    internal class WHM_AoE_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_AoE_DPS;

        private static int AssizeCount =>
            ActionWatching.CombatActions.Count(x => x == Assize);

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Holy or Holy3))
                return actionID;

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;


            #region Swiftcast Opener

            if (IsEnabled(Preset.WHM_AoE_DPS_SwiftHoly) &&
                ActionReady(Role.Swiftcast) &&
                AssizeCount == 0 && !IsMoving() && InCombat())
                return Role.Swiftcast;

            if (IsEnabled(Preset.WHM_AoE_DPS_SwiftHoly) &&
                WasLastAction(Role.Swiftcast))
                return actionID;

            #endregion

            #region Special Feature Raidwide

            if (RaidwideTemperance())
                return OriginalHook(Temperance);
            if (RaidwideAsylum())
                return Asylum.Retarget([Holy, Holy3], SimpleTarget.Self);
            if (RaidwideLiturgyOfTheBell())
                return LiturgyOfTheBell.Retarget([Holy, Holy3], SimpleTarget.Self);

            #endregion

            #region Weaves

            if (CanWeave() || IsMoving())
            {
                if (IsEnabled(Preset.WHM_AoE_DPS_Assize) &&
                    ActionReady(Assize) &&
                    HasBattleTarget() && GetTargetDistance() <= 20)
                    return Assize;

                if (IsEnabled(Preset.WHM_AoE_DPS_PresenceOfMind) &&
                    ActionReady(PresenceOfMind) &&
                    ActionWatching.NumberOfGcdsUsed >= 4 &&
                    !HasStatusEffect(Buffs.SacredSight))
                    return PresenceOfMind;
            }

            #endregion

            #region GCDS and Casts

            if (IsEnabled(Preset.WHM_AoE_DPS_Misery) && HasBattleTarget() && BloodLilyReady && 
                (AlmostFullLily || HasStatusEffect(Buffs.PresenceOfMind) || WHM_AoE_DPS_Misery_Option == 1))
                return AfflatusMisery;
            
            if (IsEnabled(Preset.WHM_AoE_DPS_GlareIV) &&
                HasStatusEffect(Buffs.SacredSight))
                return OriginalHook(Glare4);

            if (IsEnabled(Preset.WHM_AoE_DPS_LilyOvercap) && ActionReady(AfflatusRapture) &&
                LevelChecked(AfflatusMisery) &&  !BloodLilyReady &&
                (FullLily || gauge.Lily == 2 && 20000 - gauge.LilyTimer <= WHM_AoEDPS_LilyOvercap * 1000))
                return AfflatusRapture;

            var dotAction = OriginalHook(Aero);
            AeroList.TryGetValue(dotAction, out var dotDebuffID);
            var target = SimpleTarget.DottableEnemy(dotAction, dotDebuffID,
                WHM_AoE_MainCombo_DoT_HPThreshold,
                WHM_AoE_MainCombo_DoT_Reapply,
                WHM_AoE_MainCombo_DoT_MaxTargets);

            if (IsEnabled(Preset.WHM_AoE_MainCombo_DoT) &&
                ActionReady(dotAction) && target != null)
                return OriginalHook(Aero).Retarget([Holy, Holy3], target);

            #endregion

            return actionID;
        }
    }

    #endregion
    
    #region Simple Heals
    internal class WHM_SimpleST_Heals : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_SimpleSTHeals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cure)
                return actionID;
            
            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
            
            if (ActionReady(Benediction) && 
                GetTargetHPPercent(healTarget) <= 20)
                return Benediction.RetargetIfEnabled(OptionalTarget, Cure);
            
            if (ActionReady(Tetragrammaton) && 
                GetTargetHPPercent(healTarget) <= 50)
                return Tetragrammaton.RetargetIfEnabled(OptionalTarget, Cure);
            
            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);
            
            if (ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget) >= 40 &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(OptionalTarget, Cure);
            
            if (CanWeave() && Role.CanLucidDream(6500))
                return Role.LucidDreaming;
            
            if (ActionReady(Asylum) && 
                !InBossEncounter() &&
                TimeStoodStill >= TS.FromSeconds(5))
                return Asylum.Retarget(Cure ,SimpleTarget.Self);
            
            if (ActionReady(Regen) && 
                GetStatusEffect(Buffs.Regen, healTarget) == null &&  
                GetTargetHPPercent(healTarget) >= 40)
                return Regen.RetargetIfEnabled(OptionalTarget, Cure);

            if (ActionReady(DivineBenison) && 
                GetStatusEffect(Buffs.DivineBenison, healTarget) == null)
                return DivineBenison.RetargetIfEnabled(OptionalTarget, Cure);

            if (ActionReady(Aquaveil) && IsOffCooldown(Aquaveil) && (healTarget.IsInParty() && healTarget.GetRole() is CombatRole.Tank || !IsInParty()))
                return Aquaveil.RetargetIfEnabled(OptionalTarget, Cure);

            if (ActionReady(OriginalHook(Temperance)) && 
                !InBossEncounter())
                return OriginalHook(Temperance);
            
            if (ActionReady(AfflatusSolace) && !BloodLilyReady)
                return AfflatusSolace.RetargetIfEnabled(OptionalTarget, Cure);

            if (ActionReady(ThinAir) && GetRemainingCharges(ThinAir) == 2)
                return ThinAir;
            
            return LevelChecked(Cure2)
                ? Cure2.RetargetIfEnabled(OptionalTarget, Cure)
                : Cure.RetargetIfEnabled(OptionalTarget);
        }
    }
    
    internal class WHM_Simple_AoEHeals : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_Simple_AoEHeals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Medica1)
                return actionID;

            if (ActionReady(Assize))
                return Assize;
            
            if (ActionReady(Asylum) &&
                TimeStoodStill >= TS.FromSeconds(5))
                return Asylum.Retarget(Medica1, SimpleTarget.Self);

            if (CanWeave() && Role.CanLucidDream(WHM_AoEHeals_Lucid))
                return Role.LucidDreaming;
            
            if (ActionReady(OriginalHook(Temperance)) && 
                (GetPartyAvgHPPercent() <= 70 ||
                 RaidWideCasting() ||
                 HasStatusEffect(Buffs.DivineGrace)))
                return OriginalHook(Temperance);
            
            if (LevelChecked(LiturgyOfTheBell) &&
                IsOffCooldown(LiturgyOfTheBell) &&
                (GetPartyAvgHPPercent() <= 50 ||
                 RaidWideCasting()))
                return LiturgyOfTheBell;

            if (ActionReady(PlenaryIndulgence) &&
                (GetPartyAvgHPPercent() <= 70 ||
                 RaidWideCasting()))
                return PlenaryIndulgence;
            
            if (ActionReady(AfflatusRapture) && !BloodLilyReady)
                return AfflatusRapture;
            
            if (ActionReady(ThinAir) && GetRemainingCharges(ThinAir) == 2)
                return ThinAir;

            if (ActionReady(Cure3) &&
                NumberOfAlliesInRange(Cure3) >= GetPartyMembers().Count * .75)
                return Cure3.RetargetIfEnabled(OptionalTarget, Medica1);

            if (ActionReady(OriginalHook(Medica2)) &&
                !HasStatusEffect(Buffs.Medica2) &&
                !HasStatusEffect(Buffs.Medica3))
                return OriginalHook(Medica2);

            return actionID;
        }
    }
    #endregion

    #region Heals

    internal class WHM_ST_Heals : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_STHeals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cure)
                return actionID;

            #region Variables

            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            var canThinAir = LevelChecked(ThinAir) &&
                             !HasStatusEffect(Buffs.ThinAir) &&
                             GetRemainingCharges(ThinAir) >
                             WHM_STHeals_ThinAir;

            #endregion

            #region Special Feature Raidwide

            if (RaidwideTemperance())
                return OriginalHook(Temperance);
            if (RaidwideAsylum())
                return Asylum.Retarget(Cure, SimpleTarget.Self);
            if (RaidwideLiturgyOfTheBell())
                return LiturgyOfTheBell.Retarget(Cure, SimpleTarget.Self);

            #endregion

            #region Priority Cleansing
            
            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);
            
            if (IsEnabled(Preset.WHM_STHeals_Esuna) &&
                ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, WHM_STHeals_IncludeShields) >= WHM_STHeals_Esuna &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(OptionalTarget, Cure);

            #endregion

            if (IsEnabled(Preset.WHM_STHeals_Lucid) && CanWeave() &&
                Role.CanLucidDream(WHM_STHeals_Lucid))
                return Role.LucidDreaming;

            // Divine Caress
            if (IsEnabled(Preset.WHM_STHeals_Temperance) &&
                HasStatusEffect(Buffs.DivineGrace) &&
                (!WHM_STHeals_TemperanceOptions[1] || !InBossEncounter()) &&
                (!WHM_STHeals_TemperanceOptions[0] || CanWeave()))
                return OriginalHook(Temperance);

            //Priority List
            for (var i = 0; i < WHM_ST_Heals_Priority.Count; i++)
            {
                var index = WHM_ST_Heals_Priority.IndexOf(i + 1);
                var config = GetMatchingConfigST(index, OptionalTarget,
                    out var spell, out var enabled);

                if (enabled)
                {
                    if (GetTargetHPPercent(healTarget,
                            WHM_STHeals_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell.RetargetIfEnabled(OptionalTarget, Cure);
                }
            }

            if (LevelChecked(Cure2))
                return IsEnabled(Preset.WHM_STHeals_ThinAir) && canThinAir
                    ? ThinAir
                    : Cure2.RetargetIfEnabled(OptionalTarget, Cure);

            return Cure.RetargetIfEnabled(OptionalTarget);
        }
    }

    internal class WHM_AoEHeals : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_AoEHeals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Medica1)
                return actionID;

            #region Variables

            var canThinAir = LevelChecked(ThinAir) &&
                             !HasStatusEffect(Buffs.ThinAir) &&
                             GetRemainingCharges(ThinAir) >
                             WHM_AoEHeals_ThinAir;

            #endregion

            #region Special Feature Raidwide

            if (RaidwideTemperance())
                return OriginalHook(Temperance);
            if (RaidwideAsylum())
                return Asylum.Retarget(Medica1, SimpleTarget.Self);
            if (RaidwideLiturgyOfTheBell())
                return LiturgyOfTheBell.Retarget(Medica1, SimpleTarget.Self);

            #endregion

            if (IsEnabled(Preset.WHM_AoEHeals_Lucid) &&
                CanWeave() &&
                Role.CanLucidDream(WHM_AoEHeals_Lucid))
                return Role.LucidDreaming;

            //Priority List
            for (var i = 0; i < WHM_AoE_Heals_Priority.Count; i++)
            {
                var index = WHM_AoE_Heals_Priority.IndexOf(i + 1);
                var config = GetMatchingConfigAoE(index, OptionalTarget,
                    out var spell, out var enabled);

                if (enabled && GetPartyAvgHPPercent() <= config &&
                    ActionReady(spell))
                    return IsEnabled(Preset.WHM_AoEHeals_ThinAir) && canThinAir &&
                           spell is Cure3 or Medica2 or Medica3
                        ? ThinAir
                        : spell.RetargetIfEnabled(OptionalTarget, Medica1);
            }
            return actionID;
        }
    }

    #endregion

    #region Small Features

    internal class WHM_SolaceMisery : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_SolaceMisery;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AfflatusSolace)
                return actionID;

            if (BloodLilyReady)
                return AfflatusMisery;

            if (IsEnabled(Preset.WHM_Re_Solace))
                return AfflatusSolace.Retarget(SimpleTarget.Stack.AllyToHeal);

            return actionID;
        }
    }

    internal class WHM_RaptureMisery : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_RaptureMisery;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AfflatusRapture)
                return actionID;

            if (BloodLilyReady)
                return AfflatusMisery;

            return actionID;
        }
    }

    internal class WHM_CureSync : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_CureSync;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cure2)
                return actionID;

            if (!LevelChecked(Cure2))
                return IsEnabled(Preset.WHM_Re_Cure)
                    ? Cure.Retarget(Cure2, SimpleTarget.Stack.AllyToHeal)
                    : Cure;

            return IsEnabled(Preset.WHM_Re_Cure)
                ? Cure2.Retarget(Cure2, SimpleTarget.Stack.AllyToHeal)
                : Cure2;
        }
    }

    internal class WHM_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_Raise;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            var canThinAir = !HasStatusEffect(Buffs.ThinAir) && ActionReady
                (ThinAir);

            if (HasStatusEffect(Role.Buffs.Swiftcast))
                return IsEnabled(Preset.WHM_ThinAirRaise) && canThinAir ? ThinAir :
                    IsEnabled(Preset.WHM_Raise_Retarget) ? Raise.Retarget(
                        Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise) : Raise;

            return actionID;
        }
    }

    internal class WHM_Mit_ST : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_Mit_ST;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Aquaveil)
                return actionID;

            var healTarget = SimpleTarget.Stack.AllyToHeal;
            var benisonShield = GetStatusEffect(Buffs.DivineBenison, healTarget);

            if (ActionReady(Aquaveil))
                return IsEnabled(Preset.WHM_Re_Aquaveil)
                    ? Aquaveil.Retarget(Aquaveil, SimpleTarget.Stack.AllyToHeal)
                    : Aquaveil;

            if (WHM_AquaveilOptions[0] &&
                ActionReady(DivineBenison) &&
                benisonShield == null)
                return IsEnabled(Preset.WHM_Re_DivineBenison)
                    ? DivineBenison.Retarget(Aquaveil, SimpleTarget.Stack.AllyToHeal)
                    : DivineBenison;

            if (WHM_AquaveilOptions[1] &&
                ActionReady(Tetragrammaton) &&
                GetTargetHPPercent(healTarget) < WHM_Aquaveil_TetraThreshold)
                return IsEnabled(Preset.WHM_Re_Tetragrammaton)
                    ? Tetragrammaton.Retarget(Aquaveil, SimpleTarget.Stack
                        .AllyToHeal)
                    : Tetragrammaton;

            return actionID;
        }
    }

    internal class WHM_Mit_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_Mit_AoE;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Asylum)
                return actionID;

            var asylumTarget =
                (WHM_AsylumOptions[0]
                    ? SimpleTarget.HardTarget.IfHostile()
                    : null) ??
                (WHM_AsylumOptions[1]
                    ? SimpleTarget.HardTarget.IfFriendly()
                    : null) ??
                SimpleTarget.Self;

            if (ActionReady(OriginalHook(Temperance)) &&
                IsOnCooldown(Asylum))
                return OriginalHook(Temperance);

            return IsEnabled(Preset.WHM_Re_Asylum)
                ? Asylum.Retarget(Asylum, asylumTarget)
                : Asylum;
        }
    }

    internal class WHM_Retarget : CustomCombo
    {
        protected internal override Preset Preset => Preset.WHM_Retargets;

        protected override uint Invoke(uint actionID)
        {
            var healStack = SimpleTarget.Stack.AllyToHeal;

            if (!EZ.Throttle("WHMRetargetingFeature", TS.FromSeconds(.1)))
                return actionID;

            if (IsEnabled(Preset.WHM_Re_Cure))
            {
                Cure.Retarget(healStack, dontCull: true);
                Cure2.Retarget(healStack, dontCull: true);
            }

            if (IsEnabled(Preset.WHM_Re_Solace))
                AfflatusSolace.Retarget(healStack, dontCull: true);

            if (IsEnabled(Preset.WHM_Re_Aquaveil))
                Aquaveil.Retarget(healStack, dontCull: true);

            if (IsEnabled(Preset.WHM_Re_Asylum))
            {
                var asylumTarget =
                    (WHM_AsylumOptions[0]
                        ? SimpleTarget.HardTarget.IfHostile()
                        : null) ??
                    (WHM_AsylumOptions[1]
                        ? SimpleTarget.HardTarget.IfFriendly()
                        : null) ??
                    SimpleTarget.Self;
                Asylum.Retarget(asylumTarget, dontCull: true);
            }

            if (IsEnabled(Preset.WHM_Re_LiturgyOfTheBell))
            {
                var bellTarget =
                    (WHM_LiturgyOfTheBellOptions[0]
                        ? SimpleTarget.HardTarget.IfHostile()
                        : null) ??
                    (WHM_LiturgyOfTheBellOptions[1]
                        ? SimpleTarget.HardTarget.IfFriendly()
                        : null) ??
                    SimpleTarget.Self;
                LiturgyOfTheBell.Retarget(bellTarget, dontCull: true);
            }

            if (IsEnabled(Preset.WHM_Re_Cure3))
                Cure3.Retarget(healStack, dontCull: true);

            if (IsEnabled(Preset.WHM_Re_Benediction))
                Benediction.Retarget(healStack, dontCull: true);

            if (IsEnabled(Preset.WHM_Re_Tetragrammaton))
                Tetragrammaton.Retarget(healStack, dontCull: true);

            if (IsEnabled(Preset.WHM_Re_Regen))
                Regen.Retarget(healStack, dontCull: true);

            if (IsEnabled(Preset.WHM_Re_DivineBenison))
                DivineBenison.Retarget(healStack, dontCull: true);

            return actionID;
        }
    }

    #endregion
}