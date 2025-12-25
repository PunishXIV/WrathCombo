using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace WrathCombo.Window.Functions
{
    internal class AtkResNodeFunctions
    {
        public unsafe static void DrawOutline(AtkResNode* node)
        {
            if (!node->IsVisible())
                return;

            var nodeAsBase = (AtkComponentNode*)node;
            if (nodeAsBase != null)
            {
                var dragDrop = nodeAsBase->Component->GetNodeById(3);
                if (dragDrop != null)
                {
                    var position = GetNodePosition(dragDrop);
                    var scale = GetNodeScale(dragDrop);
                    var size = new Vector2(dragDrop->Width, dragDrop->Height) * scale;

                    position += ImGuiHelpers.MainViewport.Pos;

                    ImGui.GetForegroundDrawList(ImGuiHelpers.MainViewport).AddRect(position, position + size, ColorHelpers.RgbaVector4ToUint(ImGuiColors.DalamudYellow), 0, ImDrawFlags.RoundCornersAll, 6);
                }
                else
                {
                    
                }
            }
        }

        public static unsafe Vector2 GetNodePosition(AtkResNode* node)
        {
            var pos = new Vector2(node->X, node->Y);
            var par = node->ParentNode;
            while (par != null)
            {
                pos *= new Vector2(par->ScaleX, par->ScaleY);
                pos += new Vector2(par->X, par->Y);
                par = par->ParentNode;
            }

            return pos;
        }

        public static unsafe Vector2 GetNodeScale(AtkResNode* node)
        {
            if (node == null) return new Vector2(1, 1);
            var scale = new Vector2(node->ScaleX, node->ScaleY);
            while (node->ParentNode != null)
            {
                node = node->ParentNode;
                scale *= new Vector2(node->ScaleX, node->ScaleY);
            }

            return scale;
        }
    }
}
