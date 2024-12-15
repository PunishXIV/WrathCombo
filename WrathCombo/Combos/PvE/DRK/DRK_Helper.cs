#region

using Dalamud.Game.ClientState.Objects.SubKinds;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Functions = WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Options = WrathCombo.Combos.CustomComboPreset;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DRK
{
    /// <summary>
    ///     Shorter reference to the local player.
    /// </summary>
    /// <seealso cref="CustomComboFunctions.LocalPlayer" />
    private static readonly IPlayerCharacter? LocalPlayer = CustomComboFunctions.LocalPlayer;

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

        // Bail if we're not in configured content
        var inTBNContent = aoe || ContentCheck.IsInConfiguredContent(
            Config.DRK_ST_TBNDifficulty,
            Config.DRK_ST_TBNDifficultyListSet
        );

        if (!inTBNContent)
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
}
