#region

using Dalamud.Plugin;
using ECommons;
using ECommons.EzIpcManager;
using ECommons.Reflection;
using System;

#endregion

namespace WrathCombo.Services.IPC_Subscriber;

public abstract class ReusableIPC : IDisposable
{
    private IDalamudPlugin? _plugin;
    public EzIPCDisposalToken[] DisposalTokens;
    public string PluginName;
    protected bool ReflectionNotIPC;
    public Version ValidVersion;

    protected ReusableIPC
    (string? pluginName,
        Version? validVersion = null,
        bool reflectionNotIPC = false)
    {
        if (string.IsNullOrWhiteSpace(pluginName))
            throw new ArgumentException("Plugin name cannot be null or empty.",
                nameof(pluginName));

        PluginName = pluginName;
        ValidVersion = validVersion ?? new Version(0, 0, 0, 0);
        ReflectionNotIPC = reflectionNotIPC;
        DisposalTokens = ReflectionNotIPC ? [] : EzIPC.Init(this, PluginName);
    }

    public bool IsEnabled =>
        InstalledVersion >= ValidVersion || // release version
        InstalledVersion == new Version(0, 0, 0, 0); // debug ver for some plugins

    protected bool PluginIsLoaded =>
        DalamudReflector.TryGetDalamudPlugin(
            PluginName, out _plugin, ignoreCache: true);

    protected IDalamudPlugin Plugin
    {
        get
        {
            if (PluginIsLoaded)
                return _plugin!;
            throw new InvalidOperationException(
                "Plugin is not loaded or does not exist. " +
                "(This should be used after a `PluginIsLoaded` check)");
        }
    }

    public Version InstalledVersion =>
        DalamudReflector.TryGetDalamudPlugin(PluginName, out var plugin,
            ignoreCache: true)
            ? plugin.GetType().Assembly.GetName().Version ?? new Version(0, 0, 0, 1)
            : new Version(0, 0, 0, 1); // no version found

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