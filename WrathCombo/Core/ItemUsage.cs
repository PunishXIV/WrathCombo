#region

using System;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameHelpers;
using ECommons.Logging;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;

#endregion

namespace WrathCombo.Core;

#region Enums

public enum Item
{
    Item = 0,
    StatPotion = 1,
    ManaPotion = 2, // todo: not yet implemented
    HealingPotion = 3, // todo: not yet implemented
}

public enum ItemType
{
    PhoenixDown = 0,
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
    // todo: not yet implemented
}

public enum PotionLevel
{
    Highest = 0,
    TrySecondHighest = 1,
    SecondHighestOnly = 2,
}

#endregion

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
    ///     Adds a registration to use an Item, optionally on a specified target,
    ///     and returns <see cref="All.Item" /> to indicate that that registration
    ///     should be found.
    /// </summary>
    /// <returns>
    ///     <see cref="All.Item" />
    /// </returns>
    /// <remarks>
    ///     For items, like <see cref="ItemType.PhoenixDown">Phoenix Down</see>, not
    ///     potions.
    /// </remarks>
    internal static uint UseItem
    (this CustomCombo combo,
        ItemType itemType = ItemType.PhoenixDown,
        IGameObject? target = null)
    {
        var preset = combo.Preset;

        if (target is null && itemType is ItemType.PhoenixDown)
            target = SimpleTarget.AnyDeadHealerIfNoneAlive.IfWithinRange(15) ??
                     SimpleTarget.AnyDeadRaiserIfNoneAlive.IfWithinRange(15) ??
                     SimpleTarget.AnyDeadTankIfNoneAlive.IfWithinRange(15) ??
                     SimpleTarget.AnyDeadPartyMember.IfWithinRange(15);
        target ??= Player.Object;

        return All.Item;
    }

    /// <summary>
    ///     Adds a registration to use a stat-boosting potion according to the type
    ///     and level specified, and returns <see cref="All.Item" /> to indicate that
    ///     that registration should be found.
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
    public static uint UsePotion
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
    /// <returns>
    ///     <see cref="All.Item" />
    /// </returns>
    /// <remarks>
    ///     Should only be used if the <see cref="CustomCombo" /> is not
    ///     available, for example in <c>_ActionLogic.cs</c> files.
    /// </remarks>
    /// <seealso cref="UsePotion(CustomCombo, StatPotionType, PotionLevel)"/>
    public static uint UsePotion
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