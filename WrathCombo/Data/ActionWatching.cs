using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Services.ActionRequestIPC;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.ActionEffectHandler;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Action = Lumina.Excel.Sheets.Action;
namespace WrathCombo.Data;

public static class ActionWatching
{
    // Dictionaries
    internal static readonly FrozenDictionary<uint, BNpcBase> BNPCSheet =
        Svc.Data.GetExcelSheet<BNpcBase>()!
            .ToFrozenDictionary(i => i.RowId);

    internal static readonly FrozenDictionary<uint, Action> ActionSheet =
        Svc.Data.GetExcelSheet<Action>()!
            .ToFrozenDictionary(i => i.RowId);

    internal static readonly FrozenDictionary<uint, Trait> TraitSheet =
        Svc.Data.GetExcelSheet<Trait>()!
            .ToFrozenDictionary(i => i.RowId);

    internal static readonly Dictionary<uint, long> ActionTimestamps = [];
    internal static readonly Dictionary<uint, long> LastSuccessfulUseTime = [];
    internal static readonly Dictionary<(uint, ulong), long> UsedOnDict = [];

    // Lists
    internal readonly static List<uint> WeaveActions = [];
    internal readonly static List<uint> CombatActions = [];
    internal readonly static HashSet<uint> BossesBaseIds = [.. Svc.Data.GetExcelSheet<BNpcBase>().Where(charaSheet => charaSheet.Rank is 2 or 6).Select(charaSheet => charaSheet.RowId)];

    // Delegates
    public delegate void LastActionChangeDelegate();
    public static event LastActionChangeDelegate? OnLastActionChange;

    public delegate void ActionSendDelegate();
    public static event ActionSendDelegate? OnActionSend;

    private unsafe delegate void ReceiveActionEffectDelegate(uint casterEntityId, Character* casterPtr, Vector3* targetPos, Header* header, TargetEffects* effects, GameObjectId* targetEntityIds);
    private readonly static Hook<ReceiveActionEffectDelegate>? ReceiveActionEffectHook;

    private unsafe delegate bool UseActionDelegate(ActionManager* actionManager, ActionType actionType, uint actionId, ulong targetId, uint extraParam, ActionManager.UseActionMode mode, uint comboRouteId, bool* outOptAreaTargeted);
    private readonly static Hook<UseActionDelegate>? UseActionHook;

