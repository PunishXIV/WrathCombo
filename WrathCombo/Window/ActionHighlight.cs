using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.GUI;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureHotbarModule;

namespace WrathCombo.Window
{
    internal unsafe class ActionHighlight
    {
        private static uint[] HotbarActions = new uint[10 * 12];
        private static AtkUnitBase* HotBarRef { get; set; } = null;
        private static AtkResNode* HotBarSlotRef { get; set; } = null;

        internal class HighlightWindow : Dalamud.Interface.Windowing.Window
        {
            public HighlightWindow() : base("ActionHighlightWindow", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize, true)
            {
                this.Position = new System.Numerics.Vector2(0, 0);
                IsOpen = true;
                ShowCloseButton = false;
                RespectCloseHotkey = false;
                DisableWindowSounds = true;
                this.SizeConstraints = new WindowSizeConstraints()
                {
                    MaximumSize = new System.Numerics.Vector2(0, 0),
                };
            }
            public override void Draw()
            {
                try
                {
                    foreach (var action in Service.ActionReplacer.LastActionInvokeFor)
                    {
                        if (action.Key == PLD.FastBlade)
                        MakeButtonsGlow(action.Value);
                    }
                }
                catch { }
            }
        }

        public static void PopulateHotbarArray()
        {
            var raptureHotbarModule = Framework.Instance()->GetUIModule()->GetRaptureHotbarModule();
            int index = 0;

            foreach (ref var hotbar in raptureHotbarModule->Hotbars.Slice(0, 10))
            {
                foreach (ref var slot in hotbar.Slots.Slice(0, 12))
                {
                    HotbarActions[index++] = slot.CommandType is HotbarSlotType.Action ? slot.CommandId : 0;
                }
            }
        }

        public unsafe static void MakeButtonGlow(int index)
        {
            var hotbar = index / 12;
            var relativeLocation = index % 12;

            if (hotbar == 0)
            {
                HotBarRef = (AtkUnitBase*)Svc.GameGui.GetAddonByName($"_ActionBar", 1).Address;
                if (HotBarRef != null)
                {
                    HotBarSlotRef = HotBarRef->GetNodeById((uint)relativeLocation + 8);
                }

            }
            else
            {
                HotBarRef = (AtkUnitBase*)Svc.GameGui.GetAddonByName($"_ActionBar0{hotbar}", 1).Address;
                if (HotBarRef != null)
                {
                    HotBarSlotRef = HotBarRef->GetNodeById((uint)relativeLocation + 8);
                }
            }

            if (HotBarSlotRef != null && HotBarRef->IsVisible)
            {
                AtkResNodeFunctions.DrawOutline(HotBarSlotRef);
            }

        }

        internal unsafe static void MakeButtonsGlow(uint rec)
        {
            if (rec == 0) return;

            PopulateHotbarArray();
            for (int i = 0; i < HotbarActions.Length; ++i)
                if (CustomComboFunctions.OriginalHook(HotbarActions[i]) == rec)
                    MakeButtonGlow(i);
        }
    }
}
