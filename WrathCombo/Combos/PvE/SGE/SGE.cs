using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using static WrathCombo.Data.ActionWatching;
namespace WrathCombo.Combos.PvE;

internal partial class SGE : Healer
{
    /*
     * SGE_ST_DPS (Single Target DPS Combo)
     * Currently Replaces Dosis with Eukrasia when the debuff on the target is < 3 seconds or not existing
     * Kardia reminder, Lucid Dreaming, & Toxikon optional
     */
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
                        (DosisDebuff is null && DyskrasiaDebuff is null ||
                         DosisDebuff?.RemainingTime <= refreshTimer ||
                         DyskrasiaDebuff?.RemainingTime <= refreshTimer) &&
                        GetTargetHPPercent() > hpThreshold)
                        return Eukrasia;
                }

                // Phlegma
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma) &&
                    InCombat() && InActionRange(OriginalHook(Phlegma)) &&
                    ActionReady(OriginalHook(Phlegma)))
                {
                    //If not enabled or not high enough level, follow slider
                    if ((IsNotEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma_Burst) || !LevelChecked(Psyche)) &&
                        GetRemainingCharges(OriginalHook(Phlegma)) > Config.SGE_ST_DPS_Phlegma)
                        return OriginalHook(OriginalHook(Phlegma));

                    //If enabled and high enough level, burst
                    if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma_Burst) &&
                        (GetCooldownRemainingTime(Psyche) > 40 && MaxPhlegma ||
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
                        LevelChecked(Toxikon) && HasAddersting())
                        return OriginalHook(Toxikon);

                    // Dyskrasia
                    if (Config.SGE_ST_DPS_Movement[1] &&
                        LevelChecked(Dyskrasia) && InActionRange(Dyskrasia))
                        return OriginalHook(Dyskrasia);

                    // Eukrasia
                    if (Config.SGE_ST_DPS_Movement[2] &&
                        LevelChecked(Eukrasia))
                        return Eukrasia;
                }
            }

            return actionID;
        }
    }

    /*
     * SGE_AoE_DPS (Dyskrasia AoE Feature)
     * Replaces Dyskrasia with Phegma/Toxikon/Misc
     */
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

            // Variant Spirit Dart
            if (Variant.CanSpiritDart(CustomComboPreset.SGE_DPS_Variant_SpiritDart))
                return Variant.SpiritDart;

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Lucid) &&
                Role.CanLucidDream(Config.SGE_AoE_DPS_Lucid))
                return Role.LucidDreaming;

            if (CanSpellWeave() && !HasDoubleWeaved())
            {
                // Psyche
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Psyche))
                    if (ActionReady(Psyche) &&
                        HasBattleTarget() &&
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

                // Addersgall Protection
                if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_AddersgallProtect) &&
                    ActionReady(Druochole) && Addersgall >= Config.SGE_AoE_DPS_AddersgallProtect)
                    return Druochole;
            }

            //Eukrasia for DoT
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_EDyskrasia) &&
                IsOffCooldown(Eukrasia) &&
                !WasLastSpell(EukrasianDyskrasia) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                HasBattleTarget() && InActionRange(Dyskrasia) &&
                CanApplyStatus(CurrentTarget, Debuffs.EukrasianDyskrasia) &&
                GetTargetHPPercent() > 25 &&
                (DyskrasiaDebuff is null && DosisDebuff is null ||
                 DyskrasiaDebuff?.RemainingTime <= 3 ||
                 DosisDebuff?.RemainingTime <= 3))
                return Eukrasia;

            //Phlegma
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Phlegma) &&
                ActionReady(OriginalHook(Phlegma)) &&
                HasBattleTarget() &&
                InActionRange(OriginalHook(Phlegma)))
                return OriginalHook(Phlegma);

            //Toxikon
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Toxikon) &&
                ActionReady(OriginalHook(Toxikon)) &&
                HasBattleTarget() && HasAddersting() &&
                InActionRange(OriginalHook(Toxikon)))
                return OriginalHook(Toxikon);

            //Pneuma
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS__Pneuma) &&
                ActionReady(Pneuma) &&
                HasBattleTarget() &&
                InActionRange(Pneuma))
                return Pneuma;

            return actionID;
        }
    }

    /*
     * SGE_ST_Heal (Diagnosis Single Target Heal)
     * Replaces Diagnosis with various Single Target healing options,
     * Pseudo priority set by various custom user percentages
     */
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

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Esuna) && ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) >= Config.SGE_ST_Heal_Esuna &&
                HasCleansableDebuff(healTarget))
                return Role.Esuna
                    .RetargetIfEnabled(OptionalTarget, Diagnosis);

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                !HasAddersgall())
                return Rhizomata;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && LevelChecked(Kardia) &&
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

    /*
     * SGE_AoE_Heal (Prognosis AoE Heal)
     * Replaces Prognosis with various AoE healing options,
     * Pseudo priority set by various custom user percentages
     */
    internal class SGE_AoE_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_AoE_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Prognosis)
                return actionID;

            //Zoe -> Pneuma like Eukrasia 
            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_ZoePneuma) && HasStatusEffect(Buffs.Zoe))
                return Pneuma;

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && HasStatusEffect(Buffs.Eukrasia))
                return OriginalHook(Prognosis);

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                !HasAddersgall())
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

    /*
     * SGE_Raise (Swiftcast Raise)
     * Swiftcast becomes Egeiro when on cooldown
     */
    internal class SGE_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(CustomComboPreset.SGE_Raise_Retarget)
                    ? Egeiro.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Egeiro
                : actionID;
    }

    /*
     * SGE_Eukrasia (Eukrasia combo)
     * Normally after Eukrasia is used and updates the abilities, it becomes disabled
     * This will "combo" the action to user selected action
     */
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

    /*
     * SGE_Kardia
     * Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
     */
    internal class SGE_Kardia : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Kardia;

        protected override uint Invoke(uint actionID) =>
            actionID is Soteria &&
            (!HasStatusEffect(Buffs.Kardia) || IsOnCooldown(Soteria))
                ? Kardia
                : actionID;
    }

    /*
     * SGE_Rhizo
     * Replaces all Addersgal using Abilities (Taurochole/Druochole/Ixochole/Kerachole) with Rhizomata if out of Addersgall stacks
     * (Scholar speak: Replaces all Aetherflow abilities with Aetherflow when out)
     */
    internal class SGE_Rhizo : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_Rhizo;

        protected override uint Invoke(uint actionID) =>
            AddersgallList.Contains(actionID) &&
            ActionReady(Rhizomata) && !HasAddersgall() && IsOffCooldown(actionID)
                ? Rhizomata
                : actionID;
    }

    /*
     * Taurochole will be replaced by Druochole if on cooldown or below level
     */
    internal class SGE_TauroDruo : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_TauroDruo;

        protected override uint Invoke(uint actionID) =>
            (actionID is Taurochole) &&
            (!LevelChecked(Taurochole) || IsOnCooldown(Taurochole))
                ? Druochole
                : actionID;
    }

    /*
     * SGE_ZoePneuma (Zoe to Pneuma Combo)
     * Places Zoe on top of Pneuma when both are available.
     */
    internal class SGE_ZoePneuma : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.SGE_ZoePneuma;

        protected override uint Invoke(uint actionID) =>
            actionID is Pneuma && ActionReady(Pneuma) && IsOffCooldown(Zoe)
                ? Zoe
                : actionID;
    }
}