    private delegate void SendActionDelegate(ulong targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
    private static readonly Hook<SendActionDelegate>? SendActionHook;

    private static Task UpdateActionTask = null!;
    private static CancellationTokenSource source = new CancellationTokenSource();
    private static CancellationToken token;

    public static bool UpdatingActions;

    /// <summary> Handles logic when an action causes an effect. </summary>
    private unsafe static void ReceiveActionEffectDetour(uint casterEntityId, Character* casterPtr, Vector3* targetPos, Header* header, TargetEffects* effects, GameObjectId* targetEntityIds)
    {
        ReceiveActionEffectHook!.Original(casterEntityId, casterPtr, targetPos, header, effects, targetEntityIds);

        try
        {
            // Cache Data
            var dateNow = DateTime.Now;
            var actionId = header->ActionId;
            var actionType = header->ActionType;
            var currentTick = Environment.TickCount64;
            var playerObjectId = LocalPlayer.GameObjectId;
            var partyMembers = GetPartyMembers().ToDictionary(x => x.GameObjectId);
#if DEBUG
            var debugObjectTable = Svc.Objects;
            var debugActionName = actionId.ActionName();
#endif

            // Process Effects
            int numTargets = header->NumTargets;
            var targets = new List<(ulong id, ActionEffects effects)>(numTargets);
            var effectBlocks = (ActionEffects*)effects;
            for (int i = 0; i < numTargets; ++i)
            {
                targets.Add((targetEntityIds[i], effectBlocks[i]));
            }

            // Process Targets
            foreach (var target in targets)
            {
                // Cache Data
                var targetId = target.id;
#if DEBUG
                var debugTargetName = debugObjectTable.SearchById(targetId)?.Name ?? "Unknown";
#endif

                foreach (var eff in target.effects)
                {
                    // Cache Data
                    var effType = eff.Type;
                    var effValue = eff.Value;
                    var effObjectId = eff.AtSource ? casterEntityId : targetId;
                    var dataId = Svc.Objects.FirstOrDefault(x => x.GameObjectId == effObjectId)?.BaseId;

#if DEBUG
                    Svc.Log.Verbose(
                        $"[ReceiveActionEffectDetour] " +
                        $"Type: {effType} | " +
                        $"Value: {effValue} | " +
                        $"Params: [{eff.Param0}, {eff.Param1}, {eff.Param2}, {eff.Param3}, {eff.Param4}] | " +
                        $"Action: {debugActionName} (ID: {actionId}) → " +
                        $"Target: {debugTargetName} | " +
                        $"Flags: [AtSource: {eff.AtSource}, FromTarget: {eff.FromTarget}]"
                    );
#endif

                    // Event: Heal or Damage
                    if (effType is ActionEffectType.Heal or ActionEffectType.Damage)
                    {
                        if (partyMembers.TryGetValue(targetId, out var member))
                        {
                            member.CurrentHP = effType == ActionEffectType.Damage
                                ? Math.Min(member.BattleChara.MaxHp, member.CurrentHP - effValue)
                                : Math.Min(member.BattleChara.MaxHp, member.CurrentHP + effValue);

                            member.HPUpdatePending = true;
                            Svc.Framework.RunOnTick(() => member.HPUpdatePending = false, TimeSpan.FromSeconds(1.5));
                        }
                    }

                    // Event: MP Gain or MP Loss
                    if (effType is ActionEffectType.MpGain or ActionEffectType.MpLoss)
                    {
                        if (partyMembers.TryGetValue(effObjectId, out var member))
                        {
                            member.CurrentMP = effType == ActionEffectType.MpLoss
                                ? Math.Min(member.BattleChara.MaxMp, member.CurrentMP - effValue)
                                : Math.Min(member.BattleChara.MaxMp, member.CurrentMP + effValue);

                            member.MPUpdatePending = true;
                            Svc.Framework.RunOnTick(() => member.MPUpdatePending = false, TimeSpan.FromSeconds(1.5));
                        }
                    }

                    // Event: Status Gain (Source)
                    if (effType is ActionEffectType.ApplyStatusEffectSource)
                    {
                        if (partyMembers.TryGetValue(effObjectId, out var member))
                        {
                            member.BuffsGainedAt[effValue] = currentTick;
                        }
                    }

                    // Event: Status Gain (Target)
                    if (effType is ActionEffectType.ApplyStatusEffectTarget)
                    {
                        if (ICDTracker.Trackers.TryGetFirst(x => x.StatusID == effValue && x.GameObjectId == effObjectId, out var icd))
                        {
                            //This section here is just to clear out any erroneous times a status was added to the blacklist when it shouldn't have due to potential timings
                            if (dataId is uint val && Service.Configuration.StatusBlacklist.Any(x => x.Status == effValue && x.BaseId == dataId))
                            {
                                var p = Service.Configuration.StatusBlacklist.First(x => x.Status == effValue && x.BaseId == dataId);
                                Service.Configuration.StatusBlacklist.Remove(p);
                                Service.Configuration.Save();
                            }
                            icd.ICDClearedTime = dateNow + TimeSpan.FromSeconds(60);
                            icd.TimesApplied += 1;
                        }
                        else ICDTracker.Trackers.Add(new(effValue, effObjectId, TimeSpan.FromSeconds(60)));
                    }

                    if (effType is ActionEffectType.FullResistStatus)
                    {
                        if (!ICDTracker.Trackers.Any(x => x.StatusID == effValue && x.GameObjectId == effObjectId))
                        {
                            if (dataId is uint val)
                            {
                                Service.Configuration.StatusBlacklist.Add((effValue, val));
                                Service.Configuration.Save();
                            }
                        }
                    }
                }
            }

            if (casterEntityId == Player.Object.EntityId && ActionSheet.TryGetValue(actionId, out var actionSheet) && actionSheet.TargetArea)
            {
                UpdateLastUsedAction(actionId, 1, 0, 0);
            }

        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "ReceiveActionEffectDetour");
        }
    }

