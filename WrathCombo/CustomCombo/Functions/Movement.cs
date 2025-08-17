using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    private static DateTime? movementStarted;
    private static DateTime? movementStopped;

    /// <summary> Checks if the player is moving. </summary>
    public static unsafe bool IsMoving()
    {
        var agentMap = AgentMap.Instance();
        if (agentMap is null)
            return false;

        bool isMoving = agentMap->IsPlayerMoving || Player.IsJumping;

        if (isMoving)
        {
            if (movementStarted is null)
                movementStarted = DateTime.Now;

            movementStopped = null;
        }
        else
        {
            if (movementStopped is null)
                movementStopped = DateTime.Now;

            movementStarted = null;
        }

        return isMoving && TimeMoving.TotalSeconds >= Service.Configuration.MovementLeeway;
    }

    public unsafe static bool IsDashing() => MovementHook.Instance != null && MovementHook.Instance->Dashing == 1;

    public static TimeSpan TimeMoving => movementStarted is null ? TimeSpan.Zero : (DateTime.Now - movementStarted.Value);

    public static TimeSpan TimeStoodStill => movementStopped is null ? TimeSpan.Zero : (DateTime.Now - movementStopped.Value);
}

internal unsafe class MovementHook : IDisposable
{
    public static MoveControllerSubMemberForMine* Instance = null!;

    private delegate void RMIWalkDelegate(MoveControllerSubMemberForMine* self, float* sumLeft, float* sumForward, float* sumTurnLeft, byte* haveBackwardOrStrafe, byte* a6, byte bAdditiveUnk);
    [Signature("E8 ?? ?? ?? ?? 80 7B 3E 00 48 8D 3D", DetourName = nameof(RMIWalkDetour))]
    private readonly Hook<RMIWalkDelegate> _rmiWalkHook = null!;

    private void RMIWalkDetour(MoveControllerSubMemberForMine* self, float* sumLeft, float* sumForward, float* sumTurnLeft, byte* haveBackwardOrStrafe, byte* a6, byte bAdditiveUnk)
    {
        _rmiWalkHook.Original(self, sumLeft, sumForward, sumTurnLeft, haveBackwardOrStrafe, a6, bAdditiveUnk);

        Instance = self;
    }

    public void Dispose()
    {
        _rmiWalkHook?.Dispose();
    }

    internal MovementHook()
    {
        Svc.Hook.InitializeFromAttributes(this);
        _rmiWalkHook.Enable();
    }
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct MoveControllerSubMemberForMine
{
    [FieldOffset(0x10)] public Vector3 Direction;
    [FieldOffset(0x28)] public float Unk_0x28;
    [FieldOffset(0x38)] public float Unk_0x38;
    [FieldOffset(0x3C)] public byte Moved; // 1 when the character has moved
    [FieldOffset(0x3D)] public byte Rotated; // 1 when the character has rotated
    [FieldOffset(0x3E)] public byte MovementLock;
    [FieldOffset(0x3F)] public byte Unk_0x3F; // non-zero when moving with LMB+RMB
    [FieldOffset(0x40)] public byte Unk_0x40;
    [FieldOffset(0x44)] public float MoveSpeed;
    [FieldOffset(0x50)] public float* MoveSpeedMaximums;
    [FieldOffset(0x80)] public Vector3 ZoningPosition;
    [FieldOffset(0x90)] public float MoveDir;
    [FieldOffset(0x94)] public byte Unk_0x94;
    [FieldOffset(0xA0)] public Vector3 MoveForward; // direction output by MovementUpdate
    [FieldOffset(0xB0)] public float Unk_0xB0;
    [FieldOffset(0xB4)] public byte Unk_0xB4;
    [FieldOffset(0xF2)] public byte Dashing;
    [FieldOffset(0xF3)] public byte Unk_0xF3;
    [FieldOffset(0xF4)] public byte Unk_0xF4;
    [FieldOffset(0xF5)] public byte Unk_0xF5;
    [FieldOffset(0xF6)] public byte Unk_0xF6;
    [FieldOffset(0x104)] public byte Unk_0x104;
    [FieldOffset(0x110)] public Int32 WishdirChanged;
    [FieldOffset(0x114)] public float Wishdir_Horizontal;
    [FieldOffset(0x118)] public float Wishdir_Vertical;
    [FieldOffset(0x120)] public byte Unk_0x120;
    [FieldOffset(0x121)] public byte Rotated1;
    [FieldOffset(0x122)] public byte Unk_0x122;
    [FieldOffset(0x123)] public byte Unk_0x123;
    [FieldOffset(0x125)] public byte Unk_0x125;
    [FieldOffset(0x12A)] public byte Unk_0x12A;
}