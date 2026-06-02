using Dalamud.Hooking;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WrathCombo.Combos.PvE;
using WrathCombo.Extensions;
using WrathCombo.Services;

namespace WrathCombo.Native;

public sealed unsafe class CustomAction : IDisposable
{
    public CustomAction(uint id,
                        string name,
                        string description,
                        uint iconId,
                        Action? onClick = null,
                        string? customIconPath = null,
                        ushort cast100ms = 0,
                        ushort recast100ms = 250,
                        byte cooldownGroup = 58,
                        byte maxCharges = 1,
                        sbyte range = -1,
                        byte castType = 1)
    {
        Id = id;
        Name = name;
        IconId = iconId;
        CustomIconPath = customIconPath;
        OnClick = onClick;
        Description = description;

        byte[] nameUtf8 = Encoding.UTF8.GetBytes(name);
        UIntPtr rowSize = (nuint)sizeof(CustomActionManager.CustomActionRow);
        ActionRowPtr = (nint)NativeMemory.AllocZeroed(rowSize + (nuint)nameUtf8.Length + 1);
        CustomActionManager.CustomActionRow* row = (CustomActionManager.CustomActionRow*)ActionRowPtr;
        row->NameOffset = (uint)rowSize;
        row->Icon = (ushort)iconId;
        row->ActionCategory = 4;
        row->PrimaryCostType = 0;
        row->PrimaryCostValue = 0;
        row->Cast100ms = cast100ms;
        row->Recast100ms = recast100ms;
        row->CooldownGroup = cooldownGroup;
        row->MaxCharges = maxCharges;
        row->ClassJobCategory = 1;
        row->ClassJob = -1;
        row->Range = range;
        row->CastType = castType;
        nameUtf8.CopyTo(new Span<byte>((void*)(ActionRowPtr + (nint)rowSize), nameUtf8.Length));

        byte[] descBytes = Encoding.UTF8.GetBytes(description);
        UIntPtr transientSize = (nuint)(4 + descBytes.Length + 1);
        TransientRowPtr = (nint)NativeMemory.AllocZeroed(transientSize);
        *(uint*)TransientRowPtr = 4;
        descBytes.CopyTo(new Span<byte>((void*)(TransientRowPtr + 4), descBytes.Length));

        NamePtr = ActionRowPtr + (nint)rowSize;
    }

    public uint Id { get; }
    public string Name { get; }
    public uint IconId { get; }
    public string? CustomIconPath { get; }
    public Action? OnClick { get; }
    public string Description { get; }

    internal nint ActionRowPtr { get; }
    internal nint TransientRowPtr { get; }
    internal nint NamePtr { get; }

    public void Dispose()
    {
        NativeMemory.Free((void*)ActionRowPtr);
        NativeMemory.Free((void*)TransientRowPtr);
    }
}

public sealed unsafe class CustomActionManager : IDisposable
{
    private const string SigGetActionRow = "48 83 EC 28 48 8B 05 ?? ?? ?? ?? 44 8B C1 BA 04 00 00 00";
    private const string SigIsSlotUsable = "48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 0F B6 F2 48 8B D9 41 8B F8 8D 46 FF 83 F8 22";

    private readonly Dictionary<uint, CustomAction> _actions = new();
    private readonly IFramework _framework;

    private readonly Hook<GetActionRowDelegate> _getActionRowHook;
    private readonly Dictionary<uint, ISharedImmediateTexture> _iconTextures = new();
    private readonly Hook<IsSlotUsableDelegate> _isSlotUsableHook;
    private readonly Hook<LoadIconDelegate> _loadIconHook;
    private readonly List<IconInjectEntry> _pendingInjects = new();

    private readonly ITextureProvider _texProv;

    public CustomActionManager(ISigScanner sig,
                               IGameInteropProvider hooks,
                               ITextureProvider texProv,
                               IFramework framework)
    {
        _texProv = texProv;
        _framework = framework;

        _getActionRowHook = hooks.HookFromAddress<GetActionRowDelegate>(sig.ScanText(SigGetActionRow), GetActionRowDetour);
        _isSlotUsableHook = hooks.HookFromAddress<IsSlotUsableDelegate>(sig.ScanText(SigIsSlotUsable), IsSlotUsableDetour);
        _loadIconHook = hooks.HookFromAddress<LoadIconDelegate>(AtkComponentIcon.Addresses.LoadIcon.Value, LoadIconDetour);

        _getActionRowHook.Enable();
        _isSlotUsableHook.Enable();
        _loadIconHook.Enable();

        framework.Update += OnFrameworkUpdate;
    }

    public IReadOnlyCollection<CustomAction> Actions => _actions.Values;
    public IReadOnlyDictionary<uint, ISharedImmediateTexture> IconTextures => _iconTextures;

    public void Dispose()
    {
        _framework.Update -= OnFrameworkUpdate;

        _getActionRowHook.Dispose();
        _isSlotUsableHook.Dispose();
        _loadIconHook.Dispose();

        foreach (CustomAction action in _actions.Values)
        {
            action.Dispose();
        }

        _actions.Clear();
        _iconTextures.Clear();
        _pendingInjects.Clear();
    }

