#region

using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Logging;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using ECommons.Reflection;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Data.Conflicts;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using ECommons;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using UIConfig = Dalamud.Game.Config.UiConfigOption;
using UIControl = Dalamud.Game.Config.UiControlOption;
using SysConfig = Dalamud.Game.Config.SystemConfigOption;

#endregion

namespace WrathCombo;

public static class DebugFile
{
    /// <summary>
    ///     The path to the desktop.
    /// </summary>
    private static readonly string? DesktopPath =
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    /// <summary>
    ///     The file to write to.
    /// </summary>
    private static StreamWriter _file = null!;

    /// <summary>
    ///     The redundant IDs found.
    /// </summary>
    private static int[] _redundantIDs = [];

    /// <summary>
    ///     Shortcut method to add a line to the debug file.
    /// </summary>
    /// <param name="line">
    ///     The text of the line to be added.<br />
    ///     Defaults to an empty string.
    /// </param>
    private static void AddLine(string line = "") => _file.WriteLine(line);

    /// <summary>
    /// List of many things the user has done in the current session (Toggles, config changes etc.)
    /// </summary>
    internal static List<string> DebugLog = [];

    internal static void LoggingConfigChanges
        (object? _, Configuration.ConfigChangeEventArgs argument) {
        var displayValue = argument.NewValue switch
        {
            Array arr => string.Join(", ", arr.Cast<object?>()),
            _         => argument.NewValue.ToString(),
        };
        
        DebugLog.Add($"{DateTime.Now} | [{argument.Source}] " +
                     $"Changed {argument.Type} `{argument.Key}` to " +
                     $"'{displayValue}'");
    }

    /// Get the path to the debug file.
    public static string GetDebugFilePath() {
        var separator = DesktopPath?.Contains('\\') == true ? "\\" : "/";
        return $"{DesktopPath}{separator}WrathDebug.txt";
    }

    /// <summary>
    ///     Makes a debug file on the desktop.
    /// </summary>
    /// <param name="job">
    ///     The job to filter the debug file by, or none.<br />
    ///     Defaults to <see langword="null" />.
    /// </param>
    /// <param name="allJobs">
    ///     Whether the debug file should be targeted at all jobs, not a specific
    ///     one.<br/>
    ///     Overrides the <paramref name="job"/> parameter if set to
    ///     <see langword="true" />.<br/>
    ///     Defaults to <see langword="false" />.
    /// </param>
    public static void MakeDebugFile(ClassJob? job = null, bool allJobs = false)
    {
        _redundantIDs = [];

        if (allJobs)
            job = null;
        else if (job is null)
        {
            if (!Player.Available)
            {
                DuoLog.Error("Player data not currently available");
                throw new InvalidOperationException();
            }

            job = Player.Object.ClassJob.Value;
        }

        using (_file = new StreamWriter(GetDebugFilePath(), append: false))
        {
            AddLine("START DEBUG LOG");
            AddLine();
            
            AddLine($"Log at: {DateTimeOffset.Now:yyyy-MM-ddTHH:mm:sszzz}     " +
                    $"(everything in user's timezone)");
            AddLine();

            AddPluginInfo();
            AddIPCInfo();
            AddConflictingInfo();

            AddPlayerInfo();
            AddTargetInfo();

            AddSettingsInfo();
            AddAutoRotationInfo();

            AddFeatures(job);
            AddConfigs(job);
            AddStatusEffects();

            AddRedundantIDs();

            AddDebugCode();
            AddSettingsHistory();
            AddPluginList();
            AddDalamudLog();

            AddLine("END DEBUG LOG");

            DuoLog.Information(
                "WrathDebug.txt created on your desktop, for " +
                (job is null ? "all jobs" : job.Value.Abbreviation.ToString()) +
                ". Upload this file where requested.\n" +
                "If you're unsure of where the file was created, use: /wrath debug path");
        }
    }

