using System;
using WrathCombo.Core;
using WrathCombo.Window.Functions;

namespace WrathCombo.Attributes;

/// <summary>
///     Flags a Preset as one that is Retargeted with
///     <see cref="ActionRetargeting"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class RetargetedAttribute : Attribute
{
}

/// <summary>
///     Flags a Preset as one that may be Retargeted with
///     <see cref="ActionRetargeting"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class PossiblyRetargetedAttribute : Attribute
{

    /// <summary>
    ///     Designates a Preset as one that may be Retargeted.
    /// </summary>
    /// <param name="settingInfo">
    ///     The setting that controls whether this Preset is Retargeted or not.
    /// </param>
    /// <param name="condition">
    ///     The <see cref="Condition"/> that needs to be satisfied to turn from
    ///     yellow to green.
    /// </param>
    /// <remarks>
    ///     Defaults to the <paramref name="settingInfo"/> and
    ///     <paramref name="condition"/> for Single Target Healing Actions.
    /// </remarks>
    internal PossiblyRetargetedAttribute
        (string settingInfo = "Settings Tab > Retarget (Single Target) Healing Actions",
            Condition condition = Condition.RetargetHealingActionsEnabled)
    {
        SettingInfo = settingInfo;
        PossibleCondition = condition;
    }

    public string SettingInfo { get; }
    public Condition PossibleCondition { get; }

    /// <summary>
    ///     These conditions should be implemented in the switch in
    ///     <see cref="Presets.DrawRetargetedAttribute">
    ///         DrawRetargetedAttribute()
    ///     </see>.
    /// </summary>
    internal enum Condition
    {
        RetargetHealingActionsEnabled,
        ASTQuickTargetCardsFeatureEnabled,
    }
}
