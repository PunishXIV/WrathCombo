using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System;
using System.Numerics;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.CustomComboNS.Functions;

internal static class WrathMath
{
    #region Angles
    public enum AttackAngle
    {
        Front,
        Flank,
        Rear,
        Unknown,
    }

    /// <summary> Performs positional calculations. Based on the excellent Resonant plugin. </summary>
    public static float GetRotation(Vector3 a, Vector3 b) => MathF.Atan2(b.X - a.X, b.Z - a.Z);
    public static Vector3 GetDirection(Vector3 a, Vector3 b) => ToDirection(GetRotation(a, b));

    public static float ToRotation(Vector3 direction) => MathF.Atan2(direction.X, direction.Z);
    public static Vector3 ToDirection(float rotation) => new(MathF.Sin(rotation), 0f, MathF.Cos(rotation));
    #endregion

    #region Shapes

    public interface IAoeShape { }
    public struct SelfCircle : IAoeShape { }
    public struct Circle : IAoeShape { }
    public struct Cone : IAoeShape { }
    public struct Line : IAoeShape { }

    public static bool PointInCircle(Vector3 offsetFromOrigin, float radius)
    {
        return offsetFromOrigin.LengthSquared() <= radius * radius;
    }

    public static bool PointInCone(Vector3 offsetFromOrigin, Vector3 direction, float halfAngle)
    {
        return Vector3.Dot(Vector3.Normalize(offsetFromOrigin), direction) > MathF.Cos(halfAngle);
    }

    public static bool HitboxInRect(IGameObject o, float rotation, float halfLength, float halfWidth)
    {
        if (LocalPlayer is not { } player) return false;

        Vector2 A = new(player.Position.X, player.Position.Z);
        Vector2 d = new(MathF.Sin(rotation), MathF.Cos(rotation));
        Vector2 n = new(d.Y, -d.X);
        Vector2 P = new(o.Position.X, o.Position.Z);
        float R = o.HitboxRadius;

        Vector2 Q = A + d * halfLength;
        Vector2 P2 = P - Q;
        Vector2 Ptrans = new(Vector2.Dot(P2, n), Vector2.Dot(P2, d));
        Vector2 Pabs = new(Math.Abs(Ptrans.X), Math.Abs(Ptrans.Y));
        Vector2 Pcorner = new(Math.Abs(Ptrans.X) - halfWidth, Math.Abs(Ptrans.Y) - halfLength);

        #if DEBUG
        if (Svc.GameGui.WorldToScreen(o.Position, out var screenCoords))
        {
            var objectText = $"A = {A}\n" +
                            $"d = {d}\n" +
                            $"n = {n}\n" +
                            $"P = {P}\n" +
                            $"Q = {Q}\n" +
                            $"P2 = {P2}\n" +
                            $"Ptrans = {Ptrans}\n" +
                            $"Pcorner{Pcorner}\n" +
                            $"R = {R}, R * R = {R * R}\n" +
                            $"PcornerSquared = {Pcorner.LengthSquared()}\n" +
                            $"PcornerX > R = {Pcorner.X > R}, PcornerY > R = {Pcorner.Y > R}\n" +
                            $"PcornerX <= 0 = {Pcorner.X <= 0}, PcornerY <= 0 = {Pcorner.Y <= 0}";

            var screenPos = ImGui.GetMainViewport().Pos;

            ImGui.SetNextWindowPos(new Vector2(screenCoords.X, screenCoords.Y));

            ImGui.SetNextWindowBgAlpha(1f);
            if (ImGui.Begin(
                    $"Actor###ActorWindow{o.GameObjectId}",
                    ImGuiWindowFlags.NoDecoration |
                    ImGuiWindowFlags.AlwaysAutoResize |
                    ImGuiWindowFlags.NoSavedSettings |
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoMouseInputs |
                    ImGuiWindowFlags.NoDocking |
                    ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.NoNav))
                ImGui.Text(objectText);
            ImGui.End();
        }
        #endif

        if (Pcorner.X > R || Pcorner.Y > R)
            return false;

        if (Pcorner.X <= 0 || Pcorner.Y <= 0)
            return true;

        return Pcorner.LengthSquared() <= R * R;
    }
    #endregion
}