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
    /// <param name="actionID">
    ///     The action to perform, if any.<br/>
    ///     Defaults to <see cref="All.SavageBlade"/> when the
    ///     <see langword="return"/> would be <see langword="false"/>.
    /// </param>
    /// <returns>
    ///     Whether any content-specific actions are suggested.
    /// </returns>
    public static bool TryGet(out uint actionID)
    {
        actionID = All.SavageBlade;
        
        // The methods below must check (first) that the player is in
        // the appropriate area (that should not be checked here)

        if (OccultCrescent.TryGetPhantomAction(ref actionID))
            return true;

        if (Variant.TryGetVariantAction(ref actionID))
            return true;

        // Bozja actions next
        // Deep dungeons next?
        
        return false;
    }
}