    public void Register(CustomAction action)
    {
        _actions[action.Id] = action;
        if (action.CustomIconPath != null)
        {
            Svc.Log.Debug($"Registering {action.Id} from path {action.CustomIconPath}");
            _iconTextures[action.IconId] = _texProv.GetFromFileAbsolute(action.CustomIconPath);
        }
        else
        {
            if (_texProv.TryGetFromGameIcon(new GameIconLookup() { IconId = action.IconId, ItemHq = false }, out var tex))
                _iconTextures[action.IconId] = tex;
        }
    }

    public void Register(params CustomAction[] actions)
    {
        foreach (CustomAction action in actions)
        {
            if (action == null)
                continue;

            Register(action);
        }
    }

    public void ClearPendingInjects() => _pendingInjects.Clear();

    private CustomActionRow* GetActionRowDetour(uint rowId)
    {
        if (_actions.TryGetValue(rowId, out CustomAction? action))
        {
            return (CustomActionRow*)action.ActionRowPtr;
        }

        return _getActionRowHook.Original(rowId);
    }

    private bool IsSlotUsableDetour(RaptureHotbarModule.HotbarSlot* self,
                                    RaptureHotbarModule.HotbarSlotType type,
                                    uint actionId)
    {
        if (type == RaptureHotbarModule.HotbarSlotType.Action &&
            _actions.ContainsKey(actionId))
        {
            return true;
        }

        return _isSlotUsableHook.Original(self, type, actionId);
    }

    private bool LoadIconDetour(AtkComponentIcon* self, uint iconId)
    {
        Svc.Log.Debug($"LoadIcon called with iconId {iconId}");
        bool result = _loadIconHook.Original(self, iconId);
        if (_iconTextures.ContainsKey(iconId) && self->Texture != null)
        {
            _pendingInjects.RemoveAll(e => e.ComponentPtr == (nint)self);
            _pendingInjects.Add(new IconInjectEntry((nint)self, _iconTextures[iconId], 20));
        }

        return result;
    }

