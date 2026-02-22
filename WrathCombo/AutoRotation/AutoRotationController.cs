#region

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WrathCombo.API.Enum;
using WrathCombo.Attributes;
using WrathCombo.Combos.PvE;
using WrathCombo.Combos.PvE.Enums;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Services.IPC_Subscriber;
using WrathCombo.Window.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Data.ActionWatching;
using ActionType = FFXIVClientStructs.FFXIV.Client.Game.ActionType;

#endregion

namespace WrathCombo.AutoRotation;

internal unsafe class AutoRotationController
{
    public static AutoRotationConfigIPCWrapper? cfg;

    public static long HealThrottle = 0;

    static bool _lockedST = false;
    static bool _lockedAoE = false;

    static DateTime? TimeToHeal;

    const float QueryRange = 30f;

    public static bool WouldLikeToGroundTarget;
    public static bool PausedForError;

    public static IGameObject? AutorotHealTarget;

    public AutoRotationController()
    {
        OnPartyCombatChanged += ResetError;
    }

    public void Dispose()
    {
        OnPartyCombatChanged -= ResetError;
    }

    private void ResetError(bool state)
    {
        if (!state)
            PausedForError = false;
    }

    static Func<WrathPartyMember, bool> RezQuery => x =>
        x.BattleChara is not null &&
        x.BattleChara.IsDead &&
        x.BattleChara.IsTargetable &&
        (cfg.HealerSettings.AutoRezOutOfParty || GetPartyMembers().Any(y => y.GameObjectId == x.BattleChara.GameObjectId)) &&
        GetTargetDistance(x.BattleChara) <= QueryRange &&
        !HasStatusEffect(2648, x.BattleChara, true) && // Transcendent Effect
        !HasStatusEffect(148, x.BattleChara, true) && // Raise Effect
        !HasStatusEffect(4263, x.BattleChara, true) && // Raise Denied (OC)
        TimeSpentDead(x.BattleChara.GameObjectId).TotalSeconds > 2;

    public static bool LockedST
    {
        get => _lockedST;
        set
        {
            //if (_lockedST != value)
            //    Svc.Log.Debug($"Locked ST updated to {value}");

            _lockedST = value;
        }
    }
    public static bool LockedAoE
    {
        get => _lockedAoE;
        set
        {
            //if (_lockedAoE != value)
            //    Svc.Log.Debug($"Locked AoE updated to {value}");

            _lockedAoE = value;
        }
    }

    static bool CombatBypass => DPSTargeting.BaseSelection.Any(x => (cfg.BypassQuest && IsQuestMob(x)) || (cfg.BypassFATE && x.Struct()->FateId != 0 && InFATE()));
    static bool NotInCombat => !GetPartyMembers().Any(x => x.BattleChara is not null && x.BattleChara.Struct()->InCombat && !x.IsOutOfPartyNPC) || PartyEngageDuration().TotalSeconds < cfg.CombatDelay;

    private static bool ShouldSkipAutorotation()
    {
        return !cfg.Enabled
               || !Player.Available
               || Player.Object.IsDead
               || IsOccupied()
               || Player.Mounted
               || !EzThrottler.Throttle("Autorot", cfg.Throttler)
               || (cfg.DPSSettings.UnTargetAndDisableForPenalty && PlayerHasActionPenalty())
               || (ActionManager.Instance()->QueuedActionId > 0)
               || PausedForError;
    }

    internal static void Run()
    {
        cfg ??= new AutoRotationConfigIPCWrapper(Service.Configuration.RotationConfig);

        // Early exit for all conditions that should prevent autorotation
        if (ShouldSkipAutorotation())
            return;

        uint _ = 0;
        var autoActions = Presets.GetJobAutorots;

        // Pre-emptive HoT/Shield for healers
        if (cfg.HealerSettings.PreEmptiveHoT && Player.Job is Job.CNJ or Job.WHM or Job.AST)
            PreEmptiveHot();

        if (cfg.HealerSettings.PreEmptiveHoT && Player.Job is Job.SGE or Job.SCH)
            PreEmptiveShield();

        // Bypass buffs logic
        if (cfg.BypassBuffs && NotInCombat)
        {
            if (ProcessAutoActions(autoActions, ref _, false, true))
                return;
        }

        // Only run in combat if required
        if (cfg.InCombatOnly && NotInCombat && !CombatBypass)
            return;

        // Healer logic
        bool isHealer = Player.Object?.Role is CombatRole.Healer;
        var healTarget = isHealer ? AutoRotationHelper.GetSingleTarget(cfg.HealerRotationMode) : null;

        bool aoeheal = isHealer
                       && HealerTargeting.CanAoEHeal()
                       && autoActions.Any(x => x.Key.Attributes()?.AutoAction?.IsHeal == true && x.Key.Attributes()?.AutoAction?.IsAoE == true);

        bool needsHeal = ((healTarget != null
                           && autoActions.Any(x => x.Key.Attributes()?.AutoAction?.IsHeal == true && x.Key.Attributes()?.AutoAction?.IsAoE != true))
                          || aoeheal)
                         && isHealer;

        if (needsHeal && TimeToHeal is null)
            TimeToHeal = DateTime.Now;
        else if (!needsHeal)
            TimeToHeal = null;

        // Check if any healing action is ready
        bool actCheck = autoActions.Any(x =>
        {
            var attr = x.Key.Attributes();
            return attr?.AutoAction?.IsHeal == true && ActionReady(AutoRotationHelper.InvokeCombo(x.Key, attr, ref _));
        });

        bool canHeal = TimeToHeal is not null
                       && (DateTime.Now - TimeToHeal.Value).TotalSeconds >= cfg.HealerSettings.HealDelay
                       && actCheck;

        // Healer cleanse/rez logic
        if (isHealer ||
            (Player.Job is Job.SMN or Job.RDM && cfg.HealerSettings.AutoRezDPSJobs) ||
            OccultCrescent.IsEnabledAndUsable(Preset.Phantom_Chemist_Revive, OccultCrescent.Revive) ||
            Variant.CanRaise())
        {
            if (ActionManager.Instance()->QueuedActionId == RoleActions.Healer.Esuna)
                ActionManager.Instance()->QueuedActionId = 0;

            if ((!needsHeal || GetPartyMembers().Any(x => HasCleansableDoom(x.BattleChara))) && WrathOpener.CurrentOpener?.CurrentState is not
                OpenerState.InOpener)
            {
                if (cfg.HealerSettings.AutoCleanse && isHealer)
                    CleanseParty();

                if (cfg.HealerSettings.AutoRez)
                    RezParty();
            }
        }

        // SGE Kardia logic
        if (Player.Job is Job.SGE && cfg.HealerSettings.ManageKardia)
            UpdateKardiaTarget();

        // Reset locks if no action for 3 seconds
        if (TimeSinceLastAction.TotalSeconds >= 3)
        {
            LockedAoE = false;
            LockedST = false;
        }

        ProcessAutoActions(autoActions, ref _, canHeal, false);
    }

