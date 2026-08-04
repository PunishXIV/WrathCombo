using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.SGE.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class SGE
{
    private static bool IsPhlegmaCapped =>
        GetRemainingCharges(OriginalHook(Phlegma)) == GetMaxCharges(OriginalHook(Phlegma));

    private static IGameObject? Target =>
        SimpleTarget.UIMouseOverTarget.IfCanUseOn(Kardia).IfWithinRange(30) ??
        SimpleTarget.HardTarget.IfCanUseOn(Kardia).IfWithinRange(30) ??
        SimpleTarget.AnyTank;

    private static IGameObject? HealStack =>
        SimpleTarget.Stack.AllyToHeal;

    private static bool HasAddersgall => Addersgall > 0;

    private static bool HasAddersgallAboveHold => Addersgall > SGE_Heal_HoldAddersgall;

    private static bool HasAddersting => Addersting > 0;

    #region Lists

    internal static readonly FrozenDictionary<uint, ushort> EukrasianDosisList = new Dictionary<uint, ushort>
    {
        { Dosis, Debuffs.EukrasianDosis },
        { Dosis2, Debuffs.EukrasianDosis2 },
        { Dosis3, Debuffs.EukrasianDosis3 }
    }.ToFrozenDictionary();

    private static readonly List<uint>
        AddersgallList = [Taurochole, Druochole, Ixochole, Kerachole],
        DyskrasiaList = [Dyskrasia, Dyskrasia2];

    private static readonly FrozenDictionary<uint, (ushort Debuff, uint Eukrasian)> DosisList = new Dictionary<uint, (ushort D, uint E)>
    {
        { Dosis, (D: Debuffs.EukrasianDosis, E: EukrasianDosis) },
        { Dosis2, (D: Debuffs.EukrasianDosis2, E: EukrasianDosis2) },
        { Dosis3, (D: Debuffs.EukrasianDosis3, E: EukrasianDosis3) },
        { EukrasianDosis, (D: Debuffs.EukrasianDosis, E: EukrasianDosis) },
        { EukrasianDosis2, (D: Debuffs.EukrasianDosis2, E: EukrasianDosis2) },
        { EukrasianDosis3, (D: Debuffs.EukrasianDosis3, E: EukrasianDosis3) }
    }.ToFrozenDictionary();

    #endregion

    #region Gauge

    private static SGEGauge Gauge => GetJobGauge<SGEGauge>();

    private static byte Addersgall => Gauge.Addersgall;

    private static byte Addersting => Gauge.Addersting;

    #endregion

    #region Dot Checker

    internal static bool ShouldRefreshEDosis()
    {
        uint dotAction = OriginalHook(Dosis);
        int hpThreshold = IsNotEnabled(Preset.SGE_ST_Simple_DPS) ? EDosisHpThreshold(CurrentTarget) : 0;
        EukrasianDosisList.TryGetValue(dotAction, out ushort dotDebuffID);
        double dotRefresh = IsNotEnabled(Preset.SGE_ST_Simple_DPS) ? SGE_ST_Adv_DPS_EukrasianDosisUptime_Threshold : 2.5;
        float dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(Eukrasia) &&
               CanApplyStatus(CurrentTarget, dotDebuffID) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh;
    }

    internal static int EDosisHpThreshold(IGameObject? x)
    {
        if (x is null)
            return 0;

        if (InBossEncounter())
            return x.IsBoss() ? SGE_ST_Adv_DPS_EukrasianDosisBossOption : SGE_ST_Adv_DPS_EukrasianDosisBossAddsOption;

        return SGE_ST_Adv_DPS_EukrasianDosisTrashOption;
    }

    #endregion

    #region Combo

    private static bool UseKardia(bool simpleMode)
    {
        if ((!simpleMode && !IsEnabled(Preset.SGE_ST_Adv_DPS_Kardia)) ||
            !LevelChecked(Kardia) ||
            HasStatusEffect(Buffs.Kardia) ||
            Target is null)
            return false;

        return true;
    }

    private static bool UseRaidwide(ref uint actionID)
    {
        if (CanWeave())
        {
            if (RaidwideKerachole())
            {
                actionID = Kerachole;
                return true;
            }

            if (RaidwideHolos())
            {
                actionID = Holos;
                return true;
            }
        }

        if (RaidwideEprognosis())
        {
            actionID = HasStatusEffect(Buffs.Eukrasia)
                ? OriginalHook(Prognosis)
                : Eukrasia;
            return true;
        }

        return false;
    }

    private static bool UseDPSWeave(ref uint actionID, bool simpleMode, bool onAoE, uint[] retargetIds)
    {
        if (!onAoE && HasStatusEffect(Buffs.Eukrasia))
            return false;

        int addersgallProtect = simpleMode
            ? 3
            : onAoE
                ? SGE_AoE_Adv_DPS_AddersgallProtect
                : SGE_ST_Adv_DPS_AddersgallProtect;
        Preset protectPreset = onAoE
            ? Preset.SGE_AoE_Adv_DPS_AddersgallProtect
            : Preset.SGE_ST_Adv_DPS_AddersgallProtect;
        if ((simpleMode || IsEnabled(protectPreset)) &&
            ActionReady(Druochole) && Addersgall >= addersgallProtect)
        {
            actionID = Druochole.RetargetIfEnabled(retargetIds);
            return true;
        }

        Preset psychePreset = onAoE ? Preset.SGE_AoE_Adv_DPS_Psyche : Preset.SGE_ST_Adv_DPS_Psyche;
        if ((simpleMode || IsEnabled(psychePreset)) &&
            ActionReady(Psyche) &&
            HasBattleTarget() &&
            InActionRange(Psyche))
        {
            actionID = Psyche;
            return true;
        }

        int lucidMp = simpleMode
            ? 7500
            : onAoE
                ? SGE_AoE_Adv_DPS_Lucid
                : SGE_ST_Adv_DPS_Lucid;
        Preset lucidPreset = onAoE ? Preset.SGE_AoE_Adv_DPS_Lucid : Preset.SGE_ST_Adv_DPS_Lucid;
        if ((simpleMode || IsEnabled(lucidPreset)) &&
            Role.CanLucidDream(lucidMp))
        {
            actionID = Role.LucidDreaming;
            return true;
        }

        int rhizoThreshold = simpleMode
            ? 1
            : onAoE
                ? SGE_AoE_Adv_DPS_Rhizo
                : SGE_ST_Adv_DPS_Rhizo;
        Preset rhizoPreset = onAoE ? Preset.SGE_AoE_Adv_DPS_Rhizo : Preset.SGE_ST_Adv_DPS_Rhizo;
        if ((simpleMode || IsEnabled(rhizoPreset)) &&
            ActionReady(Rhizomata) && Addersgall < rhizoThreshold)
        {
            actionID = Rhizomata;
            return true;
        }

        Preset soteriaPreset = onAoE ? Preset.SGE_AoE_Adv_DPS_Soteria : Preset.SGE_ST_Adv_DPS_Soteria;
        if ((simpleMode || IsEnabled(soteriaPreset)) &&
            ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
        {
            actionID = Soteria;
            return true;
        }

        return false;
    }

    private static bool UsePhlegma(bool simpleMode)
    {
        if ((!simpleMode && !IsEnabled(Preset.SGE_ST_Adv_DPS_Phlegma)) ||
            !InActionRange(OriginalHook(Phlegma)) ||
            !ActionReady(OriginalHook(Phlegma)))
            return false;

        bool burst = simpleMode || SGE_ST_Adv_DPS_Phlegma_Burst;
        int chargePool = simpleMode ? 1 : SGE_ST_Adv_DPS_Phlegma;

        if (IsPhlegmaCapped || GetRemainingCharges(OriginalHook(Phlegma)) > chargePool)
            return true;

        if (!burst || !LevelChecked(Psyche))
            return false;

        return IsOffCooldown(Psyche) ||
               JustUsed(Psyche, 5f) ||
               IsMoving();
    }

    private static bool UseMovement(ref uint actionID, bool simpleMode)
    {
        if (!IsMoving())
            return false;

        if (simpleMode)
        {
            if (ActionReady(OriginalHook(Toxikon)) && HasAddersting)
            {
                actionID = OriginalHook(Toxikon);
                return true;
            }

            if (ActionReady(Dyskrasia) && InActionRange(Dyskrasia))
            {
                actionID = OriginalHook(Dyskrasia);
                return true;
            }

            return false;
        }

        if (!IsEnabled(Preset.SGE_ST_Adv_DPS_Movement))
            return false;

        foreach (int priority in SGE_ST_Adv_DPS_Movement_Priority.OrderBy(x => x))
        {
            int index = SGE_ST_Adv_DPS_Movement_Priority.IndexOf(priority);
            if (TryMovementOption(index, ref actionID))
                return true;
        }

        return false;
    }

    private static bool UseEDosis(ref uint actionID, bool simpleMode, uint[] retargetIds)
    {
        uint dotAction = OriginalHook(Dosis);
        DosisList.TryGetValue(dotAction, out (ushort Debuff, uint Eukrasian) debuff);

        if (simpleMode)
        {
            IGameObject? target = SimpleTarget.DottableEnemy(debuff.Eukrasian, debuff.Debuff, 0, 3, 99);
            if (target is not null && CanApplyStatus(target, debuff.Debuff) &&
                !JustUsedOn(debuff.Eukrasian, target) && LevelChecked(Eukrasia))
            {
                actionID = HasStatusEffect(Buffs.Eukrasia)
                    ? dotAction.Retarget(retargetIds, target)
                    : Eukrasia;
                return true;
            }

            return false;
        }

        if (!IsEnabled(Preset.SGE_ST_Adv_DPS_EDosis) || !PartyInCombat())
            return false;

        if (ShouldRefreshEDosis())
        {
            actionID = HasStatusEffect(Buffs.Eukrasia) ? dotAction : Eukrasia;
            return true;
        }

        IGameObject? multiTarget = SimpleTarget.DottableEnemy(
            debuff.Eukrasian, debuff.Debuff, EDosisHpThreshold,
            SGE_ST_Adv_DPS_EukrasianDosisUptime_Threshold, 2);

        if (multiTarget is not null && CanApplyStatus(multiTarget, debuff.Debuff) &&
            !JustUsedOn(debuff.Eukrasian, multiTarget) &&
            SGE_ST_Adv_DPS_EDosis_TwoTarget && LevelChecked(Eukrasia))
        {
            actionID = HasStatusEffect(Buffs.Eukrasia)
                ? dotAction.Retarget(retargetIds, multiTarget)
                : Eukrasia;
            return true;
        }

        return false;
    }

    private static bool UseEDyskrasia(bool simpleMode)
    {
        if ((!simpleMode && !IsEnabled(Preset.SGE_AoE_Adv_DPS_EDyskrasia)) ||
            !HasEDyskrasiaTargets() ||
            JustUsed(EukrasianDyskrasia) ||
            !TraitLevelChecked(Traits.OffensiveMagicMasteryII) ||
            !ActionReady(Eukrasia))
            return false;

        return true;
    }

    private static bool UseAoEPhlegma(bool simpleMode) =>
        (simpleMode || IsEnabled(Preset.SGE_AoE_Adv_DPS_Phlegma)) &&
        ActionReady(OriginalHook(Phlegma)) &&
        HasBattleTarget() &&
        InActionRange(OriginalHook(Phlegma));

    private static bool UseAoEToxikon(bool simpleMode) =>
        (simpleMode || IsEnabled(Preset.SGE_AoE_Adv_DPS_Toxikon)) &&
        ActionReady(OriginalHook(Toxikon)) &&
        HasBattleTarget() && HasAddersting &&
        InActionRange(OriginalHook(Toxikon));

    private static bool UseAoEPneuma(bool simpleMode) =>
        (simpleMode || IsEnabled(Preset.SGE_AoE_Adv_DPS_Pneuma)) &&
        (simpleMode || SGE_AoE_Adv_DPS_PneumaBossOption == 0 || TargetIsBoss()) &&
        ActionReady(Pneuma) && HasBattleTarget() &&
        InActionRange(Pneuma);

    private static bool UseAoEDPSGCD(ref uint actionID, bool simpleMode)
    {
        if (UseEDyskrasia(simpleMode))
        {
            actionID = Eukrasia;
            return true;
        }

        if (UseAoEPhlegma(simpleMode))
        {
            actionID = OriginalHook(Phlegma);
            return true;
        }

        if (UseAoEToxikon(simpleMode))
        {
            actionID = OriginalHook(Toxikon);
            return true;
        }

        if (UseAoEPneuma(simpleMode))
        {
            actionID = Pneuma;
            return true;
        }

        return false;
    }

    private static bool HasEDyskrasiaTargets() =>
        EnemiesInRange(EukrasianDyskrasia).Count(x =>
            (GetPossessedStatusRemainingTime(Debuffs.EukrasianDyskrasia, x) is <= 4 or float.NaN &&
             GetPossessedStatusRemainingTime(DosisList[OriginalHook(Dosis)].Debuff, x) is <= 4 or float.NaN) &&
            GetTargetHPPercent(x) > 25) >= 4;

    private static (uint Action, Func<bool> Logic)[] PrioritizedMovement =>
    [
        (OriginalHook(Toxikon),
            () => SGE_ST_Adv_DPS_Movement[0] &&
                  ActionReady(OriginalHook(Toxikon)) &&
                  HasAddersting),
        (OriginalHook(Dyskrasia),
            () => SGE_ST_Adv_DPS_Movement[1] &&
                  ActionReady(OriginalHook(Dyskrasia)) &&
                  InActionRange(OriginalHook(Dyskrasia))),
        (Eukrasia,
            () => SGE_ST_Adv_DPS_Movement[2] &&
                  ActionReady(Eukrasia) &&
                  !HasStatusEffect(Buffs.Eukrasia))
    ];

    private static bool TryMovementOption(int index, ref uint actionID)
    {
        uint candidate = PrioritizedMovement[index].Action;
        if (!ActionReady(candidate) || !LevelChecked(candidate) ||
            !PrioritizedMovement[index].Logic())
            return false;

        actionID = candidate;
        return true;
    }

    #endregion

    #region Healing

    #region Raidwides

    private static bool RaidwideKerachole() =>
        IsEnabled(Preset.SGE_Raidwide_Kerachole) &&
        ActionReady(Kerachole) && HasAddersgallAboveHold &&
        GroupDamageIncoming();

    private static bool RaidwideHolos() =>
        IsEnabled(Preset.SGE_Raidwide_Holos) &&
        ActionReady(Holos) && GroupDamageIncoming() &&
        GetPartyAvgHPPercent() <= SGE_Raidwide_HolosOption;

    private static bool RaidwideEprognosis()
    {
        bool shieldCheck = GetPartyBuffPercent(Buffs.EukrasianPrognosis) <= SGE_AoE_Adv_Heal_EPrognosisOption &&
                           GetPartyBuffPercent(SCH.Buffs.Galvanize) <= SGE_AoE_Adv_Heal_EPrognosisOption;

        return IsEnabled(Preset.SGE_Raidwide_EPrognosis) && shieldCheck && GroupDamageIncoming() && LevelChecked(Eukrasia);
    }

    #endregion

    private static bool UseHealWeave(ref uint actionID, bool simpleMode, bool onAoE)
    {
        int lucidMp = simpleMode
            ? 6500
            : onAoE
                ? SGE_AoE_Adv_Heal_LucidOption
                : SGE_ST_Adv_Heal_LucidOption;
        Preset lucidPreset = onAoE ? Preset.SGE_AoE_Adv_Heal_Lucid : Preset.SGE_ST_Adv_Heal_Lucid;
        if ((simpleMode || IsEnabled(lucidPreset)) &&
            Role.CanLucidDream(lucidMp))
        {
            actionID = Role.LucidDreaming;
            return true;
        }

        Preset rhizoPreset = onAoE ? Preset.SGE_AoE_Adv_Heal_Rhizomata : Preset.SGE_ST_Adv_Heal_Rhizomata;
        if ((simpleMode || IsEnabled(rhizoPreset)) &&
            ActionReady(Rhizomata) && !HasAddersgall)
        {
            actionID = Rhizomata;
            return true;
        }

        if (simpleMode && !onAoE &&
            ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia))
        {
            actionID = Soteria;
            return true;
        }

        return false;
    }

    private static bool UseAoEHealWeave(ref uint actionID)
    {
        if (ActionReady(OriginalHook(Physis)))
        {
            actionID = OriginalHook(Physis);
            return true;
        }

        if (ActionReady(Kerachole) &&
            TraitLevelChecked(Traits.EnhancedKerachole) &&
            HasAddersgall)
        {
            actionID = Kerachole;
            return true;
        }

        if (ActionReady(Holos))
        {
            actionID = Holos;
            return true;
        }

        if (ActionReady(Ixochole) && HasAddersgall)
        {
            actionID = Ixochole;
            return true;
        }

        if (ActionReady(Philosophia) && !HasStatusEffect(Buffs.Panhaima))
        {
            actionID = Philosophia;
            return true;
        }

        if (ActionReady(Panhaima) && !HasStatusEffect(Buffs.Eudaimonia))
        {
            actionID = Panhaima;
            return true;
        }

        if (ActionReady(Zoe) && (ActionReady(Pneuma) || !LevelChecked(Pneuma)))
        {
            actionID = Zoe;
            return true;
        }

        if (ActionReady(Pepsis) &&
            HasStatusEffect(Buffs.EukrasianPrognosis))
        {
            actionID = Pepsis;
            return true;
        }

        return false;
    }

    private static uint DoSTSimpleHeal(uint actionID)
    {
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

        if (CanWeave() && UseHealWeave(ref actionID, simpleMode: true, onAoE: false))
            return actionID;

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

    private static uint DoAoESimpleHeal(uint actionID)
    {
        if (CanWeave() && UseHealWeave(ref actionID, simpleMode: true, onAoE: true))
            return actionID;

        if (HasStatusEffect(Buffs.Eukrasia))
            return OriginalHook(Prognosis);

        if (CanWeave() && UseAoEHealWeave(ref actionID))
            return actionID;

        if (ActionReady(Eukrasia) &&
            GetPartyBuffPercent(Buffs.EukrasianPrognosis) <= 50 &&
            GetPartyBuffPercent(SCH.Buffs.Galvanize) <= 50 &&
            !HasStatusEffect(Buffs.Eukrasia))
            return Eukrasia;

        return OriginalHook(Prognosis);
    }

    private static uint DoSTAdvancedHeal(uint actionID)
    {
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

        if (CanWeave() && UseHealWeave(ref actionID, simpleMode: false, onAoE: false))
            return actionID;

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

    private static uint DoAoEAdvancedHeal(uint actionID)
    {
        if (UseRaidwide(ref actionID))
            return actionID;

        if (IsEnabled(Preset.SGE_AoE_Adv_Heal_EPrognosis) &&
            HasStatusEffect(Buffs.Eukrasia))
            return OriginalHook(Prognosis);

        if (CanWeave() && UseHealWeave(ref actionID, simpleMode: false, onAoE: true))
            return actionID;

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

    private static bool TrySTHealOption(int i, IGameObject? target, out uint action, out int config)
    {
        IGameObject? healTarget = target ?? SimpleTarget.Stack.AllyToHeal;
        action = Diagnosis;
        config = 0;

        bool shieldCheck = !SGE_ST_Adv_Heal_EDiagnosisOpts[0] ||
                           (!HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget, true) &&
                            !HasStatusEffect(Buffs.EukrasianPrognosis, healTarget, true));

        bool scholarShieldCheck = !SGE_ST_Adv_Heal_EDiagnosisOpts[1] ||
                                  !HasStatusEffect(SCH.Buffs.Galvanize);
        bool tankCheck = healTarget.IsInParty() && healTarget.Role is CombatRole.Tank;

        switch (i)
        {
            case 0:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Soteria))
                    return false;
                action = Soteria;
                config = SGE_ST_Adv_Heal_Soteria;
                return true;

            case 1:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Zoe))
                    return false;
                action = Zoe;
                config = SGE_ST_Adv_Heal_Zoe;
                return true;

            case 2:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Pepsis) ||
                    !HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget))
                    return false;
                action = Pepsis;
                config = SGE_ST_Adv_Heal_Pepsis;
                return true;

            case 3:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Taurochole) || !HasAddersgallAboveHold ||
                    !(tankCheck || !IsInParty() || !SGE_ST_Adv_Heal_Taurochole_TankOnly))
                    return false;
                action = Taurochole;
                config = SGE_ST_Adv_Heal_Taurochole;
                return true;

            case 4:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Haima) ||
                    SGE_ST_Adv_Heal_HaimaBossOption && InBossEncounter() ||
                    !(tankCheck || !IsInParty() || !SGE_ST_Adv_Heal_Haima_TankOnly))
                    return false;
                action = Haima;
                config = SGE_ST_Adv_Heal_Haima;
                return true;

            case 5:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Krasis) ||
                    SGE_ST_Adv_Heal_KrasisBossOption && InBossEncounter() ||
                    !(tankCheck || !IsInParty() || !SGE_ST_Adv_Heal_Krasis_TankOnly))
                    return false;
                action = Krasis;
                config = SGE_ST_Adv_Heal_Krasis;
                return true;

            case 6:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Druochole) || !HasAddersgallAboveHold)
                    return false;
                action = Druochole;
                config = SGE_ST_Adv_Heal_Druochole;
                return true;

            case 7:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_EDiagnosis) ||
                    GetTargetHPPercent(healTarget, SGE_ST_Adv_Heal_IncludeShields) > SGE_ST_Adv_Heal_EDiagnosisHP ||
                    !shieldCheck || !scholarShieldCheck)
                    return false;
                action = Eukrasia;
                config = SGE_ST_Adv_Heal_EDiagnosisHP;
                return true;

            case 8:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Kerachole) || !HasAddersgallAboveHold ||
                    SGE_ST_Adv_Heal_KeracholeBossOption && InBossEncounter())
                    return false;
                action = Kerachole;
                config = SGE_ST_Adv_Heal_KeracholeHP;
                return true;

            case 9:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Physis) ||
                    SGE_ST_Adv_Heal_PhysisBossOption && InBossEncounter())
                    return false;
                action = OriginalHook(Physis);
                config = SGE_ST_Adv_Heal_PhysisHP;
                return true;

            case 10:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Panhaima) ||
                    SGE_ST_Adv_Heal_PanhaimaBossOption && InBossEncounter())
                    return false;
                action = Panhaima;
                config = SGE_ST_Adv_Heal_PanhaimaHP;
                return true;

            case 11:
                if (!IsEnabled(Preset.SGE_ST_Adv_Heal_Holos) ||
                    SGE_ST_Adv_Heal_HolosBossOption && InBossEncounter())
                    return false;
                action = Holos;
                config = SGE_ST_Adv_Heal_HolosHP;
                return true;

            default:
                return false;
        }
    }

    private static bool TryAoEHealOption(int i, out uint action, out int config)
    {
        action = Prognosis;
        config = 0;

        bool shieldCheck = GetPartyBuffPercent(Buffs.EukrasianPrognosis) <= SGE_AoE_Adv_Heal_EPrognosisOption &&
                           GetPartyBuffPercent(SCH.Buffs.Galvanize) <= SGE_AoE_Adv_Heal_EPrognosisOption;

        bool anyPanhaima = !SGE_ST_Adv_Heal_PanhaimaOpts[0] ||
                           !HasStatusEffect(Buffs.Panhaima, null, true);

        switch (i)
        {
            case 0:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Kerachole) ||
                    SGE_AoE_Adv_Heal_KeracholeTrait && !TraitLevelChecked(Traits.EnhancedKerachole) ||
                    !HasAddersgallAboveHold)
                    return false;
                action = Kerachole;
                config = SGE_AoE_Adv_Heal_KeracholeOption;
                return true;

            case 1:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Ixochole) || !HasAddersgallAboveHold)
                    return false;
                action = Ixochole;
                config = SGE_AoE_Adv_Heal_IxocholeOption;
                return true;

            case 2:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Physis))
                    return false;
                action = OriginalHook(Physis);
                config = SGE_AoE_Adv_Heal_PhysisOption;
                return true;

            case 3:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Holos))
                    return false;
                action = Holos;
                config = SGE_AoE_Adv_Heal_HolosOption;
                return true;

            case 4:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Panhaima) || !anyPanhaima)
                    return false;
                action = Panhaima;
                config = SGE_AoE_Adv_Heal_PanhaimaOption;
                return true;

            case 5:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Pepsis) ||
                    !HasStatusEffect(Buffs.EukrasianPrognosis))
                    return false;
                action = Pepsis;
                config = SGE_AoE_Adv_Heal_PepsisOption;
                return true;

            case 6:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Philosophia))
                    return false;
                action = Philosophia;
                config = SGE_AoE_Adv_Heal_PhilosophiaOption;
                return true;

            case 7:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_Zoe))
                    return false;
                action = Zoe;
                config = SGE_AoE_Adv_Heal_ZoeOption;
                return true;

            case 8:
                if (!IsEnabled(Preset.SGE_AoE_Adv_Heal_EPrognosis) || !shieldCheck)
                    return false;
                action = Eukrasia;
                config = 100;
                return true;

            default:
                return false;
        }
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (ToxikonOpener.LevelChecked &&
            SGE_SelectedOpener == 0)
            return ToxikonOpener;

        if (PneumaOpener.LevelChecked &&
            SGE_SelectedOpener == 1)
            return PneumaOpener;

        return WrathOpener.Dummy;
    }

    internal static SGEToxikonOpener ToxikonOpener = new();
    internal static SGEPneumaOpener PneumaOpener = new();

    internal abstract class SGEOpenerBase : WrathOpener
    {
        public override int MinOpenerLevel => 92;
        public override int MaxOpenerLevel => 109;

        public override Preset Preset => Preset.SGE_ST_Adv_DPS_Opener;

        internal override UserData ContentCheckConfig => SGE_Balance_Content;
        internal override bool IncludePot => SGE_Opener_Potion;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => HasStatusEffect(Buffs.Eukrasia))
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([1], () => CountdownRemaining - 5),
            ([2], () => CountdownRemaining - 2),
            ([3], () => CountdownRemaining - 1)
        ];

        protected static bool SharedOpenerCooldowns() =>
            GetRemainingCharges(Phlegma3) is 2 &&
            IsOffCooldown(Psyche);
    }

    internal class SGEToxikonOpener : SGEOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Eukrasia,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Mind)),
            Toxikon2,
            EukrasianDosis3,
            Dosis3,
            Dosis3,
            Dosis3,
            Phlegma3,
            Psyche,
            Phlegma3,
            Dosis3,
            Dosis3,
            Dosis3,
            Dosis3,
            Eukrasia,
            EukrasianDosis3,
            Dosis3,
            Dosis3,
            Dosis3
        ];

        public override bool HasCooldowns() =>
            SharedOpenerCooldowns() &&
            HasAddersting;
    }

    internal class SGEPneumaOpener : SGEOpenerBase
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Eukrasia,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Mind)),
            Pneuma,
            EukrasianDosis3,
            Dosis3,
            Dosis3,
            Dosis3,
            Phlegma3,
            Psyche,
            Phlegma3,
            Dosis3,
            Dosis3,
            Dosis3,
            Dosis3,
            Eukrasia,
            EukrasianDosis3,
            Dosis3,
            Dosis3,
            Dosis3
        ];

        public override bool HasCooldowns() =>
            SharedOpenerCooldowns() &&
            IsOffCooldown(Pneuma);
    }

    #endregion

    #region ID's

    internal const uint

        // Heals and Shields
        Diagnosis = 24284,
        Prognosis = 24286,
        Physis = 24288,
        Druochole = 24296,
        Kerachole = 24298,
        Ixochole = 24299,
        Pepsis = 24301,
        Physis2 = 24302,
        Taurochole = 24303,
        Haima = 24305,
        Panhaima = 24311,
        Holos = 24310,
        EukrasianDiagnosis = 24291,
        EukrasianPrognosis = 24292,
        EukrasianPrognosis2 = 37034,
        Egeiro = 24287,

        // DPS
        Dosis = 24283,
        Dosis2 = 24306,
        Dosis3 = 24312,
        EukrasianDosis = 24293,
        EukrasianDosis2 = 24308,
        EukrasianDosis3 = 24314,
        Phlegma = 24289,
        Phlegma2 = 24307,
        Phlegma3 = 24313,
        Dyskrasia = 24297,
        Dyskrasia2 = 24315,
        Toxikon = 24304,
        Toxikon2 = 24316,
        Pneuma = 24318,
        EukrasianDyskrasia = 37032,
        Psyche = 37033,

        //Movement
        Icarus = 24295,

        // Buffs
        Soteria = 24294,
        Zoe = 24300,
        Krasis = 24317,
        Philosophia = 37035,

        // Other
        Kardia = 24285,
        Eukrasia = 24290,
        Rhizomata = 24309;

    internal static class Buffs
    {
        internal const ushort
            Kardia = 2604,
            Kardion = 2605,
            Eukrasia = 2606,
            EukrasianDiagnosis = 2607,
            EukrasianPrognosis = 2609,
            Haima = 2612,
            Panhaima = 2613,
            Kerachole = 2618,
            Zoe = 2611,
            Holosakos = 3365,
            Eudaimonia = 3899;
    }

    internal static class Debuffs
    {
        internal const ushort
            EukrasianDosis = 2614,
            EukrasianDosis2 = 2615,
            EukrasianDosis3 = 2616,
            EukrasianDyskrasia = 3897;
    }

    internal static class Traits
    {
        internal const ushort
            Addersgall = 370,
            Addersting = 373,
            EnhancedKerachole = 375,
            OffensiveMagicMasteryII = 376;
    }

    #endregion
}
