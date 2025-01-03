using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvE;

internal partial class SGE
{
    /*
     * SGE_Kardia
     * Soteria becomes Kardia when Kardia's Buff is not active or Soteria is on cooldown.
     */
    internal class SGE_Kardia : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Kardia;

        protected override uint Invoke(uint actionID)
        {
            return actionID is Soteria && (!HasEffect(Buffs.Kardia) || IsOnCooldown(Soteria)) ? Kardia : actionID;
        }
    }

    /*
     * SGE_Rhizo
     * Replaces all Addersgal using Abilities (Taurochole/Druochole/Ixochole/Kerachole) with Rhizomata if out of Addersgall stacks
     * (Scholar speak: Replaces all Aetherflow abilities with Aetherflow when out)
     */
    internal class SGE_Rhizo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_Rhizo;

        protected override uint Invoke(uint actionID)
        {
            return AddersgallList.Contains(actionID) && ActionReady(Rhizomata) && !Gauge.HasAddersgall() &&
                   IsOffCooldown(actionID)
                ? Rhizomata
                : actionID;
        }
    }

    /*
     * Druo/Tauro
     * Druochole Upgrade to Taurochole (like a trait upgrade)
     * Replaces Druocole with Taurochole when Taurochole is available
     * (As of 6.0) Taurochole (single target massive insta heal w/ cooldown), Druochole (Single target insta heal)
     */
    internal class SGE_DruoTauro : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_TauroDruo;

        protected override uint Invoke(uint actionID)
        {
            return actionID is Taurochole && IsOnCooldown(Taurochole) ? Druochole : actionID;
        }
    }

    /*
     * SGE_ZoePneuma (Zoe to Pneuma Combo)
     * Places Zoe on top of Pneuma when both are available.
     */
    internal class SGE_ZoePneuma : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ZoePneuma;

        protected override uint Invoke(uint actionID)
        {
            return actionID is Pneuma && ActionReady(Pneuma) && IsOffCooldown(Zoe) ? Zoe : actionID;
        }
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
            if (!DyskrasiaList.Contains(actionID)) return actionID;
            if (HasEffect(Buffs.Eukrasia)) return actionID;

            // Variant Rampart
            if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            // Variant Spirit Dart
            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

            if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_SpiritDart) &&
                IsEnabled(Variant.VariantSpiritDart) &&
                (sustainedDamage is null || sustainedDamage.RemainingTime <= 3) &&
                CanSpellWeave())
                return Variant.VariantSpiritDart;

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Lucid) &&
                ActionReady(All.LucidDreaming) && CanSpellWeave() &&
                LocalPlayer.CurrentMp <= Config.SGE_AoE_DPS_Lucid)
                return All.LucidDreaming;

            // Rhizomata
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Rhizo) && CanSpellWeave() &&
                ActionReady(Rhizomata) && Gauge.Addersgall <= Config.SGE_AoE_DPS_Rhizo)
                return Rhizomata;

            //Soteria
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Soteria) && CanSpellWeave() &&
                ActionReady(Soteria) && HasEffect(Buffs.Kardia))
                return Soteria;

            // Addersgall Protection
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_AddersgallProtect) &&
                CanSpellWeave() &&
                ActionReady(Druochole) && Gauge.Addersgall >= Config.SGE_AoE_DPS_AddersgallProtect)
                return Druochole;

            //Eukrasia for DoT
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_EDyskrasia))
                if (IsOffCooldown(Eukrasia) &&
                    !WasLastSpell(
                        EukrasianDyskrasia) && //AoE DoT can be slow to take affect, doesn't apply to target first before others
                    TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
                    HasBattleTarget() &&
                    InActionRange(Dyskrasia) && //Same range
                    DosisList.TryGetValue(OriginalHook(Dosis), out ushort dotDebuffID))
                {
                    float dotDebuff = Math.Max(GetDebuffRemainingTime(dotDebuffID),
                        GetDebuffRemainingTime(Debuffs.EukrasianDyskrasia));

                    float
                        refreshtimer =
                            3; //Will revisit if it's really needed....SGE_ST_DPS_EDosis_Adv ? Config.SGE_ST_DPS_EDosisThreshold : 3;

                    if (dotDebuff <= refreshtimer &&
                        GetTargetHPPercent() >
                        10) //Will Revisit if Config is needed Config.SGE_ST_DPS_EDosisHPPer)
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
                uint PhlegmaID = OriginalHook(Phlegma);

                if (ActionReady(PhlegmaID) &&
                    HasBattleTarget() &&
                    InActionRange(PhlegmaID))
                    return PhlegmaID;
            }

            //Toxikon
            if (IsEnabled(CustomComboPreset.SGE_AoE_DPS_Toxikon))
            {
                uint ToxikonID = OriginalHook(Toxikon);

                if (ActionReady(ToxikonID) &&
                    HasBattleTarget() &&
                    InActionRange(ToxikonID) &&
                    Gauge.HasAddersting())
                    return ToxikonID;
            }

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
            bool ActionFound = actionID is Dosis2 || (!Config.SGE_ST_DPS_Adv && DosisList.ContainsKey(actionID));

            if (!ActionFound) return actionID;

            // Kardia Reminder
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Kardia) && LevelChecked(Kardia) &&
                FindEffect(Buffs.Kardia) is null)
                return Kardia;

            // Opener for SGE
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Opener))
                if (Opener().FullOpener(ref actionID))
                    return actionID;

            // Lucid Dreaming
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Lucid) &&
                All.CanUseLucid(actionID, Config.SGE_ST_DPS_Lucid))
                return All.LucidDreaming;

            // Variant
            if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave())
                return Variant.VariantRampart;

            // Rhizomata
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Rhizo) && CanSpellWeave() &&
                ActionReady(Rhizomata) && Gauge.Addersgall <= Config.SGE_ST_DPS_Rhizo)
                return Rhizomata;

            //Soteria
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Soteria) && CanSpellWeave() &&
                ActionReady(Soteria) && HasEffect(Buffs.Kardia))
                return Soteria;

            // Addersgall Protection
            if (IsEnabled(CustomComboPreset.SGE_ST_DPS_AddersgallProtect) && CanSpellWeave() &&
                ActionReady(Druochole) && Gauge.Addersgall >= Config.SGE_ST_DPS_AddersgallProtect)
                return Druochole;

            if (HasBattleTarget() && !HasEffect(Buffs.Eukrasia))

                // Buff check Above. Without it, Toxikon and any future option will interfere in the Eukrasia->Eukrasia Dosis combo
            {
                // Eukrasian Dosis.
                // If we're too low level to use Eukrasia, we can stop here.
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_EDosis) && LevelChecked(Eukrasia) && InCombat())

                    // Grab current Dosis via OriginalHook, grab it's fellow debuff ID from Dictionary, then check for the debuff
                    // Using TryGetValue due to edge case where the actionID would be read as Eukrasian Dosis instead of Dosis
                    // EDosis will show for half a second if the buff is removed manually or some other act of God
                    if (DosisList.TryGetValue(OriginalHook(actionID), out ushort dotDebuffID))
                    {
                        if (IsEnabled(CustomComboPreset.SGE_DPS_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3 &&
                            CanSpellWeave())
                            return Variant.VariantSpiritDart;

                        // Dosis DoT Debuff
                        float dotDebuff = GetDebuffRemainingTime(dotDebuffID);

                        // Check for the AoE DoT.  These DoTs overlap, so get time remaining of any of them
                        if (TraitLevelChecked(Traits.OffensiveMagicMasteryII))
                            dotDebuff = Math.Max(dotDebuff, GetDebuffRemainingTime(Debuffs.EukrasianDyskrasia));

                        float refreshtimer = Config.SGE_ST_DPS_EDosis_Adv ? Config.SGE_ST_DPS_EDosisThreshold : 3;

                        if (dotDebuff <= refreshtimer &&
                            GetTargetHPPercent() > Config.SGE_ST_DPS_EDosisHPPer)
                            return Eukrasia;
                    }

                // Phlegma
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Phlegma) && InCombat())
                {
                    uint phlegma = OriginalHook(Phlegma);

                    if (InActionRange(phlegma) && ActionReady(phlegma)) return phlegma;
                }

                // Psyche
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Psyche) &&
                    ActionReady(Psyche) &&
                    InCombat() &&
                    CanSpellWeave())
                    return Psyche;

                // Movement Options
                if (IsEnabled(CustomComboPreset.SGE_ST_DPS_Movement) && InCombat() && IsMoving())
                {
                    // Psyche
                    if (Config.SGE_ST_DPS_Movement[3] && ActionReady(Psyche)) return Psyche;

                    // Toxikon
                    if (Config.SGE_ST_DPS_Movement[0] && LevelChecked(Toxikon) && Gauge.HasAddersting())
                        return OriginalHook(Toxikon);

                    // Dyskrasia
                    if (Config.SGE_ST_DPS_Movement[1] && LevelChecked(Dyskrasia) && InActionRange(Dyskrasia))
                        return OriginalHook(Dyskrasia);

                    // Eukrasia
                    if (Config.SGE_ST_DPS_Movement[2] && LevelChecked(Eukrasia)) return Eukrasia;
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

        protected override uint Invoke(uint actionID)
        {
            return actionID is All.Swiftcast && IsOnCooldown(All.Swiftcast) ? Egeiro : actionID;
        }
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
            if (actionID is not Eukrasia || !HasEffect(Buffs.Eukrasia))
                return actionID;

            switch ((int)Config.SGE_Eukrasia_Mode)
            {
                case 0: return OriginalHook(Dosis);

                case 1: return OriginalHook(Diagnosis);

                case 2: return OriginalHook(Prognosis);

                case 3: return OriginalHook(Dyskrasia);
            }

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
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_ST_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Diagnosis) return actionID;

            if (HasEffect(Buffs.Eukrasia))
                return EukrasianDiagnosis;

            IGameObject? healTarget = OptionalTarget ??
                                      GetHealTarget(Config.SGE_ST_Heal_Adv && Config.SGE_ST_Heal_UIMouseOver);

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Esuna) && ActionReady(All.Esuna) &&
                GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) >= Config.SGE_ST_Heal_Esuna &&
                HasCleansableDebuff(healTarget))
                return All.Esuna;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                !Gauge.HasAddersgall())
                return Rhizomata;

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_Kardia) && LevelChecked(Kardia) &&
                FindEffect(Buffs.Kardia) is null &&
                FindEffect(Buffs.Kardion, healTarget, LocalPlayer?.GameObjectId) is null)
                return Kardia;

            foreach (int prio in Config.SGE_ST_Heals_Priority.Items.OrderBy(x => x))
            {
                int index = Config.SGE_ST_Heals_Priority.IndexOf(prio);
                int config = GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                if (enabled)
                    if (GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell;
            }

            if (IsEnabled(CustomComboPreset.SGE_ST_Heal_EDiagnosis) && LevelChecked(Eukrasia) &&
                GetTargetHPPercent(healTarget, Config.SGE_ST_Heal_IncludeShields) <=
                Config.SGE_ST_Heal_EDiagnosisHP &&
                (Config.SGE_ST_Heal_EDiagnosisOpts[0] ||
                 FindEffectOnMember(Buffs.EukrasianDiagnosis, healTarget) is null) && //Ignore existing shield check
                (!Config.SGE_ST_Heal_EDiagnosisOpts[1] ||
                 FindEffectOnMember(SCH.Buffs.Galvanize, healTarget) is null)) //Galvenize Check
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
            if (actionID is not Prognosis) return actionID;

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && HasEffect(Buffs.Eukrasia))
                return OriginalHook(Prognosis);

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_Rhizomata) && ActionReady(Rhizomata) &&
                !Gauge.HasAddersgall())
                return Rhizomata;

            foreach (int prio in Config.SGE_AoE_Heals_Priority.Items.OrderBy(x => x))
            {
                int index = Config.SGE_AoE_Heals_Priority.IndexOf(prio);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);

                if (enabled)
                    if (ActionReady(spell))
                        return spell;
            }

            if (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis) && LevelChecked(Eukrasia) &&
                (IsEnabled(CustomComboPreset.SGE_AoE_Heal_EPrognosis_IgnoreShield) ||
                 FindEffect(Buffs.EukrasianPrognosis) is null))
                return Eukrasia;

            return actionID;
        }
    }

    internal class SGE_OverProtect : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGE_OverProtect;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Kerachole or Panhaima or Philosophia)) return actionID;

            if (actionID is Kerachole && IsEnabled(CustomComboPreset.SGE_OverProtect_Kerachole) &&
                ActionReady(Kerachole))
                if (HasEffectAny(Buffs.Kerachole) ||
                    (IsEnabled(CustomComboPreset.SGE_OverProtect_SacredSoil) && HasEffectAny(SCH.Buffs.SacredSoil)))
                    return SCH.SacredSoil;

            if (actionID is Panhaima && IsEnabled(CustomComboPreset.SGE_OverProtect_Panhaima) &&
                ActionReady(Panhaima) && HasEffectAny(Buffs.Panhaima)) return SCH.SacredSoil;

            if (actionID is Philosophia && IsEnabled(CustomComboPreset.SGE_OverProtect_Philosophia) &&
                ActionReady(Philosophia) && HasEffectAny(Buffs.Eudaimonia)) return SCH.Consolation;

            return actionID;
        }
    }
}
