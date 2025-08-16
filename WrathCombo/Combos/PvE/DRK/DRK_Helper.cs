#region

using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.AutoRotation;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.DRK.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK
{
    /// <summary>
    ///     Checking if there is space to weave an oGCD, with consideration for
    ///     whether triple weaves should be avoided or not.
    /// </summary>
    /// <seealso cref="CustomComboFunctions.CanWeave(double)" />
    /// <seealso cref="CanDelayedWeave(double,double)" />
    private static bool CanWeave => CanWeave() || CanDelayedWeave();

    /// <summary>
    ///     DRK's job gauge.
    /// </summary>
    private static DRKGauge Gauge => GetJobGauge<DRKGauge>();

    /// <summary>
    ///     DRK's GCD, truncated to two decimal places.
    /// </summary>
    private static double GCD => GetCooldown(HardSlash).CooldownTotal;

    #region Burst Phase

    #region Variables

    /// When the current burst phase is set to truly start.
    private static long burstStartTime;

    /// When the current burst phase is set to end.
    private static long burstEndTime;

    /// <summary>
    ///     Whether the player is being affected by other jobs' buffs.
    /// </summary>
    private static bool HasOtherJobsBuffs
    {
        get
        {
            // Only run every 1 seconds at most (should be every other burst check)
            if (!EZ.Throttle("drkBurstBuffCheck", TS.FromSeconds(1)))
                return field;

            field = TargetBuffRemainingTime(SCH.Debuffs.ChainStratagem) > 0 ||
                    BuffRemainingTime(AST.Buffs.Divination) > 0 ||
                    BuffRemainingTime(DRG.Buffs.BattleLitany) > 0 ||
                    TargetBuffRemainingTime(NIN.Debuffs.Mug) > 0 ||
                    TargetBuffRemainingTime(NIN.Debuffs.Dokumori) > 0 ||
                    BuffRemainingTime(MNK.Buffs.Brotherhood) > 0 ||
                    BuffRemainingTime(RPR.Buffs.ArcaneCircle) > 0 ||
                    BuffRemainingTime(BRD.Buffs.BattleVoice) > 0 ||
                    BuffRemainingTime(DNC.Buffs.TechnicalFinish) > 0 ||
                    BuffRemainingTime(SMN.Buffs.SearingLight) > 0 ||
                    BuffRemainingTime(RDM.Buffs.Embolden) > 0 ||
                    BuffRemainingTime(RDM.Buffs.EmboldenOthers) > 0 ||
                    BuffRemainingTime(PCT.Buffs.StarryMuse) > 0;

            return field;

            // Just a shorter name for the methods
            double TargetBuffRemainingTime(ushort buff) =>
                GetStatusEffectRemainingTime(buff, CurrentTarget, anyOwner:true);
            double BuffRemainingTime(ushort buff) =>
                GetStatusEffectRemainingTime(buff, anyOwner:true);
        }
    }

    #endregion

    /// <summary>
    /// Whether the DRK is in an even-minute Burst phase.
    /// </summary>
    internal static bool IsBursting
    {
        get
        {
            // Only run every .8 seconds at most
            if (!EZ.Throttle("drkBurstCheck", TS.FromSeconds(0.8)))
                return field;

            var burstAbility = LivingShadow;
            var burstAbilityCDWindow = 90;
            if (!LevelChecked(LivingShadow))
            {
                burstAbilityCDWindow = 40;
                burstAbility = LevelChecked(Delirium) ? Delirium : BloodWeapon;
            }

            // Fallback resetting of burst
            if (GetCooldownRemainingTime(burstAbility) < 2 || !InCombat())
            {
                burstStartTime = 0;
                burstEndTime = 0;
                field = false;
            }

            if (GetCooldownRemainingTime(burstAbility) >= burstAbilityCDWindow)
            {
                // If the buff is active, start burst in 4s
                if (burstStartTime == 0)
                    burstStartTime = Environment.TickCount64 + 4000;

                // If the buff is active, set end time to 30s away
                if (burstEndTime == 0)
                    burstEndTime = Environment.TickCount64 + 30000;

                // Set to bursting
                if ((burstStartTime > 0 && Environment.TickCount64 > burstStartTime) ||
                    HasOtherJobsBuffs)
                {
                    burstStartTime = 0;
                    field = true;
                }
            }

            // Reset bursting
            if (burstEndTime > 0 && Environment.TickCount64 > burstEndTime)
            {
                burstEndTime = 0;
                field = false;
            }

            return field;
        }
    }

    #endregion

    /// <summary>
    ///     Method for getting the player's target more reliably.
    /// </summary>
    /// <param name="flags">
    ///     The flags to describe the combo executing this method.
    /// </param>
    /// <param name="restrictToHostile">
    ///     Whether to restrict the target to hostile targets.<br />
    ///     Defaults to <c>true</c>.
    /// </param>
    /// <returns>
    ///     The player's current target, or the nearest target if AoE.
    /// </returns>
    private static IGameObject? Target(Combo flags, bool restrictToHostile = true)
    {
        var target = SimpleTarget.HardTarget ?? SimpleTarget.LastHostileHardTarget;
        if (target is not null)
            switch (restrictToHostile)
            {
                case true when target.IsHostile():
                case false:
                    return target;
            }

        if (flags.HasFlag(Combo.AoE))
            return AutoRotationController.DPSTargeting.BaseSelection
                .OrderByDescending(x => GetTargetDistance(x))
                .FirstOrDefault();

        return null;
    }

    /// <summary>
    ///     Select the opener to use.
    /// </summary>
    /// <returns>A valid <see cref="WrathOpener">Opener</see>.</returns>
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    #region Openers

    private static void handleEdgeCasts
        (uint currentAction, ref uint action, uint[] castLocations)
    {
        if (castLocations.Contains(currentAction) &&
            (Gauge.HasDarkArts || LocalPlayer.CurrentMp > 3000) &&
            CanWeave())
            action = OriginalHook(EdgeOfDarkness);
    }

    internal static DRKOpenerMaxLevel1 Opener1 = new();

    internal class DRKOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Unmend,
            HardSlash,
            EdgeOfShadow, // Not handled like a procc, since it sets up Darkside
            LivingShadow,
            SyphonStrike, // 5
            LivingShadow,
            Souleater,
            Delirium,
            HardSlash,
            Disesteem, // 10
            SaltedEarth,
            //EdgeOfShadow, // Handled like a procc
            ScarletDelirium,
            Shadowbringer,
            //EdgeOfShadow, // Handled like a procc
            Comeuppance,
            CarveAndSpit, // 15
            //EdgeOfShadow, // Handled like a procc
            Torcleaver,
            Shadowbringer,
            //EdgeOfShadow, // Handled like a procc
            Bloodspiller,
            SaltAndDarkness,
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps
        {
            get;
            set;
        } =
        [
            // Pull with Shadowstride as selected
            ([1], Shadowstride, () =>
                DRK_ST_OpenerAction == (int)PullAction.Shadowstride),
            // Pull with HardSlash as selected (requires skipping the now-duplicate HardSlash)
            ([1], HardSlash, () =>
                DRK_ST_OpenerAction == (int)PullAction.HardSlash),
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps
        {
            get;
            set;
        } =
        [
            // Skip the duplicate HardSlash, if pulling with HardSlash
            ([2], () =>
                DRK_ST_OpenerAction == (int)PullAction.HardSlash),
            // Skip the early LivingShadow, if non-standard
            ([4], () =>
                DRK_ST_OpenerAction != (int)PullAction.Unmend),
            // Skip the late LivingShadow and aligning HardSlash, if Standard
            ([6, 9], () => HasOtherJobsBuffs ||
                DRK_ST_OpenerAction == (int)PullAction.Unmend),
        ];

        internal override UserData? ContentCheckConfig =>
            DRK_ST_OpenerDifficulty;

        public override bool HasCooldowns() =>
            LocalPlayer.CurrentMp > 7000 && IsOffCooldown(LivingShadow) &&
            IsOffCooldown(Delirium) && IsOffCooldown(CarveAndSpit) &&
            IsOffCooldown(SaltedEarth) &&
            GetRemainingCharges(Shadowbringer) >= 2 &&
            (!InCombat() || CombatEngageDuration().TotalSeconds < 3);
    }

    #endregion

    #region IDs

    #region Actions

    public const uint

    #region Single-Target 1-2-3 Combo

        HardSlash = 3617,
        SyphonStrike = 3623,
        Souleater = 3632,

    #endregion

    #region AoE 1-2-3 Combo

        Unleash = 3621,
        StalwartSoul = 16468,

    #endregion

    #region Single-Target oGCDs

        CarveAndSpit = 3643, // With AbyssalDrain
        EdgeOfDarkness = 16467, // For MP
        EdgeOfShadow = 16470, // For MP // Upgrade of EdgeOfDarkness
        Bloodspiller = 7392, // For Blood
        ScarletDelirium = 36928, // Under Enhanced Delirium
        Comeuppance = 36929, // Under Enhanced Delirium
        Torcleaver = 36930, // Under Enhanced Delirium
        Shadowstride = 36926, // Dash, basically never

    #endregion

    #region AoE oGCDs

        AbyssalDrain = 3641, // Cooldown shared with CarveAndSpit
        FloodOfDarkness = 16466, // For MP
        FloodOfShadow = 16469, // For MP // Upgrade of FloodOfDarkness
        Quietus = 7391, // For Blood
        SaltedEarth = 3639,
        SaltAndDarkness = 25755, // Recast of Salted Earth
        Impalement = 36931, // Under Delirium

    #endregion

    #region Buffing oGCDs

        BloodWeapon = 3625,
        Delirium = 7390,

    #endregion

    #region Burst Window

        LivingShadow = 16472,
        Shadowbringer = 25757,
        Disesteem = 36932,

    #endregion

    #region Ranged Option

        Unmend = 3624,

    #endregion

    #region Mitigation

        Grit =
            3629, // Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=Self
        ReleaseGrit =
            32067, // Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=Self
        ShadowWall =
            3636, // Lv38, instant, 120.0s CD (group 20), range 0, single-target, targets=Self
        DarkMind =
            3634, // Lv45, instant, 60.0s CD (group 8), range 0, single-target, targets=Self
        LivingDead =
            3638, // Lv50, instant, 300.0s CD (group 24), range 0, single-target, targets=Self
        DarkMissionary =
            16471, // Lv66, instant, 90.0s CD (group 14), range 0, AOE 30 circle, targets=Self
        BlackestNight =
            7393, // Lv70, instant, 15.0s CD (group 2), range 30, single-target, targets=Self/Party
        Oblation =
            25754, // Lv82, instant, 60.0s CD (group 18/71) (2 charges), range 30, single-target, targets=Self/Party
        ShadowedVigil =
            36927; // Lv92, instant, 120.0s CD (group 20), range 0, single-target, targets=Self, animLock=???

    #endregion

    #endregion

    public static class Buffs
    {
        #region Main Buffs

        /// Tank Stance
        public const ushort Grit = 743;

        /// The lowest level buff, before Delirium
        public const ushort BloodWeapon = 742;

        /// The lower Delirium buff, with just the blood ability usage
        public const ushort Delirium = 1972;

        /// Different from Delirium, to do the Scarlet Delirium chain
        public const ushort EnhancedDelirium = 3836;

        /// The increased damage buff that should always be up - checked through gauge
        public const ushort Darkside = 741;

        #endregion

        #region "DoT" or Burst

        /// Ground DoT active status
        public const ushort SaltedEarth = 749;

        /// Charge to be able to use Disesteem
        public const ushort Scorn = 3837;

        #endregion

        #region Mitigation

        /// TBN Active - Dark arts checked through gauge
        public const ushort BlackestNightShield = 1178;

        /// The initial Invuln that needs procc'd
        public const ushort LivingDead = 810;

        /// The real, triggered Invuln that gives heals
        public const ushort WalkingDead = 811;

        /// The Invuln after completely healed
        public const ushort UndeadRebirth = 3255;

        /// Damage Reduction part of Vigil
        public const ushort ShadowedVigil = 3835;

        /// The triggered part of Vigil that needs procc'd to heal (happens below 50%)
        public const ushort ShadowedVigilant = 3902;

        /// Oblation Active
        public const ushort Oblation = 2682;

        #endregion
    }

    public static class Traits
    {
        public const uint
            BloodWeaponMastery = 570,
            EnhancedDelirium = 572,
            EnhancedShadowIII = 573;
    }

    #endregion
}
