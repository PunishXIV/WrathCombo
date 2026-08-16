using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using WrathCombo.Native;
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
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, DosisActions))
                return actionID;

            if (UseKardia())
                return Kardia.Retarget(actionID, Target);

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave() && !HasStatusEffect(Buffs.Eukrasia))
            {
                if (UseAddersgallProtect(3))
                    return Druochole.RetargetIfEnabled([actionID]);

                if (UsePsyche(PhlegmaBurstPair(true, true, true)))
                    return Psyche;

                if (UseLucid(7500))
                    return Role.LucidDreaming;

                if (UseRhizo(1))
                    return Rhizomata;

                if (UseSoteria())
                    return Soteria;
            }

            if (UseEDosis(ref actionID, simpleMode: true, [actionID]))
                return actionID;

            if (HasBattleTarget() && !HasStatusEffect(Buffs.Eukrasia) && InCombat())
            {
                if (UsePhlegma(burst: true, chargePool: 1, psycheEnabled: true))
                    return OriginalHook(Phlegma);

                if (UseMovement(ref actionID, simpleMode: true))
                    return actionID;
            }

            return OriginalHook(Dosis);
        }
    }

    internal class SGE_AoE_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_Simple_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, DyskrasiaList.ToArray()))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (CanWeave())
            {
                if (UseAddersgallProtect(3))
                    return Druochole.RetargetIfEnabled([actionID]);

                if (UsePsyche(PhlegmaBurstPair(true, true, true)))
                    return Psyche;

                if (UseLucid(7500))
                    return Role.LucidDreaming;

                if (UseRhizo(1))
                    return Rhizomata;

                if (UseSoteria())
                    return Soteria;
            }

            if (UseEDyskrasia())
                return Eukrasia;

            if (UseAoEPhlegma(psycheEnabled: true))
                return OriginalHook(Phlegma);

            if (UseAoEToxikon())
                return OriginalHook(Toxikon);

            if (UseAoEPneuma(allowNonBoss: true))
                return Pneuma;

            return OriginalHook(Dyskrasia);
        }
    }

    #endregion

    #region Advanced DPS Mode

    internal class SGE_ST_Advanced_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_ST_Advanced_DPS;

        protected override uint Invoke(uint actionID)
        {
            uint[] dosisActions = (int)SGE_ST_Adv_DPS_Advanced == 1
                ? [Dosis2]
                : DosisList.Keys.ToArray();

            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, dosisActions))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.SingleTargetDPS))
                dosisActions = [All.SingleTargetDPS];

            if (IsEnabled(Preset.SGE_ST_Adv_DPS_Kardia) && UseKardia())
                return Kardia.Retarget(actionID, Target);

            if (IsEnabled(Preset.SGE_ST_Adv_DPS_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (UseRaidwide(ref actionID))
                return actionID;

            if (CanWeave() && !HasStatusEffect(Buffs.Eukrasia))
            {
                if (IsEnabled(Preset.SGE_ST_Adv_DPS_AddersgallProtect) &&
                    UseAddersgallProtect(SGE_ST_Adv_DPS_AddersgallProtect))
                    return Druochole.RetargetIfEnabled(dosisActions);

                bool psycheEnabled = IsEnabled(Preset.SGE_ST_Adv_DPS_Psyche);
                bool phlegmaEnabled = IsEnabled(Preset.SGE_ST_Adv_DPS_Phlegma);
                if (psycheEnabled &&
                    UsePsyche(PhlegmaBurstPair(phlegmaEnabled, psycheEnabled, SGE_ST_Adv_DPS_Phlegma_Burst)))
                    return Psyche;

                if (IsEnabled(Preset.SGE_ST_Adv_DPS_Lucid) &&
                    UseLucid(SGE_ST_Adv_DPS_Lucid))
                    return Role.LucidDreaming;

                if (IsEnabled(Preset.SGE_ST_Adv_DPS_Rhizo) &&
                    UseRhizo(SGE_ST_Adv_DPS_Rhizo))
                    return Rhizomata;

                if (IsEnabled(Preset.SGE_ST_Adv_DPS_Soteria) && UseSoteria())
                    return Soteria;
            }

            if (IsEnabled(Preset.SGE_ST_Adv_DPS_EDosis) &&
                UseEDosis(ref actionID, simpleMode: false, dosisActions))
                return actionID;

            if (HasBattleTarget() && !HasStatusEffect(Buffs.Eukrasia) && InCombat())
            {
                if (IsEnabled(Preset.SGE_ST_Adv_DPS_Phlegma) &&
                    UsePhlegma(
                        burst: SGE_ST_Adv_DPS_Phlegma_Burst,
                        chargePool: SGE_ST_Adv_DPS_Phlegma,
                        psycheEnabled: IsEnabled(Preset.SGE_ST_Adv_DPS_Psyche)))
                    return OriginalHook(Phlegma);

                if (IsEnabled(Preset.SGE_ST_Adv_DPS_Movement) &&
                    UseMovement(ref actionID, simpleMode: false))
                    return actionID;
            }

            return OriginalHook(Dosis);
        }
    }

    internal class SGE_AoE_Advanced_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_Advanced_DPS;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, DyskrasiaList.ToArray()))
                return actionID;

            if (ContentSpecificActions.TryGet(ref actionID, out uint contentAction))
                return contentAction;

            if (UseRaidwide(ref actionID))
                return actionID;

            if (CanWeave())
            {
                if (IsEnabled(Preset.SGE_AoE_Adv_DPS_AddersgallProtect) &&
                    UseAddersgallProtect(SGE_AoE_Adv_DPS_AddersgallProtect))
                    return Druochole.RetargetIfEnabled([actionID]);

                bool psycheEnabled = IsEnabled(Preset.SGE_AoE_Adv_DPS_Psyche);
                bool phlegmaEnabled = IsEnabled(Preset.SGE_AoE_Adv_DPS_Phlegma);
                if (psycheEnabled &&
                    UsePsyche(PhlegmaBurstPair(phlegmaEnabled, psycheEnabled, burst: true)))
                    return Psyche;

                if (IsEnabled(Preset.SGE_AoE_Adv_DPS_Lucid) &&
                    UseLucid(SGE_AoE_Adv_DPS_Lucid))
                    return Role.LucidDreaming;

                if (IsEnabled(Preset.SGE_AoE_Adv_DPS_Rhizo) &&
                    UseRhizo(SGE_AoE_Adv_DPS_Rhizo))
                    return Rhizomata;

                if (IsEnabled(Preset.SGE_AoE_Adv_DPS_Soteria) && UseSoteria())
                    return Soteria;
            }

            if (IsEnabled(Preset.SGE_AoE_Adv_DPS_EDyskrasia) && UseEDyskrasia())
                return Eukrasia;

            if (IsEnabled(Preset.SGE_AoE_Adv_DPS_Phlegma) &&
                UseAoEPhlegma(IsEnabled(Preset.SGE_AoE_Adv_DPS_Psyche)))
                return OriginalHook(Phlegma);

            if (IsEnabled(Preset.SGE_AoE_Adv_DPS_Toxikon) && UseAoEToxikon())
                return OriginalHook(Toxikon);

            if (IsEnabled(Preset.SGE_AoE_Adv_DPS_Pneuma) &&
                UseAoEPneuma(SGE_AoE_Adv_DPS_PneumaBossOption == 0))
                return Pneuma;

            return OriginalHook(Dyskrasia);
        }
    }

    #endregion

    #region Simple Heal Mode

    internal class SGE_ST_Simple_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_ST_Simple_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetHeals, Diagnosis))
                return actionID;

            IGameObject? healTarget = SimpleTarget.Stack.OneButtonHealLogic;

            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);

            if (LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia))
                return Kardia.Retarget(actionID, SimpleTarget.AnyLivingTank);

            if (ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget) >= 40 &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(actionID);

            if (CanWeave())
            {
                if (UseLucid(6500))
                    return Role.LucidDreaming;

                if (UseRhizo(1))
                    return Rhizomata;

                if (UseSoteria())
                    return Soteria;
            }

            if (ActionReady(OriginalHook(Physis)) &&
                !InBossEncounter())
                return OriginalHook(Physis);

            if (ActionReady(Kerachole) &&
                TraitLevelChecked(Traits.EnhancedKerachole) &&
                HasAddersgall &&
                !InBossEncounter())
                return Kerachole;

            if ((healTarget.IsInParty() && healTarget.Role is CombatRole.Tank) || !IsInParty())
            {
                if (ActionReady(Krasis))
                    return Krasis.RetargetIfEnabled(actionID);

                if (ActionReady(Taurochole) && HasAddersgall)
                    return Taurochole.RetargetIfEnabled(actionID);

                if (ActionReady(Haima) && !HasStatusEffect(Buffs.Panhaima, healTarget))
                    return Haima.RetargetIfEnabled(actionID);
            }

            if (ActionReady(Druochole) && HasAddersgall)
                return Druochole.RetargetIfEnabled(actionID);

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

            return Diagnosis.RetargetIfEnabled(actionID);
        }
    }

    internal class SGE_AoE_Simple_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_Simple_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEHeals, Prognosis))
                return actionID;

            if (CanWeave())
            {
                if (UseLucid(6500))
                    return Role.LucidDreaming;

                if (UseRhizo(1))
                    return Rhizomata;
            }

            if (HasStatusEffect(Buffs.Eukrasia))
                return OriginalHook(Prognosis);

            if (ActionReady(Eukrasia) &&
                GetPartyBuffPercent(Buffs.EukrasianPrognosis) <= 50 &&
                GetPartyBuffPercent(SCH.Buffs.Galvanize) <= 50 &&
                !HasStatusEffect(Buffs.Eukrasia))
                return Eukrasia;

            if (CanWeave())
            {
                if (UsePhysis())
                    return OriginalHook(Physis);

                if (UseKerachole(requireEnhanced: true))
                    return Kerachole;

                if (UseHolos())
                    return Holos;

                if (UseIxochole())
                    return Ixochole;

                if (UsePhilosophia())
                    return Philosophia;

                if (UsePanhaima())
                    return Panhaima;

                if (UseZoe())
                    return Zoe;

                if (UseAoEPepsis())
                    return Pepsis;
            }

            return OriginalHook(Prognosis);
        }
    }

    #endregion

    #region Advanced Heal Mode

    internal class SGE_ST_Advanced_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_ST_Advanced_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetHeals, Diagnosis))
                return actionID;

            IGameObject? healTarget = SimpleTarget.Stack.OneButtonHealLogic;

            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);

            if (UseRaidwide(ref actionID))
                return actionID;

            if (IsEnabled(Preset.SGE_ST_Adv_Heal_Esuna) &&
                ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, SGE_ST_Adv_Heal_IncludeShields) >= SGE_ST_Adv_Heal_Esuna &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(actionID);

            if (HasStatusEffect(Buffs.Eukrasia))
                return EukrasianDiagnosis.RetargetIfEnabled(actionID);

            if (IsEnabled(Preset.SGE_ST_Adv_Heal_Kardia) &&
                LevelChecked(Kardia) &&
                !HasStatusEffect(Buffs.Kardia) &&
                !HasStatusEffect(Buffs.Kardion, healTarget))
                return Kardia.Retarget(actionID, Target);

            if (CanWeave())
            {
                if (IsEnabled(Preset.SGE_ST_Adv_Heal_Lucid) &&
                    UseLucid(SGE_ST_Adv_Heal_LucidOption))
                    return Role.LucidDreaming;

                if (IsEnabled(Preset.SGE_ST_Adv_Heal_Rhizomata) &&
                    UseRhizo(1))
                    return Rhizomata;
            }

            for (int i = 0; i < SGE_ST_Heals_Priority.Count; i++)
            {
                int index = SGE_ST_Heals_Priority.IndexOf(i + 1);
                if (!TrySTHealOption(index, healTarget, out uint spell, out int config))
                    continue;

                if (GetTargetHPPercent(healTarget, SGE_ST_Adv_Heal_IncludeShields) <= config &&
                    ActionReady(spell))
                    return spell.RetargetIfEnabled(actionID);
            }

            return Diagnosis.RetargetIfEnabled(actionID);
        }
    }

    internal class SGE_AoE_Advanced_Heal : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGE_AoE_Advanced_Heal;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEHeals, Prognosis))
                return actionID;

            if (UseRaidwide(ref actionID))
                return actionID;

            if (IsEnabled(Preset.SGE_AoE_Adv_Heal_EPrognosis) &&
                HasStatusEffect(Buffs.Eukrasia))
                return OriginalHook(Prognosis);

            if (CanWeave())
            {
                if (IsEnabled(Preset.SGE_AoE_Adv_Heal_Lucid) &&
                    UseLucid(SGE_AoE_Adv_Heal_LucidOption))
                    return Role.LucidDreaming;

                if (IsEnabled(Preset.SGE_AoE_Adv_Heal_Rhizomata) &&
                    UseRhizo(1))
                    return Rhizomata;
            }

            float averagePartyHP = GetPartyAvgHPPercent();
            for (int i = 0; i < SGE_AoE_Heals_Priority.Count; i++)
            {
                int index = SGE_AoE_Heals_Priority.IndexOf(i + 1);
                if (!TryAoEHealOption(index, out uint spell, out int config))
                    continue;

                if (averagePartyHP <= config && ActionReady(spell))
                    return spell;
            }

            return OriginalHook(Prognosis);
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
                   ActionReady(Rhizomata) && !HasAddersgall && IsOffCooldown(actionID)
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

            if (SGE_Eukrasia_Mode == 1)
                return IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis)
                    ? EukrasianDiagnosis.Retarget(Eukrasia, HealStack)
                    : EukrasianDiagnosis;

            if (SGE_Eukrasia_Mode == 2)
                return OriginalHook(Prognosis);

            if (SGE_Eukrasia_Mode == 3)
                return OriginalHook(Dyskrasia);

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
                    ? Druochole.Retarget(Taurochole, HealStack)
                    : Druochole;

            return IsEnabled(Preset.SGE_Retarget_Taurochole)
                ? Taurochole.Retarget(HealStack)
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
                    ? Kardia.Retarget(actionID, HealStack)
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
                    ? Krasis.Retarget(HealStack)
                    : actionID;

            if (!HasStatusEffect(Buffs.EukrasianDiagnosis, HealStack))
            {
                if (!HasStatusEffect(Buffs.Eukrasia))
                    return Eukrasia;

                return IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis)
                    ? EukrasianDiagnosis.Retarget(Krasis, HealStack)
                    : EukrasianDiagnosis;
            }

            if (SGE_Mit_ST_Options[0] && !ActionReady(Krasis) &&
                ActionReady(Haima))
                return IsEnabled(Preset.SGE_Retarget_Haima)
                    ? Haima.Retarget(Krasis, HealStack)
                    : Haima;

            if (SGE_Mit_ST_Options[1] && !ActionReady(Krasis) &&
                ActionReady(Taurochole) &&
                GetTargetHPPercent(HealStack) <= SGE_Mit_ST_TaurocholeThreshold)
                return IsEnabled(Preset.SGE_Retarget_Taurochole)
                    ? Taurochole.Retarget(Krasis, HealStack)
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

            if (SGE_Mit_AoE_Options[1] &&
                ActionReady(Kerachole) &&
                !HasStatusEffect(Buffs.Kerachole, anyOwner: true) &&
                !HasStatusEffect(SCH.Buffs.SacredSoil, anyOwner: true))
                return Kerachole;

            if (SGE_Mit_AoE_Options[0] &&
                ActionReady(Philosophia))
                return Philosophia;

            if (GetPartyBuffPercent(Buffs.EukrasianPrognosis) < SGE_Mit_AoE_PrognosisOption)
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
                OriginalHook(Diagnosis).Retarget(HealStack);

            if (IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis))
                EukrasianDiagnosis.Retarget(Diagnosis, HealStack);

            if (IsEnabled(Preset.SGE_Retarget_Haima))
                Haima.Retarget(HealStack);

            if (IsEnabled(Preset.SGE_Retarget_Druochole))
                Druochole.Retarget(HealStack);

            if (IsEnabled(Preset.SGE_Retarget_Taurochole))
                Taurochole.Retarget(HealStack);

            if (IsEnabled(Preset.SGE_Retarget_Krasis))
                Krasis.Retarget(HealStack);

            if (IsEnabled(Preset.SGE_Retarget_Kardia))
                Kardia.Retarget(HealStack);

            if (IsEnabled(Preset.SGE_Retarget_Icarus))
                Icarus.Retarget(SimpleTarget.Stack.MouseOver ?? SimpleTarget.HardTarget);

            return actionID;
        }
    }

    #endregion
}
