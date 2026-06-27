//using FFXIVClientStructs.FFXIV.Component.GUI;
//using KamiToolKit;
//using KamiToolKit.Classes;
//using KamiToolKit.Nodes;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Numerics;

//namespace WrathCombo.Native;

//public sealed unsafe class CustomActionListAddon : NativeAddon
//{
//    private const float CellSize = 44f;
//    private const float Gap = 4f;
//    private const int Columns = 5;
//    private readonly CustomActionManager manager;
//    private readonly List<DragDropNode> nodes = new();

//    [SetsRequiredMembers]
//    public CustomActionListAddon(CustomActionManager customManager)
//    {
//        manager = customManager;
//        InternalName = "WrathActions";
//        Title = "Wrath Actions";
//        Size = new Vector2(Columns * (CellSize + Gap) + Gap + 16f, 300f);
//    }

//    protected override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues) => RebuildNodes();

//    private void RebuildNodes()
//    {
//        int rows = Math.Max(1, (int)Math.Ceiling(manager.Actions.Count / (float)Columns));
//        SetWindowSize(new Vector2(Size.X, ContentStartPosition.Y + 12f + rows * (CellSize + Gap) + Gap + 16f));

//        Vector2 origin = ContentStartPosition + new Vector2(4f, 14f);

//        var i = 0;
//        foreach (CustomAction action in manager.Actions)
//        {
//            int col = i % Columns;
//            int row = i / Columns;

//            var node = new DragDropNode
//            {
//                Position = origin + new Vector2(col * (CellSize + Gap), row * (CellSize + Gap)),
//                Size = new Vector2(CellSize, CellSize),
//                IconId = action.IconId,
//                IsDraggable = true,
//                IsClickable = true,
//                TextTooltip = action.Name,
//                Payload = new DragDropPayload { Type = (DragDropType)7, Int2 = (int)action.Id }
//            };

//            CustomAction captured = action;
//            node.OnClicked = _ => captured.OnClick();

//            AddNode(node);
//            nodes.Add(node);

//            i++;
//        }
//    }

//    protected override void OnFinalize(AtkUnitBase* addon)
//    {
//        manager.ClearPendingInjects();
//        DisposeNodes();
//    }

//    private void DisposeNodes()
//    {
//        foreach (DragDropNode n in nodes) n.Dispose();
//        nodes.Clear();
//    }
//}
