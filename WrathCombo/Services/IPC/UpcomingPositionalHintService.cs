using Dalamud.Plugin.Ipc;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using System;
using WrathCombo.API;
using WrathCombo.API.Enum;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Services.IPC;

/// <summary>
///     Stores and publishes upcoming one-button positional hints for external plugins.
/// </summary>
internal static class UpcomingPositionalHintService
{
    private static PositionalHintSnapshot _current;
    private static long _lastReportTick;

    internal static ICallGateProvider<object> OnUpcomingPositionalHintProvider { get; } =
        Svc.PluginInterface.GetIpcProvider<object>("OnUpcomingPositionalHint");

    internal static void Reset()
    {
        if (_current.Direction is PositionalDirection.None)
            return;

        _current = default;
        NotifySubscribers();
    }

    internal static void Tick()
    {
        if (_current.Direction is PositionalDirection.None)
            return;

        if (!EzThrottler.Throttle("PositionalHintStaleCheck", 100))
            return;

        if (!CustomComboFunctions.HasBattleTarget() ||
            !CustomComboFunctions.TargetNeedsPositionals() ||
            IsExpired(_current))
        {
            Reset();
            return;
        }

        // Pull-only consumers refresh on GetWireSnapshot; skip live facing work with no subscribers.
        if (OnUpcomingPositionalHintProvider.SubscriptionCount == 0)
            return;

        RefreshLiveFields(notifyOnChange: true);
    }

    internal static uint[]? GetWireSnapshot()
    {
        if (_current.Direction is PositionalDirection.None || IsExpired(_current))
            return null;

        RefreshLiveFields(notifyOnChange: false);
        return RefreshExpiry(_current).ToWire();
    }

    internal static void Report(
        PositionalDirection direction,
        uint actionId,
        int gcdsUntil)
    {
        if (direction is PositionalDirection.None or PositionalDirection.Unknown ||
            actionId is 0 ||
            gcdsUntil is < 1 or > 3 ||
            !CustomComboFunctions.HasBattleTarget() ||
            !CustomComboFunctions.TargetNeedsPositionals())
        {
            return;
        }

        var target = CustomComboFunctions.CurrentTarget;
        if (target is null)
            return;

        var currentAngle = (byte)CustomComboFunctions.AngleToTarget(target);
        var requiredAngle = RequiredAngle(direction);

        var snapshot = new PositionalHintSnapshot
        {
            Direction = direction,
            ActionId = actionId,
            GcdsUntil = gcdsUntil,
            TargetObjectId = (uint)target.GameObjectId,
            ExpiresInMs = ComputeExpiresInMs(gcdsUntil),
            CurrentAngle = currentAngle,
            IsSatisfied = currentAngle == requiredAngle,
        };

        if (_current.IsActive &&
            !IsExpired(_current) &&
            !IsBetterHint(snapshot, _current))
        {
            // Same (or worse) urgency: still refresh TTL / facing so overlays stay live
            if (IsSameHint(snapshot, _current))
                ApplySnapshot(snapshot, notify: snapshot.CurrentAngle != _current.CurrentAngle ||
                                               snapshot.IsSatisfied != _current.IsSatisfied);
            return;
        }

        var notify = !IsSameHint(snapshot, _current) ||
                     snapshot.CurrentAngle != _current.CurrentAngle ||
                     snapshot.IsSatisfied != _current.IsSatisfied ||
                     IsExpired(_current);

        ApplySnapshot(snapshot, notify);
    }

    /// <summary>
    ///     Prefer sooner hints. Same urgency may replace when action/direction changes.
    /// </summary>
    private static bool IsBetterHint(PositionalHintSnapshot candidate, PositionalHintSnapshot existing)
    {
        if (candidate.GcdsUntil != existing.GcdsUntil)
            return candidate.GcdsUntil < existing.GcdsUntil;

        return candidate.ActionId != existing.ActionId ||
               candidate.Direction != existing.Direction;
    }

    private static bool IsSameHint(PositionalHintSnapshot a, PositionalHintSnapshot b) =>
        a.Direction == b.Direction &&
        a.ActionId == b.ActionId &&
        a.GcdsUntil == b.GcdsUntil &&
        a.TargetObjectId == b.TargetObjectId;

    private static void ApplySnapshot(PositionalHintSnapshot snapshot, bool notify)
    {
        _current = snapshot;
        _lastReportTick = Environment.TickCount64;
        if (notify)
            NotifySubscribers();
    }

    private static void RefreshLiveFields(bool notifyOnChange)
    {
        var target = CustomComboFunctions.CurrentTarget;
        if (target is null || (uint)target.GameObjectId != _current.TargetObjectId)
        {
            Reset();
            return;
        }

        var currentAngle = (byte)CustomComboFunctions.AngleToTarget(target);
        var isSatisfied = currentAngle == RequiredAngle(_current.Direction);
        if (currentAngle == _current.CurrentAngle && isSatisfied == _current.IsSatisfied)
            return;

        _current = _current with
        {
            CurrentAngle = currentAngle,
            IsSatisfied = isSatisfied,
        };

        if (notifyOnChange)
            NotifySubscribers();
    }

    private static byte RequiredAngle(PositionalDirection direction) =>
        direction switch
        {
            PositionalDirection.Rear => (byte)CustomComboFunctions.AttackAngle.Rear,
            PositionalDirection.Flank => (byte)CustomComboFunctions.AttackAngle.Flank,
            _ => (byte)CustomComboFunctions.AttackAngle.Unknown,
        };

    private static bool IsExpired(PositionalHintSnapshot snapshot)
    {
        if (snapshot.ExpiresInMs <= 0)
            return true;

        var elapsed = Environment.TickCount64 - _lastReportTick;
        return elapsed >= snapshot.ExpiresInMs;
    }

    private static PositionalHintSnapshot RefreshExpiry(PositionalHintSnapshot snapshot)
    {
        var elapsed = Environment.TickCount64 - _lastReportTick;
        var remaining = Math.Max(0, snapshot.ExpiresInMs - (int)elapsed);
        return snapshot with { ExpiresInMs = remaining };
    }

    private static int ComputeExpiresInMs(int gcdsUntil)
    {
        var gcdSeconds = Math.Max(CustomComboFunctions.GCDTotal, 2.0f);
        return (int)(gcdsUntil * gcdSeconds * 1000f) + 750;
    }

    private static void NotifySubscribers()
    {
        if (OnUpcomingPositionalHintProvider.SubscriptionCount > 0)
            OnUpcomingPositionalHintProvider.SendMessage();
    }
}
