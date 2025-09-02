#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.GNB.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class GNB : Tank
{
    #region Variables
    internal static byte Ammo => GetJobGauge<GNBGauge>().Ammo;
    internal static byte GunStep => GetJobGauge<GNBGauge>().AmmoComboStep;
    internal static float HPP => PlayerHealthPercentageHp();
    internal static float NMcd => GetCooldownRemainingTime(NoMercy);
    internal static float BFcd => GetCooldownRemainingTime(Bloodfest);
    internal static bool HasNM => NMcd is >= 39.5f and <= 60;
    internal static bool HasReign => HasStatusEffect(Buffs.ReadyToReign);
    internal static bool CanBS => LevelChecked(BurstStrike) && Ammo > 0;
    internal static bool CanGF => LevelChecked(GnashingFang) && GetCooldownRemainingTime(GnashingFang) < 0.6f && !HasStatusEffect(Buffs.ReadyToBlast) && GunStep == 0 && Ammo > 0;
    internal static bool CanDD => LevelChecked(DoubleDown) && GetCooldownRemainingTime(DoubleDown) < 0.6f && Ammo > 0;
    internal static bool CanBF => LevelChecked(Bloodfest) && BFcd < 0.6f;
    internal static bool CanZone => LevelChecked(DangerZone) && GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f;
    internal static bool CanSB => LevelChecked(SonicBreak) && HasStatusEffect(Buffs.ReadyToBreak);
    internal static bool CanBow => LevelChecked(BowShock) && GetCooldownRemainingTime(BowShock) < 0.6f;
    internal static bool CanContinue => LevelChecked(Continuation);
    internal static bool CanReign => LevelChecked(ReignOfBeasts) && GunStep == 0 && HasReign;
    internal static bool CanLateWeave => CanDelayedWeave(weaveStart: 0.9f);
    internal static bool InOdd => BFcd is < 90 and > 20;
    internal static bool MitUsed => JustUsed(OriginalHook(HeartOfStone), 4f) || JustUsed(OriginalHook(Nebula), 5f) || JustUsed(Camouflage, 5f) || JustUsed(Role.Rampart, 5f) || JustUsed(Aurora, 5f) || JustUsed(Superbolide, 9f);
    internal static float GCDLength => ActionManager.GetAdjustedRecastTime(ActionType.Action, KeenEdge) / 1000f;
    internal static bool SlowGNB => GCDLength >= 2.4800f;
    internal static bool MidGNB => GCDLength is <= 2.4799f and >= 2.4500f;
    internal static bool FastGNB => GCDLength is <= 2.4499f;
    internal static int STStopNM => GNB_ST_NoMercyStop;
    internal static int AoEStopNM => GNB_AoE_NoMercyStop;
    #endregion

    #region Openers
    public static Lv90FastNormalNM GNBLv90FastNormalNM = new();
    public static Lv100FastNormalNM GNBLv100FastNormalNM = new();
    public static Lv90SlowNormalNM GNBLv90SlowNormalNM = new();
    public static Lv100SlowNormalNM GNBLv100SlowNormalNM = new();
    public static Lv90FastEarlyNM GNBLv90FastEarlyNM = new();
    public static Lv100FastEarlyNM GNBLv100FastEarlyNM = new();
    public static Lv90SlowEarlyNM GNBLv90SlowEarlyNM = new();
    public static Lv100SlowEarlyNM GNBLv100SlowEarlyNM = new();

    public static WrathOpener Opener() => (!IsEnabled(Preset.GNB_ST_Opener) || !LevelChecked(DoubleDown)) ? WrathOpener.Dummy : GetOpener(GNB_Opener_NM == 0);
    private static WrathOpener GetOpener(bool isNormal)
    {
        if (FastGNB || MidGNB)
            return isNormal
                ? (LevelChecked(ReignOfBeasts) ? GNBLv100FastNormalNM : GNBLv90FastNormalNM)
                : (LevelChecked(ReignOfBeasts) ? GNBLv100FastEarlyNM : GNBLv90FastEarlyNM);

        if (SlowGNB)
            return isNormal
                ? (LevelChecked(ReignOfBeasts) ? GNBLv100SlowNormalNM : GNBLv90SlowNormalNM)
                : (LevelChecked(ReignOfBeasts) ? GNBLv100SlowEarlyNM : GNBLv90SlowEarlyNM);

        return WrathOpener.Dummy;
    }

    #region Lv90
    internal abstract class GNBOpenerLv90Base : WrathOpener
    {
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 99;
        internal override UserData ContentCheckConfig => GNB_ST_Balance_Content;
        public override bool HasCooldowns() => IsOffCooldown(NoMercy) && IsOffCooldown(GnashingFang) && IsOffCooldown(BowShock) && IsOffCooldown(Bloodfest) && IsOffCooldown(DoubleDown) && Ammo == 0;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([1], () => GNB_Opener_StartChoice == 1)];
    }
    internal class Lv90FastNormalNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            KeenEdge,
            BrutalShell,
            SolidBarrel, //+1 (1)
            NoMercy, //LateWeave
            GnashingFang, //-1 (0)
            Bloodfest, //+3 (3)
            JugularRip,
            DoubleDown, //-1 (2)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            BurstStrike, //-1 (1)
            Hypervelocity,
            BurstStrike, //-1 (0)
            Hypervelocity
        ];

        public override List<int> VeryDelayedWeaveSteps { get; set; } = [5];
    }
    internal class Lv90SlowNormalNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            KeenEdge,
            BrutalShell,
            NoMercy,
            Bloodfest, //+3 (3)
            GnashingFang, //-1 (2)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (1)
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            BurstStrike, //-1 (0)
            Hypervelocity,
            SolidBarrel, //+1 (1)
            BurstStrike, //-1 (0)
            Hypervelocity
        ];
    }
    internal class Lv90FastEarlyNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (1)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            BurstStrike, //-1 (0)
            Hypervelocity,
        ];

        public override List<int> VeryDelayedWeaveSteps { get; set; } = [4];
    }
    internal class Lv90SlowEarlyNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            KeenEdge,
            Bloodfest, //+3 (3)
            NoMercy,
            GnashingFang, //-1 (2)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (1)
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            BurstStrike, //-1 (0)
            Hypervelocity,
        ];
    }
    #endregion

    #region Lv100
    internal abstract class GNBOpenerLv100Base : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData ContentCheckConfig => GNB_ST_Balance_Content;
        public override bool HasCooldowns() => IsOffCooldown(Bloodfest) && IsOffCooldown(NoMercy) && IsOffCooldown(GnashingFang) && IsOffCooldown(DoubleDown) && IsOffCooldown(BowShock) && Ammo == 0;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([1], () => GNB_Opener_StartChoice == 1)];
    }
    internal class Lv100FastNormalNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            BrutalShell,
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (1)
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            BurstStrike, //-1 (0)
            Hypervelocity
        ];
        public override List<int> VeryDelayedWeaveSteps { get; set; } = [5];
    }
    internal class Lv100SlowNormalNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            BurstStrike, //-1 (2)
            NoMercy,
            Hypervelocity,
            GnashingFang, //-1 (1)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (0)
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
    }
    internal class Lv100FastEarlyNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (1)
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            BurstStrike, //-1 (0)
            Hypervelocity,
        ];
        public override List<int> VeryDelayedWeaveSteps { get; set; } = [3];
    }
    internal class Lv100SlowEarlyNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            BurstStrike, //-1 (2)
            NoMercy, //LateWeave
            Hypervelocity,
            GnashingFang, //-1 (1)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (0)
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
    }
    #endregion

    #endregion

    #region Helpers
    internal static int MaxCartridges() => TraitLevelChecked(Traits.CartridgeChargeII) ? 3 : TraitLevelChecked(Traits.CartridgeCharge) ? 2 : 0;
    internal static uint GetBozjaAction()
    {
        if (!Bozja.IsInBozja)
            return 0;

        bool CanUse(uint action) => HasActionEquipped(action) && IsOffCooldown(action);
        bool IsEnabledAndUsable(Preset preset, uint action) => IsEnabled(preset) && CanUse(action);

        if (!InCombat() && IsEnabledAndUsable(Preset.GNB_Bozja_LostStealth, Bozja.LostStealth))
            return Bozja.LostStealth;

        if (CanWeave())
        {
            foreach (var (preset, action) in new[]
            { (Preset.GNB_Bozja_LostFocus, Bozja.LostFocus),
            (Preset.GNB_Bozja_LostFontOfPower, Bozja.LostFontOfPower),
            (Preset.GNB_Bozja_LostSlash, Bozja.LostSlash),
            (Preset.GNB_Bozja_LostFairTrade, Bozja.LostFairTrade),
            (Preset.GNB_Bozja_LostAssassination, Bozja.LostAssassination), })
                if (IsEnabledAndUsable(preset, action))
                    return action;

            foreach (var (preset, action, powerPreset) in new[]
            { (Preset.GNB_Bozja_BannerOfNobleEnds, Bozja.BannerOfNobleEnds, Preset.GNB_Bozja_PowerEnds),
            (Preset.GNB_Bozja_BannerOfHonoredSacrifice, Bozja.BannerOfHonoredSacrifice, Preset.GNB_Bozja_PowerSacrifice) })
                if (IsEnabledAndUsable(preset, action) && (!IsEnabled(powerPreset) || JustUsed(Bozja.LostFontOfPower, 5f)))
                    return action;

            if (IsEnabledAndUsable(Preset.GNB_Bozja_BannerOfHonedAcuity, Bozja.BannerOfHonedAcuity) &&
                !HasStatusEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                return Bozja.BannerOfHonedAcuity;
        }

        foreach (var (preset, action, condition) in new[]
        { (Preset.GNB_Bozja_LostDeath, Bozja.LostDeath, true),
        (Preset.GNB_Bozja_LostCure, Bozja.LostCure, PlayerHealthPercentageHp() <= GNB_Bozja_LostCure_Health),
        (Preset.GNB_Bozja_LostArise, Bozja.LostArise, GetTargetHPPercent() == 0 && !HasStatusEffect(RoleActions.Magic.Buffs.Raise)),
        (Preset.GNB_Bozja_LostReraise, Bozja.LostReraise, PlayerHealthPercentageHp() <= GNB_Bozja_LostReraise_Health),
        (Preset.GNB_Bozja_LostProtect, Bozja.LostProtect, !HasStatusEffect(Bozja.Buffs.LostProtect)),
        (Preset.GNB_Bozja_LostShell, Bozja.LostShell, !HasStatusEffect(Bozja.Buffs.LostShell)),
        (Preset.GNB_Bozja_LostBravery, Bozja.LostBravery, !HasStatusEffect(Bozja.Buffs.LostBravery)),
        (Preset.GNB_Bozja_LostBubble, Bozja.LostBubble, !HasStatusEffect(Bozja.Buffs.LostBubble)),
        (Preset.GNB_Bozja_LostParalyze3, Bozja.LostParalyze3, !JustUsed(Bozja.LostParalyze3, 60f)) })
            if (IsEnabledAndUsable(preset, action) && condition)
                return action;

        if (IsEnabled(Preset.GNB_Bozja_LostSpellforge) &&
            CanUse(Bozja.LostSpellforge) &&
            (!HasStatusEffect(Bozja.Buffs.LostSpellforge) || !HasStatusEffect(Bozja.Buffs.LostSteelsting)))
            return Bozja.LostSpellforge;
        if (IsEnabled(Preset.GNB_Bozja_LostSteelsting) &&
            CanUse(Bozja.LostSteelsting) &&
            (!HasStatusEffect(Bozja.Buffs.LostSpellforge) || !HasStatusEffect(Bozja.Buffs.LostSteelsting)))
            return Bozja.LostSteelsting;

        return 0; //No conditions met
    }
    internal static uint OtherAction
    {
        get
        {
            if (Bozja.IsInBozja && GetBozjaAction() is uint ba && ba != 0)
                return ba;
            return 0;
        }
    }
    internal static bool ShouldUseOther => OtherAction != 0;
    #endregion

    #region Rotation
    internal static bool ShouldUseNoMercy
    {
        get
        {
            var minimum = NMcd < 0.6f && InCombat() && HasBattleTarget();
            var three = (InOdd && (Ammo >= 2 || (ComboAction is BrutalShell && Ammo == 1))) || (!InOdd && Ammo != 3);
            var two = TraitLevelChecked(Traits.CartridgeCharge) ? Ammo > 0 : NMcd < 0.6f;
            var condition = minimum && (TraitLevelChecked(Traits.CartridgeChargeII) ? three : two);
            return (SlowGNB && condition && CanWeave()) || (MidGNB && condition && (InOdd ? CanWeave() : CanLateWeave)) || (FastGNB && condition && CanLateWeave);
        }
    }
    internal static bool ShouldUseBloodfest => HasBattleTarget() && CanWeave() && CanBF && Ammo == 0;
    internal static bool ShouldUseZone => CanZone && CanWeave() && NMcd is < 57.5f and > 17f;
    internal static bool ShouldUseBowShock => CanBow && CanWeave() && NMcd is < 57.5f and >= 40;
    internal static bool ShouldUseContinuation => CanContinue && (HasStatusEffect(Buffs.ReadyToRip) || HasStatusEffect(Buffs.ReadyToTear) || HasStatusEffect(Buffs.ReadyToGouge) ||
        (LevelChecked(Hypervelocity) && HasStatusEffect(Buffs.ReadyToBlast) && (LevelChecked(DoubleDown) ? (SlowGNB ? NMcd is > 1.5f || CanDelayedWeave(0.6f, 0) : (FastGNB || MidGNB)) : !LevelChecked(DoubleDown))));
    internal static bool ShouldUseGnashingFang => CanGF && (NMcd is > 17 and < 35 || JustUsed(NoMercy, 6f));
    internal static bool ShouldUseDoubleDown => CanDD && HasNM && (IsOnCooldown(GnashingFang) || Ammo == 1);
    internal static bool ShouldUseSonicBreak => CanSB && ((IsOnCooldown(GnashingFang) || !LevelChecked(GnashingFang)) && (IsOnCooldown(DoubleDown) || !LevelChecked(DoubleDown)));
    internal static bool ShouldUseReignOfBeasts => CanReign && IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) && !HasStatusEffect(Buffs.ReadyToBreak) && GunStep == 0;
    internal static bool ShouldUseBurstStrike => (CanBS && HasNM && IsOnCooldown(GnashingFang) && (IsOnCooldown(DoubleDown) || (!LevelChecked(DoubleDown) && Ammo > 0)) && !HasReign && GunStep == 0);
    internal static uint STCombo
        => ComboTimer > 0 ? ComboAction == KeenEdge && LevelChecked(BrutalShell) ? BrutalShell : ComboAction == BrutalShell && LevelChecked(SolidBarrel)
        ? (GNB_ST_Overcap_Choice == 0 && LevelChecked(BurstStrike) && Ammo == MaxCartridges() ? BurstStrike : SolidBarrel) : KeenEdge : KeenEdge;
    internal static uint AOECombo => (ComboTimer > 0 && ComboAction == DemonSlice && LevelChecked(DemonSlaughter) && (Ammo != MaxCartridges() || GNB_AoE_Overcap_Choice == 1)) ? DemonSlaughter : DemonSlice;
    internal static bool ShouldUseLightningShot => LevelChecked(LightningShot) && !InMeleeRange() && HasBattleTarget();
    #endregion

    #region IDs

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
    public static class Traits
    {
        public const ushort
            TankMastery = 320, //Lv1
            CartridgeCharge = 257, //Lv30
            EnhancedBrutalShell = 258, //Lv52
            DangerZoneMastery = 259, //Lv80
            HeartOfStoneMastery = 424, //Lv82
            EnhancedAurora = 425, //Lv84
            MeleeMastery = 507, //Lv84
            EnhancedContinuation = 426, //Lv86
            CartridgeChargeII = 427, //Lv88
            NebulaMastery = 574, //Lv92
            EnhancedContinuationII = 575,//Lv96
            EnhancedBloodfest = 576; //Lv100
    }

    #endregion

    #region Mitigation Priority

    ///<summary>
    ///   The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///   The order of the list needs to match the order in
    ///   <see cref="Preset" />.
    ///</summary>
    ///<value>
    ///   <c>Action</c> is the action to use.<br />
    ///   <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///   <c>Logic</c> is the logic for whether to use the action.
    ///</value>
    ///<remarks>
    ///    Each logic check is already combined with checking if the preset is
    ///    enabled and if the action is <see cref="ActionReady(uint)">ready</see>
    ///    and <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///   Do not add any of these checks to <c>Logic</c>.
    ///</remarks>
    private static (uint Action, Preset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Heart of Corundum
        (OriginalHook(HeartOfStone), Preset.GNB_Mit_Corundum,
            () => !HasStatusEffect(Buffs.HeartOfCorundum) &&
                  !HasStatusEffect(Buffs.HeartOfStone) &&
                  PlayerHealthPercentageHp() <= GNB_Mit_Corundum_Health),
        //Aurora
        (Aurora, Preset.GNB_Mit_Aurora,
            () => !(TargetIsFriendly() && HasStatusEffect(Buffs.Aurora, CurrentTarget, true) ||
                    !TargetIsFriendly() && HasStatusEffect(Buffs.Aurora, anyOwner: true)) &&
                  GetRemainingCharges(Aurora) > GNB_Mit_Aurora_Charges &&
                  PlayerHealthPercentageHp() <= GNB_Mit_Aurora_Health),
        //Camouflage
        (Camouflage, Preset.GNB_Mit_Camouflage, () => true),
        //Reprisal
        (Role.Reprisal, Preset.GNB_Mit_Reprisal,
            () => Role.CanReprisal(checkTargetForDebuff:false)),
        //Heart of Light
        (HeartOfLight, Preset.GNB_Mit_HeartOfLight,
            () => GNB_Mit_HeartOfLight_PartyRequirement ==
                  (int)PartyRequirement.No ||
                  IsInParty()),
        //Rampart
        (Role.Rampart, Preset.GNB_Mit_Rampart,
            () => Role.CanRampart(GNB_Mit_Rampart_Health)),
        //Arm's Length
        (Role.ArmsLength, Preset.GNB_Mit_ArmsLength,
            () => Role.CanArmsLength(GNB_Mit_ArmsLength_EnemyCount,
                GNB_Mit_ArmsLength_Boss)),
        //Nebula
        (OriginalHook(Nebula), Preset.GNB_Mit_Nebula,
            () => PlayerHealthPercentageHp() <= GNB_Mit_Nebula_Health)
    ];

    ///<summary>
    ///   Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///   checks if the mitigation is ready and meets the provided requirements.
    ///</summary>
    ///<param name="index">
    ///   The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///   which is the order of the mitigation in <see cref="Preset" />.
    ///</param>
    ///<param name="action">
    ///   The variable to set to the action to, if the mitigation is set to be
    ///   used.
    ///</param>
    ///<returns>
    ///   Whether the mitigation is ready, enabled, and passes the provided logic
    ///   check.
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
