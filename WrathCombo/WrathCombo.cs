using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Networking.Http;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Automation.LegacyTaskManager;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using Lumina.Excel.Sheets;
using Newtonsoft.Json.Linq;
using PunishLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WrathCombo.API.Enum;
using WrathCombo.AutoRotation;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Data.BattleData;
using WrathCombo.Data.Conflicts;
using WrathCombo.Native;
using WrathCombo.Resources.Localization.UI.MainWindow;
using WrathCombo.Services;
using WrathCombo.Services.ActionRequestIPC;
using WrathCombo.Services.IPC;
using WrathCombo.Services.IPC_Subscriber;
using WrathCombo.Window;
using WrathCombo.Window.Tabs;
using GenericHelpers = ECommons.GenericHelpers;

namespace WrathCombo;

/// <summary> Main plugin implementation. </summary>
public sealed partial class WrathCombo : IAsyncDalamudPlugin
{
    internal static TaskManager? TM;
    internal ConfigWindow ConfigWindow = null!;
    private MajorChangesWindow _majorChangesWindow = null!;
    private TargetHelper TargetHelper = null!;
    internal static WrathCombo? P;
    private WindowSystem ws = null!;
    private static readonly SocketsHttpHandler httpHandler = new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        ConnectCallback = new HappyEyeballsCallback().ConnectCallback,
    };
    internal readonly HttpClient HTTPClient = new(httpHandler) { Timeout = TimeSpan.FromSeconds(5) };
    private IDtrBarEntry DtrBarEntry = null!;
    public IDtrBarEntry OpenerDtr = null!;
    internal Provider IPC = null!;
    internal Search IPCSearch = null!;
    internal UIHelper UIHelper = null!;
    internal ActionRetargeting ActionRetargeting = null!;
    internal MovementHook MoveHook = null!;
    internal CustomActionSetup CustomActions = null!;
    private readonly IDalamudPluginInterface pluginInterface;
    //private readonly CustomActionListAddon _listAddon;

    internal static bool IsAprilFools => DateTime.UtcNow.Day == 1 && DateTime.UtcNow.Month == 4;

    private readonly TextPayload starterMotd = new("[Wrath Message of the Day] ");
    private static Job? jobID;
    private static bool EnteringInstancedContent
    {
        get
        {
            return field;
        }
        set
        {
            if (field != value)
            {
                if (Service.Configuration.RotationConfig.EnableInInstance && value)
                    Service.Configuration.RotationConfig.Enabled = true;

                if (Service.Configuration.RotationConfig.DisableAfterInstance && !value)
                    Service.Configuration.RotationConfig.Enabled = false;

                field = value;
            }
        }
    }

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
        ActionRequestIPCProvider.ResetAllBlacklist();
        ActionRequestIPCProvider.ResetAllRequests();
        CustomComboFunctions.CleanupExpiredLineOfSightCache();
        TM.DelayNext(1000);
        TM.Enqueue(() =>
        {
            if (!Player.Available)
                return false;

            P.ActionRetargeting.ClearCachedRetargets();
            if (onJobChange)
                PvEFeatures.OpenToCurrentJob(true);
            if (onJobChange || firstRun)
            {
                WrathOpener.CurrentOpener?.CacheReady = false;
                WrathOpener.CurrentOpener?.ResetOpener(); //Clears opener values, just in case
                WrathOpener.SelectOpener();
                Service.ActionReplacer.UpdateFilteredCombos();
                Svc.Framework.RunOnTick(Provider.BuildCachesAction());
                P.IPCSearch.UpdateActiveJobPresets();
                P.IPC.Leasing.SuspendLeases(CancellationReason.JobChanged);
            }

            if (onTerritoryChange || firstRun)
            {
                if (Content.InstanceContentRow?.RowId > 0)
                    EnteringInstancedContent = true;
                else if (Content.InstanceContentRow?.RowId == 0)
                    EnteringInstancedContent = false;

                BattleData.LoadCombatData(Content.TerritoryID);
            }

            return true;
        }, "UpdateCaches");
    }

    /// <summary> Initializes a new instance of the <see cref="WrathCombo"/> class. </summary>
    /// <param name="pluginInterface"> Dalamud plugin interface. </param>
    public WrathCombo(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    /// <inheritdoc/>
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var loadSw = Stopwatch.StartNew();

        P = this;
        pluginInterface.Create<Service>();
        LogLoadStep("ECommons", () =>
            ECommonsMain.Init(pluginInterface, this, Module.VfxTracking, Module.DalamudReflector));

        TM = new();

        // Config parse (~120ms) overlaps PunishLib, ActionWatching.Init, CustomActions,
        // PresetStorage, and StatusCache. ActionWatching.Enable waits for config so
        // packet detours never see a null Configuration.
        var overlapSw = Stopwatch.StartNew();
        var configTask = TimedConfigLoad(cancellationToken);

        LogLoadStep("PunishLib", () =>
        {
            PunishLibMain.Init(pluginInterface, "Wrath Combo");
            ActionRequestIPCProvider.Initialize();
        });
        LogLoadStep("AddressResolver", () =>
        {
            Service.Address = new AddressResolver();
            Service.Address.Setup(Svc.SigScanner);
        });
        LogLoadStep("MovementHook", () => MoveHook = new());
        LogLoadStep("ActionWatching Init", () =>
        {
            ActionWatching.Instance = new ActionWatching();
            ActionWatching.Instance.Init();
        });

        var presetTask = Task.Run(() => LogLoadStep("PresetStorage", () =>
        {
            PresetStorage.InitializeDictionaries();
        }), cancellationToken);

        var statusTask = Task.Run(() => LogLoadStep("StatusCache", () =>
        {
            StatusCache.Dictionaries = new StatusDictionaries();
        }), cancellationToken);

        LogLoadStep("CustomActions", () => CustomActions = new());

        Service.Configuration = await configTask.ConfigureAwait(false);
        await presetTask.ConfigureAwait(false);
        await statusTask.ConfigureAwait(false);
        PresetStorage.RemoveRedundantPresets();
        ActionWatching.Instance.Enable();
        PluginLog.Information($"Overlapped init completed in {overlapSw.ElapsedMilliseconds} ms.");
        _ = OpCodeConfigHelper.UpdateOpCodesAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        LogLoadStep("ActionReplacer", () =>
        {
            Service.ComboCache = new CustomComboCache();
            Service.ActionReplacer = new ActionReplacer();
        });
        LogLoadStep("AutoRotation + Retarget + IPC", () =>
        {
            Service.AutoRotationController = new AutoRotationController();
            ActionRetargeting = new ActionRetargeting();
            IPC = Provider.Init();
            PingPluginIPC.Init();
            ConflictingPluginsChecks.Begin();
        });

        // Subscribe to language changes to update localized text if needed (Client != Selected UI)
        Svc.PluginInterface.LanguageChanged += Text.OnLanguageChanged;

        // Ensure startup culture matches Dalamud UI language
        var dalamudCulture = Svc.PluginInterface.UiLanguage.ToCulture();

        if (!Equals(CultureInfo.CurrentUICulture, dalamudCulture))
        {
            Text.OnLanguageChanged(Svc.PluginInterface.UiLanguage);
        }

        Settings.SanitiseSettings();
        _majorChangesWindow = new MajorChangesWindow();
        TargetHelper = new();
        ws = new();
        ws.AddWindow(_majorChangesWindow);
        ws.AddWindow(TargetHelper);

        Configuration.ConfigChanged += DebugFile.LoggingConfigChanges;

        Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
        Svc.PluginInterface.UiBuilder.OpenMainUi += OnOpenMainUi;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        RegisterCommands();

        DtrBarEntry = Svc.DtrBar.Get("Wrath Combo");
        DtrBarEntry.OnClick = (_) =>
        {
            AutoRotationController.ToggleAutoRotation(!Service.Configuration.RotationConfig.Enabled);
        };
        DtrBarEntry.Tooltip = new SeString(
        new TextPayload("Click to toggle Wrath Combo's Auto-Rotation.\n"),
        new TextPayload("Disable this icon in /xlsettings -> Server Info Bar"));

        OpenerDtr = Svc.DtrBar.Get("Wrath Combo Opener");

        OpenerDtr.OnClick += (_) =>
        {
            var preset = WrathOpener.CurrentOpener?.Preset;
            if (preset is not { } pre)
                return;

            PresetStorage.TogglePreset(pre);
        };

        OpenerDtr.Tooltip = new SeString(
        new TextPayload("Click to toggle Opener Preset.\n"),
        new TextPayload("Disable this icon in /xlsettings -> Server Info Bar"));

        Svc.ClientState.Login += PrintLoginMessage;
        if (Svc.ClientState.IsLoggedIn) ResetFeatures();

        Svc.Framework.Update += OnFrameworkUpdate;
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        Svc.Toasts.ErrorToast += OnErrorToast;

        CustomComboFunctions.TimerSetup();

        // Starts Retarget list cleaning process after a delay
        _ = Svc.Framework.RunOnTick(ActionRetargeting.ClearOldRetargets,
            TimeSpan.FromSeconds(60));

#if DEBUG
        VfxManager.Logging = true;
        _ = Svc.Framework.RunOnTick(() => HandleOpenCommand([""], forceOpen: true));
#endif
        PluginLog.Information($"LoadAsync completed in {loadSw.ElapsedMilliseconds} ms.");
    }

    private static void LogLoadStep(string name, System.Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        PluginLog.Information($"{name} completed in {sw.ElapsedMilliseconds} ms.");
    }

    private async Task<Configuration> TimedConfigLoad(CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var config = await LoadConfigurationAsync(cancellationToken).ConfigureAwait(false);
        PluginLog.Information($"Config load completed in {sw.ElapsedMilliseconds} ms.");
        return config;
    }

    private void OnErrorToast(ref SeString message, ref bool isHandled)
    {
        var txt = message.TextValue;
        if (Svc.Data.GetExcelSheet<LogMessage>().TryGetFirst(x => x.Text == txt, out var row))
        {
            if (row.RowId == 2288) //Aetherial Interference
            {
                if (AutoRotationController.cfg.Enabled)
                {
                    AutoRotationController.Paused = true;
                    DuoLog.Information($"Autorotation paused due to Aetherial Interference error. Will resume once party has left combat or autorotation is toggled off/on again.");
                }
            }
        }
    }

    /// <summary>
    /// Strip stale AutoActions keys, then deserialize once.
    /// Skips GetPluginConfig, which scans the assembly for a config type and reads the file again.
    /// </summary>
    private async Task<Configuration> LoadConfigurationAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Svc.PluginInterface.ConfigFile.Exists)
                return new Configuration();

            var json = JObject.Parse(await File.ReadAllTextAsync(Svc.PluginInterface.ConfigFile.FullName, cancellationToken).ConfigureAwait(false));
            if (json["AutoActions"] is JObject autoActions)
            {
                List<string>? staleKeys = null;
                foreach (var a in autoActions)
                {
                    if (a.Key == "$type")
                        continue;

                    if (Enum.TryParse(typeof(Preset), a.Key, out _))
                        continue;

                    Svc.Log.Debug($"Couldn't find {a.Key}");
                    staleKeys ??= [];
                    staleKeys.Add(a.Key);
                }

                if (staleKeys is not null)
                {
                    foreach (var key in staleKeys)
                        autoActions.Remove(key);

                    await File.WriteAllTextAsync(Svc.PluginInterface.ConfigFile.FullName, json.ToString(), cancellationToken).ConfigureAwait(false);
                }
            }

            return json.ToObject<Configuration>() ?? new Configuration();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            e.Log();
            return pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        }
    }

    private void ClientState_TerritoryChanged(uint territoryId)
    {
        UpdateCaches(false, true, false);
        Task.Run(StancePartner.CheckForIPCControl);
    }

    public const string OptionControlledByIPC =
        "(being overwritten by another plugin, check the setting in /wrath)";

    private void OnFrameworkUpdate(IFramework framework)
    {
        try
        {
            #region Checks that don't require the Player to be loaded

            Configuration.ProcessSaveQueue();

            //Hacky workaround to ensure it's always running
            CustomComboFunctions.IsMoving();

            #endregion

            // Skip Player-requiring code if not ready
            if (Player.Object is null ||
                !GenericHelpers.IsScreenReady() ||
                !Svc.ClientState.IsLoggedIn)
                return;

            #region Checks and Updates that require the Player

            JobID = Player.Job;

            PresetStorage.HandleCurrentConflicts();

            BlueMageService.PopulateBLUSpells();
            TargetHelper.Draw();

            AutoRotationController.Run();

            if (Player.IsDead)
            {
                ActionRetargeting.Retargets.Clear();
                CustomComboFunctions.CleanupExpiredLineOfSightCache();
            }

            #endregion

            #region DTR Bar Updating

            var autoOn = IPC.GetAutoRotationState();
            var icon = new IconPayload(autoOn
                ? BitmapFontIcon.SwordUnsheathed
                : BitmapFontIcon.SwordSheathed);

            var text = autoOn ? ": On" : ": Off";
            if (!Service.Configuration.ShortDTRText && autoOn)
                text += $" ({P.IPCSearch.ActiveJobPresets} active)";
            var ipcControlledText =
                P.UIHelper.AutoRotationStateControlled() is not null
                    ? "(Locked)"
                    : "";

            var pausedText =
                AutoRotationController.Paused ? "(Paused)" : "";

            var statusText = string.Join(" ", [text, ipcControlledText, pausedText]);

            var payloadText = new TextPayload(statusText);
            DtrBarEntry.Text = new SeString(icon, payloadText);

            #endregion

            if (Service.Configuration.ShowOpenerDtr)
            {
                var status = new TextPayload(WrathOpener.OpenerStatus());
                OpenerDtr.Text = new SeString(status);
                OpenerDtr.Shown = true;
            }
            else
                OpenerDtr.Shown = false;

            if (Service.Configuration.TankbusterTTS || Service.Configuration.TankbusterToast)
                CustomComboFunctions.PlayTankbusterAlert();

            if (Service.Configuration.AoEDamageTTS || Service.Configuration.AoEDamageToast)
                CustomComboFunctions.PlayGroupwideAlert();

            SimpleTargetState.ManageStateList();
        }
        catch (Exception ex)
        {
            ex.Log("Pls no crash game ty");
        }
    }

    private static void ResetFeatures()
    {
        // Enumerable.Range is a start and count, not a start and end.
        // Enumerable.Range(Start, Count)
        Service.Configuration.ResetFeatures("1.0.0.6_DNCRework", Enumerable.Range(4000, 150).ToArray());
        Service.Configuration.ResetFeatures("1.0.0.11_DRKRework", Enumerable.Range(5000, 200).ToArray());
        Service.Configuration.ResetFeatures("1.0.1.11_RDMRework", Enumerable.Range(13000, 999).ToArray());
        Service.Configuration.ResetFeatures("1.0.2.3_NINRework", Enumerable.Range(10000, 100).ToArray());
        Service.Configuration.ResetFeatures("1.0.4.21_SAMRework", Enumerable.Range(15000, 301).ToArray());
        Service.Configuration.ResetFeatures("1.0.4.21_SGERework", Enumerable.Range(14000, 100).ToArray());
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
            var basicMessage = $"Welcome to WrathCombo v{GetType().Assembly
                .GetName().Version}!";
            using var motd =
                HTTPClient.GetAsync("https://raw.githubusercontent.com/PunishXIV/WrathCombo/main/res/motd.txt").Result;
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
    public string Name => MainWindowUI.Wrath_Combo;

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        try
        {
            ActionRetargeting?.Dispose();
            ConfigWindow?.Dispose();
            Window.Tabs.Debug.Dispose();

            // Try to force a config save if there are some pending
            if (Service.Configuration is not null && Configuration.SaveQueue.Count > 0)
                lock (Configuration.SaveQueue)
                {
                    Configuration.SaveQueue.Clear();
                    Service.Configuration.Save();
                    Configuration.ProcessSaveQueue();
                }

            ws?.RemoveAllWindows();

            if (Svc.PluginInterface is not null)
            {
                Svc.DtrBar.Remove("Wrath Combo");
                Svc.DtrBar.Remove("Wrath Combo Opener");
                Configuration.ConfigChanged -= DebugFile.LoggingConfigChanges;
                Svc.Framework.Update -= OnFrameworkUpdate;
                Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
                Svc.PluginInterface.LanguageChanged -= Text.OnLanguageChanged;
                Svc.PluginInterface.UiBuilder.OpenMainUi -= OnOpenMainUi;
                Svc.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
                if (ws is not null)
                    Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
                Svc.Toasts.ErrorToast -= OnErrorToast;
                Svc.ClientState.Login -= PrintLoginMessage;
            }

            Service.ActionReplacer?.Dispose();
            Service.ComboCache?.Dispose();
            Service.AutoRotationController?.Dispose();
            ActionWatching.Instance?.Dispose();
            if (Svc.PluginInterface is not null)
                CustomComboFunctions.TimerDispose();
            IPC?.Dispose();
            MoveHook?.Dispose();
            CustomActions?.Dispose();

            ConflictingPluginsChecks.Dispose();
            AllStaticIPCSubscriptions.Dispose();
            if (Svc.PluginInterface is not null)
                ECommonsMain.Dispose();
        }
        catch (Exception e)
        {
            e.Log();
        }

        P = null;
        return ValueTask.CompletedTask;
    }

    private void OnOpenMainUi() =>
        HandleOpenCommand(forceOpen: true);

    internal void OnOpenConfigUi() =>
        HandleOpenCommand(tab: OpenWindow.Settings, forceOpen: true);

    private void EnsureConfigWindow()
    {
        if (ConfigWindow is not null)
            return;

        var sw = Stopwatch.StartNew();
        ConfigWindow = new ConfigWindow();
        ws.AddWindow(ConfigWindow);
        PluginLog.Information($"ConfigWindow created in {sw.ElapsedMilliseconds} ms.");
    }
}
