using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using WrathCombo.Native;

namespace WrathCombo.Window.Tabs
{
    internal class CustomActions : ConfigWindow
    {
        private static CustomAction? SelectedAction;
        private static bool DragDropMode;
        internal unsafe static new void Draw()
        {
            ImGuiEx.Text($"{P.CustomActions.HoveredSlot}");
            foreach (var act in P.CustomActions.Manager.Actions)
            {
                DrawAction(act);
                ImGui.SameLine();
            }

            if (ImGui.GetIO().MouseDownDuration[0] > 0.5f)
                DragDropMode = true;

            if (SelectedAction != null && ThreadLoadImageHandler.TryGetIconTextureWrap(SelectedAction.IconId, false, out var texture))
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

                ImGui.Image(texture.Handle, new(50));
                ImGui.End();

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && DragDropMode))
                {
                    if (P.CustomActions.HoveredSlot != null)
                    {
                        RaptureHotbarModule.Instance()->Hotbars[P.CustomActions.HoveredSlot.Value.Hotbar].Slots[P.CustomActions.HoveredSlot.Value.Slot].Set(RaptureHotbarModule.HotbarSlotType.Action, SelectedAction.Id);
                        
                    }
                    SelectedAction = null;
                    DragDropMode = false;
                }

            }
        }

        private static unsafe void DrawAction(CustomAction act)
        {
            if (ThreadLoadImageHandler.TryGetIconTextureWrap(act.IconId, false, out var texture))
            {
                ImGui.ImageButton(texture.Handle, new(50));

                if (ImGui.IsItemHovered() && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    SelectedAction = act;
                }
            }
        }
    }
}
