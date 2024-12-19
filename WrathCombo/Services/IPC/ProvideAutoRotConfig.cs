#region

using System;
using ECommons.EzIpcManager;

#endregion

namespace WrathCombo.Services.IPC;

public partial class Provider
{
    /// <summary>
    ///     Get the state of Auto-Rotation Configuration in Wrath Combo.
    /// </summary>
    /// <param name="option">The option to check the value of.</param>
    /// <returns>The correctly-typed value of the configuration.</returns>
    [EzIPC]
    public object? GetAutoRotationConfigState(AutoRotationConfigOption option)
    {
        var type = Helper.GetAutoRotationConfigType(option);

        // Check if the config is overriden by a lease
        var checkControlled = _leasing.CheckAutoRotationConfigControlled(option);
        if (checkControlled is not null)
        {
            return type.IsEnum
                ? checkControlled.Value
                : Convert.ChangeType(checkControlled.Value, type);
        }

        // Otherwise, return the actual config value
        var arc = Service.Configuration.RotationConfig;
        var arcD = Service.Configuration.RotationConfig.DPSSettings;
        var arcH = Service.Configuration.RotationConfig.HealerSettings;
        try
        {
            return option switch
            {
                AutoRotationConfigOption.InCombatOnly => arc.InCombatOnly,
                AutoRotationConfigOption.DPSRotationMode => arc.DPSRotationMode,
                AutoRotationConfigOption.HealerRotationMode =>
                    arc.HealerRotationMode,
                AutoRotationConfigOption.FATEPriority => arcD.FATEPriority,
                AutoRotationConfigOption.QuestPriority => arcD.QuestPriority,
                AutoRotationConfigOption.SingleTargetHPP => arcH.SingleTargetHPP,
                AutoRotationConfigOption.AoETargetHPP => arcH.AoETargetHPP,
                AutoRotationConfigOption.SingleTargetRegenHPP => arcH
                    .SingleTargetRegenHPP,
                AutoRotationConfigOption.ManageKardia => arcH.ManageKardia,
                AutoRotationConfigOption.AutoRez => arcH.AutoRez,
                AutoRotationConfigOption.AutoRezDPSJobs => arcH.AutoRezDPSJobs,
                AutoRotationConfigOption.AutoCleanse => arcH.AutoCleanse,
                _ => throw new ArgumentOutOfRangeException(nameof(option), option,
                    null)
            };
        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception _)
        {
            Logging.Error("Invalid `option`. Please refer to " +
                          "WrathCombo.Services.IPC.AutoRotationConfigOption");
            return null;
        }
#pragma warning restore CS0168 // Variable is declared but never used
    }

    /// <summary>
    ///     Set the state of Auto-Rotation Configuration in Wrath Combo.
    /// </summary>
    /// <param name="lease">Your lease ID from <see cref="RegisterForLease" /></param>
    /// <param name="option">
    ///     The Auto-Rotation Configuration option you want to set.<br />
    ///     This is a subset of the Auto-Rotation options, flattened into a single
    ///     enum.
    /// </param>
    /// <param name="value">
    ///     The value you want to set the option to.<br />
    ///     All valid options can be parsed from an int, or the exact expected types.
    /// </param>
    /// <value>+1 <c>set</c></value>
    /// <seealso cref="AutoRotationConfigOption"/>
    /// <seealso cref="AutoRotationConfigDPSRotationSubset"/>
    /// <seealso cref="AutoRotationConfigHealerRotationSubset"/>
    [EzIPC]
    public void SetAutoRotationConfigState
        (Guid lease, AutoRotationConfigOption option, object value)
    {
        // Bail for standard conditions
        if (_helper.CheckForBailConditionsAtSetTime(lease, 1))
            return;

        // Try to convert the value to the correct type
        var type = Helper.GetAutoRotationConfigType(option);
        var typeCode = Type.GetTypeCode(type);
        object convertedValue;
        try
        {
            if (type.IsEnum && typeCode is >= TypeCode.SByte and <= TypeCode.UInt64)
                convertedValue = Enum.ToObject(type, value);
            else
                convertedValue = Convert.ChangeType(value, type);
        }
        catch (Exception e)
        {
            Logging.Error("Failed to convert value to correct type.\n" +
                          "Value likely out of range for option that wanted an enum. " +
                          $"Expected type: {type}.\n" +
                          e.Message);
            return;
        }

        // Handle converting bool->int, which doesn't work for some reason, despite
        // int->bool working fine.
        if (type == typeof(bool))
            convertedValue = (bool)convertedValue ? 1 : 0;

        _leasing.AddRegistrationForAutoRotationConfig(
            lease, option, (int)convertedValue);
    }
}
