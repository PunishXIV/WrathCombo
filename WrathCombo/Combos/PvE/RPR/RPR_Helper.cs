using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.RPR.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class RPR
{
    #region Misc

    //Auto Arcane Crest
    private static bool CanUseArcaneCrest =>
        ActionReady(ArcaneCrest) &&
        (RaidWideCasting(3f) || !IsInParty());

    #endregion

    #region Enshroud

    private static bool CanEnshroud()
    {
        if (LevelChecked(Enshroud) && (Shroud >= 50 || HasStatusEffect(Buffs.IdealHost)) &&
            !HasStatusEffect(Buffs.SoulReaver) && !HasStatusEffect(Buffs.Executioner) && HasBattleTarget() &&
            !HasStatusEffect(Buffs.PerfectioParata) && !HasStatusEffect(Buffs.Enshrouded))
        {
            // Before Plentiful Harvest 
            if (!LevelChecked(PlentifulHarvest))
                return true;

            // Shroud in Arcane Circle 
            if (HasStatusEffect(Buffs.ArcaneCircle))
                return true;

            // Prep for double Enshroud
            if (LevelChecked(PlentifulHarvest) &&
                GetCooldownRemainingTime(ArcaneCircle) <= GCD + 1.5f)
                return true;

            //2nd part of Double Enshroud
            if (LevelChecked(PlentifulHarvest) &&
                JustUsed(PlentifulHarvest, 5))
                return true;

            //Natural Odd Minute Shrouds
            if (!HasStatusEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                GetCooldownRemainingTime(ArcaneCircle).InRange(49, 66))
                return true;

            // Correction for 2 min windows 
            if (!HasStatusEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                Soul >= 90)
                return true;
        }

        return false;
    }

    #endregion

    #region SoD

    private static bool CanUseShadowOfDeath(bool simpleMode = false)
    {
        if (LevelChecked(ShadowOfDeath) && !HasStatusEffect(Buffs.SoulReaver) &&
            !HasStatusEffect(Buffs.Executioner) && !HasStatusEffect(Buffs.PerfectioParata) &&
            !HasStatusEffect(Buffs.ImmortalSacrifice) && !IsComboExpiring(3) &&
            CanApplyStatus(CurrentTarget, Debuffs.DeathsDesign) &&
            !JustUsed(ShadowOfDeath))
        {
            switch (simpleMode)
            {
                case true when !InBossEncounter() && LevelChecked(PlentifulHarvest) && !HasStatusEffect(Buffs.Enshrouded) &&
                               GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= 8:
                    return true;
                case true:
                {
                    if (InBossEncounter())
                    {
                        //Double enshroud
                        if (LevelChecked(PlentifulHarvest) && HasStatusEffect(Buffs.Enshrouded) &&
                            (GetCooldownRemainingTime(ArcaneCircle) <= GCD || IsOffCooldown(ArcaneCircle)) &&
                            (JustUsed(VoidReaping, 2f) || JustUsed(CrossReaping, 2f)))
                            return true;

                        //lvl 88+ general use
                        if (LevelChecked(PlentifulHarvest) && !HasStatusEffect(Buffs.Enshrouded) &&
                            GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= 8 &&
                            (GetCooldownRemainingTime(ArcaneCircle) > GCD * 8 || IsOffCooldown(ArcaneCircle)))
                            return true;

                        //below lvl 88 use
                        if (!LevelChecked(PlentifulHarvest) &&
                            GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= 8)
                            return true;
                    }
                    break;
                }

                case false when RPR_ST_ArcaneCircleBossOption == 1 && !InBossEncounter() &&
                                !HasStatusEffect(Buffs.Enshrouded) &&
                                GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= RPR_SoDRefreshRange:
                    return true;

                case false:
                {
                    if (RPR_ST_ArcaneCircleBossOption == 0 || InBossEncounter() ||
                        IsNotEnabled(Preset.RPR_ST_ArcaneCircle))
                    {
                        //Double enshroud
                        if (LevelChecked(PlentifulHarvest) && HasStatusEffect(Buffs.Enshrouded) &&
                            (GetCooldownRemainingTime(ArcaneCircle) <= GCD || IsOffCooldown(ArcaneCircle)) &&
                            (JustUsed(VoidReaping, 2f) || JustUsed(CrossReaping, 2f)))
                            return true;

                        //lvl 88+ general use
                        if (LevelChecked(PlentifulHarvest) && !HasStatusEffect(Buffs.Enshrouded) &&
                            GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= RPR_SoDRefreshRange &&
                            (GetCooldownRemainingTime(ArcaneCircle) > GCD * 8 || IsOffCooldown(ArcaneCircle)))
                            return true;

                        //below lvl 88 use
                        if (!LevelChecked(PlentifulHarvest) &&
                            GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) <= RPR_SoDRefreshRange)
                            return true;
                    }
                    break;
                }
            }
        }

        return false;
    }

    #endregion

    #region Combos

    private static float GCD => GetCooldown(Slice).CooldownTotal;

    private static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GCD * times;

        return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < gcd;
    }

    private static bool IsDebuffExpiring(float times)
    {
        float gcd = GCD * times;

        return HasStatusEffect(Debuffs.DeathsDesign, CurrentTarget) && GetStatusEffectRemainingTime(Debuffs.DeathsDesign, CurrentTarget) < gcd;
    }

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (StandardOpenerLvl100.LevelChecked)
            return StandardOpenerLvl100;

        if (StandardOpenerLvl90.LevelChecked)
            return StandardOpenerLvl90;

        return WrathOpener.Dummy;
    }

    internal static RPRStandardOpenerLvl100 StandardOpenerLvl100 = new();

    internal static RPRStandardOpenerLvl90 StandardOpenerLvl90 = new();

    internal class RPRStandardOpenerLvl100 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Harpe,
            ShadowOfDeath,
            SoulSlice,
            ArcaneCircle,
            Gluttony,
            ExecutionersGibbet, //6
            ExecutionersGallows, //7
            SoulSlice,
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            Sacrificium,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            Perfectio,
            UnveiledGibbet, //20
            Gibbet, //21
            ShadowOfDeath,
            Slice
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => RPR_Opener_StartChoice == 1)
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([6], ExecutionersGallows, OnTargetsRear),
            ([7], ExecutionersGibbet, () => HasStatusEffect(Buffs.EnhancedGibbet)),
            ([20], UnveiledGallows, () => HasStatusEffect(Buffs.EnhancedGallows)),
            ([21], Gallows, () => HasStatusEffect(Buffs.EnhancedGallows))
        ];

        internal override UserData ContentCheckConfig => RPR_Balance_Content;

        public override bool HasCooldowns() =>
            GetRemainingCharges(SoulSlice) is 2 &&
            IsOffCooldown(ArcaneCircle) &&
            IsOffCooldown(Gluttony);
    }

    internal class RPRStandardOpenerLvl90 : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            Harpe,
            ShadowOfDeath,
            ArcaneCircle,
            SoulSlice,
            SoulSlice,
            PlentifulHarvest,
            Enshroud,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            VoidReaping,
            CrossReaping,
            LemuresSlice,
            Communio,
            HarvestMoon,
            Gluttony,
            Gibbet, //16
            Gallows, //17
            UnveiledGibbet, //18
            Gibbet //19
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => RPR_Opener_StartChoice == 1)
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([16], Gallows, OnTargetsRear),
            ([17], Gibbet, () => HasStatusEffect(Buffs.EnhancedGibbet)),
            ([18], UnveiledGallows, () => HasStatusEffect(Buffs.EnhancedGallows)),
            ([19], Gallows, () => HasStatusEffect(Buffs.EnhancedGallows))
        ];

        internal override UserData ContentCheckConfig => RPR_Balance_Content;

        public override bool HasCooldowns() =>
            GetRemainingCharges(SoulSlice) is 2 &&
            IsOffCooldown(ArcaneCircle) &&
            IsOffCooldown(Gluttony);
    }

    #endregion

    #region Gauge

    private static RPRGauge Gauge => GetJobGauge<RPRGauge>();

    private static byte Shroud => Gauge.Shroud;

    private static byte Soul => Gauge.Soul;

    private static byte Lemure => Gauge.LemureShroud;

    private static byte Void => Gauge.VoidShroud;

    #endregion

    #region ID's

    public const uint

        // Single Target
        Slice = 24373,
        WaxingSlice = 24374,
        InfernalSlice = 24375,
        ShadowOfDeath = 24378,
        SoulSlice = 24380,

        // AoE
        SpinningScythe = 24376,
        NightmareScythe = 24377,
        WhorlOfDeath = 24379,
        SoulScythe = 24381,

        // Unveiled
        Gibbet = 24382,
        Gallows = 24383,
        Guillotine = 24384,
        UnveiledGibbet = 24390,
        UnveiledGallows = 24391,
        ExecutionersGibbet = 36970,
        ExecutionersGallows = 36971,
        ExecutionersGuillotine = 36972,

        // Reaver
        BloodStalk = 24389,
        GrimSwathe = 24392,
        Gluttony = 24393,

        // Sacrifice
        ArcaneCircle = 24405,
        PlentifulHarvest = 24385,

        // Enshroud
        Enshroud = 24394,
        Communio = 24398,
        LemuresSlice = 24399,
        LemuresScythe = 24400,
        VoidReaping = 24395,
        CrossReaping = 24396,
        GrimReaping = 24397,
        Sacrificium = 36969,
        Perfectio = 36973,

        // Miscellaneous
        HellsIngress = 24401,
        HellsEgress = 24402,
        Regress = 24403,
        ArcaneCrest = 24404,
        Harpe = 24386,
        Soulsow = 24387,
        HarvestMoon = 24388;

    public static class Buffs
    {
        public const ushort
            SoulReaver = 2587,
            ImmortalSacrifice = 2592,
            ArcaneCircle = 2599,
            EnhancedGibbet = 2588,
            EnhancedGallows = 2589,
            EnhancedVoidReaping = 2590,
            EnhancedCrossReaping = 2591,
            EnhancedHarpe = 2845,
            Enshrouded = 2593,
            Soulsow = 2594,
            Threshold = 2595,
            BloodsownCircle = 2972,
            IdealHost = 3905,
            Oblatio = 3857,
            Executioner = 3858,
            PerfectioParata = 3860;
    }

    public static class Debuffs
    {
        public const ushort
            DeathsDesign = 2586;
    }

    #endregion
}
