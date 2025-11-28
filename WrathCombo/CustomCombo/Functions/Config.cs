using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.Services;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract class UserData(string configName)
{
    public string ConfigName = configName;

    public static implicit operator string(UserData o) => (o.ConfigName);

    public static Dictionary<string, UserData> MasterList = new();

    public abstract void ResetToDefault();
}

internal class UserFloat : UserData
{
    public float Default;

    public UserFloat(string configName, float defaults = 0f) : base(configName)
    {
        if (!Configuration.CustomFloatValues.ContainsKey(ConfigName))
            Configuration.SetCustomFloatValue(ConfigName, defaults, true);

        Default = defaults;
        MasterList.Add(ConfigName, this);
    }

    public static implicit operator float(UserFloat o) =>
        Configuration.GetCustomFloatValue(o.ConfigName);

    public float Value
    {
        set => Configuration.SetCustomFloatValue(ConfigName, value);
    }

    public override void ResetToDefault() =>
        Configuration.SetCustomFloatValue(ConfigName, Default);
}

internal class UserInt : UserData
{
    public int Default;

    public UserInt(string configName, int defaults = 0) : base(configName)
    {
        if (!Configuration.CustomIntValues.ContainsKey(ConfigName))
            Configuration.SetCustomIntValue(ConfigName, defaults, true);

        Default = defaults;
        MasterList.Add(ConfigName, this);
    }

    public static implicit operator int(UserInt o) =>
        Configuration.GetCustomIntValue(o.ConfigName);

    public int Value
    {
        set => Configuration.SetCustomIntValue(ConfigName, value);
    }

    public override void ResetToDefault() =>
        Configuration.SetCustomIntValue(ConfigName, Default);
}

internal class UserBool : UserData
{
    public bool Default;

    public UserBool(string configName, bool defaults = false) : base(configName)
    {
        if (!Configuration.CustomBoolValues.ContainsKey(ConfigName))
            Configuration.SetCustomBoolValue(ConfigName, defaults, true);

        Default = defaults;
        MasterList.Add(ConfigName, this);
    }

    public static implicit operator bool(UserBool o) => Configuration.GetCustomBoolValue(o.ConfigName);

    public bool Value
    {
        set => Configuration.SetCustomBoolValue(ConfigName, value);
    }

    public override void ResetToDefault() =>
        Configuration.SetCustomBoolValue(this.ConfigName, Default);
}

internal class UserIntArray : UserData
{
    public int[] Default;

    public UserIntArray(string configName, int[]? defaults = null) : base(configName)
    {
        defaults ??= [];
        if (!Configuration.CustomIntArrayValues.ContainsKey(ConfigName))
            Configuration.SetCustomIntArrayValue(ConfigName, defaults);

        Default = defaults;
        MasterList.Add(ConfigName, this);
    }

    #region Built-In Array Members
    public int Count => Configuration.GetCustomIntArrayValue(ConfigName).Length;
    public int IndexOf(int item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i] == item)
                return i;
        }
        return -1;
    }
    public void Clear(int maxValues)
    {
        var array = Configuration.GetCustomIntArrayValue(ConfigName);
        Array.Resize(ref array, maxValues);
        Configuration.SetCustomIntArrayValue(ConfigName, array);
    }
    #endregion

    #region LINQ Members
    public bool Any(Func<int, bool> func) =>
        Configuration.GetCustomIntArrayValue(ConfigName).Any(func);
    public IEnumerable<int> OrderBy<TKey>(Func<int, TKey> keySelector) =>
        Configuration.GetCustomIntArrayValue(ConfigName).OrderBy(keySelector);
    #endregion

    public static implicit operator int[](UserIntArray o) =>
        Configuration.GetCustomIntArrayValue(o.ConfigName);

    public int this[int index]
    {
        get
        {
            if (index < Count)
                return Configuration.GetCustomIntArrayValue(ConfigName)[index];

            var array = Configuration.GetCustomIntArrayValue(ConfigName);
            Array.Resize(ref array, index + 1);
            array[index] = 0;
            return Configuration.SetCustomIntArrayValue(ConfigName, array)[index];
        }
        set
        {
            if (index >= Count) return;

            var array = Configuration.GetCustomIntArrayValue(ConfigName);
            array[index] = value;
            Service.Configuration.Save();
        }
    }

    public override void ResetToDefault() =>
        Configuration.SetCustomIntArrayValue(ConfigName, (int[])Default.Clone());
}

internal class UserBoolArray : UserData
{
    public bool[] Default;

    public UserBoolArray(string configName, bool[]? defaults = null) : base(configName)
    {
        defaults ??= [];
        if (!Configuration.CustomBoolArrayValues.ContainsKey(ConfigName))
            Configuration.SetCustomBoolArrayValue(ConfigName, defaults);

        Default = defaults;
        MasterList.Add(ConfigName, this);
    }

    #region Built-In Array Members
    public int Count => Configuration.GetCustomBoolArrayValue(ConfigName).Length;
    public void Clear(int maxValues)
    {
        var array = Configuration.GetCustomBoolArrayValue(ConfigName);
        Array.Resize(ref array, maxValues);
        Configuration.SetCustomBoolArrayValue(ConfigName, array);
    }
    #endregion

    #region LINQ Members
    public bool Any(Func<bool, bool> func) =>
        Configuration.GetCustomBoolArrayValue(ConfigName).Any(func);
    public bool All(Func<bool, bool> func) =>
        Configuration.GetCustomBoolArrayValue(ConfigName).All(func);
    #endregion

    public static implicit operator bool[](UserBoolArray o) =>
        Configuration.GetCustomBoolArrayValue(o.ConfigName);
    
    public bool this[int index]
    {
        get
        {
            if (index < Count)
                return Configuration.GetCustomBoolArrayValue(ConfigName)[index];

            var array = Configuration.GetCustomBoolArrayValue(ConfigName);
            Array.Resize(ref array, index + 1);
            array[index] = false;
            return Configuration.SetCustomBoolArrayValue(ConfigName, array)[index];
        }
        set
        {
            if (index >= Count) return;

            var array = Configuration.GetCustomBoolArrayValue(ConfigName);
            array[index] = value;
            Service.Configuration.Save();
        }
    }

    public override void ResetToDefault() =>
        Configuration.SetCustomBoolArrayValue(ConfigName, Default);
}