    private void OnFrameworkUpdate(IFramework fw)
    {
        for (int i = _pendingInjects.Count - 1; i >= 0; i--)
        {
            IconInjectEntry e = _pendingInjects[i];
            AtkComponentIcon* icon = (AtkComponentIcon*)e.ComponentPtr;

            int framesLeft = e.FramesLeft - 1;
            if (((uint)icon->Flags & 0x400u) != 0 && framesLeft > 0)
            {
                _pendingInjects[i] = e with { FramesLeft = framesLeft };
                continue;
            }

            IDalamudTextureWrap? wrap = e.Tex.GetWrapOrDefault();
            if (wrap == null)
            {
                _pendingInjects[i] = e with { FramesLeft = framesLeft };
                continue;
            }

            AtkImageNode* imgNode = icon->IconImage;
            if (imgNode == null)
            {
                _pendingInjects.RemoveAt(i);
                continue;
            }

            AtkUldPartsList* partsList = imgNode->PartsList;
            if (partsList == null || partsList->PartCount == 0)
            {
                _pendingInjects.RemoveAt(i);
                continue;
            }

            AtkUldPart* part = partsList->Parts;
            if (part == null)
            {
                _pendingInjects.RemoveAt(i);
                continue;
            }

            (*part).LoadTexture(wrap);

            _loadIconHook.Original(icon, icon->IconId);
            _pendingInjects.RemoveAt(i);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    internal struct CustomActionRow
    {
        [FieldOffset(0x00)] public uint NameOffset;
        [FieldOffset(0x08)] public ushort Icon;
        [FieldOffset(0x0E)] public ushort PrimaryCostValue;
        [FieldOffset(0x14)] public ushort Cast100ms;
        [FieldOffset(0x16)] public ushort Recast100ms;
        [FieldOffset(0x22)] public byte ActionCategory;
        [FieldOffset(0x28)] public byte CastType;
        [FieldOffset(0x29)] public byte EffectRange;
        [FieldOffset(0x2B)] public byte PrimaryCostType;
        [FieldOffset(0x2E)] public byte CooldownGroup;
        [FieldOffset(0x30)] public byte MaxCharges;
        [FieldOffset(0x33)] public byte ClassJobCategory;
        [FieldOffset(0x37)] public sbyte ClassJob;
        [FieldOffset(0x38)] public sbyte Range;
    }

    private delegate CustomActionRow* GetActionRowDelegate(uint rowId);

    private delegate bool IsSlotUsableDelegate(RaptureHotbarModule.HotbarSlot* self,
                                               RaptureHotbarModule.HotbarSlotType type,
                                               uint actionId);

    private delegate bool UseActionDelegate(ActionManager* self,
                                            ActionType actionType,
                                            uint actionId,
                                            ulong targetId,
                                            uint extraParam,
                                            ActionManager.UseActionMode mode,
                                            uint comboRouteId,
                                            bool* outOptAreaTargeted);

    private delegate bool LoadIconDelegate(AtkComponentIcon* self, uint iconId);

    private record struct IconInjectEntry(nint ComponentPtr, ISharedImmediateTexture Tex, int FramesLeft);
}

public sealed unsafe class CustomActionSetup : IDisposable
{
    public readonly CustomActionManager Manager;
    private readonly CustomAction _singleTargetDPS;
    private readonly CustomAction _aoeDPS;
    private readonly CustomAction _singleTargeHeals;
    private readonly CustomAction _aoeHeals;
    public (int Hotbar, int Slot)? HoveredSlot = null;

    [EzHook("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 20 48 8B 7C 24 ?? 48 8B D9", false)]
    private EzHook<AddonActionBarBase.Delegates.ReceiveEvent>? AddonActionBarBase_ReceiveEventHook;

    private unsafe void AddonActionBarBase_ReceiveEventDetour(AddonActionBarBase* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData)
    {
        try
        {
            if (eventType == AtkEventType.DragDropRollOver)
            {
                HoveredSlot = ((int Hotbar, int Slot)?)(thisPtr->RaptureHotbarId, eventParam);
            }
            if (eventType == AtkEventType.DragDropRollOut)
            {
                HoveredSlot = null;
            }
        }
        catch (Exception e)
        {
            e.Log();
        }
        AddonActionBarBase_ReceiveEventHook.Original(thisPtr, eventType, eventParam, atkEvent, atkEventData);
    }

    public CustomActionSetup()
    {
        EzSignatureHelper.Initialize(this);
        AddonActionBarBase_ReceiveEventHook?.Enable();
        Manager = new(Svc.SigScanner, Svc.Hook, Svc.Texture, Svc.Framework);
        _singleTargetDPS = new(All.SingleTargetDPS, "Single Target DPS", "This is for the Single Target DPS combos.", 1504);
        _aoeDPS = new(All.AoEDPS, "AoE DPS", "This is for the AoE DPS combos.", 1505);
        _singleTargeHeals = new(All.SingleTargetHeals, "Single Target Heals", "This is for the Single Target Heal combos.", 1508);
        _aoeHeals = new(All.AoeHeals, "AoE Heals", "This is for the AoE Heal combos.", 1510);

        Manager.Register(_singleTargetDPS, _aoeDPS, _singleTargeHeals, _aoeHeals);
    }
    public void Dispose()
    {
        Manager.Dispose();
        AddonActionBarBase_ReceiveEventHook?.Disable();
    }

}

public class CustomActionSettings()
{
    public bool SingleTargetDPS = false;
    public bool AoEDPS = false;
    public bool SingleTargetHeals = false;
    public bool AoEHeals = false;
}

public enum CustomActionType
{
    SingleTargetDPS = 1,
    AoEDPS = 2,
    SingleTargetHeals = 3,
    AoEHeals = 4
}

public class CustomActionHelper()
{
    /// <summary>
    /// Gets the custom action type for the given action ID, or null if the action ID is not a custom action.
    /// </summary>
    /// <param name="actionId"></param>
    /// <returns></returns>
    public static CustomActionType? GetCustomActionType(uint actionId)
    {
        return actionId switch
        {
            All.SingleTargetDPS => CustomActionType.SingleTargetDPS,
            All.AoEDPS => CustomActionType.AoEDPS,
            All.SingleTargetHeals => CustomActionType.SingleTargetHeals,
            All.AoeHeals => CustomActionType.AoEHeals,
            _ => null
        };
    }

    /// <summary>
    /// Gets the custom action ID for the given custom action type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static uint GetActionId(CustomActionType type)
    {
        return type switch
        {
            CustomActionType.SingleTargetDPS => All.SingleTargetDPS,
            CustomActionType.AoEDPS => All.AoEDPS,
            CustomActionType.SingleTargetHeals => All.SingleTargetHeals,
            CustomActionType.AoEHeals => All.AoEDPS,
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Used for all auto-rotation compatible presets, aka the full one button rotations. 
    /// </summary>
    /// <param name="actionId"></param>
    /// <param name="type"></param>
    /// <param name="originals"></param>
    /// <returns></returns>
    public static bool OneButtonRotationChecker(uint actionId, CustomActionType type, params uint[] originals)
    {
        bool enabled = type switch
        {
            CustomActionType.SingleTargetDPS => Service.Configuration.CustomActionSettings.SingleTargetDPS,
            CustomActionType.AoEDPS => Service.Configuration.CustomActionSettings.AoEDPS,
            CustomActionType.SingleTargetHeals => Service.Configuration.CustomActionSettings.SingleTargetHeals,
            CustomActionType.AoEHeals => Service.Configuration.CustomActionSettings.AoEHeals,
            _ => false
        };

        if (enabled)
            return actionId == GetActionId(type);

        return originals.Contains(actionId);
    }
}