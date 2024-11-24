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
using WrathCombo.Combos;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Services;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Action = Lumina.Excel.Sheets.Action;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace WrathCombo.Window.Tabs
{
    internal class Debug : ConfigWindow
    {
        public static int debugNum = 0;

        internal class DebugCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; }
            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) => actionID;
        }

        internal static Lumina.Excel.Sheets.Action? debugSpell;
        internal unsafe static new void Draw()
        {
            DebugCombo? comboClass = new();
            IPlayerCharacter? LocalPlayer = Svc.ClientState.LocalPlayer;
            uint[] statusBlacklist = { 360, 361, 362, 363, 364, 365, 366, 367, 368 }; // Duration will not be displayed for these status effects
            var target = Svc.Targets.Target;


            // Custom Styling
            static void CustomStyleText(string label, object? value)
            {
                if (!string.IsNullOrEmpty(label))
                {
                    ImGui.TextUnformatted(label);
                    ImGui.SameLine(0, 4f);
                }
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                ImGui.TextUnformatted(value?.ToString() ?? "");
                ImGui.PopStyleColor();
            }

            if (LocalPlayer != null)
            {
                // Player Status Effects
                if (ImGui.CollapsingHeader("Player Status Effects"))
                {
                    foreach (Status? status in LocalPlayer.StatusList)
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
                        float buffDuration = GetBuffRemainingTime((ushort)status.StatusId, false);
                        if (buffDuration != 0 && !statusBlacklist.Contains(status.StatusId))
                        {
                            string formattedDuration;
                            if (buffDuration >= 60)
                            {
                                int minutes = (int)(buffDuration / 60);
                                formattedDuration = $"{minutes}m";
                            }
                            else formattedDuration = $"{buffDuration:F1}s";

                            ImGui.SameLine(0, 4f);
                            CustomStyleText("", $"({formattedDuration})");
                        }
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

                            var classId = CustomComboFunctions.JobIDs.JobToClass(JobID!.Value);
                            var cjc = Svc.Data.Excel.GetRawSheet("ClassJobCategory");
                            var cjcColumIdx = cjc.Columns[(int)JobID.Value];

                            foreach (var act in Svc.Data.GetExcelSheet<Action>()!.Where(x => x.IsPlayerAction && (x.ClassJob.RowId == classId || x.ClassJob.RowId == JobID.Value)).OrderBy(x => x.ClassJobLevel))
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
                        CustomStyleText($"Current Cast Time:", ActionManager.GetAdjustedCastTime(ActionType.Action, debugSpell.Value.RowId));
                        CustomStyleText($"Max Charges:", $"{debugSpell.Value.MaxCharges}");
                        CustomStyleText($"Range:", $"{debugSpell.Value.Range}");
                        CustomStyleText($"Effect Range:", $"{debugSpell.Value.EffectRange}");
                        CustomStyleText($"Can Target Hostile:", $"{debugSpell.Value.CanTargetHostile}");
                        CustomStyleText($"Can Target Self:", $"{debugSpell.Value.CanTargetSelf}");
                        CustomStyleText($"Can Target Friendly:", $"{debugSpell.Value.CanTargetAlly}");
                        CustomStyleText($"Can Target Party:", $"{debugSpell.Value.CanTargetParty}");
                        CustomStyleText($"Can Target Area:", $"{debugSpell.Value.TargetArea}");
                        CustomStyleText($"Cast Type:", $"{debugSpell.Value.CastType}");
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
                CustomStyleText("Hitbox Radius:", LocalPlayer.HitboxRadius);
                CustomStyleText("In FATE:", InFATE());
                CustomStyleText("Time in Combat:", CombatEngageDuration().ToString("mm\\:ss"));
                CustomStyleText("Party Combat Time:", PartyEngageDuration().ToString("mm\\:ss"));
                CustomStyleText("Limit Break:", LimitBreakValue);
                CustomStyleText("LBs Ready:", $"1.{IsLB1Ready} 2.{IsLB2Ready} 3.{IsLB3Ready}");
                CustomStyleText("LB Level:", LimitBreakLevel);
                CustomStyleText("LB Action:", LimitBreakAction.ActionName());
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
                    for (int i = 1; i <= 8; i++)
                    {
                        if (GetPartySlot(i) is not IBattleChara member || member is null) continue;
                        if (ImGui.CollapsingHeader(member.Name.ToString()))
                        {
                            CustomStyleText("Slot:", i);
                            CustomStyleText("Job:", member.ClassJob.Value.Abbreviation);
                            CustomStyleText("Dead Timer:", TimeSpentDead(member.GameObjectId));
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
            }

            else
            {
                ImGui.TextUnformatted("Please log into the game to use this tab.");
            }
        }
    }
}
