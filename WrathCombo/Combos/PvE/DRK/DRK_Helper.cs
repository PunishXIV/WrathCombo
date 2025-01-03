#region

using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK
{
    #region IDs

    public const byte JobID = 32;

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
        Grit = 3629, // Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=Self
        ReleaseGrit = 32067, // Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=Self
        ShadowWall = 3636, // Lv38, instant, 120.0s CD (group 20), range 0, single-target, targets=Self
        DarkMind = 3634, // Lv45, instant, 60.0s CD (group 8), range 0, single-target, targets=Self
        LivingDead = 3638, // Lv50, instant, 300.0s CD (group 24), range 0, single-target, targets=Self
        DarkMissionary = 16471, // Lv66, instant, 90.0s CD (group 14), range 0, AOE 30 circle, targets=Self
        BlackestNight = 7393, // Lv70, instant, 15.0s CD (group 2), range 30, single-target, targets=Self/Party
        Oblation = 25754, // Lv82, instant, 60.0s CD (group 18/71) (2 charges), range 30, single-target, targets=Self/Party
        ShadowedVigil = 36927; // Lv92, instant, 120.0s CD (group 20), range 0, single-target, targets=Self, animLock=???
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
            EnhancedDelirium = 572;
    }

    #endregion
    private static DRKGauge Gauge => GetJobGauge<DRKGauge>();

    /// <summary>
    ///     Whether the player has a shield from TBN from themselves.
    /// </summary>
    /// <seealso cref="Buffs.BlackestNightShield" />
    private static bool HasOwnTBN
    {
        get
        {
            var has = false;
            if (LocalPlayer is not null)
                has = FindEffect(
                    Buffs.BlackestNightShield,
                    LocalPlayer,
                    LocalPlayer.GameObjectId
                ) is not null;

            return has;
        }
    }

    /// <summary>
    ///     Whether the player has a shield from TBN from anyone.
    /// </summary>
    /// <seealso cref="Buffs.BlackestNightShield" />
    private static bool HasAnyTBN
    {
        get
        {
            var has = false;
            if (LocalPlayer is not null)
                has = FindEffect(Buffs.BlackestNightShield) is not null;

            return has;
        }
    }

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    /// <summary>
    ///     Decides if the player should use TBN on themselves,
    ///     based on general rules and the player's configuration.
    /// </summary>
    /// <param name="aoe">Whether AoE or ST options should be checked.</param>
    /// <returns>Whether TBN should be used on self.</returns>
    /// <seealso cref="BlackestNight" />
    /// <seealso cref="Buffs.BlackestNightShield" />
    /// <seealso cref="CustomComboPreset.DRK_ST_TBN" />
    /// <seealso cref="Config.DRK_ST_TBNThreshold" />
    /// <seealso cref="Config.DRK_ST_TBNBossRestriction" />
    /// <seealso cref="CustomComboPreset.DRK_AoE_TBN" />
    private static bool ShouldTBNSelf(bool aoe = false)
    {
        // Bail if TBN is disabled
        if ((!aoe
             && (!IsEnabled(CustomComboPreset.DRK_ST_Mitigation)
                 || !IsEnabled(CustomComboPreset.DRK_ST_TBN)))
            || (aoe
                && (!IsEnabled(CustomComboPreset.DRK_AoE_Mitigation)
                    || !IsEnabled(CustomComboPreset.DRK_AoE_TBN))))
            return false;

        // Bail if we already have TBN
        if (HasOwnTBN)
            return false;

        // Bail if we're dead or unloaded
        if (LocalPlayer is null)
            return false;

        // Bail if we have no target
        if (LocalPlayer.TargetObject is null)
            return false;

        var hpRemaining = PlayerHealthPercentageHp();
        var hpThreshold = !aoe ? (float)Config.DRK_ST_TBNThreshold : 90f;

        // Bail if we're above the threshold
        if (hpRemaining > hpThreshold)
            return false;

        var targetIsBoss = TargetIsBoss();
        var bossRestriction =
            !aoe
                ? (int)Config.DRK_ST_TBNBossRestriction
                : (int)Config.BossAvoidance.Off; // Don't avoid bosses in AoE

        // Bail if we're trying to avoid bosses and the target is one
        if (bossRestriction is (int)Config.BossAvoidance.On
            && targetIsBoss)
            return false;

        // Bail if we already have a TBN and burst is >30s away ()
        if (GetCooldownRemainingTime(LivingShadow) > 30
            && HasAnyTBN)
            return false;

        return true;
    }

    #region Mitigation Priority

    /// <summary>
    ///     The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///     The order of the list needs to match the order in
    ///     <see cref="CustomComboPreset" />.
    /// </summary>
    /// <value>
    ///     <c>Action</c> is the action to use.<br />
    ///     <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///     <c>Logic</c> is the logic for whether to use the action.
    /// </value>
    /// <remarks>
    ///     Each logic check is already combined with checking if the preset
    ///     <see cref="IsEnabled(uint)">is enabled</see>
    ///     and if the action is <see cref="ActionReady(uint)">ready</see> and
    ///     <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///     Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, CustomComboPreset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        (BlackestNight, CustomComboPreset.DRK_Mit_TheBlackestNight,
            () => !HasAnyTBN && LocalPlayer.CurrentMp > 3000 &&
                  PlayerHealthPercentageHp() <= Config.DRK_Mit_TBN_Health),
        (Oblation, CustomComboPreset.DRK_Mit_Oblation,
            () => (!((HasFriendlyTarget() && TargetHasEffectAny(Buffs.Oblation)) ||
                     (!HasFriendlyTarget() && HasEffectAny(Buffs.Oblation)))) &&
                  GetRemainingCharges(Oblation) > Config.DRK_Mit_Oblation_Charges),
        (All.Reprisal, CustomComboPreset.DRK_Mit_Reprisal,
            () => InActionRange(All.Reprisal)),
        (DarkMissionary, CustomComboPreset.DRK_Mit_DarkMissionary,
            () => Config.DRK_Mit_DarkMissionary_PartyRequirement ==
                  (int)Config.PartyRequirement.No ||
                  IsInParty()),
        (All.Rampart, CustomComboPreset.DRK_Mit_Rampart,
            () => PlayerHealthPercentageHp() <= Config.DRK_Mit_Rampart_Health),
        (DarkMind, CustomComboPreset.DRK_Mit_DarkMind, () => true),
        (All.ArmsLength, CustomComboPreset.DRK_Mit_ArmsLength,
            () => CanCircleAoe(7) >= Config.DRK_Mit_ArmsLength_EnemyCount &&
                  (Config.DRK_Mit_ArmsLength_Boss == (int)Config.BossAvoidance.Off ||
                   InBossEncounter())),
        (OriginalHook(ShadowWall), CustomComboPreset.DRK_Mit_ShadowWall,
            () => PlayerHealthPercentageHp() <= Config.DRK_Mit_ShadowWall_Health),
    ];

    /// <summary>
    ///     Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///     checks if the mitigation is ready and meets the provided requirements.
    /// </summary>
    /// <param name="index">
    ///     The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///     which is the order of the mitigation in <see cref="CustomComboPreset" />.
    /// </param>
    /// <param name="action">
    ///     The variable to set to the action to, if the mitigation is set to be
    ///     used.
    /// </param>
    /// <returns>
    ///     Whether the mitigation is ready, enabled, and passes the provided logic
    ///     check.
    /// </returns>
    private static bool CheckMitigationConfigMeetsRequirements
        (int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion

    #region Openers

    internal static DRKOpenerMaxLevel1 Opener1 = new();

    internal class DRKOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            HardSlash,
            EdgeOfShadow,
            LivingShadow,
            SyphonStrike,
            Souleater, // 5
            Delirium,
            Disesteem,
            SaltedEarth,
            //EdgeOfShadow, // Depends on TBN pop
            ScarletDelirium,
            Shadowbringer, // 10
            EdgeOfShadow,
            Comeuppance,
            CarveAndSpit,
            EdgeOfShadow,
            Torcleaver, // 15
            Shadowbringer,
            EdgeOfShadow,
            Bloodspiller,
            SaltAndDarkness,
        ];

        internal override UserData? ContentCheckConfig =>
            Config.DRK_ST_OpenerDifficulty;

        public override bool HasCooldowns()
        {
            if (LocalPlayer.CurrentMp < 7000)
                return false;

            if (!ActionReady(LivingShadow))
                return false;

            if (!ActionReady(Delirium))
                return false;

            if (!ActionReady(CarveAndSpit))
                return false;

            if (!ActionReady(SaltedEarth))
                return false;

            if (GetRemainingCharges(Shadowbringer) < 2)
                return false;

            return true;
        }
    }

    #endregion
}
