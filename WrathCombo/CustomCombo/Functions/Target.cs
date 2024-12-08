using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
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
        private static Dictionary<uint, bool> NPCPositionals = new Dictionary<uint, bool>();
        /// <summary> Gets the current target or null. </summary>
        public static IGameObject? CurrentTarget => Svc.Targets.Target;

        /// <summary> Find if the player has a target. </summary>
        /// <returns> A value indicating whether the player has a target. </returns>
        public static bool HasTarget() => CurrentTarget is not null;

        /// <summary> Gets the distance from the target. </summary>
        /// <returns> Double representing the distance from the target. </returns>
        public static float GetTargetDistance(IGameObject? optionalTarget = null, IGameObject? source = null)
        {
            if (LocalPlayer is null)
                return 0;

            IGameObject? chara = optionalTarget != null ? optionalTarget : CurrentTarget != null ? CurrentTarget : null;
            if (chara is null) return 0;

            IGameObject? sourceChara = source != null ? source : LocalPlayer;

            if (chara.GameObjectId == sourceChara.GameObjectId)
                return 0;

            Vector2 position = new(chara.Position.X, chara.Position.Z);
            Vector2 selfPosition = new(sourceChara.Position.X, sourceChara.Position.Z);
            return Math.Max(0, Vector2.Distance(position, selfPosition) - chara.HitboxRadius);
        }

        /// <summary> Gets a value indicating whether you are in melee range from the current target. </summary>
        /// <returns> Bool indicating whether you are in melee range. </returns>
        public static bool InMeleeRange()
        {
            if (Svc.Targets.Target == null)
                return false;

            float distance = GetTargetDistance();

            if (distance == 0)
                return true;

            if (distance > 3.5 + Service.Configuration.MeleeOffset)
                return false;

            return true;
        }

        /// <summary> Gets a value indicating target's HP Percent. CurrentTarget is default unless specified </summary>
        /// <returns> Double indicating percentage. </returns>
        public static float GetTargetHPPercent(IGameObject? OurTarget = null, bool includeShield = false)
        {
            if (OurTarget is null)
            {
                OurTarget = CurrentTarget; // Fallback to CurrentTarget
                if (OurTarget is null)
                    return 0;
            }

            if (OurTarget is IBattleChara chara)
            {
                float percent = (float)chara.CurrentHp / chara.MaxHp * 100f;
                if (includeShield) percent += chara.ShieldPercentage;
                return Math.Clamp(percent, 0f, 100f);
            }
            else return 0;
        }

        public static float EnemyHealthMaxHp()
        {
            if (CurrentTarget is null)
                return 0;
            if (CurrentTarget is not IBattleChara chara)
                return 0;

            return chara.MaxHp;
        }

        public static float EnemyHealthCurrentHp()
        {
            if (CurrentTarget is null)
                return 0;
            if (CurrentTarget is not IBattleChara chara)
                return 0;

            return chara.CurrentHp;
        }

        public static float PlayerHealthPercentageHp() => (float)LocalPlayer.CurrentHp / LocalPlayer.MaxHp * 100;

        public static bool HasBattleTarget() => CurrentTarget is IBattleNpc { BattleNpcKind: BattleNpcSubKind.Enemy or (BattleNpcSubKind)1 };

        /// <summary> Checks if the player's current target is also targeting the player. </summary>
        /// <returns> Bool indicating whether the player's current target is also targeting the player. </returns>
        public static bool IsPlayerTargeted() => CurrentTarget?.TargetObjectId == LocalPlayer.GameObjectId;

        public static bool HasFriendlyTarget(IGameObject? OurTarget = null)
        {
            if (OurTarget is null)
            {
                //Fallback to CurrentTarget
                OurTarget = CurrentTarget;
                if (OurTarget is null)
                    return false;
            }

            //Humans and Trusts
            if (OurTarget.ObjectKind is ObjectKind.Player)
                return true;
            //AI
            if (OurTarget is IBattleNpc) return (OurTarget as IBattleNpc).BattleNpcKind is not BattleNpcSubKind.Enemy and not (BattleNpcSubKind)1;
            return false;
        }

        /// <summary> Grabs healable target. Checks Soft Target then Hard Target. 
        /// If Party UI Mouseover is enabled, find the target and return that. Else return the player. </summary>
        /// <param name="checkMOPartyUI">Checks for a mouseover target.</param>
        /// <param name="restrictToMouseover">Forces only the mouseover target, may return null.</param>
        /// <returns> IGameObject of a player target. </returns>
        public static unsafe IGameObject? GetHealTarget(bool checkMOPartyUI = false, bool restrictToMouseover = false)
        {
            IGameObject? healTarget = null;
            ITargetManager tm = Svc.Targets;

            if (HasFriendlyTarget(tm.SoftTarget)) healTarget = tm.SoftTarget;
            if (healTarget is null && HasFriendlyTarget(CurrentTarget) && !restrictToMouseover) healTarget = CurrentTarget;
            //if (checkMO && HasFriendlyTarget(tm.MouseOverTarget)) healTarget = tm.MouseOverTarget;
            if (checkMOPartyUI)
            {
                GameObject* t = Framework.Instance()->GetUIModule()->GetPronounModule()->UiMouseOverTarget;
                if (t != null && t->GetGameObjectId().ObjectId != 0)
                {
                    IGameObject? uiTarget = Svc.Objects.Where(x => x.GameObjectId == t->GetGameObjectId().ObjectId).FirstOrDefault();
                    if (uiTarget != null && HasFriendlyTarget(uiTarget)) healTarget = uiTarget;

                    if (restrictToMouseover)
                        return healTarget;
                }

                if (restrictToMouseover)
                    return healTarget;
            }
            healTarget ??= LocalPlayer;
            return healTarget;
        }

        /// <summary> Determines if the enemy is casting an action. Optionally, limit by total cast time. </summary>
        /// <param name="minTotalCast"> The minimum total cast time required, in seconds. </param>
        /// <returns> Bool indicating whether they are casting an action or not. </returns>
        public static bool TargetIsCasting(float? minTotalCast = null)
        {
            if (CurrentTarget is null || CurrentTarget is not IBattleChara chara) return false;

            if (chara.IsCasting) return minTotalCast == null || chara.TotalCastTime >= minTotalCast;

            else return false;
        }

        /// <summary> Determines if the enemy is casting an action that can be interrupted. Optionally, limit by current cast time. </summary>
        /// <param name="minCurrentCast"> The minimum current cast time required, in seconds. </param>
        /// <returns> Bool indicating whether they can be interrupted or not. </returns>
        public static bool CanInterruptEnemy(float? minCurrentCast = null)
        {
            if (CurrentTarget is null || CurrentTarget is not IBattleChara chara) return false;

            if (chara.IsCasting && chara.IsCastInterruptible) return minCurrentCast == null || chara.CurrentCastTime >= minCurrentCast;

            else return false;
        }

        /// <summary> Sets the player's target. </summary>
        /// <param name="target"> Target must be a game object that the player can normally click and target. </param>
        public static void SetTarget(IGameObject? target) => Svc.Targets.Target = target;

        /// <summary> Checks if target is in appropriate range for targeting </summary>
        /// <param name="target"> The target object to check </param>
        public static bool IsInRange(IGameObject? target)
        {
            if (target == null || GetTargetDistance(target, LocalPlayer) >= 30)
                return false;

            return true;
        }

        public static bool TargetNeedsPositionals()
        {
            if (!HasBattleTarget()) return false;
            if (TargetHasEffectAny(3808)) return false; // Directional Disregard Effect (Patch 7.01)
            if (!NPCPositionals.ContainsKey(CurrentTarget.DataId))
            {
                if (Svc.Data.GetExcelSheet<BNpcBase>().TryGetFirst(x => x.RowId == CurrentTarget.DataId, out var bnpc))
                    NPCPositionals[CurrentTarget.DataId] = bnpc.IsOmnidirectional;
            }
            return !NPCPositionals[CurrentTarget.DataId];
        }

        /// <summary> Attempts to target the given party member </summary>
        /// <param name="target"></param>
        protected static unsafe void TargetObject(TargetType target)
        {
            GameObject* t = GetTarget(target);
            if (t == null) return;
            ulong o = PartyTargetingService.GetObjectID(t);
            IGameObject? p = Svc.Objects.Where(x => x.GameObjectId == o).First();

            if (IsInRange(p)) SetTarget(p);
        }

        public static void TargetObject(IGameObject? target)
        {
            if (IsInRange(target)) SetTarget(target);
        }

        public unsafe static GameObject* GetTarget(TargetType target)
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

        /// <summary>
        /// Get angle to target.
        /// </summary>
        /// <returns>Angle relative to target</returns>
        public static float AngleToTarget()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return 0;
            if (CurrentTarget is not IBattleChara || CurrentTarget.ObjectKind != ObjectKind.BattleNpc)
                return 0;

            var angle = PositionalMath.AngleXZ(CurrentTarget.Position, LocalPlayer.Position) - CurrentTarget.Rotation;

            var regionDegrees = PositionalMath.Degrees(angle);
            if (regionDegrees < 0)
            {
                regionDegrees = 360 + regionDegrees;
            }

            if ((regionDegrees >= 45) && (regionDegrees <= 135))
            {
                return 1;
            }
            if ((regionDegrees >= 135) && (regionDegrees <= 225))
            {
                return 2;
            }
            if ((regionDegrees >= 225) && (regionDegrees <= 315))
            {
                return 3;
            }
            if ((regionDegrees >= 315) || (regionDegrees <= 45))
            {
                return 4;
            }
            return 0;
        }

        /// <summary>
        /// Is player on target's rear.
        /// </summary>
        /// <returns>True or false.</returns>
        public static bool OnTargetsRear()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return false;
            if (CurrentTarget is not IBattleChara || CurrentTarget.ObjectKind != ObjectKind.BattleNpc)
                return false;

            var angle = PositionalMath.AngleXZ(CurrentTarget.Position, LocalPlayer.Position) - CurrentTarget.Rotation;

            var regionDegrees = PositionalMath.Degrees(angle);
            if (regionDegrees < 0)
            {
                regionDegrees = 360 + regionDegrees;
            }

            if ((regionDegrees >= 135) && (regionDegrees <= 225))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is player on target's flank.
        /// </summary>
        /// <returns>True or false.</returns>
        public static bool OnTargetsFlank()
        {
            if (CurrentTarget is null || LocalPlayer is null)
                return false;
            if (CurrentTarget is not IBattleChara || CurrentTarget.ObjectKind != ObjectKind.BattleNpc)
                return false;


            var angle = PositionalMath.AngleXZ(CurrentTarget.Position, LocalPlayer.Position) - CurrentTarget.Rotation;

            var regionDegrees = PositionalMath.Degrees(angle);
            if (regionDegrees < 0)
            {
                regionDegrees = 360 + regionDegrees;
            }

            // left flank
            if ((regionDegrees >= 45) && (regionDegrees <= 135))
            {
                return true;
            }
            // right flank
            if ((regionDegrees >= 225) && (regionDegrees <= 315))
            {
                return true;
            }
            return false;
        }

        // the following is all lifted from the excellent Resonant plugin
        internal static class PositionalMath
        {
            static internal float Radians(float degrees)
            {
                return (float)Math.PI * degrees / 180.0f;
            }

            static internal double Degrees(float radians)
            {
                return (180 / Math.PI) * radians;
            }

            static internal float AngleXZ(Vector3 a, Vector3 b)
            {
                return (float)Math.Atan2(b.X - a.X, b.Z - a.Z);
            }
        }

        internal unsafe static bool OutOfRange(uint actionID, IGameObject target) => ActionWatching.OutOfRange(actionID, Svc.ClientState.LocalPlayer!, target);

        public unsafe static bool EnemiesInRange(uint spellCheck)
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

        public unsafe static int NumberOfEnemiesInRange(uint aoeSpell, IGameObject? target)
        {
            ActionWatching.ActionSheet.Values.TryGetFirst(x => x.RowId == aoeSpell, out var sheetSpell);
            bool needsTarget = sheetSpell.CanTargetHostile;

            int count = sheetSpell.CastType switch
            {
                1 => 1,
                2 => sheetSpell.CanTargetSelf ? CanCircleAoe(sheetSpell.EffectRange) : CanRangedCircleAoe(sheetSpell.EffectRange, target),
                3 => CanConeAoe(sheetSpell.EffectRange),
                4 => CanLineAoe(sheetSpell.EffectRange),
                _ => 0
            };

            return count;
        }

        #region Position
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
            return Vector3.Dot(Vector3.Normalize(offsetFromOrigin), direction) >= MathF.Cos(halfAngle);
        }
        public static bool PointInCone(Vector3 offsetFromOrigin, float direction, float halfAngle)
        {
            return PointInCone(offsetFromOrigin, DirectionToVec3(direction), halfAngle);
        }
        #endregion
        #region Point in Rect
        public static bool PointInRect(Vector3 offsetFromOrigin, Vector3 direction, float lenFront, float lenBack, float halfWidth)
        {
            var normal = new Vector3(-direction.Z, 0, direction.X);
            var dotDir = Vector3.Dot(offsetFromOrigin, direction);
            var dotNormal = Vector3.Dot(offsetFromOrigin, normal);
            return dotDir >= -lenBack && dotDir <= lenFront && MathF.Abs(dotNormal) <= halfWidth;
        }

        public static bool PointInRect(Vector3 offsetFromOrigin, float direction, float lenFront, float lenBack, float halfWidth)
        {
            return PointInRect(offsetFromOrigin, DirectionToVec3(direction), lenFront, lenBack, halfWidth);
        }

        public static bool PointInRect(Vector3 offsetFromOrigin, Vector3 startToEnd, float halfWidth)
        {
            var len = startToEnd.Length();
            return PointInRect(offsetFromOrigin, startToEnd / len, len, 0, halfWidth);
        }
        #endregion

        #endregion

        // Circle Aoe
        public static int CanCircleAoe(float effectRange)
        {
            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 PointInCircle(o.Position - LocalPlayer.Position, effectRange + o.HitboxRadius));
        }

        // Ranged Circle Aoe 
        public static int CanRangedCircleAoe(float effectRange, IGameObject? target)
        {
            if (target == null) return 0;
            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 PointInCircle(o.Position - target.Position, effectRange + o.HitboxRadius));
        }

        // Cone Aoe 
        public static int CanConeAoe(float effectRange)
        {
            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 PointInCone(o.Position - LocalPlayer.Position, LocalPlayer.Rotation, 0 + o.HitboxRadius / 2f) &&
                                                                 PointInCircle(o.Position - LocalPlayer.Position, effectRange + o.HitboxRadius));
        }

        // Line Aoe 
        public static int CanLineAoe(float effectRange)
        {
            return Svc.Objects.Count(o => o.ObjectKind == ObjectKind.BattleNpc &&
                                                                 o.IsHostile() &&
                                                                 o.IsTargetable &&
                                                                 PointInRect(o.Position - LocalPlayer.Position, LocalPlayer.Rotation, effectRange, 1, 2));
        }

        internal unsafe static bool IsInLineOfSight(IGameObject target)
        {
            var sourcePos = FFXIVClientStructs.FFXIV.Common.Math.Vector3.Zero;

            if (!Player.Available) return false;

            sourcePos = Player.Object.Struct()->Position;
            sourcePos.Y += 2;

            var targetPos = target.Struct()->Position;
            targetPos.Y += 2;

            var direction = targetPos - sourcePos;
            var distance = direction.Magnitude;

            direction = direction.Normalized;

            Vector3 originVect = new Vector3(sourcePos.X, sourcePos.Y, sourcePos.Z);
            Vector3 directionVect = new Vector3(direction.X, direction.Y, direction.Z);

            RaycastHit hit;
            var flags = stackalloc int[] { 0x4000, 0, 0x4000, 0 };
            var isLoSBlocked = Framework.Instance()->BGCollisionModule->RaycastMaterialFilter(&hit, &originVect, &directionVect, distance, 1, flags);

            return isLoSBlocked == false;
        }

        internal unsafe static bool IsQuestMob(IGameObject target) => target.Struct()->NamePlateIconId is 71204 or 71144 or 71224 or 71344;

        internal unsafe static bool IsBoss(IGameObject target) => Svc.Data.GetExcelSheet<BNpcBase>()?.GetRow(target.DataId).Rank is 2 or 6;
    }
}
