using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WrathCombo.Data;
using WrathCombo.Services;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary> Gets the current target or null. </summary>
    public static IGameObject? CurrentTarget => Svc.Targets.Target;

    #region Target Checks

    /// <summary> Find if the player has a target. </summary>
    public static bool HasTarget() => CurrentTarget is not null;

    /// <summary> Checks if the player is being targeted by a hostile, targetable object. </summary>
    public static bool IsPlayerTargeted() => Svc.Objects.Any(x => x.IsTargetable && x.IsHostile() && x.TargetObjectId == LocalPlayer?.GameObjectId);

    /// <summary> Checks if an object is dead. Defaults to CurrentTarget unless specified. </summary>
    internal static bool TargetIsDead(IGameObject? optionalTarget = null) => (optionalTarget ?? CurrentTarget) is IBattleChara chara && chara.IsDead;

    /// <summary> Checks if an object is a boss. Defaults to CurrentTarget unless specified. </summary>
    internal static bool TargetIsBoss(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not IBattleChara chara)
            return false;

        return Svc.Data.GetExcelSheet<BNpcBase>().TryGetRow(chara.DataId, out var dataRow) && dataRow.Rank is 2 or 6;
    }

    [Obsolete("Use TargetIsBoss")]
    internal static bool IsBoss(IGameObject? target) => TargetIsBoss(target);

    /// <summary> Checks if an object is quest-related. Defaults to CurrentTarget unless specified. </summary>
    internal static unsafe bool IsQuestMob(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not { } chara)
            return false;

        return chara.Struct()->NamePlateIconId is 71204 or 71144 or 71224 or 71344;
    }

    [Obsolete("Use HasBattleTarget")]
    internal static bool TargetIsHostile() => HasBattleTarget();

    /// <summary> Checks if an object is friendly. Defaults to CurrentTarget unless specified. </summary>
    public static bool TargetIsFriendly(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not { } chara)
            return false;

        return chara.ObjectKind switch
        {
            ObjectKind.Player => true,
            _ when chara is IBattleNpc npc => npc.BattleNpcKind is not BattleNpcSubKind.Enemy and not (BattleNpcSubKind)1,
            _ => false
        };
    }

    /// <summary> Checks if the player's current target is hostile. </summary>
    public static bool HasBattleTarget() => HasTarget() && CurrentTarget.IsHostile();

    /// <summary> Checks if an object requires positionals. Defaults to CurrentTarget unless specified. </summary>
    public static bool TargetNeedsPositionals(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not IBattleChara chara || HasStatusEffect(3808, chara, true))
            return false;

        return Svc.Data.GetExcelSheet<BNpcBase>().TryGetRow(chara.DataId, out var dataRow) && !dataRow.IsOmnidirectional;
    }

    /// <summary>
    ///     Checks if the player's current target is casting an action.<br/>
    ///     Optionally, limit by percentage of cast time.
    /// </summary>
    /// <param name="minCastPercent">
    ///     The minimum percentage of the cast time completed required.<br/>
    ///     Default is 0%.<br/>
    ///     As a float representation of a percentage, value should be between
    ///     0.0f (0%) and 1.0f (100%).
    /// </param>
    /// <returns>
    ///     Bool indicating whether they are casting an action or not.<br/>
    ///     (and if the cast time is over the percentage specified)
    /// </returns>
    public static bool TargetIsCasting(float minCastPercent = 0f)
    {
        if (CurrentTarget is not IBattleChara chara || !chara.IsCasting)
            return false;

        float minThreshold = Math.Clamp(minCastPercent, 0f, 1f);

        return chara.CurrentCastTime >= chara.TotalCastTime * minThreshold;
    }

    /// <summary>
    ///     Checks if an enemy is casting an interruptible action.<br/>
    ///     Optionally, limit by percentage of cast time.<br/>
    ///     Defaults to CurrentTarget unless specified.
    /// </summary>
    /// <param name="minCastPercent">
    ///     The minimum percentage of the cast time completed required.<br/>
    ///     Default is 0%.<br/>
    ///     As a float representation of a percentage, value should be between
    ///     0.0f (0%) and 1.0f (100%).
    /// </param>
    /// <returns>
    ///     Bool indicating whether they can be interrupted or not.<br/>
    ///     (and if the cast time is over the percentage specified)
    /// </returns>
    public static bool CanInterruptEnemy(float? minCastPercent = null, IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not IBattleChara chara || !chara.IsCasting || !chara.IsCastInterruptible)
            return false;

        float minThreshold = Math.Clamp(minCastPercent ?? (float)Service.Configuration.InterruptDelay, 0f, 1f);

        return chara.CurrentCastTime >= chara.TotalCastTime * minThreshold;
    }

    /// <summary> Gets all bosses from the object table. </summary>
    internal static IEnumerable<IBattleChara> NearbyBosses => Svc.Objects.OfType<IBattleChara>().Where(x => x.ObjectKind == ObjectKind.BattleNpc && TargetIsBoss(x));

    #endregion

    #region HP Checks

    /// <summary> Gets the player's current HP as a percentage. </summary>
    public static float PlayerHealthPercentageHp() => LocalPlayer is { } player ? player.CurrentHp * 100f / player.MaxHp : 0f;

    /// <summary> Gets an object's current HP as a percentage. Defaults to CurrentTarget unless specified. </summary>
    public static float GetTargetHPPercent(IGameObject? optionalTarget = null, bool includeShield = false)
    {
        if ((optionalTarget ?? CurrentTarget) is not IBattleChara chara)
            return 0f;

        float charaHPPercent = chara.CurrentHp * 100f / chara.MaxHp;

        return includeShield
            ? Math.Clamp(charaHPPercent + chara.ShieldPercentage, 0f, 100f)
            : charaHPPercent;
    }

    [Obsolete("Use GetTargetCurrentHP")]
    public static float EnemyHealthCurrentHp() => GetTargetCurrentHP();

    /// <summary> Gets an object's maximum HP. Defaults to CurrentTarget unless specified. </summary>
    public static uint GetTargetMaxHP(IGameObject? optionalTarget = null) => (optionalTarget ?? CurrentTarget) is IBattleChara chara ? chara.MaxHp : 0;

    /// <summary> Gets an object's current HP. Defaults to CurrentTarget unless specified. </summary>
    public static uint GetTargetCurrentHP(IGameObject? optionalTarget = null) => (optionalTarget ?? CurrentTarget) is IBattleChara chara ? chara.CurrentHp : 0;

    #endregion

    #region Distance Checks

    /// <summary> Checks if an object is within melee range. Defaults to CurrentTarget unless specified. </summary>
    public static bool InMeleeRange(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not { } chara)
            return false;

        return GetTargetDistance(chara) <= (InPvP() ? 5f : 3f) + (float)Service.Configuration.MeleeOffset;
    }

    /// <summary> Checks if an object is within a given range. Defaults to CurrentTarget unless specified. </summary>
    public static bool IsInRange(IGameObject? optionalTarget = null, float range = 25f)
    {
        if ((optionalTarget ?? CurrentTarget) is not { } chara)
            return false;

        return GetTargetDistance(chara) <= range;
    }

    /// <summary>
    ///     Gets the horizontal distance between two objects. <br/>
    ///     Defaults to LocalPlayer and CurrentTarget unless specified.
    /// </summary>
    public static float GetTargetDistance(IGameObject? optionalTarget = null, IGameObject? optionalSource = null)
    {
        if ((optionalSource ?? LocalPlayer) is not { } sourceChara)
            return 0f;

        if ((optionalTarget ?? CurrentTarget) is not { } targetChara)
            return 0f;

        if (targetChara.GameObjectId == sourceChara.GameObjectId)
            return 0f;

        Vector2 targetPosition = new(targetChara.Position.X, targetChara.Position.Z);
        Vector2 sourcePosition = new(sourceChara.Position.X, sourceChara.Position.Z);

        return Math.Max(0f, Vector2.Distance(targetPosition, sourcePosition) - targetChara.HitboxRadius - sourceChara.HitboxRadius);
    }

    /// <summary>
    ///     Gets the vertical distance between two objects. <br/>
    ///     Defaults to LocalPlayer and CurrentTarget unless specified.
    /// </summary>
    public static float GetTargetHeightDifference(IGameObject? optionalTarget = null, IGameObject? optionalSource = null)
    {
        if ((optionalSource ?? LocalPlayer) is not { } sourceChara)
            return 0f;

        if ((optionalTarget ?? CurrentTarget) is not { } targetChara)
            return 0f;

        if (targetChara.GameObjectId == sourceChara.GameObjectId)
            return 0f;

        return Math.Abs(targetChara.Position.Y - sourceChara.Position.Y);
    }

    /// <summary>
    ///     Gets the number of enemies within range of an AoE action. <br/>
    ///     If the action requires a target, defaults to CurrentTarget unless specified.
    /// </summary>
    public static int NumberOfEnemiesInRange(uint aoeSpell, IGameObject? target, bool checkIgnoredList = false)
    {
        if (!ActionWatching.ActionSheet.TryGetValue(aoeSpell, out var sheetSpell))
            return 0;

        if (sheetSpell.CanTargetHostile && ((target ??= CurrentTarget) is null || GetTargetDistance(target) > ActionWatching.GetActionRange(sheetSpell.RowId)))
            return 0;

        int count = sheetSpell.CastType switch
        {
            1 => 1,
            2 => sheetSpell.CanTargetSelf
                ? CanCircleAoe(sheetSpell.EffectRange, checkIgnoredList)
                : CanRangedCircleAoe(target, sheetSpell.EffectRange, checkIgnoredList),
            3 => CanConeAoe(target, sheetSpell.Range, checkIgnoredList),
            4 => CanLineAoe(target, sheetSpell.Range, sheetSpell.XAxisModifier, checkIgnoredList),
            _ => 0
        };

        return count;
    }

    /// <summary> Gets the number of enemies within a given range from the player. </summary>
    public static int NumberOfEnemiesInRange(float range)
    {
        return Svc.Objects.Count(o =>
            o.ObjectKind == ObjectKind.BattleNpc &&
            o.IsTargetable &&
            o.IsHostile() &&
            GetTargetDistance(o) <= range);
    }

    /// <summary> Checks if an object is within line of sight of the player. </summary>
    internal static unsafe bool IsInLineOfSight(IGameObject? obj)
    {
        if (LocalPlayer is not { } player || obj is null) return false;

        Vector3 sourcePos = player.Position with { Y = player.Position.Y + 2f };
        Vector3 targetPos = obj.Position with { Y = obj.Position.Y + 2f };
        Vector3 offset = targetPos - sourcePos;

        float distance = offset.Length();
        Vector3 direction = distance > float.Epsilon
            ? offset / distance
            : Vector3.Zero;

        RaycastHit hit;
        var flags = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

        return !Framework.Instance()->BGCollisionModule->RaycastMaterialFilter(&hit, &sourcePos, &direction, distance, 1, flags);
    }

    #endregion

    #region Positional Checks

    /// <summary> Checks if the player is on target's rear. </summary>
    public static bool OnTargetsRear() => AngleToTarget() is AttackAngle.Rear;

    /// <summary> Checks if the player is on target's flank. </summary>
    public static bool OnTargetsFlank() => AngleToTarget() is AttackAngle.Flank;

    /// <summary> Checks if the player is on target's front. </summary>
    public static bool OnTargetsFront() => AngleToTarget() is AttackAngle.Front;

    #region Positional Helpers

    public enum AttackAngle
    {
        Front,
        Flank,
        Rear,
        Unknown,
    }

    /// <summary> Gets the player's position relative to the target. </summary>
    /// <returns> Front, Flank, Rear or Unknown as AttackAngle type. </returns>
    public static AttackAngle AngleToTarget()
    {
        if (LocalPlayer is not { } player || CurrentTarget is not IBattleChara target || target.ObjectKind != ObjectKind.BattleNpc)
            return AttackAngle.Unknown;

        float rotation = PositionalMath.GetRotation(target.Position, player.Position) - target.Rotation;
        float regionDegrees = PositionalMath.ToDegrees(rotation) + (rotation < 0f ? 360f : 0f);

        return regionDegrees switch
        {
            >= 315f or <= 45f       => AttackAngle.Front,   // 0° ± 45°
            >= 45f and <= 135f      => AttackAngle.Flank,   // 90° ± 45°
            >= 135f and <= 225f     => AttackAngle.Rear,    // 180° ± 45°
            >= 225f and <= 315f     => AttackAngle.Flank,   // 270° ± 45°
            _                       => AttackAngle.Unknown
        };
    }

    /// <summary> Performs positional calculations. Based on the excellent Resonant plugin. </summary>
    internal static class PositionalMath
    {
        public const float DegToRad = MathF.PI / 180f;
        public const float RadToDeg = 180f / MathF.PI;

        public static float ToRadians(float degrees) => degrees * DegToRad;
        public static float ToDegrees(float radians) => radians * RadToDeg;

        public static float GetRotation(Vector3 a, Vector3 b) => MathF.Atan2(b.X - a.X, b.Z - a.Z);
        public static Vector3 GetDirection(Vector3 a, Vector3 b) => ToDirection(GetRotation(a, b));

        public static float ToRotation(Vector3 direction) => MathF.Atan2(direction.X, direction.Z);
        public static Vector3 ToDirection(float rotation) => new(MathF.Sin(rotation), 0f, MathF.Cos(rotation));
    }

    #endregion

    #endregion

    #region Shape Checks

    /// <summary> Gets the number of enemies within range of a point-blank AoE. </summary>
    public static int CanCircleAoe(float effectRange, bool checkIgnoredList = false)
    {
        if (LocalPlayer is not { } player) return 0;

        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsTargetable &&
                                      o.IsHostile() &&
                                      !TargetIsInvincible(o) &&
                                      (!checkIgnoredList || !Service.Configuration.IgnoredNPCs.ContainsKey(o.DataId)) &&
                                      PointInCircle(o.Position - player.Position, effectRange + o.HitboxRadius));
    }

    /// <summary> Gets the number of enemies within range of a targeted AoE. </summary>
    public static int CanRangedCircleAoe(IGameObject? target, float effectRange, bool checkIgnoredList = false)
    {
        if (target is null) return 0;

        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsTargetable &&
                                      o.IsHostile() &&
                                      !TargetIsInvincible(o) &&
                                      (!checkIgnoredList || !Service.Configuration.IgnoredNPCs.ContainsKey(o.DataId)) &&
                                      PointInCircle(o.Position - target.Position, effectRange + o.HitboxRadius));
    }

    /// <summary> Gets the number of enemies within range of a cone AoE. </summary>
    public static int CanConeAoe(IGameObject? target, float range, bool checkIgnoredList = false)
    {
        if (LocalPlayer is not { } player || target is null) return 0;

        Vector3 direction = PositionalMath.GetDirection(player.Position, target.Position);

        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsTargetable &&
                                      o.IsHostile() &&
                                      !TargetIsInvincible(o) &&
                                      GetTargetDistance(o) <= range &&
                                      (!checkIgnoredList || !Service.Configuration.IgnoredNPCs.ContainsKey(o.DataId)) &&
                                      PointInCone(o.Position - player.Position, direction, 45f));
    }

    /// <summary> Gets the number of enemies within range of a line AoE. </summary>
    public static int CanLineAoe(IGameObject? target, float range, float xAxisModifier, bool checkIgnoredList = false)
    {
        if (LocalPlayer is not { } player || target is null) return 0;

        float halfLength = range * 0.5f;
        float halfWidth = xAxisModifier * 0.5f;
        float rotation = PositionalMath.GetRotation(player.Position, target.Position);

        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsTargetable &&
                                      o.IsHostile() &&
                                      !TargetIsInvincible(o) &&
                                      GetTargetDistance(o) <= range &&
                                      (!checkIgnoredList || !Service.Configuration.IgnoredNPCs.ContainsKey(o.DataId)) &&
                                      HitboxInRect(o, rotation, halfLength, halfWidth));
    }

    #region Shape Helpers

    #region Point in Circle
    public static bool PointInCircle(Vector3 offsetFromOrigin, float radius)
    {
        return offsetFromOrigin.LengthSquared() <= radius * radius;
    }
    #endregion

    #region Point in Cone
    public static bool PointInCone(Vector3 offsetFromOrigin, Vector3 direction, float halfAngle)
    {
        return Vector3.Dot(Vector3.Normalize(offsetFromOrigin), direction) > MathF.Cos(halfAngle);
    }
    #endregion

    #region Point in Rect
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

    #endregion

    #endregion
}