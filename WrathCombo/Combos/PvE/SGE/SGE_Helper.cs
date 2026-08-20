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

    private static bool UseKardia() =>
        LevelChecked(Kardia) &&
        !HasStatusEffect(Buffs.Kardia) &&
        Target is not null;

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

    private static bool UseAddersgallProtect(int threshold) =>
        ActionReady(Druochole) && Addersgall >= threshold;

    private static bool PhlegmaBurstPair(bool phlegmaEnabled, bool psycheEnabled, bool burst) =>
        LevelChecked(OriginalHook(Phlegma)) &&
        phlegmaEnabled &&
        psycheEnabled &&
        burst;

    private static bool UsePsyche(bool phlegmaBurstPair) =>
        ActionReady(Psyche) &&
        HasBattleTarget() &&
        InActionRange(Psyche) &&
        (!phlegmaBurstPair ||
         JustUsed(OriginalHook(Phlegma), 5f) ||
         !ActionReady(OriginalHook(Phlegma)) ||
         !InActionRange(OriginalHook(Phlegma)));

    private static bool UseLucid(int mpThreshold) =>
        Role.CanLucidDream(mpThreshold);

    private static bool UseRhizo(int threshold) =>
        ActionReady(Rhizomata) && Addersgall < threshold;

    private static bool UseSoteria() =>
        ActionReady(Soteria) && HasStatusEffect(Buffs.Kardia);

    private static bool UsePhysis() =>
        ActionReady(OriginalHook(Physis));

    private static bool UseKerachole(bool requireEnhanced) =>
        ActionReady(Kerachole) &&
        HasAddersgall &&
        (!requireEnhanced || TraitLevelChecked(Traits.EnhancedKerachole));

    private static bool UseHolos() =>
        ActionReady(Holos);

    private static bool UseIxochole() =>
        ActionReady(Ixochole) && HasAddersgall;

    private static bool UsePhilosophia() =>
        ActionReady(Philosophia) && !HasStatusEffect(Buffs.Panhaima);

    private static bool UsePanhaima() =>
        ActionReady(Panhaima) && !HasStatusEffect(Buffs.Eudaimonia);

    private static bool UseZoe() =>
        ActionReady(Zoe) && (ActionReady(Pneuma) || !LevelChecked(Pneuma));

    private static bool UseAoEPepsis() =>
        ActionReady(Pepsis) && HasStatusEffect(Buffs.EukrasianPrognosis);

    private static bool UsePhlegma(bool burst, int chargePool, bool psycheEnabled)
    {
        if (!InActionRange(OriginalHook(Phlegma)) ||
            !ActionReady(OriginalHook(Phlegma)))
            return false;

        if (IsPhlegmaCapped)
            return true;

        if (!burst && GetRemainingCharges(OriginalHook(Phlegma)) > chargePool)
            return true;

        if (!burst || !LevelChecked(Psyche) || !psycheEnabled)
            return false;

        if (JustUsed(Psyche, 5f))
            return true;

        if (IsOffCooldown(Psyche) && !JustUsed(OriginalHook(Phlegma), 5f))
            return true;

        return false;
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

        if (!PartyInCombat())
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

    private static bool UseEDyskrasia() =>
        HasEDyskrasiaTargets() &&
        !JustUsed(EukrasianDyskrasia) &&
        TraitLevelChecked(Traits.OffensiveMagicMasteryII) &&
        ActionReady(Eukrasia);

    private static bool UseAoEPhlegma(bool psycheEnabled)
    {
        if (!HasBattleTarget() ||
            !InActionRange(OriginalHook(Phlegma)) ||
            !ActionReady(OriginalHook(Phlegma)))
            return false;

        if (IsPhlegmaCapped)
            return true;

        if (!LevelChecked(Psyche) || !psycheEnabled)
            return true;

        if (JustUsed(Psyche, 5f))
            return true;

        if (IsOffCooldown(Psyche) && !JustUsed(OriginalHook(Phlegma), 5f))
            return true;

        return false;
    }

    private static bool UseAoEToxikon() =>
        ActionReady(OriginalHook(Toxikon)) &&
        HasBattleTarget() && HasAddersting &&
        InActionRange(OriginalHook(Toxikon));

    private static bool UseAoEPneuma(bool allowNonBoss) =>
        (allowNonBoss || TargetIsBoss()) &&
        ActionReady(Pneuma) && HasBattleTarget() &&
        InActionRange(Pneuma);

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

    private static bool UseEukrasianDiagnosis(IGameObject? healTarget, bool simpleMode, ref uint actionID)
    {
        if (!LevelChecked(Eukrasia) ||
            HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget))
            return false;

        if (!simpleMode)
        {
            if (!IsEnabled(Preset.SGE_ST_Adv_Heal_EDiagnosis))
                return false;

            bool shieldCheck = !SGE_ST_Adv_Heal_EDiagnosisOpts[0] ||
                               (!HasStatusEffect(Buffs.EukrasianDiagnosis, healTarget, true) &&
                                !HasStatusEffect(Buffs.EukrasianPrognosis, healTarget, true));
            bool scholarShieldCheck = !SGE_ST_Adv_Heal_EDiagnosisOpts[1] ||
                                      !HasStatusEffect(SCH.Buffs.Galvanize);
            if (!shieldCheck || !scholarShieldCheck)
                return false;

            if (GetTargetHPPercent(healTarget, SGE_ST_Adv_Heal_IncludeShields) >
                SGE_ST_Adv_Heal_EDiagnosisHP)
                return false;
        }

        if (HasStatusEffect(Buffs.Eukrasia) && ActionReady(EukrasianDiagnosis))
        {
            actionID = EukrasianDiagnosis.RetargetIfEnabled(actionID);
            return true;
        }

        if (!ActionReady(Eukrasia))
            return false;

        actionID = Eukrasia;
        return true;
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
