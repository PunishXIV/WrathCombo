#region

using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM : Healer
{
    #region DPS

    internal class WHM_ST_MainCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_ST_MainCombo;

        protected override uint Invoke(uint actionID)
        {
            #region Button Selection

            bool actionFound;

            if (Config.WHM_ST_MainCombo_Adv &&
                Config.WHM_ST_MainCombo_Adv_Actions.Count > 0)
            {
                var isStoneGlare = Config.WHM_ST_MainCombo_Adv_Actions[0] &&
                                   StoneGlareList.Contains(actionID);
                var isAero = Config.WHM_ST_MainCombo_Adv_Actions[1] &&
                             AeroList.ContainsKey(actionID);
                var isStone2 = Config.WHM_ST_MainCombo_Adv_Actions[2] &&
                               actionID is Stone2;
                actionFound = isStoneGlare || isAero || isStone2;
            }
            else
            {
                actionFound = StoneGlareList.Contains(actionID); //default handling
            }

            // If the action is not in the list, return the actionID
            if (!actionFound)
                return actionID;

            #endregion

            #region Opener

            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            #endregion

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            if (!InCombat()) return actionID;

            #region Weaves

            if (CanSpellWeave())
            {
                if (Variant.CanRampart(CustomComboPreset.WHM_DPS_Variant_Rampart))
                    return Variant.Rampart;

                if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind) &&
                    ActionReady(PresenceOfMind) && !HasStatusEffect(Buffs.SacredSight))
                    return PresenceOfMind;

                if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize) &&
                    ActionReady(Assize))
                    return Assize;

                if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid) &&
                    Role.CanLucidDream(Config.WHM_STDPS_Lucid))
                    return Role.LucidDreaming;

                if (Variant.CanSpiritDart(
                        CustomComboPreset.WHM_DPS_Variant_SpiritDart))
                    return Variant.SpiritDart;
            }

            #endregion

            #region GCDS and Casts

            // DoTs
            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && NeedsDoT())
                return OriginalHook(Aero);

            // Glare IV
            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_GlareIV) &&
                HasStatusEffect(Buffs.SacredSight))
                return Glare4;

            // Lily Heal Overcap
            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_LilyOvercap) &&
                ActionReady(AfflatusRapture) &&
                (FullLily || AlmostFullLily))
                return AfflatusRapture;

            // Blood Lily Spend
            if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Misery_oGCD) &&
                BloodLilyReady)
                return AfflatusMisery;

            // Needed Because of Button Selection
            return OriginalHook(Stone1);

            #endregion
        }
    }

    internal class WHM_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_AoE_DPS;

        private static int AssizeCount =>
            ActionWatching.CombatActions.Count(x => x == Assize);

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Holy or Holy3))
                return actionID;

            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();


            #region Swiftcast Opener

            if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_SwiftHoly) &&
                ActionReady(Role.Swiftcast) &&
                AssizeCount == 0 && !IsMoving() && InCombat())
                return Role.Swiftcast;

            if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_SwiftHoly) &&
                WasLastAction(Role.Swiftcast))
                return actionID;

            #endregion

            #region Weaves

            if (CanSpellWeave() || IsMoving())
            {
                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Assize) &&
                    ActionReady(Assize))
                    return Assize;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_PresenceOfMind) &&
                    ActionReady(PresenceOfMind) && !HasStatusEffect(Buffs.SacredSight))
                    return PresenceOfMind;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Lucid) &&
                    Role.CanLucidDream(Config.WHM_AoEDPS_Lucid))
                    return Role.LucidDreaming;

                if (Variant.CanRampart(CustomComboPreset.WHM_DPS_Variant_Rampart))
                    return Variant.Rampart;

                if (Variant.CanSpiritDart(CustomComboPreset
                        .WHM_DPS_Variant_SpiritDart))
                    return Variant.SpiritDart;
            }

            #endregion

            #region GCDS and Casts

            // Glare IV
            if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_GlareIV) &&
                HasStatusEffect(Buffs.SacredSight))
                return OriginalHook(Glare4);

            if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_LilyOvercap) &&
                ActionReady(AfflatusRapture) &&
                (FullLily || AlmostFullLily))
                return AfflatusRapture;

            if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Misery) &&
                BloodLilyReady &&
                HasBattleTarget())
                return AfflatusMisery;

            #endregion

            return actionID;
        }
    }

    #endregion

    #region Heals

    internal class WHM_ST_Heals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_STHeals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cure)
                return actionID;

            #region Variables

            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            var canThinAir = LevelChecked(ThinAir) &&
                             !HasStatusEffect(Buffs.ThinAir) &&
                             GetRemainingCharges(ThinAir) >
                             Config.WHM_STHeals_ThinAir;


            var canRegen = ActionReady(Regen) &&
                             !JustUsedOn(Regen, healTarget) &&
                             GetStatusEffectRemainingTime(Buffs.Regen, healTarget)
                             <= Config.WHM_STHeals_RegenTimer && //Refresh Time Threshold
                             GetTargetHPPercent(healTarget, Config.WHM_STHeals_IncludeShields)
                             >= Config.WHM_STHeals_RegenHPLower &&
                             GetTargetHPPercent(healTarget, Config.WHM_STHeals_IncludeShields)
                             <= Config.WHM_STHeals_RegenHPUpper;  
            #endregion

            #region Priority Cleansing

            if (IsEnabled(CustomComboPreset.WHM_STHeals_Esuna) &&
                ActionReady(Role.Esuna) &&
                GetTargetHPPercent(
                    healTarget, Config.WHM_STHeals_IncludeShields) >=
                Config.WHM_STHeals_Esuna &&
                HasCleansableDebuff(healTarget))
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Cure);

            #endregion

            #region OGCD Tools

            if (IsEnabled(CustomComboPreset.WHM_STHeals_Lucid) && CanSpellWeave() &&
                Role.CanLucidDream(Config.WHM_STHeals_Lucid))
                return Role.LucidDreaming;

            // OGCD Priority List
            foreach (var prio in Config.WHM_ST_Heals_Priority.Items.OrderBy(x => x))
            {
                var index = Config.WHM_ST_Heals_Priority.IndexOf(prio);
                var config = GetMatchingConfigST(index, OptionalTarget,
                    out var spell, out var enabled);

                if (!enabled) continue;

                if (GetTargetHPPercent(healTarget,
                        Config.WHM_STHeals_IncludeShields) <= config &&
                    ActionReady(spell))
                    return spell
                        .RetargetIfEnabled(OptionalTarget, Cure);
            }

            #endregion

            #region GCD Tools

            if (IsEnabled(CustomComboPreset.WHM_STHeals_Regen) && canRegen)
                return Regen
                    .RetargetIfEnabled(OptionalTarget, Cure);

            if (IsEnabled(CustomComboPreset.WHM_STHeals_Solace) && CanLily &&
                ActionReady(AfflatusSolace))
                return AfflatusSolace
                    .RetargetIfEnabled(OptionalTarget, Cure);

            if (ActionReady(Cure2))
            {
                if (IsEnabled(CustomComboPreset.WHM_STHeals_ThinAir) && canThinAir)
                    return ThinAir;
                return Cure2
                    .RetargetIfEnabled(OptionalTarget, Cure);
            }

            #endregion

            return actionID
                .RetargetIfEnabled(OptionalTarget, Cure);
        }
    }

    internal class WHM_AoEHeals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_AoEHeals;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Medica1)
                return actionID;

            #region Variables

            var canThinAir = LevelChecked(ThinAir) &&
                             !HasStatusEffect(Buffs.ThinAir) &&
                             GetRemainingCharges(ThinAir) >
                             Config.WHM_AoEHeals_ThinAir;

            var canPlenary = ActionReady(PlenaryIndulgence) &&
                             (!Config.WHM_AoEHeals_PlenaryWeave ||
                              Config.WHM_AoEHeals_PlenaryWeave &&
                              CanSpellWeave());

            var canAssize = ActionReady(Assize) &&
                            (!Config.WHM_AoEHeals_AssizeWeave ||
                             Config.WHM_AoEHeals_AssizeWeave &&
                             CanSpellWeave());

            var healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            var hasMedica2 = GetStatusEffect(Buffs.Medica2, healTarget);
            var hasMedica3 = GetStatusEffect(Buffs.Medica3, healTarget);
            
            
            var temperanceInRaidwideContent =
                ContentCheck.IsInConfiguredContent(
                    Config.WHM_AoEHeals_TemperanceRaidwideDifficulty,
                    Config.WHM_AoEHeals_TemperanceRaidwideDifficultyListSet);
            var temperanceHPCheckPassed =
                (Config.WHM_AoEHeals_TemperanceRaidwide &&
                 Config.WHM_AoEHeals_TemperanceRaidwidePrioritization &&
                 temperanceInRaidwideContent &&
                 RaidWideCasting()) ||
                GetPartyAvgHPPercent() <= Config.WHM_AoEHeals_TemperanceHP;
            
            var bellTarget =
                (IsEnabled(CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell_Enemy)
                    ? SimpleTarget.HardTarget
                    : null) ??
                (IsEnabled(CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell_Allies)
                    ? SimpleTarget.Stack.OverridesAllies
                    : null) ??
                SimpleTarget.Self;
            var asylumTarget =
                (IsEnabled(CustomComboPreset.WHM_AoEHeals_Asylum_Enemy)
                    ? SimpleTarget.HardTarget
                    : null) ??
                (IsEnabled(CustomComboPreset.WHM_AoEHeals_Asylum_Allies)
                    ? SimpleTarget.Stack.OverridesAllies
                    : null) ??
                SimpleTarget.Self;

            #endregion

            #region OGCD Tools

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Assize) &&
                canAssize)
                return Assize;

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Plenary) &&
                canPlenary)
                return PlenaryIndulgence;

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Temperance) &&
                ActionReady(Temperance) &&
                (!Config.WHM_AoEHeals_TemperanceWeave || CanSpellWeave()) &&
                temperanceHPCheckPassed &&
                ContentCheck.IsInConfiguredContent(
                    Config.WHM_AoEHeals_TemperanceDifficulty,
                    Config.WHM_AoEHeals_TemperanceDifficultyListSet) &&
                (!Config.WHM_AoEHeals_TemperanceRaidwide || 
                 (RaidWideCasting() && temperanceInRaidwideContent)))
                return Temperance;

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_DivineCaress) &&
                ActionReady(DivineCaress))
                return OriginalHook(DivineCaress);

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_LiturgyOfTheBell) &&
                ActionReady(LiturgyOfTheBell) &&
                !HasStatusEffect(Buffs.LiturgyOfTheBell) &&
                !JustUsed(LiturgyOfTheBell) &&
                BellRaidwideCheckPassed &&
                ContentCheck.IsInConfiguredContent(
                    Config.WHM_AoEHeals_LiturgyDifficulty,
                    Config.WHM_AoEHeals_LiturgyDifficultyListSet))
                return LiturgyOfTheBell.Retarget(Medica1, bellTarget);

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Asylum) &&
                ActionReady(Asylum) &&
                !IsMoving() &&
                (!Config.WHM_AoEHeals_AsylumRaidwideOnly || RaidWideCasting()))
                return Asylum.Retarget(Medica1, asylumTarget);

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Lucid) &&
                CanSpellWeave() &&
                Role.CanLucidDream(Config.WHM_AoEHeals_Lucid))
                return Role.LucidDreaming;

            #endregion

            #region GCD Tools

            // Blood Overcap
            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Misery) &&
                gauge.BloodLily == 3)
                return AfflatusMisery;

            // Heals
            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Rapture) &&
                ActionReady(AfflatusRapture) && CanLily)
                return AfflatusRapture;

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_ThinAir) && canThinAir)
                return ThinAir;

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Medica2)
                && ((hasMedica2 == null && hasMedica3 == null) || // No Medica buffs
                    (hasMedica2 != null && // Medica buff, but falling off
                     hasMedica2.RemainingTime <= Config.WHM_AoEHeals_MedicaTime) ||
                    (hasMedica3 != null &&
                     hasMedica3.RemainingTime <= Config.WHM_AoEHeals_MedicaTime))
                && (ActionReady(Medica2) || ActionReady(Medica3)))
                return LevelChecked(Medica3) ? Medica3 : Medica2;

            if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Cure3) &&
                ActionReady(Cure3) &&
                (LocalPlayer.CurrentMp >= Config.WHM_AoEHeals_Cure3MP ||
                 HasStatusEffect(Buffs.ThinAir)))
                return Cure3;

            #endregion

            return actionID;
        }
    }

    #endregion

    #region Small Features

    internal class WHM_SolaceMisery : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_SolaceMisery;

        protected override uint Invoke(uint actionID) =>
            actionID is AfflatusSolace && gauge.BloodLily == 3
                ? AfflatusMisery
                : actionID;
    }

    internal class WHM_RaptureMisery : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_RaptureMisery;

        protected override uint Invoke(uint actionID) =>
            actionID is AfflatusRapture && gauge.BloodLily == 3
                ? AfflatusMisery
                : actionID;
    }

    internal class WHM_CureSync : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_CureSync;

        protected override uint Invoke(uint actionID) =>
            actionID is Cure2 && !LevelChecked(Cure2)
                ? Cure
                : actionID;
    }

    internal class WHM_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.WHM_Raise;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            var canThinAir = !HasStatusEffect(Buffs.ThinAir) && ActionReady
                (ThinAir);

            if (HasStatusEffect(Role.Buffs.Swiftcast))
                return IsEnabled(CustomComboPreset.WHM_ThinAirRaise) && canThinAir
                    ? ThinAir
                    : IsEnabled(CustomComboPreset.WHM_Raise_Retarget)
                        ? Raise.Retarget(Role.Swiftcast,
                            SimpleTarget.Stack.AllyToRaise)
                        : Raise;

            return actionID;
        }
    }

    #endregion
}
