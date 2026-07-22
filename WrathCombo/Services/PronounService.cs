#region

using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using WrathCombo.Extensions;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace WrathCombo.Services;

public static unsafe class PronounService
{
    public unsafe static IGameObject? GetByPlaceHolder(string placeholder)
    {
        var ptr = (nint)PronounModule.Instance()->ResolvePlaceholder(placeholder, 0, 0);
        return Svc.Objects.CreateObjectReference(ptr);
    }

    public unsafe static IGameObject? UIMouseOverTarget => Svc.Objects.CreateObjectReference((nint)PronounModule.Instance()->UiMouseOverTarget);
}
