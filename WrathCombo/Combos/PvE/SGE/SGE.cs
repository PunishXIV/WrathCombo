using Dalamud.Game.ClientState.Objects.Types;
using System;
using WrathCombo.CustomComboNS;
namespace WrathCombo.Combos.PvE;

internal partial class SGE : Healer
{
    /*
     * SGE_Kardia
     * Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
     */
    internal class SGE_Kardia : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Kardia;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Rhizo;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_TauroDruo;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ZoePneuma;

        protected override uint Invoke(uint actionID) =>
            actionID is Pneuma && ActionReady(Pneuma) && IsOffCooldown(Zoe)
                ? Zoe
                : actionID;
    }

    /*
     * SGE_AoE_DPS (Dyskrasia AoE Feature)
     * Replaces Dyskrasia with Phegma/Toxikon/Misc
     */
    internal class SGE_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!DyskrasiaList.Contains(actionID))
                return actionID;
            if (HasStatusEffect(Buffs.Eukrasia))
                return actionID;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.SGE_DPS_Variant_Rampart)) return Variant.Rampart;

            // Variant Spirit Dart
            if (Variant.CanSpiritDart(CustomComboPreset.SGE_DPS_Variant_SpiritDart)) return Variant.SpiritDart;

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Lucid) && Role.CanLucidDream(Config.SGE_AoE_DPS_Lucid))
                return Role.LucidDreaming;

            // Rhizomata
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Rhizo) && CanSpellWeave() &&
                ActionReady(Rhizomata) && Gauge.Addersgall <= Config.SGE_AoE_DPS_Rhizo)
                return Rhizomata;

            //Soteria
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Soteria) && CanSpellWeave() &&
                ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                return Soteria;

            // Addersgall Protection
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_AddersgallProtect) &&
                CanSpellWeave() &&
                ActionReady(Druochole) && Gauge.Addersgall >= Config.SGE_AoE_DPS_AddersgallProtect)
                return Druochole;

            //Eukrasia for DoT
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_EDyskrasia) &&
                IsOffCooldown(Eukrasia) &&
                !WasLastSpell(EukrasianDyskrasia) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                HasBattleTarget() && InActionRange(Dyskrasia) && //Same range
                DosisList.TryGetValue(OriginalHook(actionID), out (uint Eukrasian, ushort DebuffID) currentDosis))
            {
                float dotDebuff = Math.Max(GetStatusEffectRemainingTime(currentDosis.DebuffID, CurrentTarget),
                    GetStatusEffectRemainingTime(Debuffs.EukrasianDyskrasia, CurrentTarget));

                const float refreshtimer = 3; //Will revisit if it's really needed....SGE_ST_DPS_EDosis_Adv ? Config.SGE_ST_DPS_EDosisThreshold : 3;

                if (dotDebuff <= refreshtimer && GetTargetHPPercent() > 10) //Will Revisit if Config is needed Config.SGE_ST_DPS_EDosisHPPer)
                    return Eukrasia;
            }

            // Psyche
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Psyche))
                if (ActionReady(Psyche) &&
                    HasBattleTarget() &&
                    InActionRange(Psyche) &&
                    CanSpellWeave())
                    return Psyche;

            //Phlegma
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Phlegma))
            {
                uint phlegmaID = OriginalHook(Phlegma);

                if (ActionReady(phlegmaID) &&
                    HasBattleTarget() &&
                    InActionRange(phlegmaID))
                    return phlegmaID;
            }

            //Toxikon
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Toxikon))
            {
                uint toxikonID = OriginalHook(Toxikon);

                if (ActionReady(toxikonID) &&
                    HasBattleTarget() &&
                    InActionRange(toxikonID) &&
                    HasAddersting())
                    return toxikonID;
            }

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
     * SGE_ST_DPS (Single Target DPS Combo)
     * Currently Replaces Dosis with Eukrasia when the debuff on the target is < 3 seconds or not existing
     * Kardia reminder, Lucid Dreaming, & Toxikon optional
     */
    internal class SGE_ST_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_DPS;

        protected override uint Invoke(uint actionID)
        {
            bool actionFound = actionID is Dosis2 || !Config.SGE_ST_DPS_Adv && DosisList.ContainsKey(actionID);

            if (!actionFound)
                return actionID;

            // Kardia Reminder
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Kardia) && LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia))
                return Kardia;

            // Opener for SGE
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Opener) && 
                Opener().FullOpener(ref actionID))
                return actionID;

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Lucid) && Role.CanLucidDream(Config.SGE_ST_DPS_Lucid))
                return Role.LucidDreaming;

            // Variant
            if (Variant.CanRampart(CustomComboPreset.SGE_DPS_Variant_Rampart)) return Variant.Rampart;

            // Rhizomata
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Rhizo) && CanSpellWeave() &&
                ActionReady(Rhizomata) && Gauge.Addersgall <= Config.SGE_ST_DPS_Rhizo)
                return Rhizomata;

            //Soteria
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Soteria) && CanSpellWeave() &&
                ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
                return Soteria;

            // Addersgall Protection
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_AddersgallProtect) && CanSpellWeave() &&
                ActionReady(Druochole) && Gauge.Addersgall >= Config.SGE_ST_DPS_AddersgallProtect)
                return Druochole;

            // Buff check Above. Without it, Toxikon and any future option will interfere in the Eukrasia->Eukrasia Dosis combo
            if (HasBattleTarget() && !HasStatusEffect(Buffs.Eukrasia))
            {
                // Eukrasian Dosis.
                // If we're too low level to use Eukrasia, we can stop here.
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_EDosis) && LevelChecked(Eukrasia) && InCombat())
                {
                    // Grab current Dosis via OriginalHook, grab it's fellow debuff ID from Dictionary, then check for the debuff
                    // Using TryGetValue due to edge case where the actionID would be read as Eukrasian Dosis instead of Dosis
                    // EDosis will show for half a second if the buff is removed manually or some other act of God
                    if (DosisList.TryGetValue(OriginalHook(actionID), out (uint Eukrasian, ushort DebuffID) currentDosis))
                    {
                        if (Variant.CanSpiritDart(CustomComboPreset.SGE_DPS_Variant_SpiritDart)) return Variant.SpiritDart;

                        if (!JustUsedOn(currentDosis.Eukrasian,CurrentTarget)) { 
                            // Dosis DoT Debuff
                            float dotDebuff = GetStatusEffectRemainingTime(currentDosis.DebuffID, CurrentTarget);

                            // Check for the AoE DoT.  These DoTs overlap, so get time remaining of any of them
                            if (TraitLevelChecked(Traits.OffensiveMagicMasteryII))
                                dotDebuff = Math.Max(dotDebuff, GetStatusEffectRemainingTime(Debuffs.EukrasianDyskrasia, CurrentTarget));

                            float refreshTimer = Config.SGE_ST_DPS_EDosisThreshold;
                            int hpThreshold = Config.SGE_ST_DPS_EDosisSubOption == 1 || !InBossEncounter() ? Config.SGE_ST_DPS_EDosisOption : 0;

                            if (dotDebuff <= refreshTimer &&
                                GetTargetHPPercent() > hpThreshold)
                                return Eukrasia;
                        }
                    }
                }

                // Phlegma
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma) && InCombat())
                {
                    uint phlegma = OriginalHook(Phlegma);

                    if (InActionRange(phlegma)
                        && LevelChecked(phlegma)
                        && GetRemainingCharges(phlegma) > Config.SGE_ST_DPS_Phlegma)
                        return phlegma;
                }

                // Psyche
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Psyche) &&
                    ActionReady(Psyche) &&
                    InCombat() && CanSpellWeave())
                    return Psyche;

                // Movement Options
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Movement) && InCombat() && IsMoving())
                {
                    // Toxikon
                    if (Config.SGE_ST_DPS_Movement[0] && LevelChecked(Toxikon) && HasAddersting())
                        return OriginalHook(Toxikon);

                    // Dyskrasia
                    if (Config.SGE_ST_DPS_Movement[1] && LevelChecked(Dyskrasia) && InActionRange(Dyskrasia))
                        return OriginalHook(Dyskrasia);

                    // Eukrasia
                    if (Config.SGE_ST_DPS_Movement[2] && LevelChecked(Eukrasia))
                        return Eukrasia;
                }
            }

            return actionID;
        }
    }

    /*
     * SGE_Raise (Swiftcast Raise)
     * Swiftcast becomes Egeiro when on cooldown
     */
    internal class SGE_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Raise;

        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? Egeiro
                : actionID;
    }

    /*
     * SGE_Eukrasia (Eukrasia combo)
     * Normally after Eukrasia is used and updates the abilities, it becomes disabled
     * This will "combo" the action to user selected action
     */
    internal class SGE_Eukrasia : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Eukrasia;

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
                _ => actionID,
            };
        }
    }

    /*
     * SGE_ST_Heal (Diagnosis Single Target Heal)
     * Replaces Diagnosis with various Single Target healing options,
     * Pseudo priority set by various custom user percentages
     */
    internal class SGE_ST_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Diagnosis)
                return actionID;

            if (HasStatusEffect(Buffs.Eukrasia))
                return EukrasianDiagnosis;

            IGameObject? healTarget = OptionalTarget ??
                                      GetHealTarget(Config.SGE_ST_Heal_Adv && Config.SGE_ST_Heal_UIMouseOver);

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Esuna) && ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) >= Config.SGE_ST_Heal_Esuna &&
                HasCleansableDebuff(healTarget))
                return Role.Esuna;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                !HasAddersgall())
                return Rhizomata;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia) &&
                !HasStatusEffect(Buffs.Kardion, healTarget))
                return Kardia;

            for(int i = 0; i < Config.SGE_ST_Heals_Priority.Count; i++)
            {
                int index = Config.SGE_ST_Heals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                    if (GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell;
            }

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_EDiagnosis) && LevelChecked(Eukrasia) &&
                GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) <= Config.SGE_ST_Heal_EDiagnosisHP &&
                (Config.SGE_ST_Heal_EDiagnosisOpts[0] || // Ignore Any Shield check
                !HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget, true) && //Shield Check
                (!Config.SGE_ST_Heal_EDiagnosisOpts[1] || !HasStatusEffect(SCH.Buffs.Galvanize, healTarget, true)))) //Galvanize Check
                return Eukrasia;

            return actionID;
        }
    }

    /*
     * SGE_AoE_Heal (Prognosis AoE Heal)
     * Replaces Prognosis with various AoE healing options,
     * Pseudo priority set by various custom user percentages
     */
    internal class SGE_AoE_Heal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_AoE_Heal;

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_OverProtect;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Kerachole or Panhaima or Philosophia))
                return actionID;

            if (actionID is Kerachole && IsEnabled(CustomComboPreset.SGE_OverProtect_Kerachole) &&
                ActionReady(Kerachole))
                if (HasStatusEffect(Buffs.Kerachole, anyOwner: true) ||
                    IsEnabled(CustomComboPreset.SGE_OverProtect_SacredSoil) && HasStatusEffect(SCH.Buffs.SacredSoil, anyOwner: true))
                    return SCH.SacredSoil;

            if (actionID is Panhaima && IsEnabled(CustomComboPreset.SGE_OverProtect_Panhaima) &&
                ActionReady(Panhaima) && HasStatusEffect(Buffs.Panhaima, anyOwner: true))
                return SCH.SacredSoil;

            if (actionID is Philosophia && IsEnabled(CustomComboPreset.SGE_OverProtect_Philosophia) &&
                ActionReady(Philosophia) && HasStatusEffect(Buffs.Eudaimonia, anyOwner: true))
                return SCH.Consolation;

            return actionID;
        }
    }
}
