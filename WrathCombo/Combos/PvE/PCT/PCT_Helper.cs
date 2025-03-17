using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    #region ID's

    public const byte JobID = 42;

    public const uint
        BlizzardinCyan = 34653,
        StoneinYellow = 34654,
        BlizzardIIinCyan = 34659,
        ClawMotif = 34666,
        ClawedMuse = 34672,
        CometinBlack = 34663,
        CreatureMotif = 34689,
        FireInRed = 34650,
        AeroInGreen = 34651,
        WaterInBlue = 34652,
        FireIIinRed = 34656,
        AeroIIinGreen = 34657,
        HammerMotif = 34668,
        WingedMuse = 34671,
        StrikingMuse = 34674,
        StarryMuse = 34675,
        HammerStamp = 34678,
        HammerBrush = 34679,
        PolishingHammer = 34680,
        HolyInWhite = 34662,
        StarrySkyMotif = 34669,
        LandscapeMotif = 34691,
        LivingMuse = 35347,
        MawMotif = 34667,
        MogoftheAges = 34676,
        PomMotif = 34664,
        PomMuse = 34670,
        RainbowDrip = 34688,
        RetributionoftheMadeen = 34677,
        ScenicMuse = 35349,
        Smudge = 34684,
        StarPrism = 34681,
        SteelMuse = 35348,
        SubtractivePalette = 34683,
        StoneIIinYellow = 34660,
        ThunderIIinMagenta = 34661,
        ThunderinMagenta = 34655,
        WaterinBlue = 34652,
        WeaponMotif = 34690,
        WingMotif = 34665;

    public static class Buffs
    {
        public const ushort
            SubtractivePalette = 3674,
            RainbowBright = 3679,
            HammerTime = 3680,
            MonochromeTones = 3691,
            StarryMuse = 3685,
            Hyperphantasia = 3688,
            Inspiration = 3689,
            SubtractiveSpectrum = 3690,
            Starstruck = 3681;
    }

    public static class Debuffs
    {

    }

    #endregion

    internal static PCTopenerMaxLevel1 Opener1 = new();
    internal static PCTopenerMaxLevel2 Opener2 = new();

    private static PCTGauge Gauge = GetJobGauge<PCTGauge>();

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked && Config.PCT_Opener_Choice == 0)
            return Opener1;

        if (Opener2.LevelChecked && Config.PCT_Opener_Choice == 1)
            return Opener2;

        return WrathOpener.Dummy;
    }

    public static bool HasMotifs()
    {

        if (!Gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Pom))
            return false;

        if (!Gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Weapon))
            return false;

        if (!Gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Landscape))
            return false;

        return true;
    }

    internal class PCTopenerMaxLevel1 : WrathOpener
    {
        //2nd GCD Starry Opener
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;
        public override List<uint> OpenerActions { get; set; } =
        [
            RainbowDrip,
            PomMuse,
            StrikingMuse,
            WingMotif,
            StarryMuse,
            HammerStamp,
            SubtractivePalette,
            BlizzardinCyan,
            StoneinYellow,
            ThunderinMagenta,
            CometinBlack,
            WingedMuse,
            MogoftheAges,
            StarPrism,
            HammerBrush,
            PolishingHammer,
            RainbowDrip,
            All.Swiftcast,
            ClawMotif,
            ClawedMuse,
        ];
        internal override UserData? ContentCheckConfig => Config.PCT_Balance_Content;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
[
            ([8, 9, 10], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
            ([8, 9, 10], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([8, 9, 10], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([11], HolyInWhite, () => !HasEffect(Buffs.MonochromeTones)),
        ];

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(StarryMuse))
                return false;

            if (GetRemainingCharges(LivingMuse) < 3)
                return false;

            if (GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!HasMotifs())
                return false;

            if (HasEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(All.Swiftcast))
                return false;

            return true;
        }
    }

    internal class PCTopenerMaxLevel2 : WrathOpener
    {
        //3rd GCD Starry Opener
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;
        public override List<uint> OpenerActions { get; set; } =
        [
            RainbowDrip,
            StrikingMuse,
            HolyInWhite,
            PomMuse,
            WingMotif,
            StarryMuse,
            HammerStamp,
            SubtractivePalette,
            BlizzardinCyan,
            BlizzardinCyan,
            BlizzardinCyan,
            CometinBlack,
            WingedMuse,
            MogoftheAges,
            StarPrism,
            HammerBrush,
            PolishingHammer,
            FireInRed,
            All.Swiftcast,
            ClawMotif,
            ClawedMuse
        ];
        internal override UserData? ContentCheckConfig => Config.PCT_Balance_Content;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([3], CometinBlack, () => HasEffect(Buffs.MonochromeTones)),
            ([9, 10, 11], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
             ([9, 10, 11], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([9, 10, 11], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([12], HolyInWhite, () => !HasEffect(Buffs.MonochromeTones)),
        ];

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(StarryMuse))
                return false;

            if (GetRemainingCharges(LivingMuse) < 3)
                return false;

            if (GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!HasMotifs())
                return false;

            if (HasEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(All.Swiftcast))
                return false;

            return true;
        }
    }
}