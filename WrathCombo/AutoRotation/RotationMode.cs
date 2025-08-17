namespace WrathCombo.AutoRotation;

public enum DPSRotationMode
{
    Manual = 0,
    Highest_Max = 1,
    Lowest_Max = 2,
    Highest_Current = 3,
    Lowest_Current = 4,
    Tank_Target = 5,
    Nearest = 6,
    Furthest = 7,
}

public enum HealerRotationMode
{
    Manual = 0,
    Highest_Current = 1,
    Lowest_Current = 2,
    //Self_Priority,
    //Tank_Priority,
    //Healer_Priority,
    //DPS_Priority,
}