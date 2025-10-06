#region

using System;
using ECommons.Logging;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;

#endregion

namespace WrathCombo.Core;

public enum Item
{
    PhoenixDown = 0,
    StatPotion = 1,
    HealingPotion = 2,
}

public enum StatPotion
{
    Strength = 0,
    Dexterity = 1,
    Vitality = 2,
    Intelligence = 3,
    Mind = 4,
}

public enum HealingPotion
{
}

public class ItemUsage : IDisposable
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    private class ItemUse
    {
    }
}

internal static class ItemUsageExtensions
{
    /// <summary>
    /// </summary>
    /// <example>
    ///     Just return like this in your combo:
    ///     <code>
    ///     if (timeToPot)
    ///       return this.UsePotion();
    ///     </code>
    ///     The <c>this</c> is your current combo (so this can access the
    ///     <c>Preset</c> property), and the <c>UsePotion</c> method will return
    ///     <see cref="All.Item" />, which will make the combo system use the
    ///     potion defined on the preset.
    /// </example>
    /// <returns>
    ///     <see cref="All.Item" />
    /// </returns>
    internal static uint UsePotion(this CustomCombo combo)
    {
        var preset = combo.Preset;

        return All.Item;
    }

    internal static uint UsePotion(this uint actionID, Preset preset)
    {
        if (actionID is not All.Item)
        {
            PluginLog.Error("bad!");
            return All.Item;
        }

        return All.Item;
    }
}