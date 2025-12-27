#region

using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Lumina.Excel.Sheets;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using WrathCombo.Services;

#endregion

namespace WrathCombo.Combos.PvE.Content;

public class Quests
{
    #region ARC-specific caches

    private static readonly string ArcherButtName =
        Svc.Data.GetExcelSheet<EObjName>().GetRowOrDefault(2000925)?.Singular
            .ToString() ?? string.Empty;

    #endregion

    private static Job Job => Player.Job;

    private static IGameObject? Target => SimpleTarget.HardTarget;

    private static IGameObject? HealTarget =>
        Service.Configuration.RetargetHealingActionsToStack
            ? SimpleTarget.Stack.AllyToHeal
            : null;

    public static bool TryGetQuestActionFix(ref uint actionID)
    {
        if (TryGetCNJFix(ref actionID)) return true;
        if (TryGetARCFix(ref actionID)) return true;

        return false;
    }

    private static bool TryGetCNJFix(ref uint actionID)
    {
        if (Job != Job.CNJ)
            return false;

        #region Level 30 CNJ Quest Fix

        var target = Target.IfFriendly() ?? HealTarget;

        if (Player.Level > 29 &&
            target is { ObjectKind: ObjectKind.EventNpc, BaseId: 1008174 })
        {
            actionID = WHM.Cure;
            return true;
        }

        #endregion

        return false;
    }

    private static bool TryGetARCFix(ref uint actionID)
    {
        if (Job != Job.ARC)
            return false;

        #region Level 5-15 ARC Quest Fix

        if (Player.Level < 30 &&
            ((Target?.Name.TextValue.Equals(ArcherButtName,
                 StringComparison.InvariantCultureIgnoreCase) ?? false) ||
             (SimpleTarget.NearestEnemyTarget?.Name.TextValue.Equals(ArcherButtName,
                 StringComparison.InvariantCultureIgnoreCase) ?? false) ||
             Svc.Objects.Any(x => x.IsTargetable && x.IsWithinRange(30) &&
                                  x.Name.TextValue.Equals(ArcherButtName,
                                      StringComparison.InvariantCultureIgnoreCase))))
        {
            actionID = BRD.HeavyShot;
            return true;
        }

        #endregion

        return false;
    }
}