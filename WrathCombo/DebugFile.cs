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
using System.Linq;
using System.Reflection;
using System.Text;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvP;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Data.Conflicts;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

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

            job = Svc.ClientState.LocalPlayer.ClassJob.Value;
        }

        using (_file = new StreamWriter(GetDebugFilePath(), append: false))
        {
            AddLine("START DEBUG LOG");
            AddLine();

            AddPluginInfo();
            AddIPCInfo();
            AddLine();
            AddConflictingInfo();
            AddLine();

            AddPlayerInfo();
            AddTargetInfo();

            AddSettingsInfo();
            AddAutoRotationInfo();

            AddFeatures(job);
            AddConfigs(job);
            AddStatusEffects();
            AddLine();

            AddRedundantIDs();
            AddLine();

            AddDebugCode();
            AddLogHistory();

            AddLine();
            AddLine("END DEBUG LOG");

            DuoLog.Information(
                "WrathDebug.txt created on your desktop, for " +
                (job is null ? "all jobs" : job.Value.Abbreviation.ToString()) +
                ". Upload this file where requested.\n" +
                "If you're unsure of where the file was created, use /wrath debug path");
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

        if (leaseesCount <= 0) return;

        AddLine("START IPC LEASES");
        foreach (var leasee in leasees)
            AddLine($"- {leasee.pluginName} ({leasee.configurationsCount} configs)");
        AddLine("END IPC LEASES");
    }

    private static void AddConflictingInfo()
    {
        var hasConflicts = ConflictingPlugins.TryGetConflicts(out var conflictsObj);
        var conflicts = conflictsObj.ToArray();
        var conflictingPluginsCount = conflicts.Length;

        AddLine($"Conflicting Plugins: {conflictingPluginsCount}");

        if (!hasConflicts) return;

        AddLine("START CONFLICTING PLUGINS");
        foreach (var plugin in conflicts)
            AddLine($"- {plugin.Name} v{plugin.Version} ({plugin.ConflictType}) " +
                    (string.IsNullOrEmpty(plugin.Reason)
                        ? ""
                        : "reason: " + plugin.Reason));
        AddLine("END CONFLICTING PLUGINS");
    }

    private static void AddPlayerInfo()
    {
        var player = Svc.ClientState.LocalPlayer;
        var job = player.ClassJob.Value;
        var currentZone = "Unknown";
        try
        {
            currentZone = Svc.Data.GetExcelSheet<TerritoryType>()
                .FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType)
                .PlaceName.Value.Name.ToString();
        }
        catch
        {
            // ignored
        }

        AddLine("START PLAYER INFO");
        AddLine($"Job: {job.Abbreviation} / {job.Name} / {job.NameEnglish}");
        AddLine($"Job ID: {job.RowId}");
        AddLine($"Level: {player.Level}");
        AddLine();
        AddLine($"Current Zone: {currentZone}");
        AddLine($"Current Party Size: {GetPartyMembers().Count}");
        AddLine();
        AddLine($"HP: {(player.CurrentHp / player.MaxHp * 100):F0}%");
        AddLine($"+Shield: {player.ShieldPercentage:F0}%");
        AddLine($"MP: {(player.CurrentMp / player.MaxMp * 100):F0}%");
        AddLine("END PLAYER INFO");

        AddLine();
    }

    private static void AddTargetInfo()
    {
        var target = Svc.ClientState.LocalPlayer.TargetObject;

        AddLine($"Target: {target?.GameObjectId.ToString() ?? "None"}");

        if (target is null) return;

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
        AddLine($"Is Hostile: {target.IsHostile()}");
        AddLine($"In Combat: {target.IsInCombat()}");
        AddLine($"Is Boss: {battleTarget.IsBoss()}");
        AddLine($"(In Boss Encounter: {InBossEncounter()})");
        AddLine($"Is Dead: {target.IsDead}");
        AddLine($"Distance: {GetTargetDistance(target):F1}y");
        AddLine($"Nameplate: {target.GetNameplateKind()}");
        if (battleTarget is not null)
        {
            AddLine($"IDs: entity:{battleTarget.EntityId}, " +
                    $"base/data:{battleTarget.BaseId}");
            AddLine($"Level: {battleTarget.Level}");
            AddLine($"Is Casting: {battleTarget.IsCasting}");
            AddLine($"Is Cast Interruptable: {battleTarget.IsCastInterruptible}");
            AddLine();
            AddLine($"HP: {(battleTarget.CurrentHp / battleTarget.MaxHp * 100):F0}%");
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
        AddLine("START SETTINGS INFO");
        AddLine($"Throttle: {Service.Configuration.Throttle}ms");
        AddLine($"Performance Mode: {(Service.Configuration.PerformanceMode ? "ON" : "OFF")}");
        AddLine($"Suppress Queued Actions: {(Service.Configuration.SuppressQueuedActions ? "ON" : "OFF")}");
        AddLine($"Block Spell on Move: {(Service.Configuration.BlockSpellOnMove ? "ON" : "OFF")}");
        AddLine($"Movement Delay: {Service.Configuration.MovementLeeway}s");
        AddLine($"Opener Timeout: {Service.Configuration.OpenerTimeout}s");
        AddLine($"Melee Offset: {Service.Configuration.MeleeOffset}y");
        AddLine($"Interrupt Delay: {Service.Configuration.InterruptDelay*100}%");
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
                        PluginLog.Debug(
                            $"Error printing AutoRotation property: {property.Name}");
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

                var line = $"{(int)preset} - {preset}";
                if (leaseesCount > 0)
                    if (P.UIHelper.PresetControlled(preset) is not null)
                        line += " (IPC)";
                if (preset.Attributes().AutoAction is not null)
                {
                    line += "  AUTO-MODE: ";
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

                var line = $"{(int)preset} - {preset}";
                if (leaseesCount > 0)
                    if (P.UIHelper.PresetControlled(preset) is not null)
                        line += " (IPC)";
                if (preset.Attributes().AutoAction is not null)
                {
                    line += "      AUTO-MODE: ";
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
            foreach (var config in PluginConfiguration.CustomIntValues)
                AddLine($"{config.Key.Trim()}: {config.Value}");

            AddLine();

            AddLine("---INT ARRAY VALUES---");
            foreach (var config in PluginConfiguration.CustomIntArrayValues)
                AddLine($"{config.Key.Trim()}: {string.Join(", ", config.Value)}");

            AddLine();

            AddLine("---FLOAT VALUES---");
            foreach (var config in PluginConfiguration.CustomFloatValues)
                AddLine($"{config.Key.Trim()}: {config.Value}");

            AddLine();

            AddLine("---BOOL VALUES---");
            foreach (var config in PluginConfiguration.CustomBoolValues)
                AddLine($"{config.Key.Trim()}: {config.Value}");

            AddLine();

            AddLine("---BOOL ARRAY VALUES---");
            foreach (var config in PluginConfiguration.CustomBoolArrayValues)
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
                    key = val1.GetType().BaseType.GetField("pName")
                        .GetValue(val1).ToString()!;
                }

                if (PluginConfiguration.CustomIntValues
                    .TryGetValue(key, out var intVal))
                {
                    AddLine($"{key}: {intVal}");
                    return;
                }

                if (PluginConfiguration.CustomFloatValues
                    .TryGetValue(key, out var floatVal))
                {
                    AddLine($"{key}: {floatVal}");
                    return;
                }

                if (PluginConfiguration.CustomBoolValues
                    .TryGetValue(key, out var boolVal))
                {
                    AddLine($"{key}: {boolVal}");
                    return;
                }

                if (PluginConfiguration.CustomBoolArrayValues
                    .TryGetValue(key, out var boolArrVal))
                {
                    AddLine($"{key}: {string.Join(", ", boolArrVal)}");
                    return;
                }

                if (PluginConfiguration.CustomIntArrayValues
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
        var playerID = Svc.ClientState.LocalPlayer.GameObjectId;
        var statusEffects = Svc.ClientState.LocalPlayer.StatusList;

        var statusEffectsCount = 0;
        foreach (var _ in statusEffects)
            statusEffectsCount++;

        AddLine($"Status Effects found: {statusEffectsCount} " +
                $"(max: {statusEffects.Count()})");

        if (statusEffectsCount <= 0) return;

        AddLine("START STATUS EFFECTS");
        foreach (var effect in statusEffects)
            AddLine(
                $"ID: {effect.StatusId}, " +
                $"STACKS: {effect.Param}, " +
                $"SOURCE: {(effect.SourceId == playerID ? "self" : effect
                    .SourceId)}, " +
                $"NAME: {GetStatusName(effect.StatusId)}");
        AddLine("END STATUS EFFECTS");
    }

    private static void AddRedundantIDs()
    {
        AddLine($"Redundant IDs found: {_redundantIDs.Length}");

        if (_redundantIDs.Length <= 0) return;

        AddLine("START REDUNDANT IDS");
        foreach (var id in _redundantIDs)
            AddLine(id.ToString());
        AddLine("END REDUNDANT IDS");
    }

    /// Get the debug code by itself.
    public static string GetDebugCode()
    {
        var bytes = Encoding.UTF8.GetBytes(
            JsonConvert.SerializeObject(Service.Configuration));
        return Convert.ToBase64String(bytes);
    }

    private static void AddDebugCode()
    {
        AddLine("START DEBUG CODE");
        AddLine(GetDebugCode());
        AddLine("END DEBUG CODE");
        AddLine();
    }

    private static void AddLogHistory()
    {
        AddLine($"Setting Changes log Count: {DebugLog.Count}");

        if (DebugLog.Count < 1) return;

        var logsCopy = DebugLog.ToList();
        logsCopy.Reverse();

        AddLine("START LOG HISTORY (most recent first)");
        AddLine(string.Join("\n", logsCopy));
        AddLine("END LOG HISTORY");
    }

    public static void AddLog(string log)
    {
        DebugLog.Add($"{DateTime.UtcNow} | {log}");
    }
}
