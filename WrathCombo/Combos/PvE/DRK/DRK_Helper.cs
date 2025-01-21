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

            if (!IsOffCooldown(LivingShadow))
                return false;

            if (!IsOffCooldown(Delirium))
                return false;

            if (!IsOffCooldown(CarveAndSpit))
                return false;

            if (!IsOffCooldown(SaltedEarth))
                return false;

            if (GetRemainingCharges(Shadowbringer) < 2)
                return false;


            return true;
        }
    }

    #endregion
}