    private static bool ProcessAutoActions(Dictionary<Preset, bool> autoActions, ref uint _, bool canHeal, bool stOnly)
    {
        // Pre-filter and cache attributes to avoid repeated lookups
        var filteredActions = autoActions
            .Select(x => new { Preset = x.Key, Attributes = x.Key.Attributes() })
            .Where(x => x.Attributes is { AutoAction: not null, ReplaceSkill: not null })
            .Where(x => x.Attributes.AutoAction.IsHeal == canHeal)
            .Where(x => !stOnly || x.Attributes.AutoAction.IsAoE == false)
            .OrderByDescending(x => x.Attributes.AutoAction.IsAoE);

        foreach (var entry in filteredActions)
        {
            var attributes = entry.Attributes;
            var action = attributes.AutoAction!;

            // Skip if locked
            if ((action.IsAoE && LockedST) || (!action.IsAoE && LockedAoE))
                continue;

            // Skip if rez invuln is up
            if (!action.IsHeal && HasStatusEffect(418))
                continue;

            uint gameAct = attributes.ReplaceSkill!.ActionIDs.First();
            var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, gameAct, checkCastingActive: false, checkRecastActive: false);

            if (!LevelChecked(gameAct) || status == 581)
                continue;

            if (action.IsHeal)
            {
                AutomateHealing(entry.Preset, attributes, gameAct);
                continue;
            }

            // Tank logic
            if (Player.Object?.GetRole() is CombatRole.Tank)
            {
                AutomateTanking(entry.Preset, attributes, gameAct);
                continue;
            }

            // DPS logic
            if (!action.IsHeal && AutomateDPS(entry.Preset, attributes, gameAct))
                return false;
        }

