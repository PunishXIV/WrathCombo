using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using WrathCombo.Services;
namespace WrathCombo.Window;

internal class TargetHelper : Dalamud.Interface.Windowing.Window
{
    internal TargetHelper() : base("###WrathComboTargetHelper", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs, true)
    {
        IsOpen = true;
        RespectCloseHotkey = false;
    }

    private static IGameObject? SuggestedTarget =>
        Combos.PvE.AST.CardTarget ??
        Combos.PvE.DNC.DesiredDancePartner.GetObject();

    internal unsafe void DrawTargetHelper()
    {
        if (!Service.Configuration.ShowTargetHighlight ||
            SuggestedTarget is null)
        {
            IsOpen = false;
            return;
        }
        
        IntPtr partyPointer = Svc.GameGui.GetAddonByName("_PartyList", 1);
        if (partyPointer == IntPtr.Zero)
        {
            IsOpen = false;
            return;
        }

        var plist = Marshal.PtrToStructure<AddonPartyList>(partyPointer);
        if (!plist.IsVisible)
        {
            IsOpen = false;
            return;
        }

        for (var i = 1; i <= 8; i++)
        {
            var slot = SimpleTarget.GetPartyMemberInSlotSlot(i);
            if (slot is null) continue;
            if (slot.GameObjectId != SuggestedTarget.GameObjectId) continue;
            
            var member = i switch
            {
                1 => plist.PartyMembers[0].TargetGlow,
                2 => plist.PartyMembers[1].TargetGlow,
                3 => plist.PartyMembers[2].TargetGlow,
                4 => plist.PartyMembers[3].TargetGlow,
                5 => plist.PartyMembers[4].TargetGlow,
                6 => plist.PartyMembers[5].TargetGlow,
                7 => plist.PartyMembers[6].TargetGlow,
                8 => plist.PartyMembers[7].TargetGlow,
                _ => plist.PartyMembers[0].TargetGlow,
            };

            DrawOutline(member->AtkResNode.PrevSiblingNode);
            IsOpen = true;
        }
    }

    private unsafe void DrawOutline(AtkResNode* node)
    {
        var position = GetNodePosition(node);
        var scale = GetNodeScale(node);
        var size = new Vector2(node->Width, node->Height) * scale;

        position += ImGuiHelpers.MainViewport.Pos;

        var colour = Service.Configuration.TargetHighlightColor;
        ImGui.GetForegroundDrawList(ImGuiHelpers.MainViewport).AddRect(
            position, position + size, ImGui.GetColorU32(colour),
            0, ImDrawFlags.RoundCornersAll, 2);
    }
    
    internal static unsafe Vector2 GetNodePosition(AtkResNode* node)
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

    internal static unsafe Vector2 GetNodeScale(AtkResNode* node)
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

    public override void Draw() => DrawTargetHelper();
}