    private static void AddPluginInfo()
    {
        var repo = RepoCheckFunctions.FetchCurrentRepo()?.InstalledFromUrl
                   ?? "Unknown";

        AddLine($"Plugin Version: {Svc.PluginInterface.Manifest.AssemblyVersion}");
        AddLine($"Installation Repo: {repo}");

        AddLine();
    }

    private static void AddIPCInfo()
    {
        var leaseesCount = P.UIHelper.ShowNumberOfLeasees();
        var leasees = P.UIHelper.ShowLeasees();

        AddLine($"Plugins controlling via IPC: {leaseesCount}");

        if (leaseesCount <= 0)
        {
            AddLine();
            return;
        }

        AddLine("START IPC LEASES");
        foreach (var leasee in leasees)
            AddLine($"- {leasee.pluginName} ({leasee.configurationsCount} configs)");
        AddLine("END IPC LEASES");

        AddLine();
    }

    private static void AddConflictingInfo()
    {
        var hasConflicts = ConflictingPlugins
            .TryGetConflicts(out var conflictsObj, true);
        var conflicts = conflictsObj.ToArray();
        var conflictingPluginsCount = conflicts.Length;

        AddLine($"Conflicts: {conflictingPluginsCount}");

        if (!hasConflicts)
        {
            AddLine();
            return;
        }

        AddLine("START CONFLICTS");
        foreach (var plugin in conflicts)
            AddLine($"- {plugin.Name} {plugin.Version} ({plugin.ConflictType}) " +
                    (string.IsNullOrEmpty(plugin.Reason)
                        ? ""
                        : "reason: " + plugin.Reason.Split("    ")[0]));
        AddLine("END CONFLICTS");

        AddLine();
    }

    private static void AddPlayerInfo()
    {
        var player = Player.Object;
        var job = player.ClassJob.Value;
        var currentZone = Content.ContentName ?? "Unknown";

        AddLine("START PLAYER INFO");
        AddLine($"Job: {job.Abbreviation} / {job.Name} / {job.NameEnglish}");
        AddLine($"Job ID: {job.RowId}");
        AddLine($"Level: {player.Level}");
        AddLine();
        AddLine($"Current Zone: {currentZone}");
        AddLine($"Current Party Size: {GetPartyMembers().Count}");
        AddLine();
        AddLine($"HP: {(float)player.CurrentHp / player.MaxHp * 100:F0}%");
        AddLine($"+Shield: {player.ShieldPercentage:F0}%");
        AddLine($"MP: {(float)player.CurrentMp / player.MaxMp * 100:F0}%");
        AddLine("END PLAYER INFO");

        AddLine();
    }

    private static void AddTargetInfo()
    {
        var target = Player.Object.TargetObject;

        AddLine($"Target: {target?.GameObjectId.ToString() ?? "None"}");

        if (target is null)
        {
            AddLine();
            return;
        }

        bool? failedSheetFind = null;
        IBattleChara? battleTarget = null;
        BNpcBase? battleNPCRow = null;
        if (target is IBattleChara)
        {
            battleTarget = target as IBattleChara;
            if (ActionWatching.BNPCSheet.TryGetValue(battleTarget.BaseId,
                    out var sheetRow))
            {
                battleNPCRow = sheetRow;
                failedSheetFind = false;
            }
            else
                failedSheetFind = true;
        }

        AddLine("START TARGET INFO");
        AddLine($"IDs: (<entity>/<data or base>): {target?.EntityId} / {target?.BaseId}");
        AddLine($"Is Friendly: {target.IsFriendly()}");
        AddLine($"Is Hostile: {target.IsHostile()}");
        AddLine($"In Combat: {target.IsInCombat()}");
        AddLine($"Is Boss: {battleTarget.IsBoss()}");
        AddLine($"(In Boss Encounter: {InBossEncounter()})");
        AddLine($"Is Dead: {target.IsDead}");
        AddLine($"Distance: {GetTargetDistance(target):F1}y");
        AddLine($"Nameplate: {target.GetNameplateKind()}");
        AddLine($"Name ID: {target.GetNameId()}");
        if (battleTarget is not null)
        {
            AddLine($"IDs: entity:{battleTarget.EntityId}, " +
                    $"base/data:{battleTarget.BaseId}");
            AddLine($"Level: {battleTarget.Level}");
            AddLine($"Is Casting: {battleTarget.IsCasting}");
            AddLine($"Is Cast Interruptable: {battleTarget.IsCastInterruptible}");
            AddLine();
            AddLine($"HP: {((float)battleTarget.CurrentHp / battleTarget.MaxHp * 100):F0}%");
            AddLine($"+Shield: {battleTarget.ShieldPercentage:F0}%");

            if (battleNPCRow is not null)
            {
                AddLine();
                AddLine($"Rank: {battleNPCRow.Value.Rank}");
            }
            if (failedSheetFind is not null)
            {
                AddLine($"Found Sheet: {(failedSheetFind is true ? "no" : "yes")}");
            }
        }
        AddLine("END TARGET INFO");

        AddLine();
    }

