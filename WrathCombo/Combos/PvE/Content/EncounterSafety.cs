#region

using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using ContentInfo = ECommons.GameHelpers.Content;

#endregion

namespace WrathCombo.Combos.PvE.Content;

/// <summary>
///     Encounter-specific safety blocks, for enemies that must not be attacked
///     under certain conditions.
/// </summary>
public static class EncounterSafety
{
    /// <summary>Jagd Doll's BNpcName ID.</summary>
    private const uint JagdDoll = 3759;

    private static IGameObject? Target => SimpleTarget.HardTarget;

    /// <summary>
    ///     Blocks combat actions against The Epic of Alexander (Ultimate)'s
    ///     Jagd Dolls once they are below the 25% HP feed threshold, as any
    ///     further damage risks killing them and wiping the raid.
    /// </summary>
    /// <param name="actionID">
    ///     The action; left as the blocked action.
    /// </param>
    /// <returns>Whether the action should be blocked.</returns>
    public static bool TryGetTEADollBlock(ref uint actionID)
    {
        if (ContentInfo.TerritoryID != 887) // The Epic of Alexander (Ultimate)
            return false;

        return Target is not null &&
               Target.GetNameId() is JagdDoll &&
               GetTargetHPPercent(Target) < 25;
    }
}
