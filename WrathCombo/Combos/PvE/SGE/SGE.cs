using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Data.ActionWatching;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class SGE : Healer
{
    internal class SGE_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            bool actionFound = actionID is Dosis2 || !Config.SGE_ST_DPS_Adv && DosisList.ContainsKey(actionID);
            uint[] replacedActions = Config.SGE_ST_DPS_Adv
                ? [Dosis2]
                : DosisList.Keys.ToArray();

            if (!actionFound)
                return actionID;

            // Kardia Reminder
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Kardia) &&
                LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia))
                return Kardia;

            // Opener for SGE
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;
            
            // Variant
            if (Variant.CanRampart(CustomComboPreset.SGE_DPS_Variant_Rampart))
                return Variant.Rampart;

            //Occult skills
            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            if (CanSpellWeave() && !HasDoubleWeaved() && !HasStatusEffect(Buffs.Eukrasia))
            {
                if (Variant.CanSpiritDart(CustomComboPreset.SGE_DPS_Variant_SpiritDart))
                    return Variant.SpiritDart;

                // Lucid Dreaming
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Lucid) &&
                    Role.CanLucidDream(Config.SGE_ST_DPS_Lucid))
                    return Role.LucidDreaming;

                // Addersgall Protection
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_AddersgallProtect) &&
                    ActionReady(Druochole) && Addersgall >= Config.SGE_ST_DPS_AddersgallProtect)
                    return Druochole
                        .RetargetIfEnabled(null, replacedActions);

                // Psyche
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Psyche) &&
                    ActionReady(Psyche) && InCombat())
                    return Psyche;
                
                // Rhizomata
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Rhizo) &&
                    ActionReady(Rhizomata) && Addersgall < Config.SGE_ST_DPS_Rhizo)
                    return Rhizomata;

                //Soteria
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Soteria) &&
                    ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                    return Soteria;
            }

            if (HasBattleTarget() && !HasStatusEffect(Buffs.Eukrasia))
            {
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_EDosis) &&
                    LevelChecked(Eukrasia) && InCombat() &&
                    !JustUsedOn(OriginalHook(EukrasianDosis), CurrentTarget))
                {
                    float refreshTimer = Config.SGE_ST_DPS_EDosisThreshold;
                    int hpThreshold = Config.SGE_ST_DPS_EDosisSubOption == 1 || !InBossEncounter() ? Config.SGE_ST_DPS_EDosisOption : 0;

                    if (CanApplyStatus(CurrentTarget, DosisList[OriginalHook(Dosis)]) &&
                        GetTargetHPPercent() > hpThreshold &&
                        ((DosisDebuff is null && DyskrasiaDebuff is null) ||
                         DosisDebuff?.RemainingTime <= refreshTimer ||
                         DyskrasiaDebuff?.RemainingTime <= refreshTimer))
                        return Eukrasia;
                }

                // Phlegma
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma) &&
                    InCombat() && InActionRange(OriginalHook(Phlegma)) &&
                    ActionReady(Phlegma))
                {
                    //If not enabled or not high enough level, follow slider
                    if ((IsNotEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma_Burst) || !LevelChecked(Psyche)) &&
                        GetRemainingCharges(OriginalHook(Phlegma)) > Config.SGE_ST_DPS_Phlegma)
                        return OriginalHook(Phlegma);

                    //If enabled and high enough level, burst
                    if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma_Burst) &&
                        ((GetCooldownRemainingTime(Psyche) > 40 && MaxPhlegma) ||
                         IsOffCooldown(Psyche) ||
                         JustUsed(Psyche, 5f)))
                        return OriginalHook(Phlegma);
                }

                // Movement Options
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Movement) &&
                    InCombat() && IsMoving())
                {
                    // Toxikon
                    if (Config.SGE_ST_DPS_Movement[0] &&
                        ActionReady(Toxikon) && HasAddersting())
                        return OriginalHook(Toxikon);

                    // Dyskrasia
                    if (Config.SGE_ST_DPS_Movement[1] &&
                        ActionReady(Dyskrasia) && InActionRange(Dyskrasia))
                        return OriginalHook(Dyskrasia);

                    // Eukrasia
                    if (Config.SGE_ST_DPS_Movement[2] &&
                        ActionReady(Eukrasia))
                        return Eukrasia;
                }
            }

            return actionID;
        }
    }

    internal class SGE_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_AoE_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!DyskrasiaList.Contains(actionID) ||
                HasStatusEffect(Buffs.Eukrasia))
                return actionID;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.SGE_DPS_Variant_Rampart))
                return Variant.Rampart;

            //Occult skills
            if (OccultCrescent.ShouldUsePhantomActions())
                return OccultCrescent.BestPhantomAction();

            if (CanSpellWeave() && !HasDoubleWeaved())
            {
                // Variant Spirit Dart
                if (Variant.CanSpiritDart(CustomComboPreset.SGE_DPS_Variant_SpiritDart))
                    return Variant.SpiritDart;

                // Lucid Dreaming
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Lucid) &&
                    Role.CanLucidDream(Config.SGE_AoE_DPS_Lucid))
                    return Role.LucidDreaming;

                // Addersgall Protection
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_AddersgallProtect) &&
                    ActionReady(Druochole) && Addersgall >= Config.SGE_AoE_DPS_AddersgallProtect)
                    return Druochole;

                // Psyche
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Psyche))
                    if (ActionReady(Psyche) && HasBattleTarget() &&
                        InActionRange(Psyche))
                        return Psyche;

                // Rhizomata
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Rhizo) &&
                    ActionReady(Rhizomata) && Addersgall <= Config.SGE_AoE_DPS_Rhizo)
                    return Rhizomata;

                //Soteria
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Soteria) &&
                    ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                    return Soteria;
            }

            //Eukrasia for DoT
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_EDyskrasia) &&
                IsOffCooldown(Eukrasia) &&
                !WasLastSpell(EukrasianDyskrasia) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                HasBattleTarget() && InActionRange(Dyskrasia) &&
                CanApplyStatus(CurrentTarget, Debuffs.EukrasianDyskrasia) &&
                GetTargetHPPercent() > 25 &&
                ((DyskrasiaDebuff is null && DosisDebuff is null) ||
                 DyskrasiaDebuff?.RemainingTime <= 3 ||
                 DosisDebuff?.RemainingTime <= 3))
                return Eukrasia;

            //Phlegma
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Phlegma) &&
                ActionReady(Phlegma) &&
                HasBattleTarget() &&
                InActionRange(OriginalHook(Phlegma)))
                return OriginalHook(Phlegma);

            //Toxikon
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Toxikon) &&
                ActionReady(Toxikon) &&
                HasBattleTarget() && HasAddersting() &&
                InActionRange(OriginalHook(Toxikon)))
                return OriginalHook(Toxikon);

            //Pneuma
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS__Pneuma) &&
                ActionReady(Pneuma) && HasBattleTarget() &&
                InActionRange(Pneuma))
                return Pneuma;

            return actionID;
        }
    }

    internal class SGE_ST_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Diagnosis)
                return actionID;

            if (HasStatusEffect(Buffs.Eukrasia))
                return EukrasianDiagnosis
                    .RetargetIfEnabled(OptionalTarget, Diagnosis);

            IGameObject? healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Esuna) &&
                ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) >= Config.SGE_ST_Heal_Esuna &&
                HasCleansableDebuff(healTarget))
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Diagnosis);

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) &&
                ActionReady(Rhizomata) && !HasAddersgall())
                return Rhizomata;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) &&
                LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia) &&
                !HasStatusEffect(Buffs.Kardion, healTarget))
                return Kardia
                    .RetargetIfEnabled(OptionalTarget, Diagnosis);

            for(int i = 0; i < Config.SGE_ST_Heals_Priority.Count; i++)
            {
                int index = Config.SGE_ST_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                    if (GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell
                            .RetargetIfEnabled(OptionalTarget, Diagnosis);
            }

            return actionID
                .RetargetIfEnabled(OptionalTarget, Diagnosis);
        }
    }

    internal class SGE_AoE_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Prognosis)
                return actionID;

            //Zoe -> Pneuma like Eukrasia 
            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_ZoePneuma) &&
                HasStatusEffect(Buffs.Zoe))
                return Pneuma;

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) &&
                HasStatusEffect(Buffs.Eukrasia))
                return OriginalHook(Prognosis);

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) &&
                ActionReady(Rhizomata) && !HasAddersgall())
                return Rhizomata;

            float averagePartyHP = GetPartyAvgHPPercent();
            for(int i = 0; i < Config.SGE_AoE_Heals_Priority.Count; i++)
            {
                int index = Config.SGE_AoE_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);

                if (enabled && averagePartyHP <= config && ActionReady(spell))
                    return spell;
            }

            return actionID;
        }
    }

    internal class SGE_OverProtect : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_OverProtect;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Kerachole or Panhaima or Philosophia))
                return actionID;

            switch (actionID)
            {
                case Kerachole when IsEnabled(CustomComboPreset.SGE_OverProtect_Kerachole) &&
                                    ActionReady(Kerachole) &&
                                    (HasStatusEffect(Buffs.Kerachole, anyOwner: true) ||
                                     IsEnabled(CustomComboPreset.SGE_OverProtect_SacredSoil) && HasStatusEffect(SCH.Buffs.SacredSoil, anyOwner: true)) &&
                                    (HasStatusEffect(Buffs.Kerachole, anyOwner: true) ||
                                     IsEnabled(CustomComboPreset.SGE_OverProtect_SacredSoil) && HasStatusEffect(SCH.Buffs.SacredSoil, anyOwner: true)):

                case Panhaima when IsEnabled(CustomComboPreset.SGE_OverProtect_Panhaima) &&
                                   ActionReady(Panhaima) && HasStatusEffect(Buffs.Panhaima, anyOwner: true):
                    return SCH.SacredSoil;

                case Philosophia when IsEnabled(CustomComboPreset.SGE_OverProtect_Philosophia) &&
                                      ActionReady(Philosophia) && HasStatusEffect(Buffs.Eudaimonia, anyOwner: true):
                    return SCH.Consolation;

                default:
                    return actionID;
            }
        }
    }

    internal class SGE_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(CustomComboPreset.SGE_Raise_Retarget)
                    ? Egeiro
                        .Retarget(Role.Swiftcast, SimpleTarget.Stack.AllyToRaise)
                        .AndRunMacro()
                    : Egeiro.AndRunMacro()
                : actionID;
    }

    internal class SGE_Eukrasia : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Eukrasia;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Eukrasia || !HasStatusEffect(Buffs.Eukrasia))
                return actionID;

            return (int)Config.SGE_Eukrasia_Mode switch
            {
                0 => OriginalHook(Dosis),
                1 => OriginalHook(Diagnosis),
                2 => OriginalHook(Prognosis),
                3 => OriginalHook(Dyskrasia),
                var _ => actionID
            };
        }
    }

    internal class SGE_Kardia : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Kardia;

        protected override uint Invoke(uint actionID) =>
            actionID is Soteria &&
            (!HasStatusEffect(Buffs.Kardia) || IsOnCooldown(Soteria))
                ? Kardia
                : actionID;
    }

    internal class SGE_Rhizo : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Rhizo;

        protected override uint Invoke(uint actionID) =>
            AddersgallList.Contains(actionID) &&
            ActionReady(Rhizomata) && !HasAddersgall() && IsOffCooldown(actionID)
                ? Rhizomata
                : actionID;
    }

    internal class SGE_TauroDruo : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_TauroDruo;

        protected override uint Invoke(uint actionID) =>
            (actionID is Taurochole) &&
            (!LevelChecked(Taurochole) || IsOnCooldown(Taurochole))
                ? Druochole
                : actionID;
    }

    internal class SGE_ZoePneuma : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_ZoePneuma;

        protected override uint Invoke(uint actionID) =>
            actionID is Pneuma && ActionReady(Pneuma) && IsOffCooldown(Zoe)
                ? Zoe
                : actionID;
    }
}
