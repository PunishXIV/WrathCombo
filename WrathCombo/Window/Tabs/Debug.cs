#region Directives

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using WrathCombo.AutoRotation;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Services.IPC_Subscriber;
using WrathCombo.Services.IPC;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Action = Lumina.Excel.Sheets.Action;
using BattleNPCSubKind = Dalamud.Game.ClientState.Objects.Enums.BattleNpcSubKind;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using Status = Dalamud.Game.ClientState.Statuses.Status;

#endregion

namespace WrathCombo.Window.Tabs;

internal class Debug : ConfigWindow, IDisposable
{
    public static bool DebugConfig;
    private static int _sheetCustomId = 0;
    private static string _debugError = string.Empty;
    private static string _debugConfig = string.Empty;
    private static PluginConfiguration? _previousConfig;

    private static Guid? _wrathLease;
    private static Action? _debugSpell;
    private static SheetType? _sheetType;

    public enum SheetType
    {
        PvE,
        PvP,
        Bozja,
        Occult,
        Custom
    }

    // Constants
    private const int WidthStatusId = 8;
    private const int WidthStatusDuration = 11;
    private const float SpacingSmall = 10f;
    private const float SpacingMedium = 20f;
    private const string UnknownName = "???";
    private const string SymbolDuration = "";
    private const string SymbolParameter = "";

    internal new static unsafe void Draw()
    {
        ImGui.Text("This is where you can figure out where it all went wrong.");

        ImGuiEx.Spacing(new Vector2(0f, SpacingMedium));

        #region Debug Walking

        if (DebugConfig)
        {
            ImGuiEx.Text(ImGuiColors.HealerGreen, "You are now in config debug mode.");
            _debugError = "";
        }

        if (_debugError != "")
            ImGuiEx.Text(ImGuiColors.DalamudRed, _debugError);

        ImGui.Text("Debug Config: ");
        ImGui.SameLine();
        if (ImGui.InputText("##debugConfig", ref _debugConfig, 2000000))
        {
            try
            {
                var base64 = Convert.FromBase64String(_debugConfig);
                var decode = Encoding.UTF8.GetString(base64);
                var config = JsonConvert.DeserializeObject<PluginConfiguration>(decode);
                if (config != null)
                {
                    DebugConfig = true;
                    _previousConfig = Service.Configuration;
                    Service.Configuration = config;
                    P.IPC = Provider.Init();
                    AutoRotationController.cfg = null;
                    UpdateCaches(true, true, false);
                    _debugError = "";
                }
            }
            catch (Exception ex)
            {
                _debugError = "Error decoding configuration. Check Log.";
                PluginLog.Error($"Failed to read debug configuration.\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        ImGuiComponents.HelpMarker(
            "Paste a base64 encoded configuration here to load it into the plugin." +
            "\nThis comes from a debug file." +
            "\nThis will overwrite your current configuration temporarily, restoring your own configuration when you disable debug mode." +
            "\nDebug mode will also be disabled if you unload the plugin.");

        if (DebugConfig)
            if (ImGui.Button("Disable Debug Config Mode"))
                DisableDebugConfig();

        #endregion

        ImGuiEx.Spacing(new Vector2(0f, SpacingMedium));

        var target = Svc.Targets.Target;
        var player = Svc.ClientState.LocalPlayer;

        // Custom 2-Column Styling
        static void CustomStyleText(string firstColumn, object? secondColumn, bool useMonofont = false, Vector4? optionalColor = null)
        {
            ImGui.Columns(2, border: false);
            if (!string.IsNullOrEmpty(firstColumn))
            {
                ImGui.TextUnformatted(firstColumn);
            }

            ImGui.NextColumn();

            // Optional Color
            Vector4 textColor = optionalColor ?? ImGuiColors.DalamudGrey;
            ImGui.PushStyleColor(ImGuiCol.Text, textColor);

            // Optional Monofont
            if (useMonofont) ImGui.PushFont(UiBuilder.MonoFont);
            ImGui.TextUnformatted(secondColumn?.ToString() ?? string.Empty);
            if (useMonofont) ImGui.PopFont();

            ImGui.PopStyleColor();
            ImGui.Columns(1);
        }

        if (player is null)
        {
            ImGui.TextUnformatted("Please log into the game to use this tab.");
            return;
        }

        #region Statuses

        ImGui.Text("Status Effects");
        ImGui.Separator();

        if (ImGui.CollapsingHeader("Player Statuses"))
        {
            foreach (Status? status in player.StatusList)
            {
                // Set Status
                string statusId = status.StatusId.ToString();
                string statusName = StatusCache.GetStatusName(status.StatusId) ?? string.Empty;

                // Set Source Name
                string sourceName = status.SourceId != player.GameObjectId
                    ? status.SourceObject?.Name?.ToString() ?? string.Empty
                    : string.Empty;

                // Set Duration
                float buffDuration = GetStatusEffectRemainingTime((ushort)status.StatusId, anyOwner: true);
                string formattedDuration = $"{SymbolDuration} {(buffDuration >= 60f
                    ? $"{(int)(buffDuration / 60f)}m"
                    : $"{buffDuration:F1}s")}";

                // Set Parameter
                string formattedParam = status.Param > 0
                    ? $"{SymbolParameter} {status.Param}"
                    : string.Empty;

                // Build First Column
                string firstColumn = (string.IsNullOrEmpty(statusName), string.IsNullOrEmpty(sourceName)) switch
                {
                    (false, false)  => $"{sourceName} → {statusName}:", // Both Exist
                    (false, true)   => $"{statusName}:",                // Only 'statusName'
                    (true, false)   => $"{sourceName} → {UnknownName}", // Only 'sourceName'
                    (true, true)    => UnknownName                      // Neither
                };

                // Build Second Column
                var secondColumn = new StringBuilder();
                secondColumn.Append(statusId.PadRight(WidthStatusId));
                secondColumn.Append(formattedDuration.PadRight(WidthStatusDuration));
                secondColumn.Append(formattedParam);

                // Print
                CustomStyleText(firstColumn, secondColumn, useMonofont: true);
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("Target Statuses"))
        {
            if (target is IBattleChara chara)
            {
                foreach (Status? status in chara.StatusList)
                {
                    // Set Status
                    string statusId = status.StatusId.ToString();
                    string sourceName = status.SourceObject?.Name?.ToString() ?? string.Empty;
                    string statusName = StatusCache.GetStatusName(status.StatusId) ?? string.Empty;

                    // Set Duration
                    float debuffDuration = GetStatusEffectRemainingTime((ushort)status.StatusId, chara, true);
                    string formattedDuration = $"{SymbolDuration} {(debuffDuration >= 60f
                        ? $"{(int)(debuffDuration / 60f)}m"
                        : $"{debuffDuration:F1}s")}";

                    // Set Parameter
                    string formattedParam = status.Param > 0
                        ? $"{SymbolParameter} {status.Param}"
                        : string.Empty;

                    // Build First Column
                    string firstColumn = (string.IsNullOrEmpty(statusName), string.IsNullOrEmpty(sourceName)) switch
                    {
                        (false, false)  => $"{sourceName} → {statusName}:", // Both Exist
                        (false, true)   => $"{statusName}:",                // Only 'statusName'
                        (true, false)   => $"{sourceName} → {UnknownName}", // Only 'sourceName'
                        (true, true)    => UnknownName                      // Neither
                    };

                    // Build Second Column
                    var secondColumn = new StringBuilder();
                    secondColumn.Append(statusId.PadRight(WidthStatusId));
                    secondColumn.Append(formattedDuration.PadRight(WidthStatusDuration));
                    secondColumn.Append(formattedParam);

                    // Print
                    CustomStyleText(firstColumn, secondColumn, useMonofont: true);
                }

                if (ImGui.CollapsingHeader("ICD Tracker"))
                {
                    foreach (var t in ICDTracker.Trackers.Where(x => x.GameObjectId == chara.GameObjectId))
                    {
                        CustomStyleText($"{((ushort)t.StatusID).StatusName()}", $"{t.TimeUntilExpired():mm\\:ss}");
                    }
                }
            }
        }

