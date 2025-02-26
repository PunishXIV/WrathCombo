namespace WrathCombo.AutoRotation
{
    public enum DPSRotationMode
    {
        手动模式 = 0,
        最大HP最高 = 1,
        最大HP最低 = 2,
        当前HP最高 = 3,
        当前HP最低 = 4,
        坦克目标 = 5,
        最近优先 = 6,
        最远优先 = 7,
    }

    public enum HealerRotationMode
    {
        手动模式 = 0,
        当前HP最高 = 1,
        当前HP最低 = 2,
        //Self_Priority,
        //Tank_Priority,
        //Healer_Priority,
        //DPS_Priority,
    }
}
