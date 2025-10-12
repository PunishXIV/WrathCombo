#region

using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameHelpers;
using System;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using ContentInfo = ECommons.GameHelpers.Content;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Data;

internal static class HiddenFeaturesData
{
    private const StringComparison Lower =
        StringComparison.InvariantCultureIgnoreCase;

    private static bool FeaturesEnabled
    {
        get
        {
            if (!EZ.Throttle("hiddenFeatEnabledLastCheck", TS.FromSeconds(5)))
                return field;

            field = Service.Configuration.ShowHiddenFeatures;
            return field;
        }
    }

    /// <summary>
    ///     Check if a Hidden Feature Preset is enabled, and meets your conditions,
    ///     and then passes your logic for that feature; only False if the Feature
    ///     is fully enabled and the condition is met, but the logic fails.<br />
    ///     Use when you have a Hidden Feature Preset logic to employ, but it should
    ///     only be employed under certain circumstances; i.e. when you want to check
    ///     logic for a certain fight, but if not in that fight it should let the
    ///     surrounding logic still work.
    /// </summary>
    /// <param name="preset">The Hidden Feature Preset.</param>
    /// <param name="condition">
    ///     The condition that must be met to even run the logic.<br />
    ///     i.e. when you want to only check the logic in certain fights, saves you
    ///     the effort of doing <c>((inFight &amp;&amp; logic()) || !inFight)</c>, by
    ///     just putting <c>inFight</c> here.
    /// </param>
    /// <param name="logic">
    ///     The logic that must be met to enable the feature.
    /// </param>
    /// <returns>
    ///     True if Hidden Features or the preset are disabled, or if the condition
    ///     is not met; so that it can just be included in existing action logic.
    ///     <br />
    ///     Otherwise, returns: If Hidden Features and the preset are enabled,
    ///     and if the condition is met, and the logic is true.
    /// </returns>
    public static bool NonBlockingIsEnabledWith
        (Preset preset, Func<bool> condition, Func<bool> logic) =>
        (!FeaturesEnabled || !IsEnabled(preset) || !condition()) ||
        (FeaturesEnabled && IsEnabled(preset) && condition() && logic());

    /// <summary>
    ///     Similar to
    ///     <see cref="NonBlockingIsEnabledWith(Preset, Func{bool}, Func{bool})" />,
    ///     but without condition to be met as well.<br />
    ///     Will only return False if the Feature is fully enabled, but the logic fails.
    /// </summary>
    /// <seealso cref="NonBlockingIsEnabledWith(Preset, Func{bool}, Func{bool})" />
    public static bool NonBlockingIsEnabledWith
        (Preset preset, Func<bool> logic) =>
        NonBlockingIsEnabledWith(preset, () => true, logic);

    /// <summary>
    ///     Check if a Hidden Feature Preset is enabled, and meets your conditions,
    ///     and then passes your logic for that feature.<br />
    ///     Use when you have a Hidden Feature Preset logic to employ, without any
    ///     constraint to when that logic should be employed.
    /// </summary>
    /// <param name="preset">The Hidden Feature Preset.</param>
    /// <param name="logic">
    ///     The logic that must be met to enable the feature.
    /// </param>
    /// <returns>
    ///     If Hidden Features are enabled and the preset is enabled,
    ///     and the logic is true.
    /// </returns>
    public static bool IsEnabledWith
        (Preset preset, Func<bool> logic) =>
        FeaturesEnabled && IsEnabled(preset) && logic();

    internal static class Targeting
    {
        internal static bool R6SSquirrel
        {
            get
            {
                if (!Content.InR6S)
                {
                    field = false;
                    return field;
                }

                if (!FeaturesEnabled ||
                    !EZ.Throttle("hiddenFeatR6SSquirrelCheck", TS.FromSeconds(1)))
                    return field;

                field = CurrentTarget?.Name.ToString()
                            .Contains("mu", Lower)
                        ?? false;
                return field;
            }
        }

        internal static bool R6SJabber
        {
            get
            {
                if (!Content.InR6S)
                {
                    field = false;
                    return field;
                }

                if (!FeaturesEnabled ||
                    !EZ.Throttle("hiddenFeatR6SJabberCheck", TS.FromSeconds(1)))
                    return field;

                field = CurrentTarget?.Name.ToString()
                            .Contains("jabber", Lower)
                        ?? false;
                return field;
            }
        }

        internal static bool R7SCircleCastingAdd
        {
            get
            {
                if (!Content.InR7S)
                {
                    field = false;
                    return field;
                }

                if (!FeaturesEnabled ||
                    !EZ.Throttle("hiddenFeatR7SCircleCastingAddCheck",
                        TS.FromSeconds(1)))
                    return field;

                // ReSharper disable once MergeCastWithTypeCheck
                var battleTarget =
                    CurrentTarget is IBattleChara
                        ? (IBattleChara?)CurrentTarget
                        : null;
                field = battleTarget?.CastActionId == 43277;
                return field;
            }
        }
    }

    internal static class Content
    {
        internal static bool InR6S
        {
            get
            {
                if (!FeaturesEnabled ||
                    !EZ.Throttle("hiddenFeatInR6SCheck", TS.FromSeconds(10)))
                    return field;

                field = ContentInfo.ContentDifficulty == ContentDifficulty.Savage &&
                        ContentInfo.TerritoryID == 1259;
                return field;
            }
        }

        internal static bool InR7S
        {
            get
            {
                if (!FeaturesEnabled ||
                    !EZ.Throttle("hiddenFeatInR7SCheck", TS.FromSeconds(10)))
                    return field;

                field = ContentInfo.ContentDifficulty == ContentDifficulty.Savage &&
                        ContentInfo.TerritoryID == 1261;
                return field;
            }
        }
    }
}
