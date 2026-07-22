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
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.ActionEffectHandler;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using Action = Lumina.Excel.Sheets.Action;
using Task = System.Threading.Tasks.Task;
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
    internal readonly static List<PendingHPChange> PendingHPChanges = [];

    // Delegates
    public delegate void LastActionChangeDelegate();
    public static event LastActionChangeDelegate? OnLastActionChange;

    public delegate void ActionSendDelegate();
    public static event ActionSendDelegate? OnActionSend;

    private readonly static Hook<Delegates.Receive>? ReceiveActionEffectHook;
    private readonly static Hook<ActionManager.Delegates.UseAction>? UseActionHook;
    private readonly static Hook<ActionManager.Delegates.UseActionLocation>? UseActionLocHook;

    private delegate void SendActionDelegate(ulong targetObjectId, ActionType actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
    private static readonly Hook<SendActionDelegate>? SendActionHook;
    public static readonly Hook<ActionManager.Delegates.IsActionOffCooldown> CanQueueAction;
    public static readonly Hook<PacketDispatcher.Delegates.HandleActorControlPacket> ActorControlPacketHook;
    public static readonly Hook<PacketDispatcher.Delegates.OnReceivePacket> OnRecievePacketHook;

    private static Task UpdateActionTask = null!;
    private static CancellationTokenSource source = new CancellationTokenSource();
    private static CancellationToken token;

    public static bool UpdatingActions;
    private static bool _tainted;

    static unsafe ActionWatching()
    {
        ReceiveActionEffectHook ??= Svc.Hook.HookFromAddress<Delegates.Receive>(Addresses.Receive.Value, ReceiveActionEffectDetour);
        SendActionHook ??= Svc.Hook.HookFromSignature<SendActionDelegate>("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B E9 41 0F B7 D9", SendActionDetour);
        UseActionHook ??= Svc.Hook.HookFromAddress<ActionManager.Delegates.UseAction>(ActionManager.Addresses.UseAction.Value, UseActionDetour);
        UseActionLocHook ??= Svc.Hook.HookFromAddress<ActionManager.Delegates.UseActionLocation>(ActionManager.Addresses.UseActionLocation.Value, UseActionLocationDetour);
        CanQueueAction ??= Svc.Hook.HookFromAddress<ActionManager.Delegates.IsActionOffCooldown>(ActionManager.Addresses.IsActionOffCooldown.Value, CanQueueActionDetour);
        ActorControlPacketHook ??= Svc.Hook.HookFromAddress<PacketDispatcher.Delegates.HandleActorControlPacket>(PacketDispatcher.Addresses.HandleActorControlPacket.Value, ActorControlDetour);
        OnRecievePacketHook ??= Svc.Hook.HookFromAddress<PacketDispatcher.Delegates.OnReceivePacket>((nint)PacketDispatcher.StaticVirtualTablePointer->OnReceivePacket, OnReceivePacketDetour);
        OnCastInterrupted += CancelPendingLastActionUpdate;

    }

    private static unsafe void OnReceivePacketDetour(PacketDispatcher* thisPtr, uint targetId, nint packet)
    {
        OnRecievePacketHook.Original(thisPtr, targetId, packet);
        var opCode = *(ushort*)(packet + 2);
        var tar = ((ulong)targetId).GetObject();

        if (Service.Configuration.OpCodes is { } codes && codes.GameVersion == Framework.Instance()->GameVersionString)
        {
            var opCodeName = "";
            try
            {
                opCodeName = Service.Configuration.OpCodesBackup.First(x => x.Version == codes.RetailVersion).Lists.ServerZoneIpcType.First(x => x.Opcode == opCode).Name;
            }
            catch { }

            if (!string.IsNullOrEmpty(opCodeName))
                Svc.Log.Verbose($"[OpCodeVerboseAf] Found {opCodeName} on {tar?.Name}");

            if (opCodeName == "UpdateHpMpTp")
            {
                var newHealth = *(uint*)(packet + 16);
                var newMp = *(ushort*)(packet + 20);
                Svc.Log.Verbose($"[OpCode] Natty Regen on {tar?.Name} with new health {newHealth} and MP {newMp}");
                SimpleTargetState.UpdateNaturalRegenTick(targetId, newHealth);
            }

            if (opCodeName is "EffectResult" or "EffectResultBasic")
            {
                var newHealth = *(uint*)(packet + 28);
                var globalSequence = *(uint*)(packet + 20);
                var val = *(uint*)(packet + 16);
                Svc.Log.Verbose($"[OpCode] Effect Resolved on {tar?.Name} with GS {globalSequence}.");
                PendingHPChanges.RemoveAll(x => x.globalSequence == globalSequence);
                SimpleTargetState.UpdateNaturalRegenTick(targetId, newHealth);
            }

            if (opCodeName is "Effect")
            {
                //This is an interesting one, for heals you get this right away but if the heal does not actually change HP it
                //doesn't get resolved above so maybe worth just timing these out after 1.5s if not resolved
                var globalSequence = *(uint*)(packet + 28);
                Svc.Framework.RunOnTick(() => PendingHPChanges.RemoveAll(x => x.globalSequence == globalSequence), TimeSpan.FromSeconds(1.5f));
            }
        }
    }

    private static void ActorControlDetour(uint entityId, uint category, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, uint arg6, uint arg7, uint arg8, GameObjectId targetId, bool isRecorded)
    {
        Svc.Log.Verbose($"[ActorControl] {entityId} {category} {arg1} {arg2} {arg3} {arg4} {arg5} {arg6} {arg7} {arg8} {targetId.Id} {isRecorded}");
        ActorControlPacketHook.Original(entityId, category, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, targetId, isRecorded);

        if (category == 1541) // Dots
            SimpleTargetState.UpdatePeriodicHealthChange(entityId, arg2, true);

        if (category == 1540) // Hots
            SimpleTargetState.UpdatePeriodicHealthChange(entityId, arg2, false);

        if (category == 4 && arg1 == 0)
            SimpleTargetState.RemoveDueToDroppedCombat(entityId);

    }

    private static unsafe bool UseActionLocationDetour(ActionManager* thisPtr, ActionType actionType, uint actionId, ulong targetId, Vector3* location, uint extraParam, byte a7)
    {
        // TODO Revisit maybe
        //if (actionType == ActionType.Action && !_tainted && !(location->X != 0 || location->Y != 0 || location->Z != 0))
        //{
        //    var obj = targetId.GetObject();
        //    if (!obj.CanUseOn(actionId) && !Svc.Data.GetExcelSheet<Action>().GetRow(actionId).CanTargetSelf)
        //    {
        //        _tainted = true;
        //        if (CurrentTarget?.CanUseOn(actionId) == true)
        //            targetId = CurrentTarget.ObjectId;
        //        else if (Player.Object?.CanUseOn(actionId) == true)
        //            targetId = Player.Object.ObjectId;

        //        if (targetId.GetObject() is { } validObj)
        //            DuoLog.Error($"{actionId.ActionName()} has been attempted to be used on an invalid target, likely due to using another rotation plugin. We have updated the target to {validObj.Name} which it can be used on. Please ensure you are only using one rotation plugin for correct behaviour.");
        //        else
        //            DuoLog.Error($"{actionId.ActionName()} has been attempted to be used on an invalid target, likely due to using another rotation plugin. We are unable to redirect this to a valid target. Please ensure you are only using one rotation plugin for correct behaviour.");

        //        Svc.Framework.RunOnTick(() => _tainted = false, TimeSpan.FromSeconds(2));
        //    }
        //}
        return UseActionLocHook.Original(thisPtr, actionType, actionId, targetId, location, extraParam, a7);
    }

    public static void Enable()
    {
        ReceiveActionEffectHook?.Enable();
        SendActionHook?.Enable();
        UseActionHook?.Enable();
        UseActionLocHook?.Enable();
        CanQueueAction?.Enable();
        ActorControlPacketHook?.Enable();
        OnRecievePacketHook?.Enable();
        Svc.Condition.ConditionChange += ResetActions;
    }


    public static void Dispose()
    {
        Disable();
        ReceiveActionEffectHook?.Dispose();
        SendActionHook?.Dispose();
        UseActionHook?.Dispose();
        UseActionLocHook?.Dispose();
        CanQueueAction?.Dispose();
        ActorControlPacketHook?.Dispose();
        OnRecievePacketHook?.Dispose();
        OnCastInterrupted -= CancelPendingLastActionUpdate;
    }

    /// <summary> Handles logic when an action causes an effect. </summary>
    private unsafe static void ReceiveActionEffectDetour(uint casterEntityId, Character* casterPtr, Vector3* targetPos, Header* header, TargetEffects* effects, GameObjectId* targetEntityIds)
    {
        ReceiveActionEffectHook!.Original(casterEntityId, casterPtr, targetPos, header, effects, targetEntityIds);

        try
        {
            // Cache Data
            var dateNow = DateTime.Now;
            var actionId = header->ActionId;
            var actionType = (ActionType)header->ActionType;
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
                        $"Damage HealValue: {eff.DamageHealValue} | " +
                        $"Action: {debugActionName} (ID: {actionId}) → " +
                        $"Target: {debugTargetName} ({targetId}) | " +
                        $"[AtSource: {eff.AtSource}, FromTarget: {eff.FromTarget}] | " +
                        $"Flags: {header->Flags}"
                    );
#endif

                    // Event: Heal or Damage
                    if (effType is ActionEffectType.Heal or ActionEffectType.Damage)
                    {
                        if (partyMembers.TryGetValue(targetId, out var member))
                        {
                            member.CurrentHP = (uint)(effType == ActionEffectType.Damage
                                ? Math.Min(member.BattleChara.MaxHp, member.CurrentHP - eff.DamageHealValue)
                                : Math.Min(member.BattleChara.MaxHp, member.CurrentHP + eff.DamageHealValue));

                            member.HPUpdatePending = true;
                            Svc.Framework.RunOnTick(() => member.HPUpdatePending = false, TimeSpan.FromSeconds(1.5));
                        }

                        if (Service.Configuration.OpCodes is { } codes && codes.GameVersion == Framework.Instance()->GameVersionString)
                            PendingHPChanges.Add(new PendingHPChange(effObjectId, eff.DamageHealValue, effType == ActionEffectType.Heal, header->GlobalSequence));
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

            if ((actionType == ActionType.Action && casterEntityId == Player.Object.EntityId && ActionSheet.TryGetValue(actionId, out var actionSheet) && actionSheet.TargetArea) || actionType == ActionType.Item)
            {
                UpdateLastUsedAction(actionId, actionType, 0, 0);
            }

        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "ReceiveActionEffectDetour");
        }
    }

    private static unsafe void UpdateLastUsedAction(uint actionId, ActionType actionType, ulong targetObjectId, int castTime)
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

            if (actionType == ActionType.Action)
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
            OutputLog(actionType);

        if (AutoRotationController.AutorotRaidwiding && AutoRotationController.RaidwideActions.Any(x => x.Action == actionId))
        {
            AutoRotationController.BlacklistedRaidwides.Add(actionId);
            if (actionId != SGE.Eukrasia)
                AutoRotationController.AutorotRaidwides++;
        }

        UpdatingActions = false;
    }

    /// <summary> Handles logic when an action is sent. </summary>
    private unsafe static void SendActionDetour(ulong targetObjectId, ActionType actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
    {
        try
        {
            if (P.IPC.OnActionUsedProvider.SubscriptionCount > 0)
            {
                P.IPC.OnActionUsedProvider.SendMessage(actionType, actionId);
            }

            if (actionType is ActionType.Item)
                WrathOpener.CurrentOpener?.ProgressOpener(actionId, true);

            if (actionType is ActionType.Action)
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
                UpdateLastUsedAction(actionId, actionType, targetObjectId, Math.Max(castTime - 480, 0)),
                TimeSpan.FromMilliseconds(Math.Max(castTime - 480, 0)), cancellationToken: token);

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

            OverrideTarget = null;
            AutoRotationController.AutorotHealTarget = null;
            Service.ActionReplacer.EnableActionReplacingIfRequired();
        }
        catch (Exception ex)
        {
            Service.ActionReplacer.EnableActionReplacingIfRequired();
            Svc.Log.Error(ex, "SendActionDetour");
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
        }
    }

    public unsafe static bool CanQueueCS(uint actionId) => CanQueueActionDetour(ActionManager.Instance(), ActionType.Action, actionId);

    private static unsafe bool CanQueueActionDetour(ActionManager* actionManager, ActionType actionType, uint actionID)
    {
        //if (NIN.InMudra && NIN.MudraSigns.Any(x => x == actionID) && NIN.MudraToBase(LastAction) == NIN.MudraToBase(actionID)) return false;

        float threshold = Service.Configuration.QueueAdjust ? Service.Configuration.QueueAdjustThreshold : 0.5f;

        return GetRemainingActionRecast(actionManager, actionType, actionID) is { } remaining && remaining <= threshold;

        unsafe float? GetRemainingActionRecast(ActionManager* actionManager, ActionType actionType, uint actionID)
        {
            var recastGroupDetail = actionManager->GetRecastGroupDetail(actionManager->GetRecastGroup((int)actionType, actionID));
            if (recastGroupDetail == null) return null;

            var additionalRecastGroupDetail = actionManager->GetRecastGroupDetail(actionManager->GetAdditionalRecastGroup(actionType, actionID));
            var additionalRecastRemaining = additionalRecastGroupDetail != null && additionalRecastGroupDetail->IsActive ? additionalRecastGroupDetail->Total - additionalRecastGroupDetail->Elapsed : 0;

            if (!recastGroupDetail->IsActive) return additionalRecastRemaining;

            var charges = actionType == ActionType.Action ? ActionManager.GetMaxCharges(actionID, Player.MaxLevel) : 1;
            var recastRemaining = recastGroupDetail->Total / charges - recastGroupDetail->Elapsed;
            return recastRemaining > additionalRecastRemaining ? recastRemaining : additionalRecastRemaining;
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

    public static void OutputLog(ActionType actionType)
    {
        if (actionType == ActionType.Action)
            DuoLog.Information($"You just used: {CombatActions.LastOrDefault().ActionName()} x{LastActionUseCount}");
        else if (actionType == ActionType.Item)
            DuoLog.Information($"You just used: {CombatActions.LastOrDefault().ItemName()}");

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
                if (P.CustomActions.Manager.Actions.TryGetFirst(x => x.Id == actionManager->GetAdjustedActionId(actionId), out var customAct))
                {
                    if (customAct.OnClick != null)
                    {
                        customAct.OnClick();
                        return false;
                    }
                }

                var replacedWith = Service.ActionReplacer.LastActionInvokeFor.ContainsKey(actionId) ? Service.ActionReplacer.LastActionInvokeFor[actionId] : actionId;
                var queuedAct = Service.ActionReplacer.LastActionInvokeFor.ContainsKey(actionManager->QueuedActionId) ? Service.ActionReplacer.LastActionInvokeFor[actionManager->QueuedActionId] : actionManager->QueuedActionId;

                // If the replaced action is a mudra and we're already in a mudra sequence
                // where the base mudra matches, ignore the input.
                if (NIN.MudraSigns.Contains(replacedWith) && NIN.InMudra && NIN.MudraToBase(LastAction) == NIN.MudraToBase(replacedWith))
                    return false;

                // Determine if the queued action conflicts with the current mudra state.
                var queuedProblem = (queuedAct > 0 && queuedAct != NIN.Ninjutsu && !NIN.MudraSigns.Contains(queuedAct) && !NIN.NormalJutsus.Contains(queuedAct)) || queuedAct == LastAction;

                if (NIN.InMudra && (queuedProblem || NIN.MudraUsed(replacedWith)))
                {
                    actionManager->QueuedActionId = 0;
                    return false;
                }

                var disablingReplacingTemp = (mode == ActionManager.UseActionMode.Queue || AutoRotationController.AutorotRaidwiding) && actionId < All.SingleTargetDPS;
                if (disablingReplacingTemp) // This is so we can remove queue suppression
                    Service.ActionReplacer.DisableActionReplacingIfRequired(); // It gets re-enabled at the end of sending. 

                var original = actionId; //Save the original action, do not modify
                var originalTargetId = targetId; //Save the original target, do not modify
                var changedTargetId = targetId; //This will get modified and used elsewhere

                var changed = CheckForChangedTarget(original, ref changedTargetId,
                    out var _); //Passes the original action to the retargeting framework, outputs a targetId and a replaced action

                if (replacedWith >= All.SingleTargetDPS)
                {
                    Svc.Toasts.ShowError("This is a custom action, it does nothing on its own.");
                    return false;
                }

                // If retargeting kicks in, update target ID
                if (changed)
                {
                    var targObj = changedTargetId.GetObject();
                    if (targObj == null || !targObj.IsTargetable || (targObj.IsHostile() && targObj.IsDead))
                        targetId = originalTargetId;
                    else
                        targetId = changedTargetId;
                }

                // Clear any dodgy leftover targets
                if (!Svc.Objects.Any(x => x.GameObjectId == actionManager->QueuedTargetId.Id))
                    actionManager->QueuedTargetId = 0;

                // However, if we have a queued target ID assume that's what we want and not whatever current retargeting is. TODO: Setting?
                if (actionManager->QueuedTargetId.Id != 0)
                    targetId = actionManager->QueuedTargetId.Id;

                var areaTargeted = replacedWith >= 1_000_000 ? false : ActionSheet[replacedWith].TargetArea;

                if (areaTargeted && disablingReplacingTemp) //Ground targets don't hit the send method, so it has to be re-enabled here. Could be re-enabled further down the line if it causes output issues.
                    Service.ActionReplacer.EnableActionReplacingIfRequired();

                var targetObject = targetId.GetObject();
                if (targetObject is null && targetId != 0xE000_0000)
                {
                    Service.ActionReplacer.EnableActionReplacingIfRequired();
                    return UseActionHook.Original(actionManager, actionType, actionId, targetId, extraParam, mode, comboRouteId, outOptAreaTargeted);
                }

                if (changed && !areaTargeted) //Check if the action can be used on the target, and if not revert to original
                    if (!ActionManager.CanUseActionOnTarget(replacedWith,
                            targetObject.Struct()))
                        targetId = originalTargetId;

                // Support Retargeted ground actions
                if ((areaTargeted && changed) || AutoRotationController.WouldLikeToGroundTarget)
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

                    var ret = ActionManager.Instance()->UseActionLocation
                        (actionType, replacedWith, location: &location);

                    Service.ActionReplacer.EnableActionReplacingIfRequired();

                    return ret;
                }

                if (Service.Configuration.OverwriteQueue && actionManager->QueuedActionId != 0 && CanQueueCS(replacedWith))
                    actionManager->QueuedActionId = Service.ActionReplacer.ActionReplacingEnabled ? actionId : replacedWith;

                // Determine if the action will queue according to user settings
                bool willQueue = CanQueueCS(replacedWith) && RemainingGCD > 0;

                // If the action is going to queue, and we've retargeted, update the queued target to match the retargeted target at time of queue
                if (willQueue && changed)
                {
                    Svc.Log.Verbose($"[QueuedTargetUpdate] Updating queued target ID to {Svc.Objects.SearchById(changedTargetId)?.Name}");

                    // Only sets the queued target once if overwrite is not enabled, otherwise will update each button press
                    if (actionManager->QueuedTargetId.Id == 0 || Service.Configuration.OverwriteQueue)
                        actionManager->QueuedTargetId = changedTargetId;
                }

                Svc.Log.Verbose($"[QueuedTargetUpdate] A:{actionManager->QueuedActionId.ActionName()} Q:{Svc.Objects.SearchById(actionManager->QueuedTargetId)?.Name} T:{Svc.Objects.SearchById(targetId)?.Name} M:{mode} W:{willQueue}");

                Svc.Log.Verbose($"[FinalUse] Target changed is {changed}. Using {actionId.ActionName()} ({actionId}) -> {replacedWith.ActionName()} ({replacedWith}) on {(changed ? targetId.GetObject()?.Name : originalTargetId.GetObject()?.Name)} ({(changed ? targetId : originalTargetId):X})");
                var hookResult = changed ? UseActionHook.Original(actionManager, actionType, actionId, targetId, extraParam, mode, comboRouteId, outOptAreaTargeted) :
                    UseActionHook.Original(actionManager, actionType, actionId, originalTargetId, extraParam, mode, comboRouteId, outOptAreaTargeted);

                Service.ActionReplacer.EnableActionReplacingIfRequired();

                // Fallback if the Retargeted ground action couldn't be placed smartly
                if (changed && areaTargeted)
                    ActionManager.Instance()->AreaTargetingExecuteAtObject =
                        targetId;

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
            Service.ActionReplacer.EnableActionReplacingIfRequired();
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
        UseActionLocHook?.Disable();
        CanQueueAction?.Disable();
        ActorControlPacketHook?.Disable();
        OnRecievePacketHook?.Disable();
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

    public record struct PendingHPChange(ulong gameObjectId, int value, bool positiveChange, uint globalSequence = 0);
}