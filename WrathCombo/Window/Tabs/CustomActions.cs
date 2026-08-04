using Dalamud.Game.Config;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System.Numerics;
using WrathCombo.Combos.PvE;
using WrathCombo.Native;
using WrathCombo.Services;

namespace WrathCombo.Window.Tabs
{
    internal class CustomActions : ConfigWindow
    {
        private static CustomAction? SelectedAction;
        private static bool DragDropMode;
        private static uint HiddenSlots;
        private const UiControlOption HotbarSetting = UiControlOption.HotbarEmptyVisible;
        internal unsafe static new void Draw()
        {
            Vector2 gap = new Vector2(16f.Scale());
            ImGuiEx.TextWrapped($@"This section contains a range of bespoke ""Custom Actions"" which can be used as replacements for either base action replacements or specific macros.");

            ImGui.Separator();

            ImGuiEx.TextUnderlined($"Rotation Buttons");
            ImGuiEx.TextWrapped($"These buttons replace the need to replace actions for all features marked as either Simple or Advanced. Enable the type of combo you're wanting to use custom actions for, and drag the icon to your hotbar.\n\n" +
                $"This frees up the actions for manual use, for example, for downtime where you only want to use 1-2-3 combos, or for healers to always have access to basic healing actions.");

            ImGui.Dummy(gap);

            using (var table = ImRaii.Table($"CustomActionsTable", 3))
            {
                if (!table)
                    return;

                ImGui.TableSetupColumn("Setting", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X);

                foreach (var act in P.CustomActions.Manager.Actions)
                {
                    if (act.Id > All.AoeHeals)
                        continue;

                    DrawSettingAction(act);
                }
            }


            if (ImGuiEx.CheckboxWrapped($"Don't Override Icons with Job Actions (drag action to hotbar again to take effect)", ref Service.Configuration.CustomActionSettings.AlwaysShowIcon))
                Service.Configuration.Save();

            ImGuiComponents.HelpMarker("This will hide all combo outputs on custom actions, leaving only these icons showing. This is not advised if you wish to see the real actions being output by the combos on your hotbar, and should only be used if you feel it's something you want.");

            ImGui.Dummy(gap);

            ImGuiEx.TextUnderlined("Utility Buttons");
            ImGuiEx.TextWrapped($"These are buttons which serve to control the plugin in some way. Just drag the icon to your hotbar to use them.");

            ImGui.Dummy(gap);

            ImGui.Columns(3, border: false);
            foreach (var act in P.CustomActions.Manager.Actions)
            {
                if (act.Id is > All.Cease and < All.Items)
                {
                    DrawUtilityAction(act);
                    ImGui.NextColumn();

                    if (ImGui.GetColumnIndex() == 0)
                        ImGui.Separator();
                }
            }
            ImGui.Columns(1);

            if (ImGui.GetIO().MouseDownDuration[0] > 0.5f)
                DragDropMode = true;

            if (SelectedAction != null && P.CustomActions.Manager.IconTextures[SelectedAction.IconId].TryGetWrap(out var texture, out _))
            {
                var mousePos = ImGui.GetMousePos();
                mousePos.X -= 5;
                mousePos.Y -= 5;

                ImGui.SetNextWindowPos(mousePos);
                ImGui.SetNextWindowBgAlpha(0.5f);

                ImGui.Begin(
                    $"{SelectedAction.Name}DragDrops",
                    ImGuiWindowFlags.NoDecoration |
                    ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.Image(texture.Handle, new(50f.Scale()));
                ImGui.End();

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && DragDropMode))
                {
                    if (P.CustomActions.HoveredSlot != null)
                    {
                        var val = P.CustomActions.HoveredSlot.Value;
                        RaptureHotbarModule.Instance()->Hotbars[val.Hotbar].Slots[val.Slot].Set(RaptureHotbarModule.HotbarSlotType.Action, SelectedAction.Id);
                        var actualSlot = RaptureHotbarModule.Instance()->Hotbars[val.Hotbar].Slots[val.Slot];
                        RaptureHotbarModule.Instance()->WriteSavedSlot(Player.ClassJob.RowId, (uint)val.Hotbar, (uint)val.Slot, &actualSlot, false, false);
                    }
                    SelectedAction = null;
                    DragDropMode = false;
                    Svc.GameConfig.Set(HotbarSetting, HiddenSlots);
                }

            }
        }

