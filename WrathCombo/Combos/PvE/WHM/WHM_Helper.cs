#region
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.WHM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM
{
    internal static bool NeedsDoT()
    {
        var dotAction = OriginalHook(Aero);
        var hpThreshold = IsNotEnabled(Preset.WHM_ST_Simple_DPS)
            ? computeHpThreshold()
            : 0;
        AeroList.TryGetValue(dotAction, out var dotDebuffID);
        var dotRefresh = IsNotEnabled(Preset.WHM_ST_Simple_DPS)
            ? WHM_ST_MainCombo_DoT_Threshold
            : 2.5;
        var dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(dotAction) &&
               CanApplyStatus(CurrentTarget, dotDebuffID) &&
               !JustUsedOn(dotAction, CurrentTarget, 5f) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= dotRefresh;
    }

    internal static int computeHpThreshold()
    {
        if (TargetIsBoss() && InBossEncounter())
        {
            return WHM_ST_DPS_AeroOptionBoss;
        }

        switch ((int)WHM_ST_DPS_AeroOptionSubOption)
        {
            case (int)EnemyRestriction.AllEnemies:
                return WHM_ST_DPS_AeroOptionNonBoss;
            case (int)EnemyRestriction.OnlyBosses:
                return InBossEncounter() ? WHM_ST_DPS_AeroOptionNonBoss : 0;
            default:
            case (int)EnemyRestriction.NonBosses:
                return !InBossEncounter() ? WHM_ST_DPS_AeroOptionNonBoss : 0;
        }
    }

    #region Get ST Heals

    internal static int GetMatchingConfigST(int i, IGameObject? OptionalTarget,
        out uint action, out bool enabled)
    {
        IGameObject? healTarget = OptionalTarget ?? SimpleTarget.Stack.AllyToHeal;
        bool stopHot = WHM_STHeals_RegenHPLower >=
                       GetTargetHPPercent(healTarget,
                           WHM_STHeals_IncludeShields);
        float refreshTime = WHM_STHeals_RegenTimer;
        Status? regenHoT = GetStatusEffect(Buffs.Regen, healTarget);
        Status? BenisonShield = GetStatusEffect(Buffs.DivineBenison, healTarget);

        switch (i)
        {
            case 0:
                action = Benediction;
                enabled = IsEnabled(Preset.WHM_STHeals_Benediction) &&
                          (!WHM_STHeals_BenedictionWeave || CanWeave());
                return WHM_STHeals_BenedictionHP;
            case 1:
                action = Tetragrammaton;
                enabled = IsEnabled(Preset.WHM_STHeals_Tetragrammaton) &&
                          (!WHM_STHeals_TetraWeave || CanWeave());
                return WHM_STHeals_TetraHP;
            case 2:
                action = DivineBenison;
                enabled = IsEnabled(Preset.WHM_STHeals_Benison) &&
                          BenisonShield == null &&
                          GetRemainingCharges(DivineBenison) >
                          WHM_STHeals_BenisonCharges &&
                          (!WHM_STHeals_BenisonWeave || CanWeave());
                return WHM_STHeals_BenisonHP;
            case 3:
                action = Aquaveil;
                enabled = IsEnabled(Preset.WHM_STHeals_Aquaveil) &&
                          (!WHM_STHeals_AquaveilOptions[1] || !InBossEncounter()) &&
                          (!WHM_STHeals_AquaveilOptions[0] || CanWeave());
                return WHM_STHeals_AquaveilHP;
            case 4:
                action = AfflatusSolace;
                enabled = IsEnabled(Preset.WHM_STHeals_Solace) &&
                          CanLily;
                return WHM_STHeals_SolaceHP;
            case 5:
                action = Regen;
                enabled = IsEnabled(Preset.WHM_STHeals_Regen) &&
                          !stopHot &&
                          (regenHoT is null ||
                           regenHoT.RemainingTime <= refreshTime);
                return WHM_STHeals_RegenHPUpper;

            case 6:
                action = OriginalHook(Temperance);
                enabled = IsEnabled(Preset.WHM_STHeals_Temperance) &&
                          (!WHM_STHeals_TemperanceOptions[1] ||
                           !InBossEncounter()) &&
                          (!WHM_STHeals_TemperanceOptions[0] || CanWeave());
                return WHM_STHeals_TemperanceHP;

            case 7:
                action = Asylum;
                enabled = IsEnabled(Preset.WHM_STHeals_Asylum) &&
                          (!WHM_STHeals_AsylumOptions[1] ||
                           !InBossEncounter()) &&
                          (!WHM_STHeals_AsylumOptions[0] || CanWeave());
                return WHM_STHeals_AsylumHP;
            case 8:
                action = LiturgyOfTheBell;
                enabled =
                    IsEnabled(Preset.WHM_STHeals_LiturgyOfTheBell) &&
                    !HasStatusEffect(Buffs.LiturgyOfTheBell) &&
                    (!WHM_STHeals_LiturgyOfTheBellOptions[1] ||
                     !InBossEncounter()) &&
                    (!WHM_STHeals_LiturgyOfTheBellOptions[0] || CanWeave());
                return WHM_STHeals_LiturgyOfTheBellHP;
        }

        enabled = false;
        action = 0;
        return 0;
    }

    #endregion

    #region Get Aoe Heals

    public static int GetMatchingConfigAoE(int i, IGameObject? OptionalTarget,
        out uint action, out bool enabled)
    {
        var medica3Check = !HasStatusEffect(Buffs.Medica3) ||
                           GetStatusEffectRemainingTime(Buffs.Medica3) <=
                           WHM_AoEHeals_MedicaTime;
        var medica2Check = !HasStatusEffect(Buffs.Medica2) ||
                           GetStatusEffectRemainingTime(Buffs.Medica2) <=
                           WHM_AoEHeals_MedicaTime;

        switch (i)
        {
            case 0:
                action = OriginalHook(Medica2);
                enabled = IsEnabled(Preset.WHM_AoEHeals_Medica2) &&
                          (LevelChecked(Medica3) && medica3Check ||
                           !LevelChecked(Medica3) && medica2Check);
                return WHM_AoEHeals_Medica2HP;

            case 1:
                action = Cure3;
                enabled = IsEnabled(Preset.WHM_AoEHeals_Cure3) &&
                          NumberOfAlliesInRange(Cure3, OptionalTarget)
                          >= WHM_AoEHeals_Cure3Allies &&
                          (LocalPlayer.CurrentMp >= WHM_AoEHeals_Cure3MP ||
                           HasStatusEffect(Buffs.ThinAir));
                return WHM_AoEHeals_Cure3HP;

            case 2:
                action = PlenaryIndulgence;
                enabled = IsEnabled(Preset.WHM_AoEHeals_Plenary) &&
                          (CanWeave() || !WHM_AoEHeals_PlenaryWeave);
                return WHM_AoEHeals_PlenaryHP;

            case 3:
                action = Temperance;
                enabled = IsEnabled(Preset.WHM_AoEHeals_Temperance) &&
                          (CanWeave() || !WHM_AoEHeals_TemperanceWeave) &&
                          !HasStatusEffect(Buffs.DivineGrace) &&
                          ContentCheck.IsInConfiguredContent(
                              WHM_AoEHeals_TemperanceDifficulty,
                              WHM_AoEHeals_TemperanceDifficultyListSet);
                return WHM_AoEHeals_TemperanceHP;

            case 4:
                action = Asylum;
                enabled = IsEnabled(Preset.WHM_AoEHeals_Asylum) &&
                          (CanWeave() || !WHM_AoEHeals_AsylumWeave) &&
                          !IsMoving() &&
                          ContentCheck.IsInConfiguredContent(
                              WHM_AoEHeals_AsylumDifficulty,
                              WHM_AoEHeals_AsylumDifficultyListSet);
                return WHM_AoEHeals_AsylumHP;

            case 5:
                action = LiturgyOfTheBell;
                enabled =
                    IsEnabled(Preset.WHM_AoEHeals_LiturgyOfTheBell) &&
                    !HasStatusEffect(Buffs.LiturgyOfTheBell) &&
                    (CanWeave() || !WHM_AoEHeals_LiturgyWeave) &&
                    ContentCheck.IsInConfiguredContent(
                        WHM_AoEHeals_LiturgyDifficulty,
                        WHM_AoEHeals_LiturgyDifficultyListSet);
                return WHM_AoEHeals_LiturgyHP;

            case 6:
                action = AfflatusRapture;
                enabled = IsEnabled(Preset.WHM_AoEHeals_Rapture) &&
                          CanLily;
                return WHM_AoEHeals_RaptureHP;

            case 7:
                action = Assize;
                enabled = IsEnabled(Preset.WHM_AoEHeals_Assize) &&
                          (!WHM_AoEHeals_AssizeWeave || CanWeave());
                return WHM_AoEHeals_AssizeHP;

            case 8:
                action = DivineCaress;
                enabled = IsEnabled(Preset.WHM_AoEHeals_DivineCaress) &&
                          (!WHM_AoEHeals_DivineCaressWeave || CanWeave());
                return WHM_AoEHeals_DivineCaressHP;
        }

        enabled = false;
        action = 0;
        return 0;
    }

    #endregion

    #region Raidwides

    internal static bool RaidwideAsylum()
    {
        return IsEnabled(Preset.WHM_Raidwide_Asylum) &&
               ActionReady(Asylum) &&
               CanWeave() && RaidWideCasting();
    }

    internal static bool RaidwideTemperance()
    {
        return IsEnabled(Preset.WHM_Raidwide_Temperance) &&
               ActionReady(OriginalHook(Temperance)) &&
               CanWeave() && RaidWideCasting();
    }

    internal static bool RaidwideLiturgyOfTheBell()
    {
        return IsEnabled(Preset.WHM_Raidwide_LiturgyOfTheBell) &&
               ActionReady(LiturgyOfTheBell) &&
               !HasStatusEffect(Buffs.LiturgyOfTheBell) &&
               RaidWideCasting() && CanWeave();
    }

    #endregion

    #region Variables

    //Lists
    internal static readonly List<uint>
        StoneGlareList = [Stone1, Stone2, Stone3, Stone4, Glare1, Glare3];

    internal static readonly Dictionary<uint, ushort>
        AeroList = new()
        {
            { Aero, Debuffs.Aero },
            { Aero2, Debuffs.Aero2 },
            { Dia, Debuffs.Dia },
        };

    // Gauge Stuff
    internal static WHMGauge gauge = GetJobGauge<WHMGauge>();
    internal static bool CanLily => gauge.Lily > 0;
    internal static bool FullLily => gauge.Lily == 3;
    internal static bool AlmostFullLily => gauge is { Lily: 2, LilyTimer: >= 8000 };
    internal static bool BloodLilyReady => gauge.BloodLily == 3;

    #endregion

    #region Opener

    internal static WHMOpenerMaxLevel1 Opener1 = new();

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class WHMOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 92;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Glare3,
            Dia,
            Glare3,
            Glare3,
            PresenceOfMind,
            Glare4,
            Assize,
            Glare4,
            Glare4,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Dia,
        ];

        internal override UserData ContentCheckConfig => WHM_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(PresenceOfMind))
                return false;

            if (!IsOffCooldown(Assize))
                return false;

            return true;
        }
    }

    #endregion

    #region ID's

    public const uint
        // DPS
        Glare1 = 16533,
        Glare3 = 25859,
        Glare4 = 37009,
        Stone1 = 119,
        Stone2 = 127,
        Stone3 = 3568,
        Stone4 = 7431,
        Assize = 3571,
        Holy = 139,
        Holy3 = 25860,

        // DoT
        Aero = 121,
        Aero2 = 132,
        Dia = 16532,

        // Heals
        Cure = 120,
        Cure2 = 135,
        Cure3 = 131,
        Regen = 137,
        AfflatusSolace = 16531,
        AfflatusRapture = 16534,
        Raise = 125,
        Benediction = 140,
        AfflatusMisery = 16535,
        Medica1 = 124,
        Medica2 = 133,
        Medica3 = 37010,
        Tetragrammaton = 3570,
        DivineBenison = 7432,
        Aquaveil = 25861,
        DivineCaress = 37011,
        Asylum = 3569,
        LiturgyOfTheBell = 25862,
        LiturgyOfTheBellRecast = 28509,

        // Buffs
        ThinAir = 7430,
        PresenceOfMind = 136,
        PlenaryIndulgence = 7433,
        Temperance = 16536;


    public static class Buffs
    {
        public const ushort
            Regen = 158,
            Medica2 = 150,
            Medica3 = 3880,
            PresenceOfMind = 157,
            ThinAir = 1217,
            DivineBenison = 1218,
            Aquaveil = 2708,
            SacredSight = 3879,
            LiturgyOfTheBell = 2709,
            DivineGrace = 3881,
            Temperance = 1872;
    }

    public static class Debuffs
    {
        public const ushort
            Aero = 143,
            Aero2 = 144,
            Dia = 1871;
    }

    #endregion
}