using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Schedulers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Lumina.Excel.Sheets.Action;

namespace WrathCombo.Services.ActionRequestIPC;
public static class ActionRequestDebugUI
{
    static bool? IsGCD;
    public static void Draw()
    {
        DrawList(ActionRequestIPCProvider.ActionBlacklist, "Blacklist", 
            (a, t) => ActionRequestIPCProvider.RequestBlacklist(a.ActionType, a.ActionID, t),
            a => ActionRequestIPCProvider.ResetBlacklist(a.ActionType, a.ActionID));
        ImGui.Separator();
        ImGuiEx.Checkbox("Is GCD", ref IsGCD);
        DrawList(ActionRequestIPCProvider.ActionRequests, "Requests", 
            (a, t) => ActionRequestIPCProvider.RequestActionUse(a.ActionType, a.ActionID, t, IsGCD),
            a => ActionRequestIPCProvider.ResetRequest(a.ActionType, a.ActionID));
    }

    public static void DrawList(List<ActionRequest> list, string listName, 
        Action<ActionDescriptor, int> processRequest,
        Action<ActionDescriptor> processRemovalRequest)
    {
        ImGui.PushID(listName);
        try
        {
            ImGuiEx.InputWithRightButtonsArea($"Area{listName}", () =>
            {
                ref var selectedAction = ref Ref<uint>.Get($"action{listName}");
                if(ImGui.BeginCombo("##select", selectedAction == 0 ? "Select action..." : ExcelActionHelper.GetActionName(selectedAction, true), ImGuiComboFlags.HeightLarge))
                {
                    ImGui.SetNextItemWidth(200f.Scale());
                    ImGuiEx.FilteringInputTextWithHint("##Search", "Search", out var filter);
                    foreach(var x in Svc.Data.GetExcelSheet<Action>())
                    {
                        if(!x.IsPlayerAction) continue;
                        if(x.IsPvP) continue;
                        if(!x.ClassJobCategory.Value.IsJobInCategory(Player.Job)) continue;
                        if(filter != "" && !ExcelActionHelper.GetActionName(x, true).Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
                        if(ThreadLoadImageHandler.TryGetIconTextureWrap(x.Icon,false, out var tex))
                        {
                            ImGui.Image(tex.Handle, new(ImGui.GetTextLineHeight()));
                            ImGui.SameLine();
                        }
                        if(ImGui.Selectable($"{ExcelActionHelper.GetActionName(x, true)}", selectedAction == x.RowId))
                        {
                            selectedAction = x.RowId;
                        }
                        if(ImGui.IsWindowAppearing() && selectedAction == x.RowId) ImGui.SetScrollHereY();
                    }
                    ImGui.EndCombo();
                }
            }, () =>
            {
                ImGui.SetNextItemWidth(150f.Scale());
                ImGuiEx.FilteringInputInt("Time", out var time);
                ImGui.SameLine();
                if(ImGuiEx.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.Check, "Place request"))
                {
                    processRequest(new(ActionType.Action, Ref<uint>.Get($"action{listName}")), time);
                }
            });
            if(ImGui.CollapsingHeader($"{listName} ({list.Count} requests)###ac{listName}"))
            {
                if(ImGuiEx.BeginDefaultTable(listName, ["~Action", "Time Rem", "Art. cool.", "##control"]))
                {
                    for(var i = 0; i < list.Count; i++)
                    {
                        var x = list[i];
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        var data = new RowRef<Action>(Svc.Data.Excel, x.Descriptor.ActionID);
                        if(data.IsValid && ThreadLoadImageHandler.TryGetIconTextureWrap(data.Value.Icon, false, out var tex))
                        {
                            ImGui.Image(tex.Handle, new(ImGui.GetFrameHeight()));
                            ImGui.SameLine();
                        }
                        ImGuiEx.TextV($"{ExcelActionHelper.GetActionName(x.Descriptor.ActionID, true)}");
                        ImGui.TableNextColumn();
                        var timeRem = x.Deadline - Environment.TickCount64;
                        ImGuiEx.Text(x.IsActive ? EColor.GreenBright : EColor.RedBright, $"{timeRem / 1000f:F1}");
                        ImGui.TableNextColumn();
                        ImGuiEx.Text($"{ActionRequestIPCProvider.GetArtificialCooldown(x.Descriptor.ActionType, x.Descriptor.ActionID)}");
                        ImGui.TableNextColumn();
                        if(ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Trash, $"Del{i}"))
                        {
                            new TickScheduler(() => processRemovalRequest(x.Descriptor));
                        }
                    }
                    ImGui.EndTable();
                }
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
        ImGui.PopID();
    }
}