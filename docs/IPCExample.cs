using System;
using ECommons.EzIpcManager;
using ECommons.Logging;
using ECommons.Reflection;

namespace WrathCombo;

public static class YourCode
{
    public static void EnableWrathAuto()
    {
        if (!WrathIPC.IsEnabled) return;
        try
        {
            var lease = (Guid)WrathIPC.CurrentLease!;
            // enable Wrath Combo Auto-Rotation
            WrathIPC.SetAutoRotationState(lease, true);
            // make sure the job is ready for Auto-Rotation
            WrathIPC.SetCurrentJobAutoRotationReady(lease);
            // if the job is ready, all the user's settings are locked
            // if the job is not ready, it turns on the job's simple modes, or if those don't
            // exist, it turns on the job's advanced modes with all options enabled
        }
        catch (Exception e)
        {
            PluginLog.Error("Unknown Wrath IPC error," +
                            "probably inability to register a lease." +
                            "\n" + e.Message);
        }
    }

    public static void EnableWrathAutoAndConfigureIt()
    {
        if (!WrathIPC.IsEnabled) return;
        try
        {
            var lease = (Guid)WrathIPC.CurrentLease!;
            WrathIPC.SetAutoRotationState(lease, true);
            var setJobReady = WrathIPC.SetCurrentJobAutoRotationReady(lease);
            WrathIPC.SetAutoRotationConfigState(lease,
                WrathIPC.AutoRotationConfigOption.InCombatOnly, false);
            WrathIPC.SetAutoRotationConfigState(lease,
                WrathIPC.AutoRotationConfigOption.AutoRez, true);
            WrathIPC.SetAutoRotationConfigState(lease,
                WrathIPC.AutoRotationConfigOption.SingleTargetHPP, 60);

            if (setJobReady == SetResult.Okay || setJobReady == SetResult.OkayWorking)
                PluginLog.Information("Job has been made ready for Auto-Rotation.");
        }
        catch (Exception e)
        {
            PluginLog.Error("Unknown Wrath IPC error," +
                            "probably inability to register a lease." +
                            "\n" + e.Message);
        }
    }
}

internal static class WrathIPC
{
    private static EzIPCDisposalToken[] _disposalTokens =
        EzIPC.Init(typeof(WrathIPC), "WrathCombo", SafeWrapper.IPCException);

    internal static bool IsEnabled =>
        DalamudReflector.TryGetDalamudPlugin("WrathCombo", out _, false, true);

    internal static Guid? CurrentLease
    {
        get
        {
            field ??= RegisterForLeaseWithCallback(
                "internalPluginName",
                "My Plugin's proper name",
                "myPluginIPCPrefixJustForWrath" // can be null, if your prefix=internal name
            );
            if (field is null)
                PluginLog.Warning("Failed to register for lease. " +
                                  "See logs from Wrath Combo for why.");
            return field;
        }
    }

    [EzIPC] internal static readonly Func<string, string, Guid?> RegisterForLease;
    [EzIPC] internal static readonly Func<string, string, string?, Guid?>
        RegisterForLeaseWithCallback;
    [EzIPC] internal static readonly Func<Guid, bool, SetResult>
        SetAutoRotationState;
    [EzIPC] internal static readonly Func<Guid, SetResult>
        SetCurrentJobAutoRotationReady;
    [EzIPC] internal static readonly
        Func<Guid, AutoRotationConfigOption, object, SetResult>
        SetAutoRotationConfigState;
    [EzIPC] internal static readonly Action<Guid> ReleaseControl;

    public enum SetResult
    {
        Okay = 0,
        OkayWorking = 1,

        IPCDisabled = 10,
        InvalidLease = 11,
        BlacklistedLease = 12,
        Duplicate = 13,
        PlayerNotAvailable = 14,
        InvalidConfiguration = 15,
        InvalidValue = 16,
    }

    public enum AutoRotationConfigOption
    {
        InCombatOnly = 0, // bool
        DPSRotationMode = 1, // enum
        HealerRotationMode = 2, // enum
        FATEPriority = 3, // bool
        QuestPriority = 4, // bool
        SingleTargetHPP = 5, // int
        AoETargetHPP = 6, // int
        SingleTargetRegenHPP = 7, // int
        ManageKardia = 8, // bool
        AutoRez = 9, // bool
        AutoRezDPSJobs = 10, // bool
        AutoCleanse = 11, // bool
        IncludeNPCs = 12, // bool
        OnlyAttackInCombat = 13, //bool
    }
}

public class MyIPC
{
    internal MyIPC()
    {
        EzIPC.Init(this, prefix: "myPluginIPCPrefixJustForWrath");
    }

    [EzIPC]
    public void WrathComboCallback(int reason, string additionalInfo)
    {
        PluginLog.Warning($"Lease was cancelled for reason {reason}. " +
                          $"Additional info: {additionalInfo}");

        if (reason == 0)
            PluginLog.Error("The user cancelled our lease." +
                            "We are suspended from creating a new lease for now.");

        // you can also convert the `reason` back to the `CancellationReason` enum.
        // you can copy this enum into your own class from:
        // https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Enums.cs#L117
    }
}
