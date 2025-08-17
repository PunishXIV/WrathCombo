using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Frozen;
using System.Linq;
using WrathCombo.Core;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary> Checks if the given preset is enabled. </summary>
    public static bool IsEnabled(Preset preset)
    {
        if ((int)preset < 100)
            return true;

        try
        {
            (string controllers, bool enabled, bool autoMode)? checkControlled = P.UIHelper.PresetControlled(preset);
            bool controlled = checkControlled is not null;
            bool? controlledState = checkControlled?.enabled;

            return controlled
                ? (bool)controlledState!
                : PresetStorage.IsEnabled(preset);
        }
        // IPC is not loaded yet
        catch
        {
            return PresetStorage.IsEnabled(preset);
        }
    }

    /// <summary> Checks if the given preset is not enabled. </summary>
    public static bool IsNotEnabled(Preset preset) => !IsEnabled(preset);

    public static class JobRoles
    {
        private static Lazy<FrozenSet<uint>> MakeRoleSet(int roleId) =>
            new(() => Svc.Data.GetExcelSheet<ClassJob>()!
                .Where(cj => cj.Role == roleId)
                .Select(cj => cj.RowId)
                .ToFrozenSet());

        private static readonly Lazy<FrozenSet<uint>> _tank = MakeRoleSet(1);
        private static readonly Lazy<FrozenSet<uint>> _healer = MakeRoleSet(4);
        private static readonly Lazy<FrozenSet<uint>> _melee = MakeRoleSet(2);
        private static readonly Lazy<FrozenSet<uint>> _ranged = MakeRoleSet(3);

        public static FrozenSet<uint> Tank => _tank.Value;
        public static FrozenSet<uint> Healer => _healer.Value;
        public static FrozenSet<uint> Melee => _melee.Value;
        public static FrozenSet<uint> Ranged => _ranged.Value;
    }
}