#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using WrathCombo.Extensions;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    #region Variables

    // Gauge Stuff
    internal static PCTGauge gauge = GetJobGauge<PCTGauge>();


    //Useful Bools
    internal static bool ScenicMuseReady => gauge.LandscapeMotifDrawn && ActionReady(ScenicMuse);
    internal static bool LivingMuseReady => ActionReady(LivingMuse) && gauge.CreatureMotifDrawn;
    internal static bool SteelMuseReady => ActionReady(SteelMuse) && !HasStatusEffect(Buffs.HammerTime) && gauge.WeaponMotifDrawn;
    internal static bool PortraitReady => ActionReady(MogoftheAges) && (gauge.MooglePortraitReady || gauge.MadeenPortraitReady);
    internal static bool CreatureMotifReady => !gauge.CreatureMotifDrawn && LevelChecked(CreatureMotif) && !HasStatusEffect(Buffs.StarryMuse);
    internal static bool WeaponMotifReady => !gauge.WeaponMotifDrawn && LevelChecked(WeaponMotif) && !HasStatusEffect(Buffs.StarryMuse) && !HasStatusEffect(Buffs.HammerTime);
    internal static bool LandscapeMotifReady => !gauge.LandscapeMotifDrawn && LevelChecked(LandscapeMotif) && !HasStatusEffect(Buffs.StarryMuse);
    internal static bool PaletteReady => SubtractivePalette.LevelChecked() && !HasStatusEffect(Buffs.SubtractivePalette) && !HasStatusEffect(Buffs.MonochromeTones) && 
                                            (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50 && ScenicCD > 40 || gauge.PalleteGauge == 100);
    internal static bool HasPaint => gauge.Paint > 0;
    internal static bool BurstPhaseReady => LevelChecked(StarPrism) && InCombat() && (ScenicCD <= 5 || HasStatusEffect(Buffs.StarryMuse) && gauge.PalleteGauge >= 50);



    //Buff Tracking
    internal static float ScenicCD => GetCooldownRemainingTime(StarryMuse);
    internal static float SteelCD => GetCooldownRemainingTime(StrikingMuse);


    #endregion

    #region Functions
    internal static bool HyperPhantasiaMovementPaint()
    //Increase priority for using casts as soon as possible to avoid losing DPS and ensure all abilities fit within buff windows
    //previously, there were situations where Wrath prioritized using Hammer Combo over casts during hyperphantasia, which would prevent us from generating Rainbow Bright in time when movement is required
    //so, if we have Hyperphantasia stacks and Inspiration is active from standing in PCT LeyLines, we burn it all down
    {
        if (GetStatusEffectStacks(Buffs.Hyperphantasia) > 0 && HasStatusEffect(Buffs.Inspiration) && HasPaint)
        {
            if ((IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_HolyInWhite) || IsEnabled(CustomComboPreset.PCT_ST_SimpleMode)) 
                && HolyInWhite.LevelChecked())
                return true;

            if ((IsEnabled(CustomComboPreset.PCT_ST_AdvancedMode_MovementOption_CometinBlack) || IsEnabled(CustomComboPreset.PCT_ST_SimpleMode)) 
                && CometinBlack.LevelChecked())
                return true;
        }
        return false;
    }


    internal static uint BurstWindow(uint actionId)
    {
        if (LandscapeMotifReady) //Emergency Landscape Paint if someone started a fight with no motifs.
            return OriginalHook(LandscapeMotif);        

        if (!HasStatusEffect(Buffs.StarryMuse))
        {
            if (SteelMuseReady && HasCharges(SteelMuse))
                return OriginalHook(SteelMuse);

            if (ActionReady(HammerStamp) && !HasStatusEffect(Buffs.StarryMuse) && ScenicCD < 1 && !JustUsed(HammerStamp, 3f) && GetStatusEffectStacks(Buffs.HammerTime) == 3)
                return HammerStamp;

            if (CanWeave() && ActionReady(SubtractivePalette) && !HasStatusEffect(Buffs.SubtractivePalette) && 
                !HasStatusEffect(Buffs.MonochromeTones) && IsOffCooldown(ScenicMuse) &&
                (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50))
                return SubtractivePalette;

            if (ScenicMuseReady && (CanDelayedWeave() || !CanWeave()) && IsOffCooldown(ScenicMuse)) // The !canweave option is specifically so that it can minimize drift IF it does not catch the delayed weave in time.
                return OriginalHook(ScenicMuse);            
        }

        if (HyperPhantasiaMovementPaint() && IsMoving())
            return HasStatusEffect(Buffs.MonochromeTones) ? OriginalHook(CometinBlack) : OriginalHook(HolyInWhite);

        if (HasStatusEffect(Buffs.SubtractivePalette) && (GetStatusEffectRemainingTime(Buffs.StarryMuse) > 10 || !HasStatusEffect(Buffs.StarryMuse)))
            return OriginalHook(BlizzardinCyan);

        if (HasStatusEffect(Buffs.StarryMuse) && GetStatusEffectRemainingTime(Buffs.StarryMuse) < 15)
        {
            if (CanSpellWeave() && !HasStatusEffect(Buffs.MonochromeTones))
            {                
                if (ActionReady(SubtractivePalette) && !HasStatusEffect(Buffs.Starstruck) &&
                    !HasStatusEffect(Buffs.SubtractivePalette) && (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50))
                    return SubtractivePalette;
               
                if (PortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)))
                    return OriginalHook(MogoftheAges);

                if (LivingMuseReady && !PortraitReady)
                    return OriginalHook(LivingMuse);
            }

            if (LevelChecked(CometinBlack) && HasStatusEffect(Buffs.MonochromeTones) && HasPaint && HasStatusEffect(Buffs.HammerTime))
                return OriginalHook(CometinBlack);  

            if (ActionReady(OriginalHook(HammerStamp)))
                return OriginalHook(HammerStamp);

            if (HasStatusEffect(Buffs.Starstruck))
                return StarPrism;

            if (HasStatusEffect(Buffs.RainbowBright))
                return RainbowDrip;

            if (CometinBlack.LevelChecked() && HasStatusEffect(Buffs.MonochromeTones) && HasPaint && !HasStatusEffect(Buffs.RainbowBright))
                return OriginalHook(CometinBlack);

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return OriginalHook(BlizzardinCyan);
        }
        return actionId;
    }

   
    #endregion

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

    #region Openers

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
            StarryMuse, //5
            HammerStamp,
            SubtractivePalette,
            BlizzardinCyan,
            StoneinYellow,
            ThunderinMagenta,//10
            CometinBlack,
            WingedMuse,
            MogoftheAges,
            StarPrism,
            HammerBrush,//15
            PolishingHammer,
            RainbowDrip,
            Role.Swiftcast,
            ClawMotif,
            ClawedMuse,//20
        ];
        internal override UserData? ContentCheckConfig => Config.PCT_Balance_Content;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
[
            ([8, 9, 10], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
            ([8, 9, 10], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([8, 9, 10], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([11], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
        ];
        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([17], () => !HasStatusEffect(Buffs.RainbowBright))];

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

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(Role.Swiftcast))
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
            WingMotif, //5 
            StarryMuse,
            HammerStamp,
            SubtractivePalette,
            BlizzardinCyan,
            BlizzardinCyan, //10
            BlizzardinCyan,
            CometinBlack,
            WingedMuse,
            MogoftheAges,
            StarPrism, //15
            HammerBrush,
            PolishingHammer,
            RainbowDrip,
            FireInRed,
            Role.Swiftcast, //20
            ClawMotif,
            ClawedMuse
        ];
        internal override UserData? ContentCheckConfig => Config.PCT_Balance_Content;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([18], () => !HasStatusEffect(Buffs.RainbowBright))];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([3], CometinBlack, () => HasStatusEffect(Buffs.MonochromeTones)),
            ([9, 10, 11], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
             ([9, 10, 11], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([9, 10, 11], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([12], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
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

            if (HasStatusEffect(Buffs.SubtractivePalette))
                return false;

            if (IsOnCooldown(Role.Swiftcast))
                return false;

            return true;
        }
        
    }
#endregion
}