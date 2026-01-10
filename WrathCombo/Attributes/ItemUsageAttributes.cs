#region

using System;
using WrathCombo.Core;

#endregion

namespace WrathCombo.Attributes;

public class UsesItem
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class ItemAttribute : Attribute
    {
        internal ItemAttribute(ItemType typeOfItem)
        {
            TypeOfItem = typeOfItem;
        }

        public ItemType TypeOfItem { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class StatPotionAttribute : Attribute
    {
        internal StatPotionAttribute
            (StatPotionType primaryTypeOfStatPotion)
        {
            TypeOfStatPotion = [primaryTypeOfStatPotion];
        }

        internal StatPotionAttribute
        (StatPotionType primaryTypeOfStatPotion,
            StatPotionType secondaryTypeOfStatPotion)
        {
            TypeOfStatPotion =
            [
                primaryTypeOfStatPotion,
                secondaryTypeOfStatPotion
            ];
        }

        public StatPotionType[] TypeOfStatPotion { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class ManaPotionAttribute : Attribute
    {
        internal ManaPotionAttribute
            (ManaPotionType typeOfManaPotion)
        {
            TypeOfManaPotion = typeOfManaPotion;
        }

        public ManaPotionType TypeOfManaPotion { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class HealingPotionAttribute : Attribute
    {
        internal HealingPotionAttribute
            (HealingPotionType typeOfHealingPotion)
        {
            TypeOfHealingPotion = typeOfHealingPotion;
        }

        public HealingPotionType TypeOfHealingPotion { get; }
    }
}