#region

using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.CustomComboNS;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class RPR
{
    // RPR Gauge & Extensions
    internal static RPROpenerMaxLevel1 Opener1 = new();

    internal static RPRGauge Gauge = GetJobGauge<RPRGauge>();
    
    internal static float GCD => GetCooldown(Slice).CooldownTotal;

    internal static bool trueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class RPROpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            ShadowOfDeath,
            SoulSlice,
            ArcaneCircle,
            Gluttony,
            ExecutionersGibbet,
            ExecutionersGallows,
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
            UnveiledGibbet,
            Gibbet,
            ShadowOfDeath,
            Slice
        ];

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(SoulSlice) < 2)
                return false;

            if (!ActionReady(ArcaneCircle))
                return false;

            if (!ActionReady(Gluttony))
                return false;

            return true;
        }
    }

    internal static class RPRHelper
    {
        public static unsafe bool IsComboExpiring(float Times)
        {
            float GCD = GetCooldown(Slice).CooldownTotal * Times;

            return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < GCD;
        }

        public static bool IsDebuffExpiring(float Times)
        {
            float GCD = GetCooldown(Slice).CooldownTotal * Times;

            return TargetHasEffect(Debuffs.DeathsDesign) && GetDebuffRemainingTime(Debuffs.DeathsDesign) < GCD;
        }

        public static bool UseEnshroud(RPRGauge gauge)
        {
            if (LevelChecked(Enshroud) && (gauge.Shroud >= 50 || HasEffect(Buffs.IdealHost)) &&
                !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner) &&
                !HasEffect(Buffs.PerfectioParata) && !HasEffect(Buffs.Enshrouded))
            {
                // Before Plentiful Harvest 
                if (!LevelChecked(PlentifulHarvest))
                    return true;

                // Shroud in Arcane Circle 
                if (HasEffect(Buffs.ArcaneCircle))
                    return true;

                // Prep for double Enshroud
                if (LevelChecked(PlentifulHarvest) &&
                    GetCooldownRemainingTime(ArcaneCircle) <= GCD * 2 + 1.5)
                    return true;

                //2nd part of Double Enshroud
                if (LevelChecked(PlentifulHarvest) &&
                    JustUsed(PlentifulHarvest, 5))
                    return true;

                //Natural Odd Minute Shrouds
                if (!HasEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                    GetCooldownRemainingTime(ArcaneCircle) is >= 50 and <= 65)
                    return true;

                // Correction for 2 min windows 
                if (!HasEffect(Buffs.ArcaneCircle) && !IsDebuffExpiring(5) &&
                    gauge.Soul >= 90)
                    return true;
            }

            return false;
        }

        public static bool UseShadowOfDeath()
        {
            if (LevelChecked(ShadowOfDeath) && !HasEffect(Buffs.SoulReaver) &&
                !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.PerfectioParata) &&
                !HasEffect(Buffs.ImmortalSacrifice) && !IsComboExpiring(3) &&
                !JustUsed(ShadowOfDeath))
            {
                //1st part double enshroud
                if (LevelChecked(PlentifulHarvest) && HasEffect(Buffs.Enshrouded) &&
                    GetCooldownRemainingTime(ArcaneCircle) <= GCD * 2 + 1.5 && JustUsed(Enshroud))
                    return true;

                //2nd part double enshroud
                if (LevelChecked(PlentifulHarvest) && HasEffect(Buffs.Enshrouded) &&
                    (GetCooldownRemainingTime(ArcaneCircle) <= GCD || IsOffCooldown(ArcaneCircle)) &&
                    (JustUsed(VoidReaping) || JustUsed(CrossReaping)))
                    return true;

                //lvl 88+ general use
                if (LevelChecked(PlentifulHarvest) && !HasEffect(Buffs.Enshrouded) &&
                    ((IsEnabled(CustomComboPreset.RPR_ST_SimpleMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= 8) ||
                     (IsEnabled(CustomComboPreset.RPR_ST_AdvancedMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= Config.RPR_SoDRefreshRange)) &&
                    (GetCooldownRemainingTime(ArcaneCircle) > GCD * 8 || IsOffCooldown(ArcaneCircle)))
                    return true;

                //below lvl 88 use
                if (!LevelChecked(PlentifulHarvest) &&
                    ((IsEnabled(CustomComboPreset.RPR_ST_SimpleMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= 8) ||
                     (IsEnabled(CustomComboPreset.RPR_ST_AdvancedMode) &&
                      GetDebuffRemainingTime(Debuffs.DeathsDesign) <= Config.RPR_SoDRefreshRange)))
                    return true;
            }

            return false;
        }
    }
}