﻿using System.Linq;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Data.ActionWatching;

namespace WrathCombo.Extensions;

internal static class UIntExtensions
{
    internal static bool LevelChecked(this uint value) => CustomComboFunctions.LevelChecked(value);

    internal static bool TraitLevelChecked(this uint value) => CustomComboFunctions.TraitLevelChecked(value);

    internal static string ActionName(this uint value) => CustomComboFunctions.GetActionName(value);

    internal static int Role(this uint value) => CustomComboFunctions.JobIDs.JobIDToRole(value);

    internal static ActionAttackType ActionAttackType(this uint value) => (ActionAttackType)ActionSheet[value].ActionCategory.RowId;
    
    internal static bool IsGroundTargeted(this uint value) =>
        ActionSheet.FirstOrDefault(x => x.Value.RowId == value).Value.TargetArea;
    
    internal static bool IsEnemyTargetable(this uint value) =>
        ActionSheet.FirstOrDefault(x => x.Value.RowId == value).Value.CanTargetHostile;
    
    internal static bool IsFriendlyTargetable(this uint value) =>
        ActionSheet.FirstOrDefault(x => x.Value.RowId == value).Value.CanTargetAlly;
}

internal static class UShortExtensions
{
    internal static string StatusName(this ushort value) => StatusCache.GetStatusName(value);
}