    private static unsafe void UpdateLastUsedAction(uint actionId, byte actionType, ulong targetObjectId, int castTime)
    {
        // Update Trackers
        LastAction = actionId;
        TimeLastActionUsed = DateTime.Now;
        var currentTick = Environment.TickCount64;

        // Update Counter
        if (actionId != CombatActions.LastOrDefault())
            LastActionUseCount = 1;
        else
            LastActionUseCount++;

        // Update Lists
        CombatActions.Add(actionId);
        LastSuccessfulUseTime[actionId] = currentTick;
        if (ActionSheet.TryGetValue(actionId, out var actionSheet))
        {
            switch (actionSheet.ActionCategory.Value.RowId)
            {
                case 2: // Spell
                    LastSpell = actionId;
                    WeaveActions.Clear();
                    break;

                case 3: // Weaponskill
                    LastWeaponskill = actionId;
                    WeaveActions.Clear();
                    break;

                case 4: // Ability
                    LastAbility = actionId;
                    WeaveActions.Add(actionId);
                    break;
            }

            if (actionType == 1)
            {
                ActionTimestamps[actionId] = currentTick;
                UsedOnDict[(actionId, targetObjectId)] = currentTick;
            }

            if (actionSheet.Unknown4 != 1 && castTime > 0)
                AutoRotationController.HealThrottle = Environment.TickCount64 + 1000;
        }

        if (castTime == 0)
            WrathOpener.CurrentOpener?.ProgressOpener(actionId);

        if (Service.Configuration.EnabledOutputLog)
            OutputLog();

        UpdatingActions = false;
    }

    /// <summary> Handles logic when an action is sent. </summary>
    private unsafe static void SendActionDetour(ulong targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
    {
        try
        {
            if(P.IPC.OnActionUsedProvider.SubscriptionCount > 0)
            {
                P.IPC.OnActionUsedProvider.SendMessage((ActionType)actionType, actionId);
            }
            if (actionType is 1)
            {
                OnActionSend?.Invoke();

                if (!InCombat())
                {
                    CombatActions.Clear();
                    WeaveActions.Clear();
                }

                var castTime = ActionManager.GetAdjustedCastTime((ActionType)actionType, actionId);
                token = source.Token;
                UpdatingActions = true;
                UpdateActionTask = Svc.Framework.RunOnTick(() =>
                UpdateLastUsedAction(actionId, actionType, targetObjectId, castTime),
                TimeSpan.FromMilliseconds(castTime), cancellationToken: token);

                // Update Helpers
                NIN.InMudra = NIN.MudraSigns.Contains(actionId);

                if (castTime > 0)
                {
                    TimeLastActionUsed = DateTime.Now;
                    WrathOpener.CurrentOpener?.ProgressOpener(actionId);
                }

#if DEBUG
                Svc.Log.Verbose(
                    $"[SendActionDetour] " +
                    $"Action: {actionId.ActionName()} (ID: {actionId}) | " +
                    $"Type: {actionType} | " +
                    $"Sequence: {sequence} | " +
                    $"Target: {Svc.Objects.SearchById(targetObjectId)?.Name ?? "Unknown"} | " +
                    $"Params: [{a5}, {a6}, {a7}, {a8}, {a9}]"
                );
#endif
            }
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "SendActionDetour");
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
        }
    }

    /// <summary> Gets the amount of GCDs used since combat started. </summary>
    public static int NumberOfGcdsUsed => CombatActions.Count(x => x.ActionAttackType() is ActionAttackType.Spell or ActionAttackType.Weaponskill);

