#region

using System;
using ECommons.EzIpcManager;
using arcOption = WrathCombo.Services.IPC.AutoRotationConfigOption;

#endregion

namespace WrathCombo.Services.IPC;

public partial class Provider
{
    /// <summary>
    ///     Get the state of Auto-Rotation Configuration in Wrath Combo.
    /// </summary>
    /// <param name="passedOption">
    ///     The option to check the value of.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>The correctly-typed value of the configuration.</returns>
    [EzIPC]
    public object? GetAutoRotationConfigState(object passedOption)
    {
        // Try to cast the input to an Auto-Rotation Configuration option
        arcOption option;
        try
        {
            option = (arcOption)Convert.ToInt32(passedOption);

            // Check if the config is overriden by a lease
            var checkControlled = Leasing.CheckAutoRotationConfigControlled(option);
            if (checkControlled is not null)
            {
                var type = Helper.GetAutoRotationConfigType(option);
                return type.IsEnum
                    ? checkControlled.Value
                    : Convert.ChangeType(checkControlled.Value, type);
            }
        }
        catch (Exception)
        {
            Logging.Warn("Invalid or not-yet-implemented `option` of " +
                          $"'{passedOption}'. Please refer to " +
                          "WrathCombo.Services.IPC.AutoRotationConfigOption");
            return null;
        }

        // Otherwise, return the actual config value
        var arc = Service.Configuration.RotationConfig;
        var arcD = Service.Configuration.RotationConfig.DPSSettings;
        var arcH = Service.Configuration.RotationConfig.HealerSettings;
        try
        {
            return option switch
            {
                arcOption.InCombatOnly => arc.InCombatOnly,
                arcOption.DPSRotationMode => arc.DPSRotationMode,
                arcOption.HealerRotationMode => arc.HealerRotationMode,
                arcOption.FATEPriority => arcD.FATEPriority,
                arcOption.QuestPriority => arcD.QuestPriority,
                arcOption.SingleTargetHPP => arcH.SingleTargetHPP,
                arcOption.AoETargetHPP => arcH.AoETargetHPP,
                arcOption.SingleTargetRegenHPP => arcH.SingleTargetRegenHPP,
                arcOption.ManageKardia => arcH.ManageKardia,
                arcOption.AutoRez => arcH.AutoRez,
                arcOption.AutoRezDPSJobs => arcH.AutoRezDPSJobs,
                arcOption.AutoCleanse => arcH.AutoCleanse,
                arcOption.IncludeNPCs => arcH.IncludeNPCs,
                arcOption.OnlyAttackInCombat => arcD.OnlyAttackInCombat,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(passedOption), passedOption, null)
            };
        }
        catch (Exception)
        {
            Logging.Error($"Invalid `option` of '{passedOption}'. Please refer to " +
                          "WrathCombo.Services.IPC.AutoRotationConfigOption");
            return null;
        }
    }

    /// <summary>
    ///     Set the state of Auto-Rotation Configuration in Wrath Combo.
    /// </summary>
    /// <param name="lease">Your lease ID from <see cref="RegisterForLease(string,string)" /></param>
    /// <param name="passedOption">
    ///     The Auto-Rotation Configuration option you want to set.<br />
    ///     This is a subset of the Auto-Rotation options, flattened into a single
    ///     enum.
    /// </param>
    /// <param name="value">
    ///     The value you want to set the option to.<br />
    ///     All valid options can be parsed from an int, or the exact expected types.
    /// </param>
    /// <seealso cref="AutoRotationConfigOption"/>
    [EzIPC]
    public void SetAutoRotationConfigState(Guid lease, object passedOption, object value)
    {
        // Bail for standard conditions
        if (Helper.CheckForBailConditionsAtSetTime(lease))
            return;

        // Try to cast the input to an Auto-Rotation Configuration option
        arcOption option;
        Type? type;
        TypeCode? typeCode;
        try
        {
            option = (arcOption)Convert.ToInt32(passedOption);

            // Try to convert the value to the correct type
            type = Helper.GetAutoRotationConfigType(option);
            typeCode = Type.GetTypeCode(type);
        }
        catch (Exception)
        {
            Logging.Warn("Invalid or not-yet-implemented `option` of " +
                          $"'{passedOption}'. Please refer to " +
                          "WrathCombo.Services.IPC.AutoRotationConfigOption");
            return;
        }

        object convertedValue;
        try
        {
            // Handle enum values as any number type, and convert it to the real enum
            if (type.IsEnum && typeCode is >= TypeCode.SByte and <= TypeCode.UInt64)
                convertedValue = Enum.ToObject(type, value);
            // Convert anything else directly
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

        Leasing.AddRegistrationForAutoRotationConfig(
            lease, option, (int)convertedValue);
    }
}