        private static unsafe void DrawSettingAction(CustomAction act)
        {
            using (ImRaii.Disabled(SelectedAction != null))
            {
                if (P.CustomActions.Manager.IconTextures[act.IconId].TryGetWrap(out var texture, out _))
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    var type = CustomActionHelper.GetCustomActionType(act.Id);
                    bool changed = false;
                    switch (type)
                    {
                        case CustomActionType.SingleTargetDPS:
                            changed |= ImGui.Checkbox($"{act.Name}##Custom{act.Id}", ref Service.Configuration.CustomActionSettings.SingleTargetDPS);
                            break;
                        case CustomActionType.AoEDPS:
                            changed |= ImGui.Checkbox($"{act.Name}##Custom{act.Id}", ref Service.Configuration.CustomActionSettings.AoEDPS);
                            break;
                        case CustomActionType.SingleTargetHeals:
                            changed |= ImGui.Checkbox($"{act.Name}##Custom{act.Id}", ref Service.Configuration.CustomActionSettings.SingleTargetHeals);
                            break;
                        case CustomActionType.AoEHeals:
                            changed |= ImGui.Checkbox($"{act.Name}##Custom{act.Id}", ref Service.Configuration.CustomActionSettings.AoEHeals);
                            break;

                    }

                    if (changed)
                        Service.Configuration.Save();

                    var btnSize = ImGui.GetFrameHeight() * 2.5f.Scale();
                    ImGui.TableNextColumn();
                    ImGui.ImageButton(texture.Handle, new(btnSize));

                    if (ImGui.IsItemHovered())
                    {
                        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                        {
                            Svc.GameConfig.TryGet(HotbarSetting, out HiddenSlots);
                            Svc.Log.Debug($"User has slots {(HiddenSlots == 0 ? "hidden" : "shown")}");
                        }
                        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                        {
                            SelectedAction = act;
                            Svc.GameConfig.Set(HotbarSetting, 1);
                        }

                        ImGui.BeginTooltip();
                        ImGui.Image(texture.Handle, new(50));
                        ImGui.SameLine();
                        var pos = ImGui.GetCursorPos();
                        ImGuiEx.Text($"{act.Name}");
                        ImGui.SetCursorPosX(pos.X);
                        ImGui.SetCursorPosY(pos.Y + 20f.Scale());
                        ImGuiEx.Text($"{act.Description}");
                        ImGui.EndTooltip();
                    }


                    ImGui.TableNextColumn();
                    ImGuiEx.TextWrapped($"{act.Description}");
                }
            }
        }

        private static unsafe void DrawUtilityAction(CustomAction act)
        {
            using (ImRaii.Disabled(SelectedAction != null))
            {
                if (P.CustomActions.Manager.IconTextures[act.IconId].TryGetWrap(out var texture, out _))
                {
                    ImGuiEx.TextWrapped($"{act.Name}");
                    var btnSize = ImGui.GetFrameHeight() * 2.5f.Scale();
                    ImGui.ImageButton(texture.Handle, new(btnSize));

                    if (ImGui.IsItemHovered())
                    {
                        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                        {
                            Svc.GameConfig.TryGet(HotbarSetting, out HiddenSlots);
                            Svc.Log.Debug($"User has slots {(HiddenSlots == 0 ? "hidden" : "shown")}");
                        }
                        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                        {
                            SelectedAction = act;
                            Svc.GameConfig.Set(HotbarSetting, 1);
                        }

                        ImGui.BeginTooltip();
                        ImGui.Image(texture.Handle, new(50));
                        ImGui.SameLine();
                        var pos = ImGui.GetCursorPos();
                        ImGuiEx.Text($"{act.Name}");
                        ImGui.SetCursorPosX(pos.X);
                        ImGui.SetCursorPosY(pos.Y + 20f.Scale());
                        ImGuiEx.Text($"{act.Description}");
                        ImGui.EndTooltip();
                    }

                    ImGuiEx.TextWrapped($"{act.Description}");
                }
            }
        }
    }
}
