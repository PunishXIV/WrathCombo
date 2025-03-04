#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    #region Variables
    internal static byte Ammo => GetJobGauge<GNBGauge>().Ammo;
    internal static byte GunStep => GetJobGauge<GNBGauge>().AmmoComboStep;
    internal static float HPP => PlayerHealthPercentageHp();
    internal static int NmStop => Config.GNB_AoE_NoMercyStop;
    internal static float GCD => GetCooldown(KeenEdge).CooldownTotal;
    internal static float GfCD => GetCooldownRemainingTime(GnashingFang);
    internal static float NmCD => GetCooldownRemainingTime(NoMercy);
    internal static float DdCD => GetCooldownRemainingTime(DoubleDown);
    internal static float BfCD => GetCooldownRemainingTime(Bloodfest);
    internal static float NmLeft => GetBuffRemainingTime(Buffs.NoMercy);
    internal static bool HasNM => NmCD is >= 40 and <= 60;
    internal static bool HasBreak => HasEffect(Buffs.ReadyToBreak);
    internal static bool HasReign => HasEffect(Buffs.ReadyToReign);
    internal static bool CanBS => LevelChecked(BurstStrike) && Ammo > 0;
    internal static bool CanFC => LevelChecked(FatedCircle) && Ammo > 0;
    internal static bool CanGF => LevelChecked(GnashingFang) && GfCD < 0.6f && !HasEffect(Buffs.ReadyToBlast) && GunStep == 0 && Ammo > 0;
    internal static bool CanDD => LevelChecked(DoubleDown) && DdCD < 0.6f && Ammo > 0;
    internal static bool CanBF => LevelChecked(Bloodfest) && BfCD < 0.6f;
    internal static bool CanZone => LevelChecked(DangerZone) && GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f;
    internal static bool CanBreak => LevelChecked(SonicBreak) && HasBreak;
    internal static bool CanBow => LevelChecked(BowShock) && GetCooldownRemainingTime(BowShock) < 0.6f;
    internal static bool CanContinue => LevelChecked(Continuation);
    internal static bool CanReign => LevelChecked(ReignOfBeasts) && GunStep == 0 && HasReign;
    internal static bool InOdd => BfCD is < 90 and > 20;
    internal static bool CanLateWeave => CanDelayedWeave(start: 1);
    internal static bool MitUsed => JustUsed(OriginalHook(HeartOfStone), 4f) || JustUsed(OriginalHook(Nebula), 5f) || JustUsed(Camouflage, 5f) || JustUsed(All.Rampart, 5f) || JustUsed(Aurora, 5f) || JustUsed(Superbolide, 9f);
    internal static float GCDLength => ActionManager.GetAdjustedRecastTime(ActionType.Action, KeenEdge) / 1000f;
    internal static bool FastGNB => GCDLength < 2.43f; //2.42 or lower
    internal static bool MidGNB => GCDLength is <= 2.47f and >= 2.43f; //2.43 to 2.47
    internal static bool SlowGNB => GCDLength > 2.47f; //2.48 or higher
    #endregion

    #region Openers
    public static GNBOpenerMaxLevel1 Opener1 = new();
    public static GNBOpenerMaxLevel2 Opener2 = new();

    public static WrathOpener Opener()
    {
        if ((FastGNB || MidGNB) && Opener1.LevelChecked)
            return Opener1;

        if (SlowGNB && Opener2.LevelChecked)
            return Opener2;

        return WrathOpener.Dummy;
    }    
    internal class GNBOpenerMaxLevel1 : WrathOpener
    {
        //2.47 GCD or lower
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest,
            KeenEdge,
            BrutalShell,
            NoMercy,
            GnashingFang,
            JugularRip,
            BowShock,
            DoubleDown,
            BlastingZone,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            BurstStrike,
            Hypervelocity,
            SonicBreak
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            2,
            5
        ];
        internal override UserData ContentCheckConfig => Config.GNB_ST_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(Bloodfest))
                return false;

            if (!IsOffCooldown(NoMercy))
                return false;

            if (!IsOffCooldown(Hypervelocity))
                return false;

            if (!IsOffCooldown(SonicBreak))
                return false;

            if (!IsOffCooldown(DoubleDown))
                return false;

            if (!IsOffCooldown(BowShock))
                return false;

            return true;
        }
    }

    internal class GNBOpenerMaxLevel2 : WrathOpener
    {
        //Above 2.47 GCD
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest,
            KeenEdge,
            BurstStrike,
            NoMercy,
            Hypervelocity,
            GnashingFang,
            JugularRip,
            BowShock,
            DoubleDown,
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            2
        ];

        internal override UserData ContentCheckConfig => Config.GNB_ST_Balance_Content;
        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(Bloodfest))
                return false;

            if (!IsOffCooldown(NoMercy))
                return false;

            if (!IsOffCooldown(Hypervelocity))
                return false;

            if (!IsOffCooldown(SonicBreak))
                return false;

            if (!IsOffCooldown(DoubleDown))
                return false;

            if (!IsOffCooldown(BowShock))
                return false;

            return true;
        }
    }

    #endregion

    #region Helpers
    internal static int MaxCartridges() => TraitLevelChecked(427) ? 3 : TraitLevelChecked(257) ? 2 : 0; //Level Check helper for Maximum Ammo
    internal static uint GetVariantAction()
    {
        if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) &&
            IsEnabled(Variant.VariantCure) &&
            PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
        {
            return Variant.VariantCure;
        }

        Dalamud.Game.ClientState.Statuses.Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
            IsEnabled(Variant.VariantSpiritDart) &&
            CanWeave() && sustainedDamage is null)
        {
            return Variant.VariantSpiritDart;
        }

        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
            IsEnabled(Variant.VariantUltimatum) &&
            CanWeave() && ActionReady(Variant.VariantUltimatum))
        {
            return Variant.VariantUltimatum;
        }

        return 0; //No conditions met
    }
    internal static uint GetBozjaAction()
    {
        if (!Bozja.IsInBozja)
            return 0;

        bool CanUse(uint action) => HasActionEquipped(action) && IsOffCooldown(action);
        bool IsEnabledAndUsable(CustomComboPreset preset, uint action) => IsEnabled(preset) && CanUse(action);

        //Out-of-Combat
        if (!InCombat() && IsEnabledAndUsable(CustomComboPreset.GNB_Bozja_LostStealth, Bozja.LostStealth))
            return Bozja.LostStealth;

        //OGCDs
        if (CanWeave())
        {
            foreach (var (preset, action) in new[]
            {
            (CustomComboPreset.GNB_Bozja_LostFocus, Bozja.LostFocus),
            (CustomComboPreset.GNB_Bozja_LostFontOfPower, Bozja.LostFontOfPower),
            (CustomComboPreset.GNB_Bozja_LostSlash, Bozja.LostSlash),
            (CustomComboPreset.GNB_Bozja_LostFairTrade, Bozja.LostFairTrade),
            (CustomComboPreset.GNB_Bozja_LostAssassination, Bozja.LostAssassination),
        })
                if (IsEnabledAndUsable(preset, action))
                    return action;

            foreach (var (preset, action, powerPreset) in new[]
            {
            (CustomComboPreset.GNB_Bozja_BannerOfNobleEnds, Bozja.BannerOfNobleEnds, CustomComboPreset.GNB_Bozja_PowerEnds),
            (CustomComboPreset.GNB_Bozja_BannerOfHonoredSacrifice, Bozja.BannerOfHonoredSacrifice, CustomComboPreset.GNB_Bozja_PowerSacrifice)
        })
                if (IsEnabledAndUsable(preset, action) && (!IsEnabled(powerPreset) || JustUsed(Bozja.LostFontOfPower, 5f)))
                    return action;

            if (IsEnabledAndUsable(CustomComboPreset.GNB_Bozja_BannerOfHonedAcuity, Bozja.BannerOfHonedAcuity) &&
                !HasEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                return Bozja.BannerOfHonedAcuity;
        }

        //GCDs
        foreach (var (preset, action, condition) in new[]
        {
        (CustomComboPreset.GNB_Bozja_LostDeath, Bozja.LostDeath, true),
        (CustomComboPreset.GNB_Bozja_LostCure, Bozja.LostCure, PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure_Health),
        (CustomComboPreset.GNB_Bozja_LostArise, Bozja.LostArise, GetTargetHPPercent() == 0 && !HasEffect(All.Debuffs.Raise)),
        (CustomComboPreset.GNB_Bozja_LostReraise, Bozja.LostReraise, PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostReraise_Health),
        (CustomComboPreset.GNB_Bozja_LostProtect, Bozja.LostProtect, !HasEffect(Bozja.Buffs.LostProtect)),
        (CustomComboPreset.GNB_Bozja_LostShell, Bozja.LostShell, !HasEffect(Bozja.Buffs.LostShell)),
        (CustomComboPreset.GNB_Bozja_LostBravery, Bozja.LostBravery, !HasEffect(Bozja.Buffs.LostBravery)),
        (CustomComboPreset.GNB_Bozja_LostBubble, Bozja.LostBubble, !HasEffect(Bozja.Buffs.LostBubble)),
        (CustomComboPreset.GNB_Bozja_LostParalyze3, Bozja.LostParalyze3, !JustUsed(Bozja.LostParalyze3, 60f))
        })
            if (IsEnabledAndUsable(preset, action) && condition)
                return action;

        if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSpellforge) &&
            CanUse(Bozja.LostSpellforge) &&
            (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
            return Bozja.LostSpellforge;

        if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSteelsting) &&
            CanUse(Bozja.LostSteelsting) &&
            (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
            return Bozja.LostSteelsting;

        return 0; //No conditions met
    }

    #endregion

    #region Cooldowns

    #region OGCDs
    internal static bool ShouldUseNoMercy()
    {
        bool noMercyCondition = ActionReady(NoMercy) && InCombat() && HasTarget() && (LevelChecked(DoubleDown) ? (InOdd && (Ammo >= 2 || (ComboAction is BrutalShell && Ammo == 1)) || (!InOdd && Ammo != 3)) : Ammo > 0);
        if (FastGNB && noMercyCondition && CanLateWeave)
            return true;
        if ((MidGNB || SlowGNB) && noMercyCondition && CanWeave())
            return true;

        return false;
    }
    internal static bool ShouldUseBloodfest() => InCombat() && HasTarget() && CanWeave() && CanBF && Ammo == 0;
    internal static bool ShouldUseZone() => CanZone && CanWeave() && NmCD is < 57.5f and > 17f;
    internal static bool ShouldUseBowShock() => CanBow && CanWeave() && NmCD is < 57.5f and >= 40;
    internal static bool ShouldUseContinuation() => CanContinue && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge) || (LevelChecked(Hypervelocity) && HasEffect(Buffs.ReadyToBlast)));
    #endregion

    #region GCDs
    internal static bool ShouldUseLightningShot() => LevelChecked(LightningShot) && !InMeleeRange() && HasBattleTarget();
    internal static bool ShouldUseGnashingFang() => CanGF && (NmCD is > 17 and < 35 || JustUsed(NoMercy, 6f));
    internal static bool ShouldUseDoubleDown() => CanDD && HasNM && (IsOnCooldown(GnashingFang) || Ammo == 1);
    internal static bool ShouldUseSonicBreak() => CanBreak && ((IsOnCooldown(GnashingFang) || !LevelChecked(GnashingFang)) && (IsOnCooldown(DoubleDown) || !LevelChecked(DoubleDown)));
    internal static bool ShouldUseReignOfBeasts() => CanReign && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && !HasEffect(Buffs.ReadyToBreak) && GunStep == 0;
    internal static bool ApproachingOvercap() => ComboTimer > 0 && LevelChecked(SolidBarrel) && ComboAction == BrutalShell && LevelChecked(BurstStrike) && Ammo == MaxCartridges();
    internal static bool ShouldUseBurstStrike()
    {
        if ((IsEnabled(CustomComboPreset.GNB_ST_Simple) || (IsEnabled(CustomComboPreset.GNB_ST_Advanced) && IsEnabled(CustomComboPreset.GNB_ST_BurstStrike))) &&
            CanBS && HasNM && IsOnCooldown(GnashingFang) && (IsOnCooldown(DoubleDown) || (!LevelChecked(DoubleDown) && Ammo > 0)) && !HasReign && GunStep == 0)
            return true;
        if ((IsEnabled(CustomComboPreset.GNB_ST_Simple) || (IsEnabled(CustomComboPreset.GNB_ST_Advanced) && IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) && IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && IsEnabled(CustomComboPreset.GNB_ST_BurstStrike))) &&
            (LevelChecked(DoubleDown) && NmCD < 1 && Ammo == 3 && BfCD > 110 && ComboAction is KeenEdge) ||
            (LevelChecked(ReignOfBeasts) && NmCD < 1 && Ammo == 3 && !InOdd))
            return true;
        if ((IsEnabled(CustomComboPreset.GNB_ST_Simple) || (IsEnabled(CustomComboPreset.GNB_ST_Advanced) && IsEnabled(CustomComboPreset.GNB_ST_Overcap))) &&
            ApproachingOvercap())
            return true;

        return false;
    }
    #endregion

    #endregion

    #region IDs
    public const byte JobID = 37; //Gunbreaker (GNB)

    public const uint //Actions
    #region Offensive

        KeenEdge = 16137, //Lv1, instant, GCD, range 3, single-target, targets=hostile
        NoMercy = 16138, //Lv2, instant, 60.0s CD (group 10), range 0, single-target, targets=self
        BrutalShell = 16139, //Lv4, instant, GCD, range 3, single-target, targets=hostile
        DemonSlice = 16141, //Lv10, instant, GCD, range 0, AOE 5 circle, targets=self
        LightningShot = 16143, //Lv15, instant, GCD, range 20, single-target, targets=hostile
        DangerZone = 16144, //Lv18, instant, 30s CD (group 4), range 3, single-target, targets=hostile
        SolidBarrel = 16145, //Lv26, instant, GCD, range 3, single-target, targets=hostile
        BurstStrike = 16162, //Lv30, instant, GCD, range 3, single-target, targets=hostile
        DemonSlaughter = 16149, //Lv40, instant, GCD, range 0, AOE 5 circle, targets=self
        SonicBreak = 16153, //Lv54, instant, 60.0s CD (group 13/57), range 3, single-target, targets=hostile
        GnashingFang = 16146, //Lv60, instant, 30.0s CD (group 5/57), range 3, single-target, targets=hostile, animLock=0.700
        SavageClaw = 16147, //Lv60, instant, GCD, range 3, single-target, targets=hostile, animLock=0.500
        WickedTalon = 16150, //Lv60, instant, GCD, range 3, single-target, targets=hostile, animLock=0.770
        BowShock = 16159, //Lv62, instant, 60.0s CD (group 11), range 0, AOE 5 circle, targets=self
        AbdomenTear = 16157, //Lv70, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
        JugularRip = 16156, //Lv70, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
        EyeGouge = 16158, //Lv70, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
        Continuation = 16155, //Lv70, instant, 1.0s CD (group 0), range 0, single-target, targets=self, animLock=???
        FatedCircle = 16163, //Lv72, instant, GCD, range 0, AOE 5 circle, targets=self
        Bloodfest = 16164, //Lv76, instant, 120.0s CD (group 14), range 25, single-target, targets=hostile
        BlastingZone = 16165, //Lv80, instant, 30.0s CD (group 4), range 3, single-target, targets=hostile
        Hypervelocity = 25759, //Lv86, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
        DoubleDown = 25760, //Lv90, instant, 60.0s CD (group 12/57), range 0, AOE 5 circle, targets=self
        FatedBrand = 36936, //Lv96, instant, 1.0s CD, (group 0), range 5, AOE, targets=hostile
        ReignOfBeasts = 36937, //Lv100, instant, GCD, range 3, single-target, targets=hostile
        NobleBlood = 36938, //Lv100, instant, GCD, range 3, single-target, targets=hostile
        LionHeart = 36939, //Lv100, instant, GCD, range 3, single-target, targets=hostile

    #endregion
    #region Defensive

        Camouflage = 16140, //Lv6, instant, 90.0s CD (group 15), range 0, single-target, targets=self
        RoyalGuard = 16142, //Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=self
        ReleaseRoyalGuard = 32068, //Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=self
        Nebula = 16148, //Lv38, instant, 120.0s CD (group 21), range 0, single-target, targets=self
        Aurora = 16151, //Lv45, instant, 60.0s CD (group 19/71), range 30, single-target, targets=self/party/alliance/friendly
        Superbolide = 16152, //Lv50, instant, 360.0s CD (group 24), range 0, single-target, targets=self
        HeartOfLight = 16160, //Lv64, instant, 90.0s CD (group 16), range 0, AOE 30 circle, targets=self
        HeartOfStone = 16161, //Lv68, instant, 25.0s CD (group 3), range 30, single-target, targets=self/party
        Trajectory = 36934, //Lv56, instant, 30s CD (group 9/70) (2? charges), range 20, single-target, targets=hostile
        HeartOfCorundum = 25758, //Lv82, instant, 25.0s CD (group 3), range 30, single-target, targets=self/party
        GreatNebula = 36935, //Lv92, instant, 120.0s CD, range 0, single-target, targeets=self

    #endregion

        //Limit Break
        GunmetalSoul = 17105; //LB3, instant, range 0, AOE 50 circle, targets=self, animLock=3.860

    public static class Buffs
    {
        public const ushort
            BrutalShell = 1898, //applied by Brutal Shell to self
            NoMercy = 1831, //applied by No Mercy to self
            ReadyToRip = 1842, //applied by Gnashing Fang to self
            SonicBreak = 1837, //applied by Sonic Break to target
            BowShock = 1838, //applied by Bow Shock to target
            ReadyToTear = 1843, //applied by Savage Claw to self
            ReadyToGouge = 1844, //applied by Wicked Talon to self
            ReadyToBlast = 2686, //applied by Burst Strike to self
            Nebula = 1834, //applied by Nebula to self
            Rampart = 1191, //applied by Rampart to self
            Camouflage = 1832, //applied by Camouflage to self
            ArmsLength = 1209, //applied by Arm's Length to self
            HeartOfLight = 1839, //applied by Heart of Light to self
            Aurora = 1835, //applied by Aurora to self
            Superbolide = 1836, //applied by Superbolide to self
            HeartOfStone = 1840, //applied by Heart of Stone to self
            HeartOfCorundum = 2683, //applied by Heart of Corundum to self
            ClarityOfCorundum = 2684, //applied by Heart of Corundum to self
            CatharsisOfCorundum = 2685, //applied by Heart of Corundum to self
            RoyalGuard = 1833, //applied by Royal Guard to self
            GreatNebula = 3838, //applied by Nebula to self
            ReadyToRaze = 3839, //applied by Fated Circle to self
            ReadyToBreak = 3886, //applied by No mercy to self
            ReadyToReign = 3840; //applied by Bloodfest to target
    }

    public static class Debuffs
    {
        public const ushort
            BowShock = 1838, //applied by Bow Shock to target
            SonicBreak = 1837; //applied by Sonic Break to target
    }

    #endregion

    #region Mitigation Priority

    ///<summary>
    ///    The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///    The order of the list needs to match the order in
    ///    <see cref="CustomComboPreset" />.
    ///</summary>
    ///<value>
    ///    <c>Action</c> is the action to use.<br />
    ///    <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///    <c>Logic</c> is the logic for whether to use the action.
    ///</value>
    ///<remarks>
    ///    Each logic check is already combined with checking if the preset
    ///    <see cref="IsEnabled(uint)">is enabled</see>
    ///    and if the action is <see cref="ActionReady(uint)">ready</see> and
    ///    <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///    Do not add any of these checks to <c>Logic</c>.
    ///</remarks>
    private static (uint Action, CustomComboPreset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Heart of Corundum
        (OriginalHook(HeartOfStone), CustomComboPreset.GNB_Mit_Corundum,
            () => FindEffect(Buffs.HeartOfCorundum) is null &&
                  FindEffect(Buffs.HeartOfStone) is null &&
                  PlayerHealthPercentageHp() <= Config.GNB_Mit_Corundum_Health),
        //Aurora
        (Aurora, CustomComboPreset.GNB_Mit_Aurora,
            () => !(HasFriendlyTarget() && TargetHasEffectAny(Buffs.Aurora) ||
                    !HasFriendlyTarget() && HasEffectAny(Buffs.Aurora)) &&
                  GetRemainingCharges(Aurora) > Config.GNB_Mit_Aurora_Charges &&
                  PlayerHealthPercentageHp() <= Config.GNB_Mit_Aurora_Health),
        //Camouflage
        (Camouflage, CustomComboPreset.GNB_Mit_Camouflage, () => true),
        //Reprisal
        (All.Reprisal, CustomComboPreset.GNB_Mit_Reprisal,
            () => InActionRange(All.Reprisal)),
        //Heart of Light
        (HeartOfLight, CustomComboPreset.GNB_Mit_HeartOfLight,
            () => Config.GNB_Mit_HeartOfLight_PartyRequirement ==
                  (int)Config.PartyRequirement.No ||
                  IsInParty()),
        //Rampart
        (All.Rampart, CustomComboPreset.GNB_Mit_Rampart,
            () => PlayerHealthPercentageHp() <= Config.GNB_Mit_Rampart_Health),
        //Arm's Length
        (All.ArmsLength, CustomComboPreset.GNB_Mit_ArmsLength,
            () => CanCircleAoe(7) >= Config.GNB_Mit_ArmsLength_EnemyCount &&
                  (Config.GNB_Mit_ArmsLength_Boss == (int)Config.BossAvoidance.Off ||
                   InBossEncounter())),
        //Nebula
        (OriginalHook(Nebula), CustomComboPreset.GNB_Mit_Nebula,
            () => PlayerHealthPercentageHp() <= Config.GNB_Mit_Nebula_Health)
    ];

    ///<summary>
    ///    Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///    checks if the mitigation is ready and meets the provided requirements.
    ///</summary>
    ///<param name="index">
    ///    The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///    which is the order of the mitigation in <see cref="CustomComboPreset" />.
    ///</param>
    ///<param name="action">
    ///    The variable to set to the action to, if the mitigation is set to be
    ///    used.
    ///</param>
    ///<returns>
    ///    Whether the mitigation is ready, enabled, and passes the provided logic
    ///    check.
    ///</returns>
    private static bool CheckMitigationConfigMeetsRequirements
        (int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion
}
