#region

using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Services;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

// ReSharper disable CheckNamespace

#endregion
namespace WrathCombo.Combos.PvE;

/// <summary>
///     Should be the same as <see cref="UIntExtensions" />, but with checking
///     the <see cref="PluginConfiguration.RetargetHealingActionsToStack" /> setting,
///     and automatically setting the target to the
///     <see cref="SimpleTarget.Stack.AllyToHeal">Heal Stack</see>.
/// </summary>
public static class HealRetargeting
{
    /// Just a buffer for checking the
    /// <see cref="PluginConfiguration.RetargetHealingActionsToStack" />
    /// setting.
    internal static bool RetargetSettingOn
    {
        get
        {
            if (!EZ.Throttle("healRetargetingConfig", TS.FromSeconds(0.9)))
                return field;

            field = Service.Configuration.RetargetHealingActionsToStack;
            return field;
        }
    }

    /// Just a shorter reference to
    /// <see cref="SimpleTarget.Stack.AllyToEsuna" />
    /// .
    private static IGameObject? EsunaStack => SimpleTarget.Stack.AllyToEsuna;

    /// Just a shorter reference to
    /// <see cref="SimpleTarget.Stack.AllyToHeal" />
    /// .
    private static IGameObject? HealStack => SimpleTarget.Stack.AllyToHeal;

    /// <summary>
    ///     Retargets the action if the
    ///     <see cref="PluginConfiguration.RetargetHealingActionsToStack">
    ///         option to do so
    ///     </see>
    ///     is enabled, and there is no target override.
    /// </summary>
    /// <seealso cref="UIntExtensions.Retarget(uint,IGameObject?,bool)" />
    public static uint RetargetIfEnabled
    (this uint actionID,
        IGameObject? optionalTarget,
        bool dontCull = false) =>
        RetargetSettingOn && optionalTarget is null
            ? actionID.Retarget(
                actionID == RoleActions.Healer.Esuna ? EsunaStack : HealStack,
                dontCull)
            : actionID;

    /// <summary>
    ///     Retargets the action if the
    ///     <see cref="PluginConfiguration.RetargetHealingActionsToStack">
    ///         option to do so
    ///     </see>
    ///     is enabled, and there is no target override.
    /// </summary>
    /// <seealso cref="UIntExtensions.Retarget(uint,uint,IGameObject?,bool)" />
    public static uint RetargetIfEnabled
    (this uint actionID,
        IGameObject? optionalTarget,
        uint replaced,
        bool dontCull = false) =>
        RetargetSettingOn && optionalTarget is null
            ? actionID.Retarget(replaced,
                actionID == RoleActions.Healer.Esuna ? EsunaStack : HealStack,
                dontCull)
            : actionID;

    /// <summary>
    ///     Retargets the action if the
    ///     <see cref="PluginConfiguration.RetargetHealingActionsToStack">
    ///         option to do so
    ///     </see>
    ///     is enabled, and there is no target override.
    /// </summary>
    /// <seealso cref="UIntExtensions.Retarget(uint,uint[],IGameObject?,bool)" />
    public static uint RetargetIfEnabled
    (this uint actionID,
        IGameObject? optionalTarget,
        uint[] replaced,
        bool dontCull = false) =>
        RetargetSettingOn && optionalTarget is null
            ? actionID.Retarget(replaced,
                actionID == RoleActions.Healer.Esuna ? EsunaStack : HealStack,
                dontCull)
            : actionID;
}
