#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.RDM.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using WrathCombo.Extensions;
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
        EnchantedRedoublementManafication = 45962,
        Zwerchhau = 7512,
        EnchantedZwerchhau = 7528,
        EnchantedZwerchhauManafication = 45961,
        Riposte = 7504,
        EnchantedRiposte = 7527,
        EnchantedRiposteManafication = 45960,
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
    internal static float VerFireRemaining => LocalPlayer.Status(Buffs.VerfireReady).RemainingTimeOrZero();
    internal static float VerStoneRemaining => LocalPlayer.Status(Buffs.VerstoneReady).RemainingTimeOrZero();

    //Bools
    internal static bool CanVerStone => LocalPlayer.HasStatus(Buffs.VerstoneReady);
    internal static bool CanVerFire => LocalPlayer.HasStatus(Buffs.VerfireReady);
    internal static bool CanVerFireAndStone => LocalPlayer.HasStatus(Buffs.VerstoneReady) && LocalPlayer.HasStatus(Buffs.VerfireReady);
    internal static bool CanGrandImpact => LocalPlayer.HasStatus(Buffs.GrandImpactReady);
    internal static bool CanMagickedSwordplay => LocalPlayer.HasStatus(Buffs.MagickedSwordPlay);
    internal static bool CanPrefulgence => LocalPlayer.HasStatus(Buffs.PrefulgenceReady);
    internal static bool CanViceOfThorns => LocalPlayer.HasStatus(Buffs.ThornedFlourish) && !JustUsed(Embolden, 6f);
    internal static bool HasDualcast => LocalPlayer.HasStatus(Buffs.Dualcast);
    internal static bool HasAccelerate => LocalPlayer.HasStatus(Buffs.Acceleration);
    internal static bool HasSwiftcast => LocalPlayer.HasStatus(Buffs.Swiftcast);
    internal static bool HasEmbolden => LocalPlayer.HasStatus(Buffs.Embolden);
    internal static bool HasManafication => LocalPlayer.HasStatus(Buffs.Manafication);
    internal static bool CanAcceleration => ActionLearned(Acceleration) && !CanVerFireAndStone && HasCharges(Acceleration) && CanInstantCD &&
                                            (EmboldenCD > 15 || ActionLearned(Embolden));
    internal static bool CanAccelerationMovement => ActionLearned(Acceleration) && IsMoving() && HasCharges(Acceleration) && CanInstantCD;
    internal static bool CanSwiftcast => Role.CanSwiftcast() && CanInstantCD && !CanVerFireAndStone && (EmboldenCD > 10 || ActionLearned(Embolden));
    internal static bool CanSwiftcastMovement => Role.CanSwiftcast() && CanInstantCD && IsMoving();
    internal static bool CanInstantCD => !InCombo && !HasSwiftcast && !CanGrandImpact && !HasEmbolden && !HasDualcast && !HasAccelerate && !InCombo;
    internal static bool CanEngagement => InMeleeRange() && HasCharges(Engagement) && ActionLearned(Engagement);
    internal static bool PoolEngagement => !ActionLearned(Embolden) || HasEmbolden || GetRemainingCharges(Engagement) >= 1 && GetCooldownChargeRemainingTime(Engagement) < 3;
    internal static bool SaveEngagement => GetRemainingCharges(Engagement) >= 2;
    internal static bool CanCorps => ActionLearned(Corpsacorps) && GetRemainingCharges(Corpsacorps) >= 1 && GetCooldownChargeRemainingTime(Corpsacorps) < 1;
    internal static bool CanInstantCast => HasDualcast || HasAccelerate || HasSwiftcast;
    internal static bool CanNotMagickBarrier => !ActionReady(MagickBarrier) || LocalPlayer.HasStatus(Buffs.MagickBarrier, true);
    #endregion

    #region Functions
    internal static int ManaLevel()
    {
        if (ActionLearned(Embolden)) // Level checks for Embolden then pools certain amounts of mana throughout the cd. 
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
        if (ActionLearned(Redoublement)) // Low level stuff
            return 50;
        return ActionLearned(Zwerchhau) ? 35 : 20;
    }

    internal static int ManaLevelStandalone()
    {
        if (ActionLearned(Redoublement)) // Low level stuff
            return 50;
        return ActionLearned(Zwerchhau) ? 35 : 20;
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
        if (!ActionLearned(Verthunder) && ActionLearned(Veraero)) // Low level Check
            return OriginalHook(Veraero);

        if (BlackHigher)
            return CanVerStone ?
                OriginalHook(Verthunder) :
                OriginalHook(Veraero);

        if (WhiteHigher)
            return CanVerFire ?
                OriginalHook(Veraero) :
                OriginalHook(Verthunder);

        return actionID;
    }
    internal static uint UseHolyFlare(uint actionID)
    {
        if (!ActionLearned(Verholy))
            return Verflare;

        if (BlackHigher)
        {
            if (CanVerStone && CanFlare)
                return CanVerFire ? Verholy : Verflare;
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
        if (!ActionLearned(Verthunder2))
            return OriginalHook(Jolt);
        if (BlackHigher)
            return ActionLearned(Veraero2) ? Veraero2 : Verthunder2;
        return WhiteHigher ? Verthunder2 : actionID;
    }
    #endregion

    #region Opener
    internal static Standard Opener1 = new();
    internal static GapClosing Opener2 = new();
    internal static FirstGCD Opener3 = new();

    internal static WrathOpener Opener()
    {
        if (RDM_Opener_Selection == 0 && Opener1.LevelChecked) return Opener1;
        if (RDM_Opener_Selection == 1 && Opener2.LevelChecked) return Opener2;
        if (RDM_Opener_Selection == 2 && Opener2.LevelChecked) return Opener3;

        return (Opener1.LevelChecked) ? Opener1 : WrathOpener.Dummy;
    }

    internal abstract class RDMOpenerBase : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override Preset Preset => Preset.RDM_Balance_Opener;

        internal override UserData? ContentCheckConfig => RDM_BalanceOpener_Content;
        internal override bool IncludePot => RDM_Opener_Potion;

        internal static uint Veraero3OrJolt3 =>
            PartyInCombat() && !Player.Object.IsCasting ? Jolt3 : Veraero3;

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !RDM_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 5))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => CountdownActive || InCombat() || !RDM_Opener_PrepullBlock)
        ];

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

    internal class Standard : RDMOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => Veraero3OrJolt3, // 2
            () => Verthunder3, // 3
            () => Role.Swiftcast, // 4
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Int)), // 5
            () => Verthunder3, // 6
            () => Fleche, // 7
            () => Acceleration, // 8
            () => Verthunder3, // 9
            () => Embolden, // 10
            () => Manafication, // 11
            () => EnchantedRiposteManafication, // 12
            () => ContreSixte, // 13
            () => EnchantedZwerchhauManafication, // 14
            () => Engagement, // 15
            () => EnchantedRedoublementManafication, // 16
            () => Corpsacorps, // 17
            () => Verholy, // 18
            () => ViceOfThorns, // 19
            () => Scorch, // 20
            () => Engagement, // 21
            () => Corpsacorps, // 22
            () => Resolution, // 23
            () => Prefulgence, // 24
            () => GrandImpact, // 25
            () => Acceleration, // 26
            () => Verfire, // 27
            () => GrandImpact, // 28
            () => Verthunder3, // 29
            () => Fleche, // 30
            () => Veraero3, // 31
            () => Verfire, // 32
            () => Verthunder3, // 33
            () => Verstone, // 34
            () => Veraero3, // 35
            () => Role.Swiftcast, // 36
            () => Veraero3, // 37
            () => ContreSixte // 38
        ];

        public Standard()
        {
            SkipSteps.Add(([15, 17, 21, 22], () => !InMeleeRange()));
            SkipSteps.Add(([7], () => !HasStatusEffect(Buffs.Swiftcast) && !JustUsed(Role.Swiftcast)));
        }
    }

    internal class GapClosing : RDMOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => Veraero3OrJolt3, // 2
            () => Verthunder3, // 3
            () => Role.Swiftcast, // 4
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Int)), // 5
            () => Verthunder3, // 6
            () => Fleche, // 7
            () => Acceleration, // 8
            () => Verthunder3, // 9
            () => Embolden, // 10
            () => Manafication, // 11
            () => EnchantedRiposteManafication, // 12
            () => ContreSixte, // 13
            () => EnchantedZwerchhauManafication, // 14
            () => Corpsacorps, // 15
            () => EnchantedRedoublementManafication, // 16
            () => Engagement, // 17
            () => Verholy, // 18
            () => ViceOfThorns, // 19
            () => Scorch, // 20
            () => Corpsacorps, // 21
            () => Engagement, // 22
            () => Resolution, // 23
            () => Prefulgence, // 24
            () => GrandImpact, // 25
            () => Acceleration, // 26
            () => Verfire, // 27
            () => GrandImpact, // 28
            () => Verthunder3, // 29
            () => Fleche, // 30
            () => Veraero3, // 31
            () => Verfire, // 32
            () => Verthunder3, // 33
            () => Verstone, // 34
            () => Veraero3, // 35
            () => Role.Swiftcast, // 36
            () => Veraero3, // 37
            () => ContreSixte // 38
        ];

        public GapClosing()
        {
            SkipSteps.Add(([17, 22], () => !InMeleeRange()));
            SkipSteps.Add(([36], () => !HasStatusEffect(Buffs.Swiftcast) && !JustUsed(Role.Swiftcast)));
        }
    }

    internal class FirstGCD : RDMOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => All.Cease, // 1
            () => Acceleration, // 2
            () => Veraero3OrJolt3, // 3
            () => Veraero3, // 4
            () => Embolden, // 5
            () => Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Int)), // 6
            () => GrandImpact, // 7
            () => Fleche, // 8
            () => Manafication, // 9
            () => EnchantedRiposteManafication, // 10
            () => Corpsacorps, // 11
            () => EnchantedZwerchhauManafication, // 12
            () => Engagement, // 13
            () => EnchantedRedoublementManafication, // 14
            () => ContreSixte, // 15
            () => Verflare, // 16
            () => Engagement, // 17
            () => Corpsacorps, // 18
            () => Scorch, // 19
            () => Acceleration, // 20
            () => Role.Swiftcast, // 21
            () => Resolution, // 22
            () => Veraero3, // 23
            () => ViceOfThorns, // 24
            () => Prefulgence, // 25
            () => GrandImpact, // 26
            () => Verthunder3, // 27
            () => Verfire, // 28
            () => Verthunder3, // 29
            () => Fleche // 30
        ];

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => !RDM_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 10)),
            ([3], () => !RDM_Opener_PrepullBlock ? 0 : Math.Max(0, CountdownRemaining - 5))
        ];

        public FirstGCD() =>
            SkipSteps.Add(([12, 16], () => !InMeleeRange()));

        public override bool HasCooldowns() =>
            base.HasCooldowns() &&
            !InCombat() &&
            CountdownRemaining <= 25;
    }
    #endregion
}


