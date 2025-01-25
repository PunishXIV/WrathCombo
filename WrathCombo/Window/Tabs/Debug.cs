using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;
using System.Linq;
using System.Numerics;
using WrathCombo.AutoRotation;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Services.IPC;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Action = Lumina.Excel.Sheets.Action;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace WrathCombo.Window.Tabs
{
    internal class Debug : ConfigWindow
    {
        public static int debugNum = 0;
        public static Guid? WrathLease;

        internal static Action? debugSpell;
        internal unsafe static new void Draw()
        {
            IPlayerCharacter? LocalPlayer = Svc.ClientState.LocalPlayer;
            uint[] statusBlacklist = { 360, 361, 362, 363, 364, 365, 366, 367, 368 }; // Duration will not be displayed for these status effects
            var target = Svc.Targets.Target;


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

            if (LocalPlayer != null)
            {
                // Player Status Effects
                if (ImGui.CollapsingHeader("Player Status Effects"))
                {
                    foreach (Status? status in LocalPlayer.StatusList)
                    {
                        float buffDuration = GetBuffRemainingTime((ushort)status.StatusId, false);
                        string formattedDuration = "";
                        if (buffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                        {
                            if (buffDuration >= 60)
                            {
                                int minutes = (int)(buffDuration / 60);
                                formattedDuration = $"{minutes}m";
                            }
                            else formattedDuration = $"{buffDuration:F1}s";
                        }

                        // Null Check (Status Name)
                        if (!string.IsNullOrEmpty(ActionWatching.GetStatusName(status.StatusId)))
                        {
                            CustomStyleText(ActionWatching.GetStatusName(status.StatusId) + ":", $"{status.StatusId} P: {status.Param}, {formattedDuration}");
                        }
                        else CustomStyleText("", status.StatusId);
                    }
                }

                // Target Status Effects
                if (ImGui.CollapsingHeader("Target Status Effects"))
                {
                    if (target != null && target is IBattleChara chara)
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
                            float debuffDuration = GetDebuffRemainingTime((ushort)status.StatusId, false);
                            if (debuffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                            {
                                string formattedDuration;
                                if (debuffDuration >= 60)
                                {
                                    int minutes = (int)(debuffDuration / 60);
                                    formattedDuration = $"{minutes}m";
                                }
                                else formattedDuration = $"{debuffDuration:F1}s";

                                ImGui.SameLine(0, 4f);
                                CustomStyleText("", $"({formattedDuration})");
                            }
                        }

                    }
                }
                if (ImGui.CollapsingHeader("Action Info"))
                {
                    var actions = Svc.Data.GetExcelSheet<Action>()!.Where(x => x.ClassJobLevel > 0 && x.ClassJobCategory.RowId != 1 && x.ClassJobCategory.Value.IsJobInCategory(Player.Job)).OrderBy(x => x.ClassJobLevel);
                    string prev = debugSpell == null ? "Select Action" : $"({debugSpell.Value.RowId}) Lv.{debugSpell.Value.ClassJobLevel}. {debugSpell.Value.Name} - {(debugSpell.Value.IsPvP ? "PvP" : "Normal")}";
                    ImGuiEx.SetNextItemFullWidth();
                    using (var comboBox = ImRaii.Combo("###ActionCombo", prev))
                    {
                        if (comboBox)
                        {
                            if (ImGui.Selectable("", debugSpell == null))
                            {
                                debugSpell = null;
                            }

                            var classId = JobIDs.JobToClass(JobID!.Value);
                            var cjc = Svc.Data.Excel.GetRawSheet("ClassJobCategory");
                            var cjcColumIdx = cjc.Columns[(int)JobID.Value];

                            foreach (var act in actions)
                            {
                                if (ImGui.Selectable($"({act.RowId}) Lv.{act.ClassJobLevel}. {act.Name} - {(act.IsPvP ? "PvP" : "Normal")}", debugSpell?.RowId == act.RowId))
                                {
                                    debugSpell = act;
                                }
                            }
                        }
                    }

                    if (debugSpell != null)
                    {
                        var actionStatus = ActionManager.Instance()->GetActionStatus(ActionType.Action, debugSpell.Value.RowId);
                        var icon = Svc.Texture.GetFromGameIcon(new(debugSpell.Value.Icon)).GetWrapOrEmpty().ImGuiHandle;
                        ImGui.Image(icon, new System.Numerics.Vector2(60f.Scale(), 60f.Scale()));
                        ImGui.SameLine();
                        ImGui.Image(icon, new System.Numerics.Vector2(30f.Scale(), 30f.Scale()));
                        CustomStyleText($"Action Status:", $"{actionStatus} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(actionStatus).Text})");
                        CustomStyleText($"Action Type:", debugSpell.Value.ActionCategory.Value.Name);
                        if (debugSpell.Value.UnlockLink.RowId != 0)
                            CustomStyleText($"Quest:", $"{Svc.Data.GetExcelSheet<Quest>().GetRow(debugSpell.Value.UnlockLink.RowId).Name} ({(UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(debugSpell.Value.UnlockLink.RowId) ? "Completed" : "Not Completed")})");
                        CustomStyleText($"Base Recast:", $"{debugSpell.Value.Recast100ms / 10f}s");
                        CustomStyleText("Original Hook:", OriginalHook(debugSpell.Value.RowId).ActionName());
                        CustomStyleText($"Cooldown Total:", $"{ActionManager.Instance()->GetRecastTime(ActionType.Action, debugSpell.Value.RowId)}");
                        CustomStyleText($"Current Cooldown:", GetCooldown(debugSpell.Value.RowId).CooldownRemaining);
                        CustomStyleText($"Current Cast Time:", ActionManager.GetAdjustedCastTime(ActionType.Action, debugSpell.Value.RowId));
                        CustomStyleText($"Max Charges:", $"{debugSpell.Value.MaxCharges}");
                        CustomStyleText($"Charges (Level):", $"{GetCooldown(debugSpell.Value.RowId).MaxCharges}");
                        CustomStyleText($"Range:", $"{ActionWatching.GetActionRange(debugSpell.Value.RowId)}");
                        CustomStyleText($"Effect Range:", $"{debugSpell.Value.EffectRange}");
                        CustomStyleText($"Can Target Hostile:", $"{debugSpell.Value.CanTargetHostile}");
                        CustomStyleText($"Can Target Self:", $"{debugSpell.Value.CanTargetSelf}");
                        CustomStyleText($"Can Target Friendly:", $"{debugSpell.Value.CanTargetAlly}");
                        CustomStyleText($"Can Target Party:", $"{debugSpell.Value.CanTargetParty}");
                        CustomStyleText($"Can Target Area:", $"{debugSpell.Value.TargetArea}");
                        CustomStyleText($"Cast Type:", $"{debugSpell.Value.CastType}");
                        CustomStyleText("Can Queue:", $"{CanQueue(debugSpell.Value.RowId)}");
                        if (debugSpell.Value.EffectRange > 0)
                            CustomStyleText($"Targets Hit:", $"{NumberOfEnemiesInRange(debugSpell.Value.RowId, CurrentTarget)}");

                        if (ActionWatching.ActionTimestamps.ContainsKey(debugSpell.Value.RowId))
                            CustomStyleText($"Time Since Last Use:", $"{(Environment.TickCount64 - ActionWatching.ActionTimestamps[debugSpell.Value.RowId]) / 1000f:F2}");

                        if (ActionWatching.LastSuccessfulUseTime.ContainsKey(debugSpell.Value.RowId))
                            CustomStyleText($"Last Successful Cast:", $"{ActionWatching.TimeSinceLastSuccessfulCast(debugSpell.Value.RowId) / 1000f:F2}");

                        if (Svc.Targets.Target != null)
                        {
                            var inRange = ActionManager.GetActionInRangeOrLoS(debugSpell.Value.RowId, (GameObject*)LocalPlayer.Address, (GameObject*)Svc.Targets.Target.Address);
                            CustomStyleText("InRange or LoS:", inRange == 0 ? "In range and in line of sight" : $"{inRange}: {Svc.Data.GetExcelSheet<LogMessage>().GetRow(inRange).Text}");
                            var canUseOnTarget = ActionManager.CanUseActionOnTarget(debugSpell.Value.RowId, Svc.Targets.Target.Struct());
                            CustomStyleText($"Can Use on Target:", canUseOnTarget);
                        }
                        var canUseOnSelf = ActionManager.CanUseActionOnTarget(debugSpell.Value.RowId, Player.GameObject);
                        CustomStyleText($"Can Use on Self:", canUseOnSelf);

                        Util.ShowObject(debugSpell.Value);
                    }

                    ImGuiEx.TextUnderlined("Action Readys");
                    foreach (var act in actions)
                    {
                        var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, act.RowId, checkRecastActive: false, checkCastingActive: false);
                        CustomStyleText(act.Name.ExtractText(), $"{ActionReady(act.RowId)}, {status} ({Svc.Data.GetExcelSheet<LogMessage>().GetRow(status).Text})");
                    }
                }

                // Player Info
                ImGui.Spacing();
                ImGui.Text("Player Info");
                ImGui.Separator();
                CustomStyleText("Job:", $"{LocalPlayer.ClassJob.Value.NameEnglish} (ID: {LocalPlayer.ClassJob.RowId})");
                CustomStyleText("Zone:", $"{Svc.Data.GetExcelSheet<TerritoryType>()?.FirstOrDefault(x => x.RowId == Svc.ClientState.TerritoryType).PlaceName.Value.Name} (ID: {Svc.ClientState.TerritoryType})");
                CustomStyleText("In PvP:", InPvP());
                CustomStyleText("In Combat:", InCombat());
                CustomStyleText("In Boss:", InBossEncounter());
                CustomStyleText("Cast Time:", LocalPlayer.TotalCastTime - LocalPlayer.CurrentCastTime);
                CustomStyleText("Hitbox Radius:", LocalPlayer.HitboxRadius);
                CustomStyleText("In FATE:", InFATE());
                CustomStyleText("Time in Combat:", CombatEngageDuration().ToString("mm\\:ss"));
                CustomStyleText("Party Combat Time:", PartyEngageDuration().ToString("mm\\:ss"));
                CustomStyleText("Limit Break:", LimitBreakValue);
                CustomStyleText("LBs Ready:", $"1.{IsLB1Ready} 2.{IsLB2Ready} 3.{IsLB3Ready}");
                CustomStyleText("LB Level:", LimitBreakLevel);
                CustomStyleText("LB Action:", LimitBreakAction.ActionName());
                CustomStyleText("Animation Lock:", ActionManager.Instance()->AnimationLock);
                CustomStyleText("Movement Timer:", TimeMoving.ToString("mm\\:ss\\:ff"));
                CustomStyleText("Alliance Group:", GetAllianceGroup());
                ImGui.Spacing();

                ImGui.Spacing();
                ImGui.Text($"Job Gauge");
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


                // Target Info
                ImGui.Spacing();
                ImGui.Text("Target Info");
                ImGui.Separator();
                CustomStyleText("ObjectId:", target?.GameObjectId);
                CustomStyleText("ObjectKind:", target?.ObjectKind);
                CustomStyleText("Is BattleChara:", target is IBattleChara);
                CustomStyleText("Is PlayerCharacter:", target is IPlayerCharacter);
                CustomStyleText("Distance:", $"{Math.Round(GetTargetDistance(), 2)}y");
                CustomStyleText("Hitbox Radius:", target?.HitboxRadius);
                CustomStyleText("In Melee Range:", InMeleeRange());
                CustomStyleText("Requires Postionals:", TargetNeedsPositionals());
                CustomStyleText("Relative Position:", AngleToTarget() is 2 ? "Rear" : (AngleToTarget() is 1 or 3) ? "Flank" : AngleToTarget() is 4 ? "Front" : "");
                CustomStyleText("Health:", $"{EnemyHealthCurrentHp().ToString("N0")} / {EnemyHealthMaxHp().ToString("N0")} ({Math.Round(GetTargetHPPercent(), 2)}%)");
                CustomStyleText("Shield:", (GetHealTarget() as ICharacter).ShieldPercentage);
                CustomStyleText("Health Percent (+ Shield):", $"{GetTargetHPPercent(GetHealTarget())} / {GetTargetHPPercent(GetHealTarget(), true)}");
                CustomStyleText("Party Avg HP Percent:", $"{GetPartyAvgHPPercent()}");

                ImGui.Indent();
                if (ImGui.CollapsingHeader("Relative Target Distances"))
                {
                    ImGuiEx.TextUnderlined("Enemies");
                    var enemies = Svc.Objects.Where(x => x != null && x.ObjectKind == ObjectKind.BattleNpc && x.IsTargetable && !x.IsDead).Cast<IBattleNpc>().Where(x => x.BattleNpcKind is BattleNpcSubKind.Enemy or BattleNpcSubKind.BattleNpcPart).ToList();
                    foreach (var enemy in enemies)
                    {
                        if (enemy.GameObjectId == Svc.Targets.Target?.GameObjectId) continue;
                        if (!enemy.Character()->InCombat) continue;
                        var dist = GetTargetDistance(enemy, Svc.Targets.Target);
                        CustomStyleText($"{enemy.Name} ({enemy.GameObjectId}):", $"{dist:F1}");
                    }
                }
                ImGui.Unindent();
                ImGui.Spacing();

                // Action Info
                ImGui.Spacing();
                ImGui.Text("Action Info");
                ImGui.Separator();
                CustomStyleText("GCD Total:", GCDTotal);
                CustomStyleText("Time Since Last Action:", $"{ActionWatching.TimeSinceLastAction}");
                CustomStyleText("Last Action:", ActionWatching.LastAction == 0 ? string.Empty : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(ActionWatching.LastAction)) ? "Unknown" : ActionWatching.GetActionName(ActionWatching.LastAction))} (ID: {ActionWatching.LastAction})");
                CustomStyleText("Last Action Cost:", GetResourceCost(ActionWatching.LastAction));
                CustomStyleText("Last Action Type:", ActionWatching.GetAttackType(ActionWatching.LastAction));
                CustomStyleText("Last Weaponskill:", ActionWatching.GetActionName(ActionWatching.LastWeaponskill));
                CustomStyleText("Last Spell:", ActionWatching.GetActionName(ActionWatching.LastSpell));
                CustomStyleText("Last Ability:", ActionWatching.GetActionName(ActionWatching.LastAbility));
                CustomStyleText("Combo Timer:", $"{ComboTimer:F1}");
                CustomStyleText("Combo Action:", ComboAction == 0 ? string.Empty : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(ComboAction)) ? "Unknown" : ActionWatching.GetActionName(ComboAction))} (ID: {ComboAction})");
                CustomStyleText("Cast Time:", $"{LocalPlayer.CurrentCastTime:F2} / {LocalPlayer.TotalCastTime:F2}");
                CustomStyleText("Cast Action:", LocalPlayer.CastActionId == 0 ? string.Empty : $"{(string.IsNullOrEmpty(ActionWatching.GetActionName(LocalPlayer.CastActionId)) ? "Unknown" : ActionWatching.GetActionName(LocalPlayer.CastActionId))} (ID: {LocalPlayer.CastActionId})");
                CustomStyleText("Animation Lock:", $"{ActionManager.Instance()->AnimationLock:F1}");
                CustomStyleText("Queued Action:", ActionManager.Instance()->QueuedActionId.ActionName());
                ImGui.Spacing();

                // Party Info
                ImGui.Spacing();
                ImGui.Text("Party Info");
                ImGui.Separator();
                CustomStyleText("Party ID:", Svc.Party.PartyId);
                CustomStyleText("Party Size:", GetPartyMembers().Count);
                if (ImGui.CollapsingHeader("Party Members"))
                {
                    ImGui.Indent();
                    foreach (var member in GetPartyMembers())
                    {
                        if (ImGui.CollapsingHeader(member.BattleChara.GetInitials()))
                        {
                            CustomStyleText("Job:", member.BattleChara.ClassJob.Value.Abbreviation);
                            CustomStyleText($"HP:", $"{member.CurrentHP}/{member.BattleChara.MaxHp}");
                            CustomStyleText("MP:", $"{member.CurrentMP}/{member.BattleChara.MaxMp}");
                            CustomStyleText("Dead Timer:", TimeSpentDead(member.BattleChara.GameObjectId));
                        }
                    }
                    ImGui.Unindent();
                }
                ImGui.Spacing();

                // Misc. Info
                ImGui.Spacing();
                ImGui.Text("Miscellaneous Info");
                ImGui.Separator();
                if (ImGui.CollapsingHeader("Active Blue Mage Spells"))
                {
                    ImGui.TextUnformatted($"{string.Join("\n", Service.Configuration.ActiveBLUSpells.Select(x => ActionWatching.GetActionName(x)).OrderBy(x => x))}");
                }

                if (WrathOpener.CurrentOpener is not null)
                {
                    CustomStyleText($"Current Opener", WrathOpener.CurrentOpener?.GetType());
                    CustomStyleText("Opener State:", WrathOpener.CurrentOpener?.CurrentState);
                    CustomStyleText("Current Opener Action:", WrathOpener.CurrentOpener?.CurrentOpenerAction.ActionName());
                    CustomStyleText("Current Opener Step:", WrathOpener.CurrentOpener?.OpenerStep);
                    if (WrathOpener.CurrentOpener.OpenerActions.Count > 0 && WrathOpener.CurrentOpener.OpenerStep < WrathOpener.CurrentOpener.OpenerActions.Count)
                    {
                        CustomStyleText("Next Action:", WrathOpener.CurrentOpener?.OpenerActions[WrathOpener.CurrentOpener.OpenerStep].ActionName());
                        CustomStyleText("Is Delayed Weave:", WrathOpener.CurrentOpener?.DelayedWeaveSteps.Any(x => x == WrathOpener.CurrentOpener?.OpenerStep));
                        CustomStyleText($"Can Delayed Weave:", CanDelayedWeave(end: 0.1));
                    }
                }

                CustomStyleText("Countdown Remaining:", $"{CountdownActive} {CountdownRemaining}");
                CustomStyleText("Raidwide Inc:", $"{RaidWideCasting()}");

                // IPC
                if (ImGui.CollapsingHeader("IPC"))
                {
                    CustomStyleText("Wrath Leased:", WrathLease is not null);
                    foreach (var registration in P.IPC.Leasing.Registrations)
                    {
                        CustomStyleText($"{registration.Key}", $"{registration.Value}");
                    }
                    if (WrathLease is null)
                    {
                        if (ImGui.Button("Register"))
                        {
                            WrathLease = P.IPC.RegisterForLease("WrathCombo", "WrathCombo");
                        }
                    }
                    if (WrathLease is not null)
                    {
                        if (ImGui.Button("Release"))
                        {
                            P.IPC.ReleaseControl(WrathLease.Value);
                            WrathLease = null;
                        }
                        if (ImGui.Button("Set Autorot For Job"))
                        {
                            P.IPC.SetCurrentJobAutoRotationReady(WrathLease.Value);
                        }
                        if (ImGui.Button("Mimic AD IPC"))
                        {
                            P.IPC.SetCurrentJobAutoRotationReady(WrathLease.Value);
                            P.IPC.SetAutoRotationState(WrathLease!.Value, true);
                            P.IPC.SetAutoRotationConfigState(WrathLease.Value, AutoRotationConfigOption.InCombatOnly, false);
                            P.IPC.SetAutoRotationConfigState(WrathLease.Value, AutoRotationConfigOption.AutoRez, true);
                            P.IPC.SetAutoRotationConfigState(WrathLease.Value, AutoRotationConfigOption.AutoRezDPSJobs, true);
                            P.IPC.SetAutoRotationConfigState(WrathLease.Value, AutoRotationConfigOption.IncludeNPCs, true);
                            P.IPC.SetAutoRotationConfigState(WrathLease.Value, AutoRotationConfigOption.DPSRotationMode, DPSRotationMode.Lowest_Current);
                            P.IPC.SetAutoRotationConfigState(WrathLease.Value, AutoRotationConfigOption.HealerRotationMode, HealerRotationMode.Lowest_Current);

                        }
                    }
                }

            }
            else
            {
                ImGui.TextUnformatted("Please log into the game to use this tab.");
            }
        }
    }
}
