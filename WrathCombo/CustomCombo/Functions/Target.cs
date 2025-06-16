using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
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

    /// <summary> Checks if the player is being targeted by a hostile target. </summary>
    public static bool IsPlayerTargeted() => Svc.Objects.Any(x => x.IsHostile() && x.IsTargetable && x.TargetObjectId == LocalPlayer?.GameObjectId);

    internal static bool TargetIsBoss() => IsBoss(LocalPlayer.TargetObject);

    internal static bool IsBoss(IGameObject? target) => target is not null && Svc.Data.GetExcelSheet<BNpcBase>().TryGetRow(target.DataId, out var dataRow) && dataRow.Rank is 2 or 6;

    internal static unsafe bool IsQuestMob(IGameObject? target) => target is not null && target.Struct()->NamePlateIconId is 71204 or 71144 or 71224 or 71344;

    internal static bool TargetIsHostile() => HasTarget() && CurrentTarget.IsHostile();

    public static bool TargetIsFriendly(IGameObject? OurTarget = null)
    {
        OurTarget ??= CurrentTarget;
        if (OurTarget is null)
            return false;

        return OurTarget.ObjectKind switch
        {
            ObjectKind.Player => true,
            _ when OurTarget is IBattleNpc npc => npc.BattleNpcKind is not BattleNpcSubKind.Enemy and not (BattleNpcSubKind)1,
            _ => false
        };
    }

    public static bool HasBattleTarget() => CurrentTarget is not null && CurrentTarget.IsHostile();

    public static bool TargetNeedsPositionals(IGameObject? ourTarget = null)
    {
        ourTarget ??= CurrentTarget;
        if (ourTarget is not IBattleChara target || HasStatusEffect(3808, target, true))
            return false;

        return Svc.Data.GetExcelSheet<BNpcBase>().TryGetRow(target.DataId, out var dataRow) && !dataRow.IsOmnidirectional;
    }

    /// <summary>
    ///     Determines if the enemy is casting an action. Optionally, limit by percentage of cast time.
    /// </summary>
    /// <param name="minCastPercentage">
    ///     The minimum percentage of the cast time completed required.<br/>
    ///     Default is 0%.<br/>
    ///     As a float representation of a percentage, value should be between
    ///     0.0f (0%) and 1.0f (100%).
    /// </param>
    /// <returns>
    ///     Bool indicating whether they are casting an action or not.<br/>
    ///     (and if the cast time is over the percentage specified)
    /// </returns>
    public static bool TargetIsCasting(double? minCastPercentage = null)
    {
        if (CurrentTarget is not IBattleChara chara) return false;

        minCastPercentage ??= 0.0f;
        minCastPercentage = Math.Clamp((double)minCastPercentage, 0.0d, 1.0d);
        double castPercentage = chara.CurrentCastTime / chara.TotalCastTime;

        if (chara.IsCasting)
            return minCastPercentage <= castPercentage;

        return false;
    }

    /// <summary>
    ///     Determines if the enemy is casting an action that can be interrupted.
    ///     <br/>
    ///     Optionally limited by percentage of cast time.
    /// </summary>
    /// <param name="minCastPercentage">
    ///     The minimum percentage of the cast time completed required.<br/>
    ///     Default is 0%.<br/>
    ///     As a float representation of a percentage, value should be between
    ///     0.0f (0%) and 1.0f (100%).
    /// </param>
    /// <returns>
    ///     Bool indicating whether they can be interrupted or not.<br/>
    ///     (and if the cast time is over the percentage specified)
    /// </returns>
    public static bool CanInterruptEnemy(double? minCastPercentage = null)
    {
        if (CurrentTarget is not IBattleChara chara) return false;

        minCastPercentage ??= Service.Configuration.InterruptDelay;
        minCastPercentage = Math.Clamp((double)minCastPercentage, 0.0d, 1.0d);
        double castPercentage = chara.CurrentCastTime / chara.TotalCastTime;

        if (chara is { IsCasting: true, IsCastInterruptible: true })
            return minCastPercentage <= castPercentage;

        return false;
    }

    internal static IEnumerable<IBattleChara> NearbyBosses => Svc.Objects.Where(x => x.ObjectKind == ObjectKind.BattleNpc && IsBoss(x)).Cast<IBattleChara>();

    #endregion

    #region HP Checks

    public static float PlayerHealthPercentageHp() => LocalPlayer is { } player ? player.CurrentHp * 100f / player.MaxHp : 0f;

    /// <summary> Gets a value indicating target's HP Percent. CurrentTarget is default unless specified </summary>
    public static float GetTargetHPPercent(IGameObject? OurTarget = null, bool includeShield = false)
    {
        OurTarget ??= CurrentTarget;
        if (OurTarget is not IBattleChara chara)
            return 0;

        float percent = (float)chara.CurrentHp / chara.MaxHp * 100f;
        if (includeShield) percent += chara.ShieldPercentage;
        return Math.Clamp(percent, 0f, 100f);
    }

    public static float EnemyHealthCurrentHp() => CurrentTarget is IBattleChara chara ? chara.CurrentHp : 0;

    public static float GetTargetMaxHP() => CurrentTarget is IBattleChara chara ? chara.MaxHp : 0;

    #endregion

    #region Distance Checks

    /// <summary> Gets a value indicating whether you are in melee range from the current target. </summary>
    public static bool InMeleeRange()
    {
        if (Svc.Targets.Target == null)
            return false;

        float distance = GetTargetDistance();

        return distance <= 3.0 + Service.Configuration.MeleeOffset;
    }

    /// <summary> Checks if target is in appropriate range for targeting </summary>
    /// <param name="target"> The target object to check </param>
    /// <param name="distance">Optional distance to check</param>
    public static bool IsInRange(IGameObject? target, float distance = 25f)
    {
        if (target == null || GetTargetDistance(target, LocalPlayer) >= distance)
            return false;

        return true;
    }

    /// <summary> Gets the distance from the target. </summary>
    public static float GetTargetDistance(IGameObject? optionalTarget = null, IGameObject? source = null)
    {
        if (LocalPlayer is null)
            return 0;

        IGameObject? chara = optionalTarget ?? CurrentTarget;
        if (chara is null) return 0;

        IGameObject? sourceChara = source ?? LocalPlayer;

        if (chara.GameObjectId == sourceChara.GameObjectId)
            return 0;

        Vector2 position = new(chara.Position.X, chara.Position.Z);
        Vector2 selfPosition = new(sourceChara.Position.X, sourceChara.Position.Z);

        return Math.Max(0, Vector2.Distance(position, selfPosition) - chara.HitboxRadius - sourceChara.HitboxRadius);
    }

    public static float GetTargetHeightDifference(IGameObject? target = null, IGameObject? source = null)
    {
        if (LocalPlayer is null)
            return 0;

        IGameObject? chara = target ?? CurrentTarget;
        if (chara is null) return 0;

        IGameObject? sourceChara = source ?? LocalPlayer;

        if (chara.GameObjectId == sourceChara.GameObjectId)
            return 0;

        return Math.Abs(chara.Position.Y - sourceChara.Position.Y);
    }

    public static int NumberOfEnemiesInRange(uint aoeSpell, IGameObject? target, bool checkIgnoredList = false)
    {
        ActionWatching.ActionSheet.Values.TryGetFirst(x => x.RowId == aoeSpell, out var sheetSpell);
        bool needsTarget = sheetSpell.CanTargetHostile;

        if (needsTarget && GetTargetDistance(target) > ActionWatching.GetActionRange(sheetSpell.RowId))
            return 0;


        int count = sheetSpell.CastType switch
        {
            1 => 1,
            2 => sheetSpell.CanTargetSelf ? CanCircleAoe(sheetSpell.EffectRange, checkIgnoredList) : CanRangedCircleAoe(sheetSpell.EffectRange, target, checkIgnoredList),
            3 => CanConeAoe(target, sheetSpell.Range, sheetSpell.EffectRange, checkIgnoredList),
            4 => CanLineAoe(target, sheetSpell.Range, sheetSpell.XAxisModifier, checkIgnoredList),
            _ => 0
        };

        return count;
    }

    public static int NumberOfEnemiesInRange(byte range)
    {
        return Svc.Objects.Count(
            o => o.ObjectKind == ObjectKind.BattleNpc &&
                 o.IsHostile() &&
                 o.IsTargetable &&
                 GetTargetDistance(o) <= range);
    }

    internal static unsafe bool IsInLineOfSight(IGameObject? target)
    {
        if (target is null) return false;
        if (!Player.Available) return false;

        var sourcePos = Player.Object.Struct()->Position;
        sourcePos.Y += 2;

        var targetPos = target.Struct()->Position;
        targetPos.Y += 2;

        var direction = targetPos - sourcePos;
        float distance = direction.Magnitude;

        direction = direction.Normalized;

        Vector3 originVect = new Vector3(sourcePos.X, sourcePos.Y, sourcePos.Z);
        Vector3 directionVect = new Vector3(direction.X, direction.Y, direction.Z);

        RaycastHit hit;
        int* flags = stackalloc int[] { 0x4000, 0, 0x4000, 0 };
        bool isLoSBlocked = Framework.Instance()->BGCollisionModule->RaycastMaterialFilter(&hit, &originVect, &directionVect, distance, 1, flags);

        return isLoSBlocked == false;
    }

    #endregion

    #region Positional Checks

    /// <summary> Is player on target's rear. </summary>
    /// <returns> True or false. </returns>
    public static bool OnTargetsRear() => AngleToTarget() is AttackAngle.Rear;

    /// <summary> Is player on target's flank. </summary>
    /// <returns> True or false. </returns>
    public static bool OnTargetsFlank() => AngleToTarget() is AttackAngle.Flank;

    /// <summary> Is player on target's front. </summary>
    /// <returns> True or false. </returns>
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
        if (LocalPlayer is not { } player || CurrentTarget is not IBattleChara target || target.ObjectKind != ObjectKind.BattleNpc) return AttackAngle.Unknown;

        float angle = PositionalMath.AngleXZ(target.Position, player.Position) - target.Rotation;
        float regionDegrees = PositionalMath.ToDegrees(angle) + (angle < 0f ? 360f : 0f);

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
        public const float DegreesToRadians = MathF.PI / 180f;
        public const float RadiansToDegrees = 180f / MathF.PI;

        public static float ToRadians(float degrees) => degrees * DegreesToRadians;

        public static float ToDegrees(float radians) => radians * RadiansToDegrees;

        public static float AngleXZ(Vector3 a, Vector3 b) => MathF.Atan2(b.X - a.X, b.Z - a.Z);
    }

    #endregion

    #endregion

    #region Shape Checks

    // Circle Aoe
    public static int CanCircleAoe(float effectRange, bool checkIgnoredList = false)
    {
        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsHostile() &&
                                      o.IsTargetable &&
                                      !TargetIsInvincible(o) &&
                                      (checkIgnoredList ? !Service.Configuration.IgnoredNPCs.Any(x => x.Key == o.DataId) : true) &&
                                      PointInCircle(o.Position - LocalPlayer.Position, effectRange + o.HitboxRadius));
    }

    // Ranged Circle Aoe
    public static int CanRangedCircleAoe(float effectRange, IGameObject? target, bool checkIgnoredList = false)
    {
        if (target == null) return 0;
        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsHostile() &&
                                      o.IsTargetable &&
                                      !TargetIsInvincible(o) &&
                                      (checkIgnoredList ? !Service.Configuration.IgnoredNPCs.Any(x => x.Key == o.DataId) : true) &&
                                      PointInCircle(o.Position - target.Position, effectRange + o.HitboxRadius));
    }

    // Cone Aoe
    public static int CanConeAoe(IGameObject? target, float range, float effectRange, bool checkIgnoredList = false)
    {
        if (target is null) return 0;
        float dir = PositionalMath.AngleXZ(LocalPlayer.Position, target.Position);
        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsHostile() &&
                                      o.IsTargetable &&
                                      !TargetIsInvincible(o) &&
                                      GetTargetDistance(o) <= range &&
                                      (checkIgnoredList ? !Service.Configuration.IgnoredNPCs.Any(x => x.Key == o.DataId) : true) &&
                                      PointInCone(o.Position - LocalPlayer.Position, dir, 45f));
    }

    // Line Aoe
    public static int CanLineAoe(IGameObject? target, float range, float effectRange, bool checkIgnoredList = false)
    {
        if (target is null) return 0;
        float dir = PositionalMath.AngleXZ(LocalPlayer.Position, target.Position);
        return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                      o.IsHostile() &&
                                      o.IsTargetable &&
                                      !TargetIsInvincible(o) &&
                                      GetTargetDistance(o) <= range &&
                                      (checkIgnoredList ? !Service.Configuration.IgnoredNPCs.Any(x => x.Key == o.DataId) : true) &&
                                      HitboxInRect(o, dir, range, effectRange / 2));
    }

    #region Shape Helpers

    public static Vector3 DirectionToVec3(float direction)
    {
        return new(MathF.Sin(direction), 0, MathF.Cos(direction));
    }

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
    public static bool PointInCone(Vector3 offsetFromOrigin, float direction, float halfAngle)
    {
        return PointInCone(offsetFromOrigin, DirectionToVec3(direction), halfAngle);
    }
    #endregion
    #region Point in Rect

    public static bool HitboxInRect(IGameObject o, float direction, float lenFront, float halfWidth)
    {
        Vector2 A = new Vector2(LocalPlayer.Position.X, LocalPlayer.Position.Z);
        Vector2 d = new Vector2(MathF.Sin(direction), MathF.Cos(direction));
        Vector2 n = new Vector2(d.Y, -d.X);
        Vector2 P = new Vector2(o.Position.X, o.Position.Z);
        float R = o.HitboxRadius;

        Vector2 Q = A + d * (lenFront / 2);
        Vector2 P2 = P - Q;
        Vector2 Ptrans = new Vector2(Vector2.Dot(P2, n), Vector2.Dot(P2, d));
        Vector2 Pabs = new Vector2(Math.Abs(Ptrans.X), Math.Abs(Ptrans.Y));
        Vector2 Pcorner = new Vector2(Math.Abs(Ptrans.X) - halfWidth, Math.Abs(Ptrans.Y) - (lenFront / 2));
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