        #endregion

        ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

        #region Character

        ImGui.Text("Character");
        ImGui.Separator();

        if (ImGui.CollapsingHeader("Player Data"))
        {
            CustomStyleText("Health:", $"{player.CurrentHp:N0} / {player.MaxHp:N0} ({MathF.Round(PlayerHealthPercentageHp(), 2)}%)");
            CustomStyleText("MP:", $"{player.CurrentMp:N0} / {player.MaxMp:N0}");
            CustomStyleText("Job:", $"{player.ClassJob.Value.NameEnglish} (ID: {player.ClassJob.RowId})");
            CustomStyleText("Zone:", $"{Svc.Data.GetExcelSheet<TerritoryType>().FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType).PlaceName.Value.Name} (ID: {Svc.ClientState.TerritoryType})");
            CustomStyleText("In PvP:", InPvP());
            CustomStyleText("In FATE:", InFATE());
            CustomStyleText("In Combat:", InCombat());
            CustomStyleText("Combat Time:", CombatEngageDuration().ToString("mm\\:ss\\:ff"));
            CustomStyleText("Hitbox Radius:", player.HitboxRadius);
            CustomStyleText("Movement Time:", TimeMoving.ToString("mm\\:ss\\:ff"));
            CustomStyleText($"Dashing:", IsDashing());
            CustomStyleText("In Boss Encounter:", InBossEncounter());

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

            CustomStyleText("Job Gauge Data", string.Empty);
            ImGui.Separator();
            switch (Player.Job)
            {
                case Job.PLD:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Paladin);
                    break;
                case Job.MNK:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Monk);
                    break;
                case Job.WAR:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Warrior);
                    break;
                case Job.DRG:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Dragoon);
                    break;
                case Job.BRD:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Bard);
                    break;
                case Job.WHM:
                    Util.ShowStruct(&JobGaugeManager.Instance()->WhiteMage);
                    break;
                case Job.BLM:
                    Util.ShowStruct(&JobGaugeManager.Instance()->BlackMage);
                    break;
                case Job.SMN:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Summoner);
                    break;
                case Job.SCH:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Scholar);
                    break;
                case Job.NIN:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Ninja);
                    break;
                case Job.MCH:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Machinist);
                    break;
                case Job.DRK:
                    Util.ShowStruct(&JobGaugeManager.Instance()->DarkKnight);
                    break;
                case Job.AST:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Astrologian);
                    break;
                case Job.SAM:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Samurai);
                    break;
                case Job.RDM:
                    Util.ShowStruct(&JobGaugeManager.Instance()->RedMage);
                    break;
                case Job.GNB:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Gunbreaker);
                    break;
                case Job.DNC:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Dancer);
                    break;
                case Job.RPR:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Reaper);
                    break;
                case Job.SGE:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Sage);
                    break;
                case Job.VPR:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Viper);
                    break;
                case Job.PCT:
                    Util.ShowStruct(&JobGaugeManager.Instance()->Pictomancer);
                    break;
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("Target Data"))
        {
            if (target is not null) {
                CustomStyleText("Name:", target?.Name);
                CustomStyleText("Health:", $"{GetTargetCurrentHP():N0} / {GetTargetMaxHP():N0} ({MathF.Round(GetTargetHPPercent(), 2)}%)");
                CustomStyleText("Distance:", $"{MathF.Round(GetTargetDistance(), 2)}y");
                CustomStyleText("Hitbox Radius:", target?.HitboxRadius);
                CustomStyleText("In Melee Range:", InMeleeRange());
                CustomStyleText("Height Difference:", $"{MathF.Round(GetTargetHeightDifference(), 2)}y");
                CustomStyleText("Relative Position:", AngleToTarget().ToString());
                CustomStyleText("Requires Positionals:", TargetNeedsPositionals());
                CustomStyleText("Is Invincible:", TargetIsInvincible(target!));
                CustomStyleText("Is Friendly:", target?.IsFriendly());

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

            if (ImGui.TreeNode("Cast Data"))
            {
                if (target is IBattleChara castChara)
                {
                    CustomStyleText("Cast Action:", castChara.CastActionId == 0
                        ? string.Empty
                        : $"{(string.IsNullOrEmpty(GetActionName(castChara.CastActionId))
                            ? "Unknown"
                            : GetActionName(castChara.CastActionId))} (ID: {castChara.CastActionId})");
                    CustomStyleText("Cast Time:", $"{castChara.CurrentCastTime:F2} / {castChara.TotalCastTime:F2}");

                    // Extract Lumina Data
                    var charaSpell = castChara.CastActionId > 0
                        ? Svc.Data.GetExcelSheet<Action>()?.GetRowOrDefault(castChara.CastActionId)
                        : null;

                    CustomStyleText("Cast 100ms:", $"{charaSpell?.Cast100ms * 0.1f ?? 0f:F2} + {charaSpell?.ExtraCastTime100ms * 0.1f ?? 0f:F2}");
                    CustomStyleText("Cast Type:", $"{charaSpell?.CastType ?? 0}");
                    CustomStyleText("Action Type:", $"{castChara.CastActionType}");
                    CustomStyleText("Action Range:", $"{GetActionRange(charaSpell?.RowId ?? 0)}y");
                    CustomStyleText("Effect Range:", $"{charaSpell?.EffectRange ?? 0}y");
                    CustomStyleText("Interruptible:", $"{castChara.IsCastInterruptible}");
                }
                else CustomStyleText("No valid target.", "");

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Object Data"))
            {
                CustomStyleText("DataId:", target?.DataId);

                // Display 'EntityId' only if it differs from 'GameObjectId'
                if (target is not null && target.EntityId != target.GameObjectId)
                {
                    CustomStyleText("EntityId:", target.EntityId);
                    ImGuiEx.InfoMarker("EntityId does not match ObjectId.\nThis object may have special interactivity rules.");
                }

                CustomStyleText("ObjectId:", target?.GameObjectId);
                CustomStyleText("ObjectKind:", target?.ObjectKind);
                CustomStyleText("ObjectSubKind:", target?.SubKind);
                CustomStyleText("ObjectType:", target?.GetType()?.Name);

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Enmity Data"))
            {
                CustomStyleText($"Highest Enmity DPS:", $"{StrongestDPS()?.Name}");

                if (ImGui.TreeNode("Enmity Table"))
                {
                    foreach (var h in EnmityDictParty)
                    {
                        CustomStyleText($"{Svc.Objects.First(x => x.GameObjectId == h.Key).Name}:", $"{h.Value}%");
                    }

                    ImGui.TreePop();
                }

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Heal Target Data"))
            {
                CustomStyleText("Current:", SimpleTarget.Stack.AllyToHeal.Name);
                ImGuiEx.InfoMarker("Cycles from Party UI Mouseover → Soft Target → Hard Target → Player.");

                CustomStyleText("Shield:", $"{(SimpleTarget.Stack.AllyToHeal as ICharacter).ShieldPercentage}%");
                CustomStyleText("Health:", $"{MathF.Round(GetTargetHPPercent(SimpleTarget.Stack.AllyToHeal), 2)}% / {MathF.Round(GetTargetHPPercent(SimpleTarget.Stack.AllyToHeal, true), 2)}% (+Shield)");

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

                if (ImGui.TreeNode("Enemies Near Target"))
                {
                    var enemies = Svc.Objects
                    .OfType<IBattleNpc>()
                    .Where(x => x.ObjectKind == ObjectKind.BattleNpc &&
                                x.IsTargetable &&
                                !x.IsDead &&
                                x.BattleNpcKind is BattleNPCSubKind.Enemy or BattleNPCSubKind.BattleNpcPart);

                    foreach (var enemy in enemies)
                    {
                        if (!enemy.Character()->InCombat) continue;
                        if (enemy.GameObjectId == target?.GameObjectId) continue;

                        var dist = MathF.Round(GetTargetDistance(enemy, target), 2);
                        CustomStyleText($"{enemy.Name} ({enemy.GameObjectId}):", $"{dist}y");
                    }

                    ImGui.TreePop();
                }
            }
        }

        #endregion

        ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

        #region Party

        ImGui.Text("Party");
        ImGui.Separator();

        if (ImGui.CollapsingHeader("Party Data"))
        {
            CustomStyleText("Party ID:", Svc.Party.PartyId);
            CustomStyleText("Party Size:", GetPartyMembers().Count);
            CustomStyleText("Party Avg. Health:", $"{MathF.Round(GetPartyAvgHPPercent(), 2)}%");
            CustomStyleText("Party Combat Time:", PartyEngageDuration().ToString("mm\\:ss\\:ff"));
            CustomStyleText("Alliance Group:", GetAllianceGroup());

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("Member Data"))
        {
            foreach (var member in GetPartyMembers())
            {
                if (ImGui.TreeNode($"{member?.BattleChara?.Name}###{member.GameObjectId}"))
                {
                    CustomStyleText("Health:", $"{member.CurrentHP:N0} / {member.BattleChara.MaxHp:N0} ({MathF.Round(member.CurrentHP * 100f / member.BattleChara.MaxHp, 2)}%)");
                    CustomStyleText("MP:", $"{member.CurrentMP:N0} / {member.BattleChara.MaxMp:N0}");
                    CustomStyleText("Job:", $"{member.RealJob?.NameEnglish} (ID: {member.RealJob?.RowId})");
                    CustomStyleText("Dead Timer:", TimeSpentDead(member.BattleChara.GameObjectId));
                    CustomStyleText("Role:", $"{member?.GetRole()}");

                    if (ImGui.TreeNode("Data Dump"))
                    {
                        Util.ShowObject(member.BattleChara);
                        ImGui.TreePop();
                    }

                    ImGui.TreePop();
                }
            }
        }

        #endregion

        ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

        #region Actions

        ImGui.Text("Action");
        ImGui.Separator();

        // ActionSheet Reference
        var actionSheet = Svc.Data.GetExcelSheet<Action>();

        // PvE Actions
        var actionsPvE = actionSheet
            .Where(x =>
                x.ClassJobLevel > 0 &&
                x.ClassJobCategory.RowId != 1 &&
                x.ClassJobCategory.Value.IsJobInCategory(Player.Job))
            .OrderBy(x => x.ClassJobLevel);

        // PvP Actions
        var actionsPvP = actionSheet
            .Where(x =>
                x.IsPvP &&
                x.Icon != 405 &&
                x.ClassJobCategory.RowId != 1 &&
                x.ClassJobCategory.Value.IsJobInCategory(Player.Job))
            .OrderBy(x => x.RowId);

        // Bozja Actions
        var actionsBozja = actionSheet
            .Where(x =>
                x.ActionCategory.RowId != 5 &&
                x.RowId is (>= 20701 and <= 20733) or (>= 22344 and <= 22356) or (>= 23908 and <= 23921))
            .OrderBy(x => x.RowId);

        // Occult Actions
        var actionsOccult = actionSheet
            .Where(x =>
                x.RowId is >= 41588 and <= 41651 &&
                x.RowId is not (41593 or 41632))
            .OrderBy(x => x.RowId);

        if (ImGui.CollapsingHeader("Action Data"))
        {
            CustomStyleText("Time Since Last Action:", $"{ActionWatching.TimeSinceLastAction}");
            CustomStyleText("Last Action:",
                ActionWatching.LastAction == 0
                    ? string.Empty
                    : $"{(string.IsNullOrEmpty(GetActionName(ActionWatching.LastAction))
                        ? "Unknown"
                        : GetActionName(ActionWatching.LastAction))} (ID: {ActionWatching.LastAction})");
            CustomStyleText("Last Action Cost:", GetResourceCost(ActionWatching.LastAction));
            CustomStyleText("Last Action Type:", ActionWatching.GetAttackType(ActionWatching.LastAction));
            CustomStyleText("Last Weaponskill:", GetActionName(ActionWatching.LastWeaponskill));
            CustomStyleText("Last Spell:", GetActionName(ActionWatching.LastSpell));
            CustomStyleText("Last Ability:", GetActionName(ActionWatching.LastAbility));
            CustomStyleText("Combo Timer:", $"{ComboTimer:F1}");
            CustomStyleText("Combo Action:",
                ComboAction == 0
                    ? string.Empty
                    : $"{(string.IsNullOrEmpty(GetActionName(ComboAction))
                        ? "Unknown"
                        : GetActionName(ComboAction))} (ID: {ComboAction})");
            CustomStyleText("Cast Time:", $"{player.CurrentCastTime:F2} / {player.TotalCastTime:F2}");
            CustomStyleText("Cast Action:",
                player.CastActionId == 0
                    ? string.Empty
                    : $"{(string.IsNullOrEmpty(GetActionName(player.CastActionId))
                        ? "Unknown"
                        : GetActionName(player.CastActionId))} (ID: {player.CastActionId})");
            CustomStyleText("GCD Total:", GCDTotal);
            CustomStyleText("Queued Action:", ActionManager.Instance()->QueuedActionId.ActionName());
            CustomStyleText("Animation Lock:", $"{ActionManager.Instance()->AnimationLock:F1}");
            CustomStyleText($"Duty Action 1:", $"{Action1.ActionName()}");
            CustomStyleText($"Duty Action 2:", $"{Action2.ActionName()}");
            CustomStyleText($"Duty Action 3:", $"{Action3.ActionName()}");
            CustomStyleText($"Duty Action 4:", $"{Action4.ActionName()}");
            CustomStyleText($"Duty Action 5:", $"{Action5.ActionName()}");

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

            if (ImGui.TreeNode("Opener Data"))
            {
                if (WrathOpener.CurrentOpener is not null)
                {
                    CustomStyleText("Current Opener", WrathOpener.CurrentOpener.GetType().Name);
                    CustomStyleText("Opener State:", WrathOpener.CurrentOpener.CurrentState);
                    CustomStyleText("Current Opener Action:", WrathOpener.CurrentOpener.CurrentOpenerAction.ActionName());
                    CustomStyleText("Current Opener Step:", WrathOpener.CurrentOpener.OpenerStep);

                    if (WrathOpener.CurrentOpener.OpenerActions.Count > 0 &&
                        WrathOpener.CurrentOpener.OpenerStep <
                        WrathOpener.CurrentOpener.OpenerActions.Count)
                    {
                        CustomStyleText("Next Action:", WrathOpener.CurrentOpener.OpenerActions[WrathOpener.CurrentOpener.OpenerStep].ActionName());
                        CustomStyleText("Is Delayed Weave:", WrathOpener.CurrentOpener.DelayedWeaveSteps.Any(x => x == WrathOpener.CurrentOpener.OpenerStep));
                        CustomStyleText("Can Delayed Weave:", CanDelayedWeave(weaveEnd: 0.1f));
                    }
                }

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Limit Break Data"))
            {
                CustomStyleText("Current Value:", LimitBreakValue);
                CustomStyleText("Current Level:", LimitBreakLevel);
                CustomStyleText("Limit Break Action:", LimitBreakAction.ActionName());
                CustomStyleText("Limit Break Ready:", $"Lv1: {IsLB1Ready}  Lv2: {IsLB2Ready}  Lv3: {IsLB3Ready}");

                ImGui.TreePop();
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("ActionReady"))
        {
            if (ImGui.TreeNode("PvE"))
            {
                foreach (var act in actionsPvE)
                {
                    var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, act.RowId, checkRecastActive: false, checkCastingActive: false);
                    CustomStyleText(act.Name.ExtractText(), $"{ActionReady(act.RowId)}, {status} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(status).Text})");
                }

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("PvP"))
            {
                foreach (var act in actionsPvP)
                {
                    var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, act.RowId, checkRecastActive: false, checkCastingActive: false);
                    CustomStyleText(act.Name.ExtractText(), $"{ActionReady(act.RowId)}, {status} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(status).Text})");
                }

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Bozja"))
            {
                foreach (var act in actionsBozja)
                {
                    var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, act.RowId, checkRecastActive: false, checkCastingActive: false);
                    CustomStyleText(act.Name.ExtractText(), $"{ActionReady(act.RowId)}, {status} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(status).Text})");
                }

                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Occult"))
            {
                foreach (var act in actionsOccult)
                {
                    var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, act.RowId, checkRecastActive: false, checkCastingActive: false);
                    CustomStyleText(act.Name.ExtractText(), $"{ActionReady(act.RowId)}, {status} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(status).Text})");
                }

                ImGui.TreePop();
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("ActionSheet"))
        {
            var prevAction = _debugSpell == null
                ? "Select Action"
                : $"Lv.{_debugSpell.Value.ClassJobLevel} {_debugSpell.Value.Name} ({_debugSpell.Value.RowId})";

            var prevSheet = _sheetType == null
                ? "Select Type"
                : _sheetType.Value.ToString();

            // Type Dropdown Menu
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X * 0.3f);
            using (var comboBoxSheet = ImRaii.Combo("###SheetCombo", prevSheet))
            {
                if (comboBoxSheet)
                {
                    if (ImGui.Selectable("", _sheetType == null))
                    {
                        _sheetType = null;
                        _debugSpell = null;
                        _sheetCustomId = 0;
                    }

                    foreach (SheetType sheetType in Enum.GetValues(typeof(SheetType)))
                    {
                        if (ImGui.Selectable($"{sheetType}"))
                        {
                            _sheetType = sheetType;
                            _debugSpell = null;
                            _sheetCustomId = 0;
                        }
                    }
                }
            }

            ImGui.SameLine();

            // Custom Action Input
            if (_sheetType == SheetType.Custom)
            {
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.InputInt("###CustomActionInput", ref _sheetCustomId))
                {
                    if (actionSheet.TryGetRow(rowId: (uint)_sheetCustomId, out var act))
                    {
                        _debugSpell = act;
                    }
                }
            }
            else
            {
                var currentActions = _sheetType switch
                {
                    SheetType.PvE => actionsPvE,
                    SheetType.PvP => actionsPvP,
                    SheetType.Bozja => actionsBozja,
                    SheetType.Occult => actionsOccult,
                    _ => null,
                };

                // Action Dropdown Menu
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                using var comboBoxAction = ImRaii.Combo("###ActionCombo", prevAction);
                if (comboBoxAction)
                {
                    if (ImGui.Selectable("", _debugSpell == null))
                    {
                        _debugSpell = null;
                    }

                    if (currentActions != null)
                    {
                        foreach (var act in currentActions)
                        {
                            if (ImGui.Selectable($"Lv.{act.ClassJobLevel} {act.Name} ({act.RowId})", _debugSpell?.RowId == act.RowId))
                            {
                                _debugSpell = act;
                            }
                        }
                    }
                }
            }

            // Display Action Info
            if (_debugSpell != null)
            {
                var actionStatus = ActionManager.Instance()->GetActionStatus(ActionType.Action, _debugSpell.Value.RowId);
                var icon = Svc.Texture.GetFromGameIcon(new(_debugSpell.Value.Icon)).GetWrapOrEmpty().Handle;

                ImGui.Image(icon, new Vector2(60).Scale());
                ImGui.SameLine();
                ImGui.Image(icon, new Vector2(30).Scale());

                CustomStyleText("Action Status:", $"{actionStatus} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(actionStatus).Text})");
                CustomStyleText("Action Type:", _debugSpell.Value.ActionCategory.Value.Name);

                // Quest Check
                if (_debugSpell.Value.UnlockLink.RowId != 0 && Svc.Data.GetExcelSheet<Quest>().TryGetRow(_debugSpell.Value.UnlockLink.RowId, out var unlockQuest))
                    CustomStyleText("Quest:", $"{unlockQuest.Name} ({(UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(_debugSpell.Value.UnlockLink.RowId) ? "Completed" : "Not Completed")})");

                CustomStyleText("Base Recast:", $"{_debugSpell.Value.Recast100ms / 10f}s");
                CustomStyleText("Original Hook:", OriginalHook(_debugSpell.Value.RowId).ActionName());
                CustomStyleText("Cooldown Total:", $"{GetCooldown(_debugSpell.Value.RowId).CooldownTotal}");
                CustomStyleText("Current Cooldown:", GetCooldown(_debugSpell.Value.RowId).CooldownRemaining);
                CustomStyleText("Current Cast Time:", ActionManager.GetAdjustedCastTime(ActionType.Action, _debugSpell.Value.RowId));
                CustomStyleText("Max Charges:", $"{_debugSpell.Value.MaxCharges}");
                CustomStyleText("Charges (Level):", $"{GetCooldown(_debugSpell.Value.RowId).MaxCharges}");
                CustomStyleText("Range:", $"{GetActionRange(_debugSpell.Value.RowId)}");
                CustomStyleText("Effect Range:", $"{_debugSpell.Value.EffectRange}");
                CustomStyleText("Can Target Hostile:", $"{_debugSpell.Value.CanTargetHostile}");
                CustomStyleText("Can Target Self:", $"{_debugSpell.Value.CanTargetSelf}");
                CustomStyleText("Can Target Friendly:", $"{_debugSpell.Value.CanTargetAlly}");
                CustomStyleText("Can Target Party:", $"{_debugSpell.Value.CanTargetParty}");
                CustomStyleText("Can Target Area:", $"{_debugSpell.Value.TargetArea}");
                CustomStyleText("Can Queue:", $"{CanQueue(_debugSpell.Value.RowId)}");
                CustomStyleText("Cast Type:", $"{_debugSpell.Value.CastType}");

                if (ActionWatching.ActionTimestamps.TryGetValue(_debugSpell.Value.RowId, out long lastUseTimestamp))
                    CustomStyleText("Time Since Last Use:", $"{(Environment.TickCount64 - lastUseTimestamp) / 1000f:F2}");

                if (ActionWatching.LastSuccessfulUseTime.TryGetValue(_debugSpell.Value.RowId, out long lastSuccessfulUseTimestamp))
                    CustomStyleText("Time Since Last Successful Cast:", $"{(Environment.TickCount64 - lastSuccessfulUseTimestamp) / 1000f:F2}");

                var canUseOnSelf = ActionManager.CanUseActionOnTarget(_debugSpell.Value.RowId, Player.GameObject);
                CustomStyleText("Can Use on Self:", canUseOnSelf);

                // Target Required
                if (target is not null)
                {
                    var canUseOnTarget = ActionManager.CanUseActionOnTarget(_debugSpell.Value.RowId, target.Struct());
                    CustomStyleText("Can Use on Target:", canUseOnTarget);

                    CustomStyleText($"Just Used on Target:", $"{JustUsedOn(_debugSpell.Value.RowId, target)}");

                    var inRange = ActionManager.GetActionInRangeOrLoS(_debugSpell.Value.RowId, (GameObject*)player.Address, (GameObject*)target.Address);
                    CustomStyleText("Target in Range or LoS:",
                        inRange == 0
                            ? "In range and in line of sight"
                            : $"{inRange}: {Svc.Data.GetExcelSheet<LogMessage>().GetRow(inRange).Text}");

                    if (_debugSpell.Value.EffectRange > 0)
                        CustomStyleText("Number of Targets Hit:", $"{NumberOfEnemiesInRange(_debugSpell.Value.RowId, target)}");
                }

                if (ImGui.TreeNode("Data Dump"))
                {
                    Util.ShowObject(_debugSpell.Value);
                    ImGui.TreePop();
                }
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("Action Retargeting"))
        {
            var retargets = P.ActionRetargeting.Retargets;

            CustomStyleText("Current Unique Retargeting entries:",
                $"{retargets.Select(x => x.Value.ID)
                    .Distinct().Count()}");
            CustomStyleText("Current Total Retargeting entries:", $"{retargets.Count}");
            ImGuiComponents.HelpMarker("This includes all entries for combos' Replaced Actions as well.");

            if (retargets.Count > 0)
            {
                ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

                ImGui.Indent();
                ImGuiEx.TextUnderlined("Retargets:");
                var first = true;
                var distinctRetargets = retargets
                    .DistinctBy(x => x.Value.ID)
                    .Select(x => x.Value)
                    .OrderBy(x => x.Action);
                foreach (var retarget in distinctRetargets)
                {
                    if (!first) ImGuiEx.Spacing(new Vector2(0f, SpacingSmall/2));

                    CustomStyleText($"Action: {retarget.Action.ActionName()}",
                        $"ID: {retarget.ID,20}");
                    // Set a set amount of distance to the right of the ID,
                    // so the help mark doesn't move
                    var width = ImGui.CalcTextSize($"ID: {retarget.ID,20}").X;
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 110f.Scale() - width);
                    ImGui.Text("");
                    ImGuiComponents.HelpMarker("If you see the ID constantly changing, that's a sign that the Retarget is constantly being Partially Overwritten.\n\nThis occurs when two different Retargets keep being registered, which do not fully overwrite each other (they do not share the same Replaced Actions).\nUsually this is when you have a retargeting of an action in Feature and also in a Main Combo, and is a sign that both/all the Retargets will likely work.");

                    ImGui.Indent();

                    ImGui.Indent(10f.Scale());
                    var replacedActionsString = string.Join(", ",
                        retarget.ReplacedActions.Select(x => x.ActionName()));
                    ImGuiEx.Text("Replaced Actions:");
                    ImGui.SameLine();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey, replacedActionsString);
                    ImGui.Unindent(10f.Scale());

                    CustomStyleText($"Resolver: {retarget.ResolverName}",
                        $"Resolved Target: '{retarget.Resolver()?.Name ?? "Null"}'");
                    ImGuiComponents.HelpMarker("Resolvers may only resolve to a fallback target,\nexcept under conditions where the Retargeting would actually be applied.");

                    var createdTimeString = retarget.Created.ToString(@"HH\:mm\:ss");
                    CustomStyleText($"Created: {createdTimeString}",
                        $"Don't Cull Setting: {(retarget.DontCull ? "On" : "Off")}");

                    ImGui.Unindent();

                    first = false;
                }
                ImGui.Unindent();
            }
        }

        #endregion

        ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

        #region Misc

        ImGui.Text("Other");
        ImGui.Separator();

        if (ImGui.CollapsingHeader("Blue Mage Data"))
        {
            if (ImGui.TreeNode("Active Spells"))
            {
                ImGui.TextUnformatted($"{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(GetActionName).OrderBy(x => x))}");
                ImGui.TreePop();
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("Miscellaneous Data"))
        {
            CustomStyleText("Countdown Active:", $"{CountdownActive}");
            CustomStyleText("Countdown Remaining:", $"{CountdownRemaining}");
            CustomStyleText("Raidwide Incoming:", $"{RaidWideCasting()}");
        }

        #endregion

        ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

        #region IPC

        ImGui.Text("IPC");
        ImGui.Separator();

        static void WrathIPCCallback(int cancellationReason, string extraInfo)
        {
            _wrathLease = null;
        }

        if (ImGui.CollapsingHeader("Wrath IPC"))
        {
            CustomStyleText("Wrath Leased:", _wrathLease is not null);
            if (_wrathLease is null)
            {
                ImGui.Indent();
                if (ImGui.Button("Register"))
                {
                    _wrathLease = P.IPC.RegisterForLease("WrathCombo", "WrathCombo", WrathIPCCallback);
                }
                ImGui.Unindent();
            }

            if (_wrathLease is not null)
            {
                CustomStyleText("Lease GUID", $"{_wrathLease}");
                CustomStyleText("Configurations: ", $"{P.IPC.Leasing.Registrations[_wrathLease!.Value].SetsLeased}");

                ImGuiEx.Spacing(new Vector2(20, 20));

                if (ImGui.Button("Release"))
                {
                    P.IPC.ReleaseControl(_wrathLease.Value);
                    _wrathLease = null;
                }

                ImGui.SameLine();
                if (ImGui.Button("Set Autorot For Job"))
                {
                    P.IPC.SetCurrentJobAutoRotationReady(_wrathLease!.Value);
                }
                ImGui.SameLine();
                if (ImGui.Button("Set Autorot For WHM"))
                {
                    P.IPC.Leasing.AddRegistrationForCurrentJob(_wrathLease!.Value, Job.WHM);
                }

                ImGuiEx.Spacing(new Vector2(20, 20));

                if (ImGui.Button("Mimic AutoDuty"))
                {
                    // https://github.com/ffxivcode/AutoDuty/blob/master/AutoDuty/IPC/IPCSubscriber.cs#L460
                    if (!P.IPC.IsCurrentJobAutoRotationReady())
                        P.IPC.SetCurrentJobAutoRotationReady(_wrathLease!.Value);

                    P.IPC.SetAutoRotationState(_wrathLease!.Value);
                    P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.InCombatOnly, false);
                    P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.AutoRez, true);
                    P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.AutoRezDPSJobs, true);
                    P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.IncludeNPCs, true);
                    P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.DPSRotationMode, DPSRotationMode.Lowest_Current);
                    P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.HealerRotationMode, HealerRotationMode.Lowest_Current);
                }
                ImGui.SameLine();
                if (ImGui.Button("Mimic Questionable"))
                {
                    // https://git.carvel.li/liza/Questionable/src/commit/de90882ecbb609c2f79fecc1ec17b751dc8763f2/Questionable/Controller/CombatModules/WrathComboModule.cs#L68
                    P.IPC.SetAutoRotationState(_wrathLease!.Value);
                    P.IPC.SetCurrentJobAutoRotationReady(_wrathLease!.Value);
                }
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));

            CustomStyleText("All Leases:", "");

            if (P.IPC.Leasing.Registrations.Count > 0)
            {
                ImGui.SameLine();
                if (ImGui.Button("Release All Leases"))
                {
                    P.IPC.Leasing.SuspendLeases();
                    _wrathLease = null;
                }
            }

            if (P.IPC.Leasing.Registrations.Count > 0)
            {
                foreach (var registration in P.IPC.Leasing.Registrations)
                {
                    var jobs = registration.Value.JobsControlled.Count > 0
                        ? string.Join(",", registration.Value.JobsControlled.Keys)
                        : "0";
                    var combos = registration.Value.CombosControlled.Count > 0
                        ? registration.Value.CombosControlled.Count.ToString()
                        : "0";

                    CustomStyleText(
                        $"{registration.Value.PluginName}",
                        $"Configurations: {registration.Value.SetsLeased,3}; " +
                        $"Auto-Rotation: {registration.Value.AutoRotationControlled.Count > 0}");

                    ImGui.NewLine();
                    ImGuiEx.Spacing(new Vector2(10, 0));
                    ImGui.SameLine();
                    if (ImGui.Button("Release##releaseFromList" + registration.Key))
                    {
                        P.IPC.ReleaseControl(registration.Key);
                    }
                    ImGui.SameLine();

                    CustomStyleText("", $"Jobs: {jobs,-30} " + $"Combos: {combos,-6}");
                    CustomStyleText("", $"Created: {" ",-24} {registration.Value.Created:yyyy-MM-ddTHH:mm:ss}");
                }
            }
            else
            {
                CustomStyleText("No current leases.", "");
            }

            ImGuiEx.Spacing(new Vector2(0f, SpacingSmall));
        }

        if (ImGui.CollapsingHeader("Orbwalker IPC"))
        {
            CustomStyleText("Plugin Installed & On:", $"{OrbwalkerIPC.IsEnabled}");
            if (OrbwalkerIPC.IsEnabled)
            {
                CustomStyleText("Version:", $"{OrbwalkerIPC.InstalledVersion}");
                CustomStyleText("Plugin Enabled:", OrbwalkerIPC.PluginEnabled());

                ImGui.Indent();
                if (ImGui.Button("Set Enabled"))
                {
                    OrbwalkerIPC.SetPluginEnabled(!OrbwalkerIPC.PluginEnabled());
                }
                ImGui.Unindent();

                CustomStyleText("Can OrbWalk:", OrbwalkerIPC.CanOrbwalk);
                var jobs = OrbwalkerIPC.EnabledJobs();
                CustomStyleText("Orbwalking Jobs:", string.Join(", ", jobs));

                ImGui.Indent();
                if (ImGui.Button("Toggle Current Job Enabled"))
                {
                    OrbwalkerIPC.SetEnabledJob((uint)Player.Job, jobs.All(x => x != (int)Player.Job));
                }
                ImGui.Unindent();
            }
        }

        #endregion

        ImGuiEx.Spacing(new Vector2(0, SpacingMedium));

        #region Hidden Features

        if (ImGui.Checkbox("Show Hidden Features",
                ref Service.Configuration.ShowHiddenFeatures))
            Service.Configuration.Save();

        ImGuiComponents.HelpMarker("Some features can be marked as hidden, and will only be shown if this setting is enabled.\nThis is here instead of on the Settings tab while this behavior is still early in its life, and to keep such features more secretive.");

        ImGui.SameLine();
        ImGui.TextColored(ImGuiColors.DalamudGrey, "(Do NOT publicly direct users to this setting!)");
        if (Service.Configuration.ShowHiddenFeatures)
        {
            ImGui.Indent();
            ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey,
                "Hidden Features are minor one-offs that are not priorities for dev time.\n" +
                "Do not request new ones or maintenance for existing ones publicly.\n" +
                "Do not expect Hidden Features to be maintained or even stick around after they cease to be applicable.");
            ImGui.Unindent();
        }

        #endregion
    }

    private static void DisableDebugConfig()
    {
        DebugConfig = false;
        Service.Configuration =
            _previousConfig ??
            Svc.PluginInterface.GetPluginConfig() as PluginConfiguration ??
            new PluginConfiguration();
        _previousConfig = null;

        P.IPC = Provider.Init();
        AutoRotationController.cfg = null;
        UpdateCaches(true, true, false);
    }

    public new static void Dispose()
    {
        DisableDebugConfig();
    }
}
