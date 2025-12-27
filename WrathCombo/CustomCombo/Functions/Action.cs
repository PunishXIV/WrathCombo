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
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    public const float BaseActionQueue = 0.5f;
    public const float BaseAnimationLock = 0.6f;

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
    public static string GetActionName(uint actionId) => ActionSheet.TryGetValue(actionId, out var actionSheet)
        ? actionSheet.Name.ToString()
        : "Unknown Action";

    /// <summary> Gets the name of a trait as a string. </summary>
    /// <param name="traitId"> The trait ID. </param>
    public static string GetTraitName(uint traitId) => TraitSheet.TryGetValue(traitId, out var traitSheet)
        ? traitSheet.Name.ToString()
        : "Unknown Trait";

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
        var actionRange = GetActionRange(actionId);

        // Has Range
        // Eg. Geirskogul, Fleche
        if (actionRange > 0f)
            return (optionalTarget ??= CurrentTarget) != null && GetTargetDistance(optionalTarget) <= actionRange;

        var actionRadius = GetActionEffectRange(actionId);

        // Has Radius Only
        // Eg. Dyskrasia, Art of War
        if (actionRadius > 0f)
            return (optionalTarget ??= CurrentTarget) != null && GetTargetDistance(optionalTarget) <= actionRadius;

        // Has Neither
        // Eg. Reassemble, True North
        return true;
    }

    /// <summary> Checks if an action is ready to use based on level required, current cooldown and unlock state. </summary>
    /// <param name="actionId"> The action ID. </param>
    public static unsafe bool ActionReady(uint actionId, bool recastCheck = false, bool castCheck = false)
    {
        uint hookedId = OriginalHook(actionId);

        if(ActionRequestIPCProvider.GetArtificialCooldown(ActionType.Action, hookedId) > 0)
        {
            return false;
        }

        return (HasCharges(hookedId) || (GetAttackType(hookedId) != ActionAttackType.Ability && GetCooldownRemainingTime(hookedId) <= RemainingGCD + BaseActionQueue)) &&
            ActionManager.Instance()->GetActionStatus(ActionType.Action, hookedId, checkRecastActive: recastCheck, checkCastingActive: castCheck) is 0 or 582 or 580;
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
               animationLock <= BaseActionQueue &&                                   // Animation Threshold
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

    private static bool _raidwideInc;
    public static bool RaidWideCasting(float timeRemaining = 0f)
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
                    if (timeRemaining == 0f)
                        return _raidwideInc = true;
                   
                    if ((caster.TotalCastTime - caster.CurrentCastTime) <= timeRemaining)
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
        foreach(uint actionId in actionsToCount)
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
}