        return false;
    }

    private static void PreEmptiveHot()
    {
        if (PartyInCombat() || SimpleTarget.FocusTarget is null || (InDuty() && !Svc.DutyState.IsDutyStarted))
            return;

        ushort regenBuff = Player.Job switch
        {
            Job.AST => AST.Buffs.AspectedBenefic,
            Job.CNJ or Job.WHM => WHM.Buffs.Regen,
            _ => 0
        };

        uint regenSpell = Player.Job switch
        {
            Job.AST => AST.AspectedBenefic,
            Job.CNJ or Job.WHM => WHM.Regen,
            _ => 0
        };

        if (regenSpell != 0 && !JustUsed(regenSpell, 4) && SimpleTarget.FocusTarget != null && (!HasStatusEffect(regenBuff, out var regen, SimpleTarget.FocusTarget) || regen?.RemainingTime <= 5f))
        {
            var query = Svc.Objects.Where(x => !x.IsDead && x.IsTargetable && x.IsHostile());
            if (!query.Any())
                return;

            if (query.Min(x => GetTargetDistance(x, SimpleTarget.FocusTarget)) <= QueryRange)
            {
                var spell = ActionManager.Instance()->GetAdjustedActionId(regenSpell).Retarget(SimpleTarget.FocusTarget);

                if (SimpleTarget.FocusTarget.IsDead)
                    return;

                if (!ActionReady(spell))
                    return;

                if (Player.Object is not null && ActionManager.CanUseActionOnTarget(spell, SimpleTarget.FocusTarget.Struct()) && !OutOfRange(spell, Player.Object, SimpleTarget.FocusTarget) && ActionManager.Instance()->GetActionStatus(ActionType.Action, spell) == 0)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, regenSpell);
                    return;
                }
            }
        }
    }

    private static void PreEmptiveShield()
    {
        if (PartyInCombat() || SimpleTarget.FocusTarget is null || (InDuty() && !Svc.DutyState.IsDutyStarted))
            return;

        ushort shieldBuff = Player.Job switch
        {
            Job.SGE => SGE.Buffs.EukrasianDiagnosis,
            Job.SCH => SCH.Buffs.Galvanize,
            _ => 0
        };

        uint shieldSpell = Player.Job switch
        {
            Job.SGE => SGE.EukrasianDiagnosis,
            Job.SCH => SCH.Adloquium,
            _ => 0
        };

        uint prepSpell = Player.Job switch
        {
            Job.SGE => SGE.Eukrasia,
            _ => 0
        };

        if (shieldSpell != 0 && !JustUsed(shieldSpell, 4) && SimpleTarget.FocusTarget != null && (!HasStatusEffect(shieldBuff, out var shield, SimpleTarget.FocusTarget) || shield?.RemainingTime <= 1f))
        {
            if (prepSpell != 0 && !JustUsed(prepSpell, 4) && !HasStatusEffect(SGE.Buffs.Eukrasia))
            {
                var spell = ActionManager.Instance()->GetAdjustedActionId(prepSpell).Retarget(SimpleTarget.FocusTarget);

                if (!ActionReady(prepSpell))
                    return;

                if (ActionManager.Instance()->GetActionStatus(ActionType.Action, spell) == 0)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, prepSpell);
                    return;
                }
            }

            var query = Svc.Objects.Where(x => !x.IsDead && x.IsTargetable && x.IsHostile());
            if (!query.Any())
                return;

            if (query.Min(x => GetTargetDistance(x, SimpleTarget.FocusTarget)) <= QueryRange)
            {
                var spell = ActionManager.Instance()->GetAdjustedActionId(shieldSpell).Retarget(SimpleTarget.FocusTarget);

                if (SimpleTarget.FocusTarget.IsDead)
                    return;

                if (!ActionReady(spell) ||
                    ActionManager.GetAdjustedCastTime(ActionType.Action, spell) > 0 && TimeStoodStill < TimeSpan.FromSeconds(1))
                    return;

                if (Player.Object is not null && ActionManager.CanUseActionOnTarget(spell, SimpleTarget.FocusTarget.Struct()) && !OutOfRange(spell, Player.Object, SimpleTarget.FocusTarget) && ActionManager.Instance()->GetActionStatus(ActionType.Action, spell) == 0)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, shieldSpell);
                    return;
                }
            }
        }
    }

    // Note: Similar to Kardia, because this has its own set of rules but regarding timings I'm not sure if I want to wire this up to retargeting
    private static void RezParty()
    {
        if (HasStatusEffect(418)) return;
        uint resSpell = 0;

        if (OccultCrescent.IsEnabledAndUsable(Preset.Phantom_Chemist_Revive, OccultCrescent.Revive))
        {
            resSpell = OccultCrescent.Revive;
        }
        else if (Variant.CanRaise())
        {
            resSpell = Variant.Raise;
        }
        else
        {
            resSpell = Player.Job switch
            {
                Job.CNJ or Job.WHM => WHM.Raise,
                Job.SCH or Job.SMN => SCH.Resurrection,
                Job.AST => AST.Ascend,
                Job.SGE => SGE.Egeiro,
                Job.RDM => RDM.Verraise,
                _ => 0,
            };
        }

        if (resSpell == 0)
            return;

        IEnumerable<WrathPartyMember> deadPeople = DeadPeople;

        if (cfg.HealerSettings.AutoRezDPSJobsHealersOnly && Player.Job is Job.RDM or Job.SMN)
        {
            deadPeople = deadPeople.Where(x => x.GetRole() is CombatRole.Healer || x.RealJob?.GetJob() is Job.SMN or Job.RDM);
        }

        if (ActionManager.Instance()->QueuedActionId == resSpell)
            ActionManager.Instance()->QueuedActionId = 0;

        if (Player.Object.CurrentMp >= GetResourceCost(resSpell) && ActionReady(resSpell))
        {
            var timeSinceLastRez = TimeSinceLastSuccessfulCast(resSpell);
            if ((timeSinceLastRez != -1f && timeSinceLastRez < 4f) || Player.Object.IsCasting())
                return;

            if (deadPeople.Where(RezQuery).FindFirst(x => x is not null, out var member))
            {
                if (resSpell == OccultCrescent.Revive)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.BattleChara.GameObjectId);
                    return;
                }

                if (resSpell is Variant.Raise)
                {
                    //Try to Swiftcast if Magic DPS
                    if (RoleAttribute.GetRoleFromJob(Player.Job) is JobRole.MagicalDPS)
                    {
                        if (ActionReady(RoleActions.Magic.Swiftcast) && !HasStatusEffect(RDM.Buffs.Dualcast))
                        {
                            if (ActionManager.Instance()->GetActionStatus(ActionType.Action, RoleActions.Magic.Swiftcast) == 0)
                            {
                                ActionManager.Instance()->UseAction(ActionType.Action, RoleActions.Magic.Swiftcast);
                                return;
                            }
                        }
                    }

                    if (HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast) || HasStatusEffect(RDM.Buffs.Dualcast) || !IsMoving())
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.BattleChara.GameObjectId);
                        return;
                    }
                }

                if (Player.Job is Job.RDM)
                {
                    if (ActionReady(RoleActions.Magic.Swiftcast) && !HasStatusEffect(RDM.Buffs.Dualcast))
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, RoleActions.Magic.Swiftcast);
                        return;
                    }

                    if (ActionManager.GetAdjustedCastTime(ActionType.Action, resSpell) == 0)
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.BattleChara.GameObjectId);
                    }

                }
                else
                {
                    if (ActionReady(RoleActions.Magic.Swiftcast))
                    {
                        if (ActionManager.Instance()->GetActionStatus(ActionType.Action, RoleActions.Magic.Swiftcast) == 0)
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, RoleActions.Magic.Swiftcast);
                            return;
                        }
                    }

                    if (!IsMoving() || HasStatusEffect(RoleActions.Magic.Buffs.Swiftcast))
                    {

                        if ((cfg is not null) && ((cfg.HealerSettings.AutoRezRequireSwift && ActionManager.GetAdjustedCastTime(ActionType.Action, resSpell) == 0) || !cfg.HealerSettings.AutoRezRequireSwift))
                        {
                            ActionManager.Instance()->UseAction(ActionType.Action, resSpell, member.BattleChara.GameObjectId);
                        }
                    }
                }
            }
        }
    }

    private static void CleanseParty()
    {
        if (HasStatusEffect(418) || LocalPlayer is not { } || !EzThrottler.Throttle("CleanseThrottle", 50)) return;

        if (SimpleTarget.Stack.AllyToEsuna is IBattleChara memberBC)
        {
            var res = ActionManager.GetActionInRangeOrLoS(Healer.Role.Esuna, LocalPlayer.GameObject(), memberBC.GameObject());
            if (res is 0 or 565)
            {
                Svc.Log.Debug($"Cleansing {memberBC.Name}");
                ActionManager.Instance()->UseAction(ActionType.Action, RoleActions.Healer.Esuna, memberBC.GameObjectId);
            }
        }
    }

    // Note: Not entirely sure what to do when the Kardia standalone retargeting is on since it doesn't follow this ruleset so this will be untouched for now but
    // it is known if it acts funny with the standalone retarget then that's what causes it.
    private static void UpdateKardiaTarget()
    {
        if (HasStatusEffect(418)) return;
        if (!LevelChecked(SGE.Kardia)) return;
        if (CombatEngageDuration().TotalSeconds < 3) return;

        foreach (var member in GetPartyMembers().Where(x => !x.BattleChara.IsDead).OrderByDescending(x => x.BattleChara?.GetRole() is CombatRole.Tank))
        {
            if (cfg.HealerSettings.KardiaTanksOnly && member.BattleChara?.GetRole() is not CombatRole.Tank &&
                !HasStatusEffect(3615, member.BattleChara, true)) continue;

            var enemiesTargeting = Svc.Objects.Count(x => x.IsTargetable && x.IsHostile() && x.TargetObjectId == member.BattleChara.GameObjectId);
            if (enemiesTargeting > 0 && !HasStatusEffect(SGE.Buffs.Kardion, member.BattleChara))
            {
                ActionManager.Instance()->UseAction(ActionType.Action, SGE.Kardia, member.BattleChara.GameObjectId);
                return;
            }
        }

    }

    private static bool AutomateDPS(Preset preset, PresetStorage.PresetAttributes attributes, uint gameAct)
    {
        var mode = cfg.DPSRotationMode;
        if (attributes.AutoAction!.IsAoE)
        {
            return AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
        }
        else
        {
            return AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
        }
    }

    private static bool AutomateTanking(Preset preset, PresetStorage.PresetAttributes attributes, uint gameAct)
    {
        var mode = cfg.DPSRotationMode;
        if (attributes.AutoAction!.IsAoE)
        {
            return AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
        }
        else
        {
            return AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
        }
    }

    private static bool AutomateHealing(Preset preset, PresetStorage.PresetAttributes attributes, uint gameAct)
    {
        var mode = cfg.HealerRotationMode;
        if (Player.Object?.IsCasting() is true) return false;
        if (Environment.TickCount64 < HealThrottle) return false;

        if (attributes.AutoAction!.IsAoE)
        {
            var ret = AutoRotationHelper.ExecuteAoE(mode, preset, attributes, gameAct);
            return ret;
        }
        else
        {
            var ret = AutoRotationHelper.ExecuteST(mode, preset, attributes, gameAct);
            return ret;
        }
    }

    public static class AutoRotationHelper
    {
        public static IGameObject? GetSingleTarget(Enum rotationMode)
        {
            if (rotationMode is DPSRotationMode dpsmode)
            {
                if (Player.Object?.Role is CombatRole.Tank)
                {
                    IGameObject? target = dpsmode switch
                    {
                        DPSRotationMode.Manual => Svc.Targets.Target,
                        DPSRotationMode.Highest_Max => TankTargeting.GetHighestMaxTarget(),
                        DPSRotationMode.Lowest_Max => TankTargeting.GetLowestMaxTarget(),
                        DPSRotationMode.Highest_Current => TankTargeting.GetHighestCurrentTarget(),
                        DPSRotationMode.Lowest_Current => TankTargeting.GetLowestCurrentTarget(),
                        DPSRotationMode.Tank_Target => Svc.Targets.Target,
                        DPSRotationMode.Nearest => DPSTargeting.GetNearestTarget(),
                        DPSRotationMode.Furthest => DPSTargeting.GetFurthestTarget(),
                        _ => Svc.Targets.Target,
                    };
                    return target;
                }
                else
                {
                    IGameObject? target = dpsmode switch
                    {
                        DPSRotationMode.Manual => Svc.Targets.Target,
                        DPSRotationMode.Highest_Max => DPSTargeting.GetHighestMaxTarget(),
                        DPSRotationMode.Lowest_Max => DPSTargeting.GetLowestMaxTarget(),
                        DPSRotationMode.Highest_Current => DPSTargeting.GetHighestCurrentTarget(),
                        DPSRotationMode.Lowest_Current => DPSTargeting.GetLowestCurrentTarget(),
                        DPSRotationMode.Tank_Target => DPSTargeting.GetTankTarget(),
                        DPSRotationMode.Nearest => DPSTargeting.GetNearestTarget(),
                        DPSRotationMode.Furthest => DPSTargeting.GetFurthestTarget(),
                        _ => Svc.Targets.Target,
                    };
                    return target;
                }
            }
            if (rotationMode is HealerRotationMode healermode)
            {
                if (Player.Object?.Role != CombatRole.Healer) return null;
                IGameObject? target = healermode switch
                {
                    HealerRotationMode.Manual => HealerTargeting.ManualTarget(),
                    HealerRotationMode.Highest_Current => HealerTargeting.GetHighestCurrent(),
                    HealerRotationMode.Lowest_Current => HealerTargeting.GetLowestCurrent(),
                    _ => HealerTargeting.ManualTarget(),
                };
                AutorotHealTarget = target;
                return target;
            }

            return null;
        }

        public static bool ExecuteAoE(Enum mode, Preset preset, PresetStorage.PresetAttributes attributes, uint gameAct)
        {
            if (LocalPlayer is not { } player)
                return false;

            if (attributes.AutoAction!.IsHeal)
            {
                LockedAoE = false;
                LockedST = false;

                uint outAct = OriginalHook(InvokeCombo(preset, attributes, ref gameAct, Player.Object));
                if (!ActionReady(outAct))
                    return false;

                var canQueue = outAct.ActionAttackType() is { } type && (type is ActionAttackType.Ability || type is not ActionAttackType.Ability && RemainingGCD <= cfg.QueueWindow);
                if (!canQueue)
                    return false;

                if (HealerTargeting.CanAoEHeal(outAct))
                {
                    var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                    bool orbwalking = cfg.OrbwalkerIntegration && OrbwalkerIPC.CanOrbwalk;
                    if (TimeMoving.TotalMilliseconds > 0 && castTime > 0 && !orbwalking)
                        return false;

                    var targetId = player.GameObjectId;
                    var changed = CheckForChangedTarget(gameAct, ref targetId, out var replacedWith);
                    WouldLikeToGroundTarget = ActionSheet[outAct].TargetArea;
                    var ret = ActionManager.Instance()->UseAction(ActionType.Action, Service.Configuration.ActionChanging ? gameAct : outAct, targetId);
                    WouldLikeToGroundTarget = false;

                    return true;
                }
            }
            else
            {
                var target = !cfg.DPSSettings.AoEIgnoreManual && cfg.DPSRotationMode == DPSRotationMode.Manual ?
                    Svc.Targets.Target : DPSTargeting.BaseSelection.MaxBy(x => NumberOfEnemiesInRange(OriginalHook(gameAct), x, true));

                if (!NIN.InMudra)
                {
                    var st = GetSingleTarget(mode);
                    var maxHit = NumberOfEnemiesInRange(OriginalHook(gameAct), target, true);
                    var singleTargetModeTarget = NumberOfEnemiesInRange(OriginalHook(gameAct), st, true);

                    if (singleTargetModeTarget >= maxHit)
                        target = st;

                    if (cfg.DPSSettings.DPSAoETargets == null || maxHit < cfg.DPSSettings.DPSAoETargets)
                    {
                        LockedAoE = false;
                        return false;
                    }
                    else
                    {
                        LockedAoE = true;
                        LockedST = false;
                    }
                }
                OverrideTarget = target;
                uint outAct = OriginalHook(InvokeCombo(preset, attributes, ref gameAct, target));
                if (outAct is All.SavageBlade) return true;
                if (!ActionReady(outAct))
                {
                    OverrideTarget = null;
                    return false;
                }

                var canQueue = outAct.ActionAttackType() is { } type && ((type is ActionAttackType.Ability && AnimationLock == 0) || (type is not ActionAttackType.Ability && RemainingGCD <= cfg.QueueWindow));
                if (!canQueue)
                {
                    OverrideTarget = null;
                    return false;
                }
                var sheet = ActionSheet[outAct];
                var targetsHostile = sheet.CanTargetHostile;

                bool switched = SwitchOnDChole(attributes, outAct, ref target);
                var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
                bool orbwalking = cfg.OrbwalkerIntegration && OrbwalkerIPC.CanOrbwalk;
                if (TimeMoving.TotalMilliseconds > 0 && castTime > 0 && !orbwalking)
                {
                    OverrideTarget = null;
                    return false;
                }

                if (cfg.DPSSettings.DPSAlwaysHardTarget)
                    Svc.Targets.Target = target;

                var canUseSelf = sheet.CanTargetSelf;
                var areaTargeted = ActionSheet[outAct].TargetArea;
                var acRangeCheck = ActionManager.GetActionInRangeOrLoS(outAct, player.GameObject(), target is null ? player.GameObject() : target.Struct());
                var inRange = acRangeCheck is 0 or 565 || canUseSelf || areaTargeted;

                if (targetsHostile && target is not null)
                {
                    Svc.GameConfig.TryGet(Dalamud.Game.Config.UiControlOption.AutoFaceTargetOnAction, out uint original);
                    Svc.GameConfig.Set(Dalamud.Game.Config.UiControlOption.AutoFaceTargetOnAction, 1);
                    Vector3 pos = new(Player.Object.Position.X, Player.Object.Position.Y, Player.Object.Position.Z);
                    ActionManager.Instance()->AutoFaceTargetPosition(&pos, target.GameObjectId);
                    Svc.GameConfig.Set(Dalamud.Game.Config.UiControlOption.AutoFaceTargetOnAction, original);
                }

                if (inRange)
                {
                    //Chance target of target.GameObjectID can be null
                    var targetId = (targetsHostile && target != null) || switched ? target.GameObjectId : canUseSelf ? player.GameObjectId : 0xE000_0000;
                    var changed = CheckForChangedTarget(gameAct, ref targetId, out var replacedWith);
                    WouldLikeToGroundTarget = areaTargeted;
                    var ret = ActionManager.Instance()->UseAction(ActionType.Action, Service.Configuration.ActionChanging ? gameAct : outAct, targetId);
                    WouldLikeToGroundTarget = false;
                    if (NIN.MudraSigns.Contains(outAct))
                        _lockedAoE = true;
                    else
                        _lockedAoE = false;

                    return true;
                }

            }
            return false;
        }

        public static bool ExecuteST(Enum mode, Preset preset, PresetStorage.PresetAttributes attributes, uint gameAct)
        {
            if (LocalPlayer is not { } player)
                return false;

            var target = GetSingleTarget(mode);
            OverrideTarget = target;
            var outAct = OriginalHook(InvokeCombo(preset, attributes, ref gameAct, target));
            if (!CanQueue(outAct))
            {
                return false;
            }

            bool switched = SwitchOnDChole(attributes, outAct, ref target);
            if (outAct is DNC.ClosedPosition && DNC.DancePartnerResolver() is IBattleChara dp)
                target = dp;

            var canUseSelf = NIN.MudraSigns.Contains(outAct)
                ? target is not null && target.IsHostile()
                : ActionManager.CanUseActionOnTarget(outAct, Player.GameObject);

            var blockedSelfBuffs = GetCooldown(outAct).CooldownTotal >= 5;

            if (cfg.InCombatOnly && NotInCombat && !CombatBypass && !(canUseSelf && cfg.BypassBuffs && !blockedSelfBuffs))
            {
                OverrideTarget = null;
                return false;
            }

            if (target is null && !canUseSelf)
            {
                OverrideTarget = null;
                return false;
            }

            var areaTargeted = ActionSheet[outAct].TargetArea;
            var canUseTarget = target is not null && ActionManager.CanUseActionOnTarget(outAct, target.Struct());

            var acRangeCheck = ActionManager.GetActionInRangeOrLoS(outAct, player.GameObject(), target is null ? player.GameObject() : target.Struct());
            var inRange = acRangeCheck is 0 or 565 || canUseSelf;

            var canUse = (canUseSelf || canUseTarget || areaTargeted) && outAct.ActionAttackType() is { } type && ((type is ActionAttackType.Ability && AnimationLock == 0) || (type is not ActionAttackType.Ability && RemainingGCD <= cfg.QueueWindow));
            var isHeal = attributes.AutoAction!.IsHeal;

            if ((!isHeal && cfg.DPSSettings.DPSAlwaysHardTarget && mode is not DPSRotationMode.Manual) || (isHeal && cfg.HealerSettings.HealerAlwaysHardTarget && mode is not HealerRotationMode.Manual))
                Svc.Targets.Target = target;

            var castTime = ActionManager.GetAdjustedCastTime(ActionType.Action, outAct);
            bool orbwalking = cfg.OrbwalkerIntegration && OrbwalkerIPC.CanOrbwalk;
            if (TimeMoving.TotalMilliseconds > 0 && castTime > 0 && !orbwalking)
            {
                OverrideTarget = null;
                return false;
            }

            if (canUse && (inRange || areaTargeted))
            {
                var targetId = canUseTarget || areaTargeted ? target.GameObjectId : canUseSelf ? player.GameObjectId : 0xE000_0000;
                var changed = CheckForChangedTarget(gameAct, ref targetId, out var replacedWith);
                WouldLikeToGroundTarget = ActionSheet[outAct].TargetArea;
                var ret = ActionManager.Instance()->UseAction(ActionType.Action, Service.Configuration.ActionChanging ? gameAct : outAct, targetId);
                WouldLikeToGroundTarget = false;

                if (NIN.MudraSigns.Contains(outAct))
                    _lockedST = true;
                else
                    _lockedST = false;

                return true;
            }

            return false;
        }

        private static bool SwitchOnDChole(PresetStorage.PresetAttributes attributes, uint outAct, ref IGameObject? newtarget)
        {
            if (outAct is SGE.Druochole && !attributes.AutoAction!.IsHeal)
            {
                if (GetPartyMembers()
                    .Where(x => !x.BattleChara.IsDead &&
                                x.BattleChara.IsTargetable &&
                                GetTargetDistance(x.BattleChara) <= QueryRange &&
                                IsInLineOfSight(x.BattleChara))
                    .OrderBy(x => GetTargetHPPercent(x.BattleChara))
                    .Select(x => x.BattleChara)
                    .TryGetFirst(out newtarget))
                {
                    return true;
                }
            }

            return false;
        }

        public static uint InvokeCombo(Preset preset, PresetStorage.PresetAttributes attributes, ref uint originalAct, IGameObject? optionalTarget = null)
        {
            if (attributes.ReplaceSkill is null) return originalAct;
            var outAct = attributes.ReplaceSkill.ActionIDs.FirstOrDefault();
            foreach (var actToCheck in attributes.ReplaceSkill.ActionIDs)
            {
                var customCombo = Service.ActionReplacer.CustomCombos.FirstOrDefault(x => x.Preset == preset);
                if (customCombo != null)
                {
                    if (customCombo.TryInvoke(actToCheck, out var changedAct, optionalTarget))
                    {
                        originalAct = actToCheck;
                        outAct = changedAct;
                        Service.ActionReplacer.LastActionInvokeFor[actToCheck] = outAct;
                        break;
                    }
                }
            }

            return outAct;
        }
    }

    public class DPSTargeting
    {
        private static bool Query(IGameObject x) =>
            x is IBattleChara chara &&
            !chara.IsDead &&
            chara.IsTargetable &&
            chara.IsHostile() &&
            IsInRange(chara, InBossEncounter() && cfg.DPSSettings.IgnoreRangeInBoss ? 100f : cfg.DPSSettings.MaxDistance) &&
            GetTargetHeightDifference(chara) <= (InBossEncounter() && cfg.DPSSettings.IgnoreRangeInBoss ? 100f : cfg.DPSSettings.MaxDistance) &&
            !TargetIsInvincible(chara) &&
            !Service.Configuration.IgnoredNPCs.ContainsKey(chara.BaseId) &&
            ((cfg.DPSSettings.OnlyAttackInCombat && chara.Struct()->InCombat) || !cfg.DPSSettings.OnlyAttackInCombat) &&
            IsInLineOfSight(chara);

        public static IEnumerable<IGameObject> BaseSelection => Svc.Objects.Any(x => Query(x) && IsPriority(x))
            ? Svc.Objects.Where(x => Query(x) && IsPriority(x))
            : Svc.Objects.Where(x => Query(x));

        private static bool IsPriority(IGameObject x)
        {
            if (x is IBattleChara chara)
            {
                bool isFate = cfg.DPSSettings.FATEPriority && x.Struct()->FateId != 0 && InFATE();
                bool isQuest = cfg.DPSSettings.QuestPriority && IsQuestMob(x);

                return isFate || isQuest;
            }
            return false;
        }

        public static bool IsCombatPriority(IGameObject x)
        {
            if (x is IBattleChara chara)
            {
                if (!cfg.DPSSettings.PreferNonCombat) return true;
                bool inCombat = cfg.DPSSettings.PreferNonCombat && !chara.Struct()->InCombat;
                return inCombat;
            }
            return false;
        }

        public static IGameObject? GetTankTarget()
        {
            var tank = GetPartyMembers().FirstOrDefault(x => x.BattleChara?.GetRole() == CombatRole.Tank || HasStatusEffect(3615, x.BattleChara, true));
            if (tank == null)
                return null;

            return tank.BattleChara.TargetObject;
        }

        public static IGameObject? GetNearestTarget()
        {
            return BaseSelection
                .OrderByDescending(x => IsCombatPriority(x))
                .ThenBy(x => GetTargetDistance(x))
                .FirstOrDefault();
        }

        public static IGameObject? GetFurthestTarget()
        {
            return BaseSelection
                .OrderByDescending(x => IsCombatPriority(x))
                .ThenByDescending(x => GetTargetDistance(x))
                .FirstOrDefault();
        }

        public static IGameObject? GetLowestCurrentTarget()
        {
            return BaseSelection
                .OrderByDescending(x => IsCombatPriority(x))
                .ThenBy(x => GetTargetCurrentHP(x))
                .FirstOrDefault();
        }

        public static IGameObject? GetHighestCurrentTarget()
        {
            return BaseSelection
                .OrderByDescending(x => IsCombatPriority(x))
                .ThenByDescending(x => GetTargetCurrentHP(x))
                .FirstOrDefault();
        }

        public static IGameObject? GetLowestMaxTarget()
        {

            return BaseSelection
                .OrderByDescending(x => IsCombatPriority(x))
                .ThenBy(x => GetTargetMaxHP(x))
                .ThenBy(x => GetTargetHPPercent(x))
                .FirstOrDefault();
        }

        public static IGameObject? GetHighestMaxTarget()
        {
            return BaseSelection
                .OrderByDescending(x => IsCombatPriority(x))
                .ThenByDescending(x => GetTargetMaxHP(x))
                .ThenBy(x => GetTargetHPPercent(x))
                .FirstOrDefault();
        }
    }

    public static class HealerTargeting
    {
        internal static IGameObject? ManualTarget()
        {
            if (Svc.Targets.Target == null) return null;
            var t = Svc.Targets.Target;
            bool goodToHeal = t is IBattleChara &&
                              t.IsFriendly() &&
                              GetTargetHPPercent(t) <=
                              (TargetHasExcog(t) ? cfg.HealerSettings.SingleTargetExcogHPP :
                                  TargetHasRegen(t) ? cfg.HealerSettings.SingleTargetRegenHPP :
                                  cfg.HealerSettings.SingleTargetHPP);
            if (goodToHeal && !t.IsHostile())
            {
                return t;
            }
            return null;
        }
        internal static IGameObject? GetHighestCurrent()
        {
            if (GetPartyMembers().Count == 0) return Player.Object;
            var target = GetPartyMembers()
                .Where(x => !x.BattleChara.IsDead &&
                            x.BattleChara.IsTargetable &&
                            GetTargetDistance(x.BattleChara) <= QueryRange &&
                            !TargetHasImmortality(x.BattleChara) &&
                            GetTargetHPPercent(x.BattleChara) <=
                            (TargetHasExcog(x.BattleChara) ? cfg.HealerSettings.SingleTargetExcogHPP :
                                TargetHasRegen(x.BattleChara) ? cfg.HealerSettings.SingleTargetRegenHPP :
                                cfg.HealerSettings.SingleTargetHPP) &&
                            IsInLineOfSight(x.BattleChara))
                .OrderBy(x => TargetHasTrueInvuln(x.BattleChara))
                .ThenByDescending(x => GetTargetHPPercent(x.BattleChara))
                .FirstOrDefault();
            return target?.BattleChara;
        }

        internal static IGameObject? GetLowestCurrent()
        {
            if (GetPartyMembers().Count == 0) return Player.Object;
            var target = GetPartyMembers()
                .Where(x => !x.BattleChara.IsDead &&
                            x.BattleChara.IsTargetable &&
                            GetTargetDistance(x.BattleChara) <= QueryRange &&
                            !TargetHasImmortality(x.BattleChara) &&
                            GetTargetHPPercent(x.BattleChara) <=
                            (TargetHasExcog(x.BattleChara) ? cfg.HealerSettings.SingleTargetExcogHPP :
                                TargetHasRegen(x.BattleChara) ? cfg.HealerSettings.SingleTargetRegenHPP :
                                cfg.HealerSettings.SingleTargetHPP) &&
                            IsInLineOfSight(x.BattleChara))
                .OrderBy(x => TargetHasTrueInvuln(x.BattleChara))
                .ThenBy(x => GetTargetHPPercent(x.BattleChara))
                .FirstOrDefault();
            return target?.BattleChara;
        }

        internal static bool CanAoEHeal(uint outAct = 0)
        {
            int memberCount;
            try
            {
                var members = GetPartyMembers()
                    .Where(x => x.BattleChara is not null &&
                                !x.BattleChara.IsDead &&
                                x.BattleChara.IsTargetable &&
                                !x.IsOutOfPartyNPC &&
                                (outAct == 0
                                    ? GetTargetDistance(x.BattleChara) <= 20f
                                    : InActionRange(outAct, x.BattleChara)) &&
                                GetTargetHPPercent(x.BattleChara) <= cfg.HealerSettings.AoETargetHPP);
                memberCount = members.Count();
            }
            catch { memberCount = 0; }

            if (memberCount < cfg.HealerSettings.AoEHealTargetCount)
                return false;

            return true;
        }

        private static bool TargetHasRegen(IGameObject? target)
        {
            if (target is null) return false;
            return JobID switch
            {
                Job.AST => HasStatusEffect(AST.Buffs.AspectedBenefic, target),
                Job.WHM => HasStatusEffect(WHM.Buffs.Regen, target),
                _ => false,
            };
        }
        private static bool TargetHasExcog(IGameObject? target)
        {
            return target is not null && HasStatusEffect(SCH.Buffs.Excogitation, target, true);
        }
        /// Used to skip the healing of tanks that are invuln but still receive damage
        private static bool TargetHasImmortality(IGameObject? target)
        {
            if (target is null) return false;

            return GetStatusEffectRemainingTime(DRK.Buffs.LivingDead, target, true) >= 3 ||
                   GetStatusEffectRemainingTime(DRK.Buffs.WalkingDead, target, true) >= 5 ||
                   GetStatusEffectRemainingTime(WAR.Buffs.Holmgang, target, true) >= 5;
        }
        /// Used to de-prioritize (not skip) the healing of invuln tanks
        private static bool TargetHasTrueInvuln(IGameObject? target)
        {
            if (target is null) return false;

            return GetStatusEffectRemainingTime(GNB.Buffs.Superbolide, target) >= 5 ||
                   GetStatusEffectRemainingTime(PLD.Buffs.HallowedGround, target) >= 5;
        }
    }

    public static class TankTargeting
    {
        public static IGameObject? GetLowestCurrentTarget()
        {
            return DPSTargeting.BaseSelection
                .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenBy(x => GetTargetCurrentHP(x))
                .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
        }

        public static IGameObject? GetHighestCurrentTarget()
        {
            return DPSTargeting.BaseSelection
                .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenByDescending(x => GetTargetCurrentHP(x))
                .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
        }

        public static IGameObject? GetLowestMaxTarget()
        {
            var t = DPSTargeting.BaseSelection
                .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenBy(x => GetTargetMaxHP(x))
                .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();

            return t;
        }

        public static IGameObject? GetHighestMaxTarget()
        {
            return DPSTargeting.BaseSelection
                .OrderByDescending(x => DPSTargeting.IsCombatPriority(x))
                .ThenByDescending(x => x.TargetObject?.GameObjectId != Player.Object?.GameObjectId)
                .ThenByDescending(x => GetTargetMaxHP(x))
                .ThenBy(x => GetTargetHPPercent(x)).FirstOrDefault();
        }
    }
}