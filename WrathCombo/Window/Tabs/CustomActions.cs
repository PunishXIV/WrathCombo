using System.Linq;
using System.Numerics;
using Dalamud.Game.Config;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
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
        private static float longestDescriptionWidth =>
            P.CustomActions.Manager.Actions
                .Max(x => ImGui.CalcTextSize(x.Description).X);

        internal unsafe static new void Draw()
        {
            ImGuiEx.TextWrapped(
                "This section is a purely optional way of replacing actions on your hotbar, using \"Custom Actions\".");

            ImGuiEx.Spacing(new Vector2(0, 10));
            ImGui.Separator();
            ImGuiEx.Spacing(new Vector2(0, 10));

            ImGuiEx.TextWrapped(
                "For all Single Button Combos (everything labeled either Simple or Advanced) you can opt to use completely custom actions on your hotbar instead of it overriding an existing job action.\n\n" +
                "This frees up the actions for manual use, for example, for downtime where you only want to use 1-2-3 combos, or for healers to always have access to basic healing actions.\n\n" +
                "To get started, enable each Combo you wish to use custom actions for.\n" +
                "Then, you can either just click once to select the item and click on a slot on your hotbar to place it, or click and hold the mouse button down, and drag it to the slot and release the mouse button.");

            ImGuiEx.Spacing(new Vector2(0, 10));
            ImGui.Separator();
            ImGuiEx.Spacing(new Vector2(0, 10));

            ImGui.Indent();
            foreach (var act in P.CustomActions.Manager.Actions)
                DrawAction(act);
            ImGui.Unindent();

            ImGuiEx.Spacing(new Vector2(0, 15));

            if (ImGui.Checkbox($"Don't Override Icons with Job Actions (drag action to hotbar again to take effect)", ref Service.Configuration.CustomActionSettings.AlwaysShowIcon))
                Service.Configuration.Save();

            ImGuiComponents.HelpMarker(
                "This will hide all combo outputs on custom actions, leaving only these icons here showing.\n" +
                "This is not advised if you wish to see the real actions being output by the combos on your hotbar, and should only be used if you particularly like these icons.");

            #region Icon Dragging

            if (ImGui.GetIO().MouseDownDuration[0] > 0.5f)
                DragDropMode = true;

            if (SelectedAction != null && P.CustomActions.Manager.IconTextures[SelectedAction.IconId].TryGetWrap(out var texture, out _))
            {
                var mousePos = ImGui.GetMousePos();
                mousePos.X -= 10;
                mousePos.Y -= 10;

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

            #endregion
        }

        private static void DrawAction(CustomAction act)
        {
            if (P.CustomActions.Manager.IconTextures[act.IconId].TryGetWrap(out var texture, out _))
            {
                var type = CustomActionHelper.GetCustomActionType(act.Id);
                bool changed = false;
                var alternativeChange = false;
                var preTitlePos = ImGui.GetCursorPos();
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
                ImGui.SameLine();
                ImGuiEx.HelpMarker(
                    $"Action ID is {act.Id}, " +
                    $"if you'd like to try to execute it in some other way, " +
                    $"like QoLBar or DelvCD or something.");

                ImGui.Indent(35f.Scale());
                ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey, act.Description);
                ImGui.Unindent(35f.Scale());
                var postDescPos = ImGui.GetCursorPos();

                ImGui.SetCursorPos(preTitlePos with
                {
                    X = preTitlePos.X +
                        longestDescriptionWidth +
                        70f.Scale()
                });
                var btnSize = ImGui.GetFrameHeight() * 2.5f.Scale();
                ImGui.ImageButton(texture.Handle, new Vector2(btnSize));

                if (ImGui.IsItemHovered())
                {
                    #region Icon Dragging

                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        Svc.GameConfig.TryGet(HotbarSetting, out HiddenSlots);
                        Svc.Log.Debug($"User has slots {(HiddenSlots == 0 ? "hidden" : "shown")}");
                        alternativeChange = true;
                    }
                    if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                    {
                        SelectedAction = act;
                        Svc.GameConfig.Set(HotbarSetting, 1);
                        alternativeChange = true;
                    }

                    #endregion

                    ImGui.BeginTooltip();
                    ImGui.Image(texture.Handle, new Vector2(50));
                    ImGui.SameLine();
                    var tooltipPos = ImGui.GetCursorPos();
                    ImGuiEx.Text(act.Name);
                    ImGui.SetCursorPosX(tooltipPos.X);
                    ImGui.SetCursorPosY(tooltipPos.Y + 20f.Scale());
                    ImGuiEx.Text(act.Description);
                    ImGui.EndTooltip();
                }

                ImGui.SetCursorPos(postDescPos);
                ImGuiEx.Spacing(new Vector2(0, 5));

                #region Alternative Change
                // Pretends we are clicking the checkbox on
                // if we click anything other than the checkbox

                if (alternativeChange)
                {
                    switch (type)
                    {
                        case CustomActionType.SingleTargetDPS:
                            changed |= !Service.Configuration
                                .CustomActionSettings.SingleTargetDPS;
                            Service.Configuration.CustomActionSettings
                                .SingleTargetDPS = true;
                            break;
                        case CustomActionType.AoEDPS:
                            changed |= !Service.Configuration
                                .CustomActionSettings.AoEDPS;
                            Service.Configuration.CustomActionSettings
                                .AoEDPS = true;
                            break;
                        case CustomActionType.SingleTargetHeals:
                            changed |= !Service.Configuration
                                .CustomActionSettings.SingleTargetHeals;
                            Service.Configuration.CustomActionSettings
                                .SingleTargetHeals = true;
                            break;
                        case CustomActionType.AoEHeals:
                            changed |= !Service.Configuration
                                .CustomActionSettings.AoEHeals;
                            Service.Configuration.CustomActionSettings
                                .AoEHeals = true;
                            break;
                    }
                }

                #endregion

                if (changed)
                    Service.Configuration.Save();
            }
            else
                ImGuiEx.Text(ImGuiColors.DalamudYellow,
                    "Sorry, we are failing to load our own Custom Action Icons!\n" +
                    "Please report this!");
        }
    }
}
