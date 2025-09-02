using System;

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
    
    private class ItemUse
    {
    
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}

internal static class ItemUsageExtensions
{
    internal static uint UsePotion
        (this uint action)
    {
        return 0;
    }
}