    private static uint _lastAction = 0;
    public static uint LastAction
    {
        get => _lastAction;
        set
        {
            if (_lastAction != value)
            {
                OnLastActionChange?.Invoke();
                _lastAction = value;
            }
        }
    }
    public static int LastActionUseCount { get; set; } = 0;
    public static int LastActionType { get; set; } = 0;
    public static uint LastWeaponskill { get; set; } = 0;
    public static uint LastAbility { get; set; } = 0;
    public static uint LastSpell { get; set; } = 0;

    public static TimeSpan TimeSinceLastAction => DateTime.Now - TimeLastActionUsed;
    public static DateTime TimeLastActionUsed { get; set; } = DateTime.Now;

    public static void OutputLog()
    {
        DuoLog.Information($"You just used: {CombatActions.LastOrDefault().ActionName()} x{LastActionUseCount}");
    }

    public static void Dispose()
    {
        Disable();
        ReceiveActionEffectHook?.Dispose();
        SendActionHook?.Dispose();
        UseActionHook?.Dispose();
        OnCastInterrupted -= CancelPendingLastActionUpdate;
    }

    static unsafe ActionWatching()
    {
        ReceiveActionEffectHook ??= Svc.Hook.HookFromAddress<ReceiveActionEffectDelegate>(Addresses.Receive.Value, ReceiveActionEffectDetour);
        SendActionHook ??= Svc.Hook.HookFromSignature<SendActionDelegate>("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B E9 41 0F B7 D9", SendActionDetour);
        UseActionHook ??= Svc.Hook.HookFromAddress<UseActionDelegate>(ActionManager.Addresses.UseAction.Value, UseActionDetour);
        OnCastInterrupted += CancelPendingLastActionUpdate;
    }

    private static void CancelPendingLastActionUpdate(uint interruptedAction)
    {
        source.Cancel();
        source = new CancellationTokenSource();
        UpdatingActions = false;
    }

    /// <summary> Handles logic when an action is used. </summary>
    private unsafe static bool UseActionDetour(ActionManager* actionManager, ActionType actionType, uint actionId, ulong targetId, uint extraParam, ActionManager.UseActionMode mode, uint comboRouteId, bool* outOptAreaTargeted)
    {
        try
        {

            if (actionType is ActionType.Action)
            {
                var original = actionId; //Save the original action, do not modify
                var originalTargetId = targetId; //Save the original target, do not modify

                var modifiedAction = Service.ActionReplacer.LastActionInvokeFor.ContainsKey(actionId) ? Service.ActionReplacer.LastActionInvokeFor[actionId] : actionId;
                var changed = CheckForChangedTarget(original, ref targetId,
                    out var replacedWith); //Passes the original action to the retargeting framework, outputs a targetId and a replaced action

                var areaTargeted = ActionSheet[replacedWith].TargetArea;
                var targetObject = targetId.GetObject();

                if (changed && !areaTargeted) //Check if the action can be used on the target, and if not revert to original
                    if (!ActionManager.CanUseActionOnTarget(replacedWith,
                        targetObject.Struct()))
                        targetId = originalTargetId;

                // Support Retargeted ground actions
                if ((changed && areaTargeted) || AutoRotationController.WouldLikeToGroundTarget)
                {
                    var location = Player.Position;

                    if (IsOverGround(targetObject) &&
                        Vector3.Distance(Player.Position, targetObject.Position) <= replacedWith.ActionRange()) // not GetTargetDistance or something, as hitboxes should not count here
                        location = targetObject.Position;
                    else if (TryGetNearestGroundPointWithinRange(
                                 targetObject, out var newLoc,
                                 replacedWith.ActionRange()) &&
                             newLoc is not null)
                        location = (Vector3)newLoc;

                    return ActionManager.Instance()->UseActionLocation
                        (actionType, replacedWith, location: &location);
                }

                //Important to pass actionId here and not replaced.
                var hookResult = changed ? UseActionHook.Original(actionManager, actionType, actionId, targetId, extraParam, mode, comboRouteId, outOptAreaTargeted) :
                    UseActionHook.Original(actionManager, actionType, actionId, originalTargetId, extraParam, mode, comboRouteId, outOptAreaTargeted);

                // Fallback if the Retargeted ground action couldn't be placed smartly
                if (changed && areaTargeted)
                    ActionManager.Instance()->AreaTargetingExecuteAtObject =
                        targetId;

                // This really only works if no other plugin is forcing these values to be any different than vanilla for whatever reason
                // Hookresult should only return true when an action is actually used, or when it gets queued
                // So part 2 just makes sure it's returning true only when it's not being queued
                var success = hookResult && !(mode == ActionManager.UseActionMode.None && actionManager->QueuedActionId > 0);

                //if (success)
                //{
                //    if (NIN.MudraSigns.Contains(modifiedAction))
                //    {
                //        Svc.Log.Debug($"Mudra used: {modifiedAction.ActionName()}");
                //        NIN.InMudra = true;
                //    }
                //    var castTime = ActionManager.GetAdjustedCastTime(actionType, modifiedAction);
                //    LastAction = modifiedAction;
                //    TimeLastActionUsed = DateTime.Now;
                //}

                return hookResult;
            }
            else
            {
                return UseActionHook.Original(actionManager, actionType, actionId, targetId, extraParam, mode, comboRouteId, outOptAreaTargeted);
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "UseActionDetour");
            return UseActionHook.Original(actionManager, actionType, actionId, targetId, extraParam, mode, comboRouteId, outOptAreaTargeted);
        }
    }

