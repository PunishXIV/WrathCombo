#region

using System;
using ECommons.Logging;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

#endregion

namespace WrathCombo.Core;

public enum Item
{
    PhoenixDown = 0,
    StatPotion = 1,
    HealingPotion = 2,
}

public enum StatPotionType
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

public enum PotionLevel
{
    Highest = 0,
    TrySecondHighest = 1,
    SecondHighestOnly = 2,
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
    ///       return this.UsePotion(My_PotionType_UserInt,
    ///                             My_PotionLevel_UserInt);
    ///     </code>
    ///     The <c>this</c> is your current combo (so <c>ItemUsage</c> can access the
    ///     <c>Preset</c> property), and the <c>UsePotion</c> method will return
    ///     <see cref="All.Item" />, which will make the combo system try to use the
    ///     potion defined by the two <see cref="UserInt" /> paremeters
    ///     (they are implicitly converted).
    /// </example>
    /// <returns>
    ///     <see cref="All.Item" />
    /// </returns>
    internal static uint UsePotion
    (this CustomCombo combo,
        StatPotionType potionType,
        PotionLevel potionLevel)
    {
        var preset = combo.Preset;

        return All.Item;
    }

    /// <summary>
    ///     This is just a variant of
    ///     <see cref="UsePotion(CustomCombo, StatPotionType, PotionLevel)" />
    ///     that allows for the manual passing of the <c>Preset</c>.
    /// </summary>
    /// <seealso cref="UsePotion(CustomCombo, StatPotionType, PotionLevel)"/>
    internal static uint UsePotion
    (this uint actionID, Preset preset,
        StatPotionType potionType,
        PotionLevel potionLevel)
    {
        if (actionID is not All.Item)
        {
            PluginLog.Error("bad!");
            return All.Item;
        }

        return All.Item;
    }
}