    private static void AddSettingsInfo()
    {
        Dictionary<string, Dictionary<object, object>>
            settingsToDisplay = new()
            {
                // Section Name
                ["Rotation Behavior"] = new Dictionary<object, object>
                {
                    // Key in Settings           Alias for Setting
                    ["BlockSpellOnMove"]       = "Block Spell on Move",
                    ["ActionChanging"]         = "Action Replacing",
                    ["PerformanceMode"]        = "Performance Mode",
                    ["SuppressQueuedActions"]  = "Queued Action Suppression",
                    ["Throttle"]               = "Throttle (ms)",
                    ["MovementLeeway"]         = "Movement Delay (s)",
                    ["OpenerTimeout"]          = "Opener Timeout (s)",
                    ["MeleeOffset"]            = "Melee Offset (y)",
                    ["InterruptDelay"]         = "Interrupt/Stun Delay (%)",
                    ["MaximumWeavesPerWindow"] = "Maximum Weaves Per Window",
                },
                ["Targeting"] = new Dictionary<object, object>
                {
                    ["RetargetHealingActionsToStack"] = "Retarget Healing Actions",
                    ["CustomHealStack"]               = "Heal Stack",
                    ["RaiseStack"]                    = "Raise Stack",
                },
                ["XIV"] = new Dictionary<object, object>
                {
                    [UIControl.AutoFaceTargetOnAction] = "Auto Face Target",
                    [UIConfig.GroundTargetActionExcuteType] = "2x-Press Ground Actions",
                },
            };

        AddLine("START SETTINGS INFO");

        foreach (var (section, settings) in settingsToDisplay)
        {
            AddLine($"---{section}---");
            foreach (var (property, alias) in settings)
            {
                var value = Service.Configuration.GetFoP(property.ToString());
                if (value == null)
                {
                    #region Try to get FFXIV options
                    // Currently only supports boolean-compatibles.
                    // could have try-catch's into supporting ints,
                    // and that would cover most
                    // (note: we're only getting the values - the types only have to be compatible, not actually correct)
                    if (section == "XIV")
                    {
                        try
                        {
                            switch (property)
                            {
                                case UIControl opt:
                                {
                                    if (Svc.GameConfig.TryGet(opt, out bool gameVal))
                                        value = gameVal;
                                    break;
                                }
                                case UIConfig opt:
                                {
                                    if (Svc.GameConfig.TryGet(opt, out bool gameVal))
                                        value = gameVal;
                                    break;
                                }
                                case SysConfig opt:
                                {
                                    if (Svc.GameConfig.TryGet(opt, out bool gameVal))
                                        value = gameVal;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            value = null;
                        }
                    }
                    #endregion

                    if (value == null)
                    {
                        PluginLog.Information(
                            $"failed to get setting: {section}.{property}");
                        continue;
                    }
                }

                var displayValue = property switch
                {
                    "InterruptDelay" => $"{(float)value * 100}",
                    "CustomHealStack" => Service.Configuration.UseCustomHealStack
                        .DisplayStack(separator: " > "),
                    "RaiseStack" => ((string[])value).StackString(" > ", true),
                    _ when value is bool b => b ? "ON" : "OFF",
                    _ => value.ToString(),
                };

                AddLine($"{alias}: {displayValue}");
            }

            if (!section.Equals(settingsToDisplay.Keys.Last()))
                AddLine();
        }

        AddLine("END SETTINGS INFO");

        AddLine();
    }

    private static void AddAutoRotationInfo()
    {
        var config = new
            AutoRotationConfigIPCWrapper(Service.Configuration.RotationConfig);

        AddLine("START AUTO ROTATION INFO");
        AddLine($"Auto Rotation Enabled: {P.IPC.GetAutoRotationState()}");
        PrintConfigProperties(config);
        AddLine("END AUTO ROTATION INFO");

        AddLine();

        return;

        void PrintConfigProperties(object obj, string prefix = "")
        {
            foreach (var property in obj.GetType()
                         .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(obj);
                if (value != null && (value.GetType().IsClass && value.GetType() != typeof(string)))
                {
                    PrintConfigProperties(value, $"{prefix}{property.Name}.");
                }
                else
                {
                    try
                    {
                        var controlled =
                            property.Name == "Enabled"
                                ? P.UIHelper.AutoRotationStateControlled() is not
                                    null
                                : P.UIHelper.AutoRotationConfigControlled(
                                    property.Name) is not null;
                        var ctrlText = controlled ? " (IPC)" : "";
                        AddLine($"{prefix}{property.Name}: {value} {ctrlText}");
                    }
                    catch
                    {
                        AddLine($"{prefix}{property.Name}: {value}");
                    }
                }
            }
        }
    }

    private static void AddFeatures(ClassJob? job = null)
    {
        var leaseesCount = P.UIHelper.ShowNumberOfLeasees();

        AddLine("START ENABLED FEATURES");
        if (job is null)
        {
            foreach (var preset in P.IPCSearch.EnabledActions.OrderBy(x => x))
            {
                if (int.TryParse(preset.ToString(), out _))
                {
                    _redundantIDs = _redundantIDs.Append((int)preset).ToArray();
                    continue;
                }

                var line = $"{(int)preset} - {preset}".PadRight(70);
                if (leaseesCount > 0)
                    if (P.UIHelper.PresetControlled(preset) is not null)
                        line += " (IPC)";
                if (preset.Attributes().AutoAction is not null)
                {
                    line += " AUTO-MODE: ";
                    line += P.IPCSearch.AutoActions[preset] ? "ON" : "OFF";
                    if (leaseesCount > 0)
                        if (Service.Configuration.AutoActions[preset] !=
                            P.IPCSearch.AutoActions[preset])
                            line += " (IPC)";
                }
                AddLine(line);
            }
        }
        else
        {
            foreach (var preset in P.IPCSearch.EnabledActions.OrderBy(x => x))
            {
                if (int.TryParse(preset.ToString(), out _))
                {
                    _redundantIDs = _redundantIDs.Append((int)preset).ToArray();
                    continue;
                }

                var prefix = preset.ToString()[..3].ToLowerInvariant();
                if (prefix != job.Value.Abbreviation.ToString().ToLowerInvariant() &&
                    prefix != "all" &&
                    prefix != "pvp")
                    continue;

                var line = $"{(int)preset} - {preset}".PadRight(70);
                if (leaseesCount > 0)
                    if (P.UIHelper.PresetControlled(preset) is not null)
                        line += " (IPC)";
                if (preset.Attributes().AutoAction is not null)
                {
                    line += " AUTO-MODE: ";
                    line += P.IPCSearch.AutoActions[preset] ? "ON" : "OFF";
                    if (leaseesCount > 0)
                        if (P.UIHelper.PresetControlled(preset) is not null)
                            line += " (IPC)";
                }
                AddLine(line);
            }
        }

        AddLine("END ENABLED FEATURES");

        AddLine();
    }

    private static void AddConfigs(ClassJob? job = null)
    {
        AddLine("START CONFIG SETTINGS");
        if (job is null)
        {
            AddLine("---INT VALUES---");
            foreach (var config in Configuration.CustomIntValues)
                AddLine($"{config.Key.Trim()}: {config.Value}");

            AddLine();

            AddLine("---INT ARRAY VALUES---");
            foreach (var config in Configuration.CustomIntArrayValues)
                AddLine($"{config.Key.Trim()}: {string.Join(", ", config.Value)}");

            AddLine();

            AddLine("---FLOAT VALUES---");
            foreach (var config in Configuration.CustomFloatValues)
                AddLine($"{config.Key.Trim()}: {config.Value}");

            AddLine();

            AddLine("---BOOL VALUES---");
            foreach (var config in Configuration.CustomBoolValues)
                AddLine($"{config.Key.Trim()}: {config.Value}");

            AddLine();

            AddLine("---BOOL ARRAY VALUES---");
            foreach (var config in Configuration.CustomBoolArrayValues)
                AddLine($"{config.Key.Trim()}: {string.Join(", ", config.Value)}");
        }
        else
        {
            void PrintConfig(MemberInfo? config)
            {
                var key = config.Name;

                var field = config.ReflectedType.GetField(key);
                var val1 = field.GetValue(null);
                if (val1.GetType().BaseType == typeof(UserData))
                {
                    key = val1.GetType().BaseType.GetField("ConfigName")
                        .GetValue(val1).ToString()!;
                }

                if (Configuration.CustomIntValues
                    .TryGetValue(key, out var intVal))
                {
                    AddLine($"{key}: {intVal}");
                    return;
                }

                if (Configuration.CustomFloatValues
                    .TryGetValue(key, out var floatVal))
                {
                    AddLine($"{key}: {floatVal}");
                    return;
                }

                if (Configuration.CustomBoolValues
                    .TryGetValue(key, out var boolVal))
                {
                    AddLine($"{key}: {boolVal}");
                    return;
                }

                if (Configuration.CustomBoolArrayValues
                    .TryGetValue(key, out var boolArrVal))
                {
                    AddLine($"{key}: {string.Join(", ", boolArrVal)}");
                    return;
                }

                if (Configuration.CustomIntArrayValues
                    .TryGetValue(key, out var intArrVal))
                {
                    AddLine($"{key}: {string.Join(", ", intArrVal)}");
                    return;
                }

                AddLine($"{key}: NOT SET");
            }

            #region Config Type Switch

            var configType = job.Value.RowId switch
            {
                1 or 19 => typeof(PLD.Config),
                2 or 20 => typeof(MNK.Config),
                3 or 21 => typeof(WAR.Config),
                4 or 22 => typeof(DRG.Config),
                5 or 23 => typeof(BRD.Config),
                6 or 24 => typeof(WHM.Config),
                7 or 25 => typeof(BLM.Config),
                26 or 27 => typeof(SMN.Config),
                28 => typeof(SCH.Config),
                29 or 30 => typeof(NIN.Config),
                31 => typeof(MCH.Config),
                32 => typeof(DRK.Config),
                33 => typeof(AST.Config),
                34 => typeof(SAM.Config),
                35 => typeof(RDM.Config),
                //36 => typeof(BLU.Config),
                37 => typeof(GNB.Config),
                38 => typeof(DNC.Config),
                39 => typeof(RPR.Config),
                40 => typeof(SGE.Config),
                41 => typeof(VPR.Config),
                42 => typeof(PCT.Config),
                _ => throw new NotImplementedException(),
            };

            #endregion

            foreach (var config in configType.GetMembers()
                         .Concat(typeof(PvPCommon.Config).GetMembers())
                         .Where(x => x.MemberType is
                             MemberTypes.Field or MemberTypes.Property))
                PrintConfig(config);
        }

        AddLine("END CONFIG SETTINGS");

        AddLine();
    }

    private static void AddStatusEffects()
    {
        var playerID = Player.Object.GameObjectId;
        var statusEffects = Player.Object.StatusList;

        var statusEffectsCount = 0;
        foreach (var _ in statusEffects)
            statusEffectsCount++;

        AddLine($"Status Effects found: {statusEffectsCount} " +
                $"(max: {statusEffects.Count()})");

        if (statusEffectsCount <= 0)
        {
            AddLine();
            return;
        }

        AddLine("START STATUS EFFECTS");
        foreach (var effect in statusEffects)
            AddLine(
                $"ID: {effect.StatusId}, " +
                $"STACKS: {effect.Param}, " +
                $"SOURCE: {(effect.SourceId == playerID ? "self" : effect
                    .SourceId)}, " +
                $"NAME: {GetStatusName(effect.StatusId)}");
        AddLine("END STATUS EFFECTS");

        AddLine();
    }

