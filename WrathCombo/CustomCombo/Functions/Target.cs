using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
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

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        private static readonly Dictionary<uint, bool> NPCPositionals = [];

        /// <summary> Gets the current target or null. </summary>
        public static IGameObject? CurrentTarget => Svc.Targets.Target;

        /// <summary> Checks if the player has a target. </summary>
        public static bool HasTarget() => CurrentTarget is not null;

        /// <summary> Checks if the target is a hostile object. </summary>
        public static bool HasBattleTarget() => HasTarget() && CurrentTarget.IsHostile();

        /// <summary> Checks if the target is within the specified range. </summary>
        public static bool IsTargetInRange(float maxRange, IGameObject? optionalTarget = null, IGameObject? sourceChara = null)
        {
            if (LocalPlayer is null) return false;

            IGameObject? target = optionalTarget ?? CurrentTarget;
            if (target is null) return false;

            IGameObject? source = sourceChara ?? LocalPlayer;
            if (target.GameObjectId == source.GameObjectId) return false;

            Vector2 offset = new(target.Position.X - source.Position.X, target.Position.Z - source.Position.Z);

            float squaredDistance = Vector2.Dot(offset, offset);
            float totalRadius = maxRange + target.HitboxRadius + source.HitboxRadius;

            return squaredDistance <= totalRadius * totalRadius;
        }

        /// <summary> Gets the distance from the target as a float value. </summary>
        public static float GetTargetDistance(IGameObject? optionalTarget = null, IGameObject? sourceChara = null)
        {
            if (LocalPlayer is null) return 0f;

            IGameObject? target = optionalTarget ?? CurrentTarget;
            if (target is null) return 0f;

            IGameObject? source = sourceChara ?? LocalPlayer;
            if (target.GameObjectId == source.GameObjectId) return 0f;

            Vector2 selfPosition = new(source.Position.X, source.Position.Z);
            Vector2 targetPosition = new(target.Position.X, target.Position.Z);

            return Math.Max(0f, Vector2.Distance(targetPosition, selfPosition) - target.HitboxRadius - source.HitboxRadius);
        }

        /// <summary> Gets the height distance from the target as a float value. </summary>
        public static float GetTargetHeightDifference(IGameObject? optionalTarget = null, IGameObject? sourceChara = null)
        {
            if (LocalPlayer is null) return 0f;

            IGameObject? target = optionalTarget ?? CurrentTarget;
            if (target is null) return 0f;

            IGameObject? source = sourceChara ?? LocalPlayer;
            if (target.GameObjectId == source.GameObjectId) return 0f;

            return Math.Abs(target.Position.Y - source.Position.Y);
        }

        /// <summary> Checks if the target is within melee range. </summary>
        public static bool InMeleeRange()
        {
            if (Svc.Targets.Target == null) return false;

            return IsTargetInRange(3f + (float)Service.Configuration.MeleeOffset);
        }

        /// <summary> Gets the current HP of the target as a percentage value. </summary>
        public static float GetTargetHPPercent(IGameObject? optionalTarget = null, bool includeShield = false)
        {
            optionalTarget ??= CurrentTarget;
            if (optionalTarget is not IBattleChara chara || chara.IsDead) return 0f;

            uint currentHP = chara.CurrentHp;
            uint maxHP = chara.MaxHp;

            if (currentHP == maxHP) return 100f;

            float percentHP = currentHP * 100f / maxHP;
            percentHP += includeShield ? chara.ShieldPercentage : 0f;

            return Math.Clamp(percentHP, 0f, 100f);
        }

        /// <summary> Gets the maximum HP of the target as a float value. </summary>
        public static float EnemyHealthMaxHp() => CurrentTarget is IBattleChara chara ? chara.MaxHp : 0;

        /// <summary> Gets the current HP of the target as a float value. </summary>
        public static float EnemyHealthCurrentHp() => CurrentTarget is IBattleChara chara ? chara.CurrentHp : 0;

        /// <summary> Gets the current HP of the player as a percentage value. </summary>
        public static float PlayerHealthPercentageHp()
        {
            if (LocalPlayer is null || LocalPlayer.IsDead) return 0f;

            uint currentHP = LocalPlayer.CurrentHp;
            uint maxHP = LocalPlayer.MaxHp;

            if (currentHP == maxHP) return 100f;

            return currentHP * 100f / maxHP;
        }

        /// <summary> Checks if the player is being targeted by a hostile object. </summary>
        public static bool IsPlayerTargeted() => Svc.Objects.Any(x => x.IsHostile() && x.IsTargetable && x.TargetObjectId == LocalPlayer?.GameObjectId);

        /// <summary> Checks if the target is a friendly object. </summary>
        public static bool HasFriendlyTarget(IGameObject? optionalTarget = null)
        {
            optionalTarget ??= CurrentTarget;
            if (optionalTarget is not IBattleChara chara) return false;

            return chara.ObjectKind switch
            {
                ObjectKind.Player => true,
                _ when chara is IBattleNpc npc => npc.BattleNpcKind is not BattleNpcSubKind.Enemy and not (BattleNpcSubKind)1,
                _ => false
            };
        }

        /// <summary> Gets the mouse-over target from the party list. Returns null if not found. </summary>
        public static unsafe IGameObject? GetMouseOverHealTarget()
        {
            try
            {
                GameObject* uiTargetPtr = Framework.Instance()->GetUIModule()->GetPronounModule()->UiMouseOverTarget;
                if (uiTargetPtr != null)
                {
                    var gameObjectId = uiTargetPtr->GetGameObjectId();
                    if (gameObjectId.ObjectId != 0)
                    {
                        IGameObject? uiTarget = Svc.Objects.FirstOrDefault(x => x.GameObjectId == gameObjectId.ObjectId);
                        if (uiTarget != null && HasFriendlyTarget(uiTarget))
                        {
                            return uiTarget;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return null;
        }

        /// <summary> Gets a healable target. Priority: Party UI Mouseover (optional) -> Soft Target -> Hard Target -> Player. </summary>
        public static IGameObject GetHealTarget(bool checkMOPartyUI = false)
        {
            ITargetManager tm = Svc.Targets;

            // Check optional mouseover party UI target first
            if (checkMOPartyUI && GetMouseOverHealTarget() is IGameObject uiTarget)
                return uiTarget;

            // Check soft target
            // Null checks to make sure HasFriendlyTarget doesn't use it's own failback
            if (tm.SoftTarget != null && HasFriendlyTarget(tm.SoftTarget))
                return tm.SoftTarget;

            // Check current target if not restricted to mouseover
            if (tm.Target != null && HasFriendlyTarget(tm.Target))
                return tm.Target;

            // Default to local player
            return LocalPlayer!;
        }

        /// <summary>
        ///     Determines if the enemy is casting an action. Optionally, limit by percentage of cast time.
        /// </summary>
        /// <param name="minCastPercentage">
        ///     The minimum percentage of the cast time completed required.
        ///     Default is 0%.
        ///     As a float representation of a percentage, value should be between
        ///     0.0f (0%) and 1.0f (100%).
        /// </param>
        /// <returns>
        ///     Bool indicating whether they are casting an action or not.
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
        ///     Optionally limited by percentage of cast time.
        /// </summary>
        /// <param name="minCastPercentage">
        ///     The minimum percentage of the cast time completed required.
        ///     Default is 0%.
        ///     As a float representation of a percentage, value should be between
        ///     0.0f (0%) and 1.0f (100%).
        /// </param>
        /// <returns>
        ///     Bool indicating whether they can be interrupted or not.
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

        /// <summary> Sets the player's current target. </summary>
        /// <param name="target"> Must be an object the player can normally click and target. </param>
        public static void SetTarget(IGameObject? target) => Svc.Targets.Target = target;

        /// <summary> Sets the player's current target to the given target. </summary>
        public static void TargetObject(IGameObject? target)
        {
            if (IsInRange(target))
                SetTarget(target);
        }

        /// <summary> Checks if the target is within appropriate range for targeting. </summary>
        /// <param name="target"> The target object to check. </param>
        /// <param name="distance"> The distance to check. </param>
        public static bool IsInRange(IGameObject? target, float distance = 25f)
        {
            if (target is null) return false;

            return IsTargetInRange(distance, target, LocalPlayer);
        }

        /// <summary> Checks if the target needs positionals. </summary>
        public static bool TargetNeedsPositionals()
        {
            if (!HasBattleTarget()) return false;
            if (HasStatusEffect(3808, CurrentTarget, true)) return false; // Directional Disregard Effect (Patch 7.01)
            if (!NPCPositionals.ContainsKey(CurrentTarget.DataId))
            {
                if (Svc.Data.GetExcelSheet<BNpcBase>().TryGetFirst(x => x.RowId == CurrentTarget.DataId, out var bnpc))
                    NPCPositionals[CurrentTarget.DataId] = bnpc.IsOmnidirectional;
            }
            return !NPCPositionals[CurrentTarget.DataId];
        }

        /// <summary> Sets the player's current target to the given party member. </summary>
        /// <param name="target"> The party member to target. </param>
        protected static unsafe void TargetObject(TargetType target)
        {
            GameObject* t = GetTarget(target);
            if (t == null) return;
            ulong o = PartyTargetingService.GetObjectID(t);
            IGameObject? p = Svc.Objects.Where(x => x.GameObjectId == o).First();

            if (IsInRange(p)) SetTarget(p);
        }

        /// <summary> Gets a specific type of target. </summary>
        public static unsafe GameObject* GetTarget(TargetType target)
        {
            IGameObject? o = null;

            switch (target)
            {
                case TargetType.Target:
                    o = Svc.Targets.Target;
                    break;
                case TargetType.SoftTarget:
                    o = Svc.Targets.SoftTarget;
                    break;
                case TargetType.FocusTarget:
                    o = Svc.Targets.FocusTarget;
                    break;
                case TargetType.UITarget:
                    return PartyTargetingService.UITarget;
                case TargetType.FieldTarget:
                    o = Svc.Targets.MouseOverTarget;
                    break;
                case TargetType.TargetsTarget when Svc.Targets.Target is { TargetObjectId: not 0xE0000000 }:
                    o = Svc.Targets.Target.TargetObject;
                    break;
                case TargetType.Self:
                    o = Svc.ClientState.LocalPlayer;
                    break;
                case TargetType.LastTarget:
                    return PartyTargetingService.GetGameObjectFromPronounID(1006);
                case TargetType.LastEnemy:
                    return PartyTargetingService.GetGameObjectFromPronounID(1084);
                case TargetType.LastAttacker:
                    return PartyTargetingService.GetGameObjectFromPronounID(1008);
                case TargetType.P2:
                    return PartyTargetingService.GetGameObjectFromPronounID(44);
                case TargetType.P3:
                    return PartyTargetingService.GetGameObjectFromPronounID(45);
                case TargetType.P4:
                    return PartyTargetingService.GetGameObjectFromPronounID(46);
                case TargetType.P5:
                    return PartyTargetingService.GetGameObjectFromPronounID(47);
                case TargetType.P6:
                    return PartyTargetingService.GetGameObjectFromPronounID(48);
                case TargetType.P7:
                    return PartyTargetingService.GetGameObjectFromPronounID(49);
                case TargetType.P8:
                    return PartyTargetingService.GetGameObjectFromPronounID(50);
            }

            return o != null ? (GameObject*)o.Address : null;
        }

        /// <summary> List of possible target types. </summary>
        public enum TargetType
        {
            Target,
            SoftTarget,
            FocusTarget,
            UITarget,
            FieldTarget,
            TargetsTarget,
            Self,
            LastTarget,
            LastEnemy,
            LastAttacker,
            P2,
            P3,
            P4,
            P5,
            P6,
            P7,
            P8
        }

        /// <summary> Gets the player's angle relative to the target. </summary>
        public static float AngleToTarget()
        {
            if (LocalPlayer is null || CurrentTarget is not IBattleChara chara || chara.ObjectKind != ObjectKind.BattleNpc) return 0f;

            float angle = PositionalMath.AngleXZ(chara.Position, LocalPlayer.Position) - chara.Rotation;
            float regionDegrees = (PositionalMath.Degrees(angle) + 360) % 360;

            return regionDegrees switch
            {
                >= 315 or <= 45     => 4f, // Front (0° ± 45°)
                >= 45 and <= 135    => 1f, // Right Flank (90° ± 45°)
                >= 135 and <= 225   => 2f, // Rear (180° ± 45°)
                >= 225 and <= 315   => 3f, // Left Flank (270° ± 45°)
                _                   => 0f  // Fallback
            };
        }

        /// <summary> Checks if the player is on the target's rear. </summary>
        public static bool OnTargetsRear() => AngleToTarget() is 2f;

        /// <summary> Checks if the player is on either flank of the target. </summary>
        public static bool OnTargetsFlank() => AngleToTarget() is 1f or 3f;

        /// <summary> Checks if the player is on the target's front. </summary>
        public static bool OnTargetsFront() => AngleToTarget() is 4f;

        /// <summary> Checks if an object is quest-related. </summary>
        internal static unsafe bool IsQuestMob(IGameObject target) => target.Struct()->NamePlateIconId is 71204 or 71144 or 71224 or 71344;

        /// <summary> Checks if the target is an instance boss. </summary>
        internal static bool TargetIsBoss() => IsBoss(LocalPlayer.TargetObject);

        /// <summary> Checks if an object is an instance boss.
        ///     Rank 0 - Trash.
        ///     Rank 1 - Hunt Targets (B/A/S), FATE Bosses (Shadowbringers+).
        ///     Rank 2 - Final Dungeon Boss, Trial Boss, Raid Boss, Alliance Raid Boss.
        ///     Rank 3 - Trash.
        ///     Rank 4 - Raid Trash (Alexander).
        ///     Rank 5 - Nothing.
        ///     Rank 6 - First 2 Dungeon Bosses.
        ///     Rank 7 - PvP Targets.
        ///     Rank 8 - A puppy.
        ///     Rank 32, 33, 34, 35, 36, 37 - Old Diadem Targets.
        /// </summary>
        private static bool IsBoss(IGameObject? obj) =>
            obj is not null &&
            Svc.Data.GetExcelSheet<BNpcBase>().HasRow(obj.DataId) &&
            Svc.Data.GetExcelSheet<BNpcBase>().GetRow(obj.DataId).Rank is 2 or 6;

        /// <summary> Gets a list of instance bosses from the Object Table. </summary>
        internal static IEnumerable<IBattleChara> NearbyBosses => Svc.Objects.Where(x => x.ObjectKind == ObjectKind.BattleNpc && IsBoss(x)).Cast<IBattleChara>();

        /// <summary> Checks if the object is out of range for a given action. </summary>
        internal static bool OutOfRange(uint actionID, IGameObject obj) => ActionWatching.OutOfRange(actionID, Svc.ClientState.LocalPlayer!, obj);

        /// <summary> Checks if there are enemies in range for a given action. </summary>
        public static unsafe bool EnemiesInRange(uint spellCheck)
        {
            var enemies = Svc.Objects.Where(x => x.ObjectKind == ObjectKind.BattleNpc).Cast<IBattleNpc>().Where(x => x.BattleNpcKind is BattleNpcSubKind.Enemy or BattleNpcSubKind.BattleNpcPart).ToList();
            foreach (var enemy in enemies)
            {
                var enemyChara = CharacterManager.Instance()->LookupBattleCharaByEntityId(enemy.EntityId);
                if (enemyChara->Character.InCombat)
                {
                    if (!ActionManager.CanUseActionOnTarget(7, enemy.GameObject())) continue;
                    if (!enemyChara->Character.GameObject.GetIsTargetable()) continue;

                    if (!OutOfRange(spellCheck, enemy))
                        return true;
                }

            }

            return false;
        }

        /// <summary> Gets the number of enemies in range for a given action. </summary>
        public static int NumberOfEnemiesInRange(uint aoeSpell, IGameObject? target, bool checkIgnoredList = false)
        {
            ActionWatching.ActionSheet.Values.TryGetFirst(x => x.RowId == aoeSpell, out var sheetSpell);
            bool needsTarget = sheetSpell.CanTargetHostile;

            if (needsTarget && !IsTargetInRange(ActionWatching.GetActionRange(sheetSpell.RowId), target)) return 0;

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

        /// <summary> Gets how many enemies are within range for a point-blank circle AoE. </summary>
        public static int CanCircleAoe(float effectRange, bool checkIgnoredList = false)
        {
            if (LocalPlayer is null) return 0;

            var playerPos = LocalPlayer.Position;
            var configIgnored = checkIgnoredList ? Service.Configuration.IgnoredNPCs : null;

            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 !TargetIsInvincible(o) &&
                                                                 (!checkIgnoredList || !configIgnored!.Any(x => x.Key == o.DataId)) &&
                                                                 AreaMath.PointInCircle(o.Position - playerPos, effectRange + o.HitboxRadius));
        }

        /// <summary> Gets how many enemies are within range for a targeted circle AoE. </summary>
        public static int CanRangedCircleAoe(float effectRange, IGameObject? target, bool checkIgnoredList = false)
        {
            if (target is null) return 0;

            var targetPos = target.Position;
            var configIgnored = checkIgnoredList ? Service.Configuration.IgnoredNPCs : null;

            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 !TargetIsInvincible(o) &&
                                                                 (!checkIgnoredList || !configIgnored!.Any(x => x.Key == o.DataId)) &&
                                                                 AreaMath.PointInCircle(o.Position - targetPos, effectRange + o.HitboxRadius));
        }

        /// <summary> Gets how many enemies are within range for a cone AoE. </summary>
        public static int CanConeAoe(IGameObject? target, float range, float effectRange, bool checkIgnoredList = false)
        {
            if (LocalPlayer is null || target is null) return 0;

            var playerPos = LocalPlayer.Position;
            var dir = PositionalMath.AngleXZ(playerPos, target.Position);
            var configIgnored = checkIgnoredList ? Service.Configuration.IgnoredNPCs : null;

            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 !TargetIsInvincible(o) &&
                                                                 IsTargetInRange(range, o) &&
                                                                 (!checkIgnoredList || !configIgnored!.Any(x => x.Key == o.DataId)) &&
                                                                 AreaMath.PointInCone(o.Position - playerPos, dir, effectRange));
        }

        /// <summary> Gets how many enemies are within range for a line AoE. </summary>
        public static int CanLineAoe(IGameObject? target, float range, float effectRange, bool checkIgnoredList = false)
        {
            if (LocalPlayer is null || target is null) return 0;

            float halfWidth = effectRange / 2f;
            float dir = PositionalMath.AngleXZ(LocalPlayer.Position, target.Position);
            var configIgnored = checkIgnoredList ? Service.Configuration.IgnoredNPCs : null;

            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 !TargetIsInvincible(o) &&
                                                                 IsTargetInRange(range, o) &&
                                                                 (!checkIgnoredList || !configIgnored!.Any(x => x.Key == o.DataId)) &&
                                                                 AreaMath.HitboxInRect(o, dir, range, halfWidth));
        }

        /// <summary> Checks if an object is in line of sight of the player. </summary>
        public static unsafe bool IsInLineOfSight(IGameObject? target)
        {
            if (LocalPlayer is null || target is null) return false;

            Vector3 sourcePos = LocalPlayer.Position with { Y = LocalPlayer.Position.Y + 2f };
            Vector3 targetPos = target.Position with { Y = target.Position.Y + 2f };

            Vector3 offset = targetPos - sourcePos;
            Vector3 direction = Vector3.Normalize(offset);
            float distance = offset.Length();

            RaycastHit hit;
            var flags = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

            return !Framework.Instance()->BGCollisionModule->RaycastMaterialFilter(&hit, &sourcePos, &direction, distance, 1, flags);
        }

        /// <summary> Performs positional calculations. Based on the excellent Resonant plugin. </summary>
        internal static class PositionalMath
        {
            public const float DegreesToRadians = MathF.PI / 180f;
            public const float RadiansToDegrees = 180f / MathF.PI;

            internal static float Radians(float degrees) => degrees * DegreesToRadians;

            internal static float Degrees(float radians) => radians * RadiansToDegrees;

            internal static float AngleXZ(Vector3 a, Vector3 b) => MathF.Atan2(b.X - a.X, b.Z - a.Z);
        }

        /// <summary> Performs area calculations. </summary>
        internal static class AreaMath
        {
            public static Vector3 DirectionToVector3(float direction)
            {
                return new(MathF.Sin(direction), 0, MathF.Cos(direction));
            }

            public static bool PointInCircle(Vector3 offsetFromOrigin, float radius)
            {
                return offsetFromOrigin.LengthSquared() <= radius * radius;
            }

            public static bool PointInCone(Vector3 offsetFromOrigin, Vector3 direction, float halfAngle)
            {
                return Vector3.Dot(Vector3.Normalize(offsetFromOrigin), direction) > MathF.Cos(halfAngle);
            }
            public static bool PointInCone(Vector3 offsetFromOrigin, float direction, float halfAngle)
            {
                return PointInCone(offsetFromOrigin, DirectionToVector3(direction), halfAngle);
            }

            public static bool HitboxInRect(IGameObject o, float direction, float lenFront, float halfWidth)
            {
                Vector2 A = new(LocalPlayer.Position.X, LocalPlayer.Position.Z);
                Vector2 d = new(MathF.Sin(direction), MathF.Cos(direction));
                Vector2 n = new(d.Y, -d.X);
                Vector2 P = new(o.Position.X, o.Position.Z);
                float R = o.HitboxRadius;

                Vector2 Q = A + d * (lenFront / 2);
                Vector2 P2 = P - Q;
                Vector2 Ptrans = new(Vector2.Dot(P2, n), Vector2.Dot(P2, d));
                Vector2 Pabs = new(Math.Abs(Ptrans.X), Math.Abs(Ptrans.Y));
                Vector2 Pcorner = new(Math.Abs(Ptrans.X) - halfWidth, Math.Abs(Ptrans.Y) - (lenFront / 2));

                #region Debug
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
                #endregion

                if (Pcorner.X > R || Pcorner.Y > R)
                    return false;

                if (Pcorner.X <= 0 || Pcorner.Y <= 0)
                    return true;

                return Pcorner.LengthSquared() <= R * R;
            }
        }
    }
}