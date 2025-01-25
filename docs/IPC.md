> [!TIP]
> See [The Example C#](IPCExample.cs) for a barebones example of how to set up 
> the IPC in your plugin.

> [!TIP]
> Please check out the Table of Contents on GitHub for easy navigation,
> there is a lot of explanation of the IPC, but also simple code snippet usage 
> details in this guide, as well as a Changelog at the end.

## Capabilities of the Wrath Combo IPC

The Wrath Combo IPC allows other plugins to control the majority of Wrath Combo's 
settings,  in an ethereal way where there is no cleanup for the other plugin to do 
when done with control, and nothing to worry about at `Dispose` time.

These are the settings that are accessible via the IPC:
- Auto-Rotation state
- **Some** Auto-Rotation configuration options
- Setting of a whole job to be Auto-Rotation ready
- PvE Combos state and their Auto-Mode state
- PvE Options state

These are the settings that are not accessible via the IPC:
- **All** Auto-Rotation configuration options
  - There is a subset of options that have higher reliability and may actually
    need controlled by other plugins
  - Please see the actual [Enums](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Enums.cs)
    for these subsets of options
- PvP Combos and Options
  - These may be accessible in some cases, but are not actually supported
- Config options
  - These are the UI-only settings  (sliders, etc.), in `_Config.cs` files, that are 
    usually sub-options of Combo Options, and are not supported
  - These would theoretically only need changed to optimize a job, which is seen as
    unnecessary for another plugin to do
  - Additionally, these should default to fully workable and usually optimal
    settings, so there should be no need to change them
  - If you do want to change them, this would be seen as a bug with the default 
    values or the range of the config, and should be reported

## Working with the Wrath Combo IPC

### Usage Flow

The typical flow will be:
- Register for a lease
- Set the Auto-Rotation state
- Set the Auto-Rotation configuration options to the state you desire
- Set a job to be Auto-Rotation ready
- Eventually: Release control
- Work in a Callback when the lease that was Registered is eventually Cancelled

This flow is detailed in the "Setting Up" section.

But there is slightly more to be done as well, if wanted, but not documented here:
- The `get`ting of any supported setting
  - Not detailed as the `set`ting of options also locks those options to the 
    state specified. So even if the state is already what is desired, the lock on 
    that state should still be desired.
- The `set`ting of individual Combos and their Options
  - Not detailed as the `set`ting of a whole job should always be adequate.
  - If the user already has their job set up, it will simply act as a lock on 
    the user's settings; and if the user does not have the job setup then it will 
    activate the minimum settings for the job to be ready for Auto-Rotation 
    (which would be the Simple Modes for the job, if available, and otherwise the 
    Advanced Mode combos for a job with all options enabled, and any healing 
    combos with all options enabled).

See the Provider files ([main](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Provider.cs),
[auto-rot settings](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/ProvideAutoRotConfig.cs)) for more information on these extra methods.

### Arbitrary Limitations

There are a variety of limitations designed to help keep Wrath Combo users in 
control of their own settings, and to prevent dead leases from sticking around.

But the goal is to give as much control as possible to other plugins, so these 
big points are **NOT** limited:
- There are no explicitly disallowed plugins, that cannot access the IPC
- There are no time limits on registered leases
- There are no automatic revocations of leases based on behavior
- There is not a limitation on how many leases can be registered in general or to 
  any one plugin (except that they need unique display names)

With that said, however, there are a variety of arbitrary limitations on the use 
of the IPC that should be noted:
- Leases have a configuration limit
  - This limit is currently `60` individual configurations, with the exact costs 
    detailed in the Provider files.
    - The exact, current limit can be seen with the `MaxLeaseConfigurations` field
      [here](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Leasing.cs#L163).
  - This limit is designed to keep the focus on making the user Auto-Rotation ready,
    and not on optimizing the user's settings or setting up multiple jobs.
- Leases cannot share display names
  - This is designed to prevent confusion for the user regarding duplicate names 
    listed as controlling a single setting.
- Leases revoked by the user manually are temporarily blacklisted (2 minutes)
  - This is in place to prevent the Callback from immediately reinstating all 
    control, in an attempt to make it clearer to the user where and why this 
    plugin is taking control of their settings.
- Leases are revoked if the owning plugin is disabled, or Wrath is disabled
- The most recent Lease to control a setting
  - There is no revoking of other leases when they conflict, there is no checking 
    of conflicting configurations between leases, nothing. The most recent lease
    to `Set` a configuration is the one that controls it.
  - When displaying the control indicator for a setting in the UI, all leases, 
    regardless of state, will be listed for the user to revoke if the want to.
  - This is designed to keep there from being any sort of "competition" over a 
    setting between plugins, and to keep things simple for the user.

If Registered with a Callback, the reason a lease was cancelled, and any additional
information, will be provided to the callback method.

There is a list of cancellation reasons and explanations in the 
[CancellationReason Enum here](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Enums.cs#L117).

To reiterate: these limitations are not designed to impede the use of the IPC in 
any significant way. If you find that they are, please report it as a bug.

### Suspensions of Service

#### Blacklisting

Blacklisting only occurs when a Wrath Combo user manually revokes a lease, and 
saves the Internal Plugin Name given when registering and a hash of the Plugin's
current configuration.

The blacklist is checked at `Set` times, Registration time, and periodically.
If you have multiple leases, this can lead to your other leases being revoked as 
well.

Blacklisting is temporary, and per-client, and lasts just 2 minutes.

To reiterate what was said under the "Arbitrary Limitations" section, this is
designed to prevent the Callback from immediately reinstating all control, if you 
find the blacklisting to impede your plugin's use of the IPC or to overreach, please 
report it as a bug.

#### Remote Suspension

If necessary, the Wrath Combo IPC can also be suspended remotely by the team with 
the use of the [IPC Status file here](https://github.com/PunishXIV/WrathCombo/blob/main/res/ipc_status.txt).
Please refer to that file's commit history if you encounter this issue.

This would only be used if there is a significant issue with the IPC,
and would likely result in a hotfix.

If GitHub is down, the IPC will not be suspended, instead it will assume it to be 
enabled. There will still be error logs about it though.

### IPC Methods

The Provider files ([main](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Provider.cs),
[auto-rot settings](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/ProvideAutoRotConfig.cs))
are the real documentation on all IPC methods, and have verbose doc comments, 
this here will only serve to document via the verbose method names and brief 
comments on each method.

- `Guid? RegisterForLease(string, string)`
  - To initiate IPC control
  - Gives a lease ID to use for future `Set` methods
  - Can be done multiple times per plugin
- `Guid? RegisterForLease(string, string, Action)`
  - To register, with a callback
  - Not recommended for use, as to provide an `Action` you would need to do so 
    via Reflection or locally
  - Used primarily for testing
- `Guid? RegisterForLeaseWithCallback(string, string, string)`
  - To register, with a callback
  - The callback is a method in your own IPC class
  - The callback will be called when the lease is cancelled
  - See the Provider files for documentation on how your IPC callback method 
    should be setup
- `bool GetAutoRotationState()`
  - Checks if Auto-Rotation is enabled, whether by the user or another plugin
- `void SetAutoRotationState(Guid, bool)`
  - Requires a lease
  - Sets Auto-Rotation to be enabled or disabled
  - Locks the state away from the user
  - Counts towards the lease's configuration limit
- `bool IsCurrentJobAutoRotationReady()`
  - Checks if the current job is ready for Auto-Rotation, whether by the user or 
    another plugin
- `void SetCurrentJobAutoRotationReady(Guid)`
  - Requires a lease
  - Sets the current job to be ready for Auto-Rotation
    - If the job is ready: it will lock all the user's Simple/Advanced settings, 
      and any healing settings
    - If the job is not ready: it will turn on the job's Simple Modes, or if 
      those don't exist it will turn on the job's Advanced Modes with all options 
      enabled
  - Locks the state away from the user
  - Counts towards the lease's configuration limit
- `void ReleaseControl(Guid)`
  - Requires a lease
  - Releases control of all settings
  - Should be done when the plugin is done with control
  - Will trigger a registered callback
- `Dictionary IsCurrentJobConfiguredOn()`
  - Checks if the current job is configured on, whether by the user or another 
    plugin
  - Only whether a Single-Target and Multi-Target combo are configured on, NOT 
    whether they are enabled in Auto-Mode
- `Dictionary IsCurrentJobAutoModeOn()`
  - Checks if the current job is in Auto-Mode, whether by the user or another 
    plugin
  - Only whether a Single-Target and Multi-Target combo are enabled in Auto-Mode, 
    NOT whether they are turned on
- `List? GetComboNamesForJob(uint)`
  - Gets the names of all the combos for a job
- `Dictionary? GetComboOptionNamesForJob(uint)`
  - Gets the names of all the options for a job
- `Dictionary? GetComboState(string)`
  - Gets the state and Auto-Mode state of a combo, whether by the user or another 
    plugin
- `void SetComboState(Guid, string, bool)`
  - Sets the state and Auto-Mode state of a combo
  - Locks the state away from the user
  - Counts towards the lease's configuration limit
- `Dictionary? GetComboOptionState(string)`
  - Gets the state of a combo option, whether by the user or another plugin
- `void SetComboOptionState(Guid, string, bool)`
  - Sets the state of a combo option
  - Locks the state away from the user
  - Counts towards the lease's configuration limit
- `object? GetAutoRotationConfigState(AutoRotationConfigOption)`
  - Gets the state of an Auto-Rotation configuration option, whether by the user 
    or another plugin
  - The `AutoRotationConfigOption` enum is in the [`AutoRotationConfigOption` enum](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Enums.cs#L145)
   and must be copied over to your plugin for use with this method
    - You can safely pass in enum values that are not yet released; they will 
      just provide a warning you can ignore.
  - The `object` returned is of the type specified in the enum for the option
- `void SetAutoRotationConfigState(Guid, AutoRotationConfigOption, object)`
  - The `object` must be of the type specified in the enum for the option
  - Sets the state of an Auto-Rotation configuration option
  - Locks the state away from the user
  - Counts towards the lease's configuration limit

## Setting up the Wrath Combo IPC in your plugin

All examples use [ECommons'](https://github.com/NightmareXIV/ECommons/) EzIPC.

First you set up the IPC class for EzIPC to fill in abstract methods within:

```csharp
internal static class WrathIPC
{
    [EzIPC] private static readonly Func<string, string, string?, Guid?> RegisterForLeaseWithCallback;
    // + other methods signatures you want to use from Wrath's IPC
    // see https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Provider.cs
    // and `ProvideAutoRotConfig.cs` next to it, for all the methods and their signatures
}
```
`Func` for methods with a return, last parameter being the return type.
`Action` for methods without a return.

Then you need to actually initiate EzIPC, adding a field to the same class:

```csharp
private static EzIPCDisposalToken[] _disposalTokens =
    EzIPC.Init(typeof(WrathIPC), "WrathCombo", SafeWrapper.IPCException);
```

You can also choose to add a property with Wrath's IPC status:
    
```csharp
internal static bool IsEnabled =>
    DalamudReflector.TryGetDalamudPlugin("WrathCombo", out _, false, true);
```

And since the IPC works on a lease system, it would make sense to add a property for
that as well: (done here as a property that tries to register a lease)

```csharp
internal static Guid? CurrentLease
{
    get
    {
        field ??= RegisterForLease("internalPluginName", "My Plugin's proper name");
        if (field is null)
            PluginLog.Warning("Failed to register for lease. " +
                              "See logs from Wrath Como for why.");
        return field;
    }
}
```

And the IPC is set up and ready to use.

### Setting up with a callback

If you would like to use a callback, to be notified when and why your lease was
cancelled, you can instead use the `RegisterForLeaseWithCallback` method:

```csharp
[EzIPC] private static readonly Func<string, string, string?, Guid?> RegisterForLeaseWithCallback;
```

But of course this needs you to provide a callback method, through your own IPC:
```csharp
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
```

And finally we would register our lease differently if we want that callback to be
used:

```csharp
internal static Guid? CurrentLease
{
    get
    {
        field ??= RegisterForLeaseWithCallback(
            "internalPluginName",
            "My Plugin's proper name",
            "myPluginIPCPrefixJustForWrath" // this can also be null, if the prefix
                                            // is the same as the internal plugin name
        );
        if (field is null)
            PluginLog.Warning("Failed to register for lease. " +
                              "See logs from Wrath Como for why.");
        return field;
    }
}
```
See how AutoDuty does this [here, in `Register`]((https://github.com/ffxivcode/AutoDuty/blob/master/AutoDuty/IPC/IPCSubscriber.cs#L473)) (callback [here](https://github.com/ffxivcode/AutoDuty/blob/master/AutoDuty/IPC/IPCProvider.cs#L27) and [here](https://github.com/ffxivcode/AutoDuty/blob/master/AutoDuty/IPC/IPCSubscriber.cs#L488)).

## How to use the setup IPC

Once you are set up to work with the Wrath Combo IPC, it can be very simple if all
you are trying to do is make sure the user is set up to for Auto-Rotation:

```csharp
if (WrathIPC.IsEnabled)
{
    // enable Wrath Combo Auto-Rotation
    WrathIPC.SetAutoRotationState(WrathIPC.CurrentLease, true);
    // make sure the job is ready for Auto-Rotation
    WrathIPC.SetCurrentJobAutoRotationReady(WrathIPC.CurrentLease);
    // if the job is ready, all the user's settings are locked
    // if the job is not ready, it turns on the job's simple modes, or if those don't
    // exist, it turns on the job's advanced modes with all options enabled
}
```

Or you can make it more advanced, making sure Auto-Rotation settings are as you want
them to be.

This does require you to copy over the [`AutoRotationConfigOption` enum](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Enums.cs#L145).

```csharp
if (WrathIPC.IsEnabled)
{
    WrathIPC.SetAutoRotationState(WrathIPC.CurrentLease, true);
    WrathIPC.SetCurrentJobAutoRotationReady(WrathIPC.CurrentLease);
    WrathIPC.SetAutoRotationConfigState(WrathIPC.CurrentLease,
        AutoRotationConfigOption.InCombatOnly, false);
    WrathIPC.SetAutoRotationConfigState(WrathIPC.CurrentLease,
        AutoRotationConfigOption.AutoRez, true);
    WrathIPC.SetAutoRotationConfigState(WrathIPC.CurrentLease,
        AutoRotationConfigOption.SingleTargetHPP, 50);
}
```
See how AutoDuty does this, and to what extent, 
[here, in `SetAutoMode`](https://github.com/ffxivcode/AutoDuty/blob/master/AutoDuty/IPC/IPCSubscriber.cs#L451).

Lastly, you will need to release control when you are done, you are incentivized to
release control yourself so the user is not incentivized to revoke control from you:

```csharp
if (yourCodeNoLongerRequiresAutoRotation) // such as when the user disables your auto mode
    WrathIPC.ReleaseControl(WrathIPC.CurrentLease);
```

There are also a variety of `Get` methods in addition to `Set` ones shown here,
as well as many other `Set` methods such as for controlling individual Combo 
settings, however when `Set`ting, it also locks the value away from the user.
So in most cases, you will want to `Set`, without having to `Get` first.

For more information on the nuances of using the IPC, you can refer to the extra 
resources below, or the first several sections of this guide.

## Other resources for setting up and using the IPC

- Provider files ([main](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/Provider.cs),
  [auto-rot settings](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Services/IPC/ProvideAutoRotConfig.cs)) -
  These are the actual IPC methods provided by Wrath and have verbose doc comments.
- PunishXIV/WrathCombo#188 - this is where the IPC was introduced, and lists out the methods and some
  usage notes.
- ffxivcode/AutoDuty#714 - this is where the IPC was first implemented in another
  plugin (AutoDuty).

## Changelog

- PunishXIV/WrathCombo@3ef3109 - Methods with specific job parameters are now `uint`,
  `1.0.0.9`.
- PunishXIV/WrathCombo@5699d7b - Auto-Rotation Configurations enums are no longer a 
  subset of the full options, `1.0.0.9`.
- PunishXIV/WrathCombo@0d8faa7 - Added `IncludeNPCs` healer option to the 
  `AutoRotationConfigOption` 
  enum, `1.0.0.8`.
- PunishXIV/WrathCombo#232 - Fixed capability to request a cancellation callback via your own IPC 
  method, `1.0.0.7`.
- PunishXIV/WrathCombo#188 - Initial introduction of the IPC, `1.0.0.6`.

> [!TIP]
> You can keep up to date with the latest changes to the IPC by subscribing to the
> GitHub Atom feeds for this file. For example, you can do this very easily with 
> [Blogtrottr](https://blogtrottr.com/).\
> Here you can watch *this* file, for merged changes:
> ```
> https://github.com/PunishXIV/WrathCombo/commits/main/docs/IPC.md.atom
> ```
> or @zbee's branch for upcoming changes: (though this link is more likely to move)
> ```
> https://github.com/zbee/WrathCombo/commits/IPC/docs/IPC.md.atom
> ```
