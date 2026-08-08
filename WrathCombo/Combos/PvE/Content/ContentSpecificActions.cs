using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Combos.PvE;

/// <summary>
///     This class is used to organize the TryGet methods for specific,
///     not main-line PvE, content.
/// </summary>
public static class ContentSpecificActions
{
    /// <summary>
    ///     Runs the logic for content-specific actions.
    /// </summary>
    /// <param name="contentAction">
    ///     The action to perform, if any.<br/>
    ///     Defaults to <see cref="All.Cease"/> when the
    ///     <see langword="return"/> would be <see langword="false"/>.
    /// </param>
    /// <param name="healing">
    ///     Whether the Combo executing this is a healing Combo.
    /// </param>
    /// <returns>
    ///     Whether any content-specific actions are suggested.
    /// </returns>
    public static bool TryGet(ref uint actionId, out uint contentAction, bool healing = false)
    {
        contentAction = actionId;
        
        // The methods below must check (first) that the player is in
        // the appropriate area (that should not be checked here)

        if (Quests.TryGetQuestActionFix(ref contentAction))
            return true;

        // Skip checking for Combat Actions if this is a Healing Combo
        if (healing) return false;

        if (EncounterSafety.TryGetTEADollBlock(ref contentAction))
            return true;

        if (OccultCrescent.TryGetPhantomAction(ref contentAction))
            return true;

        if (Variant.TryGetVariantAction(ref contentAction))
            return true;

        if (Bozja.TryGetBozjaAction(ref contentAction))
            return true;

        // Deep dungeons next?

        return false;
    }
}