using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.Data;
using WrathCombo.Extensions;
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

        return ActionWatching.BNPCSheet.TryGetValue(chara.DataId, out var charaSheet) && charaSheet.Rank is 2 or 6;
    }

    /// <summary> Checks if an object is quest-related. Defaults to CurrentTarget unless specified. </summary>
    internal static unsafe bool IsQuestMob(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not { } chara)
            return false;

        return chara.Struct()->NamePlateIconId is 71204 or 71144 or 71224 or 71344;
    }

    /// <summary> Checks if an object is friendly. Defaults to CurrentTarget unless specified. </summary>
    public unsafe static bool TargetIsFriendly(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not { } chara)
            return false;

        //This is a glorified universal check for friendly targets. Will return a correct value regardless of role, level or whatever.
        var ret = ActionManager.CanUseActionOnTarget(Healer.Role.Esuna, chara.Struct());
        return ret;
    }

    /// <summary> Checks if the player's current target is hostile. </summary>
    public static bool HasBattleTarget() => HasTarget() && CurrentTarget.IsHostile();

    /// <summary> Checks if an object requires positionals. Defaults to CurrentTarget unless specified. </summary>
    public static bool TargetNeedsPositionals(IGameObject? optionalTarget = null)
    {
        if ((optionalTarget ?? CurrentTarget) is not IBattleChara chara || HasStatusEffect(3808, chara, true))
            return false;

        return ActionWatching.BNPCSheet.TryGetValue(chara.DataId, out var charaSheet) && !charaSheet.IsOmnidirectional;
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
        if ((optionalTarget ?? CurrentTarget) is not IBattleChara { IsCasting: true } chara || !chara.IsCastInterruptible)
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
    /// <param name="aoeSpell">
    ///     The Action ID to check. <br />
    ///     This will be used to load up all required data from the Action Sheet.
    /// </param>
    /// <param name="target">
    ///     Target for targeted AoE shapes (all but <see cref="SelfCircle"/>). <br />
    ///     (Optional, defaults to <see cref="CurrentTarget" />)
    /// </param>
    /// <param name="checkIgnoredList">
    ///     Whether to check the 
    ///     <see cref="PluginConfiguration.IgnoredNPCs"/> list. <br />
    ///     (Optional, defaults to false)
    /// </param>
    /// <returns>
    ///     Number of enemies within the specified action's range.
    /// </returns>
    public static int NumberOfEnemiesInRange
        (uint aoeSpell, IGameObject? target = null, bool checkIgnoredList = false)
    {
        if (!ActionWatching.ActionSheet.TryGetValue(aoeSpell, out var sheetSpell))
            return 0;

        if (sheetSpell.CanTargetHostile &&
            ((target ??= CurrentTarget) is null ||
             GetTargetDistance(target) > GetActionRange(sheetSpell.RowId)))
            return 0;

        var count = sheetSpell.CastType switch
        {
            1 => 1,
            2 => sheetSpell.CanTargetSelf
                ? NumberOfObjectsInRange<SelfCircle>(sheetSpell.EffectRange,
                    checkIgnoredList: checkIgnoredList)
                : NumberOfObjectsInRange<Circle>(sheetSpell.EffectRange, target,
                    checkIgnoredList: checkIgnoredList),
            3 => NumberOfObjectsInRange<Cone>(sheetSpell.Range, target,
                checkIgnoredList: checkIgnoredList),
            4 => NumberOfObjectsInRange<Line>(sheetSpell.Range, target,
                sheetSpell.XAxisModifier, checkIgnoredList: checkIgnoredList),
            _ => 0,
        };

        return count;
    }

    /// <summary>
    ///     Gets the number of allies within range of an AoE action. <br/>
    ///     If the action requires a target, defaults to CurrentTarget unless specified.
    /// </summary>
    /// <param name="aoeSpell">
    ///     The Action ID to check. <br />
    ///     This will be used to load up all required data from the Action Sheet.
    /// </param>
    /// <param name="target">
    ///     Target for targeted AoE shapes (all but <see cref="SelfCircle"/>). <br />
    ///     (Optional, defaults to <see cref="CurrentTarget" />)
    /// </param>
    /// <returns>
    ///     Number of allies within the specified action's range.
    /// </returns>
    public static int NumberOfAlliesInRange
        (uint aoeSpell, IGameObject? target = null)
    {
        if (!ActionWatching.ActionSheet.TryGetValue(aoeSpell, out var sheetSpell))
            return 0;

        if (sheetSpell.CanTargetAlly &&
            ((target ??= CurrentTarget) is null ||
             GetTargetDistance(target) > GetActionRange(sheetSpell.RowId)))
            return 0;

        var count = sheetSpell.CastType switch
        {
            1 => 1,
            2 => sheetSpell.CanTargetSelf
                ? NumberOfObjectsInRange<SelfCircle>(sheetSpell.EffectRange,
                    enemies: false)
                : NumberOfObjectsInRange<Circle>(sheetSpell.EffectRange, target,
                    enemies: false),
            // No current cones or lines for allies, but may as well have them here
            3 => NumberOfObjectsInRange<Cone>(sheetSpell.Range, target,
                enemies: false),
            4 => NumberOfObjectsInRange<Line>(sheetSpell.Range, target,
                sheetSpell.XAxisModifier, enemies: false),
            _ => 0,
        };

        return count;
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

    /// <summary>
    ///     Checks if an object is over the ground
    /// </summary>
    internal static unsafe bool IsOverGround(IGameObject? obj)
    {
        if (obj is null) return false;

        var targetPos = obj.Position;
        var down = new Vector3(0, -1, 0);
        RaycastHit hit;
        var flags = stackalloc int[] { 0x4000, 0, 0x4000, 0 };
        
        return Framework.Instance()->BGCollisionModule->RaycastMaterialFilter(&hit, &targetPos, &down, 5, 1, flags);
    }

    /// <summary>
    ///     Checks if a point is over the ground.<br/>
    ///     (and gives the ground point if it is)
    /// </summary>
    private static unsafe bool IsOverGround
        (Vector3 pointToCheck, out Vector3 groundPoint)
    {
        var down = new Vector3(0, -1, 0);
        RaycastHit hit;
        var flags = stackalloc int[] { 0x4000, 0, 0x4000, 0 };
        
        var result = Framework.Instance()->BGCollisionModule->RaycastMaterialFilter(&hit, &pointToCheck, &down, 5, 1, flags);
        groundPoint = hit.Point;
        return result;
    }

    /// <summary>
    ///     Tries to find the nearest point to the object that is in a line
    ///     between the player and the object, and is over the ground.
    /// </summary>
    /// <param name="obj">
    ///     The object to start searching from.
    /// </param>
    /// <param name="nearestGroundPoint">
    ///     The found nearest point.
    /// </param>
    /// <param name="maxRange">
    ///     The maximum range from the the player.<br/>
    ///     Starts the search closer to the player than just the object's position.
    /// </param>
    /// <returns>
    ///     If a suitable point was found.
    /// </returns>
    internal static bool TryGetNearestGroundPointWithinRange
    (IGameObject? obj,
        out Vector3? nearestGroundPoint,
        float maxRange = 30f)
    {
        nearestGroundPoint = null;

        // Fail out if we can't find positions
        if (LocalPlayer is not { } player || obj is null) return false;

        // (search 2y up, to avoid the line crossing the ground itself)
        var source = obj.Position with { Y = obj.Position.Y + 2f };
        var target = player.Position with { Y = obj.Position.Y + 2f };
        var direction = Vector3.Normalize(target - source); // The line to walk
        var distance = Vector3.Distance(source, target); // The length of that line

        // If the player and object are too close, return the player position
        if (distance <= 0.5f)
            return (nearestGroundPoint = player.Position) != null;

        // Start closer to the player if the object is outside of maximum range
        var start = MathF.Max(0f, distance - maxRange) + 0.1f;

        // Walks from the object towards the player in 0.5 yalms increments
        for (var i = start; i <= distance; i += 0.5f)
        {
            var point = source + direction * i;
            // Skip if not a suitable point
            if (!IsOverGround(point, out var groundPoint)) continue;

            nearestGroundPoint = groundPoint;
            return true;
        }

        // Fail out if no suitable point was found
        return false;
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
    public static AttackAngle AngleToTarget(IGameObject? optionalTarget = null)
    {
        if (LocalPlayer is not { } player)
            return AttackAngle.Unknown;

        var target = optionalTarget ?? CurrentTarget;

        if (target is null)
            return AttackAngle.Unknown;

        float rotation = PositionalMath.GetRotation(target.Position, player.Position) - target.Rotation;
        float regionDegrees = PositionalMath.ToDegrees(rotation) + (rotation < 0f ? 360f : 0f);

        return regionDegrees switch
        {
            >= 315f or <= 45f => AttackAngle.Front,   // 0° ± 45°
            >= 45f and <= 135f => AttackAngle.Flank,   // 90° ± 45°
            >= 135f and <= 225f => AttackAngle.Rear,    // 180° ± 45°
            >= 225f and <= 315f => AttackAngle.Flank,   // 270° ± 45°
            _ => AttackAngle.Unknown
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

    /// <summary>
    ///     Gets the number of enemies within range of a specified AoE shape type.
    /// </summary>
    /// <typeparam name="T">
    ///     The AoE shape type (<see cref="SelfCircle"/>,
    ///     <see cref="Circle"/>, <see cref="Cone"/>, <see cref="Line"/>)<br />
    ///     Types that implement <see cref="IAoeShape" />.
    /// </typeparam>
    /// <param name="size">
    ///     The radius of the AoE (or length, for Line AoEs).
    /// </param>
    /// <param name="target">
    ///     Target for targeted AoE shapes (all but <see cref="SelfCircle"/>). <br />
    ///     (Optional, defaults to <see cref="CurrentTarget" />)
    /// </param>
    /// <param name="width">
    ///     Width parameter - Only for Line AoEs.  <br />
    ///     In the sheets, this column is labeled as "X Axis Modifier", and is
    ///     usually 0-5. <br />
    ///     (Optional, defaults to 0, which is the value for many Line AoEs)
    /// </param>
    /// <param name="checkIgnoredList">
    ///     Whether to check the 
    ///     <see cref="PluginConfiguration.IgnoredNPCs"/> list. <br />
    ///     (Optional, defaults to false)
    /// </param>
    /// <param name="enemies">
    ///     Whether enemy targets are what is searched;
    ///     if <see langword="false" /> then will search for allies instead.
    /// </param>
    /// <param name="checkInvincible">
    ///     Whether enemies should be checked for invincibility.<br />
    ///     Should only be set to <see langword="false" /> by
    ///     <see cref="CustomComboFunctions.TargetIsInvincible"/>.
    /// </param>
    /// <returns>
    ///     Number of enemies within the specified AoE shape.
    /// </returns>
    /// <remarks>
    ///     In almost every situation you should instead use
    ///     <see cref="NumberOfEnemiesInRange(uint, IGameObject?, bool)"/>.
    /// </remarks>
    internal static int NumberOfObjectsInRange<T>
    (float size,
        IGameObject? target = null,
        float width = 0f,
        bool checkIgnoredList = false,
        bool enemies = true,
        bool checkInvincible = true)
        where T : IAoeShape
    {
        // Bail if the player is not available
        if (LocalPlayer is not { } player)
            return 0;
        
        // Bail if the target is required and not available
        if (typeof(T) != typeof(SelfCircle) && (target ??= CurrentTarget) is null)
            return 0;

        // Get all possible enemies to search for the positions of
        var targets = Svc.Objects.Where(IsValidTarget);

        // Circle AoEs positioned on self
        if (typeof(T) == typeof(SelfCircle))
            return targets.Count(o =>
                PointInCircle(o.Position - player.Position,
                    size + o.HitboxRadius));

        // Circle AoEs centered on a target
        if (typeof(T) == typeof(Circle))
            return targets.Count(o => 
                PointInCircle(o.Position - target.Position,
                    size + o.HitboxRadius));
        
        // Cone AoEs
        if (typeof(T) == typeof(Cone))
            return targets.Count(o =>
                GetTargetDistance(o) <= size &&
                PointInCone(o.Position - player.Position,
                    PositionalMath.GetDirection(player.Position, target.Position),
                    45f));
        
        // Line AoEs
        if (typeof(T) == typeof(Line))
            return targets.Count(o =>
                GetTargetDistance(o) <= size &&
                HitboxInRect(o,
                    PositionalMath.GetRotation(player.Position, target.Position),
                    size * 0.5f, width * 0.5f));
        
        // If it was not a supported type
        return 0;
        
        bool IsValidTarget(IGameObject o)
        {
            if (!enemies)
                return o is IBattleChara &&
                       o.IsTargetable &&
                       o.IsWithinRange(60f) &&
                       o.IsFriendly();
            
            return o is { ObjectKind: ObjectKind.BattleNpc, IsTargetable: true } &&
                   o.IsWithinRange(60f) &&
                   o.IsHostile() &&
                   (!checkInvincible ||
                    !TargetIsInvincible(o)) &&
                   (!checkIgnoredList ||
                    !Service.Configuration.IgnoredNPCs.ContainsKey(o.DataId));
        }
    }

    #region Shape Helpers
    
    public interface IAoeShape { }
    public struct SelfCircle : IAoeShape { }
    public struct Circle : IAoeShape { }
    public struct Cone : IAoeShape { }
    public struct Line : IAoeShape { }

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