    private static void AddRedundantIDs()
    {
        AddLine($"Redundant IDs found: {_redundantIDs.Length}");

        if (_redundantIDs.Length <= 0)
        {
            AddLine();
            return;
        }

        AddLine("START REDUNDANT IDS");
        foreach (var id in _redundantIDs)
            AddLine(id.ToString());
        AddLine("END REDUNDANT IDS");
        AddLine();
    }

    /// Get the debug code by itself.
    public static string GetDebugCode()
    {
        var json  = JsonConvert.SerializeObject(
            Service.Configuration, Formatting.None);
        var bytes = Encoding.UTF8.GetBytes(json);

        byte[] compressed;
        try
        {
            // Compress the config
            using var output = new MemoryStream();
            using (var brotli = new BrotliStream(output,
                       CompressionLevel.SmallestSize, leaveOpen: true))
            {
                brotli.Write(bytes);
            }

            output.Position = 0;
            compressed      = output.ToArray();
        }
        catch
        {
            // If compression fails, fall back to uncompressed
            compressed = bytes;
        }

        // Base64 encode the compressed config
        return Convert.ToBase64String(compressed);
    }

    private static void AddDebugCode()
    {
        AddLine("START DEBUG CODE");
        AddLine(GetDebugCode());
        AddLine("END DEBUG CODE");
        AddLine();
    }

