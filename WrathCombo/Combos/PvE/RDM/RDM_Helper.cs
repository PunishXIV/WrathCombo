#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.RDM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class RDM
{
    #region ID's
   
    #region Spells
    public const uint
        Verthunder = 7505,
        Veraero = 7507,
        Veraero2 = 16525,
        Veraero3 = 25856,
        Verthunder2 = 16524,
        Verthunder3 = 25855,
        Impact = 16526,
        Redoublement = 7516,
        EnchantedRedoublement = 7529,
        Zwerchhau = 7512,
        EnchantedZwerchhau = 7528,
        Riposte = 7504,
        EnchantedRiposte = 7527,
        Scatter = 7509,
        Verstone = 7511,
        Verfire = 7510,
        Vercure = 7514,
        Jolt = 7503,
        Jolt2 = 7524,
        Jolt3 = 37004,
        Verholy = 7526,
        Verflare = 7525,
        Fleche = 7517,
        ContreSixte = 7519,
        Engagement = 16527,
        Verraise = 7523,
        Scorch = 16530,
        Resolution = 25858,
        Moulinet = 7513,
        EnchantedMoulinet = 7530,
        EnchantedMoulinetDeux = 37002,
        EnchantedMoulinetTrois = 37003,
        Corpsacorps = 7506,
        Displacement = 7515,
        EnchantedReprise = 16528,
        Reprise = 16529,
        ViceOfThorns = 37005,
        GrandImpact = 37006,
        Prefulgence = 37007,
        Acceleration = 7518,
        Manafication = 7521,
        Embolden = 7520,
        MagickBarrier = 25857;
    #endregion
    
    #region Buffs & Debuffs
    public static class Buffs
    {
        public const ushort
            Swiftcast = 167,
            VerfireReady = 1234,
            VerstoneReady = 1235,
            Dualcast = 1249,
            Chainspell = 2560,
            Acceleration = 1238,
            Embolden = 1239,
            EmboldenOthers = 1297,
            Manafication = 1971,
            MagickBarrier = 2707,
            MagickedSwordPlay = 3875,
            ThornedFlourish = 3876,
            GrandImpactReady = 3877,
            PrefulgenceReady = 3878;
    }
    public static class Debuffs
    {
        public const ushort
            Addle = 1203;
    }
    #endregion
    
    #region Traits
    public static class Traits
    {
        public const uint
            EnhancedEmbolden = 620,
            EnhancedManaficationII = 622,
            EnhancedManaficationIII = 622,
            EnhancedAccelerationII = 624;
    }
    #endregion
    #endregion
    
    #region Variables

    // Combo List
    internal static readonly List<uint>
    ComboActionsList =
    [
        Riposte, EnchantedRiposte, Zwerchhau, EnchantedZwerchhau, Redoublement, EnchantedRedoublement, Verholy,
        Verflare, Scorch, Moulinet, EnchantedMoulinet, EnchantedMoulinetDeux, EnchantedMoulinetTrois
    ];
    internal static bool InCombo => ComboActionsList.Contains(ComboAction);
    
    // Gauge Stuff
    private static RDMGauge Gauge => GetJobGauge<RDMGauge>();
    internal static bool BlackHigher => Gauge.BlackMana >= Gauge.WhiteMana;
    internal static bool WhiteHigher => Gauge.BlackMana < Gauge.WhiteMana;
    internal static bool HasEnoughManaToStart => Gauge.BlackMana >= ManaLevel() && Gauge.WhiteMana >= ManaLevel();
    internal static bool HasEnoughManaToStartStandalone => Gauge.BlackMana >= ManaLevelStandalone() && Gauge.WhiteMana >= ManaLevelStandalone();
    internal static bool HasEnoughManaForCombo => Gauge is { BlackMana: >= 15, WhiteMana: >= 15 };
    internal static bool HasManaStacks => Gauge.ManaStacks == 3;
    internal static bool CanFlare => BlackHigher && Gauge.BlackMana - Gauge.WhiteMana < 18;
    internal static bool CanHoly => WhiteHigher && Gauge.WhiteMana - Gauge.BlackMana < 18;
    internal static bool RedoublementRepriseMana => Gauge is { WhiteMana: >= 20, BlackMana: >= 20 };
    internal static bool ZwerchhauRepriseMana => Gauge is { WhiteMana: >= 35, BlackMana: >= 35 };
    
    //Floats
    internal static float EmboldenCD => GetCooldownRemainingTime(Embolden);
    internal static float VerFireRemaining => GetStatusEffectRemainingTime(Buffs.VerfireReady);
    internal static float VerStoneRemaining => GetStatusEffectRemainingTime(Buffs.VerstoneReady);
    
    //Bools
    internal static bool CanVerStone => HasStatusEffect(Buffs.VerstoneReady);
    internal static bool CanVerFire => HasStatusEffect(Buffs.VerfireReady);
    internal static bool CanVerFireAndStone => HasStatusEffect(Buffs.VerstoneReady) && HasStatusEffect(Buffs.VerfireReady);
    internal static bool CanGrandImpact => HasStatusEffect(Buffs.GrandImpactReady);
    internal static bool CanMagickedSwordplay => HasStatusEffect(Buffs.MagickedSwordPlay);
    internal static bool CanPrefulgence => HasStatusEffect(Buffs.PrefulgenceReady);
    internal static bool CanViceOfThorns => HasStatusEffect(Buffs.ThornedFlourish) && !JustUsed(Embolden, 6f);
    internal static bool HasDualcast => HasStatusEffect(Buffs.Dualcast);
    internal static bool HasAccelerate => HasStatusEffect(Buffs.Acceleration);
    internal static bool HasSwiftcast => HasStatusEffect(Buffs.Swiftcast);
    internal static bool HasEmbolden => HasStatusEffect(Buffs.Embolden);
    internal static bool CanAcceleration => LevelChecked(Acceleration) && !CanVerFireAndStone && HasCharges(Acceleration) && CanInstantCD && 
                                            (EmboldenCD > 15 || LevelChecked(Embolden));
    internal static bool CanAccelerationMovement => LevelChecked(Acceleration) && IsMoving() && HasCharges(Acceleration) && CanInstantCD;
    internal static bool CanSwiftcast => Role.CanSwiftcast() && CanInstantCD && !CanVerFireAndStone && (EmboldenCD > 10 || LevelChecked(Embolden));
    internal static bool CanSwiftcastMovement => Role.CanSwiftcast() && CanInstantCD && IsMoving();
    internal static bool CanInstantCD => !InCombo && !HasSwiftcast && !CanGrandImpact && !HasEmbolden && !HasDualcast && !HasAccelerate && !InCombo;
    internal static bool CanEngagement => InMeleeRange() && HasCharges(Engagement) && LevelChecked(Engagement);
    internal static bool PoolEngagement => !LevelChecked(Embolden) || HasEmbolden || GetRemainingCharges(Engagement) >= 1 && GetCooldownChargeRemainingTime(Engagement) < 3;
    internal static bool CanCorps => LevelChecked(Corpsacorps) && GetRemainingCharges(Corpsacorps) >= 1 && GetCooldownChargeRemainingTime(Corpsacorps) < 1;
    internal static bool CanInstantCast => HasDualcast || HasAccelerate || HasSwiftcast;
    internal static bool CanNotMagickBarrier => !ActionReady(MagickBarrier) || HasStatusEffect(Buffs.MagickBarrier, anyOwner: true);
    #endregion
    
    #region Functions
    internal static int ManaLevel()
    {
        if (LevelChecked(Embolden)) // Level checks for Embolden then pools certain amounts of mana throughout the cd. 
        {
            if (HasEmbolden)
                return 50;
            switch (EmboldenCD)
            {
                case > 80:
                    return 60; //Fresh out of Embolden window requiring slightly higher to keep a third melee combo from happening before a few of the procs can be used
                case > 40 and <= 80:
                    return 55; // Normal operating fire at 50
                case > 15 and <= 40:
                    return 70; // As it gets closer increases level so if we do a melee combo we still have enough for double melee burst
                case <= 15:
                    return 90; // to prevent it from firing unless it is about to cap, should only fire for manual embolden users. 
            }
        }
        if (LevelChecked(Redoublement)) // Low level stuff
            return 50;
        return LevelChecked(Zwerchhau) ? 35 : 20;
    }
    
    internal static int ManaLevelStandalone()
    {
        if (LevelChecked(Redoublement)) // Low level stuff
            return 50;
        return LevelChecked(Zwerchhau) ? 35 : 20;
    }
    internal static bool UseVerStone()
    {
        if (!CanVerStone || HasDualcast || HasAccelerate || HasSwiftcast || VerStoneRemaining < 2.5 ||
            (CanVerFire && VerFireRemaining < 10 && VerFireRemaining < VerStoneRemaining)) 
            return false;
        
        if (BlackHigher || WhiteHigher && !CanVerFire) return true;

        return false;
    }
    internal static bool UseVerFire()
    {
        if (!CanVerFire || HasDualcast || HasAccelerate || HasSwiftcast || VerFireRemaining < 2.5 ||
            (CanVerStone && VerStoneRemaining < 10 && VerStoneRemaining < VerFireRemaining)) 
            return false;
        
        if (WhiteHigher || BlackHigher && !CanVerStone) return true;

        return false;
    }
    internal static uint UseInstantCastST(uint actionID)
    {
        if (!LevelChecked(Verthunder) && LevelChecked(Veraero)) // Low level Check
            return OriginalHook(Veraero);
        
        if (BlackHigher)
            return CanVerStone ?
                OriginalHook(Verthunder) :
                OriginalHook(Veraero);

        if (WhiteHigher)
            return CanVerFire ?
                OriginalHook(Veraero):
                OriginalHook(Verthunder);
    
        return actionID;
    }
    internal static uint UseHolyFlare(uint actionID)
    {
        if (!LevelChecked(Verholy))
            return Verflare;
        
        if (BlackHigher)
        {
            if (CanVerStone && CanFlare)
                return CanVerFire? Verholy : Verflare;
            return Verholy;
        }
        if (WhiteHigher)
        {
            if (CanVerFire && CanHoly)
                return CanVerStone ? Verflare : Verholy;
            return Verflare;
        }
        return actionID;
    }
    internal static uint UseThunderAeroAoE(uint actionID)
    {
        if (!LevelChecked(Verthunder2))
            return OriginalHook(Jolt);
        if (BlackHigher)
            return LevelChecked(Veraero2) ? Veraero2 : Verthunder2;
        return WhiteHigher ? Verthunder2 : actionID;
    }
    #endregion

    #region Opener
    internal static Standard Opener1 = new();
    internal static GapClosing Opener2 = new();
    internal static WrathOpener Opener()
    {
        if (RDM_Opener_Selection == 0 && Opener1.LevelChecked) return Opener1;
        if (RDM_Opener_Selection == 1 && Opener2.LevelChecked) return Opener2;
        
        return  (Opener1.LevelChecked) ? Opener1 : WrathOpener.Dummy;
    }
    internal class Standard : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Veraero3, 
            Verthunder3,
            Role.Swiftcast,
            Verthunder3,
            Fleche, // 5
            Acceleration,
            Verthunder3,
            Embolden,
            Manafication,
            EnchantedRiposte, //10
            ContreSixte,
            EnchantedZwerchhau,
            Engagement,
            EnchantedRedoublement,
            Corpsacorps, //15
            Verholy,
            ViceOfThorns,
            Scorch,
            Engagement,
            Corpsacorps, //20
            Resolution,
            Prefulgence,
            GrandImpact,
            Acceleration,
            Verfire,   //25
            GrandImpact,
            Verthunder3,
            Fleche,
            Veraero3,
            Verfire,  //30
            Verthunder3,
            Verstone,
            Veraero3,
            Role.Swiftcast,
            Veraero3, //35
            ContreSixte
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([1], Jolt3, () => PartyInCombat() && !Player.Object.IsCasting)
        ];

        internal override UserData? ContentCheckConfig => RDM_BalanceOpener_Content;

            public override bool HasCooldowns()
            {
                if (!ActionsReady([Role.Swiftcast, Fleche, Embolden, ContreSixte]) || GetRemainingCharges(Acceleration) < 2 ||
                    !IsOffCooldown(Manafication) ||
                    GetRemainingCharges(Engagement) < 2 ||
                    GetRemainingCharges(Corpsacorps) < 2)
                    return false;

            return true;
        }
    }
    internal class GapClosing : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Veraero3, 
            Verthunder3,
            Role.Swiftcast,
            Verthunder3,
            Fleche, // 5
            Acceleration,
            Verthunder3,
            Embolden,
            Manafication,
            GrandImpact, //10
            Corpsacorps,
            ContreSixte,
            EnchantedRiposte,
            Engagement,
            EnchantedZwerchhau, //15
            EnchantedRedoublement,
            Verholy,
            ViceOfThorns,
            Scorch,
            Engagement, //20
            Corpsacorps, 
            Resolution,
            Prefulgence,
            Acceleration,
            Verfire,   //25
            GrandImpact,
            Verthunder3,
            Fleche,
            Veraero3,
            Verfire,  //30
            Verthunder3,
            Verstone,
            Veraero3,
            Role.Swiftcast,
            Veraero3, //35
            ContreSixte
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([1], Jolt3, () => PartyInCombat() && !Player.Object.IsCasting)
        ];

        internal override UserData? ContentCheckConfig => RDM_BalanceOpener_Content;

        public override bool HasCooldowns()
        {
            if (!ActionsReady([Role.Swiftcast, Fleche, Embolden, ContreSixte]) || GetRemainingCharges(Acceleration) < 2 ||
                !IsOffCooldown(Manafication) ||
                GetRemainingCharges(Engagement) < 2 ||
                GetRemainingCharges(Corpsacorps) < 2)
                return false;

            return true;
        }
    }
    #endregion
}
