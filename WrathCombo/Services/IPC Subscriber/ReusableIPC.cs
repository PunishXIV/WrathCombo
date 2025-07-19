#region

using System;
using ECommons;
using ECommons.EzIpcManager;
using ECommons.Reflection;

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

public abstract class ReusableIPC : IDisposable
{
    public EzIPCDisposalToken[] DisposalTokens;
    public string PluginName;
    public Version ValidVersion;

    protected ReusableIPC(string? pluginName, Version? validVersion = null)
    {
        if (string.IsNullOrWhiteSpace(pluginName))
            throw new ArgumentException("Plugin name cannot be null or empty.",
                nameof(pluginName));
        
        PluginName = pluginName;
        ValidVersion = validVersion ?? new Version(0, 0, 0, 0);
        DisposalTokens = EzIPC.Init(this, PluginName, SafeWrapper.None);
    }

    public bool IsEnabled =>
        InstalledVersion >= ValidVersion || // release version
        InstalledVersion == new Version(0, 0, 0, 0); // debug ver for some plugins

    public Version InstalledVersion =>
        (DalamudReflector.TryGetDalamudPlugin(PluginName, out var dalamudPlugin,
            false, true)
            ? dalamudPlugin.GetType().Assembly.GetName().Version!
            : new Version(0, 0, 0, 1)); // no version found

    public virtual void Dispose()
    {
        foreach (var token in DisposalTokens)
            try
            {
                token.Dispose();
            }
            catch (Exception ex)
            {
                ex.Log();
            }
    }
}