    private static void AddSettingsHistory()
    {
        AddLine($"Setting Changes History Count: {DebugLog.Count}");

        if (DebugLog.Count < 1)
        {
            AddLine();
            return;
        }

        var logsCopy = DebugLog.ToList();
        logsCopy.Reverse();

        AddLine("START SETTINGS CHANGES HISTORY (most recent first)");
        AddLine(string.Join("\n", logsCopy));
        AddLine("END SETTINGS CHANGES HISTORY");

        AddLine();
    }

    private static void AddPluginList()
    {
        var plugins = Svc.PluginInterface.InstalledPlugins
            .Where(x => x.IsLoaded).ToArray();
        AddLine($"Total Plugins Loaded: {plugins.Length}");

        AddLine("START PLUGIN LIST");
        foreach (var plugin in plugins)
        {
            AddLine($"- {plugin.InternalName} ({plugin.Name}) v{plugin.Version}");
        }
        AddLine("END PLUGIN LIST");

        AddLine();
    }

    /// <summary>
    ///     Gets the last lines of the dalamud log file.
    /// </summary>
    /// <param name="count">
    ///     How many lines to get.
    /// </param>
    /// <param name="onlyOurs">
    ///     Whether to filter the lines to those that mention the plugin.
    /// </param>
    /// <returns>
    ///     A list of the last lines of the debug file.
    /// </returns>
    /// <remarks>
    ///     Complexity is to handle multi-line log entries correctly and to avoid
    ///     trying to load the entire file into memory.
    /// </remarks>
    public static List<string> GetLastLogs(int count = 100, bool onlyOurs = false)
    {
        try
        {
            var parent = Svc.PluginInterface.ConfigDirectory.Parent.Parent;
            if (parent is null)
            {
                PluginLog.Error(
                    $"Failed to locate dalamud.log folder: no parent directory ({parent})");
                return new List<string>();
            }
            var path = Path.Combine(parent.FullName, "dalamud.log");
            if (!File.Exists(path))
            {
                PluginLog.Error(
                    $"Failed to locate dalamud.log: file does not exist ({path})");
                return new List<string>();
            }

            var mergedEntries = new List<string>();
            var currentEntry = new StringBuilder();
            var foundEntries = 0;

            const int bufferSize = 4096;
            using var stream = new FileStream(path, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                bufferSize,
                FileOptions.SequentialScan);
            var buffer = new byte[bufferSize];
            var leftOver = string.Empty;

            var headerRegex = new System.Text.RegularExpressions.Regex(
                @"^(?:\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}|\d{2}:\d{2}:\d{2}\.\d{3})",
                System.Text.RegularExpressions.RegexOptions.Compiled);

            var stop = false;
            for (long i = stream.Length; i > 0; )
            {
                var readSize = (int)Math.Min(bufferSize, i);
                i -= readSize;
                stream.Position = i;
                stream.ReadExactly(buffer, 0, readSize);

                var chunk = Encoding.UTF8.GetString(buffer, 0, readSize) + leftOver;
                var lines = chunk.Split('\n');

                if (i > 0)
                {
                    // first line is likely partial (cut), save it for the next iteration
                    leftOver = lines[0];
                    lines = lines.Skip(1).ToArray();
                }
                else
                {
                    leftOver = string.Empty;
                }

                // Process chunk lines in reverse (from file end backwards)
                for (var j = lines.Length - 1; j >= 0; j--)
                {
                    var line = lines[j].TrimEnd('\r');
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var isHeader = headerRegex.IsMatch(line.TrimStart());
                    if (isHeader)
                    {
                        // Completed a log entry: currentEntry holds the lines that came after this header
                        if (currentEntry.Length > 0)
                        {
                            var entryText = currentEntry.ToString();
                            if (!onlyOurs || entryText.Contains("WrathCombo"))
                            {
                                mergedEntries.Add(entryText);
                                foundEntries++;
                                if (foundEntries >= count)
                                {
                                    stop = true;
                                    break;
                                }
                            }

                            currentEntry.Clear();
                        }

                        // Start the new current entry with this header line
                        currentEntry.Insert(0, line);
                    }
                    else
                    {
                        // Continuation line: prepend it to the currentEntry with a newline
                        if (currentEntry.Length > 0)
                        {
                            currentEntry.Insert(0, "\n");
                            currentEntry.Insert(0, line);
                        }
                        else
                        {
                            // If we haven't seen a header yet (very start of file), treat as part of current entry
                            currentEntry.Insert(0, line);
                        }
                    }
                }

                if (stop) break;
            }

            // After reading, ensure the most-recent (last) assembled entry is included if needed
            if (currentEntry.Length > 0 && foundEntries < count)
            {
                var entryText = currentEntry.ToString();
                if (!onlyOurs || entryText.Contains("WrathCombo"))
                {
                    mergedEntries.Add(entryText);
                }
            }

            return mergedEntries;
        }
        catch (Exception ex)
        {
            PluginLog.Error("Failed to read dalamud log: " + ex.ToStringFull());
            return new List<string>();
        }
    }

    private static void AddDalamudLog()
    {
        var logs = GetLastLogs(onlyOurs: true);

        if (logs.Count < 1)
        {
            AddLine("No Dalamud log entries found.");
            AddLine();
            return;
        }
        
        AddLine("START DALAMUD LOG HISTORY (most recent first)");
        AddLine(string.Join("\n", logs));
        AddLine("END DALAMUD LOG HISTORY");

        AddLine();
    }
}