    public static bool CheckForChangedTarget(uint actionId, ref ulong targetObjectId, out uint replacedWith)
    {
        replacedWith = actionId;
        if (!P.ActionRetargeting.TryGetTargetFor(actionId, out var target, out replacedWith) ||
            target is null)
            return false;

        if (actionId == OccultCrescent.Revive)
        {
            target = SimpleTarget.Stack.AllyToRaise;
            if (target is null) return false;
        }

        targetObjectId = target.GameObjectId;
        return true;
    }

    public static void Enable()
    {
        ReceiveActionEffectHook?.Enable();
        SendActionHook?.Enable();
        UseActionHook?.Enable();
        Svc.Condition.ConditionChange += ResetActions;
    }

    private static void ResetActions(ConditionFlag flag, bool value)
    {
        if (flag == ConditionFlag.InCombat && !value)
        {
            CombatActions.Clear();
            WeaveActions.Clear();
            ActionTimestamps.Clear();
            LastAbility = 0;
            LastAction = 0;
            LastWeaponskill = 0;
            LastSpell = 0;
            UsedOnDict.Clear();
        }
    }

    public static void Disable()
    {
        ReceiveActionEffectHook.Disable();
        SendActionHook?.Disable();
        UseActionHook?.Disable();
        Svc.Condition.ConditionChange -= ResetActions;
    }

    [Obsolete("Use CustomComboFunctions.GetActionName instead. This method will be removed in a future update.")]
    public static string GetActionName(uint id) => CustomComboFunctions.GetActionName(id);

    public static unsafe bool OutOfRange(uint actionId, IGameObject source, IGameObject target)
    {
        return ActionManager.GetActionInRangeOrLoS(actionId, source.Struct(), target.Struct()) is 566;
    }

    public static string GetBLUIndex(uint id)
    {
        var aozKey = Svc.Data.GetExcelSheet<AozAction>()!.First(x => x.Action.RowId == id).RowId;
        var index = Svc.Data.GetExcelSheet<AozActionTransient>().GetRow(aozKey).Number;

        return $"#{index} ";
    }

    public static ActionAttackType GetAttackType(uint actionId)
    {
        if (!ActionSheet.TryGetValue(actionId, out var actionSheet))
            return ActionAttackType.Unknown;

        return Enum.IsDefined(typeof(ActionAttackType), actionSheet.ActionCategory.RowId)
                ? (ActionAttackType)actionSheet.ActionCategory.RowId
                : ActionAttackType.Unknown;
    }

    public enum ActionAttackType : uint
    {
        Unknown = 0,
        Spell = 2,
        Weaponskill = 3,
        Ability = 4,
    }
}