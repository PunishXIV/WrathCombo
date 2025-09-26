#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.PCT.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
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
                                            (HasStatusEffect(Buffs.SubtractiveSpectrum) || 
                                             gauge.PalleteGauge >= 50 && ScenicCD > 35 || 
                                             gauge.PalleteGauge == 100 && HasStatusEffect(Buffs.Aetherhues2)|| 
                                             gauge.PalleteGauge >= 50 && ScenicCD < 3 );
    internal static bool HasPaint => gauge.Paint > 0;
    //Buff Tracking
    internal static float ScenicCD => GetCooldownRemainingTime(StarryMuse);
    internal static float SteelCD => GetCooldownRemainingTime(StrikingMuse);
    #endregion

    #region Functions
    
    #region Hyper Phantasia
    internal static bool HyperPhantasiaMovementPaint()
    //Increase priority for using casts as soon as possible to avoid losing DPS and ensure all abilities fit within buff windows
    //previously, there were situations where Wrath prioritized using Hammer Combo over casts during hyperphantasia, which would prevent us from generating Rainbow Bright in time when movement is required
    //so, if we have Hyperphantasia stacks and Inspiration is active from standing in PCT LeyLines, we burn it all down
    {
        if (GetStatusEffectStacks(Buffs.Hyperphantasia) > 0 && HasStatusEffect(Buffs.Inspiration) && HasPaint)
        {
            if ((IsEnabled(Preset.PCT_ST_AdvancedMode_MovementOption_HolyInWhite) || IsEnabled(Preset.PCT_ST_SimpleMode)) 
                && HolyInWhite.LevelChecked())
                return true;
            if ((IsEnabled(Preset.PCT_ST_AdvancedMode_MovementOption_CometinBlack) || IsEnabled(Preset.PCT_ST_SimpleMode)) 
                && CometinBlack.LevelChecked())
                return true;
        }
        return false;
    }
    
    #endregion
    
    #region Standard Burst Window
    internal static uint BurstWindowStandard(uint actionId)
    {
        if (CanWeave())
        {
            if (ActionReady(SubtractivePalette) && 
                !HasStatusEffect(Buffs.SubtractivePalette) && 
                !HasStatusEffect(Buffs.MonochromeTones) &&
                (HasStatusEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50))
                return SubtractivePalette;
            
            if (SteelMuseReady && HasCharges(SteelMuse))
                return OriginalHook(SteelMuse);
               
            if (PortraitReady && IsOffCooldown(OriginalHook(MogoftheAges)) && !JustUsed(StarryMuse))
                return OriginalHook(MogoftheAges);

            if (LivingMuseReady && !PortraitReady && !JustUsed(StarryMuse))
                return OriginalHook(LivingMuse);
        }
        
        if (HasStatusEffect(Buffs.RainbowBright)) //Use as soon as 5 stacks are spent
            return RainbowDrip;
        
        if (ActionReady(OriginalHook(HammerStamp)) && 
            !HasStatusEffect(Buffs.Hyperphantasia) && //wait until after HP are gone to burn hammers
            GetStatusEffectRemainingTime(Buffs.StarryMuse) < 18) //before 92 dont burn hammers first
            return OriginalHook(HammerStamp);
    
        if (HasStatusEffect(Buffs.Starstruck) && 
            (GetStatusEffectRemainingTime(Buffs.StarryMuse) < 18 || //Normal use
             !HasStatusEffect(Buffs.SubtractivePalette))) //Simple opening, but time to weave a sub palette
            return StarPrism;
        
        if (HyperPhantasiaMovementPaint() && IsMoving()) //in case you need to move to burn HP
            return HasStatusEffect(Buffs.MonochromeTones) ? OriginalHook(CometinBlack) : OriginalHook(HolyInWhite);
        
        if (CometinBlack.LevelChecked() && GetStatusEffectRemainingTime(Buffs.StarryMuse) < 18 &&
            HasStatusEffect(Buffs.MonochromeTones) && HasPaint)
            return OriginalHook(CometinBlack);
                
        return HasStatusEffect(Buffs.SubtractivePalette) ? OriginalHook(BlizzardinCyan) : actionId;
    }
    #endregion
    
    #endregion

    #region ID's

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
        TempuraCoat = 34685,
        TempuraGrassa = 34686,
        ThunderIIinMagenta = 34661,
        ThunderinMagenta = 34655,
        WaterinBlue = 34652,
        WeaponMotif = 34690,
        WingMotif = 34665;

    public static class Buffs
    {
        public const ushort
            SubtractivePalette = 3674,
            Aetherhues2 = 3676,
            RainbowBright = 3679,
            HammerTime = 3680,
            MonochromeTones = 3691,
            StarryMuse = 3685,
            TempuraCoat = 3686,
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
    internal static PCT2ndStarryMaxLvl SecondStarryMaxLvl = new();
    internal static PCT3rdStarryMaxLvl ThirdStarryMaxLvl = new();
    internal static PCT2ndStarryLvl90 SecondStarryLvl90 = new();
    internal static PCT3rdStarryLvl90 ThirdStarryLvl90 = new();
    
    internal static WrathOpener Opener()
    {
        if (SecondStarryLvl90.LevelChecked && PCT_Opener_Choice == 0)
            return SecondStarryLvl90;

        if (ThirdStarryLvl90.LevelChecked && PCT_Opener_Choice == 1)
            return ThirdStarryLvl90;
        
        if (SecondStarryMaxLvl.LevelChecked && PCT_Opener_Choice == 0)
            return SecondStarryMaxLvl;
        
        if (ThirdStarryMaxLvl.LevelChecked && PCT_Opener_Choice == 1)
            return ThirdStarryMaxLvl;
        
        return WrathOpener.Dummy;
    }
    
    public static bool HasMotifs()
    {

        if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Pom))
            return false;

        if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Weapon))
            return false;

        if (!gauge.CanvasFlags.HasFlag(Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags.Landscape))
            return false;

        return true;
    }
    
    internal class PCT2ndStarryMaxLvl : WrathOpener
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
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;

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
    
    internal class PCT3rdStarryMaxLvl : WrathOpener
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
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;

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
    
     internal class PCT2ndStarryLvl90 : WrathOpener
    {
        //2nd GCD Starry Opener
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 99;
        
        public override List<uint> OpenerActions { get; set; } =
        [
            FireInRed,
            StrikingMuse,
            AeroInGreen,
            StarryMuse,
            HammerStamp,
            PomMuse,
            SubtractivePalette,
            WingMotif,
            WingedMuse,
            HammerBrush,
            MogoftheAges,
            PolishingHammer,
            ThunderinMagenta,
            BlizzardinCyan,
            StoneinYellow,//15
            CometinBlack,
            WaterInBlue,
            FireInRed//20
        ];
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
[
            ([13, 14, 15], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
            ([13, 14, 15], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([13, 14, 15], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([16], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
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

            return true;
        }
    }
     
     internal class PCT3rdStarryLvl90 : WrathOpener
    {
        //3rd GCD Starry Opener
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 99;
        public override List<uint> OpenerActions { get; set; } =
        [
            FireInRed,
            StrikingMuse,
            AeroInGreen,
            WaterInBlue,
            StarryMuse,
            HammerStamp,
            PomMuse,
            SubtractivePalette,
            WingMotif,
            WingedMuse,
            HammerBrush,
            MogoftheAges,
            PolishingHammer,
            BlizzardinCyan,
            StoneinYellow,
            ThunderinMagenta,
            CometinBlack,
            FireInRed,
            AeroInGreen,
            Role.Swiftcast, //20
            WaterInBlue
        ];
        internal override UserData? ContentCheckConfig => PCT_Balance_Content;
        
        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([14, 15, 16], BlizzardinCyan, () => OriginalHook(BlizzardinCyan) == BlizzardinCyan),
             ([14, 15, 16], StoneinYellow, () => OriginalHook(BlizzardinCyan) == StoneinYellow),
            ([14,15,16], ThunderinMagenta, () => OriginalHook(BlizzardinCyan) == ThunderinMagenta),
            ([17], HolyInWhite, () => !HasStatusEffect(Buffs.MonochromeTones)),
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