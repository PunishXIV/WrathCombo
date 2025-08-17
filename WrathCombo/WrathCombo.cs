using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Networking.Http;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ECommons;
using ECommons.Automation.LegacyTaskManager;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using Newtonsoft.Json.Linq;
using PunishLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WrathCombo.Attributes;
using WrathCombo.AutoRotation;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Data.Conflicts;
using WrathCombo.Services;
using WrathCombo.Services.IPC_Subscriber;
using WrathCombo.Services.IPC;
using WrathCombo.Window;
using WrathCombo.Window.Tabs;
namespace WrathCombo;

/// <summary> Main plugin implementation. </summary>
public sealed partial class WrathCombo : IDalamudPlugin
{
    internal static TaskManager? TM;
    internal readonly ConfigWindow ConfigWindow;
    private readonly MajorChangesWindow _majorChangesWindow;
    private readonly TargetHelper TargetHelper;
    internal static DateTime LastPresetDeconflictTime = DateTime.MinValue;
    internal static WrathCombo? P;
    private readonly WindowSystem ws;
    private static readonly SocketsHttpHandler httpHandler = new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        ConnectCallback = new HappyEyeballsCallback().ConnectCallback,
    };
    private readonly HttpClient httpClient = new(httpHandler) { Timeout = TimeSpan.FromSeconds(5) };
    private readonly IDtrBarEntry DtrBarEntry;
    internal Provider IPC;
    internal Search IPCSearch = null!;
    internal UIHelper UIHelper = null!;
    internal ActionRetargeting ActionRetargeting = new();
    internal MovementHook MoveHook;

    private readonly TextPayload starterMotd = new("[Wrath Message of the Day] ");
    private static Job? jobID;
    private static bool inInstancedContent;

    public static readonly List<Job> DisabledJobsPVE =
    [
        //Job.ADV,
        //Job.AST,
        //Job.BLM,
        //Job.BLU,
        //Job.BRD,
        //Job.DNC,
        //Job.DOL,
        //Job.DRG,
        //Job.DRK,
        //Job.GNB,
        //Job.MCH,
        //Job.MNK,
        //Job.NIN,
        //Job.PCT,
        //Job.PLD,
        //Job.RDM,
        //Job.RPR,
        //Job.SAM,
        //Job.SCH,
        //Job.SGE,
        //Job.SMN,
        //Job.VPR,
        //Job.WAR,
        //Job.WHM
    ];

    public static readonly List<Job> DisabledJobsPVP = [];

    public static Job? JobID
    {
        get => jobID;
        private set
        {
            if (jobID != value && value != null)
                UpdateCaches(jobID != null, false, jobID == null);
            jobID = value;
        }
    }

    public static void UpdateCaches
        (bool onJobChange, bool onTerritoryChange, bool firstRun)
    {
        TM.DelayNext(1000);
        TM.Enqueue(() =>
        {
            if (!Player.Available)
                return false;

            WrathOpener.SelectOpener();
            P.ActionRetargeting.ClearCachedRetargets();
            if (onJobChange)
                PvEFeatures.OpenToCurrentJob(true);
            if (onJobChange || firstRun)
            {
                Service.ActionReplacer.UpdateFilteredCombos();
                Svc.Framework.RunOnTick(Provider.BuildCachesAction());
                P.IPCSearch.UpdateActiveJobPresets();
                P.IPC.Leasing.SuspendLeases(CancellationReason.JobChanged);
            }

            if (onTerritoryChange)
            {
                if (Service.Configuration.RotationConfig.EnableInInstance && Content.InstanceContentRow?.RowId > 0 && !inInstancedContent)
                {
                    Service.Configuration.RotationConfig.Enabled = true;
                    inInstancedContent = true;
                }

                if (Service.Configuration.RotationConfig.DisableAfterInstance && Content.InstanceContentRow?.RowId == 0 && inInstancedContent)
                {
                    Service.Configuration.RotationConfig.Enabled = false;
                    inInstancedContent = false;
                }
            }

            return true;
        }, "UpdateCaches");
    }

    /// <summary> Initializes a new instance of the <see cref="WrathCombo"/> class. </summary>
    /// <param name="pluginInterface"> Dalamud plugin interface. </param>
    public WrathCombo(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        pluginInterface.Create<Service>();
        ECommonsMain.Init(pluginInterface, this, Module.All);
        PunishLibMain.Init(pluginInterface, "Wrath Combo");

        TM = new();
        RemoveNullAutos(); 
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup(Svc.SigScanner);
        MoveHook = new();
        PresetStorage.Init();

        Service.ComboCache = new CustomComboCache();
        Service.ActionReplacer = new ActionReplacer();
        ActionWatching.Enable();
        IPC = Provider.Init();
        ConflictingPluginsChecks.Begin();

        ConfigWindow = new ConfigWindow();
        _majorChangesWindow = new MajorChangesWindow();
        TargetHelper = new();
        ws = new();
        ws.AddWindow(ConfigWindow);
        ws.AddWindow(_majorChangesWindow);
        ws.AddWindow(TargetHelper);

        Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
        Svc.PluginInterface.UiBuilder.OpenMainUi += OnOpenMainUi;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        RegisterCommands();

        DtrBarEntry ??= Svc.DtrBar.Get("Wrath Combo");
        DtrBarEntry.OnClick = (_) =>
        {
            ToggleAutoRotation(!Service.Configuration.RotationConfig.Enabled);
        };
        DtrBarEntry.Tooltip = new SeString(
        new TextPayload("Click to toggle Wrath Combo's Auto-Rotation.\n"),
        new TextPayload("Disable this icon in /xlsettings -> Server Info Bar"));

        Svc.ClientState.Login += PrintLoginMessage;
        if (Svc.ClientState.IsLoggedIn) ResetFeatures();

        Svc.Framework.Update += OnFrameworkUpdate;
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;

        if (DateTime.UtcNow - LastPresetDeconflictTime > TimeSpan.FromSeconds(3))
        {
            KillRedundantIDs();
            HandleConflictedCombos();
            LastPresetDeconflictTime = DateTime.UtcNow;
        }
        CustomComboFunctions.TimerSetup();

        // Starts Retarget list cleaning process after a delay
        Svc.Framework.RunOnTick(ActionRetargeting.ClearOldRetargets,
            TimeSpan.FromSeconds(60));

#if DEBUG
        ConfigWindow.IsOpen = true;
        Svc.Framework.RunOnTick(() =>
        {
            if (Service.Configuration.OpenToCurrentJob && Player.Available)
                HandleOpenCommand([""], forceOpen:true);
        });
#endif
    }

    private void RemoveNullAutos()
    {
        try
        {
            var save = false;
            if (!Svc.PluginInterface.ConfigFile.Exists) return;

            var json = JObject.Parse(File.ReadAllText(Svc.PluginInterface.ConfigFile.FullName));
            if (json["AutoActions"] is JObject autoActions)
            {
                var clone = autoActions.JSONClone();
                foreach (var a in clone)
                {
                    if (a.Key == "$type")
                        continue;

                    if (Enum.TryParse(typeof(Preset), a.Key, out _))
                        continue;

                    Svc.Log.Debug($"Couldn't find {a.Key}");
                    autoActions[a.Key].Parent.Remove();
                    save = true;
                }
            }
            if (save)
                File.WriteAllText(Svc.PluginInterface.ConfigFile.FullName, json.ToString());
        }
        catch (Exception e)
        {
            e.Log();
        }
    }

    private void ClientState_TerritoryChanged(ushort obj)
    {
        UpdateCaches(false, true, false);

        Task.Run(StancePartner.CheckForIPCControl);
    }

    public const string OptionControlledByIPC =
        "(being overwritten by another plugin, check the setting in /wrath)";

    private static void HandleConflictedCombos()
    {
        var enabledCopy = Service.Configuration.EnabledActions.ToHashSet(); //Prevents issues later removing during enumeration
        foreach (var preset in enabledCopy)
        {
            if (!PresetStorage.IsEnabled(preset)) continue;

            var conflictingCombos = preset.GetAttribute<ConflictingCombosAttribute>();
            if (conflictingCombos == null) continue;

            foreach (var conflict in conflictingCombos.ConflictingPresets)
                if (PresetStorage.IsEnabled(conflict))
                    if (Service.Configuration.EnabledActions.Remove(conflict))
                    {
                        PluginLog.Debug($"Removed {conflict} due to conflict with {preset}");
                        Service.Configuration.Save();
                    }
        }
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        if (Player.Object is not null)
        {
            JobID = Player.Job;
            CustomComboFunctions.IsMoving(); //Hacky workaround to ensure it's always running
        }

        BlueMageService.PopulateBLUSpells();
        TargetHelper.Draw();
        AutoRotationController.Run();
        PluginConfiguration.ProcessSaveQueue();

        Service.Configuration.SetActionChanging();

        if (Player.Available && Player.IsDead)
            ActionRetargeting.Retargets.Clear();

        // Skip the IPC checking if hidden
        if (DtrBarEntry.UserHidden) return;

        var autoOn = IPC.GetAutoRotationState();
        var icon = new IconPayload(autoOn
            ? BitmapFontIcon.SwordUnsheathed
            : BitmapFontIcon.SwordSheathed);

        var text = autoOn ? ": On" : ": Off";
        if (!Service.Configuration.ShortDTRText && autoOn)
            text += $" ({P.IPCSearch.ActiveJobPresets} active)";
        var ipcControlledText =
            P.UIHelper.AutoRotationStateControlled() is not null
                ? " (Locked)"
                : "";

        var payloadText = new TextPayload(text + ipcControlledText);
        DtrBarEntry.Text = new SeString(icon, payloadText);
    }

    private static void KillRedundantIDs()
    {
        var redundantIDs = Service.Configuration.EnabledActions.Where(x => int.TryParse(x.ToString(), out _)).OrderBy(x => x).Cast<int>().ToList();
        foreach (var id in redundantIDs)
            Service.Configuration.EnabledActions.RemoveWhere(x => (int)x == id);

        Service.Configuration.Save();
    }

    private static void ResetFeatures()
    {
        // Enumerable.Range is a start and count, not a start and end.
        // Enumerable.Range(Start, Count)
        Service.Configuration.ResetFeatures("1.0.0.6_DNCRework", Enumerable.Range(4000, 150).ToArray());
        Service.Configuration.ResetFeatures("1.0.0.11_DRKRework", Enumerable.Range(5000, 200).ToArray()); 
        Service.Configuration.ResetFeatures("1.0.1.11_RDMRework", Enumerable.Range(13000, 999).ToArray());
        Service.Configuration.ResetFeatures("1.0.2.3_NINRework", Enumerable.Range(10000, 100).ToArray());
    }

    private void DrawUI()
    {
        _majorChangesWindow.Draw();
        ConfigWindow.Draw();
    }

    private void PrintLoginMessage()
    {
        Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => ResetFeatures());

        if (!Service.Configuration.HideMessageOfTheDay)
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => PrintMotD());
    }

    private void PrintMotD()
    {
        try
        {
            var basicMessage = $"Welcome to WrathCombo v{this.GetType().Assembly
                .GetName().Version}!";
            using var motd =
                httpClient.GetAsync("https://raw.githubusercontent.com/PunishXIV/WrathCombo/main/res/motd.txt").Result;
            motd.EnsureSuccessStatusCode();
            var data = motd.Content.ReadAsStringAsync().Result;
            List<Payload> payloads =
            [
                starterMotd,
                EmphasisItalicPayload.ItalicsOn,
                string.IsNullOrEmpty(data) ? new TextPayload(basicMessage) : new TextPayload(data.Trim()),
                EmphasisItalicPayload.ItalicsOff
            ];

            Svc.Chat.Print(new XivChatEntry
            {
                Message = new SeString(payloads),
                Type = XivChatType.Echo
            });
        }

        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Unable to retrieve MotD");
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used for non-static only window initialization")]
    public string Name => "Wrath Combo";

    /// <inheritdoc/>
    public void Dispose()
    {
        ActionRetargeting.Dispose();
        ConfigWindow.Dispose();
        Debug.Dispose();

        // Try to force a config save if there are some pending
        if (PluginConfiguration.SaveQueue.Count > 0)
            lock (PluginConfiguration.SaveQueue)
            {
                PluginConfiguration.SaveQueue.Clear();
                Service.Configuration.Save();
                PluginConfiguration.ProcessSaveQueue();
            }

        ws.RemoveAllWindows();
        Svc.DtrBar.Remove("Wrath Combo");
        Svc.Framework.Update -= OnFrameworkUpdate;
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
        Svc.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.Draw -= DrawUI;
        
        Service.ActionReplacer.Dispose();
        Service.ComboCache.Dispose();
        ActionWatching.Dispose();
        CustomComboFunctions.TimerDispose();
        IPC.Dispose();
        MoveHook?.Dispose();

        ConflictingPluginsChecks.Dispose();
        AllStaticIPCSubscriptions.Dispose();
        Svc.ClientState.Login -= PrintLoginMessage;
        ECommonsMain.Dispose();
        P = null;
    }

    private void OnOpenMainUi() =>
        HandleOpenCommand(forceOpen: true);

    internal void OnOpenConfigUi() =>
        HandleOpenCommand(tab: OpenWindow.Settings, forceOpen: true);
}
