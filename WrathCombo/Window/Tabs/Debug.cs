#region

using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
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
using ImGuiNET;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
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
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using Status = Dalamud.Game.ClientState.Statuses.Status;

#endregion

namespace WrathCombo.Window.Tabs
{
    internal class Debug : ConfigWindow, IDisposable
    {
        public static bool DebugConfig;
        private static string _debugError = string.Empty;
        private static string _debugConfig = string.Empty;
        private static PluginConfiguration? _previousConfig;

        private static Action? _debugSpell;

        private static Guid? _wrathLease;

        internal new static unsafe void Draw()
        {
            ImGui.Text("This is where you can figure out where it all went wrong.");

            ImGuiEx.Spacing(new Vector2(0, 20));

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
                    PluginLog.Error(
                        $"Failed to read debug configuration.\n{ex.Message}\n{ex.StackTrace}");
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

            ImGuiEx.Spacing(new Vector2(0, 20));

            var LocalPlayer = Svc.ClientState.LocalPlayer;
            var target = Svc.Targets.Target;
            uint[] statusBlacklist =
            [
                360, 361, 362, 363, 364, 365, 366, 367, 368
            ]; // Duration will not be displayed for these status effects

            // Custom Styling
            static void CustomStyleText(string label, object? value)
            {
                ImGui.Columns(2, null, false);
                if (!string.IsNullOrEmpty(label))
                {
                    ImGui.TextUnformatted(label);
                    ImGui.SameLine(0, 4f);
                }

                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                ImGui.NextColumn();
                ImGui.TextUnformatted(value?.ToString() ?? "");
                ImGui.PopStyleColor();
                ImGui.Columns(1);
            }

            if (LocalPlayer is null)
            {
                ImGui.TextUnformatted("Please log into the game to use this tab.");
                return;
            }

            #region Player

            if (ImGui.CollapsingHeader("Player Status Effects"))
            {
                foreach (Status? status in LocalPlayer.StatusList)
                {
                    float buffDuration = GetStatusEffectRemainingTime((ushort) status.StatusId, anyOwner: true);
                    string formattedDuration = "";
                    if (buffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                    {
                        if (buffDuration >= 60)
                        {
                            int minutes = (int) (buffDuration / 60);
                            formattedDuration = $"{minutes}m";
                        }
                        else formattedDuration = $"{buffDuration:F1}s";
                    }

                    // Null Check (Status Name)
                    if (!string.IsNullOrEmpty(ActionWatching.GetStatusName(status.StatusId)))
                    {
                        CustomStyleText(ActionWatching.GetStatusName(status.StatusId) + ":",
                            $"{status.StatusId} P: {status.Param}, {formattedDuration}");
                    }
                    else CustomStyleText("", status.StatusId);
                }
            }

            if (ImGui.CollapsingHeader("Player Info"))
            {
                CustomStyleText("Job:",
                    $"{LocalPlayer.ClassJob.Value.NameEnglish} (ID: {LocalPlayer.ClassJob.RowId})");
                CustomStyleText("Zone:",
                    $"{Svc.Data.GetExcelSheet<TerritoryType>().FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType).PlaceName.Value.Name} (ID: {Svc.ClientState.TerritoryType})");
                CustomStyleText($"MP:", GetPartyMembers().First().CurrentMP);
                CustomStyleText("In PvP:", InPvP());
                CustomStyleText("In Combat:", InCombat());
                CustomStyleText("In Boss:", InBossEncounter());
                CustomStyleText("Cast Time:",
                    LocalPlayer.TotalCastTime - LocalPlayer.CurrentCastTime);
                CustomStyleText("Hitbox Radius:", LocalPlayer.HitboxRadius);
                CustomStyleText("In FATE:", InFATE());
                CustomStyleText("Time in Combat:",
                    CombatEngageDuration().ToString("mm\\:ss"));
                CustomStyleText("Party Combat Time:",
                    PartyEngageDuration().ToString("mm\\:ss"));
                CustomStyleText("Limit Break:", LimitBreakValue);
                CustomStyleText("LBs Ready:",
                    $"1.{IsLB1Ready} 2.{IsLB2Ready} 3.{IsLB3Ready}");
                CustomStyleText("LB Level:", LimitBreakLevel);
                CustomStyleText("LB Action:", LimitBreakAction.ActionName());
                CustomStyleText("Animation Lock:",
                    ActionManager.Instance()->AnimationLock);
                CustomStyleText("Movement Timer:",
                    TimeMoving.ToString("mm\\:ss\\:ff"));
                CustomStyleText("Alliance Group:", GetAllianceGroup());

                ImGuiEx.Spacing(new Vector2(0, 10));

                ImGui.Text("Job Gauge");
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
            }

            #endregion

            ImGuiEx.Spacing(new Vector2(0, 20));

            #region Target

            if (ImGui.CollapsingHeader("Target Status Effects"))
            {
                if (target is IBattleChara chara)
                {
                    foreach (Status? status in chara.StatusList)
                    {
                        // Null Check (Source Name)
                        if (status.SourceObject is not null)
                        {
                            ImGui.TextUnformatted($"{status.SourceObject.Name} ->");
                            ImGui.SameLine(0, 4f);
                        }

                        // Null Check (Status Name)
                        if (!string.IsNullOrEmpty(ActionWatching.GetStatusName(status.StatusId)))
                        {
                            CustomStyleText(ActionWatching.GetStatusName(status.StatusId) + ":", status.StatusId);
                        }
                        else CustomStyleText("", status.StatusId);

                        // Duration + Blacklist Check
                        float debuffDuration = GetStatusEffectRemainingTime((ushort) status.StatusId, CurrentTarget, true);
                        if (debuffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                        {
                            string formattedDuration;
                            if (debuffDuration >= 60)
                            {
                                int minutes = (int) (debuffDuration / 60);
                                formattedDuration = $"{minutes}m";
                            }
                            else formattedDuration = $"{debuffDuration:F1}s";

                            ImGui.SameLine(0, 4f);
                            CustomStyleText("", $"({formattedDuration})");
                        }
                    }
                }
            }

            if (ImGui.CollapsingHeader("Target Info"))
            {
                CustomStyleText("ObjectId:", target?.GameObjectId);
                CustomStyleText("ObjectKind:", target?.ObjectKind);
                CustomStyleText("Is BattleChara:", target is IBattleChara);
                CustomStyleText("Is PlayerCharacter:", target is IPlayerCharacter);
                CustomStyleText("Distance:", $"{Math.Round(GetTargetDistance(), 2)}y");
                CustomStyleText("Height:", $"{Math.Round(GetTargetHeightDifference(), 2)}y");
                CustomStyleText("Hitbox Radius:", target?.HitboxRadius);
                CustomStyleText("In Melee Range:", InMeleeRange());
                CustomStyleText("Requires Postionals:", TargetNeedsPositionals());
                CustomStyleText("Relative Position:", AngleToTarget().ToString());
                CustomStyleText("Health:",
                    $"{EnemyHealthCurrentHp():N0} / {EnemyHealthMaxHp():N0} ({Math.Round(GetTargetHPPercent(), 2)}%)");
                CustomStyleText("Shield:",
                    (GetHealTarget() as ICharacter).ShieldPercentage);
                CustomStyleText("Health Percent (+ Shield):",
                    $"{GetTargetHPPercent(GetHealTarget())} / {GetTargetHPPercent(GetHealTarget(), true)}");
                CustomStyleText("Party Avg HP Percent:",
                    $"{GetPartyAvgHPPercent()}");

                ImGui.Indent();
                if (ImGui.CollapsingHeader("Relative Target Distances"))
                {
                    ImGuiEx.TextUnderlined("Enemies");
                    var enemies = Svc.Objects
                        .Where(x =>
                            x.ObjectKind == ObjectKind.BattleNpc &&
                            x is {IsTargetable: true, IsDead: false}).Cast<IBattleNpc>()
                        .Where(x => x.BattleNpcKind is BattleNpcSubKind.Enemy
                            or BattleNpcSubKind.BattleNpcPart).ToList();
                    foreach (var enemy in enemies)
                    {
                        if (enemy.GameObjectId ==
                            Svc.Targets.Target?.GameObjectId) continue;
                        if (!enemy.Character()->InCombat) continue;
                        var dist = GetTargetDistance(enemy, Svc.Targets.Target);
                        CustomStyleText($"{enemy.Name} ({enemy.GameObjectId}):",
                            $"{dist:F1}");
                    }
                }

                ImGui.Unindent();
            }

            #endregion

            ImGuiEx.Spacing(new Vector2(0, 20));

            #region Party

            if (ImGui.CollapsingHeader("Party Info"))
            {
                CustomStyleText("Party ID:", Svc.Party.PartyId);
                CustomStyleText("Party Size:", GetPartyMembers().Count);
                if (ImGui.CollapsingHeader("Party Members"))
                {
                    ImGui.Indent();
                    foreach (var member in GetPartyMembers())
                    {
                        if (ImGui.CollapsingHeader($"{member?.BattleChara?.GetInitials()}###{member.GameObjectId}"))
                        {
                            CustomStyleText("Job:",
                                member.BattleChara.ClassJob.Value.Abbreviation);
                            CustomStyleText("HP:",
                                $"{member.CurrentHP}/{member.BattleChara.MaxHp}");
                            CustomStyleText("MP:",
                                $"{member.CurrentMP}/{member.BattleChara.MaxMp}");
                            CustomStyleText("Dead Timer:",
                                TimeSpentDead(member.BattleChara.GameObjectId));

                            Util.ShowObject(member.BattleChara);
                        }
                    }

                    ImGui.Unindent();
                }
            }

            #endregion

            ImGuiEx.Spacing(new Vector2(0, 20));

            #region Actions

            var actions = Svc.Data.GetExcelSheet<Action>()
                .Where(x => x.ClassJobLevel > 0 && x.ClassJobCategory.RowId != 1 &&
                            x.ClassJobCategory.Value.IsJobInCategory(Player.Job)).OrderBy(x => x.ClassJobLevel);
            if (ImGui.CollapsingHeader("Individual Action Info"))
            {
                string prev = _debugSpell == null
                    ? "Select Action"
                    : $"({_debugSpell.Value.RowId}) Lv.{_debugSpell.Value.ClassJobLevel}. {_debugSpell.Value.Name} - {(_debugSpell.Value.IsPvP ? "PvP" : "Normal")}";
                ImGuiEx.SetNextItemFullWidth();
                using (var comboBox = ImRaii.Combo("###ActionCombo", prev))
                {
                    if (comboBox)
                    {
                        if (ImGui.Selectable("", _debugSpell == null))
                        {
                            _debugSpell = null;
                        }

                        foreach (var act in actions)
                        {
                            if (ImGui.Selectable(
                                    $"({act.RowId}) Lv.{act.ClassJobLevel}. {act.Name} - {(act.IsPvP ? "PvP" : "Normal")}",
                                    _debugSpell?.RowId == act.RowId))
                            {
                                _debugSpell = act;
                            }
                        }
                    }
                }

                    if (_debugSpell != null)
                    {
                        var actionStatus =
                            ActionManager.Instance()->GetActionStatus(ActionType.Action, _debugSpell.Value.RowId);
                        var icon = Svc.Texture.GetFromGameIcon(new(_debugSpell.Value.Icon)).GetWrapOrEmpty()
                            .ImGuiHandle;
                        ImGui.Image(icon, new Vector2(60).Scale());
                        ImGui.SameLine();
                        ImGui.Image(icon, new Vector2(30).Scale());
                        CustomStyleText("Action Status:",
                            $"{actionStatus} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(actionStatus).Text})");
                        CustomStyleText("Action Type:", _debugSpell.Value.ActionCategory.Value.Name);
                        if (_debugSpell.Value.UnlockLink.RowId != 0)
                            CustomStyleText("Quest:",
                                $"{Svc.Data.GetExcelSheet<Quest>().GetRow(_debugSpell.Value.UnlockLink.RowId).Name} ({(UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(_debugSpell.Value.UnlockLink.RowId) ? "Completed" : "Not Completed")})");
                        CustomStyleText("Base Recast:", $"{_debugSpell.Value.Recast100ms / 10f}s");
                        CustomStyleText("Original Hook:", OriginalHook(_debugSpell.Value.RowId).ActionName());
                        CustomStyleText("Cooldown Total:",
                            $"{ActionManager.Instance()->GetRecastTime(ActionType.Action, _debugSpell.Value.RowId)}");
                        CustomStyleText("Current Cooldown:", GetCooldown(_debugSpell.Value.RowId).CooldownRemaining);
                        CustomStyleText("Current Cast Time:",
                            ActionManager.GetAdjustedCastTime(ActionType.Action, _debugSpell.Value.RowId));
                        CustomStyleText("Max Charges:", $"{_debugSpell.Value.MaxCharges}");
                        CustomStyleText("Charges (Level):", $"{GetCooldown(_debugSpell.Value.RowId).MaxCharges}");
                        CustomStyleText("Range:", $"{ActionWatching.GetActionRange(_debugSpell.Value.RowId)}");
                        CustomStyleText("Effect Range:", $"{_debugSpell.Value.EffectRange}");
                        CustomStyleText("Can Target Hostile:", $"{_debugSpell.Value.CanTargetHostile}");
                        CustomStyleText("Can Target Self:", $"{_debugSpell.Value.CanTargetSelf}");
                        CustomStyleText("Can Target Friendly:", $"{_debugSpell.Value.CanTargetAlly}");
                        CustomStyleText("Can Target Party:", $"{_debugSpell.Value.CanTargetParty}");
                        CustomStyleText("Can Target Area:", $"{_debugSpell.Value.TargetArea}");
                        CustomStyleText("Cast Type:", $"{_debugSpell.Value.CastType}");
                        CustomStyleText("Can Queue:", $"{CanQueue(_debugSpell.Value.RowId)}");
                        if (_debugSpell.Value.EffectRange > 0)
                            CustomStyleText("Targets Hit:",
                                $"{NumberOfEnemiesInRange(_debugSpell.Value.RowId, CurrentTarget)}");

                    if (ActionWatching.ActionTimestamps.ContainsKey(_debugSpell.Value.RowId))
                        CustomStyleText("Time Since Last Use:",
                            $"{(Environment.TickCount64 - ActionWatching.ActionTimestamps[_debugSpell.Value.RowId]) / 1000f:F2}");

                    if (ActionWatching.LastSuccessfulUseTime.ContainsKey(_debugSpell.Value.RowId))
                        CustomStyleText("Last Successful Cast:",
                            $"{ActionWatching.TimeSinceLastSuccessfulCast(_debugSpell.Value.RowId) / 1000f:F2}");

                    CustomStyleText($"JustUsedOn:", $"{JustUsedOn(_debugSpell.Value.RowId, CurrentTarget)}");

                    if (Svc.Targets.Target != null)
                    {
                        var inRange = ActionManager.GetActionInRangeOrLoS(_debugSpell.Value.RowId,
                            (GameObject*) LocalPlayer.Address, (GameObject*) Svc.Targets.Target.Address);
                        CustomStyleText("InRange or LoS:",
                            inRange == 0
                                ? "In range and in line of sight"
                                : $"{inRange}: {Svc.Data.GetExcelSheet<LogMessage>().GetRow(inRange).Text}");
                        var canUseOnTarget = ActionManager.CanUseActionOnTarget(_debugSpell.Value.RowId,
                            Svc.Targets.Target.Struct());
                        CustomStyleText("Can Use on Target:", canUseOnTarget);
                    }

                    var canUseOnSelf =
                        ActionManager.CanUseActionOnTarget(_debugSpell.Value.RowId, Player.GameObject);
                    CustomStyleText("Can Use on Self:", canUseOnSelf);

                    Util.ShowObject(_debugSpell.Value);
                }
            }

            if (ImGui.CollapsingHeader("ActionReady Info"))
            {
                foreach (var act in actions)
                {
                    var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, act.RowId,
                        checkRecastActive: false, checkCastingActive: false);
                    CustomStyleText(act.Name.ExtractText(),
                        $"{ActionReady(act.RowId)}, {status} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(status).Text})");
                }
            }

            if (ImGui.CollapsingHeader("Action Info"))
            {
                ImGui.Text("Action Info");
                ImGui.Separator();
                CustomStyleText("GCD Total:", GCDTotal);
                CustomStyleText("Time Since Last Action:",
                    $"{ActionWatching.TimeSinceLastAction}");
                CustomStyleText("Last Action:",
                    ActionWatching.LastAction == 0
                        ? string.Empty
                        : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(ActionWatching.LastAction)) ? "Unknown" : ActionWatching.GetActionName(ActionWatching.LastAction))} (ID: {ActionWatching.LastAction})");
                CustomStyleText("Last Action Cost:",
                    GetResourceCost(ActionWatching.LastAction));
                CustomStyleText("Last Action Type:",
                    ActionWatching.GetAttackType(ActionWatching.LastAction));
                CustomStyleText("Last Weaponskill:",
                    ActionWatching.GetActionName(ActionWatching
                        .LastWeaponskill));
                CustomStyleText("Last Spell:",
                    ActionWatching.GetActionName(ActionWatching.LastSpell));
                CustomStyleText("Last Ability:",
                    ActionWatching.GetActionName(ActionWatching.LastAbility));
                CustomStyleText("Combo Timer:", $"{ComboTimer:F1}");
                CustomStyleText("Combo Action:",
                    ComboAction == 0
                        ? string.Empty
                        : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(ComboAction)) ? "Unknown" : ActionWatching.GetActionName(ComboAction))} (ID: {ComboAction})");
                CustomStyleText("Cast Time:",
                    $"{LocalPlayer.CurrentCastTime:F2} / {LocalPlayer.TotalCastTime:F2}");
                CustomStyleText("Cast Action:",
                    LocalPlayer.CastActionId == 0
                        ? string.Empty
                        : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(LocalPlayer.CastActionId)) ? "Unknown" : ActionWatching.GetActionName(LocalPlayer.CastActionId))} (ID: {LocalPlayer.CastActionId})");
                CustomStyleText("Animation Lock:",
                    $"{ActionManager.Instance()->AnimationLock:F1}");
                CustomStyleText("Queued Action:",
                    ActionManager.Instance()->QueuedActionId.ActionName());
            }

            #endregion

            ImGuiEx.Spacing(new Vector2(0, 20));

            #region Misc (BLU)

            if (ImGui.CollapsingHeader("Miscellaneous Info"))
            {
                if (ImGui.CollapsingHeader("Active Blue Mage Spells"))
                {
                    ImGui.TextUnformatted(
                        $"{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(ActionWatching.GetActionName).OrderBy(x => x))}");
                }
                ImGui.Indent();
                if (ImGui.CollapsingHeader("Enmity"))
                {
                    foreach (var h in EnmityDictParty)
                    {
                        CustomStyleText($"{Svc.Objects.First(x => x.GameObjectId == h.Key).Name}", h.Value);
                    }

                    ImGui.Spacing();
                    CustomStyleText($"\"Strongest\" DPS:", $"{StrongestDPS()?.Name}");
                }
                ImGui.Unindent();
                if (WrathOpener.CurrentOpener is not null)
                {
                    CustomStyleText("Current Opener",
                        WrathOpener.CurrentOpener.GetType());
                    CustomStyleText("Opener State:",
                        WrathOpener.CurrentOpener.CurrentState);
                    CustomStyleText("Current Opener Action:",
                        WrathOpener.CurrentOpener.CurrentOpenerAction
                            .ActionName());
                    CustomStyleText("Current Opener Step:",
                        WrathOpener.CurrentOpener.OpenerStep);
                    if (WrathOpener.CurrentOpener.OpenerActions.Count > 0 &&
                        WrathOpener.CurrentOpener.OpenerStep <
                        WrathOpener.CurrentOpener.OpenerActions.Count)
                    {
                        CustomStyleText("Next Action:",
                            WrathOpener.CurrentOpener
                                .OpenerActions[WrathOpener.CurrentOpener.OpenerStep]
                                .ActionName());
                        CustomStyleText("Is Delayed Weave:",
                            WrathOpener.CurrentOpener.DelayedWeaveSteps
                                .Any(x => x == WrathOpener.CurrentOpener.OpenerStep));
                        CustomStyleText("Can Delayed Weave:",
                            CanDelayedWeave(end: 0.1));
                    }
                }

                CustomStyleText("Countdown Remaining:",
                    $"{CountdownActive} {CountdownRemaining}");
                CustomStyleText("Raidwide Inc:", $"{RaidWideCasting()}");
            }

            #endregion

            ImGuiEx.Spacing(new Vector2(0, 20));

            #region IPC

            void WrathIPCCallback(int cancellationReason, string extraInfo)
            {
                _wrathLease = null;
            }

            if (ImGui.CollapsingHeader("IPC"))
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
                    CustomStyleText("Configurations: ",
                        $"{P.IPC.Leasing.Registrations[_wrathLease!.Value].SetsLeased}");

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
                        P.IPC.Leasing.AddRegistrationForCurrentJob(
                            _wrathLease!.Value, Job.WHM);
                    }

                    ImGuiEx.Spacing(new Vector2(20, 20));

                    ImGui.Indent();
                    if (ImGui.Button("Mimic AutoDuty"))
                    {
                        // https://github.com/ffxivcode/AutoDuty/blob/master/AutoDuty/IPC/IPCSubscriber.cs#L460
                        if (!P.IPC.IsCurrentJobAutoRotationReady())
                            P.IPC.SetCurrentJobAutoRotationReady(_wrathLease!.Value);
                        P.IPC.SetAutoRotationState(_wrathLease!.Value);
                        P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.InCombatOnly,
                            false);
                        P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.AutoRez,
                            true);
                        P.IPC.SetAutoRotationConfigState(_wrathLease!.Value,
                            AutoRotationConfigOption.AutoRezDPSJobs, true);
                        P.IPC.SetAutoRotationConfigState(_wrathLease!.Value, AutoRotationConfigOption.IncludeNPCs,
                            true);
                        P.IPC.SetAutoRotationConfigState(_wrathLease!.Value,
                            AutoRotationConfigOption.DPSRotationMode, DPSRotationMode.Lowest_Current);
                        P.IPC.SetAutoRotationConfigState(_wrathLease!.Value,
                            AutoRotationConfigOption.HealerRotationMode, HealerRotationMode.Lowest_Current);
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Mimic Questionable"))
                    {
                        // https://git.carvel.li/liza/Questionable/src/commit/de90882ecbb609c2f79fecc1ec17b751dc8763f2/Questionable/Controller/CombatModules/WrathComboModule.cs#L68
                        P.IPC.SetAutoRotationState(_wrathLease!.Value);
                        P.IPC.SetCurrentJobAutoRotationReady(_wrathLease!.Value);
                    }
                }

                ImGuiEx.Spacing(new Vector2(0, 10));

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
                        if (ImGui.Button("Release"))
                        {
                            P.IPC.ReleaseControl(registration.Key);
                        }
                        ImGui.SameLine();

                        CustomStyleText(
                            "",
                            $"Jobs: {jobs,-30} " +
                            $"Combos: {combos,-6}");
                        CustomStyleText(
                            "",
                            $"Created: {" ",-24} {registration.Value.Created:yyyy-MM-ddTHH:mm:ss}");
                    }
                }
                else
                {
                    CustomStyleText("No current leases", "");
                }

                ImGuiEx.Spacing(new Vector2(0, 15));

                if (ImGui.CollapsingHeader("OrbwalkerIPC"))
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
                            OrbwalkerIPC.SetEnabledJob((uint) Player.Job, jobs.All(x => x != (int)Player.Job));
                        }
                        ImGui.Unindent();
                    }
                }
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
}
