using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
using ECommons.GameFunctions;
using WrathCombo.Extensions;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.SGE.Config;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;
namespace WrathCombo.Combos.PvE;

internal partial class SGE : Healer
{
    #region Simple DPS Mode

    internal class SGE_ST_Simple_DPS : CustomCombo
    {
        private static uint[] DosisActions => [.. DosisList.Keys];

        protected internal override Preset Preset => Preset.SGE_ST_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!DosisActions.Contains(actionID))
                return actionID;

            // Kardia Reminder
            if (LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia) &&
                Target is not null)
                return Kardia
                    .Retarget(actionID, Target);

            //Content skills
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (CanWeave() && !HasStatusEffect(Buffs.Eukrasia))
            {
                // Lucid Dreaming
                if (Role.CanLucidDream(7500))
                    return Role.LucidDreaming;

                // Addersgall Protection
                if (ActionReady(Druochole) && Addersgall >= 3)
                    return Druochole.RetargetIfEnabled(null, DosisActions);

                // Psyche
                if (ActionReady(Psyche) && InCombat())
                    return Psyche;

                // Rhizomata
                if (ActionReady(Rhizomata) && Addersgall < 1)
                    return Rhizomata;

                //Soteria
                if (ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                    return Soteria;
            }

            if (HasBattleTarget() && !HasStatusEffect(Buffs.Eukrasia))
            {
                if (LevelChecked(Eukrasia) && InCombat() &&
                    !JustUsedOn(DosisList[OriginalHook(Dosis)].Eukrasian, CurrentTarget) &&
                    CanApplyStatus(CurrentTarget, DosisList[OriginalHook(Dosis)].Debuff))
                {
                    const float refreshTimer = 5;
                    const int hpThreshold = 1;

                    if (GetTargetHPPercent() > hpThreshold &&
                        ((DosisDebuff is null && DyskrasiaDebuff is null) ||
                         DosisDebuff?.RemainingTime <= refreshTimer ||
                         DyskrasiaDebuff?.RemainingTime <= refreshTimer))
                        return Eukrasia;
                }

                // Phlegma
                if (InCombat() && InActionRange(OriginalHook(Phlegma)) &&
                    ActionReady(Phlegma))
                {
                    //If not enabled or not high enough level, follow slider
                    if ((!LevelChecked(Psyche)) &&
                        GetRemainingCharges(OriginalHook(Phlegma)) > 1)
                        return OriginalHook(Phlegma);

                    //If enabled and high enough level, burst
                    if (((GetCooldownRemainingTime(Psyche) > 40 && MaxPhlegma) ||
                         IsOffCooldown(Psyche) ||
                         JustUsed(Psyche, 5f)))
                        return OriginalHook(Phlegma);
                }

                // Movement Options
                if (InCombat() && IsMoving() && HasBattleTarget())
                {
                    //Toxikon
                    if (ActionReady(Toxikon) && HasAddersting())
                        return OriginalHook(Toxikon);

                    // Dyskrasia
                    if (ActionReady(Dyskrasia) && InActionRange(Dyskrasia))
                        return OriginalHook(Dyskrasia);
                    //Eukrasia
                    if (ActionReady(Eukrasia) && !HasStatusEffect(Buffs.Eukrasia))
                        return Eukrasia;
                }
            }

            return actionID;
        }
    }

    internal class SGE_AoE_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!DyskrasiaList.Contains(actionID) ||
                HasStatusEffect(Buffs.Eukrasia))
                return actionID;

            //Occult skills
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            if (ActionReady(Kerachole) && HasAddersgall() &&
                CanWeave() && RaidWideCasting())
                return Kerachole;

            if (ActionReady(Holos) && CanWeave() && RaidWideCasting() &&
                GetPartyAvgHPPercent() <= 70)
                return Holos;

            if (RaidwideEprognosis())
                return HasStatusEffect(Buffs.Eukrasia)
                    ? OriginalHook(Prognosis)
                    : Eukrasia;

            if (CanWeave())
            {
                // Lucid Dreaming
                if (Role.CanLucidDream(7500))
                    return Role.LucidDreaming;

                // Addersgall Protection
                if (ActionReady(Druochole) && Addersgall >= 3)
                    return Druochole
                        .RetargetIfEnabled(null, OriginalHook(Dyskrasia));

                // Psyche
                if (ActionReady(Psyche) && HasBattleTarget() &&
                    InActionRange(Psyche))
                    return Psyche;

                // Rhizomata
                if (ActionReady(Rhizomata) && Addersgall < 1)
                    return Rhizomata;

                //Soteria
                if (ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                    return Soteria;
            }

            //Eukrasia for DoT
            if (IsOffCooldown(Eukrasia) &&
                !JustUsedOn(EukrasianDyskrasia, CurrentTarget) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                HasBattleTarget() && InActionRange(Dyskrasia) &&
                CanApplyStatus(CurrentTarget, Debuffs.EukrasianDyskrasia) &&
                GetTargetHPPercent() > 25 &&
                ((DyskrasiaDebuff is null && DosisDebuff is null) ||
                 DyskrasiaDebuff?.RemainingTime <= 4 ||
                 DosisDebuff?.RemainingTime <= 4))
                return Eukrasia;

            //Phlegma
            if (ActionReady(Phlegma) &&
                HasBattleTarget() &&
                InActionRange(OriginalHook(Phlegma)))
                return OriginalHook(Phlegma);

            //Toxikon
            if (ActionReady(Toxikon) &&
                HasBattleTarget() && HasAddersting() &&
                InActionRange(OriginalHook(Toxikon)))
                return OriginalHook(Toxikon);

            //Pneuma
            if (ActionReady(Pneuma) && HasBattleTarget() &&
                InActionRange(Pneuma))
                return Pneuma;

            return actionID;
        }
    }

    #endregion
    
    #region Advanced DPS Mode

    internal class SGE_ST_DPS_AdvancedMode : CustomCombo
    {
        private static uint[] DosisActions => SGE_ST_DPS_Adv
            ? [Dosis2]
            : [.. DosisList.Keys];

        protected internal override Preset Preset => Preset.SGE_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!DosisActions.Contains(actionID))
                return actionID;

            // Kardia Reminder
            if (IsEnabled(Preset.SGE_ST_DPS_Kardia) &&
                LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia) &&
                Target is not null)
                return Kardia
                    .Retarget(actionID, Target);

            // Opener for SGE
            if (IsEnabled(Preset.SGE_ST_DPS_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            //Content Actions
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Raidwide Feature

            if (RaidwideKerachole())
                return Kerachole;

            if (RaidwideHolos())
                return Holos;

            if (RaidwideEprognosis())
                return HasStatusEffect(Buffs.Eukrasia)
                    ? OriginalHook(Prognosis)
                    : Eukrasia;

            #endregion

            if (CanWeave() && !HasStatusEffect(Buffs.Eukrasia))
            {
                // Lucid Dreaming
                if (IsEnabled(Preset.SGE_ST_DPS_Lucid) &&
                    Role.CanLucidDream(SGE_ST_DPS_Lucid))
                    return Role.LucidDreaming;

                // Addersgall Protection
                if (IsEnabled(Preset.SGE_ST_DPS_AddersgallProtect) &&
                    ActionReady(Druochole) && Addersgall >= SGE_ST_DPS_AddersgallProtect)
                    return Druochole
                        .RetargetIfEnabled(null, DosisActions);

                // Psyche
                if (IsEnabled(Preset.SGE_ST_DPS_Psyche) &&
                    ActionReady(Psyche) && InCombat())
                    return Psyche;

                // Rhizomata
                if (IsEnabled(Preset.SGE_ST_DPS_Rhizo) &&
                    ActionReady(Rhizomata) && Addersgall < SGE_ST_DPS_Rhizo)
                    return Rhizomata;

                //Soteria
                if (IsEnabled(Preset.SGE_ST_DPS_Soteria) &&
                    ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                    return Soteria;
            }

            if (HasBattleTarget() && !HasStatusEffect(Buffs.Eukrasia))
            {
                if (IsEnabled(Preset.SGE_ST_DPS_EDosis) &&
                    LevelChecked(Eukrasia) && InCombat() &&
                    !JustUsedOn(DosisList[OriginalHook(Dosis)].Eukrasian, CurrentTarget) &&
                    CanApplyStatus(CurrentTarget, DosisList[OriginalHook(Dosis)].Debuff))
                {
                    float refreshTimer = SGE_ST_DPS_EDosisRefresh;
                    int hpThreshold = SGE_ST_DPS_EDosisBossOption == 1 || !InBossEncounter() ? SGE_ST_DPS_EDosisHPOption : 0;

                    if (GetTargetHPPercent() > hpThreshold &&
                        ((DosisDebuff is null && DyskrasiaDebuff is null) ||
                         DosisDebuff?.RemainingTime <= refreshTimer ||
                         DyskrasiaDebuff?.RemainingTime <= refreshTimer))
                        return Eukrasia;
                }

                // Phlegma
                if (IsEnabled(Preset.SGE_ST_DPS_Phlegma) &&
                    InCombat() && InActionRange(OriginalHook(Phlegma)) &&
                    ActionReady(Phlegma))
                {
                    //If not enabled or not high enough level, follow slider
                    if ((!SGE_ST_DPS_Phlegma_Burst || !LevelChecked(Psyche)) &&
                        GetRemainingCharges(OriginalHook(Phlegma)) > SGE_ST_DPS_Phlegma)
                        return OriginalHook(Phlegma);

                    //If enabled and high enough level, burst
                    if (SGE_ST_DPS_Phlegma_Burst &&
                        ((GetCooldownRemainingTime(Psyche) > 40 && MaxPhlegma) ||
                         IsOffCooldown(Psyche) ||
                         JustUsed(Psyche, 5f)))
                        return OriginalHook(Phlegma);
                }

                // Movement Options
                if (IsEnabled(Preset.SGE_ST_DPS_Movement) &&
                    InCombat() && IsMoving())
                {
                    foreach (int priority in SGE_ST_DPS_Movement_Priority.Items.OrderBy(x => x))
                    {
                        int index = SGE_ST_DPS_Movement_Priority.IndexOf(priority);
                        if (CheckMovementConfigMeetsRequirements(index, out uint action))
                            return action;
                    }
                }
            }

            return actionID;
        }
    }

    internal class SGE_AoE_DPS_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!DyskrasiaList.Contains(actionID) ||
                HasStatusEffect(Buffs.Eukrasia))
                return actionID;

            //Occult skills
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #region Raidwide Feature

            if (RaidwideKerachole())
                return Kerachole;

            if (RaidwideHolos())
                return Holos;

            if (RaidwideEprognosis())
                return HasStatusEffect(Buffs.Eukrasia)
                    ? OriginalHook(Prognosis)
                    : Eukrasia;

            #endregion

            if (CanWeave())
            {
                // Lucid Dreaming
                if (IsEnabled(Preset.SGE_AoE_DPS_Lucid) &&
                    Role.CanLucidDream(SGE_AoE_DPS_Lucid))
                    return Role.LucidDreaming;

                // Addersgall Protection
                if (IsEnabled(Preset.SGE_AoE_DPS_AddersgallProtect) &&
                    ActionReady(Druochole) && Addersgall >= SGE_AoE_DPS_AddersgallProtect)
                    return Druochole
                        .RetargetIfEnabled(null, OriginalHook(Dyskrasia));

                // Psyche
                if (IsEnabled(Preset.SGE_AoE_DPS_Psyche))
                    if (ActionReady(Psyche) && HasBattleTarget() &&
                        InActionRange(Psyche))
                        return Psyche;

                // Rhizomata
                if (IsEnabled(Preset.SGE_AoE_DPS_Rhizo) &&
                    ActionReady(Rhizomata) && Addersgall <= SGE_AoE_DPS_Rhizo)
                    return Rhizomata;

                //Soteria
                if (IsEnabled(Preset.SGE_AoE_DPS_Soteria) &&
                    ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                    return Soteria;
            }

            //Eukrasia for DoT
            if (IsEnabled(Preset.SGE_AoE_DPS_EDyskrasia) &&
                IsOffCooldown(Eukrasia) &&
                !JustUsedOn(EukrasianDyskrasia, CurrentTarget) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                HasBattleTarget() && InActionRange(Dyskrasia) &&
                CanApplyStatus(CurrentTarget, Debuffs.EukrasianDyskrasia) &&
                GetTargetHPPercent() > 25 &&
                ((DyskrasiaDebuff is null && DosisDebuff is null) ||
                 DyskrasiaDebuff?.RemainingTime <= 4 ||
                 DosisDebuff?.RemainingTime <= 4))
                return Eukrasia;

            //Phlegma
            if (IsEnabled(Preset.SGE_AoE_DPS_Phlegma) &&
                ActionReady(Phlegma) &&
                HasBattleTarget() &&
                InActionRange(OriginalHook(Phlegma)))
                return OriginalHook(Phlegma);

            //Toxikon
            if (IsEnabled(Preset.SGE_AoE_DPS_Toxikon) &&
                ActionReady(Toxikon) &&
                HasBattleTarget() && HasAddersting() &&
                InActionRange(OriginalHook(Toxikon)))
                return OriginalHook(Toxikon);

            //Pneuma
            if (IsEnabled(Preset.SGE_AoE_DPS_Pneuma) &&
                (SGE_AoE_DPS_PneumaBossOption == 0 || TargetIsBoss()) &&
                ActionReady(Pneuma) && HasBattleTarget() &&
                InActionRange(Pneuma))
                return Pneuma;

            return actionID;
        }
    }

    #endregion
    
    #region Simple Healing
    internal class SGE_Simple_ST_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Simple_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            IGameObject? healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            if (actionID is not Diagnosis)
                return actionID;

            if (LevelChecked(Kardia) && 
                !HasStatusEffect(Buffs.Kardia))
                return Kardia.Retarget(Diagnosis, SimpleTarget.AnyLivingTank);

            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);
            if (ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget) >= 40 &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(OptionalTarget, Diagnosis);

            if (Role.CanLucidDream(6500))
                return Role.LucidDreaming;

            if (ActionReady(Rhizomata) && !HasAddersgall() && 
                CanWeave())
                return Rhizomata;

            if (ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia) && 
                CanWeave())
                return Soteria;

            if (ActionReady(OriginalHook(Physis)) && 
                !InBossEncounter())
                return OriginalHook(Physis);

            if (ActionReady(Kerachole) && 
                TraitLevelChecked(Traits.EnhancedKerachole) && 
                HasAddersgall() &&
                !InBossEncounter())
                return Kerachole;

            if (healTarget.IsInParty() && healTarget.GetRole() is CombatRole.Tank || !IsInParty())
            {
                if (ActionReady(Krasis))
                    return Krasis.RetargetIfEnabled(healTarget, Diagnosis);
                if (ActionReady(Taurochole) && HasAddersgall())
                    return Taurochole.RetargetIfEnabled(healTarget, Diagnosis);
                if (ActionReady(Haima) && !HasStatusEffect(Buffs.Panhaima, healTarget))
                    return Haima.RetargetIfEnabled(healTarget, Diagnosis);
            }
            
            if (ActionReady(Druochole) && HasAddersgall())
                return Druochole.RetargetIfEnabled(healTarget, Diagnosis);
            
            if (!InBossEncounter())
            {
                if (ActionReady(Holos))
                    return Holos;

                if (ActionReady(Panhaima) && !HasStatusEffect(Buffs.Haima, healTarget))
                    return Panhaima;
            }

            if (ActionReady(Pepsis) &&
                HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget))
                return Pepsis;

            if (ActionReady(Eukrasia) && !HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget))
                return HasStatusEffect(Buffs.Eukrasia)
                    ? EukrasianDiagnosis
                    : Eukrasia;

            return actionID.RetargetIfEnabled(healTarget, Diagnosis);
        }
    }

    internal class SGE_Simple_AoE_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Simple_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Prognosis)
                return actionID;

            if (Role.CanLucidDream(6500))
                return Role.LucidDreaming;
            
            if (ActionReady(Rhizomata) && !HasAddersgall() && 
                CanWeave())
                return Rhizomata;
           
            if (ActionReady(OriginalHook(Physis)))
                return OriginalHook(Physis);

            if (ActionReady(Kerachole) && 
                TraitLevelChecked(Traits.EnhancedKerachole) && 
                HasAddersgall())
                return Kerachole;
            
            if (ActionReady(Holos))
                return Holos;
            
            if (ActionReady(Ixochole) && HasAddersgall())
                return Ixochole;
            
            if (ActionReady(Philosophia) && !HasStatusEffect(Buffs.Panhaima))
                return Philosophia;
            
            if (ActionReady(Panhaima) && !HasStatusEffect(Buffs.Eudaimonia))
                return Panhaima;

            if (ActionReady(Zoe) || HasStatusEffect(Buffs.Zoe))
                return ActionReady(Pneuma) && !HasStatusEffect(Buffs.Zoe) || !LevelChecked(Pneuma)
                    ? Zoe
                    : Pneuma;
            
            if (ActionReady(Pepsis) &&
                HasStatusEffect(Buffs.EukrasianPrognosis))
                return Pepsis;

            if (ActionReady(Eukrasia) && 
                (GetPartyBuffPercent(Buffs.EukrasianPrognosis) <= 50 || 
                 GetPartyBuffPercent(SCH.Buffs.Galvanize) <= 50))
                return HasStatusEffect(Buffs.Eukrasia)
                    ? EukrasianPrognosis
                    : Eukrasia;

            return actionID;
        }
    }
    
    #endregion

    #region Advanced Healing

    internal class SGE_ST_Heal_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            IGameObject? healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            if (actionID is not Diagnosis)
                return actionID;

            #region Raidwide Feature

            if (RaidwideKerachole())
                return Kerachole;

            if (RaidwideHolos())
                return Holos;

            if (RaidwideEprognosis())
                return HasStatusEffect(Buffs.Eukrasia)
                    ? OriginalHook(Prognosis)
                    : Eukrasia;

            #endregion

            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);
            if (IsEnabled(Preset.SGE_ST_Heal_Esuna) &&
                ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, SGE_ST_Heal_IncludeShields) >= SGE_ST_Heal_Esuna &&
                cleansableTarget)
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Diagnosis);

            if (HasStatusEffect(Buffs.Eukrasia))
                return EukrasianDiagnosis
                    .RetargetIfEnabled(OptionalTarget, Diagnosis);

            if (IsEnabled(Preset.SGE_ST_Heal_Rhizomata) &&
                ActionReady(Rhizomata) && !HasAddersgall())
                return Rhizomata;

            if (IsEnabled(Preset.SGE_ST_Heal_Kardia) &&
                LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia) &&
                !HasStatusEffect(Buffs.Kardion, healTarget))
                return Kardia
                    .Retarget(actionID, Target);

            // Lucid Dreaming
            if (IsEnabled(Preset.SGE_ST_Heal_Lucid) &&
                Role.CanLucidDream(SGE_ST_Heal_LucidOption))
                return Role.LucidDreaming;

            for (int i = 0; i < SGE_ST_Heals_Priority.Count; i++)
            {
                int index = SGE_ST_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                    if (GetTargetHPPercent(healTarget, SGE_ST_Heal_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell
                            .RetargetIfEnabled(OptionalTarget, Diagnosis);
            }

            return actionID
                .RetargetIfEnabled(OptionalTarget, Diagnosis);
        }
    }

    internal class SGE_AoE_Heal_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Prognosis)
                return actionID;

            #region Raidwide Feature

            if (RaidwideKerachole())
                return Kerachole;

            if (RaidwideHolos())
                return Holos;

            if (RaidwideEprognosis())
                return HasStatusEffect(Buffs.Eukrasia)
                    ? OriginalHook(Prognosis)
                    : Eukrasia;

            #endregion

            //Zoe -> Pneuma like Eukrasia 
            if (SGE_AoE_Heal_ZoePneuma &&
                HasStatusEffect(Buffs.Zoe))
                return Pneuma;

            if (IsEnabled(Preset.SGE_AoE_Heal_EPrognosis) &&
                HasStatusEffect(Buffs.Eukrasia))
                return OriginalHook(Prognosis);

            if (IsEnabled(Preset.SGE_AoE_Heal_Rhizomata) &&
                ActionReady(Rhizomata) && !HasAddersgall())
                return Rhizomata;

            if (IsEnabled(Preset.SGE_AoE_Heal_Lucid) &&
                Role.CanLucidDream(SGE_AoE_Heal_LucidOption))
                return Role.LucidDreaming;

            float averagePartyHP = GetPartyAvgHPPercent();
            for (int i = 0; i < SGE_AoE_Heals_Priority.Count; i++)
            {
                int index = SGE_AoE_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);

                if (enabled && averagePartyHP <= config && ActionReady(spell))
                    return spell;
            }

            return actionID;
        }
    }

    #endregion

    #region Standalones

    internal class SGE_OverProtect : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_OverProtect;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Kerachole or Panhaima or Philosophia))
                return actionID;

            switch (actionID)
            {
                case Kerachole when IsEnabled(Preset.SGE_OverProtect_Kerachole) &&
                                    ActionReady(Kerachole) &&
                                    (HasStatusEffect(Buffs.Kerachole, anyOwner: true) ||
                                     IsEnabled(Preset.SGE_OverProtect_SacredSoil) && HasStatusEffect(SCH.Buffs.SacredSoil, anyOwner: true)):
                case Panhaima when IsEnabled(Preset.SGE_OverProtect_Panhaima) &&
                                   ActionReady(Panhaima) && HasStatusEffect(Buffs.Panhaima, anyOwner: true):
                    return SCH.SacredSoil;
                case Philosophia when IsEnabled(Preset.SGE_OverProtect_Philosophia) &&
                                      ActionReady(Philosophia) && HasStatusEffect(Buffs.Eudaimonia, anyOwner: true):
                    return SCH.Consolation;
                default:
                    return actionID;
            }
        }
    }

    internal class SGE_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Raise;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            return IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(Preset.SGE_Raise_Retarget)
                    ? Egeiro.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Egeiro
                : actionID;
        }
    }

    internal class SGE_ZoePneuma : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_ZoePneuma;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Pneuma)
                return actionID;

            return ActionReady(Pneuma) && IsOffCooldown(Zoe)
                ? Zoe
                : actionID;
        }
    }

    internal class SGE_Rhizo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Rhizo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Kerachole or Taurochole or Druochole or Ixochole))
                return actionID;

            return AddersgallList.Contains(actionID) &&
                   ActionReady(Rhizomata) && !HasAddersgall() && IsOffCooldown(actionID)
                ? Rhizomata
                : actionID;
        }
    }

    internal class SGE_Eukrasia : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Eukrasia;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Eukrasia || !HasStatusEffect(Buffs.Eukrasia))
                return actionID;

            if (SGE_Eukrasia_Mode == 0)
                return OriginalHook(Dosis);
            if (SGE_Eukrasia_Mode == 2)
                return OriginalHook(Prognosis);
            if (SGE_Eukrasia_Mode == 3)
                return OriginalHook(Dyskrasia);
            if (SGE_Eukrasia_Mode == 1)
                return IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis)
                    ? EukrasianDiagnosis.Retarget(Eukrasia, HealStack)
                    : EukrasianDiagnosis;

            return actionID;
        }
    }

    internal class SGE_TauroDruo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_TauroDruo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Taurochole)
                return actionID;

            if (!LevelChecked(Taurochole) || IsOnCooldown(Taurochole))
                return IsEnabled(Preset.SGE_Retarget_Druochole)
                    ? Druochole.Retarget(Taurochole, HealStack, true)
                    : Druochole;
            return IsEnabled(Preset.SGE_Retarget_Taurochole)
                ? Taurochole.Retarget(HealStack, true)
                : Taurochole;
        }
    }

    internal class SGE_Kardia : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Kardia;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Soteria)
                return actionID;

            if (!HasStatusEffect(Buffs.Kardia) || IsOnCooldown(Soteria))
                return IsEnabled(Preset.SGE_Retarget_Kardia)
                    ? Kardia.Retarget(actionID, HealStack, true)
                    : Kardia;

            return actionID;
        }
    }

    internal class SGE_Mit_ST : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Mit_ST;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Krasis)
                return actionID;

            if (ActionReady(Krasis))
                return IsEnabled(Preset.SGE_Retarget_Krasis)
                    ? Krasis.Retarget(HealStack, true)
                    : actionID;

            if (!HasStatusEffect(Buffs.EukrasianDiagnosis, HealStack))
            {
                if (!HasStatusEffect(Buffs.Eukrasia))
                    return Eukrasia;

                return IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis)
                    ? EukrasianDiagnosis.Retarget(Krasis, HealStack, true)
                    : EukrasianDiagnosis;
            }

            if (SGE_Mit_ST_Options[0] && !ActionReady(Krasis) &&
                ActionReady(Haima))
                return IsEnabled(Preset.SGE_Retarget_Haima)
                    ? Haima.Retarget(Krasis, HealStack, true)
                    : Haima;

            if (SGE_Mit_ST_Options[1] && !ActionReady(Krasis) &&
                ActionReady(Taurochole) &&
                GetTargetHPPercent(HealStack) <= SGE_Mit_ST_TaurocholeThreshold)
                return IsEnabled(Preset.SGE_Retarget_Taurochole)
                    ? Taurochole.Retarget(Krasis, HealStack, true)
                    : Taurochole;

            return actionID;
        }
    }

    internal class SGE_Mit_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Mit_AoE;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Holos)
                return actionID;

            if (SGE_Mit_AoE_Options[0] &&
                ActionReady(Kerachole) &&
                !HasStatusEffect(Buffs.Kerachole, anyOwner: true) &&
                !HasStatusEffect(SCH.Buffs.SacredSoil, anyOwner: true))
                return Kerachole;

            if (SGE_Mit_AoE_Options[1] &&
                ActionReady(Philosophia))
                return Philosophia;

            if (GetPartyBuffPercent(Buffs.EukrasianPrognosis) <= SGE_Mit_AoE_PrognosisOption)
                return HasStatusEffect(Buffs.Eukrasia)
                    ? OriginalHook(Prognosis)
                    : Eukrasia;

            if (ActionReady(Holos) &&
                !HasStatusEffect(Buffs.Holosakos, anyOwner: true))
                return Holos;

            if (SGE_Mit_AoE_Options[2] &&
                ActionReady(Panhaima) &&
                !HasStatusEffect(Buffs.Panhaima, anyOwner: true))
                return Panhaima;

            return actionID;
        }
    }

    internal class SGE_Retarget : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_Retarget;

        protected override uint Invoke(uint actionID)
        {
            if (!EZ.Throttle("SGERetargetingFeature", TS.FromSeconds(.1)))
                return actionID;

            if (IsEnabled(Preset.SGE_Retarget_Diagnosis))
                OriginalHook(Diagnosis).Retarget(HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis))
                EukrasianDiagnosis.Retarget(Diagnosis, HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_Haima))
                Haima.Retarget(HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_Druochole))
                Druochole.Retarget(HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_Taurochole))
                Taurochole.Retarget(HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_Krasis))
                Krasis.Retarget(HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_Kardia))
                Kardia.Retarget(HealStack, true);

            if (IsEnabled(Preset.SGE_Retarget_Icarus))
                Icarus.Retarget(SimpleTarget.Stack.MouseOver ?? SimpleTarget.HardTarget, true);

            return actionID;
        }
    }

    #endregion
}