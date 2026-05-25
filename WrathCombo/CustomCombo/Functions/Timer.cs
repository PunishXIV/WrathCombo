using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.AutoRotation;
using WrathCombo.Services;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    private static DateTime combatStart = DateTime.Now;
    private static DateTime partyCombat = DateTime.Now;
    private static DateTime? castFinishedAt;
    private static uint castId;
    private static HashSet<uint> onPlayerStatuses = [];

    public static bool PartyInCombatCheck
    {
        get => field;
        set
        {
            if (field != value)
            {
                Svc.Log.Verbose($"Party has {(value ? "entered" : "left")} combat");
                OnPartyCombatChanged?.Invoke(value);
                field = value;
            }
        }
    }

    public delegate void OnCastInterruptedDelegate(uint interruptedAction);
    public static event OnCastInterruptedDelegate? OnCastInterrupted;

    public delegate void OnPartyCombatChangedDelegate(bool state);
    public static event OnPartyCombatChangedDelegate? OnPartyCombatChanged;

    public delegate void OnStatusChangedDelegate(uint statusId, bool onPlayer);
    public static event OnStatusChangedDelegate? OnStatusChanged;

    public static Dictionary<ulong, long> Deadtionary { get; set; } = new();

    /// <summary> Tells the elapsed time since the combat started. </summary>
    /// <returns> Combat time in seconds. </returns>
    public static TimeSpan CombatEngageDuration() => InCombat() ? DateTime.Now - combatStart : TimeSpan.Zero;

    public static TimeSpan PartyEngageDuration() => PartyInCombatCheck ? DateTime.Now - partyCombat : TimeSpan.Zero;

    public static TimeSpan TimeSpentDead(ulong partyMemberObjectId) => TimeSpentDead((uint)partyMemberObjectId);

    public static TimeSpan TimeSpentDead(uint partyMemberObjectId) => Deadtionary.ContainsKey(partyMemberObjectId) ? TimeSpan.FromMilliseconds((Environment.TickCount64 - Deadtionary[partyMemberObjectId])) : TimeSpan.Zero;

    public static void TimerSetup()
    {
        Svc.Condition.ConditionChange += OnCombat;
        Svc.Framework.Update += UpdatePartyTimer;
        Svc.Framework.Update += UpdateDeadtionary;
        Svc.Framework.Update += CheckInterruptedCasts;
        Svc.Framework.Update += CheckStatuses;
    }

    private static readonly HashSet<uint> _currentStatusScratch = new();
    private static readonly List<uint> _removedStatusScratch = new();

    private static void CheckStatuses(IFramework framework)
    {
        if (!Player.Available) return;

        _currentStatusScratch.Clear();
        foreach (var status in LocalPlayer.StatusList)
        {
            if (status.StatusId == 0) continue;
            _currentStatusScratch.Add(status.StatusId);

            if (onPlayerStatuses.Add(status.StatusId))
                OnStatusChanged?.Invoke(status.StatusId, true);
        }

        if (onPlayerStatuses.Count == 0)
            return;

        _removedStatusScratch.Clear();
        foreach (var statusId in onPlayerStatuses)
        {
            if (!_currentStatusScratch.Contains(statusId))
                _removedStatusScratch.Add(statusId);
        }

        for (int i = 0; i < _removedStatusScratch.Count; i++)
        {
            var id = _removedStatusScratch[i];
            OnStatusChanged?.Invoke(id, false);
            onPlayerStatuses.Remove(id);
        }
    }

    private static void CheckInterruptedCasts(IFramework framework)
    {
        if (Player.Available && Player.Object.CurrentCastTime > 0)
        {
            if (castFinishedAt is null)
            {
                castId = Player.Object.CastActionId;
                float timeLeft = ((Player.Object.TotalCastTime - Player.Object.CurrentCastTime) * 1000f) - 500f;
                castFinishedAt = DateTime.Now + TimeSpan.FromMilliseconds(timeLeft);
            }

        }
        else
        {
            if (castFinishedAt is not null)
            {
                if (DateTime.Now < castFinishedAt)
                {
                    OnCastInterrupted?.Invoke(castId);
                    Service.ActionReplacer.EnableActionReplacingIfRequired();
                }
            }

            castFinishedAt = null;
        }
    }

    private static readonly List<ulong> _deadtionaryRemovalScratch = new();

    private static void UpdateDeadtionary(IFramework framework)
    {
        if (!Player.Available) return;

        var dead = DeadPeople;
        for (int i = 0; i < dead.Count; i++)
        {
            var bc = dead[i].BattleChara;
            if (bc is null || !bc.IsDead) continue;
            var id = bc.GameObjectId;
            if (!Deadtionary.ContainsKey(id))
                Deadtionary[id] = Environment.TickCount64;
        }

        _deadtionaryRemovalScratch.Clear();
        foreach (var kvp in Deadtionary)
        {
            var obj = Svc.Objects.SearchById(kvp.Key);
            if (obj == null || !obj.IsDead)
                _deadtionaryRemovalScratch.Add(kvp.Key);
        }
        for (int i = 0; i < _deadtionaryRemovalScratch.Count; i++)
            Deadtionary.Remove(_deadtionaryRemovalScratch[i]);
    }

    private static unsafe void UpdatePartyTimer(IFramework framework)
    {
        if (!Player.Available) return;

        var members = GetPartyMembers();
        bool anyInCombat = false;
        for (int i = 0; i < members.Count; i++)
        {
            var bc = members[i].BattleChara;
            if (bc is not null && bc.Struct()->InCombat)
            {
                anyInCombat = true;
                break;
            }
        }

        if (anyInCombat && !PartyInCombatCheck)
        {
            PartyInCombatCheck = true;
            partyCombat = DateTime.Now;
        }
        else if (!anyInCombat)
        {
            PartyInCombatCheck = false;
        }
    }

    public static void TimerDispose()
    {
        Svc.Condition.ConditionChange -= OnCombat;
        Svc.Framework.Update -= UpdatePartyTimer;
        Svc.Framework.Update -= UpdateDeadtionary;
        Svc.Framework.Update -= CheckInterruptedCasts;
        Svc.Framework.Update -= CheckStatuses;
    }

    internal static void OnCombat(ConditionFlag flag, bool value)
    {
        if (flag == ConditionFlag.InCombat)
        {
            if (value)
            {
                combatStart = DateTime.Now;
                AutoRotationController.Paused = false;
            }
        }
    }

    public static unsafe float CountdownRemaining => MathF.Max(0, AgentCountDownSettingDialog.Instance()->TimeRemaining);

    public static unsafe bool CountdownActive => AgentCountDownSettingDialog.Instance()->Active;
}