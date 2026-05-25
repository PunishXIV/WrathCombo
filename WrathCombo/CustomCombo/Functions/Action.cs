using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.Data;
using WrathCombo.Services;
using WrathCombo.Services.ActionRequestIPC;
using static WrathCombo.Data.ActionWatching;
using static WrathCombo.Window.Text;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    public const float BaseActionQueue = 0.5f;
    public const float BaseAnimationLock = 0.6f;

    public unsafe static float AnimationLock => ActionManager.Instance()->AnimationLock;

    /// <summary> Gets the original hook of an action. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static uint OriginalHook(uint actionId) => Service.ActionReplacer.OriginalHook(actionId);

    /// <summary> Checks if an action matches its original hook. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static bool IsOriginal(uint actionId) => Service.ActionReplacer.OriginalHook(actionId) == actionId;

    /// <summary> Checks if the player has learned an action and is high enough level to use it. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static bool LevelChecked(uint actionId) => LocalPlayer.Level >= GetActionLevel(actionId) && IsActionUnlocked(actionId);

    /// <summary> Checks if the player is high enough level to benefit from a trait. </summary>
    /// <param name="traitId"> The trait ID. </param>
    public static bool TraitLevelChecked(uint traitId) => LocalPlayer.Level >= GetTraitLevel(traitId);

    /// <summary> Gets the minimum level required to use an action. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static int GetActionLevel(uint actionId) => ActionSheet.TryGetValue(actionId, out var actionSheet) && actionSheet.ClassJobCategory.IsValid
        ? actionSheet.ClassJobLevel
        : 255;

    /// <summary> Gets the minimum level required to benefit from a trait. </summary>
    /// <param name="traitId"> The trait ID. </param>
    public static int GetTraitLevel(uint traitId) => TraitSheet.TryGetValue(traitId, out var traitSheet) && traitSheet.ClassJobCategory.IsValid
        ? traitSheet.Level
        : 255;

    /// <summary> Gets the range of an action. </summary>
    /// <param name="actionId"> The action ID. </param>
    public unsafe static float GetActionRange(uint actionId) => ActionManager.GetActionRange(actionId);

    /// <summary> Gets the effect radius of an action. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static float GetActionEffectRange(uint actionId) => ActionSheet.TryGetValue(actionId, out var actionSheet)
        ? actionSheet.EffectRange
        : -1f;

    /// <summary> Gets the cast time of an action. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static float GetActionCastTime(uint actionId) => ActionSheet.TryGetValue(actionId, out var actionSheet)
        ? (actionSheet.Cast100ms + actionSheet.ExtraCastTime100ms) * 0.1f
        : 0f;

    /// <summary> Gets the name of an action as a string. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static string GetActionName(uint actionId) => ActionAndStatusLocalization.GetActionName(actionId);

    /// <summary> Gets the name of a trait as a string. </summary>
    /// <param name="traitId"> The trait ID. </param>
    public static string GetTraitName(uint traitId) => ActionAndStatusLocalization.GetTraitName(traitId);

    /// <summary> Gets the amount of time since an action was used, in seconds. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static float TimeSinceActionUsed(uint actionId) => ActionTimestamps.TryGetValue(actionId, out long timestamp)
        ? (Environment.TickCount64 - timestamp) / 1000f
        : -1f;

    /// <summary> Gets the amount of time since an action was successfully cast, in seconds. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static float TimeSinceLastSuccessfulCast(uint actionId) => LastSuccessfulUseTime.TryGetValue(actionId, out long timestamp)
        ? (Environment.TickCount64 - timestamp) / 1000f
        : -1f;

    /// <summary>
    ///     Checks if the player is within range to use an action. <br/>
    ///     If the action requires a target, defaults to CurrentTarget unless specified.
    /// </summary>
    public static bool InActionRange(uint actionId, IGameObject? optionalTarget = null)
    {
        optionalTarget ??= CurrentTarget;
        var actSheet = ActionSheet[actionId];
        var areaTargeted = actSheet.TargetArea;
        var selfUse = actSheet.CanTargetSelf;
        var hostile = actSheet.CanTargetHostile;
        var actionRange = GetActionRange(actionId);

        // Covers self-use and self-area targeted actions
        if (actionRange == 0)
        {
            var effectRange = GetActionEffectRange(actionId);

            if (effectRange == 0) //Range 0, EffectRange 0 = Purely self use so won't impact a target
                return true;

            // Try and make sure to only call action range on stuff that *can* hit something else, whether it be friendly or hostile
            // since stuff like Leylines technically falls into this area. Don't really want to make an exception list
            return actSheet.CastType switch
            {
                1 => true,
                2 => TargetInSelfCircle(optionalTarget, effectRange),
                3 => TargetInCone(optionalTarget, effectRange), //Weirdly only BLU will meet this condition
                4 => TargetInLine(optionalTarget, effectRange, actSheet.XAxisModifier),
                _ => false
            };
        }

        // If we don't have a target and the action cannot be used on ourselves, we're not in range clearly
        if (optionalTarget is null && !selfUse)
            return false;

        // Deal with actions that don't area target
        if (!areaTargeted)
        {
            unsafe
            {
                if (LocalPlayer is not { } || !IsInLineOfSight(optionalTarget))
                    return false;

                // LocalPlayer is always the source, our target, regardless of hostile/friendly, can be the object to check distance against
                // We should also remember this is just a range check, not a target compatibility check (use (IGameObject).CanUseOn for this) 
                var status = ActionManager.GetActionInRangeOrLoS(actionId, LocalPlayer.GameObject(), optionalTarget.Struct());
                return status is 0 or 565; //0 = no message, 565 = Target is not in range (however this only generates if you're not facing them so it's technically fine with the auto-face setting)
            }
        }

        // Now we deal with area targeted
        // Area targeted generally means things will be placed on the ground, and it's whatever is placed on the ground that will height check
        // As of writing this, only 24 actions players use are area targeted and none of them have any other form of targeting mode, it's strictly on the ground
        // Range = 0 means it's something that can only be placed on the player, otherwise the range indicates how far the action can be placed away from the player
        // Radius indicates how big the effect is once placed on the ground
        // Will have to test if the radius impacts height aswell as the horizontal distance (I think it does), but it will mostly affect PvP if so
        // For the purpose of this, it's mainly range we need to consider as even if the target is within the radius of the placed object, we can only place as far as the range
        // Also, this would have to be managed via retargeting for actually placing elsewhere not on top of the player

        if (GetTargetDistance(optionalTarget) > actionRange || !IsInLineOfSight(optionalTarget))
            return false;

        // At this point the only thing left to even consider is if they're outside the action range, would the target still get hit by the effect range once placed
        // However, that's probably going a bit too far so we'll just consider it as in range as long as the action range is good

        return true;
    }

    /// <summary> Checks if an action is ready to use based on level required, current cooldown and unlock state. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static unsafe bool ActionReady(uint actionId, bool recastCheck = false, bool castCheck = false)
    {
        if (ActionRequestIPCProvider.GetArtificialCooldown(ActionType.Action, actionId) > 0)
        {
            return false;
        }

        return (HasCharges(actionId) || (GetAttackType(actionId) != ActionAttackType.Ability && GetCooldownRemainingTime(actionId) <= RemainingGCD + BaseActionQueue)) &&
            ActionManager.Instance()->GetActionStatus(ActionType.Action, actionId, checkRecastActive: recastCheck, checkCastingActive: castCheck) is 0 or 582 or 580;
    }

    /// <summary> Checks if all passed actions are ready to be used. </summary>
    /// <param name="actionIds"> The action IDs. </param>
    public static bool ActionsReady(uint[] actionIds)
    {
        foreach (var actionId in actionIds)
            if (!ActionReady(actionId)) return false;

        return true;
    }

    /// <summary> Checks if an action was the last action performed. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static bool WasLastAction(uint actionId) => CombatActions.Count > 0 && CombatActions.LastOrDefault() == actionId;

    /// <summary> Checks if an action was the last weaponskill performed. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static bool WasLastWeaponskill(uint actionId) => LastWeaponskill == actionId;

    /// <summary> Checks if an action was the last spell performed. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static bool WasLastSpell(uint actionId) => LastSpell == actionId;

    /// <summary> Checks if an action was the last ability performed. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static bool WasLastAbility(uint actionId) => LastAbility == actionId;

    /// <summary> Gets the amount of times the last action was used in a row. </summary>
    public static int LastActionCounter() => LastActionUseCount;

    /// <summary> Checks if a spell is active in the Blue Mage Spellbook. </summary>
    /// <param name="spellId"> The action ID. </param>
    public static bool IsSpellActive(uint spellId) => Service.Configuration.ActiveBLUSpells.Contains(spellId);

    /// <summary>
    ///     Calculate the best action to use based on cooldown remaining. <br/>
    ///     If there is a tie, the original is used.
    /// </summary>
    /// <param name="original"> The original action. </param>
    /// <param name="actions"> The actions to choose from. </param>
    /// <returns> The appropriate action to use. </returns>
    public static uint CalcBestAction(uint original, params uint[] actions)
    {
        static (uint ActionID, CooldownData Data) Compare(
            uint original,
            (uint ActionID, CooldownData Data) a1,
            (uint ActionID, CooldownData Data) a2)
        {
            // Neither, return the first parameter
            if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
                return original == a1.ActionID ? a1 : a2;

            // Both, return soonest available
            if (a1.Data.IsCooldown && a2.Data.IsCooldown)
            {
                if (a1.Data.HasCharges && a2.Data.HasCharges)
                {
                    if (a1.Data.RemainingCharges == a2.Data.RemainingCharges)
                    {
                        return a1.Data.ChargeCooldownRemaining < a2.Data.ChargeCooldownRemaining
                            ? a1 : a2;
                    }

                    return a1.Data.RemainingCharges > a2.Data.RemainingCharges
                        ? a1 : a2;
                }

                else if (a1.Data.HasCharges)
                {
                    if (a1.Data.RemainingCharges > 0)
                        return a1;

                    return a1.Data.ChargeCooldownRemaining < a2.Data.CooldownRemaining
                        ? a1 : a2;
                }

                else if (a2.Data.HasCharges)
                {
                    if (a2.Data.RemainingCharges > 0)
                        return a2;

                    return a2.Data.ChargeCooldownRemaining < a1.Data.CooldownRemaining
                        ? a2 : a1;
                }

                else
                {
                    return a1.Data.CooldownRemaining < a2.Data.CooldownRemaining
                        ? a1 : a2;
                }
            }

            // One or the other
            return a1.Data.IsCooldown ? a2 : a1;
        }

        static (uint ActionID, CooldownData Data) Selector(uint actionID) => (actionID, GetCooldown(actionID));

        return actions
            .Select(Selector)
            .Aggregate((a1, a2) => Compare(original, a1, a2))
            .ActionID;
    }

    /// <summary> Checks if a certain amount of actions were weaved within the GCD window. </summary>
    public static bool HasWeaved(int weaveAmount = 1) => WeaveActions.Count >= weaveAmount;

    /// <summary> Checks if a specific action was weaved within the GCD window. </summary>
    public static bool HasWeavedAction(uint actionId) => WeaveActions.Contains(actionId);

    /// <summary> Checks if an action can be weaved within the GCD window. </summary>
    /// <param name="estimatedWeaveTime">
    ///     Amount of time required before the GCD is off cooldown.<br/>
    ///     An estimate of how long this oGCD will take.
    /// </param>
    /// <param name="maxWeaves">
    ///     Maximum amount of weaves allowed per window.<br/>
    ///     Defaults to <see cref="Configuration.MaximumWeavesPerWindow"/>.
    /// </param>
    public static unsafe bool CanWeave(float estimatedWeaveTime = BaseAnimationLock, int? maxWeaves = null)
    {
        var player = LocalPlayer;
        var weaveLimit = maxWeaves ?? Service.Configuration.MaximumWeavesPerWindow;
        var remainingCast = player.TotalCastTime - player.CurrentCastTime;
        var animationLock = ActionManager.Instance()->AnimationLock;

        return WeaveActions.Count < weaveLimit &&                                    // Multi-weave Check
               animationLock <= BaseAnimationLock &&                                   // Animation Threshold
               remainingCast <= BaseActionQueue &&                                   // Casting Threshold
               RemainingGCD > (remainingCast + estimatedWeaveTime + animationLock);  // Window End Threshold
    }

    /// <summary> Checks if an action can be weaved within the GCD window, limited by specific GCD thresholds. </summary>
    /// <param name="weaveStart">
    ///     Remaining GCD time when the window starts. <br/>
    ///     Cannot be set higher than half the GCD.
    /// </param>
    /// <param name="weaveEnd">
    ///     Remaining GCD time when the window ends. <br/>
    ///     Defaults to 0.6s unless specified.
    /// </param>
    /// <param name="maxWeaves">
    ///     Maximum amount of weaves allowed per window.<br/>
    ///     Defaults to <see cref="Configuration.MaximumWeavesPerWindow"/>.
    /// </param>
    public static unsafe bool CanDelayedWeave(float weaveStart = 1.25f, float weaveEnd = BaseAnimationLock, int? maxWeaves = null)
    {
        var halfGCD = GCDTotal * 0.5f;
        var remainingGCD = RemainingGCD;
        var weaveLimit = maxWeaves ?? Service.Configuration.MaximumWeavesPerWindow;
        var animationLock = ActionManager.Instance()->AnimationLock;

        return WeaveActions.Count < weaveLimit &&                              // Multi-weave Check
               animationLock <= BaseActionQueue &&                             // Animation Threshold
               remainingGCD > (weaveEnd + animationLock) &&                    // Window End Threshold
               remainingGCD <= (weaveStart > halfGCD ? halfGCD : weaveStart);  // Window Start Threshold
    }

    public enum WeaveTypes
    {
        None,
        Weave,
        DelayWeave
    }
    public static bool CheckWeave(WeaveTypes weave) => weave switch
    {
        WeaveTypes.None => true,
        WeaveTypes.Weave => CanWeave(),
        WeaveTypes.DelayWeave => CanDelayedWeave(),
        _ => false
    };

    /// <summary> Gets the current combo timer. </summary>
    public static unsafe float ComboTimer => ActionManager.Instance()->Combo.Timer;

    /// <summary> Gets the last combo action. </summary>
    public static unsafe uint ComboAction => ActionManager.Instance()->Combo.Action;

    /// <summary> Gets the current limit break action (PvE only). </summary>
    public static unsafe uint LimitBreakAction => Player.Object is null ? 0 : LimitBreakController.Instance()->GetActionId(Player.Object.Character(), (byte)Math.Max(0, LimitBreakLevel - 1));

    /// <summary> Checks if an action can be queued. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static unsafe bool CanQueue(uint actionId)
    {
        var player = LocalPlayer;
        var actionManager = ActionManager.Instance();
        var remainingCast = player.TotalCastTime - player.CurrentCastTime;

        return actionManager->QueuedActionId == 0 &&               // No Action Queued
               actionManager->AnimationLock <= BaseActionQueue &&  // Animation Threshold
               remainingCast <= BaseActionQueue &&                 // Casting Threshold
               ActionReady(actionId);                              // Action Ready
    }

    public static bool GroupDamageIncoming(float? maxTimeRemaining = null) =>
        RaidwideCasting(maxTimeRemaining) ||
        CheckForSharedDamageEffect(out _, out _, 6f);

    public static bool GroupDamageIncoming
        (out bool isMultiHit, float? maxTimeRemaining = null)
    {
        isMultiHit = false;
        return CheckForSharedDamageEffect(out isMultiHit, out _, 6f) ||
               RaidwideCasting(maxTimeRemaining);
    }

    private static bool _raidwideInc;
    public static bool RaidwideCasting(float? maxTimeRemaining = null)
    {
        if (!EzThrottler.Throttle("RaidWideCheck", 100))
            return _raidwideInc;

        foreach (var obj in Svc.Objects)
        {
            if (obj is not IBattleChara caster || !caster.IsHostile() || !caster.IsCasting)
                continue;

            if (ActionSheet.TryGetValue(caster.CastActionId, out var spellSheet))
            {
                if (spellSheet.CastType is 2 or 5 && spellSheet.EffectRange >= 30)
                {
                    if (maxTimeRemaining is null)
                        return _raidwideInc = true;

                    if ((caster.TotalCastTime - caster.CurrentCastTime) <= maxTimeRemaining)
                        return _raidwideInc = true;
                }
            }
        }

        return _raidwideInc = false;
    }

    private static bool _beingTargetedHostile;
    public static bool BeingTargetedHostile
    {
        get
        {
            if (!EzThrottler.Throttle("BeingTargetedHostile", 100))
                return _beingTargetedHostile;

            return _beingTargetedHostile = Svc.Objects.Any(x => x is IBattleChara chara && chara.IsHostile() && chara.CastTargetObjectId == LocalPlayer.GameObjectId);
        }
    }

    /// <summary> Gets how many times an action has been used since combat started. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static int ActionCount(uint actionId) => CombatActions.Count(x => x == OriginalHook(actionId));

    /// <summary> Gets how many times multiple actions have been used since combat started. </summary>
    /// <param name="actionIds"> The action IDs. </param>
    public static int ActionCount(uint[] actionIds)
    {
        int useCount = 0;
        foreach (var actionId in actionIds)
            useCount += ActionCount(actionId);

        return useCount;
    }

    /// <summary> Gets how many times an action was used since using another action. </summary>
    /// <param name="actionToCheckAgainst"> The action to check against. </param>
    /// <param name="actionToCount"> The action to count. </param>
    public static int TimesUsedSinceOtherAction(uint actionToCheckAgainst, uint actionToCount)
    {
        if (CombatActions.Count == 0)
            return 0;

        int useCount = 0;
        for (int i = CombatActions.Count - 1; i >= 0; i--)
        {
            var action = CombatActions[i];
            if (action == actionToCheckAgainst)
            {
                return useCount;
            }
            if (action == actionToCount)
            {
                useCount++;
            }
        }

        return 0;
    }

    /// <summary> Gets how many times multiple actions were used since using another action. </summary>
    /// <param name="actionToCheckAgainst"> The action to check against. </param>
    /// <param name="actionsToCount"> The actions to count.</param>
    public static int TimesUsedSinceOtherAction(uint actionToCheckAgainst, uint[] actionsToCount)
    {
        int useCount = 0;
        foreach (uint actionId in actionsToCount)
        {
            useCount += TimesUsedSinceOtherAction(actionToCheckAgainst, actionId);
        }

        return useCount;
    }

    /// <summary> Gets the most recently performed action from a list of actions. </summary>
    /// <param name="actionIds"> The action IDs. </param>
    public static uint WhichActionWasLast(params uint[] actionIds)
    {
        if (CombatActions.Count == 0)
            return 0;

        var actionsToCheck = new HashSet<uint>(actionIds);
        for (int i = CombatActions.Count - 1; i >= 0; i--)
        {
            var action = CombatActions[i];
            if (actionsToCheck.Contains(action))
                return action;
        }

        return 0;
    }

    public static bool ActionIsFriendly(uint actionId) => ActionSheet.TryGetValue(actionId, out var s) && s.Unknown4 == 2;
}